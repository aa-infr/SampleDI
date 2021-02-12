using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ICT.Template.Core.Services;
using ICT.Template.Core.Services.Models;
using Infrabel.ICT.Framework.Extended.AspNetCore.Extension;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ICT.Template.Api.Controllers
{
  [ApiController]
  [Route("samples")]
  public class SamplesController : ControllerBase
    {
        private readonly ISampleService _sampleService;

        public SamplesController(ISampleService sampleService)
        { _sampleService = sampleService ?? throw new ArgumentNullException(nameof(sampleService)); }

        [HttpGet(Name = nameof(GetAllSamples))]
        public IActionResult GetAllSamples()
        { return Ok(_sampleService.GetSamples()); }

        [HttpGet("{id}", Name = nameof(GetSampleById))]
        public IActionResult GetSampleById(long id)
        { return Ok(_sampleService.GetSampleById(id)); }


    }
}