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
    /// WaccaWedgeMenu — fullscreen circular sub-menu.
    ///
    /// Differences from the earlier WaccaSpeedWheel, per request:
    ///   * a SOLID BLACK full-screen backdrop covers the whole window (masks the main
    ///     game wheel behind it) instead of a small disc;
    ///   * options are drawn as "cake slices" (wedges) — the label sits between two
    ///     arcs of the wedge — instead of thin pills;
    ///   * each non-selected wedge is filled with an ANIMATED rose<->blue gradient
    ///     that sweeps; the selected wedge is the solid yellow SET slot at 9 o'clock;
    ///   * the pink animated tunnel core is kept exactly as-is on top.
    ///
    /// Same public API as before (Configure / Step / HitTestAngle / Show / Hide /
    /// RaiseConfirmed + Confirmed / Cancelled / DefaultRequested), so the launcher
    /// wiring is unchanged. Feed it real values via WaccaMenuData (see that file).
    ///
    /// Built entirely in code, original vector art (no game assets bundled), wrapped
    /// in a Viewbox. Add it to the launcher's GRID so the black backdrop fills the
    /// whole window.
    /// </summary>
    public class WaccaWedgeMenu : Viewbox
    {
        private const double D = 560;
        private double _cx = 280, _cy = 280;

        private const double RIn = 150, ROut = 250;     // wedge band
        private const double CoreR = 120, Inner = CoreR - 9;
        private const int WindowSize = 11;              // visible wedges (odd; centre = selected)
        private const double Span = 200.0;              // total fan degrees

        // palette (sampled from the screen)
        private static readonly Color Magenta = Hex(0xFF1E8C), MagentaHi = Hex(0xFF3DA0);
        private static readonly Color Cyan = Hex(0x23C4E0), Yellow = Hex(0xFFEE00);
        private static readonly Color TitlePink = Hex(0xFF5AB0), DescBlue = Hex(0xCFD6FF);
        private static readonly Color Ink = Hex(0x1A1030), White = Hex(0xFFFEF8);
        private static readonly Color Rose = Hex(0xFF2D8F), Blue = Hex(0x2B56E0), Sky = Hex(0x00A8E9);

        private string _title = "", _description = "";
        private List<string> _items = new List<string>();
        private int _selected = 0;

        private readonly Canvas _root = new Canvas { Width = D, Height = D };
        private readonly Canvas _wedges = new Canvas { Width = D, Height = D };
        private readonly Canvas _frame = new Canvas { Width = D, Height = D };
        private TextBlock _titleTb, _descTb;

        // the two animated gradient brushes shared by the wedges
        private LinearGradientBrush _gradA, _gradB;
        private readonly List<RingAnim> _spinners = new List<RingAnim>();
        private readonly ScaleTransform _centreScale = new ScaleTransform(1, 1);
        private UIElement _centre;

        public event Action<int> Confirmed;
        public event Action Cancelled;
        public event Action DefaultRequested;

        private struct RingAnim
        {
            public RotateTransform T; public double Secs; public bool Rev;
            public RingAnim(RotateTransform t, double s, bool r) { T = t; Secs = s; Rev = r; }
        }

        public WaccaWedgeMenu()
        {
            Stretch = Stretch.Uniform;
            Child = _root;
            Opacity = 0;
            Visibility = Visibility.Collapsed;

            BuildBlackBackdrop();
            BuildGradients();
            _root.Children.Add(_wedges);
            BuildCore();
            _root.Children.Add(_frame);
            BuildFrame();
        }

        // ================= public API =================
        public void Configure(string title, string description, IEnumerable<string> items, int selectedIndex)
        {
            _title = title ?? ""; _description = description ?? "";
            _items = new List<string>(items ?? new string[0]);
            _selected = Clamp(selectedIndex, 0, Math.Max(0, _items.Count - 1));
            _titleTb.Text = _title; _descTb.Text = _description;
            RebuildWedges();
        }

        /// <summary>Convenience: configure straight from a WaccaMenuData.Spec.</summary>
        public void ConfigureFromSpec(WaccaMenuData.Spec spec)
        {
            Configure(spec.Title, spec.Description, spec.Items, spec.SelectedIndex);
        }

        public int SelectedIndex { get { return _selected; } }
        public string SelectedItem
        { get { return (_selected >= 0 && _selected < _items.Count) ? _items[_selected] : null; } }

        public void Step(int delta)
        {
            if (_items.Count == 0) return;
            _selected = Clamp(_selected + delta, 0, _items.Count - 1);
            RebuildWedges();
        }

        public int HitTestAngle(double deg)
        {
            double internalDeg = Norm(deg + 270);
            double step = Span / (WindowSize - 1);
            Step((int)Math.Round((180 - internalDeg) / step));
            return _selected;
        }

        public void RaiseConfirmed() { if (Confirmed != null) Confirmed(_selected); }
        public void FireCancel() { if (Cancelled != null) Cancelled(); }
        public void FireDefault() { if (DefaultRequested != null) DefaultRequested(); }

        public void Show()
        {
            Visibility = Visibility.Visible;
            BeginAnimation(OpacityProperty, new DoubleAnimation(1, Dur(180)));
            StartAnim();
        }

        public void Hide()
        {
            var a = new DoubleAnimation(0, Dur(180));
            a.Completed += (s, e) => { Visibility = Visibility.Collapsed; StopAnim(); };
            BeginAnimation(OpacityProperty, a);
        }

        public bool IsOpen { get { return Visibility == Visibility.Visible; } }

        // ================= backdrop (fullscreen black) =================
        private void BuildBlackBackdrop()
        {
            // A black rectangle far larger than the design box: because the control is a
            // Viewbox(Uniform), the visible area is the design square, but we oversize the
            // rect so that however the host stretches/letterboxes it, black still covers
            // the whole window and hides the game wheel behind.
            var black = new Rectangle { Width = D * 3, Height = D * 3, Fill = Brushes.Black };
            Canvas.SetLeft(black, -D); Canvas.SetTop(black, -D);
            _root.Children.Add(black);
        }

        // ================= animated rose/blue gradients =================
        private void BuildGradients()
        {
            _gradA = MakeSweep(Rose, Blue, 3.0);
            _gradB = MakeSweep(Sky, Rose, 4.0);
        }

        private LinearGradientBrush MakeSweep(Color c0, Color c1, double secs)
        {
            var b = new LinearGradientBrush
            {
                StartPoint = new Point(0, 0),
                EndPoint = new Point(1, 1),
                MappingMode = BrushMappingMode.RelativeToBoundingBox
            };
            b.GradientStops.Add(new GradientStop(c0, 0));
            b.GradientStops.Add(new GradientStop(c1, 1));
            // animate the start point so the colour band sweeps across the wedge
            var anim = new PointAnimationUsingKeyFrames { RepeatBehavior = RepeatBehavior.Forever };
            anim.KeyFrames.Add(new LinearPointKeyFrame(new Point(0, 0), KeyTime.FromPercent(0)));
            anim.KeyFrames.Add(new LinearPointKeyFrame(new Point(1, 0), KeyTime.FromPercent(0.5)));
            anim.KeyFrames.Add(new LinearPointKeyFrame(new Point(0, 0), KeyTime.FromPercent(1)));
            anim.Duration = Dur(secs * 1000);
            b.BeginAnimation(LinearGradientBrush.StartPointProperty, anim);
            return b;
        }

        // ================= wedges (rebuilt on Step) =================
        private void RebuildWedges()
        {
            _wedges.Children.Clear();
            if (_items.Count == 0) return;

            int half = (WindowSize - 1) / 2;
            double step = Span / (WindowSize - 1);
            for (int k = -half; k <= half; k++)
            {
                int idx = _selected + k;
                if (idx < 0 || idx >= _items.Count) continue;
                AddWedge(180 + k * step, step, _items[idx], k == 0, Math.Abs(k), half, idx);
            }
        }

        private void AddWedge(double midDeg, double step, string label, bool selected,
                              int dist, int half, int idx)
        {
            double a0 = (midDeg - step * 0.46) * Math.PI / 180;
            double a1 = (midDeg + step * 0.46) * Math.PI / 180;
            Point p0 = Pol(RIn, a0), p1 = Pol(ROut, a0), p2 = Pol(ROut, a1), p3 = Pol(RIn, a1);

            var fig = new PathFigure { StartPoint = p0, IsClosed = true };
            fig.Segments.Add(new LineSegment(p1, true));
            fig.Segments.Add(new ArcSegment(p2, new Size(ROut, ROut), 0, false, SweepDirection.Clockwise, true));
            fig.Segments.Add(new LineSegment(p3, true));
            fig.Segments.Add(new ArcSegment(p0, new Size(RIn, RIn), 0, false, SweepDirection.Counterclockwise, true));
            var geo = new PathGeometry(); geo.Figures.Add(fig);

            Brush fill = selected ? (Brush)new SolidColorBrush(Yellow)
                                   : (idx % 2 == 0 ? _gradA : _gradB);
            var wedge = new Path
            {
                Data = geo,
                Fill = fill,
                Opacity = selected ? 1.0 : (0.35 + 0.5 * (1 - dist / (double)half)),
                Stroke = Brushes.Black,
                StrokeThickness = 2.5
            };
            _wedges.Children.Add(wedge);

            // label between the two arcs (radial, kept upright)
            double rmid = (RIn + ROut) / 2, am = midDeg * Math.PI / 180;
            Point lp = Pol(rmid, am);
            double rot = midDeg; double nd = Norm(midDeg);
            if (nd > 90 && nd < 270) rot += 180;
            var t = new TextBlock
            {
                Text = label,
                FontSize = selected ? 13 : 10,
                FontWeight = selected ? FontWeights.Bold : FontWeights.Medium,
                Foreground = new SolidColorBrush(selected ? Ink : White),
                RenderTransformOrigin = new Point(0.5, 0.5)
            };
            t.Measure(Inf);
            var grp = new TransformGroup();
            grp.Children.Add(new RotateTransform(rot));
            t.RenderTransform = grp;
            Canvas.SetLeft(t, lp.X - t.DesiredSize.Width / 2);
            Canvas.SetTop(t, lp.Y - t.DesiredSize.Height / 2);
            _wedges.Children.Add(t);
        }

        // ================= pink tunnel core (kept) =================
        private void BuildCore()
        {
            var core = new Canvas();

            var well = Radial(new[] {
                Stop(Hex(0x8A6CFF), 0.0), Stop(Hex(0x4A2F9A), 0.30),
                Stop(Hex(0x221751), 0.62), Stop(Hex(0x070416), 1.0) }, 0.48, 0.46);
            var disc = new Ellipse { Width = Inner * 2, Height = Inner * 2, Fill = well };
            Place(disc, _cx - Inner, _cy - Inner);
            core.Children.Add(disc);

            var tick = new SolidColorBrush(Hex(0x5A7DA0)) { Opacity = 0.22 };
            for (int i = 0; i < 48; i++)
            {
                double a = (i / 48.0) * 2 * Math.PI, r0 = Inner * 0.5, r1 = Inner * 0.9;
                core.Children.Add(new Line
                {
                    X1 = _cx + r0 * Math.Cos(a), Y1 = _cy + r0 * Math.Sin(a),
                    X2 = _cx + r1 * Math.Cos(a), Y2 = _cy + r1 * Math.Sin(a),
                    Stroke = tick, StrokeThickness = 0.6
                });
            }

            core.Children.Add(DashedRing(Inner * 0.90, new double[] { 5, 8 }, 0.5, 30, false));
            core.Children.Add(DashedRing(Inner * 0.72, new double[] { 3, 11 }, 0.4, 22, true));
            core.Children.Add(DashedRing(Inner * 0.54, new double[] { 2, 7 }, 0.45, 16, false));
            core.Children.Add(DashedRing(Inner * 0.38, new double[] { 2, 12 }, 0.35, 12, true));

            var arcRot = new RotateTransform { CenterX = _cx, CenterY = _cy };
            var arcs = new Canvas { RenderTransform = arcRot };
            arcs.Children.Add(ArcPath(Inner * 0.88, Math.PI * 0.95, Math.PI * 1.70, MagentaHi, 3, 0.92));
            arcs.Children.Add(ArcPath(Inner * 0.66, Math.PI * 0.05, Math.PI * 0.50, Hex(0xFF6AB8), 2, 0.8));
            arcs.Children.Add(ArcPath(Inner * 0.50, Math.PI * 1.20, Math.PI * 1.55, Cyan, 1.5, 0.6));
            core.Children.Add(arcs);
            _spinners.Add(new RingAnim(arcRot, 7, false));

            var ctr = Radial(new[] {
                Stop(Hex(0xDFE9FF), 0.0), StopA(Hex(0x9A7BFF), 0.45, 0xCC), StopA(Hex(0x5A32C0), 1.0, 0x00) }, 0.5, 0.5);
            double cr = 30;
            var centre = new Ellipse { Width = cr * 2, Height = cr * 2, Fill = ctr,
                RenderTransformOrigin = new Point(0.5, 0.5), RenderTransform = _centreScale };
            Place(centre, _cx - cr, _cy - cr);
            core.Children.Add(centre); _centre = centre;

            core.Children.Add(Ring(Inner + 1, Cyan, 1.5, 0.6));
            core.Children.Add(Ring(CoreR, Magenta, 10, 1));

            _root.Children.Add(core);
        }

        // ================= frame: title / desc / buttons =================
        private void BuildFrame()
        {
            _titleTb = new TextBlock { Text = _title, FontSize = 17, FontWeight = FontWeights.SemiBold,
                Foreground = new SolidColorBrush(TitlePink), TextAlignment = TextAlignment.Center, Width = 300 };
            Place(_titleTb, _cx - 150, 16);
            _frame.Children.Add(_titleTb);

            _descTb = new TextBlock { Text = _description, FontSize = 10.5, Foreground = new SolidColorBrush(DescBlue),
                TextWrapping = TextWrapping.Wrap, TextAlignment = TextAlignment.Center, Width = 240 };
            Place(_descTb, _cx - 120, 40);
            _frame.Children.Add(_descTb);

            Pill("Retour", TitlePink, 30, D - 52, () => FireCancel());
            Pill("défaut", DescBlue, D - 92, _cy - 10, () => FireDefault());

            var ok = new TextBlock { Text = "OK", FontSize = 16, FontWeight = FontWeights.Medium,
                Foreground = new SolidColorBrush(TitlePink) };
            Place(ok, _cx + 12, D - 42);
            ok.MouseLeftButtonUp += (s, e) => RaiseConfirmed();
            _frame.Children.Add(ok);
            var stamp = Sunburst(_cx - 6, D - 32, 13, Magenta);
            stamp.MouseLeftButtonUp += (s, e) => RaiseConfirmed();
            _frame.Children.Add(stamp);

            ArrowBtn(_cx - 96, D - 50, false, () => Step(-1));
            ArrowBtn(_cx + 96, D - 50, true, () => Step(1));
        }

        private void Pill(string text, Color border, double x, double y, Action onClick)
        {
            var tb = new TextBlock { Text = text, FontSize = 12, FontWeight = FontWeights.Medium,
                Foreground = new SolidColorBrush(Hex(0xF6C6E0)) };
            var b = new Border
            {
                Child = tb, Padding = new Thickness(12, 5, 12, 5), CornerRadius = new CornerRadius(999),
                Background = new SolidColorBrush(Color.FromArgb(0xC8, 0x2A, 0x12, 0x40)),
                BorderThickness = new Thickness(1.5), BorderBrush = new SolidColorBrush(border)
            };
            b.MouseLeftButtonUp += (s, e) => { if (onClick != null) onClick(); };
            Place(b, x, y);
            _frame.Children.Add(b);
        }

        private void ArrowBtn(double cx, double cy, bool right, Action onClick)
        {
            var c = new Ellipse { Width = 38, Height = 38, Fill = new SolidColorBrush(Hex(0x12102E)),
                Stroke = new SolidColorBrush(Color.FromArgb(0x80, 0x9A, 0x6A, 0xFF)), StrokeThickness = 1.5 };
            Place(c, cx - 19, cy - 19);
            c.MouseLeftButtonUp += (s, e) => { if (onClick != null) onClick(); };
            _frame.Children.Add(c);
            double d = right ? 1 : -1;
            var tri = new Polygon
            {
                Points = new PointCollection {
                    new Point(cx + d * 7, cy), new Point(cx - d * 6, cy - 8), new Point(cx - d * 6, cy + 8) },
                Fill = new SolidColorBrush(White)
            };
            tri.MouseLeftButtonUp += (s, e) => { if (onClick != null) onClick(); };
            _frame.Children.Add(tri);
        }

        private Canvas Sunburst(double x, double y, double r, Color col)
        {
            var c = new Canvas();
            for (int i = 0; i < 12; i++)
            {
                double a = i * (Math.PI / 6), dx = Math.Cos(a), dy = Math.Sin(a), px = -dy, py = dx, w = 2.2;
                c.Children.Add(new Polygon
                {
                    Points = new PointCollection {
                        new Point(x + r*0.45*dx + px*w, y + r*0.45*dy + py*w),
                        new Point(x + r*dx, y + r*dy),
                        new Point(x + r*0.45*dx - px*w, y + r*0.45*dy - py*w) },
                    Fill = new SolidColorBrush(col)
                });
            }
            c.Children.Add(new Ellipse { Width = r, Height = r, Fill = new SolidColorBrush(Yellow),
                RenderTransform = new TranslateTransform(x - r / 2, y - r / 2) });
            return c;
        }

        // ================= animation =================
        private void StartAnim()
        {
            foreach (var s in _spinners)
            {
                double from = s.Rev ? 360 : 0, to = s.Rev ? 0 : 360;
                s.T.BeginAnimation(RotateTransform.AngleProperty,
                    new DoubleAnimation(from, to, Dur(s.Secs * 1000)) { RepeatBehavior = RepeatBehavior.Forever });
            }
            var pulse = new DoubleAnimation(1.0, 1.14, Dur(1200))
            { AutoReverse = true, RepeatBehavior = RepeatBehavior.Forever,
              EasingFunction = new SineEase { EasingMode = EasingMode.EaseInOut } };
            _centreScale.BeginAnimation(ScaleTransform.ScaleXProperty, pulse);
            _centreScale.BeginAnimation(ScaleTransform.ScaleYProperty, pulse);
            if (_centre != null)
                _centre.BeginAnimation(UIElement.OpacityProperty, new DoubleAnimation(0.9, 1.0, Dur(1200))
                { AutoReverse = true, RepeatBehavior = RepeatBehavior.Forever,
                  EasingFunction = new SineEase { EasingMode = EasingMode.EaseInOut } });
        }

        private void StopAnim()
        {
            foreach (var s in _spinners) s.T.BeginAnimation(RotateTransform.AngleProperty, null);
            _centreScale.BeginAnimation(ScaleTransform.ScaleXProperty, null);
            _centreScale.BeginAnimation(ScaleTransform.ScaleYProperty, null);
        }

        // ================= helpers =================
        private Path DashedRing(double r, double[] dash, double opacity, double secs, bool rev)
        {
            var rot = new RotateTransform { CenterX = _cx, CenterY = _cy };
            var p = new Path
            {
                Data = new EllipseGeometry(new Point(_cx, _cy), r, r),
                Stroke = new SolidColorBrush(Cyan) { Opacity = opacity },
                StrokeThickness = 1, StrokeDashArray = new DoubleCollection(dash), RenderTransform = rot
            };
            _spinners.Add(new RingAnim(rot, secs, rev));
            return p;
        }

        private Path ArcPath(double r, double a0, double a1, Color col, double w, double opacity)
        {
            var p0 = new Point(_cx + r * Math.Cos(a0), _cy + r * Math.Sin(a0));
            var p1 = new Point(_cx + r * Math.Cos(a1), _cy + r * Math.Sin(a1));
            var fig = new PathFigure { StartPoint = p0, IsClosed = false };
            fig.Segments.Add(new ArcSegment(p1, new Size(r, r), 0, (a1 - a0) > Math.PI, SweepDirection.Clockwise, true));
            var geo = new PathGeometry(); geo.Figures.Add(fig);
            return new Path { Data = geo, Stroke = new SolidColorBrush(col) { Opacity = opacity },
                StrokeThickness = w, StrokeStartLineCap = PenLineCap.Round, StrokeEndLineCap = PenLineCap.Round };
        }

        private Ellipse Ring(double r, Color c, double w, double opacity)
        {
            var e = new Ellipse { Width = r * 2, Height = r * 2, Fill = Brushes.Transparent,
                Stroke = new SolidColorBrush(c) { Opacity = opacity }, StrokeThickness = w };
            Place(e, _cx - r, _cy - r);
            return e;
        }

        private RadialGradientBrush Radial(GradientStop[] stops, double ox, double oy)
        {
            var b = new RadialGradientBrush { GradientOrigin = new Point(ox, oy), Center = new Point(ox, oy),
                RadiusX = 0.5, RadiusY = 0.5 };
            foreach (var s in stops) b.GradientStops.Add(s);
            return b;
        }

        private Point Pol(double r, double a) { return new Point(_cx + r * Math.Cos(a), _cy + r * Math.Sin(a)); }
        private static GradientStop Stop(Color c, double off) { return new GradientStop(c, off); }
        private static GradientStop StopA(Color c, double off, byte a)
        { return new GradientStop(Color.FromArgb(a, c.R, c.G, c.B), off); }
        private static void Place(UIElement e, double x, double y) { Canvas.SetLeft(e, x); Canvas.SetTop(e, y); }
        private static Color Hex(uint rgb)
        { return Color.FromRgb((byte)(rgb >> 16), (byte)(rgb >> 8), (byte)rgb); }
        private static Duration Dur(double ms) { return new Duration(TimeSpan.FromMilliseconds(ms)); }
        private static int Clamp(int v, int lo, int hi) { return v < lo ? lo : (v > hi ? hi : v); }
        private static double Norm(double d) { return ((d % 360) + 360) % 360; }
        private static readonly Size Inf = new Size(double.PositiveInfinity, double.PositiveInfinity);
    }
}
