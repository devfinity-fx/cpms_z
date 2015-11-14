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

namespace unvell.ReoGrid.Editor
{
	partial class ControlAppearanceEditorForm
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
			this.btnExport = new System.Windows.Forms.Button();
			this.label4 = new System.Windows.Forms.Label();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.colorPickerControl = new unvell.UIControls.ColorComboBox();
			this.lstColors = new System.Windows.Forms.ListBox();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.btnReset = new System.Windows.Forms.Button();
			this.chkUseSystemHighlight = new System.Windows.Forms.CheckBox();
			this.highlightColorPickerControl = new unvell.UIControls.ColorPickerControl();
			this.label3 = new System.Windows.Forms.Label();
			this.mainColorPickerControl = new unvell.UIControls.ColorPickerControl();
			this.label2 = new System.Windows.Forms.Label();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.label5 = new System.Windows.Forms.Label();
			this.groupBox2.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.groupBox3.SuspendLayout();
			this.SuspendLayout();
			// 
			// btnExport
			// 
			this.btnExport.Location = new System.Drawing.Point(446, 36);
			this.btnExport.Name = "btnExport";
			this.btnExport.Size = new System.Drawing.Size(81, 28);
			this.btnExport.TabIndex = 7;
			this.btnExport.Text = "Export";
			this.btnExport.UseVisualStyleBackColor = true;
			this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Font = new System.Drawing.Font("MS UI Gothic", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.label4.Location = new System.Drawing.Point(305, 115);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(40, 21);
			this.label4.TabIndex = 6;
			this.label4.Text = "OR";
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.colorPickerControl);
			this.groupBox2.Controls.Add(this.lstColors);
			this.groupBox2.Location = new System.Drawing.Point(378, 13);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(276, 231);
			this.groupBox2.TabIndex = 5;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Set colors one by one";
			// 
			// colorPickerControl
			// 
			this.colorPickerControl.BackColor = System.Drawing.Color.White;
			this.colorPickerControl.CloseOnClick = true;
			this.colorPickerControl.dropdown = false;
			this.colorPickerControl.Location = new System.Drawing.Point(22, 188);
			this.colorPickerControl.Name = "colorPickerControl";
			this.colorPickerControl.Size = new System.Drawing.Size(231, 23);
			this.colorPickerControl.SolidColor = System.Drawing.Color.Empty;
			this.colorPickerControl.TabIndex = 1;
			this.colorPickerControl.Text = "colorComboBox1";
			// 
			// lstColors
			// 
			this.lstColors.FormattingEnabled = true;
			this.lstColors.Location = new System.Drawing.Point(22, 31);
			this.lstColors.Name = "lstColors";
			this.lstColors.Size = new System.Drawing.Size(231, 147);
			this.lstColors.TabIndex = 0;
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.btnReset);
			this.groupBox1.Controls.Add(this.chkUseSystemHighlight);
			this.groupBox1.Controls.Add(this.highlightColorPickerControl);
			this.groupBox1.Controls.Add(this.label3);
			this.groupBox1.Controls.Add(this.mainColorPickerControl);
			this.groupBox1.Controls.Add(this.label2);
			this.groupBox1.Location = new System.Drawing.Point(12, 13);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(257, 231);
			this.groupBox1.TabIndex = 4;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Create with theme colors";
			// 
			// btnReset
			// 
			this.btnReset.Location = new System.Drawing.Point(48, 175);
			this.btnReset.Name = "btnReset";
			this.btnReset.Size = new System.Drawing.Size(153, 29);
			this.btnReset.TabIndex = 10;
			this.btnReset.Text = "Reset to default style";
			this.btnReset.UseVisualStyleBackColor = true;
			this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
			// 
			// chkUseSystemHighlight
			// 
			this.chkUseSystemHighlight.AutoSize = true;
			this.chkUseSystemHighlight.Location = new System.Drawing.Point(22, 92);
			this.chkUseSystemHighlight.Name = "chkUseSystemHighlight";
			this.chkUseSystemHighlight.Size = new System.Drawing.Size(153, 17);
			this.chkUseSystemHighlight.TabIndex = 9;
			this.chkUseSystemHighlight.Text = "Use System Highlight Color";
			this.chkUseSystemHighlight.UseVisualStyleBackColor = true;
			this.chkUseSystemHighlight.CheckedChanged += new System.EventHandler(this.chkUseSystemHighlight_CheckedChanged);
			// 
			// highlightColorPickerControl
			// 
			this.highlightColorPickerControl.Location = new System.Drawing.Point(185, 41);
			this.highlightColorPickerControl.Name = "highlightColorPickerControl";
			this.highlightColorPickerControl.ShowShadow = false;
			this.highlightColorPickerControl.Size = new System.Drawing.Size(40, 25);
			this.highlightColorPickerControl.SolidColor = System.Drawing.Color.YellowGreen;
			this.highlightColorPickerControl.TabIndex = 8;
			this.highlightColorPickerControl.Text = "colorPickerControl3";
			this.highlightColorPickerControl.ColorPicked += new System.EventHandler(this.highlightColorPickerControl_ColorPicked);
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(123, 47);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(54, 13);
			this.label3.TabIndex = 7;
			this.label3.Text = "Highlight: ";
			// 
			// mainColorPickerControl
			// 
			this.mainColorPickerControl.Location = new System.Drawing.Point(64, 41);
			this.mainColorPickerControl.Name = "mainColorPickerControl";
			this.mainColorPickerControl.ShowShadow = false;
			this.mainColorPickerControl.Size = new System.Drawing.Size(40, 25);
			this.mainColorPickerControl.SolidColor = System.Drawing.Color.Silver;
			this.mainColorPickerControl.TabIndex = 6;
			this.mainColorPickerControl.Text = "colorPickerControl2";
			this.mainColorPickerControl.ColorPicked += new System.EventHandler(this.mainColorPickerControl_ColorPicked);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(20, 47);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(36, 13);
			this.label2.TabIndex = 5;
			this.label2.Text = "Main: ";
			// 
			// groupBox3
			// 
			this.groupBox3.Controls.Add(this.label5);
			this.groupBox3.Controls.Add(this.btnExport);
			this.groupBox3.Location = new System.Drawing.Point(12, 261);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(642, 92);
			this.groupBox3.TabIndex = 8;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "Share";
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Font = new System.Drawing.Font("MS UI Gothic", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.label5.ForeColor = System.Drawing.Color.Green;
			this.label5.Location = new System.Drawing.Point(28, 39);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(378, 19);
			this.label5.TabIndex = 8;
			this.label5.Text = "Export this appearance style as C# code";
			// 
			// ControlAppearanceEditorForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(679, 363);
			this.Controls.Add(this.groupBox3);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.groupBox2);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ControlAppearanceEditorForm";
			this.Text = "Control Appearance Editor";
			this.groupBox2.ResumeLayout(false);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.groupBox3.ResumeLayout(false);
			this.groupBox3.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ListBox lstColors;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.CheckBox chkUseSystemHighlight;
		private unvell.UIControls.ColorPickerControl highlightColorPickerControl;
		private System.Windows.Forms.Label label3;
		private unvell.UIControls.ColorPickerControl mainColorPickerControl;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Button btnExport;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Button btnReset;
		private unvell.UIControls.ColorComboBox colorPickerControl;
	}
}