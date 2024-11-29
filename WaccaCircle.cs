using LilyConsole;
using System;
using System.Linq;
using System.Threading;
using vJoyInterfaceWrap;
using SharpDX.DirectInput;
using System.IO;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Drawing;
using LilyConsole.Helpers;
//using WaccaCircle;  // needed to use WaccaTable.cs

namespace WaccaCircle
{
    internal class Class
    {
        static vJoy joystick = new vJoy();
        // Device ID (must be 1-16, based on vJoy configuration)
        static uint deviceId = 1;  // I compiled with this set to 1, 2, and 3
        static int LAG_DELAY = 50; // tweak between 0ms and 100ms to reduce CPU usage or increase responsiveness
        static long axis_max = 32767;
        static int canceled_value = 0;
        static Joystick ioboard;
        static Func<int>[] waccaCircleApps = { WaccaCircle12, WaccaCircle24, WaccaCircle32, WaccaCircle96, WaccaCircleTaiko,
                                               WaccaCircleSDVX, WaccaCircleRPG, WaccaCircleOsu, WaccaCircleCemu, WaccaCircleMouse, WaccaCircleKeyboard };
        static string[] waccaCircleText = { "WaccaCircle12", "WaccaCircle24", "WaccaCircle32", "WaccaCircle96", "WaccaCircleTaiko",
                                        "WaccaCircleSDVX", "WaccaCircleRPG", "WaccaCircleOsu", "WaccaCircleCemu", "WaccaCircleMouse", "WaccaCircleKeyboard" };
        static string exe_title = Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "WaccaCircleTitle.exe");
        static TouchController controller;
        static LightController lights;

        private delegate bool ConsoleCtrlHandlerDelegate(int sig);

        [DllImport("Kernel32")]
        private static extern bool SetConsoleCtrlHandler(ConsoleCtrlHandlerDelegate handler, bool add);

        static ConsoleCtrlHandlerDelegate _consoleCtrlHandler;

        [STAThread]
        public static void Main(string[] args)
        {
            WaccaTable.Initialize();
            ColorStorage.LoadAllColors();
            Console.WriteLine("Hello!");
            // Initialize DirectInput
            var directInput = new DirectInput();

            // Find a connected controller (joystick/gamepad)
            var joystickGuid = Guid.Empty;

            foreach (var deviceInstance in directInput.GetDevices(DeviceType.Gamepad, DeviceEnumerationFlags.AttachedOnly))
            {
                joystickGuid = deviceInstance.InstanceGuid;
            }

            // If no gamepad is found, search for a joystick
            if (joystickGuid == Guid.Empty)
            {
                foreach (var deviceInstance in directInput.GetDevices(DeviceType.Joystick, DeviceEnumerationFlags.AttachedOnly))
                {
                    joystickGuid = deviceInstance.InstanceGuid;
                }
            }

            // If still no device is found
            if (joystickGuid == Guid.Empty)
            {
                Console.WriteLine("DInput IOBoard not connected. Please plug in USB1");
                return;
            }

            Console.WriteLine("Hello!2");
            // Instantiate the joystick
            ioboard = new Joystick(directInput, joystickGuid);
            Console.WriteLine("Hello!3");

            Console.WriteLine($"Found Joystick/Gamepad: {ioboard.Information.ProductName}");

            Console.WriteLine("Hello!4");
            // Acquire the joystick
            ioboard.Acquire();
            Console.WriteLine("Hello!5");

            // Poll joystick state
            var state = new JoystickState();
            Console.WriteLine("Hello!6");

            Console.WriteLine("Press buttons to see their states. Press Ctrl+C to exit.");
            // Poll the joystick for input
            ioboard.Poll();
            state = ioboard.GetCurrentState();
            Console.WriteLine("Hello!7");

            // Get button states
            var buttons = state.Buttons;
            if (buttons.Length < 10)
            {
                Console.WriteLine("DInput IOBoard should have more than 10 buttons. Please plug in USB1");
                return;
            }
            if (SetupJoystick() == -1)
            {
                return;
            }

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

            controller = new TouchController();
            controller.Initialize();
            Console.CursorVisible = false;
            Console.WriteLine("Starting touch streams!");
            controller.StartTouchStream();
            Console.WriteLine("Started!");
            lights = new LightController();
            if (!lights.Initialize())
            {
                Console.WriteLine("Failed to load lights!");
            };

            _consoleCtrlHandler += s =>
            {
                CleanUpBeforeExit();
                return false;
            };
            SetConsoleCtrlHandler(_consoleCtrlHandler, true);
            int current = 0;
            int return_val;
            while (true)
            {
                try
                {
                    // Launch the overlay window
                    if (File.Exists(exe_title))
                    {
                        RunExternalCommand(exe_title, waccaCircleText[current]);
                    }
                    Console.WriteLine("Launching app");
                    return_val = waccaCircleApps[current]();
                    if (return_val == -2)
                    {
                        current += 1; // skip the app if it crashes?
                    }
                    else
                    {
                        current += return_val;
                        if (current < 0) { current = waccaCircleApps.Length - 1; }  // loop
                        if (current >= waccaCircleApps.Length) { current = 0; }  // loop
                    }
                }
                catch (Exception e)
                {

                    joystick.RelinquishVJD(deviceId);
                    Thread.Sleep(1000 + canceled_value);  // 0 unless ctrl + c is pressed
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
            }
        }
        public static void RunExternalCommand(string fileName, string arguments)
        {
            // Create a new process to start the external executable
            Process process = new Process();
            process.StartInfo.FileName = fileName;
            process.StartInfo.Arguments = arguments;
            process.StartInfo.UseShellExecute = false; // Optional, if you don't need output redirection

            // Start the process
            process.Start();

            // Process runs asynchronously, and the main thread is not blocked
        }
        static void CleanUpBeforeExit()
        {
            canceled_value = 2147480000;
            Console.WriteLine("ctrl+c detected!\ndisposing virtual Joystick....\ndone!\npress enter to exit...");
            // Release the device when done
            joystick.RelinquishVJD(deviceId);
            lights.CleanUp();
        }

        static bool[] ioboard_buttons = Enumerable.Repeat(false, 10).ToArray();

        /// <summary>
        /// Retrieves the value of the IO board once per press
        /// </summary>
        /// <returns>
        /// An integer representing the state of the IO board:
        /// - `-2`: if the program can't find ioboard
        /// - `0` if none of the specified buttons (0, 1, 6, 9) are pressed.
        /// - `1`: Volume Minus is pressed
        /// - `2`: Volume Plus is pressed
        /// - `3`: Both volumes are pressed
        /// - `4`: Service is pressed.
        /// - `5`: Service and Vol- are pressed.
        /// - `6`: Service and Vol+ are pressed.
        /// - `7`: Service and Vol+ and Vol- are pressed.
        /// - `8`: Test is pressed
        /// - `9`: Test and Vol- are pressed.
        /// - `10 (0xa)`: Test and Vol+ are pressed.
        /// - `11 (0xb)`: Test and Vol+ and Vol- are pressed.
        /// - `12 (0xc)`: Test and Service are pressed.
        /// - `13 (0xd)`: Test and Service and Vol- are pressed.
        /// - `14 (0xe)`: Test and Service and Vol+ are pressed.
        /// - `15 (0xf)`: Test and Service and Vol+ and Vol- are pressed.
        /// - `0x11`: Volume Minus is still pressed after consecutive polling
        /// - `0x12`: impossible
        /// - `0x13`: Volume Minus is still pressed after consecutive polling, and Vol+ has just been pressed
        /// - `0x14`: impossible
        /// - `0x15`: Volume Minus is still pressed after consecutive polling, and Service has just been pressed
        /// - `0x16`: impossible
        /// - `0x17`: Volume Minus is still pressed after consecutive polling, and both Vol+ and Service have just been pressed
        /// - `0x18`: impossible
        /// - `0x19`: Volume Minus is still pressed after consecutive polling, and Test has just been pressed
        /// - `0x1a`: impossible
        /// - `0x1b`: Volume Minus is still pressed after consecutive polling, and Test has just been pressed
        /// - `0x1c`: impossible
        /// - `0x1d`: Volume Minus is still pressed after consecutive polling, and both Test and Service have just been pressed
        /// - `0x1e`: impossible
        /// - `0x1f`: Volume Minus is still pressed after consecutive polling, and Test, Service, and Vol+ have just been pressed
        /// - `0x22`: Volume Plus is still pressed after consecutive polling
        /// - `0x33`: both Vol+ and Vol- are still pressed after consecutive polling
        /// ....
        /// - `0xff`: all buttons are still pressed after consecutive polling
        /// The return value is cumulative based on the buttons pressed.
        /// </returns>
        static int IOBoardPoll()
        {
            if (ioboard == null) return -2;
            ioboard.Poll();
            var state = ioboard.GetCurrentState();

            // Get button states
            var buttons = state.Buttons;
            int total = 0;
            if (buttons[0])
            {
                total += 1;
                if (ioboard_buttons[0])
                {
                    total += 1 << 4;
                }
                ioboard_buttons[0] = true;
            }
            if (buttons[1])
            {
                total += 2;
                if (ioboard_buttons[1])
                {
                    total += 2 << 4;
                }
                ioboard_buttons[1] = true;
            }
            if (buttons[6])
            {
                total += 4;
                if (ioboard_buttons[6])
                {
                    total += 4 << 4;
                }
                ioboard_buttons[6] = true;
            }
            if (buttons[9])
            {
                total += 8;
                if (ioboard_buttons[9])
                {
                    total += 8 << 4;
                }
                else
                {
                    if (File.Exists(Path.Combine(ahk, $"test.ahk")))
                    {
                        Process.Start(Path.Combine(ahk, $"test.ahk"));
                    }
                    else
                    {
                        Console.WriteLine($"failed to find " + Path.Combine(ahk, $"test.ahk"));
                    }
                }
                ioboard_buttons[9] = true;
            }
            for (int i = 0; i < ioboard_buttons.Length; i++)
            {
                if (buttons[i] == false && ioboard_buttons[i])
                {
                    ioboard_buttons[i] = false;
                }
            }
            return total;
        }

        // yup. defining an array is faster than doing maths
        // efficiency.
        static int[][] axes = WaccaTable.axes;
        
        static double x_mid;
        static double y_mid;
        static double rx_mid;
        static double ry_mid;
        static double sl0_mid;
        static double sl1_mid;
        static string ahk = Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "ahk");
        private static int SetupJoystick()
        {
            // Check if vJoy is enabled and ready
            if (!joystick.vJoyEnabled())
            {
                Console.WriteLine("vJoy driver not enabled: Failed to find vJoy.\npress enter to exit...");
                Console.ReadLine();
                return -1;
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
                    return -1;
                }
                Console.WriteLine("vJoy device acquired successfully.");
            }
            else
            {
                Console.WriteLine("vJoy device is not free. Status: " + status.ToString() + "\npress enter to exit...");
                Console.ReadLine();
                return -1;
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
            x_mid = (x_max - x_min) / 2;
            y_mid = (y_max - y_min) / 2;
            rx_mid = (rx_max - rx_min) / 2;
            ry_mid = (ry_max - ry_min) / 2;
            sl0_mid = (sl0_max - sl0_min) / 2;
            sl1_mid = (sl1_max - sl1_min) / 2;
            return 0;
        }
        private static int WaccaCircle12()
        {
            int a;
            /* bool[] button_pressed = { false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, };  // 48 times false
            bool[] button_pressed_on_loop = { false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, };  // 48 times false */
            bool[] button_pressed = Enumerable.Repeat(false, 13).ToArray();
            bool[] button_pressed_on_loop = Enumerable.Repeat(false, 13).ToArray();


            while (true)
            {
                Thread.Sleep(LAG_DELAY); // change this setting to 0ms, 100ms, 200ms, or 500ms.
                controller.GetTouchData();
                for (int i = 0; i < 4; i++)
                {
                    for (int j = 0; j < 60; j++)
                    {
                        if (controller.touchData[i, j])  // if the circle if touched
                        {
                            for (int k = 2; k < 3; k++)  // buttons from 1 to 12
                            {
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
                for (uint i = 1; i < 13; i++)
                {
                    if (button_pressed[i] && !button_pressed_on_loop[i])
                    {
                        joystick.SetBtn(false, deviceId, i); // Release button i
                        button_pressed[i] = false;
                    }
                    button_pressed_on_loop[i] = false;
                }
                WaccaTable.SendLight12(lights);
                a = IOBoardPoll();
                if (a == 1)
                {
                    return -1;  // scroll down
                }
                if (a == 2)
                {
                    return 1; // scroll up
                }
            }
        }
        private static int WaccaCircle24()
        {
            int a;
            bool pressed_on_loop;
            bool rx_pressed_on_loop;
            bool sl_pressed_on_loop;
            int x_current;
            int y_current;
            int rx_current;
            int ry_current;
            int sl0_current;
            int sl1_current;
            byte inner_number_of_pressed_panels;
            byte outer_number_of_pressed_panels;
            bool[] button_pressed = Enumerable.Repeat(false, 25).ToArray();
            bool[] button_pressed_on_loop = Enumerable.Repeat(false, 25).ToArray();


            while (true)
            {
                Thread.Sleep(LAG_DELAY); // tweak this setting between 0ms and 100ms.
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
                            if (i > 1)  // RXY is only on the two outer layers, i==2 and i==3
                            {
                                outer_number_of_pressed_panels++;
                                rx_current += axes[j][0];
                                ry_current += axes[j][1];
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
                                sl_pressed_on_loop = true;
                            }
                        }
                    }
                }
                if (pressed_on_loop)  // average all the axes towards the middle of all the pressed spots
                {
                    x_current = (sl0_current + rx_current) / (outer_number_of_pressed_panels + inner_number_of_pressed_panels);
                    y_current = (sl1_current + ry_current) / (outer_number_of_pressed_panels + inner_number_of_pressed_panels);
                    joystick.SetAxis(x_current, deviceId, HID_USAGES.HID_USAGE_X);
                    joystick.SetAxis(y_current, deviceId, HID_USAGES.HID_USAGE_Y);

                    if (inner_number_of_pressed_panels > 0)
                    {
                        sl0_current /= inner_number_of_pressed_panels;
                        sl1_current /= inner_number_of_pressed_panels;
                        joystick.SetAxis(sl0_current, deviceId, HID_USAGES.HID_USAGE_SL0);
                        joystick.SetAxis(sl1_current, deviceId, HID_USAGES.HID_USAGE_SL1);
                    }
                    if (outer_number_of_pressed_panels > 0)
                    {
                        rx_current /= outer_number_of_pressed_panels;
                        ry_current /= outer_number_of_pressed_panels;
                        joystick.SetAxis(rx_current, deviceId, HID_USAGES.HID_USAGE_RX);
                        joystick.SetAxis(ry_current, deviceId, HID_USAGES.HID_USAGE_RY);
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
                if (!sl_pressed_on_loop)
                {
                    // Set joystick axis to midpoint
                    joystick.SetAxis((int)sl0_mid, deviceId, HID_USAGES.HID_USAGE_SL0);
                    joystick.SetAxis((int)sl1_mid, deviceId, HID_USAGES.HID_USAGE_SL1);
                }

                WaccaTable.SendLight32(lights);
                a = IOBoardPoll();
                if (a == 1)
                {
                    return -1;  // scroll down
                }
                if (a == 2)
                {
                    return 1; // scroll up
                }
            }
        }
        private static int WaccaCircle32()
        {
            int a;
            bool pressed_on_loop;
            bool rx_pressed_on_loop;
            bool sl_pressed_on_loop;
            int x_current;
            int y_current;
            int rx_current;
            int ry_current;
            int sl0_current;
            int sl1_current;
            byte inner_number_of_pressed_panels;
            byte outer_number_of_pressed_panels;
            bool[] button_pressed = Enumerable.Repeat(false, 33).ToArray();
            bool[] button_pressed_on_loop = Enumerable.Repeat(false, 33).ToArray();

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
                            if (i > 1)  // RXY is only on the two outer layers, i==2 and i==3
                            {
                                outer_number_of_pressed_panels++;
                                rx_current += axes[j][0];
                                ry_current += axes[j][1];
                                for (int k = 4; k < 7; k++)  // outer buttons from 25 to 32
                                {
                                    button_pressed_on_loop[axes[j][k] + 8] = true;
                                    if (!button_pressed[axes[j][k] + 8])
                                    {
                                        joystick.SetBtn(true, deviceId, (uint)(axes[j][k] + 8)); // Press button axes[j][k] + 12
                                        button_pressed[axes[j][k] + 8] = true;
                                    }
                                }
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
                                sl_pressed_on_loop = true;
                            }
                        }
                    }
                }
                if (pressed_on_loop)  // average all the axes towards the middle of all the pressed spots
                {
                    x_current = (sl0_current + rx_current) / (outer_number_of_pressed_panels + inner_number_of_pressed_panels);
                    y_current = (sl1_current + ry_current) / (outer_number_of_pressed_panels + inner_number_of_pressed_panels);
                    joystick.SetAxis(x_current, deviceId, HID_USAGES.HID_USAGE_X);
                    joystick.SetAxis(y_current, deviceId, HID_USAGES.HID_USAGE_Y);

                    if (inner_number_of_pressed_panels > 0)
                    {
                        sl0_current /= inner_number_of_pressed_panels;
                        sl1_current /= inner_number_of_pressed_panels;
                        joystick.SetAxis(sl0_current, deviceId, HID_USAGES.HID_USAGE_SL0);
                        joystick.SetAxis(sl1_current, deviceId, HID_USAGES.HID_USAGE_SL1);
                    }
                    if (outer_number_of_pressed_panels > 0)
                    {
                        rx_current /= outer_number_of_pressed_panels;
                        ry_current /= outer_number_of_pressed_panels;
                        joystick.SetAxis(rx_current, deviceId, HID_USAGES.HID_USAGE_RX);
                        joystick.SetAxis(ry_current, deviceId, HID_USAGES.HID_USAGE_RY);
                    }
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
                if (!sl_pressed_on_loop)
                {
                    // Set joystick axis to midpoint
                    joystick.SetAxis((int)sl0_mid, deviceId, HID_USAGES.HID_USAGE_SL0);
                    joystick.SetAxis((int)sl1_mid, deviceId, HID_USAGES.HID_USAGE_SL1);
                }

                WaccaTable.SendLight32(lights);
                a = IOBoardPoll();
                if (a == 1)
                {
                    return -1;  // scroll down
                }
                if (a == 2)
                {
                    return 1; // scroll up
                }
            }
        }
        private static int WaccaCircle96()
        {
            int a;
            bool pressed_on_loop;
            bool rx_pressed_on_loop;
            bool sl_pressed_on_loop;
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
            bool[] button_pressed = Enumerable.Repeat(false, 97).ToArray();
            bool[] button_pressed_on_loop = Enumerable.Repeat(false, 97).ToArray();
            byte n = 32;
            int n2 = 64;

            LightFrame gradientFrame = new LightFrame
            {
                layers =
                {
                    [0] = LightPatternGenerator.Gradient(LightColor.Blue, LightColor.Red),
                }
            };

            lights.SendLightFrame(gradientFrame);

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
                            if (i > 1)  // RXY is only on the two outer layers, i==2 and i==3
                            {
                                outer_number_of_pressed_panels++;
                                rx_current += axes[j][0];
                                ry_current += axes[j][1];
                                for (int k = 2; k < 8; k++)  // parse axes columns
                                {
                                    button_pressed_on_loop[axes[j][k] + n2] = true;
                                    if (!button_pressed[axes[j][k] + n2])
                                    {
                                        joystick.SetBtn(true, deviceId, (uint)(axes[j][k] + n2)); // Press button axes[j][k] + 24
                                        button_pressed[axes[j][k] + n2] = true;
                                    }

                                    button_pressed_on_loop[axes[j][k] + n] = true;
                                    if (!button_pressed[axes[j][k] + n])
                                    {
                                        joystick.SetBtn(true, deviceId, (uint)(axes[j][k] + n)); // Press button axes[j][k] + 24
                                        button_pressed[axes[j][k] + n] = true;
                                    }
                                }
                                rx_pressed_on_loop = true;
                            }
                            else
                            {
                                inner_number_of_pressed_panels++;
                                sl0_current += axes[j][0];
                                sl1_current += axes[j][1];
                                for (int k = 2; k < 8; k++)
                                {
                                    button_pressed_on_loop[axes[j][k]] = true;
                                    if (!button_pressed[axes[j][k]])
                                    {
                                        joystick.SetBtn(true, deviceId, (uint)axes[j][k]); // Press button axes[j][k]
                                        button_pressed[axes[j][k]] = true;
                                    }

                                    button_pressed_on_loop[axes[j][k] + n] = true;
                                    if (!button_pressed[axes[j][k] + n])
                                    {
                                        joystick.SetBtn(true, deviceId, (uint)(axes[j][k] + n)); // Press button axes[j][k] + 24
                                        button_pressed[axes[j][k] + n] = true;
                                    }
                                }
                                sl_pressed_on_loop = true;
                            }
                        }
                    }
                }
                if (pressed_on_loop)  // average all the axes towards the middle of all the pressed spots
                {
                    x_current = (sl0_current + rx_current) / (outer_number_of_pressed_panels + inner_number_of_pressed_panels);
                    y_current = (sl1_current + ry_current) / (outer_number_of_pressed_panels + inner_number_of_pressed_panels);
                    joystick.SetAxis(x_current, deviceId, HID_USAGES.HID_USAGE_X);
                    joystick.SetAxis(y_current, deviceId, HID_USAGES.HID_USAGE_Y);

                    if (inner_number_of_pressed_panels > 0)
                    {
                        sl0_current /= inner_number_of_pressed_panels;
                        sl1_current /= inner_number_of_pressed_panels;
                        joystick.SetAxis(sl0_current, deviceId, HID_USAGES.HID_USAGE_SL0);
                        joystick.SetAxis(sl1_current, deviceId, HID_USAGES.HID_USAGE_SL1);
                    }
                    if (outer_number_of_pressed_panels > 0)
                    {
                        rx_current /= outer_number_of_pressed_panels;
                        ry_current /= outer_number_of_pressed_panels;
                        joystick.SetAxis(rx_current, deviceId, HID_USAGES.HID_USAGE_RX);
                        joystick.SetAxis(ry_current, deviceId, HID_USAGES.HID_USAGE_RY);
                    }
                }
                for (uint i = 1; i < 97; i++)
                {
                    if (button_pressed[i] && !button_pressed_on_loop[i])
                    {
                        joystick.SetBtn(false, deviceId, i); // Release button i
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
                if (!sl_pressed_on_loop)
                {
                    // Set joystick axis to midpoint
                    joystick.SetAxis((int)sl0_mid, deviceId, HID_USAGES.HID_USAGE_SL0);
                    joystick.SetAxis((int)sl1_mid, deviceId, HID_USAGES.HID_USAGE_SL1);
                }
                WaccaTable.SendLight32(lights);
                a = IOBoardPoll();
                if (a == 1)
                {
                    return -1;  // scroll down
                }
                if (a == 2)
                {
                    return 1; // scroll up
                }
            }
        }
        private static int WaccaCircleTaiko()
        {
            int a;
            bool[] button_pressed = Enumerable.Repeat(false, 17).ToArray();
            bool[] button_pressed_on_loop = Enumerable.Repeat(false, 17).ToArray();
            sbyte u = -11;
            sbyte s = -9;
            while (true)
            {
                Thread.Sleep(LAG_DELAY); // tweak this setting between 0ms and 100ms.
                controller.GetTouchData();
                for (int i = 0; i < 4; i++)
                {
                    for (int j = 0; j < 60; j++)
                    {
                        if (controller.touchData[i, j])  // if the circle if touched
                        {
                            if (i > 1)  // RXY is only on the two outer layers, i==2 and i==3
                            {
                                for (int k = 6; k < 7; k++)  // outer buttons from 1 to 24
                                {
                                    button_pressed_on_loop[axes[j][k] + u] = true;  // 23 - 11 = 12
                                    if (!button_pressed[axes[j][k] + u])
                                    {
                                        joystick.SetBtn(true, deviceId, (uint)(axes[j][k] + u)); // Press button axes[j][k]
                                        button_pressed[axes[j][k] + u] = true;
                                    }
                                }
                            }
                            else
                            {
                                for (int k = 6; k < 7; k++)  // inner buttons from 1 to 24
                                {
                                    button_pressed_on_loop[axes[j][k] + s] = true;  // 23 - 9 = 14
                                    if (!button_pressed[axes[j][k] + s])
                                    {
                                        joystick.SetBtn(true, deviceId, (uint)(axes[j][k] + s)); // Press button axes[j][k]
                                        button_pressed[axes[j][k] + s] = true;
                                    }
                                }
                            }
                        }
                    }
                }
                for (uint i = 1; i < 17; i++)
                {
                    if (button_pressed[i] && !button_pressed_on_loop[i])
                    {
                        joystick.SetBtn(false, deviceId, i); // Release button i
                        button_pressed[i] = false;
                    }
                    button_pressed_on_loop[i] = false;
                }
                WaccaTable.SendLightTaiko(lights);
                a = IOBoardPoll();
                if (a == 1)
                {
                    return -1;  // scroll down
                }
                if (a == 2)
                {
                    return 1; // scroll up
                }
            }
        }
        private static int WaccaCircleSDVX()
        {
            int a;
            // yup. defining an array is faster than doing maths
            // efficiency.
            int[][] axes = WaccaTable.SDVXaxes;
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
                                    button_pressed_on_loop[axes[j][k]] = true;
                                    if (!button_pressed[axes[j][k]])
                                    {
                                        if (axes[j][k] == 10)
                                        {
                                            state++;
                                            if (state == 3)
                                            {
                                                state = 0;
                                            }
                                        }
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
                WaccaTable.SendLightSDVX(lights, state);
                a = IOBoardPoll();
                if (a == 1)
                {
                    return -1;  // scroll down
                }
                if (a == 2)
                {
                    return 1; // scroll up
                }
            }
        } // custom axes
        private static int WaccaCircleRPG()
        {
            int a;
            int[][] axes = WaccaTable.RPGaxes;
            bool[] button_pressed = Enumerable.Repeat(false, 17).ToArray();
            bool[] button_pressed_on_loop = Enumerable.Repeat(false, 17).ToArray();

            while (true)
            {
                Thread.Sleep(LAG_DELAY); // tweak this setting between 0ms and 100ms.
                controller.GetTouchData();
                for (int i = 0; i < 4; i++)
                {
                    for (int j = 0; j < 60; j++)
                    {
                        if (controller.touchData[i, j])  // if the circle if touched
                        {
                            if (i > 1)  // RXY is only on the two outer layers, i==2 and i==3
                            {
                                for (int k = 4; k < 5; k++)  // inner buttons from 1 to 24
                                {
                                    button_pressed_on_loop[axes[j][k]] = true;
                                    if (!button_pressed[axes[j][k]])
                                    {
                                        joystick.SetBtn(true, deviceId, (uint)axes[j][k]); // Press button axes[j][k]
                                        button_pressed[axes[j][k]] = true;
                                    }
                                }
                            }
                            else
                            {
                                for (int k = 3; k < 4; k++)  // inner buttons from 1 to 24
                                {
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
                for (uint i = 1; i < 17; i++)
                {
                    if (button_pressed[i] && !button_pressed_on_loop[i])
                    {
                        joystick.SetBtn(false, deviceId, i); // Release button i
                        button_pressed[i] = false;
                    }
                    button_pressed_on_loop[i] = false;
                }
                WaccaTable.SendLightRPG(lights);
                a = IOBoardPoll();
                if (a == 1)
                {
                    return -1;  // scroll down
                }
                if (a == 2)
                {
                    return 1; // scroll up
                }
            }
        }  // custom axes
        private static int WaccaCircleOsu()
        {
            int a;
            bool[] button_pressed = Enumerable.Repeat(false, 33).ToArray();
            bool[] button_pressed_on_loop = Enumerable.Repeat(false, 33).ToArray();
            bool[] keydown = Enumerable.Repeat(false, 33).ToArray();
            sbyte n = -24;
            sbyte s = -20;
            sbyte u = -16;
            while (true)
            {
                Thread.Sleep(LAG_DELAY); // change this setting to 0ms, 100ms, 200ms, or 500ms.
                controller.GetTouchData();
                for (int i = 0; i < 4; i++)
                {
                    for (int j = 0; j < 60; j++)
                    {
                        if (controller.touchData[i, j])  // if the circle if touched
                        {
                            if (i > 1)  // buttons only on the two outer layers, i==2 and i==3
                            {
                                for (int k = 4; k < 5; k++)  // parse axes columns, split by 4 (x)
                                {
                                    if (!button_pressed[axes[j][k] + u])  // starts at 17 + -16 = 1
                                    {
                                        button_pressed[axes[j][k] + u] = true;
                                        if (File.Exists(Path.Combine(ahk, $"osu{axes[j][k] + u}d.ahk")))  // ends at 20 + -16 = 4
                                        {

                                            Process.Start(Path.Combine(ahk, $"osu{axes[j][k] + u}d.ahk"));
                                            keydown[axes[j][k] + u] = true;
                                        }
                                        else
                                        {
                                            Console.WriteLine($"failed to find " + Path.Combine(ahk, $"osu{axes[j][k] + u}d.ahk"));
                                        }
                                    }
                                    button_pressed_on_loop[axes[j][k] + u] = true;
                                }
                            }
                            for (int k = 7; k < 8; k++)  // buttons from 1 to 8, but mapped internally from 5 to 12
                            {
                                button_pressed_on_loop[axes[j][k] + s] = true;
                                if (!button_pressed[axes[j][k] + s])
                                {
                                    joystick.SetBtn(true, deviceId, (uint)(axes[j][k] + n)); // Press button axes[j][k]
                                    button_pressed[axes[j][k] + s] = true;
                                }

                            }
                        }
                    }
                }
                for (uint i = 1; i < 13; i++)
                {
                    if (button_pressed[i] && !button_pressed_on_loop[i])
                    {
                        joystick.SetBtn(false, deviceId, i); // Release button i
                        button_pressed[i] = false;
                        if (i < 5) // 1 to 4
                        {
                            if (keydown[i])
                            {
                                if (File.Exists(Path.Combine(ahk, $"osu{i}u.ahk")))
                                {
                                    Process.Start(Path.Combine(ahk, $"osu{i}u.ahk"));
                                }
                                else
                                {
                                    Console.WriteLine($"failed to find " + Path.Combine(ahk, $"osu{i}u.ahk"));
                                }
                                keydown[i] = false;
                            }
                        }
                        else
                        {
                            joystick.SetBtn(false, deviceId, (uint)(i + n - s));  // 5 - 24 + 20 should be 1
                        }
                    }
                    button_pressed_on_loop[i] = false;
                }
                WaccaTable.SendLightOsu(lights);
                a = IOBoardPoll();
                if (a == 1)
                {
                    return -1;  // scroll down
                }
                if (a == 2)
                {
                    return 1; // scroll up
                }
            }
        }  // TODO: buttons to press enter and escape
        private static int WaccaCircleCemu()
        {
            int a;
            bool rx_pressed_on_loop;
            int x_current;
            int y_current;
            byte outer_number_of_pressed_panels;
            bool[] button_pressed = Enumerable.Repeat(false, 29).ToArray();
            bool[] button_pressed_on_loop = Enumerable.Repeat(false, 29).ToArray();

            while (true)
            {
                Thread.Sleep(LAG_DELAY); // change this setting to 0ms, 100ms, 200ms, or 500ms.
                controller.GetTouchData();
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
                            if (i > 1)  // RXY is only on the two outer layers, i==2 and i==3
                            {
                                outer_number_of_pressed_panels++;
                                x_current += axes[j][0];
                                y_current += axes[j][1];
                                for (int k = 4; k < 5; k++)  // outer buttons from 25 to 28
                                {
                                    button_pressed_on_loop[axes[j][k] + 8] = true;
                                    if (!button_pressed[axes[j][k] + 8])
                                    {   // don't send input yet! end for the circle scan to end
                                        // joystick.SetBtn(true, deviceId, (uint)(axes[j][k] + 8)); // Press button axes[j][k] + 12
                                        button_pressed[axes[j][k] + 8] = true;
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
                                        if (axes[j][k] == 1 || axes[j][k] == 3 || axes[j][k] == 9 || axes[j][k] == 11)
                                        {
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
                    if (button_pressed[1]) // 4 buttons
                    {
                        for (uint i = 25; i < 29; i++)
                        {
                            if (button_pressed[i])
                            {
                                joystick.SetBtn(true, deviceId, i - 12); // Press button 13 to 16
                                button_pressed_on_loop[i - 12] = true;
                                button_pressed[i - 12] = true;
                            }
                        }
                    }
                    else if (button_pressed[3])  // right stick
                    {
                        if (!button_pressed_on_loop[3])
                        {
                            // Set joystick axis to midpoint
                            joystick.SetAxis((int)rx_mid, deviceId, HID_USAGES.HID_USAGE_RX);
                            joystick.SetAxis((int)ry_mid, deviceId, HID_USAGES.HID_USAGE_RY);
                        }
                        else
                        {
                            x_current /= outer_number_of_pressed_panels;
                            y_current /= outer_number_of_pressed_panels;
                            joystick.SetAxis(x_current, deviceId, HID_USAGES.HID_USAGE_RX);
                            joystick.SetAxis(y_current, deviceId, HID_USAGES.HID_USAGE_RY);
                        }
                    }
                    else if (button_pressed[9]) // D-PAD
                    {
                        for (uint i = 25; i < 29; i++)
                        {
                            if (button_pressed_on_loop[i])
                            {
                                joystick.SetBtn(true, deviceId, i - 8); // Press button 17 to 20
                                button_pressed_on_loop[i - 8] = true;
                                button_pressed[i - 8] = true;
                            }
                        }
                    }
                    else if (button_pressed[11]) // 4 buttons 
                    {
                        for (uint i = 25; i < 29; i++)
                        {
                            if (button_pressed_on_loop[i])
                            {
                                joystick.SetBtn(true, deviceId, i - 4); // Press button 21 to 24
                                button_pressed_on_loop[i - 4] = true;
                                button_pressed[i - 4] = true;
                            }
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
                else  // rx not pressed on loop
                {
                    // Set joystick axis to midpoint
                    joystick.SetAxis((int)x_mid, deviceId, HID_USAGES.HID_USAGE_X);
                    joystick.SetAxis((int)y_mid, deviceId, HID_USAGES.HID_USAGE_Y);
                    // Set joystick axis to midpoint
                    joystick.SetAxis((int)rx_mid, deviceId, HID_USAGES.HID_USAGE_RX);
                    joystick.SetAxis((int)ry_mid, deviceId, HID_USAGES.HID_USAGE_RY);
                }
                for (uint i = 1; i < 29; i++)
                {
                    if (button_pressed[i] && !button_pressed_on_loop[i])
                    {
                        if (i > 24)
                        {
                            //pass
                        }
                        else if (i == 1 || i == 3 || i == 9 || i == 11)
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
                WaccaTable.SendLight32(lights);
                a = IOBoardPoll();
                if (a == 1)
                {
                    return -1;  // scroll down
                }
                if (a == 2)
                {
                    return 1; // scroll up
                }
            }
        }
        private static int WaccaCircleKeyboard()
        {
            int a;
            bool[] button_pressed = Enumerable.Repeat(false, 49).ToArray();  // 48 + 1 since I made my table start at 1
            bool[] button_pressed_on_loop = Enumerable.Repeat(false, 49).ToArray();
            bool[] keydown = Enumerable.Repeat(false, 49).ToArray();
            sbyte n;
            const sbyte u = -16;
            const sbyte w = 28;
            sbyte x;
            string key = "key";
            while (true)
            {
                Thread.Sleep(LAG_DELAY); // 0ms uses 35% CPU while 5ms uses 4% CPU.
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
                                    n = 0;
                                    x = 32;
                                    key = "key";
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
                for (uint i = 33; i < 49; i++)
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
                            else
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
                            keydown[i] = false;
                        }
                        button_pressed[i] = false;
                    }
                    button_pressed_on_loop[i] = false;
                }
                WaccaTable.SendLight12(lights);
                a = IOBoardPoll();
                if (a == 1)
                {
                    return -1;  // scroll down
                }
                if (a == 2)
                {
                    return 1; // scroll up
                }
            }
        }
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
        private static int WaccaCircleMouse()
        {
            int a;
            int[][] axes = WaccaTable.mouseAxes;
            Point startPos = Cursor.Position;
            Point endPos;
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
                WaccaTable.SendLightMouse(lights);
                a = IOBoardPoll();
                if (a == 1)
                {
                    return -1;  // scroll down
                }
                if (a == 2)
                {
                    return 1; // scroll up
                }
            }  // end while(true)
        }  // end of Mouse()
    }  // end of Class
}  // end of namespace