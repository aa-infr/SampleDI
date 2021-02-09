using Infrabel.ICT.Framework.Extension;
using System;

namespace Infrabel.ICT.Framework.Ioc
{
    public readonly struct RegistrationInfo : IEquatable<RegistrationInfo>
    {
        public RegistrationInfo(RegistrationLifeTime lifeTime, string key = "")
        {
            if (!lifeTime.IsMemberOf<RegistrationLifeTime>())
                throw new ArgumentOutOfRangeException(nameof(lifeTime));
            LifeTime = lifeTime;

            HasKey = !string.IsNullOrWhiteSpace(key);

            Key = HasKey ? key : string.Empty;
        }

        public bool HasKey { get; }
        public string Key { get; }
        public RegistrationLifeTime LifeTime { get; }

        public bool Matches(RegistrationLifeTime lifeTime)
        {
            return lifeTime == LifeTime;
        }

        public static readonly RegistrationInfo Default = new RegistrationInfo(RegistrationLifeTime.Unknown, string.Empty);

        public bool Equals(RegistrationInfo other)
        {
            return Key == other.Key && LifeTime == other.LifeTime;
        }

        public override bool Equals(object obj)
        {
            return obj is RegistrationInfo other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Key != null ? Key.GetHashCode() : 0) * 397) ^ (int)LifeTime;
            }
        }
    }
}