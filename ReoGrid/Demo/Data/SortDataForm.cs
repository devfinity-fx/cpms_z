﻿/*****************************************************************************
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
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace unvell.ReoGrid.Demo.Data
{
	public partial class SortDataForm : Form
	{
		public SortDataForm()
		{
			InitializeComponent();
		}

		private void button1_Click(object sender, EventArgs e)
		{
			Stopwatch sw = Stopwatch.StartNew();
			grid.SetRows(10000);
			sw.Stop();
			MessageBox.Show(sw.ElapsedMilliseconds + " ms.");

			button2.Enabled = true;
			button3.Enabled = true;
		}

		private void button2_Click(object sender, EventArgs e)
		{
			Stopwatch sw = Stopwatch.StartNew();




			sw.Stop();
			MessageBox.Show(sw.ElapsedMilliseconds + " ms.");
		}

		private void button3_Click(object sender, EventArgs e)
		{
			Stopwatch sw = Stopwatch.StartNew();


		
			sw.Stop();
			MessageBox.Show(sw.ElapsedMilliseconds + " ms.");
		}

	}
}
