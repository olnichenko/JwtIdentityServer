namespace JwtIdentityServer.ConfigurationModels
{
    public class IdentitySettingsModel
    {
        public static string SectionName = "IdentitySettings";
        public int PasswordResetLinkExpirationDateHours { get; set; }
    }
}
