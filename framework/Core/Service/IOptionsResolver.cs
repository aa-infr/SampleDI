namespace Infrabel.ICT.Framework.Service
{
    public interface IOptionsResolver
    {
        TOptions Resolve<TOptions>() where TOptions : class, new();
    }

    public interface IOptionsResolver<TOptions> where TOptions : class, new()
    {
        TOptions Resolve();
    }
}