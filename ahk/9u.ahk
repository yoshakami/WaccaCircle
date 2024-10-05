#SingleInstance
#Warn
if GetKeyState("Ctrl", "P")
{
    if GetKeyState("Shift", "P")
    {
        if GetKeyState("LWin", "P")
        {
            Send "{Ctrl down}{Shift down}{LWin down}{Alt up}"
        }
        else
        {
            Send "{Ctrl down}{Shift down}{Alt up}"
        }
    }
    else
    {
        if GetKeyState("LWin", "P")
        {
            Send "{Ctrl down}{LWin down}{Alt up}"
        }
        else
        {
            Send "{Ctrl down}{Alt up}"
        }
    }
}
else
{
    if GetKeyState("Shift", "P")
    {
        if GetKeyState("LWin", "P")
        {
            Send "{Shift down}{LWin down}{Alt up}"
        }
        else
        {
            Send "{Shift down}{Alt up}"
        }
    }
    else
    {
        if GetKeyState("LWin", "P")
        {
            Send "{LWin down}{Alt up}"
        }
        else
        {
            Send "{Alt up}"
        }
    }
}
