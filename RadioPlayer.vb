Imports System.IO
Imports System.Net


Public Class RadioPlayer
    Public Property Text As String
    Public Property LblName As String
    Public Property LblSong As String
    Public Property PictBx As Image
    Public Property Upd As UpInfo

#Region "Info"

    Public Property Receiver As AudioReceiver

    Public Property Channel As Long
    Public Property GotHeader As Boolean
    Public Property Url As String
    Public Property TmpNameHold As String
    Public Property TmpNameHold2 As String

    ' SAVE LOCAL COPY
    'Public WriteFile As New clsFileIo(Me)

    Public FileIsOpen As Boolean
    Public DownloadStarted As Boolean
    ' DoDownload As Boolean
    Public DlOutput As String
    Dim SongNameUpdate As Boolean

#End Region


    Public Delegate Sub UpInfo(txt As String, lblname As String, lblSong As String, pictbx As Image)

    Public Sub UpdateInfo()
        Upd.Invoke(Text, LblName, LblSong, PictBx)
    End Sub

#Region "Construction"

    Public Sub New(ByRef up As UpInfo,
                   url As String)
        Me.Upd = up
        Me.Url = url
    End Sub

#End Region

    Public Sub Play()
        OpenUrl(Me.Url)
    End Sub

    Public Sub OpenUrl(clkUrl As String)
        With Me
            If Receiver IsNot Nothing Then
                Receiver.Close()
            End If
            .LblName = "connecting..."
            .Text = ""
            .LblSong = ""
            Receiver = New AudioReceiver(clkUrl)
            If Receiver.ChannelHandle = 0 Then
                .LblName = "not playing"
                Throw New Exception("Can't play the stream " & BASS_ErrorGetCode)
            Else
                Receiver.Start()
            End If
        End With
    End Sub

    Public Sub Clear()
    End Sub


    Public Sub SetPict(url As String)
        Try
            Dim ms As MemoryStream = New MemoryStream(New WebClient().DownloadData(url))
            Dim img As Image = Image.FromStream(ms)
            ms.Close()
            Me.PictBx = img
            ' Me.pictBx.SizeMode = PictureBoxSizeMode.StretchImage
        Catch ex As Exception

        End Try
    End Sub
End Class