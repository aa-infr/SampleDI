using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Infrabel.ICT.Framework.Entity
{
    public interface ISpecification<T> where T : class, IEntityBase, new()
    {
        Expression<Func<T, bool>> Criteria { get; }

        List<string> IncludeStrings { get; }

        List<Expression<Func<T, object>>> Includes { get; }

        Expression<Func<T, object>> OrderBy { get; }

        Expression<Func<T, object>>[] ThenOrderBy { get; }

        Expression<Func<T, object>> OrderByDescending { get; }

        Expression<Func<T, object>>[] ThenOrderByDescending { get; }

        Expression<Func<T, object>> GroupBy { get; }

        Expression<Func<T, T>> Select { get; }

        bool IsStateLessQuery { get; }

        bool IsPagingEnabled { get; }

        int Take { get; }

        int Skip { get; }

        SpecificationBase<T> AddInclude(Expression<Func<T, object>> includeExpression);

        SpecificationBase<T> AddInclude(string includeString);

        SpecificationBase<T> And(SpecificationBase<T> specification);

        Expression<Func<T, bool>> And(Expression<Func<T, bool>> leftExpression, Expression<Func<T, bool>> rightExpression);

        SpecificationBase<T> ApplyGroupBy(Expression<Func<T, object>> groupByExpression);

        SpecificationBase<T> ApplyOrderBy(Expression<Func<T, object>> orderByExpression);

        SpecificationBase<T> ApplyOrderByDescending(Expression<Func<T, object>> orderByDescendingExpression);

        SpecificationBase<T> ApplyPaging(PageRequest pageRequest);

        SpecificationBase<T> ApplyPaging(int pageNumber, int pageSize);

        SpecificationBase<T> ApplySelect(Expression<Func<T, T>> selectExpression);

        SpecificationBase<T> ApplyThenOrderBy(params Expression<Func<T, object>>[] thenOrderByExpressions);

        SpecificationBase<T> ApplyThenOrderByDescending(params Expression<Func<T, object>>[] thenOrderByDescendingExpressions);

        bool IsSatisfiedBy(T entity);

        SpecificationBase<T> Negate();

        SpecificationBase<T> Or(SpecificationBase<T> specification);

        Expression<Func<T, bool>> Or(Expression<Func<T, bool>> leftExpression, Expression<Func<T, bool>> rightExpression);

        Expression<Func<T, bool>> ToExpression();

        SpecificationBase<T> UseStatelessQuery();
    }
}