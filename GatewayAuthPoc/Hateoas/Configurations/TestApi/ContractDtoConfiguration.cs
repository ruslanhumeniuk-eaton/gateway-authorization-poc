using Contracts;

namespace OcelotGateway.Hateoas.Configurations.TestApi;

internal class ContractDtoConfiguration: IHateoasLinksConfiguration<ContractDto>
{
    public void Configure(HateoasLinksBuilder<ContractDto> linksBuilder)
    {
        linksBuilder.SelfLink(RoutesNames.GetContract);
        linksBuilder.AddLink(HateoasLinkConsts.SelfEdit, RoutesNames.UpdateContract);
        linksBuilder.AddLink(HateoasLinkConsts.SelfDelete, RoutesNames.DeleteContract);

        linksBuilder.AddLink($"{HateoasLinkConsts.Self}:contracts", RoutesNames.GetContracts);
        linksBuilder.AddLink("get:personal-data", RoutesNames.GetPersonalData);
    }
}