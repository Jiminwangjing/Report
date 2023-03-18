using CKBS.AppContext;
using CKBS.Controllers.Event;
using CKBS.Models.Services.Administrator.Inventory;
using CKBS.Models.Services.Administrator.SystemInitialization;
using CKBS.Models.Services.Administrator.Tables;
using CKBS.Models.Services.Inventory;
using CKBS.Models.Services.Inventory.Category;
using CKBS.Models.Services.POS;
using CKBS.Models.Services.POS.KVMS;
using CKBS.Models.Services.POS.service;
using CKBS.Models.Services.POS.Template;
using CKBS.Models.Services.Production;
using CKBS.Models.SignalR;
using KEDI.Core.Premise.Models.Sale;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace CKBS.Models.Services.Responsitory
{
    public interface IKVMS
    {
        //Get groups
        IEnumerable<ItemGroup3> GetGroups();
        IEnumerable<ItemGroup1> GetGroup1s { get; }
        IEnumerable<ItemGroup2> GetGroup2s { get; }

        IEnumerable<ItemGroup3> GetGroup3s { get; }
        //Filter Group

        IEnumerable<ItemGroup2> FilterGroup2(int group1_id);
        IEnumerable<ItemGroup3> FilterGroup3(int group1_id, int group2_id);

        IEnumerable<ServiceItemSales> GetItemMasterDatas(int priceList_id);
        IEnumerable<ServiceItemSales> GetItemMasterByBarcode(int pricelist, string barcode);
        IEnumerable<ServiceItemSales> GetScaleItemMasterByBarcode(int pricelist, string scale_barcode, double scaleprice);
        IEnumerable<ServiceItemSales> FilterItem(int pricelist_id, int itemid);
        IEnumerable<ServiceItemSales> FilterItemByGroup(int pricelist_id);
        IEnumerable<GeneralSetting> GetSetting(int branchid);
        IEnumerable<Order> GetOrder(int tableid, int orderid, int userid);
        IEnumerable<CKBS.Models.Services.POS.Receipt> GetReceiptReprint(int branchid, string date_from, string date_to);
        IEnumerable<CKBS.Models.Services.POS.Receipt> GetReceiptCancel(int branchid, string date_from, string date_to);
        //string GetReceiptReturn(int branchid, string date_from, string date_to);
        IEnumerable<ReceiptSummary> GetReceiptReturn(int branchid, string date_from, string date_to);
        //IEnumerable<ItemsReturn> SendOrder(Order order, string print_type);

        //VMC Edition

        IEnumerable<ItemsReturn> SendOrderQuote(Order order, string print_type, int QID);
        void PrintReceiptBillKVMS(int orderid, string print_type, int QID);

        //End of VMC Edition

        void PrintReceiptBill(int orderid, string print_type);

        //IEnumerable<ItemsReturn> SaveOrder(Order order, string printType);
        IEnumerable<OpenShift> OpenShiftData(int userid, double cash);
        CloseShift CloseShiftData(int userid, double cashout);
        //void SendDataToSecondScreen(Order order);
        void PrintReceiptReprint(int orderid, string print_type);
        void CancelReceipt(int orderid);
        void MoveTable(int old_id, int new_id);
        void CombineReceipt(CombineOrder combineReceipt);
        void SendSplit(Order order, Order addnew);
        void ClearUserOrder(int tableid);
        Task InitialStatusTable();
        Task UpdateTimeOnTable();
        void VoidOrder(int orderid);
        string GetTimeByTable(int TableID);
        string SendReturnItem(List<ReturnItem> returnItems);
    }

    public class KVMSRepository : IKVMS
    {
        private readonly DataContext _context;
        private readonly IReport _report;
        private readonly TimeDelivery _timeDelivery;
        private readonly IActionContextAccessor _accessor;
        //private readonly IIssuseInStockMaterial _IssuseInStockMaterial;
        //private readonly IReturnOrCancelStockMaterial _returnOrCancelStockMaterial;

        public KVMSRepository(DataContext context, IHubContext<SignalRClient> timehubcontext, IReport report, IActionContextAccessor accessor /*IIssuseInStockMaterial issuseInStockMaterial, IReturnOrCancelStockMaterial returnOrCancelStockMaterial*/)
        {
            _context = context;
            _report = report;
            _accessor = accessor;
            _timeDelivery = TimeDelivery.GetInstance(timehubcontext);
            //_timeDelivery.StartTimer();
            //_IssuseInStockMaterial = issuseInStockMaterial;
            //_returnOrCancelStockMaterial = returnOrCancelStockMaterial;
        }

        //VMC Edition

        IEnumerable<ItemsReturn> IKVMS.SendOrderQuote(Order data, string print_type, int QID)
        {
            List<ItemsReturn> list = new();
            List<ItemsReturn> list_group = new();

            //Assign OrderID and OrderDetailID to 0
            data.OrderID = 0;
            foreach (var item in data.OrderDetail)
            {
                item.OrderDetailID = 0;
            }

            foreach (var item in data.OrderDetail.ToList())
            {
                var check = list_group.Find(w => w.ItemID == item.ItemID);
                var item_group_uom = _context.ItemMasterDatas.Include(gu => gu.GroupUOM).Include(uom => uom.UnitofMeasureInv).FirstOrDefault(w => w.ID == item.ItemID);
                var uom_defined = _context.GroupDUoMs.FirstOrDefault(w => w.GroupUoMID == item_group_uom.GroupUomID && w.AltUOM == item.UomID);
                var bom = _context.BOMaterial.FirstOrDefault(w => w.ItemID == item.ItemID && w.Active == true);
                var item_material = (from bomd in _context.BOMDetail.Where(w => w.BID == bom.BID && w.NegativeStock == false && w.Detele == false)
                                     join i in _context.ItemMasterDatas on bomd.ItemID equals i.ID
                                     join gd in _context.GroupDUoMs on bomd.UomID equals gd.AltUOM
                                     join uom in _context.UnitofMeasures on i.InventoryUoMID equals uom.ID
                                     select new
                                     {
                                         bomd.ItemID,
                                         i.Code,
                                         i.KhmerName,
                                         Uom = uom.Name,
                                         gd.Factor,
                                         gd.GroupUoMID,
                                         GUoMID = i.GroupUomID,
                                         bomd.Qty
                                     }).Where(w => w.GroupUoMID == w.GUoMID);
                if (check == null)
                {
                    var item_warehouse = _context.WarehouseSummary.FirstOrDefault(w => w.WarehouseID == data.WarehouseID && w.ItemID == item.ItemID);
                    if (item_warehouse != null)
                    {
                        ItemsReturn item_group = new()
                        {
                            LineID = item.LineID,
                            Code = item.Code,
                            ItemID = item.ItemID,
                            KhmerName = item_group_uom.KhmerName + ' ' + item_group_uom.UnitofMeasureInv.Name,
                            Uom = item.Uom,
                            InStock = (decimal)item_warehouse.InStock - (decimal)item_warehouse.Committed,
                            OrderQty = (decimal)item.PrintQty * (decimal)uom_defined.Factor,
                            Committed = (decimal)item_warehouse.Committed
                        };
                        list_group.Add(item_group);
                    }
                    if (bom != null)
                    {
                        foreach (var items in item_material.ToList())
                        {
                            var item_warehouse_material = _context.WarehouseSummary.FirstOrDefault(w => w.WarehouseID == data.WarehouseID && w.ItemID == items.ItemID);
                            ItemsReturn item_group = new()
                            {
                                LineID = item.LineID,
                                Code = items.Code,
                                ItemID = items.ItemID,
                                KhmerName = items.KhmerName + ' ' + items.Uom,
                                Uom = item.Uom,
                                InStock = (decimal)item_warehouse_material.InStock - (decimal)item_warehouse_material.Committed,
                                OrderQty = ((decimal)item.PrintQty * (decimal)items.Factor) * ((decimal)items.Qty * (decimal)items.Factor),
                                Committed = (decimal)item_warehouse_material.Committed
                            };
                            list_group.Add(item_group);
                        }
                    }
                }
                else
                {
                    check.OrderQty = (decimal)check.OrderQty + (decimal)item.PrintQty * (decimal)uom_defined.Factor;
                }
            }
            foreach (var item in list_group)
            {
                if (item.OrderQty > item.InStock)
                {
                    var item_warehouse = _context.WarehouseSummary.FirstOrDefault(w => w.WarehouseID == data.WarehouseID && w.ItemID == item.ItemID);
                    ItemsReturn item_return = new()
                    {
                        LineID = item.LineID,
                        Code = item.Code,
                        ItemID = item.ItemID,
                        KhmerName = item.KhmerName,
                        Uom = item.Uom,
                        InStock = item.InStock,
                        OrderQty = item.OrderQty,
                        Committed = (decimal)item_warehouse.Committed
                    };
                    list.Add(item_return);
                }

            }
            if (list.Count > 0)
            {
                var _kvmsinfo = _context.KvmsInfo.FirstOrDefault(c => c.KvmsInfoID == QID);
                _context.KvmsInfo.Remove(_kvmsinfo);
                _context.SaveChanges();
                return list;
            }
            if (data.OrderID == 0)
            {
                var setting = _context.GeneralSettings.FirstOrDefault(w => w.UserID == data.UserOrderID);
                var queues = _context.Order_Queue.Where(w => w.BranchID == data.BranchID);
                var receipt = _context.Order_Receipt.Where(w => w.BranchID == data.BranchID);
                //Remove queue order
                data.ReceiptNo = 0.ToString();
                data.QueueNo = 0.ToString();
                data.CheckBill = 'N';
                _context.Order.Update(data);
                _context.SaveChanges();
                StartTime(data.TableID, "00:00:00");

                //Add queue order
                AddQueueOrder(data.BranchID, data.OrderNo);
                //Send data to print bill
                //Check stock
                IssuseCommittedOrder(data.OrderID);
                IssuseCommittedMaterial(data.OrderID);
                if (print_type == "Bill" || print_type == "Pay")
                {
                    PrintReceiptBillKVMS(data.OrderID, print_type, QID);
                }
                //Send data to print order
                SendToPrintOrder(data);

                //Real time push data
                GetOrder(data.TableID, data.OrderID, data.UserOrderID);
            }
            else
            {
                _context.Order.Update(data);
                _context.SaveChanges();
                //Check stock
                IssuseCommittedOrder(data.OrderID);
                IssuseCommittedMaterial(data.OrderID);
                if (print_type == "Bill" || print_type == "Pay")
                {
                    PrintReceiptBillKVMS(data.OrderID, print_type, QID);
                }
                SendToPrintOrder(data);

                var order_detail = _context.OrderDetail.Where(w => w.OrderID == data.OrderID);
                var order_rm = _context.Order.FirstOrDefault(w => w.OrderID == data.OrderID);
                if (order_rm != null)
                {
                    if (!order_detail.Any())
                    {
                        _context.Order.Remove(data);
                        _context.SaveChanges();

                        var count_order = _context.Order.Where(w => w.TableID == data.TableID && w.Cancel == false);
                        if (!count_order.Any())
                        {
                            var time = _timeDelivery.StopTimeTable(data.TableID, 'A');
                            var table_up = _context.Tables.Find(data.TableID);
                            table_up.Status = 'A';
                            table_up.Time = "00:00:00";
                            _context.Update(table_up);
                            _context.SaveChanges();
                        }
                    }
                }

            }
            _timeDelivery.ClearUserOrder(data.TableID);
            var detailkvms = _context.ReceiptKvms.FirstOrDefault(c => c.KvmsInfoID == QID);
            var RID = detailkvms.ReceiptKvmsID;
            var _vmcitem = new ItemsReturn
            {
                ReceiptID = RID
            };
            list.Add(_vmcitem);
            return list;
        }
        public void PrintReceiptBillKVMS(int orderid, string print_type, int QID)
        {
            //Generate receipt id
            SeriesDetail seriesDetail = new();
            var receipt_generated = "";
            var ordered = _context.Order.Include(x => x.OrderDetail).Include(x => x.Currency).FirstOrDefault(w => w.OrderID == orderid);
            if (ordered == null) { return; }

            //Review
            if (print_type == "Pay")
            {
                //var receipts = _context.Order_Receipt.Where(w => w.BranchID == ordered.BranchID);
                //receipt_generated = (receipts.Count() + 1).ToString().PadLeft(6, '0');//DateTime.Now.Year + "" + DateTime.Now.Month + "" + DateTime.Now.Day + "" + (receipts.Count() + 1).ToString().PadLeft(6, '0');
                //Order_Receipt NewReceipt = new Order_Receipt
                //{
                //    BranchID = ordered.BranchID,
                //    ReceiptID = receipt_generated,
                //    DateTime = DateTime.Now

                //};
                //_context.Order_Receipt.Add(NewReceipt);
                //_context.SaveChanges();

                //Start VMC
                var setting = _context.GeneralSettings.FirstOrDefault(w => w.UserID == ordered.UserOrderID);
                var series = _context.Series.FirstOrDefault(w => w.ID == setting.SeriesID);
                //insert seriesDetail
                seriesDetail.SeriesID = series.ID;
                seriesDetail.Number = series.NextNo;
                _context.Update(seriesDetail);
                //update series
                string Sno = series.NextNo;
                long No = long.Parse(Sno);
                series.NextNo = Convert.ToString(No + 1);
                _context.Update(series);
                _context.SaveChanges();
                //update order
                var seriesDID = seriesDetail.ID;
                ordered.SeriesID = series.ID;
                ordered.SeriesDID​ = seriesDID;
                receipt_generated = series.PreFix + "-" + seriesDetail.Number;
                //End VMC

            }


            //VMC 

            //Update check bill order
            var order = _context.Order.Include(x => x.OrderDetail).Include(x => x.Currency).FirstOrDefault(w => w.OrderID == orderid);
            order.CheckBill = 'Y';
            order.ReceiptNo = receipt_generated;
            order.DateOut = DateTime.Today;
            order.TimeOut = DateTime.Now.ToShortTimeString();
            _context.Order.Update(order);
            _context.SaveChanges();

            //End VMC

            //Update check bill order
            //var order = _context.Order.Include(x => x.OrderDetail).Include(x => x.Currency).FirstOrDefault(w => w.OrderID == orderid);
            //order.CheckBill = 'Y';

            //var InvNo = "";
            //if (_context.ReceiptKvms.Count() > 0)
            //{
            //    string tempcount = _context.ReceiptKvms.Last().ReceiptNo;
            //    string[] tempNo = tempcount.Split("-");
            //    int tempnum = Convert.ToInt32(tempNo[1]) + 1;
            //    InvNo = "INV-" + tempnum.ToString().PadLeft(7, '0');
            //}
            //else
            //{
            //    var qcount = _context.ReceiptKvms.Count() + 1;
            //    InvNo = "INV-" + qcount.ToString().PadLeft(7, '0');
            //}

            //order.ReceiptNo = InvNo;

            //order.DateOut = DateTime.Today;
            //order.TimeOut = DateTime.Now.ToString("hh:mm:ss tt");
            //_context.Order.Update(order);
            //_context.SaveChanges();




            if (order.OrderDetail.Any())
            {
                //Print bill or tender
                var table = _context.Tables.Find(order.TableID);
                if (table == null)
                {
                    table = _context.Tables.FirstOrDefault();
                }
                var user = _context.UserAccounts.Include(emp => emp.Employee).FirstOrDefault(w => w.ID == order.UserOrderID);
                var setting = _context.GeneralSettings.Where(w => w.UserID == order.UserOrderID).ToList();
                var customer = _context.BusinessPartners.FirstOrDefault(w => w.Delete == false && w.Type == "Customer" && w.ID == order.CustomerID);
                var syscurrency = _context.Currency.FirstOrDefault(w => w.ID == order.SysCurrencyID);
                var banch = _context.Branches.FirstOrDefault(w => w.Delete == false && w.ID == order.BranchID);
                var receipt = _context.ReceiptInformation.FirstOrDefault(w => w.BranchID == order.BranchID);
                var payment = _context.PaymentMeans.FirstOrDefault(w => w.ID == order.PaymentMeansID);
                string paymentType = (payment == null) ? "" : payment.Type;

                List<PrintBill> PrintBill = new();
                string ReceiptNo = "";
                var Received = "";
                var GrandTotal_Dis = "";
                var Change = "";
                var ChangeSys = "";
                //VMC Edition
                var tempBalanceDue = order.Change.ToString().Replace("-", "");

                double AppliedAmount = order.Received;
                double BalanceDue = 0;
                if (order.GrandTotal <= order.Received)
                {
                    BalanceDue = 0;
                }
                else
                {
                    BalanceDue = Convert.ToDouble(tempBalanceDue);
                }


                //Aging payment
                foreach (var item in order.OrderDetail)
                {
                    var uom = _context.UnitofMeasures.First(u => u.ID == item.UomID);
                    if (item.Qty <= 0)
                    {
                        continue;
                    }
                    if (item.TypeDis == "Cash")
                    {
                        order.DiscountRate = order.DiscountValue;
                    }
                    if (setting[0].VatAble == true)
                    {
                        ReceiptNo = order.ReceiptNo + " " + setting[0].VatNum;
                    }
                    else
                    {
                        ReceiptNo = order.ReceiptNo;
                    }
                    if (order.CurrencyDisplay == "KHR" || order.CurrencyDisplay == "៛")
                    {
                        GrandTotal_Dis = order.CurrencyDisplay + " " + string.Format("{0:n0}", order.GrandTotal_Display);
                        ChangeSys = order.CurrencyDisplay + " " + string.Format("{0:n0}", order.Change_Display);
                    }
                    else
                    {
                        GrandTotal_Dis = order.CurrencyDisplay + " " + order.GrandTotal_Display.ToString("n3")[0..^1];
                        ChangeSys = order.CurrencyDisplay + " " + order.Change_Display.ToString("n3")[0..^1]; //order.Change_Display.ToString("n3").Substring(0, order.Change_Display.ToString("n3").Length - 1)
                    }
                    if (item.Currency == "KHR" || item.Currency == "៛")
                    {
                        Received = item.Currency + " " + string.Format("{0:n0}", order.Received);
                        Change = item.Currency + " " + string.Format("{0:n0}", order.Change);
                    }
                    else
                    {
                        Received = item.Currency + " " + order.Received.ToString("n3")[0..^1];
                        Change = item.Currency + " " + order.Change.ToString("n3")[0..^1];
                    }

                    if (item.Currency == "KHR" || item.Currency == "៛")
                    {
                        PrintBill bill = new()
                        {
                            OrderID = item.OrderID,
                            ReceiptNo = ReceiptNo,
                            Logo = receipt.Logo,
                            BranchName = receipt.Title,
                            Address = receipt.Address,
                            Tel1 = receipt.Tel1,
                            Tel2 = receipt.Tel2,
                            Table = table.Name,
                            OrderNo = order.OrderNo,
                            Cashier = user.Employee.Name,
                            DateTimeIn = order.DateIn.ToString("dd-MM-yyyy") + " " + order.TimeIn,
                            DateTimeOut = order.DateOut.ToString("dd-MM-yyyy") + " " + order.TimeOut,
                            Item = item.KhmerName,
                            Qty = item.Qty + " " + uom.Name,
                            Price = string.Format("{0:n0}", item.UnitPrice),
                            DisItem = item.DiscountRate + "",
                            Amount = string.Format("{0:n0}", item.Total),
                            SubTotal = string.Format("{0:n0}", order.Sub_Total),
                            DisRate = order.DiscountRate + "%",
                            DisValue = string.Format("{0:n0}", order.DiscountValue),
                            TypeDis = order.TypeDis,
                            GrandTotal = item.Currency + " " + string.Format("{0:n0}", order.GrandTotal),
                            GrandTotalSys = GrandTotal_Dis,
                            VatRate = order.TaxRate + "%",
                            VatValue = item.Currency + " " + string.Format("{0:n0}", order.TaxValue),
                            Received = Received,
                            Change = Change,
                            ChangeSys = ChangeSys,
                            DescKh = receipt.KhmerDescription,
                            DescEn = receipt.EnglishDescription,
                            ExchangeRate = string.Format("{0:n0}", order.ExchangeRate),
                            Printer = "",
                            Print = print_type,
                            ItemDesc = item.Description,
                            CustomerInfo = customer.Code + '/' + customer.Name + '/' + customer.Phone + '/' + customer.Address + '/' + customer.Email,
                            Team = receipt.TeamCondition,
                            ItemType = item.ItemType,
                            PaymentMeans = paymentType
                        };
                        //Qty
                        if (item.ItemType == "Service")
                        {
                            var arr_time = item.Qty.ToString().Split('.');
                            bill.Qty = arr_time[0] + "h:" + arr_time[1].PadRight(2, '0') + "m";
                        }
                        PrintBill.Add(bill);
                    }
                    else
                    {
                        PrintBill bill = new()
                        {
                            OrderID = item.OrderID,
                            ReceiptNo = ReceiptNo,
                            Logo = receipt.Logo,
                            BranchName = receipt.Title,
                            Address = receipt.Address,
                            Tel1 = receipt.Tel1,
                            Tel2 = receipt.Tel2,
                            Table = table.Name,
                            OrderNo = order.OrderNo,
                            Cashier = user.Employee.Name,
                            DateTimeIn = order.DateIn.ToString("dd-MM-yyyy") + " " + order.TimeIn,
                            DateTimeOut = order.DateOut.ToString("dd-MM-yyyy") + " " + order.TimeOut,
                            Item = item.KhmerName,
                            Qty = item.Qty + " " + uom.Name,
                            Price = item.UnitPrice.ToString("n3")[0..^1],
                            DisItem = item.DiscountRate + "",
                            Amount = item.Total.ToString("n3")[0..^1],
                            SubTotal = order.Sub_Total.ToString("n3")[0..^1],
                            DisRate = order.DiscountRate + "",
                            DisValue = order.DiscountValue.ToString("n3")[0..^1],
                            TypeDis = order.TypeDis,
                            GrandTotal = item.Currency + " " + order.GrandTotal.ToString("n3")[0..^1],
                            GrandTotalSys = GrandTotal_Dis,
                            VatRate = order.TaxRate + "%",
                            VatValue = item.Currency + " " + order.TaxValue.ToString("n3")[0..^1],
                            Received = Received,
                            Change = Change,
                            ChangeSys = ChangeSys,
                            DescKh = receipt.KhmerDescription,
                            DescEn = receipt.EnglishDescription,
                            ExchangeRate = order.ExchangeRate.ToString("n3")[0..^1],
                            Printer = "",
                            Print = print_type,
                            ItemDesc = item.Description,
                            CustomerInfo = customer.Code + '/' + customer.Name + '/' + customer.Phone + '/' + customer.Address + '/' + customer.Email,
                            Team = receipt.TeamCondition,
                            ItemType = item.ItemType,
                            PaymentMeans = paymentType

                        };
                        //Qty
                        if (item.ItemType == "Service")
                        {
                            var arr_time = item.Qty.ToString().Split('.');
                            bill.Qty = arr_time[0] + "h:" + arr_time[1].PadRight(2, '0') + "m";
                        }
                        PrintBill.Add(bill);
                    }
                }

                var ip = _accessor.ActionContext.HttpContext.Connection.RemoteIpAddress.ToString();
                _timeDelivery.PrintBill(PrintBill, setting, ip);

                //Update status table
                if (print_type == "Bill")
                {
                    var table_update = _context.Tables.Find(order.TableID);
                    table_update.Status = 'P';
                    table_update.Time = _timeDelivery.StopTimeTable(order.TableID, 'P');
                    _timeDelivery.ResetTimeTable(order.TableID, 'P', table_update.Time);
                    _context.Tables.Update(table_update);
                    _context.SaveChanges();

                }
                else if (print_type == "Pay")
                {
                    var count_order = _context.Order.Where(w => w.TableID == ordered.TableID && w.Cancel == false);
                    if (count_order.Count() == 1)
                    {
                        var table_update = _context.Tables.Find(order.TableID);
                        table_update.Status = 'A';
                        table_update.Time = "00:00:00";
                        _timeDelivery.StopTimeTable(order.TableID, 'A');
                        _timeDelivery.ResetTimeTable(order.TableID, 'A', "00:00:00");
                        _context.Tables.Update(table_update);
                        _context.SaveChanges();
                    }
                    //Issuse In Stock 
                    //IssuseInStockOrder(orderid); //today
                    //Issuse In Stock Material
                    //IssuseInStockMaterial(orderid); //today
                    //Remove order

                    //VMC
                    InsertRececiptKVMS(orderid, QID, AppliedAmount, BalanceDue);
                    //End VMC

                    // _context.Database.ExecuteSqlCommand("pos_InsertReceiptKVMS @OrderID={0},@QID={1},@AppliedAmount={2},@BalanceDue={3}",
                    //     parameters: new[] {
                    // orderid.ToString(),
                    // QID.ToString(),
                    // AppliedAmount.ToString(),
                    // BalanceDue.ToString()
                    //});

                    GetOrder(order.TableID, order.OrderID, order.UserOrderID);
                }
                _timeDelivery.ClearUserOrder(order.TableID);

                var detailkvms = _context.ReceiptKvms.FirstOrDefault(c => c.KvmsInfoID == QID);
                if (detailkvms.GrandTotal <= detailkvms.Received)
                {
                    detailkvms.AppliedAmount = detailkvms.GrandTotal;
                    detailkvms.Status = StatusReceipt.Paid;
                }
                //else
                //{
                //    detailkvms.AppliedAmount = detailkvms.AppliedAmount;
                //    detailkvms.Status = StatusReceipt.Aging;
                //}
                _context.ReceiptKvms.Update(detailkvms);
                _context.SaveChanges();

                if (detailkvms.GrandTotal > detailkvms.Received)
                {
                    CreateAgingPaymentCustomer(detailkvms);
                }
            }
        }

        public void InsertRececiptKVMS(int orderid, int QID, double AppliedAmount, double BalanceDue)
        {
            ReceiptKvms receipt = new();
            var order = _context.Order.Include(w => w.OrderDetail).FirstOrDefault(w => w.OrderID == orderid);
            receipt.OrderID = order.OrderID;
            receipt.OrderNo = order.OrderNo;
            if (order.TableID == 0)
            {
                order.TableID = _context.Tables.FirstOrDefault().ID;
            }
            receipt.TableID = order.TableID;
            receipt.ReceiptNo = order.ReceiptNo;
            receipt.QueueNo = order.QueueNo;
            receipt.DateIn = order.DateIn;
            receipt.DateOut = order.DateOut;
            receipt.TimeIn = order.TimeIn;
            receipt.TimeOut = order.TimeOut;
            receipt.WaiterID = order.WaiterID;
            receipt.UserOrderID = order.UserOrderID;
            receipt.UserDiscountID = order.UserDiscountID;
            receipt.CustomerID = order.CustomerID;
            receipt.CustomerCount = order.CustomerCount;
            receipt.PriceListID = order.PriceListID;
            receipt.LocalCurrencyID = order.LocalCurrencyID;
            receipt.SysCurrencyID = order.SysCurrencyID;
            receipt.ExchangeRate = order.ExchangeRate;
            receipt.WarehouseID = order.WarehouseID;
            receipt.BranchID = order.BranchID;
            receipt.CompanyID = order.CompanyID;
            receipt.Sub_Total = order.Sub_Total;
            receipt.TypeDis = order.TypeDis;
            receipt.TaxRate = order.TaxRate;
            receipt.TaxValue = order.TaxValue;
            receipt.GrandTotal = order.GrandTotal;
            receipt.GrandTotal_Sys = order.GrandTotal_Sys;
            receipt.Tip = order.Tip;
            receipt.Received = order.Received;
            receipt.Change = order.Change;
            receipt.CurrencyDisplay = order.CurrencyDisplay;
            receipt.DisplayRate = order.DisplayRate;
            receipt.GrandTotal_Display = order.GrandTotal_Display;
            receipt.Change_Display = order.Change_Display;
            receipt.PaymentMeansID = order.PaymentMeansID;
            receipt.CheckBill = order.CheckBill;
            receipt.Cancel = order.Cancel;
            receipt.Delete = order.Delete;
            receipt.Return = false;
            receipt.PLCurrencyID = order.PLCurrencyID;
            receipt.PLRate = order.PLRate;
            receipt.LocalSetRate = order.LocalSetRate;

            //VMC
            receipt.DiscountRate = order.DiscountRate;
            receipt.DiscountValue = order.Sub_Total * (order.DiscountRate / 100);
            receipt.KvmsInfoID = QID;
            receipt.AppliedAmount = AppliedAmount;
            receipt.BalanceDue = BalanceDue;
            receipt.Status = StatusReceipt.Aging;
            receipt.SeriesID = order.SeriesID;
            receipt.SeriesDID = order.SeriesDID;
            //End VMC


            _context.Add(receipt);
            _context.SaveChanges();
            var receiptid = receipt.ReceiptKvmsID;
            foreach (var item in order.OrderDetail)
            {
                ReceiptDetailKvms receiptDetail = new();
                receiptDetail.ReceiptKvmsID = receiptid;
                receiptDetail.OrderDetailID = item.OrderDetailID;
                receiptDetail.OrderID = item.OrderID;
                receiptDetail.Line_ID = item.Line_ID;
                receiptDetail.ItemID = item.ItemID;
                receiptDetail.Code = item.Code;
                receiptDetail.KhmerName = item.KhmerName;
                receiptDetail.EnglishName = item.EnglishName;
                receiptDetail.Qty = item.Qty;
                receiptDetail.PrintQty = item.PrintQty;
                receiptDetail.UnitPrice = item.UnitPrice;
                receiptDetail.Cost = item.Cost;
                receiptDetail.DiscountRate = item.DiscountRate;
                receiptDetail.DiscountValue = item.DiscountValue;
                receiptDetail.TypeDis = item.TypeDis;
                receiptDetail.Total = item.Total;
                receiptDetail.Total_Sys = item.Total_Sys;
                receiptDetail.UomID = item.UomID;
                receiptDetail.ItemStatus = item.ItemStatus;
                receiptDetail.ItemPrintTo = item.ItemPrintTo;
                receiptDetail.Currency = item.Currency;
                receiptDetail.ItemType = item.ItemType;
                receiptDetail.OpenQty = item.Qty;
                _context.Update(receiptDetail);
                _context.SaveChanges();
            }
            _context.Remove(order);
            _context.SaveChanges();
            IssuseInStockKVMS(receiptid);
        }

        public void IssuseInStockKVMS(int receipt)
        {
            var Order = _context.ReceiptKvms.FirstOrDefault(w => w.ReceiptKvmsID == receipt);
            var OrderDetails = _context.ReceiptDetailKvms.Where(w => w.ReceiptKvmsID == receipt);
            var Com = _context.Company.FirstOrDefault(c => c.ID == Order.CompanyID);
            var Exr = _context.ExchangeRates.FirstOrDefault(e => e.CurrencyID == Com.LocalCurrencyID);
            var docType = _context.DocumentTypes.FirstOrDefault(w => w.Code == "SP");
            //IssuseInstock            
            foreach (var item in OrderDetails)
            {
                var item_master_data = _context.ItemMasterDatas.FirstOrDefault(w => w.ID == item.ItemID);
                var orft = _context.GroupDUoMs.FirstOrDefault(w => w.GroupUoMID == item_master_data.GroupUomID && w.AltUOM == item.UomID);
                if (item_master_data.Process != "Standard")
                {
                    var item_warehouse_summary = _context.WarehouseSummary.FirstOrDefault(w => w.WarehouseID == Order.WarehouseID && w.ItemID == item.ItemID);
                    var all_item_warehouse_detail = _context.WarehouseDetails.Where(w => w.WarehouseID == Order.WarehouseID && w.ItemID == item.ItemID).ToList();
                    if (item_warehouse_summary != null)
                    {
                        item_warehouse_summary.Committed -= (double)item.Qty;
                        item_master_data.StockCommit -= (double)item.Qty;
                        //WerehouseSummary
                        item_warehouse_summary.InStock -= (double)item.Qty;
                        //Itemmasterdata
                        item_master_data.StockIn = item_warehouse_summary.InStock - (double)item.Qty;
                        _context.WarehouseSummary.Update(item_warehouse_summary);
                        _context.ItemMasterDatas.Update(item_master_data);
                    }
                    double @Check_Stock;
                    double @Remain;
                    double @IssusQty;
                    double @FIFOQty;
                    double @Qty = item.Qty * orft.Factor;
                    foreach (var item_warehouse in all_item_warehouse_detail.Where(w => w.InStock > 0))
                    {
                        var item_inventory_audit = new InventoryAudit();
                        var item_IssusStock = all_item_warehouse_detail.FirstOrDefault(w => w.InStock > 0);
                        @Check_Stock = item_warehouse.InStock - @Qty;
                        if (@Check_Stock < 0)
                        {
                            @Remain = (item_warehouse.InStock - @Qty) * (-1);
                            @IssusQty = @Qty - @Remain;
                            if (@Remain <= 0)
                            {
                                @Qty = 0;
                            }
                            else
                            {
                                @Qty = @Remain;
                            }
                            if (item_master_data.Process == "FIFO")
                            {
                                item_IssusStock.InStock = item_IssusStock.InStock -= @IssusQty;
                                if (@IssusQty > 0)
                                {
                                    var inventory_audit = _context.InventoryAudits.Where(w => w.ItemID == item.ItemID && w.UomID == item.UomID && w.WarehouseID == Order.WarehouseID);
                                    item_inventory_audit.ID = 0;
                                    item_inventory_audit.WarehouseID = Order.WarehouseID;
                                    item_inventory_audit.BranchID = Order.BranchID;
                                    item_inventory_audit.UserID = Order.UserOrderID;
                                    item_inventory_audit.ItemID = item.ItemID;
                                    item_inventory_audit.CurrencyID = Com.SystemCurrencyID;
                                    item_inventory_audit.UomID = orft.BaseUOM;
                                    item_inventory_audit.InvoiceNo = Order.ReceiptNo;
                                    item_inventory_audit.Trans_Type = docType.Code;
                                    item_inventory_audit.Process = item_master_data.Process;
                                    item_inventory_audit.SystemDate = DateTime.Now;
                                    item_inventory_audit.TimeIn = DateTime.Now.ToShortTimeString().ToString();
                                    item_inventory_audit.Qty = @IssusQty * -1;
                                    item_inventory_audit.Cost = item_IssusStock.Cost;
                                    item_inventory_audit.Price = 0;
                                    item_inventory_audit.CumulativeQty = inventory_audit.Sum(q => q.Qty) + (@IssusQty * (-1));
                                    item_inventory_audit.CumulativeValue = (inventory_audit.Sum(s => s.Trans_Valuse)) + (@IssusQty * item_IssusStock.Cost) * (-1);
                                    item_inventory_audit.Trans_Valuse = (@IssusQty * item_IssusStock.Cost) * (-1);
                                    item_inventory_audit.ExpireDate = item_IssusStock.ExpireDate;
                                    item_inventory_audit.LocalCurID = Order.LocalCurrencyID;
                                    item_inventory_audit.LocalSetRate = Order.LocalSetRate;
                                    item_inventory_audit.CompanyID = Order.CompanyID;
                                    item_inventory_audit.DocumentTypeID = docType.ID;
                                    item_inventory_audit.SeriesID = Order.SeriesID;
                                    item_inventory_audit.SeriesDetailID = Order.SeriesDID;
                                }
                                _context.WarehouseDetails.Update(item_IssusStock);
                                _context.InventoryAudits.Add(item_inventory_audit);
                                _context.SaveChanges();
                            }
                            else
                            {
                                item_IssusStock.InStock = item_IssusStock.InStock -= @IssusQty;
                                if (@IssusQty > 0)
                                {
                                    var warehouse_summary = _context.WarehouseSummary.FirstOrDefault(w => w.ItemID == item.ItemID && w.WarehouseID == Order.WarehouseID);
                                    double @sysAvCost = warehouse_summary.Cost;
                                    var inventory_audit = _context.InventoryAudits.Where(w => w.ItemID == item.ItemID && w.UomID == item.UomID && w.WarehouseID == Order.WarehouseID);
                                    item_inventory_audit.ID = 0;
                                    item_inventory_audit.WarehouseID = Order.WarehouseID;
                                    item_inventory_audit.BranchID = Order.BranchID;
                                    item_inventory_audit.UserID = Order.UserOrderID;
                                    item_inventory_audit.ItemID = item.ItemID;
                                    item_inventory_audit.CurrencyID = Com.SystemCurrencyID;
                                    item_inventory_audit.UomID = orft.BaseUOM;
                                    item_inventory_audit.InvoiceNo = Order.ReceiptNo;
                                    item_inventory_audit.Trans_Type = docType.Code;
                                    item_inventory_audit.Process = item_master_data.Process;
                                    item_inventory_audit.SystemDate = DateTime.Now;
                                    item_inventory_audit.TimeIn = DateTime.Now.ToShortTimeString().ToString();
                                    item_inventory_audit.Qty = @IssusQty * -1;
                                    item_inventory_audit.Cost = ((inventory_audit.Sum(s => s.Trans_Valuse)) + (@IssusQty * @sysAvCost) * (-1)) / (inventory_audit.Sum(q => q.Qty) + (@IssusQty * (-1)));
                                    item_inventory_audit.Price = 0;
                                    item_inventory_audit.CumulativeQty = inventory_audit.Sum(q => q.Qty) + (@IssusQty * (-1));
                                    item_inventory_audit.CumulativeValue = (inventory_audit.Sum(s => s.Trans_Valuse)) + (@IssusQty * @sysAvCost) * (-1);
                                    item_inventory_audit.Trans_Valuse = (@IssusQty * @sysAvCost) * (-1);
                                    item_inventory_audit.ExpireDate = item_IssusStock.ExpireDate;
                                    item_inventory_audit.LocalCurID = Order.LocalCurrencyID;
                                    item_inventory_audit.LocalSetRate = Order.LocalSetRate;
                                    item_inventory_audit.CompanyID = Order.CompanyID;
                                    item_inventory_audit.DocumentTypeID = docType.ID;
                                    item_inventory_audit.SeriesID = Order.SeriesID;
                                    item_inventory_audit.SeriesDetailID = Order.SeriesDID;
                                }
                                UpdateAvgCost(item_warehouse.ItemID, Order.WarehouseID, item_master_data.GroupUomID, item_inventory_audit.Qty, item_inventory_audit.Cost);
                                UpdateBomCost(item_warehouse.ItemID, item_inventory_audit.Qty, item_inventory_audit.Cost);
                                _context.WarehouseDetails.Update(item_IssusStock);
                                _context.InventoryAudits.Add(item_inventory_audit);
                                _context.SaveChanges();
                            }
                        }
                        else
                        {
                            @FIFOQty = item_IssusStock.InStock - @Qty;
                            @IssusQty = item_IssusStock.InStock - @FIFOQty;
                            if (item_master_data.Process == "FIFO")
                            {
                                item_IssusStock.InStock = @FIFOQty;
                                if (@IssusQty > 0)
                                {
                                    var inventory_audit = _context.InventoryAudits.Where(w => w.ItemID == item.ItemID && w.UomID == item.UomID && w.WarehouseID == Order.WarehouseID);
                                    item_inventory_audit.ID = 0;
                                    item_inventory_audit.WarehouseID = Order.WarehouseID;
                                    item_inventory_audit.BranchID = Order.BranchID;
                                    item_inventory_audit.UserID = Order.UserOrderID;
                                    item_inventory_audit.ItemID = item.ItemID;
                                    item_inventory_audit.CurrencyID = Com.SystemCurrencyID;
                                    item_inventory_audit.UomID = orft.BaseUOM;
                                    item_inventory_audit.InvoiceNo = Order.ReceiptNo;
                                    item_inventory_audit.Trans_Type = docType.Code;
                                    item_inventory_audit.Process = item_master_data.Process;
                                    item_inventory_audit.SystemDate = DateTime.Now;
                                    item_inventory_audit.TimeIn = DateTime.Now.ToShortTimeString().ToString();
                                    item_inventory_audit.Qty = @IssusQty * -1;
                                    item_inventory_audit.Cost = item_IssusStock.Cost;
                                    item_inventory_audit.Price = 0;
                                    item_inventory_audit.CumulativeQty = inventory_audit.Sum(q => q.Qty) + (@IssusQty * (-1));
                                    item_inventory_audit.CumulativeValue = (inventory_audit.Sum(s => s.Trans_Valuse)) + (@IssusQty * item_IssusStock.Cost) * (-1);
                                    item_inventory_audit.Trans_Valuse = (@IssusQty * item_IssusStock.Cost) * (-1);
                                    item_inventory_audit.ExpireDate = item_IssusStock.ExpireDate;
                                    item_inventory_audit.LocalCurID = Order.LocalCurrencyID;
                                    item_inventory_audit.LocalSetRate = Order.LocalSetRate;
                                    item_inventory_audit.CompanyID = Order.CompanyID;
                                    item_inventory_audit.DocumentTypeID = docType.ID;
                                    item_inventory_audit.SeriesID = Order.SeriesID;
                                    item_inventory_audit.SeriesDetailID = Order.SeriesDID;
                                }
                                _context.WarehouseDetails.Update(item_IssusStock);
                                _context.InventoryAudits.Add(item_inventory_audit);
                                _context.SaveChanges();
                            }
                            else
                            {
                                item_IssusStock.InStock = @FIFOQty;
                                if (@IssusQty > 0)
                                {
                                    var warehouse_summary = _context.WarehouseSummary.FirstOrDefault(w => w.ItemID == item.ItemID && w.WarehouseID == Order.WarehouseID);
                                    double @sysAvCost = warehouse_summary.Cost;
                                    var inventory_audit = _context.InventoryAudits.Where(w => w.ItemID == item.ItemID && w.UomID == item.UomID && w.WarehouseID == Order.WarehouseID);
                                    item_inventory_audit.ID = 0;
                                    item_inventory_audit.WarehouseID = Order.WarehouseID;
                                    item_inventory_audit.BranchID = Order.BranchID;
                                    item_inventory_audit.UserID = Order.UserOrderID;
                                    item_inventory_audit.ItemID = item.ItemID;
                                    item_inventory_audit.CurrencyID = Com.SystemCurrencyID;
                                    item_inventory_audit.UomID = orft.BaseUOM;
                                    item_inventory_audit.InvoiceNo = Order.ReceiptNo;
                                    item_inventory_audit.Trans_Type = docType.Code;
                                    item_inventory_audit.Process = item_master_data.Process;
                                    item_inventory_audit.SystemDate = DateTime.Now;
                                    item_inventory_audit.TimeIn = DateTime.Now.ToShortTimeString().ToString();
                                    item_inventory_audit.Qty = @IssusQty * -1;
                                    item_inventory_audit.Cost = ((inventory_audit.Sum(s => s.Trans_Valuse)) + (@IssusQty * @sysAvCost) * (-1)) / (inventory_audit.Sum(q => q.Qty) + (@IssusQty * (-1)));
                                    item_inventory_audit.Price = 0;
                                    item_inventory_audit.CumulativeQty = inventory_audit.Sum(q => q.Qty) + (@IssusQty * (-1));
                                    item_inventory_audit.CumulativeValue = (inventory_audit.Sum(s => s.Trans_Valuse)) + (@IssusQty * @sysAvCost) * (-1);
                                    item_inventory_audit.Trans_Valuse = (@IssusQty * @sysAvCost) * (-1);
                                    item_inventory_audit.ExpireDate = item_IssusStock.ExpireDate;
                                    item_inventory_audit.LocalCurID = Order.LocalCurrencyID;
                                    item_inventory_audit.LocalSetRate = Order.LocalSetRate;
                                    item_inventory_audit.CompanyID = Order.CompanyID;
                                    item_inventory_audit.DocumentTypeID = docType.ID;
                                    item_inventory_audit.SeriesID = Order.SeriesID;
                                    item_inventory_audit.SeriesDetailID = Order.SeriesDID;
                                }
                                UpdateAvgCost(item_warehouse.ItemID, Order.WarehouseID, item_master_data.GroupUomID, item_inventory_audit.Qty, item_inventory_audit.Cost);
                                UpdateBomCost(item_warehouse.ItemID, item_inventory_audit.Qty, item_inventory_audit.Cost);
                                _context.WarehouseDetails.Update(item_IssusStock);
                                _context.InventoryAudits.Add(item_inventory_audit);
                                _context.SaveChanges();
                            }
                            all_item_warehouse_detail = new List<WarehouseDetail>();
                            break;
                        }
                    }
                }
            }
            //IssuseInStockMaterial
            foreach (var item in OrderDetails.ToList())
            {
                var itemM = _context.ItemMasterDatas.FirstOrDefault(w => w.ID == item.ItemID);
                var orft = _context.GroupDUoMs.FirstOrDefault(w => w.GroupUoMID == itemM.GroupUomID && w.AltUOM == item.UomID);
                var bom = _context.BOMaterial.FirstOrDefault(w => w.ItemID == item.ItemID && w.Active == true) ?? new BOMaterial();
                var items_material = (from bomd in _context.BOMDetail.Where(w => w.BID == bom.BID && w.Detele == false)
                                      join i in _context.ItemMasterDatas on bomd.ItemID equals i.ID
                                      join gd in _context.GroupDUoMs on bomd.UomID equals gd.AltUOM
                                      join uom in _context.UnitofMeasures on i.InventoryUoMID equals uom.ID
                                      select new
                                      {
                                          bomd.ItemID,
                                          gd.GroupUoMID,
                                          GUoMID = i.GroupUomID,
                                          Qty = ((double)item.Qty * orft.Factor) * ((double)bomd.Qty * gd.Factor),
                                          bomd.NegativeStock,
                                          i.Process,
                                          uom.ID,
                                          gd.Factor
                                      }).Where(w => w.GroupUoMID == w.GUoMID).ToList();
                if (items_material != null)
                {
                    foreach (var item_detail in items_material.ToList())
                    {
                        var item_master_data = _context.ItemMasterDatas.FirstOrDefault(w => w.ID == item_detail.ItemID);
                        var item_warehouse_material = _context.WarehouseSummary.FirstOrDefault(w => w.WarehouseID == Order.WarehouseID && w.ItemID == item_detail.ItemID);
                        var all_item_warehouse_detail = _context.WarehouseDetails.Where(w => w.WarehouseID == Order.WarehouseID && w.ItemID == item_detail.ItemID).ToList();
                        var item_nagative = from wa in _context.WarehouseSummary.Where(w => w.ItemID == item_detail.ItemID)
                                            join na in _context.BOMDetail on wa.ItemID equals na.ItemID
                                            select new
                                            {
                                                NagaStock = wa.InStock
                                            };
                        var nagative_check = item_nagative.Sum(w => w.NagaStock);
                        //WerehouseSummary
                        item_warehouse_material.Committed -= (double)item_detail.Qty;
                        item_warehouse_material.InStock -= (double)item_detail.Qty;
                        //Itemmasterdata
                        item_master_data.StockIn -= (double)item_detail.Qty;
                        if (item_detail.NegativeStock == true && nagative_check <= 0)
                        {
                            double @IssusQty;
                            double @FIFOQty;
                            double @Qty = item_detail.Qty;
                            var item_inventory_audit = new InventoryAudit();
                            var item_IssusStock = all_item_warehouse_detail.LastOrDefault(w => w.InStock <= 0);
                            @FIFOQty = item_IssusStock.InStock - @Qty;
                            @IssusQty = item_IssusStock.InStock - @FIFOQty;
                            if (item_detail.Process == "FIFO")
                            {
                                item_IssusStock.InStock = @FIFOQty;
                                if (@IssusQty > 0)
                                {
                                    var inventory_audit = _context.InventoryAudits.Where(w => w.ItemID == item_detail.ItemID && w.UomID == item_detail.ID && w.WarehouseID == Order.WarehouseID);
                                    item_inventory_audit.ID = 0;
                                    item_inventory_audit.WarehouseID = Order.WarehouseID;
                                    item_inventory_audit.BranchID = Order.BranchID;
                                    item_inventory_audit.UserID = Order.UserOrderID;
                                    item_inventory_audit.ItemID = item_detail.ItemID;
                                    item_inventory_audit.CurrencyID = Com.SystemCurrencyID;
                                    item_inventory_audit.UomID = item_detail.ID;
                                    item_inventory_audit.InvoiceNo = Order.ReceiptNo;
                                    item_inventory_audit.Trans_Type = docType.Code;
                                    item_inventory_audit.Process = item_detail.Process;
                                    item_inventory_audit.SystemDate = DateTime.Now;
                                    item_inventory_audit.TimeIn = Order.TimeIn;
                                    item_inventory_audit.Qty = @IssusQty * -1;
                                    item_inventory_audit.Cost = item_IssusStock.Cost;
                                    item_inventory_audit.Price = 0;
                                    item_inventory_audit.CumulativeQty = inventory_audit.Sum(q => q.Qty) + (@IssusQty * (-1));
                                    item_inventory_audit.CumulativeValue = (inventory_audit.Sum(s => s.Trans_Valuse)) + (@IssusQty * item_IssusStock.Cost) * (-1);
                                    item_inventory_audit.Trans_Valuse = (@IssusQty * item_IssusStock.Cost) * (-1);
                                    item_inventory_audit.ExpireDate = item_IssusStock.ExpireDate;
                                    item_inventory_audit.LocalCurID = Com.LocalCurrencyID;
                                    item_inventory_audit.LocalSetRate = Exr.SetRate;
                                    item_inventory_audit.CompanyID = Order.CompanyID;
                                    item_inventory_audit.DocumentTypeID = docType.ID;
                                    item_inventory_audit.SeriesID = Order.SeriesID;
                                    item_inventory_audit.SeriesDetailID = Order.SeriesDID;
                                }
                            }
                            else
                            {
                                item_IssusStock.InStock = @FIFOQty;
                                if (@IssusQty > 0)
                                {
                                    var warehouse_summary = _context.WarehouseSummary.FirstOrDefault(w => w.ItemID == item_detail.ItemID && w.WarehouseID == Order.WarehouseID);
                                    double @sysAvCost = warehouse_summary.Cost;
                                    var inventory_audit = _context.InventoryAudits.Where(w => w.ItemID == item_detail.ItemID && w.UomID == item_detail.ID && w.WarehouseID == Order.WarehouseID);
                                    item_inventory_audit.ID = 0;
                                    item_inventory_audit.WarehouseID = Order.WarehouseID;
                                    item_inventory_audit.BranchID = Order.BranchID;
                                    item_inventory_audit.UserID = Order.UserOrderID;
                                    item_inventory_audit.ItemID = item_detail.ItemID;
                                    item_inventory_audit.CurrencyID = Com.SystemCurrencyID;
                                    item_inventory_audit.UomID = item_detail.ID;
                                    item_inventory_audit.InvoiceNo = Order.ReceiptNo;
                                    item_inventory_audit.Trans_Type = docType.Code;
                                    item_inventory_audit.Process = item_detail.Process;
                                    item_inventory_audit.SystemDate = DateTime.Now;
                                    item_inventory_audit.TimeIn = Order.TimeIn;
                                    item_inventory_audit.Qty = @IssusQty * -1;
                                    item_inventory_audit.Cost = ((inventory_audit.Sum(s => s.Trans_Valuse)) + (@IssusQty * @sysAvCost) * (-1)) / (inventory_audit.Sum(q => q.Qty) + (@IssusQty * (-1)));
                                    item_inventory_audit.Price = 0;
                                    item_inventory_audit.CumulativeQty = inventory_audit.Sum(q => q.Qty) + (@IssusQty * (-1));
                                    item_inventory_audit.CumulativeValue = (inventory_audit.Sum(s => s.Trans_Valuse)) + (@IssusQty * @sysAvCost) * (-1);
                                    item_inventory_audit.Trans_Valuse = (@IssusQty * @sysAvCost) * (-1);
                                    item_inventory_audit.ExpireDate = item_IssusStock.ExpireDate;
                                    item_inventory_audit.LocalCurID = Com.LocalCurrencyID;
                                    item_inventory_audit.LocalSetRate = Exr.SetRate;
                                    item_inventory_audit.CompanyID = Order.CompanyID;
                                    item_inventory_audit.DocumentTypeID = docType.ID;
                                    item_inventory_audit.SeriesID = Order.SeriesID;
                                    item_inventory_audit.SeriesDetailID = Order.SeriesDID;
                                }
                                UpdateAvgCost(item_detail.ItemID, Order.WarehouseID, item_master_data.GroupUomID, item_inventory_audit.Qty, item_inventory_audit.Cost);
                                UpdateBomCost(item_detail.ItemID, item_inventory_audit.Qty, item_inventory_audit.Cost);
                            }
                            _context.WarehouseDetails.Update(item_IssusStock);
                            _context.InventoryAudits.Add(item_inventory_audit);
                            _context.SaveChanges();

                        }
                        else
                        {
                            double @Check_Stock;
                            double @Remain;
                            double @IssusQty;
                            double @FIFOQty;
                            double @Qty = item_detail.Qty;
                            foreach (var item_warehouse in all_item_warehouse_detail.Where(w => w.InStock > 0))
                            {
                                var item_inventory_audit = new InventoryAudit();
                                var item_IssusStock = all_item_warehouse_detail.FirstOrDefault(w => w.InStock > 0);
                                @Check_Stock = item_warehouse.InStock - @Qty;
                                if (@Check_Stock < 0)
                                {
                                    @Remain = (item_warehouse.InStock - @Qty) * (-1);
                                    @IssusQty = @Qty - @Remain;
                                    if (@Remain <= 0)
                                    {
                                        @Qty = 0;
                                    }
                                    else
                                    {
                                        @Qty = @Remain;
                                    }
                                    if (item_detail.Process == "FIFO")
                                    {
                                        item_IssusStock.InStock = item_IssusStock.InStock -= @IssusQty;
                                        if (@IssusQty > 0)
                                        {
                                            var inventory_audit = _context.InventoryAudits.Where(w => w.ItemID == item_detail.ItemID && w.UomID == item_detail.ID && w.WarehouseID == Order.WarehouseID);
                                            item_inventory_audit.ID = 0;
                                            item_inventory_audit.WarehouseID = Order.WarehouseID;
                                            item_inventory_audit.BranchID = Order.BranchID;
                                            item_inventory_audit.UserID = Order.UserOrderID;
                                            item_inventory_audit.ItemID = item_detail.ItemID;
                                            item_inventory_audit.CurrencyID = Com.SystemCurrencyID;
                                            item_inventory_audit.UomID = item_detail.ID;
                                            item_inventory_audit.InvoiceNo = Order.ReceiptNo;
                                            item_inventory_audit.Trans_Type = docType.Code;
                                            item_inventory_audit.Process = item_detail.Process;
                                            item_inventory_audit.SystemDate = DateTime.Now;
                                            item_inventory_audit.TimeIn = Order.TimeIn;
                                            item_inventory_audit.Qty = @IssusQty * -1;
                                            item_inventory_audit.Cost = item_IssusStock.Cost;
                                            item_inventory_audit.Price = 0;
                                            item_inventory_audit.CumulativeQty = inventory_audit.Sum(q => q.Qty) + (@IssusQty * (-1));
                                            item_inventory_audit.CumulativeValue = (inventory_audit.Sum(s => s.Trans_Valuse)) + (@IssusQty * item_IssusStock.Cost) * (-1);
                                            item_inventory_audit.Trans_Valuse = (@IssusQty * item_IssusStock.Cost) * (-1);
                                            item_inventory_audit.ExpireDate = item_IssusStock.ExpireDate;
                                            item_inventory_audit.LocalCurID = Com.LocalCurrencyID;
                                            item_inventory_audit.LocalSetRate = Exr.SetRate;
                                            item_inventory_audit.CompanyID = Order.CompanyID;
                                            item_inventory_audit.DocumentTypeID = docType.ID;
                                            item_inventory_audit.SeriesID = Order.SeriesID;
                                            item_inventory_audit.SeriesDetailID = Order.SeriesDID;
                                        }
                                        _context.WarehouseDetails.Update(item_IssusStock);
                                        _context.InventoryAudits.Add(item_inventory_audit);
                                        _context.SaveChanges();
                                    }
                                    else
                                    {
                                        item_IssusStock.InStock = item_IssusStock.InStock -= @IssusQty;
                                        if (@IssusQty > 0)
                                        {
                                            var warehouse_summary = _context.WarehouseSummary.FirstOrDefault(w => w.ItemID == item.ItemID && w.WarehouseID == Order.WarehouseID);
                                            double @sysAvCost = warehouse_summary.Cost;
                                            var inventory_audit = _context.InventoryAudits.Where(w => w.ItemID == item_detail.ItemID && w.UomID == item_detail.ID && w.WarehouseID == Order.WarehouseID);
                                            item_inventory_audit.ID = 0;
                                            item_inventory_audit.WarehouseID = Order.WarehouseID;
                                            item_inventory_audit.BranchID = Order.BranchID;
                                            item_inventory_audit.UserID = Order.UserOrderID;
                                            item_inventory_audit.ItemID = item_detail.ItemID;
                                            item_inventory_audit.CurrencyID = Com.SystemCurrencyID;
                                            item_inventory_audit.UomID = item_detail.ID;
                                            item_inventory_audit.InvoiceNo = Order.ReceiptNo;
                                            item_inventory_audit.Trans_Type = docType.Code;
                                            item_inventory_audit.Process = item_detail.Process;
                                            item_inventory_audit.SystemDate = DateTime.Now;
                                            item_inventory_audit.TimeIn = Order.TimeIn;
                                            item_inventory_audit.Qty = @IssusQty * -1;
                                            item_inventory_audit.Cost = ((inventory_audit.Sum(s => s.Trans_Valuse)) + (@IssusQty * @sysAvCost) * (-1)) / (inventory_audit.Sum(q => q.Qty) + (@IssusQty * (-1)));
                                            item_inventory_audit.Price = 0;
                                            item_inventory_audit.CumulativeQty = inventory_audit.Sum(q => q.Qty) + (@IssusQty * (-1));
                                            item_inventory_audit.CumulativeValue = (inventory_audit.Sum(s => s.Trans_Valuse)) + (@IssusQty * @sysAvCost) * (-1);
                                            item_inventory_audit.Trans_Valuse = (@IssusQty * @sysAvCost) * (-1);
                                            item_inventory_audit.ExpireDate = item_IssusStock.ExpireDate;
                                            item_inventory_audit.LocalCurID = Com.LocalCurrencyID;
                                            item_inventory_audit.LocalSetRate = Exr.SetRate;
                                            item_inventory_audit.CompanyID = Order.CompanyID;
                                            item_inventory_audit.DocumentTypeID = docType.ID;
                                            item_inventory_audit.SeriesID = Order.SeriesID;
                                            item_inventory_audit.SeriesDetailID = Order.SeriesDID;
                                        }
                                        UpdateAvgCost(item.ItemID, Order.WarehouseID, item_master_data.GroupUomID, item_inventory_audit.Qty, item_inventory_audit.Cost);
                                        UpdateBomCost(item.ItemID, item_inventory_audit.Qty, item_inventory_audit.Cost);
                                        _context.WarehouseDetails.Update(item_IssusStock);
                                        _context.InventoryAudits.Add(item_inventory_audit);
                                        _context.SaveChanges();
                                    }
                                }
                                else
                                {
                                    @FIFOQty = item_IssusStock.InStock - @Qty;
                                    @IssusQty = item_IssusStock.InStock - @FIFOQty;
                                    if (item_detail.Process == "FIFO")
                                    {
                                        item_IssusStock.InStock = @FIFOQty;
                                        if (@IssusQty > 0)
                                        {
                                            var inventory_audit = _context.InventoryAudits.Where(w => w.ItemID == item_detail.ItemID && w.UomID == item_detail.ID && w.WarehouseID == Order.WarehouseID);
                                            item_inventory_audit.ID = 0;
                                            item_inventory_audit.WarehouseID = Order.WarehouseID;
                                            item_inventory_audit.BranchID = Order.BranchID;
                                            item_inventory_audit.UserID = Order.UserOrderID;
                                            item_inventory_audit.ItemID = item_detail.ItemID;
                                            item_inventory_audit.CurrencyID = Com.SystemCurrencyID;
                                            item_inventory_audit.UomID = item_detail.ID;
                                            item_inventory_audit.InvoiceNo = Order.ReceiptNo;
                                            item_inventory_audit.Trans_Type = docType.Code;
                                            item_inventory_audit.Process = item_detail.Process;
                                            item_inventory_audit.SystemDate = DateTime.Now;
                                            item_inventory_audit.TimeIn = Order.TimeIn;
                                            item_inventory_audit.Qty = @IssusQty * -1;
                                            item_inventory_audit.Cost = item_IssusStock.Cost;
                                            item_inventory_audit.Price = 0;
                                            item_inventory_audit.CumulativeQty = inventory_audit.Sum(q => q.Qty) + (@IssusQty * (-1));
                                            item_inventory_audit.CumulativeValue = (inventory_audit.Sum(s => s.Trans_Valuse)) + (@IssusQty * item_IssusStock.Cost) * (-1);
                                            item_inventory_audit.Trans_Valuse = (@IssusQty * item_IssusStock.Cost) * (-1);
                                            item_inventory_audit.ExpireDate = item_IssusStock.ExpireDate;
                                            item_inventory_audit.LocalCurID = Com.LocalCurrencyID;
                                            item_inventory_audit.LocalSetRate = Exr.SetRate;
                                            item_inventory_audit.CompanyID = Order.CompanyID;
                                            item_inventory_audit.DocumentTypeID = docType.ID;
                                            item_inventory_audit.SeriesID = Order.SeriesID;
                                            item_inventory_audit.SeriesDetailID = Order.SeriesDID;
                                        }
                                        _context.WarehouseDetails.Update(item_IssusStock);
                                        _context.InventoryAudits.Add(item_inventory_audit);
                                        _context.SaveChanges();
                                    }
                                    else
                                    {
                                        item_IssusStock.InStock = @FIFOQty;
                                        if (@IssusQty > 0)
                                        {
                                            var warehouse_summary = _context.WarehouseSummary.FirstOrDefault(w => w.ItemID == item_detail.ItemID && w.WarehouseID == Order.WarehouseID);
                                            double @sysAvCost = warehouse_summary.Cost;
                                            var inventory_audit = _context.InventoryAudits.Where(w => w.ItemID == item_detail.ItemID && w.UomID == item_detail.ID && w.WarehouseID == Order.WarehouseID);
                                            item_inventory_audit.ID = 0;
                                            item_inventory_audit.WarehouseID = Order.WarehouseID;
                                            item_inventory_audit.BranchID = Order.BranchID;
                                            item_inventory_audit.UserID = Order.UserOrderID;
                                            item_inventory_audit.ItemID = item_detail.ItemID;
                                            item_inventory_audit.CurrencyID = Com.SystemCurrencyID;
                                            item_inventory_audit.UomID = item_detail.ID;
                                            item_inventory_audit.InvoiceNo = Order.ReceiptNo;
                                            item_inventory_audit.Trans_Type = docType.Code;
                                            item_inventory_audit.Process = item_detail.Process;
                                            item_inventory_audit.SystemDate = DateTime.Now;
                                            item_inventory_audit.TimeIn = Order.TimeIn;
                                            item_inventory_audit.Qty = @IssusQty * -1;
                                            item_inventory_audit.Cost = ((inventory_audit.Sum(s => s.Trans_Valuse)) + (@IssusQty * @sysAvCost) * (-1)) / (inventory_audit.Sum(q => q.Qty) + (@IssusQty * (-1)));
                                            item_inventory_audit.Price = 0;
                                            item_inventory_audit.CumulativeQty = inventory_audit.Sum(q => q.Qty) + (@IssusQty * (-1));
                                            item_inventory_audit.CumulativeValue = (inventory_audit.Sum(s => s.Trans_Valuse)) + (@IssusQty * @sysAvCost) * (-1);
                                            item_inventory_audit.Trans_Valuse = (@IssusQty * @sysAvCost) * (-1);
                                            item_inventory_audit.ExpireDate = item_IssusStock.ExpireDate;
                                            item_inventory_audit.LocalCurID = Com.LocalCurrencyID;
                                            item_inventory_audit.LocalSetRate = Exr.SetRate;
                                            item_inventory_audit.CompanyID = Order.CompanyID;
                                            item_inventory_audit.DocumentTypeID = docType.ID;
                                            item_inventory_audit.SeriesID = Order.SeriesID;
                                            item_inventory_audit.SeriesDetailID = Order.SeriesDID;
                                        }
                                        UpdateAvgCost(item_detail.ItemID, Order.WarehouseID, item_master_data.GroupUomID, item_inventory_audit.Qty, item_inventory_audit.Cost);
                                        UpdateBomCost(item_detail.ItemID, item_inventory_audit.Qty, item_inventory_audit.Cost);
                                        _context.WarehouseDetails.Update(item_IssusStock);
                                        _context.InventoryAudits.Add(item_inventory_audit);
                                        _context.SaveChanges();
                                    }
                                    all_item_warehouse_detail = new List<WarehouseDetail>();
                                    break;
                                }
                            }
                        }
                        _context.WarehouseSummary.Update(item_warehouse_material);
                        _context.ItemMasterDatas.Update(item_master_data);
                        _context.SaveChanges();
                    }
                }

            }
        }

        private bool CreateAgingPaymentCustomer(ReceiptKvms data)
        {
            string currencyName = _context.Currency.Find(data.PLCurrencyID).Description;
            SystemCurrency syCurrency = GetSystemCurrencies().FirstOrDefault();
            AgingPaymentCustomer aging = new()
            {
                CustomerID = data.CustomerID,
                BranchID = data.BranchID,
                WarehouseID = data.WarehouseID,
                CurrencyID = data.PLCurrencyID,
                DocumentNo = data.ReceiptNo,
                DocumentType = data.ReceiptNo.Substring(0, 3),
                Applied_Amount = data.AppliedAmount,
                CurrencyName = currencyName,
                ExchangeRate = data.ExchangeRate,
                Cash = (data.GrandTotal - data.AppliedAmount),
                DiscountRate = data.DiscountRate,
                Total = data.Sub_Total,
                DiscountValue = data.DiscountValue,
                BalanceDue = (data.GrandTotal - data.AppliedAmount),
                TotalPayment = data.GrandTotal,
                Status = data.Status,
                Date = data.DateOut,
                PostingDate = data.DateOut,
                SysCurrency = syCurrency.ID,
                SysName = syCurrency.Description,
                LocalCurID = data.LocalCurrencyID,
                LocalSetRate = data.LocalSetRate
            };

            if (aging.TotalPayment <= aging.Applied_Amount)
            {
                aging.BalanceDue = 0;
                aging.Cash = 0;
            }

            var payment = _context.AgingPaymentCustomer.FirstOrDefault(p => p.DocumentNo == data.ReceiptNo);
            if (payment != null)
            {
                payment.Applied_Amount = data.AppliedAmount;
                payment.BalanceDue = data.GrandTotal - data.AppliedAmount;
                payment.Cash = data.GrandTotal - data.AppliedAmount;
                payment.Status = data.Status;
                var paymentDetails = _context.AgingPaymentDetails.Where(ipd => ipd.DocumentNo == payment.DocumentNo);
                foreach (var pd in paymentDetails)
                {
                    pd.Delete = true;
                }
                _context.AgingPaymentCustomer.Update(payment);
            }
            else
            {
                _context.AgingPaymentCustomer.Add(aging);
            }

            _context.SaveChanges();
            return true;
        }
        private IEnumerable<SystemCurrency> GetSystemCurrencies()
        {
            IEnumerable<SystemCurrency> currencies =
                                        (from com in _context.Company.Where(x => x.Delete == false)
                                         join c in _context.Currency.Where(x => x.Delete == false) on com.SystemCurrencyID equals c.ID
                                         select new SystemCurrency
                                         {
                                             ID = c.ID,
                                             Description = c.Description
                                         }).ToList();
            return currencies;
        }

        //End of VMC Edition
        public void PrintReceiptBill(int orderid, string print_type)
        {
            //Generate receipt id
            var receipt_generated = "";
            var ordered = _context.Order.Include(x => x.OrderDetail).Include(x => x.Currency).FirstOrDefault(w => w.OrderID == orderid);
            if (ordered == null) { return; }
            if (print_type == "Pay")
            {
                var receipts = _context.Order_Receipt.Where(w => w.BranchID == ordered.BranchID);
                receipt_generated = (receipts.Count() + 1).ToString().PadLeft(6, '0');//DateTime.Now.Year + "" + DateTime.Now.Month + "" + DateTime.Now.Day + "" + (receipts.Count() + 1).ToString().PadLeft(6, '0');
                Order_Receipt NewReceipt = new()
                {
                    BranchID = ordered.BranchID,
                    ReceiptID = receipt_generated,
                    DateTime = DateTime.Now

                };
                _context.Order_Receipt.Add(NewReceipt);
                _context.SaveChanges();
            }

            //Update check bill order
            var order = _context.Order.Include(x => x.OrderDetail).Include(x => x.Currency).FirstOrDefault(w => w.OrderID == orderid);
            order.CheckBill = 'Y';
            order.ReceiptNo = receipt_generated;
            order.DateOut = DateTime.Today;
            order.TimeOut = DateTime.Now.ToShortTimeString();
            _context.Order.Update(order);
            _context.SaveChanges();

            if (order.OrderDetail.Count > 0)
            {
                //Pirint bill or tender
                var table = _context.Tables.Find(order.TableID);
                var user = _context.UserAccounts.Include(emp => emp.Employee).FirstOrDefault(w => w.ID == order.UserOrderID);
                var setting = _context.GeneralSettings.Where(w => w.UserID == order.UserOrderID).ToList();
                var customer = _context.BusinessPartners.FirstOrDefault(w => w.Delete == false && w.Type == "Customer" && w.ID == order.CustomerID);
                var syscurrency = _context.Currency.FirstOrDefault(w => w.ID == order.SysCurrencyID);
                var banch = _context.Branches.FirstOrDefault(w => w.Delete == false && w.ID == order.BranchID);
                var receipt = _context.ReceiptInformation.FirstOrDefault(w => w.BranchID == order.BranchID);
                var payment = _context.PaymentMeans.FirstOrDefault(w => w.ID == order.PaymentMeansID);
                string paymentType = "";
                if (payment != null)
                {
                    paymentType = payment.Type;
                }
                List<PrintBill> PrintBill = new();
                string ReceiptNo = "";
                var Received = "";
                var GrandTotal_Dis = "";
                var Change = "";
                var ChangeSys = "";
                foreach (var item in order.OrderDetail)
                {
                    var uom = _context.UnitofMeasures.First(u => u.ID == item.UomID);
                    if (item.Qty <= 0)
                    {
                        continue;
                    }
                    if (item.TypeDis == "Cash")
                    {
                        order.DiscountRate = order.DiscountValue;
                    }
                    if (setting[0].VatAble == true)
                    {
                        ReceiptNo = order.ReceiptNo + " " + setting[0].VatNum;
                    }
                    else
                    {
                        ReceiptNo = order.ReceiptNo;
                    }
                    if (order.CurrencyDisplay == "KHR" || order.CurrencyDisplay == "៛")
                    {
                        GrandTotal_Dis = order.CurrencyDisplay + " " + string.Format("{0:n0}", order.GrandTotal_Display);
                        ChangeSys = order.CurrencyDisplay + " " + string.Format("{0:n0}", order.Change_Display);
                    }
                    else
                    {
                        GrandTotal_Dis = order.CurrencyDisplay + " " + order.GrandTotal_Display.ToString("n3")[0..^1];
                        ChangeSys = order.CurrencyDisplay + " " + order.Change_Display.ToString("n3")[0..^1];
                    }
                    if (item.Currency == "KHR" || item.Currency == "៛")
                    {
                        Received = item.Currency + " " + string.Format("{0:n0}", order.Received);
                        Change = item.Currency + " " + string.Format("{0:n0}", order.Change);
                    }
                    else
                    {
                        Received = item.Currency + " " + order.Received.ToString("n3")[0..^1];
                        Change = item.Currency + " " + order.Change.ToString("n3")[0..^1];
                    }
                    if (item.Currency == "KHR" || item.Currency == "៛")
                    {
                        PrintBill bill = new()
                        {
                            OrderID = item.OrderID,
                            ReceiptNo = ReceiptNo,
                            Logo = receipt.Logo,
                            BranchName = receipt.Title,
                            Address = receipt.Address,
                            Tel1 = receipt.Tel1,
                            Tel2 = receipt.Tel2,
                            Table = table.Name,
                            OrderNo = order.OrderNo,
                            Cashier = user.Employee.Name,
                            DateTimeIn = order.DateIn.ToString("dd-MM-yyyy") + " " + order.TimeIn,
                            DateTimeOut = order.DateOut.ToString("dd-MM-yyyy") + " " + order.TimeOut,
                            Item = item.KhmerName,
                            Qty = item.Qty + " " + uom.Name,
                            Price = string.Format("{0:n0}", item.UnitPrice),
                            DisItem = item.DiscountRate + "%",
                            Amount = string.Format("{0:n0}", item.Total),
                            SubTotal = string.Format("{0:n0}", order.Sub_Total),
                            DisRate = order.DiscountRate + "%",
                            DisValue = string.Format("{0:n0}", order.DiscountValue),
                            TypeDis = order.TypeDis,
                            GrandTotal = item.Currency + " " + string.Format("{0:n0}", order.GrandTotal),
                            GrandTotalSys = GrandTotal_Dis,
                            VatRate = order.TaxRate + "%",
                            VatValue = item.Currency + " " + string.Format("{0:n0}", order.TaxValue),
                            Received = Received,
                            Change = Change,
                            ChangeSys = ChangeSys,
                            DescKh = receipt.KhmerDescription,
                            DescEn = receipt.EnglishDescription,
                            ExchangeRate = string.Format("{0:n0}", order.ExchangeRate),
                            Printer = "",
                            Print = print_type,
                            ItemDesc = item.Description,
                            CustomerInfo = customer.Code + '/' + customer.Name + '/' + customer.Phone + '/' + customer.Address + '/' + customer.Email,
                            Team = receipt.TeamCondition,
                            ItemType = item.ItemType,
                            PaymentMeans = paymentType
                        };
                        //Qty
                        if (item.ItemType == "Service")
                        {
                            var arr_time = item.Qty.ToString().Split('.');
                            bill.Qty = arr_time[0] + "h:" + arr_time[1].PadRight(2, '0') + "m";
                        }
                        PrintBill.Add(bill);
                    }
                    else
                    {
                        PrintBill bill = new()
                        {
                            OrderID = item.OrderID,
                            ReceiptNo = ReceiptNo,
                            Logo = receipt.Logo,
                            BranchName = receipt.Title,
                            Address = receipt.Address,
                            Tel1 = receipt.Tel1,
                            Tel2 = receipt.Tel2,
                            Table = table.Name,
                            OrderNo = order.OrderNo,
                            Cashier = user.Employee.Name,
                            DateTimeIn = order.DateIn.ToString("dd-MM-yyyy") + " " + order.TimeIn,
                            DateTimeOut = order.DateOut.ToString("dd-MM-yyyy") + " " + order.TimeOut,
                            Item = item.KhmerName,
                            Qty = item.Qty + " " + uom.Name,
                            Price = item.UnitPrice.ToString("n3")[0..^1],
                            DisItem = item.DiscountRate + "",
                            Amount = item.Total.ToString("n3")[0..^1],
                            SubTotal = order.Sub_Total.ToString("n3")[0..^1],
                            DisRate = order.DiscountRate + "%",
                            DisValue = order.DiscountValue.ToString("n3")[0..^1],
                            TypeDis = order.TypeDis,
                            GrandTotal = item.Currency + " " + order.GrandTotal.ToString("n3")[0..^1],
                            GrandTotalSys = GrandTotal_Dis,
                            VatRate = order.TaxRate + "%",
                            VatValue = item.Currency + " " + order.TaxValue.ToString("n3")[0..^1],
                            Received = Received,
                            Change = Change,
                            ChangeSys = ChangeSys,
                            DescKh = receipt.KhmerDescription,
                            DescEn = receipt.EnglishDescription,
                            ExchangeRate = order.ExchangeRate.ToString("n3")[0..^1],
                            Printer = "",
                            Print = print_type,
                            ItemDesc = item.Description,
                            CustomerInfo = customer.Code + '/' + customer.Name + '/' + customer.Phone + '/' + customer.Address + '/' + customer.Email,
                            Team = receipt.TeamCondition,
                            ItemType = item.ItemType,
                            PaymentMeans = paymentType

                        };
                        //Qty
                        if (item.ItemType == "Service")
                        {
                            var arr_time = item.Qty.ToString().Split('.');
                            bill.Qty = arr_time[0] + "h:" + arr_time[1].PadRight(2, '0') + "m";
                        }
                        PrintBill.Add(bill);
                    }

                }
                var ip = _accessor.ActionContext.HttpContext.Connection.RemoteIpAddress.ToString();
                _timeDelivery.PrintBill(PrintBill, setting, ip);
                //Update status table
                if (print_type == "Bill")
                {
                    var table_update = _context.Tables.Find(order.TableID);
                    table_update.Status = 'P';
                    table_update.Time = _timeDelivery.StopTimeTable(order.TableID, 'P');
                    _timeDelivery.ResetTimeTable(order.TableID, 'P', table_update.Time);
                    _context.Tables.Update(table_update);
                    _context.SaveChanges();
                }
                else if (print_type == "Pay")
                {
                    var count_order = _context.Order.Where(w => w.TableID == ordered.TableID && w.Cancel == false);
                    if (count_order.Count() == 1)
                    {
                        var table_update = _context.Tables.Find(order.TableID);
                        table_update.Status = 'A';
                        table_update.Time = "00:00:00";
                        _timeDelivery.StopTimeTable(order.TableID, 'A');
                        _timeDelivery.ResetTimeTable(order.TableID, 'A', "00:00:00");
                        _context.Tables.Update(table_update);
                        _context.SaveChanges();
                    }
                    //Issuse In Stock 
                    IssuseInStockOrder(orderid);
                    //Issuse In Stock Material
                    IssuseInStockMaterial(orderid);
                    //Remove order
                    _context.Database.ExecuteSqlCommand("pos_InsertReceipt @OrderID={0}",
                        parameters: new[] {
                    orderid.ToString()
                   });

                    GetOrder(order.TableID, order.OrderID, order.UserOrderID);
                }
                _timeDelivery.ClearUserOrder(order.TableID);
            }

        }
        public IEnumerable<ItemGroup3> GetGroups()
        {
            IEnumerable<ItemGroup3> list = from group1 in _context.ItemGroup1.Where(w => w.Delete == false)
                                           join group2 in _context.ItemGroup2.Where(w => w.Delete == false) on group1.ItemG1ID equals group2.ItemG1ID
                                           join group3 in _context.ItemGroup3.Where(w => w.Delete == false) on group2.ItemG2ID equals group3.ItemG2ID

                                           select new ItemGroup3
                                           {
                                               ID = group3.ID,
                                               Name = group3.Name,
                                               Images = group3.Images,
                                               ItemGroup1 = new ItemGroup1
                                               {
                                                   ItemG1ID = group1.ItemG1ID,
                                                   Name = group1.Name,
                                                   Images = group1.Images
                                               },
                                               ItemGroup2 = new ItemGroup2
                                               {
                                                   ItemG2ID = group2.ItemG2ID,
                                                   Name = group2.Name,
                                                   Images = group2.Images
                                               }

                                           };
            return list;
        }
        public IEnumerable<ItemGroup1> GetGroup1s => _context.ItemGroup1.Where(w => w.Delete == false && w.Visible == false).ToList();
        public IEnumerable<ItemGroup2> GetGroup2s => _context.ItemGroup2.Where(w => w.Delete == false).ToList();
        public IEnumerable<ItemGroup3> GetGroup3s => _context.ItemGroup3.Where(w => w.Delete == false).ToList();
        public IEnumerable<ItemGroup2> FilterGroup2(int group1_id)
        {
            var group2 = _context.ItemGroup2.Where(w => w.Delete == false && w.ItemG1ID == group1_id && w.Name != "None").ToList();
            return group2;

        }
        public IEnumerable<ItemGroup3> FilterGroup3(int group1_id, int group2_id)
        {
            var group3 = _context.ItemGroup3.Where(w => w.Delete == false && w.ItemG1ID == group1_id && w.ItemG2ID == group2_id && w.Name != "None").ToList();
            return group3;

        }
        public IEnumerable<ServiceItemSales> FilterItem(int pricelist_id, int itemid)
        {
            var item = _context.ServiceItemSales.FromSql("pos_FilterItem @PricelistID={0},@ItemID={1}",
                parameters: new[] {
                    pricelist_id.ToString(),
                    itemid.ToString()
                });
            return item.ToList();
        }
        public IEnumerable<ServiceItemSales> GetItemMasterDatas(int priceList_id)
        {
            var item = _context.ServiceItemSales.FromSql("pos_GetItemForSale @PricelistID={0}",
                parameters: new[] {
                    priceList_id.ToString()
                });
            return item;
        }
        //Scale
        public IEnumerable<ServiceItemSales> GetScaleItemMasterByBarcode(int pricelist, string scale_barcode, double scaleprice)
        {
            try
            {
                var item = _context.ServiceItemSales.FromSql("pos_FilterScaleItemByBarcode @PricelistID={0}, @Barcode={1}, @ScalePrice={2}",
                parameters: new[] {
                    pricelist.ToString(),
                    scale_barcode.ToString(),
                    scaleprice.ToString(),

                });
                return item;
            }
            catch (Exception)
            {
                return null;
            }

        }
        public IEnumerable<ServiceItemSales> GetItemMasterByBarcode(int pricelist, string barcode)
        {
            try
            {
                var item = _context.ServiceItemSales.FromSql("pos_FilterItemByBarcode @PricelistID={0}, @Barcode={1}",
                parameters: new[] {
                    pricelist.ToString(),
                    barcode.ToString(),

                });
                return item;
            }
            catch (Exception)
            {
                return null;
            }

        }
        public IEnumerable<GeneralSetting> GetSetting(int branchid)
        {
            var setting = _context.GeneralSettings.Where(w => w.BranchID == branchid).ToList();
            return setting;
        }
        public void SendSplit(Order data, Order addnew)//error not check
        {
            //update or remove old item
            foreach (var item in data.OrderDetail.ToList())
            {
                if (item.Qty <= 0)
                {
                    _context.OrderDetail.Remove(item);
                    data.OrderDetail.ToList().Remove(item);
                    _context.SaveChanges();
                }
            }
            _context.Order.Update(data);
            _context.SaveChanges();
            var remove = _context.OrderDetail.Where(w => w.OrderID == data.OrderID);
            if (!remove.Any())
            {
                var master = _context.Order.FirstOrDefault(w => w.OrderID == data.OrderID);
                _context.Order.Remove(master);
                _context.SaveChanges();
            }

            addnew.OrderDetail.ToList().RemoveAll(w => w.PrintQty <= 0);
            if (addnew.OrderDetail.ToList().Count > 0)
            {
                AddNewSplitOrder(addnew);
            }
        }
        public void AddNewSplitOrder(Order order)
        {
            var setting = _context.GeneralSettings.FirstOrDefault(w => w.BranchID == order.BranchID);
            var queues = _context.Order_Queue.Where(w => w.BranchID == order.BranchID);
            var receipt = _context.Order_Receipt.Where(w => w.BranchID == order.BranchID);

            if (queues.Count() >= setting.QueueCount)
            {
                foreach (var queue in queues.ToList())
                {
                    _context.Remove(queue);
                    _context.SaveChanges();
                }
            }
            Order orderNew = new();
            orderNew.OrderNo = "Split-" + (queues.Count() + 1);
            orderNew.TableID = order.TableID;
            orderNew.ReceiptNo = order.ReceiptNo;
            orderNew.QueueNo = (queues.Count() + 1).ToString();
            orderNew.DateIn = order.DateIn;
            orderNew.TimeIn = order.TimeIn;
            orderNew.DateOut = Convert.ToDateTime(DateTime.Today);
            orderNew.TimeOut = DateTime.Now.ToString("hh:mm:ss tt");
            orderNew.WaiterID = order.WaiterID;
            orderNew.UserOrderID = order.UserDiscountID;
            orderNew.UserDiscountID = order.UserDiscountID;
            orderNew.CustomerID = order.CustomerID;
            orderNew.PriceListID = order.PriceListID;
            orderNew.LocalCurrencyID = order.LocalCurrencyID;
            orderNew.SysCurrencyID = order.SysCurrencyID;
            orderNew.ExchangeRate = order.ExchangeRate;
            orderNew.WarehouseID = order.WarehouseID;
            orderNew.BranchID = order.BranchID;
            orderNew.CompanyID = order.CompanyID;
            orderNew.Sub_Total = 0;
            orderNew.DiscountRate = order.DiscountRate;
            orderNew.DiscountValue = 0;
            orderNew.TypeDis = order.TypeDis;
            orderNew.TaxRate = 0;
            orderNew.TaxValue = 0;
            orderNew.GrandTotal = 0;
            orderNew.GrandTotal_Sys = 0;
            orderNew.Tip = 0;
            orderNew.Received = 0;
            orderNew.Change = 0;
            orderNew.PaymentMeansID = order.PaymentMeansID;
            orderNew.CheckBill = 'N';
            orderNew.Cancel = false;
            orderNew.Delete = false;
            _context.Order.Add(orderNew);
            _context.SaveChanges();
            int orderid_new = orderNew.OrderID;
            //Add new order queue
            AddQueueOrder(orderNew.BranchID, orderNew.OrderNo);
            //Detail
            double SubTotal = 0;
            foreach (var item in order.OrderDetail.ToList())
            {
                OrderDetail detail = new();
                detail.OrderID = orderid_new;
                detail.Line_ID = item.Line_ID;
                detail.ItemID = item.ItemID;
                detail.Code = item.Code;
                detail.KhmerName = item.KhmerName;
                detail.EnglishName = item.EnglishName;
                detail.Qty = item.PrintQty;
                detail.PrintQty = 0;
                detail.UnitPrice = item.UnitPrice;
                detail.Cost = item.Cost;
                detail.DiscountRate = item.DiscountRate;
                detail.TypeDis = item.TypeDis;
                if (item.TypeDis == "Percent")
                {
                    detail.Total = (detail.Qty * detail.UnitPrice) * (1 - (detail.DiscountRate / 100));
                    detail.DiscountValue = (detail.Qty * item.UnitPrice) * detail.DiscountRate / 100;

                }
                else
                {
                    detail.Total = (detail.Qty * detail.UnitPrice) - detail.DiscountRate;
                    detail.DiscountValue = detail.DiscountRate;
                }
                detail.Total_Sys = detail.Total * orderNew.ExchangeRate;
                detail.UomID = item.UomID;
                detail.ItemStatus = item.ItemStatus;
                detail.ItemPrintTo = item.ItemPrintTo;
                detail.Currency = item.Currency;
                detail.ItemType = item.ItemType;
                SubTotal += detail.Total;
                _context.Add(detail);
                _context.SaveChanges();
            }
            //Update summary
            var order_master = _context.Order.Find(orderid_new);
            order_master.Sub_Total = SubTotal;
            if (order_master.TypeDis == "Percent")
            {
                order_master.DiscountValue = SubTotal * orderNew.DiscountRate / 100;
            }
            else
            {
                order_master.DiscountValue = orderNew.DiscountRate;
            }

            var vat = (order_master.TaxRate + 100 / 100);
            var rate = order_master.TaxRate / 100;
            order_master.TaxValue = (SubTotal / vat) * rate;
            order_master.GrandTotal = SubTotal - order_master.DiscountValue;
            order_master.GrandTotal_Sys = order_master.GrandTotal * order_master.ExchangeRate;

            _context.Update(order_master);
            _context.SaveChanges();

        }
        public void AddQueueOrder(int branchid, string orderno)
        {
            Order_Queue queue = new()
            {
                BranchID = branchid,
                OrderNo = orderno,
                DateTime = Convert.ToDateTime(DateTime.Now.ToString())
            };
            _context.Add(queue);
            _context.SaveChanges();

        }
        public void SendToPrintOrder(Order order)
        {
            var Table = _context.Tables.Find(order.TableID);
            var User = _context.UserAccounts.Include(emp => emp.Employee).FirstOrDefault(user => user.ID == order.UserOrderID);
            var PrinterNames = _context.PrinterNames.Where(w => w.Delete == false).ToList();
            var Setting = _context.GeneralSettings.Where(w => w.UserID == order.UserOrderID).ToList();
            List<PrintOrder> items = new();
            foreach (var item in order.OrderDetail.ToList())
            {
                var uom = _context.UnitofMeasures.First(u => u.ID == item.UomID);
                if (item.Qty <= 0)
                {
                    var remove = _context.OrderDetail.FirstOrDefault(w => w.Line_ID == item.Line_ID && w.OrderID == order.OrderID);
                    if (remove != null)
                    {
                        _context.Remove(remove);
                        _context.SaveChanges();
                    }
                    if (item.ItemType == "Service")
                    {
                        var time_continue = _timeDelivery.GetTimeTable(order.TableID);
                        var time = _timeDelivery.StartTimeTable(order.TableID, time_continue, 'B');
                        var table_up = _context.Tables.Find(order.TableID);
                        table_up.Status = 'B';
                        table_up.Time = time_continue;
                        _context.Update(table_up);
                        _context.SaveChanges();
                    }
                }
                if (item.ItemStatus == "old")
                {
                    if (item.PrintQty != 0)
                    {
                        if (item.Comment == null || item.Comment == "")
                        {
                            PrintOrder data = new()
                            {
                                Table = Table.Name,
                                Cashier = User.Employee.Name,
                                OrderNo = order.OrderNo,
                                Item = item.KhmerName,
                                PrintQty = item.PrintQty + " " + uom.Name,
                                ItemPrintTo = item.ItemPrintTo,
                                ParentLineID = item.ParentLevel,
                                ItemType = item.ItemType,
                                Price = item.Currency + ' ' + string.Format("{0:n2}", item.UnitPrice),
                                //Uom = item.UnitofMeansure.Name
                            };
                            items.Add(data);
                        }
                        else
                        {
                            PrintOrder data = new()
                            {
                                Table = Table.Name,
                                Cashier = User.Employee.Name,
                                OrderNo = order.OrderNo,
                                Item = item.KhmerName + " (" + item.Comment + ")",
                                PrintQty = item.PrintQty + " " + uom.Name,
                                ItemPrintTo = item.ItemPrintTo,
                                ParentLineID = item.ParentLevel,
                                ItemType = item.ItemType,
                                Price = item.Currency + ' ' + string.Format("{0:n2}", item.UnitPrice),
                                //Uom = item.UnitofMeansure.Name
                            };
                            items.Add(data);
                        }
                    }
                }
                else
                {
                    if (item.Comment == null || item.Comment == "")
                    {
                        PrintOrder data = new()
                        {
                            Table = Table.Name,
                            Cashier = User.Employee.Name,
                            OrderNo = order.OrderNo,
                            Item = item.KhmerName,
                            PrintQty = item.PrintQty + " " + uom.Name,
                            ItemPrintTo = item.ItemPrintTo,
                            ParentLineID = item.ParentLevel,
                            ItemType = item.ItemType,
                            Price = item.Currency + ' ' + string.Format("{0:n2}", item.UnitPrice),
                            //Uom = item.UnitofMeansure.Name
                        };
                        items.Add(data);
                    }
                    else
                    {
                        PrintOrder data = new()
                        {
                            Table = Table.Name,
                            Cashier = User.Employee.Name,
                            OrderNo = order.OrderNo,
                            Item = item.KhmerName + " (" + item.Comment + ")",
                            PrintQty = item.PrintQty + " " + uom.Name,
                            ItemPrintTo = item.ItemPrintTo,
                            ParentLineID = item.ParentLevel,
                            ItemType = item.ItemType,
                            Price = item.Currency + ' ' + string.Format("{0:n2}", item.UnitPrice),
                            //Uom = item.UnitofMeansure.Name
                        };
                        items.Add(data);
                    }
                }

            };
            if (items.Any())
            {
                _timeDelivery.PrintOrder(items, PrinterNames, Setting);
            }

        }
        public void IssuseCommittedOrder(int orderid)
        {
            _context.Database.ExecuteSqlCommand("pos_OrderDetailCommittedStock @OrderID={0}",
               parameters: new[] {
                    orderid.ToString()
               });
        }
        public void IssuseCommittedMaterial(int orderid)
        {
            var Order = _context.Order.First(w => w.OrderID == orderid) ?? new Order();
            var OrderDetail = _context.OrderDetail.Where(w => w.OrderID == orderid);
            var WID = _context.Warehouses.First(w => w.ID == Order.WarehouseID) ?? new Warehouse();
            foreach (var item in OrderDetail.ToList())
            {
                var bom = _context.BOMaterial.FirstOrDefault(w => w.ItemID == item.ItemID && w.Active == true) ?? new BOMaterial();
                var items_material = (from bomd in _context.BOMDetail.Where(w => w.BID == bom.BID && w.Detele == false)
                                      join i in _context.ItemMasterDatas on bomd.ItemID equals i.ID
                                      join gd in _context.GroupDUoMs on bomd.UomID equals gd.AltUOM
                                      join uom in _context.UnitofMeasures on i.InventoryUoMID equals uom.ID
                                      select new
                                      {
                                          bomd.ItemID,
                                          gd.GroupUoMID,
                                          GUoMID = i.GroupUomID,
                                          Qty = ((double)item.PrintQty * (double)gd.Factor) * ((double)bomd.Qty * (double)gd.Factor),
                                      }).Where(w => w.GroupUoMID == w.GUoMID);
                if (items_material != null)
                {
                    foreach (var item_detail in items_material.ToList())
                    {
                        var item_warehouse_material = _context.WarehouseSummary.FirstOrDefault(w => w.WarehouseID == WID.ID && w.ItemID == item_detail.ItemID);
                        item_warehouse_material.Committed += (double)item_detail.Qty;
                        _context.WarehouseSummary.Update(item_warehouse_material);
                        _context.SaveChanges();
                    }
                }
            }
        }
        public void GoodReceiptCommittedOrder(int orderid)
        {
            try
            {
                _context.Database.ExecuteSqlCommand("pos_OrderDetailCommittedStockGoodReceipt @OrderID={0}",
               parameters: new[] {
                    orderid.ToString()
               });
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
        public void IssuseInStockOrder(int orderid)
        {
            try
            {
                _context.Database.ExecuteSqlCommand("pos_OrderDetailIssuseStock @OrderID={0}",
                  parameters: new[] {
                        orderid.ToString()
                  });
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
        public void IssuseInStockMaterial(int orderid)
        {
            var Order = _context.Order.First(w => w.OrderID == orderid) ?? new Order();
            var OrderDetail = _context.OrderDetail.Where(w => w.OrderID == orderid);
            foreach (var item in OrderDetail.ToList())
            {
                var bom = _context.BOMaterial.FirstOrDefault(w => w.ItemID == item.ItemID && w.Active == true) ?? new BOMaterial();
                var items_material = (from bomd in _context.BOMDetail.Where(w => w.BID == bom.BID && w.Detele == false)
                                      join i in _context.ItemMasterDatas on bomd.ItemID equals i.ID
                                      join gd in _context.GroupDUoMs on bomd.UomID equals gd.AltUOM
                                      join uom in _context.UnitofMeasures on i.InventoryUoMID equals uom.ID
                                      select new
                                      {
                                          bomd.ItemID,
                                          gd.GroupUoMID,
                                          GUoMID = i.GroupUomID,
                                          Qty = ((double)item.Qty * (double)gd.Factor) * ((double)bomd.Qty * (double)gd.Factor),
                                          bomd.NegativeStock,
                                          i.Process,
                                          UomID = uom.ID,
                                          gd.Factor
                                      }).Where(w => w.GroupUoMID == w.GUoMID).ToList();
                if (items_material != null)
                {
                    foreach (var item_detail in items_material.ToList())
                    {
                        var item_master_data = _context.ItemMasterDatas.FirstOrDefault(w => w.ID == item_detail.ItemID);
                        var item_warehouse_material = _context.WarehouseSummary.FirstOrDefault(w => w.WarehouseID == Order.WarehouseID && w.ItemID == item_detail.ItemID);
                        var all_item_warehouse_detail = _context.WarehouseDetails.Where(w => w.WarehouseID == Order.WarehouseID && w.ItemID == item_detail.ItemID).ToList();
                        var item_nagative = from wa in _context.WarehouseSummary.Where(w => w.ItemID == item_detail.ItemID)
                                            join na in _context.BOMDetail on wa.ItemID equals na.ItemID
                                            select new
                                            {
                                                NagaStock = wa.InStock
                                            };
                        var nagative_check = item_nagative.Sum(w => w.NagaStock);
                        //WerehouseSummary
                        item_warehouse_material.Committed -= (double)item_detail.Qty;
                        item_warehouse_material.InStock -= (double)item_detail.Qty;
                        //Itemmasterdata
                        item_master_data.StockIn -= (double)item_detail.Qty;
                        if (item_detail.NegativeStock == true && nagative_check <= 0)
                        {
                            double @IssusQty;
                            double @FIFOQty;
                            double @Qty = item_detail.Qty;
                            var item_inventory_audit = new InventoryAudit();
                            var item_IssusStock = all_item_warehouse_detail.LastOrDefault(w => w.InStock <= 0);
                            @FIFOQty = item_IssusStock.InStock - @Qty;
                            @IssusQty = item_IssusStock.InStock - @FIFOQty;
                            if (item_detail.Process == "FIFO")
                            {
                                item_IssusStock.InStock = @FIFOQty;
                                if (@IssusQty > 0)
                                {
                                    var inventory_audit = _context.InventoryAudits.Where(w => w.ItemID == item_detail.ItemID && w.UomID == item_detail.UomID && w.WarehouseID == Order.WarehouseID);
                                    item_inventory_audit.ID = 0;
                                    item_inventory_audit.WarehouseID = Order.WarehouseID;
                                    item_inventory_audit.BranchID = Order.BranchID;
                                    item_inventory_audit.UserID = Order.UserOrderID;
                                    item_inventory_audit.ItemID = item_detail.ItemID;
                                    item_inventory_audit.CurrencyID = Order.LocalCurrencyID;
                                    item_inventory_audit.UomID = item_detail.UomID;
                                    item_inventory_audit.InvoiceNo = Order.ReceiptNo;
                                    item_inventory_audit.Trans_Type = "SOKV";
                                    item_inventory_audit.Process = item_detail.Process;
                                    item_inventory_audit.SystemDate = DateTime.Now;
                                    item_inventory_audit.TimeIn = Order.TimeIn;
                                    item_inventory_audit.Qty = @IssusQty * -1;
                                    item_inventory_audit.Cost = item_IssusStock.Cost;
                                    item_inventory_audit.Price = 0;
                                    item_inventory_audit.CumulativeQty = inventory_audit.Sum(q => q.Qty) + (@IssusQty * (-1));
                                    item_inventory_audit.CumulativeValue = (inventory_audit.Sum(s => s.Trans_Valuse)) + (@IssusQty * item_IssusStock.Cost) * (-1);
                                    item_inventory_audit.Trans_Valuse = (@IssusQty * item_IssusStock.Cost) * (-1);
                                    item_inventory_audit.ExpireDate = item_IssusStock.ExpireDate;
                                    //item_inventory_audit.LocalCurID = Order.l
                                }
                            }
                            else
                            {
                                item_IssusStock.InStock = @FIFOQty;
                                if (@IssusQty > 0)
                                {
                                    var item_pricelise_detail = _context.PriceListDetails.FirstOrDefault(w => w.ItemID == item_detail.ItemID && w.UomID == item_detail.UomID);
                                    double @sysAvCost = item_pricelise_detail.Cost * Order.ExchangeRate;
                                    var inventory_audit = _context.InventoryAudits.Where(w => w.ItemID == item_detail.ItemID && w.UomID == item_detail.UomID && w.WarehouseID == Order.WarehouseID);
                                    item_inventory_audit.ID = 0;
                                    item_inventory_audit.WarehouseID = Order.WarehouseID;
                                    item_inventory_audit.BranchID = Order.BranchID;
                                    item_inventory_audit.UserID = Order.UserOrderID;
                                    item_inventory_audit.ItemID = item_detail.ItemID;
                                    item_inventory_audit.CurrencyID = Order.LocalCurrencyID;
                                    item_inventory_audit.UomID = item_detail.UomID;
                                    item_inventory_audit.InvoiceNo = Order.ReceiptNo;
                                    item_inventory_audit.Trans_Type = "SOKV";
                                    item_inventory_audit.Process = item_detail.Process;
                                    item_inventory_audit.SystemDate = DateTime.Now;
                                    item_inventory_audit.TimeIn = Order.TimeIn;
                                    item_inventory_audit.Qty = @IssusQty * -1;
                                    item_inventory_audit.Cost = ((inventory_audit.Sum(s => s.Trans_Valuse)) + (@IssusQty * @sysAvCost) * (-1)) / (inventory_audit.Sum(q => q.Qty) + (@IssusQty * (-1)));
                                    item_inventory_audit.Price = 0;
                                    item_inventory_audit.CumulativeQty = inventory_audit.Sum(q => q.Qty) + (@IssusQty * (-1));
                                    item_inventory_audit.CumulativeValue = (inventory_audit.Sum(s => s.Trans_Valuse)) + (@IssusQty * @sysAvCost) * (-1);
                                    item_inventory_audit.Trans_Valuse = (@IssusQty * @sysAvCost) * (-1);
                                    item_inventory_audit.ExpireDate = item_IssusStock.ExpireDate;
                                }
                            }
                            _context.WarehouseDetails.Update(item_IssusStock);
                            _context.InventoryAudits.Add(item_inventory_audit);
                            _context.SaveChanges();

                        }
                        else
                        {
                            double @Check_Stock;
                            double @Remain;
                            double @IssusQty;
                            double @FIFOQty;
                            double @Qty = item_detail.Qty;
                            foreach (var item_warehouse in all_item_warehouse_detail.Where(w => w.InStock > 0))
                            {
                                var item_inventory_audit = new InventoryAudit();
                                var item_IssusStock = all_item_warehouse_detail.FirstOrDefault(w => w.InStock > 0);
                                @Check_Stock = item_warehouse.InStock - @Qty;
                                if (@Check_Stock < 0)
                                {
                                    @Remain = (item_warehouse.InStock - @Qty) * (-1);
                                    @IssusQty = @Qty - @Remain;
                                    if (@Remain <= 0)
                                    {
                                        @Qty = 0;
                                    }
                                    else
                                    {
                                        @Qty = @Remain;
                                    }
                                    if (item_detail.Process == "FIFO")
                                    {
                                        item_IssusStock.InStock = item_IssusStock.InStock -= @IssusQty;
                                        if (@IssusQty > 0)
                                        {
                                            var inventory_audit = _context.InventoryAudits.Where(w => w.ItemID == item_detail.ItemID && w.UomID == item_detail.UomID && w.WarehouseID == Order.WarehouseID);
                                            item_inventory_audit.ID = 0;
                                            item_inventory_audit.WarehouseID = Order.WarehouseID;
                                            item_inventory_audit.BranchID = Order.BranchID;
                                            item_inventory_audit.UserID = Order.UserOrderID;
                                            item_inventory_audit.ItemID = item_detail.ItemID;
                                            item_inventory_audit.CurrencyID = Order.LocalCurrencyID;
                                            item_inventory_audit.UomID = item_detail.UomID;
                                            item_inventory_audit.InvoiceNo = Order.ReceiptNo;
                                            item_inventory_audit.Trans_Type = "SOKV";
                                            item_inventory_audit.Process = item_detail.Process;
                                            item_inventory_audit.SystemDate = DateTime.Now;
                                            item_inventory_audit.TimeIn = Order.TimeIn;
                                            item_inventory_audit.Qty = @IssusQty * -1;
                                            item_inventory_audit.Cost = item_IssusStock.Cost;
                                            item_inventory_audit.Price = 0;
                                            item_inventory_audit.CumulativeQty = inventory_audit.Sum(q => q.Qty) + (@IssusQty * (-1));
                                            item_inventory_audit.CumulativeValue = (inventory_audit.Sum(s => s.Trans_Valuse)) + (@IssusQty * item_IssusStock.Cost) * (-1);
                                            item_inventory_audit.Trans_Valuse = (@IssusQty * item_IssusStock.Cost) * (-1);
                                            item_inventory_audit.ExpireDate = item_IssusStock.ExpireDate;
                                        }
                                        _context.WarehouseDetails.Update(item_IssusStock);
                                        _context.InventoryAudits.Add(item_inventory_audit);
                                        _context.SaveChanges();
                                    }
                                    else
                                    {
                                        item_IssusStock.InStock = item_IssusStock.InStock -= @IssusQty;
                                        if (@IssusQty > 0)
                                        {
                                            var item_pricelise_detail = _context.PriceListDetails.FirstOrDefault(w => w.ItemID == item_detail.ItemID && w.UomID == item_detail.UomID);
                                            double @sysAvCost = item_pricelise_detail.Cost * Order.ExchangeRate;
                                            var inventory_audit = _context.InventoryAudits.Where(w => w.ItemID == item_detail.ItemID && w.UomID == item_detail.UomID && w.WarehouseID == Order.WarehouseID);
                                            item_inventory_audit.ID = 0;
                                            item_inventory_audit.WarehouseID = Order.WarehouseID;
                                            item_inventory_audit.BranchID = Order.BranchID;
                                            item_inventory_audit.UserID = Order.UserOrderID;
                                            item_inventory_audit.ItemID = item_detail.ItemID;
                                            item_inventory_audit.CurrencyID = Order.LocalCurrencyID;
                                            item_inventory_audit.UomID = item_detail.UomID;
                                            item_inventory_audit.InvoiceNo = Order.ReceiptNo;
                                            item_inventory_audit.Trans_Type = "SOKV";
                                            item_inventory_audit.Process = item_detail.Process;
                                            item_inventory_audit.SystemDate = DateTime.Now;
                                            item_inventory_audit.TimeIn = Order.TimeIn;
                                            item_inventory_audit.Qty = @IssusQty * -1;
                                            item_inventory_audit.Cost = ((inventory_audit.Sum(s => s.Trans_Valuse)) + (@IssusQty * @sysAvCost) * (-1)) / (inventory_audit.Sum(q => q.Qty) + (@IssusQty * (-1)));
                                            item_inventory_audit.Price = 0;
                                            item_inventory_audit.CumulativeQty = inventory_audit.Sum(q => q.Qty) + (@IssusQty * (-1));
                                            item_inventory_audit.CumulativeValue = (inventory_audit.Sum(s => s.Trans_Valuse)) + (@IssusQty * @sysAvCost) * (-1);
                                            item_inventory_audit.Trans_Valuse = (@IssusQty * @sysAvCost) * (-1);
                                            item_inventory_audit.ExpireDate = item_IssusStock.ExpireDate;
                                        }
                                        _context.WarehouseDetails.Update(item_IssusStock);
                                        _context.InventoryAudits.Add(item_inventory_audit);
                                        _context.SaveChanges();
                                    }
                                }
                                else
                                {
                                    @FIFOQty = item_IssusStock.InStock - @Qty;
                                    @IssusQty = item_IssusStock.InStock - @FIFOQty;
                                    if (item_detail.Process == "FIFO")
                                    {
                                        item_IssusStock.InStock = @FIFOQty;
                                        if (@IssusQty > 0)
                                        {
                                            var inventory_audit = _context.InventoryAudits.Where(w => w.ItemID == item_detail.ItemID && w.UomID == item_detail.UomID && w.WarehouseID == Order.WarehouseID);
                                            item_inventory_audit.ID = 0;
                                            item_inventory_audit.WarehouseID = Order.WarehouseID;
                                            item_inventory_audit.BranchID = Order.BranchID;
                                            item_inventory_audit.UserID = Order.UserOrderID;
                                            item_inventory_audit.ItemID = item_detail.ItemID;
                                            item_inventory_audit.CurrencyID = Order.LocalCurrencyID;
                                            item_inventory_audit.UomID = item_detail.UomID;
                                            item_inventory_audit.InvoiceNo = Order.ReceiptNo;
                                            item_inventory_audit.Trans_Type = "SOKV";
                                            item_inventory_audit.Process = item_detail.Process;
                                            item_inventory_audit.SystemDate = DateTime.Now;
                                            item_inventory_audit.TimeIn = Order.TimeIn;
                                            item_inventory_audit.Qty = @IssusQty * -1;
                                            item_inventory_audit.Cost = item_IssusStock.Cost;
                                            item_inventory_audit.Price = 0;
                                            item_inventory_audit.CumulativeQty = inventory_audit.Sum(q => q.Qty) + (@IssusQty * (-1));
                                            item_inventory_audit.CumulativeValue = (inventory_audit.Sum(s => s.Trans_Valuse)) + (@IssusQty * item_IssusStock.Cost) * (-1);
                                            item_inventory_audit.Trans_Valuse = (@IssusQty * item_IssusStock.Cost) * (-1);
                                            item_inventory_audit.ExpireDate = item_IssusStock.ExpireDate;
                                        }
                                        _context.WarehouseDetails.Update(item_IssusStock);
                                        _context.InventoryAudits.Add(item_inventory_audit);
                                        _context.SaveChanges();
                                    }
                                    else
                                    {
                                        item_IssusStock.InStock = @FIFOQty;
                                        if (@IssusQty > 0)
                                        {
                                            var item_pricelise_detail = _context.PriceListDetails.FirstOrDefault(w => w.ItemID == item_detail.ItemID && w.UomID == item_detail.UomID);
                                            double @sysAvCost = item_pricelise_detail.Cost * Order.ExchangeRate;
                                            var inventory_audit = _context.InventoryAudits.Where(w => w.ItemID == item_detail.ItemID && w.UomID == item_detail.UomID && w.WarehouseID == Order.WarehouseID);
                                            item_inventory_audit.ID = 0;
                                            item_inventory_audit.WarehouseID = Order.WarehouseID;
                                            item_inventory_audit.BranchID = Order.BranchID;
                                            item_inventory_audit.UserID = Order.UserOrderID;
                                            item_inventory_audit.ItemID = item_detail.ItemID;
                                            item_inventory_audit.CurrencyID = Order.LocalCurrencyID;
                                            item_inventory_audit.UomID = item_detail.UomID;
                                            item_inventory_audit.InvoiceNo = Order.ReceiptNo;
                                            item_inventory_audit.Trans_Type = "SOKV";
                                            item_inventory_audit.Process = item_detail.Process;
                                            item_inventory_audit.SystemDate = DateTime.Now;
                                            item_inventory_audit.TimeIn = Order.TimeIn;
                                            item_inventory_audit.Qty = @IssusQty * -1;
                                            item_inventory_audit.Cost = ((inventory_audit.Sum(s => s.Trans_Valuse)) + (@IssusQty * @sysAvCost) * (-1)) / (inventory_audit.Sum(q => q.Qty) + (@IssusQty * (-1)));
                                            item_inventory_audit.Price = 0;
                                            item_inventory_audit.CumulativeQty = inventory_audit.Sum(q => q.Qty) + (@IssusQty * (-1));
                                            item_inventory_audit.CumulativeValue = (inventory_audit.Sum(s => s.Trans_Valuse)) + (@IssusQty * @sysAvCost) * (-1);
                                            item_inventory_audit.Trans_Valuse = (@IssusQty * @sysAvCost) * (-1);
                                            item_inventory_audit.ExpireDate = item_IssusStock.ExpireDate;
                                        }
                                        _context.WarehouseDetails.Update(item_IssusStock);
                                        _context.InventoryAudits.Add(item_inventory_audit);
                                        _context.SaveChanges();
                                    }
                                    all_item_warehouse_detail = new List<WarehouseDetail>();
                                    break;
                                }

                            }
                        }
                        _context.WarehouseSummary.Update(item_warehouse_material);
                        _context.ItemMasterDatas.Update(item_master_data);
                        _context.SaveChanges();
                    }
                }
            }
        }
        public void StartTime(int TableId, string Time)
        {
            if (TableId != 0)
            {
                Table table = _context.Tables.FirstOrDefault(w => w.ID == TableId);

                if (table.Status == 'A')
                {
                    _timeDelivery.StartTimeTable(table.ID, Time, 'B');
                    table.Status = 'B';
                    _context.Update(table);
                    _context.SaveChanges();
                }
                else
                {
                    Time = _timeDelivery.StopTimeTable(TableId, 'A');
                    _timeDelivery.StartTimeTable(TableId, Time, 'B');
                    table.Status = 'B';
                    _context.Update(table);
                    _context.SaveChanges();
                }

            }

        }
        public IEnumerable<Order> GetOrder(int tableid, int orderid, int userid)
        {
            var orders = _context.Order.Where(w => w.TableID == tableid && w.Cancel == false).ToList();
            var user = _context.UserAccounts.Include(emp => emp.Employee).FirstOrDefault(w => w.ID == userid);
            if (orderid == 0)
            {
                var Count = _context.Order.Where(w => w.TableID == tableid && w.Cancel == false).Count();
                if (Count > 0)
                {
                    var MinOrderId = _context.Order.Where(w => w.TableID == tableid && w.Cancel == false).Max(min => min.OrderID);
                    var order = _context.Order.Include(x => x.OrderDetail).Include(x => x.Currency).Where(w => w.TableID == tableid && w.OrderID == MinOrderId && w.Cancel == false).ToList();

                    _timeDelivery.PushOrderByTable(orders, tableid, user.Employee.Name);
                    return order;
                }
                else
                {
                    var order = _context.Order.Include(x => x.OrderDetail).Include(x => x.Currency).Where(w => w.TableID == tableid && w.Cancel == false).ToList();
                    _timeDelivery.PushOrderByTable(orders, tableid, user.Employee.Name);
                    return order;
                }

            }
            else
            {
                var order = _context.Order.Include(x => x.OrderDetail).Include(x => x.Currency).Where(w => w.TableID == tableid && w.OrderID == orderid && !w.Cancel).ToList();
                _timeDelivery.PushOrderByTable(orders, tableid, user.Employee.Name);
                return order;
            }

        }

        public void PrintReceiptReprint(int receiptId, string print_type)
        {
            if (receiptId > 0)
            {
                var receipt = _context.Receipt.Find(receiptId);
                var receipt_reprint = _context.ReceiptDetail.Where(w => w.ReceiptID == receipt.ReceiptID);

                //Pirint bill or tender
                var table = _context.Tables.Find(receipt.TableID);
                var user = _context.UserAccounts.Include(emp => emp.Employee).FirstOrDefault(w => w.ID == receipt.UserOrderID);

                var customer = _context.BusinessPartners.FirstOrDefault(w => w.Delete == false && w.Type == "Customer" && w.ID == receipt.CustomerID);
                var setting = _context.GeneralSettings.Where(w => w.BranchID == receipt.BranchID).ToList();

                var syscurrency = _context.Currency.FirstOrDefault(w => w.ID == receipt.SysCurrencyID);
                var banch = _context.Branches.FirstOrDefault(w => w.Delete == false && w.ID == receipt.BranchID);
                var receiptInfo = _context.ReceiptInformation.FirstOrDefault(w => w.BranchID == receipt.BranchID);
                var payment = _context.PaymentMeans.FirstOrDefault(w => w.ID == receipt.PaymentMeansID);
                List<PrintBill> PrintBill = new();
                var ReceiptNo = "";
                var Received = "";
                var GrandTotal_Dis = "";
                var Change = "";
                var ChangeSys = "";
                foreach (var item in receipt_reprint)
                {
                    var uom = _context.UnitofMeasures.First(u => u.ID == item.UomID);
                    //Discount
                    if (item.TypeDis == "Cash")
                    {
                        receipt.DiscountRate = receipt.DiscountValue;
                    }
                    //Vat
                    if (setting[0].VatAble == true)
                    {
                        ReceiptNo = receipt.ReceiptNo + " " + setting[0].VatNum;
                    }
                    else
                    {
                        ReceiptNo = receipt.ReceiptNo;
                    }
                    if (print_type == "Cancel")
                    {
                        ReceiptNo = "Cancel-" + ReceiptNo;
                    }
                    if (receipt.CurrencyDisplay == "KHR" || receipt.CurrencyDisplay == "៛")
                    {
                        Received = item.Currency + " " + string.Format("{0:n0}", receipt.Received);
                        Change = item.Currency + " " + string.Format("{0:n0}", receipt.Change);
                    }
                    else
                    {
                        GrandTotal_Dis = receipt.CurrencyDisplay + " " + receipt.GrandTotal_Display.ToString("n3")[0..^1];
                        ChangeSys = receipt.CurrencyDisplay + " " + receipt.Change_Display.ToString("n3")[0..^1];
                    }
                    if (item.Currency == "KHR" || item.Currency == "៛")
                    {
                        Received = item.Currency + " " + string.Format("{0:n0}", receipt.Received);
                        Change = item.Currency + " " + string.Format("{0:n0}", receipt.Change);
                    }
                    else
                    {
                        Received = item.Currency + " " + receipt.Received.ToString("n3")[0..^1];
                        Change = item.Currency + " " + receipt.Change.ToString("n3")[0..^1];
                    }
                    if (item.Currency == "KHR" || item.Currency == "៛")
                    {
                        PrintBill bill = new()
                        {
                            OrderID = item.OrderID,
                            ReceiptNo = ReceiptNo,
                            Logo = receiptInfo.Logo,
                            BranchName = receiptInfo.Title,
                            Address = receiptInfo.Address,
                            Tel1 = receiptInfo.Tel1,
                            Tel2 = receiptInfo.Tel2,
                            Table = table.Name,
                            OrderNo = receipt.OrderNo,
                            Cashier = user.Employee.Name,
                            DateTimeIn = receipt.DateIn.ToString("dd-MM-yyyy") + " " + receipt.TimeIn,
                            DateTimeOut = receipt.DateOut.ToString("dd-MM-yyyy") + " " + receipt.TimeOut,
                            Item = item.KhmerName,
                            Qty = item.Qty + "",
                            Price = string.Format("{0:n0}", item.UnitPrice),
                            DisItem = item.DiscountRate + "",
                            Amount = string.Format("{0:n0}", item.Total),
                            SubTotal = string.Format("{0:n0}", receipt.Sub_Total),
                            DisRate = receipt.DiscountRate + "%",
                            DisValue = string.Format("{0:n0}", receipt.DiscountValue),
                            TypeDis = receipt.TypeDis,
                            GrandTotal = item.Currency + " " + string.Format("{0:n0}", receipt.GrandTotal),
                            GrandTotalSys = GrandTotal_Dis,
                            VatRate = receipt.TaxRate + "%",
                            VatValue = item.Currency + " " + string.Format("{0:n0}", receipt.TaxValue),
                            Received = Received,
                            Change = Change,
                            ChangeSys = ChangeSys,
                            DescKh = receiptInfo.KhmerDescription,
                            DescEn = receiptInfo.EnglishDescription,
                            ExchangeRate = string.Format("{0:n0}", receipt.ExchangeRate),
                            Printer = "",
                            Print = print_type,
                            ItemDesc = item.Description,
                            CustomerInfo = customer.Code + '/' + customer.Name + '/' + customer.Phone + '/' + customer.Address + '/' + customer.Email,
                            Team = receiptInfo.TeamCondition,
                            ItemType = item.ItemType,
                            PaymentMeans = payment.Type
                        };
                        //Qty
                        if (item.ItemType == "Service")
                        {
                            var arr_time = item.Qty.ToString().Split('.');
                            bill.Qty = arr_time[0] + "h:" + arr_time[1].PadRight(2, '0') + "m";
                        }
                        PrintBill.Add(bill);
                    }
                    else
                    {
                        PrintBill bill = new()
                        {
                            OrderID = item.OrderID,
                            ReceiptNo = ReceiptNo,
                            Logo = receiptInfo.Logo,
                            BranchName = receiptInfo.Title,
                            Address = receiptInfo.Address,
                            Tel1 = receiptInfo.Tel1,
                            Tel2 = receiptInfo.Tel2,
                            Table = table.Name,
                            OrderNo = receipt.OrderNo,
                            Cashier = user.Employee.Name,
                            DateTimeIn = receipt.DateIn.ToString("dd-MM-yyyy") + " " + receipt.TimeIn,
                            DateTimeOut = receipt.DateOut.ToString("dd-MM-yyyy") + " " + receipt.TimeOut,
                            Item = item.KhmerName,
                            Qty = item.Qty + "" + uom.Name,
                            Price = item.UnitPrice.ToString("n3")[0..^1],
                            DisItem = item.DiscountRate + "",
                            Amount = item.Total.ToString("n3")[0..^1],
                            SubTotal = receipt.Sub_Total.ToString("n3")[0..^1],
                            DisRate = receipt.DiscountRate + "%",
                            DisValue = receipt.DiscountValue.ToString("n3")[0..^1],
                            TypeDis = receipt.TypeDis,
                            GrandTotal = item.Currency + " " + receipt.GrandTotal.ToString("n3")[0..^1],
                            GrandTotalSys = GrandTotal_Dis,
                            VatRate = receipt.TaxRate + "%",
                            VatValue = item.Currency + " " + receipt.TaxValue.ToString("n3")[0..^1],
                            Received = Received,
                            Change = Change,
                            ChangeSys = ChangeSys,
                            DescKh = receiptInfo.KhmerDescription,
                            DescEn = receiptInfo.EnglishDescription,
                            ExchangeRate = receipt.ExchangeRate.ToString("n3")[0..^1],
                            Printer = "",
                            Print = print_type,
                            ItemDesc = item.Description,
                            CustomerInfo = customer.Code + '/' + customer.Name + '/' + customer.Phone + '/' + customer.Address + '/' + customer.Email,
                            Team = receiptInfo.TeamCondition,
                            ItemType = item.ItemType,
                            PaymentMeans = payment.Type
                        };
                        //Qty
                        if (item.ItemType == "Service")
                        {
                            var arr_time = item.Qty.ToString().Split('.');
                            bill.Qty = arr_time[0] + "h:" + arr_time[1].PadRight(2, '0') + "m";
                        }
                        PrintBill.Add(bill);
                    }
                }
                var ip = _accessor.ActionContext.HttpContext.Connection.RemoteIpAddress.ToString();
                _timeDelivery.PrintBill(PrintBill, setting, ip);
            }

        }
        public void MoveTable(int old_Id, int new_Id)
        {
            var time = _timeDelivery.StopTimeTable(old_Id, 'A');
            _timeDelivery.ResetTimeTable(old_Id, 'A', "00:00:00");
            _timeDelivery.StartTimeTable(new_Id, time, 'B');
            _timeDelivery.GetTableAvailable("Move");
            var oldtable = _context.Tables.Find(old_Id);
            var newtable = _context.Tables.Find(new_Id);
            var orders = _context.Order.Where(w => w.TableID == old_Id);
            foreach (var order in orders)
            {
                order.TableID = new_Id;
                _context.Update(order);
            }
            oldtable.Status = 'A';
            oldtable.Time = "00:00:00";
            newtable.Status = 'B';
            newtable.Time = time;
            _context.Update(oldtable);
            _context.Update(newtable);
            _context.SaveChanges();
        }
        public void CombineReceipt(CombineOrder combineReceipt)
        {
            List<OrderDetail> CombineItem = new();
            foreach (var receipt in combineReceipt.Orders.ToList())
            {
                var order_detail = _context.OrderDetail.Where(w => w.OrderID != combineReceipt.OrderID && w.OrderID == receipt.OrderID).ToList();
                foreach (var item in order_detail)
                {
                    var item_update = _context.OrderDetail.FirstOrDefault(w => w.Line_ID == item.Line_ID && w.OrderID == combineReceipt.OrderID);
                    var item_count = _context.OrderDetail.Where(w => w.Line_ID == item.Line_ID && w.OrderID == combineReceipt.OrderID);
                    try
                    {
                        if (item_count.Any())
                        {
                            item_update.KhmerName += "*";
                            item_update.EnglishName += "*";
                            item_update.Qty += item.Qty;
                            item_update.PrintQty = item_update.Qty + item.Qty;
                            if (item_update.TypeDis == "Percent")
                            {
                                item_update.Total = (item_update.Qty * item_update.UnitPrice) * (1 - item_update.DiscountRate / 100);
                                item_update.DiscountValue = (item_update.Qty * item_update.UnitPrice) / 100;
                            }
                            else
                            {
                                item_update.Total = (item_update.Qty * item_update.UnitPrice) - item_update.DiscountRate;
                                item_update.DiscountValue = item_update.DiscountRate;
                            }
                            _context.OrderDetail.Remove(item);
                            _context.Update(item_update);
                            _context.SaveChanges();

                        }
                        else
                        {
                            _context.Remove(item);
                            _context.SaveChanges();

                            item.OrderDetailID = 0;
                            item.KhmerName += "*";
                            item.OrderID = combineReceipt.OrderID;
                            _context.OrderDetail.Add(item);
                            _context.SaveChanges();

                        }

                    }
                    catch (Exception)
                    {
                        //throw new Exception(ex.Message);
                        _context.OrderDetail.Remove(item);
                        _context.SaveChanges();

                        item.OrderDetailID = 0;
                        item.OrderID = combineReceipt.OrderID;
                        _context.OrderDetail.Add(item);
                        _context.SaveChanges();

                    }

                }
                //Update status delete order
                var order_update = _context.Order.Find(receipt.OrderID);
                order_update.Delete = true;
                _context.Order.Update(order_update);
                _context.SaveChanges();
                //Remove order
                var orders_delete = _context.Order.Where(w => w.Delete == true);
                foreach (var order in orders_delete.ToList())
                {
                    _context.Remove(order);
                    _context.SaveChanges();
                }

                //Update status table
                var table_ordered = _context.Order.Where(w => w.TableID == order_update.TableID && w.Cancel == false).ToList();
                if (table_ordered.Count == 0)
                {
                    //_timeDelivery.PushOrderByTableToAvailable(order_update.TableID);
                    var time = _timeDelivery.StopTimeTable(order_update.TableID, 'A');
                    var table_up = _context.Tables.Find(order_update.TableID);
                    table_up.Status = 'A';
                    table_up.Time = "00:00:00";
                    _context.Update(table_up);
                    _context.SaveChanges();

                }
            }

        }
        //New update by Mak Sokmanh
        //.............................................................//
        public void ClearUserOrder(int tableid)
        {
            _timeDelivery.ClearUserOrder(tableid);
        }
        IEnumerable<OpenShift> IKVMS.OpenShiftData(int userid, double cash)
        {
            var user = _context.UserAccounts.FirstOrDefault(w => w.ID == userid);
            var setting = _context.GeneralSettings.FirstOrDefault(w => w.UserID == userid);
            var receipts = _context.Receipt.Where(w => w.UserOrderID == userid);
            var openshifts = _context.OpenShift.Where(w => w.UserID == userid && w.Open == true).ToList();
            var exchangeRate = _context.ExchangeRates.Find(setting.LocalCurrencyID);
            try
            {

                if (openshifts.Count > 0)
                {
                    var open = _context.OpenShift.Where(w => w.UserID == userid && w.Open == true);
                    var open_update = _context.OpenShift.FirstOrDefault(w => w.UserID == userid && w.Open == true);
                    open_update.CashAmount_Sys = open.Sum(w => w.CashAmount_Sys) + cash;
                    _context.OpenShift.Update(open_update);
                    _context.SaveChanges();
                    return null;
                }
                else
                {
                    OpenShift _openShift = new()
                    {
                        DateIn = DateTime.Today,
                        TimeIn = DateTime.Now.ToShortTimeString(),
                        BranchID = user.BranchID,
                        UserID = userid,
                        CashAmount_Sys = cash,
                        Trans_From = receipts.Max(m => m.ReceiptID),
                        Open = true,
                        LocalCurrencyID = setting.LocalCurrencyID,
                        SysCurrencyID = setting.SysCurrencyID,
                        LocalSetRate = exchangeRate.SetRate
                    };
                    _context.OpenShift.Add(_openShift);
                    _context.SaveChanges();
                    return openshifts;
                }
            }
            catch (Exception)
            {
                OpenShift openShift = new()
                {
                    DateIn = DateTime.Today,
                    TimeIn = DateTime.Now.ToShortTimeString(),
                    BranchID = user.BranchID,
                    UserID = userid,
                    CashAmount_Sys = cash,
                    Trans_From = 0,
                    Open = true,
                    LocalCurrencyID = setting.LocalCurrencyID,
                    SysCurrencyID = setting.SysCurrencyID,
                    LocalSetRate = exchangeRate.SetRate
                };
                _context.OpenShift.Add(openShift);
                _context.SaveChanges();
                return openshifts;
            }

        }
        public CloseShift CloseShiftData(int userid, double cashout)
        {
            var user = _context.UserAccounts.FirstOrDefault(w => w.ID == userid);
            var setting = _context.GeneralSettings.FirstOrDefault(w => w.UserID == userid);
            var exchangeRate = _context.ExchangeRates.Find(setting.LocalCurrencyID);
            var receipts = _context.Receipt.Where(w => w.BranchID == user.BranchID).ToList();
            var trans_to = 0;
            //add new
            var trans_vto = 0;
            var trans_vFrom = 0;
            var voidItem = _context.VoidItems.Where(w => w.UserOrderID == user.ID).ToList();
            var openshift = _context.OpenShift.FirstOrDefault(w => w.UserID == userid && w.Open == true);
            if (receipts.Count > 0)
            {
                trans_to = receipts.Max(w => w.ReceiptID);
            }
            //add new
            if (voidItem.Count > 0)
            {
                trans_vFrom = voidItem.Max(w => w.ID);
                trans_vto = voidItem.Max(w => w.ID);

            }
            if (openshift == null)
            {
                return null;
            }
            else
            {
                openshift.Open = false;
                _context.OpenShift.Update(openshift);
                _context.SaveChanges();
                var amount = _context.Receipt.Where(w => w.BranchID == user.BranchID && w.UserOrderID == userid && w.ReceiptID > openshift.Trans_From && w.ReceiptID <= trans_to).Sum(w => w.GrandTotal);

                CloseShift closeShift = new()
                {
                    DateIn = openshift.DateIn,
                    TimeIn = openshift.TimeIn,
                    DateOut = DateTime.Today,
                    TimeOut = DateTime.Now.ToShortTimeString(),
                    BranchID = user.BranchID,
                    UserID = userid,
                    CashInAmount_Sys = openshift.CashAmount_Sys,
                    SaleAmount_Sys = amount * setting.RateIn,
                    CashOutAmount_Sys = cashout,
                    ExchangeRate = setting.RateIn,
                    Trans_From = openshift.Trans_From,
                    Trans_To = trans_to,
                    LocalCurrencyID = setting.LocalCurrencyID,
                    SysCurrencyID = setting.SysCurrencyID,
                    LocalSetRate = exchangeRate.SetRate,
                    Close = true
                };
                _context.CloseShift.Add(closeShift);
                _context.SaveChanges();
                var settings = _context.GeneralSettings.Where(w => w.UserID == userid).ToList();
                switch (setting.CloseShift)
                {
                    case CloseShiftType.Category:
                        var cashoutreport = _report.GetCashoutReport(openshift.Trans_From, trans_to, userid, setting.LocalCurrencyID).ToList();
                        var ip = _accessor.ActionContext.HttpContext.Connection.RemoteIpAddress.ToString();
                        var cashoutvoiditem = _report.GetCashoutVoidItems(openshift.Trans_From, trans_to, userid).ToList();
                        _timeDelivery.PrintCashout(cashoutreport, cashoutvoiditem, settings);
                        //sendToMail(closeShift);
                        return closeShift;
                    case CloseShiftType.PaymentMean:
                        var cashoutpaymentmeans = _report.GetCashoutPaymentMean(openshift.Trans_From, trans_to, userid, setting.LocalCurrencyID).ToList();

                        //var settings = _context.GeneralSetting.Where(w => w.UserID == userid).ToList();
                        _timeDelivery.PrintCashoutPaymentMeans(cashoutpaymentmeans, settings);
                        //sendToMail(closeShift);
                        return closeShift;
                    default:
                        return closeShift;
                }
            }
        }

        private void SendToMail(CloseShift closeShift)
        {
            try
            {
                var User = _context.UserAccounts.Include(u => u.Employee).FirstOrDefault(w => w.ID == closeShift.UserID);
                MailMessage message = new();
                SmtpClient sm = new();
                message.From = new MailAddress("Developer.Soeum@gmail.com");
                message.To.Add(new MailAddress(User.Employee.Email));
                message.Subject = User.Employee.Name + " completed close shift";
                message.IsBodyHtml = true;
                message.Body = BindHtml(closeShift);
                message.Priority = MailPriority.Normal;
                sm.Host = "smtp.gmail.com";
                sm.Port = 587;// 25;
                sm.EnableSsl = true;
                sm.UseDefaultCredentials = false;
                sm.DeliveryMethod = SmtpDeliveryMethod.Network;
                sm.UseDefaultCredentials = false;
                sm.Credentials = new NetworkCredential("Developer.Soeum@gmail.com", "Developer@168");//from mail and pass
                sm.Send(message);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        private string BindHtml(CloseShift closeShift)
        {
            var Branch = _context.Branches.FirstOrDefault(w => w.ID == closeShift.BranchID);
            var User = _context.UserAccounts.Include(u => u.Employee).FirstOrDefault(w => w.ID == closeShift.UserID);
            var sys_curr = _context.Company.Include(c => c.PriceList).ThenInclude(c => c.Currency).FirstOrDefault();
            StringBuilder html = new();
            html.Append("<table></table>"
                        + "<tr>"
                            + "<td>" + "Branch : " + Branch.Name + "</td>"
                        + "</tr>"
                        + "<tr>"
                            + "<td>" + "Cashier : " + User.Employee.Name + "</td>"
                        + "</tr>"
                        + "<tr>"
                            + "<td>" + "Date In : " + closeShift.DateIn.ToString("dd-MM-yyyy") + " " + closeShift.TimeIn + "</td>"
                        + "</tr>"
                         + "<tr>"
                            + "<td>" + "Date Out : " + closeShift.DateOut.ToString("dd-MM-yyyy") + " " + closeShift.TimeOut + "</td>"
                        + "</tr>"
                         + "<tr>"
                            + "<td>" + "Total Cash In : " + sys_curr.PriceList.Currency.Description + " " + string.Format("{0:n2}", closeShift.CashInAmount_Sys) + "</td>"
                        + "</tr>"
                        + "<tr>"
                            + "<td>" + "Total Sale : " + sys_curr.PriceList.Currency.Description + " " + string.Format("{0:n2}", closeShift.SaleAmount_Sys) + "</td>"
                        + "</tr>"
                         + "<tr>"
                            + "<td style='font-weight:600'>" + "Grand Total : " + sys_curr.PriceList.Currency.Description + " " + string.Format("{0:n2}", (double.Parse(closeShift.SaleAmount_Sys.ToString()) + double.Parse(closeShift.CashInAmount_Sys.ToString()))) + "</td>"
                        + "</tr>"

                );
            return html.ToString();
        }
        public IEnumerable<Receipt> GetReceiptReprint(int branchid, string date_from, string date_to)
        {
            if (date_from == null && date_to == null)
            {
                var receipts = _context.Receipt.Include(cur => cur.Currency).Include(user => user.UserAccount.Employee).Include(table => table.Table).Where(w => w.BranchID == branchid && w.DateOut == Convert.ToDateTime(DateTime.Today));
                return receipts;
            }
            else
            {
                var receipts = _context.Receipt.Include(cur => cur.Currency).Include(user => user.UserAccount.Employee).Include(table => table.Table).Where(w => w.BranchID == branchid && w.DateOut >= Convert.ToDateTime(date_from) && w.DateOut <= Convert.ToDateTime(date_to));
                return receipts;
            }
        }
        public IEnumerable<Receipt> GetReceiptCancel(int branchid, string date_from, string date_to)
        {
            if (date_from == null && date_to == null)
            {
                var receipts = _context.Receipt.Include(cur => cur.Currency).Include(user => user.UserAccount.Employee).Include(table => table.Table).Where(w => w.BranchID == branchid && w.DateOut == Convert.ToDateTime(DateTime.Today) && w.Cancel == false && w.Return == false);
                return receipts;
            }
            else
            {
                var receipts = _context.Receipt.Include(cur => cur.Currency).Include(user => user.UserAccount.Employee).Include(table => table.Table).Where(w => w.BranchID == branchid && w.DateOut >= Convert.ToDateTime(date_from) && w.DateOut <= Convert.ToDateTime(date_to) && w.Cancel == false && w.Return == false);
                return receipts;
            }
        }
        //Update VMC 2/12/2020
        public IEnumerable<ReceiptSummary> GetReceiptReturn(int branchid, string date_from, string date_to)
        {
            if (date_from == null && date_to == null)
            {
                var receipts = _context.ReceiptKvms.Where(w => w.BranchID == branchid && w.DateOut == Convert.ToDateTime(DateTime.Today) && !w.Cancel)
                    .OrderByDescending(r => r.ReceiptKvmsID)
                    .Select(r => new ReceiptSummary
                    {
                        ReceiptID = r.ReceiptKvmsID,
                        ReceiptNo = r.ReceiptNo,
                        DateOut = r.DateOut.ToString("MM-dd-yyyy"),
                        TimeOut = DateTime.Parse(r.TimeOut).ToString("hh:mm tt")
                    });
                return receipts;
            }
            else
            {
                var receipts = _context.ReceiptKvms.Where(w => w.BranchID == branchid && w.DateOut >= Convert.ToDateTime(date_from) && w.DateOut <= Convert.ToDateTime(date_to) && !w.Cancel)
                    .Select(r => new ReceiptSummary
                    {
                        ReceiptID = r.ReceiptKvmsID,
                        ReceiptNo = r.ReceiptNo,
                        DateOut = r.DateOut.ToString("MM-dd-yyyy"),
                        TimeOut = DateTime.Parse(r.TimeOut).ToString("hh:mm tt")
                    });
                return receipts;
            }
        }
        public IEnumerable<ServiceItemSales> FilterItemByGroup(int pricelist_id)
        {
            var item = _context.ServiceItemSales.FromSql("pos_FilterItemByGroup @PricelistID={0}",
                parameters: new[] {
                    pricelist_id.ToString()

                });
            return item.ToList();
        }
        public void VoidOrder(int orderid)
        {
            using var transact = _context.Database.BeginTransaction();
            var order = _context.Order.FirstOrDefault(w => w.OrderID == orderid);
            var orderDetails = _context.OrderDetail.Where(od => od.OrderID == orderid);
            order.Cancel = true;
            order.Delete = true;

            var orders = _context.Order.Where(w => w.TableID == order.TableID && order.Cancel == false).ToList();
            var user = _context.UserAccounts.Include(emp => emp.Employee).FirstOrDefault(w => w.ID == order.UserOrderID);

            _context.Update(order);
            _context.SaveChanges();

            List<VoidOrderDetail> lsDetail = new();
            //Add to Detail Void Table
            foreach (var item in orderDetails.ToList())
            {
                VoidOrderDetail detail = new()
                {
                    Code = item.Code,
                    Comment = item.Comment,
                    Cost = item.Cost,
                    Currency = item.Currency,
                    Description = item.Description,
                    DiscountRate = item.DiscountRate,
                    DiscountValue = item.DiscountValue,
                    EnglishName = item.EnglishName,
                    ItemID = item.ItemID,
                    ItemPrintTo = item.ItemPrintTo,
                    ItemStatus = item.ItemStatus,
                    ItemType = item.ItemType,
                    KhmerName = item.KhmerName,
                    Line_ID = item.Line_ID,
                    OrderID = item.OrderID,
                    ParentLevel = item.ParentLevel,
                    PrintQty = item.PrintQty,
                    Qty = item.Qty,
                    Total = item.Total,
                    Total_Sys = item.Total_Sys,
                    TypeDis = item.TypeDis,
                    UnitofMeansure = item.UnitofMeansure,
                    UnitPrice = item.UnitPrice,
                    UomID = item.UomID
                };
                lsDetail.Add(detail);
            }

            //Add to Master Void Table
            VoidOrder voidOrder = new()
            {
                BranchID = order.BranchID,
                Cancel = order.Cancel,
                Change = order.Change,
                Change_Display = order.Change_Display,
                CheckBill = order.CheckBill,
                CompanyID = order.CompanyID,
                Currency = order.Currency,
                CurrencyDisplay = order.CurrencyDisplay,
                CustomerCount = order.CustomerCount,
                CustomerID = order.CustomerID,
                DateIn = order.DateIn,
                DateOut = order.DateOut,
                Delete = order.Delete,
                DiscountRate = order.DiscountRate,
                DiscountValue = order.DiscountValue,
                DisplayRate = order.DisplayRate,
                ExchangeRate = order.ExchangeRate,
                GrandTotal = order.GrandTotal,
                GrandTotal_Display = order.GrandTotal_Display,
                GrandTotal_Sys = order.GrandTotal_Sys,
                LocalCurrencyID = order.LocalCurrencyID,
                OrderNo = order.OrderNo,
                PaymentMeansID = order.PaymentMeansID,
                PriceListID = order.PriceListID,
                QueueNo = order.QueueNo,
                ReceiptNo = order.ReceiptNo,
                Received = order.Received,
                Sub_Total = order.Sub_Total,
                SysCurrencyID = order.SysCurrencyID,
                TableID = order.TableID,
                TaxRate = order.TaxRate,
                TaxValue = order.TaxValue,
                TimeIn = order.TimeIn,
                TimeOut = order.TimeOut,
                Tip = order.Tip,
                TypeDis = order.TypeDis,
                UserDiscountID = order.UserDiscountID,
                UserOrderID = order.UserOrderID,
                WaiterID = order.WaiterID,
                WarehouseID = order.WarehouseID,
                VoidOrderDetail = lsDetail
            };
            _context.VoidOrders.Add(voidOrder);

            //Update status table
            var table_ordered = _context.Order.Where(w => w.TableID == order.TableID && w.Cancel == false).ToList();
            if (table_ordered.Count == 0)
            {
                //_timeDelivery.PushOrderByTableToAvailable(order_update.TableID);
                var time = _timeDelivery.StopTimeTable(order.TableID, 'A');
                var table_up = _context.Tables.Find(order.TableID);
                table_up.Status = 'A';
                table_up.Time = "00:00:00";
                _context.Update(table_up);
                _context.SaveChanges();
            }
            GoodReceiptCommittedOrder(orderid);
            GetOrder(order.TableID, orderid, order.UserOrderID);

            _context.OrderDetail.RemoveRange(orderDetails);
            _context.Order.Remove(order);
            _context.SaveChanges();
            transact.Commit();
        }
        public string GetTimeByTable(int TableID)
        {
            var time_continue = _timeDelivery.GetTimeTable(TableID);
            return time_continue;
        }
        public async Task InitialStatusTable()
        {
            var tables = _context.Tables.Where(w => w.Status != 'A').ToList();
            foreach (var table in tables)
            {
                _timeDelivery.StartTimeTable(table.ID, table.Time, table.Status);
            }
            await UpdateTimeOnTable();
        }
        public async Task UpdateTimeOnTable()
        {
            var tables = _context.Tables.Where(w => w.Status == 'B').ToList();
            foreach (var table in tables)
            {
                var time = _timeDelivery.GetTimeTable(table.ID);
                table.Time = time;
                _context.Update(table);
            }
            _context.SaveChanges();
            await _timeDelivery.StartTimer();
        }
        public void CancelReceipt(int orderid)
        {
            _context.Database.ExecuteSqlCommand("pos_InsertCancelReceipt @OrderID={0}",
              parameters: new[] {
                    orderid.ToString()
              });
            var new_receiptid = _context.Receipt.LastOrDefault();

            //Return Stock Material
            OrderDetailReturnStock(new_receiptid.ReceiptID);
            //var receipt = _context.ReceiptDetail.Include(r=>r.Receipt).Where(w => w.OrderID == orderid);
            //if (receipt != null)
            //{
            //    if (receipt.First().Receipt.Cancel == false)
            //    {
            //        receipt.First().Receipt.Cancel = true;
            //        receipt.First().Receipt.GrandTotal = receipt.First().Receipt.GrandTotal * (-1);
            //        receipt.First().Receipt.GrandTotal_Sys = receipt.First().Receipt.GrandTotal_Sys * (-1);
            //        receipt.First().Receipt.GrandTotal_Display = receipt.First().Receipt.GrandTotal_Display * (-1);
            //        _context.Update(receipt);
            //        _context.SaveChanges();
            //    }
            //    receipt.First().Receipt.ReceiptID = 0;
            //    foreach (var item in receipt)
            //    {
            //        item.ReceiptID = 0;
            //    }
            //    _context.Add(receipt);
            //    _context.SaveChanges();
            //    //PrintReceiptReprint(orderid, "Cancel");
            //}
        }
        // Method get macaddress
        public string GetMacAddress(string ipAddress)
        {
            string macAddress = string.Empty;
            Process pProcess = new();
            pProcess.StartInfo.FileName = "arp.exe";
            pProcess.StartInfo.Arguments = "-a " + ipAddress;
            pProcess.StartInfo.UseShellExecute = false;
            pProcess.StartInfo.RedirectStandardOutput = true;
            pProcess.StartInfo.CreateNoWindow = true;
            pProcess.Start();
            string strOutput = pProcess.StandardOutput.ReadToEnd();
            string[] substrings = strOutput.Split('\n');
            if (substrings.Length >= 5)
            {
                string[] ipaddline = substrings[3].Split(' ');
                string[] ipaddline1 = ipaddline.Where(x => !string.IsNullOrWhiteSpace(x) && (x != "\r")).ToArray();
                return ipaddline1[1];
            }
            else
            {
                return "";
            }
        }
        public string SendReturnItem(List<ReturnItem> returnItems)
        {
            string isReturned = "";
            var MemoNo = "";
            //VMC
            SeriesDetail seriesDetail = new();
            var _docType = _context.DocumentTypes.FirstOrDefault(c => c.Code == "RP");
            var _seriesRP = _context.Series.FirstOrDefault(c => c.DocuTypeID == _docType.ID && c.Default);

            //insert seriesDetail
            seriesDetail.SeriesID = _seriesRP.ID;
            seriesDetail.Number = _seriesRP.NextNo;
            _context.Update(seriesDetail);

            //update series
            string Sno = _seriesRP.NextNo;
            long No = long.Parse(Sno);
            _seriesRP.NextNo = Convert.ToString(No + 1);
            _context.Update(_seriesRP);
            _context.SaveChanges();

            //update Return Receipt No
            MemoNo = _seriesRP.PreFix + "-" + seriesDetail.Number;
            //END VMC

            using (var t = _context.Database.BeginTransaction())
            {
                var receipt = _context.ReceiptKvms.FirstOrDefault(w => w.ReceiptKvmsID == returnItems[0].ReceiptID);
                ReceiptMemo receiptMemo = new();
                //VMC
                receiptMemo.SeriesID = _seriesRP.ID;
                receiptMemo.SeriesDID = seriesDetail.ID;
                receiptMemo.DocTypeID = _docType.ID;
                //END VMC
                receiptMemo.ReceiptKvmsID = receipt.ReceiptKvmsID;
                receiptMemo.OrderNo = receipt.OrderNo;
                receiptMemo.TableID = receipt.TableID;
                receiptMemo.ReceiptMemoNo = MemoNo;
                receiptMemo.ReceiptNo = receipt.ReceiptNo;
                receiptMemo.QueueNo = receipt.OrderNo;
                receiptMemo.DateIn = DateTime.Today;
                receiptMemo.TimeIn = DateTime.Now.ToShortTimeString();
                receiptMemo.DateOut = DateTime.Today;
                receiptMemo.TimeOut = DateTime.Now.ToShortTimeString();
                receiptMemo.WaiterID = receipt.WaiterID;
                receiptMemo.UserOrderID = returnItems.First().UserID;
                receiptMemo.UserDiscountID = receipt.UserDiscountID;
                receiptMemo.CustomerID = receipt.CustomerID;
                receiptMemo.PriceListID = receipt.PriceListID;
                receiptMemo.LocalCurrencyID = receipt.LocalCurrencyID;
                receiptMemo.SysCurrencyID = receipt.SysCurrencyID;
                receiptMemo.ExchangeRate = receipt.ExchangeRate;
                receiptMemo.WarehouseID = receipt.WarehouseID;
                receiptMemo.BranchID = receipt.BranchID;
                receiptMemo.CompanyID = receipt.CompanyID;
                receiptMemo.DisRate = receipt.DiscountRate;
                receiptMemo.TypeDis = receipt.TypeDis;
                receiptMemo.SubTotal = 0;
                receiptMemo.DisValue = 0;
                receiptMemo.TaxRate = 0;
                receiptMemo.TaxValue = 0;
                receiptMemo.GrandTotal = 0;
                receiptMemo.GrandTotalSys = 0;
                receiptMemo.Tip = 0;
                receiptMemo.Received = 0;
                receiptMemo.Change = 0;
                receiptMemo.PaymentMeansID = receipt.PaymentMeansID;
                receiptMemo.CheckBill = 'N';
                receiptMemo.Cancel = false;
                receiptMemo.Delete = false;
                receiptMemo.Return = true;
                receiptMemo.CurrencyDisplay = receipt.CurrencyDisplay;
                receiptMemo.DisplayRate = receipt.DisplayRate;
                receiptMemo.PLCurrencyID = receipt.PLCurrencyID;
                receiptMemo.PLRate = receipt.PLRate;
                receiptMemo.LocalSetRate = receipt.LocalSetRate;
                receiptMemo.BasedOn = receipt.ReceiptKvmsID;
                _context.ReceiptMemo.Add(receiptMemo);
                _context.SaveChanges();

                //Detail
                double SubTotal = 0;
                foreach (var item in returnItems.Where(c => c.ReturnQty > 0).ToList())
                {
                    var reDetailKvms = _context.ReceiptDetailKvms.FirstOrDefault(w => w.ReceiptKvmsID == item.ReceiptID && w.ItemID == item.ItemID);
                    reDetailKvms.OpenQty -= item.ReturnQty;
                    _context.Update(reDetailKvms);
                    _context.SaveChanges();

                    ReceiptDetailMemo detail = new();
                    detail.ReceiptMemoID = receiptMemo.ID;
                    detail.OrderID = reDetailKvms.OrderID;
                    detail.Line_ID = reDetailKvms.Line_ID;
                    detail.ItemID = reDetailKvms.ItemID;
                    detail.Code = reDetailKvms.Code;
                    detail.KhmerName = reDetailKvms.KhmerName;
                    detail.EnglishName = reDetailKvms.EnglishName;
                    detail.Qty = item.ReturnQty;
                    detail.UnitPrice = reDetailKvms.UnitPrice;
                    detail.Cost = reDetailKvms.Cost;
                    detail.DisRate = reDetailKvms.DiscountRate;
                    detail.TypeDis = reDetailKvms.TypeDis;

                    var _disval = (detail.Qty * reDetailKvms.UnitPrice) * (detail.DisRate / 100);

                    if (reDetailKvms.TypeDis == "Percent")
                    {
                        detail.DisValue = _disval;
                        detail.Total = (detail.Qty * detail.UnitPrice) - _disval;
                    }

                    detail.Total = detail.Total;
                    detail.TotalSys = detail.Total * receiptMemo.ExchangeRate;
                    detail.UomID = reDetailKvms.UomID;
                    detail.ItemStatus = reDetailKvms.ItemStatus;
                    detail.Currency = reDetailKvms.Currency;
                    detail.ItemType = reDetailKvms.ItemType;
                    SubTotal += detail.Total;
                    _context.Add(detail);
                    _context.SaveChanges();
                }
                //Update summary
                var receipt_master = _context.ReceiptMemo.FirstOrDefault(c => c.ID == receiptMemo.ID);
                receipt_master.SubTotal = SubTotal;
                if (receipt_master.TypeDis == "Percent")
                {
                    receipt_master.DisValue = SubTotal * receiptMemo.DisRate / 100;
                }
                var vat = (receipt_master.TaxRate + 100 / 100);
                var rate = receipt_master.TaxRate / 100;
                receipt_master.TaxValue = ((SubTotal / vat) * rate);
                receipt_master.GrandTotal = (SubTotal - receipt_master.DisValue);
                receipt_master.AppliedAmount = receipt_master.GrandTotal;
                receipt_master.GrandTotalSys = (receipt_master.GrandTotal * receipt_master.ExchangeRate);
                receipt_master.GrandTotalDisplay = 0;
                _context.Update(receipt_master);
                _context.SaveChanges();
                OrderDetailReturnStock(receiptMemo.ReceiptKvmsID);

                //Update ReceitpKvms
                receipt.AppliedAmount += receipt_master.GrandTotal;
                receipt.BalanceDue = receipt.GrandTotal - receipt.AppliedAmount;
                if (receipt.BalanceDue == 0)
                {
                    receipt.Status = StatusReceipt.Paid;
                }
                else
                {
                    receipt.Status = StatusReceipt.Aging;
                }
                receipt.Return = true;
                _context.Update(receipt);
                _context.SaveChanges();
                t.Commit();
                isReturned = "true";
            }
            return isReturned;
        }

        public void OrderDetailReturnStock(int receiptid_new)
        {
            var cancelreceipt = _context.ReceiptMemo.First(r => r.ReceiptKvmsID == receiptid_new);
            var cancelReDetail = _context.ReceiptDetailMemoKvms.Where(d => d.ReceiptMemoID == cancelreceipt.ID);
            foreach (var itemReDetail in cancelReDetail.ToList())
            {
                var itemMaster = _context.ItemMasterDatas.FirstOrDefault(w => w.ID == itemReDetail.ItemID);
                if (itemMaster.Process != "Standard")
                {
                    //var receiptno = cancelreceipt.ReceiptNo.Split("-")[0];
                    //var item_master_data = _context.ItemMasterDatas.FirstOrDefault(w => w.ID == item_cancel.ItemID);
                    var item_warehouse_material = _context.WarehouseSummary.FirstOrDefault(w => w.WarehouseID == cancelreceipt.WarehouseID && w.ItemID == itemReDetail.ItemID);
                    var all_item_warehouse_detail = _context.WarehouseDetails.Where(w => w.WarehouseID == cancelreceipt.WarehouseID && w.ItemID == itemReDetail.ItemID);
                    var item_cost = _context.InventoryAudits.FirstOrDefault(w => w.InvoiceNo == cancelreceipt.ReceiptNo && w.ItemID == itemReDetail.ItemID);
                    //
                    //update_warehouse_summary && itemmasterdata
                    var warehouseSummary = _context.WarehouseSummary.FirstOrDefault(w => w.ItemID == itemReDetail.ItemID && w.WarehouseID == cancelreceipt.WarehouseID);

                    double @Qty = itemReDetail.Qty;
                    double @Cost = item_cost.Cost;
                    warehouseSummary.InStock += @Qty;
                    itemMaster.StockIn += @Qty;
                    //insert_warehousedetail
                    var inventoryAudit = new InventoryAudit();
                    var warehouseDetail = new WarehouseDetail();
                    warehouseDetail.WarehouseID = cancelreceipt.WarehouseID;
                    warehouseDetail.UomID = itemReDetail.UomID;
                    warehouseDetail.UserID = cancelreceipt.UserOrderID;
                    warehouseDetail.SyetemDate = cancelreceipt.DateOut;
                    warehouseDetail.TimeIn = DateTime.Now;
                    warehouseDetail.InStock = @Qty;
                    warehouseDetail.CurrencyID = cancelreceipt.SysCurrencyID;
                    warehouseDetail.ItemID = itemReDetail.ItemID;
                    warehouseDetail.Cost = @Cost;
                    if (itemMaster.Process == "FIFO")
                    {
                        var inventory_audit = _context.InventoryAudits.Where(w => w.ItemID == itemReDetail.ItemID && w.WarehouseID == cancelreceipt.WarehouseID);
                        inventoryAudit.ID = 0;
                        inventoryAudit.WarehouseID = cancelreceipt.WarehouseID;
                        inventoryAudit.BranchID = cancelreceipt.BranchID;
                        inventoryAudit.UserID = cancelreceipt.UserOrderID;
                        inventoryAudit.ItemID = itemReDetail.ItemID;
                        inventoryAudit.CurrencyID = cancelreceipt.SysCurrencyID;
                        inventoryAudit.UomID = itemReDetail.UomID;
                        inventoryAudit.InvoiceNo = cancelreceipt.ReceiptNo;
                        inventoryAudit.Trans_Type = "RM";
                        inventoryAudit.Process = itemMaster.Process;
                        inventoryAudit.SystemDate = DateTime.Now;
                        inventoryAudit.TimeIn = DateTime.Now.ToShortTimeString();
                        inventoryAudit.Qty = @Qty;
                        inventoryAudit.Cost = @Cost;
                        inventoryAudit.Price = 0;
                        inventoryAudit.CumulativeQty = (inventory_audit.Sum(q => q.Qty)) + (@Qty);
                        inventoryAudit.CumulativeValue = (inventory_audit.Sum(s => s.Trans_Valuse)) + (@Qty * @Cost);
                        inventoryAudit.Trans_Valuse = @Qty * @Cost;
                        inventoryAudit.LocalCurID = cancelreceipt.LocalCurrencyID;
                        inventoryAudit.LocalSetRate = cancelreceipt.LocalSetRate;
                    }
                    else
                    {
                        var inventory_audit = _context.InventoryAudits.Where(w => w.ItemID == itemReDetail.ItemID && w.WarehouseID == cancelreceipt.WarehouseID);
                        double @AvgCost = (inventory_audit.Sum(s => s.Trans_Valuse) + @Cost) / (inventory_audit.Sum(s => s.Qty) + Qty);
                        inventoryAudit.ID = 0;
                        inventoryAudit.WarehouseID = cancelreceipt.WarehouseID;
                        inventoryAudit.BranchID = cancelreceipt.BranchID;
                        inventoryAudit.UserID = cancelreceipt.UserOrderID;
                        inventoryAudit.ItemID = itemReDetail.ItemID;
                        inventoryAudit.CurrencyID = cancelreceipt.SysCurrencyID;
                        inventoryAudit.UomID = itemReDetail.UomID;
                        inventoryAudit.InvoiceNo = cancelreceipt.ReceiptNo;
                        inventoryAudit.Trans_Type = "RM";
                        inventoryAudit.Process = itemMaster.Process;
                        inventoryAudit.SystemDate = DateTime.Now;
                        inventoryAudit.TimeIn = DateTime.Now.ToShortTimeString();
                        inventoryAudit.Qty = @Qty;
                        inventoryAudit.Cost = @AvgCost;
                        inventoryAudit.Price = 0;
                        inventoryAudit.CumulativeQty = (inventory_audit.Sum(q => q.Qty)) + (@Qty);
                        inventoryAudit.CumulativeValue = (inventory_audit.Sum(s => s.Trans_Valuse)) + (@Qty * @AvgCost);
                        inventoryAudit.Trans_Valuse = @Qty * @AvgCost;
                        inventoryAudit.LocalCurID = cancelreceipt.LocalCurrencyID;
                        inventoryAudit.LocalSetRate = cancelreceipt.LocalSetRate;
                        UpdateAvgCost(itemReDetail.ItemID, cancelreceipt.WarehouseID, itemMaster.GroupUomID, @Qty, @AvgCost);
                        UpdateBomCost(itemReDetail.ItemID, @Qty, @AvgCost);
                    }
                    _context.InventoryAudits.Update(inventoryAudit);
                    _context.WarehouseSummary.Update(warehouseSummary);
                    _context.ItemMasterDatas.Update(itemMaster);
                    _context.WarehouseDetails.Update(warehouseDetail);
                    _context.SaveChanges();
                }
            }

            foreach (var item in cancelReDetail.ToList())
            {
                var itemM = _context.ItemMasterDatas.FirstOrDefault(w => w.ID == item.ItemID);
                var orft = _context.GroupDUoMs.FirstOrDefault(w => w.GroupUoMID == itemM.GroupUomID && w.AltUOM == item.UomID);
                var bom = _context.BOMaterial.FirstOrDefault(w => w.ItemID == item.ItemID) ?? new BOMaterial();
                var items_material = (from bomd in _context.BOMDetail.Where(w => w.BID == bom.BID && w.Detele == false)
                                      join i in _context.ItemMasterDatas on bomd.ItemID equals i.ID
                                      join gd in _context.GroupDUoMs on bomd.UomID equals gd.AltUOM
                                      join uom in _context.UnitofMeasures on i.InventoryUoMID equals uom.ID
                                      select new
                                      {
                                          bomd.ItemID,
                                          gd.GroupUoMID,
                                          GUoMID = i.GroupUomID,
                                          Qty = ((double)item.Qty * orft.Factor) * ((double)bomd.Qty * gd.Factor),
                                          bomd.NegativeStock,
                                          i.Process,
                                          UomID = uom.ID,
                                          gd.Factor
                                      }).Where(w => w.GroupUoMID == w.GUoMID).ToList();
                if (items_material != null)
                {
                    foreach (var item_cancel in items_material.ToList())
                    {
                        //var receiptno = cancelreceipt.ReceiptNo.Split(" ")[0];
                        //var item_master_data = _context.ItemMasterDatas.FirstOrDefault(w => w.ID == item_cancel.ItemID);
                        var item_warehouse_material = _context.WarehouseSummary.FirstOrDefault(w => w.WarehouseID == cancelreceipt.WarehouseID && w.ItemID == item_cancel.ItemID);
                        var all_item_warehouse_detail = _context.WarehouseDetails.Where(w => w.WarehouseID == cancelreceipt.WarehouseID && w.ItemID == item_cancel.ItemID);
                        var item_cost = _context.InventoryAudits.FirstOrDefault(w => w.InvoiceNo == cancelreceipt.ReceiptNo && w.ItemID == item_cancel.ItemID);
                        //
                        //update_warehouse_summary && itemmasterdata
                        var warehouseSummary = _context.WarehouseSummary.FirstOrDefault(w => w.ItemID == item_cancel.ItemID && w.WarehouseID == cancelreceipt.WarehouseID);
                        var itemMaster = _context.ItemMasterDatas.FirstOrDefault(w => w.ID == item_cancel.ItemID);
                        double @Qty = item_cancel.Qty;
                        double @Cost = item_cost.Cost;
                        warehouseSummary.InStock += @Qty;
                        itemMaster.StockIn += @Qty;
                        //insert_warehousedetail
                        var inventoryAudit = new InventoryAudit();
                        var warehouseDetail = new WarehouseDetail();
                        warehouseDetail.WarehouseID = cancelreceipt.WarehouseID;
                        warehouseDetail.UomID = item_cancel.UomID;
                        warehouseDetail.UserID = cancelreceipt.UserOrderID;
                        warehouseDetail.SyetemDate = cancelreceipt.DateOut;
                        warehouseDetail.TimeIn = DateTime.Now;
                        warehouseDetail.InStock = @Qty;
                        warehouseDetail.CurrencyID = cancelreceipt.SysCurrencyID;
                        warehouseDetail.ItemID = item_cancel.ItemID;
                        warehouseDetail.Cost = @Cost;
                        if (itemMaster.Process == "FIFO")
                        {
                            var inventory_audit = _context.InventoryAudits.Where(w => w.ItemID == item_cancel.ItemID && w.WarehouseID == cancelreceipt.WarehouseID);
                            inventoryAudit.ID = 0;
                            inventoryAudit.WarehouseID = cancelreceipt.WarehouseID;
                            inventoryAudit.BranchID = cancelreceipt.BranchID;
                            inventoryAudit.UserID = cancelreceipt.UserOrderID;
                            inventoryAudit.ItemID = item_cancel.ItemID;
                            inventoryAudit.CurrencyID = cancelreceipt.SysCurrencyID;
                            inventoryAudit.UomID = item_cancel.UomID;
                            inventoryAudit.InvoiceNo = cancelreceipt.ReceiptNo;
                            inventoryAudit.Trans_Type = "RM";
                            inventoryAudit.Process = itemMaster.Process;
                            inventoryAudit.SystemDate = DateTime.Now;
                            inventoryAudit.TimeIn = DateTime.Now.ToShortTimeString();
                            inventoryAudit.Qty = @Qty;
                            inventoryAudit.Cost = @Cost;
                            inventoryAudit.Price = 0;
                            inventoryAudit.CumulativeQty = (inventory_audit.Sum(q => q.Qty)) + (@Qty);
                            inventoryAudit.CumulativeValue = (inventory_audit.Sum(s => s.Trans_Valuse)) + (@Qty * @Cost);
                            inventoryAudit.Trans_Valuse = @Qty * @Cost;
                            inventoryAudit.LocalCurID = cancelreceipt.LocalCurrencyID;
                            inventoryAudit.LocalSetRate = cancelreceipt.LocalSetRate;
                        }
                        else
                        {
                            var inventory_audit = _context.InventoryAudits.Where(w => w.ItemID == item_cancel.ItemID && w.WarehouseID == cancelreceipt.WarehouseID);
                            double @AvgCost = (inventory_audit.Sum(s => s.Trans_Valuse) + @Cost) / (inventory_audit.Sum(s => s.Qty) + Qty);
                            inventoryAudit.ID = 0;
                            inventoryAudit.WarehouseID = cancelreceipt.WarehouseID;
                            inventoryAudit.BranchID = cancelreceipt.BranchID;
                            inventoryAudit.UserID = cancelreceipt.UserOrderID;
                            inventoryAudit.ItemID = item_cancel.ItemID;
                            inventoryAudit.CurrencyID = cancelreceipt.SysCurrencyID;
                            inventoryAudit.UomID = item_cancel.UomID;
                            inventoryAudit.InvoiceNo = cancelreceipt.ReceiptNo;
                            inventoryAudit.Trans_Type = "RM";
                            inventoryAudit.Process = itemMaster.Process;
                            inventoryAudit.SystemDate = DateTime.Now;
                            inventoryAudit.TimeIn = DateTime.Now.ToShortTimeString();
                            inventoryAudit.Qty = @Qty;
                            inventoryAudit.Cost = @AvgCost;
                            inventoryAudit.Price = 0;
                            inventoryAudit.CumulativeQty = (inventory_audit.Sum(q => q.Qty)) + (@Qty);
                            inventoryAudit.CumulativeValue = (inventory_audit.Sum(s => s.Trans_Valuse)) + (@Qty * @AvgCost);
                            inventoryAudit.Trans_Valuse = @Qty * @AvgCost;
                            inventoryAudit.LocalCurID = cancelreceipt.LocalCurrencyID;
                            inventoryAudit.LocalSetRate = cancelreceipt.LocalSetRate;
                            UpdateAvgCost(item_cancel.ItemID, cancelreceipt.WarehouseID, item_cancel.GUoMID, @Qty, @AvgCost);
                            UpdateBomCost(item_cancel.ItemID, @Qty, @AvgCost);
                        }
                        _context.InventoryAudits.Update(inventoryAudit);
                        _context.WarehouseSummary.Update(warehouseSummary);
                        _context.ItemMasterDatas.Update(itemMaster);
                        _context.WarehouseDetails.Update(warehouseDetail);
                        _context.SaveChanges();
                    }
                }

            }
        }
        //update_AvgCost
        public void UpdateAvgCost(int itemid, int whid, int guomid, double qty, double avgcost)
        {
            // update pricelistdetial
            var inventory_audit = _context.InventoryAudits.Where(w => w.ItemID == itemid);
            var pri_detial = _context.PriceListDetails.Where(w => w.ItemID == itemid);
            double @AvgCost = ((inventory_audit.Sum(s => s.Trans_Valuse) + avgcost) / (inventory_audit.Sum(q => q.Qty) + qty));
            foreach (var pri in pri_detial)
            {
                var guom = _context.GroupDUoMs.FirstOrDefault(g => g.GroupUoMID == guomid && g.AltUOM == pri.UomID);
                var exp = _context.ExchangeRates.FirstOrDefault(w => w.CurrencyID == pri.CurrencyID);
                pri.Cost = @AvgCost * exp.SetRate * guom.Factor;
                _context.PriceListDetails.Update(pri);
            }
            //update_waresummary
            var inventory_warehouse = _context.InventoryAudits.Where(w => w.ItemID == itemid && w.WarehouseID == whid);
            var waresummary = _context.WarehouseSummary.FirstOrDefault(w => w.ItemID == itemid && w.WarehouseID == whid);
            double @AvgCostWare = (inventory_warehouse.Sum(s => s.Trans_Valuse)) / (inventory_warehouse.Sum(s => s.Qty));
            waresummary.Cost = @AvgCostWare;
            _context.WarehouseSummary.Update(waresummary);
            _context.SaveChanges();
        }
        //update_bomCost
        public void UpdateBomCost(int itemid, double qty, double avgcost)
        {
            var ItemBOMDetail = _context.BOMDetail.Where(w => w.ItemID == itemid);
            foreach (var itembom in ItemBOMDetail)
            {
                var Inven = _context.InventoryAudits.Where(w => w.ItemID == itembom.ItemID);
                double @AvgCost = (Inven.Sum(s => s.Trans_Valuse) + avgcost) / (Inven.Sum(q => q.Qty) + qty);
                var Gdoum = _context.GroupDUoMs.Where(w => w.GroupUoMID == itembom.GUomID);
                var Factor = Gdoum.FirstOrDefault(w => w.AltUOM == itembom.UomID).Factor;
                itembom.Cost = @AvgCost;
                itembom.Amount = itembom.Qty * @AvgCost;
                _context.BOMDetail.UpdateRange(itembom);
                _context.SaveChanges();
                // sum 
                var BOM = _context.BOMaterial.FirstOrDefault(w => w.BID == itembom.BID);
                var DBOM = _context.BOMDetail.Where(w => w.BID == BOM.BID && !w.Detele);
                BOM.TotalCost = DBOM.Sum(s => s.Amount);
                _context.BOMaterial.Update(BOM);
                _context.SaveChanges();
            }
        }


    }


}
