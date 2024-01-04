namespace OcelotGateway.Hateoas.Configurations.TestApi;

internal class ContractDtoConfiguration: IHateoasLinksConfiguration
{
    public NestedConfigurationsDictionary? NestedObjectsConfigurations { get; set; }

    public void Configure(HateoasLinksBuilder linksBuilder)
    {
        linksBuilder.SelfLink(RoutesNames.GetContract);
        linksBuilder.AddLink(HateoasLinkConsts.SelfEdit, RoutesNames.UpdateContract);
        linksBuilder.AddLink(HateoasLinkConsts.SelfDelete, RoutesNames.DeleteContract);

        linksBuilder.AddLink("get:personal-data", RoutesNames.GetPersonalData);
    }
}