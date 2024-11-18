using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
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
        static int current = 4;
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
            int imgRadius = 192;

            myCanvas.Children.Clear();
            wheelImages.Clear();
            positions.Clear();

            // Define positions for the images
            positions.Add(new Point(-3000, 3000)); // Outside Left 2
            positions.Add(new Point(-200, 900)); // Outside Left 1
            positions.Add(new Point(16, 868)); // Left
            positions.Add(new Point(278-96, 835)); // Middle-left
            positions.Add(new Point(540 - 128, 960 - 128)); // Center
            positions.Add(new Point(704, 835)); // Middle-right
            positions.Add(new Point(868, 868)); // Right
            positions.Add(new Point(1080, 900)); // Outside Right 1
            positions.Add(new Point(3000, 3000)); // Outside Right 2
            /*
            AddImage(16, 868, 192); // left
            AddImage(869, 868, 192);  // right
            AddImage(540 - 128, 960 - 128, 256);  // middle
            AddImage(278 - 96, 835, 192);
            AddImage(704, 835, 192);*/

            // Create and add images to the canvas
            int i = 0;
            foreach (var pos in positions)
            {
                i++;
                if (i == 5)
                    imgRadius = 256;
                else
                    imgRadius = 192;
                var img = new Image
                {
                    Source = new BitmapImage(new Uri("C:\\Hatsune-Miku\\ico\\TransparentWacca.ico")),
                    Width = imgRadius,
                    Height = imgRadius
                };

                Canvas.SetLeft(img, pos.X);
                Canvas.SetTop(img, pos.Y);
                myCanvas.Children.Add(img);
                wheelImages.Add(img);
            }
        }

        static bool prevent_execution = false;
        private void RotateWheel(int direction)
        {
            if (prevent_execution)
                return;
            prevent_execution = true;
            current += direction;
                if (current == wheelImages.Count)
            {
                current = 0;
            }
            if (current < 0)
            {
                current = wheelImages.Count - 1;
            }
            // Rotate images left (-1) or right (+1)
            if (direction != -1 && direction != 1) return;

            // Update positions circularly
            var newPositions = new List<Point>(positions.Count);
            for (int i = 0; i < positions.Count; i++)
            {
                int newIndex = (i - direction + positions.Count) % positions.Count;
                newPositions.Add(positions[newIndex]);
            }

            // Animate each image to its new position and size
            for (int i = 0; i < wheelImages.Count; i++)
            {
                var img = wheelImages[i];
                var oldPos = positions[i];
                var newPos = newPositions[i];

                // Animate position
                AnimateImagePosition(img, oldPos, newPos);

                // Animate size (growth for center, shrink for others)
                int targetSize = (newPos == newPositions[current]) ? 256 : 192; // Center image grows to 256, others shrink to 192
                AnimateImageSize(img, targetSize);
            }

            // Update the positions list
            positions = newPositions;
            Task.Delay(500).ContinueWith(t => endthis());
        }
        public void endthis()
        {
            prevent_execution = false;
        }
        private void AnimateImagePosition(UIElement image, Point oldPos, Point newPos)
        {
            // Create animations for X and Y positions
            var leftAnimation = new DoubleAnimation
            {
                From = oldPos.X,
                To = newPos.X,
                Duration = TimeSpan.FromSeconds(0.5),
                EasingFunction = new SineEase() // Smooth easing
            };

            var topAnimation = new DoubleAnimation
            {
                From = oldPos.Y,
                To = newPos.Y,
                Duration = TimeSpan.FromSeconds(0.5),
                EasingFunction = new SineEase() // Smooth easing
            };

            // Apply animations
            image.BeginAnimation(Canvas.LeftProperty, leftAnimation);
            image.BeginAnimation(Canvas.TopProperty, topAnimation);
        }

        private void AnimateImageSize(Image image, int targetSize)
        {
            // Animate Width
            var widthAnimation = new DoubleAnimation
            {
                From = image.Width,
                To = targetSize,
                Duration = TimeSpan.FromSeconds(0.5),
                EasingFunction = new SineEase() // Smooth easing
            };

            // Animate Height
            var heightAnimation = new DoubleAnimation
            {
                From = image.Height,
                To = targetSize,
                Duration = TimeSpan.FromSeconds(0.5),
                EasingFunction = new SineEase() // Smooth easing
            };

            // Apply animations
            image.BeginAnimation(FrameworkElement.WidthProperty, widthAnimation);
            image.BeginAnimation(FrameworkElement.HeightProperty, heightAnimation);
        }



        bool ispaused = false;
        protected override void OnKeyDown(System.Windows.Input.KeyEventArgs e)
        {
            // Example: Rotate left and right using arrow keys
            if (e.Key == System.Windows.Input.Key.Left)
            {
                RotateWheel(1); // Rotate counterclockwise
            }
            else if (e.Key == System.Windows.Input.Key.Right)
            {
                RotateWheel(-1); // Rotate clockwise
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
