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
 *****************************************************************************
 * 
 *   LeadHead                            Header
 *           \                             |
 *          +-\---+---------------+--------|-------+----------------+
 *          |     |        A      |        B       |        C       |
 *          +-----+---------------+----------------+----------------+
 *          |  1  |     Cell A1   |     Cell B1    |     Cell C1    |
 *          +-----+---------------+----------------+----------------+
 *          |  2  |     Cell A2   |     Cell B2    |     Cell C2    |
 *          +--|--+---------------+----------------+----------------+
 *             |
 *         Row Index
 * 
 **** Cell Data Struct ********************************************************
 * 
 *   Page Size (Row)        = 128
 *   Page Size (Column)     = 32
 *   Tree Depth             = 3
 *   
 *   Rows      up to 16^5   = 1048576
 *   Columns   up to 8^5    = 32768
 *   
 *   Implementation class:  RegularTreeArray
 *                               
 *     ---             +----------------------+
 *      |              +  128x32 Page Index   +  
 *      |              +----------------------+
 *      |                   /          \
 *    depth:      +------------+      +------------+
 *      3         +  Sub Page  +      +  Sub Page  +
 *      |         +------------+      +------------+
 *      |           /                            \
 *      |        ...                              ...
 *      |       /    \                           /    \
 *     ---   ...      ...                      ...     ...
 *         /\           /\                    /\         /\
 *    Cells Data    Cells Data    ...     Cells Data    Cells Data
 *
 * 
 **** Formula & Script ********************************************************
 * 
 *   ReoGrid uses ReoScript to implement the formula evaluation and script 
 *   execution.
 *   
 *   ReoScript is ECMAScript-like .NET Script Language Engine, provided by
 *   unvell too, free and open source. More info available at:
 *   http://reoscript.codeplex.com
 *   http://www.unvell.com/ReoScript
 *   
 *   Open the script editor from ReoGrid Editor, run the following script:
 *   
 *   function hello() {
 *     alert('hello world!');
 *   }
 *   
 *   hello();
 *   
 *   The message box shown and 'hello world!' will be printed out:
 *   
 *                  +-----------------------------------+
 *                  |                                   |
 *                  |  hello world!                     |
 *                  |                                   |
 *                  |                       +------+    |
 *                  |                       |  OK  |    |
 *                  |                       +------+    |
 *                  +-----------------------------------+
 *   
 *   ReoGrid provides the API can be used to access control in script.
 *   Global object 'grid' always point to the instance of current control.
 *   'grid' object provides many method like 'getCell' which can be used to
 *   retrieve cell with specified position, e.g.:
 *   
 *       grid.getCell(0, 0).data = 'hello world';
 *       
 *   The data of Cell[0,0] will be changed and 'hello world' will be printed
 *   out.
 *   
 *   Script expression can be used as formula in cell. The following
 *   expressions are available:
 *   
 *       =A1 + B2
 *       =A1 + (B2 + C3) * 2
 *       =A3=="yes"?"true":"false"
 * 
 *   since A1:C3 is not an valid expression in ReoScript, it will be converted
 *   into 'new Range(0, 1, 3, 3)' before expression execution.
 *
 *       =sun(A1:C3)      ->    =sun(new Range(0, 0, 3, 3))
 *       =avg(B3:B10)     ->    =avg(new Range(1, 2, 1, 7))
 * 
 *   Call customize function is also valid expression, 
 *   it can be used as formula:
 *   
 *       =hello()
 *       =hello(A1:C3)    ->    =hello(new Range(0, 0, 3, 3))
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
using System.Windows.Forms.VisualStyles;
using System.Xml.Serialization;
using System.Threading;
using System.Globalization;
using System.Drawing.Printing;
using System.Text.RegularExpressions;

using unvell.Common;
using unvell.Common.Win32Lib;

using unvell.ReoGrid.XML;
using unvell.ReoGrid.Properties;
using unvell.ReoGrid.DataFormat;
using unvell.ReoGrid.Actions;
using unvell.ReoGrid.Data;

#if EX_SCRIPT
using unvell.ReoScript;
using unvell.ReoGrid.Script;
#endif

namespace unvell.ReoGrid
{
	public partial class ReoGridControl : Control
	{
		#region Constants
		/// <summary>
		/// Default width of column
		/// </summary>
		public static readonly ushort InitDefaultColumnWidth = 70;

		/// <summary>
		/// Default height of row
		/// </summary>
		public static readonly ushort InitDefaultRowHeight = 20;

		/// <summary>
		/// Default number of columns
		/// </summary>
		public static readonly int DefaultCols = 100;
		
		/// <summary>
		/// Default number of rows
		/// </summary>
		public static readonly int DefaultRows = 200;

		/// <summary>
		/// Default button size of outline buttons
		/// </summary>
		public static readonly int OutlineButtonSize = 13;

		/// <summary>
		/// Default root style of entire range of grid control
		/// </summary>
		public static readonly ReoGridRangeStyle DefaultStyle = new ReoGridRangeStyle
		{
			BackColor = Color.Empty,
			TextColor = Color.Black,
			Flag = PlainStyleFlag.FillColor | PlainStyleFlag.TextColor
				| PlainStyleFlag.FontName | PlainStyleFlag.FontSize | PlainStyleFlag.AlignAll,
			FontName = SystemFonts.DefaultFont.Name,
			FontSize = SystemFonts.DefaultFont.Size,
			//FontStyle = SystemFonts.DefaultFont.Style,
			HAlign = ReoGridHorAlign.General,
			VAlign = ReoGridVerAlign.Middle,
		};
		#endregion

		#region Constructor & Initialize
		private Cursor defaultGridSelectCursor = null;
		private Cursor defaultPickRangeCursor = null;

		/// <summary>
		/// Create instance for ReoGridControl
		/// </summary>
		public ReoGridControl()
		{
			SuspendLayout();

			BackColor = Color.White;
			DoubleBuffered = true;
			TabStop = true;

			bottomPanel = new Panel()
			{
				Size = new Size(SystemInformation.HorizontalScrollBarHeight, SystemInformation.VerticalScrollBarWidth),
				Dock = DockStyle.Bottom,
				BackColor = SystemColors.Control,
				TabStop = false,
			};
			bottomPanel.Controls.Add(new ScrollBarCorner()
			{
				Dock = DockStyle.Right,
				Size = new Size(SystemInformation.HorizontalScrollBarHeight, SystemInformation.VerticalScrollBarWidth),
				BackColor = SystemColors.Control,
				TabStop = false,
			});
			rightPanel = new Panel()
			{
				Size = new Size(SystemInformation.VerticalScrollBarWidth, SystemInformation.HorizontalScrollBarHeight),
				Dock = DockStyle.Right,
				BackColor = SystemColors.Control,
				TabStop = false,
			};

			Controls.Add(rightPanel);
			Controls.Add(bottomPanel);

			rightPanel.MouseMove += (sender, e) => Cursor = Cursors.Default;
			bottomPanel.MouseMove += (sender, e) => Cursor = Cursors.Default;

			editTextbox = new InputTextBox(this) { Visible = false, BorderStyle = BorderStyle.None };
			//numericTextbox = new NumericField(this) { Visible = false };
			//dropWindow = new DropdownWindow(this) { Visible = false };

			Controls.Add(editTextbox);

			ResumeLayout();

			//TODO: detect clipboard changes
			// need detect and remove the hightlight range when content has been removed from System Clipboard
			//ClipboardMonitor.Instance.ClipboardChanged += new EventHandler<ClipboardChangedEventArgs>(ClipboardMonitor_ClipboardChanged);

			// initialize cursors
			// normal grid selector
			defaultGridSelectCursor = LoadCursorFromResource(Resources.grid_select);
			gridSelectCursor = defaultGridSelectCursor;
			cellsSelectionCursor = defaultGridSelectCursor;

			// cell picking
			defaultPickRangeCursor = LoadCursorFromResource(Resources.pick_range);

			// full-row and full-col selector
			fullColSelectCursor = LoadCursorFromResource(Resources.full_col_select);
			fullRowSelectCursor = LoadCursorFromResource(Resources.full_row_select);

			InitGrid();

			viewportController = new NormalViewportController(this);

			// initialize default settings
			settings =
				// allow all edit behavior
				ReoGridSettings.Edit_All
				// show grids
				| ReoGridSettings.View_ShowGridLine
				// show scrollbars
				| ReoGridSettings.View_ShowScrolls;

#if EX_SCRIPT
			settings |=
				// auto run script if loaded from file
				  ReoGridSettings.Script_AutoRunOnload
		    // confirm to user whether allow to run script after loaded from file
				| ReoGridSettings.Script_PromptBeforeAutoRun;
#endif // EX_SCRIPT

			// default control styles
			SetControlStyle(CreateDefaultControlStyle());

			// register for moniting reusable action
			ActionManager.InstanceForObject(this).AfterPerformAction += (s, e) =>
			{
				if (e.Action is RGReusableAction) lastReusableAction = e.Action as RGReusableAction;
			};

			pageSettings = new PageSettings();

#if DEBUG
			Logger.RegisterWritter(RGDebugLogWritter.Instance);
#endif
		}

		protected override void OnCreateControl()
		{
			base.OnCreateControl();

			ScrollToFocusCell();

			UpdateCanvasBounds();

			UpdateViewportController();

			Focus();
		}

		~ReoGridControl()
		{
			if (defaultGridSelectCursor != null) defaultGridSelectCursor.Dispose();
			if (defaultPickRangeCursor != null) defaultPickRangeCursor.Dispose();
			if (fullColSelectCursor != null) fullColSelectCursor.Dispose();
			if (fullRowSelectCursor != null) fullRowSelectCursor.Dispose();
		}
		#endregion // Constructor & Initialize

		#region Header & Row Index
		private List<ReoGridColHead> cols = new List<ReoGridColHead>(DefaultCols);
		private List<ReoGridRowHead> rows = new List<ReoGridRowHead>(DefaultRows);

		private int colHeaderHeight = 18;
		private int rowHeaderWidth = 40;
		private ushort defaultColumnWidth = InitDefaultColumnWidth;
		private ushort defaultRowHeight = InitDefaultRowHeight;

		/// <summary>
		/// Set width of specified columns
		/// </summary>
		/// <param name="col">Start column index to set</param>
		/// <param name="count">Number of columns to set</param>
		/// <param name="width">Width value of column</param>
		public void SetColsWidth(int col, int count, ushort width)
		{
			SetColsWidth(col, count, c => width, true);
		}
		internal void SetColsWidth(int col, int count, Func<int, int> widthGetter, bool processOutlines)
		{
#if DEBUG
			Stopwatch watch = Stopwatch.StartNew();
#endif

			int endcol = col + count;
			int offset = 0;
			var colOutlines = GetOutlines(RowOrColumn.Column);

			int maxRow = Math.Min(this.rows.Count, cells.MaxRow + 1);
			int maxCol = this.cols.Count;

			for (int c = col; c < maxCol; c++)
			{
				var colhead = this.cols[c];
				colhead.Left += offset;

				int w = colhead.Width;
				int width = 0;
				bool skipped = false;

				if (c < endcol)
				{
					width = widthGetter(c);

					// skip this column when width < 0
					if (width >= 0)
					{
						// if both target width and current column's width are zero,
						// then skip adjusting column width
						if (width == 0 && colhead.Width <= 0)
						{
							skipped = true;
						}
						else
						{
							colhead.LastWidth = colhead.Width;
							colhead.Width = (ushort)width;

							if (width > 0)
							{
								#region Expand
								if (processOutlines && colOutlines != null)
								{
									colOutlines.IterateOutlines(o =>
									{
										if (o.End == c + 1 && o.Collapsed)
										{
											o.Collapsed = false;
											return false;
										}
										return true;
									});
								}
								#endregion
							}
							else // if height == 0 then collapse outlines
							{
								#region Collapse
								if (processOutlines && colOutlines != null)
								{
									colOutlines.IterateOutlines(o =>
									{
										if (o.End == c + 1 && !o.Collapsed)
										{
											bool collapse = true;

											// check all rows are non-hide
											for (int k = o.Start; k < o.End; k++)
											{
												if (this.cols[k].Width > 0)
												{
													collapse = false;
													break;
												}
											}

											if (collapse)
											{
												o.Collapsed = true;
												return false;
											}
										}
										return true;
									});
								}
								#endregion
							}
						}
					}
					else
					{
						// width must be >= zero
						width = 0;
					}
				}

				#region Offset Cells
				for (int r = 0; r < maxRow; r++)
				{
					ReoGridCell cell = cells[r, c];

					if (cell != null)
					{
						cell.Left += offset;
						cell.TextBoundsLeft += offset;

						// update non-merged cell
						if (cell.Col < endcol && cell.Colspan == 1 && cell.Rowspan == 1)
						{
							cell.Width = width + 1;
							UpdateCellTextBounds(null, cell, false);
						}
						// update merged cell
						else if (ReoGridPos.Equals(cell.MergeEndPos, cell.Pos))
						{
							ReoGridCell mergedStartCell = GetCell(cell.MergeStartPos);
							UpdateCellBounds(mergedStartCell);
						}
					}
				}
				#endregion

				if (c < endcol && !skipped)
				{
					offset += width - w;
				}
			}

			UpdateViewportController();

#if DEBUG
			watch.Stop();

			if (watch.ElapsedMilliseconds > 5)
			{
				Debug.WriteLine(string.Format("columns width change takes {0} ms.", watch.ElapsedMilliseconds));
			}
#endif
		}

		/// <summary>
		/// Set height of specified rows
		/// </summary>
		/// <param name="row">Start row index to set</param>
		/// <param name="count">Number of rows to set</param>
		/// <param name="height">Height value of row</param>
		public void SetRowsHeight(int row, int count, ushort height)
		{
			SetRowsHeight(row, count, r => height, true);
		}

		internal void SetRowsHeight(int row, int count, Func<int, int> heightGetter, bool processOutlines)
		{
#if DEBUG
			Stopwatch watch = Stopwatch.StartNew();
#endif

			int endrow = row + count;
			int offset = 0;
			var rowOutlines = GetOutlines(RowOrColumn.Row);

			int maxRow = this.rows.Count;
			int maxCol = Math.Min(this.cols.Count, cells.MaxCol + 1);

			for (int r = row; r < maxRow; r++)
			{
				var rowhead = rows[r];
				rowhead.Top += offset;

				int h = rowhead.Height;
				int height = 0;
				bool skiped = false;

				if (r < endrow)
				{
					height = heightGetter(r);

					// skip this row when height < 0
					if (height >= 0)
					{
						// if both target height and current row's height are zero,
						// then skip adjusting row height
						if (height == 0 && rowhead.Height <= 0)
						{
							skiped = true;
						}
						else
						{
							rowhead.LastHeight = rowhead.Height;
							rowhead.Height = (ushort)height;

							if (height > 0)
							{
								#region Expand
								if (processOutlines && rowOutlines != null)
								{
									rowOutlines.IterateOutlines(o =>
									{
										if (o.End == r + 1 && o.Collapsed)
										{
											o.Collapsed = false;
											return false;
										}
										return true;
									});
								}
								#endregion
							}
							else // if height == 0 then collapse outlines
							{
								#region Collapse
								if (processOutlines && rowOutlines != null)
								{
									rowOutlines.IterateOutlines(o =>
									{
										if (o.End == r + 1 && !o.Collapsed)
										{
											bool collapse = true;

											// check all rows are non-hide
											for (int k = o.Start; k < o.End; k++)
											{
												if (this.rows[k].Height > 0)
												{
													collapse = false;
													break;
												}
											}

											if (collapse)
											{
												o.Collapsed = true;
												return false;
											}
										}
										return true;
									});
								}
								#endregion
							}
						}
					}
					else
					{
						// height must be >= zero
						height = 0;
					}
				}

				#region Offset Cells
				for (int c = 0; c < maxCol; c++)
				{
					ReoGridCell cell = cells[r, c];

					if (cell != null)
					{
						cell.Top += offset;
						cell.TextBoundsTop += offset;

						// update unmerged cell
						if (cell.Row < endrow && cell.Colspan == 1 && cell.Rowspan == 1)
						{
							cell.Height = height + 1;
							UpdateCellTextBounds(null, cell, false);  // TODO: need performance improvement
						}
						// update merged cell
						else if (cell.IsEndMergedCell)
						{
							ReoGridCell mergedStartCell = GetCell(cell.MergeStartPos);
							UpdateCellBounds(mergedStartCell);
						}
					}
				}
				#endregion

				if (r < endrow && !skiped)
				{
					offset += height - h;
				}
			}

			UpdateViewportController();

#if DEBUG
			watch.Stop();
			long ms = watch.ElapsedMilliseconds;

			if (ms > 5)
			{
				Debug.WriteLine(string.Format("row height change takes {0} ms.", ms));
			}
#endif
		}

		/// <summary>
		/// Get width from specified column
		/// </summary>
		/// <param name="col">Column index to get</param>
		/// <returns>Width value of specified column</returns>
		public ushort GetColumnWidth(int col)
		{
			if (col < 0 || col >= ColCount)
			{
				throw new ArgumentOutOfRangeException("col", "invalid index to number of column");
			}
			return cols[col].Width;
		}

		/// <summary>
		/// Get height from specified row
		/// </summary>
		/// <param name="row">Row index to get</param>
		/// <returns>Height value of specified row</returns>
		public ushort GetRowHeight(int row)
		{
			if (row < 0 || row >= RowCount)
			{
				throw new ArgumentOutOfRangeException("row", "invalid index to number of row");
			}
			return rows[row].Height;
		}

		/// <summary>
		/// Append specified columns at right of grid
		/// </summary>
		/// <param name="count">number of columns</param>
		public void AppendCols(int count)
		{
			if (this.cols.Count + count >= this.cells.RowCapacity)
			{
				throw new ArgumentOutOfRangeException("count", 
					"number of columns to append exceeds maximum columns: " + this.cells.ColCapacity);
			}

			int x = cols.Count == 0 ? 0 : cols[cols.Count - 1].Right;
			int total = cols.Count + count;

			for (int i = cols.Count; i < total; i++)
			{
				cols.Add(new ReoGridColHead
				{
					Width = defaultColumnWidth,
					Col = i,
					Code = RGUtility.GetAlphaChar(i),
					Left = x
				});

				x += defaultColumnWidth;
			}

			UpdateViewportController();
		}

		/// <summary>
		/// Append specified rows at bottom of grid
		/// </summary>
		/// <param name="count">number of rows</param>
		public void AppendRows(int count)
		{
			if (this.cols.Count + count >= this.cells.RowCapacity)
			{
				throw new ArgumentOutOfRangeException("count", 
					"number of rows to append exceeds maximum rows: " + this.cells.RowCapacity);
			}

			int y = rows.Count == 0 ? 0 : rows[rows.Count - 1].Bottom;
			int total = rows.Count + count;

			for (int i = rows.Count; i < total; i++)
			{
				rows.Add(new ReoGridRowHead { Height = defaultRowHeight, Row = i, Top = y, IsAutoHeight = true });
				y += defaultRowHeight;
			}

			UpdateViewportController();
		}

		/// <summary>
		/// Resize grid to specified number of rows and cols
		/// </summary>
		/// <param name="rows">number of rows to resize</param>
		/// <param name="cols">number of columns to resize</param>
		public new void Resize(int rows, int cols)
		{
			if (cols > 0)
			{
				if (cols > this.cols.Count)
				{
					AppendCols(cols - this.cols.Count);
				}
				else if (cols < this.cols.Count)
				{
					DeleteCols(cols, this.cols.Count - cols);
				}
			}

			if (rows > 0)
			{
				if (rows > this.rows.Count)
				{
					AppendRows(rows - this.rows.Count);
				}
				else if (rows < this.rows.Count)
				{
					DeleteRows(rows, this.rows.Count - rows);
				}
			}
		}

		/// <summary>
		/// Set number of columns (up to 32768)
		/// </summary>
		/// <param name="colCount">Number of columns</param>
		public void SetCols(int colCount)
		{
			Resize(-1, colCount);
		}

		/// <summary>
		/// Set number of rows (up to 1048576)
		/// </summary>
		/// <param name="rowCount">Number of rows</param>
		public void SetRows(int rowCount)
		{
			Resize(rowCount, -1);
		}

		[Obsolete("Use InsertCols instead using this method.")]
		/// <summary>
		/// Insert column before specified column index
		/// </summary>
		/// <param name="col">index of column to insert</param>
		public void InsertCol(int col)
		{
			if (col == this.cols.Count)
			{
				AppendCols(1);
				return;
			}

			InsertCols(col, 1);
		}

		/// <summary>
		/// Insert rows before specified row index
		/// </summary>
		/// <param name="row">index of row</param>
		/// <param name="count">number of rows</param>
		public void InsertCols(int col, int count)
		{
			if (col > this.cols.Count)
			{
				throw new ArgumentOutOfRangeException("col");
			}

			if (cols.Count + count > cells.ColCapacity)
			{
				throw new ArgumentOutOfRangeException("count");
			}

			if (count < 1)
			{
				throw new ArgumentException("count must be >= 1");
			}

			if (col == this.cols.Count)
			{
				AppendCols(count);
				return;
			}

#if DEBUG
			Stopwatch watch = Stopwatch.StartNew();
#endif

			#region insert outlines
			var outlines = GetOutlines(RowOrColumn.Column);
			if (outlines != null)
			{
				outlines.IterateOutlines(o =>
				{
					if (o.Start > col)
					{
						o.Start += count;
					}
					else if (o.End > col)
					{
						o.Count += count;
					}
					return true;
				});
			}
			#endregion

			#region insert headers
			int x = col == 0 ? 0 : cols[col - 1].Right;
			ushort width = cols[col].Width;

			ReoGridColHead[] headers = new ReoGridColHead[count];

			int left = x;
			for (int i = 0; i < count; i++)
			{
				int index = col + i;

				headers[i] = new ReoGridColHead
				{
					Col = index,
					Code = RGUtility.GetAlphaChar(index),
					Left = left,
					Width = width,
					Style = cols[col].Style == null ? null : new ReoGridRangeStyle(cols[col].Style),
				};

				left += width;
			}

			// insert row header
			cols.InsertRange(col, headers);

			int totalWidth = width * count;

			#endregion

			#region move columns
			// TODO: can be optimized by moving entrie page in RegularTreeArray
			for (int c = this.cols.Count; c > col + count - 1; c--)
			{
				if (c != this.cols.Count)
				{
					int newCol = cols[c].Col + count;
					cols[c].Col = newCol;
					cols[c].Code = RGUtility.GetAlphaChar(newCol);
					cols[c].Left += totalWidth;
				}

				// move cells
				for (int r = this.rows.Count; r >= 0; r--)
				{
					cells[r, c] = cells[r, c - count];

					ReoGridCell cell = cells[r, c];

					if (cell != null)
					{
						cell.Col += count;
						cell.Left += totalWidth;
						cell.TextBoundsLeft += totalWidth;

						// move start pos
						if (!cell.MergeStartPos.IsEmpty && cell.MergeStartPos.Col >= col)
						{
							cell.MergeStartPos = cell.MergeStartPos.Offset(0, count);
						}

						// move end pos
						if (!cell.MergeEndPos.IsEmpty)
						{
							cell.MergeEndPos = cell.MergeEndPos.Offset(0, count);
						}
					}

					// move borders
					vBorders[r, c] = vBorders[r, c - count];
					hBorders[r, c] = hBorders[r, c - count];
				}
			}
			#endregion

			#region insert cols

			// TODO: can be optimized by moving entrie page in RegularTreeArray
			for (int c = col; c < col + count; c++)
			{
				for (int r = 0; r < this.rows.Count; r++)
				{
					hBorders[r, c] = null;
					vBorders[r, c] = null;
					cells[r, c] = null;
				}
			}

			//int colspan = 1;
			for (int r = this.rows.Count; r >= 0; r--)
			{
				if (col == 0)
				{
					cells[r, col] = null;
				}

				// clear old border
				vBorders[r, col] = null;
				hBorders[r, col] = null;

				#region insert horizontal border

				// insert horizontal border
				bool hhasLeft = col > 0 && hBorders[r, col - 1] != null && hBorders[r, col - 1].Border != null;
				bool hhasRight = hBorders[r, col + count] != null && hBorders[r, col + count].Border != null;

				// insert horizontal border if cell has both top and bottom borders
				if (hhasLeft && hhasRight)
				{
					// set horizontal border
					SetHBorders(r, col, count, hBorders[r, col - 1].Border, hBorders[r, col - 1].Pos);
				}

				#endregion

				// not last row
				if (r != this.rows.Count)
				{
					#region TODO: insert horizontal borders

					#endregion

					#region fill merged cell
					ReoGridCell prevCell = col <= 0 ? null : cells[r, col - 1];
					ReoGridCell nextCell = cells[r, col + count];

					ReoGridCell cell = cells[r, col] = null;

					bool isTopMerged = prevCell != null && prevCell.Rowspan != 1;
					bool isBottomMerged = nextCell != null && nextCell.Rowspan != 1;
					bool insideMergedRange = IsInsideSameMergedCell(prevCell, nextCell);

					if (insideMergedRange)
					{
						// fill empty columns inside current range
						for (int c = col; c < col + count; c++)
						{
							cell = CreateCell(r, c);
							cell.Colspan = 0;
							cell.Rowspan = 0;
							cell.MergeEndPos = prevCell.MergeEndPos.Offset(0, count);
							cell.MergeStartPos = prevCell.MergeStartPos;

							// cells inside range should be have an empty v-border and h-border
							if (r > cell.MergeStartPos.Row) hBorders[r, c] = new ReoGridHBorder();
							vBorders[r, c] = new ReoGridVBorder();
						}

						// find cells at left of inserted column offset it's merge-end-pos  
						// (merge-end-pos += number of inserted rows)
						for (int c = cell.MergeStartPos.Col; c < col; c++)
						{
							cells[r, c].MergeEndPos = cells[r, c].MergeEndPos.Offset(0, count);
						}

						// if range is splitted by inserted rows
						// the width of range should be expanded
						//
						// NOTE: only do this once when c is merge-start-row
						//
						if (r == cell.MergeStartPos.Row)
						{
							ReoGridCell startCell = GetCell(cell.MergeStartPos);
							startCell.Colspan += (short)count;
							startCell.Width += totalWidth;
						}
					}
					else
					{
						cells[r, col] = null;
					}

					#endregion
				}
			}

			#endregion // insert cols

			UpdateViewportController();

			// raise event
			if (ColInserted != null)
			{
				ColInserted(this, new RGColumnEventArgs(col));
			}

#if DEBUG
			watch.Stop();
			Debug.WriteLine("insert cols: " + watch.ElapsedMilliseconds + " ms.");
#endif
		}

		[Obsolete("Use InsertRows instead using this method")]
		/// <summary>
		/// Insert row before specified row index
		/// </summary>
		/// <param name="row">index of row to insert</param>
		public void InsertRow(int row)
		{
			if (row == this.rows.Count)
			{
				AppendRows(1);
				return;
			}

			InsertRows(row, 1);
		}

		/// <summary>
		/// Insert rows before specified row index
		/// </summary>
		/// <param name="row">index of row</param>
		/// <param name="count">number of rows</param>
		public void InsertRows(int row, int count)
		{
			if (row > this.rows.Count)
			{
				throw new ArgumentOutOfRangeException("row");
			}
			
			if (rows.Count + count > cells.RowCapacity)
			{
				throw new ArgumentOutOfRangeException("count");
			}

			if (count < 1)
			{
				throw new ArgumentException("count must be >= 1");
			}

			if (row == this.rows.Count)
			{
				AppendRows(count);
				return;
			}

#if DEBUG
			Stopwatch watch = Stopwatch.StartNew();
#endif

			#region insert outlines
			var outlines = GetOutlines(RowOrColumn.Row);
			if (outlines != null)
			{
				outlines.IterateOutlines(o =>
				{
					if (o.Start > row)
					{
						o.Start += count;
					}
					else if (o.End > row)
					{
						o.Count += count;
					}
					return true;
				});
			}
			#endregion

			#region insert headers
			int y = row == 0 ? 0 : rows[row - 1].Bottom;
			ushort height = rows[row].Height;

			ReoGridRowHead[] headers = new ReoGridRowHead[count];

			int top = y;
			for (int i = 0; i < count; i++)
			{
				headers[i] = new ReoGridRowHead
				{
					Row = row + i,
					Top = top,
					Height = height,
					Style = rows[row].Style == null ? null : new ReoGridRangeStyle(rows[row].Style),
					IsAutoHeight = true,
				};

				top += height;
			}
			
			// insert row header
			//rows.Insert(row, new ReoGridRowHead
			//{
			//	Row = row,
			//	Height = height,
			//	Style = rows[row].Style == null ? null : new ReoGridRangeStyle(rows[row].Style),
			//	Top = y,
			//	IsAutoHeight = true,
			//});
			rows.InsertRange(row, headers);

			int totalHeight = height * count;

			#endregion

			#region move rows
			// TODO: can be optimized by moving entrie page in RegularTreeArray
			for (int r = this.rows.Count; r > row + count - 1; r--)
			{
				if (r != this.rows.Count)
				{
					rows[r].Row += count;
					rows[r].Top += totalHeight;
				}

				// move cells
				for (int c = this.cols.Count; c >= 0; c--)
				{
					cells[r, c] = cells[r - count, c];

					ReoGridCell cell = cells[r, c];

					if (cell != null)
					{
						cell.Row += count;
						cell.Top += totalHeight;
						cell.TextBoundsTop += totalHeight;

						// move start pos
						if (!cell.MergeStartPos.IsEmpty && cell.MergeStartPos.Row >= row)
						{
							cell.MergeStartPos = cell.MergeStartPos.Offset(count, 0);
						}

						// move end pos
						if (!cell.MergeEndPos.IsEmpty)
						{
							cell.MergeEndPos = cell.MergeEndPos.Offset(count, 0);
						}
					}

					// move borders
					vBorders[r, c] = vBorders[r - count, c];
					hBorders[r, c] = hBorders[r - count, c];
				}
			}
			#endregion
			
			#region insert rows

			// TODO: can be optimized: to move entrie page by RegularTreeArray
			for (int r = row; r < row + count; r++)
			{
				for (int c = 0; c < this.cols.Count; c++)
				{
					hBorders[r, c] = null;
					vBorders[r, c] = null;
					cells[r, c] = null;
				}
			}

			//int colspan = 1;
			for (int c = this.cols.Count; c >= 0; c--)
			{
				if (row == 0)
				{
					cells[row, c] = null;
				}

				// clear old border
				vBorders[row, c] = null;
				hBorders[row, c] = null;

				#region insert vertial border

				// insert vertial border
				bool vhasTop = row > 0 && vBorders[row - 1, c] != null && vBorders[row - 1, c].Border != null;
				bool vhasBottom = vBorders[row + count, c] != null && vBorders[row + count, c].Border != null;

				// insert vertial border if cell has both top and bottom borders
				if (vhasTop && vhasBottom)
				{
					// set vertial border
					SetVBorders(row, c, count, vBorders[row - 1, c].Border, vBorders[row - 1, c].Pos);

					//for (int r = row; r < row + count; r++)
					//{
					//	// merge owner flag
					//	vBorders[r, c].Pos |= vBorders[row - 1, c].Pos;
					//}
				}

				#endregion

				// not last column
				if (c != this.cols.Count)
				{
					#region TODO: insert horizontal borders

					//// has old horizontal border?
					//if (hBorders[row + 1, c] != null && hBorders[row + 1, c].Border != null)
					//{
					//  // compare horizontal border from (+1,0) to (+1,+1)
					//  // if two borders are same, add colspan
					//  if (IsBorderSame(hBorders[row + 1, c], hBorders[row, c + 1])) colspan++;

					//  // get old border
					//  ReoGridHBorder hBorder = hBorders[row + 1, c];

					//  //
					//  // TODO: auto fix border
					//  //
					//  // old border is inner border of cell
					//  // need add a horizontal top border to current cell
					//  if (hBorder.Pos == ReoGridHBorderPosition.All)
					//  {
					//    //hBorders[row, c] = new ReoGridHBorder
					//    //{
					//    //  Border = hBorders[row + 1, c].Border,
					//    //  Cols = colspan,
					//    //  Pos = ReoGridHBorderPosition.All,
					//    //};
					//  }
					//  else if (hBorder.Pos == ReoGridHBorderPosition.Top)
					//  {

					//  }
					//  else if (hBorder.Pos == ReoGridHBorderPosition.Bottom)
					//  {
					//    //hBorders[row, c] = hBorders[row + 1, c];
					//    //hBorders[row + 1, c] = null;

					//    //for (int ck = c - 1; ck >= 0; ck--)
					//    //{
					//    //  if (hBorders[row + 1, ck] != null) hBorders[row + 1, ck].Cols--;
					//    //}
					//  }
					//}
					//else colspan = 1;

					#endregion

					#region fill merged cell
					ReoGridCell prevCell = row <= 0 ? null : cells[row - 1, c];
					ReoGridCell nextCell = cells[row + count, c];

					ReoGridCell cell = cells[row, c] = null;

					bool isTopMerged = prevCell != null && prevCell.Rowspan != 1;
					bool isBottomMerged = nextCell != null && nextCell.Rowspan != 1;
					bool insideMergedRange = IsInsideSameMergedCell(prevCell, nextCell);

					if (insideMergedRange)
					{
						// fill empty rows inside current range
						for (int r = row; r < row + count; r++)
						{
							cell = CreateCell(r, c);
							cell.Colspan = 0;
							cell.Rowspan = 0;
							cell.MergeEndPos = prevCell.MergeEndPos.Offset(count, 0);
							cell.MergeStartPos = prevCell.MergeStartPos;

							// cells inside range should be have an empty v-border and h-border
							if (c > cell.MergeStartPos.Col) vBorders[r, c] = new ReoGridVBorder();
							hBorders[r, c] = new ReoGridHBorder();
						}

						// find cells at above inserted row 
						// offset it's merge-end-pos to apply inserting 
						// (merge-end-pos += number of inserted rows)
						for (int r = cell.MergeStartPos.Row; r < row; r++)
						{
							cells[r, c].MergeEndPos = cells[r, c].MergeEndPos.Offset(count, 0);
						}

						// if range is splitted by inserted rows
						// the height of range should be expanded
						//
						// NOTE: only do this once when c is merge-start-column
						//
						if (c == cell.MergeStartPos.Col)
						{
							ReoGridCell startCell = GetCell(cell.MergeStartPos);
							startCell.Rowspan += (short)count;
							startCell.Height += totalHeight;
						}
					}
					else
					{
						cells[row, c] = null;
					}

					#endregion
				}
			}

			#endregion

			UpdateViewportController();

			// raise event
			if (RowInserted != null)
			{
				RowInserted(this, new RGRowEventArgs(row));
			}

#if DEBUG
			watch.Stop();
			Debug.WriteLine("insert rows: " + watch.ElapsedMilliseconds + " ms.");
#endif
		}

		/// <summary>
		/// Delete columns from specified number of column
		/// </summary>
		/// <param name="col">number of column to begin delete</param>
		/// <param name="count">number of columns to be deleted</param>
		public void DeleteCols(int col, int count)
		{
			if (col < 0 || col >= this.cols.Count)
			{
				throw new ArgumentOutOfRangeException("count");
			}

			if (count >= this.cols.Count)
			{
				throw new ArgumentOutOfRangeException("count");
			}

			if (col + count > this.cols.Count)
			{
				// at least remain 1 cols
				throw new ArgumentOutOfRangeException("col + count, at least 1 column should be keep");
			}

#if DEBUG
			Stopwatch sw = Stopwatch.StartNew();
#endif
			
			int endcol = col + count;
			int totalWidth = this.cols[endcol - 1].Right - this.cols[col].Left;

			var colOutlines = GetOutlines(RowOrColumn.Column);
			if (colOutlines != null)
			{
				OutlineGroup<IReoGridOutline> tobeRemoved = new OutlineGroup<IReoGridOutline>();

				colOutlines.IterateOutlines(o =>
				{
					if (o.Start > endcol)
					{
						o.Start -= count;
					}
					else if (o.End > col)
					{
						o.Count -= Math.Min(endcol - o.Start, count);

						// count <= 0 will be removed
						if (o.Count <= 0)
						{
							tobeRemoved.Add(o);
						}
					}

					return true;
				});

				// remove outlines which count <= 0
				foreach (var o in tobeRemoved)
				{
					RemoveOutline(RowOrColumn.Column, o);
				}

				tobeRemoved.Clear();

				colOutlines.IterateReverseOutlines(o =>
				{
					if (colOutlines.HasSame(o, tobeRemoved))
					{
						tobeRemoved.Add(o);
					}
					return true;
				});

				// remove outlines which count <= 0
				foreach (var o in tobeRemoved)
				{
					RemoveOutline(RowOrColumn.Column, o);
				}
			}

			this.cols.RemoveRange(col, count);

			for (int c = col; c < this.cols.Count; c++)
			{
				cols[c].Col -= count;
				cols[c].Code = RGUtility.GetAlphaChar(c);
				cols[c].Left -= totalWidth;

				Debug.Assert(cols[c].Col >= 0);
			}

			// left 
			for (int r = 0; r <= this.rows.Count; r++)
			{
				// TODO: bounds test
				ReoGridCell cell = cells[r, col];

				if (r < this.rows.Count && cell != null)
				{
					if (!cell.MergeStartPos.IsEmpty && cell.MergeStartPos.Col < col)
					{
						// update colspan for range
						if (cell.MergeStartPos.Row == r)
						{
							ReoGridCell mergedStartCell = cells[cell.MergeStartPos.Row, cell.MergeStartPos.Col];
							Debug.Assert(mergedStartCell.Colspan > 0);

							int span = Math.Min(count, cell.MergeEndPos.Col - col + 1);
							mergedStartCell.Colspan -= (short)span;

							mergedStartCell.Width =
								this.cols[mergedStartCell.Col + mergedStartCell.Colspan].Left -
								this.cols[mergedStartCell.Col].Left;

							Debug.Assert(mergedStartCell.Colspan > 0);
						}

						// update merge-end-col for range
						for (int c = cell.MergeStartPos.Col; c < col; c++)
						{
							ReoGridCell leftCell = cells[r, c];
							int span = Math.Min(count, leftCell.MergeEndPos.Col - col + 1);
							leftCell.MergeEndPos = leftCell.MergeEndPos.Offset(0, -span);
						}
					}
				}

				// if any borders exist in left side, it's need to update the span of borders.
				// from columns from 0 col no need to do this
				if (col > 0
					// is border at start column 
					&& this.hBorders[r, col - 1] != null && this.hBorders[r, col - 1].Cols > 0)
				{
					// find border to merge in right side
					int addspan = 0;

					// border exists in right side?
					if (this.hBorders[r, endcol] != null
						// this is not a same border range
						&& this.hBorders[r, endcol].Cols + count + 1 != this.hBorders[r, col - 1].Cols
						// does they have same style?
						&& this.hBorders[r, endcol].Border.Equals(this.hBorders[r, col - 1].Border)
						// does they have same owner position flag?
						&& this.hBorders[r, endcol].Pos == this.hBorders[r, col - 1].Pos)
					{
						addspan = this.hBorders[r, endcol].Cols;
					}

					// update borders in left side
					int subspan = 0;

					// calc how many borders in delete target range,
					// it need be subtract from left side border.
					if (this.hBorders[r, col] != null && this.hBorders[r, col].Cols > 0
						&& this.hBorders[r, col].Cols == this.hBorders[r, col - 1].Cols - 1)
					{
						subspan = Math.Min(this.hBorders[r, col].Cols, count);
					}

					// set reference span
					int refspan = this.hBorders[r, col - 1].Cols;

					this.hBorders[r, col - 1].Cols += addspan - subspan;

					if (col > 1)
					{
						// update all span in left side
						for (int c = col - 2; c >= 0; c--)
						{
							if (this.hBorders[r, c] != null && this.hBorders[r, c].Cols == refspan + 1)
							{
								this.hBorders[r, c].Cols += addspan - subspan;
								refspan++;
							}
							else
								break;
						}
					}
				}
			}

			int rightBounds = Math.Min(this.cols.Count + count, cols.Capacity);

			// right
			for (int r = 0; r < this.rows.Count; r++)
			{
				for (int c = endcol; c < rightBounds; c++)
				{
					ReoGridCell cell = cells[r, c];

					if (cell != null)
					{
						if (cell.MergeStartPos.Col >= endcol )
						{
							cell.MergeStartPos = cell.MergeStartPos.Offset(0, -count);
							cell.Left -= totalWidth;
							cell.TextBoundsLeft -= totalWidth;
						}
						else if (cell.Col >= endcol && cell.IsValidCell)
						{
							cell.Left -= totalWidth;
							cell.TextBoundsLeft -= totalWidth;
						}

						// Case:
						//
						//       col          ec
						//     +-----------+
						//     |           |
						//   0 |  1  |  2  |  3  |
						//     |     |     |     |
						//     |     +-----|-----|
						//     |     |     |     |
						//     |     +-----|-----|
						//     |     |     |     |
						//
						else if (cell.MergeStartPos.Col >= col && cell.MergeStartPos.Col < endcol)
						{
							if (c == endcol && r == cells[r, c].MergeStartPos.Row)
							{
								ReoGridCell startCell = cells[cell.MergeStartPos.Row, cell.MergeStartPos.Col];
								Debug.Assert(startCell != null);

								// create a new merged cell
								cell.Rowspan = (short)(cell.MergeEndPos.Row - cell.MergeStartPos.Row + 1);
								cell.Colspan = (short)(startCell.Colspan - endcol + cell.MergeStartPos.Col);

								cell.Bounds = GetRangeBounds(r, cell.MergeStartPos.Col, cell.Rowspan, cell.Colspan);

								// copy cell content
								ReoGridCellUtility.CopyCellContent(cell, startCell);
							}

							cell.MergeStartPos = new ReoGridPos(cell.MergeStartPos.Row, col);
						}

						// update merge-end-pos
						int espan = Math.Min(count, cell.MergeEndPos.Col - cell.MergeStartPos.Col);
						cell.MergeEndPos = cell.MergeEndPos.Offset(0, -espan);
					}
				}
			}

			// move cols
			//// todo: use max-content-column
			for (int r = 0; r <= this.rows.Count; r++)
			{
				#region move cells
				// move cells
				for (int c = col; c <= this.cols.Count; c++)
				{
					if (r < this.rows.Count && c < this.cols.Count)
					{
						var cell = cells[r, c + count];
						cells[r, c] = cell;

						if (cell != null)
						{
							cell.Col -= count;
						}
					}

					hBorders[r, c] = hBorders[r, c + count];
					vBorders[r, c] = vBorders[r, c + count];
				}
				#endregion // move cells

				// remove border to force show grid line
				//
				if (col == 0 || !IsInsideSameMergedCell(r, col - 1, r, col))
				{
					if (vBorders[r, col] != null && vBorders[r, col].Rows == 0)
						vBorders[r, col] = null;
				}
			}

			selectionRange = FixRange(selectionRange);

			UpdateViewportController();

#if DEBUG
			sw.Stop();
			long ms = sw.ElapsedMilliseconds;
			if (ms > 20)
			{
				Debug.WriteLine("deleting columns takes " + ms + " ms.");
			}
#endif

			// raise column deleted event
			if (ColDeleted != null) ColDeleted(this, new RGColumnDeletedEventArgs(col, count));
		}

		public void DeleteRows(int row, int count)
		{
			if (row < 0 || row >= this.rows.Count)
			{
				throw new ArgumentOutOfRangeException("count");
			}

			if (count >= this.rows.Count)
			{
				throw new ArgumentOutOfRangeException("count");
			}

			if (row + count > this.rows.Count)
			{
				// at least remain 1 rows
				throw new ArgumentOutOfRangeException("row + count, at least one row left");
			}

#if DEBUG
			Stopwatch sw = Stopwatch.StartNew();
#endif
			
			int endrow = row + count;
			int totalHeight = this.rows[endrow - 1].Bottom - this.rows[row].Top;

#if DEBUG
			Debug.Assert(totalHeight > 0);
#endif

			#region delete outlines

			var rowOutlines = GetOutlines(RowOrColumn.Row);
			if (rowOutlines != null)
			{
				OutlineGroup<IReoGridOutline> tobeRemoved = new OutlineGroup<IReoGridOutline>();

				rowOutlines.IterateOutlines(o =>
				{
					if (o.Start > endrow)
					{
						o.Start -= count;
					}
					else if (o.End > row)
					{
						o.Count -= Math.Min(endrow - o.Start, count);
					
						// count <= 0 will be removed
						if (o.Count <= 0)
						{
							tobeRemoved.Add(o);
						}
					}
					
					return true;
				});

				// remove outlines which count <= 0
				foreach(var o in tobeRemoved)
				{
					RemoveOutline(RowOrColumn.Row, o);
				}

				tobeRemoved.Clear();

				rowOutlines.IterateReverseOutlines(o =>
				{
					if (rowOutlines.HasSame(o, tobeRemoved))
					{
						tobeRemoved.Add(o);
					}
					return true;
				});

				// remove outlines which count <= 0
				foreach (var o in tobeRemoved)
				{
					RemoveOutline(RowOrColumn.Row, o);
				}
			}

			#endregion // delete outlines

			#region delete headers

			this.rows.RemoveRange(row, count);

			for (int r = row; r < this.rows.Count; r++)
			{
				rows[r].Row -= count;
				rows[r].Top -= totalHeight;

#if DEBUG
				Debug.Assert(rows[r].Row >= 0);
#endif
			}

			#endregion // delete headers

			#region delete top side
			for (int c = 0; c <= this.cols.Count; c++)
			{
				// TODO: bounds test
				ReoGridCell cell = cells[row, c];

				if (c < this.cols.Count && cell != null)
				{
					if (!cell.MergeStartPos.IsEmpty && cell.MergeStartPos.Row < row)
					{
						// update colspan for range
						if (cell.MergeStartPos.Col == c)
						{
							ReoGridCell mergedStartCell = cells[cell.MergeStartPos.Row, cell.MergeStartPos.Col];
							
							Debug.Assert(mergedStartCell != null);
							Debug.Assert(mergedStartCell.Colspan > 0);

							int span = Math.Min(count, cell.MergeEndPos.Row - row + 1);
							mergedStartCell.Rowspan -= (short)span;

							Debug.Assert(mergedStartCell.Rowspan > 0);

							mergedStartCell.Height =
								this.rows[mergedStartCell.Row + mergedStartCell.Rowspan].Top -
								this.rows[mergedStartCell.Row].Top;
						}

						// update merge-end-col for range
						for (int r = cell.MergeStartPos.Row; r < row; r++)
						{
							ReoGridCell topCell = cells[r, c];
							int span = Math.Min(count, topCell.MergeEndPos.Row - row + 1);
							topCell.MergeEndPos = topCell.MergeEndPos.Offset(-span, 0);
						}
					}
				}

				// if any borders exist in left side, it's need to update the span of borders.
				// from columns from 0 col no need to do this
				if (row > 0
					// is border at start column 
					&& this.vBorders[row - 1, c] != null && this.vBorders[row - 1, c].Rows > 0)
				{
					// find border to merge in right side
					int addspan = 0;

					// border exists in right side?
					if (this.vBorders[endrow, c] != null
						// this is not a same border range
						&& this.vBorders[endrow, c].Rows + count + 1 != this.vBorders[row - 1, c].Rows
						// does they have same styles?
						&& this.vBorders[endrow, c].Border.Equals(this.vBorders[row - 1, c].Border)
						// does they have same owner position flags?
						&& this.vBorders[endrow, c].Pos == this.vBorders[row - 1, c].Pos)
					{
						addspan = this.vBorders[endrow, c].Rows;
					}

					// update borders in left side
					int subspan = 0;

					// calc how many borders in delete target range,
					// it need be subtract from left side border.
					if (this.vBorders[row, c] != null && this.vBorders[row, c].Rows > 0
						&& this.vBorders[row, c].Rows == this.vBorders[row - 1, c].Rows - 1)
					{
						subspan = Math.Min(this.vBorders[row, c].Rows, count);
					}

					// set reference span
					int refspan = this.vBorders[row - 1, c].Rows;

					this.vBorders[row - 1, c].Rows += addspan - subspan;

					if (row > 1)
					{
						// update all span in left side
						for (int r = row - 2; r >= 0; r--)
						{
							if (this.vBorders[r, c] != null && this.vBorders[r, c].Rows == refspan + 1)
							{
								this.vBorders[r, c].Rows += addspan - subspan;
								refspan++;
							}
							else
								break;
						}
					}
				}
			}
			#endregion // delete top side

			#region delete bottom side
			int bottomBounds = Math.Min(this.rows.Count + count, rows.Capacity);

			// bottom
			for (int c = 0; c < this.cols.Count; c++)
			{
				for (int r = endrow; r < bottomBounds; r++)
				{
					ReoGridCell cell = cells[r, c];

					if (cell != null)
					{
						if (cell.MergeStartPos.Row >= endrow)
						{
							cell.MergeStartPos = cell.MergeStartPos.Offset(-count, 0);
							cell.Top -= totalHeight;
							cell.TextBoundsTop -= totalHeight;
						}
						else if (cell.Row >= endrow && cell.IsValidCell)
						{
							cell.Top -= totalHeight;
							cell.TextBoundsTop -= totalHeight;
						}

						// Case:
						//
						//       col          ec
						//     +-----------+
						//     |           |
						//   0 |  1  |  2  |  3  |
						//     |     |     |     |
						//     |     +-----|-----|
						//     |     |     |     |
						//     |     +-----|-----|
						//     |     |     |     |
						//
						else if (cell.MergeStartPos.Row >= row && cell.MergeStartPos.Row < endrow)
						{
							if (r == endrow && c == cell.MergeStartPos.Col)
							{
								ReoGridCell startCell = cells[cell.MergeStartPos.Row, cell.MergeStartPos.Col];
								Debug.Assert(startCell != null);

								// create a new merged cell
								cell.Rowspan = (short)(startCell.Rowspan - endrow + cell.MergeStartPos.Row);
								cell.Colspan = (short)(cell.MergeEndPos.Col - cell.MergeStartPos.Col + 1);

								cell.Bounds = GetRangeBounds(cell.MergeStartPos.Row, c, cell.Rowspan, cell.Colspan);

								// copy cell content
								ReoGridCellUtility.CopyCellContent(cell, startCell);
							}

							//int sspan = endcol - cell.MergeStartPos.Col - count;
							cell.MergeStartPos = new ReoGridPos(row, cell.MergeStartPos.Col);
						}

						// update merge-end-pos
						int espan = Math.Min(count, cell.MergeEndPos.Row - cell.MergeStartPos.Row);
						cell.MergeEndPos = cell.MergeEndPos.Offset(-espan, 0);
					}
				}
			}
			#endregion // delete bottom side

			#region move columns
			//// todo: use max-content-column
			for (int c = 0; c <= this.cols.Count; c++)
			{
				#region move cells
				// move cells
				for (int r = row; r <= this.rows.Count; r++)
				{
					var cell = cells[r + count, c];
					cells[r, c] = cell;

					if (cell != null)
					{
						cell.Row -= count;
					}

					hBorders[r, c] = hBorders[r + count, c];
					vBorders[r, c] = vBorders[r + count, c];
				}
				#endregion // move cells

				// remove border to force show grid line
				if (row == 0 || !IsInsideSameMergedCell(row - 1, c, row, c))
				{
					if (hBorders[row, c] != null && hBorders[row, c].Cols == 0)
						hBorders[row, c] = null;
				}
			}
			#endregion // move columns

			selectionRange = FixRange(selectionRange);

			UpdateViewportController();

#if DEBUG
			sw.Stop();
			long ms = sw.ElapsedMilliseconds;
			if (ms > 20)
			{
				Debug.WriteLine("delete rows takes " + ms + " ms.");
			}
#endif

			// raise column deleted event
			if (RowDeleted != null) RowDeleted(this, new RGRowDeletedEventArgs(row, count));
		}

		/// <summary>
		/// Hide specified rows 
		/// </summary>
		/// <param name="row">index of start row to hide</param>
		/// <param name="count">number of rows to be hidden</param>
		public void HideRows(int row, int count)
		{
			SetRowsHeight(row, count, 0);
		}

		/// <summary>
		/// Show specified rows
		/// </summary>
		/// <param name="row">number of row</param>
		/// <param name="count">number of rows</param>
		public void ShowRows(int row, int count)
		{
			SetRowsHeight(row, count, r =>
			{
				var rowhead = this.rows[r];

				// just show row which is hidden
				return rowhead.IsHidden ? rowhead.LastHeight : rowhead.Height;
			}, true);
		}

		/// <summary>
		/// Hide specified columns 
		/// </summary>
		/// <param name="col">index of start column to hide</param>
		/// <param name="count">number of columns to be hidden</param>
		public void HideColumns(int col, int count)
		{
			SetColsWidth(col, count, 0);
		}

		/// <summary>
		/// Show specified columns
		/// </summary>
		/// <param name="col">number of column</param>
		/// <param name="count">number of columns</param>
		public void ShowColumns(int col, int count)
		{
			SetColsWidth(col, count, c =>
			{
				var colhead = this.cols[c];
			
				// just show column which is hidden
				return colhead.IsHidden ? colhead.LastWidth : colhead.Width;
			}, true);
		}

		/// <summary>
		/// Get number of columns
		/// </summary>
		public int ColCount { get { return this.cols.Count; } set { SetCols(value); } }

		/// <summary>
		/// Get number of rows
		/// </summary>
		public int RowCount { get { return this.rows.Count; } set { SetRows(value); } }

		internal ReoGridRowHead RetrieveRowHead(int r)
		{
			return this.rows[r];
		}
		internal ReoGridColHead RetrieveColHead(int c)
		{
			return this.cols[c];
		}

		/// <summary>
		/// Disable auto-row-height adjust on specified row
		/// </summary>
		/// <param name="row">row to be disable auto-row-height</param>
		/// <returns></returns>
		public bool DisableAutoRowHeight(int row)
		{
			if (row >= 0 && row < this.rows.Count)
			{
				this.rows[row].IsAutoHeight = false;
				return true;
			}
			else
				return false;
		}

		/// <summary>
		/// Disable auto-row-height adjust on specified row
		/// </summary>
		/// <param name="row">row to be disable auto-row-height</param>
		/// <returns></returns>
		public bool EnableAutoRowHeight(int row)
		{
			if (row >= 0 && row < this.rows.Count)
			{
				this.rows[row].IsAutoHeight = true;
				return true;
			}
			else
				return false;
		}

		/// <summary>
		/// Disable auto-row-height adjust on specified row
		/// </summary>
		/// <param name="row">row to be disable auto-row-height</param>
		/// <returns></returns>
		public bool IsAutoRowHeight(int row)
		{
			return (row >= 0 && row < this.rows.Count) ? this.rows[row].IsAutoHeight : false;
		}

		/// <summary>
		/// Event raised on row inserted at given index of row
		/// </summary>
		public event EventHandler<RGRowEventArgs> RowInserted;

		/// <summary>
		/// Event raised on row deleted at given index of row
		/// </summary>
		public event EventHandler<RGRowDeletedEventArgs> RowDeleted;

		/// <summary>
		/// Event raised on column inserted at given index of column
		/// </summary>
		public event EventHandler<RGColumnEventArgs> ColInserted;

		/// <summary>
		/// Event raised on column deleted at given index of column
		/// </summary>
		public event EventHandler<RGColumnDeletedEventArgs> ColDeleted;

		#endregion

		#region View & Draw
		private Font identifierFont = new Font(SystemFonts.DefaultFont.FontFamily, 8f);
		private Rectangle canvasBounds = new Rectangle();

		//private Rectangle xRuleBounds = new Rectangle();
		//private Rectangle yRuleBounds = new Rectangle();

		//private bool isShowRules = false;

		//private static readonly Font ruleSFont = new Font("Terminal", 6f);
		//private static readonly Font ruleLFont = new Font("Terminal", 10f);

		private static readonly StringFormat headerSf = new StringFormat
		{
			LineAlignment = StringAlignment.Center,
			Alignment = StringAlignment.Center,
			Trimming = StringTrimming.None,
			FormatFlags = StringFormatFlags.LineLimit,
		};

		private void UpdateCanvasBounds()
		{
			int width = ClientRectangle.Width - (rightPanel.Visible ? rightPanel.Width : 0) - 1;
			int height = ClientRectangle.Height - (bottomPanel.Visible ? bottomPanel.Height : 0) - 1;

			if (width < 0) width = 0;
			if (height < 0) height = 0;

			canvasBounds = new Rectangle(0, 0, width, height);

			if (viewportController != null) viewportController.Bounds = canvasBounds;
		}

		#region Viewport & Viewport Controller

		#region GridRegion
		internal struct GridRegion
		{
			internal int startRow;
			internal int endRow;
			internal int startCol;
			internal int endCol;
			internal static readonly GridRegion Empty = new GridRegion()
			{
				startRow = 0,
				startCol = 0,
				endRow = 0,
				endCol = 0
			};
			public GridRegion(int startRow, int startCol, int endRow, int endCol)
			{
				this.startRow = startRow;
				this.startCol = startCol;
				this.endRow = endRow;
				this.endCol = endCol;
			}
			public bool Contains(ReoGridPos pos)
			{
				return Contains(pos.Row, pos.Col);
			}
			public bool Contains(int r, int c)
			{
				return startRow <= r && endRow >= r && startCol <= c && endCol >= c;
			}
			public override bool Equals(object obj)
			{
				if ((obj as GridRegion?) == null) return false;

				GridRegion gr2 = (GridRegion)obj;
				return startRow == gr2.startRow && startCol == gr2.startCol
					&& endRow == gr2.endRow && endCol == gr2.endCol;
			}
			public override int GetHashCode()
			{
				return startRow ^ startCol ^ endRow ^ endCol;
			}
			public static bool operator ==(GridRegion gr1, GridRegion gr2)
			{
				return gr1.Equals(gr2);
			}
			public static bool operator !=(GridRegion gr1, GridRegion gr2)
			{
				return !gr1.Equals(gr2);
			}
			public int Rows { get { return endRow - startRow + 1; } set { endRow = startRow + value - 1; } }
			public int Cols { get { return endCol - startCol + 1; } set { endCol = startCol + value - 1; } }

			public override string ToString()
			{
				return string.Format("VisibleRegion[{0},{1}-{2},{3}]", startRow, startCol, endRow, endCol);
			}
		}
		#endregion // GridRegion

		#region Parts

		#region Base classes
		internal interface IPart
		{
			RectangleF Bounds { get; set; }
			float Left { get; set; }
			float Top { get; set; }

			/// <summary>
			/// Reserved
			/// </summary>
			float ScaleFactor { get; set; }

			void Draw(RGDrawingContext dc);
			bool Visible { get; set; }

			[Obsolete("Instead of OnMouseDown, OnMouseUp, OnMouseMove")]
			bool Hittest(PointF p);

			PointF PointToView(PointF p);
			PointF PointToControl(PointF p);

			bool OnMouseDown(Point location, MouseButtons buttons, int clicks);
			bool OnMouseUp(Point location, MouseButtons buttons, int clicks);

			void UpdateView();
		}

		internal interface IViewPart : IPart
		{
			PointF ViewStart { get; set; }
			float ViewTop { get; set; }
			float ViewLeft { get; set; }

			ScrollDirection ScrollableDirections { get; }
			void Scroll(float offX, float offY);

			GridRegion VisibleRegion { get; set; }

			ReoGridPos GetPosByPoint(PointF p);
		}

		internal abstract class Part : IPart
		{
			protected ReoGridControl grid;

			public Part(ReoGridControl grid) { this.grid = grid; }

			#region Bounds
			protected RectangleF bounds;
			public RectangleF Bounds { get { return bounds; } set { bounds = value; } }
			public float Top { get { return bounds.Top; } set { bounds.Y = value; } }
			public float Left { get { return bounds.Left; } set { bounds.X = value; } }
			internal float Right { get { return bounds.Right; } }
			internal float Bottom { get { return bounds.Bottom; } }
			internal float Width { get { return bounds.Width; } set { bounds.Width = value; } }
			internal float Height { get { return bounds.Height; } set { bounds.Height = value; } }
			#endregion

			protected float scaleFactor = 1f;

			public virtual float ScaleFactor
			{
				get { return scaleFactor; }
				set { scaleFactor = value; }
			}

			private bool visible = true;

			public bool Visible
			{
				get { return visible; }
				set { visible = value; }
			}

			public abstract void Draw(RGDrawingContext dc);

			public bool Hittest(PointF p) { return bounds.Contains(p); }

			public virtual PointF PointToView(PointF p)
			{
				return new PointF(
					(p.X - bounds.Left) / grid.scaleFactor,
					(p.Y - bounds.Top) / grid.scaleFactor);
			}

			public virtual PointF PointToControl(PointF p)
			{
				return new PointF(
					(p.X + bounds.Left) * grid.scaleFactor,
					(p.X + bounds.Top) * grid.scaleFactor);
			}

			public virtual bool OnMouseDown(Point location, MouseButtons buttons, int clicks) { return false; }
			public virtual bool OnMouseUp(Point location, MouseButtons buttons, int clicks) { return false; }

			public virtual void UpdateView() { }
		}

		internal abstract class Viewport : Part, IViewPart
		{
			public Viewport(ReoGridControl grid) : base(grid) { }

			#region Visible Region
			protected GridRegion visibleRegion;

			public GridRegion VisibleRegion
			{
				get { return visibleRegion; }
				set { visibleRegion = value; }
			}
			#endregion

			#region View
			protected PointF viewStart;
			public PointF ViewStart { get { return viewStart; } set { viewStart = value; } }
			public float ViewTop { get { return viewStart.Y; } set { viewStart.Y = value; } }
			public float ViewLeft { get { return viewStart.X; } set { viewStart.X = value; } }

			private ScrollDirection scrollableDirections = ScrollDirection.None;
			public virtual ScrollDirection ScrollableDirections { get { return scrollableDirections; } set { scrollableDirections = value; } }

			public void Scroll(float offX, float offY)
			{
				//if (bounds.Width == 0 || bounds.Height == 0
				//	|| visibleRegion.Rows == 0 || visibleRegion.Cols == 0
				//	|| (offX == 0 && offY == 0)) return Rectangle.Empty;

				ViewTop += offY;
				ViewLeft += offX;

				if (ViewTop < 0) ViewTop = 0;
				if (ViewLeft < 0) ViewLeft = 0;

				// TODO: need optimum 
				//
				// now we update visible region by whole calcutating,
				// I think we can just calcutate the offset(x, y) to move all viewports faster.
				//UpdateVisibleRegion();

#if WIN32_SCROLL
				IntPtr hdc = Win32.GetDC(grid.Handle);

				Rectangle rect = new Rectangle((int)Math.Round(Left), (int)Math.Round(Top), (int)Math.Round(Width + Left), (int)Math.Round(Top + Height));
				Win32.ScrollDC(hdc, -offX, -offY, ref rect, ref rect, IntPtr.Zero, ref rect);

				Win32.ReleaseDC(hdc);
#endif

//				Rectangle redrawRect = Rectangle.Empty;

//#if WIN32_SCROLL
//				if (offY > 0)
//				{
//					redrawRect = new Rectangle((int)Left, (int)(Top + Height - offY - 1), (int)Width, offY + 2);
//				}
//				else if (offY < 0)
//				{
//					redrawRect = new Rectangle((int)Left, (int)(Top), (int)Width, -offY + 1);
//				}
//				else if (offX > 0)
//				{
//					redrawRect = new Rectangle((int)(Left + Width - offX - 1), (int)Top, offX + 1, (int)Height);
//				}
//				else if (offX < 0)
//				{
//					redrawRect = new Rectangle((int)Left, (int)Top, -offX + 1, (int)Height);
//				}
//#endif

//				return redrawRect;
			}
			#endregion

			public override PointF PointToView(PointF p)
			{
				return new PointF(
					(p.X - bounds.Left + viewStart.X * grid.scaleFactor) / grid.scaleFactor,
					(p.Y - bounds.Top + viewStart.Y * grid.scaleFactor) / grid.scaleFactor);
			}
			public override PointF PointToControl(PointF p)
			{
				return new PointF(
					(p.X + bounds.Left - viewStart.X / grid.scaleFactor) * grid.scaleFactor,
					(p.Y + bounds.Top - viewStart.Y / grid.scaleFactor) * grid.scaleFactor);
			}

			internal int GetColByPoint(float x)
			{
				if (x < grid.cols[0].Right) return 0;

				//for (int i = 0; i < this.grid.cols.Count; i++)
				//{
				//	if (grid.cols[i].Right > x) return i;
				//}

				//return grid.cols.Count - 1;

				return ArrayHelper.QuickFind((visibleRegion.endCol - visibleRegion.startCol) / 2, 0, grid.cols.Count, i =>
				{
					var col = grid.cols[i];

					if (col.Left <= x && col.Right >= x)
						return 0;
					else if (col.Left < x)
						return 1;
					else if (col.Right > x)
						return -1;
					else
						return 0;
				});

			}
			internal int GetRowByPoint(float y)
			{
				if (y < grid.rows[0].Bottom) return 0;

#if DEBUG
				Stopwatch sw = Stopwatch.StartNew();
				try
				{
#endif

					return ArrayHelper.QuickFind((visibleRegion.endRow - visibleRegion.startRow) / 2, 0, grid.rows.Count, i =>
					{
						var row = grid.rows[i];

						if (row.Top <= y && row.Bottom >= y)
							return 0;
						else if (row.Top < y)
							return 1;
						else if (row.Bottom > y)
							return -1;
						else
							return 0;
					});
				
					//var row = grid.rows.Find(_row => _row.Top <= y && _row.Bottom >= y);
					//return row == null ? grid.rows.Last().Row : row.Row;

					//for (int i = 0; i < grid.rows.Count; i++)
					//{
					//	if (grid.rows[i].Bottom > y) return i;
					//}

					//return grid.rows.Count - 1;
#if DEBUG
				}
				finally
				{
					sw.Stop();
					long ms = sw.ElapsedMilliseconds;
					if (ms > 1)
					{
						Debug.WriteLine("find row index taks " + ms + " ms.");
					}
				}
#endif
			}

			public ReoGridPos GetPosByPoint(PointF p)
			{
				return new ReoGridPos(GetRowByPoint(p.Y), GetColByPoint(p.X));
			}

			public override void Draw(RGDrawingContext dc)
			{
				var clipRect = this.bounds;
				clipRect.Width++;
				clipRect.Height++;

				var g = dc.Graphics;
				g.SetClip(clipRect);

				float offsetX = bounds.Left -
					((this.scrollableDirections & ScrollDirection.Horizontal) == ScrollDirection.Horizontal ?
						viewStart.X * grid.scaleFactor : 0);

				float offsetY = bounds.Top -
					((this.scrollableDirections & ScrollDirection.Vertical) == ScrollDirection.Vertical ?
						viewStart.Y * grid.scaleFactor : 0);

				g.TranslateTransform(offsetX, offsetY);

				DrawView(dc);

				g.ResetClip();
				g.ResetTransform();
			}
			public abstract void DrawView(RGDrawingContext dc);
		}
		#endregion

		#region LeadHeadPart
		private class LeadHeadPart : Part
		{
			public LeadHeadPart(ReoGridControl grid) : base(grid) { }
			public override void Draw(RGDrawingContext dc)
			{
				if (bounds.IsEmpty) return;

				Graphics g = dc.Graphics;

				SolidBrush b = ResourcePoolManager.Instance.GetSolidBrush(grid.controlStyle.Colors[ReoGridControlColors.LeadHeadNormal]);
				RectangleF backRect = new RectangleF(bounds.X + 1, bounds.Y + 1,
					bounds.Width - 1, bounds.Height - 1);
				g.FillRectangle(b, bounds);

				using (GraphicsPath leadHeadPath = new GraphicsPath())
				{
					leadHeadPath.AddLines(new PointF[]{ new PointF(bounds.Right - 4, bounds.Y + 4),
						new PointF(bounds.Right - 4, bounds.Bottom - 4),
						new PointF(bounds.Right - bounds.Height + 4, bounds.Bottom - 4)});
					leadHeadPath.CloseAllFigures();

					using (LinearGradientBrush lgb = new LinearGradientBrush(
						bounds, grid.isLeadHeadSelected ?
						grid.controlStyle.Colors[ReoGridControlColors.LeadHeadIndicatorStart]
						: grid.controlStyle.Colors[ReoGridControlColors.LeadHeadSelected],
						grid.controlStyle.Colors[ReoGridControlColors.LeadHeadIndicatorEnd], 90f))
					{
						g.FillPath(lgb, leadHeadPath);
					}
				}
			}
		}
		#endregion

		#region ColumnHeadPart
		private class ColumnHeaderPart : Viewport
		{
			public ColumnHeaderPart(ReoGridControl grid) : base(grid) { }

			public override void Draw(RGDrawingContext dc)
			{
				base.Draw(dc);

				var g = dc.Graphics;
				
				// bottom line
				if (!grid.HasSettings(ReoGridSettings.View_ShowGridLine))
				{
					g.DrawLine(ResourcePoolManager.Instance.GetPen(grid.controlStyle.Colors[ReoGridControlColors.ColHeadSplitter]),
						bounds.X, bounds.Bottom,
						Math.Min((grid.cols[grid.cols.Count - 1].Right - ViewLeft) * grid.scaleFactor + bounds.Left, bounds.Right), 
						bounds.Bottom);
				}
			}

			public override void DrawView(RGDrawingContext dc)
			{
				Graphics g = dc.Graphics;

				if (bounds.Height > 0)
				{
					Font scaledFont = ResourcePoolManager.Instance.GetFont(grid.identifierFont.Name,
						grid.identifierFont.Size * grid.scaleFactor, grid.identifierFont.Style);

					var splitterLinePen = ResourcePoolManager.Instance.GetPen(grid.controlStyle.Colors[ReoGridControlColors.RowHeadSplitter]);

					bool isFullColSelected = grid.SelectionRange.Rows == grid.RowCount;

					for (int i = visibleRegion.startCol; i <= visibleRegion.endCol; i++)
					{
						bool isSelected = i >= grid.SelectionRange.Col && i <= grid.SelectionRange.Col2;
						
						ReoGridColHead header = grid.cols[i];

						float x = header.Left * grid.scaleFactor;
						float width = header.Width * grid.scaleFactor;

						if (header.IsHidden)
						{
							g.DrawLine(splitterLinePen, x - 1, 0, x - 1, bounds.Bottom);
						}
						else
						{
							RectangleF rect = new RectangleF(x, 0, width, bounds.Height);

							using (LinearGradientBrush b = new LinearGradientBrush(rect,
								grid.controlStyle.GetColHeadStartColor(false, isSelected, isSelected && isFullColSelected, false),
								grid.controlStyle.GetColHeadEndColor(false, isSelected, isSelected && isFullColSelected, false), 90f))
							{
								g.FillRectangle(b, rect);
							}

							g.DrawLine(splitterLinePen, x, 0, x, bounds.Height);

							g.DrawString(header.Code, scaledFont,
								ResourcePoolManager.Instance.GetSolidBrush(grid.controlStyle.Colors[ReoGridControlColors.ColHeadText]),
								rect, headerSf);
						}
					}

					float lx = grid.cols[visibleRegion.endCol].Right * grid.scaleFactor;
					g.DrawLine(splitterLinePen, lx, 0, lx, bounds.Height);
				}
			}
		}
		#endregion

		#region RowHeadPart
		private class RowHeaderPart : Viewport
		{
			public RowHeaderPart(ReoGridControl grid) : base(grid) { }

			public override void Draw(RGDrawingContext dc)
			{
				base.Draw(dc);

				// right line
				if (!grid.HasSettings(ReoGridSettings.View_ShowGridLine))
				{
					dc.Graphics.DrawLine(ResourcePoolManager.Instance.GetPen(grid.controlStyle.Colors[ReoGridControlColors.RowHeadSplitter]),
						bounds.Right, bounds.Y, bounds.Right,
						Math.Min((grid.rows[grid.rows.Count - 1].Bottom - ViewTop) * grid.scaleFactor + bounds.Top, bounds.Bottom));
				}
			}

			public override void DrawView(RGDrawingContext dc)
			{
				Graphics g = dc.Graphics;

				var splitterLinePen = ResourcePoolManager.Instance.GetPen(grid.controlStyle.Colors[ReoGridControlColors.RowHeadSplitter]);
			
				if (bounds.Width > 0)
				{
					Font scaledFont = ResourcePoolManager.Instance.GetFont(grid.identifierFont.Name,
						grid.identifierFont.Size * grid.scaleFactor, grid.identifierFont.Style);

					bool isFullRowSelected = grid.SelectionRange.Cols == grid.ColCount;

					for (int i = visibleRegion.startRow; i <= visibleRegion.endRow; i++)
					{
						bool isSelected = i >= grid.SelectionRange.Row && i <= grid.SelectionRange.Row2;
				
						ReoGridRowHead row = grid.rows[i];
						float y = row.Top * grid.scaleFactor;

						if (row.IsHidden)
						{
							g.DrawLine(splitterLinePen, 0, y - 1, bounds.Width, y - 1);
						}
						else
						{
							RectangleF rect = new RectangleF(0, y, bounds.Width, row.Height * grid.scaleFactor);

							if (rect.Height > 0)
							{
								SolidBrush b = ResourcePoolManager.Instance.GetSolidBrush(grid.controlStyle.GetRowHeadEndColor(
									false, isSelected, isSelected && isFullRowSelected, false));

								g.FillRectangle(b, rect);

								g.DrawLine(splitterLinePen, 0, y, bounds.Width, y);

								g.DrawString((row.Row + 1).ToString(), scaledFont,
									ResourcePoolManager.Instance.GetSolidBrush(grid.controlStyle.Colors[ReoGridControlColors.RowHeadText]),
									rect, headerSf);
							}
						}
					}
				}

				if (visibleRegion.endRow >= 0)
				{
					float ly = grid.rows[visibleRegion.endRow].Bottom * grid.scaleFactor;
					g.DrawLine(splitterLinePen, 0, ly, bounds.Width, ly);
				}
			}
		}
		#endregion // RowHeadPart

		#region Outline Parts
		private class OutlineLeftTopSpace : Part
		{
			public OutlineLeftTopSpace(ReoGridControl grid) : base(grid) { }

			public override void Draw(RGDrawingContext dc)
			{
				dc.Graphics.FillRectangle(ResourcePoolManager.Instance.GetSolidBrush(
					grid.controlStyle[ReoGridControlColors.OutlinePanelBackground]), bounds);

				var borderPen = ResourcePoolManager.Instance.GetPen(
					grid.controlStyle[ReoGridControlColors.OutlinePanelBorder]);

				// right
				dc.Graphics.DrawLine(borderPen, bounds.Right, bounds.Y, bounds.Right, bounds.Bottom);
				// bottom
				dc.Graphics.DrawLine(borderPen, bounds.X, bounds.Bottom, bounds.Right, bounds.Bottom);
			}
		}

		private class OutlineHeadPart : Part
		{
			public RowOrColumn Flag { get; set; }

			public OutlineHeadPart(ReoGridControl grid, RowOrColumn flag)
				: base(grid)
			{
				this.Flag = flag;
			}
		
			public override void Draw(RGDrawingContext dc)
			{
				var g = dc.Graphics;
				var outlines = grid.outlines[Flag];

				var borderPen = ResourcePoolManager.Instance.GetPen(grid.controlStyle[ReoGridControlColors.OutlinePanelBorder]);
				var textBrush = ResourcePoolManager.Instance.GetSolidBrush(grid.controlStyle[ReoGridControlColors.OutlineButtonText]);

				g.FillRectangle(ResourcePoolManager.Instance.GetSolidBrush(grid.controlStyle[ReoGridControlColors.OutlinePanelBackground]), bounds);

				if (outlines != null)
				{
					for (int idx = 0; idx < outlines.Count; idx++)
					{
						OutlineGroup<ReoGridOutline> line = outlines[idx];

						RectangleF numberRect = line.NumberButtonBounds;
						if (pressedIndex == idx)
						{
							numberRect.Offset(1, 1);
						}

						g.DrawRectangle(borderPen, numberRect.X, numberRect.Y, numberRect.Width, numberRect.Height);

						g.DrawString((idx + 1).ToString(), grid.identifierFont, textBrush, numberRect, ReoGridControl.headerSf);
					}
				}

				// right
				g.DrawLine(borderPen, bounds.Right, bounds.Top, bounds.Right, bounds.Bottom);
				// bottom
				g.DrawLine(borderPen, bounds.X, bounds.Bottom, bounds.Right, bounds.Bottom);
			}

			public override PointF PointToView(PointF p)
			{
				return p;
			}

			private int pressedIndex = -1;

			public override bool OnMouseDown(Point location, MouseButtons buttons, int clicks)
			{
				if (bounds.Contains(location))
				{
					var outlines = grid.outlines[Flag];

					if (outlines != null)
					{
						for (int i = 0; i < outlines.Count; i++)
						{
							var group = outlines[i];

							if (group.NumberButtonBounds.Contains(location))
							{
								group.CollapseAll();
								pressedIndex = i;

								// TODO: need optimum: expand once? 
								i--;
								while (i >= 0)
								{
									outlines[i].ExpandAll();
									i--;
								}

								return true;
							}
						}
					}
				}

				return false;
			}

			public override bool OnMouseUp(Point location, MouseButtons buttons, int clicks)
			{
				if (pressedIndex >= 0)
				{
					pressedIndex = -1;
					return true;
				}
				else
				{
					return base.OnMouseUp(location, buttons, clicks);
				}
			}
		}

		private class RowOutlineHeadPart : OutlineHeadPart
		{
			public RowOutlineHeadPart(ReoGridControl grid) : base(grid, RowOrColumn.Row) { }

			public override void UpdateView()
			{
				var outlines = grid.outlines[RowOrColumn.Row];

				if (outlines != null)
				{
					float scale = Math.Min(grid.scaleFactor, 1f);

					int buttonSize = ((grid.scaleFactor > 1f) ? ReoGridControl.OutlineButtonSize
						: (int)Math.Round(ReoGridControl.OutlineButtonSize * scale));

					for (int idx = 0; idx < outlines.Count; idx++)
					{
						OutlineGroup<ReoGridOutline> line = outlines[idx];

						int x = (int)Math.Round(((3 + ReoGridControl.OutlineButtonSize) * idx) * scale);
						int y = (int)Math.Round(bounds.Top + (bounds.Height - buttonSize) / 2);
						line.NumberButtonBounds = new Rectangle(x + 1, y, buttonSize, buttonSize);
					}
				}
			}
		}

		private class ColumnOutlineHeadPart : OutlineHeadPart
		{
			public ColumnOutlineHeadPart(ReoGridControl grid) : base(grid, RowOrColumn.Column) { }

			public override void UpdateView()
			{
				var outlines = grid.outlines[RowOrColumn.Column];

				if (outlines != null)
				{
					float scale = Math.Min(grid.scaleFactor, 1f);

					int buttonSize = ((grid.scaleFactor > 1f) ? ReoGridControl.OutlineButtonSize
						: (int)Math.Round(ReoGridControl.OutlineButtonSize * scale));

					for (int idx = 0; idx < outlines.Count; idx++)
					{
						OutlineGroup<ReoGridOutline> line = outlines[idx];

						int y = (int)Math.Round(((3 + ReoGridControl.OutlineButtonSize) * idx) * scale);
						int x = (int)Math.Round(bounds.Left + (bounds.Width - buttonSize) / 2);
						line.NumberButtonBounds = new Rectangle(x, y + 1, buttonSize, buttonSize);
					}
				}
			}
		}

		private abstract class OutlinePart : Viewport
		{
			public RowOrColumn Flag { get; set; }

			public OutlinePart(ReoGridControl grid, RowOrColumn flag) : base(grid) { this.Flag = flag; }

			protected ReoGridOutline OutlineButtonHittest(OutlineCollection<ReoGridOutline> outlines, Point location)
			{
				foreach (var g in outlines)
				{
					foreach (var o in g)
					{
						if (o.ToggleButtonBounds.Contains(location))
						{
							return o;
						}
					}
				}

				return null;
			}

			public override PointF PointToView(PointF p)
			{
				return new PointF(p.X + (ViewLeft * grid.scaleFactor - bounds.X),
					p.Y + (ViewTop * grid.scaleFactor - bounds.Y));
			}

			public override bool OnMouseDown(Point location, MouseButtons buttons, int clicks)
			{
				if (grid.outlines != null)
				{
					var outlines = grid.outlines[this.Flag];

					if (outlines != null)
					{
						ReoGridOutline outline = OutlineButtonHittest(outlines, location);

						if (outline != null)
						{
							if (outline.Collapsed)
							{
								outline.Expand();
								return true;
							}
							else
							{
								outline.Collapse();
								return true;
							}
						}
					}
				}

				return base.OnMouseDown(location, buttons, clicks);
			}

			public override void UpdateView()
			{
				var outlines = grid.GetOutlines(this.Flag);

				if (outlines != null)
				{
					float scale = Math.Min(grid.scaleFactor, 1f);

					int buttonSize = (int)Math.Round((ReoGridControl.OutlineButtonSize) * grid.scaleFactor);
					if (buttonSize > ReoGridControl.OutlineButtonSize) buttonSize = ReoGridControl.OutlineButtonSize;

					for (int idx = 0; idx < outlines.Count; idx++)
					{
						int pos = (int)Math.Round(((3 + ReoGridControl.OutlineButtonSize) * idx) * scale);

						OutlineGroup<ReoGridOutline> line = outlines[idx];

						if (idx < outlines.Count - 1)
						{
							foreach (var outline in line)
							{
								outline.ToggleButtonBounds = CreateToggleButtonRect(pos, outline.End, buttonSize);
							}
						}
					}
				}
			}

			protected abstract Rectangle CreateToggleButtonRect(int loc, int pos, int buttonSize);
		}

		private class RowOutlinePart : OutlinePart
		{
			public RowOutlinePart(ReoGridControl grid)
				: base(grid, RowOrColumn.Row)
			{
			}

			#region Draw
			public override void Draw(RGDrawingContext dc)
			{
				var g = dc.Graphics;

				g.FillRectangle(ResourcePoolManager.Instance.GetSolidBrush(grid.controlStyle[ReoGridControlColors.OutlinePanelBackground]), 
					bounds.X, bounds.Y, bounds.Width, bounds.Height + 1);

				base.Draw(dc);

				g.DrawLine(ResourcePoolManager.Instance.GetPen(grid.controlStyle[ReoGridControlColors.OutlinePanelBorder]),
					bounds.Right, bounds.Top, bounds.Right, bounds.Bottom);
			}

			public override void DrawView(RGDrawingContext dc)
			{
#if DEBUG
				Stopwatch sw = Stopwatch.StartNew();
#endif
				var outlines = grid.outlines[RowOrColumn.Row];

				if (outlines != null)
				{
					var g = dc.Graphics;
					var p = ResourcePoolManager.Instance.GetPen(grid.controlStyle[ReoGridControlColors.OutlineButtonBorder]);

					float scale = Math.Min(grid.scaleFactor, 1f);
					int halfButtonSize = ReoGridControl.OutlineButtonSize / 2;

					for (int idx = 0; idx < outlines.Count; idx++)
					{
						OutlineGroup<ReoGridOutline> line = null;

						if (idx < outlines.Count - 1)
						{
							line = outlines[idx];

							foreach (var outline in line)
							{
								var endRow = grid.rows[outline.End];

								if (endRow.IsHidden) continue;

								if (grid.scaleFactor > 0.5f)
								{
									p.Width = 2;
								}

								Rectangle bbRect = outline.ToggleButtonBounds;
								float crossX = bbRect.X + bbRect.Width / 2 + 1;
								float crossY = bbRect.Y + bbRect.Height / 2 + 1;

								if (outline.Collapsed)
								{
									g.DrawLine(p, crossX, bbRect.Top + 3, crossX, bbRect.Bottom - 2);
								}
								else
								{
									// |

									var startRow = grid.rows[outline.Start];
									float y = startRow.Top * grid.scaleFactor;
									g.DrawLine(p, bbRect.Right - 1, y, crossX, y);
									g.DrawLine(p, crossX, y - 1, crossX, bbRect.Top);
								}

								// -
								g.DrawLine(p, bbRect.Left + 3, crossY, bbRect.Right - 2, crossY);

								// frame
								p.Width = 1;
								g.DrawRectangle(p, bbRect);
							}
						}

						// draw dot
						var prevGroup = idx <= 0 ? null : outlines[idx - 1];
						if (prevGroup != null)
						{
							int x = (int)Math.Round((3 + ReoGridControl.OutlineButtonSize) * idx * scale);

							foreach (var prevol in prevGroup)
							{
								if (!prevol.Collapsed)
								{
									for (int r = prevol.Start; r < prevol.End; r++)
									{
										if (line == null || !line.Any(o => o.Start <= r && o.End >= r))
										{
											var rowHead = grid.rows[r];
											if (!rowHead.IsHidden)
											{
												int y = (int)Math.Round((rowHead.Top + rowHead.Height / 2) * grid.scaleFactor);
												g.DrawLine(p, x + halfButtonSize, y - 1, x + halfButtonSize + 1, y - 1);
												g.DrawLine(p, x + halfButtonSize, y, x + halfButtonSize + 1, y);
											}
										}
									}
								}
							}
						}
					}
				}

#if DEBUG
				sw.Stop();
				long ms = sw.ElapsedMilliseconds;
				if (ms > 10)
				{
					Debug.WriteLine("draw row outlines takes " + ms + " ms.");
				}
#endif
			}
			#endregion

			protected override Rectangle CreateToggleButtonRect(int loc, int pos, int buttonSize)
			{
				var rowHead = grid.rows[pos];
				float rowTop = rowHead.Top * grid.scaleFactor;

				float rowMiddle = (rowHead.Height * grid.scaleFactor - buttonSize) / 2;
				int buttonY = (int)Math.Round((rowTop + rowMiddle));

				return new Rectangle(loc + 1, buttonY, buttonSize, buttonSize);
			}
		}

		private class ColumnOutlinePart : OutlinePart
		{
			public ColumnOutlinePart(ReoGridControl grid) : base(grid, RowOrColumn.Column) { }

			#region Draw
			public override void Draw(RGDrawingContext dc)
			{
				var g = dc.Graphics;

				g.FillRectangle(ResourcePoolManager.Instance.GetSolidBrush(grid.controlStyle[ReoGridControlColors.OutlinePanelBackground]),
					bounds.X, bounds.Y, bounds.Width + 1, bounds.Height);

				base.Draw(dc);

				g.DrawLine(ResourcePoolManager.Instance.GetPen(grid.controlStyle[ReoGridControlColors.OutlinePanelBorder]),
					bounds.Left, bounds.Bottom, bounds.Right, bounds.Bottom);
			}

			public override void DrawView(RGDrawingContext dc)
			{
#if DEBUG
				Stopwatch sw = Stopwatch.StartNew();
#endif
				var outlines = grid.outlines[RowOrColumn.Column];

				if (outlines != null)
				{
					var g = dc.Graphics;
					var p = ResourcePoolManager.Instance.GetPen(grid.controlStyle[ReoGridControlColors.OutlineButtonBorder]);

					float scale = Math.Min(grid.scaleFactor, 1f);
					int halfButtonSize = ReoGridControl.OutlineButtonSize / 2;

					for (int idx = 0; idx < outlines.Count; idx++)
					{
						OutlineGroup<ReoGridOutline> line = null;

						if (idx < outlines.Count - 1)
						{
							line = outlines[idx];

							foreach (var outline in line)
							{
								var endCol = grid.cols[outline.End];

								if (endCol.IsHidden) continue;

								if (grid.scaleFactor > 0.5f)
								{
									p.Width = 2;
								}

								Rectangle bbRect = outline.ToggleButtonBounds;

								float crossX = bbRect.X + bbRect.Width / 2 + 1;
								float crossY = bbRect.Y + bbRect.Height / 2 + 1;

								if (outline.Collapsed)
								{
									g.DrawLine(p, crossX, bbRect.Top + 3, crossX, bbRect.Bottom - 2);
								}
								else
								{
									// -

									var startCol = grid.cols[outline.Start];
									float x = startCol.Left * grid.scaleFactor;

									g.DrawLine(p, x, bbRect.Bottom - 1, x, crossY);
									g.DrawLine(p, x - 1, crossY, bbRect.Left, crossY);
								}

								// |
								g.DrawLine(p, bbRect.Left + 3, crossY, bbRect.Right - 2, crossY);

								// frame
								p.Width = 1;
								g.DrawRectangle(p, bbRect);
							}
						}

						// draw dot
						var prevGroup = idx <= 0 ? null : outlines[idx - 1];
						if (prevGroup != null)
						{
							int y = (int)Math.Round((3 + ReoGridControl.OutlineButtonSize) * idx * scale);

							foreach (var prevol in prevGroup)
							{
								if (!prevol.Collapsed)
								{
									for (int r = prevol.Start; r < prevol.End; r++)
									{
										if (line == null || !line.Any(o => o.Start <= r && o.End >= r))
										{
											var colHead = grid.cols[r];
											if (!colHead.IsHidden)
											{
												int x = (int)Math.Round((colHead.Left + colHead.Width / 2) * grid.scaleFactor);

												g.DrawLine(p, x - 1, y + halfButtonSize, x - 1, y + halfButtonSize + 1);
												g.DrawLine(p, x, y + halfButtonSize, x, y + halfButtonSize + 1);
											}
										}
									}
								}
							}
						}
					}
				}

#if DEBUG
				sw.Stop();
				long ms = sw.ElapsedMilliseconds;
				if (ms > 10)
				{
					Debug.WriteLine("draw column outlines takes " + ms + " ms.");
				}
#endif
			}
			#endregion

			protected override Rectangle CreateToggleButtonRect(int loc, int pos, int buttonSize)
			{
				var colHead = grid.cols[pos];
				float colLeft = colHead.Left * grid.scaleFactor;

				float colMiddle = (colHead.Width * grid.scaleFactor - buttonSize) / 2;
				int buttonX = (int)Math.Round((colLeft + colMiddle));

				return new Rectangle(buttonX, loc + 1, buttonSize, buttonSize);
			}

		}
		#endregion // Outline Parts

		#region XRuler (Reserved)
		private class XRuler : Part
		{
			public XRuler(ReoGridControl grid) : base(grid) { }
			public override void Draw(RGDrawingContext dc)
			{
				throw new NotImplementedException();
			}
		}
		#endregion // XRuler

		#region Space
		private class SpaceView : Part
		{
			public SpaceView(ReoGridControl grid) : base(grid) { }

			public override void Draw(RGDrawingContext dc)
			{
				dc.Graphics.FillRectangle(SystemBrushes.Control, bounds);
			}
		}
		#endregion // Space

		#endregion // Parts

		#region Viewport
		internal class CellsViewport : Viewport
		{
			public CellsViewport(ReoGridControl grid) : base(grid) { }
			#region Draw
			public override void Draw(RGDrawingContext dc)
			{
				if (!Visible //|| visibleGridRegion == GridRegion.Empty
					|| bounds.Width <= 0 || bounds.Height <= 0) return;

				Graphics g = dc.Graphics;

				var clipRect = this.bounds;
				clipRect.Width++;
				clipRect.Height++;

				//g.SetClip(clipRect);
				dc.PushClip(clipRect);
				g.TranslateTransform(bounds.Left - viewStart.X * grid.scaleFactor, bounds.Top - viewStart.Y * grid.scaleFactor);

				DrawView(dc);
				
				g.ResetTransform();

				#region Draw Header Resizing Line

				if (grid.currentColWidthChanging >= 0)
				{
					using (Pen p = new Pen(Color.Black))
					{
						p.DashStyle = DashStyle.Dot;
						ReoGridColHead col = grid.cols[grid.currentColWidthChanging];

						float left = Left + col.Left * grid.scaleFactor - ViewLeft * grid.scaleFactor;
						float right = Left + (col.Left + grid.adjustNewValue) * grid.scaleFactor - ViewLeft * grid.scaleFactor;

						g.DrawLine(p, left, Top, left, Top + Height);
						g.DrawLine(p, right, Top, right, Top + Height);
					}
				}
				else if (grid.currentRowHeightChanging >= 0)
				{
					using (Pen p = new Pen(Color.Black))
					{
						p.DashStyle = DashStyle.Dot;
						ReoGridRowHead row = grid.rows[grid.currentRowHeightChanging];

						float top = Top + row.Top * grid.scaleFactor - ViewTop * grid.scaleFactor;
						float bottom = Top + (row.Top + grid.adjustNewValue) * grid.scaleFactor - ViewTop * grid.scaleFactor;

						g.DrawLine(p, Left, top, Left + Width, top);
						g.DrawLine(p, Left, bottom, Left + Width, bottom);
					}
				}
				#endregion

				//g.ResetClip();
				dc.PopClip();
			}

			public override void DrawView(RGDrawingContext dc)
			{
				#region Background Grid
				if (grid.HasSettings(ReoGridSettings.View_ShowGridLine)
					// zoom < 40% will not display grid lines
					&& grid.scaleFactor >= 0.4f)
				{
					DrawGrid(dc);
				}
				#endregion

				#region Content
				DrawContent(dc);
				#endregion

				#region Selection
				// selection
				if (!grid.SelectionRange.IsEmpty && grid.SelectionStyle != ReoGridSelectionStyle.None)
				{
					var g = dc.Graphics;

					RectangleF selectionRect = grid.GetRangeBounds(grid.selectionRange);

					RectangleF scaledSelectionRect = new RectangleF(
						(selectionRect.X) * grid.scaleFactor,
						(selectionRect.Y) * grid.scaleFactor,
						(selectionRect.Width - 1) * grid.scaleFactor,
						(selectionRect.Height - 1) * grid.scaleFactor);

					float scaledViewTop = ViewTop * grid.scaleFactor;
					float scaledViewLeft = ViewLeft * grid.scaleFactor;

					float viewBottom = bounds.Height + scaledViewTop + 3; // 3: max select range border overflow
					float viewRight = bounds.Width + scaledViewLeft + 3;
					
					if (scaledSelectionRect.Y < scaledViewTop - 3)
					{
						scaledSelectionRect.Height -= scaledViewTop - scaledSelectionRect.Y - 3;
						scaledSelectionRect.Y = scaledViewTop - 3;
					}
					if (scaledSelectionRect.X < scaledViewLeft - 3)
					{
						scaledSelectionRect.Width -= scaledViewLeft - scaledSelectionRect.X - 3;
						scaledSelectionRect.X = scaledViewLeft - 3;
					}

					if (scaledSelectionRect.Bottom > viewBottom) scaledSelectionRect.Height = viewBottom - scaledSelectionRect.Y;
					if (scaledSelectionRect.Right > viewRight) scaledSelectionRect.Width = viewRight - scaledSelectionRect.X;

					if (scaledSelectionRect.Width > 0 || scaledSelectionRect.Height > 0)
					{
						if (grid.SelectionStyle == ReoGridSelectionStyle.Default)
						{
							Color selectionBorderColor = grid.controlStyle.Colors[ReoGridControlColors.SelectionBorder];
							Color selectionFillColor = grid.controlStyle.Colors[ReoGridControlColors.SelectionFill];

							Brush selectionBrush = ResourcePoolManager.Instance.GetSolidBrush(selectionFillColor);
							g.FillRectangle(selectionBrush, scaledSelectionRect.X, scaledSelectionRect.Y,
							scaledSelectionRect.Width, scaledSelectionRect.Height);

							if (!selectionBorderColor.IsEmpty)
							{
								Pen selectionPen = ResourcePoolManager.Instance.GetPen(selectionBorderColor,
									grid.controlStyle.SelectionBorderWidth, DashStyle.Solid);
								g.DrawRectangle(selectionPen, scaledSelectionRect.X, scaledSelectionRect.Y,
									scaledSelectionRect.Width, scaledSelectionRect.Height);
							}
						}
						else if (grid.SelectionStyle == ReoGridSelectionStyle.FocusRect)
						{
							scaledSelectionRect.Width++;
							scaledSelectionRect.Height++;
							ControlPaint.DrawFocusRectangle(g, Rectangle.Round(scaledSelectionRect));
						}
					}
				}
				#endregion
			}

			internal void DrawContent(RGDrawingContext dc)
			{
				#region Cells
				for (int r = visibleRegion.startRow; r <= visibleRegion.endRow; r++)
				{
					ReoGridRowHead rowHead = grid.rows[r];

					for (int c = visibleRegion.startCol; c <= visibleRegion.endCol; )
					{
						ReoGridCell cell = grid.cells[r, c];

						// draw cell onyl when cell's instance existing
						// and bounds of cell must be > 1 (minimum is 1, including one pixel border)
						if (cell != null && cell.Width > 1 && cell.Height > 1)
						{
							// single cell
							if (cell.Rowspan == 1 && cell.Colspan == 1)
								DrawCell(dc, cell);

							// merged cell start
							else if (cell.IsStartMergedCell)
								DrawCell(dc, cell);

							// merged cell end
							else if (cell.IsEndMergedCell
								&& !visibleRegion.Contains(cell.MergeStartPos))
								DrawCell(dc, grid.GetCell(cell.MergeStartPos));

							// merged cell outside visible region
							else if (cell.Row == visibleRegion.startRow
								&& cell.Col == visibleRegion.startCol
								&& (cell.MergeStartPos.Row < visibleRegion.startRow
								&& cell.MergeEndPos.Row > visibleRegion.endRow
								|| cell.MergeStartPos.Col < visibleRegion.startCol
								&& cell.MergeEndPos.Col > visibleRegion.endCol))
								DrawCell(dc, grid.GetCell(cell.MergeStartPos));

							int step = cell.Colspan;
							c += step > 0 ? step : 1;
						}
						else
						{
							DrawCellBackground(dc, r, c);
							c++;
						}
					}
				}
				#endregion

				#region Horizontal Borders
				for (int r = visibleRegion.startRow; r <= visibleRegion.endRow + 1; r++)
				{
					int y = r == grid.rows.Count ? grid.rows[r - 1].Bottom : grid.rows[r].Top;

					for (int c = visibleRegion.startCol; c <= visibleRegion.endCol; )
					{
						int x = grid.cols[c].Left;

						ReoGridHBorder cellBorder = grid.hBorders[r, c];
						if (cellBorder != null && cellBorder.Cols > 0 && cellBorder.Border != null)
						{
							int x2 = grid.cols[c + cellBorder.Cols - 1].Right;
							DrawBorder(dc, cellBorder.Border, x, y, x2, y);
							c += cellBorder.Cols;
						}
						else
							c++;
					}
				}
				#endregion

				#region Vertical Borders
				for (int c = visibleRegion.startCol; c <= visibleRegion.endCol + 1; c++)
				{
					int x = c == grid.cols.Count ? grid.cols[c - 1].Right : grid.cols[c].Left;

					for (int r = visibleRegion.startRow; r <= visibleRegion.endRow; )
					{
						int y = grid.rows[r].Top;

						ReoGridVBorder cellBorder = grid.vBorders[r, c];
						if (cellBorder != null && cellBorder.Rows > 0 && cellBorder.Border != null)
						{
							int y2 = grid.rows[r + cellBorder.Rows - 1].Bottom;
							DrawBorder(dc, cellBorder.Border, x, y, x, y2);
							r += cellBorder.Rows;
						}
						else
							r++;
					}
				}
				#endregion
			}

			private void DrawCell(RGDrawingContext dc, ReoGridCell cell)
			{
				if (cell == null) return;

				if (cell.IsMergedCell
					&& (cell.Width <= 1 || cell.Height <= 1)) return;
				
				if (cell.Body != null)
				{
					dc.Cell = cell;

					Stopwatch sw = Stopwatch.StartNew();

					Graphics g = dc.Graphics;

					var bc = g.BeginContainer();

					if (scaleFactor != 1f)
					{
						g.ScaleTransform(scaleFactor, scaleFactor);
					}

					g.TranslateTransform(dc.Cell.Left, dc.Cell.Top);

					cell.Body.OnPaint(dc);

					g.EndContainer(bc);
				}
				else
				{
					DrawCellBackground(dc, cell.Row, cell.Col);

					if (!string.IsNullOrEmpty(cell.Display))
					{
						DrawCellText(dc, cell);
					}
				}
			}

			internal void DrawCellText(RGDrawingContext dc, ReoGridCell cell)
			{
				Color textColor = Color.Empty;

				if (!cell.RenderColor.IsEmpty)
				{
					// render color, used to render negative number, specified by data formatter
					textColor = cell.RenderColor;
				}
				else if (!cell.Style.TextColor.IsEmpty)
				{
					// cell text color, specified by SetRangeStyle
					textColor = cell.Style.TextColor;
				}
				else
				{
					// default cell text color
					textColor = grid.controlStyle[ReoGridControlColors.GridText];
				}

				var b = textColor.IsEmpty ? Brushes.Black : ResourcePoolManager.Instance.GetSolidBrush(textColor);

				Graphics g = dc.Graphics;

				if (cell.Style != null && cell.Style.HAlign == ReoGridHorAlign.DistributedIndent)
				{
					float x = cell.TextBounds.Left;

					for (int i = 0; i < cell.Display.Length; i++)
					{
						string c = cell.Display[i].ToString();

						g.DrawString(c, cell.ScaledFont, b, x, cell.TextBounds.Top, cell.StringFormat);
						x += cell.DistributedIndentSpacing;
					}
				}
				else
				{

					RectangleF textRect = cell.TextBounds;

#if ALLOW_CELL_CLIP

					//
					// TODO: to support cell clip by using PushClip
					//       but clip works not so good, and this is very slow
					//

					Rectangle clipRect = new Rectangle(
						(int)Math.Round(cell.Left * grid.scaleFactor),
						(int)Math.Round(cell.Top * grid.scaleFactor),
						0,
						(int)Math.Round((cell.Height - 1) * grid.scaleFactor + 1));

					bool needWidthClip = (cell.IsMergedCell || cell.Style.TextWrapMode == TextWrapMode.WordBreak);

					if(needWidthClip)
					{
						clipRect.Width = (int)Math.Round(cell.Width * grid.scaleFactor);
					}
					else
					{
						// set to max view width
						clipRect.Width = (int)Math.Round(bounds.Width * grid.scaleFactor - textRect.Left);
					}

					dc.PushClip(clipRect);
#endif
					g.DrawString(cell.Display, cell.ScaledFont, b, textRect, cell.StringFormat);

#if ALLOW_CELL_CLIP
					dc.PopClip();
#endif
				}
			}

			internal void DrawCellBackground(RGDrawingContext dc, int row, int col)
			{
				ReoGridRangeStyle style = grid.GetCellStyle(row, col);

				if (!style.BackColor.IsEmpty && style.BackColor.A > 0)
				{
					RectangleF cellBounds = grid.GetCellBounds(row, col);

					// cell bounds must be not zero (minimum is 1, including one pixel border)
					if (cellBounds.Width > 1 && cellBounds.Height > 1)
					{
						Rectangle rect = new Rectangle(
							(int)Math.Round(cellBounds.X * grid.scaleFactor),
							(int)Math.Round(cellBounds.Y * grid.scaleFactor),
							(int)Math.Round((cellBounds.Width - 1) * grid.scaleFactor + 1),
							(int)Math.Round((cellBounds.Height - 1) * grid.scaleFactor + 1));

						Graphics g = dc.Graphics;

						if (!style.FillPatternColor.IsEmpty)
						{
							HatchBrush hb = ResourcePoolManager.Instance.GetHatchBrush(style.FillPatternStyle,
								style.FillPatternColor, style.BackColor);
							g.FillRectangle(hb, rect);
						}
						else
						{
							SolidBrush b = ResourcePoolManager.Instance.GetSolidBrush(style.BackColor);
							g.FillRectangle(b, rect);
						}
					}
				}
			}

			private void DrawGrid(RGDrawingContext dc)
			{
				Graphics g = dc.Graphics;

				Pen p = ResourcePoolManager.Instance.GetPen(grid.controlStyle.Colors[ReoGridControlColors.GridLine]);

				// horizontal line
				for (int r = visibleRegion.startRow; r <= visibleRegion.endRow + 1; r++)
				{
					float y = r == grid.rows.Count ? grid.rows[r - 1].Bottom : grid.rows[r].Top;
					float scaledY = y * grid.scaleFactor;

					for (int c = visibleRegion.startCol; c <= visibleRegion.endCol; c++)
					{
						// skip horizontal border - line start
						ReoGridHBorder cellBorder = null;

						int x = grid.cols[c].Left;
						int x2 = grid.cols[c].Right;

						// skip horizontal border - line end
						while (c <= visibleRegion.endCol)
						{
							cellBorder = grid.hBorders[r, c];
							if (cellBorder != null && cellBorder.Cols >= 0) break;

							c++;
						}

						x2 = c == 0 ? grid.cols[c].Left : grid.cols[c - 1].Right;

						g.DrawLine(p, x * grid.scaleFactor, scaledY, x2 * grid.scaleFactor, scaledY);
					}
				}

				// vertical line
				for (int c = visibleRegion.startCol; c <= visibleRegion.endCol + 1; c++)
				{
					float x = c == grid.cols.Count ? grid.cols[c - 1].Right : grid.cols[c].Left;
					float scaledX = x * grid.scaleFactor;

					for (int r = visibleRegion.startRow; r <= visibleRegion.endRow; r++)
					{
						ReoGridVBorder cellBorder = null;

						int y = grid.rows[r].Top;
						int y2 = grid.rows[r].Bottom;

						while (r <= visibleRegion.endRow)
						{
							cellBorder = grid.vBorders[r, c];
							if (cellBorder != null && cellBorder.Rows >= 0) break;

							r++;
						}

						y2 = r == 0 ? grid.rows[r].Top : grid.rows[r - 1].Bottom;

						g.DrawLine(p, scaledX, y * grid.scaleFactor, scaledX, y2 * grid.scaleFactor);
					}
				}
			}

			private void DrawBorder(RGDrawingContext dc, ReoGridBorderStyle style, int x, int y, int x2, int y2)
			{
				BorderPainter.Instance.DrawLine(dc.Graphics, x * grid.scaleFactor, y * grid.scaleFactor,
					x2 * grid.scaleFactor, y2 * grid.scaleFactor, style);
			}
			#endregion
		}
		#endregion

		#region Viewport Controller
		internal enum VisibleViews
		{
			None = 0,
			
			ColumnHead = 1,
			RowHead = 2,
			LeadHead = ColumnHead | RowHead,
			
			ColOutline = 4,
			RowOutline = 8,
			Outlines = ColOutline | RowOutline,
		}

		#region Abstract ViewportController
		/// <summary>
		/// Interface for freezable ViewportController
		/// </summary>
		public interface IFreezableViewportController
		{
			/// <summary>
			/// Freeze to specified cell
			/// </summary>
			/// <param name="row">Number of row of cell position</param>
			/// <param name="col">Number of column of cell position</param>
			void Freeze(int row, int col);
		}

		internal abstract class AbstractViewportController
		{
			protected ReoGridControl grid;
			public ReoGridControl Grid { get { return grid; } set { grid = value; } }

			public AbstractViewportController(ReoGridControl grid)
			{
				this.grid = grid;
			}

			#region Bounds
			protected Rectangle bounds;
			internal Rectangle Bounds { get { return bounds; } set { bounds = value; UpdateBounds(); } }
			internal Size Size { get { return bounds.Size; } set { bounds.Size = value; Resize(); } }
			internal int Left { get { return bounds.X; } set { bounds.X = value; } }
			internal int Top { get { return bounds.Y; } set { bounds.Y = value; } }
			internal int Width { get { return bounds.Width; } set { bounds.Width = value; } }
			internal int Height { get { return bounds.Height; } set { bounds.Height = value; } }

			#endregion

			#region Viewport Management
			private List<IPart> parts = new List<IPart>(5);

			internal List<IPart> Viewports
			{
				get { return parts; }
				set { parts = value; }
			}

			internal virtual void AddPart(IPart viewport)
			{
				this.parts.Add(viewport);
			}

			internal virtual void InsertPart(IPart before, IPart viewport)
			{
				int index = this.parts.IndexOf(before);
				if (index > 0 && index < this.parts.Count)
				{
					this.parts.Insert(index, viewport);
				}
				else
				{
					this.parts.Add(viewport);
				}
			}

			internal virtual void InsertPart(int index, IPart viewport)
			{
				this.parts.Insert(index, viewport);
			}

			protected VisibleViews headVisible = VisibleViews.LeadHead;

			internal bool IsHeadVisible(VisibleViews head)
			{
				return (headVisible & head) == head;
			}

			internal virtual void SetHeadVisible(VisibleViews head, bool visible)
			{
				if (visible)
				{
					headVisible |= head;
				}
				else
				{
					headVisible &= ~head;
				}

				UpdateController();
			}

			#endregion

			internal virtual void ScrollViews(ScrollDirection dir, float left, float top)
			{
				if (left == 0 && top == 0) return;

				foreach (var v in Viewports)
				{
					if (v is IViewPart)
					{
						IViewPart viewpart = v as IViewPart;

#if WIN32_SCROLL
						bool scrolled = false;
						Rectangle scrolledRect = Rectangle.Empty;
#endif

						if (viewpart.ScrollableDirections == dir)
						{

#if WIN32_SCROLL
							viewpart.Scroll(left, top);
#else
							viewpart.Scroll(left, top);
#endif
						}
						else
						{
							if ((viewpart.ScrollableDirections & ScrollDirection.Horizontal) == ScrollDirection.Horizontal
								&& (dir & ScrollDirection.Horizontal) == ScrollDirection.Horizontal)
							{
#if WIN32_SCROLL
								viewpart.Scroll(left, 0);
#else
								viewpart.Scroll(left, 0);
#endif
							}

							if ((viewpart.ScrollableDirections & ScrollDirection.Vertical) == ScrollDirection.Vertical
							 && (dir & ScrollDirection.Vertical) == ScrollDirection.Vertical)
							{
#if WIN32_SCROLL
								viewpart.Scroll(0, top);
#else
								viewpart.Scroll(0, top);
#endif
							}
						}
					}
				}

				// TODO: Performance Optimization: update visible region by offset 
				UpdateVisibleRegion();

#if WIN32_SCROLL
				var	updateRect = bounds;

				if (left > 0)
				{
					updateRect.X = updateRect.Right - left;
					updateRect.Width = left;
				}
				else if (left < 0)
				{
					updateRect.Width = -left;
				}

				if (top > 0)
				{
					updateRect.Y = updateRect.Bottom - top;
					updateRect.Height = top;
				}
				else if (top < 0)
				{
					updateRect.Height = -top;
				}

				grid.Invalidate(updateRect);
#else
				grid.InvalidateCanvas();
#endif

			}

			internal abstract void UpdateBounds();

			//internal virtual void UpdateViewports()
			//{
			//}

			internal virtual void UpdateVisibleRegion() { }

			internal virtual void UpdateController()
			{
				foreach (var v in this.parts)
				{
					if (v is IViewPart) ((IViewPart)v).UpdateView();
				}

				UpdateBounds();
			}

			protected abstract void Resize();
			internal protected abstract void Scale(float scaleFactor);

			/// <summary>
			/// Update visible region for viewport. Visible region decides how many rows and columns 
			/// of cells (from...to) will be displayed.
			/// </summary>
			internal virtual GridRegion GetVisibleRegion(RectangleF viewRect, GridRegion oldVisibleRegion)
			{
#if DEBUG
				Stopwatch watch = Stopwatch.StartNew();
#endif
				PointF viewStart = viewRect.Location;

				GridRegion region = GridRegion.Empty;

				float scaledViewLeft = viewStart.X;
				float scaledViewTop = viewStart.Y;
				float scaledViewRight = viewStart.X + viewRect.Width / grid.scaleFactor;
				float scaledViewBottom = viewStart.Y + viewRect.Height / grid.scaleFactor;

				// begin visible region updating
				if (viewRect.Height > 0 && grid.rows.Count > 0)
				{
					float contentBottom = grid.rows.Last().Bottom;

					if (scaledViewTop > contentBottom)
					{
						region.startRow = grid.RowCount - 1;
					}
					else
					{
						int index = grid.rows.Count >> 1;
						ArrayHelper.QuickFind(index, 0, grid.rows.Count, (rindex) =>
						{
							ReoGridRowHead row = grid.rows[rindex];

							float top = row.Top;
							float btn = row.Bottom;

							if (scaledViewTop >= top && scaledViewTop <= btn)
							{
								region.startRow = rindex;
								return 0;
							}
							else if (scaledViewTop < top)
								return -1;
							else if (scaledViewTop > btn)
								return 1;
							else
								throw new InvalidOperationException();			// this case should not be reached
						});
					}

					if (scaledViewBottom > contentBottom)
					{
						region.endRow = grid.rows.Count - 1;
					}
					else
					{
						int index = grid.rows.Count >> 1;
						ArrayHelper.QuickFind(index, 0, grid.rows.Count, (rindex) =>
						{
							ReoGridRowHead row = grid.rows[rindex];

							float top = row.Top;
							float btn = row.Bottom;

							if (scaledViewBottom >= top && scaledViewBottom <= btn)
							{
								region.endRow = rindex;
								return 0;
							}
							else if (scaledViewBottom < top)
								return -1;
							else if (scaledViewBottom > btn)
								return 1;
							else
								throw new InvalidOperationException();			// this case should not be reached
						});
					}
				}

				if (viewRect.Width > 0 && grid.cols.Count > 0)
				{
					float contentRight = grid.cols.Last().Right;

					if (scaledViewLeft > contentRight)
					{
						region.startCol = grid.cols.Count - 1;
					}
					else
					{
						int index = grid.cols.Count >> 1;
						ArrayHelper.QuickFind(index, 0, grid.cols.Count, (cindex) =>
						{
							ReoGridColHead col = grid.cols[cindex];

							float left = col.Left;
							float rgt = col.Right;

							if (scaledViewLeft >= left && scaledViewLeft <= rgt)
							{
								region.startCol = cindex;
								return 0;
							}
							else if (scaledViewLeft < left)
								return -1;
							else if (scaledViewLeft > rgt)
								return 1;
							else
								throw new InvalidOperationException();			// this case should not be reached
						});
					}

					if (scaledViewRight > contentRight)
					{
						region.endCol = grid.cols.Count - 1;
					}
					else
					{
						int index = grid.cols.Count >> 1;
						ArrayHelper.QuickFind(index, 0, grid.cols.Count, (cindex) =>
						{
							ReoGridColHead col = grid.cols[cindex];

							float left = col.Left;
							float rgt = col.Right;

							if (scaledViewRight >= left && scaledViewRight <= rgt)
							{
								region.endCol = cindex;
								return 0;
							}
							else if (scaledViewRight < left)
								return -1;
							else if (scaledViewRight > rgt)
								return 1;
							else
								throw new InvalidOperationException();			// this case should not be reached
						});
					}
				}
				

#if DEBUG
				Debug.Assert(region.endRow >= region.startRow);
				Debug.Assert(region.endCol >= region.startCol);

				watch.Stop();

				// for unsual visible region
				// when over 200 rows or columns were setted as visible region,
				// we need check for whether the algorithm above has any mistake.
				if (region.Rows > 200 || region.Cols > 200)
				{
					Debug.WriteLine(string.Format("unusual visible region detected: [row: {0} - {1}, col: {2} - {3}]: {4} ms.",
						region.startRow, region.endRow, region.startCol, region.endCol,
						watch.ElapsedMilliseconds));
				}

				if (watch.ElapsedMilliseconds > 15)
				{
					Debug.WriteLine("update visible region takes " + watch.ElapsedMilliseconds + " ms.");
				}
#endif

				return region;
			}

			internal virtual void Draw(RGDrawingContext dc)
			{
				foreach(var v in parts)
				{
					if (v.Visible && v.Bounds.Width > 0 && v.Bounds.Height > 0)
					{
						dc.CurrentPart = v;
						v.Draw(dc);
					}
				}
			}

			#region Part Hittest
			internal T PointInPart<T>(PointF p) where T : IPart
			{
				return (T)parts.FirstOrDefault(v => v is T && v.Visible && v.Hittest(p));
			}
			internal bool PointInLeadHead(PointF p)
			{
				return PointInPart<LeadHeadPart>(p) != null;
			}
			internal bool PointInColumnHead(PointF p)
			{
				return PointInPart<ColumnHeaderPart>(p) != null;
			}
			internal bool PointInRowHead(PointF p)
			{
				return PointInPart<RowHeaderPart>(p) != null;
			}
			internal bool PointInCells(PointF p)
			{
				return PointInPart<CellsViewport>(p) != null;
			}
			#endregion

			#region View Point Evalution
			public CellsViewport ActiveView { get; set; }

			public void ChangeActiveView(Point p)
			{
				CellsViewport viewpart = parts.FirstOrDefault(v => (v is CellsViewport) && v.Bounds.Contains(p)) as CellsViewport;
				if (viewpart != null) ActiveView = viewpart;
			}

			internal PointF PointToActivePart(Point p)
			{
				PointF vp = ActiveView.PointToView(p);
				return new PointF(vp.X - bounds.X, vp.Y - bounds.Y);
			}

			internal ReoGridPos GetPosByPoint(PointF p)
			{
				return (ActiveView == null) ? ReoGridPos.Empty : ActiveView.GetPosByPoint(PointToPart(ActiveView, p));
			}

			internal int GetRowByPoint(float p)
			{
				return (ActiveView != null) ? ActiveView.GetRowByPoint(p) : -1;
			}
			internal int GetColByPoint(float p)
			{
				return (ActiveView != null) ? ActiveView.GetColByPoint(p) : -1;
			}

			internal PointF PointToPart(IPart part, PointF p)
			{
				return (part == null) ? PointF.Empty : part.PointToView(p);
			}
			internal PointF PointToControl(IPart part, PointF p)
			{
				return (part == null) ? PointF.Empty : part.PointToControl(p);
			}

			internal IPart GetPartByPoint(Point p)
			{
				return parts.FirstOrDefault(part => part.Bounds.Contains(p));
			}

			internal bool FindColHeadIndex(float x, out int col)
			{
				int v = -1;
				bool inline = true;

				for (int i = 0; i < grid.cols.Count; i++)
				{
					if (x <= grid.cols[i].Right - 2)
					{
						inline = false;
						v = i;
						break;
					}
					else if (x <= grid.cols[i].Right + 2)
					{
						v = i;
						break;
					}
				}

				col = v;
				return inline;
			}
			internal bool FindRowHeadIndex(float y, out int row)
			{
				// TODO: need performance improvement
				int v = -1;
				bool inline = true;

				for (int i = 0; i < grid.rows.Count; i++)
				{
					if (y <= grid.rows[i].Bottom - 2)
					{
						inline = false;
						v = i;
						break;
					}
					else if (y <= grid.rows[i].Bottom + 2)
					{
						v = i;
						break;
					}
				}

				row = v;
				return inline;
			}
			#endregion // View Point Evalution

			internal virtual bool OnMouseDown(MouseEventArgs e)
			{
				var processed = false;

				Point p = new Point(e.X - bounds.X, e.Y - bounds.Y);

				foreach (var part in this.Viewports)
				{
					if (part.Visible && part.Bounds.Contains(p))
					{
						processed = part.OnMouseDown(Point.Round(part.PointToView(p)), e.Button, e.Clicks);
						if (processed) break;
					}
				}

				return processed;
			}

			internal virtual bool OnMouseUp(MouseEventArgs e)
			{
				var processed = false;

				foreach (var part in this.Viewports)
				{
					if (part.Visible && !part.Bounds.IsEmpty)
					{
						processed = part.OnMouseUp(Point.Round(this.PointToPart(part, e.Location)), e.Button, e.Clicks);
						if (processed) break;
					}
				}

				return processed;
			}

			public Point CellPositionToControl(ReoGridPos pos)
			{
				pos = grid.FixPos(pos);

				return new Point((int)Math.Round((grid.cols[pos.Col].Left * grid.scaleFactor + ActiveView.Left - ActiveView.ViewLeft * grid.scaleFactor)),
					(int)Math.Round((grid.rows[pos.Row].Top * grid.scaleFactor + ActiveView.Top - ActiveView.ViewTop * grid.scaleFactor)));
			}

			public virtual void Reset()
			{
			}
		}
		#endregion

		#region Normal ViewportController
		// TODO: ScrollableViewportController will be change name to NormalLayoutViewportController
		private class NormalViewportController : AbstractViewportController, IFreezableViewportController
		{
			private HScrollBar hScrollBar = null;
			private VScrollBar vScrollBar = null;

			private LeadHeadPart leadHeadPart;
			private ColumnHeaderPart columnHeadPart;
			private RowHeaderPart rowHeadPart;
			private CellsViewport mainViewport;

			private SpaceView rightBottomSpace;

			public NormalViewportController(ReoGridControl grid)
				: base(grid)
			{
				hScrollBar = new HScrollBar()
				{
					Size = new Size(20, 20),
					Dock = DockStyle.Fill,
					SmallChange = grid.defaultColumnWidth,
				};
				hScrollBar.Scroll += OnHorScroll;

				vScrollBar = new VScrollBar()
				{
					Size = new Size(20, 20),
					Dock = DockStyle.Fill,
					SmallChange = grid.defaultRowHeight,
				};
				vScrollBar.Scroll += OnVerScroll;

				grid.bottomPanel.Controls.Add(hScrollBar);
				grid.rightPanel.Controls.Add(vScrollBar);
				hScrollBar.BringToFront();

				// space
				AddPart(rightBottomSpace = new SpaceView(grid));

				// unfreezed
				AddPart(leadHeadPart = new LeadHeadPart(grid));
				AddPart(columnHeadPart = new ColumnHeaderPart(grid));
				AddPart(rowHeadPart = new RowHeaderPart(grid));
				AddPart(mainViewport = new CellsViewport(grid));
				ActiveView = mainViewport;

				columnHeadPart.ScrollableDirections = ScrollDirection.Horizontal;
				rowHeadPart.ScrollableDirections = ScrollDirection.Vertical;
				mainViewport.ScrollableDirections = ScrollDirection.Both;
			}

			#region Visibility Management
			internal override void SetHeadVisible(VisibleViews head, bool visible)
			{
				if (visible)
				{
					base.headVisible |= head;
				}
				else
				{
					base.headVisible &= ~head;
				}

				bool rowHeadVisible = IsHeadVisible(VisibleViews.RowHead);
				bool colHeadVisible = IsHeadVisible(VisibleViews.ColumnHead);

				bool outlineRowHeadVisible = IsHeadVisible(VisibleViews.RowOutline | VisibleViews.ColumnHead);
				bool outlineColHeadVisible = IsHeadVisible(VisibleViews.ColOutline | VisibleViews.RowHead);

				#region Column Head
				if ((head & VisibleViews.ColumnHead) == VisibleViews.ColumnHead)
				{
					if (visible)
					{
						columnHeadPart.Visible = true;
						columnHeadPart.VisibleRegion = mainVisibleRegion;

						if (freezePos.Col > 0)
						{
							freezeColumnHead.Visible = true;
							freezeColumnHead.VisibleRegion = frozenVisibleRegion;
						}
					}
					else
					{
						columnHeadPart.Visible = false;
						
						if (freezeColumnHead != null) freezeColumnHead.Visible = false;
					}
				}
				#endregion // Column Head

				#region Row Head
				if ((head & VisibleViews.RowHead) == VisibleViews.RowHead)
				{
					if (visible)
					{
						rowHeadPart.Visible = true;
						rowHeadPart.VisibleRegion = mainVisibleRegion;

						if (freezePos.Row > 0)
						{
							freezeRowHead.Visible = true;
							freezeRowHead.VisibleRegion = frozenVisibleRegion;
						}
					}
					else
					{
						rowHeadPart.Visible = false;

						if (freezeRowHead != null) freezeRowHead.Visible = false;
					}
				}
				#endregion

				leadHeadPart.Visible = IsHeadVisible(VisibleViews.LeadHead);

				if ((head & VisibleViews.RowOutline) == VisibleViews.RowOutline)
				{
					bool rowOutlineVisible = IsHeadVisible(VisibleViews.RowOutline);

					if (rowOutlineVisible)
					{
						if (visible && this.rowOutlinePart == null)
						{
							this.rowOutlinePart = new RowOutlinePart(grid);
							this.rowOutlinePart.ScrollableDirections = ScrollDirection.Vertical;
							this.AddPart(this.rowOutlinePart);
						}

						if (rowOutlinePart != null) rowOutlinePart.Visible = rowOutlineVisible;
						if (freezeRowOutlinePart != null) freezeRowOutlinePart.Visible = rowOutlineVisible;
					}
					else
					{
						if (rowOutlinePart != null) rowOutlinePart.Visible = false;
						if (freezeRowOutlinePart != null) freezeRowOutlinePart.Visible = false;
					}
				}

				if ((head & VisibleViews.RowOutline | VisibleViews.ColumnHead) > 0)
				{
					if (outlineRowHeadVisible)
					{
						if (this.rowOutlineHeadPart == null)
						{
							this.rowOutlineHeadPart = new RowOutlineHeadPart(grid);
							this.AddPart(this.rowOutlineHeadPart);
						}
						
						this.rowOutlineHeadPart.Visible = true;
					}
					else if (this.rowOutlineHeadPart != null)
					{
						this.rowOutlineHeadPart.Visible = false;
					}
				}

				bool colOutlineVisible = IsHeadVisible(VisibleViews.ColOutline);
				if (colOutlineVisible)
				{
					if (visible && this.colOutlinePart == null)
					{
						this.colOutlinePart = new ColumnOutlinePart(grid);
						this.colOutlinePart.ScrollableDirections = ScrollDirection.Horizontal;
						this.AddPart(this.colOutlinePart);
					}

					if (colOutlinePart != null) colOutlinePart.Visible = colOutlineVisible;
					if (freezeColOutlinePart != null) freezeColOutlinePart.Visible = colOutlineVisible;
				}
				else
				{
					if (colOutlinePart != null) colOutlinePart.Visible = false;
					if (freezeColOutlinePart != null) freezeColOutlinePart.Visible = false;
				}

				if ((head & VisibleViews.ColOutline | VisibleViews.RowHead) > 0)
				{
					if (outlineColHeadVisible)
					{
						if (this.colOutlineHeadPart == null)
						{
							this.colOutlineHeadPart = new ColumnOutlineHeadPart(grid);
							this.AddPart(this.colOutlineHeadPart);
						}
						
						this.colOutlineHeadPart.Visible = true;
					}
					else if (this.colOutlineHeadPart != null)
					{
						this.colOutlineHeadPart.Visible = false;
					}
				}

				if ( this.colOutlineHeadPart != null && this.rowOutlineHeadPart != null
					&& this.colOutlineHeadPart.Visible && this.rowOutlineHeadPart.Visible)
				{
					if (this.outlineLeftTopSpace == null)
					{
						this.outlineLeftTopSpace = new OutlineLeftTopSpace(grid);
						this.AddPart(outlineLeftTopSpace);
					}

					this.outlineLeftTopSpace.Visible = visible;
				}
				else if (this.outlineLeftTopSpace != null)
				{
					this.outlineLeftTopSpace.Visible = false;
				}

				UpdateController();
			}

			private RectangleF GetGridScaleBounds(ReoGridPos pos)
			{
				return GetGridScaleBounds(pos.Row, pos.Col);
			}
			private RectangleF GetGridScaleBounds(int row, int col)
			{
				var rowHead = grid.rows[freezePos.Row];
				var colHead = grid.cols[freezePos.Col];

				return new RectangleF(colHead.Left * grid.scaleFactor, rowHead.Top * grid.scaleFactor,
					colHead.Width * grid.scaleFactor + 1, rowHead.Height * grid.scaleFactor + 1);
			}
			#endregion // Visibility Management

			#region Update

			GridRegion frozenVisibleRegion = GridRegion.Empty;
			GridRegion mainVisibleRegion = GridRegion.Empty;

			// TODO: this method need to be optimised
			internal override void UpdateBounds()
			{
#if DEBUG
				Stopwatch sw = Stopwatch.StartNew();
#endif
				bool coloutlineVisible = IsHeadVisible(VisibleViews.ColOutline);
				bool rowoutlineVisible = IsHeadVisible(VisibleViews.RowOutline);

				bool leadheadVisible = IsHeadVisible(VisibleViews.LeadHead);
				bool colheadVisible = IsHeadVisible(VisibleViews.ColumnHead);
				bool rowheadVisible = IsHeadVisible(VisibleViews.RowHead);

				bool isFrozen = freezePos.Row > 0 || freezePos.Col > 0;

				float left = bounds.Left;
				float top = bounds.Top;

				float leadheadWidth = 0, leadheadHeight = 0;
				if (leadheadVisible || rowheadVisible || colheadVisible)
				{
					leadheadWidth = grid.rowHeaderWidth * grid.scaleFactor;
					leadheadHeight = grid.colHeaderHeight * grid.scaleFactor;
				}

				float headerHeight = 0;

				if (colheadVisible)
				{
					headerHeight += leadheadHeight;
				}

				float minScale = Math.Min(grid.scaleFactor, 1f);

				float colOutlinePartHeight = 0;
				if (coloutlineVisible)
				{
					var outlines = grid.outlines[RowOrColumn.Column];
					if (outlines != null)
					{
						colOutlinePartHeight = (outlines.Count * (ReoGridControl.OutlineButtonSize + 3) * minScale);
					}

					headerHeight += colOutlinePartHeight;
				}

				#region Row Outline Panel
				if (rowoutlineVisible)
				{
					var outlines = grid.outlines[RowOrColumn.Row];

					this.rowOutlinePart.Bounds = new RectangleF(left, headerHeight,
						outlines == null ? 0 : (outlines.Count * (ReoGridControl.OutlineButtonSize + 3) * minScale),
						bounds.Height);

					if (this.rowOutlineHeadPart.Visible)
					{
						this.rowOutlineHeadPart.Bounds = new RectangleF(this.rowOutlinePart.Left,
							colOutlinePartHeight, this.rowOutlinePart.Width, leadheadHeight);
					}

					left = this.rowOutlinePart.Right;
				}
				#endregion // Row Outline Panel

				float headerWidth = 0;

				if (leadheadVisible)
				{
					headerWidth = leadheadWidth;
				}
				if (rowoutlineVisible)
				{
					headerWidth += this.rowOutlinePart.Width;
				}

				#region Column Outline Panel
				if (coloutlineVisible)
				{
					if (rowheadVisible)
					{
						this.colOutlineHeadPart.Bounds = new RectangleF(left, bounds.Top,
							leadheadWidth, colOutlinePartHeight);
					}

					float colOutlinePartleft = rowheadVisible ? colOutlineHeadPart.Right : left;

					this.colOutlinePart.Bounds = new RectangleF(
						colOutlinePartleft, bounds.Top,
						bounds.Width - colOutlinePartleft, colOutlinePartHeight);

					top = this.colOutlinePart.Bottom;
				}
				#endregion // Column Outline Panel

				#region Lead header
				if (leadheadVisible)
				{
					leadHeadPart.Bounds = new RectangleF(left, top, leadheadWidth, leadheadHeight);

					top = this.leadHeadPart.Bottom;
				}
				#endregion // Lead Header

				float colHeadWidth = bounds.Width - headerWidth;
				float rowHeadHeight = bounds.Height - headerHeight;
			
				#region Row Header
				// row head
				if (rowheadVisible)
				{
					rowHeadPart.Bounds = new RectangleF(left, top,
						grid.rowHeaderWidth * grid.scaleFactor, rowHeadHeight);

					left = rowHeadPart.Right;
				}
				#endregion // Row Header

				#region Column Header
				// col head
				if (colheadVisible)
				{
					columnHeadPart.Bounds = new RectangleF(left, leadHeadPart.Top,
						colHeadWidth, grid.colHeaderHeight * grid.scaleFactor);

					top = this.columnHeadPart.Bottom;
				}
				#endregion // Column Header

				if (rowoutlineVisible && coloutlineVisible)
				{
					outlineLeftTopSpace.Visible = true;
					outlineLeftTopSpace.Bounds = new RectangleF(bounds.X, bounds.Y, rowOutlineHeadPart.Width,
						colOutlineHeadPart.Height);
				}

				RectangleF contentBounds = new RectangleF(left, top, colHeadWidth, rowHeadHeight);

				if (isFrozen)
				{
					RectangleF gridLoc = GetGridScaleBounds(freezePos);
					
					// cells
					freezeLeftTopViewport.Bounds = new RectangleF(contentBounds.Left, contentBounds.Top, gridLoc.X, gridLoc.Y);
					freezeTopViewport.Bounds = new RectangleF(freezeLeftTopViewport.Right, contentBounds.Top, 
						contentBounds.Width - gridLoc.X, gridLoc.Y);
					freezeLeftViewport.Bounds = new RectangleF(contentBounds.Left, freezeLeftTopViewport.Bottom,
						gridLoc.X, contentBounds.Height - freezeLeftTopViewport.Height);

					// head
					freezeColumnHead.Bounds = new RectangleF(contentBounds.Left, columnHeadPart.Top,
						gridLoc.X, columnHeadPart.Height);
					freezeRowHead.Bounds = new RectangleF(leadHeadPart.Left, contentBounds.Top, 
						rowHeadPart.Width, gridLoc.Y);

					// normal
					mainViewport.Bounds = new RectangleF(freezeTopViewport.Left, freezeLeftViewport.Top,
						freezeTopViewport.Width, freezeLeftViewport.Height);
					columnHeadPart.Left += gridLoc.X;
					columnHeadPart.Width -= gridLoc.X;
					rowHeadPart.Top += gridLoc.Y;
					rowHeadPart.Height -= gridLoc.Y;

					// outline
					if (rowOutlinePart != null)
					{
						rowOutlinePart.Top += gridLoc.Y;
						rowOutlinePart.Height -= gridLoc.Y;

						if (freezeRowOutlinePart != null)
						{
							freezeRowOutlinePart.Bounds = new RectangleF(rowOutlinePart.Left,
								freezeRowHead.Top, rowOutlinePart.Width, freezeRowHead.Height);
						}
					}

					if (colOutlinePart != null)
					{
						colOutlinePart.Left += gridLoc.X;
						colOutlinePart.Width -= gridLoc.X;

						if (freezeColOutlinePart != null)
						{
							freezeColOutlinePart.Bounds = new RectangleF(freezeColumnHead.Left,
								colOutlinePart.Top, freezeColumnHead.Width, colOutlinePart.Height);
						}
					}
					
				}
				else
				{
					mainViewport.Bounds = new RectangleF(contentBounds.Left, contentBounds.Top,
						contentBounds.Width, contentBounds.Height);
				}

				if (mainViewport.Width < 0) mainViewport.Width = 0;
				if (mainViewport.Height < 0) mainViewport.Height = 0;

				hScrollBar.LargeChange = bounds.Width;
				vScrollBar.LargeChange = bounds.Height;

				foreach (var p in this.Viewports)
				{
					p.UpdateView();
				}

#if DEBUG
				sw.Stop();
				long ms = sw.ElapsedMilliseconds;
				if (ms > 0)
				{
					Debug.WriteLine("update viewport bounds takes " + ms + " ms.");
				}
#endif
				UpdateVisibleRegion();
			}
			internal override void UpdateVisibleRegion()
			{
				using (Graphics g = Graphics.FromHwnd(grid.Handle))
				{
					RectangleF viewBounds = new RectangleF(mainViewport.ViewLeft, mainViewport.ViewTop,
						mainViewport.Width, mainViewport.Height);

					var oldVisible = mainVisibleRegion;
					mainVisibleRegion = GetVisibleRegion(viewBounds, mainVisibleRegion);
					UpdateNewVisibleRegionTexts(g, mainVisibleRegion, oldVisible);

					columnHeadPart.VisibleRegion = mainVisibleRegion;
					rowHeadPart.VisibleRegion = mainVisibleRegion;
					mainViewport.VisibleRegion = mainVisibleRegion;

					if (freezePos.Row > 0 || freezePos.Col > 0)
					{
						RectangleF freezeViewBounds = new RectangleF(freezeLeftTopViewport.ViewLeft,
							freezeLeftTopViewport.ViewTop, freezeLeftTopViewport.Width, freezeLeftTopViewport.Height);

						oldVisible = frozenVisibleRegion;
						frozenVisibleRegion = GetVisibleRegion(freezeViewBounds, frozenVisibleRegion);
						UpdateNewVisibleRegionTexts(g, frozenVisibleRegion, oldVisible);

						freezeColumnHead.VisibleRegion = frozenVisibleRegion;
						freezeRowHead.VisibleRegion = frozenVisibleRegion;
						freezeLeftTopViewport.VisibleRegion = frozenVisibleRegion;

						oldVisible = freezeLeftViewport.VisibleRegion;
						freezeLeftViewport.VisibleRegion = new GridRegion(mainVisibleRegion.startRow,
							frozenVisibleRegion.startCol, mainVisibleRegion.endRow, frozenVisibleRegion.endCol);
						UpdateNewVisibleRegionTexts(g, freezeLeftViewport.VisibleRegion, oldVisible);

						oldVisible = freezeTopViewport.VisibleRegion;
						freezeTopViewport.VisibleRegion = new GridRegion(frozenVisibleRegion.startRow,
							mainVisibleRegion.startCol, frozenVisibleRegion.endRow, mainVisibleRegion.endCol);

						UpdateNewVisibleRegionTexts(g, freezeTopViewport.VisibleRegion, oldVisible);
					}
				}
			}
			// TODO: Need performance improvement
			private void UpdateNewVisibleRegionTexts(Graphics g, GridRegion region, GridRegion oldVisibleRegion)
			{
#if DEBUG
				Stopwatch sw = Stopwatch.StartNew();
#endif

				// TODO: Need performance improvement
				//       do not perform this during visible region updating
				//
				// end of visible region updating
				grid.IterateCells(region.startRow, region.startCol, region.Rows, region.Cols,
					//(r, c) => true, //!oldVisibleRegion.Contains(r, c), 
					(r, c, cell) => 
					{
						if (cell.RenderScaleFactor != grid.scaleFactor
							&& !string.IsNullOrEmpty(cell.Display))
						{
							grid.UpdateCellFont(cell, g, false);
							cell.RenderScaleFactor = grid.scaleFactor;
						}
						return true;
					});
				//for (int rr = region.startRow; rr <= region.endRow; rr++)
				//{
				//	for (int cc = region.startCol; cc <= region.endCol; )
				//	{
				//		if (!oldVisibleRegion.Contains(rr, cc))
				//		{
				//			ReoGridCell cell = grid.cells[rr, cc];
				//			if (cell != null && cell.Rowspan >= 1 && cell.Colspan >= 1 && !string.IsNullOrEmpty(cell.Display))
				//			{
				//				grid.UpdateCellFont(cell, false);
				//				cc += cell.Colspan;
				//			}
				//			else
				//				cc++;
				//		}
				//		else
				//			cc++;
				//	}
				//}
#if DEBUG
				sw.Stop();
				long ms = sw.ElapsedMilliseconds;

				if (ms > 10)
				{
					Debug.WriteLine("update new visible region text takes " + ms + " ms.");
				}
#endif
			}
			#endregion // Update

			protected override void Resize()
			{
				UpdateBounds();
			}

			internal protected override void Scale(float scaleFactor)
			{
				foreach (var part in this.Viewports)
				{
					part.ScaleFactor = scaleFactor;
				}

				UpdateController();
			}

			internal override void UpdateController()
			{
				// TODO: need fix this: adjust row height in below of freeze-row will cause vscrollbar reset to 0
				if (freezePos.Row > 0 || freezePos.Col > 0)
				{
					Freeze(freezePos);
				}
				else
				{
					base.UpdateController();
				}

				UpdateSchoolBarSize();

				grid.InvalidateCanvas();
			}

			#region Scroll
			private void OnHorScroll(object sender, ScrollEventArgs e)
			{
				if (mainViewport.ViewLeft != e.NewValue)
				{
					base.ScrollViews(ScrollDirection.Horizontal, e.NewValue - mainViewport.ViewLeft, 0);
				}
			}
			private void OnVerScroll(object sender, ScrollEventArgs e)
			{
				if (mainViewport.ViewTop != e.NewValue)
				{
					base.ScrollViews(ScrollDirection.Vertical, 0, e.NewValue - mainViewport.ViewTop);
				}
			}

			internal override void ScrollViews(ScrollDirection dir, float left, float top)
			{
				float x = left, y = top;

				if ((dir & ScrollDirection.Horizontal) == ScrollDirection.Horizontal)
				{
					if (hScrollBar.Value + x > hScrollBar.Maximum - hScrollBar.LargeChange)
						x = hScrollBar.Maximum - hScrollBar.LargeChange - hScrollBar.Value;
					if (hScrollBar.Value + x < hScrollBar.Minimum)
						x = hScrollBar.Minimum - hScrollBar.Value;
				}

				if ((dir & ScrollDirection.Vertical) == ScrollDirection.Vertical)
				{
					if (vScrollBar.Value + y > vScrollBar.Maximum - vScrollBar.LargeChange)
						y = vScrollBar.Maximum - vScrollBar.LargeChange - vScrollBar.Value;
					if (vScrollBar.Value + y < vScrollBar.Minimum)
						y = vScrollBar.Minimum - vScrollBar.Value;
				}

				hScrollBar.Value += (int)Math.Round(x);
				vScrollBar.Value += (int)Math.Round(y);
				base.ScrollViews(dir, x, y);

			}
			#endregion // Scroll

			#region Freeze
			private ColumnHeaderPart freezeColumnHead = null;
			private RowHeaderPart freezeRowHead = null;
			private CellsViewport freezeLeftTopViewport = null;
			private CellsViewport freezeTopViewport = null;
			private CellsViewport freezeLeftViewport = null;

			private ReoGridPos freezePos = new ReoGridPos(0, 0);

			public ReoGridPos FreezePos
			{
				get { return freezePos; }
				set { freezePos = value; }
			}

			internal void Freeze(ReoGridPos pos)
			{
				Freeze(pos.Row, pos.Col);
			}

			public void Freeze(int row, int col)
			{
				this.freezePos.Row = row;
				this.freezePos.Col = col;

				if (row == 0 && col == 0)
				{
					if (freezeLeftTopViewport != null) freezeLeftTopViewport.Visible = false;
					if (freezeLeftViewport != null) freezeLeftViewport.Visible = false;
					if (freezeTopViewport != null) freezeTopViewport.Visible = false;
					if (freezeRowHead != null) freezeRowHead.Visible = false;
					if (freezeColumnHead != null) freezeColumnHead.Visible = false;
					if (freezeRowOutlinePart != null) freezeRowOutlinePart.Visible = false;
					if (freezeColOutlinePart != null) freezeColOutlinePart.Visible = false;
				}

				Rectangle gridLoc = grid.GetGridBounds(row, col);

				mainViewport.ViewLeft = gridLoc.X;
				mainViewport.ViewTop = gridLoc.Y;

				if (row >= 1 || col >= 1)
				{
					// frozen

					if (freezeTopViewport == null)
					{
						AddPart(freezeTopViewport = new CellsViewport(grid));
						freezeTopViewport.ScrollableDirections = ScrollDirection.Horizontal;
					}

					if (freezeLeftViewport == null)
					{
						AddPart(freezeLeftViewport = new CellsViewport(grid));
						freezeLeftViewport.ScrollableDirections = ScrollDirection.Vertical;
					}

					if (freezeRowOutlinePart == null && rowOutlinePart != null)
					{
						InsertPart(0, freezeRowOutlinePart = new RowOutlinePart(grid));
					}
					if (freezeColOutlinePart == null && colOutlinePart != null)
					{
						InsertPart(0, freezeColOutlinePart = new ColumnOutlinePart(grid));
					}

					if (freezeColumnHead == null) InsertPart(0, freezeColumnHead = new ColumnHeaderPart(grid));
					if (freezeRowHead == null) InsertPart(0, freezeRowHead = new RowHeaderPart(grid));
					if (freezeLeftTopViewport == null) InsertPart(0, freezeLeftTopViewport = new CellsViewport(grid));

					// TODO: reserved: always start at (0,0)
					//freezeLeftTopViewport.ViewStart = new PointF(0, 0);
					//freezeColumnHead.ViewStart = new PointF(0, 0);
					//freezeRowHead.ViewStart = new PointF(0, 0);

					freezeTopViewport.ViewStart = new PointF(gridLoc.X, 0);
					freezeLeftViewport.ViewStart = new PointF(0, gridLoc.Y);

					freezeLeftTopViewport.Visible =
						freezeLeftViewport.Visible =
						freezeTopViewport.Visible = true;

					freezeRowHead.Visible = rowHeadPart.Visible;
					freezeColumnHead.Visible = columnHeadPart.Visible;

					if (freezeColOutlinePart != null) freezeColOutlinePart.Visible = colOutlinePart.Visible;
					if (freezeRowOutlinePart != null) freezeRowOutlinePart.Visible = rowOutlinePart.Visible;
				}

				columnHeadPart.ViewLeft = gridLoc.X;
				rowHeadPart.ViewTop = gridLoc.Y;

				if (rowOutlinePart != null) rowOutlinePart.ViewTop = gridLoc.Y;
				if (colOutlinePart != null) colOutlinePart.ViewLeft = gridLoc.X;

				hScrollBar.Value = hScrollBar.Minimum = gridLoc.X;
				vScrollBar.Value = vScrollBar.Minimum = gridLoc.Y;

				UpdateSchoolBarSize();

				int hlc = (int)(Width - gridLoc.X);
				if (hlc < 0) hlc = 0;
				int vlc = (int)(Height - gridLoc.Y);
				if (vlc < 0) vlc = 0;
				hScrollBar.LargeChange = hlc;
				vScrollBar.LargeChange = vlc;


				UpdateBounds();
				UpdateSchoolBarSize();
			}

			private void UpdateSchoolBarSize()
			{
				float width = 0;
				if (grid.cols.Count > 0)
				{
					int viewLeft = grid.cols[mainVisibleRegion.startCol].Left;
					int viewRight = grid.cols[mainVisibleRegion.endCol].Right;
					float scaledWidth = (viewRight - viewLeft) * grid.scaleFactor;
					int unscaledWidth = grid.cols[grid.ColCount - 1].Right - viewRight;
					
					width = viewLeft + scaledWidth + unscaledWidth;
				}

				float height = 0;

				if (grid.rows.Count > 0)
				{
					int viewTop = grid.rows[mainVisibleRegion.startRow].Top;
					int viewBottom = grid.rows[mainVisibleRegion.endRow].Bottom;
					float scaledHeight = (viewBottom - viewTop) * grid.scaleFactor;
					int unscaledHeight = grid.rows[grid.RowCount - 1].Bottom - viewBottom;

					height = viewTop + scaledHeight + unscaledHeight;
				}

				hScrollBar.Maximum = (int)((width + mainViewport.Left + 200));
				vScrollBar.Maximum = (int)((height + mainViewport.Top + 200));
			}

			#endregion

			#region Outline
			private RowOutlinePart rowOutlinePart;
			private RowOutlinePart freezeRowOutlinePart;
			private RowOutlineHeadPart rowOutlineHeadPart;
			private ColumnOutlinePart colOutlinePart;
			private ColumnOutlinePart freezeColOutlinePart;
			private ColumnOutlineHeadPart colOutlineHeadPart;
			private OutlineLeftTopSpace outlineLeftTopSpace;
			#endregion

			#region Draw
			internal override void Draw(RGDrawingContext dc)
			{
				base.Draw(dc);

				Graphics g = dc.Graphics;

				if (freezePos.Col > 0) g.DrawLine(Pens.Gray, freezeLeftViewport.Right, bounds.Top, freezeLeftViewport.Right, bounds.Bottom);
				if (freezePos.Row > 0) g.DrawLine(Pens.Gray, bounds.Left, freezeTopViewport.Bottom, bounds.Right, freezeTopViewport.Bottom);

				//using (Pen p = new Pen(Color.Blue))
				//{
				//	p.DashStyle = DashStyle.Dot;
				//	g.DrawRectangle(p, Rectangle.Round(topFreezeViewport.Bounds));

				//	p.Color = Color.Red;
				//	g.DrawRectangle(p, Rectangle.Round(leftFreezeViewport.Bounds));

				//	p.Color = Color.Gold;
				//	g.DrawRectangle(p, Rectangle.Round(leftTopFreezeViewport.Bounds));
				//}
			}
			#endregion

			public override void Reset()
			{
				mainVisibleRegion = GridRegion.Empty;
				frozenVisibleRegion = GridRegion.Empty;
			}
		}
		#endregion

		#region PageLayout ViewportController
		private class PageLayoutViewportController : AbstractViewportController
		{
			public PageLayoutViewportController(ReoGridControl grid, Rectangle bounds) : base(grid) { }

			internal override void UpdateBounds()
			{
				throw new NotImplementedException();
			}

			protected override void Resize()
			{
				throw new NotImplementedException();
			}

			protected internal override void Scale(float scaleFactor)
			{
				throw new NotImplementedException();
			}
		}
		#endregion

		#region Viewport Controller Management
		private AbstractViewportController viewportController;

		internal AbstractViewportController ViewportController
		{
			get { return viewportController; }
			set { viewportController = value; }
		}

		internal void UpdateViewportController()
		{
			this.rowHeaderWidth = (this.rows.Count >= 100000) ? 50 : 40;

			if (viewportController != null)
			{
#if DEBUG
				Stopwatch sw = Stopwatch.StartNew();
#endif

				this.viewportController.UpdateController();

#if DEBUG
				sw.Stop();
				long ms = sw.ElapsedMilliseconds;
				if (ms > 15)
				{
					Debug.WriteLine("update viewport controller takes " + sw.ElapsedMilliseconds + " ms.");
				}
#endif
			}
		}
		#endregion

		#endregion // Viewport Controller


		#endregion // Viewport & Viewport Controller

		protected override void OnResize(EventArgs e)
		{
			UpdateCanvasBounds();
			base.OnResize(e);
		}

		protected override void OnPaint(PaintEventArgs e)
		{
#if DEBUG
			Stopwatch watch = Stopwatch.StartNew();
#endif

			RGDrawingContext dc = new RGDrawingContext(this, DrawMode.View, e.Graphics, e.ClipRectangle);
			viewportController.Draw(dc);

#if DEBUG
			watch.Stop();
			long ms = watch.ElapsedMilliseconds;
			if (ms > 30)
			{
				Debug.WriteLine(string.Format("end draw: {0} ms. clip: {1}", watch.ElapsedMilliseconds, e.ClipRectangle));
			}
#endif
		}

		// reserved
		//private bool isLeadHeadHover = false;
		private bool isLeadHeadSelected = false;

		public void InvalidateSheet()
		{
			InvalidateCanvas();
		}

		/// <summary>
		/// Invalidate control force to repaint entire UI region
		/// </summary>
		internal void InvalidateCanvas()
		{
			Invalidate();
		}

		internal void InvalidateCanvas(Rectangle clipRegion)
		{
			Invalidate(clipRegion);
		}

		public void InvalidateCell(int row, int col)
		{
			InvalidateCell(new ReoGridPos(row, col));
		}

		/// <summary>
		/// Invalidate control force to repaint specified region by position of cell
		/// </summary>
		/// <param name="pos">region of position will be invalidated</param>
		public void InvalidateCell(ReoGridPos pos)
		{
			if (pos.IsEmpty) return;
			pos = FixPos(pos);
			
			// TODO: invalidate only one cell must to find it over all views 
			//Rectangle cell = GetCellBounds(pos);
			//cell.X += (int)Math.Round(viewportController.ActiveView.Left);
			//cell.Y += (int)Math.Round(viewportController.ActiveView.Top);
			//InvalidateCanvas(cell);

			InvalidateCanvas();
		}
		/// <summary>
		/// Invalidate control force to repaint specified range
		/// </summary>
		/// <param name="range">region of range will be invalidated</param>
		public void InvalidateRange(ReoGridRange range)
		{
			if (range.IsEmpty) return;
			InvalidateCanvas(GetRangeBounds(range));
		}
		/// <summary>
		/// Invalidate control force to repaint specified region by cell
		/// </summary>
		/// <param name="cell">region of cell will be invalidated</param>
		public void InvalidateCell(ReoGridCell cell)
		{
			if (cell == null) return;
			InvalidateCell(cell.Pos);
		}

		/// <summary>
		/// Get position of cell by location of cursor
		/// </summary>
		/// <param name="p"></param>
		/// <returns></returns>
		public ReoGridPos GetCellPosByPoint(PointF p)
		{
			return viewportController.GetPosByPoint(p);
		}

		#endregion // View & Draw

		#region Print

		private CellsViewport printViewport = null;

		/// <summary>
		/// Create instance for PrintDocument. 
		/// </summary>
		/// <returns></returns>
		public PrintDocument CreatePrintDocument()
		{
			PrintDocument doc = new PrintDocument();

			doc.PrintPage += new PrintPageEventHandler(doc_PrintPage);
			doc.BeginPrint += new PrintEventHandler(doc_BeginPrint);

			return doc;
		}

		private List<Rectangle> pageBounds = new List<Rectangle>();

		void doc_BeginPrint(object sender, PrintEventArgs e)
		{
			Rectangle bounds = GetContentBounds();
			pageBounds.Clear();
		}

		private Rectangle GetContentBounds()
		{
			int rows, cols;

#if DEBUG
			Stopwatch stop = Stopwatch.StartNew();
#endif
			cells.FindContentBounds(out rows, out cols);
#if DEBUG
			stop.Stop();
			Debug.WriteLine("Find Content Bound = " + stop.ElapsedMilliseconds + "ms.");
#endif

			// todo: bounds before 0 row or 0 col
			return new Rectangle(0, 0, rows, cols);
		}

		void doc_PrintPage(object sender, PrintPageEventArgs e)
		{
			Graphics g = e.Graphics;

			// TODO: perhaps no need this
			IterateCells(0, 0, rows.Count, cols.Count, (r, c, cell) =>
			{
				UpdateCellTextBounds(g, cell, false);
				return true;
			});

			// TODO: apply to paper size
			if (printViewport == null)
			{
				printViewport = new CellsViewport(this);
			}

			printViewport.Bounds = new Rectangle(0, 0, 1000, 1000);
			printViewport.VisibleRegion = new GridRegion(0, 0, this.rows.Count - 1, this.cols.Count - 1);

			RGDrawingContext dc = new RGDrawingContext(this, DrawMode.Print, g, new Rectangle(0, 0, 1000, 1000));
			printViewport.DrawContent(dc);
		}

		private System.Drawing.Printing.PageSettings pageSettings;

		#endregion

		#region Freeze
		/// <summary>
		/// Freeze grid at specified position
		/// </summary>
		/// <param name="pos">position to be freezeed</param>
		public void FreezeToCell(ReoGridPos pos)
		{
			FreezeToCell(pos.Row, pos.Col);
		}

		/// <summary>
		/// Freeze grid at specified index of row and column
		/// </summary>
		/// <param name="row">index of row</param>
		/// <param name="col">index of column</param>
		public void FreezeToCell(int row, int col)
		{
			if(viewportController is IFreezableViewportController)
			{
				((IFreezableViewportController)viewportController).Freeze(row, col);
				InvalidateCanvas();
			}
			else
			{
				throw new FreezeUnsupportedException();
			}
		}

		/// <summary>
		/// Unfreeze current grid
		/// </summary>
		public void Unfreeze()
		{
			FreezeToCell(0, 0);
		}

		/// <summary>
		/// Check whether current grid can be freezed.
		/// </summary>
		/// <returns></returns>
		public bool CanFreeze()
		{
			if (viewportController is NormalViewportController)
			{
				NormalViewportController vc = ((NormalViewportController)viewportController);
				return vc.FreezePos.Row == 0 && vc.FreezePos.Col == 0;
			}
			else return false;
		}

		/// <summary>
		/// Get current freeze position
		/// </summary>
		/// <returns>the position indicates freeze rows and columns</returns>
		public ReoGridPos GetFreezePos()
		{
			if (viewportController is NormalViewportController)
			{
				NormalViewportController vc = ((NormalViewportController)viewportController);
				return vc.FreezePos;
			}
			else
			{
				return ReoGridPos.Empty;
			}
		}
		#endregion

		#region Group & Outline
		private Dictionary<RowOrColumn, OutlineCollection<ReoGridOutline>> outlines;

		/// <summary>
		/// Retrieve the attached outlines from spreadsheet
		/// </summary>
		/// <param name="flag">Row or column to be retrieved</param>
		/// <returns>Retrieved collection of outline</returns>
		public OutlineCollection<ReoGridOutline> GetOutlines(RowOrColumn flag)
		{
			return outlines == null ? null : outlines[flag];
		}

		/// <summary>
		/// Iterate over all attached outlines
		/// </summary>
		/// <param name="flag">Spcifiy that row or column to be iterated</param>
		/// <param name="iterator">Iterator to handle all outlines</param>
		public void IterateOutlines(RowOrColumn flag, Func<OutlineGroup<ReoGridOutline>, ReoGridOutline, bool> iterator)
		{
			if (iterator == null)
			{
				throw new ArgumentNullException("iterator", "iterator cannot be null");
			}

			OutlineCollection<ReoGridOutline> outlines = null;

			if (this.outlines != null 
				&& this.outlines.TryGetValue(flag, out outlines))
			{
				foreach (var group in outlines)
				{
					foreach (var outline in group)
					{
						if (!iterator(group, outline)) return;
					}
				}
			}
		}

		private static void InsertOutline(OutlineCollection<ReoGridOutline> outlineGroups, int groupIndex, ReoGridOutline outline)
		{
			int k = groupIndex;

			if (k < 0)
			{
				outlineGroups.Insert(0, new OutlineGroup<ReoGridOutline>() { outline });
				return;
			}

			var group = outlineGroups[k];

			for (int i = 0; i < group.Count; i++)
			{
				var o = group[i];

				if (o.Contains(outline))
				{
					group.RemoveAt(i);

					InsertOutline(outlineGroups, k - 1, o);
					break;
				}
			}

			group.Add(outline);
		}

		/// <summary>
		/// Add outline (Group rows) from specified start position
		/// </summary>
		/// <param name="flag">Which row and column outline to be added</param>
		/// <param name="start">Start position of outline</param>
		/// <param name="count">Total count of outline</param>
		/// <returns>An outline instance created for this outline</returns>
		public ReoGridOutline AddOutline(RowOrColumn flag, int start, int count)
		{
			int limit = ((flag == RowOrColumn.Row) ? this.rows.Count : this.cols.Count) - 1;

			if (start < 0 || start >= limit)
			{
				throw new OutlineOutOfRangeException(start, count, "row");
			}

			if (count < 0 || (start + count > limit))
			{
				throw new OutlineOutOfRangeException(start, count, "count");
			}

			if (count == 0) return null;

			if (this.outlines == null)
			{
				this.outlines = new Dictionary<RowOrColumn, OutlineCollection<ReoGridOutline>>() {
					{ RowOrColumn.Row, new OutlineCollection<ReoGridOutline>() },
					{ RowOrColumn.Column, new OutlineCollection<ReoGridOutline>() },
				};
			}

			var outlineGroups = this.outlines[flag];

			if (outlineGroups.Count >= 9)
			{
				throw new OutlineTooMuchException();
			}

			int endrow = start + count;

			int targetGroupIndex = outlineGroups.Count - 2; // last group in master

			// create new outline
			ReoGridOutline newOutline = null;
			bool found = false;

			if (flag == RowOrColumn.Row)
			{
				newOutline = new RowOutline(this, start, count);
			}
			else
			{
				newOutline = new ColumnOutline(this, start, count);
			}

			for (int k = 0; k < outlineGroups.Count - 1; k++)
			{
				var group = outlineGroups[k];

				foreach (var o in group)
				{
					if (start == o.Start && endrow == o.End)
					{
						throw new OutlineAlreadyDefinedException();
					}
					else if (newOutline.IntersectWith(o))
					{
						throw new OutlineIntersectedException
						{
							Start = o.Start,
							Count = o.Count,
						};
					}
					// find surrounded outline 
					else if (newOutline.Contains(o))
					{
						targetGroupIndex = k - 1;
						found = true;
					}
				}

				if (found) break;
			}

			// insert outline
			InsertOutline(outlineGroups, targetGroupIndex, newOutline);

			if (flag == RowOrColumn.Row)
			{
				viewportController.SetHeadVisible(VisibleViews.RowOutline, true);
			}
			else
			{
				viewportController.SetHeadVisible(VisibleViews.ColOutline, true);
			}

			this.InvalidateCanvas();

			return newOutline;
		}

		/// <summary>
		/// Retrieve outline by specified position
		/// </summary>
		/// <param name="flag">which row and column to be retrieved</param>
		/// <param name="start">Start position of outline</param>
		/// <param name="count">Total count of outline</param>
		/// <returns></returns>
		public ReoGridOutline GetOutline(RowOrColumn flag, int start, int count)
		{
			ReoGridOutline outline = null;

			var outlines = this.outlines[flag];

			foreach (var g in outlines)
			{
				outline = g.FirstOrDefault(o => o.Start == start && o.Count == count);
				if (outline != null) break;
			}
			
			if (outline == null)
			{
				throw new OutlineNotFoundException(start,
					string.Format("Outline not found at specified position {0}-{1}", start, start + count));
			}

			return outline;
		}

		/// <summary>
		/// Collapse specified outline
		/// </summary>
		/// <param name="flag">Which row and column outline to be collapsed</param>
		/// <param name="start">Start position of outline</param>
		/// <param name="count">Total count of outline</param>
		/// <returns></returns>
		public ReoGridOutline CollapseOutline(RowOrColumn flag, int start, int count)
		{
			var outline = GetOutline(flag, start, count);
			outline.Collapse();
			return outline;
		}

		/// <summary>
		/// Expand specified outline
		/// </summary>
		/// <param name="flag">which row and column outline to be expanded</param>
		/// <param name="start">Start position of outline</param>
		/// <param name="count">Total count of outline</param>
		/// <returns></returns>
		public ReoGridOutline ExpandOutline(RowOrColumn flag, int start, int count)
		{
			var outline = GetOutline(flag, start, count);
			outline.Expand();
			return outline;
		}

		/// <summary>
		/// Remove specfieid outline from collection of outlines of control
		/// </summary>
		/// <param name="flag">Which row and column to be removed</param>
		/// <param name="outline">The instance of outline will be removed</param>
		/// <returns>True if the outline existed, and removed successfully</returns>
		public bool RemoveOutline(RowOrColumn flag, IReoGridOutline outline)
		{
			return RemoveOutline(flag, outline.Start, outline.Count);
		}

		private static void RearrangementOutline(OutlineCollection<ReoGridOutline> outlineGroups, int startGroup)
		{
			if (startGroup >= outlineGroups.Count - 1 || startGroup < 1) return;

			// search for contained childrens, bring it into this level
			int k = startGroup;

			var rightGroup = outlineGroups[k];

		reloop:
			for (int i = 0; i < rightGroup.Count; i++)
			{
				var or = rightGroup[i];

				var leftGroup = outlineGroups[k - 1];

				// check for intersected
				var leftOverlap = leftGroup.FirstOrDefault(_o => _o.Start <= or.Start && _o.Count >= or.Start
					|| _o.Start <= or.End && _o.End >= or.End);

				// no intersects
				if (leftOverlap == null)
				{
					rightGroup.RemoveAt(i);
					leftGroup.Add(or);

					// all children outlines has been removed
					if (rightGroup.Count == 0)
					{
						// remove this group
						outlineGroups.RemoveAt(k);

						RearrangementOutline(outlineGroups, k);
					}
					else
					{
						RearrangementOutline(outlineGroups, k + 1);
					}

					goto reloop;
				}
			}
		}

		/// <summary>
		/// Remove specified outline from an start position
		/// </summary>
		/// <param name="flag">Which row and column to be removed</param>
		/// <param name="start">The start position of outline used to find target outline</param>
		/// <param name="count">True if the outline existed, and removed successfully</param>
		/// <returns></returns>
		public bool RemoveOutline(RowOrColumn flag, int start, int count)
		{
			var outlineGroups = this.GetOutlines(flag);

			if (outlineGroups == null) return false;

			bool found = false;

			for (int i = 0; i < outlineGroups.Count - 1; i++)
			{
				var group = outlineGroups[i];

				for (int j = 0; j < group.Count; j++)
				{
					var o = group[j];

					if (o.Start == start && o.Count == count)
					{
						// remove this outline
						group.Remove(o);

						// rearrangement behind the outlines
						RearrangementOutline(outlineGroups, i + 1);

						if (group.Count == 0)
						{
							outlineGroups.RemoveAt(i);
						}

						found = true;
						break;
					}
				}

				if (found) break;
			}

			if (outlineGroups.Count <= 1)
			{
				this.viewportController.SetHeadVisible(flag == RowOrColumn.Row ?
					VisibleViews.RowOutline : VisibleViews.ColOutline, false);
			}
			else
			{
				UpdateViewportController();
			}

			this.InvalidateCanvas();

			return found;
		}

		/// <summary>
		/// Clear all outlines, and close the outline display panel.
		/// </summary>
		/// <param name="flag">Which outline of row and column to be clear</param>
		public void ClearOutlines(RowOrColumn flag)
		{
			if (this.outlines == null) return;

			VisibleViews hideViews = VisibleViews.None;

			if ((flag & RowOrColumn.Row) == RowOrColumn.Row)
			{
				this.outlines[RowOrColumn.Row].Reset();
				hideViews |= VisibleViews.RowOutline;
			}

			if ((flag & RowOrColumn.Column) == RowOrColumn.Column)
			{
				this.outlines[RowOrColumn.Column].Reset();
				hideViews |= VisibleViews.ColOutline;
			}

			viewportController.SetHeadVisible(hideViews, false);

			this.InvalidateCanvas();
		}

		#endregion

		#region Zoom
		private static readonly float minScaleFactor = 0.1f;
		private static readonly float maxScaleFactor = 4f;
		private float scaleFactor = 1f;

		/// <summary>
		/// Current scale factor 
		/// </summary>
		[DefaultValue(1f)]
		public float ScaleFactor { get { return scaleFactor; } set { SetScale(value, Point.Empty); } }

		/// <summary>
		/// Event raised on grid scaled 
		/// </summary>
		public event EventHandler<EventArgs> GridScaled;

		//private PointF ScalePoint(PointF p)
		//{
		//  return new PointF((float)(p.X / scaleFactor), (float)(p.Y / scaleFactor));
		//}

		/// <summary>
		/// Set scale factor to zoom in/out current grid
		/// </summary>
		/// <param name="factor">factor of scale (0.1f ~ 4f)</param>
		/// <param name="refer">origin point after scaling</param>
		public void SetScale(float factor, Point refer)
		{
			if (currentEditingCell != null)
			{
				EndEdit(ReoGridEndEditReason.NormalFinish);
			}

			factor = (float)Math.Round(factor, 1);

			if (factor < minScaleFactor) factor = minScaleFactor;
			if (factor > maxScaleFactor) factor = maxScaleFactor;

			if (this.scaleFactor != factor)
			{
				this.scaleFactor = factor;

				viewportController.Scale(factor);

				if (GridScaled != null) GridScaled(this, new EventArgs());
			}
		}

		/// <summary>
		/// Zoom in current grid
		/// </summary>
		public void ZoomIn()
		{
			SetScale(scaleFactor + 0.1f, Point.Empty);
		}

		/// <summary>
		/// Zoom out current grid
		/// </summary>
		public void ZoomOut()
		{
			SetScale(scaleFactor - 0.1f, Point.Empty);
		}

		/// <summary>
		/// Set scale factor to 1f to restore current grid
		/// </summary>
		public void ZoomReset()
		{
			SetScale(1f, Point.Empty);
		}
		#endregion

		#region Merge

		/// <summary>
		/// Check are there any merged range exist in specified range
		/// </summary>
		/// <param name="range">range to be checked</param>
		/// <returns>the intersected range with specified range</returns>
		public ReoGridRange CheckIntersectedMergingRange(ReoGridRange range)
		{
			ReoGridRange intersectedRange = ReoGridRange.Empty;

			cells.Iterate(range.Row, range.Col, range.Rows, range.Cols, true, (r, c, cell) =>
			{
				if (!cell.MergeStartPos.IsEmpty)
				{
					ReoGridCell checkStartCell = GetCell(cell.MergeStartPos);
					for (int rr = checkStartCell.Row; rr <= checkStartCell.MergeEndPos.Row; rr++)
					{
						for (int cc = checkStartCell.Col; cc <= checkStartCell.MergeEndPos.Col; cc++)
						{
							var targetCell = cells[rr, cc];
							if (targetCell != null && !range.Contains(targetCell.Pos))
							{
								intersectedRange = new ReoGridRange(checkStartCell.Pos, checkStartCell.MergeEndPos);
								break;
							}
						}

						if (!intersectedRange.IsEmpty) break;
					}

					return intersectedRange.IsEmpty ? 0 : cell.Colspan;
				}
				return cell.Colspan < 1 ? 1 : cell.Colspan;
			});

			return intersectedRange;
		}

		/// <summary>
		/// Check are there any merged range exist in specified range
		/// </summary>
		/// <param name="range">range to be checked</param>
		/// <returns>true if specified range can be merged</returns>
		public bool HasIntersectedMergingRange(ReoGridRange range)
		{
			return !CheckIntersectedMergingRange(range).IsEmpty;
		}

		/// <summary>
		/// Merge specified range into single cell
		/// </summary>
		/// <param name="row">number of start row</param>
		/// <param name="col">number of start column</param>
		/// <param name="rows">number of rows to be merged</param>
		/// <param name="cols">number of columns to be merged</param>
		public void MergeRange(int row, int col, int rows, int cols)
		{
			MergeRange(new ReoGridRange(row, col, rows, cols));
		}
		/// <summary>
		/// Merge specified range into single cell
		/// </summary>
		/// <exception cref="RangeTooSmallException">thrown when specified range has only one cell.</exception>
		/// <exception cref="RangeIntersectionException">thrown when specified range intersectes with another one. </exception>
		/// <param name="range">Range to be merged</param>
		/// <seealso cref="UnmergeRange"/>
		public void MergeRange(ReoGridRange range)
		{
			MergeRange(range, true);
		}
		internal void MergeRange(ReoGridRange range, bool checkIntersection)
		{
			if (range.IsEmpty) return;

			ReoGridRange fixedRange = FixRange(range);

			if (fixedRange.Cols <= 1 && fixedRange.Rows <= 1)
			{
				return;
			}

			if (checkIntersection)
			{
				ReoGridRange intersectedRange = CheckIntersectedMergingRange(fixedRange);
				if (!intersectedRange.IsEmpty)
				{
					throw new RangeIntersectionException(intersectedRange);
				}
			}
				
			int row = fixedRange.Row;
			int col = fixedRange.Col;
			int torow = fixedRange.Row2;
			int tocol = fixedRange.Col2;

			// find start and end cell
			ReoGridCell startCell = cells[row, col];
			ReoGridCell endCell = cells[torow, tocol];

			if (startCell == null) startCell = CreateCell(row, col);
			if (endCell == null) endCell = CreateCell(torow, tocol);

			startCell.Rowspan = (short)fixedRange.Rows;
			startCell.Colspan = (short)fixedRange.Cols;

			UpdateCellBounds(startCell);

			for (int r = row; r <= torow; r++)
			{
				for (int c = col; c <= tocol; c++)
				{
					ReoGridCell cell = CreateAndGetCell(r, c);

					// reference to start and end pos
					cell.MergeStartPos = startCell.Pos;
					cell.MergeEndPos = endCell.Pos;

					// close col and row span
					cell.Colspan = 0;
					cell.Rowspan = 0;

					// apply text to merged start cell
					if (cell != startCell) cell.Data = cell.Display = null;

					if (r == row)
					{
						if (c > col) CutBeforeVBorder(r, c);
					}
					else
					{
						hBorders[r, c] = new ReoGridHBorder { Cols = 0 };
					}

					if (c == col)
					{
						if (r > row) CutBeforeHBorder(r, c);
					}
					else
					{
						vBorders[r, c] = new ReoGridVBorder { Rows = 0 };
					}
				}
			}

			startCell.Rowspan = (short)fixedRange.Rows;
			startCell.Colspan = (short)fixedRange.Cols;

			SelectRange(selectionRange);
			InvalidateCanvas();

			if (RangeMerged != null)
			{
				RangeMerged(this, new RGRangeEventArgs(fixedRange));
			}
		}

		/// <summary>
		/// Unmerge all cells contained in the specified range.
		/// </summary>
		/// <seealso cref="MergeRange"/>
		/// <param name="range">Range to be checked and all cells in this range will be unmerged.</param>
		public void UnmergeRange(ReoGridRange range)
		{
			if (range.IsEmpty) return;

			range = FixRange(range);
			
			int row = range.Row;
			int col = range.Col;
			int torow = range.Row + range.Rows - 1;
			int tocol = range.Col + range.Cols - 1;

			for (int r = row; r <= torow; r++)
			{
				for (int c = col; c <= tocol; c++)
				{
					ReoGridCell cell = CreateAndGetCell(r, c);

					if (cell.Colspan > 1 || cell.Rowspan > 1)
					{
						UnmergeCell(cell);
						c += cell.Colspan;
					}
				}
			}

			InvalidateCanvas();
		}
		private void UnmergeCell(ReoGridCell source)
		{
			ReoGridRangeStyle style = source.Style;
			int r2 = source.Row + source.Rowspan;
			int c2 = source.Col + source.Colspan;

			ReoGridRange range = new ReoGridRange(source.Row, source.Col, source.Rowspan, source.Colspan);

			for (int r = source.Row; r < r2; r++)
			{
				for (int c = source.Col; c < c2; c++)
				{
					ReoGridCell cell = CreateAndGetCell(r, c);

					cell.MergeStartPos = ReoGridPos.Empty;
					cell.MergeEndPos = ReoGridPos.Empty;
					cell.Colspan = 1;
					cell.Rowspan = 1;
					UpdateCellBounds(cell);

					if (r != source.Row) hBorders[r, c] = null;
					if (c != source.Col) vBorders[r, c] = null;

					if (r != source.Row || c != source.Col)
					{
						if (style != null)
						{
							if (cell.Style == null)
								cell.Style = new ReoGridRangeStyle(style);
							else
								cell.Style.CopyFrom(style);
						}
						else if (cell.Style != null)
						{
							cell.Style = null;
						}
					}
				}
			}

			if (RangeUnmerged != null)
			{
				RangeUnmerged(this, new RGRangeEventArgs(range));
			}
		}

		/// <summary>
		/// Event raised when range merged
		/// </summary>
		public event EventHandler<RGRangeEventArgs> RangeMerged;

		/// <summary>
		/// Event raised when range unmerged
		/// </summary>
		public event EventHandler<RGRangeEventArgs> RangeUnmerged;
		#endregion

		#region Cell & Grid Management
		private RegularTreeArray<ReoGridCell> cells = new RegularTreeArray<ReoGridCell>();

		/// <summary>
		/// Get cell at specified position.
		/// 
		/// NOTE: this method will create new cell instance if not exist.
		/// use GetCell if you do not want to create cell instance.
		/// 
		/// </summary>
		/// <param name="pos">position to get cell instance</param>
		/// <returns>cell instance</returns>
		public ReoGridCell CreateAndGetCell(ReoGridPos pos)
		{
			return CreateAndGetCell(pos.Row, pos.Col);
		}

		/// <summary>
		/// Get cell at specified position.
		/// 
		/// NOTE: this method will create new cell instance if not exist.
		/// use GetCell if you do not want to create cell instance.
		/// 
		/// </summary>
		/// <param name="row">row of position</param>
		/// <param name="col">col of position</param>
		/// <returns>cell instance</returns>
		public ReoGridCell CreateAndGetCell(int row, int col)
		{
			ReoGridCell cell = cells[row, col];
			if (cell == null) cell = CreateCell(row, col);
			return cell;
		}

		/// <summary>
		/// Create cell instance at specified position
		/// </summary>
		/// <param name="row">number of row</param>
		/// <param name="col">number of col</param>
		/// <returns>cell instance created at specified position</returns>
		internal ReoGridCell CreateCell(int row, int col)
		{
			if (row < 0 || col < 0
				|| row > this.rows.Count - 1
				|| col > this.cols.Count - 1)
			{
				return null;
			}

			ReoGridCell cell = new ReoGridCell()
			{
				Row = row,
				Col = col,
				Bounds = GetGridBounds(row, col),
				Colspan = 1,
				Rowspan = 1,
				Style = ReoGridStyleUtility.CreateDefaultGridStyle(this, row, col),
				StringFormat = new StringFormat(StringFormat.GenericTypographic),
			};
			cells[row, col] = cell;

			// update render align
			ReoGridStyleUtility.UpdateCellRenderAlign(this, cell);

			// update font of cell
			UpdateCellFont(cell, null, false);

			return cell;
		}

		/// <summary>
		/// Retrieve cell at specified position. 
		/// </summary>
		/// <param name="pos">Position of cell</param>
		/// <returns>Return null if specified position has nothing.</returns>
		public ReoGridCell GetCell(ReoGridPos pos)
		{
			return GetCell(pos.Row, pos.Col);
		}
		/// <summary>
		/// Retrieve cell at specified index of row and column.
		/// </summary>
		/// <param name="row">Index of row</param>
		/// <param name="col">Index of column</param>
		/// <returns>Return null if specified position has nothing.</returns>
		public ReoGridCell GetCell(int row, int col)
		{
			return cells[row, col];
		}

		/// <summary>
		/// Return the merged first cell inside range
		/// </summary>
		/// <param name="pos">Position in range</param>
		/// <returns>First cell of range</returns>
		public ReoGridCell GetMergedCellOfRange(ReoGridPos pos)
		{
			return GetMergedCellOfRange(pos.Row, pos.Col);
		}

		/// <summary>
		/// Return the first cell inside merged range
		/// </summary>
		/// <param name="row">Row of position in range</param>
		/// <param name="col">Column of position in range</param>
		/// <returns></returns>
		public ReoGridCell GetMergedCellOfRange(int row, int col)
		{
			return GetMergedCellOfRange(CreateAndGetCell(row, col));
		}

		/// <summary>
		/// Return the first cell inside merged range
		/// </summary>
		/// <param name="cell">Cell instance in range</param>
		/// <returns>Cell instance of merged range</returns>
		public ReoGridCell GetMergedCellOfRange(ReoGridCell cell)
		{
			if (cell == null) return null;

			if (cell.IsStartMergedCell) return cell;

			return CreateAndGetCell(cell.MergeStartPos);
		}

		/// <summary>
		/// Check whether a cell is merged cell
		/// </summary>
		/// <param name="pos">position to be checked</param>
		/// <returns>true if the cell is merged cell</returns>
		public bool IsMergedCell(ReoGridPos pos)
		{
			return IsMergedCell(pos.Row, pos.Col);
		}

		/// <summary>
		/// Check whether a cell is merged cell
		/// </summary>
		/// <param name="row">number of row to be checked</param>
		/// <param name="col">number of column to be checked</param>
		/// <returns>true if the cell is merged cell</returns>
		public bool IsMergedCell(int row, int col)
		{
			var cell = cells[row, col];
			return cell != null && cell.IsMergedCell;
		}

		/// <summary>
		/// Check whether the specified cell is a valid cell
		/// </summary>
		/// <param name="pos">Position to be checked</param>
		/// <returns>true if specified position is a valid cell</returns>
		public bool IsValidCell(ReoGridPos pos)
		{
			return IsValidCell(pos.Row, pos.Col);
		}

		/// <summary>
		/// Check whether the specified position is a valid cell
		/// </summary>
		/// <param name="row">Row of position to be checked</param>
		/// <param name="col">Column of position to be checked</param>
		/// <returns>true if specified position is valid cell</returns>
		public bool IsValidCell(int row, int col)
		{
			var cell = cells[row, col];
			return cell == null || cell.IsValidCell;
		}

		/// <summary>
		/// Iterate over all existed cells in the specified range.
		/// Nothing in position for cells will be skipped automatically.
		/// </summary>
		/// <param name="range">Specified range to iterate cells</param>
		/// <param name="iterator">Callback iterator used to check all cells. 
		/// Return 'false' from this callback to stop iteration.</param>
		public void IterateCells(ReoGridRange range, Func<int, int, ReoGridCell, bool> iterator)
		{
			var fixedRange = FixRange(range);
			IterateCells(fixedRange.Row, fixedRange.Col, fixedRange.Rows, fixedRange.Cols, iterator);
		}

		/// <summary>
		/// Iterate over all existed cells in the specified range.
		/// No instance of cells will be all skipped.
		/// </summary>
		/// <param name="row">Specified index of row</param>
		/// <param name="col">Specified index of column</param>
		/// <param name="rows">Specified number of rows</param>
		/// <param name="cols">Specified number of columns</param>
		/// <param name="iterator">Callback iterator used to check all cells. 
		public void IterateCells(int row, int col, int rows, int cols, Func<int, int, ReoGridCell, bool> iterator)
		{
			cells.Iterate(row, col, rows, cols, true, (r, c, cell) =>
			{
				int cspan = cell.Colspan;

				if (cspan <= 0) return 1;

				if (!iterator(r, c, cell))
				{
					return 0;
				}

				return (cspan <= 0) ? 1 : cspan;
			});
		}

		internal void IterateCellsEx(int row, int col, int endRow, int endCol, Func<int,int,bool> isTarget, Func<int, int, ReoGridCell, bool> iterator)
		{
			cells.IterateEx(row, col, endRow, endCol, isTarget, (r, c, cell) =>
			{
				int cspan = cell.Colspan;

				if (cspan <= 0) return 1;

				if (!iterator(r, c, cell))
				{
					return 0;
				}

				return (cspan <= 0) ? 1 : cspan;
			});
		}

		public int MaxContentRow
		{
			get
			{
				return cells.MaxRow;
			}
		}

		public int MaxContentCol
		{
			get
			{
				return cells.MaxCol;
			}
		}
		#endregion

		#region Selection
		private ReoGridPos selOriginal = new ReoGridPos(0, 0);
		private ReoGridPos selEnd = ReoGridPos.Empty;

		private ReoGridRange selectionRange = new ReoGridRange(0, 0, 1, 1);
		private ReoGridPos lastSelectionEndPos = ReoGridPos.Empty;

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public ReoGridRange SelectionRange
		{
			get { return selectionRange; }
			set { SelectRange(value); }
		}

		private ReoGridPos focusPos;

		/// <summary>
		/// Get or set current focused cell position
		/// </summary>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public ReoGridPos FocusPos
		{
			get
			{
				return focusPos;
			}
			set
			{
				if (!focusPos.IsEmpty)
				{
					var focusCell = cells[focusPos.Row, focusPos.Col];
					if (focusCell != null && focusCell.body != null && focusCell.IsValidCell)
					{
						focusCell.body.OnLostFocus(focusCell);
					}
				}

				focusPos = value;

				if ( !focusPos.IsEmpty)
				{
					var focusCell = cells[focusPos.Row, focusPos.Col];
					if (focusCell != null && focusCell.body != null && focusCell.IsValidCell)
					{
						focusCell.body.OnGotFocus(focusCell);
					}
				}
		
				//if (selectionRange.Row != value.Row
				//	|| selectionRange.Col != value.Col)
				//{
				//	SelectRange(value.Row, value.Col, 1, 1);
				//}

				if (FocusPosChanged != null)
				{
					FocusPosChanged(this, new RGPositionEventArgs(focusPos));
				}
			}
		}

		public event EventHandler<RGPositionEventArgs> FocusPosChanged;

		private ReoGridPos hoverPos;

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public ReoGridPos HoverPos
		{
			get
			{
				return hoverPos;
			}
			set
			{
				if (hoverPos != value)
				{
					// raise cell mouse enter
					if (!hoverPos.IsEmpty)
					{
						RGCellMouseEventArgs evtArg = null;

						if (CellMouseLeave != null)
						{
							evtArg = new RGCellMouseEventArgs(this, hoverPos);
							CellMouseLeave(this, evtArg);
						}

						var cell = cells[hoverPos.Row, hoverPos.Col];

						if (cell != null)
						{
							if (!cell.IsValidCell)
							{
								cell = GetMergedCellOfRange(cell);
							}

							if (cell.body != null)
							{
								if (evtArg == null)
								{
									evtArg = new RGCellMouseEventArgs(this, cell);
								}

								bool processed = cell.body.OnMouseLeave(evtArg);
								if (processed) InvalidateCell(cell);
							}
						}
					}

					hoverPos = value;

					// raise cell mouse leave
					if (!hoverPos.IsEmpty)
					{
						RGCellMouseEventArgs evtArg = null;

						if(CellMouseEnter != null)
						{
							evtArg = new RGCellMouseEventArgs(this, hoverPos);
							CellMouseEnter(this, evtArg);
						}

						var cell = cells[hoverPos.Row, hoverPos.Col];

						if (cell != null)
						{
							if (!cell.IsValidCell)
							{
								cell = GetMergedCellOfRange(cell);
							}

							if (cell.body != null)
							{
								if (evtArg == null)
								{
									evtArg = new RGCellMouseEventArgs(this, cell);
									evtArg.Cell = cell;
								}

								bool processed = cell.body.OnMouseEnter(evtArg);
								if (processed) InvalidateCell(cell);
							}
						}
					}

					if (HoverPosChanged != null)
					{
						HoverPosChanged(this, new RGPositionEventArgs(hoverPos));
					}
				}
			}
		}

		public event EventHandler<RGPositionEventArgs> HoverPosChanged;

		private ReoGridSelectionMode selectionMode = ReoGridSelectionMode.Range;

		/// <summary>
		/// Selection Mode for Control
		/// </summary>
		[DefaultValue(ReoGridSelectionMode.Range)]
		public ReoGridSelectionMode SelectionMode
		{
			get
			{
				return selectionMode;
			}
			set
			{
				if (selectionMode != value)
				{
					selectionMode = value;

					if (selectionMode == ReoGridSelectionMode.None)
					{
						selectionRange = ReoGridRange.Empty;
						InvalidateCanvas();
					}
					else if (selectionRange.IsEmpty)
					{
						SelectRange(new ReoGridRange(0, 0, 1, 1));
					}

					if (SelectionModeChanged != null)
					{
						SelectionModeChanged(this, null);
					}
				}
			}
		}

		private ReoGridSelectionStyle selectionStyle = ReoGridSelectionStyle.Default;

		/// <summary>
		/// Selection Style for Control
		/// </summary>
		[DefaultValue(ReoGridSelectionStyle.Default)]
		public ReoGridSelectionStyle SelectionStyle
		{
			get
			{
				return selectionStyle;
			}
			set
			{
				if (selectionStyle != value)
				{
					selectionStyle = value;
					InvalidateCanvas();

					if (SelectionStyleChanged != null)
					{
						SelectionStyleChanged(this, null);
					}
				}
			}
		}

		private SelectionForwardDirection selectionForwardDirection;

		/// <summary>
		/// Selection Forward Direction for Control
		/// </summary>
		[DefaultValue(SelectionForwardDirection.Right)]
		public SelectionForwardDirection SelectionForwardDirection
		{
			get
			{
				return this.selectionForwardDirection;
			}
			set
			{
				if (this.selectionForwardDirection != value)
				{
					this.selectionForwardDirection = value;
					if (this.SelectionForwardDirectionChanged != null)
					{
						SelectionForwardDirectionChanged(this, null);
					}
				}
			}
		}

		/// <summary>
		/// Select specified range.
		/// </summary>
		/// <param name="pos1">Start position of specified range</param>
		/// <param name="pos2">End position of specified range</param>
		internal void SelectRangeByPos(ReoGridPos pos1, ReoGridPos pos2)
		{
			SelectRange(new ReoGridRange(pos1, pos2));
		}
		/// <summary>
		/// Select specified range.
		/// </summary>
		/// <param name="row">Start index of row of specified range</param>
		/// <param name="col">Start index of column of specified range</param>
		/// <param name="row2">End index or row of range</param>
		/// <param name="col2">End index or column of range</param>
		internal void SelectRangeByPos(int row, int col, int row2, int col2)
		{
			int minr = Math.Min(row, row2);
			int minc = Math.Min(col, col2);
			int maxr = Math.Max(row, row2);
			int maxc = Math.Max(col, col2);

			if (minr < 0) minr = 0;
			if (minc < 0) minc = 0;
			if (maxr < 0) maxr = 0;
			if (maxc < 0) maxc = 0;

			if (maxr > this.rows.Count - 1) maxr = this.rows.Count - 1;
			if (maxc > this.cols.Count - 1) maxc = this.cols.Count - 1;

			SelectRange(new ReoGridRange(minr, minc, maxr - minr + 1, maxc - minc + 1));
		}
		/// <summary>
		/// Select specified range
		/// </summary>
		/// <param name="row">number of start row</param>
		/// <param name="col">number of start col</param>
		/// <param name="rows">number of rows to be selected</param>
		/// <param name="cols">number of columns to be selected</param>
		public void SelectRange(int row, int col, int rows, int cols)
		{
			SelectRange(new ReoGridRange(row, col, rows, cols));
		}
		/// <summary>
		/// Select specified range.
		/// </summary>
		/// <param name="range">Specified range to be selected</param>
		public void SelectRange(ReoGridRange range)
		{
			if (range.IsEmpty || this.selectionMode == ReoGridSelectionMode.None) return;

#if DEBUG
			Stopwatch stop = Stopwatch.StartNew();
#endif

			if (range.Rows == -1) range.Rows = this.rows.Count;
			if (range.Cols == -1) range.Cols = this.cols.Count;
	
			ReoGridRange fixedRange = FixRange(range);

			int minr = fixedRange.Row;
			int minc = fixedRange.Col;
			int maxr = fixedRange.Row2;
			int maxc = fixedRange.Col2;

			if (selectionMode == ReoGridSelectionMode.Cell)
			{
				maxr = minr = selOriginal.Row;
				maxc = minc = selOriginal.Col;
			}

			ReoGridCell leftTopCell = cells[minr, minc];
			ReoGridCell leftBottomCell = cells[maxr, minc];
			ReoGridCell rightTopCell = cells[minr, maxc];
			ReoGridCell rightBottomCell = cells[maxr, maxc];

			#region Check and select the whole merged region
#if DEBUG
			if (!Toolkit.IsKeyDown(Win32.VKey.VK_CONTROL))
			{
#endif
				//
				// if there any entire rows or columns are selected (full == -1)
				// the selection bounds of merged region will not be checked.
				// any changes to the selection will also not be appiled to the range.
				//
				if (fullRowSelection == -1 && fullColSelection == -1)
				{
					ReoGridRange checkedRange = CheckMergedRange(new ReoGridRange(minr, minc, maxr - minr + 1, maxc - minc + 1));

					minr = checkedRange.Row;
					minc = checkedRange.Col;
					maxr = checkedRange.Row2;
					maxc = checkedRange.Col2;
				}
#if DEBUG
			}
#endif
			#endregion

			int rows = maxr - minr + 1;
			int cols = maxc - minc + 1;

			selectionRange = new ReoGridRange(minr, minc, rows, cols);

			bool fullRowSelected = rows == this.rows.Count;
			bool fullColSelected = cols == this.cols.Count;

			if (!fixedRange.Contains(selOriginal)) selOriginal = fixedRange.StartPos;
			if (!fixedRange.Contains(selEnd)) selEnd = fixedRange.StartPos;

			if (!isRangeSelecting)
			{
				InvalidateCanvas();

				if (SelectionRangeChanged != null)
				{
					SelectionRangeChanged(this, new RGRangeEventArgs(selectionRange));
				}

#if EX_SCRIPT
				RaiseScriptEvent("onselectionchange");
#endif
			}

#if DEBUG
			stop.Stop();
			if (stop.ElapsedMilliseconds > 25)
			{
				Debug.WriteLine("select range takes " + stop.ElapsedMilliseconds + " ms.");
			}
#endif
		}

		/// <summary>
		/// Select entire sheet
		/// </summary>
		public void SelectAll()
		{
			if (IsEditing)
			{
				this.editTextbox.SelectAll();
			}
			else
			{
				SelectRange(new ReoGridRange(0, 0, RowCount, ColCount));
			}
		}

		public void SelectColumns(int col, int columns)
		{
			SelectRange(new ReoGridRange(0, col, this.rows.Count, columns));
		}

		public void SelectRows(int row, int rows)
		{
			SelectRange(new ReoGridRange(row, 0, rows, this.cols.Count));
		}

		/// <summary>
		/// Event raised on focus-selection-range changed
		/// </summary>
		public event EventHandler<RGRangeEventArgs> SelectionRangeChanged;
		/// <summary>
		/// Event raised on Selection-Mode change
		/// </summary>
		public event EventHandler SelectionModeChanged;
		/// <summary>
		/// Event raised on Selection-Style change
		/// </summary>
		public event EventHandler SelectionStyleChanged;
		/// <summary>
		/// Event raised on SelectionForwardDirection change
		/// </summary>
		public event EventHandler SelectionForwardDirectionChanged;
		#endregion // Selection
	
		#region Mouse
		private bool isRangeSelecting = false;
		private int currentColWidthChanging = -1;
		private int currentRowHeightChanging = -1;
		private float adjustBackup = 0;
		private float adjustNewValue = 0;
		private int fullColSelection = -1;
		private int fullRowSelection = -1;
		private ReoGridCell mouseCapturedCell = null;

		protected override void OnMouseDown(MouseEventArgs e)
		{
			if (!Focused) Focus();

			// if currently control is in editing mode, make the input fields invisible
			if (currentEditingCell != null)
			{
				editTextbox.Visible = false;
				//numericTextbox.Visible = false;
				//dropWindow.Visible = false;
			}

			Point p = e.Location;

			// activate a view on mouse down
			// an activated view likes a focused control to recept
			// the scrolling event when mouse wheel
			viewportController.ChangeActiveView(p);

			// convert point postion from control to view
			PointF vp = viewportController.PointToActivePart(p);

			// specify whether the mouse event has been processed
			bool isProcessed = false;

			// mouse down in LeadHead?
			if (viewportController.PointInLeadHead(p))
			{
				SelectRangeByPos(0, 0, this.rows.Count - 1, this.cols.Count - 1);
				isProcessed = true;
			}
				// mouse down in Column Headers?
			else if (viewportController.PointInColumnHead(p))
			{
				int col = -1;
				bool inSeparator = viewportController.FindColHeadIndex(vp.X, out col);

				if (col >= 0)
				{
					// adjust columns width
					if (inSeparator 
						&& e.Button == MouseButtons.Left
						&& HasSettings(ReoGridSettings.Edit_AllowAdjustColumnWidth))
					{
						currentColWidthChanging = col;
						adjustBackup = adjustNewValue = cols[currentColWidthChanging].Width;
					}
					else
					{
						bool isFullColSelected = (selectionMode == ReoGridSelectionMode.Range
							&& selectionRange.Rows == this.rows.Count
							&& selectionRange.ContainsColumn(col));

						// select whole column
						if (!isFullColSelected || e.Button == MouseButtons.Left)
						{
							if (!Toolkit.IsKeyDown(Win32.VKey.VK_SHIFT))
							{
								FocusPos = selOriginal = new ReoGridPos(0, col);
							}

							selEnd = new ReoGridPos(this.rows.Count - 1, col);
							fullColSelection = col;
							SelectRangeByPos(selOriginal, selEnd);
						}

						// show context menu
						if (e.Button == MouseButtons.Right)
						{
							if (ColHeadContextMenuStrip != null)
								ColHeadContextMenuStrip.Show(PointToScreen(e.Location));
						}
					}

					isProcessed = true;
				}
			}
				// mouse down in Row indexes?
			else if (viewportController.PointInRowHead(p))
			{
				int row = -1;
				bool inSeparator = viewportController.FindRowHeadIndex(vp.Y, out row);

				if (row >= 0)
				{
					if (inSeparator 
						&& e.Button == System.Windows.Forms.MouseButtons.Left
						&& HasSettings(ReoGridSettings.Edit_AllowAdjustRowHeight))
					{
						currentRowHeightChanging = row;
						adjustBackup = adjustNewValue = rows[currentRowHeightChanging].Height;
					}
					else
					{
						bool isFullRowSelected = (selectionMode == ReoGridSelectionMode.Range
							&& selectionRange.Cols == this.cols.Count
							&& selectionRange.ContainsRow(row));

						if (!isFullRowSelected || e.Button == MouseButtons.Left)
						{
							if (!Toolkit.IsKeyDown(Win32.VKey.VK_SHIFT))
							{
								FocusPos = selOriginal = new ReoGridPos(row, 0);
							}
							selEnd = new ReoGridPos(row, this.cols.Count - 1);
							fullRowSelection = row;
							SelectRangeByPos(selOriginal, selEnd);
						}

						if (e.Button == MouseButtons.Right)
						{
							if (RowHeadContextMenuStrip != null)
							{
								RowHeadContextMenuStrip.Show(PointToScreen(e.Location));
							}
						}
					}
					isProcessed = true;
				}
			}

			// mouse down in cells?
			if (!isProcessed && viewportController.PointInCells(p))
			{
				int row = viewportController.GetRowByPoint(vp.Y);
				if (row != -1) // in valid rows
				{
					int col = viewportController.GetColByPoint(vp.X);

					if (col != -1) // in valid cols
					{
						ReoGridPos pos = new ReoGridPos(row, col);

						var cell = cells[row, col];

						if (cell != null)
						{
							if (!cell.IsValidCell)
							{
								cell = GetMergedCellOfRange(cell);
							}

							if ((cell.body != null) || CellMouseDown != null)
							{
								var cellRect = GetCellBounds(pos);

								var evtArg = new RGCellMouseEventArgs(this, cell, new Point(
									(int)Math.Round(vp.X - cellRect.Left),
									(int)Math.Round(vp.Y - cellRect.Top)), e.Button, e.Clicks);

								if (CellMouseDown != null)
								{
									CellMouseDown(this, evtArg);
								}

								if (cell != null && cell.body != null)
								{
									if (cell.body.OnMouseDown(evtArg))
									{
										isProcessed = true;

										if (cell.body.AutoCaptureMouse() || evtArg.Capture)
										{
											mouseCapturedCell = cell;
										}
									}
								}
							}
						}

#if EX_SCRIPT
						object scriptReturn = RaiseScriptEvent("onmousedown", RSUtility.CreatePosObject(pos));
						if (scriptReturn != null && !ScriptRunningMachine.GetBoolValue(scriptReturn))
						{
							return;
						}
#endif
						if (!isProcessed)
						{
							if (e.Button == MouseButtons.Left)
							{
								if (selectionMode == ReoGridSelectionMode.Range)
								{
									isRangeSelecting = true;
								}

								// selection begin position
								if (!Toolkit.IsKeyDown(Win32.VKey.VK_SHIFT))
								{
									FocusPos = selOriginal = pos;
								}

								// selection end position
								selEnd = pos;

								// do select range
								SelectRangeByPos(selOriginal, selEnd);

								// block other processes
								isProcessed = true;
							}
							else if (e.Button == System.Windows.Forms.MouseButtons.Right
								&& cellContextMenuStrip != null)
							{
								if (!selectionRange.Contains(row, col))
								{
									FocusPos = selOriginal = selEnd = new ReoGridPos(row, col);
									SelectRangeByPos(selOriginal, selOriginal);
									isProcessed = true;
								}

								cellContextMenuStrip.Show(PointToScreen(e.Location));
							}
						}
					}
				}
			}

			if (!isProcessed)
			{
				isProcessed = viewportController.OnMouseDown(e);
			}

			if (isProcessed) InvalidateCanvas();
		}
		protected override void OnMouseDoubleClick(MouseEventArgs e)
		{
			if (!focusPos.IsEmpty && viewportController.PointInCells(e.Location)
				&& focusPos == viewportController.GetPosByPoint(e.Location))
			{
				StartEdit();
			}
		}
		protected override void OnMouseMove(MouseEventArgs e)
		{
			bool isProcessed = false;

			Point p = e.Location;
			viewportController.ChangeActiveView(p);
			PointF vp = viewportController.PointToActivePart(p);

			if (mouseCapturedCell != null && mouseCapturedCell.body != null)
			{
				int rowTop = this.rows[mouseCapturedCell.Row].Top;
				int colLeft = this.cols[mouseCapturedCell.Col].Left;

				var evtArg = new RGCellMouseEventArgs(this, mouseCapturedCell, new Point(
						(int)Math.Round(vp.X - colLeft),
						(int)Math.Round(vp.Y - rowTop)), e.Button, e.Clicks);

				isProcessed = mouseCapturedCell.body.OnMouseMove(evtArg);
			}
			else
			{
				if (fullColSelection > -1)
				{
					Cursor = fullColSelectCursor;
					int col = -1;
					viewportController.FindColHeadIndex(vp.X, out col);
					if (col > -1)
					{
						selEnd.Col = col;
						SelectRangeByPos(selOriginal, selEnd);
						isProcessed = true;
					}
				}
				else if (fullRowSelection > -1)
				{
					Cursor = fullRowSelectCursor;
					int row = -1;
					viewportController.FindRowHeadIndex(vp.Y, out row);
					if (row > -1)
					{
						selEnd.Row = row;
						SelectRangeByPos(selOriginal, selEnd);
						isProcessed = true;
					}
				}
				else if (isRangeSelecting)
				{
					if (e.Button == MouseButtons.Left)
					{
						selEnd = viewportController.GetPosByPoint(p);
						if (lastSelectionEndPos != selEnd)
						{
							ReoGridRange selectedRange = this.selectionRange;
							SelectRangeByPos(selOriginal.Row, selOriginal.Col, selEnd.Row, selEnd.Col);
							if (selectedRange != selectionRange)
							{
								ScrollToFocusCell();
								isProcessed = true;
							}
							lastSelectionEndPos = selEnd;
						}
					}
				}
				// not in adjust size 
				else if (currentColWidthChanging == -1 && currentRowHeightChanging == -1)
				{
					if (viewportController.PointInLeadHead(p))
					{
						Cursor = gridSelectCursor;
					}
					else if (viewportController.PointInColumnHead(p))
					{
						int col = -1;

						bool inline = viewportController.FindColHeadIndex(vp.X, out col)
								&& HasSettings(ReoGridSettings.Edit_AllowAdjustColumnWidth);

						if (col >= 0) Cursor = inline ? Cursors.VSplit : fullColSelectCursor;
					}
					else if (viewportController.PointInRowHead(p))
					{
						int row = -1;

						bool inline = viewportController.FindRowHeadIndex(vp.Y, out row)
								&& HasSettings(ReoGridSettings.Edit_AllowAdjustRowHeight);

						if (row >= 0) Cursor = inline ? Cursors.HSplit : fullRowSelectCursor;
					}
					else if (viewportController.PointInCells(p))
					{
						ReoGridPos newHoverPos = viewportController.GetPosByPoint(p);
						if (newHoverPos != hoverPos)
						{
							HoverPos = newHoverPos;
						}

						if (!hoverPos.IsEmpty)
						{
							var cell = cells[hoverPos.Row, hoverPos.Col];

							if (cell != null)
							{
								if (!cell.IsValidCell)
								{
									cell = GetMergedCellOfRange(cell);
								}

								if (cell.body != null || CellMouseMove != null)
								{
									var cellRect = GetCellBounds(hoverPos);

									var evtArg = new RGCellMouseEventArgs(this, cell, new Point(
											(int)Math.Round(vp.X - cellRect.Left),
											(int)Math.Round(vp.Y - cellRect.Top)), e.Button, e.Clicks);

									if (CellMouseMove != null)
									{
										CellMouseMove(this, evtArg);
									}

									if (cell.body != null)
									{
										cell.body.OnMouseMove(evtArg);
									}
								}
							}
						}

						Cursor = gridSelectCursor;
					}
					else
					{
						Cursor = Cursors.Default;
					}
				}
				else if (currentColWidthChanging >= 0)
				{
					if (e.Button == MouseButtons.Left)
					{
						ReoGridColHead col = cols[currentColWidthChanging];
						adjustNewValue = vp.X - col.Left;
						if (adjustNewValue < 0) adjustNewValue = 0;
						isProcessed = true;
					}
				}
				else if (currentRowHeightChanging >= 0)
				{
					if (e.Button == MouseButtons.Left)
					{
						ReoGridRowHead row = rows[currentRowHeightChanging];
						adjustNewValue = vp.Y - row.Top;
						if (adjustNewValue < 0) adjustNewValue = 0;
						isProcessed = true;
					}
				}
				else
				{
					this.viewportController.OnMouseDown(e);
				}
			}

			if (isProcessed) InvalidateCanvas();
		}
		protected override void OnMouseUp(MouseEventArgs e)
		{
			bool isProcessed = false;

			PointF vp = viewportController.PointToActivePart(e.Location);

			ReoGridPos pos = viewportController.GetPosByPoint(e.Location);

			if (isRangeSelecting)
			{
				selEnd = pos;
				ScrollToFocusCell();

				if (whenRangePicked != null)
				{
					if (whenRangePicked(this, selectionRange))
					{
						EndPickRange();
					}
				}

				if (SelectionRangeChanged != null)
				{
					SelectionRangeChanged(this, new RGRangeEventArgs(selectionRange));
				}
			}

			if (mouseCapturedCell != null && mouseCapturedCell.body != null)
			{
				int rowTop = this.rows[mouseCapturedCell.Row].Top;
				int colLeft = this.cols[mouseCapturedCell.Col].Left;

				var evtArg = new RGCellMouseEventArgs(this, mouseCapturedCell, new Point(
						(int)Math.Round(vp.X - colLeft),
						(int)Math.Round(vp.Y - rowTop)), e.Button, e.Clicks);

				isProcessed = mouseCapturedCell.body.OnMouseUp(evtArg);
			}
			else
			{
				int row = pos.Row;
				int col = pos.Col;

				var cell = cells[row, col];

				if ((cell != null && cell.body != null) || CellMouseUp != null)
				{
					int rowTop = this.rows[row].Top;
					int colLeft = this.cols[col].Left;

					var evtArg = new RGCellMouseEventArgs(this, cell, new Point(
						(int)Math.Round(vp.X - colLeft),
						(int)Math.Round(vp.Y - rowTop)), e.Button, e.Clicks);

					if (CellMouseUp != null)
					{
						CellMouseUp(this, evtArg);
					}

					if (cell != null && cell.body != null)
					{
						isProcessed = cell.body.OnMouseUp(evtArg);
					}
				}
			}

			mouseCapturedCell = null;

#if EX_SCRIPT
			object scriptReturn = RaiseScriptEvent("onmouseup", RSUtility.CreatePosObject(selEnd));

			// run if script return true or nothing
			if (scriptReturn == null || ScriptRunningMachine.GetBoolValue(scriptReturn))
			{
				if (isRangeSelecting)
				{
					RaiseScriptEvent("onselectionchange");
				}
			}
#endif

			if (!isProcessed)
			{
				isProcessed = this.viewportController.OnMouseUp(e);
			}

			isRangeSelecting = false;

			if (currentColWidthChanging != -1)
			{
				RGSetColsWidthAction setColsWidthAction;

				bool isFullColSelected = (selectionMode == ReoGridSelectionMode.Range
					&& selectionRange.Rows == this.rows.Count
					&& selectionRange.ContainsColumn(currentColWidthChanging));

				ushort targetWidth = (ushort)adjustNewValue;

				if (targetWidth != this.cols[currentColWidthChanging].Width)
				{
					if (isFullColSelected)
						setColsWidthAction = new RGSetColsWidthAction(selectionRange.Col, selectionRange.Cols, targetWidth);
					else
						setColsWidthAction = new RGSetColsWidthAction(currentColWidthChanging, 1, targetWidth);

					DoAction(setColsWidthAction);
				}

				currentColWidthChanging = -1;
				adjustBackup = adjustNewValue = 0;
				isProcessed = true;
			}

			if (currentRowHeightChanging != -1)
			{
				RGSetRowsHeightAction setRowsHeightAction;

				bool isFullRowSelected = (selectionMode == ReoGridSelectionMode.Range
					&& selectionRange.Cols == this.cols.Count
					&& selectionRange.ContainsRow(currentRowHeightChanging));

				ushort targetHeight = (ushort)adjustNewValue;

				if (targetHeight != this.rows[currentRowHeightChanging].Height)
				{
					if (isFullRowSelected)
						setRowsHeightAction = new RGSetRowsHeightAction(selectionRange.Row, selectionRange.Rows, targetHeight);
					else
						setRowsHeightAction = new RGSetRowsHeightAction(currentRowHeightChanging, 1, targetHeight);

					DoAction(setRowsHeightAction);
				}

				currentRowHeightChanging = -1;
				adjustBackup = adjustNewValue = 0;
				isProcessed = true;
			}

			if (fullColSelection > -1)
			{
				fullColSelection = -1;
			}

			if (fullRowSelection > -1)
			{
				fullRowSelection = -1;
			}

			if (isProcessed) InvalidateCanvas();
		}
		protected override void OnMouseWheel(MouseEventArgs e)
		{
			if (Toolkit.IsKeyDown(Win32.VKey.VK_CONTROL))
			{
				if (HasSettings(ReoGridSettings.Edit_AllowUserScale))
				{
					SetScale(scaleFactor + 0.001f * e.Delta, e.Location);
				}
			}
			else
			{

				if (!focusPos.IsEmpty)
				{
					var cell = cells[focusPos.Row, focusPos.Col];

					var cellWheelEvent = new RGCellMouseEventArgs(this, cell, e.Location,
						e.Button, e.Clicks)
						{
							Delta = e.Delta,
							CellPosition = focusPos,
						};

					if (cell != null && cell.body != null)
					{
						cell.body.OnMouseWheel(cellWheelEvent);
					}
				}

				if (Toolkit.IsKeyDown(Win32.VKey.VK_SHIFT))
				{
					viewportController.ScrollViews(ScrollDirection.Horizontal, e.Delta, 0);
				}
				else
				{
					viewportController.ScrollViews(ScrollDirection.Vertical, 0, -e.Delta);
				}
			}
		}
		protected override void OnMouseLeave(EventArgs e)
		{
			Cursor = Cursors.Default;
			HoverPos = ReoGridPos.Empty;
		}

		private Cursor cellsSelectionCursor;
		
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		/// <summary>
		/// Cursor symbol displayed when moving mouse over on cells
		/// </summary>
		public Cursor CellsSelectionCursor
		{
			get { return cellsSelectionCursor == null ? this.defaultGridSelectCursor : cellsSelectionCursor; }
			set
			{
				cellsSelectionCursor = value;
				
				gridSelectCursor = (cellsSelectionCursor == null ? this.defaultGridSelectCursor : cellsSelectionCursor);
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Cursor DefaultGridSelectCursor { get { return this.defaultGridSelectCursor; } }

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		/// <summary>
		/// Cursor symbol displayed when moving mouse over on row headers
		/// </summary>
		public Cursor FullRowSelectionCursor { get; set; }

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		/// <summary>
		/// Cursor symbol displayed when moving mouse over on column headers
		/// </summary>
		public Cursor FullColumnSelectionCursor { get; set; }

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Cursor LeadHeadHoverCursor { get; set; }

		public event EventHandler<RGCellMouseEventArgs> CellMouseEnter;
		public event EventHandler<RGCellMouseEventArgs> CellMouseLeave;
		public event EventHandler<RGCellMouseEventArgs> CellMouseMove;
		public event EventHandler<RGCellMouseEventArgs> CellMouseDown;
		public event EventHandler<RGCellMouseEventArgs> CellMouseUp;

		#endregion // Mouse

		#region Keyboard
		protected override bool IsInputKey(Keys keyData)
		{
			if (editTextbox.Visible) return false;

			switch (keyData)
			{
				case Keys.Up:
				case Keys.Down:
				case Keys.Left:
				case Keys.Right:

				case Keys.Up | Keys.Shift:
				case Keys.Down | Keys.Shift:
				case Keys.Left | Keys.Shift:
				case Keys.Right | Keys.Shift:

				case Keys.Control | Keys.C:
				case Keys.Control | Keys.X:
				case Keys.Control | Keys.V:

				case Keys.Control | Keys.Z:
				case Keys.Control | Keys.Y:

				case Keys.Control | Keys.Oemplus:
				case Keys.Control | Keys.OemMinus:
				case Keys.Control | Keys.D0:

				case Keys.Control | Keys.A:

					return true;
			}

			return base.IsInputKey(keyData);
		}
		protected override void OnKeyDown(KeyEventArgs e)
		{

#if EX_SCRIPT
			object rs = RaiseScriptEvent("onkeydown", new RSKeyEvent(e));
			if (rs != null && !ScriptRunningMachine.GetBoolValue(rs))
			{
				return;
			}
#endif

			if (!focusPos.IsEmpty)
			{
				var cell = cells[focusPos.Row, focusPos.Col];
				if (cell != null && cell.body != null)
				{
					bool processed = cell.body.OnKeyDown(e);
					if (processed) InvalidateCell(cell);
				}
			}

			switch (e.KeyData)
			{
				case Keys.Up | Keys.Shift:
					if (selEnd.Row > 0)
					{
						selEnd.Row = RowToRangeTop(selEnd.Row, SelectionRange.Col, SelectionRange.Col2);
						selEnd.Row--;
						if (selectionMode == ReoGridSelectionMode.Cell)
						{
							selOriginal.Row = selEnd.Row;
							FocusPos = selOriginal;
						}
						StepApplyRangeSelection(true);
					}
					break;
				case Keys.Down | Keys.Shift:
					if (selEnd.Row < this.rows.Count - 1)
					{
						selEnd.Row = RowToRangeBottom(selEnd.Row, SelectionRange.Col, SelectionRange.Col2);
						selEnd.Row++;
						if (selectionMode == ReoGridSelectionMode.Cell)
						{
							selOriginal.Row = selEnd.Row;
							FocusPos = selOriginal;
						}
						StepApplyRangeSelection(true);
					}
					break;
				case Keys.Left | Keys.Shift:
					if (selEnd.Col > 0)
					{
						selEnd.Col = ColToRangeLeft(selEnd.Col, SelectionRange.Row, SelectionRange.Row2);
						selEnd.Col--;
						if (selectionMode == ReoGridSelectionMode.Cell)
						{
							selOriginal.Col = selEnd.Col;
							FocusPos = selOriginal;
						}
						StepApplyRangeSelection(true);
					}
					break;
				case Keys.Right | Keys.Shift:
					if (selEnd.Col < this.cols.Count - 1)
					{
						selEnd.Col = ColToRangeRight(selEnd.Col, SelectionRange.Row, SelectionRange.Row2);
						selEnd.Col++;
						if (selectionMode == ReoGridSelectionMode.Cell)
						{
							selOriginal.Col = selEnd.Col;
							FocusPos = selOriginal;
						}
						StepApplyRangeSelection(true);
					}
					break;

				case Keys.Control | Keys.C:

					Copy();
					break;
				case Keys.Control | Keys.X:
					Cut();
					break;
				case Keys.Control | Keys.V:
					Paste();
					break;

				case Keys.Up:
					MoveSelectionUp();
					break;
				case Keys.Down:
					MoveSelectionDown();
					break;
				case Keys.Left:
					MoveSelectionLeft();
					break;
				case Keys.Right:
					MoveSelectionRight();
					break;

				case Keys.Enter:
					MoveSelectionForward();
					break;

				case Keys.F2:
					StartEdit();
					break;

				case Keys.F4:
					RepeatLastAction(selectionRange);
					break;

				case Keys.Delete:
					DeleteRangeData(selectionRange);
					break;

				case Keys.Back:
					if (!focusPos.IsEmpty)
					{
						this[focusPos] = null;
						StartEdit(focusPos);
					}
					break;

				case Keys.Control | Keys.Z:
					Undo();
					break;
				case Keys.Control | Keys.Y:
					Redo();
					break;

				case Keys.Control | Keys.Oemplus:
					if (HasSettings(ReoGridSettings.Edit_AllowUserScale)) ZoomIn();
					break;
				case Keys.Control | Keys.OemMinus:
					if (HasSettings(ReoGridSettings.Edit_AllowUserScale)) ZoomOut();
					break;
				case Keys.Control | Keys.D0:
					if (HasSettings(ReoGridSettings.Edit_AllowUserScale)) ZoomReset();
					break;

				case Keys.Control | Keys.A:
					SelectAll();
					break;

				default:
					base.OnKeyDown(e);
					break;
			}
		}

		protected override void OnKeyUp(KeyEventArgs e)
		{
#if EX_SCRIPT
			object rs = RaiseScriptEvent("onkeyup", new RSKeyEvent(e));
			if (rs != null && !ScriptRunningMachine.GetBoolValue(rs))
			{
				return;
			}
#endif

			if (!focusPos.IsEmpty)
			{
				var cell = cells[focusPos.Row, focusPos.Col];
				if (cell != null && cell.body != null)
				{
					bool processed = cell.body.OnKeyUp(e);
					if (processed) InvalidateCell(cell);
				}
			}
		}
		#endregion // Keyboard

		#region Scrolling
		private Panel rightPanel;
		private Panel bottomPanel;
		internal enum ScrollDirection : byte
		{
			None = 0,
			Horizontal = 1,
			Vertical = 2,
			Both = Horizontal | Vertical,
		}
		private class ScrollBarCorner : Control
		{
			public ScrollBarCorner()
			{
				Size = new Size(20, 20);
			}
			protected override void OnMouseMove(MouseEventArgs e)
			{
				base.OnMouseMove(e);
				Cursor = Cursors.Default;
			}
		}
		#endregion

		#region Internal Utilites
		private static Cursor LoadCursorFromResource(byte[] res)
		{
			using (MemoryStream ms = new MemoryStream(res))
			{
				return new Cursor(ms);
			}
		}
		#endregion

		#region Range Utilities
		internal ReoGridPos FixPos(ReoGridPos pos)
		{
			if (pos.Row < 0) pos.Row = 0;
			if (pos.Col < 0) pos.Col = 0;
			if (pos.Row > this.rows.Count - 1) pos.Row = this.rows.Count - 1;
			if (pos.Col > this.cols.Count - 1) pos.Col = this.cols.Count - 1;
			return pos;
		}
		
		/// <summary>
		/// Check the bounds of range, return a sefe range
		/// </summary>
		/// <param name="range">specified range to be checked</param>
		/// <returns>a safe range has been checked</returns>
		public ReoGridRange FixRange(ReoGridRange range)
		{
			range.StartPos = FixPos(range.StartPos);
			if (range.Rows == -1) range.Row2 = this.rows.Count;
			if (range.Cols == -1) range.Col2 = this.cols.Count;
			if (range.Row2 > this.rows.Count - 1 || range.Rows == -1) range.Row2 = this.rows.Count - 1;
			if (range.Col2 > this.cols.Count - 1 || range.Cols == -1) range.Col2 = this.cols.Count - 1;
			return range;
		}
	
		/// <summary>
		/// Check a range whether cross any merged range, return the most outside range
		/// </summary>
		/// <param name="range">a range to be checked</param>
		/// <returns>most outside range</returns>
		public ReoGridRange CheckMergedRange(ReoGridRange range)
		{
			int minr = range.Row;
			int minc = range.Col;
			int maxr = range.Row2;
			int maxc = range.Col2;

			bool boundsChanged = true;

			while (boundsChanged)
			{
				boundsChanged = false;

				for (int r = minr; r <= maxr; r++)
				{
					var cell = cells[r, minc];
					if (cell != null && cell.MergeStartPos != ReoGridPos.Empty)
					{
						int rr = cell.MergeStartPos.Row;
						if (minr > rr)
						{
							minr = rr;
							boundsChanged = true;
						}
						int cc = cell.MergeStartPos.Col;
						if (minc > cc)
						{
							minc = cc;
							boundsChanged = true;
						}
					}
				}
				for (int r = minr; r <= maxr; r++)
				{
					var cell = cells[r, maxc];

					if (cell != null && cell.MergeEndPos != null)
					{
						int rr = cell.MergeEndPos.Row;
						if (maxr < rr)
						{
							maxr = rr;
							boundsChanged = true;
						}
						int cc = cell.MergeEndPos.Col;
						if (maxc < cc)
						{
							maxc = cc;
							boundsChanged = true;
						}
					}
				}
				for (int c = minc; c <= maxc; c++)
				{
					if (cells[minr, c] != null && cells[minr, c].MergeStartPos != ReoGridPos.Empty)
					{
						int rr = cells[minr, c].MergeStartPos.Row;
						if (minr > rr)
						{
							minr = rr;
							boundsChanged = true;
						}
						int cc = cells[minr, c].MergeStartPos.Col;
						if (minc > cc)
						{
							minc = cc;
							boundsChanged = true;
						}
					}
				}
				for (int c = minc; c <= maxc; c++)
				{
					if (cells[maxr, c] != null && cells[maxr, c].MergeEndPos != null)
					{
						int rr = cells[maxr, c].MergeEndPos.Row;
						if (maxr < rr)
						{
							maxr = rr;
							boundsChanged = true;
						}
						int cc = cells[maxr, c].MergeEndPos.Col;
						if (maxc < cc)
						{
							maxc = cc;
							boundsChanged = true;
						}
					}
				}
			}

			return new ReoGridRange(minr, minc, maxr - minr + 1, maxc - minc + 1);
		}

		internal Rectangle GetRangeBounds(int row, int col, int rows, int cols)
		{
			return GetRangeBounds(new ReoGridRange(row, col, rows, cols));
		}
		internal Rectangle GetRangeBounds(ReoGridPos startPos, ReoGridPos endPos)
		{
			return GetRangeBounds(new ReoGridRange(startPos, endPos));
		}
		internal Rectangle GetRangeBounds(ReoGridRange range)
		{
			ReoGridRange fixedRange = FixRange(range);

			int torow = fixedRange.Row2;
			int tocol = fixedRange.Col2;

			int width = cols[tocol].Left - cols[fixedRange.Col].Left + cols[tocol].Width;
			int height = rows[torow].Top - rows[fixedRange.Row].Top + rows[torow].Height;

			return new Rectangle(cols[fixedRange.Col].Left, rows[fixedRange.Row].Top, width + 1, height + 1);
		}
		internal Rectangle GetGridBounds(ReoGridPos pos)
		{
			return GetGridBounds(pos.Row, pos.Col);
		}
		internal Rectangle GetGridBounds(int row, int col)
		{
			return new Rectangle(cols[col].Left, rows[row].Top, cols[col].Width + 1, rows[row].Height + 1);
		}
		internal Rectangle GetCellBounds(ReoGridPos pos)
		{
			return GetCellBounds(pos.Row, pos.Col);
		}
		internal Rectangle GetCellBounds(int row, int col)
		{
			if (cells[row, col] == null)
				return GetGridBounds(row, col);
			else if (cells[row, col].MergeStartPos != ReoGridPos.Empty)
			{
				ReoGridCell cell = GetCell(cells[row, col].MergeStartPos);
				return (cell != null) ? cell.Bounds : GetGridBounds(row, col);
			}
			else
				return cells[row, col].Bounds;
		}

		internal int RowToRangeTop(ReoGridPos pos) { return RowToRangeTop(pos.Row, pos.Col); }
		private int RowToRangeTop(int row, int col) { return RowToRangeTop(row, col, col); }
		private int RowToRangeTop(int row, int col1, int col2)
		{
			int minc = Math.Min(col1, col2);
			int maxc = Math.Max(col1, col2);
			int minr = row;

			for (int c = minc; c <= maxc; c++)
			{
				if (cells[row, c] != null && cells[row, c].MergeStartPos != ReoGridPos.Empty)
				{
					minr = Math.Min(minr, cells[row, c].MergeStartPos.Row);
				}
			}

			return minr;
		}

		internal int RowToRangeBottom(ReoGridPos pos) { return RowToRangeBottom(pos.Row, pos.Col); }
		private int RowToRangeBottom(int row, int col) { return RowToRangeBottom(row, col, col); }
		private int RowToRangeBottom(int row, int col1, int col2)
		{
			int minc = Math.Min(col1, col2);
			int maxc = Math.Max(col1, col2);
			int maxr = row;

			for (int c = minc; c <= maxc; c++)
			{
				if (cells[row, c] != null && cells[row, c].MergeEndPos != ReoGridPos.Empty)
				{
					maxr = Math.Max(maxr, cells[row, c].MergeEndPos.Row);
				}
			}

			return maxr;
		}

		internal int ColToRangeLeft(ReoGridPos pos) { return ColToRangeLeft(pos.Row, pos.Col); }
		private int ColToRangeLeft(int row, int col) { return ColToRangeLeft(col, row, row); }
		private int ColToRangeLeft(int col, int row1, int row2)
		{
			int minr = Math.Min(row1, row2);
			int maxr = Math.Max(row1, row2);
			int minc = col;

			for (int r = minr; r <= maxr; r++)
			{
				if (cells[r, col] != null && cells[r, col].MergeStartPos != ReoGridPos.Empty)
				{
					minc = Math.Min(minc, cells[r, col].MergeStartPos.Col);
				}
			}

			return minc;
		}

		internal int ColToRangeRight(ReoGridPos pos) { return ColToRangeRight(pos.Row, pos.Col); }
		private int ColToRangeRight(int row, int col) { return ColToRangeRight(col, row, row); }
		private int ColToRangeRight(int col, int row1, int row2)
		{
			int minr = Math.Min(row1, row2);
			int maxr = Math.Max(row1, row2);
			int maxc = col;

			for (int r = minr; r <= maxr; r++)
			{
				if (cells[r, col] != null && cells[r, col].MergeEndPos != ReoGridPos.Empty)
				{
					maxc = Math.Max(maxc, cells[r, col].MergeEndPos.Col);
				}
			}

			return maxc;
		}

		private bool IsInsideSameMergedCell(int r1, int c1, int r2, int c2)
		{
			return IsInsideSameMergedCell(cells[r1, c1], cells[r2, c2]);
		}
		private static bool IsInsideSameMergedCell(ReoGridCell cell1, ReoGridCell cell2)
		{
			return cell1 != null && cell2 != null
				&& !cell1.MergeStartPos.IsEmpty
				&& ReoGridPos.Equals(cell1.MergeStartPos, cell2.MergeStartPos);
		}
		internal bool RangeIsCell(ReoGridRange range)
		{
			ReoGridCell cell = cells[range.Row, range.Col];
			return (cell.Rowspan == range.Rows && cell.Colspan == range.Cols);
		}
		#endregion
		
		#region Pick Range
		private Func<ReoGridControl, ReoGridRange, bool> whenRangePicked;

		public void PickRange(Func<ReoGridControl, ReoGridRange, bool> onPicked, Cursor pickerCursor)
		{
			this.whenRangePicked = onPicked;
			this.gridSelectCursor = pickerCursor;
		}

		public void EndPickRange()
		{
			this.whenRangePicked = null;
			this.gridSelectCursor = (cellsSelectionCursor == null ? this.defaultGridSelectCursor : cellsSelectionCursor);
		}
		#endregion

		#region Named Range
		private Dictionary<string, ReferenceRange> namedRanges;
		public void DefineNamedRange(string name, ReoGridRange range)
		{
			if (namedRanges == null) namedRanges = new Dictionary<string, ReferenceRange>();

			namedRanges[name] = new ReferenceRange(CreateAndGetCell(range.StartPos), CreateAndGetCell(range.EndPos));
		}
		public ReoGridRange GetNamedRange(string name)
		{
			if (namedRanges == null) return ReoGridRange.Empty;

			ReferenceRange range = null;
			namedRanges.TryGetValue(name, out range);

			return new ReoGridRange(range.StartCell.Pos, range.EndCell.Pos);
		}
		public void RemoveNamedRange(string name)
		{
			if (namedRanges.ContainsKey(name))
			{
				namedRanges.Remove(name);
			}
		}
		#endregion

		#region Border
		private RegularTreeArray<ReoGridHBorder> hBorders = new RegularTreeArray<ReoGridHBorder>();
		private RegularTreeArray<ReoGridVBorder> vBorders = new RegularTreeArray<ReoGridVBorder>();

		#region Set Border

		/// <summary>
		/// Set borders to specified range
		/// </summary>
		/// <param name="row">number of start row</param>
		/// <param name="col">number of start column</param>
		/// <param name="rows">number of rows</param>
		/// <param name="cols">number of columns</param>
		/// <param name="pos">position around specified range to be set border</param>
		/// <param name="style">style of border to be set</param>
		public void SetRangeBorder(int row, int col, int rows, int cols, ReoGridBorderPos pos, ReoGridBorderStyle style)
		{
			SetRangeBorder(new ReoGridRange(row, col, rows, cols), pos, style);
		}

		/// <summary>
		/// Set border styles to specified range. Or set an empty border style to remove styles from specified range.
		/// </summary>
		/// <param name="range">Specified range to be set</param>
		/// <param name="pos">Style of which position in range should be setted</param>
		/// <see cref="ReoGridBorderPos"/>
		/// <param name="style">The style of border to be set</param>
		/// <see cref="ReoGridBorderStyle"/>
		public void SetRangeBorder(ReoGridRange range, ReoGridBorderPos pos, ReoGridBorderStyle style)
		{
			ReoGridRange fixedRange = FixRange(range);

			int r1 = fixedRange.Row;
			int c1 = fixedRange.Col;
			int r2 = fixedRange.Row2 + 1;
			int c2 = fixedRange.Col2 + 1;

			#region Left and Right
			// vertical outside
			if ((pos & ReoGridBorderPos.Left) == ReoGridBorderPos.Left)
			{
				CutBeforeVBorder(r1, c1);
				SetVBorders(r1, c1, fixedRange.Rows, style, VBorderOwnerPosition.Left);

				// set owner position
				//for (int r = r1; r < r2; r++)
				//{
				//	if (vBorders[r, c1] != null) vBorders[r, c1].Pos |= VBorderOwnerPosition.Left;
				//}
			}
			if ((pos & ReoGridBorderPos.Right) == ReoGridBorderPos.Right)
			{
				CutBeforeVBorder(r1, c2);
				SetVBorders(r1, c2, fixedRange.Rows, style, VBorderOwnerPosition.Right);

				// set owner position
				//for (int r = r1; r < r2; r++)
				//{
				//	if (vBorders[r, c2] != null) vBorders[r, c2].Pos |= VBorderOwnerPosition.Right;
				//}
			}
			#endregion

			#region Top and Bottom
			// horzontial outside
			if ((pos & ReoGridBorderPos.Top) == ReoGridBorderPos.Top)
			{
				CutBeforeHBorder(r1, c1);
				SetHBorders(r1, c1, fixedRange.Cols, style, HBorderOwnerPosition.Top);

				// set owner position
				//for (int c = c1; c < c2; c++)
				//{
				//	if (hBorders[r1, c] != null) hBorders[r1, c].Pos |= HBorderOwnerPosition.Top;
				//}
			}
			if ((pos & ReoGridBorderPos.Bottom) == ReoGridBorderPos.Bottom)
			{
				CutBeforeHBorder(r2, c1);
				SetHBorders(r2, c1, fixedRange.Cols, style, HBorderOwnerPosition.Bottom);

				// set owner position
				//for (int c = c1; c < c2; c++)
				//{
				//	if (hBorders[r2, c] != null) hBorders[r2, c].Pos |= HBorderOwnerPosition.Bottom;
				//}
			}
			#endregion

			#region Inside
			// inside
			if ((pos & ReoGridBorderPos.InsideVertical) == ReoGridBorderPos.InsideVertical)
			{
				for (int c = c1 + 1; c < c2; c++)
				{
					CutBeforeVBorder(r1, c);
					SetVBorders(r1, c, fixedRange.Rows, style, VBorderOwnerPosition.All);

					// set owner position
					//for (int r = r1; r < r2; r++)
					//{
					//	if (vBorders[r, c] != null) vBorders[r, c].Pos |= VBorderOwnerPosition.All;
					//}
				}
			}
			if ((pos & ReoGridBorderPos.InsideHorizontal) == ReoGridBorderPos.InsideHorizontal)
			{
				for (int r = r1 + 1; r < r2; r++)
				{
					CutBeforeHBorder(r, c1);
					SetHBorders(r, c1, fixedRange.Cols, style, HBorderOwnerPosition.All);

					// set owner position
					//for (int c = c1; c < c2; c++)
					//{
					//	if (hBorders[r, c] != null) hBorders[r, c].Pos |= HBorderOwnerPosition.All;
					//}
				}
			}
			#endregion

			InvalidateCanvas();

			// raise border added event
			if (!style.IsEmpty)
			{
				if (BorderAdded != null)
				{
					BorderAdded(this, new RGBorderAddedEventArgs(fixedRange, pos, style));
				}
			}
			else
			{
				if (BorderRemoved != null)
				{
					BorderRemoved(this, new RGBorderRemovedEventArgs(fixedRange, pos));
				}
			}
		}

		/// <summary>
		/// Remove border style from specified range.
		/// </summary>
		/// <param name="range">Range to be removed</param>
		/// <param name="pos">Style of which position in range should be removed</param>
		public void RemoveRangeBorder(ReoGridRange range, ReoGridBorderPos pos)
		{
			SetRangeBorder(range, pos, ReoGridBorderStyle.Empty);
		}

		/// <summary>
		/// Event fired when any border styles be setted.
		/// </summary>
		public event EventHandler<RGBorderAddedEventArgs> BorderAdded;

		/// <summary>
		/// Event fired when any border styles be removed.
		/// </summary>
		public event EventHandler<RGBorderRemovedEventArgs> BorderRemoved;
		#endregion

		#region Get Border
		/// <summary>
		/// Get borders info from specified range.
		/// </summary>
		/// <param name="range">Range to get info of border</param>
		/// <returns>Borders info retrieved from specified range</returns>
		public ReoGridRangeBorderInfo GetRangeBorder(ReoGridRange range)
		{
			return GetRangeBorder(range, false);
		}
		/// <summary>
		/// Get borders info from specified range.
		/// </summary>
		/// <param name="range">Range to get info of border</param>
		/// <param name="onlyGridOwn">Indicates whether only the borders belong to its cell to get</param>
		/// <returns>Borders info retrieved from specified range</returns>
		public ReoGridRangeBorderInfo GetRangeBorder(ReoGridRange range, bool onlyGridOwn)
		{
			ReoGridRange fixedRange = FixRange(range);

			ReoGridRangeBorderInfo borderInfo = new ReoGridRangeBorderInfo();

			if (fixedRange.Rows == 0 || fixedRange.Cols == 0) return borderInfo;

			// top
			borderInfo.Top = GetGridBorder(fixedRange.Row, fixedRange.Col, ReoGridBorderPos.Top, onlyGridOwn);
			for (int c = fixedRange.Col + 1; c <= fixedRange.Col2; c++)
			{
				if (borderInfo.Top != null)
				{
					if (hBorders[fixedRange.Row, c] == null
						|| hBorders[fixedRange.Row, c].Border != null
						&& !hBorders[fixedRange.Row, c].Border.Equals(borderInfo.Top))
					{
						borderInfo.NonUniformPos |= ReoGridBorderPos.Top;
						break;
					}
				}
				else
				{
					if (hBorders[fixedRange.Row, c] != null
						&& hBorders[fixedRange.Row, c].Border != null)
					{
						borderInfo.NonUniformPos |= ReoGridBorderPos.Top;
						break;
					}
				}
			}

			// bottom
			borderInfo.Bottom = GetGridBorder(fixedRange.Row2, fixedRange.Col, ReoGridBorderPos.Bottom, onlyGridOwn);
			if (borderInfo.Bottom != null)
			{
				for (int c = fixedRange.Col + 1; c <= fixedRange.Col2; c++)
				{
					if (hBorders[fixedRange.Row2 + 1, c] == null
						|| hBorders[fixedRange.Row2 + 1, c].Border != null
						&& !hBorders[fixedRange.Row2 + 1, c].Border.Equals(borderInfo.Bottom))
					{
						borderInfo.NonUniformPos |= ReoGridBorderPos.Bottom;
						break;
					}
				}
			}
			else
			{
				for (int c = fixedRange.Col + 1; c <= fixedRange.Col2; c++)
				{
					if (hBorders[fixedRange.Row2 + 1, c] != null
						&& hBorders[fixedRange.Row2 + 1, c].Border != null)
					{
						borderInfo.NonUniformPos |= ReoGridBorderPos.Bottom;
						break;
					}
				}
			}

			// left
			borderInfo.Left = GetGridBorder(fixedRange.Row, fixedRange.Col, ReoGridBorderPos.Left, onlyGridOwn);
			if (borderInfo.Left != null)
			{
				for (int r = fixedRange.Row + 1; r < fixedRange.Row2; r++)
				{
					if (vBorders[r, fixedRange.Col] == null
						|| vBorders[r, fixedRange.Col].Border != null
						&& !vBorders[r, fixedRange.Col].Border.Equals(borderInfo.Left))
					{
						borderInfo.NonUniformPos |= ReoGridBorderPos.Left;
						break;
					}
				}
			}
			else
			{
				for (int r = fixedRange.Row + 1; r <= fixedRange.Row2; r++)
				{
					if (vBorders[r, fixedRange.Col] != null
						&& vBorders[r, fixedRange.Col].Border != null)
					{
						borderInfo.NonUniformPos |= ReoGridBorderPos.Left;
						break;
					}
				}
			}

			// right
			borderInfo.Right = GetGridBorder(fixedRange.Row, fixedRange.Col2, ReoGridBorderPos.Right, onlyGridOwn);
			if (borderInfo.Right != null)
			{
				for (int r = fixedRange.Row + 1; r < fixedRange.Row2; r++)
				{
					if (vBorders[r, fixedRange.Col2 + 1] == null
						|| vBorders[r, fixedRange.Col2 + 1].Border != null
						&& !vBorders[r, fixedRange.Col2 + 1].Border.Equals(borderInfo.Right))
					{
						borderInfo.NonUniformPos |= ReoGridBorderPos.Right;
						break;
					}
				}
			}
			else
			{
				for (int r = fixedRange.Row + 1; r < fixedRange.Row2; r++)
				{
					if (vBorders[r, fixedRange.Col2 + 1] != null
						&& vBorders[r, fixedRange.Col2 + 1].Border != null)
					{
						borderInfo.NonUniformPos |= ReoGridBorderPos.Right;
						break;
					}
				}
			}

			bool hasSetted = false;

			// inside horizontal
			int hbEndRow = Math.Min(fixedRange.Row2, hBorders.MaxRow);
			int hbEndCol = Math.Min(fixedRange.Col2, hBorders.MaxCol);

			for (int r = fixedRange.Row + 1; r <= hbEndRow; r++)
			{
				for (int c = fixedRange.Col; c <= hbEndCol; )
				{
					ReoGridHBorder hBorder = hBorders[r, c];
					if (hBorder != null)
					{
						if (hBorder.Cols == 0)
						{
							c++;
							// ignore border in merged region
							continue;
						}
						else
							c += hBorder.Cols;

						if (!hasSetted) borderInfo.InsideHorizontal = hBorder.Border;
					}
					else
					{
						c++;
						if (!hasSetted) borderInfo.InsideHorizontal = ReoGridBorderStyle.Empty;
					}

					if (hasSetted)
					{
						if (!((hBorder == null && borderInfo.InsideHorizontal.IsEmpty)
							|| (hBorder != null && !borderInfo.InsideHorizontal.IsEmpty
							&& hBorder.Border == borderInfo.InsideHorizontal)))
						{
							borderInfo.NonUniformPos |= ReoGridBorderPos.InsideHorizontal;
							r = fixedRange.Row2;
							break;
						}
					}

					hasSetted = true;
				}
			}

			hasSetted = false;

			// inside vertical
			int vbEndRow = Math.Min(fixedRange.Row2, vBorders.MaxRow);
			int vbEndCol = Math.Min(fixedRange.Col2, vBorders.MaxCol);

			for (int c = fixedRange.Col + 1; c <= vbEndCol; c++)
			{
				for (int r = fixedRange.Row; r <= vbEndRow; )
				{
					ReoGridVBorder vBorder = vBorders[r, c];
					if (vBorder != null)
					{
						if (vBorder.Rows == 0)
						{
							r++;
							// ignore border in merged region
							continue;
						}
						else
							r += vBorder.Rows;

						if (!hasSetted) borderInfo.InsideVertical = vBorder.Border;
					}
					else
					{
						r++;
						if (!hasSetted) borderInfo.InsideVertical = ReoGridBorderStyle.Empty;
					}

					if (hasSetted)
					{
						if (!((vBorder == null && borderInfo.InsideVertical.IsEmpty)
							|| (vBorder != null && !borderInfo.InsideVertical.IsEmpty
							&& vBorder.Border == borderInfo.InsideVertical)))
						{
							borderInfo.NonUniformPos |= ReoGridBorderPos.InsideVertical;
							c = fixedRange.Col2;
							break;
						}
					}

					hasSetted = true;
				}
			}

			return borderInfo;
		}
		internal ReoGridBorderStyle GetGridBorder(int row, int col, ReoGridBorderPos pos, bool onlyGridOwn)
		{
			if (pos == ReoGridBorderPos.Top
				&& hBorders[row, col] != null
				&& (!onlyGridOwn
				|| (hBorders[row, col].Pos & HBorderOwnerPosition.Top) == HBorderOwnerPosition.Top))
			{
				return hBorders[row, col].Border;
			}
			else if (pos == ReoGridBorderPos.Bottom
				&& hBorders[row + 1, col] != null
				&& (!onlyGridOwn
				|| (hBorders[row + 1, col].Pos & HBorderOwnerPosition.Bottom) == HBorderOwnerPosition.Bottom))
			{
				return hBorders[row + 1, col].Border;
			}
			else if (pos == ReoGridBorderPos.Left
				&& vBorders[row, col] != null
				&& (!onlyGridOwn
				|| (vBorders[row, col].Pos & VBorderOwnerPosition.Left) == VBorderOwnerPosition.Left))
			{
				return vBorders[row, col].Border;
			}
			else if (pos == ReoGridBorderPos.Right
				&& vBorders[row, col + 1] != null
				&& (!onlyGridOwn
				|| (vBorders[row, col + 1].Pos & VBorderOwnerPosition.Right) == VBorderOwnerPosition.Right))
			{
				return vBorders[row, col + 1].Border;
			}
			else
				return ReoGridBorderStyle.Empty;
		}
		#endregion

		#region Internal Border Utilities
		private ReoGridHBorder GetHBorder(int row, int col)
		{
			ReoGridHBorder border = hBorders[row, col];
			if (border == null)
			{
				border = hBorders[row, col] = new ReoGridHBorder();
			}
			return border;
		}
		private ReoGridVBorder GetVBorder(int row, int col)
		{
			ReoGridVBorder border = vBorders[row, col];
			if (border == null)
			{
				border = vBorders[row, col] = new ReoGridVBorder();
			}
			return border;
		}
		internal ReoGridHBorder RetrieveHBorder(int row, int col)
		{
			return hBorders[row, col];
		}
		internal ReoGridVBorder RetrieveVBorder(int row, int col)
		{
			return vBorders[row, col];
		}

		private void IterateHBorder(ReoGridRange range, bool ignoreNull, Func<int, int, ReoGridHBorder, bool> iterator)
		{
			IterateHBorder(range.Row, range.Col, range.Rows, range.Cols, ignoreNull, iterator);
		}
		private void IterateHBorder(int row, int col, int rows, int cols, bool ignoreNull, Func<int, int, ReoGridHBorder, bool> iterator)
		{
			int rend = row + rows;
			int cend = col + cols;
			for (int r = row; r < rend; r++)
			{
				for (int c = 0; c < cend; )
				{
					ReoGridHBorder hBorder = hBorders[r, c];
					if (hBorder == null)
					{
						if (!ignoreNull) if (!iterator(r, c, hBorder)) return;
						c++;
					}
					else if (hBorder.Cols > 0)
					{
						if (!iterator(r, c, hBorder)) return;
						c += hBorder.Cols;
					}
					else c++;
				}
			}
		}
		private void IterateVBorder(int row, int col, int rows, int cols, bool ignoreNull, Func<int, int, ReoGridVBorder, bool> iterator)
		{
			int rend = row + rows;
			int cend = col + cols;
			for (int c = 0; c < cend; c++)
			{
				for (int r = row; r < rend; )
				{
					{
						ReoGridVBorder vBorder = vBorders[r, c];
						if (vBorder == null)
						{
							if (!ignoreNull) iterator(r, c, vBorder);
							r++;
						}
						else if (vBorder.Rows > 0)
						{
							iterator(r, c, vBorder);
							r += vBorder.Rows;
						}
						else r++;
					}
				}
			}
		}

		private void CutBeforeHBorder(int row, int col)
		{
			if (col < 1) return;

			ReoGridHBorder prevBorder = hBorders[row, col - 1];
			if (prevBorder != null && prevBorder.Cols > 1)
			{
				int c = col - 1;
				int offset = prevBorder.Cols;
				while (c >= 0)
				{
					if (hBorders[row, c] == null || hBorders[row, c].Cols <= 1) break;
					hBorders[row, c].Cols -= offset - 1;
					c--;
				}
			}
		}
		private void CutBeforeVBorder(int row, int col)
		{
			if (row < 1) return;

			ReoGridVBorder prevBorder = vBorders[row - 1, col];
			if (prevBorder != null && prevBorder.Rows > 1)
			{
				int r = row - 1;
				int offset = prevBorder.Rows;
				while (r >= 0)
				{
					if (vBorders[r, col] == null || vBorders[r, col].Rows <= 1) break;
					vBorders[r, col].Rows -= offset - 1;
					r--;
				}
			}
		}
		private void SetHBorder(int row, int col, int colspan, ReoGridBorderStyle borderStyle, HBorderOwnerPosition pos)
		{
			if (borderStyle == null)
			{
				// clear border
				hBorders[row, col] = null;
			}
			else
			{
				// set border style
				ReoGridHBorder hBorder = GetHBorder(row, col);
				hBorder.Cols = colspan;
				hBorder.Border = borderStyle;

				// apply border owner pos
				if (row == 0) pos &= ~HBorderOwnerPosition.Bottom;
				if (row == this.rows.Count) pos &= ~HBorderOwnerPosition.Top;
				hBorder.Pos |= pos;
			}
		}
		private void SetVBorder(int row, int col, int rowspan, ReoGridBorderStyle borderStyle, VBorderOwnerPosition pos)
		{
			if (borderStyle == null)
			{
				// clear border
				vBorders[row, col] = null;
			}
			else
			{
				// set border style
				ReoGridVBorder vBorder = GetVBorder(row, col);
				vBorder.Rows = rowspan;
				vBorder.Border = borderStyle;
			
				// apply border owner pos
				if (col == 0) pos &= ~VBorderOwnerPosition.Right;
				if (col == this.cols.Count) pos &= ~VBorderOwnerPosition.Left;
				vBorder.Pos |= pos;
			}
		}
		private int FindSameHBorderLeft(int row, int col, ReoGridBorderStyle borderStyle)
		{
			if (col <= 0) return col;
			for (int c = col - 1; c >= 0; c--)
			{
				if (hBorders[row, c] == null)
					return c + 1;
				else if (hBorders[row, c].Border == null && borderStyle != null)
					return c + 1;
				//else if (hBorders[row, c].Border != null && borderStyle == null)
				//  return c + 1;
				else if (hBorders[row, c].Border != null && !hBorders[row, c].Border.Equals(borderStyle))
					return c + 1;
			}
			return 0;
		}
		private int FindSameHBorderRight(int row, int col, ReoGridBorderStyle borderStyle)
		{
			if (col > this.cols.Count - 1) return col;
			for (int c = col + 1; c < this.cols.Count; c++)
			{
				if (hBorders[row, c] == null)
					return c - 1;
				else if (hBorders[row, c].Border == null && borderStyle != null)
					return c - 1;
				else if (!hBorders[row, c].Border.Equals(borderStyle))
					return c - 1;
			}
			return this.cols.Count - 1;
		}
		private void FillHBorders(int row, int col, int cols, ReoGridBorderStyle borderStyle, HBorderOwnerPosition pos)
		{
			int tc = col + cols;
			for (int c = col; c < tc; c++)
			{
				SetHBorder(row, c, tc - c, borderStyle, pos);
			}
		}
		private void SetHBorders(int row, int col, int cols, ReoGridBorderStyle borderStyle, HBorderOwnerPosition pos)
		{
			// todo: improve performance with back foreach

			int sc = col;
			int ec = col + cols - 1;

			if (borderStyle != null)
			{
				sc = FindSameHBorderLeft(row, col, borderStyle);
				ec = FindSameHBorderRight(row, col + cols - 1, borderStyle);
				cols = ec - sc + 1;
			}
			// find previous
			//	cols += ec - col;

			int c2 = sc + cols;

			// when the cols splitted by a merged range, need find it
			int tc = c2;
			int nextStartCol = -1;
			for (int k = sc; k < c2; k++)
			{
				if (row > 0)
				{
					ReoGridCell cell1 = cells[row - 1, k];
					ReoGridCell cell2 = cells[row, k];
					if (cell1 != null && cell2 != null
						&& !cell1.MergeEndPos.IsEmpty
						&& ReoGridPos.Equals(cell1.MergeStartPos, cell2.MergeStartPos))
					{
						tc = k;
						nextStartCol = cell2.MergeEndPos.Col + 1;
						break;
					}
				}
			}

			FillHBorders(row, sc, tc - sc, borderStyle, pos);

			// if border splitted by merged range, need set the remains
			if (nextStartCol != -1)
				SetHBorders(row, nextStartCol, c2 - nextStartCol, borderStyle, pos);
		}
		private int FindSameVBorderTop(int row, int col, ReoGridBorderStyle borderStyle)
		{
			if (row <= 0) return row;
			for (int r = row - 1; r >= 0; r--)
			{
				if (vBorders[r, col] == null)
					return r + 1;
				else if (vBorders[r, col].Border == null && borderStyle != null)
					return r + 1;
				//else if (vBorders[r, col].Border != null && borderStyle == null)
				//  return r + 1;
				else if (!vBorders[r, col].Border.Equals(borderStyle))
					return r + 1;
			}
			return 0;
		}
		private int FindSameVBorderBottom(int row, int col, ReoGridBorderStyle borderStyle)
		{
			if (row > this.rows.Count - 1) return row;
			for (int r = row + 1; r < this.rows.Count; r++)
			{
				if (vBorders[r, col] == null)
					return r - 1;
				else if (vBorders[r, col].Border == null && borderStyle != null)
					return r - 1;
				else if (!vBorders[r, col].Border.Equals(borderStyle))
					return r - 1;
			}
			return this.rows.Count - 1;
		}
		private void FillVBorders(int row, int col, int rows, ReoGridBorderStyle borderStyle, VBorderOwnerPosition pos)
		{
			int tr = row + rows;
			for (int r = row; r < tr; r++)
			{
				SetVBorder(r, col, tr - r, borderStyle, pos);
			}
		}
		private void SetVBorders(int row, int col, int rows, ReoGridBorderStyle borderStyle, VBorderOwnerPosition pos)
		{
			int sr = row;
			int er = row + rows - 1;

			if (borderStyle != null)
			{
				// find previous
				sr = FindSameVBorderTop(row, col, borderStyle);
				er = FindSameVBorderBottom(row + rows - 1, col, borderStyle);
				rows = er - sr + 1;
			}

			int r2 = sr + rows;

			// when the cols splitted by a merged range, need find it
			int tr = r2;
			int nextStartRow = -1;
			for (int k = sr; k < r2; k++)
			{
				if (col > 0)
				{
					ReoGridCell cell1 = cells[k, col - 1];
					ReoGridCell cell2 = cells[k, col];
					if (cell1 != null && cell2 != null
												&& !cell1.MergeStartPos.IsEmpty
												&& ReoGridPos.Equals(cell1.MergeStartPos, cell2.MergeStartPos))
					{
						tr = k;
						nextStartRow = cell2.MergeEndPos.Row + 1;
						break;
					}
				}
			}

			FillVBorders(sr, col, tr - sr, borderStyle, pos);

			// if border splitted by merged range, set the remains
			if (nextStartRow != -1)
				SetVBorders(nextStartRow, col, r2 - nextStartRow, borderStyle, pos);
		}
		private bool IsBorderSame(params ReoGridAbstractBorder[] borders)
		{
			if (borders.All(b => b == null)) return true;
			if (borders.Any(b => b == null)) return false;

			if (borders.All(b => b.Border == null)) return true;
			if (borders.Any(b => b.Border == null)) return false;

			ReoGridBorderStyle border = borders.First().Border;
			if (borders.Skip(1).All(b => b.Border.Equals(border))) return true;

			return false;
		}
		#endregion

		#endregion

		#region Style
		#region Set Style
		/// <summary>
		/// Set styles to each cells inner specified range
		/// </summary>
		/// <param name="row">number of row</param>
		/// <param name="col">number of col</param>
		/// <param name="rows">number of rows</param>
		/// <param name="cols">number of columns</param>
		/// <param name="style">styles to be set</param>
		public void SetRangeStyle(int row, int col, int rows, int cols, ReoGridRangeStyle style)
		{
			SetRangeStyle(new ReoGridRange(row, col, rows, cols), style);
		}

		/// <summary>
		/// Set styles to each cells inner specified range
		/// </summary>
		/// <param name="range">Specified range to the styles</param>
		/// <param name="style">Styles to be set</param>
		public void SetRangeStyle(ReoGridRange range, ReoGridRangeStyle style)
		{
			ReoGridRange fixedRange = FixRange(range);

			int r1 = fixedRange.Row;
			int c1 = fixedRange.Col;
			int r2 = fixedRange.Row2;
			int c2 = fixedRange.Col2;

			bool isFullColSelected = fixedRange.Rows == this.rows.Count;
			bool isFullRowSelected = fixedRange.Cols == this.cols.Count;
			bool isFullGridSelected = isFullRowSelected && isFullColSelected;

			bool canCreateCell = !isFullColSelected && !isFullRowSelected;

			// update default styles
			if (isFullGridSelected)
			{
				ReoGridStyleUtility.CopyStyle(style, this.rootStyle);

				// remove styles if it is already set in full-row
				for (int r = 0; r < this.rows.Count; r++)
				{
					ReoGridRowHead rowHead = rows[r];

					if (rowHead != null && rowHead.Style != null)
					{
						ReoGridStyleUtility.CopyStyle(style, rowHead.Style);
					}
				}

				// remote styles if it is already setted in full-col
				for (int c = 0; c < this.cols.Count; c++)
				{
					ReoGridColHead colHead = cols[c];

					if (colHead != null && colHead.Style != null)
					{
						ReoGridStyleUtility.CopyStyle(style, colHead.Style);
					}
				}
			}
			else if (isFullRowSelected)
			{
				for (int r = r1; r <= r2; r++)
				{
					if (this.rows[r].Style == null) rows[r].Style = new ReoGridRangeStyle(rootStyle);
					ReoGridStyleUtility.CopyStyle(style, this.rows[r].Style);
				}
			}
			else if (isFullColSelected)
			{
				for (int c = c1; c <= c2; c++)
				{
					if (this.cols[c].Style == null) cols[c].Style = new ReoGridRangeStyle(rootStyle);
					ReoGridStyleUtility.CopyStyle(style, this.cols[c].Style);
				}
			}

			// update cells
			for (int r = r1; r <= r2; r++)
			{
				for (int c = c1; c <= c2; c++)
				{
					ReoGridCell cell = cells[r, c];

					if (cell != null && cell.IsValidCell)
					{
						// if it is not part of merged cell
						if (!cell.IsMergedCell
							// if it is part of merged cell, check whether the target range contains 
							// this entire merged range
							|| (cell.IsMergedCell
							&& r1 <= cell.Row && r2 >= cell.MergeEndPos.Row
							&& c1 <= cell.Col && c2 >= cell.MergeEndPos.Col))
						{
							SetCellStyle(cell, style);
						}
					}
					else
					{
						var rowStyle = this.rows[r].Style;

						// allow to create cells
						if (canCreateCell
							// full column selected but the row of cell has also style,
							// row style has the higher priority than the column style,
							// so it is need to create instance of cell to 
							// get highest priority for cell styles
							|| (isFullColSelected && rowStyle != null))
						{
							SetCellStyle(CreateAndGetCell(r, c), style);
						}
					}

					#region obsoleted since v0.8.5
					//// is part of merged cell
					//if (cell != null && !cell.MergeStartPos.IsEmpty)
					//{
					//	// only set merged cell if selection contains entire merged range
					//	if (cell.Pos.Equals(cell.MergeStartPos)
					//	&& r1 <= cell.MergeStartPos.Row && r2 >= cell.MergeEndPos.Row
					//	&& c1 <= cell.MergeStartPos.Col && c2 >= cell.MergeEndPos.Col)
					//	{
					//		SetCellStyle(cell, style);
					//	}
					//}
					//else if (cell == null)
					//{
					//	if (
					//		// just create cell and set it's style if both full-row and full-col are not selected 
					//	canCreateCell ||
					//		// 
					//		// when this column is fully selected and will be set a style
					//		// if there is a row cross this column and it has been setted a same style, 
					//		// To solve this conflict a cell must be created and style must be copied to the cell.
					//		// (the styles of cell have the most high level priority)
					//	(!isFullRowSelected && this.rows[r].Style != null && this.cols[c].IsFullColSelected)
					//		//|| (this.cols[r].Style != null && this.cols[c].IsFullColSelected)
					//	)
					//	{
					//		SetCellStyle(r, c, style);
					//	}
					//}
					//else
					//{
					//	SetCellStyle(r, c, style);
					//}
					#endregion
				}
			}

			if (RangeStyleChanged != null)
			{
				RangeStyleChanged(this, new RGRangeEventArgs(fixedRange));
			}

			InvalidateCanvas();
		}
		internal void RemoveRangeStyle(ReoGridRange range, ReoGridRangeStyle style)
		{
			RemoveRangeStyle(range, style.Flag);
		}
		/// <summary>
		/// Remove specified style from every cells in a range. 
		/// </summary>
		/// <param name="range">The range contains target cells</param>
		/// <param name="flag">Style specified with StyleFlag will be removed</param>
		public void RemoveRangeStyle(ReoGridRange range, PlainStyleFlag flag)
		{
			ReoGridRange fixedRange = FixRange(range);

			int r1 = fixedRange.Row;
			int c1 = fixedRange.Col;
			int r2 = fixedRange.Row2;
			int c2 = fixedRange.Col2;

			bool isFullColSelected = fixedRange.Rows == this.rows.Count;
			bool isFullRowSelected = fixedRange.Cols == this.cols.Count;
			bool isFullGridSelected = isFullRowSelected && isFullColSelected;

			bool canCreateCell = !isFullColSelected && !isFullRowSelected;

			// update default styles
			if (isFullGridSelected)
			{
				this.rootStyle.Flag &= ~flag;

				// remote styles if it is already setted in full-row
				for (int r = 0; r < this.rows.Count; r++)
				{
					rows[r].Style.Flag &= ~flag;
				}

				// remote styles if it is already setted in full-col
				for (int c = 0; c < this.cols.Count; c++)
				{
					cols[c].Style.Flag &= ~flag;
				}
			}
			else if (isFullRowSelected)
			{
				for (int r = r1; r <= r2; r++)
				{
					this.rows[r].Style.Flag &= ~flag;
				}
			}
			else if (isFullColSelected)
			{
				for (int c = c1; c <= c2; c++)
				{
					this.cols[c].Style.Flag &= ~flag;
				}
			}

			// update cells
			for (int r = r1; r <= r2; r++)
			{
				for (int c = c1; c <= c2; c++)
				{
					ReoGridCell cell = cells[r, c];

					if (cell != null && !cell.MergeStartPos.IsEmpty)
					{
						// only set merged cell if selection contains the merged entire range
						if (cell.Pos.Equals(cell.MergeStartPos)
						&& r1 <= cell.MergeStartPos.Row && r2 >= cell.MergeEndPos.Row
						&& c1 <= cell.MergeStartPos.Col && c2 >= cell.MergeEndPos.Col)
						{
							ReoGridStyleUtility.CopyStyle(
								ReoGridStyleUtility.GetDefaultStyleIgnoreCell(this, r, c), cell.Style, flag);
							//cell.Style.Flag &= ~flag;
						}
					}
					else
					{
						if (cell == null || cell.Style == null)
						{
							bool currentColFullSelected = (selectionMode == ReoGridSelectionMode.Range
								&& selectionRange.Rows == this.rows.Count
								&& selectionRange.ContainsColumn(c));

							if (
								// just create cell and set it's style if both full-row and full-col are not selected 
							canCreateCell ||
								// 
								// when this column is fully selected and will be set a style
								// if there is a row cross this column and it has been set a same style, 
								// for solve this conflict need create a cell then set the cell's style 
								// (the cell's style has highest Priority on getting cell's style)
							(this.rows[r].Style.Flag & flag) > 0
							&& currentColFullSelected)
							{
								cell = CreateAndGetCell(r, c);
								//cell.Style = StyleGridStyleUtility.CreateDefaultGridStyle(this, r, c);

								// maybe no need this
								cell.Style.Flag &= ~flag;
							}

							// Known Bug:
							// 1. Set fill color 'SkyBlue' to entire grid
							// 2. Remove style from range(2,2,3,3) 
							// 3. styles in the range cannot be removed.
						}
						else
						{
							//cell.Style.Flag &= ~flag;
							ReoGridStyleUtility.CopyStyle(
								ReoGridStyleUtility.GetDefaultStyleIgnoreCell(this, r, c), cell.Style, flag);

							//if (cell.Style.Flag == PlainStyleFlag.None) cell.Style = null;
						}
					}
				}
			}

			InvalidateCanvas();
		}
		internal void SetCellStyle(ReoGridPos pos, ReoGridRangeStyle style)
		{
			SetCellStyle(pos.Row, pos.Col, style);
		}
		/// <summary>
		/// Set style to cell specified by row and col index
		/// </summary>
		/// <param name="row">index to row</param>
		/// <param name="col">index to col</param>
		/// <param name="style">style will be copied</param>
		internal void SetCellStyle(int row, int col, ReoGridRangeStyle style)
		{
			SetCellStyle(CreateAndGetCell(row, col), style);
		}
		private void SetCellStyle(ReoGridCell cell, ReoGridRangeStyle style)
		{
			// do nothing if cell is a part of merged range
			if (cell.Rowspan == 0 || cell.Colspan == 0) return;

			ReoGridStyleUtility.CopyStyle(style, cell.Style);

			// auto remove fill pattern when pattern color is empty
			if (cell.Style.FillPatternColor.IsEmpty)
			{
				ReoGridStyleUtility.RemoveStyle(cell.Style, PlainStyleFlag.FillPattern);
			}

			// auto remove background color when backcolor is empty
			if (cell.Style.BackColor.IsEmpty)
			{
				ReoGridStyleUtility.RemoveStyle(cell.Style, PlainStyleFlag.FillColor);
			}

			// update render text align when data format changed
			ReoGridStyleUtility.UpdateCellRenderAlign(this, cell);

			// when font changed, cell's scaled font need be updated.
			if (style.Flag.HasAny(PlainStyleFlag.FontAll))
			{
				// update cell font and text's bounds
				UpdateCellFont(cell, null, true);
			}
			// when font is not changed but alignment is changed, only update the bounds of text
			else if (style.Flag.HasAny(PlainStyleFlag.HorizontalAlign
				| PlainStyleFlag.VerticalAlign 
				| PlainStyleFlag.TextWrap))
			{
				UpdateCellTextBounds(null, cell, this.rows[cell.Row].IsAutoHeight);
			}

			if (style.Flag.Has(PlainStyleFlag.Padding))
			{
				cell.UpdateContentBounds();
			}
		
			InvalidateCanvas();
		}

		/// <summary>
		/// Event raised on style of range changed
		/// </summary>
		public event EventHandler<RGRangeEventArgs> RangeStyleChanged;
		#endregion

		#region Get Style
		/// <summary>
		/// Get style of specified range
		/// </summary>
		/// <param name="range">The range to get style</param>
		/// <returns>Style info of specified range</returns>
		public ReoGridRangeStyle GetRangeStyle(ReoGridRange range)
		{
			ReoGridRange fixedRange = FixRange(range);

			int r1 = fixedRange.Row;
			int c1 = fixedRange.Col;
			int r2 = fixedRange.Row2;
			int c2 = fixedRange.Col2;

			return GetCellStyle(range.StartPos);
		}

		//internal ReoGridRangeStyle GetCellStyle(ReoGridCell cell)
		//{
		//	return return cell.Style;
		//}
		/// <summary>
		/// Get style of single cell
		/// </summary>
		/// <param name="pos">Position of cell to get</param>
		/// <returns>Style of cell in the specified position</returns>
		public ReoGridRangeStyle GetCellStyle(ReoGridPos pos)
		{
			return GetCellStyle(pos.Row, pos.Col);
		}
		/// <summary>
		/// Get style of single cell
		/// </summary>
		/// <param name="row">Index of row of specified cell</param>
		/// <param name="col">Index of column of specified cell</param>
		/// <returns>Style of cell in ths specified position</returns>
		public ReoGridRangeStyle GetCellStyle(int row, int col)
		{
			ReoGridCell cell = cells[row, col];
			if (cell == null)
				return ReoGridStyleUtility.GetDefaultStyleIgnoreCell(this, row, col);
			else
				return cell.Style;
		}
		#endregion

		#region Internal Cell Updating
		private void UpdateCellBounds(ReoGridCell cell)
		{
#if DEBUG
			Debug.Assert(cell.Rowspan >= 1 && cell.Colspan >= 1);
#else
			if (cell.Rowspan < 1 || cell.Colspan < 1) return;
#endif
			cell.Bounds = GetRangeBounds(cell.Row, cell.Col, cell.Rowspan, cell.Colspan);
			UpdateCellTextBounds(null, cell, false);
		}
		private void UpdateCellFont(ReoGridCell cell, Graphics g, bool updateRowHeight)
		{
			string fontName = cell.Style.FontName;

			FontStyle fontStyle = FontStyle.Regular;
			if (cell.Style.Bold) fontStyle |= FontStyle.Bold;
			if (cell.Style.Italic) fontStyle |= FontStyle.Italic;
			if (cell.Style.Strikethrough) fontStyle |= FontStyle.Strikeout;
			if (cell.Style.Underline) fontStyle |= FontStyle.Underline;

			// bug: sometimes happen
			// cell.Style is null (cell.Style.FontSize is zero)
			if (cell.Style.FontSize <= 0) cell.Style.FontSize = 6f;

			float fontSize = (float)Math.Round(cell.Style.FontSize * scaleFactor, 1);
			cell.ScaledFont = ResourcePoolManager.Instance.GetFont(fontName, fontSize, fontStyle);
			
			// update font name
			cell.Style.FontName = cell.ScaledFont.FontFamily.Name;

			UpdateCellTextBounds(g, cell, updateRowHeight);
		}
		/// <summary>
		/// Update cell text bounds. 
		/// need to call this method when content of cell is changed, contains styles like align, font, etc.
		/// 
		/// if cell's display property is null, this method does nothing.
		/// </summary>
		/// <param name="g">The graphics device used to calculate bounds. Null to use default graphic device.</param>
		/// <param name="cell">The target cell will be updated.</param>
		internal void UpdateCellTextBounds(Graphics g, ReoGridCell cell, bool updateRowHeight)
		{
			if (cell == null || string.IsNullOrEmpty(cell.Display)) return;

			if (cell.StringFormat == null) cell.StringFormat = new StringFormat(StringFormat.GenericTypographic);
			StringFormat sf = cell.StringFormat;

			sf.FormatFlags |= StringFormatFlags.MeasureTrailingSpaces | StringFormatFlags.LineLimit;

			SizeF size = cell.Bounds.Size;

			bool graphicsCreated = false;

			if (g == null)
			{
				graphicsCreated = true;
				g = Graphics.FromHwnd(this.Handle);
			}

			ReoGridRangeStyle style = cell.Style;

			int fieldWidth = 0;

			if (!cell.IsMergedCell 
				&& cell.Style.TextWrapMode == TextWrapMode.NoWrap)
			{
				fieldWidth = 9999999; // TODO: unsafe magic number
				
				sf.FormatFlags |= StringFormatFlags.NoWrap;
			}
			else
			{
				fieldWidth = (int)Math.Round(cell.Bounds.Width * this.scaleFactor - 1);

				sf.FormatFlags &= ~StringFormatFlags.NoWrap;
			}

			size = g.MeasureString(cell.Display, cell.ScaledFont, fieldWidth, sf);

			// TODO: need fix: get incorrect size if CJK fonts
			size.Height += 2;
			
			if (graphicsCreated) g.Dispose();

			if (cell.Style.HAlign == ReoGridHorAlign.DistributedIndent)
			{
				cell.DistributedIndentSpacing = ((cell.Width - 3 - size.Width) / (cell.Display.Length - 1));
				size.Width = cell.Width;
			}

			float x = 0;
			float y = 0;

			switch (cell.RenderHorAlign)
			{
				default:
				case ReoGridRenderHorAlign.Left:
					//case StyleGridHorAlign.DistributedIndent:
					cell.RenderHorAlign = ReoGridRenderHorAlign.Left;
					sf.Alignment = StringAlignment.Near;
					x = cell.Left * scaleFactor + 2;
					break;

				case ReoGridRenderHorAlign.Center:
					cell.RenderHorAlign = ReoGridRenderHorAlign.Center;
					sf.Alignment = StringAlignment.Center;
					x = (cell.Left * scaleFactor + cell.Width * scaleFactor / 2 - size.Width / 2);
					break;

				case ReoGridRenderHorAlign.Right:
					cell.RenderHorAlign = ReoGridRenderHorAlign.Right;
					sf.Alignment = StringAlignment.Far;
					x = cell.Right * scaleFactor - 3 - size.Width;
					break;
			}

			switch (cell.Style.VAlign)
			{
				case ReoGridVerAlign.Top:
					sf.LineAlignment = StringAlignment.Near;
					y = cell.Top * scaleFactor + 1;
					break;

				default:
				case ReoGridVerAlign.Middle:
					sf.LineAlignment = StringAlignment.Center;
					y = (cell.Top * scaleFactor + (cell.Height * scaleFactor) / 2 - (size.Height) / 2);
					break;

				case ReoGridVerAlign.Bottom:
					sf.LineAlignment = StringAlignment.Far;
					y = cell.Bottom * scaleFactor - 1 - size.Height;
					break;
			}

			cell.TextBounds = new RectangleF(x, y, size.Width, size.Height);

			// update cell text column span
#if ALLOW_CELL_OVERLAY
			if (cell.Rowspan == 1 && cell.Colspan == 1)
			{
				if (cell.TextBounds.Width > cell.Width)
				{
					int right = cell.Col;
					int left = cell.Col;

					switch (cell.RenderHorAlign)
					{
						case ReoGridRenderHorAlign.Left:
							while (right < this.cols.Count && cell.TextBounds.Right > this.cols[right].Right) right++;
							break;

						case ReoGridRenderHorAlign.Center:
							break;

						case ReoGridRenderHorAlign.Right:
							while (left > 0 && cell.TextBounds.Left < this.cols[left].Left) left--;
							break;
					}

					cell.RenderTextColumnLeftSpan = (short)(cell.Col - left);
					cell.RenderTextColumnRightSpan = (short)(right);
				}
			}
#endif

			if (updateRowHeight) UpdateRowHeightToFitCell(cell);
		}
		#endregion

		private bool UpdateRowHeightToFitCell(ReoGridCell cell)
		{
			if (!HasSettings(ReoGridSettings.Edit_AutoAdjustRowHeight)) return false;

			// if cell is empty or cross over 2 rows then do nothing
			if (cell.Rowspan != 1) return false;

			ReoGridRowHead rowHead = this.rows[cell.Row];
			if (HasSettings(ReoGridSettings.Edit_AutoAdjustRowHeight) 
				&& rowHead.IsAutoHeight && !string.IsNullOrEmpty(cell.Display))
			{
				if (cell.Height < cell.TextBounds.Height / scaleFactor)
				{
					int height = (int)Math.Round(cell.TextBounds.Height / scaleFactor);
					if (height < 0) height = 0;
					SetRowsHeight(cell.Row, 1, (ushort)height);
					return true;
				}
			}

			return false;
		}
		/// <summary>
		/// Make the text of cells in specified range larger or smaller.
		/// </summary>
		/// <param name="range">The spcified range</param>
		/// <param name="stepHandler">Iterator callback to handle how to make text larger or smaller</param>
		public void StepRangeFont(ReoGridRange range, Func<float, float> stepHandler)
		{
			ReoGridRange fixedRange = FixRange(range);

			int r1 = fixedRange.Row;
			int c1 = fixedRange.Col;
			int r2 = fixedRange.Row2;
			int c2 = fixedRange.Col2;

			using (Graphics g = Graphics.FromHwnd(this.Handle))
			{
				for (int r = r1; r <= r2; r++)
				{
					for (int c = c1; c <= c2; )
					{
						ReoGridCell cell = cells[r, c];
						if (cell != null && cell.Colspan > 0 && cell.Rowspan > 0)
						{
							cell.Style.FontSize = stepHandler(cell.Style.FontSize);
							UpdateCellFont(cell, g, true);
							c += cell.Colspan;
						}
						else c++;
					}
				}
			}

			InvalidateCanvas();
		}

		/// <summary>
		/// Start to pick a range and copy the style from selected range.
		/// </summary>
		public void StartPickRangeAndCopyStyle()
		{
			ReoGridRange fromRange = selectionRange;

			PickRange((grid, range) =>
			{
				ReoGridCell fromCell = GetCell(fromRange.Row, fromRange.Col);

				if (fromCell != null && fromCell.Style != null)
				{
					ReoGridCell toCell = CreateAndGetCell(range.StartPos);

					// todo: copy and merge targer range
					RGReusableActionGroup actionGroup = new RGReusableActionGroup(range);
					actionGroup.Actions.Add(new RGSetRangeStyleAction(range, fromCell.Style));
					actionGroup.Actions.Add(new RGSetRangeDataFormatAction(range, fromCell.DataFormat, fromCell.DataFormatArgs));

					DoAction(actionGroup);
				}

				return !Toolkit.IsKeyDown(Win32.VKey.VK_CONTROL);
			}, defaultPickRangeCursor);

		}

		//private static void ProcessGridToGrid(PartialGridData fromGrid, ReoGridRange fromRange,
		//	PartialGridData toGrid, ReoGridRange toRange)
		//{

		//}

		private ReoGridRangeStyle rootStyle = null;
		internal ReoGridRangeStyle RootStyle
		{
			get { return rootStyle; }
			set { rootStyle = value; }
		}

		#region Control Style
		private ReoGridControlStyle controlStyle;

		/// <summary>
		/// Control Style Settings
		/// </summary>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public ReoGridControlStyle ControlStyle
		{
			get { return controlStyle; }
			set { SetControlStyle(value); }
		}
		/// <summary>
		/// Set the style of grid control.
		/// </summary>
		/// <param name="controlStyle"></param>
		public void SetControlStyle(ReoGridControlStyle controlStyle)
		{
			this.controlStyle = controlStyle;
			if (controlStyle.Colors.ContainsKey(ReoGridControlColors.GridBackground))
			{
				BackColor = Color.FromArgb(255, controlStyle.Colors[ReoGridControlColors.GridBackground]);
			}
			if (controlStyle.Colors.ContainsKey(ReoGridControlColors.GridText))
			{
				rootStyle.TextColor = controlStyle.Colors[ReoGridControlColors.GridText];
			}
			InvalidateCanvas();
		}
		/// <summary>
		/// Create default style for grid control.
		/// </summary>
		/// <returns>Default style created</returns>
		public ReoGridControlStyle CreateDefaultControlStyle()
		{
			ReoGridControlStyle cs = new ReoGridControlStyle();
			cs.Colors[ReoGridControlColors.LeadHeadNormal] = Color.Lavender;
			cs.Colors[ReoGridControlColors.LeadHeadSelected] = Color.Lavender;
			cs.Colors[ReoGridControlColors.LeadHeadIndicatorStart] = Color.Gainsboro;
			cs.Colors[ReoGridControlColors.LeadHeadIndicatorEnd] = Color.Silver;
			cs.Colors[ReoGridControlColors.ColHeadSplitter] = Color.LightSteelBlue;
			cs.Colors[ReoGridControlColors.ColHeadNormalStart] = Color.White;
			cs.Colors[ReoGridControlColors.ColHeadNormalEnd] = Color.Lavender;
			cs.Colors[ReoGridControlColors.ColHeadHoverStart] = Color.LightGoldenrodYellow;
			cs.Colors[ReoGridControlColors.ColHeadHoverEnd] = Color.Goldenrod;
			cs.Colors[ReoGridControlColors.ColHeadSelectedStart] = Color.LightGoldenrodYellow;
			cs.Colors[ReoGridControlColors.ColHeadSelectedEnd] = Color.Goldenrod;
			cs.Colors[ReoGridControlColors.ColHeadFullSelectedStart] = Color.WhiteSmoke;
			cs.Colors[ReoGridControlColors.ColHeadFullSelectedEnd] = Color.LemonChiffon;
			cs.Colors[ReoGridControlColors.ColHeadText] = Color.DarkBlue;
			cs.Colors[ReoGridControlColors.RowHeadSplitter] = Color.LightSteelBlue;
			cs.Colors[ReoGridControlColors.RowHeadNormal] = Color.AliceBlue;
			cs.Colors[ReoGridControlColors.RowHeadHover] = Color.LightSteelBlue;
			cs.Colors[ReoGridControlColors.RowHeadSelected] = Color.PaleGoldenrod;
			cs.Colors[ReoGridControlColors.RowHeadFullSelected] = Color.LemonChiffon;
			cs.Colors[ReoGridControlColors.RowHeadText] = Color.DarkBlue;
			cs.Colors[ReoGridControlColors.GridText] = Color.Black;
			cs.Colors[ReoGridControlColors.GridBackground] = Color.White;
			cs.Colors[ReoGridControlColors.GridLine] = Color.FromArgb(208, 215, 229);
			cs.Colors[ReoGridControlColors.SelectionBorder] = Color.FromArgb(180, SystemColors.Highlight);
			cs.Colors[ReoGridControlColors.SelectionFill] = Color.FromArgb(30, SystemColors.Highlight);
			cs.Colors[ReoGridControlColors.OutlineButtonBorder] = Color.Black;
			cs.Colors[ReoGridControlColors.OutlinePanelBackground] = SystemColors.Control;
			cs.Colors[ReoGridControlColors.OutlinePanelBorder] = Color.Silver;
			cs.Colors[ReoGridControlColors.OutlineButtonText] = SystemColors.WindowText;
			return cs;
		}
		#endregion
		#endregion

		#region Cursor & Caret
		private Cursor gridSelectCursor = null;
		private Cursor fullColSelectCursor = null;
		private Cursor fullRowSelectCursor = null;

		internal virtual void MoveSelectionForward()
		{
			if (SelectionMovedForward != null)
			{
				var arg = new RGSelectionMoveForwardEventArgs();
				SelectionMovedForward(this, arg);
				if (arg.IsCancelled)
				{
					return;
				}
			}

#if EX_SCRIPT
			var scriptReturn = RaiseScriptEvent("onnextfocus");
			if (scriptReturn != null && !ScriptRunningMachine.GetBoolValue(scriptReturn))
			{
				return;
			}
#endif

			switch (selectionForwardDirection)
			{
				case ReoGrid.SelectionForwardDirection.Right:
					{
						if (selEnd.Col < this.cols.Count - 1)
						{
							MoveSelectionRight();
						}
						else
						{
							if (selEnd.Row < this.rows.Count - 1)
							{
								// TODO: skip hidden columns
								selEnd.Col = 0;
								MoveSelectionDown();
							}
						}
					}
					break;

				case ReoGrid.SelectionForwardDirection.Down:
					{
						if (selEnd.Row < this.rows.Count - 1)
						{
							MoveSelectionDown();
						}
						else
						{
							if (selEnd.Col < this.cols.Count - 1)
							{
								// TODO: skip hidden rows
								selEnd.Row = 0;
								MoveSelectionRight();
							}
						}
					}
					break;
			}

			

		}

		/// <summary>
		/// Event raised when focus-selection will move to next position. 
		/// Change 'TargetPos' property of this EventArgs to change the next
		/// position of focus-selection.
		/// </summary>
		public event EventHandler<RGSelectionMoveForwardEventArgs> SelectionMovedForward;

		internal void MoveSelectionUp()
		{
			// TODO: skip hidden rows
			if (selEnd.Row > 0)
			{
				//var current = RowToRangeTop(selEnd);
				//var r = current;

				//if (this.rows[r].Height > 0) 
				//	r--;
				//else if (r > 0)
				//{
				//	while (this.rows[r].Height <= 0)
				//	{
				//		r--;

				//		if (r == 0)
				//		{
				//			r = current;
				//			break;
				//		}
				//	}
				//}

				//selEnd.Row = r;
				
				selEnd.Row = RowToRangeTop(selEnd) - 1;
				selOriginal.Row = selEnd.Row;
				FocusPos = selOriginal;
				SelectRangeByPos(selEnd, selEnd);
				ScrollToFocusCell();
				InvalidateCanvas();
			}
		}
		internal void MoveSelectionDown()
		{
			// TODO: skip hidden rows
			if (selEnd.Row < this.rows.Count - 1)
			{
				selEnd.Row = RowToRangeBottom(selEnd) + 1;
				selOriginal.Row = selEnd.Row;
				FocusPos = selOriginal;
				SelectRangeByPos(selEnd, selEnd);
				ScrollToFocusCell();
				InvalidateCanvas();
			}
		}
		internal void MoveSelectionLeft()
		{
			// TODO: skip hidden columns
			if (selEnd.Col > 0)
			{
				selEnd.Col = ColToRangeLeft(selEnd) - 1;
				selOriginal.Col = selEnd.Col;
				FocusPos = selOriginal;
				SelectRangeByPos(selEnd, selEnd);
				ScrollToFocusCell();
				InvalidateCanvas();
			}
		}
		internal void MoveSelectionRight()
		{
			// TODO: skip hidden columns
			if (selEnd.Col < this.cols.Count - 1)
			{
				selEnd.Col = ColToRangeRight(selEnd) + 1;
				selOriginal.Col = selEnd.Col;
				FocusPos = selOriginal;
				SelectRangeByPos(selEnd, selEnd);
				ScrollToFocusCell();
				InvalidateCanvas();
			}
		}
		internal void ScrollToFocusCell()
		{
			// TODO: scroll to cell when range selected 
			//       unless the entire row or columns is selected.

			//RectangleF rect = GetCellBounds(selEnd);

			//float x = rect.Left * scaleFactor;
			//float y = rect.Top * scaleFactor;
			//float x2 = rect.Right * scaleFactor;
			//float y2 = rect.Bottom * scaleFactor;

			//bool isMoved = false;

			//if (viewing.Y > y)
			//{
			//  if (y < 0) y = 0;
			//  viewing.Y = (vScrollBar.Value = (int)y);
			//  isMoved = true;
			//}

			//if (viewing.X > x)
			//{
			//  if (x < 0) x = 0;
			//  viewing.X = (hScrollBar.Value = (int)x);
			//  isMoved = true;
			//}

			//if (viewing.Bottom < y2)
			//{
			//  int vy = (int)(y2 - sheetBounds.Height);
			//  if (vy < 0) vy = 0;
			//  vScrollBar.Value = vy;
			//  viewing.Y = vy;
			//  isMoved = true;
			//}

			//if (viewing.Right < x2)
			//{
			//  int hx = (int)(x2 - sheetBounds.Width);
			//  if (hx < 0) hx = 0;
			//  hScrollBar.Value = hx;
			//  viewing.X = hx;
			//  isMoved = true;
			//}

			//if (isMoved)
			//{
			//  UpdateVisibleGridRegion();
			//  InvalidateCanvas();
			//}
		}
		internal void StepApplyRangeSelection(bool isMoveToCell)
		{
			SelectRangeByPos(selOriginal.Row, selOriginal.Col, selEnd.Row, selEnd.Col);
			if (isMoveToCell) ScrollToFocusCell();
			InvalidateCanvas();
		}
		#endregion

		#region Undo & Redo
		/// <summary>
		/// Determine whether there is any action can perform undo.
		/// </summary>
		/// <returns>True if there is action can perform undo</returns>
		public bool CanUndo()
		{
			return ActionManager.InstanceForObject(this).CanUndo();
		}
		/// <summary>
		/// Determine whether there is any action can perform redo.
		/// </summary>
		/// <returns>True if there is action can perform redo</returns>
		public bool CanRedo()
		{
			return ActionManager.InstanceForObject(this).CanRedo();
		}
		/// <summary>
		/// Undo the last editing action. If the action modified a range, 
		/// the range will be selected automatically.
		/// </summary>
		public void Undo()
		{
			if (IsEditing)
			{
				EndEdit(ReoGridEndEditReason.NormalFinish);
			}

			var action = ActionManager.InstanceForObject(this).Undo();
			if (action is RGReusableAction)
			{
				var reusableAction = (RGReusableAction)action;
				SelectRange(reusableAction.Range);
			}
			InvalidateCanvas();
			if (Undid != null) Undid(this, new RGActionPerformedEventArgs(action));
		}
		/// <summary>
		/// Redo the last editing action. If the action modified a range, 
		/// the range will be selected automatically.
		/// </summary>
		public void Redo()
		{
			var action = ActionManager.InstanceForObject(this).Redo();
			if (action is RGReusableAction)
			{
				var reusableAction = (RGReusableAction)action;
				SelectRange(reusableAction.Range);
			}
			InvalidateCanvas();
			if (Redid != null) Redid(this, new RGActionPerformedEventArgs(action));
		}
		private RGReusableAction lastReusableAction;
		internal void RepeatLastAction()
		{
			RepeatLastAction(selectionRange);
		}
		/// <summary>
		/// Apply last action to another specified range. Only actions that inherit from
		/// RGReusableAction are supported.
		/// </summary>
		/// <param name="range"></param>
		public void RepeatLastAction(ReoGridRange range)
		{
			ActionManager instance = ActionManager.InstanceForObject(this);

			if (instance.CanRedo())
			{
				instance.Redo();
			}
			else
			{
				if (lastReusableAction != null)
				{
					RGAction newAction = lastReusableAction.Clone(range);
					DoAction(newAction);

					InvalidateCanvas();
				}
			}
		}
		/// <summary>Perform action. Create action and pass as argument into this method to perform the action.</summary>
		/// <example>
		/// ReoGrid uses ActionManager, one lightweight framework provided by unvell, 
		/// to implement the Redo/Undo/Repeat operations.
		/// 
		/// If you want an operation could be undid, create instances of the action,
		/// pass the instance as argument to call this method.
		/// 
		/// To make many independent actions undid in once time, create instances 
		/// for action and add them into one RGActionGroup, then pass this 
		/// RGActionGroup as parameter to call this method. eg.:
		///   
		///   RGActionGroup ag = new RGActionGroup();
		///   ag.Actions.Add(new ...Action());
		///   DoAction(ag);
		/// </example>
		/// <seealso cref="ActionManager"/>
		/// <seealso cref="ActionGroup"/>
		/// <seealso cref="RGAction"/>
		/// <param name="action">action to be performed</param>
		public void DoAction(RGAction action)
		{
			// no need to check this
			//if (action.Grid != null && action.Grid != this)
			//	throw new RGOtherGridsActionException(action);

			action.Grid = this;
			ActionManager.DoAction(this, action);
			if (ActionPerformed != null) ActionPerformed(this, new RGActionPerformedEventArgs(action));
			InvalidateCanvas();
		}

		/// <summary>
		/// Event fired when any action performed.
		/// </summary>
		public event EventHandler<RGActionPerformedEventArgs> ActionPerformed;
		/// <summary>
		/// Event fired when Undo operation performed by user.
		/// </summary>
		public event EventHandler<RGActionPerformedEventArgs> Undid;
		/// <summary>
		/// Event fired when Reod operation performed by user.
		/// </summary>
		public event EventHandler<RGActionPerformedEventArgs> Redid;

		#endregion

		#region Copy & Cut & Paste
		private static readonly string ClipBoardDataFormatIdentify = "{CB3BE3D1-2BF9-4fa6-9B35-374F6A0412CE}";

		private ReoGridRange currentCopingRange = ReoGridRange.Empty;

		/// <summary>
		/// Copy data and put into Clipboard.
		/// </summary>
		public virtual bool Copy()
		{
			if (IsEditing)
			{
				this.editTextbox.Copy();
			}
			else
			{
				Cursor = Cursors.WaitCursor;

				try
				{
					if (BeforeCopy != null)
					{
						var evtArg = new RGBeforeRangeOperationEventArgs(selectionRange);
						BeforeCopy(this, evtArg);
						if (evtArg.IsCancelled)
						{
							return false;
						}
					}

#if EX_SCRIPT
					var scriptReturn = RaiseScriptEvent("oncopy");
					if (scriptReturn != null && !ScriptRunningMachine.GetBoolValue(scriptReturn))
					{
						return false;
					}
#endif // EX_SCRIPT

					// highlight current copy range
					currentCopingRange = selectionRange;

					DataObject data = new DataObject();
					data.SetData(ClipBoardDataFormatIdentify, GetPartialGrid(currentCopingRange));

					string text = StringifyRange(currentCopingRange);
					if (!string.IsNullOrEmpty(text)) data.SetText(text);

					// set object data into clipboard
					Clipboard.SetDataObject(data);
				}
				finally
				{
					Cursor = gridSelectCursor;
				}

				if (AfterCopy != null)
				{
					AfterCopy(this, new RGRangeEventArgs(this.selectionRange));
				}
			}

			return true;
		}

		/// <summary>
		/// Convert all data inside range to string
		/// </summary>
		/// <param name="range">The range will be converted</param>
		/// <returns>String of data converted from specified range</returns>
		public string StringifyRange(ReoGridRange range)
		{
			int erow = currentCopingRange.Row2;
			int ecol = currentCopingRange.Col2;
			
			// copy plain text
			StringBuilder sb = new StringBuilder();

			bool isFirst = true;
			for (int r = currentCopingRange.Row; r <= erow; r++)
			{
				if (isFirst) isFirst = false; else sb.Append('\n');

				bool isFirst2 = true;
				for (int c = currentCopingRange.Col; c <= ecol; c++)
				{
					if (isFirst2) isFirst2 = false; else sb.Append('\t');
					sb.Append(GetCellData(r, c));
				}
			}

			return sb.ToString();
		}

		/// <summary>
		/// Paste data from formmatted string into grid
		/// </summary>
		/// <param name="startPos">Start position to put data</param>
		/// <param name="str">Formatted string to be copied</param>
		/// <returns>Result range</returns>
		public ReoGridRange PasteFromString(ReoGridPos startPos, string str)
		{
			int rows = 0, cols = 0;

			string[] lines = str.Split(new string[] { "\r\n" }, StringSplitOptions.None);
			for (int r = 0; r < lines.Length; r++)
			{
				string line = lines[r];
				if (line.EndsWith("\n")) line = line.Substring(0, line.Length - 1);
				//line = line.Trim();

				if (line.Length > 0)
				{
					string[] tabs = line.Split('\t');
					cols = Math.Max(cols, tabs.Length);
					for (int c = 0; c < tabs.Length; c++)
					{
						int toRow = selectionRange.Row + r;
						int toCol = selectionRange.Col + c;

						if (!this.IsValidCell(toRow, toCol))
						{
							throw new RangeIntersectionException(new ReoGridRange(toRow, toCol, 1, 1));
						}

						string text = tabs[c];
						if (text.StartsWith("\"")) text = text.Substring(1);
						if (text.EndsWith("\"")) text = text.Substring(0, text.Length - 1);

						SetCellData(toRow, toCol, text);
					}
					rows++;
				}
			}

			return new ReoGridRange(startPos.Row, startPos.Col, rows, cols);
		}

		/// <summary>
		/// Copy data from Clipboard and put on grid.
		/// 
		/// Currently ReoGrid supports the following types of source from the clipboard.
		///  - Data from another ReoGrid instance
		///  - Plain/Unicode Text from any Windows Application.
		///  - Tabbed Plain/Unicode Data from Excel or similar application.
		/// 
		/// When data copied from another ReoGrid instance, and the destination range 
		/// is bigger than the source, ReoGrid will try to repeat putting data to fill 
		/// the entire destination range.
		/// 
		/// Todo: Copy border and cell style from Excel.
		/// </summary>
		public virtual bool Paste()
		{
			if (IsEditing)
			{
				editTextbox.Paste();
			}
			else
			{
				// Paste method will always perform action to do paste

				// do nothing if in readonly mode
				if (HasSettings(ReoGridSettings.Readonly)
					// or selection is empty
					|| this.selectionRange.IsEmpty)
				{
					return false;
				}

				if (BeforePaste != null)
				{
					var evtArg = new RGBeforeRangeOperationEventArgs(this.selectionRange);
					BeforePaste(this, evtArg);
					if (evtArg.IsCancelled)
					{
						return true;
					}
				}

#if EX_SCRIPT
				object scriptReturn = RaiseScriptEvent("onpaste");
				if (scriptReturn != null && !ScriptRunningMachine.GetBoolValue(scriptReturn))
				{
					return true;
				}
#endif // EX_SCRIPT

				try
				{
					Cursor = Cursors.WaitCursor;

					DataObject data = Clipboard.GetDataObject() as DataObject;
					if (data != null)
					{
						PartialGrid partialGrid = data.GetData(ClipBoardDataFormatIdentify) as PartialGrid;
						if (partialGrid != null)
						{
							#region Partial Grid Pasting
							int startRow = selectionRange.Row;
							int startCol = selectionRange.Col;

							int rows = partialGrid.Rows;
							int cols = partialGrid.Cols;

							int rowRepeat = 1;
							int colRepeat = 1;

							if (selectionRange.Rows % partialGrid.Rows == 0)
							{
								rows = selectionRange.Rows;
								rowRepeat = selectionRange.Rows / partialGrid.Rows;
							}
							if (selectionRange.Cols % partialGrid.Cols == 0)
							{
								cols = selectionRange.Cols;
								colRepeat = selectionRange.Cols / partialGrid.Cols;
							}

							if (startRow + rows >= this.rows.Count
								|| startCol + cols >= this.cols.Count)
							{
								// TODO: paste range overflow
								// need to notify user-code to handle this 
								return false;
							}

							// check any intersected merge-range in partial grid 
							// 
							bool crossMergedRange = false;

							if (partialGrid.Cells != null)
							{
								for (int rr = 0; rr < rowRepeat; rr++)
								{
									for (int cc = 0; cc < colRepeat; cc++)
									{
										partialGrid.Cells.IterateContent((row, col, cell) =>
										{
											if (cell.IsMergedCell)
											{
												for (int r = startRow; r < cell.MergeEndPos.Row - cell.Row + startRow + 1; r++)
												{
													for (int c = startCol; c < cell.MergeEndPos.Col - cell.Col + startCol + 1; c++)
													{
														int tr = r + rr * partialGrid.Rows;
														int tc = c + cc * partialGrid.Cols;

														var existedCell = cells[tr, tc];

														if (existedCell != null
															&& ((existedCell.Rowspan == 0 && existedCell.Colspan == 0)
															|| existedCell.IsMergedCell))
														{
															crossMergedRange = true;
															return 0;
														}
													}
												}
											}

											return crossMergedRange ? 1 : 0;
										});

										if (crossMergedRange) break;
									}

									if (crossMergedRange) break;
								}
							}

							if (crossMergedRange)
							{
								// attmpt to modify part of a merged range
								// TODO: any way to raise range exception?
							}
							else
							{
								DoAction(new RGSetPartialGridAction(new ReoGridRange(
									startRow, startCol, rows, cols), partialGrid));
							}
							#endregion // Partial Grid Pasting
						}
						else if (data.ContainsText())
						{
							#region Plain Text Pasting
							var str = data.GetText();
							if (!string.IsNullOrEmpty(str))
							{
								var arrayData = RGUtility.ParseTabbedString(str);

								int rows = Math.Max(selectionRange.Rows, arrayData.GetLength(0));
								int cols = Math.Max(selectionRange.Cols, arrayData.GetLength(1));

								DoAction(new RGSetRangeDataAction(new ReoGridRange(selectionRange.Row,
									selectionRange.Col, rows, cols), arrayData));
							}
							#endregion // Plain Text Pasting
						}
					}
				}
				catch (RangeIntersectionException /*rangeException*/)
				{
					// Application should handle this exception and alert message to end-user
					//
					// TODO: should have a better way to tell user-code that exception happening.
					//
					// MessageBox.Show(string.Format("Cannot change part of a merged cell. [{0},{1}]", 
					//	 rangeException.Range.Row, rangeException.Range.Col));
				}
				finally
				{
					Cursor = gridSelectCursor;

					InvalidateCanvas();
				}

				if (AfterPaste != null)
				{
					AfterPaste(this, new RGRangeEventArgs(this.selectionRange));
				}
			}

			return true;
		}

		/// <summary>
		/// Copy any remove anything from selected range into Clipboard.
		/// </summary>
		public virtual bool Cut()
		{
			if (IsEditing)
			{
				editTextbox.Cut();
			}
			else
			{
				if (BeforeCut != null)
				{
					var evtArg = new RGBeforeRangeOperationEventArgs(this.selectionRange);
					BeforeCut(this, evtArg);
					if (evtArg.IsCancelled)
					{
						return false;
					}
				}

#if EX_SCRIPT
				object scriptReturn = RaiseScriptEvent("oncut");
				if (scriptReturn != null && !ScriptRunningMachine.GetBoolValue(scriptReturn))
				{
					return false;
				}
#endif

				if (!Copy()) return false;

				DeleteRangeData(currentCopingRange);
				RemoveRangeStyle(currentCopingRange, PlainStyleFlag.All);

				if (AfterCut != null)
				{
					AfterCut(this, new RGRangeEventArgs(this.selectionRange));
				}
			}

			return true;
		}

		#region Partial Grid

		/// <summary>
		/// Copy specified range into a separated grid 
		/// </summary>
		/// <param name="row">number of start row</param>
		/// <param name="col">number of start col</param>
		/// <param name="rows">number of rows to be copied</param>
		/// <param name="cols">number of columns to be copied</param>
		/// <returns></returns>
		public PartialGrid GetPartialGrid(int row, int col, int rows, int cols)
		{
			return GetPartialGrid(new ReoGridRange(row, col, rows, cols));
		}

		/// <summary>
		/// Copy range into separated grid from current grid
		/// </summary>
		/// <param name="range">The range to be copied</param>
		/// <returns>A partial grid copied from specified range</returns>
		public PartialGrid GetPartialGrid(ReoGridRange range)
		{
			return GetPartialGrid(range, PartialGridCopyFlag.All, ExPartialGridCopyFlag.None);
		}
		
		internal PartialGrid GetPartialGrid(ReoGridRange range, PartialGridCopyFlag flag, ExPartialGridCopyFlag exFlag)
		{
			range = FixRange(range);

			int rows = range.Rows;
			int cols = range.Cols;

			PartialGrid data = new PartialGrid()
			{
				Cols = cols,
				Rows = rows,
			};

			if ((flag & PartialGridCopyFlag.CellData) == PartialGridCopyFlag.CellData
				|| (flag & PartialGridCopyFlag.CellStyle) == PartialGridCopyFlag.CellStyle)
			{
				data.Cells = new RegularTreeArray<ReoGridCell>();

				cells.Iterate(range.Row, range.Col, rows, cols, true, (r, c, cell) =>
				{
					int toRow = r - range.Row;
					int toCol = c - range.Col;

					var toCell = new ReoGridCell();
					ReoGridCellUtility.CopyCell(toCell, cell);
					data.Cells[toRow, toCol] = toCell;

					if ((flag & PartialGridCopyFlag.CellStyle) == PartialGridCopyFlag.CellStyle
						&& toCell != null && toCell.Style == null)
					{
						toCell.Style = ReoGridStyleUtility.CreateDefaultGridStyle(this, r, c);
					}

					return 1;
				});
			}

			if ((flag & PartialGridCopyFlag.HBorder) == PartialGridCopyFlag.HBorder)
			{
				data.HBorders = new RegularTreeArray<ReoGridHBorder>();

				hBorders.Iterate(range.Row, range.Col, rows + 1, cols, true, (r, c, hBorder) =>
				{
					// only copy borders they belong its cell unless BorderOutsideOwner is specified
					if (((exFlag & ExPartialGridCopyFlag.BorderOutsideOwner) == ExPartialGridCopyFlag.BorderOutsideOwner)
						|| (hBorder != null && hBorder.Pos == HBorderOwnerPosition.None)
						|| (
								(r != range.Row
								|| (hBorder != null
								&& (hBorder.Pos & HBorderOwnerPosition.Top) == HBorderOwnerPosition.Top))
							&&
								(r != range.Row2 + 1
								|| (hBorder != null
								&& (hBorder.Pos & HBorderOwnerPosition.Bottom) == HBorderOwnerPosition.Bottom)))
					)
					{
						int toCol = c - range.Col;
						ReoGridHBorder thBorder = ReoGridHBorder.Clone(hBorder);
						if (thBorder != null && thBorder.Cols > cols - toCol) thBorder.Cols = cols - toCol;
						data.HBorders[r - range.Row, toCol] = thBorder;
					}
					return 1;
				});
			}

			if ((flag & PartialGridCopyFlag.VBorder) == PartialGridCopyFlag.VBorder)
			{
				data.VBorders = new RegularTreeArray<ReoGridVBorder>();

				vBorders.Iterate(range.Row, range.Col, rows, cols + 1, true, (r, c, vBorder) =>
				{
					// only copy borders they belongs its cell unless BorderOutsideOwner is specified
					if (((exFlag & ExPartialGridCopyFlag.BorderOutsideOwner) == ExPartialGridCopyFlag.BorderOutsideOwner)
						|| (vBorder != null && vBorder.Pos == VBorderOwnerPosition.None)
						|| (
								(c != range.Col
								|| (vBorder != null
								&& (vBorder.Pos & VBorderOwnerPosition.Left) == VBorderOwnerPosition.Left))
							&&
								(c != range.Col2 + 1
								|| (vBorder != null
								&& (vBorder.Pos & VBorderOwnerPosition.Right) == VBorderOwnerPosition.Right)))
					)
					{
						int toRow = r - range.Row;
						ReoGridVBorder tvBorder = ReoGridVBorder.Clone(vBorder);
						if (tvBorder != null && tvBorder.Rows > rows - toRow) tvBorder.Rows = rows - toRow;
						data.VBorders[toRow, c - range.Col] = tvBorder;
					}
					return 1;
				});
			}

			return data;
		}
		
		/// <summary>
		/// Copy from a separated grid into current grid
		/// </summary>
		/// <param name="data">Partial grid to be copied</param>
		/// <param name="toRange">Range to be copied</param>
		/// <returns>Range has been copied</returns>
		public ReoGridRange SetPartialGrid(ReoGridRange toRange, PartialGrid data)
		{
			return SetPartialGrid(toRange, data, PartialGridCopyFlag.All, ExPartialGridCopyFlag.None);
		}
		
		internal ReoGridRange SetPartialGrid(ReoGridRange toRange, PartialGrid data,
			PartialGridCopyFlag flag)
		{
			return SetPartialGrid(toRange, data, flag, ExPartialGridCopyFlag.None);
		}
		
		internal ReoGridRange SetPartialGrid(ReoGridRange toRange, PartialGrid data,
			PartialGridCopyFlag flag, ExPartialGridCopyFlag exFlag)
		{
			if (toRange.IsEmpty) return toRange;

			toRange = FixRange(toRange);

			int rows = data.Rows;
			int cols = data.Cols;

			if (rows + toRange.Row > this.rows.Count) rows = this.rows.Count - toRange.Row;
			if (cols + toRange.Col > this.cols.Count) cols = this.cols.Count - toRange.Col;

			if (((flag & PartialGridCopyFlag.CellData) == PartialGridCopyFlag.CellData
				|| (flag & PartialGridCopyFlag.CellStyle) == PartialGridCopyFlag.CellStyle))
			{
				using(Graphics g = Graphics.FromHwnd(this.Handle)){

					for (int r = 0; r < rows; r++)
					{
						for (int c = 0; c < cols; c++)
						{
							ReoGridCell fromCell = data.Cells == null ? null : data.Cells[r, c];

							int tr = toRange.Row + r;
							int tc = toRange.Col + c;

							bool processed = false;

							if (fromCell != null)
							{
								#region Merge from right side
								// from cell copied as part of merged cell
								if (
									// is part of merged cell
									!fromCell.MergeStartPos.IsEmpty

									&& fromCell.MergeStartPos.Col < toRange.Col
									// is right side     -------+--      (undo from delete column at end of merged range) 
									&& (fromCell.Col - fromCell.MergeStartPos.Col > tc - toRange.Col
									// not inside       --+----+--     
									&& fromCell.MergeEndPos.Col <= toRange.Col2))
								{
									// from cell inside existed merged range
									// these two ranges should be merged
									// the original range must be expanded
									ReoGridCell fromMergedStart = CreateAndGetCell(fromCell.MergeStartPos);
									fromMergedStart.MergeEndPos = new ReoGridPos(fromMergedStart.MergeEndPos.Row, tc);
									fromMergedStart.Colspan = (short)(tc - fromMergedStart.Col + 1);

									for (int ic = fromMergedStart.Col; ic < fromMergedStart.Col + fromMergedStart.Colspan; ic++)
									{
										var insideCell = cells[tr, ic];
										if (insideCell != null)
										{
											insideCell.MergeEndPos = new ReoGridPos(insideCell.MergeEndPos.Row, tc);
										}
									}

									ReoGridCell tocell = CreateAndGetCell(tr, tc);
									tocell.MergeStartPos = fromMergedStart.Pos;
									tocell.MergeEndPos = new ReoGridPos(fromMergedStart.MergeEndPos.Row, tc);
									tocell.Colspan = 0;
									tocell.Rowspan = 0;

									if (tocell.IsEndMergedCell)
									{
										fromMergedStart.Bounds = GetRangeBounds(new ReoGridRange(
											fromMergedStart.Pos, fromMergedStart.MergeEndPos));
									}

									processed = true;

								}
								#endregion
								#region Merge from left side
								else if (
									!fromCell.MergeEndPos.IsEmpty
									&& fromCell.MergeEndPos.Col > toRange.Col2
									&& fromCell.MergeStartPos.Col <= toRange.Col2
									)
								{
									// target partial range will override exsited range
									// need to update existed range at right side
									int rightCol = Math.Min(fromCell.MergeEndPos.Col, this.cols.Count - 1);

									ReoGridCell tocell = CreateAndGetCell(tr, tc);
									tocell.MergeStartPos = new ReoGridPos(fromCell.MergeStartPos.Row, fromCell.MergeStartPos.Col + tc - fromCell.Col);
									tocell.MergeEndPos = new ReoGridPos(fromCell.MergeEndPos.Row, rightCol);

									for (int ic = toRange.Col2 + 1; ic <= rightCol; ic++)
									{
										var existedEndCell = CreateAndGetCell(tr, ic);
										existedEndCell.MergeStartPos = tocell.MergeStartPos;
										existedEndCell.Rowspan = 0;
										existedEndCell.Colspan = 0;
									}

									if (tocell.IsStartMergedCell)
									{
										tocell.Rowspan = (short)(tocell.MergeEndPos.Row - tocell.MergeStartPos.Row + 1);
										tocell.Colspan = (short)(tocell.MergeEndPos.Col - tocell.MergeStartPos.Col + 1);

										tocell.Bounds = GetRangeBounds(tocell.Pos, tocell.MergeEndPos);

										// copy cell content
										ReoGridCellUtility.CopyCellContent(tocell, fromCell);

										UpdateCellFont(tocell, g, false);
									}
									else
									{
										tocell.Rowspan = 0;
										tocell.Colspan = 0;
									}

									processed = true;
								}
								#endregion // Merge from left side
								#region Merge from bottom
								else if (
									!fromCell.MergeStartPos.IsEmpty
									// above
									&& fromCell.MergeStartPos.Row < toRange.Row
									// merged start row in the above of target fill range
									&& fromCell.Row - fromCell.MergeStartPos.Row > tr - toRange.Row
									// not inside current merged range
									&& fromCell.MergeEndPos.Row <= toRange.Row2)
								{
									var mergedStartCell = CreateAndGetCell(fromCell.MergeStartPos);
									mergedStartCell.Rowspan = (short)(tr - mergedStartCell.Row + 1);

									for (int ir = fromCell.MergeStartPos.Row; ir < tr; ir++)
									{
										var existedCell = CreateAndGetCell(ir, tc);
										existedCell.MergeEndPos = new ReoGridPos(tr, fromCell.MergeEndPos.Col);
									}

									var tocell = CreateAndGetCell(tr, tc);
									tocell.MergeStartPos = mergedStartCell.Pos;
									tocell.MergeEndPos = new ReoGridPos(tr, fromCell.MergeEndPos.Col);
									tocell.Rowspan = 0;
									tocell.Colspan = 0;

									if (tocell.IsEndMergedCell)
									{
										mergedStartCell.Bounds = GetRangeBounds(mergedStartCell.Pos, mergedStartCell.MergeEndPos);
									}

									processed = true;
								}
								#endregion // Merge from bottom
								#region Merge from top
								else if (
									!fromCell.MergeEndPos.IsEmpty
									&& fromCell.MergeEndPos.Row > toRange.Row2
									&& fromCell.MergeStartPos.Row <= toRange.Row2)
								{
									// target partial range will override exsited range
									// need to update existed range at right side
									int bottomRow = Math.Min(fromCell.MergeEndPos.Row, this.rows.Count - 1);

									for (int ir = toRange.Row2 + 1; ir <= bottomRow; ir++)
									{
										var existedEndCell = CreateAndGetCell(ir, tc);
										existedEndCell.MergeStartPos = new ReoGridPos(fromCell.MergeStartPos.Row, existedEndCell.MergeStartPos.Col);
										existedEndCell.Rowspan = 0;
										existedEndCell.Colspan = 0;
									}

									ReoGridCell tocell = CreateAndGetCell(tr, tc);
									tocell.MergeStartPos = fromCell.MergeStartPos;
									tocell.MergeEndPos = new ReoGridPos(bottomRow, fromCell.MergeEndPos.Col);

									if (tocell.IsStartMergedCell)
									{
										tocell.Rowspan = (short)(tocell.MergeEndPos.Row - tocell.MergeStartPos.Row + 1);
										tocell.Colspan = (short)(tocell.MergeEndPos.Col - tocell.MergeStartPos.Col + 1);

										tocell.Bounds = GetRangeBounds(tocell.Pos, tocell.MergeEndPos);

										// copy cell content
										ReoGridCellUtility.CopyCellContent(tocell, fromCell);

										UpdateCellFont(tocell, g, true);
									}
									else
									{
										tocell.Rowspan = 0;
										tocell.Colspan = 0;
									}

									processed = true;
								}
								#endregion // Merge from top
							}

							if (!processed)
							{
								ReoGridCell toCell = CreateAndGetCell(tr, tc);

								if (toCell.Rowspan == 0 && toCell.Colspan == 0)
								{
									continue;
								}

								if (fromCell != null)
								{
									#region Copy Data
									if ((flag & PartialGridCopyFlag.CellData) == PartialGridCopyFlag.CellData)
									{
										ReoGridCellUtility.CopyCellContent(toCell, fromCell);
									}
									#endregion // Copy Data

									#region Copy Merged info

									// is single cell
									if (toCell.Rowspan == 1 && toCell.Colspan == 1)
									{
										// then copy span info
										toCell.Rowspan = fromCell.Rowspan;
										toCell.Colspan = fromCell.Colspan;

										if (!fromCell.MergeStartPos.IsEmpty)
										{
											toCell.MergeStartPos = fromCell.MergeStartPos.Offset(tr - fromCell.Row, tc - fromCell.Col);

											Debug.Assert(toCell.MergeStartPos.Row >= 0 && toCell.MergeStartPos.Row < this.rows.Count);
											Debug.Assert(toCell.MergeStartPos.Col >= 0 && toCell.MergeStartPos.Col < this.cols.Count);
										}

										if (!fromCell.MergeEndPos.IsEmpty)
										{
											toCell.MergeEndPos = fromCell.MergeEndPos.Offset(tr - fromCell.Row, tc - fromCell.Col);

											Debug.Assert(toCell.MergeEndPos.Row >= 0 && toCell.MergeEndPos.Row < this.rows.Count);
											Debug.Assert(toCell.MergeEndPos.Col >= 0 && toCell.MergeEndPos.Col < this.cols.Count);
										}
									}
									else
									{
										UpdateCellFont(toCell, g, true);
									}
									#endregion // Copy Merged info

									#region Cell Styles
									if (((flag & PartialGridCopyFlag.CellStyle) == PartialGridCopyFlag.CellStyle)
										&& fromCell.Style != null)
									{
										toCell.Style = new ReoGridRangeStyle(fromCell.Style);

										// TODO: render alignment is not contained in cell's style
										// copy the style may also need copy the render alignment
										// or we need to update the cell format again?
										if (fromCell.Style.HAlign == ReoGridHorAlign.General)
										{
											toCell.RenderHorAlign = fromCell.RenderHorAlign;
										}
									}
									#endregion

									if (toCell.IsEndMergedCell)
									{
										ReoGridCell cell = GetCell(toCell.MergeStartPos);
										Debug.Assert(cell != null);

										UpdateCellBounds(cell);
									}

									if ((toCell.Rowspan == 1 && toCell.Colspan == 1)
										|| toCell.IsEndMergedCell)
									{
										UpdateCellFont(toCell, g, true);
									}
								}
								else
								{
									cells[tr, tc] = null;
								}
							}
						}
					}
				}
			}

			// h-borders
			if ((flag & PartialGridCopyFlag.HBorder) == PartialGridCopyFlag.HBorder)
			{
				if (data.HBorders == null)
				{
					// cut left side border
					if (toRange.Col > 0)
					{
						for (int r = toRange.Row; r <= toRange.Row2; r++)
						{
							CutBeforeHBorder(r, toRange.Col);
						}
					}

					// set border to null
					this.hBorders.Iterate(toRange.Row, toRange.Col, rows, cols, true,
						(r, c, fromHBorder) =>
						{
							this.hBorders[r, c] = null;
							return 1;
						}
					);
				}
				else
				{
					// TODO: need to improve performance
					for (int r = 0; r < rows + 1; r++)
					{
						for (int c = 0; c < cols; c++)
						{
							int tr = toRange.Row + r;
							int tc = toRange.Col + c;

							CutBeforeHBorder(tr, tc);

							var fromHBorder = data.HBorders[r, c];

							if (fromHBorder == null)
							{
								hBorders[tr, tc] = null;
							}
							else
							{
								ReoGridBorderStyle style = fromHBorder.Border;

								int hcols = fromHBorder.Cols;
								if (hcols > cols - c) hcols = cols - c;
								GetHBorder(tr, tc).Cols = hcols;

								if (data.HBorders[r, c].Border != null)
								{
									// in last col
									//if (c == cols - 1)
									//	SetHBorders(tr, tc, hcols, style, fromHBorder.Pos);
									//else
									//	hBorders[tr, tc].Border = style;

									SetHBorders(tr, tc, hcols, style, fromHBorder.Pos);
								}
								else
									hBorders[tr, tc].Border = ReoGridBorderStyle.Empty;
							}
						}
					}
				}
			}

			// v-borders
			if ((flag & PartialGridCopyFlag.VBorder) == PartialGridCopyFlag.VBorder)
			{
				if (data.VBorders == null)
				{
					// cut top side border
					if (toRange.Row > 0)
					{
						for (int c = toRange.Col; c <= toRange.Col2; c++)
						{
							CutBeforeVBorder(toRange.Row, c);
						}
					}

					// set border to null
					this.vBorders.Iterate(toRange.Row, toRange.Col, rows, cols, true,
						(r, c, fromVBorder) =>
						{
							this.vBorders[r, c] = null;
							return 1;
						}
					);
				}
				else
				{
					// TODO: need to improve performance
					for (int r = 0; r < rows; r++)
					{
						for (int c = 0; c < cols + 1; c++)
						{
							int tr = toRange.Row + r;
							int tc = toRange.Col + c;

							CutBeforeVBorder(tr, tc);

							var fromVBorder = data.VBorders[r, c];

							if (fromVBorder == null)
							{
								vBorders[tr, tc] = null;
							}
							else
							{
								ReoGridBorderStyle style = fromVBorder.Border;

								int vrows = fromVBorder.Rows;
								if (vrows > rows - r) vrows = rows - r;
								GetVBorder(tr, tc).Rows = vrows;

								if (data.VBorders[r, c].Border != null)
								{
									// is last row
									//if (r == rows - 1)
									//	SetVBorders(tr, tc, vrows, style, fromVBorder.Pos);
									//else
									//	vBorders[tr, tc].Border = fromVBorder.Border;
									SetVBorders(tr, tc, vrows, style, fromVBorder.Pos);
								}
								else
									vBorders[tr, tc].Border = ReoGridBorderStyle.Empty;
							}
						}
					}
				}
			}

			return new ReoGridRange(toRange.Row, toRange.Col, rows, cols);
		}

		/// <summary>
		/// Repeat to copy from a separated grid to fit specified range
		/// </summary>
		/// <param name="grid">Partial grid to be copied</param>
		/// <param name="range">Range to be copied</param>
		/// <returns></returns>
		public ReoGridRange SetPartialGridRepeatly(PartialGrid grid, ReoGridRange range)
		{
			if (grid.Rows <= 0 || grid.Cols <= 0) return ReoGridRange.Empty;

			int r = range.Row;
			int c = range.Col;

			for (; r <= range.Row2; r += grid.Rows)
			{
				if (r > range.Row2)
				{
					break;
				}

				for (; c <= range.Col2; c += grid.Cols)
				{
					if (c > range.Col2)
					{
						break;
					}

					SetPartialGrid(new ReoGridRange(r, c, grid.Rows, grid.Cols), grid);
				}
			}

			return range;
			//return new ReoGridRange(range.Row, range.Col, r-range.Row2+1, c-range.Col2+1);

			//int clipboardRows = grid.Rows;
			//int clipboardCols = grid.Cols;

			//bool isSingleCellSource = grid.Cells[0, 0] != null
			//	&& !grid.Cells[0, 0].MergeStartPos.IsEmpty
			//	&& grid.Cells[0, 0].Colspan == clipboardCols
			//	&& grid.Cells[0, 0].Rowspan == clipboardRows;

			//int pastedRows = 0;
			//int maxPastedCols = 0;

			//for (int r = range.Row; r <= range.Row2; r += clipboardRows)
			//{
			//	int maxPastedRows = 0;
			//	int pastedCols = 0;

			//	for (int c = range.Col; c <= range.Col2; c += clipboardCols)
			//	{
			//		ReoGridRange toRange = new ReoGridRange(r, c, clipboardRows, clipboardCols);

			//		ReoGridRange partialRange = SetPartialGrid(toRange, grid);
			//		if (partialRange.IsEmpty)
			//		{
			//			MessageBox.Show("Could not change part of merged cell.");
			//			r = range.Row2 + 1;
			//			break;
			//		}
			//		else
			//		{
			//			maxPastedRows = Math.Max(maxPastedRows, partialRange.Rows);
			//			pastedCols += partialRange.Cols;
			//		}
			//	}

			//	maxPastedCols = Math.Max(pastedCols, maxPastedCols);
			//	pastedRows += maxPastedRows;
			//}

			//return new ReoGridRange(range.Row, range.Col, pastedRows, maxPastedCols);
		}
		#endregion // Partial Grid

		private void CheckCanPaste()
		{
			// TODO
		}

		void ClipboardMonitor_ClipboardChanged(object sender, ClipboardChangedEventArgs e)
		{
			CheckCanPaste();
		}

		/// <summary>
		/// Determine whether the selected range can be copied.
		/// </summary>
		/// <returns>True if the selected range can be copied.</returns>
		public bool CanCopy()
		{
			//TODO
			return true;
		}

		/// <summary>
		/// Determine whether the selected range can be cutted.
		/// </summary>
		/// <returns>True if the selected range can be cutted.</returns>
		public bool CanCut()
		{
			//TODO
			return true;
		}

		/// <summary>
		/// Determine whether the data contained in Clipboard can be pasted into grid control.
		/// </summary>
		/// <returns>True if the data contained in Clipboard can be pasted</returns>
		public bool CanPaste()
		{
			//TODO
			return true;
		}

		/// <summary>
		/// Before a range will be pasted from Clipboard
		/// </summary>
		public event EventHandler<RGBeforeRangeOperationEventArgs> BeforePaste;

		/// <summary>
		/// When a range has been pasted into grid
		/// </summary>
		public event EventHandler<RGRangeEventArgs> AfterPaste;

		/// <summary>
		/// Before a range to be copied into Clipboard
		/// </summary>
		public event EventHandler<RGBeforeRangeOperationEventArgs> BeforeCopy;

		/// <summary>
		/// When a range has been copied into Clipboard
		/// </summary>
		public event EventHandler<RGRangeEventArgs> AfterCopy;

		/// <summary>
		/// Before a range to be moved into Clipboard
		/// </summary>
		public event EventHandler<RGBeforeRangeOperationEventArgs> BeforeCut;

		/// <summary>
		/// After a range to be moved into Clipboard
		/// </summary>
		public event EventHandler<RGRangeEventArgs> AfterCut;
		#endregion

		#region Edit
		public event EventHandler<RGCellBeforeEditEventArgs> BeforeCellEdit;
		public event EventHandler<RGCellAfterEditEventArgs> AfterCellEdit;

		private ReoGridCell currentEditingCell;
		private object backupData;
		private readonly InputTextBox editTextbox;
		//private readonly DropdownWindow dropWindow;
		//private readonly NumericField numericTextbox;

		/// <summary>
		/// Get data of cell in specified position
		/// </summary>
		/// <param name="pos">Position of cell</param>
		/// <returns>Data of cell in specified position</returns>
		public object this[ReoGridPos pos]
		{
			get { return this[pos.Row, pos.Col]; }
			set { this[pos.Row, pos.Col] = value; }
		}

		/// <summary>
		/// Get data of cell in specified position
		/// </summary>
		/// <param name="row">Index of row of specified cell</param>
		/// <param name="col">Index of column of specified cell</param>
		/// <returns>Data of cell in specified position</returns>
		public object this[int row, int col]
		{
			get
			{
				ReoGridCell cell = GetCell(row, col);
				return cell==null ? null : cell.Data;
			}
			set
			{
				SetCellData(row, col, value);
			}
		}

		/// <summary>
		/// Get or set data in specified range
		/// </summary>
		/// <param name="row">number of start row</param>
		/// <param name="col">number of start column</param>
		/// <param name="rows">number of rows</param>
		/// <param name="cols">number of columns</param>
		/// <returns></returns>
		public object this[int row, int col, int rows, int cols]
		{
			get{
				return this[new ReoGridRange(row,col,rows,cols)];
			}
			set
			{
				this[new ReoGridRange(row, col, rows, cols)] = value;
			}
		}

		/// <summary>
		/// Get or set data from specified range
		/// </summary>
		/// <param name="range">range to be get or set</param>
		/// <returns>data copied from grid</returns>
		public object this[ReoGridRange range]
		{
			get
			{
				return GetRangeData(range);
			}
			set
			{
				SetRangeData(range, value);
			}
		}

		/// <summary>
		/// Get or set data from specified position or range
		/// </summary>
		/// <example>A1 or A1:C3</example>
		/// <param name="id">position in string ("A1" or "A1:C3" etc.)</param>
		/// <returns>object to get from specified position</returns>
		public object this[string id]
		{
			get
			{
				int idx = id.IndexOf(':');
				if (idx >= 0)
				{
					return this[new ReoGridRange(id)];
				}
				else
				{
					return this[new ReoGridPos(id)];
				}
			}
			set
			{
				int idx = id.IndexOf(':');
				if (idx >= 0)
				{
					if (!(value is object[,]))
					{
						throw new ArgumentException("Data of two-dimensional array is required to a range");
					}
					// TOOD: to support more types of data
					this[new ReoGridRange(id)] = (object[,])value;
				}
				else
				{
					this[new ReoGridPos(id)] = value;
				}
			}
		}

		protected override bool ProcessDialogChar(char charCode)
		{
			if (Toolkit.IsKeyDown(Win32.VKey.VK_MENU)
				|| Toolkit.IsKeyDown(Win32.VKey.VK_CONTROL)
				|| charCode == 8			// backspace
				|| charCode == 13			// enter
				|| charCode == 27			// escape
				|| charCode == '\t'		// tab
				)
			{
				return false;
			}

			// find first cell of range
			StartEdit(SelectionRange.Row, SelectionRange.Col);

			if (editTextbox.Visible)
			{
				editTextbox.Text = charCode.ToString();
				editTextbox.SelectionStart = 1;

				if (HasSettings(ReoGridSettings.Edit_FriendlyPercentInput))
				{
					// for percent data format input
					if (charCode >= '0' && charCode <= '9')
					{
						ReoGridCell cell = cells[selectionRange.Row, selectionRange.Col];
						if (cell != null && cell.DataFormat == CellDataFormatFlag.Percent)
						{
							editTextbox.Text += "%";
							editTextbox.SelectionStart = 1;
						}
					}
				}
				//if (focusRow != null && focusCell != null
				//  && StartEdit(focusRow, focusCell))
				//{
				//  switch (currentEditingCell.Define.Type)
				//  {
				//    case StyleGridColumnType.Text:
				//Win32.SendMessage(editTextbox.Handle,
				//  (uint)Win32.WMessages.WM_CHAR, charCode, 0);
				//break;
				//  case StyleGridColumnType.Dropdown:
				//    Win32.SendMessage(dropWindow.GetListboxHandle(),
				//      (uint)Win32.WMessages.WM_CHAR, charCode, 0);
				//    break;
				//  case StyleGridColumnType.Combo:
				//    Win32.SendMessage(editTextbox.Handle,
				//      (uint)Win32.WMessages.WM_CHAR, charCode, 0);
				//    Win32.SendMessage(dropWindow.GetListboxHandle(),
				//      (uint)Win32.WMessages.WM_CHAR, charCode, 0);
				//    break;
				//  case StyleGridColumnType.Numeric:
				//    Win32.SendMessage(numericTextbox.GetTextboxHandle(),
				//      (uint)Win32.WMessages.WM_CHAR, charCode, 0);
				//    break;
				//}
				//}

				return true;
			}
			else
				return false;
		}
		#region StartEdit
		/// <summary>
		/// Start to edit selected cell
		/// </summary>
		/// <returns>True if the editing operation has been started</returns>
		public virtual bool StartEdit()
		{
			return focusPos.IsEmpty ? false : StartEdit(focusPos);
		}
		/// <summary>
		/// Start to edit specified cell
		/// </summary>
		/// <param name="pos">Position of specified cell</param>
		/// <returns>True if the editing operation has been started</returns>
		public virtual bool StartEdit(ReoGridPos pos)
		{
			return StartEdit(pos.Row, pos.Col);
		}
		/// <summary>
		/// Start to edit specified cell
		/// </summary>
		/// <param name="row">Index of row of specified cell</param>
		/// <param name="col">Index of column of specified cell</param>
		/// <returns>True if the editing operation has been started</returns>
		public virtual bool StartEdit(int row, int col)
		{
			// if cell is part of merged cell
			if (!IsValidCell(row, col))
			{
				// find the merged cell
				ReoGridCell cell = GetMergedCellOfRange(row, col);

				// start edit on merged cell
				return StartEdit(cell);
			}
			else
			{
				return StartEdit(CreateAndGetCell(row, col));
			}
		}
		internal virtual bool StartEdit(ReoGridCell cell)
		{
			if (HasSettings(ReoGridSettings.Readonly)) return false;

			//if (focusCell == cell)
			ScrollToFocusCell();

			if (BeforeCellEdit != null)
			{
				RGCellBeforeEditEventArgs args = new RGCellBeforeEditEventArgs(cell)
				{
					Cell = cell,
				};

				BeforeCellEdit(this, args);

				if (args.IsCancelled)
				{
					return false;
				}
			}

#if EX_SCRIPT
			// v0.8.2: 'beforeCellEdit' has been renamed to 'onCellEdit'
			object scriptReturn = RaiseScriptEvent("oncelledit", new RSCellObject(this, cell));
			if (scriptReturn != null && !ScriptRunningMachine.GetBoolValue(scriptReturn))
			{
				return false;
			}
#endif

			if (cell.body != null)
			{
				bool canContinue = cell.body.OnStartEdit(cell);
				if (!canContinue) return false;
			}

			bool isProcessed = false;

			if (currentEditingCell != null) EndEdit(editTextbox.Text);

			editTextbox.Text = !string.IsNullOrEmpty(cell.Formula) ?
				cell.Formula : (cell.Data == null ? string.Empty : cell.Data.ToString());

			float x = 0;
			float y = cell.Top * scaleFactor + 1;

			float width = (cell.Width);
			if (width < cell.TextBounds.Width) width = cell.TextBounds.Width;
			width = (width - 1) * scaleFactor - 1;

			float height = (cell.Height - 1) * scaleFactor - 1;
			if (!cell.IsMergedCell && cell.Style.TextWrapMode != TextWrapMode.NoWrap)
			{
				if (height < cell.TextBounds.Height) height = cell.TextBounds.Height;
			}

			editTextbox.SuspendLayout();

			switch (cell.RenderHorAlign)
			{
				default:
				case ReoGridRenderHorAlign.Left:
					editTextbox.TextAlign = HorizontalAlignment.Left;
					x = cell.Left * scaleFactor + 1;
					break;

				case ReoGridRenderHorAlign.Center:
					editTextbox.TextAlign = HorizontalAlignment.Center;
					x = (cell.Left * scaleFactor + (((cell.Width - 1) * scaleFactor - 1) - width) / 2) + 1;
					break;

				case ReoGridRenderHorAlign.Right:
					editTextbox.TextAlign = HorizontalAlignment.Right;
					x = (cell.Right - 1) * scaleFactor - width;
					break;
			}

			if (cell.Style.HAlign == ReoGridHorAlign.DistributedIndent)
			{
				editTextbox.TextAlign = HorizontalAlignment.Center;
			}

			int boxX = (int)Math.Round(x + viewportController.ActiveView.Left - viewportController.ActiveView.ViewLeft * scaleFactor);
			int boxY = (int)Math.Round(y + viewportController.ActiveView.Top - viewportController.ActiveView.ViewTop * scaleFactor);

			int offsetHeight = (int)Math.Round(height+2 - (cell.Height) * scaleFactor);

			if (offsetHeight > 0)
			{
				switch (cell.Style.VAlign)
				{
					case ReoGridVerAlign.Top:
						break;
					default:
					case ReoGridVerAlign.Middle:
						boxY -= offsetHeight / 2;
						break;
					case ReoGridVerAlign.Bottom:
						boxY -= offsetHeight;
						break;
				}
			}

			editTextbox.Bounds = new Rectangle(boxX, boxY, (int)Math.Round(width), (int)Math.Round(height));
			editTextbox.Multiline = true;
			editTextbox.TextWrap = cell.IsMergedCell || cell.Style.TextWrapMode != TextWrapMode.NoWrap;
			editTextbox.CellSize = cell.Bounds.Size;
			editTextbox.VAlign = cell.Style.VAlign;
			editTextbox.Font = cell.ScaledFont;
			editTextbox.ForeColor = cell.Style.TextColor;
			editTextbox.BackColor = (cell.Style.HasAny(PlainStyleFlag.FillAll) && !cell.Style.BackColor.IsEmpty)
				? cell.Style.BackColor :
				this.controlStyle.GetColor(ReoGridControlColors.GridBackground);
			editTextbox.SelectionStart = 0;
			editTextbox.ResumeLayout();
			editTextbox.Visible = true;

			editTextbox.Focus();

			Point p = Cursor.Position;

			Point p2 = PointToScreen(editTextbox.Location);
			p.X -= p2.X;
			p.Y -= p2.Y;

			{
				// some time the SendMessage causes PInvoke exception 
				// Do not know the reason exactly yet.
				try
				{
					Win32.SendMessage(editTextbox.Handle, (uint)Win32.WMessages.WM_LBUTTONDOWN, 0, Win32.CreateLParamPoint(p.X, p.Y));
					Win32.SendMessage(editTextbox.Handle, (uint)Win32.WMessages.WM_LBUTTONUP, 0, Win32.CreateLParamPoint(p.X, p.Y));
				}
				catch { }
			}

			isProcessed = true;

			if (isProcessed)
			{
				currentEditingCell = cell;
				backupData = cell.Data;
				currentEditingCell = cell;
				return true;
			}
			else
			{
				return false;
			}
		}
		#endregion
		#region EndEdit
		/// <summary>
		/// Check whether any cell current in edit mode
		/// </summary>
		/// <returns>true if any cell is editing</returns>
		public bool IsEditing
		{
			get
			{
				return currentEditingCell != null;
			}
		}

		/// <summary>
		/// Get position of cell that currently is in edit mode
		/// </summary>
		/// <returns>position of cell which is editing</returns>
		public ReoGridPos GetEditingCell()
		{
			return currentEditingCell == null ? ReoGridPos.Empty : currentEditingCell.Pos;
		}

		private bool endEditProcessing = false;
		/// <summary>
		/// Force end current editing operation with the specified reason.
		/// </summary>
		/// <param name="reason">Ending Reason of editing operation</param>
		/// <returns>True if currently in editing mode, and operation has been
		/// finished successfully.</returns>
		public virtual bool EndEdit(ReoGridEndEditReason reason)
		{
			return EndEdit(reason == ReoGridEndEditReason.NormalFinish ? editTextbox.Text : null, reason);
		}

		/// <summary>
		/// Force end current editing operation.
		/// Uses specified data instead of the data of user edited.
		/// </summary>
		/// <param name="data">New data to be set to the edited cell</param>
		/// <returns>True if currently in editing mode, and operation has been
		/// finished successfully.</returns>
		public virtual bool EndEdit(object data)
		{
			return EndEdit(data, ReoGridEndEditReason.NormalFinish);
		}

		/// <summary>
		/// Force end current editing operation with the specified reason.
		/// Uses specified data instead of the data of user edited.
		/// </summary>
		/// <param name="data">New data to be set to the edited cell</param>
		/// <param name="reason">Ending Reason of editing operation</param>
		/// <returns>True if currently in editing mode, and operation has been
		/// finished successfully.</returns>
		public virtual bool EndEdit(object data, ReoGridEndEditReason reason)
		{
			if (currentEditingCell == null || endEditProcessing) return false;

			endEditProcessing = true;

			if (AfterCellEdit != null)
			{
				RGCellAfterEditEventArgs arg = new RGCellAfterEditEventArgs(currentEditingCell)
				{
					EndReason = reason,
					NewData = data,
				};
				AfterCellEdit(this, arg);
				data = arg.NewData;
				reason = arg.EndReason;
			}

			switch (reason)
			{
				case ReoGridEndEditReason.Cancel:
					currentEditingCell.Data = backupData;
					break;

				case ReoGridEndEditReason.NormalFinish:
					if (data == null) data = editTextbox.Text;

					if ( data is string && string.IsNullOrEmpty((string)data)) data = null;

					if (!object.Equals(data, backupData))
					{
						DoAction(new RGSetCellDataAction(currentEditingCell.Row, currentEditingCell.Col, data));
					}
					break;
			}

			editTextbox.Visible = false;
			currentEditingCell = null;
			Focus();

			endEditProcessing = false;

			return true;
		}
		#endregion

		#region Editor - TextBox
		private class InputTextBox : TextBox
		{
			private ReoGridControl owner;
			internal bool TextWrap { get; set; }
			internal Size CellSize { get; set; }
			internal ReoGridVerAlign VAlign { get; set; }
			private Graphics graphics;
			private StringFormat sf;

			internal InputTextBox(ReoGridControl owner)
				: base()
			{
				this.owner = owner;
				SetStyle(ControlStyles.SupportsTransparentBackColor, true);
				BackColor = Color.Transparent;
			}
			protected override void OnCreateControl()
			{
				graphics = Graphics.FromHwnd(this.Handle);
				sf = new StringFormat(StringFormat.GenericDefault);
			}
			protected override void OnLostFocus(EventArgs e)
			{
				if (owner.currentEditingCell != null && Visible)
				{
					if (string.Equals(owner.backupData, base.Text))
					{
						owner.EndEdit(ReoGridEndEditReason.Cancel);
					}
					else
					{
						owner.EndEdit(Text);
					}
					Visible = false;
				}
				base.OnLostFocus(e);
			}
			protected override void OnKeyDown(KeyEventArgs e)
			{
				if (owner.currentEditingCell != null && Visible)
				{
					bool isProcessed = false;

					// in single line text
					if (!TextWrap && Text.IndexOf('\n') == -1)
					{
						if (e.KeyCode == Keys.Up)
						{
							e.SuppressKeyPress = true;
							owner.EndEdit(Text);
							owner.MoveSelectionUp();
							isProcessed = true;
						}
						else if (e.KeyCode == Keys.Down)
						{
							e.SuppressKeyPress = true;
							owner.EndEdit(Text);
							owner.MoveSelectionDown();
							isProcessed = true;
						}
					}

					if (!isProcessed)
					{
						if (!Toolkit.IsKeyDown(Win32.VKey.VK_CONTROL) && e.KeyCode == Keys.Enter)
						{
							e.SuppressKeyPress = true;
							owner.EndEdit(Text);
							owner.MoveSelectionForward();
						}
						else if (e.KeyCode == Keys.Escape)
						{
							e.SuppressKeyPress = true;
							owner.EndEdit(ReoGridEndEditReason.Cancel);
						}
					}
				}
			}
			protected override void OnKeyUp(KeyEventArgs e)
			{
				base.OnKeyUp(e);
			}

			protected override void OnTextChanged(EventArgs e)
			{
				base.OnTextChanged(e);

				CheckAndUpdateWidth();
			}
			
			protected override void OnVisibleChanged(EventArgs e)
			{
				base.OnVisibleChanged(e);

				if (Visible)
				{
					CheckAndUpdateWidth();
				}
			}
			private void CheckAndUpdateWidth()
			{
				if (graphics == null) graphics = Graphics.FromHwnd(this.Handle);
				if (sf == null) sf = new StringFormat(StringFormat.GenericTypographic);

				int fieldWidth = 0;
				if (TextWrap)
				{
					fieldWidth = CellSize.Width;
				}
				else
				{
					fieldWidth = 9999999; // todo: avoid unsafe magic number
				}

				if (TextWrap)
				{
					sf.FormatFlags &= ~StringFormatFlags.NoWrap;
				}
				else
				{
					sf.FormatFlags |= StringFormatFlags.NoWrap;
				}

				Size size = Size.Round(graphics.MeasureString(Text, Font, fieldWidth, sf));

				if (TextWrap)
				{
					this.SuspendLayout();

					if (Height < size.Height)
					{
						int offset = size.Height - Height + 1;
						
						Height += offset;

						if (Height < Font.Height)
						{
							offset = Font.Height - Height;
						}

						Height += offset;

						switch (VAlign)
						{
							case ReoGridVerAlign.Top:
								break;
							default:
							case ReoGridVerAlign.Middle:
								Top -= offset / 2;
								break;
							case ReoGridVerAlign.Bottom:
								Top -= offset;
								break;
						}
					}

					this.ResumeLayout();
				}
				else
				{
					this.SuspendLayout();

					if (Width < size.Width + 5)
					{
						int widthOffset = size.Width + 5 - Width;

						switch (TextAlign)
						{
							default:
							case HorizontalAlignment.Left:
								break;
							case HorizontalAlignment.Right:
								Left -= widthOffset;
								break;
						}

						Width += widthOffset;
					}

					if (Height < size.Height + 1)
					{
						int offset = size.Height - 1 - Height;
						Top -= offset / 2 + 0;
						Height = size.Height + 1;
					}

					this.ResumeLayout();
				}
			}

			#region IME
			protected override void WndProc(ref Message m)
			{
				if (m.Msg == (int)Win32.WMessages.WM_IME_CHAR)
				{
					//Debug.WriteLine("WM_IME_CHAR");
					base.WndProc(ref m);
				}
				else if (m.Msg == (int)Win32.WMessages.WM_IME_SETCONTEXT)
				{
					//Debug.WriteLine("WM_IME_SETCONTEXT");
					base.WndProc(ref m);
				}
				else if (m.Msg == (int)Win32.WMessages.WM_IME_STARTCOMPOSITION)
				{
					//Debug.WriteLine("WM_IME_STARTCOMPOSITION");
					CheckAndUpdateWidth();
					base.WndProc(ref m);
				}
				else if (m.Msg == (int)Win32.WMessages.WM_IME_NOTIFY)
				{
					//CheckAndUpdateWidth();
					base.WndProc(ref m);
				}
				else if (m.Msg == (int)Win32.WMessages.WM_IME_KEYDOWN)
				{
					//Debug.WriteLine("WM_IME_KEYDOWN");
					base.WndProc(ref m);
				}
				else if (m.Msg == (int)Win32.WMessages.WM_IME_SELECT)
				{
					//Debug.WriteLine("WM_IME_SELECT");
					base.WndProc(ref m);
				}
				else if (m.Msg == (int)Win32.WMessages.WM_IME_COMPOSITIONFULL)
				{
					//Debug.WriteLine("WM_IME_COMPOSITIONFULL");
					base.WndProc(ref m);
				}
				else if (m.Msg == (int)Win32.WMessages.WM_IME_COMPOSITION)
				{
					//Debug.WriteLine("WM_IME_COMPOSITION");
					IntPtr ime = Win32.ImmGetContext(this.Handle);
					int strLen = Win32.ImmGetCompositionString(ime, (int)Win32.GCS.GCS_COMPREADATTR, null, 0);
					StringBuilder sb = new StringBuilder(strLen);
					Win32.ImmGetCompositionString(ime, (int)Win32.GCS.GCS_COMPREADATTR, sb, strLen);

					string str = sb.ToString();

					//Debug.WriteLine("strlen = " + strLen + ", string = " + str);

					Win32.COMPOSITIONFORM com = new Win32.COMPOSITIONFORM();
					Win32.ImmGetCompositionWindow(ime, ref com);

					com.rcArea = new Rectangle(0, 0, 300, Height);
					bool b = Win32.ImmSetCompositionWindow(ime, ref com);

					Win32.ImmReleaseContext(this.Handle, ime);

					base.WndProc(ref m);
				}
				else if (m.Msg == (int)Win32.WMessages.WM_IME_REQUEST)
				{
					//Debug.WriteLine("WM_IME_REQUEST");
					//Debug.WriteLine(m.WParam);
					base.WndProc(ref m);
				}
				else
				{
					//Debug.WriteLine("msg = " + m);
					base.WndProc(ref m);
				}
			}
			#endregion

			protected override void Dispose(bool disposing)
			{
				if (graphics != null) graphics.Dispose();
				if (sf != null) sf.Dispose();

				base.Dispose(disposing);
			}
		}
		
		#endregion // Editor - TextBox
		#region Editor - NumericField
		private class NumericField : Control
		{
			private NumericTextbox textbox = new NumericTextbox
			{
				BorderStyle = BorderStyle.None,
				TextAlign = HorizontalAlignment.Right,
				Visible = false,
			};
			private class NumericTextbox : TextBox
			{
				private static readonly string validChars = "0123456789-.";
				protected override bool IsInputChar(char charCode)
				{
					return charCode == '\b' || false;
				}
				protected override bool ProcessDialogChar(char charCode)
				{
					return validChars.IndexOf(charCode) < 0;
				}
			}

			private ReoGridControl owner;
			private System.Windows.Forms.Timer timer;
			public NumericField(ReoGridControl owner)
			{
				this.owner = owner;
				timer = new System.Windows.Forms.Timer();
				timer.Tick += new EventHandler(timer_Tick);
				timer.Enabled = false;

				TabStop = false;
				DoubleBuffered = true;

				textbox.KeyDown += new KeyEventHandler(textbox_KeyDown);
				textbox.MouseUp += new MouseEventHandler(textbox_MouseUp);
				Controls.Add(textbox);
			}

			void textbox_MouseUp(object sender, MouseEventArgs e)
			{
				OnMouseUp(e);
			}

			void textbox_KeyDown(object sender, KeyEventArgs e)
			{
				if (Visible && e.KeyCode == Keys.Tab || e.KeyCode == Keys.Enter)
				{
					e.SuppressKeyPress = true;
					owner.EndEdit(GetValue());
					owner.MoveSelectionForward();
				}
				else if (e.KeyCode == Keys.Escape)
				{
					e.SuppressKeyPress = true;
					owner.EndEdit(backupData);
				}
				else if (e.KeyCode == Keys.Up)
				{
					ValueAdd(Toolkit.IsKeyDown(Win32.VKey.VK_SHIFT) ? 10 :
						(Toolkit.IsKeyDown(Win32.VKey.VK_CONTROL) ? 100 : 1));
					e.SuppressKeyPress = true;
				}
				else if (e.KeyCode == Keys.Down)
				{
					ValueSub(Toolkit.IsKeyDown(Win32.VKey.VK_SHIFT) ? 10 :
						(Toolkit.IsKeyDown(Win32.VKey.VK_CONTROL) ? 100 : 1));
					e.SuppressKeyPress = true;
				}
				else if (e.KeyCode == Keys.PageUp)
				{
					ValueAdd(10);
					e.SuppressKeyPress = true;
				}
				else if (e.KeyCode == Keys.PageDown)
				{
					ValueSub(10);
					e.SuppressKeyPress = true;
				}
				else if (e.KeyCode == Keys.V
					&& Toolkit.IsKeyDown(Win32.VKey.VK_CONTROL))
				{
					textbox.Paste();
				}
				else if (e.KeyCode == Keys.C
					&& Toolkit.IsKeyDown(Win32.VKey.VK_CONTROL))
				{
					textbox.Copy();
				}
				else if (e.KeyCode == Keys.X
					&& Toolkit.IsKeyDown(Win32.VKey.VK_CONTROL))
				{
					textbox.Cut();
				}
				//else if ((e.KeyValue & (int)Keys.LButton) > 0
				//  || (e.KeyValue & (int)Keys.RButton) > 0)
				//{
				//}
				//else if ((e.KeyValue < '0' || e.KeyValue > '9') && e.KeyCode != Keys.Back)
				//{
				//  e.SuppressKeyPress = true;
				//}
			}

			void timer_Tick(object sender, EventArgs e)
			{
				if (isUpPressed)
					ValueAdd(1);
				else if (isDownPressed)
					ValueSub(1);
				timer.Interval = 50;
			}
			object backupData;
			internal void SetValue(object data)
			{
				backupData = data;
				int value = 0;
				if (data is int)
					value = (int)data;
				else if (data is string)
					int.TryParse(data as string, out value);
				textbox.Text = value.ToString();
			}
			internal object GetValue()
			{
				int value = 0;
				int.TryParse(textbox.Text as string, out value);
				return value;
			}
			private static readonly int buttonSize = 17;
			private static readonly int arrowSize = 9;
			private bool isUpPressed = false;
			private bool isDownPressed = false;

			protected override void OnVisibleChanged(EventArgs e)
			{
				base.OnVisibleChanged(e);
				if (Visible)
				{
					textbox.Visible = true;
					textbox.Focus();
				}
				else
				{
					owner.EndEdit(GetValue());
				}
			}
			protected override void OnResize(EventArgs e)
			{
				base.OnResize(e);

				int hh = textbox.Height / 2;
				textbox.Bounds = new Rectangle(ClientRectangle.Left,
					ClientRectangle.Top + ClientRectangle.Height / 2 - hh - 1,
					ClientRectangle.Width - buttonSize - 1, textbox.Height);
			}
			protected override void OnPaint(PaintEventArgs e)
			{
				Graphics g = e.Graphics;

				int hh = Bounds.Height / 2 - 1;

				Rectangle rect = ClientRectangle;

				Rectangle upRect = new Rectangle(rect.Right - buttonSize, rect.Top, buttonSize, hh);
				GraphicsToolkit.Draw3DButton(g, upRect, isUpPressed);
				GraphicsToolkit.FillTriangle(g, arrowSize,
					new Point(upRect.Left + buttonSize / 2 - arrowSize / 2,
						upRect.Top + hh / 2 + (isUpPressed ? 2 : 1)),
					GraphicsToolkit.TriangleDirection.Up);

				Rectangle downRect = new Rectangle(rect.Right - buttonSize, rect.Top + hh + 1, buttonSize, hh);
				GraphicsToolkit.Draw3DButton(g, downRect, isDownPressed);
				GraphicsToolkit.FillTriangle(g, arrowSize,
					new Point(downRect.Left + buttonSize / 2 - arrowSize / 2,
						downRect.Top + hh / 2 - (isDownPressed ? 1 : 2)),
					GraphicsToolkit.TriangleDirection.Down);
			}

			internal void ValueAdd(int d)
			{
				int value = 0;
				int.TryParse(textbox.Text, out value);
				value += d;
				textbox.Text = value.ToString();
				textbox.SelectAll();
			}
			internal void ValueSub(int d)
			{
				int value = 0;
				int.TryParse(textbox.Text, out value);
				value -= d;
				textbox.Text = value.ToString();
				textbox.SelectAll();
			}

			protected override void OnMouseDown(MouseEventArgs e)
			{
				int hh = Bounds.Height / 2 - 1;

				Rectangle upRect = new Rectangle(ClientRectangle.Right - buttonSize, ClientRectangle.Top, buttonSize, hh);
				Rectangle downRect = new Rectangle(ClientRectangle.Right - buttonSize, ClientRectangle.Top + hh + 1, buttonSize, hh);

				if (upRect.Contains(e.Location))
				{
					textbox.Capture = true;
					isUpPressed = true;
					ValueAdd(1);
					timer.Interval = 600;
					timer.Start();
					Invalidate();
				}
				else if (downRect.Contains(e.Location))
				{
					textbox.Capture = true;
					isDownPressed = true;
					ValueSub(1);
					timer.Interval = 600;
					timer.Start();
					Invalidate();
				}
			}
			protected override void OnMouseUp(MouseEventArgs e)
			{
				isUpPressed = false;
				isDownPressed = false;
				timer.Stop();
				Invalidate();
			}
			protected override void OnMouseMove(MouseEventArgs e)
			{
				Cursor = Cursors.Default;
				base.OnMouseMove(e);
			}

			internal int GetNumericValue()
			{
				int num = 0;
				int.TryParse(textbox.Text, out num);
				return num;
			}
			internal void SelectAll()
			{
				textbox.SelectAll();
			}

			protected override void OnGotFocus(EventArgs e)
			{
				base.OnGotFocus(e);
				textbox.Focus();
			}
			internal IntPtr GetTextboxHandle()
			{
				return textbox.Handle;
			}
		}
		#endregion // Editor - NumericField

		public void SetCellData(string id, object data)
		{
			SetCellData(new ReoGridPos(id), data);
		}

		/// <summary>
		/// Set data of cell in the specified position
		/// </summary>
		/// <param name="pos">Position of cell</param>
		/// <param name="data">Data of cell</param>
		public void SetCellData(ReoGridPos pos, object data)
		{
			SetCellData(pos.Row, pos.Col, data);
		}

		/// <summary>
		/// Set data of cell in the specified position
		/// </summary>
		/// <param name="row">Index of row of specified cell</param>
		/// <param name="col">Index of column of specified cell</param>
		/// <param name="data">Data of cell</param>
		public void SetCellData(int row, int col, object data)
		{
			if (row < 0 || row > this.rows.Count - 1)
				throw new ArgumentOutOfRangeException("number of row out of the valid range of spreadsheet", "row");

			if (col < 0 || col > this.cols.Count - 1)
				throw new ArgumentOutOfRangeException("number of column out of the valid range of spreadsheet", "col");

			ReoGridPos pos = new ReoGridPos(row, col);

			if (data is object[,])
			{
				var arr = (object[,])data;
				int rows = arr.GetLength(0);
				int cols = arr.GetLength(1);
				SetRangeData(new ReoGridRange(row, col, rows, cols), arr);
				return;
			}
			else if (data is PartialGrid)
			{
				var subgrid = (PartialGrid)data;

				var range = new ReoGridRange(row, col, subgrid.Rows, subgrid.Cols);
				SetPartialGrid(range, subgrid);
			}
			else if (data is object[])
			{
				var array = (object[])data;

				for (int c = col; c < Math.Min(col + array.Length, this.cols.Count); c++)
				{
					SetCellData(row, c, array[c - col]);
				}
			}
			else
			{
				// do not set cell to null since something else to the cell must to be keep
				//
				//if (data == null)
				//{
				//	// clear cell when null will be set
				//	// check colspan and rowspan do nothing when cell is part of merged range
				//	ReoGridCell cell = cells[row, col];
				//	if (cell != null && cell.Colspan == 1 && cell.Rowspan == 1)
				//	{
				//		cells[cell.Row, cell.Col] = null;
				//	}
				//}
				var cell = cells[row, col];

				// both data and cell is null, then no need to update
				if ((data != null || cell != null)

					// if cell is not null, but it is invalid (merged by other cells), then need to update
					&& (cell == null || cell.IsValidCell))
				{
					SetCellData(CreateAndGetCell(row, col), data);
				}
			}
		}

		/// <summary>
		/// Set data into specified cell.
		/// User-code should do not use this method, use SetCellData(pos) or SetCellData(row, col) instead.
		/// </summary>
		/// <param name="cell"></param>
		/// <param name="data"></param>
		internal void SetCellData(ReoGridCell cell, object data)
		{
			if (data is ICellBody)
			{
				SetCellBody(cell, (ICellBody)data);

				data = cell.Body.GetData();
			}

			if (data is string || data is StringBuilder)
			{
				string str = Convert.ToString(data);

				// cell data processed as plain-text
				if (str.StartsWith("'"))
				{
#if EX_SCRIPT
					// clear old references
					ClearReferenceListForCell(cell);
#endif // EX_SCRIPT

					cell.Data = data;
					cell.Display = str.Substring(1);

					AfterUpdateCellText(cell);
					return;
				}
			}

#if EX_SCRIPT
			if (data is string || data is StringObject || data is StringBuilder)
			{
				string str = Convert.ToString(data);

				if (str.StartsWith("=") && str.Length > 1)
				{
					SetCellFormula(cell, str);
					return;
				}
			}

			// clear old references
			ClearReferenceListForCell(cell);
#endif // EX_SCRIPT

			UpdateCellData(cell, data);
		}

		/// <summary>
		/// Update data for cell without doing any format and formula evalution.
		/// </summary>
		/// <param name="cell">cell to be updated</param>
		/// <param name="data">data to be updated</param>
		internal void UpdateCellData(ReoGridCell cell, object data)
		{
			if (cell.body != null)
			{
				data = cell.body.OnSetData(data);
			}
			
			cell.Data = data;

			if (HasSettings(ReoGridSettings.Edit_AutoFormatCell))
			{
				DataFormatterManager.Instance.FormatCell(this, cell);
			}
			else
			{
				cell.Display = data == null ? string.Empty : Convert.ToString(data);
			}
		
			AfterUpdateCellText(cell);
		}

#if EX_SCRIPT
		internal void SetCellFormula(ReoGridCell cell, string formula)
		{
			// clear cell references from this cell
			List<ReoGridCell> referencedCells;
			bool referenceCellListExisted = formulaCells.TryGetValue(cell, out referencedCells);
			if (referenceCellListExisted)
			{
				referencedCells.Clear();
			}

			// clear range references from this cell
			List<ReferenceRange> referencedRanges;
			bool referencedRangeExisted = formulaRanges.TryGetValue(cell, out referencedRanges);
			if (referencedRangeExisted)
			{
				referencedRanges.Clear();
			}

			cell.Formula = formula;

			if (string.IsNullOrEmpty(formula))
			{
				UpdateCellData(cell, null);
				return;
			}

			// todo: improve: only create script context once
			//                when set data to a range
			var ctx = srm.CreateContext();
			ctx["__cell__"] = cell;

			// create an global variable getter
			ctx.PropertyGetter[id => RGUtility.CellReferenceRegex.IsMatch(id)] = (id) =>
			{
				Match m = RGUtility.CellReferenceRegex.Match(id);
				if (m.Success)
				{
					// parse cell row position
					int row = 0;
					int.TryParse(m.Groups[2].Value, out row);
					// convert it to index (index=row-1)
					row--;
					int col = RGUtility.GetAlphaNumber(m.Groups[1].Value);

					ReoGridCell referencedCell = CreateAndGetCell(row, col);

					if (referencedCell == cell)
					{
						return 0;
					}

					referencedCells.Add(referencedCell);

					return referencedCell == null || referencedCell.Data == null ? 0 : referencedCell.Data;
				}
				else
					return null;
			};

			if (!referenceCellListExisted)
			{
				formulaCells[cell] = referencedCells = new List<ReoGridCell>();
			}
			if (!referencedRangeExisted)
			{
				formulaRanges[cell] = referencedRanges = new List<ReferenceRange>();
			}

			string expression = cell.Formula.Substring(1);

			object data = null;

			try
			{
				// preprocess range syntax
				expression = RGUtility.RangeReferenceRegex.Replace(expression, (m) =>
				{
					if (m.Groups["to_col"].Length > 0 && m.Groups["to_row"].Length > 0
						&& m.Groups["from_col"].Length > 0 && m.Groups["from_row"].Length > 0)
					{
						// range
						int fromRow = -1;
						if (!int.TryParse(m.Groups["from_row"].Value, out fromRow)) return "null";
						fromRow--;

						int toRow = -1;
						if (!int.TryParse(m.Groups["to_row"].Value, out toRow)) return "null";
						toRow--;

						int fromCol = RGUtility.GetAlphaNumber(m.Groups["from_col"].Value);
						int toCol = RGUtility.GetAlphaNumber(m.Groups["to_col"].Value);

						if (fromRow < 0) fromRow = 0;
						if (fromCol < 0) fromCol = 0;
						if (toRow > RowCount - 1) toRow = RowCount - 1;
						if (toCol > RowCount - 1) toCol = ColCount - 1;

						ReoGridRange range = new ReoGridRange(fromRow, fromCol, toRow - fromRow + 1, toCol - fromCol + 1);

						// check for circular references
						if (range.Contains(cell.Pos))
						{
							// better to throw exception ?
							throw new CircularReferenceException();
							//MessageBox.Show("Circular reference detected. Value of current reference will be null.",
							//	Application.ProductName, MessageBoxButtons.OK);
						}

						referencedRanges.Add(new ReferenceRange(CreateAndGetCell(range.Row, range.Col), CreateAndGetCell(range.Row2, range.Col2)));
						return string.Format("new Range({0},{1},{2},{3})", range.Row, range.Col, range.Rows, range.Cols);
					}
					else
						return m.Value;
				});

				data = srm.CalcExpression(expression, ctx);
			}
			catch (ReoScriptException ex)
			{
				data = "#ERR: " + ex.Message;
				Logger.Log("formula", string.Format("error in cell[{0},{1}]: {2}", cell.Row, cell.Col, ex.Message));
			}

			UpdateCellData(cell, data);
		}
#endif // EX_SCRIPT

		internal void AfterUpdateCellText(ReoGridCell cell)
		{
#if EX_SCRIPT
			UpdateReferencedFormulaCells(cell);
#endif

			// update render text align when data format changed
			ReoGridStyleUtility.UpdateCellRenderAlign(this, cell);

			UpdateCellTextBounds(null, cell, true);

			// raise text changed event
			RaiseCellDataChangedEvent(cell);

			InvalidateCanvas();
		}

		/// <summary>
		/// Set body of cell at specified position of grid
		/// </summary>
		/// <param name="row">number of row</param>
		/// <param name="col">number of column</param>
		/// <param name="body">body to be set</param>
		public void SetCellBody(int row, int col, ICellBody body)
		{
			var cell = this.cells[row, col];
			if (cell == null && body == null) return;

			cell = CreateCell(row, col);
			cells[row, col] = cell;
			SetCellBody(cell, body);
		}

		/// <summary>
		/// Set body of cell into specified row
		/// </summary>
		/// <param name="cell">cell to be set</param>
		/// <param name="body">body to be set</param>
		public void SetCellBody(ReoGridCell cell, ICellBody body)
		{
			cell.Body = body;
			
			if (body != null)
			{
				body.OnSetup(this, cell);
				UpdateCellFont(cell, null, false);
			}

			cell.UpdateContentBounds();

			InvalidateCanvas();
		}

		/// <summary>
		/// Remove cell body from specified cell
		/// </summary>
		/// <param name="pos">position of specified cell</param>
		public void RemoveCellBody(ReoGridPos pos)
		{
			RemoveCellBody(pos.Row, pos.Col);
		}

		/// <summary>
		/// Remove cell body from specified cell
		/// </summary>
		/// <param name="row">number of row</param>
		/// <param name="row">number of column</param>
		public void RemoveCellBody(int row, int col)
		{
			var cell = cells[row, col];
			if (cell != null)
			{
				cell.body = null;
				InvalidateCanvas();
			}
		}

		public object GetCellData(string id)
		{
			return GetCellData(new ReoGridPos(id));
		}

		/// <summary>
		/// Get data from specified cell
		/// </summary>
		/// <param name="pos">Position of cell to get data</param>
		/// <returns>Data of cell</returns>
		public object GetCellData(ReoGridPos pos) { return GetCellData(pos.Row, pos.Col); }

		/// <summary>
		/// Get data from specified cell
		/// </summary>
		/// <param name="row">Row of cell</param>
		/// <param name="col">Column of cell</param>
		/// <returns>Data of cell</returns>
		public object GetCellData(int row, int col)
		{
			if (row < 0 || row >= this.rows.Count) return null;
			if (col < 0 || col >= this.cols.Count) return null;

			return (cells[row, col] == null) ? null : cells[row, col].Data;
		}

		public string GetCellText(string id)
		{
			return GetCellText(new ReoGridPos(id));
		}

		/// <summary>
		/// Get formatted cell text from spcified position
		/// </summary>
		/// <param name="pos">position to be get</param>
		/// <returns>formatted cell's text</returns>
		public string GetCellText(ReoGridPos pos)
		{
			return GetCellText(pos.Row, pos.Col);
		}

		/// <summary>
		/// Get formatted cell text from specified position
		/// </summary>
		/// <param name="row">Row of position</param>
		/// <param name="col">Col of position</param>
		/// <returns>Text of cell</returns>
		public string GetCellText(int row, int col)
		{
			var cell = cells[row, col];
			if (cell == null) return string.Empty;
			return string.IsNullOrEmpty(cell.Display) ? string.Empty : cell.Display;
		}

		/// <summary>
		/// Get data array from specified range
		/// </summary>
		/// <param name="range">Range to get data array</param>
		/// <returns>Data array of range</returns>
		public object[,] GetRangeData(ReoGridRange range)
		{
			ReoGridRange fixedRange = FixRange(range);
			object[,] data = new object[fixedRange.Rows, fixedRange.Cols];
			for (int r = fixedRange.Row, r2 = 0; r <= fixedRange.Row2; r++, r2++)
			{
				for (int c = fixedRange.Col, c2 = 0; c <= fixedRange.Col2; c++, c2++)
				{
					data[r2, c2] = (cells[r, c] == null) ? null : cells[r, c].Data;
				}
			}
			return data;
		}

		/// <summary>
		/// Remove all data contained in specified range
		/// </summary>
		/// <param name="range">Range to remove data array</param>
		public void DeleteRangeData(ReoGridRange range)
		{
			for (int r = range.Row; r <= range.Row2; r++)
			{
				for (int c = range.Col; c <= range.Col2; c++)
				{
					ReoGridCell cell = GetCell(r, c);
					if (cell != null)
					{
						cell.Data = null;
						cell.Display = string.Empty;
						cell.TextBounds = Rectangle.Empty;

						// TODO: auto adjust row height
					}
				}
			}
			InvalidateCanvas();
		}

		/// <summary>
		/// Set data array to specified range
		/// </summary>
		/// <param name="range">Range to set data array</param>
		/// <param name="data">Data array of range</param>
		public void SetRangeData(ReoGridRange range, object data)
		{
			if (range.IsEmpty) return;

			if (data is object[,])
			{
				var arr = (object[,])data;

				int rows = arr.GetLength(0);
				int cols = arr.GetLength(1);

				int maxRows = Math.Min(rows, range.Rows);
				int maxCols = Math.Min(cols, range.Cols);

				for (int r = 0; r < maxRows; r++)
				{
					for (int c = 0; c < maxCols; c++)
					{
						int targetRow = range.Row + r;
						int targetCol = range.Col + c;

						if (targetRow < this.rows.Count
							&& targetCol < this.cols.Count)
						{
							SetCellData(targetRow, targetCol, arr[r, c]);
						}
					}
				}
			}
			else if (data is PartialGrid)
			{
				SetPartialGrid(range, (PartialGrid)data);
			}
		}

		/// <summary>
		/// Set range data format type
		/// </summary>
		/// <param name="range">Range to be set</param>
		/// <param name="format">Format to be set</param>
		/// <param name="dataFormatArgs">Arugments belong to format type to be set</param>
		public void SetRangeDataFormat(ReoGridRange range, CellDataFormatFlag format, object dataFormatArgs)
		{
			ReoGridRange fixedRange = FixRange(range);

			int rend = fixedRange.Row2;
			int cend = fixedRange.Col2;

#if EX_SCRIPT
			List<ReoGridCell> formulaDirtyCells = new List<ReoGridCell>(10);
#endif

			using (Graphics g = Graphics.FromHwnd(this.Handle))
			{
				for (int r = fixedRange.Row; r <= rend; r++)
				{
					for (int c = fixedRange.Col; c <= cend; )
					{
						ReoGridCell cell = CreateAndGetCell(r, c);

						cell.DataFormat = format;
						cell.DataFormatArgs = dataFormatArgs;

						string oldDisplay = cell.Display;

						DataFormatterManager.Instance.FormatCell(this, cell);

						if (oldDisplay != cell.Display)
						{
							UpdateCellTextBounds(g, cell, true);
						}

#if EX_SCRIPT
						// cells
						foreach (var formulaCell in formulaCells)
						{
							if (formulaCell.Value.Any(fc => fc == cell))
							{
								if (!formulaDirtyCells.Contains(formulaCell.Key))
									formulaDirtyCells.Add(formulaCell.Key);
							}
						}

						// ranges
						foreach (var referencedRange in formulaRanges)
						{
							if (referencedRange.Value.Any(rr => rr.Contains(cell)))
							{
								if (!formulaDirtyCells.Contains(referencedRange.Key))
								{
									formulaDirtyCells.Add(referencedRange.Key);
								}
							}
						}
#endif

						c += cell.Colspan > 1 ? cell.Colspan : 1;
					}
				}
			}

#if EX_SCRIPT
			foreach (var cell in formulaDirtyCells)
			{
				RecalcCell(cell);
			}
#endif

			InvalidateCanvas();

		}

		internal void RaiseCellDataChangedEvent(ReoGridCell cell)
		{
			if (CellDataChanged != null) CellDataChanged(this, new RGCellEventArgs(cell));

#if EX_SCRIPT
			RaiseScriptEvent("ontextchange", new RSCellObject(this, cell));
#endif
		}

		/// <summary>
		/// Event raised when any data has been changed
		/// </summary>
		public event EventHandler<RGCellEventArgs> CellDataChanged;
		#endregion

		#region IME
		protected override void WndProc(ref Message m)
		{
			// Chinese and Japanese IME will send this message 
			// before start to accept user's input
			if (m.Msg == (int)Win32.WMessages.WM_IME_STARTCOMPOSITION)
			{
				StartEdit();
			}
			else if (m.Msg == (int)Win32.WMessages.WM_MOUSEHWHEEL)
			{
				// get an overflow error by my logitech mouse t620 (T_T)
				// fixed by (int)(long) cast
				int delta = (((int)(long)(m.WParam) & 0xff) >> 16); // get delta
				viewportController.ScrollViews(ScrollDirection.Horizontal, -delta, 0);
			}

			base.WndProc(ref m);
		}
		#endregion

		#region Load, Save & Reset
		#region Reset
		private void InitGrid()
		{
			InitGrid(DefaultRows, DefaultCols);
		}

		private void InitGrid(int rows, int cols)
		{
			Resize(rows, cols);

			RootStyle = new ReoGridRangeStyle(DefaultStyle);

			selOriginal = selEnd = new ReoGridPos(0, 0);
			SelectRangeByPos(0, 0, 0, 0);

#if EX_SCRIPT
			InitSRM();

			RaiseScriptEvent("onload");
#endif // EX_SCRIPT
		}
		private void ClearGrid()
		{
			// hidden edit textbox
			if (editTextbox.Visible) editTextbox.Visible = false;

			endEditProcessing = false;

			// clear row outlines
			if (this.outlines != null)
			{
				ClearOutlines(RowOrColumn.Row | RowOrColumn.Column);
			}

#if EX_SCRIPT
			// clear formula referenced cells and ranges
			formulaCells.Clear();
			formulaRanges.Clear();

			if (srm != null)
			{
				RaiseScriptEvent("unload");

				//
				// do not reset SRM and clear script 
				// in order to keep event binds
				//
				// force reset srm
				//srm.Reset();
				//
				// clear script
				//script = null;
				//
				/////////////////////////////////////////
			}
#endif // EX_SCRIPT

			// unfreeze rows and columns
			Unfreeze();

			// reset viewport controller
			viewportController.Reset();

			// TODO: release objects inside cells and borders
			cells = new RegularTreeArray<ReoGridCell>();
			hBorders = new RegularTreeArray<ReoGridHBorder>();
			vBorders = new RegularTreeArray<ReoGridVBorder>();

			// clear header & index
			this.rows.Clear();
			this.cols.Clear();
		}

		/// <summary>
		/// Reset control to default status.
		/// </summary>
		public void Reset()
		{
			Reset(DefaultRows, DefaultCols);
		}

		/// <summary>
		/// Reset control and initialize to specified size
		/// </summary>
		/// <param name="rows">number of rows to be set after resting</param>
		/// <param name="cols">number of columns to be set after reseting</param>
		public void Reset(int rows, int cols)
		{
			// cancel editing mode
			EndEdit(ReoGridEndEditReason.Cancel);

			// reset scale factor, need this?
			SetScale(1f, Point.Empty);

			// clear grid
			ClearGrid();

			// restore default cell size
			defaultRowHeight = InitDefaultRowHeight;
			defaultColumnWidth = InitDefaultColumnWidth;

			// restore UI
			SetSettings(ReoGridSettings.View_ShowGridLine 
				| ReoGridSettings.View_ShowScrolls 
				| ReoGridSettings.View_ShowHeaders);

			// reset ActionManager
			ActionManager.InstanceForObject(this).Reset();

			// init grid
			InitGrid(rows, cols);

			// repaint
			InvalidateCanvas();

			// raise reseting event
			if (Resetted != null) Resetted(this, null);
		}

		/// <summary>
		/// Event raised when control resetted
		/// </summary>
		public event EventHandler Resetted;
		#endregion // Reset
		#region Load
		/// <summary>
		/// Load grid from specified file
		/// </summary>
		/// <param name="file">Path of file to load grid</param>
		/// <returns>True if file loaded successfully</returns>
		public bool Load(string file)
		{
			using (FileStream fs = File.Open(file, FileMode.Open, FileAccess.Read))
			{
				bool rs = Load(fs);
			
				// raise file loading event
				if (rs && FileLoaded != null) FileLoaded(this, new RGFileLoadedEventArgs(file));

				return rs;
			}
		}

		/// <summary>
		/// Load grid from specified stream
		/// </summary>
		/// <param name="s">Stream to load grid</param>
		/// <returns>True if stream loaded successfully</returns>
		public bool Load(Stream s)
		{
#if DEBUG
			Stopwatch stop = Stopwatch.StartNew();
#endif

			XmlSerializer xmlReader = new XmlSerializer(typeof(RGXmlBody));
			RGXmlBody body;
			try
			{
				body = xmlReader.Deserialize(s) as RGXmlBody;
			}
			catch(Exception ex)
			{
				MessageBox.Show("Read error: " + ex.Message);
				return false;
			}

			ClearGrid();

			ScaleFactor = 1f;

			// todo: clear current view controller and viewport position
			CultureInfo culture = Thread.CurrentThread.CurrentCulture;

			// head
			if (body.head != null)
			{
				if (body.head.settings != null)
				{
					SetSettings(ReoGridSettings.View_ShowGridLine, body.head.settings.showGrid);
				}

				// apply culture
				if (!string.IsNullOrEmpty(body.head.culture))
				{
					try
					{
						culture = new CultureInfo(body.head.culture);
					}
					catch (Exception)
					{
						Logger.Log("load", "warning: unsupported culture: " + body.head.culture);
					}
				}

				defaultRowHeight = body.head.defaultRowHeight;
				defaultColumnWidth = body.head.defaultColumnWidth;
			}

			// root style
			if (body.style != null) rootStyle = ReoGridStyleUtility.ConvertFromXmlStyle(this, body.style, culture);

			// cols and rows
			Resize(body.head.rows, body.head.cols);

			foreach (RGXmlColHead col in body.cols)
			{
				var colhead = this.cols[col.col];

				colhead.Width = col.width;
				colhead.LastWidth = col.lastWidth;
				colhead.IsAutoWidth = TextFormatHelper.IsSwitchOn(col.autoWidth);
				
				colhead.Style = new ReoGridRangeStyle(rootStyle);
				ReoGridStyleUtility.CopyStyle(ReoGridStyleUtility.ConvertFromXmlStyle(
					this, col.style, culture), cols[col.col].Style);
			}
			foreach (RGXmlRowHead row in body.rows)
			{
				var rowhead = this.rows[row.row];

				rowhead.Height = row.height;
				rowhead.LastHeight = row.lastHeight;
				rowhead.IsAutoHeight = TextFormatHelper.IsSwitchOn(row.autoHeight);

				rowhead.Style = new ReoGridRangeStyle(rootStyle);
				ReoGridStyleUtility.CopyStyle(ReoGridStyleUtility.ConvertFromXmlStyle(
					this, row.style, culture), rows[row.row].Style);
			}

			int left = (this.cols.Count > 0 ? cols[0].Width : 0);
			for (int c = 1; c < this.cols.Count; c++)
			{
				cols[c].Left = left;
				left += cols[c].Width;
			}

			int top = (this.rows.Count > 0 ? rows[0].Height : 0);
			for (int r = 1; r < this.rows.Count; r++)
			{
				rows[r].Top = top;
				top += rows[r].Height;
			}

			if (body.head.outlines != null)
			{
				if (body.head.outlines.rowOutlines != null)
				{
					foreach (var xmlRowOutline in body.head.outlines.rowOutlines)
					{
						var outline = AddOutline(RowOrColumn.Row, xmlRowOutline.start, xmlRowOutline.count);
						if (xmlRowOutline.collapsed) outline.Collapse();
					}
				}

				if (body.head.outlines.colOutlines != null)
				{
					foreach (var xmlColOutline in body.head.outlines.colOutlines)
					{
						var outline = AddOutline(RowOrColumn.Column, xmlColOutline.start, xmlColOutline.count);
						if (xmlColOutline.collapsed) outline.Collapse();
					}
				}
			}

			foreach (RGXmlHBorder b in body.hborder)
			{
				SetHBorders(b.row, b.col, b.cols, b.StyleGridBorder, XmlFileFormatHelper.DecodeHBorderOwnerPos(b.pos));
			}

			foreach (RGXmlVBorder b in body.vborder)
			{
				SetVBorders(b.row, b.col, b.rows, b.StyleGridBorder, XmlFileFormatHelper.DecodeVBorderOwnerPos(b.pos));
			}

			foreach (RGXmlCell xmlCell in body.cells)
			{
				int rowspan = 1;
				int colspan = 1;
				if (xmlCell.rowspan != null) int.TryParse(xmlCell.rowspan, out rowspan);
				if (xmlCell.colspan != null) int.TryParse(xmlCell.colspan, out colspan);

				ReoGridCell cell = CreateCell(xmlCell.row, xmlCell.col);

				if (rowspan > 1 || colspan > 1)
				{
					MergeRange(new ReoGridRange(xmlCell.row, xmlCell.col, rowspan, colspan));
				}

				cell.DataFormat = XmlFileFormatHelper.DecodeCellDataFormat(xmlCell.dataFormat);

				if (xmlCell.style != null)
				{
					ReoGridRangeStyle style = ReoGridStyleUtility.ConvertFromXmlStyle(this, xmlCell.style, culture);
					if (style != null)
					{
						SetCellStyle(cell, style);
					}
				}

				if (xmlCell.dataFormatArgs != null && cell.DataFormat != CellDataFormatFlag.General)
				{
					RGXmlCellDataFormatArgs xmlFormatArgs = xmlCell.dataFormatArgs;

					object formatArgs = null;

					switch (cell.DataFormat)
					{
						case CellDataFormatFlag.Number:
							formatArgs = new NumberDataFormatter.NumberFormatArgs
							{
								DecimalPlaces = (short)TextFormatHelper.GetFloatValue(xmlFormatArgs.decimalPlaces, 2, culture),
								NegativeStyle = XmlFileFormatHelper.DecodeNegativeNumberStyle(xmlFormatArgs.negativeStyle),
								UseSeparator = TextFormatHelper.IsSwitchOn(xmlFormatArgs.useSeparator),
							};
							break;

						case CellDataFormatFlag.DateTime:
							formatArgs = new DateTimeDataFormatter.DateTimeFormatArgs
							{
								CultureName = xmlFormatArgs.culture,
								Format = xmlFormatArgs.pattern,
							};
							break;

						case CellDataFormatFlag.Currency:
							formatArgs = new CurrencyDataFormatter.CurrencyFormatArgs
							{
								DecimalPlaces = (short)TextFormatHelper.GetFloatValue(xmlFormatArgs.decimalPlaces, 2, culture),
								CultureEnglishName = xmlFormatArgs.culture,
								NegativeStyle = XmlFileFormatHelper.DecodeNegativeNumberStyle(xmlFormatArgs.negativeStyle),
								Symbol = xmlFormatArgs.pattern,
							};
							break;

						case CellDataFormatFlag.Percent:
							formatArgs = new PercentDataFormatter.PercentFormatArgs
							{
								DecimalPlaces = (short)TextFormatHelper.GetFloatValue(xmlFormatArgs.decimalPlaces, 0, culture),
							};
							break;
					}

					if (formatArgs != null) cell.DataFormatArgs = formatArgs;
				}

				// data or formula
				if (!string.IsNullOrEmpty(xmlCell.data))
				{
					SetCellData(cell, xmlCell.data);
				}
			}

			if (body.head.freezeRow > 0 || body.head.freezeCol > 0)
			{
				FreezeToCell(body.head.freezeRow, body.head.freezeCol);
			}
			else
			{
				UpdateViewportController();
			}

			// put the selection to left-top
			if (selectionMode != ReoGridSelectionMode.None)
			{
				SelectRangeByPos(0, 0, 0, 0);
			}

#if EX_SCRIPT
			// load scripts
			// TODO: include others scripts as resource document
			if (body.head != null && body.head.script != null
				&& !string.IsNullOrEmpty(body.head.script.content))
			{
				script = body.head.script.content;

				// initialize SRM
				InitSRM();
			}
			else
			{
				script = null;
			}

			if (this.srm != null && HasSettings(ReoGridSettings.Script_AutoRunOnload))
			{
				RaiseScriptEvent("onload");
			}

#endif // EX_SCRIPT


#if DEBUG
			stop.Stop();
			Debug.WriteLine("file loaded: " + stop.ElapsedMilliseconds + " ms.");
#endif

			return true;
		}

		/// <summary>
		/// Event raised when grid loaded from file
		/// </summary>
		public event EventHandler<RGFileLoadedEventArgs> FileLoaded;
	
		#endregion // Load
		#region Save
		/// <summary>
		/// save grid into specified file
		/// </summary>
		/// <param name="path">path of file to save grid</param>
		/// <returns>true if file saved successfully</returns>
		public bool Save(string path)
		{
			return Save(path, null);
		}

		/// <summary>
		/// Save grid into specified file
		/// </summary>
		/// <param name="path">Path of file to save grid</param>
		/// <param name="editorProgram">name of editor program saved in file</param>
		/// <returns>True if grid saved successfully</returns>
		public bool Save(string path, string editorProgram)
		{
			using (FileStream fs = new FileStream(path, FileMode.Create))
			{
				bool rs = Save(fs, editorProgram);
				// raise file saving event
				if (rs && FileSaved != null)
				{
					FileSaved(this, new RGFileSavedEventArgs(path));
				}
				return rs;
			}
		}

		/// <summary>
		/// Save into stream
		/// </summary>
		/// <param name="s">stream used to output xml</param>
		/// <param name="editorProgram">Grid Editor Name</param>
		/// <returns></returns>
		public bool Save(Stream s)
		{
			return Save(s, null);
		}

		/// <summary>
		/// Save grid into specified stream
		/// </summary>
		/// <param name="s">Stream to save grid</param>
		/// <param name="editorProgram">Editor program name</param>
		/// <returns>True if grid saved successfully</returns>
		public bool Save(Stream s, string editorProgram)
		{
			if (string.IsNullOrEmpty(editorProgram))
			{
				editorProgram = "ReoGrid Core " + Application.ProductVersion;
			}

			RGXmlBody body = new RGXmlBody()
			{
				head = new RGXmlHead
				{
					cols = this.cols.Count,
					rows = this.rows.Count,
					defaultColumnWidth = defaultColumnWidth,
					defaultRowHeight = defaultRowHeight,
					settings = new RGXmlGridSetting
					{
						showGrid = this.HasSettings(ReoGridSettings.View_ShowGridLine),
					},
					culture = Thread.CurrentThread.CurrentCulture.Name,
					editor = editorProgram,
#if EX_SCRIPT
					script = new RGXmlScript() { content = this.script },
#endif // EX_SCRIPT
				},
				style = ReoGridStyleUtility.ConvertToXmlStyle(this.rootStyle),
			};

			if (this.viewportController is NormalViewportController)
			{
				ReoGridPos freezePos = ((NormalViewportController)this.viewportController).FreezePos;
				body.head.freezeRow = freezePos.Row;
				body.head.freezeCol = freezePos.Col;
			}

			if (this.outlines != null)
			{
				Action<OutlineCollection<ReoGridOutline>, List<RGXmlOutline>> addOutliens = (outlines, xmlOutlines) =>
				{
					outlines.IterateOutlines((outline) =>
					{
						xmlOutlines.Add(new RGXmlOutline
						{
							start = outline.Start,
							count = outline.Count,
							collapsed = outline.Collapsed,
						});

						return true;
					});
				};

				var rowOutlines = GetOutlines(RowOrColumn.Row);
				if (rowOutlines != null)
				{
					if (body.head.outlines == null)
					{
						body.head.outlines = new RGXmlOutlineList();
					}

					body.head.outlines.rowOutlines = new List<RGXmlOutline>();
					addOutliens(rowOutlines, body.head.outlines.rowOutlines);
				}

				var colOutlines = GetOutlines(RowOrColumn.Column);
				if (colOutlines != null)
				{
					if (body.head.outlines == null)
					{
						body.head.outlines = new RGXmlOutlineList();
					}

					body.head.outlines.colOutlines = new List<RGXmlOutline>();
					addOutliens(colOutlines, body.head.outlines.colOutlines);
				}
			}

			// row-heads
			foreach(var r in rows)
			{
				if (r.Height != defaultRowHeight || r.Style != null)
				{
					ReoGridRangeStyle checkedStyle = ReoGridStyleUtility.DistinctStyle(r.Style, rootStyle);
					if (r.Height != defaultRowHeight || checkedStyle != null)
					{
						body.rows.Add(new RGXmlRowHead
						{
							row = r.Row,
							height = r.Height,
							lastHeight = r.LastHeight,
							autoHeight = TextFormatHelper.EncodeBool(r.IsAutoHeight),

							style = ReoGridStyleUtility.ConvertToXmlStyle(checkedStyle),
						});
					}
				}
			}

			// col-heads
			foreach(var c in cols)
			{
				if (c.Width != defaultRowHeight || c.Style != null)
				{
					ReoGridRangeStyle checkedStyle = ReoGridStyleUtility.DistinctStyle(c.Style, rootStyle);
					if (c.Width != defaultColumnWidth || checkedStyle != null)
					{
						body.cols.Add(new RGXmlColHead
						{
							col = c.Col,
							width = c.Width,
							lastWidth = c.LastWidth,
							autoWidth = TextFormatHelper.EncodeBool(c.IsAutoWidth),

							style = ReoGridStyleUtility.ConvertToXmlStyle(checkedStyle),
						});
					}
				}
			}

			// h-borders
			for (int r = 0; r <= this.rows.Count; r++)
			{
				for (int c = 0; c < this.cols.Count; )
				{
					ReoGridHBorder cellBorder = hBorders[r, c];
					if (cellBorder != null && cellBorder.Cols > 0 && cellBorder.Border != null
						&& cellBorder.Border.Style != BorderLineStyle.None)
					{
						body.hborder.Add(new RGXmlHBorder(r, c, cellBorder.Cols, cellBorder.Border, cellBorder.Pos));
						c += cellBorder.Cols;
					}
					else
						c++;
				}
			}

			// v-borders
			for (int c = 0; c <= this.cols.Count; c++)
			{
				for (int r = 0; r < this.rows.Count; )
				{
					ReoGridVBorder cellBorder = vBorders[r, c];
					if (cellBorder != null && cellBorder.Rows > 0 && cellBorder.Border != null
						&& cellBorder.Border.Style != BorderLineStyle.None)
					{
						body.vborder.Add(new RGXmlVBorder(r, c, cellBorder.Rows, cellBorder.Border, cellBorder.Pos));
						r += cellBorder.Rows;
					}
					else
						r++;
				}
			}

			// save cells
			for (int r = 0; r < this.rows.Count; r++)
			{
				for (int c = 0; c < this.cols.Count; c++)
				{
					ReoGridCell cell = cells[r, c];

					if (cell != null && cell.Rowspan > 0 && cell.Colspan > 0)
					{
						bool addCell = false;

						if (cell.Data != null || cell.Rowspan > 1 || cell.Colspan > 1) addCell = true;

						RGXmlCellStyle xmlStyle = null;
						if (cell.Style != null)
						{
							xmlStyle = ReoGridStyleUtility.ConvertToXmlStyle(
								ReoGridStyleUtility.CheckAndRemoveCellStyle(this, cell));

							if (xmlStyle != null) addCell = true;
						}

						RGXmlCellDataFormatArgs xmlFormatArgs = null;
						if (cell.DataFormat != CellDataFormatFlag.General)
						{
							addCell = true;

							switch (cell.DataFormat)
							{
								case CellDataFormatFlag.Number:
									NumberDataFormatter.NumberFormatArgs nargs = (NumberDataFormatter.NumberFormatArgs)cell.DataFormatArgs;
									xmlFormatArgs = new RGXmlCellDataFormatArgs();
									xmlFormatArgs.decimalPlaces = nargs.DecimalPlaces.ToString();
									xmlFormatArgs.negativeStyle = XmlFileFormatHelper.EncodeNegativeNumberStyle(nargs.NegativeStyle);
									xmlFormatArgs.useSeparator = TextFormatHelper.EncodeBool(nargs.UseSeparator);
									break;

								case CellDataFormatFlag.DateTime:
									DateTimeDataFormatter.DateTimeFormatArgs dargs = (DateTimeDataFormatter.DateTimeFormatArgs)cell.DataFormatArgs;
									xmlFormatArgs = new RGXmlCellDataFormatArgs();
									xmlFormatArgs.culture = dargs.CultureName;
									xmlFormatArgs.pattern = dargs.Format;
									break;

								case CellDataFormatFlag.Currency:
									CurrencyDataFormatter.CurrencyFormatArgs cargs = (CurrencyDataFormatter.CurrencyFormatArgs)cell.DataFormatArgs;
									xmlFormatArgs = new RGXmlCellDataFormatArgs();
									xmlFormatArgs.decimalPlaces = cargs.DecimalPlaces.ToString();
									xmlFormatArgs.culture = cargs.CultureEnglishName;
									xmlFormatArgs.negativeStyle = XmlFileFormatHelper.EncodeNegativeNumberStyle(cargs.NegativeStyle);
									xmlFormatArgs.pattern = cargs.Symbol;
									break;

								case CellDataFormatFlag.Percent:
									PercentDataFormatter.PercentFormatArgs pargs = (PercentDataFormatter.PercentFormatArgs)cell.DataFormatArgs;
									xmlFormatArgs = new RGXmlCellDataFormatArgs();
									xmlFormatArgs.decimalPlaces = pargs.DecimalPlaces.ToString();
									break;
							}
						}

						if (addCell)
						{
							body.cells.Add(new RGXmlCell()
							{
								row = r,
								col = c,
								colspan = cell.Colspan == 1 ? null : cell.Colspan.ToString(),
								rowspan = cell.Rowspan == 1 ? null : cell.Rowspan.ToString(),
								data = !string.IsNullOrEmpty(cell.Formula) ? cell.Formula :
									(cell.Data != null ? cell.Data.ToString() : string.Empty),
								style = xmlStyle,
								dataFormat = XmlFileFormatHelper.EncodeCellDataFormat(cell.DataFormat),
								dataFormatArgs = (xmlFormatArgs == null || xmlFormatArgs.IsEmpty) ? null : xmlFormatArgs,
							});
						}
					}
				}
			}

			XmlSerializer xmlWriter = new XmlSerializer(typeof(RGXmlBody));
			xmlWriter.Serialize(s, body);
			
			return true;
		}

		/// <summary>
		/// Event raised when grid saved into a file
		/// </summary>
		public event EventHandler<RGFileSavedEventArgs> FileSaved;
		#endregion // Save
		#endregion // Load, Save & Reset

		#region Context Menu Strip
		private ContextMenuStrip colHeadContextMenuStrip;

		/// <summary>
		/// Context menu strip displayed when user click on header of column
		/// </summary>
		public ContextMenuStrip ColHeadContextMenuStrip
		{
			get { return colHeadContextMenuStrip; }
			set { colHeadContextMenuStrip = value; }
		}

		private ContextMenuStrip rowHeadContextMenuStrip;

		/// <summary>
		/// Context menu strip displayed when user click on header of row
		/// </summary>
		public ContextMenuStrip RowHeadContextMenuStrip
		{
			get { return rowHeadContextMenuStrip; }
			set { rowHeadContextMenuStrip = value; }
		}

		private ContextMenuStrip cellContextMenuStrip;

		/// <summary>
		/// Context menu strip displayed when user click on cells
		/// </summary>
		public ContextMenuStrip CellContextMenuStrip
		{
			get { return cellContextMenuStrip; }
			set { cellContextMenuStrip = value; }
		}
		#endregion

		#region Settings
		private ReoGridSettings settings;

		//[DefaultValue(StyleGridSettings.Edit_All|StyleGridSettings.View_ShowGrid)]
		internal ReoGridSettings Settings { get { return settings; } set { settings = value; } }

		/// <summary>
		/// Set control settings
		/// </summary>
		/// <param name="settings">Setting flags to be set</param>
		public void SetSettings(ReoGridSettings settings)
		{
			this.settings |= settings;

			if (this.SettingsChanged != null)
			{
				this.SettingsChanged(this, new SettingsChangedEventArgs()
				{
					AddedSettings = settings,
				});
			}
		}

		/// <summary>
		/// Set control settings
		/// </summary>
		/// <param name="settings">Setting flags to be set</param>
		/// <param name="value">value of setting to be set</param>
		public void SetSettings(ReoGridSettings settings, bool value)
		{
			if (value)
				SetSettings(settings);
			else
				RemoveSetting(settings);

			if ((settings & ReoGridSettings.View_ShowHorScroll) == ReoGridSettings.View_ShowHorScroll
				|| (settings & ReoGridSettings.View_ShowVerScroll) == ReoGridSettings.View_ShowVerScroll)
			{
				// horizontal scrollbar
				bottomPanel.Visible = HasSettings(ReoGridSettings.View_ShowHorScroll);

				// vertical scrollbar
				rightPanel.Visible = HasSettings(ReoGridSettings.View_ShowVerScroll);

				UpdateCanvasBounds();
			}

			if ((settings & ReoGridSettings.View_ShowColumnHeader) == ReoGridSettings.View_ShowColumnHeader
				|| (settings & ReoGridSettings.View_ShowRowHeader) == ReoGridSettings.View_ShowRowHeader)
			{
				VisibleViews head = VisibleViews.None;
				if ((settings & ReoGridSettings.View_ShowColumnHeader) == ReoGridSettings.View_ShowColumnHeader)
					head |= VisibleViews.ColumnHead;
				if ((settings & ReoGridSettings.View_ShowRowHeader) == ReoGridSettings.View_ShowRowHeader)
					head |= VisibleViews.RowHead;

				((NormalViewportController)viewportController).SetHeadVisible(head, value);
			}
			else
			{
				UpdateViewportController();
			}

			if (this.Visible) this.InvalidateCanvas();
		}

		/// <summary>
		/// Remove control settings
		/// </summary>
		/// <param name="settings">Setting flags to be removed</param>
		public void RemoveSetting(ReoGridSettings settings)
		{
			this.settings &= ~settings;

			if (this.SettingsChanged != null)
			{
				this.SettingsChanged(this, new SettingsChangedEventArgs()
				{
					RemovedSettings = settings,
				});
			}
		}

		/// <summary>
		/// Determine whether specified settings have been set
		/// </summary>
		/// <param name="setting">Setting flags to be checked</param>
		/// <returns>True if all settings has setted</returns>
		public bool HasSettings(ReoGridSettings setting)
		{
			return (this.settings & setting) == setting;
		}

		public event EventHandler<SettingsChangedEventArgs> SettingsChanged;
		#endregion

		#region Export
		/// <summary>
		/// Export grid as html
		/// </summary>
		/// <param name="s">The stream used to export html</param>
		public void ExportAsHTML(Stream s)
		{
			RGHTMLExporter exporter = new RGHTMLExporter(this);
			exporter.Export(s);
		}
	
		#endregion

		#region Filter & Sort
		
		// todo
		private void SortData(ReoGridRange range, RowOrColumn dir, int index, bool asc)
		{
			var backupGrid = GetPartialGrid(range, PartialGridCopyFlag.CellData, ExPartialGridCopyFlag.None);

			int endRow = range.Row2;
			int endCol = range.Col2;

			for (var r = range.Row; r <= endRow; r++)
			{

			}
		}

		#endregion // Filter & Sort
	}

	#region Enums
	//internal enum ReoGridColumnType
	//{
	//  Index,
	//  Static,
	//  Text,
	//  Dropdown,
	//  Combo,
	//  Button,
	//  Link,
	//  Checkbox,
	//  Numeric,
	//  Custom,
	//}

	/// <summary>
	/// Reason for ending of cell edit
	/// </summary>
	public enum ReoGridEndEditReason
	{
		/// <summary>
		/// User edit has done normally
		/// </summary>
		NormalFinish,

		/// <summary>
		/// User has cancelled edit operation
		/// </summary>
		Cancel,
	}

	/// <summary>
	/// Selection Mode for Control
	/// </summary>
	public enum ReoGridSelectionMode
	{
		/// <summary>
		/// Do not allow to select anything
		/// </summary>
		None,

		/// <summary>
		/// Only allow to select single cell
		/// </summary>
		Cell,

		/// <summary>
		/// Allow to select both single cell and ranges. (Default)
		/// </summary>
		Range,
	}

	/// <summary>
	/// Selection Style for Control
	/// </summary>
	public enum ReoGridSelectionStyle
	{
		/// <summary>
		/// No selection will be drawn
		/// </summary>
		None,

		/// <summary>
		/// Default selection style
		/// </summary>
		Default,

		/// <summary>
		/// Windows classic focus rectangle style
		/// </summary>
		FocusRect,
	}

	/// <summary>
	/// Selection Forward Direction for Control. When user finished cell edit,
	/// the focus-selection moves into the next position to 'right' or 'down'
	/// of current range according this enum.
	/// </summary>
	public enum SelectionForwardDirection
	{
		/// <summary>
		/// Move into next position of right of current range
		/// </summary>
		Right,

		/// <summary>
		/// Move into next position of down of current range
		/// </summary>
		Down,
	}

	#endregion

	#region Position & Range
	/// <summary>
	/// Absolute position of cell in grid.
	/// </summary>
	[Serializable]
	public struct ReoGridPos
	{
		private int row;

		/// <summary>
		/// Row of cell
		/// </summary>
		public int Row
		{
			get { return row; }
			set { row = value; }
		}

		private int col;

		/// <summary>
		/// Column of cell
		/// </summary>
		public int Col
		{
			get { return col; }
			set { col = value; }
		}

		/// <summary>
		/// Create instance with number of row and number of column
		/// </summary>
		/// <param name="row"></param>
		/// <param name="col"></param>
		public ReoGridPos(int row, int col)
		{
			this.row = row;
			this.col = col;
		}

		/// <summary>
		/// Create instance with alphabet code of position (eg. new ReoGridPos("A10"))
		/// </summary>
		/// <param name="code"></param>
		/// <exception cref="ArgumentException">if id is not in correct format "A10"</exception>
		/// <example>var pos = new ReoGridPos("A10");</example>
		public ReoGridPos(string id)
		{
			Match m = RGUtility.CellReferenceRegex.Match(id);

			if (!m.Success)
			{
				throw new ArgumentException("position is invalid: " + id, "id");
			}

			this.row = 0;
			int.TryParse(m.Groups[2].Value, out row);
			row--;
			this.col = RGUtility.GetAlphaNumber(m.Groups[1].Value);
		}

		/// <summary>
		/// Offset by specified value from another position.
		/// </summary>
		/// <param name="pos">Position indicates how many rows and columns to be moved</param>
		/// <returns>Offsetted position</returns>
		public ReoGridPos Offset(ReoGridPos pos)
		{
			return Offset(pos.row, pos.col);
		}
		/// <summary>
		/// Offset by specified rows and columns.
		/// </summary>
		/// <param name="row">Rows to move</param>
		/// <param name="col">Columns to move</param>
		/// <returns></returns>
		public ReoGridPos Offset(int row, int col)
		{
			this.row += row;
			this.col += col;
			return this;
		}

		/// <summary>
		/// Convert position to string
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return string.Format("[{0}, {1}]", row, col);
		}

		/// <summary>
		/// Convert position to code (A1)
		/// </summary>
		/// <returns></returns>
		public string ToStringCode()
		{
			return RGUtility.GetAlphaChar(col) + (row + 1);
		}

		internal bool IsEmpty
		{
			get { return row == -1 && col == -1; }
		}

		public override bool Equals(object obj)
		{
			if (obj == null) return false;
			if (!(obj is ReoGridPos)) return false;
			ReoGridPos p1 = this;
			ReoGridPos p2 = (ReoGridPos)obj;
			return p1.row == p2.row && p1.col == p2.col;
		}

		public bool Equals(int row, int col)
		{
			return this.row == row && this.col == col;
		}

		/// <summary>
		/// Compare position to code (A1)
		/// </summary>
		/// <param name="code">code to be compared</param>
		/// <returns>true if position is same as code</returns>
		public bool Equals(string code)
		{
			return !string.IsNullOrEmpty(code) && Equals(new ReoGridPos(code));
		}

		public static bool operator ==(ReoGridPos r1, ReoGridPos r2)
		{
			return (r1 == null && r2 == null) || (r1 != null && r1.Equals(r2));
		}

		public static bool operator !=(ReoGridPos r1, ReoGridPos r2)
		{
			return !(r1 == r2);
		}

		public override int GetHashCode()
		{
			return row ^ col;
		}

		internal static readonly ReoGridPos Empty = new ReoGridPos(-1, -1);

		public static bool Equals(ReoGridPos pos1, ReoGridPos pos2)
		{
			return Equals(pos1, pos2.row, pos2.col);
		}
		public static bool Equals(ReoGridPos pos, int row, int col)
		{
			if (pos == null) return false;
			return pos.row == row && pos.col == col;
		}
	}

	/// <summary>
	/// Describe a range of cells on grid.
	/// </summary>
	public struct ReoGridRange
	{
		private int row;

		/// <summary>
		/// The start row of range
		/// </summary>
		public int Row
		{
			get { return row; }
			set { row = value; }
		}

		private int col;

		/// <summary>
		/// The start column of range
		/// </summary>
		public int Col
		{
			get { return col; }
			set { col = value; }
		}

		private int rows;

		/// <summary>
		/// Rows of range. (minimum is 1)
		/// </summary>
		public int Rows
		{
			get { return rows; }
			set { rows = value; }
		}

		private int cols;

		/// <summary>
		/// Columns of range. (minimum is 1)
		/// </summary>
		public int Cols
		{
			get { return cols; }
			set { cols = value; }
		}

		/// <summary>
		/// Then end row of range
		/// </summary>
		public int Row2
		{
			get { return row + rows - 1; }
			set { rows = value - row + 1; }
		}

		/// <summary>
		/// The end column of range
		/// </summary>
		public int Col2
		{
			get { return col + cols - 1; }
			set { cols = value - col + 1; }
		}

		/// <summary>
		/// The start location of range.
		/// </summary>
		public ReoGridPos StartPos
		{
			get { return new ReoGridPos(row, col); }
			set { row = value.Row; col = value.Col; }
		}

		/// <summary>
		/// The end location of range.
		/// </summary>
		public ReoGridPos EndPos
		{
			get { return new ReoGridPos(Row2, Col2); }
			set { Row2 = value.Row; Col2 = value.Col; }
		}

		/// <summary>
		/// Construct range with specified start-location and end-location. 
		/// If the end-location is less than start-location, the correct location
		/// will be fixed automatically.
		/// </summary>
		/// <param name="startPos">Start location of range</param>
		/// <param name="endPos">End location of range</param>
		public ReoGridRange(ReoGridPos startPos, ReoGridPos endPos)
		{
			this.row = Math.Min(startPos.Row, endPos.Row);
			this.col = Math.Min(startPos.Col, endPos.Col);
			this.rows = Math.Max(startPos.Row, endPos.Row) - this.row + 1;
			this.cols = Math.Max(startPos.Col, endPos.Col) - this.col + 1;
		}

		/// <summary>
		/// Construct range with specified start-location, rows and columns.
		/// </summary>
		/// <param name="row">Row of start location</param>
		/// <param name="col">Column of start location</param>
		/// <param name="rows">Rows of range. (minimum is 1)</param>
		/// <param name="cols">Columns of range. (minimum is 1)</param>
		public ReoGridRange(int row, int col, int rows, int cols)
		{
			this.row = row;
			this.col = col;
			this.rows = rows;
			this.cols = cols;
		}

		public ReoGridRange(string startCell, string endCell)
			: this(new ReoGridPos(startCell), new ReoGridPos(endCell))
		{
		}

		public ReoGridRange(string rangeDesc)
		{
			Match m = RGUtility.RangeReferenceRegex.Match(rangeDesc);
			if (!m.Success ||
				(m.Groups["to_col"].Length <= 0 || m.Groups["to_row"].Length <= 0
				|| m.Groups["from_col"].Length <= 0 || m.Groups["from_row"].Length <= 0))
			{
				throw new ArgumentException("range is invalid: " + rangeDesc);
			}

			int fromRow = -1;
			if (!int.TryParse(m.Groups["from_row"].Value, out fromRow))
			{
				throw new ArgumentException("range is invalid: " + rangeDesc);
			}
			fromRow--;

			int toRow = -1;
			if (!int.TryParse(m.Groups["to_row"].Value, out toRow))
			{
				throw new ArgumentException("range is invalid: " + rangeDesc);
			}
			toRow--;

			int fromCol = RGUtility.GetAlphaNumber(m.Groups["from_col"].Value);
			int toCol = RGUtility.GetAlphaNumber(m.Groups["to_col"].Value);

			this.row = Math.Min(fromRow, toCol);
			this.col = Math.Min(fromCol, toCol);
			this.rows = Math.Max(fromRow, toRow) - this.row + 1;
			this.cols = Math.Max(fromCol, toCol) - this.col + 1;
		}

		/// <summary>
		/// Offset range by specified rows and cols
		/// </summary>
		/// <param name="rows">rows to be offseted</param>
		/// <param name="cols">cols to be offseted</param>
		public void Offset(int rows, int cols)
		{
			this.row += rows;
			this.col += cols;
		}

		/// <summary>
		/// Check for whether specified object is same with this range
		/// </summary>
		/// <param name="obj">Target range to be checked</param>
		/// <returns></returns>
		public override bool Equals(object obj)
		{
			if (obj == null) return false;
			if (!(obj is ReoGridRange)) return false;
			ReoGridRange r1 = this;
			ReoGridRange r2 = (ReoGridRange)obj;
			return r1.row == r2.row && r1.col == r2.col && r1.rows == r2.rows && r1.cols == r2.cols;
		}

		public static bool operator ==(ReoGridRange r1, ReoGridRange r2)
		{
			return r1.row == r2.row && r1.col == r2.col && r1.rows == r2.rows && r1.cols == r2.cols;
		}

		public static bool operator !=(ReoGridRange r1, ReoGridRange r2)
		{
			return !(r1 == r2);
		}

		public override int GetHashCode()
		{
			return row ^ col ^ rows ^ cols;
		}
		
		/// <summary>
		/// Check whether the position is contained by this range
		/// </summary>
		/// <param name="pos">Position to be checked</param>
		/// <returns>true if the position is contained by this range</returns>
		public bool Contains(ReoGridPos pos)
		{
			return Contains(pos.Row, pos.Col);
		}

		/// <summary>
		/// Check whether the position specified by row and col is contained by this range
		/// </summary>
		/// <param name="row">row of position</param>
		/// <param name="col">col of position</param>
		/// <returns>true if position is contained by this range</returns>
		public bool Contains(int row, int col)
		{
			return row >= this.row && col >= this.col && row <= Row2 && col <= Col2;
		}

		/// <summary>
		/// Check whether the spcified row is contained by this range
		/// </summary>
		/// <param name="row"></param>
		/// <returns></returns>
		public bool ContainsRow(int row)
		{
			return row >= this.row && row <= this.Row2;
		}

		/// <summary>
		/// Check whether the specified column is contained by this range
		/// </summary>
		/// <param name="col"></param>
		/// <returns></returns>
		public bool ContainsColumn(int col)
		{
			return col >= this.col && col <= this.Col2;
		}

		/// <summary>
		/// Empty range constant define
		/// </summary>
		public static readonly ReoGridRange Empty = new ReoGridRange(0, 0, 0, 0);
		/// <summary>
		/// Entire range constant define
		/// </summary>
		public static readonly ReoGridRange EntireRange = new ReoGridRange(0, 0, -1, -1);

		/// <summary>
		/// Return whether current range is empty
		/// </summary>
		public bool IsEmpty
		{
			get
			{
				return row == 0 && col == 0 && rows == 0 && cols == 0;
			}
		}

		/// <summary>
		/// Convert range to string
		/// </summary>
		/// <returns>string of this range</returns>
		public override string ToString()
		{
			return string.Format("Range[{0},{1}-{2},{3}]", row, col, Row2, Col2);
		}

		/// <summary>
		/// Convert range to code (A1:D3)
		/// </summary>
		/// <returns>converted code</returns>
		public string ToStringCode()
		{
			return string.Format("{0}:{1}", StartPos.ToStringCode(), EndPos.ToStringCode());
		}
	}
	#endregion

	#region Header Defines
#if UNUSED
	internal struct RowHead
	{
		UInt16 top;
		UInt16 height;
	}
	internal struct ColHead
	{
		UInt16 left;
		UInt16 width;
	}
#endif

	internal class ReoGridColHead
	{
		private int left;

		public int Left
		{
			get { return left; }
			set { left = value; }
		}

		public ushort Width { get; set; }

		public ushort LastWidth { get; set; }

		public int Col { get; set; }

		public string Code { get; set; }

		public int Right
		{
			get { return left + Width; }
			set
			{
				int width = value - left;
				if (width < 0) width = 0;
				this.Width = (ushort)width;
			}
		}

		internal ReoGridRangeStyle Style { get; set; }

		public bool IsAutoWidth { get; set; }

		public bool IsHidden { get { return this.Width == 0; } }
	}

	internal class ReoGridRowHead
	{
		public int Top { get; set; }
	
		public ushort Height { get; set; }
	
		public ushort LastHeight { get; set; }

		public int Bottom
		{
			get { return Top + Height; }
			set
			{
				int height = value - Top;
				if (height < 0) height = 0;
				this.Height = (ushort)height;
			}
		}

		public int Row { get; set; }

		internal ReoGridRangeStyle Style { get; set; }

		public bool IsAutoHeight { get; set; }

		public bool IsHidden { get { return this.Height == 0; } }
	}

	public enum RowOrColumn : byte
	{
		Row = 1,
		Column = 2,

		// some approach will not compatible with 'Both'
		Both = 4,
	}

	/// <summary>
	/// Outline Collection for both Row and Column Outline
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class OutlineCollection<T> : List<OutlineGroup<T>> where T: IReoGridOutline
	{
		/// <summary>
		/// Only allowed to create instance by ReoGridControl
		/// </summary>
		internal OutlineCollection()
		{
			Reset();
		}

		/// <summary>
		/// Clear all outlines, reset to default status
		/// </summary>
		public void Reset()
		{
			Clear();
			Add(new OutlineGroup<T>());
		}

		/// <summary>
		/// Iterate through all of outlines
		/// </summary>
		/// <param name="iterator">iterator callback function</param>
		public void IterateOutlines(Func<T, bool> iterator)
		{
			for (int i = 0; i < this.Count - 1; i++)
			{
				var group = this[i];

				foreach (var outline in group)
				{
					if (!iterator(outline)) return;
				}
			}
		}

		/// <summary>
		/// Reverse iterate through all of outlines
		/// </summary>
		/// <param name="iterator"></param>
		public void IterateReverseOutlines(Func<T, bool> iterator)
		{
			for (int i = this.Count - 1; i >= 0; i--)
			{
				var group = this[i];

				foreach (var outline in group)
				{
					if (!iterator(outline)) return;
				}
			}
		}

		/// <summary>
		/// Check whether there is same outline exist
		/// </summary>
		/// <param name="target">Outline used to find</param>
		/// <returns>true if there is another same as target</returns>
		public bool HasSame(IReoGridOutline target, OutlineGroup<IReoGridOutline> excepts)
		{
			for (int i = 0; i < this.Count - 1; i++)
			{
				var group = this[i];

				foreach (IReoGridOutline outline in group)
				{
					if (outline != target
						&& (excepts == null || !excepts.Contains(outline))
						&& outline.Start == target.Start 
						&& outline.Count == target.Count)
					{
						return true;
					}
				}
			}

			return false;
		}

		/// <summary>
		/// Determine whether any outlines existed in this collection
		/// </summary>
		public bool HasOutlines
		{
			get
			{
				for (int i = this.Count - 1; i >= 0; i--)
				{
					var group = this[i];

					if (group.Count > 0) return true;
				}

				return false;
			}
		}

		/// <summary>
		/// Get number of outlines
		/// </summary>
		public int OutlineCount
		{
			get
			{
				int count = 0;

				for (int i = this.Count - 1; i >= 0; i--)
				{
					var group = this[i];
					count += group.Count;
				}

				return count;
			}
		}
	}

	/// <summary>
	/// Outline define
	/// </summary>
	public interface IReoGridOutline
	{
		int Start { get; set; }

		int Count { get; set; }

		int End { get; }

		void Collapse();
		void Expand();

		bool Collapsed { get; set; }
	}

	/// <summary>
	/// Outline group for both Row and Column Outline
	/// </summary>
	/// <typeparam name="T">Outline define type, must be IReoGridOutline</typeparam>
	public class OutlineGroup<T> : List<T> where T : IReoGridOutline 
	{
		/// <summary>
		/// Only allowed to create instance by ReoGridControl
		/// </summary>
		internal OutlineGroup() { }

		/// <summary>
		/// Number Button Rectangle
		/// </summary>
		internal Rectangle NumberButtonBounds { get; set; }

		/// <summary>
		/// Collapse all outlines inside this group
		/// </summary>
		public void CollapseAll()
		{
			foreach (var outline in this)
			{
				outline.Collapse();
			}
		}

		/// <summary>
		/// Expand all outlines inside this group
		/// </summary>
		public void ExpandAll()
		{
			foreach (var outline in this)
			{
				outline.Expand();
			}
		}
	}

	/// <summary>
	/// Outline instance for both Row and Column Outline
	/// </summary>
	public abstract class ReoGridOutline : IReoGridOutline
	{
		/// <summary>
		/// instance of ReoGridControl
		/// </summary>
		public ReoGridControl Grid { get; set; }

		internal ReoGridOutline(ReoGridControl grid, int start, int count)
		{
			this.Grid = grid;
			this.Start = start;
			this.Count = count;
		}

		/// <summary>
		/// Outline start index (either number of row or number of column)
		/// </summary>
		public int Start { get; set; }

		/// <summary>
		/// Outline number of count (either number of rows or number of columns)
		/// </summary>
		public int Count { get; set; }

		/// <summary>
		/// Outline end index (either number of row or number of column)
		/// </summary>
		public int End { get { return Start + Count; } }

		/// <summary>
		/// Determine whether current outline is collapsed (do not set this property, use 'Collapse' or 'Expand' instead)
		/// </summary>
		public bool Collapsed { get; set; }

		internal Rectangle ToggleButtonBounds { get; set; }

		/// <summary>
		/// Determine whether specified outline is contained entirely by this outline
		/// </summary>
		/// <param name="outline">outline to be tested</param>
		/// <returns></returns>
		public bool Contains(ReoGridOutline outline)
		{
			return this.Start <= outline.Start && this.End >= outline.End;
		}
		
		/// <summary>
		/// Determine whether specified index is contained by this outline
		/// </summary>
		/// <param name="index">index to be tested</param>
		/// <returns></returns>
		public bool Contains(int index)
		{
			return index >= Start && index < End;
		}

		/// <summary>
		/// Determine whether specified outline is intersected with this outline
		/// </summary>
		/// <param name="outline">outline to be tested</param>
		/// <returns></returns>
		public bool IntersectWith(ReoGridOutline outline)
		{
			return IntersectWith(outline.Start, outline.Count);
		}

		/// <summary>
		/// Determine whether specified range is intersected with this outline
		/// </summary>
		/// <param name="start">start index (either number of row or number of column)</param>
		/// <param name="count">number of count (either number of rows or number of columns)</param>
		/// <returns></returns>
		public bool IntersectWith(int start, int count)
		{
			int targetEnd = start + count;
			
			return (this.Start < start && this.End >= start && this.End < targetEnd)
				|| (this.Start > start && this.Start <= targetEnd && this.End >= targetEnd);
		}

		/// <summary>
		/// Collapse outline
		/// </summary>
		public abstract void Collapse();

		/// <summary>
		/// Expand outline
		/// </summary>
		public abstract void Expand();
	}

	/// <summary>
	/// Rows outline
	/// </summary>
	public class RowOutline : ReoGridOutline
	{
		internal RowOutline(ReoGridControl grid, int start, int count) 
			: base(grid, start, count) { }

		/// <summary>
		/// Collapse outline
		/// </summary>
		public override void Collapse()
		{
			var outlines = Grid.GetOutlines(RowOrColumn.Row);
			if (outlines == null) return;

			// find self group index
			int groupIndex = 0;
			for (; groupIndex < outlines.Count - 1; groupIndex++)
			{
				var o = outlines[groupIndex];
				if (o.Contains(this))
				{
					break;
				}
			}

			// find whether the rows contained by any inner outlines
			Grid.SetRowsHeight(this.Start, this.Count, r =>
			{
				for (int i = groupIndex + 1; i < outlines.Count - 1; i++)
				{
					var g = outlines[i];

					// find collapsed inner outlines
					var outline = g.FirstOrDefault(o => o.Collapsed && o != this && o.Start <= r && o.End > r);

					// if there is any inner outlines attached on this row, skip adjust this row's height
					if (outline != null)
					{
						return -1;
					}
				}

				return 0;
			}, false);

			Collapsed = true;
		}

		/// <summary>
		/// Expand outline
		/// </summary>
		public override void Expand()
		{
			var outlines = Grid.GetOutlines(RowOrColumn.Row);
			if (outlines == null) return;

			// find self group index
			int groupIndex = 0;
			for (; groupIndex < outlines.Count - 1; groupIndex++)
			{
				var o = outlines[groupIndex];
				if (o.Contains(this))
				{
					break;
				}
			}

			// find whether the rows contained by any inner outlines
			Grid.SetRowsHeight(this.Start, this.Count, r =>
			{
				for (int i = groupIndex + 1; i < outlines.Count - 1; i++)
				{
					var g = outlines[i];
					var outline = g.FirstOrDefault(o => o.Start <= r && o.End > r);

					if (outline != null && outline.Collapsed)
					{
						return 0;
					}
				}

				var rowhead = Grid.RetrieveRowHead(r);
				return rowhead.IsHidden ? rowhead.LastHeight : rowhead.Height;
			}, false);
	
			Collapsed = false;
		}
	}

	/// <summary>
	/// Column outline
	/// </summary>
	public class ColumnOutline : ReoGridOutline
	{
		internal ColumnOutline(ReoGridControl grid, int start, int count)
			: base(grid, start, count) { }

		/// <summary>
		/// Collapse outline
		/// </summary>
		public override void Collapse()
		{
			var outlines = Grid.GetOutlines(RowOrColumn.Column);
			if (outlines == null) return;

			// find self group index
			int groupIndex = 0;
			for (; groupIndex < outlines.Count - 1; groupIndex++)
			{
				var o = outlines[groupIndex];
				if (o.Contains(this))
				{
					break;
				}
			}

			// find whether the rows contained by any inner outlines
			Grid.SetColsWidth(this.Start, this.Count, c =>
			{
				for (int i = groupIndex + 1; i < outlines.Count - 1; i++)
				{
					var g = outlines[i];

					// find collapsed inner outlines
					var outline = g.FirstOrDefault(o => o.Collapsed && o != this && o.Start <= c && o.End > c);

					// if there is any inner outlines attached on this row, skip adjust this row's height
					if (outline != null)
					{
						return -1;
					}
				}

				return 0;
			}, false);

			Collapsed = true;
		}

		/// <summary>
		/// Expand outline
		/// </summary>
		public override void Expand()
		{
			var outlines = Grid.GetOutlines(RowOrColumn.Column);
			if (outlines == null) return;

			// find self group index
			int groupIndex = 0;
			for (; groupIndex < outlines.Count - 1; groupIndex++)
			{
				var o = outlines[groupIndex];
				if (o.Contains(this))
				{
					break;
				}
			}

			// find whether the rows contained by any inner outlines
			Grid.SetColsWidth(this.Start, this.Count, r =>
			{
				for (int i = groupIndex + 1; i < outlines.Count - 1; i++)
				{
					var g = outlines[i];
					var outline = g.FirstOrDefault(o => o.Start <= r && o.End > r);

					if (outline != null && outline.Collapsed)
					{
						return 0;
					}
				}

				var colhead = Grid.RetrieveColHead(r);
				return colhead.IsHidden ? colhead.LastWidth : colhead.Width;
			}, false);

			Collapsed = false;
		}
	}

	#endregion

	#region Styles

	#region Alignment
	/// <summary>
	/// Cell horizontal alignment (default: General)
	/// </summary>
	[Serializable]
	public enum ReoGridHorAlign
	{
		General,
		Left, Center, Right,
		DistributedIndent,
	}

	/// <summary>
	/// Cell vertical alignment (default: Middle)
	/// </summary>
	[Serializable]
	public enum ReoGridVerAlign
	{
		Top, Middle, Bottom,
	}

	/// <summary>
	/// Cell horizontal alignment for render (cell-auto-format)
	/// </summary>
	[Serializable]
	internal enum ReoGridRenderHorAlign
	{
		Left, Center, Right,
	}
	#endregion // Alignment

	#region Control Appearance
	/// <summary>
	/// Key of control appearance item
	/// </summary>
	public enum ReoGridControlColors : short
	{
		LeadHeadNormal = 1,
		LeadHeadHover = 2,
		LeadHeadSelected = 3,

		LeadHeadIndicatorStart = 11,
		LeadHeadIndicatorEnd = 12,

		ColHeadSplitter = 20,
		ColHeadNormalStart = 21,
		ColHeadNormalEnd = 22,
		ColHeadHoverStart = 23,
		ColHeadHoverEnd = 24,
		ColHeadSelectedStart = 25,
		ColHeadSelectedEnd = 26,
		ColHeadFullSelectedStart = 27,
		ColHeadFullSelectedEnd = 28,
		ColHeadInvalidStart = 29,
		ColHeadInvalidEnd = 30,
		ColHeadText = 36,

		RowHeadSplitter = 40,
		RowHeadNormal = 41,
		RowHeadHover = 42,
		RowHeadSelected = 43,
		RowHeadFullSelected = 44,
		RowHeadInvalid = 45,
		RowHeadText = 51,

		SelectionBorder = 61,
		SelectionFill = 62,

		GridBackground = 81,
		GridText = 82,
		GridLine = 83,

		OutlinePanelBorder = 91,
		OutlinePanelBackground = 92,
		OutlineButtonBorder = 93,
		OutlineButtonText = 94,
	}

	/// <summary>
	/// ReoGrid Control Appearance Colors
	/// </summary>
	public class ReoGridControlStyle
	{
		private Dictionary<ReoGridControlColors, Color> colors = new Dictionary<ReoGridControlColors, Color>(100);

		internal Dictionary<ReoGridControlColors, Color> Colors
		{
			get { return colors; }
			set { colors = value; }
		}

		/// <summary>
		/// Get color for appearance item
		/// </summary>
		/// <param name="colorKey"></param>
		/// <returns></returns>
		public Color GetColor(ReoGridControlColors colorKey)
		{
			Color color = Color.Empty;
			colors.TryGetValue(colorKey, out color);
			return color;
		}

		/// <summary>
		/// Set color for appearance item
		/// </summary>
		/// <param name="colorKey">Key of appearance item</param>
		/// <param name="color">Color to be set</param>
		public void SetColor(ReoGridControlColors colorKey, Color color)
		{
			colors[colorKey] = color;
		}

		/// <summary>
		/// Get or set color for appearance items
		/// </summary>
		/// <param name="colorKey"></param>
		/// <returns></returns>
		public Color this[ReoGridControlColors colorKey]
		{
			get { return GetColor(colorKey); }
			set { SetColor(colorKey, value); }
		}

		private int selectionBorderWidth = 3;
		/// <summary>
		/// Get or set selection border weight
		/// </summary>
		public int SelectionBorderWidth { get { return selectionBorderWidth; } set { selectionBorderWidth = value; } }

		/// <summary>
		/// Construct empty control appearance
		/// </summary>
		public ReoGridControlStyle() { }

		/// <summary>
		/// Construct control appearance with two theme colors
		/// </summary>
		/// <param name="mainTheme">Main theme color</param>
		/// <param name="salientTheme">Salient theme color</param>
		/// <param name="useSystemHighlight">Whether use highlight colors of system default</param>
		public ReoGridControlStyle(Color mainTheme, Color salientTheme, bool useSystemHighlight)
		{
			Color lightMainTheme = ControlPaint.Light(mainTheme);
			Color lightLightMainTheme = ControlPaint.LightLight(mainTheme);
			Color lightLightLightMainTheme = ControlPaint.Light(mainTheme, 1.5f);
			Color darkMainTheme = ControlPaint.Dark(mainTheme);
			Color darkDarkMainTheme = ControlPaint.DarkDark(mainTheme);
			Color lightSalientTheme = ControlPaint.Light(salientTheme);
			Color lightLightSalientTheme = ControlPaint.LightLight(salientTheme);
			Color lightLightLightSalientTheme = ControlPaint.Light(salientTheme, 1.7f);
			Color darkSalientTheme = ControlPaint.Dark(salientTheme);
			Color darkDarkSalientTheme = ControlPaint.DarkDark(salientTheme);

			Color leadHead = mainTheme;
			colors[ReoGridControlColors.LeadHeadNormal] = leadHead;
			colors[ReoGridControlColors.LeadHeadSelected] = darkMainTheme;
			colors[ReoGridControlColors.LeadHeadIndicatorStart] = lightLightLightSalientTheme;
			colors[ReoGridControlColors.LeadHeadIndicatorEnd] = lightLightSalientTheme;

			colors[ReoGridControlColors.ColHeadSplitter] = mainTheme;
			colors[ReoGridControlColors.ColHeadNormalStart] = lightLightLightMainTheme;
			colors[ReoGridControlColors.ColHeadNormalEnd] = mainTheme;
			colors[ReoGridControlColors.ColHeadSelectedStart] = lightLightLightSalientTheme;
			colors[ReoGridControlColors.ColHeadSelectedEnd] = salientTheme;
			colors[ReoGridControlColors.ColHeadFullSelectedStart] = lightLightLightSalientTheme;
			colors[ReoGridControlColors.ColHeadFullSelectedEnd] = lightLightSalientTheme;
			colors[ReoGridControlColors.ColHeadText] = darkDarkMainTheme;

			colors[ReoGridControlColors.RowHeadSplitter] = mainTheme;
			colors[ReoGridControlColors.RowHeadNormal] = lightLightMainTheme;
			colors[ReoGridControlColors.RowHeadHover] = ControlPaint.Dark(leadHead);
			colors[ReoGridControlColors.RowHeadSelected] = lightSalientTheme;
			colors[ReoGridControlColors.RowHeadFullSelected] = lightLightSalientTheme;
			colors[ReoGridControlColors.RowHeadText] = darkDarkMainTheme;

			if (useSystemHighlight)
			{
				colors[ReoGridControlColors.SelectionFill] = Color.FromArgb(30, SystemColors.Highlight);
				colors[ReoGridControlColors.SelectionBorder] = Color.FromArgb(180, SystemColors.Highlight);
			}
			else
			{
				colors[ReoGridControlColors.SelectionFill] = Color.FromArgb(30, darkSalientTheme);
				colors[ReoGridControlColors.SelectionBorder] = Color.FromArgb(180, lightSalientTheme);
			}

			colors[ReoGridControlColors.GridBackground] = lightLightMainTheme;
			colors[ReoGridControlColors.GridLine] = mainTheme;

			colors[ReoGridControlColors.OutlineButtonBorder] = mainTheme;
			colors[ReoGridControlColors.OutlinePanelBackground] = lightLightMainTheme;
			colors[ReoGridControlColors.OutlinePanelBorder] = mainTheme;
			colors[ReoGridControlColors.OutlineButtonText] = darkSalientTheme;
		}

		public Color GetColHeadStartColor(bool isHover, bool isSelected, bool isFullSelected, bool isInvalid)
		{
			if (isFullSelected)
				return colors[ReoGridControlColors.ColHeadFullSelectedStart];
			else if (isSelected)
				return colors[ReoGridControlColors.ColHeadSelectedStart];
			else if (isHover)
				return colors[ReoGridControlColors.ColHeadHoverStart];
			else if (isInvalid)
				return colors[ReoGridControlColors.ColHeadInvalidStart];
			else
				return colors[ReoGridControlColors.ColHeadNormalStart];
		}
		public Color GetColHeadEndColor(bool isHover, bool isSelected, bool isFullSelected, bool isInvalid)
		{
			if (isFullSelected)
				return colors[ReoGridControlColors.ColHeadFullSelectedEnd];
			else if (isSelected)
				return colors[ReoGridControlColors.ColHeadSelectedEnd];
			else if (isHover)
				return colors[ReoGridControlColors.ColHeadHoverEnd];
			else if (isInvalid)
				return colors[ReoGridControlColors.ColHeadInvalidEnd];
			else
				return colors[ReoGridControlColors.ColHeadNormalEnd];
		}
		public Color GetRowHeadEndColor(bool isHover, bool isSelected, bool isFullSelected, bool isInvalid)
		{
			if (isFullSelected)
				return colors[ReoGridControlColors.RowHeadFullSelected];
			else if (isSelected)
				return colors[ReoGridControlColors.RowHeadSelected];
			else if (isHover)
				return colors[ReoGridControlColors.RowHeadHover];
			else if (isInvalid)
				return colors[ReoGridControlColors.RowHeadInvalid];
			else
				return colors[ReoGridControlColors.RowHeadNormal];
		}
	}
	#endregion

	#region Cell Style
	/// <summary>
	/// Key of cell style item
	/// </summary>
	public enum PlainStyleFlag : long
	{
		None = 0,
		FontName = 0x1,
		FontSize = 0x2,
		FontStyleBold = 0x4,
		FontStyleItalic = 0x8,
		FontStyleStrikethrough = 0x10,
		FontStyleUnderline = 0x20,
		TextColor = 0x40,
		FillColor = 0x80,
		LineColor = 0x100,
		LineStyle = 0x200,
		LineWeight = 0x400,
		LineStartCap = 0x800,
		LineEndCap = 0x1000,
		AlignLeft = 0x2000,
		AlignRight = 0x4000,
		AlignCenter = 0x8000,
		AlignTop = 0x10000,
		AlignMiddle = 0x20000,
		AlignBottom = 0x40000,

		FillPatternColor = 0x80000,
		FillPatternStyle = 0x100000,

		TextWrap = 0x200000,
		Padding = 0x400000,

		FontStyleAll = FontStyleBold | FontStyleItalic
			| FontStyleStrikethrough | FontStyleUnderline,

		FontAll = FontName | FontSize | FontStyleAll,

		LineAll = LineColor | LineStyle | LineWeight | LineStartCap | LineEndCap,

		HorizontalAlign = AlignLeft | AlignRight | AlignCenter,
		VerticalAlign = AlignTop | AlignMiddle | AlignBottom,
		AlignAll = HorizontalAlign | VerticalAlign,

		FillPattern = FillPatternColor | FillPatternStyle,
		FillAll = FillColor | FillPattern,

		All = FontAll | TextColor | FillAll | LineAll | AlignAll,
	}

	/// <summary>
	/// Text-wrap mode of cell
	/// </summary>
	public enum TextWrapMode
	{
		/// <summary>
		/// No break (default)
		/// </summary>
		NoWrap,

		/// <summary>
		/// Normal word break
		/// </summary>
		WordBreak,

		/// <summary>
		/// Break enabled for all characters
		/// </summary>
		BreakAll,
	}

	/// <summary>
	/// Styles of range or cells. By specifying PlainStyleFlag to determine 
	/// what styles should be used in this set.
	/// </summary>
	[Serializable]
	public class ReoGridRangeStyle
	{
		public PlainStyleFlag Flag { get; set; }
		public Color BackColor { get; set; }
		public Color FillPatternColor { get; set; }
		public HatchStyle FillPatternStyle { get; set; }
		public Color TextColor { get; set; }
		public string FontName { get; set; }
		public float FontSize { get; set; }
		public bool Bold { get; set; }
		public bool Italic { get; set; }
		public bool Strikethrough { get; set; }
		public bool Underline { get; set; }
		public ReoGridHorAlign HAlign { get; set; }
		public ReoGridVerAlign VAlign { get; set; }
		public TextWrapMode TextWrapMode { get; set; }
		public Padding Padding { get; set; }

		/// <summary>
		/// Construct an empty style set
		/// </summary>
		public ReoGridRangeStyle() { }

		/// <summary>
		/// Construct style set by copying from another one
		/// </summary>
		/// <param name="source"></param>
		public ReoGridRangeStyle(ReoGridRangeStyle source) { CopyFrom(source); }

		/// <summary>
		/// Create a duplication style from specified style
		/// </summary>
		/// <param name="source">the style to be copied</param>
		/// <returns>new duplicated style</returns>
		public static ReoGridRangeStyle Clone(ReoGridRangeStyle source)
		{
			return source == null ? source : new ReoGridRangeStyle(source);
		}

		/// <summary>
		/// Copy styles from another one
		/// </summary>
		/// <param name="s">style to be copied</param>
		public void CopyFrom(ReoGridRangeStyle s)
		{
			ReoGridStyleUtility.CopyStyle(s, this);
		}

		/// <summary>
		/// Predefined empty style set
		/// </summary>
		public static ReoGridRangeStyle Empty = new ReoGridRangeStyle();

		static internal bool Equals(ReoGridRangeStyle s1, ReoGridRangeStyle s2)
		{
			if (s1.Flag != s2.Flag) return false;

			if (ReoGridStyleUtility.HasStyle(s1, PlainStyleFlag.HorizontalAlign)
				&& s1.HAlign != s2.HAlign) return false;
			if (ReoGridStyleUtility.HasStyle(s1, PlainStyleFlag.VerticalAlign)
				&& s1.VAlign != s2.VAlign) return false;
			if (ReoGridStyleUtility.HasStyle(s1, PlainStyleFlag.FillColor)
				&& s1.BackColor != s2.BackColor) return false;
			if (ReoGridStyleUtility.HasStyle(s1, PlainStyleFlag.FillPatternColor)
				&& s1.FillPatternColor != s2.FillPatternColor) return false;
			if (ReoGridStyleUtility.HasStyle(s1, PlainStyleFlag.FillPatternStyle)
				&& s1.FillPatternStyle != s2.FillPatternStyle) return false;
			if (ReoGridStyleUtility.HasStyle(s1, PlainStyleFlag.TextColor)
				&& s1.TextColor != s2.TextColor) return false;
			if (ReoGridStyleUtility.HasStyle(s1, PlainStyleFlag.FontName)
				&& s1.FontName != s2.FontName) return false;
			if (ReoGridStyleUtility.HasStyle(s1, PlainStyleFlag.FontSize)
				&& s1.FontSize != s2.FontSize) return false;
			if (ReoGridStyleUtility.HasStyle(s1, PlainStyleFlag.FontStyleBold)
				&& s1.Bold != s2.Bold) return false;
			if (ReoGridStyleUtility.HasStyle(s1, PlainStyleFlag.FontStyleItalic)
				&& s1.Italic != s2.Italic) return false;
			if (ReoGridStyleUtility.HasStyle(s1, PlainStyleFlag.FontStyleStrikethrough)
				&& s1.Strikethrough != s2.Strikethrough) return false;
			if (ReoGridStyleUtility.HasStyle(s1, PlainStyleFlag.FontStyleUnderline)
				&& s1.Underline != s2.Underline) return false;
			if (ReoGridStyleUtility.HasStyle(s1, PlainStyleFlag.TextWrap)
				&& s1.TextWrapMode != s2.TextWrapMode) return false;
			if (ReoGridStyleUtility.HasStyle(s1, PlainStyleFlag.Padding)
				&& s1.Padding != s2.Padding) return false;

			return true;
		}

		internal static bool Equals(object obj1, object obj2, PlainStyleFlag flag)
		{
			ReoGridRangeStyle s1 = obj1 as ReoGridRangeStyle;
			ReoGridRangeStyle s2 = obj2 as ReoGridRangeStyle;

			if (obj1 == null && obj2 == null) return true;

			if (obj1 == null) return false;
			if (obj2 == null) return false;

			return obj1.Equals(obj2);
		}

		/// <summary>
		/// Check whether this set of style contains specified style item
		/// </summary>
		/// <param name="flag">style item to be checked</param>
		/// <returns>ture if this set contains specified style item</returns>
		public bool HasStyle(PlainStyleFlag flag)
		{
			return (this.Flag & flag) == flag;
		}

		/// <summary>
		/// Check whether this set of style contains any of one of specified style items
		/// </summary>
		/// <param name="flag">style items to be checked</param>
		/// <returns>true if this set contains any one of specified items</returns>
		public bool HasAny(PlainStyleFlag flag)
		{
			return (this.Flag & flag) > 0;
		}
	}

	#region ReoGridStyleUtility
	public sealed class ReoGridStyleUtility
	{
		public static bool HasStyle(ReoGridRangeStyle style, PlainStyleFlag flag)
		{
			return (style.Flag & flag) == flag;
		}

		internal static ReoGridRangeStyle CreateDefaultGridStyle(ReoGridControl grid, int row, int col)
		{
			return new ReoGridRangeStyle(GetDefaultStyleIgnoreCell(grid, row, col));
		}

		internal static ReoGridRangeStyle DistinctStyle(ReoGridRangeStyle style, ReoGridRangeStyle referStyle)
		{
			if (style == null || style.Flag == PlainStyleFlag.None) return style;

			if (style.Flag.Has(PlainStyleFlag.FillColor)
				&& style.BackColor == referStyle.BackColor)
			{
				style.Flag &= ~PlainStyleFlag.FillColor;
			}

			if (ReoGridStyleUtility.HasStyle(style, PlainStyleFlag.FillPatternColor)
				&& style.FillPatternColor == referStyle.FillPatternColor)
			{
				style.Flag &= ~PlainStyleFlag.FillPatternColor;
			}

			if (ReoGridStyleUtility.HasStyle(style, PlainStyleFlag.FillColor)
				&& style.BackColor == referStyle.BackColor)
			{
				style.Flag &= ~PlainStyleFlag.FillColor;
			}

			if (ReoGridStyleUtility.HasStyle(style, PlainStyleFlag.TextColor)
				&& style.TextColor == referStyle.TextColor)
			{
				style.Flag &= ~PlainStyleFlag.TextColor;
			}

			if (ReoGridStyleUtility.HasStyle(style, PlainStyleFlag.FontName)
				&& style.FontName == referStyle.FontName)
			{
				style.Flag &= ~PlainStyleFlag.FontName;
			}
			if (ReoGridStyleUtility.HasStyle(style, PlainStyleFlag.FontSize)
				&& style.FontSize == referStyle.FontSize)
			{
				style.Flag &= ~PlainStyleFlag.FontSize;
			}

			if (ReoGridStyleUtility.HasStyle(style, PlainStyleFlag.FontStyleBold)
				&& style.Bold == referStyle.Bold)
			{
				style.Flag &= ~PlainStyleFlag.FontStyleBold;
			}
			if (ReoGridStyleUtility.HasStyle(style, PlainStyleFlag.FontStyleItalic)
				&& style.Italic == referStyle.Italic)
			{
				style.Flag &= ~PlainStyleFlag.FontStyleItalic;
			}
			if (ReoGridStyleUtility.HasStyle(style, PlainStyleFlag.FontStyleStrikethrough)
				&& style.Strikethrough == referStyle.Strikethrough)
			{
				style.Flag &= ~PlainStyleFlag.FontStyleStrikethrough;
			}
			if (ReoGridStyleUtility.HasStyle(style, PlainStyleFlag.FontStyleUnderline)
				&& style.Underline == referStyle.Underline)
			{
				style.Flag &= ~PlainStyleFlag.FontStyleUnderline;
			}

			if (ReoGridStyleUtility.HasStyle(style, PlainStyleFlag.HorizontalAlign)
				&& style.HAlign == referStyle.HAlign)
			{
				style.Flag &= ~PlainStyleFlag.HorizontalAlign;
			}

			if (ReoGridStyleUtility.HasStyle(style, PlainStyleFlag.VerticalAlign)
				&& style.VAlign == referStyle.VAlign)
			{
				style.Flag &= ~PlainStyleFlag.VerticalAlign;
			}

			if (ReoGridStyleUtility.HasStyle(style, PlainStyleFlag.TextWrap)
				&& style.TextWrapMode == referStyle.TextWrapMode)
			{
				style.Flag &= ~PlainStyleFlag.TextWrap;
			}

			if (ReoGridStyleUtility.HasStyle(style, PlainStyleFlag.Padding)
				&& style.Padding == referStyle.Padding)
			{
				style.Flag &= ~PlainStyleFlag.Padding;
			}
			
			return style.Flag == PlainStyleFlag.None ? null : style;
		}

		/// <summary>
		/// Remove repeated styles if it does same as default style
		/// This function also can be used to create a default style for specified cell
		/// </summary>
		/// <param name="grid">StyleGrid instance</param>
		/// <param name="cell">the cell will be check and removed</param>
		/// <returns>checked style, null if given cell or style of cell is null</returns>
		internal static ReoGridRangeStyle CheckAndRemoveCellStyle(ReoGridControl grid, ReoGridCell cell)
		{
			if (cell.Style == null || cell.Style == null) return null;

			int row = cell.Row;
			int col = cell.Col;

			ReoGridRangeStyle style = cell.Style;
			ReoGridRangeStyle defaultStyle = GetDefaultStyleIgnoreCell(grid, row, col);

			return DistinctStyle(style, defaultStyle);
		}

		internal static void CopyStyle(ReoGridRangeStyle sourceStyle, ReoGridRangeStyle targetStyle)
		{
			CopyStyle(sourceStyle, targetStyle, sourceStyle.Flag);
		}

		internal static void CopyStyle(ReoGridRangeStyle sourceStyle, ReoGridRangeStyle targetStyle, PlainStyleFlag flag)
		{
			if (flag.Has(PlainStyleFlag.FillColor))
				targetStyle.BackColor = sourceStyle.BackColor;

			if (flag.Has(PlainStyleFlag.FillPatternColor))
				targetStyle.FillPatternColor = sourceStyle.FillPatternColor;

			if (flag.Has(PlainStyleFlag.FillPatternStyle))
				targetStyle.FillPatternStyle = sourceStyle.FillPatternStyle;

			if (flag.Has(PlainStyleFlag.TextColor))
				targetStyle.TextColor = sourceStyle.TextColor;

			if (flag.Has(PlainStyleFlag.FontName))
				targetStyle.FontName = sourceStyle.FontName;

			if (flag.Has(PlainStyleFlag.FontSize))
				targetStyle.FontSize = sourceStyle.FontSize;

			if (flag.Has(PlainStyleFlag.FontStyleBold))
				targetStyle.Bold = sourceStyle.Bold;

			if (flag.Has(PlainStyleFlag.FontStyleItalic))
				targetStyle.Italic = sourceStyle.Italic;

			if (flag.Has(PlainStyleFlag.FontStyleStrikethrough))
				targetStyle.Strikethrough = sourceStyle.Strikethrough;

			if (flag.Has(PlainStyleFlag.FontStyleUnderline))
				targetStyle.Underline = sourceStyle.Underline;

			if (flag.Has(PlainStyleFlag.HorizontalAlign))
				targetStyle.HAlign = sourceStyle.HAlign;

			if (flag.Has(PlainStyleFlag.VerticalAlign))
				targetStyle.VAlign = sourceStyle.VAlign;

			if (flag.Has(PlainStyleFlag.TextWrap))
				targetStyle.TextWrapMode = sourceStyle.TextWrapMode;

			if (flag.Has(PlainStyleFlag.Padding))
				targetStyle.Padding = sourceStyle.Padding;

			targetStyle.Flag |= flag;
		}

		internal static ReoGridRangeStyle GetDefaultStyleIgnoreCell(ReoGridControl grid, int row, int col)
		{
			ReoGridRowHead rowhead = grid.RetrieveRowHead(row);

			if (rowhead.Style != null)
			{
				return rowhead.Style;
			}
			else
			{
				ReoGridColHead colhead = grid.RetrieveColHead(col);
				if (colhead.Style != null)
				{
					return colhead.Style;
				}
				else
				{
					return grid.RootStyle;
				}
			}
		}

		internal static void RemoveStyle(ReoGridRangeStyle style, PlainStyleFlag flag)
		{
			// remove a style just to unset its flag
			// the value will be remaind
			style.Flag &= ~flag;
		}

		internal static RGXmlCellStyle ConvertToXmlStyle(ReoGridRangeStyle style)
		{
			if (style == null || style.Flag == PlainStyleFlag.None) return null;

			RGXmlCellStyle xmlStyle = new RGXmlCellStyle();

			if (ReoGridStyleUtility.HasStyle(style, PlainStyleFlag.FillColor))
			{
				xmlStyle.backColor = TextFormatHelper.EncodeColor(style.BackColor);
			}

			if (HasStyle(style, PlainStyleFlag.FillPattern))
			{
				RGXmlCellStyleFillPattern xmlFillPattern = new RGXmlCellStyleFillPattern()
				{
					color = TextFormatHelper.EncodeColor(style.FillPatternColor),
					patternStyleId = (int)style.FillPatternStyle,
				};
				xmlStyle.fillPattern = xmlFillPattern;
			}

			if (ReoGridStyleUtility.HasStyle(style, PlainStyleFlag.TextColor))
				xmlStyle.textColor = TextFormatHelper.EncodeColor(style.TextColor);
			if (ReoGridStyleUtility.HasStyle(style, PlainStyleFlag.FontName))
				xmlStyle.font = style.FontName;
			if (ReoGridStyleUtility.HasStyle(style, PlainStyleFlag.FontSize))
				xmlStyle.fontSize = style.FontSize.ToString();
			if (ReoGridStyleUtility.HasStyle(style, PlainStyleFlag.FontStyleBold))
				xmlStyle.bold = style.Bold.ToString().ToLower();
			if (ReoGridStyleUtility.HasStyle(style, PlainStyleFlag.FontStyleItalic))
				xmlStyle.italic = style.Italic.ToString().ToLower();
			if (ReoGridStyleUtility.HasStyle(style, PlainStyleFlag.FontStyleStrikethrough))
				xmlStyle.strikethrough = style.Strikethrough.ToString().ToLower();
			if (ReoGridStyleUtility.HasStyle(style, PlainStyleFlag.FontStyleUnderline))
				xmlStyle.underline = style.Underline.ToString().ToLower();
			if (ReoGridStyleUtility.HasStyle(style, PlainStyleFlag.HorizontalAlign))
				xmlStyle.hAlign = XmlFileFormatHelper.EncodeHorizontalAlign(style.HAlign);
			if (ReoGridStyleUtility.HasStyle(style, PlainStyleFlag.VerticalAlign))
				xmlStyle.vAlign = XmlFileFormatHelper.EncodeVerticalAlign(style.VAlign);
			if (ReoGridStyleUtility.HasStyle(style, PlainStyleFlag.TextWrap))
				xmlStyle.textWrap = XmlFileFormatHelper.EncodeTextWrapMode(style.TextWrapMode);
			if (ReoGridStyleUtility.HasStyle(style, PlainStyleFlag.Padding))
				xmlStyle.padding = TextFormatHelper.EncodePadding(style.Padding);

			return xmlStyle;
		}

		internal static ReoGridRangeStyle ConvertFromXmlStyle(ReoGridControl grid, RGXmlCellStyle xmlStyle, 
			CultureInfo culture)
		{
			ReoGridRangeStyle style = new ReoGridRangeStyle();

			if (xmlStyle == null) return style;

			// back color
			if (!string.IsNullOrEmpty(xmlStyle.backColor))
			{
				style.Flag |= PlainStyleFlag.FillColor;
				style.BackColor = TextFormatHelper.DecodeColor(xmlStyle.backColor);
			}

			// fill pattern
			if (xmlStyle.fillPattern != null)
			{
				style.Flag |= PlainStyleFlag.FillPattern;
				style.FillPatternColor = TextFormatHelper.DecodeColor(xmlStyle.fillPattern.color);
				style.FillPatternStyle = (HatchStyle)xmlStyle.fillPattern.patternStyleId;
			}

			// text color
			if (!string.IsNullOrEmpty(xmlStyle.textColor))
			{
				style.Flag |= PlainStyleFlag.TextColor;
				style.TextColor = TextFormatHelper.DecodeColor(xmlStyle.textColor);
			}

			// horizontal align
			if (!string.IsNullOrEmpty(xmlStyle.hAlign))
			{
				style.Flag |= PlainStyleFlag.HorizontalAlign;
				style.HAlign = XmlFileFormatHelper.DecodeHorizontalAlign(xmlStyle.hAlign);
			}
			// vertical align
			if (!string.IsNullOrEmpty(xmlStyle.vAlign))
			{
				style.Flag |= PlainStyleFlag.VerticalAlign;
				style.VAlign = XmlFileFormatHelper.DecodeVerticalAlign(xmlStyle.vAlign);
			}

			// font name
			if (!string.IsNullOrEmpty(xmlStyle.font))
			{
				style.Flag |= PlainStyleFlag.FontName;
				style.FontName = xmlStyle.font;
			}
			// font size
			if (xmlStyle.fontSize != null)
			{
				style.Flag |= PlainStyleFlag.FontSize;
				style.FontSize = TextFormatHelper.GetFloatValue(xmlStyle.fontSize, grid.RootStyle.FontSize, culture);
			}

			// bold
			if (xmlStyle.bold != null)
			{
				style.Flag |= PlainStyleFlag.FontStyleBold;
				style.Bold = xmlStyle.bold == "true";
			}
			// italic
			if (xmlStyle.italic != null)
			{
				style.Flag |= PlainStyleFlag.FontStyleItalic;
				style.Italic = xmlStyle.italic == "true";
			}
			// strikethrough
			if (xmlStyle.strikethrough != null)
			{
				style.Flag |= PlainStyleFlag.FontStyleStrikethrough;
				style.Strikethrough = xmlStyle.strikethrough == "true";
			}
			// underline
			if (xmlStyle.underline != null)
			{
				style.Flag |= PlainStyleFlag.FontStyleUnderline;
				style.Underline = xmlStyle.underline == "true";
			}

			// text-wrap
			if (!string.IsNullOrEmpty(xmlStyle.textWrap))
			{
				style.Flag |= PlainStyleFlag.TextWrap;
				style.TextWrapMode = XmlFileFormatHelper.DecodeTextWrapMode(xmlStyle.textWrap);
			}

			// padding
			if (!string.IsNullOrEmpty(xmlStyle.padding))
			{
				style.Flag |= PlainStyleFlag.Padding;
				style.Padding = TextFormatHelper.DecodePadding(xmlStyle.padding);
			}
			
			return style;
		}

		internal static void UpdateCellRenderAlign(ReoGridControl ctrl, ReoGridCell cell)
		{
			// when a horizontal alignment to be set, the render-alignment should also be updated.
			ReoGridHorAlign halign = ReoGridHorAlign.General;

			if (cell.Style != null && cell.Style.HasStyle(PlainStyleFlag.HorizontalAlign))
			{
				halign = cell.Style.HAlign;
			}
			else
			{
				halign = ReoGridStyleUtility.GetDefaultStyleIgnoreCell(ctrl, cell.Row, cell.Col).HAlign;
			}

			if (halign != ReoGridHorAlign.General)
			{
				switch (cell.Style.HAlign)
				{
					case ReoGridHorAlign.Left:
						cell.RenderHorAlign = ReoGridRenderHorAlign.Left;
						break;
					case ReoGridHorAlign.Center:
						cell.RenderHorAlign = ReoGridRenderHorAlign.Center;
						break;
					case ReoGridHorAlign.Right:
						cell.RenderHorAlign = ReoGridRenderHorAlign.Right;
						break;
				}
			}
		}

		internal bool Equals(ReoGridRangeStyle styleA, ReoGridRangeStyle styleB)
		{
			if(styleA == null && styleB != null
				|| styleA != null && styleB == null
				|| styleA.Flag != styleB.Flag) 
				return false;

			if (styleA.HasStyle(PlainStyleFlag.FillColor)
				&& styleA.BackColor != styleB.BackColor)
				return false;

			if (styleA.HasStyle(PlainStyleFlag.FillPatternColor)
				&& styleA.FillPatternColor != styleB.FillPatternColor)
				return false;

			if (styleA.HasStyle(PlainStyleFlag.FillPatternStyle)
				&& styleA.FillPatternStyle != styleB.FillPatternStyle)
				return false;

			if (styleA.HasStyle(PlainStyleFlag.TextColor)
				&& styleA.TextColor != styleB.TextColor)
				return false;

			if (styleA.HasStyle(PlainStyleFlag.FontName)
				&& styleA.FontName != styleB.FontName)
				return false;

			if (styleA.HasStyle(PlainStyleFlag.FontSize)
				&& styleA.FontSize != styleB.FontSize)
				return false;

			if (styleA.HasStyle(PlainStyleFlag.FontStyleBold)
				&& styleA.Bold != styleB.Bold)
				return false;

			if (styleA.HasStyle(PlainStyleFlag.FontStyleItalic)
				&& styleA.Italic != styleB.Italic)
				return false;

			if (styleA.HasStyle(PlainStyleFlag.FontStyleStrikethrough)
				&& styleA.Strikethrough != styleB.Strikethrough)
				return false;

			if (styleA.HasStyle(PlainStyleFlag.FontStyleUnderline)
				&& styleA.Underline != styleB.Underline)
				return false;

			if (styleA.HasStyle(PlainStyleFlag.HorizontalAlign)
				&& styleA.HAlign != styleB.HAlign)
				return false;

			if (styleA.HasStyle(PlainStyleFlag.VerticalAlign)
				&& styleA.VAlign != styleB.VAlign)
				return false;

			if (styleA.HasStyle(PlainStyleFlag.TextWrap)
				&& styleA.TextWrapMode != styleB.TextWrapMode)
				return false;

			if (styleA.HasStyle(PlainStyleFlag.Padding)
				&& styleA.Padding != styleB.Padding)
				return false;

			return true;
		}
	}
	#endregion // ReoGridStyleUtility
	#endregion // Cell Style

	#endregion // Styles

	#region Partial Grid
	public enum PartialGridCopyFlag : int
	{
		All = CellAll | BorderAll,

		CellAll = CellData | CellStyle,

		CellData = 0x1,
		CellStyle = 0x2,
		//CellFormat = 0x4,  contained in CellData

		BorderAll = HBorder | VBorder,
		HBorder = 0x10,
		VBorder = 0x20,
	}

	internal enum ExPartialGridCopyFlag
	{
		None,

		/// <summary>
		/// Copy all borders that around the cells (ignores border's owner property)
		/// </summary>
		BorderOutsideOwner = 0x10,

		// copy merged-range-info 

		//CellBounds,
	}

	[Serializable]
	public class PartialGrid
	{
		private int rows;

		public int Rows
		{
			get { return rows; }
			set { rows = value; }
		}

		private int cols;

		public int Cols
		{
			get { return cols; }
			set { cols = value; }
		}

		private RegularTreeArray<ReoGridCell> cells;
		public RegularTreeArray<ReoGridCell> Cells
		{
			get { return cells; }
			set { cells = value; }
		}

		private RegularTreeArray<ReoGridHBorder> hBorders;
		internal RegularTreeArray<ReoGridHBorder> HBorders
		{
			get { return hBorders; }
			set { hBorders = value; }
		}

		private RegularTreeArray<ReoGridVBorder> vBorders;
		internal RegularTreeArray<ReoGridVBorder> VBorders
		{
			get { return vBorders; }
			set { vBorders = value; }
		}

		/// <summary>
		/// Create an empty partial grid without and data, borders and styles
		/// </summary>
		public PartialGrid()
		{
		}

		/// <summary>
		/// Create an empty partial grid with specified capacity
		/// </summary>
		/// <param name="initRows">capacity of rows</param>
		/// <param name="initCols">capacity of cols</param>
		public PartialGrid(int initRows, int initCols)
		{
			this.rows = initRows;
			this.cols = initCols;
		}

		public PartialGrid(object[,] data)
		{
			this.cells = new RegularTreeArray<ReoGridCell>();
			this.rows = data.GetLength(0);
			this.cols = data.GetLength(1);

			for (int r = 0; r < this.rows; r++)
			{
				for (int c = 0; c < this.cols; c++)
				{
					this.cells[r, c] = new ReoGridCell()
					{
						Row = r,
						Col = c,
						Rowspan = 1,
						Colspan = 1,
						Data = data[r, c],
					};
				}
			}
		}

		public override bool Equals(object obj)
		{
			if (obj is PartialGrid)
				return Equals((PartialGrid)obj, PartialGridCopyFlag.All);
			else
				return base.Equals(obj);
		}

		public bool Equals(PartialGrid anotherPartialGrid, PartialGridCopyFlag flag)
		{
			if (anotherPartialGrid.rows != rows
				|| anotherPartialGrid.cols != cols)
				return false;

			for (int r = 0; r < rows; r++)
			{
				for (int c = 0; c < cols; c++)
				{
					var a = cells[r, c];
					var b = anotherPartialGrid.cells[r, c];
					
					if ((flag & PartialGridCopyFlag.CellData) == PartialGridCopyFlag.CellData)
					{
						if (a == null && b != null
							|| b == null && a != null
							|| a.Data == null || b.Data != null
							|| b.Data == null || a.Data != null)
							return false;

						if (Convert.ToString(a.Display) != Convert.ToString(b.Display))
							return false;

						if (!string.Equals(a.Formula, b.Formula, StringComparison.CurrentCultureIgnoreCase))
							return false;

						if (a.DataFormat != b.DataFormat)
							return false;

						if (a.DataFormatArgs == null && b.DataFormatArgs != null
							|| a.DataFormatArgs != null && b.DataFormatArgs == null)
							return false;

						if (!a.DataFormatArgs.Equals(b.DataFormatArgs))
							return false;
					}

					if ((flag & PartialGridCopyFlag.CellStyle) == PartialGridCopyFlag.CellStyle
						&& ReoGridRangeStyle.Equals(a.Style, b.Style))
						return false;

					if ((flag & PartialGridCopyFlag.HBorder) == PartialGridCopyFlag.HBorder)
					{
						var ba = hBorders[r, c];
						var bb = anotherPartialGrid.hBorders[r, c];

						if (ba == null && bb != null
							|| bb == null && ba != null)
							return false;

						if (ba.Cols != bb.Cols
							|| !ba.Border.Equals(bb.Border)
							|| !ba.Pos.Equals(bb.Pos))
							return false;
					}

					if ((flag & PartialGridCopyFlag.VBorder) == PartialGridCopyFlag.VBorder)
					{
						var ba = vBorders[r, c];
						var bb = anotherPartialGrid.vBorders[r, c];

						if (ba == null && bb != null
							|| bb == null && ba != null)
							return false;

						if (ba.Rows != bb.Rows
							|| !ba.Border.Equals(bb.Border)
							|| !ba.Pos.Equals(bb.Pos))
							return false;
					}
				}
			}

			return true;
		}
	}
	#endregion

	#region Cell
	/// <summary>
	/// Cell object for ReoGrid control
	/// </summary>
	[Serializable]
	public sealed class ReoGridCell
	{
		#region Position
		private ReoGridPos pos;
		internal ReoGridPos Pos
		{
			get { return pos; }
			set { pos = value; }
		}

		internal int Row
		{
			get { return pos.Row; }
			set { pos.Row = value; }
		}
		internal int Col
		{
			get { return pos.Col; }
			set { pos.Col = value; }
		}

		/// <summary>
		/// Get position of this cell
		/// </summary>
		/// <returns></returns>
		public ReoGridPos GetPos() { return pos; }
		/// <summary>
		/// Get index of row of this cell
		/// </summary>
		/// <returns></returns>
		public int GetRow() { return pos.Row; }
		/// <summary>
		/// Get index of column of this cell
		/// </summary>
		/// <returns></returns>
		public int GetCol() { return pos.Col; }
		#endregion // Postion

		#region Rowspan & Colspan
		private short colspan;
		internal short Colspan
		{
			get { return colspan; }
			set { colspan = value; }
		}

		private short rowspan;
		internal short Rowspan
		{
			get { return rowspan; }
			set { rowspan = value; }
		}

		/// <summary>
		/// Get number of colspan
		/// </summary>
		/// <returns></returns>
		public short GetColspan() { return colspan; }
		/// <summary>
		/// Get number of rowspan
		/// </summary>
		/// <returns></returns>
		public short GetRowspan() { return rowspan; }
		#endregion // Rowspan & Colspan

		#region Size
		private Rectangle bounds;
		internal Rectangle Bounds
		{
			get { return bounds; }
			set
			{
				bounds = value;
				UpdateContentBounds();
			}
		}
		internal int Width
		{
			get { return bounds.Width; }
			set { bounds.Width = value; UpdateContentBounds(); }
		}
		internal int Height
		{
			get { return bounds.Height; }
			set { bounds.Height = value; UpdateContentBounds(); }
		}
		#endregion // Size

		#region Location
		internal int Top
		{
			get { return bounds.Y; }
			set
			{
				bounds.Y = value;
				UpdateContentBounds();
			}
		}
		internal int Left
		{
			get { return bounds.X; }
			set
			{
				bounds.X = value;
				UpdateContentBounds();
			}
		}
		internal int Right
		{
			get { return bounds.Right; }
			set { bounds.Width += bounds.Right - value;
			UpdateContentBounds();
			}
		}
		internal int Bottom
		{
			get { return bounds.Bottom; }
			set { bounds.Height += bounds.Bottom - value;
			UpdateContentBounds();
			}
		}
		#endregion // Location

		#region Data Format
		private CellDataFormatFlag dataFormat;
		/// <summary>
		/// Get or set the data format type
		/// </summary>
		public CellDataFormatFlag DataFormat
		{
			get { return dataFormat; }
			set { dataFormat = value; }
		}

		private object dataFormatArgs;

		/// <summary>
		/// Get or set the argument of data format type
		/// </summary>
		public object DataFormatArgs
		{
			get { return dataFormatArgs; }
			set { dataFormatArgs = value; }
		}
		#endregion // Data Format

		#region Data, Display & Formula
		internal string Formula { get; set; }
		/// <summary>
		/// Get formula of cell
		/// </summary>
		/// <returns></returns>
		public string GetFormula() { return Formula; }

		private object data;
		/// <summary>
		/// Get or set the data
		/// </summary>
		public object Data
		{
			get { return data; }
			set { data = value; }
		}

		private string display;
		/// <summary>
		/// Get or set the display text
		/// </summary>
		public string Display
		{
			get { return display; }
			set { display = value; }
		}
		#endregion // Data, Display & Formula

		#region Merged Cell
		private ReoGridPos mergeStartPos = ReoGridPos.Empty;
		internal ReoGridPos MergeStartPos
		{
			get { return mergeStartPos; }
			set
			{
				Debug.Assert(value.Row >= -1 && value.Col >= -1);

				mergeStartPos = value;
			}
		}

		private ReoGridPos mergeEndPos = ReoGridPos.Empty;
		internal ReoGridPos MergeEndPos
		{
			get { return mergeEndPos; }
			set
			{
				if (value.Row > -1 && value.Col <= -1
					|| value.Row <= -1 && value.Col > -1) Debug.Assert(false);

				Debug.Assert(value.Row >= -1 && value.Col >= -1);
				mergeEndPos = value;
			}
		}

		internal bool IsStartMergedCell
		{
			get { return !MergeStartPos.IsEmpty && pos.Equals(MergeStartPos); }
		}
		internal bool IsEndMergedCell
		{
			get { return !MergeEndPos.IsEmpty && pos.Equals(MergeEndPos); }
		}

		/// <summary>
		/// Check whether this cell is merged cell
		/// </summary>
		public bool IsMergedCell
		{
			get { return IsStartMergedCell; }
		}

		/// <summary>
		/// Check whether this cell is an valid cell
		/// </summary>
		public bool IsValidCell
		{
			get { return rowspan >= 1 && colspan >= 1; }
		}

		/// <summary>
		/// Check whether this cell inside a merged range
		/// </summary>
		public bool InsideMergedRange
		{
			get { return IsStartMergedCell || (rowspan == 0 && colspan == 0); }
		}
		#endregion // Merged Cell

		#region Style
		private ReoGridRangeStyle style;
		internal ReoGridRangeStyle Style
		{
			get { return style; }
			set { style = value; }
		}

		private Font scaledFont;
		internal Font ScaledFont
		{
			get { return scaledFont; }
			set { scaledFont = value; }
		}

		private RectangleF textBounds;
		internal RectangleF TextBounds
		{
			get { return textBounds; }
			set { textBounds = value; }
		}

		internal float TextBoundsTop { get { return textBounds.Y; } set { this.textBounds.Y = value; } }
		internal float TextBoundsLeft { get { return textBounds.X; } set { this.textBounds.X = value; } }
		internal float TextBoundsWidth { get { return textBounds.Width; } set { this.textBounds.Width = value; } }
		internal float TextBoundsHeight { get { return textBounds.Height; } set { this.textBounds.Height = value; } }

		private ReoGridRenderHorAlign renderHorAlign = ReoGridRenderHorAlign.Left;
		internal ReoGridRenderHorAlign RenderHorAlign
		{
			get { return renderHorAlign; }
			set { renderHorAlign = value; }
		}

		private short renderTextColumnLeftSpan;
		internal short RenderTextColumnLeftSpan
		{
			get { return renderTextColumnLeftSpan; }
			set { renderTextColumnLeftSpan = value; }
		}

		private short renderTextColumnRightSpan;
		internal short RenderTextColumnRightSpan
		{
			get { return renderTextColumnRightSpan; }
			set { renderTextColumnRightSpan = value; }
		}

		private Color renderColor = Color.Empty;
		/// <summary>
		/// Get the text color for cell render. Set text color by using SetRangeStyle method of control.
		/// </summary>
		internal Color RenderColor { get { return renderColor; } set { renderColor = value; } }
		public Color GetRenderColor() { return renderColor; }

		internal float RenderScaleFactor { get; set; }

		[NonSerialized]
		private StringFormat stringFormat;
		internal StringFormat StringFormat { get { return stringFormat; } set { stringFormat = value; } }

		// todo: support multi-lines
		private float distributedIndentSpacing;
		internal float DistributedIndentSpacing { get { return distributedIndentSpacing; } set { distributedIndentSpacing = value; } }
		#endregion // Style

		#region Cell Body
		internal void UpdateContentBounds()
		{
			if (Body != null)
			{
				Rectangle cb = new Rectangle(style.Padding.Left, style.Padding.Top,
					bounds.Width - 1 - style.Padding.Left - style.Padding.Right,
					bounds.Height - 1 - style.Padding.Top - style.Padding.Bottom);

				if (Body.Bounds != cb)
				{
					Body.Bounds = cb;
					Body.OnBoundsChanged(this);
				}
			}
		}

		/// <summary>
		/// Get or set the user data to cell
		/// </summary>
		public object Tag { get; set; }

		internal ICellBody body;

		/// <summary>
		/// Get or set the cell body
		/// </summary>
		public ICellBody Body
		{
			get { return body; }
			set
			{
				this.body = value;
				UpdateContentBounds();
			}
		}
		#endregion
	}

	#region Cell Utility
	public static class ReoGridCellUtility
	{
		/// <summary>
		/// Clone new cell instance from this object
		/// </summary>
		/// <param name="source"></param>
		/// <returns></returns>
		public static void CopyCell(ReoGridCell toCell, ReoGridCell fromCell)
		{
			// base
			toCell.Pos = fromCell.Pos;
			toCell.Rowspan = fromCell.Rowspan;
			toCell.Colspan = fromCell.Colspan;
			toCell.MergeStartPos = fromCell.MergeStartPos;
			toCell.MergeEndPos = fromCell.MergeEndPos;
			toCell.Bounds = fromCell.Bounds;

			// content
			CopyCellContent(toCell, fromCell);
		}

		public static void CopyCellContent(ReoGridCell toCell, ReoGridCell fromCell)
		{
			// cell content
			toCell.Data = fromCell.Data;
			toCell.Display = fromCell.Display;
			toCell.Formula = fromCell.Formula;
			toCell.DataFormat = fromCell.DataFormat;
			toCell.DataFormatArgs = fromCell.DataFormatArgs;
			toCell.Body = fromCell.body;

			// render parameters
			toCell.RenderHorAlign = fromCell.RenderHorAlign;
			toCell.RenderColor = fromCell.RenderColor;
			toCell.StringFormat = fromCell.StringFormat;
			toCell.DistributedIndentSpacing = fromCell.DistributedIndentSpacing;

			// style & display
			toCell.Style = ReoGridRangeStyle.Clone(fromCell.Style);
			toCell.ScaledFont = fromCell.ScaledFont;
			toCell.TextBounds = fromCell.TextBounds;

			// custom content
			toCell.Tag = fromCell.Tag;
		}
	}
	#endregion // Cell Utility

	#region Cell Body
	/// <summary>
	/// Designing
	/// </summary>
	public interface ICellBody
	{
		void OnSetup(ReoGridControl ctrl, ReoGridCell cell);

		Rectangle Bounds { get; set; }
		void OnBoundsChanged(ReoGridCell cell);

		bool AutoCaptureMouse();
		bool OnMouseDown(RGCellMouseEventArgs e);
		bool OnMouseMove(RGCellMouseEventArgs e);
		bool OnMouseUp(RGCellMouseEventArgs e);
		bool OnMouseEnter(RGCellMouseEventArgs e);
		bool OnMouseLeave(RGCellMouseEventArgs e);
		void OnMouseWheel(RGCellMouseEventArgs e);

		bool OnKeyDown(KeyEventArgs e);
		bool OnKeyUp(KeyEventArgs e);

		void OnPaint(RGDrawingContext dc);

		bool OnStartEdit(ReoGridCell cell);
		object OnEndEdit(ReoGridCell cell, object data);
		void OnGotFocus(ReoGridCell cell);
		void OnLostFocus(ReoGridCell cell);

		object OnSetData(object data);
		object OnSetText(string text);
		object GetData();
	}

	public class CellBody : ICellBody
	{
		public virtual void OnSetup(ReoGridControl ctrl, ReoGridCell cell) { }

		public virtual Rectangle Bounds { get; set; }
		public virtual void OnBoundsChanged(ReoGridCell cell) { }

		public virtual bool AutoCaptureMouse() { return true; }
		public virtual bool OnMouseDown(RGCellMouseEventArgs e) { return false; }
		public virtual bool OnMouseMove(RGCellMouseEventArgs e) { return false; }
		public virtual bool OnMouseUp(RGCellMouseEventArgs e) { return false; }
		public virtual bool OnMouseEnter(RGCellMouseEventArgs e) { return false; }
		public virtual bool OnMouseLeave(RGCellMouseEventArgs e) { return false; }
		public virtual void OnMouseWheel(RGCellMouseEventArgs e) { }

		public virtual bool OnKeyDown(KeyEventArgs e) { return false; }
		public virtual bool OnKeyUp(KeyEventArgs e) { return false; }

		public virtual void OnPaint(RGDrawingContext dc)
		{
			dc.DrawCellBackground();
			dc.DrawCellText();
		}

		public virtual bool OnStartEdit(ReoGridCell cell) { return true; }
		public virtual object OnEndEdit(ReoGridCell cell, object data) { return data; }
		public virtual void OnGotFocus(ReoGridCell cell) { }
		public virtual void OnLostFocus(ReoGridCell cell) { }

		public virtual object OnSetData(object data) { return data; }
		public virtual object OnSetText(string text) { return text; }
		public virtual object GetData() { return null; }
	}
	#endregion // Cell body

	/// <summary>
	/// Drawing context for render grid control
	/// </summary>
	public sealed class RGDrawingContext
	{
		public ReoGridControl Grid { get; set; }

		internal ReoGridControl.IPart CurrentPart { get; set; }

		public DrawMode DrawMode { get; set; }

		#region Cell Methods
		public ReoGridCell Cell { get; set; }

		public ReoGridPos CellPos { get; set; }

		/// <summary>
		/// Not available yet
		/// </summary>
		public void DrawCellText()
		{
			if (CurrentPart is ReoGridControl.CellsViewport
				&& Cell != null
				&& !string.IsNullOrEmpty(Cell.Display)
				&& Cell.TextBounds.Width > 0
				&& Cell.TextBounds.Height > 0)
			{
				var view = ((ReoGridControl.CellsViewport)CurrentPart);

				Graphics.ResetTransform();

				view.DrawCellText(this, Cell);

				float scaleFactor = Grid.ScaleFactor;

				if (scaleFactor != 1f)
				{
					Graphics.ScaleTransform(scaleFactor, scaleFactor);
				}

				Graphics.TranslateTransform(Cell.Left, Cell.Top);
			}
		}

		/// <summary>
		/// Not available yet
		/// </summary>
		internal void DrawCellBackground()
		{
			if (CurrentPart is ReoGridControl.CellsViewport
				&& Cell != null)
			{
				((ReoGridControl.CellsViewport)CurrentPart).DrawCellBackground(this, Cell.Row, Cell.Col);

				Graphics.ResetTransform();

				float scaleFactor = Grid.ScaleFactor;

				if (scaleFactor != 1f)
				{
					Graphics.ScaleTransform(scaleFactor, scaleFactor);
				}

				Graphics.TranslateTransform(Cell.Left, Cell.Top);
			}
		}
		#endregion // Cell Methods

		#region Clip
		private Stack<RectangleF> clipStack = new Stack<RectangleF>(2);

		public void PushClip(RectangleF clip)
		{
			if (this.Graphics.IsClipEmpty)
			{
				this.Graphics.SetClip(clip);
			}
			else
			{
				this.Graphics.IntersectClip(clip);
			}

			this.clipStack.Push(clip);
		}

		public void PopClip()
		{
			this.clipStack.Pop();

			if (clipStack.Count == 0)
			{
				this.Graphics.ResetClip();
			}
			else
			{
				var curr = clipStack.Peek();
				this.Graphics.SetClip(curr);
			}
		}

		public Rectangle ClipBounds { get; set; }

		#endregion // Clip

		public RGDrawingContext(ReoGridControl instance, DrawMode drawMode,
			Graphics g, Rectangle clipBounds)
		{
			this.Graphics = g;
			this.Grid = instance;
			this.DrawMode = drawMode;
			this.ClipBounds = clipBounds;
		}

		#region Graphics Wrap

		internal GraphicsState CellDrawGraphicsState { get; set; }
		internal GraphicsState CoreDrawGraphicsState { get; set; }

		public Graphics Graphics { get; set; }

		#endregion
	}

	/// <summary>
	/// Drawing Mode for render grid control
	/// </summary>
	public enum DrawMode
	{
		View,
		Preview,
		Print,
	}

	namespace Rendering
	{
		/// <summary>
		/// Abstract graphics define
		/// </summary>
		internal interface IGraphics
		{

		}
	}

	class ReferenceRange
	{
		public ReoGridCell StartCell { get; set; }
		public ReoGridCell EndCell { get; set; }

		public ReferenceRange(ReoGridCell startCell, ReoGridCell endCell)
		{
			this.StartCell = startCell;
			this.EndCell = endCell;
		}

		public bool Contains(ReoGridCell cell)
		{
			return cell.Row >= StartCell.Row && cell.Row <= EndCell.Row
				&& cell.Col >= StartCell.Col && cell.Col <= EndCell.Col;
		}
	}
	#endregion

	#region Border

	[Serializable]
	internal class ReoGridHBorder : ReoGridAbstractBorder
	{
		private int cols;

		public int Cols
		{
			get { return cols; }
			set { cols = value; }
		}

		private HBorderOwnerPosition pos;

		internal HBorderOwnerPosition Pos
		{
			get { return pos; }
			set { pos = value; }
		}

		public static ReoGridHBorder Clone(ReoGridHBorder source)
		{
			return source == null ? null : new ReoGridHBorder
			{
				cols = source.Cols,
				pos = source.Pos,
				Border = source.Border,
			};
		}
	}

	[Serializable]
	internal class ReoGridVBorder : ReoGridAbstractBorder
	{
		private int rows;

		public int Rows
		{
			get { return rows; }
			set { rows = value; }
		}

		private VBorderOwnerPosition pos;

		internal VBorderOwnerPosition Pos
		{
			get { return pos; }
			set { pos = value; }
		}

		public static ReoGridVBorder Clone(ReoGridVBorder source)
		{
			return source == null ? null : new ReoGridVBorder
			{
				rows = source.Rows,
				pos = source.Pos,
				Border = source.Border,
			};
		}
	}

	internal enum HBorderOwnerPosition : int
	{
		None = 0,

		All = Top | Bottom,
		Top = 1,
		Bottom = 2,
	}
	internal enum VBorderOwnerPosition : int
	{
		None = 0,

		All = Left | Right,
		Left = 1,
		Right = 2,
	}

	[Serializable]
	internal abstract class ReoGridAbstractBorder
	{
		private ReoGridBorderStyle border;

		internal ReoGridBorderStyle Border
		{
			get { return border; }
			set { border = value; }
		}
	}

	/// <summary>
	/// Position of borders in range or cell
	/// </summary>
	public enum ReoGridBorderPos : short
	{
		/// <summary>
		/// No border
		/// </summary>
		None = 0x0,

		/// <summary>
		/// Top border inside range or cell
		/// </summary>
		Top = 0x1,

		/// <summary>
		/// Bottom border inside range or cell
		/// </summary>
		Bottom = 0x2,

		/// <summary>
		/// Left side border inside range or cell
		/// </summary>
		Left = 0x4,

		/// <summary>
		/// Right side border inside range or cell
		/// </summary>
		Right = 0x8,

		/// <summary>
		/// Horizontal borders inside range or cell
		/// </summary>
		InsideHorizontal = 0x10,

		/// <summary>
		/// Vertical borders inside range or cell
		/// </summary>
		InsideVertical = 0x20,

		/// <summary>
		/// Slash lines inside cell (Reserved)
		/// </summary>
		Slash = 0x100,

		/// <summary>
		/// Backslash lines inside cell (Reserved)
		/// </summary>
		Backslash = 0x200,

		/// <summary>
		/// Borders in left and right side in range or cell
		/// </summary>
		LeftRight = Left | Right,

		/// <summary>
		/// Borders in top and bottom in range or cell
		/// </summary>
		TopBottom = Top | Bottom,

		/// <summary>
		/// Borders around range or cell
		/// </summary>
		Outline = Top | Bottom | Left | Right,

		/// <summary>
		/// Horizontal and vertical borders inside range or cell
		/// </summary>
		InsideAll = InsideHorizontal | InsideVertical,

		/// <summary>
		/// All borders belong to range or cell
		/// </summary>
		All = Outline | InsideAll,

		/// <summary>
		/// Cross line in single cell (Both Slash and Backslash, Reserved)
		/// </summary>
		X = Slash | Backslash,
	}

	/// <summary>
	/// Line style of border
	/// </summary>
	public enum BorderLineStyle : byte
	{
		None = 0,
		Solid = 1,
		Dotted = 2,
		Dashed = 3,
		DoubleLine = 4,
		Dashed2 = 5,
		Dashed6121 = 6,
		Dashed2242 = 7,
		BoldDashed224 = 8,
		BoldDashed211 = 9,
		BoldDashed31 = 10,
		BoldDotted = 11,
		BoldSolid = 12,
		BoldSolidStrong = 13,
	}

	/// <summary>
	/// Draw borders at the specified location
	/// </summary>
	public sealed class BorderPainter : IDisposable
	{
		private static readonly BorderPainter instance = new BorderPainter();
		
		/// <summary>
		/// Get BorderPainter instance
		/// </summary>
		public static BorderPainter Instance { get { return instance; } }

		private readonly Pen[] pens = new Pen[14];

		private BorderPainter()
		{
			Pen p;

			// Solid
			p = new Pen(Color.Black);
			pens[(byte)BorderLineStyle.Solid] = p;

			// Dahsed
			p = new Pen(Color.Black);
			p.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
			pens[(byte)BorderLineStyle.Dashed] = p;

			// Dotted
			p = new Pen(Color.Black);
			p.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;
			pens[(byte)BorderLineStyle.Dotted] = p;

			// DoubleLine
			p = new Pen(Color.Black, 3);
			p.CompoundArray = new float[] { 0f, 0.2f, 0.8f, 1f };
			pens[(byte)BorderLineStyle.DoubleLine] = p;

			// Dashed2
			p = new Pen(Color.Black);
			p.DashPattern = new float[] { 2f, 2f };
			pens[(byte)BorderLineStyle.Dashed2] = p;

			// Dashed6121
			p = new Pen(Color.Black);
			p.DashPattern = new float[] { 6f, 1f, 2f, 1f };
			pens[(byte)BorderLineStyle.Dashed6121] = p;

			// Dashed2242
			p = new Pen(Color.Black);
			p.DashPattern = new float[] { 2f, 2f, 4f, 2f };
			pens[(byte)BorderLineStyle.Dashed2242] = p;

			// BoldDashed224
			p = new Pen(Color.Black, 2);
			p.DashStyle = System.Drawing.Drawing2D.DashStyle.DashDot;
			pens[(byte)BorderLineStyle.BoldDashed224] = p;

			// BoldDashed211
			p = new Pen(Color.Black, 2);
			p.DashStyle = System.Drawing.Drawing2D.DashStyle.DashDotDot;
			pens[(byte)BorderLineStyle.BoldDashed211] = p;

			// BoldDashed31
			p = new Pen(Color.Black, 2);
			p.DashPattern = new float[] { 6f, 1f, 2f, 1f };
			pens[(byte)BorderLineStyle.BoldDashed31] = p;

			// BoldDotted
			p = new Pen(Color.Black, 2);
			p.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;
			pens[(byte)BorderLineStyle.BoldDotted] = p;

			// BoldSolid
			p = new Pen(Color.Black, 2);
			pens[(byte)BorderLineStyle.BoldSolid] = p;

			// BoldSolidStrong
			p = new Pen(Color.Black, 3);
			pens[(byte)BorderLineStyle.BoldSolidStrong] = p;
		}

		internal void DrawRect(Graphics g, Rectangle rect, BorderLineStyle style, Color color, Color bgColor)
		{
			if (style == BorderLineStyle.None) return;
			Pen p = pens[(byte)style];
			p.EndCap = LineCap.Square;
			p.StartCap = LineCap.Square;
			g.DrawRectangle(p, rect);
		}

		/// <summary>
		/// Draw border at specified location
		/// </summary>
		/// <param name="g">instance for graphics object</param>
		/// <param name="x">x coordinate of start point</param>
		/// <param name="y">y coordinate of start point</param>
		/// <param name="x2">x coordinate of end point</param>
		/// <param name="y2">y coordinate of end point</param>
		/// <param name="style">style instance of border</param>
		public void DrawLine(Graphics g, float x, float y, float x2, float y2, ReoGridBorderStyle style)
		{
			DrawLine(g, x, y, x2, y2, style.Style, style.Color);
		}
		/// <summary>
		/// Draw border at specified location
		/// </summary>
		/// <param name="g">instance for graphics object</param>
		/// <param name="x">x coordinate of start point</param>
		/// <param name="y">y coordinate of start point</param>
		/// <param name="x2">x coordinate of end point</param>
		/// <param name="y2">y coordinate of end point</param>
		/// <param name="style">style instance of border</param>
		/// <param name="bgColor">fill color used inner double outline</param>
		public void DrawLine(Graphics g, int x, int y, int x2, int y2, ReoGridBorderStyle style, Color bgColor)
		{
			DrawLine(g, x, y, x2, y2, style.Style, style.Color, bgColor);
		}
		/// <summary>
		/// Draw border at specified location
		/// </summary>
		/// <param name="g">instance for graphics object</param>
		/// <param name="x">x coordinate of start point</param>
		/// <param name="y">y coordinate of start point</param>
		/// <param name="x2">x coordinate of end point</param>
		/// <param name="y2">y coordinate of end point</param>
		/// <param name="style">style flag of border</param>
		/// <param name="color">color of border</param>
		public void DrawLine(Graphics g, float x, float y, float x2, float y2, BorderLineStyle style, Color color)
		{
			DrawLine(g, x, y, x2, y2, style, color, Color.White);
		}
		/// <summary>
		/// Draw border at specified location
		/// </summary>
		/// <param name="g">instance for graphics object</param>
		/// <param name="x">x coordinate of start point</param>
		/// <param name="y">y coordinate of start point</param>
		/// <param name="x2">x coordinate of end point</param>
		/// <param name="y2">y coordinate of end point</param>
		/// <param name="style">style flag of border</param>
		/// <param name="color">color of border</param>
		/// <param name="bgColor">fill color used inner double outline</param>
		public void DrawLine(Graphics g, float x, float y, float x2, float y2, BorderLineStyle style, Color color, Color bgColor)
		{
			if (style == BorderLineStyle.None) return;

			Pen p = pens[(byte)style];
			p.Color = color;
			p.StartCap = LineCap.Square;
			p.EndCap = LineCap.Square;

			g.DrawLine(p, x, y, x2, y2);

			if (style == BorderLineStyle.DoubleLine)
			{
				using (Pen bp = new Pen(bgColor))
					g.DrawLine(bp, x, y, x2, y2);
			}
		}

		/// <summary>
		/// Release all GDI objects
		/// </summary>
		public void Dispose()
		{
			for (int i = 1; i < pens.Length; i++)
			{
				pens[i].Dispose();
				pens[i] = null;
			}
		}
	}

	/// <summary>
	/// Style of border for range
	/// </summary>
	[Serializable]
	public struct ReoGridBorderStyle
	{
		/// <summary>
		/// No border
		/// </summary>
		public static readonly ReoGridBorderStyle Empty = new ReoGridBorderStyle
		{
			color = Color.Empty,
			style = BorderLineStyle.None,
		};

		private BorderLineStyle style;

		/// <summary>
		/// The border style
		/// </summary>
		public BorderLineStyle Style
		{
			get { return style; }
			set { style = value; }
		}

		private Color color;

		/// <summary>
		/// Color of border
		/// </summary>
		public Color Color
		{
			get { return color; }
			set { color = value; }
		}

		/// <summary>
		/// Determines whether this style is empty
		/// </summary>
		public bool IsEmpty
		{
			get { return this.Equals(Empty); }
		}

		/// <summary>
		/// Compare two border styles check whether they are same
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public override bool Equals(object obj)
		{
			if (obj == null && this.IsEmpty) return true;
			if (!(obj is ReoGridBorderStyle)) return false;
			ReoGridBorderStyle s2 = (ReoGridBorderStyle)obj;
			return style == s2.style && color == s2.color;
		}

		public static bool operator ==(ReoGridBorderStyle s1, object s2)
		{
			return s1.Equals(s2);
		}

		public static bool operator !=(ReoGridBorderStyle s1, object s2)
		{
			return !s1.Equals(s2);
		}

		public override int GetHashCode()
		{
			return (byte)style ^ color.ToArgb();
		}

		public override string ToString()
		{
			return "BorderStyle[Style=" + style + ",Color=" + color + "]";
		}

		#region Predefine Border Styles
		public static readonly ReoGridBorderStyle SolidBlack = new ReoGridBorderStyle
		{
			Color = Color.Black,
			Style = BorderLineStyle.Solid
		};
		public static readonly ReoGridBorderStyle SolidGray = new ReoGridBorderStyle
		{
			Color = Color.Gray,
			Style = BorderLineStyle.Solid
		};
		public static readonly ReoGridBorderStyle DottedBlack = new ReoGridBorderStyle
		{
			Color = Color.Black,
			Style = BorderLineStyle.Dotted
		};
		public static readonly ReoGridBorderStyle DottedGray = new ReoGridBorderStyle
		{
			Color = Color.Gray,
			Style = BorderLineStyle.Dotted
		};
		#endregion // Predefine Border Styles

	}

	/// <summary>
	/// Border info for ReoGrid
	/// </summary>
	[Serializable]
	public struct ReoGridBorderPosStyle
	{
		private ReoGridBorderPos pos;

		/// <summary>
		/// Position of border
		/// </summary>
		public ReoGridBorderPos Pos
		{
			get { return pos; }
			set { pos = value; }
		}

		private ReoGridBorderStyle style;

		/// <summary>
		/// Style of border
		/// </summary>
		public ReoGridBorderStyle Style
		{
			get { return style; }
			set { style = value; }
		}

		/// <summary>
		/// Create border with specified position and style
		/// </summary>
		/// <param name="pos">position of border</param>
		/// <param name="style">style of border</param>
		public ReoGridBorderPosStyle(ReoGridBorderPos pos, ReoGridBorderStyle style)
		{
			this.pos = pos;
			this.style = style;
		}
	}

	//[Serializable]
	//public class StyleGridBorder
	//{
	//  private Color color;

	//  public Color Color
	//  {
	//    get { return color; }
	//    set { color = value; }
	//  }

	//  private float weight;

	//  public float Weight
	//  {
	//    get { return weight; }
	//    set { weight = value; }
	//  }

	//  private DashStyle style;

	//  public DashStyle Style
	//  {
	//    get { return style; }
	//    set { style = value; }
	//  }

	//  public StyleGridBorder() { }

	//  public StyleGridBorder(Color color, DashStyle style, float weight)
	//  {
	//    this.color = color;
	//    this.style = style;
	//    this.weight = weight;
	//  }

	//  public StyleGridBorder(StyleGridBorder prototype)
	//    : this(prototype.Color, prototype.Style, prototype.Weight) { }

	//  public override bool Equals(object obj)
	//  {
	//    if (obj == null || (!(obj is StyleGridBorder))) return false;

	//    StyleGridBorder tb = obj as StyleGridBorder;

	//    return color.Equals(tb.color) && style == tb.style && weight == tb.weight;
	//  }
	//  public override int GetHashCode()
	//  {
	//    return color.ToArgb() ^ (int)weight ^ (int)style;
	//  }
	//}

	/// <summary>
	/// Border info for specified range. This object only be used as return object form only from GetRangeBorder method.
	/// </summary>
	[Serializable]
	public class ReoGridRangeBorderInfo
	{
		private ReoGridBorderPos hasNonUniformPos = ReoGridBorderPos.None;

		/// <summary>
		/// Borders at the positions are not same
		/// </summary>
		public ReoGridBorderPos NonUniformPos
		{
			get { return hasNonUniformPos; }
			set { hasNonUniformPos = value; }
		}

		/// <summary>
		/// Indicates whether the borders at specified position for all cells are not same
		/// </summary>
		/// <param name="pos">border position in range</param>
		/// <returns>true if borders at position are not same</returns>
		public bool IsNonUniform(ReoGridBorderPos pos)
		{
			return (hasNonUniformPos & pos) == pos;
		}

		private ReoGridBorderStyle top;
		/// <summary>
		/// Border style at top of range
		/// </summary>
		public ReoGridBorderStyle Top { get { return top; } set { top = value; } }

		private ReoGridBorderStyle right;
		/// <summary>
		/// Border style at right of range
		/// </summary>
		public ReoGridBorderStyle Right { get { return right; } set { right = value; } }

		private ReoGridBorderStyle bottom;
		/// <summary>
		/// Border style at bottom of range
		/// </summary>
		public ReoGridBorderStyle Bottom { get { return bottom; } set { bottom = value; } }

		private ReoGridBorderStyle left;
		/// <summary>
		/// Border  style at left of range
		/// </summary>
		public ReoGridBorderStyle Left { get { return left; } set { left = value; } }

		private ReoGridBorderStyle horizontal;
		/// <summary>
		/// Horizontal border style inside range
		/// </summary>
		public ReoGridBorderStyle InsideHorizontal { get { return horizontal; } set { horizontal = value; } }

		private ReoGridBorderStyle vertical;
		/// <summary>
		/// Vertical border style inside range
		/// </summary>
		public ReoGridBorderStyle InsideVertical { get { return vertical; } set { vertical = value; } }

		private ReoGridBorderStyle slash; // /
		/// <summary>
		/// Slash style inside range
		/// </summary>
		public ReoGridBorderStyle Slash { get { return slash; } set { slash = value; } }

		private ReoGridBorderStyle backslash;  // \
		/// <summary>
		/// Backslash style inside range
		/// </summary>
		public ReoGridBorderStyle Backslash { get { return backslash; } set { backslash = value; } }
	}

	#endregion

	#region Settings
	/// <summary>
	/// Settings of ReoGrid Control
	/// </summary>
	public enum ReoGridSettings : long
	{
		/// <summary>
		/// Indicates that Control works in readonly mode. Any changes are not allowed.
		/// </summary>
		Readonly = 0x00000001,

		/// <summary>
		/// Allows AutoFormatCell, FriendlyPercentInput, AutoAdjustRowHeight
		/// </summary>
		Edit_All = Edit_AutoFormatCell | Edit_FriendlyPercentInput | Edit_AutoAdjustRowHeight
			| Edit_AllowAdjustRowHeight | Edit_AllowAdjustColumnWidth | Edit_AllowUserScale
			| Edit_AutoPickingCellAddress,

		/// <summary>
		/// Indicates that allow data format after text editing by user.
		/// </summary>
		Edit_AutoFormatCell = 0x00000200,

		/// <summary>
		/// Indicates that allow putting '%' symbol at end of text when percent inputing.
		/// </summary>
		Edit_FriendlyPercentInput = 0x00000400,

		/// <summary>
		/// Indicates that allow adjusting the height of row when user enlarges font of cell.
		/// </summary>
		Edit_AutoAdjustRowHeight = 0x00000800,

		/// <summary>
		/// Allows user to adjust height of row by mouse
		/// </summary>
		Edit_AllowAdjustRowHeight = 0x00001000,

		/// <summary>
		/// Allows user to adjust column of width by mouse
		/// </summary>
		Edit_AllowAdjustColumnWidth = 0x0002000,

		/// <summary>
		/// Allows user to scale control by hoding Ctrl and scrolling mouse wheel.
		/// Ctrl+Plus, Ctrl+Minus and Ctrl+Zero will also be disabled.
		/// </summary>
		Edit_AllowUserScale = 0x0004000,

		Edit_AutoPickingCellAddress = 0x0008000,

		/// <summary>
		/// Show Column Header 
		/// </summary>
		View_ShowColumnHeader = 0x00010000,

		/// <summary>
		/// Show Row Indexer
		/// </summary>
		View_ShowRowHeader = 0x00020000,

		/// <summary>
		/// Show Column Header and Row Indexer
		/// </summary>
		View_ShowHeaders = View_ShowColumnHeader | View_ShowRowHeader,

		/// <summary>
		/// Show Horizontal Ruler (Reserved)
		/// </summary>
		View_ShowXRuler = 0x00040000,

		/// <summary>
		/// Show Vertical Ruler (Reserved)
		/// </summary>
		View_ShowYRuler = 0x00080000,

		/// <summary>
		/// Show Grid Line
		/// </summary>
		View_ShowGridLine = 0x00100000,

		/// <summary>
		/// Show Horizontal and Vertical ScrollBars
		/// </summary>
		View_ShowScrolls = View_ShowHorScroll | View_ShowVerScroll,
		
		/// <summary>
		/// Show Horizontal ScrollBars
		/// </summary>
		View_ShowHorScroll = 0x00200000,

		/// <summary>
		/// Show Vertical ScrollBars
		/// </summary>
		View_ShowVerScroll = 0x00400000,

		/// <summary>
		/// Show row outlines panel
		/// </summary>
		View_ShowRowOutlines = 0x00800000,

		/// <summary>
		/// Show column outlines panel
		/// </summary>
		View_ShowColumnOutlines = 0x01000000,

		/// <summary>
		/// Whether to run script if grid loaded from file which contains script
		/// </summary>
		Script_AutoRunOnload = 0x10000000,

		/// <summary>
		/// Prompt user to confirm whether to run script what loaded from file
		/// </summary>
		Script_PromptBeforeAutoRun = 0x20000000,

	}
	#endregion

}
