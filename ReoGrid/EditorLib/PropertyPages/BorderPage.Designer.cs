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
	partial class BorderPropertyPage
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
			this.grpLine = new System.Windows.Forms.GroupBox();
			this.label1 = new System.Windows.Forms.Label();
			this.labColor = new System.Windows.Forms.Label();
			this.borderColorSelector = new ColorComboBox();
			this.borderStyleList = new unvell.ReoGrid.PropertyPages.BorderStyleListControl();
			this.formLine1 = new unvell.UIControls.FormLine();
			this.borderSetter = new unvell.ReoGrid.PropertyPages.BorderSetterControl();
			this.formLine2 = new unvell.UIControls.FormLine();
			this.btnBackslash = new System.Windows.Forms.Button();
			this.btnRight = new System.Windows.Forms.Button();
			this.btnCenter = new System.Windows.Forms.Button();
			this.btnLeft = new System.Windows.Forms.Button();
			this.btnSlash = new System.Windows.Forms.Button();
			this.btnBottom = new System.Windows.Forms.Button();
			this.btnMiddle = new System.Windows.Forms.Button();
			this.btnTop = new System.Windows.Forms.Button();
			this.btnInside = new System.Windows.Forms.Button();
			this.btnOutline = new System.Windows.Forms.Button();
			this.btnNone = new System.Windows.Forms.Button();
			this.grpLine.SuspendLayout();
			this.SuspendLayout();
			// 
			// grpLine
			// 
			this.grpLine.Controls.Add(this.label1);
			this.grpLine.Controls.Add(this.labColor);
			this.grpLine.Controls.Add(this.borderColorSelector);
			this.grpLine.Controls.Add(this.borderStyleList);
			this.grpLine.Location = new System.Drawing.Point(16, 12);
			this.grpLine.Name = "grpLine";
			this.grpLine.Size = new System.Drawing.Size(173, 240);
			this.grpLine.TabIndex = 1;
			this.grpLine.TabStop = false;
			this.grpLine.Text = "Line:";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(13, 25);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(33, 12);
			this.label1.TabIndex = 3;
			this.label1.Text = "Style:";
			// 
			// labColor
			// 
			this.labColor.AutoSize = true;
			this.labColor.Location = new System.Drawing.Point(13, 174);
			this.labColor.Name = "labColor";
			this.labColor.Size = new System.Drawing.Size(34, 12);
			this.labColor.TabIndex = 2;
			this.labColor.Text = "Color:";
			// 
			// borderColorSelector
			// 
			this.borderColorSelector.BackColor = System.Drawing.SystemColors.Window;
			this.borderColorSelector.SolidColor = System.Drawing.Color.Black;
			this.borderColorSelector.dropdown = false;
			this.borderColorSelector.Location = new System.Drawing.Point(16, 188);
			this.borderColorSelector.Name = "borderColorSelector";
			this.borderColorSelector.Size = new System.Drawing.Size(137, 21);
			this.borderColorSelector.TabIndex = 1;
			this.borderColorSelector.Text = "colorComboBox1";
			// 
			// borderStyleList
			// 
			this.borderStyleList.BackColor = System.Drawing.SystemColors.Window;
			this.borderStyleList.BorderColor = System.Drawing.Color.Black;
			this.borderStyleList.BorderStyle = unvell.ReoGrid.BorderLineStyle.None;
			this.borderStyleList.Location = new System.Drawing.Point(16, 43);
			this.borderStyleList.Name = "borderStyleList";
			this.borderStyleList.SelectedBorderStyle = unvell.ReoGrid.BorderLineStyle.Solid;
			this.borderStyleList.Size = new System.Drawing.Size(137, 111);
			this.borderStyleList.TabIndex = 0;
			this.borderStyleList.Text = "borderStyleList1";
			// 
			// formLine1
			// 
			this.formLine1.BackColor = System.Drawing.Color.Transparent;
			this.formLine1.LineColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
			this.formLine1.Location = new System.Drawing.Point(195, 90);
			this.formLine1.Name = "formLine1";
			this.formLine1.Show3DLine = false;
			this.formLine1.Size = new System.Drawing.Size(299, 21);
			this.formLine1.TabIndex = 3;
			this.formLine1.Text = "Border";
			this.formLine1.TextPadding = 8;
			// 
			// borderSetter
			// 
			this.borderSetter.BackColor = System.Drawing.SystemColors.Window;
			this.borderSetter.Cols = 2;
			this.borderSetter.CurrentBorderStlye = unvell.ReoGrid.BorderLineStyle.None;
			this.borderSetter.CurrentColor = System.Drawing.Color.Empty;
			this.borderSetter.Location = new System.Drawing.Point(250, 124);
			this.borderSetter.MixBorders = unvell.ReoGrid.ReoGridBorderPos.None;
			this.borderSetter.Name = "borderSetter";
			this.borderSetter.Padding = new System.Windows.Forms.Padding(3);
			this.borderSetter.Rows = 2;
			this.borderSetter.Size = new System.Drawing.Size(180, 100);
			this.borderSetter.TabIndex = 2;
			this.borderSetter.Text = "borderSetterControl1";
			// 
			// formLine2
			// 
			this.formLine2.BackColor = System.Drawing.Color.Transparent;
			this.formLine2.LineColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
			this.formLine2.Location = new System.Drawing.Point(195, 12);
			this.formLine2.Name = "formLine2";
			this.formLine2.Show3DLine = false;
			this.formLine2.Size = new System.Drawing.Size(299, 21);
			this.formLine2.TabIndex = 3;
			this.formLine2.Text = "Presets";
			this.formLine2.TextPadding = 8;
			// 
			// btnBackslash
			// 
			this.btnBackslash.Image = global::unvell.ReoGrid.Editor.Properties.Resources.slash_left_solid;
			this.btnBackslash.Location = new System.Drawing.Point(441, 230);
			this.btnBackslash.Name = "btnBackslash";
			this.btnBackslash.Size = new System.Drawing.Size(26, 22);
			this.btnBackslash.TabIndex = 14;
			this.btnBackslash.UseVisualStyleBackColor = true;
			this.btnBackslash.Click += new System.EventHandler(this.btnBackslash_Click);
			// 
			// btnRight
			// 
			this.btnRight.Image = global::unvell.ReoGrid.Editor.Properties.Resources.right_line_solid;
			this.btnRight.Location = new System.Drawing.Point(404, 230);
			this.btnRight.Name = "btnRight";
			this.btnRight.Size = new System.Drawing.Size(26, 22);
			this.btnRight.TabIndex = 13;
			this.btnRight.UseVisualStyleBackColor = true;
			this.btnRight.Click += new System.EventHandler(this.btnRight_Click);
			// 
			// btnCenter
			// 
			this.btnCenter.Image = global::unvell.ReoGrid.Editor.Properties.Resources.middle_line_solid;
			this.btnCenter.Location = new System.Drawing.Point(328, 230);
			this.btnCenter.Name = "btnCenter";
			this.btnCenter.Size = new System.Drawing.Size(26, 22);
			this.btnCenter.TabIndex = 12;
			this.btnCenter.UseVisualStyleBackColor = true;
			this.btnCenter.Click += new System.EventHandler(this.btnCenter_Click);
			// 
			// btnLeft
			// 
			this.btnLeft.Image = global::unvell.ReoGrid.Editor.Properties.Resources.left_line_solid;
			this.btnLeft.Location = new System.Drawing.Point(250, 230);
			this.btnLeft.Name = "btnLeft";
			this.btnLeft.Size = new System.Drawing.Size(26, 22);
			this.btnLeft.TabIndex = 11;
			this.btnLeft.UseVisualStyleBackColor = true;
			this.btnLeft.Click += new System.EventHandler(this.btnLeft_Click);
			// 
			// btnSlash
			// 
			this.btnSlash.Image = global::unvell.ReoGrid.Editor.Properties.Resources.slash_right_solid;
			this.btnSlash.Location = new System.Drawing.Point(212, 230);
			this.btnSlash.Name = "btnSlash";
			this.btnSlash.Size = new System.Drawing.Size(26, 22);
			this.btnSlash.TabIndex = 10;
			this.btnSlash.UseVisualStyleBackColor = true;
			this.btnSlash.Click += new System.EventHandler(this.btnSlash_Click);
			// 
			// btnBottom
			// 
			this.btnBottom.Image = global::unvell.ReoGrid.Editor.Properties.Resources.bottom_line_solid;
			this.btnBottom.Location = new System.Drawing.Point(212, 194);
			this.btnBottom.Name = "btnBottom";
			this.btnBottom.Size = new System.Drawing.Size(26, 22);
			this.btnBottom.TabIndex = 9;
			this.btnBottom.UseVisualStyleBackColor = true;
			this.btnBottom.Click += new System.EventHandler(this.btnBottom_Click);
			// 
			// btnMiddle
			// 
			this.btnMiddle.Image = global::unvell.ReoGrid.Editor.Properties.Resources.center_line_solid;
			this.btnMiddle.Location = new System.Drawing.Point(212, 159);
			this.btnMiddle.Name = "btnMiddle";
			this.btnMiddle.Size = new System.Drawing.Size(26, 22);
			this.btnMiddle.TabIndex = 8;
			this.btnMiddle.UseVisualStyleBackColor = true;
			this.btnMiddle.Click += new System.EventHandler(this.btnMiddle_Click);
			// 
			// btnTop
			// 
			this.btnTop.Image = global::unvell.ReoGrid.Editor.Properties.Resources.top_line_solid;
			this.btnTop.Location = new System.Drawing.Point(212, 124);
			this.btnTop.Name = "btnTop";
			this.btnTop.Size = new System.Drawing.Size(26, 22);
			this.btnTop.TabIndex = 7;
			this.btnTop.UseVisualStyleBackColor = true;
			this.btnTop.Click += new System.EventHandler(this.btnTop_Click);
			// 
			// btnInside
			// 
			this.btnInside.Image = global::unvell.ReoGrid.Editor.Properties.Resources.inside_solid_32;
			this.btnInside.Location = new System.Drawing.Point(388, 39);
			this.btnInside.Name = "btnInside";
			this.btnInside.Size = new System.Drawing.Size(42, 37);
			this.btnInside.TabIndex = 6;
			this.btnInside.UseVisualStyleBackColor = true;
			this.btnInside.Click += new System.EventHandler(this.btnInside_Click);
			// 
			// btnOutline
			// 
			this.btnOutline.Image = global::unvell.ReoGrid.Editor.Properties.Resources.outline_solid_32;
			this.btnOutline.Location = new System.Drawing.Point(319, 39);
			this.btnOutline.Name = "btnOutline";
			this.btnOutline.Size = new System.Drawing.Size(45, 37);
			this.btnOutline.TabIndex = 5;
			this.btnOutline.UseVisualStyleBackColor = true;
			this.btnOutline.Click += new System.EventHandler(this.btnOutline_Click);
			// 
			// btnNone
			// 
			this.btnNone.Image = global::unvell.ReoGrid.Editor.Properties.Resources.none_border_32;
			this.btnNone.Location = new System.Drawing.Point(250, 39);
			this.btnNone.Name = "btnNone";
			this.btnNone.Size = new System.Drawing.Size(45, 37);
			this.btnNone.TabIndex = 4;
			this.btnNone.UseVisualStyleBackColor = true;
			this.btnNone.Click += new System.EventHandler(this.btnNone_Click);
			// 
			// BorderPropertyPage
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.btnBackslash);
			this.Controls.Add(this.btnRight);
			this.Controls.Add(this.btnCenter);
			this.Controls.Add(this.btnLeft);
			this.Controls.Add(this.btnSlash);
			this.Controls.Add(this.btnBottom);
			this.Controls.Add(this.btnMiddle);
			this.Controls.Add(this.btnTop);
			this.Controls.Add(this.btnInside);
			this.Controls.Add(this.btnOutline);
			this.Controls.Add(this.btnNone);
			this.Controls.Add(this.formLine2);
			this.Controls.Add(this.formLine1);
			this.Controls.Add(this.borderSetter);
			this.Controls.Add(this.grpLine);
			this.Name = "BorderPropertyPage";
			this.Size = new System.Drawing.Size(569, 338);
			this.grpLine.ResumeLayout(false);
			this.grpLine.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private BorderStyleListControl borderStyleList;
		private System.Windows.Forms.GroupBox grpLine;
		private ColorComboBox borderColorSelector;
		private System.Windows.Forms.Label labColor;
		private System.Windows.Forms.Label label1;
		private BorderSetterControl borderSetter;
		private unvell.UIControls.FormLine formLine1;
		private unvell.UIControls.FormLine formLine2;
		private System.Windows.Forms.Button btnNone;
		private System.Windows.Forms.Button btnOutline;
		private System.Windows.Forms.Button btnInside;
		private System.Windows.Forms.Button btnTop;
		private System.Windows.Forms.Button btnMiddle;
		private System.Windows.Forms.Button btnBottom;
		private System.Windows.Forms.Button btnSlash;
		private System.Windows.Forms.Button btnLeft;
		private System.Windows.Forms.Button btnCenter;
		private System.Windows.Forms.Button btnRight;
		private System.Windows.Forms.Button btnBackslash;
	}
}
