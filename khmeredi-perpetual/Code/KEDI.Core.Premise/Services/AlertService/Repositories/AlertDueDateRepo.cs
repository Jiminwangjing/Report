using CKBS.AppContext;
using CKBS.Models.Services.AlertManagement;
using CKBS.Models.ServicesClass.AlertViewClass;
using CKBS.Models.SignalR;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TelegramBotAPI.Models;

namespace CKBS.AlertManagementsServices.Repositories
{
    public interface IAlertDueDateRepo
    {
        Task AlertBeforeDateAsync(List<string> userIds);
        //void CheckDueDate(string userId);
        Task DueDateAlertRAsync(List<DueDateAlert> dueDate, string userId, int count = 0);
    }

    public class AlertDueDateRepo : IAlertDueDateRepo
    {
        private readonly DataContext _context;
        private readonly ICheckFrequently _check;
        private readonly IHubContext<AlertSignalRClient> _hubContext;
        private readonly ITelegramApiCleint _telBot;
        public AlertDueDateRepo(DataContext context, ICheckFrequently check, IHubContext<AlertSignalRClient> hubContext, ITelegramApiCleint telBot)
        {
            _context = context;
            _check = check;
            _hubContext = hubContext;
            _telBot = telBot;
        }

        private async Task<string> GetTokenAsync()
        {
            var token = await _context.TelegramTokens.FirstOrDefaultAsync() ?? new TelegramToken();
            return token.AccessToken;
        }
        public async Task DueDateAlertRAsync(List<DueDateAlert> dueDate, string userId, int count = 0)
        {
            var countNoti = await _check.CountNotiAsync(Convert.ToInt32(userId));
            await _hubContext.Clients.User(userId).SendAsync("AlertDueDate", JsonConvert.SerializeObject(dueDate), countNoti, count);
        }

        public async Task AlertBeforeDateAsync(List<string> userIds)
        {
            var incomOutgoing = await IncomOutgoing();
            var _incomOutgoing = new List<DueDateAlertViewModel>();
            var alertMs = await _check.AlertManagementAsync(TypeOfAlert.DueDate);
            if (alertMs != null)
            {
                var alertDetails = await _check.AlertDetailsAsync(alertMs.ID);
                foreach (var ad in alertDetails)
                {
                    await CheckTimeBfAsync(alertMs, incomOutgoing, _incomOutgoing);
                    await CheckUserAsync(_incomOutgoing, ad, userIds);
                }
            }
        }
        private async Task CheckUserAsync(List<DueDateAlertViewModel> _incomOutgoing, AlertDetail ad, List<string> userIds)
        {
            string output = JsonConvert.SerializeObject(_incomOutgoing);
            List<DueDateAlert> dueDate = JsonConvert.DeserializeObject<List<DueDateAlert>>(output, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
            });
            if (dueDate.Count > 0)
            {
                await SendToTelebotAsync(dueDate, ad.UserAlerts, userIds);
            }
        }
        private static Task CheckTimeBfAsync(AlertMaster alertMs, List<DueDateAlertViewModel> incomOutgoing, List<DueDateAlertViewModel> _incomOutgoing)
        {
            var bADate = alertMs.BeforeAppDate;
            var _alertTimeStampMin = 0;
            foreach (var item in incomOutgoing)
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

                var _temptimely = item.DueDate.AddMinutes(_alertTimeStampMin * (-1));

                if (_temptimely.CompareTo(DateTime.Now) < 0)
                {
                    item.CreatedAt = DateTime.Now;
                    _incomOutgoing.Add(item);
                }
            }
            return Task.CompletedTask;
        }
        private async Task SendToTelebotAsync(List<DueDateAlert> duedates, List<UserAlert> allowUser, List<string> userIds)
        {
            List<DueDateAlert> _duedates = new();
            var token = await GetTokenAsync();
            foreach (var i in allowUser)
            {
                if (duedates.Count > 0 && token != null)
                {
                    foreach (var duedate in duedates)
                    {
                        _duedates.Add(new DueDateAlert
                        {
                            BPID = duedate.BPID,
                            CompanyID = duedate.CompanyID,
                            CreatedAt = DateTime.Now,
                            DueDate = duedate.DueDate,
                            DueDateType = duedate.DueDateType,
                            ID = 0,
                            InvoiceID = duedate.InvoiceID,
                            InvoiceNumber = duedate.InvoiceNumber,
                            IsRead = duedate.IsRead,
                            Name = duedate.Name,
                            SeriesDID = duedate.SeriesDID,
                            TimeLeft = duedate.TimeLeft,
                            Type = duedate.Type,
                            UserID = i.UserAccountID
                        });
                        if (i.TelegramUserID != null)
                        {
                            var mes = new TelegramSendMessageRequest
                            {
                                AccessToken = token,
                                ChatID = i.TelegramUserID ?? "",
                                Text = $"{duedate.Type} Name: <b>{duedate.Name}</b>" +
                            $"\nInvoice Number: <b>{duedate.InvoiceNumber}</b>" +
                            $"\nDue Date: <b>{duedate.DueDate.ToShortDateString()}</b>" +
                            $"\nType: <i>{duedate.Type}</i>",
                                ParseMode = ParseMode.Html,
                            };
                            await _telBot.SendMessageAsync(mes);
                        }
                    }
                }
                await _context.DueDateAlerts.AddRangeAsync(_duedates);
                await _context.SaveChangesAsync();
                if (userIds.Count > 0)
                {
                    foreach (var userId in userIds.ToList())
                    {
                        if (allowUser.Any(u => u.UserAccountID.ToString() == userId))
                        {
                            var count = await _context.DueDateAlerts.AsNoTracking().Where(i => !i.IsRead && i.UserID == Convert.ToInt32(userId)).CountAsync();
                            //var _dueDate = _duedates.Where(i => i.UserID == Convert.ToInt32(userId)).ToList();
                            await DueDateAlertRAsync(_duedates.Where(i => i.UserID == Convert.ToInt32(userId)).ToList(), userId, count);
                        }
                    }
                }
                _duedates = new List<DueDateAlert>();
            }
            //await _context.StockAlerts.AddRangeAsync(stocks);
        }
        private async Task<List<DueDateAlertViewModel>> IncomOutgoing()
        {
            var incomOutgoing = new List<DueDateAlertViewModel>();
            var incom = await GetIncomingCusAsync();
            var outgoing = await GetOutgoingVendorAsync();
            foreach (var i in incom)
            {
                incomOutgoing.Add(i);
            }
            foreach (var i in outgoing)
            {
                incomOutgoing.Add(i);
            }
            return incomOutgoing;
        }
        private async Task<List<DueDateAlertViewModel>> GetIncomingCusAsync()
        {
            var data = await (from incom in _context.IncomingPaymentCustomers.Where(i => i.Status != "close")
                              join cus in _context.BusinessPartners.Where(i => i.Type == "Customer") on incom.CustomerID equals cus.ID
                              select new DueDateAlertViewModel
                              {
                                  BPID = cus.ID,
                                  DueDate = incom.Date,
                                  DueDateType = DueDateType.Customer,
                                  ID = 0,
                                  InvoiceNumber = incom.ItemInvoice,
                                  IsRead = false,
                                  Name = cus.Name,
                                  Type = "Customer",
                                  SeriesDID = incom.SeriesDID,
                                  InvoiceID = incom.IncomingPaymentCustomerID
                              }).AsNoTracking().ToListAsync();
            return data;
        }
        private async Task<List<DueDateAlertViewModel>> GetOutgoingVendorAsync()
        {
            var data = await (from outgoing in _context.OutgoingPaymentVendors.Where(i => i.Status != "close")
                              join ven in _context.BusinessPartners.Where(i => i.Type == "Vendor") on outgoing.VendorID equals ven.ID
                              select new DueDateAlertViewModel
                              {
                                  BPID = ven.ID,
                                  DueDate = outgoing.Date,
                                  DueDateType = DueDateType.Vendor,
                                  ID = 0,
                                  InvoiceNumber = outgoing.ItemInvoice,
                                  IsRead = false,
                                  Name = ven.Name,
                                  Type = "Vendor",
                                  SeriesDID = outgoing.SeriesDetailID,
                                  InvoiceID = outgoing.OutgoingPaymentVendorID
                              }).AsNoTracking().ToListAsync();
            return data;
        }
    }
}
