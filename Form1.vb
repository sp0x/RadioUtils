Imports System.Threading
Imports System.IO


Public Class Form1
    Public Property BSRDPLS() As RadioPlayer
    Public Property Stream As ShoutcastStream
    Public Property StreamPlayer As StreamPlayer
    
    Public Sub Loaded() Handles Me.Load
        On Error Resume Next
        ''Dim buu As Int32() = New Int32(8192) {}
        ''Dim fl As String = "g:\downloads\music.mp3"
        ''Dim si As IntPtr = BASS_StreamCreatePush(44100, 1, 0, Nothing)
        ''For g As Int32 = 0 To 8192
        ''    buu(g) = Int(Math.Sin((6.2832 * (500000000 / (g * g + 1))) / 10) * 10000.0)
        ''Next

        ''BASS_StreamPutData(si, buu, 16384)
        ''BASS_ChannelPlay(si, False)
        'Dim fsIn As New FileStream(fl, FileMode.Open, FileAccess.Read, FileShare.Read)
        'Dim buff As Byte() = New Byte(2048) {}

        'Bass.BASS_Init(-1, 44100, BASSInit.BassDevice_8Bits, 0)
        'Dim strm = Bass.BASS_StreamCreateFile(fl, 0, 0, 0)
        'Dim pStrm = Bass.BASS_StreamCreatePush(44100, 1, 0, Nothing)
        'Dim bRead As Int32 = 0
        'BASS_ChannelPlay(pStrm, False)
        'Do
        '    bRead = fsIn.Read(buff, 0, buff.Length)
        '    Dim i = BASS_StreamPutData(pStrm, buff, bRead)
        '    If i = -1 Then
        '        Dim err As String = BASS_ErrorGetCode.ToString
        '        err = err
        '    End If
        '    BASS_ChannelPlay(pStrm, False)
        'Loop While bRead > 0

        ''BASS_ChannelPlay(strm, False)


        ''Dim x = (a * (Math.Abs(CInt(a < b)))) Or (b * Math.Abs(CInt(a > b)))

        Dim url As String = "http://www.radioparadise.com/musiclinks/rp_128-9.m3u"
        TextBox1.Text = url
        Dim tsAction = Sub()
                           Stream = New ShoutcastStream(url)
                           StreamPlayer = New StreamPlayer(Stream)
                           StreamPlayer.Play()
                       End Sub
        Tasks.Task.Factory.StartNew(tsAction)
    End Sub



    Public Sub CHNGVol(ByVal sender As Object, ByVal e As EventArgs) Handles TrackBar1.ValueChanged
        Dim vol As Double = TrackBar1.Value/100
        BASS_SetVolume(vol)
        Dim err As Int64 = BASS_ErrorGetCode
        err = err
    End Sub

    Private Sub Button2_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BtnPlay.Click
        Try
            Stream = New ShoutcastStream(TextBox1.Text)
            StreamPlayer = New StreamPlayer(Stream)
            StreamPlayer.PlayThreaded()
            'Dim bs As New RadioPlayer(New RadioPlayer.UpInfo(AddressOf UpdateSongInfo), Me.TextBox1.Text)
            ''Me.Url = TextBox1.Text
            'Dim th As New Thread(AddressOf bs.Play) With {.IsBackground = True}
            'th.Start()
        Catch ex As Exception
            ex = ex
        End Try
    End Sub

    Public Sub UpdateSongInfo(ByVal txt As String, ByVal name As String, ByVal sngname As String, ByVal px As Image)
        If Me.InvokeRequired Then
            Me.Invoke(New RadioPlayer.UpInfo(AddressOf UpdateSongInfo), txt, name, sngname, px)
        Else
            Me.PictureBox1.Image = px
            Me.Text = txt
            Me.lblName.Text = name
            Me.lblSong.Text = sngname
        End If
    End Sub

    End Class
