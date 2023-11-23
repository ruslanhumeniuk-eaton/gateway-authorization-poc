using Contracts;

namespace OcelotGateway.Hateoas.Configurations.TestApi;

internal class ContractMetaConfiguration: IHateoasLinksConfiguration<ContractMeta>
{
    public void Configure(HateoasLinksBuilder<ContractMeta> linksBuilder)
    {
        linksBuilder.SelfLink(RoutesNames.GetContract);
    }
}