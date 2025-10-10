
using LilyConsole;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace WaccaCircle
{
    public struct WaccaColor
    {
        public double H { get; set; } // Hue: 0-360 degrees
        public double S { get; set; } // Saturation: 0-1
        public double V { get; set; } // Value: 0-1

        public WaccaColor(double h, double s, double v)
        {
            H = h;
            S = s;
            V = v;
        }

        /// <summary>
        /// Converts the HSV WaccaColor to an RGB WaccaColor and returns the result as a tuple of 3 bytes.
        /// </summary>
        /// <returns>A tuple containing R, G, and B as bytes (0-255).</returns>
        /// <summary>
        /// Converts the HSV WaccaColor to an RGB WaccaColor and returns the result as a byte array.
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
                double sector = (H % 360) / 60.0; // This ensures 360 becomes 0
                int sectorIndex = (int)Math.Abs(Math.Floor(sector));
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
    public class WaccaColorData
    {
        public sbyte animIndex { get; set; }
        public double animBreatheSpeedStepBetween0And1 { get; set; }
        public double animWaccaColorSpeedStepBetween0And360 { get; set; }
        public double animWaccaColorJumpStepBetween0And360 { get; set; }
        public uint animWaccaColorJumpDelayCount { get; set; }
        public byte animWaccaColorWaccaSpeedBetween0And60 { get; set; }
        public WaccaColor WaccaColor1 { get; set; }
        public WaccaColor WaccaColor2 { get; set; }
        public WaccaColor WaccaColor3 { get; set; }
        public WaccaColor WaccaColor4 { get; set; }
        public WaccaColor WaccaColor5 { get; set; }
        public WaccaColor WaccaColor6 { get; set; }
        public WaccaColor WaccaColor7 { get; set; }
        public WaccaColor WaccaColor8 { get; set; }
        public WaccaColor WaccaColor9 { get; set; }
        public WaccaColor WaccaColor10 { get; set; }
        public WaccaColor WaccaColor11 { get; set; }
        public WaccaColor WaccaColor12 { get; set; }
        public WaccaColor WaccaColor13 { get; set; }
        public WaccaColor WaccaColor14 { get; set; }
        public WaccaColor WaccaColor15 { get; set; }
        public WaccaColor WaccaColor16 { get; set; }
        public WaccaColor WaccaColor17 { get; set; }
        public WaccaColor WaccaColor18 { get; set; }
        public WaccaColor WaccaColor19 { get; set; }
        public WaccaColor WaccaColor20 { get; set; }
        public WaccaColor WaccaColor21 { get; set; }
        public WaccaColor WaccaColor22 { get; set; }
        public WaccaColor WaccaColor23 { get; set; }
        public WaccaColor WaccaColor24 { get; set; }
        public WaccaColor ArrowUp { get; set; }
        public WaccaColor ArrowRight { get; set; }
        public WaccaColor ArrowDown { get; set; }
        public WaccaColor ArrowLeft { get; set; }
        public WaccaColor LoveLiveBack12 { get; set; }
        public WaccaColor LoveLiveCoin0 { get; set; }
        public WaccaColor LoveLive1Left { get; set; }
        public WaccaColor LoveLive2 { get; set; }
        public WaccaColor LoveLive3 { get; set; }
        public WaccaColor LoveLive4 { get; set; }
        public WaccaColor LoveLive5Bottom { get; set; }
        public WaccaColor LoveLive6 { get; set; }
        public WaccaColor LoveLive7 { get; set; }
        public WaccaColor LoveLive8 { get; set; }
        public WaccaColor LoveLive9Right { get; set; }
        public WaccaColor LoveLiveStart10 { get; set; }
        public WaccaColor LoveLiveP11 { get; set; }

        public WaccaColor OuterCircleWaccaColor { get; set; }
        public WaccaColor SDVXouterL { get; set; }
        public WaccaColor SDVXouterR { get; set; }
        public WaccaColor SDVX1 { get; set; }
        public WaccaColor SDVX2 { get; set; }
        public WaccaColor SDVX3 { get; set; }
        public WaccaColor SDVX4rightFX { get; set; }
        public WaccaColor SDVX5rightLane { get; set; }
        public WaccaColor SDVX6middleRightLane { get; set; }
        public WaccaColor SDVX7middleLeftLane { get; set; }
        public WaccaColor SDVX8leftLane { get; set; }
        public WaccaColor SDVX9leftFX { get; set; }
        public WaccaColor SDVX10 { get; set; }
        public WaccaColor SDVX11 { get; set; }
        public WaccaColor SDVX12 { get; set; }
        public WaccaColor SDVX13top { get; set; }
        public WaccaColor Osu1topRight { get; set; }
        public WaccaColor Osu2 { get; set; }
        public WaccaColor Osu3 { get; set; }
        public WaccaColor Osu4BottomRight { get; set; }
        public WaccaColor Osu5BottomLeft { get; set; }
        public WaccaColor Osu6 { get; set; }
        public WaccaColor Osu7 { get; set; }
        public WaccaColor Osu8topLeft { get; set; }
        public WaccaColor MouseUp { get; set; }
        public WaccaColor MouseDown { get; set; }
        public WaccaColor MouseLeft { get; set; }
        public WaccaColor MouseRight { get; set; }
        public WaccaColor MouseOuter { get; set; }
        public WaccaColor TaikoOuterL;
        public WaccaColor TaikoInnerL;
        public WaccaColor TaikoInnerR;
        public WaccaColor TaikoOuterR;
        public WaccaColor rpgBack;
        public WaccaColor rpgEnter;
        public WaccaColor rpgMenu;
        public WaccaColor rpgAttacc;
        public WaccaColor rpgOuterUp;
        public WaccaColor rpgOuterDown;
        public WaccaColor rpgOuterLeft;
        public WaccaColor rpgOuterRight;
    }
    public static class WaccaColorStorage
    {
        public static sbyte animIndex = 0;
        public static double animBreatheSpeedStepBetween0And1 = 0.05;
        public static double animWaccaColorSpeedStepBetween0And360 = 2.0;
        public static double animWaccaColorJumpStepBetween0And360 = 15.0;
        public static uint animWaccaColorJumpDelayCount = 50;
        public static byte animWaccaColorWaccaSpeedBetween0And60 = 1;
        public static readonly WaccaColor[] WaccaColorsHSV12 =
        {
            new WaccaColor(0, 1, 1),       // Red
            new WaccaColor(30, 0.8, 1),      // Orange
            new WaccaColor(60, 1, 1),      // Yellow
            new WaccaColor(90, 0.8, 1),      // Light Green
            new WaccaColor(82, 0.92, 0.74),     // Green
            new WaccaColor(319, 0.98, 0.95),     // Cyan-Green
            new WaccaColor(82, 0.92, 0.74),     // Cyan
            new WaccaColor(210, 0.8, 1),     // Blue-Cyan
            new WaccaColor(240, 1, 1),     // Blue
            new WaccaColor(270, 0.8, 1),     // Purple
            new WaccaColor(300, 1, 1),     // Magenta
            new WaccaColor(330, 0.8, 1),     // Pink
        };
        public static readonly WaccaColor[] ArrayHSV12 =
        {
            new WaccaColor(90, 0.8, 1),      // Light Green
            new WaccaColor(120, 1, 1),     // Green
            new WaccaColor(150, 0.5, 1),     // Cyan-Green
            new WaccaColor(180, 1, 1),     // Cyan
            new WaccaColor(210, 0.5, 1),     // Blue-Cyan
            new WaccaColor(240, 1, 1),     // Blue
            new WaccaColor(270, 1, 1),     // Purple
            new WaccaColor(300, 0.6, 1),     // Magenta
            new WaccaColor(330, 1, 1),     // Pink
            new WaccaColor(0, 1, 1),       // Red
            new WaccaColor(30, 0.8, 1),      // Orange
            new WaccaColor(60, 1, 1),      // Yellow
        };
        public static readonly WaccaColor[] ArrowsHSV =
        {
            new WaccaColor(0, 1, 1),       // Red
            new WaccaColor(240, 1, 1),     // Blue
            new WaccaColor(120, 1, 1),     // Green
            new WaccaColor(60, 1, 1),      // Yellow
        };

        public static readonly WaccaColor[] LoveLiveWaccaColorsHSV =
        {
            new WaccaColor(60, 1, 1),       // Yellow coin
            new WaccaColor(30, 1, 1),      // Orange
            new WaccaColor(330, 1, 1),     // Pink
            new WaccaColor(30, 1, 1),      // Orange
            new WaccaColor(330, 1, 1),     // Pink
            new WaccaColor(30, 1, 1),      // Orange
            new WaccaColor(330, 1, 1),     // Pink
            new WaccaColor(30, 1, 1),      // Orange
            new WaccaColor(330, 1, 1),     // Pink
            new WaccaColor(30, 1, 1),      // Orange
            new WaccaColor(190, 1, 1),     // Cyan start
            new WaccaColor(270, 0.8, 1),     // Purple P
            new WaccaColor(350, 0.8, 1),     // Red Back
        };

        public static WaccaColor outer = new WaccaColor(0, 0, 1);

        public static WaccaColor SDVXouterL = new WaccaColor(200, 0.8, 1);     // Blue 
        public static WaccaColor SDVXouterR = new WaccaColor(330, 1, 1);     // Pink 

        public static readonly WaccaColor[] SDVXWaccaColorsHSV =
        {
            new WaccaColor(0, 1, 1),       // Red
            new WaccaColor(60, 1, 1),      // Yellow
            new WaccaColor(120, 1, 1),     // Green
            new WaccaColor(30, 1, 1),      // Orange
            new WaccaColor(0, 0, 0.8),      // White
            new WaccaColor(0, 0, 0.6),      // Light Grey
            new WaccaColor(0, 0, 0.4),      // Dark Grey
            new WaccaColor(0, 0, 0.8),      // White
            new WaccaColor(30, 1, 1),      // Orange
            new WaccaColor(120, 1, 1),     // Green
            new WaccaColor(60, 1, 1),      // Yellow
            new WaccaColor(0, 1, 1),       // Red
            new WaccaColor(320, 1, 1),     // Pink
        };


        public static readonly WaccaColor[] OsuWaccaColorsHSV =
        {
            new WaccaColor(00, 1, 1),      // Red
            new WaccaColor(50, 1, 1),      // Orange
            new WaccaColor(80, 1, 1),      // Yellow
            new WaccaColor(120, 1, 1),     // Green
            new WaccaColor(180, 1, 1),     // Cyan
            new WaccaColor(265, 1, 1),     // Purple
            new WaccaColor(300, 1, 1),     // Magenta
            new WaccaColor(330, 1, 1),     // Pink
        };


        public static WaccaColor[] mouseHSV = {
            new WaccaColor(0, 1, 1),       // Up
            new WaccaColor(60, 1, 1),      // Right
            new WaccaColor(120, 1, 1),     // Down
            new WaccaColor(240, 1, 1),     // Left
            new WaccaColor(0, 0, 1),     // Outer
            };

        public static readonly WaccaColor[] TaikoWaccaColorsHSV =
        {
            new WaccaColor(0, 1, 1),       // Red Outer
            new WaccaColor(230, 1, 1),      // Blue
            new WaccaColor(60, 1, 1),      // Yellow
            new WaccaColor(195, 1, 1),     // Cyan
        };
        public static readonly WaccaColor[] RPGWaccaColorsHSV =
        {
            new WaccaColor(),       // Unused
            new WaccaColor(0, 1, 1),      // back
            new WaccaColor(50, 1, 1),      // enter
            new WaccaColor(),      // Unused
            new WaccaColor(270, 1, 1),     // Menu
            new WaccaColor(),     // Unused
            new WaccaColor(),     // Unused
            new WaccaColor(175, 1, 1),     // Attacc
            new WaccaColor(0, 0, 1),     // Up
            new WaccaColor(0, 0, 1),     // Down
            new WaccaColor(0, 0, 0.7),     // Left
            new WaccaColor(0, 0, 0.7),     // Right
        };
        public static void SaveAllWaccaColors()
        {
            WaccaColorData data = new WaccaColorData
            {
                animBreatheSpeedStepBetween0And1 = animBreatheSpeedStepBetween0And1,
                animWaccaColorJumpDelayCount = animWaccaColorJumpDelayCount,
                animWaccaColorJumpStepBetween0And360 = animWaccaColorJumpStepBetween0And360,
                animWaccaColorSpeedStepBetween0And360 = animWaccaColorSpeedStepBetween0And360,
                animWaccaColorWaccaSpeedBetween0And60 = animWaccaColorWaccaSpeedBetween0And60,
                animIndex = WaccaColorStorage.animIndex,
                WaccaColor1 = WaccaColorStorage.WaccaColorsHSV12[0],
                WaccaColor2 = WaccaColorStorage.WaccaColorsHSV12[1],
                WaccaColor3 = WaccaColorStorage.WaccaColorsHSV12[2],
                WaccaColor4 = WaccaColorStorage.WaccaColorsHSV12[3],
                WaccaColor5 = WaccaColorStorage.WaccaColorsHSV12[4],
                WaccaColor6 = WaccaColorStorage.WaccaColorsHSV12[5],
                WaccaColor7 = WaccaColorStorage.WaccaColorsHSV12[6],
                WaccaColor8 = WaccaColorStorage.WaccaColorsHSV12[7],
                WaccaColor9 = WaccaColorStorage.WaccaColorsHSV12[8],
                WaccaColor10 = WaccaColorStorage.WaccaColorsHSV12[9],
                WaccaColor11 = WaccaColorStorage.WaccaColorsHSV12[10],
                WaccaColor12 = WaccaColorStorage.WaccaColorsHSV12[11],
                WaccaColor13 = ArrayHSV12[0],
                WaccaColor14 = ArrayHSV12[1],
                WaccaColor15 = ArrayHSV12[2],
                WaccaColor16 = ArrayHSV12[3],
                WaccaColor17 = ArrayHSV12[4],
                WaccaColor18 = ArrayHSV12[5],
                WaccaColor19 = ArrayHSV12[6],
                WaccaColor20 = ArrayHSV12[7],
                WaccaColor21 = ArrayHSV12[8],
                WaccaColor22 = ArrayHSV12[9],
                WaccaColor23 = ArrayHSV12[10],
                WaccaColor24 = ArrayHSV12[11],
                ArrowUp = ArrowsHSV[0],
                ArrowRight = ArrowsHSV[1],
                ArrowDown = ArrowsHSV[2],
                ArrowLeft = ArrowsHSV[3],
                LoveLiveBack12 = LoveLiveWaccaColorsHSV[12],
                LoveLiveCoin0 = LoveLiveWaccaColorsHSV[0],
                LoveLive1Left = LoveLiveWaccaColorsHSV[1],
                LoveLive2 = LoveLiveWaccaColorsHSV[2],
                LoveLive3 = LoveLiveWaccaColorsHSV[3],
                LoveLive4 = LoveLiveWaccaColorsHSV[4],
                LoveLive5Bottom = LoveLiveWaccaColorsHSV[5],
                LoveLive6 = LoveLiveWaccaColorsHSV[6],
                LoveLive7 = LoveLiveWaccaColorsHSV[7],
                LoveLive8 = LoveLiveWaccaColorsHSV[8],
                LoveLive9Right = LoveLiveWaccaColorsHSV[9],
                LoveLiveStart10 = LoveLiveWaccaColorsHSV[10],
                LoveLiveP11 = LoveLiveWaccaColorsHSV[11],
                OuterCircleWaccaColor = outer,
                SDVXouterL = SDVXouterL,
                SDVXouterR = SDVXouterR,
                SDVX1 = SDVXWaccaColorsHSV[0],
                SDVX2 = SDVXWaccaColorsHSV[1],
                SDVX3 = SDVXWaccaColorsHSV[2],
                SDVX4rightFX = SDVXWaccaColorsHSV[3],
                SDVX5rightLane = SDVXWaccaColorsHSV[4],
                SDVX6middleRightLane = SDVXWaccaColorsHSV[5],
                SDVX7middleLeftLane = SDVXWaccaColorsHSV[6],
                SDVX8leftLane = SDVXWaccaColorsHSV[7],
                SDVX9leftFX = SDVXWaccaColorsHSV[8],
                SDVX10 = SDVXWaccaColorsHSV[9],
                SDVX11 = SDVXWaccaColorsHSV[10],
                SDVX12 = SDVXWaccaColorsHSV[11],
                SDVX13top = SDVXWaccaColorsHSV[12],
                Osu1topRight = OsuWaccaColorsHSV[0],
                Osu2 = OsuWaccaColorsHSV[1],
                Osu3 = OsuWaccaColorsHSV[2],
                Osu4BottomRight = OsuWaccaColorsHSV[3],
                Osu5BottomLeft = OsuWaccaColorsHSV[4],
                Osu6 = OsuWaccaColorsHSV[5],
                Osu7 = OsuWaccaColorsHSV[6],
                Osu8topLeft = OsuWaccaColorsHSV[7],
                MouseUp = mouseHSV[0],
                MouseRight = mouseHSV[1],
                MouseDown = mouseHSV[2],
                MouseLeft = mouseHSV[3],
                MouseOuter = mouseHSV[4],
                TaikoOuterL = TaikoWaccaColorsHSV[0],
                TaikoOuterR = TaikoWaccaColorsHSV[1],
                TaikoInnerL = TaikoWaccaColorsHSV[2],
                TaikoInnerR = TaikoWaccaColorsHSV[3],
                rpgBack = RPGWaccaColorsHSV[1],
                rpgEnter = RPGWaccaColorsHSV[2],
                rpgMenu = RPGWaccaColorsHSV[4],
                rpgAttacc = RPGWaccaColorsHSV[7],
                rpgOuterUp = RPGWaccaColorsHSV[8],
                rpgOuterDown = RPGWaccaColorsHSV[9],
                rpgOuterLeft = RPGWaccaColorsHSV[10],
                rpgOuterRight = RPGWaccaColorsHSV[11],
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
                    var data = JsonConvert.DeserializeObject<WaccaColorData>(json);
                    WaccaColorStorage.animIndex = data.animIndex;
                    animBreatheSpeedStepBetween0And1 = Math.Abs(data.animBreatheSpeedStepBetween0And1);
                    animWaccaColorSpeedStepBetween0And360 = Math.Abs(data.animWaccaColorSpeedStepBetween0And360);
                    animWaccaColorJumpStepBetween0And360 = Math.Abs(data.animWaccaColorJumpStepBetween0And360);
                    animWaccaColorWaccaSpeedBetween0And60 = data.animWaccaColorWaccaSpeedBetween0And60;
                    animWaccaColorJumpDelayCount = data.animWaccaColorJumpDelayCount;
                    WaccaTable.UpdateMyAnimBasedOnList(false);
                    // regex find
                    // ([^ ]*) = ([^,]*),
                    // replace with
                    // $2 = data.$1;
                    WaccaColorStorage.WaccaColorsHSV12[0] = data.WaccaColor1;
                    WaccaColorStorage.WaccaColorsHSV12[1] = data.WaccaColor2;
                    WaccaColorStorage.WaccaColorsHSV12[2] = data.WaccaColor3;
                    WaccaColorStorage.WaccaColorsHSV12[3] = data.WaccaColor4;
                    WaccaColorStorage.WaccaColorsHSV12[4] = data.WaccaColor5;
                    WaccaColorStorage.WaccaColorsHSV12[5] = data.WaccaColor6;
                    WaccaColorStorage.WaccaColorsHSV12[6] = data.WaccaColor7;
                    WaccaColorStorage.WaccaColorsHSV12[7] = data.WaccaColor8;
                    WaccaColorStorage.WaccaColorsHSV12[8] = data.WaccaColor9;
                    WaccaColorStorage.WaccaColorsHSV12[9] = data.WaccaColor10;
                    WaccaColorStorage.WaccaColorsHSV12[10] = data.WaccaColor11;
                    WaccaColorStorage.WaccaColorsHSV12[11] = data.WaccaColor12;
                    ArrayHSV12[0] = data.WaccaColor13;
                    ArrayHSV12[1] = data.WaccaColor14;
                    ArrayHSV12[2] = data.WaccaColor15;
                    ArrayHSV12[3] = data.WaccaColor16;
                    ArrayHSV12[4] = data.WaccaColor17;
                    ArrayHSV12[5] = data.WaccaColor18;
                    ArrayHSV12[6] = data.WaccaColor19;
                    ArrayHSV12[7] = data.WaccaColor20;
                    ArrayHSV12[8] = data.WaccaColor21;
                    ArrayHSV12[9] = data.WaccaColor22;
                    ArrayHSV12[10] = data.WaccaColor23;
                    ArrayHSV12[11] = data.WaccaColor24;
                    ArrowsHSV[0] = data.ArrowUp;
                    ArrowsHSV[1] = data.ArrowRight;
                    ArrowsHSV[2] = data.ArrowDown;
                    ArrowsHSV[3] = data.ArrowLeft;
                    LoveLiveWaccaColorsHSV[12] = data.LoveLiveBack12;
                    LoveLiveWaccaColorsHSV[0] = data.LoveLiveCoin0;
                    LoveLiveWaccaColorsHSV[1] = data.LoveLive1Left;
                    LoveLiveWaccaColorsHSV[2] = data.LoveLive2;
                    LoveLiveWaccaColorsHSV[3] = data.LoveLive3;
                    LoveLiveWaccaColorsHSV[4] = data.LoveLive4;
                    LoveLiveWaccaColorsHSV[5] = data.LoveLive5Bottom;
                    LoveLiveWaccaColorsHSV[6] = data.LoveLive6;
                    LoveLiveWaccaColorsHSV[7] = data.LoveLive7;
                    LoveLiveWaccaColorsHSV[8] = data.LoveLive8;
                    LoveLiveWaccaColorsHSV[9] = data.LoveLive9Right;
                    LoveLiveWaccaColorsHSV[10] = data.LoveLiveStart10;
                    LoveLiveWaccaColorsHSV[11] = data.LoveLiveP11;
                    outer = data.OuterCircleWaccaColor;
                    SDVXouterL = data.SDVXouterL;
                    SDVXouterR = data.SDVXouterR;
                    SDVXWaccaColorsHSV[0] = data.SDVX1;
                    SDVXWaccaColorsHSV[1] = data.SDVX2;
                    SDVXWaccaColorsHSV[2] = data.SDVX3;
                    SDVXWaccaColorsHSV[3] = data.SDVX4rightFX;
                    SDVXWaccaColorsHSV[4] = data.SDVX5rightLane;
                    SDVXWaccaColorsHSV[5] = data.SDVX6middleRightLane;
                    SDVXWaccaColorsHSV[6] = data.SDVX7middleLeftLane;
                    SDVXWaccaColorsHSV[7] = data.SDVX8leftLane;
                    SDVXWaccaColorsHSV[8] = data.SDVX9leftFX;
                    SDVXWaccaColorsHSV[9] = data.SDVX10;
                    SDVXWaccaColorsHSV[10] = data.SDVX11;
                    SDVXWaccaColorsHSV[11] = data.SDVX12;
                    SDVXWaccaColorsHSV[12] = data.SDVX13top;
                    OsuWaccaColorsHSV[0] = data.Osu1topRight;
                    OsuWaccaColorsHSV[1] = data.Osu2;
                    OsuWaccaColorsHSV[2] = data.Osu3;
                    OsuWaccaColorsHSV[3] = data.Osu4BottomRight;
                    OsuWaccaColorsHSV[4] = data.Osu5BottomLeft;
                    OsuWaccaColorsHSV[5] = data.Osu6;
                    OsuWaccaColorsHSV[6] = data.Osu7;
                    OsuWaccaColorsHSV[7] = data.Osu8topLeft;
                    mouseHSV[0] = data.MouseUp;
                    mouseHSV[1] = data.MouseRight;
                    mouseHSV[2] = data.MouseDown;
                    mouseHSV[3] = data.MouseLeft;
                    mouseHSV[4] = data.MouseOuter;
                    TaikoWaccaColorsHSV[0] = data.TaikoOuterL;
                    TaikoWaccaColorsHSV[1] = data.TaikoOuterR;
                    TaikoWaccaColorsHSV[2] = data.TaikoInnerL;
                    TaikoWaccaColorsHSV[3] = data.TaikoInnerR;
                    RPGWaccaColorsHSV[1] = data.rpgBack;
                    RPGWaccaColorsHSV[2] = data.rpgEnter;
                    RPGWaccaColorsHSV[4] = data.rpgMenu;
                    RPGWaccaColorsHSV[7] = data.rpgAttacc;
                    RPGWaccaColorsHSV[8] = data.rpgOuterUp;
                    RPGWaccaColorsHSV[9] = data.rpgOuterDown;
                    RPGWaccaColorsHSV[10] = data.rpgOuterLeft;
                    RPGWaccaColorsHSV[11] = data.rpgOuterRight;
                    Console.WriteLine("Loaded WaccaCircleConfig.json!");
                    Console.WriteLine(data.animBreatheSpeedStepBetween0And1);
                }
                catch (Exception e)
                {
                    SaveAllWaccaColors();
                }
            }
            else
            {
                Console.WriteLine("WaccaCircleConfig.json not found!\nCreating config file...");
                SaveAllWaccaColors();
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
        public static double step = WaccaColorStorage.animBreatheSpeedStepBetween0And1;
        public static double WaccaColorStep = WaccaColorStorage.animWaccaColorSpeedStepBetween0And360;
        private static double jumpStep = WaccaColorStorage.animWaccaColorJumpStepBetween0And360;
        public static uint jumpDelay = WaccaColorStorage.animWaccaColorJumpDelayCount;
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
        public static double HSVWaccaColorJump()
        {
            if (f2 > jumpDelay)
            {
                f2 = 0;
                h += jumpStep;
            }
            f2++;
            if (h < 0)
            {
                h = 360.0;
            }
            else if (h >= 360.0)
            {
                h = 0.0;
            }
            return h;
        }
        public static double HSVWaccaColorJumpReverse()
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
            else if (h > 360.0)
            {
                h = 0.0;
            }
            return h;
        }
        public static double HSVWaccaColorCycle()
        {
            h += WaccaColorStep;
            if (h < 0)
            {
                h = 360.0;
            }
            else if (h >= 360.0)
            {
                h = 0.0;
            }
            return h;
        }
        public static double HSVWaccaColorCycleReverse()
        {
            h -= WaccaColorStep;
            if (h <= 0.0)
            {
                h = 360.0;
            }
            else if (h > 360.0)
            {
                h = 0.0;
            }
            return h;
        }
        public static double HSVWaccaColorAdd6()
        {
            h += 6.0;
            if (h < 0)
            {
                h = 360.0;
            }
            else if (h >= 360.0)
            {
                h = 0.0;
            }
            return h;
        }
        public static double HSVWaccaColorSub6()
        {
            h -= 6.0;
            if (h < 0)
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
            return v;
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
        private static Func<double> anim = WaccaLightAnimation.Static;
        private static readonly long axis_max = 32767;
        /* Tutorial: how to add a new animation:
         * add your func to MyAnimList below
         * add a text entry to waccaCircleText
         * change animMap -- depending on what HSV component you're changing
         * feel free to add a new axes ref inside WaccaTable, or a custom animation */
        public static byte color_anim = 0;
        public static void Initialize()
        {
            MyAnimList.Add(WaccaLightAnimation.Static);
            MyAnimList.Add(WaccaLightAnimation.HSVbreathe);
            MyAnimList.Add(WaccaLightAnimation.HSVmid);
            MyAnimList.Add(WaccaLightAnimation.HSVsineMid);
            MyAnimList.Add(WaccaLightAnimation.HSVsine);
            MyAnimList.Add(WaccaLightAnimation.HSVWaccaColorJump);
            MyAnimList.Add(WaccaLightAnimation.HSVWaccaColorJumpReverse);
            MyAnimList.Add(WaccaLightAnimation.HSVWaccaColorCycle);
            MyAnimList.Add(WaccaLightAnimation.HSVWaccaColorCycleReverse);
            MyAnimList.Add(WaccaLightAnimation.Freeze);
            // ========== FULL CIRCLE WaccaColor START ===========
            MyAnimList.Add(WaccaLightAnimation.HSVWaccaColorJump);
            MyAnimList.Add(WaccaLightAnimation.HSVWaccaColorJumpReverse);
            MyAnimList.Add(WaccaLightAnimation.HSVWaccaColorCycle);
            MyAnimList.Add(WaccaLightAnimation.HSVWaccaColorCycleReverse);
            MyAnimList.Add(WaccaLightAnimation.HSVWaccaColorAdd6);
            MyAnimList.Add(WaccaLightAnimation.HSVWaccaColorSub6);
            // ========== FULL CIRCLE WaccaColor END ===========
            MyAnimList.Add(WaccaLightAnimation.Off);
        }
        static string[] waccaCircleText = { "Static", "Breathe", "Mid-Breathe", "Sine Mid-Breathe", "Sine Breathe", "Jump",
                                        "Reverse Jump", "WaccaColor Cycle", "Reverse WaccaColor Cycle", "Freeze", "Full Circle Jump",
                                        "Full Circle Reverse Jump", "Full Circle WaccaColor Cycle", "Full Circle Reverse WaccaColor Cycle", "Wacca", "Reverse Wacca", "Off", "Custom"};

        static byte[] animMap = new byte[]
        {
                0, 0, 0, 0, 0,   // 0–4 → brightness
                1, 1, 1, 1,      // 5–8 → hue
                2,              // 9    → freeze
                3, 3, 3, 3,      // 10–13 → circle
                4, 4,           // 14–15 → Wacca
                5,              // 16   → off
                6               // 17   → reset
        };
        public static void UpdateMyAnimBasedOnList(bool display_name = true)
        {
            if (WaccaColorStorage.animIndex < 0)
            {
                WaccaColorStorage.animIndex = (sbyte)(MyAnimList.Count - 1);
            }
            if (WaccaColorStorage.animIndex >= MyAnimList.Count) {
                WaccaColorStorage.animIndex = 0;
                    }
            anim = MyAnimList[WaccaColorStorage.animIndex];  // changes the default function
            // Launch the overlay window
            if (File.Exists(WaccaCircle.exe_title) && display_name)
            {
                WaccaCircle.RunExternalCommand(WaccaCircle.exe_title, waccaCircleText[WaccaColorStorage.animIndex]);
            }
            Console.WriteLine("Changing animation");
            // Define the animation map, by index. This is a flat one-to-one lookup table.

            // Assign directly
            color_anim = animMap[WaccaColorStorage.animIndex];
            if (color_anim == 5)  // off
            {
                TurnOffTheLights();
                WaccaCircle.lights_have_been_sent_once = false;
            }
            if (color_anim == 6)   // reset
            {
                WaccaColorStorage.LoadAllColors();
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

        public static LightColor WaccaColor0;
        public static LightColor WaccaColor1;
        public static LightColor WaccaColor2;
        public static LightColor WaccaColor3;
        public static LightColor WaccaColor4;
        public static LightColor WaccaColor5;
        public static LightColor WaccaColor6;
        public static LightColor WaccaColor7;
        public static LightColor WaccaColor8;
        public static LightColor WaccaColor9;
        public static LightColor WaccaColor10;
        public static LightColor WaccaColor11;
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
        public static LightColor[] WaccaColor_num = { WaccaColor0, WaccaColor1, WaccaColor2, WaccaColor3, WaccaColor4, WaccaColor5, WaccaColor6, WaccaColor7, WaccaColor8, WaccaColor9, WaccaColor10, WaccaColor11, innerL, innerR, outerL, outerR, r, mouseUp, mouseRight, mouseDown, mouseLeft, sdvx0, sdvx1, sdvx2, sdvx3, sdvx4, sdvx5, sdvx6 };


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
        private static void SetAwesomeWaccaColors()
        {
            if (WaccaColorStorage.animIndex == 13) // forward Wacca
            {
                for (byte i = 59; i <= 0; i++)
                {
                    WaccaColorStorage.WaccaColorsHSV12[0].H = anim();
                    byte[] rgbBytes = WaccaColorStorage.WaccaColorsHSV12[0].ToRGB();
                    LightColor c = new LightColor(rgbBytes[0], rgbBytes[1], rgbBytes[2]);
                    layer0.SetSegmentColor(0, i, c);
                    layer0.SetSegmentColor(1, i, c);
                    layer0.SetSegmentColor(2, i, c);
                    layer0.SetSegmentColor(3, i, c);
                }
                for (byte i = 0; i < WaccaColorStorage.animWaccaColorWaccaSpeedBetween0And60; i++)
                {
                    anim();
                }
                return;
            }
            for (byte i = 0; i < 60; i++)  // backwards
            {
                WaccaColorStorage.WaccaColorsHSV12[0].H = anim();
                byte[] rgbBytes = WaccaColorStorage.WaccaColorsHSV12[0].ToRGB();
                LightColor c = new LightColor(rgbBytes[0], rgbBytes[1], rgbBytes[2]);
                layer0.SetSegmentColor(0, i, c);
                layer0.SetSegmentColor(1, i, c);
                layer0.SetSegmentColor(2, i, c);
                layer0.SetSegmentColor(3, i, c);
            }
            for (byte i = 0; i < WaccaColorStorage.animWaccaColorWaccaSpeedBetween0And60; i++)
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
        static public int previous = 0;
        public static void PrepLight12(LightController lights, bool onlyInner=false)
        {
            anim();
            for (int i = 0; i < WaccaColorStorage.WaccaColorsHSV12.Length; i++)
            {
                switch (color_anim)
                {
                    case 0:
                        WaccaColorStorage.WaccaColorsHSV12[i].V = WaccaLightAnimation.V();
                        break;
                    case 1:
                        WaccaColorStorage.WaccaColorsHSV12[i].H = WaccaLightAnimation.H();
                        break;
                    case 3:
                        WaccaColorStorage.WaccaColorsHSV12[i].H = WaccaLightAnimation.H();
                        break;
                    case 4:
                        SetAwesomeWaccaColors();
                        return;
                    case 5:
                        return;
                    default:
                        break;
                } // 0 is full circle, 1 is localized for each button, 2 is freeze, 3 is full circle, 4 is wacca title, 5 is off, and 6 is reset
                byte[] rgbBytes = WaccaColorStorage.WaccaColorsHSV12[i].ToRGB();
                WaccaColor_num[i] = new LightColor(rgbBytes[0], rgbBytes[1], rgbBytes[2]);
            }
            if (onlyInner)
            {
                if (color_anim == 1)
                {
                    bool previous_was_not_found = true;
                    bool first_time = true;
                    while (previous_was_not_found)
                    {
                        for (byte i = 0; i < 60; i++)
                        {
                            if ((axes[i][2] - 1) == previous)
                            {
                                previous_was_not_found = false;
                                if (!first_time)
                                {
                                    layer0.SetSegmentColor(0, i, WaccaColor_num[axes[i][2] - 1]);
                                    layer0.SetSegmentColor(1, i, WaccaColor_num[axes[i][2] - 1]);
                                }
                            }
                            if ((axes[i][2] - 1) != previous && !previous_was_not_found)
                            {
                                if (!first_time)
                                    return;
                                first_time = false;
                                previous = (axes[i][2] - 1);
                                layer0.SetSegmentColor(0, i, WaccaColor_num[axes[i][2] - 1]);
                                layer0.SetSegmentColor(1, i, WaccaColor_num[axes[i][2] - 1]);
                            }
                        }
                        if (previous_was_not_found)
                        {
                            previous = axes[0][2] - 1;
                        }
                    }
                    return;
                }
                for (byte i = 0; i < 60; i++)
                {
                    layer0.SetSegmentColor(0, i, WaccaColor_num[axes[i][2] - 1]);
                    layer0.SetSegmentColor(1, i, WaccaColor_num[axes[i][2] - 1]);
                }
                return;
            }
            if (color_anim == 1)
            {
                LightSeparateAnim(axes, 2, -1);
            }
            for (byte i = 0; i < 60; i++)
            {
                layer0.SetSegmentColor(0, i, WaccaColor_num[axes[i][2] - 1]);
                layer0.SetSegmentColor(1, i, WaccaColor_num[axes[i][2] - 1]);
                layer0.SetSegmentColor(2, i, WaccaColor_num[axes[i][2] - 1]);
                layer0.SetSegmentColor(3, i, WaccaColor_num[axes[i][2] - 1]);
            }
        }
        private static void LightSeparateAnim(int[][] axes_num, int j, int k)
        {
            bool previous_was_not_found = true;
            bool first_time = true;
            while (previous_was_not_found)
            {
                for (byte i = 0; i < 60; i++)
                {
                    if ((axes_num[i][j] + k) == previous)
                    {
                        previous_was_not_found = false;
                        if (!first_time)
                        {
                            layer0.SetSegmentColor(0, i, WaccaColor_num[axes_num[i][j] + k]);
                            layer0.SetSegmentColor(1, i, WaccaColor_num[axes_num[i][j] + k]);
                            layer0.SetSegmentColor(2, i, WaccaColor_num[axes_num[i][j] + k]);
                            layer0.SetSegmentColor(3, i, WaccaColor_num[axes_num[i][j] + k]);
                        }
                    }
                    if ((axes_num[i][j] + k) != previous && !previous_was_not_found)
                    {
                        if (!first_time)
                            return;
                        first_time = false;
                        previous = (axes_num[i][j] + k);
                        layer0.SetSegmentColor(0, i, WaccaColor_num[axes_num[i][j] + k]);
                        layer0.SetSegmentColor(1, i, WaccaColor_num[axes_num[i][j] + k]);
                        layer0.SetSegmentColor(2, i, WaccaColor_num[axes_num[i][j] + k]);
                        layer0.SetSegmentColor(3, i, WaccaColor_num[axes_num[i][j] + k]);
                    }
                }
                if (previous_was_not_found)
                {
                    previous = axes[0][2] - 1;
                }
            }
            return;
        }
        public static void PrepLightArrowsOuter(LightController lights)
        {
            byte[] rgbBytes;
            for (int i = 0; i < WaccaColorStorage.ArrowsHSV.Count(); i++)
            {
                switch (color_anim)
                {
                    case 0:
                        WaccaColorStorage.ArrowsHSV[i].V = WaccaLightAnimation.V();
                        break;
                    case 1:
                        WaccaColorStorage.ArrowsHSV[i].H = WaccaLightAnimation.H();
                        break;
                    case 3:
                        WaccaColorStorage.ArrowsHSV[i].H = WaccaLightAnimation.H();
                        break;
                    case 4:
                        SetAwesomeWaccaColors();
                        return;
                    case 5:
                        return;
                    default:
                        break;
                } // 0 is full circle, 1 is localized for each button, 2 is freeze, 3 is full circle, 4 is wacca title, 5 is off, and 6 is reset
                rgbBytes = WaccaColorStorage.ArrowsHSV[i].ToRGB();
                WaccaColor_num[i] = new LightColor(rgbBytes[0], rgbBytes[1], rgbBytes[2]);
            }
            for (byte i = 0; i < 60; i++)
            {
                layer0.SetSegmentColor(2, i, WaccaColor_num[axes[i][4] - 17]);
                layer0.SetSegmentColor(3, i, WaccaColor_num[axes[i][4] - 17]);
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
            anim();
            for (int i = 0; i < WaccaColorStorage.LoveLiveWaccaColorsHSV.Length; i++)
            {
                switch (color_anim)
                {
                    case 0:
                        WaccaColorStorage.LoveLiveWaccaColorsHSV[i].V = WaccaLightAnimation.V();
                        break;
                    case 1:
                        WaccaColorStorage.LoveLiveWaccaColorsHSV[i].H = WaccaLightAnimation.H();
                        break;
                    case 3:
                        WaccaColorStorage.LoveLiveWaccaColorsHSV[i].H = WaccaLightAnimation.H();
                        break;
                    case 4:
                        SetAwesomeWaccaColors();
                        return;
                    case 5:
                        return;
                    default:
                        break;
                } // 0 is full circle, 1 is localized for each button, 2 is freeze, 3 is full circle, 4 is wacca title, 5 is off, and 6 is reset
                byte[] rgbBytes = WaccaColorStorage.LoveLiveWaccaColorsHSV[i].ToRGB();
                WaccaColor_num[i] = new LightColor(rgbBytes[0], rgbBytes[1], rgbBytes[2]);
            }
            for (byte i = 0; i < 60; i++)
            {
                layer0.SetSegmentColor(0, i, WaccaColor_num[SDVXaxesxy0125LoveLive017[i][7] - 1]);
                layer0.SetSegmentColor(1, i, WaccaColor_num[SDVXaxesxy0125LoveLive017[i][7] - 1]);
                layer0.SetSegmentColor(2, i, WaccaColor_num[SDVXaxesxy0125LoveLive017[i][7] - 1]);
                layer0.SetSegmentColor(3, i, WaccaColor_num[SDVXaxesxy0125LoveLive017[i][7] - 1]);
            }
        }
        public static LightColor[] sdvx = { sdvx0, sdvx1, sdvx2, sdvx3, sdvx4, sdvx5, sdvx6, sdvx7, sdvx8, sdvx9, sdvx10, sdvx11, sdvx12 };
        public static void PrepLightSDVX(LightController lights, byte state)
        {
            anim();
            byte[] rgbBytes;
            for (int i = 0; i < WaccaColorStorage.SDVXWaccaColorsHSV.Length; i++)
            {

                switch (color_anim)
                {
                    case 0:
                        WaccaColorStorage.SDVXWaccaColorsHSV[i].V = WaccaLightAnimation.V();
                        break;
                    case 1:
                        WaccaColorStorage.SDVXWaccaColorsHSV[i].H = WaccaLightAnimation.H();
                        break;
                    case 3:
                        WaccaColorStorage.SDVXWaccaColorsHSV[i].H = WaccaLightAnimation.H();
                        break;
                    case 4:
                        SetAwesomeWaccaColors();
                        return;
                    case 5:
                        return;
                    default:
                        break;
                } // 0 is full circle, 1 is localized for each button, 2 is freeze, 3 is full circle, 4 is wacca title, 5 is off, and 6 is reset
                rgbBytes = WaccaColorStorage.SDVXWaccaColorsHSV[i].ToRGB();
                sdvx[i] = new LightColor(rgbBytes[0], rgbBytes[1], rgbBytes[2]);
            }
            rgbBytes = WaccaColorStorage.SDVXouterR.ToRGB();
            sdvxPink = new LightColor(rgbBytes[0], rgbBytes[1], rgbBytes[2]);
            rgbBytes = WaccaColorStorage.SDVXouterL.ToRGB();
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
            anim();
            for (int i = 0; i < WaccaColorStorage.OsuWaccaColorsHSV.Length; i++)
            {
                switch (color_anim)
                {
                    case 0:
                        WaccaColorStorage.OsuWaccaColorsHSV[i].V = WaccaLightAnimation.V();
                        break;
                    case 1:
                        WaccaColorStorage.OsuWaccaColorsHSV[i].H = WaccaLightAnimation.H();
                        break;
                    case 3:
                        WaccaColorStorage.OsuWaccaColorsHSV[i].H = WaccaLightAnimation.H();
                        break;
                    case 4:
                        SetAwesomeWaccaColors();
                        return;
                    case 5:
                        return;
                    default:
                        break;
                } // 0 is full circle, 1 is localized for each button, 2 is freeze, 3 is full circle, 4 is wacca title, 5 is off, and 6 is reset
                byte[] rgbBytes = WaccaColorStorage.OsuWaccaColorsHSV[i].ToRGB();
                osu[i] = new LightColor(rgbBytes[0], rgbBytes[1], rgbBytes[2]);
            }
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
            anim();
            byte[] rgbBytes;
            for (int i = 17; i < 21; i++)
            {
                switch (color_anim)
                {
                    case 0:
                        WaccaColorStorage.mouseHSV[i - 17].V = WaccaLightAnimation.V();
                        break;
                    case 1:
                        WaccaColorStorage.mouseHSV[i - 17].H = WaccaLightAnimation.H();
                        break;
                    case 3:
                        WaccaColorStorage.mouseHSV[i - 17].H = WaccaLightAnimation.H();
                        break;
                    case 4:
                        SetAwesomeWaccaColors();
                        return;
                    case 5:
                        return;
                    default:
                        break;
                } // 0 is full circle, 1 is localized for each button, 2 is freeze, 3 is full circle, 4 is wacca title, 5 is off, and 6 is reset
                rgbBytes = WaccaColorStorage.mouseHSV[i - 17].ToRGB();
                WaccaColor_num[i] = new LightColor(rgbBytes[0], rgbBytes[1], rgbBytes[2]);
            }
            rgbBytes = WaccaColorStorage.mouseHSV[4].ToRGB();
            mouseOuter = new LightColor(rgbBytes[0], rgbBytes[1], rgbBytes[2]);
            for (byte i = 0; i < 60; i++)
            {
                layer0.SetSegmentColor(0, i, WaccaColor_num[mouseAxes[i][4]]);
                layer0.SetSegmentColor(1, i, WaccaColor_num[mouseAxes[i][4]]);
                layer0.SetSegmentColor(2, i, mouseOuter);
                layer0.SetSegmentColor(3, i, mouseOuter);
            }
        }
        public static LightColor[] rpg = { r, rpgBack, rpgEnter, r, rpgMenu, r, r, rpgAttacc, rpgUp, rpgDown, rpgLeft, rpgRight };
        public static void PrepLightRPG(LightController lights)
        {
            anim();
            for (int i = 0; i < WaccaColorStorage.RPGWaccaColorsHSV.Length; i++)
            {
                switch (color_anim)
                {
                    case 0:
                        WaccaColorStorage.RPGWaccaColorsHSV[i].V = WaccaLightAnimation.V();
                        break;
                    case 1:
                        WaccaColorStorage.RPGWaccaColorsHSV[i].H = WaccaLightAnimation.H();
                        break;
                    case 3:
                        WaccaColorStorage.RPGWaccaColorsHSV[i].H = WaccaLightAnimation.H();
                        break;
                    case 4:
                        SetAwesomeWaccaColors();
                        return;
                    case 5:
                        return;
                    default:
                        break;
                } // 0 is full circle, 1 is localized for each button, 2 is freeze, 3 is full circle, 4 is wacca title, 5 is off, and 6 is reset
                byte[] rgbBytes = WaccaColorStorage.RPGWaccaColorsHSV[i].ToRGB();
                rpg[i] = new LightColor(rgbBytes[0], rgbBytes[1], rgbBytes[2]);
            }
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
            anim();
            for (int i = 12; i < 16; i++)
            {
                switch (color_anim)
                {
                    case 0:
                        WaccaColorStorage.TaikoWaccaColorsHSV[i - 12].V = WaccaLightAnimation.V();
                        break;
                    case 1:
                        WaccaColorStorage.TaikoWaccaColorsHSV[i - 12].H = WaccaLightAnimation.H();
                        break;
                    case 3:
                        WaccaColorStorage.TaikoWaccaColorsHSV[i - 12].H = WaccaLightAnimation.H();
                        break;
                    case 4:
                        SetAwesomeWaccaColors();
                        return;
                    case 5:
                        return;
                    default:
                        break;
                } // 0 is full circle, 1 is localized for each button, 2 is freeze, 3 is full circle, 4 is wacca title, 5 is off, and 6 is reset
                byte[] rgbBytes = WaccaColorStorage.TaikoWaccaColorsHSV[i - 12].ToRGB();
                WaccaColor_num[i] = new LightColor(rgbBytes[0], rgbBytes[1], rgbBytes[2]);
            }
            for (byte i = 0; i < 60; i++)
            {
                layer0.SetSegmentColor(0, i, WaccaColor_num[axes[i][6] - 23 + 12]);
                layer0.SetSegmentColor(1, i, WaccaColor_num[axes[i][6] - 23 + 12]);
                layer0.SetSegmentColor(2, i, WaccaColor_num[axes[i][6] - 23 + 14]);
                layer0.SetSegmentColor(3, i, WaccaColor_num[axes[i][6] - 23 + 14]);
            }
        }
        public static LightColor white;
        public static void PrepLight24(LightController lights)
        {
            anim();
            byte[] rgbBytes;
            for (int i = 0; i < WaccaColorStorage.WaccaColorsHSV12.Length; i++)
            {
                switch (color_anim)
                {
                    case 0:
                        WaccaColorStorage.WaccaColorsHSV12[i].V = WaccaLightAnimation.V();
                        break;
                    case 1:
                        WaccaColorStorage.WaccaColorsHSV12[i].H = WaccaLightAnimation.H();
                        break;
                    case 3:
                        WaccaColorStorage.WaccaColorsHSV12[i].H = WaccaLightAnimation.H();
                        break;
                    case 4:
                        SetAwesomeWaccaColors();
                        return;
                    case 5:
                        return;
                    default:
                        break;
                } // 0 is full circle, 1 is localized for each button, 2 is freeze, 3 is full circle, 4 is wacca title, 5 is off, and 6 is reset
                rgbBytes = WaccaColorStorage.WaccaColorsHSV12[i].ToRGB();
                WaccaColor_num[i] = new LightColor(rgbBytes[0], rgbBytes[1], rgbBytes[2]);
            }
            for (int i = 0; i < WaccaColorStorage.ArrayHSV12.Length; i++)
            {
                switch (color_anim)
                {
                    case 0:
                        WaccaColorStorage.ArrayHSV12[i].V = WaccaLightAnimation.V();
                        break;
                    case 1:
                        WaccaColorStorage.ArrayHSV12[i].H = WaccaLightAnimation.H();
                        break;
                    case 3:
                        WaccaColorStorage.ArrayHSV12[i].H = WaccaLightAnimation.H();
                        break;
                    case 4:
                        SetAwesomeWaccaColors();
                        return;
                    case 5:
                        return;
                    default:
                        break;
                } // 0 is full circle, 1 is localized for each button, 2 is freeze, 3 is full circle, 4 is wacca title, 5 is off, and 6 is reset
                rgbBytes = WaccaColorStorage.ArrayHSV12[i].ToRGB();
                WaccaColor_num[i + 12] = new LightColor(rgbBytes[0], rgbBytes[1], rgbBytes[2]);
            }
            for (byte i = 0; i < 60; i++)
            {
                layer0.SetSegmentColor(0, i, WaccaColor_num[axes[i][2] - 1]);
                layer0.SetSegmentColor(1, i, WaccaColor_num[axes[i][2] - 1]);
                layer0.SetSegmentColor(2, i, WaccaColor_num[axes[i][2] + 11]);
                layer0.SetSegmentColor(3, i, WaccaColor_num[axes[i][2] + 11]);
            }
        }
        public static void PrepLight32(LightController lights)
        {
            byte[] rgbBytes;
            for (int i = 0; i < WaccaColorStorage.WaccaColorsHSV12.Length; i++)
            {
                switch (color_anim)
                {
                    case 0:
                        WaccaColorStorage.WaccaColorsHSV12[i].V = WaccaLightAnimation.V();
                        break;
                    case 1:
                        WaccaColorStorage.WaccaColorsHSV12[i].H = WaccaLightAnimation.H();
                        break;
                    case 3:
                        WaccaColorStorage.WaccaColorsHSV12[i].H = WaccaLightAnimation.H();
                        break;
                    case 4:
                        SetAwesomeWaccaColors();
                        return;
                    case 5:
                        return;
                    default:
                        break;
                } // 0 is full circle, 1 is localized for each button, 2 is freeze, 3 is full circle, 4 is wacca title, 5 is off, and 6 is reset
                rgbBytes = WaccaColorStorage.WaccaColorsHSV12[i].ToRGB();
                WaccaColor_num[i] = new LightColor(rgbBytes[0], rgbBytes[1], rgbBytes[2]);
            }
            rgbBytes = WaccaColorStorage.outer.ToRGB();
            white = new LightColor(rgbBytes[0], rgbBytes[1], rgbBytes[2]);
            anim();

            for (byte i = 0; i < 60; i++)
            {
                layer0.SetSegmentColor(0, i, WaccaColor_num[axes[i][2] - 1]);
                layer0.SetSegmentColor(1, i, WaccaColor_num[axes[i][2] - 1]);
                layer0.SetSegmentColor(2, i, white);
                layer0.SetSegmentColor(3, i, white);
            }
        }
    }
}