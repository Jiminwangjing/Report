using CKBS.AppContext;
using CKBS.Models.Services.AlertManagement;
using CKBS.Models.Services.Inventory;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;

namespace CKBS.AlertManagementsServices.AlertManagementServices
{
    public class GetTeleBotInput
    {
        private readonly DataContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _config;
        private readonly IHttpClientFactory _httpClientFactory;
        private TelegramBotClient Bot;
        public GetTeleBotInput(DataContext context,
            IWebHostEnvironment env, IConfiguration iConfig, IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _env = env;
            _config = iConfig;
            _httpClientFactory = httpClientFactory;
        }
        private string ImagePath => Path.Combine(_env.WebRootPath, "Images/items");
        private TelegramToken GetToken()
        {
            var token = _context.TelegramTokens.FirstOrDefault() ?? new TelegramToken();
            return token;
        }

#pragma warning disable CA1041 // Provide ObsoleteAttribute message
        [Obsolete]
#pragma warning restore CA1041 // Provide ObsoleteAttribute message
        public Task GetInputAsync()
        {
            string tlToken = GetToken().AccessToken;
            if (!string.IsNullOrWhiteSpace(tlToken))
            {
                Bot = new TelegramBotClient(tlToken); //token
                Bot.StartReceiving();
                Bot.OnMessage += Bot_OnMessage;
                //Bot.StopReceiving();         
            }
            return Task.CompletedTask;
        }
        #region
        [Obsolete]
        private async void Bot_OnMessage(object sender, MessageEventArgs e)
        {
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Referrer = new Uri("https://103.9.190.155");
            var url = _config.GetSection("AlertUrl").Value;
            var me = Bot.GetMeAsync().Result;
            var message = e.Message;
            if (message == null || message.Type != MessageType.Text) return;
            if (message.Text == "/getme")
            {
                string text = $"Your name is *{message.From.FirstName} {message.From.LastName}*" +
                    $"\n Your ID is *{message.From.Id}*";
                await Bot.SendTextMessageAsync(message.From.Id, text, parseMode: ParseMode.MarkdownV2);
                return;
            }
            if (message.Text == "/help")
            {
                string text = $"/getme - Getting your details" +
                    $"\n/getbot - Getting bot details" +
                    $"\n/listallitems - Getting all items" +
                    $"\n/listactiveitems - Getting only active items" +
                    $"\n/listinactiveitems - Getting only inactive items" +
                    $"\n/code 'code item' - Getting item by code" +
                    $"\n/name 'name item' - Getting item by name";
                await Bot.SendTextMessageAsync(message.From.Id, text);
                return;
            }
            else if (message.Text == "/getbot")
            {
                string text = $"Hi\\! I am a bot\\. My name is *{me.FirstName} {me.LastName}*";
                await Bot.SendTextMessageAsync(message.From.Id, text, parseMode: ParseMode.MarkdownV2);
                return;
            }
            else if (message.Text.Split(" ").Length >= 2)
            {
                if (message.Text.Split(" ")[0] == "/code")
                {
                    var res = await client.GetAsync($"{url}GetStockAlertByCode/{message.Text[6..]}");
                    var dataString = await res.Content.ReadAsStringAsync();
                    List<StockAlert> data = JsonConvert.DeserializeObject<List<StockAlert>>(dataString,
                    new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore
                    });
                    await SendItemsAsync(data, message, "Code", 6);
                }
                else if (message.Text.Split(" ")[0] == "/name")
                {
                    try
                    {
                        var res = await client.GetAsync($"{url}GetStockAlertByName/{message.Text[6..]}");
                        var dataString = await res.Content.ReadAsStringAsync();
                        List<StockAlert> data = JsonConvert.DeserializeObject<List<StockAlert>>(dataString,
                        new JsonSerializerSettings
                        {
                            NullValueHandling = NullValueHandling.Ignore
                        });
                        await SendItemsAsync(data, message, "Name", 6);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Command \"/name\" could not perform : {ex.Message}");
                        await Bot.SendTextMessageAsync(message.From.Id, $"The Command \"{message.Text.Split(" ")[0]}\" could not be excecuted", parseMode: ParseMode.MarkdownV2);
                    }
                }
            }
            else if (message.Text == "/listallitems")
            {
                try
                {
                    var res = await client.GetAsync($"{url}GetItems/{false}/{false}");
                    var dataString = await res.Content.ReadAsStringAsync();
                    List<ItemMasterData> data = JsonConvert.DeserializeObject<List<ItemMasterData>>(dataString,
                    new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore
                    });
                    await SendItemsAsync(data, message);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    await Bot.SendTextMessageAsync(message.From.Id, $"The Command \"{message.Text}\" could not be excecuted", parseMode: ParseMode.MarkdownV2);
                }
            }
            else if (message.Text == "/listactiveitems")
            {
                try
                {
                    var res = await client.GetAsync($"{url}GetItems/{true}/{false}");
                    var dataString = await res.Content.ReadAsStringAsync();
                    List<ItemMasterData> data = JsonConvert.DeserializeObject<List<ItemMasterData>>(dataString,
                    new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore
                    });
                    await SendItemsAsync(data, message);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    await Bot.SendTextMessageAsync(message.From.Id, $"The Command \"{message.Text}\" could not be excecuted", parseMode: ParseMode.MarkdownV2);
                }
            }
            else if (message.Text == "/listinactiveitems")
            {
                try
                {
                    var res = await client.GetAsync($"{url}GetItems/{false}/{true}");
                    var dataString = await res.Content.ReadAsStringAsync();
                    List<ItemMasterData> data = JsonConvert.DeserializeObject<List<ItemMasterData>>(dataString,
                    new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore
                    });
                    await SendItemsAsync(data, message);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    await Bot.SendTextMessageAsync(message.From.Id, $"The Command \"{message.Text}\" could not be excecuted", parseMode: ParseMode.MarkdownV2);
                }
            }
        }
        #endregion
        private async Task SendItemsAsync(List<StockAlert> data, Message message, string type, int startIndex)
        {
            if (data.Count > 0)
            {
                foreach (var i in data)
                {
                    FileStream fsSource = new($"{ImagePath}/{i.Image}", FileMode.Open, FileAccess.Read);
                    InputOnlineFile file = new(fsSource);
                    string caption = $"Item: <b>{i.ItemName}</b>" +
                    $"\nInstock: <b>{i.InStock}</b>" +
                    $"\nMin Stock: <b>{i.MinInv}</b>" +
                    $"\nMax Stock: <b>{i.MaxInv}</b>" +
                    $"\nWarehouse: <b>{i.WarehouseName}</b>";
                    await Bot.SendPhotoAsync(message.From.Id, file, caption, parseMode: ParseMode.Html);
                }
                return;
            }
            else
            {
                await Bot.SendTextMessageAsync(message.From.Id, $"No item with {type} {message.Text[startIndex..]}");
                return;
            }
        }
        private async Task SendItemsAsync(List<ItemMasterData> data, Message message)
        {
            if (data.Count > 0)
            {
                data = data.OrderBy(i => i.Code).ToList();
                int sendOneTime = 30;
                int totalItems = data.Count;
                int numSkip = 0;
                int totalLoopSend = (int)Math.Ceiling((decimal)totalItems / (decimal)sendOneTime);
                for (var i = 0; i < totalLoopSend; i++)
                {
                    string space = "___";
                    string text = $"\n<b>Code</b>{space}<b>Name</b>{space}<b>InStock</b>\n";
                    List<ItemMasterData> dataSend = data.Skip(numSkip).Take(sendOneTime).ToList();
                    foreach (var d in dataSend)
                    {
                        var name = d.KhmerName + "__InStock__" + d.StockIn;
                        text += $"\n{d.Code} {space} {name}\n";
                    }
                    await Bot.SendTextMessageAsync(message.From.Id, text, ParseMode.Html);
                    numSkip += sendOneTime;
                }

                return;
            }
            else
            {
                await Bot.SendTextMessageAsync(message.From.Id, $"<b><i>No items!</i></b>", ParseMode.Html);
                return;
            }
        }
        #region
        //private Image DrawTextImage(String currencyCode, Font font, Color textColor, Color backColor, Size minSize)
        //{
        //    //first, create a dummy bitmap just to get a graphics object
        //    SizeF textSize;
        //    using (Image img = new Bitmap(1, 1))
        //    {
        //        using (Graphics drawing = Graphics.FromImage(img))
        //        {
        //            //measure the string to see how big the image needs to be
        //            textSize = drawing.MeasureString(currencyCode, font);
        //            if (!minSize.IsEmpty)
        //            {
        //                textSize.Width = textSize.Width > minSize.Width ? textSize.Width : minSize.Width;
        //                textSize.Height = textSize.Height > minSize.Height ? textSize.Height : minSize.Height;
        //            }
        //        }
        //    }

        //    //create a new image of the right size
        //    Image retImg = new Bitmap((int)textSize.Width, (int)textSize.Height);
        //    using (var drawing = Graphics.FromImage(retImg))
        //    {
        //        //paint the background
        //        drawing.Clear(backColor);

        //        //create a brush for the text
        //        using (Brush textBrush = new SolidBrush(textColor))
        //        {
        //            drawing.DrawString(currencyCode, font, textBrush, 10, 10);
        //            drawing.Save();
        //        }
        //    }
        //    return retImg;
        //}
        #endregion
    }
}
