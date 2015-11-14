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
	partial class FormatPage
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
			this.formatList = new System.Windows.Forms.ListBox();
			this.label1 = new System.Windows.Forms.Label();
			this.sampleGroup = new System.Windows.Forms.GroupBox();
			this.labSample = new System.Windows.Forms.Label();
			this.labType = new System.Windows.Forms.Label();
			this.datetimeFormatList = new System.Windows.Forms.ListBox();
			this.labLocation = new System.Windows.Forms.Label();
			this.datetimeLocationList = new System.Windows.Forms.ComboBox();
			this.label2 = new System.Windows.Forms.Label();
			this.numberDecimalPlaces = new System.Windows.Forms.NumericUpDown();
			this.label3 = new System.Windows.Forms.Label();
			this.numberPanel = new System.Windows.Forms.Panel();
			this.numberNegativeStyleList = new unvell.UIControls.ColoredListBox();
			this.chkNumberUseSeparator = new System.Windows.Forms.CheckBox();
			this.currencyPanel = new System.Windows.Forms.Panel();
			this.currencySymbolList = new System.Windows.Forms.ComboBox();
			this.label6 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.currencyNegativeStyleList = new unvell.UIControls.ColoredListBox();
			this.currencyDecimalPlaces = new System.Windows.Forms.NumericUpDown();
			this.label5 = new System.Windows.Forms.Label();
			this.datetimePanel = new System.Windows.Forms.Panel();
			this.txtDatetimeFormat = new System.Windows.Forms.TextBox();
			this.label7 = new System.Windows.Forms.Label();
			this.percentPanel = new System.Windows.Forms.Panel();
			this.percentDecimalPlaces = new System.Windows.Forms.NumericUpDown();
			this.label9 = new System.Windows.Forms.Label();
			this.sampleGroup.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.numberDecimalPlaces)).BeginInit();
			this.numberPanel.SuspendLayout();
			this.currencyPanel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.currencyDecimalPlaces)).BeginInit();
			this.datetimePanel.SuspendLayout();
			this.percentPanel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.percentDecimalPlaces)).BeginInit();
			this.SuspendLayout();
			// 
			// formatList
			// 
			this.formatList.FormattingEnabled = true;
			this.formatList.Location = new System.Drawing.Point(16, 35);
			this.formatList.Name = "formatList";
			this.formatList.ScrollAlwaysVisible = true;
			this.formatList.Size = new System.Drawing.Size(133, 277);
			this.formatList.TabIndex = 3;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(13, 13);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(52, 13);
			this.label1.TabIndex = 2;
			this.label1.Text = "Category:";
			// 
			// sampleGroup
			// 
			this.sampleGroup.Controls.Add(this.labSample);
			this.sampleGroup.Location = new System.Drawing.Point(165, 18);
			this.sampleGroup.Name = "sampleGroup";
			this.sampleGroup.Size = new System.Drawing.Size(377, 50);
			this.sampleGroup.TabIndex = 4;
			this.sampleGroup.TabStop = false;
			this.sampleGroup.Text = "Sample";
			// 
			// labSample
			// 
			this.labSample.Location = new System.Drawing.Point(15, 23);
			this.labSample.Name = "labSample";
			this.labSample.Size = new System.Drawing.Size(349, 20);
			this.labSample.TabIndex = 0;
			// 
			// labType
			// 
			this.labType.AutoSize = true;
			this.labType.Location = new System.Drawing.Point(4, 29);
			this.labType.Name = "labType";
			this.labType.Size = new System.Drawing.Size(34, 13);
			this.labType.TabIndex = 2;
			this.labType.Text = "&Type:";
			// 
			// datetimeFormatList
			// 
			this.datetimeFormatList.FormattingEnabled = true;
			this.datetimeFormatList.Location = new System.Drawing.Point(6, 46);
			this.datetimeFormatList.Name = "datetimeFormatList";
			this.datetimeFormatList.ScrollAlwaysVisible = true;
			this.datetimeFormatList.Size = new System.Drawing.Size(377, 134);
			this.datetimeFormatList.TabIndex = 3;
			// 
			// labLocation
			// 
			this.labLocation.AutoSize = true;
			this.labLocation.Location = new System.Drawing.Point(3, 189);
			this.labLocation.Name = "labLocation";
			this.labLocation.Size = new System.Drawing.Size(88, 13);
			this.labLocation.TabIndex = 2;
			this.labLocation.Text = "&Locale (location):";
			// 
			// datetimeLocationList
			// 
			this.datetimeLocationList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.datetimeLocationList.FormattingEnabled = true;
			this.datetimeLocationList.Location = new System.Drawing.Point(6, 206);
			this.datetimeLocationList.Name = "datetimeLocationList";
			this.datetimeLocationList.Size = new System.Drawing.Size(377, 21);
			this.datetimeLocationList.TabIndex = 5;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(3, 5);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(83, 13);
			this.label2.TabIndex = 6;
			this.label2.Text = "&Decimal Places:";
			// 
			// numberDecimalPlaces
			// 
			this.numberDecimalPlaces.Location = new System.Drawing.Point(92, 3);
			this.numberDecimalPlaces.Maximum = new decimal(new int[] {
            30,
            0,
            0,
            0});
			this.numberDecimalPlaces.Name = "numberDecimalPlaces";
			this.numberDecimalPlaces.Size = new System.Drawing.Size(59, 20);
			this.numberDecimalPlaces.TabIndex = 7;
			this.numberDecimalPlaces.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(3, 59);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(96, 13);
			this.label3.TabIndex = 6;
			this.label3.Text = "&Negative numbers:";
			// 
			// numberPanel
			// 
			this.numberPanel.Controls.Add(this.label3);
			this.numberPanel.Controls.Add(this.numberNegativeStyleList);
			this.numberPanel.Controls.Add(this.chkNumberUseSeparator);
			this.numberPanel.Controls.Add(this.numberDecimalPlaces);
			this.numberPanel.Controls.Add(this.label2);
			this.numberPanel.Location = new System.Drawing.Point(668, 3);
			this.numberPanel.Name = "numberPanel";
			this.numberPanel.Size = new System.Drawing.Size(393, 199);
			this.numberPanel.TabIndex = 8;
			// 
			// numberNegativeStyleList
			// 
			this.numberNegativeStyleList.ColorGetter = null;
			this.numberNegativeStyleList.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
			this.numberNegativeStyleList.FormattingEnabled = true;
			this.numberNegativeStyleList.Location = new System.Drawing.Point(6, 76);
			this.numberNegativeStyleList.Name = "numberNegativeStyleList";
			this.numberNegativeStyleList.Size = new System.Drawing.Size(377, 95);
			this.numberNegativeStyleList.TabIndex = 9;
			// 
			// chkNumberUseSeparator
			// 
			this.chkNumberUseSeparator.AutoSize = true;
			this.chkNumberUseSeparator.Checked = true;
			this.chkNumberUseSeparator.CheckState = System.Windows.Forms.CheckState.Checked;
			this.chkNumberUseSeparator.Location = new System.Drawing.Point(6, 33);
			this.chkNumberUseSeparator.Name = "chkNumberUseSeparator";
			this.chkNumberUseSeparator.Size = new System.Drawing.Size(133, 17);
			this.chkNumberUseSeparator.TabIndex = 8;
			this.chkNumberUseSeparator.Text = "&Use 1000 Separator (,)";
			this.chkNumberUseSeparator.UseVisualStyleBackColor = true;
			// 
			// currencyPanel
			// 
			this.currencyPanel.Controls.Add(this.currencySymbolList);
			this.currencyPanel.Controls.Add(this.label6);
			this.currencyPanel.Controls.Add(this.label4);
			this.currencyPanel.Controls.Add(this.currencyNegativeStyleList);
			this.currencyPanel.Controls.Add(this.currencyDecimalPlaces);
			this.currencyPanel.Controls.Add(this.label5);
			this.currencyPanel.Location = new System.Drawing.Point(668, 209);
			this.currencyPanel.Name = "currencyPanel";
			this.currencyPanel.Size = new System.Drawing.Size(393, 184);
			this.currencyPanel.TabIndex = 8;
			// 
			// currencySymbolList
			// 
			this.currencySymbolList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.currencySymbolList.FormattingEnabled = true;
			this.currencySymbolList.Location = new System.Drawing.Point(53, 30);
			this.currencySymbolList.Name = "currencySymbolList";
			this.currencySymbolList.Size = new System.Drawing.Size(330, 21);
			this.currencySymbolList.TabIndex = 11;
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(3, 33);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(44, 13);
			this.label6.TabIndex = 10;
			this.label6.Text = "&Symbol:";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(3, 59);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(96, 13);
			this.label4.TabIndex = 6;
			this.label4.Text = "&Negative numbers:";
			// 
			// currencyNegativeStyleList
			// 
			this.currencyNegativeStyleList.ColorGetter = null;
			this.currencyNegativeStyleList.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
			this.currencyNegativeStyleList.FormattingEnabled = true;
			this.currencyNegativeStyleList.Location = new System.Drawing.Point(6, 76);
			this.currencyNegativeStyleList.Name = "currencyNegativeStyleList";
			this.currencyNegativeStyleList.Size = new System.Drawing.Size(377, 95);
			this.currencyNegativeStyleList.TabIndex = 9;
			// 
			// currencyDecimalPlaces
			// 
			this.currencyDecimalPlaces.Location = new System.Drawing.Point(92, 3);
			this.currencyDecimalPlaces.Maximum = new decimal(new int[] {
            30,
            0,
            0,
            0});
			this.currencyDecimalPlaces.Name = "currencyDecimalPlaces";
			this.currencyDecimalPlaces.Size = new System.Drawing.Size(59, 20);
			this.currencyDecimalPlaces.TabIndex = 7;
			this.currencyDecimalPlaces.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(3, 5);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(83, 13);
			this.label5.TabIndex = 6;
			this.label5.Text = "&Decimal Places:";
			// 
			// datetimePanel
			// 
			this.datetimePanel.Controls.Add(this.txtDatetimeFormat);
			this.datetimePanel.Controls.Add(this.label7);
			this.datetimePanel.Controls.Add(this.datetimeFormatList);
			this.datetimePanel.Controls.Add(this.labLocation);
			this.datetimePanel.Controls.Add(this.datetimeLocationList);
			this.datetimePanel.Controls.Add(this.labType);
			this.datetimePanel.Location = new System.Drawing.Point(668, 400);
			this.datetimePanel.Name = "datetimePanel";
			this.datetimePanel.Size = new System.Drawing.Size(393, 237);
			this.datetimePanel.TabIndex = 8;
			// 
			// txtDatetimeFormat
			// 
			this.txtDatetimeFormat.Location = new System.Drawing.Point(53, 2);
			this.txtDatetimeFormat.Name = "txtDatetimeFormat";
			this.txtDatetimeFormat.Size = new System.Drawing.Size(330, 20);
			this.txtDatetimeFormat.TabIndex = 7;
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Location = new System.Drawing.Point(4, 5);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(44, 13);
			this.label7.TabIndex = 6;
			this.label7.Text = "&Pattern:";
			// 
			// percentPanel
			// 
			this.percentPanel.Controls.Add(this.percentDecimalPlaces);
			this.percentPanel.Controls.Add(this.label9);
			this.percentPanel.Location = new System.Drawing.Point(668, 644);
			this.percentPanel.Name = "percentPanel";
			this.percentPanel.Size = new System.Drawing.Size(393, 38);
			this.percentPanel.TabIndex = 8;
			// 
			// percentDecimalPlaces
			// 
			this.percentDecimalPlaces.Location = new System.Drawing.Point(92, 3);
			this.percentDecimalPlaces.Maximum = new decimal(new int[] {
            30,
            0,
            0,
            0});
			this.percentDecimalPlaces.Name = "percentDecimalPlaces";
			this.percentDecimalPlaces.Size = new System.Drawing.Size(59, 20);
			this.percentDecimalPlaces.TabIndex = 7;
			// 
			// label9
			// 
			this.label9.AutoSize = true;
			this.label9.Location = new System.Drawing.Point(3, 5);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(83, 13);
			this.label9.TabIndex = 6;
			this.label9.Text = "&Decimal Places:";
			// 
			// FormatPage
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.datetimePanel);
			this.Controls.Add(this.percentPanel);
			this.Controls.Add(this.currencyPanel);
			this.Controls.Add(this.numberPanel);
			this.Controls.Add(this.sampleGroup);
			this.Controls.Add(this.formatList);
			this.Controls.Add(this.label1);
			this.Name = "FormatPage";
			this.Size = new System.Drawing.Size(1076, 707);
			this.sampleGroup.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.numberDecimalPlaces)).EndInit();
			this.numberPanel.ResumeLayout(false);
			this.numberPanel.PerformLayout();
			this.currencyPanel.ResumeLayout(false);
			this.currencyPanel.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.currencyDecimalPlaces)).EndInit();
			this.datetimePanel.ResumeLayout(false);
			this.datetimePanel.PerformLayout();
			this.percentPanel.ResumeLayout(false);
			this.percentPanel.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.percentDecimalPlaces)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ListBox formatList;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.GroupBox sampleGroup;
		private System.Windows.Forms.Label labSample;
		private System.Windows.Forms.Label labType;
		private System.Windows.Forms.ListBox datetimeFormatList;
		private System.Windows.Forms.Label labLocation;
		private System.Windows.Forms.ComboBox datetimeLocationList;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.NumericUpDown numberDecimalPlaces;
		private System.Windows.Forms.Panel numberPanel;
		private System.Windows.Forms.CheckBox chkNumberUseSeparator;
		private ColoredListBox numberNegativeStyleList;
		private System.Windows.Forms.Panel currencyPanel;
		private System.Windows.Forms.Label label4;
		private ColoredListBox currencyNegativeStyleList;
		private System.Windows.Forms.NumericUpDown currencyDecimalPlaces;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.ComboBox currencySymbolList;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Panel datetimePanel;
		private System.Windows.Forms.Panel percentPanel;
		private System.Windows.Forms.NumericUpDown percentDecimalPlaces;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.TextBox txtDatetimeFormat;
		private System.Windows.Forms.Label label7;
	}
}
