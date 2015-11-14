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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace unvell.ReoGrid.Editor
{
	public partial class ControlAppearanceEditorForm : Form
	{
		public ControlAppearanceEditorForm()
		{
			InitializeComponent();

			lstColors.Items.AddRange(Enum.GetNames(typeof(
				ReoGridControlColors)).ToList().ConvertAll<object>(s => s).ToArray());

			lstColors.SelectedIndex = 0;

			colorPickerControl.ColorSelected += colorPickerControl_ColorPicked;
			lstColors.SelectedIndexChanged += lstColors_SelectedIndexChanged;
		}

		private ReoGridControl grid;

		public ReoGridControl Grid
		{
			get { return grid; }
			set { grid = value;
			mainColorPickerControl.SolidColor = grid.ControlStyle.GetColor(ReoGridControlColors.LeadHeadNormal);
			highlightColorPickerControl.SolidColor = Color.FromArgb(255, grid.ControlStyle.GetColor(ReoGridControlColors.SelectionBorder));
			}
		}

		private void lstColors_SelectedIndexChanged(object sender, EventArgs e)
		{
			ReoGridControlColors key = (ReoGridControlColors)Enum.Parse(
				typeof(ReoGridControlColors), lstColors.SelectedItem.ToString());

			colorPickerControl.SolidColor = grid.ControlStyle.GetColor(key);
		}

		private void colorPickerControl_ColorPicked(object sender, EventArgs e)
		{
			ReoGridControlColors key = (ReoGridControlColors)Enum.Parse(
			typeof(ReoGridControlColors), lstColors.SelectedItem.ToString());

			grid.ControlStyle.SetColor(key, colorPickerControl.SolidColor);

			grid.SetControlStyle(grid.ControlStyle);
		}

		private void btnExport_Click(object sender, EventArgs e)
		{
			StringBuilder sb = new StringBuilder("ReoGridControlStyle rgcs = new ReoGridControlStyle();");
			sb.AppendLine();

			Enum.GetNames(typeof(ReoGridControlColors)).ToList().ForEach(name =>
			{
				ReoGridControlColors key = (ReoGridControlColors)Enum.Parse(typeof(ReoGridControlColors), name);
				Color c = grid.ControlStyle.GetColor(key);
				sb.AppendLine(string.Format("rgcs[ReoGridControlColors.{0}] = Color.FromArgb({1},{2},{3},{4});", 
					name, c.A, c.R, c.G, c.B));
			});

			sb.AppendLine();

			Clipboard.SetText(sb.ToString());

			MessageBox.Show("Code exported into Clipboard.");
		}

		private void mainColorPickerControl_ColorPicked(object sender, EventArgs e)
		{
			CreateFromTheme();
		}

		private void CreateFromTheme()
		{
			grid.SetControlStyle(new ReoGridControlStyle(
				mainColorPickerControl.SolidColor.IsEmpty ? Color.White : mainColorPickerControl.SolidColor,
				highlightColorPickerControl.SolidColor.IsEmpty ? Color.White : highlightColorPickerControl.SolidColor, 
				chkUseSystemHighlight.Checked));
		}

		private void highlightColorPickerControl_ColorPicked(object sender, EventArgs e)
		{
			CreateFromTheme();
		}

		private void chkUseSystemHighlight_CheckedChanged(object sender, EventArgs e)
		{
			CreateFromTheme();
		}

		private void btnReset_Click(object sender, EventArgs e)
		{
			grid.SetControlStyle(grid.CreateDefaultControlStyle());
		}
	}
}
