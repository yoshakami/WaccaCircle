#SingleInstance
#Warn
Sleep(100)
if WinExist("Dolphin")
{
    WinActivate
}
else
{		
	Sleep(1000)
	if WinExist("Dolphin")
	{
		WinActivate
	}
}