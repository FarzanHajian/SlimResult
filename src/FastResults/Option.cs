namespace FarzanHajian.FastResults;

public struct Option<TValue>
{
    private bool isSome;
    private TValue? value;

    public readonly bool IsSome => isSome;
    public readonly bool IsNone => !isSome;
    public readonly TValue Value => isSome ? value! : throw new InvalidOperationException($"The current {nameof(Option<TValue>)} instance is empty.");

    public Option()
    {
        isSome = false;
        value = default;
    }

    public Option(TValue value)
    {
        ArgumentNullException.ThrowIfNull(value, nameof(value));
        isSome = true;
        this.value = value;
    }

    public void Set(TValue value)
    {
        ArgumentNullException.ThrowIfNull(value, nameof(value));
        this.value = value;
        isSome = true;
    }

    public void Unset()
    {
        value = default;
        isSome = false;
    }

    public readonly void Match(Action<TValue>? some = null, Action? none = null)
    {
        if (isSome)
            some?.Invoke(value!);
        else
            none?.Invoke();
    }

    public readonly Task MatchAsync(Func<TValue, Task>? some = null, Func<Task>? none = null)
    {
        if (isSome)
            return (some?.Invoke(value!)) ?? Task.CompletedTask;
        else
            return (none?.Invoke()) ?? Task.CompletedTask;
    }

    public readonly T MatchValue<T>(Func<TValue, T> some, Func<T> none)
    {
        return isSome ? some(value!) : none();
    }

    public readonly Task<T> MatchValueAsync<T>(Func<TValue, Task<T>> some, Func<Task<T>> none)
    {
        return isSome ? some(value!) : none();
    }

    public static Option<TValue> Some(TValue value) => new(value);

    public static Option<TValue> None() => new();

    public static implicit operator Option<TValue>(TValue value) => new(value);
}