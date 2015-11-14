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
using System.Linq;
using System.Text;
using System.Windows.Forms;

using unvell.Common;
using unvell.ReoGrid.DataFormat;

namespace unvell.ReoGrid
{
	public class RGActionPerformedEventArgs : EventArgs
	{
		internal IAction Action { get; set; }
		public RGActionPerformedEventArgs(IAction action) { this.Action = action; }
	}

	public class RGPositionEventArgs : EventArgs
	{
		public ReoGridPos Position { get; set; }

		public RGPositionEventArgs(ReoGridPos pos)
		{
			this.Position = pos;
		}
	}

	/// <summary>
	/// Event raised on action was performed for cell.
	/// </summary>
	public class RGCellEventArgs : EventArgs
	{
		/// <summary>
		/// Cell of action performed
		/// </summary>
		public ReoGridCell Cell { get; set; }

		/// <summary>
		/// Create instance for CellEventArgs with specified cell.
		/// </summary>
		/// <param name="cell">Cell of action performed</param>
		public RGCellEventArgs(ReoGridCell cell) { this.Cell = cell; }
	}

	public class RGCellMouseEventArgs : EventArgs
	{
		public ReoGridControl Grid { get; set; }
		public MouseButtons Buttons { get; set; }
		public Point CursorPosition { get; set; }
		public int Clicks { get; set; }
		public int Delta { get; set; }
		public ReoGridCell Cell { get; set; }
		public bool Capture { get; set; }

		public ReoGridPos CellPosition{get;set;}

		public RGCellMouseEventArgs(ReoGridControl grid, ReoGridPos cellPosition)
			: this(grid, null, cellPosition, Point.Empty, MouseButtons.None, 0)
		{
		}

		public RGCellMouseEventArgs(ReoGridControl grid, ReoGridPos cellPosition, Point cursorPos, MouseButtons buttons, int clicks)
			: this(grid, null, cellPosition, cursorPos, buttons, clicks)
		{
		}

		public RGCellMouseEventArgs(ReoGridControl grid, ReoGridCell cell)
			: this(grid, cell, cell.Pos, Point.Empty, MouseButtons.None, 0)
		{
		}

		public RGCellMouseEventArgs(ReoGridControl grid, ReoGridCell cell, Point cursorPos, MouseButtons buttons, int clicks)
			: this(grid, cell, cell == null ? ReoGridPos.Empty : cell.Pos, cursorPos, buttons, clicks)
		{
		}

		public RGCellMouseEventArgs(ReoGridControl grid, ReoGridCell cell, ReoGridPos cellPosition,
			Point cursorPos, MouseButtons buttons, int clicks)
		{
			this.Grid = grid;
			this.Cell = cell;
			this.CursorPosition = cursorPos;
			this.Buttons = buttons;
			this.Clicks = clicks;
			this.CellPosition = cellPosition;
		}

		internal Point PointToControl(Point p)
		{
			return Point.Round(Grid.ViewportController.PointToControl(Grid.ViewportController.ActiveView, p));
		}

		internal Point CellPositionToControl(ReoGridPos pos)
		{
			return Grid.ViewportController.CellPositionToControl(pos);
		}
	}

	/// <summary>
	/// Event raised on action was performed for range
	/// </summary>
	public class RGRangeEventArgs : EventArgs
	{
		/// <summary>
		/// Range of action performed
		/// </summary>
		public ReoGridRange Range { get; set; }

		/// <summary>
		/// Create instance for RangeEventArgs with specified range.
		/// </summary>
		/// <param name="range">Range of action performed</param>
		public RGRangeEventArgs(ReoGridRange range)
		{
			this.Range = range;
		}
	}

	/// <summary>
	/// Event raised after cell editing. Set 'NewData' property to a
	/// new value to change the data instead of edited value.
	/// </summary>
	public class RGCellAfterEditEventArgs : RGCellEventArgs
	{
		/// <summary>
		/// Set the data to new value instead of edited value.
		/// </summary>
		public object NewData { get; set; }

		/// <summary>
		/// Reason of edit operation ending. Set this property to restore 
		/// the data to the value of before editing.
		/// </summary>
		public ReoGridEndEditReason EndReason { get; set; }

		/// <summary>
		/// When new data has been inputed, ReoGrid choose one formatter to 
		/// try to format the data. Set this property to force change the 
		/// formatter for the new data.
		/// </summary>
		public CellDataFormatFlag? DataFormat { get; set; }

		/// <summary>
		/// Create instance for CellAfterEditEventArgs
		/// </summary>
		/// <param name="cell">Cell edited by user</param>
		public RGCellAfterEditEventArgs(ReoGridCell cell) : base(cell) { }
	}

	/// <summary>
	/// Event raised before cell editing. Set 'IsCancelled' property to
	/// force stop current edit operation.
	/// </summary>
	public class RGCellBeforeEditEventArgs : RGCellEventArgs
	{
		/// <summary>
		/// Edit operation whether should be aborted.
		/// </summary>
		public bool IsCancelled { get; set; }

		/// <summary>
		/// Candidate string list. This list will be displayed on popuped
		/// window that allows user to choose item instead of inputing.
		/// (Reserved)
		/// </summary>
		public string[] CandidateItems { get; set; }

		/// <summary>
		/// Create instance for CellBeforeEditEventArgs with specified cell.
		/// </summary>
		/// <param name="cell">Cell edited by user</param>
		public RGCellBeforeEditEventArgs(ReoGridCell cell) : base(cell) { }
	}

	/// <summary>
	/// Event raised on action performed on row indexer.
	/// </summary>
	public class RGRowEventArgs : EventArgs
	{
		/// <summary>
		/// Row index number
		/// </summary>
		public int Row { get; set; }

		/// <summary>
		/// Create instance for RowEventArgs with specified row index number.
		/// </summary>
		/// <param name="row">Row index number</param>
		public RGRowEventArgs(int row) { this.Row = row; }
	}

	/// <summary>
	/// Event raised on action performed on row indexer.
	/// </summary>
	public class RGRowDeletedEventArgs : EventArgs
	{
		/// <summary>
		/// Row index number
		/// </summary>
		public int Row { get; set; }

		public int Count { get; set; }

		/// <summary>
		/// Create instance for RowEventArgs with specified row index number.
		/// </summary>
		/// <param name="row">Row index number</param>
		public RGRowDeletedEventArgs(int row, int count) { this.Row = row; this.Count = count; }
	}

	/// <summary>
	/// Event raised on action performed on column header.
	/// </summary>
	public class RGColumnEventArgs : EventArgs
	{
		/// <summary>
		/// Column index number
		/// </summary>
		public int Column { get; set; }

		/// <summary>
		/// Create instead for ColumnEventArgs with specified column header number.
		/// </summary>
		/// <param name="col">Column index number</param>
		public RGColumnEventArgs(int col) { this.Column = col; }
	}

	public class RGColumnDeletedEventArgs : EventArgs
	{
		public int Col { get; set; }
		public int Count { get; set; }

		public RGColumnDeletedEventArgs(int col, int count)
		{
			this.Col = col;
			this.Count = count;
		}
	}

	/// <summary>
	/// Event raised on border added to a range.
	/// </summary>
	public class RGBorderAddedEventArgs : RGRangeEventArgs
	{
		/// <summary>
		/// Position of border added.
		/// </summary>
		public ReoGridBorderPos Pos { get; set; }

		/// <summary>
		/// Style of border added.
		/// </summary>
		public ReoGridBorderStyle Style { get; set; }

		/// <summary>
		/// Create instance for BorderAddedEventArgs with specified range, 
		/// position of border and style of border.
		/// </summary>
		/// <param name="range"></param>
		/// <param name="pos"></param>
		/// <param name="style"></param>
		public RGBorderAddedEventArgs(ReoGridRange range, ReoGridBorderPos pos, ReoGridBorderStyle style)
			: base(range)
		{
			this.Pos = pos;
			this.Style = style;
		}
	}

	/// <summary>
	/// Event raised on border removed from a range.
	/// </summary>
	public class RGBorderRemovedEventArgs : RGRangeEventArgs
	{
		/// <summary>
		/// Position of border removed
		/// </summary>
		public ReoGridBorderPos Pos { get; set; }

		/// <summary>
		/// Create instance for BorderRemovedEventArgs with specified range and
		/// position of border.
		/// </summary>
		/// <param name="range"></param>
		/// <param name="pos"></param>
		public RGBorderRemovedEventArgs(ReoGridRange range, ReoGridBorderPos pos)
			: base(range)
		{
			this.Pos = pos;
		}
	}

	/// <summary>
	/// Event raised on grid loaded from a stream.
	/// </summary>
	public class RGFileLoadedEventArgs : EventArgs
	{
		/// <summary>
		/// Full path of file. Available only grid was loaded from a file stream.
		/// </summary>
		public string Filename { get; set; }

		/// <summary>
		/// Create instance for FileSavedEventArgs with specified file path.
		/// </summary>
		/// <param name="filename">Full path of file</param>
		public RGFileLoadedEventArgs(string filename) { this.Filename = filename; }
	}

	/// <summary>
	/// Event raised on grid saved to a stream.
	/// </summary>
	public class RGFileSavedEventArgs : EventArgs
	{
		/// <summary>
		/// Full path of file. Available only grid be saved into a file stream.
		/// </summary>
		public string Filename { get; set; }

		/// <summary>
		/// Create instance for FileSavedEventArgs with specified file path.
		/// </summary>
		/// <param name="filename">Full path of file</param>
		public RGFileSavedEventArgs(string filename) { this.Filename = filename; }
	}

	/// <summary>
	/// Event raised when selection will move to another position. 
	/// ReoGrid automatically move the selection to 'right' or 'down' according
	/// to 'SelectionForwardDirection' property of control. 
	/// Modify 'TargetPos' of this object to change the target position which 
	/// used to put the next selection of focus.
	/// </summary>
	public class RGSelectionMoveForwardEventArgs : EventArgs
	{
		///// <summary>
		///// Target position used to put next selection of focus.
		///// </summary>
		//public ReoGridPos TargetPos { get; set; }

		/// <summary>
		/// Decide whether to cancel current operation
		/// </summary>
		public bool IsCancelled { get; set; }

		/// <summary>
		/// Create instance for SelectionMoveForwardEventArgs with specified 
		/// position.
		/// </summary>
		/// <param name="targetPos"></param>
		public RGSelectionMoveForwardEventArgs(/*ReoGridPos targetPos*/)
		{
			/*this.TargetPos = targetPos;*/
		}
	}

	/// <summary>
	/// Event raised when operation to be performed to range, this class has
	/// the property 'IsCancelled' it used to notify grid control to abort
	/// current operation.
	/// </summary>
	public class RGBeforeRangeOperationEventArgs : RGRangeEventArgs
	{
		/// <summary>
		/// Get or set the flag that be used to notify the grid 
		/// whether to abort current operation
		/// </summary>
		public bool IsCancelled { get; set; }

		/// <summary>
		/// Construct instance with specified range
		/// </summary>
		/// <param name="range"></param>
		public RGBeforeRangeOperationEventArgs(ReoGridRange range) : base(range) { }
	}

	/// <summary>
	/// Event raised when control's settings has been changed
	/// </summary>
	public class SettingsChangedEventArgs : EventArgs
	{
		/// <summary>
		/// The setting flags what have been added
		/// </summary>
		public ReoGridSettings AddedSettings { get; set; }

		/// <summary>
		/// The setting flags what have been removed
		/// </summary>
		public ReoGridSettings RemovedSettings { get; set; }
	}
}
