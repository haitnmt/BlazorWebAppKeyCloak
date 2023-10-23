using System.Security.Claims;
using BlazorBff.Configuration;
using BlazorBff.Helpers;
using IdentityModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

namespace BlazorBff;

public static class AuthorizationRegistrar
{
    internal static void AddOidcAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var config = new OidcConfiguration();
        configuration.GetSection(OidcConfiguration.OidcSection).Bind(config);

        services.AddAuthentication(options =>
        {
            options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
        })
        .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
        {
            options.Cookie.SameSite = SameSiteMode.Strict;

            options.Events.OnSigningOut = async e =>
            {
                await e.HttpContext.RevokeUserRefreshTokenAsync();
            };
        })
        .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
        {
            options.Authority = config.Authority;
            options.ClientId = config.ClientId;
            options.ClientSecret = config.ClientSecret;
            options.MetadataAddress = config.MetadataUrl;
            options.CallbackPath = config.CallbackPath;
            options.ResponseType = OpenIdConnectResponseType.Code;
            options.Scope.Add("profile");
            options.Scope.Add("roles");
            options.Scope.Add("offline_access");
            options.SaveTokens = true;
            options.UsePkce = true;
            options.GetClaimsFromUserInfoEndpoint = true;
            options.RequireHttpsMetadata = false;

            options.TokenValidationParameters = new TokenValidationParameters
            {
                NameClaimType = JwtClaimTypes.Name,
                RoleClaimType = JwtClaimTypes.Role,
            };

            options.Events = new OpenIdConnectEvents
            {
                OnTokenValidated = OnTokenValidated,
            };
        });

        services.AddAccessTokenManagement();
    }

    private static Task OnTokenValidated(TokenValidatedContext context)
    {
        if (context.Principal == null)
        {
            return Task.CompletedTask;
        }

        var roles = new List<string>();

        // Get roles from access token
        var accessToken = context.TokenEndpointResponse!.AccessToken;

#if DEBUG
        Console.WriteLine($"access token={accessToken}");
#endif

        roles.AddRange(JwtRolesHelper.ExtractRoles(accessToken));

        // Get reoles from DB if req
        roles.AddRange(GetRolesByUserIdentity(context.Principal));

#if DEBUG
        Console.WriteLine($"Roles: {string.Join(", ", roles)}");
#endif
        var claims = roles.Select(r => new Claim(ClaimsIdentity.DefaultRoleClaimType, r));

        if (claims.Any())
        {
            context.Principal.AddIdentity(new ClaimsIdentity(claims));
        }

        return Task.CompletedTask;
    }

    private static string[] GetRolesByUserIdentity(ClaimsPrincipal claimsPrincipal)
    {
        var userClaims = claimsPrincipal.Claims.ToLookup(c => c.Type, c => c.Value);
        var userLogin = userClaims["preferred_username"].FirstOrDefault();
        var userName = userClaims["name"].FirstOrDefault();

        if (userLogin == null && userName == null)
        {
            return Array.Empty<string>();
        }

        // TODO: Add roles from DB by userLogin and/or userName
        return Array.Empty<string>();
    }
}
