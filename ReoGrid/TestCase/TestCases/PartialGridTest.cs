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
using System.Windows.Forms;

namespace unvell.ReoGrid.TestCases
{
	[TestSet(DebugEnabled = true, ReleaseEnabled = true)]
	class PartialGridTest : ReoGridTestSet
	{
		[TestCase]
		public void CopyData()
		{
			var objs = new object[,]{
				{1,2,3},
				{5,6,7},
				{"a","b","c"},
				{'a','b','c'},
			};

			grid[0, 0] = objs;

			PartialGrid subgrid = grid.GetPartialGrid(new ReoGridRange(0, 0, 4, 3));

			AssertEquals(subgrid.Cells[0, 0].Data, 1);
			AssertEquals(subgrid.Cells[0, 2].Data, 3);
			AssertEquals(subgrid.Cells[3, 0].Data, 'a');
			AssertEquals(subgrid.Cells[3, 2].Data, 'c');
		}

		[TestCase]
		public void DuplicateData()
		{
			SetUp();

			var objs = new object[,]{
				{1,2,3},
				{5,6,7},
				{"a","b","c"},
				{'a','b','c'},
			};

			grid[0, 0] = objs;

			var range = new ReoGridRange(0, 0, 4, 3);
			PartialGrid subgrid = grid.GetPartialGrid(range);

			grid[0, 5] = subgrid;

			for (int r = range.Row; r < range.Row2; r++)
			{
				for (int c = range.Col; c <= range.Col2; c++)
				{
					AssertEquals(grid.GetCellText(r, c), grid.GetCellText(r, c + 5));
				}
			}
		}

		[TestCase]
		public void DuplicateBorder()
		{
			SetUp();

			var range = new ReoGridRange(4, 4, 4, 4);
			grid.SetRangeBorder(range, ReoGridBorderPos.All, ReoGridBorderStyle.SolidBlack);

			var subgrid = grid.GetPartialGrid(range);

			grid[0, 4] = subgrid; // top
			grid[4, 0] = subgrid; // left
			grid[8, 4] = subgrid; // bottom
			grid[4, 8] = subgrid; // right
			AssertTrue(grid._Debug_Validate_All());

			// top
			ReoGridRangeBorderInfo border = grid.GetRangeBorder(new ReoGridRange(0, 4, 4, 4));
			AssertEquals(border.Top.Style, BorderLineStyle.Solid);
			AssertEquals(border.Left.Style, BorderLineStyle.Solid);
			AssertEquals(border.Right.Style, BorderLineStyle.Solid);
			AssertEquals(border.Bottom.Style, BorderLineStyle.Solid);
			AssertEquals(border.NonUniformPos, ReoGridBorderPos.None); // border at all positions are same

			// left
			border = grid.GetRangeBorder(new ReoGridRange(4, 0, 4, 4));
			AssertEquals(border.Top.Style, BorderLineStyle.Solid);
			AssertEquals(border.Left.Style, BorderLineStyle.Solid);
			AssertEquals(border.Right.Style, BorderLineStyle.Solid);
			AssertEquals(border.Bottom.Style, BorderLineStyle.Solid);
			AssertEquals(border.NonUniformPos, ReoGridBorderPos.None); // border at all positions are same

			// bottom
			border = grid.GetRangeBorder(new ReoGridRange(8, 4, 4, 4));
			AssertEquals(border.Top.Style, BorderLineStyle.Solid);
			AssertEquals(border.Left.Style, BorderLineStyle.Solid);
			AssertEquals(border.Right.Style, BorderLineStyle.Solid);
			AssertEquals(border.Bottom.Style, BorderLineStyle.Solid);
			AssertEquals(border.NonUniformPos, ReoGridBorderPos.None); // border at all positions are same

			// right
			border = grid.GetRangeBorder(new ReoGridRange(4, 8, 4, 4));
			AssertEquals(border.Top.Style, BorderLineStyle.Solid);
			AssertEquals(border.Left.Style, BorderLineStyle.Solid);
			AssertEquals(border.Right.Style, BorderLineStyle.Solid);
			AssertEquals(border.Bottom.Style, BorderLineStyle.Solid);
			AssertEquals(border.NonUniformPos, ReoGridBorderPos.None); // border at all positions are same

		}

		[TestCase]
		public void DuplicateMergedCell()
		{
			SetUp();

			var range = new ReoGridRange(4, 4, 4, 4);
			grid.MergeRange(range);

			var subgrid = grid.GetPartialGrid(range);

			grid[0, 4] = subgrid; // top
			AssertTrue(grid._Debug_Validate_All());

			grid[4, 0] = subgrid; // left
			AssertTrue(grid._Debug_Validate_All());

			grid[8, 4] = subgrid; // bottom
			AssertTrue(grid._Debug_Validate_All());

			grid[4, 8] = subgrid; // right
			AssertTrue(grid._Debug_Validate_All());

			AssertTrue(grid.IsMergedCell(0, 4));
			AssertEquals(grid.GetCell(0, 4).GetColspan(), (short)4);
			AssertEquals(grid.GetCell(0, 4).GetRowspan(), (short)4);

			AssertTrue(grid.IsMergedCell(4, 0));
			AssertEquals(grid.GetCell(4, 0).GetColspan(), (short)4);
			AssertEquals(grid.GetCell(4, 0).GetRowspan(), (short)4);

			AssertTrue(grid.IsMergedCell(8, 4));
			AssertEquals(grid.GetCell(8, 4).GetColspan(), (short)4);
			AssertEquals(grid.GetCell(8, 4).GetRowspan(), (short)4);

			AssertTrue(grid.IsMergedCell(4, 8));
			AssertEquals(grid.GetCell(4, 8).GetColspan(), (short)4);
			AssertEquals(grid.GetCell(4, 8).GetRowspan(), (short)4);
		}

		[TestCase]
		public void OverrideBorder()
		{
			SetUp();

			var range = new ReoGridRange(0, 0, 12, 12);
			grid.SetRangeBorder(range, ReoGridBorderPos.All, ReoGridBorderStyle.SolidBlack);

			var subgrid = new PartialGrid(4, 4);

			grid[0, 0] = subgrid; // left-top
			grid[8, 0] = subgrid; // left-bottom
			grid[0, 8] = subgrid; // right-top
			grid[8, 8] = subgrid; // right-bottom

			AssertTrue(grid._Debug_Validate_All());

			// left-top
			var borderInfo = grid.GetRangeBorder(new ReoGridRange(0, 0, 4,4));
			AssertEquals(borderInfo.Top.Color, Color.Empty);
			// left-bottom
			borderInfo = grid.GetRangeBorder(new ReoGridRange(8, 0, 4, 4));
			AssertEquals(borderInfo.Top.Color, Color.Empty);
			// right-top
			borderInfo = grid.GetRangeBorder(new ReoGridRange(0, 8, 4, 4));
			AssertEquals(borderInfo.Top.Color, Color.Empty);
			// right-bottom
			borderInfo = grid.GetRangeBorder(new ReoGridRange(8, 8, 4, 4));
			AssertEquals(borderInfo.Top.Color, Color.Empty);
		}

		[TestCase]
		public void CopyBorderWithoutRightSide()
		{
			SetUp(20, 20);

			grid.SetRangeBorder(new ReoGridRange(2, 4, 2, 4), ReoGridBorderPos.Outline, ReoGridBorderStyle.SolidBlack);

			var pg = grid.GetPartialGrid(new ReoGridRange(2, 5, 2, 3));
			grid[2, 6] = pg;

			AssertTrue(grid._Debug_Validate_All());
		}

		[TestCase]
		public void SetPartialMergedRangeHorizontal()
		{
			SetUp(20, 20);

			// before:
			// +------------+----+
			// |            |    |
			// +------------+----+
			//
			// after:
			// +-----------------+
			// |                 |
			// +-----------------+

			// test case double row and single row

			PartialGrid pg = null;

			// double right
			grid.MergeRange(2, 2, 2, 4);
			pg = grid.GetPartialGrid(2, 4, 2, 2);
			grid[2, 4] = pg;
			AssertTrue(grid._Debug_Validate_All());
			AssertEquals(grid.GetCell(2, 2).GetColspan(), (short)4);
			AssertEquals(grid.GetCell(2, 2).GetRowspan(), (short)2);

			// single right
			grid.MergeRange(6, 2, 1, 4);
			pg = grid.GetPartialGrid(6, 4, 1, 2);
			grid[6, 4] = pg;
			AssertTrue(grid._Debug_Validate_All());
			AssertEquals(grid.GetCell(6, 2).GetColspan(), (short)4);
			AssertEquals(grid.GetCell(6, 2).GetRowspan(), (short)1);

			// double left
			grid.MergeRange(8, 2, 2, 4);
			pg = grid.GetPartialGrid(8, 2, 2, 2);
			grid[8, 2] = pg;
			AssertTrue(grid._Debug_Validate_All());
			AssertEquals(grid.GetCell(8, 2).GetColspan(), (short)4);
			AssertEquals(grid.GetCell(8, 2).GetRowspan(), (short)2);

			// single left
			grid.MergeRange(12, 2, 1, 4);
			pg = grid.GetPartialGrid(12, 2, 1, 2);
			grid[12, 2] = pg;
			AssertTrue(grid._Debug_Validate_All());
			AssertEquals(grid.GetCell(12, 2).GetColspan(), (short)4);
			AssertEquals(grid.GetCell(12, 2).GetRowspan(), (short)1);
		}

		[TestCase]
		public void SetPartialMergedRangeVertial()
		{
			SetUp(20, 20);

			PartialGrid pg = null;

			// double bottom
			grid.MergeRange(2, 2, 4, 2);
			pg = grid.GetPartialGrid(4, 2, 2, 2);
			grid[4, 2] = pg;
			AssertTrue(grid._Debug_Validate_All());
			AssertEquals(grid.GetCell(2, 2).GetColspan(), (short)2);
			AssertEquals(grid.GetCell(2, 2).GetRowspan(), (short)4);

			// single bottom
			grid.MergeRange(2, 6, 4, 1);
			pg = grid.GetPartialGrid(4, 6, 2, 1);
			grid[4, 6] = pg;
			AssertTrue(grid._Debug_Validate_All());
			AssertEquals(grid.GetCell(2, 6).GetColspan(), (short)1);
			AssertEquals(grid.GetCell(2, 6).GetRowspan(), (short)4);

			// double top
			grid.MergeRange(2, 8, 4, 2);
			pg = grid.GetPartialGrid(2, 8, 2, 2);
			grid[2, 8] = pg;
			AssertTrue(grid._Debug_Validate_All());
			AssertEquals(grid.GetCell(2, 8).GetColspan(), (short)2);
			AssertEquals(grid.GetCell(2, 8).GetRowspan(), (short)4);

			// single top
			grid.MergeRange(2, 12, 4, 1);
			pg = grid.GetPartialGrid(2, 12, 2, 1);
			grid[2, 12] = pg;
			AssertTrue(grid._Debug_Validate_All());
			AssertEquals(grid.GetCell(2, 12).GetColspan(), (short)1);
			AssertEquals(grid.GetCell(2, 12).GetRowspan(), (short)4);

		}

		//public void CopyIntoMergedCell


	}

}
