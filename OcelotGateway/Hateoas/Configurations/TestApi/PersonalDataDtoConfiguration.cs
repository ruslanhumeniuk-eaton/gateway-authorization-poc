namespace OcelotGateway.Hateoas.Configurations.TestApi;

internal class PersonalDataDtoConfiguration: IHateoasLinksConfiguration
{
    public NestedConfigurationsDictionary? NestedObjectsConfigurations { get; set; }
    public void Configure(HateoasLinksBuilder linksBuilder)
    {
        linksBuilder.SelfLink(RoutesNames.GetPersonalDataByContract).Replace("contractId", x => x.Current.Value.GetRawText());

        linksBuilder.AddLink($"{HateoasLinkConsts.Self}:no-permission", RoutesNames.GetNoPermission);
        linksBuilder.AddLink($"{HateoasLinkConsts.Self}:driver-permission", RoutesNames.GetDriverPermission);
        linksBuilder.AddLink($"{HateoasLinkConsts.Self}:future-data", RoutesNames.GetFutureData);
    }
}