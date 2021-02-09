using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Infrabel.ICT.Framework.Entity
{
    public interface IRepository<T> where T : class, IEntityBase, new()
    {
        bool EnableTranslation { get; set; }

        bool Exists(long id);

        Task<bool> ExistsAsync(long id, CancellationToken cancellationToken = default);

        bool Exists(SpecificationBase<T> spec);

        Task<bool> ExistsAsync(SpecificationBase<T> spec, CancellationToken cancellationToken = default);

        void Add(T entity);

        void Add(IEnumerable<T> entities);

        void Remove(T entity);

        void Remove(IEnumerable<T> entities);

        void Remove(SpecificationBase<T> spec);

        void RemoveById(long id);

        void Update(T entity);

        void Commit();

        Task CommitAsync(CancellationToken cancellationToken = default);

        void Translate(T entity);

        void Translate(IEnumerable<T> entities);

        Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default);

        IQueryable<T> Query();

        IQueryable<T> QueryNoTrack();

        Task<T> FindByIdAsync(long id, CancellationToken cancellationToken = default);

        T FindById(long id);

        IEnumerable<T> Find(SpecificationBase<T> spec);

        Task<T> FindOneAsync(SpecificationBase<T> spec, CancellationToken cancellationToken = default);

        Task<IEnumerable<T>> FindAsync(SpecificationBase<T> spec, CancellationToken cancellationToken = default);

        Task<PaginatedListResult<T>> FindPageAsync(SpecificationBase<T> specification, CancellationToken cancellationToken = default);

    }
}