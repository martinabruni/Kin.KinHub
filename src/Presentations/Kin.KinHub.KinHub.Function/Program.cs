using FluentValidation;
using Kin.KinHub.KinHub.Function.Middleware;
using Kin.KinHub.KinHub.Function.Validators;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

builder.UseMiddleware<JwtAuthenticationMiddleware>();

var dataDirectory = builder.Configuration["JsonDataDirectory"]
    ?? Path.Combine(AppContext.BaseDirectory, "Data");

var jwtSecret = builder.Configuration["Jwt:Secret"]
    ?? "CHANGE-ME-use-a-long-random-secret-at-least-32-chars!";

var jwtIssuer = builder.Configuration["Jwt:Issuer"]
    ?? "kinhub";

builder.Services
    .AddValidatorsFromAssemblyContaining<Program>(ServiceLifetime.Scoped, includeInternalTypes: true)
    .AddScoped(typeof(IRequestValidator<>), typeof(FluentRequestValidator<>))
    .AddKinHubJsonInfrastructure(o => o.DataDirectory = dataDirectory)
    .AddKinHubJwtInfrastructure(o =>
    {
        o.Secret = jwtSecret;
        o.Issuer = jwtIssuer;
    })
    .AddKinHubBusiness(_ => { });

builder.Build().Run();
