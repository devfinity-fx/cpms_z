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
using unvell.ReoGrid.Demo.Properties;

namespace unvell.ReoGrid.Demo.CustomCells
{
	public partial class AnimationCellForm : Form
	{
		// timer to update animation frames intervally
		Timer timer = new Timer();
		
		private LoadingCell loadingCell;

		private GifImageCell gifCell;

		public AnimationCellForm()
		{
			InitializeComponent();

			// prepare timer to udpate sheet
			timer.Interval = 10;
			timer.Tick += timer_Tick;

			// change cells size
			grid.SetColsWidth(1, 5, 100);
			grid.SetRowsHeight(5, 1, 100);

			grid.SetRangeStyle(3, 1, 3, 5, new ReoGridRangeStyle
			{
				Flag = PlainStyleFlag.HorizontalAlign,
				HAlign = ReoGridHorAlign.Center,
			});

			// waiting cell
			loadingCell = new LoadingCell();
			grid[3, 1] = loadingCell;

			// gif image cell
			gifCell = new GifImageCell(Resources.loading);
			grid[5, 2] = gifCell;

			// gif image cell
			grid[5, 4] = new BlinkCell();
			grid[5, 4] = "Blink Cell";

			// note text
			grid[7, 1] = "NOTE: Too many updates during animation will affect the rendering performance.";
			grid.SetRangeStyle(7, 1, 1, 1, new ReoGridRangeStyle
			{
				Flag = PlainStyleFlag.TextColor | PlainStyleFlag.FillColor,
				BackColor = Color.Orange,
				TextColor = Color.White,
			});
			grid.MergeRange(7, 1, 1, 6);
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			timer.Start();
		}

		void timer_Tick(object sender, EventArgs e)
		{
			// update frame
			loadingCell.NextFrame();

			// sample: retrieve body from cell
			((BlinkCell)(grid.GetCell(5, 4).Body)).NextFrame();

			// repaint sheet
			grid.InvalidateSheet();
		}
	}

	class LoadingCell : CellBody
	{
		public LoadingCell()
		{
			ThumbSize = 30;
			StepSize = 1;
		}

		public void NextFrame()
		{
			if (dir > 0)
			{
				offset += StepSize;
				if (offset >= Bounds.Width - ThumbSize - StepSize) dir = -1;
			}
			else
			{
				offset -= StepSize;
				if (offset <= 0) dir = 1;
			}
		}

		private int offset = 0;
		private int dir = 1;

		public int ThumbSize { get; set; }
		public int StepSize { get; set; }

		public override void OnPaint(RGDrawingContext dc)
		{
			dc.Graphics.FillRectangle(Brushes.SkyBlue, new Rectangle(offset, 0, ThumbSize, Bounds.Height));

			// call core text draw
			dc.DrawCellText();
		}
	}

	class GifImageCell : CellBody
	{
		public Image Gif { get; set; }

		public GifImageCell(Image gif)
		{
			this.Gif = gif;
		}

		public override void OnSetup(ReoGridControl ctrl, ReoGridCell cell)
		{
			ImageAnimator.Animate(Gif, OnFrameChanged);
		}

		private void OnFrameChanged(object o, EventArgs e)
		{
			lock (Gif) ImageAnimator.UpdateFrames(Gif);
		}

		public override void OnPaint(RGDrawingContext dc)
		{
			lock (Gif) dc.Graphics.DrawImage(Gif, Bounds);

			// call core text draw
			dc.DrawCellText();
		}
	}

	class BlinkCell : CellBody
	{
		public BlinkCell()
		{
			StepSize = 2;
		}

		public void NextFrame()
		{
			if (dir > 0)
			{
				alpha += StepSize;
				if (alpha >= 100) dir = -1;
			}
			else
			{
				alpha -= StepSize;
				if (alpha <= 0) dir = 1;
			}
		}

		private int alpha = 0;
		private int dir = 1;

		public int StepSize { get; set; }

		public override void OnPaint(RGDrawingContext dc)
		{
			using (var b = new SolidBrush(Color.FromArgb(alpha, Color.Orange)))
			{
				dc.Graphics.FillRectangle(b, Bounds);
			}

			// call core text draw
			dc.DrawCellText();
		}
	}

}
