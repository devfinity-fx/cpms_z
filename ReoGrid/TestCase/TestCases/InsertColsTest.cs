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

#pragma warning disable 612, 618

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

using unvell.ReoGrid;

namespace unvell.ReoGrid.TestCases
{
	[TestSet(DebugEnabled = true, ReleaseEnabled = true)]
	class InsertColTestCases : ReoGridTestSet 
	{
		[TestCase]
		public void NormalInsert()
		{
			SetUp();

			// resize to 10 columns
			grid.SetCols(10);

			// insert at begin
			grid.InsertCol(0);
			AssertEquals(grid.ColCount, 11);

			// insert at center
			grid.InsertCol(5);
			AssertEquals(grid.ColCount, 12);

			// insert at last
			grid.InsertCol(12);
			AssertEquals(grid.ColCount, 13);
		}

		/// <summary>
		/// Insert Col in Merged Cell
		/// </summary>
		[TestCase]
		public void TestInsertColInMergedCell()
		{
			SetUp();

			grid.MergeRange(new ReoGridRange(2, 2, 3, 3));
			for (int i = 5; i > 1; i--)
			{
				grid.InsertCol(i);
				AssertTrue(grid._Debug_Validate_MergedCells());
				AssertTrue(grid._Debug_Validate_Unmerged_Range(ReoGridRange.EntireRange));
			}
		}

		/// <summary>
		/// Insert Col in Merged Cell (Multi-Cells)
		/// </summary>
		[TestCase]
		public void TestInsertColInMergedCell2()
		{
			SetUp();

			grid.MergeRange(new ReoGridRange(2, 3, 3, 3));
			grid.MergeRange(new ReoGridRange(5, 1, 3, 3));
			grid.MergeRange(new ReoGridRange(5, 4, 3, 3));
			grid.MergeRange(new ReoGridRange(8, 2, 3, 4));

			for (int i = 7; i > 4; i--)
			{
				grid.InsertCol(i);
				AssertTrue(grid._Debug_Validate_MergedCells());
				AssertTrue(grid._Debug_Validate_Unmerged_Range(ReoGridRange.EntireRange));
			}
		}

		[TestCase]
		public void TestBorderSpan()
		{
			SetUp();

			var range = new ReoGridRange(2, 2, 2, 5);
			grid.MergeRange(range);
			grid.SetRangeBorder(range, ReoGridBorderPos.Outline, ReoGridBorderStyle.SolidBlack);

			grid.InsertCols(4, 2);

			AssertTrue(grid._Debug_Validate_All());
		}
	}
}
