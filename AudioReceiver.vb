Imports System.Threading
Imports System.IO
Imports System.Runtime.InteropServices
Imports RadioPlayer.Extensions


Public Class AudioReceiver

#Region "Props"

    Public Property ChannelHandle As IntPtr
    Public Property Thread As Thread
    Public Property CurrentSongName As String
    Public Property Text As String
    Public Property CoverImage As Image
    Public Property CoverSource As String
    Public Property OutputPath As String
    Public Property OutputStream As Stream
    Public Property Playing As Boolean

    Public Property SongNameUpdate As Boolean
    Public Property GotHeader As Boolean
#End Region


#Region "Events"

    Public Event StatusUpdated(progress As Double, type As StatusType)
    Public Event CoverUpdated(sender As Object, url As String)
    Public Event SongNameUpdated(sender As Object, newName As String)
    Public Event SongUpdate(text As String, name As String, p3 As String, p4 As Image)

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
        ChannelHandle = BASS_StreamCreateURL(url, 0, BASS_STREAM_BLOCK Or BASS_STREAM_STATUS Or BASS_STREAM_AUTOFREE,
                                             AddressOf HandleDownload, 0)
    End Sub

    Public Sub Start()
        Playing = True
        If Thread IsNot Nothing AndAlso Thread.IsAlive Then
        Else
            Thread = New Thread(AddressOf RecieveMusic)
            Thread.Start()
        End If
    End Sub

#End Region

    Private Sub handleReceiving()
        Do
            If Not Playing Then Exit Do

            Dim progress As Long
            Dim flEndPos As Long = BASS_StreamGetFilePosition(ChannelHandle, BASS_FILEPOS_END)
            Try
                progress = BASS_StreamGetFilePosition(ChannelHandle, BASS_FILEPOS_BUFFER) * 100 / flEndPos _
                ' percentage of buffer filled
            Catch EX As Exception
                EX = EX
            End Try
            If (progress > 75 Or BASS_StreamGetFilePosition(ChannelHandle, BASS_FILEPOS_CONNECTED) = 0) Then _
                ' over 75% full (or end of download)
                DownloadStarted = False ' finished prebuffering, stop monitoring
                ' get the broadcast name and bitrate
                Dim icyPtr As IntPtr
                icyPtr = BASS_ChannelGetTags(ChannelHandle, BASS_TAG_ICY)
                If (icyPtr = IntPtr.Zero) Then icyPtr = BASS_ChannelGetTags(ChannelHandle, BASS_TAG_HTTP) _
                ' no ICY tags, try HTTP
                If (icyPtr) Then
                    Dim icyStr As String
                    Do
                        icyStr = PointerToString(icyPtr)
                        icyPtr = icyPtr.ToInt64 + Len(icyStr) + 1
                        CurrentSongName = If(Mid(icyStr, 1, 9) = "icy-name:", Mid(icyStr, 10), CurrentSongName)
                        Text = If(Mid(icyStr, 1, 7) = "icy-br:", "bitrate: " & Mid(icyStr, 8), Text)
                        RaiseSongUpdate(Text, CurrentSongName)

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

    Private Sub RecieveMusic()
        Try
            handleReceiving()
        Catch ex As Exception
            ex = ex
        End Try
    End Sub
    

    Public Sub HandleDownload(buffer As IntPtr, length As Integer, user As Integer)
        Return
        If (buffer.ToInt64 And length = 0) Then
            Me.Text = PointerToString(buffer) ' display connection status
            Exit Sub
        End If
        If String.IsNullOrEmpty(OutputPath) Then Exit Sub

        If (Not DownloadStarted) Then
            DownloadStarted = True
            Try
                If OutputStream IsNot Nothing Then OutputStream.Close()
                If (OutputStream.OpenFileWr(OutputPath)) Then
                    SongNameUpdate = False
                Else
                    SongNameUpdate = True
                    GotHeader = False
                End If
            Catch :
            End Try
        End If

        If (Not SongNameUpdate) Then
            If (length) Then
                Try
                    'Dim lpBuff As New SafeMemoryHandle(buffer)
                    'lpBuff.Initialize(length)
                    'Dim bw As New UnmanagedMemoryStream(lpBuff, 0, length)
                    'Dim buff As Byte() = New Byte(length) {}
                    'bw.Read(buff, 0, length)

                    'OutputStream.Write(buff, 0, length)
                Catch :
                End Try
            Else
                Try : OutputStream.Close() :
                Catch :
                End Try
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
        Dim tmpName As String

        If meta = IntPtr.Zero Then Exit Sub
        tmpMeta = PointerToString(meta)

        If ((Mid(tmpMeta, 1, 13) = "StreamTitle='")) Then
            p = tmpMeta.Substring(13)
            tmpName = p.Substring(0, p.IndexOf(";") - 1)
            RaiseEvent SongNameUpdated(Me, tmpName)
            ParseCover(p)

            If tmpName = CurrentSongName Then
                ' do noting
            Else
                CurrentSongName = tmpName
                Trace.WriteLine(String.Format("{0}-{1}", CurrentSongName, Text))
                GotHeader = False
                DownloadStarted = False
            End If

            OutputPath = Application.StartupPath & "\" & (Mid(p, 1, InStr(p, ";") - 2)).RemoveIllegalPathCharacters &
                         ".mp3"
            If OutputStream Is Nothing Then
                OutputStream = New FileStream(OutputPath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read)

            End If
        End If
    End Sub

#End Region

#Region "Synchronisers"

    Private Sub MetaSync(handle As Integer, channel As Integer, data As Integer, user As IntPtr)
        ParseMeta()
    End Sub

    Private Sub EndSync(handle As Integer, channel As Integer, data As Integer, user As IntPtr)
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
