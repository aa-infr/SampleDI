using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

namespace Infrabel.ICT.Framework.Entity
{
    [ExcludeFromCodeCoverage]
    internal class ParameterReplacer : ExpressionVisitor
    {
        private readonly ParameterExpression _parameter;

        internal ParameterReplacer(ParameterExpression parameter) => _parameter = parameter;

        protected override Expression VisitParameter(ParameterExpression node) => base.VisitParameter(_parameter);
    }
}