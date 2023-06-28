namespace JWTClaimsExtractor.Exceptions;
/// <summary>
/// The token validation exception.
/// </summary>

public class TokenValidationException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TokenValidationException"/> class.
    /// </summary>
    /// <param name="statusCode">The status code.</param>
    /// <param name="message">The message.</param>
    /// <param name="details">The details.</param>
    public TokenValidationException(int statusCode, string message, string? details = null)
        : base(message)
    {
        StatusCode = statusCode;
        Details = details;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TokenValidationException"/> class.
    /// </summary>
    /// <param name="statusCode">The status code.</param>
    /// <param name="message">The message.</param>
    /// <param name="inner">The inner.</param>
    /// <param name="details">The details.</param>
    public TokenValidationException(int statusCode, string message, Exception inner, string? details = null)
        : base(message, inner)
    {
        StatusCode = statusCode;
        Details = details;
    }

    /// <summary>
    /// Gets the status code.
    /// </summary>
    public int StatusCode { get; }

    /// <summary>
    /// Gets the details.
    /// </summary>
    public string? Details { get; }
}