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

using unvell.UIControls;

namespace unvell.ReoGrid.PropertyPages
{
	partial class FillPage
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
			this.sampleGroup = new System.Windows.Forms.GroupBox();
			this.labSample = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.groupPattern = new System.Windows.Forms.GroupBox();
			this.patternStyleComboBox = new unvell.UIControls.FillPatternComboBox();
			this.patternColorComboBox = new unvell.UIControls.ColorComboBox();
			this.colorPanel = new unvell.UIControls.ColorPickerPanel();
			this.sampleGroup.SuspendLayout();
			this.groupPattern.SuspendLayout();
			this.SuspendLayout();
			// 
			// sampleGroup
			// 
			this.sampleGroup.Controls.Add(this.labSample);
			this.sampleGroup.Location = new System.Drawing.Point(13, 249);
			this.sampleGroup.Name = "sampleGroup";
			this.sampleGroup.Size = new System.Drawing.Size(485, 63);
			this.sampleGroup.TabIndex = 1;
			this.sampleGroup.TabStop = false;
			this.sampleGroup.Text = "Sample";
			// 
			// labSample
			// 
			this.labSample.Location = new System.Drawing.Point(11, 16);
			this.labSample.Name = "labSample";
			this.labSample.Size = new System.Drawing.Size(461, 36);
			this.labSample.TabIndex = 0;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(28, 29);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(75, 12);
			this.label1.TabIndex = 3;
			this.label1.Text = "Pattern &Color:";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(10, 10);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(98, 12);
			this.label2.TabIndex = 4;
			this.label2.Text = "&Background Color:";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(28, 84);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(74, 12);
			this.label3.TabIndex = 3;
			this.label3.Text = "&Pattern Style:";
			// 
			// groupPattern
			// 
			this.groupPattern.Controls.Add(this.patternStyleComboBox);
			this.groupPattern.Controls.Add(this.patternColorComboBox);
			this.groupPattern.Controls.Add(this.label1);
			this.groupPattern.Controls.Add(this.label3);
			this.groupPattern.Location = new System.Drawing.Point(270, 29);
			this.groupPattern.Name = "groupPattern";
			this.groupPattern.Size = new System.Drawing.Size(228, 139);
			this.groupPattern.TabIndex = 6;
			this.groupPattern.TabStop = false;
			this.groupPattern.Text = "Fill Pattern";
			// 
			// patternStyleComboBox
			// 
			this.patternStyleComboBox.BackColor = System.Drawing.Color.White;
			this.patternStyleComboBox.dropdown = false;
			this.patternStyleComboBox.Location = new System.Drawing.Point(28, 99);
			this.patternStyleComboBox.Name = "patternStyleComboBox";
			this.patternStyleComboBox.PatternColor = System.Drawing.Color.Black;
			this.patternStyleComboBox.PatternStyle = System.Drawing.Drawing2D.HatchStyle.Horizontal;
			this.patternStyleComboBox.Size = new System.Drawing.Size(170, 21);
			this.patternStyleComboBox.TabIndex = 5;
			this.patternStyleComboBox.Text = "fillPatternComboBox1";
			// 
			// patternColorComboBox
			// 
			this.patternColorComboBox.BackColor = System.Drawing.Color.White;
			this.patternColorComboBox.SolidColor = System.Drawing.Color.Black;
			this.patternColorComboBox.dropdown = false;
			this.patternColorComboBox.Location = new System.Drawing.Point(28, 47);
			this.patternColorComboBox.Name = "patternColorComboBox";
			this.patternColorComboBox.Size = new System.Drawing.Size(170, 21);
			this.patternColorComboBox.TabIndex = 2;
			this.patternColorComboBox.Text = "colorComboBox1";
			// 
			// colorPanel
			// 
			this.colorPanel.BackColor = System.Drawing.SystemColors.Window;
			this.colorPanel.SolidColor = System.Drawing.Color.Empty;
			this.colorPanel.Location = new System.Drawing.Point(13, 29);
			this.colorPanel.Margin = new System.Windows.Forms.Padding(1);
			this.colorPanel.Name = "colorPanel";
			this.colorPanel.Padding = new System.Windows.Forms.Padding(1);
			this.colorPanel.Size = new System.Drawing.Size(173, 212);
			this.colorPanel.TabIndex = 0;
			this.colorPanel.Text = "colorPickPanel1";
			// 
			// FillPage
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.groupPattern);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.sampleGroup);
			this.Controls.Add(this.colorPanel);
			this.Name = "FillPage";
			this.Size = new System.Drawing.Size(554, 331);
			this.sampleGroup.ResumeLayout(false);
			this.groupPattern.ResumeLayout(false);
			this.groupPattern.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private unvell.UIControls.ColorPickerPanel colorPanel;
		private System.Windows.Forms.GroupBox sampleGroup;
		private System.Windows.Forms.Label labSample;
		private ColorComboBox patternColorComboBox;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private FillPatternComboBox patternStyleComboBox;
		private System.Windows.Forms.GroupBox groupPattern;
	}
}
