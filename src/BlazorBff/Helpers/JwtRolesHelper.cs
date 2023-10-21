using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BlazorBff.Helpers;

public static class JwtRolesHelper
{
    private const string _resourceAccess = "resource_access";
    private const string _realmAccess = "realm_access";

    public static string[] ExtractRoles(
        string? rawJwtAccessToken,
        bool includeRealmRoles = false)
    {
        if (string.IsNullOrEmpty(rawJwtAccessToken))
        {
            return Array.Empty<string>();
        }

        var handler = new JwtSecurityTokenHandler();
        var accessTokenDecoded = handler.ReadJwtToken(rawJwtAccessToken);
        var res = new List<string>();

        var resAccess = accessTokenDecoded.Claims.FirstOrDefault(c => c.Type == _resourceAccess);
        res.AddRange(GetResourceAccessRoles(resAccess));

        if (includeRealmRoles)
        {
            var realmAccess = accessTokenDecoded.Claims.FirstOrDefault(c => c.Type == _realmAccess);
            res.AddRange(GetRealmAccessRoles(realmAccess));
        }

        return res.ToArray();
    }

    private static string[] GetRealmAccessRoles(Claim? realmAccessClaim)
    {
        if (realmAccessClaim == null || realmAccessClaim.Type != _realmAccess)
        {
            return Array.Empty<string>();
        }

        var realmAccess = JsonSerializer.Deserialize<RealmAccess>(realmAccessClaim.Value);

        if (realmAccess == null)
        {
            return Array.Empty<string>();
        }

        return realmAccess.Roles;
    }

    private static string[] GetResourceAccessRoles(Claim? resourceAccessClaim)
    {
        if (resourceAccessClaim == null || resourceAccessClaim.Type != _resourceAccess)
        {
            return Array.Empty<string>();
        }

        var resourceAccess = JsonSerializer.Deserialize<ResourceAccess>(resourceAccessClaim.Value);

        if (resourceAccess == null || resourceAccess.Account == null)
        {
            return Array.Empty<string>();
        }

        return resourceAccess.Account.Roles;
    }

    private sealed record RealmAccess
    {
        [JsonPropertyName("roles")]
        public string[] Roles { get; init; } = Array.Empty<string>();
    }
    private sealed record ResourceAccess
    {
        [JsonPropertyName("account")]
        public Account? Account { get; init; }
    }
    private sealed record Account
    {
        [JsonPropertyName("roles")]
        public string[] Roles { get; init; } = Array.Empty<string>();
    }
}

