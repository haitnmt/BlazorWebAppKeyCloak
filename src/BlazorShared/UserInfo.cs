namespace BlazorShared;

public class UserInfo
{
    public ClaimInfo[] Claims { get; }

    public UserInfo(ClaimInfo[] claims)
    {
        Claims = claims ?? Array.Empty<ClaimInfo>();
    }
}

public record ClaimInfo(string Type, string Value);
