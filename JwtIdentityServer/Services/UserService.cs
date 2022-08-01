using DAL;
using DAL.Models;
using DAL.Repositories;
using JwtIdentityServer.Validators;
using System.Security.Cryptography;
using System.Text;

namespace JwtIdentityServer.Services
{
    public class UserService
    {
        private UserRepository _userRepository;
        private UserValidator _userValidator;
        public UserService(AppDbContext appDbContext, UserValidator userValidator)
        {
            _userRepository = new UserRepository(appDbContext);
            _userValidator = userValidator;
        }
        public async Task<User> CreateUser(User user)
        {
            var valResult = _userValidator.Validate(user);
            if (!valResult)
            {
                return user;
            }
            user.Password = GetHash(user.Password);
            var result = await _userRepository.Create(user);
            return result;
        }
        private string GetHash(string text)
        {
            // SHA512 is disposable by inheritance.  
            using (var sha256 = SHA256.Create())
            {
                // Send a sample text to hash.  
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(text));
                // Get the hashed string.  
                return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            }
        }
    }
}
