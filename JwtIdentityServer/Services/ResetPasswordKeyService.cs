using DAL;
using DAL.Models;
using DAL.Repositories;
using JwtIdentityServer.ConfigurationModels;
using Microsoft.Extensions.Options;

namespace JwtIdentityServer.Services
{
    public class ResetPasswordKeyService
    {
        private readonly IdentitySettingsModel _identitySettingsModel;
        private readonly ResetPasswordKeyRepository _resetPasswordKeyRepository;
        public ResetPasswordKeyService(AppDbContext appDbContext, IOptions<IdentitySettingsModel> identitySettingsModel)
        {
            _identitySettingsModel = identitySettingsModel.Value;
            _resetPasswordKeyRepository = new ResetPasswordKeyRepository(appDbContext);
        }
        public async Task<string> CreateResetPasswordLink(User user)
        {
            var resetPasswordKey = new ResetPasswordKey
            {
                ExpirationDate = DateTime.UtcNow.AddHours(_identitySettingsModel.PasswordResetLinkExpirationDateHours),
                User = user
            };

            var result = await _resetPasswordKeyRepository.Create(resetPasswordKey, user);

            var linkText = _identitySettingsModel.PasswordResetLinkUrl + result.Id;
            return linkText;
        }

        public async Task<bool> SetResetKeyAsUsed(Guid resetKey)
        {
            var resetPasswordKey = await _resetPasswordKeyRepository.FindById(resetKey);
            if (resetPasswordKey != null)
            {
                resetPasswordKey.IsUsed = true;
                var result = await _resetPasswordKeyRepository.Update(resetPasswordKey);
                return result.IsUsed;
            }
            return false;
        }
    }
}
