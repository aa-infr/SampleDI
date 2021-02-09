using System;

namespace Infrabel.ICT.Framework.Entity
{
    public interface IEntityBase : ICloneable
    {
        long Id { get; }
    }
}