﻿<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Form1
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Me.btn_apk = New System.Windows.Forms.Button()
        Me.lbl_apkpath = New System.Windows.Forms.Label()
        Me.Adbpathdialog = New System.Windows.Forms.OpenFileDialog()
        Me.ADB_btn = New System.Windows.Forms.Button()
        Me.btn_about = New System.Windows.Forms.Button()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.lbl_Adb_version = New System.Windows.Forms.Label()
        Me.lbl_adbpath = New System.Windows.Forms.Label()
        Me.Button1 = New System.Windows.Forms.Button()
        Me.Timer1 = New System.Windows.Forms.Timer(Me.components)
        Me.apklbl = New System.Windows.Forms.Label()
        Me.apklistbox = New System.Windows.Forms.ListBox()
        Me.apklistgrp = New System.Windows.Forms.GroupBox()
        Me.apkresultbox = New System.Windows.Forms.ListBox()
        Me.PictureBox1 = New System.Windows.Forms.PictureBox()
        Me.BindingSource1 = New System.Windows.Forms.BindingSource(Me.components)
        Me.rerun = New System.Windows.Forms.Button()
        Me.apklist = New System.Windows.Forms.BindingSource(Me.components)
        Me.darkcheck = New System.Windows.Forms.CheckBox()
        Me.GroupBox1.SuspendLayout()
        Me.apklistgrp.SuspendLayout()
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.BindingSource1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.apklist, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'btn_apk
        '
        Me.btn_apk.Enabled = False
        Me.btn_apk.Location = New System.Drawing.Point(420, 42)
        Me.btn_apk.Name = "btn_apk"
        Me.btn_apk.Size = New System.Drawing.Size(93, 36)
        Me.btn_apk.TabIndex = 0
        Me.btn_apk.Text = "&Select APK"
        Me.btn_apk.UseVisualStyleBackColor = True
        '
        'lbl_apkpath
        '
        Me.lbl_apkpath.AutoSize = True
        Me.lbl_apkpath.Location = New System.Drawing.Point(417, 26)
        Me.lbl_apkpath.Name = "lbl_apkpath"
        Me.lbl_apkpath.Size = New System.Drawing.Size(51, 13)
        Me.lbl_apkpath.TabIndex = 2
        Me.lbl_apkpath.Text = "Apk Path"
        '
        'Adbpathdialog
        '
        Me.Adbpathdialog.FileName = "adb.exe"
        Me.Adbpathdialog.Filter = "Application|*.exe|All files|*.*"
        Me.Adbpathdialog.Title = "Please Select The Location of ADB.exe"
        '
        'ADB_btn
        '
        Me.ADB_btn.Location = New System.Drawing.Point(6, 32)
        Me.ADB_btn.Name = "ADB_btn"
        Me.ADB_btn.Size = New System.Drawing.Size(75, 23)
        Me.ADB_btn.TabIndex = 1
        Me.ADB_btn.Text = "Select &ADB"
        Me.ADB_btn.UseVisualStyleBackColor = True
        '
        'btn_about
        '
        Me.btn_about.Location = New System.Drawing.Point(420, 243)
        Me.btn_about.Name = "btn_about"
        Me.btn_about.Size = New System.Drawing.Size(75, 23)
        Me.btn_about.TabIndex = 3
        Me.btn_about.Text = "A&bout"
        Me.btn_about.UseVisualStyleBackColor = True
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.lbl_Adb_version)
        Me.GroupBox1.Controls.Add(Me.ADB_btn)
        Me.GroupBox1.Controls.Add(Me.lbl_adbpath)
        Me.GroupBox1.Location = New System.Drawing.Point(414, 98)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(350, 62)
        Me.GroupBox1.TabIndex = 4
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "Currently using ADB from"
        '
        'lbl_Adb_version
        '
        Me.lbl_Adb_version.AutoEllipsis = True
        Me.lbl_Adb_version.AutoSize = True
        Me.lbl_Adb_version.Location = New System.Drawing.Point(107, 32)
        Me.lbl_Adb_version.Name = "lbl_Adb_version"
        Me.lbl_Adb_version.Size = New System.Drawing.Size(0, 13)
        Me.lbl_Adb_version.TabIndex = 2
        Me.lbl_Adb_version.Visible = False
        '
        'lbl_adbpath
        '
        Me.lbl_adbpath.AutoSize = True
        Me.lbl_adbpath.Location = New System.Drawing.Point(6, 16)
        Me.lbl_adbpath.Name = "lbl_adbpath"
        Me.lbl_adbpath.Size = New System.Drawing.Size(60, 13)
        Me.lbl_adbpath.TabIndex = 0
        Me.lbl_adbpath.Text = "no adb.exe"
        '
        'Button1
        '
        Me.Button1.Location = New System.Drawing.Point(420, 175)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(75, 23)
        Me.Button1.TabIndex = 5
        Me.Button1.Text = "Button1"
        Me.Button1.UseVisualStyleBackColor = True
        '
        'Timer1
        '
        Me.Timer1.Enabled = True
        Me.Timer1.Interval = 5000
        '
        'apklbl
        '
        Me.apklbl.AutoSize = True
        Me.apklbl.Location = New System.Drawing.Point(15, 17)
        Me.apklbl.Name = "apklbl"
        Me.apklbl.Size = New System.Drawing.Size(140, 13)
        Me.apklbl.TabIndex = 7
        Me.apklbl.Text = "List of Installed Applications:"
        '
        'apklistbox
        '
        Me.apklistbox.FormattingEnabled = True
        Me.apklistbox.HorizontalScrollbar = True
        Me.apklistbox.Location = New System.Drawing.Point(18, 33)
        Me.apklistbox.Name = "apklistbox"
        Me.apklistbox.Size = New System.Drawing.Size(172, 342)
        Me.apklistbox.TabIndex = 8
        '
        'apklistgrp
        '
        Me.apklistgrp.Controls.Add(Me.apkresultbox)
        Me.apklistgrp.Controls.Add(Me.apklbl)
        Me.apklistgrp.Controls.Add(Me.apklistbox)
        Me.apklistgrp.Location = New System.Drawing.Point(12, 12)
        Me.apklistgrp.Name = "apklistgrp"
        Me.apklistgrp.Size = New System.Drawing.Size(396, 377)
        Me.apklistgrp.TabIndex = 9
        Me.apklistgrp.TabStop = False
        Me.apklistgrp.Visible = False
        '
        'apkresultbox
        '
        Me.apkresultbox.FormattingEnabled = True
        Me.apkresultbox.HorizontalScrollbar = True
        Me.apkresultbox.Location = New System.Drawing.Point(187, 33)
        Me.apkresultbox.Name = "apkresultbox"
        Me.apkresultbox.Size = New System.Drawing.Size(203, 342)
        Me.apkresultbox.TabIndex = 9
        Me.apkresultbox.Visible = False
        '
        'PictureBox1
        '
        Me.PictureBox1.Image = Global.kinc_first_app.My.Resources.Resources.k_logo
        Me.PictureBox1.Location = New System.Drawing.Point(12, 12)
        Me.PictureBox1.Name = "PictureBox1"
        Me.PictureBox1.Size = New System.Drawing.Size(396, 377)
        Me.PictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.PictureBox1.TabIndex = 6
        Me.PictureBox1.TabStop = False
        '
        'rerun
        '
        Me.rerun.Location = New System.Drawing.Point(551, 42)
        Me.rerun.Name = "rerun"
        Me.rerun.Size = New System.Drawing.Size(93, 36)
        Me.rerun.TabIndex = 10
        Me.rerun.Text = "&Rerun"
        Me.rerun.UseVisualStyleBackColor = True
        '
        'apklist
        '
        Me.apklist.DataSource = GetType(kinc_first_app.Form1)
        '
        'darkcheck
        '
        Me.darkcheck.AutoSize = True
        Me.darkcheck.Location = New System.Drawing.Point(666, 409)
        Me.darkcheck.Name = "darkcheck"
        Me.darkcheck.Size = New System.Drawing.Size(75, 17)
        Me.darkcheck.TabIndex = 11
        Me.darkcheck.Text = "&Darkmode"
        Me.darkcheck.UseVisualStyleBackColor = True
        '
        'Form1
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(800, 450)
        Me.Controls.Add(Me.darkcheck)
        Me.Controls.Add(Me.rerun)
        Me.Controls.Add(Me.PictureBox1)
        Me.Controls.Add(Me.Button1)
        Me.Controls.Add(Me.GroupBox1)
        Me.Controls.Add(Me.btn_about)
        Me.Controls.Add(Me.lbl_apkpath)
        Me.Controls.Add(Me.btn_apk)
        Me.Controls.Add(Me.apklistgrp)
        Me.ForeColor = System.Drawing.Color.Black
        Me.Name = "Form1"
        Me.Text = "Form1"
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        Me.apklistgrp.ResumeLayout(False)
        Me.apklistgrp.PerformLayout()
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.BindingSource1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.apklist, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents lbl_apkpath As Label
    Friend WithEvents Adbpathdialog As OpenFileDialog
    Friend WithEvents ADB_btn As Button
    Friend WithEvents btn_about As Button
    Friend WithEvents GroupBox1 As GroupBox
    Friend WithEvents lbl_adbpath As Label
    Friend WithEvents BindingSource1 As BindingSource
    Friend WithEvents Button1 As Button
    Friend WithEvents PictureBox1 As PictureBox
    Friend WithEvents Timer1 As Timer
    Friend WithEvents apklbl As Label
    Friend WithEvents apklist As BindingSource
    Friend WithEvents apklistbox As ListBox
    Friend WithEvents apklistgrp As GroupBox
    Public WithEvents btn_apk As Button
    Public WithEvents rerun As Button
    Friend WithEvents lbl_Adb_version As Label
    Friend WithEvents apkresultbox As ListBox
    Friend WithEvents darkcheck As CheckBox
End Class
