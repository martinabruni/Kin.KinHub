using Azure.Monitor.OpenTelemetry.AspNetCore;
using FluentValidation;
using Kin.KinHub.Core.Api.Middlewares;
using Kin.KinHub.Core.Api.Validators;
using Kin.KinHub.Core.Api.Validators.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddValidatorsFromAssemblyContaining<Program>(ServiceLifetime.Scoped, includeInternalTypes: true)
    .AddScoped(typeof(IRequestValidator<>), typeof(FluentRequestValidator<>))
    .AddKinHubCoreSqlInfrastructure(o => o.ConnectionString = builder.Configuration.GetConnectionString("KinHub")!)
    .AddKinHubJwtInfrastructure(o =>
    {
        o.Secret = builder.Configuration["Jwt:Secret"]
            ?? "CHANGE-ME-use-a-long-random-secret-at-least-32-chars!";
        o.AccessTokenExpiryMinutes = int.Parse(builder.Configuration["Jwt:AccessTokenExpiryMinutes"] ?? "15");
        o.Issuer = builder.Configuration["Jwt:Issuer"]
            ?? "kinhub";
    })
    .AddKinHubCoreBusiness();

builder.Services.AddOpenTelemetry().UseAzureMonitor();
builder.Services
    .AddHealthChecks()
    .AddSqlServer(builder.Configuration.GetConnectionString("KinHub")!);

builder.Services.AddScoped<JwtAuthenticationMiddleware>();
builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddCors(options =>
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader()));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseMiddleware<JwtAuthenticationMiddleware>();
app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/health");

app.Run();
