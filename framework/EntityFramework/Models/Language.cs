using Infrabel.ICT.Framework.Entity;
using System.Diagnostics;

namespace Infrabel.ICT.Framework.Extended.EntityFramework.Models
{
    [DebuggerDisplay("Id = {Id} - Name = {Name}")]
    public sealed class Language : EntityBase, ILanguage
    {
        public string Name { get; set; }
        public string IsoCode { get; set; }
        public bool Default { get; set; }
    }
}