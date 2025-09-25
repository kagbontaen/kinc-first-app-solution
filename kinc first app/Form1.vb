Imports System.IO
Imports Microsoft.Win32
Imports Microsoft.VisualBasic.FileIO
Imports System.Threading

''' <summary>
''' Utility module providing message box helpers with auto-close functionality.
''' </summary>
Module FormUtils
    ''' <summary>
    ''' Flag to check if the message box was auto-closed.
    ''' </summary>
    Public sAutoClosed As Boolean

    ''' <summary>
    ''' Default delay (in seconds) before auto-closing a message box.
    ''' </summary>
    Public Const sDefualtDelay As Integer = 5

    ''' <summary>
    ''' Worker thread that simulates pressing Enter to close a message box after delay.
    ''' </summary>
    ''' <param name="data">Delay in milliseconds</param>
    Public Sub CloseMsgBoxDelay(ByVal data As Object)
        Thread.Sleep(CInt(data))
        SendKeys.SendWait(vbCrLf)
        sAutoClosed = True
    End Sub

    ''' <summary>
    ''' Displays a MsgBox that auto-closes after a delay.
    ''' </summary>
    ''' <param name="prompt">Text to display</param>
    ''' <param name="buttons">Button style</param>
    ''' <param name="title">Dialog title</param>
    ''' <param name="delay">Time before auto-close (seconds)</param>
    ''' <param name="delayedResult">Result returned if auto-closed</param>
    ''' <returns>MsgBoxResult</returns>
    Public Function MsgBoxDelayClose(prompt As String, Optional buttons As MsgBoxStyle = MsgBoxStyle.ApplicationModal, Optional title As Object = Nothing, Optional delay As Integer = sDefualtDelay, Optional delayedResult As MsgBoxResult = MsgBoxResult.Ok) As MsgBoxResult
        Dim t As New Thread(AddressOf CloseMsgBoxDelay)

        If delay > 0 Then
            sAutoClosed = False
            t = New Thread(AddressOf CloseMsgBoxDelay)
            t.Start(delay * 1000)

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

''' <summary>
''' Main form for ADB APK Installer tool.
''' Handles adb detection, apk selection, and installation logic.
''' </summary>
Public Class Form1
    ''' <summary>Error/help text when PATH variable is missing.</summary>
    Private Const Promptpath As String = "Environment variable 'PATH' does not exist." _
                                         + vbCrLf _
                                         + "this rarely happens and is a problem on your end," _
                                         + vbCrLf _
                                         + "please use the choose adb button to select the adb file" _
                                         + vbCrLf _
                                         + "or make sure adb.exe exist in the same directory as this application"

    ''' <summary>GroupBox label prefix when adb path is displayed.</summary>
    Private Const grptxt1 = "Currently Using adb from: "

    ''' <summary>Full adb.exe path currently in use.</summary>
    Public Adbpath As String

    ''' <summary>Current APK path being installed.</summary>
    Public apkpath As String

    ''' <summary>Array of multiple APK paths selected by user.</summary>
    Public apkpaths() As String

    ''' <summary>Flag if application is running in silent mode.</summary>
    Public running_quietly = False

    ''' <summary>Flag if adb.exe was found.</summary>
    Public adbfound = False

    ''' <summary>Parsed adb version number.</summary>
    Public adb_version As Integer

    ''' <summary>
    ''' Initializes form components.
    ''' </summary>
    Public Sub New()
        InitializeComponent()
    End Sub

    ''' <summary>
    ''' Displays a popup message box using WScript.Shell with timeout.
    ''' </summary>
    Public Function Msbox(data As String, Optional buttons As MsgBoxStyle = vbOKOnly, Optional title As String = "Message box", Optional timeout As Integer = 5)
        CreateObject("WScript.Shell").Popup(data, timeout, title, buttons)
        Return "done"
    End Function

    ''' <summary>
    ''' Form load handler: processes launch parameters and checks adb status.
    ''' Applies dark mode preference if set.
    ''' </summary>
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ' Apply dark mode preference on startup
        Dim darkPref As String = Environment.GetEnvironmentVariable("darkmode", EnvironmentVariableTarget.User)
        If darkPref = "dark" Then
            darkcheck.CheckState = CheckState.Checked
            ApplyTheme(Me, True)
        Else
            darkcheck.CheckState = CheckState.Unchecked
            ApplyTheme(Me, False)
        End If

        ' Collect launch arguments
        Call Getlaunchparam()
        If running_quietly Then
            Dispose(disposing:=True)
            Application.ExitThread()
            Exit Sub
        End If
        Call CheckIfRunning()
    End Sub


    ''' <summary>
    ''' Shows AboutBox form.
    ''' </summary>
    Private Sub Btn_about_Click(sender As Object, e As EventArgs) Handles btn_about.Click
        Me.Hide()
        AboutBox1.Show()
    End Sub

    ''' <summary>
    ''' Checks if adb.exe is already running.
    ''' If not, tries to find adb in known locations.
    ''' </summary>
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
            Call Testadbpath()
            lbl_apkpath.Text = "adb Is Not running"
        End If
        Return Adbpath
    End Function

    ''' <summary>
    ''' Parses program launch parameters and determines run mode.
    ''' </summary>
    Public Function Getlaunchparam()
        If Environment.GetEnvironmentVariable("darkmode") = "dark" Then
            BackColor = Color.Black
            ForeColor = Color.White
            darkcheck.CheckState = CheckState.Checked
        End If
        On Error GoTo Er
        Select Case Environment.GetCommandLineArgs().Length
            Case Is <= 1
                running_quietly = False
                Exit Function
            Case Else
                running_quietly = True
                Call CheckIfRunning()
                Call Waitfordevice()
                For i As Integer = 1 To Environment.GetCommandLineArgs().Length - 1
                    apkpath = Environment.GetCommandLineArgs()(i)
                    Call Apkinstaller(apkpath)
                Next
                Return apkpath & " has been installed"
                Close()
        End Select

Er:
        Dim msgBoxEr = MsgBoxDelayClose("an error occurred whle getting program launch parameters" _
                              + vbCrLf +
                              "all bets are off" _
                              + vbCrLf +
                              "i wonder how you were able to launch a program without it's launch parameters" _
                              + vbCrLf +
                              "i would've loved to help you but alas i am just a humble program",
               MsgBoxStyle.Critical,
               "Something is wrong in the force")
        Return "9999"
    End Function

    ''' <summary>
    ''' Tests adb.exe availability in current dir or system PATH.
    ''' Prompts user if not found.
    ''' </summary>
    Public Function Testadbpath()
        Try
            If File.Exists($"{FileSystem.CurrentDirectory}\adb.exe") Then
                Adbpath = $"{CurDir()}\adb.exe"
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
                    Dim MsgBoxResult = MsgBoxDelayClose("I can't find adb.exe, and i have really searched" _
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
            Dim msgBoxResult = MsgBoxDelayClose(Promptpath)
        End Try
        Return Adbpath
    End Function

    ''' <summary>
    ''' Updates UI and environment when adb.exe is found.
    ''' </summary>
    Public Function Func_adbfound(adbpath As String)
        adbfound = True
        Environment.SetEnvironmentVariable("adbpath", adbpath)
        lbl_adbpath.Text = adbpath
        If Not running_quietly Then
            MsgBoxDelayClose("adb.exe was found in " + adbpath + vbCrLf + "I will therefore use it", 0, "Hurray adb.exe has been found", 5)
        End If
        ADB_btn.Text = "Change ADB"
        btn_apk.Enabled = True
        Call Get_adb_version(adbpath)
        Return adbpath
    End Function

    ''' <summary>
    ''' Runs adb version command and parses output.
    ''' </summary>
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

    ''' <summary>
    ''' Hides splash logo after delay.
    ''' </summary>
    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        PictureBox1.Dispose()
    End Sub

    ''' <summary>
    ''' Re-installs previously selected APKs.
    ''' </summary>
    Public Sub Rerun_Click(sender As Object, e As EventArgs) Handles rerun.Click
        For i As Integer = 0 To apkpaths.Length - 1
            lbl_apkpath.Text = apkpaths(i)
            apklistbox.Items.Add(apkpaths(i).Split("\").LastOrDefault)
            Call Apkinstaller(apkpaths(i))
        Next
    End Sub

    ''' <summary>
    ''' Toggles dark mode for the form and all child controls, persists preference.
    ''' </summary>
    Private Sub Darkcheck_CheckedChanged(sender As Object, e As EventArgs) Handles darkcheck.CheckedChanged
        Dim isDark As Boolean = darkcheck.Checked

        ' Save user preference in environment
        If isDark Then
            Environment.SetEnvironmentVariable("darkmode", "dark", EnvironmentVariableTarget.User)
        Else
            Environment.SetEnvironmentVariable("darkmode", "light", EnvironmentVariableTarget.User)
        End If

        ' Apply theme recursively
        ApplyTheme(Me, isDark)
    End Sub

    ''' <summary>
    ''' Applies dark or light theme to a control and all its children.
    ''' </summary>
    ''' <param name="ctrl">The root control (Form or container).</param>
    ''' <param name="dark">True for dark mode, False for light mode.</param>
    Private Sub ApplyTheme(ctrl As Control, dark As Boolean)
        If dark Then
            ctrl.BackColor = Color.Black
            ctrl.ForeColor = Color.White
        Else
            ctrl.BackColor = Color.White
            ctrl.ForeColor = Color.Black
        End If

        ' Recurse through child controls
        For Each child As Control In ctrl.Controls
            ApplyTheme(child, dark)
        Next
    End Sub


    ''' <summary>
    ''' Lets the user manually select adb.exe.
    ''' </summary>
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

    ''' <summary>
    ''' Handles adb file selection dialog confirm.
    ''' </summary>
    Public Sub Adbpathdialog_FileOk(sender As Object, e As System.ComponentModel.CancelEventArgs) Handles Adbpathdialog.FileOk
        Try
            Adbpath = Adbpathdialog.FileName
        Catch
            MsgBoxDelayClose("cold feet?")
        End Try
    End Sub

    ''' <summary>
    ''' Handles APK selection and installation process.
    ''' </summary>
    Public Sub Btn_APK_Click(sender As Object, e As EventArgs) Handles btn_apk.Click
        Waitfordevice()
        Dim apkpathdialog = New OpenFileDialog
        With apkpathdialog
            .Filter = "Android Application|*.apk|All Files|*.*"
            .Title = "Select Location of Adb Binary"
            .InitialDirectory = "c:\kk\apk"
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
            apklistbox.Items.Add(apkpathdialog.FileNames(i).Split("\").LastOrDefault)
            Call Apkinstaller(apkpathdialog.FileNames(i))
        Next
    End Sub

    ''' <summary>
    ''' Waits for device connection using adb.
    ''' </summary>
    Public Function Waitfordevice()
        Dim waitfordevices As New Process
        With waitfordevices
            .StartInfo.UseShellExecute = False
            .StartInfo.RedirectStandardOutput = True
            .StartInfo.FileName = Adbpath
            .StartInfo.Arguments = "wait-for-any-device"
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
    End Function

    ''' <summary>
    ''' Installs APK via adb install command.
    ''' </summary>
    ''' <param name="apkpath">APK file path</param>
    ''' <returns>ADB command output</returns>
    Public Function Apkinstaller(apkpath As String)
        If Not running_quietly Then
            MsgBoxDelayClose("Installing " + apkpath + " to Device", 0, "Installing")
        End If
        Dim apkinstall As New Process
        With apkinstall
            .StartInfo.UseShellExecute = False
            If running_quietly Then
                .StartInfo.RedirectStandardOutput = False
            Else
                .StartInfo.RedirectStandardOutput = True
            End If
            .StartInfo.FileName = Adbpath
            .StartInfo.Arguments = "install -r " + """" + apkpath + """"
            .StartInfo.WindowStyle = 1
            .Start()
        End With
        Dim ed As String = apkinstall.StandardOutput.ReadToEnd.ToString()
        If running_quietly Then
            MsgBoxDelayClose(ed, 0, apkpath + " has finished installing")
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
        End If
        Return ed
    End Function

    ''' <summary>
    ''' Debug/test button showing adb path.
    ''' </summary>
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        FormUtils.MsgBoxDelayClose(Adbpath, 5, MsgBoxResult.Ok, MsgBoxStyle.OkCancel, adb_version)
    End Sub
End Class
