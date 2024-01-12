' Written in 2024 by Glenn Larsson.
' Usage: Dim Result As String = fGetAssembliesToSBOM

Imports System.IO
Imports System.Reflection

Module SBOM_DEF

    Public sSBOMProjectName As String = Assembly.GetCallingAssembly.ToString.Split(",")(0) ' Namespace (Projectname)
    Public sSBOMEmbededResourceFile As String = "project.assets.json" ' Embedded resource (your job to include)

    Dim sAssets As String = ""
    Public oRX As System.Text.RegularExpressions.Regex

End Module

Module SBOM

    Public Function fGetAssemblies() As String

        Dim asm() As Reflection.Assembly = AppDomain.CurrentDomain.GetAssemblies

        Dim sAssembly As String = ""
        Dim sResult As String = ""

        For Each sItem In asm
            sAssembly = sItem.FullName.ToString
            sResult = sResult & "Package: " & sAssembly.Split(",")(0) & ", Version:"
            sResult = sResult & sAssembly.Split(",")(1).Replace("Version=", "") & vbCrLf
        Next

        Return sResult

    End Function

    Public Function fGetAssembliesToSBOM() As String

        Dim sEmbeddedProjectAssetsFile As String = fGetEmbeddedResource(sSBOMProjectName & "." & sSBOMEmbededResourceFile)

        Dim sLines() As String = sEmbeddedProjectAssetsFile.Split(vbCrLf)

        Dim sResult As String = fGetAssemblies() ' Init with assemblies in appdomain
        Dim sAssemblyItem() As String = {}
        Dim sAssemblyString As String = ""

        For n = 0 To UBound(sLines)
            sLines(n) = sLines(n).Replace(vbCr, "")
            sLines(n) = sLines(n).Replace(vbLf, "")
            sLines(n) = sLines(n).Trim()

            If oRX.IsMatch(sLines(n), "'[A-Za-z\./]{1,}\/[0-9\.]{3,}': {".Replace("'", Chr(34))) = True Then
                sLines(n) = sLines(n).Replace("{", "")
                sLines(n) = sLines(n).Replace(Chr(34) & ":", "")
                sLines(n) = sLines(n).Replace(Chr(34), "")
                sAssemblyItem = sLines(n).Split("/")
                sAssemblyString = "Package: " & sAssemblyItem(0) & ", Version: " & sAssemblyItem(1)

                If InStr(1, sResult, sAssemblyString) = 0 Then
                    sAssemblyItem = sLines(n).Split("/")
                    If UBound(sAssemblyItem) = 1 Then
                        sResult = sResult & sAssemblyString & vbCrLf
                    End If
                End If
            End If


        Next

        Dim x() As String = sResult.Split(vbCrLf)
        sResult = ""
        Array.Sort(x)
        For n = 0 To UBound(x)
            x(n) = x(n).Replace(vbCrLf, "")
            If x(n).Trim <> "" Then
                sResult = sResult & x(n) & vbCrLf
            End If
        Next

        Return sResult

    End Function

    Public Function fGetEmbeddedResource(Optional sResourceFile As String = "") As String

        ' sResourceFile -> Needs the full name, i.e. Project name + embedded filename: "AutoSBom.project.assets.json"
        If sResourceFile = "" Then sResourceFile = "AutoSBom.project.assets.json"

        Dim oStream As Stream = Assembly.GetExecutingAssembly.GetManifestResourceStream(sResourceFile)
        Dim strReader As StreamReader = New StreamReader(oStream)
        Return strReader.ReadToEnd.ToString

    End Function

End Module
