using System.Collections.Generic;

namespace Infrabel.ICT.Framework.Service
{
    public interface ICustomMediaTypeService
    {
        IEnumerable<string> GetJsonTypes();

        IEnumerable<string> GetXmlTypes();

        IEnumerable<string> GetAll();
    }
}