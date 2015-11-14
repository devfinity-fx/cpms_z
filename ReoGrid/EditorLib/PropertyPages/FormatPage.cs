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
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Globalization;
using System.Threading;
using System.Reflection;

using unvell.ReoGrid.DataFormat;
using unvell.ReoGrid.Actions;

namespace unvell.ReoGrid.PropertyPages
{
	public partial class FormatPage : UserControl, IPropertyPage
	{
		private ReoGridControl grid;

		public ReoGridControl Grid
		{
			get { return grid; }
			set { this.grid = value; }
		}

		private ReoGridCell sampleCell;

		private object originalData = null;

		private CellDataFormatFlag? currentFormat = null;
		private object currentFormatArgs = null;

		private Panel currentSettingPanel = null;

		public FormatPage()
		{
			InitializeComponent();

			#region Initialize Setting Panels

			numberNegativeStyleList.Items.Add(new NegativeStyleListItem(
				NumberDataFormatter.NumberNegativeStyle.Minus, (-1234.10f).ToString("###,###.00"), SystemColors.WindowText));
			numberNegativeStyleList.Items.Add(new NegativeStyleListItem(
				NumberDataFormatter.NumberNegativeStyle.Red, (1234.10f).ToString("###,###.00"), Color.Red));
			numberNegativeStyleList.Items.Add(new NegativeStyleListItem(
				NumberDataFormatter.NumberNegativeStyle.Brackets, (1234.10f).ToString("(###,###.00)"), SystemColors.WindowText));
			numberNegativeStyleList.Items.Add(new NegativeStyleListItem(
				NumberDataFormatter.NumberNegativeStyle.RedBrackets, (1234.10f).ToString("(###,###.00)"), Color.Red));

			numberNegativeStyleList.ColorGetter += (i, item) =>
			{
				return ((NegativeStyleListItem)item).Color;
			};

			if (numberNegativeStyleList.Items.Count > 0) numberNegativeStyleList.SelectedIndex = 0;

			currencyNegativeStyleList.Items.Add(new NegativeStyleListItem(
				NumberDataFormatter.NumberNegativeStyle.Minus, /*"-$1,234.10"*/(-1234.10f).ToString("$###,###.00"), SystemColors.WindowText));
			currencyNegativeStyleList.Items.Add(new NegativeStyleListItem(
				NumberDataFormatter.NumberNegativeStyle.Red, /*"$1,234.10"*/(1234.10f).ToString("$###,###.00"), Color.Red));
			currencyNegativeStyleList.Items.Add(new NegativeStyleListItem(
				NumberDataFormatter.NumberNegativeStyle.Brackets, /*"($1,234.10)"*/(1234.10f).ToString("($###,###.00)"), SystemColors.WindowText));
			currencyNegativeStyleList.Items.Add(new NegativeStyleListItem(
				NumberDataFormatter.NumberNegativeStyle.RedBrackets, /*"($1,234.10)"*/(1234.10f).ToString("($###,###.00)"), Color.Red));

			currencyNegativeStyleList.ColorGetter += (i, item) =>
			{
				return ((NegativeStyleListItem)item).Color;
			};

			if (currencyNegativeStyleList.Items.Count > 0) currencyNegativeStyleList.SelectedIndex = 0;

			datetimeFormatList.SelectedIndexChanged += (s, e) =>
			{
				if (datetimeFormatList.SelectedItem != null)
				{
					DatetimeFormatListItem item = (DatetimeFormatListItem)datetimeFormatList.SelectedItem;
					txtDatetimeFormat.Text = item.Pattern;
				}
			};

			CultureInfo currentCulture = Thread.CurrentThread.CurrentCulture;
		
			datetimeFormatList.Items.AddRange(new object[] {
				// culture patterns
				new DatetimeFormatListItem(true, currentCulture.DateTimeFormat.ShortDatePattern),
				new DatetimeFormatListItem(true, currentCulture.DateTimeFormat.LongDatePattern),
				new DatetimeFormatListItem(true, currentCulture.DateTimeFormat.ShortDatePattern 
					+ " " + currentCulture.DateTimeFormat.ShortTimePattern),
				new DatetimeFormatListItem(true, currentCulture.DateTimeFormat.LongDatePattern 
					+ " " + currentCulture.DateTimeFormat.LongTimePattern),
				new DatetimeFormatListItem(true, currentCulture.DateTimeFormat.ShortTimePattern),
				new DatetimeFormatListItem(true, currentCulture.DateTimeFormat.LongTimePattern),

				// predefine patterns
				new DatetimeFormatListItem(false, "yyyy/M/d"),
				new DatetimeFormatListItem(false, "yyyy/M/d hh:mm"),
				new DatetimeFormatListItem(false, "M/d"),
				new DatetimeFormatListItem(false, "hh:mm"),
			});

			datetimeLocationList.SelectedIndexChanged += (s, e) =>
			{
				CultureInfo ci = (CultureInfo)datetimeLocationList.SelectedItem;

				((DatetimeFormatListItem)(datetimeFormatList.Items[0])).Pattern = ci.DateTimeFormat.ShortDatePattern;
				((DatetimeFormatListItem)(datetimeFormatList.Items[1])).Pattern = ci.DateTimeFormat.LongDatePattern;
				((DatetimeFormatListItem)(datetimeFormatList.Items[2])).Pattern = ci.DateTimeFormat.ShortDatePattern + " " + ci.DateTimeFormat.ShortTimePattern;
				((DatetimeFormatListItem)(datetimeFormatList.Items[3])).Pattern = ci.DateTimeFormat.LongDatePattern + " " + ci.DateTimeFormat.LongTimePattern;
				((DatetimeFormatListItem)(datetimeFormatList.Items[4])).Pattern = ci.DateTimeFormat.ShortTimePattern;
				((DatetimeFormatListItem)(datetimeFormatList.Items[5])).Pattern = ci.DateTimeFormat.LongTimePattern;

				DateTime dt = new DateTime(1980, 7, 13);
				CultureInfo culture = (CultureInfo)datetimeLocationList.SelectedItem;
				foreach (DatetimeFormatListItem item in datetimeFormatList.Items)
				{
					item.Sample = dt.ToString(item.Pattern, culture);
				}

				typeof(ListBox).InvokeMember("RefreshItems", 
					BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.InvokeMethod,
					null, datetimeFormatList, new object[] { });

				object backup = datetimeFormatList.SelectedItem;
				if (backup != null)
				{
					datetimeFormatList.SelectedItem = null;
					datetimeFormatList.SelectedItem = backup;
				}
				else
				{
					datetimeFormatList.SelectedIndex = 0;
				}
			};

			var cultures = CultureInfo.GetCultures(CultureTypes.InstalledWin32Cultures).OrderBy(c => c.EnglishName);
			foreach (CultureInfo info in cultures)
			{
				datetimeLocationList.Items.Add(info);
				if (info.Equals(currentCulture))
				{
					datetimeLocationList.SelectedItem = info;
				}

				try
				{
					CurrencySymbolListItem item = new CurrencySymbolListItem(info.EnglishName, info.NumberFormat.CurrencySymbol);
					currencySymbolList.Items.Add(item);
					if (info.Equals(currentCulture))
					{
						currencySymbolList.SelectedItem = item;
					}
				}
				catch { }
			}

			formatList.Items.AddRange(Enum.GetNames(typeof(CellDataFormatFlag)));

			numberFormatArgs = new NumberDataFormatter.NumberFormatArgs
			{
				DecimalPlaces = 2,
				NegativeStyle = NumberDataFormatter.NumberNegativeStyle.Minus,
				UseSeparator = true,
			};
			datetimeFormatArgs = new DateTimeDataFormatter.DateTimeFormatArgs
			{
				Format = currentCulture.DateTimeFormat.ShortDatePattern,
			};
			currencyFormatArgs = new CurrencyDataFormatter.CurrencyFormatArgs()
			{
				CultureEnglishName = currentCulture.EnglishName,
				DecimalPlaces = (short)currentCulture.NumberFormat.CurrencyDecimalDigits,
				NegativeStyle = NumberDataFormatter.NumberNegativeStyle.Minus,
			};
			percentFormatArgs = new PercentDataFormatter.PercentFormatArgs()
			{
				DecimalPlaces = 0,
			};

			#endregion

			formatList.SelectedIndexChanged += (sender, e) =>
			{
				try
				{
					currentFormat = (CellDataFormatFlag)Enum.Parse(typeof(CellDataFormatFlag), formatList.Text);
				}
				catch
				{
					currentFormat = CellDataFormatFlag.General;
				}

				Panel newSettingPanel = null;

				switch (currentFormat)
				{
					case CellDataFormatFlag.Number:
						newSettingPanel = numberPanel;
						break;
					case CellDataFormatFlag.DateTime:
						newSettingPanel = datetimePanel;
						break;
					case CellDataFormatFlag.Currency:
						newSettingPanel = currencyPanel;
						break;
					case CellDataFormatFlag.Percent:
						newSettingPanel = percentPanel;
						break;
				}

				if (newSettingPanel != currentSettingPanel)
				{
					if (newSettingPanel != null)
					{
						newSettingPanel.Location = new Point(sampleGroup.Left - 5, sampleGroup.Bottom + 10);
						newSettingPanel.Show();
					}

					if (currentSettingPanel != null)
					{
						currentSettingPanel.Hide();
					}

					currentSettingPanel = newSettingPanel;
				}

				UpdateSample();
			};

			numberDecimalPlaces.ValueChanged += (s, e) => UpdateSample();
			chkNumberUseSeparator.CheckedChanged += (s, e) => UpdateSample();
			numberNegativeStyleList.SelectedIndexChanged += (s, e) => UpdateSample();
			currencySymbolList.SelectedIndexChanged += (s, e) => UpdateSample();
			currencyDecimalPlaces.ValueChanged += (s, e) => UpdateSample();
			currencyNegativeStyleList.SelectedIndexChanged += (s, e) => UpdateSample();
			percentDecimalPlaces.ValueChanged += (s, e) => UpdateSample();

			formatList.DoubleClick += (s, e) => RaiseDone();
			numberNegativeStyleList.DoubleClick += (s, e) => RaiseDone();
			datetimeFormatList.DoubleClick += (s, e) => RaiseDone();

			txtDatetimeFormat.TextChanged += (s, e) => UpdateSample();
		}

		public event EventHandler Done;
		public void RaiseDone()
		{
			if (Done != null) Done(this, null);
		}

		private NumberDataFormatter.NumberFormatArgs numberFormatArgs;
		private DateTimeDataFormatter.DateTimeFormatArgs datetimeFormatArgs;
		private CurrencyDataFormatter.CurrencyFormatArgs currencyFormatArgs;
		private PercentDataFormatter.PercentFormatArgs percentFormatArgs;

		private void UpdateSample()
		{
			currentFormatArgs = null;

			switch (currentFormat)
			{
				case CellDataFormatFlag.Number:
					numberFormatArgs.DecimalPlaces = (short)numberDecimalPlaces.Value;
					numberFormatArgs.NegativeStyle = ((NegativeStyleListItem)numberNegativeStyleList.SelectedItem).NegativeStyle;
					numberFormatArgs.UseSeparator = chkNumberUseSeparator.Checked;
					currentFormatArgs = numberFormatArgs;
					break;

				case CellDataFormatFlag.DateTime:
					datetimeFormatArgs.Format = txtDatetimeFormat.Text;
					datetimeFormatArgs.CultureName = ((CultureInfo)(datetimeLocationList.SelectedItem)).Name;
					currentFormatArgs = datetimeFormatArgs;
					break;

				case CellDataFormatFlag.Currency:
					if (currencySymbolList.SelectedItem != null)
					{
						CurrencySymbolListItem symbolInfo = ((CurrencySymbolListItem)currencySymbolList.SelectedItem);
						currencyFormatArgs.CultureEnglishName = symbolInfo.CultureEnglishName;
						currencyFormatArgs.Symbol = symbolInfo.Symbol;
					}
					currencyFormatArgs.NegativeStyle = ((NegativeStyleListItem)currencyNegativeStyleList.SelectedItem).NegativeStyle;
					currencyFormatArgs.DecimalPlaces = (short)currencyDecimalPlaces.Value;
					currentFormatArgs = currencyFormatArgs;
					break;

				case CellDataFormatFlag.Percent:
					percentFormatArgs.DecimalPlaces = (short)percentDecimalPlaces.Value;
					currentFormatArgs = percentFormatArgs;
					break;
			}

			if (sampleCell != null)
			{
				sampleCell.Data = originalData;
				sampleCell.DataFormat = currentFormat ?? CellDataFormatFlag.General;
				sampleCell.DataFormatArgs = currentFormatArgs;

				// force format sample cell
				DataFormatterManager.Instance.DataFormatters[sampleCell.DataFormat].FormatCell(grid, sampleCell);

				labSample.Text = sampleCell.Display;
				labSample.ForeColor = sampleCell.GetRenderColor();
			}
		}

		private CellDataFormatFlag? backupFormat;
		private object backupFormatArgs;

		public void LoadPage()
		{
			grid.IterateCells(grid.SelectionRange, (r, c, cell) =>
			{
				if (backupFormat == null)
				{
					sampleCell = new ReoGridCell();
					ReoGridCellUtility.CopyCellContent(sampleCell, cell);

					if (cell != null) originalData = cell.Data;

					backupFormat = cell.DataFormat;
					return true;
				}
				else if (backupFormat == cell.DataFormat)
				{
					return true;
				}
				else
				{
					backupFormat = null;
					return false;
				}
			});

			currentFormat = backupFormat;

			backupFormatArgs = null;

			if (currentFormat != null)
			{
				switch (currentFormat)
				{
					case CellDataFormatFlag.Number:
						if (sampleCell.DataFormatArgs is NumberDataFormatter.NumberFormatArgs)
						{
							NumberDataFormatter.NumberFormatArgs nargs = (NumberDataFormatter.NumberFormatArgs)sampleCell.DataFormatArgs;
							numberDecimalPlaces.Value = nargs.DecimalPlaces;
							chkNumberUseSeparator.Checked = nargs.UseSeparator;
							foreach (NegativeStyleListItem item in numberNegativeStyleList.Items)
							{
								if (item.NegativeStyle == nargs.NegativeStyle)
								{
									numberNegativeStyleList.SelectedItem = item;
									break;
								}
							}
							backupFormatArgs = nargs;
						}
						break;

					case CellDataFormatFlag.DateTime:
						DateTimeDataFormatter.DateTimeFormatArgs dargs = (DateTimeDataFormatter.DateTimeFormatArgs)sampleCell.DataFormatArgs;
						txtDatetimeFormat.Text = dargs.Format;
						int dfindex =-1;
						for (int i = 0; i < datetimeFormatList.Items.Count; i++)
						{
							DatetimeFormatListItem item = (DatetimeFormatListItem)datetimeFormatList.Items[i];
							if (item.Pattern.Equals(dargs.Format, StringComparison.CurrentCultureIgnoreCase))
							{
								dfindex = i;
								break;
							}
						}
						datetimeFormatList.SelectedIndex = dfindex;
						backupFormatArgs = dargs;
						break;

					case CellDataFormatFlag.Currency:
						CurrencyDataFormatter.CurrencyFormatArgs cargs = (CurrencyDataFormatter.CurrencyFormatArgs)sampleCell.DataFormatArgs;
						currencyDecimalPlaces.Value = cargs.DecimalPlaces;
						int cnindex = (int)cargs.NegativeStyle;
						if (cnindex >= 0 && cnindex < currencyNegativeStyleList.Items.Count) currencyNegativeStyleList.SelectedIndex = cnindex;
						foreach (NegativeStyleListItem item in currencyNegativeStyleList.Items)
						{
							if (item.NegativeStyle == cargs.NegativeStyle)
							{
								currencyNegativeStyleList.SelectedItem = item;
								break;
							}
						}
						backupFormatArgs = cargs;
						break;

					case CellDataFormatFlag.Percent:
						PercentDataFormatter.PercentFormatArgs pargs = (PercentDataFormatter.PercentFormatArgs)sampleCell.DataFormatArgs;
						percentDecimalPlaces.Value = pargs.DecimalPlaces;
						backupFormatArgs = pargs;
						break;
				}

				for (int i = 0; i < formatList.Items.Count; i++)
				{
					var item = formatList.Items[i].ToString();

					if (string.Equals(item, currentFormat.ToString(), StringComparison.CurrentCultureIgnoreCase))
					{
						formatList.SelectedIndex = i;
						break;
					}
				}
			}
			else
			{
				formatList.SelectedIndex = 0;
			}

			backupFormat = currentFormat;
		}

		public RGReusableAction CreateUpdateAction()
		{
			if(currentFormat != backupFormat || currentFormatArgs != backupFormatArgs
				&& currentFormat!=null)
			{
				return new RGSetRangeDataFormatAction(grid.SelectionRange, 
					(CellDataFormatFlag)currentFormat, currentFormatArgs);
			}
			else
				return null;
		}

		#region CurrencySymbolListItem
		private struct CurrencySymbolListItem
		{
			private string cultureEnglishName;
			public string CultureEnglishName { get { return cultureEnglishName; } set { cultureEnglishName = value; } }

			private string symbol;
			public string Symbol { get { return symbol; } set { symbol = value; } }

			public CurrencySymbolListItem(string cultureEnglishName, string symbol)
			{
				this.cultureEnglishName = cultureEnglishName;
				this.symbol = symbol;
			}

			public override string ToString()
			{
				return string.Format("{0} ({1})", cultureEnglishName, symbol);
			}
		}
		#endregion

		#region NegativeStyleListItem
		private struct NegativeStyleListItem
		{
			private NumberDataFormatter.NumberNegativeStyle negativeStyle;

			public NumberDataFormatter.NumberNegativeStyle NegativeStyle
			{
				get { return negativeStyle; }
				set { negativeStyle = value; }
			}

			private string sample;

			public string Sample
			{
				get { return sample; }
				set { sample = value; }
			}
			private Color color;

			public Color Color
			{
				get { return color; }
				set { color = value; }
			}

			public NegativeStyleListItem(NumberDataFormatter.NumberNegativeStyle negativeStyle,
				string sample, Color color)
			{
				this.negativeStyle = negativeStyle;
				this.sample = sample;
				this.color = color;
			}

			public override string ToString()
			{
				return this.sample;
			}
		}
		#endregion

		#region DatetimeFormatListItem
		private class DatetimeFormatListItem
		{
			private bool inCulture;

			public bool InCulture
			{
				get { return inCulture; }
				set { inCulture = value; }
			}

			private string pattern;

			public string Pattern
			{
				get { return pattern; }
				set { pattern = value; }
			}

			private string sample;

			public string Sample
			{
				get { return sample; }
				set { sample = value; }
			}

			public DatetimeFormatListItem() { }

			public DatetimeFormatListItem(bool inCulture, string pattern)
			{
				this.inCulture = inCulture;
				this.pattern = pattern;
			}

			public override string ToString()
			{
				return (inCulture ? "*": string.Empty) + sample;
			}
		}
		#endregion
	}
}
