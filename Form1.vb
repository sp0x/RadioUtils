Imports System.Threading


Public Class Form1
    Public Property BSRDPLS() As RadioPlayer
    Public Property Stream As ShoutcastStream
    Public Property StreamPlayer As StreamPlayer
    
    Public Sub Loaded() Handles Me.Load
        TextBox1.Text = "http://www.radioparadise.com/musiclinks/rp_128-9.m3u"
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
