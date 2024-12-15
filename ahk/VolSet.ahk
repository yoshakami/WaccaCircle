#Requires AutoHotkey v2.0

; Check if an argument is passed
if A_Args.Length > 0 {
    volume := A_Args[1] ; Get the first command-line argument
    volume := Integer(volume) ; Ensure it's an integer

    ; Validate the range (0-100)
    if (volume >= 0 && volume <= 100) {
        SoundSetVolume(volume) ; Set the system volume
    }
}
