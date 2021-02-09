using Infrabel.ICT.Framework.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;

namespace ICT.Template.Api.Controllers
{
    [ApiController]
    [Route("roles")]
    [Authorize("Elevated")]
    public class RolesController : ControllerBase
    {
        private readonly IUserContext _context;

        public RolesController(IUserContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        [HttpGet(Name = nameof(GetRoles))]
        public IActionResult GetRoles()
        {
            return Ok(_context.Identity.GetRoles());
        }
    }
}