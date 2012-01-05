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
		Uri Find (Expression expression, StringBuilder queryBuilder, IList<string> arguments);

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
		Tuple<string, Type> GetColumn (MemberInfo memberInfo);
	}
}