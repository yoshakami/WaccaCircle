using System.Windows.Forms;
using System.Drawing;
using System.Threading;

class Program
{
    static void Main()
    {
        // Define start and target positions
        Point startPos = Cursor.Position;  // Starting position (current mouse position)
        Point targetPos = new Point(1920, 1080);  // Target position
        int duration = 5000;  // Duration of the movement in milliseconds
        int steps = 1000;  // Number of steps for smooth movement

        MoveMouseSmoothly(startPos, targetPos, duration, steps);
    }

    static void MoveMouseSmoothly(Point start, Point target, int duration, int steps)
    {
        // Calculate the time per step (in milliseconds)
        int sleepTime = duration / steps;

        // Calculate the incremental step size in X and Y directions
        double stepX = (target.X - start.X) / (double)steps;
        double stepY = (target.Y - start.Y) / (double)steps;

        // Move the cursor step by step
        for (int i = 0; i < steps; i++)
        {
            // Calculate the next position
            int newX = (int)(start.X + stepX * i);
            int newY = (int)(start.Y + stepY * i);

            // Set the new cursor position
            Cursor.Position = new Point(newX, newY);

            // Pause for a short duration between steps
            Thread.Sleep(sleepTime);
        }

        // Ensure the final position is exactly the target position
        Cursor.Position = target;
    }
}
