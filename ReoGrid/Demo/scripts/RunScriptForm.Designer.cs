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

namespace unvell.ReoGrid.Demo.Scripts
{
	partial class RunScriptForm
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
			this.btnHelloworld = new System.Windows.Forms.Button();
			this.button1 = new System.Windows.Forms.Button();
			this.grid = new unvell.ReoGrid.ReoGridControl();
			this.button2 = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// btnHelloworld
			// 
			this.btnHelloworld.Location = new System.Drawing.Point(496, 12);
			this.btnHelloworld.Name = "btnHelloworld";
			this.btnHelloworld.Size = new System.Drawing.Size(165, 27);
			this.btnHelloworld.TabIndex = 2;
			this.btnHelloworld.Text = "Hello world";
			this.btnHelloworld.UseVisualStyleBackColor = true;
			this.btnHelloworld.Click += new System.EventHandler(this.btnHelloworld_Click);
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(496, 45);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(165, 27);
			this.button1.TabIndex = 3;
			this.button1.Text = "Get current selection";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// grid
			// 
			this.grid.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
			this.grid.CellContextMenuStrip = null;
			this.grid.ColHeadContextMenuStrip = null;
			this.grid.Location = new System.Drawing.Point(3, 3);
			this.grid.Name = "grid";
			this.grid.RowHeadContextMenuStrip = null;
			this.grid.ScaleFactor = 1F;
			this.grid.Script = null;
			this.grid.Size = new System.Drawing.Size(472, 335);
			this.grid.TabIndex = 1;
			this.grid.TabStop = true;
			this.grid.Text = "reoGridControl1";
			// 
			// button2
			// 
			this.button2.Location = new System.Drawing.Point(496, 78);
			this.button2.Name = "button2";
			this.button2.Size = new System.Drawing.Size(165, 27);
			this.button2.TabIndex = 4;
			this.button2.Text = "Set focus cell back color";
			this.button2.UseVisualStyleBackColor = true;
			this.button2.Click += new System.EventHandler(this.button2_Click);
			// 
			// RunScriptForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(689, 340);
			this.Controls.Add(this.button2);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.btnHelloworld);
			this.Controls.Add(this.grid);
			this.Name = "RunScriptForm";
			this.Text = "RunScriptForm";
			this.ResumeLayout(false);

		}

		#endregion

		private ReoGridControl grid;
		private System.Windows.Forms.Button btnHelloworld;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.Button button2;
	}
}