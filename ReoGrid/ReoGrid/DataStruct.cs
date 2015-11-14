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

namespace unvell.ReoGrid.Data
{
	#region Triangle Tree Array
	[Serializable]
	public sealed class TriangleTreeArray<T>
	{
		public static readonly int RowSize = 64;
		public static readonly int ColSize = 16;

		private T[,][,][,] data = new T[RowSize, ColSize][,][,];

		public T this[int row, int col]
		{
			get
			{
				int r = row >> 12;
				int c = col >> 8;
				T[,][,] page1 = data[r, c];
				if (page1 == null) return default(T);

				int r2 = (row >> 6) % RowSize;
				int c2 = (col >> 4) % ColSize;
				T[,] page2 = page1[r2, c2];

				return page2 == null ? default(T) : page2[row % RowSize, col % ColSize];
			}
			set
			{
				int r = row >> 12;
				int c = col >> 8;
				if (data[r, c] == null)
				{
					if (value == null)
						return;
					else
						data[r, c] = new T[RowSize, ColSize][,];
				}

				int r2 = (row >> 6) % RowSize;
				int c2 = (col >> 4) % ColSize;
				if (data[r, c][r2, c2] == null)
					if (value == null)
						return;
					else
					{
						data[r, c][r2, c2] = new T[RowSize, ColSize];
					}

				data[r, c][r2, c2][row % RowSize, col % ColSize] = value;
			}
		}

		public bool IsPageNull(int row, int col)
		{
			int r = row >> 12;
			int c = col >> 8;
			T[,][,] page1 = data[r, c];
			if (page1 == null) return true;

			int r2 = (row >> 6) % RowSize;
			int c2 = (col >> 4) % ColSize;
			return page1[r2, c2] == null;
		}

		public int Rows { get { return RowSize << 12; } }
		public int Cols { get { return ColSize << 8; } }

		public void Iterate(int row, int col, int rows, int cols, bool ignoreNull, Func<int, int, T, bool> iterator)
		{
			int r2 = row + rows;
			int c2 = col + cols;
			for (int r = row; r < r2; r++)
			{
				for (int c = col; c < c2; )
				{
					if (IsPageNull(r, c))
					{
						c += (ColSize - (c % ColSize));
					}
					else
					{
						if (!iterator(r, c, this[r, c])) return;
						c++;
					}
				}
			}
		}
	}
	#endregion

	#region Dictionary Tree Array
	[Serializable]
	public sealed class DictionaryTreeArray<T>
	{
		public static readonly int RowSize = 64;
		public static readonly int ColSize = 16;

		private Dictionary<long, T[,][,]> data = new Dictionary<long, T[,][,]>();

		public T this[int row, int col]
		{
			get
			{
				long key = ((row >> 12) << 16) | (col >> 8);
				if (!data.ContainsKey(key)) return default(T);

				int r2 = (row >> 6) % RowSize;
				int c2 = (col >> 4) % ColSize;
				T[,] page2 = data[key][r2, c2];

				return page2 == null ? default(T) : page2[row % RowSize, col % ColSize];
			}
			set
			{
				long key = ((row >> 12) << 16) | (col >> 8);

				T[,][,] page = null;

				if (!data.ContainsKey(key))
					if (value == null)
						return;
					else
						data.Add(key, page = new T[RowSize, ColSize][,]);
				else
					page = data[key];

				int r2 = (row >> 6) % RowSize;
				int c2 = (col >> 4) % ColSize;
				if (page[r2, c2] == null)
					if (value == null)
						return;
					else
						page[r2, c2] = new T[RowSize, ColSize];

				page[r2, c2][row % RowSize, col % ColSize] = value;
			}
		}
	}
	#endregion

	#region Dictionary Regular Array
	/// <summary>
	/// Implementation of integer-indexed two-dimensional dictionary array.
	/// (up to 1048576 x 1048576 elements)
	/// </summary>
	/// <typeparam name="T">Any type as element in array</typeparam>
	[Serializable]
	public sealed class DictionaryRegularArray<T>
	{
		public static readonly int RowSize = 16;
		public static readonly int ColSize = 16;

		private Dictionary<long, T[,][,]> data = new Dictionary<long, T[,][,]>();

		public T this[int row, int col]
		{
			get
			{
				long key = ((row >> 12) << 16) | (col >> 8);
				if (!data.ContainsKey(key)) return default(T);

				int r2 = (row >> 6) % RowSize;
				int c2 = (col >> 4) % ColSize;
				T[,] page2 = data[key][r2, c2];

				return page2 == null ? default(T) : page2[row % RowSize, col % ColSize];
			}
			set
			{
				long key = ((row >> 12) << 16) | (col >> 8);

				T[,][,] page = null;

				if (!data.ContainsKey(key))
					if (value == null)
						return;
					else
						data.Add(key, page = new T[RowSize, ColSize][,]);
				else
					page = data[key];

				int r2 = (row >> 6) % RowSize;
				int c2 = (col >> 4) % ColSize;
				if (page[r2, c2] == null)
					if (value == null)
						return;
					else
						page[r2, c2] = new T[RowSize, ColSize];

				page[r2, c2][row % RowSize, col % ColSize] = value;
			}
		}
	}
	#endregion

	#region Regular Tree Array
	/// <summary>
	/// Implementation of page-indexed two-dimensional regular array.
	/// (up to 1048576 x 32768 elements)
	/// </summary>
	/// <typeparam name="T">Any type as element in array</typeparam>
	[Serializable]
	public sealed class RegularTreeArray<T>
	{
		public const int RowSize = 128;
		public const int RowBitLen = 7;
		public const int ColSize = 32;
		public const int ColBitLen = 5;
		public const int MaxDepth = 3;

		private Node root = new Node();

		public RegularTreeArray()
		{
			root.nodes = new Node[RowSize, ColSize];
		}

		private int maxRow, maxCol;
		public int MaxRow { get { return maxRow; } set { maxRow = value; } }
		public int MaxCol { get { return maxCol; } set { maxCol = value; } }

		public int ContentBoundCol { get { return maxCol; } }
		public int ContentBoundRow { get { return maxRow; } }

		public T this[int row, int col]
		{
get
{
	Node node = root;

	for (int d = MaxDepth - 1; d > 0; d--)
	{
		int r = (row >> (RowBitLen * d)) % RowSize;
		int c = (col >> (ColBitLen * d)) % ColSize;

		node = node.nodes[r, c];

		if (node == null) return default(T);
	}

	return node.data[row % RowSize, col % ColSize];
}
set
{
	Node node = root;

	for (int d = MaxDepth - 1; d > 0; d--)
	{
		int r = (row >> (RowBitLen * d)) % RowSize;
		int c = (col >> (ColBitLen * d)) % ColSize;

		Node child = node.nodes[r, c];

		if (child == null)
		{
			if (value == null)
				return;
			else
			{
				child = node.nodes[r, c] = new Node();
				if (d > 1) child.nodes = new Node[RowSize, ColSize];
			}
		}

		node = child;
	}

	if (node.data == null)
	{
		if (value == null)
			return;
		else
			node.data = new T[RowSize, ColSize];
	}

	node.data[row % RowSize, col % ColSize] = value;

	if (value != null)
	{
		if (row > maxRow) maxRow = row;
		if (col > maxCol) maxCol = col;
	}
}
		}

		public long RowCapacity { get { return (long)Math.Pow(RowSize, MaxDepth); } }
		public long ColCapacity { get { return (long)Math.Pow(ColSize, MaxDepth); } }

		[Serializable]
		public sealed class Node
		{
			internal Node[,] nodes;
			internal T[,] data;
		}

		public bool IsPageNull(int row, int col)
		{
			Node node = root;

			for (int d = MaxDepth - 1; d > 0; d--)
			{
				int r = (row >> (RowBitLen * d)) % RowSize;
				int c = (col >> (ColBitLen * d)) % ColSize;

				node = node.nodes[r, c];

				if (node == null) return true;
			}

			return node.data == null;
		}

		public void IterateContent(Func<int, int, T, int> iterator)
		{
			Iterate(0, 0, maxRow + 1, maxCol + 1, true, iterator);
		}

		public void Iterate(int startRow, int startCol, int rows, int cols, bool ignoreNull, Func<int, int, T, int> iterator)
		{
			int r2 = startRow + rows;
			int c2 = startCol + cols;

			for (int r = startRow; r < r2; r++)
			{
				for (int c = startCol; c < c2; )
				{
					if (IsPageNull(r, c))
					{
						c += (ColSize - (c % ColSize));
					}
					else
					{
						T obj = this[r, c];
						int cspan = 1;
						if (!ignoreNull || obj != null)
						{
							cspan = iterator(r, c, obj);
							if (cspan <= 0) return;
						}
						c += cspan;
					}
				}
			}
		}

		internal void IterateEx(int startRow, int startCol, int endRow, int endCol, Func<int, int, bool> isTarget, Func<int, int, T, int> iterator)
		{
			for (int r = startRow; r < endRow; r++)
			{
				for (int c = startCol; c < endCol; )
				{
					if (IsPageNull(r, c))
					{
						c += (ColSize - (c % ColSize));
					}
					else if (!isTarget(r, c))
					{
						c++;
					}
					else
					{
						T obj = this[r, c];
						int cspan = 1;
						if (obj != null)
						{
							cspan = iterator(r, c, obj);
							if (cspan <= 0) return;
						}
						c += cspan;
					}
				}
			}
		}


		private bool IterateTree(Node node, int offrow, int offcol, Func<int, int, T, bool> handler)
		{
			if (node.nodes != null)
			{
				for (int r = RowSize; r >= 0; r--)
				{
					for (int c = ColSize; c >= 0; c--)
					{
						bool ret = IterateTree(node.nodes[r, c], offrow + r, offcol + c, handler);
						if (!ret) return false;
					}
				}
			}

			if (node.data != null)
			{
				for (int r = RowSize; r >= 0; r--)
				{
					for (int c = ColSize; c >= 0; c--)
					{
						bool rs = handler(offrow + r, offcol + c, node.data[r, c]);
						if (!rs) return false;
					}
				}
			}

			return true;
		}

		public void FindContentBounds(out int row, out int col)
		{
			int r2 = maxRow;
			int c2 = maxCol;

			for (int r = r2; r >= 0; r--)
			{
				for (int c = c2; c >= 0; c--)
				{
					if (IsPageNull(r, c))
					{
						c -= (c % ColSize);
					}
					else
					{
						T obj = this[r, c];
						if (obj != null)
						{
							row = r;
							col = c;
							return;
						}
					}
				}
			}

			row = 0;
			col = 0;
			return;
		}
	}

	#endregion

	#region Tree Array
	/// <summary>
	/// Implementation of page-indexed one-dimensional array.
	/// (up to 1048576 elements @ 16^5)
	/// </summary>
	/// <typeparam name="T">Any type as element in array</typeparam>
	[Serializable]
	public sealed class TreeArray<T>
	{
		private int nodeSize = 16;
		private int maxDepth = 5;

		private long capacity;

		public TreeArray() : this(16, 5) { }

		public TreeArray(int nodeSize, int maxDepth)
		{
			this.nodeSize = nodeSize;
			this.maxDepth = maxDepth;
			this.capacity = (long)Math.Pow(nodeSize, maxDepth);
		}

		private Node root = new Node();

		public T this[int index]
		{
			get
			{
				Node node = root;

				for (int d = maxDepth; d > 0; d--)
				{
					int i = (index >> (4 * d)) % nodeSize;

					node = node.nodes[i];

					if (node == null) return default(T);
				}

				return node.data[index % nodeSize];
			}
			set
			{
				Node node = root;

				for (int d = maxDepth; d > 0; d--)
				{
					int i = (index >> (4 * d)) % nodeSize;

					Node child = node.nodes[i];

					if (child == null)
					{
						if (value == null)
							return;
						else
						{
							child = node.nodes[i] = new Node();
							if (d > 0) child.nodes = new Node[nodeSize];
						}
					}

					node = child;
				}

				if (node.data == null)
				{
					if (value == null)
						return;
					else
						node.data = new T[nodeSize];
				}

				node.data[index % nodeSize] = value;
			}
		}

		public void RemoveAt(int index)
		{
			//Node node = root;

			//for (int d = MaxDepth; d > 0; d--)
			//{
			//  int i = (index >> (4 * d)) % NodeSize;

			//  Node child = node.nodes[i];

			//  if (child == null)
			//    return;

			//  node = child;
			//}

			// todo

			this[index] = default(T);
		}

		public long Capacity { get { return capacity; } }

		[Serializable]
		public sealed class Node
		{
			internal Node[] nodes;
			internal T[] data = null;
		}

		public bool IsPageNull(int index)
		{
			Node node = root;

			for (int d = maxDepth; d > 0; d--)
			{
				int i = (index >> (4 * d)) % nodeSize;

				node = node.nodes[i];

				if (node == null) return true;
			}

			return node.data == null;
		}

		public void Iterate(int index, int count, bool ignoreNull, Func<int, T, bool> iterator)
		{
			int end = index + count;
			if (end > capacity) end = (int)(capacity);

			for (int i = index; i < end; )
			{
				if (IsPageNull(i))
				{
					i += (nodeSize - (i % nodeSize));
				}
				else
				{
					if (!iterator(i, this[i])) return;
					i++;
				}
			}
		}

		public void IterateReverse(int index, int count, bool ignoreNull, Func<int, T, bool> iterator)
		{
			int end = index - count;
			if (end <= -1) end = -1;

			for (int i = index; i > end; )
			{
				if (IsPageNull(i))
				{
					i -= (i % nodeSize);
				}
				else
				{
					if (!iterator(i, this[i])) return;
					i--;
				}
			}
		}

		public void RemoveRange(int index, int count)
		{
			int end = index + count;

			for (int i = index; i < end; )
			{
				if (IsPageNull(i))
				{
					i += (nodeSize - (i % nodeSize));
				}
				else
				{
					this[i] = default(T);
					i++;
				}
			}
		}
	}
	#endregion

	#region Array Helper
	/// <summary>
	/// Generic Array Utility
	/// </summary>
	public sealed class ArrayHelper
	{
		public static int QuickFind(int start, int end, Func<int, int> compare)
		{
			return QuickFind((end - start) >> 1, start, end, compare);
		}

		public static int QuickFind(int split, int start, int end, Func<int, int> compare)
		{
			if (split == start || split == end) return split;

			int r = compare(split);

			if (r == 0)
			{
				return split;
			}
			else if (r < 0)
			{
				int p = (split - start) >> 1;
				return QuickFind(split - p, start, split, compare);
			}
			else if (r > 0)
			{
				int p = (end - split) >> 1;
				return QuickFind(split + p, split, end, compare);
			}

			return -1;
		}
	}
	#endregion

	#region Tree
	public sealed class SimpleTree<T>
	{
		public TreeNode<T> Root { get; set; }

		public class TreeNode<T>
		{
			public TreeNode<T> Childrens { get; set; }

			public List<T> Elements { get; set; }
		}
	}
	#endregion

}
