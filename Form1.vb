Imports RadioPlayer.Bass
Imports System.Threading


Public Class Form1
    Public BSRDPLS() As BSRDPlay

#Region "Initiation and config"

    Public Sub ConfigSound()
        Call BASS_SetConfig(BASS_CONFIG_NET_PLAYLIST, 1) ' enable playlist processing
        Call BASS_SetConfig(BASS_CONFIG_NET_PREBUF, 0) ' minimize automatic pre-buffering, so we can do it (and display it) instead
        Call BASS_SetConfigPtr(BASS_CONFIG_NET_PROXY, 0)  ' setup proxy server location
    End Sub

    Public Function InitBASS()
        Dim bv As Long = HiWord(BASS_GetVersion)
        If bv <> BASSVERSION Then MsgBox("INVALID BASS.DLL VERSION!", MsgBoxStyle.Critical, "ERROR!") : Return False
        Dim init As Long = BASS_Init(-1, 44100, 0, Me.Handle, 0)
        If init = 0 Then MsgBox("Can't initialize device") : Return False
        Return True
    End Function

#End Region


    Public Sub ld() Handles Me.Load
        If Not InitBASS() Then Return
        ConfigSound()
        TextBox1.Text = "http://www.radioparadise.com/musiclinks/rp_128-9.m3u"
    End Sub

    Public Declare Ansi Function BASS_SetVolume Lib "bass.dll" (ByVal vol As Single) As BassBool
    Public Sub CHNGVol(ByVal sender As Object, ByVal e As EventArgs) Handles TrackBar1.ValueChanged
        Dim vol As Double = TrackBar1.Value / 100
        Dim reps As BassBool = Me.BASS_SetVolume(vol)
        Dim err As Int64 = BASS_ErrorGetCode
        err = err
    End Sub
    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        Dim bs As New BSRDPlay(New BSRDPlay.UpInfo(AddressOf UpdateSongInfo), Me.TextBox1.Text)

        'Me.Url = TextBox1.Text
        Dim th As New Threading.Thread(AddressOf bs.LoadRadioTHR) With {.IsBackground = True}
        th.Start()
    End Sub

    Public Sub UpdateSongInfo(ByVal txt As String, ByVal name As String, ByVal sngname As String, ByVal px As Image)
        If Me.InvokeRequired Then
            Me.Invoke(New BSRDPlay.UpInfo(AddressOf UpdateSongInfo), txt, name, sngname, px)
        Else
            Me.PictureBox1.Image = px
            Me.Text = txt
            Me.lblName.Text = name
            Me.lblSong.Text = sngname
        End If
    End Sub




    Public Class AIRadio



    End Class


    
    Private Sub TrackBar1_Scroll(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TrackBar1.Scroll

    End Sub
End Class
