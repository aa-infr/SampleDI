using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ICT.Template.Core.Services.Models;

namespace ICT.Template.Core.Services
{
    public interface ISampleService
    {
    List<SampleDto> GetSamples();

        SampleDto GetSampleById(long id);
    }
}