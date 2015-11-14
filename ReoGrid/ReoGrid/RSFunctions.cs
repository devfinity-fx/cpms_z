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

#if EX_SCRIPT

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using unvell.ReoScript;

namespace unvell.ReoGrid.Script
{
	static class RSFunctions
	{
		private static readonly Regex AutoCellReferenceRegex=  new Regex(
			"(?:\"[^\"]*\\s*\")|(?:(?<from_col>[A-Z]+)(?<from_row>[0-9]+):(?<to_col>[A-Z]+)(?<to_row>[0-9]+))|(?:(?<from_col>[A-Z]+)(?<from_row>[0-9]+))");
		//public static string PreprocessCellReference(ReoGridControl grid, string str, List<ReferenceRange> referencedCells)
		//{
		//	return RangeReferenceRegex.Replace(str, (m) =>
		//		{
		//			if (m.Groups["to_col"].Length > 0 && m.Groups["to_row"].Length > 0
		//				&& m.Groups["from_col"].Length > 0 && m.Groups["from_row"].Length > 0)
		//			{
		//				// range
		//				int fromRow = ReoGridControl.GetAlphaNumber(m.Groups["from_row"].Value);
		//				int fromCol = ReoGridControl.GetAlphaNumber(m.Groups["from_col"].Value);
		//				int toRow = ReoGridControl.GetAlphaNumber(m.Groups["to_row"].Value);
		//				int toCol = ReoGridControl.GetAlphaNumber(m.Groups["to_col"].Value);

		//				if (fromRow >= 0 && fromRow < grid.RowCount - 1
		//					&& toRow >= 0 && toRow < grid.RowCount - 1
		//					&& fromCol >= 0 && fromCol < grid.ColCount - 1
		//					&& toCol >= 0 && toCol < grid.ColCount - 1)
		//				{
		//					ReoGridRange range = new ReoGridRange(fromRow, fromCol, toRow - fromRow + 1, toCol - fromCol + 1);
		//					if(range.Contains(
		//					referencedCells.Add(new ReferenceRange(grid.GetCell(range.Row, range.Col), grid.GetCell(range.Row2, range.Col2)));
		//					return string.Format("new Range({0},{1},{2},{3})", range.Row, range.Col, range.Row2, range.Col2);
		//				}
		//				else
		//					return m.Value;
		//			}
		//			//else if (m.Groups["to_col"].Length <= 0 || m.Groups["to_row"].Length <= 0)
		//			//{
		//			//	int fromRow = ReoGridControl.GetAlphaNumber(m.Groups["from_row"].Value);
		//			//	int fromCol = ReoGridControl.GetAlphaNumber(m.Groups["from_col"].Value);

		//			//	// cell
		//			//	if (fromRow >= 0 && fromRow < grid.RowCount - 1
		//			//		&& fromCol >= 0 && fromCol < grid.ColCount - 1)
		//			//	{
		//			//		referencedCells.Add(grid.GetCell(fromRow, fromCol));
		//			//		return string.Format("grid.getCell({0},{1}).data", fromRow, fromCol);
		//			//	}
		//			//	else
		//			//		return m.Value;
		//			//}
		//			//else
		//				return m.Value;
		//		});
		//}

		static RSFunctions()
		{
		}

		public static double Sum(ReoGridControl ctrl, ReoGridRange range)
		{
			double val = 0;

			ctrl.IterateCells(range, (row, col, cell) =>
			{
				if (cell != null && ScriptRunningMachine.IsNumber(cell.Data))
				{
					val += ScriptRunningMachine.GetDoubleValue(cell.Data);
				}
				return true;
			});

			return val;
		}

		public static double Avg(ReoGridControl ctrl, ReoGridRange range)
		{
			double val = 0;
			int count = 0;

			ctrl.IterateCells(range, (row, col, cell) =>
			{
				if (cell != null && ScriptRunningMachine.IsNumber(cell.Data))
				{
					val += ScriptRunningMachine.GetDoubleValue(cell.Data);
					count++;
				}
				return true;
			});

			return val / count;
		}

		public static double Count(ReoGridControl ctrl, ReoGridRange range)
		{
			int count = 0;

			ctrl.IterateCells(range, (row, col, cell) =>
			{
				count++;
				return true;
			});

			return count;
		}

		internal static void SetupBuiltinFunctions(ReoGridControl grid, ScriptRunningMachine srm)
		{
			srm["sum"] = new NativeFunctionObject("sum", (ctx, owner, args) => Sum(grid, RSUtility.GetRangeFromArgs(grid, args)));
			srm["SUM"] = new NativeFunctionObject("SUM", (ctx, owner, args) => Sum(grid, RSUtility.GetRangeFromArgs(grid, args)));
			srm["avg"] = new NativeFunctionObject("avg", (ctx, owner, args) => Avg(grid, RSUtility.GetRangeFromArgs(grid, args)));
			srm["AVG"] = new NativeFunctionObject("AVG", (ctx, owner, args) => Avg(grid, RSUtility.GetRangeFromArgs(grid, args)));
			srm["count"] = new NativeFunctionObject("count", (ctx, owner, args) => Count(grid, RSUtility.GetRangeFromArgs(grid, args)));
			srm["COUNT"] = new NativeFunctionObject("COUNT", (ctx, owner, args) => Count(grid, RSUtility.GetRangeFromArgs(grid, args)));

		}
	}
}

#endif