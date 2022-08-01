using DAL.Models;

namespace JwtIdentityServer.Validators
{
    public class UserValidator : IValidator<User>
    {
        public bool Validate(User entity)
        {
            if (string.IsNullOrEmpty(entity.Email) || string.IsNullOrEmpty(entity.Password))
            {
                return false;
            }
            return true;
        }
    }
}
