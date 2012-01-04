using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Android.Provider;

namespace Xamarin
{
	/*
	 * Find first Where/Select/Order to determine table
	 * Generate args to ContentResover.Query
	 * Caller replaces queryable with enumerable created from query
	 * Query should be for IDs to be filled as loaded
	 */

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

		private bool fallback = false;
		private List<string> columns;
		private StringBuilder sortBuilder;
		private readonly List<string> parameters = new List<string>();
		private readonly StringBuilder queryBuilder = new StringBuilder();

		protected override Expression VisitMethodCall (MethodCallExpression methodCall)
		{
			if (this.fallback)
				return methodCall;

			Expression expression = base.VisitMethodCall (methodCall);

			methodCall = expression as MethodCallExpression;
			if (methodCall == null)
				return expression;

			if (methodCall.Method.Name == "Where")
				expression = VisitWhere (methodCall);
			else if (methodCall.Method.Name == "Any")
				expression = VisitAny (methodCall);
			else if (methodCall.Method.Name == "Select" || methodCall.Method.Name == "SelectMany")
				expression = VisitSelect (methodCall);
			else if (methodCall.Method.Name == "OrderBy" || methodCall.Method.Name == "OrderByDescending")
				expression = VisitOrder (methodCall);
			
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
			return methodCall;
		}

		private Expression VisitOrder (MethodCallExpression methodCall)
		{
			MemberExpression me = FindMemberExpression (methodCall.Arguments[1]);
			if (!TryGetTable (me))
				return methodCall;

			string column = GetContactColumn (me.Member);
			if (column != null)
			{
				StringBuilder builder = this.sortBuilder ?? (this.sortBuilder = new StringBuilder());
				if (builder.Length > 0)
					builder.Append (", ");

				builder.Append (column);
				if (methodCall.Method.Name == "OrderByDescending")
					builder.Append (" DESC");

				return methodCall.Arguments[0];
			}

			return methodCall;
		}

		private bool TryGetTable (MemberExpression me)
		{
			Android.Net.Uri table = this.tableFinder.Find (me, this.queryBuilder, this.parameters);

			if (Table == null)
				Table = table;
			else if (Table != table)
			{
				this.fallback = true;
				return false;
			}

			return true;
		}

		private string GetContactColumn (MemberInfo member)
		{
			switch (member.Name)
			{
				case "DisplayName":
					return ContactsContract.ContactsColumns.DisplayName;
				case "Prefix":
					return ContactsContract.CommonDataKinds.StructuredName.Prefix;
				case "FirstName":
					return ContactsContract.CommonDataKinds.StructuredName.GivenName;
				case "LastName":
					return ContactsContract.CommonDataKinds.StructuredName.FamilyName;
				case "Suffix":
					return ContactsContract.CommonDataKinds.StructuredName.Suffix;

				default:
					return null;
			}
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