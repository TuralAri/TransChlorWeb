using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TransChlorApi.Services;

var builder = WebApplication.CreateBuilder(args);

var webAddress = "http://web/api/"; //Replace "web" by localhost:8000 if you run the web app locally (without Docker)

builder.Services.AddHttpClient<RandomComputingStrategy>(client =>
{
    client.BaseAddress = new Uri(webAddress);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

builder.Services.AddHttpClient<RealComputingStrategy>(client =>
{
    client.BaseAddress = new Uri(webAddress);
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