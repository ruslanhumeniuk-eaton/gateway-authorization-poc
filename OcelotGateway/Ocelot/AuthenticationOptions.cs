namespace OcelotGateway.Ocelot;

public class AuthenticationOptions
{
    public string AuthenticationProviderKey { get; set; }
    public List<string> AllowedScopes { get; set; }
}