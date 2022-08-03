namespace JwtIdentityServer.ConfigurationModels
{
    public class MailSettingsModel
    {
        public static string SectionName = "MailSettings";
        public string Mail { get; set; }
        public string DisplayName { get; set; }
        public string Password { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
        public string ConfirmEmailUrl { get; set; }
        public string ConfirmEmailSubject { get; set; }
        public string Security { get; set; }
        public string PasswordResetUrl { get; set; }
        public string PasswordResetSubject { get; set; }
    }
}
