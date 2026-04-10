using FluentValidation;
using Kin.KinHub.Core.Function.Middlewares;
using Kin.KinHub.Core.Function.Validators;
using Kin.KinHub.Core.Function.Validators.Interfaces;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

builder.UseMiddleware<JwtAuthenticationMiddleware>();

var dataDirectory = builder.Configuration["JsonDataDirectory"]
    ?? Path.Combine(AppContext.BaseDirectory, "Data");

builder.Services
    .AddValidatorsFromAssemblyContaining<Program>(ServiceLifetime.Scoped, includeInternalTypes: true)
    .AddScoped(typeof(IRequestValidator<>), typeof(FluentRequestValidator<>))
    .AddKinHubCoreJsonInfrastructure(o => o.DataDirectory = dataDirectory)
    .AddKinHubJwtInfrastructure(o =>
    {
        o.Secret = builder.Configuration["Jwt:Secret"]
            ?? "CHANGE-ME-use-a-long-random-secret-at-least-32-chars!";
        o.AccessTokenExpiryMinutes = int.Parse(builder.Configuration["Jwt:AccessTokenExpiryMinutes"] ?? "15");
        o.Issuer = builder.Configuration["Jwt:Issuer"]
            ?? "kinhub";
    })
    .AddKinHubCoreBusiness();

builder.Build().Run();
