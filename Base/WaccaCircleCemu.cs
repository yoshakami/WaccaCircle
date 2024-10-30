using LilyConsole;
using System;
using System.Linq;
using System.Net.Sockets;
using System.Net;
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

        static DSUClient dsuClient = new DSUClient();
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
            dsuClient.Close();
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
            if (joystick.GetVJDAxisExist(deviceId, HID_USAGES.HID_USAGE_RX))
                Console.WriteLine("Axis RX available");
            if (joystick.GetVJDAxisExist(deviceId, HID_USAGES.HID_USAGE_RY))
                Console.WriteLine("Axis RY available");

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
            long[] maxes = { y_max, x_max, ry_max, rx_max, sl0_max, sl1_max };
            long[] mines = { y_min, x_min, ry_min, rx_min, sl0_min, sl1_min };
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

            var controller = new TouchController();
            controller.Initialize();
            Console.CursorVisible = false;
            Console.WriteLine("Starting touch streams!");
            controller.StartTouchStream();
            Console.WriteLine("Started!");
            bool pressed_on_loop;
            bool rx_pressed_on_loop;
            int x_current;
            int y_current;
            float giro_x;
            float giro_y;
            byte outer_number_of_pressed_panels;
            /* bool[] button_pressed = { false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, };  // 48 times false
            bool[] button_pressed_on_loop = { false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, };  // 48 times false */
            bool[] button_pressed = Enumerable.Repeat(false, 25).ToArray();
            bool[] button_pressed_on_loop = Enumerable.Repeat(false, 25).ToArray();
            bool toggle11 = false;
            while (true)
            {
                Thread.Sleep(LAG_DELAY); // change this setting to 0ms, 100ms, 200ms, or 500ms.
                controller.GetTouchData();
                pressed_on_loop = false;
                rx_pressed_on_loop = false;
                outer_number_of_pressed_panels = 0;
                x_current = 0;
                y_current = 0;
                for (int i = 0; i < 4; i++)
                {
                    for (int j = 0; j < 60; j++)
                    {
                        if (controller.touchData[i, j])  // if the circle if touched
                        {
                            pressed_on_loop = true;
                            if (i > 1)  // RXY is only on the two outer layers, i==2 and i==3
                            {
                                outer_number_of_pressed_panels++;
                                x_current += axes[j][0];
                                y_current += axes[j][1];
                                for (int k = 4; k < 5; k++)  // outer buttons from 21 to 24
                                {
                                    button_pressed_on_loop[axes[j][k] + 4] = true;
                                    if (!button_pressed[axes[j][k] + 4])
                                    {   // don't send input yet! end for the circle scan to end
                                        // joystick.SetBtn(true, deviceId, (uint)(axes[j][k] + 8)); // Press button axes[j][k] + 12
                                        button_pressed[axes[j][k] + 4] = true;
                                    }
                                }
                                rx_pressed_on_loop = true;
                            }
                            else
                            {
                                for (int k = 2; k < 3; k++)  // inner buttons from 1 to 12
                                {
                                    button_pressed_on_loop[axes[j][k]] = true;
                                    if (!button_pressed[axes[j][k]])
                                    {
                                        if (axes[j][k] == 11)
                                        {
                                            toggle11 = toggle11 == false;
                                        }
                                        else if (axes[j][k] == 2 || axes[j][k] == 3 || axes[j][k] == 9 || axes[j][k] == 10)
                                        {
                                            if (toggle11)
                                            {
                                                joystick.SetBtn(true, deviceId, (uint)axes[j][k]); // Press button axes[j][k]
                                            }
                                        }
                                        else
                                        {
                                            joystick.SetBtn(true, deviceId, (uint)axes[j][k]); // Press button axes[j][k]
                                        }
                                        button_pressed[axes[j][k]] = true;
                                    }
                                }
                            }
                        }
                    }
                }
                if (rx_pressed_on_loop)  // average all the axes towards the middle of all the pressed spots
                {
                    if (button_pressed[1]) // 4 buttons xdddd
                    {
                        for (uint i = 21; i < 25; i++)
                        {
                            if (button_pressed[i])
                            {
                                joystick.SetBtn(true, deviceId, i - 8); // Press button 13 to 16
                                button_pressed_on_loop[i - 8] = true;
                                button_pressed[i - 8] = true;
                            }
                        }
                    }
                    else if (button_pressed[2] && !toggle11) // shake
                    {
                        if (!button_pressed_on_loop[10])
                        {
                            dsuClient.SendMotionData(controllerId: 0,
                                                     accelX: 0.0f, accelY: 0.0f, accelZ: 9.8f,
                                                     gyroX: 0.0f, gyroY: 0.0f, gyroZ: 0.0f);
                        }
                        else
                        {
                            x_current /= outer_number_of_pressed_panels;
                            giro_y = (x_current / outer_number_of_pressed_panels / 2048.0f) - 8.0f;
                            giro_x = (y_current / outer_number_of_pressed_panels / 2048.0f) - 8.0f;
                            // Send shake motion data
                            dsuClient.SendMotionData(controllerId: 0,
                                                     accelX: giro_x, accelY: giro_y, accelZ: 9.8f + giro_y,
                                                     gyroX: giro_x / 8, gyroY: giro_y / 8, gyroZ: giro_y / 8 + giro_x / 8);
                        }
                    }
                    else if (button_pressed[3] && !toggle11)  // right stick
                    {
                        x_current /= outer_number_of_pressed_panels;
                        y_current /= outer_number_of_pressed_panels;
                        joystick.SetAxis(x_current, deviceId, HID_USAGES.HID_USAGE_RX);
                        joystick.SetAxis(y_current, deviceId, HID_USAGES.HID_USAGE_RY);
                    }
                    else if (button_pressed[9] && !toggle11)
                    {
                        for (uint i = 21; i < 25; i++)
                        {
                            if (button_pressed_on_loop[i])
                            {
                                joystick.SetBtn(true, deviceId, i - 4); // Press button 17 to 20
                                button_pressed_on_loop[i - 4] = true;
                                button_pressed[i - 4] = true;
                            }
                        }
                    }
                    else if (button_pressed[10] && !toggle11)  // tilt
                    {
                        if (!button_pressed_on_loop[10])
                        {
                            dsuClient.SendMotionData(controllerId: 0,
                                                     accelX: 0.0f, accelY: 0.0f, accelZ: 9.8f,
                                                     gyroX: 0.0f, gyroY: 0.0f, gyroZ: 0.0f);
                        }
                        else
                        {
                            x_current /= outer_number_of_pressed_panels;
                            giro_y = (x_current / outer_number_of_pressed_panels / 16383.5f) - 1.0f;
                            giro_x = (y_current / outer_number_of_pressed_panels / 16383.5f) - 1.0f;
                            // Send tilt motion data
                            dsuClient.SendMotionData(controllerId: 0,
                                                     accelX: 0.0f, accelY: 0.0f, accelZ: 9.8f,
                                                     gyroX: giro_x, gyroY: giro_y, gyroZ: 0.0f);

                        }
                    }
                    else  // left stick
                    {
                        x_current /= outer_number_of_pressed_panels;
                        y_current /= outer_number_of_pressed_panels;
                        joystick.SetAxis(x_current, deviceId, HID_USAGES.HID_USAGE_X);
                        joystick.SetAxis(y_current, deviceId, HID_USAGES.HID_USAGE_Y);
                    }
                }
                for (uint i = 1; i < 25; i++)
                {
                    if (button_pressed[i] && !button_pressed_on_loop[i])
                    {
                        if (i > 20)
                        {
                            //pass
                        }
                        else if (toggle11)
                        {
                            joystick.SetBtn(false, deviceId, i); // Release button i
                        }
                        else if (i == 2 || i == 3 || i == 9 || i == 10)
                        {
                            //"do nothing";
                        }
                        else
                        {
                            joystick.SetBtn(false, deviceId, i); // Release button i
                        }
                        
                        button_pressed[i] = false;
                    }
                    button_pressed_on_loop[i] = false;
                }
                if (!pressed_on_loop)
                {
                    // Set joystick axis to midpoint
                    joystick.SetAxis((int)x_mid, deviceId, HID_USAGES.HID_USAGE_X);
                    joystick.SetAxis((int)y_mid, deviceId, HID_USAGES.HID_USAGE_Y);
                }
                if (!rx_pressed_on_loop)
                {
                    // Set joystick axis to midpoint
                    joystick.SetAxis((int)rx_mid, deviceId, HID_USAGES.HID_USAGE_RX);
                    joystick.SetAxis((int)ry_mid, deviceId, HID_USAGES.HID_USAGE_RY);
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
    class DSUClient
    {
        private readonly UdpClient udpClient;
        private readonly IPEndPoint endPoint;

        public DSUClient(string ip = "127.0.0.1", int port = 26760)
        {
            endPoint = new IPEndPoint(IPAddress.Parse(ip), port);
            udpClient = new UdpClient();
        }

        public void SendMotionData(int controllerId, float accelX, float accelY, float accelZ, float gyroX, float gyroY, float gyroZ)
        {
            // Prepare the DSU packet
            byte[] packet = new byte[100];
            int offset = 0;

            // DSU packet header
            Array.Copy(BitConverter.GetBytes(0x01000000), 0, packet, offset, 4); // Protocol version (1)
            offset += 4;
            Array.Copy(BitConverter.GetBytes(controllerId), 0, packet, offset, 4); // Controller slot ID
            offset += 4;
            Array.Copy(BitConverter.GetBytes(3), 0, packet, offset, 4); // Message type 3 for data
            offset += 4;

            // Gyroscope data
            Array.Copy(BitConverter.GetBytes(gyroX), 0, packet, offset, 4);
            offset += 4;
            Array.Copy(BitConverter.GetBytes(gyroY), 0, packet, offset, 4);
            offset += 4;
            Array.Copy(BitConverter.GetBytes(gyroZ), 0, packet, offset, 4);
            offset += 4;

            // Accelerometer data
            Array.Copy(BitConverter.GetBytes(accelX), 0, packet, offset, 4);
            offset += 4;
            Array.Copy(BitConverter.GetBytes(accelY), 0, packet, offset, 4);
            offset += 4;
            Array.Copy(BitConverter.GetBytes(accelZ), 0, packet, offset, 4);
            offset += 4;

            // Fill remaining data as necessary (timestamp, buttons, battery, etc.)
            byte[] timestamp = BitConverter.GetBytes((long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds);
            Array.Copy(timestamp, 0, packet, offset, timestamp.Length);

            // Send packet
            udpClient.Send(packet, packet.Length, endPoint);
        }

        public void Close()
        {
            udpClient.Close();
        }
    }
}