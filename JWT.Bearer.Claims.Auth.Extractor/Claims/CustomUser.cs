namespace JWT.Bearer.Claims.Auth.Extractor.Claims;

public record CustomUser(IDictionary<string, object> JwtCustomClaims = default!);