using Contracts.Application;
using NSubstitute;
using OcelotGateway.Hateoas;
using Shared;
using Shared.Encryption;
using System.Text.Json;
using System.Text.Json.Serialization;
using OcelotGateway.Hateoas.Configurations;

namespace OcelotGateway.Tests.Hateoas;

/// <summary>
///     Test HATEOAS links configuration for resource DTO.
/// </summary>
public class ResourceHateoasLinksConfigurationTests
{
    private static JsonSerializerOptions _serializerOptions = new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    /// <summary>
    ///     Assert that a self link configured without place holder builds a templated self link.
    /// </summary>
    /// <returns>Async task.</returns>
    [Fact]
    public async Task ConfigurationSelfLinkOnly_WithoutPlaceHolderValue_BuildsTemplatedSelfLink()
    {
        // Arrange
        var builder = PrepareAndGetLinksBuilder<ResourceSelfLinkHateoasLinksConfiguration>();
        var serialized = JsonSerializer.Serialize(new FakeResourceDto(), _serializerOptions);
        var deserialized = JsonSerializer.Deserialize<JsonElement>(serialized, JsonSerializerOptions.Default)
            .EnumerateObject();

        // Act
        IDictionary<string, Link> linkDictionary = await builder.BuildLinksAsync(deserialized);

        // Assert
        Assert.Equal(1, linkDictionary.Count);
        Link selfLink = Assert.Contains(HateoasLinkConsts.Self, linkDictionary);
        Assert.Equal("GET", selfLink.Method);
        Assert.Equal("/get-contract/{contractId}",
            selfLink.Href);
        Assert.True(selfLink.Templated);
    }

    /// <summary>
    ///     Assert that a self link configured with a resource place holder builds a self link with encrypted corresponding
    ///     value.
    /// </summary>
    /// <returns>Async task.</returns>
    [Fact]
    public async Task ConfigurationSelfLinkOnly_WithResourcePlaceHolderValue_BuildsSelfLinkWithEncryptedValue()
    {
        // Arrange
        var builder = PrepareAndGetLinksBuilder<ResourceSelfLinkHateoasLinksConfiguration>();
        var serialized =
            JsonSerializer.Serialize(
                new FakeResourceDto
                    { ContractId = Guid.Parse("015caaae-90c9-4a3c-ba04-b1a6ae01872b") },
                _serializerOptions);
        var deserialized = JsonSerializer.Deserialize<JsonElement>(serialized, JsonSerializerOptions.Default)
            .EnumerateObject();

        // Act
        IDictionary<string, Link> linkDictionary = await builder.BuildLinksAsync(deserialized);

        // Assert
        Assert.Equal(1, linkDictionary.Count);
        Link selfLink = Assert.Contains(HateoasLinkConsts.Self, linkDictionary);
        Assert.Equal("GET", selfLink.Method);
        Assert.Equal("/get-contract/4ojvJogGlT1MtjFAUyDFwgC2X-YZD4_C2cwPlDoJ6LyY74OkdRO_G0vS-f26Ve9F",
            selfLink.Href);
        Assert.False(selfLink.Templated);
    }

    /// <summary>
    ///     Assert that a self link configured with a place holder builds a self link with corresponding value.
    /// </summary>
    /// <returns>Async task.</returns>
    [Fact]
    public async Task
        ConfigurationSelfLinkOnlyWithPlaceHolder_WithResourcePlaceHolderValue_BuildsSelfLinkWithConfiguredPlaceHolder()
    {
        // Arrange
        var builder = PrepareAndGetLinksBuilder<ResourceSelfLinkWithPlaceHolderHateoasLinksConfiguration>();
        var serialized =
            JsonSerializer.Serialize(
                new FakeResourceDto
                    { ContractId = Guid.Parse("015caaae-90c9-4a3c-ba04-b1a6ae01872b") },
                _serializerOptions);
        var deserialized = JsonSerializer.Deserialize<JsonElement>(serialized, JsonSerializerOptions.Default)
            .EnumerateObject();

        // Act
        IDictionary<string, Link> linkDictionary = await builder.BuildLinksAsync(deserialized);

        // Assert
        Assert.Equal(1, linkDictionary.Count);
        Link selfLink = Assert.Contains(HateoasLinkConsts.Self, linkDictionary);
        Assert.Equal("GET", selfLink.Method);
        Assert.Equal("/get-contract/Lorem",
            selfLink.Href);
        Assert.False(selfLink.Templated);
    }

    /// <summary>
    ///     Assert that an additional link without place holder builds additional link with templated link.
    /// </summary>
    /// <returns>Async task.</returns>
    [Fact]
    public async Task ConfigurationAdditionalLinkOnly_WithoutPlaceHolderValue_BuildsTemplatedAdditionalLink()
    {
        // Arrange
        var builder = PrepareAndGetLinksBuilder<ResourceAdditionalLinkHateoasLinksConfiguration>();
        var serialized = JsonSerializer.Serialize(new FakeResourceDto(), _serializerOptions);
        var deserialized = JsonSerializer.Deserialize<JsonElement>(serialized, JsonSerializerOptions.Default)
            .EnumerateObject();

        // Act
        IDictionary<string, Link> linkDictionary = await builder.BuildLinksAsync(deserialized);

        // Assert
        Assert.Equal(1, linkDictionary.Count);
        Link childCollectionLink = Assert.Contains(ResourceAdditionalLinkHateoasLinksConfiguration.ChildrenCollection,
            linkDictionary);
        Assert.Equal("GET", childCollectionLink.Method);
        Assert.Equal(
            "/get-personal-data-by-contract/{contractId}?page={page}&pageSize={pageSize}&sortBy={sortBy}&sortOrder={sortOrder}&filterBy={filterBy}&filterCriteria={filterCriteria}",
            childCollectionLink.Href);
        Assert.True(childCollectionLink.Templated);
    }

    /// <summary>
    ///     Assert that an additional link with resource place holder builds additional link with link with encrypted
    ///     corresponding value.
    /// </summary>
    /// <returns>Async task.</returns>
    [Fact]
    public async Task
        ConfigurationAdditionalLinkOnly_WithResourcePlaceHolderValue_BuildsAdditionalLinkWithEncryptedValue()
    {
        // Arrange
        var builder = PrepareAndGetLinksBuilder<ResourceAdditionalLinkHateoasLinksConfiguration>();
        var serialized =
            JsonSerializer.Serialize(
                new FakeResourceDto
                    { ContractId = Guid.Parse("015caaae-90c9-4a3c-ba04-b1a6ae01872b") },
                _serializerOptions);
        var deserialized = JsonSerializer.Deserialize<JsonElement>(serialized, JsonSerializerOptions.Default)
            .EnumerateObject();

        // Act
        IDictionary<string, Link> linkDictionary = await builder.BuildLinksAsync(deserialized);

        // Assert
        Assert.Equal(1, linkDictionary.Count);
        Link childCollectionLink = Assert.Contains(ResourceAdditionalLinkHateoasLinksConfiguration.ChildrenCollection,
            linkDictionary);
        Assert.Equal("GET", childCollectionLink.Method);
        Assert.Equal(
            "/get-personal-data-by-contract/4ojvJogGlT1MtjFAUyDFwgC2X-YZD4_C2cwPlDoJ6LyY74OkdRO_G0vS-f26Ve9F?page={page}&pageSize={pageSize}&sortBy={sortBy}&sortOrder={sortOrder}&filterBy={filterBy}&filterCriteria={filterCriteria}",
            childCollectionLink.Href);
        Assert.True(childCollectionLink.Templated);
    }

    /// <summary>
    ///     Assert that an additional link configured with place holder builds additional link with link with configured
    ///     corresponding value.
    /// </summary>
    /// <returns>Async task.</returns>
    [Fact]
    public async Task
        ConfigurationAdditionalLinkOnlyWithPlaceHolder_WithResourcePlaceHolderValue_BuildsAdditionalLinkWithConfiguredPlaceHolder()
    {
        // Arrange
        var builder = PrepareAndGetLinksBuilder<ResourceAdditionalLinkWithPlaceHolderHateoasLinksConfiguration>();
        var serialized =
            JsonSerializer.Serialize(
                new FakeResourceDto
                    { ContractId = Guid.Parse("015caaae-90c9-4a3c-ba04-b1a6ae01872b") },
                _serializerOptions);
        var deserialized = JsonSerializer.Deserialize<JsonElement>(serialized, JsonSerializerOptions.Default)
            .EnumerateObject();

        // Act
        IDictionary<string, Link> linkDictionary = await builder.BuildLinksAsync(deserialized);

        // Assert
        Assert.Equal(1, linkDictionary.Count);
        Link childCollectionLink = Assert.Contains(ResourceAdditionalLinkHateoasLinksConfiguration.ChildrenCollection,
            linkDictionary);
        Assert.Equal("GET", childCollectionLink.Method);
        Assert.Equal(
            "/get-personal-data-by-contract/Lorem?page={page}&pageSize={pageSize}&sortBy={sortBy}&sortOrder={sortOrder}&filterBy={filterBy}&filterCriteria={filterCriteria}",
            childCollectionLink.Href);
        Assert.True(childCollectionLink.Templated);
    }

    /// <summary>
    ///     Assert that an additional link configured with a condition builds no link if the condition is not met.
    /// </summary>
    /// <returns>Async task.</returns>
    [Fact]
    public async Task ConfigurationAdditionalLinkOnly_WithNotMetCondition_BuildsNoLink()
    {
        // Arrange
        var builder = PrepareAndGetLinksBuilder<ResourceAdditionalLinkWithConditionHateoasLinksConfiguration>();
        var serialized =
            JsonSerializer.Serialize(
                new FakeResourceDto
                    { HasChildren = false },
                _serializerOptions);
        var deserialized = JsonSerializer.Deserialize<JsonElement>(serialized, JsonSerializerOptions.Default)
            .EnumerateObject();

        // Act
        IDictionary<string, Link> linkDictionary = await builder.BuildLinksAsync(deserialized);

        // Assert
        Assert.Empty(linkDictionary);
    }

    /// <summary>
    ///     Assert that an additional link configured with a condition builds the link if the condition is met.
    /// </summary>
    /// <returns>Async task.</returns>
    [Fact]
    public async Task ConfigurationAdditionalLinkOnly_WithMetCondition_BuildsLink()
    {
        // Arrange
        var builder = PrepareAndGetLinksBuilder<ResourceAdditionalLinkWithConditionHateoasLinksConfiguration>();
        var serialized =
            JsonSerializer.Serialize(
                new FakeResourceDto
                    { HasChildren = true },
                _serializerOptions);
        var deserialized = JsonSerializer.Deserialize<JsonElement>(serialized, JsonSerializerOptions.Default)
            .EnumerateObject();

        // Act
        IDictionary<string, Link> linkDictionary = await builder.BuildLinksAsync(deserialized);

        // Assert
        Assert.Equal(1, linkDictionary.Count);
        Link childCollectionLink = Assert.Contains(ResourceAdditionalLinkHateoasLinksConfiguration.ChildrenCollection,
            linkDictionary);
        Assert.Equal("GET", childCollectionLink.Method);
        Assert.Equal(
            "/get-personal-data-by-contract/{contractId}?page={page}&pageSize={pageSize}&sortBy={sortBy}&sortOrder={sortOrder}&filterBy={filterBy}&filterCriteria={filterCriteria}",
            childCollectionLink.Href);
        Assert.True(childCollectionLink.Templated);
    }

    private HateoasLinksBuilder PrepareAndGetLinksBuilder<T>() where T : class, IHateoasLinksConfiguration, new()
    {
        var routeCollection = new FakeRouteCollection();
        var configuration = new T();
        var encryptor = new Encryptor("QDvif2EV3nYn1l6ZuiUUH9SFUbBSW5wUxAawbH9OAig=", "JDGM4Km1U95JSRJR5CfMLQ==");
        var authorizationService = Substitute.For<IAuthorizationService>();
        var serviceProvider = Substitute.For<IServiceProvider>();
        serviceProvider.GetService(typeof(IAuthorizationService)).Returns(authorizationService);
        return new HateoasLinksBuilder(configuration, encryptor, routeCollection, serviceProvider);
    }

    private class FakeResourceDto
    {
        public Guid? ContractId { get; set; }
        public bool HasChildren { get; set; }
    }

    private class ResourceSelfLinkHateoasLinksConfiguration : IHateoasLinksConfiguration
    {
        public NestedConfigurationsDictionary? NestedObjectsConfigurations { get; set; }

        public void Configure(HateoasLinksBuilder linksBuilder)
        {
            linksBuilder.SelfLink(RoutesNames.GetContract);
        }
    }

    private class ResourceSelfLinkWithPlaceHolderHateoasLinksConfiguration : IHateoasLinksConfiguration
    {
        public NestedConfigurationsDictionary? NestedObjectsConfigurations { get; set; }

        public void Configure(HateoasLinksBuilder linksBuilder)
        {
            linksBuilder.SelfLink(RoutesNames.GetContract).Replace("contractId", x => "Lorem");
        }
    }

    private class ResourceAdditionalLinkHateoasLinksConfiguration : IHateoasLinksConfiguration
    {
        public const string ChildrenCollection = HateoasLinkConsts.Collection + ":children";
        public NestedConfigurationsDictionary? NestedObjectsConfigurations { get; set; }

        public void Configure(HateoasLinksBuilder linksBuilder)
        {
            linksBuilder.AddLink(ChildrenCollection, RoutesNames.GetPersonalDataByContract);
        }
    }

    private class ResourceAdditionalLinkWithPlaceHolderHateoasLinksConfiguration : IHateoasLinksConfiguration
    {
        public const string ChildrenCollection = HateoasLinkConsts.Collection + ":children";
        public NestedConfigurationsDictionary? NestedObjectsConfigurations { get; set; }

        public void Configure(HateoasLinksBuilder linksBuilder)
        {
            linksBuilder.AddLink(ChildrenCollection, RoutesNames.GetPersonalDataByContract)
                .Replace("contractId", x => "Lorem");
        }
    }

    private class ResourceAdditionalLinkWithConditionHateoasLinksConfiguration : IHateoasLinksConfiguration
    {
        public const string ChildrenCollection = HateoasLinkConsts.Collection + ":children";
        public NestedConfigurationsDictionary? NestedObjectsConfigurations { get; set; }

        public void Configure(HateoasLinksBuilder linksBuilder)
        {
            linksBuilder.AddLink(ChildrenCollection, RoutesNames.GetPersonalDataByContract,
                x => x.Any(y =>
                    string.Equals(y.Name, nameof(FakeResourceDto.HasChildren),
                        StringComparison.InvariantCultureIgnoreCase)
                    && y.Value.GetBoolean()));
        }
    }
}