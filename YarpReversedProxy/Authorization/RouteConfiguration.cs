using System.Text.Json.Serialization;

namespace YarpReversedProxy.Authorization
{
    public class RouteConfiguration
    {
        public string Permission { get; set; }
        public MatchConfig Match { get; set; }

        [JsonIgnore]
        public string GetEndpointPath => Match.Path;
    }

    public class MatchConfig
    {
        public string Path { get; set; }
    }
}
