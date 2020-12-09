Imports System.IO
Imports Microsoft.Win32



Public Class Form1
    Private Const Promptpath As String = "Environment variable 'PATH' does not exist.
this is a problem on your end,
please use the choose adb button to select the adb file
or make sure adb.exe exist in the same directory as this application"
    Dim Adbpath, apkpath As String
#Disable Warning IDE0044 ' Add readonly modifier
    Dim apkpaths() As String
    Dim silent, adbfound As Boolean
#Enable Warning IDE0044 ' Add readonly modifier

    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()
        ' Add any initialization after the InitializeComponent() call.

    End Sub



    Public Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Call Getlaunchparam()                                        'collecting launch arguments and parameters and determine if running silent
        Call CheckIfRunning(Adbpath)                                 'checking if ADB is already running
    End Sub


    Private Sub Btn_APK_Click(sender As Object, e As EventArgs) Handles btn_apk.Click
        Waitfordevice()
        Dim apkpathdialog As OpenFileDialog = New OpenFileDialog With {
            .Filter = "Android Application|*.apk|All Files|*.*",
            .Title = "Select Location of Adb Binary",
            .InitialDirectory = "c:\kk\apk",                    '.InitialDirectory = CurDir(),
            .RestoreDirectory = True,
            .Multiselect = True
        }

        apkpathdialog.ShowDialog()
        For i As Integer = 0 To apkpathdialog.FileNames.Length - 1
            lbl_apkpath.Text = apkpathdialog.FileNames(i)
            Call Apkinstaller(apkpathdialog.FileNames(i))
        Next
        'lbl_apkpath.Text = apkpath
        'Call Apkinstaller(apkpath)
    End Sub


    Private Sub Button2_Click(a As Object, e As EventArgs) Handles Button2.Click
        Dim apkpathdialog = New OpenFileDialog
        With apkpathdialog
            .Filter = "Application|*.exe|All Files|*.*"
            .Title = "Select Location of Adb Binary"
            .InitialDirectory = CurDir()
            .RestoreDirectory = True
        End With
        apkpathdialog.ShowDialog()
        Adbpath = apkpathdialog.FileName
        lbl_adbpath.Text = Adbpath
        btn_apk.Visible = True
    End Sub

    Public Sub Adbpathdialog_FileOk(sender As Object, e As System.ComponentModel.CancelEventArgs) Handles Adbpathdialog.FileOk
        Dim adbpath As String = Adbpathdialog.FileName
    End Sub

    Private Sub Btn_about_Click(sender As Object, e As EventArgs) Handles btn_about.Click
        Me.Hide()
        AboutBox1.Show()
    End Sub

    Public Function CheckIfRunning(adbpath As String)
        If (Process.GetProcessesByName("adb").Count > 0) Then
            lbl_apkpath.Text = "adb Is already running"
            Dim objWMIService = GetObject("winmgmts:\\.\root\cimv2")
            Dim colProcess As Object = objWMIService.ExecQuery("Select * from win32_Process " & "Where Name = 'adb.exe'")
            For Each objProcess In colProcess
                adbpath = objProcess.ExecutablePath
                MsgBox("adb is currently running" + vbCrLf + "will attempt to use the already running one" + vbCrLf + adbpath)
            Next
        Else
            Call Testadbpath()                                              'testing for adb in PATH directories
            lbl_apkpath.Text = "adb Is Not running"
        End If
        Return adbpath
    End Function

    Public Function Waitfordevice()
        Dim waitfordevices As New Process
        With waitfordevices
            .StartInfo.UseShellExecute = False
            .StartInfo.RedirectStandardOutput = True
            .StartInfo.FileName = Adbpath
            .StartInfo.Arguments = "wait-for-any-device"
            '.StartInfo.Arguments = "devices"
            .StartInfo.WindowStyle = 1
            .Start()
            .WaitForExit()
        End With
        If waitfordevices.HasExited Then
            Dim strreadr As StringReader = New StringReader(waitfordevices.StandardOutput.ReadToEnd())
            Dim output = strreadr.ReadToEnd
            Return output
            Exit Function
        End If

        Return waitfordevices.Id
    End Function

    Public Function Testadbpath()
        Try
            Dim syspath As String = My.Application.GetEnvironmentVariable("PATH")
            Dim syspatharr() As String
            syspatharr = syspath.Split(";")

            For i As Integer = 0 To syspatharr.Length - 1
                Dim adbtestpath As String = syspatharr(i)
                If File.Exists(adbtestpath + "\adb.exe") Then
                    Adbpath = adbtestpath + "\adb.exe"
                    Environment.SetEnvironmentVariable(Adbpath, adbtestpath + "\adb.exe")               ' might end up using this in the end. but it's useless for now
                    lbl_adbpath.Text = Adbpath
                    'MsgBox("adb.exe was found in " + Adbpath,, "Hurray adb.exe has been found")
                    Button2.Text = "Change ADB"
                    Exit For
                End If
            Next
        Catch ex As System.ArgumentException
            Dim msgBoxResult = MessageBox.Show(Promptpath)
            If File.Exists(CurDir() + "\adb.exe") Then
                Adbpath = CurDir() _
                            + "\adb.exe"
                lbl_adbpath.Text = Adbpath
                MsgBox("adb.exe was found in " + Adbpath,, "Hurray adb.exe has been found")
                Button2.Text = "Change ADB"
            Else
                msgBoxResult = MsgBox("I can't find adb.exe, and i have really searched
please use the select adb button to choose an adb.exe binary")
                btn_apk.Visible = False
                Button2.Visible = True
            End If
        End Try
        Return Adbpath
    End Function
    Public Function Getlaunchparam()
        Dim startargs() As String = Environment.GetCommandLineArgs()
        Select Case startargs.Length
            Case Is <= 1
                silent = False
                'MsgBox(startargs(0))
                'Call Apkinstaller(startargs(0))
                Exit Function
            Case Else
                silent = True
                Call CheckIfRunning(Adbpath)
                For i As Integer = 1 To startargs.Length - 1
                    apkpath = startargs(i)
                    Call Apkinstaller(apkpath)
                Next
                Return apkpath
                Form.ActiveForm.Close()
                My.Application.MinimumSplashScreenDisplayTime = &H1B58
                On Error Resume Next
        End Select
        MsgBox("an error occurred whle getting program launch parameters", MsgBoxStyle.Critical, "Something is wrong in the force")
        Return "9999"
    End Function

    Public Function Apkinstaller(apkpath As String)
        MsgBox("Installing " + apkpath + " to Device")
        Dim apkinstall As New Process
        With apkinstall
            .StartInfo.UseShellExecute = False
            .StartInfo.RedirectStandardOutput = True
            .StartInfo.FileName = Adbpath
            .StartInfo.Arguments = "install " + """" + apkpath + """"
            '.StartInfo.Arguments = "devices"
            .StartInfo.WindowStyle = 2
            .Start()
            '.WaitForExit()
        End With


        Dim output As StringReader = New StringReader(apkinstall.StandardOutput.ReadToEnd())

        Dim unused = output.ReadLine
        MsgBox(unused)                                              'remove when done or make part of program
        Return output.ReadToEnd

        '        Return adbdev

    End Function

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Explorer1.Show()
    End Sub
End Class




