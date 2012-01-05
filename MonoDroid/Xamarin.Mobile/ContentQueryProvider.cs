using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Android.Content;
using Android.Content.Res;
using Android.Database;

namespace Xamarin
{
	internal abstract class ContentQueryProvider
		: IQueryProvider
	{
		internal ContentQueryProvider (ContentResolver content, Resources resources, ITableFinder tableFinder)
		{
			this.content = content;
			this.resources = resources;
			this.tableFinder = tableFinder;
		}

		public ITableFinder TableFinder
		{
			get { return this.tableFinder; }
		}

		protected readonly ContentResolver content;
		protected readonly Resources resources;
		private readonly ITableFinder tableFinder;

		IQueryable IQueryProvider.CreateQuery (Expression expression)
		{
			throw new NotImplementedException();
		}

		object IQueryProvider.Execute (Expression expression)
		{
			var translator = new ContentQueryTranslator (this.tableFinder);
			expression = translator.Visit (expression);
			
			IQueryable q = GetObjectReader (this.content, this.resources, translator).AsQueryable();

			//IQueryable<T> q = GetElements().AsQueryable();
			expression = ReplaceQueryable (expression, q);
			
			if (expression.Type.IsGenericType && expression.Type.GetGenericTypeDefinition() == typeof(IOrderedQueryable<>))
				return q.Provider.CreateQuery (expression);
			else
				return q.Provider.Execute (expression);
		}

		protected abstract IEnumerable GetObjectReader (ContentResolver content, Resources resources, ContentQueryTranslator translator);

		IQueryable<TElement> IQueryProvider.CreateQuery<TElement> (Expression expression)
		{
			return new Query<TElement> (this, expression);
		}

		TResult IQueryProvider.Execute<TResult> (Expression expression)
		{
			return (TResult)((IQueryProvider)this).Execute (expression);
		}

		private Expression ReplaceQueryable (Expression expression, object value)
		{
			MethodCallExpression mc = expression as MethodCallExpression;
			if (mc != null)
			{
				Expression[] args = mc.Arguments.ToArray();
				Expression narg = ReplaceQueryable (mc.Arguments[0], value);
				if (narg != args[0])
				{
					args[0] = narg;
					return Expression.Call (mc.Method, args);
				}
				else
					return mc;
			}

			ConstantExpression c = expression as ConstantExpression;
			if (c != null && c.Type.GetInterfaces().Contains (typeof(IQueryable)))
				return Expression.Constant (value);

			return expression;
		}
	}
}