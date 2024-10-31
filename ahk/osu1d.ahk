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
                Send "{Alt down}{Ctrl down}{Shift down}{LWin down}{Up down}"
            }
            else
            {
                Send "{Alt down}{Ctrl down}{Shift down}{Up down}"
            }
        }
        else
        {
            if GetKeyState("LWin", "P")
            {
                Send "{Alt down}{Ctrl down}{LWin down}{Up down}"
            }
            else
            {
                Send "{Alt down}{Ctrl down}{Up down}"
            }
        }
    }
	else
	{
        if GetKeyState("Shift", "P")
        {
            if GetKeyState("LWin", "P")
            {
                Send "{Alt down}{Shift down}{LWin down}{Up down}"
            }
            else
            {
                Send "{Alt down}{Shift down}{Up down}"
            }
        }
        else
        {
            if GetKeyState("LWin", "P")
            {
                Send "{Alt down}{LWin down}{Up down}"
            }
            else
            {
                Send "{Alt down}{Up down}"
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
                Send "{Ctrl down}{Shift down}{LWin down}{Up down}"
            }
            else
            {
                Send "{Ctrl down}{Shift down}{Up down}"
            }
        }
        else
        {
            if GetKeyState("LWin", "P")
            {
                Send "{Ctrl down}{LWin down}{Up down}"
            }
            else
            {
                Send "{Ctrl down}{Up down}"
            }
        }
    }
	else
	{
        if GetKeyState("Shift", "P")
        {
            if GetKeyState("LWin", "P")
            {
                Send "{Shift down}{LWin down}{Up down}"
            }
            else
            {
                Send "{Shift down}{Up down}"
            }
        }
        else
        {
            if GetKeyState("LWin", "P")
            {
                Send "{LWin down}{Up down}"
            }
            else
            {
                Send "{Up down}"
            }
        }
	}
}