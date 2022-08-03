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
        private readonly IMapper _mapper;
        private readonly IResetPasswordKeyService _resetPasswordKeyService;
        private readonly IMailService _mailService;
        public AccountController(ITokenService tokenService, IMapper mapper, IUserService userService, IResetPasswordKeyService resetPasswordKeyService, IMailService mailService)
        {
            _userService = userService;
            _tokenService = tokenService;
            _mapper = mapper;
            _resetPasswordKeyService = resetPasswordKeyService;
            _mailService = mailService;
        }

        [HttpPost]
        public async Task<string> Register(UserVM userVM)
        {
            var user = _mapper.Map<User>(userVM);
            var resultUser = await _userService.CreateUser(user);
            var resultMessage = "Error";
            if (resultUser.Id > 0)
            {
                _mailService.SendEmailConfirmationMessage(resultUser.Email, resultUser.EmailConfirmationKey);
                resultMessage = "Confirm your EMAIL";
            }
            return resultMessage;
        }

        [HttpPost]
        public string GetToken(string email, string password)
        {
            var user = _userService.GetByEmailAndPassword(email, password);
            var token = string.Empty;
            if (user != null && user.IsEmailConfirmed)
            {
                token = _tokenService.GenerateToken(user);
            }
            return token;
        }

        [HttpPost]
        public async Task<string> ResetPassword(string email)
        {
            var user = _userService.GetByEmailWithResetPasswordKeys(email);
            if (user != null && user.IsEmailConfirmed)
            {
                var result = await _resetPasswordKeyService.CreateResetPasswordKey(user);
                if (result != null)
                {
                    _mailService.SendResetPasswordMessage(user.Email, result.Id);
                    return "Message sended";
                }
            }
            return "Error";
        }

        [HttpGet("{emailConfirmationKey}")]
        public async Task<bool> ConfirmEmail(Guid emailConfirmationKey)
        {
            var result = await _userService.ConfirmUserEmail(emailConfirmationKey);
            return result;
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
