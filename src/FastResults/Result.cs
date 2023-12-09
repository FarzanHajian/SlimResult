namespace FarzanHajian.FastResults;

/// <summary>
/// Represents an operation result with statuses of either Success or Failure.
/// The result holds a value if it is successfull or holds an error in case of failure,
/// If you operation does not produce any returning value, use <see cref="Result"/> instead.
/// </summary>
/// <typeparam name="TValue">Type of the value that can be hold be the result instance</typeparam>
public readonly struct Result<TValue>
{
    private readonly bool isSuccess;
    private readonly Option<TValue> value;
    private readonly Option<Error> error;

    /// <summary>
    /// Returns true if the result is in the Success status.
    /// </summary>
    public readonly bool IsSuccess => isSuccess;

    /// <summary>
    /// Returns true is the result is in the Failure status.
    /// </summary>
    public readonly bool IsFailure => !isSuccess;

    /// <summary>
    /// Return the value held by the result if it is successful otherwise, throws <see cref="ArgumentNullException"></see> is thrown.
    /// </summary>
    public readonly TValue Value => value.Value;

    /// <summary>
    /// Return the error held by the result if it is failed otherwise, throws <see cref="ArgumentNullException"></see> is thrown.
    /// </summary>
    public readonly Error Error => error.Value;

    /// <summary>
    /// The default constructor cannot be used. It throws <see cref="InvalidOperationException"/>.
    /// </summary>
    /// <exception cref="InvalidOperationException"></exception>
    public Result()
    {
        // There is no way to remove the default constructor.
        throw new InvalidOperationException("Using the default contructor is not valid.");
    }

    /// <summary>
    /// Creates a successful result that holds the given value.
    /// </summary>
    /// <param name="value">The value to be held by the result</param>
    public Result(TValue value)
    {
        isSuccess = true;
        this.value = value;
        error = Option<Error>.None();
    }

    /// <summary>
    /// Create a failed result using an <see cref="Exception"/>.
    /// </summary>
    /// <param name="exception">The exception to be held by the result</param>
    public Result(Exception exception)
    {
        isSuccess = false;
        value = default;
        error = Option<Error>.Some((Error)exception);
    }

    /// <summary>
    /// Create a failed result using an <see cref="FastResults.Error"/>.
    /// </summary>
    /// <param name="error">The error to be held by the result</param>
    public Result(Error error)
    {
        isSuccess = false;
        value = default;
        this.error = Option<Error>.Some(error);
    }

    /// <summary>
    /// Returns the value help by the result or a default one if the result is failed.
    /// </summary>
    /// <param name="default">The default value to be returned if the result is failed</param>
    /// <returns></returns>
    public TValue ValueOrDefault(TValue @default) => isSuccess ? value.Value : @default;

    /// <summary>
    /// Invokes one of the provided callable objects based on whether the result is successful or not.
    /// </summary>
    /// <param name="succ">The callable object to be invoked when the result is successful</param>
    /// <param name="fail">The callable object to be invoked when the result is failed</param>
    public void Match(Action<TValue> succ, Action<Error> fail)
    {
        if (isSuccess)
            succ(value.Value);
        else
            fail(error.Value);
    }

    /// <summary>
    /// Invokes one of the provided callable objects based on whether the result is successful or not.
    /// </summary>
    /// <param name="succ">The callable object to be invoked when the result is successful</param>
    /// <param name="fail">The callable object to be invoked when the result is failed</param>
    public Task Match(Func<TValue, Task> succ, Func<Error, Task> fail)
    {
        return isSuccess ? succ(value.Value) : fail(error.Value);
    }

    /// <summary>
    /// Returns a value by invoking one of the provided functions based on whether the result is successfull or not.
    /// </summary>
    /// <typeparam name="TRet">Type of the resturning value</typeparam>
    /// <param name="succ">The function to be invoked when the result is successful</param>
    /// <param name="fail">The function to be invoked when the result is failed</param>
    public TRet MatchReturn<TRet>(Func<TValue, TRet> succ, Func<Error, TRet> fail)
    {
        return isSuccess ? succ(value.Value) : fail(error.Value);
    }

    /// <summary>
    /// Returns a value by invoking one of the provided functions based on whether the result is successfull or not.
    /// </summary>
    /// <typeparam name="TRet">Type of the resturning value</typeparam>
    /// <param name="succ">The function to be invoked when the result is successful</param>
    /// <param name="fail">The function to be invoked when the result is failed</param>
    public Task<TRet> MatchReturn<TRet>(Func<TValue, Task<TRet>> succ, Func<Error, Task<TRet>> fail)
    {
        return isSuccess ? succ(value.Value) : fail(error.Value);
    }

    /// <summary>
    /// Returns the returning value of the provided custom handler if the result is successful, otherwise, returns the result instance itself.
    /// </summary>
    /// <param name="succ">The custom handler</param>
    /// <returns>A <see cref="Result{TValue}"/> instnace</returns>
    public Result<TValue> IfSuccess(Func<TValue, Result<TValue>> succ)
    {
        return isSuccess ? succ(Value) : this;
    }

    /// <summary>
    /// Returns the returning value of the provided custom handler if the result is successful, otherwise, returns the result instance itself.
    /// </summary>
    /// <param name="succ">The custom handler</param>
    /// <returns>A <see cref="Result{TValue}"/> instnace</returns>
    public Task<Result<TValue>> IfSuccess(Func<TValue, Task<Result<TValue>>> succ)
    {
        return isSuccess ? succ(Value) : Task.FromResult(this);
    }

    /// <summary>
    /// Returns the returning value of the provided custom handler if the result is failed, otherwise, returns the result instance itself.
    /// </summary>
    /// <param name="fail">The custom handler</param>
    /// <returns>A <see cref="Result{TValue}"/> instnace</returns>
    public Result<TValue> IfFailure(Func<Error, Result<TValue>> fail)
    {
        return isSuccess ? this : fail(Error);
    }

    /// <summary>
    /// Returns the returning value of the provided custom handler if the result is failed, otherwise, returns the result instance itself.
    /// </summary>
    /// <param name="fail">The custom handler</param>
    /// <returns>A <see cref="Result{TValue}"/> instnace</returns>
    public Task<Result<TValue>> IfFailure(Func<Error, Task<Result<TValue>>> fail)
    {
        return isSuccess ? Task.FromResult(this) : fail(Error);
    }

    /// <summary>
    /// Creates a successful result that holds the given value.
    /// </summary>
    /// <param name="value">The value to be held by the result</param>
    public static Result<TValue> Success(TValue value) => new(value);

    /// <summary>
    /// Create a failed result using an <see cref="FastResults.Error"/> or <see cref="Exception"/>.
    /// </summary>
    /// <param name="error">The error (or exception) to be held by the result</param>
    public static Result<TValue> Failure(Error error) => new(error);

    /// <summary>
    /// The implicit cast operator from a value to a successful result.
    /// </summary>
    /// <param name="value">The value to be held by the instance.</param>
    public static implicit operator Result<TValue>(TValue value) => new(value);

    /// <summary>
    /// The implicit cast from <see cref="Result{TValue}"/> to <see cref="Result"/>
    /// </summary>
    /// <param name="source">The source result</param>
    public static implicit operator Result(Result<TValue> source)
    {
        return source.isSuccess ? new Result() : new Result(source.error.Value);
    }
}

/// <summary>
/// Represents an operation result with statuses of either Success or Failure.
/// The result holds an error in case of failure,
/// If you operation produces any returning value, use <see cref="Result{TValue}"/> instead.
/// </summary>
public readonly struct Result
{
    private readonly bool isSuccess;
    private readonly Option<Error> error;

    /// <summary>
    /// Returns true if the result is in the Success status.
    /// </summary>
    public readonly bool IsSuccess => isSuccess;

    /// <summary>
    /// Returns true is the result is in the Failure status.
    /// </summary>
    public readonly bool IsFailure => !isSuccess;

    /// <summary>
    /// Return the error held by the result if it is failed otherwise, throws <see cref="ArgumentNullException"></see> is thrown.
    /// </summary>
    public readonly Error Error => error.Value;

    /// <summary>
    /// Creates a successful result.
    /// </summary>
    public Result()
    {
        isSuccess = true;
        error = Option<Error>.None();
    }

    /// <summary>
    /// Create a failed result using an <see cref="Exception"/>.
    /// </summary>
    /// <param name="exception">The exception to be held by the result</param>
    public Result(Exception exception)
    {
        isSuccess = false;
        error = Option<Error>.Some(exception);
    }

    /// <summary>
    /// Create a failed result using an <see cref="FastResults.Error"/>.
    /// </summary>
    /// <param name="error">The error to be held by the result</param>
    public Result(Error error)
    {
        isSuccess = false;
        this.error = Option<Error>.Some(error);
    }

    /// <summary>
    /// Invokes one of the provided callable objects based on whether the result is successful or not.
    /// </summary>
    /// <param name="succ">The callable object to be invoked when the result is successful</param>
    /// <param name="fail">The callable object to be invoked when the result is failed</param>
    public void Match(Action succ, Action<Error> fail)
    {
        if (isSuccess)
            succ();
        else
            fail(error.Value);
    }

    /// <summary>
    /// Invokes one of the provided callable objects based on whether the result is successful or not.
    /// </summary>
    /// <param name="succ">The callable object to be invoked when the result is successful</param>
    /// <param name="fail">The callable object to be invoked when the result is failed</param>
    public Task Match(Func<Task> succ, Func<Error, Task> fail)
    {
        return isSuccess ? succ() : fail(error.Value);
    }

    /// <summary>
    /// Returns a value by invoking one of the provided functions based on whether the result is successfull or not.
    /// </summary>
    /// <typeparam name="TRet">Type of the resturning value</typeparam>
    /// <param name="succ">The function to be invoked when the result is successful</param>
    /// <param name="fail">The function to be invoked when the result is failed</param>
    public TRet MatchReturn<TRet>(Func<TRet> succ, Func<Error, TRet> fail)
    {
        return isSuccess ? succ() : fail(error.Value);
    }

    /// <summary>
    /// Returns a value by invoking one of the provided functions based on whether the result is successfull or not.
    /// </summary>
    /// <typeparam name="TRet">Type of the resturning value</typeparam>
    /// <param name="succ">The function to be invoked when the result is successful</param>
    /// <param name="fail">The function to be invoked when the result is failed</param>
    public Task<TRet> MatchReturn<TRet>(Func<Task<TRet>> succ, Func<Error, Task<TRet>> fail)
    {
        return isSuccess ? succ() : fail(error.Value);
    }

    /// <summary>
    /// Returns the returning value of the provided custom handler if the result is successful, otherwise, returns the result instance itself.
    /// </summary>
    /// <param name="succ">The custom handler</param>
    /// <returns>A <see cref="Result"/> instnace</returns>
    public Result IfSuccess(Func<Result> succ)
    {
        return isSuccess ? succ() : this;
    }

    /// <summary>
    /// Returns the returning value of the provided custom handler if the result is successful, otherwise, returns the result instance itself.
    /// </summary>
    /// <param name="succ">The custom handler</param>
    /// <returns>A <see cref="Result"/> instnace</returns>
    public Task<Result> IfSuccess(Func<Task<Result>> succ)
    {
        return isSuccess ? succ() : Task.FromResult(this);
    }

    /// <summary>
    /// Returns the returning value of the provided custom handler if the result is failed, otherwise, returns the result instance itself.
    /// </summary>
    /// <param name="fail">The custom handler</param>
    /// <returns>A <see cref="Result"/> instnace</returns>
    public Result IfFailure(Func<Error, Result> fail)
    {
        return isSuccess ? this : fail(Error);
    }

    /// <summary>
    /// Returns the returning value of the provided custom handler if the result is failed, otherwise, returns the result instance itself.
    /// </summary>
    /// <param name="fail">The custom handler</param>
    /// <returns>A <see cref="Result"/> instnace</returns>
    public Task<Result> IfFailure(Func<Error, Task<Result>> fail)
    {
        return isSuccess ? Task.FromResult(this) : fail(Error);
    }

    /// <summary>
    /// Creates a successful result.
    /// </summary>
    /// <returns></returns>
    public static Result Success() => new();

    /// <summary>
    /// Create a failed result using an <see cref="FastResults.Error"/> or <see cref="Exception"/>.
    /// </summary>
    /// <param name="error">The error (or exception) to be held by the result</param>
    public static Result Failure(Error error) => new(error);

    /// <summary>
    /// Invokes a callable object in a try/catch block and returns a appropriate result instance.
    /// </summary>
    /// <param name="action">The callable object to be invoked</param>
    /// <param name="handler">An oprtional custom expcetion handler which should return a failed result</param>
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
    /// Invokes a callable object in a try/catch block and returns a appropriate result instance.
    /// </summary>
    /// <param name="action">The callable object to be invoked</param>
    /// <param name="handler">An oprtional custom expcetion handler which should return a failed result</param>
    /// <returns>A successful result if the callable object executes successully of a failed result if it throws an exception.</returns>
    public static Task<Result> Try(Func<Task> action, Func<Exception, Result>? handler = null)
    {
        return action().ContinueWith(Continue);

        Result Continue(Task task)
        {
            if (task.IsCompletedSuccessfully) return new Result();
            return handler?.Invoke(task.Exception!.InnerException!) ?? new Result(task.Exception!.InnerException!);
        }
    }
}