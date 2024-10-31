#SingleInstance
#Warn
Loop
{
	Sleep(500)
	if WinExist("spice64")
	{
		WinActivate
		Exitapp
	}
}