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

namespace unvell.ReoGrid.Demo.Features
{
	partial class ZoomForm
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
			this.trackBar1 = new System.Windows.Forms.TrackBar();
			this.label1 = new System.Windows.Forms.Label();
			this.grid = new unvell.ReoGrid.ReoGridControl();
			this.label2 = new System.Windows.Forms.Label();
			this.panel1 = new System.Windows.Forms.Panel();
			((System.ComponentModel.ISupportInitialize)(this.trackBar1)).BeginInit();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// trackBar1
			// 
			this.trackBar1.Location = new System.Drawing.Point(15, 37);
			this.trackBar1.Maximum = 40;
			this.trackBar1.Minimum = 1;
			this.trackBar1.Name = "trackBar1";
			this.trackBar1.Size = new System.Drawing.Size(160, 45);
			this.trackBar1.TabIndex = 3;
			this.trackBar1.Value = 10;
			this.trackBar1.Scroll += new System.EventHandler(this.trackBar1_Scroll);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(12, 13);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(35, 12);
			this.label1.TabIndex = 4;
			this.label1.Text = "Zoom:";
			// 
			// grid
			// 
			this.grid.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
			this.grid.CellContextMenuStrip = null;
			this.grid.ColCount = 100;
			this.grid.ColHeadContextMenuStrip = null;
			this.grid.Dock = System.Windows.Forms.DockStyle.Fill;
			this.grid.Location = new System.Drawing.Point(0, 0);
			this.grid.Name = "grid";
			this.grid.RowCount = 200;
			this.grid.RowHeadContextMenuStrip = null;
			this.grid.Script = null;
			this.grid.Size = new System.Drawing.Size(630, 521);
			this.grid.TabIndex = 2;
			this.grid.TabStop = false;
			this.grid.Text = "reoGridControl1";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(52, 93);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(73, 12);
			this.label2.TabIndex = 5;
			this.label2.Text = "Current: 100%";
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.label2);
			this.panel1.Controls.Add(this.label1);
			this.panel1.Controls.Add(this.trackBar1);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Right;
			this.panel1.Location = new System.Drawing.Point(630, 0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(192, 521);
			this.panel1.TabIndex = 6;
			// 
			// ZoomForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(822, 521);
			this.Controls.Add(this.grid);
			this.Controls.Add(this.panel1);
			this.Name = "ZoomForm";
			this.Text = "ZoomForm";
			((System.ComponentModel.ISupportInitialize)(this.trackBar1)).EndInit();
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private ReoGridControl grid;
		private System.Windows.Forms.TrackBar trackBar1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Panel panel1;
	}
}