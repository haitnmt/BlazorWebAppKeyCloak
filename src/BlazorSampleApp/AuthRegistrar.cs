using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using BlazorSampleApp.Client;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace BlazorSampleApp;

public static class AuthRegistrar
{
    // private const string MS_OIDC_SCHEME = "MicrosoftOidc";

    internal static void AddOidcAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var config = new OidcConfiguration();
        configuration.GetSection(OidcConfiguration.OidcSection).Bind(config);

        services
            .AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
                options.DefaultSignOutScheme = OpenIdConnectDefaults.AuthenticationScheme;

            })
            .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, oidcOptions =>
            {
                oidcOptions.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;

                oidcOptions.Authority = config.Authority;
                oidcOptions.MetadataAddress = config.MetadataUrl;
                oidcOptions.RequireHttpsMetadata = false;

                oidcOptions.ClientId = config.ClientId;
                oidcOptions.ClientSecret = config.ClientSecret;

                oidcOptions.ResponseType = OpenIdConnectResponseType.Code;
                oidcOptions.MapInboundClaims = false;
                oidcOptions.TokenValidationParameters.NameClaimType = JwtRegisteredClaimNames.Name;
                oidcOptions.TokenValidationParameters.RoleClaimType = "role";

                oidcOptions.Events = new OpenIdConnectEvents
                {
                    OnTokenValidated = OnTokenValidated,
                };

            })
            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
            {
                // set session lifetime
                options.ExpireTimeSpan = TimeSpan.FromHours(8);

                // sliding or absolute
                options.SlidingExpiration = false;

                // host prefixed cookie name
                //options.Cookie.Name = "__Host-spa";

                // strict SameSite handling
                //options.Cookie.SameSite = SameSiteMode.Strict;
            });

        /*
        services
        .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
        {
            options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            options.Authority = config.Authority;
            options.ClientId = config.ClientId;
            options.ClientSecret = config.ClientSecret;
            options.MetadataAddress = config.MetadataUrl;
            options.RequireHttpsMetadata = false;

            options.MapInboundClaims = false;
            options.ResponseType = OpenIdConnectResponseType.Code;

            options.TokenValidationParameters.NameClaimType = JwtRegisteredClaimNames.Name;
            options.TokenValidationParameters.RoleClaimType = "role";
        })
        .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
        {
            // set session lifetime
            options.ExpireTimeSpan = TimeSpan.FromHours(8);

            // sliding or absolute
            options.SlidingExpiration = false;

            // host prefixed cookie name
            options.Cookie.Name = "__Host-spa";

            // strict SameSite handling
            options.Cookie.SameSite = SameSiteMode.Strict;
        });


        .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
        {
            options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;

            options.Authority = config.Authority;
            options.ClientId = config.ClientId;
            options.ClientSecret = config.ClientSecret;
            options.MetadataAddress = config.MetadataUrl;

            //options.CallbackPath = config.CallbackPath;
            options.ResponseType = OpenIdConnectResponseType.Code;
            options.ResponseMode = OpenIdConnectResponseMode.Query;
            options.MapInboundClaims = false;
            options.GetClaimsFromUserInfoEndpoint = true;

            // save tokens into authentication session
            // to enable automatic token management
            options.SaveTokens = true;

            options.Scope.Add("profile");
            options.Scope.Add("roles");
            options.Scope.Add(OpenIdConnectScope.OfflineAccess);
            options.Scope.Add(OpenIdConnectScope.OpenIdProfile);

            options.UsePkce = true;
            options.RequireHttpsMetadata = false;
            options.GetClaimsFromUserInfoEndpoint = true;

            // options.MapInboundClaims = false;
            options.TokenValidationParameters.NameClaimType = JwtRegisteredClaimNames.Name;
            options.TokenValidationParameters.RoleClaimType = "role";

            options.Events = new OpenIdConnectEvents
            {
                OnTokenValidated = OnTokenValidated,
            };
        });
        */
    }

    public static IServiceCollection ConfigureCookieOidcRefresh(this IServiceCollection services, string cookieScheme, string oidcScheme)
    {
        services.AddSingleton<CookieOidcRefresher>();
        services.AddOptions<CookieAuthenticationOptions>(cookieScheme).Configure<CookieOidcRefresher>((cookieOptions, refresher) =>
        {
            cookieOptions.Events.OnValidatePrincipal = context => refresher.ValidateOrRefreshCookieAsync(context, oidcScheme);
        });
        services.AddOptions<OpenIdConnectOptions>(oidcScheme).Configure(oidcOptions =>
        {
            // Request a refresh_token.
            oidcOptions.Scope.Add(OpenIdConnectScope.OfflineAccess);
            // Store the refresh_token.
            oidcOptions.SaveTokens = true;
        });
        return services;
    }

    private static Task OnTokenValidated(TokenValidatedContext context)
    {
        if (context.Principal == null)
        {
            return Task.CompletedTask;
        }

#if DEBUG
        var userInfo = UserInfo.FromClaimsPrincipal(context.Principal);
        Console.WriteLine($"User Info: UserId={userInfo.UserId} Name={userInfo.Name} Email={userInfo.Email}");
#endif

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
            return [];
        }

        // TODO: Add roles from DB by userLogin and/or userName
        return [];
    }
}
