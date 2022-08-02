using DAL.Models;

namespace JwtIdentityServer.Services
{
    public interface IResetPasswordKeyService
    {
        Task<string> CreateResetPasswordLink(User user);
        Task<bool> SetResetKeyAsUsed(Guid resetKey);
    }
}
