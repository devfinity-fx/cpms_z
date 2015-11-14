/*****************************************************************************
 * 
 * ReoGrid - .NET Spreadsheet Control
 * 
 * http://reogrid.codeplex.com/
 * 
 * FlatTab Control - Provide a simple and plain style of tab control
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
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace unvell.UIControls
{
	public class FlatTabControl : Control
	{
		public FlatTabControl()
		{
			DoubleBuffered = true;
		}

		private string[] tabs = { };

		public string[] Tabs
		{
			get { return tabs; }
			set
			{
				tabs = value;

				using (var tabFont = new Font(FontFamily.GenericSansSerif, 8))
				{
					sizes = new int[tabs.Length];
					for (int i = 0; i < tabs.Length; i++)
					{
						sizes[i] = TextRenderer.MeasureText(tabs[i], tabFont).Width + 8;
					}
				}

				Invalidate();
			}
		}

		private int[] sizes;

		private FlatTabStyle style;

		internal FlatTabStyle Style
		{
			get { return style; }
			set { style = value; }
		}

		private int selectedIndex = 0;

		public int SelectedIndex
		{
			get { return selectedIndex; }
			set {
				if (selectedIndex != value)
				{
					selectedIndex = value;

					Invalidate();

					if (SelectedIndexChanged != null)
						SelectedIndexChanged(this, null);
				}
			}
		}

		private bool shadow;

		public bool Shadow
		{
			get { return shadow; }
			set { shadow = value; }
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			Graphics g = e.Graphics;

			Rectangle rect = new Rectangle(2, 2, ClientRectangle.Width - 4,
				ClientRectangle.Height - 2);

			g.DrawLine(SystemPens.ControlDarkDark, 0, rect.Bottom - 1,
					ClientRectangle.Right, rect.Bottom - 1);

			using (var tabFont = new Font(FontFamily.GenericSansSerif, 8))
			{
				for (int i = 0; i < tabs.Length; i++)
				{
					rect.Width = sizes[i];

					if (i != selectedIndex)
					{
						string tab = tabs[i];
						g.DrawString(tab, tabFont, Brushes.DimGray, rect.Left + 5, rect.Top + 3);
					}

					rect.Offset(rect.Width, 0);
				}

				rect = new Rectangle(2, 2, ClientRectangle.Width - 4,
								ClientRectangle.Height - 2);

				for (int i = 0; i < tabs.Length; i++)
				{
					rect.Width = sizes[i];

					if (i == selectedIndex)
					{
						string tab = tabs[i];

						g.DrawLine(SystemPens.ControlDarkDark, rect.Left, rect.Top + 1,
							rect.Left, rect.Bottom);
						g.DrawLine(SystemPens.ControlDarkDark, rect.Left + 1, rect.Top,
							rect.Right - 1, rect.Top);
						g.DrawLine(SystemPens.ControlDarkDark, rect.Right, rect.Top + 1,
							rect.Right, rect.Bottom);

						if (Shadow)
							g.DrawLine(SystemPens.ControlDark, rect.Right + 1, rect.Top + 2,
								rect.Right + 1, rect.Bottom - 1);

						g.FillRectangle(SystemBrushes.Window,
							new Rectangle(rect.Left + 1, rect.Top + 1, rect.Width - 1, rect.Height - 1));

						g.DrawString(tab, tabFont, Brushes.DimGray, rect.Left + 4, rect.Top + 2);

						break;
					}

					rect.Offset(rect.Width, 0);
				}

			}
			base.OnPaint(e);
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			int left=0;
			for (int i = 0; i < tabs.Length; i++)
			{
				left += sizes[i];

				if (e.X < left)
				{
					if (selectedIndex != i)
					{
						selectedIndex = i;
						if (SelectedIndexChanged != null)
							SelectedIndexChanged(this, null);
					}
					
					Invalidate();
					break;
				}
			}
		}

		public event EventHandler SelectedIndexChanged;

		internal enum FlatTabStyle
		{
			RectShadow,
			SplitRouned,
		}
	}
}
