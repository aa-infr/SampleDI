using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ICT.Template.Core.Entities;
using ICT.Template.Core.Services;
using ICT.Template.Core.Services.Models;
using Infrabel.ICT.Framework.Exception;
using Infrabel.ICT.Framework.Service;

namespace ICT.Template.Infrastructure.Services
{

    public class SampleService : ISampleService
    {


        public SampleService()
        {

        }

        public  List<SampleDto> GetSamples()
        {
            var item1 = new SampleDto() { Id = 1 };
            var list = new List<SampleDto>();
            list.Add(item1);
            return list;
        }

        public SampleDto GetSampleById(long id)
        {
          return new SampleDto() { Id = 1 };
        }
  }
}