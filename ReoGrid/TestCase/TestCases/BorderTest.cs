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
	class BorderTest : ReoGridTestSet
	{
		[TestCase]
		public void NormalSet()
		{
			SetUp();

			grid.SetRangeBorder(new ReoGridRange(1, 1, 2, 10), ReoGridBorderPos.Outline, ReoGridBorderStyle.SolidBlack);
			grid.SetRangeBorder(new ReoGridRange(1, 5, 10, 2), ReoGridBorderPos.Outline, ReoGridBorderStyle.SolidBlack);

			AssertTrue(grid._Debug_Validate_All());
		}

		[TestCase]
		public void DeleteRangeBorder()
		{
			SetUp();

			grid.SetRangeBorder(new ReoGridRange(2, 2, 3, 5),
				ReoGridBorderPos.All, ReoGridBorderStyle.SolidBlack);

			grid.SetRangeBorder(new ReoGridRange(2, 7, 3, 5),
				ReoGridBorderPos.All, ReoGridBorderStyle.SolidBlack);

			grid.DeleteCols(3, 5);

			AssertTrue(grid._Debug_Validate_All());
		}
		
		[TestCase]
		void RandomSpanTest()
		{
			// set
			for (int i = 0; i < 100; i++)
			{
				int r = rand.Next(25);
				int c = rand.Next(20);

				ReoGridBorderPos borderPos = ReoGridBorderPos.None;
				switch (rand.Next(5))
				{
					case 0: borderPos = ReoGridBorderPos.Left; break;
					case 1: borderPos = ReoGridBorderPos.Right; break;
					case 2: borderPos = ReoGridBorderPos.Top; break;
					case 3: borderPos = ReoGridBorderPos.Bottom; break;
					case 4: borderPos = ReoGridBorderPos.Outline; break;
				}

				grid.SetRangeBorder(new ReoGridRange(r, c, 1, 1), borderPos, ReoGridBorderStyle.SolidBlack);

				//Application.DoEvents();
			}

			// check
			AssertTrue(Grid._Debug_Validate_BorderSpan());
		}


	}

}
