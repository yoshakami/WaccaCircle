#SingleInstance
#Warn
if GetKeyState("Alt", "P")
{
        if GetKeyState("Shift", "P")
        {
            if GetKeyState("LWin", "P")
            {
                Send "{Alt down}{Shift down}{LWin down}{Ctrl up}"
            }
            else
            {
                Send "{Alt down}{Shift down}{Ctrl up}"
            }
        }
        else
        {
            if GetKeyState("LWin", "P")
            {
                Send "{Alt down}{LWin down}{Ctrl up}"
            }
            else
            {
                Send "{Alt down}{Ctrl up}"
            }
        }
	
}
else
{
        if GetKeyState("Shift", "P")
        {
            if GetKeyState("LWin", "P")
            {
                Send "{Shift down}{LWin down}{Ctrl up}"
            }
            else
            {
                Send "{Shift down}{Ctrl up}"
            }
        }
        else
        {
            if GetKeyState("LWin", "P")
            {
                Send "{LWin down}{Ctrl up}"
            }
            else
            {
                Send "{Ctrl up}"
            }
        }
	
}