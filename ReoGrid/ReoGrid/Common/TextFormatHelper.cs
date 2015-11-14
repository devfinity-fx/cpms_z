/*****************************************************************************
 * 
 * ReoGrid - .NET Spreadsheet Control
 * 
 * http://reogrid.codeplex.com/
 *
 * Plain Text Format Convert Utilies
 * 
 * - Convert between .NET objects and string what used in XML serialization
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
using System.Globalization;

using unvell.Common;

namespace unvell.Common
{
	public class TextFormatHelper
	{
		#region Encoding
		public static string EncodeRect(RectangleF rect)
		{
			return (string.Format("({0},{1},{2},{3})",
					rect.Left, rect.Top, rect.Width, rect.Height));
		}
		public static string EncodePadding(Padding pad)
		{
			return (string.Format("({0},{1},{2},{3})",
					pad.Left, pad.Top, pad.Right, pad.Bottom));
		}
		public static string EncodeMargins(System.Drawing.Printing.Margins margins)
		{
			return (string.Format("({0},{1},{2},{3})",
					margins.Left, margins.Top, margins.Right, margins.Bottom));
		}
		public static string EncodeSize(Size size)
		{
			return (string.Format("({0},{1})", size.Width, size.Height));
		}
		public static string EncodeColor(Color c)
		{
			return c.IsEmpty ? "none" :
				(c.IsNamedColor ? c.Name :
				(c.A == 255 ? (string.Format("#{0:x2}{1:x2}{2:x2}", c.R, c.G, c.B))
				: string.Format("#{0:x2}{1:x2}{2:x2}{3:x2}", c.A, c.R, c.G, c.B)));
		}
		public static string EncodePoint(Point p)
		{
			return EncodePoint(new PointF(p.X, p.Y));
		}
		public static string EncodePoint(PointF p)
		{
			return (string.Format("({0},{1})", p.X, p.Y));
		}
		public static string EncodePoints(IEnumerable<PointF> points)
		{
			StringBuilder sb = new StringBuilder();
			foreach (var p in points)
			{
				if (sb.Length > 0) sb.Append(",");
				sb.AppendFormat("{0} {1}",p.X, p.Y);
			}
			return sb.ToString();
		}
		public static string EncodePointsHex(IEnumerable<PointF> points)
		{
			StringBuilder sb = new StringBuilder();
			foreach (var p in points)
			{
				byte[] buf = BitConverter.GetBytes(p.X);
				byte[] buf2 = BitConverter.GetBytes(p.Y);
				sb.AppendFormat("{0:x1}{1:x1}{2:x1}{3:x1}{4:x1}{5:x1}{6:x1}{7:x1}",
					buf[0], buf[1], buf[2], buf[3],
					buf2[0], buf2[1], buf2[2], buf2[3]);
			}
			return sb.ToString();
		}

		public static string EncodeFontStyle(FontStyle fs)
		{
			StringBuilder sb = new StringBuilder();
			if ((fs & FontStyle.Bold) > 0)
			{
				sb.Append("blob");
			}
			if ((fs & FontStyle.Italic) > 0)
			{
				if (sb.Length > 0) sb.Append(" ");
				sb.Append("italic");
			}
			if ((fs & FontStyle.Strikeout) > 0)
			{
				if (sb.Length > 0) sb.Append(" ");
				sb.Append("strikeout");
			}
			if ((fs & FontStyle.Underline) > 0)
			{
				if (sb.Length > 0) sb.Append(" ");
				sb.Append("underline");
			}
			
			return sb.Length == 0 ? "normal": sb.ToString();
		}
		public static string EncodeLineStyle(DashStyle ds)
		{
			switch (ds)
			{
				case (System.Drawing.Drawing2D.DashStyle.Dash):
					return "dash";
				case System.Drawing.Drawing2D.DashStyle.DashDot:
					return "dash-dot";
				case System.Drawing.Drawing2D.DashStyle.DashDotDot:
					return "dash-dot-dot";
				case System.Drawing.Drawing2D.DashStyle.Dot:
					return "dot";
				default:
				case System.Drawing.Drawing2D.DashStyle.Solid:
					return "solid";
			}
		}
		public static string EncodeLineCapStyle(LineCap cap)
		{
			switch (cap)
			{
				case LineCap.ArrowAnchor:
					return "arrow";
				case LineCap.DiamondAnchor:
					return "diamond";
				case LineCap.RoundAnchor:
					return "round";
				case LineCap.SquareAnchor:
					return "square";
				default:
				case LineCap.NoAnchor:
					return "none";
			}
		}

		public static string EncodeBool(bool p)
		{
			return p ? "true" : "false";
		}

		public static string EncodeHorizontalAlign(HorizontalAlign halign)
		{
			switch (halign)
			{
				case HorizontalAlign.Left:
					return "left";
				default:
				case HorizontalAlign.Center:
					return "center";
				case HorizontalAlign.Right:
					return "right";
			}
		}
		public static string EncodeVerticalAlign(VerticalAlign valign)
		{
			switch (valign)
			{
				case VerticalAlign.Top:
					return "top";
				default:
				case VerticalAlign.Middle:
					return "middle";
				case VerticalAlign.Bottom:
					return "bottom";
			}
		}

		public static string EncodeFormWindowState(FormWindowState state)
		{
			switch (state)
			{
				default:
				case FormWindowState.Normal:
					return "normal";
				case FormWindowState.Maximized:
					return "maximized";
			}
		}

		public static string EncodeFloatArray(params float[] values)
		{
			StringBuilder sb = new StringBuilder();
			foreach (float v in values)
			{
				if (sb.Length > 0) sb.Append(",");
				sb.Append(v);
			}
			return sb.ToString();
		}

		#endregion

		#region Decoding
		public static Guid DecodeGuid(string data)
		{
			try
			{
				return new Guid(data);
			}
			catch { return Guid.Empty; }
		}
		public static readonly Regex RectRegex =
				new Regex(@"\(\s*([-\w.]+)\s*,\s*([-\w.]+)\s*,\s*([-\w.]+)\s*,\s*([-\w.]+)\)\s*");
		public static RectangleF DecodeRect(string data)
		{
			Match m = RectRegex.Match(data);
			if (m.Success)
			{
				return new RectangleF(GetPixelValue(m.Groups[1].Value),
					GetPixelValue(m.Groups[2].Value),
					GetPixelValue(m.Groups[3].Value),
					GetPixelValue(m.Groups[4].Value));
			}
			else
				return Rectangle.Empty;
		}
		public static Padding DecodePadding(string data)
		{
			Rectangle r = Rectangle.Round(DecodeRect(data));
			return r.IsEmpty ? Padding.Empty : new Padding(
				r.Left, r.Top, r.Width, r.Height);
		}
		public static System.Drawing.Printing.Margins DecodeMargins(string data)
		{
			Rectangle r = Rectangle.Round(DecodeRect(data));
			return r.IsEmpty ? new System.Drawing.Printing.Margins(): new System.Drawing.Printing.Margins(
				r.Left, r.Top, r.Width, r.Height);
		}
		public static SizeF DecodeSize(string data)
		{
			return new SizeF(DecodePoint(data));
		}

		public static readonly Regex RGBColorRegex =
			new Regex(@"rgb\s*\(\s*((\d+)\s*,)?(\d+)\s*,(\d+)\s*,(\d+)\s*\)");
		public static readonly Regex WebColorRegex = new
				Regex(@"\#([0-9a-fA-F]{2})?([0-9a-fA-F]{2})([0-9a-fA-F]{2})([0-9a-fA-F]{2})");
		public static bool IsRGBColorFormat(string data)
		{
			return RGBColorRegex.IsMatch(data);
		}
		public static bool IsWebColorFormat(string data)
		{
			return WebColorRegex.IsMatch(data);
		}
		public static Color DecodeColor(string data)
		{
			if (data == null || data.Length == 0 || data.ToLower().Equals("none"))
			{
				return Color.Empty;
			}
			
			Match m = RGBColorRegex.Match(data.ToLower());
			if (m.Success)
			{
				return Color.FromArgb(m.Groups[2].Value.Length > 0 ?
					Convert.ToInt32(m.Groups[2].Value) : 255,
					Convert.ToInt32(m.Groups[3].Value),
					Convert.ToInt32(m.Groups[4].Value),
					Convert.ToInt32(m.Groups[5].Value));
			}
			else if ((m = WebColorRegex.Match(data)).Success)
			{
				return Color.FromArgb(m.Groups[1].Value.Length > 0 ?
									Convert.ToInt32(m.Groups[1].Value, 16) : 255,
									Convert.ToInt32(m.Groups[2].Value, 16),
									Convert.ToInt32(m.Groups[3].Value, 16),
									Convert.ToInt32(m.Groups[4].Value, 16));
			}
			else
			{
				try { return Color.FromName(data); }
				catch { }
			}
			return Color.Empty;
		}

		public static readonly Regex PointRegex =
				new Regex(@"\(\s*([-\w.]+)\s*,\s*([-\w.]+)\)\s*");
		public static bool IsRectFormat(string data)
		{
			return RectRegex.IsMatch(data);
		}		
		public static PointF DecodePoint(string data)
		{
			Match m = PointRegex.Match(data);
			if (m.Success)
			{
				return new PointF(GetPixelValue(m.Groups[1].Value),
					GetPixelValue(m.Groups[2].Value));
			}
			else
				return PointF.Empty;
		}

		public static Font DecodeFont(string fontName, string fontSize, string fontStyle)
		{
			string name = (fontName == null) ?
				SystemFonts.DefaultFont.FontFamily.Name : fontName;

			FontStyle fs = 0;
			if (fontStyle == null)
				fs = FontStyle.Regular;
			else
			{
				string[] fontStyles = fontStyle.Split(',');
				if (fontStyles.Length == 0)
				{
					fs = FontStyle.Regular;
				}
				else
				{
					foreach (string fstyle in fontStyles)
					{
						string fst = fstyle.Trim().ToLower();
						if (fst.Equals("blob"))
							fs |= FontStyle.Bold;
						else if (fst.Equals("italic"))
							fs |= FontStyle.Italic;
						else if (fst.Equals("strikeout"))
							fs |= FontStyle.Strikeout;
						else if (fst.Equals("underline"))
							fs |= FontStyle.Underline;
					}
				}
			}

			float size = GetFloatPixelValue(fontSize, SystemFonts.DefaultFont.Size);

			return ResourcePoolManager.Instance.GetFont(name, size, fs);
		}
		public static FontStyle DecodeFontStyle(string fontStyleStr)
		{
			FontStyle fs = FontStyle.Regular;
			string[] fsstr = fontStyleStr.Split(' ', ',');
			if (fsstr.Contains("blob"))
				fs |= FontStyle.Bold;
			else if (fsstr.Contains("italic"))
				fs |= FontStyle.Italic;
			else if (fsstr.Contains("strikeout"))
				fs |= FontStyle.Strikeout;
			else if (fsstr.Contains("underline"))
				fs |= FontStyle.Underline;
			else
				fs = FontStyle.Regular;
			return fs;
		}

		public static DashStyle DecodeLineStyle(string data)
		{
			switch(data.Trim().ToLower())
			{
				case "dash":
					return System.Drawing.Drawing2D.DashStyle.Dash;
				case "dash-dot":
					return System.Drawing.Drawing2D.DashStyle.DashDot;
				case "dash-dot-dot":
					return System.Drawing.Drawing2D.DashStyle.DashDotDot;
				case "dot":
					return System.Drawing.Drawing2D.DashStyle.Dot;
				default:
				case "solid":
					return System.Drawing.Drawing2D.DashStyle.Solid;
			}
		}
		public static LineCap DecodeLineCapStyle(string data)
		{
			switch (data)
			{
				case "arrow":
					return LineCap.ArrowAnchor;
				case "diamond":
					return LineCap.DiamondAnchor;
				case "round":
					return LineCap.RoundAnchor;
				case "square":
					return LineCap.SquareAnchor;
				default:
				case "none":
					return LineCap.NoAnchor;
			}
		}

		public static readonly Regex pointArrayRegex = new Regex(@"(\d*\.?\d+)\s(\d*\.?\d+)");
		public static List<PointF> DecodePoints(string data)
		{
			List<PointF> pts = new List<PointF>(5);
			foreach (Match m in pointArrayRegex.Matches(data))
			{
				pts.Add(new PointF(GetFloatPixelValue(m.Groups[1].Value, 0f),
					GetFloatPixelValue(m.Groups[2].Value, 0f)));
			}
			return pts;
		}

		public static HorizontalAlign DecodeHorizontalAlign(string align)
		{
			switch (align)
			{
				case "left":
					return HorizontalAlign.Left;
				default:
				case "center":
					return HorizontalAlign.Center;
				case "right":
					return HorizontalAlign.Right;
			}
		}
		public static VerticalAlign DecodeVerticalAlign(string valign)
		{
			switch (valign)
			{
				case "top":
					return VerticalAlign.Top;
				default:
				case "middle":
					return VerticalAlign.Middle;
				case "bottom":
					return VerticalAlign.Bottom;
			}
		}
		public static FormWindowState DecodeFormWindowState(string state)
		{
			switch (state)
			{
				default:
				case "normal":
					return FormWindowState.Normal;
				case "maximized":
					return FormWindowState.Maximized;
			}
		}

		public static readonly Regex PathDataRegex = new Regex(@"(\w?)\(([-?\d+,?]+)\),?");
		public static float[] DecodeFloatArray(string str)
		{
			List<float> f = new List<float>();
			foreach (string s in str.Split(',')) f.Add(GetFloatValue(s, 0));
			return f.ToArray();
		}
		#endregion

		public static float GetFloatValue(string str, float def)
		{
			float.TryParse(str, out def);
			return def;
		}
		public static float GetFloatValue(string str, float def, CultureInfo culture)
		{
			float.TryParse(str, NumberStyles.Any, culture, out def);
			return def;
		}
		public static float GetFloatPixelValue(string str, float def)
		{
			return GetFloatPixelValue(str, def, CultureInfo.CurrentCulture);
		}
		public static float GetFloatPixelValue(string str, float def, CultureInfo culture)
		{
			if (str == null) return def;
			str = str.Trim();
			if (str.EndsWith("px"))
				str = str.Substring(0, str.Length - 2).Trim();

			float v = 0;
			float.TryParse(str, NumberStyles.Any, culture, out v);

			return v;
		}
		public static int GetPixelValue(string str)
		{
			return (int)GetPixelValue(str, 0);
		}
		public static int GetPixelValue(string str, CultureInfo culture)
		{
			return (int)GetPixelValue(str, 0, culture);
		}
		public static int GetPixelValue(string str, int def)
		{
			return (int)GetFloatPixelValue(str, (float)def, CultureInfo.CurrentCulture);
		}
		public static int GetPixelValue(string str, int def, CultureInfo culture)
		{
			return (int)GetFloatPixelValue(str, (float)def, culture);
		}
		public static float GetPercentValue(string str, float def)
		{
			if(string.IsNullOrEmpty(str)) return def;
			string p = str.Substring(0, str.Length - 1).Trim();
			return GetFloatValue(p, def) / 100f;
		}

		public static readonly Regex AttrRegex
			= new Regex(@"\s*([-_\w.]+)\s*:\s*((\'([^\']*)\')|([^;]*))\s*;?");
		public static void ParseDataAttribute(string attr, Action<string, string> a)
		{
			if (attr != null)
			{
				foreach (Match m in AttrRegex.Matches(attr))
				{
					string key = m.Groups[1].Value;
					string value = (m.Groups[5].Value != null
						&& m.Groups[5].Length > 0) ? m.Groups[5].Value : m.Groups[4].Value;

					a(key, value);
				}
			}
		}
		public static T CreateElementFromAttribute<T>(T obj, string attr) where T : new()
		{
			T t = (obj == null ? new T() : obj);

			ParseDataAttribute(attr, (key, value) =>
			{
				if (key.Length > 0 && value != null)
				{
					FieldInfo fi = t.GetType().GetField(key);
					if (fi != null)
					{
						fi.SetValue(t, value);
					}
					else
					{
						key = key.Substring(0, 1).ToUpper() + key.Substring(1);

						PropertyInfo pi = t.GetType().GetProperty(key);
						if (pi == null)
						{
							pi = t.GetType().GetProperties().FirstOrDefault(p =>
							{
								XmlAttributeAttribute[] attrs
									= p.GetCustomAttributes(typeof(XmlAttributeAttribute), true)
									as XmlAttributeAttribute[];
								return (attrs != null && attrs.Length > 0 && attrs[0] != null
								&& attrs[0].AttributeName.ToLower().Equals(key.ToLower()));
							});
						}
						if (pi != null)
						{
							pi.SetValue(t, value, null);
						}
					}
				}
			});

			return t;
		}
		public static string GenerateDataAttributeString(Dictionary<string, string> data)
		{
			StringBuilder sb = new StringBuilder();
			foreach (string key in data.Keys) {
				if (sb.Length > 0) sb.Append("; ");
				sb.Append(string.Format("{0}: {1}", key, data[key]));
			}
			return sb.ToString();
		}
		public static string GenerateDataAttributeString(params string[] data)
		{
			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < data.Length; i += 2)
			{
				if (sb.Length > 0) sb.Append(" ");
				sb.Append(string.Format("{0}: {1};", data[i], data[i + 1]));
			}
			return sb.ToString();
		}

		public static bool IsSwitchOn(string value)
		{
			if (string.IsNullOrEmpty(value)) return false;
			
			return string.Equals(value, "yes", StringComparison.CurrentCultureIgnoreCase)
				|| string.Equals(value, "on", StringComparison.CurrentCultureIgnoreCase)
				|| string.Equals(value, "true", StringComparison.CurrentCultureIgnoreCase);
		}

		public static T LoadXml<T>(string filepath)
		{
			XmlSerializer xmlReader = new XmlSerializer(typeof(T));
			using (FileStream fs = new FileStream(filepath, FileMode.Open))
			{
				T t = (T)xmlReader.Deserialize(fs);
				return t;
			}
		}

	}
}
