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

namespace unvell.ReoGrid.Demo.Data
{
	partial class LargeDataForm
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LargeDataForm));
			this.grid = new unvell.ReoGrid.ReoGridControl();
			this.panel1 = new System.Windows.Forms.Panel();
			this.button1 = new System.Windows.Forms.Button();
			this.button2 = new System.Windows.Forms.Button();
			this.button3 = new System.Windows.Forms.Button();
			this.button4 = new System.Windows.Forms.Button();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
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
			this.grid.Size = new System.Drawing.Size(630, 645);
			this.grid.TabIndex = 8;
			this.grid.TabStop = true;
			this.grid.Text = "reoGridControl1";
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.button4);
			this.panel1.Controls.Add(this.button3);
			this.panel1.Controls.Add(this.button2);
			this.panel1.Controls.Add(this.button1);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Right;
			this.panel1.Location = new System.Drawing.Point(630, 0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(188, 645);
			this.panel1.TabIndex = 9;
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(6, 7);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(175, 32);
			this.button1.TabIndex = 3;
			this.button1.Text = "Append to 10000 rows";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// button2
			// 
			this.button2.Enabled = false;
			this.button2.Location = new System.Drawing.Point(6, 45);
			this.button2.Name = "button2";
			this.button2.Size = new System.Drawing.Size(175, 32);
			this.button2.TabIndex = 4;
			this.button2.Text = "Fill 10000 rows";
			this.button2.UseVisualStyleBackColor = true;
			this.button2.Click += new System.EventHandler(this.button2_Click);
			// 
			// button3
			// 
			this.button3.Enabled = false;
			this.button3.Location = new System.Drawing.Point(6, 83);
			this.button3.Name = "button3";
			this.button3.Size = new System.Drawing.Size(175, 32);
			this.button3.TabIndex = 5;
			this.button3.Text = "Append to 100,000 rows";
			this.button3.UseVisualStyleBackColor = true;
			this.button3.Click += new System.EventHandler(this.button3_Click);
			// 
			// button4
			// 
			this.button4.Enabled = false;
			this.button4.Location = new System.Drawing.Point(6, 121);
			this.button4.Name = "button4";
			this.button4.Size = new System.Drawing.Size(175, 36);
			this.button4.TabIndex = 6;
			this.button4.Text = "Fill 100,000 rows\r\nsome slowly :(";
			this.button4.UseVisualStyleBackColor = true;
			this.button4.Click += new System.EventHandler(this.button4_Click);
			// 
			// LargeDataForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(818, 645);
			this.Controls.Add(this.grid);
			this.Controls.Add(this.panel1);
			this.Name = "LargeDataForm";
			this.Text = "LargeDataForm";
			this.panel1.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private ReoGridControl grid;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.Button button4;
		private System.Windows.Forms.Button button3;
		private System.Windows.Forms.Button button2;
	}
}