using System.Diagnostics;
using Infrabel.ICT.Framework.Entity;

namespace Infrabel.ICT.Framework.Extended.EntityFramework.Models
{
    [DebuggerDisplay("SearchableInfo = {SearchableType.ToString()} - {ColumnName}")]
    internal sealed class SearchableInfo
    {
        public SearchableInfo(SearchableType searchableType,string columnName)
        {
            SearchableType = searchableType;
            ColumnName = columnName;
        }

        public SearchableType SearchableType { get;  }

        public string ColumnName { get;  }

    }
}
