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

using unvell.ReoScript;

namespace unvell.ReoGrid.Demo.Scripts
{
	public partial class AddCustomizeFunctionForm : Form
	{
		public AddCustomizeFunctionForm()
		{
			InitializeComponent();

			grid[0, 0] = "func1:";
			grid[1, 0] = "func2:";
		}

		private void btnAddCustomizeFunction_Click(object sender, EventArgs e)
		{
			grid.Srm["func1"] = new NativeFunctionObject("func1", (ctx, owner, args) =>
			{
				return (args.Length < 1) ? null : ("[" + args[0] + "]");
			});

			grid[0, 1] = "abc";
			grid[0, 2] = "=func1(B1)";
		}

		private void button1_Click(object sender, EventArgs e)
		{
			grid.RunScript(@"

script.func2 = function(data) {
  return '[' + data + ']';
}

@");

			grid[1, 1] = "xyz";
			grid[1, 2] = "=func2(B2)";
		}
	}
}
