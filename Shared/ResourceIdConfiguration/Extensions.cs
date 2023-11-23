using Contracts.Application;
using Microsoft.Extensions.DependencyInjection;
using Shared.Encryption;
using Shared.Serialization;
using System.ComponentModel;

namespace Shared.ResourceIdConfiguration;

internal static class Extensions
{
    internal static IServiceCollection AddResourceIdConfiguration(this IServiceCollection services)
    {
        
        // add the converter for encrypting/decrypting ResourceId
        services.AddSingleton<IJsonConverterFactory, ResourceIdJsonConverterFactory>();
        services.AddSingleton<IJsonConverterFactory, ResourceIdDictionaryKeyJsonConverterFactory>();
        services.ConfigureOptions<ResourceIdMvcOptions>();

        // Update Open API documentation
        TypeDescriptor.AddAttributes(typeof(IResourceId), new TypeConverterAttribute(typeof(StringParameterTypeConverter)));

        return services;
    }
}