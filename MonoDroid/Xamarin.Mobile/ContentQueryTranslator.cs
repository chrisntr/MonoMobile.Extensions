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
		public ContentQueryTranslator (ITableFinder tableFinder)
		{
			this.tableFinder = tableFinder;
			Skip = -1;
			Take = -1;
		}

		public Android.Net.Uri Table
		{
			get;
			private set;
		}

		public bool IsAny
		{
			get;
			private set;
		}

		public bool IsCount
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
			get { return (this.arguments.Count > 0) ? this.arguments.ToArray() : null; }
		}

		public string SortString
		{
			get { return (this.sortBuilder != null) ? this.sortBuilder.ToString() : null; }
		}

		public int Skip
		{
			get;
			private set;
		}

		public int Take
		{
			get;
			private set;
		}

		public Expression Translate (Expression expression)
		{
			Expression expr = Visit (expression);

			if (Table == null)
				Table = this.tableFinder.DefaultTable;

			return expr;
		}

		private readonly ITableFinder tableFinder;
		private bool fallback = false;
		private List<Tuple<string, Type>> projections;
		private List<string> columns;
		private StringBuilder sortBuilder;
		private readonly List<string> arguments = new List<string>();
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
				else if (methodCall.Method.Name == "Select")
					expression = VisitSelect (methodCall);
				else if (methodCall.Method.Name == "SelectMany")
					expression = VisitSelectMany (methodCall);
				else if (methodCall.Method.Name == "OrderBy" || methodCall.Method.Name == "OrderByDescending")
					expression = VisitOrder (methodCall);
				else if (methodCall.Method.Name == "Skip")
				    expression = VisitSkip (methodCall);
				else if (methodCall.Method.Name == "Take")
				    expression = VisitTake (methodCall);
				else if (methodCall.Method.Name == "Count")
					expression = VisitCount (methodCall);
			}

			return expression;
		}

		private Expression VisitCount (MethodCallExpression methodCall)
		{
			if (methodCall.Arguments.Count > 1)
			{
				VisitWhere (methodCall);
				if (this.fallback)
					return methodCall;
			}

			IsCount = true;
			return methodCall.Arguments[0];
		}

		private Expression VisitTake (MethodCallExpression methodCall)
		{
			ConstantExpression ce = (ConstantExpression) methodCall.Arguments[1];
			Take = (int) ce.Value;

			return methodCall.Arguments[0];
		}

		private Expression VisitSkip (MethodCallExpression methodCall)
		{
			ConstantExpression ce = (ConstantExpression) methodCall.Arguments[1];
			Skip = (int) ce.Value;

			return methodCall.Arguments[0];
		}

		private Expression VisitAny (MethodCallExpression methodCall)
		{
			if (methodCall.Arguments.Count > 1)
			{
				VisitWhere (methodCall);
				if (this.fallback)
					return methodCall;
			}

			IsAny = true;
			return methodCall.Arguments[0];
		}

		private class WhereEvaluator
			: ExpressionVisitor
		{
			public WhereEvaluator (ITableFinder tableFinder, Android.Net.Uri existingTable)
			{
				this.tableFinder = tableFinder;
				this.table = existingTable;
			}

			public TableFindResult Result
			{
				get;
				private set;
			}

			public bool Fallback
			{
				get;
				private set;
			}

			public Expression Evaluate (Expression expression)
			{
				expression = Visit (expression);

				Result = new TableFindResult (this.table, this.builder.ToString(), this.arguments);

				return expression;
			}

			private readonly ITableFinder tableFinder;
			private Android.Net.Uri table;
			private readonly StringBuilder builder = new StringBuilder();
			private readonly List<string> arguments = new List<string>();

			protected override Expression VisitMemberAccess (MemberExpression memberExpression)
			{
				TableFindResult result = this.tableFinder.Find (memberExpression);
				if (this.table == null)
				{
					this.table = result.Table;
					this.builder.Append (result.QueryString);
					this.arguments.AddRange (result.Arguments);
				}
				else if (this.table != result.Table)
				{
					Fallback = true;
					return memberExpression;
				}

				Tuple<string, Type> column = this.tableFinder.GetColumn (memberExpression.Member);
				if (column == null)
				{
					Fallback = true;
					return memberExpression;
				}

				if (this.builder.Length > 0)
					this.builder.Append (" AND ");

				this.builder.Append (column.Item1);

				return base.VisitMemberAccess (memberExpression);
			}

			protected override Expression VisitConstant (ConstantExpression constant)
			{
				if (constant.Value is IQueryable)
					return constant;

				if (constant.Value == null)
					this.builder.Append ("NULL");
				else
				{
					switch (Type.GetTypeCode (constant.Value.GetType()))
					{
						case TypeCode.Object:
							Fallback = true;
							return constant;

						case TypeCode.Boolean:
							this.arguments.Add ((bool)constant.Value ? "1" : "0");
							this.builder.Append ("?");
							break;

						default:
							this.arguments.Add (constant.Value.ToString());
							this.builder.Append ("?");
							break;
					}
				}

				return base.VisitConstant (constant);
			}

			protected override Expression VisitBinary (BinaryExpression binary)
			{
				this.builder.Append ("(");

				Visit (binary.Left);

				switch (binary.NodeType)
				{
					case ExpressionType.And:
						this.builder.Append (" AND ");
						break;

					case ExpressionType.Or:
						this.builder.Append (" OR ");
						break;

					case ExpressionType.Equal:
						this.builder.Append (" = ");
						break;

					default:
						Fallback = true;
						return binary;
				}

				Visit (binary.Right);

				this.builder.Append (")");

				return binary;
			}
		}

		private Expression VisitWhere (MethodCallExpression methodCall)
		{
			Expression expression = ExpressionEvaluator.Evaluate (methodCall);

			var eval = new WhereEvaluator (this.tableFinder, Table);
			expression = eval.Evaluate (expression);

			TableFindResult result = eval.Result;
			if (Table == null)
				Table = result.Table;

			if (eval.Fallback || result.Table == null || result.Table != Table)
			{
				this.fallback = true;
				return methodCall;
			}

			this.arguments.AddRange (result.Arguments);
			if (this.queryBuilder.Length > 0)
				this.queryBuilder.Append (" AND ");

			this.queryBuilder.Append (result.QueryString);

			return methodCall.Arguments[0];
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

//		private Expression VisitSelect (MethodCallExpression methodCall)
//		{
//			List<MemberExpression> mes = MemberExpressionFinder.Find (methodCall.Arguments[1], this.tableFinder);
//			if (!TryGetTable (mes))
//				return methodCall;
//
//			Type returnType = null;
//
//			List<Tuple<string, Type>> projs = new List<Tuple<string, Type>>();
//			foreach (MemberExpression me in mes)
//			{
//				Tuple<string, Type> column = this.tableFinder.GetColumn (me.Member);
//				if (column == null)
//					return methodCall;
//				
//				if (returnType == null)
//					returnType = column.Item2;
//				if (returnType != column.Item2)
//					return methodCall;
//
//				projs.Add (column);
//			}
//
//			ReturnType = returnType;
//			this.fallback = true;
//
//			(this.projections ?? (this.projections = new List<Tuple<string, Type>>()))
//				.AddRange (projs);
//
//			return methodCall.Arguments[0];
//		}

		private Expression VisitSelectMany (MethodCallExpression methodCall)
		{
//			List<MemberExpression> mes = MemberExpressionFinder.Find (methodCall, this.tableFinder);
//			if (!TryGetTable (mes))
//				return methodCall;

			return methodCall;
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

		private bool TryGetTable (List<MemberExpression> memberExpressions)
		{
			if (memberExpressions.Count == 0)
			{
				this.fallback = true;
				return false;
			}

			Android.Net.Uri existingTable = Table;

			TableFindResult presult = null;

			foreach (MemberExpression me in memberExpressions)
			{
				TableFindResult result = this.tableFinder.Find (me);
				if (result.Table == null)
				{
					this.fallback = true;
					return false;
				}

				if (existingTable == null)
				{
					existingTable = result.Table;
					presult = result;
				}
				else if (existingTable != result.Table)
				{
					this.fallback = true;
					return false;
				}
			}

			if (presult == null)
			{
				this.fallback = true;
				return false;
			}

			Table = presult.Table;

			if (presult.QueryString != null)
				this.queryBuilder.Append (presult.QueryString);

			this.arguments.AddRange (presult.Arguments);

			return true;
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