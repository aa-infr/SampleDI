using Infrabel.ICT.Framework.Ioc;
using System;

namespace Infrabel.ICT.Framework.Service
{
    [IoCRegistration(RegistrationLifeTime.Singleton)]
    internal class DateService : IDateService
    {
        public DateTime Now => DateTime.Now;
        public DateTime Today => Now.Date;
        public int CurrentYear => Now.Year;

        public DateTime UtcNow => DateTime.UtcNow;
        public DateTime UtcToday => UtcNow.Date;
        public int UtcCurrentYear => UtcNow.Year;
    }
}