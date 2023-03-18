using System;
using System.Collections.Generic;
using System.Text;

namespace TelegramBotAPI.Helpers
{
    public static class APIEndpoint
    {
        public const string BaseUrl = "https://api.telegram.org/bot{0}/";
        public const string SendMessage = "sendMessage";
        public const string SendPhoto = "sendPhoto";
        public const string GetUpdates =  "getUpdates";
    }
}
