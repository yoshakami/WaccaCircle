#SingleInstance
#Warn
if GetKeyState("Alt", "P")
{
	if GetKeyState("Ctrl", "P")
    {
            if GetKeyState("LWin", "P")
            {
                Send "{Alt down}{Ctrl down}{LWin down}{Shift down}"
            }
            else
            {
                Send "{Alt down}{Ctrl down}{Shift down}"
            }
        
    }
	else
	{
            if GetKeyState("LWin", "P")
            {
                Send "{Alt down}{LWin down}{Shift down}"
            }
            else
            {
                Send "{Alt down}{Shift down}"
            }
        
	}
}
else
{
	if GetKeyState("Ctrl", "P")
    {
            if GetKeyState("LWin", "P")
            {
                Send "{Ctrl down}{LWin down}{Shift down}"
            }
            else
            {
                Send "{Ctrl down}{Shift down}"
            }
        
    }
	else
	{
            if GetKeyState("LWin", "P")
            {
                Send "{LWin down}{Shift down}"
            }
            else
            {
                Send "{Shift down}"
            }
        
	}
}