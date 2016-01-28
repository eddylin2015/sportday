BookPath=WScript.Arguments.Item(0)
Const OLECMDID_PRINT = 6
Const OLECMDEXECOPT_DONTPROMPTUSER = 2
Const PRINT_WAITFORCOMPLETION = 2
objStartFolder = "C:\code\SportDay\SportDay\bin\Debug"
Set objExplorer = CreateObject("InternetExplorer.Application")
Set oShell = CreateObject("Shell.Application")
objExplorer.Visible=true
       handle = objExplorer.Hwnd
        objExplorer.Navigate objStartFolder & "\" & BookPath
        For Each Wnd In oShell.Windows
            If handle = Wnd.Hwnd Then Set objExplorer = Wnd
        Next

        Do While objExplorer.Busy
            WScript.Sleep 1000 'milliseconds
        Loop

        objExplorer.ExecWB OLECMDID_PRINT, OLECMDEXECOPT_DONTPROMPTUSER
		
Set oShell = Nothing
Set objExplorer = Nothing