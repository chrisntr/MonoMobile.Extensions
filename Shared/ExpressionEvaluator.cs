using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Xamarin
{
	internal static class ExpressionEvaluator
	{
		public static Expression Evaluate (Expression expression, Func<Expression, bool> predicate)
		{
			HashSet<Expression> canidates = new EvaluationNominator (predicate).Nominate (expression);
			return new SubtreeEvaluator (canidates).Visit (expression);
		}

		public static Expression Evaluate (Expression expression)
		{
			return Evaluate (expression, e => e.NodeType != ExpressionType.Parameter);
		}

		private class SubtreeEvaluator
			: ExpressionVisitor
		{
			public SubtreeEvaluator (HashSet<Expression> candidate)
			{
				this.candidate = candidate;
			}

			public override Expression Visit (Expression expression)
			{
				if (expression == null)
					return null;

				if (this.candidate.Contains (expression))
					return EvaluateCandidate (expression);

				return base.Visit (expression);
			}

			private readonly HashSet<Expression> candidate;

			private Expression EvaluateCandidate (Expression expression)
			{
				if (expression.NodeType == ExpressionType.Constant)
					return expression;

				LambdaExpression lambda = Expression.Lambda (expression);
				Delegate fn = lambda.Compile();

				return Expression.Constant (fn.DynamicInvoke (null), expression.Type);
			}
		}
	}
}