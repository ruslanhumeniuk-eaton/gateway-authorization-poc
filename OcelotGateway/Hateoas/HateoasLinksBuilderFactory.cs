namespace OcelotGateway.Hateoas;

public class HateoasLinksBuilderFactory : IHateoasLinksBuilderFactory
{
    private readonly IServiceProvider _serviceProvider;

    public HateoasLinksBuilderFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public HateoasLinksBuilder ConstructLinksBuilder(Type configurationType)
    {
        var configuration = Activator.CreateInstance(configurationType);
        ObjectFactory builderFactory =
            ActivatorUtilities.CreateFactory(typeof(HateoasLinksBuilder), new[] { configurationType });
        return (HateoasLinksBuilder)builderFactory(_serviceProvider, new[] { configuration });
    }
}