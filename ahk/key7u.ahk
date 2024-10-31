#SingleInstance
#Warn
if GetKeyState("Alt", "P")
{
	if GetKeyState("Ctrl", "P")
    {
            if GetKeyState("LWin", "P")
            {
                Send "{Alt down}{Ctrl down}{LWin down}{Shift up}"
            }
            else
            {
                Send "{Alt down}{Ctrl down}{Shift up}"
            }
        
    }
	else
	{
            if GetKeyState("LWin", "P")
            {
                Send "{Alt down}{LWin down}{Shift up}"
            }
            else
            {
                Send "{Alt down}{Shift up}"
            }
        
	}
}
else
{
	if GetKeyState("Ctrl", "P")
    {
            if GetKeyState("LWin", "P")
            {
                Send "{Ctrl down}{LWin down}{Shift up}"
            }
            else
            {
                Send "{Ctrl down}{Shift up}"
            }
        
    }
	else
	{
            if GetKeyState("LWin", "P")
            {
                Send "{LWin down}{Shift up}"
            }
            else
            {
                Send "{Shift up}"
            }
        
	}
}