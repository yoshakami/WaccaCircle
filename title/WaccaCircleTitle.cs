using System.Runtime.InteropServices;
using System;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace WaccaCircle
{
    internal class Class
    {
        [STAThread]
        public static void Main(string[] args)
        {
            // Validate command-line arguments
            if (args.Length < 1)
            {
                Console.WriteLine("Please provide the text to display as a command-line argument.");
                return;
            }

            string text = string.Join(" ", args); // Combine all arguments into a single string

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            // Launch the overlay window
            Application.Run(new TransparentOverlay(text));
        }
    }

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
            this.TopMost = true;
            Width = 1000;
            Height = 200;
            BackColor = System.Drawing.Color.Blue; // Transparency key requires this
            TransparencyKey = System.Drawing.Color.Blue; // Make the form background transparent

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

            // Define your rectangle with padding and size
            RectangleF rect = new RectangleF(
                (Width - textSize.Width) / 2 - padding, // Horizontal center with padding
                (Height - textSize.Height) / 2 - padding, // Vertical center with padding
                textSize.Width + 2 * padding,            // Total width with padding
                textSize.Height + 2 * padding            // Total height with padding
            );

            // Create a GraphicsPath to define the rounded rectangle
            using (GraphicsPath path = new GraphicsPath())
            {
                // Set the radius for the rounded corners
                float cornerRadius = 50f; // Adjust this value to control the roundness
                path.AddArc(rect.X, rect.Y, cornerRadius, cornerRadius, 180, 90); // Top-left corner
                path.AddArc(rect.Right - cornerRadius, rect.Y, cornerRadius, cornerRadius, 270, 90); // Top-right corner
                path.AddArc(rect.Right - cornerRadius, rect.Bottom - cornerRadius, cornerRadius, cornerRadius, 0, 90); // Bottom-right corner
                path.AddArc(rect.X, rect.Bottom - cornerRadius, cornerRadius, cornerRadius, 90, 90); // Bottom-left corner
                path.CloseFigure();

                // Draw the semi-transparent background with rounded corners
                using (Brush backgroundBrush = new SolidBrush(Color.FromArgb(255, 0, 0, 0))) // Semi-transparent black
                {
                    e.Graphics.FillPath(backgroundBrush, path);
                }
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
}