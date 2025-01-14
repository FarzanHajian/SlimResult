using System.Runtime.CompilerServices;

namespace FarzanHajian.SlimResult;

/// <summary>
/// Represents a variable that either holds a value or is empty.
/// </summary>
/// <typeparam name="TValue">Type of the value this instance can hold</typeparam>
public partial struct Option<TValue> : IEquatable<Option<TValue>>, IComparable<Option<TValue>>, IComparable
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
    public readonly TValue Value => isSome ? value! : throw new InvalidOperationException($"The current Option instance is empty.");

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
    public readonly TValue ValueOrDefault(TValue @default) => isSome ? value! : @default;

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
    /// Retuens a string that represents the current object.
    /// </summary>
    /// <returns>A string that represents the current object.</returns>
    public override readonly string? ToString() => isSome ? value!.ToString() : "";

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
    public static implicit operator TValue(Option<TValue> option)
    {
        return option.isSome
            ? option.Value
            : throw new InvalidOperationException($"An empty Option of {typeof(TValue).Name} cannot be converted to a value of type {typeof(TValue).Name}.");
    }
}