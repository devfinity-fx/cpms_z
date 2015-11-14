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
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using unvell.ReoGrid.PropertyPages;
using unvell.ReoGrid.Actions;

namespace unvell.ReoGrid.Editor.PropertyPages
{
	public partial class AlignmentPage : UserControl, IPropertyPage
	{
		public AlignmentPage()
		{
			InitializeComponent();

			cmbHorAlign.Items.AddRange(new string[]{
				"General", "Left", "Center", "Right", "Distributed (Indent)"
			});
			cmbVerAlign.Items.AddRange(new string[]{
				"Top", "Middle", "Bottom"
			});

		}

		#region IPropertyPage Members

		private ReoGridControl grid;

		public ReoGridControl Grid
		{
			get { return grid; }
			set { this.grid = value; }
		}

		private CheckState backupTextWrapState;
		private ReoGridHorAlign backupHorAlign;
		private ReoGridVerAlign backupVerAlign;

		public void LoadPage()
		{
			ReoGridRangeStyle style = grid.GetRangeStyle(grid.SelectionRange);

			if (style.TextWrapMode == TextWrapMode.WordBreak
				|| style.TextWrapMode == TextWrapMode.BreakAll)
			{
				backupTextWrapState = CheckState.Checked;
			}
			else
			{
				backupTextWrapState = CheckState.Unchecked;
			}

			backupHorAlign = style.HAlign;
			backupVerAlign = style.VAlign;

			switch (style.HAlign)
			{
				case ReoGridHorAlign.General:
					cmbHorAlign.SelectedIndex = 0; break;
				case ReoGridHorAlign.Left:
					cmbHorAlign.SelectedIndex = 1; break;
				case ReoGridHorAlign.Center:
					cmbHorAlign.SelectedIndex = 2; break;
				case ReoGridHorAlign.Right:
					cmbHorAlign.SelectedIndex = 3; break;
				case ReoGridHorAlign.DistributedIndent:
					cmbHorAlign.SelectedIndex = 4; break;
			}

			switch (style.VAlign)
			{
				case ReoGridVerAlign.Top:
					cmbVerAlign.SelectedIndex = 0; break;
				case ReoGridVerAlign.Middle:
					cmbVerAlign.SelectedIndex = 1; break;
				case ReoGridVerAlign.Bottom:
					cmbVerAlign.SelectedIndex = 2; break;
			}

			chkWrapText.CheckState = backupTextWrapState;
		}

		public RGReusableAction CreateUpdateAction()
		{
			var style = new ReoGridRangeStyle();

			ReoGridHorAlign halign = ReoGridHorAlign.General;
			ReoGridVerAlign valign = ReoGridVerAlign.Middle;

			switch (cmbHorAlign.SelectedIndex)
			{
				default:
				case 0: halign = ReoGridHorAlign.General; break;
				case 1: halign = ReoGridHorAlign.Left; break;
				case 2: halign = ReoGridHorAlign.Center; break;
				case 3: halign = ReoGridHorAlign.Right; break;
				case 4: halign = ReoGridHorAlign.DistributedIndent; break;
			}

			switch (cmbVerAlign.SelectedIndex)
			{
				case 0: valign = ReoGridVerAlign.Top; break;
				default: case 1: valign = ReoGridVerAlign.Middle; break;
				case 2: valign = ReoGridVerAlign.Bottom; break;
			}

			switch (style.VAlign)
			{
				case ReoGridVerAlign.Top:
					cmbHorAlign.SelectedIndex = 0; break;
				case ReoGridVerAlign.Middle:
					cmbHorAlign.SelectedIndex = 1; break;
				case ReoGridVerAlign.Bottom:
					cmbHorAlign.SelectedIndex = 2; break;
			}

			if (backupHorAlign != halign)
			{
				style.Flag |= PlainStyleFlag.HorizontalAlign;
				style.HAlign = halign;
			}

			if (backupVerAlign != valign)
			{
				style.Flag |= PlainStyleFlag.VerticalAlign;
				style.VAlign = valign;
			}

			if (backupTextWrapState != chkWrapText.CheckState)
			{
				style.Flag |= PlainStyleFlag.TextWrap;

				if (chkWrapText.Checked)
				{
					style.TextWrapMode = TextWrapMode.WordBreak;
				}
				else
				{
					style.TextWrapMode = TextWrapMode.NoWrap;
				}
			}

			return new RGSetRangeStyleAction(grid.SelectionRange, style);
		}

		public event EventHandler Done;

		#endregion
	}
}
