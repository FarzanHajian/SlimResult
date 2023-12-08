namespace FarzanHajian.FastResults;

public readonly struct Error
{
    public readonly string Message { get; init; }

    public Error(string message) => Message = message;

    public Error(Exception exception) => Message = exception.Message;

    public static implicit operator Error(string message) => new Error(message);

    public static implicit operator Error(Exception exception) => new Error(exception);
}