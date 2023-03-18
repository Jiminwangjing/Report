
using KEDI.Core.Cloud.Models;
using KEDI.Core.Hosting.Models;
using KEDI.Core.Http;
using KEDI.Core.Security.Cryptography;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Threading.Tasks;

namespace KEDI.Core.Hosting
{
    public class HostMessenger
    {
        readonly HostContract _host;
        readonly HttpHandler _httpHandler;
        readonly ILogger<HostMessenger> _logger;
        
        public HostMessenger(ILogger<HostMessenger> logger, IConfiguration config)
        {
            _logger = logger;
            _host = config.GetSection("HostContract").Get<HostContract>();
            RemoveLastSlash(_host.Referrer, _host.Issuer);
            _httpHandler = new HttpHandler
            {
                BaseAddress = _host.Issuer,
                Referrer = _host.Referrer
            };
        }

        public bool IsConnectable(string redirectUrl)
        {
            try
            {
                Uri url = new Uri(redirectUrl);
                string pingUrl = string.Format("{0}", url.Host);
                string host = pingUrl;
                Ping p = new Ping();
                PingReply reply = p.Send(host, 3000);
                return reply.Status == IPStatus.Success;
            }
            catch { return false; }
        }

        public async Task<ContractResponse<TResult>> SendAsync<TResult>(string url, object data)
        {
            try
            {
                var request = new ContractRequest<object>
                {
                    Payload = data
                };

                var responseMessage = await _httpHandler.PostAsync(url, request);
                var response = JsonConvert.DeserializeObject<ContractResponse<TResult>>
                    (await responseMessage.Content.ReadAsStringAsync()) ?? new ContractResponse<TResult>();
                if (!string.IsNullOrWhiteSpace(response?.JwtToken))
                {
                    _httpHandler.Authorization = $"Bearer {response.JwtToken}";
                }
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new ContractResponse<TResult>();
            }
        }

        private static string[] RemoveLastSlash(params string[] urls)
        {
            try
            {
                foreach (string url in urls)
                {
                    if (!string.IsNullOrWhiteSpace(url) && url.EndsWith("/")) { _ = url.Remove(url.Length - 1); }
                }
            }
            catch (Exception ex)
            {
                return new string[] { ex.Message };
            }
            return urls;
        }
    }
}
