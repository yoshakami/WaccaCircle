#SingleInstance
#Warn
; Loop to check for an active window and resize it
Loop
{
    Sleep(500)
    if WinExist("A") ; "A" means the currently active window
    {
        ; Move and resize the active window to specified coordinates and size
        WinMove(0, 400, 1080, 1080, "A")
        ExitApp
    }
}
