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

namespace unvell.ReoGrid
{
	#region ReoGridException
	public class ReoGridException : Exception
	{
		public ReoGridException(string msg) : base(msg) { }
	}
	public abstract class ReoGridCellException : Exception
	{
		public ReoGridPos Pos { get; set; }
		public ReoGridCellException(ReoGridPos pos) { this.Pos = pos; }
	}
	public class ReoGridCellNotFoundException : ReoGridCellException
	{
		public ReoGridCellNotFoundException(ReoGridPos pos) : base(pos) { }
	}
	public class RGLoadFileException : ReoGridException
	{
		public RGLoadFileException(string msg) : base(msg) { }
	}
	public class FreezeUnsupportedException : ReoGridException
	{
		public FreezeUnsupportedException() : base("Current view model is not compatible with freeze panel.") { }
	}
	#endregion

	#region NamedRangeNotFoundException
	public class NamedRangeNotFoundException : ReoGridException
	{
		public NamedRangeNotFoundException(string msg) : base(msg) { }
	}
	#endregion

	#region RangeException
	public abstract class RangeException : ReoGridException
	{
		public ReoGridRange Range { get; set; }
		public RangeException(ReoGridRange range) : this(null, range) { }
		public RangeException(string msg, ReoGridRange range)
			: base(msg)
		{
			this.Range = range;
		}
	}
	public class RangeIntersectionException : RangeException
	{
		public RangeIntersectionException(ReoGridRange range) : base(range) { }
	}
	public class NotValidRangeException : RangeException
	{
		public NotValidRangeException(ReoGridRange range) : base(range) { }
	}
	public class RangeTooSmallException : RangeException
	{
		public RangeTooSmallException(ReoGridRange range) : base(range) { }
	}
	#endregion // RangeException

	#region ScriptException
#if EX_SCRIPT
	public class CircularReferenceException : ReoGridException
	{
		public CircularReferenceException()
			: base("The referenced cell of a refereced range contained in the range itself.")
		{
		}
	}
#endif // EX_SCRIPT
	#endregion // ScriptException

	#region Outline Exceptions
	public class OutlineException : ReoGridException
	{
		public int Start { get; set; }
		public int Count { get; set; }
		public int End { get { return Start + Count; } }

		public OutlineException(string msg) : base(msg) { }
	}
	public class OutlineIntersectedException : OutlineException
	{
		public OutlineIntersectedException()
			: base("Specified outline intersects with another one. To group rows make sure that range of row contains others outline completely.")
		{
		}
	}

	public class OutlineAlreadyDefinedException : ReoGridException
	{
		public OutlineAlreadyDefinedException()
			: base("Specified outline does already exist.") { }
	}

	public class OutlineNotFoundException : OutlineException
	{
		public OutlineNotFoundException(int start, string msg) : base(msg) { }
	}

	public class OutlineOutOfRangeException : OutlineException
	{
		public OutlineOutOfRangeException(int start, int count, string msg)
			: base("Specified outline cannot be added because it out of maximum available range.") { }
	}

	public class OutlineTooMuchException : OutlineException
	{
		public OutlineTooMuchException()
			: base("Number of outline levels reached the maximum levels 10.") { }
	}
	#endregion // OutlineIntersectedException
}
