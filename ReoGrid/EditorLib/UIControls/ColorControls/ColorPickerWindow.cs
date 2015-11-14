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

/*****************************************************************************
 * Color Picker Popup Panel
 *
 * Author: Jing Lu <dujid0 at gmail.com>
 * Copyright (c) 2012-2014 unvell.com, all rights reserved.
 *
 ****************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Diagnostics;
using System.ComponentModel;

using unvell.Common;
using unvell.Common.Win32Lib;

namespace unvell.UIControls
{
	internal class ColorPickerWindow : ToolStripDropDown
	{
		private ColorPickerPanel colorPickerPanel = new ColorPickerPanel();

		private ToolStripControlHost controlHost;

		public ColorPickerWindow()
			: base()
		{
			this.TabStop = false;
			this.Margin = this.Padding = new Padding(1);
			this.AutoSize = false;
		
			colorPickerPanel.Dock = DockStyle.Fill;
			colorPickerPanel.Location = new Point(0, 0);
			colorPickerPanel.ColorPicked += (s, e) =>
			{
				if (ColorPicked != null) ColorPicked(this, null);
			};

			controlHost = new ToolStripControlHost(colorPickerPanel);
			controlHost.AutoSize = false;

			Items.Add(controlHost);

			this.Size = new Size(172, 220);
		}

		protected override void OnMouseMove(MouseEventArgs mea)
		{
			base.OnMouseMove(mea);

			Debug.WriteLine(mea.Location);
		}

		public AbstractColor CurrentColor
		{
			get { return colorPickerPanel.CurrentColor; }
			set { colorPickerPanel.CurrentColor = value; }
		}

		public Color SolidColor
		{
			get { return colorPickerPanel.SolidColor; }
			set { colorPickerPanel.SolidColor = value; }
		}

		protected override void OnResize(EventArgs e)
		{
			base.OnResize(e);
			if (controlHost != null) controlHost.Size = new Size(ClientRectangle.Width - 2, ClientRectangle.Height - 3);
		}

		internal event EventHandler ColorPicked;
	}
}
