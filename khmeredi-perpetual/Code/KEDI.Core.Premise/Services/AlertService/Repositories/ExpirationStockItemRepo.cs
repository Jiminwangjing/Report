using CKBS.AlertManagementsServices.Repositories;
using CKBS.AppContext;
using CKBS.Models.Services.Administrator.Inventory;
using CKBS.Models.Services.AlertManagement;
using CKBS.Models.Services.Inventory;
using CKBS.Models.SignalR;
using KEDI.Core.Premise.Models.Services.AlertManagement;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TelegramBotAPI.Models;

namespace KEDI.Core.Premise.Services.AlertService.Repositories
{
    public interface IExpirationStockItemRepo
    {
        Task CheckStockAsync(List<string> userIds);
        Task ExpirationStockItemAlertRAsync(string userId, List<ExpirationStockItem> stockAlerts, int count = 0);
    }

    public class ExpirationStockItemRepo : IExpirationStockItemRepo
    {
        private readonly DataContext _context;
        private readonly IHubContext<AlertSignalRClient> _hubContext;
        private readonly ICheckFrequently _check;
        private readonly ITelegramApiCleint _telBot;
        private readonly IWebHostEnvironment _env;

        public ExpirationStockItemRepo(DataContext context, IHubContext<AlertSignalRClient> hubContext, ICheckFrequently check,
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
            List<ExpirationStockItem> stockAlerts = new();
            var appId = await _check.AlertManagementAsync(TypeOfAlert.ExpireItem);
            if (appId != null)
            {
                var alertDetails = await _check.AlertDetailsAsync(appId.ID);
                foreach (var ad in alertDetails)
                {
                    await SendToClientAsync(appId, ad, stockAlerts);
                    await SendToTelebotAsync(stockAlerts, ad.UserAlerts, userIds);
                }
            }
        }
        public async Task ExpirationStockItemAlertRAsync(string userId, List<ExpirationStockItem> stockAlerts, int count = 0)
        {
            var countNoti = await _check.CountNotiAsync(Convert.ToInt32(userId));
            await _hubContext.Clients.User(userId).SendAsync("ExpireItem", JsonConvert.SerializeObject(stockAlerts), countNoti, count);
        }
        private async Task SendToClientAsync(AlertMaster alertMs, AlertDetail setting, List<ExpirationStockItem> stockAlerts)
        {
            foreach (var j in setting.AlertWarehouses)
            {
                var dataStock = await StockAsync(j.WarehouseID);
                await CheckTimeBfAsync(alertMs, dataStock, stockAlerts);
            }
        }
        private static Task CheckTimeBfAsync(AlertMaster alertMs, List<ExpirationStockItem> expirations, List<ExpirationStockItem> _expirations)
        {
            var bADate = alertMs.BeforeAppDate;
            var _alertTimeStampMin = 0;
            foreach (var item in expirations)
            {
                switch (alertMs.TypeBeforeAppDate)
                {
                    case TypeFrequently.Minutes:
                        _alertTimeStampMin = (int)bADate;
                        break;
                    case TypeFrequently.Hours:
                        _alertTimeStampMin = (int)bADate * 60;
                        break;
                    case TypeFrequently.Days:
                        _alertTimeStampMin = (int)bADate * 1440;
                        break;
                    case TypeFrequently.Weeks:
                        _alertTimeStampMin = (int)bADate * 10080;
                        break;
                    case TypeFrequently.Months:
                        _alertTimeStampMin = (int)bADate * 40320;
                        break;
                    case TypeFrequently.Years:
                        _alertTimeStampMin = (int)bADate * 483840;
                        break;
                }

                var _temptimely = item.ExpirationDate.AddMinutes(_alertTimeStampMin * (-1));

                if (_temptimely.CompareTo(DateTime.Now) < 0)
                {
                    item.CreatedDate = DateTime.Now;
                    item.Type = "Expiration Stock Item";
                    _expirations.Add(item);
                }
            }
            return Task.CompletedTask;
        }
        private async Task SendToTelebotAsync(List<ExpirationStockItem> stocks, List<UserAlert> allowUser, List<string> userIds)
        {
            List<ExpirationStockItem> stockAlerts = new();
            var token = await GetTokenAsync();
            foreach (var i in allowUser)
            {
                if (stocks.Count > 0 && token != null)
                {
                    foreach (var stock in stocks)
                    {
                        stockAlerts.Add(new ExpirationStockItem
                        {
                            ID = 0,
                            CompanyID = stock.CompanyID,
                            CreatedDate = DateTime.Now,
                            ImageItem = stock.ImageItem,
                            Instock = stock.Instock,
                            IsRead = stock.IsRead,
                            ItemId = stock.ItemId,
                            ItemName = stock.ItemName,
                            Type = stock.Type,
                            UserID = i.UserAccountID,
                            WarehouseId = stock.WarehouseId,
                            WarehouseName = stock.WarehouseName,
                            WareDId = stock.WareDId,
                            AddmissionDate = stock.AddmissionDate,
                            BatchAttribute1 = stock.BatchAttribute1,
                            BatchAttribute2 = stock.BatchAttribute2,
                            BatchNo = stock.BatchNo,
                            ExpirationDate = stock.ExpirationDate,
                            ItemBarcode = stock.ItemBarcode,
                            ItemCode = stock.ItemCode,
                            MfrDate = stock.MfrDate,
                            UomName = stock.UomName,
                            UomID = stock.UomID
                        });
                        if (i.TelegramUserID != null)
                        {
                            var fileName = stock.ImageItem == "" ? "no-image.jpg" : stock.ImageItem;
                            var mes = new SendPhotoRequest
                            {
                                IsUrlPhoto = false,
                                AccessToken = token,
                                ChatID = i.TelegramUserID ?? "",
                                // Photo = $"{ImagePath}/{fileName}",
                                Caption = $"Item: <b>{stock.ItemName}</b>" +
                            $"\nInstock: <b>{stock.Instock}</b>" +
                            $"\nBatch No: <b>{stock.BatchNo}</b>" +
                            $"\nExpiration Date: <b>{stock.ExpirationDate.ToShortDateString()}</b>" +
                            $"\nWarehouse: <b>{stock.WarehouseName}</b>" +
                            $"\nType: <i>{stock.Type}</i>",
                                ParseMode = ParseMode.Html,
                            };
                            await _telBot.SendPhotoAsync(mes);
                        }
                    }
                }
                await _context.ExpirationStockItems.AddRangeAsync(stockAlerts);
                await _context.SaveChangesAsync();
                if (userIds.Count > 0)
                {
                    foreach (var userId in userIds)
                    {
                        if (allowUser.Any(u => u.UserAccountID.ToString() == userId))
                        {
                            var _stockAlerts = stockAlerts.Where(i => i.UserID == Convert.ToInt32(userId)).ToList();
                            var count = await _context.ExpirationStockItems.AsNoTracking().Where(i => !i.IsRead && i.UserID == Convert.ToInt32(userId)).CountAsync();
                            await ExpirationStockItemAlertRAsync(userId, _stockAlerts, count);
                        }
                    }
                }
                stockAlerts = new List<ExpirationStockItem>();
            }
        }
        private async Task<List<ExpirationStockItem>> StockAsync(int whId)
        {
            var data = await (from wh in _context.Warehouses.AsNoTracking().Where(i => !i.Delete && i.ID == whId)
                              join whd in _context.WarehouseDetails.AsNoTracking()
                                .Where(i => i.InStock > 0 && !string.IsNullOrEmpty(i.BatchNo)) on wh.ID equals whd.WarehouseID
                              join item in _context.ItemMasterDatas.AsNoTracking()
                                .Where(i => !i.Delete && i.ManItemBy == ManageItemBy.Batches) on whd.ItemID equals item.ID
                              let uom = _context.UnitofMeasures.FirstOrDefault(i => i.ID == item.InventoryUoMID) ?? new UnitofMeasure()
                              select new ExpirationStockItem
                              {
                                  AddmissionDate = whd.AdmissionDate,
                                  ItemId = item.ID,
                                  Instock = (decimal)whd.InStock,
                                  ItemName = item.KhmerName,
                                  WareDId = whd.ID,
                                  WarehouseName = wh.Name,
                                  ImageItem = item.Image ?? "",
                                  BatchAttribute1 = whd.BatchAttr1,
                                  BatchAttribute2 = whd.BatchAttr2,
                                  BatchNo = whd.BatchNo,
                                  ExpirationDate = whd.ExpireDate,
                                  ItemBarcode = item.Barcode,
                                  ItemCode = item.Code,
                                  MfrDate = whd.MfrDate,
                                  UomName = uom.Name,
                                  WarehouseId = wh.ID,
                                  UomID = uom.ID
                              })
                              .Where(i => i.ExpirationDate.ToString() != "0001-01-01")
                              .AsNoTracking().ToListAsync();
            return data;
        }

        //public async Task AlertBeforeDateAsync(List<string> userIds)
        //{
        //    var incomOutgoing = await StockAsync();
        //    var _incomOutgoing = new List<DueDateAlertViewModel>();
        //    var alertMs = await _check.AlertManagementAsync(TypeOfAlert.DueDate);
        //    if (alertMs != null)
        //    {
        //        var alertDetails = await _check.AlertDetailsAsync(alertMs.ID);
        //        foreach (var ad in alertDetails)
        //        {
        //            await CheckTimeBfAsync(alertMs, incomOutgoing, _incomOutgoing);
        //            await CheckUserAsync(_incomOutgoing, ad, userIds);
        //        }
        //    }
        //}
    }
}
