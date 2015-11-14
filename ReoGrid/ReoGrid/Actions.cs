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
using System.Linq;
using System.Text;

using unvell.Common;
using unvell.ReoGrid.DataFormat;

namespace unvell.ReoGrid.Actions
{
	// Base Actions & ActionGroups
	#region SGActions
	/// <summary>
	/// Base action for all actions that used in ReoGrid Control.
	/// </summary>
	public abstract class RGAction : IUndoableAction
	{
		private ReoGridControl grid;

		/// <summary>
		/// Instance for the grid control will be setted before action performed.
		/// </summary>
		internal ReoGridControl Grid
		{
			get { return grid; }
			set { grid = value; }
		}

		#region IUndoableAction Members
		/// <summary>
		/// Undo this action
		/// </summary>
		public abstract void Undo();
		/// <summary>
		/// Do this action
		/// </summary>
		public abstract void Do();
		/// <summary>
		/// Get friendly name of this action
		/// </summary>
		/// <returns></returns>
		public abstract string GetName();
		#endregion
	}

	/// <summary>
	/// The action group is one type of RGAction to support Do/Undo/Redo a series of actions.
	/// </summary>
	public class RGActionGroup : RGAction
	{
		/// <summary>
		/// Actions stored in this list will be Do/Undo/Redo together
		/// </summary>
		public List<RGAction> Actions { get; set; }

		/// <summary>
		/// Create instance for RGActionGroup
		/// </summary>
		public RGActionGroup()
		{
			this.Actions = new List<RGAction>();
		}

		/// <summary>
		/// Do all actions stored in this action group
		/// </summary>
		public override void Do()
		{
			foreach (var action in Actions)
			{
				action.Grid = this.Grid;
				action.Do();
			}
		}

		/// <summary>
		/// Undo all actions stored in this action group
		/// </summary>
		public override void Undo()
		{
			for (int i = Actions.Count - 1; i >= 0; i--)
			{
				var action = Actions[i];

				action.Grid = this.Grid;
				action.Undo();
			}
		}

		/// <summary>
		/// Get friendly name of this action group
		/// </summary>
		/// <returns></returns>
		public override string GetName()
		{
			return "ReoGrid Action Group";
		}
	}

	/// <summary>
	/// Reusable action is one type of RGAction to support repeat operation
	/// to a specified range. It is good practice to make all actions with 
	/// a range target to inherit from this class.
	/// </summary>
	public abstract class RGReusableAction : RGAction
	{
		/// <summary>
		/// Range to be appiled this action
		/// </summary>
		public ReoGridRange Range { get; set; }

		/// <summary>
		/// Constructor of RGReusableAction 
		/// </summary>
		/// <param name="range">Range to be applied this action</param>
		public RGReusableAction(ReoGridRange range)
		{
			this.Range = range;
		}

		internal abstract RGReusableAction Clone(ReoGridRange range);
	}

	/// <summary>
	/// Reusable action group is one type of RGActionGroup to support repeat 
	/// operation to a specified range. It is good practice to make all reusable 
	/// action groups to inherit from this class.
	/// </summary>
	public class RGReusableActionGroup : RGReusableAction
	{
		private List<RGReusableAction> actions;

		/// <summary>
		/// All reusable actions stored in this list will be performed together.
		/// </summary>
		public List<RGReusableAction> Actions
		{
			get { return actions; }
			set { actions = value; }
		}

		/// <summary>
		/// Constructor of ReusableActionGroup
		/// </summary>
		/// <param name="range">Range to be appiled this action group</param>
		public RGReusableActionGroup(ReoGridRange range)
			: base(range)
		{
			this.actions = new List<RGReusableAction>();
		}

		/// <summary>
		/// Constructor of ReusableActionGroup
		/// </summary>
		/// <param name="range">Range to be appiled this action group</param>
		/// <param name="actions">Action list to be performed together</param>
		public RGReusableActionGroup(ReoGridRange range, List<RGReusableAction> actions)
			: base(range)
		{
			this.actions = actions;
		}

		private bool first = true;

		/// <summary>
		/// Do all actions stored in this action group
		/// </summary>
		public override void Do()
		{
			if (first)
			{
				for (int i = 0; i < actions.Count; i++)
					actions[i].Grid = this.Grid;
				first = false;
			}

			for (int i = 0; i < actions.Count; i++)
			{
				actions[i].Do();
			}
		}

		/// <summary>
		/// Undo all actions stored in this action group
		/// </summary>
		public override void Undo()
		{
			for (int i = actions.Count - 1; i >= 0; i--)
			{
				actions[i].Undo();
			}
		}

		/// <summary>
		/// Get friendly name of this action group
		/// </summary>
		/// <returns></returns>
		public override string GetName()
		{
			return "Multi-Aciton[" + actions.Count + "]";
		}

		/// <summary>
		/// Create cloned reusable action group from this action group
		/// </summary>
		/// <param name="range">Specified new range to apply this action group</param>
		/// <returns>New reusable action group cloned from this action group</returns>
		internal override RGReusableAction Clone(ReoGridRange range)
		{
			List<RGReusableAction> clonedActions = new List<RGReusableAction>();

			foreach (RGReusableAction action in actions)
			{
				clonedActions.Add(action.Clone(range));
			}

			return new RGReusableActionGroup(range, clonedActions);
		}
	}

	internal class RGActionException : ActionException
	{
		public RGActionException(RGAction action, string msg) : base(action, msg) { }
	}
	internal class RGOtherGridsActionException : RGActionException
	{
		public RGOtherGridsActionException(RGAction action)
			: base(action, "Aciton is belong to another control.") { }
	}
	#endregion // SGActions

	// Style Editing Actions
	#region Actions - Style

	/// <summary>
	/// Set style to specified range action
	/// </summary>
	public class RGSetRangeStyleAction : RGReusableAction
	{
		private ReoGridRangeStyle style;

		/// <summary>
		/// Style to be set
		/// </summary>
		public ReoGridRangeStyle Style { get { return style; } set { style = value; } }

		private ReoGridRangeStyle backupRootStyle = null;
		private ReoGridRangeStyle[] rowStyles = null;
		private ReoGridRangeStyle[] colStyles = null;
		private PartialGrid backupData = null;
		private bool isFullColSelected = false;
		private bool isFullRowSelected = false;
		private bool isFullGridSelected = false;

		/// <summary>
		/// Create an action that perform set styles to specified range
		/// </summary>
		/// <param name="row">number of row</param>
		/// <param name="col">number of col</param>
		/// <param name="rows">number of rows</param>
		/// <param name="cols">number of cols</param>
		/// <param name="style">style to be set</param>
		public RGSetRangeStyleAction(int row, int col, int rows, int cols, ReoGridRangeStyle style)
			: this(new ReoGridRange(row, col, rows, cols), style)
		{
		}

		/// <summary>
		/// Create an action that perform set styles to specified range
		/// </summary>
		/// <param name="range">Range to be appiled this action</param>
		/// <param name="style">Style to be set to specified range</param>
		public RGSetRangeStyleAction(ReoGridRange range, ReoGridRangeStyle style)
			: base(range)
		{
			this.style = new ReoGridRangeStyle(style);
		}

		/// <summary>
		/// Do this action
		/// </summary>
		public override void Do()
		{
			backupData = Grid.GetPartialGrid(Range);

			ReoGridRange range = Range;

			int r1 = Range.Row;
			int c1 = Range.Col;
			int r2 = Range.Row2;
			int c2 = Range.Col2;

			int rowCount = Grid.RowCount;
			int colCount = Grid.ColCount;

			isFullColSelected = range.Rows == rowCount;
			isFullRowSelected = range.Cols == colCount;
			isFullGridSelected = isFullRowSelected && isFullColSelected;

			// update default styles
			if (isFullGridSelected)
			{
				backupRootStyle = ReoGridRangeStyle.Clone(Grid.RootStyle);

				rowStyles = new ReoGridRangeStyle[rowCount];
				colStyles = new ReoGridRangeStyle[colCount];

				// remote styles if it is already setted in full-row
				for (int r = 0; r < rowCount; r++)
				{
					ReoGridRowHead rowHead = Grid.RetrieveRowHead(r);
					if (rowHead != null && rowHead.Style != null)
					{
						rowStyles[r] = ReoGridRangeStyle.Clone(rowHead.Style);
					}
				}

				// remote styles if it is already setted in full-col
				for (int c = 0; c < colCount; c++)
				{
					ReoGridColHead colHead = Grid.RetrieveColHead(c);
					if (colHead != null && colHead.Style != null)
					{
						colStyles[c] = ReoGridRangeStyle.Clone(colHead.Style);
					}
				}
			}
			else if (isFullRowSelected)
			{
				rowStyles = new ReoGridRangeStyle[r2 - r1 + 1];
				for (int r = r1; r <= r2; r++)
				{
					rowStyles[r - r1] = ReoGridRangeStyle.Clone(Grid.RetrieveRowHead(r).Style);
				}
			}
			else if (isFullColSelected)
			{
				colStyles = new ReoGridRangeStyle[c2 - c1 + 1];
				for (int c = c1; c <= c2; c++)
				{
					colStyles[c - c1] = ReoGridRangeStyle.Clone(Grid.RetrieveColHead(c).Style);
				}
			}

			Grid.SetRangeStyle(range, style);
		}

		/// <summary>
		/// Undo this action
		/// </summary>
		public override void Undo()
		{
			ReoGridRange range = Range;

			if (isFullGridSelected)
			{
				Grid.RootStyle = ReoGridRangeStyle.Clone(backupRootStyle);

				// remote styles if it is already setted in full-row
				for (int r = 0; r < rowStyles.Length; r++)
				{
					if (rowStyles[r] != null)
					{
						ReoGridRowHead rowHead = Grid.RetrieveRowHead(r);
						if (rowHead != null)
						{
							rowHead.Style = ReoGridRangeStyle.Clone(rowStyles[r]);
							rowHead.Style.Flag = PlainStyleFlag.None;
							//rowHead.Style.BackColor = System.Drawing.Color.Empty;
						}
					}
				}

				// remote styles if it is already setted in full-col
				for (int c = 0; c < colStyles.Length; c++)
				{
					if (colStyles[c] != null)
					{
						ReoGridColHead colHead = Grid.RetrieveColHead(c);
						if (colHead != null)
						{
							colHead.Style = ReoGridRangeStyle.Clone(colStyles[c]);
							colHead.Style.Flag = PlainStyleFlag.None;
							//colHead.Style.BackColor = System.Drawing.Color.Empty;
						}
					}
				}
			}
			else if (isFullRowSelected)
			{
				for (int r = 0; r < rowStyles.Length; r++)
				{
					ReoGridRowHead rowHead = Grid.RetrieveRowHead(r + range.Row);
					if (rowHead != null) rowHead.Style = rowStyles[r];
				}
			}
			else if (isFullColSelected)
			{
				for (int c = 0; c < colStyles.Length; c++)
				{
					ReoGridColHead colHead = Grid.RetrieveColHead(c + range.Col);
					if (colHead != null) colHead.Style = colStyles[c];
				}
			}

			Grid.SetPartialGrid(range, backupData, PartialGridCopyFlag.CellStyle);
		}

		/// <summary>
		/// Get friendly name of this action
		/// </summary>
		/// <returns></returns>
		public override string GetName()
		{
			return "Set Style";
		}

		/// <summary>
		/// Create an action copy with different range from this reusable action
		/// </summary>
		/// <param name="range">Specified new range to appiled created action</param>
		/// <returns>Action copy created from this action</returns>
		internal override RGReusableAction Clone(ReoGridRange range)
		{
			return new RGSetRangeStyleAction(range, style);
		}
	}

	/// <summary>
	/// Remove style from specified range action
	/// </summary>
	public class RGRemoveRangeStyleAction : RGReusableAction
	{
		private PlainStyleFlag flag;

		/// <summary>
		/// Style flag indicates what type of style to be handled.
		/// </summary>
		public PlainStyleFlag Flag { get { return flag; } set { flag = value; } }

		private PartialGrid backupData;

		/// <summary>
		/// Create instance for action to remove style from specified range.
		/// </summary>
		/// <param name="range">Styles from this specified range to be removed</param>
		/// <param name="flag">Style flag indicates what type of style should be removed</param>
		public RGRemoveRangeStyleAction(ReoGridRange range, PlainStyleFlag flag)
			: base(range)
		{
			this.flag = flag;
		}

		/// <summary>
		/// Do this action
		/// </summary>
		public override void Do()
		{
			backupData = Grid.GetPartialGrid(Range);
			Grid.RemoveRangeStyle(Range, flag);
		}

		/// <summary>
		/// Undo this action
		/// </summary>
		public override void Undo()
		{
			Grid.SetPartialGrid(Range, backupData, PartialGridCopyFlag.CellStyle);
		}

		/// <summary>
		/// Get friendly name of this action
		/// </summary>
		/// <returns></returns>
		public override string GetName()
		{
			return "Delete Style";
		}

		internal override RGReusableAction Clone(ReoGridRange range)
		{
			return new RGRemoveRangeStyleAction(Range, flag);
		}
	}

	/// <summary>
	/// Make font size larger or smaller action.
	/// </summary>
	public class RGStepRangeFontSizeAction : RGReusableAction
	{
		/// <summary>
		/// True if this action making font size larger.
		/// </summary>
		public bool Enlarge { get; set; }

		/// <summary>
		/// Create instance for this action with specified range and enlarge flag.
		/// </summary>
		/// <param name="range">Specified range to apply this action</param>
		/// <param name="enlarge">True to set text larger, false to set smaller</param>
		public RGStepRangeFontSizeAction(ReoGridRange range, bool enlarge)
			: base(range)
		{
			this.Enlarge = enlarge;
		}

		/// <summary>
		/// Do this action
		/// </summary>
		public override void Do()
		{
			Grid.StepRangeFont(Range, size =>
			{
				return Enlarge ?
						(size >= FontToolkit.FontSizeList.Max()) ? size : FontToolkit.FontSizeList.Where(f => f > size).Min()
						: (size <= FontToolkit.FontSizeList.Min()) ? size : FontToolkit.FontSizeList.Where(f => f < size).Max();
			});
		}

		/// <summary>
		/// Undo this action
		/// </summary>
		public override void Undo()
		{
			Grid.StepRangeFont(Range, size =>
			{
				return !Enlarge ?
						(size >= FontToolkit.FontSizeList.Max()) ? size : FontToolkit.FontSizeList.Where(f => f > size).Min()
						: (size <= FontToolkit.FontSizeList.Min()) ? size : FontToolkit.FontSizeList.Where(f => f < size).Max();
			});
		}

		/// <summary>
		/// Get friendly name of this action
		/// </summary>
		/// <returns></returns>
		public override string GetName()
		{
			return Enlarge ? "Make Text Bigger" : "Make Text Smaller";
		}

		internal override RGReusableAction Clone(ReoGridRange range)
		{
			return new RGStepRangeFontSizeAction(range, Enlarge);
		}
	}
	#endregion // Actions - Style

	// Headers of Row and Column Editing Actions
	#region Actions - Row & Column Header
	/// <summary>
	/// Insert rows action
	/// </summary>
	public class RGInsertRowsAction : RGReusableAction
	{
		/// <summary>
		/// Index of row to insert empty rows. Set to Control.RowCount to 
		/// append columns at end of rows.
		/// </summary>
		public int Row { get; set; }

		/// <summary>
		/// Number of empty rows to be inserted
		/// </summary>
		public int Count { get; set; }

		/// <summary>
		/// Create instance for InsertRowsAction
		/// </summary>
		/// <param name="row">Index of row to insert</param>
		/// <param name="count">Number of rows to be inserted</param>
		public RGInsertRowsAction(int row, int count)
			: base(ReoGridRange.Empty)
		{
			this.Row = row;
			this.Count = count;
		}

		int insertedRow = -1;

		/// <summary>
		/// Do this action
		/// </summary>
		public override void Do()
		{
			insertedRow = Row;
			Grid.InsertRows(Row, Count);
			Range = new ReoGridRange(Row, 0, Count, Grid.ColCount);
		}

		/// <summary>
		/// Undo this action
		/// </summary>
		public override void Undo()
		{
			Grid.DeleteRows(insertedRow, Count);
		}

		/// <summary>
		/// Get friendly name of this action
		/// </summary>
		/// <returns></returns>
		public override string GetName()
		{
			return "Insert Rows";
		}

		internal override RGReusableAction Clone(ReoGridRange range)
		{
			return new RGInsertRowsAction(range.Row, range.Rows);
		}
	}

	/// <summary>
	/// Insert columns action
	/// </summary>
	public class RGInsertColumnsAction : RGReusableAction
	{
		/// <summary>
		/// Index of column to insert new columns. Set to Control.ColCount to
		/// append columns at end of columns.
		/// </summary>
		public int Col { get; set; }

		/// <summary>
		/// Number of columns to be inserted
		/// </summary>
		public int Count { get; set; }

		/// <summary>
		/// Create instance for InsertColumnsAction
		/// </summary>
		/// <param name="col">Index of column to insert</param>
		/// <param name="count">Number of columns to be insertted</param>
		public RGInsertColumnsAction(int col, int count)
			: base(ReoGridRange.Empty)
		{
			this.Col = col;
			this.Count = count;
		}

		int insertedCol = -1;

		/// <summary>
		/// Do this action
		/// </summary>
		public override void Do()
		{
			insertedCol = Col;
			Grid.InsertCols(Col, Count);
			Range = new ReoGridRange(0, Col, Grid.RowCount, Count);
		}

		/// <summary>
		/// Undo this action
		/// </summary>
		public override void Undo()
		{
			Grid.DeleteCols(insertedCol, Count);
		}

		/// <summary>
		/// Get friendly name of this action
		/// </summary>
		/// <returns></returns>
		public override string GetName()
		{
			return "Insert Columns";
		}

		internal override RGReusableAction Clone(ReoGridRange range)
		{
			return new RGInsertColumnsAction(range.Col, range.Cols);
		}
	}

	/// <summary>
	/// Remove rows actions
	/// </summary>
	public class RGRemoveRowsAction : RGReusableAction
	{
		private PartialGrid backupData = null;

		/// <summary>
		/// Create instance for RemoveRowsAction
		/// </summary>
		/// <param name="row">Index of row start to remove</param>
		/// <param name="rows">Number of rows to be removed</param>
		public RGRemoveRowsAction(int row, int rows)
			: base(new ReoGridRange(row, 0, rows, -1))
		{ }

		private int[] backupHeights;

		/// <summary>
		/// Do this action
		/// </summary>
		public override void Do()
		{
			if (Range.Rows == -1)
			{
				Logger.Log("remove rows", "attmpt to remove all columns but grid must have one column, operation aborted.");
				return;
			}

			backupHeights = new int[Range.Rows];

			for (int i = Range.Row; i <= Range.Row2; i++)
			{
				backupHeights[i - Range.Row] = Grid.RetrieveRowHead(i).Height;
			}

			Range = Grid.FixRange(Range);
			backupData = Grid.GetPartialGrid(Range);
			Debug.Assert(backupData != null);
			Grid.DeleteRows(Range.Row, Range.Rows);
		}

		/// <summary>
		/// Undo this action
		/// </summary>
		public override void Undo()
		{
			if (Range.Rows == -1)
			{
				Logger.Log("remove columns", "attmpt to undo removing all columns but grid must have one column, operation aborted.");
				return;
			}
			
			Grid.InsertRows(Range.Row, Range.Rows);

			if (backupData == null)
			{
#if DEBUG
				Logger.Log("remove rows", "no backup data");
				Debug.Assert(false, "why no backup data here?");
#else
				return;
#endif
			}

			Grid.SetPartialGrid(Range, backupData);
		}

		/// <summary>
		/// Get friendly name of this action
		/// </summary>
		/// <returns></returns>
		public override string GetName()
		{
			return "Remove Rows";
		}

		internal override RGReusableAction Clone(ReoGridRange range)
		{
			return new RGRemoveRowsAction(range.Row, range.Rows);
		}
	}

	/// <summary>
	/// Remove columns action
	/// </summary>
	public class RGRemoveColumnsAction : RGReusableAction
	{
		private PartialGrid backupData = null;

		/// <summary>
		/// Create instance for RemoveColumnsAction
		/// </summary>
		/// <param name="col">Index of column start to remove</param>
		/// <param name="count">Number of columns to be removed</param>
		public RGRemoveColumnsAction(int col, int count)
			: base(new ReoGridRange(0, col, -1, count))
		{ }

		private int[] backupWidths;

		/// <summary>
		/// Do this action
		/// </summary>
		public override void Do()
		{
			if (Range.Cols == -1)
			{
				Logger.Log("remove columns", "attmpt to remove all columns but grid must have one column, operation aborted.");
				return;
			}

			backupWidths = new int[Range.Cols];

			for (int i = Range.Col; i <= Range.Col2; i++)
			{
				backupWidths[i - Range.Col] = Grid.RetrieveColHead(i).Width;
			}

			backupData = Grid.GetPartialGrid(Range);
			Debug.Assert(backupData != null);
			Grid.DeleteCols(Range.Col, Range.Cols);
		}

		/// <summary>
		/// Undo this action
		/// </summary>
		public override void Undo()
		{
			if (Range.Cols == -1)
			{
				Logger.Log("remove columns", "attmpt to undo removing all columns but grid must have one column, operation aborted.");
				return;
			}
			
			Grid.InsertCols(Range.Col, Range.Cols);
			Grid.SetColsWidth(Range.Col, Range.Cols, col => backupWidths[col - Range.Col], true);

			if (backupData == null)
			{
#if DEBUG
				Logger.Log("remove rows", "no backup data");
				Debug.Assert(false, "why no backup data here?");
#else
				return;
#endif
			}

			Grid.SetPartialGrid(Range, backupData);
		}

		/// <summary>
		/// Get friendly name of this action
		/// </summary>
		/// <returns></returns>
		public override string GetName()
		{
			return "Remove Columns";
		}

		internal override RGReusableAction Clone(ReoGridRange range)
		{
			return new RGRemoveColumnsAction(range.Col, range.Cols);
		}
	}

	/// <summary>
	/// Set height of row action
	/// </summary>
	public class RGSetRowsHeightAction : RGReusableAction
	{
		private ushort height;

		/// <summary>
		/// Height to be set
		/// </summary>
		public ushort Height { get { return height; } set { height = value; } }

		/// <summary>
		/// Create instance for SetRowsHeightAction
		/// </summary>
		/// <param name="row">Index of row start to set</param>
		/// <param name="count">Number of rows to be set</param>
		/// <param name="height">New height to set to specified rows</param>
		public RGSetRowsHeightAction(int row, int count, ushort height)
			: base(new ReoGridRange(row, 0, count, -1))
		{
			this.height = height;
		}

		/// <summary>
		/// Do this action
		/// </summary>
		public override void Do()
		{
			int row = Range.Row;
			int count = Range.Rows;

			backupRows.Clear();

			int r2 = row + count;
			for (int r = row; r < r2; r++)
			{
				ReoGridRowHead rowHead = Grid.RetrieveRowHead(r);

				backupRows.Add(r, new RowHeadData
				{
					autoHeight = rowHead.IsAutoHeight,
					row = rowHead.Row,
					height = rowHead.Height,
				});

				// disable auto-height-adjusting if user has changed height of this row
				rowHead.IsAutoHeight = false;
			}

			Grid.SetRowsHeight(row, count, height);
		}

		/// <summary>
		/// Undo this action
		/// </summary>
		public override void Undo()
		{
			Grid.SetRowsHeight(Range.Row, Range.Rows, r => backupRows[r].height, true);

			foreach (int r in backupRows.Keys)
			{
				ReoGridRowHead rowHead = Grid.RetrieveRowHead(r);
				rowHead.IsAutoHeight = backupRows[r].autoHeight;
			}
		}

		/// <summary>
		/// Get friendly name of this action
		/// </summary>
		/// <returns></returns>
		public override string GetName()
		{
			return "Set Rows Height: " + height;
		}

		Dictionary<int, RowHeadData> backupRows = new Dictionary<int, RowHeadData>();

		internal struct RowHeadData
		{
			internal int row;
			internal ushort height;
			internal bool autoHeight;
		}

		internal override RGReusableAction Clone(ReoGridRange range)
		{
			return new RGSetRowsHeightAction(range.Row, range.Rows, height);
		}
	}

	/// <summary>
	/// Set width of column action
	/// </summary>
	public class RGSetColsWidthAction : RGReusableAction
	{
		private ushort width;

		/// <summary>
		/// Width to be set
		/// </summary>
		public ushort Width { get { return width; } set { width = value; } }

		/// <summary>
		/// Create instance for SetColsWidthAction
		/// </summary>
		/// <param name="col">Index of column start to set</param>
		/// <param name="count">Number of columns to be set</param>
		/// <param name="width">Width of column to be set</param>
		public RGSetColsWidthAction(int col, int count, ushort width)
			: base(new ReoGridRange(0, col, -1, count))
		{
			this.width = width;
		}

		/// <summary>
		/// Do this action
		/// </summary>
		public override void Do()
		{
			int col = base.Range.Col;
			int count = base.Range.Cols;

			backupCols.Clear();

			int c2 = col + count;
			for (int c = col; c < c2; c++)
			{
				ReoGridColHead colHead = Grid.RetrieveColHead(c);
				backupCols.Add(c, colHead.Width);
			}

			Grid.SetColsWidth(col, count, width);
		}

		private Dictionary<int, ushort> backupCols = new Dictionary<int, ushort>();

		/// <summary>
		/// Undo this action
		/// </summary>
		public override void Undo()
		{
			int col = base.Range.Col;
			int count = base.Range.Cols;

			Grid.SetColsWidth(col, count, c => backupCols[c], true);
		}

		/// <summary>
		/// Get friendly name of this action
		/// </summary>
		/// <returns></returns>
		public override string GetName()
		{
			return "Set Cols Width: " + width;
		}

		internal override RGReusableAction Clone(ReoGridRange range)
		{
			return new RGSetColsWidthAction(range.Col, range.Cols, width);
		}
	}
	#endregion // Actions - Row & Column Header

	// Border Editing Actions
	#region Actions - Borders
	/// <summary>
	/// Set borders to specified range action
	/// </summary>
	public class RGSetRangeBorderAction : RGReusableAction
	{
		private ReoGridBorderPosStyle[] borders;

		/// <summary>
		/// Borders to be set
		/// </summary>
		public ReoGridBorderPosStyle[] Borders
		{
			get { return borders; }
			set { borders = value; }
		}

		private PartialGrid backupData;

		/// <summary>
		/// Create action that perform setting border to a range
		/// </summary>
		/// <param name="range">Range to be appiled this action</param>
		/// <param name="pos">Position of range to set border</param>
		/// <param name="style">Style of border</param>
		public RGSetRangeBorderAction(ReoGridRange range, ReoGridBorderPos pos, ReoGridBorderStyle style)
			: this(range, new ReoGridBorderPosStyle[] { new ReoGridBorderPosStyle(pos, style) }) { }

		/// <summary>
		/// Create action that perform setting border to a range
		/// </summary>
		/// <param name="range">Range to be appiled this action</param>
		/// <param name="style">Style of border</param>
		public RGSetRangeBorderAction(ReoGridRange range, ReoGridBorderPosStyle[] styles)
			: base(range)
		{
			this.borders = styles;
		}

		/// <summary>
		/// Do this action
		/// </summary>
		public override void Do()
		{
			backupData = Grid.GetPartialGrid(Range, PartialGridCopyFlag.BorderAll,
				ExPartialGridCopyFlag.BorderOutsideOwner);

			for (int i = 0; i < borders.Length; i++)
			{
				Grid.SetRangeBorder(Range, borders[i].Pos, borders[i].Style);
			}
		}

		/// <summary>
		/// Undo this action
		/// </summary>
		public override void Undo()
		{
			Grid.SetPartialGrid(Range, backupData, PartialGridCopyFlag.BorderAll,
				ExPartialGridCopyFlag.BorderOutsideOwner);
		}

		/// <summary>
		/// Get friendly name of this action
		/// </summary>
		/// <returns></returns>
		public override string GetName()
		{
			return "Set Range Border";
		}

		internal override RGReusableAction Clone(ReoGridRange range)
		{
			return new RGSetRangeBorderAction(range, borders);
		}
	}

	/// <summary>
	/// Action of Removing borders from specified range
	/// </summary>
	public class RGRemoveRangeBorderAction : RGReusableAction
	{
		public ReoGridBorderPos BorderPos { get; set; }

		private PartialGrid backupData;

		/// <summary>
		/// Create instance for SetRangeBorderAction with specified range and border styles.
		/// </summary>
		/// <param name="range">Range to be appiled this action</param>
		/// <param name="pos">Position of range to set border</param>
		/// <param name="style">Style of border</param>
		public RGRemoveRangeBorderAction(ReoGridRange range, ReoGridBorderPos pos)
			: base(range)
		{
			this.BorderPos = pos;
		}

		/// <summary>
		/// Do this action
		/// </summary>
		public override void Do()
		{
			backupData = Grid.GetPartialGrid(Range, PartialGridCopyFlag.BorderAll,
				ExPartialGridCopyFlag.BorderOutsideOwner);

			Grid.RemoveRangeBorder(Range, BorderPos);
		}

		/// <summary>
		/// Undo this action
		/// </summary>
		public override void Undo()
		{
			Grid.SetPartialGrid(Range, backupData, PartialGridCopyFlag.BorderAll,
				ExPartialGridCopyFlag.BorderOutsideOwner);
		}

		/// <summary>
		/// Get friendly name of this action
		/// </summary>
		/// <returns></returns>
		public override string GetName()
		{
			return "Set Range Border";
		}

		internal override RGReusableAction Clone(ReoGridRange range)
		{
			return new RGRemoveRangeBorderAction(range, BorderPos);
		}
	}
	#endregion // Actions - Borders

	// Range/Cell Editing Actions
	#region Actions - Range & Cell Edit
	/// <summary>
	/// Merge range action
	/// </summary>
	public class RGMergeRangeAction : RGReusableAction
	{
		/// <summary>
		/// Create instance for MergeRangeAction with specified range
		/// </summary>
		/// <param name="range">The range to be merged</param>
		public RGMergeRangeAction(ReoGridRange range) : base(range) { }

		internal override RGReusableAction Clone(ReoGridRange range)
		{
			return new RGMergeRangeAction(range);
		}

		private PartialGrid backupData;

		/// <summary>
		/// Do this action
		/// </summary>
		public override void Do()
		{
			// todo
			backupData = Grid.GetPartialGrid(Range, PartialGridCopyFlag.All, ExPartialGridCopyFlag.None);
			Grid.MergeRange(Range);
		}

		/// <summary>
		/// Undo this action
		/// </summary>
		public override void Undo()
		{
			Grid.UnmergeRange(base.Range);
			Grid.SetPartialGrid(Range, backupData, PartialGridCopyFlag.All);
		}

		/// <summary>
		/// Get friendly name of this action
		/// </summary>
		/// <returns></returns>
		public override string GetName()
		{
			return "Merge Range";
		}
	}

	/// <summary>
	/// Unmerge range action
	/// </summary>
	public class RGUnmergeRangeAction : RGReusableAction
	{
		/// <summary>
		/// Create instance for UnmergeRangeAction with specified range
		/// </summary>
		/// <param name="range">The range to be unmerged</param>
		public RGUnmergeRangeAction(ReoGridRange range) : base(range) { }

		internal override RGReusableAction Clone(ReoGridRange range)
		{
			return new RGUnmergeRangeAction(range);
		}

		private PartialGrid backupData;

		/// <summary>
		/// Do this action
		/// </summary>
		public override void Do()
		{
			// todo
			backupData = Grid.GetPartialGrid(Range, PartialGridCopyFlag.All, ExPartialGridCopyFlag.None);
			Grid.UnmergeRange(Range);
		}

		/// <summary>
		/// Undo this action
		/// </summary>
		public override void Undo()
		{
			Grid.SetPartialGrid(Range, backupData, PartialGridCopyFlag.All);
		}

		/// <summary>
		/// Get friendly name of this action
		/// </summary>
		/// <returns></returns>
		public override string GetName()
		{
			return "Merge Range";
		}
	}

	/// <summary>
	/// Set data of cell action
	/// </summary>
	public class RGSetCellDataAction : RGAction
	{
		private int row;
		/// <summary>
		/// Index of row to set data
		/// </summary>
		public int Row { get { return row; } set { row = value; } }

		private int col;
		/// <summary>
		/// Index of column to set data
		/// </summary>
		public int Col { get { return col; } set { col = value; } }

		private bool isCellNull;

		private object data;

		/// <summary>
		/// Data of cell
		/// </summary>
		public object Data
		{
			get { return data; }
			set { data = value; }
		}

		private object dataBackup;
		private string displayBackup;
		private CellDataFormatFlag formatBackup;
		private CellDataFormatFlag? CellDataFormat { get; set; }

		/// <summary>
		/// Create SetCellValueAction with specified index of row and column
		/// </summary>
		/// <param name="row">Index of row to set data</param>
		/// <param name="col">Index of column to set data</param>
		/// <param name="data">Data to be set</param>
		public RGSetCellDataAction(int row, int col, object data)
		{
			this.row = row;
			this.col = col;
			this.data = data;
		}

		/// <summary>
		/// Do this action
		/// </summary>
		public override void Do()
		{
			isCellNull = Grid.GetCell(row, col) == null;

			ReoGridCell cell = Grid.CreateAndGetCell(row, col);
			if (cell != null)
			{
				dataBackup = cell.Data;
				displayBackup = cell.Display;
				formatBackup = cell.DataFormat;

				Grid.SetCellData(cell, data);
				Grid.SelectRange(new ReoGridRange(cell.Row, cell.Col, cell.Rowspan, cell.Colspan));

				isCellNull = false;
			}
			else
				isCellNull = true;
		}

		/// <summary>
		/// Undo this action
		/// </summary>
		public override void Undo()
		{
			if (isCellNull)
			{
				Grid.SetCellData(row, col, null);
			}
			else
			{
				ReoGridCell cell = Grid.GetCell(row, col);
				if (cell != null)
				{

#if EX_DATA_TRIGGER
						bool isChanged = cell.Data != dataBackup || cell.Display != displayBackup;
#endif

					cell.Data = dataBackup;
					cell.Display = displayBackup;

#if EX_DATA_TRIGGER
						if (isChanged)
						{
							Grid.RaiseCellDataChangedEvent(cell);
						}
#endif

#if EX_SCRIPT
					Grid.UpdateReferencedFormulaCells(cell);
#endif

					cell.DataFormat = formatBackup;
					Grid.SelectRange(new ReoGridRange(cell.Row, cell.Col, cell.Rowspan, cell.Colspan));
				}
			}
		}

		/// <summary>
		/// Get friendly name of this action
		/// </summary>
		/// <returns></returns>
		public override string GetName()
		{
			string str = data == null ? "null" : data.ToString();
			return "Set Value: " + (str.Length > 10 ? (str.Substring(0, 7) + "...") : str);
		}
	}

	/// <summary>
	/// Set range data format action
	/// </summary>
	public class RGSetRangeDataFormatAction : RGReusableAction
	{
		private CellDataFormatFlag format;
		private object formatArgs;
		private PartialGrid backupData = null;

		/// <summary>
		/// Create instance for SetRangeDataFormatAction
		/// </summary>
		/// <param name="range">Range to be appiled this action</param>
		/// <param name="format">Format type of cell to be set</param>
		/// <param name="dataFormatArgs">Argument belongs to format type to be set</param>
		public RGSetRangeDataFormatAction(ReoGridRange range, CellDataFormatFlag format,
			object dataFormatArgs)
			: base(range)
		{
			this.format = format;
			this.formatArgs = dataFormatArgs;
		}

		/// <summary>
		/// Do this action
		/// </summary>
		public override void Do()
		{
			backupData = Grid.GetPartialGrid(Range, PartialGridCopyFlag.CellData, ExPartialGridCopyFlag.None);
			Grid.SetRangeDataFormat(Range, format, formatArgs);
		}

		/// <summary>
		/// Undo this action
		/// </summary>
		public override void Undo()
		{
			Grid.SetPartialGrid(Range, backupData);
		}

		internal override RGReusableAction Clone(ReoGridRange range)
		{
			return new RGSetRangeDataFormatAction(range, this.format, formatArgs);
		}

		/// <summary>
		/// Get friendly name of this action
		/// </summary>
		/// <returns></returns>
		public override string GetName()
		{
			return "Set Cell Format: " + format.ToString();
		}
	}

	public class RGSetRangeDataAction : RGReusableAction
	{
		private object[,] data;
		private object[,] backupData;

		public RGSetRangeDataAction(ReoGridRange range, object[,] data)
			: base(range)
		{
			this.data = data;
		}

		internal override RGReusableAction Clone(ReoGridRange range)
		{
			return new RGSetRangeDataAction(range, data);
		}

		public override void Do()
		{
			backupData = Grid.GetRangeData(base.Range);
			Debug.Assert(backupData != null);
			base.Grid.SetRangeData(base.Range, data);
			Grid.SelectRange(base.Range);
		}

		public override void Undo()
		{
			Debug.Assert(backupData != null);
			base.Grid.SetRangeData(base.Range, backupData);
		}

		public override string GetName()
		{
			return "Set Partial Grid";
		}
	}

	#endregion

	// Partial Grid
	#region Actions - PartialGrid
	public class RGSetPartialGridAction : RGReusableAction
	{
		private PartialGrid data;
		private PartialGrid backupData;

		public RGSetPartialGridAction(ReoGridRange range, PartialGrid data)
			: base(range)
		{
			this.data = data;
		}

		internal override RGReusableAction Clone(ReoGridRange range)
		{
			return new RGSetPartialGridAction(range, data);
		}

		public override void Do()
		{
			backupData = Grid.GetPartialGrid(base.Range, PartialGridCopyFlag.All, ExPartialGridCopyFlag.BorderOutsideOwner);
			Debug.Assert(backupData != null);
			base.Range = base.Grid.SetPartialGridRepeatly(data, base.Range);
			Grid.SelectRange(base.Range);
		}

		public override void Undo()
		{
			Debug.Assert(backupData != null);
			base.Grid.SetPartialGrid(base.Range, backupData, PartialGridCopyFlag.All, ExPartialGridCopyFlag.BorderOutsideOwner);
		}

		public override string GetName()
		{
			return "Set Partial Grid";
		}
	}
	#endregion
}
