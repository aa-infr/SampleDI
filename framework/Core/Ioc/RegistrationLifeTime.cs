namespace Infrabel.ICT.Framework.Ioc
{
    public enum RegistrationLifeTime
    {
        Unknown = 0,
        Scoped = 1,
        Singleton = 2,
        Transient = 3,
        ScopedOrSingleton = 4,
        None = 5
    }
}