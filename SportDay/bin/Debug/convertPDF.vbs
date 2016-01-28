' VBScript source code
'Option Explicit

'Dim i

' Show all of the arguments.
'WScript.Echo WScript.Arguments.Count & " arguments"

'For i = 0 to WScript.Arguments.Count - 1
'    WScript.Echo " " & WScript.Arguments.Item(i)
'Next



Private BookPath
Private BookPath0
Private book
BookPath=WScript.Arguments.Item(0)
BookPath0=WScript.Arguments.Item(1)
PSFileName="c:\temp.ps"
With CreateObject("Excel.Application")
.Visible=False
.DisplayAlerts=False
.EnableEvents = False
Set book = .WorkBooks.Open(BookPath)

With .ActiveSheet.PageSetup
.TopMargin = 10
.BottomMargin = 10
.LeftMargin = 10
.RightMargin = 10
End With


.ActivePrinter = "Adobe PDF on Ne08:"
call  book.ActiveSheet.PrintOut(Null,Null, 1, False,   "Adobe PDF on Ne08:" , True,True, PSFileName)  
.Quit
End With
Set book = Nothing
    Set myPDF = CreateObject("PdfDistiller.PdfDistiller.1")
    myPDF.FileToPDF PSFileName, BookPath0, ""

