using LilyConsole;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Input;

namespace WaccaKeyBind
{
    internal class Program
    {

        // Import ShowWindow from user32.dll
        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        // Import GetConsoleWindow to retrieve console handle
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetConsoleWindow();

        // Constant for minimizing the window
        private const int SW_MINIMIZE = 6;

        static string ahk = Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "ahk");
        static long axis_max = 32767;

        [STAThread]
        public static void Main(string[] args)
        {
            /*
            LilyConsole.ActiveSegment = 
            LilyConsole.Command
            LilyConsole.LightColor
            LilyConsole.LightController
            LilyConsole.LightFrame 
            LilyConsole.TouchCommand;
            LilyConsole.TouchController;
            LilyConsole.TouchManager;
            LilyConsole.VFDController; */
            //LilyConsole.TouchController = new LilyConsole.TouchController();
            // Initialize vJoy interface

            // Get the console window handle
            IntPtr consoleWindow = GetConsoleWindow();

            // Minimize the console window
            ShowWindow(consoleWindow, SW_MINIMIZE);

            try
            {
                TouchCombinedTest();
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
            return;
        }



        public static void TouchCombinedTest()
        {
            /*
             * 
            double rx_step = (rx_max - rx_min) / 30;
            double ry_step = (ry_max - ry_min) / 30;
            double sl0_step = (sl0_max - sl0_min) / 2;
            double sl1_mid = (sl1_max - sl1_min) / 2;

             * // Set joystick X axis to midpoint
        joystick.SetAxis((maxValue - minValue) / 2, deviceId, HID_USAGES.HID_USAGE_X);
        
        // Set joystick Y axis to maximum
        joystick.SetAxis(maxValue, deviceId, HID_USAGES.HID_USAGE_Y);

        // Simulate button presses
        joystick.SetBtn(true, deviceId, 1); // Press button 1
        joystick.SetBtn(false, deviceId, 1); // Release button 1

             */

            // yup. defining an array is faster than doing maths
            // efficiency.
            int[][] axes =
            {      //       x axis    y-axis   1-12  13-16  17-20  21-22  23-24  25-32
                new int[] { X(0.5),   Y(0.5),   12,   16,    17,    21,    23,    32},    // 0  top circle
                new int[] { X(1.5),   Y(1.5),   12,   16,    17,    21,    23,    32},    // 1
                new int[] { X(2.5),   Y(2.5),   12,   16,    17,    21,    23,    32},    // 2
                new int[] { X(3.5),   Y(3.5),   11,   16,    17,    21,    23,    32},    // 3
                new int[] { X(4.5),   Y(4.5),   11,   16,    17,    21,    23,    32},    // 4
                new int[] { X(5.5),   Y(5.5),   11,   16,    17,    21,    23,    32},    // 5
                new int[] { X(6.5),   Y(6.5),   11,   16,    17,    21,    23,    32},    // 6
                new int[] { X(7.5),   Y(7.5),   11,   16,    17,    21,    23,    32},    // 7
                new int[] { X(8.5),   Y(8.5),   10,   16,    20,    21,    23,    31},    // 8
                new int[] { X(9.5),   Y(9.5),   10,   16,    20,    21,    23,    31},    // 9
                new int[] { X(10.5),  Y(10.5),  10,   16,    20,    21,    23,    31},    // 10
                new int[] { X(11.5),  Y(11.5),  10,   16,    20,    21,    23,    31},    // 11
                new int[] { X(12.5),  Y(12.5),  10,   16,    20,    21,    23,    31},    // 12
                new int[] { X(13.5),  Y(13.5),   9,   16,    20,    21,    23,    31},    // 13
                new int[] { X(14.5),  Y(14.5),   9,   16,    20,    21,    23,    31},    // 14  left
                new int[] { X(15.5),  Y(15.5),   9,   15,    20,    22,    23,    31},    // 15  left 
                new int[] { X(16.5),  Y(16.5),   9,   15,    20,    22,    23,    30},    // 16
                new int[] { X(17.5),  Y(17.5),   9,   15,    20,    22,    23,    30},    // 17
                new int[] { X(18.5),  Y(18.5),   8,   15,    20,    22,    23,    30},    // 18
                new int[] { X(19.5),  Y(19.5),   8,   15,    20,    22,    23,    30},    // 19
                new int[] { X(20.5),  Y(20.5),   8,   15,    20,    22,    23,    30},    // 20
                new int[] { X(21.5),  Y(21.5),   8,   15,    20,    22,    23,    30},    // 21
                new int[] { X(22.5),  Y(22.5),   8,   15,    20,    22,    23,    30},    // 22
                new int[] { X(23.5),  Y(23.5),   7,   15,    19,    22,    23,    29},    // 23
                new int[] { X(24.5),  Y(24.5),   7,   15,    19,    22,    23,    29},    // 24
                new int[] { X(25.5),  Y(25.5),   7,   15,    19,    22,    23,    29},    // 25
                new int[] { X(26.5),  Y(26.5),   7,   15,    19,    22,    23,    29},    // 26
                new int[] { X(27.5),  Y(27.5),   7,   15,    19,    22,    23,    29},    // 27
                new int[] { X(28.5),  Y(28.5),   6,   15,    19,    22,    23,    29},    // 28
                new int[] { X(29.5),  Y(29.5),   6,   15,    19,    22,    23,    29},    // 29  bottom
                new int[] { X(30.5),  Y(30.5),   6,   14,    19,    22,    24,    28},    // 30  bottom
                new int[] { X(31.5),  Y(31.5),   6,   14,    19,    22,    24,    28},    // 31
                new int[] { X(32.5),  Y(32.5),   6,   14,    19,    22,    24,    28},    // 32
                new int[] { X(33.5),  Y(33.5),   5,   14,    19,    22,    24,    28},    // 33
                new int[] { X(34.5),  Y(34.5),   5,   14,    19,    22,    24,    28},    // 34
                new int[] { X(35.5),  Y(35.5),   5,   14,    19,    22,    24,    28},    // 35
                new int[] { X(36.5),  Y(36.5),   5,   14,    19,    22,    24,    28},    // 36
                new int[] { X(37.5),  Y(37.5),   5,   14,    19,    22,    24,    27},    // 37
                new int[] { X(38.5),  Y(38.5),   4,   14,    18,    22,    24,    27},    // 38
                new int[] { X(39.5),  Y(39.5),   4,   14,    18,    22,    24,    27},    // 39
                new int[] { X(40.5),  Y(40.5),   4,   14,    18,    22,    24,    27},    // 40
                new int[] { X(41.5),  Y(41.5),   4,   14,    18,    22,    24,    27},    // 41
                new int[] { X(42.5),  Y(42.5),   4,   14,    18,    22,    24,    27},    // 42
                new int[] { X(43.5),  Y(43.5),   3,   14,    18,    22,    24,    27},    // 43
                new int[] { X(44.5),  Y(44.5),   3,   14,    18,    22,    24,    26},    // 44  right
                new int[] { X(45.5),  Y(45.5),   3,   13,    18,    21,    24,    26},    // 45  right
                new int[] { X(46.5),  Y(46.5),   3,   13,    18,    21,    24,    26},    // 46
                new int[] { X(47.5),  Y(47.5),   3,   13,    18,    21,    24,    26},    // 47
                new int[] { X(48.5),  Y(48.5),   2,   13,    18,    21,    24,    26},    // 48
                new int[] { X(49.5),  Y(49.5),   2,   13,    18,    21,    24,    26},    // 49
                new int[] { X(50.5),  Y(50.5),   2,   13,    18,    21,    24,    26},    // 50
                new int[] { X(51.5),  Y(51.5),   2,   13,    18,    21,    24,    26},    // 51
                new int[] { X(52.5),  Y(52.5),   2,   13,    18,    21,    24,    25},    // 52
                new int[] { X(53.5),  Y(53.5),   1,   13,    17,    21,    24,    25},    // 53
                new int[] { X(54.5),  Y(54.5),   1,   13,    17,    21,    24,    25},    // 54
                new int[] { X(55.5),  Y(55.5),   1,   13,    17,    21,    24,    25},    // 55
                new int[] { X(56.5),  Y(56.5),   1,   13,    17,    21,    24,    25},    // 56
                new int[] { X(57.5),  Y(57.5),   1,   13,    17,    21,    24,    25},    // 57
                new int[] { X(58.5),  Y(58.5),  12,   13,    17,    21,    24,    25},    // 58
                new int[] { X(59.5),  Y(59.5),  12,   13,    17,    21,    24,    25},    // 59  top circle
            };


            var controller = new TouchController();
            controller.Initialize();
            Console.CursorVisible = false;
            Console.WriteLine("Starting touch streams!");
            controller.StartTouchStream();
            Console.WriteLine("Started!");
            /* bool[] button_pressed = { false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, };  // 48 times false
            bool[] button_pressed_on_loop = { false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, };  // 48 times false */
            bool[] button_pressed = Enumerable.Repeat(false, 49).ToArray();  // 48 + 1 since I made my table start at 1
            bool[] button_pressed_on_loop = Enumerable.Repeat(false, 49).ToArray();
            bool[] keydown = Enumerable.Repeat(false, 49).ToArray();
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            Console.WriteLine(desktopPath);
            sbyte n;
            const sbyte u = -16;
            const sbyte w = 28;
            sbyte x = 0;
            string key = "key";
            while (true)
            {
                Thread.Sleep(50); // 0ms uses 35% CPU while 5ms uses 4% CPU.
                controller.GetTouchData();

                for (int i = 0; i < 4; i++)
                {
                    for (int j = 0; j < 60; j++)
                    {
                        if (controller.touchData[i, j])  // if the circle if touched
                        {
                            if (i > 1)  // RXY is only on the two outer layers, i==2 and i==3
                            {
                                for (int k = 4; k < 5; k++)  // parse axes columns
                                {
                                    if (!button_pressed[axes[j][k] + w])  // starts at 17 + 28 = 45
                                    {
                                        button_pressed[axes[j][k] + w] = true;
                                        if (File.Exists(Path.Combine(ahk, $"arrow{axes[j][k] + u}d.ahk")))  // starts at 17 + -16 = 1
                                        {

                                            Process.Start(Path.Combine(ahk, $"arrow{axes[j][k] + u}d.ahk")); // ends at 20 + -16 = 4
                                            keydown[axes[j][k] + w] = true;
                                        }
                                        else
                                        {
                                            Console.WriteLine($"failed to find " + Path.Combine(ahk, $"arrow{axes[j][k] + u}d.ahk"));
                                        }
                                    }
                                    
                                    button_pressed_on_loop[axes[j][k] + w] = true;  // ends at  20 + 28 = 48
                                }
                            }
                            else  // parse the 2 inner layers
                            {
                                for (int k = 2; k < 3; k++)
                                {
                                    if (Keyboard.IsKeyDown(Key.Left)) // launch 1 to 12
                                    {
                                        n = 0;
                                        x = 0;
                                        key = "";
                                    }
                                    else if (Keyboard.IsKeyDown(Key.Right))  // launch 12 to 24
                                    {
                                        n = 12;
                                        x = 12;
                                        key = "";
                                    }
                                    else if (Keyboard.IsKeyDown(Key.Up))  // launch 24 to 32
                                    {
                                        k = 7;
                                        n = 0;
                                        x = 0;
                                        key = "";
                                    }
                                    else  // launch key1 to key12 (or 33 to 44 but unused)
                                    {
                                        n = 0;
                                        x = 32;
                                        key = "key";
                                    }
                                    // launch 1 to 32.lnk if it exists
                                    if (File.Exists(Path.Combine(desktopPath, $"{axes[j][k] + x}.lnk")))
                                    {
                                        button_pressed_on_loop[axes[j][k] + x] = true;
                                        if (!button_pressed[axes[j][k] + x])
                                        {
                                            button_pressed[axes[j][k] + x] = true;
                                            Process.Start(Path.Combine(desktopPath, $"{axes[j][k] + x}.lnk"));
                                        }
                                    }
                                    else  // triggered if lnk doesn't exist
                                    {
                                        button_pressed_on_loop[axes[j][k] + x] = true;
                                        if (!button_pressed[axes[j][k] + x])
                                        {
                                            if (File.Exists(Path.Combine(ahk, $"{key}{axes[j][k] + n}d.ahk")))
                                            {
                                                Process.Start(Path.Combine(ahk, $"{key}{axes[j][k] + n}d.ahk"));
                                            }
                                            else
                                            {
                                                Console.WriteLine($"failed to find " + Path.Combine(ahk, $"{key}{axes[j][k] + n}d.ahk"));
                                            }
                                            button_pressed[axes[j][k] + x] = true;
                                            keydown[axes[j][k] + x] = true;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                for (uint i = 0; i < 49; i++)
                {
                    if (button_pressed[i] && !button_pressed_on_loop[i])
                    {
                        if (keydown[i])
                        {
                            if (i > 44)
                            {
                                if (File.Exists(Path.Combine(ahk, $"arrow{i - 44}u.ahk")))  // starts at 45 + -44 = 1
                                {

                                    Process.Start(Path.Combine(ahk, $"arrow{i - 44}u.ahk")); // ends at 48 + -44 = 4
                                }
                                else
                                {
                                    Console.WriteLine($"failed to find " + Path.Combine(ahk, $"arrow{i - 44}u.ahk"));
                                }
                            }
                            else if (i > 32)
                            {
                                if (File.Exists(Path.Combine(ahk, $"key{i - 32}u.ahk")))  // starts at 33 + -32 = 1
                                {

                                    Process.Start(Path.Combine(ahk, $"key{i - 32}u.ahk")); // ends at 44 + -32 = 12
                                }
                                else
                                {
                                    Console.WriteLine($"failed to find " + Path.Combine(ahk, $"key{i - 32}u.ahk"));
                                }
                            }
                            else
                            {
                                if (File.Exists(Path.Combine(ahk, $"{i}u.ahk")))
                                {
                                    Process.Start(Path.Combine(ahk, $"{i}u.ahk"));
                                }
                                else
                                {
                                    Console.WriteLine($"failed to find " + Path.Combine(ahk, $"{i}u.ahk"));
                                }
                            }
                            keydown[i] = false;
                        }
                        button_pressed[i] = false;
                    }
                    button_pressed_on_loop[i] = false;
                }
            }
        }
        private static int Y(double v)
        {
            // Use Cos to calculate the Y position and shift it to the range [0, axis_max]
            return (int)((Math.Cos(v * Math.PI / 30) + 1) / 2 * axis_max);
        }

        private static int X(double v)
        {
            // Use -Sin to calculate the X position and shift it to the range [0, axis_max]
            return (int)((-Math.Sin(v * Math.PI / 30) + 1) / 2 * axis_max);
        }
    }
}