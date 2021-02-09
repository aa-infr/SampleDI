using System;

namespace Infrabel.ICT.Framework.Service
{
    public interface IDateOffsetService
    {
        DateTimeOffset Today { get; }
        DateTimeOffset Now { get; }
        int CurrentYear { get; }
        DateTimeOffset UtcNow { get; }
        DateTimeOffset UtcToday { get; }
        int UtcCurrentYear { get; }
    }
}