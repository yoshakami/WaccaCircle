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
                Send "{Alt down}{Ctrl down}{Shift down}{LWin down}{F1 up}"
            }
            else
            {
                Send "{Alt down}{Ctrl down}{Shift down}{F1 up}"
            }
        }
        else
        {
            if GetKeyState("LWin", "P")
            {
                Send "{Alt down}{Ctrl down}{LWin down}{F1 up}"
            }
            else
            {
                Send "{Alt down}{Ctrl down}{F1 up}"
            }
        }
    }
	else
	{
        if GetKeyState("Shift", "P")
        {
            if GetKeyState("LWin", "P")
            {
                Send "{Alt down}{Shift down}{LWin down}{F1 up}"
            }
            else
            {
                Send "{Alt down}{Shift down}{F1 up}"
            }
        }
        else
        {
            if GetKeyState("LWin", "P")
            {
                Send "{Alt down}{LWin down}{F1 up}"
            }
            else
            {
                Send "{Alt down}{F1 up}"
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
                Send "{Ctrl down}{Shift down}{LWin down}{F1 up}"
            }
            else
            {
                Send "{Ctrl down}{Shift down}{F1 up}"
            }
        }
        else
        {
            if GetKeyState("LWin", "P")
            {
                Send "{Ctrl down}{LWin down}{F1 up}"
            }
            else
            {
                Send "{Ctrl down}{F1 up}"
            }
        }
    }
	else
	{
        if GetKeyState("Shift", "P")
        {
            if GetKeyState("LWin", "P")
            {
                Send "{Shift down}{LWin down}{F1 up}"
            }
            else
            {
                Send "{Shift down}{F1 up}"
            }
        }
        else
        {
            if GetKeyState("LWin", "P")
            {
                Send "{LWin down}{F1 up}"
            }
            else
            {
                Send "{F1 up}"
            }
        }
	}
}