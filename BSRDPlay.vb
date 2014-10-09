Imports System.Threading


Public Class BSRDPlay
    Public Text As String
    Public lblName As String
    Public lblSong As String
    Public pictBX As Image
    Public Upd As UpInfo

    Public Delegate Sub UpInfo(ByVal txt As String, ByVal lblname As String, ByVal lblSong As String, ByVal pictbx As Image)

    Public Sub UpdateInfo()
        Me.Upd.Invoke(Text, lblName, lblSong, pictBX)
    End Sub

#Region "Construction"
    Public Sub New(ByRef up As UpInfo, _
                    url As String)
        Me.Upd = up
        Me.Url = url
        WriteFile = New clsFileIo(Me)
    End Sub
#End Region


#Region "Info"
    Public Property Channel As Long
    Public Property GotHeader As Boolean
    Public Property Url As String
    Public Property TmpNameHold As String
    Public Property TmpNameHold2 As String

    ' SAVE LOCAL COPY
    Public WriteFile As New clsFileIo(Me)

    Public FileIsOpen As Boolean
    Public DownloadStarted As Boolean
    ' DoDownload As Boolean
    Public DlOutput As String
    Dim SongNameUpdate As Boolean
#End Region


    Public Sub LoadRadioTHR()
        OpenURL(Me.Url)
    End Sub

    Public Property SDWNHndlr As DOWNLOADPROC_Handler = New DOWNLOADPROC_Handler(AddressOf SUBDOWNLOADPROC)

    Public Sub OpenURL(clkURL As String)
        With Me
            Refresh = False
            Call BASS_StreamFree(Channel)
            .lblName = "connecting..."
            .Text = ""
            .lblSong = ""
            Channel = BASS_StreamCreateURL(clkURL, 0, BASS_STREAM_BLOCK Or BASS_STREAM_STATUS Or BASS_STREAM_AUTOFREE, SDWNHndlr, 0) 'AddressOf SUBDOWNLOADPROC, 0)
            If Channel = 0 Then
                .lblName = "not playing"
                Throw New Exception("Can't play the stream " & BASS_ErrorGetCode)
            Else
                StartRCMusic()
            End If
        End With
    End Sub

    Public Sub Clear()

    End Sub





    ' The following functions where added by Peter Hebels
    Public Sub SUBDOWNLOADPROC(ByVal buffer As IntPtr, ByVal length As Long, ByVal user As Long)
        If (buffer.ToInt64 And length = 0) Then
            Me.Text = PointerToString(buffer) ' display connection status
            Exit Sub
        End If

        If (Trim(DlOutput) = "") Then Exit Sub

        If (Not DownloadStarted) Then
            DownloadStarted = True
            Try
                Call WriteFile.CloseFile()


                If (WriteFile.OpenFile(DlOutput)) Then
                    SongNameUpdate = False
                Else

                    SongNameUpdate = True

                    GotHeader = False
                End If
            Catch : End Try
        End If

        If (Not SongNameUpdate) Then
            If (length) Then
                Try : Call WriteFile.WriteBytes(buffer, length) : Catch : End Try
            Else
                Try : Call WriteFile.CloseFile() : Catch : End Try
                GotHeader = False
            End If
        Else
            DownloadStarted = False
            Call WriteFile.CloseFile()
            GotHeader = False
        End If
    End Sub

    Public Function RemoveSpecialChar(ByVal strFileName As String)
        Dim i As Byte
        Dim SpecialChar As Boolean
        Dim SelChar As String = String.Empty
        Dim OutFileName As String = String.Empty

        For i = 1 To Len(strFileName)
            SelChar = Mid(strFileName, i, 1)
            SpecialChar = InStr(":/\?*|<>" & Chr(34), SelChar) > 0

            If (Not SpecialChar) Then
                OutFileName = OutFileName & SelChar
                SpecialChar = False
            Else
                OutFileName = OutFileName
                SpecialChar = False
            End If
        Next i

        RemoveSpecialChar = OutFileName
    End Function

    Public MSYNCH As SYNCPROC_Handler = New SYNCPROC_Handler(AddressOf MetaSync)
    Public ESYNCH As SYNCPROC_Handler = New SYNCPROC_Handler(AddressOf EndSync)
    Public Refresh As Boolean = False

    Public Sub StartRCMusic()
        Me.Refresh = True
        Dim th As New Thread(AddressOf RecieveMusic) With {.IsBackground = True}
        th.Start()
    End Sub

    Private Sub RecieveMusic()
        Do
            If Not Me.Refresh Then Exit Do

            Dim progress As Long
            progress = BASS_StreamGetFilePosition(Channel, BASS_FILEPOS_BUFFER) * 100 / BASS_StreamGetFilePosition(Channel, BASS_FILEPOS_END)    ' percentage of buffer filled
            If (progress > 75 Or BASS_StreamGetFilePosition(Channel, BASS_FILEPOS_CONNECTED) = 0) Then ' over 75% full (or end of download)
                Refresh = False ' finished prebuffering, stop monitoring
                ' get the broadcast name and bitrate
                Dim icyPtr As IntPtr
                icyPtr = BASS_ChannelGetTags(Channel, BASS_TAG_ICY)
                If (icyPtr = 0) Then icyPtr = BASS_ChannelGetTags(Channel, BASS_TAG_HTTP) ' no ICY tags, try HTTP
                If (icyPtr) Then
                    Dim icyStr As String
                    Do
                        icyStr = PointerToString(icyPtr)
                        icyPtr = icyPtr.ToInt64 + Len(icyStr) + 1
                        Me.lblName = IIf(Mid(icyStr, 1, 9) = "icy-name:", Mid(icyStr, 10), Me.lblName)
                        Me.Text = IIf(Mid(icyStr, 1, 7) = "icy-br:", "bitrate: " & Mid(icyStr, 8), Me.Text)
                        UpdateInfo()

                        ' NOTE: you can get more ICY info like: icy-genre:, icy-url:... :)
                    Loop While (icyStr <> "")
                End If

                ' get the stream title and set sync for subsequent titles
                WorkMeta()

                BASS_ChannelSetSync(Channel, BASS_SYNC_META, 0, Me.MSYNCH, 0)
                ' set sync for end of stream
                BASS_ChannelSetSync(Channel, BASS_SYNC_END, 0, Me.ESYNCH, 0)
                ' play it!
                BASS_ChannelPlay(Channel, BASSFALSE)
            Else
                Me.lblName = "buffering... " & progress & "%"
            End If


        Loop
    End Sub


    Public Sub SetSNGTXT(ByVal txt As String)
        Me.lblSong = txt
    End Sub

    Public Sub SetPict(ByVal url As String)
        Try
            Dim ms As IO.MemoryStream = New IO.MemoryStream(New Net.WebClient().DownloadData(url))
            Dim img As Drawing.Image = Drawing.Image.FromStream(ms)
            ms.Close()
            Me.pictBX = img
            ' Me.pictBx.SizeMode = PictureBoxSizeMode.StretchImage
        Catch ex As Exception

        End Try
    End Sub



    Public Sub GetCover(ByVal p As String)
        If p.Contains(".jpg") Or p.Contains(".png") Or p.Contains(".bmp") Then
            Dim STRT As Int32 = p.IndexOf("StreamUrl=") + "StreamUrl='".Length
            Dim url As String = p.Substring(STRT, p.LastIndexOf("';") - STRT)
            SetPict(url)
        End If
    End Sub


    Sub WorkMeta()
        Dim meta As IntPtr = BASS_ChannelGetTags(Channel, BASS_TAG_META)
        Dim p As String
        Dim tmpMeta As String
        If meta = IntPtr.Zero Then Exit Sub
        tmpMeta = PointerToString(meta)

        If ((Mid(tmpMeta, 1, 13) = "StreamTitle='")) Then
            p = tmpMeta.Substring(14)
            TmpNameHold = p.Substring(0, p.IndexOf(";") - 1)
            SetSNGTXT(TmpNameHold)

            GetCover(p)
            UpdateInfo()
            If TmpNameHold = TmpNameHold2 Then
                ' do noting
            Else
                TmpNameHold2 = TmpNameHold
                GotHeader = False
                DownloadStarted = False
            End If

            DlOutput = Application.StartupPath & "\" & RemoveSpecialChar(Mid(p, 1, InStr(p, ";") - 2)) & ".mp3"
        End If
    End Sub

    Sub MetaSync(ByVal handle As Long, ByVal data As Long, ByVal user As Long)
        WorkMeta()
    End Sub

   Private Sub EndSync(handle As IntPtr, data As Long, user As Long)
        With Me
            .lblName = "not playing"
            .Text = ""
            .lblSong = ""
        End With
    End Sub

End Class