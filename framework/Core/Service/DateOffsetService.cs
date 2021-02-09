using Infrabel.ICT.Framework.Ioc;
using System;

namespace Infrabel.ICT.Framework.Service
{
    [IoCRegistration(RegistrationLifeTime.Singleton)]
    internal class DateOffsetService : IDateOffsetService
    {
        public DateTimeOffset Today
        {
            get
            {
                var now = Now;
                return new DateTimeOffset(now.Year, now.Month, now.Day, 0, 0, 0, now.Offset);
            }
        }

        public DateTimeOffset Now => DateTimeOffset.Now;

        public int CurrentYear => Now.Year;

        public DateTimeOffset UtcToday
        {
            get
            {
                var nowUtc = UtcNow;
                return new DateTimeOffset(nowUtc.Year, nowUtc.Month, nowUtc.Day, 0, 0, 0, nowUtc.Offset);
            }
        }

        public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;

        public int UtcCurrentYear => UtcNow.Year;
    }
}