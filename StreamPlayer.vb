Imports System.IO
Imports System.Runtime.InteropServices
Imports System.Reflection

Public Class StreamPlayer
    Implements IStreamPlayer
    Implements IDisposable

#Region "Variables"

    Public Property Thread As System.Threading.Thread
    Public Property PlaybackStream() As IntPtr Implements IStreamPlayer.PlaybackStream
    Public Property BaseStream As Stream Implements IStreamPlayer.BaseStream
    Public Property AudioBuffer As Byte() = New Byte(1024 * 100) {}
    Public Property BufferHandle As IntPtr
    Public Property Position As Int32
#End Region

#Region "Construction"
    Public Sub New(stream As Stream)
        BaseStream = stream
    End Sub
#End Region

#Region "Events"
    Public Event StatusUpdated(perc As Double, type As StatusType)

#End Region

#Region "Initiation and config"

    Private Sub ConfigSound()
        'Call BASS_SetConfig(BASSConfig.BASS_CONFIG_NET_PLAYLIST, 1) ' enable playlist processing
        'Call BASS_SetConfig(BASSConfig.BASS_CONFIG_NET_PREBUF, 0) _
        ' minimize automatic pre-buffering, so we can do it (and display it) instead
    End Sub

    Private Function InitBass() As Boolean
        Dim bv As Long = HiWord(BASS_GetVersion())
        If bv <> BASSVERSION Then MsgBox("INVALID BASS.DLL VERSION!", MsgBoxStyle.Critical, "ERROR!") : Return False
        Dim init As Long = BASS_Init(-1, 44100, BASSInit.BassDevice_8Bits, 0, Nothing)

        If init = 0 Then MsgBox("Can't initialize device") : Return False
        ConfigSound()
        Return True
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
#End Region

#Region "Stream output"

    Private Function ExecuteAudioStream(buffer As IntPtr, length As Integer, user As IntPtr) As Int32
        Dim nRead As Int32
        If BaseStream Is Nothing Then
            Return 0
        End If
        If buffer = IntPtr.Zero Then
            Return 0
        End If

        If BaseStream IsNot Nothing AndAlso BaseStream.CanRead Then
            nRead = BaseStream.Read(AudioBuffer, 0, length)
            Position += nRead
        End If
        If nRead > 0 Then
            Marshal.Copy(AudioBuffer, 0, buffer, nRead)
        End If
        Return nRead
    End Function

#End Region


#Region "Playback"
    Public Sub Play() Implements IStreamPlayer.Play
        Dim buff As Byte() = New Byte(1024 * 100 * 1) {}
        If PlaybackStream = IntPtr.Zero Then
            If Not Initialize() Then
                Throw New InvalidOperationException
            End If
        End If
        Do While BaseStream.CanRead
            Dim prcComplete As Double = 0D
            Dim pos As Long = BASS_StreamGetFilePosition(PlaybackStream, BASS_FILEPOS_END)
            Try
                prcComplete = BASS_StreamGetFilePosition(PlaybackStream, BASS_FILEPOS_BUFFER) * 100 / pos
                playStream(prcComplete)
            Catch EX As Exception
                Trace.WriteLine(EX.Message)
            End Try
        Loop


    End Sub
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
    Public Function [Stop]() As Boolean Implements IStreamPlayer.[Stop]
        Return Bass.BASS_ChannelStop(PlaybackStream)
    End Function



    Private Sub playStream(bufferLoadPercentage As Double)
        Dim posConnected As Long = BASS_StreamGetFilePosition(PlaybackStream, BASS_FILEPOS_CONNECTED)
        If (bufferLoadPercentage >= 75 Or posConnected = 0) Then
            BASS_ChannelPlay(PlaybackStream, False)
        Else
            RaiseEvent StatusUpdated(bufferLoadPercentage, StatusType.Buffering)
            '  Trace.WriteLine("buffering... " & bufferLoadPercentage.ToString() & "%")
        End If
    End Sub

#End Region

    
    #Region "Disposing"
    Public Sub Dispose() Implements IDisposable.Dispose
        If Not PlaybackStream = IntPtr.Zero Then
            BASS_StreamFree(PlaybackStream)
        End If
    End Sub
#End Region


End Class