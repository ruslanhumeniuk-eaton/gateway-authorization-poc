using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using OcelotGateway.Authorization;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("ocelot.json", false, true);

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o =>
    {
        o.Authority = "https://login.dev.greenmotion.tech";

        o.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false
        };
    });

//var scopes = new string[] { };
//builder.Services.AddAuthorization(o =>
//{
//    o.AddPolicy("SomePolicy", policyBuilder =>
//    {
//        policyBuilder.RequireAuthenticatedUser();
//        policyBuilder.RequireClaim(JwtClaimTypes.Scope, scopes);
//        //policyBuilder.Requirements.Add(new HasPermissionRequirement());
//    });

//    o.FallbackPolicy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().RequireClaim(JwtClaimTypes.Scope, scopes).Build();
//});


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMemoryCache();

builder.Services.AddOcelot();
builder.Services.AddSingleton<IPermissionService, PermissionService>();
builder.Services.AddScoped<AuthorizationMiddleware>(); // <--
builder.Services.AddHttpClient("user-access", client => { client.BaseAddress = new Uri("https://localhost:7278"); });

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

await app.UseOcelot(); // <--

app.Run();