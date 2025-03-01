using System.Net;
using System.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PANDA.Common.Core.Authentication;

namespace PANDA.Common.Extensions;

public static class WebApplicationExtensions
{
    public static void InitiateServices(this WebApplicationBuilder builder, string @namespace, bool includeAuthentication = true, params string[] additionalSpaces)
    {
        builder.Services.AddCors();
        builder.Services.AddAntiforgery();
        builder.Services.AddRazorPages();
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
            builder.Services.AddSession(options =>
                {
                    options.IdleTimeout = TimeSpan.FromMinutes(Convert.ToInt32(ConfigurationManager.ByPath("Environment:Timeout") ?? "30"));
                })
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = AuthenticationCommon.SchemeName;
                    options.DefaultChallengeScheme = AuthenticationCommon.SchemeName;
                    options.DefaultScheme = AuthenticationCommon.SchemeName;
                })
                .AddScheme<AuthenticationSchemeOptions, AuthenticationHandler>(AuthenticationCommon.SchemeName, null);
        }
    }

    public static void InitiateApp(this WebApplication app)
    {
        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseSession();

        app.UseRouting();

        app.UseAuthorization();
        app.UseAuthentication();

        app.MapRazorPages();
        app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}");
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
    }

    public static void AuthenticateRequests(this WebApplication app)
    {
        app
            .UseStatusCodePages(async context =>
            {
                var response = context.HttpContext.Response;
                var path = context.HttpContext.Request.Path.ToString().ToLower();

                if (!(path.Contains("/image/") || path.EndsWith(".css") || path.EndsWith(".js") || path.EndsWith(".gif") || path.EndsWith(".jpg") || path.EndsWith(".png") || path.EndsWith(".svg")))
                {
                    if (new[] { (int)HttpStatusCode.Unauthorized, (int)HttpStatusCode.Forbidden }.Contains(response.StatusCode))
                    {
                        response.Redirect($"{context.HttpContext.ApplicationPath(false)}/login/sign-in?f={HttpUtility.UrlEncode(context.HttpContext.Request.Path.Value + context.HttpContext.Request.QueryString)}&e={response.StatusCode}");
                    }

                    if (new[] { (int)HttpStatusCode.NotFound }.Contains(response.StatusCode))
                    {
                        response.Redirect($"{context.HttpContext.ApplicationPath(false)}/login/sign-in?e={response.StatusCode}");
                    }
                }
            });

        app
            .Use(async (context, next) =>
            {
                var token = context.Session.GetString(AuthenticationCommon.Static.Token);
                if (!string.IsNullOrEmpty(token))
                {
                    //context.Request.Headers.TryAddOrUpdate("Authorization", token);
                    //context.Response.Headers.TryAddOrUpdate("Authorization", token);
                }

                string path = context.Request.Path.ToString().ToLower();
                if (path.Contains("/image/") || path.EndsWith(".css") || path.EndsWith(".js") || path.EndsWith(".gif") || path.EndsWith(".jpg") || path.EndsWith(".png") || path.EndsWith(".svg"))
                {
                    TimeSpan maxAge = new(31, 0, 0, 0);
                    context.Response.Headers.Append("Cache-Control", "max-age=" + maxAge.TotalSeconds.ToString("0"));
                }

                await next();
            });
    }
}