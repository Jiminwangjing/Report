using CKBS.AppContext;
using CKBS.Models.Services.Administrator.AlertManagement;
using CKBS.Models.Services.AlertManagement;
using CKBS.Models.SignalR;
using KEDI.Core.Premise.Repository;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TelegramBotAPI.Models;

namespace CKBS.AlertManagementsServices.Repositories
{
    public interface IAlertStockRepo
    {
        Task CheckStockAsync(List<string> userIds);
        Task StockAlertRAsync(string userId, List<StockAlert> stockAlerts, int count = 0);
    }

    public class AlertStockRepo : IAlertStockRepo
    {
        private readonly DataContext _context;
        private readonly IHubContext<AlertSignalRClient> _hubContext;
        private readonly ICheckFrequently _check;
        private readonly ITelegramApiCleint _telBot;
        private readonly IWebHostEnvironment _env;
        private readonly UtilityModule _utils;

        public AlertStockRepo(DataContext context, IHubContext<AlertSignalRClient> hubContext, ICheckFrequently check,
            ITelegramApiCleint telegramApiCleint, IWebHostEnvironment env)
        {
            _context = context;
            _hubContext = hubContext;
            _check = check;
            _telBot = telegramApiCleint;
            _env = env;
        }
        private async Task<string> GetTokenAsync()
        {
            var token = await _context.TelegramTokens.FirstOrDefaultAsync() ?? new TelegramToken();
            return token.AccessToken;
        }
        private string ImagePath => Path.Combine(_env.WebRootPath, "Images/items");
        public async Task CheckStockAsync(List<string> userIds)
        {
            List<StockAlert> stockAlerts = new();
            var appId = await _check.AlertManagementAsync(TypeOfAlert.Stock);
            if (appId != null)
            {
                var alertDetails = await _check.AlertDetailsAsync(appId.ID);
                foreach (var ad in alertDetails)
                {
                    await SendToClientAsync(ad, stockAlerts);
                    await SendToTelebotAsync(stockAlerts, ad.UserAlerts, userIds);
                }
            }
        }
        public async Task StockAlertRAsync(string userId, List<StockAlert> stockAlerts, int count = 0)
        {
            var countNoti = await _check.CountNotiAsync(Convert.ToInt32(userId));
            await _hubContext.Clients.User(userId).SendAsync("AlertStock", JsonConvert.SerializeObject(stockAlerts), countNoti, count);
        }
        private async Task SendToClientAsync(AlertDetail setting, List<StockAlert> stockAlerts)
        {
            foreach (var j in setting.AlertWarehouses)
            {
                var dataStock = await StockAsync(j.WarehouseID);
                foreach (var i in dataStock)
                {
                    if (i.InStock > 0 && i.InStock <= i.MinInv)
                    {
                        i.StockType = StockType.MinStock;
                        i.Type = "Minimum Stock";
                        i.CreatedAt = DateTime.Now;
                        stockAlerts.Add(i);
                    }
                    if (i.InStock > 0 && i.InStock >= i.MaxInv)
                    {
                        i.StockType = StockType.MaxStock;
                        i.CreatedAt = DateTime.Now;
                        i.Type = "Maximun Stock";
                        stockAlerts.Add(i);
                    }
                    if (i.InStock == 0)
                    {
                        i.StockType = StockType.Null;
                        i.CreatedAt = DateTime.Now;
                        i.Type = "Out of Stock";
                        stockAlerts.Add(i);
                    }
                }
            }
        }
        private async Task SendToTelebotAsync(List<StockAlert> stocks, List<UserAlert> allowUser, List<string> userIds)
        {
            List<StockAlert> stockAlerts = new();
            var token = await GetTokenAsync();
            foreach (var i in allowUser)
            {
                if (stocks.Count > 0 && token != null)
                {
                    foreach (var stock in stocks)
                    {
                        stockAlerts.Add(new StockAlert
                        {
                            ID = 0,
                            CompanyID = stock.CompanyID,
                            CreatedAt = DateTime.Now,
                            Image = stock.Image,
                            InStock = stock.InStock,
                            IsRead = stock.IsRead,
                            ItemID = stock.ItemID,
                            ItemName = stock.ItemName,
                            MaxInv = stock.MaxInv,
                            MinInv = stock.MinInv,
                            StockType = stock.StockType,
                            Type = stock.Type,
                            UserID = i.UserAccountID,
                            WarehouseID = stock.WarehouseID,
                            WarehouseName = stock.WarehouseName,
                        });
                        if (i.TelegramUserID != null)
                        {
                            // // var fileName = stock.Image == "" ? "no-image.jpg" : stock.Image;
                            // var mes = new TelegramSendMessageRequest
                            // {
                            //     // IsUrlPhoto = true,
                            //     // AccessToken = token,
                            //     // ChatID = i.TelegramUserID ?? "",
                            //     // Caption = "alertstock",
                            //     // //    $"\nInstock: <b>{stock.InStock}</b>" +
                            //     // //    $"\nMin Stock: <b>{stock.MinInv}</b>" +
                            //     // //    $"\nMax Stock: <b>{stock.MaxInv}</b>" +
                            //     // //    $" \nWarehouse: <b>{stock.WarehouseName}</b>" +
                            //     // //    $"\nType: <i>{stock.Type}</i>",
                            //     // ParseMode = ParseMode.Html,

                            //     AccessToken = token,
                            //     ChatID = i.TelegramUserID ?? "",
                            //     Text = $"alertsotck</b>" +
                            //     $"\nInvoice Number: sfsf</b>" +
                            //     $"\nDue Date: <b>sfsf</b>" +
                            //     $"\nType: <i>sfss</i>",
                            //     ParseMode = ParseMode.Html,
                            // };
                            // await _telBot.SendMessageAsync(mes);
                            var mes = new TelegramSendMessageRequest
                            {
                                AccessToken = token,
                                ChatID = i.TelegramUserID ?? "",
                                Text = $"<b>Alert Stock</b>" +
                                                       $"\nItemName: <b> {stock.ItemName}</b>" +
                                                       $"\nMin Stock: <b>{stock.MinInv}</b>" +
                                                       $"\nMax Stock: <b>{stock.MaxInv}</b>" +
                                                       $"\nWharehouse Name: <b>{stock.WarehouseName}</b>" +
                                                       $"\nType: <i>{stock.Type}</i>",
                                ParseMode = ParseMode.Html,
                            };
                            await _telBot.SendMessageAsync(mes);


                        }
                    }
                }
                await _context.StockAlerts.AddRangeAsync(stockAlerts);
                await _context.SaveChangesAsync();
                if (userIds.Count > 0)
                {
                    foreach (var userId in userIds)
                    {
                        if (allowUser.Any(u => u.UserAccountID.ToString() == userId))
                        {
                            var _stockAlerts = stockAlerts.Where(i => i.UserID == Convert.ToInt32(userId)).ToList();
                            var count = await _context.StockAlerts.AsNoTracking().Where(i => !i.IsRead && i.UserID == Convert.ToInt32(userId)).CountAsync();
                            await StockAlertRAsync(userId, _stockAlerts, count);
                        }
                    }
                }
                stockAlerts = new List<StockAlert>();
            }
        }
        private async Task<IEnumerable<StockAlert>> StockAsync(int whId)
        {
            var data = await (from wh in _context.Warehouses.AsNoTracking().Where(i => !i.Delete && i.ID == whId)
                              join ia in _context.ItemAccountings.AsNoTracking() on wh.ID equals ia.WarehouseID
                              join item in _context.ItemMasterDatas.AsNoTracking().Where(i => !i.Delete && i.Inventory && i.Process != "Standard") on ia.ItemID equals item.ID
                              select new StockAlert
                              {
                                  ItemID = item.ID,
                                  InStock = ia.InStock,
                                  IsRead = false,
                                  ItemName = item.KhmerName,
                                  MaxInv = ia.MaximunInventory,
                                  MinInv = ia.MinimunInventory,
                                  WarehouseID = (int)ia.WarehouseID,
                                  WarehouseName = wh.Name,
                                  Image = item.Image ?? "",
                              }).AsNoTracking().ToListAsync();
            return data;
        }
    }
}
