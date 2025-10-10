#SingleInstance
#Warn
; Loop to check for an active window and resize it
Loop
{
    Sleep(500)
    if WinExist("Mercury") ; "A" means the currently active window
    {
        WinActivate
        ; Move and resize the active window to specified coordinates and size
        WinMove(0, 0, 800, 1080, "Mercury")
        ExitApp
    }
}