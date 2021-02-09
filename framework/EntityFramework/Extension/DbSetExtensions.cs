using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Infrabel.ICT.Framework.Extended.EntityFramework.Extension
{
    public static class DbSetExtensions
    {
        private static IQueryable<T> Query<T>(this DbSet<T> dbSet) where T : class
        {
            return dbSet;
        }

        public static IQueryable<T> ReadonlyQuery<T>(this DbSet<T> dbSet) where T : class
        {
            return dbSet.Query()
                .AsNoTracking();
        }
    }
}