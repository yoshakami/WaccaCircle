// yup. defining an array is faster than doing maths
// efficiency.
int[][] axes =
{      //       x axis    y-axis   1-12  13-16  17-20  21-22  23-24  25-32
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