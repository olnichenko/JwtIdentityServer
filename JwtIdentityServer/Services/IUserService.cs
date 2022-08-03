using DAL.Models;

namespace JwtIdentityServer.Services
{
    public interface IUserService
    {
        Task<User> CreateUser(User user);
        User? GetByEmailAndPassword(string email, string password);
        User? GetByEmailWithResetPasswordKeys(string email);
        Task<bool> ResetUserPassword(Guid resetPasswordKey, string newPassword);
        Task<bool> ConfirmUserEmail(Guid confirmationKey);
    }
}
