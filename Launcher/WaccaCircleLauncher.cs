using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using static System.Net.Mime.MediaTypeNames;

namespace SpinWheelApp
{
    public partial class MainWindow : Window
    {
        private const double Radius = 200;
        private const int ImageCount = 5;
        private double currentAngle = 0; // Current rotation angle

        public MainWindow()
        {
            InitializeComponent();
            DrawWheel();
        }

        private void DrawWheel()
        {
            for (int i = 0; i < ImageCount; i++)
            {
                // Calculate angle for each image
                double angle = Math.PI * 2 * i / ImageCount + currentAngle;
                double x = Radius * Math.Cos(angle);
                double y = Radius * Math.Sin(angle);

                // Create and position the image
                var img = new Image
                {
                    Source = new BitmapImage(new Uri("C:\\Hatsune-Miku\\ico\\TransparentWacca.ico")),
                    Width = 100,
                    Height = 100
                };
                Canvas.SetLeft(img, x + this.Width / 2 - img.Width / 2);
                Canvas.SetTop(img, y + this.Height / 2 - img.Height / 2);
                myCanvas.Children.Add(img); // Add to canvas
            }
        }

        private void RotateWheel(double angleDelta)
        {
            currentAngle += angleDelta;
            myCanvas.Children.Clear();
            DrawWheel();
        }
    }
}
