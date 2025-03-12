using System.Security.Claims;

namespace BlazorSampleApp.Client;

// Add properties to this class and update the server and client AuthenticationStateProviders
// to expose more information about the authenticated user to the client.
public sealed class UserInfo
{
    public required string UserId { get; init; }
    public required string Name { get; init; }
    public required string Email { get; init; }
    public string? AccessToken { get; init; }
    public string? IdToken { get; init; }
    public string? RefreshToken { get; init; }
    public DateTimeOffset? TokenExpiration { get; init; }
    public List<string> Roles { get; init; } = new();

    public const string UserIdClaimType = "sub";
    public const string NameClaimType = "name";
    public const string EmailClaimType = "email";
    public const string AccessTokenPropertyName = "access_token";
    public const string IdTokenPropertyName = "id_token";
    public const string RefreshTokenPropertyName = "refresh_token";
    public const string ExpiresAtPropertyName = "expires_at";
    public const string RoleClaimType = "role";

    public static UserInfo FromClaimsPrincipal(ClaimsPrincipal principal) =>
        new()
        {
            UserId = GetRequiredClaim(principal, UserIdClaimType),
            Name = GetRequiredClaim(principal, NameClaimType),
            Email = GetRequiredClaim(principal, EmailClaimType),
            // Authentication tokens not available directly from claims
            Roles = principal.FindAll(RoleClaimType).Select(c => c.Value).ToList()
        };

    public static UserInfo FromClaimsPrincipalWithTokens(ClaimsPrincipal principal, string? accessToken, string? idToken, string? refreshToken, DateTimeOffset? expiresAt) =>
        new()
        {
            UserId = GetRequiredClaim(principal, UserIdClaimType),
            Name = GetRequiredClaim(principal, NameClaimType),
            Email = GetRequiredClaim(principal, EmailClaimType),
            AccessToken = accessToken,
            IdToken = idToken,
            RefreshToken = refreshToken,
            TokenExpiration = expiresAt,
            Roles = principal.FindAll(RoleClaimType).Select(c => c.Value).ToList()
        };

    public ClaimsPrincipal ToClaimsPrincipal()
    {
        var claims = new List<Claim>
        {
            new(UserIdClaimType, UserId),
            new(NameClaimType, Name),
            new(EmailClaimType, Email)
        };

        // Add role claims
        claims.AddRange(Roles.Select(role => new Claim(RoleClaimType, role)));
        
        return new ClaimsPrincipal(new ClaimsIdentity(
            claims,
            authenticationType: nameof(UserInfo),
            nameType: NameClaimType,
            roleType: RoleClaimType));
    }

    private static string GetRequiredClaim(ClaimsPrincipal principal, string claimType) =>
        principal.FindFirst(claimType)?.Value ?? throw new InvalidOperationException($"Could not find required '{claimType}' claim.");
}
