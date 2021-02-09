using ICT.Template.Core.Entities;
using ICT.Template.Core.Repositories;
using Infrabel.ICT.Framework.Extended.EntityFramework;
using Infrabel.ICT.Framework.Service;

namespace ICT.Template.Infrastructure.Data
{
    public class SampleRepository : DbRepository<Sample>, ISampleRepository
    {
        public SampleRepository(IDbContextFactory factory, IUserContext userContext, ILexiconFactoryService lexiconFactoryService, IDateService dateService) : base(factory, userContext, lexiconFactoryService, dateService)
        {
        }
    }
}