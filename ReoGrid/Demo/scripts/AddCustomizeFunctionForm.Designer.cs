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
	partial class AddCustomizeFunctionForm
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
			this.btnAddCustomizeFunction = new System.Windows.Forms.Button();
			this.grid = new unvell.ReoGrid.ReoGridControl();
			this.button1 = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// btnAddCustomizeFunction
			// 
			this.btnAddCustomizeFunction.Location = new System.Drawing.Point(493, 88);
			this.btnAddCustomizeFunction.Name = "btnAddCustomizeFunction";
			this.btnAddCustomizeFunction.Size = new System.Drawing.Size(228, 27);
			this.btnAddCustomizeFunction.TabIndex = 4;
			this.btnAddCustomizeFunction.Text = "Add \'func1\' Function in C#";
			this.btnAddCustomizeFunction.UseVisualStyleBackColor = true;
			this.btnAddCustomizeFunction.Click += new System.EventHandler(this.btnAddCustomizeFunction_Click);
			// 
			// grid
			// 
			this.grid.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
			this.grid.CellContextMenuStrip = null;
			this.grid.ColHeadContextMenuStrip = null;
			this.grid.Location = new System.Drawing.Point(1, 1);
			this.grid.Name = "grid";
			this.grid.RowHeadContextMenuStrip = null;
			this.grid.Script = null;
			this.grid.Size = new System.Drawing.Size(472, 335);
			this.grid.TabIndex = 3;
			this.grid.TabStop = true;
			this.grid.Text = "reoGridControl1";
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(493, 121);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(228, 27);
			this.button1.TabIndex = 5;
			this.button1.Text = "Add \'func2\' Function in Script";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// label1
			// 
			this.label1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
			this.label1.Font = new System.Drawing.Font("Lucida Console", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label1.Location = new System.Drawing.Point(491, 18);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(230, 49);
			this.label1.TabIndex = 6;
			this.label1.Text = "function func1(data) {\r\n  return \'[\' + data + \']\';\r\n}";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// AddCustomizeFunctionForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(742, 336);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.btnAddCustomizeFunction);
			this.Controls.Add(this.grid);
			this.Name = "AddCustomizeFunctionForm";
			this.Text = "AddCustomizeFunctionForm";
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button btnAddCustomizeFunction;
		private ReoGridControl grid;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.Label label1;
	}
}