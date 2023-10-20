using BlazorShared;
using Microsoft.AspNetCore.Mvc;

namespace BlazorBff.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    [HttpGet]
    public UserInfo GetUser()
    {
        var claims = new List<ClaimInfo>();
        foreach (var claim in HttpContext.User.Claims)
        {
            claims.Add(new ClaimInfo(claim.Type, claim.Value));
        }

        var res = new UserInfo(claims.ToArray());
        return res;
    }
}
