using System;

namespace Infrabel.ICT.Framework.Ioc
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true)]
    public class IoCRegistrationAttribute : Attribute
    {
        public IoCRegistrationAttribute(RegistrationLifeTime lifeTime, string key = "")
        {
            Information = new RegistrationInfo(lifeTime, key);
        }

        public RegistrationInfo Information { get; }
    }
}