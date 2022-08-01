using DAL;
using DAL.Models;
using JwtIdentityServer.Services;
using JwtIdentityServer.Validators;
using Microsoft.AspNetCore.Mvc;

namespace JwtIdentityServer.Controllers
{
    public class AccountController : BaseController
    {
        private UserService _userService;
        public AccountController(AppDbContext appDbContext) : base(appDbContext)
        {
            var userValidator = new UserValidator();
            _userService = new UserService(appDbContext, userValidator);
        }

        public async Task<User> Register(User user)
        {
            var result = await _userService.CreateUser(user);
            return result;
        }
    }
}
