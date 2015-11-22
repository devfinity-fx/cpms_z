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
using System.Windows.Forms;

namespace unvell.ReoGrid.TestCases
{
	[TestSet(DebugEnabled = true, ReleaseEnabled = true)]
	class CellDataTest : ReoGridTestSet
	{
		private class MyData 
		{
			public override string ToString()
			{
				return "mydata";
			}
		}

		[TestCase]
		public void SetData()
		{
			SetUp();

			var dt = new DateTime(2013, 12, 12, 1, 2, 3);
			var sb = new StringBuilder("hello world");
			var mydata = new MyData();

			var objs = new object[,]{
				{1, 2, 3},
				{"a", "b", "c"},
				{'a', 'b', 'c'},
				{dt, sb, mydata},
			};

			grid[0, 0] = objs;

			AssertEquals(grid[0, 0], 1);
			AssertEquals(grid[0, 1], 2);
			AssertEquals(grid[0, 2], 3);
			AssertEquals(grid[1, 0], "a");
			AssertEquals(grid[1, 1], "b");
			AssertEquals(grid[1, 2], "c");
			AssertEquals(grid[2, 0], 'a');
			AssertEquals(grid[2, 1], 'b');
			AssertEquals(grid[2, 2], 'c');
			AssertEquals(grid[3, 0], dt);
			AssertEquals(grid[3, 1], sb);
			AssertEquals(grid[3, 2], mydata);
			AssertEquals(grid.GetCellText(3, 2), "mydata");

			grid[1, 1] = 12345;
			AssertEquals(grid[1, 1], 12345);
		}

		[TestCase]
		public void DisplayText()
		{
			grid[5, 0] = new object[,] { { 10, "a", 'b' } };

			AssertEquals(grid.GetCellText(5, 0), "10");
			AssertEquals(grid.GetCellText(5, 1), "a");
			AssertEquals(grid.GetCellText(5, 2), "b");
		}

		[TestCase]
		public void SetRange()
		{
			grid[10, 0] = new object[,] 
			{ 
				{ 10, null, null, null, 'b' } 
			};

			AssertEquals(grid[10, 0], 10);
			AssertEquals(grid[10, 2], null);
			AssertEquals(grid[10, 4], 'b');

			grid[10, 10] = new object[,] {
				{ 'a', 10, null, 15, "bbb", System.Drawing.Color.Black }
			};

			AssertEquals(grid[10, 10], 'a');
			AssertEquals(grid[10, 11], 10);
			AssertEquals(grid[10, 12], null);
			AssertEquals(grid[10, 13], 15);
			AssertEquals(grid[10, 14], "bbb");
			AssertEquals(grid[10, 15], System.Drawing.Color.Black);
		
		}

		[TestCase]
		public void ParseTabbedString()
		{
			grid[10, 5] = RGUtility.ParseTabbedString("A\tB\tC\t");

			AssertEquals(grid[10, 5], "A");
			AssertEquals(grid[10, 6], "B");
			AssertEquals(grid[10, 7], "C");

			grid[10, 10] = RGUtility.ParseTabbedString("A\t\tC\t");

			AssertEquals(grid[10, 10], "A");
			AssertEquals(grid[10, 11], "");
			AssertEquals(grid[10, 12], "C");
		
			grid[10, 15] = RGUtility.ParseTabbedString("A\nB\nC\n");

			AssertEquals(grid[10, 15], "A");
			AssertEquals(grid[11, 15], "B");
			AssertEquals(grid[12, 15], "C");
		
			grid[10, 20] = RGUtility.ParseTabbedString("1\t\t3\n\t\nA\t\tC");

			AssertEquals(grid.GetCellText(10, 20), "1");
			AssertEquals(grid.GetCellText(10, 22), "3");
			AssertEquals(grid.GetCellText(11, 20), "");
			AssertEquals(grid.GetCellText(11, 22), "");
			AssertEquals(grid.GetCellText(12, 20), "A");
			AssertEquals(grid.GetCellText(12, 22), "C");
		}

		[TestCase(true)]
		public void TestMaxBounds()
		{
			SetUp(20, 20);

			grid[10, 10] = "A";
			AssertEquals(grid.MaxContentRow, 10);
			AssertEquals(grid.MaxContentCol, 10);

			grid.DeleteCols(5, 2);
			AssertEquals(grid.MaxContentRow, 10);
			AssertEquals(grid.MaxContentCol, 10);

			grid.InsertCols(5, 2);
			AssertEquals(grid.MaxContentRow, 10);
			AssertEquals(grid.MaxContentCol, 10);

			grid[15, 15] = "B";
			AssertEquals(grid.MaxContentRow, 15);
			AssertEquals(grid.MaxContentCol, 15);
		
			grid[18, 18] = null;
			AssertEquals(grid.MaxContentRow, 15);
			AssertEquals(grid.MaxContentCol, 15);
			
			grid[15, 15] = null;
			AssertEquals(grid.MaxContentRow, 15);
			AssertEquals(grid.MaxContentCol, 15);
		}

		[TestCase]
		public void UpdateWithoutFormatAndFormula()
		{
			SetUp(20, 20);

			grid.SetSettings(ReoGridSettings.Edit_AutoFormatCell, false);

			grid[0, 0] = "10";
			AssertEquals(grid.GetCellText(0, 0), "10");

			grid[0, 1] = "'20";
			AssertEquals(grid.GetCellText(0, 1), "20");
		
			grid[0, 2] = "'=30";
			AssertEquals(grid.GetCellText(0, 2), "=30");
		}

		[TestCase]
		public void SetToInvalidCell()
		{
			SetUp(20, 20);

			grid.MergeRange(0, 0, 5, 5);
			grid[0, 0] = "valid";
			grid[1, 0] = "invalid"; // nothing to set into invalid cell

			AssertEquals(grid[0, 0], "valid");
			AssertEquals(grid[1, 0], null); // null since nothing to set into invalid cell
		}

		[TestCase]
		public void SetAnythingToNull()
		{
			grid[10, 10] = "hello";
			grid[10, 10] = null;

			AssertEquals(grid[10, 10], null);
		}
	}

}
