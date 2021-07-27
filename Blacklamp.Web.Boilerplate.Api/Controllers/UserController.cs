using System.Net.Security;
using System.Threading.Tasks;
using Blacklamp.Web.Boilerplate.Api.Request;
using Blacklamp.Web.Boilerplate.Infrastructure.Identity.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Blacklamp.Web.Boilerplate.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController
    {
        private readonly IMediator _mediator;

        public UserController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [AllowAnonymous]
        [HttpPost("Authenticate")]
        [ProducesResponseType(Microsoft.AspNetCore.Http.StatusCodes.Status200OK)]
        [ProducesResponseType(Microsoft.AspNetCore.Http.StatusCodes.Status400BadRequest)]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Post))]
        public async Task<TokenResponse> AuthenticateAsync([FromBody] Authenticate.AuthenticateCommand command)
        {
            var response = await _mediator.Send(command);
            return response.Resource;
        }
    }
}