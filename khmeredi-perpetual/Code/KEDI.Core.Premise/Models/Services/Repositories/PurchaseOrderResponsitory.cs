using Microsoft.EntityFrameworkCore;
using CKBS.AppContext;
using CKBS.Models.ClassCopy;
using CKBS.Models.ReportClass;
using CKBS.Models.Services.Administrator.General;
using CKBS.Models.Services.Administrator.Inventory;
using CKBS.Models.Services.Banking;
using CKBS.Models.Services.Inventory.PriceList;
using CKBS.Models.ServicesClass;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CKBS.Models.Services.Inventory;
using KEDI.Core.Premise.Models.Services.Purchase;
using CKBS.Models.Services.Purchase;
using System;
using KEDI.Core.Helpers.Enumerations;
using static KEDI.Core.Premise.Controllers.PurchaseRequestController;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CKBS.Models.Services.Responsitory
{
    public interface IPurchaseOrder
    {
        IEnumerable<Company> GetCurrencyDefualt();
        IEnumerable<GroupDUoM> GetAllGroupDefind();
        IEnumerable<ServiceMapItemMasterDataPurchaseOrder> ServiceMapItemMasterDataPurchaseOrders(int ID);
        IEnumerable<ServiceMapItemMasterDataPurchaseOrder> GetItemFindeBarcode(int WarehouseID, string Barcode);
        Task GoodReceiptStockOrderAsync(int PurchaseID, int wareId);
        IEnumerable<ReportPurchaseOrder> ReportPurchaseOrders(int BranchID, int WarehouseID, string PostingDate, string DocumentDate, string DeliveryDate, string Check);
        IEnumerable<ServiceMapItemMasterDataPurchaseOrder> ServiceMapItemMasterDataPurchaseOrders_Detail(int WarehouseID, string number);
        IEnumerable<ReportPurchaseQuotation> GetAllPurchaeQuotation(int BranchID);
        IEnumerable<PurchaesOrder_from_Quotation> GetPurchaseOrder_From_PurchaseQuotation(int ID, string Invoice);
        IEnumerable<PurchaesOrder_from_Quotation> GetPurchaseOrder_From_PurchaseRequest(int ID, string Invoice);
        IEnumerable<ReportPurchaseRequset> GetAllPurchaseRequest();
        List<PurchaseReport> GetAllPurchaseRequests(int UserID, int BranchID, int WarehouseID, string PostingDate, string DocumentDate, bool check);
        //Task<List<ServiceMapItemMasterDataPurchaseOrder>> GetItemMasterDataAsync(int wareid, int sysCurId);
        void POCancel(int purchaseID);
        void PQCancel(int purchaseID);
        
    }
    public class PurchaseOrderResponsitory : IPurchaseOrder
    {
        private readonly DataContext _context;
        public PurchaseOrderResponsitory(DataContext context)
        {
            _context = context;
        }

  
        public IEnumerable<GroupDUoM> GetAllGroupDefind()
        {
            var uom = _context.UnitofMeasures.Where(u => u.Delete == false);
            var guom = _context.GroupUOMs.Where(g => g.Delete == false);
            var duom = _context.GroupDUoMs.Where(o => o.Delete == false);
            var list = (from du in duom
                        join g_uom in guom on du.GroupUoMID equals g_uom.ID
                        join buo in uom on du.BaseUOM equals buo.ID
                        join auo in uom on du.AltUOM equals auo.ID
                        where g_uom.Delete == false
                        select new GroupDUoM
                        {
                            ID = du.ID,
                            GroupUoMID = du.GroupUoMID,
                            UoMID = du.UoMID,
                            AltQty = du.AltQty,
                            BaseUOM = du.BaseUOM,
                            AltUOM = du.AltUOM,
                            BaseQty = du.BaseQty,
                            Factor = du.Factor,

                            UnitofMeasure = new UnitofMeasure
                            {
                                ID = auo.ID,
                                Name = buo.Name,
                                AltUomName = auo.Name
                            },

                        }
                );
            return list;
        }

        public IEnumerable<ReportPurchaseQuotation> GetAllPurchaeQuotation(int BranchID) => _context.ReportPurchaseQuotations.FromSql("sp_GetAllPurchaeQuotation @BranchID={0}",
            parameters: new[] {
                BranchID.ToString()
            });

        public IEnumerable<Company> GetCurrencyDefualt()
        {
            IEnumerable<Company> list = (
                from pd in _context.PriceLists.Where(x => x.Delete == false)
                join com in _context.Company.Where(x => x.Delete == false) on
                pd.ID equals com.PriceListID
                join cur in _context.Currency.Where(x => x.Delete == false)
                on pd.CurrencyID equals cur.ID
                select new Company
                {
                    PriceList = new PriceLists
                    {
                        Currency = new Currency
                        {
                            Description = cur.Description
                        }
                    }
                }

                );
            return list;
        }

        public IEnumerable<ServiceMapItemMasterDataPurchaseOrder> GetItemFindeBarcode(int WarehouseID, string Barcode) => _context.ServiceMapItemMasterDataPurchaseOrders.FromSql("sp_FindeBarcodePurchaseOrder @WarehouseID={0},@Barcode={1}",
            parameters: new[] {
                WarehouseID.ToString(),
                Barcode.ToString()
            });

        public async Task GoodReceiptStockOrderAsync(int purId, int wareId)
        {
            //_context.Database.ExecuteSqlCommand("sp_GoodReceiptStockPurchaseOrder @purchaseID={0}",
            //    parameters: new[] {
            //        PurchaseID.ToString()
            //    });

            var pods = await _context.PurchaseOrderDetails.Where(i => i.PurchaseOrderID == purId).ToListAsync();
            foreach (var pod in pods)
            {
                var item = await _context.ItemMasterDatas.FirstOrDefaultAsync(i => i.ID == pod.ItemID) ?? new ItemMasterData();
                var gUoM = await _context.GroupDUoMs.FirstOrDefaultAsync(i => i.GroupUoMID == item.GroupUomID && pod.UomID == i.AltUOM) ?? new GroupDUoM();
                var whs = await _context.WarehouseSummary.FirstOrDefaultAsync(i => i.ItemID == pod.ItemID && i.WarehouseID == wareId) ?? new WarehouseSummary();
                var itemAcc = await _context.ItemAccountings.FirstOrDefaultAsync(w => w.ItemID == pod.ItemID && w.WarehouseID == wareId) ?? new Financials.ItemAccounting();
                decimal factor = (decimal)gUoM.Factor;
                item.StockOnHand += pod.Qty * (double)factor;
                whs.Ordered += pod.Qty * (double)factor;
                itemAcc.Ordered += pod.Qty * (double)factor;
                await _context.SaveChangesAsync();
            }
        }

        public IEnumerable<ReportPurchaseOrder> ReportPurchaseOrders(int BranchID, int WarehouseID, string PostingDate, string DocumentDate, string DeliveryDate, string Check) => _context.ReportPurchaseOrders.FromSql("sp_ReportPurchaseCreditOrder @BranchID={0},@warehouseID={1},@PostingDate={2},@DeliveryDate={3},@DocumentDate={4},@Check={5}",
            parameters: new[] {
                BranchID.ToString(),
                WarehouseID.ToString(),
                PostingDate.ToString(),
                DocumentDate.ToString(),
                DeliveryDate.ToString(),
                Check.ToString()
            });


        public IEnumerable<ServiceMapItemMasterDataPurchaseOrder> ServiceMapItemMasterDataPurchaseOrders(int ID) => _context.ServiceMapItemMasterDataPurchaseOrders.FromSql("sp_GetListItemMasterDataPurchaseOrder @WarehouseID={0}",
            parameters: new[] {
                ID.ToString()
            });

        public IEnumerable<ServiceMapItemMasterDataPurchaseOrder> ServiceMapItemMasterDataPurchaseOrders_Detail(int WarehouseID, string Number) => _context.ServiceMapItemMasterDataPurchaseOrders.FromSql("sp_GetListItemMasterDataPurchaseOrder_Detail @WarehouseID={0},@Number={1}",
            parameters: new[] {

                WarehouseID.ToString(),
                Number.ToString()
            });

        public IEnumerable<PurchaesOrder_from_Quotation> GetPurchaseOrder_From_PurchaseQuotation(int ID, string Invoice) => _context.PurchaesOrder_From_Quotations.FromSql("sp_GetPurchaseOrder_form_PurchaseQuotation @PurchaseID={0},@Invoice={1}",
         parameters: new[] {
                ID.ToString(),
                Invoice.ToString()
         });

        public IEnumerable<ReportPurchaseRequset> GetAllPurchaseRequest() => _context.ReportPurchaseRequsets.FromSql("sp_GetAllPurchaeRequest");
        #region old getall purchaserequest
        public Dictionary<int, string> Typevat => EnumHelper.ToDictionary(typeof(PVatType));
        public List<PurchaseReport> GetAllPurchaseRequests(int UserID, int BranchID, int WarehouseID, string PostingDate, string DocumentDate, bool check)
        {
            List<PurchaseRequest> ServiceCalls = new();
            //filter WareHouse
            if (BranchID != 0 && WarehouseID != 0 && UserID == 0 && PostingDate == null && DocumentDate == null && check == false)
            {
                ServiceCalls = _context.PurchaseRequests.Where(w => w.BranchID == w.BranchID).ToList();
            }
            //filter Vendor
            else if (BranchID == 0 && WarehouseID == 0 && UserID != 0 && PostingDate == null && DocumentDate == null && check == false)
            {
                ServiceCalls = _context.PurchaseRequests.Where(w => w.UserID == UserID).ToList();
            }
            //filter WareHouse and VendorName
            else if (BranchID != 0 && WarehouseID != 0 && UserID != 0 && PostingDate == null && DocumentDate == null && check == false)
            {
                ServiceCalls = _context.PurchaseRequests.Where(w => w.BranchID == BranchID && w.UserID == UserID).ToList();
            }
            //filter all item
            else if (BranchID != 0 && WarehouseID == 0 && UserID == 0 && PostingDate == null && DocumentDate == null)
            {
                ServiceCalls = _context.PurchaseRequests.Where(w => w.UserID == BranchID).ToList();
            }
            //filter warehouse, vendor, datefrom ,dateto
            else if (BranchID != 0 && WarehouseID != 0 & UserID != 0 && PostingDate != null && DocumentDate != null && check == false)
            {
                ServiceCalls = _context.PurchaseRequests.Where(w => w.UserID == UserID && w.BranchID == BranchID && w.PostingDate >= Convert.ToDateTime(PostingDate) && w.DocumentDate <= Convert.ToDateTime(DocumentDate)).ToList();
            }
            //filter vendor and Datefrom and Dateto
            else if (BranchID == 0 && WarehouseID == 0 && UserID != 0 && PostingDate != null && DocumentDate != null && check == false)
            {
                ServiceCalls = _context.PurchaseRequests.Where(w => w.UserID == UserID && w.PostingDate >= Convert.ToDateTime(PostingDate) && w.DocumentDate <= Convert.ToDateTime(DocumentDate)).ToList();
            }
            //filter warehouse and Datefrom and DateTo
            else if (BranchID != 0 && WarehouseID != 0 && UserID == 0 && PostingDate != null && DocumentDate != null && check == false)
            {
                ServiceCalls = _context.PurchaseRequests.Where(w => w.PostingDate >= Convert.ToDateTime(PostingDate) && w.DocumentDate <= Convert.ToDateTime(DocumentDate)).ToList();
            }
            //filter Datefrom and DateTo
            else if (WarehouseID == 0 && UserID == 0 && PostingDate != null && DocumentDate != null)
            {
                ServiceCalls = _context.PurchaseRequests.Where(w => w.PostingDate >= Convert.ToDateTime(PostingDate) && w.PostingDate <= Convert.ToDateTime(DocumentDate)).ToList();
            }
            else
            {
                ServiceCalls = _context.PurchaseRequests.ToList();
            }

            var list = (from s in ServiceCalls
                        join req in _context.UserAccounts.Include(i => i.Employee) on s.RequesterID equals req.ID
                        join item in _context.UserAccounts on s.UserID equals item.ID
                        select new PurchaseReport
                        {
                            ID = s.ID,
                            Invoice = s.Number,
                            Requester = req.Username,
                            VendorCode = req.Employee.Code,
                            VendorName = req.Employee.Name,
                            UserName = item.Username,
                            Balance = s.BalanceDue,
                            ExchangeRate = s.PurRate,
                            Cancele = "<i class= 'fa fa-ban'style='color:red;' ></i>",
                            Status = s.Status,
                            VatType = Typevat.Select(s => new SelectListItem
                            {
                                Value = s.Key.ToString(),
                                Text = s.Value
                            }).ToList(),
                        }).ToList();
            return list;
        }

        #endregion

        public IEnumerable<PurchaesOrder_from_Quotation> GetPurchaseOrder_From_PurchaseRequest(int ID, string Invoice) => _context.PurchaesOrder_From_Quotations.FromSql("sp_GetPurchaseOrder_from_PurchaseRequest @PurchaseID={0},@Invoice={1}",
            parameters: new[] {
                ID.ToString(),
                Invoice.ToString()
            });

        public void POCancel(int purchaseID)
        {
            if (purchaseID != 0)
            {
                var purchase = _context.PurchaseOrders.Include(w => w.PurchaseOrderDetails).FirstOrDefault(w => w.PurchaseOrderID == purchaseID);
                if (purchase != null || purchase.PurchaseOrderDetails != null)
                {
                    purchase.Status = "close";
                    _context.PurchaseOrders.Update(purchase);
                    _context.SaveChanges();
                    foreach (var item in purchase.PurchaseOrderDetails.ToList())
                    {
                        var item_master = _context.ItemMasterDatas.FirstOrDefault(w => w.ID == item.ItemID);
                        var gduom = _context.GroupDUoMs.FirstOrDefault(w => w.GroupUoMID == item_master.GroupUomID && w.AltUOM == item.UomID);

                        var warehouse_sum = _context.WarehouseSummary.FirstOrDefault(w => w.WarehouseID == purchase.WarehouseID
                                                                                        && w.ItemID == item.ItemID
                                                                                        );
                        var itemAcc = _context.ItemAccountings.FirstOrDefault(w => w.WarehouseID == purchase.WarehouseID
                                                                                        && w.ItemID == item.ItemID
                                                                                        );
                        if (warehouse_sum != null)
                        {
                            warehouse_sum.Ordered -= (item.OpenQty * gduom.Factor);
                            itemAcc.Ordered = warehouse_sum.Ordered;
                            _context.ItemAccountings.Update(itemAcc);
                            _context.WarehouseSummary.Update(warehouse_sum);
                            _context.SaveChanges();
                        }
                        //Update in item maser
                        item_master.StockOnHand -= (item.OpenQty * gduom.Factor);
                        _context.ItemMasterDatas.Update(item_master);
                        _context.SaveChanges();
                    }
                }
            }
        }
        public void PQCancel(int purchaseID)
        {
            if (purchaseID != 0)
            {
                var purchase = _context.PurchaseQuotations.Include(w => w.PurchaseQuotationDetails).FirstOrDefault(w => w.ID == purchaseID) ?? new Purchase.PurchaseQuotation();
                if (purchase != null || purchase.PurchaseQuotationDetails != null)
                {
                    purchase.Status = "close";
                    _context.PurchaseQuotations.Update(purchase);
                    _context.SaveChanges();
                    foreach (var item in purchase.PurchaseQuotationDetails.ToList())
                    {
                        var item_master = _context.ItemMasterDatas.FirstOrDefault(w => w.ID == item.ItemID) ?? new ItemMasterData();
                        var gduom = _context.GroupDUoMs.FirstOrDefault(w => w.GroupUoMID == item_master.GroupUomID && w.AltUOM == item.UomID) ?? new GroupDUoM();

                        var warehouse_sum = _context.WarehouseSummary.FirstOrDefault(w => w.WarehouseID == purchase.WarehouseID && w.ItemID == item.ItemID) ?? new WarehouseSummary();
                        var itemAcc = _context.ItemAccountings.FirstOrDefault(w => w.WarehouseID == purchase.WarehouseID && w.ItemID == item.ItemID) ?? new Financials.ItemAccounting();
                        if (warehouse_sum != null)
                        {
                            warehouse_sum.Ordered -= (item.OpenQty * gduom.Factor);
                            itemAcc.Ordered = warehouse_sum.Ordered;
                            _context.ItemAccountings.Update(itemAcc);
                            _context.WarehouseSummary.Update(warehouse_sum);
                            _context.SaveChanges();
                        }
                        //Update in item maser
                        item_master.StockOnHand -= (item.OpenQty * gduom.Factor);
                        _context.ItemMasterDatas.Update(item_master);
                        _context.SaveChanges();
                    }
                }
            }
        }
    }
}
