'    ¡GCall FTPUpload("192.168.1.1", "domain\user", "password", "p.txt", "")
'    ¡GCall FTPDownload("192.168.1.1", "domain\user", "password", "C:", "","*")


 
Function FTPUpload(sSite, sUsername, sPassword, sLocalFile, sRemotePath)
  'This script is provided under the Creative Commons license

  Const OpenAsDefault = -2
  Const FailIfNotExist = 0
  Const ForReading = 1
  Const ForWriting = 2

  Set oFTPScriptFSO = CreateObject("Scripting.FileSystemObject")
  Set oFTPScriptShell = CreateObject("WScript.Shell")

  sRemotePath = Trim(sRemotePath)
  sLocalFile = Trim(sLocalFile)

  If InStr(sRemotePath, " ") > 0 Then
    If Left(sRemotePath, 1) <> """" And Right(sRemotePath, 1) <> """" Then
      sRemotePath = """" & sRemotePath & """"
    End If
  End If

  If InStr(sLocalFile, " ") > 0 Then
    If Left(sLocalFile, 1) <> """" And Right(sLocalFile, 1) <> """" Then
      sLocalFile = """" & sLocalFile & """"
    End If
  End If

  If Len(sRemotePath) = 0 Then
    sRemotePath = ""
  End If

  If InStr(sLocalFile, "*") Then
    If InStr(sLocalFile, " ") Then
      FTPUpload = "Error: Wildcard uploads do not work if the path contains a " & _
      "space." & vbCRLF
      FTPUpload = FTPUpload & "This is a limitation of the Microsoft FTP client."
      Exit Function
    End If
  ElseIf Len(sLocalFile) = 0 or Not oFTPScriptFSO.FileExists(sLocalFile) Then
    FTPUpload = "Error: File Not Found."
    Exit Function
  End If
  '--------END Path Checks---------

  sFTPScript = sFTPScript & "USER " & sUsername & vbCRLF
  sFTPScript = sFTPScript & sPassword & vbCRLF
  sFTPScript = sFTPScript & "cd " & sRemotePath & vbCRLF
  sFTPScript = sFTPScript & "binary" & vbCRLF
  sFTPScript = sFTPScript & "prompt n" & vbCRLF
  sFTPScript = sFTPScript & "put " & sLocalFile & vbCRLF
  sFTPScript = sFTPScript & "quit" & vbCRLF & "quit" & vbCRLF & "quit" & vbCRLF

  sFTPTemp = oFTPScriptShell.ExpandEnvironmentStrings("%TEMP%")
  sFTPTempFile = sFTPTemp & "" & oFTPScriptFSO.GetTempName
  sFTPResults = sFTPTemp & "" & oFTPScriptFSO.GetTempName

  'Write the input file for the ftp commandto a temporary file.
  Set fFTPScript = oFTPScriptFSO.CreateTextFile(sFTPTempFile, True)
  fFTPScript.WriteLine(sFTPScript)
  fFTPScript.Close
  Set fFTPScript = Nothing

  oFTPScriptShell.Run "%comspec% /c FTP -n -s:" & sFTPTempFile & " " & sSite & " > " & sFTPResults, 0, TRUE
  Wscript.Sleep 1000

  'Check results of transfer.
  Set fFTPResults = oFTPScriptFSO.OpenTextFile(sFTPResults, ForReading, _
  FailIfNotExist, OpenAsDefault)
  sResults = fFTPResults.ReadAll
  fFTPResults.Close

  oFTPScriptFSO.DeleteFile(sFTPTempFile)
  oFTPScriptFSO.DeleteFile (sFTPResults)

  If InStr(sResults, "226 Transfer complete.") > 0 Then
    FTPUpload = True
  ElseIf InStr(sResults, "File not found") > 0 Then
    FTPUpload = "Error: File Not Found"
  ElseIf InStr(sResults, "cannot log in.") > 0 Then
    FTPUpload = "Error: Login Failed."
  Else
    FTPUpload = "Error: Unknown."
  End If

  Set oFTPScriptFSO = Nothing
  Set oFTPScriptShell = Nothing
End Function

Function FTPDownload(sSite, sUsername, sPassword, sLocalPath, sRemotePath, sRemoteFile)
  Const OpenAsDefault = -2
  Const FailIfNotExist = 0
  Const ForReading = 1
  Const ForWriting = 2

  Set oFTPScriptFSO = CreateObject("Scripting.FileSystemObject")
  Set oFTPScriptShell = CreateObject("WScript.Shell")

  sRemotePath = Trim(sRemotePath)
  sLocalPath = Trim(sLocalPath)

  '----------Path Checks---------
  If InStr(sRemotePath, " ") > 0 Then
    If Left(sRemotePath, 1) <> """" And Right(sRemotePath, 1) <> """" Then
      sRemotePath = """" & sRemotePath & """"
    End If
  End If

  If Len(sRemotePath) = 0 Then
    sRemotePath = ""
  End If

  'If the local path was blank. Pass the current working direcory.
  If Len(sLocalPath) = 0 Then
    sLocalpath = oFTPScriptShell.CurrentDirectory
  End If

  If Not oFTPScriptFSO.FolderExists(sLocalPath) Then
    'destination not found
    FTPDownload = "Error: Local Folder Not Found."
    Exit Function
  End If

  sOriginalWorkingDirectory = oFTPScriptShell.CurrentDirectory
  oFTPScriptShell.CurrentDirectory = sLocalPath
  '--------END Path Checks---------

  'build input file for ftp command
  sFTPScript = sFTPScript & "USER " & sUsername & vbCRLF
  sFTPScript = sFTPScript & sPassword & vbCRLF
  sFTPScript = sFTPScript & "cd " & sRemotePath & vbCRLF
  sFTPScript = sFTPScript & "binary" & vbCRLF
  sFTPScript = sFTPScript & "prompt n" & vbCRLF
  sFTPScript = sFTPScript & "mget " & sRemoteFile & vbCRLF
  sFTPScript = sFTPScript & "quit" & vbCRLF & "quit" & vbCRLF & "quit" & vbCRLF

  sFTPTemp = oFTPScriptShell.ExpandEnvironmentStrings("%TEMP%")
  sFTPTempFile = sFTPTemp & "" & oFTPScriptFSO.GetTempName
  sFTPResults = sFTPTemp & "" & oFTPScriptFSO.GetTempName

  'Write the input file for the ftp command to a temporary file.
  Set fFTPScript = oFTPScriptFSO.CreateTextFile(sFTPTempFile, True)
  fFTPScript.WriteLine(sFTPScript)
  fFTPScript.Close
  Set fFTPScript = Nothing

  oFTPScriptShell.Run "%comspec% /c FTP -n -s:" & sFTPTempFile & " " & sSite & _
  " > " & sFTPResults, 0, TRUE

  Wscript.Sleep 1000

  'Check results of transfer.
  Set fFTPResults = oFTPScriptFSO.OpenTextFile(sFTPResults, ForReading, _
                    FailIfNotExist, OpenAsDefault)
  sResults = fFTPResults.ReadAll
  fFTPResults.Close

  If InStr(sResults, "226 Transfer complete.") > 0 Then
    FTPDownload = True
  ElseIf InStr(sResults, "File not found") > 0 Then
    FTPDownload = "Error: File Not Found"
  ElseIf InStr(sResults, "cannot log in.") > 0 Then
    FTPDownload = "Error: Login Failed."
  Else
    FTPDownload = "Error: Unknown."
  End If

  Set oFTPScriptFSO = Nothing
  Set oFTPScriptShell = Nothing
End Function
