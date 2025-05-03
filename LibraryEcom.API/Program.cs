using LibraryEcom.Middleware;
using LibraryEcom.Domain.Common;
using LibraryEcom.Configurations;
using LibraryEcom.Identity.Dependency;
using System.Text.Json.Serialization;
using System.IdentityModel.Tokens.Jwt;
using LibraryEcom.Domain.Common.Property;
using LibraryEcom.Infrastructure.Dependency;
using LibraryEcom.Infrastructure.Persistence;
using Swashbuckle.AspNetCore.SwaggerUI;


var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;

var configuration = builder.Configuration;

builder.AddConfigurations(); 

services.AddControllers().AddJsonOptions(x =>
{
    x.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

services.AddEndpointsApiExplorer(); 

services.AddDependencyServices(); 

services.AddIdentityServices(configuration); 

services.AddInfrastructureService(configuration); 


await services.AddDataSeedMigrationService(); 

services.AddCustomSwaggerGen();

// Log.Logger = new LoggerConfiguration()
//     .ReadFrom.Configuration(builder.Configuration).CreateLogger();


var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

if (app.Environment.IsDevelopment() || app.Environment.IsStaging() || app.Environment.IsProduction())
{
    app.UseSwagger();

    app.UseSwaggerUI(options =>
    {
        options.DocExpansion(DocExpansion.None);
        options.DefaultModelsExpandDepth(-1);
        options.ConfigObject.AdditionalItems.Add("persistAuthorization", "true");
    });
}

app.UseStaticFiles(new StaticFileOptions
{
    OnPrepareResponse = ctx =>
    {
        ctx.Context.Response.Headers.Append("Access-Control-Allow-Origin", "*");
        ctx.Context.Response.Headers.Append("Access-Control-Allow-Methods", "GET, OPTIONS");
        ctx.Context.Response.Headers.Append("Access-Control-Allow-Headers", "Content-Type");
    }
});

app.UseHttpsRedirection();

app.UseCors(Constants.Cors.MyAllowSpecificOrigins);

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();