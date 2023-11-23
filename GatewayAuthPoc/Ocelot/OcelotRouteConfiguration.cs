namespace OcelotGateway.Ocelot;

public class OcelotRouteConfiguration
{
    public string Name { get; set; }

    public string UpstreamPathTemplate { get; set; }

    public string[] PermissionKeys { get; set; }

    public List<string> UpstreamHttpMethod { get; set; }

    public string DownstreamPathTemplate { get; set; }

    public AuthenticationOptions? AuthenticationOptions { get; set; }

    public string DtoType { get; set; }

    //public string DownstreamScheme { get; set; }
    //public List<HostAndPortConfig> DownstreamHostAndPorts { get; set; }    
    //public Dictionary<string, string> RouteClaimsRequirement { get; set; }
}