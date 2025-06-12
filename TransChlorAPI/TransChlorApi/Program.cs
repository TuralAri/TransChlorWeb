using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TransChlorApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient<RandomComputingStrategy>(client =>
{
    client.BaseAddress = new Uri("http://web/api/");
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

builder.Services.AddHttpClient<RealComputingStrategy>(client =>
{
    client.BaseAddress = new Uri("http://web/api/");
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

builder.Services.AddSingleton<ComputationTaskManager>();
builder.Services.AddSingleton<ComputingStrategyFactory>();

builder.Services.AddSingleton<ComputationTaskManager>();

builder.Services.AddHttpClient();

builder.Services.AddControllers();

var app = builder.Build();

app.MapControllers();

app.Run();