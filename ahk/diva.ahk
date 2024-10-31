#SingleInstance
#Warn
Loop
{
	Sleep(500)
	if WinExist("Hatsune")
	{
		WinActivate
		WinMove -700, 400, 1080, 1080, "Hatsune"
		;WinMove -1580, 400, 2650, 1450, "teaGfx"
		Exitapp
	}
}