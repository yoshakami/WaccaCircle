#SingleInstance
#Warn
Run("explorer.exe C:\")
WinWait("ahk_class CabinetWClass") ; Wait for the explorer window
Loop
{
	Sleep(500)
	if WinExist("Local Disk")
	{
		WinActivate
		WinMove 0, 400, 1080, 1080, "Local Disk"
		Exitapp
	}
}
