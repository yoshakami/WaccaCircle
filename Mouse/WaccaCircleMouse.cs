using LilyConsole;
using System;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;
using System.IO;
using System.Diagnostics;
using System.Threading;

class Program
{
    static int LAG_DELAY = 50; // tweak between 0ms and 100ms to reduce CPU usage or increase responsiveness
    static long axis_max = 10;
    static string ahk = Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "ahk");
    // Import SendInput from user32.dll
    [DllImport("user32.dll", SetLastError = true)]
    public static extern uint SendInput(uint nInputs, ref INPUT pInputs, int cbSize);

    // Constants for input types
    public const int INPUT_MOUSE = 0;
    public const uint MOUSEEVENTF_LEFTDOWN = 0x0002;
    public const uint MOUSEEVENTF_LEFTUP = 0x0004;
    public const uint MOUSEEVENTF_MOVE = 0x0001;
    public const uint MOUSEEVENTF_RIGHTDOWN = 0x0008; // Right button down
    public const uint MOUSEEVENTF_RIGHTUP = 0x0010;   // Right button up
    public const uint MOUSEEVENTF_MIDDLEDOWN = 0x0020; // Middle button down
    public const uint MOUSEEVENTF_MIDDLEUP = 0x0040;   // Middle button up


    [StructLayout(LayoutKind.Sequential)]
    public struct INPUT
    {
        public uint type;
        public MOUSEINPUT mi;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MOUSEINPUT
    {
        public int dx;
        public int dy;
        public uint mouseData;
        public uint dwFlags;
        public uint time;
        public IntPtr dwExtraInfo;
    }

    private static int Y(double v)
    {
        // Use Cos to calculate the Y position and shift it to the range [-axis_max, axis_max]
        return (int)((Math.Cos(v * Math.PI / 30)) * axis_max);
    }

    private static int X(double v)
    {
        // Use -Sin to calculate the X position and shift it to the range [-axis_max, axis_max]
        return (int)((-Math.Sin(v * Math.PI / 30)) * axis_max);
    }

    // yup. defining an array is faster than doing maths
    // efficiency.
    static int[][] axes =
    {      //       x axis    y-axis   1-12  13-16  17-20  21-22  23-24  25-32
        new int[] { X(0.5),   Y(0.5),   12,   16,    17,    21,    23,    25},    // 0  top circle
        new int[] { X(1.5),   Y(1.5),   12,   16,    17,    21,    23,    25},    // 1
        new int[] { X(2.5),   Y(2.5),   12,   16,    17,    21,    23,    25},    // 2
        new int[] { X(3.5),   Y(3.5),   11,   16,    17,    21,    23,    25},    // 3
        new int[] { X(4.5),   Y(4.5),   11,   16,    17,    21,    23,    25},    // 4
        new int[] { X(5.5),   Y(5.5),   11,   16,    17,    21,    23,    25},    // 5
        new int[] { X(6.5),   Y(6.5),   11,   16,    17,    21,    23,    25},    // 6
        new int[] { X(7.5),   Y(7.5),   11,   16,    17,    21,    23,    25},    // 7
        new int[] { X(8.5),   Y(8.5),   10,   16,    20,    21,    23,    26},    // 8
        new int[] { X(9.5),   Y(9.5),   10,   16,    20,    21,    23,    26},    // 9
        new int[] { X(10.5),  Y(10.5),  10,   16,    20,    21,    23,    26},    // 10
        new int[] { X(11.5),  Y(11.5),  10,   16,    20,    21,    23,    26},    // 11
        new int[] { X(12.5),  Y(12.5),  10,   16,    20,    21,    23,    26},    // 12
        new int[] { X(13.5),  Y(13.5),   9,   16,    20,    21,    23,    26},    // 13
        new int[] { X(14.5),  Y(14.5),   9,   16,    20,    21,    23,    26},    // 14  left
        new int[] { X(15.5),  Y(15.5),   9,   15,    20,    22,    23,    26},    // 15  left 
        new int[] { X(16.5),  Y(16.5),   9,   15,    20,    22,    23,    27},    // 16
        new int[] { X(17.5),  Y(17.5),   9,   15,    20,    22,    23,    27},    // 17
        new int[] { X(18.5),  Y(18.5),   8,   15,    20,    22,    23,    27},    // 18
        new int[] { X(19.5),  Y(19.5),   8,   15,    20,    22,    23,    27},    // 19
        new int[] { X(20.5),  Y(20.5),   8,   15,    20,    22,    23,    27},    // 20
        new int[] { X(21.5),  Y(21.5),   8,   15,    20,    22,    23,    27},    // 21
        new int[] { X(22.5),  Y(22.5),   8,   15,    20,    22,    23,    27},    // 22
        new int[] { X(23.5),  Y(23.5),   7,   15,    19,    22,    23,    28},    // 23
        new int[] { X(24.5),  Y(24.5),   7,   15,    19,    22,    23,    28},    // 24
        new int[] { X(25.5),  Y(25.5),   7,   15,    19,    22,    23,    28},    // 25
        new int[] { X(26.5),  Y(26.5),   7,   15,    19,    22,    23,    28},    // 26
        new int[] { X(27.5),  Y(27.5),   7,   15,    19,    22,    23,    28},    // 27
        new int[] { X(28.5),  Y(28.5),   6,   15,    19,    22,    23,    28},    // 28
        new int[] { X(29.5),  Y(29.5),   6,   15,    19,    22,    23,    28},    // 29  bottom
        new int[] { X(30.5),  Y(30.5),   6,   14,    19,    22,    24,    29},    // 30  bottom
        new int[] { X(31.5),  Y(31.5),   6,   14,    19,    22,    24,    29},    // 31
        new int[] { X(32.5),  Y(32.5),   6,   14,    19,    22,    24,    29},    // 32
        new int[] { X(33.5),  Y(33.5),   5,   14,    19,    22,    24,    29},    // 33
        new int[] { X(34.5),  Y(34.5),   5,   14,    19,    22,    24,    29},    // 34
        new int[] { X(35.5),  Y(35.5),   5,   14,    19,    22,    24,    29},    // 35
        new int[] { X(36.5),  Y(36.5),   5,   14,    19,    22,    24,    29},    // 36
        new int[] { X(37.5),  Y(37.5),   5,   14,    19,    22,    24,    30},    // 37
        new int[] { X(38.5),  Y(38.5),   4,   14,    18,    22,    24,    30},    // 38
        new int[] { X(39.5),  Y(39.5),   4,   14,    18,    22,    24,    30},    // 39
        new int[] { X(40.5),  Y(40.5),   4,   14,    18,    22,    24,    30},    // 40
        new int[] { X(41.5),  Y(41.5),   4,   14,    18,    22,    24,    30},    // 41
        new int[] { X(42.5),  Y(42.5),   4,   14,    18,    22,    24,    30},    // 42
        new int[] { X(43.5),  Y(43.5),   3,   14,    18,    22,    24,    30},    // 43
        new int[] { X(44.5),  Y(44.5),   3,   14,    18,    22,    24,    31},    // 44  right
        new int[] { X(45.5),  Y(45.5),   3,   13,    18,    21,    24,    31},    // 45  right
        new int[] { X(46.5),  Y(46.5),   3,   13,    18,    21,    24,    31},    // 46
        new int[] { X(47.5),  Y(47.5),   3,   13,    18,    21,    24,    31},    // 47
        new int[] { X(48.5),  Y(48.5),   2,   13,    18,    21,    24,    31},    // 48
        new int[] { X(49.5),  Y(49.5),   2,   13,    18,    21,    24,    31},    // 49
        new int[] { X(50.5),  Y(50.5),   2,   13,    18,    21,    24,    31},    // 50
        new int[] { X(51.5),  Y(51.5),   2,   13,    18,    21,    24,    31},    // 51
        new int[] { X(52.5),  Y(52.5),   2,   13,    18,    21,    24,    32},    // 52
        new int[] { X(53.5),  Y(53.5),   1,   13,    17,    21,    24,    32},    // 53
        new int[] { X(54.5),  Y(54.5),   1,   13,    17,    21,    24,    32},    // 54
        new int[] { X(55.5),  Y(55.5),   1,   13,    17,    21,    24,    32},    // 55
        new int[] { X(56.5),  Y(56.5),   1,   13,    17,    21,    24,    32},    // 56
        new int[] { X(57.5),  Y(57.5),   1,   13,    17,    21,    24,    32},    // 57
        new int[] { X(58.5),  Y(58.5),  12,   13,    17,    21,    24,    32},    // 58
        new int[] { X(59.5),  Y(59.5),  12,   13,    17,    21,    24,    32},    // 59  top circle
    };

    [STAThread] // Required for SendKeys
    static void Main()
    {
        /*
         * 
                    // Send the string "Hello, World!" to the active application
                    SendKeys.SendWait("Hello, World!");

                    // Optionally, you can send an Enter key after the message
                    SendKeys.SendWait("{ENTER}");
        */
        while (true)
        {
            try
            {
                Mouse();
            }
            catch (Exception e)
            {
                Console.WriteLine("vvv---------- Message -----------vvv");
                Console.WriteLine(e.Message);
                Console.WriteLine("vvv---------- StackTrace --------vvv");
                Console.WriteLine(e.StackTrace);
                if (e.InnerException != null)
                {
                    Console.WriteLine("vvv---------- InnerException Message --------vvv");
                    Console.WriteLine(e.InnerException.Message);
                }
                Console.WriteLine("vvv---------- Source ------------vvv");
                Console.WriteLine(e.Source);
                Console.WriteLine("vvv---------- TargetSite --------vvv");
                Console.WriteLine(e.TargetSite);
                Console.WriteLine("vvv---------- HelpLink --------vvv");
                Console.WriteLine(e.HelpLink);
            }
            Thread.Sleep(1000);
        }
    }
        
     // end Main()
    static void Mouse()
    {
        Point startPos = Cursor.Position;
        Point endPos;
        var controller = new TouchController();
        controller.Initialize();
        Console.CursorVisible = false;
        Console.WriteLine("Starting touch streams!");
        controller.StartTouchStream();
        Console.WriteLine("Started!");
        bool[] button_pressed = Enumerable.Repeat(false, 25).ToArray();
        bool[] button_pressed_on_loop = Enumerable.Repeat(false, 25).ToArray();
        while (true)
        {
            System.Threading.Thread.Sleep(LAG_DELAY);
            controller.GetTouchData();
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 60; j++)
                {
                    if (controller.touchData[i, j])  // if the circle if touched
                    {
                        if (i > 1)
                        {
                            startPos = Cursor.Position;
                            endPos = new Point(startPos.X + axes[j][0], startPos.Y - axes[j][1]);
                            Cursor.Position = endPos;
                        }
                        else
                        {
                            button_pressed_on_loop[axes[j][4]] = true;
                            if (button_pressed[axes[j][4]])
                            {
                                continue;
                            }
                            button_pressed[axes[j][4]] = true;
                            if (axes[j][4] == 17) // (top)
                            {
                                if (File.Exists(Path.Combine(ahk, $"moused.ahk")))
                                {
                                    Process.Start(Path.Combine(ahk, $"moused.ahk"));
                                }
                                else
                                {
                                    Console.WriteLine($"failed to find " + Path.Combine(ahk, $"moused.ahk"));
                                }
                            }
                            else if (axes[j][4] == 18) // right
                            {
                                // right click
                                // Simulate mouse down (right button press)
                                INPUT mouseInput = new INPUT();
                                mouseInput.type = INPUT_MOUSE;
                                mouseInput.mi.dwFlags = MOUSEEVENTF_RIGHTDOWN;
                                SendInput(1, ref mouseInput, Marshal.SizeOf(mouseInput));

                            }
                            else if (axes[j][4] == 19) // bottom
                            {
                                // Simulate mouse down (middle button press)
                                INPUT mouseInput = new INPUT();
                                mouseInput.type = INPUT_MOUSE;
                                mouseInput.mi.dwFlags = MOUSEEVENTF_MIDDLEDOWN;
                                SendInput(1, ref mouseInput, Marshal.SizeOf(mouseInput));
                            }
                            else if (axes[j][4] == 20) // left
                            {
                                // left click
                                // Simulate mouse down (left button press)
                                INPUT mouseInput = new INPUT();
                                mouseInput.type = INPUT_MOUSE;
                                mouseInput.mi.dwFlags = MOUSEEVENTF_LEFTDOWN;
                                SendInput(1, ref mouseInput, Marshal.SizeOf(mouseInput));
                            } // end of else if clicked on inner left
                        } // end of if touched on inner circle
                    }  // end of if circle touched
                } // end lane circle loop
            }  // end circle layer loop
            for (uint i = 17; i < 21; i++)
            {
                if (button_pressed[i] && !button_pressed_on_loop[i])
                {
                    if (i == 17) // (top)
                    {
                        if (File.Exists(Path.Combine(ahk, $"mouseu.ahk")))
                        {
                            Process.Start(Path.Combine(ahk, $"mouseu.ahk"));
                        }
                        else
                        {
                            Console.WriteLine($"failed to find " + Path.Combine(ahk, $"mouseu.ahk"));
                        }
                    }
                    else if (i == 18) // right
                    {
                        // right click
                        // Simulate mouse down (right button press)
                        INPUT mouseInput = new INPUT();
                        mouseInput.type = INPUT_MOUSE;
                        mouseInput.mi.dwFlags = MOUSEEVENTF_RIGHTUP;
                        SendInput(1, ref mouseInput, Marshal.SizeOf(mouseInput));

                    }
                    else if (i == 19) // bottom
                    {
                        // Simulate mouse down (middle button press)
                        INPUT mouseInput = new INPUT();
                        mouseInput.type = INPUT_MOUSE;
                        mouseInput.mi.dwFlags = MOUSEEVENTF_MIDDLEUP;
                        SendInput(1, ref mouseInput, Marshal.SizeOf(mouseInput));
                    }
                    else if (i == 20) // left
                    {
                        // left click
                        // Simulate mouse down (left button Release)
                        INPUT mouseInput = new INPUT();
                        mouseInput.type = INPUT_MOUSE;
                        mouseInput.mi.dwFlags = MOUSEEVENTF_LEFTUP;
                        SendInput(1, ref mouseInput, Marshal.SizeOf(mouseInput));
                    } // end of else if clicked on inner left
                    button_pressed[i] = false;
                }
                button_pressed_on_loop[i] = false;
            } // end for buttons 17 to 20
        }  // end while(true)
    }
}
