namespace OcelotGateway.Ocelot;

public class OcelotRouteConfiguration
{
    public string UpstreamPathTemplate { get; set; }

    public string PermissionKey { get; set; }

    //public string DownstreamPathTemplate { get; set; }
    //public string DownstreamScheme { get; set; }
    //public List<HostAndPortConfig> DownstreamHostAndPorts { get; set; }
    //public List<string> UpstreamHttpMethod { get; set; }
    //public AuthenticationOptions AuthenticationOptions { get; set; }
    //public Dictionary<string, string> RouteClaimsRequirement { get; set; }
}