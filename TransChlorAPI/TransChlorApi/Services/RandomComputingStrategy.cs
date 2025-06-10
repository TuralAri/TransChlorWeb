using System.Net.Http.Json;
using TransChlorApi.Models;

namespace TransChlorApi.Services;

public class RandomComputingStrategy : IComputingStrategy
{
    private readonly HttpClient _httpClient;
    private int steps = 365 + 1;
    private readonly TimeSpan _delay = TimeSpan.FromSeconds(2); //5 seconds before each steps to simulate a long computing

    private static readonly string[] _types = new[]
    {
        "temperature_potential",
        "moisture_potential",
        "moisture_content",
        "total_chloride",
        "free_chloride",
        "ph"
    };

    public RandomComputingStrategy(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task ExecuteAsync(int computationId, CancellationToken cancellationToken, string outfile, string options)
    {
        var depths = Enumerable.Range(0, 101).Select(i => (double)i).ToList();
        var rand   = new Random();

        for (int t = 0; t < steps; t++) //one step here would be one hour in expo files
        {
            //Check if cancellation was asked
            cancellationToken.ThrowIfCancellationRequested(); //Will throw an exception if cancellation requested
            // waiting 5 seconds before each steps
            if(t%30 == 0 || t%365 ==0)
                await Task.Delay(_delay);
            
            foreach (var type in _types)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var result = new ComputationResult
                {
                    Time   = t,
                    Values = depths.Select(_ => Math.Round(rand.NextDouble() * 100, 2)).ToList(),
                    Type   = type,
                    ComputationId = computationId,
                };
                if (t % 30 == 0)
                {
                    //Here data will be temporary saved for graphs
                    await _httpClient.PostAsJsonAsync("computations-actual-results", result);
                }
                else if(t % 365 == 0)
                {
                    //on this route, data will be saved in the DB
                    await _httpClient.PostAsJsonAsync("computations-results", result);
                }
            }
        }
        //send the info that computation is over
        await _httpClient.PostAsJsonAsync("computations-over", computationId);
    }
}