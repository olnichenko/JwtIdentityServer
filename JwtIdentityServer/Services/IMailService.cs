namespace JwtIdentityServer.Services
{
    public interface IMailService
    {
        void SendEmailConfirmationMessage(string emailTo, Guid confirmEmailKey);
        void SendResetPasswordMessage(string emailTo, Guid resetPasswordKey);
    }
}
