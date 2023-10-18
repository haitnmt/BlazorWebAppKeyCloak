# keycloak4blazor

Keycloak for Blazor demo

## Keycloak resources

- [Keycloak.AuthServices](https://github.com/NikiforovAll/keycloak-authorization-services-dotnet)
- [Use Keycloak as Identity Provider from Blazor WebAssembly](https://nikiforovall.github.io/blazor/dotnet/2022/12/08/dotnet-keycloak-blazorwasm-auth.html)
- [Single Sign-on user authentication on Blazor WebAssembly SignalR client](https://scientificprogrammer.net/2022/08/12/single-sign-on-user-authentication-on-blazor-webassembly-signalr-client/)
- [ASP.NET Core - Keycloak authorization guide](https://github.com/tuxiem/AspNetCore-keycloak/tree/master)
- [Posts tagged with keycloak](https://nikiforovall.github.io/tags.html#keycloak-ref)
- [Using OpenID Connect to secure applications and services](https://www.keycloak.org/docs/latest/securing_apps/#_oidc)
- [OpenID Connect Client with .NET](https://curity.io/resources/learn/dotnet-openid-connect-website/)
- [C#/NetStandard OpenID Connect Client Library for native Applications](https://github.com/IdentityModel/IdentityModel.OidcClient)
- [Secure ASP.NET Core Blazor WebAssembly](https://learn.microsoft.com/en-us/aspnet/core/blazor/security/webassembly/?view=aspnetcore-7.0)
- [OpenIDConnect Response Type Confusion](https://stackoverflow.com/questions/29275477/openidconnect-response-type-confusion)



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

Add an audience to a client by using client scopes
- On the left side bar click on “Clients” item.
- Click “blazor-client”
- Open “Client scopes” tab
- Click on “blazor-client-dedicated”, should be on tope of the list of scopes
- From the “Mappers” tab, click “Create a new mapper”
- Pick “Audience” from the list
- specify name: Audience
- include client audience: “blazor-client”
- Click “Save”
Besides “Setup” sub-tab, “Client Scopes” tab has “Evaluate” sub-tab. It might come in handy when you need to figure out effective protocol mappers, effective role scope mappings, the content of access, and id tokens.


Add valid redirect urls: http://localhost:5278/*


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


## App setup


```
dotnet add package Keycloak.AuthServices.Authentication
```
