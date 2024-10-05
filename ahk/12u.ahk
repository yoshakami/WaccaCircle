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
                Send "{Alt down}{Ctrl down}{Shift down}{LWin down}{Enter up}"
            }
            else
            {
                Send "{Alt down}{Ctrl down}{Shift down}{Enter up}"
            }
        }
        else
        {
            if GetKeyState("LWin", "P")
            {
                Send "{Alt down}{Ctrl down}{LWin down}{Enter up}"
            }
            else
            {
                Send "{Alt down}{Ctrl down}{Enter up}"
            }
        }
    }
	else
	{
        if GetKeyState("Shift", "P")
        {
            if GetKeyState("LWin", "P")
            {
                Send "{Alt down}{Shift down}{LWin down}{Enter up}"
            }
            else
            {
                Send "{Alt down}{Shift down}{Enter up}"
            }
        }
        else
        {
            if GetKeyState("LWin", "P")
            {
                Send "{Alt down}{LWin down}{Enter up}"
            }
            else
            {
                Send "{Alt down}{Enter up}"
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
                Send "{Ctrl down}{Shift down}{LWin down}{Enter up}"
            }
            else
            {
                Send "{Ctrl down}{Shift down}{Enter up}"
            }
        }
        else
        {
            if GetKeyState("LWin", "P")
            {
                Send "{Ctrl down}{LWin down}{Enter up}"
            }
            else
            {
                Send "{Ctrl down}{Enter up}"
            }
        }
    }
	else
	{
        if GetKeyState("Shift", "P")
        {
            if GetKeyState("LWin", "P")
            {
                Send "{Shift down}{LWin down}{Enter up}"
            }
            else
            {
                Send "{Shift down}{Enter up}"
            }
        }
        else
        {
            if GetKeyState("LWin", "P")
            {
                Send "{LWin down}{Enter up}"
            }
            else
            {
                Send "{Enter up}"
            }
        }
	}
}
if WinExist("Dolphin")
{
    Run 'wmic process where "name like `"%WaccaCircle%`"" call terminate'
    Sleep(1000)
    Run(A_ScriptDir . "/../WaccaCircle32.exe")
    Sleep(1000)
    WinActivate
}
else
{
	if WinExist("SOUND VOLTE")
	{
        Run 'wmic process where "name like `"%WaccaCircle%`"" call terminate'
        Sleep(1000)
        Run(A_ScriptDir . "/../WaccaCircle32.exe")
        Sleep(1000)
        WinActivate
	}
}