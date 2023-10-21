using BlazorBff.Helpers;
using IdentityModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

builder.Services.AddAuthentication(options =>
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
        options.Authority = builder.Configuration["OpenIDConnectSettings:Authority"];
        options.ClientId = builder.Configuration["OpenIDConnectSettings:ClientId"];
        options.ClientSecret = builder.Configuration["OpenIDConnectSettings:ClientSecret"];
        // options.CallbackPath = "/Account/Login-callback";
        options.ResponseType = OpenIdConnectResponseType.Code;
        options.Scope.Add("profile");
        options.Scope.Add("roles");
        options.Scope.Add("offline_access");
        options.SaveTokens = true;
        options.UsePkce = true;
        options.GetClaimsFromUserInfoEndpoint = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            NameClaimType = JwtClaimTypes.Name,
            RoleClaimType = JwtClaimTypes.Role,
        };

        options.Events = new OpenIdConnectEvents
        {
            OnTokenValidated = OnTokenValidated,
        };

        options.RequireHttpsMetadata = false;
    });

builder.Services.AddAccessTokenManagement();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();

static Task OnTokenValidated(TokenValidatedContext context)
{
    var res = Task.CompletedTask;

    // Console.WriteLine($"token={context.SecurityToken.RawData}");

    if (context.Principal == null)
    {
        return res;
    }

    var accessToken = context.TokenEndpointResponse!.AccessToken;
    var roles = JwtRolesHelper.ExtractRoles(accessToken, true);
    Console.WriteLine($"Roles: {string.Join(", ", roles)}");

    var userClaims = context.Principal.Claims.ToLookup(c => c.Type, c => c.Value);
    var userLogin = userClaims["preferred_username"].FirstOrDefault();
    var userName = userClaims["name"].FirstOrDefault();

    if (userLogin == null && userName == null)
    {
        return res;
    }

    return res;
}
