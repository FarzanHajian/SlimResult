using System.Runtime.CompilerServices;

namespace FarzanHajian.SlimResult;

/// <summary>
/// A global class that is loaded helper methods to ease commoly-used tasks.
/// </summary>
public static class SlimResultHelper
{
    /// <summary>
    /// Creates an optional variable that hold a value or throws an <see cref="ArgumentNullException"></see> if the value is null.
    /// </summary>
    /// <typeparam name="TValue">Type of the optional variable.</typeparam>
    /// <param name="value">The value to be passed to the instance.</param>
    /// <returns>Returns an instance of <see cref="Option{TValue}"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Option<TValue> Some<TValue>(TValue value) => new(value);

    /// <summary>
    /// Creates an optional variable which is empty.
    /// </summary>
    /// <typeparam name="TValue">Type of the optional variable.</typeparam>
    /// <returns>Returns an instance of <see cref="Option{TValue}"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Option<TValue> None<TValue>() => new();

    /// <summary>
    /// Creates a successful result that holds the given value.
    /// </summary>
    /// <param name="value">The value to be held by the result.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Result<TValue> Success<TValue>(TValue value) => new(value);

    /// <summary>
    /// Create a failed result using an <see cref="SlimResult.Error"/> or <see cref="Exception"/>.
    /// </summary>
    /// <param name="error">The error (or exception) to be held by the result.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Result<TValue> Failure<TValue>(Error error) => new(error);

    /// <summary>
    /// Creates a successful result.
    /// </summary>
    /// <returns>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Result Success() => new();

    /// <summary>
    /// Create a failed result using an <see cref="SlimResult.Error"/> or <see cref="Exception"/>.
    /// </summary>
    /// <param name="error">The error (or exception) to be held by the result.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Result Failure(Error error) => new(error);

    /// <summary>
    /// Invokes a callable object in a try/catch block and returns an appropriate result instance.
    /// </summary>
    /// <param name="action">The callable object to be invoked.</param>
    /// <param name="handler">An oprtional custom expcetion handler which should return a failed result.</param>
    /// <returns>A successful result if the callable object executes successully of a failed result if it throws an exception.</returns>
    public static Result Try(Action action, Func<Exception, Result>? handler = null)
    {
        try
        {
            action();
            return new Result();
        }
        catch (Exception ex)
        {
            return handler?.Invoke(ex) ?? new Result(ex);
        }
    }

    /// <summary>
    /// Invokes a callable object in a try/catch block and returns an appropriate result instance.
    /// </summary>
    /// <param name="action">The callable object to be invoked.</param>
    /// <param name="handler">An oprtional custom expcetion handler which should return a failed result.</param>
    /// <returns>A successful result if the callable object executes successully of a failed result if it throws an exception.</returns>
    public static Task<Result> Try(Func<Task> action, Func<Exception, Result>? handler = null)
    {
        return action().ContinueWith(new Func<Task, Result>(Continue));

        Result Continue(Task task)
        {
            if (task.IsCompletedSuccessfully) return new Result();
            return handler?.Invoke(task.Exception!.InnerException!) ?? new Result(task.Exception!.InnerException!);
        }
    }
}