using System;
using System.Windows.Forms;
using System.Drawing;

namespace WaccaBlack
{
    internal class Class
    {
        [STAThread]
        public static void Main(string[] args)
        {
            // Validate command-line arguments
            if (args.Length < 4)
            {
                Console.WriteLine("usage:\nWaccaBlack <x pos> <y pos> <width> <height>");
                return;
            }

            //string text = string.Join(" ", args); // Combine all arguments into a single string

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            // Launch the overlay window
            Application.Run(new BlackOverlay(args));
        }
    }

    internal class BlackOverlay : Form
    {
        private string displayText;

        public BlackOverlay(string[] args)
        {
            Console.WriteLine("Launching overlay at position ({0}, {1}) with size ({2}x{3})",
                  args[0], args[1], args[2], args[3]);

            // Parse the position from the arguments
            int x = int.Parse(args[0]);
            int y = int.Parse(args[1]);

            // Set the location of the form
            StartPosition = FormStartPosition.Manual; // Required to allow manual positioning
            Location = new Point(x, y);

            // Remove window border
            FormBorderStyle = FormBorderStyle.None;

            TopMost = true; // Always stay on top
            this.TopMost = true;
            Width = int.Parse(args[2]);
            Height = int.Parse(args[3]);
            BackColor = System.Drawing.Color.Black; // Transparency key requires this
        }
    }
}