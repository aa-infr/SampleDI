using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

namespace Infrabel.ICT.Framework.Entity
{
	[ExcludeFromCodeCoverage]
	public class OrSpecification<T> : SpecificationBase<T> where T : class, IEntityBase, new()
    {
		private readonly SpecificationBase<T> _left;
		private readonly SpecificationBase<T> _right;

		public OrSpecification(SpecificationBase<T> left, SpecificationBase<T> right)
		{
			_right = right;
			_left = left;
		}

		public override Expression<Func<T, bool>> ToExpression()
		{
			var leftExpression = _left.ToExpression();
			var rightExpression = _right.ToExpression();
			var paramExpr = Expression.Parameter(typeof(T));
			var exprBody = Expression.OrElse(leftExpression.Body, rightExpression.Body);
			exprBody = (BinaryExpression) new ParameterReplacer(paramExpr).Visit(exprBody);
			var finalExpr = Expression.Lambda<Func<T, bool>>(exprBody ?? throw new InvalidOperationException(), paramExpr);
			return finalExpr;
		}
	}
}