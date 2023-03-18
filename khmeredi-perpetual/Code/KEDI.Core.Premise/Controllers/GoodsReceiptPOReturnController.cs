using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CKBS.AppContext;
using CKBS.Models.Services.Account;
using CKBS.Models.Services.HumanResources;
using CKBS.Models.Services.Purchase;
using CKBS.Models.Services.Responsitory;
using KEDI.Core.Premise.Authorization;
// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CKBS.Controllers
{
    [Privilege]
    public class GoodsReceiptPOReturnController : Controller
    {
        private readonly DataContext _context;
        private readonly IGoodsReceiptPoReturn _poReturn;
        public GoodsReceiptPOReturnController(DataContext context,IGoodsReceiptPoReturn poReturn)
        {
            _context = context;
            _poReturn = poReturn;
        }
        
        public IActionResult GoodsReceiptPoReturn()
        {
            ViewBag.style = "fa fa-shopping-cart";
            ViewBag.Main = "Purchase";
            ViewBag.Page = "Goods Return";
            ViewBag.Menu = "show";
            return View();
        }

        public IActionResult GoodsReturnHistory()
        {
            ViewBag.style = "fa fa-shopping-cart";
            ViewBag.Main = "Purchase";
            ViewBag.Page = "Goods Return";
            ViewBag.Subpag = "History";
            ViewBag.Menu = "show";
            return View();
        }

        [HttpGet]
        public IActionResult GetInvoicenomber()
        {
            var count = _context.GoodsReceiptPoReturns.Count() + 1;
            var list = "PR-" + count.ToString().PadLeft(7, '0');
            return Ok(list);
        }

        [HttpGet]
        public IActionResult GetBusinessPartners()
        {
            var list = _context.BusinessPartners.Where(x => x.Delete == false && x.Type == "Vendor").ToList();
            return Ok(list);
        }

        [HttpGet]
        public IActionResult GetCurrencyDefualt()
        {
            var list = _poReturn.GetCurrencies().ToList();
            return Ok(list);
        }

        [HttpGet]
        public IActionResult GetWarehouses(int ID)
        {
            var list = _context.Warehouses.Where(x => x.Delete == false && x.BranchID == ID).ToList();
            return Ok(list);
        }

        [HttpGet]
        public IActionResult GetGroupDefind()
        {
            var list = _poReturn.GetAllgroupDUoMs();
            return Ok(list);
        }

        [HttpGet]
        public IActionResult Getcurrency()
        {
            var list = _context.Currency.Where(x => x.Delete == false).ToList();
            return Ok(list);
        }

        [HttpGet]
        public IActionResult GetItemByWarehouse_Memo(int ID)
        {
            var list = _poReturn.ServiceMapItemMasterDataGoodRetrun(ID).ToList();
            return Ok(list);
        }

        [HttpPost]
        public IActionResult SaveGoodReturn(GoodsReceiptPoReturn purchase,string Type) {
            var remark = purchase.Remark;
            string InvoiceAp = "";
            double openqty = 0;
            if (Type == "Add")
            {
                List<GoodsReceiptPoReturnDetail> list = new List<GoodsReceiptPoReturnDetail>();
                List<GoodsReceiptPoReturnDetail> Comfirn = new List<GoodsReceiptPoReturnDetail>();
                foreach (var items in purchase.GoodsReceiptPoReturnDetail.ToList())
                {
                    var checkstock = _context.WarehouseDetails.FirstOrDefault(w => w.ItemID == items.ItemID && w.Cost == items.PurchasPrice * purchase.ExchangeRate && w.UomID == items.UomID && w.ExpireDate == items.ExpireDate && w.WarehouseID == purchase.WarehouseID);
                    if (checkstock == null)
                    {
                        items.Check = "Not​​​​ Transaction";
                        list.Add(items);
                    }
                    else
                    {
                        if (items.Qty > checkstock.InStock)
                        {
                            items.Check = "Excessive Stock";
                            list.Add(items);
                        }
                        else
                        {
                            if (items.Qty > 0)
                            {
                                Comfirn.Add(items);
                            }
                        }
                    }
                }
                if (list.Count() > 0)
                {
                    return Ok(list);
                }
                else
                {
                  
                    if (purchase.GoodsReturnID == 0)
                    {
                        _context.GoodsReceiptPoReturns.Add(purchase);
                        _context.SaveChanges();
                        foreach (var check in purchase.GoodsReceiptPoReturnDetail.ToList())
                        {
                            if (check.Qty <= 0)
                            {
                                _context.Remove(check);
                                _context.SaveChanges();
                            }
                        }
                        _poReturn.GoodReturnPO(purchase.GoodsReturnID, InvoiceAp);
                    }
                  
                }
            } else if (Type == "GRPO")
            {
                InvoiceAp = (remark.Split("/"))[1];
                List<GoodsReceiptPoReturnDetail> list = new List<GoodsReceiptPoReturnDetail>();
                List<GoodsReceiptPoReturnDetail> Comfirn = new List<GoodsReceiptPoReturnDetail>();
                // check payment
                foreach (var items in purchase.GoodsReceiptPoReturnDetail.ToList())
                {
                    var checkstock = _context.WarehouseDetails.FirstOrDefault(w => w.ItemID == items.ItemID && w.Cost == items.PurchasPrice * purchase.ExchangeRate && w.UomID == items.UomID && w.ExpireDate == items.ExpireDate && w.WarehouseID == purchase.WarehouseID);
                    if (checkstock == null)
                    {
                        items.Check = "Not​​​​ Transaction";
                        list.Add(items);
                    }
                    else
                    {
                        if (items.Qty > checkstock.InStock)
                        {
                            items.Check = "Over Stock";
                            list.Add(items);
                        }
                        else
                        {
                            if (items.Qty > 0)
                            {
                                Comfirn.Add(items);
                            }
                           
                        }
                    }
                }
                if (list.Count() > 0)
                {
                    return Ok(list);
                }
                else
                {
                    if (purchase.GoodsReturnID == 0)
                    {
                        _context.GoodsReceiptPoReturns.Add(purchase);
                        _context.SaveChanges();
                        foreach (var check in purchase.GoodsReceiptPoReturnDetail.ToList())
                        {
                            if (check.Qty <= 0)
                            {
                                _context.Remove(check);
                                _context.SaveChanges();
                            }
                        }
                        _poReturn.GoodReturnPO(purchase.GoodsReturnID, InvoiceAp);

                        var purchaseAp = _context.GoodsReciptPOs.FirstOrDefault(x => x.InvoiceNo == InvoiceAp);
                        var subApplied_Amount = _context.GoodsReciptPOs.Where(x => x.InvoiceNo == InvoiceAp);

                        purchaseAp.AppliedAmount = subApplied_Amount.Sum(x => x.AppliedAmount) + purchase.Balance_Due;
                        _context.GoodsReciptPOs.Update(purchaseAp);
                        _context.SaveChanges();
                        int ApID = purchaseAp.ID;
                        var detail = _context.GoodReciptPODetails.Where(x => x.GoodsReciptPOID == ApID && x.Delete == false);
                        if (Comfirn.Count() > 0)
                        {
                            foreach (var item in detail.ToList())
                            {
                                foreach (var items in Comfirn.ToList())
                                {
                                    if (item.ID == items.APID)
                                    {
                                        openqty = item.OpenQty - items.Qty;

                                        var purchaseDetail = _context.GoodReciptPODetails.FirstOrDefault(x => x.ID == item.ID);
                                        purchaseDetail.OpenQty = openqty;
                                        _context.Update(purchaseDetail);
                                        _context.SaveChanges();

                                        if (openqty == 0)
                                        {
                                            purchaseDetail = _context.GoodReciptPODetails.FirstOrDefault(x => x.ID == item.ID);
                                            purchaseDetail.Delete = true;
                                            _context.Update(purchaseDetail);
                                            _context.SaveChanges();
                                        }

                                    }
                                }
                            }
                        }
                        if (checkStatusGoodReceipt(detail))
                        {
                            var purchaseAP = _context.GoodsReciptPOs.FirstOrDefault(x => x.InvoiceNo == InvoiceAp);
                            purchaseAP.Status = "close";
                            _context.Update(purchaseAP);
                            _context.SaveChanges();
                        }
                    }
                }
            }
            return Ok();
        }

        public bool checkStatusGoodReceipt(IEnumerable<GoodReciptPODetail> invoices)
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
        public IActionResult FindGoodsReturn(string number)
        {
            var list = _context.GoodsReceiptPoReturns.Include(x => x.GoodsReceiptPoReturnDetail).FirstOrDefault(x => x.InvoiceNo == number);
            return Ok(list);
        }

        [HttpGet]
        public IActionResult GetItemByWarehouse_GoodReturn_Detail(int warehouseid, string invoice)
        {
            var list = _poReturn.ServiceMapItemMasterDataGoodReturnDetail(warehouseid, invoice).ToList();
            return Ok(list);
        }

        [HttpGet]
        public IActionResult GetBusinessPartner_Memo()
        {
            var list = _context.BusinessPartners.Where(x => x.Delete == false && x.Type == "Vendor").ToList();
            return Ok(list);
        }

        [HttpPost]
        public IActionResult GetUserAccout_Memo(int UserID)
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

        [HttpGet]
        public IActionResult GetWarehouse_Memo(int ID)
        {
            var list = _context.Warehouses.Where(x => x.BranchID == ID && x.Delete == false).ToList();
            return Ok(list);
        }

        [HttpGet]
        public IActionResult GetGoodReturnReport(int BranchID,int WarehouseID,string PostingDate,string DocumenteDate,string DueDate ,string Check)
        {
            var list = _poReturn.ReportGoodReturn(BranchID, WarehouseID, PostingDate, DocumenteDate, DueDate, Check);
            return Ok(list);
        }

        [HttpGet]
        public IActionResult GetWarehouses_Memo(int ID)
        {
            var list = _context.Warehouses.Where(x => x.Delete == false && x.BranchID == ID).ToList();
            return Ok(list);
        }

        [HttpGet]
        public IActionResult GetGoodReturnByWarehouse(int BranchID, int WarehouseID, string PostingDate, string DocumenteDate, string DueDate, string Check)
        {
            var list = _poReturn.ReportGoodReturn(BranchID, WarehouseID, PostingDate, DocumenteDate, DueDate, Check);
            return Ok(list);
        }

        [HttpGet]
        public IActionResult GetGoodReturnByPostingDate(int BranchID, int WarehouseID, string PostingDate, string DocumenteDate, string DueDate, string Check)
        {
            var list = _poReturn.ReportGoodReturn(BranchID, WarehouseID, PostingDate, DocumenteDate, DueDate, Check);
            return Ok(list);
        }

        [HttpGet]
        public IActionResult GetGoodReturnByDocumentDate(int BranchID, int WarehouseID, string PostingDate, string DocumenteDate, string DueDate, string Check)
        {
            var list = _poReturn.ReportGoodReturn(BranchID, WarehouseID, PostingDate, DocumenteDate, DueDate, Check);
            return Ok(list);
        }

        [HttpGet]
        public IActionResult GetPGoodReturnByDeliveryDatedDate(int BranchID, int WarehouseID, string PostingDate, string DocumenteDate, string DueDate, string Check)
        {
            var list = _poReturn.ReportGoodReturn(BranchID, WarehouseID, PostingDate, DocumenteDate, DueDate, Check);
            return Ok(list);
        }

        [HttpGet]
        public IActionResult GetGoodReturnAllItem(int BranchID, int WarehouseID, string PostingDate, string DocumenteDate, string DueDate, string Check)
        {
            var list = _poReturn.ReportGoodReturn(BranchID, WarehouseID, PostingDate, DocumenteDate, DueDate, Check);
            return Ok(list);
        }
    }
}
