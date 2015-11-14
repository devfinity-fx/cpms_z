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
using System.Text.RegularExpressions;

namespace unvell.ReoGrid
{
	public static class RGUtility
	{
		internal static readonly Regex CellReferenceRegex = new Regex("(?:(?<col>[A-Z]+)(?<row>[0-9]+))");

		internal static readonly Regex RangeReferenceRegex = new Regex(
			"(?:\"[^\"]*\\s*\")|(?:(?<from_col>[A-Z]+)(?<from_row>[0-9]+):(?<to_col>[A-Z]+)(?<to_row>[0-9]+))");

		private static readonly string AlphaChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

		/// <summary>
		/// Get alphabet of number (eg. A is 1 and 30 is AD)
		/// </summary>
		/// <param name="a">number to be converted</param>
		/// <returns>alphabet of number</returns>
		public static string GetAlphaChar(long a)
		{
			char[] v = new char[10];

			int i = 9;
			while (a >= AlphaChars.Length)
			{
				v[i] = AlphaChars[((int)(a % AlphaChars.Length))];
				a = a / AlphaChars.Length - 1;

				i--;
			}

			v[i] = AlphaChars[((int)(a % AlphaChars.Length))];
			return new string(v, i, 10 - i);
		}

		/// <summary>
		/// Get number of alphabet from string (eg. 1 is A and AD is 30)
		/// </summary>
		/// <param name="id">alphabet to be converted</param>
		/// <returns>number of alphabet</returns>
		public static int GetAlphaNumber(string id)
		{
			if (string.IsNullOrEmpty(id) || id.Length < 1 || id.Any(c => c < 'A' || c > 'Z'))
			{
				throw new ArgumentException("cannot convert from empty id into number", "id");
			}
			else
			{
				// fixed incorrect result in 0.8.4 by Bill Woodruff
				int idx = id[0] - AlphaChars[0] + 1;
				for (int i = 1; i < id.Length; i++)
				{
					idx *= AlphaChars.Length;
					idx += id[i] - AlphaChars[0] + 1;
				}
				return idx - 1;
			}
		}

		/// <summary>
		/// Parse tabbed string into regular array
		/// </summary>
		/// <param name="str">string to be parsed</param>
		/// <returns>parsed regular array</returns>
		public static object[,] ParseTabbedString(string str)
		{
			int rows = 0, cols = 0;
			
			string[] lines = str.Split(new string[] { "\n" }, StringSplitOptions.None);

			for (int r = 0; r < lines.Length; r++)
			{
				string line = lines[r];
				
				if (line.EndsWith("\n")) line = line.Substring(0, line.Length - 1);
				if (line.EndsWith("\r")) line = line.Substring(0, line.Length - 1);

				//if (line.Length > 0)
				//{
					string[] tabs = line.Split('\t');
					cols = Math.Max(cols, tabs.Length);
					rows++;
				//}
			}

			object[,] arr = new object[rows, cols];

			for (int r = 0; r < lines.Length; r++)
			{
				string line = lines[r];
			
				if (line.EndsWith("\n")) line = line.Substring(0, line.Length - 1);
				if (line.EndsWith("\r")) line = line.Substring(0, line.Length - 1);
				//line = line.Trim();

				if (line.Length > 0)
				{
					string[] tabs = line.Split('\t');
					cols = Math.Max(cols, tabs.Length);

					for (int c = 0; c < tabs.Length; c++)
					{
						string text = tabs[c];
						if (text.StartsWith("\"")) text = text.Substring(1);
						if (text.EndsWith("\"")) text = text.Substring(0, text.Length - 1);

						arr[r, c] = text;
					}
					rows++;
				}
			}

			return arr;
		}
	}
}
