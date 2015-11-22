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

using unvell.ReoScript;

namespace unvell.ReoGrid.Demo.Scripts
{
	public partial class HandleEventsForm : Form
	{
		public HandleEventsForm()
		{
			InitializeComponent();

			grid.SetRows(14);
			grid.SetCols(6);

			for (int r = 0; r < 10; r++)
			{
				for (int c = 0; c < 5; c++)
				{
					grid[r, c] = (r + 1) * (c + 1);
				}
			}

#if EX_SCRIPT

			chkCellBeforeEdit.CheckedChanged += UpdateEventHandlers;
			chkCellMouseDown.CheckedChanged += UpdateEventHandlers;
			chkCellMouseUp.CheckedChanged += UpdateEventHandlers;
			chkCellDataChanged.CheckedChanged += UpdateEventHandlers;
			chkSelectionChange.CheckedChanged += UpdateEventHandlers;
			chkSelectionMoveNext.CheckedChanged += UpdateEventHandlers;
			chkOnload.CheckedChanged += UpdateEventHandlers;
			chkUnload.CheckedChanged += UpdateEventHandlers;
			chkOnCopy.CheckedChanged += UpdateEventHandlers;
			chkOnPaste.CheckedChanged += UpdateEventHandlers;
			chkOnCut.CheckedChanged += UpdateEventHandlers;


			grid.Srm.AddStdOutputListener(new ListBoxStandardOutputListener { List = listbox1 });
#else
			chkOnload.CheckedChanged += (s, e) => {
					MessageBox.Show("Script execution is not available in this edition.");
			};
#endif
		}

		#region For output message from script into listbox
		/// <summary>
		/// 
		///  ReoScript Standard Output 'console.log'
		///                   |
		///                   |
		///         Standard I/O Listeners
		///                   |
		///                   |
		///  ListBoxStandardOutputListener (this class)
		///                   |
		///                   |
		///                listbox1
		///  
		/// </summary>
		private class ListBoxStandardOutputListener : IStandardOutputListener
		{
			internal ListBox List { get; set; }

			private void AppendLine(string msg)
			{
				List.Items.Add(msg);
				List.SelectedIndex = List.Items.Count - 1;
			}
			
			public void Write(object obj)
			{
				AppendLine(obj.ToString());
			}

			public void Write(byte[] buf, int index, int count)
			{
				AppendLine(Encoding.Unicode.GetString(buf, index,count));
			}

			public void WriteLine(string line)
			{
				AppendLine(line);
			}
		}
		#endregion

		private void btnReset_Click(object sender, EventArgs e)
		{
			grid.Reset();
		}

#if EX_SCRIPT
		private void UpdateEventHandlers(object sender, EventArgs e)
		{
			#region onload
			if (chkOnload.Checked)
			{
				grid.RunScript(" grid.onload = function() { console.log('onload raised'); }; ");
			}
			else
			{
				grid.RunScript(" grid.onload = null; ");
			}
			#endregion // onload

			#region cellMouseDown
			if (chkCellMouseDown.Checked)
			{
				grid.RunScript(" grid.onmousedown = function(pos) { console.log('onmousedown: ' + pos.row + ':' + pos.col); }; ");
			}
			else
			{
				grid.RunScript(" grid.onmousedown = null; ");
			}
			#endregion // cellMouseDown

			#region cellMouseUp
			if (chkCellMouseUp.Checked)
			{
				grid.RunScript(" grid.onmouseup = function(pos) { console.log('onmouseup: ' + pos.row + ':' + pos.col); }; ");
			}
			else
			{
				grid.RunScript(" grid.onmouseup = null; ");
			}
			#endregion // onCellMouseUp

			#region onCellEdit
			if (chkCellBeforeEdit.Checked)
			{
				grid.RunScript(" grid.oncelledit = function(cell) { console.log('oncelledit: ' + cell.row + ':' + cell.col); }; ");
			}
			else
			{
				grid.RunScript(" grid.oncelledit = null; ");
			}
			#endregion // cellBeforeEdit

			#region selectionChanged
			if (chkSelectionChange.Checked)
			{
				grid.RunScript(" grid.onselectionchange = function() { console.log('onselectionchange'); }; ");
			}
			else
			{
				grid.RunScript(" grid.onselectionchange = null; ");
			}
			#endregion // selectionChanged

			#region selectionMoveNext
			if (chkSelectionMoveNext.Checked)
			{
				grid.RunScript(" grid.onnextfocus = function() { console.log('onnextfocus'); }; ");
			}
			else
			{
				grid.RunScript(" grid.onnextfocus = null; ");
			}
			#endregion // selectionMoveNext

			#region cellDataChanged
			if (chkCellDataChanged.Checked)
			{
				grid.RunScript(" grid.ontextchange = function(cell) { console.log('ontextchange: ' + cell.pos + ':' + cell.data); }; ");
			}
			else
			{
				grid.RunScript(" grid.ontextchange = null; ");
			}
			#endregion // cellDataChanged

			#region oncopy
			if (chkOnCopy.Checked)
			{
				grid.RunScript(" grid.oncopy = function() { console.log('oncopy'); }; ");
			}
			else
			{
				grid.RunScript(" grid.oncopy = null; ");
			}
			#endregion // oncopy

			#region onpaste
			if (chkOnPaste.Checked)
			{
				grid.RunScript(" grid.onpaste = function() { console.log('onpaste'); }; ");
			}
			else
			{
				grid.RunScript(" grid.onpaste = null; ");
			}
			#endregion // onpaste

			#region oncut
			if (chkOnCut.Checked)
			{
				grid.RunScript(" grid.oncut = function() { console.log('oncut'); }; ");
			}
			else
			{
				grid.RunScript(" grid.oncut = null; ");
			}
			#endregion // oncut

			if (chkOnload.Checked && !promptted)
			{
				MessageBox.Show("onload event raised when control has been reset. click 'Reset Control' button to view result.");
				promptted = true;
			}
		}
#endif

		private bool promptted = false;

	}
}
