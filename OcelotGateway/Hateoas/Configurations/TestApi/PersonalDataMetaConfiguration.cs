namespace OcelotGateway.Hateoas.Configurations.TestApi;

internal class PersonalDataMetaConfiguration: IHateoasLinksConfiguration
{
    public NestedConfigurationsDictionary? NestedObjectsConfigurations { get; set; }
    public void Configure(HateoasLinksBuilder linksBuilder)
    {
        linksBuilder.SelfLink(RoutesNames.GetPersonalData);
    }
}