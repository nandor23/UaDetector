namespace UaDetector;

public interface IUaDetectorCache
{
    bool TryGet<T>(string key, out T? value);
    bool Set<T>(string key, T? value);
}
