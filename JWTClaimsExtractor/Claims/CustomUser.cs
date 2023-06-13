namespace JWTClaimsExtractor.Claims;

public record CustomUser(IDictionary<string, object> AdditionalClaims = default!);