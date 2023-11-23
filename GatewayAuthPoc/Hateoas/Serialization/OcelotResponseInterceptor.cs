using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Ocelot.Middleware;
using Ocelot.Request.Middleware;
using OcelotGateway.Ocelot;

namespace OcelotGateway.Hateoas.Serialization;

public class OcelotResponseInterceptor : IMiddleware
{
    private readonly IConfiguration _configuration;
    private readonly JsonSerializerSettings _jsonSerializerSettings;

    public OcelotResponseInterceptor(IConfiguration configuration, IOptions<MvcNewtonsoftJsonOptions> options)
    {
        _configuration = configuration;
        _jsonSerializerSettings = options.Value.SerializerSettings;
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
        OcelotRouteConfiguration? currentRoute = routeConfigs?.FirstOrDefault(x => x.DownstreamPathTemplate == downstreamRequest?.AbsolutePath);

        if (currentRoute is null)
        {
            await next.Invoke(context);
            return;
        }

        if (await ModifyResponseAsync(context, response, currentRoute.DtoType))
        {
            newBody.Seek(0, SeekOrigin.Begin);
            await newBody.CopyToAsync(originBody);
            response.Body = originBody;
        }
    }

    private async Task<bool> ModifyResponseAsync(HttpContext context, HttpResponse response, string responseDtoType)
    {
        DownstreamResponse? downstreamResponse = context.Items.DownstreamResponse();
        var dtoType = Type.GetType(responseDtoType);
        string content = await downstreamResponse.Content.ReadAsStringAsync();
        Stream stream = response.Body;
        var responseDto = JsonConvert.DeserializeObject(content, dtoType, _jsonSerializerSettings);
        string updatedBodyContent = JsonConvert.SerializeObject(responseDto, _jsonSerializerSettings);

        stream.SetLength(0);
        await using var writer = new StreamWriter(stream, leaveOpen: true);
        await writer.WriteAsync(updatedBodyContent);
        await writer.FlushAsync();
        response.ContentLength = stream.Length;

        return true;
    }
}