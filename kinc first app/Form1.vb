Imports System.IO
Imports Microsoft.Win32



Public Class Form1
    Private Const Promptpath As String = "Environment variable 'PATH' does not exist.
this is a problem on your end,
please use the choose adb button to select the adb file
or make sure adb.exe exist in the same directory as this application"
    Dim Adbpath As String
    Dim apkpath As String
    Dim apkpaths() As String

    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()
        ' Add any initialization after the InitializeComponent() call.

    End Sub



    Public Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Call CheckIfRunning()
        'testing for adb in PATH directories
        Call Testadbpath()
        Call Getlaunchparam()
    End Sub


    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Waitfordevice()
    End Sub

    Private Sub Label1_Click(sender As Object, e As EventArgs) Handles Label1.Click

    End Sub

    Private Sub Button2_Click(a As Object, e As EventArgs) Handles Button2.Click
        Dim adbpathdialog As OpenFileDialog = New OpenFileDialog With {
            .Filter = "Application|*.exe|All Files|*.*",
            .Title = "Select Location of Adb Binary",
            .InitialDirectory = CurDir()
        }
        adbpathdialog.ShowDialog()
        Adbpath = adbpathdialog.FileName
        filegetr.Text = Adbpath
        Button1.Visible = True
    End Sub

    Public Sub Adbpathdialog_FileOk(sender As Object, e As System.ComponentModel.CancelEventArgs) Handles Adbpathdialog.FileOk
        Dim adbpath As String = Adbpathdialog.FileName
    End Sub

    Private Sub Btn_about_Click(sender As Object, e As EventArgs) Handles btn_about.Click
        Me.Hide()
        AboutBox1.Show()
    End Sub

    Public Function CheckIfRunning()
        If (Process.GetProcessesByName("adb").Count > 0) Then
            Label1.Text = "adb Is already running"
        Else
            Label1.Text = "adb Is Not running"
            Adbpath = "adb.exe"
            Process.Start(Adbpath, "start-server").WaitForExit()

        End If
        Return Process.GetProcessesByName("adb")
    End Function

    Public Function Waitfordevice()
        Dim waitfordevices As New Process
        With waitfordevices
            .StartInfo.UseShellExecute = False
            .StartInfo.RedirectStandardOutput = True
            .StartInfo.FileName = Adbpath
            .StartInfo.Arguments = "wait-for-any-device"
            '.StartInfo.Arguments = "devices"
            .Start()
            .StartInfo.WindowStyle = ProcessWindowStyle.Minimized
            .WaitForExit()
        End With

        Dim output As String = waitfordevices.StandardOutput.ReadToEnd()
        Return output

        '        Return adbdev
    End Function

    Public Function Testadbpath()
        Try
            Dim syspath As String = My.Application.GetEnvironmentVariable("PATH")
            Dim stringSeparators() As String = {";"}
            Dim syspatharr() As String
            Dim count As Integer
            syspatharr = syspath.Split(";")

            For count = 0 To syspatharr.Length - 1
                Dim adbtestpath As String = syspatharr(count)
                'MsgBox("Testing for adb.exe in" + adbtestpath,, "Running Tests")
                If File.Exists(adbtestpath + "\adb.exe") Then
                    Adbpath = adbtestpath + "\adb.exe"
                    Environment.SetEnvironmentVariable(Adbpath, adbtestpath + "\adb.exe")               ' might end up using this in the end. but it's useless for now
                    filegetr.Text = Adbpath
                    MsgBox("adb.exe was found in " + Adbpath, MsgBoxStyle.Information, "Hurray adb.exe has been found")
                    Button2.Text = "Change ADB"
                    Exit For
                End If
            Next
        Catch ex As System.ArgumentException
            Dim msgBoxResult = MessageBox.Show(Promptpath)
            If File.Exists(CurDir() + "\adb.exe") Then
                Adbpath = CurDir() _
                            + "\adb.exe"
                filegetr.Text = Adbpath
                MsgBox("adb.exe was found in " + Adbpath, MsgBoxStyle.Information, "Hurray adb.exe has been found")
                Button2.Text = "Change ADB"
            Else
                msgBoxResult = MsgBox("I can't find adb.exe, and i have really searched
please use the select adb button to choose an adb.exe binary")
                Button1.Visible = False
                Button2.Visible = True
            End If
        End Try
        Return Adbpath
    End Function
    Public Function Getlaunchparam()
        Dim startargs() As String = Environment.GetCommandLineArgs()
        Select Case startargs.Length
            Case Is <= 1
                MsgBox(startargs(0))
                Call Apkinstaller(startargs(0))
                Exit Function
            Case Else

                For i As Integer = 1 To startargs.Length - 1
                    apkpath = startargs(i)
                    Call Apkinstaller(apkpath)
                Next
                Return apkpath
        End Select
    End Function

    Public Function Apkinstaller(apkpath)
        MsgBox("Installing " + apkpath + " to Device")
        Dim apkinstall As New Process
        With apkinstall
            .StartInfo.UseShellExecute = False
            .StartInfo.RedirectStandardOutput = True
            .StartInfo.FileName = Adbpath
            .StartInfo.Arguments = "install " + Text.Getcharac apkpath
            '.StartInfo.Arguments = "devices"
            .Start()
            .StartInfo.WindowStyle = ProcessWindowStyle.Normal
            .WaitForExit()
        End With

        Dim output As String = apkinstall.StandardOutput.ReadToEnd()
        Return output

        '        Return adbdev

    End Function
End Class




