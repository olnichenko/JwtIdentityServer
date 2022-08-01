namespace JwtIdentityServer.ConfigurationModels
{
    public class TokenSettingsModel
    {
        public static string SectionName = "TokenSettings";
        public string SecretKey { get; set; }
        public string Issuer { get; set; }
        public int ValidTokenHours { get; set; }
    }
}
