Public Class clsFileIo
    ' This ClassModule is written by Peter Hebels

    Private Const GENERIC_WRITE = &H40000000
    Private Const FILE_ATTRIBUTE_NORMAL = &H80
    Private Const CREATE_ALWAYS = 2
    Private Const OPEN_ALWAYS = 4
    Private Const INVALID_HANDLE_VALUE = -1

    Private Declare Function MessageBox Lib "user32" Alias "MessageBoxA" (ByVal hwnd As Long, ByVal lpText As String, ByVal lpCaption As String, ByVal wType As Long) As Long
    Private Declare Function ReadFile Lib "kernel32" (ByVal hFile As Long, ByVal lpBuffer As Object, ByVal nNumberOfBytesToRead As Long, ByVal lpNumberOfBytesRead As Long, ByVal lpOverlapped As Long) As Long
    Private Declare Function CloseHandle Lib "kernel32" (ByVal hObject As Long) As Long
    Private Declare Function WriteFile Lib "kernel32" (ByVal hFile As Long, ByVal lpBuffer As Object, ByVal nNumberOfBytesToWrite As Long, ByVal lpNumberOfBytesWritten As Long, ByVal lpOverlapped As Long) As Long
    Private Declare Function CreateFile Lib "kernel32" Alias "CreateFileA" (ByVal lpFileName As String, ByVal dwDesiredAccess As Long, ByVal dwShareMode As Long, ByVal lpSecurityAttributes As Long, ByVal dwCreationDisposition As Long, ByVal dwFlagsAndAttributes As Long, ByVal hTemplateFile As Long) As Long
    Private Declare Function FlushFileBuffers Lib "kernel32" (ByVal hFile As Long) As Long
    Private Declare Sub CopyMemory Lib "kernel32" Alias "RtlMoveMemory" (ByVal Destination As Object, ByVal Source As Object, ByVal length As Long)

    Private fHandle As Long
    Private fSuccess As Long
    Private lFilePos As Long
    Private File_Name As String
    Public CLNTRadio As BSRDPlay


    Public Sub New(ByRef caller As BSRDPlay)
        Me.CLNTRadio = caller
    End Sub

    Public Function OpenFile(ByVal FileN As String) As Boolean
        fHandle = CreateFile(FileN, GENERIC_WRITE, 0, 0, CREATE_ALWAYS, FILE_ATTRIBUTE_NORMAL, 0)
        If fHandle <> INVALID_HANDLE_VALUE Then
            File_Name = FileN
            OpenFile = True
        Else
            OpenFile = False
        End If
    End Function

    Public Function CloseFile() As Boolean
        If fHandle <> INVALID_HANDLE_VALUE And File_Name <> "" Then
            fSuccess = CloseHandle(fHandle)
            CloseFile = True
        Else
            CloseFile = False
        End If
    End Function

    Public Function WriteBytes(ByVal Pointer As Long, ByVal Size As Long) As Boolean
        Dim hHdr As String
        Dim NewHeader As String
        Dim i As Long

        Dim d() As Byte
        ReDim d(Size)

        If fHandle <> INVALID_HANDLE_VALUE And File_Name <> "" Then
            hHdr = Chr(255) & Chr(251)
            If CLNTRadio.GotHeader = False Then
                Call CopyMemory(d(0), Pointer, Size)
                For i = 1 To Size
                    If d(i) = 255 Then
                        If d(i + 1) = 251 Then
                            NewHeader = hHdr & Chr(d(i + 2))
                            fSuccess = WriteFile(fHandle, NewHeader, Len(NewHeader), lFilePos, 0)
                            If fSuccess <> 0 Then
                                fSuccess = FlushFileBuffers(fHandle)
                                CLNTRadio.GotHeader = True
                                Exit For
                            End If
                        End If
                    End If
                Next i
            End If

            If CLNTRadio.GotHeader = True Then
                fSuccess = WriteFile(fHandle, Pointer, Size, lFilePos, 0)
                If fSuccess <> 0 Then
                    fSuccess = FlushFileBuffers(fHandle)
                    WriteBytes = True
                Else
                    WriteBytes = False
                    Exit Function
                End If
            End If
        Else
            WriteBytes = False
            Exit Function
        End If

    End Function




End Class
