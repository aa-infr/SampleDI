namespace Infrabel.ICT.Framework.Service
{
    public interface IConnectionStringResolver
    {
        string Resolve<TDbContext>() where TDbContext : class;

        string Resolve(string name);
    }
}