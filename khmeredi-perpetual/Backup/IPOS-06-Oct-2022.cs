using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using CKBS.AppContext;
using CKBS.Controllers.Event;
using CKBS.Models.Services.Administrator.General;
using CKBS.Models.Services.Administrator.Inventory;
using CKBS.Models.Services.Administrator.Tables;
using CKBS.Models.Services.Banking;
using CKBS.Models.Services.Inventory;
using CKBS.Models.Services.Inventory.Category;
using CKBS.Models.Services.POS;
using CKBS.Models.Services.POS.service;
using CKBS.Models.Services.Production;
using CKBS.Models.ServicesClass;
using CKBS.Models.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Receipt = CKBS.Models.Services.POS.Receipt;
using CKBS.Models.Services.POS.Template;
using CKBS.Models.Services.Administrator.SystemInitialization;
using CKBS.Models.Services.Financials;
using Type = CKBS.Models.Services.Financials.Type;
using CKBS.Models.Services.ChartOfAccounts;
using SetGlAccount = CKBS.Models.Services.Inventory.SetGlAccount;
using CKBS.Models.Services.POS.KVMS;
using CKBS.Models.Services.Administrator.Setup;
using CKBS.Models.Services.AlertManagement;
using TelegramBotAPI.Models;
using CKBS.AlertManagementsServices.Repositories;
using CKBS.Models.ServicesClass.AlertViewClass;
using KEDI.Core.Premise.Models.ServicesClass.SerialBatches.Sales;
using KEDI.Core.Premise.Models.Services.Administrator.Inventory;
using KEDI.Core.Premise.Repository;
using KEDI.Core.Premise.Models.Services.POS.KSMS;
using KEDI.Core.Premise.Models.Services.Banking;
using KEDI.Core.Premise.Models.Services.Administrator.SetUp;
using KEDI.Core.Premise.Models.Services.CardMembers;
using KEDI.Core.Premise.Models.ServicesClass.PrintBarcode;
using CKBS.Models.Services.Inventory.PriceList;
using CKBS.Models.Services.ReportSale.dev;
using KEDI.Core.Premise.Models.Services.POS.CanRing;
using KEDI.Core.Premise.Models.ServicesClass.SerialBatches.Purchases;
using KEDI.Core.Premise.Models.Services.POS;
using KEDI.Core.Premise.Services;
using KEDI.Core.Premise.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CKBS.Models.Services.Responsitory
{
    public interface IPOS
    {
        //Get groups
        IEnumerable<ItemGroup3> GetGroups();
        IEnumerable<ItemGroup1> GetGroup1s { get; }
        IEnumerable<ItemGroup2> GetGroup2s { get; }

        IEnumerable<ItemGroup3> GetGroup3s { get; }
        //Filter Group

        IEnumerable<ItemGroup2> FilterGroup2(int group1_id);
        IEnumerable<ItemGroup3> FilterGroup3(int group1_id, int group2_id);

        Task<IEnumerable<ServiceItemSales>> GetItemMasterDatasAsync(int priceList_id, int comId, string barcode = "");
        ItemMasterData GetItemMasterDataById(int id);
        IEnumerable<ServiceItemSales> GetItemMasterByBarcode(int pricelist, string barcode);
        IEnumerable<ServiceItemSales> GetScaleItemMasterByBarcode(int pricelist, string scale_barcode, double scaleprice);
        IEnumerable<ServiceItemSales> FilterItem(int pricelist_id, int itemid);
        IEnumerable<ServiceItemSales> FilterItemByGroup(int pricelist_id);
        IEnumerable<GeneralSetting> GetSetting(int branchid);
        //Order GetOrder(Order order);
        IEnumerable<Order> GetOrder(int tableid, int orderid, int userid);
        IEnumerable<POS.Receipt> GetReceiptReprint(int branchid, string date_from, string date_to);
        IEnumerable<POS.Receipt> GetReceiptCancel(int branchid, string date_from, string date_to);
        IEnumerable<ReceiptSummary> GetReceiptReturn(int userId, int branchId, string date_from, string date_to);
        //Task<ItemsReturnObj> SendOrderAsync(Order data, string print_type, List<SerialNumber> serials = null, List<BatchNo> batches = null);
        IEnumerable<OpenShift> OpenShiftData(int userid, double cash);
        Task<List<CashoutReportMaster>> CloseShiftData(int userid, double cashout, bool isWebClient = true);
        void SaveOrder(Order data);
        //void SendDataToSecondScreen(Order order);
        Task<int> PrintReceiptBillAsync(Order order, string print_type, List<SerialNumber> serials, List<BatchNo> batches);
        Task<List<PrintBill>> PrintReceiptReprintAsync(int orderid, string print_type, int userId, bool isWebClient = true);
        void CancelReceipt(Receipt receipt, List<SerialNumber> serials, List<BatchNo> batches,int? PaymentmeansID=0);
        int MoveReceipt(int fromTableId, int toTableId, int orderId);
        int MoveTable(int fromTableId, int toTableId);
        void CombineOrders(CombineOrder combineReceipt);
        Order SendSplit(Order splitOrder);
        void ClearUserOrder(int tableid);
        Task InitialStatusTable();
        Task UpdateTimeOnTable();
        void VoidOrder(int orderId, string reason);
        string GetTimeByTable(int TableID);
        bool SendReturnItem(List<ReturnItem> returnItems, List<SerialNumber> serials, List<BatchNo> batches,int? PaymentmeansID=0);
        Task PrintBarcodeItemsAsync(List<ItemPrintBarcodeView> items, string printerName);
        void ResetQueue(GeneralSetting setting);
        string CreateQueueNumber(GeneralSetting setting);
        List<CashoutReportMaster> ReprintCloseShift(int userid, int closeShiftId, bool isWebClient = true);
        void UpdateTableVoidItem(int tableId);
        void SendToPrintOrder(Order order, string type = "");
        void IssuseStockCanRing(CanRingMaster crm, List<SerialNumber> serials, List<BatchNo> batches, List<SerialViewModelPurchase> serialViewModelPurchases, List<BatchViewModelPurchase> batchViewModelPurchases);
        void IssuseInStock(int receipt, OrderStatus orderStaus, List<SerialNumber> serials, List<BatchNo> batches, List<MultiPaymentMeans> multiPayments);
        Task IssueStockAsync(Receipt receipt, OrderStatus orderStaus, List<SerialNumber> serials, List<BatchNo> batches, List<MultiPaymentMeans> multiPayments);
        Task<ItemsReturnObj> SubmitOrderAsync(Order data, string printType, List<SerialNumber> serials = null, List<BatchNo> batches = null);
    }

    public class POSRepository : IPOS
    {
        private readonly ILogger<PosRetailModule> _logger;
        private readonly DataContext _context;
        private readonly IReport _report;
        private readonly TimeDelivery _timeDelivery;
        private readonly ITelegramApiCleint _teleBot;
        private readonly ICheckFrequently _check;
        private readonly IActionContextAccessor _accessor;
        private readonly IHubContext<AlertSignalRClient> _hubContext;
        private readonly LoyaltyProgramPosModule _loyaltyProgram;
        private readonly UtilityModule _utility;
        private readonly IDataPropertyRepository _dataProp;
        private readonly IPosClientSignal _clientSignal;
        public POSRepository(ILogger<PosRetailModule> logger, DataContext context, IHubContext<SignalRClient> timehubcontext, IReport report,
            IActionContextAccessor accessor, ITelegramApiCleint telegramApi, UtilityModule utility,
            ICheckFrequently check, IHubContext<AlertSignalRClient> hubContext, LoyaltyProgramPosModule loyaltyProgram,
            IDataPropertyRepository dataProperty, IPosClientSignal clientSignal)
        {
            _logger = logger;
            _context = context;
            _report = report;
            _accessor = accessor;
            _timeDelivery = TimeDelivery.GetInstance(timehubcontext);

            _teleBot = telegramApi;
            _check = check;
            _hubContext = hubContext;
            _loyaltyProgram = loyaltyProgram;
            _utility = utility;
            _dataProp = dataProperty;
            _clientSignal = clientSignal;
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

        public async Task<IEnumerable<ServiceItemSales>> GetItemMasterDatasAsync(int priceList_id, int comId, string barcode = "")
        {
            var list = await _context.ServiceItemSales.FromSql("pos_GetItemForSale @PricelistID={0}",
                parameters: new[] {
                    priceList_id.ToString()
                }).ToListAsync();
            if (!string.IsNullOrEmpty(barcode)) list = list.Where(i => string.Compare(i.Barcode, barcode, true) == 0).ToList();
            _dataProp.DataProperty(list, comId, "ItemID", "AddictionProps");
            return list;
        }

        public ItemMasterData GetItemMasterDataById(int id)
        {
            return _context.ItemMasterDatas.Find(id) ?? new ItemMasterData();
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

        public Order SendSplit(Order splitOrder)
        {
            var _order = _context.Order.Include(o => o.OrderDetail).FirstOrDefault(o => o.OrderID == splitOrder.OrderID);
            if (_order == null) { return splitOrder; }
            using (var t = _context.Database.BeginTransaction())
            {
                foreach (var item in _order.OrderDetail)
                {
                    foreach (var _item in splitOrder.OrderDetail)
                    {
                        if (item.OrderDetailID == _item.OrderDetailID)
                        {
                            if (_item.PrintQty > 0 && _item.PrintQty <= item.Qty)
                            {
                                item.Qty -= _item.PrintQty;
                                item.PrintQty = 0;
                                if (string.Compare(item.TypeDis, "percent", true) == 0)
                                {
                                    item.Total = (item.Qty * item.UnitPrice) * (1 - (item.DiscountRate / 100));
                                    item.DiscountValue = item.Qty * item.UnitPrice * item.DiscountRate / 100;
                                }
                                else
                                {
                                    item.Total = (item.Qty * item.UnitPrice) - item.DiscountRate;
                                    item.DiscountValue = item.DiscountRate;
                                }
                                if (item.Qty == 0) _context.OrderDetail.Remove(item);
                            }
                        }
                    }
                }

                _context.SaveChanges();
                if (splitOrder.OrderDetail.Count > 0)
                {
                    splitOrder = AddNewSplitOrder(splitOrder);
                    _order.CheckBill = 'N';
                    _context.Order.Update(_order);
                    _context.SaveChanges();
                    UpdateTableTimeContiuely(splitOrder.TableID, true);
                }

                t.Commit();
            }
            return splitOrder;
        }

        public Order AddNewSplitOrder(Order order)
        {
            var setting = _context.GeneralSettings.FirstOrDefault(w => w.BranchID == order.BranchID);
            var queues = _context.Order_Queue.Where(w => w.BranchID == order.BranchID).ToList();
            string queueNo = CreateQueueNumber(setting);
            Order orderNew = new();
            orderNew.OrderNo = "Split-" + queueNo;
            orderNew.TableID = order.TableID;
            orderNew.ReceiptNo = order.ReceiptNo ?? "";
            orderNew.QueueNo = (queues.Count + 1).ToString();
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
            orderNew.PLCurrencyID = order.PLCurrencyID;
            orderNew.PLRate = order.PLRate;
            orderNew.LocalSetRate = order.LocalSetRate;
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
            orderNew.VehicleID = order.VehicleID;
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
                detail.LineID = item.LineID;
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
                detail.Total = (detail.Qty * detail.UnitPrice) - detail.DiscountRate;
                detail.DiscountValue = detail.DiscountRate;
                detail.Total_Sys = detail.Total * orderNew.ExchangeRate;
                detail.GroupUomID = item.GroupUomID;
                detail.UomID = item.UomID;
                detail.Uom = item.Uom;
                detail.ItemStatus = item.ItemStatus;
                detail.ItemPrintTo = item.ItemPrintTo;
                detail.Currency = item.Currency;
                detail.ItemType = item.ItemType;
                detail.KSServiceSetupId = item.KSServiceSetupId;
                detail.VehicleId = item.VehicleId;
                detail.IsKsms = item.IsKsms;
                detail.IsKsmsMaster = item.IsKsmsMaster;
                detail.IsScale = item.IsScale;
                detail.IsReadonly = item.IsReadonly;
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
            var vat = order_master.TaxRate + 100 / 100;
            var rate = order_master.TaxRate / 100;
            order_master.TaxValue = SubTotal / vat * rate;
            order_master.GrandTotal = SubTotal - order_master.DiscountValue;
            order_master.GrandTotal_Sys = order_master.GrandTotal * order_master.ExchangeRate;
            _context.Update(order_master);
            _context.SaveChanges();
            return order_master;
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

        public void SendToPrintOrder(Order order, string type = "")
        {
            var Table = _context.Tables.Find(order.TableID);
            var User = _context.UserAccounts.Include(emp => emp.Employee).FirstOrDefault(user => user.ID == order.UserOrderID);
            var PrinterNames = _context.PrinterNames.Where(w => w.Delete == false).ToList();
            var Setting = _context.GeneralSettings.Where(w => w.UserID == order.UserOrderID).ToList();
            List<PrintOrder> items = new();
            //var lineItems = order.OrderDetail.Where(od => od.PrintQty != 0).ToList();
            foreach (var item in order.OrderDetail.ToList())
            {
                var uom = _context.UnitofMeasures.First(u => u.ID == item.UomID);
                //var orderDetail = _context.OrderDetail.AsNoTracking().FirstOrDefault(w => w.LineID == item.LineID && w.OrderID == order.OrderID);
                if (string.IsNullOrEmpty(item.Comment)) item.Comment = "";             
                if (item.PrintQty != 0)
                {
                    PrintOrder data = new()
                    {
                        LineID = item.LineID.ToString(),
                        Table = Table.Name,
                        Cashier = User.Employee.Name,
                        OrderNo = order.OrderNo,
                        Item = item.KhmerName,
                        PrintQty = item.PrintQty.ToString(),
                        Uom = uom.Name,
                        ItemPrintTo = item.ItemPrintTo,
                        ParentLineID = item.ParentLineID,
                        ItemType = item.ItemType,
                        Price = item.Currency + ' ' + string.Format("{0:n2}", item.UnitPrice),
                        LinePosition = item.LinePosition.ToString(),
                        Comment = item.Comment,
                    };
                    items.Add(data);
                }

                if (item.Qty <= 0)
                {
                    if (item != null)
                    {
                        _context.OrderDetail.Remove(item);
                        _context.SaveChanges();
                    }

                    if (item.ItemType == "Service")
                    {
                        UpdateTableTimeContiuely(order.TableID);
                    }
                }
            };

            if (items.Count > 0)
            {
                if (type == "Send")
                {
                    UpdateTableTimeContiuely(order.TableID, true);
                }
                _timeDelivery.PrintOrder(items, PrinterNames, Setting);
            }
        }

        static string CheckCommentItem(string comment, string updateComment)
        {
            if (comment != null && !string.IsNullOrEmpty(updateComment))
            {
                List<string> comments = new(comment.Split("\n"));
                List<string> updateComments = new(updateComment.Split("\n"));

                foreach (var i in updateComments.ToList())
                {
                    var com = comments.FirstOrDefault(c => i.Trim() == c.Trim());
                    if (!string.IsNullOrEmpty(com)) updateComments.Remove(i);
                }
                return SetCommentItem(updateComments);
            }
            else
            {
                return "";
            }

        }
        private static string SetCommentItem(List<string> comSplit)
        {
            string _comment = "";
            foreach (var (item, index) in comSplit.ToList().Select((value, i) => (value, i)))
            {
                var slash = "";
                int length = comSplit.Count - 1;

                if (index == length) slash = "";
                else slash = "\n";

                _comment += $"{item} {slash}";
            }
            return _comment;
        }
        void UpdateTableTimeContiuely(int tableId, bool sendAfterBill = false)
        {
            var time_continue = _timeDelivery.GetTimeTable(tableId);
            var table_up = _context.Tables.Find(tableId);
            table_up.Status = 'B';
            table_up.Time = time_continue;
            _context.Update(table_up);
            _context.SaveChanges();
            if (sendAfterBill) _timeDelivery.StartTimeTable(tableId, time_continue, 'B');
        }

        private async Task IssueCommittedOrderAsync(int orderid)
        {
            await _context.Database.ExecuteSqlCommandAsync("pos_OrderDetailCommittedStock @OrderID={0}",
               parameters: new[] {
                    orderid.ToString()
               });
        }

        private async Task IssueCommittedMaterialAsync(int orderId)
        {
            var order = _context.Order.AsNoTracking().FirstOrDefault(w => w.OrderID == orderId) ?? new Order();
            var orderDetails = await _context.OrderDetail.AsNoTracking().Where(w => w.OrderID == orderId).ToListAsync();
            var WID = _context.Warehouses.First(w => w.ID == order.WarehouseID) ?? new Warehouse();
            foreach (var item in orderDetails)
            {
                var bom = _context.BOMaterial.FirstOrDefault(w => w.ItemID == item.ItemID && w.Active == true) ?? new BOMaterial();
                var items_material = (from bomd in _context.BOMDetail.Where(w => w.BID == bom.BID && w.Detele == false)
                                      join i in _context.ItemMasterDatas.Where(im => !im.Delete) on bomd.ItemID equals i.ID
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
                        await _context.SaveChangesAsync();
                    }
                }
            }
        }

        public void GoodReceiptCommittedOrder(int orderid)
        {
            _context.Database.ExecuteSqlCommand("pos_OrderDetailCommittedStockGoodReceipt @OrderID={0}",
               parameters: new[] {
                    orderid.ToString()
               });
        }
        public void IssuseInStockOrder(int orderid)
        {
            _context.Database.ExecuteSqlCommand("pos_OrderDetailIssuseStock @OrderID={0}",
               parameters: new[] {
                    orderid.ToString()
               });
            var Order = _context.Order.First(w => w.OrderID == orderid) ?? new Order();
            var OrderDetail = _context.OrderDetail.Where(w => w.OrderID == orderid).ToList();
            foreach (var item in OrderDetail)
            {
                var itemMaster = _context.ItemMasterDatas.FirstOrDefault(w => w.ID == item.ItemID && !w.Delete);
                if (itemMaster.Process == "Average")
                {
                    var guom = _context.GroupDUoMs.FirstOrDefault(g => g.GroupUoMID == itemMaster.GroupUomID && g.AltUOM == item.UomID);
                    var wareSummary = _context.WarehouseSummary.FirstOrDefault(w => w.ItemID == item.ItemID && w.WarehouseID == Order.WarehouseID);
                    double @Qty = item.Qty * guom.Factor;
                    _utility.UpdateAvgCost(item.ItemID, Order.WarehouseID, itemMaster.GroupUomID, @Qty, wareSummary.Cost);
                    _utility.UpdateBomCost(item.ItemID, @Qty, wareSummary.Cost);
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
        //===============================getorder=========================
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
                var order = _context.Order.Include(x => x.OrderDetail).Include(x => x.Currency).Where(w => w.TableID == tableid && w.OrderID == orderid && w.Cancel == false).ToList();

                _timeDelivery.PushOrderByTable(orders, tableid, user.Employee.Name);
                return order;
            }

        }

        //===============================getorder=======================
        #region
        //public Order GetOrder(Order order)
        //{
        //    var orders = _context.Order.Where(w => w.TableID == order.TableID && !w.Cancel).ToList();
        //    var user = _context.UserAccounts.Include(emp => emp.Employee).FirstOrDefault(w => w.ID == order.UserOrderID);
        //    if (order.OrderID == 0)
        //    {
        //        var Count = orders.Count;
        //        if (Count > 0)
        //        {
        //            var MinOrderId = orders.Max(min => min.OrderID);
        //            _timeDelivery.PushOrderByTable(orders, order.TableID, user.Employee.Name);
        //            return order;
        //        }
        //        else
        //        {
        //            _timeDelivery.PushOrderByTable(orders, order.TableID, user.Employee.Name);
        //            return order;
        //        }

        //    }
        //    else
        //    {
        //        _timeDelivery.PushOrderByTable(orders, order.TableID, user.Employee.Name);
        //        return order;
        //    }

        //}
        #endregion


        public async Task<List<PrintBill>> PrintReceiptReprintAsync(int receiptId, string print_type, int userId, bool isWebClient = true)
        {
            List<PrintBill> printBills = new();
            if (receiptId > 0)
            {
                var receiptDetails = await _context.ReceiptDetail.Where(w => w.ReceiptID == receiptId).ToListAsync();
                var receipts = _context.Receipt.Where(r => r.ReceiptID == receiptId);
                var receipt = receipts.FirstOrDefault();
                var CUR = await _context.Currency.FirstOrDefaultAsync(s => s.ID == receipt.SysCurrencyID) ?? new Currency();
               
                var currencies = GetDisplayPayCurrency(receipt.PriceListID);
                var baseCurrency = currencies.FirstOrDefault(i => i.AltCurrencyID == i.BaseCurrencyID) ?? new DisplayPayCurrencyModel();
                var altCurrency = currencies.FirstOrDefault(i => i.IsActive) ?? new DisplayPayCurrencyModel();
                var receiptMemo = (from r in receipts
                                   join rm in _context.ReceiptMemo on r.ReceiptID equals rm.BasedOn
                                   group new { rm, r } by new { rm } into g
                                   let data = g.FirstOrDefault()
                                   select new
                                   {
                                       data.rm.ID,
                                       GrandToralSys = data.rm.GrandTotalSys,
                                       AmountFreight = data.r.AmountFreight,
                                       data.rm.GrandTotal,
                                       data.rm.DisValue,
                                       data.rm.TaxValue,
                                       data.rm.LocalCurrencyID,
                                       data.rm.SysCurrencyID,
                                       data.rm.PLRate,
                                       UserID = data.r.UserOrderID,
                                       data.rm.SubTotal,
                                   }).ToList();
                var detail = await _context.ReceiptDetail.Where(w => w.ReceiptID == receiptId).ToListAsync();
                var detailMemo = (from r in receiptMemo
                                  join rd in _context.ReceiptDetailMemoKvms on r.ID equals rd.ReceiptMemoID
                                  select new ReceiptDetail
                                  {
                                      ItemID = rd.ItemID,
                                      KhmerName = rd.KhmerName,
                                      AmountFreight = r.AmountFreight,
                                      Qty = rd.Qty * (-1),
                                      UnitPrice = rd.UnitPrice,
                                      DiscountValue = rd.DisValue,
                                      Total = rd.Total * (-1),
                                      UomID = rd.UomID,
                                      Currency = rd.Currency,
                                      ItemType = rd.ItemType,
                                      Description = rd.Description,
                                      OrderID = rd.OrderID,
                                      DiscountRate = rd.DisRate,


                                  }).ToList();
                var allDetail = new List<ReceiptDetail>
                    (detail.Count + detailMemo.Count);
                allDetail.AddRange(detail);
                allDetail.AddRange(detailMemo);
                //Pirint bill or tender
                var table = await _context.Tables.FindAsync(receipt.TableID);
                var user = await _context.UserAccounts.Include(emp => emp.Employee)
                    .Include(u => u.Company)
                    .FirstOrDefaultAsync(w => w.ID == receipt.UserOrderID);
                var customer = await _context.BusinessPartners.FirstOrDefaultAsync(w => w.Delete == false && w.Type.ToLower() == "customer" && w.ID == receipt.CustomerID);
                var automobile = _context.AutoMobiles.Where(w => w.BusinessPartnerID == receipt.CustomerID);
                var setting = await _context.GeneralSettings.Where(w => w.UserID == userId).ToListAsync();
                var syscurrency = await _context.Currency.FirstOrDefaultAsync(w => w.ID == receipt.SysCurrencyID) ?? new Currency();
                var banch = await _context.Branches.FirstOrDefaultAsync(w => w.Delete == false && w.ID == receipt.BranchID) ?? new Branch();
                var company = await _context.Company.FirstOrDefaultAsync(w => w.Delete == false && w.ID == banch.CompanyID) ?? new Company();
                var receiptInfo = await _context.ReceiptInformation.FirstOrDefaultAsync(w => w.BranchID == receipt.BranchID) ?? new ReceiptInformation();
                var mulipay = await _context.MultiPaymentMeans.Where(s => s.ReceiptID == receipt.ReceiptID).ToListAsync();

                string paymentType = "";
                var payment = (from mpay in mulipay
                               join pay in _context.PaymentMeans on mpay.PaymentMeanID equals pay.ID
                               select new PaymentMeans
                               {
                                   ID = pay.ID,
                                   Type = pay.Type,

                               }).ToList();
                foreach (var multipay in payment)
                {
                    paymentType += multipay.Type + " / ";
                }

                var ReceiptNo = receipt.ReceiptNo;
                var Received = _utility.ToCurrency(receipt.Received, baseCurrency.DecimalPlaces);
                var Change = _utility.ToCurrency(receipt.Change, baseCurrency.DecimalPlaces);
                var ChangeSys = _utility.ToCurrency(receipt.Change_Display, altCurrency.DecimalPlaces);
                var granTotalDisplay = " ";
                var otherCurr = receipt.GrandTotalOtherCurrenciesDisplay.Split('|');
                var receiptsByCustomer = await _context.Receipt.Where(r => r.CustomerID == receipt.CustomerID).ToListAsync();
                int index = 0;
                foreach (var item in otherCurr)
                {
                    if (!string.IsNullOrWhiteSpace(item))
                    {
                        var currdis = item.Split(' ');
                        int currid = int.Parse(currdis[0]);
                        _ = decimal.TryParse(currdis[1], out decimal rate);
                        _ = int.TryParse(currdis[2], out int decimalPlace);
                        var currcy = _context.Currency.FirstOrDefault(w => w.ID == currid) ?? new Currency();
                        decimal total = ((decimal)receipt.GrandTotal - receipt.BalancePay) * rate;
                        granTotalDisplay += index == 0 ? currcy.Description + " " + _utility.ToCurrency(total, decimalPlace) : "\n" + currcy.Description + " " + _utility.ToCurrency(total, decimalPlace);
                        index++;
                    }
                }

                foreach (var item in allDetail)
                {
                    var uom = await _context.UnitofMeasures.FirstOrDefaultAsync(u => u.ID == item.UomID);
                    var itemmaster = await _context.ItemMasterDatas.FirstOrDefaultAsync(s => s.ID == item.ItemID) ?? new ItemMasterData();
                    //Discount
                    PrintBill bill = new()
                    {

                        OrderID = item.OrderID,
                        ReceiptNo = ReceiptNo,
                        AmountFrieght = receipts.Sum(x => ((double)x.AmountFreight)),
                        Logo = company.Logo == null || company.Logo == "" ? "" : company.Logo,
                        Logo2 = company.Logo2 == null || company.Logo2 == "" ? "" : company.Logo2,
                        ReceiptTitle1 = receiptInfo.Title == null ? " " : receiptInfo.Title,
                        ReceiptTitle2 = receiptInfo.Title2 == null ? " " : receiptInfo.Title2,
                        CompanyName = user.Company.Name,
                        BrandKh = banch.Name2,
                        BranchName = banch.Name,
                        Photo = itemmaster.Image,
                        Team2 = receiptInfo.TeamCondition2 == null ? " " : receiptInfo.TeamCondition2,
                        Address = receiptInfo.Address,
                        ReceiptAddress2 = receiptInfo.Address2 == null ? " " : receiptInfo.Address2,
                        ReceiptEmail = receiptInfo.Email == null ? " " : receiptInfo.Email,
                        RecVATTIN = receiptInfo.Email == null ? " " : receiptInfo.Email,
                        RecWebsite = receiptInfo.Website == null ? " " : receiptInfo.Website,
                        PowerBy = receiptInfo.PowerBy == null ? " " : receiptInfo.PowerBy,
                        Tel1 = receiptInfo.Tel1,
                        Tel2 = receiptInfo.Tel2,
                        Table = table.Name,
                        OrderNo = receipt.OrderNo,
                        Cashier = user.Employee.Name,
                        DateTimeIn = receipt.DateIn.ToString("dd-MM-yyyy") + " " + receipt.TimeIn,
                        DateTimeOut = receipt.DateOut.ToString("dd-MM-yyyy") + " " + receipt.TimeOut,
                        Item = item.KhmerName,
                        ItemEn = item.EnglishName,
                        Qty = item.Qty.ToString(),
                        Uom = uom.Name,
                        Price = _utility.ToCurrency(item.UnitPrice, baseCurrency.DecimalPlaces),
                        DisItem = item.DiscountRate.ToString() + "" + "%",
                        Amount = _utility.ToCurrency(item.Total, baseCurrency.DecimalPlaces),
                        SubTotal = _utility.ToCurrency(receipt.Sub_Total - receiptMemo.Sum(s => s.SubTotal), baseCurrency.DecimalPlaces),
                        DisRate = receipt.DiscountRate + "" + "%",
                        DisValue = _utility.ToCurrency(receipt.DiscountValue - receiptMemo.Sum(s => s.DisValue), baseCurrency.DecimalPlaces),
                        TypeDis = receipt.TypeDis,
                        GrandTotal = item.Currency + " " + _utility.ToCurrency(receipt.GrandTotal - receiptMemo.Sum(s => s.GrandTotal), baseCurrency.DecimalPlaces),
                        GrandTotalSys = _utility.ToCurrency(receipt.GrandTotal_Display, altCurrency.DecimalPlaces),
                        VatRate = _utility.ToCurrency(receipt.TaxRate, baseCurrency.DecimalPlaces) + "%",
                        VatValue = item.Currency + " " + _utility.ToCurrency(receipt.TaxValue, baseCurrency.DecimalPlaces),
                        Received = Received,
                        Change = _utility.ToCurrency(receipt.Change - (double)receipt.AmountFreight, baseCurrency.DecimalPlaces),
                        ChangeSys = ChangeSys,
                        DescKh = receiptInfo.KhmerDescription,
                        DescEn = receiptInfo.EnglishDescription,
                        ExchangeRate = _utility.ToCurrency(receipt.ExchangeRate, baseCurrency.DecimalPlaces),
                        Printer = "",
                        Print = print_type,
                        ItemDesc = item.Description,
                        CustomerInfo = customer.Code + '/' + customer.Name + '/' + customer.Phone + '/' + customer.Address + '/' + customer.Email,
                        Team = receiptInfo.TeamCondition,
                        ItemType = item.ItemType,
                        PaymentMean = payment,
                        PaymentMeans = paymentType,
                        LabelUSA = CUR.Description,
                        // add new
                        TotalQty = _utility.ToCurrency(allDetail.Sum(od => od.Qty), baseCurrency.DecimalPlaces),
                        VatNumber = setting.FirstOrDefault().VatNum,
                        BarCode = itemmaster == null ? "" : itemmaster.Barcode,
                        TaxRate = _utility.ToCurrency(item.TaxRate, baseCurrency.DecimalPlaces) + "%",
                        TaxValue = item.Currency + " " + _utility.ToCurrency(item.TaxValue, baseCurrency.DecimalPlaces),
                        TaxTotal = item.Currency + " " + _utility.ToCurrency(receipt.TaxValue, baseCurrency.DecimalPlaces),
                        Freights = _utility.ToCurrency(receipt.AmountFreight, baseCurrency.DecimalPlaces),
                        LinePosition = item.LinePosition.ToString(),
                        ChangeCurrenciesDisplay = receipt.ChangeCurrenciesDisplay,
                        GrandTotalCurrenciesDisplay = granTotalDisplay,
                        ReceiptCount = receiptsByCustomer.Count(),
                        Plate = automobile.FirstOrDefault().Plate

                    };
                    //Qty
                    if (item.ItemType?.ToLower() == "service")
                    {
                        var arr_time = item.Qty.ToString().Split('.');
                        bill.Qty = arr_time[0] + "h:" + arr_time[1].PadRight(2, '0') + "m";
                    }
                    printBills.Add(bill);
                }

                var ip = _accessor.ActionContext.HttpContext.Connection.RemoteIpAddress.ToString();
                if (isWebClient)
                {
                    _timeDelivery.PrintBill(printBills, setting, ip);
                }
            }
            return printBills;
        }

        public async Task<int> PrintReceiptBillAsync(Order ordered, string print_type, List<SerialNumber> serials, List<BatchNo> batches)
        {
            //Generate receipt id
            SeriesDetail seriesDetail = new();
            var receipt_generated = "";
            int receiptId = 0;
            if (ordered == null) { return 0; }
            if (print_type == "Pay")
            {
                var setting = _context.GeneralSettings.FirstOrDefault(w => w.UserID == ordered.UserOrderID) ?? new GeneralSetting();
                var series = _context.Series.FirstOrDefault(w => w.ID == setting.SeriesID) ?? new Series() ?? new Series();
                if (series.ID > 0)
                {
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
                    receipt_generated = seriesDetail.Number;
                }
            }

            //Update check bill order
            ordered.CheckBill = 'Y';
            ordered.ReceiptNo = receipt_generated;
            ordered.DateOut = DateTime.Today;
            ordered.TimeOut = DateTime.Now.ToShortTimeString();
            _context.Order.Update(ordered);
            _context.SaveChanges();
            if (ordered.OrderDetail.Count > 0)
            {
                //Pirint bill or tender
                var table = _context.Tables.Find(ordered.TableID);
                var user = _context.UserAccounts.Include(emp => emp.Employee)
                    .Include(u => u.Company)
                    .FirstOrDefault(w => w.ID == ordered.UserOrderID);
                var setting = _context.GeneralSettings.Where(w => w.UserID == ordered.UserOrderID).ToList();
                var customer = _context.BusinessPartners.FirstOrDefault(w => w.Delete == false && w.Type == "Customer" && w.ID == ordered.CustomerID) ?? new HumanResources.BusinessPartner();
                var automobile = _context.AutoMobiles.FirstOrDefault(w => w.BusinessPartnerID == ordered.CustomerID) ?? new HumanResources.AutoMobile();
                var syscurrency = _context.Currency.FirstOrDefault(w => w.ID == ordered.SysCurrencyID) ?? new Currency();
                var banch = _context.Branches.FirstOrDefault(w => w.Delete == false && w.ID == ordered.BranchID) ?? new Branch();
                var receipt = _context.ReceiptInformation.FirstOrDefault(w => w.BranchID == ordered.BranchID) ?? new ReceiptInformation();
                var payment = _context.PaymentMeans.FirstOrDefault(w => w.ID == ordered.PaymentMeansID) ?? new PaymentMeans();

                string paymentType = "";
                foreach (var multipay in ordered.MultiPaymentMeans.Where(s => s.Total > 0))
                {
                    var paymenttype = _context.PaymentMeans.FirstOrDefault(s => s.ID == multipay.PaymentMeanID);
                    paymentType += paymenttype.Type + ",";
                }

                var Web_payment = (from mpay in ordered.MultiPaymentMeans.Where(s => s.Total > 0)
                                   join pay in _context.PaymentMeans on mpay.PaymentMeanID equals pay.ID
                                   select new PaymentMeans
                                   {
                                       Type = pay.Type.ToString(),
                                   }).ToList();

                List<PrintBill> PrintBill = new();
                string ReceiptNo = "";
                var point = _loyaltyProgram.CountPointOrder(ordered).Result;
                foreach (var item in ordered.OrderDetail)
                {
                    var Itemmaster = _context.ItemMasterDatas.FirstOrDefault(s => s.ID == item.ItemID);
                    var uom = _context.UnitofMeasures.First(u => u.ID == item.UomID);
                    if (item.Qty <= 0)
                    {
                        continue;
                    }
                    if (item.TypeDis == "Cash")
                    {
                        ordered.DiscountRate = ordered.DiscountValue;
                    }
                    if (setting[0].TaxOption == TaxOptions.Include)
                    {
                        ReceiptNo = ordered.ReceiptNo /*+ " " + setting[0].VatNum*/;
                    }
                    else
                    {
                        ReceiptNo = ordered.ReceiptNo;
                    }
                    var baseCurrency = _utility.GetBaseCurrency(ordered.PriceListID, ordered.PLCurrencyID);// ordered.DisplayPayOtherCurrency?.FirstOrDefault(i => i.AltCurrencyID == i.BaseCurrencyID) ?? new DisplayPayCurrencyModel();
                    var altCurrency = _utility.GetAltCurrency(ordered.PriceListID);//ordered.DisplayPayOtherCurrency?.FirstOrDefault(i => i.IsActive) ?? new DisplayPayCurrencyModel();
                    var receiptsByCustomer = await _context.Receipt.Where(r => r.CustomerID == ordered.CustomerID).ToListAsync();
                    var company = await _context.Company.FirstOrDefaultAsync(w => w.Delete == false && w.ID == banch.CompanyID) ?? new Company();
                    PrintBill bill = new()
                    {
                        Point = _utility.ToCurrency(point.Point, 2),
                        OutStandingPoint = _utility.ToCurrency(point.OutStandingPoint, 2),
                        OrderID = item.OrderID,
                        ReceiptNo = ReceiptNo,
                        // AmountFrieght = receiptReprint.Sum(x => ((double)x.AmountFreight)),
                        Logo = company.Logo == null || company.Logo == "" ? "" : company.Logo,
                        Logo2 = company.Logo2 == null || company.Logo2 == "" ? "" : company.Logo2,
                        ReceiptTitle1 = receipt.Title == null ? " " : receipt.Title,
                        ReceiptTitle2 = receipt.Title2 == null ? " " : receipt.Title2,
                        CompanyName = user.Company.Name,
                        BrandKh = banch.Name2,
                        BranchName = banch.Name,
                        // Photo = itemmaster.Image,
                        Team2 = receipt.TeamCondition2 == null ? " " : receipt.TeamCondition2,
                        Address = receipt.Address,
                        ReceiptAddress2 = receipt.Address2 == null ? " " : receipt.Address2,
                        ReceiptEmail = receipt.Email == null ? " " : receipt.Email,
                        RecVATTIN = receipt.Email == null ? " " : receipt.Email,
                        RecWebsite = receipt.Website == null ? " " : receipt.Website,
                        PowerBy = receipt.PowerBy == null ? " " : receipt.PowerBy,
                        Table = table.Name,
                        OrderNo = ordered.OrderNo,
                        Cashier = user.Employee.Name,
                        DateTimeIn = ordered.DateIn.ToString("dd-MM-yyyy") + " " + ordered.TimeIn,
                        DateTimeOut = ordered.DateOut.ToString("dd-MM-yyyy") + " " + ordered.TimeOut,
                        Item = item.KhmerName,
                        Qty = item.Qty.ToString(),
                        Uom = uom.Name,
                        Price = _utility.ToCurrency(item.UnitPrice, baseCurrency.DecimalPlaces),
                        DisItem = _utility.ToCurrency(item.DiscountRate, 2) + "%",
                        Amount = _utility.ToCurrency(item.Total, baseCurrency.DecimalPlaces),
                        SubTotal = _utility.ToCurrency(ordered.Sub_Total, baseCurrency.DecimalPlaces),
                        DisRate = _utility.ToCurrency(ordered.DiscountRate, 2) + "%",
                        DisValue = _utility.ToCurrency(ordered.DiscountValue, baseCurrency.DecimalPlaces),
                        TypeDis = ordered.TypeDis,
                        GrandTotal = item.Currency + " " + _utility.ToCurrency(ordered.GrandTotal, baseCurrency.DecimalPlaces),
                        GrandTotalSys = ordered.CurrencyDisplay + " " + _utility.ToCurrency(ordered.GrandTotal_Display, altCurrency.DecimalPlaces),
                        VatRate = _utility.ToCurrency(ordered.TaxRate, baseCurrency.DecimalPlaces) + "%",
                        VatValue = item.Currency + " " + _utility.ToCurrency(ordered.TaxValue, baseCurrency.DecimalPlaces),
                        Received = item.Currency + " " + _utility.ToCurrency(ordered.Received, baseCurrency.DecimalPlaces),
                        Change = item.Currency + " " + _utility.ToCurrency(ordered.Change, baseCurrency.DecimalPlaces),
                        ChangeSys = ordered.CurrencyDisplay + " " + _utility.ToCurrency(ordered.Change_Display, altCurrency.DecimalPlaces),
                        DescKh = receipt.KhmerDescription,
                        DescEn = receipt.EnglishDescription,
                        ExchangeRate = _utility.ToCurrency(ordered.ExchangeRate, baseCurrency.DecimalPlaces),
                        Printer = "",
                        Print = print_type,
                        ItemDesc = item.Description,
                        CustomerInfo = customer.Code + '/' + customer.Name + '/' + customer.Phone + '/' + customer.Address + '/' + customer.Email,
                        Team = receipt.TeamCondition,
                        ItemType = item.ItemType,
                        PaymentMean = Web_payment,
                        PaymentMeans = paymentType,
                        ItemEn = item.EnglishName,
                        Remark = ordered.Remark,
                        //// add new
                        TotalQty = _utility.ToCurrency(ordered.OrderDetail.Sum(od => od.Qty), baseCurrency.DecimalPlaces),
                        VatNumber = setting.FirstOrDefault().VatNum,
                        BarCode = Itemmaster.Barcode,
                        TaxRate = _utility.ToCurrency(item.TaxRate, baseCurrency.DecimalPlaces) + "%",
                        TaxValue = item.Currency + " " + _utility.ToCurrency(item.TaxValue, baseCurrency.DecimalPlaces),
                        TaxTotal = item.Currency + " " + _utility.ToCurrency(ordered.TaxValue, baseCurrency.DecimalPlaces),
                        Freights = _utility.ToCurrency(ordered.FreightAmount, baseCurrency.DecimalPlaces),
                        LinePosition = item.LinePosition.ToString(),
                        ChangeCurrenciesDisplay = ordered.ChangeCurrenciesDisplay,
                        GrandTotalCurrenciesDisplay = ordered.GrandTotalCurrenciesDisplay,
                        Code = item.Code,
                        ReceiptCount = receiptsByCustomer.Count + 1,
                        Plate = automobile.Plate
                    };
                    //Qty
                    if (item.ItemType == "Service")
                    {
                        var arr_time = item.Qty.ToString().Split('.');
                        bill.Qty = arr_time[0] + "h:" + arr_time[1].PadRight(2, '0') + "m";
                    }
                    PrintBill.Add(bill);
                }
                var ip = _accessor.ActionContext.HttpContext.Connection.RemoteIpAddress.ToString();

                _timeDelivery.PrintBill(PrintBill, setting, ip);
                //Update status table
                if (string.Compare(print_type, "Bill", true) == 0)
                {
                    bool isStillHasOrder = _context.Order.Any(i => i.TableID == ordered.TableID && i.CheckBill == 'Y');
                    if (isStillHasOrder)
                    {
                        var table_update = _context.Tables.Find(ordered.TableID);
                        table_update.Status = 'P';
                        table_update.Time = _timeDelivery.StopTimeTable(ordered.TableID, 'P');
                        _timeDelivery.ResetTimeTable(ordered.TableID, 'P', table_update.Time);
                        _context.Tables.Update(table_update);
                        _context.SaveChanges();
                    }
                }

                if (string.Compare(print_type, "Pay", true) == 0)
                {
                    receiptId = await InsertReceiptAsync(ordered.OrderID, serials, batches, ordered.MultiPaymentMeans, point);
                    GetOrder(ordered.TableID, ordered.OrderID, ordered.UserOrderID);
                    var count_order = _context.Order.Where(w => w.TableID == ordered.TableID && !w.Cancel).ToList();
                    if (count_order.Count <= 0)
                    {
                        var table_update = _context.Tables.Find(ordered.TableID);
                        table_update.Status = 'A';
                        table_update.Time = "00:00:00";
                        _timeDelivery.StopTimeTable(ordered.TableID, 'A');
                        _timeDelivery.ResetTimeTable(ordered.TableID, 'A', "00:00:00");
                        _context.Tables.Update(table_update);
                        _context.SaveChanges();
                    }
                }
                _timeDelivery.ClearUserOrder(ordered.TableID);
            }
            return receiptId;
        }
        public int MoveReceipt(int fromTableId, int toTableId, int orderId)
        {
            var currentOrder = _context.Order.Find(orderId);
            if (currentOrder != null)
            {
                currentOrder.TableID = toTableId;
                _context.SaveChanges();
                var oldtable = _context.Tables.Find(fromTableId);
                var newtable = _context.Tables.Find(toTableId);
                if (oldtable != null && newtable != null)
                {
                    if (!(newtable.Status == 'P' && currentOrder.CheckBill == 'Y'))
                    {
                        newtable.Status = 'B';
                    }

                    _timeDelivery.StartTimeTable(toTableId, oldtable.Time, newtable.Status);
                    if (!_context.Order.Any(o => !o.Delete && o.TableID == oldtable.ID))
                    {
                        oldtable.Status = 'A';
                        oldtable.Time = "00:00:00";
                        _timeDelivery.StopTimeTable(fromTableId, 'A');
                    }
                    _context.SaveChanges();
                }
            }
            return currentOrder == null ? 0 : currentOrder.OrderID;
        }

        public int MoveTable(int fromTableId, int toTableId)
        {
            _timeDelivery.StopTimeTable(fromTableId, 'A');
            _timeDelivery.ResetTimeTable(fromTableId, 'A', "00:00:00");
            _timeDelivery.GetTableAvailable("Move");
            var oldtable = _context.Tables.Find(fromTableId);
            var newtable = _context.Tables.Find(toTableId);
            var orders = _context.Order.Where(w => w.TableID == fromTableId).ToList();
            foreach (var order in orders)
            {
                order.TableID = toTableId;
            }

            _timeDelivery.StartTimeTable(toTableId, oldtable.Time, oldtable.Status);
            newtable.Status = oldtable.Status;
            newtable.Time = oldtable.Time;
            oldtable.Status = 'A';
            oldtable.Time = "00:00:00";
            _context.SaveChanges();

            return newtable == null ? 0 : newtable.ID;
        }

        public void CombineOrders(CombineOrder combineReceipt)
        {
            List<OrderDetail> CombineItem = new();
            foreach (var receipt in combineReceipt.Orders.ToList())
            {
                var order_detail = _context.OrderDetail.Where(w => w.OrderID != combineReceipt.OrderID && w.OrderID == receipt.OrderID).ToList();
                foreach (var item in order_detail)
                {
                    var item_update = _context.OrderDetail.FirstOrDefault(w => w.LineID == item.LineID && w.OrderID == combineReceipt.OrderID);
                    var item_count = _context.OrderDetail.Where(w => w.LineID == item.LineID && w.OrderID == combineReceipt.OrderID).ToList();
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
                                item_update.Total = item_update.Qty * item_update.UnitPrice * (1 - item_update.DiscountRate / 100);
                                item_update.DiscountValue = item_update.Qty * item_update.UnitPrice / 100;
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
                int orderInTable = _context.Order.Where(i => i.TableID == order_update.TableID && !i.Delete).ToList().Count;
                //Remove order
                var orders_delete = _context.Order.Where(w => w.Delete == true).ToList();
                _context.Order.RemoveRange(orders_delete);
                _context.SaveChanges();
                if (combineReceipt.TableID != order_update.TableID && orderInTable <= 0) UpdateTableVoidItem(order_update.TableID);
            }
        }

        public async Task<ItemsReturnObj> SubmitOrderAsync(Order order, string printType,
           List<SerialNumber> serials = null, List<BatchNo> batches = null)
        {
            if (order.OrderID <= 0)
            {
                order.TimeIn = DateTime.Now.ToShortTimeString();
                order.DateIn = Convert.ToDateTime(DateTime.Today);
                order.DateOut = Convert.ToDateTime(DateTime.Today);
                order.CheckBill = 'N';
                AddQueueOrder(order.BranchID, order.OrderNo);
            }

            _context.Order.Update(order);
            await _context.SaveChangesAsync();

            await IssueCommittedOrderAsync(order.OrderID);
            await IssueCommittedMaterialAsync(order.OrderID);
            int receiptId = await PrintReceiptBillAsync(order, printType, serials, batches);
            await ChangeTimeTableStatusAsync(order, printType);
            SendToPrintOrder(order, "Send");
            return new ItemsReturnObj
            {
                ReceiptID = receiptId
            };
        }

        private async Task ChangeTimeTableStatusAsync(Order order, string printType = "send")
        {
            switch (printType.ToLower())
            {
                case "send":
                    await _clientSignal.AddTimeTableAsync(order.TableID);
                    break;
                case "bill":
                    await _clientSignal.UpdateTimeTableAsync(order.TableID, 'P');
                    break;
                case "pay":
                    bool ordersRemaining = _context.Order.Any(o => o.TableID == order.TableID);
                    if (!ordersRemaining)
                    {
                        await _clientSignal.UpdateTimeTableAsync(order.TableID, 'A', "00:00:00");
                    }
                    break;
            }
        }

        public void ClearUserOrder(int tableid)
        {
            _timeDelivery.ClearUserOrder(tableid);
        }

        IEnumerable<OpenShift> IPOS.OpenShiftData(int userid, double cash)
        {
            var user = _context.UserAccounts.FirstOrDefault(w => w.ID == userid);
            var company = _context.Company.Find(user.CompanyID) ?? new Company();
            var receipts = _context.Receipt.Where(w => w.UserOrderID == userid).ToList();
            var openshifts = _context.OpenShift.Where(w => w.UserID == userid && w.Open == true).ToList();
            var exchangeRate = _context.ExchangeRates.Find(company.LocalCurrencyID);
            try
            {
                if (openshifts.Count > 0)
                {
                    var open = _context.OpenShift.Where(w => w.UserID == userid && w.Open == true).ToList();
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
                        LocalCurrencyID = company.LocalCurrencyID,
                        SysCurrencyID = company.SystemCurrencyID,
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
                    LocalCurrencyID = company.LocalCurrencyID,
                    SysCurrencyID = company.SystemCurrencyID,
                    LocalSetRate = exchangeRate.SetRate
                };
                _context.OpenShift.Add(openShift);
                _context.SaveChanges();
                return openshifts;
            }
        }

        //Queue Option
        public string CreateQueueNumber(GeneralSetting setting)
        {
            ResetQueue(setting);
            var queues = _context.Order_Queue.Where(q => q.BranchID == setting.BranchID).ToList();
            string queueNo = (queues.Count + 1).ToString();
            return queueNo;
        }
        
        public void ResetQueue(GeneralSetting setting)
        {
            var queues = _context.Order_Queue.Where(w => w.BranchID == setting.BranchID).ToList();
            if (setting.AutoQueue)
            {
                switch (setting.QueueOption)
                {
                    case QueueOptions.Day:
                        var _queues = queues.Where(q => q.DateTime.CompareTo(DateTime.Today) < 0).ToList();
                        _context.Order_Queue.RemoveRange(_queues);
                        break;
                    case QueueOptions.Counter:
                        if (setting.QueueCount <= queues.Count)
                        {
                            _context.Order_Queue.RemoveRange(queues);
                        }
                        break;
                }
                _context.SaveChanges();
            }
        }
        public async Task<List<CashoutReportMaster>> CloseShiftData(int userid, double cashout, bool isWebClient = true)
        {
            var user = await _context.UserAccounts.FirstOrDefaultAsync(w => w.ID == userid);
            var setting = await _context.GeneralSettings.FirstOrDefaultAsync(w => w.UserID == userid) ?? new GeneralSetting();
            var company = await _context.Company.FindAsync(user.CompanyID) ?? new Company();
            var exchangeRate = await _context.ExchangeRates.FindAsync(company.LocalCurrencyID);
            var receipts = await _context.Receipt.Where(w => w.UserOrderID == user.ID).ToListAsync();
            var trans_to = 0;
            // add new 

            var openshift = _context.OpenShift.FirstOrDefault(w => w.UserID == userid && w.Open == true);
            if (receipts.Count > 0)
            {
                trans_to = receipts.Max(w => w.ReceiptID);
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
                var _receipts = _context.Receipt
                    .Where(w => w.BranchID == user.BranchID && w.UserOrderID == userid && w.ReceiptID > openshift.Trans_From && w.ReceiptID <= trans_to)
                    .ToList();
                var amount = _receipts.Sum(w => w.GrandTotal_Sys);
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
                    LocalCurrencyID = company.LocalCurrencyID,
                    SysCurrencyID = company.SystemCurrencyID,
                    LocalSetRate = exchangeRate.SetRate,
                    Close = true
                };

                _context.CloseShift.Add(closeShift);
                _context.SaveChanges();
                await AlertCashOutAsync(closeShift);
                //Queue Option
                if (setting.QueueOption == QueueOptions.Sheet && closeShift.Close)
                {
                    var queues = _context.Order_Queue.Where(w => w.BranchID == setting.BranchID).ToList();
                    _context.Order_Queue.RemoveRange(queues);
                    _context.SaveChanges();
                }

                return PrintCloseShift(user.ID, closeShift, isWebClient);
            }
        }

        //AlertPayment
        private async Task AlertPaymentAsync(Receipt alpayment, string seriesName, string seriesDetail, int count)
        {
            var alertMs = await _check.AlertManagementAsync(TypeOfAlert.Payment);
            if (alertMs != null)
            {
                var alertDetails = await _check.AlertDetailsAsync(alertMs.ID);
                foreach (var ad in alertDetails)
                {
                    await SendPaymentToTeleBotAsync(alpayment, ad, seriesName, seriesDetail, count);
                }
            }
        }


        private async Task SendPaymentToTeleBotAsync(Receipt alpayment, AlertDetail ad, string seriesName, string seriesDetail, int count)
        {
            var branch = _context.Branches.FirstOrDefault(w => w.ID == alpayment.BranchID);
            var user = _context.UserAccounts.Include(u => u.Employee).FirstOrDefault(w => w.ID == alpayment.UserOrderID);
            var token = await GetTelegramTokenAsync();
            var baseCurrency = alpayment.DisplayPayOtherCurrency?.FirstOrDefault(i => i.AltCurrencyID == alpayment.SysCurrencyID) ?? new DisplayPayCurrencyModel();
            var plCurrency = alpayment.DisplayPayOtherCurrency?.FirstOrDefault(i => i.AltCurrencyID == alpayment.PLCurrencyID) ?? new DisplayPayCurrencyModel();
            var paymentMeant = _context.PaymentMeans.Find(alpayment.PaymentMeansID) ?? new PaymentMeans();
            //var grandTotal = double.Parse(alpayment.SaleAmount_Sys.ToString()) + double.Parse(alpayment.CashInAmount_Sys.ToString());
            //List<PaymentAlertView> PaymenttAlertVs = new();
            foreach (var i in ad.UserAlerts)
            {
                if (i.TelegramUserID != null && token != null)
                {
                    var msg = new TelegramSendMessageRequest
                    {
                        AccessToken = token.AccessToken,
                        ChatID = i.TelegramUserID ?? "",
                        Text = $"    <b>Payment</b>    " +
                        $"\n\nPayment Means : <b>{paymentMeant.Type}</b>" +
                        $"\nBranch : <b>{branch.Name}</b>" +
                        $"\nCashier : <b>{user.Employee.Name}</b>" +
                        $"\nDate In : <b>{alpayment.DateIn:dd-MM-yyyy} {alpayment.TimeIn}</b>" +
                        $"\nDate Out : <b>{alpayment.DateOut:dd-MM-yyyy} {alpayment.TimeOut}</b>" +
                        $"\nInvoice No : <b>{seriesName}-{seriesDetail}</b>" +
                        $"\nTotal Items: <b>{count}</b>" +
                        $"\nTotal Freight: <b>{plCurrency.AltCurrency} {_utility.ToCurrency(alpayment.AmountFreight, plCurrency.DecimalPlaces)}</b>" +
                        $"\nSub_Total : <b>{plCurrency.AltCurrency} {_utility.ToCurrency(alpayment.Sub_Total, plCurrency.DecimalPlaces)}</b>" +
                        $"\nDiscount : <b>{plCurrency.AltCurrency} {_utility.ToCurrency(alpayment.DiscountValue, plCurrency.DecimalPlaces)}</b>" +
                        $"\nGrand_Total : <b>{plCurrency.AltCurrency} {_utility.ToCurrency(alpayment.GrandTotal, plCurrency.DecimalPlaces)}</b>" +
                        $"\nReceived : <b>{plCurrency.AltCurrency} {_utility.ToCurrency(alpayment.Received, plCurrency.DecimalPlaces)}</b>" +
                         $"\nChang : <b>{plCurrency.AltCurrency} {_utility.ToCurrency(alpayment.Change, plCurrency.DecimalPlaces)}</b>" +
                        $"\n<b>Grand_Total_Sys : {baseCurrency.AltCurrency} {_utility.ToCurrency(alpayment.GrandTotal_Sys, baseCurrency.DecimalPlaces)}</b>",
                        ParseMode = ParseMode.Html,
                    };
                    //PaymentAlertView PaymentAlertV = new()
                    //{
                    //    TimeOut = alpayment.TimeOut,
                    //    BrandID = branch.ID,
                    //    SubTotal = (decimal)alpayment.Sub_Total,
                    //    CurrencyID = sys_curr.PriceList.CurrencyID,
                    //    BrandName = branch.Name,
                    //    Currency = sys_curr.PriceList.Currency.Description,
                    //    DateIn = alpayment.DateIn,
                    //    DateOut = alpayment.DateOut,
                    //    EmpID = user.Employee.ID,
                    //    EmpName = user.Employee.Name,
                    //    GrandTotal = (decimal)alpayment.GrandTotal,

                    //    TimeIn = alpayment.TimeIn,
                    //    UserID = i.UserAccountID,
                    //    IsRead = false,
                    //    CreatedAt = DateTime.Now,
                    //};
                    //PaymenttAlertVs.Add(PaymentAlertV);
                    await _teleBot.SendMessageAsync(msg);
                    //string output = JsonConvert.SerializeObject(PaymentAlertV);
                    //CashOutAlert cashOut = JsonConvert.DeserializeObject<CashOutAlert>(output, new JsonSerializerSettings
                    //{
                    //    NullValueHandling = NullValueHandling.Ignore,
                    //});
                    //await _context.CashOutAlerts.AddAsync(cashOut);
                    //await _context.SaveChangesAsync();
                    //PaymenttAlertVs.ForEach(_i => _i.ID = cashOut.ID);
                    //var count = await _context.CashOutAlerts.AsNoTracking().Where(_i => !_i.IsRead && _i.UserID == i.UserAccountID).CountAsync();
                    //var _cashOutAlertVs = PaymenttAlertVs.Where(_i => _i.UserID == i.UserAccountID).ToList();
                    //await PaymentAlertRAsync(_cashOutAlertVs, i.UserAccountID.ToString(), count);
                }
            }
        }

        ///
        private async Task AlertCashOutAsync(CloseShift closeShift)
        {
            var alertMs = await _check.AlertManagementAsync(TypeOfAlert.CashOut);
            if (alertMs != null)
            {
                var alertDetails = await _check.AlertDetailsAsync(alertMs.ID);
                foreach (var ad in alertDetails)
                {
                    await SendToTelebotAsync(closeShift, ad);
                }
            }
        }

        private async Task SendToTelebotAsync(CloseShift closeShift, AlertDetail ad)
        {
            var branch = _context.Branches.FirstOrDefault(w => w.ID == closeShift.BranchID);
            var user = _context.UserAccounts.Include(u => u.Employee).FirstOrDefault(w => w.ID == closeShift.UserID);
            var sys_curr = _context.Company.Include(c => c.PriceList).ThenInclude(c => c.Currency).FirstOrDefault();
            var token = await GetTelegramTokenAsync();
            var grandTotal = double.Parse(closeShift.SaleAmount_Sys.ToString()) + double.Parse(closeShift.CashInAmount_Sys.ToString());
            List<CashOutAlertViewModel> cashOutAlertVs = new();
            foreach (var i in ad.UserAlerts)
            {
                if (i.TelegramUserID != null && token != null)
                {
                    var msg = new TelegramSendMessageRequest
                    {
                        AccessToken = token.AccessToken,
                        ChatID = i.TelegramUserID ?? "",
                        Text = $"    <b>Cash Out</b>    " +
                        $"\n\nBranch : <b>{branch.Name}</b>" +
                        $"\nCashier : <b>{user.Employee.Name}</b>" +
                        $"\nDate In : <b>{closeShift.DateIn:dd-MM-yyyy} {closeShift.TimeIn}</b>" +
                        $"\nDate Out : <b>{closeShift.DateOut:dd-MM-yyyy} {closeShift.TimeOut}</b>" +
                        $"\nTotal Cash In : <b>{sys_curr.PriceList.Currency.Description} {string.Format("{0:n3}", closeShift.CashInAmount_Sys)}</b>" +
                        $"\nTotal Sale : <b>{sys_curr.PriceList.Currency.Description} {string.Format("{0:n3}", closeShift.SaleAmount_Sys)}</b>" +
                        $"\n<b>Grand Total : {sys_curr.PriceList.Currency.Description} {string.Format("{0:n3}", grandTotal)}</b>",
                        ParseMode = ParseMode.Html,
                    };
                    CashOutAlertViewModel cashOutAlertV = new()
                    {
                        TimeOut = closeShift.TimeOut,
                        BrandID = branch.ID,
                        CashInAmountSys = (decimal)closeShift.CashInAmount_Sys,
                        CurrencyID = sys_curr.PriceList.CurrencyID,
                        BrandName = branch.Name,
                        Currency = sys_curr.PriceList.Currency.Description,
                        DateIn = closeShift.DateIn,
                        DateOut = closeShift.DateOut,
                        EmpID = user.Employee.ID,
                        EmpName = user.Employee.Name,
                        GrandTotal = (decimal)grandTotal,
                        SaleAmountSys = (decimal)closeShift.SaleAmount_Sys,
                        TimeIn = closeShift.TimeIn,
                        UserID = i.UserAccountID,
                        IsRead = false,
                        CreatedAt = DateTime.Now,
                    };
                    cashOutAlertVs.Add(cashOutAlertV);
                    await _teleBot.SendMessageAsync(msg);
                    string output = JsonConvert.SerializeObject(cashOutAlertV);
                    CashOutAlert cashOut = JsonConvert.DeserializeObject<CashOutAlert>(output, new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                    });
                    await _context.CashOutAlerts.AddAsync(cashOut);
                    await _context.SaveChangesAsync();
                    cashOutAlertVs.ForEach(_i => _i.ID = cashOut.ID);
                    var count = await _context.CashOutAlerts.AsNoTracking().Where(_i => !_i.IsRead && _i.UserID == i.UserAccountID).CountAsync();
                    var _cashOutAlertVs = cashOutAlertVs.Where(_i => _i.UserID == i.UserAccountID).ToList();
                    await CashOutAlertRAsync(_cashOutAlertVs, i.UserAccountID.ToString(), count);
                }
            }
        }

        private async Task<TelegramToken> GetTelegramTokenAsync()
        {
            var token = await _context.TelegramTokens.FirstOrDefaultAsync() ?? new TelegramToken();
            return token;
        }

        private async Task CashOutAlertRAsync(List<CashOutAlertViewModel> cashOuts, string userId, int count = 0)
        {
            var countNoti = await _check.CountNotiAsync(Convert.ToInt32(userId));
            await _hubContext.Clients.User(userId).SendAsync("AlertCashOut", JsonConvert.SerializeObject(cashOuts), countNoti, count);
        }

        //private void SendToMail(CloseShift closeShift)
        //{
        //    try
        //    {
        //        var User = _context.UserAccounts.Include(u => u.Employee).FirstOrDefault(w => w.ID == closeShift.UserID);
        //        MailMessage message = new ();
        //        SmtpClient sm = new ();
        //        message.From = new MailAddress("mathsokkrey@gmail.com");
        //        message.To.Add(new MailAddress("developer.sokkrey@gmail.com"));
        //        message.Subject = User.Employee.Name + " completed close shift";
        //        message.IsBodyHtml = true;
        //        message.Body = BindHtml(closeShift);
        //        message.Priority = MailPriority.Normal;
        //        sm.Host = "smtp.gmail.com";
        //        sm.Port = 587;// 25;
        //        sm.EnableSsl = true;
        //        sm.UseDefaultCredentials = false;
        //        sm.DeliveryMethod = SmtpDeliveryMethod.Network;
        //        sm.UseDefaultCredentials = false;
        //        sm.Credentials = new NetworkCredential("mathsokkrey@gmail.com", "");//from mail and pass
        //        sm.Send(message);
        //    }
        //    catch (Exception e)
        //    {
        //        throw new Exception(e.Message);
        //    }
        //}

        //private string BindHtml(CloseShift closeShift)
        //{
        //    var Branch = _context.Branches.FirstOrDefault(w => w.ID == closeShift.BranchID);
        //    var User = _context.UserAccounts.Include(u => u.Employee).FirstOrDefault(w => w.ID == closeShift.UserID);
        //    var sys_curr = _context.Company.Include(c => c.PriceList).ThenInclude(c => c.Currency).FirstOrDefault();
        //    StringBuilder html = new();
        //    html.Append("<table></table>"
        //                + "<tr>"
        //                    + "<td>" + "Branch : " + Branch.Name + "</td>"
        //                + "</tr>"
        //                + "<tr>"
        //                    + "<td>" + "Cashier : " + User.Employee.Name + "</td>"
        //                + "</tr>"
        //                + "<tr>"
        //                    + "<td>" + "Date In : " + closeShift.DateIn.ToString("dd-MM-yyyy") + " " + closeShift.TimeIn + "</td>"
        //                + "</tr>"
        //                 + "<tr>"
        //                    + "<td>" + "Date Out : " + closeShift.DateOut.ToString("dd-MM-yyyy") + " " + closeShift.TimeOut + "</td>"
        //                + "</tr>"
        //                 + "<tr>"
        //                    + "<td>" + "Total Cash In : " + sys_curr.PriceList.Currency.Description + " " + string.Format("{0:n2}", closeShift.CashInAmount_Sys) + "</td>"
        //                + "</tr>"
        //                + "<tr>"
        //                    + "<td>" + "Total Sale : " + sys_curr.PriceList.Currency.Description + " " + string.Format("{0:n2}", closeShift.SaleAmount_Sys) + "</td>"
        //                + "</tr>"
        //                 + "<tr>"
        //                    + "<td style='font-weight:600'>" + "Grand Total : " + sys_curr.PriceList.Currency.Description + " " + string.Format("{0:n2}", (double.Parse(closeShift.SaleAmount_Sys.ToString()) + double.Parse(closeShift.CashInAmount_Sys.ToString()))) + "</td>"
        //                + "</tr>"

        //        );
        //    return html.ToString();
        //}

        public IEnumerable<POS.Receipt> GetReceiptReprint(int branchid, string date_from, string date_to)
        {
            if (date_from == null && date_to == null)
            {
                var receipts = _context.Receipt.Include(cur => cur.Currency).Include(user => user.UserAccount.Employee).Include(table => table.Table)
                    .Where(w => w.BranchID == branchid && w.DateOut == Convert.ToDateTime(DateTime.Today));
                return receipts;
            }
            else
            {
                var receipts = _context.Receipt.Include(cur => cur.Currency).Include(user => user.UserAccount.Employee).Include(table => table.Table)
                    .Where(w => w.BranchID == branchid && w.DateOut >= Convert.ToDateTime(date_from) && w.DateOut <= Convert.ToDateTime(date_to));
                return receipts;
            }
        }

        public IEnumerable<POS.Receipt> GetReceiptCancel(int branchid, string date_from, string date_to)
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

        public IEnumerable<ReceiptSummary> GetReceiptReturn(int userId, int branchId, string date_from, string date_to)
        {
            _ = DateTime.TryParse(date_from, out DateTime _dateFrom);
            _ = DateTime.TryParse(date_to, out DateTime _dateTo);
            var _receipts = _context.Receipt.Include(rd => rd.RececiptDetail)
                .Where(w => w.BranchID == branchId && !w.Cancel && IsBetweenDate(_dateFrom, _dateTo, w.DateOut));
            var receipts = _receipts.Where(r => r.RececiptDetail.Any(rd => rd.OpenQty > 0))
                   .Select(r => new ReceiptSummary
                   {
                       ReceiptID = r.ReceiptID,
                       ReceiptNo = r.ReceiptNo,
                       DateOut = r.DateOut.ToString("MM-dd-yyyy"),
                       TimeOut = DateTime.Parse(r.TimeOut).ToString("hh:mm tt"),
                       ReturnItems = r.RececiptDetail.Where(rd => rd.OpenQty > 0)
                       .Select(rd => new ReturnItem
                       {
                           ID = rd.ID,
                           ReceiptID = rd.ReceiptID,
                           ItemID = rd.ItemID,
                           Code = rd.Code,
                           KhName = rd.KhmerName,
                           UoM = rd.UnitofMeansure.Name,
                           UomID = rd.UomID,
                           OpenQty = rd.OpenQty,
                           ReturnQty = 0,
                           UserID = userId,
                           Price=rd.UnitPrice,
                           Amount=0,
                           GrandAmount=0,
                           Status =false,
                       }).ToList()
                   });
            return receipts;
        }

        private static bool IsBetweenDate(DateTime dateFrom, DateTime dateTo, DateTime dateInput)
        {
            return dateFrom.CompareTo(dateInput) <= 0 && dateTo.CompareTo(dateInput) >= 0;
        }

        public IEnumerable<ServiceItemSales> FilterItemByGroup(int pricelist_id)
        {
            var item = _context.ServiceItemSales.FromSql("pos_FilterItemByGroup @PricelistID={0}",
                parameters: new[] {
                    pricelist_id.ToString()

                });
            return item.ToList();
        }

        public void VoidOrder(int orderId, string reason)
        {
            using var transact = _context.Database.BeginTransaction();
            var order = _context.Order.FirstOrDefault(w => w.OrderID == orderId);
            var orderDetails = _context.OrderDetail.Where(od => od.OrderID == orderId);
            order.Cancel = true;
            order.Delete = true;
            order.Reason = reason;
            var orders = _context.Order.Where(w => w.TableID == order.TableID && order.Cancel == false).ToList();
            var user = _context.UserAccounts.Include(emp => emp.Employee).FirstOrDefault(w => w.ID == order.UserOrderID);

            _context.Update(order);
            _context.SaveChanges();

            List<VoidOrderDetail> lsDetail = new();
            //Add to Detail Void Table
            foreach (var item in orderDetails)
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
                    TaxGroupID = item.TaxGroupID,
                    TaxRate = item.TaxRate,
                    TaxValue = item.TaxValue,
                    EnglishName = item.EnglishName,
                    ItemID = item.ItemID,
                    ItemPrintTo = item.ItemPrintTo,
                    ItemStatus = item.ItemStatus,
                    ItemType = item.ItemType,
                    KhmerName = item.KhmerName,
                    LineID = item.LineID,
                    OrderID = item.OrderID,
                    ParentLineID = item.ParentLineID,
                    PrintQty = item.PrintQty,
                    Qty = item.Qty,
                    Total = item.Total,
                    Total_Sys = item.Total_Sys,
                    TypeDis = item.TypeDis,
                    UnitofMeansure = item.UnitofMeansure,
                    UnitPrice = item.UnitPrice,
                    UomID = item.UomID,
                    IsKsms = item.IsKsms,
                    IsKsmsMaster = item.IsKsmsMaster,
                    IsScale = item.IsScale,
                    KSServiceSetupId = item.KSServiceSetupId,
                    VehicleId = item.VehicleId
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
                PLCurrencyID = order.PLCurrencyID,
                PLRate = order.PLRate,
                LocalSetRate = order.LocalSetRate,
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
                Reason = order.Reason, //update on 19-06-2021
                TaxOption = order.TaxOption,
                VoidOrderDetail = lsDetail
            };
            _context.VoidOrders.Add(voidOrder);

            //Update status table
            UpdateTableVoidItem(order.TableID);
            GoodReceiptCommittedOrder(orderId);
            //GetOrder(order);
            GetOrder(order.TableID, order.OrderID, order.UserOrderID);
            _context.OrderDetail.RemoveRange(orderDetails);
            _context.Order.Remove(order);
            _context.SaveChanges();
            transact.Commit();
        }
        public void UpdateTableVoidItem(int tableId)
        {
            var table_ordered = _context.Order.Where(w => w.TableID == tableId && w.Cancel == false).ToList();
            if (table_ordered.Any())
            {
                var isHasSend = table_ordered.Any(i => i.CheckBill == 'N');
                var isHasBill = table_ordered.Any(i => i.CheckBill == 'Y');
                if (isHasSend) UpdateTableTimeContiuely(tableId);
                else if (isHasBill)
                {
                    var table_up = _context.Tables.Find(tableId);
                    table_up.Status = 'P';
                    _context.Update(table_up);
                    _context.SaveChanges();
                    _timeDelivery.StartTimeTable(table_up.ID, table_up.Time, table_up.Status);
                }
            }
            else
            {
                var time = _timeDelivery.StopTimeTable(tableId, 'A');
                var table_up = _context.Tables.Find(tableId);
                table_up.Status = 'A';
                table_up.Time = "00:00:00";
                _context.Update(table_up);
                _context.SaveChanges();
            }
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

        public void CancelReceipt(Receipt receipt, List<SerialNumber> serials, List<BatchNo> batches,int? PaymentmeansID=0)
        {
            var MemoNo = "";
            SeriesDetail seriesDetail = new();
            var _docType = _context.DocumentTypes.FirstOrDefault(c => c.Code == "RP");
            var _seriesRP = _context.Series.FirstOrDefault(c => c.DocuTypeID == _docType.ID && c.Default) ?? new Series();
            if (_seriesRP.ID > 0)
            {
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
                MemoNo = seriesDetail.Number;
            }
            var receiptdetail = _context.ReceiptDetail.Where(w => w.ReceiptID == receipt.ReceiptID && w.OpenQty > 0).ToList();
            ReceiptMemo receiptNew = new();
            receiptNew.SeriesID = _seriesRP.ID;
            receiptNew.SeriesDID = seriesDetail.ID;
            receiptNew.DocTypeID = _docType.ID;
            receiptNew.OrderNo = receipt.OrderNo;
            receiptNew.TableID = receipt.TableID;
            receiptNew.ReceiptNo = MemoNo;
            receiptNew.QueueNo = receipt.OrderNo;
            receiptNew.DateIn = DateTime.Today;
            receiptNew.TimeIn = DateTime.Now.ToShortTimeString();
            receiptNew.DateOut = DateTime.Today;
            receiptNew.TimeOut = DateTime.Now.ToShortTimeString();
            receiptNew.WaiterID = receipt.WaiterID;
            receiptNew.UserOrderID = receipt.UserOrderID;
            receiptNew.UserDiscountID = receipt.UserDiscountID;
            receiptNew.CustomerID = receipt.CustomerID;
            receiptNew.PriceListID = receipt.PriceListID;
            receiptNew.LocalCurrencyID = receipt.LocalCurrencyID;
            receiptNew.SysCurrencyID = receipt.SysCurrencyID;
            receiptNew.ExchangeRate = receipt.ExchangeRate;
            receiptNew.WarehouseID = receipt.WarehouseID;
            receiptNew.BranchID = receipt.BranchID;
            receiptNew.CompanyID = receipt.CompanyID;
            receiptNew.SubTotal = 0;
            receiptNew.DisRate = receipt.DiscountRate;
            receiptNew.DisValue = 0;
            receiptNew.TypeDis = receipt.TypeDis;
            receiptNew.TaxOption = receipt.TaxOption;
            receiptNew.TaxRate = receipt.TaxRate;
            receiptNew.TaxValue = receipt.TaxValue;
            receiptNew.GrandTotal = receipt.GrandTotal;
            receiptNew.GrandTotalSys = receipt.GrandTotal_Sys;
            receiptNew.Tip = 0;
            receiptNew.Received = (double)receipt.AppliedAmount == receipt.GrandTotal ? receipt.GrandTotal : (double)receipt.AppliedAmount;
            receiptNew.AppliedAmount = (double)receipt.AppliedAmount;
            receiptNew.Change = 0;
            receiptNew.PaymentMeansID = (int)PaymentmeansID;
            receiptNew.CheckBill = 'N';
            receiptNew.Cancel = true;
            receiptNew.Delete = false;
            receiptNew.Return = true;
            receiptNew.CurrencyDisplay = receipt.CurrencyDisplay;
            receiptNew.DisplayRate = receipt.DisplayRate;
            receiptNew.PLCurrencyID = receipt.PLCurrencyID;
            receiptNew.PLRate = receipt.PLRate;
            receiptNew.LocalSetRate = receipt.LocalSetRate;
            receiptNew.BasedOn = receipt.ReceiptID;
            receiptNew.ReceiptMemoNo = receipt.ReceiptNo;
            receiptNew.Status = receipt.Status;
            receiptNew.VehicleID = receipt.VehicleID;
            receiptNew.RemarkDiscount = receipt.RemarkDiscount;
            receiptNew.BuyXAmountGetXDisID = receipt.BuyXAmountGetXDisID;
            receiptNew.BuyXAmGetXDisRate = receipt.BuyXAmGetXDisRate;
            receiptNew.BuyXAmGetXDisType = receipt.BuyXAmGetXDisType;
            receiptNew.BuyXAmGetXDisValue = receipt.BuyXAmGetXDisValue;
            receiptNew.GrandTotalCurrenciesDisplay = receipt.GrandTotalCurrenciesDisplay;
            receiptNew.OtherPaymentGrandTotal = receipt.OtherPaymentGrandTotal;
            receipt.Cancel = true;
            receiptNew.TaxGroupID = receipt.TaxGroupID;
            receiptNew.OtherPaymentGrandTotal = receipt.OtherPaymentGrandTotal;
            receiptNew.GrandTotalCurrenciesDisplay = receipt.GrandTotalCurrenciesDisplay;
            receiptNew.PaymentType = receipt.PaymentType;
            receiptNew.GrandTotalCurrencies = receipt.GrandTotalCurrencies;
            receiptNew.ChangeCurrenciesDisplay = receipt.ChangeCurrenciesDisplay;
            receiptNew.ChangeCurrencies = receipt.ChangeCurrencies;
            receiptNew.PromoCodeDiscRate = receipt.PromoCodeDiscRate;
            receiptNew.PromoCodeDiscValue = receipt.PromoCodeDiscValue;
            receiptNew.PromoCodeID = receipt.PromoCodeID;
            receiptNew.GrandTotalOtherCurrencies = receipt.GrandTotalOtherCurrencies;
            receiptNew.GrandTotalOtherCurrenciesDisplay = receipt.GrandTotalOtherCurrenciesDisplay;
            receiptNew.CardMemberDiscountRate = receipt.CardMemberDiscountRate;
            receiptNew.CardMemberDiscountValue = receipt.CardMemberDiscountValue;
            receiptNew.BalancePay = receipt.BalancePay;
            _context.ReceiptMemo.Update(receiptNew);
            _context.SaveChanges();
            foreach (var item in receiptdetail.ToList())
            {
                ReceiptDetailMemo detail = new();
                detail.ReceiptMemoID = receiptNew.ID;
                detail.OrderDetailID = item.OrderDetailID;
                detail.OrderID = item.OrderID;
                detail.LineID = item.LineID;
                detail.ItemID = item.ItemID;
                detail.Code = item.Code;
                detail.KhmerName = item.KhmerName;
                detail.EnglishName = item.EnglishName;
                detail.Qty = item.OpenQty;
                detail.UnitPrice = item.UnitPrice;
                detail.Cost = item.Cost;
                detail.DisRate = item.DiscountRate;
                detail.DisValue = item.DiscountValue;
                detail.TypeDis = item.TypeDis;
                detail.TaxGroupID = item.TaxGroupID;
                detail.TaxRate = item.TaxRate;
                detail.TaxValue = item.TaxValue;
                detail.Total = item.UnitPrice * item.OpenQty;
                detail.TotalSys = item.UnitPrice * item.OpenQty * receiptNew.PLRate;
                detail.UomID = item.UomID;
                detail.ItemStatus = item.ItemStatus;
                detail.Currency = item.Currency;
                detail.ItemType = item.ItemType;
                detail.KSServiceSetupId = item.KSServiceSetupId;
                detail.VehicleId = item.VehicleId;
                detail.IsKsms = item.IsKsms;
                detail.IsKsmsMaster = item.IsKsmsMaster;
                detail.IsScale = item.IsScale;
                detail.IsReadonly = item.IsReadonly;
                detail.ComboSaleType = item.ComboSaleType;
                detail.RemarkDiscountID = detail.RemarkDiscountID;
                double DTotal = detail.Total;
                if (detail.DisRate > 0)
                {
                    detail.DisValue = DTotal * detail.DisRate / 100;
                    detail.Total = DTotal - detail.DisValue;
                    detail.TotalSys = (DTotal - detail.DisValue) * receiptNew.PLRate;
                }
                if (receipt.TaxOption != TaxOptions.Include)
                {
                    detail.DisValue = DTotal * detail.DisRate / 100;
                    detail.Total = DTotal - detail.DisValue + (double)detail.TaxValue;
                    detail.TotalSys = (DTotal - detail.DisValue + (double)detail.TaxValue) * receiptNew.PLRate;
                }
                //insert to KSService
                if (detail.IsKsmsMaster)
                {
                    var ksSetUpExisted = _context.KSServices.FirstOrDefault(i => i.KSServiceSetupId == detail.KSServiceSetupId && i.CusId == receiptNew.CustomerID);
                    if (ksSetUpExisted != null)
                    {
                        ksSetUpExisted.MaxCount -= detail.Qty;
                        ksSetUpExisted.Qty -= detail.Qty;
                        _context.Update(ksSetUpExisted);
                        _context.SaveChanges();
                    }
                }
                _context.ReceiptDetailMemoKvms.Update(detail);
                _context.SaveChanges();
            }
            var memoDetail = _context.ReceiptDetailMemoKvms.Where(w => w.ReceiptMemoID == receiptNew.ID).ToList();
            double Total = memoDetail.Sum(s => s.Total);
            double vatValue = 0;
            receiptNew.SubTotal = Total;

            double alltotaldiscount = receiptNew.DisRate + (double)receiptNew.BuyXAmGetXDisRate + receiptNew.PromoCodeDiscRate + (double)receiptNew.CardMemberDiscountRate;
            receiptNew.DisValue = (Total - receiptNew.TaxValue) * alltotaldiscount / 100;
            if (receiptNew.TaxOption == TaxOptions.InvoiceVAT)
            {
                vatValue = (Total - receiptNew.DisValue) * receiptNew.TaxRate / 100;
            }
            receiptNew.GrandTotal = Total + vatValue - receiptNew.DisValue;
            receiptNew.GrandTotalSys = receiptNew.GrandTotal * receiptNew.PLRate;
            if (receipt.OpenOtherPaymentGrandTotal == 0)
            {
                receiptNew.OtherPaymentGrandTotal = receipt.OpenOtherPaymentGrandTotal;
                receipt.OpenOtherPaymentGrandTotal -= receiptNew.OtherPaymentGrandTotal;
                receiptNew.OtherPaymentGrandTotal = receipt.OpenOtherPaymentGrandTotal;
            }
            else if (receipt.OpenOtherPaymentGrandTotal <= receiptNew.OtherPaymentGrandTotal && receipt.OpenOtherPaymentGrandTotal > 0)
            {
                receiptNew.OtherPaymentGrandTotal = receipt.OpenOtherPaymentGrandTotal;
                receipt.OpenOtherPaymentGrandTotal = 0;
            }
            _context.Receipt.Update(receipt);
            _context.Update(receiptNew);
            _context.SaveChanges();
            OrderDetailReturnStock(receiptNew.ID, serials, batches,receipt.ReceiptID, "return");
        }
        // Method get macaddress
        public string GetMacAddress(string ipAddress)
        {
            string macAddress = string.Empty;
            System.Diagnostics.Process pProcess = new();
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
        public bool SendReturnItem(List<ReturnItem> returnItems, List<SerialNumber> serials, List<BatchNo> batches,int? PaymentmeansID)
        {
            bool isReturned = false;
            var MemoNo = "";
            SeriesDetail seriesDetail = new();
            var _docType = _context.DocumentTypes.FirstOrDefault(c => c.Code == "RP");
            var _seriesRP = _context.Series.FirstOrDefault(c => c.DocuTypeID == _docType.ID && c.Default) ?? new Series();
            if (_seriesRP.ID > 0)
            {
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
                MemoNo = seriesDetail.Number;
            }

            var receipt = _context.Receipt.FirstOrDefault(w => w.ReceiptID == returnItems[0].ReceiptID);
            ReceiptMemo receiptNew = new();
            receiptNew.ReceiptKvmsID = receipt.ReceiptID;
            receiptNew.SeriesID = _seriesRP.ID;
            receiptNew.SeriesDID = seriesDetail.ID;
            receiptNew.DocTypeID = _docType.ID;
            receiptNew.OrderNo = receipt.OrderNo;
            receiptNew.TableID = receipt.TableID;
            receiptNew.ReceiptNo = MemoNo;
            receiptNew.QueueNo = receipt.OrderNo;
            receiptNew.DateIn = DateTime.Today;
            receiptNew.TimeIn = DateTime.Now.ToShortTimeString();
            receiptNew.DateOut = DateTime.Today;
            receiptNew.TimeOut = DateTime.Now.ToShortTimeString();
            receiptNew.WaiterID = receipt.WaiterID;
            receiptNew.UserOrderID = returnItems.First().UserID;
            receiptNew.UserDiscountID = receipt.UserDiscountID;
            receiptNew.CustomerID = receipt.CustomerID;
            receiptNew.PriceListID = receipt.PriceListID;
            receiptNew.LocalCurrencyID = receipt.LocalCurrencyID;
            receiptNew.SysCurrencyID = receipt.SysCurrencyID;
            receiptNew.ExchangeRate = receipt.ExchangeRate;
            receiptNew.WarehouseID = receipt.WarehouseID;
            receiptNew.BranchID = receipt.BranchID;
            receiptNew.CompanyID = receipt.CompanyID;
            receiptNew.SubTotal = 0;
            receiptNew.DisRate = receipt.DiscountRate;
            receiptNew.DisValue = 0;
            receiptNew.TypeDis = receipt.TypeDis;
            receiptNew.TaxRate = receipt.TaxRate;
            receiptNew.TaxValue = receipt.TaxValue;
            receiptNew.TaxOption = receipt.TaxOption;
            receiptNew.GrandTotal = 0;
            receiptNew.GrandTotalSys = 0;
            receiptNew.Tip = 0;
            receiptNew.Received = 0;
            receiptNew.Change = 0;
            receiptNew.PaymentMeansID = (int)PaymentmeansID;
            receiptNew.CheckBill = 'N';
            receiptNew.Cancel = true;
            receiptNew.Delete = false;
            receiptNew.Return = false;
            receiptNew.CurrencyDisplay = receipt.CurrencyDisplay;
            receiptNew.DisplayRate = receipt.DisplayRate;
            receiptNew.PLCurrencyID = receipt.PLCurrencyID;
            receiptNew.PLRate = receipt.PLRate;
            receiptNew.LocalSetRate = receipt.LocalSetRate;
            receiptNew.BasedOn = receipt.ReceiptID;
            receiptNew.ReceiptMemoNo = receipt.ReceiptNo;
            receiptNew.Status = receipt.Status;
            receiptNew.VehicleID = receipt.VehicleID;
            receiptNew.RemarkDiscount = receipt.RemarkDiscount;
            receiptNew.BuyXAmountGetXDisID = receipt.BuyXAmountGetXDisID;
            receiptNew.BuyXAmGetXDisRate = receipt.BuyXAmGetXDisRate;
            receiptNew.BuyXAmGetXDisType = receipt.BuyXAmGetXDisType;
            receiptNew.BuyXAmGetXDisValue = receipt.BuyXAmGetXDisValue;
            receiptNew.TaxGroupID = receipt.TaxGroupID;
            receiptNew.OtherPaymentGrandTotal = receipt.OtherPaymentGrandTotal;
            receiptNew.GrandTotalCurrenciesDisplay = receipt.GrandTotalCurrenciesDisplay;
            receiptNew.PaymentType = receipt.PaymentType;
            receiptNew.GrandTotalCurrencies = receipt.GrandTotalCurrencies;
            receiptNew.ChangeCurrenciesDisplay = receipt.ChangeCurrenciesDisplay;
            receiptNew.ChangeCurrencies = receipt.ChangeCurrencies;
            receiptNew.GrandTotalOtherCurrencies = receipt.GrandTotalOtherCurrencies;
            receiptNew.GrandTotalOtherCurrenciesDisplay = receipt.GrandTotalOtherCurrenciesDisplay;
            receiptNew.PromoCodeDiscRate = receipt.PromoCodeDiscRate;
            receiptNew.PromoCodeDiscValue = receipt.PromoCodeDiscValue;
            receiptNew.PromoCodeID = receipt.PromoCodeID;
            receiptNew.CardMemberDiscountRate = receipt.CardMemberDiscountRate;
            receiptNew.CardMemberDiscountValue = receipt.CardMemberDiscountValue;
            receiptNew.BalanceReturn = receipt.BalanceReturn;
            receiptNew.BalancePay = receipt.BalancePay;
            //receiptNew.
            _context.ReceiptMemo.Update(receiptNew);
            _context.SaveChanges();
            double vatValue = 0;
            double alltotaldiscount = receiptNew.DisRate + (double)receiptNew.BuyXAmGetXDisRate + receiptNew.PromoCodeDiscRate + (double)receiptNew.CardMemberDiscountRate;
            foreach (var item in returnItems.ToList())
            {
                var receiptDetail = _context.ReceiptDetail.FirstOrDefault(w => w.ID == item.ID && w.ItemID == item.ItemID);
                receiptDetail.OpenQty -= item.ReturnQty;
                _context.Update(receiptDetail);
                _context.SaveChanges();
                ReceiptDetailMemo detail = new();
                detail.ReceiptMemoID = receiptNew.ID;
                detail.OrderID = receiptDetail.OrderID;
                detail.LineID = receiptDetail.LineID;
                detail.ItemID = receiptDetail.ItemID;
                detail.Code = receiptDetail.Code;
                detail.KhmerName = receiptDetail.KhmerName;
                detail.EnglishName = receiptDetail.EnglishName;
                detail.Qty = item.ReturnQty;
                detail.UnitPrice = receiptDetail.UnitPrice;
                detail.Cost = receiptDetail.Cost;
                detail.DisRate = receiptDetail.DiscountRate;
                detail.TypeDis = receiptDetail.TypeDis;
                detail.DisRate = receiptDetail.DiscountRate;
                detail.DisValue = receiptDetail.DiscountValue;
                detail.UomID = receiptDetail.UomID;
                detail.ItemStatus = receiptDetail.ItemStatus;
                detail.Currency = receiptDetail.Currency;
                detail.ItemType = receiptDetail.ItemType;
                detail.Total = detail.Qty * detail.UnitPrice;
                detail.TotalSys = detail.Total * receiptNew.ExchangeRate;
                detail.KSServiceSetupId = receiptDetail.KSServiceSetupId;
                detail.VehicleId = receiptDetail.VehicleId;
                detail.IsKsms = receiptDetail.IsKsms;
                detail.IsKsmsMaster = receiptDetail.IsKsmsMaster;
                detail.IsReadonly = receiptDetail.IsReadonly;
                detail.ComboSaleType = receiptDetail.ComboSaleType;
                detail.RemarkDiscountID = receiptDetail.RemarkDiscountID;
                detail.TaxRate = receiptDetail.TaxRate;
                detail.TaxGroupID = receiptDetail.TaxGroupID;

                //detail.
                double DTotal = detail.Total;
                if (receiptDetail.DiscountRate > 0)
                {
                    detail.DisValue = DTotal * receiptDetail.DiscountRate / 100;
                    detail.Total = DTotal - detail.DisValue;
                    detail.TotalSys = (DTotal - detail.DisValue) * receiptNew.PLRate;
                }
                double disValue = (alltotaldiscount + receiptDetail.DiscountRate) * detail.Total / 100;
                detail.TaxValue = detail.TaxRate * (decimal)(detail.Total - disValue) / 100;
                if (receipt.TaxOption != TaxOptions.Include)
                {
                    detail.DisValue = DTotal * receiptDetail.DiscountRate / 100;
                    detail.Total = DTotal - detail.DisValue + (double)detail.TaxValue;
                    detail.TotalSys = (DTotal - detail.DisValue + (double)detail.TaxValue) * receiptNew.PLRate;
                }
                vatValue += (double)detail.TaxValue;
                //insert to KSService
                if (detail.IsKsmsMaster)
                {
                    var ksSetUpExisted = _context.KSServices.FirstOrDefault(i => i.KSServiceSetupId == detail.KSServiceSetupId && i.CusId == receiptNew.CustomerID);
                    if (ksSetUpExisted != null)
                    {
                        ksSetUpExisted.MaxCount -= item.ReturnQty;
                        ksSetUpExisted.Qty -= item.ReturnQty;
                        _context.Update(ksSetUpExisted);
                        _context.SaveChanges();
                    }
                }
                _context.Add(detail);
                _context.SaveChanges();
            }
            //Update summary
            var memoDetail = _context.ReceiptDetailMemoKvms.Where(w => w.ReceiptMemoID == receiptNew.ID).ToList();
            double Total = memoDetail.Sum(s => s.Total);
            receiptNew.SubTotal = Total;
            receiptNew.DisValue = (Total - vatValue) * alltotaldiscount / 100;
            if (receiptNew.TaxOption == TaxOptions.InvoiceVAT)
            {
                vatValue = (Total - receiptNew.DisValue) * receiptNew.TaxRate / 100;
            }
            receiptNew.GrandTotal = (Total - receiptNew.DisValue) + vatValue;
            receiptNew.GrandTotalSys = receiptNew.GrandTotal * receiptNew.PLRate;
            receiptNew.TaxValue = vatValue;
            if ((decimal)receiptNew.GrandTotal <= receipt.OpenOtherPaymentGrandTotal)
            {
                receiptNew.OtherPaymentGrandTotal = (decimal)receiptNew.GrandTotal;
                receiptNew.GrandTotal = (double)receiptNew.OtherPaymentGrandTotal;
                receiptNew.GrandTotalSys = receiptNew.GrandTotal * receiptNew.PLRate;
                receipt.OpenOtherPaymentGrandTotal -= (decimal)receiptNew.GrandTotal;
            }
            else
            {
                receiptNew.OtherPaymentGrandTotal = receipt.OpenOtherPaymentGrandTotal;
                receipt.OpenOtherPaymentGrandTotal = 0;
            }

            _context.Receipt.Update(receipt);
            _context.Update(receiptNew);
            _context.SaveChanges();
            string type = "return";
            OrderDetailReturnStock(receiptNew.ID, serials, batches,receipt.ReceiptID,type);
            isReturned = true;
            return isReturned;
        }

        //IssuseStock BOM Material
        public void IssuseInStock(int receipt, OrderStatus orderStaus, List<SerialNumber> serials, List<BatchNo> batches, List<MultiPaymentMeans> multiPayments)
        {
            var Order = _context.Receipt.FirstOrDefault(w => w.ReceiptID == receipt);
            var OrderDetails = _context.ReceiptDetail.Where(w => w.ReceiptID == receipt && w.Qty > 0).ToList();
            var Com = _context.Company.FirstOrDefault(c => c.ID == Order.CompanyID);
            var Exr = _context.ExchangeRates.FirstOrDefault(e => e.CurrencyID == Com.LocalCurrencyID);
            var docType = _context.DocumentTypes.FirstOrDefault(w => w.Code == "SP");
            var glAccs = _context.GLAccounts.Where(i => i.IsActive);
            var series = _context.Series.Find(Order.SeriesID) ?? new Series();
            var warehouse = _context.Warehouses.Find(Order.WarehouseID) ?? new Warehouse();
            IncomingPaymentCustomer incomingCus = new();
            IncomingPayment incoming = new();
            IncomingPaymentDetail incomingDetail = new();
            JournalEntry journalEntry = new();
            SeriesDetail seriesDetail = new();
            string OffsetAcc = "";
            var douTypeID = _context.DocumentTypes.FirstOrDefault(w => w.Code == "JE");
            var defaultJE = _context.Series.FirstOrDefault(w => w.Default == true && w.DocuTypeID == douTypeID.ID && w.NextNo != w.LastNo) ?? new Series();
            if (defaultJE.ID > 0)
            {
                // update series
                string Sno = defaultJE.NextNo;
                long No = long.Parse(Sno);
                defaultJE.NextNo = Convert.ToString(No + 1);
                // update series details
                seriesDetail.SeriesID = journalEntry.SeriesID;
                seriesDetail.Number = Sno;
                _context.Update(defaultJE);
                _context.Update(seriesDetail);
                _context.SaveChanges();
                // Insert Journal Entry
                journalEntry.SeriesID = defaultJE.ID;
                journalEntry.Number = defaultJE.PreFix + "-" + Sno;
                journalEntry.DouTypeID = defaultJE.DocuTypeID;
                journalEntry.Creator = Order.UserOrderID;
                journalEntry.TransNo = Order.ReceiptNo;
                journalEntry.PostingDate = Order.DateOut;
                journalEntry.DocumentDate = Order.DateOut;
                journalEntry.DueDate = Order.DateOut;
                journalEntry.SSCID = Order.SysCurrencyID;
                journalEntry.LLCID = Order.LocalCurrencyID;
                journalEntry.CompanyID = Order.CompanyID;
                journalEntry.LocalSetRate = (decimal)Order.LocalSetRate;
                journalEntry.SeriesDID = seriesDetail.ID;
                journalEntry.Remarks = series.Name + "-" + Order.ReceiptNo;
                _context.Update(journalEntry);
            }
            _context.SaveChanges();
            //Debit account receivable  
            List<JournalEntryDetail> journalEntryDetail = new();
            List<AccountBalance> accountBalance = new();
            var accountReceive = _context.BusinessPartners.FirstOrDefault(w => w.ID == Order.CustomerID);
            var glAcc = _context.GLAccounts.FirstOrDefault(w => w.ID == accountReceive.GLAccID) ?? new GLAccount();
            OffsetAcc = accountReceive.Code + "-" + glAcc.Code;
            //decimal cardPay = Order.OtherPaymentGrandTotal * (decimal)Order.ExchangeRate;
            var cardPay = multiPayments.FirstOrDefault(x => x.Type == PaymentMeanType.CardMember);
            if (cardPay != null)
            {
                //Card Member
                var cardMember = _context.AccountMemberCards.FirstOrDefault(i => i.CashAccID > 0 && i.UnearnedRevenueID > 0) ?? new AccountMemberCard();
                var cashAcc = _context.GLAccounts.FirstOrDefault(w => w.ID == cardMember.UnearnedRevenueID);
                if (cashAcc.ID > 0)
                {
                    var cardjur = journalEntryDetail.FirstOrDefault(w => w.ItemID == cashAcc.ID) ?? new JournalEntryDetail();
                    if (cardjur.ItemID > 0)
                    {
                        var accBalance = accountBalance.FirstOrDefault(w => w.GLAID == cashAcc.ID);
                        cashAcc.Balance += cardPay.Amount*cardPay.SCRate;
                        //journalEntryDetail
                        cardjur.Debit += cardPay.Amount * cardPay.SCRate;
                        //accountBalance
                        accBalance.CumulativeBalance = cashAcc.Balance;
                        accBalance.Debit += cardPay.Amount * cardPay.SCRate;
                    }
                    else
                    {
                        cashAcc.Balance += cardPay.Amount * cardPay.SCRate;
                        journalEntryDetail.Add(new JournalEntryDetail
                        {
                            JEID = journalEntry.ID,
                            Type = Type.GLAcct,
                            ItemID = cashAcc.ID,
                            Debit = cardPay.Amount * cardPay.SCRate,
                    });
                        //
                        accountBalance.Add(new AccountBalance
                        {
                            JEID = journalEntry.ID,
                            PostingDate = Order.DateIn,
                            Origin = docType.ID,
                            OriginNo = Order.ReceiptNo,
                            OffsetAccount = cashAcc.Code,
                            Details = douTypeID.Name + " - " + cashAcc.Code,
                            CumulativeBalance = cashAcc.Balance,
                            Debit = cardPay.Amount * cardPay.SCRate,
                        LocalSetRate = (decimal)Order.LocalSetRate,
                            GLAID = cashAcc.ID,
                        });
                    }
                    _context.Update(cashAcc);
                }
                var customer = _context.BusinessPartners.Find(Order.CustomerID);
                if (customer != null)
                {
                    customer.Balance -= Order.OtherPaymentGrandTotal;
                    //CardMemberDepositTransactions
                    CardMemberDepositTransaction cmdt = new()
                    {
                        Amount = Order.OtherPaymentGrandTotal * -1,
                        CardMemberDepositID = 0,
                        CardMemberID = customer.CardMemberID,
                        CumulativeBalance = customer.Balance,
                        CusID = customer.ID,
                        ID = 0,
                        DocTypeID = docType.ID,
                        Number = Order.ReceiptNo,
                        PostingDate = Order.DateIn,
                        SeriesDID = Order.SeriesDID,
                        SeriesID = Order.SeriesID,
                        UserID = Order.UserOrderID,
                    };
                    _context.BusinessPartners.Update(customer);
                    _context.CardMemberDepositTransactions.Update(cmdt);
                    _context.SaveChanges();
                }

            }
            decimal grandTotalSys = (decimal)Order.GrandTotal_Sys;
            if (glAcc.ID > 0 && grandTotalSys > 0)
            {
                journalEntryDetail.Add(new JournalEntryDetail
                {
                    JEID = journalEntry.ID,
                    Type = Type.BPCode,
                    ItemID = accountReceive.GLAccID,
                    Debit = grandTotalSys,
                    BPAcctID = Order.CustomerID,
                });
                //Insert             
                glAcc.Balance += grandTotalSys;
                accountBalance.Add(new AccountBalance
                {
                    JEID = journalEntry.ID,
                    PostingDate = Order.DateOut,
                    Origin = docType.ID,
                    OriginNo = Order.ReceiptNo,
                    OffsetAccount = OffsetAcc,
                    Details = douTypeID.Name + "-" + glAcc.Code,
                    CumulativeBalance = glAcc.Balance,
                    Debit = grandTotalSys,
                    LocalSetRate = (decimal)Order.LocalSetRate,
                    GLAID = accountReceive.GLAccID,
                    BPAcctID = Order.CustomerID,
                    Creator = Order.UserOrderID
                });
                _context.AccountBalances.UpdateRange(accountBalance);
                _context.GLAccounts.Update(glAcc);
            }
            #region
            //if (cardPay > 0)
            //{
            //    if (Order.PaymentType == PaymentType.CardMember)
            //    {
            //        //Card Member
            //        var cardMember = _context.AccountMemberCards.FirstOrDefault(i => i.CashAccID > 0 && i.UnearnedRevenueID > 0) ?? new AccountMemberCard();
            //        var cashAcc = _context.GLAccounts.FirstOrDefault(w => w.ID == cardMember.UnearnedRevenueID);
            //        if (cashAcc.ID > 0)
            //        {
            //            var cardjur = journalEntryDetail.FirstOrDefault(w => w.ItemID == cashAcc.ID) ?? new JournalEntryDetail();
            //            if (cardjur.ItemID > 0)
            //            {
            //                var accBalance = accountBalance.FirstOrDefault(w => w.GLAID == cashAcc.ID);
            //                cashAcc.Balance += cardPay;
            //                //journalEntryDetail
            //                cardjur.Debit += cardPay;
            //                //accountBalance
            //                accBalance.CumulativeBalance = cashAcc.Balance;
            //                accBalance.Debit += cardPay;
            //            }
            //            else
            //            {
            //                cashAcc.Balance += cardPay;
            //                journalEntryDetail.Add(new JournalEntryDetail
            //                {
            //                    JEID = journalEntry.ID,
            //                    Type = Type.GLAcct,
            //                    ItemID = cashAcc.ID,
            //                    Debit = cardPay,
            //                });
            //                //
            //                accountBalance.Add(new AccountBalance
            //                {
            //                    PostingDate = Order.DateIn,
            //                    Origin = docType.ID,
            //                    OriginNo = Order.ReceiptNo,
            //                    OffsetAccount = cashAcc.Code,
            //                    Details = douTypeID.Name + " - " + cashAcc.Code,
            //                    CumulativeBalance = cashAcc.Balance,
            //                    Debit = cardPay,
            //                    LocalSetRate = (decimal)Order.LocalSetRate,
            //                    GLAID = cashAcc.ID,
            //                });
            //            }
            //            _context.Update(cashAcc);
            //        }
            //        var customer = _context.BusinessPartners.Find(Order.CustomerID);
            //        if (customer != null)
            //        {
            //            customer.Balance -= Order.OtherPaymentGrandTotal;
            //            //CardMemberDepositTransactions
            //            CardMemberDepositTransaction cmdt = new()
            //            {
            //                Amount = Order.OtherPaymentGrandTotal * -1,
            //                CardMemberDepositID = 0,
            //                CardMemberID = customer.CardMemberID,
            //                CumulativeBalance = customer.Balance,
            //                CusID = customer.ID,
            //                ID = 0,
            //                DocTypeID = docType.ID,
            //                Number = Order.ReceiptNo,
            //                PostingDate = Order.DateIn,
            //                SeriesDID = Order.SeriesDID,
            //                SeriesID = Order.SeriesID,
            //                UserID = Order.UserOrderID,
            //            };
            //            _context.BusinessPartners.Update(customer);
            //            _context.CardMemberDepositTransactions.Update(cmdt);
            //            _context.SaveChanges();
            //        }
            //    }
            //}
            #endregion
            // VAT Invoice
            var taxgInvoice = _context.TaxGroups.Find(Order.TaxGroupID) ?? new TaxGroup();
            var taxAccInVoice = _context.GLAccounts.Find(taxgInvoice.GLID) ?? new GLAccount();
            decimal taxValueInvoice = (decimal)(Order.TaxValue * Order.ExchangeRate);
            if (taxAccInVoice.ID > 0 && taxValueInvoice > 0)
            {
                var taxjur = journalEntryDetail.FirstOrDefault(w => w.ItemID == taxAccInVoice.ID) ?? new JournalEntryDetail();
                if (taxjur.ItemID > 0 )
                {
                    var accBalance = accountBalance.FirstOrDefault(w => w.GLAID == taxAccInVoice.ID);
                    taxAccInVoice.Balance -= taxValueInvoice;
                    //journalEntryDetail
                    taxjur.Credit += taxValueInvoice;
                    //accountBalance
                    accBalance.CumulativeBalance = taxAccInVoice.Balance;
                    accBalance.Credit += taxValueInvoice;
                }
                else
                {
                    taxAccInVoice.Balance -= taxValueInvoice;
                    journalEntryDetail.Add(new JournalEntryDetail
                    {
                        JEID = journalEntry.ID,
                        Type = Type.GLAcct,
                        ItemID = taxAccInVoice.ID,
                        Credit = taxValueInvoice,
                    });
                    //
                    accountBalance.Add(new AccountBalance
                    {
                        JEID = journalEntry.ID,
                        PostingDate = Order.DateIn,
                        Origin = docType.ID,
                        OriginNo = Order.ReceiptNo,
                        OffsetAccount = taxAccInVoice.Code,
                        Details = douTypeID.Name + " - " + taxAccInVoice.Code,
                        CumulativeBalance = taxAccInVoice.Balance,
                        Credit = taxValueInvoice,
                        LocalSetRate = (decimal)Order.LocalSetRate,
                        GLAID = taxAccInVoice.ID,
                    });
                }
                _context.Update(taxAccInVoice);
            }

            var refreight = _context.FreightReceipts.Where(w => w.ReceiptID == Order.ReceiptID).ToList();
            foreach (var item in refreight)
            {
                if (item.AmountReven > 0)
                {
                    var freignt = _context.Freights.FirstOrDefault(w => w.ID == item.FreightID) ?? new Freight();
                    var frReven = _context.GLAccounts.FirstOrDefault(w => w.ID == freignt.RevenAcctID) ?? new GLAccount();
                    if (freignt.ID > 0)
                    {
                        var list = journalEntryDetail.FirstOrDefault(w => w.ItemID == frReven.ID) ?? new JournalEntryDetail();
                        if (list.ItemID > 0)
                        {
                            var accBalance = accountBalance.FirstOrDefault(w => w.GLAID == freignt.RevenAcctID);
                            frReven.Balance -= item.AmountReven * (decimal)Order.ExchangeRate;
                            //journalEntryDetail
                            list.Credit += item.AmountReven * (decimal)Order.ExchangeRate;
                            //accountBalance
                            accBalance.CumulativeBalance = frReven.Balance;
                            accBalance.Credit += item.AmountReven * (decimal)Order.ExchangeRate;
                        }
                        else
                        {
                            frReven.Balance -= item.AmountReven * (decimal)Order.ExchangeRate;
                            journalEntryDetail.Add(new JournalEntryDetail
                            {
                                JEID = journalEntry.ID,
                                Type = Type.GLAcct,
                                ItemID = freignt.RevenAcctID,
                                Credit = item.AmountReven * (decimal)Order.ExchangeRate,
                            });
                            //
                            accountBalance.Add(new AccountBalance
                            {
                                JEID = journalEntry.ID,
                                PostingDate = Order.DateOut,
                                Origin = docType.ID,
                                OriginNo = Order.ReceiptNo,
                                OffsetAccount = OffsetAcc,
                                Details = douTypeID.Name + "-" + frReven.Code,
                                CumulativeBalance = frReven.Balance,
                                Credit = item.AmountReven * (decimal)Order.ExchangeRate,
                                LocalSetRate = (decimal)Order.LocalSetRate,
                                GLAID = freignt.RevenAcctID,
                            });
                        }
                        _context.Update(frReven);
                    }
                }
            }
            _context.SaveChanges();

            foreach (var item in OrderDetails)
            {
                int revenueAccID = 0, inventoryAccID = 0, COGSAccID = 0;
                decimal revenueAccAmount = 0, inventoryAccAmount = 0, COGSAccAmount = 0;
                var itemMaster = _context.ItemMasterDatas.FirstOrDefault(w => w.ID == item.ItemID && !w.Delete);
                decimal disvalue = (decimal)item.Total_Sys * ((decimal)(Order.DiscountRate + Order.PromoCodeDiscRate) + Order.BuyXAmGetXDisRate + Order.CardMemberDiscountRate) / 100;
                if (itemMaster.SetGlAccount == SetGlAccount.ItemLevel)
                {
                    var itemAccs = _context.ItemAccountings.Where(w => w.ItemID == item.ItemID && w.WarehouseID == Order.WarehouseID);
                    var revenueAcc = (from ia in itemAccs
                                      join gl in glAccs on ia.RevenueAccount equals gl.Code
                                      select gl
                                         ).FirstOrDefault() ?? new GLAccount();
                    var inventoryAcc = (from ia in itemAccs
                                        join gl in glAccs on ia.InventoryAccount equals gl.Code
                                        select gl
                                         ).FirstOrDefault() ?? new GLAccount();
                    var COGSAcc = (from ia in itemAccs
                                   join gl in glAccs on ia.CostofGoodsSoldAccount equals gl.Code
                                   select gl
                                         ).FirstOrDefault() ?? new GLAccount();
                    revenueAccID = revenueAcc.ID;
                    inventoryAccID = inventoryAcc.ID;
                    COGSAccID = COGSAcc.ID;

                    if (Order.TaxOption == TaxOptions.Exclude)
                    {
                        decimal total = (decimal)item.Total_Sys - item.TaxValue;
                        disvalue = total * ((decimal)(Order.DiscountRate + Order.PromoCodeDiscRate) + Order.BuyXAmGetXDisRate + Order.CardMemberDiscountRate) / 100;
                        revenueAccAmount = total - disvalue;
                    }
                    else
                    {
                        revenueAccAmount = (decimal)item.Total_Sys - disvalue - (item.TaxValue * (decimal)Order.ExchangeRate);
                    }
                }
                else if (itemMaster.SetGlAccount == SetGlAccount.ItemGroup)
                {
                    var itemAccs = _context.ItemAccountings.Where(w => w.ItemGroupID == itemMaster.ItemGroup1ID);
                    var revenueAcc = (from ia in itemAccs
                                      join gl in glAccs on ia.RevenueAccount equals gl.Code
                                      select gl
                                         ).FirstOrDefault() ?? new GLAccount();
                    var inventoryAcc = (from ia in itemAccs
                                        join gl in glAccs on ia.InventoryAccount equals gl.Code
                                        select gl
                                         ).FirstOrDefault() ?? new GLAccount();
                    var COGSAcc = (from ia in itemAccs
                                   join gl in glAccs on ia.CostofGoodsSoldAccount equals gl.Code
                                   select gl
                                         ).FirstOrDefault() ?? new GLAccount();
                    revenueAccID = revenueAcc.ID;
                    inventoryAccID = inventoryAcc.ID;
                    COGSAccID = COGSAcc.ID;
                    if (Order.TaxOption == TaxOptions.Exclude)
                    {
                        decimal total = (decimal)item.Total_Sys - item.TaxValue;
                        disvalue = total * ((decimal)(Order.DiscountRate + Order.PromoCodeDiscRate) + Order.BuyXAmGetXDisRate + Order.CardMemberDiscountRate) / 100;
                        revenueAccAmount = total - disvalue;
                    }
                    else
                    {
                        revenueAccAmount = (decimal)item.Total_Sys - disvalue - (item.TaxValue * (decimal)Order.ExchangeRate);
                    }
                }
                var orft = _context.GroupDUoMs.FirstOrDefault(w => w.GroupUoMID == itemMaster.GroupUomID && w.AltUOM == item.UomID);

                if (!item.IsKsms && !item.IsKsmsMaster)
                {
                    if (itemMaster.Process != "Standard")
                    {
                        double @Check_Stock;
                        double @Remain;
                        double @IssusQty;
                        double @FIFOQty;
                        double @Qty = item.Qty * orft.Factor;
                        double Cost = 0;
                        var _itemAcc = _context.ItemAccountings.FirstOrDefault(i => i.WarehouseID == Order.WarehouseID && i.ItemID == item.ItemID);
                        var item_warehouse_summary = _context.WarehouseSummary.FirstOrDefault(w => w.WarehouseID == Order.WarehouseID && w.ItemID == item.ItemID);
                        var wareDetails = _context.WarehouseDetails.Where(w => w.WarehouseID == Order.WarehouseID && w.ItemID == item.ItemID).ToList();
                        if (item_warehouse_summary != null)
                        {
                            if (orderStaus == OrderStatus.Order)
                            {
                                //changed 21-09-2021
                                item_warehouse_summary.Committed -= @Qty - item.PrintQty * orft.Factor;
                                itemMaster.StockCommit -= @Qty;
                                _context.ItemMasterDatas.Update(itemMaster);
                            }
                            //WerehouseSummary
                            item_warehouse_summary.InStock -= @Qty;
                            //Itemmasterdata
                            //item_master_data.StockIn = item_warehouse_summary.InStock - (double)item.Qty;
                            _context.WarehouseSummary.Update(item_warehouse_summary);
                            _utility.UpdateItemAccounting(_itemAcc, item_warehouse_summary);
                            _context.SaveChanges();
                        }
                        //Checking Serial Batch //
                        if (itemMaster.ManItemBy == ManageItemBy.SerialNumbers)
                        {
                            if (serials.Count > 0)
                            {
                                List<WareForAudiView> wareForAudis = new();

                                foreach (var s in serials)
                                {
                                    if (s.SerialNumberSelected != null)
                                    {
                                        foreach (var ss in s.SerialNumberSelected.SerialNumberSelectedDetails)
                                        {
                                            var waredetial = wareDetails.FirstOrDefault(i => ss.SerialNumber == i.SerialNumber && i.InStock > 0);
                                            decimal _inventoryAccAmount = 0M;
                                            decimal _COGSAccAmount = 0M;
                                            if (waredetial != null)
                                            {
                                                Cost = waredetial.Cost;
                                                wareForAudis.Add(new WareForAudiView
                                                {
                                                    Cost = waredetial.Cost,
                                                    Qty = waredetial.InStock,
                                                    ExpireDate = waredetial.ExpireDate,
                                                });
                                                waredetial.InStock -= 1;
                                                // insert to warehouse detail
                                                var stockOut = new StockOut
                                                {
                                                    AdmissionDate = waredetial.AdmissionDate,
                                                    Cost = (decimal)waredetial.Cost,
                                                    CurrencyID = waredetial.CurrencyID,
                                                    Details = waredetial.Details,
                                                    ID = 0,
                                                    InStock = 1,
                                                    ItemID = waredetial.ItemID,
                                                    Location = waredetial.Location,
                                                    LotNumber = waredetial.LotNumber,
                                                    MfrDate = waredetial.MfrDate,
                                                    MfrSerialNumber = waredetial.MfrSerialNumber,
                                                    MfrWarDateEnd = waredetial.MfrWarDateEnd,
                                                    MfrWarDateStart = waredetial.MfrWarDateStart,
                                                    ProcessItem = ProcessItem.SEBA,
                                                    SerialNumber = waredetial.SerialNumber,
                                                    SyetemDate = DateTime.Now,
                                                    SysNum = 0,
                                                    TimeIn = DateTime.Now,
                                                    WarehouseID = waredetial.WarehouseID,
                                                    UomID = item.UomID,
                                                    UserID = Order.UserOrderID,
                                                    ExpireDate = waredetial.ExpireDate,
                                                    TransType = TransTypeWD.POS,
                                                    TransID = Order.ReceiptID,
                                                    Contract = itemMaster.ContractID,
                                                    OutStockFrom = Order.ReceiptID,
                                                    FromWareDetialID = waredetial.ID,
                                                    BPID = Order.CustomerID
                                                };
                                                _inventoryAccAmount = (decimal)waredetial.Cost;
                                                _COGSAccAmount = (decimal)waredetial.Cost;
                                                _context.StockOuts.Add(stockOut);
                                                _context.SaveChanges();
                                            }
                                            InsertFinancialPOS(
                                                inventoryAccID, COGSAccID, _inventoryAccAmount, _COGSAccAmount, journalEntryDetail,
                                                accountBalance, journalEntry, docType, douTypeID, Order, OffsetAcc, item
                                            );
                                        }
                                    }
                                }
                                wareForAudis = (from wa in wareForAudis
                                                group wa by wa.Cost into g
                                                let wha = g.FirstOrDefault()
                                                select new WareForAudiView
                                                {
                                                    Qty = g.Sum(i => i.Qty),
                                                    Cost = wha.Cost,
                                                    ExpireDate = wha.ExpireDate,
                                                }).ToList();
                                if (wareForAudis.Any())
                                {
                                    foreach (var i in wareForAudis)
                                    {
                                        // Insert to Inventory Audit
                                        var inventory_audit = _context.InventoryAudits
                                            .Where(w => w.ItemID == item.ItemID && w.WarehouseID == Order.WarehouseID && w.Cost == i.Cost).ToList();
                                        //var item_IssusStock = wareDetails.FirstOrDefault(w => w.InStock > 0);
                                        var inventory = new InventoryAudit
                                        {
                                            ID = 0,
                                            WarehouseID = Order.WarehouseID,
                                            BranchID = Order.BranchID,
                                            UserID = Order.UserOrderID,
                                            ItemID = item.ItemID,
                                            CurrencyID = Order.SysCurrencyID,
                                            UomID = orft.BaseUOM,
                                            InvoiceNo = Order.ReceiptNo,
                                            Trans_Type = docType.Code,
                                            Process = itemMaster.Process,
                                            SystemDate = DateTime.Now,
                                            TimeIn = DateTime.Now.ToShortTimeString().ToString(),
                                            Qty = i.Qty * -1,
                                            Cost = i.Cost,
                                            Price = 0,
                                            CumulativeQty = inventory_audit.Sum(q => q.Qty) - i.Qty,
                                            CumulativeValue = inventory_audit.Sum(s => s.Trans_Valuse) - (i.Qty * i.Cost),
                                            Trans_Valuse = i.Qty * i.Cost * -1,
                                            ExpireDate = i.ExpireDate,
                                            LocalCurID = Order.LocalCurrencyID,
                                            LocalSetRate = Order.LocalSetRate,
                                            CompanyID = Order.CompanyID,
                                            DocumentTypeID = docType.ID,
                                            SeriesID = Order.SeriesID,
                                            SeriesDetailID = Order.SeriesDID,
                                        };
                                        _utility.CumulativeValue(inventory.WarehouseID, inventory.ItemID, inventory.CumulativeValue, _itemAcc);
                                        _context.InventoryAudits.Add(inventory);
                                        _context.SaveChanges();
                                    }
                                }
                            }
                        }
                        else if (itemMaster.ManItemBy == ManageItemBy.Batches)
                        {
                            if (batches.Count > 0)
                            {
                                List<WareForAudiView> wareForAudis = new();
                                foreach (var b in batches)
                                {
                                    if (b.BatchNoSelected != null)
                                    {
                                        foreach (var sb in b.BatchNoSelected.BatchNoSelectedDetails)
                                        {
                                            decimal selectedQty = sb.SelectedQty * (decimal)orft.Factor;
                                            var waredetial = wareDetails.FirstOrDefault(i => sb.BatchNo == i.BatchNo && i.InStock > 0);
                                            decimal _inventoryAccAmount = 0M;
                                            decimal _COGSAccAmount = 0M;
                                            if (waredetial != null)
                                            {
                                                wareForAudis.Add(new WareForAudiView
                                                {
                                                    Cost = waredetial.Cost,
                                                    Qty = (double)selectedQty,
                                                    ExpireDate = waredetial.ExpireDate,
                                                });
                                                waredetial.InStock -= (double)selectedQty;
                                                Cost = waredetial.Cost;
                                                // insert to waredetial
                                                var stockOut = new StockOut
                                                {
                                                    AdmissionDate = waredetial.AdmissionDate,
                                                    Cost = (decimal)waredetial.Cost,
                                                    CurrencyID = waredetial.CurrencyID,
                                                    Details = waredetial.Details,
                                                    ID = 0,
                                                    InStock = selectedQty,
                                                    ItemID = item.ItemID,
                                                    Location = waredetial.Location,
                                                    MfrDate = waredetial.MfrDate,
                                                    ProcessItem = ProcessItem.SEBA,
                                                    SyetemDate = DateTime.Now,
                                                    SysNum = 0,
                                                    TimeIn = DateTime.Now,
                                                    WarehouseID = waredetial.WarehouseID,
                                                    UomID = item.UomID,
                                                    UserID = Order.UserOrderID,
                                                    ExpireDate = waredetial.ExpireDate,
                                                    BatchAttr1 = waredetial.BatchAttr1,
                                                    BatchAttr2 = waredetial.BatchAttr2,
                                                    BatchNo = waredetial.BatchNo,
                                                    TransType = TransTypeWD.POS,
                                                    TransID = Order.ReceiptID,
                                                    OutStockFrom = Order.ReceiptID,
                                                    FromWareDetialID = waredetial.ID,
                                                    BPID = Order.CustomerID
                                                };
                                                _inventoryAccAmount = (decimal)waredetial.Cost * selectedQty;
                                                _COGSAccAmount = (decimal)waredetial.Cost * selectedQty;
                                                _context.StockOuts.Add(stockOut);
                                                _context.SaveChanges();
                                            }
                                            InsertFinancialPOS(
                                                inventoryAccID, COGSAccID, _inventoryAccAmount, _COGSAccAmount, journalEntryDetail,
                                                accountBalance, journalEntry, docType, douTypeID, Order, OffsetAcc, item
                                            );
                                        }
                                    }
                                }
                                wareForAudis = (from wa in wareForAudis
                                                group wa by wa.Cost into g
                                                let wha = g.FirstOrDefault()
                                                select new WareForAudiView
                                                {
                                                    Qty = g.Sum(i => i.Qty),
                                                    Cost = wha.Cost,
                                                    ExpireDate = wha.ExpireDate
                                                }).ToList();

                                if (wareForAudis.Any())
                                {
                                    foreach (var i in wareForAudis)
                                    {
                                        // insert to inventory audit
                                        var inventory_audit = _context.InventoryAudits
                                            .Where(w => w.ItemID == item.ItemID && w.WarehouseID == Order.WarehouseID && w.Cost == i.Cost).ToList();
                                        var inventory = new InventoryAudit
                                        {
                                            ID = 0,
                                            WarehouseID = Order.WarehouseID,
                                            BranchID = Order.BranchID,
                                            UserID = Order.UserOrderID,
                                            ItemID = item.ItemID,
                                            CurrencyID = Order.SysCurrencyID,
                                            UomID = orft.BaseUOM,
                                            InvoiceNo = Order.ReceiptNo,
                                            Trans_Type = docType.Code,
                                            Process = itemMaster.Process,
                                            SystemDate = DateTime.Now,
                                            TimeIn = DateTime.Now.ToShortTimeString().ToString(),
                                            Qty = i.Qty * -1,
                                            Cost = i.Cost,
                                            Price = 0,
                                            CumulativeQty = inventory_audit.Sum(q => q.Qty) - i.Qty,
                                            CumulativeValue = inventory_audit.Sum(s => s.Trans_Valuse) - (i.Qty * i.Cost),
                                            Trans_Valuse = i.Qty * i.Cost * -1,
                                            ExpireDate = i.ExpireDate,
                                            LocalCurID = Order.LocalCurrencyID,
                                            LocalSetRate = Order.LocalSetRate,
                                            CompanyID = Order.CompanyID,
                                            DocumentTypeID = docType.ID,
                                            SeriesID = Order.SeriesID,
                                            SeriesDetailID = Order.SeriesDID,
                                        };
                                        _utility.CumulativeValue(inventory.WarehouseID, inventory.ItemID, inventory.CumulativeValue, _itemAcc);
                                        _context.InventoryAudits.Add(inventory);
                                        _context.SaveChanges();
                                    }
                                }
                            }
                        }
                        else
                        {
                            List<WarehouseDetail> _whlists = wareDetails.Where(w => w.InStock > 0).OrderBy(i => i.SyetemDate).ToList();
                            if (warehouse.IsAllowNegativeStock && _whlists.Count == 0)
                            {
                                var wh = wareDetails.LastOrDefault();
                                _whlists.Add(wh);
                            }
                            foreach (var (item_warehouse, index) in _whlists.Select((value, i) => (value, i)))
                            {
                                InventoryAudit item_inventory_audit = new();
                                WarehouseDetail item_IssusStock = new();
                                @Check_Stock = item_warehouse.InStock - @Qty;
                                if (@Check_Stock < 0)
                                {
                                    @Remain = @Check_Stock * (-1);
                                    @IssusQty = @Qty - @Remain;
                                    if (@Remain <= 0)
                                    {
                                        @Qty = 0;
                                    }
                                    else if (@Qty > 0 && index == _whlists.Count - 1 && warehouse.IsAllowNegativeStock)
                                    {
                                        @IssusQty = @Qty;
                                    }
                                    else
                                    {
                                        @Qty = @Remain;
                                    }

                                    if (itemMaster.Process == "FIFO")
                                    {
                                        item_IssusStock = item_warehouse;
                                        double _cost = item_IssusStock.Cost;
                                        item_IssusStock.InStock -= @IssusQty;
                                        if (@IssusQty > 0)
                                        {
                                            var stockOuts = new StockOut
                                            {
                                                Cost = (decimal)item_warehouse.Cost,
                                                CurrencyID = item_warehouse.CurrencyID,
                                                ID = 0,
                                                InStock = (decimal)@IssusQty,
                                                ItemID = item.ItemID,
                                                ProcessItem = ProcessItem.FIFO,
                                                SyetemDate = DateTime.Now,
                                                TimeIn = DateTime.Now,
                                                WarehouseID = item_warehouse.WarehouseID,
                                                UomID = item.UomID,
                                                UserID = Order.UserOrderID,
                                                ExpireDate = item_IssusStock.ExpireDate,
                                                TransType = TransTypeWD.POS,
                                                TransID = Order.ReceiptID,
                                                OutStockFrom = Order.ReceiptID,
                                                BPID = Order.CustomerID,
                                                FromWareDetialID = item_IssusStock.ID,
                                            };
                                            _context.StockOuts.Add(stockOuts);
                                            var inventory_audit = _context.InventoryAudits.Where(w => w.ItemID == item.ItemID && w.WarehouseID == Order.WarehouseID).ToList();
                                            item_inventory_audit.ID = 0;
                                            item_inventory_audit.WarehouseID = Order.WarehouseID;
                                            item_inventory_audit.BranchID = Order.BranchID;
                                            item_inventory_audit.UserID = Order.UserOrderID;
                                            item_inventory_audit.ItemID = item.ItemID;
                                            item_inventory_audit.CurrencyID = Com.SystemCurrencyID;
                                            item_inventory_audit.UomID = orft.BaseUOM;
                                            item_inventory_audit.InvoiceNo = Order.ReceiptNo;
                                            item_inventory_audit.Trans_Type = docType.Code;
                                            item_inventory_audit.Process = itemMaster.Process;
                                            item_inventory_audit.SystemDate = DateTime.Now;
                                            item_inventory_audit.TimeIn = DateTime.Now.ToShortTimeString().ToString();
                                            item_inventory_audit.Qty = @IssusQty * -1;
                                            item_inventory_audit.Cost = _cost;
                                            item_inventory_audit.Price = 0;
                                            item_inventory_audit.CumulativeQty = inventory_audit.Sum(q => q.Qty) - @IssusQty;
                                            item_inventory_audit.CumulativeValue = inventory_audit.Sum(s => s.Trans_Valuse) - (@IssusQty * _cost);
                                            item_inventory_audit.Trans_Valuse = @IssusQty * item_IssusStock.Cost * -1;
                                            item_inventory_audit.ExpireDate = item_IssusStock.ExpireDate;
                                            item_inventory_audit.LocalCurID = Order.LocalCurrencyID;
                                            item_inventory_audit.LocalSetRate = Order.LocalSetRate;
                                            item_inventory_audit.CompanyID = Order.CompanyID;
                                            item_inventory_audit.DocumentTypeID = docType.ID;
                                            item_inventory_audit.SeriesID = Order.SeriesID;
                                            item_inventory_audit.SeriesDetailID = Order.SeriesDID;
                                        }
                                        inventoryAccAmount += (decimal)item_inventory_audit.Cost * (decimal)@IssusQty;
                                        COGSAccAmount += (decimal)item_inventory_audit.Cost * (decimal)@IssusQty;
                                        _utility.CumulativeValue(item_inventory_audit.WarehouseID, item_inventory_audit.ItemID, item_inventory_audit.CumulativeValue, _itemAcc);
                                        _context.WarehouseDetails.Update(item_IssusStock);
                                        _context.InventoryAudits.Add(item_inventory_audit);
                                        _context.SaveChanges();
                                    }
                                    else if (itemMaster.Process == "Average")
                                    {
                                        item_IssusStock = wareDetails.OrderByDescending(i => i.SyetemDate).FirstOrDefault();
                                        item_IssusStock.InStock -= @IssusQty;
                                        if (@IssusQty > 0)
                                        {
                                            var warehouse_summary = _context.WarehouseSummary.FirstOrDefault(w => w.ItemID == item.ItemID && w.WarehouseID == Order.WarehouseID);
                                            double @sysAvCost = warehouse_summary.Cost;
                                            var inventory_audit = _context.InventoryAudits.Where(w => w.ItemID == item.ItemID && w.WarehouseID == Order.WarehouseID).ToList();
                                            var stockOuts = new StockOut
                                            {
                                                Cost = (decimal)@sysAvCost,
                                                CurrencyID = item_warehouse.CurrencyID,
                                                ID = 0,
                                                InStock = (decimal)@IssusQty,
                                                ItemID = item.ItemID,
                                                ProcessItem = ProcessItem.Average,
                                                SyetemDate = DateTime.Now,
                                                TimeIn = DateTime.Now,
                                                WarehouseID = item_warehouse.WarehouseID,
                                                UomID = item.UomID,
                                                UserID = Order.UserOrderID,
                                                ExpireDate = item_IssusStock.ExpireDate,
                                                TransType = TransTypeWD.POS,
                                                TransID = Order.ReceiptID,
                                                OutStockFrom = Order.ReceiptID,
                                                BPID = Order.CustomerID,
                                                FromWareDetialID = item_IssusStock.ID,
                                            };
                                            _context.StockOuts.Add(stockOuts);

                                            item_inventory_audit.ID = 0;
                                            item_inventory_audit.WarehouseID = Order.WarehouseID;
                                            item_inventory_audit.BranchID = Order.BranchID;
                                            item_inventory_audit.UserID = Order.UserOrderID;
                                            item_inventory_audit.ItemID = item.ItemID;
                                            item_inventory_audit.CurrencyID = Com.SystemCurrencyID;
                                            item_inventory_audit.UomID = orft.BaseUOM;
                                            item_inventory_audit.InvoiceNo = Order.ReceiptNo;
                                            item_inventory_audit.Trans_Type = docType.Code;
                                            item_inventory_audit.Process = itemMaster.Process;
                                            item_inventory_audit.SystemDate = DateTime.Now;
                                            item_inventory_audit.TimeIn = DateTime.Now.ToShortTimeString().ToString();
                                            item_inventory_audit.Qty = @IssusQty * -1;
                                            item_inventory_audit.Cost = @sysAvCost;
                                            item_inventory_audit.Price = 0;
                                            item_inventory_audit.CumulativeQty = inventory_audit.Sum(q => q.Qty) - @IssusQty;
                                            item_inventory_audit.CumulativeValue = inventory_audit.Sum(s => s.Trans_Valuse) - (@IssusQty * @sysAvCost);
                                            item_inventory_audit.Trans_Valuse = @IssusQty * @sysAvCost * -1;
                                            item_inventory_audit.ExpireDate = item_IssusStock.ExpireDate;
                                            item_inventory_audit.LocalCurID = Order.LocalCurrencyID;
                                            item_inventory_audit.LocalSetRate = Order.LocalSetRate;
                                            item_inventory_audit.CompanyID = Order.CompanyID;
                                            item_inventory_audit.DocumentTypeID = docType.ID;
                                            item_inventory_audit.SeriesID = Order.SeriesID;
                                            item_inventory_audit.SeriesDetailID = Order.SeriesDID;
                                        }
                                        double @AvgCost = _utility.CalAVGCost(item.ItemID, Order.WarehouseID, item_inventory_audit);
                                        inventoryAccAmount += (decimal)(@AvgCost * @IssusQty);
                                        COGSAccAmount += (decimal)(@AvgCost * @IssusQty);
                                        _utility.UpdateAvgCost(item_warehouse.ItemID, Order.WarehouseID, itemMaster.GroupUomID, item_inventory_audit.Qty, item_inventory_audit.Cost);
                                        _utility.UpdateBomCost(item_warehouse.ItemID, item_inventory_audit.Qty, item_inventory_audit.Cost);
                                        _utility.CumulativeValue(item_inventory_audit.WarehouseID, item_inventory_audit.ItemID, item_inventory_audit.CumulativeValue, _itemAcc);
                                        _context.WarehouseDetails.Update(item_IssusStock);
                                        _context.InventoryAudits.Add(item_inventory_audit);
                                        _context.SaveChanges();
                                    }
                                }
                                else
                                {

                                    if (itemMaster.Process == "FIFO")
                                    {
                                        item_IssusStock = item_warehouse;
                                        @FIFOQty = item_IssusStock.InStock - @Qty;
                                        @IssusQty = item_IssusStock.InStock - @FIFOQty;
                                        item_IssusStock.InStock = @FIFOQty;
                                        if (@IssusQty > 0)
                                        {
                                            var stockOuts = new StockOut
                                            {
                                                Cost = (decimal)item_warehouse.Cost,
                                                CurrencyID = item_warehouse.CurrencyID,
                                                ID = 0,
                                                InStock = (decimal)@IssusQty,
                                                ItemID = item.ItemID,
                                                ProcessItem = ProcessItem.FIFO,
                                                SyetemDate = DateTime.Now,
                                                TimeIn = DateTime.Now,
                                                WarehouseID = item_warehouse.WarehouseID,
                                                UomID = item.UomID,
                                                UserID = Order.UserOrderID,
                                                ExpireDate = item_IssusStock.ExpireDate,
                                                TransType = TransTypeWD.POS,
                                                TransID = Order.ReceiptID,
                                                OutStockFrom = Order.ReceiptID,
                                                BPID = Order.CustomerID,
                                                FromWareDetialID = item_IssusStock.ID,
                                            };
                                            _context.StockOuts.Add(stockOuts);
                                            var inventory_audit = _context.InventoryAudits.Where(w => w.ItemID == item.ItemID && w.WarehouseID == Order.WarehouseID).ToList();
                                            item_inventory_audit.ID = 0;
                                            item_inventory_audit.WarehouseID = Order.WarehouseID;
                                            item_inventory_audit.BranchID = Order.BranchID;
                                            item_inventory_audit.UserID = Order.UserOrderID;
                                            item_inventory_audit.ItemID = item.ItemID;
                                            item_inventory_audit.CurrencyID = Com.SystemCurrencyID;
                                            item_inventory_audit.UomID = orft.BaseUOM;
                                            item_inventory_audit.InvoiceNo = Order.ReceiptNo;
                                            item_inventory_audit.Trans_Type = docType.Code;
                                            item_inventory_audit.Process = itemMaster.Process;
                                            item_inventory_audit.SystemDate = DateTime.Now;
                                            item_inventory_audit.TimeIn = DateTime.Now.ToShortTimeString().ToString();
                                            item_inventory_audit.Qty = @IssusQty * -1;
                                            item_inventory_audit.Cost = item_IssusStock.Cost;
                                            item_inventory_audit.Price = 0;
                                            item_inventory_audit.CumulativeQty = inventory_audit.Sum(q => q.Qty) - @IssusQty;
                                            item_inventory_audit.CumulativeValue = inventory_audit.Sum(s => s.Trans_Valuse) - (@IssusQty * item_IssusStock.Cost);
                                            item_inventory_audit.Trans_Valuse = @IssusQty * item_IssusStock.Cost * -1;
                                            item_inventory_audit.ExpireDate = item_IssusStock.ExpireDate;
                                            item_inventory_audit.LocalCurID = Order.LocalCurrencyID;
                                            item_inventory_audit.LocalSetRate = Order.LocalSetRate;
                                            item_inventory_audit.CompanyID = Order.CompanyID;
                                            item_inventory_audit.DocumentTypeID = docType.ID;
                                            item_inventory_audit.SeriesID = Order.SeriesID;
                                            item_inventory_audit.SeriesDetailID = Order.SeriesDID;
                                        }
                                        inventoryAccAmount += (decimal)item_inventory_audit.Cost * (decimal)@IssusQty;
                                        COGSAccAmount += (decimal)item_inventory_audit.Cost * (decimal)@IssusQty;
                                        _utility.CumulativeValue(item_inventory_audit.WarehouseID, item_inventory_audit.ItemID, item_inventory_audit.CumulativeValue, _itemAcc);
                                        _context.WarehouseDetails.Update(item_IssusStock);
                                        _context.InventoryAudits.Add(item_inventory_audit);
                                        _context.SaveChanges();
                                    }
                                    else if (itemMaster.Process == "Average")
                                    {
                                        item_IssusStock = wareDetails.OrderByDescending(i => i.SyetemDate).FirstOrDefault();
                                        @FIFOQty = item_IssusStock.InStock - @Qty;
                                        @IssusQty = item_IssusStock.InStock - @FIFOQty;
                                        item_IssusStock.InStock = @FIFOQty;
                                        if (@IssusQty > 0)
                                        {
                                            var warehouse_summary = _context.WarehouseSummary.FirstOrDefault(w => w.ItemID == item.ItemID && w.WarehouseID == Order.WarehouseID);
                                            double @sysAvCost = warehouse_summary.Cost;
                                            var inventory_audit = _context.InventoryAudits.Where(w => w.ItemID == item.ItemID && w.WarehouseID == Order.WarehouseID).ToList();

                                            var stockOuts = new StockOut
                                            {
                                                Cost = (decimal)@sysAvCost,
                                                CurrencyID = item_warehouse.CurrencyID,
                                                ID = 0,
                                                InStock = (decimal)@IssusQty,
                                                ItemID = item.ItemID,
                                                ProcessItem = ProcessItem.Average,
                                                SyetemDate = DateTime.Now,
                                                TimeIn = DateTime.Now,
                                                WarehouseID = item_warehouse.WarehouseID,
                                                UomID = item.UomID,
                                                UserID = Order.UserOrderID,
                                                ExpireDate = item_IssusStock.ExpireDate,
                                                TransType = TransTypeWD.POS,
                                                TransID = Order.ReceiptID,
                                                OutStockFrom = Order.ReceiptID,
                                                FromWareDetialID = item_IssusStock.ID,
                                                BPID = Order.CustomerID
                                            };
                                            _context.StockOuts.Add(stockOuts);
                                            item_inventory_audit.ID = 0;
                                            item_inventory_audit.WarehouseID = Order.WarehouseID;
                                            item_inventory_audit.BranchID = Order.BranchID;
                                            item_inventory_audit.UserID = Order.UserOrderID;
                                            item_inventory_audit.ItemID = item.ItemID;
                                            item_inventory_audit.CurrencyID = Com.SystemCurrencyID;
                                            item_inventory_audit.UomID = orft.BaseUOM;
                                            item_inventory_audit.InvoiceNo = Order.ReceiptNo;
                                            item_inventory_audit.Trans_Type = docType.Code;
                                            item_inventory_audit.Process = itemMaster.Process;
                                            item_inventory_audit.SystemDate = DateTime.Now;
                                            item_inventory_audit.TimeIn = DateTime.Now.ToShortTimeString().ToString();
                                            item_inventory_audit.Qty = @IssusQty * -1;
                                            item_inventory_audit.Cost = @sysAvCost;
                                            item_inventory_audit.Price = 0;
                                            item_inventory_audit.CumulativeQty = inventory_audit.Sum(q => q.Qty) - @IssusQty;
                                            item_inventory_audit.CumulativeValue = inventory_audit.Sum(s => s.Trans_Valuse) - (@IssusQty * @sysAvCost);
                                            item_inventory_audit.Trans_Valuse = @IssusQty * @sysAvCost * -1;
                                            item_inventory_audit.ExpireDate = item_IssusStock.ExpireDate;
                                            item_inventory_audit.LocalCurID = Order.LocalCurrencyID;
                                            item_inventory_audit.LocalSetRate = Order.LocalSetRate;
                                            item_inventory_audit.CompanyID = Order.CompanyID;
                                            item_inventory_audit.DocumentTypeID = docType.ID;
                                            item_inventory_audit.SeriesID = Order.SeriesID;
                                            item_inventory_audit.SeriesDetailID = Order.SeriesDID;
                                        }
                                        double @AvgCost = _utility.CalAVGCost(item.ItemID, Order.WarehouseID, item_inventory_audit);
                                        inventoryAccAmount += (decimal)(@AvgCost * @IssusQty);
                                        COGSAccAmount += (decimal)(@AvgCost * @IssusQty);
                                        _utility.UpdateAvgCost(item_warehouse.ItemID, Order.WarehouseID, itemMaster.GroupUomID, item_inventory_audit.Qty, item_inventory_audit.Cost);
                                        _utility.UpdateBomCost(item_warehouse.ItemID, item_inventory_audit.Qty, item_inventory_audit.Cost);
                                        _utility.CumulativeValue(item_inventory_audit.WarehouseID, item_inventory_audit.ItemID, item_inventory_audit.CumulativeValue, _itemAcc);
                                        _context.WarehouseDetails.Update(item_IssusStock);
                                        _context.InventoryAudits.Add(item_inventory_audit);
                                        _context.SaveChanges();
                                    }
                                    wareDetails = new List<WarehouseDetail>();
                                    break;
                                }
                            }
                        }

                    }
                    else
                    {
                        var priceListDetail = _context.PriceListDetails.FirstOrDefault(w => w.ItemID == item.ItemID && w.UomID == item.UomID && w.PriceListID == Order.PriceListID) ?? new PriceListDetail();
                        inventoryAccAmount += (decimal)priceListDetail.Cost * (decimal)item.Qty * (decimal)Order.ExchangeRate;
                        COGSAccAmount += (decimal)priceListDetail.Cost * (decimal)item.Qty * (decimal)Order.ExchangeRate;
                        var inventory_audit = _context.InventoryAudits.Where(w => w.ItemID == item.ItemID && w.WarehouseID == Order.WarehouseID).ToList();
                        InventoryAudit item_inventory_audit = new()
                        {
                            ID = 0,
                            WarehouseID = Order.WarehouseID,
                            BranchID = Order.BranchID,
                            UserID = Order.UserOrderID,
                            ItemID = item.ItemID,
                            CurrencyID = Com.SystemCurrencyID,
                            UomID = orft.BaseUOM,
                            InvoiceNo = Order.ReceiptNo,
                            Trans_Type = docType.Code,
                            Process = itemMaster.Process,
                            SystemDate = DateTime.Now,
                            TimeIn = DateTime.Now.ToShortTimeString().ToString(),
                            Qty = item.Qty * -1,
                            Cost = priceListDetail.Cost,
                            Price = 0,
                            CumulativeQty = inventory_audit.Sum(q => q.Qty) - item.Qty,
                            CumulativeValue = inventory_audit.Sum(s => s.Trans_Valuse) - (item.Qty * priceListDetail.Cost),
                            Trans_Valuse = item.Qty * priceListDetail.Cost * -1,
                            //ExpireDate = item_IssusStock.ExpireDate,
                            LocalCurID = Order.LocalCurrencyID,
                            LocalSetRate = Order.LocalSetRate,
                            CompanyID = Order.CompanyID,
                            DocumentTypeID = docType.ID,
                            SeriesID = Order.SeriesID,
                            SeriesDetailID = Order.SeriesDID,
                            TypeItem = "Standard",
                        };
                        _context.InventoryAudits.Update(item_inventory_audit);
                        _context.SaveChanges();
                    }

                    // Tax Account ///
                    var taxg = _context.TaxGroups.Find(item.TaxGroupID) ?? new TaxGroup();
                    var taxAcc = _context.GLAccounts.Find(taxg.GLID) ?? new GLAccount();
                    decimal taxValue = item.TaxValue * (decimal)Order.ExchangeRate;
                    if (taxAcc.ID > 0 && taxValue>0)
                    {
                        var taxjur = journalEntryDetail.FirstOrDefault(w => w.ItemID == taxAcc.ID) ?? new JournalEntryDetail();
                        if (taxjur.ItemID > 0)
                        {
                            var accBalance = accountBalance.FirstOrDefault(w => w.GLAID == taxAcc.ID);
                            taxAcc.Balance -= taxValue;
                            //journalEntryDetail
                            taxjur.Credit += taxValue;
                            //accountBalance
                            accBalance.CumulativeBalance = taxAcc.Balance;
                            accBalance.Credit += taxValue;
                        }
                        else
                        {
                            taxAcc.Balance -= taxValue;
                            journalEntryDetail.Add(new JournalEntryDetail
                            {
                                JEID = journalEntry.ID,
                                Type = Type.GLAcct,
                                ItemID = taxAcc.ID,
                                Credit = taxValue,
                            });
                            //
                            accountBalance.Add(new AccountBalance
                            {
                                JEID = journalEntry.ID,
                                PostingDate = Order.DateIn,
                                Origin = docType.ID,
                                OriginNo = Order.ReceiptNo,
                                OffsetAccount = taxAcc.Code,
                                Details = douTypeID.Name + " - " + taxAcc.Code,
                                CumulativeBalance = taxAcc.Balance,
                                Credit = taxValue,
                                LocalSetRate = (decimal)Order.LocalSetRate,
                                GLAID = taxAcc.ID,
                            });
                        }
                        _context.Update(taxAcc);
                    }
                    // Account Revenue
                    var glAccRevenfifo = _context.GLAccounts.FirstOrDefault(w => w.ID == revenueAccID) ?? new GLAccount();
                    if (glAccRevenfifo.ID > 0 && revenueAccAmount>0)
                    {
                        var list = journalEntryDetail.FirstOrDefault(w => w.ItemID == glAccRevenfifo.ID) ?? new JournalEntryDetail();
                        if (list.ItemID > 0)
                        {
                            var accBalance = accountBalance.FirstOrDefault(w => w.GLAID == revenueAccID);
                            glAccRevenfifo.Balance -= revenueAccAmount;
                            //journalEntryDetail
                            list.Credit += revenueAccAmount;
                            //accountBalance
                            accBalance.CumulativeBalance = glAccRevenfifo.Balance;
                            accBalance.Credit += revenueAccAmount;

                        }
                        else
                        {
                            glAccRevenfifo.Balance -= revenueAccAmount;
                            journalEntryDetail.Add(new JournalEntryDetail
                            {
                                JEID = journalEntry.ID,
                                Type = Type.GLAcct,
                                ItemID = revenueAccID,
                                Credit = revenueAccAmount,
                            });
                            accountBalance.Add(new AccountBalance
                            {
                                JEID = journalEntry.ID,
                                PostingDate = Order.DateOut,
                                Origin = docType.ID,
                                OriginNo = Order.ReceiptNo,
                                OffsetAccount = OffsetAcc,
                                Details = douTypeID.Name + "-" + glAccRevenfifo.Code,
                                CumulativeBalance = glAccRevenfifo.Balance,
                                Credit = revenueAccAmount,
                                LocalSetRate = (decimal)Order.LocalSetRate,
                                GLAID = revenueAccID,
                            });
                        }
                        _context.Update(glAccRevenfifo);
                    }

                }

                if (itemMaster.ManItemBy == ManageItemBy.None || item.IsKsmsMaster)
                {
                    InsertFinancialPOS(
                        inventoryAccID, COGSAccID, inventoryAccAmount, COGSAccAmount, journalEntryDetail,
                        accountBalance, journalEntry, docType, douTypeID, Order, OffsetAcc, item
                        );
                }
            }
            //IssuseInStockMaterial
            List<ItemMaterial> itemMaterials = new();
            foreach (var item in OrderDetails.ToList())
            {
                var itemM = _context.ItemMasterDatas.FirstOrDefault(w => w.ID == item.ItemID);
                var orft = _context.GroupDUoMs.FirstOrDefault(w => w.GroupUoMID == itemM.GroupUomID && w.AltUOM == item.UomID);
                var bom = _context.BOMaterial.FirstOrDefault(w => w.ItemID == item.ItemID && w.Active == true) ?? new BOMaterial();
                var items_material = (from bomd in _context.BOMDetail.Where(w => w.BID == bom.BID && w.Detele == false)
                                      join i in _context.ItemMasterDatas.Where(im => !im.Delete) on bomd.ItemID equals i.ID
                                      join gd in _context.GroupDUoMs on bomd.UomID equals gd.AltUOM
                                      join uom in _context.UnitofMeasures on i.InventoryUoMID equals uom.ID
                                      select new ItemMaterial
                                      {
                                          ItemID = bomd.ItemID,
                                          GroupUoMID = gd.GroupUoMID,
                                          GUoMID = i.GroupUomID,
                                          Qty = ((double)item.Qty * orft.Factor) * ((double)bomd.Qty * gd.Factor),
                                          NegativeStock = bomd.NegativeStock,
                                          Process = i.Process,
                                          UomID = uom.ID,
                                          Factor = gd.Factor,
                                          IsKsms = item.IsKsms,
                                          IsKsmsMaster = item.IsKsmsMaster,
                                          IsReadOnly = item.IsReadonly
                                      }).Where(w => w.GroupUoMID == w.GUoMID).ToList();
                itemMaterials.AddRange(items_material);
            }
            var allMaterials = (from all in itemMaterials
                                group new { all } by new { all.ItemID, all.NegativeStock } into g
                                let data = g.FirstOrDefault()
                                select new
                                {
                                    data.all.ItemID,
                                    data.all.GroupUoMID,
                                    data.all.GUoMID,
                                    Qty = g.Sum(s => s.all.Qty),
                                    data.all.NegativeStock,
                                    data.all.Process,
                                    data.all.UomID,
                                    data.all.Factor,
                                    data.all.IsKsms,
                                    data.all.IsKsmsMaster,
                                    data.all.IsReadOnly
                                }).ToList();
            if (allMaterials.Count > 0)
            {
                foreach (var item_detail in allMaterials.ToList())
                {
                    int inventoryAccIDavg = 0, COGSAccIDavg = 0;
                    decimal inventoryAccAmountavg = 0, COGSAccAmountavg = 0;
                    var item_master_data = _context.ItemMasterDatas.FirstOrDefault(w => w.ID == item_detail.ItemID);
                    var item_warehouse_material = _context.WarehouseSummary.FirstOrDefault(w => w.WarehouseID == Order.WarehouseID && w.ItemID == item_detail.ItemID);
                    var _itemAcc = _context.ItemAccountings.FirstOrDefault(i => i.WarehouseID == Order.WarehouseID && i.ItemID == item_detail.ItemID);
                    var all_item_warehouse_detail = _context.WarehouseDetails.Where(w => w.WarehouseID == Order.WarehouseID && w.ItemID == item_detail.ItemID).ToList();
                    var item_nagative = from wa in _context.WarehouseSummary.Where(w => w.ItemID == item_detail.ItemID)
                                        join na in _context.BOMDetail on wa.ItemID equals na.ItemID
                                        select new
                                        {
                                            NagaStock = wa.InStock
                                        };
                    if (item_master_data.SetGlAccount == SetGlAccount.ItemLevel)
                    {
                        var itemAccs = _context.ItemAccountings.Where(w => w.ItemID == item_detail.ItemID && w.WarehouseID == Order.WarehouseID);
                        var inventoryAcc = (from ia in itemAccs
                                            join gl in glAccs on ia.InventoryAccount equals gl.Code
                                            select gl
                                             ).FirstOrDefault() ?? new GLAccount();
                        var COGSAcc = (from ia in itemAccs
                                       join gl in glAccs on ia.CostofGoodsSoldAccount equals gl.Code
                                       select gl
                                             ).FirstOrDefault() ?? new GLAccount();
                        inventoryAccIDavg = inventoryAcc.ID;
                        COGSAccIDavg = COGSAcc.ID;
                    }
                    else if (item_master_data.SetGlAccount == SetGlAccount.ItemGroup)
                    {
                        var itemAccs = _context.ItemAccountings.Where(w => w.ItemGroupID == item_master_data.ItemGroup1ID);
                        var inventoryAcc = (from ia in itemAccs
                                            join gl in glAccs on ia.InventoryAccount equals gl.Code
                                            select gl
                                             ).FirstOrDefault() ?? new GLAccount();
                        var COGSAcc = (from ia in itemAccs
                                       join gl in glAccs on ia.CostofGoodsSoldAccount equals gl.Code
                                       select gl
                                             ).FirstOrDefault() ?? new GLAccount();
                        inventoryAccIDavg = inventoryAcc.ID;
                        COGSAccIDavg = COGSAcc.ID;
                    }
                    var nagative_check = item_nagative.Sum(w => w.NagaStock);
                    //WerehouseSummary
                    item_warehouse_material.Committed -= (double)item_detail.Qty;
                    item_warehouse_material.InStock -= (double)item_detail.Qty;
                    //Itemmasterdata
                    item_master_data.StockIn -= (double)item_detail.Qty;
                    _utility.UpdateItemAccounting(_itemAcc, item_warehouse_material);

                    if (!item_detail.IsKsms && !item_detail.IsKsmsMaster)
                    {
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
                                    var stockOuts = new StockOut
                                    {
                                        Cost = (decimal)item_IssusStock.Cost,
                                        CurrencyID = item_IssusStock.CurrencyID,
                                        ID = 0,
                                        InStock = (decimal)@IssusQty,
                                        ItemID = item_detail.ItemID,
                                        ProcessItem = ProcessItem.FIFO,
                                        SyetemDate = DateTime.Now,
                                        TimeIn = DateTime.Now,
                                        WarehouseID = item_IssusStock.WarehouseID,
                                        UomID = item_detail.UomID,
                                        UserID = Order.UserOrderID,
                                        ExpireDate = item_IssusStock.ExpireDate,
                                        TransType = TransTypeWD.POS,
                                        TransID = Order.ReceiptID,
                                        OutStockFrom = Order.ReceiptID,
                                        BPID = Order.CustomerID,
                                        FromWareDetialID = item_IssusStock.ID
                                    };
                                    _context.StockOuts.Add(stockOuts);

                                    var inventory_audit = _context.InventoryAudits.Where(w => w.ItemID == item_detail.ItemID && w.WarehouseID == Order.WarehouseID).ToList();
                                    item_inventory_audit.ID = 0;
                                    item_inventory_audit.WarehouseID = Order.WarehouseID;
                                    item_inventory_audit.BranchID = Order.BranchID;
                                    item_inventory_audit.UserID = Order.UserOrderID;
                                    item_inventory_audit.ItemID = item_detail.ItemID;
                                    item_inventory_audit.CurrencyID = Com.SystemCurrencyID;
                                    item_inventory_audit.UomID = item_detail.UomID;
                                    item_inventory_audit.InvoiceNo = Order.ReceiptNo;
                                    item_inventory_audit.Trans_Type = docType.Code;
                                    item_inventory_audit.Process = item_detail.Process;
                                    item_inventory_audit.SystemDate = DateTime.Now;
                                    item_inventory_audit.TimeIn = Order.TimeIn;
                                    item_inventory_audit.Qty = @IssusQty * -1;
                                    item_inventory_audit.Cost = item_IssusStock.Cost;
                                    item_inventory_audit.Price = 0;
                                    item_inventory_audit.CumulativeQty = inventory_audit.Sum(q => q.Qty) - @IssusQty;
                                    item_inventory_audit.CumulativeValue = inventory_audit.Sum(s => s.Trans_Valuse) - (@IssusQty * item_IssusStock.Cost);
                                    item_inventory_audit.Trans_Valuse = @IssusQty * item_IssusStock.Cost * (-1);
                                    item_inventory_audit.ExpireDate = item_IssusStock.ExpireDate;
                                    item_inventory_audit.LocalCurID = Com.LocalCurrencyID;
                                    item_inventory_audit.LocalSetRate = Exr.SetRate;
                                    item_inventory_audit.CompanyID = Order.CompanyID;
                                    item_inventory_audit.DocumentTypeID = docType.ID;
                                    item_inventory_audit.SeriesID = Order.SeriesID;
                                    item_inventory_audit.SeriesDetailID = Order.SeriesDID;
                                }
                                //
                                inventoryAccAmountavg += (decimal)item_inventory_audit.Cost * (decimal)@Qty;
                                COGSAccAmountavg += (decimal)item_inventory_audit.Cost * (decimal)@Qty;
                                _utility.CumulativeValue(item_inventory_audit.WarehouseID, item_inventory_audit.ItemID, item_inventory_audit.CumulativeValue, _itemAcc);
                            }
                            else if (item_detail.Process == "Average")
                            {
                                item_IssusStock.InStock = @FIFOQty;
                                if (@IssusQty > 0)
                                {
                                    var warehouse_summary = _context.WarehouseSummary.FirstOrDefault(w => w.ItemID == item_detail.ItemID && w.WarehouseID == Order.WarehouseID);
                                    double @sysAvCost = warehouse_summary.Cost;
                                    var inventory_audit = _context.InventoryAudits.Where(w => w.ItemID == item_detail.ItemID && w.WarehouseID == Order.WarehouseID).ToList();

                                    var stockOuts = new StockOut
                                    {
                                        Cost = (decimal)@sysAvCost,
                                        CurrencyID = item_IssusStock.CurrencyID,
                                        ID = 0,
                                        InStock = (decimal)@IssusQty,
                                        ItemID = item_detail.ItemID,
                                        ProcessItem = ProcessItem.Average,
                                        SyetemDate = DateTime.Now,
                                        TimeIn = DateTime.Now,
                                        WarehouseID = item_IssusStock.WarehouseID,
                                        UomID = item_detail.UomID,
                                        UserID = Order.UserOrderID,
                                        ExpireDate = item_IssusStock.ExpireDate,
                                        TransType = TransTypeWD.POS,
                                        TransID = Order.ReceiptID,
                                        OutStockFrom = Order.ReceiptID,
                                        BPID = Order.CustomerID,
                                        FromWareDetialID = item_IssusStock.ID
                                    };
                                    _context.StockOuts.Add(stockOuts);

                                    item_inventory_audit.ID = 0;
                                    item_inventory_audit.WarehouseID = Order.WarehouseID;
                                    item_inventory_audit.BranchID = Order.BranchID;
                                    item_inventory_audit.UserID = Order.UserOrderID;
                                    item_inventory_audit.ItemID = item_detail.ItemID;
                                    item_inventory_audit.CurrencyID = Com.SystemCurrencyID;
                                    item_inventory_audit.UomID = item_detail.UomID;
                                    item_inventory_audit.InvoiceNo = Order.ReceiptNo;
                                    item_inventory_audit.Trans_Type = docType.Code;
                                    item_inventory_audit.Process = item_detail.Process;
                                    item_inventory_audit.SystemDate = DateTime.Now;
                                    item_inventory_audit.TimeIn = Order.TimeIn;
                                    item_inventory_audit.Qty = @IssusQty * -1;
                                    item_inventory_audit.Cost = @sysAvCost;
                                    item_inventory_audit.Price = 0;
                                    item_inventory_audit.CumulativeQty = inventory_audit.Sum(q => q.Qty) - @IssusQty;
                                    item_inventory_audit.CumulativeValue = inventory_audit.Sum(s => s.Trans_Valuse) - (@IssusQty * @sysAvCost);
                                    item_inventory_audit.Trans_Valuse = @IssusQty * @sysAvCost * (-1);
                                    item_inventory_audit.ExpireDate = item_IssusStock.ExpireDate;
                                    item_inventory_audit.LocalCurID = Com.LocalCurrencyID;
                                    item_inventory_audit.LocalSetRate = Exr.SetRate;
                                    item_inventory_audit.CompanyID = Order.CompanyID;
                                    item_inventory_audit.DocumentTypeID = docType.ID;
                                    item_inventory_audit.SeriesID = Order.SeriesID;
                                    item_inventory_audit.SeriesDetailID = Order.SeriesDID;
                                }
                                double @AvgCost = _utility.CalAVGCost(item_detail.ItemID, Order.WarehouseID, item_inventory_audit);
                                inventoryAccAmountavg += (decimal)(@AvgCost * @Qty);
                                COGSAccAmountavg += (decimal)(@AvgCost * @Qty);
                                _utility.UpdateAvgCost(item_detail.ItemID, Order.WarehouseID, item_master_data.GroupUomID, item_inventory_audit.Qty, item_inventory_audit.Cost);
                                _utility.UpdateBomCost(item_detail.ItemID, item_inventory_audit.Qty, item_inventory_audit.Cost);
                                _utility.CumulativeValue(item_inventory_audit.WarehouseID, item_inventory_audit.ItemID, item_inventory_audit.CumulativeValue, _itemAcc);
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
                                InventoryAudit item_inventory_audit = new();
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
                                            var stockOuts = new StockOut
                                            {
                                                Cost = (decimal)item_IssusStock.Cost,
                                                CurrencyID = item_IssusStock.CurrencyID,
                                                ID = 0,
                                                InStock = (decimal)@IssusQty,
                                                ItemID = item_detail.ItemID,
                                                ProcessItem = ProcessItem.FIFO,
                                                SyetemDate = DateTime.Now,
                                                TimeIn = DateTime.Now,
                                                WarehouseID = item_IssusStock.WarehouseID,
                                                UomID = item_detail.UomID,
                                                UserID = Order.UserOrderID,
                                                ExpireDate = item_IssusStock.ExpireDate,
                                                TransType = TransTypeWD.POS,
                                                TransID = Order.ReceiptID,
                                                OutStockFrom = Order.ReceiptID,
                                                BPID = Order.CustomerID,
                                                FromWareDetialID = item_IssusStock.ID
                                            };
                                            _context.StockOuts.Add(stockOuts);
                                            var inventory_audit = _context.InventoryAudits.Where(w => w.ItemID == item_detail.ItemID && w.WarehouseID == Order.WarehouseID).ToList();
                                            item_inventory_audit.ID = 0;
                                            item_inventory_audit.WarehouseID = Order.WarehouseID;
                                            item_inventory_audit.BranchID = Order.BranchID;
                                            item_inventory_audit.UserID = Order.UserOrderID;
                                            item_inventory_audit.ItemID = item_detail.ItemID;
                                            item_inventory_audit.CurrencyID = Com.SystemCurrencyID;
                                            item_inventory_audit.UomID = item_detail.UomID;
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
                                        //                                        
                                        inventoryAccAmountavg += (decimal)item_inventory_audit.Cost * (decimal)@Qty;
                                        COGSAccAmountavg += (decimal)item_inventory_audit.Cost * (decimal)@Qty;
                                        _utility.CumulativeValue(item_inventory_audit.WarehouseID, item_inventory_audit.ItemID, item_inventory_audit.CumulativeValue, _itemAcc);
                                        _context.WarehouseDetails.Update(item_IssusStock);
                                        _context.InventoryAudits.Add(item_inventory_audit);
                                        _context.SaveChanges();
                                    }
                                    else if (item_detail.Process == "Average")
                                    {
                                        item_IssusStock.InStock = item_IssusStock.InStock -= @IssusQty;
                                        if (@IssusQty > 0)
                                        {
                                            var warehouse_summary = _context.WarehouseSummary.FirstOrDefault(w => w.ItemID == item_detail.ItemID && w.WarehouseID == Order.WarehouseID);
                                            double @sysAvCost = warehouse_summary.Cost;
                                            var inventory_audit = _context.InventoryAudits.Where(w => w.ItemID == item_detail.ItemID && w.WarehouseID == Order.WarehouseID).ToList();

                                            var stockOuts = new StockOut
                                            {
                                                Cost = (decimal)item_IssusStock.Cost,
                                                CurrencyID = item_IssusStock.CurrencyID,
                                                ID = 0,
                                                InStock = (decimal)@IssusQty,
                                                ItemID = item_detail.ItemID,
                                                ProcessItem = ProcessItem.FIFO,
                                                SyetemDate = DateTime.Now,
                                                TimeIn = DateTime.Now,
                                                WarehouseID = item_IssusStock.WarehouseID,
                                                UomID = item_detail.UomID,
                                                UserID = Order.UserOrderID,
                                                ExpireDate = item_IssusStock.ExpireDate,
                                                TransType = TransTypeWD.POS,
                                                TransID = Order.ReceiptID,
                                                OutStockFrom = Order.ReceiptID,
                                                BPID = Order.CustomerID,
                                                FromWareDetialID = item_IssusStock.ID
                                            };
                                            _context.StockOuts.Add(stockOuts);
                                            item_inventory_audit.ID = 0;
                                            item_inventory_audit.WarehouseID = Order.WarehouseID;
                                            item_inventory_audit.BranchID = Order.BranchID;
                                            item_inventory_audit.UserID = Order.UserOrderID;
                                            item_inventory_audit.ItemID = item_detail.ItemID;
                                            item_inventory_audit.CurrencyID = Com.SystemCurrencyID;
                                            item_inventory_audit.UomID = item_detail.UomID;
                                            item_inventory_audit.InvoiceNo = Order.ReceiptNo;
                                            item_inventory_audit.Trans_Type = docType.Code;
                                            item_inventory_audit.Process = item_detail.Process;
                                            item_inventory_audit.SystemDate = DateTime.Now;
                                            item_inventory_audit.TimeIn = Order.TimeIn;
                                            item_inventory_audit.Qty = @IssusQty * -1;
                                            item_inventory_audit.Cost = @sysAvCost;
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
                                            item_inventory_audit.DocumentTypeID = docType.ID;
                                            item_inventory_audit.SeriesID = Order.SeriesID;
                                            item_inventory_audit.SeriesDetailID = Order.SeriesDID;
                                        }
                                        //
                                        var inventoryAcct = _context.InventoryAudits.Where(w => w.ItemID == item_detail.ItemID).ToList();
                                        double @AvgCost = _utility.CalAVGCost(item_detail.ItemID, Order.WarehouseID, item_inventory_audit);
                                        inventoryAccAmountavg += (decimal)(@AvgCost * @Qty);
                                        COGSAccAmountavg += (decimal)(@AvgCost * @Qty);
                                        _utility.UpdateAvgCost(item_detail.ItemID, Order.WarehouseID, item_master_data.GroupUomID, item_inventory_audit.Qty, item_inventory_audit.Cost);
                                        _utility.UpdateBomCost(item_detail.ItemID, item_inventory_audit.Qty, item_inventory_audit.Cost);
                                        _utility.CumulativeValue(item_inventory_audit.WarehouseID, item_inventory_audit.ItemID, item_inventory_audit.CumulativeValue, _itemAcc);
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

                                            var stockOuts = new StockOut
                                            {
                                                Cost = (decimal)item_IssusStock.Cost,
                                                CurrencyID = item_IssusStock.CurrencyID,
                                                ID = 0,
                                                InStock = (decimal)@IssusQty,
                                                ItemID = item_detail.ItemID,
                                                ProcessItem = ProcessItem.FIFO,
                                                SyetemDate = DateTime.Now,
                                                TimeIn = DateTime.Now,
                                                WarehouseID = item_IssusStock.WarehouseID,
                                                UomID = item_detail.UomID,
                                                UserID = Order.UserOrderID,
                                                ExpireDate = item_IssusStock.ExpireDate,
                                                TransType = TransTypeWD.POS,
                                                TransID = Order.ReceiptID,
                                                OutStockFrom = Order.ReceiptID,
                                                BPID = Order.CustomerID,
                                                FromWareDetialID = item_IssusStock.ID
                                            };
                                            _context.StockOuts.Add(stockOuts);
                                            var inventory_audit = _context.InventoryAudits.Where(w => w.ItemID == item_detail.ItemID && w.WarehouseID == Order.WarehouseID).ToList();
                                            item_inventory_audit.ID = 0;
                                            item_inventory_audit.WarehouseID = Order.WarehouseID;
                                            item_inventory_audit.BranchID = Order.BranchID;
                                            item_inventory_audit.UserID = Order.UserOrderID;
                                            item_inventory_audit.ItemID = item_detail.ItemID;
                                            item_inventory_audit.CurrencyID = Com.SystemCurrencyID;
                                            item_inventory_audit.UomID = item_detail.UomID;
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
                                            item_inventory_audit.DocumentTypeID = docType.ID;
                                            item_inventory_audit.SeriesID = Order.SeriesID;
                                            item_inventory_audit.SeriesDetailID = Order.SeriesDID;
                                        }
                                        //
                                        inventoryAccAmountavg += (decimal)item_inventory_audit.Cost * (decimal)@Qty;
                                        COGSAccAmountavg += (decimal)item_inventory_audit.Cost * (decimal)@Qty;
                                        _utility.CumulativeValue(item_inventory_audit.WarehouseID, item_inventory_audit.ItemID, item_inventory_audit.CumulativeValue, _itemAcc);
                                        _context.WarehouseDetails.Update(item_IssusStock);
                                        _context.InventoryAudits.Add(item_inventory_audit);
                                        _context.SaveChanges();
                                    }
                                    else if (item_detail.Process == "Average")
                                    {
                                        item_IssusStock.InStock = @FIFOQty;
                                        if (@IssusQty > 0)
                                        {
                                            var warehouse_summary = _context.WarehouseSummary.FirstOrDefault(w => w.ItemID == item_detail.ItemID && w.WarehouseID == Order.WarehouseID);
                                            double @sysAvCost = warehouse_summary.Cost;
                                            var inventory_audit = _context.InventoryAudits.Where(w => w.ItemID == item_detail.ItemID && w.WarehouseID == Order.WarehouseID).ToList();

                                            var stockOuts = new StockOut
                                            {
                                                Cost = (decimal)item_IssusStock.Cost,
                                                CurrencyID = item_IssusStock.CurrencyID,
                                                ID = 0,
                                                InStock = (decimal)@IssusQty,
                                                ItemID = item_detail.ItemID,
                                                ProcessItem = ProcessItem.FIFO,
                                                SyetemDate = DateTime.Now,
                                                TimeIn = DateTime.Now,
                                                WarehouseID = item_IssusStock.WarehouseID,
                                                UomID = item_detail.UomID,
                                                UserID = Order.UserOrderID,
                                                ExpireDate = item_IssusStock.ExpireDate,
                                                TransType = TransTypeWD.POS,
                                                TransID = Order.ReceiptID,
                                                OutStockFrom = Order.ReceiptID,
                                                BPID = Order.CustomerID,
                                                FromWareDetialID = item_IssusStock.ID
                                            };
                                            _context.StockOuts.Add(stockOuts);
                                            item_inventory_audit.ID = 0;
                                            item_inventory_audit.WarehouseID = Order.WarehouseID;
                                            item_inventory_audit.BranchID = Order.BranchID;
                                            item_inventory_audit.UserID = Order.UserOrderID;
                                            item_inventory_audit.ItemID = item_detail.ItemID;
                                            item_inventory_audit.CurrencyID = Com.SystemCurrencyID;
                                            item_inventory_audit.UomID = item_detail.UomID;
                                            item_inventory_audit.InvoiceNo = Order.ReceiptNo;
                                            item_inventory_audit.Trans_Type = docType.Code;
                                            item_inventory_audit.Process = item_detail.Process;
                                            item_inventory_audit.SystemDate = DateTime.Now;
                                            item_inventory_audit.TimeIn = Order.TimeIn;
                                            item_inventory_audit.Qty = @IssusQty * -1;
                                            item_inventory_audit.Cost = @sysAvCost;
                                            item_inventory_audit.Price = 0;
                                            item_inventory_audit.CumulativeQty = inventory_audit.Sum(q => q.Qty) - @IssusQty;
                                            item_inventory_audit.CumulativeValue = inventory_audit.Sum(s => s.Trans_Valuse) - (@IssusQty * @sysAvCost);
                                            item_inventory_audit.Trans_Valuse = @IssusQty * @sysAvCost * -1;
                                            item_inventory_audit.ExpireDate = item_IssusStock.ExpireDate;
                                            item_inventory_audit.LocalCurID = Com.LocalCurrencyID;
                                            item_inventory_audit.LocalSetRate = Exr.SetRate;
                                            item_inventory_audit.CompanyID = Order.CompanyID;
                                            item_inventory_audit.DocumentTypeID = docType.ID;
                                            item_inventory_audit.SeriesID = Order.SeriesID;
                                            item_inventory_audit.SeriesDetailID = Order.SeriesDID;
                                        }
                                        double @AvgCost = _utility.CalAVGCost(item_detail.ItemID, Order.WarehouseID, item_inventory_audit);
                                        inventoryAccAmountavg += (decimal)(@AvgCost * @Qty);
                                        COGSAccAmountavg += (decimal)(@AvgCost * @Qty);
                                        _utility.UpdateAvgCost(item_detail.ItemID, Order.WarehouseID, item_master_data.GroupUomID, item_inventory_audit.Qty, item_inventory_audit.Cost);
                                        _utility.UpdateBomCost(item_detail.ItemID, item_inventory_audit.Qty, item_inventory_audit.Cost);
                                        _utility.CumulativeValue(item_inventory_audit.WarehouseID, item_inventory_audit.ItemID, item_inventory_audit.CumulativeValue, _itemAcc);

                                        _context.WarehouseDetails.Update(item_IssusStock);
                                        _context.InventoryAudits.Add(item_inventory_audit);
                                        _context.SaveChanges();
                                    }
                                    all_item_warehouse_detail = new List<WarehouseDetail>();
                                    break;
                                }
                            }
                        }

                        //inventoryAccID
                        var glAccInvenfifo = _context.GLAccounts.FirstOrDefault(w => w.ID == inventoryAccIDavg) ?? new GLAccount();
                        if (glAccInvenfifo.ID > 0)
                        {
                            var journalDetail = journalEntryDetail.FirstOrDefault(w => w.ItemID == glAccInvenfifo.ID) ?? new JournalEntryDetail();
                            if (journalDetail.ItemID > 0)
                            {
                                var accBalance = accountBalance.FirstOrDefault(w => w.GLAID == inventoryAccIDavg);
                                glAccInvenfifo.Balance -= inventoryAccAmountavg;
                                //journalEntryDetail
                                journalDetail.Credit += inventoryAccAmountavg;
                                //accountBalance
                                accBalance.CumulativeBalance = glAccInvenfifo.Balance;
                                accBalance.Credit += inventoryAccAmountavg;
                            }
                            else
                            {
                                glAccInvenfifo.Balance -= inventoryAccAmountavg;
                                journalEntryDetail.Add(new JournalEntryDetail
                                {
                                    JEID = journalEntry.ID,
                                    Type = Type.GLAcct,
                                    ItemID = inventoryAccIDavg,
                                    Credit = inventoryAccAmountavg,
                                });
                                //
                                accountBalance.Add(new AccountBalance
                                {
                                    JEID = journalEntry.ID,
                                    PostingDate = Order.DateOut,
                                    Origin = docType.ID,
                                    OriginNo = Order.ReceiptNo,
                                    OffsetAccount = OffsetAcc,
                                    Details = douTypeID.Name + " - " + glAccInvenfifo.Code,
                                    CumulativeBalance = glAccInvenfifo.Balance,
                                    Credit = inventoryAccAmountavg,
                                    LocalSetRate = (decimal)Order.LocalSetRate,
                                    GLAID = inventoryAccIDavg,
                                });
                            }
                            _context.GLAccounts.Update(glAccInvenfifo);
                        }
                        // COGS
                        var glAccCOGSfifo = _context.GLAccounts.FirstOrDefault(w => w.ID == COGSAccIDavg) ?? new GLAccount();
                        if (glAccCOGSfifo.ID > 0)
                        {
                            var journalDetail = journalEntryDetail.FirstOrDefault(w => w.ItemID == glAccCOGSfifo.ID) ?? new JournalEntryDetail();
                            if (journalDetail.ItemID > 0)
                            {
                                var accBalance = accountBalance.FirstOrDefault(w => w.GLAID == COGSAccIDavg);
                                glAccCOGSfifo.Balance += COGSAccAmountavg;
                                //journalEntryDetail
                                journalDetail.Debit += COGSAccAmountavg;
                                //accountBalance
                                accBalance.CumulativeBalance = glAccCOGSfifo.Balance;
                                accBalance.Debit += COGSAccAmountavg;
                            }
                            else
                            {
                                glAccCOGSfifo.Balance += COGSAccAmountavg;
                                journalEntryDetail.Add(new JournalEntryDetail
                                {
                                    JEID = journalEntry.ID,
                                    Type = Type.GLAcct,
                                    ItemID = COGSAccIDavg,
                                    Debit = COGSAccAmountavg,
                                });
                                //
                                accountBalance.Add(new AccountBalance
                                {
                                    JEID = journalEntry.ID,
                                    PostingDate = Order.DateOut,
                                    Origin = docType.ID,
                                    OriginNo = Order.ReceiptNo,
                                    OffsetAccount = OffsetAcc,
                                    Details = douTypeID.Name + "-" + glAccCOGSfifo.Code,
                                    CumulativeBalance = glAccCOGSfifo.Balance,
                                    Debit = COGSAccAmountavg,
                                    LocalSetRate = (decimal)Order.LocalSetRate,
                                    GLAID = COGSAccIDavg,
                                });
                            }
                            _context.GLAccounts.Update(glAccCOGSfifo);
                        }
                    }
                    _context.WarehouseSummary.Update(item_warehouse_material);
                    _context.ItemMasterDatas.Update(item_master_data);
                    _context.SaveChanges();
                }
            }
             if (Order.Received == Order.GrandTotal)
                {
                        var incomAccReceive = _context.BusinessPartners.FirstOrDefault(w => w.ID == Order.CustomerID);
                        var glAccD = _context.GLAccounts.FirstOrDefault(w => w.ID == incomAccReceive.GLAccID);
                        ////BPacc
                        glAccD.Balance -= (decimal)Order.GrandTotal_Sys;
                        journalEntryDetail.Add(new JournalEntryDetail
                        {
                            JEID = journalEntry.ID,
                            Type = Type.BPCode,
                            ItemID = incomAccReceive.GLAccID,
                            Credit = (decimal)Order.GrandTotal_Sys,
                            BPAcctID = Order.CustomerID,
                        });
                        glAccD.Balance -= (decimal)Order.GrandTotal_Sys;
                        accountBalance.Add(new AccountBalance
                        {
                            JEID = journalEntry.ID,
                            PostingDate = Order.DateIn,
                            Origin = docType.ID,
                            OriginNo = Order.ReceiptNo,
                            OffsetAccount = glAccD.Code,
                            Details = douTypeID.Name + " - " + glAccD.Code,
                            CumulativeBalance = glAccD.Balance,
                            Credit = (decimal)Order.GrandTotal_Sys,
                            LocalSetRate = (decimal)Order.LocalSetRate,
                            GLAID = incomAccReceive.GLAccID,
                            BPAcctID = Order.CustomerID,
                            Creator = Order.UserOrderID,
                        });
                        _context.Update(glAcc);
                        foreach (var mutli in multiPayments.Where(x => x.Amount > 0))
                        {
                            if (mutli.Amount > 0)
                            {
                                var accountSelect = _context.PaymentMeans.Find(mutli.PaymentMeanID) ?? new PaymentMeans();
                                var glAccC = _context.GLAccounts.FirstOrDefault(w => w.ID == accountSelect.AccountID) ?? new GLAccount();

                                if (glAccC.ID > 0)
                                {
                                    var vatAmountJur = journalEntryDetail.FirstOrDefault(w => w.ItemID == glAccC.ID) ?? new JournalEntryDetail();
                                    if (vatAmountJur.ItemID > 0)
                                    {
                                        var accBalance = accountBalance.FirstOrDefault(w => w.GLAID == glAccC.ID);
                                    glAccC.Balance += mutli.Amount * mutli.SCRate;
                                        //journalEntryDetail
                                        vatAmountJur.Debit += mutli.Amount *mutli.SCRate;
                                    //accountBalance
                                    accBalance.CumulativeBalance = glAccC.Balance;
                                        accBalance.Debit += mutli.Amount*mutli.SCRate;
                                        }
                                    else
                                    {
                                        journalEntryDetail.Add(new JournalEntryDetail
                                        {
                                            JEID = journalEntry.ID,
                                            Type = Financials.Type.BPCode,
                                            ItemID = accountSelect.AccountID,
                                            Debit = mutli.Amount *mutli.SCRate,
                                        });

                                        glAccC.Balance += mutli.Amount * mutli.SCRate;
                                    accountBalance.Add(new AccountBalance
                                        {
                                        JEID = journalEntry.ID,
                                        PostingDate = Order.DateIn,
                                            Origin = docType.ID,
                                            OriginNo = Order.ReceiptNo,
                                            OffsetAccount = glAccC.Code,
                                            Details = douTypeID.Name + "-" + glAccC.Code,
                                            CumulativeBalance = glAccC.Balance,
                                            Debit = mutli.Amount * mutli.SCRate,
                                        LocalSetRate = mutli.Amount * mutli.SCRate,
                                        GLAID = accountSelect.AccountID,
                                            Creator = Order.UserOrderID,
                                        });
                                    }
                                    _context.Update(glAccC);
                                }
                            }

                        }
                    
                }

            else if (Order.Received > Order.GrandTotal)
            {
                var incomAccReceive = _context.BusinessPartners.FirstOrDefault(w => w.ID == Order.CustomerID);
                var glAccD = _context.GLAccounts.FirstOrDefault(w => w.ID == incomAccReceive.GLAccID);
                ////BPacc
                glAccD.Balance -= (decimal)Order.Received;
                journalEntryDetail.Add(new JournalEntryDetail
                {
                    JEID = journalEntry.ID,
                    Type = Type.BPCode,
                    ItemID = incomAccReceive.GLAccID,
                    Credit = (decimal)Order.Received,
                    BPAcctID = Order.CustomerID,
                });
                glAccD.Balance -= (decimal)Order.Received;
                accountBalance.Add(new AccountBalance
                {
                    JEID = journalEntry.ID,
                    PostingDate = Order.DateIn,
                    Origin = docType.ID,
                    OriginNo = Order.ReceiptNo,
                    OffsetAccount = glAccD.Code,
                    Details = douTypeID.Name + " - " + glAccD.Code,
                    CumulativeBalance = glAccD.Balance,
                    Credit = (decimal)Order.Received,
                    LocalSetRate = (decimal)Order.LocalSetRate,
                    GLAID = incomAccReceive.GLAccID,
                    BPAcctID = Order.CustomerID,
                    Creator = Order.UserOrderID,
                });

                //=======================acount change=======================
                var lists = multiPayments.FirstOrDefault(x => x.Amount < 0) ?? new MultiPaymentMeans();
                if (lists.Amount == 0)
                {
                    var incomAccReceives = _context.BusinessPartners.FirstOrDefault(w => w.ID == Order.CustomerID);
                    var glAccDs = _context.GLAccounts.FirstOrDefault(w => w.ID == incomAccReceive.GLAccID);
                    ////BPacc
                    ///
                    glAccDs.Balance += (decimal)Order.Change;
                    journalEntryDetail.Add(new JournalEntryDetail
                    {
                        JEID = journalEntry.ID,
                        Type = Type.BPCode,
                        ItemID = incomAccReceives.GLAccID,
                        Debit = (decimal)Order.Change,
                        BPAcctID = Order.CustomerID,
                    });
                    glAccDs.Balance += (decimal)Order.Change;
                    accountBalance.Add(new AccountBalance
                    {
                        JEID = journalEntry.ID,
                        PostingDate = Order.DateIn,
                        Origin = docType.ID,
                        OriginNo = Order.ReceiptNo,
                        OffsetAccount = glAccDs.Code,
                        Details = douTypeID.Name + " - " + glAccDs.Code,
                        CumulativeBalance = glAccDs.Balance,
                        Debit = (decimal)Order.Change,
                        LocalSetRate = (decimal)Order.LocalSetRate,
                        GLAID = incomAccReceives.GLAccID,
                        BPAcctID = Order.CustomerID,
                        Creator = Order.UserOrderID,
                    });



                    var data = _context.MultipayMeansSetting.FirstOrDefault(x => x.Changed == true) ?? new MultipayMeansSetting();
                    var list = _context.PaymentMeans.FirstOrDefault(x => x.ID == data.PaymentID) ?? new PaymentMeans();
                    var glcc = _context.GLAccounts.FirstOrDefault(x => x.ID == list.AccountID) ?? new GLAccount();

                    if (glcc.ID > 0)
                    {
                        glcc.Balance -= (decimal)Order.Change;
                        journalEntryDetail.Add(new JournalEntryDetail
                        {
                            JEID = journalEntry.ID,
                            Type = Type.BPCode,
                            ItemID = list.AccountID,
                            Credit = (decimal)Order.Change,
                        });
                        glcc.Balance -= (decimal)Order.Change;
                        accountBalance.Add(new AccountBalance
                        {
                            JEID = journalEntry.ID,
                            PostingDate = Order.DateIn,
                            Origin = docType.ID,
                            OriginNo = Order.ReceiptNo,
                            OffsetAccount = glcc.Code,
                            Details = douTypeID.Name + " - " + glcc.Code,
                            CumulativeBalance = glcc.Balance,
                            Credit = (decimal)Order.Change,
                            LocalSetRate = (decimal)Order.LocalSetRate,
                            GLAID = list.AccountID,
                            Creator = Order.UserOrderID,
                        });
                    }
                }
                //=======================end acount change=======================

                //======================= acount pay=======================
                foreach (var mutli in multiPayments.Where(x => x.Amount != 0))
                {
                    if (mutli.Amount >= 0)
                    {
                        var d = _context.MultipayMeansSetting.FirstOrDefault(x => x.Changed == true) ?? new MultipayMeansSetting();
                        if (mutli.PaymentMeanID == d.PaymentID)
                        {
                            mutli.Amount += (decimal)Order.Change;
                        }
                        var accountSelect = _context.PaymentMeans.Find(mutli.PaymentMeanID) ?? new PaymentMeans();
                        var glAccC = _context.GLAccounts.FirstOrDefault(w => w.ID == accountSelect.AccountID) ?? new GLAccount();
                        if (glAccC.ID > 0)
                        {
                            //glAccC.Balance += mutli.Amount * mutli.SCRate;
                            journalEntryDetail.Add(new JournalEntryDetail
                            {
                                JEID = journalEntry.ID,
                                Type = Type.BPCode,
                                ItemID = accountSelect.AccountID,
                                Debit = mutli.Amount * mutli.SCRate,
                            });
                            glAccC.Balance += mutli.Amount * mutli.SCRate;
                            accountBalance.Add(new AccountBalance
                            {
                                JEID = journalEntry.ID,
                                PostingDate = Order.DateIn,
                                Origin = docType.ID,
                                OriginNo = Order.ReceiptNo,
                                OffsetAccount = glAccC.Code,
                                Details = douTypeID.Name + " - " + glAccC.Code,
                                CumulativeBalance = glAccC.Balance,
                                Debit = mutli.Amount * mutli.SCRate,
                                LocalSetRate = (decimal)Order.LocalSetRate,
                                GLAID = accountSelect.AccountID,
                                Creator = Order.UserOrderID,
                            });
                        }
                        #region
                        //if (glAccC.ID > 0)
                        //{
                        //    var vatAmountJur = journalEntryDetail.FirstOrDefault(w => w.ItemID == glAccC.ID) ?? new JournalEntryDetail();
                        //    if (vatAmountJur.ItemID > 0)
                        //    {
                        //        var accBalance = accountBalance.FirstOrDefault(w => w.GLAID == glAccC.ID);
                        //        glAccC.Balance += mutli.Amount * mutli.SCRate; 
                        //        //journalEntryDetail
                        //        vatAmountJur.Debit += mutli.Amount * mutli.SCRate;
                        //        //accountBalance
                        //        accBalance.CumulativeBalance = glAccC.Balance;
                        //        accBalance.Debit += mutli.Amount * mutli.SCRate;
                        //    }
                        //    else
                        //    {
                        //        journalEntryDetail.Add(new JournalEntryDetail
                        //        {
                        //            JEID = journalEntry.ID,
                        //            Type = Financials.Type.BPCode,
                        //            ItemID = accountSelect.AccountID,
                        //            Debit = mutli.Amount * mutli.SCRate,
                        //        });

                        //        glAccC.Balance += mutli.Amount * mutli.SCRate;
                        //        accountBalance.Add(new AccountBalance
                        //        {
                        //            PostingDate = Order.DateIn,
                        //            Origin = docType.ID,
                        //            OriginNo = Order.ReceiptNo,
                        //            OffsetAccount = glAccC.Code,
                        //            Details = douTypeID.Name + "-" + glAccC.Code,
                        //            CumulativeBalance = glAccC.Balance,
                        //            Debit = mutli.Amount * mutli.SCRate,
                        //            LocalSetRate = mutli.Amount * mutli.SCRate,
                        //            GLAID = accountSelect.AccountID,
                        //            Creator = Order.UserOrderID,
                        //        });
                        //    }
                        //    _context.Update(glAccC);
                        //}
                        #endregion

                    }
                    if (mutli.Amount < 0)
                    {
                        var incomAccReceives = _context.BusinessPartners.FirstOrDefault(w => w.ID == Order.CustomerID);
                        var glAccDs = _context.GLAccounts.FirstOrDefault(w => w.ID == incomAccReceive.GLAccID);
                        ////BPacc
                        ///
                        glAccDs.Balance += (decimal)Order.Change;
                        journalEntryDetail.Add(new JournalEntryDetail
                        {
                            JEID = journalEntry.ID,
                            Type = Type.BPCode,
                            ItemID = incomAccReceives.GLAccID,
                            Debit = (decimal)Order.Change,
                            BPAcctID = Order.CustomerID,
                        });
                        glAccDs.Balance += (decimal)Order.Change;
                        accountBalance.Add(new AccountBalance
                        {
                            JEID = journalEntry.ID,
                            PostingDate = Order.DateIn,
                            Origin = docType.ID,
                            OriginNo = Order.ReceiptNo,
                            OffsetAccount = glAccDs.Code,
                            Details = douTypeID.Name + " - " + glAccDs.Code,
                            CumulativeBalance = glAccDs.Balance,
                            Debit = (decimal)Order.Change,
                            LocalSetRate = (decimal)Order.LocalSetRate,
                            GLAID = incomAccReceives.GLAccID,
                            BPAcctID = Order.CustomerID,
                            Creator = Order.UserOrderID,
                        });

                        var accountSelect = _context.PaymentMeans.Find(mutli.PaymentMeanID) ?? new PaymentMeans();
                        var glAccC = _context.GLAccounts.FirstOrDefault(w => w.ID == accountSelect.AccountID) ?? new GLAccount();

                        if (glAccC.ID > 0)
                        {
                            var vatAmountJur = journalEntryDetail.FirstOrDefault(w => w.ItemID == glAccC.ID) ?? new JournalEntryDetail();
                            if (vatAmountJur.ItemID > 0)
                            {
                                var accBalance = accountBalance.FirstOrDefault(w => w.GLAID == glAccC.ID);
                                glAccC.Balance -= mutli.Amount * mutli.SCRate * -1;
                                //journalEntryDetail
                                vatAmountJur.Credit += mutli.Amount * mutli.SCRate * -1;
                                //accountBalance
                                accBalance.CumulativeBalance = glAccC.Balance;
                                accBalance.Credit += mutli.Amount * mutli.SCRate * -1;
                            }
                            else
                            {
                                journalEntryDetail.Add(new JournalEntryDetail
                                {
                                    JEID = journalEntry.ID,
                                    Type = Financials.Type.BPCode,
                                    ItemID = accountSelect.AccountID,
                                    Credit = mutli.Amount * mutli.SCRate * -1,
                                });

                                glAccC.Balance -= mutli.Amount * mutli.SCRate * -1;
                                accountBalance.Add(new AccountBalance
                                {
                                    JEID = journalEntry.ID,
                                    PostingDate = Order.DateIn,
                                    Origin = docType.ID,
                                    OriginNo = Order.ReceiptNo,
                                    OffsetAccount = glAccC.Code,
                                    Details = douTypeID.Name + "-" + glAccC.Code,
                                    CumulativeBalance = glAccC.Balance,
                                    Credit = mutli.Amount * mutli.SCRate * -1,
                                    LocalSetRate = mutli.Amount * mutli.SCRate * -1,
                                    GLAID = accountSelect.AccountID,
                                    Creator = Order.UserOrderID,
                                });
                            }
                            _context.Update(glAccC);
                        }
                    }


                }
                //=======================end acount pay=======================

            }
            else if (Order.Received < Order.GrandTotal_Sys && Order.Received>0)
            {
                var incomAccReceive = _context.BusinessPartners.FirstOrDefault(w => w.ID == Order.CustomerID);
                var glAccD = _context.GLAccounts.FirstOrDefault(w => w.ID == incomAccReceive.GLAccID);
                ////BPacc
                glAccD.Balance -= (decimal)Order.GrandTotal_Sys;
                journalEntryDetail.Add(new JournalEntryDetail
                {
                    JEID = journalEntry.ID,
                    Type = Type.BPCode,
                    ItemID = incomAccReceive.GLAccID,
                    Credit = (decimal)Order.Received,
                    BPAcctID = Order.CustomerID,
                });
                glAccD.Balance -= (decimal)Order.Received;
                accountBalance.Add(new AccountBalance
                {
                    JEID = journalEntry.ID,
                    PostingDate = Order.DateIn,
                    Origin = docType.ID,
                    OriginNo = Order.ReceiptNo,
                    OffsetAccount = glAccD.Code,
                    Details = douTypeID.Name + " - " + glAccD.Code,
                    CumulativeBalance = glAccD.Balance,
                    Credit = (decimal)Order.Received,
                    LocalSetRate = (decimal)Order.LocalSetRate,
                    GLAID = incomAccReceive.GLAccID,
                    BPAcctID = Order.CustomerID,
                    Creator = Order.UserOrderID,
                });
                ////BPacc 
                foreach (var mutli in multiPayments.Where(x => x.Amount != 0))
                {
                    if (mutli.Amount >= 0)
                    {

                        var accountSelect = _context.PaymentMeans.Find(mutli.PaymentMeanID) ?? new PaymentMeans();
                        var glAccC = _context.GLAccounts.FirstOrDefault(w => w.ID == accountSelect.AccountID) ?? new GLAccount();
                        if (glAccC.ID > 0)
                        {
                            var vatAmountJur = journalEntryDetail.FirstOrDefault(w => w.ItemID == glAccC.ID) ?? new JournalEntryDetail();
                            if (vatAmountJur.ItemID > 0)
                            {
                                var accBalance = accountBalance.FirstOrDefault(w => w.GLAID == glAccC.ID);
                                glAccC.Balance += mutli.Amount * mutli.SCRate;
                                //journalEntryDetail
                                vatAmountJur.Debit += mutli.Amount * mutli.SCRate;
                                //accountBalance
                                accBalance.CumulativeBalance = glAccC.Balance;
                                accBalance.Debit += mutli.Amount * mutli.SCRate;
                            }
                            else
                            {
                                journalEntryDetail.Add(new JournalEntryDetail
                                {
                                    JEID = journalEntry.ID,
                                    Type = Financials.Type.BPCode,
                                    ItemID = accountSelect.AccountID,
                                    Debit = mutli.Amount * mutli.SCRate,
                                });

                                glAccC.Balance += mutli.Amount * mutli.SCRate;
                                accountBalance.Add(new AccountBalance
                                {
                                    JEID = journalEntry.ID,
                                    PostingDate = Order.DateIn,
                                    Origin = docType.ID,
                                    OriginNo = Order.ReceiptNo,
                                    OffsetAccount = glAccC.Code,
                                    Details = douTypeID.Name + "-" + glAccC.Code,
                                    CumulativeBalance = glAccC.Balance,
                                    Debit = mutli.Amount * mutli.SCRate,
                                    LocalSetRate = mutli.Amount * mutli.SCRate,
                                    GLAID = accountSelect.AccountID,
                                    Creator = Order.UserOrderID,
                                });
                            }
                            _context.Update(glAccC);
                        }
                    }

                }

                var sysCurrency = _context.Currency.FirstOrDefault(i => i.ID == Order.SysCurrencyID);
                decimal appliSys = Order.AppliedAmount * (decimal)Order.ExchangeRate;
                // insert data to IncomingPaymentCustomer table ///
                incomingCus.Applied_Amount = (double)Order.AppliedAmount;
                incomingCus.BalanceDue = Order.GrandTotal - (double)Order.AppliedAmount;
                incomingCus.BranchID = Order.BranchID;
                incomingCus.CashDiscount = 0;
                incomingCus.CompanyID = Order.CompanyID;
                incomingCus.CurrencyID = Order.PLCurrencyID;
                incomingCus.CurrencyName = Order.Currency.Description;
                incomingCus.CustomerID = Order.CustomerID;
                incomingCus.Date = Order.DateIn;
                incomingCus.DocTypeID = docType.ID;
                incomingCus.ExchangeRate = Order.ExchangeRate;
                incomingCus.InvoiceNumber = Order.ReceiptNo;
                incomingCus.ItemInvoice = $"{docType.Code}-{Order.ReceiptNo}";
                incomingCus.LocalCurID = Order.LocalCurrencyID;
                incomingCus.LocalSetRate = Order.LocalSetRate;
                incomingCus.PostingDate = Order.DateIn;
                incomingCus.SeriesDID = Order.SeriesDID;
                incomingCus.SeriesID = Order.SeriesID;
                incomingCus.Status = "open";
                incomingCus.SysCurrency = Order.SysCurrencyID;
                incomingCus.SysName = sysCurrency.Description;
                incomingCus.Total = (double)Order.GrandTotal;
                incomingCus.TotalDiscount = 0;
                incomingCus.TotalPayment = Order.GrandTotal - (double)Order.AppliedAmount;
                incomingCus.WarehouseID = Order.WarehouseID;
                _context.IncomingPaymentCustomers.Update(incomingCus);
                // Create Series And Journal //
                SeriesDetail seriesDetailRC = new();
                var docTypeRC = _context.DocumentTypes.FirstOrDefault(i => i.Code == "RC");
                var seriesRC = _context.Series.FirstOrDefault(i => i.Default && i.CompanyID == Order.CompanyID && i.DocuTypeID == docTypeRC.ID);
                seriesDetailRC.Number = seriesRC.NextNo;
                seriesDetailRC.SeriesID = seriesRC.ID;
                _context.SeriesDetails.Update(seriesDetailRC);
                _context.SaveChanges();
                var seriesDetailID = seriesDetail.ID;
                string Sno = seriesRC.NextNo;
                long No = long.Parse(Sno);
                seriesRC.NextNo = Convert.ToString(No + 1);
                _context.Series.Update(seriesRC);
                _context.SaveChanges();
                /// insert data to incomingpayment ///
                incoming.BranchID = Order.BranchID;
                incoming.CompanyID = Order.CompanyID;
                incoming.CustomerID = Order.CustomerID;
                incoming.DocTypeID = docTypeRC.ID;
                incoming.SeriesDID = seriesDetailRC.ID;
                incoming.SeriesID = seriesRC.ID;
                incoming.InvoiceNumber = seriesDetailRC.Number;
                incoming.DocumentDate = Order.DateIn;
                incoming.LocalCurID = Order.LocalCurrencyID;
                incoming.LocalSetRate = Order.LocalSetRate;
                incoming.PaymentMeanID = Order.PaymentMeansID;
                incoming.PostingDate = Order.DateIn;
                incoming.TotalAmountDue = Order.GrandTotal - (double)Order.AppliedAmount;
                incoming.UserID = Order.UserOrderID;
                incoming.Status = "open";
                incoming.IcoPayCusID = incomingCus.IncomingPaymentCustomerID;
                _context.IncomingPayments.Update(incoming);
                _context.SaveChanges();
                incomingDetail.Applied_Amount = (double)Order.AppliedAmount;
                incomingDetail.BalanceDue = Order.GrandTotal - (double)Order.AppliedAmount;
                incomingDetail.CashDiscount = 0;
                incomingDetail.CurrencyID = Order.PLCurrencyID;
                incomingDetail.Date = Order.DateIn;
                incomingDetail.Delete = false;
                incomingDetail.CheckPay = true;
                incomingDetail.DocNo = docType.Code;
                incomingDetail.DocTypeID = docType.ID;
                incomingDetail.IncomingPaymentID = incoming.IncomingPaymentID;
                incomingDetail.InvoiceNumber = Order.ReceiptNo;
                incomingDetail.ItemInvoice = $"{docType.Code}-{Order.ReceiptNo}";
                incomingDetail.LocalCurID = Order.LocalCurrencyID;
                incomingDetail.LocalSetRate = Order.LocalSetRate;
                incomingDetail.Total = Order.GrandTotal;
                incomingDetail.TotalDiscount = 0;
                incomingDetail.Totalpayment = (double)Order.AppliedAmount;
                incomingDetail.CurrencyName = Order.Currency.Description;
                incomingDetail.ExchangeRate = Order.ExchangeRate;
                incomingDetail.IcoPayCusID = incomingCus.IncomingPaymentCustomerID;
                _context.IncomingPaymentDetails.Update(incomingDetail);
                _context.SeriesDetails.Update(seriesDetailRC);
                _context.SaveChanges();
            }

            else if (Order.Received == 0 && Order.GrandTotal > 0)
            {
                var sysCurrency = _context.Currency.FirstOrDefault(i => i.ID == Order.SysCurrencyID);
                // insert data to IncomingPaymentCustomer table ///
                incomingCus.Applied_Amount = (double)Order.AppliedAmount;
                incomingCus.BalanceDue = Order.GrandTotal - (double)Order.AppliedAmount;
                incomingCus.BranchID = Order.BranchID;
                incomingCus.CashDiscount = 0;
                incomingCus.CompanyID = Order.CompanyID;
                incomingCus.CurrencyID = Order.PLCurrencyID;
                incomingCus.CurrencyName = Order.Currency.Description;
                incomingCus.CustomerID = Order.CustomerID;
                incomingCus.Date = Order.DateIn;
                incomingCus.DocTypeID = docType.ID;
                incomingCus.ExchangeRate = Order.ExchangeRate;
                incomingCus.InvoiceNumber = Order.ReceiptNo;
                incomingCus.ItemInvoice = $"{docType.Code}-{Order.ReceiptNo}";
                incomingCus.LocalCurID = Order.LocalCurrencyID;
                incomingCus.LocalSetRate = Order.LocalSetRate;
                incomingCus.PostingDate = Order.DateIn;
                incomingCus.SeriesDID = Order.SeriesDID;
                incomingCus.SeriesID = Order.SeriesID;
                incomingCus.Status = "open";
                incomingCus.SysCurrency = Order.SysCurrencyID;
                incomingCus.SysName = sysCurrency.Description;
                incomingCus.Total = (double)Order.GrandTotal;
                incomingCus.TotalDiscount = 0;
                incomingCus.TotalPayment = Order.GrandTotal - (double)Order.AppliedAmount;
                incomingCus.WarehouseID = Order.WarehouseID;
                _context.IncomingPaymentCustomers.Update(incomingCus);
                _context.SaveChanges();
            }

            #region
            //else if ((decimal)Order.GrandTotal > Order.AppliedAmount && Order.AppliedAmount > 0)
            //{
            //    var sysCurrency = _context.Currency.FirstOrDefault(i => i.ID == Order.SysCurrencyID);
            //    decimal appliSys = Order.AppliedAmount * (decimal)Order.ExchangeRate;
            //    // insert data to IncomingPaymentCustomer table ///
            //    incomingCus.Applied_Amount = (double)Order.AppliedAmount;
            //    incomingCus.BalanceDue = Order.GrandTotal - (double)Order.AppliedAmount;
            //    incomingCus.BranchID = Order.BranchID;
            //    incomingCus.CashDiscount = 0;
            //    incomingCus.CompanyID = Order.CompanyID;
            //    incomingCus.CurrencyID = Order.PLCurrencyID;
            //    incomingCus.CurrencyName = Order.Currency.Description;
            //    incomingCus.CustomerID = Order.CustomerID;
            //    incomingCus.Date = Order.DateIn;
            //    incomingCus.DocTypeID = docType.ID;
            //    incomingCus.ExchangeRate = Order.ExchangeRate;
            //    incomingCus.InvoiceNumber = Order.ReceiptNo;
            //    incomingCus.ItemInvoice = $"{docType.Code}-{Order.ReceiptNo}";
            //    incomingCus.LocalCurID = Order.LocalCurrencyID;
            //    incomingCus.LocalSetRate = Order.LocalSetRate;
            //    incomingCus.PostingDate = Order.DateIn;
            //    incomingCus.SeriesDID = Order.SeriesDID;
            //    incomingCus.SeriesID = Order.SeriesID;
            //    incomingCus.Status = "open";
            //    incomingCus.SysCurrency = Order.SysCurrencyID;
            //    incomingCus.SysName = sysCurrency.Description;
            //    incomingCus.Total = (double)Order.GrandTotal;
            //    incomingCus.TotalDiscount = 0;
            //    incomingCus.TotalPayment = Order.GrandTotal - (double)Order.AppliedAmount;
            //    incomingCus.WarehouseID = Order.WarehouseID;
            //    _context.IncomingPaymentCustomers.Update(incomingCus);
            //    // Create Series And Journal //
            //    SeriesDetail seriesDetailRC = new();
            //    var docTypeRC = _context.DocumentTypes.FirstOrDefault(i => i.Code == "RC");
            //    var seriesRC = _context.Series.FirstOrDefault(i => i.Default && i.CompanyID == Order.CompanyID && i.DocuTypeID == docTypeRC.ID);
            //    seriesDetailRC.Number = seriesRC.NextNo;
            //    seriesDetailRC.SeriesID = seriesRC.ID;
            //    _context.SeriesDetails.Update(seriesDetailRC);
            //    _context.SaveChanges();
            //    var seriesDetailID = seriesDetail.ID;
            //    string Sno = seriesRC.NextNo;
            //    long No = long.Parse(Sno);
            //    seriesRC.NextNo = Convert.ToString(No + 1);
            //    _context.Series.Update(seriesRC);
            //    _context.SaveChanges();
            //    /// insert data to incomingpayment ///
            //    incoming.BranchID = Order.BranchID;
            //    incoming.CompanyID = Order.CompanyID;
            //    incoming.CustomerID = Order.CustomerID;
            //    incoming.DocTypeID = docTypeRC.ID;
            //    incoming.SeriesDID = seriesDetailRC.ID;
            //    incoming.SeriesID = seriesRC.ID;
            //    incoming.InvoiceNumber = seriesDetailRC.Number;
            //    incoming.DocumentDate = Order.DateIn;
            //    incoming.LocalCurID = Order.LocalCurrencyID;
            //    incoming.LocalSetRate = Order.LocalSetRate;
            //    incoming.PaymentMeanID = Order.PaymentMeansID;
            //    incoming.PostingDate = Order.DateIn;
            //    incoming.TotalAmountDue = Order.GrandTotal - (double)Order.AppliedAmount;
            //    incoming.UserID = Order.UserOrderID;
            //    incoming.Status = "open";
            //    incoming.IcoPayCusID = incomingCus.IncomingPaymentCustomerID;
            //    _context.IncomingPayments.Update(incoming);
            //    _context.SaveChanges();
            //    incomingDetail.Applied_Amount = (double)Order.AppliedAmount;
            //    incomingDetail.BalanceDue = Order.GrandTotal - (double)Order.AppliedAmount;
            //    incomingDetail.CashDiscount = 0;
            //    incomingDetail.CurrencyID = Order.PLCurrencyID;
            //    incomingDetail.Date = Order.DateIn;
            //    incomingDetail.Delete = false;
            //    incomingDetail.CheckPay = true;
            //    incomingDetail.DocNo = docType.Code;
            //    incomingDetail.DocTypeID = docType.ID;
            //    incomingDetail.IncomingPaymentID = incoming.IncomingPaymentID;
            //    incomingDetail.InvoiceNumber = Order.ReceiptNo;
            //    incomingDetail.ItemInvoice = $"{docType.Code}-{Order.ReceiptNo}";
            //    incomingDetail.LocalCurID = Order.LocalCurrencyID;
            //    incomingDetail.LocalSetRate = Order.LocalSetRate;
            //    incomingDetail.Total = Order.GrandTotal;
            //    incomingDetail.TotalDiscount = 0;
            //    incomingDetail.Totalpayment = (double)Order.AppliedAmount;
            //    incomingDetail.CurrencyName = Order.Currency.Description;
            //    incomingDetail.ExchangeRate = Order.ExchangeRate;
            //    incomingDetail.IcoPayCusID = incomingCus.IncomingPaymentCustomerID;
            //    _context.IncomingPaymentDetails.Update(incomingDetail);
            //    _context.SeriesDetails.Update(seriesDetailRC);
            //    _context.SaveChanges();

            //    IncomingPaymentSeriesAccounting(journalEntryDetail, accountBalance, journalEntry, docType, douTypeID, incoming, incomingDetail, Order);
            //    if (multiPayments != null)
            //    {
            //        var incomAccReceive = _context.BusinessPartners.FirstOrDefault(w => w.ID == Order.CustomerID);
            //        var glAccD = _context.GLAccounts.FirstOrDefault(w => w.ID == incomAccReceive.GLAccID);
            //        ////BPacc
            //        glAccD.Balance += (decimal)Order.GrandTotal_Sys;
            //        journalEntryDetail.Add(new JournalEntryDetail
            //        {
            //            JEID = journalEntry.ID,
            //            Type = Type.BPCode,
            //            ItemID = incomAccReceive.GLAccID,
            //            Credit = (decimal)Order.GrandTotal_Sys,
            //            BPAcctID = Order.CustomerID,
            //        });
            //        glAccD.Balance += (decimal)Order.GrandTotal_Sys;
            //        accountBalance.Add(new AccountBalance
            //        {
            //            JEID = journalEntry.ID,

            //            PostingDate = Order.DateIn,
            //            Origin = docType.ID,
            //            OriginNo = Order.ReceiptNo,
            //            OffsetAccount = glAccD.Code,
            //            Details = douTypeID.Name + " - " + glAccD.Code,
            //            CumulativeBalance = glAccD.Balance,
            //            Credit = (decimal)Order.GrandTotal_Sys,
            //            LocalSetRate = (decimal)Order.LocalSetRate,
            //            GLAID = incomAccReceive.GLAccID,
            //            BPAcctID = Order.CustomerID,
            //            Creator = Order.UserOrderID,
            //        });
            //        _context.Update(glAcc);



            //        foreach (var mutli in multiPayments.Where(x => x.Amount > 0))
            //        {
            //            if (mutli.Amount > 0)
            //            {
            //                var accountSelect = _context.PaymentMeans.Find(mutli.PaymentMeanID) ?? new PaymentMeans();
            //                var glAccC = _context.GLAccounts.FirstOrDefault(w => w.ID == accountSelect.AccountID) ?? new GLAccount();

            //                if (glAccC.ID > 0)
            //                {
            //                    var vatAmountJur = journalEntryDetail.FirstOrDefault(w => w.ItemID == glAccC.ID) ?? new JournalEntryDetail();
            //                    if (vatAmountJur.ItemID > 0)
            //                    {
            //                        var accBalance = accountBalance.FirstOrDefault(w => w.GLAID == glAccC.ID);
            //                        glAccC.Balance += mutli.Amount * mutli.SCRate;
            //                        //journalEntryDetail
            //                        vatAmountJur.Debit += mutli.Amount * mutli.SCRate;
            //                        //accountBalance
            //                        accBalance.CumulativeBalance = glAccC.Balance;
            //                        accBalance.Debit += mutli.Amount * mutli.SCRate;
            //                    }
            //                    else
            //                    {
            //                        journalEntryDetail.Add(new JournalEntryDetail
            //                        {
            //                            JEID = journalEntry.ID,
            //                            Type = Financials.Type.BPCode,
            //                            ItemID = accountSelect.AccountID,
            //                            Debit = mutli.Amount * mutli.SCRate,
            //                        });

            //                        glAccC.Balance += mutli.Amount * mutli.SCRate;
            //                        accountBalance.Add(new AccountBalance
            //                        {
            //                            JEID = journalEntry.ID,

            //                            PostingDate = Order.DateIn,
            //                            Origin = docType.ID,
            //                            OriginNo = Order.ReceiptNo,
            //                            OffsetAccount = glAccC.Code,
            //                            Details = douTypeID.Name + "-" + glAccC.Code,
            //                            CumulativeBalance = glAccC.Balance,
            //                            Debit = mutli.Amount * mutli.SCRate,
            //                            LocalSetRate = mutli.Amount * mutli.SCRate,
            //                            GLAID = accountSelect.AccountID,
            //                            Creator = Order.UserOrderID,
            //                        });
            //                    }
            //                    _context.Update(glAccC);
            //                }
            //            }
            //        }

            //    }

            //}
            //else if (Order.AppliedAmount == 0 && Order.GrandTotal > 0)
            //{
            //    var sysCurrency = _context.Currency.FirstOrDefault(i => i.ID == Order.SysCurrencyID);
            //    // insert data to IncomingPaymentCustomer table ///
            //    incomingCus.Applied_Amount = (double)Order.AppliedAmount;
            //    incomingCus.BalanceDue = Order.GrandTotal - (double)Order.AppliedAmount;
            //    incomingCus.BranchID = Order.BranchID;
            //    incomingCus.CashDiscount = 0;
            //    incomingCus.CompanyID = Order.CompanyID;
            //    incomingCus.CurrencyID = Order.PLCurrencyID;
            //    incomingCus.CurrencyName = Order.Currency.Description;
            //    incomingCus.CustomerID = Order.CustomerID;
            //    incomingCus.Date = Order.DateIn;
            //    incomingCus.DocTypeID = docType.ID;
            //    incomingCus.ExchangeRate = Order.ExchangeRate;
            //    incomingCus.InvoiceNumber = Order.ReceiptNo;
            //    incomingCus.ItemInvoice = $"{docType.Code}-{Order.ReceiptNo}";
            //    incomingCus.LocalCurID = Order.LocalCurrencyID;
            //    incomingCus.LocalSetRate = Order.LocalSetRate;
            //    incomingCus.PostingDate = Order.DateIn;
            //    incomingCus.SeriesDID = Order.SeriesDID;
            //    incomingCus.SeriesID = Order.SeriesID;
            //    incomingCus.Status = "open";
            //    incomingCus.SysCurrency = Order.SysCurrencyID;
            //    incomingCus.SysName = sysCurrency.Description;
            //    incomingCus.Total = (double)Order.GrandTotal;
            //    incomingCus.TotalDiscount = 0;
            //    incomingCus.TotalPayment = Order.GrandTotal - (double)Order.AppliedAmount;
            //    incomingCus.WarehouseID = Order.WarehouseID;
            //    _context.IncomingPaymentCustomers.Update(incomingCus);
            //    _context.SaveChanges();
            //}

            #endregion

            if (journalEntry.ID > 0)
            {
                journalEntry.TotalDebit = journalEntryDetail.Sum(s => s.Debit);
                journalEntry.TotalCredit = journalEntryDetail.Sum(s => s.Credit);
                _context.JournalEntries.Update(journalEntry);
            }
            AlertPaymentAsync(Order, series.Name, Order.ReceiptNo, OrderDetails.Count).Wait();
            _context.JournalEntryDetails.UpdateRange(journalEntryDetail);
            
            _context.AccountBalances.UpdateRange(accountBalance);
            _context.SaveChanges();
        }
        private void IncomingPaymentSeriesAccounting(List<JournalEntryDetail> journalEntryDetail,
            List<AccountBalance> accountBalance, JournalEntry journalEntry, DocumentType docType,
            DocumentType douTypeID, IncomingPayment incoming, IncomingPaymentDetail incomingDetail,
            Receipt Order)
        {
            // AccountReceice
            var incomAccReceive = _context.BusinessPartners.FirstOrDefault(w => w.ID == incoming.CustomerID);
            var glAccD = _context.GLAccounts.FirstOrDefault(w => w.ID == incomAccReceive.GLAccID);
            decimal cardPay = Order.OtherPaymentGrandTotal * (decimal)Order.ExchangeRate;
            decimal totalpay = (decimal)(incomingDetail.Applied_Amount * incomingDetail.ExchangeRate) - cardPay;
            journalEntryDetail.Add(new JournalEntryDetail
            {
                JEID = journalEntry.ID,
                Type = Type.BPCode,
                ItemID = incomAccReceive.GLAccID,
                Credit = totalpay,
                BPAcctID = incoming.CustomerID,
            });
            //Insert 
            glAccD.Balance -= totalpay;
            accountBalance.Add(new AccountBalance
            {
                JEID = journalEntry.ID,

                PostingDate = incoming.PostingDate,
                Origin = docType.ID,
                OriginNo = Order.ReceiptNo,
                OffsetAccount = glAccD.Code,
                Details = douTypeID.Name + " - " + glAccD.Code,
                CumulativeBalance = glAccD.Balance,
                Credit = totalpay,
                LocalSetRate = (decimal)incoming.LocalSetRate,
                GLAID = incomAccReceive.GLAccID,
                BPAcctID = incoming.CustomerID,
                Creator = incoming.UserID,
            });
            var accountReceive = _context.BusinessPartners.FirstOrDefault(w => w.ID == Order.CustomerID);
            var glAcc = _context.GLAccounts.FirstOrDefault(w => w.ID == accountReceive.GLAccID) ?? new GLAccount();
            string OffsetAcc = accountReceive.Code + "-" + glAcc.Code;
            journalEntryDetail.Add(new JournalEntryDetail
            {
                JEID = journalEntry.ID,
                Type = Type.BPCode,
                ItemID = accountReceive.GLAccID,
                Credit = cardPay,
                BPAcctID = Order.CustomerID,
            });
            //Insert             
            glAcc.Balance -= cardPay;
            accountBalance.Add(new AccountBalance
            {
                JEID = journalEntry.ID,

                PostingDate = Order.DateOut,
                Origin = docType.ID,
                OriginNo = Order.ReceiptNo,
                OffsetAccount = OffsetAcc,
                Details = douTypeID.Name + "-" + glAcc.Code,
                CumulativeBalance = glAcc.Balance,
                Credit = cardPay,
                LocalSetRate = (decimal)Order.LocalSetRate,
                GLAID = accountReceive.GLAccID,
                BPAcctID = Order.CustomerID,
                Creator = Order.UserOrderID
            });

            var accountSelected = _context.PaymentMeans.Find(incoming.PaymentMeanID);
            var glAccC = _context.GLAccounts.FirstOrDefault(w => w.ID == accountSelected.AccountID);
            journalEntryDetail.Add(new JournalEntryDetail
            {
                JEID = journalEntry.ID,
                Type = Type.GLAcct,
                ItemID = accountSelected.AccountID,
                Debit = totalpay,
                BPAcctID = incoming.CustomerID,
            });
            //Insert 
            glAccC.Balance += totalpay;
            accountBalance.Add(new AccountBalance
            {
                JEID = journalEntry.ID,

                PostingDate = incoming.PostingDate,
                Origin = docType.ID,
                OriginNo = Order.ReceiptNo,
                OffsetAccount = glAccC.Code,
                Details = douTypeID.Name + " - " + glAccC.Code,
                CumulativeBalance = glAccC.Balance,
                Debit = totalpay,
                LocalSetRate = (decimal)incoming.LocalSetRate,
                GLAID = accountSelected.AccountID,
            });
            _context.Update(glAccC);
            _context.Update(glAccD);
        }
        private void InsertFinancialPOS(
            int inventoryAccID, int COGSAccID, decimal inventoryAccAmount, decimal COGSAccAmount,
            List<JournalEntryDetail> journalEntryDetail, List<AccountBalance> accountBalance,
            JournalEntry journalEntry, DocumentType docType, DocumentType douTypeID, Receipt Order,
            string OffsetAcc, ReceiptDetail item)
        {
            if (!item.IsKsms && !item.IsKsmsMaster)
            {
                //inventoryAccID
                var glAccInvenfifo = _context.GLAccounts.FirstOrDefault(w => w.ID == inventoryAccID) ?? new GLAccount();
                if (glAccInvenfifo.ID > 0 && inventoryAccAmount>0)
                {
                    var journalDetail = journalEntryDetail.FirstOrDefault(w => w.ItemID == glAccInvenfifo.ID) ?? new JournalEntryDetail();
                    if (journalDetail.ItemID > 0)
                    {
                        var accBalance = accountBalance.FirstOrDefault(w => w.GLAID == inventoryAccID);
                        glAccInvenfifo.Balance -= inventoryAccAmount;
                        //journalEntryDetail
                        journalDetail.Credit += inventoryAccAmount;
                        //accountBalance
                        accBalance.CumulativeBalance = glAccInvenfifo.Balance;
                        accBalance.Credit += inventoryAccAmount;
                    }
                    else
                    {
                        glAccInvenfifo.Balance -= inventoryAccAmount;
                        journalEntryDetail.Add(new JournalEntryDetail
                        {
                            JEID = journalEntry.ID,
                            Type = Type.GLAcct,
                            ItemID = inventoryAccID,
                            Credit = inventoryAccAmount,
                        });
                        //
                        accountBalance.Add(new AccountBalance
                        {
                            JEID = journalEntry.ID,

                            PostingDate = Order.DateOut,
                            Origin = docType.ID,
                            OriginNo = Order.ReceiptNo,
                            OffsetAccount = OffsetAcc,
                            Details = douTypeID.Name + "-" + glAccInvenfifo.Code,
                            CumulativeBalance = glAccInvenfifo.Balance,
                            Credit = inventoryAccAmount,
                            LocalSetRate = (decimal)Order.LocalSetRate,
                            GLAID = inventoryAccID,
                        });
                    }
                    _context.Update(glAccInvenfifo);
                }
                //Add COGS
                var glAccCOGSfifo = _context.GLAccounts.FirstOrDefault(w => w.ID == COGSAccID) ?? new GLAccount();
                if (glAccCOGSfifo.ID > 0 && COGSAccAmount>0)
                {
                    var journalDetail = journalEntryDetail.FirstOrDefault(w => w.ItemID == glAccCOGSfifo.ID) ?? new JournalEntryDetail();
                    if (journalDetail.ItemID > 0)
                    {
                        var accBalance = accountBalance.FirstOrDefault(w => w.GLAID == COGSAccID);
                        glAccCOGSfifo.Balance += COGSAccAmount;
                        //journalEntryDetail
                        journalDetail.Debit += COGSAccAmount;
                        //accountBalance
                        accBalance.CumulativeBalance = glAccCOGSfifo.Balance;
                        accBalance.Debit += COGSAccAmount;
                    }
                    else
                    {
                        glAccCOGSfifo.Balance += COGSAccAmount;
                        journalEntryDetail.Add(new JournalEntryDetail
                        {
                            JEID = journalEntry.ID,
                            Type = Type.GLAcct,
                            ItemID = COGSAccID,
                            Debit = COGSAccAmount,
                        });
                        accountBalance.Add(new AccountBalance
                        {
                            JEID = journalEntry.ID,

                            PostingDate = Order.DateOut,
                            Origin = docType.ID,
                            OriginNo = Order.ReceiptNo,
                            OffsetAccount = OffsetAcc,
                            Details = douTypeID.Name + "-" + glAccCOGSfifo.Code,
                            CumulativeBalance = glAccCOGSfifo.Balance,
                            Debit = COGSAccAmount,
                            LocalSetRate = (decimal)Order.LocalSetRate,
                            GLAID = COGSAccID,
                        });
                    }
                    _context.Update(glAccCOGSfifo);
                }
            }
            _context.SaveChanges();
        }

        public async Task<int> InsertReceiptAsync(int orderid, List<SerialNumber> serials, List<BatchNo> batches, List<MultiPaymentMeans> multiPayments, PointTemplate point = null)
        {
            Receipt receipt = new();
            var order = await _context.Order.Include(w => w.OrderDetail).FirstOrDefaultAsync(w => w.OrderID == orderid);
            var disCurrency = await _context.DisplayCurrencies.Where(w => w.PriceListID == order.PriceListID
                        && w.IsShowCurrency == true && w.IsShowCurrency).ToListAsync();
            var currenciesDisplay = "";
            foreach (var item in disCurrency)
            {
                currenciesDisplay += item.AltCurrencyID + " " + item.DisplayRate + " " + item.DecimalPlaces + "|";
            }
            receipt.AmountFreight = order.FreightAmount;
            receipt.OrderID = order.OrderID;
            receipt.OrderNo = order.OrderNo;
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
            receipt.DiscountRate = order.DiscountRate;
            receipt.DiscountValue = order.DiscountValue;
            receipt.TypeDis = order.TypeDis;
            receipt.TaxOption = order.TaxOption;
            receipt.TaxRate = order.TaxRate;
            receipt.TaxValue = order.TaxValue;
            receipt.GrandTotal = order.GrandTotal;
            receipt.GrandTotal_Sys = order.GrandTotal_Sys;
            receipt.Tip = order.Tip;
            receipt.Received = order.Received;
            receipt.VehicleID = order.VehicleID;
            receipt.PromoCodeID = order.PromoCodeID;
            receipt.PromoCodeDiscValue = order.PromoCodeDiscValue;
            receipt.PromoCodeDiscRate = order.PromoCodeDiscRate;
            receipt.RemarkDiscount = _context.RemarkDiscounts.Find(order.RemarkDiscountID)?.Remark;
            receipt.BuyXAmountGetXDisID = order.BuyXAmountGetXDisID;
            receipt.BuyXAmGetXDisRate = order.BuyXAmGetXDisRate;
            receipt.BuyXAmGetXDisType = order.BuyXAmGetXDisType;
            receipt.BuyXAmGetXDisValue = order.BuyXAmGetXDisValue;
            receipt.TaxGroupID = order.TaxGroupID;
            receipt.GrandTotalCurrencies = order.GrandTotalCurrencies ?? new List<DisplayPayCurrencyModel>();
            receipt.DisplayPayOtherCurrency = order.DisplayPayOtherCurrency ?? new List<DisplayPayCurrencyModel>();
            receipt.ChangeCurrencies = order.ChangeCurrencies ?? new List<DisplayPayCurrencyModel>();
            receipt.GrandTotalOtherCurrencies = order.GrandTotalOtherCurrencies ?? new List<DisplayPayCurrencyModel>();
            receipt.OtherPaymentGrandTotal = order.OtherPaymentGrandTotal;
            receipt.OpenOtherPaymentGrandTotal = order.OtherPaymentGrandTotal;
            receipt.GrandTotalCurrenciesDisplay = order.GrandTotalCurrenciesDisplay;
            receipt.PaymentType = order.PaymentType;
            receipt.Male = order.Male;
            receipt.Female = order.Female;
            receipt.Children = order.Children;
            if (receipt.Received >= receipt.GrandTotal)
            {
                receipt.AppliedAmount = (decimal)receipt.GrandTotal;
                receipt.Status = StatusReceipt.Paid;
            }
            else
            {
                receipt.AppliedAmount = (decimal)receipt.Received;
                receipt.Status = StatusReceipt.Aging;
            }
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
            receipt.SeriesID = order.SeriesID;
            receipt.SeriesDID = order.SeriesDID;
            receipt.ChangeCurrenciesDisplay = order.ChangeCurrenciesDisplay;
            receipt.GrandTotalOtherCurrenciesDisplay = currenciesDisplay;
            receipt.CardMemberDiscountRate = order.CardMemberDiscountRate;
            receipt.CardMemberDiscountValue = order.CardMemberDiscountValue;
            receipt.BalanceReturn = receipt.AppliedAmount;
            receipt.BalanceToPay = (decimal)receipt.GrandTotal - receipt.AppliedAmount;
            await _context.AddAsync(receipt);
            await _context.SaveChangesAsync();
            var receiptid = receipt.ReceiptID;
            foreach (var item in order.OrderDetail)
            {
                ReceiptDetail receiptDetail = new();
                receiptDetail.ReceiptID = receiptid;
                receiptDetail.OrderDetailID = item.OrderDetailID;
                receiptDetail.OrderID = item.OrderID;
                receiptDetail.LineID = item.LineID;
                receiptDetail.ParentLineID = item.ParentLineID;
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
                receiptDetail.TaxGroupID = item.TaxGroupID;
                receiptDetail.TaxRate = item.TaxRate;
                receiptDetail.TaxValue = item.TaxValue;
                receiptDetail.Total = item.Total;
                receiptDetail.Total_Sys = item.Total_Sys;
                receiptDetail.UomID = item.UomID;
                receiptDetail.ItemStatus = item.ItemStatus;
                receiptDetail.ItemPrintTo = item.ItemPrintTo;
                receiptDetail.Currency = item.Currency;
                receiptDetail.ItemType = item.ItemType;
                receiptDetail.OpenQty = item.Qty;
                receiptDetail.PromoType = item.PromoType;
                receiptDetail.LinePosition = item.LinePosition;
                receiptDetail.KSServiceSetupId = item.KSServiceSetupId;
                receiptDetail.VehicleId = item.VehicleId;
                receiptDetail.IsKsms = item.IsKsms;
                receiptDetail.IsKsmsMaster = item.IsKsmsMaster;
                receiptDetail.IsScale = item.IsScale;
                receiptDetail.IsReadonly = item.IsReadonly;
                receiptDetail.ComboSaleType = item.ComboSaleType;
                receiptDetail.RemarkDiscountID = item.RemarkDiscountID;
                _context.Update(receiptDetail);
                await _context.SaveChangesAsync();
                //insert to KSService
                if (receiptDetail.IsKsmsMaster)
                {
                    var ksSetUpExisted = _context.KSServices.FirstOrDefault(i => i.KSServiceSetupId == receiptDetail.KSServiceSetupId
                                        && i.CusId == receipt.CustomerID);
                    if (ksSetUpExisted != null)
                    {
                        ksSetUpExisted.MaxCount += receiptDetail.Qty;
                        ksSetUpExisted.Qty += receiptDetail.Qty;
                    }
                    else
                    {
                        KSService ksmsService = new()
                        {
                            CusId = receipt.CustomerID,
                            ID = 0,
                            KSServiceSetupId = receiptDetail.KSServiceSetupId,
                            MaxCount = receiptDetail.Qty,
                            Qty = receiptDetail.Qty,
                            UsedCount = 0,
                            VehicleID = receiptDetail.VehicleId,
                            ReceiptDID = receiptDetail.ID,
                            ReceiptID = receipt.ReceiptID,
                            PriceListID = receipt.PriceListID,
                        };
                        _context.KSServices.Update(ksmsService);
                    }
                    _context.SaveChanges();
                }
            }

            //Freights of payment
            var freights = order.Freights ?? new List<FreightReceipt>();
            if (freights.Count > 0)
            {
                freights.ForEach(f =>
                {
                    f.ReceiptID = receipt.ReceiptID;
                });
                await _context.FreightReceipts.AddRangeAsync(order.Freights);
                await _context.SaveChangesAsync();
            }

            await AddMultiPaymentMeansAsync(receipt, multiPayments);
            await _loyaltyProgram.CountPointsReceiptAsync(receipt.CustomerID, point);
            await IssueStockAsync(receipt, order.Status, serials, batches, multiPayments);

            _context.OrderDetail.RemoveRange(order.OrderDetail);
            _context.Order.Remove(order);
            await _context.SaveChangesAsync();
            return receiptid;
        }

        private async Task AddMultiPaymentMeansAsync(Receipt receipt, List<MultiPaymentMeans> multiPayments)
        {
            foreach (var data in multiPayments)
            {
                data.OpenAmount = data.Amount;
            }
            MultiPaymentMeans obj = new MultiPaymentMeans();
            var paymentmean = _context.MultipayMeansSetting.FirstOrDefault(s => s.Changed == true) ?? new MultipayMeansSetting();

            //Save Multipay
            multiPayments.Select(s =>
            {
                s.ReceiptID = receipt.ReceiptID;
                return s;
            }).ToList();

            multiPayments = multiPayments.Where(x => x.Amount > 0).ToList();
            if (receipt.Change > 0)
            {
                var objcmp = multiPayments.FirstOrDefault(s => s.PaymentMeanID == paymentmean.PaymentID) ?? new MultiPaymentMeans();
                if (objcmp.PaymentMeanID == 0)
                {
                    var obj2 = multiPayments.FirstOrDefault();
                    obj.ID = 0;
                    obj.ReceiptID = obj2.ReceiptID;
                    obj.PaymentMeanID = paymentmean.PaymentID;
                    obj.AltCurrencyID = obj2.AltCurrencyID;
                    obj.AltCurrency = obj2.AltCurrency;
                    obj.AltRate = obj2.AltRate;
                    obj.PLCurrencyID = obj2.PLCurrencyID;
                    obj.PLCurrency = obj2.PLCurrency;
                    obj.PLRate = obj2.PLRate;
                    obj.Amount = (-1) * (decimal)receipt.Change;
                    obj.OpenAmount = (-1) * (decimal)receipt.Change;
                    obj.Total = (-1) * (decimal)receipt.Change;
                    obj.SCRate = obj2.SCRate;
                    obj.LCRate = obj2.LCRate;
                    multiPayments.Add(obj);
                }
                else
                {
                    foreach (var i in multiPayments)
                    {
                        if (i.PaymentMeanID == paymentmean.PaymentID)
                        {
                            i.Amount = i.Amount - (decimal)receipt.Change;
                            i.OpenAmount = i.OpenAmount - (decimal)receipt.Change;
                            i.Total = i.Total - (decimal)receipt.Change;
                            break;
                        }
                    }
                }
            }
            _context.AddRange(multiPayments);
            await _context.SaveChangesAsync();
        }

        public async Task IssueStockAsync(Receipt receipt, OrderStatus orderStaus, List<SerialNumber> serials,
            List<BatchNo> batches, List<MultiPaymentMeans> multiPayments)
        {
            var receitDetails = receipt.RececiptDetail ?? await _context.ReceiptDetail
                                .Where(rd => rd.ReceiptID == receipt.ReceiptID).ToListAsync();
            var series = await _context.Series.FindAsync(receipt.SeriesID) ?? new Series();
            var docTypeJE = await _context.DocumentTypes.FirstOrDefaultAsync(w => w.Code == "JE");
            var seriesByJE = await _context.Series.FirstOrDefaultAsync(w => w.Default
                            && w.DocuTypeID == docTypeJE.ID && w.NextNo != w.LastNo) ?? new Series();

            JournalEntry journalEntry = new();
            SeriesDetail seriesDetail = new();

            if (seriesByJE.ID > 0)
            {
                // update series
                string Sno = seriesByJE.NextNo;
                long No = long.Parse(Sno);
                seriesByJE.NextNo = Convert.ToString(No + 1);
                // update series details
                seriesDetail.SeriesID = journalEntry.SeriesID;
                seriesDetail.Number = Sno;
                _context.Update(seriesByJE);
                _context.Update(seriesDetail);
                _context.SaveChanges();
                // Insert Journal Entry
                journalEntry.SeriesID = seriesByJE.ID;
                journalEntry.Number = seriesByJE.PreFix + "-" + Sno;
                journalEntry.DouTypeID = seriesByJE.DocuTypeID;
                journalEntry.Creator = receipt.UserOrderID;
                journalEntry.TransNo = receipt.ReceiptNo;
                journalEntry.PostingDate = receipt.DateOut;
                journalEntry.DocumentDate = receipt.DateOut;
                journalEntry.DueDate = receipt.DateOut;
                journalEntry.SSCID = receipt.SysCurrencyID;
                journalEntry.LLCID = receipt.LocalCurrencyID;
                journalEntry.CompanyID = receipt.CompanyID;
                journalEntry.LocalSetRate = (decimal)receipt.LocalSetRate;
                journalEntry.SeriesDID = seriesDetail.ID;
                journalEntry.Remarks = series.Name + "-" + receipt.ReceiptNo;
                _context.Update(journalEntry);
                await _context.SaveChangesAsync();
            }

            await DebitAccountRecievableAsync(receipt, serials, batches, multiPayments, journalEntry, seriesDetail, series, orderStaus);
        }

        private async Task DebitAccountRecievableAsync(Receipt receipt,
            List<SerialNumber> serials, List<BatchNo> batches,
            List<MultiPaymentMeans> multiPayments, JournalEntry journalEntry,
            SeriesDetail seriesDetail, Series series, OrderStatus orderStatus)
        {
            List<JournalEntryDetail> journalEntryDetail = new();
            List<AccountBalance> accountBalance = new();
            List<ReceiptDetail> receiptDetails = receipt.RececiptDetail;
            IncomingPaymentCustomer incomingCus = new();
            IncomingPayment incoming = new();
            IncomingPaymentDetail incomingDetail = new();
            var company = await _context.Company.FindAsync(receipt.CompanyID);
            var warehouse = await _context.Warehouses.FindAsync(receipt.WarehouseID) ?? new Warehouse();
            var glAccts = await _context.GLAccounts.Where(i => i.IsActive).ToListAsync();
            var docTypeSP = await _context.DocumentTypes.FirstOrDefaultAsync(w => w.Code == "SP");
            var docTypeJE = await _context.DocumentTypes.FirstOrDefaultAsync(w => w.Code == "JE");

            var customer = await _context.BusinessPartners.FirstOrDefaultAsync(w => w.ID == receipt.CustomerID);
            var glAcc = _context.GLAccounts.FirstOrDefault(w => w.ID == customer.GLAccID) ?? new GLAccount();
            string offsetAcc = customer.Code + "-" + glAcc.Code;
            var cardPay = multiPayments.FirstOrDefault(x => x.Type == PaymentMeanType.CardMember);
            if (cardPay != null)
            {
                //Card Member
                var cardMember = _context.AccountMemberCards.FirstOrDefault(i => i.CashAccID > 0 && i.UnearnedRevenueID > 0) ?? new AccountMemberCard();
                var cashAcc = _context.GLAccounts.FirstOrDefault(w => w.ID == cardMember.UnearnedRevenueID);
                if (cashAcc.ID > 0)
                {
                    var cardjur = journalEntryDetail.FirstOrDefault(w => w.ItemID == cashAcc.ID) ?? new JournalEntryDetail();
                    if (cardjur.ItemID > 0)
                    {
                        var accBalance = accountBalance.FirstOrDefault(w => w.GLAID == cashAcc.ID);
                        cashAcc.Balance += cardPay.Amount * cardPay.SCRate;
                        //journalEntryDetail
                        cardjur.Debit += cardPay.Amount * cardPay.SCRate;
                        //accountBalance
                        accBalance.CumulativeBalance = cashAcc.Balance;
                        accBalance.Debit += cardPay.Amount * cardPay.SCRate;
                    }
                    else
                    {
                        cashAcc.Balance += cardPay.Amount * cardPay.SCRate;
                        journalEntryDetail.Add(new JournalEntryDetail
                        {
                            JEID = journalEntry.ID,
                            Type = Type.GLAcct,
                            ItemID = cashAcc.ID,
                            Debit = cardPay.Amount * cardPay.SCRate,
                        });
                        //
                        accountBalance.Add(new AccountBalance
                        {
                            JEID = journalEntry.ID,
                            PostingDate = receipt.DateIn,
                            Origin = docTypeSP.ID,
                            OriginNo = receipt.ReceiptNo,
                            OffsetAccount = cashAcc.Code,
                            Details = docTypeJE.Name + " - " + cashAcc.Code,
                            CumulativeBalance = cashAcc.Balance,
                            Debit = cardPay.Amount * cardPay.SCRate,
                            LocalSetRate = (decimal)receipt.LocalSetRate,
                            GLAID = cashAcc.ID,
                        });
                    }
                    _context.Update(cashAcc);
                }

                if (customer != null)
                {
                    customer.Balance -= receipt.OtherPaymentGrandTotal;
                    //CardMemberDepositTransactions
                    CardMemberDepositTransaction cmdt = new()
                    {
                        Amount = receipt.OtherPaymentGrandTotal * -1,
                        CardMemberDepositID = 0,
                        CardMemberID = customer.CardMemberID,
                        CumulativeBalance = customer.Balance,
                        CusID = customer.ID,
                        ID = 0,
                        DocTypeID = docTypeSP.ID,
                        Number = receipt.ReceiptNo,
                        PostingDate = receipt.DateIn,
                        SeriesDID = receipt.SeriesDID,
                        SeriesID = receipt.SeriesID,
                        UserID = receipt.UserOrderID,
                    };
                    _context.BusinessPartners.Update(customer);
                    _context.CardMemberDepositTransactions.Update(cmdt);
                    await _context.SaveChangesAsync();
                }
            }

            decimal grandTotalSys = (decimal)receipt.GrandTotal_Sys;
            if (glAcc.ID > 0 && grandTotalSys > 0)
            {
                journalEntryDetail.Add(new JournalEntryDetail
                {
                    JEID = journalEntry.ID,
                    Type = Type.BPCode,
                    ItemID = customer.GLAccID,
                    Debit = grandTotalSys,
                    BPAcctID = receipt.CustomerID,
                });
                //Insert             
                glAcc.Balance += grandTotalSys;
                accountBalance.Add(new AccountBalance
                {
                    JEID = journalEntry.ID,
                    PostingDate = receipt.DateOut,
                    Origin = docTypeSP.ID,
                    OriginNo = receipt.ReceiptNo,
                    OffsetAccount = offsetAcc,
                    Details = docTypeJE.Name + "-" + glAcc.Code,
                    CumulativeBalance = glAcc.Balance,
                    Debit = grandTotalSys,
                    LocalSetRate = (decimal)receipt.LocalSetRate,
                    GLAID = customer.GLAccID,
                    BPAcctID = receipt.CustomerID,
                    Creator = receipt.UserOrderID
                });
                _context.AccountBalances.UpdateRange(accountBalance);
                _context.GLAccounts.Update(glAcc);
            }

            // VAT Invoice
            var taxgInvoice = await _context.TaxGroups.FindAsync(receipt.TaxGroupID) ?? new TaxGroup();
            var taxAccInVoice = await _context.GLAccounts.FindAsync(taxgInvoice.GLID) ?? new GLAccount();
            decimal taxValueInvoice = (decimal)(receipt.TaxValue * receipt.ExchangeRate);
            if (taxAccInVoice.ID > 0 && taxValueInvoice > 0)
            {
                var taxjur = journalEntryDetail.FirstOrDefault(w => w.ItemID == taxAccInVoice.ID) ?? new JournalEntryDetail();
                if (taxjur.ItemID > 0)
                {
                    var accBalance = accountBalance.FirstOrDefault(w => w.GLAID == taxAccInVoice.ID);
                    taxAccInVoice.Balance -= taxValueInvoice;
                    //journalEntryDetail
                    taxjur.Credit += taxValueInvoice;
                    //accountBalance
                    accBalance.CumulativeBalance = taxAccInVoice.Balance;
                    accBalance.Credit += taxValueInvoice;
                }
                else
                {
                    taxAccInVoice.Balance -= taxValueInvoice;
                    journalEntryDetail.Add(new JournalEntryDetail
                    {
                        JEID = journalEntry.ID,
                        Type = Type.GLAcct,
                        ItemID = taxAccInVoice.ID,
                        Credit = taxValueInvoice,
                    });
                    //
                    accountBalance.Add(new AccountBalance
                    {
                        JEID = journalEntry.ID,
                        PostingDate = receipt.DateIn,
                        Origin = docTypeSP.ID,
                        OriginNo = receipt.ReceiptNo,
                        OffsetAccount = taxAccInVoice.Code,
                        Details = docTypeJE.Name + " - " + taxAccInVoice.Code,
                        CumulativeBalance = taxAccInVoice.Balance,
                        Credit = taxValueInvoice,
                        LocalSetRate = (decimal)receipt.LocalSetRate,
                        GLAID = taxAccInVoice.ID,
                    });
                }
                _context.Update(taxAccInVoice);
            }

            var refreight = await _context.FreightReceipts.Where(w => w.ReceiptID == receipt.ReceiptID).ToListAsync();
            foreach (var item in refreight)
            {
                if (item.AmountReven > 0)
                {
                    var freignt = _context.Freights.FirstOrDefault(w => w.ID == item.FreightID) ?? new Freight();
                    var frReven = _context.GLAccounts.FirstOrDefault(w => w.ID == freignt.RevenAcctID) ?? new GLAccount();
                    if (freignt.ID > 0)
                    {
                        var list = journalEntryDetail.FirstOrDefault(w => w.ItemID == frReven.ID) ?? new JournalEntryDetail();
                        if (list.ItemID > 0)
                        {
                            var accBalance = accountBalance.FirstOrDefault(w => w.GLAID == freignt.RevenAcctID);
                            frReven.Balance -= item.AmountReven * (decimal)receipt.ExchangeRate;
                            //journalEntryDetail
                            list.Credit += item.AmountReven * (decimal)receipt.ExchangeRate;
                            //accountBalance
                            accBalance.CumulativeBalance = frReven.Balance;
                            accBalance.Credit += item.AmountReven * (decimal)receipt.ExchangeRate;
                        }
                        else
                        {
                            frReven.Balance -= item.AmountReven * (decimal)receipt.ExchangeRate;
                            journalEntryDetail.Add(new JournalEntryDetail
                            {
                                JEID = journalEntry.ID,
                                Type = Type.GLAcct,
                                ItemID = freignt.RevenAcctID,
                                Credit = item.AmountReven * (decimal)receipt.ExchangeRate,
                            });
                            //
                            accountBalance.Add(new AccountBalance
                            {
                                JEID = journalEntry.ID,
                                PostingDate = receipt.DateOut,
                                Origin = docTypeSP.ID,
                                OriginNo = receipt.ReceiptNo,
                                OffsetAccount = offsetAcc,
                                Details = docTypeJE.Name + "-" + frReven.Code,
                                CumulativeBalance = frReven.Balance,
                                Credit = item.AmountReven * (decimal)receipt.ExchangeRate,
                                LocalSetRate = (decimal)receipt.LocalSetRate,
                                GLAID = freignt.RevenAcctID,
                            });
                        }
                        _context.Update(frReven);
                    }
                }
            }
            await _context.SaveChangesAsync();

            foreach (var item in receiptDetails)
            {
                int revenueAccID = 0, inventoryAccID = 0, COGSAccID = 0;
                decimal revenueAccAmount = 0, inventoryAccAmount = 0, COGSAccAmount = 0;
                var itemMaster = _context.ItemMasterDatas.FirstOrDefault(w => w.ID == item.ItemID && !w.Delete);
                decimal disvalue = (decimal)item.Total_Sys * ((decimal)(receipt.DiscountRate + receipt.PromoCodeDiscRate) + receipt.BuyXAmGetXDisRate + receipt.CardMemberDiscountRate) / 100;
                if (itemMaster.SetGlAccount == SetGlAccount.ItemLevel)
                {
                    var itemAccs = _context.ItemAccountings.Where(w => w.ItemID == item.ItemID && w.WarehouseID == receipt.WarehouseID);
                    var revenueAcc = (from ia in itemAccs
                                      join gl in glAccts on ia.RevenueAccount equals gl.Code
                                      select gl
                                         ).FirstOrDefault() ?? new GLAccount();
                    var inventoryAcc = (from ia in itemAccs
                                        join gl in glAccts on ia.InventoryAccount equals gl.Code
                                        select gl
                                         ).FirstOrDefault() ?? new GLAccount();
                    var COGSAcc = (from ia in itemAccs
                                   join gl in glAccts on ia.CostofGoodsSoldAccount equals gl.Code
                                   select gl
                                         ).FirstOrDefault() ?? new GLAccount();
                    revenueAccID = revenueAcc.ID;
                    inventoryAccID = inventoryAcc.ID;
                    COGSAccID = COGSAcc.ID;

                    if (receipt.TaxOption == TaxOptions.Exclude)
                    {
                        decimal total = (decimal)item.Total_Sys - item.TaxValue;
                        disvalue = total * ((decimal)(receipt.DiscountRate + receipt.PromoCodeDiscRate) + receipt.BuyXAmGetXDisRate + receipt.CardMemberDiscountRate) / 100;
                        revenueAccAmount = total - disvalue;
                    }
                    else
                    {
                        revenueAccAmount = (decimal)item.Total_Sys - disvalue - (item.TaxValue * (decimal)receipt.ExchangeRate);
                    }
                }
                else if (itemMaster.SetGlAccount == SetGlAccount.ItemGroup)
                {
                    var itemAccs = _context.ItemAccountings.Where(w => w.ItemGroupID == itemMaster.ItemGroup1ID);
                    var revenueAcc = (from ia in itemAccs
                                      join gl in glAccts on ia.RevenueAccount equals gl.Code
                                      select gl
                                         ).FirstOrDefault() ?? new GLAccount();
                    var inventoryAcc = (from ia in itemAccs
                                        join gl in glAccts on ia.InventoryAccount equals gl.Code
                                        select gl
                                         ).FirstOrDefault() ?? new GLAccount();
                    var COGSAcc = (from ia in itemAccs
                                   join gl in glAccts on ia.CostofGoodsSoldAccount equals gl.Code
                                   select gl
                                         ).FirstOrDefault() ?? new GLAccount();
                    revenueAccID = revenueAcc.ID;
                    inventoryAccID = inventoryAcc.ID;
                    COGSAccID = COGSAcc.ID;
                    if (receipt.TaxOption == TaxOptions.Exclude)
                    {
                        decimal total = (decimal)item.Total_Sys - item.TaxValue;
                        disvalue = total * ((decimal)(receipt.DiscountRate + receipt.PromoCodeDiscRate) + receipt.BuyXAmGetXDisRate + receipt.CardMemberDiscountRate) / 100;
                        revenueAccAmount = total - disvalue;
                    }
                    else
                    {
                        revenueAccAmount = (decimal)item.Total_Sys - disvalue - (item.TaxValue * (decimal)receipt.ExchangeRate);
                    }
                }

                var orft = _context.GroupDUoMs.FirstOrDefault(w => w.GroupUoMID == itemMaster.GroupUomID && w.AltUOM == item.UomID);

                if (!item.IsKsms && !item.IsKsmsMaster)
                {
                    if (itemMaster.Process != "Standard")
                    {
                        double @Check_Stock;
                        double @Remain;
                        double @IssusQty;
                        double @FIFOQty;
                        double @Qty = item.Qty * orft.Factor;
                        double Cost = 0;
                        var _itemAcc = _context.ItemAccountings.FirstOrDefault(i => i.WarehouseID == receipt.WarehouseID && i.ItemID == item.ItemID);
                        var item_warehouse_summary = _context.WarehouseSummary.FirstOrDefault(w => w.WarehouseID == receipt.WarehouseID && w.ItemID == item.ItemID);
                        var wareDetails = _context.WarehouseDetails.Where(w => w.WarehouseID == receipt.WarehouseID && w.ItemID == item.ItemID).ToList();
                        if (item_warehouse_summary != null)
                        {
                            if (orderStatus == OrderStatus.Order)
                            {
                                //changed 21-09-2021
                                item_warehouse_summary.Committed -= @Qty - item.PrintQty * orft.Factor;
                                itemMaster.StockCommit -= @Qty;
                                _context.ItemMasterDatas.Update(itemMaster);
                            }
                            //WerehouseSummary
                            item_warehouse_summary.InStock -= @Qty;
                            //Itemmasterdata
                            //item_master_data.StockIn = item_warehouse_summary.InStock - (double)item.Qty;
                            _context.WarehouseSummary.Update(item_warehouse_summary);
                            _utility.UpdateItemAccounting(_itemAcc, item_warehouse_summary);
                            _context.SaveChanges();
                        }

                        //Checking Serial Batch //
                        if (itemMaster.ManItemBy == ManageItemBy.SerialNumbers)
                        {
                            if (serials.Count > 0)
                            {
                                List<WareForAudiView> wareForAudis = new();

                                foreach (var s in serials)
                                {
                                    if (s.SerialNumberSelected != null)
                                    {
                                        foreach (var ss in s.SerialNumberSelected.SerialNumberSelectedDetails)
                                        {
                                            var waredetial = wareDetails.FirstOrDefault(i => ss.SerialNumber == i.SerialNumber && i.InStock > 0);
                                            decimal _inventoryAccAmount = 0M;
                                            decimal _COGSAccAmount = 0M;
                                            if (waredetial != null)
                                            {
                                                Cost = waredetial.Cost;
                                                wareForAudis.Add(new WareForAudiView
                                                {
                                                    Cost = waredetial.Cost,
                                                    Qty = waredetial.InStock,
                                                    ExpireDate = waredetial.ExpireDate,
                                                });
                                                waredetial.InStock -= 1;
                                                // insert to warehouse detail
                                                var stockOut = new StockOut
                                                {
                                                    AdmissionDate = waredetial.AdmissionDate,
                                                    Cost = (decimal)waredetial.Cost,
                                                    CurrencyID = waredetial.CurrencyID,
                                                    Details = waredetial.Details,
                                                    ID = 0,
                                                    InStock = 1,
                                                    ItemID = waredetial.ItemID,
                                                    Location = waredetial.Location,
                                                    LotNumber = waredetial.LotNumber,
                                                    MfrDate = waredetial.MfrDate,
                                                    MfrSerialNumber = waredetial.MfrSerialNumber,
                                                    MfrWarDateEnd = waredetial.MfrWarDateEnd,
                                                    MfrWarDateStart = waredetial.MfrWarDateStart,
                                                    ProcessItem = ProcessItem.SEBA,
                                                    SerialNumber = waredetial.SerialNumber,
                                                    SyetemDate = DateTime.Now,
                                                    SysNum = 0,
                                                    TimeIn = DateTime.Now,
                                                    WarehouseID = waredetial.WarehouseID,
                                                    UomID = item.UomID,
                                                    UserID = receipt.UserOrderID,
                                                    ExpireDate = waredetial.ExpireDate,
                                                    TransType = TransTypeWD.POS,
                                                    TransID = receipt.ReceiptID,
                                                    Contract = itemMaster.ContractID,
                                                    OutStockFrom = receipt.ReceiptID,
                                                    FromWareDetialID = waredetial.ID,
                                                    BPID = receipt.CustomerID
                                                };
                                                _inventoryAccAmount = (decimal)waredetial.Cost;
                                                _COGSAccAmount = (decimal)waredetial.Cost;
                                                _context.StockOuts.Add(stockOut);
                                                _context.SaveChanges();
                                            }
                                            InsertFinancialPOS(
                                                inventoryAccID, COGSAccID, _inventoryAccAmount, _COGSAccAmount, journalEntryDetail,
                                                accountBalance, journalEntry, docTypeSP, docTypeJE, receipt, offsetAcc, item
                                            );
                                        }
                                    }
                                }
                                wareForAudis = (from wa in wareForAudis
                                                group wa by wa.Cost into g
                                                let wha = g.FirstOrDefault()
                                                select new WareForAudiView
                                                {
                                                    Qty = g.Sum(i => i.Qty),
                                                    Cost = wha.Cost,
                                                    ExpireDate = wha.ExpireDate,
                                                }).ToList();
                                if (wareForAudis.Any())
                                {
                                    foreach (var i in wareForAudis)
                                    {
                                        // Insert to Inventory Audit
                                        var inventory_audit = _context.InventoryAudits
                                            .Where(w => w.ItemID == item.ItemID && w.WarehouseID == receipt.WarehouseID && w.Cost == i.Cost).ToList();
                                        var inventory = new InventoryAudit
                                        {
                                            ID = 0,
                                            WarehouseID = receipt.WarehouseID,
                                            BranchID = receipt.BranchID,
                                            UserID = receipt.UserOrderID,
                                            ItemID = item.ItemID,
                                            CurrencyID = receipt.SysCurrencyID,
                                            UomID = orft.BaseUOM,
                                            InvoiceNo = receipt.ReceiptNo,
                                            Trans_Type = docTypeSP.Code,
                                            Process = itemMaster.Process,
                                            SystemDate = DateTime.Now,
                                            TimeIn = DateTime.Now.ToShortTimeString().ToString(),
                                            Qty = i.Qty * -1,
                                            Cost = i.Cost,
                                            Price = 0,
                                            CumulativeQty = inventory_audit.Sum(q => q.Qty) - i.Qty,
                                            CumulativeValue = inventory_audit.Sum(s => s.Trans_Valuse) - (i.Qty * i.Cost),
                                            Trans_Valuse = i.Qty * i.Cost * -1,
                                            ExpireDate = i.ExpireDate,
                                            LocalCurID = receipt.LocalCurrencyID,
                                            LocalSetRate = receipt.LocalSetRate,
                                            CompanyID = receipt.CompanyID,
                                            DocumentTypeID = docTypeSP.ID,
                                            SeriesID = receipt.SeriesID,
                                            SeriesDetailID = receipt.SeriesDID
                                        };
                                        _utility.CumulativeValue(inventory.WarehouseID, inventory.ItemID, inventory.CumulativeValue, _itemAcc);
                                        _context.InventoryAudits.Add(inventory);
                                        _context.SaveChanges();
                                    }
                                }
                            }
                        }
                        else if (itemMaster.ManItemBy == ManageItemBy.Batches)
                        {
                            if (batches.Count > 0)
                            {
                                List<WareForAudiView> wareForAudis = new();
                                foreach (var b in batches)
                                {
                                    if (b.BatchNoSelected != null)
                                    {
                                        foreach (var sb in b.BatchNoSelected.BatchNoSelectedDetails)
                                        {
                                            decimal selectedQty = sb.SelectedQty * (decimal)orft.Factor;
                                            var waredetial = wareDetails.FirstOrDefault(i => sb.BatchNo == i.BatchNo && i.InStock > 0);
                                            decimal _inventoryAccAmount = 0M;
                                            decimal _COGSAccAmount = 0M;
                                            if (waredetial != null)
                                            {
                                                wareForAudis.Add(new WareForAudiView
                                                {
                                                    Cost = waredetial.Cost,
                                                    Qty = (double)selectedQty,
                                                    ExpireDate = waredetial.ExpireDate,
                                                });
                                                waredetial.InStock -= (double)selectedQty;
                                                Cost = waredetial.Cost;
                                                // insert to waredetial
                                                var stockOut = new StockOut
                                                {
                                                    AdmissionDate = waredetial.AdmissionDate,
                                                    Cost = (decimal)waredetial.Cost,
                                                    CurrencyID = waredetial.CurrencyID,
                                                    Details = waredetial.Details,
                                                    ID = 0,
                                                    InStock = selectedQty,
                                                    ItemID = item.ItemID,
                                                    Location = waredetial.Location,
                                                    MfrDate = waredetial.MfrDate,
                                                    ProcessItem = ProcessItem.SEBA,
                                                    SyetemDate = DateTime.Now,
                                                    SysNum = 0,
                                                    TimeIn = DateTime.Now,
                                                    WarehouseID = waredetial.WarehouseID,
                                                    UomID = item.UomID,
                                                    UserID = receipt.UserOrderID,
                                                    ExpireDate = waredetial.ExpireDate,
                                                    BatchAttr1 = waredetial.BatchAttr1,
                                                    BatchAttr2 = waredetial.BatchAttr2,
                                                    BatchNo = waredetial.BatchNo,
                                                    TransType = TransTypeWD.POS,
                                                    TransID = receipt.ReceiptID,
                                                    OutStockFrom = receipt.ReceiptID,
                                                    FromWareDetialID = waredetial.ID,
                                                    BPID = receipt.CustomerID
                                                };
                                                _inventoryAccAmount = (decimal)waredetial.Cost * selectedQty;
                                                _COGSAccAmount = (decimal)waredetial.Cost * selectedQty;
                                                _context.StockOuts.Add(stockOut);
                                                _context.SaveChanges();
                                            }
                                            InsertFinancialPOS(
                                                inventoryAccID, COGSAccID, _inventoryAccAmount, _COGSAccAmount, journalEntryDetail,
                                                accountBalance, journalEntry, docTypeSP, docTypeJE, receipt, offsetAcc, item
                                            );
                                        }
                                    }
                                }
                                wareForAudis = (from wa in wareForAudis
                                                group wa by wa.Cost into g
                                                let wha = g.FirstOrDefault()
                                                select new WareForAudiView
                                                {
                                                    Qty = g.Sum(i => i.Qty),
                                                    Cost = wha.Cost,
                                                    ExpireDate = wha.ExpireDate
                                                }).ToList();

                                if (wareForAudis.Any())
                                {
                                    foreach (var i in wareForAudis)
                                    {
                                        // insert to inventory audit
                                        var inventory_audit = _context.InventoryAudits
                                            .Where(w => w.ItemID == item.ItemID && w.WarehouseID == receipt.WarehouseID && w.Cost == i.Cost).ToList();
                                        var inventory = new InventoryAudit
                                        {
                                            ID = 0,
                                            WarehouseID = receipt.WarehouseID,
                                            BranchID = receipt.BranchID,
                                            UserID = receipt.UserOrderID,
                                            ItemID = item.ItemID,
                                            CurrencyID = receipt.SysCurrencyID,
                                            UomID = orft.BaseUOM,
                                            InvoiceNo = receipt.ReceiptNo,
                                            Trans_Type = docTypeSP.Code,
                                            Process = itemMaster.Process,
                                            SystemDate = DateTime.Now,
                                            TimeIn = DateTime.Now.ToShortTimeString().ToString(),
                                            Qty = i.Qty * -1,
                                            Cost = i.Cost,
                                            Price = 0,
                                            CumulativeQty = inventory_audit.Sum(q => q.Qty) - i.Qty,
                                            CumulativeValue = inventory_audit.Sum(s => s.Trans_Valuse) - (i.Qty * i.Cost),
                                            Trans_Valuse = i.Qty * i.Cost * -1,
                                            ExpireDate = i.ExpireDate,
                                            LocalCurID = receipt.LocalCurrencyID,
                                            LocalSetRate = receipt.LocalSetRate,
                                            CompanyID = receipt.CompanyID,
                                            DocumentTypeID = docTypeSP.ID,
                                            SeriesID = receipt.SeriesID,
                                            SeriesDetailID = receipt.SeriesDID,
                                        };
                                        _utility.CumulativeValue(inventory.WarehouseID, inventory.ItemID, inventory.CumulativeValue, _itemAcc);
                                        _context.InventoryAudits.Add(inventory);
                                        _context.SaveChanges();
                                    }
                                }
                            }
                        }
                        else
                        {
                            List<WarehouseDetail> _whlists = wareDetails.Where(w => w.InStock > 0).OrderBy(i => i.SyetemDate).ToList();
                            if (warehouse.IsAllowNegativeStock && _whlists.Count == 0)
                            {
                                var wh = wareDetails.LastOrDefault();
                                _whlists.Add(wh);
                            }
                            foreach (var (item_warehouse, index) in _whlists.Select((value, i) => (value, i)))
                            {
                                InventoryAudit item_inventory_audit = new();
                                WarehouseDetail item_IssusStock = new();
                                @Check_Stock = item_warehouse.InStock - @Qty;
                                if (@Check_Stock < 0)
                                {
                                    @Remain = @Check_Stock * (-1);
                                    @IssusQty = @Qty - @Remain;
                                    if (@Remain <= 0)
                                    {
                                        @Qty = 0;
                                    }
                                    else if (@Qty > 0 && index == _whlists.Count - 1 && warehouse.IsAllowNegativeStock)
                                    {
                                        @IssusQty = @Qty;
                                    }
                                    else
                                    {
                                        @Qty = @Remain;
                                    }

                                    if (itemMaster.Process == "FIFO")
                                    {
                                        item_IssusStock = item_warehouse;
                                        double _cost = item_IssusStock.Cost;
                                        item_IssusStock.InStock -= @IssusQty;
                                        if (@IssusQty > 0)
                                        {
                                            var stockOuts = new StockOut
                                            {
                                                Cost = (decimal)item_warehouse.Cost,
                                                CurrencyID = item_warehouse.CurrencyID,
                                                ID = 0,
                                                InStock = (decimal)@IssusQty,
                                                ItemID = item.ItemID,
                                                ProcessItem = ProcessItem.FIFO,
                                                SyetemDate = DateTime.Now,
                                                TimeIn = DateTime.Now,
                                                WarehouseID = item_warehouse.WarehouseID,
                                                UomID = item.UomID,
                                                UserID = receipt.UserOrderID,
                                                ExpireDate = item_IssusStock.ExpireDate,
                                                TransType = TransTypeWD.POS,
                                                TransID = receipt.ReceiptID,
                                                OutStockFrom = receipt.ReceiptID,
                                                BPID = receipt.CustomerID,
                                                FromWareDetialID = item_IssusStock.ID,
                                            };
                                            _context.StockOuts.Add(stockOuts);
                                            var inventory_audit = _context.InventoryAudits.Where(w => w.ItemID == item.ItemID && w.WarehouseID == receipt.WarehouseID).ToList();
                                            item_inventory_audit.ID = 0;
                                            item_inventory_audit.WarehouseID = receipt.WarehouseID;
                                            item_inventory_audit.BranchID = receipt.BranchID;
                                            item_inventory_audit.UserID = receipt.UserOrderID;
                                            item_inventory_audit.ItemID = item.ItemID;
                                            item_inventory_audit.CurrencyID = company.SystemCurrencyID;
                                            item_inventory_audit.UomID = orft.BaseUOM;
                                            item_inventory_audit.InvoiceNo = receipt.ReceiptNo;
                                            item_inventory_audit.Trans_Type = docTypeSP.Code;
                                            item_inventory_audit.Process = itemMaster.Process;
                                            item_inventory_audit.SystemDate = DateTime.Now;
                                            item_inventory_audit.TimeIn = DateTime.Now.ToShortTimeString().ToString();
                                            item_inventory_audit.Qty = @IssusQty * -1;
                                            item_inventory_audit.Cost = _cost;
                                            item_inventory_audit.Price = 0;
                                            item_inventory_audit.CumulativeQty = inventory_audit.Sum(q => q.Qty) - @IssusQty;
                                            item_inventory_audit.CumulativeValue = inventory_audit.Sum(s => s.Trans_Valuse) - (@IssusQty * _cost);
                                            item_inventory_audit.Trans_Valuse = @IssusQty * item_IssusStock.Cost * -1;
                                            item_inventory_audit.ExpireDate = item_IssusStock.ExpireDate;
                                            item_inventory_audit.LocalCurID = receipt.LocalCurrencyID;
                                            item_inventory_audit.LocalSetRate = receipt.LocalSetRate;
                                            item_inventory_audit.CompanyID = receipt.CompanyID;
                                            item_inventory_audit.DocumentTypeID = docTypeSP.ID;
                                            item_inventory_audit.SeriesID = receipt.SeriesID;
                                            item_inventory_audit.SeriesDetailID = receipt.SeriesDID;
                                        }
                                        inventoryAccAmount += (decimal)item_inventory_audit.Cost * (decimal)@IssusQty;
                                        COGSAccAmount += (decimal)item_inventory_audit.Cost * (decimal)@IssusQty;
                                        _utility.CumulativeValue(item_inventory_audit.WarehouseID, item_inventory_audit.ItemID, item_inventory_audit.CumulativeValue, _itemAcc);
                                        _context.WarehouseDetails.Update(item_IssusStock);
                                        _context.InventoryAudits.Add(item_inventory_audit);
                                        _context.SaveChanges();
                                    }
                                    else if (itemMaster.Process == "Average")
                                    {
                                        item_IssusStock = wareDetails.OrderByDescending(i => i.SyetemDate).FirstOrDefault();
                                        item_IssusStock.InStock -= @IssusQty;
                                        if (@IssusQty > 0)
                                        {
                                            var warehouse_summary = _context.WarehouseSummary.FirstOrDefault(w => w.ItemID == item.ItemID && w.WarehouseID == receipt.WarehouseID);
                                            double @sysAvCost = warehouse_summary.Cost;
                                            var inventory_audit = _context.InventoryAudits.Where(w => w.ItemID == item.ItemID && w.WarehouseID == receipt.WarehouseID).ToList();
                                            var stockOuts = new StockOut
                                            {
                                                Cost = (decimal)@sysAvCost,
                                                CurrencyID = item_warehouse.CurrencyID,
                                                ID = 0,
                                                InStock = (decimal)@IssusQty,
                                                ItemID = item.ItemID,
                                                ProcessItem = ProcessItem.Average,
                                                SyetemDate = DateTime.Now,
                                                TimeIn = DateTime.Now,
                                                WarehouseID = item_warehouse.WarehouseID,
                                                UomID = item.UomID,
                                                UserID = receipt.UserOrderID,
                                                ExpireDate = item_IssusStock.ExpireDate,
                                                TransType = TransTypeWD.POS,
                                                TransID = receipt.ReceiptID,
                                                OutStockFrom = receipt.ReceiptID,
                                                BPID = receipt.CustomerID,
                                                FromWareDetialID = item_IssusStock.ID,
                                            };
                                            _context.StockOuts.Add(stockOuts);

                                            item_inventory_audit.ID = 0;
                                            item_inventory_audit.WarehouseID = receipt.WarehouseID;
                                            item_inventory_audit.BranchID = receipt.BranchID;
                                            item_inventory_audit.UserID = receipt.UserOrderID;
                                            item_inventory_audit.ItemID = item.ItemID;
                                            item_inventory_audit.CurrencyID = company.SystemCurrencyID;
                                            item_inventory_audit.UomID = orft.BaseUOM;
                                            item_inventory_audit.InvoiceNo = receipt.ReceiptNo;
                                            item_inventory_audit.Trans_Type = docTypeSP.Code;
                                            item_inventory_audit.Process = itemMaster.Process;
                                            item_inventory_audit.SystemDate = DateTime.Now;
                                            item_inventory_audit.TimeIn = DateTime.Now.ToShortTimeString().ToString();
                                            item_inventory_audit.Qty = @IssusQty * -1;
                                            item_inventory_audit.Cost = @sysAvCost;
                                            item_inventory_audit.Price = 0;
                                            item_inventory_audit.CumulativeQty = inventory_audit.Sum(q => q.Qty) - @IssusQty;
                                            item_inventory_audit.CumulativeValue = inventory_audit.Sum(s => s.Trans_Valuse) - (@IssusQty * @sysAvCost);
                                            item_inventory_audit.Trans_Valuse = @IssusQty * @sysAvCost * -1;
                                            item_inventory_audit.ExpireDate = item_IssusStock.ExpireDate;
                                            item_inventory_audit.LocalCurID = receipt.LocalCurrencyID;
                                            item_inventory_audit.LocalSetRate = receipt.LocalSetRate;
                                            item_inventory_audit.CompanyID = receipt.CompanyID;
                                            item_inventory_audit.DocumentTypeID = docTypeSP.ID;
                                            item_inventory_audit.SeriesID = receipt.SeriesID;
                                            item_inventory_audit.SeriesDetailID = receipt.SeriesDID;
                                        }
                                        double @AvgCost = _utility.CalAVGCost(item.ItemID, receipt.WarehouseID, item_inventory_audit);
                                        inventoryAccAmount += (decimal)(@AvgCost * @IssusQty);
                                        COGSAccAmount += (decimal)(@AvgCost * @IssusQty);
                                        _utility.UpdateAvgCost(item_warehouse.ItemID, receipt.WarehouseID, itemMaster.GroupUomID, item_inventory_audit.Qty, item_inventory_audit.Cost);
                                        _utility.UpdateBomCost(item_warehouse.ItemID, item_inventory_audit.Qty, item_inventory_audit.Cost);
                                        _utility.CumulativeValue(item_inventory_audit.WarehouseID, item_inventory_audit.ItemID, item_inventory_audit.CumulativeValue, _itemAcc);
                                        _context.WarehouseDetails.Update(item_IssusStock);
                                        _context.InventoryAudits.Add(item_inventory_audit);
                                        await _context.SaveChangesAsync();
                                    }
                                }
                                else
                                {

                                    if (itemMaster.Process == "FIFO")
                                    {
                                        item_IssusStock = item_warehouse;
                                        @FIFOQty = item_IssusStock.InStock - @Qty;
                                        @IssusQty = item_IssusStock.InStock - @FIFOQty;
                                        item_IssusStock.InStock = @FIFOQty;
                                        if (@IssusQty > 0)
                                        {
                                            var stockOuts = new StockOut
                                            {
                                                Cost = (decimal)item_warehouse.Cost,
                                                CurrencyID = item_warehouse.CurrencyID,
                                                ID = 0,
                                                InStock = (decimal)@IssusQty,
                                                ItemID = item.ItemID,
                                                ProcessItem = ProcessItem.FIFO,
                                                SyetemDate = DateTime.Now,
                                                TimeIn = DateTime.Now,
                                                WarehouseID = item_warehouse.WarehouseID,
                                                UomID = item.UomID,
                                                UserID = receipt.UserOrderID,
                                                ExpireDate = item_IssusStock.ExpireDate,
                                                TransType = TransTypeWD.POS,
                                                TransID = receipt.ReceiptID,
                                                OutStockFrom = receipt.ReceiptID,
                                                BPID = receipt.CustomerID,
                                                FromWareDetialID = item_IssusStock.ID,
                                            };
                                            _context.StockOuts.Add(stockOuts);
                                            var inventory_audit = _context.InventoryAudits.Where(w => w.ItemID == item.ItemID && w.WarehouseID == receipt.WarehouseID).ToList();
                                            item_inventory_audit.ID = 0;
                                            item_inventory_audit.WarehouseID = receipt.WarehouseID;
                                            item_inventory_audit.BranchID = receipt.BranchID;
                                            item_inventory_audit.UserID = receipt.UserOrderID;
                                            item_inventory_audit.ItemID = item.ItemID;
                                            item_inventory_audit.CurrencyID = company.SystemCurrencyID;
                                            item_inventory_audit.UomID = orft.BaseUOM;
                                            item_inventory_audit.InvoiceNo = receipt.ReceiptNo;
                                            item_inventory_audit.Trans_Type = docTypeSP.Code;
                                            item_inventory_audit.Process = itemMaster.Process;
                                            item_inventory_audit.SystemDate = DateTime.Now;
                                            item_inventory_audit.TimeIn = DateTime.Now.ToShortTimeString().ToString();
                                            item_inventory_audit.Qty = @IssusQty * -1;
                                            item_inventory_audit.Cost = item_IssusStock.Cost;
                                            item_inventory_audit.Price = 0;
                                            item_inventory_audit.CumulativeQty = inventory_audit.Sum(q => q.Qty) - @IssusQty;
                                            item_inventory_audit.CumulativeValue = inventory_audit.Sum(s => s.Trans_Valuse) - (@IssusQty * item_IssusStock.Cost);
                                            item_inventory_audit.Trans_Valuse = @IssusQty * item_IssusStock.Cost * -1;
                                            item_inventory_audit.ExpireDate = item_IssusStock.ExpireDate;
                                            item_inventory_audit.LocalCurID = receipt.LocalCurrencyID;
                                            item_inventory_audit.LocalSetRate = receipt.LocalSetRate;
                                            item_inventory_audit.CompanyID = receipt.CompanyID;
                                            item_inventory_audit.DocumentTypeID = docTypeSP.ID;
                                            item_inventory_audit.SeriesID = receipt.SeriesID;
                                            item_inventory_audit.SeriesDetailID = receipt.SeriesDID;
                                        }
                                        inventoryAccAmount += (decimal)item_inventory_audit.Cost * (decimal)@IssusQty;
                                        COGSAccAmount += (decimal)item_inventory_audit.Cost * (decimal)@IssusQty;
                                        _utility.CumulativeValue(item_inventory_audit.WarehouseID, item_inventory_audit.ItemID, item_inventory_audit.CumulativeValue, _itemAcc);
                                        _context.WarehouseDetails.Update(item_IssusStock);
                                        _context.InventoryAudits.Add(item_inventory_audit);
                                        _context.SaveChanges();
                                    }
                                    else if (itemMaster.Process == "Average")
                                    {
                                        item_IssusStock = wareDetails.OrderByDescending(i => i.SyetemDate).FirstOrDefault();
                                        @FIFOQty = item_IssusStock.InStock - @Qty;
                                        @IssusQty = item_IssusStock.InStock - @FIFOQty;
                                        item_IssusStock.InStock = @FIFOQty;
                                        if (@IssusQty > 0)
                                        {
                                            var warehouse_summary = _context.WarehouseSummary.FirstOrDefault(w => w.ItemID == item.ItemID && w.WarehouseID == receipt.WarehouseID);
                                            double @sysAvCost = warehouse_summary.Cost;
                                            var inventory_audit = _context.InventoryAudits.Where(w => w.ItemID == item.ItemID && w.WarehouseID == receipt.WarehouseID).ToList();

                                            var stockOuts = new StockOut
                                            {
                                                Cost = (decimal)@sysAvCost,
                                                CurrencyID = item_warehouse.CurrencyID,
                                                ID = 0,
                                                InStock = (decimal)@IssusQty,
                                                ItemID = item.ItemID,
                                                ProcessItem = ProcessItem.Average,
                                                SyetemDate = DateTime.Now,
                                                TimeIn = DateTime.Now,
                                                WarehouseID = item_warehouse.WarehouseID,
                                                UomID = item.UomID,
                                                UserID = receipt.UserOrderID,
                                                ExpireDate = item_IssusStock.ExpireDate,
                                                TransType = TransTypeWD.POS,
                                                TransID = receipt.ReceiptID,
                                                OutStockFrom = receipt.ReceiptID,
                                                FromWareDetialID = item_IssusStock.ID,
                                                BPID = receipt.CustomerID
                                            };
                                            _context.StockOuts.Add(stockOuts);
                                            item_inventory_audit.ID = 0;
                                            item_inventory_audit.WarehouseID = receipt.WarehouseID;
                                            item_inventory_audit.BranchID = receipt.BranchID;
                                            item_inventory_audit.UserID = receipt.UserOrderID;
                                            item_inventory_audit.ItemID = item.ItemID;
                                            item_inventory_audit.CurrencyID = company.SystemCurrencyID;
                                            item_inventory_audit.UomID = orft.BaseUOM;
                                            item_inventory_audit.InvoiceNo = receipt.ReceiptNo;
                                            item_inventory_audit.Trans_Type = docTypeSP.Code;
                                            item_inventory_audit.Process = itemMaster.Process;
                                            item_inventory_audit.SystemDate = DateTime.Now;
                                            item_inventory_audit.TimeIn = DateTime.Now.ToShortTimeString().ToString();
                                            item_inventory_audit.Qty = @IssusQty * -1;
                                            item_inventory_audit.Cost = @sysAvCost;
                                            item_inventory_audit.Price = 0;
                                            item_inventory_audit.CumulativeQty = inventory_audit.Sum(q => q.Qty) - @IssusQty;
                                            item_inventory_audit.CumulativeValue = inventory_audit.Sum(s => s.Trans_Valuse) - (@IssusQty * @sysAvCost);
                                            item_inventory_audit.Trans_Valuse = @IssusQty * @sysAvCost * -1;
                                            item_inventory_audit.ExpireDate = item_IssusStock.ExpireDate;
                                            item_inventory_audit.LocalCurID = receipt.LocalCurrencyID;
                                            item_inventory_audit.LocalSetRate = receipt.LocalSetRate;
                                            item_inventory_audit.CompanyID = receipt.CompanyID;
                                            item_inventory_audit.DocumentTypeID = docTypeSP.ID;
                                            item_inventory_audit.SeriesID = receipt.SeriesID;
                                            item_inventory_audit.SeriesDetailID = receipt.SeriesDID;
                                        }
                                        double @AvgCost = _utility.CalAVGCost(item.ItemID, receipt.WarehouseID, item_inventory_audit);
                                        inventoryAccAmount += (decimal)(@AvgCost * @IssusQty);
                                        COGSAccAmount += (decimal)(@AvgCost * @IssusQty);
                                        _utility.UpdateAvgCost(item_warehouse.ItemID, receipt.WarehouseID, itemMaster.GroupUomID, item_inventory_audit.Qty, item_inventory_audit.Cost);
                                        _utility.UpdateBomCost(item_warehouse.ItemID, item_inventory_audit.Qty, item_inventory_audit.Cost);
                                        _utility.CumulativeValue(item_inventory_audit.WarehouseID, item_inventory_audit.ItemID, item_inventory_audit.CumulativeValue, _itemAcc);
                                        _context.WarehouseDetails.Update(item_IssusStock);
                                        _context.InventoryAudits.Add(item_inventory_audit);
                                        _context.SaveChanges();
                                    }
                                    wareDetails = new List<WarehouseDetail>();
                                    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        var priceListDetail = await _context.PriceListDetails.FirstOrDefaultAsync(w => w.ItemID == item.ItemID
                                        && w.UomID == item.UomID && w.PriceListID == receipt.PriceListID) ?? new PriceListDetail();
                        inventoryAccAmount += (decimal)priceListDetail.Cost * (decimal)item.Qty * (decimal)receipt.ExchangeRate;
                        COGSAccAmount += (decimal)priceListDetail.Cost * (decimal)item.Qty * (decimal)receipt.ExchangeRate;
                        var inventory_audit = _context.InventoryAudits.Where(w => w.ItemID == item.ItemID && w.WarehouseID == receipt.WarehouseID).ToList();
                        InventoryAudit item_inventory_audit = new()
                        {
                            ID = 0,
                            WarehouseID = receipt.WarehouseID,
                            BranchID = receipt.BranchID,
                            UserID = receipt.UserOrderID,
                            ItemID = item.ItemID,
                            CurrencyID = company.SystemCurrencyID,
                            UomID = orft.BaseUOM,
                            InvoiceNo = receipt.ReceiptNo,
                            Trans_Type = docTypeSP.Code,
                            Process = itemMaster.Process,
                            SystemDate = DateTime.Now,
                            TimeIn = DateTime.Now.ToShortTimeString().ToString(),
                            Qty = item.Qty * -1,
                            Cost = priceListDetail.Cost,
                            Price = 0,
                            CumulativeQty = inventory_audit.Sum(q => q.Qty) - item.Qty,
                            CumulativeValue = inventory_audit.Sum(s => s.Trans_Valuse) - (item.Qty * priceListDetail.Cost),
                            Trans_Valuse = item.Qty * priceListDetail.Cost * -1,
                            //ExpireDate = item_IssusStock.ExpireDate,
                            LocalCurID = receipt.LocalCurrencyID,
                            LocalSetRate = receipt.LocalSetRate,
                            CompanyID = receipt.CompanyID,
                            DocumentTypeID = docTypeSP.ID,
                            SeriesID = receipt.SeriesID,
                            SeriesDetailID = receipt.SeriesDID,
                            TypeItem = "Standard",
                        };
                        _context.InventoryAudits.Update(item_inventory_audit);
                        _context.SaveChanges();
                    }

                    // Tax Account ///
                    var taxg = _context.TaxGroups.Find(item.TaxGroupID) ?? new TaxGroup();
                    var taxAcc = _context.GLAccounts.Find(taxg.GLID) ?? new GLAccount();
                    decimal taxValue = item.TaxValue * (decimal)receipt.ExchangeRate;
                    if (taxAcc.ID > 0 && taxValue > 0)
                    {
                        var taxjur = journalEntryDetail.FirstOrDefault(w => w.ItemID == taxAcc.ID) ?? new JournalEntryDetail();
                        if (taxjur.ItemID > 0)
                        {
                            var accBalance = accountBalance.FirstOrDefault(w => w.GLAID == taxAcc.ID);
                            taxAcc.Balance -= taxValue;
                            //journalEntryDetail
                            taxjur.Credit += taxValue;
                            //accountBalance
                            accBalance.CumulativeBalance = taxAcc.Balance;
                            accBalance.Credit += taxValue;
                        }
                        else
                        {
                            taxAcc.Balance -= taxValue;
                            journalEntryDetail.Add(new JournalEntryDetail
                            {
                                JEID = journalEntry.ID,
                                Type = Type.GLAcct,
                                ItemID = taxAcc.ID,
                                Credit = taxValue,
                            });
                            //
                            accountBalance.Add(new AccountBalance
                            {
                                JEID = journalEntry.ID,
                                PostingDate = receipt.DateIn,
                                Origin = docTypeSP.ID,
                                OriginNo = receipt.ReceiptNo,
                                OffsetAccount = taxAcc.Code,
                                Details = docTypeJE.Name + " - " + taxAcc.Code,
                                CumulativeBalance = taxAcc.Balance,
                                Credit = taxValue,
                                LocalSetRate = (decimal)receipt.LocalSetRate,
                                GLAID = taxAcc.ID,
                            });
                        }
                        _context.Update(taxAcc);
                    }

                    // Account Revenue
                    var glAccRevenfifo = _context.GLAccounts.FirstOrDefault(w => w.ID == revenueAccID) ?? new GLAccount();
                    if (glAccRevenfifo.ID > 0 && revenueAccAmount > 0)
                    {
                        var list = journalEntryDetail.FirstOrDefault(w => w.ItemID == glAccRevenfifo.ID) ?? new JournalEntryDetail();
                        if (list.ItemID > 0)
                        {
                            var accBalance = accountBalance.FirstOrDefault(w => w.GLAID == revenueAccID);
                            glAccRevenfifo.Balance -= revenueAccAmount;
                            //journalEntryDetail
                            list.Credit += revenueAccAmount;
                            //accountBalance
                            accBalance.CumulativeBalance = glAccRevenfifo.Balance;
                            accBalance.Credit += revenueAccAmount;
                        }
                        else
                        {
                            glAccRevenfifo.Balance -= revenueAccAmount;
                            journalEntryDetail.Add(new JournalEntryDetail
                            {
                                JEID = journalEntry.ID,
                                Type = Type.GLAcct,
                                ItemID = revenueAccID,
                                Credit = revenueAccAmount,
                            });
                            accountBalance.Add(new AccountBalance
                            {
                                JEID = journalEntry.ID,
                                PostingDate = receipt.DateOut,
                                Origin = docTypeSP.ID,
                                OriginNo = receipt.ReceiptNo,
                                OffsetAccount = offsetAcc,
                                Details = docTypeJE.Name + "-" + glAccRevenfifo.Code,
                                CumulativeBalance = glAccRevenfifo.Balance,
                                Credit = revenueAccAmount,
                                LocalSetRate = (decimal)receipt.LocalSetRate,
                                GLAID = revenueAccID,
                            });
                        }
                        _context.Update(glAccRevenfifo);
                    }
                }

                if (itemMaster.ManItemBy == ManageItemBy.None || item.IsKsmsMaster)
                {
                    InsertFinancialPOS(
                        inventoryAccID, COGSAccID, inventoryAccAmount, COGSAccAmount, journalEntryDetail,
                        accountBalance, journalEntry, docTypeSP, docTypeJE, receipt, offsetAcc, item
                    );
                }
            }

            //IssuseInStockMaterial
            var xchangeRate = await _context.ExchangeRates.FirstOrDefaultAsync(x => !x.Delete && x.CurrencyID == company.LocalCurrencyID);
            List<ItemMaterial> itemMaterials = new();
            foreach (var item in receiptDetails)
            {
                var itemM = _context.ItemMasterDatas.FirstOrDefault(w => w.ID == item.ItemID);
                var orft = _context.GroupDUoMs.FirstOrDefault(w => w.GroupUoMID == itemM.GroupUomID && w.AltUOM == item.UomID);
                var bom = _context.BOMaterial.FirstOrDefault(w => w.ItemID == item.ItemID && w.Active == true) ?? new BOMaterial();
                var items_material = (from bomd in _context.BOMDetail.Where(w => w.BID == bom.BID && w.Detele == false)
                                      join i in _context.ItemMasterDatas.Where(im => !im.Delete) on bomd.ItemID equals i.ID
                                      join gd in _context.GroupDUoMs on bomd.UomID equals gd.AltUOM
                                      join uom in _context.UnitofMeasures on i.InventoryUoMID equals uom.ID
                                      select new ItemMaterial
                                      {
                                          ItemID = bomd.ItemID,
                                          GroupUoMID = gd.GroupUoMID,
                                          GUoMID = i.GroupUomID,
                                          Qty = ((double)item.Qty * orft.Factor) * ((double)bomd.Qty * gd.Factor),
                                          NegativeStock = bomd.NegativeStock,
                                          Process = i.Process,
                                          UomID = uom.ID,
                                          Factor = gd.Factor,
                                          IsKsms = item.IsKsms,
                                          IsKsmsMaster = item.IsKsmsMaster,
                                          IsReadOnly = item.IsReadonly
                                      }).Where(w => w.GroupUoMID == w.GUoMID).ToList();
                itemMaterials.AddRange(items_material);
            }
            var allMaterials = (from all in itemMaterials
                                group new { all } by new { all.ItemID, all.NegativeStock } into g
                                let data = g.FirstOrDefault()
                                select new
                                {
                                    data.all.ItemID,
                                    data.all.GroupUoMID,
                                    data.all.GUoMID,
                                    Qty = g.Sum(s => s.all.Qty),
                                    data.all.NegativeStock,
                                    data.all.Process,
                                    data.all.UomID,
                                    data.all.Factor,
                                    data.all.IsKsms,
                                    data.all.IsKsmsMaster,
                                    data.all.IsReadOnly
                                }).ToList();
            if (allMaterials.Count > 0)
            {
                foreach (var item_detail in allMaterials.ToList())
                {
                    int inventoryAccIDavg = 0, COGSAccIDavg = 0;
                    decimal inventoryAccAmountavg = 0, COGSAccAmountavg = 0;
                    var item_master_data = _context.ItemMasterDatas.FirstOrDefault(w => w.ID == item_detail.ItemID);
                    var item_warehouse_material = _context.WarehouseSummary.FirstOrDefault(w => w.WarehouseID == receipt.WarehouseID && w.ItemID == item_detail.ItemID);
                    var _itemAcc = _context.ItemAccountings.FirstOrDefault(i => i.WarehouseID == receipt.WarehouseID && i.ItemID == item_detail.ItemID);
                    var all_item_warehouse_detail = _context.WarehouseDetails.Where(w => w.WarehouseID == receipt.WarehouseID && w.ItemID == item_detail.ItemID).ToList();
                    var item_nagative = from wa in _context.WarehouseSummary.Where(w => w.ItemID == item_detail.ItemID)
                                        join na in _context.BOMDetail on wa.ItemID equals na.ItemID
                                        select new
                                        {
                                            NagaStock = wa.InStock
                                        };
                    if (item_master_data.SetGlAccount == SetGlAccount.ItemLevel)
                    {
                        var itemAccs = _context.ItemAccountings.Where(w => w.ItemID == item_detail.ItemID && w.WarehouseID == receipt.WarehouseID);
                        var inventoryAcc = (from ia in itemAccs
                                            join gl in glAccts on ia.InventoryAccount equals gl.Code
                                            select gl
                                             ).FirstOrDefault() ?? new GLAccount();
                        var COGSAcc = (from ia in itemAccs
                                       join gl in glAccts on ia.CostofGoodsSoldAccount equals gl.Code
                                       select gl
                                             ).FirstOrDefault() ?? new GLAccount();
                        inventoryAccIDavg = inventoryAcc.ID;
                        COGSAccIDavg = COGSAcc.ID;
                    }
                    else if (item_master_data.SetGlAccount == SetGlAccount.ItemGroup)
                    {
                        var itemAccs = _context.ItemAccountings.Where(w => w.ItemGroupID == item_master_data.ItemGroup1ID);
                        var inventoryAcc = (from ia in itemAccs
                                            join gl in glAccts on ia.InventoryAccount equals gl.Code
                                            select gl
                                             ).FirstOrDefault() ?? new GLAccount();
                        var COGSAcc = (from ia in itemAccs
                                       join gl in glAccts on ia.CostofGoodsSoldAccount equals gl.Code
                                       select gl
                                             ).FirstOrDefault() ?? new GLAccount();
                        inventoryAccIDavg = inventoryAcc.ID;
                        COGSAccIDavg = COGSAcc.ID;
                    }

                    var nagative_check = item_nagative.Sum(w => w.NagaStock);
                    //WerehouseSummary
                    item_warehouse_material.Committed -= (double)item_detail.Qty;
                    item_warehouse_material.InStock -= (double)item_detail.Qty;
                    //Itemmasterdata
                    item_master_data.StockIn -= (double)item_detail.Qty;
                    _utility.UpdateItemAccounting(_itemAcc, item_warehouse_material);

                    if (!item_detail.IsKsms && !item_detail.IsKsmsMaster)
                    {
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
                                    var stockOuts = new StockOut
                                    {
                                        Cost = (decimal)item_IssusStock.Cost,
                                        CurrencyID = item_IssusStock.CurrencyID,
                                        ID = 0,
                                        InStock = (decimal)@IssusQty,
                                        ItemID = item_detail.ItemID,
                                        ProcessItem = ProcessItem.FIFO,
                                        SyetemDate = DateTime.Now,
                                        TimeIn = DateTime.Now,
                                        WarehouseID = item_IssusStock.WarehouseID,
                                        UomID = item_detail.UomID,
                                        UserID = receipt.UserOrderID,
                                        ExpireDate = item_IssusStock.ExpireDate,
                                        TransType = TransTypeWD.POS,
                                        TransID = receipt.ReceiptID,
                                        OutStockFrom = receipt.ReceiptID,
                                        BPID = receipt.CustomerID,
                                        FromWareDetialID = item_IssusStock.ID
                                    };
                                    _context.StockOuts.Add(stockOuts);

                                    var inventory_audit = _context.InventoryAudits.Where(w => w.ItemID == item_detail.ItemID && w.WarehouseID == receipt.WarehouseID).ToList();
                                    item_inventory_audit.ID = 0;
                                    item_inventory_audit.WarehouseID = receipt.WarehouseID;
                                    item_inventory_audit.BranchID = receipt.BranchID;
                                    item_inventory_audit.UserID = receipt.UserOrderID;
                                    item_inventory_audit.ItemID = item_detail.ItemID;
                                    item_inventory_audit.CurrencyID = company.SystemCurrencyID;
                                    item_inventory_audit.UomID = item_detail.UomID;
                                    item_inventory_audit.InvoiceNo = receipt.ReceiptNo;
                                    item_inventory_audit.Trans_Type = docTypeSP.Code;
                                    item_inventory_audit.Process = item_detail.Process;
                                    item_inventory_audit.SystemDate = DateTime.Now;
                                    item_inventory_audit.TimeIn = receipt.TimeIn;
                                    item_inventory_audit.Qty = @IssusQty * -1;
                                    item_inventory_audit.Cost = item_IssusStock.Cost;
                                    item_inventory_audit.Price = 0;
                                    item_inventory_audit.CumulativeQty = inventory_audit.Sum(q => q.Qty) - @IssusQty;
                                    item_inventory_audit.CumulativeValue = inventory_audit.Sum(s => s.Trans_Valuse) - (@IssusQty * item_IssusStock.Cost);
                                    item_inventory_audit.Trans_Valuse = @IssusQty * item_IssusStock.Cost * (-1);
                                    item_inventory_audit.ExpireDate = item_IssusStock.ExpireDate;
                                    item_inventory_audit.LocalCurID = company.LocalCurrencyID;
                                    item_inventory_audit.LocalSetRate = xchangeRate.SetRate;
                                    item_inventory_audit.CompanyID = receipt.CompanyID;
                                    item_inventory_audit.DocumentTypeID = docTypeSP.ID;
                                    item_inventory_audit.SeriesID = receipt.SeriesID;
                                    item_inventory_audit.SeriesDetailID = receipt.SeriesDID;
                                }
                                //
                                inventoryAccAmountavg += (decimal)item_inventory_audit.Cost * (decimal)@Qty;
                                COGSAccAmountavg += (decimal)item_inventory_audit.Cost * (decimal)@Qty;
                                _utility.CumulativeValue(item_inventory_audit.WarehouseID, item_inventory_audit.ItemID, item_inventory_audit.CumulativeValue, _itemAcc);
                            }
                            else if (item_detail.Process == "Average")
                            {
                                item_IssusStock.InStock = @FIFOQty;
                                if (@IssusQty > 0)
                                {
                                    var warehouse_summary = _context.WarehouseSummary.FirstOrDefault(w => w.ItemID == item_detail.ItemID && w.WarehouseID == receipt.WarehouseID);
                                    double @sysAvCost = warehouse_summary.Cost;
                                    var inventory_audit = _context.InventoryAudits.Where(w => w.ItemID == item_detail.ItemID && w.WarehouseID == receipt.WarehouseID).ToList();

                                    var stockOuts = new StockOut
                                    {
                                        Cost = (decimal)@sysAvCost,
                                        CurrencyID = item_IssusStock.CurrencyID,
                                        ID = 0,
                                        InStock = (decimal)@IssusQty,
                                        ItemID = item_detail.ItemID,
                                        ProcessItem = ProcessItem.Average,
                                        SyetemDate = DateTime.Now,
                                        TimeIn = DateTime.Now,
                                        WarehouseID = item_IssusStock.WarehouseID,
                                        UomID = item_detail.UomID,
                                        UserID = receipt.UserOrderID,
                                        ExpireDate = item_IssusStock.ExpireDate,
                                        TransType = TransTypeWD.POS,
                                        TransID = receipt.ReceiptID,
                                        OutStockFrom = receipt.ReceiptID,
                                        BPID = receipt.CustomerID,
                                        FromWareDetialID = item_IssusStock.ID
                                    };
                                    _context.StockOuts.Add(stockOuts);

                                    item_inventory_audit.ID = 0;
                                    item_inventory_audit.WarehouseID = receipt.WarehouseID;
                                    item_inventory_audit.BranchID = receipt.BranchID;
                                    item_inventory_audit.UserID = receipt.UserOrderID;
                                    item_inventory_audit.ItemID = item_detail.ItemID;
                                    item_inventory_audit.CurrencyID = company.SystemCurrencyID;
                                    item_inventory_audit.UomID = item_detail.UomID;
                                    item_inventory_audit.InvoiceNo = receipt.ReceiptNo;
                                    item_inventory_audit.Trans_Type = docTypeSP.Code;
                                    item_inventory_audit.Process = item_detail.Process;
                                    item_inventory_audit.SystemDate = DateTime.Now;
                                    item_inventory_audit.TimeIn = receipt.TimeIn;
                                    item_inventory_audit.Qty = @IssusQty * -1;
                                    item_inventory_audit.Cost = @sysAvCost;
                                    item_inventory_audit.Price = 0;
                                    item_inventory_audit.CumulativeQty = inventory_audit.Sum(q => q.Qty) - @IssusQty;
                                    item_inventory_audit.CumulativeValue = inventory_audit.Sum(s => s.Trans_Valuse) - (@IssusQty * @sysAvCost);
                                    item_inventory_audit.Trans_Valuse = @IssusQty * @sysAvCost * (-1);
                                    item_inventory_audit.ExpireDate = item_IssusStock.ExpireDate;
                                    item_inventory_audit.LocalCurID = company.LocalCurrencyID;
                                    item_inventory_audit.LocalSetRate = xchangeRate.SetRate;
                                    item_inventory_audit.CompanyID = receipt.CompanyID;
                                    item_inventory_audit.DocumentTypeID = docTypeSP.ID;
                                    item_inventory_audit.SeriesID = receipt.SeriesID;
                                    item_inventory_audit.SeriesDetailID = receipt.SeriesDID;
                                }
                                double @AvgCost = _utility.CalAVGCost(item_detail.ItemID, receipt.WarehouseID, item_inventory_audit);
                                inventoryAccAmountavg += (decimal)(@AvgCost * @Qty);
                                COGSAccAmountavg += (decimal)(@AvgCost * @Qty);
                                _utility.UpdateAvgCost(item_detail.ItemID, receipt.WarehouseID, item_master_data.GroupUomID, item_inventory_audit.Qty, item_inventory_audit.Cost);
                                _utility.UpdateBomCost(item_detail.ItemID, item_inventory_audit.Qty, item_inventory_audit.Cost);
                                _utility.CumulativeValue(item_inventory_audit.WarehouseID, item_inventory_audit.ItemID, item_inventory_audit.CumulativeValue, _itemAcc);
                            }
                            _context.WarehouseDetails.Update(item_IssusStock);
                            _context.InventoryAudits.Add(item_inventory_audit);
                            await _context.SaveChangesAsync();
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
                                InventoryAudit item_inventory_audit = new();
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
                                            var stockOuts = new StockOut
                                            {
                                                Cost = (decimal)item_IssusStock.Cost,
                                                CurrencyID = item_IssusStock.CurrencyID,
                                                ID = 0,
                                                InStock = (decimal)@IssusQty,
                                                ItemID = item_detail.ItemID,
                                                ProcessItem = ProcessItem.FIFO,
                                                SyetemDate = DateTime.Now,
                                                TimeIn = DateTime.Now,
                                                WarehouseID = item_IssusStock.WarehouseID,
                                                UomID = item_detail.UomID,
                                                UserID = receipt.UserOrderID,
                                                ExpireDate = item_IssusStock.ExpireDate,
                                                TransType = TransTypeWD.POS,
                                                TransID = receipt.ReceiptID,
                                                OutStockFrom = receipt.ReceiptID,
                                                BPID = receipt.CustomerID,
                                                FromWareDetialID = item_IssusStock.ID
                                            };

                                            _context.StockOuts.Add(stockOuts);
                                            var inventory_audit = _context.InventoryAudits.Where(w => w.ItemID == item_detail.ItemID && w.WarehouseID == receipt.WarehouseID).ToList();
                                            item_inventory_audit.ID = 0;
                                            item_inventory_audit.WarehouseID = receipt.WarehouseID;
                                            item_inventory_audit.BranchID = receipt.BranchID;
                                            item_inventory_audit.UserID = receipt.UserOrderID;
                                            item_inventory_audit.ItemID = item_detail.ItemID;
                                            item_inventory_audit.CurrencyID = company.SystemCurrencyID;
                                            item_inventory_audit.UomID = item_detail.UomID;
                                            item_inventory_audit.InvoiceNo = receipt.ReceiptNo;
                                            item_inventory_audit.Trans_Type = docTypeSP.Code;
                                            item_inventory_audit.Process = item_detail.Process;
                                            item_inventory_audit.SystemDate = DateTime.Now;
                                            item_inventory_audit.TimeIn = receipt.TimeIn;
                                            item_inventory_audit.Qty = @IssusQty * -1;
                                            item_inventory_audit.Cost = item_IssusStock.Cost;
                                            item_inventory_audit.Price = 0;
                                            item_inventory_audit.CumulativeQty = inventory_audit.Sum(q => q.Qty) + (@IssusQty * (-1));
                                            item_inventory_audit.CumulativeValue = (inventory_audit.Sum(s => s.Trans_Valuse)) + (@IssusQty * item_IssusStock.Cost) * (-1);
                                            item_inventory_audit.Trans_Valuse = (@IssusQty * item_IssusStock.Cost) * (-1);
                                            item_inventory_audit.ExpireDate = item_IssusStock.ExpireDate;
                                            item_inventory_audit.LocalCurID = company.LocalCurrencyID;
                                            item_inventory_audit.LocalSetRate = xchangeRate.SetRate;
                                            item_inventory_audit.CompanyID = receipt.CompanyID;
                                            item_inventory_audit.DocumentTypeID = docTypeSP.ID;
                                            item_inventory_audit.SeriesID = receipt.SeriesID;
                                            item_inventory_audit.SeriesDetailID = receipt.SeriesDID;
                                        }
                                        //                                        
                                        inventoryAccAmountavg += (decimal)item_inventory_audit.Cost * (decimal)@Qty;
                                        COGSAccAmountavg += (decimal)item_inventory_audit.Cost * (decimal)@Qty;
                                        _utility.CumulativeValue(item_inventory_audit.WarehouseID, item_inventory_audit.ItemID, item_inventory_audit.CumulativeValue, _itemAcc);
                                        _context.WarehouseDetails.Update(item_IssusStock);
                                        _context.InventoryAudits.Add(item_inventory_audit);
                                        _context.SaveChanges();
                                    }
                                    else if (item_detail.Process == "Average")
                                    {
                                        item_IssusStock.InStock = item_IssusStock.InStock -= @IssusQty;
                                        if (@IssusQty > 0)
                                        {
                                            var warehouse_summary = _context.WarehouseSummary.FirstOrDefault(w => w.ItemID == item_detail.ItemID && w.WarehouseID == receipt.WarehouseID);
                                            double @sysAvCost = warehouse_summary.Cost;
                                            var inventory_audit = _context.InventoryAudits.Where(w => w.ItemID == item_detail.ItemID && w.WarehouseID == receipt.WarehouseID).ToList();

                                            var stockOuts = new StockOut
                                            {
                                                Cost = (decimal)item_IssusStock.Cost,
                                                CurrencyID = item_IssusStock.CurrencyID,
                                                ID = 0,
                                                InStock = (decimal)@IssusQty,
                                                ItemID = item_detail.ItemID,
                                                ProcessItem = ProcessItem.FIFO,
                                                SyetemDate = DateTime.Now,
                                                TimeIn = DateTime.Now,
                                                WarehouseID = item_IssusStock.WarehouseID,
                                                UomID = item_detail.UomID,
                                                UserID = receipt.UserOrderID,
                                                ExpireDate = item_IssusStock.ExpireDate,
                                                TransType = TransTypeWD.POS,
                                                TransID = receipt.ReceiptID,
                                                OutStockFrom = receipt.ReceiptID,
                                                BPID = receipt.CustomerID,
                                                FromWareDetialID = item_IssusStock.ID
                                            };
                                            _context.StockOuts.Add(stockOuts);
                                            item_inventory_audit.ID = 0;
                                            item_inventory_audit.WarehouseID = receipt.WarehouseID;
                                            item_inventory_audit.BranchID = receipt.BranchID;
                                            item_inventory_audit.UserID = receipt.UserOrderID;
                                            item_inventory_audit.ItemID = item_detail.ItemID;
                                            item_inventory_audit.CurrencyID = company.SystemCurrencyID;
                                            item_inventory_audit.UomID = item_detail.UomID;
                                            item_inventory_audit.InvoiceNo = receipt.ReceiptNo;
                                            item_inventory_audit.Trans_Type = docTypeSP.Code;
                                            item_inventory_audit.Process = item_detail.Process;
                                            item_inventory_audit.SystemDate = DateTime.Now;
                                            item_inventory_audit.TimeIn = receipt.TimeIn;
                                            item_inventory_audit.Qty = @IssusQty * -1;
                                            item_inventory_audit.Cost = @sysAvCost;
                                            item_inventory_audit.Price = 0;
                                            item_inventory_audit.CumulativeQty = inventory_audit.Sum(q => q.Qty) + (@IssusQty * (-1));
                                            item_inventory_audit.CumulativeValue = (inventory_audit.Sum(s => s.Trans_Valuse)) + (@IssusQty * @sysAvCost) * (-1);
                                            item_inventory_audit.Trans_Valuse = (@IssusQty * @sysAvCost) * (-1);
                                            item_inventory_audit.ExpireDate = item_IssusStock.ExpireDate;
                                            item_inventory_audit.LocalCurID = company.LocalCurrencyID;
                                            item_inventory_audit.LocalSetRate = xchangeRate.SetRate;
                                            item_inventory_audit.CompanyID = receipt.CompanyID;
                                            item_inventory_audit.DocumentTypeID = docTypeSP.ID;
                                            item_inventory_audit.SeriesID = receipt.SeriesID;
                                            item_inventory_audit.SeriesDetailID = receipt.SeriesDID;
                                            item_inventory_audit.DocumentTypeID = docTypeSP.ID;
                                            item_inventory_audit.SeriesID = receipt.SeriesID;
                                            item_inventory_audit.SeriesDetailID = receipt.SeriesDID;
                                        }
                                        //
                                        var inventoryAcct = _context.InventoryAudits.Where(w => w.ItemID == item_detail.ItemID).ToList();
                                        double @AvgCost = _utility.CalAVGCost(item_detail.ItemID, receipt.WarehouseID, item_inventory_audit);
                                        inventoryAccAmountavg += (decimal)(@AvgCost * @Qty);
                                        COGSAccAmountavg += (decimal)(@AvgCost * @Qty);
                                        _utility.UpdateAvgCost(item_detail.ItemID, receipt.WarehouseID, item_master_data.GroupUomID, item_inventory_audit.Qty, item_inventory_audit.Cost);
                                        _utility.UpdateBomCost(item_detail.ItemID, item_inventory_audit.Qty, item_inventory_audit.Cost);
                                        _utility.CumulativeValue(item_inventory_audit.WarehouseID, item_inventory_audit.ItemID, item_inventory_audit.CumulativeValue, _itemAcc);
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
                                            var stockOuts = new StockOut
                                            {
                                                Cost = (decimal)item_IssusStock.Cost,
                                                CurrencyID = item_IssusStock.CurrencyID,
                                                ID = 0,
                                                InStock = (decimal)@IssusQty,
                                                ItemID = item_detail.ItemID,
                                                ProcessItem = ProcessItem.FIFO,
                                                SyetemDate = DateTime.Now,
                                                TimeIn = DateTime.Now,
                                                WarehouseID = item_IssusStock.WarehouseID,
                                                UomID = item_detail.UomID,
                                                UserID = receipt.UserOrderID,
                                                ExpireDate = item_IssusStock.ExpireDate,
                                                TransType = TransTypeWD.POS,
                                                TransID = receipt.ReceiptID,
                                                OutStockFrom = receipt.ReceiptID,
                                                BPID = receipt.CustomerID,
                                                FromWareDetialID = item_IssusStock.ID
                                            };
                                            _context.StockOuts.Add(stockOuts);
                                            var inventory_audit = _context.InventoryAudits.Where(w => w.ItemID == item_detail.ItemID && w.WarehouseID == receipt.WarehouseID).ToList();
                                            item_inventory_audit.ID = 0;
                                            item_inventory_audit.WarehouseID = receipt.WarehouseID;
                                            item_inventory_audit.BranchID = receipt.BranchID;
                                            item_inventory_audit.UserID = receipt.UserOrderID;
                                            item_inventory_audit.ItemID = item_detail.ItemID;
                                            item_inventory_audit.CurrencyID = company.SystemCurrencyID;
                                            item_inventory_audit.UomID = item_detail.UomID;
                                            item_inventory_audit.InvoiceNo = receipt.ReceiptNo;
                                            item_inventory_audit.Trans_Type = docTypeSP.Code;
                                            item_inventory_audit.Process = item_detail.Process;
                                            item_inventory_audit.SystemDate = DateTime.Now;
                                            item_inventory_audit.TimeIn = receipt.TimeIn;
                                            item_inventory_audit.Qty = @IssusQty * -1;
                                            item_inventory_audit.Cost = item_IssusStock.Cost;
                                            item_inventory_audit.Price = 0;
                                            item_inventory_audit.CumulativeQty = inventory_audit.Sum(q => q.Qty) + (@IssusQty * (-1));
                                            item_inventory_audit.CumulativeValue = (inventory_audit.Sum(s => s.Trans_Valuse)) + (@IssusQty * item_IssusStock.Cost) * (-1);
                                            item_inventory_audit.Trans_Valuse = (@IssusQty * item_IssusStock.Cost) * (-1);
                                            item_inventory_audit.ExpireDate = item_IssusStock.ExpireDate;
                                            item_inventory_audit.LocalCurID = company.LocalCurrencyID;
                                            item_inventory_audit.LocalSetRate = xchangeRate.SetRate;
                                            item_inventory_audit.CompanyID = receipt.CompanyID;
                                            item_inventory_audit.DocumentTypeID = docTypeSP.ID;
                                            item_inventory_audit.SeriesID = receipt.SeriesID;
                                            item_inventory_audit.SeriesDetailID = receipt.SeriesDID;
                                            item_inventory_audit.DocumentTypeID = docTypeSP.ID;
                                            item_inventory_audit.SeriesID = receipt.SeriesID;
                                            item_inventory_audit.SeriesDetailID = receipt.SeriesDID;
                                        }
                                        //
                                        inventoryAccAmountavg += (decimal)item_inventory_audit.Cost * (decimal)@Qty;
                                        COGSAccAmountavg += (decimal)item_inventory_audit.Cost * (decimal)@Qty;
                                        _utility.CumulativeValue(item_inventory_audit.WarehouseID, item_inventory_audit.ItemID, item_inventory_audit.CumulativeValue, _itemAcc);
                                        _context.WarehouseDetails.Update(item_IssusStock);
                                        _context.InventoryAudits.Add(item_inventory_audit);
                                        _context.SaveChanges();
                                    }
                                    else if (item_detail.Process == "Average")
                                    {
                                        item_IssusStock.InStock = @FIFOQty;
                                        if (@IssusQty > 0)
                                        {
                                            var warehouse_summary = _context.WarehouseSummary.FirstOrDefault(w => w.ItemID == item_detail.ItemID && w.WarehouseID == receipt.WarehouseID);
                                            double @sysAvCost = warehouse_summary.Cost;
                                            var inventory_audit = _context.InventoryAudits.Where(w => w.ItemID == item_detail.ItemID && w.WarehouseID == receipt.WarehouseID).ToList();

                                            var stockOuts = new StockOut
                                            {
                                                Cost = (decimal)item_IssusStock.Cost,
                                                CurrencyID = item_IssusStock.CurrencyID,
                                                ID = 0,
                                                InStock = (decimal)@IssusQty,
                                                ItemID = item_detail.ItemID,
                                                ProcessItem = ProcessItem.FIFO,
                                                SyetemDate = DateTime.Now,
                                                TimeIn = DateTime.Now,
                                                WarehouseID = item_IssusStock.WarehouseID,
                                                UomID = item_detail.UomID,
                                                UserID = receipt.UserOrderID,
                                                ExpireDate = item_IssusStock.ExpireDate,
                                                TransType = TransTypeWD.POS,
                                                TransID = receipt.ReceiptID,
                                                OutStockFrom = receipt.ReceiptID,
                                                BPID = receipt.CustomerID,
                                                FromWareDetialID = item_IssusStock.ID
                                            };
                                            _context.StockOuts.Add(stockOuts);
                                            item_inventory_audit.ID = 0;
                                            item_inventory_audit.WarehouseID = receipt.WarehouseID;
                                            item_inventory_audit.BranchID = receipt.BranchID;
                                            item_inventory_audit.UserID = receipt.UserOrderID;
                                            item_inventory_audit.ItemID = item_detail.ItemID;
                                            item_inventory_audit.CurrencyID = company.SystemCurrencyID;
                                            item_inventory_audit.UomID = item_detail.UomID;
                                            item_inventory_audit.InvoiceNo = receipt.ReceiptNo;
                                            item_inventory_audit.Trans_Type = docTypeSP.Code;
                                            item_inventory_audit.Process = item_detail.Process;
                                            item_inventory_audit.SystemDate = DateTime.Now;
                                            item_inventory_audit.TimeIn = receipt.TimeIn;
                                            item_inventory_audit.Qty = @IssusQty * -1;
                                            item_inventory_audit.Cost = @sysAvCost;
                                            item_inventory_audit.Price = 0;
                                            item_inventory_audit.CumulativeQty = inventory_audit.Sum(q => q.Qty) - @IssusQty;
                                            item_inventory_audit.CumulativeValue = inventory_audit.Sum(s => s.Trans_Valuse) - (@IssusQty * @sysAvCost);
                                            item_inventory_audit.Trans_Valuse = @IssusQty * @sysAvCost * -1;
                                            item_inventory_audit.ExpireDate = item_IssusStock.ExpireDate;
                                            item_inventory_audit.LocalCurID = company.LocalCurrencyID;
                                            item_inventory_audit.LocalSetRate = xchangeRate.SetRate;
                                            item_inventory_audit.CompanyID = receipt.CompanyID;
                                            item_inventory_audit.DocumentTypeID = docTypeSP.ID;
                                            item_inventory_audit.SeriesID = receipt.SeriesID;
                                            item_inventory_audit.SeriesDetailID = receipt.SeriesDID;
                                        }
                                        double @AvgCost = _utility.CalAVGCost(item_detail.ItemID, receipt.WarehouseID, item_inventory_audit);
                                        inventoryAccAmountavg += (decimal)(@AvgCost * @Qty);
                                        COGSAccAmountavg += (decimal)(@AvgCost * @Qty);
                                        _utility.UpdateAvgCost(item_detail.ItemID, receipt.WarehouseID, item_master_data.GroupUomID, item_inventory_audit.Qty, item_inventory_audit.Cost);
                                        _utility.UpdateBomCost(item_detail.ItemID, item_inventory_audit.Qty, item_inventory_audit.Cost);
                                        _utility.CumulativeValue(item_inventory_audit.WarehouseID, item_inventory_audit.ItemID, item_inventory_audit.CumulativeValue, _itemAcc);

                                        _context.WarehouseDetails.Update(item_IssusStock);
                                        _context.InventoryAudits.Add(item_inventory_audit);
                                        _context.SaveChanges();
                                    }
                                    all_item_warehouse_detail = new List<WarehouseDetail>();
                                    break;
                                }
                            }
                        }

                        //inventoryAccID
                        var glAccInvenfifo = _context.GLAccounts.FirstOrDefault(w => w.ID == inventoryAccIDavg) ?? new GLAccount();
                        if (glAccInvenfifo.ID > 0)
                        {
                            var journalDetail = journalEntryDetail.FirstOrDefault(w => w.ItemID == glAccInvenfifo.ID) ?? new JournalEntryDetail();
                            if (journalDetail.ItemID > 0)
                            {
                                var accBalance = accountBalance.FirstOrDefault(w => w.GLAID == inventoryAccIDavg);
                                glAccInvenfifo.Balance -= inventoryAccAmountavg;
                                //journalEntryDetail
                                journalDetail.Credit += inventoryAccAmountavg;
                                //accountBalance
                                accBalance.CumulativeBalance = glAccInvenfifo.Balance;
                                accBalance.Credit += inventoryAccAmountavg;
                            }
                            else
                            {
                                glAccInvenfifo.Balance -= inventoryAccAmountavg;
                                journalEntryDetail.Add(new JournalEntryDetail
                                {
                                    JEID = journalEntry.ID,
                                    Type = Type.GLAcct,
                                    ItemID = inventoryAccIDavg,
                                    Credit = inventoryAccAmountavg,
                                });
                                //
                                accountBalance.Add(new AccountBalance
                                {
                                    JEID = journalEntry.ID,
                                    PostingDate = receipt.DateOut,
                                    Origin = docTypeSP.ID,
                                    OriginNo = receipt.ReceiptNo,
                                    OffsetAccount = offsetAcc,
                                    Details = docTypeJE.Name + " - " + glAccInvenfifo.Code,
                                    CumulativeBalance = glAccInvenfifo.Balance,
                                    Credit = inventoryAccAmountavg,
                                    LocalSetRate = (decimal)receipt.LocalSetRate,
                                    GLAID = inventoryAccIDavg,
                                });
                            }
                            _context.GLAccounts.Update(glAccInvenfifo);
                        }
                        // COGS
                        var glAccCOGSfifo = _context.GLAccounts.FirstOrDefault(w => w.ID == COGSAccIDavg) ?? new GLAccount();
                        if (glAccCOGSfifo.ID > 0)
                        {
                            var journalDetail = journalEntryDetail.FirstOrDefault(w => w.ItemID == glAccCOGSfifo.ID) ?? new JournalEntryDetail();
                            if (journalDetail.ItemID > 0)
                            {
                                var accBalance = accountBalance.FirstOrDefault(w => w.GLAID == COGSAccIDavg);
                                glAccCOGSfifo.Balance += COGSAccAmountavg;
                                //journalEntryDetail
                                journalDetail.Debit += COGSAccAmountavg;
                                //accountBalance
                                accBalance.CumulativeBalance = glAccCOGSfifo.Balance;
                                accBalance.Debit += COGSAccAmountavg;
                            }
                            else
                            {
                                glAccCOGSfifo.Balance += COGSAccAmountavg;
                                journalEntryDetail.Add(new JournalEntryDetail
                                {
                                    JEID = journalEntry.ID,
                                    Type = Type.GLAcct,
                                    ItemID = COGSAccIDavg,
                                    Debit = COGSAccAmountavg,
                                });
                                //
                                accountBalance.Add(new AccountBalance
                                {
                                    JEID = journalEntry.ID,
                                    PostingDate = receipt.DateOut,
                                    Origin = docTypeSP.ID,
                                    OriginNo = receipt.ReceiptNo,
                                    OffsetAccount = offsetAcc,
                                    Details = docTypeJE.Name + "-" + glAccCOGSfifo.Code,
                                    CumulativeBalance = glAccCOGSfifo.Balance,
                                    Debit = COGSAccAmountavg,
                                    LocalSetRate = (decimal)receipt.LocalSetRate,
                                    GLAID = COGSAccIDavg,
                                });
                            }
                            _context.GLAccounts.Update(glAccCOGSfifo);
                        }
                    }
                    _context.WarehouseSummary.Update(item_warehouse_material);
                    _context.ItemMasterDatas.Update(item_master_data);
                    _context.SaveChanges();
                }
            }

            if (receipt.Received == receipt.GrandTotal)
            {
                var incomAccReceive = _context.BusinessPartners.FirstOrDefault(w => w.ID == receipt.CustomerID);
                var glAccD = _context.GLAccounts.FirstOrDefault(w => w.ID == incomAccReceive.GLAccID);
                ////BPacc
                glAccD.Balance -= (decimal)receipt.GrandTotal_Sys;
                journalEntryDetail.Add(new JournalEntryDetail
                {
                    JEID = journalEntry.ID,
                    Type = Type.BPCode,
                    ItemID = incomAccReceive.GLAccID,
                    Credit = (decimal)receipt.GrandTotal_Sys,
                    BPAcctID = receipt.CustomerID,
                });
                glAccD.Balance -= (decimal)receipt.GrandTotal_Sys;
                accountBalance.Add(new AccountBalance
                {
                    JEID = journalEntry.ID,
                    PostingDate = receipt.DateIn,
                    Origin = docTypeSP.ID,
                    OriginNo = receipt.ReceiptNo,
                    OffsetAccount = glAccD.Code,
                    Details = docTypeJE.Name + " - " + glAccD.Code,
                    CumulativeBalance = glAccD.Balance,
                    Credit = (decimal)receipt.GrandTotal_Sys,
                    LocalSetRate = (decimal)receipt.LocalSetRate,
                    GLAID = incomAccReceive.GLAccID,
                    BPAcctID = receipt.CustomerID,
                    Creator = receipt.UserOrderID,
                });
                _context.Update(glAcc);

                foreach (var mutli in multiPayments.Where(x => x.Amount > 0))
                {
                    if (mutli.Amount > 0)
                    {
                        var accountSelect = _context.PaymentMeans.Find(mutli.PaymentMeanID) ?? new PaymentMeans();
                        var glAccC = _context.GLAccounts.FirstOrDefault(w => w.ID == accountSelect.AccountID) ?? new GLAccount();

                        if (glAccC.ID > 0)
                        {
                            var vatAmountJur = journalEntryDetail.FirstOrDefault(w => w.ItemID == glAccC.ID) ?? new JournalEntryDetail();
                            if (vatAmountJur.ItemID > 0)
                            {
                                var accBalance = accountBalance.FirstOrDefault(w => w.GLAID == glAccC.ID);
                                glAccC.Balance += mutli.Amount * mutli.SCRate;
                                //journalEntryDetail
                                vatAmountJur.Debit += mutli.Amount * mutli.SCRate;
                                //accountBalance
                                accBalance.CumulativeBalance = glAccC.Balance;
                                accBalance.Debit += mutli.Amount * mutli.SCRate;
                            }
                            else
                            {
                                journalEntryDetail.Add(new JournalEntryDetail
                                {
                                    JEID = journalEntry.ID,
                                    Type = Financials.Type.BPCode,
                                    ItemID = accountSelect.AccountID,
                                    Debit = mutli.Amount * mutli.SCRate,
                                });

                                glAccC.Balance += mutli.Amount * mutli.SCRate;
                                accountBalance.Add(new AccountBalance
                                {
                                    JEID = journalEntry.ID,
                                    PostingDate = receipt.DateIn,
                                    Origin = docTypeSP.ID,
                                    OriginNo = receipt.ReceiptNo,
                                    OffsetAccount = glAccC.Code,
                                    Details = docTypeJE.Name + "-" + glAccC.Code,
                                    CumulativeBalance = glAccC.Balance,
                                    Debit = mutli.Amount * mutli.SCRate,
                                    LocalSetRate = mutli.Amount * mutli.SCRate,
                                    GLAID = accountSelect.AccountID,
                                    Creator = receipt.UserOrderID,
                                });
                            }
                            _context.Update(glAccC);
                        }
                    }

                }

            }

            else if (receipt.Received > receipt.GrandTotal)
            {
                var incomAccReceive = _context.BusinessPartners.FirstOrDefault(w => w.ID == receipt.CustomerID);
                var glAccD = _context.GLAccounts.FirstOrDefault(w => w.ID == incomAccReceive.GLAccID);
                ////BPacc
                glAccD.Balance -= (decimal)receipt.Received;
                journalEntryDetail.Add(new JournalEntryDetail
                {
                    JEID = journalEntry.ID,
                    Type = Type.BPCode,
                    ItemID = incomAccReceive.GLAccID,
                    Credit = (decimal)receipt.Received,
                    BPAcctID = receipt.CustomerID,
                });
                glAccD.Balance -= (decimal)receipt.Received;
                accountBalance.Add(new AccountBalance
                {
                    JEID = journalEntry.ID,
                    PostingDate = receipt.DateIn,
                    Origin = docTypeSP.ID,
                    OriginNo = receipt.ReceiptNo,
                    OffsetAccount = glAccD.Code,
                    Details = docTypeJE.Name + " - " + glAccD.Code,
                    CumulativeBalance = glAccD.Balance,
                    Credit = (decimal)receipt.Received,
                    LocalSetRate = (decimal)receipt.LocalSetRate,
                    GLAID = incomAccReceive.GLAccID,
                    BPAcctID = receipt.CustomerID,
                    Creator = receipt.UserOrderID,
                });

                //=======================acount change=======================
                var lists = multiPayments.FirstOrDefault(x => x.Amount < 0) ?? new MultiPaymentMeans();
                if (lists.Amount == 0)
                {
                    var incomAccReceives = _context.BusinessPartners.FirstOrDefault(w => w.ID == receipt.CustomerID);
                    var glAccDs = _context.GLAccounts.FirstOrDefault(w => w.ID == incomAccReceive.GLAccID);
                    ////BPacc
                    ///
                    glAccDs.Balance += (decimal)receipt.Change;
                    journalEntryDetail.Add(new JournalEntryDetail
                    {
                        JEID = journalEntry.ID,
                        Type = Type.BPCode,
                        ItemID = incomAccReceives.GLAccID,
                        Debit = (decimal)receipt.Change,
                        BPAcctID = receipt.CustomerID,
                    });
                    glAccDs.Balance += (decimal)receipt.Change;
                    accountBalance.Add(new AccountBalance
                    {
                        JEID = journalEntry.ID,
                        PostingDate = receipt.DateIn,
                        Origin = docTypeSP.ID,
                        OriginNo = receipt.ReceiptNo,
                        OffsetAccount = glAccDs.Code,
                        Details = docTypeJE.Name + " - " + glAccDs.Code,
                        CumulativeBalance = glAccDs.Balance,
                        Debit = (decimal)receipt.Change,
                        LocalSetRate = (decimal)receipt.LocalSetRate,
                        GLAID = incomAccReceives.GLAccID,
                        BPAcctID = receipt.CustomerID,
                        Creator = receipt.UserOrderID,
                    });

                    var data = _context.MultipayMeansSetting.FirstOrDefault(x => x.Changed == true) ?? new MultipayMeansSetting();
                    var list = _context.PaymentMeans.FirstOrDefault(x => x.ID == data.PaymentID) ?? new PaymentMeans();
                    var glcc = _context.GLAccounts.FirstOrDefault(x => x.ID == list.AccountID) ?? new GLAccount();

                    if (glcc.ID > 0)
                    {
                        glcc.Balance -= (decimal)receipt.Change;
                        journalEntryDetail.Add(new JournalEntryDetail
                        {
                            JEID = journalEntry.ID,
                            Type = Type.BPCode,
                            ItemID = list.AccountID,
                            Credit = (decimal)receipt.Change,
                        });
                        glcc.Balance -= (decimal)receipt.Change;
                        accountBalance.Add(new AccountBalance
                        {
                            JEID = journalEntry.ID,
                            PostingDate = receipt.DateIn,
                            Origin = docTypeSP.ID,
                            OriginNo = receipt.ReceiptNo,
                            OffsetAccount = glcc.Code,
                            Details = docTypeJE.Name + " - " + glcc.Code,
                            CumulativeBalance = glcc.Balance,
                            Credit = (decimal)receipt.Change,
                            LocalSetRate = (decimal)receipt.LocalSetRate,
                            GLAID = list.AccountID,
                            Creator = receipt.UserOrderID,
                        });
                    }
                }
                //=======================end acount change=======================

                //======================= acount pay=======================
                foreach (var mutli in multiPayments.Where(x => x.Amount != 0))
                {
                    if (mutli.Amount >= 0)
                    {
                        var d = _context.MultipayMeansSetting.FirstOrDefault(x => x.Changed == true) ?? new MultipayMeansSetting();
                        if (mutli.PaymentMeanID == d.PaymentID)
                        {
                            mutli.Amount += (decimal)receipt.Change;
                        }
                        var accountSelect = _context.PaymentMeans.Find(mutli.PaymentMeanID) ?? new PaymentMeans();
                        var glAccC = _context.GLAccounts.FirstOrDefault(w => w.ID == accountSelect.AccountID) ?? new GLAccount();
                        if (glAccC.ID > 0)
                        {
                            //glAccC.Balance += mutli.Amount * mutli.SCRate;
                            journalEntryDetail.Add(new JournalEntryDetail
                            {
                                JEID = journalEntry.ID,
                                Type = Type.BPCode,
                                ItemID = accountSelect.AccountID,
                                Debit = mutli.Amount * mutli.SCRate,
                            });
                            glAccC.Balance += mutli.Amount * mutli.SCRate;
                            accountBalance.Add(new AccountBalance
                            {
                                JEID = journalEntry.ID,
                                PostingDate = receipt.DateIn,
                                Origin = docTypeSP.ID,
                                OriginNo = receipt.ReceiptNo,
                                OffsetAccount = glAccC.Code,
                                Details = docTypeJE.Name + " - " + glAccC.Code,
                                CumulativeBalance = glAccC.Balance,
                                Debit = mutli.Amount * mutli.SCRate,
                                LocalSetRate = (decimal)receipt.LocalSetRate,
                                GLAID = accountSelect.AccountID,
                                Creator = receipt.UserOrderID,
                            });
                        }
                    }

                    if (mutli.Amount < 0)
                    {
                        var incomAccReceives = _context.BusinessPartners.FirstOrDefault(w => w.ID == receipt.CustomerID);
                        var glAccDs = _context.GLAccounts.FirstOrDefault(w => w.ID == incomAccReceive.GLAccID);
                        ////BPacc
                        ///
                        glAccDs.Balance += (decimal)receipt.Change;
                        journalEntryDetail.Add(new JournalEntryDetail
                        {
                            JEID = journalEntry.ID,
                            Type = Type.BPCode,
                            ItemID = incomAccReceives.GLAccID,
                            Debit = (decimal)receipt.Change,
                            BPAcctID = receipt.CustomerID,
                        });
                        glAccDs.Balance += (decimal)receipt.Change;
                        accountBalance.Add(new AccountBalance
                        {
                            JEID = journalEntry.ID,
                            PostingDate = receipt.DateIn,
                            Origin = docTypeSP.ID,
                            OriginNo = receipt.ReceiptNo,
                            OffsetAccount = glAccDs.Code,
                            Details = docTypeJE.Name + " - " + glAccDs.Code,
                            CumulativeBalance = glAccDs.Balance,
                            Debit = (decimal)receipt.Change,
                            LocalSetRate = (decimal)receipt.LocalSetRate,
                            GLAID = incomAccReceives.GLAccID,
                            BPAcctID = receipt.CustomerID,
                            Creator = receipt.UserOrderID,
                        });

                        var accountSelect = _context.PaymentMeans.Find(mutli.PaymentMeanID) ?? new PaymentMeans();
                        var glAccC = _context.GLAccounts.FirstOrDefault(w => w.ID == accountSelect.AccountID) ?? new GLAccount();

                        if (glAccC.ID > 0)
                        {
                            var vatAmountJur = journalEntryDetail.FirstOrDefault(w => w.ItemID == glAccC.ID) ?? new JournalEntryDetail();
                            if (vatAmountJur.ItemID > 0)
                            {
                                var accBalance = accountBalance.FirstOrDefault(w => w.GLAID == glAccC.ID);
                                glAccC.Balance -= mutli.Amount * mutli.SCRate * -1;
                                //journalEntryDetail
                                vatAmountJur.Credit += mutli.Amount * mutli.SCRate * -1;
                                //accountBalance
                                accBalance.CumulativeBalance = glAccC.Balance;
                                accBalance.Credit += mutli.Amount * mutli.SCRate * -1;
                            }
                            else
                            {
                                journalEntryDetail.Add(new JournalEntryDetail
                                {
                                    JEID = journalEntry.ID,
                                    Type = Financials.Type.BPCode,
                                    ItemID = accountSelect.AccountID,
                                    Credit = mutli.Amount * mutli.SCRate * -1,
                                });

                                glAccC.Balance -= mutli.Amount * mutli.SCRate * -1;
                                accountBalance.Add(new AccountBalance
                                {
                                    JEID = journalEntry.ID,
                                    PostingDate = receipt.DateIn,
                                    Origin = docTypeSP.ID,
                                    OriginNo = receipt.ReceiptNo,
                                    OffsetAccount = glAccC.Code,
                                    Details = docTypeJE.Name + "-" + glAccC.Code,
                                    CumulativeBalance = glAccC.Balance,
                                    Credit = mutli.Amount * mutli.SCRate * -1,
                                    LocalSetRate = mutli.Amount * mutli.SCRate * -1,
                                    GLAID = accountSelect.AccountID,
                                    Creator = receipt.UserOrderID,
                                });
                            }
                            _context.Update(glAccC);
                        }
                    }
                }
                //=======================end acount pay=======================

            }
            else if (receipt.Received < receipt.GrandTotal_Sys && receipt.Received > 0)
            {
                var incomAccReceive = _context.BusinessPartners.FirstOrDefault(w => w.ID == receipt.CustomerID);
                var glAccD = _context.GLAccounts.FirstOrDefault(w => w.ID == incomAccReceive.GLAccID);
                ////BPacc
                glAccD.Balance -= (decimal)receipt.GrandTotal_Sys;
                journalEntryDetail.Add(new JournalEntryDetail
                {
                    JEID = journalEntry.ID,
                    Type = Type.BPCode,
                    ItemID = incomAccReceive.GLAccID,
                    Credit = (decimal)receipt.Received,
                    BPAcctID = receipt.CustomerID,
                });
                glAccD.Balance -= (decimal)receipt.Received;
                accountBalance.Add(new AccountBalance
                {
                    JEID = journalEntry.ID,
                    PostingDate = receipt.DateIn,
                    Origin = docTypeSP.ID,
                    OriginNo = receipt.ReceiptNo,
                    OffsetAccount = glAccD.Code,
                    Details = docTypeJE.Name + " - " + glAccD.Code,
                    CumulativeBalance = glAccD.Balance,
                    Credit = (decimal)receipt.Received,
                    LocalSetRate = (decimal)receipt.LocalSetRate,
                    GLAID = incomAccReceive.GLAccID,
                    BPAcctID = receipt.CustomerID,
                    Creator = receipt.UserOrderID,
                });
                ////BPacc 
                foreach (var mutli in multiPayments.Where(x => x.Amount != 0))
                {
                    if (mutli.Amount >= 0)
                    {

                        var accountSelect = _context.PaymentMeans.Find(mutli.PaymentMeanID) ?? new PaymentMeans();
                        var glAccC = _context.GLAccounts.FirstOrDefault(w => w.ID == accountSelect.AccountID) ?? new GLAccount();
                        if (glAccC.ID > 0)
                        {
                            var vatAmountJur = journalEntryDetail.FirstOrDefault(w => w.ItemID == glAccC.ID) ?? new JournalEntryDetail();
                            if (vatAmountJur.ItemID > 0)
                            {
                                var accBalance = accountBalance.FirstOrDefault(w => w.GLAID == glAccC.ID);
                                glAccC.Balance += mutli.Amount * mutli.SCRate;
                                //journalEntryDetail
                                vatAmountJur.Debit += mutli.Amount * mutli.SCRate;
                                //accountBalance
                                accBalance.CumulativeBalance = glAccC.Balance;
                                accBalance.Debit += mutli.Amount * mutli.SCRate;
                            }
                            else
                            {
                                journalEntryDetail.Add(new JournalEntryDetail
                                {
                                    JEID = journalEntry.ID,
                                    Type = Financials.Type.BPCode,
                                    ItemID = accountSelect.AccountID,
                                    Debit = mutli.Amount * mutli.SCRate,
                                });

                                glAccC.Balance += mutli.Amount * mutli.SCRate;
                                accountBalance.Add(new AccountBalance
                                {
                                    JEID = journalEntry.ID,
                                    PostingDate = receipt.DateIn,
                                    Origin = docTypeSP.ID,
                                    OriginNo = receipt.ReceiptNo,
                                    OffsetAccount = glAccC.Code,
                                    Details = docTypeJE.Name + "-" + glAccC.Code,
                                    CumulativeBalance = glAccC.Balance,
                                    Debit = mutli.Amount * mutli.SCRate,
                                    LocalSetRate = mutli.Amount * mutli.SCRate,
                                    GLAID = accountSelect.AccountID,
                                    Creator = receipt.UserOrderID,
                                });
                            }
                            _context.Update(glAccC);
                        }
                    }

                }

                var sysCurrency = _context.Currency.FirstOrDefault(i => i.ID == receipt.SysCurrencyID);
                decimal appliSys = receipt.AppliedAmount * (decimal)receipt.ExchangeRate;
                // insert data to IncomingPaymentCustomer table ///
                incomingCus.Applied_Amount = (double)receipt.AppliedAmount;
                incomingCus.BalanceDue = receipt.GrandTotal - (double)receipt.AppliedAmount;
                incomingCus.BranchID = receipt.BranchID;
                incomingCus.CashDiscount = 0;
                incomingCus.CompanyID = receipt.CompanyID;
                incomingCus.CurrencyID = receipt.PLCurrencyID;
                incomingCus.CurrencyName = receipt.Currency.Description;
                incomingCus.CustomerID = receipt.CustomerID;
                incomingCus.Date = receipt.DateIn;
                incomingCus.DocTypeID = docTypeSP.ID;
                incomingCus.ExchangeRate = receipt.ExchangeRate;
                incomingCus.InvoiceNumber = receipt.ReceiptNo;
                incomingCus.ItemInvoice = $"{docTypeSP.Code}-{receipt.ReceiptNo}";
                incomingCus.LocalCurID = receipt.LocalCurrencyID;
                incomingCus.LocalSetRate = receipt.LocalSetRate;
                incomingCus.PostingDate = receipt.DateIn;
                incomingCus.SeriesDID = receipt.SeriesDID;
                incomingCus.SeriesID = receipt.SeriesID;
                incomingCus.Status = "open";
                incomingCus.SysCurrency = receipt.SysCurrencyID;
                incomingCus.SysName = sysCurrency.Description;
                incomingCus.Total = (double)receipt.GrandTotal;
                incomingCus.TotalDiscount = 0;
                incomingCus.TotalPayment = receipt.GrandTotal - (double)receipt.AppliedAmount;
                incomingCus.WarehouseID = receipt.WarehouseID;
                _context.IncomingPaymentCustomers.Update(incomingCus);
                // Create Series And Journal //
                SeriesDetail seriesDetailRC = new();
                var docTypeRC = _context.DocumentTypes.FirstOrDefault(i => i.Code == "RC");
                var seriesRC = _context.Series.FirstOrDefault(i => i.Default && i.CompanyID == receipt.CompanyID && i.DocuTypeID == docTypeRC.ID);
                seriesDetailRC.Number = seriesRC.NextNo;
                seriesDetailRC.SeriesID = seriesRC.ID;
                _context.SeriesDetails.Update(seriesDetailRC);
                _context.SaveChanges();
                var seriesDetailID = seriesDetail.ID;
                string Sno = seriesRC.NextNo;
                long No = long.Parse(Sno);
                seriesRC.NextNo = Convert.ToString(No + 1);
                _context.Series.Update(seriesRC);
                _context.SaveChanges();
                /// insert data to incomingpayment ///
                incoming.BranchID = receipt.BranchID;
                incoming.CompanyID = receipt.CompanyID;
                incoming.CustomerID = receipt.CustomerID;
                incoming.DocTypeID = docTypeRC.ID;
                incoming.SeriesDID = seriesDetailRC.ID;
                incoming.SeriesID = seriesRC.ID;
                incoming.InvoiceNumber = seriesDetailRC.Number;
                incoming.DocumentDate = receipt.DateIn;
                incoming.LocalCurID = receipt.LocalCurrencyID;
                incoming.LocalSetRate = receipt.LocalSetRate;
                incoming.PaymentMeanID = receipt.PaymentMeansID;
                incoming.PostingDate = receipt.DateIn;
                incoming.TotalAmountDue = receipt.GrandTotal - (double)receipt.AppliedAmount;
                incoming.UserID = receipt.UserOrderID;
                incoming.Status = "open";
                incoming.IcoPayCusID = incomingCus.IncomingPaymentCustomerID;
                _context.IncomingPayments.Update(incoming);
                _context.SaveChanges();
                incomingDetail.Applied_Amount = (double)receipt.AppliedAmount;
                incomingDetail.BalanceDue = receipt.GrandTotal - (double)receipt.AppliedAmount;
                incomingDetail.CashDiscount = 0;
                incomingDetail.CurrencyID = receipt.PLCurrencyID;
                incomingDetail.Date = receipt.DateIn;
                incomingDetail.Delete = false;
                incomingDetail.CheckPay = true;
                incomingDetail.DocNo = docTypeSP.Code;
                incomingDetail.DocTypeID = docTypeSP.ID;
                incomingDetail.IncomingPaymentID = incoming.IncomingPaymentID;
                incomingDetail.InvoiceNumber = receipt.ReceiptNo;
                incomingDetail.ItemInvoice = $"{docTypeSP.Code}-{receipt.ReceiptNo}";
                incomingDetail.LocalCurID = receipt.LocalCurrencyID;
                incomingDetail.LocalSetRate = receipt.LocalSetRate;
                incomingDetail.Total = receipt.GrandTotal;
                incomingDetail.TotalDiscount = 0;
                incomingDetail.Totalpayment = (double)receipt.AppliedAmount;
                incomingDetail.CurrencyName = receipt.Currency.Description;
                incomingDetail.ExchangeRate = receipt.ExchangeRate;
                incomingDetail.IcoPayCusID = incomingCus.IncomingPaymentCustomerID;
                _context.IncomingPaymentDetails.Update(incomingDetail);
                _context.SeriesDetails.Update(seriesDetailRC);
                _context.SaveChanges();
            }
            else if (receipt.Received == 0 && receipt.GrandTotal > 0)
            {
                var sysCurrency = _context.Currency.FirstOrDefault(i => i.ID == receipt.SysCurrencyID);
                // insert data to IncomingPaymentCustomer table ///
                incomingCus.Applied_Amount = (double)receipt.AppliedAmount;
                incomingCus.BalanceDue = receipt.GrandTotal - (double)receipt.AppliedAmount;
                incomingCus.BranchID = receipt.BranchID;
                incomingCus.CashDiscount = 0;
                incomingCus.CompanyID = receipt.CompanyID;
                incomingCus.CurrencyID = receipt.PLCurrencyID;
                incomingCus.CurrencyName = receipt.Currency.Description;
                incomingCus.CustomerID = receipt.CustomerID;
                incomingCus.Date = receipt.DateIn;
                incomingCus.DocTypeID = docTypeSP.ID;
                incomingCus.ExchangeRate = receipt.ExchangeRate;
                incomingCus.InvoiceNumber = receipt.ReceiptNo;
                incomingCus.ItemInvoice = $"{docTypeSP.Code}-{receipt.ReceiptNo}";
                incomingCus.LocalCurID = receipt.LocalCurrencyID;
                incomingCus.LocalSetRate = receipt.LocalSetRate;
                incomingCus.PostingDate = receipt.DateIn;
                incomingCus.SeriesDID = receipt.SeriesDID;
                incomingCus.SeriesID = receipt.SeriesID;
                incomingCus.Status = "open";
                incomingCus.SysCurrency = receipt.SysCurrencyID;
                incomingCus.SysName = sysCurrency.Description;
                incomingCus.Total = (double)receipt.GrandTotal;
                incomingCus.TotalDiscount = 0;
                incomingCus.TotalPayment = receipt.GrandTotal - (double)receipt.AppliedAmount;
                incomingCus.WarehouseID = receipt.WarehouseID;
                _context.IncomingPaymentCustomers.Update(incomingCus);
                await _context.SaveChangesAsync();
            }

            if (journalEntry.ID > 0)
            {
                journalEntry.TotalDebit = journalEntryDetail.Sum(s => s.Debit);
                journalEntry.TotalCredit = journalEntryDetail.Sum(s => s.Credit);
                _context.JournalEntries.Update(journalEntry);
            }

            await AlertPaymentAsync(receipt, series.Name, receipt.ReceiptNo, receiptDetails.Count);
            _context.JournalEntryDetails.UpdateRange(journalEntryDetail);

            _context.AccountBalances.UpdateRange(accountBalance);
            await _context.SaveChangesAsync();
        }

        //Return Stock OrderDetail
        public void OrderDetailReturnStock(int receiptid_new, List<SerialNumber> serials, List<BatchNo> batches, int? receiptid = 0, string? type = "")
        {
            var cancelreceipt = _context.ReceiptMemo.First(r => r.ID == receiptid_new);
            var receiptdetail = _context.ReceiptDetailMemoKvms.Where(d => d.ReceiptMemoID == receiptid_new);
            var docType = _context.DocumentTypes.FirstOrDefault(w => w.Code == "RP");
            var Com = _context.Company.FirstOrDefault(c => c.ID == cancelreceipt.CompanyID);
            var series = _context.Series.Find(cancelreceipt.SeriesID) ?? new Series();
            JournalEntry journalEntry = new();
            SeriesDetail seriesDetail = new();
            List<JournalEntryDetail> journalEntryDetail = new();
            List<AccountBalance> accountBalance = new();
            string OffsetAcc = "";
            var douTypeID = _context.DocumentTypes.FirstOrDefault(w => w.Code == "JE");
            var defaultJE = _context.Series.FirstOrDefault(w => w.Default == true && w.DocuTypeID == douTypeID.ID) ?? new Series();
            if (defaultJE.ID > 0)
            {
                // update series
                string Sno = defaultJE.NextNo;
                long No = long.Parse(Sno);
                defaultJE.NextNo = Convert.ToString(No + 1);
                // update series details
                seriesDetail.SeriesID = journalEntry.SeriesID;
                seriesDetail.Number = Sno;
                _context.Update(defaultJE);
                _context.Update(seriesDetail);
                _context.SaveChanges();
                // Insert Journal Entry
                journalEntry.SeriesID = defaultJE.ID;
                journalEntry.Number = defaultJE.PreFix + "-" + Sno;
                journalEntry.DouTypeID = defaultJE.DocuTypeID;
                journalEntry.Creator = cancelreceipt.UserOrderID;
                journalEntry.TransNo = cancelreceipt.ReceiptNo;
                journalEntry.PostingDate = cancelreceipt.DateOut;
                journalEntry.DocumentDate = cancelreceipt.DateOut;
                journalEntry.DueDate = cancelreceipt.DateOut;
                journalEntry.SSCID = cancelreceipt.SysCurrencyID;
                journalEntry.LLCID = cancelreceipt.LocalCurrencyID;
                journalEntry.CompanyID = cancelreceipt.CompanyID;
                journalEntry.LocalSetRate = (decimal)cancelreceipt.LocalSetRate;
                journalEntry.SeriesDID = seriesDetail.ID;
                journalEntry.Remarks = series.Name + "-" + cancelreceipt.ReceiptNo;
                _context.Update(journalEntry);
            }
            _context.SaveChanges();

            //Credit account receivable  
            var accountReceive = _context.BusinessPartners.FirstOrDefault(w => w.ID == cancelreceipt.CustomerID);
            var glAcc = _context.GLAccounts.FirstOrDefault(w => w.ID == accountReceive.GLAccID) ?? new GLAccount();
            OffsetAcc = accountReceive.Code + " " + glAcc.Code;
            decimal cardPay = cancelreceipt.OtherPaymentGrandTotal * (decimal)cancelreceipt.ExchangeRate;
            decimal grandTotalSys = (decimal)cancelreceipt.GrandTotalSys;
            if (glAcc.ID > 0 && grandTotalSys > 0)
            {
                journalEntryDetail.Add(new JournalEntryDetail
                {
                    JEID = journalEntry.ID,
                    Type = Type.BPCode,
                    ItemID = accountReceive.GLAccID,
                    Credit = grandTotalSys,
                    BPAcctID = cancelreceipt.CustomerID,
                });
                //Insert             
                glAcc.Balance -= grandTotalSys;
                accountBalance.Add(new AccountBalance
                {
                    JEID = journalEntry.ID,

                    PostingDate = cancelreceipt.DateOut,
                    Origin = docType.ID,
                    OriginNo = cancelreceipt.ReceiptNo,
                    OffsetAccount = OffsetAcc,
                    Details = douTypeID.Name + "-" + glAcc.Code,
                    CumulativeBalance = glAcc.Balance,
                    Credit = grandTotalSys,
                    LocalSetRate = (decimal)cancelreceipt.LocalSetRate,
                    GLAID = accountReceive.GLAccID,
                    BPAcctID = cancelreceipt.CustomerID,
                    Creator = cancelreceipt.UserOrderID,
                });
                _context.AccountBalances.UpdateRange(accountBalance);
                _context.GLAccounts.Update(glAcc);
            }
            #region
            //if (cancelreceipt.OtherPaymentGrandTotal > 0)
            //{
            //    if (cancelreceipt.PaymentType == PaymentType.CardMember)
            //    {
            //        //Card Member
            //        var cardMember = _context.AccountMemberCards.FirstOrDefault(i => i.CashAccID > 0 && i.UnearnedRevenueID > 0) ?? new AccountMemberCard();
            //        var cashAcc = _context.GLAccounts.FirstOrDefault(w => w.ID == cardMember.UnearnedRevenueID);
            //        decimal totalPayMemberCardSys = cancelreceipt.OtherPaymentGrandTotal * (decimal)cancelreceipt.ExchangeRate;
            //        if (cashAcc.ID > 0)
            //        {
            //            var cardjur = journalEntryDetail.FirstOrDefault(w => w.ItemID == cashAcc.ID) ?? new JournalEntryDetail();
            //            if (cardjur.ItemID > 0)
            //            {
            //                var accBalance = accountBalance.FirstOrDefault(w => w.GLAID == cashAcc.ID);
            //                cashAcc.Balance -= totalPayMemberCardSys;
            //                //journalEntryDetail
            //                cardjur.Credit += totalPayMemberCardSys;
            //                //accountBalance
            //                accBalance.CumulativeBalance = cashAcc.Balance;
            //                accBalance.Credit += totalPayMemberCardSys;
            //            }
            //            else
            //            {
            //                cashAcc.Balance -= totalPayMemberCardSys;
            //                journalEntryDetail.Add(new JournalEntryDetail
            //                {
            //                    JEID = journalEntry.ID,
            //                    Type = Type.GLAcct,
            //                    ItemID = cashAcc.ID,
            //                    Credit = totalPayMemberCardSys,
            //                });
            //                //
            //                accountBalance.Add(new AccountBalance
            //                {
            //                    PostingDate = cancelreceipt.DateIn,
            //                    Origin = docType.ID,
            //                    OriginNo = cancelreceipt.ReceiptNo,
            //                    OffsetAccount = cashAcc.Code,
            //                    Details = douTypeID.Name + " - " + cashAcc.Code,
            //                    CumulativeBalance = cashAcc.Balance,
            //                    Credit = totalPayMemberCardSys,
            //                    LocalSetRate = (decimal)cancelreceipt.LocalSetRate,
            //                    GLAID = cashAcc.ID,
            //                });
            //            }
            //            _context.Update(cashAcc);
            //        }
            //        var customer = _context.BusinessPartners.Find(cancelreceipt.CustomerID);
            //        if (customer != null)
            //        {
            //            customer.Balance += cancelreceipt.OtherPaymentGrandTotal;
            //            //CardMemberDepositTransactions
            //            CardMemberDepositTransaction cmdt = new()
            //            {
            //                Amount = cancelreceipt.OtherPaymentGrandTotal,
            //                CardMemberDepositID = 0,
            //                CardMemberID = customer.CardMemberID,
            //                CumulativeBalance = customer.Balance,
            //                CusID = customer.ID,
            //                ID = 0,
            //                DocTypeID = docType.ID,
            //                Number = cancelreceipt.ReceiptNo,
            //                PostingDate = cancelreceipt.DateIn,
            //                SeriesDID = cancelreceipt.SeriesDID,
            //                SeriesID = cancelreceipt.SeriesID,
            //                UserID = cancelreceipt.UserOrderID,
            //            };
            //            _context.BusinessPartners.Update(customer);
            //            _context.CardMemberDepositTransactions.Update(cmdt);
            //            _context.SaveChanges();
            //        }
            //    }
            //}
            #endregion
            //=================paycard =============================

            MultiPaymentMeans returncardPay = new MultiPaymentMeans();
            MultiPaymentMeans returnMultipay = new MultiPaymentMeans();
            List<MultiPaymentMeans> multi = new List<MultiPaymentMeans>();
            var datas = _context.MultiPaymentMeans.Where(x => x.ReceiptID == receiptid).ToList();
            decimal TotalAmount = 0;
            foreach (var d in datas)
            {
                TotalAmount += d.Amount * d.SCRate;
            }
            if (TotalAmount == (decimal)cancelreceipt.GrandTotalSys)
            {

                returncardPay = _context.MultiPaymentMeans.FirstOrDefault(x => x.ReceiptID == receiptid && x.Type == PaymentMeanType.CardMember) ?? new MultiPaymentMeans();
                if (returncardPay.ID > 0)
                {
                    //Card Member
                    var cardMember = _context.AccountMemberCards.FirstOrDefault(i => i.CashAccID > 0 && i.UnearnedRevenueID > 0) ?? new AccountMemberCard();
                    var cashAcc = _context.GLAccounts.FirstOrDefault(w => w.ID == cardMember.UnearnedRevenueID);
                    if (cashAcc.ID > 0)
                    {
                        var cardjur = journalEntryDetail.FirstOrDefault(w => w.ItemID == cashAcc.ID) ?? new JournalEntryDetail();
                        if (cardjur.ItemID > 0)
                        {
                            var accBalance = accountBalance.FirstOrDefault(w => w.GLAID == cashAcc.ID);
                            cashAcc.Balance -= returncardPay.Amount * returncardPay.SCRate;
                            //journalEntryDetail
                            cardjur.Credit += returncardPay.Amount * returncardPay.SCRate;
                            //accountBalance
                            accBalance.CumulativeBalance = cashAcc.Balance;
                            accBalance.Credit += returncardPay.Amount * returncardPay.SCRate;
                        }
                        else
                        {
                            cashAcc.Balance -= returncardPay.Amount * returncardPay.SCRate;
                            journalEntryDetail.Add(new JournalEntryDetail
                            {
                                JEID = journalEntry.ID,
                                Type = Type.GLAcct,
                                ItemID = cashAcc.ID,
                                Credit = returncardPay.Amount * returncardPay.SCRate,
                            });
                            //
                            accountBalance.Add(new AccountBalance
                            {
                                JEID = journalEntry.ID,

                                PostingDate = cancelreceipt.DateIn,
                                Origin = docType.ID,
                                OriginNo = cancelreceipt.ReceiptNo,
                                OffsetAccount = cashAcc.Code,
                                Details = douTypeID.Name + " - " + cashAcc.Code,
                                CumulativeBalance = cashAcc.Balance,
                                Credit = returncardPay.Amount * returncardPay.SCRate,
                                GLAID = cashAcc.ID,
                            });
                        }
                            _context.Update(cashAcc);
                        }
                        var customer = _context.BusinessPartners.Find(cancelreceipt.CustomerID);
                        if (customer != null)
                        {
                            customer.Balance -= cancelreceipt.OtherPaymentGrandTotal;
                            //CardMemberDepositTransactions
                            CardMemberDepositTransaction cmdt = new()
                            {
                                Amount = cancelreceipt.OtherPaymentGrandTotal * -1,
                                CardMemberDepositID = 0,
                                CardMemberID = customer.CardMemberID,
                                CumulativeBalance = customer.Balance,
                                CusID = customer.ID,
                                ID = 0,
                                DocTypeID = docType.ID,
                                Number = cancelreceipt.ReceiptNo,
                                PostingDate = cancelreceipt.DateIn,
                                SeriesDID = cancelreceipt.SeriesDID,
                                SeriesID = cancelreceipt.SeriesID,
                                UserID = cancelreceipt.UserOrderID,
                            };
                            _context.BusinessPartners.Update(customer);
                            _context.CardMemberDepositTransactions.Update(cmdt);
                            _context.SaveChanges();
                        }

                    }
                }
                else if (TotalAmount > (decimal)cancelreceipt.GrandTotalSys)
                {
                    if (type != "return")
                    {
                        var multipay = _context.MultipayMeansSetting.FirstOrDefault(w => w.Changed == true) ?? new MultipayMeansSetting();
                        var pay = _context.PaymentMeans.FirstOrDefault(s => s.ID == multipay.PaymentID) ?? new PaymentMeans();
                        var glaccdata = _context.GLAccounts.FirstOrDefault(w => w.ID == pay.AccountID) ?? new GLAccount();

                        if (glaccdata.ID > 0)
                        {
                            var vatAmountJur = journalEntryDetail.FirstOrDefault(w => w.ItemID == glaccdata.ID) ?? new JournalEntryDetail();
                            if (vatAmountJur.ItemID > 0)
                            {
                                var accBalance = accountBalance.FirstOrDefault(w => w.GLAID == glaccdata.ID);
                                glaccdata.Balance += TotalAmount - (decimal)cancelreceipt.GrandTotalSys;
                                //journalEntryDetail
                                vatAmountJur.Debit += TotalAmount - (decimal)cancelreceipt.GrandTotalSys;
                                //accountBalance
                                accBalance.CumulativeBalance = glaccdata.Balance;
                                accBalance.Debit += TotalAmount - (decimal)cancelreceipt.GrandTotalSys;
                            }
                            else
                            {
                                journalEntryDetail.Add(new JournalEntryDetail
                                {
                                    JEID = journalEntry.ID,
                                    Type = Financials.Type.BPCode,
                                    ItemID = pay.AccountID,
                                    Debit = TotalAmount - (decimal)cancelreceipt.GrandTotalSys
                                });

                                glaccdata.Balance += TotalAmount - (decimal)cancelreceipt.GrandTotalSys;
                                accountBalance.Add(new AccountBalance
                                {
                                    JEID = journalEntry.ID,

                                    PostingDate = cancelreceipt.DateIn,
                                    Origin = docType.ID,
                                    OriginNo = cancelreceipt.ReceiptNo,
                                    OffsetAccount = glaccdata.Code,
                                    Details = douTypeID.Name + "-" + glaccdata.Code,
                                    CumulativeBalance = glaccdata.Balance,
                                    Debit = TotalAmount - (decimal)cancelreceipt.GrandTotalSys,
                                    LocalSetRate = TotalAmount - (decimal)cancelreceipt.GrandTotalSys,
                                    GLAID = pay.AccountID,
                                    Creator = cancelreceipt.UserOrderID,
                                });
                            }
                            _context.Update(glaccdata);
                        }

                    }


                    returncardPay = _context.MultiPaymentMeans.FirstOrDefault(x => x.ReceiptID == receiptid && x.Type == PaymentMeanType.CardMember) ?? new MultiPaymentMeans();
                    if (returncardPay != null)
                    {
                        //Card Member
                        var cardMember = _context.AccountMemberCards.FirstOrDefault(i => i.CashAccID > 0 && i.UnearnedRevenueID > 0) ?? new AccountMemberCard();
                        var cashAcc = _context.GLAccounts.FirstOrDefault(w => w.ID == cardMember.UnearnedRevenueID);
                        if (cashAcc.ID > 0)
                        {
                            var cardjur = journalEntryDetail.FirstOrDefault(w => w.ItemID == cashAcc.ID) ?? new JournalEntryDetail();
                            if (cardjur.ItemID > 0)
                            {
                                var accBalance = accountBalance.FirstOrDefault(w => w.GLAID == cashAcc.ID);
                                cashAcc.Balance -= returncardPay.Amount * returncardPay.SCRate;
                                //journalEntryDetail
                                cardjur.Credit += returncardPay.Amount * returncardPay.SCRate;
                                //accountBalance
                                accBalance.CumulativeBalance = cashAcc.Balance;
                                accBalance.Credit += returncardPay.Amount * returncardPay.SCRate;
                            }
                            else
                            {
                                cashAcc.Balance -= returncardPay.Amount * returncardPay.SCRate;
                                journalEntryDetail.Add(new JournalEntryDetail
                                {
                                    JEID = journalEntry.ID,
                                    Type = Type.GLAcct,
                                    ItemID = cashAcc.ID,
                                    Credit = returncardPay.Amount * returncardPay.SCRate,
                                });
                                //
                                accountBalance.Add(new AccountBalance
                                {
                                    JEID = journalEntry.ID,

                                    PostingDate = cancelreceipt.DateIn,
                                    Origin = docType.ID,
                                    OriginNo = cancelreceipt.ReceiptNo,
                                    OffsetAccount = cashAcc.Code,
                                    Details = douTypeID.Name + " - " + cashAcc.Code,
                                    CumulativeBalance = cashAcc.Balance,
                                    Credit = returncardPay.Amount * returncardPay.SCRate,
                                    LocalSetRate = (decimal)cancelreceipt.LocalSetRate,
                                    GLAID = cashAcc.ID,
                                });
                            }
                            _context.Update(cashAcc);
                        }
                        var customer = _context.BusinessPartners.Find(cancelreceipt.CustomerID);
                        if (customer != null)
                        {
                            customer.Balance -= cancelreceipt.OtherPaymentGrandTotal;
                            //CardMemberDepositTransactions
                            CardMemberDepositTransaction cmdt = new()
                            {
                                Amount = cancelreceipt.OtherPaymentGrandTotal * -1,
                                CardMemberDepositID = 0,
                                CardMemberID = customer.CardMemberID,
                                CumulativeBalance = customer.Balance,
                                CusID = customer.ID,
                                ID = 0,
                                DocTypeID = docType.ID,
                                Number = cancelreceipt.ReceiptNo,
                                PostingDate = cancelreceipt.DateIn,
                                SeriesDID = cancelreceipt.SeriesDID,
                                SeriesID = cancelreceipt.SeriesID,
                                UserID = cancelreceipt.UserOrderID,
                            };
                            _context.BusinessPartners.Update(customer);
                            _context.CardMemberDepositTransactions.Update(cmdt);
                            _context.SaveChanges();
                        }

                    }
                }


                //===========================return creditmeo multipaymenent============
                var incomAccReceive = _context.BusinessPartners.FirstOrDefault(w => w.ID == cancelreceipt.CustomerID);
                var glAccD = _context.GLAccounts.FirstOrDefault(w => w.ID == incomAccReceive.GLAccID);
                ////BPacc
                glAccD.Balance += (decimal)cancelreceipt.GrandTotalSys;
                journalEntryDetail.Add(new JournalEntryDetail
                {
                    JEID = journalEntry.ID,
                    Type = Type.BPCode,
                    ItemID = incomAccReceive.GLAccID,
                    Debit = (decimal)cancelreceipt.GrandTotalSys,
                    BPAcctID = cancelreceipt.CustomerID,
                });
                glAccD.Balance += (decimal)cancelreceipt.GrandTotalSys;
                accountBalance.Add(new AccountBalance
                {
                    JEID = journalEntry.ID,

                    PostingDate = cancelreceipt.DateIn,
                    Origin = docType.ID,
                    OriginNo = cancelreceipt.ReceiptNo,
                    OffsetAccount = glAccD.Code,
                    Details = douTypeID.Name + " - " + glAccD.Code,
                    CumulativeBalance = glAccD.Balance,
                    Debit = (decimal)cancelreceipt.GrandTotalSys,
                    LocalSetRate = (decimal)cancelreceipt.LocalSetRate,
                    GLAID = incomAccReceive.GLAccID,
                    BPAcctID = cancelreceipt.CustomerID,
                    Creator = cancelreceipt.UserOrderID,
                });
                _context.Update(glAcc);


                if (type == "return")
                {
                    returnMultipay.Amount = (decimal)cancelreceipt.SubTotal;
                    returnMultipay.PaymentMeanID = cancelreceipt.PaymentMeansID;
                    multi.Add(returnMultipay);

                }
                else
                    multi = _context.MultiPaymentMeans.Where(x => x.ReceiptID == receiptid && x.Type == 0).ToList();
                foreach (var data in multi)
                {
                    var accountSSelect = _context.PaymentMeans.Find(data.PaymentMeanID);
                    var glAccC = _context.GLAccounts.FirstOrDefault(w => w.ID == accountSSelect.AccountID) ?? new GLAccount();

                    if (glAccC.ID > 0 && data.Amount > 0)
                    {
                        var vatAmountJur = journalEntryDetail.FirstOrDefault(w => w.ItemID == glAccC.ID) ?? new JournalEntryDetail();
                        if (vatAmountJur.ItemID > 0)
                        {
                            var accBalance = accountBalance.FirstOrDefault(w => w.GLAID == glAccC.ID);
                            glAccC.Balance -= type == "return" ? data.Amount : data.Amount * (decimal)data.SCRate;
                            //journalEntryDetail
                            vatAmountJur.Credit += type == "return" ? data.Amount : data.Amount * (decimal)data.SCRate;
                            //accountBalance
                            accBalance.CumulativeBalance = glAccC.Balance;
                            accBalance.Credit += type == "return" ? data.Amount : data.Amount * (decimal)data.SCRate;
                        }
                        else
                        {
                            journalEntryDetail.Add(new JournalEntryDetail
                            {
                                JEID = journalEntry.ID,
                                Type = Financials.Type.BPCode,
                                ItemID = accountSSelect.AccountID,
                                Credit = type == "return" ? data.Amount : data.Amount * (decimal)data.SCRate,
                            });

                            glAccC.Balance -= type == "return" ? data.Amount : data.Amount * (decimal)data.SCRate;
                            accountBalance.Add(new AccountBalance
                            {
                                JEID = journalEntry.ID,

                                PostingDate = cancelreceipt.DateIn,
                                Origin = docType.ID,
                                OriginNo = cancelreceipt.ReceiptNo,
                                OffsetAccount = glAccC.Code,
                                Details = douTypeID.Name + "-" + glAccC.Code,
                                CumulativeBalance = glAccC.Balance,
                                Credit = type == "return" ? data.Amount : data.Amount * (decimal)data.SCRate,
                                LocalSetRate = type == "return" ? data.Amount : data.Amount * (decimal)data.SCRate,
                                GLAID = accountSSelect.AccountID,
                                Creator = cancelreceipt.UserOrderID,
                            });

                        }
                        _context.Update(glAccC);
                    }
                }



                //============================end return creditmemo multipayment==========
                // VAT Invoice
                var taxgInvoice = _context.TaxGroups.Find(cancelreceipt.TaxGroupID) ?? new TaxGroup();
                var taxAccInVoice = _context.GLAccounts.Find(taxgInvoice.GLID) ?? new GLAccount();
                decimal taxValueInvoice = (decimal)(cancelreceipt.TaxValue * cancelreceipt.ExchangeRate);
                if (taxAccInVoice.ID > 0)
                {
                    var taxjur = journalEntryDetail.FirstOrDefault(w => w.ItemID == taxAccInVoice.ID) ?? new JournalEntryDetail();
                    if (taxjur.ItemID > 0)
                    {
                        var accBalance = accountBalance.FirstOrDefault(w => w.GLAID == taxAccInVoice.ID);
                        taxAccInVoice.Balance += taxValueInvoice;
                        //journalEntryDetail
                        taxjur.Debit += taxValueInvoice;
                        //accountBalance
                        accBalance.CumulativeBalance = taxAccInVoice.Balance;
                        accBalance.Debit += taxValueInvoice;
                    }
                    else
                    {
                        taxAccInVoice.Balance += taxValueInvoice;
                        journalEntryDetail.Add(new JournalEntryDetail
                        {
                            JEID = journalEntry.ID,
                            Type = Type.GLAcct,
                            ItemID = taxAccInVoice.ID,
                            Debit = taxValueInvoice,
                        });
                        //
                        accountBalance.Add(new AccountBalance
                        {
                            JEID = journalEntry.ID,

                            PostingDate = cancelreceipt.DateIn,
                            Origin = docType.ID,
                            OriginNo = cancelreceipt.ReceiptNo,
                            OffsetAccount = taxAccInVoice.Code,
                            Details = douTypeID.Name + " - " + taxAccInVoice.Code,
                            CumulativeBalance = taxAccInVoice.Balance,
                            Debit = taxValueInvoice,
                            LocalSetRate = (decimal)cancelreceipt.LocalSetRate,
                            GLAID = taxAccInVoice.ID,
                        });
                    }
                    _context.Update(taxAccInVoice);
                }
                _context.SaveChanges();
                foreach (var receipt in receiptdetail.ToList())
                {
                    int revenueAccID = 0, inventoryAccID = 0, COGSAccID = 0;
                    decimal revenueAccAmount = 0, inventoryAccAmount = 0, COGSAccAmount = 0;
                    var itemMaster = _context.ItemMasterDatas.FirstOrDefault(w => w.ID == receipt.ItemID);
                    var orft = _context.GroupDUoMs.FirstOrDefault(w => w.GroupUoMID == itemMaster.GroupUomID && w.AltUOM == receipt.UomID);
                    decimal disvalue = (decimal)receipt.TotalSys * ((decimal)(cancelreceipt.DisRate + cancelreceipt.PromoCodeDiscRate) + cancelreceipt.BuyXAmGetXDisRate + cancelreceipt.CardMemberDiscountRate) / 100;
                    var _itemAcc = _context.ItemAccountings.FirstOrDefault(i => i.WarehouseID == cancelreceipt.WarehouseID && i.ItemID == receipt.ItemID);
                    if (itemMaster.SetGlAccount == SetGlAccount.ItemLevel)
                    {
                        var revenueAcc = (from ia in _context.ItemAccountings.Where(w => w.ItemID == receipt.ItemID && w.WarehouseID == cancelreceipt.WarehouseID)
                                          join gl in _context.GLAccounts on ia.RevenueAccount equals gl.Code
                                          select gl
                                             ).FirstOrDefault() ?? new GLAccount();
                        var inventoryAcc = (from ia in _context.ItemAccountings.Where(w => w.ItemID == receipt.ItemID && w.WarehouseID == cancelreceipt.WarehouseID)
                                            join gl in _context.GLAccounts on ia.InventoryAccount equals gl.Code
                                            select gl
                                             ).FirstOrDefault() ?? new GLAccount();
                        var COGSAcc = (from ia in _context.ItemAccountings.Where(w => w.ItemID == receipt.ItemID && w.WarehouseID == cancelreceipt.WarehouseID)
                                       join gl in _context.GLAccounts on ia.CostofGoodsSoldAccount equals gl.Code
                                       select gl
                                             ).FirstOrDefault() ?? new GLAccount();
                        revenueAccID = revenueAcc.ID;
                        inventoryAccID = inventoryAcc.ID;
                        COGSAccID = COGSAcc.ID;
                        if (cancelreceipt.TaxOption == TaxOptions.Exclude)
                        {
                            decimal total = (decimal)receipt.TotalSys - receipt.TaxValue;
                            disvalue = total * ((decimal)(cancelreceipt.DisRate + cancelreceipt.PromoCodeDiscRate) + cancelreceipt.BuyXAmGetXDisRate + cancelreceipt.CardMemberDiscountRate) / 100;
                            revenueAccAmount = total - disvalue;
                        }
                        else
                        {
                            revenueAccAmount = (decimal)receipt.TotalSys - disvalue - (receipt.TaxValue * (decimal)cancelreceipt.ExchangeRate);
                        }
                    }
                    else if (itemMaster.SetGlAccount == SetGlAccount.ItemGroup)
                    {
                        var revenueAcc = (from ia in _context.ItemAccountings.Where(w => w.ItemGroupID == itemMaster.ItemGroup1ID)
                                          join gl in _context.GLAccounts on ia.RevenueAccount equals gl.Code
                                          select gl
                                             ).FirstOrDefault() ?? new GLAccount();
                        var inventoryAcc = (from ia in _context.ItemAccountings.Where(w => w.ItemGroupID == itemMaster.ItemGroup1ID)
                                            join gl in _context.GLAccounts on ia.InventoryAccount equals gl.Code
                                            select gl
                                             ).FirstOrDefault() ?? new GLAccount();
                        var COGSAcc = (from ia in _context.ItemAccountings.Where(w => w.ItemGroupID == itemMaster.ItemGroup1ID)
                                       join gl in _context.GLAccounts on ia.CostofGoodsSoldAccount equals gl.Code
                                       select gl
                                             ).FirstOrDefault() ?? new GLAccount();
                        revenueAccID = revenueAcc.ID;
                        inventoryAccID = inventoryAcc.ID;
                        COGSAccID = COGSAcc.ID;
                        if (cancelreceipt.TaxOption == TaxOptions.Exclude)
                        {
                            decimal total = (decimal)receipt.TotalSys - receipt.TaxValue;
                            disvalue = total * ((decimal)(cancelreceipt.DisRate + cancelreceipt.PromoCodeDiscRate) + cancelreceipt.BuyXAmGetXDisRate + cancelreceipt.CardMemberDiscountRate) / 100;
                            revenueAccAmount = total - disvalue;
                        }
                        else
                        {
                            revenueAccAmount = (decimal)receipt.TotalSys - disvalue - (receipt.TaxValue * (decimal)cancelreceipt.ExchangeRate);
                        }
                    }
                    if (receipt.IsKsms == false && receipt.IsKsmsMaster == false)
                    {
                        if (itemMaster.Process != "Standard")
                        {
                            var receiptOld = _context.Receipt.Find(cancelreceipt.BasedOn);
                            var waredetail = _context.WarehouseDetails.Where(w => w.WarehouseID == cancelreceipt.WarehouseID && w.ItemID == receipt.ItemID).ToList();
                            var item_cost = _context.InventoryAudits.FirstOrDefault(w => w.SeriesDetailID == receiptOld.SeriesDID && w.ItemID == receipt.ItemID);
                            //update_warehouse_summary && itemmasterdata
                            var warehouseSummary = _context.WarehouseSummary.FirstOrDefault(w => w.ItemID == receipt.ItemID && w.WarehouseID == cancelreceipt.WarehouseID);
                            double @Qty = receipt.Qty * orft.Factor;
                            double @Cost = item_cost.Cost;
                            warehouseSummary.InStock += @Qty;
                            itemMaster.StockIn += @Qty;
                            _utility.UpdateItemAccounting(_itemAcc, warehouseSummary);
                            if (itemMaster.ManItemBy == ManageItemBy.SerialNumbers)
                            {
                                if (serials.Count > 0)
                                {
                                    List<WareForAudiView> wareForAudis = new();
                                    foreach (var s in serials)
                                    {
                                        if (s.SerialNumberSelected != null)
                                        {
                                            foreach (var ss in s.SerialNumberSelected.SerialNumberSelectedDetails)
                                            {
                                                decimal _inventoryAccAmount = 0M;
                                                decimal _COGSAccAmount = 0M;
                                                StockOut stockout = _context.StockOuts
                                                .FirstOrDefault(i =>
                                                i.ItemID == receipt.ItemID
                                                && ss.SerialNumber == i.SerialNumber
                                                && i.InStock > 0 && i.TransType == TransTypeWD.POS);
                                                if (stockout != null)
                                                {
                                                    stockout.InStock -= 1;
                                                    // insert to warehouse detail
                                                    var ware = new WarehouseDetail
                                                    {
                                                        AdmissionDate = stockout.AdmissionDate,
                                                        Cost = (double)stockout.Cost,
                                                        CurrencyID = stockout.CurrencyID,
                                                        Details = stockout.Details,
                                                        ID = 0,
                                                        InStock = 1,
                                                        ItemID = stockout.ItemID,
                                                        Location = stockout.Location,
                                                        LotNumber = stockout.LotNumber,
                                                        MfrDate = stockout.MfrDate,
                                                        MfrSerialNumber = stockout.MfrSerialNumber,
                                                        MfrWarDateEnd = stockout.MfrWarDateEnd,
                                                        MfrWarDateStart = stockout.MfrWarDateStart,
                                                        ProcessItem = ProcessItem.SEBA,
                                                        SerialNumber = stockout.SerialNumber,
                                                        SyetemDate = DateTime.Now,
                                                        SysNum = 0,
                                                        TimeIn = DateTime.Now,
                                                        WarehouseID = stockout.WarehouseID,
                                                        UomID = receipt.UomID,
                                                        UserID = cancelreceipt.UserOrderID,
                                                        ExpireDate = (DateTime)stockout.ExpireDate,
                                                        TransType = TransTypeWD.POS,
                                                        InStockFrom = cancelreceipt.ID,
                                                        BPID = cancelreceipt.CustomerID,
                                                        IsDeleted = true,
                                                    };
                                                    wareForAudis.Add(new WareForAudiView
                                                    {
                                                        Cost = ware.Cost,
                                                        Qty = ware.InStock,
                                                        ExpireDate = ware.ExpireDate,
                                                    });
                                                    @Cost = ware.Cost;
                                                    _inventoryAccAmount = stockout.Cost;
                                                    _COGSAccAmount = stockout.Cost;
                                                    _context.WarehouseDetails.Add(ware);
                                                    _context.SaveChanges();
                                                }
                                                InsertFinancialReceiptMemo(
                                                    inventoryAccID, COGSAccID, _inventoryAccAmount, _COGSAccAmount,
                                                    journalEntryDetail, accountBalance, journalEntry,
                                                    cancelreceipt, docType, douTypeID, OffsetAcc, receipt
                                                );
                                            }
                                        }
                                    }
                                    // Insert to Inventory Audit
                                    wareForAudis = (from wa in wareForAudis
                                                    group wa by wa.Cost into g
                                                    let wha = g.FirstOrDefault()
                                                    select new WareForAudiView
                                                    {
                                                        Qty = g.Sum(i => i.Qty),
                                                        Cost = wha.Cost,
                                                        ExpireDate = wha.ExpireDate,
                                                    }).ToList();
                                    if (wareForAudis.Any())
                                    {
                                        foreach (var i in wareForAudis)
                                        {
                                            var inventory_audit = _context.InventoryAudits.Where(w => w.ItemID == receipt.ItemID && w.WarehouseID == cancelreceipt.WarehouseID).ToList();
                                            var inventory = new InventoryAudit
                                            {
                                                ID = 0,
                                                WarehouseID = cancelreceipt.WarehouseID,
                                                BranchID = cancelreceipt.BranchID,
                                                UserID = cancelreceipt.UserOrderID,
                                                ItemID = receipt.ItemID,
                                                CurrencyID = cancelreceipt.SysCurrencyID,
                                                UomID = orft.BaseUOM,
                                                InvoiceNo = cancelreceipt.ReceiptNo,
                                                Trans_Type = docType.Code,
                                                Process = itemMaster.Process,
                                                SystemDate = DateTime.Now,
                                                TimeIn = DateTime.Now.ToShortTimeString().ToString(),
                                                Qty = i.Qty,
                                                Cost = i.Cost,
                                                Price = 0,
                                                CumulativeQty = inventory_audit.Sum(q => q.Qty) + i.Qty,
                                                CumulativeValue = inventory_audit.Sum(s => s.Trans_Valuse) + i.Qty * i.Cost,
                                                Trans_Valuse = i.Cost * i.Qty,
                                                ExpireDate = i.ExpireDate,
                                                LocalCurID = cancelreceipt.LocalCurrencyID,
                                                LocalSetRate = (double)cancelreceipt.LocalSetRate,
                                                CompanyID = cancelreceipt.CompanyID,
                                                DocumentTypeID = docType.ID,
                                                SeriesID = cancelreceipt.SeriesID,
                                                SeriesDetailID = cancelreceipt.SeriesDID,
                                            };
                                            _utility.CumulativeValue(inventory.WarehouseID, inventory.ItemID, inventory.CumulativeValue, _itemAcc);
                                            _context.InventoryAudits.Add(inventory);
                                            _context.SaveChanges();
                                        }
                                    }
                                }
                            }
                            else if (itemMaster.ManItemBy == ManageItemBy.Batches)
                            {
                                if (batches.Count > 0)
                                {
                                    List<WareForAudiView> wareForAudis = new();
                                    foreach (var b in batches)
                                    {
                                        if (b.BatchNoSelected != null)
                                        {
                                            foreach (var sb in b.BatchNoSelected.BatchNoSelectedDetails)
                                            {
                                                var waredetial = _context.StockOuts
                                                    .FirstOrDefault(i =>
                                                    i.ItemID == receipt.ItemID
                                                    && sb.BatchNo == i.BatchNo
                                                    && i.TransType == TransTypeWD.POS && i.InStock > 0);
                                                decimal _inventoryAccAmount = 0M;
                                                decimal _COGSAccAmount = 0M;
                                                if (waredetial != null)
                                                {
                                                    decimal selectedQty = sb.SelectedQty * (decimal)orft.Factor;
                                                    waredetial.InStock -= selectedQty;
                                                    _context.SaveChanges();

                                                    // insert to waredetial
                                                    var ware = new WarehouseDetail
                                                    {
                                                        AdmissionDate = waredetial.AdmissionDate,
                                                        Cost = (double)waredetial.Cost,
                                                        CurrencyID = waredetial.CurrencyID,
                                                        Details = waredetial.Details,
                                                        ID = 0,
                                                        InStock = (double)selectedQty,
                                                        ItemID = receipt.ItemID,
                                                        Location = waredetial.Location,
                                                        MfrDate = waredetial.MfrDate,
                                                        ProcessItem = ProcessItem.SEBA,
                                                        SyetemDate = DateTime.Now,
                                                        SysNum = 0,
                                                        TimeIn = DateTime.Now,
                                                        WarehouseID = waredetial.WarehouseID,
                                                        UomID = receipt.UomID,
                                                        UserID = cancelreceipt.UserOrderID,
                                                        ExpireDate = (DateTime)waredetial.ExpireDate,
                                                        BatchAttr1 = waredetial.BatchAttr1,
                                                        BatchAttr2 = waredetial.BatchAttr2,
                                                        BatchNo = waredetial.BatchNo,
                                                        TransType = TransTypeWD.ReturnPOS,
                                                        InStockFrom = cancelreceipt.ID,
                                                        BPID = cancelreceipt.CustomerID,
                                                        IsDeleted = true
                                                    };
                                                    @Cost = ware.Cost;
                                                    wareForAudis.Add(new WareForAudiView
                                                    {
                                                        Cost = ware.Cost,
                                                        Qty = ware.InStock,
                                                        ExpireDate = ware.ExpireDate,
                                                    });
                                                    _inventoryAccAmount = waredetial.Cost * selectedQty;
                                                    _COGSAccAmount = waredetial.Cost * selectedQty;
                                                    _context.WarehouseDetails.Add(ware);
                                                    _context.SaveChanges();
                                                }
                                                InsertFinancialReceiptMemo(
                                                    inventoryAccID, COGSAccID, _inventoryAccAmount, _COGSAccAmount,
                                                    journalEntryDetail, accountBalance, journalEntry,
                                                    cancelreceipt, docType, douTypeID, OffsetAcc, receipt
                                                );
                                            }
                                        }
                                    }
                                    // insert to inventory audit
                                    wareForAudis = (from wa in wareForAudis
                                                    group wa by wa.Cost into g
                                                    let wha = g.FirstOrDefault()
                                                    select new WareForAudiView
                                                    {
                                                        Qty = g.Sum(i => i.Qty),
                                                        Cost = wha.Cost,
                                                        ExpireDate = wha.ExpireDate,
                                                    }).ToList();
                                    if (wareForAudis.Any())
                                    {
                                        foreach (var i in wareForAudis)
                                        {
                                            var inventory_audit = _context.InventoryAudits.Where(w => w.ItemID == receipt.ItemID && w.WarehouseID == cancelreceipt.WarehouseID).ToList();
                                            var inventory = new InventoryAudit
                                            {
                                                ID = 0,
                                                WarehouseID = cancelreceipt.WarehouseID,
                                                BranchID = cancelreceipt.BranchID,
                                                UserID = cancelreceipt.UserOrderID,
                                                ItemID = receipt.ItemID,
                                                CurrencyID = cancelreceipt.SysCurrencyID,
                                                UomID = orft.BaseUOM,
                                                InvoiceNo = cancelreceipt.ReceiptNo,
                                                Trans_Type = docType.Code,
                                                Process = itemMaster.Process,
                                                SystemDate = DateTime.Now,
                                                TimeIn = DateTime.Now.ToShortTimeString().ToString(),
                                                Qty = i.Qty,
                                                Cost = i.Cost,
                                                Price = 0,
                                                CumulativeQty = inventory_audit.Sum(q => q.Qty) + i.Qty,
                                                CumulativeValue = inventory_audit.Sum(s => s.Trans_Valuse) + (i.Qty * i.Cost),
                                                Trans_Valuse = i.Qty * i.Cost,
                                                ExpireDate = i.ExpireDate,
                                                LocalCurID = cancelreceipt.LocalCurrencyID,
                                                LocalSetRate = (double)cancelreceipt.LocalSetRate,
                                                CompanyID = cancelreceipt.CompanyID,
                                                DocumentTypeID = docType.ID,
                                                SeriesID = cancelreceipt.SeriesID,
                                                SeriesDetailID = cancelreceipt.SeriesDID,
                                            };
                                            _utility.CumulativeValue(inventory.WarehouseID, inventory.ItemID, inventory.CumulativeValue, _itemAcc);
                                            _context.InventoryAudits.Add(inventory);
                                            _context.SaveChanges();
                                        }
                                    }
                                }
                            }
                            else
                            {
                                //insert_warehousedetail
                                var inventoryAudit = new InventoryAudit();
                                var warehouseDetail = new WarehouseDetail();
                                warehouseDetail.WarehouseID = cancelreceipt.WarehouseID;
                                warehouseDetail.UomID = receipt.UomID;
                                warehouseDetail.UserID = cancelreceipt.UserOrderID;
                                warehouseDetail.SyetemDate = cancelreceipt.DateOut;
                                warehouseDetail.TimeIn = DateTime.Now;
                                warehouseDetail.InStock = @Qty;
                                warehouseDetail.CurrencyID = cancelreceipt.SysCurrencyID;
                                warehouseDetail.ItemID = receipt.ItemID;
                                warehouseDetail.Cost = @Cost;
                                warehouseDetail.InStockFrom = cancelreceipt.ID;
                                warehouseDetail.IsDeleted = true;
                                warehouseDetail.BPID = cancelreceipt.CustomerID;
                                if (itemMaster.Process == "FIFO")
                                {
                                    var inventory_audit = _context.InventoryAudits.Where(w => w.ItemID == receipt.ItemID && w.WarehouseID == cancelreceipt.WarehouseID).ToList();
                                    inventoryAudit.ID = 0;
                                    inventoryAudit.WarehouseID = cancelreceipt.WarehouseID;
                                    inventoryAudit.BranchID = cancelreceipt.BranchID;
                                    inventoryAudit.UserID = cancelreceipt.UserOrderID;
                                    inventoryAudit.ItemID = receipt.ItemID;
                                    inventoryAudit.CurrencyID = cancelreceipt.SysCurrencyID;
                                    inventoryAudit.UomID = receipt.UomID;
                                    inventoryAudit.InvoiceNo = cancelreceipt.ReceiptNo;
                                    inventoryAudit.Trans_Type = docType.Code;
                                    inventoryAudit.Process = itemMaster.Process;
                                    inventoryAudit.SystemDate = DateTime.Now;
                                    inventoryAudit.TimeIn = DateTime.Now.ToShortTimeString();
                                    inventoryAudit.Qty = @Qty;
                                    inventoryAudit.Cost = @Cost;
                                    inventoryAudit.Price = 0;
                                    inventoryAudit.CumulativeQty = inventory_audit.Sum(q => q.Qty) + @Qty;
                                    inventoryAudit.CumulativeValue = inventory_audit.Sum(s => s.Trans_Valuse) + (@Qty * @Cost);
                                    inventoryAudit.Trans_Valuse = @Qty * @Cost;
                                    inventoryAudit.LocalCurID = cancelreceipt.LocalCurrencyID;
                                    inventoryAudit.LocalSetRate = cancelreceipt.LocalSetRate;
                                    inventoryAudit.CompanyID = cancelreceipt.CompanyID;
                                    inventoryAudit.DocumentTypeID = docType.ID;
                                    inventoryAudit.SeriesID = cancelreceipt.SeriesID;
                                    inventoryAudit.SeriesDetailID = cancelreceipt.SeriesDID;
                                    //
                                    inventoryAccAmount += (decimal)inventoryAudit.Cost * (decimal)@Qty;
                                    COGSAccAmount += (decimal)inventoryAudit.Cost * (decimal)@Qty;
                                    _utility.CumulativeValue(inventoryAudit.WarehouseID, inventoryAudit.ItemID, inventoryAudit.CumulativeValue, _itemAcc);
                                }
                                else if (itemMaster.Process == "Average")
                                {
                                    var inventory_audit = _context.InventoryAudits.Where(w => w.ItemID == receipt.ItemID && w.WarehouseID == cancelreceipt.WarehouseID).ToList();
                                    InventoryAudit avgInventory = new() { Cost = @Cost, Qty = Qty };
                                    double @AvgCost = _utility.CalAVGCost(receipt.ItemID, cancelreceipt.WarehouseID, avgInventory);
                                    inventoryAudit.ID = 0;
                                    inventoryAudit.WarehouseID = cancelreceipt.WarehouseID;
                                    inventoryAudit.BranchID = cancelreceipt.BranchID;
                                    inventoryAudit.UserID = cancelreceipt.UserOrderID;
                                    inventoryAudit.ItemID = receipt.ItemID;
                                    inventoryAudit.CurrencyID = cancelreceipt.SysCurrencyID;
                                    inventoryAudit.UomID = receipt.UomID;
                                    inventoryAudit.InvoiceNo = cancelreceipt.ReceiptNo;
                                    inventoryAudit.Trans_Type = docType.Code;
                                    inventoryAudit.Process = itemMaster.Process;
                                    inventoryAudit.SystemDate = DateTime.Now;
                                    inventoryAudit.TimeIn = DateTime.Now.ToShortTimeString();
                                    inventoryAudit.Qty = @Qty;
                                    inventoryAudit.Cost = @AvgCost;
                                    inventoryAudit.Price = 0;
                                    inventoryAudit.CumulativeQty = inventory_audit.Sum(q => q.Qty) + @Qty;
                                    inventoryAudit.CumulativeValue = inventory_audit.Sum(s => s.Trans_Valuse) + (@Qty * @AvgCost);
                                    inventoryAudit.Trans_Valuse = @Qty * @AvgCost;
                                    inventoryAudit.LocalCurID = cancelreceipt.LocalCurrencyID;
                                    inventoryAudit.LocalSetRate = cancelreceipt.LocalSetRate;
                                    inventoryAudit.CompanyID = cancelreceipt.CompanyID;
                                    inventoryAudit.DocumentTypeID = docType.ID;
                                    inventoryAudit.SeriesID = cancelreceipt.SeriesID;
                                    inventoryAudit.SeriesDetailID = cancelreceipt.SeriesDID;
                                    double avgcost = _utility.CalAVGCost(receipt.ItemID, cancelreceipt.WarehouseID, inventoryAudit);
                                    inventoryAccAmount += (decimal)(avgcost * @Qty);
                                    COGSAccAmount += (decimal)(avgcost * @Qty);
                                    _utility.UpdateAvgCost(receipt.ItemID, cancelreceipt.WarehouseID, itemMaster.GroupUomID, @Qty, @AvgCost);
                                    _utility.UpdateBomCost(receipt.ItemID, @Qty, @AvgCost);
                                    _utility.CumulativeValue(inventoryAudit.WarehouseID, inventoryAudit.ItemID, inventoryAudit.CumulativeValue, _itemAcc);
                                }
                                _context.InventoryAudits.Update(inventoryAudit);
                                _context.WarehouseSummary.Update(warehouseSummary);
                                _context.ItemMasterDatas.Update(itemMaster);
                                _context.WarehouseDetails.Update(warehouseDetail);
                                _context.SaveChanges();
                            }
                        }
                        else
                        {
                            var priceListDetail = _context.PriceListDetails.FirstOrDefault(w => w.ItemID == receipt.ItemID && w.UomID == receipt.UomID && w.PriceListID == cancelreceipt.PriceListID) ?? new Inventory.PriceList.PriceListDetail();
                            inventoryAccAmount += (decimal)priceListDetail.Cost * (decimal)receipt.Qty * (decimal)cancelreceipt.ExchangeRate;
                            COGSAccAmount += (decimal)priceListDetail.Cost * (decimal)receipt.Qty * (decimal)cancelreceipt.ExchangeRate;
                            var inventory_audit = _context.InventoryAudits.Where(w => w.ItemID == receipt.ItemID && w.WarehouseID == cancelreceipt.WarehouseID).ToList();
                            InventoryAudit item_inventory_audit = new()
                            {
                                ID = 0,
                                WarehouseID = cancelreceipt.WarehouseID,
                                BranchID = cancelreceipt.BranchID,
                                UserID = cancelreceipt.UserOrderID,
                                ItemID = receipt.ItemID,
                                CurrencyID = Com.SystemCurrencyID,
                                UomID = orft.BaseUOM,
                                InvoiceNo = cancelreceipt.ReceiptNo,
                                Trans_Type = docType.Code,
                                Process = itemMaster.Process,
                                SystemDate = DateTime.Now,
                                TimeIn = DateTime.Now.ToShortTimeString().ToString(),
                                Qty = receipt.Qty,
                                Cost = priceListDetail.Cost,
                                Price = 0,
                                CumulativeQty = inventory_audit.Sum(q => q.Qty) + receipt.Qty,
                                CumulativeValue = inventory_audit.Sum(s => s.Trans_Valuse) + (receipt.Qty * priceListDetail.Cost),
                                Trans_Valuse = receipt.Qty * priceListDetail.Cost,
                                //ExpireDate = item_IssusStock.ExpireDate,
                                LocalCurID = cancelreceipt.LocalCurrencyID,
                                LocalSetRate = cancelreceipt.LocalSetRate,
                                CompanyID = cancelreceipt.CompanyID,
                                DocumentTypeID = docType.ID,
                                SeriesID = cancelreceipt.SeriesID,
                                SeriesDetailID = cancelreceipt.SeriesDID,
                                TypeItem = "Standard",
                            };
                            _context.InventoryAudits.Update(item_inventory_audit);
                            _context.SaveChanges();
                        }
                    }

                    if (!receipt.IsKsms && !receipt.IsReadonly)
                    {
                        // Tax Account ///
                        var taxg = _context.TaxGroups.Find(receipt.TaxGroupID) ?? new TaxGroup();
                        var taxAcc = _context.GLAccounts.Find(taxg.GLID) ?? new GLAccount();
                        decimal taxValue = receipt.TaxValue * (decimal)cancelreceipt.ExchangeRate;
                        if (taxAcc.ID > 0)
                        {
                            var taxjur = journalEntryDetail.FirstOrDefault(w => w.ItemID == taxAcc.ID) ?? new JournalEntryDetail();
                            if (taxjur.ItemID > 0)
                            {
                                var accBalance = accountBalance.FirstOrDefault(w => w.GLAID == taxAcc.ID);
                                taxAcc.Balance += taxValue;
                                //journalEntryDetail
                                taxjur.Debit += taxValue;
                                //accountBalance
                                accBalance.CumulativeBalance = taxAcc.Balance;
                                accBalance.Debit += taxValue;
                            }
                            else
                            {
                                taxAcc.Balance += taxValue;
                                journalEntryDetail.Add(new JournalEntryDetail
                                {
                                    JEID = journalEntry.ID,
                                    Type = Type.GLAcct,
                                    ItemID = taxAcc.ID,
                                    Debit = taxValue,
                                });
                                //
                                accountBalance.Add(new AccountBalance
                                {
                                    JEID = journalEntry.ID,

                                    PostingDate = cancelreceipt.DateIn,
                                    Origin = docType.ID,
                                    OriginNo = cancelreceipt.ReceiptNo,
                                    OffsetAccount = taxAcc.Code,
                                    Details = douTypeID.Name + " - " + taxAcc.Code,
                                    CumulativeBalance = taxAcc.Balance,
                                    Debit = taxValue,
                                    LocalSetRate = (decimal)cancelreceipt.LocalSetRate,
                                    GLAID = taxAcc.ID,
                                });
                            }
                            _context.Update(taxAcc);
                        }
                        // Account Revenue
                        var glAccRevenfifo = _context.GLAccounts.FirstOrDefault(w => w.ID == revenueAccID) ?? new GLAccount();
                        if (glAccRevenfifo.ID > 0)
                        {
                            var list = journalEntryDetail.FirstOrDefault(w => w.ItemID == glAccRevenfifo.ID) ?? new JournalEntryDetail();
                            if (list.ItemID > 0)
                            {
                                var accBalance = accountBalance.FirstOrDefault(w => w.GLAID == revenueAccID);
                                glAccRevenfifo.Balance += revenueAccAmount;
                                //journalEntryDetail
                                list.Debit += revenueAccAmount;
                                //accountBalance
                                accBalance.CumulativeBalance = glAccRevenfifo.Balance;
                                accBalance.Debit += revenueAccAmount;
                            }
                            else
                            {
                                glAccRevenfifo.Balance += revenueAccAmount;
                                journalEntryDetail.Add(new JournalEntryDetail
                                {
                                    JEID = journalEntry.ID,
                                    Type = Type.GLAcct,
                                    ItemID = revenueAccID,
                                    Debit = revenueAccAmount,
                                });
                                //
                                accountBalance.Add(new AccountBalance
                                {
                                    JEID = journalEntry.ID,

                                    PostingDate = cancelreceipt.DateOut,
                                    Origin = docType.ID,
                                    OriginNo = cancelreceipt.ReceiptNo,
                                    OffsetAccount = OffsetAcc,
                                    Details = douTypeID.Name + "-" + glAccRevenfifo.Code,
                                    CumulativeBalance = glAccRevenfifo.Balance,
                                    Debit = revenueAccAmount,
                                    LocalSetRate = (decimal)cancelreceipt.LocalSetRate,
                                    GLAID = revenueAccID,
                                });
                            }
                            _context.Update(glAccRevenfifo);
                        }
                    }
                    if (itemMaster.ManItemBy == ManageItemBy.None || receipt.IsKsmsMaster)
                    {
                        InsertFinancialReceiptMemo(
                            inventoryAccID, COGSAccID, inventoryAccAmount, COGSAccAmount,
                            journalEntryDetail, accountBalance, journalEntry,
                            cancelreceipt, docType, douTypeID, OffsetAcc, receipt
                        );
                    }
                }
                foreach (var item in receiptdetail.ToList())
                {
                    var itemM = _context.ItemMasterDatas.FirstOrDefault(w => w.ID == item.ItemID);
                    var orft = _context.GroupDUoMs.FirstOrDefault(w => w.GroupUoMID == itemM.GroupUomID && w.AltUOM == item.UomID);
                    var bom = _context.BOMaterial.FirstOrDefault(w => w.ItemID == item.ItemID) ?? new BOMaterial();
                    var items_material = (from bomd in _context.BOMDetail.Where(w => w.BID == bom.BID && w.Detele == false)
                                          join i in _context.ItemMasterDatas.Where(im => !im.Delete) on bomd.ItemID equals i.ID
                                          join gd in _context.GroupDUoMs on bomd.UomID equals gd.AltUOM
                                          join uom in _context.UnitofMeasures on i.InventoryUoMID equals uom.ID
                                          select new
                                          {
                                              bomd.ItemID,
                                              gd.GroupUoMID,
                                              GUoMID = i.GroupUomID,
                                              Qty = (double)item.Qty * orft.Factor * ((double)bomd.Qty * gd.Factor),
                                              bomd.NegativeStock,
                                              i.Process,
                                              UomID = uom.ID,
                                              gd.Factor,

                                          }).Where(w => w.GroupUoMID == w.GUoMID).ToList();
                    if (items_material.Count > 0)
                    {
                        foreach (var item_cancel in items_material.ToList())
                        {
                            int inventoryAccIDavg = 0, COGSAccIDavg = 0;
                            decimal inventoryAccAmountavg = 0, COGSAccAmountavg = 0;
                            var receiptno = cancelreceipt.ReceiptNo.Split(" ")[0];
                            var item_warehouse_material = _context.WarehouseSummary.FirstOrDefault(w => w.WarehouseID == cancelreceipt.WarehouseID && w.ItemID == item_cancel.ItemID);
                            var all_item_warehouse_detail = _context.WarehouseDetails.Where(w => w.WarehouseID == cancelreceipt.WarehouseID && w.ItemID == item_cancel.ItemID).ToList();
                            var item_cost = _context.InventoryAudits.FirstOrDefault(w => w.InvoiceNo == receiptno && w.ItemID == item_cancel.ItemID) ?? new InventoryAudit();
                            //
                            //update_warehouse_summary && itemmasterdata
                            var warehouseSummary = _context.WarehouseSummary.FirstOrDefault(w => w.ItemID == item_cancel.ItemID && w.WarehouseID == cancelreceipt.WarehouseID);
                            var itemMaster = _context.ItemMasterDatas.FirstOrDefault(w => w.ID == item_cancel.ItemID);
                            var _itemAcc = _context.ItemAccountings.FirstOrDefault(i => i.WarehouseID == cancelreceipt.WarehouseID && i.ItemID == item_cancel.ItemID);
                            if (itemMaster.SetGlAccount == SetGlAccount.ItemLevel)
                            {
                                var inventoryAcc = (from ia in _context.ItemAccountings.Where(w => w.ItemID == item.ItemID && w.WarehouseID == cancelreceipt.WarehouseID)
                                                    join gl in _context.GLAccounts on ia.InventoryAccount equals gl.Code
                                                    select gl
                                                     ).FirstOrDefault() ?? new GLAccount();
                                var COGSAcc = (from ia in _context.ItemAccountings.Where(w => w.ItemID == item.ItemID && w.WarehouseID == cancelreceipt.WarehouseID)
                                               join gl in _context.GLAccounts on ia.CostofGoodsSoldAccount equals gl.Code
                                               select gl
                                                     ).FirstOrDefault();
                                inventoryAccIDavg = inventoryAcc.ID;
                                COGSAccIDavg = COGSAcc.ID;
                            }
                            else if (itemMaster.SetGlAccount == SetGlAccount.ItemGroup)
                            {
                                var inventoryAcc = (from ia in _context.ItemAccountings.Where(w => w.ItemGroupID == itemMaster.ItemGroup1ID)
                                                    join gl in _context.GLAccounts on ia.InventoryAccount equals gl.Code
                                                    select gl
                                                     ).FirstOrDefault() ?? new GLAccount();
                                var COGSAcc = (from ia in _context.ItemAccountings.Where(w => w.ItemGroupID == itemMaster.ItemGroup1ID)
                                               join gl in _context.GLAccounts on ia.CostofGoodsSoldAccount equals gl.Code
                                               select gl
                                                     ).FirstOrDefault() ?? new GLAccount();
                                inventoryAccIDavg = inventoryAcc.ID;
                                COGSAccIDavg = COGSAcc.ID;
                            }
                            var inventoryAudit = new InventoryAudit();
                            var warehouseDetail = new WarehouseDetail();
                            if (!item.IsKsms && !item.IsKsmsMaster)
                            {
                                double @Qty = item_cancel.Qty;
                                double @Cost = item_cost.Cost;
                                warehouseSummary.InStock += @Qty;
                                itemMaster.StockIn += @Qty;
                                _utility.UpdateItemAccounting(_itemAcc, warehouseSummary);
                                //insert_warehousedetail
                                warehouseDetail.WarehouseID = cancelreceipt.WarehouseID;
                                warehouseDetail.UomID = item_cancel.UomID;
                                warehouseDetail.UserID = cancelreceipt.UserOrderID;
                                warehouseDetail.SyetemDate = cancelreceipt.DateOut;
                                warehouseDetail.TimeIn = DateTime.Now;
                                warehouseDetail.InStock = @Qty;
                                warehouseDetail.CurrencyID = cancelreceipt.SysCurrencyID;
                                warehouseDetail.ItemID = item_cancel.ItemID;
                                warehouseDetail.Cost = @Cost;
                                warehouseDetail.InStockFrom = cancelreceipt.ID;
                                warehouseDetail.IsDeleted = true;
                                warehouseDetail.BPID = cancelreceipt.CustomerID;
                                if (itemMaster.Process == "FIFO")
                                {
                                    var inventory_audit = _context.InventoryAudits.Where(w => w.ItemID == item_cancel.ItemID && w.WarehouseID == cancelreceipt.WarehouseID).ToList();
                                    inventoryAudit.ID = 0;
                                    inventoryAudit.WarehouseID = cancelreceipt.WarehouseID;
                                    inventoryAudit.BranchID = cancelreceipt.BranchID;
                                    inventoryAudit.UserID = cancelreceipt.UserOrderID;
                                    inventoryAudit.ItemID = item_cancel.ItemID;
                                    inventoryAudit.CurrencyID = cancelreceipt.SysCurrencyID;
                                    inventoryAudit.UomID = item_cancel.UomID;
                                    inventoryAudit.InvoiceNo = cancelreceipt.ReceiptNo;
                                    inventoryAudit.Trans_Type = docType.Code;
                                    inventoryAudit.Process = itemMaster.Process;
                                    inventoryAudit.SystemDate = DateTime.Now;
                                    inventoryAudit.TimeIn = DateTime.Now.ToShortTimeString();
                                    inventoryAudit.Qty = @Qty;
                                    inventoryAudit.Cost = @Cost;
                                    inventoryAudit.Price = 0;
                                    inventoryAudit.CumulativeQty = inventory_audit.Sum(q => q.Qty) + @Qty;
                                    inventoryAudit.CumulativeValue = inventory_audit.Sum(s => s.Trans_Valuse) + (@Qty * @Cost);
                                    inventoryAudit.Trans_Valuse = @Qty * @Cost;
                                    inventoryAudit.LocalCurID = cancelreceipt.LocalCurrencyID;
                                    inventoryAudit.LocalSetRate = cancelreceipt.LocalSetRate;
                                    inventoryAudit.DocumentTypeID = docType.ID;
                                    inventoryAudit.SeriesID = cancelreceipt.SeriesID;
                                    inventoryAudit.SeriesDetailID = cancelreceipt.SeriesDID;
                                    //
                                    inventoryAccAmountavg += (decimal)inventoryAudit.Cost * (decimal)@Qty;
                                    COGSAccAmountavg += (decimal)inventoryAudit.Cost * (decimal)@Qty;
                                    _utility.CumulativeValue(inventoryAudit.WarehouseID, inventoryAudit.ItemID, inventoryAudit.CumulativeValue, _itemAcc);
                                }
                                else
                                {
                                    var inventory_audit = _context.InventoryAudits.Where(w => w.ItemID == item_cancel.ItemID && w.WarehouseID == cancelreceipt.WarehouseID).ToList();
                                    InventoryAudit avgInventory = new() { Cost = @Cost, Qty = Qty };
                                    double @AvgCost = _utility.CalAVGCost(item_cancel.ItemID, cancelreceipt.WarehouseID, avgInventory);
                                    inventoryAudit.ID = 0;
                                    inventoryAudit.WarehouseID = cancelreceipt.WarehouseID;
                                    inventoryAudit.BranchID = cancelreceipt.BranchID;
                                    inventoryAudit.UserID = cancelreceipt.UserOrderID;
                                    inventoryAudit.ItemID = item_cancel.ItemID;
                                    inventoryAudit.CurrencyID = cancelreceipt.SysCurrencyID;
                                    inventoryAudit.UomID = item_cancel.UomID;
                                    inventoryAudit.InvoiceNo = cancelreceipt.ReceiptNo;
                                    inventoryAudit.Trans_Type = docType.Code;
                                    inventoryAudit.Process = itemMaster.Process;
                                    inventoryAudit.SystemDate = DateTime.Now;
                                    inventoryAudit.TimeIn = DateTime.Now.ToShortTimeString();
                                    inventoryAudit.Qty = @Qty;
                                    inventoryAudit.Cost = @AvgCost;
                                    inventoryAudit.Price = 0;
                                    inventoryAudit.CumulativeQty = inventory_audit.Sum(q => q.Qty) + @Qty;
                                    inventoryAudit.CumulativeValue = inventory_audit.Sum(s => s.Trans_Valuse) + (@Qty * @AvgCost);
                                    inventoryAudit.Trans_Valuse = @Qty * @AvgCost;
                                    inventoryAudit.LocalCurID = cancelreceipt.LocalCurrencyID;
                                    inventoryAudit.LocalSetRate = cancelreceipt.LocalSetRate;
                                    inventoryAudit.DocumentTypeID = docType.ID;
                                    inventoryAudit.SeriesID = cancelreceipt.SeriesID;
                                    inventoryAudit.SeriesDetailID = cancelreceipt.SeriesDID;
                                    double @ACost = _utility.CalAVGCost(item_cancel.ItemID, cancelreceipt.WarehouseID, inventoryAudit);
                                    inventoryAccAmountavg += (decimal)(@ACost * @Qty);
                                    COGSAccAmountavg += (decimal)(@ACost * @Qty);
                                    //
                                    _utility.UpdateAvgCost(item_cancel.ItemID, cancelreceipt.WarehouseID, item_cancel.GUoMID, @Qty, @AvgCost);
                                    _utility.UpdateBomCost(item_cancel.ItemID, @Qty, @AvgCost);
                                    _utility.CumulativeValue(inventoryAudit.WarehouseID, inventoryAudit.ItemID, inventoryAudit.CumulativeValue, _itemAcc);
                                }

                                //inventoryAccID
                                var glAccInvenfifo = _context.GLAccounts.FirstOrDefault(w => w.ID == inventoryAccIDavg) ?? new GLAccount();
                                if (glAccInvenfifo.ID > 0)
                                {
                                    var journalDetail = journalEntryDetail.FirstOrDefault(w => w.ItemID == glAccInvenfifo.ID) ?? new JournalEntryDetail();
                                    if (journalDetail.ItemID > 0)
                                    {
                                        var accBalance = accountBalance.FirstOrDefault(w => w.GLAID == inventoryAccIDavg);
                                        glAccInvenfifo.Balance += inventoryAccAmountavg;
                                        //journalEntryDetail
                                        journalDetail.Debit += inventoryAccAmountavg;
                                        //accountBalance
                                        accBalance.CumulativeBalance = glAccInvenfifo.Balance;
                                        accBalance.Debit += inventoryAccAmountavg;
                                    }
                                    else
                                    {
                                        glAccInvenfifo.Balance += inventoryAccAmountavg;
                                        journalEntryDetail.Add(new JournalEntryDetail
                                        {
                                            JEID = journalEntry.ID,
                                            Type = Type.GLAcct,
                                            ItemID = inventoryAccIDavg,
                                            Debit = inventoryAccAmountavg,
                                        });
                                        //
                                        accountBalance.Add(new AccountBalance
                                        {
                                            JEID = journalEntry.ID,

                                            PostingDate = cancelreceipt.DateOut,
                                            Origin = docType.ID,
                                            OriginNo = cancelreceipt.ReceiptNo,
                                            OffsetAccount = OffsetAcc,
                                            Details = douTypeID.Name + "-" + glAccInvenfifo.Code,
                                            CumulativeBalance = glAccInvenfifo.Balance,
                                            Debit = inventoryAccAmountavg,
                                            LocalSetRate = (decimal)cancelreceipt.LocalSetRate,
                                            GLAID = inventoryAccIDavg,
                                        });
                                    }
                                    _context.GLAccounts.Update(glAccInvenfifo);
                                }
                                // COGS
                                var glAccCOGSfifo = _context.GLAccounts.FirstOrDefault(w => w.ID == COGSAccIDavg) ?? new GLAccount();
                                if (glAccCOGSfifo.ID > 0)
                                {
                                    var journalDetail = journalEntryDetail.FirstOrDefault(w => w.ItemID == glAccCOGSfifo.ID) ?? new JournalEntryDetail();
                                    if (journalDetail.ItemID > 0)
                                    {
                                        var accBalance = accountBalance.FirstOrDefault(w => w.GLAID == COGSAccIDavg);
                                        glAccCOGSfifo.Balance -= COGSAccAmountavg;
                                        //journalEntryDetail
                                        journalDetail.Credit += COGSAccAmountavg;
                                        //accountBalance
                                        accBalance.CumulativeBalance = glAccCOGSfifo.Balance;
                                        accBalance.Credit += COGSAccAmountavg;
                                    }
                                    else
                                    {
                                        glAccCOGSfifo.Balance -= COGSAccAmountavg;
                                        journalEntryDetail.Add(new JournalEntryDetail
                                        {
                                            JEID = journalEntry.ID,
                                            Type = Type.GLAcct,
                                            ItemID = COGSAccIDavg,
                                            Credit = COGSAccAmountavg,
                                        });
                                        //
                                        accountBalance.Add(new AccountBalance
                                        {
                                            JEID = journalEntry.ID,

                                            PostingDate = cancelreceipt.DateOut,
                                            Origin = docType.ID,
                                            OriginNo = cancelreceipt.ReceiptNo,
                                            OffsetAccount = OffsetAcc,
                                            Details = douTypeID.Name + "-" + glAccCOGSfifo.Code,
                                            CumulativeBalance = glAccCOGSfifo.Balance,
                                            Credit = COGSAccAmountavg,
                                            LocalSetRate = (decimal)cancelreceipt.LocalSetRate,
                                            GLAID = COGSAccIDavg,
                                        });
                                    }
                                    _context.GLAccounts.Update(glAccCOGSfifo);
                                }
                            }
                            _context.InventoryAudits.Update(inventoryAudit);
                            _context.WarehouseSummary.Update(warehouseSummary);
                            _context.ItemMasterDatas.Update(itemMaster);
                            _context.WarehouseDetails.Update(warehouseDetail);
                            _context.SaveChanges();
                        }
                    }
                }
                //IncomingPaymentSeriesAccountingReturn(journalEntryDetail, accountBalance, journalEntry, docType, douTypeID, cancelreceipt);
                var journal = _context.JournalEntries.Find(journalEntry.ID);
                journal.TotalDebit = journalEntryDetail.Sum(s => s.Debit);
                journal.TotalCredit = journalEntryDetail.Sum(s => s.Credit);
                _context.JournalEntries.Update(journal);
                _context.JournalEntryDetails.UpdateRange(journalEntryDetail);
                _context.AccountBalances.UpdateRange(accountBalance);
                _context.SaveChanges();
            }
        
        private void IncomingPaymentSeriesAccountingReturn(List<JournalEntryDetail> journalEntryDetail,
            List<AccountBalance> accountBalance, JournalEntry journalEntry, DocumentType docType,
            DocumentType douTypeID, ReceiptMemo rmemo)
        {
            // AccountReceice
            var incomAccReceive = _context.BusinessPartners.FirstOrDefault(w => w.ID == rmemo.CustomerID);
            var glAccD = _context.GLAccounts.FirstOrDefault(w => w.ID == incomAccReceive.GLAccID);
            decimal cardPay = rmemo.OtherPaymentGrandTotal * (decimal)rmemo.ExchangeRate;
            decimal totalpay = (rmemo.BalancePay * (decimal)rmemo.ExchangeRate) - cardPay;
            journalEntryDetail.Add(new JournalEntryDetail
            {
                JEID = journalEntry.ID,
                Type = Type.BPCode,
                ItemID = incomAccReceive.GLAccID,
                Debit = totalpay,
                BPAcctID = rmemo.CustomerID,
            });
            //Insert 
            glAccD.Balance += totalpay;
            accountBalance.Add(new AccountBalance
            {
                JEID = journalEntry.ID,

                PostingDate = rmemo.DateIn,
                Origin = docType.ID,
                OriginNo = rmemo.ReceiptNo,
                OffsetAccount = glAccD.Code,
                Details = douTypeID.Name + " - " + glAccD.Code,
                CumulativeBalance = glAccD.Balance,
                Debit = totalpay,
                LocalSetRate = (decimal)rmemo.LocalSetRate,
                GLAID = incomAccReceive.GLAccID,
                BPAcctID = rmemo.CustomerID,
                Creator = rmemo.UserOrderID,
            });

            var accountReceive = _context.BusinessPartners.FirstOrDefault(w => w.ID == rmemo.CustomerID);
            var glAcc = _context.GLAccounts.FirstOrDefault(w => w.ID == accountReceive.GLAccID) ?? new GLAccount();
            string OffsetAcc = accountReceive.Code + "-" + glAcc.Code;
            journalEntryDetail.Add(new JournalEntryDetail
            {
                JEID = journalEntry.ID,
                Type = Type.BPCode,
                ItemID = accountReceive.GLAccID,
                Debit = cardPay,
                BPAcctID = rmemo.CustomerID,
            });
            //Insert             
            glAcc.Balance += cardPay;
            accountBalance.Add(new AccountBalance
            {
                JEID = journalEntry.ID,

                PostingDate = rmemo.DateIn,
                Origin = docType.ID,
                OriginNo = rmemo.ReceiptNo,
                OffsetAccount = OffsetAcc,
                Details = douTypeID.Name + "-" + glAcc.Code,
                CumulativeBalance = glAcc.Balance,
                Debit = cardPay,
                LocalSetRate = (decimal)rmemo.LocalSetRate,
                GLAID = accountReceive.GLAccID,
                BPAcctID = rmemo.CustomerID,
                Creator = rmemo.UserOrderID
            });
            var accountSelected = _context.PaymentMeans.Find(rmemo.PaymentMeansID);
            var glAccC = _context.GLAccounts.FirstOrDefault(w => w.ID == accountSelected.AccountID);
            journalEntryDetail.Add(new JournalEntryDetail
            {
                JEID = journalEntry.ID,
                Type = Type.GLAcct,
                ItemID = accountSelected.AccountID,
                Credit = totalpay,
                BPAcctID = rmemo.CustomerID,
            });
            //Insert 
            glAccC.Balance -= totalpay;
            accountBalance.Add(new AccountBalance
            {
                JEID = journalEntry.ID,

                PostingDate = rmemo.DateIn,
                Origin = docType.ID,
                OriginNo = rmemo.ReceiptNo,
                OffsetAccount = glAccC.Code,
                Details = douTypeID.Name + " - " + glAccC.Code,
                CumulativeBalance = glAccC.Balance,
                Credit = totalpay,
                LocalSetRate = (decimal)rmemo.LocalSetRate,
                GLAID = accountSelected.AccountID,
            });
            _context.Update(glAccC);
            _context.Update(glAccD);
        }

        private void InsertFinancialReceiptMemo(
            int inventoryAccID, int COGSAccID, decimal inventoryAccAmount, decimal COGSAccAmount,
            List<JournalEntryDetail> journalEntryDetail, List<AccountBalance> accountBalance,
            JournalEntry journalEntry, ReceiptMemo cancelreceipt, DocumentType docType, DocumentType douTypeID,
            string OffsetAcc, ReceiptDetailMemo item)
        {
            if (!item.IsKsms && !item.IsKsmsMaster)
            {
                //inventoryAccID
                var glAccInvenfifo = _context.GLAccounts.FirstOrDefault(w => w.ID == inventoryAccID) ?? new GLAccount();
                if (glAccInvenfifo.ID > 0)
                {
                    var journalDetail = journalEntryDetail.FirstOrDefault(w => w.ItemID == glAccInvenfifo.ID) ?? new JournalEntryDetail();
                    if (journalDetail.ItemID > 0)
                    {
                        var accBalance = accountBalance.FirstOrDefault(w => w.GLAID == inventoryAccID);
                        glAccInvenfifo.Balance += inventoryAccAmount;
                        //journalEntryDetail
                        journalDetail.Debit += inventoryAccAmount;
                        //accountBalance
                        accBalance.CumulativeBalance = glAccInvenfifo.Balance;
                        accBalance.Debit += inventoryAccAmount;
                    }
                    else
                    {
                        glAccInvenfifo.Balance += inventoryAccAmount;
                        journalEntryDetail.Add(new JournalEntryDetail
                        {
                            JEID = journalEntry.ID,
                            Type = Type.GLAcct,
                            ItemID = inventoryAccID,
                            Debit = inventoryAccAmount,
                        });
                        //
                        accountBalance.Add(new AccountBalance
                        {
                            JEID = journalEntry.ID,

                            PostingDate = cancelreceipt.DateOut,
                            Origin = docType.ID,
                            OriginNo = cancelreceipt.ReceiptNo,
                            OffsetAccount = OffsetAcc,
                            Details = douTypeID.Name + "-" + glAccInvenfifo.Code,
                            CumulativeBalance = glAccInvenfifo.Balance,
                            Debit = inventoryAccAmount,
                            LocalSetRate = (decimal)cancelreceipt.LocalSetRate,
                            GLAID = inventoryAccID,
                        });
                    }
                    _context.Update(glAccInvenfifo);
                }
                //Add COGS
                var glAccCOGSfifo = _context.GLAccounts.FirstOrDefault(w => w.ID == COGSAccID) ?? new GLAccount();
                if (glAccCOGSfifo.ID > 0)
                {
                    var journalDetail = journalEntryDetail.FirstOrDefault(w => w.ItemID == glAccCOGSfifo.ID) ?? new JournalEntryDetail();
                    if (journalDetail.ItemID > 0)
                    {
                        var accBalance = accountBalance.FirstOrDefault(w => w.GLAID == COGSAccID);
                        glAccCOGSfifo.Balance -= COGSAccAmount;
                        //journalEntryDetail
                        journalDetail.Credit += COGSAccAmount;
                        //accountBalance
                        accBalance.CumulativeBalance = glAccCOGSfifo.Balance;
                        accBalance.Credit += COGSAccAmount;
                    }
                    else
                    {
                        glAccCOGSfifo.Balance -= COGSAccAmount;
                        journalEntryDetail.Add(new JournalEntryDetail
                        {
                            JEID = journalEntry.ID,
                            Type = Type.GLAcct,
                            ItemID = COGSAccID,
                            Credit = COGSAccAmount,
                        });
                        accountBalance.Add(new AccountBalance
                        {
                            JEID = journalEntry.ID,

                            PostingDate = cancelreceipt.DateOut,
                            Origin = docType.ID,
                            OriginNo = cancelreceipt.ReceiptNo,
                            OffsetAccount = OffsetAcc,
                            Details = douTypeID.Name + "-" + glAccCOGSfifo.Code,
                            CumulativeBalance = glAccCOGSfifo.Balance,
                            Credit = COGSAccAmount,
                            LocalSetRate = (decimal)cancelreceipt.LocalSetRate,
                            GLAID = COGSAccID,
                        });
                    }
                    _context.Update(glAccCOGSfifo);
                }
            }
            _context.SaveChanges();
        }
        public void SaveOrder(Order data)
        {
            if (data.OrderID <= 0)
            {
                var order_queue = _context.Order_Queue.FirstOrDefault(i => i.ID == _context.Order_Queue.Max(_i => _i.ID)) ?? new Order_Queue();
                if (order_queue.ID == 0)
                {
                    data.OrderNo = $"Order-{1}";
                    data.QueueNo = $"{1}";
                }
                else
                {
                    int order_no = Convert.ToInt32(order_queue.OrderNo.Split("-")[1]);
                    data.OrderNo = $"Order-{order_no + 1}";
                    data.QueueNo = $"{order_no + 1}";
                }
                Order_Queue order_Queue = new()
                {
                    BranchID = data.BranchID,
                    OrderNo = data.OrderNo,
                    DateTime = DateTime.Now,
                };
                _context.Order_Queue.Update(order_Queue);
            }
            data.Status = OrderStatus.Pay;
            _context.Order.Update(data);
            _context.SaveChanges();
        }
        public async Task PrintBarcodeItemsAsync(List<ItemPrintBarcodeView> items, string printerName)
        {
            await _timeDelivery.PrintBarcodeItems(items, printerName);
        }

        public List<CashoutReportMaster> ReprintCloseShift(int userid, int closeShiftId, bool isWebClient = true)
        {
            var closeshift = _context.CloseShift.Find(closeShiftId);
            return PrintCloseShift(userid, closeshift, isWebClient);
        }

        private List<CashoutReportMaster> PrintCloseShift(int userid, CloseShift closeshift, bool isWebClient = true)
        {
            var settings = _context.GeneralSettings.Where(w => w.UserID == userid).ToList();
            var setting = settings.FirstOrDefault() ?? new GeneralSetting();
            List<CashoutReportMaster> cashOutReports = new List<CashoutReportMaster>();
            if (closeshift.ID <= 0) { return cashOutReports; }
            switch (setting.CloseShift)
            {
                case CloseShiftType.Category:
                    cashOutReports = _report.GetCashoutReport(closeshift.Trans_From, closeshift.Trans_To, userid, setting.LocalCurrencyID).ToList();
                    var ip = _accessor.ActionContext.HttpContext.Connection.RemoteIpAddress.ToString();
                    var cashoutvoiditem = _report.GetCashoutVoidItems(userid).ToList();
                    if (isWebClient)
                    {
                        _timeDelivery.PrintCashout(cashOutReports, cashoutvoiditem, settings);
                    }
                    break;
                case CloseShiftType.PaymentMean:
                    cashOutReports = _report.GetCashoutPaymentMean(closeshift.Trans_From, closeshift.Trans_To, userid, setting.LocalCurrencyID).ToList();
                    if (isWebClient)
                    {
                        _timeDelivery.PrintCashoutPaymentMeans(cashOutReports, settings);
                    }

                    break;
                case CloseShiftType.CategorySummary:
                    cashOutReports = _report.GetCashoutReport(closeshift.Trans_From, closeshift.Trans_To, userid, setting.LocalCurrencyID).ToList();
                    var ips = _accessor.ActionContext.HttpContext.Connection.RemoteIpAddress.ToString();
                    if (isWebClient)
                    {
                        _timeDelivery.PrintCashoutSummary(cashOutReports, settings);
                    }
                    break;
                case CloseShiftType.PaymentMeanSummary:
                    cashOutReports = _report.GetCashoutPaymentMean(closeshift.Trans_From, closeshift.Trans_To, userid, setting.LocalCurrencyID).ToList();
                    if (isWebClient)
                    {
                        _timeDelivery.PrintCashoutPaymentMeanSummary(cashOutReports, settings);
                    }
                    break;
                case CloseShiftType.CategoryAndPayment:
                    var cashoutreportboth = _report.GetCashoutReport(closeshift.Trans_From, closeshift.Trans_To, userid, setting.LocalCurrencyID).ToList();
                    var ipboth = _accessor.ActionContext.HttpContext.Connection.RemoteIpAddress.ToString();
                    var cashoutpaymentmeanboth = _report.GetCashoutPaymentMean(closeshift.Trans_From, closeshift.Trans_To, userid, setting.LocalCurrencyID).ToList();
                    cashOutReports = new(cashoutreportboth.Count + cashoutpaymentmeanboth.Count);
                    cashOutReports.AddRange(cashoutreportboth);
                    cashOutReports.AddRange(cashoutpaymentmeanboth);
                    if (isWebClient)
                    {
                        _timeDelivery.PrintCategoryAndPayment(cashOutReports, settings);
                    }
                    break;
                case CloseShiftType.CategorySummaryAndPaymentSummary:
                    var cashoutreportsummary = _report.GetCashoutReport(closeshift.Trans_From, closeshift.Trans_To, userid, setting.LocalCurrencyID).ToList();
                    var ipsummary = _accessor.ActionContext.HttpContext.Connection.RemoteIpAddress.ToString();
                    var ipp = _accessor.ActionContext.HttpContext.Connection.RemoteIpAddress.ToString();
                    var cashoutpaymentmeansummaryboth = _report.GetCashoutPaymentMean(closeshift.Trans_From, closeshift.Trans_To, userid, setting.LocalCurrencyID).ToList();
                    cashOutReports = new(cashoutreportsummary.Count + cashoutpaymentmeansummaryboth.Count);
                    cashOutReports.AddRange(cashoutreportsummary);
                    cashOutReports.AddRange(cashoutpaymentmeansummaryboth);
                    if (isWebClient)
                    {
                        _timeDelivery.PrintCashoutSummaryAndPaymentSummary(cashOutReports, settings);
                    }
                    break;
                default:
                    break;
            }
            return cashOutReports;
        }

        //IssuseStock Can Ring
        public void IssuseStockCanRing(CanRingMaster crm, List<SerialNumber> serials, List<BatchNo> batches, List<SerialViewModelPurchase> serialViewModelPurchases, List<BatchViewModelPurchase> batchViewModelPurchases)
        {
            var Com = _context.Company.FirstOrDefault(c => c.ID == crm.CompanyID);
            var Exr = _context.ExchangeRates.FirstOrDefault(e => e.CurrencyID == Com.LocalCurrencyID);
            var docType = _context.DocumentTypes.FirstOrDefault(w => w.Code == "CR");
            var glAccs = _context.GLAccounts.Where(i => i.IsActive);
            var series = _context.Series.Find(crm.SeriesID) ?? new Series();
            var warehouse = _context.Warehouses.Find(crm.WarehouseID) ?? new Warehouse();
            JournalEntry journalEntry = new();
            SeriesDetail seriesDetail = new();
            string OffsetAcc = "";
            var douTypeID = _context.DocumentTypes.FirstOrDefault(w => w.Code == "JE");
            var defaultJE = _context.Series.FirstOrDefault(w => w.Default == true && w.DocuTypeID == douTypeID.ID && w.NextNo != w.LastNo) ?? new Series();
            if (defaultJE.ID > 0)
            {
                // update series
                string Sno = defaultJE.NextNo;
                long No = long.Parse(Sno);
                defaultJE.NextNo = Convert.ToString(No + 1);
                // update series details
                seriesDetail.SeriesID = journalEntry.SeriesID;
                seriesDetail.Number = Sno;
                _context.Update(defaultJE);
                _context.Update(seriesDetail);
                _context.SaveChanges();
                // Insert Journal Entry
                journalEntry.SeriesID = defaultJE.ID;
                journalEntry.Number = defaultJE.PreFix + "-" + Sno;
                journalEntry.DouTypeID = defaultJE.DocuTypeID;
                journalEntry.Creator = crm.UserID;
                journalEntry.TransNo = crm.Number;
                journalEntry.PostingDate = crm.CreatedAt;
                journalEntry.DocumentDate = crm.CreatedAt;
                journalEntry.DueDate = crm.CreatedAt;
                journalEntry.SSCID = crm.SysCurrencyID;
                journalEntry.LLCID = crm.LocalCurrencyID;
                journalEntry.CompanyID = crm.CompanyID;
                journalEntry.LocalSetRate = (decimal)crm.LocalSetRate;
                journalEntry.SeriesDID = seriesDetail.ID;
                journalEntry.Remarks = series.Name + "-" + crm.Number;
                _context.Update(journalEntry);
            }
            _context.SaveChanges();
            //Debit account receivable  
            List<JournalEntryDetail> journalEntryDetail = new();
            List<AccountBalance> accountBalance = new();
            var accountReceive = _context.BusinessPartners.FirstOrDefault(w => w.ID == crm.CusId);
            var glAccCus = _context.GLAccounts.FirstOrDefault(w => w.ID == accountReceive.GLAccID) ?? new GLAccount();
            OffsetAcc = accountReceive.Code + "-" + glAccCus.Code;
            if (glAccCus.ID > 0)
            {
                journalEntryDetail.Add(new JournalEntryDetail
                {
                    JEID = journalEntry.ID,
                    Type = Type.BPCode,
                    ItemID = accountReceive.GLAccID,
                    Debit = crm.TotalSystem,
                    BPAcctID = crm.CusId,
                });
                //Insert             
                glAccCus.Balance += crm.TotalSystem;
                accountBalance.Add(new AccountBalance
                {
                    JEID = journalEntry.ID,

                    PostingDate = crm.CreatedAt,
                    Origin = docType.ID,
                    OriginNo = crm.Number,
                    OffsetAccount = OffsetAcc,
                    Details = douTypeID.Name + "-" + glAccCus.Code,
                    CumulativeBalance = glAccCus.Balance,
                    Debit = crm.TotalSystem,
                    LocalSetRate = (decimal)crm.LocalSetRate,
                    GLAID = accountReceive.GLAccID,
                    BPAcctID = crm.CusId,
                    Creator = crm.UserID
                });
            }
            var paymentmean = _context.PaymentMeans.Find(crm.PaymentMeanID) ?? new PaymentMeans();
            var glAccCashBank = _context.GLAccounts.FirstOrDefault(w => w.ID == paymentmean.AccountID) ?? new GLAccount();
            if (glAccCashBank.ID > 0)
            {
                journalEntryDetail.Add(new JournalEntryDetail
                {
                    JEID = journalEntry.ID,
                    Type = Type.BPCode,
                    ItemID = paymentmean.AccountID,
                    Debit = crm.TotalSystem,
                });
                //Insert             
                glAccCashBank.Balance += crm.TotalSystem;
                accountBalance.Add(new AccountBalance
                {
                    JEID = journalEntry.ID,

                    PostingDate = crm.CreatedAt,
                    Origin = docType.ID,
                    OriginNo = crm.Number,
                    OffsetAccount = OffsetAcc,
                    Details = douTypeID.Name + "-" + glAccCashBank.Code,
                    CumulativeBalance = glAccCashBank.Balance,
                    Debit = crm.TotalSystem,
                    LocalSetRate = (decimal)crm.LocalSetRate,
                    GLAID = paymentmean.AccountID,
                });
            }
            if (glAccCus.ID > 0)
            {
                journalEntryDetail.Add(new JournalEntryDetail
                {
                    JEID = journalEntry.ID,
                    Type = Type.BPCode,
                    ItemID = accountReceive.GLAccID,
                    Credit = crm.TotalSystem,
                    BPAcctID = crm.CusId,
                });
                //Insert             
                glAccCus.Balance -= crm.TotalSystem;
                accountBalance.Add(new AccountBalance
                {
                    JEID = journalEntry.ID,

                    PostingDate = crm.CreatedAt,
                    Origin = docType.ID,
                    OriginNo = crm.Number,
                    OffsetAccount = OffsetAcc,
                    Details = douTypeID.Name + "-" + glAccCus.Code,
                    CumulativeBalance = glAccCus.Balance,
                    Credit = crm.TotalSystem,
                    LocalSetRate = (decimal)crm.LocalSetRate,
                    GLAID = accountReceive.GLAccID,
                    BPAcctID = crm.CusId,
                    Creator = crm.UserID
                });
            }
            _context.AccountBalances.UpdateRange(accountBalance);
            _context.GLAccounts.Update(glAccCus);
            _context.SaveChanges();

            foreach (var item in crm.CanRingDetials)
            {
                int revenueAccID = 0, inventoryAccID = 0, inventoryAccIncreaseID = 0;
                decimal inventoryAccAmount = 0, revenueAccAmount = 0;
                var itemMasterChange = _context.ItemMasterDatas.FirstOrDefault(w => w.ID == item.ItemChangeID && !w.Delete);
                var orftChange = _context.GroupDUoMs.FirstOrDefault(w => w.GroupUoMID == itemMasterChange.GroupUomID && w.AltUOM == item.UomChangeID);
                double qtyChange = (double)item.ChangeQty * orftChange.Factor;
                var wareDetailsChange = _context.WarehouseDetails.Where(w => w.WarehouseID == crm.WarehouseID && w.ItemID == item.ItemChangeID).ToList();
                WarehouseDetail lastItemWh = wareDetailsChange.OrderByDescending(i => i.SyetemDate).FirstOrDefault(w => w.InStock > 0) ?? new WarehouseDetail();
                if (itemMasterChange.SetGlAccount == SetGlAccount.ItemLevel)
                {
                    var itemAccs = _context.ItemAccountings.Where(w => w.ItemID == item.ItemChangeID && w.WarehouseID == crm.WarehouseID);
                    var revenueAcc = (from ia in itemAccs
                                      join gl in glAccs on ia.RevenueAccount equals gl.Code
                                      select gl
                                         ).FirstOrDefault() ?? new GLAccount();
                    var inventoryAcc = (from ia in itemAccs
                                        join gl in glAccs on ia.InventoryAccount equals gl.Code
                                        select gl
                                         ).FirstOrDefault() ?? new GLAccount();
                    revenueAccID = revenueAcc.ID;
                    inventoryAccID = inventoryAcc.ID;
                }
                else if (itemMasterChange.SetGlAccount == SetGlAccount.ItemGroup)
                {
                    var itemAccs = _context.ItemAccountings.Where(w => w.ItemGroupID == itemMasterChange.ItemGroup1ID);
                    var revenueAcc = (from ia in itemAccs
                                      join gl in glAccs on ia.RevenueAccount equals gl.Code
                                      select gl
                                         ).FirstOrDefault() ?? new GLAccount();
                    var inventoryAcc = (from ia in itemAccs
                                        join gl in glAccs on ia.InventoryAccount equals gl.Code
                                        select gl
                                         ).FirstOrDefault() ?? new GLAccount();
                    revenueAccID = revenueAcc.ID;
                    inventoryAccID = inventoryAcc.ID;
                }
                revenueAccAmount = item.ChargePrice * crm.ExchangeRate;

                #region normal item increase stock
                var itemMaster = _context.ItemMasterDatas.FirstOrDefault(w => w.ID == item.ItemID && !w.Delete);
                if (itemMaster.SetGlAccount == SetGlAccount.ItemLevel)
                {
                    var itemAccs = _context.ItemAccountings.Where(w => w.ItemID == item.ItemID && w.WarehouseID == crm.WarehouseID);
                    var inventoryAcc = (from ia in itemAccs
                                        join gl in glAccs on ia.InventoryAccount equals gl.Code
                                        select gl
                                         ).FirstOrDefault() ?? new GLAccount();
                    inventoryAccIncreaseID = inventoryAcc.ID;
                }
                else if (itemMaster.SetGlAccount == SetGlAccount.ItemGroup)
                {
                    var itemAccs = _context.ItemAccountings.Where(w => w.ItemGroupID == itemMaster.ItemGroup1ID);
                    var inventoryAcc = (from ia in itemAccs
                                        join gl in glAccs on ia.InventoryAccount equals gl.Code
                                        select gl
                                         ).FirstOrDefault() ?? new GLAccount();
                    inventoryAccIncreaseID = inventoryAcc.ID;
                }
                if (itemMaster.Process != "Standard")
                {
                    InventoryAudit item_inventory_audit = new();
                    WarehouseDetail warehousedetail = new();
                    double _cost = lastItemWh.Cost;
                    var orft = _context.GroupDUoMs.FirstOrDefault(w => w.GroupUoMID == itemMaster.GroupUomID && w.AltUOM == item.UomID);
                    double qty = (double)item.Qty * orft.Factor;
                    var _itemAcc = _context.ItemAccountings.FirstOrDefault(i => i.WarehouseID == crm.WarehouseID && i.ItemID == item.ItemID);
                    var item_warehouse_summary = _context.WarehouseSummary.FirstOrDefault(w => w.WarehouseID == crm.WarehouseID && w.ItemID == item.ItemID);
                    var wareDetails = _context.WarehouseDetails.Where(w => w.WarehouseID == crm.WarehouseID && w.ItemID == item.ItemID).ToList();
                    //var waredetialChange = wareDetailsChange.FirstOrDefault(i => ss.SerialNumber == i.SerialNumber && i.InStock > 0);
                    if (item_warehouse_summary != null)
                    {
                        item_warehouse_summary.InStock += qty;
                        _context.WarehouseSummary.Update(item_warehouse_summary);
                        _utility.UpdateItemAccounting(_itemAcc, item_warehouse_summary);
                        _context.SaveChanges();
                    }

                    //insert warehousedetail
                    if (itemMaster.ManItemBy == ManageItemBy.SerialNumbers && itemMaster.ManMethod == ManagementMethod.OnEveryTransation)
                    {

                        var svmp = serialViewModelPurchases.FirstOrDefault(s => s.ItemID == item.ItemID);
                        List<WarehouseDetail> whsDetials = new();
                        List<InventoryAudit> inventoryAudit = new();
                        if (svmp != null)
                        {
                            foreach (var sv in svmp.SerialDetialViewModelPurchase.Where(i => !string.IsNullOrEmpty(i.SerialNumber)).ToList())
                            {
                                whsDetials.Add(new WarehouseDetail
                                {
                                    AdmissionDate = sv.AdmissionDate,
                                    Cost = _cost,
                                    CurrencyID = crm.SysCurrencyID,
                                    Details = sv.Detials,
                                    ID = 0,
                                    InStock = 1,
                                    ItemID = item.ItemID,
                                    Location = sv.Location,
                                    LotNumber = sv.LotNumber,
                                    MfrDate = sv.MfrDate,
                                    MfrSerialNumber = sv.MfrSerialNo,
                                    MfrWarDateEnd = sv.MfrWarrantyEnd,
                                    MfrWarDateStart = sv.MfrWarrantyStart,
                                    ProcessItem = _utility.CheckProcessItem(itemMaster.Process),
                                    SerialNumber = sv.SerialNumber,
                                    SyetemDate = DateTime.Now,
                                    SysNum = 0,
                                    TimeIn = DateTime.Now,
                                    WarehouseID = crm.WarehouseID,
                                    UomID = item.UomID,
                                    UserID = crm.UserID,
                                    ExpireDate = sv.ExpirationDate == null ? default : (DateTime)sv.ExpirationDate,
                                    TransType = TransTypeWD.CanRing,
                                    BPID = crm.CusId,
                                    IsDeleted = true,
                                    InStockFrom = crm.ID
                                });
                                //InsertFinancialCanRingInventory(inventoryAccIncreaseID, (decimal)_cost, journalEntryDetail, accountBalance, journalEntry, docType, douTypeID, crm, OffsetAcc, false);
                            }
                            //insert inventoryaudit
                            InventoryAudit invAudit = new();
                            var inventory_audit = _context.InventoryAudits.Where(w => w.ItemID == item.ItemID && w.UomID == item.UomID && w.WarehouseID == crm.WarehouseID);
                            var pri_detial = _context.PriceListDetails.Where(w => w.ItemID == item.ItemID && w.UomID == item.UomID);
                            invAudit.ID = 0;
                            invAudit.WarehouseID = crm.WarehouseID;
                            invAudit.BranchID = crm.BranchID;
                            invAudit.UserID = crm.UserID;
                            invAudit.ItemID = item.ItemID;
                            invAudit.CurrencyID = crm.SysCurrencyID;
                            invAudit.UomID = item.UomID;
                            invAudit.InvoiceNo = crm.Number;
                            invAudit.Trans_Type = docType.Code;
                            invAudit.Process = itemMaster.Process;
                            invAudit.SystemDate = DateTime.Now;
                            invAudit.TimeIn = DateTime.Now.ToString("h:mm:ss tt");
                            invAudit.Qty = qty;
                            invAudit.Cost = _cost;
                            invAudit.Price = 0;
                            invAudit.CumulativeQty = inventory_audit.Sum(q => q.Qty) + qty;
                            invAudit.CumulativeValue = inventory_audit.Sum(s => s.Trans_Valuse) + qty * _cost;
                            invAudit.Trans_Valuse = qty * _cost;
                            invAudit.LocalCurID = crm.LocalCurrencyID;
                            invAudit.LocalSetRate = crm.LocalSetRate;
                            invAudit.DocumentTypeID = crm.DocTypeID;
                            invAudit.CompanyID = crm.CompanyID;
                            invAudit.SeriesID = crm.SeriesID;
                            invAudit.SeriesDetailID = crm.SeriesDID;
                            // update pricelistdetial
                            _utility.CumulativeValue(invAudit.WarehouseID, invAudit.ItemID, invAudit.CumulativeValue, _itemAcc);
                            _context.InventoryAudits.Add(invAudit);
                            _context.WarehouseDetails.AddRange(whsDetials);
                            _context.SaveChanges();
                        }
                    }
                    else if (itemMaster.ManItemBy == ManageItemBy.Batches && itemMaster.ManMethod == ManagementMethod.OnEveryTransation)
                    {
                        var bvmp = batchViewModelPurchases.FirstOrDefault(s => s.ItemID == item.ItemID);
                        List<WarehouseDetail> whsDetials = new();

                        if (bvmp != null)
                        {
                            var bvs = bvmp.BatchDetialViewModelPurchases.Where(i => !string.IsNullOrEmpty(i.Batch) && i.Qty > 0).ToList();
                            foreach (var bv in bvs)
                            {
                                var _qty = (double)bv.Qty;
                                whsDetials.Add(new WarehouseDetail
                                {
                                    AdmissionDate = bv.AdmissionDate,
                                    Cost = _cost,
                                    CurrencyID = crm.SysCurrencyID,
                                    Details = bv.Detials,
                                    ID = 0,
                                    InStock = _qty,
                                    ItemID = item.ItemID,
                                    Location = bv.Location,
                                    MfrDate = bv.MfrDate,
                                    ProcessItem = _utility.CheckProcessItem(itemMaster.Process),
                                    SyetemDate = DateTime.Now,
                                    SysNum = 0,
                                    TimeIn = DateTime.Now,
                                    WarehouseID = crm.WarehouseID,
                                    UomID = item.UomID,
                                    UserID = crm.UserID,
                                    ExpireDate = bv.ExpirationDate == null ? default : (DateTime)bv.ExpirationDate,
                                    BatchAttr1 = bv.BatchAttribute1,
                                    BatchAttr2 = bv.BatchAttribute2,
                                    BatchNo = bv.Batch,
                                    TransType = TransTypeWD.CanRing,
                                    BPID = crm.CusId,
                                    InStockFrom = crm.ID,
                                    IsDeleted = true,
                                });
                                //InsertFinancialCanRingInventory(inventoryAccIncreaseID, (decimal)_cost, journalEntryDetail, accountBalance, journalEntry, docType, douTypeID, crm, OffsetAcc, false);
                            }
                            //insert inventoryaudit
                            InventoryAudit invAudit = new();
                            var inventory_audit = _context.InventoryAudits
                                .Where(w => w.ItemID == item.ItemID && w.UomID == item.UomID && w.WarehouseID == crm.WarehouseID)
                                .ToList();
                            // var pri_detial = _context.PriceListDetails.Where(w => w.ItemID == item.ItemID && w.UomID == item.UomID);
                            invAudit.ID = 0;
                            invAudit.WarehouseID = crm.WarehouseID;
                            invAudit.BranchID = crm.BranchID;
                            invAudit.UserID = crm.UserID;
                            invAudit.ItemID = item.ItemID;
                            invAudit.CurrencyID = crm.SysCurrencyID;
                            invAudit.UomID = item.UomID;
                            invAudit.InvoiceNo = crm.Number;
                            invAudit.Trans_Type = docType.Code;
                            invAudit.Process = itemMaster.Process;
                            invAudit.SystemDate = DateTime.Now;
                            invAudit.TimeIn = DateTime.Now.ToString("h:mm:ss tt");
                            invAudit.Qty = qty;
                            invAudit.Cost = _cost;
                            invAudit.Price = 0;
                            invAudit.CumulativeQty = inventory_audit.Sum(q => q.Qty) + qty;
                            invAudit.CumulativeValue = inventory_audit.Sum(s => s.Trans_Valuse) + qty * _cost;
                            invAudit.Trans_Valuse = qty * _cost;
                            invAudit.LocalCurID = crm.LocalCurrencyID;
                            invAudit.LocalSetRate = crm.LocalSetRate;
                            invAudit.DocumentTypeID = crm.DocTypeID;
                            invAudit.CompanyID = crm.CompanyID;
                            invAudit.SeriesID = crm.SeriesID;
                            invAudit.SeriesDetailID = crm.SeriesDID;
                            // update pricelistdetial
                            _utility.CumulativeValue(invAudit.WarehouseID, invAudit.ItemID, invAudit.CumulativeValue, _itemAcc);
                            _context.InventoryAudits.Add(invAudit);
                            _context.WarehouseDetails.AddRange(whsDetials);
                            _context.SaveChanges();
                        }
                    }
                    else
                    {
                        warehousedetail.ID = 0;
                        warehousedetail.WarehouseID = crm.WarehouseID;
                        warehousedetail.UserID = crm.UserID;
                        warehousedetail.UomID = item.UomID;
                        warehousedetail.SyetemDate = DateTime.Now;
                        warehousedetail.TimeIn = DateTime.Now;
                        warehousedetail.InStock = qty;
                        warehousedetail.CurrencyID = crm.SysCurrencyID;
                        warehousedetail.ItemID = item.ItemID;
                        warehousedetail.Cost = _cost;
                        warehousedetail.IsDeleted = true;
                        warehousedetail.BPID = crm.CusId;
                        warehousedetail.TransType = TransTypeWD.CanRing;
                        warehousedetail.InStockFrom = crm.ID;
                        _context.WarehouseDetails.Add(warehousedetail);
                        _context.SaveChanges();
                        if (itemMaster.Process == "FIFO")
                        {
                            //insert inventoryaudit
                            var inventory_audit = _context.InventoryAudits.Where(w => w.ItemID == item.ItemID && w.WarehouseID == crm.WarehouseID);
                            var pri_detial = _context.PriceListDetails.Where(w => w.ItemID == item.ItemID);
                            item_inventory_audit.ID = 0;
                            item_inventory_audit.WarehouseID = crm.WarehouseID;
                            item_inventory_audit.BranchID = crm.BranchID;
                            item_inventory_audit.UserID = crm.UserID;
                            item_inventory_audit.ItemID = item.ItemID;
                            item_inventory_audit.CurrencyID = crm.SysCurrencyID;
                            item_inventory_audit.UomID = orft.BaseUOM;
                            item_inventory_audit.InvoiceNo = crm.Number;
                            item_inventory_audit.Trans_Type = docType.Code;
                            item_inventory_audit.Process = itemMaster.Process;
                            item_inventory_audit.SystemDate = DateTime.Now;
                            item_inventory_audit.TimeIn = DateTime.Now.ToShortTimeString();
                            item_inventory_audit.Qty = qty;
                            item_inventory_audit.Cost = _cost;
                            item_inventory_audit.Price = 0;
                            item_inventory_audit.CumulativeQty = inventory_audit.Sum(q => q.Qty) + qty;
                            item_inventory_audit.CumulativeValue = inventory_audit.Sum(s => s.Trans_Valuse) + qty * _cost;
                            item_inventory_audit.Trans_Valuse = qty * _cost;
                            //item_inventory_audit.ExpireDate = item.ExpireDate;
                            item_inventory_audit.LocalCurID = crm.LocalCurrencyID;
                            item_inventory_audit.LocalSetRate = crm.LocalSetRate;
                            item_inventory_audit.SeriesDetailID = crm.SeriesDID;
                            item_inventory_audit.SeriesID = crm.SeriesID;
                            item_inventory_audit.DocumentTypeID = crm.DocTypeID;
                            item_inventory_audit.CompanyID = crm.CompanyID;
                            // update pricelistdetial
                            foreach (var pri in pri_detial)
                            {
                                var guom = _context.GroupDUoMs.FirstOrDefault(g => g.GroupUoMID == itemMaster.GroupUomID && g.AltUOM == pri.UomID);
                                var exp = _context.ExchangeRates.FirstOrDefault(w => w.CurrencyID == pri.CurrencyID);
                                pri.Cost = _cost * exp.SetRate * guom.Factor;
                                _context.PriceListDetails.Update(pri);
                            }
                            _utility.CumulativeValue(item_inventory_audit.WarehouseID, item_inventory_audit.ItemID, item_inventory_audit.CumulativeValue, _itemAcc);
                            _context.InventoryAudits.Add(item_inventory_audit);
                            _context.SaveChanges();
                        }
                        else if (itemMaster.Process == "Average")
                        {
                            //insert inventoryaudit
                            var inventory_audit = _context.InventoryAudits.Where(w => w.ItemID == item.ItemID && w.WarehouseID == crm.WarehouseID);
                            var warehouse_sammary = _context.WarehouseSummary.FirstOrDefault(w => w.ItemID == item.ItemID && w.WarehouseID == crm.WarehouseID);
                            var pri_detial = _context.PriceListDetails.Where(w => w.ItemID == item.ItemID);
                            InventoryAudit avgInAudit = new() { Qty = qty, Cost = _cost };
                            double @AvgCost = _utility.CalAVGCost(item.ItemID, crm.WarehouseID, avgInAudit);
                            @AvgCost = _utility.CheckNaNOrInfinity(@AvgCost);
                            item_inventory_audit.ID = 0;
                            item_inventory_audit.WarehouseID = crm.WarehouseID;
                            item_inventory_audit.BranchID = crm.BranchID;
                            item_inventory_audit.UserID = crm.UserID;
                            item_inventory_audit.ItemID = item.ItemID;
                            item_inventory_audit.CurrencyID = crm.SysCurrencyID;
                            item_inventory_audit.UomID = orft.BaseUOM;
                            item_inventory_audit.InvoiceNo = crm.Number;
                            item_inventory_audit.Trans_Type = docType.Code;
                            item_inventory_audit.Process = itemMaster.Process;
                            item_inventory_audit.SystemDate = DateTime.Now;
                            item_inventory_audit.TimeIn = DateTime.Now.ToShortTimeString();
                            item_inventory_audit.Qty = qty;
                            item_inventory_audit.Cost = @AvgCost;
                            item_inventory_audit.Price = 0;
                            item_inventory_audit.CumulativeQty = inventory_audit.Sum(q => q.Qty) + qty;
                            item_inventory_audit.CumulativeValue = inventory_audit.Sum(s => s.Trans_Valuse) + qty * _cost;
                            item_inventory_audit.Trans_Valuse = qty * _cost;
                            //item_inventory_audit.ExpireDate = item.ExpireDate;
                            item_inventory_audit.LocalCurID = crm.LocalCurrencyID;
                            item_inventory_audit.LocalSetRate = crm.LocalSetRate;
                            item_inventory_audit.SeriesDetailID = crm.SeriesDID;
                            item_inventory_audit.SeriesID = crm.SeriesID;
                            item_inventory_audit.DocumentTypeID = crm.DocTypeID;
                            item_inventory_audit.CompanyID = crm.CompanyID;
                            // update_warehouse_summary
                            warehouse_sammary.Cost = @AvgCost;
                            _context.WarehouseSummary.Update(warehouse_sammary);
                            // update_pricelistdetial
                            var inventory_pricelist = _context.InventoryAudits.Where(w => w.ItemID == item.ItemID && w.WarehouseID == crm.WarehouseID).ToList();
                            double @AvgCostPL = inventory_pricelist.Sum(s => s.Trans_Valuse) / inventory_pricelist.Sum(q => q.Qty);
                            @AvgCostPL = _utility.CheckNaNOrInfinity(@AvgCostPL);
                            foreach (var pri in pri_detial)
                            {
                                var guom = _context.GroupDUoMs.Where(g => g.GroupUoMID == itemMaster.GroupUomID && g.AltUOM == pri.UomID);
                                var exp = _context.ExchangeRates.FirstOrDefault(w => w.CurrencyID == pri.CurrencyID);
                                foreach (var g in guom)
                                {
                                    pri.Cost = @AvgCostPL * exp.SetRate * g.Factor;
                                }
                                _context.PriceListDetails.Update(pri);
                            }
                            _utility.CumulativeValue(item_inventory_audit.WarehouseID, item_inventory_audit.ItemID, item_inventory_audit.CumulativeValue, _itemAcc);
                            _context.InventoryAudits.Add(item_inventory_audit);
                            _context.SaveChanges();
                        }
                    }
                }
                #endregion

                #region item change redue stock
                if (itemMasterChange.Process != "Standard")
                {
                    double @Check_Stock;
                    double @Remain;
                    double @IssusQty;
                    double @FIFOQty;
                    double Cost = 0;

                    var _itemAccChange = _context.ItemAccountings.FirstOrDefault(i => i.WarehouseID == crm.WarehouseID && i.ItemID == item.ItemChangeID);
                    var item_warehouse_summaryChange = _context.WarehouseSummary.FirstOrDefault(w => w.WarehouseID == crm.WarehouseID && w.ItemID == item.ItemChangeID);
                    if (item_warehouse_summaryChange != null)
                    {
                        item_warehouse_summaryChange.InStock -= qtyChange;
                        _context.WarehouseSummary.Update(item_warehouse_summaryChange);
                        _utility.UpdateItemAccounting(_itemAccChange, item_warehouse_summaryChange);
                        _context.SaveChanges();
                    }

                    //Checking Serial Batch //
                    if (itemMasterChange.ManItemBy == ManageItemBy.SerialNumbers)
                    {
                        if (serials.Count > 0)
                        {
                            List<WareForAudiView> wareForAudis = new();

                            foreach (var s in serials)
                            {
                                if (s.SerialNumberSelected != null)
                                {
                                    foreach (var ss in s.SerialNumberSelected.SerialNumberSelectedDetails)
                                    {
                                        var waredetial = wareDetailsChange.FirstOrDefault(i => ss.SerialNumber == i.SerialNumber && i.InStock > 0);
                                        decimal _inventoryAccAmount = 0M;
                                        if (waredetial != null)
                                        {
                                            Cost = waredetial.Cost;
                                            wareForAudis.Add(new WareForAudiView
                                            {
                                                Cost = waredetial.Cost,
                                                Qty = waredetial.InStock,
                                                ExpireDate = waredetial.ExpireDate,
                                            });
                                            waredetial.InStock -= 1;
                                            // insert to warehouse detail
                                            var stockOut = new StockOut
                                            {
                                                AdmissionDate = waredetial.AdmissionDate,
                                                Cost = (decimal)waredetial.Cost,
                                                CurrencyID = waredetial.CurrencyID,
                                                Details = waredetial.Details,
                                                ID = 0,
                                                InStock = 1,
                                                ItemID = waredetial.ItemID,
                                                Location = waredetial.Location,
                                                LotNumber = waredetial.LotNumber,
                                                MfrDate = waredetial.MfrDate,
                                                MfrSerialNumber = waredetial.MfrSerialNumber,
                                                MfrWarDateEnd = waredetial.MfrWarDateEnd,
                                                MfrWarDateStart = waredetial.MfrWarDateStart,
                                                ProcessItem = _utility.CheckProcessItem(itemMasterChange.Process),
                                                SerialNumber = waredetial.SerialNumber,
                                                SyetemDate = DateTime.Now,
                                                SysNum = 0,
                                                TimeIn = DateTime.Now,
                                                WarehouseID = waredetial.WarehouseID,
                                                UomID = item.UomChangeID,
                                                UserID = crm.UserID,
                                                ExpireDate = waredetial.ExpireDate,
                                                TransType = TransTypeWD.CanRing,
                                                TransID = crm.ID,
                                                Contract = itemMasterChange.ContractID,
                                                OutStockFrom = crm.ID,
                                                FromWareDetialID = waredetial.ID,
                                                BPID = crm.CusId
                                            };
                                            _inventoryAccAmount = (decimal)waredetial.Cost;
                                            inventoryAccAmount += _inventoryAccAmount;
                                            _context.StockOuts.Add(stockOut);
                                            _context.SaveChanges();
                                        }
                                        InsertFinancialCanRingInventory(inventoryAccID, _inventoryAccAmount, journalEntryDetail, accountBalance, journalEntry, docType, douTypeID, crm, OffsetAcc);
                                    }
                                }
                            }
                            wareForAudis = (from wa in wareForAudis
                                            group wa by wa.Cost into g
                                            let wha = g.FirstOrDefault()
                                            select new WareForAudiView
                                            {
                                                Qty = g.Sum(i => i.Qty),
                                                Cost = wha.Cost,
                                                ExpireDate = wha.ExpireDate,
                                            }).ToList();
                            if (wareForAudis.Any())
                            {
                                foreach (var i in wareForAudis)
                                {
                                    // Insert to Inventory Audit
                                    var inventory_audit = _context.InventoryAudits
                                        .Where(w => w.ItemID == item.ItemChangeID && w.WarehouseID == crm.WarehouseID && w.Cost == i.Cost).ToList();
                                    //var item_IssusStock = wareDetails.FirstOrDefault(w => w.InStock > 0);
                                    var inventory = new InventoryAudit
                                    {
                                        ID = 0,
                                        WarehouseID = crm.WarehouseID,
                                        BranchID = crm.BranchID,
                                        UserID = crm.UserID,
                                        ItemID = item.ItemChangeID,
                                        CurrencyID = crm.SysCurrencyID,
                                        UomID = orftChange.BaseUOM,
                                        InvoiceNo = crm.Number,
                                        Trans_Type = docType.Code,
                                        Process = itemMasterChange.Process,
                                        SystemDate = DateTime.Now,
                                        TimeIn = DateTime.Now.ToShortTimeString().ToString(),
                                        Qty = i.Qty * -1,
                                        Cost = i.Cost,
                                        Price = 0,
                                        CumulativeQty = inventory_audit.Sum(q => q.Qty) - i.Qty,
                                        CumulativeValue = inventory_audit.Sum(s => s.Trans_Valuse) - (i.Qty * i.Cost),
                                        Trans_Valuse = i.Qty * i.Cost * -1,
                                        ExpireDate = i.ExpireDate,
                                        LocalCurID = crm.LocalCurrencyID,
                                        LocalSetRate = crm.LocalSetRate,
                                        CompanyID = crm.CompanyID,
                                        DocumentTypeID = docType.ID,
                                        SeriesID = crm.SeriesID,
                                        SeriesDetailID = crm.SeriesDID,
                                    };
                                    _utility.CumulativeValue(inventory.WarehouseID, inventory.ItemID, inventory.CumulativeValue, _itemAccChange);
                                    _context.InventoryAudits.Add(inventory);
                                    _context.SaveChanges();
                                }
                            }
                        }
                    }
                    else if (itemMasterChange.ManItemBy == ManageItemBy.Batches)
                    {
                        if (batches.Count > 0)
                        {
                            List<WareForAudiView> wareForAudis = new();
                            foreach (var b in batches)
                            {
                                if (b.BatchNoSelected != null)
                                {
                                    foreach (var sb in b.BatchNoSelected.BatchNoSelectedDetails)
                                    {
                                        decimal _inventoryAccAmount = 0M;
                                        decimal selectedQty = sb.SelectedQty * (decimal)orftChange.Factor;
                                        var waredetial = wareDetailsChange.FirstOrDefault(i => sb.BatchNo == i.BatchNo && i.InStock > 0);
                                        if (waredetial != null)
                                        {
                                            wareForAudis.Add(new WareForAudiView
                                            {
                                                Cost = waredetial.Cost,
                                                Qty = (double)selectedQty,
                                                ExpireDate = waredetial.ExpireDate,
                                            });
                                            waredetial.InStock -= (double)selectedQty;
                                            Cost = waredetial.Cost;
                                            // insert to waredetial
                                            var stockOut = new StockOut
                                            {
                                                AdmissionDate = waredetial.AdmissionDate,
                                                Cost = (decimal)waredetial.Cost,
                                                CurrencyID = waredetial.CurrencyID,
                                                Details = waredetial.Details,
                                                ID = 0,
                                                InStock = selectedQty,
                                                ItemID = item.ItemChangeID,
                                                Location = waredetial.Location,
                                                MfrDate = waredetial.MfrDate,
                                                ProcessItem = _utility.CheckProcessItem(itemMasterChange.Process),
                                                SyetemDate = DateTime.Now,
                                                SysNum = 0,
                                                TimeIn = DateTime.Now,
                                                WarehouseID = waredetial.WarehouseID,
                                                UomID = item.UomChangeID,
                                                UserID = crm.UserID,
                                                ExpireDate = waredetial.ExpireDate,
                                                BatchAttr1 = waredetial.BatchAttr1,
                                                BatchAttr2 = waredetial.BatchAttr2,
                                                BatchNo = waredetial.BatchNo,
                                                TransType = TransTypeWD.CanRing,
                                                TransID = crm.ID,
                                                OutStockFrom = crm.ID,
                                                FromWareDetialID = waredetial.ID,
                                                BPID = crm.CusId
                                            };
                                            _inventoryAccAmount = (decimal)waredetial.Cost;
                                            inventoryAccAmount += _inventoryAccAmount;
                                            _context.StockOuts.Add(stockOut);
                                            _context.SaveChanges();
                                        }
                                        InsertFinancialCanRingInventory(inventoryAccID, _inventoryAccAmount, journalEntryDetail, accountBalance, journalEntry, docType, douTypeID, crm, OffsetAcc);
                                    }
                                }
                            }
                            wareForAudis = (from wa in wareForAudis
                                            group wa by wa.Cost into g
                                            let wha = g.FirstOrDefault()
                                            select new WareForAudiView
                                            {
                                                Qty = g.Sum(i => i.Qty),
                                                Cost = wha.Cost,
                                                ExpireDate = wha.ExpireDate
                                            }).ToList();

                            if (wareForAudis.Any())
                            {
                                foreach (var i in wareForAudis)
                                {
                                    // insert to inventory audit
                                    var inventory_audit = _context.InventoryAudits
                                        .Where(w => w.ItemID == item.ItemID && w.WarehouseID == crm.WarehouseID && w.Cost == i.Cost).ToList();
                                    var inventory = new InventoryAudit
                                    {
                                        ID = 0,
                                        WarehouseID = crm.WarehouseID,
                                        BranchID = crm.BranchID,
                                        UserID = crm.UserID,
                                        ItemID = item.ItemID,
                                        CurrencyID = crm.SysCurrencyID,
                                        UomID = orftChange.BaseUOM,
                                        InvoiceNo = crm.Number,
                                        Trans_Type = docType.Code,
                                        Process = itemMasterChange.Process,
                                        SystemDate = DateTime.Now,
                                        TimeIn = DateTime.Now.ToShortTimeString().ToString(),
                                        Qty = i.Qty * -1,
                                        Cost = i.Cost,
                                        Price = 0,
                                        CumulativeQty = inventory_audit.Sum(q => q.Qty) - i.Qty,
                                        CumulativeValue = inventory_audit.Sum(s => s.Trans_Valuse) - (i.Qty * i.Cost),
                                        Trans_Valuse = i.Qty * i.Cost * -1,
                                        ExpireDate = i.ExpireDate,
                                        LocalCurID = crm.LocalCurrencyID,
                                        LocalSetRate = crm.LocalSetRate,
                                        CompanyID = crm.CompanyID,
                                        DocumentTypeID = docType.ID,
                                        SeriesID = crm.SeriesID,
                                        SeriesDetailID = crm.SeriesDID,
                                    };
                                    _utility.CumulativeValue(inventory.WarehouseID, inventory.ItemID, inventory.CumulativeValue, _itemAccChange);
                                    _context.InventoryAudits.Add(inventory);
                                    _context.SaveChanges();
                                }
                            }
                        }
                    }
                    else
                    {
                        List<WarehouseDetail> _whlists = wareDetailsChange.Where(w => w.InStock > 0).OrderBy(i => i.SyetemDate).ToList();
                        if (warehouse.IsAllowNegativeStock && _whlists.Count == 0)
                        {
                            var wh = wareDetailsChange.LastOrDefault();
                            _whlists.Add(wh);
                        }
                        foreach (var (item_warehouse, index) in _whlists.Select((value, i) => (value, i)))
                        {
                            InventoryAudit item_inventory_audit = new();
                            WarehouseDetail item_IssusStock = new();
                            @Check_Stock = item_warehouse.InStock - qtyChange;
                            if (@Check_Stock < 0)
                            {
                                @Remain = @Check_Stock * (-1);
                                @IssusQty = qtyChange - @Remain;
                                if (@Remain <= 0)
                                {
                                    qtyChange = 0;
                                }
                                else if (qtyChange > 0 && index == _whlists.Count - 1 && warehouse.IsAllowNegativeStock)
                                {
                                    @IssusQty = qtyChange;
                                }
                                else
                                {
                                    qtyChange = @Remain;
                                }

                                if (itemMasterChange.Process == "FIFO")
                                {
                                    item_IssusStock = item_warehouse;
                                    double _cost = item_IssusStock.Cost;
                                    item_IssusStock.InStock -= @IssusQty;
                                    if (@IssusQty > 0)
                                    {
                                        var stockOuts = new StockOut
                                        {
                                            Cost = (decimal)item_warehouse.Cost,
                                            CurrencyID = item_warehouse.CurrencyID,
                                            ID = 0,
                                            InStock = (decimal)@IssusQty,
                                            ItemID = item.ItemID,
                                            ProcessItem = ProcessItem.FIFO,
                                            SyetemDate = DateTime.Now,
                                            TimeIn = DateTime.Now,
                                            WarehouseID = item_warehouse.WarehouseID,
                                            UomID = item.UomID,
                                            UserID = crm.UserID,
                                            ExpireDate = item_IssusStock.ExpireDate,
                                            TransType = TransTypeWD.POS,
                                            TransID = crm.ID,
                                            OutStockFrom = crm.ID,
                                            BPID = crm.CusId,
                                            FromWareDetialID = item_IssusStock.ID,
                                        };
                                        _context.StockOuts.Add(stockOuts);
                                        var inventory_audit = _context.InventoryAudits.Where(w => w.ItemID == item.ItemChangeID && w.WarehouseID == crm.WarehouseID).ToList();
                                        item_inventory_audit.ID = 0;
                                        item_inventory_audit.WarehouseID = crm.WarehouseID;
                                        item_inventory_audit.BranchID = crm.BranchID;
                                        item_inventory_audit.UserID = crm.UserID;
                                        item_inventory_audit.ItemID = item.ItemChangeID;
                                        item_inventory_audit.CurrencyID = Com.SystemCurrencyID;
                                        item_inventory_audit.UomID = orftChange.BaseUOM;
                                        item_inventory_audit.InvoiceNo = crm.Number;
                                        item_inventory_audit.Trans_Type = docType.Code;
                                        item_inventory_audit.Process = itemMasterChange.Process;
                                        item_inventory_audit.SystemDate = DateTime.Now;
                                        item_inventory_audit.TimeIn = DateTime.Now.ToShortTimeString().ToString();
                                        item_inventory_audit.Qty = @IssusQty * -1;
                                        item_inventory_audit.Cost = _cost;
                                        item_inventory_audit.Price = 0;
                                        item_inventory_audit.CumulativeQty = inventory_audit.Sum(q => q.Qty) - @IssusQty;
                                        item_inventory_audit.CumulativeValue = inventory_audit.Sum(s => s.Trans_Valuse) - (@IssusQty * _cost);
                                        item_inventory_audit.Trans_Valuse = @IssusQty * item_IssusStock.Cost * -1;
                                        item_inventory_audit.ExpireDate = item_IssusStock.ExpireDate;
                                        item_inventory_audit.LocalCurID = crm.LocalCurrencyID;
                                        item_inventory_audit.LocalSetRate = crm.LocalSetRate;
                                        item_inventory_audit.CompanyID = crm.CompanyID;
                                        item_inventory_audit.DocumentTypeID = docType.ID;
                                        item_inventory_audit.SeriesID = crm.SeriesID;
                                        item_inventory_audit.SeriesDetailID = crm.SeriesDID;
                                    }
                                    inventoryAccAmount += (decimal)(item_inventory_audit.Cost * @IssusQty);
                                    _utility.CumulativeValue(item_inventory_audit.WarehouseID, item_inventory_audit.ItemID, item_inventory_audit.CumulativeValue, _itemAccChange);
                                    _context.WarehouseDetails.Update(item_IssusStock);
                                    _context.InventoryAudits.Add(item_inventory_audit);
                                    _context.SaveChanges();
                                }
                                else if (itemMasterChange.Process == "Average")
                                {
                                    item_IssusStock = wareDetailsChange.OrderByDescending(i => i.SyetemDate).FirstOrDefault();
                                    item_IssusStock.InStock -= @IssusQty;
                                    if (@IssusQty > 0)
                                    {
                                        var warehouse_summary = _context.WarehouseSummary.FirstOrDefault(w => w.ItemID == item.ItemChangeID && w.WarehouseID == crm.WarehouseID);
                                        double @sysAvCost = warehouse_summary.Cost;
                                        var inventory_audit = _context.InventoryAudits.Where(w => w.ItemID == item.ItemChangeID && w.WarehouseID == crm.WarehouseID).ToList();
                                        var stockOuts = new StockOut
                                        {
                                            Cost = (decimal)@sysAvCost,
                                            CurrencyID = item_warehouse.CurrencyID,
                                            ID = 0,
                                            InStock = (decimal)@IssusQty,
                                            ItemID = item.ItemChangeID,
                                            ProcessItem = _utility.CheckProcessItem(itemMasterChange.Process),
                                            SyetemDate = DateTime.Now,
                                            TimeIn = DateTime.Now,
                                            WarehouseID = item_warehouse.WarehouseID,
                                            UomID = item.UomChangeID,
                                            UserID = crm.UserID,
                                            ExpireDate = item_IssusStock.ExpireDate,
                                            TransType = TransTypeWD.CanRing,
                                            TransID = crm.ID,
                                            OutStockFrom = crm.ID,
                                            BPID = crm.CusId,
                                            FromWareDetialID = item_IssusStock.ID,
                                        };
                                        _context.StockOuts.Add(stockOuts);

                                        item_inventory_audit.ID = 0;
                                        item_inventory_audit.WarehouseID = crm.WarehouseID;
                                        item_inventory_audit.BranchID = crm.BranchID;
                                        item_inventory_audit.UserID = crm.UserID;
                                        item_inventory_audit.ItemID = item.ItemChangeID;
                                        item_inventory_audit.CurrencyID = Com.SystemCurrencyID;
                                        item_inventory_audit.UomID = orftChange.BaseUOM;
                                        item_inventory_audit.InvoiceNo = crm.Number;
                                        item_inventory_audit.Trans_Type = docType.Code;
                                        item_inventory_audit.Process = itemMasterChange.Process;
                                        item_inventory_audit.SystemDate = DateTime.Now;
                                        item_inventory_audit.TimeIn = DateTime.Now.ToShortTimeString().ToString();
                                        item_inventory_audit.Qty = @IssusQty * -1;
                                        item_inventory_audit.Cost = @sysAvCost;
                                        item_inventory_audit.Price = 0;
                                        item_inventory_audit.CumulativeQty = inventory_audit.Sum(q => q.Qty) - @IssusQty;
                                        item_inventory_audit.CumulativeValue = inventory_audit.Sum(s => s.Trans_Valuse) - (@IssusQty * @sysAvCost);
                                        item_inventory_audit.Trans_Valuse = @IssusQty * @sysAvCost * -1;
                                        item_inventory_audit.ExpireDate = item_IssusStock.ExpireDate;
                                        item_inventory_audit.LocalCurID = crm.LocalCurrencyID;
                                        item_inventory_audit.LocalSetRate = crm.LocalSetRate;
                                        item_inventory_audit.CompanyID = crm.CompanyID;
                                        item_inventory_audit.DocumentTypeID = docType.ID;
                                        item_inventory_audit.SeriesID = crm.SeriesID;
                                        item_inventory_audit.SeriesDetailID = crm.SeriesDID;
                                    }
                                    double @AvgCost = _utility.CalAVGCost(item.ItemChangeID, crm.WarehouseID, item_inventory_audit);
                                    inventoryAccAmount += (decimal)(@AvgCost * @IssusQty);
                                    _utility.UpdateAvgCost(item_warehouse.ItemID, crm.WarehouseID, itemMasterChange.GroupUomID, item_inventory_audit.Qty, item_inventory_audit.Cost);
                                    _utility.UpdateBomCost(item_warehouse.ItemID, item_inventory_audit.Qty, item_inventory_audit.Cost);
                                    _utility.CumulativeValue(item_inventory_audit.WarehouseID, item_inventory_audit.ItemID, item_inventory_audit.CumulativeValue, _itemAccChange);
                                    _context.WarehouseDetails.Update(item_IssusStock);
                                    _context.InventoryAudits.Add(item_inventory_audit);
                                    _context.SaveChanges();
                                }
                            }
                            else
                            {

                                if (itemMasterChange.Process == "FIFO")
                                {
                                    item_IssusStock = item_warehouse;
                                    @FIFOQty = item_IssusStock.InStock - qtyChange;
                                    @IssusQty = item_IssusStock.InStock - @FIFOQty;
                                    item_IssusStock.InStock = @FIFOQty;
                                    if (@IssusQty > 0)
                                    {
                                        var stockOuts = new StockOut
                                        {
                                            Cost = (decimal)item_warehouse.Cost,
                                            CurrencyID = item_warehouse.CurrencyID,
                                            ID = 0,
                                            InStock = (decimal)@IssusQty,
                                            ItemID = item.ItemChangeID,
                                            ProcessItem = _utility.CheckProcessItem(itemMasterChange.Process),
                                            SyetemDate = DateTime.Now,
                                            TimeIn = DateTime.Now,
                                            WarehouseID = item_warehouse.WarehouseID,
                                            UomID = item.UomChangeID,
                                            UserID = crm.UserID,
                                            ExpireDate = item_IssusStock.ExpireDate,
                                            TransType = TransTypeWD.CanRing,
                                            TransID = crm.ID,
                                            OutStockFrom = crm.ID,
                                            BPID = crm.CusId,
                                            FromWareDetialID = item_IssusStock.ID,
                                        };
                                        _context.StockOuts.Add(stockOuts);
                                        var inventory_audit = _context.InventoryAudits.Where(w => w.ItemID == item.ItemChangeID && w.WarehouseID == crm.WarehouseID).ToList();
                                        item_inventory_audit.ID = 0;
                                        item_inventory_audit.WarehouseID = crm.WarehouseID;
                                        item_inventory_audit.BranchID = crm.BranchID;
                                        item_inventory_audit.UserID = crm.UserID;
                                        item_inventory_audit.ItemID = item.ItemChangeID;
                                        item_inventory_audit.CurrencyID = Com.SystemCurrencyID;
                                        item_inventory_audit.UomID = orftChange.BaseUOM;
                                        item_inventory_audit.InvoiceNo = crm.Number;
                                        item_inventory_audit.Trans_Type = docType.Code;
                                        item_inventory_audit.Process = itemMasterChange.Process;
                                        item_inventory_audit.SystemDate = DateTime.Now;
                                        item_inventory_audit.TimeIn = DateTime.Now.ToShortTimeString().ToString();
                                        item_inventory_audit.Qty = @IssusQty * -1;
                                        item_inventory_audit.Cost = item_IssusStock.Cost;
                                        item_inventory_audit.Price = 0;
                                        item_inventory_audit.CumulativeQty = inventory_audit.Sum(q => q.Qty) - @IssusQty;
                                        item_inventory_audit.CumulativeValue = inventory_audit.Sum(s => s.Trans_Valuse) - (@IssusQty * item_IssusStock.Cost);
                                        item_inventory_audit.Trans_Valuse = @IssusQty * item_IssusStock.Cost * -1;
                                        item_inventory_audit.ExpireDate = item_IssusStock.ExpireDate;
                                        item_inventory_audit.LocalCurID = crm.LocalCurrencyID;
                                        item_inventory_audit.LocalSetRate = crm.LocalSetRate;
                                        item_inventory_audit.CompanyID = crm.CompanyID;
                                        item_inventory_audit.DocumentTypeID = docType.ID;
                                        item_inventory_audit.SeriesID = crm.SeriesID;
                                        item_inventory_audit.SeriesDetailID = crm.SeriesDID;
                                    }
                                    inventoryAccAmount += (decimal)item_inventory_audit.Cost * (decimal)@IssusQty;
                                    _utility.CumulativeValue(item_inventory_audit.WarehouseID, item_inventory_audit.ItemID, item_inventory_audit.CumulativeValue, _itemAccChange);
                                    _context.WarehouseDetails.Update(item_IssusStock);
                                    _context.InventoryAudits.Add(item_inventory_audit);
                                    _context.SaveChanges();
                                }
                                else if (itemMasterChange.Process == "Average")
                                {
                                    item_IssusStock = wareDetailsChange.OrderByDescending(i => i.SyetemDate).FirstOrDefault();
                                    @FIFOQty = item_IssusStock.InStock - qtyChange;
                                    @IssusQty = item_IssusStock.InStock - @FIFOQty;
                                    item_IssusStock.InStock = @FIFOQty;
                                    if (@IssusQty > 0)
                                    {
                                        var warehouse_summary = _context.WarehouseSummary.FirstOrDefault(w => w.ItemID == item.ItemChangeID && w.WarehouseID == crm.WarehouseID);
                                        double @sysAvCost = warehouse_summary.Cost;
                                        var inventory_audit = _context.InventoryAudits.Where(w => w.ItemID == item.ItemChangeID && w.WarehouseID == crm.WarehouseID).ToList();

                                        var stockOuts = new StockOut
                                        {
                                            Cost = (decimal)@sysAvCost,
                                            CurrencyID = item_warehouse.CurrencyID,
                                            ID = 0,
                                            InStock = (decimal)@IssusQty,
                                            ItemID = item.ItemChangeID,
                                            ProcessItem = _utility.CheckProcessItem(itemMasterChange.Process),
                                            SyetemDate = DateTime.Now,
                                            TimeIn = DateTime.Now,
                                            WarehouseID = item_warehouse.WarehouseID,
                                            UomID = item.UomChangeID,
                                            UserID = crm.UserID,
                                            ExpireDate = item_IssusStock.ExpireDate,
                                            TransType = TransTypeWD.CanRing,
                                            TransID = crm.ID,
                                            OutStockFrom = crm.ID,
                                            FromWareDetialID = item_IssusStock.ID,
                                            BPID = crm.CusId
                                        };
                                        _context.StockOuts.Add(stockOuts);
                                        item_inventory_audit.ID = 0;
                                        item_inventory_audit.WarehouseID = crm.WarehouseID;
                                        item_inventory_audit.BranchID = crm.BranchID;
                                        item_inventory_audit.UserID = crm.UserID;
                                        item_inventory_audit.ItemID = item.ItemChangeID;
                                        item_inventory_audit.CurrencyID = Com.SystemCurrencyID;
                                        item_inventory_audit.UomID = orftChange.BaseUOM;
                                        item_inventory_audit.InvoiceNo = crm.Number;
                                        item_inventory_audit.Trans_Type = docType.Code;
                                        item_inventory_audit.Process = itemMasterChange.Process;
                                        item_inventory_audit.SystemDate = DateTime.Now;
                                        item_inventory_audit.TimeIn = DateTime.Now.ToShortTimeString().ToString();
                                        item_inventory_audit.Qty = @IssusQty * -1;
                                        item_inventory_audit.Cost = @sysAvCost;
                                        item_inventory_audit.Price = 0;
                                        item_inventory_audit.CumulativeQty = inventory_audit.Sum(q => q.Qty) - @IssusQty;
                                        item_inventory_audit.CumulativeValue = inventory_audit.Sum(s => s.Trans_Valuse) - (@IssusQty * @sysAvCost);
                                        item_inventory_audit.Trans_Valuse = @IssusQty * @sysAvCost * -1;
                                        item_inventory_audit.ExpireDate = item_IssusStock.ExpireDate;
                                        item_inventory_audit.LocalCurID = crm.LocalCurrencyID;
                                        item_inventory_audit.LocalSetRate = crm.LocalSetRate;
                                        item_inventory_audit.CompanyID = crm.CompanyID;
                                        item_inventory_audit.DocumentTypeID = docType.ID;
                                        item_inventory_audit.SeriesID = crm.SeriesID;
                                        item_inventory_audit.SeriesDetailID = crm.SeriesDID;
                                    }
                                    double @AvgCost = _utility.CalAVGCost(item.ItemChangeID, crm.WarehouseID, item_inventory_audit);
                                    inventoryAccAmount += (decimal)(@AvgCost * @IssusQty);
                                    _utility.UpdateAvgCost(item_warehouse.ItemID, crm.WarehouseID, itemMasterChange.GroupUomID, item_inventory_audit.Qty, item_inventory_audit.Cost);
                                    _utility.UpdateBomCost(item_warehouse.ItemID, item_inventory_audit.Qty, item_inventory_audit.Cost);
                                    _utility.CumulativeValue(item_inventory_audit.WarehouseID, item_inventory_audit.ItemID, item_inventory_audit.CumulativeValue, _itemAccChange);
                                    _context.WarehouseDetails.Update(item_IssusStock);
                                    _context.InventoryAudits.Add(item_inventory_audit);
                                    _context.SaveChanges();
                                }
                                wareDetailsChange = new List<WarehouseDetail>();
                                break;
                            }
                        }
                    }
                }
                else
                {
                    var priceListDetail = _context.PriceListDetails.FirstOrDefault(w => w.ItemID == item.ItemChangeID && w.UomID == item.UomChangeID && w.PriceListID == crm.PriceListID) ?? new PriceListDetail();
                    inventoryAccAmount += (decimal)priceListDetail.Cost * item.ChangeQty * crm.ExchangeRate;
                    var inventory_audit = _context.InventoryAudits.Where(w => w.ItemID == item.ItemChangeID && w.WarehouseID == crm.WarehouseID).ToList();
                    InventoryAudit item_inventory_audit = new()
                    {
                        ID = 0,
                        WarehouseID = crm.WarehouseID,
                        BranchID = crm.BranchID,
                        UserID = crm.UserID,
                        ItemID = item.ItemChangeID,
                        CurrencyID = Com.SystemCurrencyID,
                        UomID = orftChange.BaseUOM,
                        InvoiceNo = crm.Number,
                        Trans_Type = docType.Code,
                        Process = itemMasterChange.Process,
                        SystemDate = DateTime.Now,
                        TimeIn = DateTime.Now.ToShortTimeString().ToString(),
                        Qty = qtyChange * -1,
                        Cost = priceListDetail.Cost,
                        Price = 0,
                        CumulativeQty = inventory_audit.Sum(q => q.Qty) - qtyChange,
                        CumulativeValue = inventory_audit.Sum(s => s.Trans_Valuse) - (qtyChange * priceListDetail.Cost),
                        Trans_Valuse = qtyChange * priceListDetail.Cost * -1,
                        //ExpireDate = item_IssusStock.ExpireDate,
                        LocalCurID = crm.LocalCurrencyID,
                        LocalSetRate = crm.LocalSetRate,
                        CompanyID = crm.CompanyID,
                        DocumentTypeID = docType.ID,
                        SeriesID = crm.SeriesID,
                        SeriesDetailID = crm.SeriesDID,
                        TypeItem = "Standard",
                    };
                    _context.InventoryAudits.Update(item_inventory_audit);
                    _context.SaveChanges();
                }
                #endregion

                var glAccRevenfifo = _context.GLAccounts.FirstOrDefault(w => w.ID == revenueAccID) ?? new GLAccount();
                if (glAccRevenfifo.ID > 0)
                {
                    var list = journalEntryDetail.FirstOrDefault(w => w.ItemID == glAccRevenfifo.ID) ?? new JournalEntryDetail();
                    if (list.ItemID > 0)
                    {
                        var accBalance = accountBalance.FirstOrDefault(w => w.GLAID == revenueAccID);
                        glAccRevenfifo.Balance -= revenueAccAmount;
                        //journalEntryDetail
                        list.Credit += revenueAccAmount;
                        //accountBalance
                        accBalance.CumulativeBalance = glAccRevenfifo.Balance;
                        accBalance.Credit += revenueAccAmount;
                    }
                    else
                    {
                        glAccRevenfifo.Balance -= revenueAccAmount;
                        journalEntryDetail.Add(new JournalEntryDetail
                        {
                            JEID = journalEntry.ID,
                            Type = Type.GLAcct,
                            ItemID = revenueAccID,
                            Credit = revenueAccAmount,
                        });
                        //
                        accountBalance.Add(new AccountBalance
                        {
                            JEID = journalEntry.ID,

                            PostingDate = crm.CreatedAt,
                            Origin = docType.ID,
                            OriginNo = crm.Number,
                            OffsetAccount = OffsetAcc,
                            Details = douTypeID.Name + "-" + glAccRevenfifo.Code,
                            CumulativeBalance = glAccRevenfifo.Balance,
                            Credit = revenueAccAmount,
                            LocalSetRate = (decimal)crm.LocalSetRate,
                            GLAID = revenueAccID,
                        });
                    }
                    _context.Update(glAccRevenfifo);
                }

                InsertFinancialCanRingInventory(inventoryAccIncreaseID, inventoryAccAmount, journalEntryDetail, accountBalance, journalEntry, docType, douTypeID, crm, OffsetAcc, false);
                if (itemMasterChange.ManItemBy == ManageItemBy.None)
                {
                    InsertFinancialCanRingInventory(inventoryAccID, inventoryAccAmount, journalEntryDetail, accountBalance, journalEntry, docType, douTypeID, crm, OffsetAcc);
                }
            }
            List<ItemMaterial> itemMaterials = new();
            foreach (var item in crm.CanRingDetials)
            {
                var itemM = _context.ItemMasterDatas.FirstOrDefault(w => w.ID == item.ItemChangeID);
                var orft = _context.GroupDUoMs.FirstOrDefault(w => w.GroupUoMID == itemM.GroupUomID && w.AltUOM == item.UomChangeID);
                var bom = _context.BOMaterial.FirstOrDefault(w => w.ItemID == item.ItemChangeID && w.Active == true) ?? new BOMaterial();
                var items_material = (from bomd in _context.BOMDetail.Where(w => w.BID == bom.BID && w.Detele == false)
                                      join i in _context.ItemMasterDatas.Where(im => !im.Delete) on bomd.ItemID equals i.ID
                                      join gd in _context.GroupDUoMs on bomd.UomID equals gd.AltUOM
                                      join uom in _context.UnitofMeasures on i.InventoryUoMID equals uom.ID
                                      select new ItemMaterial
                                      {
                                          ItemID = bomd.ItemID,
                                          GroupUoMID = gd.GroupUoMID,
                                          GUoMID = i.GroupUomID,
                                          Qty = (double)item.ChangeQty * orft.Factor * ((double)bomd.Qty * gd.Factor),
                                          NegativeStock = bomd.NegativeStock,
                                          Process = i.Process,
                                          UomID = uom.ID,
                                          Factor = gd.Factor,
                                      }).Where(w => w.GroupUoMID == w.GUoMID).ToList();
                itemMaterials.AddRange(items_material);
                #region bom normal item increase stock
                var ItemBOMDetail = _context.BOMDetail.Where(w => w.ItemID == item.ItemID);
                foreach (var itembom in ItemBOMDetail)
                {
                    var BOM = _context.BOMaterial.First(w => w.BID == itembom.BID);
                    var DBOM = _context.BOMDetail.Where(w => w.BID == BOM.BID && w.Detele == false);
                    var Inven = _context.InventoryAudits.Where(w => w.ItemID == itembom.ItemID);
                    double @AvgCost = Inven.Sum(s => s.Trans_Valuse) / Inven.Sum(q => q.Qty);
                    @AvgCost = _utility.CheckNaNOrInfinity(@AvgCost);
                    var Gdoum = _context.GroupDUoMs.Where(w => w.GroupUoMID == itembom.GUomID);
                    var Factor = Gdoum.FirstOrDefault(w => w.AltUOM == itembom.UomID).Factor;
                    itembom.Cost = @AvgCost * Factor;
                    itembom.Amount = itembom.Qty * (@AvgCost * Factor);
                    _context.BOMDetail.UpdateRange(ItemBOMDetail);
                    _context.SaveChanges();
                    BOM.TotalCost = DBOM.Sum(w => w.Amount);
                    _context.BOMaterial.Update(BOM);
                    _context.SaveChanges();
                }
                #endregion
            }

            #region bom change item reduce stock
            var allMaterials = (from all in itemMaterials
                                group new { all } by new { all.ItemID, all.NegativeStock } into g
                                let data = g.FirstOrDefault()
                                select new
                                {
                                    data.all.ItemID,
                                    data.all.GroupUoMID,
                                    data.all.GUoMID,
                                    Qty = g.Sum(s => s.all.Qty),
                                    data.all.NegativeStock,
                                    data.all.Process,
                                    data.all.UomID,
                                    data.all.Factor,
                                }).ToList();
            if (allMaterials.Count > 0)
            {
                foreach (var item_detail in allMaterials.ToList())
                {
                    var item_master_data = _context.ItemMasterDatas.FirstOrDefault(w => w.ID == item_detail.ItemID);
                    var item_warehouse_material = _context.WarehouseSummary.FirstOrDefault(w => w.WarehouseID == crm.WarehouseID && w.ItemID == item_detail.ItemID);
                    var _itemAcc = _context.ItemAccountings.FirstOrDefault(i => i.WarehouseID == crm.WarehouseID && i.ItemID == item_detail.ItemID);
                    var all_item_warehouse_detail = _context.WarehouseDetails.Where(w => w.WarehouseID == crm.WarehouseID && w.ItemID == item_detail.ItemID).ToList();
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
                    _utility.UpdateItemAccounting(_itemAcc, item_warehouse_material);
                    if (item_detail.NegativeStock == true && nagative_check <= 0)
                    {
                        double @IssusQty;
                        double @FIFOQty;
                        double qty = item_detail.Qty;
                        var item_inventory_audit = new InventoryAudit();
                        var item_IssusStock = all_item_warehouse_detail.LastOrDefault(w => w.InStock <= 0);
                        @FIFOQty = item_IssusStock.InStock - qty;
                        @IssusQty = item_IssusStock.InStock - @FIFOQty;
                        if (item_detail.Process == "FIFO")
                        {
                            item_IssusStock.InStock = @FIFOQty;
                            if (@IssusQty > 0)
                            {
                                var stockOuts = new StockOut
                                {
                                    Cost = (decimal)item_IssusStock.Cost,
                                    CurrencyID = item_IssusStock.CurrencyID,
                                    ID = 0,
                                    InStock = (decimal)@IssusQty,
                                    ItemID = item_detail.ItemID,
                                    ProcessItem = ProcessItem.FIFO,
                                    SyetemDate = DateTime.Now,
                                    TimeIn = DateTime.Now,
                                    WarehouseID = item_IssusStock.WarehouseID,
                                    UomID = item_detail.UomID,
                                    UserID = crm.UserID,
                                    ExpireDate = item_IssusStock.ExpireDate,
                                    TransType = TransTypeWD.CanRing,
                                    TransID = crm.ID,
                                    OutStockFrom = crm.ID,
                                    BPID = crm.CusId,
                                    FromWareDetialID = item_IssusStock.ID
                                };
                                _context.StockOuts.Add(stockOuts);

                                var inventory_audit = _context.InventoryAudits.Where(w => w.ItemID == item_detail.ItemID && w.WarehouseID == crm.WarehouseID).ToList();
                                item_inventory_audit.ID = 0;
                                item_inventory_audit.WarehouseID = crm.WarehouseID;
                                item_inventory_audit.BranchID = crm.BranchID;
                                item_inventory_audit.UserID = crm.UserID;
                                item_inventory_audit.ItemID = item_detail.ItemID;
                                item_inventory_audit.CurrencyID = Com.SystemCurrencyID;
                                item_inventory_audit.UomID = item_detail.UomID;
                                item_inventory_audit.InvoiceNo = crm.Number;
                                item_inventory_audit.Trans_Type = docType.Code;
                                item_inventory_audit.Process = item_detail.Process;
                                item_inventory_audit.SystemDate = DateTime.Now;
                                item_inventory_audit.TimeIn = DateTime.Now.ToShortTimeString();
                                item_inventory_audit.Qty = @IssusQty * -1;
                                item_inventory_audit.Cost = item_IssusStock.Cost;
                                item_inventory_audit.Price = 0;
                                item_inventory_audit.CumulativeQty = inventory_audit.Sum(q => q.Qty) - @IssusQty;
                                item_inventory_audit.CumulativeValue = inventory_audit.Sum(s => s.Trans_Valuse) - (@IssusQty * item_IssusStock.Cost);
                                item_inventory_audit.Trans_Valuse = @IssusQty * item_IssusStock.Cost * (-1);
                                item_inventory_audit.ExpireDate = item_IssusStock.ExpireDate;
                                item_inventory_audit.LocalCurID = Com.LocalCurrencyID;
                                item_inventory_audit.LocalSetRate = Exr.SetRate;
                                item_inventory_audit.CompanyID = crm.CompanyID;
                                item_inventory_audit.DocumentTypeID = docType.ID;
                                item_inventory_audit.SeriesID = crm.SeriesID;
                                item_inventory_audit.SeriesDetailID = crm.SeriesDID;
                            }
                            _utility.CumulativeValue(item_inventory_audit.WarehouseID, item_inventory_audit.ItemID, item_inventory_audit.CumulativeValue, _itemAcc);
                        }
                        else if (item_detail.Process == "Average")
                        {
                            item_IssusStock.InStock = @FIFOQty;
                            if (@IssusQty > 0)
                            {
                                var warehouse_summary = _context.WarehouseSummary.FirstOrDefault(w => w.ItemID == item_detail.ItemID && w.WarehouseID == crm.WarehouseID);
                                double @sysAvCost = warehouse_summary.Cost;
                                var inventory_audit = _context.InventoryAudits.Where(w => w.ItemID == item_detail.ItemID && w.WarehouseID == crm.WarehouseID).ToList();

                                var stockOuts = new StockOut
                                {
                                    Cost = (decimal)@sysAvCost,
                                    CurrencyID = item_IssusStock.CurrencyID,
                                    ID = 0,
                                    InStock = (decimal)@IssusQty,
                                    ItemID = item_detail.ItemID,
                                    ProcessItem = ProcessItem.Average,
                                    SyetemDate = DateTime.Now,
                                    TimeIn = DateTime.Now,
                                    WarehouseID = item_IssusStock.WarehouseID,
                                    UomID = item_detail.UomID,
                                    UserID = crm.UserID,
                                    ExpireDate = item_IssusStock.ExpireDate,
                                    TransType = TransTypeWD.CanRing,
                                    TransID = crm.ID,
                                    OutStockFrom = crm.ID,
                                    BPID = crm.CusId,
                                    FromWareDetialID = item_IssusStock.ID
                                };
                                _context.StockOuts.Add(stockOuts);
                                item_inventory_audit.ID = 0;
                                item_inventory_audit.WarehouseID = crm.WarehouseID;
                                item_inventory_audit.BranchID = crm.BranchID;
                                item_inventory_audit.UserID = crm.UserID;
                                item_inventory_audit.ItemID = item_detail.ItemID;
                                item_inventory_audit.CurrencyID = Com.SystemCurrencyID;
                                item_inventory_audit.UomID = item_detail.UomID;
                                item_inventory_audit.InvoiceNo = crm.Number;
                                item_inventory_audit.Trans_Type = docType.Code;
                                item_inventory_audit.Process = item_detail.Process;
                                item_inventory_audit.SystemDate = DateTime.Now;
                                item_inventory_audit.TimeIn = DateTime.Now.ToShortTimeString();
                                item_inventory_audit.Qty = @IssusQty * -1;
                                item_inventory_audit.Cost = @sysAvCost;
                                item_inventory_audit.Price = 0;
                                item_inventory_audit.CumulativeQty = inventory_audit.Sum(q => q.Qty) - @IssusQty;
                                item_inventory_audit.CumulativeValue = inventory_audit.Sum(s => s.Trans_Valuse) - (@IssusQty * @sysAvCost);
                                item_inventory_audit.Trans_Valuse = @IssusQty * @sysAvCost * (-1);
                                item_inventory_audit.ExpireDate = item_IssusStock.ExpireDate;
                                item_inventory_audit.LocalCurID = Com.LocalCurrencyID;
                                item_inventory_audit.LocalSetRate = Exr.SetRate;
                                item_inventory_audit.CompanyID = crm.CompanyID;
                                item_inventory_audit.DocumentTypeID = docType.ID;
                                item_inventory_audit.SeriesID = crm.SeriesID;
                                item_inventory_audit.SeriesDetailID = crm.SeriesDID;
                            }
                            _utility.UpdateAvgCost(item_detail.ItemID, crm.WarehouseID, item_master_data.GroupUomID, item_inventory_audit.Qty, item_inventory_audit.Cost);
                            _utility.UpdateBomCost(item_detail.ItemID, item_inventory_audit.Qty, item_inventory_audit.Cost);
                            _utility.CumulativeValue(item_inventory_audit.WarehouseID, item_inventory_audit.ItemID, item_inventory_audit.CumulativeValue, _itemAcc);
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
                        double qty = item_detail.Qty;
                        foreach (var item_warehouse in all_item_warehouse_detail.Where(w => w.InStock > 0))
                        {
                            InventoryAudit item_inventory_audit = new();
                            var item_IssusStock = all_item_warehouse_detail.FirstOrDefault(w => w.InStock > 0);
                            @Check_Stock = item_warehouse.InStock - qty;
                            if (@Check_Stock < 0)
                            {
                                @Remain = (item_warehouse.InStock - qty) * (-1);
                                @IssusQty = qty - @Remain;
                                if (@Remain <= 0)
                                {
                                    qty = 0;
                                }
                                else
                                {
                                    qty = @Remain;
                                }
                                if (item_detail.Process == "FIFO")
                                {
                                    item_IssusStock.InStock = item_IssusStock.InStock -= @IssusQty;
                                    if (@IssusQty > 0)
                                    {
                                        var stockOuts = new StockOut
                                        {
                                            Cost = (decimal)item_IssusStock.Cost,
                                            CurrencyID = item_IssusStock.CurrencyID,
                                            ID = 0,
                                            InStock = (decimal)@IssusQty,
                                            ItemID = item_detail.ItemID,
                                            ProcessItem = ProcessItem.FIFO,
                                            SyetemDate = DateTime.Now,
                                            TimeIn = DateTime.Now,
                                            WarehouseID = item_IssusStock.WarehouseID,
                                            UomID = item_detail.UomID,
                                            UserID = crm.UserID,
                                            ExpireDate = item_IssusStock.ExpireDate,
                                            TransType = TransTypeWD.CanRing,
                                            TransID = crm.ID,
                                            OutStockFrom = crm.ID,
                                            BPID = crm.CusId,
                                            FromWareDetialID = item_IssusStock.ID
                                        };
                                        _context.StockOuts.Add(stockOuts);
                                        var inventory_audit = _context.InventoryAudits.Where(w => w.ItemID == item_detail.ItemID && w.WarehouseID == crm.WarehouseID).ToList();
                                        item_inventory_audit.ID = 0;
                                        item_inventory_audit.WarehouseID = crm.WarehouseID;
                                        item_inventory_audit.BranchID = crm.BranchID;
                                        item_inventory_audit.UserID = crm.UserID;
                                        item_inventory_audit.ItemID = item_detail.ItemID;
                                        item_inventory_audit.CurrencyID = Com.SystemCurrencyID;
                                        item_inventory_audit.UomID = item_detail.UomID;
                                        item_inventory_audit.InvoiceNo = crm.Number;
                                        item_inventory_audit.Trans_Type = docType.Code;
                                        item_inventory_audit.Process = item_detail.Process;
                                        item_inventory_audit.SystemDate = DateTime.Now;
                                        item_inventory_audit.TimeIn = DateTime.Now.ToShortTimeString();
                                        item_inventory_audit.Qty = @IssusQty * -1;
                                        item_inventory_audit.Cost = item_IssusStock.Cost;
                                        item_inventory_audit.Price = 0;
                                        item_inventory_audit.CumulativeQty = inventory_audit.Sum(q => q.Qty) + (@IssusQty * (-1));
                                        item_inventory_audit.CumulativeValue = (inventory_audit.Sum(s => s.Trans_Valuse)) + (@IssusQty * item_IssusStock.Cost) * (-1);
                                        item_inventory_audit.Trans_Valuse = (@IssusQty * item_IssusStock.Cost) * (-1);
                                        item_inventory_audit.ExpireDate = item_IssusStock.ExpireDate;
                                        item_inventory_audit.LocalCurID = Com.LocalCurrencyID;
                                        item_inventory_audit.LocalSetRate = Exr.SetRate;
                                        item_inventory_audit.CompanyID = crm.CompanyID;
                                        item_inventory_audit.DocumentTypeID = docType.ID;
                                        item_inventory_audit.SeriesID = crm.SeriesID;
                                        item_inventory_audit.SeriesDetailID = crm.SeriesDID;
                                    }
                                    _utility.CumulativeValue(item_inventory_audit.WarehouseID, item_inventory_audit.ItemID, item_inventory_audit.CumulativeValue, _itemAcc);
                                    _context.WarehouseDetails.Update(item_IssusStock);
                                    _context.InventoryAudits.Add(item_inventory_audit);
                                    _context.SaveChanges();
                                }
                                else if (item_detail.Process == "Average")
                                {
                                    item_IssusStock.InStock = item_IssusStock.InStock -= @IssusQty;
                                    if (@IssusQty > 0)
                                    {
                                        var warehouse_summary = _context.WarehouseSummary.FirstOrDefault(w => w.ItemID == item_detail.ItemID && w.WarehouseID == crm.WarehouseID);
                                        double @sysAvCost = warehouse_summary.Cost;
                                        var inventory_audit = _context.InventoryAudits.Where(w => w.ItemID == item_detail.ItemID && w.WarehouseID == crm.WarehouseID).ToList();

                                        var stockOuts = new StockOut
                                        {
                                            Cost = (decimal)item_IssusStock.Cost,
                                            CurrencyID = item_IssusStock.CurrencyID,
                                            ID = 0,
                                            InStock = (decimal)@IssusQty,
                                            ItemID = item_detail.ItemID,
                                            ProcessItem = ProcessItem.FIFO,
                                            SyetemDate = DateTime.Now,
                                            TimeIn = DateTime.Now,
                                            WarehouseID = item_IssusStock.WarehouseID,
                                            UomID = item_detail.UomID,
                                            UserID = crm.UserID,
                                            ExpireDate = item_IssusStock.ExpireDate,
                                            TransType = TransTypeWD.CanRing,
                                            TransID = crm.ID,
                                            OutStockFrom = crm.ID,
                                            BPID = crm.CusId,
                                            FromWareDetialID = item_IssusStock.ID
                                        };
                                        _context.StockOuts.Add(stockOuts);
                                        item_inventory_audit.ID = 0;
                                        item_inventory_audit.WarehouseID = crm.WarehouseID;
                                        item_inventory_audit.BranchID = crm.BranchID;
                                        item_inventory_audit.UserID = crm.UserID;
                                        item_inventory_audit.ItemID = item_detail.ItemID;
                                        item_inventory_audit.CurrencyID = Com.SystemCurrencyID;
                                        item_inventory_audit.UomID = item_detail.UomID;
                                        item_inventory_audit.InvoiceNo = crm.Number;
                                        item_inventory_audit.Trans_Type = docType.Code;
                                        item_inventory_audit.Process = item_detail.Process;
                                        item_inventory_audit.SystemDate = DateTime.Now;
                                        item_inventory_audit.TimeIn = DateTime.Now.ToShortTimeString();
                                        item_inventory_audit.Qty = @IssusQty * -1;
                                        item_inventory_audit.Cost = @sysAvCost;
                                        item_inventory_audit.Price = 0;
                                        item_inventory_audit.CumulativeQty = inventory_audit.Sum(q => q.Qty) + (@IssusQty * (-1));
                                        item_inventory_audit.CumulativeValue = (inventory_audit.Sum(s => s.Trans_Valuse)) + (@IssusQty * @sysAvCost) * (-1);
                                        item_inventory_audit.Trans_Valuse = (@IssusQty * @sysAvCost) * (-1);
                                        item_inventory_audit.ExpireDate = item_IssusStock.ExpireDate;
                                        item_inventory_audit.LocalCurID = Com.LocalCurrencyID;
                                        item_inventory_audit.LocalSetRate = Exr.SetRate;
                                        item_inventory_audit.CompanyID = crm.CompanyID;
                                        item_inventory_audit.DocumentTypeID = docType.ID;
                                        item_inventory_audit.SeriesID = crm.SeriesID;
                                        item_inventory_audit.SeriesDetailID = crm.SeriesDID;
                                        item_inventory_audit.DocumentTypeID = docType.ID;
                                        item_inventory_audit.SeriesID = crm.SeriesID;
                                        item_inventory_audit.SeriesDetailID = crm.SeriesDID;
                                    }
                                    _utility.UpdateAvgCost(item_detail.ItemID, crm.WarehouseID, item_master_data.GroupUomID, item_inventory_audit.Qty, item_inventory_audit.Cost);
                                    _utility.UpdateBomCost(item_detail.ItemID, item_inventory_audit.Qty, item_inventory_audit.Cost);
                                    _utility.CumulativeValue(item_inventory_audit.WarehouseID, item_inventory_audit.ItemID, item_inventory_audit.CumulativeValue, _itemAcc);
                                    _context.WarehouseDetails.Update(item_IssusStock);
                                    _context.InventoryAudits.Add(item_inventory_audit);
                                    _context.SaveChanges();
                                }
                            }
                            else
                            {
                                @FIFOQty = item_IssusStock.InStock - qty;
                                @IssusQty = item_IssusStock.InStock - @FIFOQty;
                                if (item_detail.Process == "FIFO")
                                {
                                    item_IssusStock.InStock = @FIFOQty;
                                    if (@IssusQty > 0)
                                    {

                                        var stockOuts = new StockOut
                                        {
                                            Cost = (decimal)item_IssusStock.Cost,
                                            CurrencyID = item_IssusStock.CurrencyID,
                                            ID = 0,
                                            InStock = (decimal)@IssusQty,
                                            ItemID = item_detail.ItemID,
                                            ProcessItem = ProcessItem.FIFO,
                                            SyetemDate = DateTime.Now,
                                            TimeIn = DateTime.Now,
                                            WarehouseID = item_IssusStock.WarehouseID,
                                            UomID = item_detail.UomID,
                                            UserID = crm.UserID,
                                            ExpireDate = item_IssusStock.ExpireDate,
                                            TransType = TransTypeWD.CanRing,
                                            TransID = crm.ID,
                                            OutStockFrom = crm.ID,
                                            BPID = crm.CusId,
                                            FromWareDetialID = item_IssusStock.ID
                                        };
                                        _context.StockOuts.Add(stockOuts);
                                        var inventory_audit = _context.InventoryAudits.Where(w => w.ItemID == item_detail.ItemID && w.WarehouseID == crm.WarehouseID).ToList();
                                        item_inventory_audit.ID = 0;
                                        item_inventory_audit.WarehouseID = crm.WarehouseID;
                                        item_inventory_audit.BranchID = crm.BranchID;
                                        item_inventory_audit.UserID = crm.UserID;
                                        item_inventory_audit.ItemID = item_detail.ItemID;
                                        item_inventory_audit.CurrencyID = Com.SystemCurrencyID;
                                        item_inventory_audit.UomID = item_detail.UomID;
                                        item_inventory_audit.InvoiceNo = crm.Number;
                                        item_inventory_audit.Trans_Type = docType.Code;
                                        item_inventory_audit.Process = item_detail.Process;
                                        item_inventory_audit.SystemDate = DateTime.Now;
                                        item_inventory_audit.TimeIn = DateTime.Now.ToShortTimeString();
                                        item_inventory_audit.Qty = @IssusQty * -1;
                                        item_inventory_audit.Cost = item_IssusStock.Cost;
                                        item_inventory_audit.Price = 0;
                                        item_inventory_audit.CumulativeQty = inventory_audit.Sum(q => q.Qty) - @IssusQty;
                                        item_inventory_audit.CumulativeValue = inventory_audit.Sum(s => s.Trans_Valuse) - @IssusQty * item_IssusStock.Cost;
                                        item_inventory_audit.Trans_Valuse = @IssusQty * item_IssusStock.Cost * -1;
                                        item_inventory_audit.ExpireDate = item_IssusStock.ExpireDate;
                                        item_inventory_audit.LocalCurID = Com.LocalCurrencyID;
                                        item_inventory_audit.LocalSetRate = Exr.SetRate;
                                        item_inventory_audit.CompanyID = crm.CompanyID;
                                        item_inventory_audit.DocumentTypeID = docType.ID;
                                        item_inventory_audit.SeriesID = crm.SeriesID;
                                        item_inventory_audit.SeriesDetailID = crm.SeriesDID;
                                        item_inventory_audit.DocumentTypeID = docType.ID;
                                        item_inventory_audit.SeriesID = crm.SeriesID;
                                        item_inventory_audit.SeriesDetailID = crm.SeriesDID;
                                    }
                                    _utility.CumulativeValue(item_inventory_audit.WarehouseID, item_inventory_audit.ItemID, item_inventory_audit.CumulativeValue, _itemAcc);
                                    _context.WarehouseDetails.Update(item_IssusStock);
                                    _context.InventoryAudits.Add(item_inventory_audit);
                                    _context.SaveChanges();
                                }
                                else if (item_detail.Process == "Average")
                                {
                                    item_IssusStock.InStock = @FIFOQty;
                                    if (@IssusQty > 0)
                                    {
                                        var warehouse_summary = _context.WarehouseSummary.FirstOrDefault(w => w.ItemID == item_detail.ItemID && w.WarehouseID == crm.WarehouseID);
                                        double @sysAvCost = warehouse_summary.Cost;
                                        var inventory_audit = _context.InventoryAudits.Where(w => w.ItemID == item_detail.ItemID && w.WarehouseID == crm.WarehouseID).ToList();

                                        var stockOuts = new StockOut
                                        {
                                            Cost = (decimal)item_IssusStock.Cost,
                                            CurrencyID = item_IssusStock.CurrencyID,
                                            ID = 0,
                                            InStock = (decimal)@IssusQty,
                                            ItemID = item_detail.ItemID,
                                            ProcessItem = ProcessItem.FIFO,
                                            SyetemDate = DateTime.Now,
                                            TimeIn = DateTime.Now,
                                            WarehouseID = item_IssusStock.WarehouseID,
                                            UomID = item_detail.UomID,
                                            UserID = crm.UserID,
                                            ExpireDate = item_IssusStock.ExpireDate,
                                            TransType = TransTypeWD.CanRing,
                                            TransID = crm.ID,
                                            OutStockFrom = crm.ID,
                                            BPID = crm.CusId,
                                            FromWareDetialID = item_IssusStock.ID
                                        };
                                        _context.StockOuts.Add(stockOuts);
                                        item_inventory_audit.ID = 0;
                                        item_inventory_audit.WarehouseID = crm.WarehouseID;
                                        item_inventory_audit.BranchID = crm.BranchID;
                                        item_inventory_audit.UserID = crm.UserID;
                                        item_inventory_audit.ItemID = item_detail.ItemID;
                                        item_inventory_audit.CurrencyID = Com.SystemCurrencyID;
                                        item_inventory_audit.UomID = item_detail.UomID;
                                        item_inventory_audit.InvoiceNo = crm.Number;
                                        item_inventory_audit.Trans_Type = docType.Code;
                                        item_inventory_audit.Process = item_detail.Process;
                                        item_inventory_audit.SystemDate = DateTime.Now;
                                        item_inventory_audit.TimeIn = DateTime.Now.ToShortTimeString();
                                        item_inventory_audit.Qty = @IssusQty * -1;
                                        item_inventory_audit.Cost = @sysAvCost;
                                        item_inventory_audit.Price = 0;
                                        item_inventory_audit.CumulativeQty = inventory_audit.Sum(q => q.Qty) - @IssusQty;
                                        item_inventory_audit.CumulativeValue = inventory_audit.Sum(s => s.Trans_Valuse) - (@IssusQty * @sysAvCost);
                                        item_inventory_audit.Trans_Valuse = @IssusQty * @sysAvCost * -1;
                                        item_inventory_audit.ExpireDate = item_IssusStock.ExpireDate;
                                        item_inventory_audit.LocalCurID = Com.LocalCurrencyID;
                                        item_inventory_audit.LocalSetRate = Exr.SetRate;
                                        item_inventory_audit.CompanyID = crm.CompanyID;
                                        item_inventory_audit.DocumentTypeID = docType.ID;
                                        item_inventory_audit.SeriesID = crm.SeriesID;
                                        item_inventory_audit.SeriesDetailID = crm.SeriesDID;
                                    }
                                    _utility.UpdateAvgCost(item_detail.ItemID, crm.WarehouseID, item_master_data.GroupUomID, item_inventory_audit.Qty, item_inventory_audit.Cost);
                                    _utility.UpdateBomCost(item_detail.ItemID, item_inventory_audit.Qty, item_inventory_audit.Cost);
                                    _utility.CumulativeValue(item_inventory_audit.WarehouseID, item_inventory_audit.ItemID, item_inventory_audit.CumulativeValue, _itemAcc);

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
            #endregion
            if (journalEntry.ID > 0)
            {
                journalEntry.TotalDebit = journalEntryDetail.Sum(s => s.Debit);
                journalEntry.TotalCredit = journalEntryDetail.Sum(s => s.Credit);
                _context.JournalEntries.Update(journalEntry);
            }
            _context.JournalEntryDetails.UpdateRange(journalEntryDetail);
            _context.AccountBalances.UpdateRange(accountBalance);
            _context.SaveChanges();
        }
        private void InsertFinancialCanRingInventory(
            int inventoryAccID, decimal inventoryAccAmount, List<JournalEntryDetail> journalEntryDetail, List<AccountBalance> accountBalance,
            JournalEntry journalEntry, DocumentType docType, DocumentType douTypeID, CanRingMaster crm,
            string OffsetAcc, bool itemChange = true)
        {
            if (itemChange)
            {
                //inventoryAccID
                var glAccInvenfifo = _context.GLAccounts.FirstOrDefault(w => w.ID == inventoryAccID) ?? new GLAccount();
                if (glAccInvenfifo.ID > 0)
                {
                    var journalDetail = journalEntryDetail.FirstOrDefault(w => w.ItemID == glAccInvenfifo.ID) ?? new JournalEntryDetail();
                    if (journalDetail.ItemID > 0)
                    {
                        var accBalance = accountBalance.FirstOrDefault(w => w.GLAID == inventoryAccID);
                        glAccInvenfifo.Balance -= inventoryAccAmount;
                        //journalEntryDetail
                        journalDetail.Credit += inventoryAccAmount;
                        //accountBalance
                        accBalance.CumulativeBalance = glAccInvenfifo.Balance;
                        accBalance.Credit += inventoryAccAmount;
                    }
                    else
                    {
                        glAccInvenfifo.Balance -= inventoryAccAmount;
                        journalEntryDetail.Add(new JournalEntryDetail
                        {
                            JEID = journalEntry.ID,
                            Type = Type.GLAcct,
                            ItemID = inventoryAccID,
                            Credit = inventoryAccAmount,
                        });
                        //
                        accountBalance.Add(new AccountBalance
                        {
                            JEID = journalEntry.ID,

                            PostingDate = crm.CreatedAt,
                            Origin = docType.ID,
                            OriginNo = crm.Number,
                            OffsetAccount = OffsetAcc,
                            Details = douTypeID.Name + "-" + glAccInvenfifo.Code,
                            CumulativeBalance = glAccInvenfifo.Balance,
                            Credit = inventoryAccAmount,
                            LocalSetRate = (decimal)crm.LocalSetRate,
                            GLAID = inventoryAccID,
                        });
                    }
                    _context.Update(glAccInvenfifo);
                }
            }
            else
            {
                //inventoryAccID
                var glAccInvenfifo = _context.GLAccounts.FirstOrDefault(w => w.ID == inventoryAccID) ?? new GLAccount();
                if (glAccInvenfifo.ID > 0)
                {
                    var journalDetail = journalEntryDetail.FirstOrDefault(w => w.ItemID == glAccInvenfifo.ID) ?? new JournalEntryDetail();
                    if (journalDetail.ItemID > 0)
                    {
                        var accBalance = accountBalance.FirstOrDefault(w => w.GLAID == inventoryAccID);
                        glAccInvenfifo.Balance += inventoryAccAmount;
                        //journalEntryDetail
                        journalDetail.Debit += inventoryAccAmount;
                        //accountBalance
                        accBalance.CumulativeBalance = glAccInvenfifo.Balance;
                        accBalance.Debit += inventoryAccAmount;
                    }
                    else
                    {
                        glAccInvenfifo.Balance += inventoryAccAmount;
                        journalEntryDetail.Add(new JournalEntryDetail
                        {
                            JEID = journalEntry.ID,
                            Type = Type.GLAcct,
                            ItemID = inventoryAccID,
                            Debit = inventoryAccAmount,
                        });
                        //
                        accountBalance.Add(new AccountBalance
                        {
                            JEID = journalEntry.ID,

                            PostingDate = crm.CreatedAt,
                            Origin = docType.ID,
                            OriginNo = crm.Number,
                            OffsetAccount = OffsetAcc,
                            Details = douTypeID.Name + "-" + glAccInvenfifo.Code,
                            CumulativeBalance = glAccInvenfifo.Balance,
                            Debit = inventoryAccAmount,
                            LocalSetRate = (decimal)crm.LocalSetRate,
                            GLAID = inventoryAccID,
                        });
                    }
                    _context.Update(glAccInvenfifo);
                }
            }
            _context.SaveChanges();
        }

        PriceLists GetPriceList(int priceListId)
        {
            return _context.PriceLists.Include(p => p.Currency)
                     .FirstOrDefault(p => p.ID == priceListId)
                     ?? new PriceLists
                     {
                         Currency = new Currency()
                     };
        }
        List<DisplayPayCurrencyModel> GetDisplayPayCurrency(int priceListId)
        {
            PriceLists priceList = GetPriceList(priceListId);
            var cur = _context.Currency.FirstOrDefault(i => i.ID == priceList.CurrencyID) ?? new Currency();
            var dcs = _context.DisplayCurrencies.Where(i => i.PriceListID == priceListId).ToList();
            var dc = dcs.FirstOrDefault(i => i.IsActive) ?? new DisplayCurrency();
            var altCurrencies = (from c in _context.Currency.Where(i => !i.Delete)
                                 let plBasedDc = dcs.FirstOrDefault(dc => dc.PriceListID == priceList.ID && c.ID == dc.AltCurrencyID) ?? new DisplayCurrency()
                                 select new DisplayPayCurrencyModel
                                 {
                                     LineID = $"{DateTime.Now.Ticks}{c.ID}",
                                     AltCurrency = c.Description,
                                     BaseCurrency = cur.Description,
                                     Amount = 0,
                                     AltAmount = 0,
                                     Rate = (decimal)plBasedDc.PLDisplayRate,
                                     AltRate = (decimal)plBasedDc.DisplayRate,
                                     BaseCurrencyID = cur.ID,
                                     AltCurrencyID = c.ID,
                                     AltSymbol = c.Symbol,
                                     BaseSymbol = cur.Symbol,
                                     IsLocalCurrency = dc.AltCurrencyID == 0 ? c.ID == cur.ID : c.ID == dc.AltCurrencyID,
                                     IsShowCurrency = plBasedDc.IsShowCurrency,
                                     IsActive = plBasedDc.IsActive,
                                     IsShowOtherCurrency = plBasedDc.IsShowOtherCurrency,
                                     DecimalPlaces = plBasedDc.DecimalPlaces,
                                 }).ToList();
            return altCurrencies;
        }
    }
}
