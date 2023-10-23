using BlazorBff.Helpers;

namespace BlazorBff.Tests.Helpers;

[TestFixture(Category = "Unit")]
public class JwtRolesHelperTest
{
    private const string _jwtRawString1 = @"eyJhbGciOiJSUzI1NiIsInR5cCIgOiAiSldUIiwia2lkIiA6ICJQeFcwXzdaTXhtZXVfY3dYenFlNy1EcU50R0pMOTBsQ0xfNkFMQjd0NWhzIn0.eyJleHAiOjE2OTc4MjI1MjcsImlhdCI6MTY5NzgyMjIyNywiYXV0aF90aW1lIjoxNjk3ODIyMjI3LCJqdGkiOiI1YTc5MzQ4Ny1iZjNmLTQ5ZjItOGNjMy03YWE2ZmQwMzJkMTgiLCJpc3MiOiJodHRwOi8vbG9jYWxob3N0OjgwODAvcmVhbG1zL215cmVhbG0iLCJhdWQiOiJhY2NvdW50Iiwic3ViIjoiN2RiMzRkM2QtMmRkNS00OTI3LTkwZmYtZTQwOGFlZjk5ZTRmIiwidHlwIjoiQmVhcmVyIiwiYXpwIjoiYmxhem9yLWNsaWVudCIsIm5vbmNlIjoiNjM4MzM0MTkwMjAzMjA4MTQ0LlpUUXlaalkxTlRVdE1ESm1PUzAwTnpreExUZzNNemN0TTJFNFpEZ3lORFEwWWpRME9HUXhObUUxTkRFdE5XSTFZUzAwWTJWaExXRm1Zamt0TkdJeE5tUXpZakF6WmpVMSIsInNlc3Npb25fc3RhdGUiOiJlMzlkOGU2NS0zMWVmLTRkNWMtYTk5YS03OGFkOTVhZjg3NjAiLCJhY3IiOiIxIiwiYWxsb3dlZC1vcmlnaW5zIjpbIioiXSwicmVhbG1fYWNjZXNzIjp7InJvbGVzIjpbImRlZmF1bHQtcm9sZXMtbXlyZWFsbSIsIm9mZmxpbmVfYWNjZXNzIiwidW1hX2F1dGhvcml6YXRpb24iLCJteXJvbGUiXX0sInJlc291cmNlX2FjY2VzcyI6eyJhY2NvdW50Ijp7InJvbGVzIjpbIm1hbmFnZS1hY2NvdW50IiwibWFuYWdlLWFjY291bnQtbGlua3MiLCJ2aWV3LXByb2ZpbGUiXX19LCJzY29wZSI6Im9wZW5pZCBlbWFpbCBvZmZsaW5lX2FjY2VzcyBwcm9maWxlIiwic2lkIjoiZTM5ZDhlNjUtMzFlZi00ZDVjLWE5OWEtNzhhZDk1YWY4NzYwIiwiZW1haWxfdmVyaWZpZWQiOnRydWUsIm5hbWUiOiJBbmF0b2x5IEZlZHlhbmluIiwicHJlZmVycmVkX3VzZXJuYW1lIjoiYWZlZHlhbmluIiwiZ2l2ZW5fbmFtZSI6IkFuYXRvbHkiLCJmYW1pbHlfbmFtZSI6IkZlZHlhbmluIiwiZW1haWwiOiJhbmZlbWFpbEBnbWFpbC5jb20ifQ.V6bkdRqBBbfQk2e-wugqW404L16GecCBgnfTISp_RgZ4ZPD-A519oX4kV70U4H5UG-r-wsAL42_t6maKOc-BdZEaYgqJD5TQGBFtWdKlpRzCLzxtrNx5ujZzYxdQdJYfV0q-P80CDl6tlm-tjRtK0hYlFaBWrCrPxEVLTD79SqiHIj-FecnXoqK1rzJiGIXq8C97zsk_rXnWyHeh9hu1bxJq7TVKdy3fFG3Kg1mR5fAAP64-MYOg6X8smTBGUS05ffVi-Sh_oqMOTZLd5n2Wqa4FKA7i4XOjlFLDsEzRzzoqG8ifxzrpTWhxxrQ-HTatD0fnr91N_B4IJXjl_WX7xA";
    private const string _jwtRawString2 = @"eyJhbGciOiJSUzI1NiIsInR5cCIgOiAiSldUIiwia2lkIiA6ICJrTnJTbFNaOGpHdWNvOUd0elZ2RTltZEs3NWFfbUU2ZmxDcG1mZGVJSHRZIn0.eyJleHAiOjE2OTgwNDgxOTAsImlhdCI6MTY5ODA0Nzg5MCwiYXV0aF90aW1lIjoxNjk4MDQ3ODkwLCJqdGkiOiJmMmYyN2NlYi1lNmE2LTQ1NTYtODJiYy1kY2ZhMzFhMzU1N2IiLCJpc3MiOiJodHRwOi8vbG9jYWxob3N0OjgwODAvcmVhbG1zL215cmVhbG0iLCJhdWQiOiJhY2NvdW50Iiwic3ViIjoiYzE2NzZmZGQtZjc1Yi00M2MyLWE0NzAtNzFkN2Y5N2ZhZDEwIiwidHlwIjoiQmVhcmVyIiwiYXpwIjoiYmxhem9yLWNsaWVudCIsIm5vbmNlIjoiNjM4MzM2NDQ2ODMwOTEyNTIwLllqSXlNR015WkRZdFptSTNNeTAwWmpKakxUazNOREV0WXpkaU5HWTRNekl6WVdZMFpEaGtZMlV5TkRjdFkySXlOUzAwTWpjMkxUaGtaV010Wm1JMlpXRmlNamRtTldReiIsInNlc3Npb25fc3RhdGUiOiI5ZWEyNmIyNS02MGJjLTQ2NDUtYWZkMC1lYTM3NjZlZWExN2YiLCJhY3IiOiIxIiwiYWxsb3dlZC1vcmlnaW5zIjpbIioiXSwicmVhbG1fYWNjZXNzIjp7InJvbGVzIjpbImRlZmF1bHQtcm9sZXMtbXlyZWFsbSIsIm9mZmxpbmVfYWNjZXNzIiwidW1hX2F1dGhvcml6YXRpb24iLCJteXJvbGUiXX0sInJlc291cmNlX2FjY2VzcyI6eyJibGF6b3ItY2xpZW50Ijp7InJvbGVzIjpbInJvbGUxIiwicm9sZTIiXX0sImFjY291bnQiOnsicm9sZXMiOlsibWFuYWdlLWFjY291bnQiLCJtYW5hZ2UtYWNjb3VudC1saW5rcyIsInZpZXctcHJvZmlsZSJdfX0sInNjb3BlIjoib3BlbmlkIHByb2ZpbGUgZW1haWwgb2ZmbGluZV9hY2Nlc3MiLCJzaWQiOiI5ZWEyNmIyNS02MGJjLTQ2NDUtYWZkMC1lYTM3NjZlZWExN2YiLCJlbWFpbF92ZXJpZmllZCI6dHJ1ZSwibmFtZSI6IkFuYXRvbHkgRmVkeWFuaW4iLCJwcmVmZXJyZWRfdXNlcm5hbWUiOiJhZmVkeWFuaW4iLCJnaXZlbl9uYW1lIjoiQW5hdG9seSIsImZhbWlseV9uYW1lIjoiRmVkeWFuaW4iLCJlbWFpbCI6ImFuZmVtYWlsQGdtYWlsLmNvbSJ9.U7rOCk4GCHchQ2PC_xcwkfdhQgEqcC8-14NHrtYhK0jhOw2BBFqkCLC-V_8XVGbNaJYlILNxnn5jTRUc1bJ1dzgjc825nxj_3q0AoQGwfhNN3yqLklV4kUKiYH1aR_4sbw_noRaONvpT-PGqknDjps2IesOwy8r1CJf036W5c-6Wt96WDQZJjoln_bs6y95vrx5XjkbDm0O659bBpKbOf4_epNWkfqbsBp6X-zqprXXTJHlWDY-Q4md4Yma9JxfXDdUbV2lppDHgrPXVBRVuUU8TpqU9Z8881UOGkjNiSl87I6hGGWl4FR4Gmh5mySJMjlpdyNqI7-7OYjp_gfZL3w";

    [Test]
    public void CanGetRolesWithoutRealm()
    {
        var roles = JwtRolesHelper.ExtractRoles(_jwtRawString1, new string[] { "account" });
        Assert.That(roles, Is.Not.Null);
        Assert.That(roles, Has.Length.EqualTo(3));
        Assert.That(roles, Has.Member("manage-account"));
        Assert.That(roles, Has.Member("manage-account-links"));
        Assert.That(roles, Has.Member("view-profile"));
    }

    [Test]
    public void CanGetRolesWithRealm()
    {
        var roles = JwtRolesHelper.ExtractRoles(_jwtRawString1, new string[] { "account" }, true);
        Assert.That(roles, Is.Not.Null);
        Assert.That(roles, Has.Length.EqualTo(7));
        Assert.That(roles, Has.Member("default-roles-myrealm"));
        Assert.That(roles, Has.Member("offline_access"));
        Assert.That(roles, Has.Member("uma_authorization"));
        Assert.That(roles, Has.Member("myrole"));
    }

    [Test]
    public void CanGetRolesWithSeveralResources()
    {
        var roles = JwtRolesHelper.ExtractRoles(_jwtRawString2, new string[] { "account", "blazor-client" });
        Assert.That(roles, Is.Not.Null);
        Assert.That(roles, Has.Length.EqualTo(5));
        Assert.That(roles, Has.Member("manage-account"));
        Assert.That(roles, Has.Member("manage-account-links"));
        Assert.That(roles, Has.Member("view-profile"));
        Assert.That(roles, Has.Member("role1"));
        Assert.That(roles, Has.Member("role2"));
    }
}
