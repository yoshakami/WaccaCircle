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
using Newtonsoft.Json;
using System.Diagnostics;
using System.Media;

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
    public struct Color
    {
        public double H { get; set; } // Hue: 0-360 degrees
        public double S { get; set; } // Saturation: 0-1
        public double V { get; set; } // Value: 0-1

        public Color(double h, double s, double v)
        {
            H = h;
            S = s;
            V = v;
        }

        /// <summary>
        /// Converts the HSV color to an RGB color and returns the result as a tuple of 3 bytes.
        /// </summary>
        /// <returns>A tuple containing R, G, and B as bytes (0-255).</returns>
        /// <summary>
        /// Converts the HSV color to an RGB color and returns the result as a byte array.
        /// </summary>
        /// <returns>An array of 3 bytes representing R, G, and B (0-255).</returns>
        public byte[] ToRGB()
        {
            double r = 0, g = 0, b = 0;

            if (S == 0) // Achromatic (gray)
            {
                r = g = b = V;
            }
            else
            {
                double sector = H / 60.0; // Sector index (0-5)
                int sectorIndex = (int)Math.Floor(sector);
                double fractionalPart = sector - sectorIndex; // Fractional part of sector

                double p = V * (1 - S);
                double q = V * (1 - S * fractionalPart);
                double t = V * (1 - S * (1 - fractionalPart));

                switch (sectorIndex)
                {
                    case 0:
                        r = V; g = t; b = p;
                        break;
                    case 1:
                        r = q; g = V; b = p;
                        break;
                    case 2:
                        r = p; g = V; b = t;
                        break;
                    case 3:
                        r = p; g = q; b = V;
                        break;
                    case 4:
                        r = t; g = p; b = V;
                        break;
                    case 5:
                        r = V; g = p; b = q;
                        break;
                }
            }

            // Convert to 0-255 range and return as bytes
            return new byte[] { (byte)(r * 255), (byte)(g * 255), (byte)(b * 255) };
        }


        public override string ToString()
        {
            var rgbBytes = ToRGB();
            return $"HSV({H}, {S}, {V}) -> R: {rgbBytes[0]}, G: {rgbBytes[1]}, B: {rgbBytes[2]}";

        }
    }
    public class ParamWrittenInTheJson
    {
        public Color TitleColor { get; set; }
        public Color DescriptionColor { get; set; }
        public string FontFamily { get; set; }
        public List<string> Titles { get; set; }
        public List<string> Descriptions { get; set; }
    }
    public static class ParamStoredInRam
    {
        public static Color TitleColor = new Color(0, 0, 1);
        public static Color DescriptionColor = new Color(200, 0.8, 1);     // Blue 

        public static string FontFamily = "Arial";

        public static List<string> Titles = new List<string>();
        public static List<string> Descriptions = new List<string>();
    }

    public partial class MainWindow : Window
    {
        public static readonly string configName = "WaccaCircleLauncher.json";
        public static void SaveConfig()
        {
            ParamWrittenInTheJson data = new ParamWrittenInTheJson
            {
                TitleColor = ParamStoredInRam.TitleColor,
                DescriptionColor = ParamStoredInRam.DescriptionColor,
                FontFamily = ParamStoredInRam.FontFamily,
                Titles = ParamStoredInRam.Titles,
                Descriptions = ParamStoredInRam.Descriptions,
            };
            string json = JsonConvert.SerializeObject(data, Formatting.Indented);
            File.WriteAllText(configName, json);
        }
        public static void LoadConfig()
        {
            if (File.Exists(configName))
            {
                try
                {
                    string json = File.ReadAllText(configName);
                    var data = JsonConvert.DeserializeObject<ParamWrittenInTheJson>(json);
                    ParamStoredInRam.TitleColor = data.TitleColor;
                    ParamStoredInRam.DescriptionColor = data.DescriptionColor;
                    ParamStoredInRam.Titles = data.Titles;
                    ParamStoredInRam.Descriptions = data.Descriptions;
                    ParamStoredInRam.FontFamily = data.FontFamily;

                    Console.WriteLine($"Loaded {configName}!");
                }
                catch (Exception e)
                {
                    SaveConfig();
                }
            }
            else
            {
                Console.WriteLine($"{configName} not found!\nCreating config file...");
                SaveConfig();
            }
        }

        private const double Radius = 3000; // Larger radius for better positioning
        private double currentAngle = 0; // Current rotation angle in radians
        private static MediaElement videoPlayer;
        private Canvas myCanvas;
        static int current = 4;
        static int completeCurrent = 4;
        private const double AnimationDuration = 0.5; // Seconds
        private List<string> wheelTitles = new List<string>();
        private List<string> wheelDescriptions = new List<string>();
        private List<Image> wheelImages = new List<Image>();
        private List<string> wheelExe = new List<string>();
        private List<string> imageList = new List<string>();
        private List<string> exeList = new List<string>();
        private List<Point> positions = new List<Point>();
        private static readonly string gamesPath = Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "Games");
        private static readonly string execPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        // Play a WAV file
        SoundPlayer player = new SoundPlayer(Path.Combine(execPath, "rotate.wav"));
        private static bool change_wheel_images = true;
        public MainWindow()
        {
            InitializeComponent();

            // Set the window position to (0, 0)
            this.WindowStartupLocation = WindowStartupLocation.Manual;
            this.Left = 0;
            this.Top = 0;
            LoadConfig();
            PlayBGM();
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
        public static Image overlay;
        private void PlayBGM()
        {
            // Create a MediaElement programmatically
            var mediaElement = new System.Windows.Controls.MediaElement
            {
                LoadedBehavior = System.Windows.Controls.MediaState.Manual, // Don't auto-play
                UnloadedBehavior = System.Windows.Controls.MediaState.Manual, // Don't auto-stop
                Volume = 1,  // Adjust volume as needed
                IsMuted = false
            };

            // Set the source to your MP3 file
            if (File.Exists(Path.Combine(execPath, "bgm.flac")))
            {
                mediaElement.Source = new Uri(Path.Combine(execPath, "bgm.flac"));
            }
            else if (File.Exists(Path.Combine(execPath, "bgm.mp3")))
            {
                mediaElement.Source = new Uri(Path.Combine(execPath, "bgm.mp3"));
            }
            else if (File.Exists(Path.Combine(execPath, "bgm.wav")))
            {
                mediaElement.Source = new Uri(Path.Combine(execPath, "bgm.wav"));
            }
            else if (File.Exists(Path.Combine(execPath, "bgm.aac")))
            {
                mediaElement.Source = new Uri(Path.Combine(execPath, "bgm.aac"));
            }
            else if (File.Exists(Path.Combine(execPath, "bgm.ogg")))
            {
                mediaElement.Source = new Uri(Path.Combine(execPath, "bgm.ogg"));
            }
            else if (File.Exists(Path.Combine(execPath, "bgm.opus")))
            {
                mediaElement.Source = new Uri(Path.Combine(execPath, "bgm.opus"));
            }

            // Set MediaElement to loop indefinitely
            mediaElement.MediaEnded += (sender, e) =>
            {
                mediaElement.Position = TimeSpan.Zero; // Rewind the track to start
                mediaElement.Play(); // Restart the playback
            };

            // Play the media file
            mediaElement.Play();
        }
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
        TextBlock RightTitle;
        TextBlock RightDesc;
        TextBlock centerTitle;
        TextBlock centerDesc;
        TextBlock LeftTitle;
        TextBlock LeftDesc;
        private void InitializeWheel()
        {
            int imgRadius = 192;

            myCanvas.Children.Clear();
            wheelImages.Clear();
            positions.Clear();
            exeList.Clear();
            imageList.Clear();
            wheelExe.Clear();
            wheelDescriptions.Clear();
            wheelTitles.Clear();

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
                Source = new BitmapImage(new Uri(Path.Combine(System.IO.Path.GetDirectoryName
                (System.Reflection.Assembly.GetExecutingAssembly().Location), "overlay.png"))),
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
                        Console.WriteLine($"Adding: {executableFile}");
                        /////////********** THIS IS WHERE EACH GAME IS BEING ADDED IN RAM AS AN ENTRY ***********/////////////
                        //LaunchFile(executableFile);
                        imageList.Add(f);
                        exeList.Add(executableFile);

                        if (ParamStoredInRam.Titles.Count <= j) // since j starts at zero then increases here, it means it'll take entries from the json if they exist, or add them here
                        {
                            ParamStoredInRam.Titles.Add(f);
                            ParamStoredInRam.Descriptions.Add(f);
                        }
                        if (j < wheelImages.Count)  // max size is 9
                        {
                            wheelImages[j].Source = new BitmapImage(new Uri(f));
                            wheelExe.Add(executableFile);
                            wheelTitles.Add(ParamStoredInRam.Titles[j]);
                            wheelDescriptions.Add(ParamStoredInRam.Descriptions[j]);
                        }
                        j++;
                    }
                }
            }
            if (j <= wheelImages.Count && j > 0)  // if there's less than 9 games in the wheel of 9 elements
            {
                change_wheel_images = false; // fill the wheel
                while (j < wheelImages.Count)
                {
                    for (int k = 0; k < imageList.Count && j < wheelImages.Count; k++, j++)
                    {
                        if (ParamStoredInRam.Titles.Count <= j)
                        {
                            ParamStoredInRam.Titles.Add(ParamStoredInRam.Titles[k]);
                            ParamStoredInRam.Descriptions.Add(ParamStoredInRam.Descriptions[k]);
                        }
                        wheelImages[j].Source = new BitmapImage(new Uri(imageList[k]));
                        wheelExe.Add(wheelExe[k]);
                        wheelTitles.Add(wheelTitles[k]);
                        wheelDescriptions.Add(wheelDescriptions[k]);
                    }
                }
            }
            byte[] rgbTitle = ParamStoredInRam.TitleColor.ToRGB();
            byte[] rgbDesc = ParamStoredInRam.DescriptionColor.ToRGB();
            SolidColorBrush brushTitle = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, rgbTitle[0], rgbTitle[1], rgbTitle[2]));
            SolidColorBrush brushDesc = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, rgbDesc[0], rgbDesc[1], rgbDesc[2]));
            InitializeText(ref LeftTitle, ref brushTitle, ParamStoredInRam.Titles[3],                180,      899,     200,        20,          18);
            InitializeText(ref LeftDesc, ref brushDesc, ParamStoredInRam.Descriptions[3],            180,      923,     200,        20,          18);
            InitializeText(ref centerTitle, ref brushTitle, ParamStoredInRam.Titles[4],              365,     1060,     300,        30,          25);
            InitializeText(ref centerDesc, ref brushDesc, ParamStoredInRam.Descriptions[4],          365,     1096,     300,        30,          25);
            InitializeText(ref RightTitle, ref brushTitle, ParamStoredInRam.Titles[5],               704,      899,     200,        20,          18);
            InitializeText(ref RightDesc, ref brushDesc, ParamStoredInRam.Descriptions[5],           704,      923,     200,        20,          18);

            SaveConfig();
        }
        private void InitializeText(ref TextBlock text, ref SolidColorBrush brush, string value, int left, int top, int width, int height, byte fontsize)
        {
            // Create a TextBlock
            if (text == null)
            {
                text = new TextBlock
                {
                    Text = value, // Set your desired text
                    FontSize = fontsize, // ParamStoredInRam.FontSize,           // Change to desired font size
                    FontFamily = new FontFamily(ParamStoredInRam.FontFamily), // Set the font family
                    Foreground = brush,
                    MaxWidth = width,
                    Width = width,
                    Height = height,
                    MaxHeight = height,
                    TextAlignment = TextAlignment.Center,  // Center text horizontally
                    TextWrapping = TextWrapping.Wrap,      // Wrap text within the width
                    VerticalAlignment = VerticalAlignment.Center
                };
                // Position the TextBlock on the Canvas
                Canvas.SetLeft(text, left); // X-coordinate (adjust as needed)
                Canvas.SetTop(text, top); // Y-coordinate (adjust as needed)

                // Optional: Set ZIndex to layer the text
                Panel.SetZIndex(text, 200);

                // Add the TextBlock to the Canvas
                myCanvas.Children.Add(text);
            }
            else
            {
                text.Text = value;
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
            completeCurrent += direction;
            if (completeCurrent == imageList.Count)
            {
                completeCurrent = 0;
            }
            if (completeCurrent < 0)
            {
                completeCurrent = imageList.Count - 1;
            }
            // Rotate images right (-1) or left (+1)
            if (direction == -1)
            {
            }
            else if (direction == 1)
            {
            }
            else
            {
                return;
            }
            int left_id = -1;
            int right_id = -1;
            int offScreenLeftWheelId = -1;
            int offScreenRightWheelId = -1;
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
                    left_id = i;
                }
                else if (i == current + 1)
                {
                    last_2 = img;
                    right_id = i;
                }
                else
                {
                    AnimateImageSize(img, targetSize, i);
                }
            }
            if (last_1 == null)
            {
                last_1 = wheelImages[8];
                left_id = 8;
                offScreenLeftWheelId = 6;
                offScreenRightWheelId = 3;
            }
            else if (last_2 == null)
            {
                last_2 = wheelImages[0];
                right_id = 0;
                offScreenLeftWheelId = 5;
                offScreenRightWheelId = 2;
            }
            else
            {
                offScreenLeftWheelId = current < 3 ? current + 6 : current - 3;
                offScreenRightWheelId = current > 5 ? current - 6 : current + 3;
            }
            int completeCurrentLeft = completeCurrent < 3 ? completeCurrent + imageList.Count - 3 : completeCurrent - 3;
            int completeCurrentRight = completeCurrent < 3 ? completeCurrent + imageList.Count - 3 : completeCurrent - 3;

            wheelExe[offScreenLeftWheelId] = exeList[completeCurrentLeft];
            wheelImages[offScreenLeftWheelId].Source = new BitmapImage(new Uri(imageList[completeCurrentLeft]));
            wheelTitles[offScreenLeftWheelId] = ParamStoredInRam.Titles[completeCurrentLeft];
            wheelDescriptions[offScreenLeftWheelId] = ParamStoredInRam.Descriptions[completeCurrentLeft];

            wheelExe[offScreenRightWheelId] = exeList[completeCurrentRight];
            wheelImages[offScreenRightWheelId].Source = new BitmapImage(new Uri(imageList[completeCurrentRight]));
            wheelTitles[offScreenRightWheelId] = ParamStoredInRam.Titles[completeCurrentRight];
            wheelDescriptions[offScreenRightWheelId] = ParamStoredInRam.Descriptions[completeCurrentRight];

            AnimateImageSize(last_1, 192, 50);
            AnimateImageSize(last_2, 192, 51);

            byte[] rgbTitle = ParamStoredInRam.TitleColor.ToRGB();
            byte[] rgbDesc = ParamStoredInRam.DescriptionColor.ToRGB();
            SolidColorBrush brushTitle = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, rgbTitle[0], rgbTitle[1], rgbTitle[2]));
            SolidColorBrush brushDesc = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, rgbDesc[0], rgbDesc[1], rgbDesc[2]));
            InitializeText(ref LeftTitle, ref brushTitle, wheelTitles[left_id], 180, 899, 200, 20, 18);
            InitializeText(ref LeftDesc, ref brushDesc, wheelDescriptions[left_id], 180, 923, 200, 20, 18);
            InitializeText(ref centerTitle, ref brushTitle, wheelTitles[current], 365, 1060, 300, 30, 25);
            InitializeText(ref centerDesc, ref brushDesc, wheelDescriptions[current], 365, 1096, 300, 30, 25);
            InitializeText(ref RightTitle, ref brushTitle, wheelTitles[right_id], 704, 899, 200, 20, 18);
            InitializeText(ref RightDesc, ref brushDesc, wheelDescriptions[right_id], 704, 923, 200, 20, 18);
            // Update the positions list
            positions = newPositions;
            Task.Delay(100).ContinueWith(t =>
            {
                // Make sure to update UI elements on the UI thread
                Dispatcher.Invoke(() => endthis());
            });
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
                Duration = TimeSpan.FromSeconds(0.1),
                EasingFunction = new SineEase() // Smooth easing
            };

            var topAnimation = new DoubleAnimation
            {
                From = oldPos.Y,
                To = newPos.Y,
                Duration = TimeSpan.FromSeconds(0.1),
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
                Duration = TimeSpan.FromSeconds(0.1),
                EasingFunction = new SineEase() // Smooth easing
            };

            // Animate Height
            var heightAnimation = new DoubleAnimation
            {
                From = image.Height,
                To = targetSize,
                Duration = TimeSpan.FromSeconds(0.1),
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
                player.Play();
                RotateWheel(-1); // Rotate counterclockwise
            }
            else if (e.Key == System.Windows.Input.Key.Right)
            {
                player.Play();
                RotateWheel(1); // Rotate clockwise
            }
            else if (e.Key == System.Windows.Input.Key.Enter)
            {
                Process.Start(wheelExe[current]);
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
