using CKBS.AlertManagementsServices.Repositories;
using CKBS.AppContext;
using CKBS.Models.Services.AlertManagement;
using CKBS.Models.SignalR;
using KEDI.Core.Premise.Models.Services.AlertManagement;
using KEDI.Core.Premise.Models.ServicesClass.AlertViewClass;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TelegramBotAPI.Models;

namespace KEDI.Core.Premise.Services.AlertService.Repositories
{
    public class AlertExpiresDateServiceRepo
    {
        public interface IAlertExpiresDateServiceRepo
        {
            Task AlertBeforeDateAsync(List<string> userIds);
            //void CheckDueDate(string userId);
            Task ServiceContractAlertRAsync(List<ServiceContractAlert> dueDate, string userId, int count = 0);
        }
        public class AlertARServiceContract : IAlertExpiresDateServiceRepo
        {
            private readonly DataContext _context;
            private readonly ICheckFrequently _check;
            private readonly IHubContext<AlertSignalRClient> _hubContext;
            private readonly ITelegramApiCleint _telBot;
            public AlertARServiceContract(DataContext context, ICheckFrequently check, IHubContext<AlertSignalRClient> hubContext, ITelegramApiCleint telBot)
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
            public async Task ServiceContractAlertRAsync(List<ServiceContractAlert> dueDate, string userId, int count = 0)
            {
                var countNoti = await _check.CountNotiAsync(Convert.ToInt32(userId));
                await _hubContext.Clients.User(userId).SendAsync("SaleARServiceContract", JsonConvert.SerializeObject(dueDate), countNoti, count);
            }

            public async Task AlertBeforeDateAsync(List<string> userIds)
            {
                var saleArServices = await SaleARServiceContractsAsync();
                var _saleArServices = new List<ServiceContractAlertView>();
                var alertMs = await _check.AlertManagementAsync(TypeOfAlert.ARServiceContract);
                if (alertMs != null)
                {
                    var alertDetails = await _check.AlertDetailsAsync(alertMs.ID);
                    foreach (var ad in alertDetails)
                    {
                        await CheckTimeBfAsync(alertMs, saleArServices, _saleArServices);
                        await CheckUserAsync(_saleArServices, ad, userIds);
                    }
                }
            }
            private async Task CheckUserAsync(List<ServiceContractAlertView> _incomOutgoing, AlertDetail ad, List<string> userIds)
            {
                string output = JsonConvert.SerializeObject(_incomOutgoing);
                List<ServiceContractAlert> dueDate = JsonConvert.DeserializeObject<List<ServiceContractAlert>>(output, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                });
                if (dueDate.Count > 0)
                {
                    await SendToTelebotAsync(dueDate, ad.UserAlerts, userIds);
                }
            }
            private static Task CheckTimeBfAsync(AlertMaster alertMs, List<ServiceContractAlertView> incomOutgoing, List<ServiceContractAlertView> _incomOutgoing)
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
                    var _temptimely = item.DueDate.AddMinutes(_alertTimeStampMin * (-1)); // 07-05-2022 - 07=> 04-05-2022, tody : 30-04-2022, 3days
                    if (_temptimely.CompareTo(DateTime.Now) < 0)
                    {
                        item.CreatedAt = DateTime.Now;
                        _incomOutgoing.Add(item);
                    }
                }
                return Task.CompletedTask;
            }
            private async Task SendToTelebotAsync(List<ServiceContractAlert> duedates, List<UserAlert> allowUser, List<string> userIds)
            {
                List<ServiceContractAlert> _duedates = new();
                var token = await GetTokenAsync();
                foreach (var i in allowUser)
                {
                    if (duedates.Count > 0 && token != null)
                    {
                        foreach (var duedate in duedates)
                        {
                            _duedates.Add(new ServiceContractAlert
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
                                $"\nContract End Date: <b>{duedate.DueDate.ToShortDateString()}</b>" +
                                $"\nType: <i>{duedate.Type}</i>",
                                    ParseMode = ParseMode.Html,
                                };
                                await _telBot.SendMessageAsync(mes);
                            }
                        }
                    }
                    await _context.ServiceContractAlerts.AddRangeAsync(_duedates);
                    await _context.SaveChangesAsync();
                    if (userIds.Count > 0)
                    {
                        foreach (var userId in userIds.ToList())
                        {
                            if (allowUser.Any(u => u.UserAccountID.ToString() == userId))
                            {
                                var count = await _context.ServiceContractAlerts.AsNoTracking().Where(i => !i.IsRead && i.UserID == Convert.ToInt32(userId)).CountAsync();
                                //var _dueDate = _duedates.Where(i => i.UserID == Convert.ToInt32(userId)).ToList();
                                await ServiceContractAlertRAsync(_duedates.Where(i => i.UserID == Convert.ToInt32(userId)).ToList(), userId, count);
                            }
                        }

                    }
                    _duedates = new List<ServiceContractAlert>();
                }
                //await _context.StockAlerts.AddRangeAsync(stocks);
            }
            private async Task<List<ServiceContractAlertView>> SaleARServiceContractsAsync()
            {
                List<ServiceContractAlertView> saleARServiceContracts = await (from sarsc in _context.ServiceContracts
                                                                               join cus in _context.BusinessPartners on sarsc.CusID equals cus.ID
                                                                               join doc in _context.DocumentTypes on sarsc.DocTypeID equals doc.ID
                                                                               select new ServiceContractAlertView
                                                                               {
                                                                                   BPID = cus.ID,
                                                                                   DueDate = sarsc.ContractENDate,
                                                                                   DueDateType = DueDateType.Customer,
                                                                                   ID = 0,
                                                                                   InvoiceNumber = $"{doc.Code}-{sarsc.InvoiceNumber}",
                                                                                   IsRead = false,
                                                                                   Name = cus.Name,
                                                                                   Type = "Customer",
                                                                                   SeriesDID = sarsc.SeriesDID,
                                                                                   InvoiceID = sarsc.ID
                                                                               }).ToListAsync();

                return saleARServiceContracts;
            }
            //private async Task<List<ServiceContractAlertView>> GetIncomingCusAsync()
            //{
            //    var data = await (from incom in _context.IncomingPaymentCustomers.Where(i => i.Status != "close")
            //                      join cus in _context.BusinessPartners.Where(i => i.Type == "Customer") on incom.CustomerID equals cus.ID
            //                      select new ServiceContractAlertView
            //                      {
            //                          BPID = cus.ID,
            //                          DueDate = incom.Date,
            //                          //DueDateType = ContractType.Customer,
            //                          ID = 0,
            //                          InvoiceNumber = incom.ItemInvoice,
            //                          IsRead = false,
            //                          Name = cus.Name,
            //                          Type = "Customer",
            //                          SeriesDID = incom.SeriesDID,
            //                          InvoiceID = incom.IncomingPaymentCustomerID
            //                      }).AsNoTracking().ToListAsync();
            //    return data;
            //}
            //private async Task<List<ServiceContractAlertView>> GetOutgoingVendorAsync()
            //{
            //    var data = await (from outgoing in _context.OutgoingPaymentVendors.Where(i => i.Status != "close")
            //                      join ven in _context.BusinessPartners.Where(i => i.Type == "Vendor") on outgoing.VendorID equals ven.ID
            //                      select new ServiceContractAlertView
            //                      {
            //                          BPID = ven.ID,
            //                          DueDate = outgoing.Date,
            //                          //DueDateType = ContractType.Vendor,
            //                          ID = 0,
            //                          InvoiceNumber = outgoing.ItemInvoice,
            //                          IsRead = false,
            //                          Name = ven.Name,
            //                          Type = "Vendor",
            //                          SeriesDID = outgoing.SeriesDetailID,
            //                          InvoiceID = outgoing.OutgoingPaymentVendorID
            //                      }).AsNoTracking().ToListAsync();
            //    return data;
            //}
        }
    }
}
