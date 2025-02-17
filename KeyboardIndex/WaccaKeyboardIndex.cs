using System.Runtime.InteropServices;
using System;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace WaccaKeyboardIndex
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

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            // Launch the overlay window
            Application.Run(new TransparentOverlay(args));
        }
    }
    internal class TransparentOverlay : Form
    {
        private string[] displayText;

        public TransparentOverlay(string[] text)
        {
            displayText = text;

            // Remove window border and make it transparent
            FormBorderStyle = FormBorderStyle.None;
            StartPosition = FormStartPosition.CenterScreen;
            TopMost = true;
            Width = 1100;
            Height = 1100;
            BackColor = System.Drawing.Color.Blue;
            TransparencyKey = System.Drawing.Color.Blue;

            // Enable click-through (optional)
            int initialStyle = NativeMethods.GetWindowLong(Handle, NativeMethods.GWL_EXSTYLE);
            NativeMethods.SetWindowLong(Handle, NativeMethods.GWL_EXSTYLE, initialStyle | NativeMethods.WS_EX_LAYERED | NativeMethods.WS_EX_TOPMOST);

            // Create the circle of labels
            CreateCircleOfLabels();
        }

        private void CreateCircleOfLabels()
        {
            int numLabels = 12;
            int radius = 400;
            Point center = new Point(Width / 2, Height / 2);

            for (int i = 0; i < numLabels; i++)
            {
                double angle = (2 * Math.PI / numLabels) * i;
                int x = (int)(center.X + radius * Math.Cos(angle)) - 30;
                int y = (int)(center.Y + radius * Math.Sin(angle)) - 15;

                Label lbl = new Label
                {
                    Text = (i < displayText.Length) ? displayText[i] : $"Label {i + 1}",
                    AutoSize = true,
                    Location = new Point(x, y),
                    ForeColor = Color.White,
                    BackColor = Color.Black,
                    Font = new Font("Arial", 24, FontStyle.Bold)
                };

                Controls.Add(lbl);
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