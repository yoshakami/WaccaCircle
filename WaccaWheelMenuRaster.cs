using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SpinWheelApp
{
    /// <summary>
    /// Raster-sprite version of the Wacca speed-wheel sub-menu.
    ///
    /// It composites two PNG sprites that you place in a `sprites/` folder next to
    /// the executable:
    ///   sprites/core.png  - the animated tunnel core (square, alpha outside the rim)
    ///   sprites/ring.png  - the outer glitch ring + baked chrome (square annulus,
    ///                        transparent centre hole)
    /// On top of those it DRAWS the parts that must stay dynamic (so the same control
    /// still works as a reusable menu): the value lanes + their text, the yellow
    /// selected banner + value, the SET tab, the title/description. The core sprite is
    /// rotated continuously for the "spinning tunnel" effect.
    ///
    /// If the sprite files are missing it falls back to drawn placeholders, so a clone
    /// of the repo without the art still builds and runs.
    ///
    /// Same public API as the vector WaccaWheelMenu (Configure / Step / HitTestAngle /
    /// Show / Hide + Confirmed / Cancelled / DefaultRequested), so it is a drop-in swap.
    ///
    /// Geometry note: sprites were cut from a wheel of source-radius ~528 px centred on
    /// the wheel. Everything below is scaled into a 1000x1000 design space (Viewbox,
    /// Uniform) sharing WaccaBackground's centre. The radii (core 158, ring hole 341,
    /// lanes 175..333) keep the same proportions as the source so the drawn lanes land
    /// exactly in the gap between the core sprite and the ring sprite's hole.
    /// </summary>
    public class WaccaWheelMenuRaster : Viewbox
    {
        private const double DesignW = 1000, DesignH = 1000;
        private double _cx = 500, _cy = 490;

        private const double RingHalf = 460;   // ring.png half-width in design px (full 920)
        private const double CoreHalf = 158;   // core.png half-width in design px (full 316)
        private const double LaneIn = 175;
        private const double LaneOut = 333;
        private const double LaneH = 22;
        private const double LaneStep = 20;
        private const int LanesEachSide = 7;

        private static readonly Color Magenta = Color.FromRgb(0xFF, 0x1E, 0x8C);
        private static readonly Color Cyan = Color.FromRgb(0x16, 0xC0, 0xE8);
        private static readonly Color Blue = Color.FromRgb(0x2B, 0x56, 0xE0);
        private static readonly Color White = Colors.White;
        private static readonly Color Yellow = Color.FromRgb(0xFF, 0xE0, 0x00);
        private static readonly Color TitlePink = Color.FromRgb(0xFF, 0x5A, 0xB0);
        private static readonly Color DescBlue = Color.FromRgb(0xCF, 0xD6, 0xFF);
        private static readonly Color Ink = Color.FromRgb(0x1A, 0x10, 0x30);
        private static readonly Color[] LaneCols = { White, Cyan, Blue, White, Cyan, Magenta };

        private string _title = "", _description = "";
        private List<string> _items = new List<string>();
        private int _selected = 0;

        private readonly Canvas _root = new Canvas { Width = DesignW, Height = DesignH };
        private readonly Canvas _laneHost = new Canvas { Width = DesignW, Height = DesignH };
        private readonly Canvas _frame = new Canvas { Width = DesignW, Height = DesignH };
        private readonly RotateTransform _coreRot = new RotateTransform();
        private readonly ScaleTransform _haloScale = new ScaleTransform(1, 1);
        private TextBlock _titleTb, _descTb;

        public event Action<int> Confirmed;
        public event Action Cancelled;
        public event Action DefaultRequested;

        // sprite directory — change here if you keep art elsewhere
        public static string SpriteDir =
            System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "sprites");

        public WaccaWheelMenuRaster()
        {
            Stretch = Stretch.Uniform;
            Child = _root;
            Opacity = 0;
            Visibility = Visibility.Collapsed;

            BuildBackdrop();    // dark disc that masks the song wheel underneath
            BuildRing();        // ring.png  (or fallback)
            _root.Children.Add(_laneHost);
            BuildCore();        // core.png  (or fallback), rotates
            _root.Children.Add(_frame);
            BuildFrameText();   // title + description (vector, always crisp)
        }

        // =================================================================
        //  public API (identical to WaccaWheelMenu)
        // =================================================================
        public void Configure(string title, string description, IEnumerable<string> items, int selectedIndex)
        {
            _title = title ?? ""; _description = description ?? "";
            _items = new List<string>(items ?? new string[0]);
            _selected = Clamp(selectedIndex, 0, Math.Max(0, _items.Count - 1));
            _titleTb.Text = _title; _descTb.Text = _description;
            RebuildLanes();
        }

        public int SelectedIndex { get { return _selected; } }
        public string SelectedItem
        {
            get { return (_selected >= 0 && _selected < _items.Count) ? _items[_selected] : null; }
        }

        public void Step(int delta)
        {
            if (_items.Count == 0) return;
            _selected = Clamp(_selected + delta, 0, _items.Count - 1);
            RebuildLanes();
        }

        public int HitTestAngle(double deg)
        {
            double internalDeg = NormDeg(deg + 270);
            int k = (int)Math.Round((180 - internalDeg) / LaneStep);
            Step(k);
            return _selected;
        }

        public void RaiseConfirmed() { if (Confirmed != null) Confirmed(_selected); }

        public void Show()
        {
            Visibility = Visibility.Visible;
            BeginAnimation(OpacityProperty, new DoubleAnimation(1, new Duration(TimeSpan.FromMilliseconds(160))));
            StartAnim();
        }

        public void Hide()
        {
            var a = new DoubleAnimation(0, new Duration(TimeSpan.FromMilliseconds(160)));
            a.Completed += (s, e) => { Visibility = Visibility.Collapsed; StopAnim(); };
            BeginAnimation(OpacityProperty, a);
        }

        public bool IsOpen { get { return Visibility == Visibility.Visible; } }

        // expose so the launcher can fire these from its own button hits if it wants
        public void FireCancel() { if (Cancelled != null) Cancelled(); }
        public void FireDefault() { if (DefaultRequested != null) DefaultRequested(); }

        // =================================================================
        //  build
        // =================================================================
        private void BuildBackdrop()
        {
            var rg = new RadialGradientBrush { Center = new Point(0.5, 0.5), GradientOrigin = new Point(0.5, 0.45) };
            rg.GradientStops.Add(new GradientStop(Color.FromRgb(0x24, 0x1A, 0x52), 0));
            rg.GradientStops.Add(new GradientStop(Color.FromRgb(0x0E, 0x0A, 0x26), 1));
            var disc = new Ellipse { Width = RingHalf * 2, Height = RingHalf * 2, Fill = rg };
            Canvas.SetLeft(disc, _cx - RingHalf); Canvas.SetTop(disc, _cy - RingHalf);
            _root.Children.Add(disc);
        }

        private static BitmapImage TryLoad(string file)
        {
            try
            {
                string p = System.IO.Path.Combine(SpriteDir, file);
                if (!File.Exists(p)) return null;
                var bmp = new BitmapImage();
                bmp.BeginInit();
                bmp.CacheOption = BitmapCacheOption.OnLoad;     // don't lock the file
                bmp.UriSource = new Uri(p, UriKind.Absolute);
                bmp.EndInit();
                bmp.Freeze();
                return bmp;
            }
            catch { return null; }
        }

        private void BuildRing()
        {
            var bmp = TryLoad("ring.png");
            if (bmp != null)
            {
                var img = new Image { Source = bmp, Width = RingHalf * 2, Height = RingHalf * 2 };
                Canvas.SetLeft(img, _cx - RingHalf); Canvas.SetTop(img, _cy - RingHalf);
                _root.Children.Add(img);
            }
            else
            {
                // fallback: simple dark ring so the build runs without the art
                var ring = new Ellipse
                {
                    Width = RingHalf * 2, Height = RingHalf * 2,
                    Stroke = new SolidColorBrush(Color.FromRgb(0x3a, 0x22, 0x66)),
                    StrokeThickness = RingHalf - 341,
                    Fill = Brushes.Transparent
                };
                Canvas.SetLeft(ring, _cx - RingHalf); Canvas.SetTop(ring, _cy - RingHalf);
                _root.Children.Add(ring);
            }
        }

        private void BuildCore()
        {
            // pulsing magenta halo behind the core (vector, cheap, sells the glow)
            var halo = new RadialGradientBrush { Center = new Point(0.5, 0.5), GradientOrigin = new Point(0.5, 0.5) };
            halo.GradientStops.Add(new GradientStop(Color.FromArgb(0x00, Magenta.R, Magenta.G, Magenta.B), 0.66));
            halo.GradientStops.Add(new GradientStop(Color.FromArgb(0x66, Magenta.R, Magenta.G, Magenta.B), 0.86));
            halo.GradientStops.Add(new GradientStop(Color.FromArgb(0x00, Magenta.R, Magenta.G, Magenta.B), 1.0));
            var haloPath = new System.Windows.Shapes.Path
            {
                Data = new EllipseGeometry(new Point(0, 0), CoreHalf + 26, CoreHalf + 26),
                Fill = halo, RenderTransformOrigin = new Point(0.5, 0.5)
            };
            Canvas.SetLeft(haloPath, _cx); Canvas.SetTop(haloPath, _cy);
            haloPath.RenderTransform = _haloScale;
            _root.Children.Add(haloPath);

            var bmp = TryLoad("core.png");
            if (bmp != null)
            {
                var img = new Image
                {
                    Source = bmp, Width = CoreHalf * 2, Height = CoreHalf * 2,
                    RenderTransformOrigin = new Point(0.5, 0.5),
                    RenderTransform = _coreRot
                };
                Canvas.SetLeft(img, _cx - CoreHalf); Canvas.SetTop(img, _cy - CoreHalf);
                _root.Children.Add(img);
            }
            else
            {
                var coreFill = new RadialGradientBrush { Center = new Point(0.5, 0.52), GradientOrigin = new Point(0.5, 0.52) };
                coreFill.GradientStops.Add(new GradientStop(Color.FromRgb(0x3a, 0x22, 0x70), 0));
                coreFill.GradientStops.Add(new GradientStop(Color.FromRgb(0x08, 0x06, 0x1c), 1));
                var disc = new Ellipse { Width = CoreHalf * 2, Height = CoreHalf * 2, Fill = coreFill };
                Canvas.SetLeft(disc, _cx - CoreHalf); Canvas.SetTop(disc, _cy - CoreHalf);
                _root.Children.Add(disc);
                var rim = new Ellipse
                {
                    Width = CoreHalf * 2, Height = CoreHalf * 2,
                    Stroke = new SolidColorBrush(Magenta), StrokeThickness = 11, Fill = Brushes.Transparent
                };
                Canvas.SetLeft(rim, _cx - CoreHalf); Canvas.SetTop(rim, _cy - CoreHalf);
                _root.Children.Add(rim);
            }
        }

        private void BuildFrameText()
        {
            _titleTb = new TextBlock
            {
                Text = _title, FontSize = 30, FontWeight = FontWeights.Medium,
                Foreground = new SolidColorBrush(TitlePink), TextAlignment = TextAlignment.Center, Width = 560
            };
            Canvas.SetLeft(_titleTb, _cx - 280); Canvas.SetTop(_titleTb, 140);
            _frame.Children.Add(_titleTb);

            _descTb = new TextBlock
            {
                Text = _description, FontSize = 15, FontWeight = FontWeights.Normal,
                Foreground = new SolidColorBrush(DescBlue), TextWrapping = TextWrapping.Wrap, Width = 280
            };
            Canvas.SetLeft(_descTb, _cx + 50); Canvas.SetTop(_descTb, 198);
            _frame.Children.Add(_descTb);
        }

        // =================================================================
        //  lanes + banner + SET  (the dynamic layer, rebuilt on Step)
        // =================================================================
        private void RebuildLanes()
        {
            _laneHost.Children.Clear();
            if (_items.Count == 0) return;

            double effStep = LaneStep;
            if (_items.Count > 1)
                effStep = Math.Max(LaneStep, Math.Min(40.0, 300.0 / (_items.Count - 1)));

            int span = Math.Max(LanesEachSide, _items.Count);
            for (int k = -span; k <= span; k++)
            {
                int idx = _selected + k;
                if (idx < 0 || idx >= _items.Count || k == 0) continue;
                double deg = 180 - k * effStep;
                AddLane(deg, _items[idx], LaneCols[((idx % LaneCols.Length) + LaneCols.Length) % LaneCols.Length]);
            }
            AddBanner(_items[_selected]);
        }

        private void AddLane(double deg, string label, Color col)
        {
            var g = new Canvas { RenderTransform = new RotateTransform(deg, _cx, _cy) };
            var lane = new Rectangle
            {
                Width = LaneOut - LaneIn, Height = LaneH, RadiusX = LaneH / 2, RadiusY = LaneH / 2,
                Fill = new SolidColorBrush(col), Opacity = 0.97
            };
            Canvas.SetLeft(lane, _cx + LaneIn); Canvas.SetTop(lane, _cy - LaneH / 2);
            g.Children.Add(lane);

            double mid = _cx + (LaneIn + LaneOut) / 2;
            var t = new TextBlock
            {
                Text = label, FontSize = 17, FontWeight = FontWeights.SemiBold,
                Foreground = new SolidColorBrush(Ink)
            };
            t.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            double tw = t.DesiredSize.Width, th = t.DesiredSize.Height;
            Canvas.SetLeft(t, mid - tw / 2); Canvas.SetTop(t, _cy - th / 2);
            double nd = ((deg % 360) + 360) % 360;
            if (nd > 90 && nd < 270)
                t.RenderTransform = new RotateTransform(180, tw / 2, th / 2);  // keep text upright
            g.Children.Add(t);
            _laneHost.Children.Add(g);
        }

        private void AddBanner(string label)
        {
            double bw = _cx - LaneIn + 4, bh = 52;
            var banner = new Rectangle
            {
                Width = bw, Height = bh, RadiusX = 5, RadiusY = 5, Fill = new SolidColorBrush(Yellow)
            };
            Canvas.SetLeft(banner, 0); Canvas.SetTop(banner, _cy - bh / 2);
            _laneHost.Children.Add(banner);

            var big = new TextBlock
            {
                Text = label, FontSize = 38, FontWeight = FontWeights.Bold, Foreground = new SolidColorBrush(Ink)
            };
            big.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            Canvas.SetLeft(big, bw * 0.30 - big.DesiredSize.Width / 2 + 30);
            Canvas.SetTop(big, _cy - big.DesiredSize.Height / 2);
            _laneHost.Children.Add(big);

            double sx = _cx - LaneIn;
            var wedge = new Polygon
            {
                Points = new PointCollection
                {
                    new Point(sx - 58, _cy - 23), new Point(sx, _cy - 23),
                    new Point(sx, _cy + 23), new Point(sx - 58, _cy + 23), new Point(sx - 72, _cy)
                },
                Fill = new SolidColorBrush(Magenta)
            };
            _laneHost.Children.Add(wedge);
            var st = new TextBlock
            {
                Text = "SET", FontSize = 18, FontWeight = FontWeights.Medium, Foreground = new SolidColorBrush(White)
            };
            st.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            Canvas.SetLeft(st, sx - 50); Canvas.SetTop(st, _cy - st.DesiredSize.Height / 2);
            _laneHost.Children.Add(st);
        }

        // =================================================================
        //  animation
        // =================================================================
        private void StartAnim()
        {
            // RenderTransformOrigin=0.5 on the Image already pivots at its centre,
            // so the RotateTransform centre stays at 0.
            _coreRot.BeginAnimation(RotateTransform.AngleProperty,
                new DoubleAnimation(0, 360, new Duration(TimeSpan.FromSeconds(18)))
                { RepeatBehavior = RepeatBehavior.Forever });

            var pulse = new DoubleAnimation(1.0, 1.08, new Duration(TimeSpan.FromSeconds(0.95)))
            {
                AutoReverse = true, RepeatBehavior = RepeatBehavior.Forever,
                EasingFunction = new SineEase { EasingMode = EasingMode.EaseInOut }
            };
            _haloScale.BeginAnimation(ScaleTransform.ScaleXProperty, pulse);
            _haloScale.BeginAnimation(ScaleTransform.ScaleYProperty, pulse);
        }

        private void StopAnim()
        {
            _coreRot.BeginAnimation(RotateTransform.AngleProperty, null);
            _haloScale.BeginAnimation(ScaleTransform.ScaleXProperty, null);
            _haloScale.BeginAnimation(ScaleTransform.ScaleYProperty, null);
        }

        private static int Clamp(int v, int lo, int hi) { return v < lo ? lo : (v > hi ? hi : v); }
        private static double NormDeg(double d) { return ((d % 360) + 360) % 360; }
    }
}
