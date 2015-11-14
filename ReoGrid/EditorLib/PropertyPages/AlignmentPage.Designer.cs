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

namespace unvell.ReoGrid.Editor.PropertyPages
{
	partial class AlignmentPage
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
			this.formLine1 = new unvell.UIControls.FormLine();
			this.cmbHorAlign = new System.Windows.Forms.ComboBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.cmbVerAlign = new System.Windows.Forms.ComboBox();
			this.formLine2 = new unvell.UIControls.FormLine();
			this.chkWrapText = new System.Windows.Forms.CheckBox();
			this.SuspendLayout();
			// 
			// formLine1
			// 
			this.formLine1.BackColor = System.Drawing.Color.Transparent;
			this.formLine1.LineColor = System.Drawing.Color.Empty;
			this.formLine1.Location = new System.Drawing.Point(6, 11);
			this.formLine1.Name = "formLine1";
			this.formLine1.Size = new System.Drawing.Size(341, 19);
			this.formLine1.TabIndex = 0;
			this.formLine1.Text = "Text alignment";
			// 
			// cmbHorAlign
			// 
			this.cmbHorAlign.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cmbHorAlign.FormattingEnabled = true;
			this.cmbHorAlign.Location = new System.Drawing.Point(30, 59);
			this.cmbHorAlign.Name = "cmbHorAlign";
			this.cmbHorAlign.Size = new System.Drawing.Size(209, 20);
			this.cmbHorAlign.TabIndex = 1;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(28, 44);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(58, 12);
			this.label1.TabIndex = 2;
			this.label1.Text = "&Horizontal:";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(28, 92);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(47, 12);
			this.label2.TabIndex = 4;
			this.label2.Text = "&Vertical:";
			// 
			// cmbVerAlign
			// 
			this.cmbVerAlign.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cmbVerAlign.FormattingEnabled = true;
			this.cmbVerAlign.Location = new System.Drawing.Point(30, 107);
			this.cmbVerAlign.Name = "cmbVerAlign";
			this.cmbVerAlign.Size = new System.Drawing.Size(209, 20);
			this.cmbVerAlign.TabIndex = 3;
			// 
			// formLine2
			// 
			this.formLine2.BackColor = System.Drawing.Color.Transparent;
			this.formLine2.LineColor = System.Drawing.Color.Empty;
			this.formLine2.Location = new System.Drawing.Point(6, 147);
			this.formLine2.Name = "formLine2";
			this.formLine2.Size = new System.Drawing.Size(341, 19);
			this.formLine2.TabIndex = 5;
			this.formLine2.Text = "Text control";
			// 
			// chkWrapText
			// 
			this.chkWrapText.AutoSize = true;
			this.chkWrapText.Location = new System.Drawing.Point(30, 182);
			this.chkWrapText.Name = "chkWrapText";
			this.chkWrapText.Size = new System.Drawing.Size(73, 16);
			this.chkWrapText.TabIndex = 6;
			this.chkWrapText.Text = "&Wrap text";
			this.chkWrapText.UseVisualStyleBackColor = true;
			// 
			// AlignmentPage
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.chkWrapText);
			this.Controls.Add(this.formLine2);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.cmbVerAlign);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.cmbHorAlign);
			this.Controls.Add(this.formLine1);
			this.Name = "AlignmentPage";
			this.Size = new System.Drawing.Size(584, 325);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private UIControls.FormLine formLine1;
		private System.Windows.Forms.ComboBox cmbHorAlign;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.ComboBox cmbVerAlign;
		private UIControls.FormLine formLine2;
		private System.Windows.Forms.CheckBox chkWrapText;
	}
}
