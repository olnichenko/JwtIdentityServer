using DAL;
using JwtIdentityServer;
using JwtIdentityServer.ConfigurationModels;
using JwtIdentityServer.Services;

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

// extensions Startup.cs
builder.Services.ConfigureServices();
builder.Services.MapSettings(builder.Configuration);
builder.Services.MapRepositories();
builder.Services.MapServices();

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddSwaggerGen();

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

app.UseSwagger();
app.UseSwaggerUI(c => {
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V2");
});

app.Run();
