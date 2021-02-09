using System;

namespace Infrabel.ICT.Framework.Entity
{
    public class ExpirableEntity : AuditableEntity, IExpirable
    {
        public DateTime ValidFrom { get; set; }

        public DateTime? ValidTo { get; set; }
    }
}
