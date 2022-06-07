using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

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




builder.Services.AddSingleton<IUserServices, BlogUserServices>();

builder.Services.AddSingleton<IBlogService, FileBlogService>();



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



app.Run();
