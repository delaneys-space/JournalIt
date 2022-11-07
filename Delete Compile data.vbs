
Dim fso
Set fso = CreateObject("Scripting.FileSystemObject")



dim path
path = fso.GetParentFolderName(WScript.ScriptFullName)



dim deleteFolder
deleteFolder = path + "\.vs"
If fso.FolderExists(deleteFolder) Then
	fso.DeleteFolder deleteFolder 
End If


deleteFolder = path + "\JournalIt\bin"
If fso.FolderExists(deleteFolder) Then
	fso.DeleteFolder deleteFolder 
End If

deleteFolder = path + "\JournalIt\obj"
If fso.FolderExists(deleteFolder) Then
	fso.DeleteFolder deleteFolder 
End If


set fso = Nothing

msgbox("Deleted")