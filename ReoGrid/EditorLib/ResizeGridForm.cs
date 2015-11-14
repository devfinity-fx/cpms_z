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

namespace unvell.ReoGrid.Editor
{
	public partial class ResizeGridForm : Form
	{
		public int Rows { get; set; }
		public int Cols { get; set; }

		public ResizeGridForm()
		{
			InitializeComponent();

			numRows.KeyDown += numRows_KeyDown;
			numCols.KeyDown += numRows_KeyDown;
		}

		void numRows_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter)
				button1.PerformClick();
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			numRows.Value = Rows;
			numCols.Value = Cols;
		}

		private void button1_Click(object sender, EventArgs e)
		{
			Rows = (int)numRows.Value;
			Cols = (int)numCols.Value;
		}
	}
}
