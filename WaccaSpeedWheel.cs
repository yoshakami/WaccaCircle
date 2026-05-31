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
    /// WaccaSpeedWheel — a polished, fully animated vector recreation of the in-game
    /// circular sub-menu (the "réglage de la vitesse" screen and friends).
    ///
    /// One reusable control for every sub-menu. Call Configure(title, description,
    /// items, selectedIndex) and Show(); the same control serves the speed wheel,
    /// the category picker, the difficulty picker, the favourite/sort picker — only
    /// the items and labels change, never a second mp4 or a second control.
    ///
    /// It is built entirely in code (no XAML, matching the rest of the project) and
    /// wrapped in a Viewbox so it scales Uniform to whatever it is parented into.
    /// Add it to the launcher's Grid (NOT a bare Canvas) so it fills and centres.
    ///
    /// Layers, bottom to top:
    ///   1. backdrop disc          - masks the song wheel underneath while open
    ///   2. segmented glitch ring  - 48 coloured wedges + dark streaks (the outer band)
    ///   3. value lanes            - rounded pills with upright radial text (dynamic)
    ///   4. yellow banner + SET tab - the selected value at 9 o'clock
    ///   5. tunnel core            - well gradient, radar ticks, 4 dashed rings spinning
    ///                               at different rates, pink spiral arcs, pulsing violet
    ///                               centre, pulsing magenta halo + thick rim
    ///   6. frame                  - title, description, Retour / défaut / OK / arrows
    ///
    /// API matches the earlier WaccaWheelMenu so wiring in INTEGRATION.md is unchanged:
    ///   Configure / Step / HitTestAngle / Show / Hide / RaiseConfirmed
    ///   events: Confirmed(index) / Cancelled / DefaultRequested
    ///
    /// Colours are sampled from the real screen; this is an original vector rendering
    /// (no game art is bundled), so it ships safely in a public repo.
    /// </summary>
    public class WaccaSpeedWheel : Viewbox
    {
        // ---- design space (560 matches the proven layout; vectors scale crisply) ----
        private const double D = 560;
        private double _cx = 280, _cy = 280;

        private const double RingOut = 276, RingIn = 212;     // outer glitch band
        private const double LaneIn = 128, LaneOut = 205, LaneH = 15;
        private const double CoreR = 118;                     // magenta rim radius
        private const double Inner = CoreR - 9;               // tunnel content radius
        private const double LaneStepTight = 12.6;            // deg between lanes (long lists)
        private const int LanesEachSide = 9;

        // ---- exact palette (sampled from the screen) ----
        private static readonly Color Magenta   = Hex(0xFF1E8C);
        private static readonly Color MagentaHi = Hex(0xFF3DA0);
        private static readonly Color Cyan      = Hex(0x23C4E0);
        private static readonly Color SetPink   = Hex(0xF63AF0);
        private static readonly Color TitlePink = Hex(0xFF5AB0);
        private static readonly Color DescBlue  = Hex(0xCFD6FF);
        private static readonly Color Yellow    = Hex(0xFFEE00);
        private static readonly Color Ink       = Hex(0x1A1030);
        private static readonly Color White     = Hex(0xFFFEF8);
        private static readonly Color LaneCyan  = Hex(0x51C8E0);
        private static readonly Color LaneBlue1 = Hex(0x00A8E9);
        private static readonly Color LaneBlue2 = Hex(0x2B56E0);
        private static readonly Color LaneMag   = Hex(0xFE0176);
        private static readonly Color[] LaneCols =
            { White, LaneCyan, LaneBlue1, White, LaneCyan, LaneMag, White, LaneBlue2 };
        private static readonly uint[] RingCols =
            { 0xFF06FE, 0x5B10AE, 0x00A8E9, 0x2B56E0, 0xFF2D8F, 0x7A1FD0,
              0x1FC3EA, 0x3A0F7A, 0xFF06FE, 0x9B2FD6, 0x00A8E9, 0xC01F9A };

        // ---- config ----
        private string _title = "", _description = "";
        private List<string> _items = new List<string>();
        private int _selected = 0;

        // ---- layers ----
        private readonly Canvas _root  = new Canvas { Width = D, Height = D };
        private readonly Canvas _lanes = new Canvas { Width = D, Height = D };
        private readonly Canvas _frame = new Canvas { Width = D, Height = D };
        private TextBlock _titleTb, _descTb, _bannerTb;

        // animated transforms (collected so Start/Stop can drive them)
        private readonly List<RingAnim> _spinners = new List<RingAnim>();
        private readonly ScaleTransform _haloScale = new ScaleTransform(1, 1);
        private readonly ScaleTransform _centreScale = new ScaleTransform(1, 1);
        private UIElement _halo, _centre;

        public event Action<int> Confirmed;
        public event Action Cancelled;
        public event Action DefaultRequested;

        private struct RingAnim
        {
            public RotateTransform T; public double Secs; public bool Rev;
            public RingAnim(RotateTransform t, double s, bool r) { T = t; Secs = s; Rev = r; }
        }

        public WaccaSpeedWheel()
        {
            Stretch = Stretch.Uniform;
            Child = _root;
            Opacity = 0;
            Visibility = Visibility.Collapsed;

            BuildBackdrop();
            BuildRing();
            _root.Children.Add(_lanes);
            BuildCore();
            _root.Children.Add(_frame);
            BuildFrame();
        }

        // ===================================================================
        //  public API
        // ===================================================================
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
        { get { return (_selected >= 0 && _selected < _items.Count) ? _items[_selected] : null; } }

        public void Step(int delta)
        {
            if (_items.Count == 0) return;
            _selected = Clamp(_selected + delta, 0, _items.Count - 1);
            RebuildLanes();
        }

        /// <summary>Map a touch angle (deg, 0 = up, clockwise) to the lane under it.</summary>
        public int HitTestAngle(double deg)
        {
            double internalDeg = Norm(deg + 270);
            Step((int)Math.Round((180 - internalDeg) / LaneStepTight));
            return _selected;
        }

        public void RaiseConfirmed() { if (Confirmed != null) Confirmed(_selected); }
        public void FireCancel()     { if (Cancelled != null) Cancelled(); }
        public void FireDefault()    { if (DefaultRequested != null) DefaultRequested(); }

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

        // ===================================================================
        //  layer 1 — backdrop
        // ===================================================================
        private void BuildBackdrop()
        {
            var rg = Radial(new[] {
                Stop(Hex(0x2A1D5E), 0.0), Stop(Hex(0x160F38), 0.55), Stop(Hex(0x080518), 1.0) },
                0.5, 0.5);
            var disc = new Ellipse { Width = RingOut * 2, Height = RingOut * 2, Fill = rg };
            Place(disc, _cx - RingOut, _cy - RingOut);
            _root.Children.Add(disc);
        }

        // ===================================================================
        //  layer 2 — segmented glitch ring
        // ===================================================================
        private void BuildRing()
        {
            int n = 48;
            for (int i = 0; i < n; i++)
            {
                double a0 = (i / (double)n) * 2 * Math.PI;
                double a1 = ((i + 1) / (double)n) * 2 * Math.PI + 0.004;
                var c = Hex(RingCols[i % RingCols.Length]);
                _root.Children.Add(new Path
                {
                    Data = Wedge(_cx, _cy, RingIn, RingOut, a0, a1),
                    Fill = new SolidColorBrush(c) { Opacity = (i % 5 == 0) ? 0.55 : 0.92 }
                });
            }
            // deterministic dark streaks (no RNG so it looks identical every run)
            double[] streaks = { 0.3, 1.1, 1.9, 2.6, 3.4, 4.2, 5.0, 5.7 };
            foreach (var a in streaks)
                _root.Children.Add(new Path
                {
                    Data = Wedge(_cx, _cy, RingIn, RingOut, a, a + 0.05),
                    Fill = new SolidColorBrush(Hex(0x0A0618)) { Opacity = 0.4 }
                });
            _root.Children.Add(Ring(RingIn, Hex(0x0A0618), 3, 1));
            _root.Children.Add(Ring(RingOut - 1, Hex(0x1A0F3A), 2, 1));
        }

        // ===================================================================
        //  layer 5 — tunnel core (built here; some parts animate)
        // ===================================================================
        private void BuildCore()
        {
            var core = new Canvas();

            // the well
            var well = Radial(new[] {
                Stop(Hex(0x8A6CFF), 0.0), Stop(Hex(0x4A2F9A), 0.26),
                Stop(Hex(0x221751), 0.60), Stop(Hex(0x070416), 1.0) }, 0.48, 0.46);
            var disc = new Ellipse { Width = Inner * 2, Height = Inner * 2, Fill = well };
            Place(disc, _cx - Inner, _cy - Inner);
            core.Children.Add(disc);

            // radar ticks
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

            // concentric dashed rings (animated)
            core.Children.Add(DashedRing(Inner * 0.90, new double[] { 5, 8 }, 0.5, 30, false));
            core.Children.Add(DashedRing(Inner * 0.72, new double[] { 3, 11 }, 0.4, 22, true));
            core.Children.Add(DashedRing(Inner * 0.54, new double[] { 2, 7 }, 0.45, 16, false));
            core.Children.Add(DashedRing(Inner * 0.38, new double[] { 2, 12 }, 0.35, 12, true));

            // pink spiral arcs on a rotating host (pivot at the wheel centre)
            var arcRot = new RotateTransform { CenterX = _cx, CenterY = _cy };
            var arcs = new Canvas { RenderTransform = arcRot };
            arcs.Children.Add(ArcPath(Inner * 0.88, Math.PI * 0.95, Math.PI * 1.70, MagentaHi, 3, 0.92));
            arcs.Children.Add(ArcPath(Inner * 0.66, Math.PI * 0.05, Math.PI * 0.50, Hex(0xFF6AB8), 2, 0.8));
            arcs.Children.Add(ArcPath(Inner * 0.50, Math.PI * 1.20, Math.PI * 1.55, Cyan, 1.5, 0.6));
            core.Children.Add(arcs);
            _spinners.Add(new RingAnim(arcRot, 7, false));

            // pulsing violet centre
            var ctr = Radial(new[] {
                Stop(Hex(0xDFE9FF), 0.0), StopA(Hex(0x9A7BFF), 0.45, 0xCC), StopA(Hex(0x5A32C0), 1.0, 0x00) }, 0.5, 0.5);
            double cr = 30;
            var centre = new Ellipse { Width = cr * 2, Height = cr * 2, Fill = ctr,
                RenderTransformOrigin = new Point(0.5, 0.5), RenderTransform = _centreScale };
            Place(centre, _cx - cr, _cy - cr);
            core.Children.Add(centre); _centre = centre;

            // pulsing magenta halo (soft)
            var haloBrush = Radial(new[] {
                StopA(Magenta, 0.62, 0x00), StopA(Magenta, 0.82, 0x80), StopA(Magenta, 1.0, 0x00) }, 0.5, 0.5);
            double hr = CoreR + 14;
            var halo = new Path
            {
                Data = new EllipseGeometry(new Point(0, 0), hr, hr), Fill = haloBrush,
                RenderTransformOrigin = new Point(0.5, 0.5), RenderTransform = _haloScale
            };
            Canvas.SetLeft(halo, _cx); Canvas.SetTop(halo, _cy);
            core.Children.Add(halo); _halo = halo;

            // inner cyan edge + thick magenta rim
            core.Children.Add(Ring(Inner + 1, Cyan, 1.5, 0.6));
            core.Children.Add(Ring(CoreR, Magenta, 11, 1));

            _root.Children.Add(core);
        }

        // ===================================================================
        //  layer 3+4 — value lanes + banner + SET (rebuilt on Step)
        // ===================================================================
        private void RebuildLanes()
        {
            _lanes.Children.Clear();
            if (_items.Count == 0) return;

            double step = LaneStepTight;
            if (_items.Count > 1)
                step = Math.Max(LaneStepTight, Math.Min(26.0, 200.0 / (_items.Count - 1)));

            int span = Math.Max(LanesEachSide, _items.Count);
            for (int k = -span; k <= span; k++)
            {
                int idx = _selected + k;
                if (idx < 0 || idx >= _items.Count || k == 0) continue;
                AddLane(180 + k * step, _items[idx],
                    LaneCols[((idx % LaneCols.Length) + LaneCols.Length) % LaneCols.Length]);
            }
            AddBanner(_items[_selected]);
        }

        private void AddLane(double deg, string label, Color col)
        {
            var g = new Canvas { RenderTransform = new RotateTransform(deg, _cx, _cy) };
            var pill = new Rectangle
            {
                Width = LaneOut - LaneIn, Height = LaneH, RadiusX = LaneH / 2, RadiusY = LaneH / 2,
                Fill = new SolidColorBrush(col)
            };
            Place(pill, _cx + LaneIn, _cy - LaneH / 2);
            g.Children.Add(pill);

            double mid = _cx + (LaneIn + LaneOut) / 2;
            var t = new TextBlock { Text = label, FontSize = 11, FontWeight = FontWeights.SemiBold,
                Foreground = new SolidColorBrush(Ink) };
            t.Measure(Inf); double tw = t.DesiredSize.Width, th = t.DesiredSize.Height;
            Place(t, mid - tw / 2, _cy - th / 2);
            double nd = Norm(deg);
            if (nd > 90 && nd < 270) t.RenderTransform = new RotateTransform(180, tw / 2, th / 2);
            g.Children.Add(t);
            _lanes.Children.Add(g);
        }

        private void AddBanner(string label)
        {
            double bw = _cx - LaneIn + 2, bh = 40;
            var banner = new Rectangle { Width = bw, Height = bh, RadiusX = 4, RadiusY = 4,
                Fill = new SolidColorBrush(Yellow) };
            Place(banner, 0, _cy - bh / 2);
            _lanes.Children.Add(banner);

            _bannerTb = new TextBlock { Text = label, FontSize = 30, FontWeight = FontWeights.Bold,
                Foreground = new SolidColorBrush(Ink) };
            _bannerTb.Measure(Inf);
            Place(_bannerTb, bw * 0.40 - _bannerTb.DesiredSize.Width / 2, _cy - _bannerTb.DesiredSize.Height / 2);
            _lanes.Children.Add(_bannerTb);

            double sx = _cx - LaneIn;
            _lanes.Children.Add(new Polygon
            {
                Points = new PointCollection {
                    new Point(sx - 46, _cy - 18), new Point(sx, _cy - 18),
                    new Point(sx, _cy + 18), new Point(sx - 46, _cy + 18), new Point(sx - 58, _cy) },
                Fill = new SolidColorBrush(SetPink)
            });
            var st = new TextBlock { Text = "SET", FontSize = 14, FontWeight = FontWeights.Medium,
                Foreground = new SolidColorBrush(White) };
            st.Measure(Inf);
            Place(st, sx - 40, _cy - st.DesiredSize.Height / 2);
            _lanes.Children.Add(st);
        }

        // ===================================================================
        //  layer 6 — frame: title / description / buttons
        // ===================================================================
        private void BuildFrame()
        {
            _titleTb = new TextBlock { Text = _title, FontSize = 17, FontWeight = FontWeights.SemiBold,
                Foreground = new SolidColorBrush(TitlePink), TextAlignment = TextAlignment.Center, Width = 300 };
            Place(_titleTb, _cx - 132, 18);
            _frame.Children.Add(_titleTb);

            _descTb = new TextBlock { Text = _description, FontSize = 10.5, Foreground = new SolidColorBrush(DescBlue),
                TextWrapping = TextWrapping.Wrap, Width = 150 };
            Place(_descTb, _cx + 28, 56);
            _frame.Children.Add(_descTb);

            Pill("Retour", TitlePink, 40, D - 56, () => FireCancel());
            Pill("défaut", DescBlue, D - 96, _cy - 10, () => FireDefault());

            // OK + sunburst at the bottom centre
            var ok = new TextBlock { Text = "OK", FontSize = 16, FontWeight = FontWeights.Medium,
                Foreground = new SolidColorBrush(TitlePink) };
            Place(ok, _cx + 12, D - 44);
            ok.MouseLeftButtonUp += (s, e) => RaiseConfirmed();
            _frame.Children.Add(ok);
            var stamp = Sunburst(_cx - 6, D - 34, 13, Magenta);
            stamp.MouseLeftButtonUp += (s, e) => RaiseConfirmed();
            _frame.Children.Add(stamp);

            ArrowBtn(_cx - 96, D - 52, false, () => Step(-1));
            ArrowBtn(_cx + 96, D - 52, true, () => Step(1));
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

        // ===================================================================
        //  animation
        // ===================================================================
        private void StartAnim()
        {
            foreach (var s in _spinners)
            {
                double from = s.Rev ? 360 : 0, to = s.Rev ? 0 : 360;
                s.T.BeginAnimation(RotateTransform.AngleProperty,
                    new DoubleAnimation(from, to, Dur(s.Secs * 1000)) { RepeatBehavior = RepeatBehavior.Forever });
            }
            Breathe(_haloScale, 1.0, 1.14, 1.9);
            Breathe(_centreScale, 1.0, 1.14, 2.4);
            if (_halo != null) Fade(_halo, 0.9, 1.0, 1.9);
            if (_centre != null) Fade(_centre, 0.9, 1.0, 2.4);
        }

        private void StopAnim()
        {
            foreach (var s in _spinners) s.T.BeginAnimation(RotateTransform.AngleProperty, null);
            _haloScale.BeginAnimation(ScaleTransform.ScaleXProperty, null);
            _haloScale.BeginAnimation(ScaleTransform.ScaleYProperty, null);
            _centreScale.BeginAnimation(ScaleTransform.ScaleXProperty, null);
            _centreScale.BeginAnimation(ScaleTransform.ScaleYProperty, null);
        }

        private static void Breathe(ScaleTransform t, double from, double to, double period)
        {
            var a = new DoubleAnimation(from, to, Dur(period * 500))
            { AutoReverse = true, RepeatBehavior = RepeatBehavior.Forever,
              EasingFunction = new SineEase { EasingMode = EasingMode.EaseInOut } };
            t.BeginAnimation(ScaleTransform.ScaleXProperty, a);
            t.BeginAnimation(ScaleTransform.ScaleYProperty, a);
        }

        private static void Fade(UIElement e, double from, double to, double period)
        {
            e.BeginAnimation(UIElement.OpacityProperty, new DoubleAnimation(from, to, Dur(period * 500))
            { AutoReverse = true, RepeatBehavior = RepeatBehavior.Forever,
              EasingFunction = new SineEase { EasingMode = EasingMode.EaseInOut } });
        }

        // ===================================================================
        //  geometry + brush helpers
        // ===================================================================
        private Path DashedRing(double r, double[] dash, double opacity, double secs, bool rev)
        {
            var rot = new RotateTransform { CenterX = _cx, CenterY = _cy };
            var p = new Path
            {
                Data = new EllipseGeometry(new Point(_cx, _cy), r, r),
                Stroke = new SolidColorBrush(Cyan) { Opacity = opacity },
                StrokeThickness = 1, StrokeDashArray = new DoubleCollection(dash),
                RenderTransform = rot
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

        private PathGeometry Wedge(double cx, double cy, double r0, double r1, double a0, double a1)
        {
            Point oA = new Point(cx + r1 * Math.Cos(a0), cy + r1 * Math.Sin(a0));
            Point oB = new Point(cx + r1 * Math.Cos(a1), cy + r1 * Math.Sin(a1));
            Point iB = new Point(cx + r0 * Math.Cos(a1), cy + r0 * Math.Sin(a1));
            Point iA = new Point(cx + r0 * Math.Cos(a0), cy + r0 * Math.Sin(a0));
            bool big = (a1 - a0) > Math.PI;
            var fig = new PathFigure { StartPoint = oA, IsClosed = true };
            fig.Segments.Add(new ArcSegment(oB, new Size(r1, r1), 0, big, SweepDirection.Clockwise, true));
            fig.Segments.Add(new LineSegment(iB, true));
            fig.Segments.Add(new ArcSegment(iA, new Size(r0, r0), 0, big, SweepDirection.Counterclockwise, true));
            var geo = new PathGeometry(); geo.Figures.Add(fig);
            return geo;
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
