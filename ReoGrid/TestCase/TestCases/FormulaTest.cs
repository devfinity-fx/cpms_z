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

using unvell.ReoScript;

namespace unvell.ReoGrid.TestCases
{
	[TestSet(DebugEnabled = true, ReleaseEnabled = true)]
	class FormulaTest : ReoGridTestSet
	{
		[TestCase]
		public void NormalFormula()
		{
			SetUp();

			grid[0, 0] = 10;
			grid[0, 1] = "=A1";
			AssertEquals(grid.GetCellText(0, 1), "10");
		}

		[TestCase]
		public void ReferenceUpdate()
		{
			grid[1, 0] = 10;
			grid[1, 1] = "=A2";
			grid[1, 0] = 20;
			AssertEquals(grid.GetCellText(1, 1), "20");
		}

		[TestCase]
		public void ReferenceById()
		{
			grid["A1"] = 10;
			grid["A2"] = 20;
			grid["A3"] = "=A1+A2";

			AssertEquals(grid.GetCellText("A3"), "30");
		}

		[TestCase]
		public void RangeCalc()
		{
			grid.SetRangeData(new ReoGridRange(10, 0, 3, 3), new object[,]{
				{1,2,3},
				{4,5,6},
				{7,8,9},	
			});
			
			grid[10, 5] = "=sum(" + new ReoGridRange(10, 0, 3, 3).ToStringCode() + ")";
			AssertEquals(grid.GetCellText(10, 5), "45");
			
			grid[10, 6] = "=sum(new Range(10, 0, 3, 3))";
			AssertEquals(grid.GetCellText(10, 6), "45");

			AssertEquals(grid.GetCellText(10, 5), grid.GetCellText(10, 6));
		}

		[TestCase]
		public void MathFunctions()
		{
			SetUp();

			grid[0, 0] = "=Math.floor(Math.sin(0.625)*100000)";
			AssertEquals(grid.GetCellText(0, 0), "58509");

			// cell reference sin
			grid[0, 1] = 0.625;
			grid[0, 2] = "=Math.floor(Math.sin(B1)*100000)";
			grid[0, 1] = 1.25;
			AssertEquals(grid.GetCellText(0, 2), "94898");
		}

		[TestCase]
		public void CustomFunction()
		{
			// custom function in script
			grid.RunScript("script.myfun = data => '[' + data + ']'; ");
			grid[10, 0] = "=myfun('abc')";
			AssertEquals(grid.GetCellText(10, 0), "[abc]");

			// custom function in .NET
			grid.Srm["add"] = new NativeFunctionObject("add", (ctx, owner, args) =>
			{
				int v1 = ScriptRunningMachine.GetIntParam(args, 0);
				int v2 = ScriptRunningMachine.GetIntParam(args, 1);
				return v1 + v2;
			});

			grid[0, 10] = "=add(2,3)";
			AssertEquals(grid.GetCellText(0, 10), "5");
		}

		[TestCase]
		public void RangeTest()
		{
			grid.Reset(20, 20);
			grid[0, 0] = 10;
			grid[1, 0] = 20;

			grid[10, 10] = "=sum(1,1,10,10)";
		}
	}

}
