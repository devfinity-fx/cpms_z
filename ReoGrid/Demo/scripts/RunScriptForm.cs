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

namespace unvell.ReoGrid.Demo.Scripts
{
	public partial class RunScriptForm : Form
	{
		public RunScriptForm()
		{
			InitializeComponent();

			grid.SetRows(14);
			grid.SetCols(6);

			for (int r = 0; r < 10; r++)
			{
				for (int c = 0; c < 5; c++)
				{
					grid[r, c] = (r + 1) * (c + 1);
				}
			}
		}

		private void btnHelloworld_Click(object sender, EventArgs e)
		{
#if EX_SCRIPT
			grid.RunScript("alert('hello world');");
#else
			MessageBox.Show("Script execution is not available in this edition.");
#endif
		}

		private void button1_Click(object sender, EventArgs e)
		{
#if EX_SCRIPT
			var script = @"

var range = grid.selection.range;

alert('current selection: ' + range);

";

			grid.RunScript(script);
#else
			MessageBox.Show("Script execution is not available in this edition.");
#endif
		}

		private void button2_Click(object sender, EventArgs e)
		{
#if EX_SCRIPT
			var script = @"

var pos = grid.selection.pos;

var cell = grid.getCell(pos);

cell.style.backgroundColor = 'darkgreen';

";

			grid.RunScript(script);
#else
			MessageBox.Show("Script execution is not available in this edition.");
#endif
		}

	}
}
