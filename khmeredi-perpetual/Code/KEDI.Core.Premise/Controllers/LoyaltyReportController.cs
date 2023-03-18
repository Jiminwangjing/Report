using CKBS.AppContext;
using CKBS.Models.Services.POS;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KEDI.Core.Premise.Authorization;
using CKBS.Models.Services.HumanResources;
using KEDI.Core.Premise.Models.ServicesClass.ClearPoint;

namespace KEDI.Core.Premise.Controllers
{
    [Privilege]
    public class LoyaltyReportController : Controller
    {
        private readonly DataContext _context;
        public LoyaltyReportController(DataContext context)
        {
            _context = context;
        }
        public IActionResult BuyOneGetOne()
        {
            ViewBag.BuyOneGetOne = "highlight";
            return View();
        }

        public IActionResult GetBuyOneGetOne(string DateFrom, string DateTo, int BranchID, int UserID)
        {

            List<Receipt> receiptsFilter = new List<Receipt>();
            if (DateFrom != null && DateTo != null && BranchID == 0 && UserID == 0)
            {
                receiptsFilter = _context.Receipt.Where(w => w.DateOut >= Convert.ToDateTime(DateFrom) && w.DateOut <= Convert.ToDateTime(DateTo)).ToList();
            }
            else if (DateFrom != null && DateTo != null && BranchID != 0 && UserID == 0)
            {
                receiptsFilter = _context.Receipt.Where(w => w.DateOut >= Convert.ToDateTime(DateFrom) && w.DateOut <= Convert.ToDateTime(DateTo) && w.BranchID == BranchID).ToList();
            }
            else if (DateFrom != null && DateTo != null && BranchID != 0 && UserID != 0)
            {
                receiptsFilter = _context.Receipt.Where(w => w.DateOut >= Convert.ToDateTime(DateFrom) && w.DateOut <= Convert.ToDateTime(DateTo) && w.BranchID == BranchID && w.UserOrderID == UserID).ToList();
            }
            else
            {
                return Ok(new List<Receipt>());
            }
            if (DateFrom == null)
            {
                DateFrom = "1900-01-01";

            }
            if (DateTo == null)
            {
                DateTo = "1900-01-01";
            }

            var Receipts = receiptsFilter;
            var Sale = from ch in _context.ReceiptDetail.Where(w => w.PromoType == PromotionType.BuyXGetX)
                       join r in Receipts on ch.ReceiptID equals r.ReceiptID
                       join curr_pl in _context.Currency on r.PLCurrencyID equals curr_pl.ID
                       join curr in _context.Currency on r.LocalCurrencyID equals curr.ID
                       join curr_sys in _context.Currency on r.SysCurrencyID equals curr_sys.ID
                       join user in _context.UserAccounts on r.UserOrderID equals user.ID
                       join emp in _context.Employees on user.EmployeeID equals emp.ID
                       join Uom in _context.UnitofMeasures on ch.UomID equals Uom.ID
                       join pa in _context.ReceiptDetail on ch.ParentLineID equals pa.LineID
                       join chiUom in _context.UnitofMeasures on ch.UomID equals chiUom.ID
                       select new
                       {
                           //Parant
                           ParLineID = pa.LineID,
                           ParItemCode = pa.Code,
                           ParKhmerName = pa.KhmerName,
                           ParItemID = pa.ItemID,
                           ParQty = pa.Qty,
                           ParUom = Uom.Name,
                           ParBuy = "Buy",
                           ParUnitPrice = pa.UnitPrice.ToString("0.000"),
                           ParDisItem = pa.DiscountValue.ToString("0.000"),
                           ParTotal = pa.Total,
                           ParLCTotal = pa.Total_Sys * r.LocalSetRate,
                           ParPLCurr = curr_pl.Description,
                           //Child
                           ChiLineID = ch.LineID,
                           ChiItemCode = ch.Code,
                           ChiKhmerName = ch.KhmerName,
                           ChiGet = "Get",
                           ChiItemID = ch.ItemID,
                           ChiQty = ch.Qty,
                           ChiUom = Uom.Name,
                           ChiUnitPrice = ch.UnitPrice.ToString("0.000"),
                           ChiDisItem = ch.DiscountValue,
                           ChiTotal = ch.Total,
                           SysCrr = curr_sys.Description,
                           LCrr = curr.Description,
                       };
            var allMaterials = (from all in Sale
                                group new { all } by new { all.ParLineID } into g
                                let data = g.FirstOrDefault()
                                let GTotal = Sale.ToList()
                                select new
                                {
                                    //Parant
                                    ParLineID = data.all.ParLineID,
                                    ParItemCode = data.all.ParItemCode,
                                    ParKhmerName = data.all.ParKhmerName,
                                    ParItemID = data.all.ParItemID,
                                    ParQty = g.Sum(s => s.all.ParQty),
                                    ParUom = data.all.ParUom,
                                    ParBuy = data.all.ParBuy,
                                    ParUnitPrice = data.all.LCrr + " " + data.all.ParUnitPrice,
                                    ParDisItem = data.all.ParDisItem,
                                    ParTotal = data.all.LCrr + " " + g.Sum(s => s.all.ParTotal).ToString("0.000"),
                                    //Child
                                    ChiLineID = data.all.ChiLineID,
                                    ChiItemCode = data.all.ChiItemCode,
                                    ChiKhmerName = data.all.ChiKhmerName,
                                    ChiItemID = data.all.ChiItemID,
                                    ChiQty = g.Sum(s => s.all.ChiQty),
                                    ChiUom = data.all.ChiUom,
                                    ChiGet = data.all.ChiGet,
                                    ChiUnitPrice = data.all.ChiUnitPrice,
                                    ChiDisItem = g.Sum(s => s.all.ChiDisItem).ToString("0.000"),
                                    ChiTotal = g.Sum(s => s.all.ChiTotal).ToString("0.000"),
                                    SGrandTotal = Sale.FirstOrDefault().SysCrr + " " + GTotal.Sum(s => s.ParTotal).ToString("0.000"),
                                    LGrandTotal = Sale.FirstOrDefault().LCrr + " " + GTotal.Sum(s => s.ParLCTotal).ToString("0.000"),
                                    //Summary
                                    DateFrom = Convert.ToDateTime(DateFrom).ToString("dd-MM-yyyy"),
                                    DateTo = Convert.ToDateTime(DateTo).ToString("dd-MM-yyyy"),
                                }).ToList();
            return Ok(allMaterials);
        }
        public IActionResult GetRedeem(string DateFrom, string DateTo, int BranchID, int UserID)
        {
            List<Receipt> receiptsFilter = new List<Receipt>();
            if (DateFrom != null && DateTo != null && BranchID == 0 && UserID == 0)
            {
                receiptsFilter = _context.Receipt.Where(w => w.DateOut >= Convert.ToDateTime(DateFrom) && w.DateOut <= Convert.ToDateTime(DateTo)).ToList();
            }
            else if (DateFrom != null && DateTo != null && BranchID != 0 && UserID == 0)
            {
                receiptsFilter = _context.Receipt.Where(w => w.DateOut >= Convert.ToDateTime(DateFrom) && w.DateOut <= Convert.ToDateTime(DateTo) && w.BranchID == BranchID).ToList();
            }
            else if (DateFrom != null && DateTo != null && BranchID != 0 && UserID != 0)
            {
                receiptsFilter = _context.Receipt.Where(w => w.DateOut >= Convert.ToDateTime(DateFrom) && w.DateOut <= Convert.ToDateTime(DateTo) && w.BranchID == BranchID && w.UserOrderID == UserID).ToList();
            }
            else
            {
                return Ok(new List<Receipt>());
            }
            if (DateFrom == null)
            {
                DateFrom = "1900-01-01";

            }
            if (DateTo == null)
            {
                DateTo = "1900-01-01";
            }

            var Receipts = receiptsFilter;
            var sale = (from r in _context.Receipt
                        join rd in _context.ReceiptDetail on r.ReceiptID equals rd.ReceiptID
                        join b in _context.BusinessPartners on r.CustomerID equals b.ID
                        join u in _context.UnitofMeasures on rd.UomID equals u.ID
                        join curr_pl in _context.Currency on r.PLCurrencyID equals curr_pl.ID
                        join curr in _context.Currency on r.LocalCurrencyID equals curr.ID
                        join curr_sys in _context.Currency on r.SysCurrencyID equals curr_sys.ID
                        select new
                        {
                            RecieptID = r.ReceiptID,
                            ParItemCode = rd.Code,
                            ParKhmerName = rd.KhmerName,
                            ParItemID = rd.ItemID,
                            ParQty = rd.Qty,
                            ParUom = u.Name,
                            OutStandingPoint = b.OutstandPoint,
                            CusName = b.Name,
                            CusCode = b.Code,
                            SysCrr = curr_sys.Description,
                            LCrr = curr.Description
                        }).ToList();
            var allsale = (from all in sale
                           group new { all } by new { all.CusCode } into g
                           let data = g.FirstOrDefault()
                           select new
                           {
                               ParQty = g.Sum(s => s.all.ParQty),
                               ParItemCode = data.all.ParItemCode,
                               ParKhmerName = data.all.ParKhmerName,
                               ParItemID = data.all.ParItemID,
                               ParUom = data.all.ParUom,
                               OutStandingPoint = data.all.OutStandingPoint,
                               CusName = data.all.CusName,
                               CusCode = data.all.CusCode,
                               SysCrr = data.all.SysCrr,
                               LCrr = data.all.LCrr
                           });
            return Ok(allsale);

        }


    }
}
