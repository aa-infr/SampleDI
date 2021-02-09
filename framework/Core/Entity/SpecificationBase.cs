using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Infrabel.ICT.Framework.Entity
{
    public abstract class SpecificationBase<T> : ISpecification<T>
   where T : class, IEntityBase, new()
    {
        public Expression<Func<T, bool>> Criteria => ToExpression();

        public Expression<Func<T, object>> GroupBy { get; private set; }

        public List<Expression<Func<T, object>>> Includes { get; private set; }

        public List<string> IncludeStrings { get; private set; }

        public bool IsPagingEnabled { get; private set; }

        public bool IsStateLessQuery { get; private set; }

        public Expression<Func<T, object>> OrderBy { get; private set; }

        public Expression<Func<T, object>> OrderByDescending { get; private set; }

        public Expression<Func<T, T>> Select { get; private set; }

        public int Skip { get; private set; }

        public int Take { get; private set; }

        public Expression<Func<T, object>>[] ThenOrderBy { get; private set; }

        public Expression<Func<T, object>>[] ThenOrderByDescending { get; private set; }

        public static SpecificationBase<T> operator !(SpecificationBase<T> specification)
        {
            if (specification == null)
                throw new ArgumentNullException(nameof(specification));

            return specification.Negate();
        }

        public static SpecificationBase<T> operator &(SpecificationBase<T> first, SpecificationBase<T> second)
        {
            if (first == null)
                throw new ArgumentNullException(nameof(first));

            if (second == null)
                throw new ArgumentNullException(nameof(second));

            return new AndSpecification<T>(first, second);
        }

        public virtual SpecificationBase<T> AddInclude(Expression<Func<T, object>> includeExpression)
        {
            if (Includes == null)
                Includes = new List<Expression<Func<T, object>>>();
            Includes.Add(includeExpression);
            return this;
        }

        public virtual SpecificationBase<T> AddInclude(string includeString)
        {
            if (IncludeStrings == null)
                IncludeStrings = new List<string>();
            IncludeStrings.Add(includeString);
            return this;
        }

        public Expression<Func<T, bool>> And(Expression<Func<T, bool>> leftExpression, Expression<Func<T, bool>> rightExpression)
        {
            var paramExpr = Expression.Parameter(typeof(T));
            var exprBody = Expression.AndAlso(leftExpression.Body, rightExpression.Body);
            exprBody = (BinaryExpression)new ParameterReplacer(paramExpr).Visit(exprBody);
            var finalExpr = Expression.Lambda<Func<T, bool>>(exprBody ?? throw new InvalidOperationException(), paramExpr);
            return finalExpr;
        }

        public SpecificationBase<T> And(SpecificationBase<T> specification)
        {
            return new AndSpecification<T>(this, specification);
        }

        public virtual SpecificationBase<T> ApplyGroupBy(Expression<Func<T, object>> groupByExpression)
        {
            GroupBy = groupByExpression;
            return this;
        }

        public virtual SpecificationBase<T> ApplyOrderBy(Expression<Func<T, object>> orderByExpression)
        {
            OrderBy = orderByExpression;
            return this;
        }

        public virtual SpecificationBase<T> ApplyOrderByDescending(Expression<Func<T, object>> orderByDescendingExpression)
        {
            OrderByDescending = orderByDescendingExpression;
            return this;
        }

        public virtual SpecificationBase<T> ApplyPaging(int pageNumber, int pageSize)
        {
            Skip = (pageNumber - 1) * pageSize;
            Take = pageSize;
            IsPagingEnabled = true;
            return this;
        }

        public virtual SpecificationBase<T> ApplyPaging(PageRequest pageRequest)
        {
            if (pageRequest?.PageSize == null)
                return this;
            ApplyPaging(pageRequest.PageNumber, (int)pageRequest.PageSize);
            IsPagingEnabled = true;
            return this;
        }

        public virtual SpecificationBase<T> ApplySelect(Expression<Func<T, T>> selectExpression)
        {
            Select = selectExpression;
            return this;
        }

        public virtual SpecificationBase<T> ApplyThenOrderBy(
            params Expression<Func<T, object>>[] thenOrderByExpressions)
        {
            ThenOrderBy = thenOrderByExpressions;
            return this;
        }

        public virtual SpecificationBase<T> ApplyThenOrderByDescending(
            params Expression<Func<T, object>>[] thenOrderByDescendingExpressions)
        {
            ThenOrderByDescending = thenOrderByDescendingExpressions;
            return this;
        }

        public bool IsSatisfiedBy(T entity)
        {
            return ToExpression()
                   .Compile()(entity);
        }

        public SpecificationBase<T> Negate()
        {
            return new NotSpecification<T>(this);
        }

        public Expression<Func<T, bool>> Or(Expression<Func<T, bool>> leftExpression, Expression<Func<T, bool>> rightExpression)
        {
            var paramExpr = Expression.Parameter(typeof(T));
            var exprBody = Expression.OrElse(leftExpression.Body, rightExpression.Body);
            exprBody = (BinaryExpression)new ParameterReplacer(paramExpr).Visit(exprBody);
            var finalExpr = Expression.Lambda<Func<T, bool>>(exprBody ?? throw new InvalidOperationException(), paramExpr);
            return finalExpr;
        }

        public SpecificationBase<T> Or(SpecificationBase<T> specification)
        {
            return new OrSpecification<T>(this, specification);
        }

        public abstract Expression<Func<T, bool>> ToExpression();

        public virtual SpecificationBase<T> UseStatelessQuery()
        {
            IsStateLessQuery = true;
            return this;
        }
    }
}