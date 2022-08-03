using JwtIdentityServer.ConfigurationModels;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace JwtIdentityServer.Services
{
    public class SmtpMailService : IMailService
    {
        private SmtpClient _smtpClient;
        private readonly MailSettingsModel _mailSettings;
        private MailAddress _mailAddressFrom;
        private MailAddress _mailAddressTo;
        private MailMessage _mailMessage;
        public SmtpMailService(IOptions<MailSettingsModel> mailSettings)
        {
            _mailSettings = mailSettings.Value;
        }
        public void SendEmailConfirmationMessage(string emailTo, Guid confirmEmailKey)
        {
            ConfigureClient();
            ConfigureMailMessage(emailTo);
            _mailMessage.Body = $"Email confirmation link: {_mailSettings.ConfirmEmailUrl}{confirmEmailKey}";
            _mailMessage.Subject = _mailSettings.ConfirmEmailSubject;
            SendMail();
        }
        public void SendResetPasswordMessage(string emailTo, Guid resetPasswordKey)
        {
            ConfigureClient();
            ConfigureMailMessage(emailTo);
            _mailMessage.Body = $"Reset password link: {_mailSettings.PasswordResetUrl}{resetPasswordKey}";
            _mailMessage.Subject = _mailSettings.PasswordResetSubject;
            SendMail();
        }
        protected virtual async void SendMail()
        {
            await _smtpClient.SendMailAsync(_mailMessage);
        }
        protected virtual void ConfigureClient()
        {
            _smtpClient = new SmtpClient(_mailSettings.Host, _mailSettings.Port);
            var credentials = new NetworkCredential(_mailSettings.Mail, _mailSettings.Password);
            _smtpClient.Credentials = credentials;
            _smtpClient.EnableSsl = true;
            _mailAddressFrom = new MailAddress(_mailSettings.Mail, _mailSettings.DisplayName);
        }
        protected virtual void ConfigureMailMessage(string emailTo)
        {
            _mailAddressTo = new MailAddress(emailTo);
            _mailMessage = new MailMessage(_mailAddressFrom, _mailAddressTo);
        }
    }
}
