namespace JwtIdentityServer.ConfigurationModels
{
    public class DbSettingsModel
    {
        public static string SectionName = "DbSettings";
        public string ConnectionString { get; set; }
    }
}
