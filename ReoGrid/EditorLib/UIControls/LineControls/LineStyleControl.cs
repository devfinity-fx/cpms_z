﻿/*****************************************************************************
 * 
 * ReoGrid - .NET Spreadsheet Control
 * 
 * http://reogrid.codeplex.com/
 *
 * Line Style Selection Control
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
using System.Drawing.Drawing2D;

namespace unvell.UIControls
{
	public partial class LineStyleControl : ComboBox
	{
		public LineStyleControl()
		{
			DropDownStyle = ComboBoxStyle.DropDownList;
			DrawMode = DrawMode.OwnerDrawFixed;

			//foreach (object val in Enum.GetValues(typeof(DashStyle)))
			//{
			//  Items.Add(val);
			//}
			base.Items.Add(DashStyle.Solid);
			base.Items.Add(DashStyle.Dot);
			base.Items.Add(DashStyle.Dash);
			base.Items.Add(DashStyle.DashDot);
			base.Items.Add(DashStyle.DashDotDot);
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new ObjectCollection Items { get { return base.Items; } set { } }

		protected override void OnDrawItem(DrawItemEventArgs e)
		{
			Graphics g = e.Graphics;

			e.DrawBackground();

			if (e.Index >= 0 && e.Index < base.Items.Count)
			{
				int x = e.Bounds.X+3;
				int y = e.Bounds.Y + e.Bounds.Height / 2 - 1;
				int x2 = e.Bounds.Right - 3;

				using (Pen p = new Pen(ForeColor, 2f))
				{
					if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
					{
						p.Color = SystemColors.HighlightText;
					}

					p.DashStyle = (DashStyle)Items[e.Index];
					g.DrawLine(p, x, y, x2, y);
				}
			}
		}


	}
}
