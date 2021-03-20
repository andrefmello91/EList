using System;
using System.Collections.Generic;
using System.Linq;
#nullable enable

namespace andrefmello91.EList
{
	/// <summary>
	///     Extended list class with events. Implementation by CodeProject:
	///     <para>https://www.codeproject.com/Articles/31539/List-With-Events</para>
	/// </summary>
	/// <inheritdoc cref="IEList{T}"/>
	public class EList<T> : List<T>, IEList<T>
		where T : IEquatable<T>, IComparable<T>
	{
		#region Events

		/// <summary>
		///     Event to run when the list count changes.
		/// </summary>
		public event EventHandler<CountChangedEventArgs>? CountChanged;

		/// <summary>
		///     Event to run when an item is added.
		/// </summary>
		public event EventHandler<ItemEventArgs<T>>? ItemAdded;

		/// <summary>
		///     Event to run when an item is removed.
		/// </summary>
		public event EventHandler<ItemEventArgs<T>>? ItemRemoved;

		/// <summary>
		///     Event to run when a range of items is added.
		/// </summary>
		public event EventHandler<RangeEventArgs<T>>? RangeAdded;

		/// <summary>
		///     Event to run when a range of items is removed.
		/// </summary>
		public event EventHandler<RangeEventArgs<T>>? RangeRemoved;

		/// <summary>
		///     Event to run when the list is sorted.
		/// </summary>
		public event EventHandler? ListSorted;

		#endregion

		#region Properties

		/// <summary>
		///     Allow duplicate items in this list.
		/// </summary>
		/// <remarks>
		///     If false, an item is not added if this list contains it.
		/// </remarks>
		public bool AllowDuplicates { get; set; }

		/// <summary>
		///     Allow null items in this list.
		/// </summary>
		/// <remarks>
		///     If false, an item is not added if it is null.
		/// </remarks>
		public bool AllowNull { get; set; }

		#endregion

		#region Constructors

		/// <inheritdoc cref="EList{T}(bool, bool)"/>
		public EList()
			: this (false)
		{
		}

		/// <summary>
		///     Create a new empty <see cref="EList{T}" />.
		/// </summary>
		/// <param name="allowDuplicates">Allow duplicate items in this list?</param>
		/// <param name="allowNull">Allow null items in this list?</param>
		public EList(bool allowDuplicates = false, bool allowNull = false)
			: base()
		{
			AllowDuplicates = allowDuplicates;
			AllowNull       = allowNull;
		}

		/// <summary>
		///     Create a new <see cref="EList{T}" /> from a <paramref name="collection" />.
		/// </summary>
		/// <inheritdoc cref="EList{T}(bool, bool)"/>
		public EList(IEnumerable<T> collection, bool allowDuplicates = false, bool allowNull = false)
			: this(allowDuplicates, allowNull) =>
			AddRange(collection, false, false);

		#endregion

		#region  Methods

		public bool Add(T item, bool raiseEvents = true, bool sort = true)
		{
			if (!AllowDuplicates && Contains(item) || !AllowNull && item is null)
				return false;

			base.Add(item);

			if (raiseEvents)
			{
				RaiseCountEvent(CountChanged);
				RaiseItemEvent(ItemAdded, item);
			}
			
			if (sort)
				Sort(raiseEvents);

			return true;
		}

		public int AddRange(IEnumerable<T>? collection, bool raiseEvents = true, bool sort = true)
		{
			if (collection is null || !collection.Any())
				return 0;

			// Add each permitted item
			var added = collection.Where(item => Add(item, false, false)).ToList();

			//// Check duplicates
			//var toAdd =
			//(
			//	AllowDuplicates
			//		? collection
			//		: collection.Distinct().Where(t => !Contains(t))
			//).ToList();

			//// Check null
			//toAdd = AllowNull
			//	? toAdd
			//	: toAdd.Where(t => !(t is null)).ToList();

			//base.AddRange(toAdd);

			if (raiseEvents)
			{
				RaiseCountEvent(CountChanged);
				RaiseRangeEvent(RangeAdded, added);
			}

			if (sort)
				Sort(raiseEvents);

			return added.Count;
		}

		public void Clear(bool raiseEvents = true)
		{
			// Get the initial collection
			var list = this.ToList();

			base.Clear();

			if (!raiseEvents)
				return;

			RaiseCountEvent(CountChanged);
			RaiseRangeEvent(RangeRemoved, list);
		}

		public bool Remove(T item, bool raiseEvents = true, bool sort = true)
		{
			var index = IndexOf(item);

			if (!base.Remove(item))
				return false;

			if (raiseEvents)
			{
				RaiseCountEvent(CountChanged);
				RaiseItemEvent(ItemRemoved, item, index);
			}

			if (sort)
				Sort(raiseEvents);

			return true;
		}

		public int RemoveAll(Predicate<T> match, bool raiseEvents = true, bool sort = true)
		{
			// Get the items
			var removed = this.Where(t => match(t)).ToList();

			var count = base.RemoveAll(match);

			if (raiseEvents)
			{
				RaiseRangeEvent(RangeRemoved, removed);
				RaiseCountEvent(CountChanged);
			}

			if (sort)
				Sort(raiseEvents);

			return count;
		}

		public int RemoveRange(IEnumerable<T>? collection, bool raiseEvents = true, bool sort = true) =>
			collection is null || !collection.Any()
				? 0
				: RemoveAll(collection.Contains, raiseEvents, sort);

		public void Sort(bool raiseEvents = true)
		{
			base.Sort();

			if (raiseEvents)
				RaiseSortEvent(ListSorted);
		}

		public void Sort(IComparer<T> comparer, bool raiseEvents = true)
		{
			base.Sort(comparer);

			if (raiseEvents)
				RaiseSortEvent(ListSorted);
		}

		/// <summary>
		///     Raise the count event.
		/// </summary>
		private void RaiseCountEvent(EventHandler<CountChangedEventArgs>? eventHandler)
		{
			// Copy to a temporary variable to be thread-safe (MSDN).
			var tmp = eventHandler;
			tmp?.Invoke(this, new CountChangedEventArgs(Count));
		}

		/// <summary>
		///     Raise the item event.
		/// </summary>
		private void RaiseItemEvent(EventHandler<ItemEventArgs<T>>? eventHandler, T item, int? index = null)
		{
			// Copy to a temporary variable to be thread-safe (MSDN).
			var tmp = eventHandler;
			tmp?.Invoke(this, new ItemEventArgs<T>(item, index ?? Count - 1));
		}

		/// <summary>
		///     Raise the range event.
		/// </summary>
		private void RaiseRangeEvent(EventHandler<RangeEventArgs<T>>? eventHandler, IEnumerable<T> collection)
		{
			// Copy to a temporary variable to be thread-safe (MSDN).
			var tmp = eventHandler;
			tmp?.Invoke(this, new RangeEventArgs<T>(collection));
		}

		/// <summary>
		///     Raise the sort event.
		/// </summary>
		private void RaiseSortEvent(EventHandler? eventHandler)
		{
			// Copy to a temporary variable to be thread-safe (MSDN).
			var tmp = eventHandler;
			tmp?.Invoke(this, new EventArgs());
		}

		#endregion

		//---------------------------------------------------------------
		//------------------------------------------------------------------
	}
}