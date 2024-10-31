#SingleInstance
#Warn
Loop
{
	Sleep(500)
	if WinExist("osu")
	{
		WinActivate
		Exitapp
	}
}