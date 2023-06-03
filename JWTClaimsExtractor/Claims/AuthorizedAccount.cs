namespace JWTClaimsExtractor.Claims;

public record AuthorizedAccount
{
    public string Id { get; set; } = default!;
    public string OrganizationId { get; set; } = default!;
    public string CustomerAccountNumber { get; set; } = default!;
    public string? LetterOfAuthorityId { get; set; }  // Now it can be null
    public string? RequestId { get; set; }  // New property
    public long CreatedOn { get; set; }  // New property
}
