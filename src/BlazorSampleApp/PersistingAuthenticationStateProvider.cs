using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Http;
using BlazorSampleApp.Client;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;

namespace BlazorSampleApp;

// This is a server-side AuthenticationStateProvider that uses PersistentComponentState to flow the
// authentication state to the client which is then fixed for the lifetime of the WebAssembly application.
internal sealed class PersistingAuthenticationStateProvider : AuthenticationStateProvider, IHostEnvironmentAuthenticationStateProvider, IDisposable
{
    private readonly PersistentComponentState persistentComponentState;
    private readonly PersistingComponentStateSubscription subscription;
    private readonly IHttpContextAccessor httpContextAccessor;
    private Task<AuthenticationState>? authenticationStateTask;

    public PersistingAuthenticationStateProvider(PersistentComponentState state, IHttpContextAccessor httpContextAccessor)
    {
        persistentComponentState = state;
        this.httpContextAccessor = httpContextAccessor;
        subscription = state.RegisterOnPersisting(OnPersistingAsync, RenderMode.InteractiveWebAssembly);
    }

    public override Task<AuthenticationState> GetAuthenticationStateAsync() => authenticationStateTask ??
            throw new InvalidOperationException($"Do not call {nameof(GetAuthenticationStateAsync)} outside of the DI scope for a Razor component. Typically, this means you can call it only within a Razor component or inside another DI service that is resolved for a Razor component.");

    public void SetAuthenticationState(Task<AuthenticationState> task)
    {
        authenticationStateTask = task;
    }

    private async Task OnPersistingAsync()
    {
        var authenticationState = await GetAuthenticationStateAsync();
        var principal = authenticationState.User;

        if (principal.Identity?.IsAuthenticated == true)
        {
            var httpContext = httpContextAccessor.HttpContext;
            if (httpContext != null)
            {
                // Get authentication tokens from the current session
                var accessToken = await httpContext.GetTokenAsync(OpenIdConnectDefaults.AuthenticationScheme, UserInfo.AccessTokenPropertyName);
                var idToken = await httpContext.GetTokenAsync(OpenIdConnectDefaults.AuthenticationScheme, UserInfo.IdTokenPropertyName);
                var refreshToken = await httpContext.GetTokenAsync(OpenIdConnectDefaults.AuthenticationScheme, UserInfo.RefreshTokenPropertyName);
                
                // Get token expiration time
                DateTimeOffset? expiresAt = null;
                var expiresAtStr = await httpContext.GetTokenAsync(OpenIdConnectDefaults.AuthenticationScheme, UserInfo.ExpiresAtPropertyName);
                if (!string.IsNullOrEmpty(expiresAtStr) && DateTimeOffset.TryParse(expiresAtStr, out var expiration))
                {
                    expiresAt = expiration;
                }
                
                var userInfo = UserInfo.FromClaimsPrincipalWithTokens(principal, accessToken, idToken, refreshToken, expiresAt);
                persistentComponentState.PersistAsJson(nameof(UserInfo), userInfo);
            }
            else
            {
                persistentComponentState.PersistAsJson(nameof(UserInfo), UserInfo.FromClaimsPrincipal(principal));
            }
        }
    }

    public void Dispose()
    {
        subscription.Dispose();
    }
}
