using UaDetector.Abstractions;

namespace UaDetector.Tests.Helpers;

/// <summary>
/// An <see cref="IUaDetectorCache"/> that records every call so tests can assert
/// that a parser actually reads from and writes to the cache.
/// </summary>
public sealed class RecordingCache : IUaDetectorCache
{
    private readonly Dictionary<string, object?> _store = new();

    public List<string> GetKeys { get; } = [];
    public List<string> SetKeys { get; } = [];
    public int GetCount => GetKeys.Count;
    public int SetCount => SetKeys.Count;

    public bool TryGet<T>(string key, out T? value)
    {
        GetKeys.Add(key);

        if (_store.TryGetValue(key, out var stored))
        {
            value = (T?)stored;
            return true;
        }

        value = default;
        return false;
    }

    public bool Set<T>(string key, T? value)
    {
        SetKeys.Add(key);
        _store[key] = value;
        return true;
    }
}
