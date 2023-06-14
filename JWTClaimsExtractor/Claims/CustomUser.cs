namespace JWTClaimsExtractor.Claims;

public record CustomUser(IDictionary<string, object> JwtCustomClaims = default!);