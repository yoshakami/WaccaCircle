#SingleInstance
#Warn
if GetKeyState("Alt", "P")
{
	if GetKeyState("Ctrl", "P")
    {
        if GetKeyState("Shift", "P")
        {
                Send "{Alt down}{Ctrl down}{Shift down}{LWin up}"
            
        }
        else
        {
                Send "{Alt down}{Ctrl down}{LWin up}"
            
        }
    }
	else
	{
        if GetKeyState("Shift", "P")
        {
                Send "{Alt down}{Shift down}{LWin up}"
            
        }
        else
        {
                Send "{Alt down}{LWin up}"
            
        }
	}
}
else
{
	if GetKeyState("Ctrl", "P")
    {
        if GetKeyState("Shift", "P")
        {
                Send "{Ctrl down}{Shift down}{LWin up}"
            
        }
        else
        {
                Send "{Ctrl down}{LWin up}"
            
        }
    }
	else
	{
        if GetKeyState("Shift", "P")
        {
                Send "{Shift down}{LWin up}"
            
        }
        else
        {
                Send "{LWin up}"
            
        }
	}
}