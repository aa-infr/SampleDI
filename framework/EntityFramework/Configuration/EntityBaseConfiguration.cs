using Infrabel.ICT.Framework.Entity;
using Infrabel.ICT.Framework.Extended.EntityFramework.Configuration.Rule;
using Infrabel.ICT.Framework.Extended.EntityFramework.Extension;
using Infrabel.ICT.Framework.Ioc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Diagnostics.CodeAnalysis;

namespace Infrabel.ICT.Framework.Extended.EntityFramework.Configuration
{
    [ExcludeFromCodeCoverage]
    [IoCRegistration(RegistrationLifeTime.Transient)]
    public abstract class EntityBaseConfiguration
    {
        protected abstract string TableAlias { get; }

        protected abstract string SchemaName { get; }

        protected virtual string ContextName { get; } = string.Empty;

        protected abstract bool UseDefaultConventions { get; }

        public string GetContextName() => ContextName ?? string.Empty;
    }

    [ExcludeFromCodeCoverage]
    public abstract class EntityBaseConfiguration<T> : EntityBaseConfiguration, IEntityTypeConfiguration<T>
            where T : class, IEntityBase, new()
    {
        private readonly IConfigurationRulesService<T> _rulesService;
        private readonly DataBaseProvider _provider;
        private static readonly string DefaultColumnIdSuffix = @"Id";
        private static readonly string BaseQuery = @"SELECT {0},{1} FROM {2}";

        protected EntityBaseConfiguration(DataBaseProvider dataBaseProvider = DataBaseProvider.MySql, IConfigurationRulesService<T> rulesService = null)
        {
            _rulesService = rulesService;
            _provider = dataBaseProvider;
            EntityProvider = dataBaseProvider;
            TableName = DbColumnBuilder.CreateWithNormalizedValue(typeof(T).Name, _provider)
                                .Build();
            ColumnIdSuffix = DbColumnBuilder.CreateWithNormalizedValue(DefaultColumnIdSuffix, _provider)
                                     .Build();
        }

        protected virtual string TableName { get; }

        protected virtual string ColumnIdSuffix { get; }

        public void Configure(EntityTypeBuilder<T> builder) { Map(builder); }

        protected virtual void Map(EntityTypeBuilder<T> builder)
        {
            MapPrimaryKey(builder);
            MapProperties(builder);
            MapConfigurationRules(builder);
            MapTable(builder);
        }

        // !!!!!!!!!!!!!!! Dont remove, used by lexicon dynamic configuration !!!!!!!!!!!!!
        internal DataBaseProvider GetDataBaseProvider() => _provider;

        internal string GetTableAlias() => TableAlias;

        internal string GetTableName() => TableName;

        internal string GetSchemaName() => SchemaName;

        protected abstract void MapProperties(EntityTypeBuilder<T> builder);

        internal string[] GetEntityProperties() => EntityTypeBuilderExtensions.GetProperties(typeof(T));

        protected virtual string GetPrimaryKey() => DbColumnBuilder.CreateWithPristineValue(TableAlias, _provider)
                                                            .AddPristine(ColumnIdSuffix)
                                                            .Build();

        protected string GetTableFullName() => _provider == DataBaseProvider.SqlServer && !string.IsNullOrWhiteSpace(SchemaName) ? DbColumnBuilder.CreateWithPristineValue(TableName, _provider)
                                                                                                                                           .Build()
                                                       : DbColumnBuilder.CreateWithPristineValue(SchemaName, _provider)
                                                                 .AddPristine(TableName)
                                                                 .Build();

        internal string GetSql() => string.Format(BaseQuery, GetPrimaryKey(), GetSelect(), GetTableFullName());

        private void MapTable(EntityTypeBuilder<T> builder)
        {
            // cache configuration
            if(_provider == DataBaseProvider.SqlServer && !string.IsNullOrWhiteSpace(SchemaName))
                builder.ToTable(GetTableFullName(), SchemaName);
            else
                builder.ToTable(GetTableFullName());
        }

        private string GetSelect() => EntityTypeBuilderExtensions.GetSelect(typeof(T));

        private void MapPrimaryKey(EntityTypeBuilder<T> builder)
        {
            builder.Property(x => x.Id)
                    .HasColumnName(GetPrimaryKey())
                    .ValueGeneratedOnAdd();
            builder.HasKey(x => x.Id);
        }

        private void MapConfigurationRules(EntityTypeBuilder<T> builder)
        {
            if(_rulesService == null)
                return;

            if(UseDefaultConventions)
                _rulesService.Process(builder, EntityProvider, x => x.IsGeneric);
            else _rulesService.Process(builder, EntityProvider, x => !x.IsGeneric);
        }

        private static DataBaseProvider EntityProvider;

        internal static DataBaseProvider GetProvider() => EntityProvider;
    }
}