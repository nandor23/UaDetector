using System.Collections.Frozen;

namespace UADetector.Utils;

public class FrozenBiDictionary<TKey, TValue> where TKey : notnull where TValue : notnull
{
    private readonly FrozenDictionary<TKey, TValue> _forwardDictionary;
    private readonly FrozenDictionary<TValue, TKey> _reverseDictionary;


    public FrozenBiDictionary(Dictionary<TKey, TValue> dictionary)
    {
        _forwardDictionary = dictionary.ToFrozenDictionary();
        _reverseDictionary = dictionary.ToDictionary(e => e.Value, e => e.Key).ToFrozenDictionary();
    }

    public bool TryGetValue(TKey key, out TValue? value)
    {
        return _forwardDictionary.TryGetValue(key, out value);
    }

    public bool TryGetKey(TValue value, out TKey? key)
    {
        return _reverseDictionary.TryGetValue(value, out key);
    }
}
