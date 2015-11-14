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
using System.Drawing.Drawing2D;
using System.IO;
using System.Diagnostics;
using System.Drawing.Printing;

using unvell.Common;

using unvell.ReoGrid.PropertyPages;
using unvell.ReoGrid.Actions;
using unvell.ReoGrid.Editor.Properties;

#if EX_SCRIPT
using unvell.ReoScript.Editor;
using unvell.ReoScript;
#endif

namespace unvell.ReoGrid.Editor
{
	public partial class ReoGridEditor : Form
	{
		#region Constructor

		/// <summary>
		/// Construct ReoGrid Control.
		/// </summary>
		public ReoGridEditor()
		{
			InitializeComponent();

			NewDocumentOnLoad = true;

			SuspendLayout();
			isUIUpdating = true;

			fontToolStripComboBox.Text = ReoGridControl.DefaultStyle.FontName;

			fontSizeToolStripComboBox.Text = ReoGridControl.DefaultStyle.FontSize.ToString();
			fontSizeToolStripComboBox.Items.AddRange(FontToolkit.FontSizeList.Select(f => (object)f).ToArray());

			backColorPickerToolStripButton.CloseOnClick = true;
			borderColorPickToolStripItem.CloseOnClick = true;
			textColorPickToolStripItem.CloseOnClick = true;

			zoomToolStripDropDownButton.Text = "100%";

			isUIUpdating = false;
			ResumeLayout();

			grid.SelectionRangeChanged += grid_SelectionRangeChanged;
			grid.SelectionModeChanged += (s, e) => UpdateSelectionModeAndStyle();
			grid.SelectionStyleChanged += (s, e) => UpdateSelectionModeAndStyle();
			grid.SelectionForwardDirectionChanged += (s, e) => UpdateSelectionForwardDirection();
			selModeNoneToolStripMenuItem.Click += (s, e) => grid.SelectionMode = ReoGridSelectionMode.None;
			selModeCellToolStripMenuItem.Click += (s, e) => grid.SelectionMode = ReoGridSelectionMode.Cell;
			selModeRangeToolStripMenuItem.Click += (s, e) => grid.SelectionMode = ReoGridSelectionMode.Range;
			selStyleNoneToolStripMenuItem.Click += (s, e) => grid.SelectionStyle = ReoGridSelectionStyle.None;
			selStyleDefaultToolStripMenuItem.Click += (s, e) => grid.SelectionStyle = ReoGridSelectionStyle.Default;
			selStyleFocusRectToolStripMenuItem.Click += (s, e) => grid.SelectionStyle = ReoGridSelectionStyle.FocusRect;
			selDirRightToolStripMenuItem.Click += (s, e) => grid.SelectionForwardDirection = SelectionForwardDirection.Right;
			selDirDownToolStripMenuItem.Click += (s, e) => grid.SelectionForwardDirection = SelectionForwardDirection.Down;

			grid.GridScaled += (ss, ee) => zoomToolStripDropDownButton.Text = grid.ScaleFactor * 100 + "%";
			zoomToolStripDropDownButton.TextChanged += zoomToolStripDropDownButton_TextChanged;

			undoToolStripButton.Click += Undo;
			redoToolStripButton.Click += Redo;
			undoToolStripMenuItem.Click += Undo;
			redoToolStripMenuItem.Click += Redo;

			mergeRangeToolStripMenuItem.Click += MergeSelectionRange;
			cellMergeToolStripButton.Click += MergeSelectionRange;
			unmergeRangeToolStripMenuItem.Click += UnmergeSelectionRange;
			unmergeRangeToolStripButton.Click += UnmergeSelectionRange;
			mergeCellsToolStripMenuItem.Click += MergeSelectionRange;
			unmergeCellsToolStripMenuItem.Click += UnmergeSelectionRange;
			formatCellsToolStripMenuItem.Click += formatCellToolStripMenuItem_Click;
			resizeToolStripMenuItem.Click += resizeToolStripMenuItem_Click;
			textWrapToolStripButton.Click += textWrapToolStripButton_Click;
	
			grid.ActionPerformed += (s, e) => UpdateMenuAndToolStripsWhenAction(s, e);
			grid.Undid += (s, e) => UpdateMenuAndToolStripsWhenAction(s, e);
			grid.Redid += (s, e) => UpdateMenuAndToolStripsWhenAction(s, e);
			grid.Resetted += (s, e) => statusToolStripStatusLabel.Text = string.Empty;

			rowHeightToolStripMenuItem.Click += (s, e) =>
			{
				using (SetWidthOrHeightForm rowHeightForm = new SetWidthOrHeightForm(ChangeWidthOrHeight.Height))
				{
					rowHeightForm.Value = grid.GetRowHeight(grid.SelectionRange.Row);
					if (rowHeightForm.ShowDialog() == DialogResult.OK)
					{
						grid.DoAction(new RGSetRowsHeightAction(grid.SelectionRange.Row, 
							grid.SelectionRange.Rows, (ushort)rowHeightForm.Value));
					}
				}
			};

			columnWidthToolStripMenuItem.Click += (s, e) =>
			{
				using (SetWidthOrHeightForm colWidthForm = new SetWidthOrHeightForm(ChangeWidthOrHeight.Width))
				{
					colWidthForm.Value = grid.GetColumnWidth(grid.SelectionRange.Col);
					if (colWidthForm.ShowDialog() == DialogResult.OK)
					{
						grid.DoAction(new RGSetColsWidthAction(grid.SelectionRange.Col,
							grid.SelectionRange.Cols, (ushort)colWidthForm.Value));
					}
				}
			};

			exportAsHtmlToolStripMenuItem.Click += (s, e) =>
			{
				using (SaveFileDialog sfd = new SaveFileDialog())
				{
					sfd.Filter="HTML File(*.html;*.htm)|*.html;*.htm";
					sfd.FileName="Exported ReoGrid";
					if (sfd.ShowDialog() == DialogResult.OK)
					{
						using (FileStream fs = new FileStream(sfd.FileName, FileMode.Create))
						{
							grid.ExportAsHTML(fs);
						}

						Process.Start(sfd.FileName);
					}
				}
			};

			editXMLToolStripMenuItem.Click += (s, e) =>
			{
				string filepath = null;

				if (string.IsNullOrEmpty(this.CurrentFilePath))
				{
					if (string.IsNullOrEmpty(currentTempFilePath))
					{
						currentTempFilePath = Path.Combine(Path.GetTempPath(), 
							Path.GetFileNameWithoutExtension(Path.GetTempFileName()) + ".txt");
						filepath = currentTempFilePath;
					}
				}
				else
				{
					if (MessageBox.Show("Editor will save current file, continue?",
						"Edit XML", MessageBoxButtons.OKCancel, MessageBoxIcon.Question)
						== System.Windows.Forms.DialogResult.Cancel)
					{
						return;
					}

					filepath = this.CurrentFilePath;
				}

				using (var fs = new FileStream(filepath, FileMode.Create, FileAccess.Write))
				{
					grid.Save(fs);
				}

				Process p = Process.Start("notepad.exe", filepath);
				p.WaitForExit();

				if (p.ExitCode == 0)
				{
					grid.Load(filepath);
				}
			};

			saveAsToolStripMenuItem.Click += (s, e) =>
			{
				string newFilepath = null;

				using (SaveFileDialog sfd = new SaveFileDialog())
				{
					sfd.Filter = "ReoGrid XML Format(*.rgf;*.xml)|*.rgf;*.xml|All Files(*.*)|*.*";
					if (sfd.ShowDialog(this) == DialogResult.OK)
					{
						newFilepath = sfd.FileName;
					}
				}

				if (!string.IsNullOrEmpty(newFilepath))
				{
					SaveFile(newFilepath);
				}
			};

			groupRowsToolStripMenuItem.Click += groupRowsToolStripMenuItem_Click;
			groupRowsToolStripMenuItem1.Click += groupRowsToolStripMenuItem_Click;
			ungroupRowsToolStripMenuItem.Click += ungroupRowsToolStripMenuItem_Click;
			ungroupRowsToolStripMenuItem1.Click += ungroupRowsToolStripMenuItem_Click;
			ungroupAllRowsToolStripMenuItem.Click += ungroupAllRowsToolStripMenuItem_Click;
			ungroupAllRowsToolStripMenuItem1.Click += ungroupAllRowsToolStripMenuItem_Click;

			groupColumnsToolStripMenuItem.Click += groupColumnsToolStripMenuItem_Click;
			groupColumnsToolStripMenuItem1.Click += groupColumnsToolStripMenuItem_Click;
			ungroupColumnsToolStripMenuItem.Click += ungroupColumnsToolStripMenuItem_Click;
			ungroupColumnsToolStripMenuItem1.Click += ungroupColumnsToolStripMenuItem_Click;
			ungroupAllColumnsToolStripMenuItem.Click += ungroupAllColumnsToolStripMenuItem_Click;
			ungroupAllColumnsToolStripMenuItem1.Click += ungroupAllColumnsToolStripMenuItem_Click;

			hideRowsToolStripMenuItem.Click += hideRowToolStripMenuItem_Click;
			unhideRowsToolStripMenuItem.Click += unhideRowsToolStripMenuItem_Click;

			hideColumnsToolStripMenuItem.Click += hideColumnsToolStripMenuItem_Click;
			unhideColumnsToolStripMenuItem.Click += unhideColumnsToolStripMenuItem_Click;

#if EX_SCRIPT
			scriptEditorToolStripMenuItem.Click += (s, e) =>
			{
				if (scriptEditor == null || scriptEditor.IsDisposed)
				{
					scriptEditor = new ReoScriptEditor();
					scriptEditor.Srm = grid.Srm;

					// copy script form editor to control once script compiled 
					scriptEditor.ScriptCompiled += (ss, ee) =>
					{
						grid.Script = scriptEditor.Script;
					};
				}

				scriptEditor.Show();
				if (grid.Script == null)
				{
					grid.Script = Resources._default;
				}
					
				scriptEditor.Script = grid.Script;

				scriptEditor.Disposed += (ss, ee) => grid.Script = scriptEditor.Script;
			};

			runFunctionToolStripMenuItem.Click += (s, e) =>
			{
				using (var runFuncForm = new RunFunctionForm())
				{
					Cursor = Cursors.WaitCursor;

					if (grid.Srm != null && grid.Script != null)
					{
						var compiledScript = grid.Srm.Compile(grid.Script);
						runFuncForm.Srm = grid.Srm;
						runFuncForm.Script = compiledScript;
					}

					Cursor = Cursors.Default;

					runFuncForm.ShowDialog(this);
				}
			};

#else // EX_SCRIPT

			//scriptToolStripMenuItem.Visible = false;
			scriptEditorToolStripMenuItem.Click += (s, e) =>
			{
			  MessageBox.Show("Script execution is not supported by this edition.", Application.ProductName);
			};
#endif

#if DEBUG
			#region Debug Validation Events
			grid.RowInserted += (s, e) => _Debug_Auto_Validate_All();
			grid.ColInserted += (s, e) => _Debug_Auto_Validate_All();
			grid.RowDeleted += (s, e) => _Debug_Auto_Validate_All();
			grid.ColDeleted += (s, e) => _Debug_Auto_Validate_All();
			grid.RangeMerged += (s, e) => _Debug_Auto_Validate_All();
			grid.RangeUnmerged += (s, e) => _Debug_Auto_Validate_All(e.Range);
			grid.Undid += (s, e) => _Debug_Auto_Validate_All();
			grid.Redid += (s, e) => _Debug_Auto_Validate_All();
			grid.AfterPaste += (s, e) => _Debug_Auto_Validate_All();

			showDebugInfoToolStripMenuItem.Click += (s, e) =>
			{
				showDebugFormToolStripButton.PerformClick();
				showDebugInfoToolStripMenuItem.Checked = showDebugFormToolStripButton.Checked;
			};

			validateBorderSpanToolStripMenuItem.Click += (s, e) => _Debug_Validate_BorderSpan(true);
			validateMergedRangeToolStripMenuItem.Click += (s, e) => _Debug_Validate_Merged_Cell(true);
			validateAllToolStripMenuItem.Click += (s, e) => _Debug_Validate_All(true);
			#endregion
#endif // DEBUG
		}

		private void hideRowToolStripMenuItem_Click(object sender, EventArgs e)
		{
			grid.HideRows(grid.SelectionRange.Row, grid.SelectionRange.Rows);
		}

		private void unhideRowsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			grid.ShowRows(grid.SelectionRange.Row, grid.SelectionRange.Rows);
		}

		private void hideColumnsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			grid.HideColumns(grid.SelectionRange.Col, grid.SelectionRange.Cols);
		}

		private void unhideColumnsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			grid.ShowColumns(grid.SelectionRange.Col, grid.SelectionRange.Cols);
		}

		void textWrapToolStripButton_Click(object sender, EventArgs e)
		{
			if (textWrapToolStripButton.Checked)
			{
				grid.DoAction(new RGSetRangeStyleAction(grid.SelectionRange, new ReoGridRangeStyle
				{
					Flag = PlainStyleFlag.TextWrap,
					TextWrapMode = TextWrapMode.WordBreak,
				}));
			}
			else
			{
				grid.DoAction(new RGRemoveRangeStyleAction(grid.SelectionRange, PlainStyleFlag.TextWrap)); // TODO
			}
		}

		#endregion // Constructor

		#region Utility

#if DEBUG
		#region Debug Validations
		/// <summary>
		/// Use for Debug mode. Check for border span is valid.
		/// </summary>
		/// <param name="showSuccessMsg"></param>
		/// <returns></returns>
		bool _Debug_Validate_BorderSpan(bool showSuccessMsg)
		{
			bool rs = grid._Debug_Validate_BorderSpan();

			if (rs)
			{
				if (showSuccessMsg) MessageBox.Show("Border span validation ok.");
			}
			else
			{
				ShowError("Border span test failed.");

				if (!showDebugInfoToolStripMenuItem.Checked)
				{
					showDebugInfoToolStripMenuItem.PerformClick();
				}
			}

			return rs;
		}
		bool _Debug_Validate_Merged_Cell(bool showSuccessMsg)
		{
			bool rs = grid._Debug_Validate_MergedCells();
		
			if (rs)
			{
				if (showSuccessMsg) MessageBox.Show("Merged range validation ok.");
			}
			else
			{
				ShowError("Merged range validation failed.");

				if (!showDebugInfoToolStripMenuItem.Checked)
				{
					showDebugInfoToolStripMenuItem.PerformClick();
				}
			}
		
			return rs;
		}
		bool _Debug_Validate_Unmerged_Range(bool showSuccessMsg, ReoGridRange range)
		{
			bool rs = grid._Debug_Validate_Unmerged_Range(range);

			if (rs)
			{
				if (showSuccessMsg) MessageBox.Show("Unmerged range validation ok.");
			}
			else
			{
				ShowError("Unmerged range validation failed.");

				if (!showDebugInfoToolStripMenuItem.Checked)
				{
					showDebugInfoToolStripMenuItem.PerformClick();
				}
			}

			return rs;
		}
		bool _Debug_Validate_All(bool showSuccessMsg)
		{
			return _Debug_Validate_All(showSuccessMsg, ReoGridRange.EntireRange);
		}
		bool _Debug_Validate_All(ReoGridRange range)
		{
			return _Debug_Validate_All(false, range);
		}
		bool _Debug_Validate_All(bool showSuccessMsg, ReoGridRange range)
		{
			bool rs = _Debug_Validate_BorderSpan(showSuccessMsg);
			if (rs) rs = _Debug_Validate_Merged_Cell(showSuccessMsg);
			if (rs) rs = _Debug_Validate_Unmerged_Range(showSuccessMsg, range);

			return rs;
		}
		bool _Debug_Auto_Validate_All() { return _Debug_Validate_All(false); }
		bool _Debug_Auto_Validate_All(ReoGridRange range) { return _Debug_Validate_All(range); }
	#endregion // Debug Validations
#endif // DEBUG

		public ReoGridControl Grid { get { return grid; } }

#if EX_SCRIPT
		private ReoScriptEditor scriptEditor;
		public ReoScriptEditor ScriptEditor { get { return scriptEditor; } }
#endif // EX_SCRIPT

		public void ShowStatus(string msg)
		{
			ShowStatus(msg, false);
		}
		public void ShowStatus(string msg, bool error)
		{
			statusToolStripStatusLabel.Text = msg;
			statusToolStripStatusLabel.ForeColor = error ? Color.Red : SystemColors.WindowText;
		}
		public void ShowError(string msg)
		{
			ShowStatus(msg, true);
		}

		private void UpdateMenuAndToolStripsWhenAction(object sender, EventArgs e)
		{
			UpdateMenuAndToolStrips();
		}

		private void Undo(object sender, EventArgs e)
		{
			grid.Undo(); 
		}
		private void Redo(object sender, EventArgs e)
		{
			grid.Redo(); 
		}

		void zoomToolStripDropDownButton_TextChanged(object sender, EventArgs e)
		{
			if (isUIUpdating) return;

			if (zoomToolStripDropDownButton.Text.Length > 0)
			{
				int value = 100;
				if (int.TryParse(zoomToolStripDropDownButton.Text.Substring(0, zoomToolStripDropDownButton.Text.Length - 1), out value))
				{
					grid.SetScale((float)value / 100f, new Point(0, 0));
				}
			}
		}

		void grid_SelectionRangeChanged(object sender, RGRangeEventArgs e)
		{
			if (grid.SelectionRange == ReoGridRange.Empty)
			{
				rangeInfoToolStripStatusLabel.Text = "Selection None";
			}
			else
			{
				rangeInfoToolStripStatusLabel.Text = grid.SelectionRange.ToString() + " " + grid.SelectionRange.ToStringCode();
			}
			UpdateMenuAndToolStrips();
		}

		#endregion // Utility

		#region Update Menus & Toolbars
		private bool isUIUpdating = false;
		private void UpdateMenuAndToolStrips()
		{
			if (isUIUpdating)
				return;

			isUIUpdating = true;

			ReoGridRangeStyle style = grid.GetCellStyle(grid.SelectionRange.StartPos);
			if (style != null)
			{
				// cross-thread exception
				Action set = () =>
				{
					fontToolStripComboBox.Text = style.FontName;
					fontSizeToolStripComboBox.Text = style.FontSize.ToString();
					boldToolStripButton.Checked = style.Bold;
					italicToolStripButton.Checked = style.Italic;
					strikethroughToolStripButton.Checked = style.Strikethrough;
					underlineToolStripButton.Checked = style.Underline;
					textColorPickToolStripItem.SolidColor = style.TextColor;
					backColorPickerToolStripButton.SolidColor = style.BackColor;
					textAlignLeftToolStripButton.Checked = style.HAlign == ReoGridHorAlign.Left;
					textAlignCenterToolStripButton.Checked = style.HAlign == ReoGridHorAlign.Center;
					textAlignRightToolStripButton.Checked = style.HAlign == ReoGridHorAlign.Right;
					distributedIndentToolStripButton.Checked = style.HAlign == ReoGridHorAlign.DistributedIndent;
					textAlignTopToolStripButton.Checked = style.VAlign == ReoGridVerAlign.Top;
					textAlignMiddleToolStripButton.Checked = style.VAlign == ReoGridVerAlign.Middle;
					textAlignBottomToolStripButton.Checked = style.VAlign == ReoGridVerAlign.Bottom;
					textWrapToolStripButton.Checked = style.TextWrapMode != TextWrapMode.NoWrap;


					ReoGridRangeBorderInfo borderInfo = grid.GetRangeBorder(grid.SelectionRange);
					if (borderInfo.Left != null && !borderInfo.Left.Color.IsEmpty)
					{
						borderColorPickToolStripItem.SolidColor = borderInfo.Left.Color;
					}
					else if (borderInfo.Right != null && !borderInfo.Right.Color.IsEmpty)
					{
						borderColorPickToolStripItem.SolidColor = borderInfo.Right.Color;
					}
					else if (borderInfo.Top != null && !borderInfo.Top.Color.IsEmpty)
					{
						borderColorPickToolStripItem.SolidColor = borderInfo.Top.Color;
					}
					else if (borderInfo.Bottom != null && !borderInfo.Bottom.Color.IsEmpty)
					{
						borderColorPickToolStripItem.SolidColor = borderInfo.Bottom.Color;
					}
					else if (borderInfo.InsideHorizontal != null && !borderInfo.InsideHorizontal.Color.IsEmpty)
					{
						borderColorPickToolStripItem.SolidColor = borderInfo.InsideHorizontal.Color;
					}
					else if (borderInfo.InsideVertical != null && !borderInfo.InsideVertical.Color.IsEmpty)
					{
						borderColorPickToolStripItem.SolidColor = borderInfo.InsideVertical.Color;
					}
					else
					{
						borderColorPickToolStripItem.SolidColor = Color.Black;
					}

					undoToolStripButton.Enabled =
						undoToolStripMenuItem.Enabled =
						grid.CanUndo();

					redoToolStripButton.Enabled =
						redoToolStripMenuItem.Enabled =
						grid.CanRedo();

					repeatLastActionToolStripMenuItem.Enabled =
						grid.CanUndo() || grid.CanRedo();

					cutToolStripButton.Enabled =
						cutToolStripMenuItem.Enabled =
						grid.CanCut();

					copyToolStripButton.Enabled =
						copyToolStripMenuItem.Enabled =
						grid.CanCopy();

					pasteToolStripButton.Enabled =
						pasteToolStripMenuItem.Enabled =
						grid.CanPaste();

					freezeToCellToolStripMenuItem.Enabled = grid.CanFreeze();
					unfreezeToolStripMenuItem.Enabled = !freezeToCellToolStripMenuItem.Enabled;

					isUIUpdating = false;
				};

				if (this.InvokeRequired)
					this.Invoke(set);
				else
					set();
			}

#if !DEBUG
			debugToolStripMenuItem.Enabled = false;
#endif

		}

		private bool settingSelectionMode = false;

		private void UpdateSelectionModeAndStyle()
		{
			if (settingSelectionMode) return;

			settingSelectionMode = true;

			selModeNoneToolStripMenuItem.Checked = false;
			selModeCellToolStripMenuItem.Checked = false;
			selModeRangeToolStripMenuItem.Checked = false;

			switch (grid.SelectionMode)
			{
				case ReoGridSelectionMode.None:
					selModeNoneToolStripMenuItem.Checked = true;
					break;
				case ReoGridSelectionMode.Cell:
					selModeCellToolStripMenuItem.Checked = true;
					break;
				default:
				case ReoGridSelectionMode.Range:
					selModeRangeToolStripMenuItem.Checked = true;
					break;
			}

			selStyleNoneToolStripMenuItem.Checked = false;
			selStyleDefaultToolStripMenuItem.Checked = false;
			selStyleFocusRectToolStripMenuItem.Checked = false;

			switch (grid.SelectionStyle)
			{
				case ReoGridSelectionStyle.None:
					selStyleNoneToolStripMenuItem.Checked = true;
					break;
				default:
				case ReoGridSelectionStyle.Default:
					selStyleDefaultToolStripMenuItem.Checked = true;
					break;
				case ReoGridSelectionStyle.FocusRect:
					selStyleFocusRectToolStripMenuItem.Checked = true;
					break;
			}

			settingSelectionMode = false;
		}
		private void UpdateSelectionForwardDirection()
		{
			switch (grid.SelectionForwardDirection)
			{
				default:
				case SelectionForwardDirection.Right:
					selDirRightToolStripMenuItem.Checked = true;
					selDirDownToolStripMenuItem.Checked = false;
					break;
				case SelectionForwardDirection.Down:
					selDirRightToolStripMenuItem.Checked = false;
					selDirDownToolStripMenuItem.Checked = true;
					break;
			}
		}
		#endregion

		#region Document
		public string CurrentFilePath { get; set; }
		private string currentTempFilePath;

		public void LoadFile(string path)
		{
			this.CurrentFilePath = null;
			if (grid.Load(path))
			{
				this.Text = System.IO.Path.GetFileName(path) + " - ReoGrid Editor";
				showGridToolStripMenuItem.Checked = grid.HasSettings(ReoGridSettings.View_ShowGridLine);
				ShowStatus(string.Empty);
				this.CurrentFilePath = path;
				this.currentTempFilePath = null;

#if EX_SCRIPT
				// check whether grid contains any scripts
				if (!string.IsNullOrEmpty(grid.Script))
				{
					if (MessageBox.Show("The document contains macro and executable script. Do you want to run the script now?",
						"Executable Script", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
					{
						if (scriptEditor == null)
						{
							scriptEditor = new ReoScriptEditor()
							{
								Script = grid.Script,
								Srm = grid.Srm,
							};
						}

						// run init script
						grid.RunScript();

						// show script editor window
						if (!scriptEditor.Visible)
						{
							scriptEditor.Show();
						}
					}
				}
#endif
			}
		}
		private void NewFile()
		{
			grid.Reset();

			this.Text = "Untitled - ReoGrid Editor";

			showGridToolStripMenuItem.Checked = grid.HasSettings(ReoGridSettings.View_ShowGridLine);
			this.CurrentFilePath = null;
			this.currentTempFilePath = null;

#if DEBUG // for test case
			//showDebugFormToolStripButton.PerformClick();
			ForTest();
#endif
		}
		
		public class MyData
		{
			public override string ToString() { return "mydata"; }
		}

		private class MyCellBody : CellBody
		{
			public override void OnPaint(RGDrawingContext dc)
			{
				dc.Graphics.DrawEllipse(Pens.Blue, base.Bounds);
			}

			public override bool OnStartEdit(ReoGridCell cell)
			{
				return false;
			}
		}

		class DropdownCell : CellBody
		{
			public ReoGridControl Grid { get; set; }

			private bool focused;

			private int ButtonWidth = 20;
			private int ButtonHeight = 20;

			public DropdownCell(ReoGridControl grid)
			{
				this.Grid = grid;
			}

			public override void OnPaint(RGDrawingContext dc)
			{
				if (focused)
				{
					Rectangle btnRect = new Rectangle(Bounds.Right + 1,
							Bounds.Top + (Bounds.Height - ButtonHeight) / 2,
							ButtonWidth, ButtonHeight);

					ControlPaint.DrawComboButton(dc.Graphics, btnRect, ButtonState.Normal);
				}
			}

			public override void OnGotFocus(ReoGridCell cell)
			{
				focused = true;
			}

			public override void OnLostFocus(ReoGridCell cell)
			{
				focused = false;
			}

			public override bool OnStartEdit(ReoGridCell cell)
			{
				return false;
			}

			public override object GetData()
			{
				return "DropDown";
			}
		}

		private void SaveFile(string path)
		{
#if EX_SCRIPT
			if (scriptEditor != null && scriptEditor.Visible)
			{
				grid.Script = scriptEditor.Script;
			}
#endif // EX_SCRIPT

			if(grid.Save(path, "ReoGridEditor " + this.ProductVersion.ToString()))
			{
				this.Text = System.IO.Path.GetFileName(path) + " - ReoGrid Editor";
				this.CurrentFilePath = path;
				this.currentTempFilePath = null;
#if DEBUG
			//Process.Start("notepad.exe", path);
#endif
			}
		}

		private void newToolStripButton_Click(object sender, EventArgs e)
		{
			NewFile();
		}
		private void loadToolStripButton_Click(object sender, EventArgs e)
		{
			using (OpenFileDialog ofd = new OpenFileDialog())
			{
				ofd.Filter = "ReoGrid XML Format(*.rgf;*.xml)|*.rgf;*.xml|All Files(*.*)|*.*";
				if (ofd.ShowDialog(this) == DialogResult.OK)
				{
					LoadFile(ofd.FileName);
				}
			}
		}
		private void saveToolStripButton_Click(object sender, EventArgs e)
		{
			if (string.IsNullOrEmpty(CurrentFilePath))
			{
				using (SaveFileDialog sfd = new SaveFileDialog())
				{
					sfd.Filter = "ReoGrid XML Format(*.rgf;*.xml)|*.rgf;*.xml|All Files(*.*)|*.*";
					if (sfd.ShowDialog(this) == DialogResult.OK)
					{
						this.CurrentFilePath = sfd.FileName;
					}
				}
			}

			if (!string.IsNullOrEmpty(this.CurrentFilePath))
			{
				SaveFile(this.CurrentFilePath);
			}
		}

		public bool NewDocumentOnLoad { get; set; }

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

#if DEBUG
			if (Toolkit.IsKeyDown(unvell.Common.Win32Lib.Win32.VKey.VK_CONTROL))
			{
				FileInfo file = new FileInfo("..\\..\\autosave.sgf");
				if (file.Exists) LoadFile(file.FullName);
			}
#endif

			// load file if specified 
			if (CurrentFilePath != null)
			{
				LoadFile(CurrentFilePath);
			}
			else if(NewDocumentOnLoad)
			{
				NewFile();
			}

#if EX_SCRIPT
			if (!string.IsNullOrEmpty(grid.Script) && (scriptEditor == null || !scriptEditor.Visible || scriptEditor.IsDisposed))
			{
				scriptEditorToolStripMenuItem.PerformClick();
			}
#endif // EX_SCRIPT

			UpdateSelectionModeAndStyle();
			UpdateSelectionForwardDirection();

			grid.Focus();
		}

		protected override void OnClosing(CancelEventArgs e)
		{
			base.OnClosing(e);

#if DEBUG
			if (Toolkit.IsKeyDown(unvell.Common.Win32Lib.Win32.VKey.VK_CONTROL))
			{
				grid.Save("..\\..\\autosave.sgf");
			}
#endif // DEBUG
		}

		private void newToolStripMenuItem_Click(object sender, EventArgs e)
		{
			newToolStripButton.PerformClick();
		}

		private void openToolStripMenuItem_Click(object sender, EventArgs e)
		{
			loadToolStripButton.PerformClick();
		}

		private void saveToolStripMenuItem_Click(object sender, EventArgs e)
		{
			saveToolStripButton.PerformClick();
		}

		#endregion

		#region Alignment
		private void textLeftToolStripButton_Click(object sender, EventArgs e)
		{
			grid.DoAction(new RGSetRangeStyleAction(grid.SelectionRange, new ReoGridRangeStyle
			{
				Flag = PlainStyleFlag.HorizontalAlign,
				HAlign = ReoGridHorAlign.Left,
			}));
		}
		private void textCenterToolStripButton_Click(object sender, EventArgs e)
		{
			grid.DoAction(new RGSetRangeStyleAction(grid.SelectionRange, new ReoGridRangeStyle
			{
				Flag = PlainStyleFlag.HorizontalAlign,
				HAlign = ReoGridHorAlign.Center,
			}));
		}
		private void textRightToolStripButton_Click(object sender, EventArgs e)
		{
			grid.DoAction(new RGSetRangeStyleAction(grid.SelectionRange, new ReoGridRangeStyle
			{
				Flag = PlainStyleFlag.HorizontalAlign,
				HAlign = ReoGridHorAlign.Right,
			}));
		}
		private void distributedIndentToolStripButton_Click(object sender, EventArgs e)
		{
			grid.DoAction(new RGSetRangeStyleAction(grid.SelectionRange, new ReoGridRangeStyle
			{
				Flag = PlainStyleFlag.HorizontalAlign,
				HAlign = ReoGridHorAlign.DistributedIndent,
			}));
		}

		private void textAlignTopToolStripButton_Click(object sender, EventArgs e)
		{
			grid.DoAction(new RGSetRangeStyleAction(grid.SelectionRange, new ReoGridRangeStyle
			{
				Flag = PlainStyleFlag.VerticalAlign,
				VAlign = ReoGridVerAlign.Top,
			}));
		}
		private void textAlignMiddleToolStripButton_Click(object sender, EventArgs e)
		{
			grid.DoAction(new RGSetRangeStyleAction(grid.SelectionRange, new ReoGridRangeStyle
			{
				Flag = PlainStyleFlag.VerticalAlign,
				VAlign = ReoGridVerAlign.Middle,
			}));
		}
		private void textAlignBottomToolStripButton_Click(object sender, EventArgs e)
		{
			grid.DoAction(new RGSetRangeStyleAction(grid.SelectionRange, new ReoGridRangeStyle
			{
				Flag = PlainStyleFlag.VerticalAlign,
				VAlign = ReoGridVerAlign.Bottom,
			}));
		}
		#endregion

		#region Border Settings

		#region Outline Borders
		private void SetSelectionBorder(ReoGridBorderPos borderPos, BorderLineStyle style)
		{
			grid.DoAction(new RGSetRangeBorderAction(grid.SelectionRange, borderPos,
				new ReoGridBorderStyle { Color = borderColorPickToolStripItem.SolidColor, Style = style }));
		}

		private void boldOutlineToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SetSelectionBorder(ReoGridBorderPos.Outline, BorderLineStyle.BoldSolid);
		}
		private void dottedOutlineToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SetSelectionBorder(ReoGridBorderPos.Outline, BorderLineStyle.Dotted);
		}
		private void boundsSolidToolStripButton_ButtonClick(object sender, EventArgs e)
		{
			SetSelectionBorder(ReoGridBorderPos.Outline, BorderLineStyle.Solid);
		}
		private void solidOutlineToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SetSelectionBorder(ReoGridBorderPos.Outline, BorderLineStyle.Solid);
		}
		private void dashedOutlineToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SetSelectionBorder(ReoGridBorderPos.Outline, BorderLineStyle.Dashed);
		}
		#endregion

		#region Inside Borders
		private void insideSolidToolStripSplitButton_ButtonClick(object sender, EventArgs e)
		{
			SetSelectionBorder(ReoGridBorderPos.InsideAll, BorderLineStyle.Solid);
		}
		private void insideSolidToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SetSelectionBorder(ReoGridBorderPos.InsideAll, BorderLineStyle.Solid);
		}
		private void insideBoldToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SetSelectionBorder(ReoGridBorderPos.InsideAll, BorderLineStyle.BoldSolid);
		}
		private void insideDottedToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SetSelectionBorder(ReoGridBorderPos.InsideAll, BorderLineStyle.Dotted);
		}
		private void insideDashedToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SetSelectionBorder(ReoGridBorderPos.InsideAll, BorderLineStyle.Dashed);
		}
		#endregion

		#region Left & Right Borders
		private void leftRightSolidToolStripSplitButton_ButtonClick(object sender, EventArgs e)
		{
			SetSelectionBorder(ReoGridBorderPos.LeftRight, BorderLineStyle.Solid);
		}
		private void leftRightSolidToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SetSelectionBorder(ReoGridBorderPos.LeftRight, BorderLineStyle.Solid);
		}
		private void leftRightBoldToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SetSelectionBorder(ReoGridBorderPos.LeftRight, BorderLineStyle.BoldSolid);
		}
		private void leftRightDotToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SetSelectionBorder(ReoGridBorderPos.LeftRight, BorderLineStyle.Dotted);
		}
		private void leftRightDashToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SetSelectionBorder(ReoGridBorderPos.LeftRight, BorderLineStyle.Dashed);
		}
		#endregion

		#region Top & Bottom Borders
		private void topBottomSolidToolStripSplitButton_ButtonClick(object sender, EventArgs e)
		{
			SetSelectionBorder(ReoGridBorderPos.TopBottom, BorderLineStyle.Solid);
		}
		private void topBottomSolidToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SetSelectionBorder(ReoGridBorderPos.TopBottom, BorderLineStyle.Solid);
		}
		private void topBottomBoldToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SetSelectionBorder(ReoGridBorderPos.TopBottom, BorderLineStyle.BoldSolid);
		}
		private void topBottomDotToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SetSelectionBorder(ReoGridBorderPos.TopBottom, BorderLineStyle.Dotted);
		}
		private void topBottomDashToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SetSelectionBorder(ReoGridBorderPos.TopBottom, BorderLineStyle.Dashed);
		}
		#endregion

		#region All Borders
		private void allSolidToolStripSplitButton_ButtonClick(object sender, EventArgs e)
		{
			SetSelectionBorder(ReoGridBorderPos.All, BorderLineStyle.Solid);
		}
		private void allSolidToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SetSelectionBorder(ReoGridBorderPos.All, BorderLineStyle.Solid);
		}
		private void allBoldToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SetSelectionBorder(ReoGridBorderPos.All, BorderLineStyle.BoldSolid);
		}
		private void allDottedToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SetSelectionBorder(ReoGridBorderPos.All, BorderLineStyle.Dotted);
		}
		private void allDashedToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SetSelectionBorder(ReoGridBorderPos.All, BorderLineStyle.Dashed);
		}
		#endregion

		#region Left Border
		private void leftSolidToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SetSelectionBorder(ReoGridBorderPos.Left, BorderLineStyle.Solid);
		}

		private void leftSolidToolStripButton_ButtonClick(object sender, EventArgs e)
		{
			SetSelectionBorder(ReoGridBorderPos.Left, BorderLineStyle.Solid);
		}

		private void leftBoldToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SetSelectionBorder(ReoGridBorderPos.Left, BorderLineStyle.BoldSolid);
		}

		private void leftDotToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SetSelectionBorder(ReoGridBorderPos.Left, BorderLineStyle.Dotted);
		}

		private void leftDashToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SetSelectionBorder(ReoGridBorderPos.Left, BorderLineStyle.Dashed);
		}
		#endregion

		#region Top Border
		private void topSolidToolStripButton_ButtonClick(object sender, EventArgs e)
		{
			SetSelectionBorder(ReoGridBorderPos.Top, BorderLineStyle.Solid);
		}

		private void topSolidToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SetSelectionBorder(ReoGridBorderPos.Top, BorderLineStyle.Solid);
		}

		private void topBlodToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SetSelectionBorder(ReoGridBorderPos.Top, BorderLineStyle.BoldSolid);
		}

		private void topDotToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SetSelectionBorder(ReoGridBorderPos.Top, BorderLineStyle.Dotted);
		}

		private void topDashToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SetSelectionBorder(ReoGridBorderPos.Top, BorderLineStyle.Dashed);
		}
		#endregion

		#region Bottom Border
		private void bottomToolStripButton_ButtonClick(object sender, EventArgs e)
		{
			SetSelectionBorder(ReoGridBorderPos.Bottom, BorderLineStyle.Solid);
		}

		private void bottomSolidToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SetSelectionBorder(ReoGridBorderPos.Bottom, BorderLineStyle.Solid);
		}

		private void bottomBoldToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SetSelectionBorder(ReoGridBorderPos.Bottom, BorderLineStyle.BoldSolid);
		}

		private void bottomDotToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SetSelectionBorder(ReoGridBorderPos.Bottom, BorderLineStyle.Dotted);
		}

		private void bottomDashToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SetSelectionBorder(ReoGridBorderPos.Bottom, BorderLineStyle.Dashed);
		}
		#endregion

		#region Right Border
		private void rightSolidToolStripButton_ButtonClick(object sender, EventArgs e)
		{
			SetSelectionBorder(ReoGridBorderPos.Right, BorderLineStyle.Solid);
		}

		private void rightSolidToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SetSelectionBorder(ReoGridBorderPos.Right, BorderLineStyle.Solid);
		}

		private void rightBoldToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SetSelectionBorder(ReoGridBorderPos.Right, BorderLineStyle.BoldSolid);
		}

		private void rightDotToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SetSelectionBorder(ReoGridBorderPos.Right, BorderLineStyle.Dotted);
		}

		private void rightDashToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SetSelectionBorder(ReoGridBorderPos.Right, BorderLineStyle.Dashed);
		}
		#endregion

		#region Slash
		private void slashRightSolidToolStripButton_ButtonClick(object sender, EventArgs e)
		{
			SetSelectionBorder(ReoGridBorderPos.Slash, BorderLineStyle.Solid);
		}

		private void slashRightSolidToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SetSelectionBorder(ReoGridBorderPos.Slash, BorderLineStyle.Solid);
		}

		private void slashRightBoldToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SetSelectionBorder(ReoGridBorderPos.Slash, BorderLineStyle.BoldSolid);
		}

		private void slashRightDotToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SetSelectionBorder(ReoGridBorderPos.Slash, BorderLineStyle.Dotted);
		}

		private void slashRightDashToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SetSelectionBorder(ReoGridBorderPos.Slash, BorderLineStyle.Dashed);
		}
		#endregion

		#region Backslash
		private void slashLeftSolidToolStripButton_ButtonClick(object sender, EventArgs e)
		{
			SetSelectionBorder(ReoGridBorderPos.Backslash, BorderLineStyle.Solid);
		}

		private void slashLeftSolidToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SetSelectionBorder(ReoGridBorderPos.Backslash, BorderLineStyle.Solid);
		}

		private void slashLeftBoldToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SetSelectionBorder(ReoGridBorderPos.Backslash, BorderLineStyle.BoldSolid);
		}

		private void slashLeftDotToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SetSelectionBorder(ReoGridBorderPos.Backslash, BorderLineStyle.Dotted);
		}

		private void slashLeftDashToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SetSelectionBorder(ReoGridBorderPos.Backslash, BorderLineStyle.Dashed);
		}
		#endregion

		#region Clear Borders
		private void clearBordersToolStripButton_Click(object sender, EventArgs e)
		{
			grid.DoAction(new RGSetRangeBorderAction(grid.SelectionRange, ReoGridBorderPos.All,
				new ReoGridBorderStyle { Color = Color.Empty, Style = BorderLineStyle.None }));
		}
		#endregion

		#endregion

		#region Style
		private void backColorPickerToolStripButton_ColorPicked(object sender, EventArgs e)
		{
			Color c = backColorPickerToolStripButton.SolidColor;
			//if (c.IsEmpty)
			//{
			//  grid.DoAction(new SGRemoveRangeStyleAction(grid.SelectionRange, PlainStyleFlag.FillColor));
			//}
			//else
			//{
				grid.DoAction(new RGSetRangeStyleAction(grid.SelectionRange, new ReoGridRangeStyle()
				{
					Flag = PlainStyleFlag.FillColor,
					BackColor = c,
				}));
			//}
		}

		private void textColorPickToolStripItem_ColorPicked(object sender, EventArgs e)
		{
			grid.DoAction(new RGSetRangeStyleAction(grid.SelectionRange, new ReoGridRangeStyle
			{
				Flag = PlainStyleFlag.TextColor,
				TextColor = textColorPickToolStripItem.SolidColor,
			}));
		}

		private void boldToolStripButton_Click(object sender, EventArgs e)
		{
			grid.DoAction(new RGSetRangeStyleAction(grid.SelectionRange, new ReoGridRangeStyle
			{
				Flag = PlainStyleFlag.FontStyleBold,
				Bold = boldToolStripButton.Checked,
			}));
		}

		private void italicToolStripButton_Click(object sender, EventArgs e)
		{
			grid.DoAction(new RGSetRangeStyleAction(grid.SelectionRange, new ReoGridRangeStyle
			{
				Flag = PlainStyleFlag.FontStyleItalic,
				Italic = italicToolStripButton.Checked,
			}));
		}

		private void underlineToolStripButton_Click(object sender, EventArgs e)
		{
			grid.DoAction(new RGSetRangeStyleAction(grid.SelectionRange, new ReoGridRangeStyle
			{
				Flag = PlainStyleFlag.FontStyleUnderline,
				Underline = underlineToolStripButton.Checked,
			}));
		}

		private void strikethroughToolStripButton_Click(object sender, EventArgs e)
		{
			grid.DoAction(new RGSetRangeStyleAction(grid.SelectionRange, new ReoGridRangeStyle
			{
				Flag = PlainStyleFlag.FontStyleStrikethrough,
				Strikethrough = strikethroughToolStripButton.Checked,
			}));
		}

		private void styleBrushToolStripButton_Click(object sender, EventArgs e)
		{
			grid.StartPickRangeAndCopyStyle();
		}

		private void enlargeFontToolStripButton_Click(object sender, EventArgs e)
		{
			grid.DoAction(new RGStepRangeFontSizeAction(grid.SelectionRange, true));
			UpdateMenuAndToolStrips();
		}

		private void fontSmallerToolStripButton_Click(object sender, EventArgs e)
		{
			grid.DoAction(new RGStepRangeFontSizeAction(grid.SelectionRange, false));
			UpdateMenuAndToolStrips();
		}

		private void fontToolStripComboBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (isUIUpdating) return;

			grid.DoAction(new RGSetRangeStyleAction(grid.SelectionRange, new ReoGridRangeStyle
			{
				Flag = PlainStyleFlag.FontName,
				FontName = fontToolStripComboBox.Text,
			}));
		}

		private void fontSizeToolStripComboBox_TextChanged(object sender, EventArgs e)
		{
			SetGridFontSize();
		}

		private void SetGridFontSize()
		{
			if (isUIUpdating) return;

			float size = TextFormatHelper.GetFloatValue(fontSizeToolStripComboBox.Text, 9);

			if (size <= 0) size = 1f;

			grid.DoAction(new RGSetRangeStyleAction(grid.SelectionRange, new ReoGridRangeStyle
			{
				Flag = PlainStyleFlag.FontSize,
				FontSize = size,
			}));
		}

		#endregion

		#region Cell & Range
		private void MergeSelectionRange(object sender, EventArgs e)
		{
			try
			{
				grid.DoAction(new RGMergeRangeAction(grid.SelectionRange));
			}
			catch (RangeTooSmallException) { }
			catch (RangeIntersectionException)
			{
				MessageBox.Show("Specified range intersects with another one. Operation cannot be performmed.",
					Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
		}

		private void UnmergeSelectionRange(object sender, EventArgs e)
		{
			grid.DoAction(new RGUnmergeRangeAction(grid.SelectionRange));
		}

		void resizeToolStripMenuItem_Click(object sender, EventArgs e)
		{
			using (var rgf = new ResizeGridForm())
			{
				rgf.Rows = Grid.RowCount;
				rgf.Cols = Grid.ColCount;

				if (rgf.ShowDialog() == System.Windows.Forms.DialogResult.OK)
				{
					RGActionGroup ag = new RGActionGroup();

					if (rgf.Rows < grid.RowCount)
					{
						ag.Actions.Add(new RGRemoveRowsAction(rgf.Rows, grid.RowCount - rgf.Rows));
					}
					else if (rgf.Rows > grid.RowCount)
					{
						ag.Actions.Add(new RGInsertRowsAction(grid.RowCount, rgf.Rows - grid.RowCount));
					}

					if (rgf.Cols < grid.ColCount)
					{
						ag.Actions.Add(new RGRemoveColumnsAction(rgf.Cols, grid.ColCount - rgf.Cols));
					}
					else if (rgf.Cols > grid.ColCount)
					{
						ag.Actions.Add(new RGInsertColumnsAction(grid.ColCount, rgf.Cols- grid.ColCount));
					}

					if (ag.Actions.Count > 0)
					{
						Cursor = Cursors.WaitCursor;

						try
						{
							grid.DoAction(ag);
						}
						finally
						{
							Cursor = Cursors.Default;
						}
					}
				}
			}
		}
		#endregion

		#region Context Menu
		private void insertColToolStripMenuItem_Click(object sender, EventArgs e)
		{
			grid.DoAction(new RGInsertColumnsAction(grid.SelectionRange.Col, grid.SelectionRange.Cols));
		}
		private void insertRowToolStripMenuItem_Click(object sender, EventArgs e)
		{
			grid.DoAction(new RGInsertRowsAction(grid.SelectionRange.Row, grid.SelectionRange.Rows));
		}
		private void deleteColumnToolStripMenuItem_Click(object sender, EventArgs e)
		{
			grid.DoAction(new RGRemoveColumnsAction(grid.SelectionRange.Col, grid.SelectionRange.Cols));
		}
		private void deleteRowsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			grid.DoAction(new RGRemoveRowsAction(grid.SelectionRange.Row, grid.SelectionRange.Rows));
		}
		private void resetToDefaultWidthToolStripMenuItem_Click(object sender, EventArgs e)
		{
			grid.DoAction(new RGSetColsWidthAction(grid.SelectionRange.Col, 
				grid.SelectionRange.Cols, ReoGridControl.InitDefaultColumnWidth));
		}
		private void resetToDefaultHeightToolStripMenuItem_Click(object sender, EventArgs e)
		{
			grid.DoAction(new RGSetRowsHeightAction(grid.SelectionRange.Row, 
				grid.SelectionRange.Rows, ReoGridControl.InitDefaultRowHeight));
		}
		#endregion

		#region Debug Form
		private DebugForm cellDebugForm = null;
		private DebugForm borderDebugForm = null;

		private void showDebugFormToolStripButton_Click(object sender, EventArgs e)
		{
			if (cellDebugForm == null)
			{
				cellDebugForm = new DebugForm();
				cellDebugForm.Grid = grid;

				cellDebugForm.VisibleChanged += (ss, se) => showDebugFormToolStripButton.Checked = cellDebugForm.Visible;
			}
			else if(cellDebugForm.Visible)
			{
				cellDebugForm.Visible = false;
				borderDebugForm.Visible = false;
				return;
			}

			if (!cellDebugForm.Visible)
			{
				cellDebugForm.InitTabType = DebugForm.InitTab.Grid;
				cellDebugForm.Show(this);
			}

			if (borderDebugForm == null)
			{
				borderDebugForm = new DebugForm();
				borderDebugForm.Grid = grid;
			}

			if (!borderDebugForm.Visible)
			{
				borderDebugForm.InitTabType = DebugForm.InitTab.Border;
				borderDebugForm.Show(this);
			}

			if (cellDebugForm.Visible || borderDebugForm.Visible) ResetDebugFormLocation();
		}

		protected override void OnShown(EventArgs e)
		{
			base.OnShown(e);
		}

		protected override void OnMove(EventArgs e)
		{
			base.OnMove(e);
			ResetDebugFormLocation();
		}

		private void ResetDebugFormLocation()
		{
			if (cellDebugForm != null && cellDebugForm.Visible)
			{
				cellDebugForm.Location = new Point(this.Right, this.Top);
			}
			if (borderDebugForm != null && borderDebugForm.Visible)
			{
				borderDebugForm.Location = new Point(cellDebugForm.Left, cellDebugForm.Bottom);
			}
		}
		#endregion

		#region Editing
		private void cutRangeToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Cut();
		}
		private void copyRangeToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Copy();
		}
		private void pasteRangeToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Paste();
		}

		private void Cut()
		{
			// Cut method will always perform action to do cut
			grid.Cut();
		}
		private void Copy()
		{
			grid.Copy();
		}
		private void Paste()
		{
			grid.Paste();
		}
		#endregion

		#region Window
		private void toolbarToolStripMenuItem_Click(object sender, EventArgs e)
		{
			toolStrip1.Visible = fontToolStrip.Visible = toolbarToolStripMenuItem.Checked;
		}

		private void exitToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void newWindowToolStripMenuItem_Click(object sender, EventArgs e)
		{
			new ReoGridEditor().Show();
		}

		private void styleEditorToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ControlAppearanceEditorForm styleEditor = new ControlAppearanceEditorForm();
			styleEditor.Grid = grid;
			styleEditor.Show(this);
		}
		#endregion

		#region Edit
		private void cutToolStripMenuItem_Click(object sender, EventArgs e)
		{
			cutToolStripButton.PerformClick();
		}

		private void copyToolStripMenuItem_Click(object sender, EventArgs e)
		{
			copyToolStripButton.PerformClick();
		}

		private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
		{
			pasteToolStripButton.PerformClick();
		}

		private void repeatLastActionToolStripMenuItem_Click(object sender, EventArgs e)
		{
			grid.RepeatLastAction(grid.SelectionRange);
		}

		private void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
		{
			grid.SelectAll();
		}
		#endregion

		#region View & Print
		private void freezeToCellToolStripMenuItem_Click(object sender, EventArgs e)
		{
			grid.FreezeToCell(grid.SelectionRange.StartPos);
			UpdateMenuAndToolStrips();
		}

		private void unfreezeToolStripMenuItem_Click(object sender, EventArgs e)
		{
			grid.Unfreeze();
			UpdateMenuAndToolStrips();
		}

		private void formatCellToolStripMenuItem_Click(object sender, EventArgs e)
		{
			using (PropertyForm form = new PropertyForm(grid))
			{
				form.ShowDialog(this);
			}
		}

		private void printPreviewToolStripMenuItem_Click(object sender, EventArgs e)
		{
			printPreviewToolStripButton.PerformClick();
		}

		private void showGridToolStripMenuItem_Click(object sender, EventArgs e)
		{
			grid.SetSettings(ReoGridSettings.View_ShowGridLine, showGridToolStripMenuItem.Checked);
		}

		private void printPreviewToolStripButton_Click(object sender, EventArgs e)
		{
			using (PrintDocument doc = grid.CreatePrintDocument())
			{
				using (PrintPreviewDialog ppd = new PrintPreviewDialog())
				{
					ppd.SetBounds(200, 200, 1024, 768);
					ppd.Document = doc;
					ppd.PrintPreviewControl.Zoom = 1d;
					ppd.ShowDialog(this);
				}
			}
		}

		private void showHorizontaScrolllToolStripMenuItem_Click(object sender, EventArgs e)
		{
			grid.SetSettings(ReoGridSettings.View_ShowHorScroll, showHorizontaScrolllToolStripMenuItem.Checked);
		}

		private void showVerticalScrollbarToolStripMenuItem_Click(object sender, EventArgs e)
		{
			grid.SetSettings(ReoGridSettings.View_ShowVerScroll, showVerticalScrollbarToolStripMenuItem.Checked);
		}

		private void showColumnHeaderToolStripMenuItem_Click(object sender, EventArgs e)
		{
			grid.SetSettings(ReoGridSettings.View_ShowColumnHeader, showColumnHeaderToolStripMenuItem.Checked);
		}

		private void showRowIndexToolStripMenuItem_Click(object sender, EventArgs e)
		{
			grid.SetSettings(ReoGridSettings.View_ShowRowHeader, showRowHeaderToolStripMenuItem.Checked);
		}

		#endregion

		#region Help
		private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
		{
			new AboutForm().ShowDialog(this);
		}
		#endregion

		#region Outline

		void groupRowsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			try
			{
				grid.AddOutline(RowOrColumn.Row, grid.SelectionRange.Row, grid.SelectionRange.Rows);
			}
			catch (OutlineOutOfRangeException)
			{
				MessageBox.Show("Outline out of available range. The last row of spreadsheet cannot be grouped into outlines.");
			}
			catch (OutlineAlreadyDefinedException)
			{
				MessageBox.Show("Another same outline has already exist.");
			}
			catch (OutlineIntersectedException)
			{
				MessageBox.Show("The outline to be added intersects with another existed one.");
			}
			catch (OutlineTooMuchException)
			{
				MessageBox.Show("Level of outlines reached the maximum number of levels (10).");
			}
		}

		void ungroupRowsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			try
			{
				grid.RemoveOutline(RowOrColumn.Row, grid.SelectionRange.Row, grid.SelectionRange.Rows);
			}
			catch (OutlineNotFoundException)
			{
				MessageBox.Show("No grouped rows and outline found at specified position.");
			}
		}

		void groupColumnsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			try
			{
				grid.AddOutline(RowOrColumn.Column, grid.SelectionRange.Col, grid.SelectionRange.Cols);
			}
			catch (OutlineOutOfRangeException)
			{
				MessageBox.Show("Outline out of available range. The last column of spreadsheet cannot be grouped into outlines.");
			}
			catch (OutlineAlreadyDefinedException)
			{
				MessageBox.Show("Another outline which same as selected one has already exist.");
			}
			catch (OutlineIntersectedException)
			{
				MessageBox.Show("The outline to be added intersects with another existed one.");
			}
			catch (OutlineTooMuchException)
			{
				MessageBox.Show("Level of outlines reached the maximum number of levels (10).");
			}
		}

		void ungroupColumnsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			try
			{
				grid.RemoveOutline(RowOrColumn.Column, grid.SelectionRange.Col, grid.SelectionRange.Cols);
			}
			catch (OutlineNotFoundException)
			{
				MessageBox.Show("No grouped columns and outline found at specified position.");
			}
		}
		
		void ungroupAllRowsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			grid.ClearOutlines(RowOrColumn.Row);
		}

		void ungroupAllColumnsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			grid.ClearOutlines(RowOrColumn.Column);
		}

		#endregion

#if DEBUG
		private void ForTest()
		{
			//------------------------
			//grid.SetRangeBorder(new StyleGridRange(5, 4, 2, 3), StyleGridBorderPos.All, new StyleGridBorderStyle
			//{
			//  Color = Color.Black,
			//  Style = BorderLineStyle.Solid,
			//});

			//------------------------
			//var testRange = new ReoGridRange(2, 2, 7, 7);

			//grid.SetRangeBorder(testRange, ReoGridBorderPos.Outline, new ReoGridBorderStyle
			//{
			//	Color = Color.Black,
			//	Style = BorderLineStyle.Dashed,
			//});

			//grid.SetRangeData(testRange, new object[,]{
			//	{1,2,3,4,5,6,7},
			//	{11,12,13,14,15,16,17},
			//	{21,22,23,24,25,26,27},
			//	{31,32,33,34,35,36,37},
			//	{41,42,43,44,45,46,47},
			//	{51,52,53,54,55,56,57},
			//	{61,62,63,64,65,66,67},
			//});

			//grid.MergeRange(testRange);

			//grid.DeleteRows(5, 5);
			//------------------------

			//grid.InsertRows(5, 2);

			//grid.SetRangeData(new StyleGridRange(10, 3, 2, 5), new object[,]{
			//  {"abc","hello world", "123","123.456","-990011"},
			//  {"2009/4/1","13:15", "35%", "12.5%", "-200%"},
			//});

			//grid.FreezeToCell(new StyleGridPos(4, 0));
			//grid.SetRangeStyle(new StyleGridRange(0, 0, 10, 10), new StyleGridStyle
			//{
			//  Flag = PlainStyleFlag.FillPattern,
			//  FillPatternColor = Color.Silver,
			//  FillPatternStyle = HatchStyle.Percent30,
			//  BackColor = Color.White,
			//});

			//grid.SetSettings(StyleGridSettings.Readonly);

			//grid.SetRows(2000);


			//------------------------

			//grid.SetRangeStyle(new ReoGridRange(2, 2, 1, 1), new ReoGridRangeStyle
			//{
			//	Flag = PlainStyleFlag.TextWrap,
			//	TextWrapMode = TextWrapMode.WordBreak,
			//});

			//grid[2, 2] = "This is a test for text-wrap";
			//------------------------

			//var cell = grid.CreateAndGetCell(1, 1);
			//------------------------

			//grid.SetRows(1);
			//grid.MergeRange(new ReoGridRange(0, 2, 1, 10));

			//grid.DeleteCols(10, 4);
			//grid.SelectRange(new ReoGridRange(0, 1, 1, 2));
			//------------------------

			//grid.MergeRange(2, 2, 2, 3);
			//grid.MergeRange(2, 6, 2, 3);

			//grid.DoAction(new RGRemoveColumnsAction(4, 3));
			//grid.DeleteCols(4, 3);

			//grid.SetRangeBorder(new ReoGridRange(2, 1, 2, 5), ReoGridBorderPos.Outline, ReoGridBorderStyle.SolidBlack);
			//grid.SetRangeBorder(new ReoGridRange(2, 8, 2, 5), ReoGridBorderPos.Outline, ReoGridBorderStyle.SolidBlack);

			//grid.DeleteCols(6, 3);

			//------------------------

			//Show();
			//var range = new ReoGridRange(0, 0, 4, 4);
			//grid.SetRangeBorder(range, ReoGridBorderPos.All, ReoGridBorderStyle.SolidBlack);

			//var subgrid = grid.GetPartialGrid(range);

			//grid[0, 5] = subgrid;

			//var newsg = new PartialGrid(4, 4);
			//grid[0, 0] = newsg;

			//var range = new ReoGridRange(1, 1, 12, 12);
			//grid.SetRangeBorder(range, ReoGridBorderPos.All, ReoGridBorderStyle.SolidBlack);

			//var subgrid = new PartialGrid(4, 4);

			////grid[1, 1] = subgrid; // left-top
			////grid[9, 1] = subgrid; // left-bottom
			////grid[1, 9] = subgrid; // right-top
			//grid[9, 9] = subgrid; // top

			////AssertTrue(grid._Debug_Validate_All());

			//// left-top
			//var borderInfo = grid.GetRangeBorder(new ReoGridRange(0, 0, 4, 4));
			////AssertEquals(borderInfo.Top.Color, Color.Empty);
			//// left-bottom
			//borderInfo = grid.GetRangeBorder(new ReoGridRange(4, 0, 4, 4));
			////AssertEquals(borderInfo.Top.Color, Color.Empty);
			//// right-top
			//borderInfo = grid.GetRangeBorder(new ReoGridRange(0, 8, 4, 4));
			////AssertEquals(borderInfo.Top.Color, Color.Empty);
			//// right-bottom
			//borderInfo = grid.GetRangeBorder(new ReoGridRange(8, 8, 4, 4));
			////AssertEquals(borderInfo.Top.Color, Color.Empty);

			//_Debug_Auto_Validate_All();
			//------------------------

			//grid.MergeRange(new ReoGridRange(2, 4, 2, 4));
			//grid.SetRangeBorder(new ReoGridRange(2, 4, 2, 4), ReoGridBorderPos.Outline, ReoGridBorderStyle.SolidBlack);
			//grid.DoAction(new RGRemoveColumnsAction(5, 2));
			//grid.Undo();

			//------------------------

			//PartialGrid pg = null;

			//grid.MergeRange(2, 2, 2, 4);
			//var pg = grid.GetPartialGrid(2, 4, 2, 2);
			//grid[2, 6] = pg;

			//grid.MergeRange(6, 2, 1, 4);
			//pg = grid.GetPartialGrid(6, 4, 1, 2);
			//grid[6, 6] = pg;

			//grid.MergeRange(8, 2, 1, 4);
			//pg = grid.GetPartialGrid(8, 2, 1, 2);
			//grid[8, 0] = pg;

			//grid.MergeRange(10, 2, 2, 4);
			//pg = grid.GetPartialGrid(10, 2, 2, 2);
			//grid[10, 0] = pg;

			//------------------------

			//var range = new ReoGridRange(8, 4, 4, 4);
			//grid.MergeRange(range);

			//var subgrid = grid.GetPartialGrid(range);

			//grid[4, 4] = subgrid; // top
			//grid[8, 0] = subgrid; // left
			//grid[12, 4] = subgrid; // bottom
			//grid[8, 8] = subgrid; // right

			//grid[12, 8] = subgrid; // right
			//------------------------

			//grid.MergeRange(2, 4, 20, 2);
			//grid.SetRangeBorder(new ReoGridRange(2, 4, 20, 2), ReoGridBorderPos.Outline, ReoGridBorderStyle.SolidBlack);

			//var pg = grid.GetPartialGrid(20, 4, 2, 2);
			//grid[21, 4] = pg;

			//Show();
			//Application.DoEvents();

			//MessageBox.Show("a");

			//grid.MergeRange(10, 8, 2, 2);
			//grid.SetRangeBorder(new ReoGridRange(10, 8, 2, 2), ReoGridBorderPos.Outline, ReoGridBorderStyle.SolidBlack);
			//------------------------

			//grid.MergeRange(4, 4, 1, 2);
			//grid.MergeRange(4, 6, 1, 2);
			//grid.SetRangeBorder(4, 4, 1, 4, ReoGridBorderPos.All, ReoGridBorderStyle.SolidBlack);
			//------------------------

			//var range = new ReoGridRange(4, 4, 4, 4);
			//grid.MergeRange(range);

			//var subgrid = grid.GetPartialGrid(range);

			//grid[0, 4] = subgrid; // top
			//grid[4, 0] = subgrid; // left
			//grid[8, 4] = subgrid; // bottom
			//grid[4, 8] = subgrid; // right
			//------------------------

			//grid.SetRangeBorder(new ReoGridRange(1, 1, 2, 10), ReoGridBorderPos.Outline, ReoGridBorderStyle.SolidBlack);
			//grid.SetRangeBorder(new ReoGridRange(1, 5, 10, 2), ReoGridBorderPos.Outline, ReoGridBorderStyle.SolidBlack);
			//------------------------
			//grid.Resize(20, 20);

			//grid.MergeRange(2, 1, 8, 8);
			//grid.MergeRange(10, 10, 5, 8);

			//grid.DeleteCols(5, 3);
			//------------------------

			//var myCell = new MyCellBody();
			//grid[2, 2] = myCell;

			//grid[2, 4] = new DropdownCell(grid);

			//grid[0, 3] = new MyData();
			//------------------------
			//grid.SetSettings(ReoGridSettings.Edit_AllowUserScale, false);
			//------------------------
			//grid.AddOutline(OutlineFlag.Row, 4, 2);

			//grid.RemoveOutline(OutlineFlag.Row, 4, 2);
			//grid.AddOutline(OutlineFlag.Row, 2, 6);
			//grid.CollapseRowOutline(4);
			//grid.FreezeToCell(3, 3);
			//------------------------
			//for (int i = 1; i < 4; i++)
			//{
			//	grid.AddOutline(RowOrColumn.Column, 3, i + 3);
			//}

			//for (int i = 1; i < 4; i++)
			//{
			//	grid.AddOutline(RowOrColumn.Row, 3, i + 3);
			//}
			//------------------------

			//grid[1, 1] = "hello";

			//grid.SetRangeStyle(new ReoGridRange(1, 1, 1, 1), new ReoGridRangeStyle
			//{
			//	// set both back color and bold font
			//	Flag = PlainStyleFlag.FillColor | PlainStyleFlag.FontStyleBold,
			//	BackColor = Color.LightGray,
			//	Bold = true,
			//});

			//grid.Srm["IF"] = new NativeFunctionObject("IF", (ctx, owner, args) => {
			//	if(args.Length < 3) throw new Exception();// .... exception

			//	return ScriptRunningMachine.GetBoolValue(args[0]) ? args[1] : args[2];
			//});

			//grid.SetRangeBorder(new ReoGridRange(1, 1, 1, 1), ReoGridBorderPos.All,
			//	new ReoGridBorderStyle
			//	{
			//		// grid line color of control style 
			//		Color = grid.ControlStyle[ReoGridControlColors.GridLine],
			//		Style = BorderLineStyle.Solid
			//	});

			//------------------------
			//grid.Srm["CEILING"] = new NativeFunctionObject("CEILING", (ctx, owner, args) =>
			//{
			//	if (args.Length < 1) return double.NaN;

			//	double input = ScriptRunningMachine.GetDoubleValue(args[0]);

			//	if (args.Length < 2)
			//	{
			//		return Math.Ceiling(input);
			//	}

			//	double significance = ScriptRunningMachine.GetDoubleValue(args[1]);

			//	if ((input % significance) != 0)
			//	{
			//		return ((int)(input / significance) * significance) + significance;
			//	}
			//	else
			//	{
			//		return Convert.ToInt32(input);
			//	}
			//});

			//grid[1, 1] = "=CEILING(101/5, 1)";
		}
#endif
	}
}
