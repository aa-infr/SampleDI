using System.Collections.Generic;

namespace Infrabel.ICT.Framework.Entity
{
    public class PaginatedListResultDto<T> where T : class, new()
    {
        public int Count { get; set; }
        public int TotalPage { get; set; }
        public int PageSize { get; set; }
        public int PageNumber { get; set; }
        public IEnumerable<T> Items { get; set; }
    }
}