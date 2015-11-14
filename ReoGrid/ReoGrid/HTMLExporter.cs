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
using System.IO;
using System.Drawing;

using unvell.Common;
using unvell.ReoGrid.XML;

namespace unvell.ReoGrid
{
	public class RGHTMLExporter
	{
		public ReoGridControl Grid { get; set; }

		public RGHTMLExporter(ReoGridControl grid)
		{
			this.Grid = grid;
		}

		/// <summary>
		/// Export grid as html5 into specified stream
		/// </summary>
		/// <param name="s">stream contains exported html5 data</param>
		public void Export(Stream s)
		{
			using (StreamWriter sw = new StreamWriter(s))
			{
				StringBuilder sb = new StringBuilder();
				ReoGridCell cell;

				sw.WriteLine("<!DOCTYPE html>");
				sw.WriteLine("<html>");
				sw.WriteLine("<head><title>ReoGrid Exported HTML</title></head>");
				sw.WriteLine("<body>");

				sw.WriteLine("  <table style='border-collapse:collapse;border:none;'>");
				for (int r = 0; r < Grid.RowCount - 1; r++)
				{
					sw.WriteLine(string.Format("    <tr style='height:{0}px;'>", Grid.RetrieveRowHead(r).Height));

					for (int c = 0; c < Grid.ColCount - 1; )
					{
						cell = Grid.GetCell(r, c);

						if (cell == null)
						{
							if (r == 0)
							{
								sw.WriteLine("      <td style='width:" + Grid.RetrieveColHead(c).Width + ";'></td>");
								sw.WriteLine("      <td></td>");
							}
							c++;
							continue;
						}
						else if(cell.Colspan <= 0 || cell.Rowspan <= 0)
						{
							c++;
							continue;
						}

						sb.Length = 0;
						sb.Append("      <td");

						if (cell.Rowspan > 1)
						{
							sb.Append(" rowspan='" + cell.Rowspan + "'");
						}
						if (cell.Colspan > 1)
						{
							sb.Append(" colspan='" + cell.Colspan + "'");
						}

						sb.AppendFormat(" style='width:{0};", cell.Width);

						ReoGridRangeStyle style = Grid.GetCellStyle(r, c);
						if (style != null)
						{

							// back color
							if (style.HasStyle(PlainStyleFlag.FillColor) && style.BackColor != Color.White)
							{
								WriteHtmlStyle(sb, "background-color", TextFormatHelper.EncodeColor(style.BackColor));
							}

							// text color
							if (style.HasStyle(PlainStyleFlag.TextColor) && style.TextColor != Color.Black)
							{
								WriteHtmlStyle(sb, "color", TextFormatHelper.EncodeColor(style.TextColor));
							}

							// font size
							if (style.HasStyle(PlainStyleFlag.FontSize))
							{
								WriteHtmlStyle(sb, "font-size", style.FontSize.ToString() +"pt");
							}

							// horizontal align
							if (style.HasStyle(PlainStyleFlag.HorizontalAlign))
							{
								WriteHtmlStyle(sb, "text-align", XmlFileFormatHelper.EncodeHorizontalAlign(style.HAlign));
							}

							// vertical align
							if (style.HasStyle(PlainStyleFlag.VerticalAlign))
							{
								WriteHtmlStyle(sb, "vertical-align", XmlFileFormatHelper.EncodeVerticalAlign(style.VAlign));
							}

						}

						ReoGridRangeBorderInfo rbi = Grid.GetRangeBorder(new ReoGridRange(cell.Row, cell.Col, cell.Rowspan, cell.Colspan));
					
						if (!rbi.Top.IsEmpty)
						{
							WriteHtmlStyle(sb, "border-top", string.Format("{0} {1} {2}",
								rbi.Top.Style.ToString(), "1px", TextFormatHelper.EncodeColor(rbi.Top.Color)));
						}
						if (!rbi.Left.IsEmpty)
						{
							WriteHtmlStyle(sb, "border-left", string.Format("{0} {1} {2}",
								rbi.Left.Style.ToString(), "1px", TextFormatHelper.EncodeColor(rbi.Left.Color)));
						}
						
						sb.Append("'>");

						sw.WriteLine(sb.ToString());

						cell = Grid.GetCell(r, c);
						if (cell != null)
						{
							sw.WriteLine(System.Web.HttpUtility.HtmlEncode(cell.Display));
						}

						sw.WriteLine("      </td>");

						c += cell.Colspan;
					}
					sw.WriteLine("    </tr>");
				}
				sw.WriteLine("  </table>");

				sw.WriteLine("</body>");
				sw.WriteLine("</html>");
			}
		}

		private static void WriteHtmlStyle(StringBuilder sb, string name, string value)
		{
			sb.AppendFormat("{0}:{1};", name, value);
		}
	}
}
