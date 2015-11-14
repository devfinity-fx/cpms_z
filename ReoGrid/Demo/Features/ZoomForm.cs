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
	public partial class ZoomForm : Form
	{
		public ZoomForm()
		{
			InitializeComponent();

			for (int r = 0; r < 40; r++)
			{
				for (int c = 0; c < 10; c++)
				{
					grid[r, c] = (r + 1) * (c + 1);
				}
			}
		}

		private void trackBar1_Scroll(object sender, EventArgs e)
		{
			grid.ScaleFactor = trackBar1.Value / 10f;

			label2.Text = "Current: " + (grid.ScaleFactor*100) + "%";
		}
	}
}
