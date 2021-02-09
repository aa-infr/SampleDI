using Infrabel.ICT.Framework.Entity;
using System.Diagnostics;

namespace Infrabel.ICT.Framework.Extended.EntityFramework.Models
{
    [DebuggerDisplay("Id = {Id} - Name = {Name}")]
    public sealed class Lexicon : EntityBase, ILexicon
    {
        public string Name { get; set; }
        public string EntityName { get; set; }
        public string FromProperty { get; set; }
        public string ToProperty { get; set; }
        public int LocalLexiconId { get; set; }
        public int ItemCount { get; set; } = 0;
    }
}