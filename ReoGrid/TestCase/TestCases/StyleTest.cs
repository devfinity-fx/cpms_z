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
using System.Drawing;
using System.Linq;
using System.Text;

using unvell.ReoGrid.Actions;

namespace unvell.ReoGrid.TestCases
{
	[TestSet(DebugEnabled = true, ReleaseEnabled = true)]
	class StylesTest : ReoGridTestSet
	{
		[TestCase]
		public void SetStyleTest()
		{
			SetUp();

			grid.SetRangeStyle(new ReoGridRange(1, 1, 5, 5), new ReoGridRangeStyle()
			{
				Flag = PlainStyleFlag.FillColor,
				BackColor = Color.DarkBlue,
			});

			grid.SetRangeStyle(new ReoGridRange(2, 1, 1, 1), new ReoGridRangeStyle()
			{
				Flag = PlainStyleFlag.TextColor,
				TextColor = Color.Cyan,
			});

			grid.SetRangeStyle(new ReoGridRange(2, 2, 1, 1), new ReoGridRangeStyle()
			{
				Flag = PlainStyleFlag.HorizontalAlign,
				HAlign = ReoGridHorAlign.Right,
			});
		
			grid.SetRangeStyle(new ReoGridRange(1, 2, 1, 1), new ReoGridRangeStyle()
			{
				Flag = PlainStyleFlag.FontStyleBold,
				Bold = true,
			});

			grid.SetRangeStyle(new ReoGridRange(3, 1, 1, 1), new ReoGridRangeStyle()
			{
				Flag = PlainStyleFlag.TextWrap,
				TextWrapMode = TextWrapMode.WordBreak,
			});

			AssertHasStyle(grid.GetCellStyle(1, 1).Flag, PlainStyleFlag.FillColor);
			AssertHasStyle(grid.GetCellStyle(2, 1).Flag, PlainStyleFlag.TextColor | PlainStyleFlag.FillColor);
			AssertHasStyle(grid.GetCellStyle(2, 2).Flag, PlainStyleFlag.HorizontalAlign | PlainStyleFlag.FillColor);
			AssertHasStyle(grid.GetCellStyle(1, 2).Flag, PlainStyleFlag.FontStyleBold | PlainStyleFlag.FillColor);
			AssertHasStyle(grid.GetCellStyle(3, 1).Flag, PlainStyleFlag.TextWrap | PlainStyleFlag.FillColor);

			AssertTrue(grid.GetCellStyle(1, 1).BackColor == Color.DarkBlue);
			AssertTrue(grid.GetCellStyle(2, 1).TextColor == Color.Cyan);
			AssertTrue(grid.GetCellStyle(2, 2).HAlign == ReoGridHorAlign.Right);
			AssertTrue(grid.GetCellStyle(1, 2).Bold == true);
			AssertTrue(grid.GetCellStyle(3, 1).TextWrapMode == TextWrapMode.WordBreak);
		}

		private void AssertHasStyle(PlainStyleFlag flag, PlainStyleFlag target)
		{
			if ((flag & target) != target)
			{
				TestAssert.Failure(string.Format("expect has " + target + " but not"));
			}
		}

		[TestCase]
		public void SetStyleByActionTest()
		{
			SetUp();

			grid.DoAction(new RGSetRangeStyleAction(new ReoGridRange(1, 1, 5, 5), new ReoGridRangeStyle()
			{
				Flag = PlainStyleFlag.FillColor,
				BackColor = Color.DarkBlue,
			}));

			grid.DoAction(new RGSetRangeStyleAction(new ReoGridRange(2, 1, 1, 1), new ReoGridRangeStyle()
			{
				Flag = PlainStyleFlag.TextColor,
				TextColor = Color.Cyan,
			}));

			grid.DoAction(new RGSetRangeStyleAction(new ReoGridRange(2, 2, 1, 1), new ReoGridRangeStyle()
			{
				Flag = PlainStyleFlag.HorizontalAlign,
				HAlign = ReoGridHorAlign.Right,
			}));

			grid.DoAction(new RGSetRangeStyleAction(new ReoGridRange(1, 2, 1, 1), new ReoGridRangeStyle()
			{
				Flag = PlainStyleFlag.FontStyleBold,
				Bold = true,
			}));

			grid.DoAction(new RGSetRangeStyleAction(new ReoGridRange(3, 1, 1, 1), new ReoGridRangeStyle()
			{
				Flag = PlainStyleFlag.TextWrap,
				TextWrapMode = TextWrapMode.WordBreak,
			}));

			AssertHasStyle(grid.GetCellStyle(1, 1).Flag, PlainStyleFlag.FillColor);
			AssertHasStyle(grid.GetCellStyle(2, 1).Flag, PlainStyleFlag.TextColor | PlainStyleFlag.FillColor);
			AssertHasStyle(grid.GetCellStyle(2, 2).Flag, PlainStyleFlag.HorizontalAlign | PlainStyleFlag.FillColor);
			AssertHasStyle(grid.GetCellStyle(1, 2).Flag, PlainStyleFlag.FontStyleBold | PlainStyleFlag.FillColor);
			AssertHasStyle(grid.GetCellStyle(3, 1).Flag, PlainStyleFlag.TextWrap | PlainStyleFlag.FillColor);

			AssertTrue(grid.GetCellStyle(1, 1).BackColor == Color.DarkBlue);
			AssertTrue(grid.GetCellStyle(2, 1).TextColor == Color.Cyan);
			AssertTrue(grid.GetCellStyle(2, 2).HAlign == ReoGridHorAlign.Right);
			AssertTrue(grid.GetCellStyle(1, 2).Bold == true);
			AssertTrue(grid.GetCellStyle(3, 1).TextWrapMode == TextWrapMode.WordBreak);

			// undo all
			while (grid.CanUndo())
			{
				grid.Undo();
			}

			// get root style
			ReoGridRangeStyle rootStyle = grid.GetRangeStyle(new ReoGridRange(0, 0, 1, 1));

			// compare to root style
			AssertTrue(grid.GetCellStyle(1, 1).Flag == rootStyle.Flag);
			AssertTrue(grid.GetCellStyle(2, 1).Flag == rootStyle.Flag);
			AssertTrue(grid.GetCellStyle(2, 2).Flag == rootStyle.Flag);
			AssertTrue(grid.GetCellStyle(1, 2).Flag == rootStyle.Flag);
			AssertTrue(grid.GetCellStyle(3, 1).Flag == rootStyle.Flag);

			AssertEquals(grid.GetCellStyle(1, 1).BackColor, rootStyle.BackColor);
			AssertEquals(grid.GetCellStyle(2, 1).TextColor, rootStyle.TextColor);
			AssertEquals(grid.GetCellStyle(2, 2).HAlign, rootStyle.HAlign);
			AssertEquals(grid.GetCellStyle(1, 2).Bold, rootStyle.Bold);
			AssertEquals(grid.GetCellStyle(3, 1).TextWrapMode, rootStyle.TextWrapMode);


		}

		[TestCase]
		public void FullRowColumnAction()
		{
			SetUp(20, 20);

			grid.SetRangeStyle(new ReoGridRange(4, 4, 2, 2), new ReoGridRangeStyle
			{
				Flag = PlainStyleFlag.FillColor,
				BackColor = Color.Red
			});

			AssertEquals(grid.GetCellStyle(4, 4).BackColor, Color.Red);

			// full row
			grid.DoAction(new RGSetRangeStyleAction(new ReoGridRange(4, 0, 1, 20), new ReoGridRangeStyle
			{
				Flag = PlainStyleFlag.FillColor,
				BackColor = Color.Blue,
			}));

			for (int c = 0; c < 20; c++)
			{
				AssertEquals(grid.GetCellStyle(4, c).BackColor, Color.Blue);
			}

			grid.Undo();

			for (int c = 0; c < 20; c++)
			{
				AssertEquals(grid.GetCellStyle(4, c).BackColor, (c == 4 || c == 5) ? Color.Red : Color.Empty);
			}
			
			// ---------------------------------------------------

			// full col
			grid.DoAction(new RGSetRangeStyleAction(new ReoGridRange(0, 4, 20, 2), new ReoGridRangeStyle
			{
				Flag = PlainStyleFlag.FillColor,
				BackColor = Color.Blue,
			}));

			for (int r = 0; r < 20; r++)
			{
				AssertEquals(grid.GetCellStyle(r, 4).BackColor, Color.Blue);
				AssertEquals(grid.GetCellStyle(r, 5).BackColor, Color.Blue);
			}

			grid.Undo();

			for (int r = 0; r < 20; r++)
			{
				AssertEquals(grid.GetCellStyle(r, 4).BackColor, (r == 4 || r == 5) ? Color.Red : Color.Empty);
			}
			grid.Undo();
		}

		[TestCase]
		public void FullGridUndo()
		{
			SetUp(20, 20);

			// single cell
			grid.DoAction(new RGSetRangeStyleAction(new ReoGridRange(4, 4, 1, 1), new ReoGridRangeStyle
			{
				Flag = PlainStyleFlag.FillColor,
				BackColor = Color.Blue,
			}));

			// full column
			grid.DoAction(new RGSetRangeStyleAction(new ReoGridRange(0, 4, 20, 1), new ReoGridRangeStyle
			{
				Flag = PlainStyleFlag.FillColor,
				BackColor = Color.Red,
			}));

			// full grid
			grid.DoAction(new RGSetRangeStyleAction(new ReoGridRange(0, 0, 20, 20), new ReoGridRangeStyle
			{
				Flag = PlainStyleFlag.FillColor,
				BackColor = Color.Yellow,
			}));

			// test full grid
			AssertEquals(grid.GetCellStyle(4, 4).BackColor, Color.Yellow);
			AssertEquals(grid.GetCellStyle(5, 4).BackColor, Color.Yellow);
			AssertEquals(grid.GetCellStyle(0, 0).BackColor, Color.Yellow);
			AssertEquals(grid.GetCellStyle(0, 19).BackColor, Color.Yellow);
			AssertEquals(grid.GetCellStyle(19, 0).BackColor, Color.Yellow);
			AssertEquals(grid.GetCellStyle(19, 19).BackColor, Color.Yellow);

			grid.Undo();

			// test full column
			AssertEquals(grid.GetCellStyle(4, 4).BackColor, Color.Red);
			AssertEquals(grid.GetCellStyle(5, 4).BackColor, Color.Red);
			AssertEquals(grid.GetCellStyle(0, 4).BackColor, Color.Red);
			AssertEquals(grid.GetCellStyle(19, 4).BackColor, Color.Red);

			AssertEquals(grid.GetCellStyle(0, 0).BackColor, Color.Empty);
			AssertEquals(grid.GetCellStyle(0, 19).BackColor, Color.Empty);
			AssertEquals(grid.GetCellStyle(19, 0).BackColor, Color.Empty);
			AssertEquals(grid.GetCellStyle(19, 19).BackColor, Color.Empty);

			grid.Undo();

			// test full grid
			AssertEquals(grid.GetCellStyle(4, 4).BackColor, Color.Blue);
			AssertEquals(grid.GetCellStyle(5, 4).BackColor, Color.Empty);

			grid.Undo();

			AssertEquals(grid.GetCellStyle(4, 4).BackColor, Color.Empty);
			AssertEquals(grid.GetCellStyle(5, 4).BackColor, Color.Empty);
			AssertEquals(grid.GetCellStyle(0, 4).BackColor, Color.Empty);
			AssertEquals(grid.GetCellStyle(19, 4).BackColor, Color.Empty);

			AssertEquals(grid.GetCellStyle(0, 0).BackColor, Color.Empty);
			AssertEquals(grid.GetCellStyle(0, 19).BackColor, Color.Empty);
			AssertEquals(grid.GetCellStyle(19, 0).BackColor, Color.Empty);
			AssertEquals(grid.GetCellStyle(19, 19).BackColor, Color.Empty);

		}
	}
}
