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
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using unvell.ReoGrid.CellTypes;
using unvell.ReoGrid.Demo.Properties;

namespace unvell.ReoGrid.Demo.CustomCells
{
	public partial class BuiltInTypesForm : Form
	{
		public BuiltInTypesForm()
		{
			InitializeComponent();

			// set grid cursor to windows default
			grid.SetRangeStyle(ReoGridRange.EntireRange, new ReoGridRangeStyle
			{
				Flag = PlainStyleFlag.FontName,
				FontName = "Arial",
			});

			grid.CellsSelectionCursor = Cursors.Default;
			grid.SetSettings(ReoGridSettings.View_ShowGridLine, false);
			grid.SelectionMode = ReoGridSelectionMode.Cell;
			grid.SelectionStyle = ReoGridSelectionStyle.FocusRect;

			var middleStyle = new ReoGridRangeStyle
			{
				Flag = PlainStyleFlag.Padding | PlainStyleFlag.HorizontalAlign,
				Padding = new Padding(2),
				HAlign = ReoGridHorAlign.Center,
			};

			var grayTextStyle = new ReoGridRangeStyle 
			{ 
				Flag = PlainStyleFlag.TextColor, 
				TextColor =	Color.DimGray 
			};

			grid.MergeRange(1, 1, 1, 6);

			grid.SetRangeStyle(1, 1, 1, 6, new ReoGridRangeStyle
			{
				Flag = PlainStyleFlag.TextColor | PlainStyleFlag.FontSize,
				TextColor = Color.DarkGreen,
				FontSize = 18,
			});

			grid[1, 1] = "Built-in Cell Bodies";

			grid.SetColsWidth(1, 1, 100);
			grid.SetColsWidth(2, 1, 30);
			grid.SetColsWidth(3, 1, 100);
			grid.SetColsWidth(6, 2, 65);

			// button
			grid.MergeRange(3, 2, 1, 2);
			var btn = new ButtonCell("Hello");
			grid[3, 1] = new object[] { "Button: ", btn };
			btn.Click += (s, e) => ShowText("Button clicked.");

			// link
			grid.MergeRange(5, 2, 1, 2);
			var link = new HyperlinkCell("http://www.google.com");
			grid[5, 1] = new object[] { "Hyperlink", link };
			link.Click += (s, e) => System.Diagnostics.Process.Start(grid.GetCellText(5, 2));

			// checkbox
			var checkbox = new CheckBoxCell();
			grid.SetRangeStyle(7, 2, 1, 1, middleStyle);
			grid.SetRangeStyle(8, 2, 1, 1, grayTextStyle);
			grid[7, 1] = new object[] { "Check box", checkbox, "Auto destroy after 5 minutes." };
			grid[8, 2] = "(Keyboard is also supported to change the status of control)";
			checkbox.CheckChanged += (s, e) => ShowText("Check box switch to " + checkbox.ButtonState.ToString());

			// radio & radio group
			grid[10, 1] = "Radio Button";
			grid.SetRangeStyle(10, 2, 3, 1, middleStyle);
			var radioGroup = new RadioButtonGroup();
			grid[10, 2] = new object[,] {
				{new RadioButtonCell() { RadioGroup = radioGroup }, "Apple"},
				{new RadioButtonCell() { RadioGroup = radioGroup }, "Orange"},
				{new RadioButtonCell() { RadioGroup = radioGroup }, "Banana"}
			};
			grid[13, 2] = "(By adding into RadioGroup will make it toggled automatically)";
			grid.SetRangeStyle(13, 2, 1, 1, grayTextStyle);

			// dropdown 
			grid.MergeRange(15, 2, 1, 3);
			var dropdown = new DropdownCell(new object[] { "Apple", "Orange", "Banana", "Pear", "Pumpkin", "Cherry", "Coconut" });
			grid[15, 1] = new object[] { "Dropdown", dropdown };
			grid.SetRangeBorder(15, 2, 1, 3, ReoGridBorderPos.Outline, ReoGridBorderStyle.SolidGray);

			// image
			grid.MergeRange(2, 6, 5, 2);
			grid[2, 6] = new ImageCell(Resources.computer_laptop);

			// information cell
			grid.SetRangeBorder(19, 0, 1, 10, ReoGridBorderPos.Top, ReoGridBorderStyle.SolidGray);
		}

		private void ShowText(string text)
		{
			grid[19, 0] = text;
		}

		private void chkGridlines_CheckedChanged(object sender, EventArgs e)
		{
			grid.SetSettings(ReoGridSettings.View_ShowGridLine, chkGridlines.Checked);
		}

		private void chkSelection_CheckedChanged(object sender, EventArgs e)
		{
			if (chkSelectionNone.Checked)
			{
				grid.SelectionMode = ReoGridSelectionMode.None;
			}
			else if (chkSelectionRange.Checked)
			{
				grid.SelectionMode = ReoGridSelectionMode.Range;
			}
			else if (chkSelectionCell.Checked)
			{
				grid.SelectionMode = ReoGridSelectionMode.Cell;
			}
		}

		private void rdoNormal_CheckedChanged(object sender, EventArgs e)
		{
			if (rdoFocus.Checked)
			{
				grid.SelectionStyle = ReoGridSelectionStyle.FocusRect;
			}
			else if (rdoNormal.Checked)
			{
				grid.SelectionStyle = ReoGridSelectionStyle.Default;
			}
		}
	}
}
