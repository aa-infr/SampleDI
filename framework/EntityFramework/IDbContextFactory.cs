using Infrabel.ICT.Framework.Entity;

namespace Infrabel.ICT.Framework.Extended.EntityFramework
{
    public interface IDbContextFactory
    {
        DbContextBase Create<T>() where T : class, IEntityBase, new();
    }
}