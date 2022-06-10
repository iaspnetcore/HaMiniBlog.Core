using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.HttpOverrides; // for  ForwardedHeaders.XForwardedHost 

using Miniblog.Core;
using Microsoft.Extensions.Hosting;

using Miniblog.Core.Services;

using WebEssentials.AspNetCore.OutputCaching;

using WebMarkupMin.AspNetCore6;
using WebMarkupMin.Core;

using WilderMinds.MetaWeblog;

using IWmmLogger = WebMarkupMin.Core.Loggers.ILogger;
using MetaWeblogService = Miniblog.Core.Services.MetaWeblogService;
using WmmNullLogger = WebMarkupMin.Core.Loggers.NullLogger;





var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddW3CLogging(logging =>
{
    // Log all W3C fields
    logging.LoggingFields = W3CLoggingFields.All;

    ////logging.FileSizeLimit = 5 * 1024 * 1024;
    ////logging.RetainedFileCountLimit = 2;
    ////logging.FileName = "MyLogFile";
    ////logging.LogDirectory = @"C:\logs";
    //logging.FlushInterval = TimeSpan.FromSeconds(2);
});

//custom extend
//come from:https://soapfault.com/2020/02/24/asp-net-core-reverse-proxy-and-x-forwarded-headers/
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders =
        ForwardedHeaders.XForwardedHost |     //Not included in the defaults using ASPNETCORE_FORWARDEDHEADERS_ENABLED
        ForwardedHeaders.XForwardedFor |
        ForwardedHeaders.XForwardedProto;
    options.ForwardLimit = 4;
    //options.KnownNetworks.Clear(); //In a real scenario we would add the real proxy network(s) here based on a config parameter
    //options.KnownProxies.Clear();  //In a real scenario add the real proxy here based on a config parameter
});

builder.Services.AddSingleton<IUserServices, BlogUserServices>();


builder.Services.AddSingleton<IBlogService, FileBlogJsonDataService>();

builder.Services.Configure<BlogSettings>(builder.Configuration.GetSection("blog"));
builder.Services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddMetaWeblog<MetaWeblogService>();

// Progressive Web Apps https://github.com/madskristensen/WebEssentials.AspNetCore.ServiceWorker
builder.Services.AddProgressiveWebApp(
    new WebEssentials.AspNetCore.Pwa.PwaOptions
    {
        OfflineRoute = "/shared/offline/"
    });

// Output caching (https://github.com/madskristensen/WebEssentials.AspNetCore.OutputCaching)
builder.Services.AddOutputCaching(
    options =>
    {
        options.Profiles["default"] = new OutputCacheProfile
        {
            Duration = 3600
        };
    });

// Cookie authentication.
builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(
        options =>
        {
            options.LoginPath = "/login/";
            options.LogoutPath = "/logout/";
        });

// HTML minification (https://github.com/Taritsyn/WebMarkupMin)
builder.Services
    .AddWebMarkupMin(
        options =>
        {
            options.AllowMinificationInDevelopmentEnvironment = true;
            options.DisablePoweredByHttpHeaders = true;
        })
    .AddHtmlMinification(
        options =>
        {
            options.MinificationSettings.RemoveOptionalEndTags = false;
            options.MinificationSettings.WhitespaceMinificationMode = WhitespaceMinificationMode.Safe;
        });
builder.Services.AddSingleton<IWmmLogger, WmmNullLogger>(); // Used by HTML minifier

// Bundling, minification and Sass transpilation (https://github.com/ligershark/WebOptimizer)
builder.Services.AddWebOptimizer(
    pipeline =>
    {
        pipeline.MinifyJsFiles();
        pipeline.CompileScssFiles()
                .InlineImages(1);
    });

var app = builder.Build();

//call UseForwardedHeaders before diagnostics https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/proxy-load-balancer?view=aspnetcore-6.0
app.UseForwardedHeaders();

app.UseW3CLogging();

app.MapGet("/first-w3c-log", (IWebHostEnvironment webHostEnvironment) =>
{
    return Results.Ok(new { PathToWrite = webHostEnvironment.ContentRootPath });
})
.WithName("GetW3CLog");

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Shared/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}



app.UseHttpsRedirection();


app.Use(
    (context, next) =>
    {
        context.Response.Headers["X-Content-Type-Options"] = "nosniff";
        return next();
    });


app.UseWebOptimizer();



app.UseStaticFilesWithCache();



app.UseMetaWeblog("/metaweblog");
app.UseAuthentication();

app.UseOutputCaching();
app.UseWebMarkupMin();

app.UseRouting();





app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Blog}/{action=Index}/{id?}");

//cone from:https://soapfault.com/2020/02/24/asp-net-core-reverse-proxy-and-x-forwarded-headers/
app.MapGet("/ipaddress", async context =>
{
    //Output the relevant properties as the framework sees it
    await context.Response.WriteAsync($"---As the application sees it{Environment.NewLine}");
    await context.Response.WriteAsync($"HttpContext.Connection.RemoteIpAddress : {context.Connection.RemoteIpAddress}{Environment.NewLine}");
    await context.Response.WriteAsync($"HttpContext.Connection.RemoteIpPort : {context.Connection.RemotePort}{Environment.NewLine}");
    await context.Response.WriteAsync($"HttpContext.Request.Scheme : {context.Request.Scheme}{Environment.NewLine}");
    await context.Response.WriteAsync($"HttpContext.Request.Host : {context.Request.Host}{Environment.NewLine}");

    //Output relevant request headers (starting with an X)
    await context.Response.WriteAsync($"{Environment.NewLine}---Request Headers starting with X{Environment.NewLine}");
    foreach (var header in context.Request.Headers.Where(h => h.Key.StartsWith("X", StringComparison.OrdinalIgnoreCase)))
    {
        await context.Response.WriteAsync($"Request-Header {header.Key}: {header.Value}{Environment.NewLine}");
    }

    //come from:https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/proxy-load-balancer?view=aspnetcore-5.0

    // Request method, scheme, and path
    await context.Response.WriteAsync(
        $"Request Method: {context.Request.Method}{Environment.NewLine}");
    await context.Response.WriteAsync(
        $"Request Scheme: {context.Request.Scheme}{Environment.NewLine}");
    await context.Response.WriteAsync(
        $"Request Path: {context.Request.Path}{Environment.NewLine}");

    // Headers
    await context.Response.WriteAsync($"---all Request Headers:{Environment.NewLine}");

    foreach (var header in context.Request.Headers)
    {
        await context.Response.WriteAsync($"{header.Key}: " +
            $"{header.Value}{Environment.NewLine}");

        
        if (header.Key.Contains("X-Forwarded-For"))
        {          
            await context.Response.WriteAsync($"{header.Key}:{header.Value.ToString() } {header.Value.Count.ToString()} {Environment.NewLine}" );
        }
    }

    await context.Response.WriteAsync(Environment.NewLine);



    // Connection: RemoteIp
    await context.Response.WriteAsync(
        $"context.Connection.RemoteIpAddress: {context.Connection.RemoteIpAddress}");
});


app.Run();
