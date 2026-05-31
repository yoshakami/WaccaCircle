using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace SpinWheelApp
{
    /// <summary>
    /// Faithful recreation of the Wacca "réglage de la vitesse" wheel sub-menu.
    /// One reusable control; you Configure() it per arrowMode (title, description,
    /// the list of lane values/options, the selected index, button labels) and Show()
    /// it. Because it is vector + parameterised, every sub-menu (left category,
    /// difficulty, favourite/sort, back) reuses this SAME control — no extra mp4s.
    ///
    /// Layout (1000x1000 design space, Viewbox-scaled Uniform, same centre as
    /// WaccaBackground): a navy backdrop disc masks the song wheel underneath; value
    /// "lanes" radiate around the ring; the selected value snaps to the 9-o'clock SET
    /// slot as a big yellow banner; a pink animated core sits in the middle; title /
    /// description / Retour / défaut / OK + two arrows frame it.
    ///
    /// Events: Confirmed(index) on OK, Cancelled on Retour, DefaultRequested on défaut.
    /// Drive it from your existing wheel input: Step(±1) to move, HitTestAngle(deg) to
    /// jump, then raise Confirmed via your OK/決定 press.
    /// </summary>
    public class WaccaWheelMenu : Viewbox
    {
        // ---- design space (mirror WaccaBackground so they overlay exactly) ----
        private const double DesignW = 1000, DesignH = 1000;
        private double _cx = 500, _cy = 490;
        private const double BackdropR = 405;   // masks the song wheel beneath
        private const double CoreR = 155;        // navy animated core (bigger = closer to game)
        private const double LaneIn = 192;       // inner end of a lane
        private const double LaneOut = 394;      // outer end of a lane
        private const double LaneH = 27;         // lane thickness
        private const double LaneStep = 20;      // degrees between lanes
        private const int LanesEachSide = 7;     // how many lanes to draw around

        // ---- palette (from the screenshot) ------------------------------------
        private static readonly Color Navy = Color.FromRgb(0x1B, 0x13, 0x40);
        private static readonly Color Magenta = Color.FromRgb(0xFF, 0x1E, 0x8C);
        private static readonly Color Cyan = Color.FromRgb(0x16, 0xC0, 0xE8);
        private static readonly Color Blue = Color.FromRgb(0x2B, 0x56, 0xE0);
        private static readonly Color White = Colors.White;
        private static readonly Color Yellow = Color.FromRgb(0xFF, 0xE0, 0x00);
        private static readonly Color TitlePink = Color.FromRgb(0xFF, 0x5A, 0xB0);
        private static readonly Color DescBlue = Color.FromRgb(0xCF, 0xD6, 0xFF);
        private static readonly Color Ink = Color.FromRgb(0x1A, 0x10, 0x30);
        private static readonly Color[] LaneCols = { White, Cyan, Blue, White, Cyan, Magenta };

        // ---- config ------------------------------------------------------------
        private string _title = "";
        private string _description = "";
        private List<string> _items = new List<string>();
        private int _selected = 0;
        private string _setLabel = "SET";
        private string _backLabel = "Retour";
        private string _defaultLabel = "défaut";
        private string _okLabel = "OK";

        // ---- visual roots ------------------------------------------------------
        private readonly Canvas _root = new Canvas { Width = DesignW, Height = DesignH };
        private readonly Canvas _laneHost = new Canvas { Width = DesignW, Height = DesignH };
        private TextBlock _titleTb, _descTb;
        private readonly RotateTransform _ring1 = new RotateTransform();
        private readonly RotateTransform _ring2 = new RotateTransform();
        private readonly RotateTransform _ring3 = new RotateTransform();
        private readonly RotateTransform _arc = new RotateTransform();
        private readonly ScaleTransform _haloScale = new ScaleTransform(1, 1);
        private Path _halo;

        public event Action<int> Confirmed;
        public event Action Cancelled;
        public event Action DefaultRequested;

        public WaccaWheelMenu()
        {
            Stretch = Stretch.Uniform;
            Child = _root;
            Opacity = 0;
            Visibility = Visibility.Collapsed;

            BuildBackdrop();
            _root.Children.Add(_laneHost);   // lanes go between backdrop and core
            BuildCore();
            BuildFrame();
        }

        // =======================================================================
        //  public API
        // =======================================================================
        public void Configure(string title, string description, IEnumerable<string> items,
                              int selectedIndex)
        {
            _title = title ?? "";
            _description = description ?? "";
            _items = new List<string>(items ?? new string[0]);
            _selected = Clamp(selectedIndex, 0, Math.Max(0, _items.Count - 1));
            _titleTb.Text = _title;
            _descTb.Text = _description;
            RebuildLanes();
        }

        /// <summary>Set the button captions (defaults SET / Retour / défaut / OK).</summary>
        public void SetButtons(string set, string back, string deflt, string ok)
        {
            _setLabel = set; _backLabel = back; _defaultLabel = deflt; _okLabel = ok;
            BuildFrame();              // cheap; rebuilds the static caption layer
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

        /// <summary>Map a touch angle (deg, 0 = up, clockwise) to the lane under the
        /// finger and move the selection there. SET slot (9 o'clock) = no move.</summary>
        public int HitTestAngle(double deg)
        {
            double internalDeg = NormDeg(deg + 270);        // "0=up" input -> internal (0=right)
            int k = (int)Math.Round((180 - internalDeg) / LaneStep);
            Step(k);
            return _selected;
        }

        public void RaiseConfirmed() { if (Confirmed != null) Confirmed(_selected); }

        public void Show()
        {
            Visibility = Visibility.Visible;
            BeginAnimation(OpacityProperty, new DoubleAnimation(1, new Duration(TimeSpan.FromMilliseconds(160))));
            StartCoreAnim();
        }

        public void Hide()
        {
            var a = new DoubleAnimation(0, new Duration(TimeSpan.FromMilliseconds(160)));
            a.Completed += (s, e) => { Visibility = Visibility.Collapsed; StopCoreAnim(); };
            BeginAnimation(OpacityProperty, a);
        }

        public bool IsOpen { get { return Visibility == Visibility.Visible; } }

        // =======================================================================
        //  build: backdrop + core
        // =======================================================================
        private void BuildBackdrop()
        {
            var rg = new RadialGradientBrush { Center = new Point(0.5, 0.5), GradientOrigin = new Point(0.5, 0.45) };
            rg.GradientStops.Add(new GradientStop(Color.FromRgb(0x24, 0x1A, 0x52), 0));
            rg.GradientStops.Add(new GradientStop(Color.FromRgb(0x0E, 0x0A, 0x26), 1));
            var disc = new Ellipse { Width = BackdropR * 2, Height = BackdropR * 2, Fill = rg };
            Canvas.SetLeft(disc, _cx - BackdropR);
            Canvas.SetTop(disc, _cy - BackdropR);
            _root.Children.Add(disc);
        }

        private void BuildCore()
        {
            // The "tunnel": deep navy disc, radar ticks, several rotating dashed
            // rings, pink spiral arcs, a bright violet centre, and a thick glowing
            // magenta rim. Drawn inner -> outer so the rim sits on top.

            // outer magenta halo (soft glow, pulses)
            var halo = new RadialGradientBrush { Center = new Point(0.5, 0.5), GradientOrigin = new Point(0.5, 0.5) };
            halo.GradientStops.Add(new GradientStop(Color.FromArgb(0x00, Magenta.R, Magenta.G, Magenta.B), 0.62));
            halo.GradientStops.Add(new GradientStop(Color.FromArgb(0x88, Magenta.R, Magenta.G, Magenta.B), 0.84));
            halo.GradientStops.Add(new GradientStop(Color.FromArgb(0x00, Magenta.R, Magenta.G, Magenta.B), 1.0));
            _halo = new Path
            {
                Data = new EllipseGeometry(new Point(0, 0), CoreR + 34, CoreR + 34),
                Fill = halo,
                RenderTransformOrigin = new Point(0.5, 0.5)
            };
            Canvas.SetLeft(_halo, _cx); Canvas.SetTop(_halo, _cy);
            _halo.RenderTransform = _haloScale;
            _root.Children.Add(_halo);

            double inner = CoreR - 12;   // tunnel content radius

            // deep navy disc (slightly off-centre gradient = "looking down a well")
            var coreFill = new RadialGradientBrush { Center = new Point(0.5, 0.52), GradientOrigin = new Point(0.5, 0.52) };
            coreFill.GradientStops.Add(new GradientStop(Color.FromRgb(0x3a, 0x22, 0x70), 0));
            coreFill.GradientStops.Add(new GradientStop(Color.FromRgb(0x16, 0x10, 0x3a), 0.55));
            coreFill.GradientStops.Add(new GradientStop(Color.FromRgb(0x08, 0x06, 0x1c), 1));
            var disc = new Ellipse { Width = inner * 2, Height = inner * 2, Fill = coreFill };
            Canvas.SetLeft(disc, _cx - inner); Canvas.SetTop(disc, _cy - inner);
            _root.Children.Add(disc);

            // radar tick marks all around (faint)
            var tickBrush = new SolidColorBrush(Color.FromArgb(0x40, 0x3a, 0x6a, 0x8a));
            for (int i = 0; i < 36; i++)
            {
                double a = i * (Math.PI / 18);
                double r0 = inner * 0.55, r1 = inner * 0.92;
                _root.Children.Add(new Line
                {
                    X1 = _cx + r0 * Math.Cos(a), Y1 = _cy + r0 * Math.Sin(a),
                    X2 = _cx + r1 * Math.Cos(a), Y2 = _cy + r1 * Math.Sin(a),
                    Stroke = tickBrush, StrokeThickness = 1
                });
            }

            // three concentric dashed rings, different radii (rotate at diff speeds)
            _root.Children.Add(DashedRing(inner * 0.88, new double[] { 4, 7 }, 0.55, _ring1));
            _root.Children.Add(DashedRing(inner * 0.66, new double[] { 3, 10 }, 0.40, _ring2));
            var ring3 = DashedRing(inner * 0.46, new double[] { 2, 8 }, 0.50, _ring3);
            _root.Children.Add(ring3);

            // pink spiral arcs (rotate together on their own host)
            var arcHost = new Canvas { RenderTransform = _arc };
            arcHost.Children.Add(Arc(inner * 0.90, 180, 95));   // big bottom-left sweep
            arcHost.Children.Add(Arc(inner * 0.72, 0, 90));     // smaller top-right sweep
            _root.Children.Add(arcHost);

            // bright violet centre point
            var cg = new RadialGradientBrush { Center = new Point(0.5, 0.5), GradientOrigin = new Point(0.5, 0.5) };
            cg.GradientStops.Add(new GradientStop(Color.FromRgb(0xCF, 0xE6, 0xFF), 0.0));
            cg.GradientStops.Add(new GradientStop(Color.FromArgb(0xB0, 0x7A, 0x4A, 0xD0), 0.55));
            cg.GradientStops.Add(new GradientStop(Color.FromArgb(0x00, 0x2A, 0x15, 0x60), 1.0));
            double cr = inner * 0.34;
            var centre = new Ellipse { Width = cr * 2, Height = cr * 2, Fill = cg };
            Canvas.SetLeft(centre, _cx - cr); Canvas.SetTop(centre, _cy - cr);
            _root.Children.Add(centre);

            // thin cyan inner edge
            var cyanEdge = new Ellipse
            {
                Width = inner * 2, Height = inner * 2,
                Stroke = new SolidColorBrush(Cyan), StrokeThickness = 2, Opacity = 0.7,
                Fill = Brushes.Transparent
            };
            Canvas.SetLeft(cyanEdge, _cx - inner); Canvas.SetTop(cyanEdge, _cy - inner);
            _root.Children.Add(cyanEdge);

            // soft magenta rim glow (wide, blurred via low-opacity wide stroke)
            var rimGlow = new Ellipse
            {
                Width = CoreR * 2, Height = CoreR * 2,
                Stroke = new SolidColorBrush(Color.FromArgb(0x55, Magenta.R, Magenta.G, Magenta.B)),
                StrokeThickness = 24, Fill = Brushes.Transparent,
                Effect = new System.Windows.Media.Effects.BlurEffect { Radius = 10 }
            };
            Canvas.SetLeft(rimGlow, _cx - CoreR); Canvas.SetTop(rimGlow, _cy - CoreR);
            _root.Children.Add(rimGlow);

            // thick bright magenta rim (the donut)
            var rim = new Ellipse
            {
                Width = CoreR * 2, Height = CoreR * 2,
                Stroke = new SolidColorBrush(Magenta), StrokeThickness = 13,
                Fill = Brushes.Transparent
            };
            Canvas.SetLeft(rim, _cx - CoreR); Canvas.SetTop(rim, _cy - CoreR);
            _root.Children.Add(rim);
        }

        private Ellipse DashedRing(double r, double[] dash, double opacity, RotateTransform rot)
        {
            rot.CenterX = _cx; rot.CenterY = _cy;
            var e = new Ellipse
            {
                Width = r * 2,
                Height = r * 2,
                Stroke = new SolidColorBrush(Cyan),
                StrokeThickness = 1.5,
                Opacity = opacity,
                Fill = Brushes.Transparent,
                StrokeDashArray = new DoubleCollection(dash),
                RenderTransform = rot
            };
            Canvas.SetLeft(e, _cx - r); Canvas.SetTop(e, _cy - r);
            return e;
        }

        private Path Arc(double r, double startDeg, double sweepDeg)
        {
            double a0 = startDeg * Math.PI / 180, a1 = (startDeg + sweepDeg) * Math.PI / 180;
            var p0 = new Point(_cx + r * Math.Cos(a0), _cy + r * Math.Sin(a0));
            var p1 = new Point(_cx + r * Math.Cos(a1), _cy + r * Math.Sin(a1));
            var fig = new PathFigure { StartPoint = p0, IsClosed = false };
            fig.Segments.Add(new ArcSegment(p1, new Size(r, r), 0, false, SweepDirection.Clockwise, true));
            var geo = new PathGeometry(); geo.Figures.Add(fig);
            return new Path
            {
                Data = geo,
                Stroke = new SolidColorBrush(Color.FromRgb(0xFF, 0x3D, 0xA0)),
                StrokeThickness = 4,
                StrokeStartLineCap = PenLineCap.Round,
                StrokeEndLineCap = PenLineCap.Round
            };
        }

        // =======================================================================
        //  build: the value lanes + selected banner + SET  (rebuilt on Step)
        // =======================================================================
        private void RebuildLanes()
        {
            _laneHost.Children.Clear();
            if (_items.Count == 0) return;

            // Adaptive step: many values (speed wheel) pack tight at LaneStep; a short
            // list (categories, difficulties) spreads wider so it still fills the ring
            // instead of bunching at the bottom. Total fan kept under ~320 deg so it
            // never collides with the title panel at the top.
            double effStep = LaneStep;
            if (_items.Count > 1)
                effStep = Math.Max(LaneStep, Math.Min(40.0, 300.0 / (_items.Count - 1)));

            int span = Math.Max(LanesEachSide, _items.Count);   // draw all items if few
            for (int k = -span; k <= span; k++)
            {
                int idx = _selected + k;
                if (idx < 0 || idx >= _items.Count) continue;
                if (k == 0) continue;                            // selected handled by banner
                double deg = 180 - k * effStep;                  // 180 = 9 o'clock; higher values clockwise (down)
                AddLane(deg, _items[idx], LaneCols[((idx % LaneCols.Length) + LaneCols.Length) % LaneCols.Length]);
            }

            AddBanner(_items[_selected]);                        // big yellow banner + SET wedge
        }

        private void AddLane(double deg, string label, Color col)
        {
            var g = new Canvas { RenderTransform = new RotateTransform(deg, _cx, _cy) };
            var lane = new Rectangle
            {
                Width = LaneOut - LaneIn,
                Height = LaneH,
                RadiusX = LaneH / 2,
                RadiusY = LaneH / 2,
                Fill = new SolidColorBrush(col),
                Opacity = 0.95
            };
            Canvas.SetLeft(lane, _cx + LaneIn);
            Canvas.SetTop(lane, _cy - LaneH / 2);
            g.Children.Add(lane);

            double mid = _cx + (LaneIn + LaneOut) / 2;
            var t = new TextBlock
            {
                Text = label,
                FontSize = 20,
                FontWeight = FontWeights.Medium,
                Foreground = new SolidColorBrush(Ink)
            };
            t.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            double tw = t.DesiredSize.Width, th = t.DesiredSize.Height;
            Canvas.SetLeft(t, mid - tw / 2);
            Canvas.SetTop(t, _cy - th / 2);

            // The group rotates the whole lane by `deg`. If the lane points into the
            // left half (deg between 90 and 270), the text would be upside down
            // ("Pop" -> "dod"), so flip it 180 about its own centre. Result: text
            // always reads upright, radially, like the real wheel.
            double nd = ((deg % 360) + 360) % 360;
            if (nd > 90 && nd < 270)
                t.RenderTransform = new RotateTransform(180, tw / 2, th / 2);

            g.Children.Add(t);                                  // group's rotate(deg) orients the lane
            _laneHost.Children.Add(g);
        }

        private void AddBanner(string label)
        {
            double bx = 20, bw = _cx - LaneIn - 6, bh = 64;
            var banner = new Rectangle
            {
                Width = bw,
                Height = bh,
                RadiusX = 6,
                RadiusY = 6,
                Fill = new SolidColorBrush(Yellow)
            };
            Canvas.SetLeft(banner, bx);
            Canvas.SetTop(banner, _cy - bh / 2);
            _laneHost.Children.Add(banner);

            var big = new TextBlock
            {
                Text = label,
                FontSize = 46,
                FontWeight = FontWeights.Bold,
                Foreground = new SolidColorBrush(Ink)
            };
            big.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            Canvas.SetLeft(big, bx + 36);
            Canvas.SetTop(big, _cy - big.DesiredSize.Height / 2);
            _laneHost.Children.Add(big);

            // SET wedge pointing right into the core (arrow on its left edge)
            double sx = _cx - LaneIn - 4, sy = _cy;
            var wedge = new Polygon
            {
                Points = new PointCollection
                {
                    new Point(sx - 66, sy - 28), new Point(sx, sy - 28),
                    new Point(sx, sy + 28), new Point(sx - 66, sy + 28),
                    new Point(sx - 82, sy)
                },
                Fill = new SolidColorBrush(Magenta)
            };
            _laneHost.Children.Add(wedge);
            var st = new TextBlock
            {
                Text = _setLabel,
                FontSize = 22,
                FontWeight = FontWeights.Medium,
                Foreground = new SolidColorBrush(White)
            };
            st.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            Canvas.SetLeft(st, sx - 60);
            Canvas.SetTop(st, sy - st.DesiredSize.Height / 2);
            _laneHost.Children.Add(st);
        }

        // =======================================================================
        //  build: title / description / Retour / défaut / OK / arrows
        // =======================================================================
        private FrameworkElement _frameLayer;
        private void BuildFrame()
        {
            if (_frameLayer != null) _root.Children.Remove(_frameLayer);
            var layer = new Canvas { Width = DesignW, Height = DesignH };
            _frameLayer = layer;

            _titleTb = Label(_title, TitlePink, 30, FontWeights.Medium);
            _titleTb.TextAlignment = TextAlignment.Center;
            _titleTb.Width = 560;
            Canvas.SetLeft(_titleTb, _cx - 280);
            Canvas.SetTop(_titleTb, 132);
            layer.Children.Add(_titleTb);

            _descTb = Label(_description, DescBlue, 15, FontWeights.Normal);
            _descTb.Width = 300;
            _descTb.TextWrapping = TextWrapping.Wrap;
            Canvas.SetLeft(_descTb, _cx + 40);
            Canvas.SetTop(_descTb, 196);
            layer.Children.Add(_descTb);

            AddButton(layer, _backLabel, TitlePink, _cx - 320, _cy + 300, () => { if (Cancelled != null) Cancelled(); });
            AddButton(layer, _defaultLabel, DescBlue, _cx + 300, _cy + 8, () => { if (DefaultRequested != null) DefaultRequested(); });

            // OK / 決定 at the very bottom
            var ok = Label(_okLabel, TitlePink, 24, FontWeights.Medium);
            Canvas.SetLeft(ok, _cx + 18);
            Canvas.SetTop(ok, _cy + 392);
            ok.MouseLeftButtonUp += (s, e) => RaiseConfirmed();
            layer.Children.Add(ok);
            var stamp = Sunburst(_cx - 8, _cy + 404, 22, Magenta);
            stamp.MouseLeftButtonUp += (s, e) => RaiseConfirmed();
            layer.Children.Add(stamp);

            // two arrows (previous / next) at the bottom corners
            ArrowButton(layer, _cx - 165, _cy + 380, false, () => Step(-1));
            ArrowButton(layer, _cx + 165, _cy + 380, true, () => Step(1));

            _root.Children.Add(_frameLayer);
        }

        private void AddButton(Canvas layer, string text, Color border, double x, double y, Action onClick)
        {
            var tb = new TextBlock
            {
                Text = text,
                FontSize = 18,
                FontWeight = FontWeights.Medium,
                Foreground = new SolidColorBrush(Color.FromRgb(0xF6, 0xC6, 0xE0))
            };
            var b = new Border
            {
                Child = tb,
                Padding = new Thickness(16, 7, 16, 7),
                CornerRadius = new CornerRadius(999),
                Background = new SolidColorBrush(Color.FromArgb(0xC8, 0x2A, 0x12, 0x40)),
                BorderThickness = new Thickness(2),
                BorderBrush = new SolidColorBrush(border)
            };
            b.MouseLeftButtonUp += (s, e) => { if (onClick != null) onClick(); };
            Canvas.SetLeft(b, x); Canvas.SetTop(b, y);
            layer.Children.Add(b);
        }

        private void ArrowButton(Canvas layer, double cx, double cy, bool right, Action onClick)
        {
            var circle = new Ellipse
            {
                Width = 56, Height = 56,
                Fill = new SolidColorBrush(Color.FromRgb(0x12, 0x10, 0x2E)),
                Stroke = new SolidColorBrush(Color.FromArgb(0x80, 0x9a, 0x6a, 0xff)),
                StrokeThickness = 2
            };
            Canvas.SetLeft(circle, cx - 28); Canvas.SetTop(circle, cy - 28);
            circle.MouseLeftButtonUp += (s, e) => { if (onClick != null) onClick(); };
            layer.Children.Add(circle);

            double dir = right ? 1 : -1;
            var tri = new Polygon
            {
                Points = new PointCollection
                {
                    new Point(cx + dir * 10, cy), new Point(cx - dir * 8, cy - 11), new Point(cx - dir * 8, cy + 11)
                },
                Fill = new SolidColorBrush(White)
            };
            tri.MouseLeftButtonUp += (s, e) => { if (onClick != null) onClick(); };
            layer.Children.Add(tri);
        }

        private Canvas Sunburst(double x, double y, double r, Color col)
        {
            var c = new Canvas();
            int spikes = 12;
            for (int i = 0; i < spikes; i++)
            {
                double a = i * (360.0 / spikes) * Math.PI / 180;
                double dx = Math.Cos(a), dy = Math.Sin(a), px = -dy, py = dx, w = 3.5;
                c.Children.Add(new Polygon
                {
                    Points = new PointCollection
                    {
                        new Point(x + r*0.45*dx + px*w, y + r*0.45*dy + py*w),
                        new Point(x + r*dx, y + r*dy),
                        new Point(x + r*0.45*dx - px*w, y + r*0.45*dy - py*w)
                    },
                    Fill = new SolidColorBrush(col)
                });
            }
            c.Children.Add(new Ellipse
            {
                Width = r, Height = r,
                Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xE0, 0x00)),
                RenderTransform = new TranslateTransform(x - r / 2, y - r / 2)
            });
            return c;
        }

        private static TextBlock Label(string text, Color color, double size, FontWeight weight)
        {
            return new TextBlock
            {
                Text = text,
                FontSize = size,
                FontWeight = weight,
                Foreground = new SolidColorBrush(color)
            };
        }

        // =======================================================================
        //  core animation
        // =======================================================================
        private void StartCoreAnim()
        {
            Loop(_ring1, RotateTransform.AngleProperty, 0, 360, 14);
            Loop(_ring2, RotateTransform.AngleProperty, 360, 0, 9);
            Loop(_ring3, RotateTransform.AngleProperty, 0, 360, 6);
            Loop(_arc, RotateTransform.AngleProperty, 0, 360, 5);
            var pulse = new DoubleAnimation(1.0, 1.10, new Duration(TimeSpan.FromSeconds(0.95)))
            {
                AutoReverse = true,
                RepeatBehavior = RepeatBehavior.Forever,
                EasingFunction = new SineEase { EasingMode = EasingMode.EaseInOut }
            };
            _haloScale.BeginAnimation(ScaleTransform.ScaleXProperty, pulse);
            _haloScale.BeginAnimation(ScaleTransform.ScaleYProperty, pulse);
        }

        private void StopCoreAnim()
        {
            _ring1.BeginAnimation(RotateTransform.AngleProperty, null);
            _ring2.BeginAnimation(RotateTransform.AngleProperty, null);
            _ring3.BeginAnimation(RotateTransform.AngleProperty, null);
            _arc.BeginAnimation(RotateTransform.AngleProperty, null);
            _haloScale.BeginAnimation(ScaleTransform.ScaleXProperty, null);
            _haloScale.BeginAnimation(ScaleTransform.ScaleYProperty, null);
        }

        private static void Loop(Animatable t, DependencyProperty p, double from, double to, double secs)
        {
            t.BeginAnimation(p, new DoubleAnimation(from, to, new Duration(TimeSpan.FromSeconds(secs)))
            {
                RepeatBehavior = RepeatBehavior.Forever
            });
        }

        private static int Clamp(int v, int lo, int hi) { return v < lo ? lo : (v > hi ? hi : v); }
        private static double NormDeg(double d) { return ((d % 360) + 360) % 360; }
    }
}
