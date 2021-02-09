using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System;

namespace Infrabel.ICT.Framework.Entity
{
    [ExcludeFromCodeCoverage]
    public class PaginatedListResult<T> where T : class, IEntityBase, new()
    {
        public PaginatedListResult()
        {
            Entities = new List<T>();
        }

        private PaginatedListResult(IList<T> source, bool hasNext, bool hasPrevious, int count, int pageCount, int currentPage, int pageSize) : this()
        {
            HasNext = hasNext;
            HasPrevious = hasPrevious;
            Count = count;
            PageCount = pageCount;
            CurrentPage = currentPage;
            PageSize = pageSize;
            Entities.AddRange(source);
        }

        /// <summary>
        ///     Does the returned result contains more rows to be retrieved?
        /// </summary>
        public bool HasNext { get; }
        public int PageCount { get; }
        public int PageSize { get; }
        public int CurrentPage { get; }
        /// <summary>
        ///     Does the returned result contains previous items ?
        /// </summary>
        public bool HasPrevious { get; }

        /// <summary>
        ///     Total number of rows that could be possibly be retrieved.
        /// </summary>
        public int Count { get; }

        public List<T> Entities { get; set; }

        public static PaginatedListResult<T> Create(IList<T> source, int pageSize, int skip, int count)
        {
            var hasNext = skip + pageSize <count;
            var hasPrevious = skip >0 ;
            var pageCount = (int)Math.Ceiling((double)count / pageSize);
            var current = (int)Math.Ceiling((double)skip /pageSize)+1;
            return new PaginatedListResult<T>(source, hasNext, hasPrevious, count, pageCount, current, pageSize);
        }
    }
}
