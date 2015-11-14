/*****************************************************************************
 * 
 * ReoGrid - .NET Spreadsheet Control
 * 
 * http://reogrid.codeplex.com/
 *
 * PrintHelper
 * 
 *  - Define paper size
 *  - Help Application to calculate paper size and perform print
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
using System.Drawing;

namespace unvell.Common.Printing
{
	public class PrintHelper
	{
		public static readonly Dictionary<PaperSize, SizeF> PageSizeListMM = new Dictionary<PaperSize, SizeF>() {
			{PaperSize.A0, new SizeF(841, 1189)},
			{PaperSize.A1, new SizeF(594, 841)},
			{PaperSize.A2, new SizeF(420, 594)},
			{PaperSize.A3, new SizeF(297, 420)},
			{PaperSize.A4, new SizeF(210, 297)},
			{PaperSize.A5, new SizeF(148.5f, 210)},
			{PaperSize.A6, new SizeF(105f, 148.5f)},
			{PaperSize.A7, new SizeF(74f, 105f)},
			{PaperSize.A8, new SizeF(52f, 74f)},
			{PaperSize.A9, new SizeF(37f, 52f)},
			{PaperSize.A10, new SizeF(26f, 37f)},
			
			{PaperSize.B0, new SizeF(1000, 1414)},
			{PaperSize.B1, new SizeF(707, 1000)},
			{PaperSize.B2, new SizeF(500, 707)},
			{PaperSize.B3, new SizeF(353, 500)},
			{PaperSize.B4, new SizeF(250, 353)},
			{PaperSize.B5, new SizeF(176, 250)},
			{PaperSize.B6, new SizeF(125, 176)},
			{PaperSize.B7, new SizeF(88, 125)},
			{PaperSize.B8, new SizeF(62, 88)},
			{PaperSize.B9, new SizeF(44, 62)},
			{PaperSize.B10, new SizeF(31, 44)},
			{PaperSize.Letter, new SizeF(215.9f, 279.4f)},
			{PaperSize.GovernmentLetter, new SizeF(203.2f, 266.7f)},
			
			{PaperSize.JIS_B0, new SizeF(1030, 1456)},
			{PaperSize.JIS_B1, new SizeF(728, 1030)},
			{PaperSize.JIS_B2, new SizeF(515, 728)},
			{PaperSize.JIS_B3, new SizeF(364, 515)},
			{PaperSize.JIS_B4, new SizeF(257, 364)},
			{PaperSize.JIS_B5, new SizeF(182, 257)},
			{PaperSize.JIS_B6, new SizeF(128, 182)},
			{PaperSize.JIS_B7, new SizeF(91, 128)},
			{PaperSize.JIS_B8, new SizeF(64, 91)},
			{PaperSize.JIS_B9, new SizeF(45, 64)},
			{PaperSize.JIS_B10, new SizeF(32, 45)},
			{PaperSize.JIS_B11, new SizeF(22, 32)},
			{PaperSize.JIS_B12, new SizeF(16, 22)},

			{PaperSize.Custom, new SizeF(210, 297)},
		};

		// a4           210mm x 297mm
		//      inchs   8.2677 in x 11.6929
		public static SizeF GetA4Pixel(Graphics g)
		{
			return new SizeF(g.DpiX * 8.2677f, g.DpiY * 11.6929f);
		}

	}
	
	// ISO Paper Size
	public enum PaperSize
	{
		Custom,
		A0, A1, A2, A3, A4, A5, A6, A7, A8, A9, A10,
		B0, B1, B2, B3, B4, B5, B6, B7, B8, B9, B10,

		JIS_B0, JIS_B1, JIS_B2, JIS_B3, JIS_B4, JIS_B5,
		JIS_B6, JIS_B7, JIS_B8, JIS_B9, JIS_B10, JIS_B11, JIS_B12,

		Letter, GovernmentLetter,
	}

	public class PaperSizeInfo
	{
		public PaperSize Key { get; set; }
		public float Width { get; set; }
		public float Height { get; set; }

		public PaperSizeInfo(PaperSize key, float width, float height)
		{
			this.Key = key;
			this.Width = width;
			this.Height = height;
		}

		public override string ToString()
		{
			return string.Format("{0} ({1}mm x {2}mm)", Key, Width, Height);
		}
	}
}
