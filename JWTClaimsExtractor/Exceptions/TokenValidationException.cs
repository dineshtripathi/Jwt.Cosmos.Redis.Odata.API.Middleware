namespace JWTClaimsExtractor.Exceptions;

public class TokenValidationException : Exception
{
    public TokenValidationException(int statusCode, string message, string details = null)
        : base(message)
    {
        StatusCode = statusCode;
        Details = details;
    }

    public TokenValidationException(int statusCode, string message, Exception inner, string details = null)
        : base(message, inner)
    {
        StatusCode = statusCode;
        Details = details;
    }

    public int StatusCode { get; }

    public string Details { get; }
}