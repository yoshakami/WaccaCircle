#SingleInstance
#Warn
Loop
{
	Sleep(500)
	if WinExist("Dolphin")
	{
		WinActivate
		Exitapp
	}
}