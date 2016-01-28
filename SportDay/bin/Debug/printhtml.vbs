surl="day1.htm"    'your input

Const OLECMDID_PRINT = 6
Const OLECMDEXECOPT_DONTPROMPTUSER = 2
Const PRINT_WAITFORCOMPLETION = 2

objStartFolder = "C:\code\SportDay\SportDay\bin\Debug"
Set objFSO = CreateObject("Scripting.FileSystemObject")
Set objFolder = objFSO.GetFolder(objStartFolder)
Set objExplorer = CreateObject("InternetExplorer.Application")

Set oShell = CreateObject("Shell.Application")

For Each objFile In objFolder.Files
    strFileName = objFile.Name
    If objFSO.GetExtensionName(strFileName) = "htm" Then
        handle = objExplorer.Hwnd
        objExplorer.Navigate objFolder.Path + "\" + objFile.Name

        For Each Wnd In oShell.Windows
            If handle = Wnd.Hwnd Then Set objExplorer = Wnd
        Next

        Do While objExplorer.Busy
            WScript.Sleep 1000 'milliseconds
        Loop

        objExplorer.ExecWB OLECMDID_PRINT, OLECMDEXECOPT_DONTPROMPTUSER
    End If
Next
Set oShell = Nothing
Set objFSO = Nothing
Set objFolder = Nothing
Set objExplorer = Nothing