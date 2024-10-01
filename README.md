# WaccaCircle
 LilyConsole Extension using vJoy
 link : https://github.com/yellowberryHN/LilyConsole

## Usage
You need to download vJoy and configure a controller (don't forget to set up buttons number. by default it's 8).
download: https://sourceforge.net/projects/vjoystick/
source: https://github.com/shauleiz/vJoy

You need to configure the joystick#1 before launching any exe of WaccaCircle.
the -id2 or -id3 at the end of the exe name means that it's acting with joystick#2 or joystick#3 respectively


## WaccaArrows
this exe presses or releases keystrokes when you press or release one of the 4 parts of the whole circle.
top part: sends Up Arrow
right part: sends Right Arrow
bottom part: sends Down Arrow
left part: sends Left Arrow

## WaccaStartup
this is basically WaccaArrows for the outer 2 layers.
for the inner 2 layers, the circle is divided by 12 areas (which are mapped by their clockwise position. 1 is at 1 o'clock)
for each area, if [number].lnk exists on your desktop, then it launches that lnk (you can press or hold, it doesn't matter, it'll only launch once per touch)
for example, if 12.lnk exists on my desktop and I touch the inner top of the circle, then it will launch 12.lnk
else, here's the mapping of the key presses and releases.
1:  None
2:  None
3:  None
4:  Suppr
5:  Escape
6:  F11 (located at the bottom of the inner circle)
7:  Shift
8:  Ctrl
9:  Alt
10: Tab
11: F4
12: Enter

## WaccaCircle72
### axes
x-axis and y-axis for the whole circle <br>
rx-axis and ry-axis for the outer circle (2 layers)<br>
sl0-axis and sl1-axis for the inner circle (2 layers near the screen)<br>
### inner circle buttons
 buttons 1 to 12 mapped clockwise (top is 12) for each inner 2 segment / inner circle<br>
 <br>
 button 13: top-right quarter of the inner circle  (the center of the quarter is the top-right aka pi/4)<br>
 button 14: bottom-right quarter of the inner circle  <br>
 button 15: bottom-left quarter of the inner circle  <br>
 button 16: top-left quarter of the inner circle  <br>
  <br>
 button 17: top quarter of the inner circle (the center of the quarter is the top aka pi/2)<br>
 button 18: right quarter of the inner circle <br>
 button 19: bottom quarter of the inner circle <br>
 button 20: left quarter of the inner circle <br>
 <br>
 button 21: top half of the inner circle (the center of this segment is the top aka pi/2)<br>
 button 22: bottom half of the inner circle <br>
 <br>
 button 23: left half of the inner circle (the center of this segment is the left aka pi)<br>
 button 24: right half of the inner circle <br>
 <br>
 buttons 25 to 36 mapped clockwise (top is 36) for each segment of the whole circle<br>
 <br>
 ### whole circle buttons
 button 37: top-right quarter of the whole circle  (the center of the quarter is the top-right aka pi/4)<br>
 button 38: bottom-right quarter of the whole circle  <br>
 button 39: bottom-left quarter of the whole circle  <br>
 button 40: top-left quarter of the whole circle  <br>
  <br>
 button 41: top quarter of the whole circle (the center of the quarter is the top aka pi/2)<br>
 button 42: right quarter of the whole circle <br>
 button 43: bottom quarter of the whole circle <br>
 button 44: left quarter of the whole circle <br>
 <br>
 button 45: top half of the whole circle (the center of this segment is the top aka pi/2)<br>
 button 46: bottom half of the whole circle <br>
 <br>
 button 47: left half of the whole circle (the center of this segment is the left aka pi)<br>
 button 48: right half of the whole circle <br>
 <br>
 ### outer circle buttons
 buttons 49 to 60 mapped clockwise (top is 60) for each segment of the outer circle<br>
 <br>
 button 61: top-right quarter of the outer circle  (the center of the quarter is the top-right aka pi/4)<br>
 button 62: bottom-right quarter of the outer circle  <br>
 button 63: bottom-left quarter of the outer circle  <br>
 button 64: top-left quarter of the outer circle  <br>
 <br>
 button 65: top quarter of the outer circle (the center of the quarter is the top aka pi/2)<br>
 button 66: right quarter of the outer circle <br>
 button 67: bottom quarter of the outer circle <br>
 button 68: left quarter of the outer circle <br>
 <br>
 button 69: top half of the outer circle (the center of this segment is the top aka pi/2)<br>
 button 70: bottom half of the outer circle <br>
 <br>
 button 71: left half of the whole circle (the center of this segment is the left aka pi)<br>
 button 72: right half of the whole circle <br>
 <br>
