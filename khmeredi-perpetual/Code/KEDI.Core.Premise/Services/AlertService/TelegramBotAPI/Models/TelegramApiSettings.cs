using System;
using System.Collections.Generic;
using System.Text;

namespace TelegramBotAPI.Models
{
    public class TelegramApiSettings
    {
        public string BaseUrl { get; set; }
        public TelegramApiEndPoint EndPoint { get; set; }

    }
    public class TelegramApiEndPoint
    {
        public string SendMessage { get; set; }
        public string GetUpdates { get; set; }
        public string SendPhoto { get; set; }
    }
}
