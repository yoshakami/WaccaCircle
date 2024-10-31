#SingleInstance
#Warn
if GetKeyState("Ctrl", "P")
{
    if GetKeyState("Shift", "P")
    {
        if GetKeyState("LWin", "P")
        {
            Send "{Ctrl down}{Shift down}{LWin down}{Alt down}"
        }
        else
        {
            Send "{Ctrl down}{Shift down}{Alt down}"
        }
    }
    else
    {
        if GetKeyState("LWin", "P")
        {
            Send "{Ctrl down}{LWin down}{Alt down}"
        }
        else
        {
            Send "{Ctrl down}{Alt down}"
        }
    }
}
else
{
    if GetKeyState("Shift", "P")
    {
        if GetKeyState("LWin", "P")
        {
            Send "{Shift down}{LWin down}{Alt down}"
        }
        else
        {
            Send "{Shift down}{Alt down}"
        }
    }
    else
    {
        if GetKeyState("LWin", "P")
        {
            Send "{LWin down}{Alt down}"
        }
        else
        {
            Send "{Alt down}"
        }
    }
}
