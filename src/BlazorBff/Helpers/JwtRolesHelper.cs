using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;

namespace BlazorBff.Helpers;

public static class JwtRolesHelper
{
    private const string _resourceAccess = "resource_access";
    private const string _realmAccess = "realm_access";

    public static string[] ExtractRoles(
        string? rawJwtAccessToken,
        string[] resources,
        bool includeRealmRoles = false)
    {
        if (string.IsNullOrEmpty(rawJwtAccessToken))
        {
            return Array.Empty<string>();
        }

        var handler = new JwtSecurityTokenHandler();
        var accessTokenDecoded = handler.ReadJwtToken(rawJwtAccessToken);
        var res = new List<string>();

        if (includeRealmRoles)
        {
            var realmAccess = accessTokenDecoded.Claims.FirstOrDefault(c => c.Type == _realmAccess);
            res.AddRange(GetRealmAccessRoles(realmAccess));
        }

        var resAccess = accessTokenDecoded.Claims.FirstOrDefault(c => c.Type == _resourceAccess);
        res.AddRange(GetResourceAccessRoles(resAccess, resources));

        return res.ToArray();
    }

    private static string[] GetRealmAccessRoles(Claim? realmAccessClaim)
    {
        if (realmAccessClaim == null || realmAccessClaim.Type != _realmAccess)
        {
            return Array.Empty<string>();
        }

        var realmAccessDict = JsonSerializer.Deserialize<IDictionary<string, string[]>>(realmAccessClaim.Value);

        if (realmAccessDict != null && realmAccessDict.TryGetValue("roles", out var realmAccessRoles))
        {
            return realmAccessRoles;
        }

        return Array.Empty<string>();
    }

    private static string[] GetResourceAccessRoles(Claim? resourceAccessClaim, string[] resourceNames)
    {
        if (resourceAccessClaim == null || resourceAccessClaim.Type != _resourceAccess)
        {
            return Array.Empty<string>();
        }

        var resourceAccess = JsonSerializer.Deserialize<IDictionary<string, IDictionary<string, string[]>>>(resourceAccessClaim.Value);

        if (resourceAccess == null)
        {
            return Array.Empty<string>();
        }

        var roles = new List<string>();

        foreach (var resource in resourceNames)
        {
            if (resourceAccess.TryGetValue(resource, out var resourceRolesDict))
            {
                if (resourceRolesDict.TryGetValue("roles", out var resourceRoles))
                {
                    roles.AddRange(resourceRoles);
                }
            }
        }

        return roles.ToArray();
    }
}

