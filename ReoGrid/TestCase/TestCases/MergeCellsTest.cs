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
using System.Linq;
using System.Text;

namespace unvell.ReoGrid.TestCases
{
	[TestSet(DebugEnabled = true, ReleaseEnabled = true)]
	class MergeCellsTest : ReoGridTestSet
	{
		[TestCase]
		void NormalMerge()
		{
			SetUp();

			grid.MergeRange(new ReoGridRange(2, 2, 3, 5));
			ValidateMergedCells();
			AssertTrue(grid._Debug_Validate_All());
		}

		[TestCase]
		void MergeByScript()
		{
			SetUp();

			grid.RunScript("grid.mergeRange(new Range(2,2,3,5));");
			ValidateMergedCells();
			AssertTrue(grid._Debug_Validate_All());
		}

		private void ValidateMergedCells()
		{
			AssertEquals(grid.IsMergedCell(2, 1), false);
			AssertEquals(grid.IsMergedCell(1, 2), false);
			AssertEquals(grid.IsMergedCell(2, 2), true);
			AssertEquals(grid.IsMergedCell(2, 3), false);
			AssertEquals(grid.IsMergedCell(3, 2), false);
			AssertEquals(grid.IsMergedCell(5, 7), false);

			AssertEquals(grid.IsValidCell(2, 1), true);
			AssertEquals(grid.IsValidCell(1, 2), true);
			AssertEquals(grid.IsValidCell(2, 2), true);
			AssertEquals(grid.IsValidCell(2, 3), false);
			AssertEquals(grid.IsValidCell(3, 2), false);
			AssertEquals(grid.IsValidCell(4, 6), false);
		}

		[TestCase]
		public void NormalUnmerge()
		{
			SetUp();

			grid.MergeRange(new ReoGridRange(2, 2, 3, 5));
			grid.UnmergeRange(new ReoGridRange(2, 2, 3, 5));
			ValidateUnmergedCells();
			AssertTrue(grid._Debug_Validate_All());
		}

		[TestCase]
		public void UnmergeByScript()
		{
			SetUp();

			grid.RunScript("grid.mergeRange(new Range(2,2,3,5));");
			grid.RunScript("grid.unmergeRange(new Range(2,2,3,5));");
			ValidateUnmergedCells();
			AssertTrue(grid._Debug_Validate_All());
		}

		private void ValidateUnmergedCells()
		{
			AssertEquals(grid.IsMergedCell(2, 1), false);
			AssertEquals(grid.IsMergedCell(1, 2), false);
			AssertEquals(grid.IsMergedCell(2, 2), false);
			AssertEquals(grid.IsMergedCell(2, 3), false);
			AssertEquals(grid.IsMergedCell(3, 2), false);
			AssertEquals(grid.IsMergedCell(5, 7), false);

			AssertEquals(grid.IsValidCell(2, 1), true);
			AssertEquals(grid.IsValidCell(1, 2), true);
			AssertEquals(grid.IsValidCell(2, 2), true);
			AssertEquals(grid.IsValidCell(2, 3), true);
			AssertEquals(grid.IsValidCell(3, 2), true);
			AssertEquals(grid.IsValidCell(4, 6), true);
		}

		[TestCase]
		public void TestIntersectedMerge()
		{
		}

		[TestCase]
		void RandomlyMerged()
		{
			SetUp();

			grid.Resize(20, 20);

			grid.SetRowsHeight(0, 10, 10);
			grid.SetColsWidth(0, 10, 10);

			var rand = new Random();
			for (int i = 0; i < 20; )
			{
				int row = rand.Next(16);
				int col = rand.Next(16);
				int rows = 2 + rand.Next(2);
				int cols = 2 + rand.Next(2);

				var range = new ReoGridRange(row, col, rows, cols);
				if (grid.HasIntersectedMergingRange(range))
					continue;
				else 
					i++;

				grid.MergeRange(range);
				grid._Debug_Validate_All();
			}
		}
	}
}
