//////////////////////////////////////////////////////////////////////////
// File: Bug_ReporterApp.h
//
// Description:
//   Contains main entry point to the executable.  
//
// Header File:
//   NA
//
// Copyright:
//   Copyright © 1996-2010 WysiCorp, Inc.
//   All contents of this file are considered WysiCorp Software proprietary.
//////////////////////////////////////////////////////////////////////////

Option Strict On

Public Class frmMain
	Inherits System.Windows.Forms.Form

	Public Event SaveWhileClosingCancelled As System.EventHandler
	Public Event ExitApplication As System.EventHandler

	Private m_Dirty As Boolean = False
	Private m_ClosingComplete As Boolean = False
	Private m_DocumentName As String
	Private m_FileName As String

#Region " Windows Form Designer generated code "

	Public Sub New()
		MyBase.New()

		InitializeComponent()

		Dim ainfo As New AssemblyInfo()

		Me.mnuAbout.Text = String.Format("&About {0} ...", ainfo.Title)

	End Sub

	Protected Overloads Overrides Sub Dispose(ByVal disposing As Boolean)
		If disposing Then
			If Not (components Is Nothing) Then
				components.Dispose()
			End If
		End If
		MyBase.Dispose(disposing)
	End Sub

	Private components As System.ComponentModel.IContainer

	Friend WithEvents mnuMain As System.Windows.Forms.MainMenu
	Friend WithEvents MenuItem1 As System.Windows.Forms.MenuItem
	<System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
		Dim resources As System.Resources.ResourceManager = New System.Resources.ResourceManager(GetType(frmMain))
		Me.mnuMain = New System.Windows.Forms.MainMenu()
		Me.MenuItem1 = New System.Windows.Forms.MenuItem()
		Me.SuspendLayout()
		'
		'mnuMain
		'
		Me.mnuMain.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.mnuFile, Me.mnuHelp})
		'
		'mnuFile
		'
		Me.mnuFile.Index = 0
		Me.mnuFile.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.mnuNew, Me.mnuClose, Me.MenuItem1, Me.mnuSave, Me.MenuItem2, Me.mnuExit})
		Me.mnuFile.Text = "&File"
		'
		'mnuExit
		'
		Me.mnuExit.Index = 5
		Me.mnuExit.Text = "E&xit"

	End Sub

#End Region
