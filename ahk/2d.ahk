#SingleInstance
#Warn
if GetKeyState("Alt", "P")
{
	if GetKeyState("Ctrl", "P")
    {
        if GetKeyState("Shift", "P")
        {
                Send "{Alt down}{Ctrl down}{Shift down}{LWin down}"
            
        }
        else
        {
                Send "{Alt down}{Ctrl down}{LWin down}"
            
        }
    }
	else
	{
        if GetKeyState("Shift", "P")
        {
                Send "{Alt down}{Shift down}{LWin down}"
            
        }
        else
        {
                Send "{Alt down}{LWin down}"
            
        }
	}
}
else
{
	if GetKeyState("Ctrl", "P")
    {
        if GetKeyState("Shift", "P")
        {
                Send "{Ctrl down}{Shift down}{LWin down}"
            
        }
        else
        {
                Send "{Ctrl down}{LWin down}"
            
        }
    }
	else
	{
        if GetKeyState("Shift", "P")
        {
                Send "{Shift down}{LWin down}"
            
        }
        else
        {
                Send "{LWin down}"
            
        }
	}
}