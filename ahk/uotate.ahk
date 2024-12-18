#SingleInstance Force

pToken := Gdip_Startup()I see what you're saying now. My apologies for the confusion. Let me correct the syntax fully to AHK v2 standards. In AHK v2, we handle strings and expressions with different syntax rules, and I will ensure that this script properly uses AHK v2 conventions, including handling the text properly.Updated AHK v2 Scriptahk#Requires AutoHotkey v2.0#SingleInstance Force; Initialize GDI+pToken := Gdip_Startup(); Find the window with the title containing "teaGfx"WinGet, hwnd, ID, ahk_exe teaGfxif (!hwnd){    MsgBox("Window with 'teaGfx' not found!")    ExitApp}; Create a new window to hold the rotated contentGui, 2: -Caption +E0x80000 +LastFound +OwnDialogs +Owner +AlwaysOnTopGui, 2: Show, NAhwnd2 := WinExist(); Get the dimensions of the original windowWinGetPos, &x, &y, &w, &h, ahk_id %hwnd%; Create a DIBSection and compatible DC for drawinghbm := CreateDIBSection(h, w)hdc := CreateCompatibleDC()obm := SelectObject(hdc, hbm)G := Gdip_GraphicsFromHDC(hdc); Set a timer to update the window every 50msSetTimer(Update, 50)returnUpdate:    ; Capture the window content into a bitmap    pBitmap := Gdip_BitmapFromScreen("hwnd:" hwnd)        ; Rotate the image 90 degrees clockwise    Gdip_ImageRotateFlip(pBitmap, 1)  ; 1 = rotate 90 degrees clockwise    ; Draw the rotated image onto the new window    Gdip_DrawImage(G, pBitmap, 0, 0)        ; Update the layered window with the rotated image    UpdateLayeredWindow(hwnd2, hdc, 0, 0, h, w)        ; Clean up the bitmap object to free memory    Gdip_DisposeImage(pBitmap)returnGuiClose:    ; Clean up GDI resources and shut down GDI+    SelectObject(hdc, obm)    DeleteObject(hbm)    DeleteDC(hdc)
Gdip_DeleteGraphics(G)
Gdip_Shutdown(pToken)

ExitAppreturn