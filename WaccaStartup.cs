using LilyConsole;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

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

            while (true)
            {
                TouchCombinedTest();
                // Arrows(); // send up, down, left, or right key depending on where you touch
            }
        }



        public static void TouchCombinedTest()
        {
            double x_mid = 0;
            double y_mid = 0;
            double x_step = 0;
            double y_step = 0;
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
            // (not sure about the smooth transition from minus to plus)
            int[][] axes =
            {  //   x axis (unused here)         y-axis (unused here)     1-12  13-16  17-20  21-22  23-24
    new int[] { (int)(x_mid - x_step * 1),     (int)(y_mid + y_step * 15), 12,   16,    17,    21,    23},    // 0  top circle
    new int[] { (int)(x_mid - x_step * 2),     (int)(y_mid + y_step * 14), 12,   16,    17,    21,    23},    // 1
    new int[] { (int)(x_mid - x_step * 3),     (int)(y_mid + y_step * 13), 12,   16,    17,    21,    23},    // 2
    new int[] { (int)(x_mid - x_step * 4),     (int)(y_mid + y_step * 12), 11,   16,    17,    21,    23},    // 3
    new int[] { (int)(x_mid - x_step * 5),     (int)(y_mid + y_step * 11), 11,   16,    17,    21,    23},    // 4
    new int[] { (int)(x_mid - x_step * 6),     (int)(y_mid + y_step * 10), 11,   16,    17,    21,    23},    // 5
    new int[] { (int)(x_mid - x_step * 7),     (int)(y_mid + y_step * 9),  11,   16,    17,    21,    23},    // 6
    new int[] { (int)(x_mid - x_step * 8),     (int)(y_mid + y_step * 8),  11,   16,    17,    21,    23},    // 7
    new int[] { (int)(x_mid - x_step * 9),     (int)(y_mid + y_step * 7),  10,   16,    20,    21,    23},    // 8
    new int[] { (int)(x_mid - x_step * 10),    (int)(y_mid + y_step * 6),  10,   16,    20,    21,    23},    // 9
    new int[] { (int)(x_mid - x_step * 11),    (int)(y_mid + y_step * 5),  10,   16,    20,    21,    23},    // 10
    new int[] { (int)(x_mid - x_step * 12),    (int)(y_mid + y_step * 4),  10,   16,    20,    21,    23},    // 11
    new int[] { (int)(x_mid - x_step * 13),    (int)(y_mid + y_step * 3),  10,   16,    20,    21,    23},    // 12
    new int[] { (int)(x_mid - x_step * 14),    (int)(y_mid + y_step * 2),   9,   16,    20,    21,    23},    // 13
    new int[] { (int)(x_mid - x_step * 15),    (int)(y_mid + y_step * 1),   9,   16,    20,    21,    23},    // 14  left
    new int[] { (int)(x_mid - x_step * 15),    (int)(y_mid - y_step * 1),   9,   15,    20,    22,    23},    // 15  left 
    new int[] { (int)(x_mid - x_step * 14),    (int)(y_mid - y_step * 2),   9,   15,    20,    22,    23},    // 16
    new int[] { (int)(x_mid - x_step * 13),    (int)(y_mid - y_step * 3),   9,   15,    20,    22,    23},    // 17
    new int[] { (int)(x_mid - x_step * 12),    (int)(y_mid - y_step * 4),   8,   15,    20,    22,    23},    // 18
    new int[] { (int)(x_mid - x_step * 11),    (int)(y_mid - y_step * 5),   8,   15,    20,    22,    23},    // 19
    new int[] { (int)(x_mid - x_step * 10),    (int)(y_mid - y_step * 6),   8,   15,    20,    22,    23},    // 20
    new int[] { (int)(x_mid - x_step * 09),    (int)(y_mid - y_step * 7),   8,   15,    20,    22,    23},    // 21
    new int[] { (int)(x_mid - x_step * 08),    (int)(y_mid - y_step * 8),   8,   15,    20,    22,    23},    // 22
    new int[] { (int)(x_mid - x_step * 07),    (int)(y_mid - y_step * 9),   7,   15,    19,    22,    23},    // 23
    new int[] { (int)(x_mid - x_step * 06),    (int)(y_mid - y_step * 10),  7,   15,    19,    22,    23},    // 24
    new int[] { (int)(x_mid - x_step * 05),    (int)(y_mid - y_step * 11),  7,   15,    19,    22,    23},    // 25
    new int[] { (int)(x_mid - x_step * 04),    (int)(y_mid - y_step * 12),  7,   15,    19,    22,    23},    // 26
    new int[] { (int)(x_mid - x_step * 03),    (int)(y_mid - y_step * 13),  7,   15,    19,    22,    23},    // 27
    new int[] { (int)(x_mid - x_step * 02),    (int)(y_mid - y_step * 14),  6,   15,    19,    22,    23},    // 28
    new int[] { (int)(x_mid - x_step * 01),    (int)(y_mid - y_step * 15),  6,   15,    19,    22,    23},    // 29  bottom
    new int[] { (int)(x_mid + x_step * 01),    (int)(y_mid - y_step * 15),  6,   14,    19,    22,    24},    // 30  bottom
    new int[] { (int)(x_mid + x_step * 02),    (int)(y_mid - y_step * 14),  6,   14,    19,    22,    24},    // 31
    new int[] { (int)(x_mid + x_step * 03),    (int)(y_mid - y_step * 13),  6,   14,    19,    22,    24},    // 32
    new int[] { (int)(x_mid + x_step * 04),    (int)(y_mid - y_step * 12),  5,   14,    19,    22,    24},    // 33
    new int[] { (int)(x_mid + x_step * 05),    (int)(y_mid - y_step * 11),  5,   14,    19,    22,    24},    // 34
    new int[] { (int)(x_mid + x_step * 06),    (int)(y_mid - y_step * 10),  5,   14,    19,    22,    24},    // 35
    new int[] { (int)(x_mid + x_step * 07),    (int)(y_mid - y_step * 9),   5,   14,    19,    22,    24},    // 36
    new int[] { (int)(x_mid + x_step * 08),    (int)(y_mid - y_step * 8),   5,   14,    19,    22,    24},    // 37
    new int[] { (int)(x_mid + x_step * 09),    (int)(y_mid - y_step * 7),   4,   14,    18,    22,    24},    // 38
    new int[] { (int)(x_mid + x_step * 10),    (int)(y_mid - y_step * 6),   4,   14,    18,    22,    24},    // 39
    new int[] { (int)(x_mid + x_step * 11),    (int)(y_mid - y_step * 5),   4,   14,    18,    22,    24},    // 40
    new int[] { (int)(x_mid + x_step * 12),    (int)(y_mid - y_step * 4),   4,   14,    18,    22,    24},    // 41
    new int[] { (int)(x_mid + x_step * 13),    (int)(y_mid - y_step * 3),   4,   14,    18,    22,    24},    // 42
    new int[] { (int)(x_mid + x_step * 14),    (int)(y_mid - y_step * 2),   3,   14,    18,    22,    24},    // 43
    new int[] { (int)(x_mid + x_step * 15),    (int)(y_mid - y_step * 1),   3,   14,    18,    22,    24},    // 44  right
    new int[] { (int)(x_mid + x_step * 15),    (int)(y_mid + y_step * 1),   3,   13,    18,    21,    24},    // 45  right
    new int[] { (int)(x_mid + x_step * 14),    (int)(y_mid + y_step * 2),   3,   13,    18,    21,    24},    // 46
    new int[] { (int)(x_mid + x_step * 13),    (int)(y_mid + y_step * 3),   3,   13,    18,    21,    24},    // 47
    new int[] { (int)(x_mid + x_step * 12),    (int)(y_mid + y_step * 4),   2,   13,    18,    21,    24},    // 48
    new int[] { (int)(x_mid + x_step * 11),    (int)(y_mid + y_step * 5),   2,   13,    18,    21,    24},    // 49
    new int[] { (int)(x_mid + x_step * 10),    (int)(y_mid + y_step * 6),   2,   13,    18,    21,    24},    // 50
    new int[] { (int)(x_mid + x_step * 09),    (int)(y_mid + y_step * 7),   2,   13,    18,    21,    24},    // 51
    new int[] { (int)(x_mid + x_step * 08),    (int)(y_mid + y_step * 8),   2,   13,    18,    21,    24},    // 52
    new int[] { (int)(x_mid + x_step * 07),    (int)(y_mid + y_step * 9),   1,   13,    17,    21,    24},    // 53
    new int[] { (int)(x_mid + x_step * 06),    (int)(y_mid + y_step * 10),  1,   13,    17,    21,    24},    // 54
    new int[] { (int)(x_mid + x_step * 05),    (int)(y_mid + y_step * 11),  1,   13,    17,    21,    24},    // 55
    new int[] { (int)(x_mid + x_step * 04),    (int)(y_mid + y_step * 12),  1,   13,    17,    21,    24},    // 56
    new int[] { (int)(x_mid + x_step * 03),    (int)(y_mid + y_step * 13),  1,   13,    17,    21,    24},    // 57
    new int[] { (int)(x_mid + x_step * 02),    (int)(y_mid + y_step * 14), 12,   13,    17,    21,    24},    // 58
    new int[] { (int)(x_mid + x_step * 01),    (int)(y_mid + y_step * 15), 12,   13,    17,    21,    24},    // 59  top circle
};


            var controller = new TouchController();
            controller.Initialize();
            Console.CursorVisible = false;
            Console.WriteLine("Starting touch streams!");
            controller.StartTouchStream();
            Console.WriteLine("Started!");
            /* bool[] button_pressed = { false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, };  // 48 times false
            bool[] button_pressed_on_loop = { false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, };  // 48 times false */
            bool[] button_pressed = Enumerable.Repeat(false, 17).ToArray();
            bool[] button_pressed_on_loop = Enumerable.Repeat(false, 17).ToArray();
            bool[] keydown = Enumerable.Repeat(false, 17).ToArray();
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            Console.WriteLine(desktopPath);

            while (true)
            {
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
                                    if (File.Exists(Path.Combine(ahk, $"{axes[j][k] - 4}d.ahk")))
                                    {
                                        if (!button_pressed[axes[j][k] - 4])
                                        {
                                            button_pressed[axes[j][k] - 4] = true;
                                            Process.Start(Path.Combine(ahk, $"{axes[j][k] - 4}d.ahk"));
                                            keydown[axes[j][k] - 4] = true;
                                        }
                                    }
                                    else
                                    {
                                        Console.WriteLine($"failed to find " + Path.Combine(ahk, $"{axes[j][k] - 4}d.ahk"));
                                    }
                                    button_pressed_on_loop[axes[j][k] - 4] = true;
                                }
                            }
                            else  // parse the 2 inner layers
                            {
                                for (int k = 2; k < 3; k++)
                                {
                                    // launch 1 to 12.lnk if it exists
                                    if (File.Exists(Path.Combine(desktopPath, $"{axes[j][k]}.lnk")))
                                    {
                                        button_pressed_on_loop[axes[j][k]] = true;
                                        if (!button_pressed[axes[j][k]])
                                        {
                                            button_pressed[axes[j][k]] = true;
                                            Process.Start(Path.Combine(desktopPath, $"{axes[j][k]}.lnk"));
                                        }
                                    }
                                    else  // triggered if lnk doesn't exist
                                    {
                                        button_pressed_on_loop[axes[j][k]] = true;
                                        if (!button_pressed[axes[j][k]])
                                        {
                                            if (File.Exists(Path.Combine(ahk, $"{axes[j][k]}d.ahk")))
                                            {
                                                Process.Start(Path.Combine(ahk, $"{axes[j][k]}d.ahk"));
                                            }
                                            else
                                            {
                                                Console.WriteLine($"failed to find " + Path.Combine(ahk, $"{i}d.ahk"));
                                            }
                                            button_pressed[axes[j][k]] = true;
                                            keydown[axes[j][k]] = true;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                for (uint i = 0; i < 17; i++)
                {
                    if (button_pressed[i] && !button_pressed_on_loop[i])
                    {
                        if (keydown[i])
                        {
                            if (File.Exists(Path.Combine(ahk, $"{i}u.ahk")))
                            {
                                Process.Start(Path.Combine(ahk, $"{i}u.ahk"));
                            }
                            else
                            {
                                Console.WriteLine($"failed to find " + Path.Combine(ahk, $"{i}u.ahk"));
                            }
                            keydown[i] = false;
                        }
                        button_pressed[i] = false;
                    }
                    button_pressed_on_loop[i] = false;
                }
            }
        }
    }
}