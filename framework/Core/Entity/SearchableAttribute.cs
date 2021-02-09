using System;

namespace Infrabel.ICT.Framework.Entity
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class SearchableAttribute : Attribute
    {
        public SearchableAttribute(SearchableType type = SearchableType.None) => Type = type;

        public SearchableType Type { get; }
    }
}