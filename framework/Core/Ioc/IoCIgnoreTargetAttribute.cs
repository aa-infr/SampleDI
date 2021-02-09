using System;

namespace Infrabel.ICT.Framework.Ioc
{
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class, Inherited = false)]
    public class IoCIgnoreTargetAttribute : Attribute
    {
    }
}