using Infrabel.ICT.Framework.Entity;
using System.Collections.Generic;
using System.Linq;

namespace Infrabel.ICT.Framework.Extension
{
    public static class EntityBaseExtensions
    {
        public static IEnumerable<T> Clone<T>(this IEnumerable<T> entities) where T : class, IEntityBase
        {
            return entities?.Select(e => e.Clone() as T);
        }
    }
}