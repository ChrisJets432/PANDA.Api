using System.Net;
using System.Reflection;
using System.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Localization;
using PANDA.Common.Converters;
using PANDA.Common.Core.Authentication;

namespace PANDA.Api.Extensions;

public static class WebApplicationExtensions
{
    private static readonly List<string> _whitelist = ["Service", "Data", "ViewBuilder", "Repository"];
    
    public static IServiceCollection ScopeDependencyInjection(this IServiceCollection services, string baseSpace, params string[] additionalSpaces)
    {
        var spaces = new List<string> { baseSpace };
        spaces.AddRange(additionalSpaces.Select(x => 
            x.StartsWith('.') 
                ? baseSpace + x 
                : x.Contains('{') && x.Contains('}') 
                    ? string.Format(x, baseSpace) 
                    : x
        ));

        foreach (var space in spaces)
        {
            try
            {
                Assembly.Load(space)
                    .GetTypes()
                    .Where(s => _whitelist.Any(s.Name.EndsWith) && !s.IsInterface)
                    .ToList()
                    .ForEach(appService =>
                    {
                        var @interface = appService.GetInterface($"I{appService.Name}");

                        if (@interface != null)
                        {
                            services.AddSingleton(@interface, appService);
                        }
                    });
            }
            catch (FileNotFoundException)
            {
                // ignored
            }
        }

        return services;
    }
    
    public static WebApplicationBuilder InitiateServices(this WebApplicationBuilder builder, string @namespace, bool includeAuthentication = true, params string[] additionalSpaces)
    {
        builder.Services.AddCors();
        builder.Services.AddAntiforgery();
        builder.Services.AddRazorPages();
        builder.Services.AddControllers();
        builder.Services.AddControllersWithViews();
        builder.Services.AddHttpContextAccessor();
        
        builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        
        builder.Services.ScopeDependencyInjection(@namespace, [".Service", ".Data", ..additionalSpaces]);
        
        builder.Services.AddMvc(options =>
        {
            options.InputFormatters.Insert(0, new RawJsonBodyInputFormatter());
        });
        
        builder.Services.AddDistributedMemoryCache();

        if (includeAuthentication)
        {
            builder.Services.AddSession()
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = AuthenticationCommon.SchemeName;
                    options.DefaultChallengeScheme = AuthenticationCommon.SchemeName;
                    options.DefaultScheme = AuthenticationCommon.SchemeName;
                })
                .AddScheme<AuthenticationSchemeOptions, AuthenticationHandler>(AuthenticationCommon.SchemeName, null);
        }

        return builder;
    }

    public static WebApplication InitiateApp(this WebApplication app)
    {
        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseSession();

        app.UseRouting();

        app.UseAuthorization();
        app.UseAuthentication();

        app.MapControllerRoute(name: "default", pattern: "{controller}/{action}");
        app.MapControllers();

        //app.UseStatusCodePagesWithReExecute("/component/systemautomatic/{0}");
        app.UseRequestLocalization(options =>
        {
            var defaultCulture = AuthenticationCommon.Cultures.English;
            var supportedCultures = new[] { AuthenticationCommon.Cultures.French, defaultCulture };

            options
                .AddSupportedUICultures(supportedCultures)
                .AddSupportedCultures(supportedCultures)
                .AddInitialRequestCultureProvider(new CustomRequestCultureProvider(async context =>
                {
                    var cultureCode = defaultCulture;

                    if (context.Request.HttpContext.User.Claims.Any(x => x.Type == AuthenticationCommon.Static.Claims.Culture))
                    {
                        cultureCode = context.Request.HttpContext.User.Claims.FirstOrDefault(x => x.Type == AuthenticationCommon.Static.Claims.Culture)
                            ?.Value;
                    }

                    return new ProviderCultureResult(cultureCode);
                }));
        });

        return app;
    }
}