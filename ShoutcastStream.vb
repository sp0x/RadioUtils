Imports System
Imports System.Collections.Generic
Imports System.Text
Imports System.IO
Imports System.Net
Imports System.Text.RegularExpressions
Imports System.Linq
Imports System.Net.Sockets


Public Enum StreamType
    Shoutcast
    Mpeg
End Enum

''' <summary>
''' Provides the functionality to receive a shoutcast media stream
''' </summary>
Public Class ShoutcastStream
    Inherits Stream
    Implements IAudioStream

#Region "Variables"
    Private blockSize As Integer
    Private receivedBytes As Integer
    Private netStream As Stream
    Private _bConnected As Boolean = False
    Private _strStreamTitle As String
    Private _strCoverUrl As String
    Public Property Type As StreamType = StreamType.Mpeg
    Public Property ServerList As List(Of String)
    Public Property Bitrate() As Integer Implements IAudioStream.Bitrate
#End Region


    ''' <summary>
    ''' Gets the title of the stream
    ''' </summary>
    Public ReadOnly Property StreamTitle As String
        Get
            Return _strStreamTitle
        End Get
    End Property

#Region "Events"
    ''' <summary>
    ''' Is fired, when a new StreamTitle is received
    ''' </summary>
    Public Event StreamTitleChanged As EventHandler
    ''' <summary>
    ''' Fires the StreamTitleChanged event
    ''' </summary>
    Protected Overridable Sub RaiseStreamTitleChanged()
        RaiseEvent StreamTitleChanged(Me, EventArgs.Empty)
    End Sub
#End Region

#Region "Construction"
    Public Shared Function OpenMediaLink(url As String, Optional ByRef response As WebResponse = Nothing) As Stream
        Dim request As HttpWebRequest = HttpWebRequest.Create(url)
        request.Headers.Add("Icy-MetaData", "1")
        request.KeepAlive = False
        request.UserAgent = "VLC media player"
        request.Proxy = Nothing
        response = request.GetResponse()
        Return response.GetResponseStream()
    End Function
    ''' <summary>
    ''' Creates a new ShoutcastStream and connects to the specified Url
    ''' </summary>
    ''' <param name="url">Url of the Shoutcast stream</param>
    Public Sub New(url As String)
        Dim resp As HttpWebResponse
        receivedBytes = 0
        netStream = OpenMediaLink(url, resp)

        If resp.ContentType = "audio/x-mpegurl" AndAlso Not resp.Headers.AllKeys.Contains("icy-metaint") Then
            Type = StreamType.Mpeg
            ServerList = New List(Of String)
            Dim str As StreamReader = New StreamReader(netStream)
            Dim cserver As String = str.ReadLine()
            Do While cserver IsNot Nothing
                ServerList.Add(cserver) : cserver = str.ReadLine()
            Loop
            SourceAddress = ServerList.LastOrDefault()
            If SourceAddress IsNot Nothing Then netStream = OpenMediaLink(SourceAddress, resp)
            If resp.Headers.AllKeys.Contains("icy-metaint") Then
                blockSize = Integer.Parse(resp.Headers("icy-metaint"))
                Integer.TryParse(resp.Headers("icy-br"), Bitrate)
                Type = StreamType.Shoutcast

            End If

        Else
            blockSize = Integer.Parse(resp.Headers("icy-metaint"))
            Type = StreamType.Shoutcast
        End If
        _bConnected = True


    End Sub
#End Region

#Region "Props"
    Public Property SourceAddress As String


    Public Property CoverUrl As String
        Get
            Return _strCoverUrl
        End Get
        Private Set(value As String)
            _strCoverUrl = value
        End Set
    End Property

#Region "Can"

    ''' <summary>
    ''' Gets a value that indicates whether the ShoutcastStream supports reading.
    ''' </summary>
    Public Overrides ReadOnly Property CanRead() As Boolean
        Get
            Return Connected
        End Get
    End Property
    Public ReadOnly Property Connected As Boolean
        Get
            Return _bConnected
        End Get
    End Property
    ''' <summary>
    ''' Gets a value that indicates whether the ShoutcastStream supports seeking.
    ''' This property will always be false.
    ''' </summary>
    Public Overrides ReadOnly Property CanSeek() As Boolean
        Get
            Return False
        End Get
    End Property

    ''' <summary>
    ''' Gets a value that indicates whether the ShoutcastStream supports writing.
    ''' This property will always be false.
    ''' </summary>
    Public Overrides ReadOnly Property CanWrite() As Boolean
        Get
            Return False
        End Get
    End Property
#End Region

#Region "Non Supported"
    ''' <summary>
    ''' Gets the length of the data available on the Stream.
    ''' This property is not currently supported and always thows a <see cref="NotSupportedException"/>.
    ''' </summary>
    Public Overrides ReadOnly Property Length() As Long
        Get
            Throw New NotSupportedException()
        End Get
    End Property

    ''' <summary>
    ''' Gets or sets the current position in the stream.
    ''' This property is not currently supported and always thows a <see cref="NotSupportedException"/>.
    ''' </summary>
    Public Overrides Property Position() As Long
        Get
            Throw New NotSupportedException()
        End Get
        Set(value As Long)
            Throw New NotSupportedException()
        End Set
    End Property
    ''' <summary>
    ''' Sets the current position of the stream to the given value.
    ''' This Method is not currently supported and always throws a <see cref="NotSupportedException"/>.
    ''' </summary>
    ''' <param name="offset"></param>
    ''' <param name="origin"></param>
    ''' <returns></returns>
    Public Overrides Function Seek(offset As Long, origin As SeekOrigin) As Long
        Throw New NotSupportedException()
    End Function

    ''' <summary>
    ''' Sets the length of the stream.
    ''' This Method always throws a <see cref="NotSupportedException"/>.
    ''' </summary>
    ''' <param name="value"></param>
    Public Overrides Sub SetLength(value As Long)
        Throw New NotSupportedException()
    End Sub

    ''' <summary>
    ''' Writes data to the ShoutcastStream.
    ''' This method is not currently supported and always throws a <see cref="NotSupportedException"/>.
    ''' </summary>
    ''' <param name="buffer"></param>
    ''' <param name="offset"></param>
    ''' <param name="count"></param>
    Public Overrides Sub Write(buffer As Byte(), offset As Integer, count As Integer)
        Throw New NotSupportedException()
    End Sub
#End Region
#End Region

#Region "Reading and meta parsing"
    Private ReadOnly rxStreamMeta As New Regex("(StreamTitle=')(.*)(';StreamUrl=')(.*)'", RegexOptions.Compiled Or RegexOptions.IgnoreCase)
    ''' <summary>
    ''' Parses the received Meta Info
    ''' </summary>
    ''' <param name="metaInfo"></param>
    Private Sub ParseMetaInfo(metaInfo As Byte())
        Dim metaString As String = Encoding.ASCII.GetString(metaInfo)
        Dim tmpMatch As Match = rxStreamMeta.Match(metaString)
        Dim newStreamTitle As String = tmpMatch.Groups(2).Value.Trim()
        CoverUrl = tmpMatch.Groups(4).Value.Trim()
        If Not newStreamTitle.Equals(StreamTitle) Then
            _strStreamTitle = newStreamTitle
            RaiseStreamTitleChanged()
        End If
    End Sub

    Public Overrides Function Read(buffer() As Byte, offset As Integer, count As Integer) As Int32
        Select Case Type
            Case StreamType.Shoutcast
                Return ReadShoutcast(buffer, offset, count)
            Case StreamType.Mpeg
                Return netStream.Read(buffer, offset, count)
        End Select
    End Function
    ''' <summary>
    ''' Reads data from the ShoutcastStream.
    ''' </summary>
    ''' <param name="buffer">An array of bytes to store the received data from the ShoutcastStream.</param>
    ''' <param name="offset">The location in the buffer to begin storing the data to.</param>
    ''' <param name="count">The number of bytes to read from the ShoutcastStream.</param>
    ''' <returns>The number of bytes read from the ShoutcastStream.</returns>
    Public Function ReadShoutcast(ByRef buffer As Byte(), offset As Integer, count As Integer) As Integer
        Dim metaSize As Int32 = 0
        Try
            Dim metaRed As Int32 = readMetaHeader()
            If metaRed > 0 Then
                metaRed = metaRed
            End If
            Dim bytesLeft As Integer = 0
            If ((blockSize - receivedBytes) > count) Then
                bytesLeft = count
            Else
                bytesLeft = blockSize - receivedBytes
            End If
            Dim result As Integer = netStream.Read(buffer, offset, bytesLeft)
            receivedBytes += result
            Return result
        Catch e As Exception
            _bConnected = False
            Trace.WriteLine(e.Message)
            Return -1
        End Try
    End Function

    Private Function readMetaHeader() As Int32
        If receivedBytes = blockSize Then
            Dim metaSz As Integer = netStream.ReadByte()
            If metaSz > 0 Then
                Dim metaInfo As Byte() = New Byte(metaSz * 16) {}
                Dim mLen As Integer = 0
                mLen += netStream.Read(metaInfo, mLen, metaInfo.Length - mLen)
                While (mLen) < metaInfo.Length
                    mLen += netStream.Read(metaInfo, mLen, metaInfo.Length - mLen)
                    If metaInfo.Length - mLen = 0 Then Exit While
                End While
                If mLen = metaInfo.Length Then ' All meta has been red 
                    ParseMetaInfo(metaInfo)
                End If
            End If
            receivedBytes = 0
            Return metaSz * 16
        End If
        Return 0
    End Function
#End Region

#Region "Disposing"
    ''' <summary>
    ''' Flushes data from the stream.
    ''' This method is currently not supported
    ''' </summary>
    Public Overrides Sub Flush()
        Return
    End Sub

    ''' <summary>
    ''' Closes the ShoutcastStream.
    ''' </summary>
    Public Overrides Sub Close()
        _bConnected = False
        netStream.Close()
    End Sub
#End Region


End Class