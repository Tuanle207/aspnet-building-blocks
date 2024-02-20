using BuildingBlocks.Logging;
using Microsoft.Extensions.Localization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using BuildingBlock.DemoWebApi.ServiceRegister;
using BuildingBlock.DemoWebApi.Localization.Users;

var builder = WebApplication.CreateBuilder(args);

IConfiguration configuration = builder.Configuration;

builder.Services.AddLogging(configuration);
builder.Services.AddLocalization(configuration);

var app = builder.Build();

app.UseAppRequestLocalization();

app.MapGet("/", ([FromServices] ILoggerFactory loggerFactory, [FromServices] IStringLocalizer L, [FromServices] IStringLocalizer<UsersResource> L1) =>
{
    ILogger logger = loggerFactory.CreateLogger("home");
    logger.LogInformation("test");
    var x = L.GetAllStrings();
    foreach (var item in x)
    {
        logger.LogInformation(item.Name);
    }
    return new {
        Default = L["Hello_User", "Tuan"].Value,
        Nested = L["Shared.Page", 199].Value,
        Nested2 = L["Shared.Header.Title"].Value,
        Nested3 = L["Shared.Header.Subtitle"].Value,
        SpecificResource = L1["Hello"].Value,
        SpecificResource2 = L1["Shared.Page"].Value,
    };
});

app.Run();
        