using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Uri = Android.Net.Uri;

namespace Xamarin
{
	internal interface ITableFinder
	{
		/// <summary>
		/// Gets the default table (content hierarchy root).
		/// </summary>
		Uri DefaultTable { get; }

		TableFindResult Find (Expression expression);

		/// <summary>
		/// Gets whether the <paramref name="type"/> is a supported type for this finder.
		/// </summary>
		/// <param name="type">The type to check.</param>
		/// <returns><c>true</c> if the <paramref name="type"/> is supported, <c>false</c> otherwise.</returns>
		bool IsSupportedType (Type type);

		/// <summary>
		/// Gets the Android column name for the model's member.
		/// </summary>
		/// <param name="memberInfo">The <see cref="MemberInfo"/> for the model's member.</param>
		/// <returns>Android column name for the model's member, <c>null</c> if unknown.</returns>
		ContentResolverColumnMapping GetColumn (MemberInfo memberInfo);
	}

	internal class TableFindResult
	{
		internal TableFindResult (Uri table, string queryString, IEnumerable<string> arguments)
		{
			Table = table;
			QueryString = queryString;
			Arguments = arguments;
		}

		public Uri Table
		{
			get;
			private set;
		}

		public string QueryString
		{
			get;
			private set;
		}

		public IEnumerable<string> Arguments
		{
			get;
			private set;
		}
	}
}