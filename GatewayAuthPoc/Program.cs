using Contracts;
using Etds.BuildingBlocks.Infrastructure.Contexts;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using OcelotGateway.Authorization;
using OcelotGateway.Hateoas;
using OcelotGateway.Hateoas.Serialization;
using Shared;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("ocelot.json", false, true);

builder.Services.AddControllers();
builder.Services.AddEncryption();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddSerialization();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMemoryCache();
builder.Services.AddContext();

builder.Services.AddOcelot(); // <--

builder.Services.AddGreenMotionIdentityServerAuthentication();
builder.Services.AddPermissionServices();
builder.Services.AddScoped<AuthorizationMiddleware>(); // <--
builder.Services.AddScoped<OcelotResponseInterceptor>(); // <--
builder.Services.AddHateoas();

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<AuthorizationMiddleware>(); // <--

app.MapControllers();

app.UseMiddleware<OcelotResponseInterceptor>();
await app.UseOcelot(); // <--

app.Run();