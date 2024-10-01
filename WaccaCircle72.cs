using LilyConsole;
using System;
using System.Collections.Generic;
using System.Linq;
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
                // Arrows(); // send up, down, left, or right key depending on where you touch
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

            double x_mid = (x_max - x_min) / 2;
            double y_mid = (y_max - y_min) / 2;
            double rx_mid = (rx_max - rx_min) / 2;
            double ry_mid = (ry_max - ry_min) / 2;
            double sl0_mid = (sl0_max - sl0_min) / 2;
            double sl1_mid = (sl1_max - sl1_min) / 2;
            double x_step = (x_max - x_min) / 30;
            double y_step = (y_max - y_min) / 30;
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
            {  //   x axis value                     y-axis value         1-12  13-16  17-20  21-22  23-24
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
            bool pressed = false;
            bool pressed_on_loop = false;
            bool rx_pressed = false;
            bool rx_pressed_on_loop = false;
            bool sl_pressed = false;
            bool sl_pressed_on_loop = false;
            /* bool[] button_pressed = { false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, };  // 48 times false
            bool[] button_pressed_on_loop = { false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, };  // 48 times false */
            bool[] button_pressed = Enumerable.Repeat(false, 72).ToArray();
            bool[] button_pressed_on_loop = Enumerable.Repeat(false, 72).ToArray();

            while (true)
            {
                controller.GetTouchData();
                pressed_on_loop = false;
                rx_pressed_on_loop = false;
                sl_pressed_on_loop = false;
                for (int i = 0; i < 4; i++)
                {
                    for (int j = 0; j < 60; j++)
                    {
                        if (controller.touchData[i, j])  // if the circle if touched
                        {
                            joystick.SetAxis(axes[j][0], deviceId, HID_USAGES.HID_USAGE_X);
                            joystick.SetAxis(axes[j][1], deviceId, HID_USAGES.HID_USAGE_Y);
                            pressed_on_loop = true;
                            pressed = true;
                            if (i > 1)  // RXY is only on the two outer layers, i==2 and i==3
                            {
                                for (int k = 2; k < 7; k++)  // parse axes columns
                                {
                                    button_pressed_on_loop[axes[j][k] + 48] = true;
                                    if (!button_pressed[axes[j][k] + 48])
                                    {
                                        joystick.SetBtn(true, deviceId, (uint)(axes[j][k] + 48)); // Press button axes[j][k] + 24
                                        button_pressed[axes[j][k] + 48] = true;
                                    }

                                    button_pressed_on_loop[axes[j][k] + 24] = true;
                                    if (!button_pressed[axes[j][k] + 24])
                                    {
                                        joystick.SetBtn(true, deviceId, (uint)(axes[j][k] + 24)); // Press button axes[j][k] + 24
                                        button_pressed[axes[j][k] + 24] = true;
                                    }
                                }
                                rx_pressed = true;
                                rx_pressed_on_loop = true;
                                joystick.SetAxis(axes[j][0], deviceId, HID_USAGES.HID_USAGE_RX);
                                joystick.SetAxis(axes[j][1], deviceId, HID_USAGES.HID_USAGE_RY);
                            }
                            else
                            {
                                for (int k = 2; k < 7; k++)
                                {
                                    button_pressed_on_loop[axes[j][k]] = true;
                                    if (!button_pressed[axes[j][k]])
                                    {
                                        joystick.SetBtn(true, deviceId, (uint)axes[j][k]); // Press button axes[j][k]
                                        button_pressed[axes[j][k]] = true;
                                    }

                                    button_pressed_on_loop[axes[j][k] + 24] = true;
                                    if (!button_pressed[axes[j][k] + 24])
                                    {
                                        joystick.SetBtn(true, deviceId, (uint)(axes[j][k] + 24)); // Press button axes[j][k] + 24
                                        button_pressed[axes[j][k] + 24] = true;
                                    }
                                }
                                sl_pressed = true;
                                sl_pressed_on_loop = true;
                                joystick.SetAxis(axes[j][0], deviceId, HID_USAGES.HID_USAGE_SL0);
                                joystick.SetAxis(axes[j][1], deviceId, HID_USAGES.HID_USAGE_SL1);
                            }
                        }
                    }
                }
                for (uint i = 0; i < 72; i++)
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
    }
}