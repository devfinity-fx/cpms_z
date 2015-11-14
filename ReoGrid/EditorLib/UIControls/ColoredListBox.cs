/*****************************************************************************
 * 
 * ReoGrid - .NET Spreadsheet Control
 * 
 * http://reogrid.codeplex.com/
 *
 * Colored Listbox - Show list items with color.

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

namespace unvell.UIControls
{
	public partial class ColoredListBox : ListBox
	{
		public ColoredListBox()
			: base()
		{
			base.DrawMode = DrawMode.OwnerDrawFixed;
		}

		private Func<int, object, Color> colorGetter;

		public Func<int, object, Color> ColorGetter
		{
			get { return colorGetter; }
			set { colorGetter = value; }
		}

		protected override void OnDrawItem(DrawItemEventArgs e)
		{
			e.DrawBackground();

			if (Items.Count <= 0)
			{
				base.OnDrawItem(e);
				return;
			}

			Color color;

			color = ColorGetter == null ? SystemColors.WindowText
					: ColorGetter(e.Index, Items[e.Index]);

			if(color == SystemColors.WindowText 
				&& ((e.State & DrawItemState.Selected) == DrawItemState.Selected))
				color = SystemColors.HighlightText;

			using (Brush b = new SolidBrush(color))
			{
				e.Graphics.DrawString(Convert.ToString(Items[e.Index]), e.Font, b, e.Bounds);
			}

			if((e.State & DrawItemState.Focus) == DrawItemState.Focus)
			{
				ControlPaint.DrawFocusRectangle(e.Graphics, e.Bounds);
			}
		}
	}
}
