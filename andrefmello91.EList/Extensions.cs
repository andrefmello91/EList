#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;

namespace andrefmello91.EList
{
	/// <summary>
	///     <see cref="EList{T}" /> extensions.
	/// </summary>
	public static class Extensions
	{

		#region Methods

		/// <summary>
		///     Creates an <see cref="EList{T}" /> based in an <see cref="IEnumerable{T}" />.
		/// </summary>
		/// <inheritdoc cref="IEList{T}" />
		/// <param name="collection">The collection to transform.</param>
		public static EList<T>? ToEList<T>(this IEnumerable<T?>? collection)
			where T : IEquatable<T>, IComparable<T> =>
			collection is null
				? null
				: !collection.Any()
					? new EList<T>()
					: new EList<T>(collection);

		#endregion

	}
}