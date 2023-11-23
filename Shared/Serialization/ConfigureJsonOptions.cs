using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Shared.Serialization;

/// <summary>
///     Configure JSON options for the whole application.
/// </summary>
public class ConfigureJsonOptions : IConfigureOptions<MvcNewtonsoftJsonOptions>
{
    private readonly IContractResolver _contractResolver;

    /// <summary>
    ///     Initializes a new instance of the <see cref="ConfigureJsonOptions" /> class.
    /// </summary>
    /// <param name="contractResolver">JSON contract resolver.</param>
    public ConfigureJsonOptions(IContractResolver contractResolver)
    {
        _contractResolver = contractResolver;
    }

    /// <inheritdoc />
    public void Configure(MvcNewtonsoftJsonOptions options)
    {
        options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
        options.SerializerSettings.ContractResolver = _contractResolver;
    }
}