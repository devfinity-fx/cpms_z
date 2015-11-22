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

namespace unvell.ReoGrid.Demo.CustomCells
{
	public partial class NumericProgressForm : Form
	{
		public NumericProgressForm()
		{
			InitializeComponent();

			var rand = new Random();

			grid[1, 2] = "Try change the value below: ";

			for (int r = 3; r <= 7; r++)
			{
				// set the cell with customized body
				grid.SetCellBody(r, 2, new NumericProgressCell());

				// set formula to get data from the cell of right side
				grid[r, 2] = "=" + new ReoGridPos(r, 3).ToStringCode(); // e.g. D3

				// set data format as percent
				grid.SetRangeDataFormat(new ReoGridRange(r, 3, 1, 1), DataFormat.CellDataFormatFlag.Percent, null);

				// generate a random value
				grid[r, 3] =  rand.Next(100);
			}

			// change selection forward direction to down
			grid.SelectionForwardDirection = SelectionForwardDirection.Down;

			// put focus on cell
			grid.FocusPos = new ReoGridPos(3, 3);

			// link
			grid.MergeRange(12, 0, 1, 7);
			grid[11, 0] = "More info about Custom Cell:";
			grid[12, 0] = new HyperlinkCell(
				"https://reogrid.codeplex.com/wikipage?title=Custom%20Cell&referringTitle=Documentation", true);
		}
	}

	internal class NumericProgressCell : CellBody
	{
		public override void OnPaint(RGDrawingContext dc)
		{
			int value = 0;
			int.TryParse(dc.Cell.Display, out value);

			if (value > 0)
			{
				Graphics g = dc.Graphics;

				Rectangle rect = new Rectangle(Bounds.Left, Bounds.Top + 1,
					(int)(Math.Round(Bounds.Width / 100f * value)), Bounds.Height - 1);

				using (LinearGradientBrush lgb = new LinearGradientBrush(rect, Color.Coral, Color.IndianRed, 90f))
				{
					g.PixelOffsetMode = PixelOffsetMode.Half;
					g.FillRectangle(lgb, rect);
					g.PixelOffsetMode = PixelOffsetMode.Default;
				}
			}
		}
	}
}
