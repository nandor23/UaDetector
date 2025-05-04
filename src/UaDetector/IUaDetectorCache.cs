using System.Diagnostics.CodeAnalysis;

namespace UaDetector;

public interface IUaDetectorCache
{
    bool TryGet<T>(string key, [NotNullWhen(true)] out T? value);
    void Set<T>(string key, T value);
}
