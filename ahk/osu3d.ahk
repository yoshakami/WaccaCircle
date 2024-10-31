#SingleInstance
#Warn
if GetKeyState("Alt", "P")
{
	if GetKeyState("Ctrl", "P")
    {
        if GetKeyState("Shift", "P")
        {
            if GetKeyState("LWin", "P")
            {
                Send "{Alt down}{Ctrl down}{Shift down}{LWin down}{Down down}"
            }
            else
            {
                Send "{Alt down}{Ctrl down}{Shift down}{Down down}"
            }
        }
        else
        {
            if GetKeyState("LWin", "P")
            {
                Send "{Alt down}{Ctrl down}{LWin down}{Down down}"
            }
            else
            {
                Send "{Alt down}{Ctrl down}{Down down}"
            }
        }
    }
	else
	{
        if GetKeyState("Shift", "P")
        {
            if GetKeyState("LWin", "P")
            {
                Send "{Alt down}{Shift down}{LWin down}{Down down}"
            }
            else
            {
                Send "{Alt down}{Shift down}{Down down}"
            }
        }
        else
        {
            if GetKeyState("LWin", "P")
            {
                Send "{Alt down}{LWin down}{Down down}"
            }
            else
            {
                Send "{Alt down}{Down down}"
            }
        }
	}
}
else
{
	if GetKeyState("Ctrl", "P")
    {
        if GetKeyState("Shift", "P")
        {
            if GetKeyState("LWin", "P")
            {
                Send "{Ctrl down}{Shift down}{LWin down}{Down down}"
            }
            else
            {
                Send "{Ctrl down}{Shift down}{Down down}"
            }
        }
        else
        {
            if GetKeyState("LWin", "P")
            {
                Send "{Ctrl down}{LWin down}{Down down}"
            }
            else
            {
                Send "{Ctrl down}{Down down}"
            }
        }
    }
	else
	{
        if GetKeyState("Shift", "P")
        {
            if GetKeyState("LWin", "P")
            {
                Send "{Shift down}{LWin down}{Down down}"
            }
            else
            {
                Send "{Shift down}{Down down}"
            }
        }
        else
        {
            if GetKeyState("LWin", "P")
            {
                Send "{LWin down}{Down down}"
            }
            else
            {
                Send "{Down down}"
            }
        }
	}
}