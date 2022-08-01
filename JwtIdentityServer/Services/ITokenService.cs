using DAL.Models;

namespace JwtIdentityServer.Services
{
    public interface ITokenService
    {
        public string GenerateToken(User user);
        public bool ValidateToken(string token);
    }
}
