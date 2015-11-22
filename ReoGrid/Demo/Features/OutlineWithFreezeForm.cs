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
	public partial class OutlineWithFreezeForm : Form
	{
		public OutlineWithFreezeForm()
		{
			InitializeComponent();

			for (int i = 1; i < 9; i++)
			{
				grid.AddOutline(RowOrColumn.Row, 4, i);
			}
			for (int i = 1; i < 9; i++)
			{
				grid.AddOutline(RowOrColumn.Column, 2, i);
			}

			grid.FreezeToCell(5, 5);

			grid[2, 2] = "This is sample of frozen outlines.";
		}
	}
}
