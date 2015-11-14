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
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

using unvell.ReoGrid.Actions;

namespace unvell.ReoGrid.PropertyPages
{
	public partial class FillPage : UserControl, IPropertyPage
	{
		public FillPage()
		{
			InitializeComponent();

			patternColorComboBox.SolidColor = Color.Black;

			labSample.Paint += new PaintEventHandler(labSample_Paint);

			patternColorComboBox.ColorSelected += (s, e) => labSample.Invalidate();
			patternStyleComboBox.PatternSelected += (s, e) => labSample.Invalidate();
			colorPanel.ColorPicked += (s, e) => labSample.Invalidate();
		}

		void labSample_Paint(object sender, PaintEventArgs e)
		{
			if (!patternStyleComboBox.HasPatternStyle)
			{
				using (Brush b = new SolidBrush(colorPanel.SolidColor))
				{
					e.Graphics.FillRectangle(b, labSample.ClientRectangle);
				}
			}
			else
			{
				using (HatchBrush hb = new HatchBrush(patternStyleComboBox.PatternStyle,
					patternColorComboBox.SolidColor, colorPanel.SolidColor))
				{
					e.Graphics.FillRectangle(hb, labSample.ClientRectangle);
				}
			}
		}

		private ReoGridControl grid;

		public ReoGridControl Grid
		{
			get { return this.grid; }
			set { this.grid = value; }
		}

		private bool backuphasPatternStyle = false;
		private Color backupBackColor = Color.Empty;
		private Color backupPatternColor = Color.Empty;
		private HatchStyle backupPatternStyle = HatchStyle.Min;

		public void LoadPage()
		{
			ReoGridRangeStyle style = grid.GetRangeStyle(grid.SelectionRange);
			colorPanel.SolidColor = backupBackColor = style.BackColor;

			if (ReoGridStyleUtility.HasStyle(style, PlainStyleFlag.FillPattern))
			{
				patternColorComboBox.SolidColor = style.FillPatternColor;
				patternStyleComboBox.PatternStyle = style.FillPatternStyle;
				patternStyleComboBox.HasPatternStyle = true;
			}

			backupPatternColor = patternColorComboBox.SolidColor;
			backupPatternStyle = patternStyleComboBox.PatternStyle;
			backuphasPatternStyle = patternStyleComboBox.HasPatternStyle;
		}

		public RGReusableAction CreateUpdateAction()
		{
			if (backupBackColor != colorPanel.SolidColor
				|| backuphasPatternStyle != patternStyleComboBox.HasPatternStyle
				|| backupPatternColor != patternColorComboBox.SolidColor
				|| backupPatternStyle != patternStyleComboBox.PatternStyle)
			{
				ReoGridRangeStyle style = new ReoGridRangeStyle();

				style.Flag |= PlainStyleFlag.FillColor;
				style.BackColor = colorPanel.SolidColor;

				style.Flag |= PlainStyleFlag.FillPattern;
				style.FillPatternColor = patternStyleComboBox.HasPatternStyle ? patternColorComboBox.SolidColor : Color.Empty;
				style.FillPatternStyle = patternStyleComboBox.PatternStyle;

				// pattern style need a back color
				// when pattern style setted but back color is not setted, set the backcolor to white
				if (patternStyleComboBox.HasPatternStyle && style.BackColor.IsEmpty)
				{
					style.BackColor = Color.White;
					style.Flag |= PlainStyleFlag.FillColor;
				}

				return new RGSetRangeStyleAction(grid.SelectionRange, style);
			}
			else
				return null;
		}

		public event EventHandler Done;
	}
}
