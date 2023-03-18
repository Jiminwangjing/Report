using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Text;
using System.Threading.Tasks;
using TelegramBotAPI.Helpers;

namespace TelegramBotAPI.Models
{
    public class TelegramApiCleint : ITelegramApiCleint
    {
        private readonly IHttpClientFactory _httpCleintFactory;
        public TelegramApiCleint(IHttpClientFactory httpClientFactory)
        {
            _httpCleintFactory = httpClientFactory;
        }

        public async Task<dynamic> GetDetailAsync(TelegramSendMessageRequest request)
        {
            var baseUrl = string.Format(APIEndpoint.BaseUrl, request.AccessToken);
            var client = _httpCleintFactory.CreateClient();
            client.DefaultRequestHeaders.Referrer = new Uri("https://103.9.190.155");
            await client.GetAsync($"{baseUrl}getUpdates");
            return "";
        }

        public async Task SendMessageAsync(TelegramSendMessageRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }
            var baseUrl = string.Format(APIEndpoint.BaseUrl, request.AccessToken);
            var client = _httpCleintFactory.CreateClient();
            client.DefaultRequestHeaders.Referrer = new Uri("https://103.9.190.155");
            HttpContent requestContent = new ObjectContent(typeof(TelegramSendMessageRequest), request,
                new JsonMediaTypeFormatter
                {
                    SerializerSettings = new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore
                    },
                    SupportedEncodings = { Encoding.UTF8 }
                });
            await client.PostAsync($"{baseUrl}{APIEndpoint.SendMessage}", requestContent);
        }
        public async Task SendPhotoAsync(SendPhotoRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }
            // var fileName = request.Photo.Split('\\').Last();
            var baseUrl = string.Format(APIEndpoint.BaseUrl, request.AccessToken);
            var client = _httpCleintFactory.CreateClient();
            client.DefaultRequestHeaders.Referrer = new Uri("https://103.9.190.155");
            if (request.IsUrlPhoto)
            {
                HttpContent requestContent = new ObjectContent(typeof(SendPhotoRequest), request,
                new JsonMediaTypeFormatter
                {
                    SerializerSettings = new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore
                    },
                    SupportedEncodings = { Encoding.UTF8 }
                });
                await client.PostAsync($"{baseUrl}{APIEndpoint.SendPhoto}", requestContent);

            }
            else
            {
                using var content = new MultipartFormDataContent();
                // using FileStream fileStream = new FileStream(request.Photo, FileMode.Open, FileAccess.Read);
                var objJson = JsonConvert.SerializeObject(request);
                var captionEntities = JsonConvert.SerializeObject(request.CaptionEntities);
                var replyMarkup = JsonConvert.SerializeObject(request.ReplyMarkup);
                content.Add(new StringContent(objJson, Encoding.UTF8, "multipart/form-data"));
                content.Add(new StringContent(request.ChatID, Encoding.UTF8), "chat_id");
                // content.Add(new StreamContent(fileStream), "photo", fileName);
                content.Add(new StringContent(request.Caption, Encoding.UTF8), "caption");
                content.Add(new StringContent(request.ParseMode.ToString(), Encoding.UTF8), "parse_mode");
                //content.Add(new StringContent(captionEntities, Encoding.UTF8), "caption_entities");
                //content.Add(new StringContent(request.DisableNotification.ToString(), Encoding.UTF8), "disable_notification");
                //content.Add(new StringContent(request.ReplyToMessageId.ToString(), Encoding.UTF8), "reply_to_message_id");
                //content.Add(new StringContent(request.AllowsendingWithoutReply.ToString(), Encoding.UTF8), "allow_sending_without_reply");
                //content.Add(new StringContent(replyMarkup, Encoding.UTF8), "reply_markup");
                await client.PostAsync($"{baseUrl}{APIEndpoint.SendPhoto}", content);
            }
        }
    }
}
