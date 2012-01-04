using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using Uri = Android.Net.Uri;

namespace Xamarin
{
	internal interface ITableFinder
	{
		Uri Find (Expression expression, StringBuilder queryBuilder, IList<string> arguments);
		bool IsSupportedType (Type type);
	}
}