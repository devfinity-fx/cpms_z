/*****************************************************************************
 * 
 * ReoGrid - .NET Spreadsheet Control
 * 
 * http://reogrid.codeplex.com/
 * 
 * FontDropDownControl - Show font sample as every list item
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

using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using unvell.Common;

namespace unvell.UIControls
{
	public partial class FontDropDownControl : ComboBox
	{
		public FontDropDownControl()
		{
			DropDownStyle = ComboBoxStyle.DropDownList;
			DrawMode = DrawMode.OwnerDrawFixed;
			DropDownHeight = 500;
			ItemHeight = 20;
			DrawItem += new DrawItemEventHandler(ComboBox_DrawItem);

			foreach (FontFamily family in FontFamily.Families)
			{
				base.Items.Add(family);
			}

			//if (base.Items.Count > 0) Text = Font.FontFamily.Name;
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new object Items { get { return null; } set { } }

		void ComboBox_DrawItem(object sender, DrawItemEventArgs e)
		{
			Graphics g = e.Graphics;

			e.DrawBackground();

			if (e.Index >= 0 && e.Index < base.Items.Count)
			{
				FontToolkit.DrawFontItem(g, (FontFamilyInfo)base.Items[e.Index], e.Bounds,
					(e.State & DrawItemState.Selected) == DrawItemState.Selected);
			}
		}

		//protected override void OnTextChanged(EventArgs e)
		//{
		//  if (string.IsNullOrEmpty(Text))
		//  {
		//    base.SelectedIndex = -1;
		//  }
		//  else
		//  {
		//    foreach (FontFamilyInfo item in base.Items)
		//    {
		//      if (item.IsFamilyName(Text))
		//      {
		//        base.SelectedItem = item;
		//        SelectionStart = Text.Length;
		//        break;
		//      }
		//    }
		//  }
		//}

		public FontFamilyInfo SelectedFontFamily
		{
			get
			{
				return ((FontFamilyInfo)base.SelectedItem);
			}
			set
			{
				base.SelectedItem = value;
			}
		}
	}
}
