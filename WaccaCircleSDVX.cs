using LilyConsole;
using System;
using System.Linq;
using System.Threading;
using vJoyInterfaceWrap;

namespace WaccaKeyBind
{
    internal class WaccaCircle32
    {
        static vJoy joystick = new vJoy();
        // Device ID (must be 1-16, based on vJoy configuration)
        static uint deviceId = 1;  // I compiled with this set to 1, 2, and 3
        static int LAG_DELAY = 30; // tweak between 0ms and 100ms to reduce CPU usage or increase responsiveness
        static long axis_max = 32767;
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

            Console.CancelKeyPress += new ConsoleCancelEventHandler(OnCancelKeyPress);
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
        static void OnCancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            Console.WriteLine("ctrl+c detected!\ndisposing virtual Joystick....\ndone!\npress enter to exit...");
            Console.ReadLine();
            // Release the device when done
            joystick.RelinquishVJD(deviceId);
        }
        public static void TouchCombinedTest()
        {
            // Check if vJoy is enabled and ready
            if (!joystick.vJoyEnabled())
            {
                Console.WriteLine("vJoy driver not enabled: Failed to find vJoy.\npress enter to exit...");
                Console.ReadLine();
                return;
            }

            // Acquire the vJoy device
            VjdStat status = joystick.GetVJDStatus(deviceId);

            if (status == VjdStat.VJD_STAT_FREE)
            {
                // Attempt to acquire the joystick
                if (!joystick.AcquireVJD(deviceId))
                {
                    Console.WriteLine("Failed to acquire vJoy device.\npress enter to exit...");
                    Console.ReadLine();
                    return;
                }
                Console.WriteLine("vJoy device acquired successfully.");
            }
            else
            {
                Console.WriteLine("vJoy device is not free. Status: " + status.ToString() + "\npress enter to exit...");
                Console.ReadLine();
                return;
            }

            if (joystick.GetVJDAxisExist(deviceId, HID_USAGES.HID_USAGE_RX))
                Console.WriteLine("Axis RX available");
            if (joystick.GetVJDAxisExist(deviceId, HID_USAGES.HID_USAGE_RY))
                Console.WriteLine("Axis RY available");

            // RX and RY are the outer half of the circle
            long rx_max = 0;
            joystick.GetVJDAxisMax(deviceId, HID_USAGES.HID_USAGE_RX, ref rx_max);
            long rx_min = 0;
            joystick.GetVJDAxisMin(deviceId, HID_USAGES.HID_USAGE_RX, ref rx_min);
            long ry_max = 0;
            joystick.GetVJDAxisMax(deviceId, HID_USAGES.HID_USAGE_RY, ref ry_max);
            long ry_min = 0;
            joystick.GetVJDAxisMin(deviceId, HID_USAGES.HID_USAGE_RY, ref ry_min);

            long[] maxes = { ry_max, rx_max };
            long[] mines = { ry_min, rx_min };
            for (int i = 0; i < maxes.Length; i++)
            {
                if (maxes[i] != axis_max)
                {
                    Console.WriteLine($"this program will not work as expected. maxes[{i}] is {maxes[i]}\nchanging axis max....");
                    axis_max = maxes[i];
                }
                if (mines[i] != 0)
                {
                    Console.WriteLine($"this program will not work as expected. mines[{i}] is {mines[i]}");
                }
            }
            double rx_mid = (rx_max - rx_min) / 2;
            double ry_mid = (ry_max - ry_min) / 2;
            /*
             * 

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
            {      // SDVX  x axis    y-axis   1-13  13-16  17-20  21-22  23-24  25-32
                new int[] { X(0.5),   Y(30.5),  13,   16,    17,    21,    23,    25},    // 0  top circle
                new int[] { X(1.5),   Y(31.5),  13,   16,    17,    21,    23,    25},    // 1
                new int[] { X(2.5),   Y(32.5),  13,   16,    17,    21,    23,    25},    // 2
                new int[] { X(3.5),   Y(33.5),  12,   16,    17,    21,    23,    25},    // 3
                new int[] { X(4.5),   Y(34.5),  12,   16,    17,    21,    23,    25},    // 4
                new int[] { X(5.5),   Y(35.5),  12,   16,    17,    21,    23,    25},    // 5
                new int[] { X(6.5),   Y(36.5),  12,   16,    17,    21,    23,    25},    // 6
                new int[] { X(7.5),   Y(37.5),  12,   16,    17,    21,    23,    25},    // 7
                new int[] { X(8.5),   Y(38.5),  11,   16,    20,    21,    23,    26},    // 8
                new int[] { X(9.5),   Y(39.5),  11,   16,    20,    21,    23,    26},    // 9
                new int[] { X(10.5),  Y(40.5),  11,   16,    20,    21,    23,    26},    // 10
                new int[] { X(11.5),  Y(41.5),  11,   16,    20,    21,    23,    26},    // 11
                new int[] { X(12.5),  Y(42.5),  11,   16,    20,    21,    23,    26},    // 12
                new int[] { X(13.5),  Y(43.5),  10,   16,    20,    21,    23,    26},    // 13
                new int[] { X(14.5),  Y(44.5),  10,   16,    20,    21,    23,    26},    // 14  left
                new int[] { X(45.5),  Y(45.5),  10,   15,    20,    22,    23,    26},    // 15  left 
                new int[] { X(46.5),  Y(46.5),  10,   15,    20,    22,    23,    27},    // 16
                new int[] { X(47.5),  Y(47.5),  10,   15,    20,    22,    23,    27},    // 17
                new int[] { X(48.5),  Y(48.5),   9,   15,    20,    22,    23,    27},    // 18
                new int[] { X(49.5),  Y(49.5),   9,   15,    20,    22,    23,    27},    // 19
                new int[] { X(50.5),  Y(50.5),   9,   15,    20,    22,    23,    27},    // 20
                new int[] { X(51.5),  Y(51.5),   9,   15,    20,    22,    23,    27},    // 21
                new int[] { X(52.5),  Y(52.5),   9,   15,    20,    22,    23,    27},    // 22
                new int[] { X(53.5),  Y(53.5),   8,   15,    19,    22,    23,    28},    // 23
                new int[] { X(54.5),  Y(54.5),   8,   15,    19,    22,    23,    28},    // 24
                new int[] { X(55.5),  Y(55.5),   8,   15,    19,    22,    23,    28},    // 25
                new int[] { X(56.5),  Y(56.5),   8,   15,    19,    22,    23,    28},    // 26
                new int[] { X(57.5),  Y(57.5),   7,   15,    19,    22,    23,    28},    // 27
                new int[] { X(58.5),  Y(58.5),   7,   15,    19,    22,    23,    28},    // 28
                new int[] { X(59.5),  Y(59.5),   7,   15,    19,    22,    23,    28},    // 29  bottom
                new int[] { X(00.5),  Y(30.5),   6,   14,    19,    22,    24,    29},    // 30  bottom
                new int[] { X(01.5),  Y(31.5),   6,   14,    19,    22,    24,    29},    // 31
                new int[] { X(02.5),  Y(32.5),   6,   14,    19,    22,    24,    29},    // 32
                new int[] { X(03.5),  Y(33.5),   5,   14,    19,    22,    24,    29},    // 33
                new int[] { X(04.5),  Y(34.5),   5,   14,    19,    22,    24,    29},    // 34
                new int[] { X(05.5),  Y(35.5),   5,   14,    19,    22,    24,    29},    // 35
                new int[] { X(06.5),  Y(36.5),   5,   14,    19,    22,    24,    29},    // 36
                new int[] { X(07.5),  Y(37.5),   4,   14,    19,    22,    24,    30},    // 37
                new int[] { X(08.5),  Y(38.5),   4,   14,    18,    22,    24,    30},    // 38
                new int[] { X(09.5),  Y(39.5),   4,   14,    18,    22,    24,    30},    // 39
                new int[] { X(10.5),  Y(40.5),   4,   14,    18,    22,    24,    30},    // 40
                new int[] { X(11.5),  Y(41.5),   4,   14,    18,    22,    24,    30},    // 41
                new int[] { X(12.5),  Y(42.5),   4,   14,    18,    22,    24,    30},    // 42
                new int[] { X(13.5),  Y(43.5),   3,   14,    18,    22,    24,    30},    // 43
                new int[] { X(14.5),  Y(44.5),   3,   14,    18,    22,    24,    31},    // 44  right
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
                new int[] { X(58.5),  Y(58.5),  13,   13,    17,    21,    24,    32},    // 58
                new int[] { X(59.5),  Y(59.5),  13,   13,    17,    21,    24,    32},    // 59  top circle
            };

            var controller = new TouchController();
            controller.Initialize();
            Console.CursorVisible = false;
            Console.WriteLine("Starting touch streams!");
            controller.StartTouchStream();
            Console.WriteLine("Started!");
            int rx_current;
            int ry_current;
            byte outer_number_ry;
            byte outer_number_of_pressed_panels;
            /* bool[] button_pressed = { false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, };  // 48 times false
            bool[] button_pressed_on_loop = { false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, };  // 48 times false */
            bool[] button_pressed = Enumerable.Repeat(false, 33).ToArray();
            bool[] button_pressed_on_loop = Enumerable.Repeat(false, 33).ToArray();
            byte state = 0; // 0 = full left, 1 = half left half right, 2 = full right
            while (true)
            {
                Thread.Sleep(LAG_DELAY); // change this setting to 0ms, 100ms, 200ms, or 500ms.
                controller.GetTouchData();
                outer_number_of_pressed_panels = 0;  // rx-axis only
                outer_number_ry = 0;
                rx_current = 0;
                ry_current = 0;
                for (int i = 0; i < 4; i++)
                {
                    for (int j = 0; j < 60; j++)
                    {
                        if (controller.touchData[i, j])  // if the circle if touched
                        {
                            if (i > 1)  // RXY is only on the two outer layers, i==2 and i==3
                            {
                                for (int k = 4; k < 8; k++)  // outer buttons from 17 to 32
                                {
                                    button_pressed_on_loop[axes[j][k]] = true;
                                    if (!button_pressed[axes[j][k]])
                                    {
                                        joystick.SetBtn(true, deviceId, (uint)(axes[j][k])); // Press button axes[j][k]
                                        button_pressed[axes[j][k]] = true;
                                    }
                                }
                                if (state == 0)
                                {
                                    outer_number_of_pressed_panels++;
                                    rx_current += axes[j][0];
                                }
                                else if (state == 2)
                                {
                                    outer_number_ry++;
                                    ry_current += axes[j][1];
                                }
                                if (state == 1 && j < 30)
                                {
                                    ry_current += axes[j][1];
                                    outer_number_ry++;
                                }
                                else  // state == 1 && j > 29
                                {
                                    outer_number_of_pressed_panels++;
                                    rx_current += axes[j][1];  // yup, I put a 1 there
                                }
                            }
                            else
                            {
                                for (int k = 2; k < 3; k++)  // inner buttons from 1 to 13
                                {
                                    if (axes[j][k] == 13)
                                    {
                                        state++;
                                        if (state == 3)
                                        {
                                            state = 0;
                                        }
                                    }
                                    button_pressed_on_loop[axes[j][k]] = true;
                                    if (!button_pressed[axes[j][k]])
                                    {
                                        joystick.SetBtn(true, deviceId, (uint)axes[j][k]); // Press button axes[j][k]
                                        button_pressed[axes[j][k]] = true;
                                    }
                                }
                            }
                        }
                    }
                }
                if (outer_number_of_pressed_panels > 0)  // average all the axes towards the middle of all the pressed spots
                {
                    rx_current /= outer_number_of_pressed_panels;
                    joystick.SetAxis(rx_current, deviceId, HID_USAGES.HID_USAGE_RX);
                }
                if (outer_number_ry > 0)
                {
                    ry_current /= outer_number_ry;
                    joystick.SetAxis(ry_current, deviceId, HID_USAGES.HID_USAGE_RY);
                }
                for (uint i = 1; i < 33; i++)
                {
                    if (button_pressed[i] && !button_pressed_on_loop[i])
                    {
                        joystick.SetBtn(false, deviceId, i); // Release button i
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