# WaccaCircle
 LilyConsole Extension using vJoy <br>
 link : https://github.com/yellowberryHN/LilyConsole

## Usage
You need to download vJoy and configure a controller (don't forget to set up buttons number. by default it's 8).<br>
download: https://sourceforge.net/projects/vjoystick/ <br>
source: https://github.com/shauleiz/vJoy <br>

First, choose your lag delay, this will change the CPU usage of the app on Wacca
You need to configure the joystick#1 before launching any exe of WaccaCircle.
the -id2 or -id3 at the end of the exe name means that it's acting with joystick#2 or joystick#3 respectively

## lnk
if you wanna make a .lnk file to a start.bat, the beginning of your file should be like below. <br>
The second line will kill all instances of WaccaCircle since they collectivize the touch panels and prevent wacca from starting. <br>
if you wanna automatically launch WaccaCircleStartup when you're done playing Wacca, you need to launch `WaccaStartupBackgroundLauncher.exe` on startup. <br>
There's no way to stop mercury other than plugging a keyboard, a rubber ducky, or using remote desktop.
```
@echo off
wmic process where "name like '%WaccaCircle%'" call terminate
```


## faq
Q: What happens if I press two directions at the same time on the joystick axes?<br>
A: it will average all pressed directions on the same axis, thus reaching a spot on the joystick that wouldn't be possible by touching the circle with only one hand

Q: Will it do the average too if I press more than two directions at the same time on the joystick axes?<br>
A: Yes

Q: Does it Spam Stick Directions if I keep pressing ?<br>
A: Nope, if you don't move your hand and keep pressing, it will still use the last stick position without sending a new one

Q: Does it Spam buttons if I keep the panel pressed?<br>
A: Nope, it will only release the button once you stop pressing (or move your hand out of bounds)

Q: What happens if I press two buttons at the same time?<br>
A: it presses two buttons at the same time.

Q: Can I press buttons and move the axes at the same time?<br>
A: Yes

```
Q: Unhandled Exception: System.IO.IOException: The port 'COM4' does not exist.<br>
   at System.IO.Ports.InternalResources.WinIOError(Int32 errorCode, String str)<br>
   at System.IO.Ports.SerialStream..ctor(String portName, Int32 baudRate, Parity parity, Int32 dataBits, StopBits stopBits, Int32 readTimeout, Int32 writeTimeout, Handshake handshake, Boolean dtrEnable, Boolean rtsEnable, Boolean discardNull, Byte parityReplace)<br>
   at System.IO.Ports.SerialPort.Open()<br>
   at LilyConsole.TouchManager.Initialize() in C:\C#\LilyConsole\TouchController.cs:line 199<br>
   at WaccaKeyBind.Program.TouchCombinedTest()<br>
   at WaccaKeyBind.Program.Main(String[] args)<br>
A: This tool is not for you. It is meant to work on Wacca, not on your weird computer without COM ports.
```

Q: DLL Error<br>
A: you need to paste all the dll files in the same folder as the .exe,<br>
   which means that if you're compiling, you need to paste all dll files in the bin folder.<br>


Q: How do I add WaccaStartup to windows startup?<br>
A: copy WaccaStartup.exe, press Win+R then type `shell:startup` and press enter, then paste shortcut (do not paste the exe, you definitely want a lnk there) 

Q: What are each exe for?<br>
A: Read below

## Dolphin
be careful: dolphin only accepts up to 32 buttons in DInput (use SDL on newer versions, but these newer versions won't work on wacca because qt6 decided to stop working on windows OS not updated prior to 2020)<br>
so there's no use to launching WaccaCircle96 if you intend to use it on dolphin.<br>
download latest version of dolphin that works : https://dl.dolphin-emu.org/builds/0c/ca/dolphin-master-5.0-16793-x64.7z<br>
mirror : https://web.archive.org/web/20230605023019/https://dl.dolphin-emu.org/builds/0c/ca/dolphin-master-5.0-16793-x64.7z

## Cemu
Cemu runs on a solid 60FPS on Wacca. it supports Xinput and Dinput, but you can't map names by hand in a config file<br>
that's why the joystick made for cemu has no overlap in controls. Here are the controls:<br>
outer circle: left stick (X and Y)<br>
inner circle: 12 buttons with special mapping some buttons unless you press the motion toggle<br>
suggested controls for each button of the inner circle:<br>
outer circle + hold 1: up blow mic, down show screen, left stick press, right stick press<br>
~~outer circle + hold 2: shake forward, backwards, left, and right~~ motion controls are only for SDL controllers at the moment in cemu<br>
2: X
outer circle + hold 3: right stick (RX and RY)<br>
4: ZR<br>
5: R<br>
6: A<br>
7: L<br>
8: ZL<br>
outer circle + hold 9: D-pad (4 buttons)<br>
~~outer circle + hold 10: tilt~~ motion controls are only for SDL controllers at the moment in cemu<br>
10: Y
outer circle + hold 11: left=minus, right=plus<br>
12: B<br>

## Xinput
the exe made for Cemu uses DirectInput<br>
this one uses Xinput with the same controls

## osu
I've made an exe file especially for sentakki, I also edited the ruleset to remove touch screen use, see https://github.com/yoshakami/osu-without-touch-screen/releases/tag/2024.718.0 <br>
the mappings are as such (if you didn't change controls): <br>
top: up<br>
bottom: down <br>
2 rightest lanes: enter <br>
2 toppest lanes: escape (maintain 2-sec during a song to escape from it)<br>
if you wanna press up instead of escape, you just need to touch outside of the 2 lanes at the top.<br>
<br>
press the outer right part twice to enter song selection<br>
press the outer top part to nagivate upwards<br>
press the outer bottom part to navigate downwards<br>
press the outer right part again to start a song.<br>
you can play it by tapping anywhere you want on the circle<br>
after the play is finished, press the outer left part to go back to song selection<br>
if you wanna quit the game, spam press the outer left part<br>

## spice
if you wanna bind controls with spicecfg, you'll see that whenever you click on bind, the ioboard takes over.<br>
in order to fix this, you just need to unplug USB-1. (once you're done mapping, plug it back to play safe)

## taiko
if your screen is not 120Hz (which is the case for wacca)<br>
you will need NVIDIA Control Pannel ver 472.12 (or another, but 417.71 is too low and 565.90 is too high)<br>
on the nvidia control pannel, do this with taiko.exe: <br>
- Max Frame Rate: 120FPS <br>
- Low Latency Mode: Ultra <br>
- Power Management Mode: Prefer maximum performance <br>
- Vertical Sync: Fast <br>
<br>
on the file "config.toml", do this : <br>
shared_audio = false <br>
vsync = false <br>
windowed = false <br>
<br>
| button id   | Description | location on the circle |
| ----------- | ----------- | ---------------------- |
| 12          | outer left  | outer left             |
| 13          | outer right | outer right            |
| 14          | inner left  | outer bottom           |
| 15          | inner right | inner right            |

## rpg
| button id   | Description | location on the circle |
| ----------- | ----------- | ---------------------- |
| 1           | back        | inner left             |
| 2           | enter       | inner right            |
| 4           | open menu   | inner top              |
| 7           | attacc      | inner bottom           |
| 13          | up          | outer top              |
| 14          | down        | outer bottom           |
| 15          | left        | outer left             |
| 16          | right       | outer right            |

## WaccaCircleDesktop
the following conditions needs to be met before launching this one:
you have 24 lnk on your desktop (named from 1 to 24),<br>
you've set the screen resolution to 1080x1920 and the zoom to 100% (in display settings),<br>
you've made the taskbar to be docked at the bottom, and always visible
when you right click on an empty space of the desktop, the options are set as follows
view > [check] show desktop icons
view > large icons
view > [uncheck] auto arrange icons
view > [uncheck] align icons to grid
sort by > item type


## WaccaStartupBackgroundLauncher
this is the executable that you'd add to windows startup.<br>
this headless executable will check if any WaccaCircle*.exe is launched on the system<br>
if not, then it'll check if any Mercury*.exe is launched on the system<br>
if not, then it'll launch `WaccaCircleOsu.exe` if you're running osu<br>
if not, then it'll launch `WaccaCircleSDVX.exe` if you're running SDVX<br>
if not, then it'll launch `WaccaCircle32.exe` if you're running Dolphin<br>
if not, then it'll launch `WaccaCircleStartup.exe` (it knows where the file is because the exe knows its own folder path)<br>
in order to completely control your wacca, you'd want to make lnk files to shutdown the system or launch apps

## WaccaCircleStartup
this exe presses or releases keystrokes when you press or release one of the outer 2 layers. (25 to 28.ahk)<br>
top part: sends Up Arrow<br>
right part: sends Right Arrow<br>
bottom part: sends Down Arrow<br>
left part: sends Left Arrow<br>
<br>
for the inner 2 layers, the circle is divided by 12 areas (which are mapped by their clockwise position. 1 is at 1 o'clock)<br>
for each area, if [number].lnk exists on your desktop, then it launches that lnk (you can press or hold, it doesn't matter, it'll only launch once per touch)<br>
for example, if 12.lnk exists on my desktop and I touch the inner top of the circle, then it will launch 12.lnk<br>
else, here's the mapping of the key presses and releases.<br>
the release will contain the ahk files launching the keystrokes below.<br>
if you press Alt, then it'll add 12 to all panels, so you can press alt+tab and still have 10.lnk<br>
example : Desktop/WaccaCircle/1d.ahk when panel is pressed down, and 1u.ahk when panel is released up. <br>
1:  F1<br>
2:  Win<br>
3:  Ctrl<br>
4:  Del<br>
5:  Escape<br>
6:  F11 (located at the bottom of the inner circle)<br>
7:  Shift<br>
9:  Alt<br>
10+12: Tab<br>
11+12: F4<br>
11: Win+D<br>
12: Enter + switch to WaccaCircle32 if dolphin or spice is launched<br>
<br>
(outer pannels)<br>
25: Up<br>
26: Right<br>
27: Down<br>
28: Left<br>
<br>
(Osu! outer pannels)
29: Up<br>
30: Enter<br>
31: Down<br>
32: Esc<br>
Dolphin: focuses the window.

## WaccaCircle12
buttons 1 to 12 mapped clockwise (top is 12) for the whole circle<br>
no axes

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


## WaccaCircle96
see WaccaCircle24 for all the axes and buttons from 1 to 24 <br>
<br>
buttons 25 to 32 divide the inner circle by 8, mapped clockwise<br>
<br>
buttons 33 to 44 mapped clockwise (top is 36) for each segment of the whole circle<br>
<br>
### whole circle buttons
button 45: top-right quarter of the whole circle  (the center of the quarter is the top-right aka pi/4)<br>
button 46: bottom-right quarter of the whole circle  <br>
button 47: bottom-left quarter of the whole circle  <br>
button 48: top-left quarter of the whole circle  <br>
<br>
button 49: top quarter of the whole circle (the center of the quarter is the top aka pi/2)<br>
button 50: right quarter of the whole circle <br>
button 51: bottom quarter of the whole circle <br>
button 52: left quarter of the whole circle <br>
<br>
button 53: top half of the whole circle (the center of this segment is the top aka pi/2)<br>
button 54: bottom half of the whole circle <br>
<br>
button 55: left half of the whole circle (the center of this segment is the left aka pi)<br>
button 56: right half of the whole circle <br>
<br>
buttons 57 to 64 divide the whole circle by 8, mapped clockwise
<br>
### outer circle buttons
buttons 65 to 76 mapped clockwise (top is 60) for each segment of the outer circle<br>
<br>
button 77: top-right quarter of the outer circle  (the center of the quarter is the top-right aka pi/4)<br>
button 78: bottom-right quarter of the outer circle  <br>
button 79: bottom-left quarter of the outer circle  <br>
button 80: top-left quarter of the outer circle  <br>
<br>
button 81: top quarter of the outer circle (the center of the quarter is the top aka pi/2)<br>
button 82: right quarter of the outer circle <br>
button 83: bottom quarter of the outer circle <br>
button 84: left quarter of the outer circle <br>
<br>
button 85: top half of the outer circle (the center of this segment is the top aka pi/2)<br>
button 86: bottom half of the outer circle <br>
<br>
button 87: left half of the outer circle (the center of this segment is the left aka pi)<br>
button 88: right half of the outer circle <br>
<br>
buttons 89 to 96 divide the inner circle by 8, mapped clockwise
<br>

## compilation
the different folders are there because some files do not require the same dependencies to be compiled. they use less DLL, so the exe are optimized<br>
<br>
install 22GB of Visual Studio C# compilers and install .NET Framework 4.5.2 library SDK, then open the .sln file with visual studio.<br>
In solution explorer, right click the .cs file => exclude from project, then right click on WaccaCircle => add => existing item => select desired .cs file.<br>
Then Choose `Release` and `x64` from the dropdown menus and click start. once you did, WaccaCircle.exe will appear in the bin folder, feel free to rename it to whatever you want.<br>
depending on your desired .cs file, you'd need to swap WaccaCircle.csproj with the correct one.
