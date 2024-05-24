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


