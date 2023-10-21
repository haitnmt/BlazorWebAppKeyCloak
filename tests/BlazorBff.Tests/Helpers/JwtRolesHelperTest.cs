using BlazorBff.Helpers;

namespace BlazorBff.Tests.Helpers;

[TestFixture(Category = "Unit")]
public class JwtRolesHelperTest
{
    private const string _jwtRawString = @"eyJhbGciOiJSUzI1NiIsInR5cCIgOiAiSldUIiwia2lkIiA6ICJQeFcwXzdaTXhtZXVfY3dYenFlNy1EcU50R0pMOTBsQ0xfNkFMQjd0NWhzIn0.eyJleHAiOjE2OTc4MjI1MjcsImlhdCI6MTY5NzgyMjIyNywiYXV0aF90aW1lIjoxNjk3ODIyMjI3LCJqdGkiOiI1YTc5MzQ4Ny1iZjNmLTQ5ZjItOGNjMy03YWE2ZmQwMzJkMTgiLCJpc3MiOiJodHRwOi8vbG9jYWxob3N0OjgwODAvcmVhbG1zL215cmVhbG0iLCJhdWQiOiJhY2NvdW50Iiwic3ViIjoiN2RiMzRkM2QtMmRkNS00OTI3LTkwZmYtZTQwOGFlZjk5ZTRmIiwidHlwIjoiQmVhcmVyIiwiYXpwIjoiYmxhem9yLWNsaWVudCIsIm5vbmNlIjoiNjM4MzM0MTkwMjAzMjA4MTQ0LlpUUXlaalkxTlRVdE1ESm1PUzAwTnpreExUZzNNemN0TTJFNFpEZ3lORFEwWWpRME9HUXhObUUxTkRFdE5XSTFZUzAwWTJWaExXRm1Zamt0TkdJeE5tUXpZakF6WmpVMSIsInNlc3Npb25fc3RhdGUiOiJlMzlkOGU2NS0zMWVmLTRkNWMtYTk5YS03OGFkOTVhZjg3NjAiLCJhY3IiOiIxIiwiYWxsb3dlZC1vcmlnaW5zIjpbIioiXSwicmVhbG1fYWNjZXNzIjp7InJvbGVzIjpbImRlZmF1bHQtcm9sZXMtbXlyZWFsbSIsIm9mZmxpbmVfYWNjZXNzIiwidW1hX2F1dGhvcml6YXRpb24iLCJteXJvbGUiXX0sInJlc291cmNlX2FjY2VzcyI6eyJhY2NvdW50Ijp7InJvbGVzIjpbIm1hbmFnZS1hY2NvdW50IiwibWFuYWdlLWFjY291bnQtbGlua3MiLCJ2aWV3LXByb2ZpbGUiXX19LCJzY29wZSI6Im9wZW5pZCBlbWFpbCBvZmZsaW5lX2FjY2VzcyBwcm9maWxlIiwic2lkIjoiZTM5ZDhlNjUtMzFlZi00ZDVjLWE5OWEtNzhhZDk1YWY4NzYwIiwiZW1haWxfdmVyaWZpZWQiOnRydWUsIm5hbWUiOiJBbmF0b2x5IEZlZHlhbmluIiwicHJlZmVycmVkX3VzZXJuYW1lIjoiYWZlZHlhbmluIiwiZ2l2ZW5fbmFtZSI6IkFuYXRvbHkiLCJmYW1pbHlfbmFtZSI6IkZlZHlhbmluIiwiZW1haWwiOiJhbmZlbWFpbEBnbWFpbC5jb20ifQ.V6bkdRqBBbfQk2e-wugqW404L16GecCBgnfTISp_RgZ4ZPD-A519oX4kV70U4H5UG-r-wsAL42_t6maKOc-BdZEaYgqJD5TQGBFtWdKlpRzCLzxtrNx5ujZzYxdQdJYfV0q-P80CDl6tlm-tjRtK0hYlFaBWrCrPxEVLTD79SqiHIj-FecnXoqK1rzJiGIXq8C97zsk_rXnWyHeh9hu1bxJq7TVKdy3fFG3Kg1mR5fAAP64-MYOg6X8smTBGUS05ffVi-Sh_oqMOTZLd5n2Wqa4FKA7i4XOjlFLDsEzRzzoqG8ifxzrpTWhxxrQ-HTatD0fnr91N_B4IJXjl_WX7xA";

    [Test]
    public void CanGetRolesWithoutRealm()
    {
        var roles = JwtRolesHelper.ExtractRoles(_jwtRawString);
        Assert.That(roles, Is.Not.Null);
        Assert.That(roles, Has.Length.EqualTo(3));
        Assert.That(roles, Has.Member("manage-account"));
        Assert.That(roles, Has.Member("manage-account-links"));
        Assert.That(roles, Has.Member("view-profile"));
    }

    [Test]
    public void CanGetRolesWithRealm()
    {
        var roles = JwtRolesHelper.ExtractRoles(_jwtRawString, true);
        Assert.That(roles, Is.Not.Null);
        Assert.That(roles, Has.Length.EqualTo(7));
        Assert.That(roles, Has.Member("default-roles-myrealm"));
        Assert.That(roles, Has.Member("offline_access"));
        Assert.That(roles, Has.Member("uma_authorization"));
        Assert.That(roles, Has.Member("myrole"));
    }
}
