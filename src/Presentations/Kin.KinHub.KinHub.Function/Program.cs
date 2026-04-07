using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

var dataDirectory = builder.Configuration["JsonDataDirectory"]
    ?? Path.Combine(AppContext.BaseDirectory, "Data");

builder.Services
    .AddJsonInfrastructure(o => o.DataDirectory = dataDirectory)
    .AddKinHubBusiness(_ => { });

builder.Build().Run();
