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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace unvell.ReoGrid.Demo.Features
{
	public partial class ClipboardEvent : UserControl
	{
		public ClipboardEvent()
		{
			InitializeComponent();

			grid[1, 1] = new object[,]{
				{ "a","b","c"},
				{ "2012", "2013", "2014"},
				{ 2012, 2013, 2014},
			};

			grid.BeforePaste += grid_BeforePaste;
		}

		void grid_BeforePaste(object sender, RGBeforeRangeOperationEventArgs e)
		{
			if (chkPreventPasteEvent.Checked || chkCustomizePaste.Checked)
			{
				e.IsCancelled = true;

				if (chkCustomizePaste.Checked)
				{
					string text = Clipboard.GetText();

					object[,] data = RGUtility.ParseTabbedString(text);

					// set a new range 
					var applyRange = new ReoGridRange(grid.SelectionRange.Row,
						grid.SelectionRange.Col,
						data.GetLength(0), data.GetLength(1));

					grid.SetRangeData(applyRange, data);

					grid.SetRangeStyle(applyRange, new ReoGridRangeStyle
					{
						Flag = PlainStyleFlag.FillAll,
						BackColor = Color.Yellow,
					});
				}
			}
		}
	}
}
