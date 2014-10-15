Imports System.Runtime.InteropServices

<Serializable, StructLayout(LayoutKind.Sequential, CharSet:=CharSet.Auto)> _
Public NotInheritable Class BASS_FILEPROCS
    Public close As Filecloseproc
    Public length As Filelenproc
    Public read As Filereadproc
    Public seek As Fileseekproc
    Public Sub New(closeCallback As Filecloseproc, lengthCallback As Filelenproc, readCallback As Filereadproc, seekCallback As Fileseekproc)
        Me.close = closeCallback
        Me.length = lengthCallback
        Me.read = readCallback
        Me.seek = seekCallback
    End Sub
End Class