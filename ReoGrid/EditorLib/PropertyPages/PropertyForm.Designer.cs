/*****************************************************************************
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

namespace unvell.ReoGrid.PropertyPages
{
	partial class PropertyForm
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
			this.tabControl1 = new System.Windows.Forms.TabControl();
			this.tabPage1 = new System.Windows.Forms.TabPage();
			this.numberPage = new unvell.ReoGrid.PropertyPages.FormatPage();
			this.tabBorder = new System.Windows.Forms.TabPage();
			this.borderPage = new unvell.ReoGrid.PropertyPages.BorderPropertyPage();
			this.tabPage2 = new System.Windows.Forms.TabPage();
			this.fillPage1 = new unvell.ReoGrid.PropertyPages.FillPage();
			this.btnCancel = new System.Windows.Forms.Button();
			this.btnOK = new System.Windows.Forms.Button();
			this.panel1 = new System.Windows.Forms.Panel();
			this.splitter1 = new System.Windows.Forms.Splitter();
			this.tabPage3 = new System.Windows.Forms.TabPage();
			this.alignmentPage1 = new unvell.ReoGrid.Editor.PropertyPages.AlignmentPage();
			this.tabControl1.SuspendLayout();
			this.tabPage1.SuspendLayout();
			this.tabBorder.SuspendLayout();
			this.tabPage2.SuspendLayout();
			this.panel1.SuspendLayout();
			this.tabPage3.SuspendLayout();
			this.SuspendLayout();
			// 
			// tabControl1
			// 
			this.tabControl1.Controls.Add(this.tabPage1);
			this.tabControl1.Controls.Add(this.tabPage3);
			this.tabControl1.Controls.Add(this.tabBorder);
			this.tabControl1.Controls.Add(this.tabPage2);
			this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tabControl1.Location = new System.Drawing.Point(6, 6);
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.Size = new System.Drawing.Size(573, 354);
			this.tabControl1.TabIndex = 0;
			// 
			// tabPage1
			// 
			this.tabPage1.Controls.Add(this.numberPage);
			this.tabPage1.Location = new System.Drawing.Point(4, 22);
			this.tabPage1.Name = "tabPage1";
			this.tabPage1.Size = new System.Drawing.Size(565, 328);
			this.tabPage1.TabIndex = 1;
			this.tabPage1.Text = "Format";
			this.tabPage1.UseVisualStyleBackColor = true;
			// 
			// numberPage
			// 
			this.numberPage.Dock = System.Windows.Forms.DockStyle.Fill;
			this.numberPage.Grid = null;
			this.numberPage.Location = new System.Drawing.Point(0, 0);
			this.numberPage.Name = "numberPage";
			this.numberPage.Size = new System.Drawing.Size(565, 328);
			this.numberPage.TabIndex = 0;
			// 
			// tabBorder
			// 
			this.tabBorder.Controls.Add(this.borderPage);
			this.tabBorder.Location = new System.Drawing.Point(4, 22);
			this.tabBorder.Name = "tabBorder";
			this.tabBorder.Size = new System.Drawing.Size(565, 328);
			this.tabBorder.TabIndex = 0;
			this.tabBorder.Text = "Border";
			this.tabBorder.UseVisualStyleBackColor = true;
			// 
			// borderPage
			// 
			this.borderPage.Dock = System.Windows.Forms.DockStyle.Fill;
			this.borderPage.Grid = null;
			this.borderPage.Location = new System.Drawing.Point(0, 0);
			this.borderPage.Name = "borderPage";
			this.borderPage.Size = new System.Drawing.Size(565, 328);
			this.borderPage.TabIndex = 0;
			// 
			// tabPage2
			// 
			this.tabPage2.Controls.Add(this.fillPage1);
			this.tabPage2.Location = new System.Drawing.Point(4, 22);
			this.tabPage2.Name = "tabPage2";
			this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage2.Size = new System.Drawing.Size(565, 328);
			this.tabPage2.TabIndex = 2;
			this.tabPage2.Text = "Fill";
			this.tabPage2.UseVisualStyleBackColor = true;
			// 
			// fillPage1
			// 
			this.fillPage1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.fillPage1.Grid = null;
			this.fillPage1.Location = new System.Drawing.Point(3, 3);
			this.fillPage1.Name = "fillPage1";
			this.fillPage1.Size = new System.Drawing.Size(559, 322);
			this.fillPage1.TabIndex = 0;
			// 
			// btnCancel
			// 
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Dock = System.Windows.Forms.DockStyle.Right;
			this.btnCancel.Location = new System.Drawing.Point(491, 6);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(80, 22);
			this.btnCancel.TabIndex = 1;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.UseVisualStyleBackColor = true;
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			// 
			// btnOK
			// 
			this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnOK.Dock = System.Windows.Forms.DockStyle.Right;
			this.btnOK.Location = new System.Drawing.Point(403, 6);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(80, 22);
			this.btnOK.TabIndex = 1;
			this.btnOK.Text = "OK";
			this.btnOK.UseVisualStyleBackColor = true;
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.btnOK);
			this.panel1.Controls.Add(this.splitter1);
			this.panel1.Controls.Add(this.btnCancel);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.panel1.Location = new System.Drawing.Point(6, 360);
			this.panel1.Name = "panel1";
			this.panel1.Padding = new System.Windows.Forms.Padding(10, 6, 2, 0);
			this.panel1.Size = new System.Drawing.Size(573, 28);
			this.panel1.TabIndex = 2;
			// 
			// splitter1
			// 
			this.splitter1.Dock = System.Windows.Forms.DockStyle.Right;
			this.splitter1.Enabled = false;
			this.splitter1.Location = new System.Drawing.Point(483, 6);
			this.splitter1.Name = "splitter1";
			this.splitter1.Size = new System.Drawing.Size(8, 22);
			this.splitter1.TabIndex = 2;
			this.splitter1.TabStop = false;
			// 
			// tabPage3
			// 
			this.tabPage3.Controls.Add(this.alignmentPage1);
			this.tabPage3.Location = new System.Drawing.Point(4, 22);
			this.tabPage3.Name = "tabPage3";
			this.tabPage3.Size = new System.Drawing.Size(565, 328);
			this.tabPage3.TabIndex = 3;
			this.tabPage3.Text = "Alignment";
			this.tabPage3.UseVisualStyleBackColor = true;
			// 
			// alignmentPage1
			// 
			this.alignmentPage1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.alignmentPage1.Location = new System.Drawing.Point(0, 0);
			this.alignmentPage1.Name = "alignmentPage1";
			this.alignmentPage1.Size = new System.Drawing.Size(565, 328);
			this.alignmentPage1.TabIndex = 0;
			// 
			// PropertyForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btnCancel;
			this.ClientSize = new System.Drawing.Size(585, 394);
			this.Controls.Add(this.tabControl1);
			this.Controls.Add(this.panel1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "PropertyForm";
			this.Padding = new System.Windows.Forms.Padding(6, 6, 6, 6);
			this.ShowInTaskbar = false;
			this.Text = "Format Cells";
			this.tabControl1.ResumeLayout(false);
			this.tabPage1.ResumeLayout(false);
			this.tabBorder.ResumeLayout(false);
			this.tabPage2.ResumeLayout(false);
			this.panel1.ResumeLayout(false);
			this.tabPage3.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TabControl tabControl1;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Splitter splitter1;
		private System.Windows.Forms.TabPage tabBorder;
		private BorderPropertyPage borderPage;
		private System.Windows.Forms.TabPage tabPage1;
		private FormatPage numberPage;
		private System.Windows.Forms.TabPage tabPage2;
		private FillPage fillPage1;
		private System.Windows.Forms.TabPage tabPage3;
		private Editor.PropertyPages.AlignmentPage alignmentPage1;
	}
}