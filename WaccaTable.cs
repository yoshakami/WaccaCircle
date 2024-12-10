
using LilyConsole;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net.NetworkInformation;

namespace WaccaCircle
{
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
    public class ColorData
    {
        public byte animIndex { get; set; }
        public double animBreatheSpeedStepBetween0And1 { get; set; }
        public double animColorSpeedStepBetween0And360 { get; set; }
        public double animColorJumpStepBetween0And360 { get; set; }
        public uint animColorJumpDelayCount { get; set; }
        public byte animColorWaccaSpeedBetween0And60 { get; set; }
        public Color Color1 { get; set; }
        public Color Color2 { get; set; }
        public Color Color3 { get; set; }
        public Color Color4 { get; set; }
        public Color Color5 { get; set; }
        public Color Color6 { get; set; }
        public Color Color7 { get; set; }
        public Color Color8 { get; set; }
        public Color Color9 { get; set; }
        public Color Color10 { get; set; }
        public Color Color11 { get; set; }
        public Color Color12 { get; set; }
        public Color LoveLiveBack12 { get; set; }
        public Color LoveLiveCoin0 { get; set; }
        public Color LoveLive1Left { get; set; }
        public Color LoveLive2 { get; set; }
        public Color LoveLive3 { get; set; }
        public Color LoveLive4 { get; set; }
        public Color LoveLive5Bottom { get; set; }
        public Color LoveLive6 { get; set; }
        public Color LoveLive7 { get; set; }
        public Color LoveLive8 { get; set; }
        public Color LoveLive9Right { get; set; }
        public Color LoveLiveStart10 { get; set; }
        public Color LoveLiveP11 { get; set; }

        public Color OuterCircleColor { get; set; }
        public Color SDVXouterL { get; set; }
        public Color SDVXouterR { get; set; }
        public Color SDVX1 { get; set; }
        public Color SDVX2 { get; set; }
        public Color SDVX3 { get; set; }
        public Color SDVX4rightFX { get; set; }
        public Color SDVX5rightLane { get; set; }
        public Color SDVX6middleRightLane { get; set; }
        public Color SDVX7middleLeftLane { get; set; }
        public Color SDVX8leftLane { get; set; }
        public Color SDVX9leftFX { get; set; }
        public Color SDVX10 { get; set; }
        public Color SDVX11 { get; set; }
        public Color SDVX12 { get; set; }
        public Color SDVX13top { get; set; }
        public Color Osu1topRight { get; set; }
        public Color Osu2 { get; set; }
        public Color Osu3 { get; set; }
        public Color Osu4BottomRight { get; set; }
        public Color Osu5BottomLeft { get; set; }
        public Color Osu6 { get; set; }
        public Color Osu7 { get; set; }
        public Color Osu8topLeft { get; set; }
        public Color MouseUp { get; set; }
        public Color MouseDown { get; set; }
        public Color MouseLeft { get; set; }
        public Color MouseRight { get; set; }
        public Color MouseOuter { get; set; }
        public Color TaikoOuterL;
        public Color TaikoInnerL;
        public Color TaikoInnerR;
        public Color TaikoOuterR;
        public Color rpgBack;
        public Color rpgEnter;
        public Color rpgMenu;
        public Color rpgAttacc;
        public Color rpgOuterUp;
        public Color rpgOuterDown;
        public Color rpgOuterLeft;
        public Color rpgOuterRight;
    }
    public static class ColorStorage
    {
        public static byte animIndex = 0;
        public static double animBreatheSpeedStepBetween0And1 = 0.05;
        public static double animColorSpeedStepBetween0And360 = 2.0;
        public static double animColorJumpStepBetween0And360 = 15.0;
        public static uint animColorJumpDelayCount = 10;
        public static byte animColorWaccaSpeedBetween0And60 = 1;
        public static readonly Color[] ColorsHSV12 =
        {
            new Color(0, 1, 1),       // Red
            new Color(30, 0.8, 1),      // Orange
            new Color(60, 1, 1),      // Yellow
            new Color(90, 0.8, 1),      // Light Green
            new Color(120, 1, 1),     // Green
            new Color(150, 0.8, 1),     // Cyan-Green
            new Color(180, 1, 1),     // Cyan
            new Color(210, 0.8, 1),     // Blue-Cyan
            new Color(240, 1, 1),     // Blue
            new Color(270, 0.8, 1),     // Purple
            new Color(300, 1, 1),     // Magenta
            new Color(330, 0.8, 1),     // Pink
        };

        public static readonly Color[] LoveLiveColorsHSV =
        {
            new Color(60, 1, 1),       // Yellow coin
            new Color(30, 1, 1),      // Orange
            new Color(330, 1, 1),     // Pink
            new Color(30, 1, 1),      // Orange
            new Color(330, 1, 1),     // Pink
            new Color(30, 1, 1),      // Orange
            new Color(330, 1, 1),     // Pink
            new Color(30, 1, 1),      // Orange
            new Color(330, 1, 1),     // Pink
            new Color(30, 1, 1),      // Orange
            new Color(190, 1, 1),     // Cyan start
            new Color(270, 0.8, 1),     // Purple P
            new Color(350, 0.8, 1),     // Red Back
        };

        public static Color outer = new Color(0, 0, 1);

        public static Color SDVXouterL = new Color(200, 0.8, 1);     // Blue 
        public static Color SDVXouterR = new Color(330, 1, 1);     // Pink 

        public static readonly Color[] SDVXColorsHSV =
        {
            new Color(0, 1, 1),       // Red
            new Color(60, 1, 1),      // Yellow
            new Color(120, 1, 1),     // Green
            new Color(30, 1, 1),      // Orange
            new Color(0, 0, 0.8),      // White
            new Color(0, 0, 0.6),      // Light Grey
            new Color(0, 0, 0.4),      // Dark Grey
            new Color(0, 0, 0.8),      // White
            new Color(30, 1, 1),      // Orange
            new Color(120, 1, 1),     // Green
            new Color(60, 1, 1),      // Yellow
            new Color(0, 1, 1),       // Red
            new Color(320, 1, 1),     // Pink
        };


        public static readonly Color[] OsuColorsHSV =
        {
            new Color(00, 1, 1),      // Red
            new Color(50, 1, 1),      // Orange
            new Color(80, 1, 1),      // Yellow
            new Color(120, 1, 1),     // Green
            new Color(180, 1, 1),     // Cyan
            new Color(265, 1, 1),     // Purple
            new Color(300, 1, 1),     // Magenta
            new Color(330, 1, 1),     // Pink
        };


        public static Color[] mouseHSV = {
            new Color(0, 1, 1),       // Up
            new Color(60, 1, 1),      // Right
            new Color(120, 1, 1),     // Down
            new Color(240, 1, 1),     // Left
            new Color(0, 0, 1),     // Outer
            };

        public static readonly Color[] TaikoColorsHSV =
        {
            new Color(0, 1, 1),       // Red Outer
            new Color(230, 1, 1),      // Blue
            new Color(60, 1, 1),      // Yellow
            new Color(195, 1, 1),     // Cyan
        };
        public static readonly Color[] RPGColorsHSV =
        {
            new Color(),       // Unused
            new Color(0, 1, 1),      // back
            new Color(50, 1, 1),      // enter
            new Color(),      // Unused
            new Color(270, 1, 1),     // Menu
            new Color(),     // Unused
            new Color(),     // Unused
            new Color(175, 1, 1),     // Attacc
            new Color(0, 0, 1),     // Up
            new Color(0, 0, 1),     // Down
            new Color(0, 0, 0.7),     // Left
            new Color(0, 0, 0.7),     // Right
        };
        public static void SaveAllColors()
        {
            ColorData data = new ColorData
            {
                animBreatheSpeedStepBetween0And1 = animBreatheSpeedStepBetween0And1,
                animColorJumpDelayCount = animColorJumpDelayCount,
                animColorJumpStepBetween0And360 = animColorJumpStepBetween0And360,
                animColorSpeedStepBetween0And360 = animColorSpeedStepBetween0And360,
                animColorWaccaSpeedBetween0And60 = animColorWaccaSpeedBetween0And60,
                animIndex = ColorStorage.animIndex,
                Color1 = ColorStorage.ColorsHSV12[0],
                Color2 = ColorStorage.ColorsHSV12[1],
                Color3 = ColorStorage.ColorsHSV12[2],
                Color4 = ColorStorage.ColorsHSV12[3],
                Color5 = ColorStorage.ColorsHSV12[4],
                Color6 = ColorStorage.ColorsHSV12[5],
                Color7 = ColorStorage.ColorsHSV12[6],
                Color8 = ColorStorage.ColorsHSV12[7],
                Color9 = ColorStorage.ColorsHSV12[8],
                Color10 = ColorStorage.ColorsHSV12[9],
                Color11 = ColorStorage.ColorsHSV12[10],
                Color12 = ColorStorage.ColorsHSV12[11],
                LoveLiveBack12 = LoveLiveColorsHSV[12],
                LoveLiveCoin0 = LoveLiveColorsHSV[0],
                LoveLive1Left = LoveLiveColorsHSV[1],
                LoveLive2 = LoveLiveColorsHSV[2],
                LoveLive3 = LoveLiveColorsHSV[3],
                LoveLive4 = LoveLiveColorsHSV[4],
                LoveLive5Bottom = LoveLiveColorsHSV[5],
                LoveLive6 = LoveLiveColorsHSV[6],
                LoveLive7 = LoveLiveColorsHSV[7],
                LoveLive8 = LoveLiveColorsHSV[8],
                LoveLive9Right = LoveLiveColorsHSV[9],
                LoveLiveStart10 = LoveLiveColorsHSV[10],
                LoveLiveP11 = LoveLiveColorsHSV[11],
                OuterCircleColor = outer,
                SDVXouterL = SDVXouterL,
                SDVXouterR = SDVXouterR,
                SDVX1 = SDVXColorsHSV[0],
                SDVX2 = SDVXColorsHSV[1],
                SDVX3 = SDVXColorsHSV[2],
                SDVX4rightFX = SDVXColorsHSV[3],
                SDVX5rightLane = SDVXColorsHSV[4],
                SDVX6middleRightLane = SDVXColorsHSV[5],
                SDVX7middleLeftLane = SDVXColorsHSV[6],
                SDVX8leftLane = SDVXColorsHSV[7],
                SDVX9leftFX = SDVXColorsHSV[8],
                SDVX10 = SDVXColorsHSV[9],
                SDVX11 = SDVXColorsHSV[10],
                SDVX12 = SDVXColorsHSV[11],
                SDVX13top = SDVXColorsHSV[12],
                Osu1topRight = OsuColorsHSV[0],
                Osu2 = OsuColorsHSV[1],
                Osu3 = OsuColorsHSV[2],
                Osu4BottomRight = OsuColorsHSV[3],
                Osu5BottomLeft = OsuColorsHSV[4],
                Osu6 = OsuColorsHSV[5],
                Osu7 = OsuColorsHSV[6],
                Osu8topLeft = OsuColorsHSV[7],
                MouseUp = mouseHSV[0],
                MouseRight = mouseHSV[1],
                MouseDown = mouseHSV[2],
                MouseLeft = mouseHSV[3],
                MouseOuter = mouseHSV[4],
                TaikoOuterL = TaikoColorsHSV[0],
                TaikoOuterR = TaikoColorsHSV[1],
                TaikoInnerL = TaikoColorsHSV[2],
                TaikoInnerR = TaikoColorsHSV[3],
                rpgBack = RPGColorsHSV[1],
                rpgEnter = RPGColorsHSV[2],
                rpgMenu = RPGColorsHSV[4],
                rpgAttacc = RPGColorsHSV[7],
                rpgOuterUp = RPGColorsHSV[8],
                rpgOuterDown = RPGColorsHSV[9],
                rpgOuterLeft = RPGColorsHSV[10],
                rpgOuterRight = RPGColorsHSV[11],
            };
            string json = JsonConvert.SerializeObject(data, Formatting.Indented);
            File.WriteAllText("WaccaCircleConfig.json", json);
        }
        public static void LoadAllColors()
        {
            if (File.Exists("WaccaCircleConfig.json"))
            {
                try
                {
                    string json = File.ReadAllText("WaccaCircleConfig.json");
                    var data = JsonConvert.DeserializeObject<ColorData>(json);
                    ColorStorage.animIndex = data.animIndex;
                    animBreatheSpeedStepBetween0And1 = data.animBreatheSpeedStepBetween0And1;
                    animColorSpeedStepBetween0And360 = data.animColorSpeedStepBetween0And360;
                    animColorJumpStepBetween0And360 = data.animColorJumpStepBetween0And360;
                    animColorWaccaSpeedBetween0And60 = data.animColorWaccaSpeedBetween0And60;
                    animColorJumpDelayCount = data.animColorJumpDelayCount;
                    WaccaTable.UpdateMyAnimBasedOnList(false);
                    // regex find
                    // ([^ ]*) = ([^,]*),
                    // replace with
                    // $2 = data.$1;
                    ColorStorage.ColorsHSV12[0] = data.Color1;
                    ColorStorage.ColorsHSV12[1] = data.Color2;
                    ColorStorage.ColorsHSV12[2] = data.Color3;
                    ColorStorage.ColorsHSV12[3] = data.Color4;
                    ColorStorage.ColorsHSV12[4] = data.Color5;
                    ColorStorage.ColorsHSV12[5] = data.Color6;
                    ColorStorage.ColorsHSV12[6] = data.Color7;
                    ColorStorage.ColorsHSV12[7] = data.Color8;
                    ColorStorage.ColorsHSV12[8] = data.Color9;
                    ColorStorage.ColorsHSV12[9] = data.Color10;
                    ColorStorage.ColorsHSV12[10] = data.Color11;
                    ColorStorage.ColorsHSV12[11] = data.Color12;
                    LoveLiveColorsHSV[12] = data.LoveLiveBack12;
                    LoveLiveColorsHSV[0] = data.LoveLiveCoin0;
                    LoveLiveColorsHSV[1] = data.LoveLive1Left;
                    LoveLiveColorsHSV[2] = data.LoveLive2;
                    LoveLiveColorsHSV[3] = data.LoveLive3;
                    LoveLiveColorsHSV[4] = data.LoveLive4;
                    LoveLiveColorsHSV[5] = data.LoveLive5Bottom;
                    LoveLiveColorsHSV[6] = data.LoveLive6;
                    LoveLiveColorsHSV[7] = data.LoveLive7;
                    LoveLiveColorsHSV[8] = data.LoveLive8;
                    LoveLiveColorsHSV[9] = data.LoveLive9Right;
                    LoveLiveColorsHSV[10] = data.LoveLiveStart10;
                    LoveLiveColorsHSV[11] = data.LoveLiveP11;
                    outer = data.OuterCircleColor;
                    SDVXouterL = data.SDVXouterL;
                    SDVXouterR = data.SDVXouterR;
                    SDVXColorsHSV[0] = data.SDVX1;
                    SDVXColorsHSV[1] = data.SDVX2;
                    SDVXColorsHSV[2] = data.SDVX3;
                    SDVXColorsHSV[3] = data.SDVX4rightFX;
                    SDVXColorsHSV[4] = data.SDVX5rightLane;
                    SDVXColorsHSV[5] = data.SDVX6middleRightLane;
                    SDVXColorsHSV[6] = data.SDVX7middleLeftLane;
                    SDVXColorsHSV[7] = data.SDVX8leftLane;
                    SDVXColorsHSV[8] = data.SDVX9leftFX;
                    SDVXColorsHSV[9] = data.SDVX10;
                    SDVXColorsHSV[10] = data.SDVX11;
                    SDVXColorsHSV[11] = data.SDVX12;
                    SDVXColorsHSV[12] = data.SDVX13top;
                    OsuColorsHSV[0] = data.Osu1topRight;
                    OsuColorsHSV[1] = data.Osu2;
                    OsuColorsHSV[2] = data.Osu3;
                    OsuColorsHSV[3] = data.Osu4BottomRight;
                    OsuColorsHSV[4] = data.Osu5BottomLeft;
                    OsuColorsHSV[5] = data.Osu6;
                    OsuColorsHSV[6] = data.Osu7;
                    OsuColorsHSV[7] = data.Osu8topLeft;
                    mouseHSV[0] = data.MouseUp;
                    mouseHSV[1] = data.MouseRight;
                    mouseHSV[2] = data.MouseDown;
                    mouseHSV[3] = data.MouseLeft;
                    mouseHSV[4] = data.MouseOuter;
                    TaikoColorsHSV[0] = data.TaikoOuterL;
                    TaikoColorsHSV[1] = data.TaikoOuterR;
                    TaikoColorsHSV[2] = data.TaikoInnerL;
                    TaikoColorsHSV[3] = data.TaikoInnerR;
                    RPGColorsHSV[1] = data.rpgBack;
                    RPGColorsHSV[2] = data.rpgEnter;
                    RPGColorsHSV[4] = data.rpgMenu;
                    RPGColorsHSV[7] = data.rpgAttacc;
                    RPGColorsHSV[8] = data.rpgOuterUp;
                    RPGColorsHSV[9] = data.rpgOuterDown;
                    RPGColorsHSV[10] = data.rpgOuterLeft;
                    RPGColorsHSV[11] = data.rpgOuterRight;
                    Console.WriteLine("Loaded WaccaCircleConfig.json!");
                    Console.WriteLine(data.animBreatheSpeedStepBetween0And1);
                }
                catch (Exception e)
                {
                    SaveAllColors();
                }
            }
            else
            {
                Console.WriteLine("WaccaCircleConfig.json not found!\nCreating config file...");
                SaveAllColors();
            }
        }


    }
    public static class WaccaLightAnimation
    {
        private static double v = 1.0;
        private static double sine = 0.0;
        private static double h = 0.0;
        private static byte f2 = 0;
        private static bool dimmer = true;
        private static double step = ColorStorage.animBreatheSpeedStepBetween0And1;
        private static double colorStep = ColorStorage.animColorSpeedStepBetween0And360;
        private static double jumpStep = ColorStorage.animColorJumpStepBetween0And360;
        private static uint jumpDelay = ColorStorage.animColorJumpDelayCount;
        public static double V() { return v; }
        public static double H() { return h; }
        public static double HSVbreathe()
        {
            if (v <= 0.0)
            {
                if (f2 > 10)
                {
                    f2 = 0;
                    v += step;
                    dimmer = false;
                }
                f2++;
            }
            else if (v >= 1.0)
            {
                if (f2 > 10)
                {
                    f2 = 0;
                    v -= step;
                    dimmer = true;
                }
                f2++;
            }
            else
            {
                if (dimmer)
                {
                    v -= step;
                }
                else
                {
                    v += step;
                }
            }
            if (v < 0.0)
            {
                v = 0.0;
            }
            else if (v > 1.0)
            {
                v = 1.0;
            }
            return v;
        }
        public static double HSVmid()
        {
            if (v <= 0.25)
            {
                if (f2 > 10)
                {
                    f2 = 0;
                    v += step;
                    dimmer = false;
                }
                f2++;
            }
            else if (v >= 0.75)
            {
                if (f2 > 10)
                {
                    f2 = 0;
                    v -= step;
                    dimmer = true;
                }
                f2++;
            }
            else
            {
                if (dimmer)
                {
                    v -= step;
                }
                else
                {
                    v += step;
                }
            }
            if (v < 0.0)
            {
                v = 0.0;
            }
            else if (v > 1.0)
            {
                v = 1.0;
            }
            return v;
        }
        public static double HSVsine()
        {
            sine += step;
            v = (Math.Sin(sine) + 1.0) / 2.0; // shift between 0 and 1
            return v;
        }
        public static double HSVsineMid()
        {
            sine += step;
            v = (Math.Sin(sine) + 2.0) / 4.0; // shift between 0.25 and 0.75
            return v;
        }
        public static double HSVColorJump()
        {
            if (f2 > jumpDelay)
            {
                f2 = 0;
                h += jumpStep;
            }
            f2++;
            if (h <= 0)
            {
                h = 360.0;
            }
            else if (h >= 360.0)
            {
                h = 0.0;
            }
            return h;
        }
        public static double HSVColorJumpReverse()
        {
            if (f2 > jumpDelay)
            {
                f2 = 0;
                h -= jumpStep;
            }
            f2++;
            if (h <= 0)
            {
                h = 360.0;
            }
            else if (h >= 360.0)
            {
                h = 0.0;
            }
            return h;
        }
        public static double HSVColorCycle()
        {
            h += colorStep;
            if (h <= 0)
            {
                h = 360.0;
            }
            else if (h >= 360.0)
            {
                h = 0.0;
            }
            return h;
        }
        public static double HSVColorCycleReverse()
        {
            h -= colorStep;
            if (h <= 0.0)
            {
                h = 360.0;
            }
            else if (h >= 360.0)
            {
                h = 0.0;
            }
            return h;
        }
        public static double HSVColorAdd6()
        {
            h += 6.0;
            if (h <= 0)
            {
                h = 360.0;
            }
            else if (h >= 360.0)
            {
                h = 0.0;
            }
            return h;
        }
        public static double HSVColorSub6()
        {
            h -= 6.0;
            if (h <= 0)
            {
                h = 360.0;
            }
            else if (h >= 360.0)
            {
                h = 0.0;
            }
            return h;
        }
        public static double Static()
        {
            return h;
        }
        public static double Freeze()
        {
            return h;
        }
        public static double Off()
        {
            v = 0;
            return 0;
        }
    }
    public static class WaccaTable
    {
        public static LightLayer layer0 = new LightLayer();
        private static List<Func<double>> MyAnimList = new List<Func<double>>();
        private static Func<double> anim = WaccaLightAnimation.HSVmid;
        private static readonly long axis_max = 32767;
        // if you wanna add a new color anim, just att a text entry to waccaCircleText, then add your func to MyAnimList below.
        public static byte color_anim = 0;
        public static void Initialize()
        {
            MyAnimList.Add(WaccaLightAnimation.HSVbreathe);
            MyAnimList.Add(WaccaLightAnimation.HSVmid);
            MyAnimList.Add(WaccaLightAnimation.HSVsine);
            MyAnimList.Add(WaccaLightAnimation.HSVsineMid);
            MyAnimList.Add(WaccaLightAnimation.HSVColorJump);
            MyAnimList.Add(WaccaLightAnimation.HSVColorJumpReverse);
            MyAnimList.Add(WaccaLightAnimation.HSVColorCycle);
            MyAnimList.Add(WaccaLightAnimation.HSVColorCycleReverse);
            MyAnimList.Add(WaccaLightAnimation.Freeze);
            // ========== FULL CIRCLE COLOR START ===========
            MyAnimList.Add(WaccaLightAnimation.HSVColorJump);
            MyAnimList.Add(WaccaLightAnimation.HSVColorJumpReverse);
            MyAnimList.Add(WaccaLightAnimation.HSVColorCycle);
            MyAnimList.Add(WaccaLightAnimation.HSVColorCycleReverse);
            MyAnimList.Add(WaccaLightAnimation.HSVColorAdd6);
            MyAnimList.Add(WaccaLightAnimation.HSVColorSub6);
            // ========== FULL CIRCLE COLOR END ===========
            MyAnimList.Add(WaccaLightAnimation.Off);
            MyAnimList.Add(WaccaLightAnimation.Static);
        }
        static string[] waccaCircleText = { "Breathe", "Mid-Breathe", "Sine Breathe", "Sine Mid-Breathe", "Jump",
                                        "Reverse Jump", "Color Cycle", "Reverse Color Cycle", "Freeze", "Jump",
                                        "Reverse Jump", "Color Cycle", "Reverse Color Cycle", "Wacca", "Reverse Wacca", "Off", "Custom"};
        public static void UpdateMyAnimBasedOnList(bool display_name = true)
        {
            if (ColorStorage.animIndex < 0 || ColorStorage.animIndex >= MyAnimList.Count)
            {
                ColorStorage.animIndex = 0;
            }
            anim = MyAnimList[ColorStorage.animIndex];  // changes the default function
            // Launch the overlay window
            if (File.Exists(WaccaCircle.exe_title) && display_name)
            {
                WaccaCircle.RunExternalCommand(WaccaCircle.exe_title, waccaCircleText[ColorStorage.animIndex]);
            }
            Console.WriteLine("Changing animation");
            if (ColorStorage.animIndex < 4)
            {
                color_anim = 0;  // brightness anim Value
            }
            else if (ColorStorage.animIndex < 8)
            {
                color_anim = 1;  // hue anim
            }
            else if (ColorStorage.animIndex < 9)
            {
                color_anim = 2;  // freeze
                WaccaCircle.lights_have_been_sent_once = false;
            }
            else if (ColorStorage.animIndex < 13)
            {
                color_anim = 3;  // Complete circle anim
                WaccaCircle.lights_have_been_sent_once = false;
            }
            else if (ColorStorage.animIndex < 15)
            {
                color_anim = 4;  // Wacca
                WaccaCircle.lights_have_been_sent_once = false;
            }
            else if (ColorStorage.animIndex < 16)
            {
                color_anim = 5;  // off
                TurnOffTheLights();
                WaccaCircle.lights_have_been_sent_once = false;
            }
            else if (ColorStorage.animIndex < 17)
            {
                color_anim = 6;  // reset
                ColorStorage.LoadAllColors();
                WaccaCircle.lights_have_been_sent_once = false;
            }
        }
        private static int A(double v)
        {
            // Use -Sin to calculate the X position and shift it to the range [-axis_max, axis_max]
            return (int)((-Math.Sin(v * Math.PI / 30)) * 10);
        }
        private static int B(double v)
        {
            // Use Cos to calculate the Y position and shift it to the range [-axis_max, axis_max]
            return (int)((Math.Cos(v * Math.PI / 30)) * 10);
        }
        private static int X(double v)
        {
            // Use -Sin to calculate the X position and shift it to the range [0, axis_max]
            return (int)((-Math.Sin(v * Math.PI / 30) + 1) / 2 * axis_max);
        }

        private static int Y(double v)
        {
            // Use Cos to calculate the Y position and shift it to the range [0, axis_max]
            return (int)((Math.Cos(v * Math.PI / 30) + 1) / 2 * axis_max);
        }


        public static readonly int[][] axes =
         {     //       x axis    y-axis   1-12  13-16  17-20  21-22  23-24  25-32
            new int[] { X(0.5),   Y(0.5),   12,   16,    17,    21,    23,    32},    // 0  top circle
            new int[] { X(1.5),   Y(1.5),   12,   16,    17,    21,    23,    32},    // 1
            new int[] { X(2.5),   Y(2.5),   12,   16,    17,    21,    23,    32},    // 2
            new int[] { X(3.5),   Y(3.5),   11,   16,    17,    21,    23,    32},    // 3
            new int[] { X(4.5),   Y(4.5),   11,   16,    17,    21,    23,    32},    // 4
            new int[] { X(5.5),   Y(5.5),   11,   16,    17,    21,    23,    32},    // 5
            new int[] { X(6.5),   Y(6.5),   11,   16,    17,    21,    23,    32},    // 6
            new int[] { X(7.5),   Y(7.5),   11,   16,    17,    21,    23,    32},    // 7
            new int[] { X(8.5),   Y(8.5),   10,   16,    20,    21,    23,    31},    // 8
            new int[] { X(9.5),   Y(9.5),   10,   16,    20,    21,    23,    31},    // 9
            new int[] { X(10.5),  Y(10.5),  10,   16,    20,    21,    23,    31},    // 10
            new int[] { X(11.5),  Y(11.5),  10,   16,    20,    21,    23,    31},    // 11
            new int[] { X(12.5),  Y(12.5),  10,   16,    20,    21,    23,    31},    // 12
            new int[] { X(13.5),  Y(13.5),   9,   16,    20,    21,    23,    31},    // 13
            new int[] { X(14.5),  Y(14.5),   9,   16,    20,    21,    23,    31},    // 14  left
            new int[] { X(15.5),  Y(15.5),   9,   15,    20,    22,    23,    31},    // 15  left 
            new int[] { X(16.5),  Y(16.5),   9,   15,    20,    22,    23,    30},    // 16
            new int[] { X(17.5),  Y(17.5),   9,   15,    20,    22,    23,    30},    // 17
            new int[] { X(18.5),  Y(18.5),   8,   15,    20,    22,    23,    30},    // 18
            new int[] { X(19.5),  Y(19.5),   8,   15,    20,    22,    23,    30},    // 19
            new int[] { X(20.5),  Y(20.5),   8,   15,    20,    22,    23,    30},    // 20
            new int[] { X(21.5),  Y(21.5),   8,   15,    20,    22,    23,    30},    // 21
            new int[] { X(22.5),  Y(22.5),   8,   15,    20,    22,    23,    30},    // 22
            new int[] { X(23.5),  Y(23.5),   7,   15,    19,    22,    23,    29},    // 23
            new int[] { X(24.5),  Y(24.5),   7,   15,    19,    22,    23,    29},    // 24
            new int[] { X(25.5),  Y(25.5),   7,   15,    19,    22,    23,    29},    // 25
            new int[] { X(26.5),  Y(26.5),   7,   15,    19,    22,    23,    29},    // 26
            new int[] { X(27.5),  Y(27.5),   7,   15,    19,    22,    23,    29},    // 27
            new int[] { X(28.5),  Y(28.5),   6,   15,    19,    22,    23,    29},    // 28
            new int[] { X(29.5),  Y(29.5),   6,   15,    19,    22,    23,    29},    // 29  bottom
            new int[] { X(30.5),  Y(30.5),   6,   14,    19,    22,    24,    28},    // 30  bottom
            new int[] { X(31.5),  Y(31.5),   6,   14,    19,    22,    24,    28},    // 31
            new int[] { X(32.5),  Y(32.5),   6,   14,    19,    22,    24,    28},    // 32
            new int[] { X(33.5),  Y(33.5),   5,   14,    19,    22,    24,    28},    // 33
            new int[] { X(34.5),  Y(34.5),   5,   14,    19,    22,    24,    28},    // 34
            new int[] { X(35.5),  Y(35.5),   5,   14,    19,    22,    24,    28},    // 35
            new int[] { X(36.5),  Y(36.5),   5,   14,    19,    22,    24,    28},    // 36
            new int[] { X(37.5),  Y(37.5),   5,   14,    19,    22,    24,    27},    // 37
            new int[] { X(38.5),  Y(38.5),   4,   14,    18,    22,    24,    27},    // 38
            new int[] { X(39.5),  Y(39.5),   4,   14,    18,    22,    24,    27},    // 39
            new int[] { X(40.5),  Y(40.5),   4,   14,    18,    22,    24,    27},    // 40
            new int[] { X(41.5),  Y(41.5),   4,   14,    18,    22,    24,    27},    // 41
            new int[] { X(42.5),  Y(42.5),   4,   14,    18,    22,    24,    27},    // 42
            new int[] { X(43.5),  Y(43.5),   3,   14,    18,    22,    24,    27},    // 43
            new int[] { X(44.5),  Y(44.5),   3,   14,    18,    22,    24,    26},    // 44  right
            new int[] { X(45.5),  Y(45.5),   3,   13,    18,    21,    24,    26},    // 45  right
            new int[] { X(46.5),  Y(46.5),   3,   13,    18,    21,    24,    26},    // 46
            new int[] { X(47.5),  Y(47.5),   3,   13,    18,    21,    24,    26},    // 47
            new int[] { X(48.5),  Y(48.5),   2,   13,    18,    21,    24,    26},    // 48
            new int[] { X(49.5),  Y(49.5),   2,   13,    18,    21,    24,    26},    // 49
            new int[] { X(50.5),  Y(50.5),   2,   13,    18,    21,    24,    26},    // 50
            new int[] { X(51.5),  Y(51.5),   2,   13,    18,    21,    24,    26},    // 51
            new int[] { X(52.5),  Y(52.5),   2,   13,    18,    21,    24,    25},    // 52
            new int[] { X(53.5),  Y(53.5),   1,   13,    17,    21,    24,    25},    // 53
            new int[] { X(54.5),  Y(54.5),   1,   13,    17,    21,    24,    25},    // 54
            new int[] { X(55.5),  Y(55.5),   1,   13,    17,    21,    24,    25},    // 55
            new int[] { X(56.5),  Y(56.5),   1,   13,    17,    21,    24,    25},    // 56
            new int[] { X(57.5),  Y(57.5),   1,   13,    17,    21,    24,    25},    // 57
            new int[] { X(58.5),  Y(58.5),  12,   13,    17,    21,    24,    25},    // 58
            new int[] { X(59.5),  Y(59.5),  12,   13,    17,    21,    24,    25},    // 59  top circle
        };
        public static readonly int[][] mouseAxes =
        {          //       x axis    y-axis   1-12  13-16  17-20  21-22  23-24  25-32
                new int[] { A(0.5),   B(0.5),   12,   16,    17,    21,    23,    25},    // 0  top circle
                new int[] { A(1.5),   B(1.5),   12,   16,    17,    21,    23,    25},    // 1
                new int[] { A(2.5),   B(2.5),   12,   16,    17,    21,    23,    25},    // 2
                new int[] { A(3.5),   B(3.5),   11,   16,    17,    21,    23,    25},    // 3
                new int[] { A(4.5),   B(4.5),   11,   16,    17,    21,    23,    25},    // 4
                new int[] { A(5.5),   B(5.5),   11,   16,    17,    21,    23,    25},    // 5
                new int[] { A(6.5),   B(6.5),   11,   16,    17,    21,    23,    25},    // 6
                new int[] { A(7.5),   B(7.5),   11,   16,    17,    21,    23,    25},    // 7
                new int[] { A(8.5),   B(8.5),   10,   16,    20,    21,    23,    26},    // 8
                new int[] { A(9.5),   B(9.5),   10,   16,    20,    21,    23,    26},    // 9
                new int[] { A(10.5),  B(10.5),  10,   16,    20,    21,    23,    26},    // 10
                new int[] { A(11.5),  B(11.5),  10,   16,    20,    21,    23,    26},    // 11
                new int[] { A(12.5),  B(12.5),  10,   16,    20,    21,    23,    26},    // 12
                new int[] { A(13.5),  B(13.5),   9,   16,    20,    21,    23,    26},    // 13
                new int[] { A(14.5),  B(14.5),   9,   16,    20,    21,    23,    26},    // 14  left
                new int[] { A(15.5),  B(15.5),   9,   15,    20,    22,    23,    26},    // 15  left 
                new int[] { A(16.5),  B(16.5),   9,   15,    20,    22,    23,    27},    // 16
                new int[] { A(17.5),  B(17.5),   9,   15,    20,    22,    23,    27},    // 17
                new int[] { A(18.5),  B(18.5),   8,   15,    20,    22,    23,    27},    // 18
                new int[] { A(19.5),  B(19.5),   8,   15,    20,    22,    23,    27},    // 19
                new int[] { A(20.5),  B(20.5),   8,   15,    20,    22,    23,    27},    // 20
                new int[] { A(21.5),  B(21.5),   8,   15,    20,    22,    23,    27},    // 21
                new int[] { A(22.5),  B(22.5),   8,   15,    20,    22,    23,    27},    // 22
                new int[] { A(23.5),  B(23.5),   7,   15,    19,    22,    23,    28},    // 23
                new int[] { A(24.5),  B(24.5),   7,   15,    19,    22,    23,    28},    // 24
                new int[] { A(25.5),  B(25.5),   7,   15,    19,    22,    23,    28},    // 25
                new int[] { A(26.5),  B(26.5),   7,   15,    19,    22,    23,    28},    // 26
                new int[] { A(27.5),  B(27.5),   7,   15,    19,    22,    23,    28},    // 27
                new int[] { A(28.5),  B(28.5),   6,   15,    19,    22,    23,    28},    // 28
                new int[] { A(29.5),  B(29.5),   6,   15,    19,    22,    23,    28},    // 29  bottom
                new int[] { A(30.5),  B(30.5),   6,   14,    19,    22,    24,    29},    // 30  bottom
                new int[] { A(31.5),  B(31.5),   6,   14,    19,    22,    24,    29},    // 31
                new int[] { A(32.5),  B(32.5),   6,   14,    19,    22,    24,    29},    // 32
                new int[] { A(33.5),  B(33.5),   5,   14,    19,    22,    24,    29},    // 33
                new int[] { A(34.5),  B(34.5),   5,   14,    19,    22,    24,    29},    // 34
                new int[] { A(35.5),  B(35.5),   5,   14,    19,    22,    24,    29},    // 35
                new int[] { A(36.5),  B(36.5),   5,   14,    19,    22,    24,    29},    // 36
                new int[] { A(37.5),  B(37.5),   5,   14,    19,    22,    24,    30},    // 37
                new int[] { A(38.5),  B(38.5),   4,   14,    18,    22,    24,    30},    // 38
                new int[] { A(39.5),  B(39.5),   4,   14,    18,    22,    24,    30},    // 39
                new int[] { A(40.5),  B(40.5),   4,   14,    18,    22,    24,    30},    // 40
                new int[] { A(41.5),  B(41.5),   4,   14,    18,    22,    24,    30},    // 41
                new int[] { A(42.5),  B(42.5),   4,   14,    18,    22,    24,    30},    // 42
                new int[] { A(43.5),  B(43.5),   3,   14,    18,    22,    24,    30},    // 43
                new int[] { A(44.5),  B(44.5),   3,   14,    18,    22,    24,    31},    // 44  right
                new int[] { A(45.5),  B(45.5),   3,   13,    18,    21,    24,    31},    // 45  right
                new int[] { A(46.5),  B(46.5),   3,   13,    18,    21,    24,    31},    // 46
                new int[] { A(47.5),  B(47.5),   3,   13,    18,    21,    24,    31},    // 47
                new int[] { A(48.5),  B(48.5),   2,   13,    18,    21,    24,    31},    // 48
                new int[] { A(49.5),  B(49.5),   2,   13,    18,    21,    24,    31},    // 49
                new int[] { A(50.5),  B(50.5),   2,   13,    18,    21,    24,    31},    // 50
                new int[] { A(51.5),  B(51.5),   2,   13,    18,    21,    24,    31},    // 51
                new int[] { A(52.5),  B(52.5),   2,   13,    18,    21,    24,    32},    // 52
                new int[] { A(53.5),  B(53.5),   1,   13,    17,    21,    24,    32},    // 53
                new int[] { A(54.5),  B(54.5),   1,   13,    17,    21,    24,    32},    // 54
                new int[] { A(55.5),  B(55.5),   1,   13,    17,    21,    24,    32},    // 55
                new int[] { A(56.5),  B(56.5),   1,   13,    17,    21,    24,    32},    // 56
                new int[] { A(57.5),  B(57.5),   1,   13,    17,    21,    24,    32},    // 57
                new int[] { A(58.5),  B(58.5),  12,   13,    17,    21,    24,    32},    // 58
                new int[] { A(59.5),  B(59.5),  12,   13,    17,    21,    24,    32},    // 59  top circle
            };
        // this means column 0 (x axis), 1 (y-axis), 2 (1-13), and 5 (21-22) are used by sdvx
        // also column 7 is used by Love Live
        public static readonly int[][] SDVXaxesxy0125LoveLive017 =
            {      // SDVX  x axis    y-axis   1-13  13-16  17-20  21-22  23-24  1-13
                new int[] { X(0.5),   Y(30.5),  13,   16,    17,    21,    23,    1},    // 0  top circle
                new int[] { X(1.5),   Y(31.5),  13,   16,    17,    21,    23,    1},    // 1
                new int[] { X(2.5),   Y(32.5),  13,   16,    17,    21,    23,    1},    // 2
                new int[] { X(3.5),   Y(33.5),  12,   16,    17,    21,    23,    1},    // 3
                new int[] { X(4.5),   Y(34.5),  12,   16,    17,    21,    23,    1},    // 4
                new int[] { X(5.5),   Y(35.5),  12,   16,    17,    21,    23,    13},    // 5
                new int[] { X(6.5),   Y(36.5),  12,   16,    17,    21,    23,    13},    // 6
                new int[] { X(7.5),   Y(37.5),  12,   16,    17,    21,    23,    13},    // 7
                new int[] { X(8.5),   Y(38.5),  11,   16,    20,    21,    23,    13},    // 8
                new int[] { X(9.5),   Y(39.5),  11,   16,    20,    21,    23,    13},    // 9
                new int[] { X(10.5),  Y(40.5),  11,   16,    20,    21,    23,    2},    // 10
                new int[] { X(11.5),  Y(41.5),  11,   16,    20,    21,    23,    2},    // 11
                new int[] { X(12.5),  Y(42.5),  11,   16,    20,    21,    23,    2},    // 12
                new int[] { X(13.5),  Y(43.5),  10,   16,    20,    21,    23,    2},    // 13
                new int[] { X(14.5),  Y(44.5),  10,   16,    20,    21,    23,    2},    // 14  left
                new int[] { X(45.5),  Y(45.5),  10,   15,    20,    22,    23,    2},    // 15  left 
                new int[] { X(46.5),  Y(46.5),  10,   15,    20,    22,    23,    2},    // 16
                new int[] { X(47.5),  Y(47.5),  10,   15,    20,    22,    23,    3},    // 17
                new int[] { X(48.5),  Y(48.5),   9,   15,    20,    22,    23,    3},    // 18
                new int[] { X(49.5),  Y(49.5),   9,   15,    20,    22,    23,    3},    // 19
                new int[] { X(50.5),  Y(50.5),   9,   15,    20,    22,    23,    3},    // 20
                new int[] { X(51.5),  Y(51.5),   9,   15,    20,    22,    23,    4},    // 21
                new int[] { X(52.5),  Y(52.5),   9,   15,    20,    22,    23,    4},    // 22
                new int[] { X(53.5),  Y(53.5),   8,   15,    19,    22,    23,    4},    // 23
                new int[] { X(54.5),  Y(54.5),   8,   15,    19,    22,    23,    4},    // 24
                new int[] { X(55.5),  Y(55.5),   8,   15,    19,    22,    23,    5},    // 25
                new int[] { X(56.5),  Y(56.5),   8,   15,    19,    22,    23,    5},    // 26
                new int[] { X(57.5),  Y(57.5),   7,   15,    19,    22,    23,    5},    // 27
                new int[] { X(58.5),  Y(58.5),   7,   15,    19,    22,    23,    6},    // 28
                new int[] { X(59.5),  Y(59.5),   7,   15,    19,    22,    23,    6},    // 29  bottom
                new int[] { X(00.5),  Y(30.5),   6,   14,    19,    22,    24,    6},    // 30  bottom
                new int[] { X(01.5),  Y(31.5),   6,   14,    19,    22,    24,    6},    // 31
                new int[] { X(02.5),  Y(32.5),   6,   14,    19,    22,    24,    7},    // 32
                new int[] { X(03.5),  Y(33.5),   5,   14,    19,    22,    24,    7},    // 33
                new int[] { X(04.5),  Y(34.5),   5,   14,    19,    22,    24,    7},    // 34
                new int[] { X(05.5),  Y(35.5),   5,   14,    19,    22,    24,    8},    // 35
                new int[] { X(06.5),  Y(36.5),   5,   14,    19,    22,    24,    8},    // 36
                new int[] { X(07.5),  Y(37.5),   4,   14,    19,    22,    24,    8},    // 37
                new int[] { X(08.5),  Y(38.5),   4,   14,    18,    22,    24,    8},    // 38
                new int[] { X(09.5),  Y(39.5),   4,   14,    18,    22,    24,    9},    // 39
                new int[] { X(10.5),  Y(40.5),   4,   14,    18,    22,    24,    9},    // 40
                new int[] { X(11.5),  Y(41.5),   4,   14,    18,    22,    24,    9},    // 41
                new int[] { X(12.5),  Y(42.5),   4,   14,    18,    22,    24,    9},    // 42
                new int[] { X(13.5),  Y(43.5),   3,   14,    18,    22,    24,    10},    // 43
                new int[] { X(14.5),  Y(44.5),   3,   14,    18,    22,    24,    10},    // 44  right
                new int[] { X(45.5),  Y(45.5),   3,   13,    18,    21,    24,    10},    // 45  right
                new int[] { X(46.5),  Y(46.5),   3,   13,    18,    21,    24,    10},    // 46
                new int[] { X(47.5),  Y(47.5),   3,   13,    18,    21,    24,    10},    // 47
                new int[] { X(48.5),  Y(48.5),   2,   13,    18,    21,    24,    10},    // 48
                new int[] { X(49.5),  Y(49.5),   2,   13,    18,    21,    24,    10},    // 49
                new int[] { X(50.5),  Y(50.5),   2,   13,    18,    21,    24,    11},    // 50
                new int[] { X(51.5),  Y(51.5),   2,   13,    18,    21,    24,    11},    // 51
                new int[] { X(52.5),  Y(52.5),   2,   13,    18,    21,    24,    11},    // 52
                new int[] { X(53.5),  Y(53.5),   1,   13,    17,    21,    24,    11},    // 53
                new int[] { X(54.5),  Y(54.5),   1,   13,    17,    21,    24,    11},    // 54
                new int[] { X(55.5),  Y(55.5),   1,   13,    17,    21,    24,    12},    // 55
                new int[] { X(56.5),  Y(56.5),   1,   13,    17,    21,    24,    12},    // 56
                new int[] { X(57.5),  Y(57.5),   1,   13,    17,    21,    24,    12},    // 57
                new int[] { X(58.5),  Y(58.5),  13,   13,    17,    21,    24,    12},    // 58
                new int[] { X(59.5),  Y(59.5),  13,   13,    17,    21,    24,    12},    // 59  top circle
            };
        public static readonly int[][] RPGaxes =
            {     //  RPG   x axis    y-axis   1-12  1-7   13-16  21-22  23-24  25-32
                new int[] { X(0.5),   Y(0.5),   12,   4,    13,    21,    23,    25},    // 0  top circle
                new int[] { X(1.5),   Y(1.5),   12,   4,    13,    21,    23,    25},    // 1
                new int[] { X(2.5),   Y(2.5),   12,   4,    13,    21,    23,    25},    // 2
                new int[] { X(3.5),   Y(3.5),   11,   4,    13,    21,    23,    25},    // 3
                new int[] { X(4.5),   Y(4.5),   11,   4,    13,    21,    23,    25},    // 4
                new int[] { X(5.5),   Y(5.5),   11,   4,    13,    21,    23,    25},    // 5
                new int[] { X(6.5),   Y(6.5),   11,   4,    13,    21,    23,    25},    // 6
                new int[] { X(7.5),   Y(7.5),   11,   4,    13,    21,    23,    25},    // 7
                new int[] { X(8.5),   Y(8.5),   10,   1,    15,    21,    23,    26},    // 8
                new int[] { X(9.5),   Y(9.5),   10,   1,    15,    21,    23,    26},    // 9
                new int[] { X(10.5),  Y(10.5),  10,   1,    15,    21,    23,    26},    // 10
                new int[] { X(11.5),  Y(11.5),  10,   1,    15,    21,    23,    26},    // 11
                new int[] { X(12.5),  Y(12.5),  10,   1,    15,    21,    23,    26},    // 12
                new int[] { X(13.5),  Y(13.5),   9,   1,    15,    21,    23,    26},    // 13
                new int[] { X(14.5),  Y(14.5),   9,   1,    15,    21,    23,    26},    // 14  left
                new int[] { X(15.5),  Y(15.5),   9,   1,    15,    22,    23,    26},    // 15  left 
                new int[] { X(16.5),  Y(16.5),   9,   1,    15,    22,    23,    27},    // 16
                new int[] { X(17.5),  Y(17.5),   9,   1,    15,    22,    23,    27},    // 17
                new int[] { X(18.5),  Y(18.5),   8,   1,    15,    22,    23,    27},    // 18
                new int[] { X(19.5),  Y(19.5),   8,   1,    15,    22,    23,    27},    // 19
                new int[] { X(20.5),  Y(20.5),   8,   1,    15,    22,    23,    27},    // 20
                new int[] { X(21.5),  Y(21.5),   8,   1,    15,    22,    23,    27},    // 21
                new int[] { X(22.5),  Y(22.5),   8,   1,    15,    22,    23,    27},    // 22
                new int[] { X(23.5),  Y(23.5),   7,   7,    14,    22,    23,    28},    // 23
                new int[] { X(24.5),  Y(24.5),   7,   7,    14,    22,    23,    28},    // 24
                new int[] { X(25.5),  Y(25.5),   7,   7,    14,    22,    23,    28},    // 25
                new int[] { X(26.5),  Y(26.5),   7,   7,    14,    22,    23,    28},    // 26
                new int[] { X(27.5),  Y(27.5),   7,   7,    14,    22,    23,    28},    // 27
                new int[] { X(28.5),  Y(28.5),   6,   7,    14,    22,    23,    28},    // 28
                new int[] { X(29.5),  Y(29.5),   6,   7,    14,    22,    23,    28},    // 29  bottom
                new int[] { X(30.5),  Y(30.5),   6,   7,    14,    22,    24,    29},    // 30  bottom
                new int[] { X(31.5),  Y(31.5),   6,   7,    14,    22,    24,    29},    // 31
                new int[] { X(32.5),  Y(32.5),   6,   7,    14,    22,    24,    29},    // 32
                new int[] { X(33.5),  Y(33.5),   5,   7,    14,    22,    24,    29},    // 33
                new int[] { X(34.5),  Y(34.5),   5,   7,    14,    22,    24,    29},    // 34
                new int[] { X(35.5),  Y(35.5),   5,   7,    14,    22,    24,    29},    // 35
                new int[] { X(36.5),  Y(36.5),   5,   7,    14,    22,    24,    29},    // 36
                new int[] { X(37.5),  Y(37.5),   5,   7,    14,    22,    24,    30},    // 37
                new int[] { X(38.5),  Y(38.5),   4,   2,    16,    22,    24,    30},    // 38
                new int[] { X(39.5),  Y(39.5),   4,   2,    16,    22,    24,    30},    // 39
                new int[] { X(40.5),  Y(40.5),   4,   2,    16,    22,    24,    30},    // 40
                new int[] { X(41.5),  Y(41.5),   4,   2,    16,    22,    24,    30},    // 41
                new int[] { X(42.5),  Y(42.5),   4,   2,    16,    22,    24,    30},    // 42
                new int[] { X(43.5),  Y(43.5),   3,   2,    16,    22,    24,    30},    // 43
                new int[] { X(44.5),  Y(44.5),   3,   2,    16,    22,    24,    31},    // 44  right
                new int[] { X(45.5),  Y(45.5),   3,   2,    16,    21,    24,    31},    // 45  right
                new int[] { X(46.5),  Y(46.5),   3,   2,    16,    21,    24,    31},    // 46
                new int[] { X(47.5),  Y(47.5),   3,   2,    16,    21,    24,    31},    // 47
                new int[] { X(48.5),  Y(48.5),   2,   2,    16,    21,    24,    31},    // 48
                new int[] { X(49.5),  Y(49.5),   2,   2,    16,    21,    24,    31},    // 49
                new int[] { X(50.5),  Y(50.5),   2,   2,    16,    21,    24,    31},    // 50
                new int[] { X(51.5),  Y(51.5),   2,   2,    16,    21,    24,    31},    // 51
                new int[] { X(52.5),  Y(52.5),   2,   2,    16,    21,    24,    32},    // 52
                new int[] { X(53.5),  Y(53.5),   1,   4,    13,    21,    24,    32},    // 53
                new int[] { X(54.5),  Y(54.5),   1,   4,    13,    21,    24,    32},    // 54
                new int[] { X(55.5),  Y(55.5),   1,   4,    13,    21,    24,    32},    // 55
                new int[] { X(56.5),  Y(56.5),   1,   4,    13,    21,    24,    32},    // 56
                new int[] { X(57.5),  Y(57.5),   1,   4,    13,    21,    24,    32},    // 57
                new int[] { X(58.5),  Y(58.5),  12,   4,    13,    21,    24,    32},    // 58
                new int[] { X(59.5),  Y(59.5),  12,   4,    13,    21,    24,    32},    // 59  top circle
            };

        public static LightColor color0;
        public static LightColor color1;
        public static LightColor color2;
        public static LightColor color3;
        public static LightColor color4;
        public static LightColor color5;
        public static LightColor color6;
        public static LightColor color7;
        public static LightColor color8;
        public static LightColor color9;
        public static LightColor color10;
        public static LightColor color11;
        public static LightColor outerL;
        public static LightColor innerL;
        public static LightColor innerR;
        public static LightColor outerR;
        public static LightColor osu0;
        public static LightColor osu1;
        public static LightColor osu2;
        public static LightColor osu3;
        public static LightColor osu4;
        public static LightColor osu5;
        public static LightColor osu6;
        public static LightColor osu7;
        public static LightColor sdvx0;
        public static LightColor sdvx1;
        public static LightColor sdvx2;
        public static LightColor sdvx3;
        public static LightColor sdvx4;
        public static LightColor sdvx5;
        public static LightColor sdvx6;
        public static LightColor sdvx7;
        public static LightColor sdvx8;
        public static LightColor sdvx9;
        public static LightColor sdvx10;
        public static LightColor sdvx11;
        public static LightColor sdvx12;
        public static LightColor sdvxPink;
        public static LightColor sdvxBlue;
        public static LightColor rpgBack;
        public static LightColor rpgEnter;
        public static LightColor rpgMenu;
        public static LightColor rpgAttacc;
        public static LightColor rpgUp;
        public static LightColor rpgDown;
        public static LightColor rpgLeft;
        public static LightColor rpgRight;
        public static LightColor mouseUp;
        public static LightColor mouseRight;
        public static LightColor mouseDown;
        public static LightColor mouseLeft;
        public static LightColor mouseOuter;
        public static LightColor r;
        public static LightColor[] color_num = { color0, color1, color2, color3, color4, color5, color6, color7, color8, color9, color10, color11, innerL, innerR, outerL, outerR, r, mouseUp, mouseRight, mouseDown, mouseLeft, sdvx0, sdvx1, sdvx2, sdvx3, sdvx4, sdvx5, sdvx6 };


        public static LightColor off = new LightColor(0, 0, 0);
        private static void TurnOffTheLights()
        {
            for (byte i = 0; i < 60; i++)
            {
                layer0.SetSegmentColor(0, i, off);
                layer0.SetSegmentColor(1, i, off);
                layer0.SetSegmentColor(2, i, off);
                layer0.SetSegmentColor(3, i, off);
            }
        }
        private static void SetAwesomeColors()
        {
            for (byte i = 0; i < 60; i++)
            {
                ColorStorage.ColorsHSV12[0].H = anim();
                byte[] rgbBytes = ColorStorage.ColorsHSV12[0].ToRGB();
                LightColor c = new LightColor(rgbBytes[0], rgbBytes[1], rgbBytes[2]);
                layer0.SetSegmentColor(0, i, c);
                layer0.SetSegmentColor(1, i, c);
                layer0.SetSegmentColor(2, i, c);
                layer0.SetSegmentColor(3, i, c);
            }
            for (byte i = 0; i < ColorStorage.animColorWaccaSpeedBetween0And60; i++)
            {
                anim();
            }
        }
        // lights.SendLightFrame(new LightFrame(new LightColor(255, 0, 255)), controller.segments);
        //
        //var testFrame = new LightFrame(LightColor.Green);
        //lights.SendLightFrame(testFrame);


        /// <summary>
        /// Sends a light configuration to the specified light controller to simulate a breathing animation.
        /// </summary>
        /// <param name="lights">
        /// The <see cref="LightController"/> instance that will receive the light animation settings.
        /// </param>
        /// <remarks>
        /// Call this method repeatedly to produce a breathing animation effect.  
        /// To reset the animation, set <see cref="f"/> back to <c>1.0</c>.
        /// </remarks>
        public static void PrepLight12(LightController lights)
        {

            for (int i = 0; i < ColorStorage.ColorsHSV12.Length; i++)
            {
                switch (color_anim)
                {
                    case 0:
                        ColorStorage.ColorsHSV12[i].V = WaccaLightAnimation.V();
                        break;
                    case 1:
                        ColorStorage.ColorsHSV12[i].H = WaccaLightAnimation.H();
                        anim();
                        break;
                    case 3:
                        ColorStorage.ColorsHSV12[i].H = WaccaLightAnimation.H();
                        break;
                    case 4:
                        SetAwesomeColors();
                        return;
                    case 5:
                        return;
                    default:
                        break;
                } // 0 is full circle, 1 is localized for each button, 2 is freeze, 3 is full circle, 4 is wacca title, 5 is off, and 6 is reset
                byte[] rgbBytes = ColorStorage.ColorsHSV12[i].ToRGB();
                color_num[i] = new LightColor(rgbBytes[0], rgbBytes[1], rgbBytes[2]);
            }
            anim();
            for (byte i = 0; i < 60; i++)
            {
                layer0.SetSegmentColor(0, i, color_num[axes[i][2] - 1]);
                layer0.SetSegmentColor(1, i, color_num[axes[i][2] - 1]);
                layer0.SetSegmentColor(2, i, color_num[axes[i][2] - 1]);
                layer0.SetSegmentColor(3, i, color_num[axes[i][2] - 1]);
            }
        }
        /// <summary>
        /// Sends a light configuration to the specified light controller to simulate a breathing animation.
        /// </summary>
        /// <param name="lights">
        /// The <see cref="LightController"/> instance that will receive the light animation settings.
        /// </param>
        /// <remarks>
        /// Call this method repeatedly to produce a breathing animation effect.  
        /// To reset the animation, set <see cref="f"/> back to <c>1.0</c>.
        /// </remarks>
        public static void PrepLightLoveLive(LightController lights)
        {

            for (int i = 0; i < ColorStorage.LoveLiveColorsHSV.Length; i++)
            {
                switch (color_anim)
                {
                    case 0:
                        ColorStorage.LoveLiveColorsHSV[i].V = WaccaLightAnimation.V();
                        break;
                    case 1:
                        ColorStorage.LoveLiveColorsHSV[i].H = WaccaLightAnimation.H();
                        anim();
                        break;
                    case 3:
                        ColorStorage.LoveLiveColorsHSV[i].H = WaccaLightAnimation.H();
                        break;
                    case 4:
                        SetAwesomeColors();
                        return;
                    case 5:
                        return;
                    default:
                        break;
                } // 0 is full circle, 1 is localized for each button, 2 is freeze, 3 is full circle, 4 is wacca title, 5 is off, and 6 is reset
                byte[] rgbBytes = ColorStorage.LoveLiveColorsHSV[i].ToRGB();
                color_num[i] = new LightColor(rgbBytes[0], rgbBytes[1], rgbBytes[2]);
            }
            anim();
            for (byte i = 0; i < 60; i++)
            {
                layer0.SetSegmentColor(0, i, color_num[SDVXaxesxy0125LoveLive017[i][7] - 1]);
                layer0.SetSegmentColor(1, i, color_num[SDVXaxesxy0125LoveLive017[i][7] - 1]);
                layer0.SetSegmentColor(2, i, color_num[SDVXaxesxy0125LoveLive017[i][7] - 1]);
                layer0.SetSegmentColor(3, i, color_num[SDVXaxesxy0125LoveLive017[i][7] - 1]);
            }
        }
        public static LightColor[] sdvx = { sdvx0, sdvx1, sdvx2, sdvx3, sdvx4, sdvx5, sdvx6, sdvx7, sdvx8, sdvx9, sdvx10, sdvx11, sdvx12 };
        public static void PrepLightSDVX(LightController lights, byte state)
        {
            byte[] rgbBytes;
            for (int i = 0; i < ColorStorage.SDVXColorsHSV.Length; i++)
            {

                switch (color_anim)
                {
                    case 0:
                        ColorStorage.SDVXColorsHSV[i].V = WaccaLightAnimation.V();
                        break;
                    case 1:
                        ColorStorage.SDVXColorsHSV[i].H = WaccaLightAnimation.H();
                        anim();
                        break;
                    case 3:
                        ColorStorage.SDVXColorsHSV[i].H = WaccaLightAnimation.H();
                        break;
                    case 4:
                        SetAwesomeColors();
                        return;
                    case 5:
                        return;
                    default:
                        break;
                } // 0 is full circle, 1 is localized for each button, 2 is freeze, 3 is full circle, 4 is wacca title, 5 is off, and 6 is reset
                rgbBytes = ColorStorage.SDVXColorsHSV[i].ToRGB();
                sdvx[i] = new LightColor(rgbBytes[0], rgbBytes[1], rgbBytes[2]);
            }
            anim();
            rgbBytes = ColorStorage.SDVXouterR.ToRGB();
            sdvxPink = new LightColor(rgbBytes[0], rgbBytes[1], rgbBytes[2]);
            rgbBytes = ColorStorage.SDVXouterL.ToRGB();
            sdvxBlue = new LightColor(rgbBytes[0], rgbBytes[1], rgbBytes[2]);
            if (state == 0)
            {
                sdvxPink = sdvxBlue; // full blue
            }
            else if (state == 2)
            {
                sdvxBlue = sdvxPink; // full pink
            }
            LightColor white = new LightColor(255, 255, 255);

            for (byte i = 0; i < 60; i++)
            {
                layer0.SetSegmentColor(0, i, sdvx[SDVXaxesxy0125LoveLive017[i][2] - 1]);
                layer0.SetSegmentColor(1, i, sdvx[SDVXaxesxy0125LoveLive017[i][2] - 1]);
                if (i < 30)
                {
                    layer0.SetSegmentColor(2, i, sdvxBlue);
                    layer0.SetSegmentColor(3, i, sdvxBlue);
                }
                else
                {
                    layer0.SetSegmentColor(2, i, sdvxPink);
                    layer0.SetSegmentColor(3, i, sdvxPink);
                }
            }
        }
        public static LightColor[] osu = { osu0, osu1, osu2, osu3, osu4, osu5, osu6, osu7 };
        public static void PrepLightOsu(LightController lights)
        {
            for (int i = 0; i < ColorStorage.OsuColorsHSV.Length; i++)
            {
                switch (color_anim)
                {
                    case 0:
                        ColorStorage.OsuColorsHSV[i].V = WaccaLightAnimation.V();
                        break;
                    case 1:
                        ColorStorage.OsuColorsHSV[i].H = WaccaLightAnimation.H();
                        anim();
                        break;
                    case 3:
                        ColorStorage.OsuColorsHSV[i].H = WaccaLightAnimation.H();
                        break;
                    case 4:
                        SetAwesomeColors();
                        return;
                    case 5:
                        return;
                    default:
                        break;
                } // 0 is full circle, 1 is localized for each button, 2 is freeze, 3 is full circle, 4 is wacca title, 5 is off, and 6 is reset
                byte[] rgbBytes = ColorStorage.OsuColorsHSV[i].ToRGB();
                osu[i] = new LightColor(rgbBytes[0], rgbBytes[1], rgbBytes[2]);
            }
            anim();
            for (byte i = 0; i < 60; i++)
            {
                layer0.SetSegmentColor(0, i, osu[axes[i][7] - 25]);
                layer0.SetSegmentColor(1, i, osu[axes[i][7] - 25]);
                layer0.SetSegmentColor(2, i, osu[axes[i][7] - 25]);
                layer0.SetSegmentColor(3, i, osu[axes[i][7] - 25]);
            }
        }
        public static void PrepLightMouse(LightController lights)
        {
            byte[] rgbBytes;
            for (int i = 17; i < color_num.Length; i++)
            {
                switch (color_anim)
                {
                    case 0:
                        ColorStorage.mouseHSV[i - 17].V = WaccaLightAnimation.V();
                        break;
                    case 1:
                        ColorStorage.mouseHSV[i - 17].H = WaccaLightAnimation.H();
                        anim();
                        break;
                    case 3:
                        ColorStorage.mouseHSV[i - 17].H = WaccaLightAnimation.H();
                        break;
                    case 4:
                        SetAwesomeColors();
                        return;
                    case 5:
                        return;
                    default:
                        break;
                } // 0 is full circle, 1 is localized for each button, 2 is freeze, 3 is full circle, 4 is wacca title, 5 is off, and 6 is reset
                rgbBytes = ColorStorage.mouseHSV[i - 17].ToRGB();
                color_num[i] = new LightColor(rgbBytes[0], rgbBytes[1], rgbBytes[2]);
            }
            rgbBytes = ColorStorage.mouseHSV[4].ToRGB();
            mouseOuter = new LightColor(rgbBytes[0], rgbBytes[1], rgbBytes[2]);
            anim();
            for (byte i = 0; i < 60; i++)
            {
                layer0.SetSegmentColor(0, i, color_num[mouseAxes[i][4]]);
                layer0.SetSegmentColor(1, i, color_num[mouseAxes[i][4]]);
                layer0.SetSegmentColor(2, i, mouseOuter);
                layer0.SetSegmentColor(3, i, mouseOuter);
            }
        }
        public static LightColor[] rpg = { r, rpgBack, rpgEnter, r, rpgMenu, r, r, rpgAttacc, rpgUp, rpgDown, rpgLeft, rpgRight };
        public static void PrepLightRPG(LightController lights)
        {
            for (int i = 0; i < ColorStorage.RPGColorsHSV.Length; i++)
            {
                switch (color_anim)
                {
                    case 0:
                        ColorStorage.RPGColorsHSV[i].V = WaccaLightAnimation.V();
                        break;
                    case 1:
                        ColorStorage.RPGColorsHSV[i].H = WaccaLightAnimation.H();
                        anim();
                        break;
                    case 3:
                        ColorStorage.RPGColorsHSV[i].H = WaccaLightAnimation.H();
                        break;
                    case 4:
                        SetAwesomeColors();
                        return;
                    case 5:
                        return;
                    default:
                        break;
                } // 0 is full circle, 1 is localized for each button, 2 is freeze, 3 is full circle, 4 is wacca title, 5 is off, and 6 is reset
                byte[] rgbBytes = ColorStorage.RPGColorsHSV[i].ToRGB();
                rpg[i] = new LightColor(rgbBytes[0], rgbBytes[1], rgbBytes[2]);
            }
            anim();

            for (byte i = 0; i < 60; i++)
            {
                layer0.SetSegmentColor(0, i, rpg[RPGaxes[i][3]]);
                layer0.SetSegmentColor(1, i, rpg[RPGaxes[i][3]]);
                layer0.SetSegmentColor(2, i, rpg[RPGaxes[i][4] - 13 + 8]);
                layer0.SetSegmentColor(3, i, rpg[RPGaxes[i][4] - 13 + 8]);
            }
        }
        public static void PrepLightTaiko(LightController lights)
        {
            for (int i = 12; i < 16; i++)
            {
                switch (color_anim)
                {
                    case 0:
                        ColorStorage.TaikoColorsHSV[i - 12].V = WaccaLightAnimation.V();
                        break;
                    case 1:
                        ColorStorage.TaikoColorsHSV[i - 12].H = WaccaLightAnimation.H();
                        anim();
                        break;
                    case 3:
                        ColorStorage.TaikoColorsHSV[i - 12].H = WaccaLightAnimation.H();
                        break;
                    case 4:
                        SetAwesomeColors();
                        return;
                    case 5:
                        return;
                    default:
                        break;
                } // 0 is full circle, 1 is localized for each button, 2 is freeze, 3 is full circle, 4 is wacca title, 5 is off, and 6 is reset
                byte[] rgbBytes = ColorStorage.TaikoColorsHSV[i - 12].ToRGB();
                color_num[i] = new LightColor(rgbBytes[0], rgbBytes[1], rgbBytes[2]);
            }
            anim();

            for (byte i = 0; i < 60; i++)
            {
                layer0.SetSegmentColor(0, i, color_num[axes[i][6] - 23 + 12]);
                layer0.SetSegmentColor(1, i, color_num[axes[i][6] - 23 + 12]);
                layer0.SetSegmentColor(2, i, color_num[axes[i][6] - 23 + 14]);
                layer0.SetSegmentColor(3, i, color_num[axes[i][6] - 23 + 14]);
            }
        }
        public static LightColor white;
        public static void PrepLight24(LightController lights)
        {
            byte[] rgbBytes;
            for (int i = 0; i < ColorStorage.ColorsHSV12.Length; i++)
            {
                switch (color_anim)
                {
                    case 0:
                        ColorStorage.ColorsHSV12[i].V = WaccaLightAnimation.V();
                        break;
                    case 1:
                        ColorStorage.ColorsHSV12[i].H = WaccaLightAnimation.H();
                        anim();
                        break;
                    case 3:
                        ColorStorage.ColorsHSV12[i].H = WaccaLightAnimation.H();
                        break;
                    case 4:
                        SetAwesomeColors();
                        return;
                    case 5:
                        return;
                    default:
                        break;
                } // 0 is full circle, 1 is localized for each button, 2 is freeze, 3 is full circle, 4 is wacca title, 5 is off, and 6 is reset
                rgbBytes = ColorStorage.ColorsHSV12[i].ToRGB();
                color_num[i] = new LightColor(rgbBytes[0], rgbBytes[1], rgbBytes[2]);
            }
            for (int i = 0; i < ColorStorage.SDVXColorsHSV.Length; i++)
            {
                switch (color_anim)
                {
                    case 0:
                        ColorStorage.SDVXColorsHSV[i].V = WaccaLightAnimation.V();
                        break;
                    case 1:
                        ColorStorage.SDVXColorsHSV[i].H = WaccaLightAnimation.H();
                        anim();
                        break;
                    case 3:
                        ColorStorage.SDVXColorsHSV[i].H = WaccaLightAnimation.H();
                        break;
                    case 4:
                        SetAwesomeColors();
                        return;
                    case 5:
                        return;
                    default:
                        break;
                } // 0 is full circle, 1 is localized for each button, 2 is freeze, 3 is full circle, 4 is wacca title, 5 is off, and 6 is reset
                rgbBytes = ColorStorage.SDVXColorsHSV[i].ToRGB();
                color_num[i + 12] = new LightColor(rgbBytes[0], rgbBytes[1], rgbBytes[2]);
            }
            anim();

            for (byte i = 0; i < 60; i++)
            {
                layer0.SetSegmentColor(0, i, color_num[axes[i][3] - 1]);
                layer0.SetSegmentColor(1, i, color_num[axes[i][3] - 1]);
                layer0.SetSegmentColor(2, i, color_num[axes[i][3] + 11]);
                layer0.SetSegmentColor(3, i, color_num[axes[i][3] + 11]);
            }
        }
        public static void PrepLight32(LightController lights)
        {
            byte[] rgbBytes;
            for (int i = 0; i < ColorStorage.ColorsHSV12.Length; i++)
            {
                switch (color_anim)
                {
                    case 0:
                        ColorStorage.ColorsHSV12[i].V = WaccaLightAnimation.V();
                        break;
                    case 1:
                        ColorStorage.ColorsHSV12[i].H = WaccaLightAnimation.H();
                        anim();
                        break;
                    case 3:
                        ColorStorage.ColorsHSV12[i].H = WaccaLightAnimation.H();
                        break;
                    case 4:
                        SetAwesomeColors();
                        return;
                    case 5:
                        return;
                    default:
                        break;
                } // 0 is full circle, 1 is localized for each button, 2 is freeze, 3 is full circle, 4 is wacca title, 5 is off, and 6 is reset
                rgbBytes = ColorStorage.ColorsHSV12[i].ToRGB();
                color_num[i] = new LightColor(rgbBytes[0], rgbBytes[1], rgbBytes[2]);
            }
            rgbBytes = ColorStorage.outer.ToRGB();
            white = new LightColor(rgbBytes[0], rgbBytes[1], rgbBytes[2]);
            anim();

            for (byte i = 0; i < 60; i++)
            {
                layer0.SetSegmentColor(0, i, color_num[axes[i][2] - 1]);
                layer0.SetSegmentColor(1, i, color_num[axes[i][2] - 1]);
                layer0.SetSegmentColor(2, i, white);
                layer0.SetSegmentColor(3, i, white);
            }
        }
    }
}