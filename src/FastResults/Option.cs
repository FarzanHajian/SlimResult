using System.Runtime.CompilerServices;

namespace FarzanHajian.FastResults;

/// <summary>
/// Represents a variable that either holds a value or is empty.
/// </summary>
/// <typeparam name="TValue">Type of the value this instance can hold</typeparam>
public struct Option<TValue>// : IEquatable<Option<TValue>>
{
    private bool isSome;
    private TValue? value;

    /// <summary>
    /// Returns true if this instance is currently holding a value.
    /// </summary>
    public readonly bool IsSome => isSome;

    /// <summary>
    /// Returns true if this instance is currently empty.
    /// </summary>
    public readonly bool IsNone => !isSome;

    /// <summary>
    /// Returns the current value held by the instance or throws <see cref="ArgumentNullException"></see> if it is empty.
    /// </summary>
    public readonly TValue Value => isSome ? value! : throw new InvalidOperationException($"The current {nameof(Option<TValue>)} instance is empty.");

    /// <summary>
    /// Creates an empty variable.
    /// </summary>
    public Option()
    {
        isSome = false;
        value = default;
    }

    /// <summary>
    /// Creates a variable that hold a value or throws an <see cref="ArgumentNullException"></see> if the value is null.
    /// </summary>
    /// <param name="value">.</param>
    public Option(TValue value)
    {
        if (value is null) throw new ArgumentNullException(nameof(value));
        isSome = true;
        this.value = value;
    }

    /// <summary>
    /// Makes the instance hold a new value or throws an <see cref="ArgumentNullException"></see> if the value is null.
    /// </summary>
    /// <param name="value">.</param>
    public void Set(TValue value)
    {
        if (value is null) throw new ArgumentNullException(nameof(value));
        this.value = value;
        isSome = true;
    }

    /// <summary>
    /// Makes the instance empty by removing its value (if any).
    /// </summary>
    public void Unset()
    {
        value = default;
        isSome = false;
    }

    /// <summary>
    /// Returns the value held by the instnace or a default one if the instnace is empty.
    /// </summary>
    /// <param name="default">The default value to be returned if the instance is empty.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TValue ValueOrDefault(TValue @default) => IsSome ? value! : @default;

    /// <summary>
    /// Invokes one of the provided callable objects based on whether the instance holds a value or not.
    /// </summary>
    /// <param name="some">The callable object to be invoked when the instance holds a value.</param>
    /// <param name="none">The callable object to be invoked when the instance is empty.</param>
    public readonly void Match(Action<TValue> some, Action none)
    {
        if (isSome)
            some(value!);
        else
            none();
    }

    /// <summary>
    /// Returns a value by invoking one of the provided functions based on whether the instance holds a value or not.
    /// </summary>
    /// <typeparam name="TRet">Type of the resturning value</typeparam>
    /// <param name="some">The function to be invoked when the instance holds a value.</param>
    /// <param name="none">The function object to be invoked when the instance is empty.</param>
    public readonly TRet Match<TRet>(Func<TValue, TRet> some, Func<TRet> none)
    {
        return isSome ? some(value!) : none();
    }

    /// <summary>
    /// Creates a variable that hold a value or throws an <see cref="ArgumentNullException"></see> if the value is null.
    /// </summary>
    /// <param name="value">.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Option<TValue> Some(TValue value) => new(value);

    /// <summary>
    /// Creates an empty variable.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Option<TValue> None() => new();

    ///// <summary>
    ///// Indicates whether the current object is equal to another object of the same type.
    ///// </summary>
    ///// <param name="other">An object to compare with this object.</param>
    ///// <returns>true if the current object is equal to the other parameter; otherwise, false.</returns>
    //public readonly bool Equals(Option<TValue> other)
    //{
    //    if (isSome && other.isSome) return value!.Equals(other.value!);
    //    return (isSome == other.isSome);
    //}

    //public readonly bool Equals(TValue other)
    //{
    //    if (!isSome) return false;
    //    return value!.Equals(other);
    //}

    //public readonly bool Equals<TOther>(Option<TOther> other)
    //{
    //    if (isSome && other.isSome) return value!.Equals(other.value!);
    //    return (isSome == other.isSome);
    //}

    ///// <summary>
    ///// Indicates whether the current object is equal to another object of the same type.
    ///// </summary>
    ///// <param name="other">An object to compare with this object.</param>
    ///// <returns>true if the current object is equal to the other parameter; otherwise, false.</returns>
    //public override readonly bool Equals(object? other)
    //{
    //    // other == null
    //    // other == Option<TValue>
    //    // other == Option<TValue2>
    //    // other == scalar value (of type TValue / not) if both have value

    //    if (other is null) return false;
    //    if (other is Option<TValue> opt) return Equals(opt);
    //    //

    //    ////////////// ORIGINAL
    //    //if (other is not Option<TValue> opt) return false;
    //    //return Equals(opt);
    //}

    ///// <summary>
    ///// Returns the hash code for this instnace.
    ///// </summary>
    ///// <returns>A 32-bit signed integer that is the hash code for this instnace.</returns>
    //public override readonly int GetHashCode() => isSome ? value!.GetHashCode() : 0;

    /// <summary>
    /// Retuens a string that represents the current object.
    /// </summary>
    /// <returns>A string that represents the current object.</returns>
    public override readonly string? ToString() => isSome ? value!.ToString() : "";

    /// <summary>
    /// Performs the equality check between to objects.
    /// </summary>
    /// <param name="left">The left object.</param>
    /// <param name="right">The right object</param>
    /// <returns>true if the left object is equal to the right object; otherwise, false.</returns>
    //public static bool operator ==(Option<TValue> left, Option<TValue> right) => left.Equals(right);

    ///// <summary>
    ///// Performs the inequality check between to objects.
    ///// </summary>
    ///// <param name="left">The left object.</param>
    ///// <param name="right">The right object</param>
    ///// <returns>true if the first object is not equal to the second object; otherwise, false.</returns>
    //public static bool operator !=(Option<TValue> left, Option<TValue> right) => !left.Equals(right);

    /// <summary>
    /// The implicit cast operator from a value.
    /// </summary>
    /// <param name="value">The value to be held by the instance.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Option<TValue>(TValue value) => new(value);

    /// <summary>
    /// The implicit cast operator to a value. If the option instance is empty, <see cref="InvalidOperationException"/> is thrown.
    /// </summary>
    /// <param name="option">The source option instance.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator TValue(Option<TValue> option) => option.Value;
}

/// <summary>
/// A class that contains <see cref="Option{TValue}"/> helper methods.
/// </summary>
public static class OptionHelper
{
    /// <summary>
    /// A helper method that wraps <see cref="Option{TValue}.Some(TValue)"/>.
    /// </summary>
    /// <typeparam name="TValue">Type of the value.</typeparam>
    /// <param name="value">The value to be passed to <see cref="Option{TValue}.Some(TValue)"/>.</param>
    /// <returns>Returns an instance of <see cref="Option{TValue}.Some(TValue)"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Option<TValue> Some<TValue>(TValue value) => new(value);

    /// <summary>
    /// A helper method that wraps <see cref="Option{TValue}.None"/>.
    /// </summary>
    /// <typeparam name="TValue">Type of the value.</typeparam>
    /// <returns>Returns an instance of <see cref="Option{TValue}.Some(TValue)"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Option<TValue> None<TValue>() => new();
}