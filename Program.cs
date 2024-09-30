using LilyConsole;
using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using vJoyInterfaceWrap;

namespace WaccaKeyBind
{
    internal class Program
    {
        static vJoy joystick = new vJoy();
        // Device ID (must be 1-16, based on vJoy configuration)
        static uint deviceId = 1;
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
            while (true)
            {
                TouchCombinedTest();
            }
        }
        static void OnCancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            Console.WriteLine("ctrl+c detected!\ndisposing virtual Joystick....\ndone!\npress enter to exit...");
            Console.ReadLine();
            // Release the device when done
            joystick.RelinquishVJD(deviceId);
        }
        // Define constants for the input types
        private const int INPUT_KEYBOARD = 1;
        private const ushort VK_LEFT = 0x25; // Virtual-Key Code for the Left Arrow
        private const ushort VK_UP = 0x26; // Virtual-Key Code for the Up Arrow
        private const ushort VK_RIGHT = 0x27; // Virtual-Key Code for the Right Arrow
        private const ushort VK_DOWN = 0x28; // Virtual-Key Code for the Down Arrow
        private const uint KEYEVENTF_KEYDOWN = 0x0000; // Key down flag
        private const uint KEYEVENTF_KEYUP = 0x0002; // Key up flag

        // Structure for SendInput function
        [StructLayout(LayoutKind.Sequential)]
        public struct INPUT
        {
            public uint type;
            public InputUnion u;
        }

        [StructLayout(LayoutKind.Explicit)]
        public struct InputUnion
        {
            [FieldOffset(0)] public KEYBDINPUT ki;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct KEYBDINPUT
        {
            public ushort wVk;
            public ushort wScan;
            public uint dwFlags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        [DllImport("user32.dll", SetLastError = true)]
        public static extern uint SendInput(uint nInputs, INPUT[] pInputs, int cbSize);



        // Send key down (press)
        static void SendKeyDown(ushort keyCode)
        {
            INPUT[] inputs = new INPUT[1];

            inputs[0] = new INPUT();
            inputs[0].type = INPUT_KEYBOARD;
            inputs[0].u.ki.wVk = keyCode;
            inputs[0].u.ki.wScan = 0;
            inputs[0].u.ki.dwFlags = KEYEVENTF_KEYDOWN; // Key down flag
            inputs[0].u.ki.time = 0;
            inputs[0].u.ki.dwExtraInfo = IntPtr.Zero;

            SendInput((uint)inputs.Length, inputs, Marshal.SizeOf(typeof(INPUT)));
        }

        // Send key up (release)
        static void SendKeyUp(ushort keyCode)
        {
            INPUT[] inputs = new INPUT[1];

            inputs[0] = new INPUT();
            inputs[0].type = INPUT_KEYBOARD;
            inputs[0].u.ki.wVk = keyCode;
            inputs[0].u.ki.wScan = 0;
            inputs[0].u.ki.dwFlags = KEYEVENTF_KEYUP; // Key up flag
            inputs[0].u.ki.time = 0;
            inputs[0].u.ki.dwExtraInfo = IntPtr.Zero;

            SendInput((uint)inputs.Length, inputs, Marshal.SizeOf(typeof(INPUT)));
        }
        public static void Arrows()
        {
            var controller = new TouchController();
            controller.Initialize();
            Console.CursorVisible = false;
            Console.WriteLine("Starting touch streams!");
            controller.StartTouchStream();
            Console.WriteLine("Started!");
            bool up = false;
            bool down = false;
            bool left = false;
            bool right = false;
            while (true)
            {
                controller.GetTouchData();

                Console.WriteLine("Current Touch Frame:");
                bool up_pressed_on_loop = false;
                bool down_pressed_on_loop = false;
                bool left_pressed_on_loop = false;
                bool right_pressed_on_loop = false;

                for (int i = 0; i < 4; i++)
                {
                    for (int j = 0; j < 60; j++)
                    {
                        if (controller.touchData[i, j])
                        {
                            if ((0 <= j && j < 8) || (53 <= j && j < 60))
                            {
                                up_pressed_on_loop = true;
                                if (!up)
                                {
                                    SendKeyUp(VK_UP);
                                    up = true;
                                }
                            }
                            if ((8 <= j && j < 23))
                            {
                                left_pressed_on_loop = true;
                                if (!left)
                                {
                                    SendKeyDown(VK_LEFT);
                                    left = true;
                                }
                            }
                            if ((23 <= j && j < 38))
                            {
                                down_pressed_on_loop = true;
                                if (!down)
                                {
                                    SendKeyUp(VK_DOWN);
                                    down = true;
                                }
                            }
                            if ((38 <= j && j < 53))
                            {
                                right_pressed_on_loop = true;
                                if (!right)
                                {
                                    SendKeyDown(VK_RIGHT);
                                    right = true;
                                }
                            }
                        }
                    }
                }
                if (up && !up_pressed_on_loop)
                {
                    SendKeyDown(VK_UP);
                    up = false;
                }
                else
                {
                    if (left && !left_pressed_on_loop)
                    {
                        SendKeyDown(VK_LEFT);
                        left = false;
                    }
                    else
                    {
                        if (down && !down_pressed_on_loop)
                        {
                            SendKeyDown(VK_DOWN);
                            down = false;
                        }
                        else
                        {
                            if (right && !right_pressed_on_loop)
                            {
                                SendKeyDown(VK_RIGHT);
                                right = false;
                            }
                        }
                    }
                }
            }
        
    }
        public static void TouchCombinedTest()
        {
            // Check if vJoy is enabled and ready
            if (!joystick.vJoyEnabled())
            {
                Console.WriteLine("vJoy driver not enabled: Failed to find vJoy.");
                return;
            }

            // Acquire the vJoy device
            VjdStat status = joystick.GetVJDStatus(deviceId);

            if (status == VjdStat.VJD_STAT_FREE)
            {
                // Attempt to acquire the joystick
                if (!joystick.AcquireVJD(deviceId))
                {
                    Console.WriteLine("Failed to acquire vJoy device.");
                    return;
                }
                Console.WriteLine("vJoy device acquired successfully.");
            }
            else
            {
                Console.WriteLine("vJoy device is not free. Status: " + status.ToString());
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
                Console.WriteLine("Axis SL0 available");

            // XY is the whole circle
            long x_max = 0;
            joystick.GetVJDAxisMax(deviceId, HID_USAGES.HID_USAGE_X, ref x_max);
            long x_min = 0;
            joystick.GetVJDAxisMin(deviceId, HID_USAGES.HID_USAGE_X, ref x_min);
            long y_max = 0;
            joystick.GetVJDAxisMax(deviceId, HID_USAGES.HID_USAGE_Y, ref y_max);
            long y_min = 0;
            joystick.GetVJDAxisMin(deviceId, HID_USAGES.HID_USAGE_Y, ref y_min);
            Console.WriteLine($"x_max: {x_max}   y_max : {y_max}");
            Console.WriteLine($"x_min: {x_min}   y_min : {y_min}");

            // RX and RY are the outer half of the circle
            long rx_max = 0;
            joystick.GetVJDAxisMax(deviceId, HID_USAGES.HID_USAGE_RX, ref rx_max);
            long rx_min = 0;
            joystick.GetVJDAxisMin(deviceId, HID_USAGES.HID_USAGE_RX, ref rx_min);
            long ry_max = 0;
            joystick.GetVJDAxisMax(deviceId, HID_USAGES.HID_USAGE_RY, ref ry_max);
            long ry_min = 0;
            joystick.GetVJDAxisMin(deviceId, HID_USAGES.HID_USAGE_RY, ref ry_min);
            Console.WriteLine($"rx_max: {rx_max}   ry_max : {ry_max}");
            Console.WriteLine($"rx_min: {rx_min}   ry_min : {ry_min}");
            int x_mid = (int)(x_max - x_min) / 2;
            int y_mid = (int)(y_max - y_min) / 2;
            int rx_mid = (int)(rx_max - rx_min) / 2;
            int ry_mid = (int)(ry_max - ry_min) / 2;
            int x_step = (int)(x_max - x_min) / 30;
            int y_step = (int)(y_max - y_min) / 30;
            int rx_step = (int)(rx_max - rx_min) / 30;
            int ry_step = (int)(ry_max - ry_min) / 30;
            /*
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
            {
                new int[] { x_mid - x_step * 1,     y_mid + y_step * 15,  12},  // 0  top circle
                new int[] { x_mid - x_step * 2,     y_mid + y_step * 14,  12},  // 1
                new int[] { x_mid - x_step * 3,     y_mid + y_step * 13,  12},  // 2
                new int[] { x_mid - x_step * 4,     y_mid + y_step * 12,  11},  // 3
                new int[] { x_mid - x_step * 5,     y_mid + y_step * 11,  11},  // 4
                new int[] { x_mid - x_step * 6,     y_mid + y_step * 10,  11},  // 5
                new int[] { x_mid - x_step * 7,     y_mid + y_step * 9,  11},   // 6
                new int[] { x_mid - x_step * 8,     y_mid + y_step * 8,  11},   // 7
                new int[] { x_mid - x_step * 9,     y_mid + y_step * 7,  10},   // 8
                new int[] { x_mid - x_step * 10,    y_mid + y_step * 6,  10},   // 9
                new int[] { x_mid - x_step * 11,    y_mid + y_step * 5,  10},   // 10
                new int[] { x_mid - x_step * 12,    y_mid + y_step * 4,  10},   // 11
                new int[] { x_mid - x_step * 13,    y_mid + y_step * 3,  10},   // 12
                new int[] { x_mid - x_step * 14,    y_mid + y_step * 2,   9},   // 13
                new int[] { x_mid - x_step * 15,    y_mid + y_step * 1,   9},   // 14  left
                new int[] { x_mid - x_step * 15,    y_mid - y_step * 1,   9},   // 15  left 
                new int[] { x_mid - x_step * 14,    y_mid - y_step * 2,   9},   // 16
                new int[] { x_mid - x_step * 13,    y_mid - y_step * 3,   9},   // 17
                new int[] { x_mid - x_step * 12,    y_mid - y_step * 4,   8},   // 18
                new int[] { x_mid - x_step * 11,    y_mid - y_step * 5,   8},   // 19
                new int[] { x_mid - x_step * 10,    y_mid - y_step * 6,   8},   // 20
                new int[] { x_mid - x_step * 09,    y_mid - y_step * 7,   8},   // 21
                new int[] { x_mid - x_step * 08,    y_mid - y_step * 8,   8},   // 22
                new int[] { x_mid - x_step * 07,    y_mid - y_step * 9,   7},   // 23
                new int[] { x_mid - x_step * 06,    y_mid - y_step * 10,  7},   // 24
                new int[] { x_mid - x_step * 05,    y_mid - y_step * 11,  7},   // 25
                new int[] { x_mid - x_step * 04,    y_mid - y_step * 12,  7},   // 26
                new int[] { x_mid - x_step * 03,    y_mid - y_step * 13,  7},   // 27
                new int[] { x_mid - x_step * 02,    y_mid - y_step * 14,  6},   // 28
                new int[] { x_mid - x_step * 01,    y_mid - y_step * 15,  6},   // 29  bottom
                new int[] { x_mid + x_step * 01,    y_mid - y_step * 15,  6},   // 30  bottom
                new int[] { x_mid + x_step * 02,    y_mid - y_step * 14,  6},   // 31
                new int[] { x_mid + x_step * 03,    y_mid - y_step * 13,  6},   // 32
                new int[] { x_mid + x_step * 04,    y_mid - y_step * 12,  5},   // 33
                new int[] { x_mid + x_step * 05,    y_mid - y_step * 11,  5},   // 34
                new int[] { x_mid + x_step * 06,    y_mid - y_step * 10,  5},   // 35
                new int[] { x_mid + x_step * 07,    y_mid - y_step * 9,   5},   // 36
                new int[] { x_mid + x_step * 08,    y_mid - y_step * 8,   5},   // 37
                new int[] { x_mid + x_step * 09,    y_mid - y_step * 7,   4},   // 38
                new int[] { x_mid + x_step * 10,    y_mid - y_step * 6,   4},   // 39
                new int[] { x_mid + x_step * 11,    y_mid - y_step * 5,   4},   // 40
                new int[] { x_mid + x_step * 12,    y_mid - y_step * 4,   4},   // 41
                new int[] { x_mid + x_step * 13,    y_mid - y_step * 3,   4},   // 42
                new int[] { x_mid + x_step * 14,    y_mid - y_step * 2,   3},   // 43
                new int[] { x_mid + x_step * 15,    y_mid - y_step * 1,   3},   // 44  right
                new int[] { x_mid + x_step * 15,    y_mid + y_step * 1,   3},   // 45  right
                new int[] { x_mid + x_step * 14,    y_mid + y_step * 2,   3},   // 46
                new int[] { x_mid + x_step * 13,    y_mid + y_step * 3,   3},   // 47
                new int[] { x_mid + x_step * 12,    y_mid + y_step * 4,   2},   // 48
                new int[] { x_mid + x_step * 11,    y_mid + y_step * 5,   2},   // 49
                new int[] { x_mid + x_step * 10,    y_mid + y_step * 6,   2},   // 50
                new int[] { x_mid + x_step * 09,    y_mid + y_step * 7,   2},   // 51
                new int[] { x_mid + x_step * 08,    y_mid + y_step * 8,   2},   // 52
                new int[] { x_mid + x_step * 07,    y_mid + y_step * 9,   1},   // 53
                new int[] { x_mid + x_step * 06,    y_mid + y_step * 10,  1},   // 54
                new int[] { x_mid + x_step * 05,    y_mid + y_step * 11,  1},   // 55
                new int[] { x_mid + x_step * 04,    y_mid + y_step * 12,  1},   // 56
                new int[] { x_mid + x_step * 03,    y_mid + y_step * 13,  1},   // 57
                new int[] { x_mid + x_step * 02,    y_mid + y_step * 14,  12},  // 58
                new int[] { x_mid + x_step * 01,    y_mid + y_step * 15,  12},  // 59  top circle
            };

            var controller = new TouchController();
            controller.Initialize();
            Console.CursorVisible = false;
            Console.WriteLine("Starting touch streams!");
            controller.StartTouchStream();
            Console.WriteLine("Started!");
            bool pressed = false;
            bool rx_pressed = false;
            bool rx_pressed_on_loop = false;
            bool pressed_on_loop = false;
            bool[] button_pressed = { false, false, false, false, false, false, false, false, false, false, false, false };  // 12 times false
            bool[] button_pressed_on_loop = { false, false, false, false, false, false, false, false, false, false, false, false };  // 12 times false
            while (true)
            {
                controller.GetTouchData();
                pressed_on_loop = false;
                rx_pressed_on_loop = false;
                for (int i = 0; i < 4; i++)
                {
                    for (int j = 0; j < 60; j++)
                    {
                        if (controller.touchData[i, j])
                        {
                            // i doesn't matter here. the circle has a Z axis of 4
                            // but we're mapping a joystick so no Z
                            // if j == 0 : then x = x_mid and y = y_max
                            // if j == 14 : then x = x_max and y = y_mid
                            // if j == 29 : then x = x_mid and y = y_min
                            // if j == 44 : then x = x_max and y = y_mid
                            joystick.SetAxis(axes[j][0], deviceId, HID_USAGES.HID_USAGE_X);
                            joystick.SetAxis(axes[j][1], deviceId, HID_USAGES.HID_USAGE_Y);
                            pressed_on_loop = true;
                            pressed = true;
                            if (i > 1)  // RXY is only on the two outer layers, i==2 and i==3
                            {
                                rx_pressed = true;
                                rx_pressed_on_loop = true;
                                joystick.SetAxis(axes[j][0], deviceId, HID_USAGES.HID_USAGE_RX);
                                joystick.SetAxis(axes[j][1], deviceId, HID_USAGES.HID_USAGE_RY);
                            } 
                            else
                            {
                                button_pressed_on_loop[axes[j][2]] = true;
                                if (!button_pressed[axes[j][2]])
                                {
                                    joystick.SetBtn(true, deviceId, (uint)axes[j][2]); // Press button axes[j][2]
                                    button_pressed[axes[j][2]] = true;
                                }
                            }
                        }
                    }
                }
                for (uint i = 0; i < 12; i++)
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
                    joystick.SetAxis(x_mid, deviceId, HID_USAGES.HID_USAGE_X);
                    joystick.SetAxis(y_mid, deviceId, HID_USAGES.HID_USAGE_Y);
                    pressed = false;
                }
                if (rx_pressed && !rx_pressed_on_loop)
                {
                    // Set joystick axis to midpoint
                    joystick.SetAxis(rx_mid, deviceId, HID_USAGES.HID_USAGE_RX);
                    joystick.SetAxis(ry_mid, deviceId, HID_USAGES.HID_USAGE_RY);
                    rx_pressed = false;
                }
            }
        }
    }
}