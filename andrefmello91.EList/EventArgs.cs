using System;
using System.Collections.Generic;
using System.Linq;

namespace andrefmello91.EList
{
	/// <summary>
	///     Item EventArgs.
	/// </summary>
	public class ItemEventArgs<T> : EventArgs
	{
		#region Properties

		/// <summary>
		///     The index of <see cref="Item" />.
		/// </summary>
		public int Index { get; set; }

		/// <summary>
		///     The item.
		/// </summary>
		public T Item { get; set; }

		#endregion

		#region Constructors

		//----------------------------------------------------------
		public ItemEventArgs(T item, int index = -1)
		{
			Item  = item;
			Index = index;
		}

		#endregion
	}

	/// <summary>
	///     Range EventArgs.
	/// </summary>
	public class RangeEventArgs<T> : EventArgs
	{
		#region Properties

		/// <summary>
		///     The item collection.
		/// </summary>
		public List<T> ItemCollection { get; set; }

		#endregion

		#region Constructors

		//----------------------------------------------------------
		public RangeEventArgs(IEnumerable<T> collection) => ItemCollection = collection.ToList();

		#endregion
	}

	/// <summary>
	///     Count changed EventArgs.
	/// </summary>
	public class CountChangedEventArgs : EventArgs
	{
		#region Properties

		/// <summary>
		///     Number of elements in the list.
		/// </summary>
		public int Count { get; set; }

		#endregion

		#region Constructors

		//----------------------------------------------------------
		public CountChangedEventArgs(int count) => Count = count;

		#endregion
	}
}