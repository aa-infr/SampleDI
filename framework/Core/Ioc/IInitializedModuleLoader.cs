namespace Infrabel.ICT.Framework.Ioc
{
    public interface IInitializedModuleLoader : IModuleLoader
    {
        IRegistrationContainer BuildContainer();
    }
}