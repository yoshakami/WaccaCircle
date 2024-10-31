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
                Send "{Alt down}{Ctrl down}{Shift down}{LWin down}{Esc up}"
            }
            else
            {
                Send "{Alt down}{Ctrl down}{Shift down}{Esc up}"
            }
        }
        else
        {
            if GetKeyState("LWin", "P")
            {
                Send "{Alt down}{Ctrl down}{LWin down}{Esc up}"
            }
            else
            {
                Send "{Alt down}{Ctrl down}{Esc up}"
            }
        }
    }
	else
	{
        if GetKeyState("Shift", "P")
        {
            if GetKeyState("LWin", "P")
            {
                Send "{Alt down}{Shift down}{LWin down}{Esc up}"
            }
            else
            {
                Send "{Alt down}{Shift down}{Esc up}"
            }
        }
        else
        {
            if GetKeyState("LWin", "P")
            {
                Send "{Alt down}{LWin down}{Esc up}"
            }
            else
            {
                Send "{Alt down}{Esc up}"
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
                Send "{Ctrl down}{Shift down}{LWin down}{Esc up}"
            }
            else
            {
                Send "{Ctrl down}{Shift down}{Esc up}"
            }
        }
        else
        {
            if GetKeyState("LWin", "P")
            {
                Send "{Ctrl down}{LWin down}{Esc up}"
            }
            else
            {
                Send "{Ctrl down}{Esc up}"
            }
        }
    }
	else
	{
        if GetKeyState("Shift", "P")
        {
            if GetKeyState("LWin", "P")
            {
                Send "{Shift down}{LWin down}{Esc up}"
            }
            else
            {
                Send "{Shift down}{Esc up}"
            }
        }
        else
        {
            if GetKeyState("LWin", "P")
            {
                Send "{LWin down}{Esc up}"
            }
            else
            {
                Send "{Esc up}"
            }
        }
	}
}

	if WinExist("osu")
	{
        WinActivate
	}
	else
	{
        Run 'wmic process where "name like `"%WaccaCircle%`"" call terminate'
        Sleep(1000)
        Run(A_ScriptDir . "/../WaccaCircleStartup.exe")
        Sleep(1000)
	}