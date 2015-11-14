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

using unvell.ReoGrid.Actions;

namespace unvell.ReoGrid.PropertyPages
{
	internal interface IPropertyPage
	{
		ReoGridControl Grid { get; set; }
		void LoadPage();
		RGReusableAction CreateUpdateAction();
		event EventHandler Done;
	}

	public partial class PropertyForm : Form
	{
		private ReoGridControl grid;

		public ReoGridControl Grid
		{
			get { return grid; }
			set { SetGrid(value); }
		}

		private static int lastTabPageIndex = 0;

		public PropertyForm() : this(null) { }

		public PropertyForm(ReoGridControl grid)
		{
			this.grid = grid;
			
			InitializeComponent();

			numberPage.Done += new EventHandler(IPropertyPage_Done);
			
			tabControl1.SelectedIndex = lastTabPageIndex;
		}

		void IPropertyPage_Done(object sender, EventArgs e)
		{
			btnOK.PerformClick();
		}

		protected override void OnCreateControl()
		{
			base.OnCreateControl();

			if (grid != null)
			{
				SetGrid(grid);
			}
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			Point p = new Point();
			unvell.Common.Win32Lib.Win32.GetCursorPos(ref p);

			this.Left = p.X - (Width >> 1);
			this.Top = p.Y - (Height >> 1);

			if (this.Left < 0) this.Left = 0;
			if (this.Top < 0) this.Top = 0;
		}

		public void SetGrid(ReoGridControl grid)
		{
			ReoGridCell cell = (grid == null) ? null : (grid.GetCell(grid.SelectionRange.StartPos));
			ProcessAllPages(p =>
			{
				p.Grid = grid;
				p.LoadPage();
			});
		}

		private void ProcessAllPages(Action<IPropertyPage> handler)
		{
			for (int i = 0; i < tabControl1.TabPages.Count; i++)
			{
				foreach (Control ctrl in tabControl1.TabPages[i].Controls)
				{
					if (ctrl is IPropertyPage)
					{
						handler((IPropertyPage)ctrl);
					}
				}
			}
		}

		private void btnCancel_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void btnOK_Click(object sender, EventArgs e)
		{
			if (grid != null)
			{
				RGReusableActionGroup actionGroup = new RGReusableActionGroup(grid.SelectionRange);

				ProcessAllPages(p =>
				{
					RGReusableAction action = p.CreateUpdateAction();
					if (action != null) actionGroup.Actions.Add(action);
				});

				grid.DoAction(actionGroup);
			}

			Close();
		}

		protected override void OnClosing(CancelEventArgs e)
		{
			base.OnClosing(e);
			lastTabPageIndex = tabControl1.SelectedIndex;
		}
	}
}
