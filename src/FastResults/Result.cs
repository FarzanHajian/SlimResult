namespace FarzanHajian.FastResults;

public readonly struct Result<TValue>
{
    private readonly bool isSuccess;
    private readonly Option<TValue> value;
    private readonly Option<Error> error;

    public readonly bool IsSuccess => isSuccess;
    public readonly bool IsFailure => !isSuccess;
    public readonly TValue Value => value.Value;
    public readonly Error Error => error.Value;

    public Result()
    {
        // There is no way to remove the default constructor.
        throw new InvalidOperationException("Using the default contructor is not valid.");
    }

    public Result(TValue value)
    {
        isSuccess = true;
        this.value = value;
        error = Option<Error>.None();
    }

    public Result(Exception exception)
    {
        isSuccess = false;
        value = default;
        error = Option<Error>.Some((Error)exception);
    }

    public Result(Error error)
    {
        isSuccess = false;
        value = default;
        this.error = Option<Error>.Some(error);
    }

    public TValue ValueOrDefault(TValue @default) => isSuccess ? value.Value : @default;

    public void Match(Action<TValue>? succ = null, Action<Error>? fail = null)
    {
        if (isSuccess)
            succ?.Invoke(value.Value);
        else
            fail?.Invoke(error.Value);
    }

    public TRet Match<TRet>(Func<TValue, TRet> succ, Func<Error, TRet> fail)
    {
        return isSuccess ? succ(value.Value) : fail(error.Value);
    }

    public Task MatchAsync(Func<TValue, Task>? succ = null, Func<Error, Task>? fail = null)
    {
        if (isSuccess)
            return (succ?.Invoke(value.Value)) ?? Task.CompletedTask;
        else
            return (fail?.Invoke(error.Value)) ?? Task.CompletedTask;
    }

    public Task<TRet> MatchAsync<TRet>(Func<TValue, Task<TRet>> succ, Func<Error, Task<TRet>> fail)
    {
        return isSuccess ? succ(value.Value) : fail(error.Value);
    }

    public static Result<TValue> Success(TValue value) => new(value);

    public static Result<TValue> Failure(Error error) => new(error);

    public static implicit operator Result<TValue>(TValue value) => new Result<TValue>(value);

    public static implicit operator Result(Result<TValue> source)
    {
        return source.isSuccess ? new Result() : new Result(source.error.Value);
    }
}

public readonly struct Result
{
    private readonly bool isSuccess;
    private readonly Option<Error> error;

    public readonly bool IsSuccess => isSuccess;
    public readonly bool IsFailure => !isSuccess;
    public readonly Error Error => error.Value;

    public Result()
    {
        isSuccess = true;
        error = Option<Error>.None();
    }

    public Result(Exception exception)
    {
        isSuccess = false;
        error = Option<Error>.Some(exception);
    }

    public Result(Error error)
    {
        isSuccess = false;
        this.error = Option<Error>.Some(error);
    }

    public void Match(Action? succ = null, Action<Error>? fail = null)
    {
        if (isSuccess)
            succ?.Invoke();
        else
            fail?.Invoke(error.Value);
    }

    public TRet Match<TRet>(Func<TRet> succ, Func<Error, TRet> fail)
    {
        return isSuccess ? succ() : fail(error.Value);
    }

    public Task MatchAsync(Func<Task>? succ = null, Func<Error, Task>? fail = null)
    {
        if (isSuccess)
            return (succ?.Invoke()) ?? Task.CompletedTask;
        else
            return (fail?.Invoke(error.Value)) ?? Task.CompletedTask;
    }

    public Task<TRet> MatchAsync<TRet>(Func<Task<TRet>> succ, Func<Error, Task<TRet>> fail)
    {
        return isSuccess ? succ() : fail(error.Value);
    }

    public static Result Success() => new();

    public static Result Failure(Error error) => new(error);

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

    public static Task<Result> TryAsync(Func<Task> action, Func<Exception, Result>? handler = null)
    {
        return action().ContinueWith(Continue);

        Result Continue(Task task)
        {
            if (task.IsCompletedSuccessfully) return new Result();
            return handler?.Invoke(task.Exception!.InnerException!) ?? new Result(task.Exception!.InnerException!);
        }
    }
}