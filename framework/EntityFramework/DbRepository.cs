using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Infrabel.ICT.Framework.Entity;
using Infrabel.ICT.Framework.Extended.EntityFramework.Extension;
using Infrabel.ICT.Framework.Extended.EntityFramework.Models;
using Infrabel.ICT.Framework.Extension;
using Infrabel.ICT.Framework.Service;
using Microsoft.EntityFrameworkCore;

namespace Infrabel.ICT.Framework.Extended.EntityFramework
{
    [ExcludeFromCodeCoverage]
    [Guid("98E71DC0-B8EC-4ECC-AAFA-8D088BF9B3D1")]
    public class DbRepository<T> : IRepository<T> where T : class, IEntityBase, new()
    {
        private readonly DbSet<T> _dbSet;
        private readonly LexiconDictionary _lexiconDictionary;
        private readonly IUserContext _userContext;
        protected readonly IDateService DateService;
        protected readonly DbContextBase DbContext;

        public DbRepository(IDbContextFactory factory, IUserContext userContext,
            ILexiconFactoryService lexiconFactoryService, IDateService dateService) : this(factory.Create<T>(), userContext,
            lexiconFactoryService, dateService)
        {
        }

        protected DbRepository(DbContextBase context)
        {
            DbContext = context;
            _dbSet = context.Set<T>();
            EnableTranslation = false;
        }

        protected DbRepository(DbContextBase context, IUserContext userContext,
            ILexiconFactoryService lexiconFactoryService, IDateService dateService) : this(context)
        {
            _userContext = userContext;
            DateService = dateService;
            _lexiconDictionary = lexiconFactoryService.Create();
            EnableTranslation = _lexiconDictionary != null;
            if (EnableTranslation)
                DbContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }

        public bool EnableTranslation { get; set; }

        public void Add(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            // fill shadow property
            if (EntityTypeBuilderExtensions.HasShadowField(typeof(T)))
                InsertShadowProperties(entity);

            _dbSet.Add(entity);
        }

        public void Add(IEnumerable<T> entities)
        {
            var enumerable = entities.ToList();
            if (entities == null || !enumerable.Any())
                throw new ArgumentNullException(nameof(entities));

            // fill shadow property
            if (EntityTypeBuilderExtensions.HasShadowField(typeof(T)))
                foreach (var entity in enumerable)
                    InsertShadowProperties(entity);

            _dbSet.AddRange(enumerable);
        }

        public void Commit()
        {
            DbContext.SaveChanges();
        }

        public async Task CommitAsync(CancellationToken cancellationToken = default)
        {
            await DbContext.SaveChangesAsync(cancellationToken);
        }

        public bool Exists(long id)
        {
            return _dbSet.AsNoTracking()
                         .Any(t => t.Id == id);
        }

        public Task<bool> ExistsAsync(long id, CancellationToken cancellationToken = default)
        {
            return _dbSet.AsNoTracking()
                         .AnyAsync(t => t.Id == id, cancellationToken);
        }

        public bool Exists(SpecificationBase<T> spec)
        {
            return ApplySpecification(spec.UseStatelessQuery())
                   .Any();
        }

        public Task<bool> ExistsAsync(SpecificationBase<T> spec, CancellationToken cancellationToken = default)
        {
            return ApplySpecification(spec.UseStatelessQuery())
                   .AnyAsync(cancellationToken);
        }

        // handle translation
        public IEnumerable<T> Find(SpecificationBase<T> spec)
        {
            return EnableTranslation && _lexiconDictionary.IsMultiLingualEntity(typeof(T))
                ? _lexiconDictionary.Translate(
                    ApplySpecification(spec)
                    .ToList(), _userContext?.Identity.LanguageIsoCode)
                : ApplySpecification(spec)
                  .ToList();
        }

        // handle translation : TODO manage paging, orderby & translation
        public async Task<IEnumerable<T>> FindAsync(SpecificationBase<T> specification,
            CancellationToken cancellationToken = default)
        {
            return EnableTranslation && _lexiconDictionary.IsMultiLingualEntity(typeof(T))
                ? _lexiconDictionary.Translate(
                    await ApplySpecification(specification)
                          .ToListAsync(cancellationToken), _userContext?.Identity.LanguageIsoCode)
                : await ApplySpecification(specification)
                        .ToListAsync(cancellationToken);
        }

        // handle translation
        public T FindById(long id)
        {
            return EnableTranslation && _lexiconDictionary.IsMultiLingualEntity(typeof(T))
                ? _lexiconDictionary.Translate(
                    _dbSet.Find(id), _userContext?.Identity.LanguageIsoCode)
                : _dbSet.Find(id);
        }

        // handle translation
        public async Task<T> FindByIdAsync(long id, CancellationToken cancellationToken = default)
        {
            return EnableTranslation && _lexiconDictionary.IsMultiLingualEntity(typeof(T))
                ? _lexiconDictionary.Translate(
                    await _dbSet.FindAsync(new object[] { id }, cancellationToken), _userContext?.Identity.LanguageIsoCode)
                : await _dbSet.FindAsync(new object[] { id }, cancellationToken);
        }

        // handle translation : TODO manage orderby & translation
        public async Task<T> FindOneAsync(SpecificationBase<T> specification,
            CancellationToken cancellationToken = default)
        {
            var result = await ApplySpecification(specification)
                               .Take(1)
                               .ToListAsync(cancellationToken);
            if (!result.Any())
                return null;
            return _lexiconDictionary?.IsMultiLingualEntity(typeof(T)) == true
                ? _lexiconDictionary.Translate(
                    result.First(), _userContext?.Identity.LanguageIsoCode)
                : result.First();
        }

        // handle translation : TODO manage paging, orderby & translation
        public async Task<PaginatedListResult<T>> FindPageAsync(SpecificationBase<T> specification,
            CancellationToken cancellationToken = default)
        {
            if (!specification.IsPagingEnabled)
                throw new ArgumentException("Invalid specification (IsPagingEnabled==FALSE)");

            var result = await ApplySpecification(specification)
                               .ToListAsync(cancellationToken);
            var resultCount = await SpecificationCountAsync(specification, cancellationToken);
            return PaginatedListResult<T>.Create(result, specification.Take, specification.Skip, resultCount);
        }
        public IQueryable<T> Query()
        {
            return EnableTranslation && _lexiconDictionary.IsMultiLingualEntity(typeof(T))
                ? _lexiconDictionary.Translate(
                    _dbSet.ToList(), _userContext?.Identity.LanguageIsoCode)
                                    .AsQueryable()
                : _dbSet.AsQueryable();
        }

        public void Remove(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));
            if (entity.Id <= 0)
                throw new ArgumentOutOfRangeException(nameof(entity));
            if (entity is IDeletable deletable)
                deletable.Deletion = DateService.UtcNow;
            else if (entity is IExpirable expirable)
                expirable.ValidTo = DateService.UtcNow;
            else
                _dbSet.Remove(entity);
        }

        public void Remove(IEnumerable<T> entities)
        {
            entities.Execute(Remove);
        }

        //todo properly handle deletable and expirable
        public void Remove(SpecificationBase<T> specification)
        {
            if (specification == null)
                throw new ArgumentNullException(nameof(specification));
            _dbSet.RemoveRange(_dbSet.Where(specification.ToExpression())
                                     .Select(x => x));
        }

        public void RemoveById(long id)
        {
            if (id <= 0)
                throw new ArgumentOutOfRangeException(nameof(id));
            Remove(_dbSet.Find(id));
        }

        public void Update(T entity)
        {
            DbContext.Entry(entity)
                     .State = EntityState.Modified;
            if (EntityTypeBuilderExtensions.HasShadowField(typeof(T)))
                UpdateShadowProperties(entity);
        }

        public async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return EnableTranslation && _lexiconDictionary.IsMultiLingualEntity(typeof(T))
                ? _lexiconDictionary.Translate(
                    await _dbSet.ToListAsync(cancellationToken), _userContext?.Identity.LanguageIsoCode)
                : await _dbSet.ToListAsync(cancellationToken);
        }

        public IQueryable<T> QueryNoTrack()
        {
            return EnableTranslation && _lexiconDictionary.IsMultiLingualEntity(typeof(T))
                ? _lexiconDictionary.Translate(_dbSet.AsNoTracking()
                                                     .ToList(), _userContext?.Identity.LanguageIsoCode)
                                    .AsQueryable()
                : _dbSet.AsQueryable()
                        .AsNoTracking();
        }

        // handle translation

        #region private methods

        public void Translate(T entity)
        {
            _ = _lexiconDictionary?.Translate(entity, _userContext?.Identity.LanguageIsoCode);
        }

        public void Translate(IEnumerable<T> entities)
        {
            _ = _lexiconDictionary?.Translate(entities, _userContext?.Identity.LanguageIsoCode);
        }

        private IQueryable<T> ApplyCriteria(IQueryable<T> query, ISpecification<T> specification)
        {
            return query.Where(specification.Criteria);
        }

        // TODO: apply orderby on Searchable fields
        private IQueryable<T> ApplySpecification(ISpecification<T> specification)
        {
            var query = DbContext.Set<T>()
                                 .AsQueryable();

            if (specification == null)
                return query;

            // modify the IQueryable using the specification's criteria expression
            if (specification.Criteria != null)
                query = ApplyCriteria(query, specification);

            // includes all expression-based includes
            if (specification.Includes != null)
                query = specification.Includes
                                     .Aggregate(query, (current, include) => current.Include(include));

            if (specification.IncludeStrings != null)
                query = specification.IncludeStrings
                                     .Aggregate(query, (current, include) => current.Include(include));

            // Apply ordering if expressions are set
            if (specification.OrderBy != null)
                query = query.OrderBy(specification.OrderBy);
            else if (specification.OrderByDescending != null)
                query = query.OrderByDescending(specification.OrderByDescending);

            if (specification.ThenOrderBy != null && specification.ThenOrderBy
                                                                  .Any())
                query = specification.ThenOrderBy
                                     .Aggregate(query,
                    (current, thenOrderBy) => ((IOrderedQueryable<T>)current).ThenBy(thenOrderBy));
            else if (specification.ThenOrderByDescending != null && specification.ThenOrderByDescending
                                                                                 .Any())
                query = specification.ThenOrderByDescending
                                     .Aggregate(query,
                    (current, thenOrderByDescending) => ((IOrderedQueryable<T>)current).ThenByDescending(thenOrderByDescending));

            if (specification.GroupBy != null)
                query = query.GroupBy(specification.GroupBy)
                             .SelectMany(x => x);

            if (specification.Select != null)
                query = query.Select(specification.Select);

            // Apply paging if enabled
            if (specification.IsPagingEnabled)
                query = query.Skip(specification.Skip)
                             .Take(specification.Take);

            if (specification.IsStateLessQuery)
                query = query.AsNoTracking();

            return query;
        }

        /// <summary>
        /// Fill the shadow properties during insert
        /// </summary>
        private void InsertShadowProperties(T entity)
        {
            DbContext.WriteDebug("Insert Shadow Properties");
            var props = entity.GetType()
                              .GetProperties();
            var entityType = typeof(T);

            foreach (var prop in props)
            {
                var searchInfo = EntityTypeBuilderExtensions.GetShadowField(entityType, prop);
                var value = prop.GetValue(entity);

                if (searchInfo != null && value != null)
                {
                    DbContext.WriteDebug($"Shadow Properties [{searchInfo.ColumnName}] = {value}");
                    DbContext.Entry(entity)
                             .Property(searchInfo.ColumnName)
                             .CurrentValue =
                        value.ToString()
                             .ToSearchable(searchInfo.SearchableType);
                }
            }
        }

        private Task<int> SpecificationCountAsync(SpecificationBase<T> specification, CancellationToken cancellationToken = default)
        {
            var query = DbContext.Set<T>()
                                 .AsQueryable();
            // modify the IQueryable using the specification's criteria expression
            if (specification?.Criteria != null)
                query = ApplyCriteria(query, specification);
            if (specification?.GroupBy != null)
                query = query.GroupBy(specification.GroupBy)
                             .SelectMany(x => x);
            return query.CountAsync(cancellationToken);
        }

        /// <summary>
        /// Fill the shadow properties during update considering the tracker
        /// </summary>
        private void UpdateShadowProperties(T entity)
        {
            DbContext.WriteDebug("Update Shadow Properties");
            var entry = DbContext.Entry(entity);
            var propertyNames = entry.Metadata
                                     .GetProperties()
                                     .Select(p => p.Name);
            var props = propertyNames.Select(p => entry.Property(p))
                                     .Where(e => e.IsModified);
            var entityType = typeof(T);

            foreach (var prop in props)
            {
                if (prop.Metadata
                        .IsShadowProperty())
                    continue;
                var searchInfo = EntityTypeBuilderExtensions.GetShadowField(entityType, prop);
                var value = prop.CurrentValue;

                if (searchInfo != null && value != null)
                    DbContext.Entry(entity)
                             .Property(searchInfo.ColumnName)
                             .CurrentValue =
                        value.ToString()
                             .ToSearchable(searchInfo.SearchableType);
            }
        }

        #endregion private methods

    }
}