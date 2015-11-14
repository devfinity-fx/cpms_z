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
using System.Drawing;
using System.Linq;
using System.Text;

using unvell.ReoGrid.Actions;
using unvell.ReoGrid.DataFormat;

namespace unvell.ReoGrid.TestCases
{
	[TestSet(DebugEnabled = true, ReleaseEnabled = true)]
	class DoActionTest : ReoGridTestSet
	{
		[TestCase]
		public void DoAllActionsWithoutException()
		{
			SetUp();

			grid.DoAction(new RGSetCellDataAction(1, 1, "abc"));
			grid.Undo();
			grid.Redo();
			AssertTrue(grid._Debug_Validate_All());

			grid.DoAction(new RGSetRangeBorderAction(new ReoGridRange(1, 1, 2, 2),
				ReoGridBorderPos.All, ReoGridBorderStyle.SolidBlack));
			grid.Undo();
			grid.Redo();
			AssertTrue(grid._Debug_Validate_All());

			grid.DoAction(new RGSetRangeStyleAction(new ReoGridRange(1, 1, 2, 2),
				new ReoGridRangeStyle
				{
					Flag = PlainStyleFlag.FillAll | PlainStyleFlag.TextColor,
					BackColor = Color.DarkBlue,
					TextColor = Color.Silver,
				}));
			grid.Undo();
			grid.Redo();
			AssertTrue(grid._Debug_Validate_All());

			grid.DoAction(new RGMergeRangeAction(new ReoGridRange(1, 1, 2, 2)));
			grid.Undo();
			grid.Redo();
			AssertTrue(grid._Debug_Validate_All());

			grid.DoAction(new RGSetRangeDataFormatAction(new ReoGridRange(1, 1, 2, 2),
				DataFormat.CellDataFormatFlag.Number,
				new NumberDataFormatter.NumberFormatArgs
				{
					DecimalPlaces = 4,
					UseSeparator = true,
				}));
			grid.Undo();
			grid.Redo();
			AssertTrue(grid._Debug_Validate_All());

			grid.RepeatLastAction(new ReoGridRange(3, 3, 2, 2));
			grid.Undo();
			grid.Redo();
			AssertTrue(grid._Debug_Validate_All());

			
		}

	}
}
