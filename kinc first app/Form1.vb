Imports System.IO



Public Class Form1

    Public Const adbdev As String = "adb.exe wait-for-device"
    Private adb As Object

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Label1.Text = Shell(adbdev, 1, True,)
    End Sub
    Public Sub Button1_hover(adbpath As Object)
        ' Dim adb As Object = FileGet(Me.adb)
        If Not FileIO.FileSystem.FileExists(adb.exe) Then
            MsgBox("adb not found,
please select loaction for adb", 2, "error")
        End If
        adbpath = adb
        If Me.adb.count > 0 Then
            Label1.Text = Me.adb
        End If

    End Sub

    Private Function FileGet(exe As Object) As Object
        FileOpen(100,
                 adb.exe,
                 OpenMode.Binary,
                 OpenAccess.Read,
                 OpenShare.Shared)
        Throw New NotImplementedException()
    End Function

    Private Function [Get](adb As String) As Object
        Throw New NotImplementedException()
    End Function

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Call CheckIfRunning()
        Call TestGetEnvironmentVariable()
    End Sub
    Public Function CheckIfRunning()
        Dim adbpath
        ' adbpath = FileGet(adb.exe)
        Dim p As Process()
        p = Process.GetProcessesByName("adb.exe")

        If p.Count > 0 Then
            Me.Label1.Text = "adb Is running"
        Else
            Me.Label1.Text = "adb Is Not running"
            ' Dim processd = Process.Start(adbpath, "Devices")
        End If
    End Function
    Private Sub TestGetEnvironmentVariable()
        Try
            Dim msgboxresult = MsgBox("PATH = " & My.Application.GetEnvironmentVariable("PATH"))
        Catch ex As System.ArgumentException
            Dim msgBoxResult = MsgBox("Environment variable 'PATH' does not exist.")
        End Try
    End Sub

    Private Sub Label1_Click(sender As Object, e As EventArgs) Handles Label1.Click

    End Sub
End Class




