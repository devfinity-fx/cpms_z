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
	class DeleteRowsTest : ReoGridTestSet
	{
		[TestCase]
		public void SimpleDelete()
		{
			SetUp();

			// normal 
			grid.SetRows(10);
			AssertEquals(grid.RowCount, 10);
			grid.DeleteRows(5, 5);
			AssertEquals(grid.RowCount, 5);

			// row count overflow
			grid.SetRows(10);
			AssertEquals(grid.RowCount, 10);
			grid.DeleteRows(8, 2);
			AssertEquals(grid.RowCount, 8);
		}

		[TestCase]
		public void DeleteInMergedCell()
		{
			SetUp();

			grid.SetRows(20);

			grid.MergeRange(new ReoGridRange(1, 1, 10, 10));
			AssertTrue(grid._Debug_Validate_MergedCells());
			AssertEquals(grid.RowCount, 20);

			grid.DeleteRows(3, 3);
			AssertTrue(grid._Debug_Validate_All());
			AssertEquals(grid.RowCount, 17);

			grid.DeleteRows(4, 10);
			AssertTrue(grid._Debug_Validate_All());
			AssertEquals(grid.RowCount, 7);
		}

		[TestCase]
		public void DeleteCrossMergedCell()
		{
			SetUp(20, 20);

			grid.MergeRange(new ReoGridRange(1, 1, 10, 10));
			AssertTrue(grid._Debug_Validate_MergedCells());
			AssertEquals(grid.RowCount, 20);

			grid.DeleteRows(5, 10);
			AssertTrue(grid._Debug_Validate_All());
			AssertEquals(grid.RowCount, 10);

			AssertTrue(!grid.IsMergedCell(6, 1));
		}

		[TestCase]
		public void DeleteAboveMergedCell()
		{
			SetUp();

			grid.SetRows(20);

			grid.MergeRange(new ReoGridRange(5, 1, 10, 10));
			AssertTrue(grid._Debug_Validate_MergedCells());
			AssertEquals(grid.RowCount, 20);

			grid.DeleteRows(2, 3);
			AssertTrue(grid._Debug_Validate_All());
			AssertEquals(grid.RowCount, 17);
		}

		[TestCase]
		public void DeleteAboveMergedCell_Action()
		{
			SetUp();

			grid.SetRows(20);

			grid.MergeRange(new ReoGridRange(5, 1, 10, 10));
			AssertTrue(grid._Debug_Validate_MergedCells());
			AssertEquals(grid.RowCount, 20);

			grid.DoAction(new RGRemoveRowsAction(2,3));
			AssertTrue(grid._Debug_Validate_All());
			AssertEquals(grid.RowCount, 17);

			grid.Undo();
			AssertTrue(grid._Debug_Validate_All());
			AssertEquals(grid.RowCount, 20);
		}

		[TestCase]
		public void DeleteRangeBorder()
		{
			SetUp();

			grid.SetRangeBorder(new ReoGridRange(2, 2, 3, 5),
				ReoGridBorderPos.All, ReoGridBorderStyle.SolidBlack);
			grid.DeleteRows(3, 2);
			AssertTrue(grid._Debug_Validate_All());
		}

		[TestCase]
		public void UnionBorderSpanAfterDelete()
		{
			SetUp();

			grid.SetRangeBorder(new ReoGridRange(2, 2, 3, 3), ReoGridBorderPos.All, ReoGridBorderStyle.SolidBlack);
			grid.SetRangeBorder(new ReoGridRange(7, 2, 3, 3), ReoGridBorderPos.All, ReoGridBorderStyle.SolidBlack);
			grid.DeleteRows(5, 2);
			AssertTrue(grid._Debug_Validate_All());
		}

		[TestCase]
		public void CorssMultiMergedCells()
		{
			SetUp(20, 20);

			for (int i = 1; i < 10; i += 2)
			{
				grid.MergeRange(i, 1, 2, 10);
			}

			for (int i = 1; i < 18; i += 2)
			{
				grid.DeleteRows(1, 2);
			}

			AssertEquals(grid.RowCount, 2);
			AssertTrue(grid._Debug_Validate_All());
		}

		[TestCase]
		public void CrossMultiMergedCellUndo()
		{
			SetUp(20, 20);

			for (int i = 1; i < 10; i += 2)
			{
				grid.MergeRange(1, i, 10, 2);
			}

			for (int i = 1; i < 18; i += 2)
			{
				grid.DoAction(new RGRemoveRowsAction(1, 2));
			}

			AssertEquals(grid.RowCount, 2);
			AssertTrue(grid._Debug_Validate_All());

			while (grid.CanUndo())
			{
				grid.Undo();
			}

			AssertEquals(grid.RowCount, 20);
			AssertTrue(grid._Debug_Validate_All());
		}

		[TestCase]
		public void NoAffectOtherRanges()
		{
			SetUp(20, 10);

			grid.MergeRange(1, 2, 8, 8);
			grid.MergeRange(10, 10, 8, 5);

			grid.DoAction(new RGRemoveRowsAction(3, 5));

			AssertTrue(grid._Debug_Validate_All());
		}

		[TestCase]
		public void KeepStyleAfterDeleteMergedCell()
		{
			SetUp(20, 20);

			grid.MergeRange(3, 3, 3, 6);
			AssertEquals(grid.GetCellStyle(3, 3).BackColor, Color.Empty);

			// set style to range
			grid.DoAction(new RGSetRangeStyleAction(3, 3, 3, 6, new ReoGridRangeStyle
			{
				Flag = PlainStyleFlag.FillColor,
				BackColor = Color.Beige,
			}));

			AssertEquals(grid.GetCellStyle(3, 3).BackColor, Color.Beige);

			// remove 1 column
			grid.DoAction(new RGRemoveColumnsAction(3, 1));
			AssertEquals(grid.GetCellStyle(3, 3).BackColor, Color.Beige);

			// undo remove 1 column
			grid.Undo();
			AssertEquals(grid.GetCellStyle(3, 3).BackColor, Color.Beige);

			// remove 2 columns
			grid.DoAction(new RGRemoveColumnsAction(3, 2));
			AssertEquals(grid.GetCellStyle(3, 3).BackColor, Color.Beige);

			// undo remove 2 columns
			grid.Undo();
			AssertEquals(grid.GetCellStyle(3, 3).BackColor, Color.Beige);

			// undo to default
			grid.Undo();
			AssertEquals(grid.GetCellStyle(3, 3).BackColor, Color.Empty);
		}
	}
}
