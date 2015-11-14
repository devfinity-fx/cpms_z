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
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using unvell.Common;

namespace unvell.ReoGrid.CellTypes
{
	// Built-in Cell Types

	#region Button
	public class ButtonCell : CellBody
	{
		private string defaultText;

		public ButtonCell()
		{
		}

		public ButtonCell(string defaultText)
		{
			this.defaultText = defaultText;
		}

		public override object GetData()
		{
			return this.defaultText;
		}

		public override void OnSetup(ReoGridControl ctrl, ReoGridCell cell)
		{
			// set text to center
			cell.Style.HAlign = ReoGridHorAlign.Center;
			cell.Style.VAlign = ReoGridVerAlign.Middle;
		}

		public override void OnPaint(RGDrawingContext dc)
		{
			// draw background
			ControlPaint.DrawButton(dc.Graphics, Bounds, IsPressed ? ButtonState.Pushed : ButtonState.Normal);

			// get style
			var style = dc.Cell.Style;
			var textColor = style.TextColor;

			// call core text drawing method
			dc.DrawCellText();
		}

		/// <summary>
		/// Check whether current is pressed 
		/// </summary>
		public bool IsPressed { get; set; }

		public override bool OnMouseDown(RGCellMouseEventArgs e)
		{
			IsPressed = true;
			return true;
		}

		public override bool OnMouseUp(RGCellMouseEventArgs e)
		{
			if (IsPressed)
			{
				if (Bounds.Contains(e.CursorPosition)
					&& Click != null)
				{
					Click(this, null);
				}

				IsPressed = false;
				return true;
			}
			else
				return false;
		}

		public override bool OnStartEdit(ReoGridCell cell)
		{
			return false;
		}

		public override bool OnKeyDown(KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Space)
			{
				IsPressed = true;
				return true;
			}
			else
				return false;
		}

		public override bool OnKeyUp(KeyEventArgs e)
		{
			if (IsPressed)
			{
				IsPressed = false;
				return true;
			}
			else
			{
				return false;
			}
		}

		public event EventHandler Click;
	}
	#endregion

	#region Progress
	public class ProgressCell : CellBody
	{
		public Color TopColor { get; set; }
		public Color BottomColor { get; set; }

		public ProgressCell()
		{
			TopColor = ControlPaint.Light(SystemColors.ActiveCaption);
			BottomColor = SystemColors.ActiveCaption;
		}

		public override void OnPaint(RGDrawingContext dc)
		{
			int value = 0;
			int.TryParse(dc.Cell.Display, out value);

			if (value > 0)
			{
				Graphics g = dc.Graphics;

				Rectangle rect = new Rectangle(Bounds.Left, Bounds.Top + 1,
					(int)(Math.Round(Bounds.Width / 100f * value)), Bounds.Height - 1);

				using (LinearGradientBrush lgb = new LinearGradientBrush(rect, TopColor, BottomColor, 90f))
				{
					g.PixelOffsetMode = PixelOffsetMode.Half;
					g.FillRectangle(lgb, rect);
					g.PixelOffsetMode = PixelOffsetMode.Default;
				}
			}
		}
	}
	#endregion // Progress

	#region Hyperlink
	public class HyperlinkCell : CellBody
	{
		public string LinkURL { get; set; }

		public HyperlinkCell()
		{
		}

		public HyperlinkCell(string defaultURL)
		{
			this.LinkURL = defaultURL;
		}

		public HyperlinkCell(string defaultURL, bool autoNavigate)
		{
			this.LinkURL = defaultURL;
			this.AutoNavigate = autoNavigate;
		}

		public override object GetData()
		{
			return this.LinkURL;
		}

		public bool IsPressed { get; set; }

		public override void OnSetup(ReoGridControl ctrl, ReoGridCell cell)
		{
			cell.Style.TextColor = Color.Blue;
			cell.Style.Underline = true;
		}

		public override bool OnMouseDown(RGCellMouseEventArgs e)
		{
			IsPressed = true;

			e.Cell.RenderColor = Color.Red;

			return true;
		}

		public override bool OnMouseUp(RGCellMouseEventArgs e)
		{
			if (IsPressed)
			{
				if (Bounds.Contains(e.CursorPosition))
				{
					PerformClick();
				}
			}

			IsPressed = false;

			e.Cell.RenderColor = e.Cell.Style.TextColor;

			return true;
		}

		private Cursor backupCursor = null;

		public override bool OnMouseEnter(RGCellMouseEventArgs e)
		{
			// save current cursor
			backupCursor = e.Grid.CellsSelectionCursor;

			// set cursor to hand
			e.Grid.CellsSelectionCursor = Cursors.Hand;

			return false;
		}

		public override bool OnMouseLeave(RGCellMouseEventArgs e)
		{
			// restore last cursor
			e.Grid.CellsSelectionCursor = backupCursor;

			return false;
		}

		public override bool OnKeyDown(KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Space)
			{
				IsPressed = true;
				return true;
			}
			else
			{
				return false;
			}
		}

		public override bool OnKeyUp(KeyEventArgs e)
		{
			if (IsPressed)
			{
				IsPressed = false;
				PerformClick();
				return true;
			}
			else
			{
				return false;
			}
		}

		public override void OnLostFocus(ReoGridCell cell)
		{
			if (IsPressed)
			{
				IsPressed = false;
			}
		}

		public void PerformClick()
		{
			if (AutoNavigate)
			{
				System.Diagnostics.Process.Start(LinkURL);
			}

			if (Click != null)
			{
				Click(this, null);
			}
		}

		public override object OnSetData(object data)
		{
			LinkURL = Convert.ToString(data);
			return data;
		}

		public event EventHandler Click;

		public bool AutoNavigate { get; set; }
	}
	#endregion // Hyperlink

	#region Check box
	public class CheckBoxCell : CellBody
	{
		public CheckBoxCell()
		{
			RadioButtonSize = 20;
		}

		public Rectangle CheckRect { get; set; }

		public int RadioButtonSize { get; set; }

		public ButtonState ButtonState { get; set; }

		public override void OnBoundsChanged(ReoGridCell cell)
		{
			int minButtonSize = Math.Min(Math.Min(Bounds.Width, Bounds.Height), RadioButtonSize);

			int x = 0, y = 0;
			switch (cell.Style.HAlign)
			{
				case ReoGridHorAlign.Left:
					x = Bounds.X + 1;
					break;

				case ReoGridHorAlign.Center:
					x = Bounds.X + (Bounds.Width - minButtonSize) / 2;
					break;

				case ReoGridHorAlign.Right:
					x = Bounds.Right - minButtonSize - 1;
					break;
			}

			switch (cell.Style.VAlign)
			{
				case ReoGridVerAlign.Top:
					y = Bounds.Y + 1;
					break;
				case ReoGridVerAlign.Middle:
					y = Bounds.Y + (Bounds.Height - minButtonSize) / 2;
					break;
				case ReoGridVerAlign.Bottom:
					y = Bounds.Bottom - minButtonSize - 1;
					break;
			}

			CheckRect = new Rectangle(x, y, minButtonSize, minButtonSize);
		}

		public override bool OnMouseDown(RGCellMouseEventArgs e)
		{
			if (CheckRect.Contains(e.CursorPosition))
			{
				ButtonState |= System.Windows.Forms.ButtonState.Pushed;
				return true;
			}
			else
				return false;
		}

		public override bool OnMouseUp(RGCellMouseEventArgs e)
		{
			ButtonState &= ~System.Windows.Forms.ButtonState.Pushed;

			if (CheckRect.Contains(e.CursorPosition))
			{
				ToggleCheckStatus();

				if (Click != null)
				{
					Click(this, null);
				}
			}

			return true;
		}

		public event EventHandler Click;

		public override void OnPaint(RGDrawingContext dc)
		{
			ControlPaint.DrawCheckBox(dc.Graphics, CheckRect, ButtonState);
		}

		public event EventHandler CheckChanged;

		public override bool OnStartEdit(ReoGridCell cell)
		{
			return false;
		}

		public void ToggleCheckStatus()
		{
			Checked = !Checked;

			if (CheckChanged != null)
			{
				CheckChanged(this, null);
			}
		}

		public bool Checked
		{
			get
			{
				return (ButtonState & System.Windows.Forms.ButtonState.Checked)
					== System.Windows.Forms.ButtonState.Checked;
			}
			set
			{
				if (value)
				{
					ButtonState |= System.Windows.Forms.ButtonState.Checked;
				}
				else
				{
					ButtonState &= ~System.Windows.Forms.ButtonState.Checked;
				}
			}
		}

		public override bool OnKeyDown(KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Space)
			{
				ButtonState |= System.Windows.Forms.ButtonState.Pushed;
				return true;
			}
			else
			{
				return false;
			}
		}

		public override bool OnKeyUp(KeyEventArgs e)
		{
			if ((ButtonState & System.Windows.Forms.ButtonState.Pushed)
				== System.Windows.Forms.ButtonState.Pushed)
			{
				ButtonState &= ~System.Windows.Forms.ButtonState.Pushed;
				if (e.KeyCode == Keys.Space)
				{
					ToggleCheckStatus();
				}

				return true;
			}
			else
			{
				return false;
			}
		}
	}
	#endregion // Check box

	#region Radio button

	public class RadioButtonGroup
	{
		private List<RadioButtonCell> radios = new List<RadioButtonCell>();

		public void AddRadioButton(RadioButtonCell cell)
		{
			if (cell == null) return;

			if (!radios.Contains(cell))
			{
				radios.Add(cell);
			}

			if (cell.RadioGroup != this)
			{
				cell.RadioGroup = this;
			}
		}

		internal void ClearAllButMe(RadioButtonCell cell)
		{
			foreach (var radio in radios)
			{
				if (radio != cell) radio.ButtonState = ButtonState.Normal;
			}
		}

		public bool Contains(RadioButtonCell cell)
		{
			return radios.Contains(cell);
		}
	}

	public class RadioButtonCell : CellBody
	{
		public RadioButtonCell()
		{
			RadioButtonSize = 20;
		}

		public ReoGridControl Control { get; set; }

		public ReoGridCell Cell { get; set; }

		public override void OnSetup(ReoGridControl ctrl, ReoGridCell cell)
		{
			this.Control = ctrl;
			this.Cell = cell;
		}

		private RadioButtonGroup radioGroup { get; set; }
		public RadioButtonGroup RadioGroup
		{
			get { return radioGroup; }
			set
			{
				if (value == null)
				{
					this.RadioGroup = null;
				}
				else
				{
					if (!value.Contains(this))
					{
						value.AddRadioButton(this);
					}

					this.radioGroup = value;
				}
			}
		}

		public Rectangle CheckRect { get; set; }

		public int RadioButtonSize { get; set; }

		private ButtonState buttonState;

		public ButtonState ButtonState
		{
			get
			{
				return buttonState;
			}
			set
			{
				buttonState = value;

				if (RadioGroup != null
					&& ((value & System.Windows.Forms.ButtonState.Checked)
					== System.Windows.Forms.ButtonState.Checked))
				{
					RadioGroup.ClearAllButMe(this);
				}

				if (CheckChanged != null)
				{
					CheckChanged(this, null);
				}
			}
		}

		public override void OnBoundsChanged(ReoGridCell cell)
		{
			int minButtonSize = Math.Min(Math.Min(Bounds.Width, Bounds.Height), RadioButtonSize);

			int x = 0, y = 0;
			switch (cell.Style.HAlign)
			{
				case ReoGridHorAlign.Left:
					x = Bounds.X + 1;
					break;

				case ReoGridHorAlign.Center:
					x = Bounds.X + (Bounds.Width - minButtonSize) / 2;
					break;

				case ReoGridHorAlign.Right:
					x = Bounds.Right - minButtonSize - 1;
					break;
			}

			switch (cell.Style.VAlign)
			{
				case ReoGridVerAlign.Top:
					y = Bounds.Y + 1;
					break;
				case ReoGridVerAlign.Middle:
					y = Bounds.Y + (Bounds.Height - minButtonSize) / 2;
					break;
				case ReoGridVerAlign.Bottom:
					y = Bounds.Bottom - minButtonSize - 1;
					break;
			}

			CheckRect = new Rectangle(x, y, minButtonSize, minButtonSize);
		}

		public override bool OnMouseDown(RGCellMouseEventArgs e)
		{
			if (CheckRect.Contains(e.CursorPosition))
			{
				buttonState |= System.Windows.Forms.ButtonState.Pushed;
				return true;
			}
			else
				return false;
		}

		public override bool OnMouseUp(RGCellMouseEventArgs e)
		{
			buttonState &= ~System.Windows.Forms.ButtonState.Pushed;
			
			if (CheckRect.Contains(e.CursorPosition))
			{
				Toggle();

				if (Click != null)
				{
					Click(this ,null);
				}
			}

			return true;
		}

		public event EventHandler Click;

		public void Toggle()
		{
			if (RadioGroup == null)
			{
				if ((buttonState & ButtonState.Checked) == ButtonState.Checked)
				{
					ButtonState = buttonState & ~ButtonState.Checked;
				}
				else
				{
					ButtonState = buttonState | System.Windows.Forms.ButtonState.Checked;
				}
			}
			else
			{
				ButtonState = buttonState | System.Windows.Forms.ButtonState.Checked;
			}
		}

		public override void OnPaint(RGDrawingContext dc)
		{
			ControlPaint.DrawRadioButton(dc.Graphics, CheckRect, ButtonState);
		}

		public override bool OnStartEdit(ReoGridCell cell)
		{
			return false;
		}

		public override bool OnKeyDown(KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Space)
			{
				buttonState |= System.Windows.Forms.ButtonState.Pushed;
				return true;
			}
			else
				return false;
		}

		public override bool OnKeyUp(KeyEventArgs e)
		{
			if ((buttonState & System.Windows.Forms.ButtonState.Pushed)
				== System.Windows.Forms.ButtonState.Pushed)
			{
				buttonState &= ~System.Windows.Forms.ButtonState.Pushed;
				Toggle();

				if (Click != null)
				{
					Click(this, null);
				}

				return true;
			}
			else
				return false;
		}

		public event EventHandler CheckChanged;
	}
	#endregion // Check box

	#region Dropdown
	public class DropdownCell : CellBody
	{
		public ReoGridControl GridInstance { get; set; }
		public ReoGridCell Cell { get; set; }
		private DropdownWindow dropdown;

		public DropdownCell()
		{
			DropdownButtonSize = new Size(20, 20);
			DropdownHeight = 200;
			MinimumDropdownWidth = 120;

			dropdown = new DropdownWindow();
			dropdown.VisibleChanged += dropdown_VisibleChanged;
			dropdown.ListBox.Click += ListBox_Click;
			dropdown.ListBox.KeyDown += ListBox_KeyDown;
		}

		void ListBox_KeyDown(object sender, KeyEventArgs e)
		{
			if (dropdown.Visible && GridInstance != null)
			{
				if ((e.KeyCode == Keys.Enter || e.KeyCode == Keys.Space))
				{
					SetSelectedItem(dropdown.ListBox.SelectedItem);
					PullUp();
				}
				else if (e.KeyCode == Keys.Escape)
				{
					PullUp();
				}
			}
		}

		public override void OnSetup(ReoGridControl ctrl, ReoGridCell cell)
		{
			this.GridInstance = ctrl;
			this.Cell = cell;
		}

		void ListBox_Click(object sender, EventArgs e)
		{
			if (dropdown.Visible && GridInstance != null)
			{
				SetSelectedItem(dropdown.ListBox.SelectedItem);
				PullUp();
			}
		}

		public object SelectedItem
		{
			get
			{
				return dropdown.ListBox.SelectedItem;
			}
			set
			{
				dropdown.ListBox.SelectedItem = value;
			}
		}

		public int SelectedIndex
		{
			get
			{
				return dropdown.ListBox.SelectedIndex;
			}
			set
			{
				dropdown.ListBox.SelectedIndex = value;
			}
		}

		private void SetSelectedItem(object obj)
		{
			GridInstance.SetCellData(Cell, obj);

			if (SelectedItemChanged != null)
			{
				SelectedItemChanged(this, null);
			}
		}

		public event EventHandler SelectedItemChanged;

		void dropdown_VisibleChanged(object sender, EventArgs e)
		{
			if (!dropdown.Visible)
			{
				PullUp();
			}
		}

		public DropdownCell(object[] candidates)
			: this()
		{
			this.Candidates = candidates;
		}

		public object[] Candidates { get; set; }

		public Size DropdownButtonSize { get; set; }

		public int DropdownHeight { get; set; }
		public int MinimumDropdownWidth { get; set; }

		private Rectangle dropdownButtonRect = new Rectangle(0, 0, 20, 20);

		public override void OnBoundsChanged(ReoGridCell cell)
		{
			dropdownButtonRect.X = Bounds.Right - DropdownButtonSize.Width;
			dropdownButtonRect.Y = Bounds.Top + (Bounds.Height - DropdownButtonSize.Height) / 2;
		}

		public override void OnPaint(RGDrawingContext dc)
		{
			// call core draw text method
			dc.DrawCellText();

			ControlPaint.DrawComboButton(dc.Graphics, dropdownButtonRect,
				IsDropdown ? ButtonState.Pushed : ButtonState.Normal);
		}

		public override void OnLostFocus(ReoGridCell cell)
		{
			PullUp();
		}

		public bool IsDropdown { get; set; }

		public override bool OnMouseDown(RGCellMouseEventArgs e)
		{
			if (IsDropdown)
			{
				PullUp();
			}
			else
			{
				PushDown();
			}

			return true;
		}

		private void PushDown()
		{
			dropdown.owner = this.GridInstance;

			dropdown.ListBox.Items.Clear();
			dropdown.ListBox.Items.AddRange(Candidates);
			dropdown.ListBox.SelectedItem = this.Cell.Data;
			dropdown.Width = Math.Max((int)Math.Round(Bounds.Width * this.GridInstance.ScaleFactor), MinimumDropdownWidth);
			dropdown.Height = DropdownHeight;

			var p = this.GridInstance.ViewportController.CellPositionToControl(this.Cell.Pos);
			dropdown.Show(this.GridInstance, new Point(p.X, (int)Math.Round(p.Y + Bounds.Height * this.GridInstance.ScaleFactor)));
			dropdown.ListBox.Focus();

			IsDropdown = true;
		}

		private void PullUp()
		{
			dropdown.Hide();

			IsDropdown = false;

			if (dropdown.owner != null)
			{
				dropdown.owner.InvalidateSheet();
			}
		}

		public override bool OnStartEdit(ReoGridCell cell)
		{
			PushDown();
			return false;
		}

		#region Dropdown Window

		private class DropdownWindow : ToolStripDropDown
		{
			internal ReoGridControl owner;
			private DropdownListbox listbox;
			private ToolStripControlHost controlHost;
			internal DropdownWindow()
				: base()
			{
				listbox = new DropdownListbox();
				listbox.Dock = DockStyle.Fill;
				listbox.BorderStyle = BorderStyle.None;
				BackColor = listbox.BackColor;
				AutoSize = false;
				TabStop = true;
				Items.Add(controlHost = new ToolStripControlHost(listbox));
				controlHost.Margin = controlHost.Padding = new Padding(0);
				controlHost.AutoSize = false;
				listbox.MouseMove += (sender, e) =>
				{
					int index =listbox.IndexFromPoint(e.Location);
					if (index != -1) listbox.SelectedIndex = index;
				};
			}

			private class DropdownListbox : ListBox
			{
				protected override bool ProcessDialogKey(Keys keyData)
				{
					if (keyData == Keys.Escape)
						return false;
					else
						return base.ProcessDialogKey(keyData);
				}
			}

			protected override void OnVisibleChanged(EventArgs e)
			{
				base.OnVisibleChanged(e);
				if (!Visible)
				{
					owner.EndEdit(ReoGridEndEditReason.Cancel);
				}
			}

			internal ListBox ListBox { get { return listbox; } }

			protected override void OnResize(EventArgs e)
			{
				base.OnResize(e);
				if (controlHost != null) controlHost.Size = new Size(ClientRectangle.Width - 2, ClientRectangle.Height - 2);
			}

			private string orginalText;
			internal void SetItems(string[] items, string defaultSelected)
			{
				orginalText = defaultSelected;
				listbox.Items.Clear();
				listbox.Items.AddRange(items);
				Height = listbox.Font.Height * items.Count() + 10;
				listbox.SelectedItem = defaultSelected;
			}
			internal IntPtr GetListboxHandle()
			{
				return listbox.Handle;
			}
		}
		#endregion // Dropdown Window
	}
	#endregion // Dropdown

	#region Image
	public class ImageCell : CellBody
	{
		public Image Image { get; set; }

		public ImageCell() { }

		public ImageCell(Image image)
		{
			this.Image = image;
		}

		public override void OnPaint(RGDrawingContext dc)
		{
			if (Image != null)
			{
				dc.Graphics.DrawImage(Image, Bounds);
			}

			dc.DrawCellText();
		}
	}
	#endregion // Image
}
