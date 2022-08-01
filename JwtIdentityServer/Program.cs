using DAL;
using JwtIdentityServer.ConfigurationModels;

var builder = WebApplication.CreateBuilder(args);

// if is Development get setting from appsettings.<computer name>.json
// else standart settings
if (builder.Environment.EnvironmentName == "Development")
{
    builder.Configuration.SetBasePath(builder.Environment.ContentRootPath)
        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
        .AddJsonFile($"appsettings.{Environment.MachineName}.json", optional: true)
        .AddEnvironmentVariables();
}

builder.Services.Configure<DbSettingsModel>(
    builder.Configuration.GetSection(DbSettingsModel.SectionName));

var dbSettings = builder.Configuration.GetSection(DbSettingsModel.SectionName).Get<DbSettingsModel>();
builder.Services.AddScoped((_) => new AppDbContext(dbSettings.ConnectionString));

// Add services to the container.

builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseRouting();

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapDefaultControllerRoute();
});

app.MapControllers();

app.Run();
