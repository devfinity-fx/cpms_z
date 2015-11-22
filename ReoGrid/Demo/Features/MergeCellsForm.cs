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

namespace unvell.ReoGrid.Demo.Features
{
	public partial class MergeCellsForm : Form
	{
		public MergeCellsForm()
		{
			InitializeComponent();

			grid.SelectionRangeChanged += (s, e) =>
			{
				label1.Text = "Selected range: " + grid.SelectionRange.ToString();
			};
		}

		private void btnMerge_Click(object sender, EventArgs e)
		{
			if (grid.SelectionRange.Rows == 1 && grid.SelectionRange.Cols == 1)
			{
				MessageBox.Show("Selected range must contain at least two cells.");
			}
			else
			{
				try
				{
					grid.MergeRange(grid.SelectionRange);
				}
				catch (RangeIntersectionException)
				{
					MessageBox.Show("Cannot merge an intersected range.");
				}
			}
		}

		private void btnUnmerge_Click(object sender, EventArgs e)
		{
			grid.UnmergeRange(grid.SelectionRange);
		}

		private void btnMergeByScript_Click(object sender, EventArgs e)
		{
#if EX_SCRIPT
			if (grid.SelectionRange.Rows == 1 && grid.SelectionRange.Cols == 1)
			{
				MessageBox.Show("Selected range must contain at least two cells.");
			}
			else
			{
				grid.RunScript("grid.mergeRange(grid.selection));");
			}
#else
			MessageBox.Show("Script execution is not available in this edition.");
#endif
		}

		private void btnUnmergeByScript_Click(object sender, EventArgs e)
		{
#if EX_SCRIPT
	grid.RunScript("grid.unmergeRange(grid.selection));");
#else
			MessageBox.Show("Script execution is not available in this edition.");
#endif
		}

		private void btnSimulateException_Click(object sender, EventArgs e)
		{
			grid.Reset();

			try
			{
				// try to merge an intersected range will get an exception
				grid.MergeRange(new ReoGridRange(2, 2, 5, 5));
				grid.MergeRange(new ReoGridRange(3, 3, 5, 5));
			}
			catch(RangeIntersectionException)
			{
				MessageBox.Show("Exception RangeIntersectionException catched!\n\n Cannot change part of a range.");
			}
		}
	}
}
