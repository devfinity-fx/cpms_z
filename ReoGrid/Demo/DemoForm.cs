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
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using unvell.Common;
using unvell.ReoGrid.Demo.CustomCells;
using unvell.ReoGrid.Demo.Data;
using unvell.ReoGrid.Demo.Features;
using unvell.ReoGrid.Demo.Other;
using unvell.ReoGrid.Demo.Scripts;
using unvell.ReoGrid.Editor;

namespace unvell.ReoGrid.Demo
{
	public partial class DemoForm : Form
	{
		public DemoForm()
		{
			InitializeComponent();
		}

		private void DemoForm_Load(object sender, EventArgs e)
		{
			labVersion.Text = "Version " + ProductVersion.ToString();
		}

		private static void Preload()
		{
			try
			{
				Cursor.Current = Cursors.AppStarting;

				// preload to improve performance when first time to open demo
				object preload = ResourcePoolManager.Instance;
				preload = BorderPainter.Instance;

				unvell.ReoScript.ScriptRunningMachine srm = new ReoScript.ScriptRunningMachine();
				srm.Run("eval('true');");

				ReoGridEditor grid = new ReoGridEditor();
				grid.Dispose();
			}
			catch { }
			finally
			{
				Cursor.Current = Cursors.Default;
			}
		}

		protected override void OnShown(EventArgs e)
		{
			base.OnShown(e);

			try
			{
				// async-preload 
				((Action)Preload).BeginInvoke(null, null);
			}
			catch { }
		}

		private void button2_Click(object sender, EventArgs e)
		{
			Open("order_sample.rgf");
		}

		private void Open(string filename)
		{
			try
			{
				Cursor = Cursors.WaitCursor;

				using (ReoGridEditor editor = new ReoGridEditor())
				{
#if DEBUG
					editor.CurrentFilePath = System.IO.Path.Combine("..\\..\\..\\Samples\\", filename);
#else
					editor.CurrentFilePath = filename;
#endif
					editor.ShowDialog();
				}
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		private void button1_Click(object sender, EventArgs e)
		{
			Open("calendar_2008_1.rgf");
		}

		private void button3_Click(object sender, EventArgs e)
		{
			Open("project_cost_summary.rgf");
		}

		private void button4_Click(object sender, EventArgs e)
		{
			using (GridForm gf = new GridForm())
			{
				gf.Open("calendar_2013.rgf", (grid) =>
				{
					grid.SetSettings(ReoGridSettings.View_ShowRowHeader |
						ReoGridSettings.View_ShowColumnHeader, false);

					grid.SetSettings(ReoGridSettings.Readonly, true);

					grid.SelectionMode = ReoGridSelectionMode.None;
				});

				gf.ShowDialog();
			}
		}

		private void button5_Click(object sender, EventArgs e)
		{
			Open("Financial_Ratios.rgf");
		}

		private void button6_Click(object sender, EventArgs e)
		{
			Open("background_color_patterns.rgf");
		}

		private void button7_Click(object sender, EventArgs e)
		{
			Open("border_styles.rgf");
		}

		private void button8_Click(object sender, EventArgs e)
		{
			Open("merged_range.rgf");
		}

		private void button9_Click(object sender, EventArgs e)
		{
			Open("Maze.rgf");
		}

		private void button10_Click(object sender, EventArgs e)
		{
			Open("cell_format.rgf");
		}

		private void button14_Click(object sender, EventArgs e)
		{
			using (var f = new OnlyNumberInputForm()) f.ShowDialog();
		}

		private void button15_Click(object sender, EventArgs e)
		{
			Open("change_colors.rgf");
		}

		private void btnMergeCells_Click(object sender, EventArgs e)
		{
			using (var f = new MergeCellsForm()) f.ShowDialog();
		}

		private void button16_Click(object sender, EventArgs e)
		{
			using (var f = new SetEditableRangeForm()) f.ShowDialog();
		}

		private void button18_Click(object sender, EventArgs e)
		{
			using (var f = new RunScriptForm()) f.ShowDialog();
		}

		private void button21_Click(object sender, EventArgs e)
		{
			using (var f = new PickRangeForm()) f.ShowDialog();
		}

		private void button19_Click(object sender, EventArgs e)
		{
			using (var f = new ClipboardEventForm()) f.ShowDialog();
		}

		private void button17_Click(object sender, EventArgs e)
		{
			using (var f = new HandleEventsForm()) f.ShowDialog();
		}

		private void button20_Click(object sender, EventArgs e)
		{
			using (var f = new DataFormatForm()) f.ShowDialog();
		}

		private void projectHomepageToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Process.Start("http://reogrid.codeplex.com/");
		}

		private void button23_Click(object sender, EventArgs e)
		{
			using (var f = new ZoomForm()) f.ShowDialog();
		}

		private void button22_Click(object sender, EventArgs e)
		{
			try
			{
				Cursor = Cursors.Default;
				new ReoGridEditor().Show();
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		private void button24_Click(object sender, EventArgs e)
		{
			using (var f = new AddCustomizeFunctionForm()) f.ShowDialog();
		}

		private void button25_Click(object sender, EventArgs e)
		{
			using (ReoGridEditor editor = new ReoGridEditor())
			{
				editor.Grid.Script =
@"// Joke sample: reoquery

function reo(cell) {
  this.cell = cell;
  
  this.val = function(str) { 
    if(__args__.length == 0) {
      return this.cell.data;
    } else {
      this.cell.data = str;
      return this;
    }
  };
  
  this.style = function(key, value) {
    if (__args__.length == 1) {
      return this.cell.style[key];
    } else {
      this.cell.style[key] = value;
      return this;
    }
  };
}

script.$ = function(r, c) {
  return new reo( c == null ? grid.getCell(r) : grid.getCell(r, c));
};

// call grid like jquery
$('B4').val('hello').style('backgroundColor', 'yellow');
$(3, 2).val('world!').style('backgroundColor', 'lightgreen');


";
				editor.Grid[1, 1] = "Please see the script editor!";
				editor.NewDocumentOnLoad = false;
				editor.ShowDialog();
			}
		}

		private void button26_Click(object sender, EventArgs e)
		{
			using (var f = new CellsEventForm()) f.ShowDialog();
		}

		private void button27_Click(object sender, EventArgs e)
		{
			customCellsContextMenuStrip.Show(button27, new Point(0, button27.Height));
		}

		private void numericProgressToolStripMenuItem_Click(object sender, EventArgs e)
		{
			using (var f = new NumericProgressForm()) f.ShowDialog();
		}

		private void slideCellToolStripMenuItem_Click(object sender, EventArgs e)
		{
			using (var f = new SlideCellForm()) f.ShowDialog();
		}

		private void dropdownButton1_Click(object sender, EventArgs e)
		{
			controlAppearanceContextMenuStrip.Show(dropdownButton1, new Point(0, dropdownButton1.Height));
		}

		private void goldSilverToolStripMenuItem_Click(object sender, EventArgs e)
		{
			using (ReoGridEditor editor = new ReoGridEditor())
			{
				ReoGridControlStyle rgcs = new ReoGridControlStyle(Color.White, Color.DarkOrange, false);
				rgcs.SetColor(ReoGridControlColors.GridBackground, Color.DimGray);
				rgcs.SetColor(ReoGridControlColors.GridLine, Color.Gray);
				editor.Grid.SetControlStyle(rgcs);
				editor.ShowDialog();
			}
		}

		private void lightGreenToolStripMenuItem_Click(object sender, EventArgs e)
		{
			using (ReoGridEditor editor = new ReoGridEditor())
			{
				ReoGridControlStyle rgcs = new ReoGridControlStyle(Color.LightGreen, Color.White, false);
				rgcs.SetColor(ReoGridControlColors.SelectionBorder, Color.Green);
				rgcs.SetColor(ReoGridControlColors.SelectionFill, Color.FromArgb(30, Color.Green));
				rgcs.SetColor(ReoGridControlColors.GridLine, Color.FromArgb(200, 255, 200));
				rgcs.SetColor(ReoGridControlColors.GridBackground, Color.White);
				editor.Grid.SetControlStyle(rgcs);

				editor.ShowDialog();
			}
		}

		private void snowWhiteToolStripMenuItem_Click(object sender, EventArgs e)
		{
			using (ReoGridEditor editor = new ReoGridEditor())
			{
				ReoGridControlStyle rgcs = new ReoGridControlStyle(Color.LightSkyBlue, Color.White, false);
				rgcs.SetColor(ReoGridControlColors.SelectionBorder, Color.SkyBlue);
				rgcs.SetColor(ReoGridControlColors.SelectionFill, Color.FromArgb(30, Color.Blue));
				rgcs.SetColor(ReoGridControlColors.GridLine, Color.FromArgb(220, 220, 255));
				rgcs.SetColor(ReoGridControlColors.GridBackground, Color.White);
				editor.Grid.SetControlStyle(rgcs);

				editor.ShowDialog();
			}
		}

		private void button11_Click(object sender, EventArgs e)
		{
			using (var f = new LargeDataForm()) f.ShowDialog();
		}

		private void button13_Click(object sender, EventArgs e)
		{
			Open("outline.rgf");
		}

		private void reportBugToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Process.Start("http://reogrid.codeplex.com/workitem/list/basic");
		}
			
		private void lnkReportBug_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			Process.Start("http://reogrid.codeplex.com/workitem/list/basic");
		}	

		private void button29_Click(object sender, EventArgs e)
		{
			using (var f = new BuiltInTypesForm()) f.ShowDialog();
		}

		private void button30_Click(object sender, EventArgs e)
		{
			using (var f = new OutlineWithFreezeForm()) f.ShowDialog();
		}

		private void button31_Click(object sender, EventArgs e)
		{
			using (var f = new AnimationCellForm()) f.ShowDialog();
		}

		private void button12_Click(object sender, EventArgs e)
		{

		}

		private void button28_Click(object sender, EventArgs e)
		{

		}

		private void button32_Click(object sender, EventArgs e)
		{
			using (var f = new MaximumGridForm()) f.ShowDialog();
		}

		private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
		{
			new AboutForm().ShowDialog();
		}

		private void button33_Click(object sender, EventArgs e)
		{
			using (var f = new NumericProgressForm()) f.ShowDialog();
		}

		private void button34_Click(object sender, EventArgs e)
		{
			using (var f = new SlideCellForm()) f.ShowDialog();
		}

	}
}
