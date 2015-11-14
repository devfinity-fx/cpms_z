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
using System.Drawing.Drawing2D;

using unvell.ReoGrid.Actions;

namespace unvell.ReoGrid.PropertyPages
{
	public partial class BorderPropertyPage : UserControl, IPropertyPage
	{
		private ReoGridControl grid;

		public ReoGridControl Grid
		{
			get { return grid; }
			set { this.grid = value; }
		}

		public BorderPropertyPage()
		{
			InitializeComponent();

			borderColorSelector.ColorSelected += (sender, e) =>
			{
				borderStyleList.BorderColor = borderColorSelector.SolidColor;
				borderSetter.CurrentColor = borderColorSelector.SolidColor;
			};

			borderStyleList.BorderStyleSelectionChanged += (sender, e) =>
			{
				borderSetter.CurrentBorderStlye = borderStyleList.SelectedBorderStyle;
			};

			borderColorSelector.SolidColor = Color.Black;
			borderStyleList.SelectedBorderStyle = BorderLineStyle.Solid;
			borderSetter.CurrentColor = borderColorSelector.SolidColor;
		}

		public event EventHandler Done;

		#region Border Button
		private void btnNone_Click(object sender, EventArgs e)
		{
			borderSetter.RemoveBorderStyle(ReoGridBorderPos.All);
		}

		private void btnOutline_Click(object sender, EventArgs e)
		{
			borderSetter.CheckBorderStyle(ReoGridBorderPos.Outline);
		}

		private void btnInside_Click(object sender, EventArgs e)
		{
			borderSetter.CheckBorderStyle(ReoGridBorderPos.InsideAll);
		}

		private void btnTop_Click(object sender, EventArgs e)
		{
			borderSetter.InvertBorderStats(ReoGridBorderPos.Top);
		}

		private void btnMiddle_Click(object sender, EventArgs e)
		{
			borderSetter.InvertBorderStats(ReoGridBorderPos.InsideHorizontal);
		}

		private void btnBottom_Click(object sender, EventArgs e)
		{
			borderSetter.InvertBorderStats(ReoGridBorderPos.Bottom);
		}

		private void btnSlash_Click(object sender, EventArgs e)
		{
			borderSetter.InvertBorderStats(ReoGridBorderPos.Slash);
		}

		private void btnLeft_Click(object sender, EventArgs e)
		{
			borderSetter.InvertBorderStats(ReoGridBorderPos.Left);
		}

		private void btnCenter_Click(object sender, EventArgs e)
		{
			borderSetter.InvertBorderStats(ReoGridBorderPos.InsideVertical);
		}

		private void btnRight_Click(object sender, EventArgs e)
		{
			borderSetter.InvertBorderStats(ReoGridBorderPos.Right);
		}

		private void btnBackslash_Click(object sender, EventArgs e)
		{
			borderSetter.InvertBorderStats(ReoGridBorderPos.Backslash);
		}
		#endregion

		public void LoadPage()
		{
			borderSetter.ReadFromGrid(grid);

			btnCenter.Enabled = borderSetter.Rows > 1;
			btnMiddle.Enabled = borderSetter.Cols > 1;

			ReoGridBorderStyle style = borderSetter.Borders.Values.FirstOrDefault();
			if (!style.IsEmpty)
			{
				borderStyleList.SelectedBorderStyle = style.Style;
				borderColorSelector.SolidColor = style.Color;
				borderColorSelector.RaiseColorPicked();
			}
		}

		public RGReusableAction CreateUpdateAction()
		{
			return borderSetter.CreateUpdateAction(grid);
		}
	}

	#region Border Style List
	internal class BorderStyleListControl : Control
	{
		public List<BorderStyleListItem> items = new List<BorderStyleListItem>();

		public BorderStyleListControl()
		{
			DoubleBuffered = true;

			BorderLineStyle[] styles = (BorderLineStyle[])Enum.GetValues(typeof(BorderLineStyle));
			for (int i = 0; i < styles.Length; i++)
			{
				items.Add(new BorderStyleListItem
				{
					Bounds = Rectangle.Empty,
					Style = styles[i],
					IsSelected = styles[i] == borderStyle,
				});
			}
		}

		protected override void OnCreateControl()
		{
			base.OnCreateControl();

			int w = (ClientRectangle.Width >> 1) - 10;

			Rectangle rect = new Rectangle(5, 7, w, 15);

			for (int i = 0; i < items.Count; i++)
			{
				items[i].Bounds = rect;

				rect.Offset(0, 15);

				if (i == 6)
				{
					rect.Location = new Point(w + 15, 7);
				}
			}
		}

		private Color borderColor = Color.Black;

		public Color BorderColor
		{
			get { return borderColor; }
			set { borderColor = value; Invalidate(); }
		}

		private BorderLineStyle borderStyle = BorderLineStyle.Solid;

		[DefaultValue(BorderLineStyle.Solid)]
		public BorderLineStyle BorderStyle
		{
			get { return borderStyle; }
			set { borderStyle = value; }
		}

		public BorderLineStyle SelectedBorderStyle
		{
			get
			{
				BorderStyleListItem item = items.FirstOrDefault(i => i.IsSelected);
				return (item == null) ? BorderLineStyle.None : item.Style;
			}
			set
			{
				SelectBorderStyle(value);
			}
		}

		public void SelectBorderStyle(BorderLineStyle style)
		{
			foreach (var i in items)
			{
				i.IsSelected = (i.Style == style);
			}

			Invalidate();
			if (BorderStyleSelectionChanged != null) BorderStyleSelectionChanged(this, null);
		}

		public event EventHandler BorderStyleSelectionChanged;

		protected override void OnMouseDown(MouseEventArgs e)
		{
			BorderStyleListItem item = items.FirstOrDefault(i=>i.Bounds.Contains(e.Location));
			if (item != null) SelectBorderStyle(item.Style);
		}

		private static readonly StringFormat sf = new StringFormat();
		static BorderStyleListControl()
		{
			sf.Alignment = StringAlignment.Center;
			sf.LineAlignment = StringAlignment.Center;
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			Graphics g = e.Graphics;
		
			foreach(var i in items)
			{
				if (i.Style == BorderLineStyle.None)
				{
					g.DrawString("None", SystemFonts.DefaultFont, Brushes.Black, i.Bounds, sf);
				}
				else
				{
					BorderPainter.Instance.DrawLine(g, i.Bounds.Left, i.Bounds.Top + i.Bounds.Height / 2,
						i.Bounds.Right, i.Bounds.Top + i.Bounds.Height / 2, i.Style, borderColor);
				}

				if (i.IsSelected)
				{
					using (Pen p = new Pen(Color.Black))
					{
						p.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;
						g.DrawRectangle(p, i.Bounds);
					}
				}

				if (i.IsFocus)
				{
					ControlPaint.DrawFocusRectangle(g, i.Bounds);
				}
			}

			Rectangle rect = new Rectangle(ClientRectangle.Left, ClientRectangle.Left,
				ClientRectangle.Width - 1, ClientRectangle.Height - 1);
			g.DrawRectangle(SystemPens.ButtonShadow, rect);
		}
	}

	internal class BorderStyleListItem
	{
		private Rectangle bounds;

		public Rectangle Bounds
		{
			get { return bounds; }
			set { bounds = value; }
		}

		private BorderLineStyle style;

		public BorderLineStyle Style
		{
			get { return style; }
			set { style = value; }
		}

		private bool isFocus = false;

		public bool IsFocus
		{
			get { return isFocus; }
			set { isFocus = value; }
		}

		private bool isSelected = false;

		public bool IsSelected
		{
			get { return isSelected; }
			set { isSelected = value; }
		}
	}
	#endregion

	#region Border Setter
	internal class BorderSetterControl : Control
	{
		ReoGridBorderPos[] allPos;

		public BorderSetterControl()
		{
			DoubleBuffered = true;
			allPos = (ReoGridBorderPos[])Enum.GetValues(typeof(ReoGridBorderPos));
		}

		private int rows = 2;

		public int Rows
		{
			get { return rows; }
			set { rows = value; if (rows > 2) rows = 2; UpdateRects(); }
		}

		private int cols = 2;

		public int Cols
		{
			get { return cols; }
			set { cols = value; if (cols > 2) cols = 2; UpdateRects(); }
		}

		protected override void OnResize(EventArgs e)
		{
			base.OnResize(e);

			UpdateRects();
		}

		private BorderLineItem[] items = new BorderLineItem[8];

		private void UpdateRects()
		{
			int width = ClientRectangle.Width - 16 - Padding.Left - Padding.Right;
			int height = ClientRectangle.Height - 16 - Padding.Top - Padding.Bottom;

			int cw = width / cols;
			int rw = height / rows;

			int y = ClientRectangle.Top + 8 + Padding.Top;
			int x = ClientRectangle.Left + 8 + Padding.Left;

			Rectangle rect = new Rectangle(x, y - 6, width, 13);
			
			int i = 0;

			for (int r = 0; r <= rows; r++)
			{
				items[i].rect = rect;
				if (r == 0) items[i].pos = ReoGridBorderPos.Top;
				else if (r == rows) items[i].pos = ReoGridBorderPos.Bottom;
				else items[i].pos = ReoGridBorderPos.InsideHorizontal;
				rect.Y += rw;
				i++;
			}

			rect.X = x - 6;
			rect.Y = y;
			rect.Size = new Size(13, height);

			for (int c = 0; c <= cols; c++)
			{
				items[i].rect = rect;
				if (c == 0) items[i].pos = ReoGridBorderPos.Left;
				else if (c == cols) items[i].pos = ReoGridBorderPos.Right;
				else items[i].pos = ReoGridBorderPos.InsideVertical;
				rect.X += cw;
				i++;
			}
			
			Invalidate();
		}

		private bool allowHover = false;

		[DefaultValue(false)]
		public bool AllowHover
		{
			get { return allowHover; }
			set { allowHover = value; }
		}

		#region Draw
		protected override void OnPaint(PaintEventArgs e)
		{
			Graphics g = e.Graphics;

			int width = ClientRectangle.Width - 16 - Padding.Right - Padding.Left;
			int height = ClientRectangle.Height - 16 - Padding.Top - Padding.Bottom;

			int cw = width / cols;
			int rw = height / rows;

			if (allowHover)
			{
				using (Brush hoverB = new SolidBrush(Color.FromArgb(224, 224, 224)))
				{
					for (int i = 0; i < items.Length; i++)
					{
						if (items[i].hover) g.FillRectangle(hoverB, items[i].rect);
					}
				}
			}

			Pen markPen = SystemPens.ButtonShadow;

			int y = ClientRectangle.Top + 8 + Padding.Top;
			int x = 0;

			for (int r = 0; r <= rows; r++)
			{
				x = ClientRectangle.Left + 8 + Padding.Left;

				for (int c = 0; c <= cols; c++)
				{
					if (r == 0)
					{
						if (c == 0)
						{
							g.DrawLine(markPen, x - 1, y, x - 5, y);
							g.DrawLine(markPen, x, y - 1, x, y - 5);
						}
						else if (c == cols)
						{
							g.DrawLine(markPen, x + 1, y, x + 5, y);
							g.DrawLine(markPen, x, y - 1, x, y - 5);
						}
						else
						{
							g.DrawLine(markPen, x - 3, y - 1, x + 3, y - 1);
							g.DrawLine(markPen, x, y - 1, x, y - 5);
						}
					}
					else if (r == rows)
					{
						if (c == 0)
						{
							g.DrawLine(markPen, x - 1, y, x - 5, y);
							g.DrawLine(markPen, x, y + 1, x, y + 5);
						}
						else if (c == cols)
						{
							g.DrawLine(markPen, x + 1, y, x + 5, y);
							g.DrawLine(markPen, x, y + 1, x, y + 5);
						}
						else
						{
							g.DrawLine(markPen, x - 3, y + 1, x + 3, y + 1);
							g.DrawLine(markPen, x, y + 2, x, y + 6);
						}
					}
					else
					{
						if (c == 0)
						{
							g.DrawLine(markPen, x - 1, y - 3, x - 1, y + 3);
							g.DrawLine(markPen, x - 2, y, x - 6, y);
						}
						else if (c == cols)
						{
							g.DrawLine(markPen, x + 1, y - 3, x + 1, y + 3);
							g.DrawLine(markPen, x + 2, y, x + 6, y);
						}
					}

					if (r < rows && c < cols)
					{
						Rectangle rect = new Rectangle(x, y, cw, rw);

						using (StringFormat sf = new StringFormat()
						{
							Alignment = StringAlignment.Center,
							LineAlignment = StringAlignment.Center
						})
						{
							g.DrawString("Text", Font, SystemBrushes.ControlDarkDark, rect, sf);
						}
					}

					x += cw;
				}

				y += rw;
			}

			y = ClientRectangle.Top + 8 + Padding.Top;
			x = ClientRectangle.Left + 8 + Padding.Left;

			using (Brush mixB = new HatchBrush(HatchStyle.Percent50, Color.White, Color.Silver))
			{
				for (int r = 0; r <= rows; r++)
				{
					if (r == 0)
					{
						if ((mixBorders & ReoGridBorderPos.Top) == ReoGridBorderPos.Top)
							g.FillRectangle(mixB, x, y - 1, width, 3);
						else if (borders.ContainsKey(ReoGridBorderPos.Top))
							BorderPainter.Instance.DrawLine(g, x, y, x + width, y, borders[ReoGridBorderPos.Top]);
					}
					else if (r == rows)
					{
						if ((mixBorders & ReoGridBorderPos.Bottom) == ReoGridBorderPos.Bottom)
							g.FillRectangle(mixB, x, y - 1, width, 3);
						else if (borders.ContainsKey(ReoGridBorderPos.Bottom))
							BorderPainter.Instance.DrawLine(g, x, y, x + width, y, borders[ReoGridBorderPos.Bottom]);
					}
					else
					{
						if ((mixBorders & ReoGridBorderPos.InsideHorizontal) == ReoGridBorderPos.InsideHorizontal)
							g.FillRectangle(mixB, x, y - 1, width, 3);
						else if (borders.ContainsKey(ReoGridBorderPos.InsideHorizontal))
							BorderPainter.Instance.DrawLine(g, x, y, x + width, y, borders[ReoGridBorderPos.InsideHorizontal]);
					}

					y += rw;
				}

				y = ClientRectangle.Top + 8 + Padding.Top;
				for (int c = 0; c <= cols; c++)
				{
					if (c == 0)
					{
						if ((mixBorders & ReoGridBorderPos.Left) == ReoGridBorderPos.Left)
							g.FillRectangle(mixB, x - 1, y, 3, height);
						else if (c == 0 && borders.ContainsKey(ReoGridBorderPos.Left))
							BorderPainter.Instance.DrawLine(g, x, y, x, y + height, borders[ReoGridBorderPos.Left]);
					}
					else if (c == cols)
					{
						if ((mixBorders & ReoGridBorderPos.Right) == ReoGridBorderPos.Right)
							g.FillRectangle(mixB, x - 1, y, 3, height);
						else if (borders.ContainsKey(ReoGridBorderPos.Right))
							BorderPainter.Instance.DrawLine(g, x, y, x, y + height, borders[ReoGridBorderPos.Right]);
					}
					else
					{
						if ((mixBorders & ReoGridBorderPos.InsideVertical) == ReoGridBorderPos.InsideVertical)
							g.FillRectangle(mixB, x - 1, y, 3, height);
						else if (borders.ContainsKey(ReoGridBorderPos.InsideVertical))
							BorderPainter.Instance.DrawLine(g, x, y, x, y + height, borders[ReoGridBorderPos.InsideVertical]);
					}

					x += cw;
				}
			}


			if (controlBorder)
			{
				Rectangle controlBoundsRect = new Rectangle(ClientRectangle.Left, ClientRectangle.Top,
					ClientRectangle.Width - 1, ClientRectangle.Height - 1);
				g.DrawRectangle(SystemPens.ButtonShadow, controlBoundsRect);
			}
		}
		#endregion

		private bool controlBorder = true;

		[DefaultValue(true)]
		public bool ControlBorder
		{
			get { return controlBorder; }
			set { controlBorder = value; Invalidate(); }
		}

		private Color currentColor = Color.Black;

		public Color CurrentColor
		{
			get { return currentColor; }
			set { currentColor = value; }
		}

		private BorderLineStyle currentBorderStlye;

		public BorderLineStyle CurrentBorderStlye
		{
			get { return currentBorderStlye; }
			set { currentBorderStlye = value; }
		}

		private Dictionary<ReoGridBorderPos, ReoGridBorderStyle> borders =
			new Dictionary<ReoGridBorderPos, ReoGridBorderStyle>();

		public Dictionary<ReoGridBorderPos, ReoGridBorderStyle> Borders
		{
			get { return borders; }
			set { borders = value; }
		}

		private void ProcessBorderStyles(ReoGridBorderPos tpos, Action<ReoGridBorderPos> handler)
		{
			for (int i = 0; i < allPos.Length; i++)
			{
				ReoGridBorderPos pos = allPos[i];
				if (allPos[i] != ReoGridBorderPos.None && (tpos & pos) == pos)
				{
					handler(pos);
				}
			}
		}

		public void CheckBorderStyle(ReoGridBorderPos pos)
		{
			ProcessBorderStyles(pos, p =>
			{
				borders[p] = new ReoGridBorderStyle
				{
					Style = currentBorderStlye,
					Color = currentColor,
				};
				mixBorders &= ~p;
			});

			borderAdded |= pos;
			borderRemoved &= ~pos;
			Invalidate();
		}

		public void RemoveBorderStyle(ReoGridBorderPos pos)
		{
			ProcessBorderStyles(pos, p =>
			{
				borders.Remove(p);
				mixBorders &= ~p;
				borderRemoved |= pos;
				borderAdded &= ~p;
			});
			Invalidate();
		}

		public void InvertBorderStats(ReoGridBorderPos pos)
		{
			ProcessBorderStyles(pos, p =>
			{
				if ((mixBorders & p) == p)
				{
					mixBorders &= ~p;
					borders.Remove(p);
					borderRemoved |= p;
					borderAdded &= ~p;
				}

				if (borders.ContainsKey(p) && borders[p].Style == currentBorderStlye && borders[p].Color == currentColor)
				{
					borders.Remove(p);
					borderRemoved |= p;
					borderAdded &= ~p;
				}
				else
					CheckBorderStyle(p);
			});

			Invalidate();
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			ReoGridBorderPos pos = GetBorderPos(e.Location);
			if (pos != ReoGridBorderPos.None)
			{
				InvertBorderStats(pos);
				Invalidate();
			}
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			if (allowHover)
			{
				for (int i = 0; i < items.Length; i++)
				{
					bool beforeHover = items[i].hover;
					items[i].hover = items[i].rect.Contains(e.Location);
					if (beforeHover != items[i].hover) Invalidate();
				}
			}
		}

		private ReoGridBorderPos GetBorderPos(Point p)
		{
			for (int i = 0; i < items.Length; i++)
			{
				if (items[i].rect.Contains(p)) return items[i].pos;
			}
			return ReoGridBorderPos.None;
		}

		private ReoGridBorderPos borderAdded = ReoGridBorderPos.None;
		private ReoGridBorderPos borderRemoved = ReoGridBorderPos.None;

		public RGSetRangeBorderAction CreateUpdateAction(ReoGridControl grid)
		{
			// just creating actions for changed borders
			if (borderAdded != ReoGridBorderPos.None || borderRemoved != ReoGridBorderPos.None)
			{
				List<ReoGridBorderPosStyle> styles = new List<ReoGridBorderPosStyle>();

				AddBorderPosStyle(styles, borderAdded);

				// fix bug: border cannot be removed
				//AddBorderPosStyle(styles, borderRemoved);
				styles.Add(new ReoGridBorderPosStyle(borderRemoved, ReoGridBorderStyle.Empty));

				RGSetRangeBorderAction action = new RGSetRangeBorderAction(grid.SelectionRange, styles.ToArray());
				return action;
			}
			else
				return null;
		}

		private void AddBorderPosStyle(List<ReoGridBorderPosStyle> posStyles, ReoGridBorderPos scope)
		{
			// filter border without any changes
			var q = borders.Where(b => (scope & b.Key) == b.Key).GroupBy(b => b.Value);

			ReoGridBorderPos allPos = ReoGridBorderPos.None;

			// find same styles for creating actions as few as possible
			foreach (var bs in q)
			{
				ReoGridBorderPos pos = ReoGridBorderPos.None;

				// find same styles
				foreach (var p in borders.Where(b => (scope & b.Key) == b.Key && b.Value == bs.Key))
				{
					// union position
					pos |= p.Key;
				}

				allPos |= pos;

				if (pos != ReoGridBorderPos.None)
				{
					posStyles.Add(new ReoGridBorderPosStyle(pos, bs.Key));
				}
			}
		}

		/// <summary>
		/// Load border info from grid
		/// </summary>
		/// <param name="grid"></param>
		public void ReadFromGrid(ReoGridControl grid)
		{
			if (grid.SelectionRange.IsEmpty)
			{
				this.Enabled = false;
			}
			else
			{
				ReoGridRangeBorderInfo info = grid.GetRangeBorder(grid.SelectionRange, false);

				if (!info.Left.IsEmpty) borders[ReoGridBorderPos.Left] = info.Left;
				if (!info.Right.IsEmpty) borders[ReoGridBorderPos.Right] = info.Right;
				if (!info.Top.IsEmpty) borders[ReoGridBorderPos.Top] = info.Top;
				if (!info.Bottom.IsEmpty) borders[ReoGridBorderPos.Bottom] = info.Bottom;
				if (!info.InsideHorizontal.IsEmpty) borders[ReoGridBorderPos.InsideHorizontal] = info.InsideHorizontal;
				if (!info.InsideVertical.IsEmpty) borders[ReoGridBorderPos.InsideVertical] = info.InsideVertical;

				rows = grid.SelectionRange.Rows > 2 ? 2 : grid.SelectionRange.Rows;
				cols = grid.SelectionRange.Cols > 2 ? 2 : grid.SelectionRange.Cols;

				mixBorders |= info.NonUniformPos;
			}
		}

		private struct BorderLineItem
		{
			internal Rectangle rect;
			internal ReoGridBorderPos pos;
			internal bool hover;
			//internal BorderLineStyle style;
		}

		private ReoGridBorderPos mixBorders = ReoGridBorderPos.None;

		public ReoGridBorderPos MixBorders
		{
			get { return mixBorders; }
			set { mixBorders = value; }
		}
	}
	#endregion
}
