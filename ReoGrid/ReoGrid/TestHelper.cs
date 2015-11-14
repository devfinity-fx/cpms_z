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
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

using unvell.Common;

#if DEBUG
namespace unvell.ReoGrid
{
	public partial class ReoGridControl
	{
		/// <summary>
		/// [Debug Method]
		/// Validate whether border span is correct after border changes
		/// </summary>
		/// <returns>True if test passed</returns>
		public bool _Debug_Validate_BorderSpan()
		{
			bool result = true;

			int span = 0;
			ReoGridBorderStyle lastBorder = ReoGridBorderStyle.Empty;

			// check h-borders
			for (int r = this.rows.Count - 1; r >= 0; r--)
			{
				for (int c = this.cols.Count - 1; c >= 0; c--)
				{
					if (hBorders[r, c] == null || hBorders[r, c].Border == null)
					{
						span = 0;
						lastBorder = ReoGridBorderStyle.Empty;
					}
					else
					{
						if (lastBorder != null && hBorders[r, c].Border.Equals(lastBorder))
						{
							span++;
						}
						else
						{
							lastBorder = hBorders[r, c].Border;
							span = 1;
						}

						if (hBorders[r, c].Cols != span)
						{
							_Debug_MarkCellError(r, c, "hborder colspan", hBorders[r, c].Cols, span);
							result = false;
						}

						//if (span == 1 && hBorders[r, c].Pos == HBorderOwnerPosition.None)
						//{
						//	_Debug_MarkCellError(r, c, "hborder has no owner pos", "none", "any pos");
						//	result = false;
						//}
					}
				}

				span = 0;
			}

			span = 0;
			lastBorder = ReoGridBorderStyle.Empty;

			// check v-borders
			for (int c = this.cols.Count - 1; c >= 0; c--)
			{
				for (int r = this.rows.Count - 1; r >= 0; r--)
				{
					if (vBorders[r, c] == null || vBorders[r, c].Border == null)
					{
						span = 0;
						lastBorder = ReoGridBorderStyle.Empty;
					}
					else
					{
						if (lastBorder != null && vBorders[r, c].Border.Equals(lastBorder))
						{
							span++;
						}
						else
						{
							lastBorder = vBorders[r, c].Border;
							span = 1;
						}

						if (vBorders[r, c].Rows != span)
						{
							_Debug_MarkCellError(r, c, "vborder rowspan", vBorders[r, c].Rows, span);
							result = false;
						}

						//if (span == 1 && vBorders[r, c].Pos == VBorderOwnerPosition.None)
						//{
						//	_Debug_MarkCellError(r, c, "vborder has no owner pos", "none", "any pos");
						//	result = false;
						//}
					}
				}

				span = 0;
			}

			return result;
		}

		/// <summary>
		/// [Debug Method]
		/// Validate whether merged cell is correct after range merging
		/// </summary>
		/// <returns>True if test passed</returns>
		public bool _Debug_Validate_MergedCells()
		{
			bool rs = true;

			int rows = Math.Min(this.rows.Count, cells.MaxRow + 1);
			int cols = Math.Min(this.cols.Count, cells.MaxCol + 1);

			cells.Iterate(0, 0, rows, cols, true, (row, col, cell) =>
			{
				if (row != cell.Row || col != cell.Col)
				{
					_Debug_MarkCellError(row, col, "cell pos", cell.Row + "," + cell.Col, row + "," + col);
					rs = false;
				}

				// is merged start cell
				if (rs && cell.IsStartMergedCell)
				{
					#region From start-merge-cell

					int rowspan = cell.MergeEndPos.Row - cell.MergeStartPos.Row + 1;
					int colspan = cell.MergeEndPos.Col - cell.MergeStartPos.Col + 1;

					// merged start cell rowspan
					if (cell.Rowspan != rowspan || cell.Rowspan != cell.MergeEndPos.Row - cell.Row + 1)
					{
						_Debug_MarkCellError(cell.Row, cell.Col, "start cell rowspan", cell.Rowspan, rowspan);
						rs = false;
					}

					// merged start cell colspan
					if (cell.Colspan != colspan || cell.Colspan != cell.MergeEndPos.Col - cell.Col + 1)
					{
						_Debug_MarkCellError(cell.Row, cell.Col, "start cell colspan", cell.Colspan, colspan);
						rs = false;
					}

					#region Validate children cells
					for (int r = row; r <= cell.MergeEndPos.Row; r++)
					{
						for (int c = col; c <= cell.MergeEndPos.Col; c++)
						{
							if (cells[r, c] == null) continue;

							ReoGridCell gc = cells[r, c];

							if (!gc.IsStartMergedCell)
							{
								// merged middle cell
								if (gc.Rowspan != 0)
								{
									_Debug_MarkCellError(r, c, "cell rowspan", gc.Rowspan, 0);
									rs = false;
								}
								if (rs && gc.Colspan != 0)
								{
									_Debug_MarkCellError(r, c, "cell colspan", gc.Colspan, 0);
									rs = false;
								}
							}

							if (rs && (!gc.IsStartMergedCell && gc.MergeStartPos.Row > cell.Row && gc.MergeEndPos.Col > cell.Col))
							{
								_Debug_MarkCellError(r, c, "merged start pos after cell", gc.MergeStartPos, cell.Pos);
								rs = false;
							}

							// check cell merged start pos
							if (rs && gc.MergeStartPos != cell.Pos)
							{
								_Debug_MarkCellError(r, c, "cell merged start pos", gc.MergeStartPos, cell.Pos);
								rs = false;
							}

							// check cell merged end pos
							if (rs && gc.MergeEndPos != cell.MergeEndPos)
							{
								_Debug_MarkCellError(r, c, "cell merged end pos", gc.MergeEndPos, cell.MergeEndPos);
								rs = false;
							}

							// check right border in merged cell
							if (rs && c < this.cols.Count - 1 && c > col)
							{
								ReoGridCell rightCell = cells[r, c + 1];

								if (IsInsideSameMergedCell(gc, rightCell))
								{
									if (vBorders[r, c] == null || (vBorders[r, c] != null && vBorders[r, c].Rows != 0))
									{
										_Debug_MarkCellError(r, c, "merged cell right border should be null", null, null);
									}
								}
							}

							// check bottom border in merged cell
							if (rs && r < this.rows.Count - 1 && r > row)
							{
								ReoGridCell downCell = cells[r + 1, c];

								if (IsInsideSameMergedCell(gc, downCell))
								{
									if (hBorders[r, c] == null || (hBorders[r, c] != null && hBorders[r, c].Cols != 0))
									{
										_Debug_MarkCellError(r, c, "merged cell bottom border should be null", null, null);
									}
								}
							}

							if (!rs) break;
						}

						if (!rs) break;
					}
					#endregion // Validate child cells

					//Rectangle rangeBounds = GetRangeBounds(new ReoGridRange(cell.Row, cell.Col, cell.Rowspan, cell.Colspan));
					//rangeBounds.Width++;
					//rangeBounds.Height++;

					//if (cell.Bounds != rangeBounds)
					//{
					//_Debug_MarkCellError(cell.Row, cell.Col, "mismatched merged cell size", cell.Bounds, rangeBounds);
					//rs = false;
					//}

					#endregion // from start-merge-cell
				}
				// not a merged start cell, check its merged start cell
				else if (rs && (cell.Rowspan == 0 && cell.Colspan == 0))
				{
					var mergeStartCell = cells[cell.MergeStartPos.Row, cell.MergeStartPos.Col];
					if (mergeStartCell == null)
					{
						_Debug_MarkCellError(row, col, "cell merged start is null", null, "cell");
						rs = false;
					}
					else
					{
						if (mergeStartCell.MergeEndPos.Row < cell.Row
							|| mergeStartCell.MergeEndPos.Col < cell.Col)
						{
							_Debug_MarkCellError(row, col, "mismatched merged-start-pos");
							rs = false;
						}
					}
				}

				return rs ? 1 : 0;
			});

			return rs;
		}

		/// <summary>
		/// [Debug Method]
		/// Validate whether unmerged cells are correct after range unmerging
		/// </summary>
		/// <param name="range">Range to be tested</param>
		/// <returns>True if test passed</returns>
		public bool _Debug_Validate_Unmerged_Range(ReoGridRange range)
		{
			//if (range.IsEmpty)
			//{
			//	range = new ReoGridRange(0, 0, rows.Count, cols.Count);
			//}
			range = FixRange(range);

			bool rs = true;

			cells.Iterate(range.Row, range.Col, range.Rows, range.Cols, true, (r, c, cell) =>
			{
				if (!cell.MergeStartPos.IsEmpty) return 1;

				// unmerged cell rowspan should be 1
				if (cell.Rowspan != 1)
				{
					_Debug_MarkCellError(r, c, "cell rowspan", cell.Rowspan, 1);
					rs = false;
				}

				// unmerged cell colspan should be 1
				if (rs && cell.Colspan != 1)
				{
					_Debug_MarkCellError(r, c, "cell colspan", cell.Colspan, 1);
					rs = false;
				}

				if (rs)
				{
					// bounds of cell must be single grid
					RectangleF bounds = GetGridBounds(r, c);
					if (cell.Bounds != bounds)
					{
						_Debug_MarkCellError(r, c, "cell bounds", cell.Bounds, bounds);
						rs = false;
					}
				}

				return rs ? 1 : 0;
			});

			return rs;
		}

		/// <summary>
		/// [Debug Method]
		/// Validate whether all cells are correct after some operations
		/// </summary>
		/// <returns></returns>
		public bool _Debug_Validate_All()
		{
			bool rs = _Debug_Validate_BorderSpan();
			if (rs) rs = _Debug_Validate_MergedCells();
			if (rs) rs = _Debug_Validate_Unmerged_Range(ReoGridRange.EntireRange);

			return rs;
		}

		public void _Debug_MarkCellError(int r, int c, string msg)
		{
			_Debug_MarkCellError(r, c, msg, null, null);
		}

		/// <summary>
		/// [Debug Method]
		/// Set specified cell to warning style
		/// </summary>
		/// <param name="r">Index of row</param>
		/// <param name="c">Index of column</param>
		/// <param name="msg">Message to be printed out in log</param>
		/// <param name="but">Test value to be printed</param>
		/// <param name="expect">Expect value to be printed</param>
		public void _Debug_MarkCellError(int r, int c, string msg, object but, object expect)
		{
			SetCellStyle(r, c, new ReoGridRangeStyle()
			{
				Flag = PlainStyleFlag.FillColor,
				BackColor = Color.LightCoral
			});

			if (but == null)
			{
				Logger.Log("rgdebug", string.Format("[{0},{1}] {2}", r, c, msg));
			}
			else
			{
				Logger.Log("rgdebug", string.Format("[{0},{1}] {2}, expect {3} but {4}", r, c, msg, expect, but));
				Debug.WriteLine("rgdebug: " + string.Format("[{0},{1}] {2}, expect {3} but {4}", r, c, msg, expect, but));
			}
		}
	}
}

namespace unvell.ReoGrid.TestCases
{
	public enum DumpFlag : int
	{
		HBorder = 0x10,
		VBorder = 0x20,
	}

	public static class TestHelper
	{
		public static void DumpGrid(ReoGridControl grid, string file)
		{
			using (var fs = new FileStream(file, FileMode.Create, FileAccess.Write))
			{
				DumpGrid(grid, fs);
			}
		}

		public static void DumpGrid(ReoGridControl grid, Stream stream)
		{
			var sw = new StreamWriter(stream);
			
			for (int row = 0; row < grid.RowCount; row++)
			{
				for (int col = 0; col < grid.ColCount; col++)
				{
					var hborder = grid.RetrieveHBorder(row, col);
					if (hborder != null)
					{
						sw.Write(hborder.Cols);
					}
						
					sw.Write("\t");
				}

				sw.WriteLine("\n");
			}
			
		}
	}
}
#endif // DEBUG
