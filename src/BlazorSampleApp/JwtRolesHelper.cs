using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;

namespace BlazorSampleApp;

public static class JwtRolesHelper
{
    private const string _resourceAccess = "resource_access";
    private const string _realmAccess = "realm_access";

    public static string[] ExtractRoles(
        string? rawJwtAccessToken,
        bool includeRealmRoles = false)
    {
        var resources = Array.Empty<string>();
        return ExtractRoles(rawJwtAccessToken, resources, includeRealmRoles);
    }

    public static string[] ExtractRoles(
    string? rawJwtAccessToken,
    string[] resources,
    bool includeRealmRoles = false)
    {
        if (string.IsNullOrEmpty(rawJwtAccessToken))
        {
            return [];
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

        return [.. res];
    }

    private static string[] GetRealmAccessRoles(Claim? realmAccessClaim)
    {
        if (realmAccessClaim == null || realmAccessClaim.Type != _realmAccess)
        {
            return [];
        }

        var realmAccessDict = JsonSerializer.Deserialize<IDictionary<string, string[]>>(realmAccessClaim.Value);

        if (realmAccessDict != null && realmAccessDict.TryGetValue("roles", out var realmAccessRoles))
        {
            return realmAccessRoles;
        }

        return [];
    }

    private static string[] GetResourceAccessRoles(Claim? resourceAccessClaim, string[] resourceNames)
    {
        if (resourceAccessClaim == null || resourceAccessClaim.Type != _resourceAccess)
        {
            return [];
        }

        var resourceAccess = JsonSerializer.Deserialize<IDictionary<string, IDictionary<string, string[]>>>(resourceAccessClaim.Value);

        if (resourceAccess == null)
        {
            return [];
        }

        var roles = new List<string>();
        var resourceKeys = resourceNames.Length != 0 ? resourceNames : resourceAccess.Keys;

        foreach (var resource in resourceKeys)
        {
            if (resourceAccess.TryGetValue(resource, out var resourceRolesDict))
            {
                if (resourceRolesDict.TryGetValue("roles", out var resourceRoles))
                {
                    roles.AddRange(resourceRoles);
                }
            }
        }

        return [.. roles];
    }
}

