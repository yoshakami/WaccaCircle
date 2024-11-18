using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace SpinWheelApp
{
    public class Program
    {
        [STAThread]
        public static void Main()
        {
            var app = new Application();
            var mainWindow = new MainWindow();
            app.Run(mainWindow);
        }
    }
    public partial class MainWindow : Window
    {
        private const double Radius = 3000; // Larger radius for better positioning
        private const int ImageCount = 80;
        private double currentAngle = 0; // Current rotation angle in radians
        private MediaElement videoPlayer;
        private Canvas myCanvas;
        public MainWindow()
        {
            InitializeComponent();
            PlayVideo();
            // Start video playback when the window is loaded
            this.Loaded += (s, e) =>
            {
                videoPlayer.Play();
                DrawWheel();
            };
        }
        static int screenWidth = (int)SystemParameters.PrimaryScreenWidth; // Full screen width
        static int screenHeight = (int)SystemParameters.PrimaryScreenHeight; // Full screen height
        static int centerX = 520;  // Center point for rotation in the canvas
        static int centerY = 3960;  // Center point for rotation in the canvas

        static int imgRadius = 256;
        private void PlayVideo()
        {
            // Set up the window
            this.Width = screenWidth;
            this.Height = screenHeight;
            this.WindowStyle = WindowStyle.None; // No borders
            this.ResizeMode = ResizeMode.NoResize; // Fixed size
            this.Topmost = true; // Always on top
            this.Background = Brushes.Black; // Background for video display

            // Create a Grid to layer elements
            var mainGrid = new Grid();
            this.Content = mainGrid;

            // Create and configure the MediaElement
            videoPlayer = new MediaElement
            {
                Width = 1080,
                Height = 1920,
                LoadedBehavior = MediaState.Manual, // Control playback manually
                UnloadedBehavior = MediaState.Close, // Release resources when not in use
                Stretch = Stretch.Fill, // Scale to fill the screen
                Source = new Uri("C:\\Users\\yoshi\\Downloads\\2024-11-18 09-02-11.mkv"), // Update with your video path
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };

            mainGrid.Children.Add(videoPlayer);

            // Add Canvas for images
            myCanvas = new Canvas
            {
                Width = 1080,
                Height = 1920,
                Background = Brushes.Transparent // Ensure transparency
            };
            mainGrid.Children.Add(myCanvas);
        }
        private void DrawWheel()
        {
            myCanvas.Children.Clear(); // Clear existing images
            for (int i = 0; i < ImageCount; i++)
            {
                double imgSize = (i == 60) ? imgRadius : imgRadius / 2; // Larger size for child 30, smaller for others
                // Calculate position based on angle
                double angle = Math.PI * 2 * i / ImageCount + currentAngle; // Position angle
                double x = centerX + Radius * Math.Cos(angle) - (imgSize / 2); // Adjust for image size
                double y = centerY + Radius * Math.Sin(angle) - (imgSize / 2);

                // Create the image
                var img = new Image
                {
                    Source = new BitmapImage(new Uri("C:\\Hatsune-Miku\\ico\\TransparentWacca.ico")),
                    Width = imgSize,
                    Height = imgSize
                };

                // Apply a transform group: position the image and keep it upright
                var transformGroup = new TransformGroup();
                transformGroup.Children.Add(new RotateTransform(-currentAngle * (180 / Math.PI))); // Counteract wheel rotation
                img.RenderTransform = transformGroup;
                img.RenderTransformOrigin = new Point(0.5, 0.5); // Center of the image for rotation

                Canvas.SetLeft(img, x);
                Canvas.SetTop(img, y);
                myCanvas.Children.Add(img);
            }
        }

        static bool cannotcall = false;

        public void RotateWheel(double angleDelta)
        {
            if (cannotcall) return;
            cannotcall = true;

            // Create or update the RotateTransform on the Canvas (myCanvas)
            var canvasRotateTransform = myCanvas.RenderTransform as RotateTransform;
            if (canvasRotateTransform == null)
            {
                canvasRotateTransform = new RotateTransform(0);
                myCanvas.RenderTransform = canvasRotateTransform;

                // Set the RenderTransformOrigin to the point (centerX, centerY) in the canvas' coordinate space
                myCanvas.RenderTransformOrigin = new Point(centerX / myCanvas.ActualWidth, centerY / myCanvas.ActualHeight);
            }

            // Animate the rotation of the Canvas itself
            var canvasAnimation = new DoubleAnimation
            {
                From = canvasRotateTransform.Angle, // Current angle
                To = canvasRotateTransform.Angle + angleDelta * (180 / Math.PI), // Add angleDelta (in radians to degrees)
                Duration = TimeSpan.FromSeconds(0.3), // Smooth 0.3-second animation
                EasingFunction = new SineEase() // Smooth easing
            };

            // Apply the animation to the RotateTransform of the Canvas
            canvasRotateTransform.BeginAnimation(RotateTransform.AngleProperty, canvasAnimation);

            // Now, counter-rotate each child element to keep them fixed
            foreach (UIElement child in myCanvas.Children)
            {
                var childRotateTransform = child.RenderTransform as RotateTransform;
                if (childRotateTransform == null)
                {
                    childRotateTransform = new RotateTransform(0);
                    child.RenderTransform = childRotateTransform;
                    child.RenderTransformOrigin = new Point(0.5, 0.5); // Rotate around the center of each image
                }

                // Animate the counter-rotation of the child to negate the canvas rotation
                var childAnimation = new DoubleAnimation
                {
                    From = childRotateTransform.Angle,
                    To = childRotateTransform.Angle - angleDelta * (180 / Math.PI), // Counter-rotation to cancel canvas rotation
                    Duration = TimeSpan.FromSeconds(0.3), // Smooth 0.5-second animation
                    EasingFunction = new SineEase() // Smooth easing
                };

                // Apply the counter-rotation animation to each child
                childRotateTransform.BeginAnimation(RotateTransform.AngleProperty, childAnimation);
            }

            cannotcall = false;
        }



        protected override void OnKeyDown(System.Windows.Input.KeyEventArgs e)
        {
            // Example: Rotate left and right using arrow keys
            if (e.Key == System.Windows.Input.Key.Left)
                RotateWheel(-Math.PI / 45.0); // Rotate counterclockwise
            else if (e.Key == System.Windows.Input.Key.Right)
                RotateWheel(Math.PI / 45.0); // Rotate clockwise
                                              else if (e.Key == System.Windows.Input.Key.Up)
            {
                centerY += 100;  // Center point for rotation in the canvas
                DrawWheel();
            }
            else if (e.Key == System.Windows.Input.Key.Down)
            {
                centerY -= 100;  // Center point for rotation in the canvas
                DrawWheel();
            }
            else if (e.Key == System.Windows.Input.Key.RightShift)
                DrawWheel();
            // Optional: Add keyboard controls for the video
            if (e.Key == System.Windows.Input.Key.Space) // Pause/Play with Space
            {
                if (videoPlayer.CanPause)
                    videoPlayer.Pause();
                else
                    videoPlayer.Play();
            }
            else if (e.Key == System.Windows.Input.Key.Escape) // Close the window with Escape
            {
                this.Close();
            }
        }
    }
}
