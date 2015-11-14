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
	partial class HandleEventsForm
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HandleEventsForm));
			this.chkOnload = new System.Windows.Forms.CheckBox();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.chkSelectionMoveNext = new System.Windows.Forms.CheckBox();
			this.chkOnCut = new System.Windows.Forms.CheckBox();
			this.chkOnPaste = new System.Windows.Forms.CheckBox();
			this.chkOnCopy = new System.Windows.Forms.CheckBox();
			this.chkUnload = new System.Windows.Forms.CheckBox();
			this.chkCellDataChanged = new System.Windows.Forms.CheckBox();
			this.chkCellBeforeEdit = new System.Windows.Forms.CheckBox();
			this.chkSelectionChange = new System.Windows.Forms.CheckBox();
			this.chkCellMouseUp = new System.Windows.Forms.CheckBox();
			this.chkCellMouseDown = new System.Windows.Forms.CheckBox();
			this.btnReset = new System.Windows.Forms.Button();
			this.listbox1 = new System.Windows.Forms.ListBox();
			this.grid = new unvell.ReoGrid.ReoGridControl();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// chkOnload
			// 
			this.chkOnload.AutoSize = true;
			this.chkOnload.Location = new System.Drawing.Point(18, 27);
			this.chkOnload.Name = "chkOnload";
			this.chkOnload.Size = new System.Drawing.Size(57, 16);
			this.chkOnload.TabIndex = 2;
			this.chkOnload.Text = "onload";
			this.chkOnload.UseVisualStyleBackColor = true;
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.chkSelectionMoveNext);
			this.groupBox1.Controls.Add(this.chkOnCut);
			this.groupBox1.Controls.Add(this.chkOnPaste);
			this.groupBox1.Controls.Add(this.chkOnCopy);
			this.groupBox1.Controls.Add(this.chkUnload);
			this.groupBox1.Controls.Add(this.chkCellDataChanged);
			this.groupBox1.Controls.Add(this.chkCellBeforeEdit);
			this.groupBox1.Controls.Add(this.chkSelectionChange);
			this.groupBox1.Controls.Add(this.chkCellMouseUp);
			this.groupBox1.Controls.Add(this.chkCellMouseDown);
			this.groupBox1.Controls.Add(this.chkOnload);
			this.groupBox1.Location = new System.Drawing.Point(457, 11);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(185, 339);
			this.groupBox1.TabIndex = 3;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Events";
			// 
			// chkSelectionMoveNext
			// 
			this.chkSelectionMoveNext.AutoSize = true;
			this.chkSelectionMoveNext.Location = new System.Drawing.Point(18, 90);
			this.chkSelectionMoveNext.Name = "chkSelectionMoveNext";
			this.chkSelectionMoveNext.Size = new System.Drawing.Size(86, 16);
			this.chkSelectionMoveNext.TabIndex = 12;
			this.chkSelectionMoveNext.Text = "onnextfocus";
			this.chkSelectionMoveNext.UseVisualStyleBackColor = true;
			// 
			// chkOnCut
			// 
			this.chkOnCut.AutoSize = true;
			this.chkOnCut.Location = new System.Drawing.Point(18, 239);
			this.chkOnCut.Name = "chkOnCut";
			this.chkOnCut.Size = new System.Drawing.Size(52, 16);
			this.chkOnCut.TabIndex = 11;
			this.chkOnCut.Text = "oncut";
			this.chkOnCut.UseVisualStyleBackColor = true;
			// 
			// chkOnPaste
			// 
			this.chkOnPaste.AutoSize = true;
			this.chkOnPaste.Location = new System.Drawing.Point(18, 218);
			this.chkOnPaste.Name = "chkOnPaste";
			this.chkOnPaste.Size = new System.Drawing.Size(64, 16);
			this.chkOnPaste.TabIndex = 10;
			this.chkOnPaste.Text = "onpaste";
			this.chkOnPaste.UseVisualStyleBackColor = true;
			// 
			// chkOnCopy
			// 
			this.chkOnCopy.AutoSize = true;
			this.chkOnCopy.Location = new System.Drawing.Point(18, 197);
			this.chkOnCopy.Name = "chkOnCopy";
			this.chkOnCopy.Size = new System.Drawing.Size(60, 16);
			this.chkOnCopy.TabIndex = 9;
			this.chkOnCopy.Text = "oncopy";
			this.chkOnCopy.UseVisualStyleBackColor = true;
			// 
			// chkUnload
			// 
			this.chkUnload.AutoSize = true;
			this.chkUnload.Location = new System.Drawing.Point(18, 48);
			this.chkUnload.Name = "chkUnload";
			this.chkUnload.Size = new System.Drawing.Size(57, 16);
			this.chkUnload.TabIndex = 8;
			this.chkUnload.Text = "unload";
			this.chkUnload.UseVisualStyleBackColor = true;
			// 
			// chkCellDataChanged
			// 
			this.chkCellDataChanged.AutoSize = true;
			this.chkCellDataChanged.Location = new System.Drawing.Point(18, 175);
			this.chkCellDataChanged.Name = "chkCellDataChanged";
			this.chkCellDataChanged.Size = new System.Drawing.Size(92, 16);
			this.chkCellDataChanged.TabIndex = 7;
			this.chkCellDataChanged.Text = "ontextchange";
			this.chkCellDataChanged.UseVisualStyleBackColor = true;
			// 
			// chkCellBeforeEdit
			// 
			this.chkCellBeforeEdit.AutoSize = true;
			this.chkCellBeforeEdit.Location = new System.Drawing.Point(18, 154);
			this.chkCellBeforeEdit.Name = "chkCellBeforeEdit";
			this.chkCellBeforeEdit.Size = new System.Drawing.Size(73, 16);
			this.chkCellBeforeEdit.TabIndex = 6;
			this.chkCellBeforeEdit.Text = "oncelledit";
			this.chkCellBeforeEdit.UseVisualStyleBackColor = true;
			// 
			// chkSelectionChange
			// 
			this.chkSelectionChange.AutoSize = true;
			this.chkSelectionChange.Location = new System.Drawing.Point(18, 69);
			this.chkSelectionChange.Name = "chkSelectionChange";
			this.chkSelectionChange.Size = new System.Drawing.Size(118, 16);
			this.chkSelectionChange.TabIndex = 5;
			this.chkSelectionChange.Text = "onselectionchange";
			this.chkSelectionChange.UseVisualStyleBackColor = true;
			// 
			// chkCellMouseUp
			// 
			this.chkCellMouseUp.AutoSize = true;
			this.chkCellMouseUp.Location = new System.Drawing.Point(18, 133);
			this.chkCellMouseUp.Name = "chkCellMouseUp";
			this.chkCellMouseUp.Size = new System.Drawing.Size(81, 16);
			this.chkCellMouseUp.TabIndex = 4;
			this.chkCellMouseUp.Text = "onmouseup";
			this.chkCellMouseUp.UseVisualStyleBackColor = true;
			// 
			// chkCellMouseDown
			// 
			this.chkCellMouseDown.AutoSize = true;
			this.chkCellMouseDown.Location = new System.Drawing.Point(18, 112);
			this.chkCellMouseDown.Name = "chkCellMouseDown";
			this.chkCellMouseDown.Size = new System.Drawing.Size(95, 16);
			this.chkCellMouseDown.TabIndex = 3;
			this.chkCellMouseDown.Text = "onmousedown";
			this.chkCellMouseDown.UseVisualStyleBackColor = true;
			// 
			// btnReset
			// 
			this.btnReset.Location = new System.Drawing.Point(475, 364);
			this.btnReset.Name = "btnReset";
			this.btnReset.Size = new System.Drawing.Size(157, 23);
			this.btnReset.TabIndex = 4;
			this.btnReset.Text = "Reset Control";
			this.btnReset.UseVisualStyleBackColor = true;
			this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
			// 
			// listbox1
			// 
			this.listbox1.FormattingEnabled = true;
			this.listbox1.ItemHeight = 12;
			this.listbox1.Location = new System.Drawing.Point(648, 11);
			this.listbox1.Name = "listbox1";
			this.listbox1.Size = new System.Drawing.Size(230, 376);
			this.listbox1.TabIndex = 5;
			// 
			// grid
			// 
			this.grid.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
			this.grid.CellContextMenuStrip = null;
			this.grid.ColCount = 100;
			this.grid.ColHeadContextMenuStrip = null;
			this.grid.Location = new System.Drawing.Point(3, 3);
			this.grid.Name = "grid";
			this.grid.RowCount = 200;
			this.grid.RowHeadContextMenuStrip = null;
			this.grid.Script = null;
			this.grid.Size = new System.Drawing.Size(448, 390);
			this.grid.TabIndex = 1;
			this.grid.TabStop = true;
			this.grid.Text = "reoGridControl1";
			// 
			// HandleEventsForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(885, 395);
			this.Controls.Add(this.listbox1);
			this.Controls.Add(this.btnReset);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.grid);
			this.Name = "HandleEventsForm";
			this.Text = "RunScriptForm";
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private ReoGridControl grid;
		private System.Windows.Forms.CheckBox chkOnload;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Button btnReset;
		private System.Windows.Forms.CheckBox chkCellMouseDown;
		private System.Windows.Forms.ListBox listbox1;
		private System.Windows.Forms.CheckBox chkCellMouseUp;
		private System.Windows.Forms.CheckBox chkSelectionChange;
		private System.Windows.Forms.CheckBox chkUnload;
		private System.Windows.Forms.CheckBox chkCellDataChanged;
		private System.Windows.Forms.CheckBox chkCellBeforeEdit;
		private System.Windows.Forms.CheckBox chkSelectionMoveNext;
		private System.Windows.Forms.CheckBox chkOnCut;
		private System.Windows.Forms.CheckBox chkOnPaste;
		private System.Windows.Forms.CheckBox chkOnCopy;
	}
}