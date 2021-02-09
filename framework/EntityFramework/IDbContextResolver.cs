using Infrabel.ICT.Framework.Entity;

namespace Infrabel.ICT.Framework.Extended.EntityFramework
{
    public interface IDbContextResolver
    {
        string Resolve<T>() where T : class, IEntityBase, new();
    }
}