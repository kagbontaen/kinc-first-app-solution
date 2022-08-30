Imports System.IO
Imports Microsoft.Win32
Imports Microsoft.VisualBasic.FileIO
Imports System.Threading

Module FormUtils
    Public sAutoClosed As Boolean

    Public Sub CloseMsgBoxDelay(ByVal data As Object)
        System.Threading.Thread.Sleep(CInt(data))
        SendKeys.SendWait("~")
        sAutoClosed = True
    End Sub

    Public Function MsgBoxDelayClose(prompt As Object, ByVal delay As Integer, Optional delayedResult As MsgBoxResult = MsgBoxResult.Ok, Optional buttons As MsgBoxStyle = MsgBoxStyle.ApplicationModal, Optional title As Object = Nothing) As MsgBoxResult
        Dim t As Thread

        If delay > 0 Then
            sAutoClosed = False
            t = New Thread(AddressOf CloseMsgBoxDelay)
            t.Start(delay)

            MsgBoxDelayClose = MsgBox(prompt, buttons, title)
            If sAutoClosed Then
                MsgBoxDelayClose = delayedResult
            Else
                t.Abort()
            End If
        Else
            MsgBoxDelayClose = MsgBox(prompt, buttons, title)
        End If

    End Function
End Module

Public Class Form1
    Private Const Promptpath As String = "Environment variable 'PATH' does not exist." _
                                         + vbCrLf _
                                         + "this rarely happens and is a problem on your end," _
                                         + vbCrLf _
                                         + "please use the choose adb button to select the adb file" _
                                         + vbCrLf _
                                         + "or make sure adb.exe exist in the same directory as this application"
    Private Const grptxt1 = "Currently Using adb from: "
    Public Adbpath, apkpath, apkpaths() As String
    Public silent = False, adbfound = False
    Public adb_version As Integer

    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()
        ' Add any initialization after the InitializeComponent() call.

    End Sub

    Public Function msbox(data As String, Optional timeout As Integer = 5, Optional title As String = "Message box")
        CreateObject("WScript.Shell").Popup(data, timeout, title)
        Return "done"
    End Function

    Public Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Call Getlaunchparam()                                 'collecting launch arguments and parameters and determine if running silent
        If silent Then
            Dispose(True)
            Application.ExitThread()
            Exit Sub
        End If
        Call CheckIfRunning()                                 'checking if ADB is already running
    End Sub






    Private Sub Btn_about_Click(sender As Object, e As EventArgs) Handles btn_about.Click
        Me.Hide()
        AboutBox1.Show()
    End Sub

    Public Function CheckIfRunning()
        If Process.GetProcessesByName("adb").Count > 0 Then
            GroupBox1.Text = "adb Is already running from"
            For Each objProcess In GetObject("winmgmts:\\.\root\cimv2").ExecQuery("Select * from win32_Process " & "Where Name = 'adb.exe'")
                Adbpath = objProcess.ExecutablePath
                Call Func_adbfound(Adbpath)
                GroupBox1.Text = grptxt1 + "(Running)"
                Return Adbpath
                Exit Function
            Next
        Else
            Call Testadbpath()                                              'testing for adb in current and PATH directories
            lbl_apkpath.Text = "adb Is Not running"
        End If
        Return Adbpath
    End Function
    Public Function Getlaunchparam()
        If Environment.GetEnvironmentVariable("darkmode") = "dark" Then
            BackColor = Color.Black
            ForeColor = Color.White
            darkcheck.CheckState = CheckState.Checked
        End If
        On Error GoTo Er
        Select Case Environment.GetCommandLineArgs().Length
            Case Is <= 1
                silent = False
                'MsgBox(startargs(0))
                'Call Apkinstaller(startargs(0))
                Exit Function
            Case Else
                silent = True
                Call CheckIfRunning()
                Call Waitfordevice()
                For i As Integer = 1 To Environment.GetCommandLineArgs().Length - 1
                    apkpath = Environment.GetCommandLineArgs()(i)
                    Call Apkinstaller(apkpath)
                Next
                Return apkpath & " has been installed"
                Close()
                'Me.Dispose()

        End Select
Er:
        Dim msgBoxEr = MsgBox("an error occurred whle getting program launch parameters",
               MsgBoxStyle.Critical,
               "Something is wrong in the force")
        Return "9999"
    End Function
    Public Function Testadbpath()
        Try
            If File.Exists($"{FileSystem.CurrentDirectory}\adb.exe") Then
                Adbpath = CurDir() _
                            + "\adb.exe"
                GroupBox1.Text = grptxt1 + "(Current Directory)"
                Call Func_adbfound(Adbpath)
            Else
                Dim syspath As String = My.Application.GetEnvironmentVariable("path")
                For i As Integer = 0 To syspath.Split(";").Length - 1
                    Dim adbtestpath As String = syspath.Split(";")(i)
                    If File.Exists(path:=$"{adbtestpath}\adb.exe") Then
                        Adbpath = $"{adbtestpath}\adb.exe"
                        Call Func_adbfound(Adbpath)
                        GroupBox1.Text = grptxt1 + "(Enviroment Path)"
                        Exit For
                    End If
                Next
                If Not adbfound Then
                    Dim MsgBoxResult = MsgBox("I can't find adb.exe, and i have really searched" _
                                              + vbCrLf _
                                              + "please use the select adb button to choose an adb.exe binary" _
                                              + vbCrLf _
                                              + "to fix this error permanently," _
                                              + vbCrLf _
                                              + "copy the android debug bridge binaries" _
                                              + vbCrLf _
                                              + "(adb.exe, adbwinapi.dll, adbwinusbapi.dll, e.t.c.)" _
                                              + vbCrLf _
                                              + "into the program directory" _
                                              + vbCrLf _
                                              + FileSystem.CurrentDirectory, MsgBoxStyle.Critical)
                    btn_apk.Enabled = False
                    ADB_btn.Visible = True
                End If
            End If
        Catch ex As System.ArgumentException
            Dim msgBoxResult = MessageBox.Show(Promptpath)
        End Try
        Return Adbpath
    End Function
    Public Function Func_adbfound(adbpath As String)
        adbfound = True
        Environment.SetEnvironmentVariable("adbpath", adbpath)               ' might end up using this in the end. but it's useless for now
        lbl_adbpath.Text = adbpath
        If Not silent Then
            msbox("adb.exe was found in " + adbpath + vbCrLf + "I will therefore use it", 4, "Hurray adb.exe has been found")
        End If
        ADB_btn.Text = "Change ADB"
        btn_apk.Enabled = True
        Call Get_adb_version(adbpath)
        Return adbpath
    End Function

    Function Get_adb_version(adbpath) As Integer
        Dim version As New Process
        With version
            .StartInfo.UseShellExecute = False
            .StartInfo.RedirectStandardOutput = True
            .StartInfo.FileName = adbpath
            .StartInfo.Arguments = "version"
            .StartInfo.CreateNoWindow = True
            .Start()
            .WaitForExit()
        End With
        Dim adb_full_version = version.StandardOutput.ReadToEnd().Split(vbCrLf)(0).Split(" ").LastOrDefault
        adb_version = adb_full_version.Split(".").LastOrDefault
        lbl_Adb_version.Text = $"version: {adb_full_version}"
        lbl_Adb_version.Visible = True
        Return adb_version
    End Function


    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        PictureBox1.Dispose()
    End Sub

    Public Sub Rerun_Click(sender As Object, e As EventArgs) Handles rerun.Click

        For i As Integer = 0 To apkpaths.Length - 1
            lbl_apkpath.Text = apkpaths(i)
            apklistbox.Items.Add(apkpaths(i).Split("\").LastOrDefault) 'GetValue(qapkpatharr.Last)) 'don't judge me, i was tired and this fix was taking too much time
            Call Apkinstaller(apkpaths(i))
        Next
    End Sub

    Private Sub darkcheck_CheckedChanged(sender As Object, e As EventArgs) Handles darkcheck.CheckedChanged
        If darkcheck.Checked Then
            Environment.SetEnvironmentVariable("darkmode", "dark", EnvironmentVariableTarget.User)
        Else
            Environment.SetEnvironmentVariable("darkmode", "light", EnvironmentVariableTarget.User)
        End If

        If Environment.GetEnvironmentVariable("darkmode", EnvironmentVariableTarget.User) = "dark" Then
            BackColor = Color.Black
            ForeColor = Color.White
        ElseIf Environment.GetEnvironmentVariable("darkmode", EnvironmentVariableTarget.User) = "light" Then
            BackColor = Color.White
            ForeColor = Color.Black
        End If
    End Sub

    Private Sub Button2_Click(a As Object, e As EventArgs) Handles ADB_btn.Click
        Dim adbpathdialog = New OpenFileDialog
        With adbpathdialog
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
        If adbpathdialog.FileName = "" Then
            Return
        End If
        Adbpath = adbpathdialog.FileName
        lbl_adbpath.Text = Adbpath
        btn_apk.Visible = True
        GroupBox1.Text = grptxt1 + "(Selected binary)"
        Call Get_adb_version(Adbpath)

    End Sub

    Public Sub Adbpathdialog_FileOk(sender As Object, e As System.ComponentModel.CancelEventArgs) Handles Adbpathdialog.FileOk
        Try
            Adbpath = Adbpathdialog.FileName
        Catch
            MsgBox("cold feet?")
        End Try

    End Sub

    Public Sub Btn_APK_Click(sender As Object, e As EventArgs) Handles btn_apk.Click
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
        If apkpathdialog.FileNames.Length >= 1 Then
            apklistgrp.Visible = True
        End If
        apkpaths = apkpathdialog.FileNames
        For i As Integer = 0 To apkpathdialog.FileNames.Length - 1


            lbl_apkpath.Text = apkpathdialog.FileNames(i)
            apklistbox.Items.Add(apkpathdialog.FileNames(i).Split("\").LastOrDefault) 'GetValue(qapkpatharr.Last)) 'don't judge me, i was tired and this fix was taking too much time
            Call Apkinstaller(apkpathdialog.FileNames(i))
        Next
        'lbl_apkpath.Text = apkpath
        'Call Apkinstaller(apkpath)
    End Sub


    Public Function Waitfordevice()
        Dim waitfordevices As New Process
        With waitfordevices
            .StartInfo.UseShellExecute = False
            .StartInfo.RedirectStandardOutput = True
            .StartInfo.FileName = Adbpath
            .StartInfo.Arguments = "wait-for-any-device"
            '.StartInfo.Arguments = "devices"
            .StartInfo.WindowStyle = ProcessWindowStyle.Hidden
            .Start()
            .WaitForExit()
        End With
        Using serialreader As New Process
            With serialreader
                .StartInfo.UseShellExecute = False
                .StartInfo.RedirectStandardOutput = True
                .StartInfo.FileName = Adbpath
                .StartInfo.Arguments = "devices -l"
                '.StartInfo.Arguments = "devices"
                .StartInfo.WindowStyle = ProcessWindowStyle.Hidden
                .Start()
                .WaitForExit(3000)
            End With
            If serialreader.HasExited Then
                Dim strreadr As StringReader = New StringReader(waitfordevices.StandardOutput.ReadToEnd())
                Dim output = strreadr.ReadToEnd
                Return output
                Exit Function
            End If
        End Using

        Return waitfordevices.Id
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="apkpath"></param>
    ''' <returns></returns>
    Public Function Apkinstaller(apkpath As String)
        If Not silent Then
            msbox("Installing " + apkpath + " to Device", 5, "Installing")
        End If
        Dim apkinstall As New Process
        With apkinstall
            .StartInfo.UseShellExecute = False
            If silent Then
                .StartInfo.RedirectStandardOutput = False
            Else
                .StartInfo.RedirectStandardOutput = True
            End If
            .StartInfo.FileName = Adbpath
            .StartInfo.Arguments = "install -r " + """" + apkpath + """"
            .StartInfo.WindowStyle = 1
            .Start()
            '.WaitForExit()
        End With
        Dim ed As String = apkinstall.StandardOutput.ReadToEnd.ToString()
        If silent Then
            msbox(ed)                                              'remove when done or make part of program
        Else
            Dim edarr() As String = ed.Split(vbCrLf)
            If apkresultbox.Visible = False Then
                apkresultbox.Visible = True
            End If
            Select Case edarr.Length
                Case 0
                    apkresultbox.Items.Add("error: confirm from device")
                Case 2
                    apkresultbox.Items.Add(edarr(0))
                Case > 2
                    apkresultbox.Items.Add(edarr.GetValue(edarr.Length - 2))
            End Select
            Dim counting As Timer = Timer1

    End Function

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        FormUtils.MsgBoxDelayClose(Adbpath, 5, MsgBoxResult.Cancel, MsgBoxStyle.Critical, adb_version)
        'Me.Hide()
        'Explorer1.Show()
    End Sub
End Class




