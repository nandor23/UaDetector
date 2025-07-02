using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace UaDetector.SourceGenerator.Collections;

/// <summary>
/// A wrapper for IReadOnlyDictionary that provides value equality support for the wrapped dictionary.
/// </summary>
[ExcludeFromCodeCoverage]
internal readonly struct EquatableReadOnlyDictionary<TKey, TValue>(
    IReadOnlyDictionary<TKey, TValue>? dictionary
) : IEquatable<EquatableReadOnlyDictionary<TKey, TValue>>, IReadOnlyDictionary<TKey, TValue>
    where TKey : notnull
{
    private IReadOnlyDictionary<TKey, TValue> Dictionary =>
        dictionary ?? new Dictionary<TKey, TValue>();

    public bool Equals(EquatableReadOnlyDictionary<TKey, TValue> other)
    {
        if (Dictionary.Count != other.Dictionary.Count)
            return false;

        foreach (var kvp in Dictionary)
        {
            if (!other.Dictionary.TryGetValue(kvp.Key, out var otherValue))
            {
                return false;
            }

            if (!EqualityComparer<TValue>.Default.Equals(kvp.Value, otherValue))
            {
                return false;
            }
        }

        return true;
    }

    public override bool Equals(object? obj) =>
        obj is EquatableReadOnlyDictionary<TKey, TValue> other && Equals(other);

    public override int GetHashCode()
    {
        var hash = new HashCode();

        foreach (var kvp in Dictionary.OrderBy(x => x.Key.GetHashCode()))
        {
            hash.Add(kvp.Key);
            hash.Add(kvp.Value);
        }

        return hash.ToHashCode();
    }

    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => Dictionary.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => Dictionary.GetEnumerator();

    public int Count => Dictionary.Count;

    public bool ContainsKey(TKey key) => Dictionary.ContainsKey(key);

    public bool TryGetValue(TKey key, [UnscopedRef] out TValue value) =>
        Dictionary.TryGetValue(key, out value);

    public TValue this[TKey key] => Dictionary[key];

    public IEnumerable<TKey> Keys => Dictionary.Keys;

    public IEnumerable<TValue> Values => Dictionary.Values;

    public static bool operator ==(
        EquatableReadOnlyDictionary<TKey, TValue> left,
        EquatableReadOnlyDictionary<TKey, TValue> right
    ) => left.Equals(right);

    public static bool operator !=(
        EquatableReadOnlyDictionary<TKey, TValue> left,
        EquatableReadOnlyDictionary<TKey, TValue> right
    ) => !left.Equals(right);
}
