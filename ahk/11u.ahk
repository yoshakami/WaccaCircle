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
                Send "{Alt down}{Ctrl down}{Shift down}{LWin down}{F4 up}"
            }
            else
            {
                Send "{Alt down}{Ctrl down}{Shift down}{F4 up}"
            }
        }
        else
        {
            if GetKeyState("LWin", "P")
            {
                Send "{Alt down}{Ctrl down}{LWin down}{F4 up}"
            }
            else
            {
                Send "{Alt down}{Ctrl down}{F4 up}"
            }
        }
    }
	else
	{
        if GetKeyState("Shift", "P")
        {
            if GetKeyState("LWin", "P")
            {
                Send "{Alt down}{Shift down}{LWin down}{F4 up}"
            }
            else
            {
                Send "{Alt down}{Shift down}{F4 up}"
            }
        }
        else
        {
            if GetKeyState("LWin", "P")
            {
                Send "{Alt down}{LWin down}{F4 up}"
            }
            else
            {
                Send "{Alt down}{F4 up}"
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
                Send "{Ctrl down}{Shift down}{LWin down}{F4 up}"
            }
            else
            {
                Send "{Ctrl down}{Shift down}{F4 up}"
            }
        }
        else
        {
            if GetKeyState("LWin", "P")
            {
                Send "{Ctrl down}{LWin down}{F4 up}"
            }
            else
            {
                Send "{Ctrl down}{F4 up}"
            }
        }
    }
	else
	{
        if GetKeyState("Shift", "P")
        {
            if GetKeyState("LWin", "P")
            {
                Send "{Shift down}{LWin down}{F4 up}"
            }
            else
            {
                Send "{Shift down}{F4 up}"
            }
        }
        else
        {
            if GetKeyState("LWin", "P")
            {
                Send "{LWin down}{F4 up}"
            }
            else
            {
                Send "{F4 up}"
            }
        }
	}
}