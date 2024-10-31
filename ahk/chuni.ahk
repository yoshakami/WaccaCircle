#SingleInstance
#Warn
Loop
{
	Sleep(500)
	if WinExist("teaGfx")
	{
		WinActivate
		WinMove -700, 400, 1780, 1400, "teaGfx"
		;WinMove -1580, 400, 2650, 1450, "teaGfx"
		Exitapp
	}
}