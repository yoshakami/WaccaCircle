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
                Send "{Alt down}{Ctrl down}{Shift down}{LWin down}{Del down}"
            }
            else
            {
                Send "{Alt down}{Ctrl down}{Shift down}{Del down}"
            }
        }
        else
        {
            if GetKeyState("LWin", "P")
            {
                Send "{Alt down}{Ctrl down}{LWin down}{Del down}"
            }
            else
            {
                Send "{Alt down}{Ctrl down}{Del down}"
            }
        }
    }
	else
	{
        if GetKeyState("Shift", "P")
        {
            if GetKeyState("LWin", "P")
            {
                Send "{Alt down}{Shift down}{LWin down}{Del down}"
            }
            else
            {
                Send "{Alt down}{Shift down}{Del down}"
            }
        }
        else
        {
            if GetKeyState("LWin", "P")
            {
                Send "{Alt down}{LWin down}{Del down}"
            }
            else
            {
                Send "{Alt down}{Del down}"
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
                Send "{Ctrl down}{Shift down}{LWin down}{Del down}"
            }
            else
            {
                Send "{Ctrl down}{Shift down}{Del down}"
            }
        }
        else
        {
            if GetKeyState("LWin", "P")
            {
                Send "{Ctrl down}{LWin down}{Del down}"
            }
            else
            {
                Send "{Ctrl down}{Del down}"
            }
        }
    }
	else
	{
        if GetKeyState("Shift", "P")
        {
            if GetKeyState("LWin", "P")
            {
                Send "{Shift down}{LWin down}{Del down}"
            }
            else
            {
                Send "{Shift down}{Del down}"
            }
        }
        else
        {
            if GetKeyState("LWin", "P")
            {
                Send "{LWin down}{Del down}"
            }
            else
            {
                Send "{Del down}"
            }
        }
	}
}