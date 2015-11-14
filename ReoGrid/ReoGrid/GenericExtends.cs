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

using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace unvell.ReoGrid
{
	public static class GenericExtends
	{
		public static bool Has(this PlainStyleFlag flag, PlainStyleFlag target)
		{
			return (flag & target) == target;
		}

		public static bool HasAny(this PlainStyleFlag flag, PlainStyleFlag target)
		{
			return (flag & target) > 0;
		}

		public static void Set(this PlainStyleFlag flag, PlainStyleFlag target)
		{
			flag |= target;
		}

		public static void Unset(this PlainStyleFlag flag, PlainStyleFlag target)
		{
			flag &= ~target;
		}

		/// <summary>
		/// Comparing 2 Generic Dictionary Instances
		/// quoted from LukeH
		/// http://stackoverflow.com/questions/3928822/comparing-2-dictionarystring-string-instances
		/// </summary>
		public static bool DictionaryEquals<TKey, TValue>(
		this IDictionary<TKey, TValue> first, IDictionary<TKey, TValue> second)
		{
			if (first == second) return true;
			if ((first == null) || (second == null)) return false;
			if (first.Count != second.Count) return false;

			var comparer = EqualityComparer<TValue>.Default;

			foreach (KeyValuePair<TKey, TValue> kvp in first)
			{
				TValue secondValue;
				if (!second.TryGetValue(kvp.Key, out secondValue)) return false;
				if (!comparer.Equals(kvp.Value, secondValue)) return false;
			}
			return true;
		}
	}
}
