#SingleInstance
#Warn
Loop
{
	Sleep(1000)
	SetTitleMatchMode(3) ; Exact match
	if WinExist("ll3")
	{
		WinActivate
		WinMove -6, 795, 1092, 657, "ll3"
		Sleep(1000)
		; Get client position and size
        WinGetClientPos(&x, &y, &w, &h, "ll3")

        ; Check if the client area is at the desired position
        if (x == -6 && y == 795 && w == 1092 && h == 657) {
            ; Adjust the window to another position
            WinMove 0, 826, 1080, 618, "ll3"
        }
		ExitApp
	}
}