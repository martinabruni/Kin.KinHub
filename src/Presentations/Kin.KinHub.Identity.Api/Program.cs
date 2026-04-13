using FluentValidation;
using Kin.KinHub.Identity.Api.Middlewares;
using Kin.KinHub.Identity.Api.Validators;
using Kin.KinHub.Identity.Api.Validators.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddValidatorsFromAssemblyContaining<Program>(ServiceLifetime.Scoped, includeInternalTypes: true)
    .AddScoped(typeof(IRequestValidator<>), typeof(FluentRequestValidator<>))
    .AddKinHubIdentitySqlInfrastructure(o => o.ConnectionString = builder.Configuration.GetConnectionString("KinHub")!)
    .AddKinHubJwtInfrastructure(o =>
    {
        o.Secret = builder.Configuration["Jwt:Secret"]
            ?? "CHANGE-ME-use-a-long-random-secret-at-least-32-chars!";
        o.Issuer = builder.Configuration["Jwt:Issuer"]
            ?? "kinhub";
    })
    .AddKinHubBusiness();

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

app.Run();
