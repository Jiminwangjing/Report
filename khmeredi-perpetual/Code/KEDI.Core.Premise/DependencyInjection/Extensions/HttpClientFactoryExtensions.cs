using KEDI.Core.Premise.Models.ClientApi;
using KEDI.Core.Premise.Models.Services.Administrator.ApiManagement;
using KEDI.Core.Premise.Repositories.Integrations;
using KEDI.Core.Premise.Repositories.Sync;
using KEDI.Core.Premise.Repository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace KEDI.Core.Premise.DependencyInjection.Extensions
{
    public static class HttpClientFactoryExtensions
    {
        public static IServiceCollection AddHttpClientFactory(this IServiceCollection services, IConfiguration config)
        {

            IDictionary<string, HostSetting> _serverHosts = config.GetSection("HostSettings")
                .Get<Dictionary<string, HostSetting>>();
            services.AddSingleton(_serverHosts);
            services.AddHttpClient<SyncSender, SyncSender>("ExternalSync", httpClient =>
            {
                var setting = _serverHosts["ExternalSync"];
                httpClient.BaseAddress = new Uri(setting.BaseUrl);
                httpClient.Timeout = TimeSpan.FromMinutes(5);
            });

            services.AddHttpClient<IChipMongRepo, ChipMongRepo>("ChipMongSync", httpClient =>
            {
                var setting = _serverHosts["ChipMongSync"];
                httpClient.BaseAddress = new Uri(setting.BaseUrl);
                httpClient.Timeout = TimeSpan.FromMinutes(5);
            });

            services.AddHttpClient<IAeonRepo, AeonRepo>("AeonSync", httpClient =>
            {
                var setting = _serverHosts["AeonSync"];
                httpClient.BaseAddress = new Uri(setting.BaseUrl);
                httpClient.Timeout = TimeSpan.FromMinutes(5);
            });

            services.AddHttpClient<IPrintTemplateRepo, PrintTemplateRepo>(httpClient =>
            {
                httpClient.BaseAddress = new Uri(_serverHosts["PrintTemplate"].BaseUrl);
            });

            return services;
        }

        public class ValidateHeaderHandler : DelegatingHandler
        {
            protected override async Task<HttpResponseMessage> SendAsync(
                HttpRequestMessage request, CancellationToken cancellationToken)
            {
                if (!request.Headers.Contains(ApiKeyHeader.ApiKey))
                {
                    return new HttpResponseMessage(HttpStatusCode.BadRequest)
                    {
                        Content = new StringContent($"The API key header {ApiKeyHeader.ApiKey} is required.")
                    };
                }

                return await base.SendAsync(request, cancellationToken);
            }
        }
    }
}
