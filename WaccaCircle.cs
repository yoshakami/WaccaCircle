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
using System.Threading.Tasks;

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
        static Func<int>[] waccaCircleApps = { WaccaCircle12, WaccaCircle24, WaccaCircle32, WaccaCircle96, WaccaCircleTaiko, WaccaCircleSDVX, WaccaCircleRPG, WaccaCircleOsu, WaccaCircleCemu, WaccaCircleMouse };
        static string[] waccaCircleText = { "WaccaCircle12", "WaccaCircle24", "WaccaCircle32", "WaccaCircle96", "WaccaCircleTaiko", "WaccaCircleSDVX", "WaccaCircleRPG", "WaccaCircleOsu", "WaccaCircleCemu", "WaccaCircleMouse", };
        static TouchController controller;

        private delegate bool ConsoleCtrlHandlerDelegate(int sig);

        [DllImport("Kernel32")]
        private static extern bool SetConsoleCtrlHandler(ConsoleCtrlHandlerDelegate handler, bool add);

        static ConsoleCtrlHandlerDelegate _consoleCtrlHandler;

        public static void Main(string[] args)
        {
            _consoleCtrlHandler += s =>
            {
                CleanUpBeforeExit();
                return false;
            };
            SetConsoleCtrlHandler(_consoleCtrlHandler, true);

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

            // Instantiate the joystick
            ioboard = new Joystick(directInput, joystickGuid);

            Console.WriteLine($"Found Joystick/Gamepad: {ioboard.Information.ProductName}");

            // Acquire the joystick
            ioboard.Acquire();

            // Poll joystick state
            var state = new JoystickState();

            Console.WriteLine("Press buttons to see their states. Press Ctrl+C to exit.");
            // Poll the joystick for input
            ioboard.Poll();
            state = ioboard.GetCurrentState();

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


            int current = 0;
            int return_val;
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            while (true)
            {
                try
                {
                    // Launch the overlay window
                    Thread overlayThread = new Thread(() =>
                    {
                        Application.Run(new TransparentOverlay(waccaCircleText[current]));
                    });

                    overlayThread.SetApartmentState(ApartmentState.STA); // Required for WinForms
                    overlayThread.Start();
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

        static void CleanUpBeforeExit()
        {
            canceled_value = 2147480000;
            Console.WriteLine("ctrl+c detected!\ndisposing virtual Joystick....\ndone!\npress enter to exit...");
            // Release the device when done
            joystick.RelinquishVJD(deviceId);
            Console.ReadLine();
            Environment.Exit(0);
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
        static int[][] axes =
        {          //       x axis    y-axis   1-12  13-16  17-20  21-22  23-24  25-32
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
            int[][] axes =
           {      // RPG   x axis    y-axis   1-12   1-7   13-16  21-22  23-24  25-32
                new int[] { X(0.5),   Y(0.5),   12,   4,    13,    21,    23,    25},    // 0  top circle
                new int[] { X(1.5),   Y(1.5),   12,   4,    13,    21,    23,    25},    // 1
                new int[] { X(2.5),   Y(2.5),   12,   4,    13,    21,    23,    25},    // 2
                new int[] { X(3.5),   Y(3.5),   11,   4,    13,    21,    23,    25},    // 3
                new int[] { X(4.5),   Y(4.5),   11,   4,    13,    21,    23,    25},    // 4
                new int[] { X(5.5),   Y(5.5),   11,   4,    13,    21,    23,    25},    // 5
                new int[] { X(6.5),   Y(6.5),   11,   4,    13,    21,    23,    25},    // 6
                new int[] { X(7.5),   Y(7.5),   11,   4,    13,    21,    23,    25},    // 7
                new int[] { X(8.5),   Y(8.5),   10,   1,    15,    21,    23,    26},    // 8
                new int[] { X(9.5),   Y(9.5),   10,   1,    15,    21,    23,    26},    // 9
                new int[] { X(10.5),  Y(10.5),  10,   1,    15,    21,    23,    26},    // 10
                new int[] { X(11.5),  Y(11.5),  10,   1,    15,    21,    23,    26},    // 11
                new int[] { X(12.5),  Y(12.5),  10,   1,    15,    21,    23,    26},    // 12
                new int[] { X(13.5),  Y(13.5),   9,   1,    15,    21,    23,    26},    // 13
                new int[] { X(14.5),  Y(14.5),   9,   1,    15,    21,    23,    26},    // 14  left
                new int[] { X(15.5),  Y(15.5),   9,   1,    15,    22,    23,    26},    // 15  left 
                new int[] { X(16.5),  Y(16.5),   9,   1,    15,    22,    23,    27},    // 16
                new int[] { X(17.5),  Y(17.5),   9,   1,    15,    22,    23,    27},    // 17
                new int[] { X(18.5),  Y(18.5),   8,   1,    15,    22,    23,    27},    // 18
                new int[] { X(19.5),  Y(19.5),   8,   1,    15,    22,    23,    27},    // 19
                new int[] { X(20.5),  Y(20.5),   8,   1,    15,    22,    23,    27},    // 20
                new int[] { X(21.5),  Y(21.5),   8,   1,    15,    22,    23,    27},    // 21
                new int[] { X(22.5),  Y(22.5),   8,   1,    15,    22,    23,    27},    // 22
                new int[] { X(23.5),  Y(23.5),   7,   7,    14,    22,    23,    28},    // 23
                new int[] { X(24.5),  Y(24.5),   7,   7,    14,    22,    23,    28},    // 24
                new int[] { X(25.5),  Y(25.5),   7,   7,    14,    22,    23,    28},    // 25
                new int[] { X(26.5),  Y(26.5),   7,   7,    14,    22,    23,    28},    // 26
                new int[] { X(27.5),  Y(27.5),   7,   7,    14,    22,    23,    28},    // 27
                new int[] { X(28.5),  Y(28.5),   6,   7,    14,    22,    23,    28},    // 28
                new int[] { X(29.5),  Y(29.5),   6,   7,    14,    22,    23,    28},    // 29  bottom
                new int[] { X(30.5),  Y(30.5),   6,   7,    14,    22,    24,    29},    // 30  bottom
                new int[] { X(31.5),  Y(31.5),   6,   7,    14,    22,    24,    29},    // 31
                new int[] { X(32.5),  Y(32.5),   6,   7,    14,    22,    24,    29},    // 32
                new int[] { X(33.5),  Y(33.5),   5,   7,    14,    22,    24,    29},    // 33
                new int[] { X(34.5),  Y(34.5),   5,   7,    14,    22,    24,    29},    // 34
                new int[] { X(35.5),  Y(35.5),   5,   7,    14,    22,    24,    29},    // 35
                new int[] { X(36.5),  Y(36.5),   5,   7,    14,    22,    24,    29},    // 36
                new int[] { X(37.5),  Y(37.5),   5,   7,    14,    22,    24,    30},    // 37
                new int[] { X(38.5),  Y(38.5),   4,   2,    16,    22,    24,    30},    // 38
                new int[] { X(39.5),  Y(39.5),   4,   2,    16,    22,    24,    30},    // 39
                new int[] { X(40.5),  Y(40.5),   4,   2,    16,    22,    24,    30},    // 40
                new int[] { X(41.5),  Y(41.5),   4,   2,    16,    22,    24,    30},    // 41
                new int[] { X(42.5),  Y(42.5),   4,   2,    16,    22,    24,    30},    // 42
                new int[] { X(43.5),  Y(43.5),   3,   2,    16,    22,    24,    30},    // 43
                new int[] { X(44.5),  Y(44.5),   3,   2,    16,    22,    24,    31},    // 44  right
                new int[] { X(45.5),  Y(45.5),   3,   2,    16,    21,    24,    31},    // 45  right
                new int[] { X(46.5),  Y(46.5),   3,   2,    16,    21,    24,    31},    // 46
                new int[] { X(47.5),  Y(47.5),   3,   2,    16,    21,    24,    31},    // 47
                new int[] { X(48.5),  Y(48.5),   2,   2,    16,    21,    24,    31},    // 48
                new int[] { X(49.5),  Y(49.5),   2,   2,    16,    21,    24,    31},    // 49
                new int[] { X(50.5),  Y(50.5),   2,   2,    16,    21,    24,    31},    // 50
                new int[] { X(51.5),  Y(51.5),   2,   2,    16,    21,    24,    31},    // 51
                new int[] { X(52.5),  Y(52.5),   2,   2,    16,    21,    24,    32},    // 52
                new int[] { X(53.5),  Y(53.5),   1,   4,    13,    21,    24,    32},    // 53
                new int[] { X(54.5),  Y(54.5),   1,   4,    13,    21,    24,    32},    // 54
                new int[] { X(55.5),  Y(55.5),   1,   4,    13,    21,    24,    32},    // 55
                new int[] { X(56.5),  Y(56.5),   1,   4,    13,    21,    24,    32},    // 56
                new int[] { X(57.5),  Y(57.5),   1,   4,    13,    21,    24,    32},    // 57
                new int[] { X(58.5),  Y(58.5),  12,   4,    13,    21,    24,    32},    // 58
                new int[] { X(59.5),  Y(59.5),  12,   4,    13,    21,    24,    32},    // 59  top circle
            };
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

        private static int B(double v)
        {
            // Use Cos to calculate the Y position and shift it to the range [-axis_max, axis_max]
            return (int)((Math.Cos(v * Math.PI / 30)) * 10);
        }

        private static int A(double v)
        {
            // Use -Sin to calculate the X position and shift it to the range [-axis_max, axis_max]
            return (int)((-Math.Sin(v * Math.PI / 30)) * 10);
        }
        private static int WaccaCircleMouse()
        {
            int a;
            int[][] axes =
            {      //       x axis    y-axis   1-12  13-16  17-20  21-22  23-24  25-32
                new int[] { A(0.5),   B(0.5),   12,   16,    17,    21,    23,    25},    // 0  top circle
                new int[] { A(1.5),   B(1.5),   12,   16,    17,    21,    23,    25},    // 1
                new int[] { A(2.5),   B(2.5),   12,   16,    17,    21,    23,    25},    // 2
                new int[] { A(3.5),   B(3.5),   11,   16,    17,    21,    23,    25},    // 3
                new int[] { A(4.5),   B(4.5),   11,   16,    17,    21,    23,    25},    // 4
                new int[] { A(5.5),   B(5.5),   11,   16,    17,    21,    23,    25},    // 5
                new int[] { A(6.5),   B(6.5),   11,   16,    17,    21,    23,    25},    // 6
                new int[] { A(7.5),   B(7.5),   11,   16,    17,    21,    23,    25},    // 7
                new int[] { A(8.5),   B(8.5),   10,   16,    20,    21,    23,    26},    // 8
                new int[] { A(9.5),   B(9.5),   10,   16,    20,    21,    23,    26},    // 9
                new int[] { A(10.5),  B(10.5),  10,   16,    20,    21,    23,    26},    // 10
                new int[] { A(11.5),  B(11.5),  10,   16,    20,    21,    23,    26},    // 11
                new int[] { A(12.5),  B(12.5),  10,   16,    20,    21,    23,    26},    // 12
                new int[] { A(13.5),  B(13.5),   9,   16,    20,    21,    23,    26},    // 13
                new int[] { A(14.5),  B(14.5),   9,   16,    20,    21,    23,    26},    // 14  left
                new int[] { A(15.5),  B(15.5),   9,   15,    20,    22,    23,    26},    // 15  left 
                new int[] { A(16.5),  B(16.5),   9,   15,    20,    22,    23,    27},    // 16
                new int[] { A(17.5),  B(17.5),   9,   15,    20,    22,    23,    27},    // 17
                new int[] { A(18.5),  B(18.5),   8,   15,    20,    22,    23,    27},    // 18
                new int[] { A(19.5),  B(19.5),   8,   15,    20,    22,    23,    27},    // 19
                new int[] { A(20.5),  B(20.5),   8,   15,    20,    22,    23,    27},    // 20
                new int[] { A(21.5),  B(21.5),   8,   15,    20,    22,    23,    27},    // 21
                new int[] { A(22.5),  B(22.5),   8,   15,    20,    22,    23,    27},    // 22
                new int[] { A(23.5),  B(23.5),   7,   15,    19,    22,    23,    28},    // 23
                new int[] { A(24.5),  B(24.5),   7,   15,    19,    22,    23,    28},    // 24
                new int[] { A(25.5),  B(25.5),   7,   15,    19,    22,    23,    28},    // 25
                new int[] { A(26.5),  B(26.5),   7,   15,    19,    22,    23,    28},    // 26
                new int[] { A(27.5),  B(27.5),   7,   15,    19,    22,    23,    28},    // 27
                new int[] { A(28.5),  B(28.5),   6,   15,    19,    22,    23,    28},    // 28
                new int[] { A(29.5),  B(29.5),   6,   15,    19,    22,    23,    28},    // 29  bottom
                new int[] { A(30.5),  B(30.5),   6,   14,    19,    22,    24,    29},    // 30  bottom
                new int[] { A(31.5),  B(31.5),   6,   14,    19,    22,    24,    29},    // 31
                new int[] { A(32.5),  B(32.5),   6,   14,    19,    22,    24,    29},    // 32
                new int[] { A(33.5),  B(33.5),   5,   14,    19,    22,    24,    29},    // 33
                new int[] { A(34.5),  B(34.5),   5,   14,    19,    22,    24,    29},    // 34
                new int[] { A(35.5),  B(35.5),   5,   14,    19,    22,    24,    29},    // 35
                new int[] { A(36.5),  B(36.5),   5,   14,    19,    22,    24,    29},    // 36
                new int[] { A(37.5),  B(37.5),   5,   14,    19,    22,    24,    30},    // 37
                new int[] { A(38.5),  B(38.5),   4,   14,    18,    22,    24,    30},    // 38
                new int[] { A(39.5),  B(39.5),   4,   14,    18,    22,    24,    30},    // 39
                new int[] { A(40.5),  B(40.5),   4,   14,    18,    22,    24,    30},    // 40
                new int[] { A(41.5),  B(41.5),   4,   14,    18,    22,    24,    30},    // 41
                new int[] { A(42.5),  B(42.5),   4,   14,    18,    22,    24,    30},    // 42
                new int[] { A(43.5),  B(43.5),   3,   14,    18,    22,    24,    30},    // 43
                new int[] { A(44.5),  B(44.5),   3,   14,    18,    22,    24,    31},    // 44  right
                new int[] { A(45.5),  B(45.5),   3,   13,    18,    21,    24,    31},    // 45  right
                new int[] { A(46.5),  B(46.5),   3,   13,    18,    21,    24,    31},    // 46
                new int[] { A(47.5),  B(47.5),   3,   13,    18,    21,    24,    31},    // 47
                new int[] { A(48.5),  B(48.5),   2,   13,    18,    21,    24,    31},    // 48
                new int[] { A(49.5),  B(49.5),   2,   13,    18,    21,    24,    31},    // 49
                new int[] { A(50.5),  B(50.5),   2,   13,    18,    21,    24,    31},    // 50
                new int[] { A(51.5),  B(51.5),   2,   13,    18,    21,    24,    31},    // 51
                new int[] { A(52.5),  B(52.5),   2,   13,    18,    21,    24,    32},    // 52
                new int[] { A(53.5),  B(53.5),   1,   13,    17,    21,    24,    32},    // 53
                new int[] { A(54.5),  B(54.5),   1,   13,    17,    21,    24,    32},    // 54
                new int[] { A(55.5),  B(55.5),   1,   13,    17,    21,    24,    32},    // 55
                new int[] { A(56.5),  B(56.5),   1,   13,    17,    21,    24,    32},    // 56
                new int[] { A(57.5),  B(57.5),   1,   13,    17,    21,    24,    32},    // 57
                new int[] { A(58.5),  B(58.5),  12,   13,    17,    21,    24,    32},    // 58
                new int[] { A(59.5),  B(59.5),  12,   13,    17,    21,    24,    32},    // 59  top circle
            };
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
    internal class TransparentOverlay : Form
    {
        private string displayText;

        public TransparentOverlay(string text)
        {
            displayText = text;

            // Remove window border and make it transparent
            FormBorderStyle = FormBorderStyle.None;
            StartPosition = FormStartPosition.CenterScreen;
            TopMost = true; // Always stay on top
            Width = 800;
            Height = 200;
            BackColor = Color.Black; // Transparency key requires this
            TransparencyKey = Color.Black; // Make the form background transparent

            // Enable click-through (optional)
            int initialStyle = NativeMethods.GetWindowLong(Handle, NativeMethods.GWL_EXSTYLE);
            NativeMethods.SetWindowLong(Handle, NativeMethods.GWL_EXSTYLE, initialStyle | NativeMethods.WS_EX_LAYERED | NativeMethods.WS_EX_TOPMOST);

            // Timer to close after 5 seconds
            System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer
            {
                Interval = 5000 // 5 seconds
            };
            timer.Tick += (s, e) =>
            {
                timer.Stop();
                Close();
            };
            timer.Start();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            // Set rectangle size and position
            int padding = 20;
            SizeF textSize;
            using (Font font = new Font("Arial", 24, FontStyle.Bold))
            {
                textSize = e.Graphics.MeasureString(displayText, font);
            }

            RectangleF rect = new RectangleF(
                (Width - textSize.Width) / 2 - padding,  // Horizontal center with padding
                (Height - textSize.Height) / 2 - padding, // Vertical center with padding
                textSize.Width + 2 * padding,            // Total width with padding
                textSize.Height + 2 * padding            // Total height with padding
            );

            // Draw a semi-transparent black rectangle as the background
            using (Brush backgroundBrush = new SolidBrush(Color.FromArgb(127, 0, 0, 0))) // RGBA(0, 0, 0, 127)
            {
                e.Graphics.FillRectangle(backgroundBrush, rect);
            }

            // Draw white text on top
            using (Font font = new Font("Arial", 24, FontStyle.Bold))
            using (Brush textBrush = new SolidBrush(Color.White))
            {
                StringFormat stringFormat = new StringFormat
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                };

                // Center the text in the rectangle
                e.Graphics.DrawString(displayText, font, textBrush, rect, stringFormat);
            }
        }


    }
    internal static class NativeMethods
    {
        public const int GWL_EXSTYLE = -20;
        public const int WS_EX_LAYERED = 0x80000;
        public const int WS_EX_TOPMOST = 0x00000008;

        [DllImport("user32.dll")]
        public static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        public static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
    }

}  // end of namespace