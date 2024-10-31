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
                Send "{Alt down}{Ctrl down}{Shift down}{LWin down}{F11 down}"
            }
            else
            {
                Send "{Alt down}{Ctrl down}{Shift down}{F11 down}"
            }
        }
        else
        {
            if GetKeyState("LWin", "P")
            {
                Send "{Alt down}{Ctrl down}{LWin down}{F11 down}"
            }
            else
            {
                Send "{Alt down}{Ctrl down}{F11 down}"
            }
        }
    }
	else
	{
        if GetKeyState("Shift", "P")
        {
            if GetKeyState("LWin", "P")
            {
                Send "{Alt down}{Shift down}{LWin down}{F11 down}"
            }
            else
            {
                Send "{Alt down}{Shift down}{F11 down}"
            }
        }
        else
        {
            if GetKeyState("LWin", "P")
            {
                Send "{Alt down}{LWin down}{F11 down}"
            }
            else
            {
                Send "{Alt down}{F11 down}"
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
                Send "{Ctrl down}{Shift down}{LWin down}{F11 down}"
            }
            else
            {
                Send "{Ctrl down}{Shift down}{F11 down}"
            }
        }
        else
        {
            if GetKeyState("LWin", "P")
            {
                Send "{Ctrl down}{LWin down}{F11 down}"
            }
            else
            {
                Send "{Ctrl down}{F11 down}"
            }
        }
    }
	else
	{
        if GetKeyState("Shift", "P")
        {
            if GetKeyState("LWin", "P")
            {
                Send "{Shift down}{LWin down}{F11 down}"
            }
            else
            {
                Send "{Shift down}{F11 down}"
            }
        }
        else
        {
            if GetKeyState("LWin", "P")
            {
                Send "{LWin down}{F11 down}"
            }
            else
            {
                Send "{F11 down}"
            }
        }
	}
}