// Services/ComputingStrategyFactory.cs

using System;
using Microsoft.Extensions.DependencyInjection;
using TransChlorApi.Services;

namespace TransChlorApi.Services;

public class ComputingStrategyFactory
{
    private readonly IServiceProvider _provider;

    // Le constructeur reçoit le provider de services pour pouvoir instancier les stratégies
    public ComputingStrategyFactory(IServiceProvider provider)
    {
        _provider = provider;
    }
    
    public IComputingStrategy GetStrategy(string mode, int computationId)
    {
        if (string.IsNullOrWhiteSpace(mode))
            throw new ArgumentException("Le mode ne peut pas être vide.", nameof(mode));

        switch (mode.Trim().ToLowerInvariant())
        {
            case "random":
                return _provider.GetRequiredService<RandomComputingStrategy>();
            
            case "1d":
                return _provider.GetRequiredService<RealComputingStrategy>();
            
            default:
                throw new ArgumentException($"Mode inconnu : '{mode}'", nameof(mode));
        }
    }
}