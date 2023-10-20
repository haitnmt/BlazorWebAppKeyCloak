namespace BlazorShared.Authorization;

public class UserInfo
{
    public static readonly UserInfo Anonymous = new UserInfo();

    public bool IsAuthenticated { get; set; }

    public string NameClaimType { get; set; } = string.Empty;

    public string RoleClaimType { get; set; } = string.Empty;

    public ICollection<ClaimValue>? Claims { get; set; }
}
