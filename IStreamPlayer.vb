Imports System.IO

Public Interface IStreamPlayer
    Sub Play()
    Function [Stop]() As Boolean
    Sub PlayThreaded()
    Property PlaybackStream As IntPtr
    Property BaseStream As Stream
End Interface