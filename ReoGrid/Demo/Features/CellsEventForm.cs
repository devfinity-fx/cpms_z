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

namespace unvell.ReoGrid.Demo.Features
{
	public partial class CellsEventForm : Form
	{
		public CellsEventForm()
		{
			InitializeComponent();

			chkEnter.CheckedChanged += UpdateEventBind;
			chkLeave.CheckedChanged += UpdateEventBind;
			chkDown.CheckedChanged += UpdateEventBind;
			chkUp.CheckedChanged += UpdateEventBind;
			chkMove.CheckedChanged += UpdateEventBind;
			chkHoverHighlight.CheckedChanged += UpdateEventBind;
			chkBackground.CheckedChanged += UpdateEventBind;

			chkGridLines.CheckedChanged += (s, e) => grid.SetSettings(
				ReoGridSettings.View_ShowGridLine, chkGridLines.Checked);
			chkSelectionRect.CheckedChanged += (s, e) => grid.SelectionMode =
				chkSelectionRect.Checked ? ReoGridSelectionMode.Range : ReoGridSelectionMode.None;
		}

		private void UpdateEventBind(object sender, EventArgs e)
		{
			grid.CellMouseEnter -= Grid_CellMouseEnter;
			grid.CellMouseLeave -= Grid_CellMouseLeave;
			grid.CellMouseDown -= Grid_CellMouseDown;
			grid.CellMouseUp -= Grid_CellMouseUp;
			grid.CellMouseMove -= Grid_CellMouseMove;

			if (chkEnter.Checked || chkHoverHighlight.Checked || chkBackground.Checked) grid.CellMouseEnter += Grid_CellMouseEnter;
			if (chkLeave.Checked || chkHoverHighlight.Checked || chkBackground.Checked) grid.CellMouseLeave += Grid_CellMouseLeave;
			if (chkDown.Checked) grid.CellMouseDown += Grid_CellMouseDown;
			if (chkUp.Checked) grid.CellMouseUp += Grid_CellMouseUp;
			if (chkMove.Checked) grid.CellMouseMove += Grid_CellMouseMove;
		}

		private void Grid_CellMouseEnter(object sender, RGCellMouseEventArgs e)
		{
			if(chkEnter.Checked) Log("cell mouse enter: " + e.CellPosition);

			if (chkHoverHighlight.Checked)
			{
				grid.SetRangeBorder(new ReoGridRange(e.CellPosition, e.CellPosition), ReoGridBorderPos.Outline,
					ReoGridBorderStyle.SolidGray);
			}

			if (chkBackground.Checked)
			{
				grid.SetRangeStyle(new ReoGridRange(e.CellPosition, e.CellPosition), new ReoGridRangeStyle
				{
					Flag = PlainStyleFlag.FillColor,
					BackColor = Color.Silver,
				});
			}
		}
		private void Grid_CellMouseLeave(object sender, RGCellMouseEventArgs e)
		{
			if (chkLeave.Checked) Log("cell mouse leave: " + e.CellPosition);

			if (chkHoverHighlight.Checked)
			{
				grid.RemoveRangeBorder(new ReoGridRange(e.CellPosition, e.CellPosition), ReoGridBorderPos.Outline);
			}

			if (chkBackground.Checked)
			{
				grid.RemoveRangeStyle(new ReoGridRange(e.CellPosition, e.CellPosition), PlainStyleFlag.FillColor);
			}
		}
		private void Grid_CellMouseDown(object sender, RGCellMouseEventArgs e)
		{
			Log("cell mouse down: " + e.CellPosition + ", " + e.CursorPosition);
		}
		private void Grid_CellMouseUp(object sender, RGCellMouseEventArgs e)
		{
			Log("cell mouse up: " + e.CellPosition + ", " + e.CursorPosition);
		}
		private void Grid_CellMouseMove(object sender, RGCellMouseEventArgs e)
		{
			Log("cell mouse move: " + e.CellPosition + ", " + e.CursorPosition);
		}

		private void Log(string msg)
		{
			listbox1.Items.Add(msg);
			listbox1.SelectedIndex = listbox1.Items.Count - 1;
		}
	}
}
