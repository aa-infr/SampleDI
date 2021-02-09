using System;

namespace Infrabel.ICT.Framework.Ioc
{
    [Flags]
    public enum RegistrationTarget
    {
        None = 0,
        Interfaces = 1,
        AbstractBases = 2,
        Self = 4
    }
}