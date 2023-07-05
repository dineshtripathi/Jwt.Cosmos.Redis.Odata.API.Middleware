namespace JWT.Bearer.Claims.Auth.Extractor.Exceptions;
/// <summary>
/// The http exception.
/// </summary>

public class HttpException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="HttpException"/> class.
    /// </summary>
    /// <param name="statusCode">The status code.</param>
    public HttpException(int statusCode) : this(statusCode, $"Response failed with status code: {statusCode}")
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="HttpException"/> class.
    /// </summary>
    /// <param name="statusCode">The status code.</param>
    /// <param name="message">The message.</param>
    public HttpException(int statusCode, string message) : base(message)
    {
        StatusCode = statusCode;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="HttpException"/> class.
    /// </summary>
    /// <param name="statusCode">The status code.</param>
    /// <param name="message">The message.</param>
    /// <param name="inner">The inner.</param>
    public HttpException(int statusCode, string message, Exception inner) : base(message, inner)
    {
        StatusCode = statusCode;
    }

    /// <summary>
    /// Gets the status code.
    /// </summary>
    public int StatusCode { get; }
}