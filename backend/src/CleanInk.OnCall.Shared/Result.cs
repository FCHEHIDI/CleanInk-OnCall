namespace CleanInk.OnCall.Shared;

/// <summary>
/// Discriminated union representing either a successful value of type <typeparamref name="T"/>
/// or a domain <see cref="Error"/>.
/// </summary>
/// <typeparam name="T">The type of the success value.</typeparam>
public sealed class Result<T>
{
    private readonly T? _value;

    private Result(T value)
    {
        IsSuccess = true;
        _value = value;
        Error = Error.None;
    }

    private Result(Error error)
    {
        IsSuccess = false;
        _value = default;
        Error = error;
    }

    /// <summary>Gets a value indicating whether the result is a success.</summary>
    public bool IsSuccess { get; }

    /// <summary>Gets a value indicating whether the result is a failure.</summary>
    public bool IsFailure => !IsSuccess;

    /// <summary>Gets the success value. Throws <see cref="InvalidOperationException"/> on failure.</summary>
    public T Value =>
        IsSuccess ? _value! : throw new InvalidOperationException("Cannot access Value on a failed result.");

    /// <summary>Gets the error. Returns <see cref="Error.None"/> on success.</summary>
    public Error Error { get; }

    /// <summary>Creates a successful result.</summary>
    /// <param name="value">The success value.</param>
    /// <returns>A successful <see cref="Result{T}"/>.</returns>
    public static Result<T> Success(T value) => new(value);

    /// <summary>Creates a failed result.</summary>
    /// <param name="error">The error describing the failure.</param>
    /// <returns>A failed <see cref="Result{T}"/>.</returns>
    public static Result<T> Failure(Error error) => new(error);

    /// <summary>Implicit conversion from <typeparamref name="T"/> to a successful result.</summary>
    public static implicit operator Result<T>(T value) => Success(value);

    /// <summary>Implicit conversion from <see cref="Error"/> to a failed result.</summary>
    public static implicit operator Result<T>(Error error) => Failure(error);
}

/// <summary>
/// Non-generic result used for commands that return no value.
/// </summary>
public sealed class Result
{
    private Result(bool isSuccess, Error error)
    {
        IsSuccess = isSuccess;
        Error = error;
    }

    /// <summary>Gets a value indicating whether the result is a success.</summary>
    public bool IsSuccess { get; }

    /// <summary>Gets a value indicating whether the result is a failure.</summary>
    public bool IsFailure => !IsSuccess;

    /// <summary>Gets the error. Returns <see cref="Error.None"/> on success.</summary>
    public Error Error { get; }

    /// <summary>A cached successful result.</summary>
    public static readonly Result Success = new(true, Error.None);

    /// <summary>Creates a failed result.</summary>
    /// <param name="error">The error describing the failure.</param>
    /// <returns>A failed <see cref="Result"/>.</returns>
    public static Result Failure(Error error) => new(false, error);
}
