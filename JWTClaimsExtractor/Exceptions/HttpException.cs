namespace JWTClaimsExtractor.Exceptions;

public class HttpException : Exception
{
    public int StatusCode { get; }

    public HttpException(int statusCode) : this(statusCode, $"Response failed with status code: {statusCode}")
    {
    }

    public HttpException(int statusCode, string message) : base(message)
    {
        StatusCode = statusCode;
    }

    public HttpException(int statusCode, string message, Exception inner) : base(message, inner)
    {
        StatusCode = statusCode;
    }
}