using System.Net.Http.Json;
using System.Runtime.InteropServices.JavaScript;
using System.Text.Json;
using TransChlorApi.Models;

namespace TransChlorApi.Services;

public class RealComputingStrategy : IComputingStrategy
{
    private readonly HttpClient _httpClient;
    
    public RealComputingStrategy(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    
    public async Task ExecuteAsync(int computationId, CancellationToken cancellationToken, string outfile, string options)
    {
        if (outfile == "")
        {
            throw new ArgumentNullException(nameof(outfile));
        }
        
        Console.WriteLine($"Computing strategy: {computationId}" + "\n");
        double[,] Creadtherm = new double[2, 2];
        double[,] Creadhydr = new double[2, 2];
        double[,] Creadion = new double[2, 2];
        short Nbre1 = 0;
        short Nbre2 = 0;
        short Nbre3 = 0;
        
        Compute1D C1D = new Compute1D();
        C1D.computationId = computationId;
        C1D.httpClient = _httpClient;
        
        if (options != "")
        {
            var jsonOptions = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(options);
            int nLayers = jsonOptions["nlayers"].GetInt32();
            Console.WriteLine("nlayers" + "\n");
            var epLayers = jsonOptions["epLayers"]
                .EnumerateArray()
                .Select(x => x.GetDouble())
                .ToArray();
            Console.WriteLine("epLayers" + "\n");
            C1D.nLayersExt = nLayers;
            C1D.epLayersExt = epLayers;
        }
        
        // string filePath = "/home/iutbgdin/RiderProjects/TransChlorApi/public/exports/input_calcul.txt";
        Console.WriteLine($"filepath\n");

        Console.WriteLine($"on commence lire fichier init\n");
        C1D.ReadFile(outfile,ref Nbre1, ref Nbre2, ref Nbre3, ref Creadtherm, ref Creadhydr, ref Creadion);

        Console.WriteLine($"Initial Conditions\n");
        C1D.InitialConditions(ref Nbre1, ref Nbre2, ref Nbre3, ref Creadtherm, ref Creadhydr, ref Creadion);

        Console.WriteLine($"On lance compute all\n");
        C1D.Compute_All(cancellationToken);
        
        await _httpClient.PostAsJsonAsync("computations-over", computationId);
    }
    
}