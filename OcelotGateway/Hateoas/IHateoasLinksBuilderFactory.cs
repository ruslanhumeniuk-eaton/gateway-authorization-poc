namespace OcelotGateway.Hateoas;

public interface IHateoasLinksBuilderFactory
{
    internal HateoasLinksBuilder ConstructLinksBuilder(Type configurationType);
}