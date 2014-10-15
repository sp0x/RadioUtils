Imports System.IO
Imports System.Runtime.InteropServices
Imports System.Reflection

Public Class StreamPlayer
    Implements IStreamPlayer
    Implements IDisposable

    Public Property Thread As System.Threading.Thread
    Public Property PlaybackStream() As IntPtr Implements IStreamPlayer.PlaybackStream
    Public Property BaseStream As Stream Implements IStreamPlayer.BaseStream
    Public Property AudioBuffer As Byte() = New Byte(1024 * 100) {}
    Public Property BufferHandle As IntPtr
    Public Property Position As Int32

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
        Dim init As Long = BASS_Init(-1, 44100, BASSInit.BassDevice_8Bits, IntPtr.Zero, Nothing)

        If init = 0 Then MsgBox("Can't initialize device") : Return False
        ConfigSound()
        Return True
    End Function
#End Region

    Private Function ExecuteAudioStream(buffer As IntPtr, length As Integer, user As IntPtr) As Int32
        Dim nRead As Int32
        If BaseStream IsNot Nothing AndAlso BaseStream.CanRead Then
            nRead = BaseStream.Read(AudioBuffer, 0, length)
            Position += nRead
        End If
        If nRead > 0 Then Marshal.Copy(AudioBuffer, 0, buffer, nRead)
        Return nRead
    End Function

    Public Function Initialize() As Boolean
        InitBass()
        Dim fileproc As New BASS_FILEPROCS(Nothing, Nothing, Nothing, Nothing)
        fileproc.close = New Filecloseproc(Sub(user As IntPtr) Trace.WriteLine("closeproc"))
        fileproc.length = New Filelenproc(Function(user As IntPtr) 0)
        fileproc.read = AddressOf ExecuteAudioStream

        PlaybackStream = BASS_StreamCreateFileUser(BassStreamSystem.StreamfileBuffer, BASS_STREAM_BLOCK Or BASS_STREAM_STATUS Or BASS_STREAM_AUTOFREE, fileproc, IntPtr.Zero)
        If PlaybackStream = IntPtr.Zero Then
            PlaybackStream = BASS_ErrorGetCode
            Dim err As BassError = PlaybackStream
            Throw New Exception("Bass: " & err.ToString)
        End If
        Return Not PlaybackStream = IntPtr.Zero
    End Function



    Private Sub DoPlayback()
        Dim pflag As Boolean = Bass.BASS_ChannelPlay(PlaybackStream, False)
        If pflag Then
            ' playing...
        Else
            Trace.WriteLine(String.Format("PlayErr= {0}", Bass.BASS_ErrorGetCode()))
        End If
    End Sub

    Public Sub Play() Implements IStreamPlayer.Play
        Dim buff As Byte() = New Byte(1024 * 100 * 1) {}
        If PlaybackStream = IntPtr.Zero Then
            If Not Initialize() Then
                Throw New InvalidOperationException
            End If
        End If
        Do While BaseStream.CanRead
            ' ''Dim nRead As Int32 = BaseStream.Read(buff, 0, buff.Length)
            ' ''Position += nRead
            ' Trace.WriteLine(String.Format("Red {0} bytes", nRead))
            'If nRead = 0 Then Exit Do
            '   _pushBuffer(buff, nRead)
            '    _playPlayback()

            'If pFlag > -1 Then
            '    ' playing...
            'Else
            '    '  Trace.WriteLine(String.Format("Err= {0}", Bass.BASS_ErrorGetCode()))
            'End If
            '  Marshal.FreeHGlobal(lpBuff) ' ;//release memory from heap
        Loop
    End Sub

    Private Sub DupDSP(handle As IntPtr, channel As Integer,
                   buffer As IntPtr, length As Integer, user As IntPtr)
        Bass.BASS_StreamPutData(user.ToInt32(), buffer, length)
    End Sub

    Public Function [Stop]() As Boolean Implements IStreamPlayer.[Stop]
        Return Bass.BASS_ChannelStop(PlaybackStream)
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
        If Not PlaybackStream = IntPtr.Zero Then
            BASS_StreamFree(PlaybackStream)
        End If
    End Sub
End Class