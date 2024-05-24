# keycloak4blazor

Keycloak for Blazor demo

## Keycloak resources

- [Single Sign-on user authentication on Blazor WebAssembly SignalR client](https://scientificprogrammer.net/2022/08/12/single-sign-on-user-authentication-on-blazor-webassembly-signalr-client/)
- [ASP.NET Core - Keycloak authorization guide](https://github.com/tuxiem/AspNetCore-keycloak/tree/master)
- [Posts tagged with keycloak](https://nikiforovall.github.io/tags.html#keycloak-ref)
- [Using OpenID Connect to secure applications and services](https://www.keycloak.org/docs/latest/securing_apps/#_oidc)
- [OpenID Connect Client with .NET](https://curity.io/resources/learn/dotnet-openid-connect-website/)
- [C#/NetStandard OpenID Connect Client Library for native Applications](https://github.com/IdentityModel/IdentityModel.OidcClient)
- [Secure ASP.NET Core Blazor WebAssembly](https://learn.microsoft.com/en-us/aspnet/core/blazor/security/webassembly/?view=aspnetcore-7.0)
- [OpenIDConnect Response Type Confusion](https://stackoverflow.com/questions/29275477/openidconnect-response-type-confusion)
- [Blazor.BFF.OpenIDConnect.Template](https://github.com/damienbod/Blazor.BFF.OpenIDConnect.Template)

## BFF Security

- [Securing SPAs and Blazor Applications using the BFF (Backend for Frontend) Pattern - Dominick Baier](https://www.youtube.com/watch?v=hWJuX-8Ur2k)
- [Backend for Frontend (BFF) Security Framework](https://duendesoftware.com/products/bff)

## Openid connect
- [OpenID Connect Client with .NET](https://curity.io/resources/learn/dotnet-openid-connect-website/)
- [Persist additional claims and tokens from external providers in ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/security/authentication/social/additional-claims?view=aspnetcore-7.0)


## Additional
- [Server-side ASP.NET Core Blazor additional security scenarios](https://learn.microsoft.com/en-us/aspnet/core/blazor/security/server/additional-scenarios?view=aspnetcore-7.0)
- [OIDC authentication in server-side Blazor](https://stackoverflow.com/questions/64853618/oidc-authentication-in-server-side-blazor)
- [Authentication and Authorization](https://gist.github.com/SteveSandersonMS/175a08dcdccb384a52ba760122cd2eda)
- [BlazorWebAssemblyCookieAuth](https://github.com/berhir/BlazorWebAssemblyCookieAuth)

## Access token
- [AccessTokenManagement](https://github.com/DuendeSoftware/Duende.AccessTokenManagement)

## Blazor 8

https://learn.microsoft.com/en-us/aspnet/core/blazor/security/?view=aspnetcore-8.0



## Keycloak setup

```
docker pull keycloak/keycloak

docker run -e KEYCLOAK_ADMIN=admin -e KEYCLOAK_ADMIN_PASSWORD=admin -p 8080:8080 keycloak/keycloak:latest start-dev

http://localhost:8080/

```


- Create realm: myrealm
- Create user:  afedyanin, set password
- Create group: mygroup 
- Add user to group
- Create global role: myrole
- Assign role to user

```
http://localhost:8080/realms/myrealm/account
```

- Create Client: blazor-client
- AuthFlow: Standard flow, Direct access grants
- Client Auth - On

- Add valid redirect urls: http://localhost:5278/*
- Enable CORS on Keycloak +
- Download adapter config

```
{
  "realm": "myrealm",
  "auth-server-url": "http://localhost:8080/",
  "ssl-required": "external",
  "resource": "blazor-client",
  "credentials": {
    "secret": "aNZUREfcTwZjh1qiD095SGQnzL6SQWo0"
  },
  "confidential-port": 0
}
```

```
curl --data "grant_type=password&client_id=blazor-client&username=afedyanin&password=afedyanin&client_secret=aNZUREfcTwZjh1qiD095SGQnzL6SQWo0" localhost:8080/realms/myrealm/protocol/openid-connect/token
```



