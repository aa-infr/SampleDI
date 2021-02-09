using System;

namespace Infrabel.ICT.Framework.Entity
{
    public interface IAuditableEntity: IEntityBase
    {
        string CreatedBy { get; set; }

        DateTime CreationDate { get; set; }

        string UpdatedBy { get; set; }

        DateTime UpdateDate { get; set; }
    }
}