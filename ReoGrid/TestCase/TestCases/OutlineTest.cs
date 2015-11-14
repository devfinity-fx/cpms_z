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
	[TestSet]
	class OutlineTest : ReoGridTestSet
	{
		[TestCase]
		public void RowSingleOutline()
		{
			SetUp(20, 20);

			grid.AddOutline(RowOrColumn.Row, 4, 2);

			var outlines = grid.GetOutlines(RowOrColumn.Row);

			AssertEquals(outlines.Count, 2);
			AssertEquals(outlines[0].Count, 1);

			var outline = outlines[0][0];

			AssertEquals(outline.Start, 4);
			AssertEquals(outline.Count, 2);
			AssertEquals(outline.End, 6);
		}

		[TestCase]
		public void RowSeparatedOutline()
		{
			grid.AddOutline(RowOrColumn.Row, 8, 2);

			var outlines = grid.GetOutlines(RowOrColumn.Row);

			AssertEquals(outlines.Count, 2);
			AssertEquals(outlines[0].Count, 2);

			var outline = outlines[0][0];

			AssertEquals(outline.Start, 4);
			AssertEquals(outline.Count, 2);
			AssertEquals(outline.End, 6);

			var outline2 = outlines[0][1];

			AssertEquals(outline2.Start, 8);
			AssertEquals(outline2.Count, 2);
			AssertEquals(outline2.End, 10);
		}

		[TestCase]
		public void RowOverlappedOutline()
		{
			grid.AddOutline(RowOrColumn.Row, 4, 3);

			var outlines = grid.GetOutlines(RowOrColumn.Row);

			AssertEquals(outlines.Count, 3);
			AssertEquals(outlines[0].Count, 1);
			AssertEquals(outlines[1].Count, 2);

			var outline3 = outlines[0][0];

			AssertEquals(outline3.Start, 4);
			AssertEquals(outline3.Count, 3);
			AssertEquals(outline3.End, 7);

			var outline = outlines[1][0];

			AssertEquals(outline.Start, 4);
			AssertEquals(outline.Count, 2);
			AssertEquals(outline.End, 6);

			var outline2 = outlines[1][1];

			AssertEquals(outline2.Start, 8);
			AssertEquals(outline2.Count, 2);
			AssertEquals(outline2.End, 10);
		}

		[TestCase]
		public void CollapseOne()
		{
			var outline = grid.CollapseOutline(RowOrColumn.Row, 4, 2);

			AssertTrue(outline != null);
			AssertEquals(grid.GetRowHeight(4), (ushort)0);
			AssertEquals(grid.GetRowHeight(5), (ushort)0);
		}
	
		[TestCase]
		public void ExpandOne()
		{
			var outline = grid.ExpandOutline(RowOrColumn.Row, 4, 2);

			AssertTrue(outline != null);
			AssertEquals(grid.GetRowHeight(4), ReoGridControl.InitDefaultRowHeight);
			AssertEquals(grid.GetRowHeight(5), ReoGridControl.InitDefaultRowHeight);
		}

		[TestCase]
		public void RemoveOutline()
		{
			// remove first
			grid.RemoveOutline(RowOrColumn.Row, 4, 3);

			var outlines = grid.GetOutlines(RowOrColumn.Row);

			AssertEquals(outlines.Count, 2);
			AssertEquals(outlines[0].Count, 2);
			AssertEquals(outlines[1].Count, 0);

			var outline3 = outlines[0][0];

			AssertEquals(outline3.Start, 4);
			AssertEquals(outline3.Count, 2);

			var outline2 = outlines[0][1];

			AssertEquals(outline2.Start, 8);
			AssertEquals(outline2.Count, 2);

			// remove second
			grid.RemoveOutline(RowOrColumn.Row, 8, 2);

			AssertEquals(outlines.Count, 2);
			AssertEquals(outlines[0].Count, 1);
			AssertEquals(outlines[1].Count, 0);

			outline3 = outlines[0][0];
			AssertEquals(outline3.Start, 4);
			AssertEquals(outline3.Count, 2);

			// remove third
			grid.RemoveOutline(RowOrColumn.Row, 4, 2);

			AssertEquals(outlines.Count, 1);
			AssertEquals(outlines[0].Count, 0);
		}

		[TestCase]
		public void InsertMiddle()
		{
			grid.AddOutline(RowOrColumn.Row, 4, 2); // 6
			grid.AddOutline(RowOrColumn.Row, 2, 8); // 10
			grid.AddOutline(RowOrColumn.Row, 3, 4); // 7

			var outlines = grid.GetOutlines(RowOrColumn.Row);

			AssertEquals(outlines.Count, 4);
			AssertEquals(outlines[0].Count, 1);
			AssertEquals(outlines[1].Count, 1);
			AssertEquals(outlines[2].Count, 1);

			AssertEquals(outlines[0][0].Start, 2);
			AssertEquals(outlines[0][0].Count, 8);
			AssertEquals(outlines[1][0].Start, 3);
			AssertEquals(outlines[1][0].Count, 4);
			AssertEquals(outlines[2][0].Start, 4);
			AssertEquals(outlines[2][0].Count, 2);
		}

		[TestCase]
		public void RemoveMiddle()
		{
			grid.RemoveOutline(RowOrColumn.Row, 3, 4);

			var outlines = grid.GetOutlines(RowOrColumn.Row);

			AssertEquals(outlines.Count, 3);
			AssertEquals(outlines[0][0].Start, 2);
			AssertEquals(outlines[0][0].Count, 8);
			AssertEquals(outlines[1][0].Start, 4);
			AssertEquals(outlines[1][0].Count, 2);
		}

		[TestCase]
		public void ExpandOneLevelOutline()
		{
			ReoGridOutline inner = grid.CollapseOutline(RowOrColumn.Row, 4, 2);
			AssertEquals(inner.Collapsed, true);
			AssertEquals(grid.GetRowHeight(5), (ushort)0);

			ReoGridOutline outer = grid.CollapseOutline(RowOrColumn.Row, 2, 8);
			AssertEquals(outer.Collapsed, true);
			AssertEquals(grid.GetRowHeight(3), (ushort)0);
			AssertEquals(grid.GetRowHeight(5), (ushort)0);
			AssertEquals(grid.GetRowHeight(7), (ushort)0);

			outer.Expand();
			AssertEquals(outer.Collapsed, false);
			AssertEquals(grid.GetRowHeight(3), ReoGridControl.InitDefaultRowHeight);
			AssertEquals(grid.GetRowHeight(5), (ushort)0);
			AssertEquals(grid.GetRowHeight(8), ReoGridControl.InitDefaultRowHeight);

			inner.Expand();
			AssertEquals(inner.Collapsed, false);
			AssertEquals(grid.GetRowHeight(5), ReoGridControl.InitDefaultRowHeight);
		}

		[TestCase]
		public void CollapseAllInGroup()
		{
			var outlines = grid.GetOutlines(RowOrColumn.Row);

			ReoGridOutline inner = grid.GetOutline(RowOrColumn.Row, 4, 2);
			inner.Expand();
			AssertEquals(inner.Collapsed, false);
			AssertEquals(grid.GetRowHeight(5), ReoGridControl.InitDefaultRowHeight);

			AssertEquals(outlines[1].Count, 1);
			grid.AddOutline(RowOrColumn.Row, 11, 3);
			AssertEquals(outlines[1].Count, 2);

			outlines[1].CollapseAll();
			AssertEquals(outlines[1][0].Collapsed, true);
			AssertEquals(outlines[1][1].Collapsed, true);
			AssertEquals(grid.GetRowHeight(5), (ushort)0);
			AssertEquals(grid.GetRowHeight(12), (ushort)0);
		}

		[TestCase]
		public void ExpandBySetRowHeight()
		{
			grid.SetRowsHeight(2, 12, ReoGridControl.InitDefaultRowHeight);

			var outlines = grid.GetOutlines(RowOrColumn.Row);

			AssertEquals(outlines[1][0].Collapsed, false);
			AssertEquals(outlines[1][1].Collapsed, false);
			AssertEquals(grid.GetRowHeight(5), ReoGridControl.InitDefaultRowHeight);
			AssertEquals(grid.GetRowHeight(12), ReoGridControl.InitDefaultRowHeight);
		}

		[TestCase]
		public void CollapseByHideRows()
		{
			grid.HideRows(2, 12);

			var outlines = grid.GetOutlines(RowOrColumn.Row);

			AssertEquals(outlines[1][0].Collapsed, true);
			AssertEquals(outlines[1][1].Collapsed, true);
			AssertEquals(grid.GetRowHeight(5), (ushort)0);
			AssertEquals(grid.GetRowHeight(12), (ushort)0);
		}

		[TestCase]
		public void ExpandEntireGroup()
		{
			var outlines = grid.GetOutlines(RowOrColumn.Row);
			outlines[1].ExpandAll();

			AssertEquals(outlines[1][0].Collapsed, false);
			AssertEquals(outlines[1][1].Collapsed, false);
			AssertEquals(grid.GetRowHeight(5), ReoGridControl.InitDefaultRowHeight);
			AssertEquals(grid.GetRowHeight(12), ReoGridControl.InitDefaultRowHeight);
		}

		[TestCase]
		public void UpdateOutlineByRemoveRows()
		{
			SetUp(50, 50);

			for (int i = 0; i < 4; i++)
			{
				grid.AddOutline(RowOrColumn.Column, 3, i + 3);
			}

			for (int i = 0; i < 4; i++)
			{
				grid.AddOutline(RowOrColumn.Row, 3, i + 3);
			}
		}

	}

}
