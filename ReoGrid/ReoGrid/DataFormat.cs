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
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;

namespace unvell.ReoGrid.DataFormat
{
	#region Defines
	/// <summary>
	/// Cell data format type
	/// </summary>
	public enum CellDataFormatFlag
	{
		/// <summary>
		/// Auto format type (compliant with Text and Number)
		/// </summary>
		General,

		/// <summary>
		/// Number Type
		/// </summary>
		Number,

		/// <summary>
		/// Date and Time Type
		/// </summary>
		DateTime,

		/// <summary>
		/// Percent Type
		/// </summary>
		Percent,

		/// <summary>
		/// Currency Type
		/// </summary>
		Currency,

		/// <summary>
		/// String
		/// </summary>
		Text,
	}

	/// <summary>
	/// Data Formatter Manager
	/// </summary>
	public sealed class DataFormatterManager
	{
		private static DataFormatterManager instance;

		/// <summary>
		/// Instance for this class
		/// </summary>
		public static DataFormatterManager Instance
		{
			get
			{
				if (instance == null) instance = new DataFormatterManager();
				return instance;
			}
		}

		private Dictionary<CellDataFormatFlag, IDataFormatter> dataFormatters = new Dictionary<CellDataFormatFlag, IDataFormatter>();

		/// <summary>
		/// Built-in data formatters
		/// </summary>
		public Dictionary<CellDataFormatFlag, IDataFormatter> DataFormatters
		{
			get { return dataFormatters; }
			set { dataFormatters = value; }
		}

		private DataFormatterManager()
		{
			// add data formatter by this order to decide format detecting priority
			// by default General Data Formatter is first formatter
			dataFormatters.Add(CellDataFormatFlag.General, new GeneralDataFormatter());
			dataFormatters.Add(CellDataFormatFlag.Number, new NumberDataFormatter());
			dataFormatters.Add(CellDataFormatFlag.DateTime, new DateTimeDataFormatter());
			dataFormatters.Add(CellDataFormatFlag.Percent, new PercentDataFormatter());
			dataFormatters.Add(CellDataFormatFlag.Currency, new CurrencyDataFormatter());
			dataFormatters.Add(CellDataFormatFlag.Text, new TextDataFormatter());
		}

		internal void FormatCell(ReoGridControl grid, ReoGridCell cell)
		{
			// clear cell render color
			// render color used for draw a negative number
			cell.RenderColor = Color.Empty;

			if (cell.DataFormat == CellDataFormatFlag.General)
			{
				bool found = false;
				foreach (CellDataFormatFlag flag in dataFormatters.Keys)
				{
					var formatter = dataFormatters[flag];

					if (formatter.PerformTestFormat()
						&& dataFormatters[flag].FormatCell(grid, cell))
					{
						cell.DataFormat = flag;
						found = true;
						break;
					}
				}

				if (!found)
				{
					cell.Display = Convert.ToString(cell.Data);

					// if horizontal-align is auto self-adapt, 
					// set the render alignment to left for string type
					if (cell.Style.HAlign == ReoGridHorAlign.General)
					{
						cell.RenderHorAlign = ReoGridRenderHorAlign.Left;
					}
				}

			}
			else
			{
				bool success = DataFormatters[cell.DataFormat].FormatCell(grid, cell);

				if (!success)
				{
					DataFormatters[CellDataFormatFlag.Text].FormatCell(grid, cell);
				}
			}
		}
	}

	[Serializable]
	internal sealed class DataFormatAttribute : Attribute
	{
		private CellDataFormatFlag formatFlag;
		public DataFormatAttribute(CellDataFormatFlag formatFlag)
		{
			this.formatFlag = formatFlag;
		}

		public bool PerformTypeTest { get; set; }
	}

	/// <summary>
	/// Data formatter interface
	/// </summary>
	public interface IDataFormatter
	{
		/// <summary>
		/// Format data stored in cell
		/// </summary>
		/// <param name="grid">Instance of grid control</param>
		/// <param name="cell">Instance of cell to be formatted</param>
		/// <returns></returns>
		bool FormatCell(ReoGridControl grid, ReoGridCell cell);

		/// <summary>
		/// Indicate that whether need to perform a data type, if this method returns false,
		/// this data formatter will not be iterated to perform test by DataFormatManager.
		/// </summary>
		/// <returns></returns>
		bool PerformTestFormat();
	}
	#endregion

	#region General
	/// <summary>
	/// GeneralDataFormatter supports both Text and Numeric format.
	/// And format type can be switched after data changed by user inputing.
	/// </summary>
	internal class GeneralDataFormatter : IDataFormatter
	{
		public bool FormatCell(ReoGridControl grid, ReoGridCell cell)
		{
			object data = cell.Data;

			// check numeric
			bool isNumeric = false;

			double value = 0;
			if (data is int)
			{
				value = (double)(int)data;
				isNumeric = true;
			}
			else if (data is double)
			{
				value = (double)data;
				isNumeric = true;
			}
			else if (data is string)
			{
				string strdata = (data as string).Trim();

				isNumeric = double.TryParse(strdata, out value);

				if (isNumeric) cell.Data = value;
			}

			if (isNumeric)
			{
				if (cell.Style.HAlign == ReoGridHorAlign.General)
				{
					cell.RenderHorAlign = ReoGridRenderHorAlign.Right;
				}

				cell.Display = Convert.ToString(value);

				return true;
			}
			else
				return false;
		}
		public string[] Formats { get { return null; } }


		public bool PerformTestFormat()
		{
			return true;
		}
	}
	#endregion

	#region Number
	/// <summary>
	/// Number Formatter used to format data as numeric format.
	/// Available also to format data with different negative styles.
	/// </summary>
	[DataFormat(CellDataFormatFlag.Number)]
	public class NumberDataFormatter : IDataFormatter
	{
		/// <summary>
		/// Format given cell
		/// </summary>
		/// <param name="grid">Instance of grid control</param>
		/// <param name="cell">Instance of cell to be formatted</param>
		/// <returns></returns>
		public bool FormatCell(ReoGridControl grid, ReoGridCell cell)
		{
			object data = cell.Data;

			// check numeric
			bool isNumeric = false;

			double value = 0;
			if (data is int)
			{
				value = (double)(int)data;
				isNumeric = true;
			}
			else if (data is float)
			{
				value = (double)(float)data;
				isNumeric = true;
			}
			else if (data is double)
			{
				value = (double)data;
				isNumeric = true;
			}
			else if (data is string)
			{
				string strdata = (data as string).Trim();

				isNumeric = double.TryParse(strdata, out value);

				if (!isNumeric) isNumeric = double.TryParse(strdata.Replace(",", ""), out value);

				if (isNumeric) cell.Data = value;
			}
			else if (data is DateTime)
			{
				value = ((DateTime)data - new DateTime(1900, 1, 1)).TotalDays;
				isNumeric = true;
			}

			if (isNumeric)
			{
				if (cell.Style.HAlign == ReoGridHorAlign.General)
				{
					cell.RenderHorAlign = ReoGridRenderHorAlign.Right;
				}

				short decimals = 2;
				bool useSeparator = true;
				NumberNegativeStyle negativeStyle = NumberNegativeStyle.Minus;

				if (cell.DataFormatArgs != null && cell.DataFormatArgs is NumberFormatArgs)
				{
					NumberFormatArgs args = (NumberFormatArgs)cell.DataFormatArgs;
					decimals = args.DecimalPlaces;
					negativeStyle = args.NegativeStyle;
					useSeparator = args.UseSeparator;
				}
				else
				{
					//cell.DataFormatArgs = new NumberFormatArgs
					//{
					//  DecimalPlaces = decimals,
					//  NegativeStyle = negativeStyle,
					//  UseSeparator = true
					//};
				}

				if (value < 0)
				{
					if ((negativeStyle & NumberNegativeStyle.Red) == NumberNegativeStyle.Red)
						cell.RenderColor = Color.Red;
					else
						cell.RenderColor = Color.Empty;
				}

				// decimal places
				string decimalPlacePart = new string('0', decimals);

				// number
				string numberPart = (useSeparator ? "#,##0." : "0.") + decimalPlacePart;
				if ((negativeStyle & NumberNegativeStyle.Brackets) == NumberNegativeStyle.Brackets)
				{
					numberPart = (value < 0) ? ("(" + numberPart + ")") : numberPart;
				}

				// negative
				if (negativeStyle != NumberNegativeStyle.Minus)
				{
					value = Math.Abs(value);
				}

				cell.Display = value.ToString(numberPart);

				return true;
			}
			else
				return false;
		}

		[Serializable]
		public struct NumberFormatArgs
		{
			private short decimalPlaces;
			/// <summary>
			/// Number of decimal places
			/// </summary>
			public short DecimalPlaces { get { return decimalPlaces; } set { decimalPlaces = value; } }
			private NumberNegativeStyle negativeStyle;
			/// <summary>
			/// Style of negative number
			/// </summary>
			public NumberNegativeStyle NegativeStyle { get { return negativeStyle; } set { negativeStyle = value; } }
			private bool useSeparator;
			/// <summary>
			/// Determine whether to use a separator to split number every 3 digits.
			/// </summary>
			public bool UseSeparator { get { return useSeparator; } set { useSeparator = value; } }
			/// <summary>
			/// Compare to another instance of NumberFormatArgs
			/// </summary>
			/// <param name="obj">Another instance to be compared</param>
			/// <returns></returns>
			public override bool Equals(object obj)
			{
				if (!(obj is NumberFormatArgs)) return false;
				NumberFormatArgs o = (NumberFormatArgs)obj;
				return decimalPlaces.Equals(o.decimalPlaces)
					&& negativeStyle.Equals(o.negativeStyle)
					&& useSeparator.Equals(o.useSeparator);
			}

			public override int GetHashCode()
			{
				return decimalPlaces ^ (int)negativeStyle ^ (useSeparator.GetHashCode());
			}
		}

		/// <summary>
		/// System negative number display support (currently unused)
		///
		/// NumberFormat.NumberNegativePattern
		///
		/// Default         :       (1,234.00)
		/// Pattern 0       :       (1,234.00)
		/// Pattern 1       :       -1,234.00
		/// Pattern 2       :       - 1,234.00
		/// Pattern 3       :       1,234.00-
		/// Pattern 4       :       1,234.00 -
		///
		/// NumberFormatInfo.NumberNegativePattern.ToString("N", NumberFormatInfo) 
		/// </summary>
		public enum NumberNegativeStyle : byte
		{
			Minus = 0,
			Brackets = 0x1,
			Red = 0x2,

			RedMinus = Minus | Red,
			RedBrackets = Brackets | Red,
		}
		/*
		internal enum NumberNegativeStyle : byte
		{
			Minus = 0x1,
			Red = 0x2,
			Brackets = 0x4,

			MinusRed = Minus | Red,
			BracketsRed = Brackets | Red,

			// todo: support culture prefix negative symbol
		}*/

		public bool PerformTestFormat()
		{
			return true;
		}
	}
	#endregion

	#region Text
	internal class TextDataFormatter : IDataFormatter
	{
		public bool FormatCell(ReoGridControl grid, ReoGridCell cell)
		{
			cell.Display = Convert.ToString(cell.Data);

			if (cell.Style.HAlign == ReoGridHorAlign.General)
			{
				cell.RenderHorAlign = ReoGridRenderHorAlign.Left;
			}

			return true;
		}

		public bool PerformTestFormat()
		{
			return false;
		}
	}
	#endregion

	#region DateTime
	public class DateTimeDataFormatter : IDataFormatter
	{
		public bool FormatCell(ReoGridControl grid, ReoGridCell cell)
		{
			object data = cell.Data;

			bool isFormat = false;

			DateTime value = new DateTime(1900, 1, 1);

			if (data is DateTime)
			{
				value = (DateTime)data;
				isFormat = true;
			}
			else if (data is double)
			{
				try
				{
					value = value.AddDays((double)data);
					isFormat = true;
				}
				catch { }
			}
			else
			{
				string strdata = Convert.ToString(data);

				double days = 0;
				if (double.TryParse(strdata, out days))
				{
					try
					{
						value = value.AddDays(days);
						isFormat = true;
					}
					catch { }
				}
				else
				{
					isFormat = (DateTime.TryParse(strdata, out value));
				}
			}

			if (isFormat)
			{
				if (cell.Style.HAlign == ReoGridHorAlign.General)
				{
					cell.RenderHorAlign = ReoGridRenderHorAlign.Right;
				}

				CultureInfo culture = null;

				string format = System.Threading.Thread.CurrentThread.CurrentCulture.DateTimeFormat.ShortDatePattern;

				if (cell.DataFormatArgs != null && cell.DataFormatArgs is DateTimeFormatArgs)
				{
					DateTimeFormatArgs dargs = (DateTimeFormatArgs)cell.DataFormatArgs;

					format = dargs.Format;
					culture = new CultureInfo(dargs.CultureName);
				}
				else
				{
					culture = System.Threading.Thread.CurrentThread.CurrentCulture;
					cell.DataFormatArgs = new DateTimeFormatArgs { Format = format, CultureName = culture.Name };
				}

				cell.Display = value.ToString(format, culture);
			}

			return isFormat;
		}

		[Serializable]
		public struct DateTimeFormatArgs
		{
			private string format;
			public string Format { get { return format; } set { format = value; } }

			private string cultureName;
			public string CultureName { get { return cultureName; } set { cultureName = value; } }

			public override bool Equals(object obj)
			{
				if (!(obj is DateTimeFormatArgs)) return false;
				DateTimeFormatArgs o = (DateTimeFormatArgs)obj;
				return format.Equals(o.format)
					&& cultureName.Equals(o.cultureName);
			}

			public override int GetHashCode()
			{
				return format.GetHashCode() ^ cultureName.GetHashCode();
			}
		}

		public bool PerformTestFormat()
		{
			return true;
		}
	}
	#endregion

	#region Percent
	public class PercentDataFormatter : IDataFormatter
	{
		public bool FormatCell(ReoGridControl grid, ReoGridCell cell)
		{
			object data = cell.Data;

			double percent = 0;
			bool isFormat = false;
			short dec = 0;

			if (data is double)
			{
				percent = (double)data;
				isFormat = true;
				dec = 9;
			}
			else if (data is DateTime)
			{
				percent = ((DateTime)data - new DateTime(1900, 1, 1)).TotalDays;
				isFormat = true;
				dec = 0;
			}
			else
			{
				string str = Convert.ToString(data);
				if (str.Length > 1 && str.EndsWith("%"))
				{
					str = str.Substring(0, str.Length - 1);

					isFormat = double.TryParse(str, out percent);

					if (isFormat)
					{
						int decimalPlaceIndex = str.LastIndexOf('.');
						if (decimalPlaceIndex >= 0)
						{
							dec = (short)(str.Length - decimalPlaceIndex);
						}
					}
				}
				else
				{
					isFormat = double.TryParse(str, out percent);

					if (!isFormat)
					{
						DateTime date = new DateTime(1900, 1, 1);
						if (DateTime.TryParse(str, out date))
						{
							percent = (date - new DateTime(1900, 1, 1)).TotalDays;
							isFormat = true;
						}
					}

					// should not use 'else' here
					if (isFormat)
					{
						int decimalPlaceIndex = str.LastIndexOf('.');
						if (decimalPlaceIndex >= 0)
						{
							dec = (short)(str.Length - decimalPlaceIndex);
						}
					}
				}

				if (isFormat) cell.Data = percent;
			}

			if (isFormat)
			{
				if (cell.DataFormatArgs != null && cell.DataFormatArgs is PercentFormatArgs)
				{
					dec = ((PercentFormatArgs)cell.DataFormatArgs).DecimalPlaces;
				}
				else
				{
					cell.DataFormatArgs = new PercentFormatArgs { DecimalPlaces = dec };
				}

				string decimalPlacePart = new string('0', dec);

				cell.Display = percent.ToString("0." + decimalPlacePart) + "%";

				if (cell.Style.HAlign == ReoGridHorAlign.General)
				{
					cell.RenderHorAlign = ReoGridRenderHorAlign.Right;
				}
			}

			return isFormat;
		}

		[Serializable]
		public struct PercentFormatArgs
		{
			private short decimalPlaces;
			public short DecimalPlaces { get { return decimalPlaces; } set { decimalPlaces = value; } }

			public override bool Equals(object obj)
			{
				if (!(obj is PercentFormatArgs)) return false;
				PercentFormatArgs o = (PercentFormatArgs)obj;
				return decimalPlaces.Equals(o.decimalPlaces);
			}

			public override int GetHashCode()
			{
				return decimalPlaces;
			}
		}

		public bool PerformTestFormat()
		{
			return true;
		}
	}
	#endregion

	#region Currency
	public class CurrencyDataFormatter : IDataFormatter
	{
		public bool FormatCell(ReoGridControl grid, ReoGridCell cell)
		{
			bool isFormat = false;

			object data = cell.Data;
			double currency = double.NaN;

			if (data is double)
			{
				isFormat = true;
				currency = (double)data;
			}
			else if (data is DateTime)
			{
				currency = (new DateTime(1900, 1, 1) - (DateTime)data).TotalDays;
				isFormat = true;
			}
			else
			{
				string str = Convert.ToString(data).Trim();
				string number = string.Empty;

				if (str.StartsWith("$"))
				{
					number = str.Substring(1);
					if (double.TryParse(number, out currency))
					{
						isFormat = true;
						cell.Data = currency;
					}
				}
				else
				{
					DateTime date = new DateTime(1900, 1, 1);
					if (DateTime.TryParse(str, out date))
					{
						currency = (date - new DateTime(1900, 1, 1)).TotalDays;
						isFormat = true;
					}
					else
					{
						isFormat = double.TryParse(str, out currency);
					}
				}
			}

			if (isFormat)
			{
				if (cell.Style.HAlign == ReoGridHorAlign.General)
				{
					cell.RenderHorAlign = ReoGridRenderHorAlign.Right;
				}

				string symbol = null;
				short decimals = 2;
				NumberDataFormatter.NumberNegativeStyle negativeStyle = NumberDataFormatter.NumberNegativeStyle.Minus;

				if (cell.DataFormatArgs != null && cell.DataFormatArgs is CurrencyFormatArgs)
				{
					CurrencyFormatArgs args = (CurrencyFormatArgs)cell.DataFormatArgs;
					symbol = args.Symbol;
					decimals = args.DecimalPlaces;
					negativeStyle = args.NegativeStyle;
				}
				else
				{
					symbol = Thread.CurrentThread.CurrentCulture.NumberFormat.CurrencySymbol;
					cell.DataFormatArgs = new CurrencyFormatArgs { Symbol = symbol, DecimalPlaces = decimals };
				}

				if (currency < 0)
				{
					if ((negativeStyle & NumberDataFormatter.NumberNegativeStyle.Red) == NumberDataFormatter.NumberNegativeStyle.Red)
						cell.RenderColor = Color.Red;
					else
						cell.RenderColor = Color.Empty;
				}

				// decimal places
				string decimalPlacePart = new string('0', decimals);

				// number
				string numberPart = symbol + "#,##0." + decimalPlacePart;
				if ((negativeStyle & NumberDataFormatter.NumberNegativeStyle.Brackets) == NumberDataFormatter.NumberNegativeStyle.Brackets)
				{
					numberPart = (currency < 0) ? ("(" + numberPart + ")") : numberPart;
				}

				// negative
				if (negativeStyle != NumberDataFormatter.NumberNegativeStyle.Minus)
				{
					currency = Math.Abs(currency);
				}

				cell.Display = currency.ToString(numberPart);




				//string decimalPlaceFormat = new string('0', decimalPlaces);
				//cell.Display = string.Format("{0}{1:#,##0." + decimalPlaceFormat + "}", symbol, currency);
			}

			return isFormat;
		}

		[Serializable]
		public struct CurrencyFormatArgs
		{
			private string symbol;
			public string Symbol { get { return symbol; } set { symbol = value; } }
			private short decimalPlaces;
			public short DecimalPlaces { get { return decimalPlaces; } set { decimalPlaces = value; } }
			private string cultureEnglishName;
			public string CultureEnglishName { get { return cultureEnglishName; } set { cultureEnglishName = value; } }
			private NumberDataFormatter.NumberNegativeStyle negativeStyle;
			public NumberDataFormatter.NumberNegativeStyle NegativeStyle { get { return negativeStyle; } set { negativeStyle = value; } }

			public override bool Equals(object obj)
			{
				if (!(obj is CurrencyFormatArgs)) return false;
				CurrencyFormatArgs o = (CurrencyFormatArgs)obj;
				return symbol.Equals(o.symbol)
					&& decimalPlaces.Equals(o.decimalPlaces)
					&& cultureEnglishName.Equals(o.cultureEnglishName)
					&& negativeStyle.Equals(o.negativeStyle);
			}

			public override int GetHashCode()
			{
				return symbol.GetHashCode() ^ decimalPlaces ^ cultureEnglishName.GetHashCode() ^ (int)negativeStyle;
			}
		}

		public bool PerformTestFormat()
		{
			return true;
		}
	}
	#endregion
}
