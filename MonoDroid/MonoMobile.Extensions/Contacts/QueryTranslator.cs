using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Android.Provider;

namespace Xamarin.Contacts
{
	/*
	 * Find first Where/Select/Order to determine table
	 * Generate args to ContentResover.Query
	 * Caller replaces queryable with enumerable created from query
	 * Query should be for IDs to be filled as loaded
	 */

	internal class QueryTranslator
		: ExpressionVisitor
	{
		public QueryTranslator (bool rawContacts)
		{
			this.rawContacts = rawContacts;
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
			get { return (this.queryBuilder.Length > 0) ? this.sortBuilder.ToString() : null; }
		}

		public string[] ClauseParameters
		{
			get { return (this.parameters.Count > 0) ? this.parameters.ToArray() : null; }
		}

		public string SortString
		{
			get { return (this.sortBuilder != null) ? this.sortBuilder.ToString() : null; }
		} 

		protected override Expression VisitMethodCall (MethodCallExpression methodCall)
		{
			methodCall = (MethodCallExpression)base.VisitMethodCall (methodCall); // go inner first

			if (!this.fallback)
			{
				if (methodCall.Method.Name == "Where")
					methodCall = VisitWhere (methodCall);
				else if (methodCall.Method.Name == "Any")
					methodCall = VisitAny (methodCall);
				else if (methodCall.Method.Name == "Select" || methodCall.Method.Name == "SelectMany")
					methodCall = VisitSelect (methodCall);
				else if (methodCall.Method.Name == "OrderBy" || methodCall.Method.Name == "OrderBy")
					methodCall = VisitOrder (methodCall);
			}
			
			return methodCall;
		}

		private readonly bool rawContacts;

		private bool fallback = false;
		private List<string> columns;
		private StringBuilder sortBuilder;
		private readonly List<string> parameters = new List<string>();
		private readonly StringBuilder queryBuilder = new StringBuilder();

		private MethodCallExpression VisitAny (MethodCallExpression methodCall)
		{
			return methodCall;
		}

		private MethodCallExpression VisitWhere (MethodCallExpression methodCall)
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

		private MethodCallExpression VisitSelect (MethodCallExpression methodCall)
		{
			return methodCall;
		}

		private MethodCallExpression VisitOrder (MethodCallExpression methodCall)
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
				if (methodCall.Method.Name == "OrderByDesending")
					builder.Append (" DESC");

				return null;
			}

			return methodCall;
		}

		private bool TryGetTable (MemberExpression me)
		{
			Android.Net.Uri table = TableFinder.Find (me, this.rawContacts, this.queryBuilder, this.parameters);

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
			if (me != null && IsSupportedType (me.Member.DeclaringType))
				return me;

			BinaryExpression be = expression as BinaryExpression;
			if (be != null)
			{
				me = be.Left as MemberExpression;
				if (me != null && IsSupportedType (me.Member.DeclaringType))
					return me;

				me = be.Right as MemberExpression;
				if (me != null && IsSupportedType (me.Member.DeclaringType))
					return me;
			}

			return null;
		}

		private bool IsSupportedType (Type type)
		{
			return type == typeof(Contact) || type == typeof(Phone) || type == typeof (Email);
		}
	}
}