using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Linq;
using System.Reflection;

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
        private static MediaElement videoPlayer;
        private static MediaElement videoRight;
        private static MediaElement videoLeft;
        private Canvas myCanvas;
        static int current = 4;
        private const double AnimationDuration = 0.5; // Seconds
        private List<Image> wheelImages = new List<Image>();
        private List<string> wheelExe = new List<string>();
        private List<string> imageList = new List<string>();
        private List<string> exeList = new List<string>();
        private List<Point> positions = new List<Point>();
        private static readonly string gamesPath = Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "Games");
        private static readonly string execPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        private static bool change_wheel_images = true;
        public MainWindow()
        {
            InitializeComponent();

            // Set the window position to (0, 0)
            this.WindowStartupLocation = WindowStartupLocation.Manual;
            this.Left = 0;
            this.Top = 0;

            PlayVideo();
            // Start video playback when the window is loaded
            this.Loaded += (s, e) =>
            {
                videoPlayer.Play();
                InitializeWheel();
            };
        }
        static int screenWidth = 1080; // Full screen width
        static int screenHeight = 1600; // Full screen height
        static int centerX = 540;  // Center point for rotation in the canvas
        static int centerY = 3960;  // Center point for rotation in the canvas
        static Image overlay;

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
                Width = screenWidth,
                Height = screenHeight,
                LoadedBehavior = MediaState.Manual, // Control playback manually
                UnloadedBehavior = MediaState.Close, // Release resources when not in use
                Stretch = Stretch.Uniform, // Scale to fill the screen
                Source = new Uri(Path.Combine(execPath, "background.mp4")), // Update with your video path
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

            videoLeft = new MediaElement
            {
                Width = screenWidth,
                Height = screenHeight,
                LoadedBehavior = MediaState.Manual, // Control playback manually
                UnloadedBehavior = MediaState.Close, // Release resources when not in use
                Stretch = Stretch.Uniform, // Scale to fill the screen
                Source = new Uri(Path.Combine(execPath, "left.mp4")), // Update with your video path
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                IsHitTestVisible = false, // Allow clicks to pass through
            };
            mainGrid.Children.Add(videoLeft);
            videoLeft.MediaEnded += (s, f) =>
            {
                videoLeft.Visibility = Visibility.Hidden;
            };
            videoRight = new MediaElement
            {
                Width = screenWidth,
                Height = screenHeight,
                LoadedBehavior = MediaState.Manual, // Control playback manually
                UnloadedBehavior = MediaState.Close, // Release resources when not in use
                Stretch = Stretch.Uniform, // Scale to fill the screen
                Source = new Uri(Path.Combine(execPath, "right.mp4")), // Update with your video path
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                IsHitTestVisible = false, // Allow clicks to pass through
            };
            videoRight.MediaEnded += (s, f) =>
            {
                videoRight.Visibility = Visibility.Hidden;
            };
            mainGrid.Children.Add(videoRight);

            // Add Canvas for images
            myCanvas = new Canvas
            {
                Width = screenWidth,
                Height = screenHeight,
                Background = Brushes.Transparent // Ensure transparency
            };
            mainGrid.Children.Add(myCanvas);
        }
        // native .net library supported images
        static readonly string[] imageExtensions = { ".bmp", ".png", ".jpg", ".jpeg", ".gif", ".ico", ".tif", ".tiff" };
        static readonly string[] gameExtensions = { ".bat", ".exe", ".vbs", ".ahk", ".lnk" };
        private void InitializeWheel()
        {
            int imgRadius = 192;

            myCanvas.Children.Clear();
            wheelImages.Clear();
            positions.Clear();
            exeList.Clear();
            imageList.Clear();
            wheelExe.Clear();

            // Define positions for the images
            positions.Add(new Point(-3000, 3000 - 160)); // Outside Left 2
            positions.Add(new Point(-200, 900 - 160)); // Outside Left 1
            positions.Add(new Point(16, 868 - 160)); // Left
            positions.Add(new Point(278 - 96, 835 - 160)); // Middle-left
            positions.Add(new Point(540 - 128, 962 - 128 - 160)); // Center
            positions.Add(new Point(704, 835 - 160)); // Middle-right
            positions.Add(new Point(868, 868 - 160)); // Right
            positions.Add(new Point(1080, 900 - 160)); // Outside Right 1
            positions.Add(new Point(3000, 3000 - 160)); // Outside Right 2
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
                    Source = null, //new BitmapImage(new Uri("C:\\Hatsune-Miku\\ico\\TransparentWacca.ico")),
                    Width = imgRadius,
                    Height = imgRadius
                };

                Canvas.SetLeft(img, pos.X);
                Canvas.SetTop(img, pos.Y);
                if (i == 6)
                {
                    Panel.SetZIndex(img, 50);  // higher value than images
                }
                myCanvas.Children.Add(img);
                wheelImages.Add(img);
            }
            overlay = new Image
            {
                Source = new BitmapImage(new Uri(Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "overlay.png"))),
                Width = 746,
                Height = 319
            };

            Canvas.SetLeft(overlay, 166);
            Canvas.SetTop(overlay, 632);
            myCanvas.Children.Add(overlay);
            // Bring the image to the front by setting a high ZIndex
            Panel.SetZIndex(overlay, 52);  // higher value than images
            int j = 0;
            if (!System.IO.Directory.Exists(gamesPath))
            {
                Console.WriteLine($"no file in {gamesPath}");
                System.IO.Directory.CreateDirectory(gamesPath);
            }
            foreach (string f in System.IO.Directory.EnumerateFiles(gamesPath))
            {
                if (imageExtensions.Any(ext => f.EndsWith(ext, StringComparison.OrdinalIgnoreCase)))
                {
                    string fileWithoutExtension = Path.Combine(Path.GetDirectoryName(f), Path.GetFileNameWithoutExtension(f));
                    string executableFile = gameExtensions
                                            .Select(ext => fileWithoutExtension + ext)
                                            .FirstOrDefault(File.Exists);

                    if (executableFile != null)
                    {
                        Console.WriteLine($"Launching: {executableFile}");
                        //LaunchFile(executableFile);
                        imageList.Add(f);
                        exeList.Add(executableFile);
                        if (j < wheelImages.Count)
                        {
                            wheelImages[j].Source = new BitmapImage(new Uri(f));
                            wheelExe.Add(executableFile);
                        }
                        j++;
                    }
                }
            }
            if (j < wheelImages.Count && j > 0)
            {
                change_wheel_images = false; // fill the wheel
                while (j < wheelImages.Count)
                {
                    for (int k = 0; k < imageList.Count && j < wheelImages.Count; k++)
                    {
                        wheelImages[j].Source = new BitmapImage(new Uri(imageList[k]));
                        wheelExe.Add(wheelExe[k]);
                        j++;
                    }
                }
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
            // Rotate images right (-1) or left (+1)
            if (direction == -1)
            {
                //Panel.SetZIndex(overlay, 1);
                videoRight.Position = TimeSpan.FromTicks(-10000000);    // reset position with a weird number because it's microsoft
                videoRight.Visibility = Visibility.Visible;
                videoRight.Play();
            }
            else if (direction == 1)
            {
                videoLeft.Visibility = Visibility.Visible;
                videoLeft.Play();
            }
            else
            {
                return;
            }

            // Update positions circularly
            var newPositions = new List<Point>();
            for (int i = 0; i < positions.Count; i++)
            {
                int newIndex = (i - direction + positions.Count) % positions.Count;
                newPositions.Add(positions[newIndex]);
            }
            Image last_1 = null;
            Image last_2 = null;
            // Animate each image to its new position and size
            for (int i = 0; i < wheelImages.Count; i++)
            {
                var img = wheelImages[i];
                var oldPos = positions[i];
                var newPos = newPositions[i];

                // Animate position
                AnimateImagePosition(img, oldPos, newPos);

                // Animate size (growth for center, shrink for others)
                int targetSize = (i == current) ? 256 : 192; // Center image grows to 256, others shrink to 192
                if (i == current - 1)
                {
                    last_1 = img;
                }
                else if (i == current + 1)
                {
                    last_2 = img;
                }
                else
                {
                    AnimateImageSize(img, targetSize, i);
                }
            }
            if (last_1 == null)
            {
                last_1 = wheelImages[8];
            }
            if (last_2 == null)
            {
                last_2 = wheelImages[0];
            }
            AnimateImageSize(last_1, 192, 50);
            AnimateImageSize(last_2, 192, 51);

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

        private void AnimateImageSize(Image image, int targetSize, int zIndex)
        {
            // Bring the image to the front by setting a high ZIndex
            Panel.SetZIndex(image, zIndex);

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
