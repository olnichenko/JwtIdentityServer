using DAL;
using DAL.Models;
using DAL.Repositories;
using JwtIdentityServer.Validators;
using System.Security.Cryptography;
using System.Text;

namespace JwtIdentityServer.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository<User, long> _userRepository;
        private readonly IValidator<User> _userValidator;
        
        public UserService(IValidator<User> userValidator, IUserRepository<User, long> userRepository)
        {
            _userRepository = userRepository;
            _userValidator = userValidator;
        }

        public async Task<bool> ConfirmUserEmail(Guid confirmationKey)
        {
            var user = _userRepository.Get(x => x.EmailConfirmationKey == confirmationKey).SingleOrDefault();
            if (user != null)
            {
                user.IsEmailConfirmed = true;
                await _userRepository.Update(user);
                return true;
            }
            return false;
        }

        public async Task<User> CreateUser(User user)
        {
            var valResult = _userValidator.Validate(user);
            if (!valResult)
            {
                return user;
            }
            user.Password = GetHash(user.Password);
            user.EmailConfirmationKey = Guid.NewGuid();
            var result = await _userRepository.Create(user);
            return result;
        }
        public User? GetByEmailAndPassword(string email, string password)
        {
            password = GetHash(password);
            var user = _userRepository.Get(_ => _.Password == password && _.Email == email).SingleOrDefault();
            return user;
        }
        public User? GetByEmailWithResetPasswordKeys(string email)
        {
            var user = _userRepository.GetWithInclude(_ => _.Email == email, _ => _.resetPasswordKeys).SingleOrDefault();
            return user;
        }
        public async Task<bool> ResetUserPassword(Guid resetPasswordKey, string newPassword)
        {
            newPassword = GetHash(newPassword);
            return await _userRepository.ChangePassword(resetPasswordKey, newPassword);
        }
        protected virtual string GetHash(string text)
        {  
            using (var sha256 = SHA256.Create())
            { 
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(text));  
                return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            }
        }
    }
}
