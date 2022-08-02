using AutoMapper;
using DAL;
using DAL.Models;
using JwtIdentityServer.ConfigurationModels;
using JwtIdentityServer.Services;
using JwtIdentityServer.Validators;
using JwtIdentityServer.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace JwtIdentityServer.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class AccountController : Controller
    {
        private readonly IUserService _userService;
        private readonly ITokenService _tokenService;
        private readonly IdentitySettingsModel _identitySettingsModel;
        private readonly IMapper _mapper;
        private readonly IResetPasswordKeyService _resetPasswordKeyService;
        public AccountController(ITokenService tokenService, IOptions<IdentitySettingsModel> identitySettingsModel, IMapper mapper, IUserService userService, IResetPasswordKeyService resetPasswordKeyService)
        {
            _userService = userService;
            _tokenService = tokenService;
            _identitySettingsModel = identitySettingsModel.Value;
            _mapper = mapper;
            _resetPasswordKeyService = resetPasswordKeyService;
        }

        [HttpPost]
        public async Task<string> Register(UserVM userVM)
        {
            var user = _mapper.Map<User>(userVM);
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
                link = await _resetPasswordKeyService.CreateResetPasswordLink(user);
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
            {;
                await _resetPasswordKeyService.SetResetKeyAsUsed(resetKey);
                return true;
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
