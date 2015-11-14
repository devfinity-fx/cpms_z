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

namespace unvell.ReoGrid.Demo.Other
{
	public partial class MaximumGridForm : Form
	{
		public MaximumGridForm()
		{
			InitializeComponent();

			grid.Resize(1048576, 32768);

			grid.MergeRange(1, 1, 1, 6);
			grid[1, 1] = "This is sample for maximum cells (1,048,576 x 32,768)";

			grid.MergeRange(3, 1, 2, 6);
			grid[3, 1] = "You can try scrolling, zoom in/out, edit cell in anywhere or adjust the cells size.";
		}
	}
}
