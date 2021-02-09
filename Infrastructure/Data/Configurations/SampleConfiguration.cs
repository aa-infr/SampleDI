using ICT.Template.Core.Entities;
using Infrabel.ICT.Framework.Entity;
using Infrabel.ICT.Framework.Extended.EntityFramework.Configuration;
using Infrabel.ICT.Framework.Extended.EntityFramework.Configuration.Rule;
using Infrabel.ICT.Framework.Extended.EntityFramework.Extension;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ICT.Template.Infrastructure.Data.Configurations
{
    public class SampleConfiguration : EntityBaseConfiguration<Sample>
    {
        protected override string TableAlias => @"SMPL";
        protected override string SchemaName => "S1891";
        protected override bool UseDefaultConventions => true;

        //protected override string ContextName => nameof(SampleDbContext);

        protected override void MapProperties(EntityTypeBuilder<Sample> builder)
        {
            builder.AddProperty(x => x.Name);
            builder.AddProperty(x => x.NullableName);
            builder.AddProperty(x => x.NumberInt);
            builder.AddProperty(x => x.NumberDouble);
            builder.AddProperty(x => x.NumberIntNullable);
            builder.AddProperty(x => x.NumberDoubleNullable);
        }

        public SampleConfiguration(IConfigurationRulesService<Sample> rulesService) : base(DataBaseProvider.Oracle, rulesService)
        {
        }
    }
}