using System.Text.Json;
using System.Text.Json.Serialization;
using Ocelot.Middleware;
using Ocelot.Request.Middleware;
using OcelotGateway.Hateoas.Configurations.TestApi;
using OcelotGateway.Ocelot;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace OcelotGateway.Hateoas.Serialization;

public class OcelotResponseInterceptor : IMiddleware
{
    private readonly IConfiguration _configuration;
    private readonly JsonConverterFactory _hateoasJsonConverterFactory;

    private static IDictionary<string, Type> _routesConfigurations = new Dictionary<string, Type>
    {
        { "GetContract", typeof(ContractDtoConfiguration) },
        { "GetContracts", typeof(ContractDtoListConfiguration) },
        { "GetContractMeta", typeof(ContractMetaConfiguration) },
        { "GetPersonalData", typeof(PersonalDataDtoConfiguration) },
        { "GetPersonalDataMeta", typeof(PersonalDataMetaConfiguration) }
    };

    public OcelotResponseInterceptor(IConfiguration configuration, JsonConverterFactory hateoasJsonConverterFactory)
    {
        _configuration = configuration;
        _hateoasJsonConverterFactory = hateoasJsonConverterFactory;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        HttpResponse response = context.Response;
        Stream originBody = response.Body;
        using var newBody = new MemoryStream();
        response.Body = newBody;

        await next.Invoke(context);

        DownstreamRequest? downstreamRequest = context.Items.DownstreamRequest();
        var routeConfigs = _configuration.GetSection("Routes").Get<List<OcelotRouteConfiguration>>();
        OcelotRouteConfiguration? currentRoute =
            routeConfigs?.FirstOrDefault(x => x.DownstreamPathTemplate == downstreamRequest?.AbsolutePath);

        if (currentRoute is null)
        {
            await next.Invoke(context);
            return;
        }

        if (await ModifyResponseAsync(context, response, currentRoute))
        {
            newBody.Seek(0, SeekOrigin.Begin);
            await newBody.CopyToAsync(originBody);
            response.Body = originBody;
        }
    }

    private async Task<bool> ModifyResponseAsync(HttpContext context, HttpResponse response, OcelotRouteConfiguration currentRoute)
    {
        DownstreamResponse? downstreamResponse = context.Items.DownstreamResponse();
        var configurationType = _routesConfigurations.First(x => x.Key == currentRoute.Name).Value;
        string content = await downstreamResponse.Content.ReadAsStringAsync();
        var contentObject = JsonSerializer.Deserialize<JsonElement>(content, JsonSerializerOptions.Default);
        Stream stream = response.Body;

        var serializerOptions = new JsonSerializerOptions();
        serializerOptions.Converters.Add(_hateoasJsonConverterFactory.CreateConverter(configurationType, JsonSerializerOptions.Default));
        serializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        serializerOptions.WriteIndented = true;
        serializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        
        string updatedBodyContent = JsonSerializer.Serialize(contentObject, serializerOptions);

        stream.SetLength(0);
        await using var writer = new StreamWriter(stream, leaveOpen: true);
        await writer.WriteAsync(updatedBodyContent);
        await writer.FlushAsync();
        response.ContentLength = stream.Length;

        return true;
    }
}