using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;

namespace BlazorBff.Helpers;

public static class JwtRolesHelper
{
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

        var resAccess = accessTokenDecoded.Claims.FirstOrDefault(c => c.Type == "resource_access");
        res.AddRange(GetResourceAccessRoles(resAccess));

        if (includeRealmRoles)
        {
            var realmAccess = accessTokenDecoded.Claims.FirstOrDefault(c => c.Type == "realm_access");
            res.AddRange(GetRealmAccessRoles(realmAccess));
        }

        return res.ToArray();
    }

    private static string[] GetRealmAccessRoles(Claim? realmAccessClaim)
    {
        if (realmAccessClaim == null || realmAccessClaim.Type != "realm_access")
        {
            return Array.Empty<string>();
        }

        var realmAccess = JsonSerializer.Deserialize<Realm_Access>(realmAccessClaim.Value);

        if (realmAccess == null)
        {
            return Array.Empty<string>();
        }

        return realmAccess.Roles;
    }

    private static string[] GetResourceAccessRoles(Claim? resourceAccessClaim)
    {
        if (resourceAccessClaim == null || resourceAccessClaim.Type != "resource_access")
        {
            return Array.Empty<string>();
        }

        var resourceAccess = JsonSerializer.Deserialize<Resource_Access>(resourceAccessClaim.Value);

        if (resourceAccess == null || resourceAccess.Account == null)
        {
            return Array.Empty<string>();
        }

        return resourceAccess.Account.Roles;
    }

    private sealed record Realm_Access
    {
        public string[] Roles { get; init; } = Array.Empty<string>();
    }
    private sealed record Resource_Access
    {
        public Account? Account { get; init; }
    }
    private sealed record Account
    {
        public string[] Roles { get; init; } = Array.Empty<string>();
    }
}

