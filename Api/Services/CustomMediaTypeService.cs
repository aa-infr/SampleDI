using Infrabel.ICT.Framework.Ioc;
using Infrabel.ICT.Framework.Service;
using System.Collections.Generic;

namespace ICT.Template.Api.Services
{
    [IoCRegistration(RegistrationLifeTime.None)]
    public class CustomMediaTypeService : ICustomMediaTypeService
    {
        public IEnumerable<string> GetJsonTypes()
        {
            return new string[] { };
        }

        public IEnumerable<string> GetXmlTypes()
        {
            return new string[] { };
        }

        public IEnumerable<string> GetAll()
        {
            return new string[] { };
        }
    }
}