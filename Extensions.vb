
Imports System.Runtime.CompilerServices
Imports System.Text.RegularExpressions
Imports System.IO

Namespace Extensions
    Public Module StreamExtensions
        <Extension> _
        Public Function OpenFileWr(ByRef fs As Stream, outputPath As String) As Boolean
            If String.IsNullOrEmpty(outputPath) Then Return False
            fs = New FileStream(outputPath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read)
            Return True
        End Function

    End Module
    Public Module StringExtensions
        <Extension>
        Public Function RemoveIllegalPathCharacters(path As String) As String
            Dim regexSearch As String = New String(System.IO.Path.GetInvalidFileNameChars()) + New String(System.IO.Path.GetInvalidPathChars())
            Dim r As New Regex(String.Format("[{0}]", Regex.Escape(regexSearch)))
            Return r.Replace(path, "")
        End Function
    End Module
End Namespace
