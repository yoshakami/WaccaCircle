# WaccaCircle
 LilyConsole Extension using vJoy
 link : https://github.com/yellowberryHN/LilyConsole

## Usage
You need to download vJoy and configure a controller (don't forget to set up buttons number. by default it's 8).
download: https://sourceforge.net/projects/vjoystick/
source: https://github.com/shauleiz/vJoy

You need to configure the joystick#1 before launching any exe of WaccaCircle.
the -id2 or -id3 at the end of the exe name means that it's acting with joystick#2 or joystick#3 respectively

be careful: dolphin only accepts up to 32 buttons in DInput (use SDL on newer versions, but these newer versions won't work on wacca because qt6 decided to stop working on windows OS not updated prior to 2020)
so there's no use to launching WaccaCircle72.

## faq
Q: What happens if I press two directions at the same time on the joystick axes?
A: it will go to each direction on each frame it updates the touched panels, which means it will spam directions

Q: Does it Spam Stick Directions if I keep the stick pressed at only one spot though?
A: Nope, if you don't move your hand and keep pressing, it will still use the last stick position without sending a new one

Q: Does it Spam buttons if I keep it pressed?
A: Nope, it will only release the button once you stop pressing (or move your hand out of bounds)

Q: What happens if I press two buttons at the same time?
A: it presses two buttons at the same time.

Q: Can I press buttons and move the axes at the same time?
A: Yes

Q: Unhandled Exception: System.IO.IOException: The port 'COM4' does not exist.
   at System.IO.Ports.InternalResources.WinIOError(Int32 errorCode, String str)
   at System.IO.Ports.SerialStream..ctor(String portName, Int32 baudRate, Parity parity, Int32 dataBits, StopBits stopBits, Int32 readTimeout, Int32 writeTimeout, Handshake handshake, Boolean dtrEnable, Boolean rtsEnable, Boolean discardNull, Byte parityReplace)
   at System.IO.Ports.SerialPort.Open()
   at LilyConsole.TouchManager.Initialize() in C:\C#\LilyConsole\TouchController.cs:line 199
   at WaccaKeyBind.Program.TouchCombinedTest()
   at WaccaKeyBind.Program.Main(String[] args)
A: This tool is not for you. It is meant to work on Wacca, not on your weird computer without COM ports.

Q: How do I compile?
A: install 22GB of Visual Studio C# compilers and install .NET Framework 4.5.2 library SDK, then open the .sln file with visual studio. In solution explorer, right click the .cs file => exclude from project, then right click on WaccaCircle => add => existing item => select desired .cs file. Then Choose `Release` and `x64` from the dropdown menus and click start. once you did, WaccaCircle.exe will appear in the bin folder, feel free to rename it to whatever you want.

Q: DLL Error
A: you need to paste all the dll files in the same folder as the .exe, which means that if you're compiling, you need to paste all dll files in the bin folder.

Q: How do I add WaccaStartup to windows startup?
A: copy WaccaStartup.exe, press Win+R then type `shell:startup` and press enter, then paste shortcut (do not paste the exe, you definitely want a lnk there) 

Q: What are each exe for?
A: Read below

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

## WaccaCircle24
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
 ## WaccaCircle24trim
 same as WaccaCircle24 but without the x-axis, y-axis, sl0-axis, and sl1-axis.<br>
 So there is no overlap in controls.

 ## WaccaCircle32
 see WaccaCircle24 for all the axes and buttons from 1 to 24 <br>
 <br>
 button 25: top quarter of the outer circle (the center of the quarter is the top aka pi/2)<br>
 button 26: right quarter of the outer circle <br>
 button 27: bottom quarter of the outer circle <br>
 button 28: left quarter of the outer circle <br>
 <br>
 button 29: top half of the outer circle (the center of this segment is the top aka pi/2)<br>
 button 30: bottom half of the outer circle <br>
 <br>
 button 31: left half of the outer circle (the center of this segment is the left aka pi)<br>
 button 32: right half of the outer circle <br>
 <br>


 ## WaccaCircle72
 see WaccaCircle24 for all the axes and buttons from 1 to 24 <br>
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
 button 71: left half of the outer circle (the center of this segment is the left aka pi)<br>
 button 72: right half of the outer circle <br>
 <br>
