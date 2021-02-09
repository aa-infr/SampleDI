using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

namespace Infrabel.ICT.Framework.Entity
{
    [ExcludeFromCodeCoverage]
    public class NotSpecification<T> : SpecificationBase<T> where T : class, IEntityBase, new()
    {
        private readonly SpecificationBase<T> _specification;

        public NotSpecification(SpecificationBase<T> specification)
        {
            _specification = specification ?? throw new ArgumentNullException(nameof(specification));
        }

        public override Expression<Func<T, bool>> ToExpression()
        {
            var baseExpression = _specification.ToExpression();
            var param = baseExpression.Parameters;
            var body = Expression.Not(baseExpression.Body);
            return Expression.Lambda<Func<T, bool>>(body, param);
        }
    }
}