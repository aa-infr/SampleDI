using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Infrabel.ICT.Framework.Extended.AspNetCore.Pagination
{
    public class PaginatedRequest 
    {
        [Required(AllowEmptyStrings = false)]
        [Range(1, int.MaxValue)]
        public int PageNumber { get; set; }

        [Required(AllowEmptyStrings = false)]
        [Range(1, 500)]
        public int PageSize { get; set; }

        public string OrderBy { get; set; }

        public bool Reverse { get; set; }

        public IDictionary<string, IEnumerable<string>> Filters { get; set; } = new Dictionary<string, IEnumerable<string>>();
    }
}