using DAL;
using DAL.Models;
using JwtIdentityServer.ConfigurationModels;
using JwtIdentityServer.Services;
using JwtIdentityServer.Validators;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace JwtIdentityServer.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class AccountController : Controller
    {
        private readonly UserService _userService;
        private readonly ITokenService _tokenService;
        private readonly IOptions<IdentitySettingsModel> _identitySettingsModel;
        private readonly AppDbContext _appDbContext;
        public AccountController(AppDbContext appDbContext, ITokenService tokenService, IOptions<IdentitySettingsModel> identitySettingsModel)
        {
            var userValidator = new UserValidator();
            _appDbContext = appDbContext;
            _userService = new UserService(_appDbContext, userValidator);
            _tokenService = tokenService;
            _identitySettingsModel = identitySettingsModel;
        }

        [HttpPost]
        public async Task<string> Register(User user)
        {
            var resultUser = await _userService.CreateUser(user);
            var token = string.Empty;
            if (resultUser.Id > 0)
            {
                token = _tokenService.GenerateToken(resultUser);
            }
            return token;
        }

        [HttpPost]
        public string GetToken(string email, string password)
        {
            var user = _userService.GetByEmailAndPassword(email, password);
            var token = string.Empty;
            if (user != null)
            {
                token = _tokenService.GenerateToken(user);
            }
            return token;
        }

        [HttpPost]
        public async Task<string> ResetPassword(string email)
        {
            var user = _userService.GetByEmailWithResetPasswordKeys(email);
            var link = string.Empty;
            if (user != null)
            {
                var resetPasswordService = new ResetPasswordKeyService(_appDbContext, _identitySettingsModel);
                link = await resetPasswordService.CreateResetPasswordLink(user);
            }
            return link;
        }

        [HttpGet("{resetKey}")]
        public ActionResult ResetPassword(Guid resetKey)
        {
            return View(resetKey);
        }

        [HttpPost("{resetKey}")]
        public async Task<bool> ResetPassword(Guid resetKey, [FromForm] string newPassword)
        {
            var result = await _userService.ResetUserPassword(resetKey, newPassword);
            if (result)
            {
                var resetPasswordService = new ResetPasswordKeyService(_appDbContext, _identitySettingsModel);
                var resetKeyResult = await resetPasswordService.SetResetKeyAsUsed(resetKey);
                return resetKeyResult;
            }
            return false;
        }

        [HttpPost]
        public bool ValidateToken(string token)
        {
            var result = _tokenService.ValidateToken(token);
            return result;
        }
    }
}
