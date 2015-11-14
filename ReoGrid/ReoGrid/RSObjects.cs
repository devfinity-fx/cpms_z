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

using unvell.Common;
using unvell.ReoScript;
using unvell.ReoGrid.XML;
using System.Windows.Forms;

namespace unvell.ReoGrid.Script
{
	#region Base Object
	internal abstract class RSBaseObject : ObjectValue
	{
		public ReoGridControl Grid { get; set; }

		public RSBaseObject(ReoGridControl grid)
		{
			this.Grid = grid;
		}
	}
	#endregion

	#region Grid
	internal class RSGridObject : RSBaseObject
	{
		private RSRangeObject Selection { get; set; }

		public RSGridObject(ReoGridControl grid)
			: base(grid)
		{
			#region Attributes
			this["readonly"] = new ExternalProperty(
				() => { return grid.HasSettings(ReoGridSettings.Readonly); },
				(v) => { grid.SetSettings(ReoGridSettings.Readonly, (v as bool?) ?? false); });
			#endregion

			#region Selection
			Selection = new RSRangeObject(grid);

			this["selection"] = new ExternalProperty(() => Selection,
				(obj) => grid.SelectionRange = RSUtility.GetRangeFromValue(grid, obj));

			this["selectRange"] = new NativeFunctionObject("selectRange", (srm, owner, args) =>
			{
				ReoGridRange range = RSUtility.GetRangeFromArgs(grid, args);
				if (range.IsEmpty) return false;

				try
				{
					grid.SelectRange(range);
					return true;
				}
				catch (Exception)
				{
					return false;
				}
			});

			this["selectionMode"] = new ExternalProperty(() => null);
			#endregion

			#region Rows & Cols
			this["rows"] = new ExternalProperty(() => { return grid.RowCount; }, (v) => { grid.SetRows(ScriptRunningMachine.GetIntValue(v)); });
			this["cols"] = new ExternalProperty(() => { return grid.ColCount; }, (v) => { grid.SetCols(ScriptRunningMachine.GetIntValue(v)); });
			this["getRow"] = new NativeFunctionObject("getRow", (srm, owner, args) =>
			{
				return args.Length == 0 ? null : new RSRowObject(grid, ScriptRunningMachine.GetIntValue(args[0]));
			});
			this["getCol"] = new NativeFunctionObject("getCol", (srm, owner, args) =>
			{
				return args.Length == 0 ? null : new RSColumnObject(grid, ScriptRunningMachine.GetIntValue(args[0]));
			});
			#endregion

			#region Cell & Style
			this["setRangeStyle"] = new NativeFunctionObject("setRangeStyle", (ctx, owner, args) =>
			{
				if (args.Length < 1) return false;

				ReoGridRange range = RSUtility.GetRangeFromValue(grid, args[0]);
				ReoGridRangeStyle styleObj = RSUtility.GetRangeStyleObject(args[0]);

				grid.SetRangeStyle(range, styleObj);
				
				return styleObj;
			});

			this["getCell"] = new NativeFunctionObject("getCell", (srm, owner, args) =>
			{
				if (args.Length < 1) return null;

				ReoGridPos pos = RSUtility.GetPosFromValue(args);

				return new RSCellObject(grid, grid.CreateAndGetCell(pos));
			});

			#endregion

			#region Range
			this["mergeRange"] = new NativeFunctionObject("mergeRange", (srm, owner, args) =>
			{
				ReoGridRange range = RSUtility.GetRangeFromArgs(grid, args);
				if (range.IsEmpty) return false;

				try
				{
					grid.MergeRange(range);
					return true;
				}
				catch (Exception)
				{
					return false;
				}
			});

			this["unmergeRange"] = new NativeFunctionObject("unmergeRange", (srm, owner, args) =>
			{
				ReoGridRange range = RSUtility.GetRangeFromArgs(grid, args);
				if (range.IsEmpty) return false;

				try
				{
					grid.UnmergeRange(range);
					return true;
				}
				catch (Exception)
				{
					return false;
				}
			});
			#endregion

			this["reset"] = new NativeFunctionObject("reset", (ctx, owner, args) =>
			{
				if (args.Length == 2)
				{
					grid.Reset(ScriptRunningMachine.GetIntParam(args, 0, 1),
						ScriptRunningMachine.GetIntParam(args, 1, 1));
				}
				else
				{
					grid.Reset();
				}
				return null;
			});
		}
	}
	#endregion

	#region Range
	internal class RSRangeObject : RSBaseObject
	{
		public ReoGridRange Range {get;set;}
		public ReoGridRangeStyle Style { get; set; }

		public RSRangeObject(ReoGridControl grid)
			: base(grid)
		{
			this["style"] = new ExternalProperty(
				() =>
				{
					if (Style == null)
					{
						Style = grid.GetRangeStyle(this.Range);
					}
					return Style;
				},
				(v) =>
				{
					grid.SetRangeStyle(this.Range, RSUtility.GetRangeStyleObject(v));
				}
			);

			this["pos"] = new ExternalProperty(
				() => grid.SelectionRange.StartPos,
				(v) =>
				{
					ReoGridRange range = grid.SelectionRange;
					range.StartPos = RSUtility.GetPosFromValue(v);
					grid.SelectRange(range);
				});

			this["range"] = new ExternalProperty(
				() => grid.SelectionRange,
				(v) =>
				{
					grid.SelectRange(RSUtility.GetRangeFromValue(grid, v));
				});
		}
	}
	#endregion

	#region Selection
	internal class RSSelectionObject : RSBaseObject
	{
		public RSSelectionObject(ReoGridControl grid)
			: base(grid)
		{
			this["moveUp"] = new NativeFunctionObject("moveUp", (srm, owner, args) =>
			{
				grid.MoveSelectionUp(); return null;
			});
			this["moveDown"] = new NativeFunctionObject("moveUp", (srm, owner, args) =>
			{
				grid.MoveSelectionDown(); return null;
			});
			this["moveLeft"] = new NativeFunctionObject("moveUp", (srm, owner, args) =>
			{
				grid.MoveSelectionLeft(); return null;
			});
			this["moveRight"] = new NativeFunctionObject("moveUp", (srm, owner, args) =>
			{
				grid.MoveSelectionRight(); return null;
			});

			this["pos"] = new ExternalProperty(
				() => grid.SelectionRange.StartPos,
				(v) =>
				{
					ReoGridRange range = grid.SelectionRange;
					range.StartPos = RSUtility.GetPosFromValue(v);
					grid.SelectRange(range);
				});

			this["range"] = new ExternalProperty(
				() => grid.SelectionRange,
				(v) =>
				{
					grid.SelectRange(RSUtility.GetRangeFromValue(grid, v));
				});
		}
	}
	#endregion

	#region Row & Column & Cell
	internal class RSRowObject : RSBaseObject
	{
		public RSCellStyleObject Style { get; set; }

		public RSRowObject(ReoGridControl grid, int row)
			: base(grid)
		{
			this["height"] = new ExternalProperty(
				() => { return grid.GetRowHeight(row); },
				(v) => { grid.SetRowsHeight(row, 1, (ushort)ScriptRunningMachine.GetIntValue(v)); }
			);
		}
	}

	internal class RSColumnObject : RSBaseObject
	{
		public RSColumnObject(ReoGridControl grid, int col)
			: base(grid)
		{
			this["width"] = new ExternalProperty(
				() => grid.GetColumnWidth(col),
				(v) => grid.SetColsWidth(col, 1, (ushort)ScriptRunningMachine.GetIntValue(v))
			);
		}
	}

	internal class RSCellObject : RSBaseObject
	{
		//public ReoGridPos Pos { get; set; }
		public ReoGridCell Cell { get; set; }
		public RSCellStyleObject Style { get; set; }

		public RSCellObject(ReoGridControl grid, ReoGridCell cell)
			: base(grid)
		{
			this.Cell = cell;

			this["style"] = new ExternalProperty(() =>
			{
				if (this.Style == null)
				{
					this.Style = new RSCellStyleObject(grid, cell);
				}
				return this.Style;
			}, null);

			this["data"] = new ExternalProperty(
				() => cell.Data,
				(v) => { grid.SetCellData(cell, v); });

			this["formula"] = new ExternalProperty(
				() => cell.Formula,
				(v) => { grid.SetCellData(cell, v); });

			// name changed from 'display' to 'text'
			this["text"] = new ExternalProperty(
				() => cell.Display,
				(v) => { cell.Display = ScriptRunningMachine.ConvertToString(v); });

			this["pos"] = new ExternalProperty(() => cell.Pos);
			this["row"] = new ExternalProperty(() => cell.Row);
			this["col"] = new ExternalProperty(() => cell.Col);
		}

		public override string Name
		{
			get { return string.Format("cell[{0},{1}]", Cell.Pos.Row, Cell.Pos.Col); }
		}
	}
	#endregion

	#region Cell Style
	internal class RSCellStyleObject : RSBaseObject
	{
		public ReoGridCell Cell { get; set; }

		public RSCellStyleObject(ReoGridControl grid, ReoGridCell cell)
			: base(grid)
		{
			this.Cell = cell;

			this["backgroundColor"] = new ExternalProperty(
				() => TextFormatHelper.EncodeColor(cell.Style.BackColor),
				(v) =>
				{
					grid.SetCellStyle(cell.Pos, new ReoGridRangeStyle
					{
						Flag = PlainStyleFlag.FillColor,
						BackColor = TextFormatHelper.DecodeColor(ScriptRunningMachine.ConvertToString(v)),
					});
				});

			this["color"] = new ExternalProperty(
				() => TextFormatHelper.EncodeColor(cell.Style.TextColor),
				(v) =>
				{
					grid.SetCellStyle(cell.Pos, new ReoGridRangeStyle
					{
						Flag = PlainStyleFlag.TextColor,
						TextColor = TextFormatHelper.DecodeColor(ScriptRunningMachine.ConvertToString(v)),
					});
				});

			this["font-size"] = new ExternalProperty(
				() => cell.Style.FontSize,
				(v) =>
				{
					grid.SetCellStyle(cell.Pos, new ReoGridRangeStyle
					{
						Flag = PlainStyleFlag.FontSize,
						FontSize = TextFormatHelper.GetFloatPixelValue(ScriptRunningMachine.ConvertToString(v),
							System.Drawing.SystemFonts.DefaultFont.Size),
					});
				});

			this["align"] = new ExternalProperty(
				() => cell.Style.HAlign,
				(v) =>
				{
					grid.SetCellStyle(cell.Pos, new ReoGridRangeStyle
					{
						Flag = PlainStyleFlag.HorizontalAlign,
						HAlign = XmlFileFormatHelper.DecodeHorizontalAlign(ScriptRunningMachine.ConvertToString(v)),
					});
				});

			this["valign"] = new ExternalProperty(
				() => cell.Style.VAlign,
				(v) =>
				{
					grid.SetCellStyle(cell.Pos, new ReoGridRangeStyle
					{
						Flag = PlainStyleFlag.VerticalAlign,
						VAlign = XmlFileFormatHelper.DecodeVerticalAlign(ScriptRunningMachine.ConvertToString(v)),
					});
				});

			//this["border"] = new ExternalProperty(
			//	() =>
			//	{
			//		ReoGridCellStyle style = grid.GetCellStyle(Pos);
			//		return style.FontSize + "px";
			//	},
			//	(v) =>
			//	{
			//		grid.SetCellStyle(Pos, new ReoGridCellStyle
			//		{
			//			Flag = PlainStyleFlag.FontSize,
			//			FontSize = XmlFileFormatHelper.GetFloatPixelValue(Convert.ToString(v), System.Drawing.SystemFonts.DefaultFont.Size),
			//		});
			//	});
		}

		public override string Name
		{
			get { return "CellStyle"; }
		}
	}
	#endregion

	#region Event Args
	internal class RSKeyEvent : ObjectValue
	{
		public KeyEventArgs EventArgs { get; set; }

		public RSKeyEvent(KeyEventArgs eventArgs)
		{
			this.EventArgs = eventArgs;

			this["keyCode"] = new ExternalProperty(() => (int)eventArgs.KeyCode);
		}
	}
	#endregion

	#region Script Utility
	public class RSUtility
	{
		#region Range
		public static ReoGridRange GetRangeFromValue(ReoGridControl grid, object arg)
		{
			if (arg is RSRangeObject)
			{
				return grid.SelectionRange;
			}

			ObjectValue obj = arg as ObjectValue;
			if (obj == null) return ReoGridRange.Empty;

			ReoGridRange range = ReoGridRange.Empty;

			range.Row = ScriptRunningMachine.GetIntValue(obj["row"]);
			range.Col = ScriptRunningMachine.GetIntValue(obj["col"]);
			range.Rows = ScriptRunningMachine.GetIntValue(obj["rows"]);
			range.Cols = ScriptRunningMachine.GetIntValue(obj["cols"]);

			return range;
		}

		public static ReoGridRange GetRangeFromArgs(ReoGridControl grid, object[] args)
		{
			if (args.Length == 0) return ReoGridRange.Empty;

			if (args.Length == 1)
			{
				return GetRangeFromValue(grid, args[0]);
			}
			else
			{
				ReoGridRange range = ReoGridRange.Empty;

				range.Row = Convert.ToInt32(args[0]);
				if (args.Length > 1) range.Col = ScriptRunningMachine.GetIntValue(args[1]);
				if (args.Length > 2) range.Rows = ScriptRunningMachine.GetIntValue(args[2]);
				if (args.Length > 3) range.Cols = ScriptRunningMachine.GetIntValue(args[3]);

				return range;
			}
		}
		#endregion

		#region Pos
		public static ObjectValue CreatePosObject(ReoGridPos pos)
		{
			return CreatePosObject(pos.Row, pos.Col);
		}
		public static ObjectValue CreatePosObject(int row, int col)
		{
			ObjectValue obj = new ObjectValue();
			
			obj["row"] = row;
			obj["col"] = col;

			return obj;
		}

		public static ReoGridPos GetPosFromValue(object arg)
		{
			if (arg is object[])
			{
				object[] arr = (object[])arg;
				if (arr.Length == 0)
					return ReoGridPos.Empty;

				else if (arr.Length == 1)
				{
					if (arr[0] is ReoGridPos)
					{
						return (ReoGridPos)arr[0];
					}
					else if (arr[0] is string)
					{
						return new ReoGridPos(ScriptRunningMachine.ConvertToString(arr[0]));
					}
					else if (arr[0] is ObjectValue)
					{
						var obj = (ObjectValue)arr[0];

						return new ReoGridPos(ScriptRunningMachine.GetIntValue(obj["row"]),
						ScriptRunningMachine.GetIntValue(obj["col"]));
					}
				}
				else if (arr.Length == 2)
				{
					return new ReoGridPos(ScriptRunningMachine.GetIntValue(arr[0], 0),
					ScriptRunningMachine.GetIntValue(arr[1], 0));
				}
			}
			else if (arg is ObjectValue)
			{
				var obj = (ObjectValue)arg;

				return new ReoGridPos(ScriptRunningMachine.GetIntValue(obj["row"]),
				ScriptRunningMachine.GetIntValue(obj["col"]));
			}

			return ReoGridPos.Empty;
		}

		public static ReoGridPos GetPosFromArgs(object[] args)
		{
			if (args.Length == 0) return ReoGridPos.Empty;

			ReoGridPos pos = ReoGridPos.Empty;

			if (args.Length == 1)
			{
				ObjectValue obj = args[0] as ObjectValue;
				if (obj == null) return ReoGridPos.Empty;

				pos.Row = ScriptRunningMachine.GetIntValue(obj["row"]);
				pos.Col = ScriptRunningMachine.GetIntValue(obj["col"]);
			}
			else
			{
				pos.Row = Convert.ToInt32(args[0]);
				if (args.Length > 1) pos.Col = Convert.ToInt32(args[1]);
			}

			return pos;
		}
		#endregion

		public static T DecodeSelectionMode<T>(object obj)
		{
			return (T)Enum.Parse(typeof(T), Convert.ToString(obj));
		}

		public static string EncodeSelectionMode(object enm)
		{
			string str = Convert.ToString(enm);
			if (!string.IsNullOrEmpty(str))
			{
				str = char.ToLower(str[0]).ToString() + str.Substring(1);
			}
			return str;
		}

		internal static ReoGridRangeStyle GetRangeStyleObject(object p)
		{
			if (p is ReoGridRangeStyle)
			{
				return (ReoGridRangeStyle)p;
			}
			else if (p is ObjectValue)
			{
				var obj = (ObjectValue)p;

				ReoGridRangeStyle style = new ReoGridRangeStyle();

				object backColor = obj["backgroundColor"];
				if (backColor != null)
				{
					style.Flag |= PlainStyleFlag.FillColor;
					style.BackColor = TextFormatHelper.DecodeColor(ScriptRunningMachine.ConvertToString(backColor));
				}
				
				return style;
			}
			else
				return null;
		}
	}
	#endregion
}

#endif // EX_SCRIPT