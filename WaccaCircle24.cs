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
        static int LAG_DELAY = 5; // I compiled with this setting set to 0ms, 100ms, 200ms, and 500ms.
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
            // Initialize vJoy interface
            Console.CancelKeyPress += new ConsoleCancelEventHandler(OnCancelKeyPress);
            TouchCombinedTest();
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

            // Check available axes
            if (joystick.GetVJDAxisExist(deviceId, HID_USAGES.HID_USAGE_X))
                Console.WriteLine("Axis X available");
            if (joystick.GetVJDAxisExist(deviceId, HID_USAGES.HID_USAGE_Y))
                Console.WriteLine("Axis Y available");
            if (joystick.GetVJDAxisExist(deviceId, HID_USAGES.HID_USAGE_Z))
                Console.WriteLine("Axis Z available");
            if (joystick.GetVJDAxisExist(deviceId, HID_USAGES.HID_USAGE_RX))
                Console.WriteLine("Axis RX available");
            if (joystick.GetVJDAxisExist(deviceId, HID_USAGES.HID_USAGE_RY))
                Console.WriteLine("Axis RY available");
            if (joystick.GetVJDAxisExist(deviceId, HID_USAGES.HID_USAGE_RZ))
                Console.WriteLine("Axis RZ available");
            if (joystick.GetVJDAxisExist(deviceId, HID_USAGES.HID_USAGE_SL0))
                Console.WriteLine("Axis SL0 available");
            if (joystick.GetVJDAxisExist(deviceId, HID_USAGES.HID_USAGE_SL1))
                Console.WriteLine("Axis SL1 available");

            // XY is the whole circle
            long x_max = 0;
            joystick.GetVJDAxisMax(deviceId, HID_USAGES.HID_USAGE_X, ref x_max);
            long x_min = 0;
            joystick.GetVJDAxisMin(deviceId, HID_USAGES.HID_USAGE_X, ref x_min);
            long y_max = 0;
            joystick.GetVJDAxisMax(deviceId, HID_USAGES.HID_USAGE_Y, ref y_max);
            long y_min = 0;
            joystick.GetVJDAxisMin(deviceId, HID_USAGES.HID_USAGE_Y, ref y_min);

            // RX and RY are the outer half of the circle
            long rx_max = 0;
            joystick.GetVJDAxisMax(deviceId, HID_USAGES.HID_USAGE_RX, ref rx_max);
            long rx_min = 0;
            joystick.GetVJDAxisMin(deviceId, HID_USAGES.HID_USAGE_RX, ref rx_min);
            long ry_max = 0;
            joystick.GetVJDAxisMax(deviceId, HID_USAGES.HID_USAGE_RY, ref ry_max);
            long ry_min = 0;
            joystick.GetVJDAxisMin(deviceId, HID_USAGES.HID_USAGE_RY, ref ry_min);

            // sl0 and sl1 are the inner half of the circle (near the screen)
            long sl0_max = 0;
            joystick.GetVJDAxisMax(deviceId, HID_USAGES.HID_USAGE_RX, ref sl0_max);
            long sl0_min = 0;
            joystick.GetVJDAxisMin(deviceId, HID_USAGES.HID_USAGE_RX, ref sl0_min);
            long sl1_max = 0;
            joystick.GetVJDAxisMax(deviceId, HID_USAGES.HID_USAGE_RY, ref sl1_max);
            long sl1_min = 0;
            joystick.GetVJDAxisMin(deviceId, HID_USAGES.HID_USAGE_RY, ref sl1_min);
            Console.WriteLine($"sl0_max: {sl0_max}   sl1_max : {sl1_max}");
            Console.WriteLine($"sl0_min: {sl0_min}   sl1_min : {sl1_min}");
            long[] maxes = {y_max,x_max,ry_max, rx_max , sl0_max, sl1_max};
            long[] mines = { y_min, x_min, ry_min, rx_min, sl0_min, sl1_min };
            for (int i = 0;i < maxes.Length; i++)
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
            double x_mid = (x_max - x_min) / 2;
            double y_mid = (y_max - y_min) / 2;
            double rx_mid = (rx_max - rx_min) / 2;
            double ry_mid = (ry_max - ry_min) / 2;
            double sl0_mid = (sl0_max - sl0_min) / 2;
            double sl1_mid = (sl1_max - sl1_min) / 2;
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
            {      //       x axis    y-axis   1-12  13-16  17-20  21-22  23-24
                new int[] { X(0.5),   Y(0.5),   12,   16,    17,    21,    23},    // 0  top circle
                new int[] { X(1.5),   Y(1.5),   12,   16,    17,    21,    23},    // 1
                new int[] { X(2.5),   Y(2.5),   12,   16,    17,    21,    23},    // 2
                new int[] { X(3.5),   Y(3.5),   11,   16,    17,    21,    23},    // 3
                new int[] { X(4.5),   Y(4.5),   11,   16,    17,    21,    23},    // 4
                new int[] { X(5.5),   Y(5.5),   11,   16,    17,    21,    23},    // 5
                new int[] { X(6.5),   Y(6.5),   11,   16,    17,    21,    23},    // 6
                new int[] { X(7.5),   Y(7.5),   11,   16,    17,    21,    23},    // 7
                new int[] { X(8.5),   Y(8.5),   10,   16,    20,    21,    23},    // 8
                new int[] { X(9.5),   Y(9.5),   10,   16,    20,    21,    23},    // 9
                new int[] { X(10.5),  Y(10.5),  10,   16,    20,    21,    23},    // 10
                new int[] { X(11.5),  Y(11.5),  10,   16,    20,    21,    23},    // 11
                new int[] { X(12.5),  Y(12.5),  10,   16,    20,    21,    23},    // 12
                new int[] { X(13.5),  Y(13.5),   9,   16,    20,    21,    23},    // 13
                new int[] { X(14.5),  Y(14.5),   9,   16,    20,    21,    23},    // 14  left
                new int[] { X(15.5),  Y(15.5),   9,   15,    20,    22,    23},    // 15  left 
                new int[] { X(16.5),  Y(16.5),   9,   15,    20,    22,    23},    // 16
                new int[] { X(17.5),  Y(17.5),   9,   15,    20,    22,    23},    // 17
                new int[] { X(18.5),  Y(18.5),   8,   15,    20,    22,    23},    // 18
                new int[] { X(19.5),  Y(19.5),   8,   15,    20,    22,    23},    // 19
                new int[] { X(20.5),  Y(20.5),   8,   15,    20,    22,    23},    // 20
                new int[] { X(21.5),  Y(21.5),   8,   15,    20,    22,    23},    // 21
                new int[] { X(22.5),  Y(22.5),   8,   15,    20,    22,    23},    // 22
                new int[] { X(23.5),  Y(23.5),   7,   15,    19,    22,    23},    // 23
                new int[] { X(24.5),  Y(24.5),   7,   15,    19,    22,    23},    // 24
                new int[] { X(25.5),  Y(25.5),   7,   15,    19,    22,    23},    // 25
                new int[] { X(26.5),  Y(26.5),   7,   15,    19,    22,    23},    // 26
                new int[] { X(27.5),  Y(27.5),   7,   15,    19,    22,    23},    // 27
                new int[] { X(28.5),  Y(28.5),   6,   15,    19,    22,    23},    // 28
                new int[] { X(29.5),  Y(29.5),   6,   15,    19,    22,    23},    // 29  bottom
                new int[] { X(30.5),  Y(30.5),   6,   14,    19,    22,    24},    // 30  bottom
                new int[] { X(31.5),  Y(31.5),   6,   14,    19,    22,    24},    // 31
                new int[] { X(32.5),  Y(32.5),   6,   14,    19,    22,    24},    // 32
                new int[] { X(33.5),  Y(33.5),   5,   14,    19,    22,    24},    // 33
                new int[] { X(34.5),  Y(34.5),   5,   14,    19,    22,    24},    // 34
                new int[] { X(35.5),  Y(35.5),   5,   14,    19,    22,    24},    // 35
                new int[] { X(36.5),  Y(36.5),   5,   14,    19,    22,    24},    // 36
                new int[] { X(37.5),  Y(37.5),   5,   14,    19,    22,    24},    // 37
                new int[] { X(38.5),  Y(38.5),   4,   14,    18,    22,    24},    // 38
                new int[] { X(39.5),  Y(39.5),   4,   14,    18,    22,    24},    // 39
                new int[] { X(40.5),  Y(40.5),   4,   14,    18,    22,    24},    // 40
                new int[] { X(41.5),  Y(41.5),   4,   14,    18,    22,    24},    // 41
                new int[] { X(42.5),  Y(42.5),   4,   14,    18,    22,    24},    // 42
                new int[] { X(43.5),  Y(43.5),   3,   14,    18,    22,    24},    // 43
                new int[] { X(44.5),  Y(44.5),   3,   14,    18,    22,    24},    // 44  right
                new int[] { X(45.5),  Y(45.5),   3,   13,    18,    21,    24},    // 45  right
                new int[] { X(46.5),  Y(46.5),   3,   13,    18,    21,    24},    // 46
                new int[] { X(47.5),  Y(47.5),   3,   13,    18,    21,    24},    // 47
                new int[] { X(48.5),  Y(48.5),   2,   13,    18,    21,    24},    // 48
                new int[] { X(49.5),  Y(49.5),   2,   13,    18,    21,    24},    // 49
                new int[] { X(50.5),  Y(50.5),   2,   13,    18,    21,    24},    // 50
                new int[] { X(51.5),  Y(51.5),   2,   13,    18,    21,    24},    // 51
                new int[] { X(52.5),  Y(52.5),   2,   13,    18,    21,    24},    // 52
                new int[] { X(53.5),  Y(53.5),   1,   13,    17,    21,    24},    // 53
                new int[] { X(54.5),  Y(54.5),   1,   13,    17,    21,    24},    // 54
                new int[] { X(55.5),  Y(55.5),   1,   13,    17,    21,    24},    // 55
                new int[] { X(56.5),  Y(56.5),   1,   13,    17,    21,    24},    // 56
                new int[] { X(57.5),  Y(57.5),   1,   13,    17,    21,    24},    // 57
                new int[] { X(58.5),  Y(58.5),  12,   13,    17,    21,    24},    // 58
                new int[] { X(59.5),  Y(59.5),  12,   13,    17,    21,    24},    // 59  top circle
            };

            var controller = new TouchController();
            controller.Initialize();
            Console.CursorVisible = false;
            Console.WriteLine("Starting touch streams!");
            controller.StartTouchStream();
            Console.WriteLine("Started!");
            bool pressed = false;
            bool pressed_on_loop;
            bool rx_pressed = false;
            bool rx_pressed_on_loop;
            bool sl_pressed = false;
            bool sl_pressed_on_loop;
            int x_last = 0;
            int y_last = 0;
            int rx_last = 0;
            int ry_last = 0;
            int sl0_last = 0;
            int sl1_last = 0;
            int x_current;
            int y_current;
            int rx_current;
            int ry_current;
            int sl0_current;
            int sl1_current;
            byte inner_number_of_pressed_panels;
            byte outer_number_of_pressed_panels;
            /* bool[] button_pressed = { false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, };  // 48 times false
            bool[] button_pressed_on_loop = { false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, };  // 48 times false */
            bool[] button_pressed = Enumerable.Repeat(false, 25).ToArray();
            bool[] button_pressed_on_loop = Enumerable.Repeat(false, 25).ToArray();

            while (true)
            {
                Thread.Sleep(LAG_DELAY); // change this setting to 0ms, 100ms, 200ms, or 500ms.
                controller.GetTouchData();
                pressed_on_loop = false;
                rx_pressed_on_loop = false;
                sl_pressed_on_loop = false;
                inner_number_of_pressed_panels = 0;
                outer_number_of_pressed_panels = 0;
                rx_current = 0;
                ry_current = 0;
                sl0_current = 0;
                sl1_current = 0;
                for (int i = 0; i < 4; i++)
                {
                    for (int j = 0; j < 60; j++)
                    {
                        if (controller.touchData[i, j])  // if the circle if touched
                        {
                            pressed_on_loop = true;
                            pressed = true;
                            if (i > 1)  // RXY is only on the two outer layers, i==2 and i==3
                            {
                                outer_number_of_pressed_panels++;
                                rx_current += axes[j][0];
                                ry_current += axes[j][1];
                                rx_pressed = true;
                                rx_pressed_on_loop = true;
                            }
                            else
                            {
                                inner_number_of_pressed_panels++;
                                sl0_current += axes[j][0];
                                sl1_current += axes[j][1];
                                for (int k = 2; k < 7; k++)  // inner buttons from 1 to 24
                                {
                                    button_pressed_on_loop[axes[j][k]] = true;
                                    if (!button_pressed[axes[j][k]])
                                    {
                                        joystick.SetBtn(true, deviceId, (uint)axes[j][k]); // Press button axes[j][k]
                                        button_pressed[axes[j][k]] = true;
                                    }
                                }
                                sl_pressed = true;
                                sl_pressed_on_loop = true;
                            }
                        }
                    }
                }
                if (pressed_on_loop)  // average all the axes towards the middle of all the pressed spots
                {
                    x_current = (sl0_current + rx_current) / (outer_number_of_pressed_panels + inner_number_of_pressed_panels);
                    y_current = (sl1_current + ry_current) / (outer_number_of_pressed_panels + inner_number_of_pressed_panels);

                    if (inner_number_of_pressed_panels > 0)
                    {
                        sl0_current /= inner_number_of_pressed_panels;
                        sl1_current /= inner_number_of_pressed_panels;
                    }
                    if (outer_number_of_pressed_panels > 0)
                    {
                        rx_current /= outer_number_of_pressed_panels;
                        ry_current /= outer_number_of_pressed_panels;
                    }
                    if (x_current != x_last)
                    {
                        joystick.SetAxis(x_current, deviceId, HID_USAGES.HID_USAGE_X);
                        x_last = x_current;
                    }
                    if (y_current != y_last)
                    {
                        joystick.SetAxis(y_current, deviceId, HID_USAGES.HID_USAGE_Y);
                        y_last = y_current;
                    }
                    if (rx_current != rx_last)
                    {
                        joystick.SetAxis(rx_current, deviceId, HID_USAGES.HID_USAGE_RX);
                        rx_last = rx_current;
                    }
                    if (ry_current != ry_last)
                    {
                        joystick.SetAxis(ry_current, deviceId, HID_USAGES.HID_USAGE_RY);
                        ry_last = ry_current;
                    }
                    if (sl0_current != sl0_last)
                    {
                        joystick.SetAxis(sl0_current, deviceId, HID_USAGES.HID_USAGE_SL0);
                        sl0_last = sl0_current;
                    }
                    if (sl1_current != sl1_last)
                    {
                        joystick.SetAxis(sl1_current, deviceId, HID_USAGES.HID_USAGE_SL1);
                        sl1_last = sl1_current;
                    }
                }
                for (uint i = 1; i < 25; i++)
                {
                    if (button_pressed[i] && !button_pressed_on_loop[i])
                    {
                        joystick.SetBtn(false, deviceId, i); // Release button i
                        button_pressed[i] = false;
                    }
                    button_pressed_on_loop[i] = false;
                }
                if (pressed && !pressed_on_loop)
                {
                    // Set joystick axis to midpoint
                    joystick.SetAxis((int)x_mid, deviceId, HID_USAGES.HID_USAGE_X);
                    joystick.SetAxis((int)y_mid, deviceId, HID_USAGES.HID_USAGE_Y);
                    pressed = false;
                }
                if (rx_pressed && !rx_pressed_on_loop)
                {
                    // Set joystick axis to midpoint
                    joystick.SetAxis((int)rx_mid, deviceId, HID_USAGES.HID_USAGE_RX);
                    joystick.SetAxis((int)ry_mid, deviceId, HID_USAGES.HID_USAGE_RY);
                    rx_pressed = false;
                }
                if (sl_pressed && !sl_pressed_on_loop)
                {
                    // Set joystick axis to midpoint
                    joystick.SetAxis((int)sl0_mid, deviceId, HID_USAGES.HID_USAGE_SL0);
                    joystick.SetAxis((int)sl1_mid, deviceId, HID_USAGES.HID_USAGE_SL1);
                    sl_pressed = false;
                }
            }
        }

        private static int Y(double v)
        {
            return (int)(Math.Cos(v * 2 * Math.PI / 60) * axis_max);
        }

        private static int X(double value)
        {
            return (int)(-Math.Sin(value * 2 * Math.PI / 60) * axis_max); // 0 starts on top of the circle, and 32767 is the max value of the stick
        }
    }
}