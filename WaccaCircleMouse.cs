using System;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

class Program
{
    // Import SendInput from user32.dll
    [DllImport("user32.dll", SetLastError = true)]
    public static extern uint SendInput(uint nInputs, ref INPUT pInputs, int cbSize);

    // Constants for input types
    public const int INPUT_MOUSE = 0;
    public const uint MOUSEEVENTF_LEFTDOWN = 0x0002;
    public const uint MOUSEEVENTF_LEFTUP = 0x0004;
    public const uint MOUSEEVENTF_MOVE = 0x0001;

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

    [STAThread] // Required for SendKeys
    static void Main()
    {
        // Define start and end positions for the drag
        Point startPos = Cursor.Position;
        Point endPos = new Point(startPos.X + 200, startPos.Y + 100); // Move by 200, 100

        // Perform a drag-click
        DragClick(startPos, endPos);

        // Give the user a moment to focus on the target application
        Console.WriteLine("You have 5 seconds to focus on the target application...");
        System.Threading.Thread.Sleep(5000);

        // Send the string "Hello, World!" to the active application
        SendKeys.SendWait("Hello, World!");

        // Optionally, you can send an Enter key after the message
        SendKeys.SendWait("{ENTER}");
    }


public static void DragClick(Point start, Point end)
    {
        // Move the cursor to the starting position
        Cursor.Position = start;

        // Simulate mouse down (left button press)
        INPUT mouseInput = new INPUT();
        mouseInput.type = INPUT_MOUSE;
        mouseInput.mi.dwFlags = MOUSEEVENTF_LEFTDOWN;
        SendInput(1, ref mouseInput, Marshal.SizeOf(mouseInput));

        // Simulate mouse movement
        MoveMouseSmoothly(start, end, 500, 50);  // Move smoothly over 500 ms with 50 steps

        // Simulate mouse up (left button release)
        mouseInput.mi.dwFlags = MOUSEEVENTF_LEFTUP;
        SendInput(1, ref mouseInput, Marshal.SizeOf(mouseInput));
    }

    // Reuse the smooth movement code
    public static void MoveMouseSmoothly(Point start, Point target, int duration, int steps)
    {
        int sleepTime = duration / steps;
        double stepX = (target.X - start.X) / (double)steps;
        double stepY = (target.Y - start.Y) / (double)steps;

        for (int i = 0; i < steps; i++)
        {
            int newX = (int)(start.X + stepX * i);
            int newY = (int)(start.Y + stepY * i);
            Cursor.Position = new Point(newX, newY);
            Thread.Sleep(sleepTime);
        }

        Cursor.Position = target;
    }
}
