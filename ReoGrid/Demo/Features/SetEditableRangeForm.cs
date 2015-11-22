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

using unvell.ReoGrid.Actions;

namespace unvell.ReoGrid.Demo.Features
{
	public partial class SetEditableRangeForm : Form
	{
		public SetEditableRangeForm()
		{
			InitializeComponent();
		}

		private void btnSetEditableRange_Click(object sender, EventArgs ee)
		{
			var editableRange = new ReoGridRange(3, 1, 2, 3);

			grid.SetRangeBorder(editableRange, ReoGridBorderPos.Outline, ReoGridBorderStyle.SolidBlack);

			grid[2, 1] = "Edit only be allowed in this range:";
			grid.BeforeCellEdit += (s, e) => e.IsCancelled = !editableRange.Contains(e.Cell.GetPos());
		}

	}
}
