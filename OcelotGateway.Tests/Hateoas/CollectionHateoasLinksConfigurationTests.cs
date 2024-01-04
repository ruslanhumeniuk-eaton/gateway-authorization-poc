using System.Text.Json;
using System.Text.Json.Serialization;
using Contracts.Application;
using NSubstitute;
using OcelotGateway.Hateoas;
using OcelotGateway.Hateoas.Configurations;
using Shared;
using Shared.Encryption;

namespace OcelotGateway.Tests.Hateoas;

/// <summary>
///     Test HATEOAS links configuration for collection.
/// </summary>
public class CollectionHateoasLinksConfigurationTests
{
    private static JsonSerializerOptions _serializerOptions = new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    /// <summary>
    ///     Assert that an empty collection returns only the minimum collection links.
    /// </summary>
    /// <returns>Async task.</returns>
    [Fact]
    public async Task EmptyCollection_ReturnMinimumCollectionLinks()
    {
        // Arrange
        var builder = PrepareAndGetLinksBuilder();
        var serialized = JsonSerializer.Serialize(new FakeResourceCollectionDto(), _serializerOptions);
        var deserialized = JsonSerializer.Deserialize<JsonElement>(serialized, JsonSerializerOptions.Default)
            .EnumerateObject();

        // Act
        IDictionary<string, Link> linkDictionary = await builder.BuildLinksAsync(deserialized);

        // Assert
        Assert.Equal(4, linkDictionary.Count);

        Link collectionFirst = Assert.Contains($"{HateoasLinkConsts.Collection}:first", linkDictionary);
        Assert.Equal("GET", collectionFirst.Method);
        Assert.Equal(
            "/get-personal-data-by-contract/{contractId}?page=1&pageSize={pageSize}&sortBy={sortBy}&sortOrder={sortOrder}&filterBy={filterBy}&filterCriteria={filterCriteria}",
            collectionFirst.Href);
        Assert.True(collectionFirst.Templated);

        Link collectionPage = Assert.Contains($"{HateoasLinkConsts.Collection}:page", linkDictionary);
        Assert.Equal("GET", collectionPage.Method);
        Assert.Equal(
            "/get-personal-data-by-contract/{contractId}?page={page}&pageSize={pageSize}&sortBy={sortBy}&sortOrder={sortOrder}&filterBy={filterBy}&filterCriteria={filterCriteria}",
            collectionPage.Href);
        Assert.True(collectionPage.Templated);

        Link collectionSort = Assert.Contains($"{HateoasLinkConsts.Collection}:sort", linkDictionary);
        Assert.Equal("GET", collectionSort.Method);
        Assert.Equal(
            "/get-personal-data-by-contract/{contractId}?page=1&pageSize={pageSize}&sortBy={sortBy}&sortOrder={sortOrder}&filterBy={filterBy}&filterCriteria={filterCriteria}",
            collectionSort.Href);
        Assert.True(collectionSort.Templated);

        Link collectionFilter = Assert.Contains($"{HateoasLinkConsts.Collection}:filter", linkDictionary);
        Assert.Equal("GET", collectionFilter.Method);
        Assert.Equal(
            "/get-personal-data-by-contract/{contractId}?page=1&pageSize={pageSize}&sortBy={sortBy}&sortOrder={sortOrder}&filterBy={filterBy}&filterCriteria={filterCriteria}",
            collectionFilter.Href);
        Assert.True(collectionFilter.Templated);
    }

    /// <summary>
    ///     Assert that a collection without pagination returns only the minimum collection links.
    /// </summary>
    /// <returns>Async task.</returns>
    [Fact]
    public async Task CollectionWithoutPagination_ReturnMinimumCollectionLinks()
    {
        // Arrange
        var builder = PrepareAndGetLinksBuilder();
        var serialized = JsonSerializer.Serialize(new FakeResourceCollectionDto { Total = 125 }, _serializerOptions);
        var deserialized = JsonSerializer.Deserialize<JsonElement>(serialized, JsonSerializerOptions.Default)
            .EnumerateObject();

        // Act
        IDictionary<string, Link> linkDictionary = await builder.BuildLinksAsync(deserialized);

        // Assert
        Assert.Equal(4, linkDictionary.Count);

        Link collectionFirst = Assert.Contains($"{HateoasLinkConsts.Collection}:first", linkDictionary);
        Assert.Equal("GET", collectionFirst.Method);
        Assert.Equal(
            "/get-personal-data-by-contract/{contractId}?page=1&pageSize={pageSize}&sortBy={sortBy}&sortOrder={sortOrder}&filterBy={filterBy}&filterCriteria={filterCriteria}",
            collectionFirst.Href);
        Assert.True(collectionFirst.Templated);

        Link collectionPage = Assert.Contains($"{HateoasLinkConsts.Collection}:page", linkDictionary);
        Assert.Equal("GET", collectionPage.Method);
        Assert.Equal(
            "/get-personal-data-by-contract/{contractId}?page={page}&pageSize={pageSize}&sortBy={sortBy}&sortOrder={sortOrder}&filterBy={filterBy}&filterCriteria={filterCriteria}",
            collectionPage.Href);
        Assert.True(collectionPage.Templated);

        Link collectionSort = Assert.Contains($"{HateoasLinkConsts.Collection}:sort", linkDictionary);
        Assert.Equal("GET", collectionSort.Method);
        Assert.Equal(
            "/get-personal-data-by-contract/{contractId}?page=1&pageSize={pageSize}&sortBy={sortBy}&sortOrder={sortOrder}&filterBy={filterBy}&filterCriteria={filterCriteria}",
            collectionSort.Href);
        Assert.True(collectionSort.Templated);

        Link collectionFilter = Assert.Contains($"{HateoasLinkConsts.Collection}:filter", linkDictionary);
        Assert.Equal("GET", collectionFilter.Method);
        Assert.Equal(
            "/get-personal-data-by-contract/{contractId}?page=1&pageSize={pageSize}&sortBy={sortBy}&sortOrder={sortOrder}&filterBy={filterBy}&filterCriteria={filterCriteria}",
            collectionFilter.Href);
        Assert.True(collectionFilter.Templated);
    }

    /// <summary>
    ///     Assert that a collection with pagination, on first page returns pagination links without previous link.
    /// </summary>
    /// <returns>Async task.</returns>
    [Fact]
    public async Task CollectionWithPagination_FirstPage_ReturnCollectionLinks()
    {
        var builder = PrepareAndGetLinksBuilder();
        var serialized =
            JsonSerializer.Serialize(new FakeResourceCollectionDto { Total = 125, PageSize = 10, Page = 1 },
                _serializerOptions);
        var deserialized = JsonSerializer.Deserialize<JsonElement>(serialized, JsonSerializerOptions.Default)
            .EnumerateObject();

        // Act
        IDictionary<string, Link> linkDictionary = await builder.BuildLinksAsync(deserialized);

        // Assert
        Assert.Equal(6, linkDictionary.Count);

        Link collectionFirst = Assert.Contains($"{HateoasLinkConsts.Collection}:first", linkDictionary);
        Assert.Equal("GET", collectionFirst.Method);
        Assert.Equal(
            "/get-personal-data-by-contract/{contractId}?page=1&pageSize=10&sortBy={sortBy}&sortOrder={sortOrder}&filterBy={filterBy}&filterCriteria={filterCriteria}",
            collectionFirst.Href);
        Assert.True(collectionFirst.Templated);

        Link collectionNext = Assert.Contains($"{HateoasLinkConsts.Collection}:next", linkDictionary);
        Assert.Equal("GET", collectionNext.Method);
        Assert.Equal(
            "/get-personal-data-by-contract/{contractId}?page=2&pageSize=10&sortBy={sortBy}&sortOrder={sortOrder}&filterBy={filterBy}&filterCriteria={filterCriteria}",
            collectionNext.Href);
        Assert.True(collectionNext.Templated);

        Link collectionLast = Assert.Contains($"{HateoasLinkConsts.Collection}:last", linkDictionary);
        Assert.Equal("GET", collectionLast.Method);
        Assert.Equal(
            "/get-personal-data-by-contract/{contractId}?page=13&pageSize=10&sortBy={sortBy}&sortOrder={sortOrder}&filterBy={filterBy}&filterCriteria={filterCriteria}",
            collectionLast.Href);
        Assert.True(collectionLast.Templated);

        Link collectionPage = Assert.Contains($"{HateoasLinkConsts.Collection}:page", linkDictionary);
        Assert.Equal("GET", collectionPage.Method);
        Assert.Equal(
            "/get-personal-data-by-contract/{contractId}?page={page}&pageSize=10&sortBy={sortBy}&sortOrder={sortOrder}&filterBy={filterBy}&filterCriteria={filterCriteria}",
            collectionPage.Href);
        Assert.True(collectionPage.Templated);

        Link collectionSort = Assert.Contains($"{HateoasLinkConsts.Collection}:sort", linkDictionary);
        Assert.Equal("GET", collectionSort.Method);
        Assert.Equal(
            "/get-personal-data-by-contract/{contractId}?page=1&pageSize=10&sortBy={sortBy}&sortOrder={sortOrder}&filterBy={filterBy}&filterCriteria={filterCriteria}",
            collectionSort.Href);
        Assert.True(collectionSort.Templated);

        Link collectionFilter = Assert.Contains($"{HateoasLinkConsts.Collection}:filter", linkDictionary);
        Assert.Equal("GET", collectionFilter.Method);
        Assert.Equal(
            "/get-personal-data-by-contract/{contractId}?page=1&pageSize=10&sortBy={sortBy}&sortOrder={sortOrder}&filterBy={filterBy}&filterCriteria={filterCriteria}",
            collectionFilter.Href);
        Assert.True(collectionFilter.Templated);
    }

    /// <summary>
    ///     Assert that a collection with pagination, on middle page returns all pagination links.
    /// </summary>
    /// <returns>Async task.</returns>
    [Fact]
    public async Task CollectionWithPagination_MiddlePage_ReturnCollectionLinks()
    {
        // Arrange
        var builder = PrepareAndGetLinksBuilder();
        var serialized =
            JsonSerializer.Serialize(new FakeResourceCollectionDto { Total = 125, PageSize = 10, Page = 7 },
                _serializerOptions);
        var deserialized = JsonSerializer.Deserialize<JsonElement>(serialized, JsonSerializerOptions.Default)
            .EnumerateObject();

        // Act
        IDictionary<string, Link> linkDictionary = await builder.BuildLinksAsync(deserialized);

        // Assert
        Assert.Equal(7, linkDictionary.Count);

        Link collectionFirst = Assert.Contains($"{HateoasLinkConsts.Collection}:first", linkDictionary);
        Assert.Equal("GET", collectionFirst.Method);
        Assert.Equal(
            "/get-personal-data-by-contract/{contractId}?page=1&pageSize=10&sortBy={sortBy}&sortOrder={sortOrder}&filterBy={filterBy}&filterCriteria={filterCriteria}",
            collectionFirst.Href);
        Assert.True(collectionFirst.Templated);

        Link collectionPrevious = Assert.Contains($"{HateoasLinkConsts.Collection}:previous", linkDictionary);
        Assert.Equal("GET", collectionPrevious.Method);
        Assert.Equal(
            "/get-personal-data-by-contract/{contractId}?page=6&pageSize=10&sortBy={sortBy}&sortOrder={sortOrder}&filterBy={filterBy}&filterCriteria={filterCriteria}",
            collectionPrevious.Href);
        Assert.True(collectionPrevious.Templated);

        Link collectionNext = Assert.Contains($"{HateoasLinkConsts.Collection}:next", linkDictionary);
        Assert.Equal("GET", collectionNext.Method);
        Assert.Equal(
            "/get-personal-data-by-contract/{contractId}?page=8&pageSize=10&sortBy={sortBy}&sortOrder={sortOrder}&filterBy={filterBy}&filterCriteria={filterCriteria}",
            collectionNext.Href);
        Assert.True(collectionNext.Templated);

        Link collectionLast = Assert.Contains($"{HateoasLinkConsts.Collection}:last", linkDictionary);
        Assert.Equal("GET", collectionLast.Method);
        Assert.Equal(
            "/get-personal-data-by-contract/{contractId}?page=13&pageSize=10&sortBy={sortBy}&sortOrder={sortOrder}&filterBy={filterBy}&filterCriteria={filterCriteria}",
            collectionLast.Href);
        Assert.True(collectionLast.Templated);

        Link collectionPage = Assert.Contains($"{HateoasLinkConsts.Collection}:page", linkDictionary);
        Assert.Equal("GET", collectionPage.Method);
        Assert.Equal(
            "/get-personal-data-by-contract/{contractId}?page={page}&pageSize=10&sortBy={sortBy}&sortOrder={sortOrder}&filterBy={filterBy}&filterCriteria={filterCriteria}",
            collectionPage.Href);
        Assert.True(collectionPage.Templated);

        Link collectionSort = Assert.Contains($"{HateoasLinkConsts.Collection}:sort", linkDictionary);
        Assert.Equal("GET", collectionSort.Method);
        Assert.Equal(
            "/get-personal-data-by-contract/{contractId}?page=1&pageSize=10&sortBy={sortBy}&sortOrder={sortOrder}&filterBy={filterBy}&filterCriteria={filterCriteria}",
            collectionSort.Href);
        Assert.True(collectionSort.Templated);

        Link collectionFilter = Assert.Contains($"{HateoasLinkConsts.Collection}:filter", linkDictionary);
        Assert.Equal("GET", collectionFilter.Method);
        Assert.Equal(
            "/get-personal-data-by-contract/{contractId}?page=1&pageSize=10&sortBy={sortBy}&sortOrder={sortOrder}&filterBy={filterBy}&filterCriteria={filterCriteria}",
            collectionFilter.Href);
        Assert.True(collectionFilter.Templated);
    }

    private HateoasLinksBuilder PrepareAndGetLinksBuilder()
    {
        var routeCollection = new FakeRouteCollection();
        var configuration = new FakeFakeResourceCollectionDtoWithOnlySelfLinksConfiguration();
        var encryptor = new Encryptor(null, null);
        var authorizationService = Substitute.For<IAuthorizationService>();
        var serviceProvider = Substitute.For<IServiceProvider>();
        serviceProvider.GetService(typeof(IAuthorizationService)).Returns(authorizationService);
        return new HateoasLinksBuilder(configuration, encryptor, routeCollection, serviceProvider);
    }

    private class FakeFakeResourceCollectionDtoWithOnlySelfLinksConfiguration : IHateoasLinksConfiguration
    {
        public NestedConfigurationsDictionary? NestedObjectsConfigurations { get; set; }

        public void Configure(HateoasLinksBuilder linksBuilder)
        {
            linksBuilder.SelfLink(RoutesNames.GetPersonalDataByContract);
        }
    }

    private class FakeResourceCollectionDto : ResourceCollectionDto
    {
    }
}