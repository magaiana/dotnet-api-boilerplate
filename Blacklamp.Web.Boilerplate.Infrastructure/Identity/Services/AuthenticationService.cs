using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Blacklamp.Web.Boilerplate.Infrastructure.Identity.Models;
using Blacklamp.Web.Boilerplate.Infrastructure.Identity.Models.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Blacklamp.Web.Boilerplate.Infrastructure.Identity.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly TokenSettings _token;
        private readonly HttpContext _httpContext;

        public AuthenticationService(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager,
            IOptions<TokenSettings> options, IHttpContextAccessor httpContextAccessor)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _token = options.Value;
            _httpContext = httpContextAccessor.HttpContext;
        }


        public async Task<TokenResponse> Authenticate(TokenRequest request, string ipAddress = "")
        {
            if (await IsValidUser(request.Username, request.Password))
            {
                ApplicationUser user = await GetUserByEmail(request.Username);

                if (user != null && user.IsEnabled)
                {
                    var role = (await _userManager.GetRolesAsync(user))[0];
                    var jwtToken = await GenerateJwtToken(user);

                    await _userManager.UpdateAsync(user);

                    return new TokenResponse(
                        user, role, jwtToken
                    );
                }
            }

            return null;
        }

        
        private async Task<bool> IsValidUser(string username, string password)
        {
            var user = await GetUserByEmail(username);
            if (user == null)
            {
                return false;
            }

            return await _userManager.CheckPasswordAsync(user, password);
        }
        
        private async Task<ApplicationUser> GetUserByEmail(string username) =>
            await _userManager.FindByEmailAsync(username);

        
        private async Task<string> GenerateJwtToken(ApplicationUser user)
        {
            var role = (await _userManager.GetRolesAsync(user))[0];
            var secret = Encoding.ASCII.GetBytes(_token.Secret);

            var handler = new JwtSecurityTokenHandler();
            var descriptor = new SecurityTokenDescriptor
            {
                Issuer = _token.Issuer,
                Audience = _token.Audience,
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("UserId", user.Id),
                    new Claim("FullName", user.FullName),
                    new Claim(ClaimTypes.Name, user.Email),
                    new Claim(ClaimTypes.NameIdentifier, user.Email),
                    new Claim(ClaimTypes.Role, role),
                }),
                Expires = DateTime.UtcNow.AddMinutes(_token.Expiry),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(secret),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = handler.CreateToken(descriptor);
            return handler.WriteToken(token);
        }
    }
}