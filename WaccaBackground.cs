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
    /// Vector reproduction of the animated chrome that used to live in background.mp4:
    ///   - rotating neon "rainbow" outer ring  (the lighting sweep)
    ///   - breathing selection glow            (the pulse around the centre jacket)
    ///   - slowly rotating sunburst stamp       (the 決定 badge at the bottom)
    ///   - scrolling marquee text               (song-title ticker)
    ///   - faint radial segment dividers + the 4 directional arrow nubs
    ///
    /// It is a plain FrameworkElement built entirely in code (no XAML / no .csproj
    /// page registration), so it drops into your existing code-built UI the same way
    /// the MediaElement did.  Everything is drawn inside a fixed 1000x1000 design
    /// canvas wrapped in a Viewbox, so it scales Uniform to the window exactly like
    /// the old video did.
    ///
    /// Because it is vector + parameterised, a sub-menu can reuse the SAME instance
    /// and just recolour / relabel it (SetAccent, SetRingTint, SetMarquee) instead of
    /// you having to render a separate mp4 per sub-menu.
    /// </summary>
    public class WaccaBackground : Viewbox
    {
        // ---- design space -------------------------------------------------
        private const double DesignW = 1000;
        private const double DesignH = 1000;
        private double _cx = 500;     // circle centre
        private double _cy = 470;
        private const double R_out = 480;   // outer edge of the neon ring
        private const double R_in = 440;    // inner edge of the neon ring

        // ---- tunables -----------------------------------------------------
        public double RingSpeedSeconds = 6.0;     // full sweep period
        public double SunburstSpeedSeconds = 9.0;  // stamp rotation period
        public double GlowPeriodSeconds = 1.8;     // breathe period
        public int RingSegments = 144;             // smoothness of the ring

        // ---- visual roots we animate / mutate -----------------------------
        private readonly Canvas _root = new Canvas { Width = DesignW, Height = DesignH };
        private readonly Canvas _ringHost = new Canvas();       // holds the wedge paths
        private readonly RotateTransform _ringRot = new RotateTransform();
        private Path _glow;
        private readonly ScaleTransform _glowScale = new ScaleTransform(1, 1);
        private Canvas _sunburst;
        private readonly RotateTransform _sunRot = new RotateTransform();
        private TextBlock _marquee;
        private readonly TranslateTransform _marqueeTr = new TranslateTransform();
        private Canvas _marqueeClip;

        private Color _accent = Color.FromRgb(0x37, 0xE3, 0x6B);   // selection green
        private Color _stamp = Color.FromRgb(0xFF, 0x2D, 0x8F);    // 決定 pink
        private double _ringTint = 1.0;                            // 1 = full rainbow

        public WaccaBackground()
        {
            Stretch = Stretch.Uniform;     // == old MediaElement Stretch.Uniform
            IsHitTestVisible = false;      // let touches/clicks fall through to the wheel
            Child = _root;

            BuildRing();
            BuildSegmentsAndArrows();
            BuildGlow();
            BuildSunburst();
            BuildMarquee();

            Loaded += (s, e) => Start();
        }

        // ===================================================================
        //  build helpers
        // ===================================================================
        private void BuildRing()
        {
            // A static rainbow annulus that we simply spin. Spinning a full
            // rainbow == the colour-sweep you saw in the mp4.
            _ringHost.Width = DesignW;
            _ringHost.Height = DesignH;
            RebuildRingColors();

            _ringRot.CenterX = _cx;
            _ringRot.CenterY = _cy;
            _ringHost.RenderTransform = _ringRot;
            _root.Children.Add(_ringHost);

            // dark inner bezel so the ring reads as a ring
            var bezel = new Ellipse
            {
                Width = R_in * 2,
                Height = R_in * 2,
                Stroke = new SolidColorBrush(Color.FromArgb(0x60, 0x78, 0x8C, 0xC8)),
                StrokeThickness = 3,
                Fill = Brushes.Transparent
            };
            Canvas.SetLeft(bezel, _cx - R_in);
            Canvas.SetTop(bezel, _cy - R_in);
            _root.Children.Add(bezel);
        }

        private void RebuildRingColors()
        {
            _ringHost.Children.Clear();
            double step = 360.0 / RingSegments;
            for (int i = 0; i < RingSegments; i++)
            {
                double a0 = i * step;
                double a1 = a0 + step + 0.6; // tiny overlap to avoid seams
                Color c = HsvToColor(a0, 0.85 * _ringTint, 1.0);
                // desaturate toward white when tint < 1 (used by sub-menus)
                if (_ringTint < 1.0)
                    c = Lerp(Color.FromRgb(0x9f, 0xb0, 0xe8), c, _ringTint);

                _ringHost.Children.Add(WedgePath(_cx, _cy, R_in, R_out, a0, a1, c));
            }
        }

        private static Path WedgePath(double cx, double cy, double rIn, double rOut,
                                      double aDeg0, double aDeg1, Color color)
        {
            double a0 = aDeg0 * Math.PI / 180.0 - Math.PI / 2; // 0deg = top
            double a1 = aDeg1 * Math.PI / 180.0 - Math.PI / 2;
            Point oA = new Point(cx + rOut * Math.Cos(a0), cy + rOut * Math.Sin(a0));
            Point oB = new Point(cx + rOut * Math.Cos(a1), cy + rOut * Math.Sin(a1));
            Point iB = new Point(cx + rIn * Math.Cos(a1), cy + rIn * Math.Sin(a1));
            Point iA = new Point(cx + rIn * Math.Cos(a0), cy + rIn * Math.Sin(a0));

            var fig = new PathFigure { StartPoint = oA, IsClosed = true };
            fig.Segments.Add(new ArcSegment(oB, new Size(rOut, rOut), 0, false, SweepDirection.Clockwise, true));
            fig.Segments.Add(new LineSegment(iB, true));
            fig.Segments.Add(new ArcSegment(iA, new Size(rIn, rIn), 0, false, SweepDirection.Counterclockwise, true));
            var geo = new PathGeometry();
            geo.Figures.Add(fig);
            return new Path { Data = geo, Fill = new SolidColorBrush(color) };
        }

        private void BuildSegmentsAndArrows()
        {
            // 12 faint touch-zone dividers (matches Wacca's "top = 12, clockwise")
            var div = new SolidColorBrush(Color.FromArgb(0x2E, 0x96, 0xAA, 0xE6));
            for (int i = 0; i < 12; i++)
            {
                double a = i * 30.0 * Math.PI / 180.0 - Math.PI / 2;
                double r0 = 250, r1 = R_in - 4;
                _root.Children.Add(new Line
                {
                    X1 = _cx + r0 * Math.Cos(a),
                    Y1 = _cy + r0 * Math.Sin(a),
                    X2 = _cx + r1 * Math.Cos(a),
                    Y2 = _cy + r1 * Math.Sin(a),
                    Stroke = div,
                    StrokeThickness = 1
                });
            }

            // 4 directional arrow nubs (N/E/S/W). These are the things the radial
            // sub-menu replaces, so keep them on their own canvas you can hide.
            ArrowNub(0);   // up
            ArrowNub(90);  // right
            ArrowNub(180); // down
            ArrowNub(270); // left
        }

        private void ArrowNub(double aDeg)
        {
            double a = aDeg * Math.PI / 180.0 - Math.PI / 2;
            double r = R_in - 24;
            double bx = _cx + r * Math.Cos(a);
            double by = _cy + r * Math.Sin(a);
            // perpendicular for the base
            double pa = a + Math.PI / 2;
            double half = 18, depth = 26;
            var tip = new Point(_cx + (r + depth) * Math.Cos(a), _cy + (r + depth) * Math.Sin(a));
            var l = new Point(bx + half * Math.Cos(pa), by + half * Math.Sin(pa));
            var rr = new Point(bx - half * Math.Cos(pa), by - half * Math.Sin(pa));
            _root.Children.Add(new Polygon
            {
                Points = new PointCollection { tip, l, rr },
                Fill = new SolidColorBrush(Color.FromRgb(0xCF, 0xD8, 0xF5))
            });
        }

        private void BuildGlow()
        {
            // Soft breathing ring around the selected (centre) item.
            double rg = 150;
            var rg2 = new RadialGradientBrush
            {
                GradientOrigin = new Point(0.5, 0.5),
                Center = new Point(0.5, 0.5),
                RadiusX = 0.5,
                RadiusY = 0.5
            };
            rg2.GradientStops.Add(new GradientStop(Color.FromArgb(0x00, _accent.R, _accent.G, _accent.B), 0.62));
            rg2.GradientStops.Add(new GradientStop(Color.FromArgb(0xC8, _accent.R, _accent.G, _accent.B), 0.80));
            rg2.GradientStops.Add(new GradientStop(Color.FromArgb(0x00, _accent.R, _accent.G, _accent.B), 1.0));

            _glow = new Path
            {
                Data = new EllipseGeometry(new Point(0, 0), rg, rg),
                Fill = rg2,
                RenderTransformOrigin = new Point(0.5, 0.5)
            };
            Canvas.SetLeft(_glow, _cx);
            Canvas.SetTop(_glow, _cy);
            _glow.RenderTransform = _glowScale;
            _root.Children.Add(_glow);
        }

        private void BuildSunburst()
        {
            _sunburst = new Canvas();
            int spikes = 12;
            double rIn = 16, rOut = 40, w = 7;
            var fill = new SolidColorBrush(_stamp);
            for (int i = 0; i < spikes; i++)
            {
                double a = i * (360.0 / spikes) * Math.PI / 180.0;
                double dx = Math.Cos(a), dy = Math.Sin(a);
                double px = -dy, py = dx; // perpendicular
                var p1 = new Point(rIn * dx + px * w, rIn * dy + py * w);
                var p2 = new Point(rOut * dx, rOut * dy);
                var p3 = new Point(rIn * dx - px * w, rIn * dy - py * w);
                _sunburst.Children.Add(new Polygon
                {
                    Points = new PointCollection { p1, p2, p3 },
                    Fill = fill
                });
            }
            _sunburst.Children.Add(new Ellipse
            {
                Width = 28,
                Height = 28,
                Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xE0, 0x00)),
                RenderTransform = new TranslateTransform(-14, -14)
            });
            double sx = _cx, sy = _cy + 290;     // bottom-centre, inside the ring
            Canvas.SetLeft(_sunburst, sx);
            Canvas.SetTop(_sunburst, sy);
            _sunburst.RenderTransform = _sunRot;
            _root.Children.Add(_sunburst);
        }

        private void BuildMarquee()
        {
            _marqueeClip = new Canvas
            {
                Width = 760,
                Height = 30,
                ClipToBounds = true
            };
            Canvas.SetLeft(_marqueeClip, _cx - 380);
            Canvas.SetTop(_marqueeClip, _cy + 110);

            _marquee = new TextBlock
            {
                Text = "",
                FontSize = 22,
                FontWeight = FontWeights.Medium,
                Foreground = new SolidColorBrush(Color.FromRgb(0x9f, 0xb0, 0xe8)),
                RenderTransform = _marqueeTr
            };
            _marqueeClip.Children.Add(_marquee);
            _root.Children.Add(_marqueeClip);
        }

        // ===================================================================
        //  animation control
        // ===================================================================
        public void Start()
        {
            // ring sweep
            BeginLoop(_ringRot, RotateTransform.AngleProperty, 0, 360, RingSpeedSeconds);
            // sunburst
            BeginLoop(_sunRot, RotateTransform.AngleProperty, 0, 360, SunburstSpeedSeconds);
            // breathing glow (scale + opacity)
            BeginBreathe();
            // marquee (needs measured width)
            Dispatcher.BeginInvoke(new Action(StartMarquee), System.Windows.Threading.DispatcherPriority.Loaded);
        }

        public void Stop()
        {
            _ringRot.BeginAnimation(RotateTransform.AngleProperty, null);
            _sunRot.BeginAnimation(RotateTransform.AngleProperty, null);
            _glowScale.BeginAnimation(ScaleTransform.ScaleXProperty, null);
            _glowScale.BeginAnimation(ScaleTransform.ScaleYProperty, null);
            _glow.BeginAnimation(UIElement.OpacityProperty, null);
            _marqueeTr.BeginAnimation(TranslateTransform.XProperty, null);
        }

        private static void BeginLoop(Animatable target, DependencyProperty prop,
                                      double from, double to, double seconds)
        {
            var a = new DoubleAnimation(from, to, new Duration(TimeSpan.FromSeconds(seconds)))
            {
                RepeatBehavior = RepeatBehavior.Forever
            };
            target.BeginAnimation(prop, a);
        }

        private void BeginBreathe()
        {
            var sc = new DoubleAnimation(1.0, 1.12, new Duration(TimeSpan.FromSeconds(GlowPeriodSeconds / 2)))
            {
                AutoReverse = true,
                RepeatBehavior = RepeatBehavior.Forever,
                EasingFunction = new SineEase { EasingMode = EasingMode.EaseInOut }
            };
            _glowScale.BeginAnimation(ScaleTransform.ScaleXProperty, sc);
            _glowScale.BeginAnimation(ScaleTransform.ScaleYProperty, sc);
            var op = new DoubleAnimation(0.85, 1.0, new Duration(TimeSpan.FromSeconds(GlowPeriodSeconds / 2)))
            {
                AutoReverse = true,
                RepeatBehavior = RepeatBehavior.Forever,
                EasingFunction = new SineEase { EasingMode = EasingMode.EaseInOut }
            };
            _glow.BeginAnimation(UIElement.OpacityProperty, op);
        }

        private void StartMarquee()
        {
            _marquee.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            double w = _marquee.DesiredSize.Width;
            if (w < 1) return;
            var a = new DoubleAnimation(_marqueeClip.Width, -w,
                new Duration(TimeSpan.FromSeconds(Math.Max(4, w / 90.0))))
            {
                RepeatBehavior = RepeatBehavior.Forever
            };
            _marqueeTr.BeginAnimation(TranslateTransform.XProperty, a);
        }

        // ===================================================================
        //  public knobs a sub-menu can use to re-skin the SAME background
        // ===================================================================
        public void SetMarquee(string text)
        {
            _marquee.Text = (text ?? "") + "        ";   // trailing pad before loop
            StartMarquee();
        }

        /// <summary>Recolour the selection glow + (optionally) the stamp.</summary>
        public void SetAccent(Color accent)
        {
            _accent = accent;
            _root.Children.Remove(_glow);
            BuildGlow();
            BeginBreathe();
        }

        /// <summary>0 = ring fades to neutral blue, 1 = full rainbow. Lets a
        /// "Lighting" sub-menu dim/desaturate the ring live without a new video.</summary>
        public void SetRingTint(double tint01)
        {
            _ringTint = Math.Max(0, Math.Min(1, tint01));
            RebuildRingColors();
        }

        public void SetRingSpeed(double seconds)
        {
            RingSpeedSeconds = Math.Max(0.5, seconds);
            BeginLoop(_ringRot, RotateTransform.AngleProperty, 0, 360, RingSpeedSeconds);
        }

        // ===================================================================
        //  small colour helpers
        // ===================================================================
        private static Color HsvToColor(double hDeg, double s, double v)
        {
            double h = (hDeg % 360 + 360) % 360 / 60.0;
            int i = (int)Math.Floor(h);
            double f = h - i;
            double p = v * (1 - s);
            double q = v * (1 - s * f);
            double t = v * (1 - s * (1 - f));
            double r, g, b;
            switch (i % 6)
            {
                case 0: r = v; g = t; b = p; break;
                case 1: r = q; g = v; b = p; break;
                case 2: r = p; g = v; b = t; break;
                case 3: r = p; g = q; b = v; break;
                case 4: r = t; g = p; b = v; break;
                default: r = v; g = p; b = q; break;
            }
            return Color.FromRgb((byte)(r * 255), (byte)(g * 255), (byte)(b * 255));
        }

        private static Color Lerp(Color a, Color b, double t)
        {
            return Color.FromRgb(
                (byte)(a.R + (b.R - a.R) * t),
                (byte)(a.G + (b.G - a.G) * t),
                (byte)(a.B + (b.B - a.B) * t));
        }
    }
}
