using System;
using SharpDX;
using SharpDX.DirectInput;
using System.Threading;
using System.Diagnostics;
using System.Windows.Forms;

class JoystickPoller
{
    private DirectInput directInput;
    private Joystick joystick;
    private JoystickState state;

    public JoystickPoller()
    {
        // Initialize DirectInput
        directInput = new DirectInput();

        // Get the first joystick device (you can choose specific GUID if you want a particular joystick)
        var joystickGuid = Guid.Empty;
        foreach (var device in directInput.GetDevices(DeviceType.Gamepad, DeviceEnumerationFlags.AllDevices))
        {
            joystickGuid = device.InstanceGuid;
            break; // Use the first available joystick
        }

        // Create joystick instance
        if (joystickGuid != Guid.Empty)
        {
            joystick = new Joystick(directInput, joystickGuid);
            Console.WriteLine($"Found Joystick: {joystick.Information.ProductName}");
        }
        else
        {
            Console.WriteLine("No joystick found!");
        }
    }

    public void StartPolling()
    {
        if (joystick == null)
            return;

        // Set the cooperative level (non-exclusive, background mode)
        joystick.SetCooperativeLevel(IntPtr.Zero, CooperativeLevel.Background | CooperativeLevel.NonExclusive);

        // Acquire the joystick to start receiving input
        joystick.Acquire();
        Console.WriteLine("Joystick acquired.");

        // Initialize JoystickState
        state = new JoystickState();

        int stopwatch = 0;

        // Polling loop
        while (true)
        {
            // Poll joystick input
            joystick.Poll();

            // Get the current state
            state = joystick.GetCurrentState();

            // Check if button 1 (usually mapped to index 0) is pressed
            bool isButton1Pressed = state.Buttons[0]; // Buttons array index may vary based on joystick

            stopwatch += 500;
            if (isButton1Pressed)
            {
                // If the button is pressed, start or continue the stopwatch

                // Check if button has been held for 5 seconds
                if (stopwatch >= 5000)
                {
                    Console.WriteLine("Volume Up has been held for 5 seconds!\nSending Alt F4....");
                    SendKeys.Send("%{F4}");
                    stopwatch = 0; // Reset stopwatch to track the next hold
                }
            }
            else
            {
                // If the button is released, reset the stopwatch
                stopwatch = 0;
            }

            // Optional: Delay to reduce CPU usage
            System.Threading.Thread.Sleep(500);
        }
    }

    public static void Main()
    {
        var poller = new JoystickPoller();
        poller.StartPolling();
    }
}
