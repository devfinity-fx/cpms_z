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
	class CellSizeTest : ReoGridTestSet
	{
		ushort zero = 0;

		[TestCase]
		void HideRowsTest()
		{
			SetUp(20, 20);

			AssertEquals(grid.GetRowHeight(2), ReoGridControl.InitDefaultRowHeight);
			AssertEquals(grid.GetRowHeight(3), ReoGridControl.InitDefaultRowHeight);

			// normal hide
			grid.HideRows(2, 2);
			AssertEquals(grid.GetRowHeight(2), zero);
			AssertEquals(grid.GetRowHeight(3), zero);
			AssertEquals(grid.GetRowHeight(4), ReoGridControl.InitDefaultRowHeight);

			// restore height of rows
			grid.ShowRows(2, 2);
			AssertEquals(grid.GetRowHeight(2), ReoGridControl.InitDefaultRowHeight);
			AssertEquals(grid.GetRowHeight(3), ReoGridControl.InitDefaultRowHeight);
			AssertEquals(grid.GetRowHeight(4), ReoGridControl.InitDefaultRowHeight);

			// repeat to show rows
			grid.ShowRows(2, 2);
			AssertEquals(grid.GetRowHeight(2), ReoGridControl.InitDefaultRowHeight);
			AssertEquals(grid.GetRowHeight(3), ReoGridControl.InitDefaultRowHeight);
			AssertEquals(grid.GetRowHeight(4), ReoGridControl.InitDefaultRowHeight);

			// show rows after adjust height
			grid.SetRowsHeight(2, 2, 30);
			grid.ShowRows(2, 2);
			AssertEquals(grid.GetRowHeight(2), (ushort)30);
			AssertEquals(grid.GetRowHeight(3), (ushort)30);
			AssertEquals(grid.GetRowHeight(4), ReoGridControl.InitDefaultRowHeight);

			// hide adjusted height
			grid.HideRows(2, 2);
			AssertEquals(grid.GetRowHeight(2), zero);
			AssertEquals(grid.GetRowHeight(3), zero);
			AssertEquals(grid.GetRowHeight(4), ReoGridControl.InitDefaultRowHeight);

			// restore adjusted height
			grid.ShowRows(2, 2);
			AssertEquals(grid.GetRowHeight(2), (ushort)30);
			AssertEquals(grid.GetRowHeight(3), (ushort)30);
			AssertEquals(grid.GetRowHeight(4), ReoGridControl.InitDefaultRowHeight);
		}

		[TestCase]
		void HideColumnsTest()
		{
			SetUp(20, 20);

			AssertEquals(grid.GetColumnWidth(2), ReoGridControl.InitDefaultColumnWidth);
			AssertEquals(grid.GetColumnWidth(3), ReoGridControl.InitDefaultColumnWidth);

			// normal hide
			grid.HideColumns(2, 2);
			AssertEquals(grid.GetColumnWidth(2), zero);
			AssertEquals(grid.GetColumnWidth(3), zero);
			AssertEquals(grid.GetColumnWidth(4), ReoGridControl.InitDefaultColumnWidth);

			// restore height of columns
			grid.ShowColumns(2, 2);
			AssertEquals(grid.GetColumnWidth(2), ReoGridControl.InitDefaultColumnWidth);
			AssertEquals(grid.GetColumnWidth(3), ReoGridControl.InitDefaultColumnWidth);
			AssertEquals(grid.GetColumnWidth(4), ReoGridControl.InitDefaultColumnWidth);

			// repeat to show columns
			grid.ShowColumns(2, 2);
			AssertEquals(grid.GetColumnWidth(2), ReoGridControl.InitDefaultColumnWidth);
			AssertEquals(grid.GetColumnWidth(3), ReoGridControl.InitDefaultColumnWidth);
			AssertEquals(grid.GetColumnWidth(4), ReoGridControl.InitDefaultColumnWidth);

			// show rows after adjust width
			grid.SetColsWidth(2, 2, 30);
			grid.ShowColumns(2, 2);
			AssertEquals(grid.GetColumnWidth(2), (ushort)30);
			AssertEquals(grid.GetColumnWidth(3), (ushort)30);
			AssertEquals(grid.GetColumnWidth(4), ReoGridControl.InitDefaultColumnWidth);

			// hide adjusted width
			grid.HideColumns(2, 2);
			AssertEquals(grid.GetColumnWidth(2), zero);
			AssertEquals(grid.GetColumnWidth(3), zero);
			AssertEquals(grid.GetColumnWidth(4), ReoGridControl.InitDefaultColumnWidth);

			// restore adjusted width
			grid.ShowColumns(2, 2);
			AssertEquals(grid.GetColumnWidth(2), (ushort)30);
			AssertEquals(grid.GetColumnWidth(3), (ushort)30);
			AssertEquals(grid.GetColumnWidth(4), ReoGridControl.InitDefaultColumnWidth);
		}

	}

}
