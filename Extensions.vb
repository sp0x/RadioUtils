
Imports System.Runtime.CompilerServices
Imports System.IO
Imports System.Text.RegularExpressions

Namespace Extensions
    Public Module StreamExtensions
        <Extension>
        Public Function OpenFileWr(ByRef fs As Stream, outputPath As String) As Boolean
            If String.IsNullOrEmpty(outputPath) Then Return False
            fs = New FileStream(outputPath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read)
            Return True
        End Function
    End Module

    Public Module StringExtensions
        <Extension>
        Public Function RemoveIllegalPathCharacters(path As String) As String
            Dim regexSearch As String = New String(IO.Path.GetInvalidFileNameChars()) +
                                        New String(IO.Path.GetInvalidPathChars())
            Dim r As New Regex(String.Format("[{0}]", Regex.Escape(regexSearch)))
            Return r.Replace(path, "")
        End Function
    End Module
End Namespace
