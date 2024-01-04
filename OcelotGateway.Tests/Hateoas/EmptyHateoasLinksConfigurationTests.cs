using Contracts.Application;
using NSubstitute;
using OcelotGateway.Hateoas;
using Shared;
using Shared.Encryption;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OcelotGateway.Tests.Hateoas;

/// <summary>
///     Assert that empty configuration return an empty links dictionary.
/// </summary>
public class EmptyHateoasLinksConfigurationTests
{
    private static JsonSerializerOptions _serializerOptions = new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    /// <summary>
    ///     Assert that an empty resource configuration return an empty links dictionary.
    /// </summary>
    /// <returns>Async task</returns>
    [Fact]
    public async Task EmptyResourceConfiguration_BuildsNoLinks()
    {
        // Arrange
        var builder = PrepareAndGetLinksBuilder();
        var serialized = JsonSerializer.Serialize(new FakeResourceDto(), _serializerOptions);
        var deserialized = JsonSerializer.Deserialize<JsonElement>(serialized, JsonSerializerOptions.Default)
            .EnumerateObject();

        // Act
        IDictionary<string, Link> linkDictionary = await builder.BuildLinksAsync(deserialized);

        // Assert
        Assert.Empty(linkDictionary);
    }

    /// <summary>
    ///     Assert that an empty resource collection configuration return an empty links dictionary.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task EmptyResourceCollectionConfiguration_BuildsNoLinks()
    {
        // Arrange
        var builder = PrepareAndGetLinksBuilder();
        var serialized = JsonSerializer.Serialize(new FakeResourceCollectionDto(), _serializerOptions);
        var deserialized = JsonSerializer.Deserialize<JsonElement>(serialized, JsonSerializerOptions.Default)
            .EnumerateObject();

        // Act
        IDictionary<string, Link> linkDictionary = await builder.BuildLinksAsync(deserialized);

        // Assert
        Assert.Empty(linkDictionary);
    }

    private HateoasLinksBuilder PrepareAndGetLinksBuilder()
    {
        var routeCollection = new FakeRouteCollection();
        var configuration = new EmptyResourceHateoasLinksConfiguration();
        var encryptor = new Encryptor(null, null);
        var authorizationService = Substitute.For<IAuthorizationService>();
        var serviceProvider = Substitute.For<IServiceProvider>();
        serviceProvider.GetService(typeof(IAuthorizationService)).Returns(authorizationService);
        return new HateoasLinksBuilder(configuration, encryptor, routeCollection, serviceProvider);
    }

    private class FakeResourceDto
    {
    }

    private class FakeResourceCollectionDto : ResourceCollectionDto
    {
    }

    private class EmptyResourceHateoasLinksConfiguration : IHateoasLinksConfiguration
    {
        public NestedConfigurationsDictionary? NestedObjectsConfigurations { get; set; }

        public void Configure(HateoasLinksBuilder linksBuilder)
        {
            // Empty
        }
    }
}