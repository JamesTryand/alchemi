#region Alchemi copyright and license notice

/*
* Alchemi [.NET Grid Computing Framework]
* http://www.alchemi.net
*
* Title			:	ConsoleParentForm.cs
* Project		:	Alchemi Console
* Created on	:	Sep 2005
* Copyright		:	Copyright � 2005 The University of Melbourne
*					This technology has been developed with the support of 
*					the Australian Research Council and the University of Melbourne
*					research grants as part of the Gridbus Project
*					within GRIDS Laboratory at the University of Melbourne, Australia.
* Author         :  Krishna Nadiminti (kna@cs.mu.oz.au) and Rajkumar Buyya (raj@cs.mu.oz.au)
* License        :  GPL
*					This program is free software; you can redistribute it and/or 
*					modify it under the terms of the GNU General Public
*					License as published by the Free Software Foundation;
*					See the GNU General Public License 
*					(http://www.gnu.org/copyleft/gpl.html) for more details.
*
*/ 
#endregion

using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace Alchemi.Console
{
	/// <summary>
	/// Summary description for ConsoleParentForm.
	/// </summary>
	public class ConsoleParentForm : System.Windows.Forms.Form
	{
		private ConsoleForm mainchild = null;
		private ArrayList consoleForms;

		private System.Windows.Forms.MainMenu mainMenu1;
		private System.Windows.Forms.MenuItem mnuWindow;
		private System.Windows.Forms.MenuItem mnuCascade;
		private System.Windows.Forms.MenuItem mnuTileH;
		private System.Windows.Forms.MenuItem mnuTileV;
		private System.Windows.Forms.MenuItem mnuHelp;
		private System.Windows.Forms.MenuItem mnuAbout;
		private System.Windows.Forms.MenuItem mnuFile;
		private System.Windows.Forms.MenuItem mnuExit;
		private System.Windows.Forms.MenuItem mnuGridConnect;
		private System.Windows.Forms.MenuItem mnuFileSep1;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public ConsoleParentForm()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
			consoleForms = new ArrayList();

			//show the main child form
			mainchild = new ConsoleForm();
			mainchild.MdiParent = this;
			mainchild.WindowState = FormWindowState.Maximized;
			mainchild.ControlBox = false;
			mainchild.Show();
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.mainMenu1 = new System.Windows.Forms.MainMenu();
			this.mnuFile = new System.Windows.Forms.MenuItem();
			this.mnuGridConnect = new System.Windows.Forms.MenuItem();
			this.mnuFileSep1 = new System.Windows.Forms.MenuItem();
			this.mnuExit = new System.Windows.Forms.MenuItem();
			this.mnuWindow = new System.Windows.Forms.MenuItem();
			this.mnuCascade = new System.Windows.Forms.MenuItem();
			this.mnuTileH = new System.Windows.Forms.MenuItem();
			this.mnuTileV = new System.Windows.Forms.MenuItem();
			this.mnuHelp = new System.Windows.Forms.MenuItem();
			this.mnuAbout = new System.Windows.Forms.MenuItem();
			// 
			// mainMenu1
			// 
			this.mainMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					  this.mnuFile,
																					  this.mnuWindow,
																					  this.mnuHelp});
			// 
			// mnuFile
			// 
			this.mnuFile.Index = 0;
			this.mnuFile.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					this.mnuGridConnect,
																					this.mnuFileSep1,
																					this.mnuExit});
			this.mnuFile.Text = "File";
			// 
			// mnuGridConnect
			// 
			this.mnuGridConnect.Index = 0;
			this.mnuGridConnect.Text = "New Grid Connection..";
			this.mnuGridConnect.Click += new System.EventHandler(this.mnuGridConnect_Click);
			// 
			// mnuFileSep1
			// 
			this.mnuFileSep1.Index = 1;
			this.mnuFileSep1.Text = "-";
			// 
			// mnuExit
			// 
			this.mnuExit.Index = 2;
			this.mnuExit.Text = "Exit";
			this.mnuExit.Click += new System.EventHandler(this.mnuExit_Click);
			// 
			// mnuWindow
			// 
			this.mnuWindow.Index = 1;
			this.mnuWindow.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					  this.mnuCascade,
																					  this.mnuTileH,
																					  this.mnuTileV});
			this.mnuWindow.MergeOrder = 1;
			this.mnuWindow.Text = "Window";
			// 
			// mnuCascade
			// 
			this.mnuCascade.Index = 0;
			this.mnuCascade.Text = "Cascade";
			this.mnuCascade.Click += new System.EventHandler(this.mnuWindow_Click);
			// 
			// mnuTileH
			// 
			this.mnuTileH.Index = 1;
			this.mnuTileH.Text = "Tile Horizontally";
			this.mnuTileH.Click += new System.EventHandler(this.mnuWindow_Click);
			// 
			// mnuTileV
			// 
			this.mnuTileV.Index = 2;
			this.mnuTileV.Text = "Tile Vertically";
			this.mnuTileV.Click += new System.EventHandler(this.mnuWindow_Click);
			// 
			// mnuHelp
			// 
			this.mnuHelp.Index = 2;
			this.mnuHelp.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					this.mnuAbout});
			this.mnuHelp.MergeOrder = 2;
			this.mnuHelp.Text = "Help";
			// 
			// mnuAbout
			// 
			this.mnuAbout.Index = 0;
			this.mnuAbout.Text = "About";
			this.mnuAbout.Click += new System.EventHandler(this.mnuAbout_Click);
			// 
			// ConsoleParentForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(692, 453);
			this.IsMdiContainer = true;
			this.Menu = this.mainMenu1;
			this.Name = "ConsoleParentForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "ConsoleParentForm";

		}
		#endregion

		private void mnuExit_Click(object sender, System.EventArgs e)
		{
			//todo disconnect everything before we close.
			//close all forms.
			//then exit application
			foreach (Object obj in consoleForms)
			{
				if (obj is Form)
				{
					if (obj != mainchild)
						((Form)obj).Close();
				}
			}
			Application.Exit();
		}

		private void mnuGridConnect_Click(object sender, System.EventArgs e)
		{
			//new grid connection
			ConsoleForm cf = new ConsoleForm();
			cf.MdiParent = this;
			cf.Show();
			cf.Closed += new EventHandler(ConsoleChildForm_Closed);
			consoleForms.Add(cf);
		}

		private void mnuWindow_Click(object sender, System.EventArgs e)
		{
			//todo
		}

		private void mnuAbout_Click(object sender, System.EventArgs e)
		{
			//todo
		}

		private void ConsoleChildForm_Closed(object sender, EventArgs e)
		{
			if (consoleForms.Contains(sender))
			{
				consoleForms.Remove(sender);

				//todo maintain and update the menu item...recent window list etc..
			}
		}
	}
}
