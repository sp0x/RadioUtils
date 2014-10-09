Imports System.IO
Imports System.Runtime.InteropServices
Imports System.Security.Permissions
Imports RadioPlayer.Extensions
Imports RadioPlayer.StreamExtensions
Imports Microsoft.Win32.SafeHandles


Public Class AudioReceiver

#Region "Props"
    Public Property ChannelHandle As IntPtr
    Public Property Thread As System.Threading.Thread
    Public Property CurrentSongName As String
    Public Property Text As String
    Public Property CoverImage As System.Drawing.Image
    Public Property CoverSource As String
    Public Property OutputPath As String
    Public Property OutputStream As Stream
    Public Property Working As Boolean
#End Region


#Region "Events"

    Public Event StatusUpdated(progress As Double, type As StatusType)
    Public Event CoverUpdated(sender As Object, url As String)
    Public Event SongNameUpdated(sender As Object, newName As String)
    Public Event SongUpdate(text As String, name As String, p3 As String, p4 As System.Drawing.Image)

    Private Property DownloadStarted As Boolean

    Public Sub RaiseSongUpdate(text As String, name As String)
        RaiseEvent SongUpdate(text, name, CurrentSongName, CoverImage)
    End Sub

#End Region

#Region "Construction"
    Public Sub New(handle As IntPtr)
        ChannelHandle = handle
    End Sub
    Public Sub New(url As String)
        ChannelHandle = BASS_StreamCreateURL(url, 0, BASS_STREAM_BLOCK Or BASS_STREAM_STATUS Or BASS_STREAM_AUTOFREE, _
                                             AddressOf SUBDOWNLOADPROC, 0)
    End Sub

    Public Sub Start()
        If Thread IsNot Nothing AndAlso Thread.IsAlive Then
        Else
            Thread = New Threading.Thread(AddressOf RecieveMusic)
            Thread.Start()
        End If
    End Sub
#End Region


    Private Sub RecieveMusic()
        Do
            If Not Working Then Exit Do

            Dim progress As Long
            Dim flEndPos As Long = BASS_StreamGetFilePosition(ChannelHandle, BASS_FILEPOS_END)
            progress = BASS_StreamGetFilePosition(ChannelHandle, BASS_FILEPOS_BUFFER) * 100 / flEndPos    ' percentage of buffer filled
            If (progress > 75 Or BASS_StreamGetFilePosition(ChannelHandle, BASS_FILEPOS_CONNECTED) = 0) Then ' over 75% full (or end of download)
                Working = False ' finished prebuffering, stop monitoring
                ' get the broadcast name and bitrate
                Dim icyPtr As IntPtr
                icyPtr = BASS_ChannelGetTags(ChannelHandle, BASS_TAG_ICY)
                If (icyPtr = IntPtr.Zero) Then icyPtr = BASS_ChannelGetTags(ChannelHandle, BASS_TAG_HTTP) ' no ICY tags, try HTTP
                If (icyPtr) Then
                    Dim icyStr As String
                    Dim name As String
                    Do
                        icyStr = PointerToString(icyPtr)
                        icyPtr = icyPtr.ToInt64 + Len(icyStr) + 1
                        name = If(Mid(icyStr, 1, 9) = "icy-name:", Mid(icyStr, 10), name)
                        Text = If(Mid(icyStr, 1, 7) = "icy-br:", "bitrate: " & Mid(icyStr, 8), Text)
                        RaiseSongUpdate(Text, name)

                        ' NOTE: you can get more ICY info like: icy-genre:, icy-url:... :)
                    Loop While (icyStr <> "")
                End If

                ' get the stream title and set sync for subsequent titles
                ParseMeta()

                BASS_ChannelSetSync(ChannelHandle, BASS_SYNC_META, 0, AddressOf MetaSync, 0)
                ' set sync for end of stream
                BASS_ChannelSetSync(ChannelHandle, BASS_SYNC_END, 0, AddressOf EndSync, 0)
                ' play it!
                BASS_ChannelPlay(ChannelHandle, BASSFALSE)
            Else
                RaiseEvent StatusUpdated(progress, StatusType.Buffering)
                '  Me.lblName = "buffering... " & progress & "%"
            End If


        Loop
    End Sub
    Public Class SafeMemoryHandle
        Inherits SafeBuffer


        <DllImport("kernel32.dll", SetLastError:=True)> _
        Public Shared Function CloseHandle(ByVal hObject As IntPtr) As <MarshalAs(UnmanagedType.Bool)> Boolean
        End Function
        Public Sub New()
            MyBase.New(True)
        End Sub
        Public Sub New(ptr As IntPtr)
            MyBase.New(True)
            handle = ptr
        End Sub
        Protected Overrides Function ReleaseHandle() As Boolean
            Return CloseHandle(Me.handle)
        End Function
        
    End Class
    Public Sub SUBDOWNLOADPROC(buffer As IntPtr, length As Long, user As Long)
        If (buffer.ToInt64 And length = 0) Then
            Me.Text = PointerToString(buffer) ' display connection status
            Exit Sub
        End If
        Dim lpBuff As New SafeMemoryHandle(buffer)
        lpBuff.Initialize(length)
        Dim bw As New UnmanagedMemoryStream(lpBuff, 0, length)
        Dim buff As Byte() = New Byte(length) {}
        bw.Read(buff, 0, length)
        'If (Trim(OutputPath) = "") Then Exit Sub
        Dim SongNameUpdate As Boolean = True
        If (Not DownloadStarted) Then
            DownloadStarted = True
            Try
                If OutputStream IsNot Nothing Then OutputStream.Close()
                If (OutputStream.OpenFileWr(OutputPath)) Then
                    '     SongNameUpdate = False
                Else
                    'SongNameUpdate = True
                    'GotHeader = False
                End If
            Catch : End Try
        End If

        If (Not SongNameUpdate) Then
            If (length) Then
                Try : OutputStream.Write(buff, 0, length) : Catch : End Try
            Else
                Try : OutputStream.Close() : Catch : End Try
                'GotHeader = False
            End If
        Else
            DownloadStarted = False
            If OutputStream IsNot Nothing Then OutputStream.Close()
            'GotHeader = False
        End If
    End Sub

#Region "Meta Processors"

    Private Sub ParseCover(p As String)
        If p.Contains(".jpg") Or p.Contains(".png") Or p.Contains(".bmp") Then
            Dim strt As Int32 = p.IndexOf("StreamUrl=") + "StreamUrl='".Length
            CoverSource = p.Substring(strt, p.LastIndexOf("';") - strt)
            RaiseEvent CoverUpdated(Me, CoverSource)
        End If
    End Sub
    '
    Private Sub ParseMeta()
        Dim meta As IntPtr = BASS_ChannelGetTags(ChannelHandle, BASS_TAG_META)
        Dim p As String
        Dim tmpMeta As String
        If meta = IntPtr.Zero Then Exit Sub
        tmpMeta = PointerToString(meta)

        If ((Mid(tmpMeta, 1, 13) = "StreamTitle='")) Then
            p = tmpMeta.Substring(14)
            CurrentSongName = p.Substring(0, p.IndexOf(";") - 1)
            RaiseEvent SongNameUpdated(Me, CurrentSongName)
            ParseCover(p)
            'RaiseSongUpdate("", CurrentSongName)
            'If TmpNameHold = TmpNameHold2 Then
            '    ' do noting
            'Else
            '    'TmpNameHold2 = TmpNameHold
            '    'GotHeader = False
            '    'DownloadStarted = False
            'End If
            OutputPath = Application.StartupPath & "\" & (Mid(p, 1, InStr(p, ";") - 2)).RemoveIllegalPathCharacters & ".mp3"
            If OutputStream Is Nothing Then
                OutputStream = New FileStream(OutputPath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read)

            End If
        End If
    End Sub

#End Region

#Region "Synchronisers"

    Private Sub MetaSync(ByVal handle As Long, ByVal data As Long, ByVal user As Long)
        ParseMeta()
    End Sub

    Private Sub EndSync(handle As IntPtr, data As Long, user As Long)
        With Me
            '.lblName = "not playing"
            '.Text = ""
            '.lblSong = ""
        End With
    End Sub

#End Region

#Region "Disposition"
    Sub Close()
        If Not ChannelHandle = IntPtr.Zero Then
            BASS_StreamFree(ChannelHandle)
        End If
    End Sub
#End Region


End Class

Public Enum StatusType
    Buffering
    ICY
End Enum
