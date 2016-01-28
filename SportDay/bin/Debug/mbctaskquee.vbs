' VBScript source code
'set objArgs = wscript.Arguments
'for each objArg in objArgs
'    wscript.echo objarg
'next

Option Explicit
Private temp
Private ThisDir
Private BookPath
Private BookPath0
Private book
Private sdir
Private ddir 
sdir = "D:\\mbcspd2009\\process"
ddir = "D:\mbcspd2009\report_to_public\report_result\比賽結果"

With CreateObject("Scripting.FileSystemObject") 
temp = WScript.ScriptFullName
ThisDir=.GetParentfolderName(temp)
temp=.GetBaseName(temp)
BookPath=.BuildPath(sdir,temp&".xls")
BookPath0=.BuildPath(ddir,temp&".htm")
End With

With CreateObject("Excel.Application")
.Visible=False
.DisplayAlerts=False
.EnableEvents = False
Set book = .WorkBooks.Open(BookPath)
Book.SaveAs BookPath0,44
.Quit
End With
Set book = Nothing


