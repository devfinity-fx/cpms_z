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
	[TestSet(DebugEnabled = false, ReleaseEnabled = true)]
	class InsertRowTestCases : ReoGridTestSet 
	{
		[TestCase]
		public void AppendRows()
		{
			SetUp();

			// resize to 10 columns
			int rows = grid.RowCount;
			// append 10 rows 
			grid.AppendRows(10);
			AssertEquals(grid.RowCount, rows + 10);
		}

		[TestCase]
		public void Append10000Rows()
		{
			SetUp();

			// resize to 10 columns
			int rows = grid.RowCount;
			// append 10 rows 
			grid.AppendRows(10000);
			AssertEquals(grid.RowCount, rows + 10000);
		}

		[TestCase]
		public void NormalInsert()
		{
			SetUp();

			// resize to 10 columns
			grid.SetRows(10);

			// insert at begin
			grid.InsertRow(0);
			AssertEquals(grid.RowCount, 11);

			// insert at center
			grid.InsertRow(5);
			AssertEquals(grid.RowCount, 12);

			// insert at last
			grid.InsertRow(12);
			AssertEquals(grid.RowCount, 13);
		
			// insert at last
			grid.InsertRow(1);
			AssertEquals(grid.RowCount, 14);
		}

		[TestCase]
		public void NormalInserts()
		{
			SetUp();

			// resize to 10 columns
			grid.SetRows(10);

			// insert at begin
			grid.InsertRows(0, 3);
			AssertEquals(grid.RowCount, 13);

			// insert at center
			grid.InsertRows(5, 3);
			AssertEquals(grid.RowCount, 16);

			// insert at last
			grid.InsertRows(16, 3);
			AssertEquals(grid.RowCount, 19);
		
			// insert before modified rows
			grid.InsertRows(1, 3);
			AssertEquals(grid.RowCount, 22);
		}

		/// <summary>
		///  O
		/// OO
		/// O
		/// </summary>
		[TestCase]
		public void TestInsertRow1()
		{
			SetUp();

			grid.SetRangeBorder(new ReoGridRange(4, 6, 3, 1), ReoGridBorderPos.All, new ReoGridBorderStyle
			{
				Color = Color.Black,
				Style = BorderLineStyle.Solid
			});

			grid.SetRangeBorder(new ReoGridRange(2, 7, 3, 1), ReoGridBorderPos.All, new ReoGridBorderStyle
			{
				Color = Color.Black,
				Style = BorderLineStyle.Solid
			});

			grid.InsertRow(5);
			AssertTrue(grid._Debug_Validate_BorderSpan());

			grid.InsertRow(3);
			AssertTrue(grid._Debug_Validate_BorderSpan());
		}

		/// <summary>
		/// O
		/// OO
		///  O
		/// </summary>
		[TestCase]
		public void TestInsertRow2()
		{
			SetUp();

			grid.SetRangeBorder(new ReoGridRange(2, 6, 3, 1), ReoGridBorderPos.All, new ReoGridBorderStyle
			{
				Color = Color.Black,
				Style = BorderLineStyle.Solid
			});

			grid.SetRangeBorder(new ReoGridRange(4, 7, 3, 1), ReoGridBorderPos.All, new ReoGridBorderStyle
			{
				Color = Color.Black,
				Style = BorderLineStyle.Solid
			});

			grid.InsertRow(5);
			AssertTrue(grid._Debug_Validate_BorderSpan());

			grid.InsertRow(3);
			AssertTrue(grid._Debug_Validate_BorderSpan());
		}

		/// <summary>
		/// O O
		/// OOO
		///  O
		/// </summary>
		[TestCase]
		public void TestInsertRow3()
		{
			SetUp();

			grid.SetRangeBorder(new ReoGridRange(2, 5, 3, 1), ReoGridBorderPos.All, new ReoGridBorderStyle
			{
				Color = Color.Black,
				Style = BorderLineStyle.Solid
			});

			grid.SetRangeBorder(new ReoGridRange(4, 6, 3, 1), ReoGridBorderPos.All, new ReoGridBorderStyle
			{
				Color = Color.Black,
				Style = BorderLineStyle.Solid
			});

			grid.SetRangeBorder(new ReoGridRange(2, 7, 3, 1), ReoGridBorderPos.All, new ReoGridBorderStyle
			{
				Color = Color.Black,
				Style = BorderLineStyle.Solid
			});

			grid.InsertRow(5);
			AssertTrue(grid._Debug_Validate_BorderSpan());
		
			grid.InsertRow(3);
			AssertTrue(grid._Debug_Validate_BorderSpan());
		}

		/// <summary>
		///  O
		/// OOO
		/// O O
		/// </summary>
		[TestCase]
		public void TestInsertRow4()
		{
			SetUp();

			grid.SetRangeBorder(new ReoGridRange(4, 5, 3, 1), ReoGridBorderPos.All, new ReoGridBorderStyle
			{
				Color = Color.Black,
				Style = BorderLineStyle.Solid
			});

			grid.SetRangeBorder(new ReoGridRange(2, 6, 3, 1), ReoGridBorderPos.All, new ReoGridBorderStyle
			{
				Color = Color.Black,
				Style = BorderLineStyle.Solid
			});

			grid.SetRangeBorder(new ReoGridRange(4, 7, 3, 1), ReoGridBorderPos.All, new ReoGridBorderStyle
			{
				Color = Color.Black,
				Style = BorderLineStyle.Solid
			});

			grid.InsertRow(5);
			AssertTrue(grid._Debug_Validate_BorderSpan());

			grid.InsertRow(3);
			AssertTrue(grid._Debug_Validate_BorderSpan());
		}

		/// <summary>
		/// O O
		/// OOO
		/// </summary>
		[TestCase]
		public void TestInsertRow5()
		{
			SetUp();

			grid.SetRangeBorder(new ReoGridRange(2, 5, 5, 1), ReoGridBorderPos.All, new ReoGridBorderStyle
			{
				Color = Color.Black,
				Style = BorderLineStyle.Solid
			});

			grid.SetRangeBorder(new ReoGridRange(4, 6, 3, 1), ReoGridBorderPos.All, new ReoGridBorderStyle
			{
				Color = Color.Black,
				Style = BorderLineStyle.Solid
			});

			grid.SetRangeBorder(new ReoGridRange(2, 7, 5, 1), ReoGridBorderPos.All, new ReoGridBorderStyle
			{
				Color = Color.Black,
				Style = BorderLineStyle.Solid
			});

			grid.InsertRow(5);
			AssertTrue(grid._Debug_Validate_BorderSpan());

			grid.InsertRow(3);
			AssertTrue(grid._Debug_Validate_BorderSpan());
		}

		/// <summary>
		/// OOO
		///  O
		/// </summary>
		[TestCase]
		public void TestInsertRow11()
		{
			SetUp();

			grid.SetRangeBorder(new ReoGridRange(2, 2, 3, 3), ReoGridBorderPos.All, new ReoGridBorderStyle
			{
				Color = Color.Black,
				Style = BorderLineStyle.Solid
			});

			grid.SetRangeBorder(new ReoGridRange(5, 3, 3, 1), ReoGridBorderPos.All, new ReoGridBorderStyle
			{
				Color = Color.Black,
				Style = BorderLineStyle.Solid
			});

			grid.InsertRow(5);
			AssertTrue(grid._Debug_Validate_BorderSpan());
		}

		/// <summary>
		/// Insert Row in Merged Cell
		/// </summary>
		[TestCase]
		public void TestInsertRowInMergedCell()
		{
			SetUp();
			grid.MergeRange(new ReoGridRange(2, 2, 3, 3));
			for (int i = 5; i > 1; i--)
			{
				grid.InsertRow(i);
				AssertTrue(grid._Debug_Validate_MergedCells());
			}
		}

		/// <summary>
		/// Insert Row in Merged Cell (Multi-Cells)
		/// </summary>
		[TestCase]
		public void TestInsertRowInMergedCell2()
		{
			SetUp();

			grid.MergeRange(new ReoGridRange(2, 2, 4, 3));
			grid.MergeRange(new ReoGridRange(1, 5, 3, 3));
			grid.MergeRange(new ReoGridRange(4, 5, 3, 3));
			grid.MergeRange(new ReoGridRange(2, 8, 3, 3));

			for (int i = 0; i > 0; i--)
			{
				grid.InsertRow(i);
				AssertTrue(grid._Debug_Validate_MergedCells());
			}
		}
	}
}
