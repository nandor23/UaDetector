namespace UaDetector;

public sealed class UaDetectorOptionsBuilder
{
    public readonly UaDetectorOptions Options = new();

    public UaDetectorOptionsBuilder AddCache(IUaDetectorCache cache)
    {
        Options.Cache = cache;
        return this;
    }
}
