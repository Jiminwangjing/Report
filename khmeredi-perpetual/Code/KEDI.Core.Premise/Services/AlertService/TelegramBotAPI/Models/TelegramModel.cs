using Newtonsoft.Json;
using TelegramBotAPI.Helpers;

namespace TelegramBotAPI.Models
{
    [JsonConverter(typeof(ParseModeConverter))]
    public enum ParseMode
    {
        MarkdownV2,
        Markdown,
        Html
    }
    public interface IReplyMarkup { }
    public class TelegramModel
    {
        public string AccessToken { get; set; }
        [JsonProperty("chat_id")]
        public string ChatID { get; set; }
        [JsonProperty("parse_mode")]
        public ParseMode ParseMode { get; set; }
        [JsonProperty("disable_web_page_preview")]
        public bool DisableWebPagePreview { get; set; }
        [JsonProperty("disable_notification")]
        public bool DisableNotification { get; set; }
        [JsonProperty("reply_to_messageId")]
        public int ReplyToMessageId { get; set; }
        [JsonProperty("allow_sending_without_reply")]
        public bool AllowsendingWithoutReply { get; set; }
        [JsonProperty("reply_markup")]
        public IReplyMarkup ReplyMarkup { get; set; } /// Need to impliment InlineKeyboardMarkup or ReplyKeyboardMarkup or ReplyKeyboardRemove or ForceReply
    }
}
