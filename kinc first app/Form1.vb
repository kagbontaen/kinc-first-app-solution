Imports System.IO
Imports Microsoft.Win32



Public Class Form1
    Private Const Promptpath As String = "Environment variable 'PATH' does not exist." _
                                         + vbCrLf _
                                         + "this rarely happens and is a problem on your end," _
                                         + vbCrLf _
                                         + "please use the choose adb button to select the adb file" _
                                         + vbCrLf _
                                         + "or make sure adb.exe exist in the same directory as this application"
    Public Adbpath, apkpath, apkpaths() As String
    Public silent, adbfound As Boolean

    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()
        ' Add any initialization after the InitializeComponent() call.

    End Sub



    Public Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Call Getlaunchparam()                                 'collecting launch arguments and parameters and determine if running silent
        Call CheckIfRunning()                                 'checking if ADB is already running
        If silent Then Close()
    End Sub


    Private Sub Btn_APK_Click(sender As Object, e As EventArgs) Handles btn_apk.Click
        Waitfordevice()
        Dim apkpathdialog = New OpenFileDialog
        With apkpathdialog
            .Filter = "Android Application|*.apk|All Files|*.*"
            .Title = "Select Location of Adb Binary"
            .InitialDirectory = "c:\kk\apk"                    ', set this way for testing, change to this when done, .InitialDirectory = CurDir()
            .RestoreDirectory = True
            .Multiselect = True
            .DereferenceLinks = True
            .ShowDialog()
        End With
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
            .Multiselect = False
            .ValidateNames = True
            .CheckFileExists = True
            .DefaultExt = ".exe"
            .ShowDialog()
        End With
        If Not apkpathdialog.FileName = "" Then
            Adbpath = apkpathdialog.FileName
            lbl_adbpath.Text = Adbpath
            btn_apk.Visible = True
        End If
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
            lbl_apkpath.Text = "adb Is already running"
            Dim objWMIService = GetObject("winmgmts:\\.\root\cimv2")
            Dim colProcess As Object = objWMIService.ExecQuery("Select * from win32_Process " & "Where Name = 'adb.exe'")
            For Each objProcess In colProcess
                Adbpath = objProcess.ExecutablePath
                'Dim a As MessageBox
                'a("adb is currently running and i will" _
                '       + vbCrLf _
                '       + "attempt to use the already running one at" _
                '       + vbCrLf _
                '       + Adbpath)
                Call Founadbfound(Adbpath, True)
                Return Adbpath
                Exit Function
            Next
        Else
            Call Testadbpath()                                              'testing for adb in PATH directories
            lbl_apkpath.Text = "adb Is Not running"
        End If
        Return Adbpath
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
                    Call Founadbfound(Adbpath, False)
                    Exit For
                End If
            Next
        Catch ex As System.ArgumentException
            Dim msgBoxResult = MessageBox.Show(Promptpath)
            If File.Exists(CurDir() + "\adb.exe") Then
                Adbpath = CurDir() _
                            + "\adb.exe"
                Call Founadbfound(Adbpath, True)
            Else
                msgBoxResult = MsgBox("I can't find adb.exe, and i have really searched" _
                                      + vbCrLf _
                                      + "please use the select adb button to choose an adb.exe binary", MsgBoxStyle.Critical)
                btn_apk.Enabled = False
                Button2.Visible = True
            End If
        End Try
        Return Adbpath
    End Function
    Public Function Founadbfound(adbpath As String, silent As Boolean)
        Environment.SetEnvironmentVariable(adbpath, adbpath)               ' might end up using this in the end. but it's useless for now
        lbl_adbpath.Text = adbpath
        If Not silent Then
            MsgBox("adb.exe was found in " + adbpath,, "Hurray adb.exe has been found")
        End If
        Button2.Text = "Change ADB"
        btn_apk.Enabled = True
        Return adbpath
    End Function

    Public Function Getlaunchparam()
        On Error GoTo Er
        Dim startargs() As String = Environment.GetCommandLineArgs()
        Select Case startargs.Length
            Case Is <= 1
                silent = False
                'MsgBox(startargs(0))
                'Call Apkinstaller(startargs(0))
                Exit Function
            Case Else
                silent = True
                Call CheckIfRunning()
                Call Waitfordevice()
                For i As Integer = 1 To startargs.Length - 1
                    apkpath = startargs(i)
                    Call Apkinstaller(apkpath)
                Next
                Return apkpath & " has been installed"
                Close()
                Me.Close()
                Me.Dispose()

        End Select
Er:
        Dim msgBoxEr = MsgBox("an error occurred whle getting program launch parameters",
               MsgBoxStyle.Critical,
               "Something is wrong in the force")
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

        Dim ed As String = apkinstall.StandardOutput.ReadToEnd.ToString()

        'Dim output As StringReader = New StringReader(apkinstall.StandardOutput.ReadToEnd())

        'Dim unused = output.ReadLine
        MsgBox(ed)                                              'remove when done or make part of program
        Return ed

        '        Return adbdev

    End Function

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Me.Hide()
        Explorer1.Show()
    End Sub
End Class




