using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Xamarin
{
	internal class ContentQueryTranslator
		: ExpressionVisitor
	{
		private readonly ITableFinder tableFinder;

		public ContentQueryTranslator (ITableFinder tableFinder)
		{
			this.tableFinder = tableFinder;
		}

		public Android.Net.Uri Table
		{
			get;
			private set;
		}

		public Type ReturnType
		{
			get;
			private set;
		}

		public IEnumerable<Tuple<string, Type>> Projections
		{
			get { return this.projections; }
		}

		public string[] Columns
		{
			get { return (this.columns != null) ? this.columns.ToArray() : null; }
		}

		public string QueryString
		{
			get { return (this.queryBuilder.Length > 0) ? this.queryBuilder.ToString() : null; }
		}

		public string[] ClauseParameters
		{
			get { return (this.parameters.Count > 0) ? this.parameters.ToArray() : null; }
		}

		public string SortString
		{
			get { return (this.sortBuilder != null) ? this.sortBuilder.ToString() : null; }
		}

		public Expression Translate (Expression expression)
		{
			Expression expr = Visit (expression);

			if (Table == null)
				Table = this.tableFinder.DefaultTable;

			return expr;
		}

		private bool fallback = false;
		private List<Tuple<string, Type>> projections;
		private List<string> columns;
		private StringBuilder sortBuilder;
		private readonly List<string> parameters = new List<string>();
		private readonly StringBuilder queryBuilder = new StringBuilder();

		protected override Expression VisitMethodCall (MethodCallExpression methodCall)
		{
			Expression expression = base.VisitMethodCall (methodCall);

			methodCall = expression as MethodCallExpression;
			if (methodCall == null)
				return expression;

			if (!this.fallback)
			{
				if (methodCall.Method.Name == "Where")
					expression = VisitWhere (methodCall);
				else if (methodCall.Method.Name == "Any")
					expression = VisitAny (methodCall);
				else if (methodCall.Method.Name == "Select" || methodCall.Method.Name == "SelectMany")
					expression = VisitSelect (methodCall);
				else if (methodCall.Method.Name == "OrderBy" || methodCall.Method.Name == "OrderByDescending")
					expression = VisitOrder (methodCall);
			}

			return expression;
		}

		private MethodCallExpression VisitAny (MethodCallExpression methodCall)
		{
			return methodCall;
		}

		private Expression VisitWhere (MethodCallExpression methodCall)
		{
			// TODO: Need to actually evaluate expression to determine
			// whether we can use it in the query, the operand
			// and any constant values

			//MemberExpression me = FindMemberExpression (methodCall.Arguments[1]);
			//if (!TryGetTable (me))
			//    return methodCall;

			//if (me.Member.DeclaringType == typeof(Contact))
			//{
			//    string column = GetContactColumn (me.Member);
			//    //this.parameters.Add (column);
				
			//    if (this.queryBuilder.Length > 0)
			//        this.queryBuilder.Append (" AND ");

			//    this.queryBuilder.Append (column);
			//    //this.queryBuilder.Append ("=?");
			//}

			return methodCall;
		}

		private Expression VisitSelect (MethodCallExpression methodCall)
		{
			MemberExpression me = FindMemberExpression (methodCall.Arguments[1]);
			if (!TryGetTable (me))
				return methodCall;

			Tuple<string, Type> column = this.tableFinder.GetColumn (me.Member);
			if (column == null)
				return methodCall;

			(this.projections ?? (this.projections = new List<Tuple<string, Type>>())).Add (column);
			if (column.Item2.IsValueType || column.Item2 == typeof(string))
				ReturnType = column.Item2;

			this.fallback = true;

			return methodCall.Arguments[0];
		}

		private Expression VisitOrder (MethodCallExpression methodCall)
		{
			MemberExpression me = FindMemberExpression (methodCall.Arguments[1]);
			if (!TryGetTable (me))
				return methodCall;

			Tuple<string, Type> column = this.tableFinder.GetColumn (me.Member);
			if (column != null)
			{
				StringBuilder builder = this.sortBuilder ?? (this.sortBuilder = new StringBuilder());
				if (builder.Length > 0)
					builder.Append (", ");

				builder.Append (column.Item1);

				if (methodCall.Method.Name == "OrderByDescending")
					builder.Append (" DESC");

				return methodCall.Arguments[0];
			}

			return methodCall;
		}

		private bool TryGetTable (MemberExpression me)
		{
			if (me == null)
			{
				this.fallback = true;
				return false;
			}

			TableFindResult result = this.tableFinder.Find (me);
			if (result.QueryString != null)
				this.queryBuilder.Append (result.QueryString);
			
			this.arguments.AddRange (result.Arguments);

			if (Table == null)
				Table = result.Table;
			else if (Table != result.Table)
			{
				this.fallback = true;
				return false;
			}

			return true;
		}

		private MemberExpression FindMemberExpression (Expression expression)
		{
			UnaryExpression ue = expression as UnaryExpression;
			if (ue != null)
				expression = ue.Operand;

			LambdaExpression le = expression as LambdaExpression;
			if (le != null)
				expression = le.Body;

			MemberExpression me = expression as MemberExpression;
			if (me != null && this.tableFinder.IsSupportedType (me.Member.DeclaringType))
				return me;

			BinaryExpression be = expression as BinaryExpression;
			if (be != null)
			{
				me = be.Left as MemberExpression;
				if (me != null && this.tableFinder.IsSupportedType (me.Member.DeclaringType))
					return me;

				me = be.Right as MemberExpression;
				if (me != null && this.tableFinder.IsSupportedType (me.Member.DeclaringType))
					return me;
			}

			return null;
		}
	}
}