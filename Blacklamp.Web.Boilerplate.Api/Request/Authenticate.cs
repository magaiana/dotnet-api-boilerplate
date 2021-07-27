using System;
using System.Security.Authentication;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Blacklamp.Web.Boilerplate.Infrastructure.Identity.Models;
using Blacklamp.Web.Boilerplate.Infrastructure.Identity.Services;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Blacklamp.Web.Boilerplate.Api.Request
{
    public class Authenticate
    {
        public class AuthenticateCommand : TokenRequest, IRequest<CommandResponse>
        {
        }

        public class CommandResponse
        {
            public TokenResponse Resource { get; set; }
        }

        public class AuthenticateCommandValidator : AbstractValidator<AuthenticateCommand>
        {
            public AuthenticateCommandValidator()
            {
                RuleFor(x => x.Username)
                    .Cascade(CascadeMode.Stop)
                    .NotEmpty();

                RuleFor(x => x.Password)
                    .Cascade(CascadeMode.Stop)
                    .NotEmpty();
            }
        }

        public class CommandHandler : IRequestHandler<AuthenticateCommand, CommandResponse>
        {
            private readonly IMapper _mapper;
            private readonly IAuthenticationService _tokenService;
            private readonly HttpContext _httpContext;

            public CommandHandler(IAuthenticationService tokenService, IMapper mapper, IHttpContextAccessor httpContextAccessor)
            {
                _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
                _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
                _httpContext = (httpContextAccessor != null)
                    ? httpContextAccessor.HttpContext
                    : throw new ArgumentNullException(nameof(httpContextAccessor));
            }


            public async Task<CommandResponse> Handle(AuthenticateCommand command, CancellationToken cancellationToken)
            {
                var response = new CommandResponse();
                var ipAddress = string.Empty;

                if (_httpContext.Connection.RemoteIpAddress != null)
                {
                    ipAddress = _httpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
                }

                var tokenResponse = await _tokenService.Authenticate(command, ipAddress);
                if (tokenResponse == null)
                {
                    throw new InvalidCredentialException("Incorrect Username or Password");
                }

                response.Resource = tokenResponse;
                return response;
            }
        }
    }
}