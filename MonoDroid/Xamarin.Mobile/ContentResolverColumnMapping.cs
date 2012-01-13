using System;

namespace Xamarin
{
	internal class ContentResolverColumnMapping
	{
		public ContentResolverColumnMapping (string column, Type returnType)
		{
			if (returnType == null)
				throw new ArgumentNullException ("returnType");

			if (column != null)
				Columns = new [] { column };

			ReturnType = returnType;
		}

		public ContentResolverColumnMapping (string column, Type returnType, Func<object, object> toQueryable, Func<object, object> fromQueryable)
			: this (column, returnType)
		{
			if (toQueryable == null)
				throw new ArgumentNullException ("toQueryable");
			if (fromQueryable == null)
				throw new ArgumentNullException ("fromQueryable");

			ValueToQueryable = toQueryable;
			QueryableToValue = fromQueryable;
		}

		public ContentResolverColumnMapping (string[] columns, Type returnType)
		{
			if (returnType == null)
				throw new ArgumentNullException ("returnType");

			Columns = columns;
			ReturnType = returnType;
		}

		public ContentResolverColumnMapping (string[] columns, Type returnType, Func<object, object> toQueryable, Func<object, object> fromQueryable)
			: this (columns, returnType)
		{
			if (toQueryable == null)
				throw new ArgumentNullException ("toQueryable");
			if (fromQueryable == null)
				throw new ArgumentNullException ("fromQueryable");

			ValueToQueryable = toQueryable;
			QueryableToValue = fromQueryable;
		}

		public string[] Columns
		{
			get;
			private set;
		}

		public Func<object, object> ValueToQueryable
		{
			get;
			private set;
		}

		public Func<object, object> QueryableToValue
		{
			get;
			private set;
		}

		public Type ReturnType
		{
			get;
			private set;
		}
	}
}