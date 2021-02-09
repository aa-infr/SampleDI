using System;
using System.Collections.Generic;

namespace Infrabel.ICT.Framework.Ioc
{
    public interface IResolutionContainer
    {
        object Resolve(Type typeToResolve, string key = null);

        IEnumerable<object> ResolveAll(Type typeToResolve, string key = null);

        TType Resolve<TType>(string key = null) where TType : class;

        IEnumerable<TType> ResolveAll<TType>(string key = null) where TType : class;
    }
}