﻿/*****************************************************************************
 * 
 * ReoGrid - .NET Spreadsheet Control
 * 
 * http://reogrid.codeplex.com/
 *
 * THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
 * KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
 * IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
 * PURPOSE.
 *
 * This software released under LGPLv3 license.
 * Author: Jing Lu <dujid0 at gmail.com>
 * 
 * Copyright (c) 2012-2014 unvell.com, all rights reserved.
 * 
 ****************************************************************************/

namespace unvell.ReoGrid.TestCases
{
	partial class RunForm
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RunForm));
			this.caseList = new System.Windows.Forms.ListView();
			this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader4 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader5 = new System.Windows.Forms.ColumnHeader();
			this.imageList1 = new System.Windows.Forms.ImageList(this.components);
			this.toolStrip1 = new System.Windows.Forms.ToolStrip();
			this.runToolStripButton = new System.Windows.Forms.ToolStripButton();
			this.runInConsoleToolStripButton = new System.Windows.Forms.ToolStripButton();
			this.panel1 = new System.Windows.Forms.Panel();
			this.splitter1 = new System.Windows.Forms.Splitter();
			this.tabControl1 = new System.Windows.Forms.TabControl();
			this.tabPage1 = new System.Windows.Forms.TabPage();
			this.tabPage2 = new System.Windows.Forms.TabPage();
			this.txtException = new System.Windows.Forms.TextBox();
			this.toolStrip1.SuspendLayout();
			this.tabControl1.SuspendLayout();
			this.tabPage1.SuspendLayout();
			this.tabPage2.SuspendLayout();
			this.SuspendLayout();
			// 
			// caseList
			// 
			this.caseList.CheckBoxes = true;
			this.caseList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader4,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader5});
			this.caseList.Dock = System.Windows.Forms.DockStyle.Top;
			this.caseList.FullRowSelect = true;
			this.caseList.LargeImageList = this.imageList1;
			this.caseList.Location = new System.Drawing.Point(0, 25);
			this.caseList.Name = "caseList";
			this.caseList.Size = new System.Drawing.Size(1166, 250);
			this.caseList.SmallImageList = this.imageList1;
			this.caseList.TabIndex = 0;
			this.caseList.UseCompatibleStateImageBehavior = false;
			this.caseList.View = System.Windows.Forms.View.Details;
			// 
			// columnHeader1
			// 
			this.columnHeader1.Text = "TestSet";
			this.columnHeader1.Width = 260;
			// 
			// columnHeader4
			// 
			this.columnHeader4.Text = "Last Case";
			this.columnHeader4.Width = 120;
			// 
			// columnHeader2
			// 
			this.columnHeader2.Text = "Result";
			this.columnHeader2.Width = 400;
			// 
			// columnHeader3
			// 
			this.columnHeader3.Text = "Expense";
			this.columnHeader3.Width = 120;
			// 
			// columnHeader5
			// 
			this.columnHeader5.Text = "Memory";
			this.columnHeader5.Width = 120;
			// 
			// imageList1
			// 
			this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
			this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
			this.imageList1.Images.SetKeyName(0, "silver_light_exp.png");
			this.imageList1.Images.SetKeyName(1, "green_light_exp.png");
			this.imageList1.Images.SetKeyName(2, "red_light_exp.png");
			this.imageList1.Images.SetKeyName(3, "yellow_light_exp.png");
			this.imageList1.Images.SetKeyName(4, "blue_light_exp.png");
			// 
			// toolStrip1
			// 
			this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.runToolStripButton,
            this.runInConsoleToolStripButton});
			this.toolStrip1.Location = new System.Drawing.Point(0, 0);
			this.toolStrip1.Name = "toolStrip1";
			this.toolStrip1.Size = new System.Drawing.Size(1166, 25);
			this.toolStrip1.TabIndex = 1;
			this.toolStrip1.Text = "toolStrip1";
			// 
			// runToolStripButton
			// 
			this.runToolStripButton.Image = global::unvell.ReoGrid.TestCases.Properties.Resources.FormRunHS;
			this.runToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.runToolStripButton.Name = "runToolStripButton";
			this.runToolStripButton.Size = new System.Drawing.Size(50, 22);
			this.runToolStripButton.Text = "Run";
			this.runToolStripButton.Click += new System.EventHandler(this.runToolStripButton_Click);
			// 
			// runInConsoleToolStripButton
			// 
			this.runInConsoleToolStripButton.Image = global::unvell.ReoGrid.TestCases.Properties.Resources.cmd;
			this.runInConsoleToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.runInConsoleToolStripButton.Name = "runInConsoleToolStripButton";
			this.runInConsoleToolStripButton.Size = new System.Drawing.Size(111, 22);
			this.runInConsoleToolStripButton.Text = "Run in console";
			this.runInConsoleToolStripButton.Click += new System.EventHandler(this.runInConsoleToolStripButton_Click);
			// 
			// panel1
			// 
			this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel1.Location = new System.Drawing.Point(0, 0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(1158, 380);
			this.panel1.TabIndex = 2;
			// 
			// splitter1
			// 
			this.splitter1.Dock = System.Windows.Forms.DockStyle.Top;
			this.splitter1.Location = new System.Drawing.Point(0, 275);
			this.splitter1.Name = "splitter1";
			this.splitter1.Size = new System.Drawing.Size(1166, 3);
			this.splitter1.TabIndex = 3;
			this.splitter1.TabStop = false;
			// 
			// tabControl1
			// 
			this.tabControl1.Controls.Add(this.tabPage1);
			this.tabControl1.Controls.Add(this.tabPage2);
			this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tabControl1.Location = new System.Drawing.Point(0, 278);
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.Size = new System.Drawing.Size(1166, 406);
			this.tabControl1.TabIndex = 4;
			// 
			// tabPage1
			// 
			this.tabPage1.Controls.Add(this.panel1);
			this.tabPage1.Location = new System.Drawing.Point(4, 22);
			this.tabPage1.Name = "tabPage1";
			this.tabPage1.Size = new System.Drawing.Size(1158, 380);
			this.tabPage1.TabIndex = 0;
			this.tabPage1.Text = "Grid";
			this.tabPage1.UseVisualStyleBackColor = true;
			// 
			// tabPage2
			// 
			this.tabPage2.Controls.Add(this.txtException);
			this.tabPage2.Location = new System.Drawing.Point(4, 22);
			this.tabPage2.Name = "tabPage2";
			this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage2.Size = new System.Drawing.Size(1158, 380);
			this.tabPage2.TabIndex = 1;
			this.tabPage2.Text = "Exception";
			this.tabPage2.UseVisualStyleBackColor = true;
			// 
			// txtException
			// 
			this.txtException.BackColor = System.Drawing.SystemColors.Window;
			this.txtException.Dock = System.Windows.Forms.DockStyle.Fill;
			this.txtException.Location = new System.Drawing.Point(3, 3);
			this.txtException.Multiline = true;
			this.txtException.Name = "txtException";
			this.txtException.ReadOnly = true;
			this.txtException.Size = new System.Drawing.Size(1152, 374);
			this.txtException.TabIndex = 0;
			// 
			// RunForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1166, 684);
			this.Controls.Add(this.tabControl1);
			this.Controls.Add(this.splitter1);
			this.Controls.Add(this.caseList);
			this.Controls.Add(this.toolStrip1);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "RunForm";
			this.Text = "TestCase Manager";
			this.toolStrip1.ResumeLayout(false);
			this.toolStrip1.PerformLayout();
			this.tabControl1.ResumeLayout(false);
			this.tabPage1.ResumeLayout(false);
			this.tabPage2.ResumeLayout(false);
			this.tabPage2.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ListView caseList;
		private System.Windows.Forms.ColumnHeader columnHeader1;
		private System.Windows.Forms.ColumnHeader columnHeader2;
		private System.Windows.Forms.ColumnHeader columnHeader3;
		private System.Windows.Forms.ToolStrip toolStrip1;
		private System.Windows.Forms.ToolStripButton runToolStripButton;
		private System.Windows.Forms.ImageList imageList1;
		private System.Windows.Forms.ColumnHeader columnHeader4;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Splitter splitter1;
		private System.Windows.Forms.ColumnHeader columnHeader5;
		private System.Windows.Forms.ToolStripButton runInConsoleToolStripButton;
		private System.Windows.Forms.TabControl tabControl1;
		private System.Windows.Forms.TabPage tabPage1;
		private System.Windows.Forms.TabPage tabPage2;
		private System.Windows.Forms.TextBox txtException;
	}
}

