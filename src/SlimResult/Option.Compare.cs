using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace FarzanHajian.SlimResult;

public partial struct Option<TValue>
{
    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly bool Equals(Option<TValue> other)
    {
        return (isSome, other.isSome) switch
        {
            (true, true) => value!.Equals(other.value),
            (false, false) => true,
            _ => false
        };
    }

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override readonly bool Equals([NotNullWhen(true)] object? obj)
    {
        return (obj is Option<TValue> opt) && Equals(opt);
    }

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override readonly int GetHashCode()
    {
        return isSome ? value!.GetHashCode() : 0;
    }

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(Option<TValue> obj1, Option<TValue> obj2)
    {
        return obj1.Equals(obj2);
    }

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(Option<TValue> obj1, Option<TValue> obj2)
    {
        return !obj1.Equals(obj2);
    }

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly int CompareTo(Option<TValue> other)
    {
        return (isSome, other.isSome) switch
        {
            (true, true) => CompareValues(value!),
            (false, false) => 0,
            (false, true) => -1,
            (true, false) => 1
        };

        int CompareValues(TValue value)     // "value" is passed instead of being captured due to CS1673
        {
            if (value is IComparable<TValue> v) return v.CompareTo(other.value);
            if (value is IComparable v2) return v2.CompareTo(other.value);
            throw new ArgumentException("TValue must impelement either IComparable or IComparable<T>.");
        }
    }

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly int CompareTo(object? obj)
    {
        return (obj is Option<TValue> opt)
            ? CompareTo(opt)
            : throw new ArgumentException("obj is not the same type as this instance.");
    }

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator <(Option<TValue> obj1, Option<TValue> obj2)
    {
        // No value is less than None and None is not less than any other value
        if (!obj1.isSome || !obj2.isSome) return false;

        return obj1.CompareTo(obj2) == -1;
    }

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator >(Option<TValue> obj1, Option<TValue> obj2)
    {
        // No value is less than None and None is not less than any other value
        if (!obj1.isSome || !obj2.isSome) return false;

        return obj1.CompareTo(obj2) == 1;
    }

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator <=(Option<TValue> obj1, Option<TValue> obj2)
    {
        // The comparison is always false between None and any other value and vice versa
        if (!obj1.isSome ^ !obj2.isSome) return false;

        return obj1.CompareTo(obj2) != 1;
    }

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator >=(Option<TValue> obj1, Option<TValue> obj2)
    {
        // The comparison is always false between None and any other value and vice versa
        if (!obj1.isSome ^ !obj2.isSome) return false;

        return obj1.CompareTo(obj2) != -1;
    }
}