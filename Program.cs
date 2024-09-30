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

        public static void TouchCombinedTest()
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
    }
}