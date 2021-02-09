using System;

namespace Infrabel.ICT.Framework.Entity
{
    public abstract class AuditableEntity : EntityBase, IAuditableEntity
    {
        public string CreatedBy { get; set; }

        public DateTime CreationDate { get; set; }

        public string UpdatedBy { get; set; }

        public DateTime UpdateDate { get; set; }
    }
}
