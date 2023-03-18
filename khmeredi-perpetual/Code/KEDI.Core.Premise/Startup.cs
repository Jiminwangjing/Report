using CKBS.AppContext;
using CKBS.Models.SignalR;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Serialization;
using System;
using System.Globalization;
using Rotativa.AspNetCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using KEDI.Core.Premise.Resources;
using KEDI.Core.Localization.Resources;
using TelegramBotAPI;
using CKBS.AlertManagementsServices.AlertManagementServices;
using System.Net.Http;
using KEDI.Core.Premise.Models.SignalR;
using Microsoft.Extensions.Logging;
using System.Linq;
using KEDI.Core.Premise.DependencyInjection.Extensions;
using Newtonsoft.Json;
using KEDI.Core.Premise.Services;
using System.IO;
using Microsoft.Extensions.FileProviders;

namespace CKBS
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.ResolveConflictingActions(apiDescriptions => apiDescriptions.FirstOrDefault());
            });

            services.AddMemoryCache();
            services.AddHttpContextAccessor();
            services.AddRazorPages().AddRazorRuntimeCompilation();
            services.AddControllersWithViews();
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddAntiforgery(options => options.HeaderName = "MY-XSRF-TOKEN");
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromHours(24);
                options.Cookie.HttpOnly = false;
                options.IOTimeout = TimeSpan.FromHours(1);
            });

            services.AddAuthorization(options =>
            {
                //options.AddPolicy("Permission", policy => policy.AddRequirements(new PrivilegeRequirement(services)));
                options.AddPolicy("UserOnly", policy => policy.RequireClaim("User"));
                options.AddPolicy("UserOnly", policy => policy.RequireUserName("manager"));
            });

            services.AddSignalR(hubOptions =>
            {
                hubOptions.EnableDetailedErrors = true;
                hubOptions.MaximumReceiveMessageSize = long.MaxValue;
                hubOptions.ClientTimeoutInterval = TimeSpan.FromHours(12);
                hubOptions.KeepAliveInterval = TimeSpan.FromHours(6);
            }).AddJsonProtocol(options =>
            {
                options.PayloadSerializerOptions.PropertyNamingPolicy = null;
            });

            services.AddMvc(options => options.MaxModelBindingCollectionSize = int.MaxValue)
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0)
            .AddDataAnnotationsLocalization(options =>
            {
                options.DataAnnotationLocalizerProvider = (type, factory) =>
                {
                    var viewResourceType = typeof(ViewResource);
                    return factory.Create(viewResourceType.Name, viewResourceType.Assembly.FullName);
                };
            })
            .AddSessionStateTempDataProvider()
            .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix, options => { options.ResourcesPath = "Resources"; })
            .AddDataAnnotationsLocalization()
            .AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ContractResolver = new DefaultContractResolver();
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            });
            // services.AddDbContext<DataContext>(option => 
            // option.UseSqlServer(Configuration["UsersConnection:ConnectionString"])
            // .EnableSensitiveDataLogging());

            services.AddTransient<CultureLocalizer>();
            services.Configure<RequestLocalizationOptions>(options =>
            {
                var supportedCultures = new[]
                {
                    new CultureInfo("en"),
                    new CultureInfo("km"),
                    new CultureInfo("zh"),
                    new CultureInfo("th"),
                };
                options.DefaultRequestCulture = new RequestCulture("en");
                //options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;
            });
            services.AddLocalization(option => option.ResourcesPath = "Resources");
            //Add custom object services of KEDI purposes
            services.AddKediCore(Configuration).AddKediHostedServices();
            services.AddKediAuthentication();
            services.RegisterTelegramApiDependency(Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        #pragma warning disable CA1041 // Provide ObsoleteAttribute message
        [Obsolete]
        #pragma warning restore CA1041 // Provide ObsoleteAttribute message
        public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory, IWebHostEnvironment env,
            Microsoft.AspNetCore.Hosting.IHostingEnvironment env2, DataContext context, IHttpClientFactory httpClientFactory)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory;
            loggerFactory.AddFile($"{path}/Logs/Logging.log");

            // Getting Bot Input //
            GetTeleBotInput getInput = new(context, env, Configuration, httpClientFactory);
            getInput.GetInputAsync().Wait();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("v1/swagger.json", "Khmer EDI API");
            });
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            var locOptions = app.ApplicationServices.GetService<IOptions<RequestLocalizationOptions>>();
            app.UseRequestLocalization(locOptions.Value);
            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(
                    Path.Combine(env.ContentRootPath, "ClientApp")),
                RequestPath = "/ClientApp"
            });
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<HubSystem>("/hubSystem");
                endpoints.MapHub<SignalRClient>("/realbox");
                endpoints.MapHub<AlertSignalRClient>("/realalert");
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{Controller=Account}/{Action=Login}");
            });
            RotativaConfiguration.Setup(env2);
        }
    }
}
