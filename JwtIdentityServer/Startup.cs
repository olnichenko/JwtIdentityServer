using DAL;
using DAL.Models;
using DAL.Repositories;
using JwtIdentityServer.ConfigurationModels;
using JwtIdentityServer.Services;
using JwtIdentityServer.Validators;

namespace JwtIdentityServer
{
    public static class Startup
    {
        public static void ConfigureServices(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(Program));
            services.AddControllersWithViews();
        }
        public static void MapSettings(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<DbSettingsModel>(configuration.GetSection(DbSettingsModel.SectionName));
            services.Configure<TokenSettingsModel>(configuration.GetSection(TokenSettingsModel.SectionName));
            services.Configure<IdentitySettingsModel>(configuration.GetSection(IdentitySettingsModel.SectionName));
            services.Configure<MailSettingsModel>(configuration.GetSection(MailSettingsModel.SectionName));

            var dbSettings = configuration.GetSection(DbSettingsModel.SectionName).Get<DbSettingsModel>();
            services.AddScoped((_) => new AppDbContext(dbSettings.ConnectionString));
        }
        public static void MapRepositories(this IServiceCollection services)
        {
            services.AddScoped<IUserRepository<User, long>, UserRepository>();
            services.AddScoped<IResetPasswordKeyRepository<ResetPasswordKey, Guid>, ResetPasswordKeyRepository>();
        }
        public static void MapServices(this IServiceCollection services)
        {
            services.AddScoped<IValidator<User>, UserValidator>();

            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IResetPasswordKeyService, ResetPasswordKeyService>();
            services.AddScoped<IMailService, SmtpMailService>();
        }
    }
}
