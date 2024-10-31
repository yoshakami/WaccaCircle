#SingleInstance Force

pToken := Gdip_Startup()

Gui, 1: +LastFound
hwnd := WinExist()
Gui, 1: Add, Edit, x10 y10 w300 h200, Here is some text
Gui, 1: Show, AutoSize, Window 1

WinGetPos,,, w, h, ahk_id %hwnd%

Gui, 2: -Caption +E0x80000 +LastFound +OwnDialogs +Owner +AlwaysOnTop
Gui, 2: Show, NA
hwnd2 := WinExist()

hbm := CreateDIBSection(h, w), hdc := CreateCompatibleDC(), obm := SelectObject(hdc, hbm), G := Gdip_GraphicsFromHDC(hdc)
UpdateLayeredWindow(hwnd2, hdc, 0, 0, h, w)
SetTimer, Update, 50
return

;#####################################################################################

Update:
pBitmap := Gdip_BitmapFromScreen("hwnd:" hwnd)
Gdip_ImageRotateFlip(pBitmap, 1)
Gdip_DrawImage(G, pBitmap)
UpdateLayeredWindow(hwnd2, hdc)
Gdip_DisposeImage(pBitmap)
return

;#####################################################################################

GuiClose:
SelectObject(hdc, obm), DeleteObject(hbm), DeleteDC(hdc)
Gdip_DeleteGraphics(G)
Gdip_Shutdown(pToken)
ExitApp
return