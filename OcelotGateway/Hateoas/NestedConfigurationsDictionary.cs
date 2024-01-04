namespace OcelotGateway.Hateoas;

public class NestedConfigurationsDictionary
{
    public NestedConfigurationsDictionary(IDictionary<string, Type> configurations)
    {
        foreach (var configuration in configurations)
        {
            AddType(configuration.Key, configuration.Value);
        }
    }

    public Dictionary<string, Type> ConfigurationsDictionary { get; } = new();

    private void AddType(string key, Type type)
    {
        if (typeof(IHateoasLinksConfiguration).IsAssignableFrom(type))
        {
            ConfigurationsDictionary[key] = type;
        }
        else
        {
            throw new ArgumentException("The provided type must implement IHateoasLinksConfiguration", nameof(type));
        }
    }
}