Imports System.IO
Imports System.Runtime.InteropServices
Imports System.Reflection

Public Class StreamPlayer
    Implements IStreamPlayer
    Implements IDisposable

    Public Property Thread As System.Threading.Thread
    Public Property Handle() As IntPtr Implements IStreamPlayer.Handle
    Public Property BaseStream As Stream Implements IStreamPlayer.BaseStream
    Public Property AudioBuffer As Byte() = New Byte(1024 * 100) {}
    Public Property BufferHandle As IntPtr

#Region "Construction"
    Public Sub New(stream As Stream)
        BaseStream = stream
    End Sub
#End Region


#Region "Initiation and config"

    Private Sub ConfigSound()
        Call BASS_SetConfig(BASSConfig.BASS_CONFIG_NET_PLAYLIST, 1) ' enable playlist processing
        Call BASS_SetConfig(BASSConfig.BASS_CONFIG_NET_PREBUF, 0) _
        ' minimize automatic pre-buffering, so we can do it (and display it) instead
    End Sub

    Private Function InitBass() As Boolean
        Dim bv As Long = HiWord(BASS_GetVersion())
        If bv <> BASSVERSION Then MsgBox("INVALID BASS.DLL VERSION!", MsgBoxStyle.Critical, "ERROR!") : Return False
        Dim init As Long = BASS_Init(-1, 44100, 0, Me.Handle, Nothing)
        If init = 0 Then MsgBox("Can't initialize device") : Return False
        ConfigSound()
       Return True
    End Function
#End Region

    Public Function Initialize() As Boolean
        InitBass()
        Dim gch = GCHandle.Alloc(AudioBuffer, GCHandleType.Pinned)
        BufferHandle = gch.AddrOfPinnedObject()
        Handle = BASS_StreamCreatePush(44100, 2, BASS_STREAM_BLOCK Or BASS_STREAM_STATUS Or BASS_STREAM_AUTOFREE, 0)
        
        If Handle = IntPtr.Zero Then
            Handle = BASS_ErrorGetCode
            Dim err As BassError = Handle
            Throw New Exception("Bass: " & err.ToString)
        End If
        Return Not Handle = IntPtr.Zero
    End Function


    Public Sub Play() Implements IStreamPlayer.Play
        Dim buff As Byte() = New Byte(1024 * 100 * 1) {}
        If Handle = IntPtr.Zero Then
            If Not Initialize() Then
                Throw New InvalidOperationException
            End If
        End If
        Do While BaseStream.CanRead
            Dim nRead As Int32 = BaseStream.Read(buff, 0, buff.Length)
            If nRead = 0 Then Exit Do
            Dim pg As IntPtr = BASS_StreamPutData(Handle, buff, nRead)
            Dim pFlag As Int32 = 0
            pFlag = Bass.BASS_ChannelPlay(Handle, False)
            '   pg = BASS_ChannelGetData(Handle, buff, 0)
            'pg = BASS_ChannelGetPosition(Handle)

            If pFlag > -1 Then
                ' playing...
            Else
                '  Trace.WriteLine(String.Format("Err= {0}", Bass.BASS_ErrorGetCode()))
            End If
            '  Marshal.FreeHGlobal(lpBuff) ' ;//release memory from heap
        Loop
    End Sub

    Private Sub DupDSP(handle As IntPtr, channel As Integer,
                   buffer As IntPtr, length As Integer, user As IntPtr)
        Bass.BASS_StreamPutData(user.ToInt32(), buffer, length)
    End Sub

    Public Function [Stop]() As Boolean Implements IStreamPlayer.[Stop]
        Return Bass.BASS_ChannelStop(Handle)
    End Function
    
    Public Sub PlayThreaded() Implements IStreamPlayer.PlayThreaded
        If Thread Is Nothing Then
            Thread = New System.Threading.Thread(AddressOf Play)
            Thread.Start()
        Else
            If Thread.IsAlive Then
            Else
            End If
        End If
    End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        If Not Handle = IntPtr.Zero Then
            BASS_StreamFree(Handle)
        End If
    End Sub
End Class