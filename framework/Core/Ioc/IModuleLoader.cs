using System.Reflection;

namespace Infrabel.ICT.Framework.Ioc
{
    public interface IModuleLoader
    {
        IInitializedModuleLoader LoadModules(Assembly targetAssembly);

        IInitializedModuleLoader LoadModules<TAnchor>() where TAnchor : class;
    }
}