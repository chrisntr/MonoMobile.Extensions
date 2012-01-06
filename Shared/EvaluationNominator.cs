using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Xamarin
{
	internal class EvaluationNominator
		: ExpressionVisitor
	{
		internal EvaluationNominator (Func<Expression, bool> predicate)
		{
			if (predicate == null)
				throw new ArgumentNullException ("predicate");

			this.predicate = predicate;
		}

		public HashSet<Expression> Nominate (Expression expression)
		{
			this.candidates = new HashSet<Expression>();
			Visit (expression);
			return this.candidates;
		}

		public override Expression Visit (Expression expression)
		{
			if (expression == null)
				return null;

			bool currentState = this.cannotBeEvaluated;
			this.cannotBeEvaluated = false;

			base.Visit (expression);

			if (!this.cannotBeEvaluated)
			{
				if (predicate (expression))
					this.candidates.Add (expression);
				else
					this.cannotBeEvaluated = true;
			}

			this.cannotBeEvaluated |= currentState;

			return expression;
		}

		private readonly Func<Expression, bool> predicate;
		private bool cannotBeEvaluated;
		private HashSet<Expression> candidates;
	}
}