using DAL.Models;

namespace JwtIdentityServer.Services
{
    public interface IResetPasswordKeyService
    {
        Task<ResetPasswordKey> CreateResetPasswordKey(User user);
        Task<bool> SetResetKeyAsUsed(Guid resetKey);
    }
}
