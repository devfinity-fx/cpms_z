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

namespace unvell.ReoGrid.Demo.CustomCells
{
	public partial class SlideCellForm : Form
	{
		public SlideCellForm()
		{
			InitializeComponent();

			chkShowGridLines.CheckedChanged += (s, e) => 
				grid.SetSettings(ReoGridSettings.View_ShowGridLine, chkShowGridLines.Checked);

			chkDisableSelection.CheckedChanged += (s, e) =>
				grid.SelectionMode = chkDisableSelection.Checked ?
				ReoGridSelectionMode.None : ReoGridSelectionMode.Range;
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			grid.SetColsWidth(2, 1, 100);
			grid.SetColsWidth(4, 1, 120);

			grid[4, 4] = new SlideCell();
			grid[4, 4] = 50;
			grid[4, 3] = "=E5+'%'";

			grid[7, 4] = new SlideCell();
			grid[7, 4] = 50;
			grid[7, 2] = new NumericProgressCell();
			grid[7, 2] = "=E8";
			grid[7, 3] = "=E8+'%'";

			grid[6, 2] = "bind by '=E8'";
			grid[2, 3] = "Try slide the green thumb below...";



			// link
			grid.MergeRange(12, 0, 1, 7);
			grid[11, 0] = "More info about Custom Cell:";
			grid[12, 0] = new unvell.ReoGrid.CellTypes.HyperlinkCell(
				"https://reogrid.codeplex.com/wikipage?title=Custom%20Cell&referringTitle=Documentation", true);
		}
	}

	public class SlideCell : CellBody
	{
		// hold the instance of grid control
		public ReoGridControl Grid { get; set; }

		public override void OnSetup(ReoGridControl ctrl, ReoGridCell cell)
		{
			this.Grid = ctrl;
		}

		public bool IsHover { get; set; }

		public override void OnPaint(RGDrawingContext dc)
		{
			// try getting the cell value
			int value = 0;
			int.TryParse(dc.Cell.Display, out value);

			// retrieve graphics object
			var g = dc.Graphics;

			int halfHeight = Bounds.Height / 2;
			int sliderHeight = Math.Min(Bounds.Height - 4, 20);

			// draw slide bar
			g.FillRectangle(Brushes.Gainsboro, 4, halfHeight - 3, Bounds.Width - 8, 6);

			int x = 2 + (int)Math.Round(value / 100f * (Bounds.Width - 12));

			// thumb rectangle
			Rectangle rect = new Rectangle(x, halfHeight - sliderHeight / 2, 8, sliderHeight);

			// draw slide thumb
			g.FillRectangle(IsHover ? Brushes.LimeGreen : Brushes.LightGreen, rect);

		}

		public override bool OnMouseDown(RGCellMouseEventArgs e)
		{
			UpdateValueByCursorX(e.CellPosition, e.CursorPosition.X);

			// return true to notify control that the mouse-down operation has been hanlded.
			// all operations after this will be aborted.
			return true;
		}

		public override bool OnMouseMove(RGCellMouseEventArgs e)
		{
			// requires the left button
			if (e.Buttons == System.Windows.Forms.MouseButtons.Left)
			{
				UpdateValueByCursorX(e.CellPosition, e.CursorPosition.X);
			}

			return false;
		}

		private void UpdateValueByCursorX(ReoGridPos cellPos, int x)
		{
			// calcutate value by cursor position
			int value = (int)Math.Round(x * 100f / (Bounds.Width - 2f));

			if (value < 0) value = 0;
			if (value > 100) value = 100;

			Grid.SetCellData(cellPos, value);
		}

		public override bool OnMouseEnter(RGCellMouseEventArgs e)
		{
			IsHover = true;
			return true;
		}

		public override bool OnMouseLeave(RGCellMouseEventArgs e)
		{
			IsHover = false;
			return true;		
		}

		public override bool OnStartEdit(ReoGridCell cell)
		{
			// disable editing on this cell
			return false;
		}
	}

}
