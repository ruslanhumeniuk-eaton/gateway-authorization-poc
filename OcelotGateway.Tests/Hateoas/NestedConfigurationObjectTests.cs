using Contracts.Application;
using OcelotGateway.Hateoas.Configurations;
using OcelotGateway.Hateoas;
using System.Text.Json.Serialization;
using System.Text.Json;
using NSubstitute;
using Shared;
using Shared.Encryption;
using OcelotGateway.Hateoas.Serialization;

namespace OcelotGateway.Tests.Hateoas;

/// <summary>
///     Test HATEOAS links configuration for inner objects.
/// </summary>
public class NestedConfigurationObjectTests
{
    /// <summary>
    ///     Assert that there is an object built with correct nested objects' links.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public void ConfigurationWithNestedConfigurations_BuildsLinksForEveryNestedObjects()
    {
        // Arrange
        var data = new FakeContractListDto
        {
            Contracts = new List<FakeContractDto>
            {
                new()
                {
                    Records = new List<FakePersonalDataDto>
                    {
                        new()
                    }
                }
            }
        };
        // that's how the data comes - 3-party service returns a response in JSON, we deserialize it and serialize again but with links if any configured
        var serialized =
            JsonSerializer.Serialize(data, JsonSerializerOptions.Default);
        var deserialized = JsonSerializer.Deserialize<JsonElement>(serialized, JsonSerializerOptions.Default);

        var serializerOptions = PrepareAndGetSerializerOptions();

        // Act
        var enrichedDataWithLinks = JsonSerializer.Serialize(deserialized, serializerOptions);

        // assert is not presented here as we have got a JSON string and no easy way to check it is correct.
        // but it allows you to debug the test and check the string by yourself to see how it is generated
        // and whether it is right or not
    }


    private JsonSerializerOptions PrepareAndGetSerializerOptions()
    {
        var authorizationService = Substitute.For<IAuthorizationService>();
        var serviceProvider = Substitute.For<IServiceProvider>();
        serviceProvider.GetService(typeof(IAuthorizationService)).Returns(authorizationService);
        serviceProvider.GetService(typeof(IEncryptor)).Returns(new Encryptor(null, null));
        serviceProvider.GetService(typeof(IRouteCollection)).Returns(new FakeRouteCollection());
        serviceProvider.GetService(typeof(IServiceProvider)).Returns(serviceProvider);
        var serializerOptions = new JsonSerializerOptions();
        serializerOptions.Converters.Add(
            new HateoasJsonConverter<ContractsListHateoasLinksConfiguration>(
                new HateoasLinksBuilderFactory(serviceProvider)));
        serializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        serializerOptions.WriteIndented = true;
        serializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        return serializerOptions;
    }

    private class FakeContractListDto : ResourceCollectionDto
    {
        public IEnumerable<FakeContractDto> Contracts { get; set; }
    }

    private class FakeContractDto
    {
        public IEnumerable<FakePersonalDataDto> Records { get; set; }
    }

    private class FakePersonalDataDto
    {
        public string RandomData { get; set; } = "random data";
    }

    private class ContractsListHateoasLinksConfiguration : IHateoasLinksConfiguration
    {
        public NestedConfigurationsDictionary? NestedObjectsConfigurations { get; set; } = new(
            new Dictionary<string, Type>
            {
                { "contracts", typeof(ContractObjectHateoasLinksConfiguration) }
            });

        public void Configure(HateoasLinksBuilder linksBuilder)
        {
            linksBuilder.SelfLink(RoutesNames.GetContracts);
            linksBuilder.AddLink("get-contract/{contractId}", RoutesNames.GetContract);
        }
    }

    private class ContractObjectHateoasLinksConfiguration : IHateoasLinksConfiguration
    {
        public NestedConfigurationsDictionary? NestedObjectsConfigurations { get; set; } = new(
            new Dictionary<string, Type>
            {
                { "records", typeof(PersonalDataHateoasLinksConfiguration) }
            });

        public void Configure(HateoasLinksBuilder linksBuilder)
        {
            linksBuilder.SelfLink(RoutesNames.GetContract);
            linksBuilder.AddLink("get-personal-data-by-contract/{contractId}", RoutesNames.GetPersonalDataByContract);
        }
    }

    private class PersonalDataHateoasLinksConfiguration : IHateoasLinksConfiguration
    {
        public NestedConfigurationsDictionary? NestedObjectsConfigurations { get; set; }

        public void Configure(HateoasLinksBuilder linksBuilder)
        {
            linksBuilder.SelfLink(RoutesNames.GetPersonalDataByContract);
        }
    }
}