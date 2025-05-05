using System.Diagnostics.CodeAnalysis;

namespace UaDetector;

public interface IUaDetectorCache
{
    bool TryGet<T>(string key, [NotNullWhen(true)] out T? value);
    bool Set<T>(string key, T value);
}
