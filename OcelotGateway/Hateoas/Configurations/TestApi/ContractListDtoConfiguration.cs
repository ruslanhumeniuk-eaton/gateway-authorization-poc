namespace OcelotGateway.Hateoas.Configurations.TestApi;

internal class ContractDtoListConfiguration : IHateoasLinksConfiguration
{
    public NestedConfigurationsDictionary NestedObjectsConfigurations { get; set; } =
        new(new Dictionary<string, Type>
        {
            { "contracts", typeof(ContractDtoConfiguration) }
        });

    public void Configure(HateoasLinksBuilder linksBuilder)
    {
        linksBuilder.SelfLink(RoutesNames.GetContracts);
    }
}