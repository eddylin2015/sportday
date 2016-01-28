' VBScript source code
'Option Explicit

'Dim i

' Show all of the arguments.
'WScript.Echo WScript.Arguments.Count & " arguments"

'For i = 0 to WScript.Arguments.Count - 1
'    WScript.Echo " " & WScript.Arguments.Item(i)
'Next



Private temp
Private ThisDir
Private BookPath
Private BookPath0
Private book
Private sdir
Private ddir
sdir=WScript.Arguments.Item(0)
ddir=WScript.Arguments.Item(1)

With CreateObject("Scripting.FileSystemObject") 
temp = WScript.ScriptFullName
ThisDir=.GetParentfolderName(temp)
temp=.GetBaseName(temp)

BookPath=WScript.Arguments.Item(0)
BookPath0=WScript.Arguments.Item(1)
End With

With CreateObject("Excel.Application")
.Visible=False
.DisplayAlerts=False
.EnableEvents = False
Set book = .WorkBooks.Open(BookPath)
Book.SaveAs BookPath0,6
.Quit
End With
Set book = Nothing


