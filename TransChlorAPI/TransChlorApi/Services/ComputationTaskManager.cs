using System.Collections.Concurrent;

namespace TransChlorApi.Services;

public class ComputationTaskManager
{
    private readonly ConcurrentDictionary<int, CancellationTokenSource> _tasks = new();

    public bool TryStart(int computationId, CancellationTokenSource cts)
    {
        return _tasks.TryAdd(computationId, cts);
    }

    public bool TryStop(int computationId)
    {
        if (_tasks.TryRemove(computationId, out var cts))
        {
            cts.Cancel();
            return true;
        }
        return false;
    }
    
    public bool Exists(int computationId) => _tasks.ContainsKey(computationId);
}