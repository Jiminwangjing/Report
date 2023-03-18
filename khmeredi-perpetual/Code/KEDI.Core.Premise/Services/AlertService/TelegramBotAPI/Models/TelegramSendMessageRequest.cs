using Newtonsoft.Json;

namespace TelegramBotAPI.Models
{
    public class TelegramSendMessageRequest : TelegramModel
    {
        [JsonProperty("text")]
        public string Text { get; set; }
        [JsonProperty("entities")]
        public string[] MessageEntities { get; set; }

        // [JsonProperty("caption")]
        // public string Caption { get; set; }
        // [JsonProperty("caption_entities")]
        // public string[] CaptionEntities { get; set; }
        // [JsonIgnore]
        // public bool IsUrlPhoto { get; set; }

    }

}
