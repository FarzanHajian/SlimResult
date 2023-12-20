namespace FarzanHajian.FastResults;

/// <summary>
/// Represents an occured error.
/// </summary>
public readonly struct Error
{
    /// <summary>
    /// Returns the error message.
    /// </summary>
    public readonly string Message { get; init; }

    /// <summary>
    /// Returns the exception (if any) based on which this error is created.
    /// </summary>
    public readonly Option<Exception> Exception { get; init; }

    /// <summary>
    /// Creates an error using an error message.
    /// </summary>
    /// <param name="message">The actual error message.</param>
    public Error(string message)
    {
        Message = message;
        Exception = new Option<Exception>();
    }

    /// <summary>
    /// Creates an error using an exception.
    /// </summary>
    /// <param name="exception">The exception for which an error should be created.</param>
    public Error(Exception exception)
    {
        Message = exception.Message;
        Exception = new Option<Exception>(exception);
    }

    /// <summary>
    /// The implicit cast operator to from a string.
    /// </summary>
    /// <param name="message">The error message.</param>
    public static implicit operator Error(string message) => new Error(message);

    /// <summary>
    /// The implicit cast operator to from an exception.
    /// </summary>
    /// <param name="exception">The exception.</param>
    public static implicit operator Error(Exception exception) => new Error(exception);
}