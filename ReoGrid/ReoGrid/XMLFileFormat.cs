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
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Diagnostics;
using System.Reflection;

using unvell.Common;
using unvell.ReoGrid.DataFormat;

namespace unvell.ReoGrid
{
	// Classes in this namespace used to persistence the grid control.
	// Data of cells, Styles and Borders of range, Script and Macro,
	// all the information belong to ReoGrid control will be able to 
	// convert into XML.
	namespace XML
	{
		internal sealed class XmlFileFormatHelper
		{
			#region Alignement
			internal static string EncodeHorizontalAlign(ReoGridHorAlign halign)
			{
				switch (halign)
				{
					default:
					case ReoGridHorAlign.General:
						return "general";
					case ReoGridHorAlign.Left:
						return "left";
					case ReoGridHorAlign.Center:
						return "center";
					case ReoGridHorAlign.Right:
						return "right";
					case ReoGridHorAlign.DistributedIndent:
						return "distributed-indent";
				}
			}
			internal static string EncodeVerticalAlign(ReoGridVerAlign valign)
			{
				switch (valign)
				{
					case ReoGridVerAlign.Top:
						return "top";
					default:
					case ReoGridVerAlign.Middle:
						return "middle";
					case ReoGridVerAlign.Bottom:
						return "bottom";
				}
			}

			internal static ReoGridHorAlign DecodeHorizontalAlign(string align)
			{
				switch (align)
				{
					default:
					case "general":
						return ReoGridHorAlign.General;
					case "left":
						return ReoGridHorAlign.Left;
					case "center":
						return ReoGridHorAlign.Center;
					case "right":
						return ReoGridHorAlign.Right;
					case "distributed-indent":
						return ReoGridHorAlign.DistributedIndent;
				}
			}
			internal static ReoGridVerAlign DecodeVerticalAlign(string valign)
			{
				switch (valign)
				{
					case "top":
						return ReoGridVerAlign.Top;
					default:
					case "middle":
						return ReoGridVerAlign.Middle;
					case "bottom":
						return ReoGridVerAlign.Bottom;
				}
			}
			#endregion

			#region Data Format
			internal static string EncodeCellDataFormat(CellDataFormatFlag format)
			{
				switch (format)
				{
					default:
					case CellDataFormatFlag.General:
						return null;
					case CellDataFormatFlag.Number:
						return "number";
					case CellDataFormatFlag.Text:
						return "text";
					case CellDataFormatFlag.DateTime:
						return "datetime";
					case CellDataFormatFlag.Percent:
						return "percent";
					case CellDataFormatFlag.Currency:
						return "currency";
				}
			}
			internal static CellDataFormatFlag DecodeCellDataFormat(string format)
			{
				if (format == null) return CellDataFormatFlag.General;

				switch (format.ToLower())
				{
					default:
						return CellDataFormatFlag.General;
					case "number":
						return CellDataFormatFlag.Number;
					case "text":
						return CellDataFormatFlag.Text;
					case "datetime":
						return CellDataFormatFlag.DateTime;
					case "percent":
						return CellDataFormatFlag.Percent;
					case "currency":
						return CellDataFormatFlag.Currency;
				}
			}

			internal static string EncodeNegativeNumberStyle(NumberDataFormatter.NumberNegativeStyle numberNegativeStyle)
			{
				if (numberNegativeStyle == NumberDataFormatter.NumberNegativeStyle.Minus) return null;

				StringBuilder sb = new StringBuilder(30);

				if ((numberNegativeStyle & NumberDataFormatter.NumberNegativeStyle.Red) == NumberDataFormatter.NumberNegativeStyle.Red)
					sb.Append("red ");
				if ((numberNegativeStyle & NumberDataFormatter.NumberNegativeStyle.Brackets) == NumberDataFormatter.NumberNegativeStyle.Brackets)
					sb.Append("brackets ");

				if (sb[sb.Length - 1] == ' ') sb.Length--;

				return sb.ToString();
			}
			internal static NumberDataFormatter.NumberNegativeStyle DecodeNegativeNumberStyle(string p)
			{
				NumberDataFormatter.NumberNegativeStyle flag = NumberDataFormatter.NumberNegativeStyle.Minus;

				if (string.IsNullOrEmpty(p)) return flag;

				string[] tokens = p.Split(' ');
				foreach (string token in tokens)
				{
					if (token == "red") flag |= NumberDataFormatter.NumberNegativeStyle.Red;
					else if (token == "brackets") flag |= NumberDataFormatter.NumberNegativeStyle.Brackets;
				}

				return flag;
			}
			#endregion

			#region Border
			internal static string EncodeBorderPos(ReoGridBorderPos pos)
			{
				return pos.ToString().ToLower();
			}
			internal static object DecodeBorderPos(string p)
			{
				if (string.IsNullOrEmpty(p)) return ReoGridBorderPos.None;
				// TODO: need convert first char upper 
				return (ReoGridBorderPos)Enum.Parse(typeof(ReoGridBorderPos), p);
			}

			internal static string EncodeHBorderOwnerPos(HBorderOwnerPosition pos)
			{
				return pos.ToString().ToLower();
			}
			internal static HBorderOwnerPosition DecodeHBorderOwnerPos(string p)
			{
				if (string.IsNullOrEmpty(p)) return HBorderOwnerPosition.None;
				if (p.Equals("all", StringComparison.CurrentCultureIgnoreCase))
					return HBorderOwnerPosition.All;
				else if (p.Equals("top", StringComparison.CurrentCultureIgnoreCase))
					return HBorderOwnerPosition.Top;
				else if (p.Equals("bottom", StringComparison.CurrentCultureIgnoreCase))
					return HBorderOwnerPosition.Bottom;
				else
					return HBorderOwnerPosition.None;
			}

			internal static string EncodeVBorderOwnerPos(VBorderOwnerPosition pos)
			{
				return pos.ToString().ToLower();
			}
			internal static VBorderOwnerPosition DecodeVBorderOwnerPos(string p)
			{
				if (string.IsNullOrEmpty(p)) return VBorderOwnerPosition.None;
				if (p.Equals("all", StringComparison.CurrentCultureIgnoreCase))
					return VBorderOwnerPosition.All;
				else if (p.Equals("left", StringComparison.CurrentCultureIgnoreCase))
					return VBorderOwnerPosition.Left;
				else if (p.Equals("right", StringComparison.CurrentCultureIgnoreCase))
					return VBorderOwnerPosition.Right;
				else
					return VBorderOwnerPosition.None;
			}
			#endregion

			#region TextWrap
			internal static string EncodeTextWrapMode(TextWrapMode wrapMode)
			{
				switch (wrapMode)
				{
					default:
					case TextWrapMode.NoWrap:
						return "no-wrap";
					case TextWrapMode.WordBreak:
						return "word-break";
					case TextWrapMode.BreakAll:
						return "break-all";
				}
			}
			internal static TextWrapMode DecodeTextWrapMode(string p)
			{
				if (string.IsNullOrEmpty(p)) return TextWrapMode.NoWrap;

				if (p.Equals("word-break", StringComparison.CurrentCultureIgnoreCase))
					return TextWrapMode.WordBreak;
				else if (p.Equals("break-all", StringComparison.CurrentCultureIgnoreCase))
					return TextWrapMode.BreakAll;
				else
					return TextWrapMode.NoWrap;
			}
			#endregion
		}

		#region XML Serialization

		[XmlRoot("grid")]
		public class RGXmlBody
		{
			[XmlElement("head")]
			public RGXmlHead head;

			[XmlElement("style")]
			public RGXmlCellStyle style;

			[XmlArray("rows"), XmlArrayItem("row")]
			public List<RGXmlRowHead> rows = new List<RGXmlRowHead>();
			[XmlArray("cols"), XmlArrayItem("col")]
			public List<RGXmlColHead> cols = new List<RGXmlColHead>();

			[XmlArray("v-borders"), XmlArrayItem("v-border")]
			public List<RGXmlVBorder> vborder = new List<RGXmlVBorder>();
			[XmlArray("h-borders"), XmlArrayItem("h-border")]
			public List<RGXmlHBorder> hborder = new List<RGXmlHBorder>();

			[XmlArray("cells"), XmlArrayItem("cell")]
			public List<RGXmlCell> cells = new List<RGXmlCell>();
		}

		public class RGXmlHead
		{
			[XmlElement("default-row-height")]
			public ushort defaultRowHeight;
			[XmlElement("default-col-width")]
			public ushort defaultColumnWidth;

			[XmlElement("rows")]
			public int rows;
			[XmlElement("cols")]
			public int cols;

			[XmlElement("freeze-row")]
			public int freezeRow;
			[XmlElement("freeze-col")]
			public int freezeCol;

			[XmlElement("freeze-top")]
			public string freezeTop;
			[XmlElement("freeze-left")]
			public string freezeLeft;

			[XmlElement("outlines")]
			public RGXmlOutlineList outlines;

			[XmlElement("settings")]
			public RGXmlGridSetting settings;

			[XmlElement("culture")]
			public string culture;

			[XmlElement("editor")]
			public string editor;

			[XmlElement("script")]
			public RGXmlScript script;
		}

		public class RGXmlMeta
		{
			[XmlElement("core-ver")]
			public string version;
		}

		public class RGXmlGridSetting
		{
			[XmlAttribute("show-grid")]
			public bool showGrid;
		}

		public class RGXmlRowHead
		{
			[XmlAttribute]
			public int row;
			[XmlAttribute]
			public ushort height;
			[XmlAttribute]
			public ushort lastHeight;
			[XmlAttribute("auto-height")]
			public string autoHeight;
			[XmlElement("style")]
			public RGXmlCellStyle style;
		}

		public class RGXmlColHead
		{
			[XmlAttribute]
			public int col;
			[XmlAttribute]
			public ushort width;
			[XmlAttribute]
			public ushort lastWidth;
			[XmlAttribute("auto-width")]
			public string autoWidth;
			[XmlElement("style")]
			public RGXmlCellStyle style;
		}

		public class RGXmlVBorder : RGXmlBorder
		{
			[XmlAttribute]
			public int rows;

			public RGXmlVBorder() { }

			internal RGXmlVBorder(int row, int col, int rows, ReoGridBorderStyle borderStyle, VBorderOwnerPosition pos)
				: base(row, col, borderStyle, XmlFileFormatHelper.EncodeVBorderOwnerPos(pos)) { this.rows = rows; }
		}
		public class RGXmlHBorder : RGXmlBorder
		{
			[XmlAttribute]
			public int cols;

			public RGXmlHBorder() { }

			internal RGXmlHBorder(int row, int col, int cols, ReoGridBorderStyle borderStyle, HBorderOwnerPosition pos)
				: base(row, col, borderStyle, XmlFileFormatHelper.EncodeHBorderOwnerPos(pos)) { this.cols = cols; }
		}

		public class RGXmlBorder
		{
			[XmlAttribute]
			public int row;
			[XmlAttribute]
			public int col;

			[XmlAttribute]
			public string color;
			[XmlAttribute]
			public string style;

			[XmlAttribute]
			public string pos;

			public RGXmlBorder() { }

			internal RGXmlBorder(int row, int col, ReoGridBorderStyle borderStyle, string pos)
			{
				this.row = row;
				this.col = col;
				this.pos = pos;

				color = TextFormatHelper.EncodeColor(borderStyle.Color);
				style = borderStyle.Style.ToString();
			}

			[XmlIgnore]
			internal ReoGridBorderStyle StyleGridBorder
			{
				get
				{
					BorderLineStyle borderStyle;

					if (style.Equals("dot"))									// for Backward-compatibility
						borderStyle = BorderLineStyle.Dotted;
					else if (style.Equals("dash"))						// for Backward-compatibility
						borderStyle = BorderLineStyle.Dashed;
					else
						borderStyle = (BorderLineStyle)Enum.Parse(typeof(BorderLineStyle), style, true);

					return new ReoGridBorderStyle
					{
						Color = TextFormatHelper.DecodeColor(color),
						Style = borderStyle,
					};
				}
			}
		}

		public class RGXmlCell
		{
			[XmlAttribute]
			public int row;
			[XmlAttribute]
			public int col;
			[XmlAttribute]
			public string colspan;
			[XmlAttribute]
			public string rowspan;

			[XmlText]
			public string data;

			[XmlAttribute("format")]
			public string dataFormat;
			[XmlElement("format-args")]
			public RGXmlCellDataFormatArgs dataFormatArgs;

			[XmlElement("style")]
			public RGXmlCellStyle style;

		}

		public class RGXmlCellStyle
		{
			[XmlAttribute("bgcolor")]
			public string backColor;
			[XmlAttribute("color")]
			public string textColor;
			[XmlAttribute("font")]
			public string font;
			[XmlAttribute("font-size")]
			public string fontSize;
			[XmlAttribute("bold")]
			public string bold;
			[XmlAttribute("italic")]
			public string italic;
			[XmlAttribute("strikethrough")]
			public string strikethrough;
			[XmlAttribute("underline")]
			public string underline;
			[XmlAttribute("align")]
			public string hAlign;
			[XmlAttribute("valign")]
			public string vAlign;
			[XmlAttribute("text-wrap")]
			public string textWrap;
			[XmlAttribute("padding")]
			public string padding;

			[XmlElement("fill-pattern")]
			public RGXmlCellStyleFillPattern fillPattern;
		}

		public class RGXmlCellStyleFillPattern
		{
			[XmlAttribute("color")]
			public string color;
			[XmlAttribute("style")]
			public int patternStyleId;
		}

		public class RGXmlCellDataFormatArgs
		{
			[XmlAttribute("decimal-places")]
			public string decimalPlaces;
			[XmlAttribute("use-separator")]
			public string useSeparator;
			[XmlAttribute("pattern")]
			public string pattern;
			[XmlAttribute("culture")]
			public string culture;
			[XmlAttribute("negative-style")]
			public string negativeStyle;

			public bool IsEmpty
			{
				get
				{
					return string.IsNullOrEmpty(decimalPlaces)
						&& string.IsNullOrEmpty(useSeparator)
						&& string.IsNullOrEmpty(pattern)
						&& string.IsNullOrEmpty(culture)
						&& string.IsNullOrEmpty(negativeStyle);
				}
			}
		}

		public class RGXmlOutlineList
		{
			[XmlElement("row-outline")]
			public List<RGXmlOutline> rowOutlines;
			[XmlElement("col-outline")]
			public List<RGXmlOutline> colOutlines;
		}

		public class RGXmlOutline
		{
			[XmlAttribute("start")]
			public int start;
			[XmlAttribute("count")]
			public int count;
			[XmlAttribute("collapsed")]
			public bool collapsed;
		}

		public class RGXmlScript
		{
			[XmlText]
			public string content;
		}

		#endregion // XML
	}
}