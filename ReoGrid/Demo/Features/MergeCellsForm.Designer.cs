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
	partial class MergeCellsForm
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
			this.btnMerge = new System.Windows.Forms.Button();
			this.btnUnmerge = new System.Windows.Forms.Button();
			this.btnMergeByScript = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.btnUnmergeByScript = new System.Windows.Forms.Button();
			this.btnSimulateException = new System.Windows.Forms.Button();
			this.grid = new unvell.ReoGrid.ReoGridControl();
			this.panel1 = new System.Windows.Forms.Panel();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// btnMerge
			// 
			this.btnMerge.Location = new System.Drawing.Point(33, 52);
			this.btnMerge.Name = "btnMerge";
			this.btnMerge.Size = new System.Drawing.Size(202, 32);
			this.btnMerge.TabIndex = 1;
			this.btnMerge.Text = "Merge Selected Range";
			this.btnMerge.UseVisualStyleBackColor = true;
			this.btnMerge.Click += new System.EventHandler(this.btnMerge_Click);
			// 
			// btnUnmerge
			// 
			this.btnUnmerge.Location = new System.Drawing.Point(33, 90);
			this.btnUnmerge.Name = "btnUnmerge";
			this.btnUnmerge.Size = new System.Drawing.Size(202, 32);
			this.btnUnmerge.TabIndex = 2;
			this.btnUnmerge.Text = "Unmerge Selected Range";
			this.btnUnmerge.UseVisualStyleBackColor = true;
			this.btnUnmerge.Click += new System.EventHandler(this.btnUnmerge_Click);
			// 
			// btnMergeByScript
			// 
			this.btnMergeByScript.Location = new System.Drawing.Point(33, 153);
			this.btnMergeByScript.Name = "btnMergeByScript";
			this.btnMergeByScript.Size = new System.Drawing.Size(202, 32);
			this.btnMergeByScript.TabIndex = 3;
			this.btnMergeByScript.Text = "Merge Selected Range by Script";
			this.btnMergeByScript.UseVisualStyleBackColor = true;
			this.btnMergeByScript.Click += new System.EventHandler(this.btnMergeByScript_Click);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(10, 11);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(83, 12);
			this.label1.TabIndex = 4;
			this.label1.Text = "Selected range:";
			// 
			// btnUnmergeByScript
			// 
			this.btnUnmergeByScript.Location = new System.Drawing.Point(33, 191);
			this.btnUnmergeByScript.Name = "btnUnmergeByScript";
			this.btnUnmergeByScript.Size = new System.Drawing.Size(202, 32);
			this.btnUnmergeByScript.TabIndex = 5;
			this.btnUnmergeByScript.Text = "Unmerge Selected Range by Script";
			this.btnUnmergeByScript.UseVisualStyleBackColor = true;
			this.btnUnmergeByScript.Click += new System.EventHandler(this.btnUnmergeByScript_Click);
			// 
			// btnSimulateException
			// 
			this.btnSimulateException.Location = new System.Drawing.Point(33, 252);
			this.btnSimulateException.Name = "btnSimulateException";
			this.btnSimulateException.Size = new System.Drawing.Size(202, 32);
			this.btnSimulateException.TabIndex = 6;
			this.btnSimulateException.Text = "Cause RangeIntersectionException";
			this.btnSimulateException.UseVisualStyleBackColor = true;
			this.btnSimulateException.Click += new System.EventHandler(this.btnSimulateException_Click);
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
			this.grid.Size = new System.Drawing.Size(620, 379);
			this.grid.TabIndex = 0;
			this.grid.TabStop = true;
			this.grid.Text = "reoGridControl1";
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.btnSimulateException);
			this.panel1.Controls.Add(this.btnUnmergeByScript);
			this.panel1.Controls.Add(this.label1);
			this.panel1.Controls.Add(this.btnMergeByScript);
			this.panel1.Controls.Add(this.btnUnmerge);
			this.panel1.Controls.Add(this.btnMerge);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Right;
			this.panel1.Location = new System.Drawing.Point(620, 0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(260, 379);
			this.panel1.TabIndex = 7;
			// 
			// MergeCellsForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(880, 379);
			this.Controls.Add(this.grid);
			this.Controls.Add(this.panel1);
			this.Name = "MergeCellsForm";
			this.Text = "MergeCellsForm";
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private ReoGridControl grid;
		private System.Windows.Forms.Button btnMerge;
		private System.Windows.Forms.Button btnUnmerge;
		private System.Windows.Forms.Button btnMergeByScript;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button btnUnmergeByScript;
		private System.Windows.Forms.Button btnSimulateException;
		private System.Windows.Forms.Panel panel1;
	}
}