using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace TelegramBotAPI.Models
{
    public class SendPhotoRequest:TelegramModel
    {
        // [JsonProperty("photo")]
        // public string Photo { get; set; }
        [JsonProperty("caption")]
        public string Caption { get; set; }
        [JsonProperty("caption_entities")]
        public string[] CaptionEntities { get; set; }
        [JsonIgnore]
        public bool IsUrlPhoto { get; set; }
    }
}
