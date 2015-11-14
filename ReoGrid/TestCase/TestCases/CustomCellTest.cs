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
	class TestCellBody : CellBody
	{
		public bool initialized = false;

		public override void OnSetup(ReoGridControl ctrl, ReoGridCell cell)
		{
			// set flag to specify that OnSetup method has been called actually
			this.initialized = true;
		}

		public override object OnSetData(object data)
		{
			// test for returning an edited data
			return "(" + data + ")";
		}

		public bool editRequired = false;

		public override bool OnStartEdit(ReoGridCell cell)
		{
			// set flag to specify that editing operation has been called actually
			editRequired = true;

			// return false to stop editing operation
			return false;
		}
	}

	[TestSet]
	class CustomCellTest : ReoGridTestSet
	{
		[TestCase]
		void SetAndRemove()
		{
			SetUp();

			// create customize cell body for test
			var testCell = new TestCellBody();

			// apply this body to cell 1,1
			grid[1, 1] = testCell;

			// check the cell body does exist inside cell
			AssertEquals(grid.GetCell(1, 1).Body, testCell);

			// check flag to test whether OnSetup method has been called actually
			AssertEquals(testCell.initialized, true);

			// remove the cell body
			grid.RemoveCellBody(1, 1);

			// check body that must be not there
			AssertEquals(grid.GetCell(1, 1).Body, null);
		}

		[TestCase]
		void DataEdit()
		{
			var testCell = new TestCellBody();
			grid[1, 2] = testCell;

			// set data, this should triggers the OnSetData method of body
			grid[1, 2] = "abc";

			// check data that has been edited by cell body atucally
			AssertEquals(grid[1, 2], "(abc)");
		}

		[TestCase]
		void StopEdit()
		{
			var testCell = new TestCellBody();

			grid[1, 3] = testCell;
			grid.StartEdit(1, 3);

			// grid must be not in editing mode
			AssertEquals(grid.IsEditing, false);

			// edit-required flag must be set to true
			AssertEquals(testCell.editRequired, true);

		}
	}

}
