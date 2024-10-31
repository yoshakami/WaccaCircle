using System;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using static Program;

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
        Click(new Point(1079, 1919));
        Thread.Sleep(1000);
        int[] hours_x = { 540, 770, 938, 1000, 938, 770, 540, 310, 141, 80, 141, 309, 540, 700, 817, 860, 817, 700, 540, 380, 262, 220, 262, 379, 540, 697, 762, 697, 540, 382, 317, 382 };
        int[] hours_y = { 447, 508, 677, 907, 1137, 1305, 1367, 1305, 1137, 907, 677, 508, 587, 629, 747, 907, 1067, 1184, 1227, 1184, 1067, 907, 747, 629, 684, 749, 907, 1064, 1129, 1064, 907, 382 };
        int x;
        int y;
        x = 150;
        y = 650;
        int[] moved_x = { 365, 470, 575, 680 };
        int[] moved_y = { 50, 50, 50, 50 };
        Point startPos;
        Point endPos;
        int y_space = 149;
        int LAG_DELAY = 100;
        for (int k = 0; k < 4; k++)
        {
            Point startPos2 = new Point(x, y);
            Point endPos2 = new Point(moved_x[k], moved_y[k]);
            // Perform a drag-click
            DragClick(startPos2, endPos2);
            Thread.Sleep(LAG_DELAY);
            y += 150;
        }
        x = 50;
        y = 200;
        int prev_x = 0;
        int prev_y = 0;
        for (int j = 0; j < 2; j++)
        {
            for (int i = 1; i < 11; i++)
            {
                if (j * 11 + i == 16)
                {
                    for (int k = 0; k < 4; k++)
                    {
                        Point startPos3 = new Point(moved_x[k], moved_y[k]);
                        Point endPos3 = new Point(hours_x[k + 16], hours_y[k + 16]);
                        // Perform a drag-click
                        DragClick(startPos3, endPos3);
                        Thread.Sleep(LAG_DELAY);
                    }
                }
                if (j * 11 + i == 12)
                {
                    startPos = new Point(prev_x, prev_y);
                    endPos = new Point(hours_x[11], hours_y[11]);
                    // Perform a drag-click
                    DragClick(startPos, endPos);
                    Thread.Sleep(LAG_DELAY);
                    startPos = new Point(x, y);
                    endPos = new Point(hours_x[0], hours_y[0]);
                    // Perform a drag-click
                    DragClick(startPos, endPos);
                    Thread.Sleep(LAG_DELAY);
                    y += y_space;
                    continue;
                }
                if (j * 11 + i >= 16 && j * 11 + i <= 19)
                {
                    y += y_space;
                    continue;
                }
                Point startPos1 = new Point(x, y);
                Point endPos1 = new Point(hours_x[j * 11 + i], hours_y[j * 11 + i]);
                // Perform a drag-click
                DragClick(startPos1, endPos1);
                Thread.Sleep(LAG_DELAY);
                y += y_space;
            }
            prev_x = x;
            prev_y = y;
            x += 105;
            y = 50;
        }
        startPos = new Point(prev_x, prev_y);
        endPos = new Point(hours_x[22], hours_y[22]);
        // place circle
        DragClick(startPos, endPos);
        Thread.Sleep(LAG_DELAY);
        startPos = new Point(prev_x, prev_y + y_space);
        endPos = new Point(hours_x[23], hours_y[23]);
        // place circle
        DragClick(startPos, endPos);
        Thread.Sleep(LAG_DELAY);
        x = 260;
        y = 50 + y_space;
        startPos = new Point(260, 50);
        endPos = new Point(hours_x[12], hours_y[12]);
        // place circle
        DragClick(startPos, endPos);
        Thread.Sleep(LAG_DELAY);
        for (int i = 25; i < 33; i++)
        {
            startPos = new Point(x, y);
            endPos = new Point(hours_x[i], hours_y[i]);
            DragClick(startPos, endPos);
            y += y_space;
        }
        startPos = new Point(x, y);
        endPos = new Point(hours_x[24], hours_y[24]);
        // place circle
        DragClick(startPos, endPos);
        Thread.Sleep(LAG_DELAY);
        // deselect- click in the middle of nowhere
        endPos = new Point(hours_x[24], hours_y[24] + 200);
        Click(endPos);
        // Give the user a moment to focus on the target application
        //Console.WriteLine("You have 5 seconds to focus on the target application...");
        //*/
        // System.Threading.Thread.Sleep(5000);

        // Send the string "Hello, World!" to the active application
        //SendKeys.SendWait("Hello, World!");

        // Optionally, you can send an Enter key after the message
        //SendKeys.SendWait("%d"); // % represents the Alt key
    }


    public static void Click(Point start)
    {

        // Move the cursor to the starting position
        Cursor.Position = start;

        // Simulate mouse down (left button press)
        INPUT mouseInput = new INPUT();
        mouseInput.type = INPUT_MOUSE;
        mouseInput.mi.dwFlags = MOUSEEVENTF_LEFTDOWN;
        SendInput(1, ref mouseInput, Marshal.SizeOf(mouseInput));

        // Simulate mouse movement
        Thread.Sleep(100);

        // Simulate mouse up (left button release)
        mouseInput.mi.dwFlags = MOUSEEVENTF_LEFTUP;
        SendInput(1, ref mouseInput, Marshal.SizeOf(mouseInput));
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
        MoveMouseSmoothly(start, end, 200, 30);  // Move smoothly over 200 ms with 30 steps

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
