using System;

namespace Infrabel.ICT.Framework.Service
{
    public interface IDateService
    {
        DateTime Today { get; }

        DateTime Now { get; }
        int CurrentYear { get; }

        DateTime UtcNow { get; }
        DateTime UtcToday { get; }
        int UtcCurrentYear { get; }
    }
}