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
	public partial class OnlyNumberInputForm : Form
	{
		public OnlyNumberInputForm()
		{
			InitializeComponent();

			grid.AfterCellEdit += grid_AfterCellEdit;

			chkErrorPrompt.CheckedChanged += (s, e) =>
			{
				if (!chkOnlyNumeric.Checked) chkOnlyNumeric.Checked = true;
			};
		}

		void grid_AfterCellEdit(object sender, RGCellAfterEditEventArgs e)
		{
			if (chkOnlyNumeric.Checked)
			{
				float val = 0f;

				if (e.NewData == null || !float.TryParse(e.NewData.ToString(), out val))
				{
					if (chkErrorPrompt.Checked)
					{
						MessageBox.Show("Only numeric is allowed.");
					}
				
					e.EndReason = ReoGridEndEditReason.Cancel;
				}
				else
				{
					e.NewData = val;
				}
			}
		}
	}
}
