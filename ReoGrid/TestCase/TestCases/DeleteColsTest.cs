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
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

using unvell.ReoGrid.Actions;

namespace unvell.ReoGrid.TestCases
{
	[TestSet(DebugEnabled = true, ReleaseEnabled = true)]
	class DeleteColsTest : ReoGridTestSet
	{
		[TestCase]
		public void ValidColCountAfterDelete()
		{
			SetUp();

			// normal 
			grid.SetCols(7);
			AssertEquals(grid.ColCount, 7);
			grid.DeleteCols(2, 5);
			AssertEquals(grid.ColCount, 2);

			// row count overflow
			grid.SetCols(10);
			AssertEquals(grid.ColCount, 10);
			grid.DeleteCols(8, 2);
			AssertEquals(grid.ColCount, 8);
		}

		[TestCase]
		public void DeleteOnBoundary()
		{
			SetUp();

			int colcount = grid.ColCount;

			grid.DeleteCols(0, 1);
			grid.DeleteCols(grid.ColCount - 1, 1);
			grid.DeleteCols(0, 2);
			grid.DeleteCols(grid.ColCount - 2, 2);

			AssertEquals(colcount - 6, grid.ColCount);
		}

		[TestCase]
		public void DeleteInMergedCell()
		{
			SetUp();

			grid.SetCols(20);

			grid.MergeRange(new ReoGridRange(1, 1, 10, 10));
			AssertTrue(grid._Debug_Validate_MergedCells());
			AssertEquals(grid.ColCount, 20);

			grid.DeleteCols(3, 3);
			AssertTrue(grid._Debug_Validate_All());
			AssertEquals(grid.ColCount, 17);

			grid.DeleteCols(4, 10);
			AssertTrue(grid._Debug_Validate_All());
			AssertEquals(grid.ColCount, 7);
		}

		[TestCase]
		public void DeleteCorssOneMergedCell()
		{
			SetUp();

			grid.SetCols(20);
			AssertEquals(grid.ColCount, 20);

			grid.MergeRange(new ReoGridRange(1, 5, 10, 10));
			AssertTrue(grid._Debug_Validate_MergedCells());
			grid.DeleteCols(3, 5);
			AssertTrue(grid._Debug_Validate_All());
			AssertEquals(grid.ColCount, 15);

			grid.DeleteCols(10, 5);
			AssertTrue(grid._Debug_Validate_All());
			AssertEquals(grid.ColCount, 10);
		}

		[TestCase]
		public void DeleteLeftAndRightMergedCell()
		{
			SetUp();

			grid.SetCols(20);

			grid.MergeRange(new ReoGridRange(1, 5, 10, 10));
			grid.DeleteCols(2, 3);
			AssertTrue(grid._Debug_Validate_All());
			AssertEquals(grid.ColCount, 17);

			grid.DeleteCols(12, 3);
			AssertTrue(grid._Debug_Validate_All());
			AssertEquals(grid.ColCount, 14);
		}

		[TestCase]
		public void DeleteCorssTwoMergedCells()
		{
			SetUp();

			grid.SetCols(20);
			grid.MergeRange(new ReoGridRange(1, 2, 5, 5));
			grid.MergeRange(new ReoGridRange(1, 8, 5, 5));
		
			grid.DeleteCols(5, 5);
			AssertTrue(grid._Debug_Validate_All());
			AssertEquals(grid.ColCount, 15);

			grid.DeleteCols(2, 11);
			AssertTrue(grid._Debug_Validate_All());
			AssertEquals(grid.ColCount, 4);
		}

		[TestCase]
		public void DeleteCrossBorder()
		{
			SetUp();

			grid.SetRangeBorder(new ReoGridRange(2, 2, 3, 5), ReoGridBorderPos.All, ReoGridBorderStyle.SolidBlack);
			grid.DeleteCols(3, 2);
			AssertTrue(grid._Debug_Validate_All());
		}

		[TestCase]
		public void MergeBorderAfterDelete()
		{
			SetUp();

			grid.SetRangeBorder(new ReoGridRange(2, 2, 3, 3), ReoGridBorderPos.All, ReoGridBorderStyle.SolidBlack);
			grid.SetRangeBorder(new ReoGridRange(2, 7, 3, 3), ReoGridBorderPos.All, ReoGridBorderStyle.SolidBlack);
			grid.DeleteCols(5, 2);
			AssertTrue(grid._Debug_Validate_All());
		}

		[TestCase]
		public void DeleteHalfSideBorder()
		{
			SetUp();

			grid.SetRangeBorder(new ReoGridRange(2, 2, 3, 5), ReoGridBorderPos.All, ReoGridBorderStyle.SolidBlack);
			grid.DeleteCols(1, 3);
			AssertTrue(grid._Debug_Validate_All());

			grid.SetRangeBorder(new ReoGridRange(2, 2, 3, 5), ReoGridBorderPos.All, ReoGridBorderStyle.SolidBlack);
			grid.DeleteCols(2, 3);
			AssertTrue(grid._Debug_Validate_All());
		}

		// this case cannot be passed 100% (fail with 10%)
		//
		// Unknow what is the reason
		//
		[TestCase(false)]
		public void DeleteRandomBorder()
		{
			SetUp();
			var rand = new Random();
			for (int i = 0; i < 20; i++)
			{
				int r = rand.Next(30);
				int c = rand.Next(30);
				int rs = rand.Next(30);
				int cs = rand.Next(30);

				grid.SetRangeBorder(new ReoGridRange(r, c, rs, cs), ReoGridBorderPos.Outline, ReoGridBorderStyle.SolidBlack);
			}
			AssertTrue(grid._Debug_Validate_All());

			MemoryStream ms = new MemoryStream(40960);

			for (int i = 0; i < 20; i++)
			{
				int c = rand.Next(20);
				int cc = rand.Next(5);

				ms.Seek(0, SeekOrigin.Begin);
				ms.SetLength(0);

				TestHelper.DumpGrid(Grid, ms);

				grid.DeleteCols(c, cc);
				var rs = grid._Debug_Validate_All();

				if(!rs)
				{
					using (var fs = new FileStream("randomly-delcols-hborder-before.txt", FileMode.Create, FileAccess.Write))
					{
						byte[] buf = ms.ToArray();
						fs.Write(buf, 0, buf.Length);
					}

					Process.Start("randomly-delcols-hborder-before.txt");

					TestHelper.DumpGrid(Grid, "randomly-delcols-hborder-after.txt");
					Process.Start("randomly-delcols-hborder-after.txt");

					AssertTrue(false);
				}
			}
		}

		[TestCase]
		public void BoundsTest()
		{
			SetUp(20, 20);

			grid.SetRangeBorder(ReoGridRange.EntireRange, ReoGridBorderPos.All, ReoGridBorderStyle.SolidBlack);
			AssertTrue(grid._Debug_Validate_All());

			grid.DeleteCols(19, 1);
			AssertEquals(grid.ColCount, 19);
			AssertTrue(grid._Debug_Validate_All());
		}

		[TestCase]
		public void CorssMultiMergedCells()
		{
			SetUp(20, 20);

			for (int i = 1; i < 10; i += 2)
			{
				grid.MergeRange(i, i, 2, 10);
			}

			for (int i = 1; i < 18; i += 2)
			{
				grid.DeleteCols(1, 2);
			}

			AssertEquals(grid.ColCount, 2);
			AssertTrue(grid._Debug_Validate_All());
		}

		[TestCase]
		public void CrossMultiMergedCellUndo()
		{
			SetUp(20, 20);

			for (int i = 1; i < 10; i += 2)
			{
				grid.MergeRange(i, 1, 2, 10);
			}

			for (int i = 1; i < 18; i += 2)
			{
				grid.DoAction(new RGRemoveColumnsAction(1, 2));
			}

			AssertEquals(grid.ColCount, 2);
			AssertTrue(grid._Debug_Validate_All());

			while (grid.CanUndo())
			{
				grid.Undo();
			}

			AssertEquals(grid.ColCount, 20);
			AssertTrue(grid._Debug_Validate_All());
		}

		[TestCase]
		public void NoAffectOtherRanges()
		{
			SetUp(20, 20);

			grid.MergeRange(2, 1, 8, 8);
			grid.MergeRange(10, 10, 5, 8);

			grid.DoAction(new RGRemoveColumnsAction(5, 3));

			AssertTrue(grid._Debug_Validate_All());
		}
	}
}
