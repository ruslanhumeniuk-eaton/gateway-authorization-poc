using Contracts;

namespace OcelotGateway.Hateoas.Configurations.TestApi;

internal class PersonalDataMetaConfiguration: IHateoasLinksConfiguration<PersonalDataMeta>
{
    public void Configure(HateoasLinksBuilder<PersonalDataMeta> linksBuilder)
    {
        linksBuilder.SelfLink(RoutesNames.GetPersonalData);
    }
}