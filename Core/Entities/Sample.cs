using Infrabel.ICT.Framework.Entity;
using System.ComponentModel.DataAnnotations;

namespace ICT.Template.Core.Entities
{
    public class Sample : AuditableEntity
    {
        public string Name { get; set; }

        public string NullableName { get; set; }
        public int NumberInt { get; set; }
        public double NumberDouble { get; set; }
        public int? NumberIntNullable { get; set; }
        public double? NumberDoubleNullable { get; set; }

        public void SetId(long id) => Id = id;
    }
}