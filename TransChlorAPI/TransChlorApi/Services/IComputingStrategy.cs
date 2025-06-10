using System.Text.Json;
using TransChlorApi.Models;

namespace TransChlorApi.Services;

public interface IComputingStrategy
{
    Task ExecuteAsync(int computationId, CancellationToken cancellationToken, string outfile, string options);
}