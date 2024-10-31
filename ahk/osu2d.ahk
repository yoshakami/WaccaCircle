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
                Send "{Alt down}{Ctrl down}{Shift down}{LWin down}{Enter down}"
            }
            else
            {
                Send "{Alt down}{Ctrl down}{Shift down}{Enter down}"
            }
        }
        else
        {
            if GetKeyState("LWin", "P")
            {
                Send "{Alt down}{Ctrl down}{LWin down}{Enter down}"
            }
            else
            {
                Send "{Alt down}{Ctrl down}{Enter down}"
            }
        }
    }
	else
	{
        if GetKeyState("Shift", "P")
        {
            if GetKeyState("LWin", "P")
            {
                Send "{Alt down}{Shift down}{LWin down}{Enter down}"
            }
            else
            {
                Send "{Alt down}{Shift down}{Enter down}"
            }
        }
        else
        {
            if GetKeyState("LWin", "P")
            {
                Send "{Alt down}{LWin down}{Enter down}"
            }
            else
            {
                Send "{Alt down}{Enter down}"
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
                Send "{Ctrl down}{Shift down}{LWin down}{Enter down}"
            }
            else
            {
                Send "{Ctrl down}{Shift down}{Enter down}"
            }
        }
        else
        {
            if GetKeyState("LWin", "P")
            {
                Send "{Ctrl down}{LWin down}{Enter down}"
            }
            else
            {
                Send "{Ctrl down}{Enter down}"
            }
        }
    }
	else
	{
        if GetKeyState("Shift", "P")
        {
            if GetKeyState("LWin", "P")
            {
                Send "{Shift down}{LWin down}{Enter down}"
            }
            else
            {
                Send "{Shift down}{Enter down}"
            }
        }
        else
        {
            if GetKeyState("LWin", "P")
            {
                Send "{LWin down}{Enter down}"
            }
            else
            {
                Send "{Enter down}"
            }
        }
	}
}