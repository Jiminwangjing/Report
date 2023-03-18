using CKBS.Models.Services.POS;
using KEDI.Core.Net.Http.Extionsions;
using KEDI.Core.Premise.Models.Sync;
using KEDI.Core.Repository;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System;
using System.Net;
using KEDI.Core.Premise.Models.ClientApi.UserAccount;
using KEDI.Core.Premise.Models.ClientApi;
using Microsoft.Extensions.Logging;
using KEDI.Core.Premise.Models.Sync.Customs.Clients;
using KEDI.Core.Premise.Models.Sync.Customs.Server;

namespace KEDI.Core.Premise.Repositories.Sync
{
    public interface ISyncSender
    {
        bool EnableExternalSync();
        bool EnableInternalSync();
        Task<BearerResponse> LoginApiKeyAsync(ApiKeyLogin loginModel);
        Task<ServerSyncContainer> PullEntityContainerAsync();
        Task PushBackEntityContainerAsync(Dictionary<Type, IEnumerable<ISyncEntity>> entities);
        Task<IEnumerable<ReceiptContainer>> PushRangeReceiptAsync(IEnumerable<ReceiptContainer> receipts);
        Task<IEnumerable<ReceiptMemoContainer>> PushRangeReceiptMemoAsync(
            IEnumerable<ReceiptMemoContainer> receiptMemos
        );

        Task<IEnumerable<VoidOrderContainer>> PushRangeVoidOrderAsync(
            IEnumerable<VoidOrderContainer> voidOrders
        );
        Task<IEnumerable<VoidItemContainer>> PushRangeVoidItemAsync(
            IEnumerable<VoidItemContainer> voidItems
        );

        Task<IEnumerable<EntryMap<OpenShift>>> PushRangeOpenShiftAsync(
            IEnumerable<EntryMap<OpenShift>> openShifts
        );
        Task<IEnumerable<EntryMap<CloseShift>>> PushRangeCloseShiftAsync(
            IEnumerable<EntryMap<CloseShift>> closeShifts
        );    
    }

    public class SyncSender : ISyncSender
    {
        ILogger<SyncSender> _logger;
        private readonly HttpClient _httpClient;
        private readonly IClientApiRepo _apiManager;
        private readonly HostSetting _internalSettings;
        private readonly HostSetting _externalSetting;
        public SyncSender(ILogger<SyncSender> logger, HttpClient httpClient, IClientApiRepo apiManager, 
        IHttpClientFactory clientFactory, IDictionary<string, HostSetting> serverHosts)
        {
            _logger = logger;
            _httpClient = httpClient;
            _apiManager = apiManager;
            _httpClient = clientFactory.CreateClient("ExternalSync");          
            serverHosts.TryGetValue("ExternalSync", out HostSetting extSettings);
            serverHosts.TryGetValue("InternalSync", out HostSetting intSettings);
            _externalSetting = extSettings;
            _internalSettings = intSettings;                     
        }

        public async Task<BearerResponse> LoginApiKeyAsync(){
            var login = new ApiKeyLogin{
                ApiKey = _externalSetting.ApiKey,
                SecretKey = _externalSetting.SecretKey
            };
            return await LoginApiKeyAsync(login);
        }
        public async Task<BearerResponse> LoginApiKeyAsync(ApiKeyLogin loginModel){
            if(string.IsNullOrWhiteSpace(loginModel.ApiKey)
                || string.IsNullOrWhiteSpace(loginModel.SecretKey)){
                _logger.LogWarning("API key and secret required.");
            }
  
            var auth = await PostDataAsync<BearerResponse, ApiKeyLogin>("/api/v1/account/loginApiKey", loginModel);
            if(!string.IsNullOrWhiteSpace(auth.AccessToken)){
                _httpClient.DefaultRequestHeaders.Remove("Authorization");
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {auth.AccessToken}");
            }
           
            return auth;
        }

        public bool EnableInternalSync()
        {
            return _internalSettings.Enabled;
        }

        public bool EnableExternalSync()
        {
            return _externalSetting.Enabled;
        }

        public async Task<TResult> AuthorizedPostAsync<TResult>(string url){
            var responseMsg = await AuthorizedPostAsync(url);
            return await responseMsg.Content.ReadAsAsync<TResult>();
        }

        public async Task<HttpResponseMessage> AuthorizedPostAsync(string url){
            var responseMsg = await _httpClient.PostAsync(url, null);
            if(responseMsg.StatusCode == HttpStatusCode.Unauthorized){
                var auth = await LoginApiKeyAsync();
                responseMsg = await _httpClient.PostAsync(url, null);
            }
            responseMsg.EnsureSuccessStatusCode();
            return responseMsg;
        }


        public async Task<HttpResponseMessage> AuthorizedPostAsync<TInput>(
            string url, TInput input
        ){
            var responseMsg = await _httpClient.PostAsJsonAsync(url, input);
            if (responseMsg.StatusCode == HttpStatusCode.Unauthorized)
            {
                await LoginApiKeyAsync();
                responseMsg = await _httpClient.PostAsJsonAsync(url, input);
            }
            responseMsg.EnsureSuccessStatusCode();
            return responseMsg;
        }

        public async Task<TResult> PostDataAsync<TResult, TInput>(string url, TInput data)
        {
            var responseMsg = await _httpClient.PostAsJsonAsync(url, data);
            responseMsg.EnsureSuccessStatusCode();
            var result = await responseMsg.Content.ReadAsAsync<TResult>();
            return result;
        }
        public async Task<ServerSyncContainer> PullEntityContainerAsync()
        {
            var result = await AuthorizedPostAsync<ServerSyncContainer>("/api/v1/dataSync/pullEntityContainer");
            return result;
        }

        public async Task PushBackEntityContainerAsync(Dictionary<Type, IEnumerable<ISyncEntity>> entities)
        {
            await AuthorizedPostAsync("/api/v1/dataSync/pushBackEntityContainer", entities);
        }

        public async Task<IEnumerable<ReceiptContainer>> PushRangeReceiptAsync(IEnumerable<ReceiptContainer> receipts)
        {
            var response = await AuthorizedPostAsync("/api/v1/dataSync/pushReceipts", receipts);
            var result = await response.Content.ReadAsAsync<IEnumerable<ReceiptContainer>>();
            return result;
        }

        public async Task<IEnumerable<ReceiptMemoContainer>> PushRangeReceiptMemoAsync(
            IEnumerable<ReceiptMemoContainer> receiptMemos
        )
        {
            var response = await AuthorizedPostAsync("/api/v1/dataSync/pushReceiptMemos", receiptMemos);
            var result = await response.Content.ReadAsAsync<IEnumerable<ReceiptMemoContainer>>();
            return result;
        }

        public async Task<IEnumerable<VoidOrderContainer>> PushRangeVoidOrderAsync(
            IEnumerable<VoidOrderContainer> voidOrders
        ) {
            var response = await AuthorizedPostAsync("/api/v1/dataSync/pushVoidOrders", voidOrders);
            var result = await response.Content.ReadAsAsync<IEnumerable<VoidOrderContainer>>();
            return result;
        }

        public async Task<IEnumerable<VoidItemContainer>> PushRangeVoidItemAsync(
            IEnumerable<VoidItemContainer> voidItems
        )
        {
            var response = await AuthorizedPostAsync("/api/v1/dataSync/pushVoidItems", voidItems);
            var result = await response.Content.ReadAsAsync<IEnumerable<VoidItemContainer>>();
            return result;
        }

        public async Task<IEnumerable<EntryMap<OpenShift>>> PushRangeOpenShiftAsync(
          IEnumerable<EntryMap<OpenShift>> openShifts
        )
        {
            var response = await AuthorizedPostAsync("/api/v1/dataSync/pushOpenShifts", openShifts);
            var result = await response.Content.ReadAsAsync<IEnumerable<EntryMap<OpenShift>>>();
            return result;
        }

        public async Task<IEnumerable<EntryMap<CloseShift>>> PushRangeCloseShiftAsync(
            IEnumerable<EntryMap<CloseShift>> closeShifts
        )
        {
            var response = await AuthorizedPostAsync("/api/v1/dataSync/pushCloseShifts", closeShifts);
            var result = await response.Content.ReadAsAsync<IEnumerable<EntryMap<CloseShift>>>();
            return result;
        }
    }
}
