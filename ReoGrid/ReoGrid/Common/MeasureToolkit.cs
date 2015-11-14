/*****************************************************************************
 * 
 * ReoGrid - .NET Spreadsheet Control
 * 
 * http://reogrid.codeplex.com/
 *
 * MeasureToolkit - Convert between inch, cm and pixel
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

namespace unvell.Common
{
	public static class MeasureToolkit
	{
		// 1 inch = 2.54 cm
		private const float _cmpreinch = 2.54f;
		private const float _windows_standard_dpi = 96f;

		public static float InchToPixel(float inch, float dpi)
		{
			return inch * dpi;
		}
		public static float PixelToInch(float px, float dpi)
		{
			return px / dpi;
		}

		public static float InchToPixel(float inch)
		{
			return InchToPixel(inch, _windows_standard_dpi);
		}
		public static float PixelToInch(float px)
		{
			return PixelToInch(px, _windows_standard_dpi);
		}

		public static float InchToCM(float inch)
		{
			return inch * _cmpreinch;
		}
		public static float CMToInch(float cm)
		{
			return cm / _cmpreinch;
		}

		public static float CMToPixel(float cm, float dpi)
		{
			return cm * dpi / _cmpreinch;
		}
		public static float PixelToCM(float px, float dpi)
		{
			return px * _cmpreinch / dpi;
		}

		public static float CMTOPixel(float cm)
		{
			return CMToPixel(cm, _windows_standard_dpi);
		}
		public static float PixelToCM(float px)
		{
			return PixelToCM(px, _windows_standard_dpi);
		}
	}
}
