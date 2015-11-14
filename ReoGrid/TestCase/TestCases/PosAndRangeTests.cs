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
	class PosAndRangeTests : ReoGridTestSet
	{
		[TestCase]
		public void BasePos()
		{
			var pos1 = new ReoGridPos(0, 0);
			AssertEquals(pos1.Row, 0);
			AssertEquals(pos1.Col, 0);

			var pos2 = new ReoGridPos(10, 20);
			AssertEquals(pos2.Row, 10);
			AssertEquals(pos2.Col, 20);

			AssertTrue(pos1 != pos2);
			AssertEquals(pos2, new ReoGridPos(10, 20));
		}

		[TestCase]
		public void BaseRange()
		{
			var r1 = new ReoGridRange(0, 0, 1, 1);
			AssertEquals(r1.Row, 0);
			AssertEquals(r1.Col, 0);
			AssertEquals(r1.Row2, 0);
			AssertEquals(r1.Col2, 0);
			AssertEquals(r1.Rows, 1);
			AssertEquals(r1.Cols, 1);

			var r2 = new ReoGridRange(10, 20, 5, 5);
			AssertEquals(r2.Row, 10);
			AssertEquals(r2.Col, 20);
			AssertEquals(r2.Row2, 14);
			AssertEquals(r2.Col2, 24);
			AssertEquals(r2.Rows, 5);
			AssertEquals(r2.Cols, 5);

			AssertTrue(r1 != r2);
			AssertEquals(r2, new ReoGridRange(10, 20, 5, 5));
		}

		[TestCase]
		public void CodeFormat()
		{
			var pos1 = new ReoGridPos(0, 0);
			var pos2 = new ReoGridPos(10, 20);

			AssertEquals(pos1.ToStringCode(), "A1");
			AssertEquals(pos2.ToStringCode(), "U11");

			AssertEquals(pos1, new ReoGridPos("A1"));
			AssertEquals(pos2, new ReoGridPos("U11"));

			var r1 = new ReoGridRange(0, 0, 1, 1);
			var r2 = new ReoGridRange(0, 0, 3, 3);
			var r3 = new ReoGridRange(10, 20, 5, 5);

			AssertEquals(r1.ToStringCode(), "A1:A1");
			AssertEquals(r2.ToStringCode(), "A1:C3");
			AssertEquals(r3.ToStringCode(), "U11:Y15");

			AssertEquals(r1, new ReoGridRange("A1:A1"));
			AssertEquals(r2, new ReoGridRange("A1:C3"));
			AssertEquals(r3, new ReoGridRange("U11:Y15"));
		}

		[TestCase]
		public void CodeConvert()
		{
			AssertEquals(new ReoGridPos("Z1").ToStringCode(), "Z1");
			AssertEquals(new ReoGridPos("AA1").ToStringCode(), "AA1");
			AssertEquals(new ReoGridPos("ZZ1").ToStringCode(), "ZZ1");
		}
	}
}
