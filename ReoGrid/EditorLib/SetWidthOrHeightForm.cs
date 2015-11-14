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

using unvell.Common.Win32Lib;

namespace unvell.ReoGrid.Editor
{
	public partial class SetWidthOrHeightForm : Form
	{
		private int value;

		public int Value
		{
			get { return value; }
			set { this.value = value; }
		}

		public SetWidthOrHeightForm(ChangeWidthOrHeight widthOrHeight)
		{
			InitializeComponent();

			StartPosition = FormStartPosition.Manual;

			if(widthOrHeight== ChangeWidthOrHeight.Height){
			Text = "Row height";
			label1.Text = "Row height";
			}
			else
			{
				Text = "Column width";
				label1.Text = "Column height";
			}
			btnOK.Text = "OK";
			btnCancel.Text = "Cancel";

			numericUpDown1.KeyDown += new KeyEventHandler(numericUpDown1_KeyDown);
		}

		void numericUpDown1_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter)
			{
				btnOK.PerformClick();
			}
			else if (e.KeyCode == Keys.Escape)
			{
				DialogResult = DialogResult.Cancel;
				Close();
			}
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			numericUpDown1.Value = value;
			numericUpDown1.Select(0, numericUpDown1.Value.ToString().Length);

			Point p = Cursor.Position;
			p.Offset(-115, -61);
			Location = p;
		}

		private void btnOK_Click(object sender, EventArgs e)
		{
			value = (int)numericUpDown1.Value;
		}
	}

	public enum ChangeWidthOrHeight
	{
		Width,
		Height
	}
}
