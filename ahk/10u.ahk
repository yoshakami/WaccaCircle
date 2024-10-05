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
                Send "{Alt down}{Ctrl down}{Shift down}{LWin down}{Tab up}"
            }
            else
            {
                Send "{Alt down}{Ctrl down}{Shift down}{Tab up}"
            }
        }
        else
        {
            if GetKeyState("LWin", "P")
            {
                Send "{Alt down}{Ctrl down}{LWin down}{Tab up}"
            }
            else
            {
                Send "{Alt down}{Ctrl down}{Tab up}"
            }
        }
    }
	else
	{
        if GetKeyState("Shift", "P")
        {
            if GetKeyState("LWin", "P")
            {
                Send "{Alt down}{Shift down}{LWin down}{Tab up}"
            }
            else
            {
                Send "{Alt down}{Shift down}{Tab up}"
            }
        }
        else
        {
            if GetKeyState("LWin", "P")
            {
                Send "{Alt down}{LWin down}{Tab up}"
            }
            else
            {
                Send "{Alt down}{Tab up}"
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
                Send "{Ctrl down}{Shift down}{LWin down}{Tab up}"
            }
            else
            {
                Send "{Ctrl down}{Shift down}{Tab up}"
            }
        }
        else
        {
            if GetKeyState("LWin", "P")
            {
                Send "{Ctrl down}{LWin down}{Tab up}"
            }
            else
            {
                Send "{Ctrl down}{Tab up}"
            }
        }
    }
	else
	{
        if GetKeyState("Shift", "P")
        {
            if GetKeyState("LWin", "P")
            {
                Send "{Shift down}{LWin down}{Tab up}"
            }
            else
            {
                Send "{Shift down}{Tab up}"
            }
        }
        else
        {
            if GetKeyState("LWin", "P")
            {
                Send "{LWin down}{Tab up}"
            }
            else
            {
                Send "{Tab up}"
            }
        }
	}
}