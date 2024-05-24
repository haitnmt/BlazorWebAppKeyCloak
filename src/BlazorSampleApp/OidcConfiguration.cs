namespace BlazorSampleApp;

public class OidcConfiguration
{
    public static readonly string OidcSection = "OpenIDConnectSettings";

    public string Authority { get; set; } = string.Empty;

    public string MetadataUrl { get; set; } = string.Empty;

    public string TokenEndpoint { get; set; } = string.Empty;

    public string ClientId { get; set; } = string.Empty;

    public string ClientSecret { get; set; } = string.Empty;

    public string CallbackPath { get; set; } = string.Empty;

    public string SignedOutCallbackPath { get; set; } = string.Empty;
}
