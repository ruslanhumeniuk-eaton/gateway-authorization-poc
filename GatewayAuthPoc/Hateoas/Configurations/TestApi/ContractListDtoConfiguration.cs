using Contracts;

namespace OcelotGateway.Hateoas.Configurations.TestApi;

internal class ContractDtoListConfiguration: IHateoasLinksConfiguration<ContractListDto>
{
    public void Configure(HateoasLinksBuilder<ContractListDto> linksBuilder)
    {
        linksBuilder.SelfLink(RoutesNames.GetContracts);
    }
}