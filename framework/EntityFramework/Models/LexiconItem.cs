using Infrabel.ICT.Framework.Entity;
using System.Diagnostics;

namespace Infrabel.ICT.Framework.Extended.EntityFramework.Models
{
    [DebuggerDisplay("LexiconId = {LexiconId} - Value = {Value} - TargetItemId = {TargetItemId} - LanguageId = {LanguageId}")]
    public sealed class LexiconItem : EntityBase, ILexiconItem
    {
        public long? LexiconId { get; set; }

        public string Value { get; set; }

        public long? TargetItemId { get; set; }

        public long? LanguageId { get; set; }
    }
}