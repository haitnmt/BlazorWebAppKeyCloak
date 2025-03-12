using System.Collections.Immutable;
using System.Security.Claims;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace BlazorSampleApp.Client;

// This is a client-side AuthenticationStateProvider that determines the user's authentication state by
// looking for data persisted in the page when it was rendered on the server. This authentication state will
// be fixed for the lifetime of the WebAssembly application. So, if the user needs to log in or out, a full
// page reload is required.
internal sealed class PersistentAuthenticationStateProvider : AuthenticationStateProvider
{
    private static readonly Task<AuthenticationState> defaultUnauthenticatedTask =
        Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity())));

    private readonly Task<AuthenticationState> authenticationStateTask = defaultUnauthenticatedTask;

    // Store the UserInfo for access by other components
    public UserInfo? CurrentUserInfo { get; private set; }

    public PersistentAuthenticationStateProvider(PersistentComponentState state)
    {
        if (!state.TryTakeFromJson<UserInfo>(nameof(UserInfo), out var userInfo) || userInfo is null)
        {
            return;
        }

        CurrentUserInfo = userInfo;
        authenticationStateTask = Task.FromResult(new AuthenticationState(userInfo.ToClaimsPrincipal()));
    }

    public override Task<AuthenticationState> GetAuthenticationStateAsync() => authenticationStateTask;

    // Provide a way to access the authentication tokens
    public string? GetAccessToken() => CurrentUserInfo?.AccessToken;
    public string? GetIdToken() => CurrentUserInfo?.IdToken;
    public string? GetRefreshToken() => CurrentUserInfo?.RefreshToken;
    public DateTimeOffset? GetTokenExpiration() => CurrentUserInfo?.TokenExpiration;
    public IReadOnlyList<string> GetRoles() => CurrentUserInfo?.Roles ?? [];
}
