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
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using unvell.ReoGrid.DataFormat;

namespace unvell.ReoGrid.Demo.Features
{
	public partial class DataFormatForm : Form
	{
		public DataFormatForm()
		{
			InitializeComponent();

			grid.SetRows(100);

			for (int r = 0; r < 100; r++)
			{
				for (int c = 0; c < 20; c++)
				{
					grid[r, c] = (r + 1) * (c + 1);
				}
			}
		}

		private void btnFormatAsNumber_Click(object sender, EventArgs e)
		{
			grid.SetRangeDataFormat(ReoGridRange.EntireRange, CellDataFormatFlag.Number,
				new NumberDataFormatter.NumberFormatArgs()
				{
					// decimal digit places 0.1234
					DecimalPlaces = 4,
					
					// negative number style: ( 123) 
					NegativeStyle = NumberDataFormatter.NumberNegativeStyle.RedBrackets,

					// use separator: 123,456
					UseSeparator = true,
				});
		}

		private void button2_Click(object sender, EventArgs e)
		{
			grid.SetRangeDataFormat(ReoGridRange.EntireRange, CellDataFormatFlag.DateTime,
				new DateTimeDataFormatter.DateTimeFormatArgs
				{
					// culture
					CultureName = "en-US",

					// pattern
					Format = "yyyy/MM/dd",
				});
		}

		private void button1_Click(object sender, EventArgs e)
		{
			grid.SetRangeDataFormat(ReoGridRange.EntireRange, CellDataFormatFlag.Percent,
				new PercentDataFormatter.PercentFormatArgs
				{
					// decimal digit places
					DecimalPlaces = 2,
				});
		}

		private void button4_Click(object sender, EventArgs e)
		{
			grid.SetRangeDataFormat(ReoGridRange.EntireRange, CellDataFormatFlag.Currency,
				new CurrencyDataFormatter.CurrencyFormatArgs
				{
					// culture name
					CultureEnglishName = "en-US",

					// decimal digit places
					DecimalPlaces = 1,

					// symbol
					Symbol = "$",
				});
		}

		private void button3_Click(object sender, EventArgs e)
		{
			grid.SetRangeDataFormat(ReoGridRange.EntireRange, CellDataFormatFlag.Text, null);
		}
	}
}
