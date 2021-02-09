using Infrabel.ICT.Framework.Entity;
using System.Linq;

namespace Infrabel.ICT.Framework.Extension
{
    public static class QueryableExtensions
    {
        public static IQueryable<T> FilterExpired<T>(this IQueryable<T> query) where T : class, IExpirable =>
            query.Where(r => r.ValidTo == null);

        public static IQueryable<T> FilterInactive<T>(this IQueryable<T> query) where T : class, IEnablable =>
                    query.Where(r => r.IsEnabled);

        public static IQueryable<T> FilterDeleted<T>(this IQueryable<T> query) where T : class, IDeletable =>
            query.Where(r => r.Deletion == null);
    }
}