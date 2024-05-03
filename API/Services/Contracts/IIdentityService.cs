using Contracts.V1.Requests.Auth;
using Contracts.V1.Response.Auth;

namespace API.Services.Contracts
{
    public interface IIdentityService
    {
        Task<AuthResponse> LoginAsync(AuthRequest loginRequest);
    }
}
