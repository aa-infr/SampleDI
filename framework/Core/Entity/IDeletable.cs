using System;

namespace Infrabel.ICT.Framework.Entity
{
    public interface IDeletable
    {
        DateTime? Deletion { get; set; }
    }
}