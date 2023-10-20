using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// builder.Services.AddBff();

builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = "cookie";
        options.DefaultChallengeScheme = "oidc";
        options.DefaultSignOutScheme = "oidc";
    })
    .AddCookie("cookie", options =>
    {
        options.Cookie.Name = "__Host-blazor";
        options.Cookie.SameSite = SameSiteMode.Strict;
    })
    .AddOpenIdConnect("oidc", options =>
    {
        options.Authority = builder.Configuration["OpenIDConnectSettings:Authority"];
        options.ClientId = builder.Configuration["OpenIDConnectSettings:ClientId"];
        options.ClientSecret = builder.Configuration["OpenIDConnectSettings:ClientSecret"];
        options.ResponseType = OpenIdConnectResponseType.Code;
        options.CallbackPath = "/auth/*";
        options.RequireHttpsMetadata = false;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            NameClaimType = ClaimTypes.Name,
            RoleClaimType = ClaimTypes.Role,
            ValidateIssuer = true,
        };

        options.MapInboundClaims = false;
        options.GetClaimsFromUserInfoEndpoint = true;
        options.SaveTokens = true;

        options.Scope.Clear();
        options.Scope.Add("profile");
        options.Scope.Add("email");
        options.Scope.Add("roles");
        options.Scope.Add("openid");
        options.Scope.Add("offline_access");

        options.Events = new OpenIdConnectEvents
        {
            OnTokenValidated = OnTokenValidated,
            OnAuthorizationCodeReceived = OnAuthorizationCodeReceived,
        };
    });

builder.Services.AddAuthorization();

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
// app.UseBff();
app.UseAuthorization();

app.MapBffManagementEndpoints();

app.MapRazorPages();

app.MapControllers()
    .RequireAuthorization();

// .AsBffApiEndpoint();

app.MapFallbackToFile("index.html");

app.Run();

static Task OnAuthorizationCodeReceived(AuthorizationCodeReceivedContext context)
{
    // Console.WriteLine($"OnAuthorizationCodeReceived Succeeded={context.Result.Succeeded}");
    Console.WriteLine($"OnAuthorizationCodeReceived");
    return Task.CompletedTask;
}

static Task OnTokenValidated(TokenValidatedContext context)
{
    var res = Task.CompletedTask;

    Console.WriteLine($"token={context.SecurityToken.RawData}");

    if (context.Principal == null)
    {
        return res;
    }

    var userClaims = context.Principal.Claims.ToLookup(c => c.Type, c => c.Value);

    foreach (var claim in context.SecurityToken.Claims)
    {
        Console.WriteLine($"type={claim.Type} value={claim.Value}  props=[{DictToString(claim.Properties)}]");
    }

    var userLogin = userClaims["preferred_username"].FirstOrDefault();
    var userName = userClaims["name"].FirstOrDefault();

    if (userLogin == null && userName == null)
    {
        return res;
    }

    return res;
}

static string DictToString(IDictionary<string, string> dict)
{
    var sb = new StringBuilder();

    foreach (var kvp in dict)
    {
        sb.Append($"key={kvp.Key} val={kvp.Value}; ");
    }

    return sb.ToString();
}
