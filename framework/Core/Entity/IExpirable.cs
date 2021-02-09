using System;

namespace Infrabel.ICT.Framework.Entity
{
    public interface IExpirable
    {
        DateTime ValidFrom { get; set; }

        DateTime? ValidTo { get; set; }
    }
}