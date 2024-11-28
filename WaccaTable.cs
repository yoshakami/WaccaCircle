
using LilyConsole;
using SharpDX.DirectInput;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace WaccaCircle
{
    public static class WaccaTable
    {
        public static readonly long axis_max = 32767;
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
        public static readonly int[][] SDVXaxes =
            {      // SDVX  x axis    y-axis   1-13  13-16  17-20  21-22  23-24  25-32
                new int[] { X(0.5),   Y(30.5),  13,   16,    17,    21,    23,    25},    // 0  top circle
                new int[] { X(1.5),   Y(31.5),  13,   16,    17,    21,    23,    25},    // 1
                new int[] { X(2.5),   Y(32.5),  13,   16,    17,    21,    23,    25},    // 2
                new int[] { X(3.5),   Y(33.5),  12,   16,    17,    21,    23,    25},    // 3
                new int[] { X(4.5),   Y(34.5),  12,   16,    17,    21,    23,    25},    // 4
                new int[] { X(5.5),   Y(35.5),  12,   16,    17,    21,    23,    25},    // 5
                new int[] { X(6.5),   Y(36.5),  12,   16,    17,    21,    23,    25},    // 6
                new int[] { X(7.5),   Y(37.5),  12,   16,    17,    21,    23,    25},    // 7
                new int[] { X(8.5),   Y(38.5),  11,   16,    20,    21,    23,    26},    // 8
                new int[] { X(9.5),   Y(39.5),  11,   16,    20,    21,    23,    26},    // 9
                new int[] { X(10.5),  Y(40.5),  11,   16,    20,    21,    23,    26},    // 10
                new int[] { X(11.5),  Y(41.5),  11,   16,    20,    21,    23,    26},    // 11
                new int[] { X(12.5),  Y(42.5),  11,   16,    20,    21,    23,    26},    // 12
                new int[] { X(13.5),  Y(43.5),  10,   16,    20,    21,    23,    26},    // 13
                new int[] { X(14.5),  Y(44.5),  10,   16,    20,    21,    23,    26},    // 14  left
                new int[] { X(45.5),  Y(45.5),  10,   15,    20,    22,    23,    26},    // 15  left 
                new int[] { X(46.5),  Y(46.5),  10,   15,    20,    22,    23,    27},    // 16
                new int[] { X(47.5),  Y(47.5),  10,   15,    20,    22,    23,    27},    // 17
                new int[] { X(48.5),  Y(48.5),   9,   15,    20,    22,    23,    27},    // 18
                new int[] { X(49.5),  Y(49.5),   9,   15,    20,    22,    23,    27},    // 19
                new int[] { X(50.5),  Y(50.5),   9,   15,    20,    22,    23,    27},    // 20
                new int[] { X(51.5),  Y(51.5),   9,   15,    20,    22,    23,    27},    // 21
                new int[] { X(52.5),  Y(52.5),   9,   15,    20,    22,    23,    27},    // 22
                new int[] { X(53.5),  Y(53.5),   8,   15,    19,    22,    23,    28},    // 23
                new int[] { X(54.5),  Y(54.5),   8,   15,    19,    22,    23,    28},    // 24
                new int[] { X(55.5),  Y(55.5),   8,   15,    19,    22,    23,    28},    // 25
                new int[] { X(56.5),  Y(56.5),   8,   15,    19,    22,    23,    28},    // 26
                new int[] { X(57.5),  Y(57.5),   7,   15,    19,    22,    23,    28},    // 27
                new int[] { X(58.5),  Y(58.5),   7,   15,    19,    22,    23,    28},    // 28
                new int[] { X(59.5),  Y(59.5),   7,   15,    19,    22,    23,    28},    // 29  bottom
                new int[] { X(00.5),  Y(30.5),   6,   14,    19,    22,    24,    29},    // 30  bottom
                new int[] { X(01.5),  Y(31.5),   6,   14,    19,    22,    24,    29},    // 31
                new int[] { X(02.5),  Y(32.5),   6,   14,    19,    22,    24,    29},    // 32
                new int[] { X(03.5),  Y(33.5),   5,   14,    19,    22,    24,    29},    // 33
                new int[] { X(04.5),  Y(34.5),   5,   14,    19,    22,    24,    29},    // 34
                new int[] { X(05.5),  Y(35.5),   5,   14,    19,    22,    24,    29},    // 35
                new int[] { X(06.5),  Y(36.5),   5,   14,    19,    22,    24,    29},    // 36
                new int[] { X(07.5),  Y(37.5),   4,   14,    19,    22,    24,    30},    // 37
                new int[] { X(08.5),  Y(38.5),   4,   14,    18,    22,    24,    30},    // 38
                new int[] { X(09.5),  Y(39.5),   4,   14,    18,    22,    24,    30},    // 39
                new int[] { X(10.5),  Y(40.5),   4,   14,    18,    22,    24,    30},    // 40
                new int[] { X(11.5),  Y(41.5),   4,   14,    18,    22,    24,    30},    // 41
                new int[] { X(12.5),  Y(42.5),   4,   14,    18,    22,    24,    30},    // 42
                new int[] { X(13.5),  Y(43.5),   3,   14,    18,    22,    24,    30},    // 43
                new int[] { X(14.5),  Y(44.5),   3,   14,    18,    22,    24,    31},    // 44  right
                new int[] { X(45.5),  Y(45.5),   3,   13,    18,    21,    24,    31},    // 45  right
                new int[] { X(46.5),  Y(46.5),   3,   13,    18,    21,    24,    31},    // 46
                new int[] { X(47.5),  Y(47.5),   3,   13,    18,    21,    24,    31},    // 47
                new int[] { X(48.5),  Y(48.5),   2,   13,    18,    21,    24,    31},    // 48
                new int[] { X(49.5),  Y(49.5),   2,   13,    18,    21,    24,    31},    // 49
                new int[] { X(50.5),  Y(50.5),   2,   13,    18,    21,    24,    31},    // 50
                new int[] { X(51.5),  Y(51.5),   2,   13,    18,    21,    24,    31},    // 51
                new int[] { X(52.5),  Y(52.5),   2,   13,    18,    21,    24,    32},    // 52
                new int[] { X(53.5),  Y(53.5),   1,   13,    17,    21,    24,    32},    // 53
                new int[] { X(54.5),  Y(54.5),   1,   13,    17,    21,    24,    32},    // 54
                new int[] { X(55.5),  Y(55.5),   1,   13,    17,    21,    24,    32},    // 55
                new int[] { X(56.5),  Y(56.5),   1,   13,    17,    21,    24,    32},    // 56
                new int[] { X(57.5),  Y(57.5),   1,   13,    17,    21,    24,    32},    // 57
                new int[] { X(58.5),  Y(58.5),  13,   13,    17,    21,    24,    32},    // 58
                new int[] { X(59.5),  Y(59.5),  13,   13,    17,    21,    24,    32},    // 59  top circle
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
        /*public static readonly LightFrame whiteFrame060 = new LightFrame(new LightColor[]
                {
                    new LightColor(0, 0, 0),
                    new LightColor(1, 1, 1),
                    new LightColor(2, 2, 2),
                    new LightColor(3, 3, 3),
                    new LightColor(4, 4, 4),
                    new LightColor(5, 5, 5),
                    new LightColor(6, 6, 6),
                    new LightColor(7, 7, 7),
                    new LightColor(8, 8, 8),
                    new LightColor(9, 9, 9),
                    new LightColor(10, 10, 10),
                    new LightColor(11, 11, 11),
                    new LightColor(12, 12, 12),
                    new LightColor(13, 13, 13),
                    new LightColor(14, 14, 14),
                    new LightColor(15, 15, 15),
                    new LightColor(16, 16, 16),
                    new LightColor(17, 17, 17),
                    new LightColor(18, 18, 18),
                    new LightColor(19, 19, 19),
                    new LightColor(20, 20, 20),
                    new LightColor(21, 21, 21),
                    new LightColor(22, 22, 22),
                    new LightColor(23, 23, 23),
                    new LightColor(24, 24, 24),
                    new LightColor(25, 25, 25),
                    new LightColor(26, 26, 26),
                    new LightColor(27, 27, 27),
                    new LightColor(28, 28, 28),
                    new LightColor(29, 29, 29),
                    new LightColor(30, 30, 30),
                    new LightColor(31, 31, 31),
                    new LightColor(32, 32, 32),
                    new LightColor(33, 33, 33),
                    new LightColor(34, 34, 34),
                    new LightColor(35, 35, 35),
                    new LightColor(36, 36, 36),
                    new LightColor(37, 37, 37),
                    new LightColor(38, 38, 38),
                    new LightColor(39, 39, 39),
                    new LightColor(40, 40, 40),
                    new LightColor(41, 41, 41),
                    new LightColor(42, 42, 42),
                    new LightColor(43, 43, 43),
                    new LightColor(44, 44, 44),
                    new LightColor(45, 45, 45),
                    new LightColor(46, 46, 46),
                    new LightColor(47, 47, 47),
                    new LightColor(48, 48, 48),
                    new LightColor(49, 49, 49),
                    new LightColor(50, 50, 50),
                    new LightColor(51, 51, 51),
                    new LightColor(52, 52, 52),
                    new LightColor(53, 53, 53),
                    new LightColor(54, 54, 54),
                    new LightColor(55, 55, 55),
                    new LightColor(56, 56, 56),
                    new LightColor(57, 57, 57),
                    new LightColor(58, 58, 58),
                    new LightColor(59, 59, 59),
            });*/
        public static void SendWhiteLightFrame(LightController lights)
        {
            LightColor[] l0 = new LightColor[]
            {
                new LightColor(0, 0, 0),
new LightColor(1, 1, 1),
new LightColor(2, 2, 2),
new LightColor(3, 3, 3),
new LightColor(4, 4, 4),
new LightColor(5, 5, 5),
new LightColor(6, 6, 6),
new LightColor(7, 7, 7),
new LightColor(8, 8, 8),
new LightColor(9, 9, 9),
new LightColor(10, 10, 10),
new LightColor(11, 11, 11),
new LightColor(12, 12, 12),
new LightColor(13, 13, 13),
new LightColor(14, 14, 14),
new LightColor(15, 15, 15),
new LightColor(16, 16, 16),
new LightColor(17, 17, 17),
new LightColor(18, 18, 18),
new LightColor(19, 19, 19),
new LightColor(20, 20, 20),
new LightColor(21, 21, 21),
new LightColor(22, 22, 22),
new LightColor(23, 23, 23),
new LightColor(24, 24, 24),
new LightColor(25, 25, 25),
new LightColor(26, 26, 26),
new LightColor(27, 27, 27),
new LightColor(28, 28, 28),
new LightColor(29, 29, 29),
new LightColor(30, 30, 30),
new LightColor(31, 31, 31),
new LightColor(32, 32, 32),
new LightColor(33, 33, 33),
new LightColor(34, 34, 34),
new LightColor(35, 35, 35),
new LightColor(36, 36, 36),
new LightColor(37, 37, 37),
new LightColor(38, 38, 38),
new LightColor(39, 39, 39),
new LightColor(40, 40, 40),
new LightColor(41, 41, 41),
new LightColor(42, 42, 42),
new LightColor(43, 43, 43),
new LightColor(44, 44, 44),
new LightColor(45, 45, 45),
new LightColor(46, 46, 46),
new LightColor(47, 47, 47),
new LightColor(48, 48, 48),
new LightColor(49, 49, 49),
new LightColor(50, 50, 50),
new LightColor(51, 51, 51),
new LightColor(52, 52, 52),
new LightColor(53, 53, 53),
new LightColor(54, 54, 54),
new LightColor(55, 55, 55),
new LightColor(56, 56, 56),
new LightColor(57, 57, 57),
new LightColor(58, 58, 58),
new LightColor(59, 59, 59),}
            ;
            LightColor[] l1 = new LightColor[] {
new LightColor(60, 60, 60),
new LightColor(61, 61, 61),
new LightColor(62, 62, 62),
new LightColor(63, 63, 63),
new LightColor(64, 64, 64),
new LightColor(65, 65, 65),
new LightColor(66, 66, 66),
new LightColor(67, 67, 67),
new LightColor(68, 68, 68),
new LightColor(69, 69, 69),
new LightColor(70, 70, 70),
new LightColor(71, 71, 71),
new LightColor(72, 72, 72),
new LightColor(73, 73, 73),
new LightColor(74, 74, 74),
new LightColor(75, 75, 75),
new LightColor(76, 76, 76),
new LightColor(77, 77, 77),
new LightColor(78, 78, 78),
new LightColor(79, 79, 79),
new LightColor(80, 80, 80),
new LightColor(81, 81, 81),
new LightColor(82, 82, 82),
new LightColor(83, 83, 83),
new LightColor(84, 84, 84),
new LightColor(85, 85, 85),
new LightColor(86, 86, 86),
new LightColor(87, 87, 87),
new LightColor(88, 88, 88),
new LightColor(89, 89, 89),
new LightColor(90, 90, 90),
new LightColor(91, 91, 91),
new LightColor(92, 92, 92),
new LightColor(93, 93, 93),
new LightColor(94, 94, 94),
new LightColor(95, 95, 95),
new LightColor(96, 96, 96),
new LightColor(97, 97, 97),
new LightColor(98, 98, 98),
new LightColor(99, 99, 99),
new LightColor(100, 100, 100),
new LightColor(101, 101, 101),
new LightColor(102, 102, 102),
new LightColor(103, 103, 103),
new LightColor(104, 104, 104),
new LightColor(105, 105, 105),
new LightColor(106, 106, 106),
new LightColor(107, 107, 107),
new LightColor(108, 108, 108),
new LightColor(109, 109, 109),
new LightColor(110, 110, 110),
new LightColor(111, 111, 111),
new LightColor(112, 112, 112),
new LightColor(113, 113, 113),
new LightColor(114, 114, 114),
new LightColor(115, 115, 115),
new LightColor(116, 116, 116),
new LightColor(117, 117, 117),
new LightColor(118, 118, 118),
new LightColor(119, 119, 119), };

            LightColor[] l2 = new LightColor[]
            {

new LightColor(120, 120, 120),
new LightColor(121, 121, 121),
new LightColor(122, 122, 122),
new LightColor(123, 123, 123),
new LightColor(124, 124, 124),
new LightColor(125, 125, 125),
new LightColor(126, 126, 126),
new LightColor(127, 127, 127),
new LightColor(128, 128, 128),
new LightColor(129, 129, 129),
new LightColor(130, 130, 130),
new LightColor(131, 131, 131),
new LightColor(132, 132, 132),
new LightColor(133, 133, 133),
new LightColor(134, 134, 134),
new LightColor(135, 135, 135),
new LightColor(136, 136, 136),
new LightColor(137, 137, 137),
new LightColor(138, 138, 138),
new LightColor(139, 139, 139),
new LightColor(140, 140, 140),
new LightColor(141, 141, 141),
new LightColor(142, 142, 142),
new LightColor(143, 143, 143),
new LightColor(144, 144, 144),
new LightColor(145, 145, 145),
new LightColor(146, 146, 146),
new LightColor(147, 147, 147),
new LightColor(148, 148, 148),
new LightColor(149, 149, 149),
new LightColor(150, 150, 150),
new LightColor(151, 151, 151),
new LightColor(152, 152, 152),
new LightColor(153, 153, 153),
new LightColor(154, 154, 154),
new LightColor(155, 155, 155),
new LightColor(156, 156, 156),
new LightColor(157, 157, 157),
new LightColor(158, 158, 158),
new LightColor(159, 159, 159),
new LightColor(160, 160, 160),
new LightColor(161, 161, 161),
new LightColor(162, 162, 162),
new LightColor(163, 163, 163),
new LightColor(164, 164, 164),
new LightColor(165, 165, 165),
new LightColor(166, 166, 166),
new LightColor(167, 167, 167),
new LightColor(168, 168, 168),
new LightColor(169, 169, 169),
new LightColor(170, 170, 170),
new LightColor(171, 171, 171),
new LightColor(172, 172, 172),
new LightColor(173, 173, 173),
new LightColor(174, 174, 174),
new LightColor(175, 175, 175),
new LightColor(176, 176, 176),
new LightColor(177, 177, 177),
new LightColor(178, 178, 178),
new LightColor(179, 179, 179),
            };
            LightColor[] l3 = new LightColor[]
        {

        new LightColor(180, 180, 180),
        new LightColor(181, 181, 181),
        new LightColor(182, 182, 182),
        new LightColor(183, 183, 183),
        new LightColor(184, 184, 184),
        new LightColor(185, 185, 185),
        new LightColor(186, 186, 186),
        new LightColor(187, 187, 187),
        new LightColor(188, 188, 188),
        new LightColor(189, 189, 189),
        new LightColor(190, 190, 190),
        new LightColor(191, 191, 191),
        new LightColor(192, 192, 192),
        new LightColor(193, 193, 193),
        new LightColor(194, 194, 194),
        new LightColor(195, 195, 195),
        new LightColor(196, 196, 196),
        new LightColor(197, 197, 197),
        new LightColor(198, 198, 198),
        new LightColor(199, 199, 199),
        new LightColor(200, 200, 200),
        new LightColor(201, 201, 201),
        new LightColor(202, 202, 202),
        new LightColor(203, 203, 203),
        new LightColor(204, 204, 204),
        new LightColor(205, 205, 205),
        new LightColor(206, 206, 206),
        new LightColor(207, 207, 207),
        new LightColor(208, 208, 208),
        new LightColor(209, 209, 209),
        new LightColor(210, 210, 210),
        new LightColor(211, 211, 211),
        new LightColor(212, 212, 212),
        new LightColor(213, 213, 213),
        new LightColor(214, 214, 214),
        new LightColor(215, 215, 215),
        new LightColor(216, 216, 216),
        new LightColor(217, 217, 217),
        new LightColor(218, 218, 218),
        new LightColor(219, 219, 219),
        new LightColor(220, 220, 220),
        new LightColor(221, 221, 221),
        new LightColor(222, 222, 222),
        new LightColor(223, 223, 223),
        new LightColor(224, 224, 224),
        new LightColor(225, 225, 225),
        new LightColor(226, 226, 226),
        new LightColor(227, 227, 227),
        new LightColor(228, 228, 228),
        new LightColor(229, 229, 229),
        new LightColor(230, 230, 230),
        new LightColor(231, 231, 231),
        new LightColor(232, 232, 232),
        new LightColor(233, 233, 233),
        new LightColor(234, 234, 234),
        new LightColor(235, 235, 235),
        new LightColor(236, 236, 236),
        new LightColor(237, 237, 237),
        new LightColor(238, 238, 238),
        new LightColor(239, 239, 239),
        };
            LightLayer layer0 = new LightLayer();
            for (byte i = 0; i < 60; i++)
            {
                layer0.SetSegmentColor(0, i, l0[i]);
                layer0.SetSegmentColor(1, i, l1[i]);
                layer0.SetSegmentColor(2, i, l2[i]);
                layer0.SetSegmentColor(3, i, l3[i]);
            }

            LightFrame gradientFrame = new LightFrame
            {
                layers =
                {
                    [0] = layer0,
                }
            };


            lights.SendLightFrame(gradientFrame);
        }
        
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
        public static LightColor[] color_num = { color0, color1, color2, color3, color4, color5, color6, color7, color8, color9, color10, color11, innerL, innerR, outerL, outerR, r, mouseUp, mouseRight, mouseDown, mouseLeft };
        public static LightColor[] osu = { osu0, osu1, osu2, osu3, osu4, osu5, osu6, osu7 };
        public static LightColor[] sdvx = { sdvx0, sdvx1, sdvx2, sdvx3, sdvx4, sdvx5, sdvx6, sdvx7, sdvx8, sdvx9, sdvx10, sdvx11, sdvx12 };
        public static LightColor[] rpg = { r, rpgBack, rpgEnter, r, rpgMenu, r, r, rpgAttacc, rpgUp, rpgDown, rpgLeft, rpgRight };

        /// <summary>
        /// Represents the current state of the breathing animation factor.
        /// </summary>
        /// <remarks>
        /// This value should be reset to <c>1.0</c> to restart the breathing animation.
        /// </remarks>
        public static double f = 1.0;
        public static void ChangeF()
        {
            f += 0.1;
            if (f >= -1.1 && f < 1.0)
            {
                f = 1.0;  // reset f
            }
            if (f > 2.0 || f < -2.0)
            {
                f += 1.0;  // accelerate anim
            }
            if (f > 20.0)
            {
                f = -20.0;  // reverse anim
            }
        }

        public static int[][] colors12 =
        {
            new int[] { 255, 0, 0 },
            new int[] { 255, 128, 0 },
            new int[] { 255, 255, 0 },
            new int[] { 128, 255, 0 },
            new int[] { 0, 255, 0 },
            new int[] { 0, 255, 128 },
            new int[] { 0, 255, 255 },
            new int[] { 0, 128, 255 },
            new int[] { 0, 0, 255 },
            new int[] { 128, 0, 255 },
            new int[] { 255, 0, 255 },
            new int[] { 255, 0, 128 },
        };

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
        public static void SendLight12(LightController lights)
        {
            byte n = 0;
            color0 = new LightColor((byte)(colors12[n][0] / f), (byte)(colors12[n][1] / f), (byte)(colors12[n][2] / f));
            n++;
            color1 = new LightColor((byte)(colors12[n][0] / f), (byte)(colors12[n][1] / f), (byte)(colors12[n][2] / f));
            n++;
            color2 = new LightColor((byte)(colors12[n][0] / f), (byte)(colors12[n][1] / f), (byte)(colors12[n][2] / f));
            n++;
            color3 = new LightColor((byte)(colors12[n][0] / f), (byte)(colors12[n][1] / f), (byte)(colors12[n][2] / f));
            n++;
            color4 = new LightColor((byte)(colors12[n][0] / f), (byte)(colors12[n][1] / f), (byte)(colors12[n][2] / f));
            n++;
            color5 = new LightColor((byte)(colors12[n][0] / f), (byte)(colors12[n][1] / f), (byte)(colors12[n][2] / f));
            n++;
            color6 = new LightColor((byte)(colors12[n][0] / f), (byte)(colors12[n][1] / f), (byte)(colors12[n][2] / f));
            n++;
            color7 = new LightColor((byte)(colors12[n][0] / f), (byte)(colors12[n][1] / f), (byte)(colors12[n][2] / f));
            n++;
            color8 = new LightColor((byte)(colors12[n][0] / f), (byte)(colors12[n][1] / f), (byte)(colors12[n][2] / f));
            n++;
            color9 = new LightColor((byte)(colors12[n][0] / f), (byte)(colors12[n][1] / f), (byte)(colors12[n][2] / f));
            n++;
            color10 = new LightColor((byte)(colors12[n][0] / f), (byte)(colors12[n][1] / f), (byte)(colors12[n][2] / f));
            n++;
            color11 = new LightColor((byte)(colors12[n][0] / f), (byte)(colors12[n][1] / f), (byte)(colors12[n][2] / f));
        LightColor[] color_num = { color0, color1, color2, color3, color4, color5, color6, color7, color8, color9, color10, color11, innerL, innerR, outerL, outerR, r, mouseUp, mouseRight, mouseDown, mouseLeft };
        ChangeF();
            LightLayer layer0 = new LightLayer();
            for (byte i = 0; i < 60; i++)
            {
                    layer0.SetSegmentColor(0, i, color_num[axes[i][2] - 1]);
                    layer0.SetSegmentColor(1, i, color_num[axes[i][2] - 1]);
                    layer0.SetSegmentColor(2, i, color_num[axes[i][2] - 1]);
                    layer0.SetSegmentColor(3, i, color_num[axes[i][2] - 1]);
            }

            LightFrame gradientFrame = new LightFrame { layers = { [0] = layer0, } };
            lights.SendLightFrame(gradientFrame);
        }
        public static void SendLightSDVX(LightController lights, byte state)
        {
            sdvx0 = new LightColor((byte)(255 / f), (byte)(128 / f), (byte)(128 / f));
            sdvx1 = new LightColor((byte)(100 / f), (byte)(64 / f), (byte)(255 / f));
            sdvx2 = new LightColor((byte)(255 / f), (byte)(128 / f), (byte)(128 / f));
            sdvx3 = new LightColor((byte)(100 / f), (byte)(64 / f), (byte)(255 / f));
            sdvx4 = new LightColor((byte)(100 / f), (byte)(64 / f), (byte)(255 / f));
            sdvx5 = new LightColor((byte)(100 / f), (byte)(64 / f), (byte)(255 / f));
            sdvx6 = new LightColor((byte)(100 / f), (byte)(64 / f), (byte)(255 / f));
            sdvx7 = new LightColor((byte)(100 / f), (byte)(64 / f), (byte)(255 / f));
            sdvx8 = new LightColor((byte)(100 / f), (byte)(64 / f), (byte)(255 / f));
            sdvx9 = new LightColor((byte)(100 / f), (byte)(64 / f), (byte)(255 / f));
            sdvx10 = new LightColor((byte)(100 / f), (byte)(64 / f), (byte)(255 / f));
            sdvx11 = new LightColor((byte)(100 / f), (byte)(64 / f), (byte)(255 / f));
            sdvx12 = new LightColor((byte)(100 / f), (byte)(64 / f), (byte)(255 / f));
            sdvxPink = new LightColor((byte)(255 / f), (byte)(128 / f), (byte)(128 / f));
            sdvxBlue = new LightColor((byte)(100 / f), (byte)(64 / f), (byte)(255 / f));
            LightColor[] sdvx = { sdvx0, sdvx1, sdvx2, sdvx3, sdvx4, sdvx5, sdvx6, sdvx7, sdvx8, sdvx9, sdvx10, sdvx11, sdvx12 };
            if (state == 0)
            {
                sdvxPink = sdvxBlue; // full blue
            }
            else if (state == 2)
            {
                sdvxBlue = sdvxPink; // full pink
            }
            ChangeF();

            LightLayer layer0 = new LightLayer();
            for (byte i = 0; i < 60; i++)
            {
                layer0.SetSegmentColor(0, i, sdvx[SDVXaxes[i][2] - 1]);
                layer0.SetSegmentColor(1, i, sdvx[SDVXaxes[i][2] - 1]);
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

            LightFrame gradientFrame = new LightFrame { layers = { [0] = layer0, } };
            lights.SendLightFrame(gradientFrame);
        }

        public static void SendLightOsu(LightController lights)
        {
            osu0 = new LightColor((byte)(255 / f), (byte)(128 / f), (byte)(128 / f));
            osu1 = new LightColor((byte)(100 / f), (byte)(64 / f), (byte)(255 / f));
            osu2 = new LightColor((byte)(255 / f), (byte)(128 / f), (byte)(128 / f));
            osu3 = new LightColor((byte)(100 / f), (byte)(64 / f), (byte)(255 / f));
            osu4 = new LightColor((byte)(100 / f), (byte)(64 / f), (byte)(255 / f));
            osu5 = new LightColor((byte)(100 / f), (byte)(64 / f), (byte)(255 / f));
            osu6 = new LightColor((byte)(100 / f), (byte)(64 / f), (byte)(255 / f));
            osu7 = new LightColor((byte)(100 / f), (byte)(64 / f), (byte)(255 / f));
            ChangeF();

            LightLayer layer0 = new LightLayer();
            for (byte i = 0; i < 60; i++)
            {
                layer0.SetSegmentColor(0, i, osu[axes[i][7] - 25]);
                layer0.SetSegmentColor(1, i, osu[axes[i][7] - 25]);
                layer0.SetSegmentColor(2, i, osu[axes[i][7] - 25]);
                layer0.SetSegmentColor(3, i, osu[axes[i][7] - 25]);
            }

            LightFrame gradientFrame = new LightFrame { layers = { [0] = layer0, } };
            lights.SendLightFrame(gradientFrame);
        }
        public static void SendLightMouse(LightController lights)
        {
            mouseUp = new LightColor((byte)(255 / f), (byte)(128 / f), (byte)(128 / f));
            mouseDown = new LightColor((byte)(100 / f), (byte)(64 / f), (byte)(255 / f));
            mouseLeft = new LightColor((byte)(255 / f), (byte)(128 / f), (byte)(128 / f));
            mouseRight = new LightColor((byte)(255 / f), (byte)(64 / f), (byte)(255 / f));
            mouseOuter = new LightColor((byte)(255 / f), (byte)(255 / f), (byte)(255 / f));
            ChangeF();

            LightLayer layer0 = new LightLayer();
            for (byte i = 0; i < 60; i++)
            {
                layer0.SetSegmentColor(0, i, color_num[mouseAxes[i][4]]);
                layer0.SetSegmentColor(1, i, color_num[mouseAxes[i][4]]);
                layer0.SetSegmentColor(2, i, mouseOuter);
                layer0.SetSegmentColor(3, i, mouseOuter);
            }

            LightFrame gradientFrame = new LightFrame { layers = { [0] = layer0, } };
            lights.SendLightFrame(gradientFrame);
        }
        public static void SendLightRPG(LightController lights)
        {
            rpgBack = new LightColor((byte)(255 / f), (byte)(128 / f), (byte)(128 / f));
            rpgEnter = new LightColor((byte)(100 / f), (byte)(64 / f), (byte)(255 / f));
            rpgMenu = new LightColor((byte)(100 / f), (byte)(64 / f), (byte)(255 / f));
            rpgAttacc = new LightColor((byte)(255 / f), (byte)(128 / f), (byte)(128 / f));
            rpgUp = new LightColor((byte)(255 / f), (byte)(128 / f), (byte)(128 / f));
            rpgDown = new LightColor((byte)(255 / f), (byte)(128 / f), (byte)(128 / f));
            rpgLeft = new LightColor((byte)(255 / f), (byte)(128 / f), (byte)(128 / f));
            rpgRight = new LightColor((byte)(255 / f), (byte)(128 / f), (byte)(128 / f));
            ChangeF();

            LightLayer layer0 = new LightLayer();
            for (byte i = 0; i < 60; i++)
            {
                layer0.SetSegmentColor(0, i, rpg[RPGaxes[i][3]]);
                layer0.SetSegmentColor(1, i, rpg[RPGaxes[i][3]]);
                layer0.SetSegmentColor(2, i, rpg[RPGaxes[i][4] - 13 + 8]);
                layer0.SetSegmentColor(3, i, rpg[RPGaxes[i][4] - 13 + 8]);
            }

            LightFrame gradientFrame = new LightFrame { layers = { [0] = layer0, } };
            lights.SendLightFrame(gradientFrame);
        }
        public static void SendLightTaiko(LightController lights)
        {
            outerL = new LightColor((byte)(255 / f), (byte)(128 / f), (byte)(128 / f));
            innerL = new LightColor((byte)(100 / f), (byte)(64 / f), (byte)(255 / f));
            innerR = new LightColor((byte)(255 / f), (byte)(128 / f), (byte)(128 / f));
            outerR = new LightColor((byte)(100 / f), (byte)(64 / f), (byte)(255 / f));
            ChangeF();

            LightLayer layer0 = new LightLayer();
            for (byte i = 0; i < 60; i++)
            {
                layer0.SetSegmentColor(0, i, color_num[axes[i][6] - 23 + 12]);
                layer0.SetSegmentColor(1, i, color_num[axes[i][6] - 23 + 12]);
                layer0.SetSegmentColor(2, i, color_num[axes[i][6] - 23 + 14]);
                layer0.SetSegmentColor(3, i, color_num[axes[i][6] - 23 + 14]);
            }

            LightFrame gradientFrame = new LightFrame { layers = { [0] = layer0, } };
            lights.SendLightFrame(gradientFrame);
        }

        public static LightColor white;
        public static void SendLight32(LightController lights)
        {
            for (int i = 0; i < 12; i++)
            {
                color_num[i] = new LightColor((byte)(colors12[i][0] / f), (byte)(colors12[i][1] / f), (byte)(colors12[i][2] / f));
            }
            white = new LightColor((byte)(255 / f), (byte)(255 / f), (byte)(255 / f));
            ChangeF();

            LightLayer layer0 = new LightLayer();
            for (byte i = 0; i < 60; i++)
            {
                layer0.SetSegmentColor(0, i, color_num[axes[i][2] - 1]);
                layer0.SetSegmentColor(1, i, color_num[axes[i][2] - 1]);
                layer0.SetSegmentColor(2, i, white);
                layer0.SetSegmentColor(3, i, white);
            }

            LightFrame gradientFrame = new LightFrame { layers = { [0] = layer0, } };
            lights.SendLightFrame(gradientFrame);
        }
    }
}