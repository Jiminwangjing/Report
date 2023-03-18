using Microsoft.AspNetCore.SignalR;
using CKBS.Models.Services.Administrator.General;
using CKBS.Models.Services.POS;
using CKBS.Models.Services.POS.service;
using CKBS.Models.Services.ReportSale.dev;
using CKBS.Models.SignalR;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using KEDI.Core.Premise.Models.Services.POS.Templates;
using KEDI.Core.Premise.Models.ServicesClass.PrintBarcode;

namespace CKBS.Controllers.Event
{
    public class TimeDelivery
    {
        private static TimeDelivery instance;
        private static List<TableTimerTask> _tableTimerTasks = new();

        private static Thread thread1;

        private readonly CancellationTokenSource tokenSource = new ();
        private static IHubContext<SignalRClient> hubcontext;

        public static IHubContext<SignalRClient> Hubcontext { get => hubcontext; set => hubcontext = value; }

        public static TimeDelivery GetInstance(IHubContext<SignalRClient> hubcontext)
        {
            if (instance == null)
            {
                instance = new TimeDelivery();
                Hubcontext = hubcontext;                
            }
            
            return instance;
        }

        public Task PushTimer(int tableId, string time, char status, CancellationTokenSource cancellationToken)
        {
            var data = StartTimeTable(tableId, time, status);
            return Hubcontext.Clients.All.SendAsync("TimeWalker", data, cancellationToken.Token);
        }

        public void InitTableTime(List<TableTimerTask> tableTimerTasks)
        {
            _tableTimerTasks = tableTimerTasks;
        }

        public TableTimerTask StartTimeTable(int tableId, string time, char status)
        {
            TableTimerTask tableTimerTask = _tableTimerTasks.Find(t => t.TableTime.ID == tableId);
            if (tableTimerTask == null)
            {
                tableTimerTask = new TableTimerTask
                {
                    TableTime = new TableTime { Index = _tableTimerTasks.Count, ID = tableId, Time = time, Status = status },
                    CancellationToken = new CancellationTokenSource()
                };
                _tableTimerTasks.Add(tableTimerTask);

            }
            else
            {
                tableTimerTask.TableTime.Time = time;//tableTimerTask.TableTime.Time;
                tableTimerTask.TableTime.Status = status;
            }

            return tableTimerTask;
        }
        public void ResetTimeTable(int tableId,char status,string time)
        {
            TableTimerTask tableTimerTask = _tableTimerTasks.Find(t => t.TableTime.ID == tableId);
            if (tableTimerTask != null)
            {
                _tableTimerTasks[tableTimerTask.TableTime.Index].TableTime.Status =status;
                _tableTimerTasks[tableTimerTask.TableTime.Index].TableTime.Time = time;
            }
        }
        public string StopTimeTable(int tableId, char status)
        {
            TableTimerTask tableTimerTask = _tableTimerTasks.Find(t => t.TableTime.ID == tableId);
            if (tableTimerTask != null)
            {
                _tableTimerTasks[tableTimerTask.TableTime.Index].TableTime.Status = status;
                return tableTimerTask.TableTime.Time;
            }
            return "00:00:00";

        }
        public string GetTimeTable(int tableId)
        {
            TableTimerTask tableTimerTask = _tableTimerTasks.Find(t => t.TableTime.ID == tableId);
            if (tableTimerTask != null)
            {
                return tableTimerTask.TableTime.Time;
            }
            return "00:00:00";

        }

        public Task StartTimer()
        {
            try
            {
                if (thread1 == null || !thread1.IsAlive)
                {
                    thread1 = new Thread(() =>
                    {
                        InvokeTimeWalkerAsync();
                    });
                    thread1.Start();
                    //isInterval = true;
                }
            }
            catch (Exception e)
            {
                Debug.Write(e);
            }
            return Task.CompletedTask;
        }

        public void StopTimer()
        {
            if (thread1 != null && thread1.IsAlive)
            {
                try
                {
                    tokenSource.Cancel();
                }
                catch (Exception e)
                {
                    Console.Write(e);
                }
                //isInterval = false;
            }
        }

        public void Canceltask(int tableid)
        {
            try
            {
                CancellationTokenSource tokenSource = _tableTimerTasks.Find(t => t.TableTime.ID == tableid).CancellationToken;
                tokenSource.Cancel();
                CancellationToken ct = tokenSource.Token;
                ct.ThrowIfCancellationRequested();
            }
            catch (Exception e)
            {
                Console.Write(e);
            }
        }
        public async void PushOrderByTable(List<Order> list, int tableid, string user)
        {
           await Hubcontext.Clients.All.SendAsync("PushOrder", list, tableid, user);
           // await _hubcontext.Clients.All.SendAsync("PushDataToSecondScreen", list);
        }

        public void ClearUserOrder(int tableid)
        {
            Hubcontext.Clients.All.SendAsync("ClearUserOrder", tableid);
        }

        public async void PushOrderByTableToAvailable(int tableid)
        {
            await Hubcontext.Clients.All.SendAsync("PushOrderToAvailable", tableid);
        }
       
        public async void PrintOrder(List<PrintOrder> items, List<PrinterName> printers,List<GeneralSetting> setting)
        {
           await Hubcontext.Clients.All.SendAsync("PrintItemOrder", items, printers,setting);
        }
        public async void PrintBill(List<PrintBill> list,List<GeneralSetting> setting,string ip)
        {
            if (list.Count > 0)
            {
                await Hubcontext.Clients.All.SendAsync("PrintItemBill", list,setting,ip);
                await Hubcontext.Clients.All.SendAsync("PushStatusBill", list[0].OrderID);
            }
        }
        public async void GetTableAvailable(string move)
        {
            await Hubcontext.Clients.All.SendAsync("GetTableAvailable", move);
        }

        public static Task InvokeTimeWalkerAsync()
        {
            foreach (TableTimerTask tableTimerTask in _tableTimerTasks.ToList())
            {
                if (tableTimerTask.TableTime.Status == TableTime.LOCK)
                {
                    DateTime dateTime = DateTime.ParseExact(tableTimerTask.TableTime.Time, "HH:mm:ss", CultureInfo.InvariantCulture);
                    DateTime dateTime1 = dateTime.AddSeconds(1);
                    _tableTimerTasks[tableTimerTask.TableTime.Index].TableTime.Time = dateTime1.ToString("HH:mm:ss", DateTimeFormatInfo.InvariantInfo);
                }
            }
            return Hubcontext.Clients.All.SendAsync("TimeWalker", _tableTimerTasks.ToArray());
        }

        //POS report
        public async void PrintCashout(List<CashoutReportMaster> cashouts, List<VoidItemReport> voidItems, List<GeneralSetting> settings)
        {
            await Hubcontext.Clients.All.SendAsync("PrintCashout", cashouts, settings);
            await Hubcontext.Clients.All.SendAsync("PrintVoidItem", voidItems, settings);
        }
        public async void PrintCashoutPaymentMeanSummary(List<CashoutReportMaster> cashouts, List<GeneralSetting> settings)
        {
            await Hubcontext.Clients.All.SendAsync("PrintCashoutPaymentMeanSummary", cashouts, settings);
        }
        public async void PrintCategoryAndPayment(List<CashoutReportMaster> cashoutReports, List<GeneralSetting> settings)
        {
            await Hubcontext.Clients.All.SendAsync("PrintCashoutandPayment", cashoutReports, settings);
        }
        public async void PrintCashoutSummaryAndPaymentSummary(List<CashoutReportMaster> cashoutReports, List<GeneralSetting> settings)
        {
            await Hubcontext.Clients.All.SendAsync("PrintCashoutSummaryAndPaymentSummary", cashoutReports, settings);
        }
        public async void PrintCashoutPaymentMeans(List<CashoutReportMaster> cashouts, List<GeneralSetting> settings)
        {
            await Hubcontext.Clients.All.SendAsync("PrintCashoutPaymentMeans", cashouts, settings);
        }
        public async void PrintCashoutSummary(List<CashoutReportMaster> cashouts, List<GeneralSetting> settings)
        {
            await Hubcontext.Clients.All.SendAsync("PrintCashoutSummary", cashouts, settings);
        }
        public async Task PrintBarcodeItems(List<ItemPrintBarcodeView> items, string printerName)
        {
            await Hubcontext.Clients.All.SendAsync("PrintBarcodeItems", items, printerName);
        }
    }  
}
