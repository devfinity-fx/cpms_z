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
	partial class ClipboardEvent
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

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.chkCustomizePaste = new System.Windows.Forms.CheckBox();
			this.chkPreventPasteEvent = new System.Windows.Forms.CheckBox();
			this.grid = new unvell.ReoGrid.ReoGridControl();
			this.SuspendLayout();
			// 
			// chkCustomizePaste
			// 
			this.chkCustomizePaste.AutoSize = true;
			this.chkCustomizePaste.Location = new System.Drawing.Point(569, 69);
			this.chkCustomizePaste.Name = "chkCustomizePaste";
			this.chkCustomizePaste.Size = new System.Drawing.Size(110, 16);
			this.chkCustomizePaste.TabIndex = 7;
			this.chkCustomizePaste.Text = "Customize Paste";
			this.chkCustomizePaste.UseVisualStyleBackColor = true;
			// 
			// chkPreventPasteEvent
			// 
			this.chkPreventPasteEvent.AutoSize = true;
			this.chkPreventPasteEvent.Location = new System.Drawing.Point(569, 34);
			this.chkPreventPasteEvent.Name = "chkPreventPasteEvent";
			this.chkPreventPasteEvent.Size = new System.Drawing.Size(127, 16);
			this.chkPreventPasteEvent.TabIndex = 6;
			this.chkPreventPasteEvent.Text = "Prevent paste event";
			this.chkPreventPasteEvent.UseVisualStyleBackColor = true;
			// 
			// grid
			// 
			this.grid.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
			this.grid.CellContextMenuStrip = null;
			this.grid.ColHeadContextMenuStrip = null;
			this.grid.Dock = System.Windows.Forms.DockStyle.Left;
			this.grid.Location = new System.Drawing.Point(0, 0);
			this.grid.Name = "grid";
			this.grid.RowHeadContextMenuStrip = null;
			this.grid.Script = null;
			this.grid.Size = new System.Drawing.Size(537, 346);
			this.grid.TabIndex = 5;
			this.grid.TabStop = true;
			this.grid.Text = "reoGridControl1";
			// 
			// ClipboardEvent
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.chkCustomizePaste);
			this.Controls.Add(this.chkPreventPasteEvent);
			this.Controls.Add(this.grid);
			this.Name = "ClipboardEvent";
			this.Size = new System.Drawing.Size(806, 346);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.CheckBox chkCustomizePaste;
		private System.Windows.Forms.CheckBox chkPreventPasteEvent;
		private ReoGridControl grid;
	}
}
