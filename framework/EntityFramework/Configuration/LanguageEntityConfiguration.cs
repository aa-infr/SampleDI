using Infrabel.ICT.Framework.Entity;
using Infrabel.ICT.Framework.Extended.EntityFramework.Configuration.Rule;
using Infrabel.ICT.Framework.Extended.EntityFramework.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

namespace Infrabel.ICT.Framework.Extended.EntityFramework.Configuration
{
    [ExcludeFromCodeCoverage]
    internal class LanguageEntityConfiguration : EntityBaseConfiguration<Language>
    {
        private readonly string _tableAlias;
        private readonly string _schemaName;
        private readonly string _tableName;

        public LanguageEntityConfiguration(DataBaseProvider driver, string alias, string schemaName, string tableName, IConfigurationRulesService<Language> rulesService = null) : base(driver, rulesService)
        {
            _tableAlias = alias;
            _schemaName = schemaName;
            _tableName = tableName;
        }

        protected override string TableAlias => _tableAlias;
        protected override string SchemaName => _schemaName;
        protected override bool UseDefaultConventions => false;
        protected override string TableName => _tableName;

        protected override void MapProperties(EntityTypeBuilder<Language> builder) => Expression.Empty();
    }
}