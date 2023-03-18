using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CKBS.AppContext;
using CKBS.Models.Services.Account;
using CKBS.Models.Services.Banking;
using CKBS.Models.Services.HumanResources;
using CKBS.Models.Services.Purchase;
using CKBS.Models.Services.Responsitory;
using KEDI.Core.Premise.Authorization;
// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CKBS.Controllers
{
    [Privilege]
    public class GoodsReceiptsController : Controller
    {
        private readonly DataContext _context;
        private readonly IGoodsReceipts _purchaseAP;
        private readonly IGUOM _gUOM;
        public GoodsReceiptsController(DataContext context, IGoodsReceipts purchaseAP, IGUOM gUOM)
        {
            _context = context;
            _purchaseAP = purchaseAP;
            _gUOM = gUOM;
        }

        [Privilege("A024")]
        public IActionResult PurchaseAP()
        {
            ViewBag.style = "fa fa-shopping-cart";
            ViewBag.Main = "Inventory";
            ViewBag.Page = "Transaction";
            ViewBag.SubPage = "Goods Receipt";
            ViewBag.PurchaseMenu = "show";
            ViewBag.PurchaseAP = "highlight";
            return View();
        }

        [HttpGet]
        public IActionResult GetInvoicenomber()
        {
            var count = _context.Purchase_APs.Count() + 1;
            var list = "SI-" + count.ToString().PadLeft(7, '0');
            return Ok(list);
        }

        [HttpGet]
        public IActionResult GetBusinessPartners()
        {
            var list = _context.BusinessPartners.Where(x => x.Delete == false && x.Type == "Vendor").ToList();
            return Ok(list);
        }

        [HttpGet]
        public IActionResult GetWarehouses(int ID)
        {
            var list = _context.Warehouses.Where(x => x.Delete == false && x.BranchID == ID).ToList();
            return Ok(list);
        }

        [HttpGet]
        public IActionResult GetItemByWarehouse_AP(int ID)
        {
            var list = _purchaseAP.ServiceMapItemMasterDataPurchasAPs(ID).ToList();
            return Ok(list);
        }

        [HttpGet]
        public IActionResult GetItemByWarehouse_AP_Detail(int warehouseid, string invoice)
        {
            var list = _purchaseAP.ServiceMapItemMasterDataPurchasAPs_Detail(warehouseid, invoice).ToList();
            return Ok(list);
        }

        [HttpGet]
        public IActionResult Getcurrency()
        {
            var list = _context.Currency.Where(x => x.Delete == false).ToList();
            return Ok(list);
        }

        [HttpGet]
        public IActionResult GetCurrencyDefualt()
        {
            var list = _purchaseAP.GetCurrencies();
            return Ok(list);
        }

        [HttpGet]
        public IActionResult GetGroupDefind()
        {
            var list = _gUOM.GetAllgroupDUoMs().ToList();
            return Ok(list);
        }

        [HttpGet]
        public IActionResult GetFilterLocaCurrency(int CurrencyID)
        {
            var list = _purchaseAP.GetExchangeRates(CurrencyID).ToList();
            return Ok(list);
        }

        [HttpPost]
        public IActionResult SavePurchaseAP(Purchase_AP purchase, string Type)
        {
            var remark = purchase.Remark;
            string InvoiceOrder = "";
            double openqty = 0;
            if (Type == "Add")
            {
                _context.Purchase_APs.Add(purchase);
                _context.SaveChanges();

                foreach (var check in purchase.PurchaseAPDetails.ToList())
                {
                    if (check.Qty <= 0)
                    {
                        _context.Remove(check);
                        _context.SaveChanges();
                    }
                }
                _purchaseAP.GoodReceiptStock(purchase.PurchaseAPID, "AP");
            }
            else if (Type == "PO")
            {
                InvoiceOrder = remark.Split("/")[1];
                List<Purchase_APDetail> Comfirn = new();
                List<Purchase_APDetail> List = new();
                foreach (var items in purchase.PurchaseAPDetails.ToList())
                {
                    if (items.Qty > 0)
                    {
                        Comfirn.Add(items);
                    }
                }
                if (List.Count > 0)
                {
                    return Ok(List);
                }
                else
                {
                    if (purchase.PurchaseAPID == 0)
                    {
                        _context.Purchase_APs.Add(purchase);
                        _context.SaveChanges();
                        _purchaseAP.GoodReceiptStock(purchase.PurchaseAPID, "PO");
                        var purchaseOrder = _context.PurchaseOrders.FirstOrDefault(x => x.InvoiceNo == InvoiceOrder);
                        var subAppliedAmount = _context.PurchaseOrders.Where(x => x.InvoiceNo == InvoiceOrder);
                        purchaseOrder.AppliedAmount = subAppliedAmount.Sum(x => x.AppliedAmount) + purchase.BalanceDue;
                        _context.PurchaseOrders.Update(purchaseOrder);
                        _context.SaveChanges();

                        int OrderID = purchaseOrder.PurchaseOrderID;
                        var detail = _context.PurchaseOrderDetails.Where(x => x.PurchaseOrderID == OrderID && x.Delete == false);
                        if (Comfirn.Count > 0)
                        {
                            foreach (var item in detail.ToList())
                            {
                                var items = Comfirn.FirstOrDefault(i => item.PurchaseOrderDetailID == i.OrderID);
                                if (items != null)
                                {
                                    openqty = item.OpenQty - items.Qty;
                                    var purchaseDetail = _context.PurchaseOrderDetails.FirstOrDefault(x => x.PurchaseOrderDetailID == item.PurchaseOrderDetailID);
                                    purchaseDetail.OpenQty = openqty;
                                    _context.Update(purchaseDetail);
                                    _context.SaveChanges();
                                    if (openqty == 0)
                                    {
                                        purchaseDetail.Delete = true;
                                        _context.Update(purchaseDetail);
                                        _context.SaveChanges();
                                    }
                                }
                            }
                        }
                        if (CheckStatus(detail))
                        {
                            var purchaseAP = _context.PurchaseOrders.FirstOrDefault(x => x.InvoiceNo == InvoiceOrder);
                            purchaseAP.Status = "close";
                            _context.Update(purchaseAP);
                            _context.SaveChanges();
                        }
                    }
                }

            }
            else if (Type == "GRPO")
            {
                InvoiceOrder = remark.Split("/")[1];
                List<Purchase_APDetail> List = new();
                foreach (var items in purchase.PurchaseAPDetails.ToList())
                {
                    var checkOrdered = _context.WarehouseDetails.FirstOrDefault(w => w.ItemID == items.ItemID && w.Cost == items.PurchasPrice * purchase.PurRate && w.UomID == items.UomID && w.ExpireDate == items.ExpireDate && w.WarehouseID == purchase.WarehouseID);

                    if (checkOrdered == null)
                    {
                        List.Add(items);
                    }
                }
                if (List.Count > 0)
                {
                    return Ok(List);
                }
                else
                {
                    if (purchase.PurchaseAPID == 0)
                    {
                        _context.Purchase_APs.Add(purchase);
                        _context.SaveChanges();
                        _purchaseAP.GoodReceiptStock(purchase.PurchaseAPID, "PD");
                        var goddpo = _context.GoodsReciptPOs.FirstOrDefault(x => x.InvoiceNo == InvoiceOrder);
                        var subAppliedAmount = _context.GoodsReciptPOs.Where(x => x.InvoiceNo == InvoiceOrder);
                        goddpo.AppliedAmount = subAppliedAmount.Sum(x => x.AppliedAmount) + purchase.BalanceDue;
                        goddpo.Status = "close";
                        _context.GoodsReciptPOs.Update(goddpo);
                        _context.SaveChanges();

                    }
                }
            }
            return Ok();
        }

        public bool CheckStatus(IEnumerable<PurchaseOrderDetail> invoices)
        {
            bool result = true;
            foreach (var inv in invoices)
            {
                if (inv.Delete == false)
                {
                    result = false;
                    break;
                }
            }
            return result;
        }

        [HttpPost]
        public IActionResult FindPurchaseAP(string number)
        {
            var list = _context.Purchase_APs.Include(w => w.PurchaseAPDetails).FirstOrDefault(x => x.InvoiceNo == number);
            if (list != null)
            {
                return Ok(list);
            }
            else
            {
                return Ok();
            }

        }

        [HttpGet]
        public IActionResult GetBusinessPartner_AP()
        {
            var list = _context.BusinessPartners.Where(x => x.Delete == false && x.Type == "Vendor").ToList();
            return Ok(list);
        }

        [HttpGet]
        public IActionResult GetWarehouse_AP(int ID)
        {
            var list = _context.Warehouses.Where(x => x.BranchID == ID && x.Delete == false).ToList();
            return Ok(list);
        }

        [HttpPost]
        public IActionResult GetUserAccout_AP(int UserID)
        {
            var list = from user in _context.UserAccounts.Where(x => x.Delete == false)
                       join
                       emp in _context.Employees.Where(x => x.Delete == false) on user.EmployeeID equals emp.ID
                       where user.ID == UserID
                       select new UserAccount
                       {
                           Employee = new Employee
                           {
                               Name = emp.Name
                           }
                       };
            return Ok(list);
        }

        [Privilege("A024")]
        public IActionResult PurchaseAPStory()
        {
            ViewBag.style = "fa fa-shopping-cart";
            ViewBag.Main = "Purchase";
            ViewBag.Page = "Purchase A/P";
            ViewBag.Subpage = "Purchase A/P Story";
            ViewBag.Menu = "show";
            return View();
        }

        [HttpGet]
        public IActionResult GetPurchaseAPReport(int BarbchID, int WarehouseID, string PostingDate, string DocumentDate, string DeliveryDate, string Check)
        {
            var list = _purchaseAP.GetReportPurchaseAPs(BarbchID, WarehouseID, PostingDate, DocumentDate, DeliveryDate, Check).ToList();
            return Ok(list);
        }

        [HttpGet]
        public IActionResult GetPurchaseAPByWarehouse(int BranchID, int WarehouseID, string PostingDate, string DocumentDate, string DeliveryDate, string Check)
        {
            var list = _purchaseAP.GetReportPurchaseAPs(BranchID, WarehouseID, PostingDate, DocumentDate, DeliveryDate, Check).ToList();
            return Ok(list);
        }

        [HttpGet]
        public IActionResult GetPurchaseAPByPostingDate(int BranchID, int WarehouseID, string PostingDate, string DocumentDate, string DeliveryDate, string Check)
        {
            var list = _purchaseAP.GetReportPurchaseAPs(BranchID, WarehouseID, PostingDate, DocumentDate, DeliveryDate, Check).ToList();
            return Ok(list);
        }

        [HttpGet]
        public IActionResult GetPurchaseAPByDocumentDate(int BranchID, int WarehouseID, string PostingDate, string DocumentDate, string DeliveryDate, string Check)
        {
            var list = _purchaseAP.GetReportPurchaseAPs(BranchID, WarehouseID, PostingDate, DocumentDate, DeliveryDate, Check).ToList();
            return Ok(list);
        }

        [HttpGet]
        public IActionResult GetPurchaseAPByDeliveryDatedDate(int BranchID, int WarehouseID, string PostingDate, string DocumentDate, string DeliveryDate, string Check)
        {
            var list = _purchaseAP.GetReportPurchaseAPs(BranchID, WarehouseID, PostingDate, DocumentDate, DeliveryDate, Check).ToList();
            return Ok(list);
        }

        [HttpGet]
        public IActionResult GetPurchaseAPAllItem(int BranchID, int WarehouseID, string PostingDate, string DocumentDate, string DeliveryDate, string Check)
        {
            var list = _purchaseAP.GetReportPurchaseAPs(BranchID, WarehouseID, PostingDate, DocumentDate, DeliveryDate, Check);
            return Ok(list);
        }

        [HttpGet]
        public IActionResult GetPurchaseorder(int BranchID)
        {
            var list = _purchaseAP.GetAllPruchaseOrder(BranchID).ToList();
            return Ok(list);
        }

        [HttpGet]
        public IActionResult FindPurhcaseOrder(int ID, string Invoice)
        {
            var list = _purchaseAP.GetPurchaseAP_From_PurchaseOrder(ID, Invoice).ToList();
            return Ok(list);
        }

        [HttpGet]
        public IActionResult FindBarcode(int WarehouseID, string Barcode)
        {
            try
            {
                var list = _purchaseAP.FindItemBarcode(WarehouseID, Barcode).ToList();
                return Ok(list);
            }
            catch (Exception)
            {
                return Ok();
            }
        }

        [HttpGet]
        public IActionResult GetGoodReceiptPO(int BranchID)
        {
            var list = _purchaseAP.GetALlGoodReceiptPO(BranchID).ToList();
            return Ok(list);
        }

        [HttpGet]
        public IActionResult FindGoodReceiptPO(int ID, string Invoice)
        {
            var list = _purchaseAP.GetPurchaseAP_From_PurchaseGoodReceipt(ID, Invoice).ToList();
            return Ok(list);
        }
    }
}
