using System;
using System.Collections.Generic;
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
        private List<string> image_list = new List<string>();
        private List<string> exe_list = new List<string>();
        private double currentAngle = 0; // Current rotation angle in radians
        private MediaElement videoPlayer;
        private Canvas myCanvas;
        static int current = 61;
        private const double AnimationDuration = 0.5; // Seconds
        private List<Image> wheelImages = new List<Image>();
        private List<Point> positions = new List<Point>();
        public MainWindow()
        {
            InitializeComponent();
            PlayVideo();
            // Start video playback when the window is loaded
            this.Loaded += (s, e) =>
            {
                videoPlayer.Play();
                InitializeWheel();
            };
        }
        static int screenWidth = (int)SystemParameters.PrimaryScreenWidth; // Full screen width
        static int screenHeight = (int)SystemParameters.PrimaryScreenHeight; // Full screen height
        static int centerX = 540;  // Center point for rotation in the canvas
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
                VerticalAlignment = VerticalAlignment.Center,
                IsHitTestVisible = false // Allow clicks to pass through
            };
            // Set video to loop
            videoPlayer.MediaEnded += (s, e) =>
            {
                videoPlayer.Position = TimeSpan.Zero; // Restart the video
                videoPlayer.Play();
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
        private void InitializeWheel()
        {
            myCanvas.Children.Clear();
            wheelImages.Clear();
            positions.Clear();

            // Define positions for the images
            positions.Add(new Point(16, 868)); // Left
            positions.Add(new Point(278, 835)); // Middle-left
            positions.Add(new Point(540 - 128, 960 - 128)); // Center
            positions.Add(new Point(704, 835)); // Middle-right
            positions.Add(new Point(869, 868)); // Right

            // Create and add images to the canvas
            foreach (var pos in positions)
            {
                var img = new Image
                {
                    Source = new BitmapImage(new Uri("C:\\Hatsune-Miku\\ico\\TransparentWacca.ico")),
                    Width = 192,
                    Height = 192
                };

                Canvas.SetLeft(img, pos.X);
                Canvas.SetTop(img, pos.Y);
                myCanvas.Children.Add(img);
                wheelImages.Add(img);
            }
        }

        static bool cannotcall = false;
        private void RotateWheel(int direction)
        {
            // Rotate images left (-1) or right (+1)
            if (direction != -1 && direction != 1) return;

            // Update positions circularly
            var newPositions = new List<Point>(positions.Count);
            for (int i = 0; i < positions.Count; i++)
            {
                int newIndex = (i - direction + positions.Count) % positions.Count;
                newPositions.Add(positions[newIndex]);
            }

            // Animate each image to its new position
            for (int i = 0; i < wheelImages.Count; i++)
            {
                var img = wheelImages[i];
                var oldPos = positions[i];
                var newPos = newPositions[i];

                // Animate position
                AnimateImagePosition(img, oldPos, newPos);
            }

            // Update the positions list
            positions = newPositions;
        }

        private void AnimateImagePosition(UIElement image, Point oldPos, Point newPos)
        {
            // Create animations for X and Y positions
            var leftAnimation = new DoubleAnimation
            {
                From = oldPos.X,
                To = newPos.X,
                Duration = TimeSpan.FromSeconds(AnimationDuration),
                EasingFunction = new SineEase() // Smooth easing
            };

            var topAnimation = new DoubleAnimation
            {
                From = oldPos.Y,
                To = newPos.Y,
                Duration = TimeSpan.FromSeconds(AnimationDuration),
                EasingFunction = new SineEase() // Smooth easing
            };

            // Apply animations
            image.BeginAnimation(Canvas.LeftProperty, leftAnimation);
            image.BeginAnimation(Canvas.TopProperty, topAnimation);
        }


        bool ispaused = false;
        protected override void OnKeyDown(System.Windows.Input.KeyEventArgs e)
        {
            // Example: Rotate left and right using arrow keys
            if (e.Key == System.Windows.Input.Key.Left)
            {
                current += 1;
                RotateWheel(-1); // Rotate counterclockwise
            }
            else if (e.Key == System.Windows.Input.Key.Right)
            {
                current -= 1;
                RotateWheel(1); // Rotate clockwise
            }
            else if (e.Key == System.Windows.Input.Key.Up)
            {
                centerY += 100;  // Center point for rotation in the canvas
                InitializeWheel();
            }
            else if (e.Key == System.Windows.Input.Key.Down)
            {
                centerY -= 100;  // Center point for rotation in the canvas
                InitializeWheel();
            }
            else if (e.Key == System.Windows.Input.Key.RightShift)
                InitializeWheel();
            // Optional: Add keyboard controls for the video
            if (e.Key == System.Windows.Input.Key.Space) // Pause/Play with Space
            {
                if (ispaused)
                {
                    videoPlayer.Play();
                    ispaused = false;
                }
                else
                {
                    videoPlayer.Pause();
                    ispaused = true;
                }
            }
            else if (e.Key == System.Windows.Input.Key.Escape) // Close the window with Escape
            {
                this.Close();
            }
        }
    }
}
