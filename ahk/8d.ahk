#SingleInstance
#Warn
if GetKeyState("Alt", "P")
{
        if GetKeyState("Shift", "P")
        {
            if GetKeyState("LWin", "P")
            {
                Send "{Alt down}{Shift down}{LWin down}{Ctrl down}"
            }
            else
            {
                Send "{Alt down}{Shift down}{Ctrl down}"
            }
        }
        else
        {
            if GetKeyState("LWin", "P")
            {
                Send "{Alt down}{LWin down}{Ctrl down}"
            }
            else
            {
                Send "{Alt down}{Ctrl down}"
            }
        }
	
}
else
{
        if GetKeyState("Shift", "P")
        {
            if GetKeyState("LWin", "P")
            {
                Send "{Shift down}{LWin down}{Ctrl down}"
            }
            else
            {
                Send "{Shift down}{Ctrl down}"
            }
        }
        else
        {
            if GetKeyState("LWin", "P")
            {
                Send "{LWin down}{Ctrl down}"
            }
            else
            {
                Send "{Ctrl down}"
            }
        }
	
}