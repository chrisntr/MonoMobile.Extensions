using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Xamarin
{
	internal class MemberExpressionFinder
		: ExpressionVisitor
	{
		internal MemberExpressionFinder (ITableFinder tableFinder)
		{
			if (tableFinder == null)
				throw new ArgumentNullException ("tableFinder");

			this.tableFinder = tableFinder;
		}

		private readonly List<MemberExpression> expressions = new List<MemberExpression>();
		private readonly ITableFinder tableFinder;

		protected override Expression VisitMemberAccess (MemberExpression member)
		{
			if (this.tableFinder.IsSupportedType (member.Member.DeclaringType))
				this.expressions.Add (member);

			return base.VisitMemberAccess (member);
		}

		internal static List<MemberExpression> Find (Expression expression, ITableFinder tableFinder)
		{
			if (expression == null)
				throw new ArgumentNullException ("expression");
			if (tableFinder == null)
				throw new ArgumentNullException ("tableFinder");

			var finder = new MemberExpressionFinder (tableFinder);
			finder.Visit (expression);

			return finder.expressions;
		}
	}
}