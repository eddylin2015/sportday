
' List Printer Drivers


strComputer = "."
Set objWMIService = GetObject("winmgmts:" _
    & "{impersonationLevel=impersonate}!\\" & strComputer & "\root\cimv2")
Set colInstalledPrinters =  objWMIService.ExecQuery _
    ("Select * from Win32_PrinterDriver")

For each objPrinter in colInstalledPrinters
    Wscript.echo "Configuration File: " & objPrinter.ConfigFile _
     & vbCrLf &  "Data File: " & objPrinter.DataFile _
     & vbCrLf &  "Description: " & objPrinter.Description _
     & vbCrLf &  "Driver Path: " & objPrinter.DriverPath _
     & vbCrLf &  "File Path: " & objPrinter.FilePath _
     & vbCrLf &  "Help File: " & objPrinter.HelpFile _
     & vbCrLf &  "INF Name: " & objPrinter.InfName _
     & vbCrLf &  "Monitor Name: " & objPrinter.MonitorName _
     & vbCrLf &  "Name: " & objPrinter.Name _
     & vbCrLf &  "OEM Url: " & objPrinter.OEMUrl _
     & vbCrLf &  "Supported Platform: " & objPrinter.SupportedPlatform _
     & vbCrLf &  "Version: " & objPrinter.Version
Next