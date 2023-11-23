using Contracts;

namespace OcelotGateway.Hateoas.Configurations.TestApi;

internal class PersonalDataDtoConfiguration: IHateoasLinksConfiguration<PersonalDataDto>
{
    public void Configure(HateoasLinksBuilder<PersonalDataDto> linksBuilder)
    {
        linksBuilder.SelfLink(RoutesNames.GetPersonalData);

        linksBuilder.AddLink($"{HateoasLinkConsts.Self}:no-permission", RoutesNames.GetNoPermission);
        linksBuilder.AddLink($"{HateoasLinkConsts.Self}:driver-permission", RoutesNames.GetDriverPermission);
        linksBuilder.AddLink($"{HateoasLinkConsts.Self}:future-data", RoutesNames.GetFutureData);
    }
}