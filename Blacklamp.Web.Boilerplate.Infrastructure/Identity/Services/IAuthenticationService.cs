using System.Threading.Tasks;
using Blacklamp.Web.Boilerplate.Infrastructure.Identity.Models;

namespace Blacklamp.Web.Boilerplate.Infrastructure.Identity.Services
{
    public interface IAuthenticationService
    {
        Task<TokenResponse> Authenticate(TokenRequest request, string ipAddress = "");
    }
}