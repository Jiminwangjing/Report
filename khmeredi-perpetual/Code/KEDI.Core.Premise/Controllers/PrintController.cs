using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using CKBS.AppContext;
using CKBS.Models.Services.Purchase.Print;
using CKBS.Models.Services.Responsitory;
using Rotativa.AspNetCore;
using CKBS.Models.Services.Administrator.General;
using CKBS.Models.Services.Inventory.Property;
using CKBS.Models.PrintList;
using KEDI.Core.Premise.Authorization;
using System.Security.Claims;
using KEDI.Core.Premise.Models.Sale.Print;
using CKBS.Models.ServicesClass.KAMSService;
using CKBS.Models.Services.Banking;
using KEDI.Core.Premise.Models.Sale;
using CKBS.Models.Services.HumanResources;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CKBS.Controllers
{
    [Privilege]
    public class PrintController : Controller
    {
        // GET: /<controller>/
        private readonly DataContext _context;
        private readonly IReport _report;
        public PrintController(DataContext context, IReport report)
        {
            _context = context;
            _report = report;
        }
        //Summary Sale
        public IActionResult PrintSummarySale(string DateFrom, string DateTo, int BranchID, int UserID)
        {
            if (DateFrom == null)
            {
                DateFrom = "1900-01-01";

            }
            if (DateTo == null)
            {
                DateTo = "1900-01-01";
            }
            var list = _report.GetSummarySales(DateFrom, DateTo, BranchID, UserID).ToList();
            if (list.Count > 0)
            {
                list.First().DateFrom = DateFrom;
                list.First().DateTo = DateTo;
            }
            return new ViewAsPdf(list);
        }

        //Top Sale Quatity
        public IActionResult PrintTopSaleQuantity(string DateFrom, string DateTo, int BranchID)
        {
            if (DateFrom == null)
            {
                DateFrom = "1900-01-01";

            }
            if (DateTo == null)
            {
                DateTo = "1900-01-01";
            }
            var list = _report.GetTopSaleQuantities(DateFrom, DateTo, BranchID).ToList();
            if (list.Count > 0)
            {
                list.First().DateFrom = DateFrom;
                list.First().DateTo = DateTo;
            }
            return new ViewAsPdf(list);
        }
        public IActionResult PrintSaleAPEdit(int PurchaseID)
        {
            var list = (from PA in _context.SaleAREdites.Where(m => m.SARID == PurchaseID && m.CompanyID == GetCompany().ID)
                        join PAD in _context.SaleAREditeDetails on PA.SARID equals PAD.SARID
                        join BP in _context.BusinessPartners on PA.UserID equals BP.ID
                        join U in _context.UserAccounts on PA.UserID equals U.ID
                        join B in _context.Branches on PA.BranchID equals B.ID
                        join R in _context.ReceiptInformation on B.ID equals R.ID
                        join E in _context.Employees on U.EmployeeID equals E.ID
                        join C in _context.Currency on PA.SaleCurrencyID equals C.ID
                        join D in _context.Company on B.CompanyID equals D.ID
                        join I in _context.ItemMasterDatas on PAD.ItemID equals I.ID
                        join N in _context.UnitofMeasures on PAD.UomID equals N.ID
                        join S in _context.Series on PA.SeriesID equals S.ID
                        select new PrintSaleHistory
                        {
                            AddressEng = R.EnglishDescription,
                            PreFix = S.PreFix,
                            //Brand = cp == null ? "" : cp.Name,
                            CusNo = BP.Code,
                            Invoice = PA.InvoiceNo,
                            ExchangeRate = PA.ExchangeRate,
                            Balance_Due_Sys = PA.TotalAmountSys,
                            Applied_Amount = PA.AppliedAmount,
                            PostingDate = PA.PostingDate.ToString("dd-MM-yyyy"),
                            DocumentDate = PA.DocumentDate.ToString("dd-MM-yyyy"),
                            //DueDate = PA..ToString("dd-MM-yyyy"),
                            Sub_Total = PA.TotalAmount,
                            DiscountValue = PA.DisValue,
                            DiscountRate = PA.DisRate,
                            TaxValue = Convert.ToDecimal(PA.VatValue),
                            CompanyName = D.Name,
                            Addresskh = R.Address,
                            TaxRate = Convert.ToDecimal(PA.VatRate),
                            TypeDis = PA.TypeDis,
                            CusName = BP.Name,
                            Tel1 = R.Tel1,
                            Tel2 = R.Tel2,
                            Phone = BP.Phone,
                            Address = BP.Address,
                            UserName = E.Name,
                            RefNo = PA.RefNo,
                            //c = PA.Number,
                            Logo = D.Logo,
                            LocalCurrency = C.Description,
                            //Deatai
                            Total = PAD.Total,
                            ItemNameKh = I.KhmerName,
                            ItemCode = I.Code,
                            Price = PAD.UnitPrice,
                            UomName = N.Name,
                            Qty = PAD.Qty,
                            Remarks = PA.Remarks
                        });
            if (!list.Any()) return NotFound();
            return new ViewAsPdf(list)
            {
                CustomSwitches = "--footer-left \"Powerby: KHMER EDI\" " + "--footer-center \"Page: [page] of [toPage]\" --footer-font-size \"9\""
            };
        }
        //Revenues Item
        public IActionResult PrintRevenuesItem(string DateFrom, string DateTo, int BranchID, int ItemID, string Process)
        {
            if (DateFrom == null)
            {
                DateFrom = "1900-01-01";

            }
            if (DateTo == null)
            {
                DateTo = "1900-01-01";
            }
            if (BranchID == 0)
            {
                BranchID = Convert.ToInt32(User.FindFirst("BranchID").Value);
            }
            var brand = _context.Branches.FirstOrDefault(x => x.ID == BranchID);

            var list = _report.GetSummaryRevenuesItems(DateFrom, DateTo, BranchID, ItemID, Process).ToList();
            if (list.Count > 0)
            {
                list.First().DateTo = DateTo;
                list.First().DateFrom = DateFrom;
                list.First().Branch = brand.Name;
            }

            return new ViewAsPdf(list);
        }

        //Print cashout
        public IActionResult PrintCashout(int Tran_F, int Tran_T, int UserID, string DateFrom, string DateTo)
        {
            if (DateFrom == null)
            {
                DateFrom = "1900-01-01";
            }
            if (DateTo == null)
            {
                DateTo = "1900-01-01";
            }

            var cashoutreport = _report.GetDetailCloseShif(UserID, Tran_F, Tran_T).ToList();
            if (cashoutreport.Count > 0)
            {
                cashoutreport.First().DateFrom = DateFrom;
                cashoutreport.First().DateTo = DateTo;
            }
            return new ViewAsPdf(cashoutreport);
        }

        private int GetUserID()
        {
            _ = int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out int _id);
            return _id;
        }

        private Company GetCompany()
        {
            var com = (from us in _context.UserAccounts.Where(w => w.ID == GetUserID())
                       join co in _context.Company on us.CompanyID equals co.ID
                       select co
                       ).FirstOrDefault();
            return com;
        }

        //PrintPricelist
        public IActionResult PrintPriceLists(int pricelistid)
        {
            var brand = _context.Property.FirstOrDefault(i => i.Name == "Brand" && i.Active && i.CompanyID == GetCompany().ID) ?? new Property();
            var com = _context.Company;
            var Receiots = _context.ReceiptInformation;
            var list = (from PLd in _context.PriceListDetails.Where(m => m.PriceListID == pricelistid)
                        join I in _context.ItemMasterDatas on PLd.ItemID equals I.ID
                        join C in _context.Currency on PLd.CurrencyID equals C.ID

                        let pd = _context.PropertyDetails.FirstOrDefault(i => i.ProID == brand.ID && i.ItemID == I.ID)
                        let pdId = pd == null ? 0 : pd.Value
                        let cp = _context.ChildPreoperties.FirstOrDefault(i => i.ID == pdId)

                        select new PrintPricelist
                        {
                            AddressEng = Receiots.FirstOrDefault().EnglishDescription,
                            Brand = cp == null ? "" : cp.Name,
                            Addresskh = Receiots.FirstOrDefault().Address,
                            Code = I.Code,
                            ItemName = I.KhmerName,
                            Price = PLd.UnitPrice,
                            Logo = com.FirstOrDefault().Logo,
                            Currency = C.Description,
                        }).ToList();
            if (!list.Any()) return NotFound();
            return new ViewAsPdf(list)
            {
                CustomSwitches = "--footer-left \"Powerby: KHMER EDI\" " + "--footer-center \"Page: [page] of [toPage]\" --footer-font-size \"9\""
            };
        }

        //Print PurchaseOrder
        public IActionResult PrintPurchaseOrder(int PurchaseID)
        {
            var brand = _context.Property.FirstOrDefault(i => i.Name == "Brand" && i.Active && i.CompanyID == GetCompany().ID) ?? new Property();
            var list = (from PO in _context.PurchaseOrders.Where(m => m.PurchaseOrderID == PurchaseID && m.CompanyID == GetCompany().ID)
                        join POD in _context.PurchaseOrderDetails on PO.PurchaseOrderID equals POD.PurchaseOrderID
                        join BP in _context.BusinessPartners on PO.VendorID equals BP.ID
                        join U in _context.UserAccounts on PO.UserID equals U.ID
                        join B in _context.Branches on PO.BranchID equals B.ID
                        join E in _context.Employees on U.EmployeeID equals E.ID
                        join C in _context.Currency on PO.PurCurrencyID equals C.ID
                        join R in _context.ReceiptInformation on B.ID equals R.ID
                        join D in _context.Company on B.CompanyID equals D.ID
                        join I in _context.ItemMasterDatas on POD.ItemID equals I.ID
                        join N in _context.UnitofMeasures on POD.UomID equals N.ID
                        join S in _context.Series on PO.SeriesID equals S.ID
                        let pd = _context.PropertyDetails.FirstOrDefault(i => i.ProID == brand.ID && i.ItemID == I.ID)
                        let pdId = pd == null ? 0 : pd.Value
                        let cp = _context.ChildPreoperties.FirstOrDefault(i => i.ID == pdId)

                        select new PrintPurchaseAP
                        {
                            AddressEng = R.EnglishDescription,
                            Brand = cp == null ? "" : cp.Name,
                            PreFix = S.PreFix,
                            CusCode = BP.Code,
                            Invoice = PO.Number,
                            ExchangeRate = PO.PurRate,
                            Balance_Due_Sys = PO.BalanceDueSys,
                            Applied_Amount = PO.AppliedAmount,
                            PostingDate = PO.PostingDate.ToString("dd-MM-yyyy"),
                            DocumentDate = PO.DocumentDate.ToString("dd-MM-yyyy"),
                            DueDate = PO.DeliveryDate.ToString("dd-MM-yyyy"),
                            DiscountValue = PO.DiscountValue,
                            DiscountValue_Detail = POD.DiscountValue,
                            TaxValue = PO.TaxValue,
                            CompanyName = D.Name,
                            Addresskh = R.Address,
                            TaxRate = PO.TaxRate,
                            TypeDis = PO.TypeDis,
                            VendorName = BP.Name,
                            Tel1 = R.Tel1,
                            Tel2 = R.Tel2,
                            Phone = BP.Phone,
                            Address = BP.Address,
                            UserName = E.Name,
                            SQN = PO.ReffNo,
                            VendorNo = PO.Number,
                            Logo = D.Logo,
                            LocalCurrency = C.Description,
                            Total = POD.Total,
                            Sub_Total = PO.BalanceDue,
                            KhmerName = I.KhmerName,
                            Code = I.Code,
                            Price = POD.PurchasPrice,
                            UomName = N.Name,
                            Qty = POD.Qty,
                            Remark = PO.Remark
                        }).ToList();
            if (!list.Any()) return NotFound();
            return new ViewAsPdf(list)
            {
                CustomSwitches = "--footer-left \"Powerby: KHMER EDI\" " + "--footer-center \"Page: [page] of [toPage]\" --footer-font-size \"9\""
            };
        }

        //PrintPurchaseCreditMemo
        public IActionResult PrintPurchaseCreditMemo(int PurchaseID)
        {
            var brand = _context.Property.FirstOrDefault(i => i.Name == "Brand" && i.Active && i.CompanyID == GetCompany().ID) ?? new Property();
            var list = (from PC in _context.PurchaseCreditMemos.Where(m => m.PurchaseMemoID == PurchaseID && m.CompanyID == GetCompany().ID)
                        join PCD in _context.PurchaseCreditMemoDetails on PC.PurchaseMemoID equals PCD.PurchaseCreditMemoID
                        join BP in _context.BusinessPartners on PC.VendorID equals BP.ID
                        join U in _context.UserAccounts on PC.UserID equals U.ID
                        join E in _context.Employees on U.EmployeeID equals E.ID
                        join C in _context.Currency on PC.PurCurrencyID equals C.ID
                        join I in _context.ItemMasterDatas on PCD.ItemID equals I.ID
                        join N in _context.UnitofMeasures on PCD.UomID equals N.ID
                        join B in _context.Branches on PC.BranchID equals B.ID
                        join R in _context.ReceiptInformation on B.ID equals R.ID
                        join D in _context.Company on B.CompanyID equals D.ID
                        join S in _context.Series on PC.SeriesID equals S.ID
                        let pd = _context.PropertyDetails.FirstOrDefault(i => i.ProID == brand.ID && i.ItemID == I.ID)
                        let pdId = pd == null ? 0 : pd.Value
                        let cp = _context.ChildPreoperties.FirstOrDefault(i => i.ID == pdId)

                        select new PrintPurchaseAP
                        {
                            AddressEng = R.EnglishDescription,
                            PreFix = S.PreFix,
                            Brand = cp == null ? "" : cp.Name,
                            CusCode = BP.Code,
                            Invoice = PC.Number,
                            Balance_Due_Sys = PC.BalanceDueSys,
                            Applied_Amount = PC.AppliedAmount,
                            PostingDate = PC.PostingDate.ToString("dd-MM-yyyy"),
                            DocumentDate = PC.DocumentDate.ToString("dd-MM-yyyy"),
                            //DueDate = PC.DeliveryDate.ToString("dd-MM-yyyy"),
                            Sub_Total = PC.BalanceDue,
                            DiscountValue = PC.DiscountValue,
                            DiscountRate = PC.DiscountRate,
                            TaxValue = PC.TaxValue,
                            CompanyName = D.Name,
                            Addresskh = R.Address,
                            TaxRate = PC.TaxRate,
                            TypeDis = PC.TypeDis,
                            VendorName = BP.Name,
                            Tel1 = R.Tel1,
                            Tel2 = R.Tel2,
                            Phone = BP.Phone,
                            Address = BP.Address,
                            UserName = E.Name,
                            SQN = PC.ReffNo,
                            VendorNo = PC.Number,
                            Logo = D.Logo,
                            LocalCurrency = C.Description,
                            //Deatai
                            Total = PCD.Total,
                            KhmerName = I.KhmerName,
                            Code = I.Code,
                            Price = PCD.PurchasPrice,
                            UomName = N.Name,
                            Qty = PCD.Qty,
                            Remark = PC.Remark
                        }).ToList();
            if (!list.Any()) return NotFound();
            return new ViewAsPdf(list)
            {
                CustomSwitches = "--footer-left \"Powerby: KHMER EDI\" " + "--footer-center \"Page: [page] of [toPage]\" --footer-font-size \"9\""
            };
        }

        //Print GoodReceiptPO
        public IActionResult PrintGoodsReceiptPO(int PurchaseID)
        {
            var brand = _context.Property.FirstOrDefault(i => i.Name == "Brand" && i.Active && i.CompanyID == GetCompany().ID) ?? new Property();
            var list = (from PR in _context.GoodsReciptPOs.Where(m => m.ID == PurchaseID && m.CompanyID == GetCompany().ID)
                        join PRD in _context.GoodReciptPODetails on PR.ID equals PRD.GoodsReciptPOID
                        join BP in _context.BusinessPartners on PR.VendorID equals BP.ID
                        join U in _context.UserAccounts on PR.UserID equals U.ID
                        join E in _context.Employees on U.EmployeeID equals E.ID
                        join C in _context.Currency on PR.PurCurrencyID equals C.ID
                        join I in _context.ItemMasterDatas on PRD.ItemID equals I.ID
                        join N in _context.UnitofMeasures on PRD.UomID equals N.ID
                        join B in _context.Branches on PR.BranchID equals B.ID
                        join R in _context.ReceiptInformation on B.ID equals R.ID
                        join D in _context.Company on B.CompanyID equals D.ID
                        join S in _context.Series on PR.SeriesID equals S.ID
                        let pd = _context.PropertyDetails.FirstOrDefault(i => i.ProID == brand.ID && i.ItemID == I.ID)
                        let pdId = pd == null ? 0 : pd.Value
                        let cp = _context.ChildPreoperties.FirstOrDefault(i => i.ID == pdId)

                        select new PrintPurchaseAP
                        {
                            AddressEng = R.EnglishDescription,
                            PreFix = S.PreFix,
                            Brand = cp == null ? "" : cp.Name,
                            Invoice = PR.InvoiceNo,
                            CusCode = BP.Code,
                            ExchangeRate = PR.PurRate,
                            Balance_Due_Sys = PR.BalanceDueSys,
                            Applied_Amount = PR.AppliedAmount,
                            DiscountValue_Detail = PR.DiscountValue,
                            DiscountRate_Detail = PR.DiscountRate,
                            PostingDate = PR.PostingDate.ToString("dd-MM-yyyy"),
                            DocumentDate = PR.DocumentDate.ToString("dd-MM-yyyy"),
                            //DueDate = PR.di.ToString("dd-MM-yyyy"),
                            Sub_Total = PR.BalanceDue,
                            DiscountValue = PR.DiscountValue,
                            DiscountRate = PR.DiscountRate,
                            VendorName = BP.Name,
                            CompanyName = D.Name,
                            Phone = BP.Phone,
                            Addresskh = R.Address,
                            Address = BP.Address,
                            UserName = E.Name,
                            LocalCurrency = C.Description,
                            //Detail
                            SQN = PR.ReffNo,
                            Logo = D.Logo,
                            Tel1 = R.Tel1,
                            Tel2 = R.Tel2,
                            KhmerName = I.KhmerName,
                            Code = I.Code,
                            Price = I.UnitPrice,
                            UomName = N.Name,
                            Qty = PRD.Qty,
                            Remark = PR.Remark,
                        }).ToList();
            if (!list.Any()) return NotFound();
            return new ViewAsPdf(list)
            {
                CustomSwitches = "--footer-left \"Powerby: KHMER EDI\" " + "--footer-center \"Page: [page] of [toPage]\" --footer-font-size \"9\""
            };
        }

        //Print PurchaseAP
        public IActionResult PrintPurchaseAP(int PurchaseID)
        {
            var brand = _context.Property.FirstOrDefault(i => i.Name == "Brand" && i.Active && i.CompanyID == GetCompany().ID) ?? new Property();
            var list = (from PA in _context.Purchase_APs.Where(m => m.PurchaseAPID == PurchaseID && m.CompanyID == GetCompany().ID)
                        join PAD in _context.PurchaseAPDetail on PA.PurchaseAPID equals PAD.PurchaseAPID
                        join BP in _context.BusinessPartners on PA.VendorID equals BP.ID
                        join U in _context.UserAccounts on PA.UserID equals U.ID
                        join B in _context.Branches on PA.BranchID equals B.ID
                        join R in _context.ReceiptInformation on B.ID equals R.ID
                        join E in _context.Employees on U.EmployeeID equals E.ID
                        join C in _context.Currency on PA.PurCurrencyID equals C.ID
                        join D in _context.Company on B.CompanyID equals D.ID
                        join I in _context.ItemMasterDatas on PAD.ItemID equals I.ID
                        join N in _context.UnitofMeasures on PAD.UomID equals N.ID
                        join S in _context.Series on PA.SeriesID equals S.ID
                        let pd = _context.PropertyDetails.FirstOrDefault(i => i.ProID == brand.ID && i.ItemID == I.ID)
                        let pdId = pd == null ? 0 : pd.Value
                        let cp = _context.ChildPreoperties.FirstOrDefault(i => i.ID == pdId)

                        select new PrintPurchaseAP
                        {
                            AddressEng = R.EnglishDescription,
                            PreFix = S.PreFix,
                            Brand = cp == null ? "" : cp.Name,
                            CusCode = BP.Code,
                            Invoice = PA.InvoiceNo,
                            ExchangeRate = PA.PurRate,
                            Balance_Due_Sys = PA.BalanceDueSys,
                            Applied_Amount = PA.AppliedAmount,
                            PostingDate = PA.PostingDate.ToString("dd-MM-yyyy"),
                            DocumentDate = PA.DocumentDate.ToString("dd-MM-yyyy"),
                            //DueDate = PA..ToString("dd-MM-yyyy"),
                            Sub_Total = PA.BalanceDue,
                            DiscountValue = PA.DiscountValue,
                            DiscountRate = PA.DiscountRate,
                            TaxValue = PA.TaxValue,
                            CompanyName = D.Name,
                            Addresskh = R.Address,
                            TaxRate = PA.TaxRate,
                            TypeDis = PA.TypeDis,
                            VendorName = BP.Name,
                            Tel1 = R.Tel1,
                            Tel2 = R.Tel2,
                            Phone = BP.Phone,
                            Address = BP.Address,
                            UserName = E.Name,
                            SQN = PA.ReffNo,
                            VendorNo = PA.Number,
                            Logo = D.Logo,
                            LocalCurrency = C.Description,
                            //Deatai
                            Total = PAD.Total,
                            KhmerName = I.KhmerName,
                            Code = I.Code,
                            Price = PAD.PurchasPrice,
                            UomName = N.Name,
                            Qty = PAD.Qty,
                            Remark = PA.Remark
                        }).ToList();
            if (!list.Any()) return NotFound();
            return new ViewAsPdf(list)
            {
                CustomSwitches = "--footer-left \"Powerby: KHMER EDI\" " + "--footer-center \"Page: [page] of [toPage]\" --footer-font-size \"9\""
            };
        }
        //Print PurchaseAP reserve
        public IActionResult PrintPurchaseAPReserve(int PurchaseID)
        {
            var brand = _context.Property.FirstOrDefault(i => i.Name == "Brand" && i.Active && i.CompanyID == GetCompany().ID) ?? new Property();
            var list = (from PA in _context.PurchaseAPReserves.Where(m => m.ID == PurchaseID && m.CompanyID == GetCompany().ID)
                        join PAD in _context.PurchaseAPReserveDetails on PA.ID equals PAD.PurchaseAPReserveID
                        join BP in _context.BusinessPartners on PA.VendorID equals BP.ID
                        join U in _context.UserAccounts on PA.UserID equals U.ID
                        join B in _context.Branches on PA.BranchID equals B.ID
                        join R in _context.ReceiptInformation on B.ID equals R.ID
                        join E in _context.Employees on U.EmployeeID equals E.ID
                        join C in _context.Currency on PA.PurCurrencyID equals C.ID
                        join D in _context.Company on B.CompanyID equals D.ID
                        join I in _context.ItemMasterDatas on PAD.ItemID equals I.ID
                        join N in _context.UnitofMeasures on PAD.UomID equals N.ID
                        join S in _context.Series on PA.SeriesID equals S.ID
                        let pd = _context.PropertyDetails.FirstOrDefault(i => i.ProID == brand.ID && i.ItemID == I.ID)
                        let pdId = pd == null ? 0 : pd.Value
                        let cp = _context.ChildPreoperties.FirstOrDefault(i => i.ID == pdId)

                        select new PrintPurchaseAP
                        {
                            AddressEng = R.EnglishDescription,
                            PreFix = S.PreFix,
                            Brand = cp == null ? "" : cp.Name,
                            CusCode = BP.Code,
                            Invoice = PA.InvoiceNo,
                            ExchangeRate = PA.PurRate,
                            Balance_Due_Sys = PA.BalanceDueSys,
                            Applied_Amount = PA.AppliedAmount,
                            PostingDate = PA.PostingDate.ToString("dd-MM-yyyy"),
                            DocumentDate = PA.DocumentDate.ToString("dd-MM-yyyy"),
                            //DueDate = PA..ToString("dd-MM-yyyy"),
                            Sub_Total = PA.BalanceDue,
                            DiscountValue = PA.DiscountValue,
                            DiscountRate = PA.DiscountRate,
                            TaxValue = PA.TaxValue,
                            CompanyName = D.Name,
                            Addresskh = R.Address,
                            TaxRate = PA.TaxRate,
                            TypeDis = PA.TypeDis,
                            VendorName = BP.Name,
                            Tel1 = R.Tel1,
                            Tel2 = R.Tel2,
                            Phone = BP.Phone,
                            Address = BP.Address,
                            UserName = E.Name,
                            SQN = PA.ReffNo,
                            VendorNo = PA.Number,
                            Logo = D.Logo,
                            LocalCurrency = C.Description,
                            //Deatai
                            Total = PAD.Total,
                            KhmerName = I.KhmerName,
                            Code = I.Code,
                            Price = PAD.PurchasPrice,
                            UomName = N.Name,
                            Qty = PAD.Qty,
                            Remark = PA.Remark
                        }).ToList();
            if (!list.Any()) return NotFound();
            return new ViewAsPdf(list)
            {
                CustomSwitches = "--footer-left \"Powerby: KHMER EDI\" " + "--footer-center \"Page: [page] of [toPage]\" --footer-font-size \"9\""
            };
        }
        //Print SaleQuote
        public IActionResult SaleQuoteHistory(int ID)
        {
            var list = GetSaleQoute(ID);
            if (!list.Any()) return NotFound();
            return new ViewAsPdf(list)
            {
                CustomSwitches = "--footer-left \"Powerby: KHMER EDI\" " + "--footer-center \"Page: [page] of [toPage]\" --footer-font-size \"9\""
            };
        }
        //Print SaleQuote report none vat
        public IActionResult SaleQuoteHistoryNoneVat(int ID)
        {
            var list = GetSaleQoute(ID);
            if (list == null) return NotFound();
            return new ViewAsPdf(list)
            {
                CustomSwitches = "--footer-left \"Powerby: KHMER EDI\" " + "--footer-center \"Page: [page] of [toPage]\" --footer-font-size \"9\""
            };
        }
        //Print SaleQuote report  vat
        public IActionResult SaleQuoteHistoryVat(int ID)
        {
            var list = GetSaleQoute(ID);
            if (list == null) return NotFound();
            return new ViewAsPdf(list)
            {
                CustomSwitches = "--footer-left \"Powerby: KHMER EDI\" " + "--footer-center \"Page: [page] of [toPage]\" --footer-font-size \"9\""
            };
        }

        //Print SaleOrder
        public IActionResult SaleOrderHistory(int ID)
        {
            var brand = _context.Property.FirstOrDefault(i => i.Name == "Brand" && i.Active && i.CompanyID == GetCompany().ID) ?? new Property();
            var list = (from SO in _context.SaleOrders.Where(m => m.SOID == ID)
                        join SOD in _context.SaleOrderDetails on SO.SOID equals SOD.SOID
                        join BP in _context.BusinessPartners on SO.CusID equals BP.ID
                        join I in _context.ItemMasterDatas on SOD.ItemID equals I.ID
                        join CUR in _context.Currency on SO.SaleCurrencyID equals CUR.ID
                        join B in _context.Branches on SO.BranchID equals B.ID
                        join U in _context.UserAccounts on SO.BranchID equals U.BranchID
                        join R in _context.ReceiptInformation on B.ID equals R.ID
                        join C in _context.Company on B.CompanyID equals C.ID
                        join S in _context.Series on SO.SeriesID equals S.ID
                        let pd = _context.PropertyDetails.FirstOrDefault(i => i.ProID == brand.ID && i.ItemID == I.ID)
                        let pdId = pd == null ? 0 : pd.Value
                        let cp = _context.ChildPreoperties.FirstOrDefault(i => i.ID == pdId)
                        group new { SO, SOD, BP, I, CUR, B, U, R, C, pd, pdId, cp, S } by SOD.SODID into g
                        let data = g.FirstOrDefault()
                        let master = data.SO
                        let detail = data.SOD
                        let cu = data.CUR
                        let BP = data.BP
                        let B = data.B
                        let U = data.U
                        let R = data.R
                        let C = data.C
                        let cp = data.cp
                        let S = data.S
                        select new PrintSaleHistory
                        {
                            //Master
                            ID = g.FirstOrDefault().SO.SOID,
                            Invoice = master.InvoiceNumber,
                            PostingDate = master.PostingDate.ToString("dd-MM-yyyy"),
                            DocumentDate = master.DocumentDate.ToString("dd-MM-yyyy"),
                            DueDate = master.DeliveryDate.ToString("dd-MM-yyyy"),
                            CusName = BP.Name,
                            Phone = BP.Phone,
                            Address = BP.Address,
                            UserName = U.Username,
                            CusNo = BP.Code,
                            RefNo = master.RefNo,
                            Branch = g.First().B.Name,
                            Remarks = master.Remarks,
                            //Detail
                            PreFix = S.PreFix,
                            Brand = cp == null ? "" : cp.Name,
                            ItemCode = detail.ItemCode,
                            ItemNameKh = detail.ItemNameKH,
                            Qty = detail.Qty,
                            Price = detail.UnitPrice,
                            DiscountValue_Detail = detail.DisValue,
                            UomName = detail.UomName,
                            Amount = detail.Total,
                            LocalCurrency = cu.Description,
                            Tel1 = R.Tel1,
                            Tel2 = R.Tel2,
                            Addresskh = R.Address,
                            AddressEng = R.EnglishDescription,
                            CompanyName = C.Name,
                            Logo = C.Logo,
                            //Summary
                            DiscountValue = master.DisValue,
                            VatValue = master.VatValue,
                            TotalAmount = master.TotalAmount
                        }).ToList();
            //list = null;
            if (!list.Any()) return NotFound();
            return new ViewAsPdf(list)
            {
                CustomSwitches = "--footer-left \"Powerby: KHMER EDI\" " + "--footer-center \"Page: [page] of [toPage]\" --footer-font-size \"9\""
            };
        }
        //SaleOrderHistoryNonVat
        public IActionResult SaleOrderHistoryNonVat(int ID)
        {
            var brand = _context.Property.FirstOrDefault(i => i.Name == "Brand" && i.Active && i.CompanyID == GetCompany().ID) ?? new Property();
            var list = (from SO in _context.SaleOrders.Where(m => m.SOID == ID)
                        join SOD in _context.SaleOrderDetails on SO.SOID equals SOD.SOID
                        join BP in _context.BusinessPartners on SO.CusID equals BP.ID
                        join I in _context.ItemMasterDatas on SOD.ItemID equals I.ID
                        join CUR in _context.Currency on SO.SaleCurrencyID equals CUR.ID
                        join B in _context.Branches on SO.BranchID equals B.ID
                        join U in _context.UserAccounts on SO.BranchID equals U.BranchID
                        join R in _context.ReceiptInformation on B.ID equals R.ID
                        join C in _context.Company on B.CompanyID equals C.ID
                        join S in _context.Series on SO.SeriesID equals S.ID
                        let pd = _context.PropertyDetails.FirstOrDefault(i => i.ProID == brand.ID && i.ItemID == I.ID)
                        let emp = _context.Employees.FirstOrDefault(x => x.ID == SO.SaleEmID) ?? new Models.Services.HumanResources.Employee()
                        let pdId = pd == null ? 0 : pd.Value
                        let cp = _context.ChildPreoperties.FirstOrDefault(i => i.ID == pdId)
                        group new { SO, SOD, BP, I, CUR, B, U, R, C, pd, pdId, cp, S, emp } by SOD.SODID into g
                        let data = g.FirstOrDefault()
                        let master = data.SO
                        let detail = data.SOD
                        let cu = data.CUR
                        let BP = data.BP
                        let B = data.B
                        let emp = data.emp
                        let U = data.U
                        let R = data.R
                        let C = data.C
                        let cp = data.cp
                        let S = data.S

                        let reqBy = _context.Employees.FirstOrDefault(i => i.ID == master.RequestedBy) ?? new Employee()
                        let shipBy = _context.Employees.FirstOrDefault(i => i.ID == master.ShippedBy) ?? new Employee()
                        let receiveBy = _context.Employees.FirstOrDefault(i => i.ID == master.ReceivedBy) ?? new Employee()
                        select new PrintSaleHistory
                        {
                            //Master
                            ID = g.FirstOrDefault().SO.SOID,
                            Invoice = master.InvoiceNumber,
                            PostingDate = master.PostingDate.ToString("dd-MM-yyyy"),
                            DocumentDate = master.DocumentDate.ToString("dd-MM-yyyy"),
                            DueDate = master.DeliveryDate.ToString("dd-MM-yyyy"),
                            CusName = BP.Name,
                            RequestedBy = reqBy.Name,
                            ShippedBy = shipBy.Name,
                            ReceivedBy = receiveBy.Name,
                            TotalQty = _context.SaleOrderDetails.Where(x => x.SOID == master.SOID).Sum(x => x.Qty),
                            TotalUniprice = _context.SaleOrderDetails.Where(sc => sc.SOID == master.SOID).Sum(sc => sc.UnitPrice),
                            EmpName = emp.Name,
                            Phone = BP.Phone,
                            Address = BP.Address,
                            UserName = U.Username,
                            CusNo = BP.Code,
                            RefNo = master.RefNo,
                            Branch = g.First().B.Name,
                            Remarks = detail.Remarks,
                            //Detail
                            PreFix = S.PreFix,
                            Brand = cp == null ? "" : cp.Name,
                            ItemCode = detail.ItemCode,
                            ItemNameKh = detail.ItemNameKH,
                            Qty = detail.Qty,
                            Price = detail.UnitPrice,
                            DiscountValue_Detail = detail.DisValue,
                            UomName = detail.UomName,
                            Amount = detail.Total,

                            LocalCurrency = cu.Description,
                            Tel1 = R.Tel1,
                            Tel2 = R.Tel2,
                            Addresskh = R.Address,
                            AddressEng = R.EnglishDescription,
                            CompanyName = C.Name,
                            Logo = C.Logo,
                            //Summary
                            DiscountValue = master.DisValue,
                            VatValue = master.VatValue,
                            Disvalue = detail.DisValue,
                            TotalDisValue = _context.SaleOrderDetails.Where(x => x.SOID == master.SOID).Sum(x => x.DisValue),
                            TotalBeforDis = detail.Qty * detail.UnitPrice,
                            TotalOfAmountBeforDis = _context.SaleOrderDetails.Where(x => x.SOID == master.SOID).Sum(x => x.UnitPrice * x.Qty),
                            TotalAmount = master.TotalAmount,
                            SubTotal = master.SubTotal,
                            TaxValue = detail.TaxValue,
                            TotalWTax = detail.TotalWTax,

                        }).ToList();
            //list = null;
            if (!list.Any()) return NotFound();
            return new ViewAsPdf(list)
            {
                CustomSwitches = "--footer-left \"Powerby: KHMER EDI\" " + "--footer-center \"Page: [page] of [toPage]\" --footer-font-size \"9\""
            };
        }
        //SaleOrderHistoryVat
        public IActionResult SaleOrderHistoryVat(int ID)
        {
            var brand = _context.Property.FirstOrDefault(i => i.Name == "Brand" && i.Active && i.CompanyID == GetCompany().ID) ?? new Property();
            var list = (from SO in _context.SaleOrders.Where(m => m.SOID == ID)
                        join SOD in _context.SaleOrderDetails on SO.SOID equals SOD.SOID
                        join BP in _context.BusinessPartners on SO.CusID equals BP.ID
                        join I in _context.ItemMasterDatas on SOD.ItemID equals I.ID
                        join CUR in _context.Currency on SO.SaleCurrencyID equals CUR.ID
                        join B in _context.Branches on SO.BranchID equals B.ID
                        join U in _context.UserAccounts on SO.BranchID equals U.BranchID
                        join R in _context.ReceiptInformation on B.ID equals R.ID
                        join C in _context.Company on B.CompanyID equals C.ID
                        join S in _context.Series on SO.SeriesID equals S.ID
                        let pd = _context.PropertyDetails.FirstOrDefault(i => i.ProID == brand.ID && i.ItemID == I.ID)
                        let emp = _context.Employees.FirstOrDefault(x => x.ID == SO.SaleEmID) ?? new Models.Services.HumanResources.Employee()
                        let pdId = pd == null ? 0 : pd.Value
                        let cp = _context.ChildPreoperties.FirstOrDefault(i => i.ID == pdId)
                        group new { SO, SOD, BP, I, CUR, B, U, R, C, pd, pdId, cp, S, emp } by SOD.SODID into g
                        let data = g.FirstOrDefault()
                        let master = data.SO
                        let detail = data.SOD
                        let cu = data.CUR
                        let BP = data.BP
                        let B = data.B
                        let emp = data.emp
                        let U = data.U
                        let R = data.R
                        let C = data.C
                        let cp = data.cp
                        let S = data.S
                        let reqBy = _context.Employees.FirstOrDefault(i => i.ID == master.RequestedBy) ?? new Employee()
                        let shipBy = _context.Employees.FirstOrDefault(i => i.ID == master.ShippedBy) ?? new Employee()
                        let receiveBy = _context.Employees.FirstOrDefault(i => i.ID == master.ReceivedBy) ?? new Employee()
                        select new PrintSaleHistory
                        {
                            //Master
                            ID = g.FirstOrDefault().SO.SOID,
                            Invoice = master.InvoiceNumber,
                            PostingDate = master.PostingDate.ToString("dd-MM-yyyy"),
                            DocumentDate = master.DocumentDate.ToString("dd-MM-yyyy"),
                            DueDate = master.DeliveryDate.ToString("dd-MM-yyyy"),
                            CusName = BP.Name,
                            RequestedBy = reqBy.Name,
                            ShippedBy = shipBy.Name,
                            ReceivedBy = receiveBy.Name,
                            EmpName = emp.Name,
                            Phone = BP.Phone,
                            Address = BP.Address,
                            UserName = U.Username,
                            CusNo = BP.Code,
                            RefNo = master.RefNo,
                            Branch = g.First().B.Name,
                            Remarks = detail.Remarks,
                            //Detail
                            PreFix = S.PreFix,
                            Brand = cp == null ? "" : cp.Name,
                            ItemCode = detail.ItemCode,
                            ItemNameKh = detail.ItemNameKH,
                            Qty = detail.Qty,
                            TotalQty = _context.SaleOrderDetails.Where(sc => sc.SOID == master.SOID).Sum(sc => sc.Qty),
                            TotalUniprice = _context.SaleOrderDetails.Where(sc => sc.SOID == master.SOID).Sum(sc => sc.UnitPrice),
                            Price = detail.UnitPrice,
                            DiscountValue_Detail = detail.DisValue,
                            UomName = detail.UomName,
                            Amount = detail.Total,
                            LocalCurrency = cu.Description,
                            Tel1 = R.Tel1,
                            Tel2 = R.Tel2,
                            Addresskh = R.Address,
                            AddressEng = R.EnglishDescription,
                            CompanyName = C.Name,
                            Logo = C.Logo,
                            //Summary
                            DiscountValue = master.DisValue,
                            Disvalue = detail.DisValue,
                            VatValue = master.VatValue,
                            TotalAmount = master.TotalAmount,
                            SubTotal = master.SubTotal,
                            TaxValue = detail.TaxValue,
                            TotalWTax = detail.TotalWTax

                        }).ToList();
            //list = null;
            if (!list.Any()) return NotFound();
            return new ViewAsPdf(list)
            {
                CustomSwitches = "--footer-left \"Powerby: KHMER EDI\" " + "--footer-center \"Page: [page] of [toPage]\" --footer-font-size \"9\""
            };
        }
        //Print SaleCreditMemo
        public IActionResult SaleCreditMemoHistory(int ID)
        {

            var brand = _context.Property.FirstOrDefault(i => i.Name == "Brand" && i.Active && i.CompanyID == GetCompany().ID) ?? new Property();
            var list = (from SC in _context.SaleCreditMemos.Where(m => m.SCMOID == ID)
                        join SCD in _context.SaleCreditMemoDetails on SC.SCMOID equals SCD.SCMOID
                        join BP in _context.BusinessPartners on SC.CusID equals BP.ID
                        join I in _context.ItemMasterDatas on SCD.ItemID equals I.ID
                        join CUR in _context.Currency on SC.SaleCurrencyID equals CUR.ID
                        join B in _context.Branches on SC.BranchID equals B.ID
                        join U in _context.UserAccounts on SC.BranchID equals U.BranchID
                        join R in _context.ReceiptInformation on B.ID equals R.ID
                        join C in _context.Company on B.CompanyID equals C.ID
                        join S in _context.Series on SC.SeriesID equals S.ID
                        let pd = _context.PropertyDetails.FirstOrDefault(i => i.ProID == brand.ID && i.ItemID == I.ID)
                        let pdId = pd == null ? 0 : pd.Value
                        let cp = _context.ChildPreoperties.FirstOrDefault(i => i.ID == pdId)
                        group new { SC, SCD, BP, I, CUR, B, U, R, C, cp, S } by SCD.SCMODID into g
                        let data = g.FirstOrDefault()
                        let master = data.SC
                        let detail = data.SCD
                        let cu = data.CUR
                        let BP = data.BP
                        let B = data.B
                        let U = data.U
                        let R = data.R
                        let C = data.C
                        let cp = data.cp
                        let S = data.S
                        select new PrintSaleHistory
                        {
                            //Master
                            ID = g.FirstOrDefault().SC.SCMOID,
                            Invoice = master.InvoiceNumber,
                            PostingDate = master.PostingDate.ToString("dd-MM-yyyy"),
                            DocumentDate = master.DocumentDate.ToString("dd-MM-yyyy"),
                            DueDate = master.DueDate.ToString("dd-MM-yyyy"),
                            CusName = BP.Name,
                            Phone = BP.Phone,
                            Address = BP.Address,
                            UserName = U.Username,
                            CusNo = BP.Code,
                            RefNo = master.RefNo,
                            Branch = g.First().B.Name,
                            BaseOn = master.CopyKey,
                            Remarks = master.Remarks,
                            //Detail
                            PreFix = S.PreFix,
                            Brand = cp == null ? "" : cp.Name,
                            ItemCode = detail.ItemCode,
                            ItemNameKh = detail.ItemNameKH,
                            Qty = detail.Qty,
                            Price = detail.UnitPrice,
                            DiscountValue_Detail = detail.DisValue,
                            UomName = detail.UomName,
                            Amount = detail.Total,
                            LocalCurrency = cu.Description,
                            Tel1 = R.Tel1,
                            Tel2 = R.Tel2,
                            Addresskh = R.Address,
                            AddressEng = R.EnglishDescription,
                            CompanyName = C.Name,
                            Logo = C.Logo,
                            //Summary
                            DiscountValue = master.DisValue,
                            VatValue = master.VatValue,
                            Applied_Amount = master.AppliedAmount,
                            TotalAmount = master.TotalAmount
                        }).ToList();
            //list = null;
            if (!list.Any()) return NotFound();
            return new ViewAsPdf(list)
            {
                CustomSwitches = "--footer-left \"Powerby: KHMER EDI\" " + "--footer-center \"Page: [page] of [toPage]\" --footer-font-size \"9\""
            };
        }

        //Print SaleAR
        public IActionResult SaleARHistory(int ID)
        {
            var list = GetSaleARData(ID);
            if (!list.Any()) return NotFound();
            //return View(list);
            return new ViewAsPdf(list)
            {
                CustomSwitches = "--footer-left \"Powerby: KHMER EDI\" " + "--footer-center \"Page: [page] of [toPage]\" --footer-line --footer-font-size \"9\" --footer-spacing 2 --footer-font-name \"Myriad Pro\""
            };
        }
        //SaleAREdit

        public IActionResult SaleARHistoryEdit(int ID)
        {
            var list = GetSaleAREditable(ID);
            if (!list.Any()) return NotFound();
            return new ViewAsPdf(list)
            {
                CustomSwitches = "--footer-left \"Powerby: KHMER EDI\" " + "--footer-center \"Page: [page] of [toPage]\" --footer-font-size \"9\""
            };
        }


        //ARReserveHistory
        public IActionResult ARReserveHistory(int ID)
        {
            var brand = _context.Property.FirstOrDefault(i => i.Name == "Brand" && i.Active && i.CompanyID == GetCompany().ID) ?? new Property();
            var list = (from SD in _context.ARReserveInvoices.Where(m => m.ID == ID)
                        join SDD in _context.ARReserveInvoiceDetails on SD.ID equals SDD.ARReserveInvoiceID
                        join BP in _context.BusinessPartners on SD.CusID equals BP.ID
                        join I in _context.ItemMasterDatas on SDD.ItemID equals I.ID
                        join CUR in _context.Currency on SD.SaleCurrencyID equals CUR.ID
                        join B in _context.Branches on SD.BranchID equals B.ID
                        join U in _context.UserAccounts on SD.BranchID equals U.BranchID
                        join R in _context.ReceiptInformation on B.ID equals R.ID
                        join C in _context.Company on B.CompanyID equals C.ID
                        join S in _context.Series on SD.SeriesID equals S.ID
                        let pd = _context.PropertyDetails.FirstOrDefault(i => i.ProID == brand.ID && i.ItemID == I.ID)
                        let pdId = pd == null ? 0 : pd.Value
                        let cp = _context.ChildPreoperties.FirstOrDefault(i => i.ID == pdId)
                        group new { SD, SDD, BP, I, CUR, B, U, R, C, pd, pdId, cp, S } by SDD.ID into g
                        let data = g.FirstOrDefault()
                        let master = data.SD
                        let detail = data.SDD
                        let cu = data.CUR
                        let BP = data.BP
                        let B = data.B
                        let U = data.U
                        let R = data.R
                        let C = data.C
                        let S = data.S
                        let cp = data.cp
                        select new PrintSaleHistory
                        {
                            //Master
                            ID = g.FirstOrDefault().SD.ID,
                            Invoice = master.InvoiceNumber,
                            PostingDate = master.PostingDate.ToString("dd-MM-yyyy"),
                            DocumentDate = master.DocumentDate.ToString("dd-MM-yyyy"),
                            DueDate = master.DueDate.ToString("dd-MM-yyyy"),
                            CusName = BP.Name,
                            Phone = BP.Phone,
                            Address = BP.Address,
                            UserName = U.Username,
                            CusNo = BP.Code,
                            RefNo = master.RefNo,
                            Branch = B.Name,
                            PreFix = S.PreFix,
                            Remarks = master.Remarks,
                            //Detail
                            Brand = cp == null ? "" : cp.Name,
                            ItemCode = detail.ItemCode,
                            ItemNameKh = detail.ItemNameKH,
                            Qty = (double)detail.Qty,
                            Price = (double)detail.UnitPrice,
                            DiscountValue_Detail = (double)detail.DisValue,
                            UomName = detail.UomName,
                            Amount = detail.Total,
                            LocalCurrency = cu.Description,
                            Tel1 = R.Tel1,
                            Tel2 = R.Tel2,
                            Addresskh = R.Address,
                            AddressEng = R.EnglishDescription,
                            CompanyName = C.Name,
                            Logo = C.Logo,
                            //Summary
                            DiscountValue = (double)master.DisValue,
                            VatValue = (double)master.VatValue,
                            TotalAmount = master.TotalAmount
                        }).ToList();
            if (!list.Any()) return NotFound();
            return new ViewAsPdf(list)
            {
                CustomSwitches = "--footer-left \"Powerby: KHMER EDI\" " + "--footer-center \"Page: [page] of [toPage]\" --footer-font-size \"9\""
            };
        }
        //ARReserveEditableHistory
        public IActionResult ARReserveEditableHistory(int ID)
        {
            var brand = _context.Property.FirstOrDefault(i => i.Name == "Brand" && i.Active && i.CompanyID == GetCompany().ID) ?? new Property();
            var list = (from SD in _context.ARReserveInvoiceEditables.Where(m => m.ID == ID)
                        join SDD in _context.ARReserveInvoiceEditableDetails on SD.ID equals SDD.ARReserveInvoiceEditableID
                        join BP in _context.BusinessPartners on SD.CusID equals BP.ID
                        join I in _context.ItemMasterDatas on SDD.ItemID equals I.ID
                        join CUR in _context.Currency on SD.SaleCurrencyID equals CUR.ID
                        join B in _context.Branches on SD.BranchID equals B.ID
                        join U in _context.UserAccounts on SD.BranchID equals U.BranchID
                        join R in _context.ReceiptInformation on B.ID equals R.ID
                        join C in _context.Company on B.CompanyID equals C.ID
                        join S in _context.Series on SD.SeriesID equals S.ID
                        let pd = _context.PropertyDetails.FirstOrDefault(i => i.ProID == brand.ID && i.ItemID == I.ID)
                        let pdId = pd == null ? 0 : pd.Value
                        let cp = _context.ChildPreoperties.FirstOrDefault(i => i.ID == pdId)
                        group new { SD, SDD, BP, I, CUR, B, U, R, C, pd, pdId, cp, S } by SDD.ID into g
                        let data = g.FirstOrDefault()
                        let master = data.SD
                        let detail = data.SDD
                        let cu = data.CUR
                        let BP = data.BP
                        let B = data.B
                        let U = data.U
                        let R = data.R
                        let C = data.C
                        let S = data.S
                        let cp = data.cp
                        select new PrintSaleHistory
                        {
                            //Master
                            ID = g.FirstOrDefault().SD.ID,
                            Invoice = master.InvoiceNumber,
                            PostingDate = master.PostingDate.ToString("dd-MM-yyyy"),
                            DocumentDate = master.DocumentDate.ToString("dd-MM-yyyy"),
                            DueDate = master.DueDate.ToString("dd-MM-yyyy"),
                            CusName = BP.Name,
                            Phone = BP.Phone,
                            Address = BP.Address,
                            UserName = U.Username,
                            CusNo = BP.Code,
                            RefNo = master.RefNo,
                            Branch = B.Name,
                            PreFix = S.PreFix,
                            Remarks = master.Remarks,
                            //Detail
                            Brand = cp == null ? "" : cp.Name,
                            ItemCode = detail.ItemCode,
                            ItemNameKh = detail.ItemNameKH,
                            Qty = (double)detail.Qty,
                            Price = (double)detail.UnitPrice,
                            DiscountValue_Detail = (double)detail.DisValue,
                            UomName = detail.UomName,
                            Amount = detail.Total,
                            LocalCurrency = cu.Description,
                            Tel1 = R.Tel1,
                            Tel2 = R.Tel2,
                            Addresskh = R.Address,
                            AddressEng = R.EnglishDescription,
                            CompanyName = C.Name,
                            Logo = C.Logo,
                            //Summary
                            DiscountValue = (double)master.DisValue,
                            VatValue = (double)master.VatValue,
                            TotalAmount = master.TotalAmount
                        }).ToList();
            if (!list.Any()) return NotFound();
            return new ViewAsPdf(list)
            {
                CustomSwitches = "--footer-left \"Powerby: KHMER EDI\" " + "--footer-center \"Page: [page] of [toPage]\" --footer-font-size \"9\""
            };
        }


        //Print SaleAR
        public IActionResult SaleARNoneVat01History(int ID)
        {
            var list = GetSaleARData(ID);
            if (!list.Any()) return NotFound();
            //return View(list);
            return new ViewAsPdf(list)
            {
                CustomSwitches = "--footer-left \"Powerby: KHMER EDI\" " + "--footer-center \"Page: [page] of [toPage]\" --footer-line --footer-font-size \"9\" --footer-spacing 2 --footer-font-name \"Myriad Pro\""
            };
        }

        // //Print SaleAR NoneVat
        public IActionResult SaleARHistoryNoneVat(int ID)
        {
            var list = GetSaleARData(ID);
            if (list == null) return NotFound();
            //return View(list);
            return new ViewAsPdf(list)
            {
                CustomSwitches = "--footer-left \"Powerby: KHMER EDI\" " + "--footer-center \"Page: [page] of [toPage]\" --footer-line --footer-font-size \"9\" --footer-spacing 2 --footer-font-name \"Myriad Pro\""
            };
        }

        // //Print SaleAR has Vat
        public IActionResult SaleARHistoryVat(int ID)
        {
            var list = GetSaleARData(ID);
            if (list == null) return NotFound();
            //return View(list);
            return new ViewAsPdf(list)
            {
                CustomSwitches = "--footer-left \"Powerby: KHMER EDI\" " + "--footer-center \"Page: [page] of [toPage]\" --footer-line --footer-font-size \"9\" --footer-spacing 2 --footer-font-name \"Myriad Pro\""
            };
        }
        // //Print SaleAR has VatB
        public IActionResult SaleARHistoryVatB(int ID)
        {
            var list = GetSaleARData(ID);
            if (list == null) return NotFound();
            //return View(list);
            return new ViewAsPdf(list)
            {
                CustomSwitches = "--footer-left \"Powerby: KHMER EDI\" " + "--footer-center \"Page: [page] of [toPage]\" --footer-line --footer-font-size \"9\" --footer-spacing 2 --footer-font-name \"Myriad Pro\""
            };
        }

        // //Print SaleAR has VatC
        public IActionResult SaleARHistoryVatC(int ID)
        {
            var list = GetSaleARData(ID);
            if (list == null) return NotFound();
            //return View(list);
            return new ViewAsPdf(list)
            {
                CustomSwitches = "--footer-left \"Powerby: KHMER EDI\" " + "--footer-center \"Page: [page] of [toPage]\" --footer-line --footer-font-size \"9\" --footer-spacing 2 --footer-font-name \"Myriad Pro\""
            };
        }
        //Print SaleDelivery
        public IActionResult SaleDeliveryHistory(int ID)
        {
            var list = GetSaleDelivery(ID);
            if (!list.Any()) return NotFound();
            return new ViewAsPdf(list)
            {
                CustomSwitches = "--footer-left \"Powerby: KHMER EDI\" " + "--footer-center \"Page: [page] of [toPage]\" --footer-font-size \"9\""
            };
        }
        //Print SaleDelivery SaleDeliveryHistoryNonVat
        public IActionResult SaleDeliveryHistoryNonVat(int ID)
        {
            var list = GetSaleDelivery(ID);
            if (list == null) return NotFound();
            return new ViewAsPdf(list)
            {
                CustomSwitches = "--footer-left \"Powerby: KHMER EDI\" " + "--footer-center \"Page: [page] of [toPage]\" --footer-font-size \"9\""
            };
        }
        //Print SaleDelivery SaleDeliveryHistory has Vat
        public IActionResult SaleDeliveryHistoryVat(int ID)
        {
            var list = GetSaleDelivery(ID);
            if (list == null) return NotFound();
            return new ViewAsPdf(list)
            {
                CustomSwitches = "--footer-left \"Powerby: KHMER EDI\" " + "--footer-center \"Page: [page] of [toPage]\" --footer-font-size \"9\""
            };
        }

        //Print Sale Return Delivery
        public IActionResult ReturnSaleDeliveryHistory(int ID)
        {
            var brand = _context.Property.FirstOrDefault(i => i.Name == "Brand" && i.Active && i.CompanyID == GetCompany().ID) ?? new Property();
            var list = (from SD in _context.ReturnDeliverys.Where(m => m.ID == ID)
                        join SDD in _context.ReturnDeliveryDetails on SD.ID equals SDD.ReturnDeliveryID
                        join BP in _context.BusinessPartners on SD.CusID equals BP.ID
                        join I in _context.ItemMasterDatas on SDD.ItemID equals I.ID
                        join CUR in _context.Currency on SD.SaleCurrencyID equals CUR.ID
                        join B in _context.Branches on SD.BranchID equals B.ID
                        join U in _context.UserAccounts on SD.BranchID equals U.BranchID
                        join R in _context.ReceiptInformation on B.ID equals R.ID
                        join C in _context.Company on B.CompanyID equals C.ID
                        join S in _context.Series on SD.SeriesID equals S.ID
                        let pd = _context.PropertyDetails.FirstOrDefault(i => i.ProID == brand.ID && i.ItemID == I.ID)
                        let pdId = pd == null ? 0 : pd.Value
                        let cp = _context.ChildPreoperties.FirstOrDefault(i => i.ID == pdId)
                        group new { SD, SDD, BP, I, CUR, B, U, R, C, pd, pdId, cp, S } by SDD.ID into g
                        let data = g.FirstOrDefault()
                        let master = data.SD
                        let detail = data.SDD
                        let cu = data.CUR
                        let BP = data.BP
                        let B = data.B
                        let U = data.U
                        let R = data.R
                        let C = data.C
                        let S = data.S
                        let cp = data.cp
                        select new PrintSaleHistory
                        {
                            //Master
                            ID = g.FirstOrDefault().SD.ID,
                            Invoice = master.InvoiceNumber,
                            PostingDate = master.PostingDate.ToString("dd-MM-yyyy"),
                            DocumentDate = master.DocumentDate.ToString("dd-MM-yyyy"),
                            DueDate = master.DocumentDate.ToString("dd-MM-yyyy"),
                            CusName = BP.Name,
                            Phone = BP.Phone,
                            Address = BP.Address,
                            UserName = U.Username,
                            CusNo = BP.Code,
                            RefNo = master.RefNo,
                            Branch = B.Name,
                            PreFix = S.PreFix,
                            Remarks = master.Remarks,
                            //Detail
                            Brand = cp == null ? "" : cp.Name,
                            ItemCode = detail.ItemCode,
                            ItemNameKh = detail.ItemNameKH,
                            Qty = detail.Qty,
                            Price = detail.UnitPrice,
                            DiscountValue_Detail = detail.DisValue,
                            UomName = detail.UomName,
                            Amount = detail.Total,
                            LocalCurrency = cu.Description,
                            Tel1 = R.Tel1,
                            Tel2 = R.Tel2,
                            Addresskh = R.Address,
                            AddressEng = R.EnglishDescription,
                            CompanyName = C.Name,
                            Logo = C.Logo,
                            //Summary
                            DiscountValue = (double)master.DisValue,
                            VatValue = master.VatValue,
                            TotalAmount = master.TotalAmount
                        }).ToList();
            if (!list.Any()) return NotFound();
            return new ViewAsPdf(list)
            {
                CustomSwitches = "--footer-left \"Powerby: KHMER EDI\" " + "--footer-center \"Page: [page] of [toPage]\" --footer-font-size \"9\""
            };
        }
        //Print Sale AR Down Payment
        public IActionResult SaleARDownPaymentHistory(int ID)
        {
            var brand = _context.Property.FirstOrDefault(i => i.Name == "Brand" && i.Active && i.CompanyID == GetCompany().ID) ?? new Property();
            var list = (from SD in _context.ARDownPayments.Where(m => m.ARDID == ID)
                        join SDD in _context.ARDownPaymentDetails on SD.ARDID equals SDD.ARDID
                        join BP in _context.BusinessPartners on SD.CusID equals BP.ID
                        join I in _context.ItemMasterDatas on SDD.ItemID equals I.ID
                        join CUR in _context.Currency on SD.SaleCurrencyID equals CUR.ID
                        join B in _context.Branches on SD.BranchID equals B.ID
                        join U in _context.UserAccounts on SD.BranchID equals U.BranchID
                        join R in _context.ReceiptInformation on B.ID equals R.ID
                        join C in _context.Company on B.CompanyID equals C.ID
                        join S in _context.Series on SD.SeriesID equals S.ID
                        let pd = _context.PropertyDetails.FirstOrDefault(i => i.ProID == brand.ID && i.ItemID == I.ID)
                        let pdId = pd == null ? 0 : pd.Value
                        let cp = _context.ChildPreoperties.FirstOrDefault(i => i.ID == pdId)
                        group new { SD, SDD, BP, I, CUR, B, U, R, C, pd, pdId, cp, S } by SDD.ID into g
                        let data = g.FirstOrDefault()
                        let master = data.SD
                        let detail = data.SDD
                        let cu = data.CUR
                        let BP = data.BP
                        let B = data.B
                        let U = data.U
                        let R = data.R
                        let C = data.C
                        let S = data.S
                        let cp = data.cp
                        let I = data.I
                        select new PrintSaleHistory
                        {
                            //Master
                            ID = g.FirstOrDefault().SD.ARDID,
                            Invoice = master.InvoiceNumber,
                            PostingDate = master.PostingDate.ToString("dd-MMM-yyyy"),
                            DocumentDate = master.DocumentDate.ToString("dd-MM-yyyy"),
                            DueDate = master.DueDate.ToString("dd-MM-yyyy"),
                            CusName = BP.Name,
                            Phone = BP.Phone,
                            Address = BP.Address,
                            UserName = U.Username,
                            CusNo = BP.Code,
                            RefNo = master.RefNo,
                            Branch = B.Name,
                            Email = R.Email,
                            PreFix = S.PreFix,
                            Remarks = master.Remarks,
                            Title = R.Title,
                            //Detail
                            Brand = cp == null ? "" : cp.Name,
                            ItemCode = detail.ItemCode,
                            ItemNameKh = detail.ItemNameKH,
                            Qty = (double)detail.Qty,
                            Price = (double)detail.UnitPrice,
                            DiscountValue_Detail = (double)detail.DisValue,
                            UomName = detail.UomName,
                            Amount = detail.Total,
                            LocalCurrency = cu.Description,
                            Tel1 = R.Tel1,
                            Tel2 = R.Tel2,
                            Addresskh = R.Address,
                            AddressEng = R.EnglishDescription,
                            CompanyName = C.Name,
                            Logo = C.Logo,
                            SysCurrency = cu.Description,
                            Barcode = I.Barcode,
                            //Summary
                            DiscountValue = (double)master.DisValue,
                            DiscountRate = (double)master.DisRate,
                            VatValue = (double)master.VatValue,
                            TotalAmount = master.TotalAmount,
                            BalanceDue = master.BalanceDue,
                            SubTotalAfterDis = master.SubTotalAfterDis,
                            DPMRate = master.DPMRate,
                        }).ToList();
            if (!list.Any()) return NotFound();
            return new ViewAsPdf(list)
            {
                CustomSwitches = "--footer-left \"Powerby: KHMER EDI\" " + "--footer-center \"Page: [page] of [toPage]\" --footer-font-size \"9\""
            };
        }
        public IActionResult PrintCreditMemoIncomingPamyment(string id)
        {
            var invCM = _context.SaleCreditMemos.FirstOrDefault(s => s.InvoiceNo == id);
            var list = (from SC in _context.SaleCreditMemos.Where(m => m.SCMOID == invCM.SCMOID)
                        join SCD in _context.SaleCreditMemoDetails on SC.SCMOID equals SCD.SCMOID
                        join BP in _context.BusinessPartners on SC.CusID equals BP.ID
                        join I in _context.ItemMasterDatas on SCD.ItemID equals I.ID
                        join CUR in _context.Currency on SC.SaleCurrencyID equals CUR.ID
                        join B in _context.Branches on SC.BranchID equals B.ID
                        join U in _context.UserAccounts on SC.BranchID equals U.BranchID
                        group new { SC, SCD, BP, I, CUR, B, U } by SCD.SCMODID into g
                        let data = g.FirstOrDefault()
                        let master = data.SC
                        let detail = data.SCD
                        let cu = data.CUR
                        let BP = data.BP
                        let B = data.B
                        let U = data.U
                        select new PrintSaleHistory
                        {
                            //Master
                            ID = g.FirstOrDefault().SC.SCMOID,
                            Invoice = master.InvoiceNo,
                            PostingDate = master.PostingDate.ToString("dd-MM-yyyy"),
                            DocumentDate = master.DocumentDate.ToString("dd-MM-yyyy"),
                            DueDate = master.DueDate.ToString("dd-MM-yyyy"),
                            CusName = BP.Name,
                            Phone = BP.Phone,
                            Address = BP.Address,
                            UserName = U.Username,
                            CusNo = BP.Code,
                            RefNo = master.RefNo,
                            Branch = g.First().B.Name,
                            BaseOn = master.CopyKey,
                            //Detail
                            ItemCode = detail.ItemCode,
                            ItemNameKh = detail.ItemNameKH,
                            Qty = detail.Qty,
                            Price = detail.UnitPrice,
                            DiscountValue_Detail = detail.DisValue,
                            UomName = detail.UomName,
                            Amount = detail.Total,
                            LocalCurrency = cu.Description,
                            //Summary
                            DiscountValue = master.DisValue,
                            VatValue = master.VatValue,
                            Applied_Amount = master.AppliedAmount,
                            TotalAmount = master.TotalAmount
                        }).ToList();
            //list = null;
            if (!list.Any()) return NotFound();
            return new ViewAsPdf(list);
        }

        public IActionResult PrintIncomingCustomer(string id)
        {

            var invAR = _context.SaleARs.FirstOrDefault(s => s.InvoiceNo == id);
            var list = (from SAR in _context.SaleARs.Where(m => m.SARID == invAR.SARID)
                        join SARD in _context.SaleARDetails on SAR.SARID equals SARD.SARID
                        join BP in _context.BusinessPartners on SAR.CusID equals BP.ID
                        join I in _context.ItemMasterDatas on SARD.ItemID equals I.ID
                        join CUR in _context.Currency on SAR.SaleCurrencyID equals CUR.ID
                        join B in _context.Branches on SAR.BranchID equals B.ID
                        join U in _context.UserAccounts on SAR.BranchID equals U.BranchID
                        join RI in _context.ReceiptInformation on B.ID equals RI.ID
                        group new { SAR, SARD, BP, I, CUR, B, U, RI } by SARD.SARDID into g
                        let data = g.FirstOrDefault()
                        let master = data.SAR
                        let detail = data.SARD
                        let cu = data.CUR
                        let BP = data.BP
                        let B = data.B
                        let U = data.U
                        let RI = data.RI
                        select new PrintSaleHistory
                        {
                            //Master
                            ID = g.FirstOrDefault().SAR.SARID,
                            Invoice = master.InvoiceNo,
                            PostingDate = master.PostingDate.ToString("dd-MM-yyyy"),
                            DocumentDate = master.DocumentDate.ToString("dd-MM-yyyy"),
                            DueDate = master.DueDate.ToString("dd-MM-yyyy"),
                            CusName = BP.Name,
                            Phone = BP.Phone,
                            Address = BP.Address,
                            UserName = U.Username,
                            CusNo = BP.Code,
                            RefNo = master.RefNo,
                            Branch = B.Name,
                            KhmerDesc = RI.KhmerDescription,
                            EnglishDesc = RI.EnglishDescription,
                            //PhoneList = GetPhoneList(RI.Tel1, RI.Tel2),
                            Addresskh = RI.Address,
                            //Detail
                            ItemCode = detail.ItemCode,
                            ItemNameKh = detail.ItemNameKH,
                            Qty = detail.Qty,
                            Price = detail.UnitPrice,
                            DiscountValue_Detail = detail.DisValue,
                            UomName = detail.UomName,
                            Amount = detail.Total,
                            LocalCurrency = cu.Description,
                            //Summary
                            DiscountValue = master.DisValue,
                            VatValue = master.VatValue,
                            Applied_Amount = master.AppliedAmount,
                            TotalAmount = master.TotalAmount
                        }).ToList();
            if (!list.Any()) return NotFound();
            return new ViewAsPdf(list);
        }

        public IActionResult PrintInvoiceKSMS(int id)
        {
            var totalItem = _context.ReceiptDetail.Where(i => i.ReceiptID == id && !i.IsKsms).Count();
            var receipt = _context.Receipt.Where(i => i.ReceiptID == id).ToList();

            PrintInvKvms data = new();
            if (receipt.FirstOrDefault().VehicleID > 0)
            {
                data = (from rp in receipt
                        join automobile in _context.AutoMobiles on rp.VehicleID equals automobile.AutoMID
                        join vihecle in _context.AutoModels on automobile.ModelID equals vihecle.ModelID
                        join type in _context.AutoTypes on automobile.TypeID equals type.TypeID
                        join brand in _context.AutoBrands on automobile.BrandID equals brand.BrandID
                        join color in _context.AutoColors on automobile.ColorID equals color.ColorID
                        join cus in _context.BusinessPartners on rp.CustomerID equals cus.ID
                        join pl in _context.PriceLists on rp.PriceListID equals pl.ID
                        join user in _context.UserAccounts on rp.UserOrderID equals user.ID
                        join com in _context.ReceiptInformation on rp.BranchID equals com.BranchID
                        select new PrintInvKvms
                        {
                            QNo = rp.ReceiptNo,
                            Code = cus.Code,
                            Name = cus.Name,
                            PriceListName = pl.Name,
                            Phone = cus.Phone,
                            Email = cus.Email,
                            Address = cus.Address,
                            Plate = automobile.Plate,
                            Frame = automobile.Frame,
                            Engine = automobile.Engine,
                            TypeName = type.TypeName,
                            BrandName = brand.BrandName,
                            ModelName = vihecle.ModelName,
                            Year = automobile.Year,
                            ColorName = color.ColorName,
                            PrintDetailQuotes = (from rpd in _context.ReceiptDetail.Where(i => i.ReceiptID == rp.ReceiptID && !i.IsKsms)
                                                 join item in _context.ItemMasterDatas on rpd.ItemID equals item.ID
                                                 join uom in _context.UnitofMeasures on rpd.UomID equals uom.ID
                                                 let cur = _context.Currency.FirstOrDefault(i => i.ID == pl.CurrencyID) ?? new Currency()
                                                 select new PrintDetailQuotes
                                                 {
                                                     Code = item.Code,
                                                     EnglishName = item.EnglishName,
                                                     KhmerName = item.KhmerName,
                                                     Qty = rpd.Qty.ToString(),
                                                     UoM = uom.Name,
                                                     UnitPrice = string.Format("{0:#,0.000}", rpd.UnitPrice),
                                                     DisRate = rpd.DiscountRate + " %",
                                                     TypeDis = rpd.TypeDis,
                                                     Currency = cur.Description,
                                                     Total = string.Format("{0:#,0.000}", rpd.Total)
                                                 }).ToList(),
                            Username = user.Username,
                            //Summary
                            Count = totalItem.ToString(),
                            Subtotal = string.Format("{0:#,0.000}", rp.Sub_Total),
                            DisRate = rp.DiscountRate.ToString() + " %",
                            TaxValue = string.Format("{0:#,0.000}", rp.TaxValue),
                            GrandTotal = string.Format("{0:#,0.000}", rp.GrandTotal),
                            AppliedAmount = string.Format("{0:#,0.000}", rp.AppliedAmount),
                            //BalanceDue = string.Format("{0:#,0.000}", rp.),
                            //Company Info
                            ComBName = com.Title,
                            ComBAddress = com.Address,
                            ComBPhone = com.Tel1 + " / " + com.Tel2
                        }).FirstOrDefault();
            }
            else
            {
                data = (from rp in receipt
                        join cus in _context.BusinessPartners on rp.CustomerID equals cus.ID
                        join pl in _context.PriceLists on rp.PriceListID equals pl.ID
                        join user in _context.UserAccounts on rp.UserOrderID equals user.ID
                        join com in _context.ReceiptInformation on rp.BranchID equals com.BranchID
                        select new PrintInvKvms
                        {
                            QNo = rp.ReceiptNo,
                            Code = cus.Code,
                            Name = cus.Name,
                            PriceListName = pl.Name,
                            Phone = cus.Phone,
                            Email = cus.Email,
                            Address = cus.Address,
                            PrintDetailQuotes = (from rpd in _context.ReceiptDetail.Where(i => i.ReceiptID == rp.ReceiptID && !i.IsKsms)
                                                 join item in _context.ItemMasterDatas on rpd.ItemID equals item.ID
                                                 join uom in _context.UnitofMeasures on rpd.UomID equals uom.ID
                                                 let cur = _context.Currency.FirstOrDefault(i => i.ID == pl.CurrencyID) ?? new Currency()
                                                 select new PrintDetailQuotes
                                                 {
                                                     Code = item.Code,
                                                     EnglishName = item.EnglishName,
                                                     KhmerName = item.KhmerName,
                                                     Qty = rpd.Qty.ToString(),
                                                     UoM = uom.Name,
                                                     UnitPrice = string.Format("{0:#,0.000}", rpd.UnitPrice),
                                                     DisRate = rpd.DiscountRate + " %",
                                                     TypeDis = rpd.TypeDis,
                                                     Currency = cur.Description,
                                                     Total = string.Format("{0:#,0.000}", rpd.Total)
                                                 }).ToList(),
                            Username = user.Username,
                            //Summary
                            Count = totalItem.ToString(),
                            Subtotal = string.Format("{0:#,0.000}", rp.Sub_Total),
                            DisRate = rp.DiscountRate.ToString() + " %",
                            TaxValue = string.Format("{0:#,0.000}", rp.TaxValue),
                            GrandTotal = string.Format("{0:#,0.000}", rp.GrandTotal),
                            AppliedAmount = string.Format("{0:#,0.000}", rp.AppliedAmount),
                            //BalanceDue = string.Format("{0:#,0.000}", rp.),
                            //Company Info
                            ComBName = com.Title,
                            ComBAddress = com.Address,
                            ComBPhone = com.Tel1 + " / " + com.Tel2
                        }).FirstOrDefault();
            }
            return new ViewAsPdf(data);
        }
        //===============comment getsalehistory=======
        #region
        private List<PrintSaleHistory> GetSaleARData(int id)
        {
            var list = (from SAR in _context.SaleARs.Where(m => m.SARID == id)
                        join SARD in _context.SaleARDetails on SAR.SARID equals SARD.SARID
                        join BP in _context.BusinessPartners on SAR.CusID equals BP.ID
                        join I in _context.ItemMasterDatas on SARD.ItemID equals I.ID
                        join CUR in _context.Currency on SAR.SaleCurrencyID equals CUR.ID
                        join Lcur in _context.Currency on SAR.LocalCurID equals Lcur.ID
                        join B in _context.Branches on SAR.BranchID equals B.ID
                        join U in _context.UserAccounts on SAR.UserID equals U.ID
                        join RI in _context.ReceiptInformation on B.ID equals RI.BranchID
                        join C in _context.Company on SAR.CompanyID equals C.ID
                        join S in _context.Series on SAR.SeriesID equals S.ID
                        join Ex in _context.ExchangeRates on CUR.ID equals Ex.CurrencyID
                        group new { SAR, SARD, BP, I, CUR, B, U, RI, C, S, Lcur, Ex } by SARD.SARDID into g
                        let data = g.FirstOrDefault()
                        let master = data.SAR
                        let detail = data.SARD
                        let cu = data.CUR
                        let I = data.I
                        let BP = data.BP
                        let B = data.B
                        let U = data.U
                        let RI = data.RI
                        let C = data.C
                        let S = data.S
                        let Lcur = data.Lcur
                        let SAR = data.SAR
                        let SARD = data.SARD
                        let ex = data.Ex
                        let dlar = _context.SaleDeliveries.FirstOrDefault(i => data.SAR.CopyType == SaleCopyType.Delivery && i.SDID == data.SAR.BaseOnID) ?? new SaleDelivery()
                        let orderar = _context.SaleOrders.FirstOrDefault(i => data.SAR.CopyType == SaleCopyType.Order && i.SOID == data.SAR.BaseOnID) ?? new SaleOrder()
                        let quotear = _context.SaleQuotes.FirstOrDefault(i => data.SAR.CopyType == SaleCopyType.Quotation && i.SQID == data.SAR.BaseOnID) ?? new SaleQuote()
                        let dlOrder = data.SAR.CopyType == SaleCopyType.Delivery ? _context.SaleOrders.FirstOrDefault(i => dlar.CopyType == SaleCopyType.Order && i.SOID == dlar.BaseOnID) ?? new SaleOrder() : new SaleOrder()
                        let dlQuote = data.SAR.CopyType == SaleCopyType.Delivery ? _context.SaleQuotes.FirstOrDefault(i => dlOrder.CopyType == SaleCopyType.Quotation && i.SQID == dlOrder.BaseOnID) ?? new SaleQuote() : new SaleQuote()
                        let orderQuote = data.SAR.CopyType == SaleCopyType.Order ? _context.SaleQuotes.FirstOrDefault(i => orderar.CopyType == SaleCopyType.Quotation && i.SQID == orderar.BaseOnID) ?? new SaleQuote() : new SaleQuote()
                        let paymterm = _context.PaymentTerms.Where(s => s.ID == BP.PaymentTermsID).FirstOrDefault() ?? new Models.Services.HumanResources.PaymentTerms()
                        select new PrintSaleHistory
                        {
                            //Master
                            ID = data.SAR.SARID,
                            Invoice = master.InvoiceNumber,
                            PostingDate = master.PostingDate.ToString("dd-MM-yyyy"),
                            DocumentDate = master.DocumentDate.ToString("dd-MM-yyyy"),
                            DueDate = master.DueDate.ToString("dd-MM-yyyy"),
                            CusName = BP.Name,
                            Phone = BP.Phone,
                            Address = BP.Address,
                            UserName = U.Username,
                            CusNo = BP.Code,
                            RefNo = master.RefNo,
                            Branch = RI.Title,
                            KhmerDesc = RI.KhmerDescription,
                            EnglishDesc = RI.EnglishDescription,
                            Addresskh = RI.Address,
                            AddressEng = RI.EnglishDescription,
                            PreFix = S.PreFix,
                            Remarks = master.Remarks,
                            DSNumber = dlar.InvoiceNumber,
                            OrderNumber = data.SAR.CopyType == SaleCopyType.Delivery ? dlOrder.InvoiceNumber : orderar.InvoiceNumber,
                            QSNumber = data.SAR.CopyType == SaleCopyType.Delivery ? dlQuote.InvoiceNumber : data.SAR.CopyType == SaleCopyType.Order ? orderQuote.InvoiceNumber : quotear.InvoiceNumber,

                            //Detail
                            ItemCode = detail.ItemCode,
                            ItemNameKh = detail.ItemNameKH,
                            Qty = detail.Qty,
                            Price = detail.UnitPrice,
                            DiscountValue_Detail = detail.DisValue,
                            DiscountRate_Detail = detail.DisRate,
                            UomName = detail.UomName,
                            Amount = detail.TotalWTax,
                            LocalCurrency = Lcur.Description,
                            SysCurrency = cu.Description,
                            CompanyName = B.Name,
                            Logo = C.Logo,
                            Tel1 = RI.Tel1,
                            Tel2 = RI.Tel2,
                            //Brand = cp == null ? "" : cp.Name,
                            //Summary
                            Sub_Total = (double)master.SubTotalAfterDis,
                            DiscountValue = master.DisValue,
                            DiscountRate = master.DisRate,
                            TaxValue = (decimal)master.VatValue,
                            TaxRate = (decimal)master.VatRate,
                            Applied_Amount = master.AppliedAmount,
                            TotalAmount = master.TotalAmount,
                            TotalAmountSys = cu.Description == "KHR" ? master.TotalAmountSys : master.TotalAmountSys * master.LocalSetRate,
                            Barcode = I.Barcode,
                            ExchangeRate = ex.DisplayRate, //master.ExchangeRate,
                            LocalSetRate = SAR.LocalSetRate,
                            PriceList = cu.Description,
                            LabelUSA = cu.Description == "KHR" ? "៛" : cu.Description == "USD" ? "$" : cu.Description,
                            LabelReal = Lcur.Description == "KHR" ? "៛" : Lcur.Description == "USD" ? "$" : Lcur.Description,
                            Sub_totalAfterdis = master.SubTotalAfterDis,
                            Balance_Due_Local = 0,
                            Balance_Due_Sys = 0,
                            BaseOn = master.BasedCopyKeys,
                            BPBrandName = "",
                            Debit = master.TotalAmount - master.AppliedAmount,
                            DebitSys = cu.Description == "KHR" ? master.TotalAmount - master.AppliedAmount : (master.TotalAmount - master.AppliedAmount) * master.LocalSetRate,
                            Paymenterm = paymterm.Days,
                            VatNumber = BP.VatNumber.ToString(),
                            Email = BP.Email,
                        }).ToList();
            return list;
        }
        #endregion
        #region
        private List<PrintSaleHistory> GetSaleDelivery(int id)
        {
            var brand = _context.Property.FirstOrDefault(i => i.Name == "Brand" && i.Active && i.CompanyID == GetCompany().ID) ?? new Property();
            var list = (from SD in _context.SaleDeliveries.Where(m => m.SDID == id)
                        join SDD in _context.SaleDeliveryDetails on SD.SDID equals SDD.SDID
                        join BP in _context.BusinessPartners on SD.CusID equals BP.ID
                        join I in _context.ItemMasterDatas on SDD.ItemID equals I.ID
                        join CUR in _context.Currency on SD.SaleCurrencyID equals CUR.ID
                        join B in _context.Branches on SD.BranchID equals B.ID
                        join U in _context.UserAccounts on SD.UserID equals U.ID
                        join R in _context.ReceiptInformation on B.ID equals R.BranchID
                        join C in _context.Company on SD.CompanyID equals C.ID
                        join S in _context.Series on SD.SeriesID equals S.ID
                        let pd = _context.PropertyDetails.FirstOrDefault(i => i.ProID == brand.ID && i.ItemID == I.ID)
                        let pdId = pd == null ? 0 : pd.Value
                        let cp = _context.ChildPreoperties.FirstOrDefault(i => i.ID == pdId)
                        group new { SD, SDD, BP, I, CUR, B, U, R, C, pd, pdId, cp, S } by SDD.SDDID into g
                        let data = g.FirstOrDefault()
                        let master = data.SD
                        let detail = data.SDD
                        let cu = data.CUR
                        let BP = data.BP
                        let B = data.B
                        let U = data.U
                        let R = data.R
                        let C = data.C
                        let S = data.S
                        let cp = data.cp
                        select new PrintSaleHistory
                        {
                            //Master
                            ID = g.FirstOrDefault().SD.SDID,
                            Invoice = master.InvoiceNumber,
                            PostingDate = master.PostingDate.ToString("dd-MM-yyyy"),
                            DocumentDate = master.DocumentDate.ToString("dd-MM-yyyy"),
                            DueDate = master.DueDate.ToString("dd-MM-yyyy"),
                            CusName = BP.Name,
                            Phone = BP.Phone,
                            Address = BP.Address,
                            UserName = U.Username,
                            CusNo = BP.Code,
                            RefNo = master.RefNo,
                            Branch = B.Name,
                            PreFix = S.PreFix,
                            Remarks = master.Remarks,
                            //Detail
                            Brand = cp == null ? "" : cp.Name,
                            ItemCode = detail.ItemCode,
                            ItemNameKh = detail.ItemNameKH,
                            Qty = detail.Qty,
                            Price = detail.UnitPrice,
                            DiscountValue_Detail = detail.DisValue,
                            UomName = detail.UomName,
                            Amount = detail.Total,
                            LocalCurrency = cu.Description,
                            Tel1 = R.Tel1,
                            Tel2 = R.Tel2,
                            Addresskh = R.Address,
                            AddressEng = R.EnglishDescription,
                            CompanyName = C.Name,
                            Logo = C.Logo,
                            //Summary
                            DiscountValue = master.DisValue,
                            VatValue = master.VatValue,
                            TotalAmount = master.TotalAmount
                        }).ToList();
            return list;
        }
        #endregion

        private List<PrintSaleHistory> GetSaleQoute(int id)
        {
            var brand = _context.Property.FirstOrDefault(i => i.Name == "Brand" && i.Active && i.CompanyID == GetCompany().ID) ?? new Property();
            var list = (from SQ in _context.SaleQuotes.Where(m => m.SQID == id)
                        join SQD in _context.SaleQuoteDetails on SQ.SQID equals SQD.SQID
                        join BP in _context.BusinessPartners on SQ.CusID equals BP.ID
                        join I in _context.ItemMasterDatas on SQD.ItemID equals I.ID
                        join CUR in _context.Currency on SQ.SaleCurrencyID equals CUR.ID
                        join B in _context.Branches on SQ.BranchID equals B.ID
                        join U in _context.UserAccounts on SQ.UserID equals U.ID
                        join R in _context.ReceiptInformation on B.ID equals R.BranchID
                        join C in _context.Company on SQ.CompanyID equals C.ID
                        join S in _context.Series on SQ.SeriesID equals S.ID
                        let pd = _context.PropertyDetails.FirstOrDefault(i => i.ProID == brand.ID && i.ItemID == I.ID)
                        let pdId = pd == null ? 0 : pd.Value
                        let cp = _context.ChildPreoperties.FirstOrDefault(i => i.ID == pdId)

                        group new { SQ, SQD, BP, I, CUR, B, U, R, C, pd, pdId, cp, S } by SQD.SQDID into g
                        let data = g.FirstOrDefault()
                        let master = data.SQ
                        let detail = data.SQD
                        let cu = data.CUR
                        let BP = data.BP
                        let B = data.B
                        let U = data.U
                        let R = data.R
                        let C = data.C
                        let cp = data.cp
                        let S = data.S
                        let paymterm = _context.PaymentTerms.Where(s => s.ID == BP.PaymentTermsID).FirstOrDefault() ?? new PaymentTerms()
                        select new PrintSaleHistory
                        {
                            //Master
                            ID = g.FirstOrDefault().SQ.SQID,
                            Invoice = master.InvoiceNumber,
                            PostingDate = master.PostingDate.ToString("dd-MM-yyyy"),
                            DocumentDate = master.DocumentDate.ToString("dd-MM-yyyy"),
                            DueDate = master.ValidUntilDate.ToString("dd-MM-yyyy"),
                            CusName = BP.Name,
                            StoreName = BP.StoreName,
                            Phone = BP.Phone,
                            Address = BP.Address,
                            UserName = U.Username,
                            CusNo = BP.Code,
                            Email = BP.Email,
                            RefNo = master.RefNo,
                            Branch = g.First().B.Name,
                            PreFix = S.PreFix,
                            Remarks = master.Remarks,
                            //Detail
                            Brand = cp == null ? "" : cp.Name,
                            ItemCode = detail.ItemCode,
                            ItemNameKh = detail.ItemNameKH,
                            Qty = detail.Qty,
                            Price = detail.UnitPrice,
                            DiscountRate = detail.DisRate,
                            DiscountValue_Detail = detail.DisValue,
                            UomName = detail.UomName,
                            //Amount = detail.Total,
                            Amount = detail.Qty * detail.UnitPrice,
                            Sub_totalAfterdis = (decimal)detail.Total,
                            LocalCurrency = cu.Description,
                            Tel1 = R.Tel1,
                            Tel2 = R.Tel2,
                            Addresskh = R.Address,
                            AddressEng = R.EnglishDescription,
                            CompanyName = C.Name,
                            Logo = C.Logo,
                            //Summary
                            DiscountValue = master.DisValue,
                            VatValue = master.VatValue,
                            VatNumber = BP.VatNumber.ToString() == "0" ? "" : BP.VatNumber.ToString(),
                            TotalAmount = master.TotalAmount,
                            LabelUSA = cu.Description == "KHR" ? "៛" : cu.Description == "USD" ? "$" : cu.Description,
                            Paymenterm = paymterm.Days,
                        }).ToList();
            return list;
        }
        //Print PurchaseRequest
        public IActionResult PrintPurchasRequest(int PurchaseID)
        {
            var brand = _context.Property.FirstOrDefault(i => i.Name == "Brand" && i.Active && i.CompanyID == GetCompany().ID) ?? new Property();
            var list = (from PO in _context.PurchaseRequests.Where(m => m.ID == PurchaseID && m.CompanyID == GetCompany().ID)
                        join POD in _context.PurchaseRequestDetails on PO.ID equals POD.PurchaseRequestID
                        join BP in _context.BusinessPartners on PO.RequesterID equals BP.ID
                        join U in _context.UserAccounts on PO.UserID equals U.ID
                        join B in _context.Branches on PO.BranchID equals B.ID
                        join E in _context.Employees on U.EmployeeID equals E.ID
                        join C in _context.Currency on PO.PurCurrencyID equals C.ID
                        join R in _context.ReceiptInformation on B.ID equals R.BranchID
                        join D in _context.Company on PO.CompanyID equals D.ID
                        join I in _context.ItemMasterDatas on POD.ItemID equals I.ID
                        join N in _context.UnitofMeasures on POD.UomID equals N.ID
                        join S in _context.Series on PO.SeriesID equals S.ID
                        let pd = _context.PropertyDetails.FirstOrDefault(i => i.ProID == brand.ID && i.ItemID == I.ID)
                        let pdId = pd == null ? 0 : pd.Value
                        let cp = _context.ChildPreoperties.FirstOrDefault(i => i.ID == pdId)

                        select new PrintPurchaseAP
                        {
                            AddressEng = R.EnglishDescription,
                            Brand = cp == null ? "" : cp.Name,
                            PreFix = S.PreFix,
                            CusCode = BP.Code,
                            Invoice = PO.Number,
                            ExchangeRate = PO.PurRate,
                            Balance_Due_Sys = PO.BalanceDueSys,
                            Applied_Amount = PO.AppliedAmount,
                            PostingDate = PO.PostingDate.ToString("dd-MM-yyyy"),
                            DocumentDate = PO.DocumentDate.ToString("dd-MM-yyyy"),
                            DueDate = PO.DeliveryDate.ToString("dd-MM-yyyy"),
                            DiscountValue = PO.DiscountValue,
                            DiscountValue_Detail = POD.DiscountValue,
                            TaxValue = PO.TaxValue,
                            CompanyName = D.Name,
                            Addresskh = R.Address,
                            TaxRate = PO.TaxRate,
                            VendorName = BP.Name,
                            Tel1 = R.Tel1,
                            Tel2 = R.Tel2,
                            Phone = BP.Phone,
                            Address = BP.Address,
                            UserName = E.Name,
                            SQN = PO.ReffNo,
                            VendorNo = PO.Number,
                            Logo = D.Logo,
                            LocalCurrency = C.Description,
                            Total = POD.Total,
                            Sub_Total = PO.BalanceDue,
                            KhmerName = I.KhmerName,
                            Code = I.Code,
                            Price = POD.PurchasPrice,
                            UomName = N.Name,
                            Qty = POD.Qty,
                            Remark = PO.Remark
                        }).ToList();
            if (!list.Any()) return NotFound();
            return new ViewAsPdf(list)
            {
                CustomSwitches = "--footer-left \"Powerby: KHMER EDI\" " + "--footer-center \"Page: [page] of [toPage]\" --footer-font-size \"9\""
            };
        }

        //Print PurchaseQuotation
        public IActionResult PrintPurchasQuotation(int PurchaseID)
        {
            var brand = _context.Property.FirstOrDefault(i => i.Name == "Brand" && i.Active && i.CompanyID == GetCompany().ID) ?? new Property();
            var list = (from PO in _context.PurchaseQuotations.Where(m => m.ID == PurchaseID && m.CompanyID == GetCompany().ID)
                        join POD in _context.PurchaseQuotationDetails on PO.ID equals POD.PurchaseQuotationID
                        join BP in _context.BusinessPartners on PO.VendorID equals BP.ID
                        join U in _context.UserAccounts on PO.UserID equals U.ID
                        join B in _context.Branches on PO.BranchID equals B.ID
                        join E in _context.Employees on U.EmployeeID equals E.ID
                        join C in _context.Currency on PO.PurCurrencyID equals C.ID
                        join R in _context.ReceiptInformation on B.ID equals R.BranchID
                        join D in _context.Company on PO.CompanyID equals D.ID
                        join I in _context.ItemMasterDatas on POD.ItemID equals I.ID
                        join N in _context.UnitofMeasures on POD.UomID equals N.ID
                        join S in _context.Series on PO.SeriesID equals S.ID
                        let pd = _context.PropertyDetails.FirstOrDefault(i => i.ProID == brand.ID && i.ItemID == I.ID)
                        let pdId = pd == null ? 0 : pd.Value
                        let cp = _context.ChildPreoperties.FirstOrDefault(i => i.ID == pdId)

                        select new PrintPurchaseAP
                        {
                            AddressEng = R.EnglishDescription,
                            Brand = cp == null ? "" : cp.Name,
                            PreFix = S.PreFix,
                            CusCode = BP.Code,
                            Invoice = PO.Number,
                            ExchangeRate = PO.PurRate,
                            Balance_Due_Sys = PO.BalanceDueSys,
                            Applied_Amount = PO.AppliedAmount,
                            PostingDate = PO.PostingDate.ToString("dd-MM-yyyy"),
                            DocumentDate = PO.DocumentDate.ToString("dd-MM-yyyy"),
                            DueDate = PO.DeliveryDate.ToString("dd-MM-yyyy"),
                            DiscountValue = PO.DiscountValue,
                            DiscountValue_Detail = POD.DiscountValue,
                            TaxValue = PO.TaxValue,
                            CompanyName = D.Name,
                            Addresskh = R.Address,
                            TaxRate = PO.TaxRate,
                            TypeDis = PO.TypeDis,
                            VendorName = BP.Name,
                            Tel1 = R.Tel1,
                            Tel2 = R.Tel2,
                            Phone = BP.Phone,
                            Address = BP.Address,
                            UserName = E.Name,
                            SQN = PO.ReffNo,
                            VendorNo = PO.Number,
                            Logo = D.Logo,
                            LocalCurrency = C.Description,
                            Total = POD.Total,
                            Sub_Total = PO.BalanceDue,
                            KhmerName = I.KhmerName,
                            Code = I.Code,
                            Price = POD.PurchasPrice,
                            UomName = N.Name,
                            Qty = POD.Qty,
                            Remark = PO.Remark
                        }).ToList();
            if (!list.Any()) return NotFound();
            return new ViewAsPdf(list)
            {
                CustomSwitches = "--footer-left \"Powerby: KHMER EDI\" " + "--footer-center \"Page: [page] of [toPage]\" --footer-font-size \"9\""
            };
        }

        public IActionResult SummarySheet(string DateFrom, string DateTo, int Customer, int WarehouseUser, int Delivery, bool Check)
        {
            List<SaleAR> saleARs = new();
            if (DateFrom != null && DateTo != null && Customer == 0 && WarehouseUser == 0 && Delivery == 0 && Check == false)
            {
                saleARs = _context.SaleARs.Where(x => x.PostingDate >= Convert.ToDateTime(DateFrom) && x.PostingDate <= Convert.ToDateTime(DateTo)).ToList();
            }
            else if (DateFrom != null && DateTo != null && Customer != 0 && WarehouseUser == 0 && Delivery == 0 && Check == false)
            {
                saleARs = _context.SaleARs.Where(x => x.PostingDate >= Convert.ToDateTime(DateFrom) && x.PostingDate <= Convert.ToDateTime(DateTo) && x.CusID == Customer).ToList();
            }
            else if (DateFrom != null && DateTo != null && Customer != 0 && WarehouseUser != 0 && Delivery == 0 && Check == false)
            {
                saleARs = _context.SaleARs.Where(x => x.PostingDate >= Convert.ToDateTime(DateFrom) && x.PostingDate <= Convert.ToDateTime(DateTo) && x.CusID == Customer && x.WarehouseID == WarehouseUser).ToList();
            }
            else if (DateFrom != null && DateTo != null && Customer != 0 && WarehouseUser != 0 && Delivery != 0 && Check == false)
            {
                saleARs = _context.SaleARs.Where(x => x.PostingDate >= Convert.ToDateTime(DateFrom) && x.PostingDate <= Convert.ToDateTime(DateTo) && x.CusID == Customer && x.WarehouseID == WarehouseUser && x.ShippedBy == Delivery).ToList();
            }
            else if (DateFrom != null && DateTo != null && Customer == 0 && WarehouseUser != 0 && Delivery == 0 && Check == false)
            {
                saleARs = _context.SaleARs.Where(x => x.PostingDate >= Convert.ToDateTime(DateFrom) && x.PostingDate <= Convert.ToDateTime(DateTo) && x.WarehouseID == WarehouseUser).ToList();
            }
            else if (DateFrom != null && DateTo != null && Customer == 0 && WarehouseUser == 0 && Delivery != 0 && Check == false)
            {
                saleARs = _context.SaleARs.Where(x => x.PostingDate >= Convert.ToDateTime(DateFrom) && x.PostingDate <= Convert.ToDateTime(DateTo) && x.ShippedBy == Delivery).ToList();
            }
            else if (DateFrom != null && DateTo != null && Customer == 0 && WarehouseUser != 0 && Delivery != 0 && Check == false)
            {
                saleARs = _context.SaleARs.Where(x => x.PostingDate >= Convert.ToDateTime(DateFrom) && x.PostingDate <= Convert.ToDateTime(DateTo) && x.CusID == Customer && x.WarehouseID == WarehouseUser && x.ShippedBy == Delivery).ToList();
            }

            var list = (from sa in saleARs
                        join BP in _context.BusinessPartners.Where(bp => bp.Type.ToLower() == "customer" && !bp.Delete)
                        on sa.CusID equals BP.ID
                        join B in _context.Branches on sa.BranchID equals B.ID
                        join R in _context.ReceiptInformation on B.ID equals R.ID
                        join C in _context.Company on sa.CompanyID equals C.ID
                        join w in _context.Warehouses.Where(wh => !wh.Delete) on sa.WarehouseID equals w.ID
                        join cu in _context.Currency on sa.SaleCurrencyID equals cu.ID
                        join docType in _context.DocumentTypes on sa.DocTypeID equals docType.ID
                        join s in _context.Series on sa.SeriesID equals s.ID
                        // join em in _context.Employees on typeof(T).GetProperty("ShippedBy").GetValue(sa) equals em.ID
                        let em = _context.Employees.Where(x => x.ID == sa.ShippedBy).FirstOrDefault() ?? new Employee()
                        select new PrintSaleHistory
                        {
                            ID = sa.SARID,
                            CusName = BP.Name,
                            Phone = BP.Phone,
                            Address = BP.Address,
                            UserName = HttpContext.User.Identity.Name,
                            // BalanceDueLC = string.Format("{0} {1:N3}", cu.Description, sa.TotalAmount),
                            // BalanceDueSC = string.Format("{0} {1:N3}", cu.Description,sa.TotalAmountSys),
                            //ExchangeRate = string.Format("{0:N3}", 1 / Convert.ToDecimal(GetValue(sa, "ExchangeRate"))),
                            // ExchangeRate = string.Format("{0:N2}", sa.ExchangeRate),
                            Branch = B.Name,
                            Tel1 = R.Tel1,
                            Tel2 = R.Tel2,
                            Addresskh = R.Address,
                            CompanyName = C.Name,
                            Logo = C.Logo,
                            PostingDate = sa.PostingDate.ToString("MM-dd-yyyy"),
                            DocumentDate = sa.DocumentDate.ToString("MM-dd-yyyy"),
                            DueDate = sa.DueDate.ToString("MM-dd-yyyy"),
                            TotalAmount = sa.TotalAmount,
                            Invoice = sa.InvoiceNo,
                            Remarks = sa.Remarks,
                            SysCurrency = cu.Description,
                        }).ToList();
            if (!list.Any()) return NotFound();
            return new ViewAsPdf(list)
            {
                CustomSwitches = "--footer-left \"Powerby: KHMER EDI\" " + "--footer-center \"Page: [page] of [toPage]\" --footer-font-size \"9\""
            };
        }
        public List<PrintSaleHistory> GetSaleAREditable(int ID)
        {
            var list = (from PA in _context.SaleAREdites.Where(m => m.SARID == ID && m.CompanyID == GetCompany().ID)
                        join PAD in _context.SaleAREditeDetails on PA.SARID equals PAD.SARID
                        join BP in _context.BusinessPartners on PA.CusID equals BP.ID
                        join U in _context.UserAccounts on PA.UserID equals U.ID
                        join B in _context.Branches on PA.BranchID equals B.ID
                        join R in _context.ReceiptInformation on B.ID equals R.ID
                        join E in _context.Employees on U.EmployeeID equals E.ID
                        join C in _context.Currency on PA.SaleCurrencyID equals C.ID
                        join D in _context.Company on B.CompanyID equals D.ID
                        join I in _context.ItemMasterDatas on PAD.ItemID equals I.ID
                        join N in _context.UnitofMeasures on PAD.UomID equals N.ID
                        join S in _context.Series on PA.SeriesID equals S.ID
                        select new PrintSaleHistory
                        {
                            AddressEng = R.EnglishDescription,
                            PreFix = S.PreFix,
                            //Brand = cp == null ? "" : cp.Name,
                            CusNo = BP.Code,
                            Invoice = PA.InvoiceNo,
                            ExchangeRate = PA.ExchangeRate,
                            Balance_Due_Sys = PA.TotalAmountSys,
                            Applied_Amount = PA.AppliedAmount,
                            PostingDate = PA.PostingDate.ToString("dd-MM-yyyy"),
                            DocumentDate = PA.DocumentDate.ToString("dd-MM-yyyy"),
                            //DueDate = PA..ToString("dd-MM-yyyy"),
                            Sub_Total = PA.TotalAmount,
                            DiscountValue = PA.DisValue,
                            DiscountRate = PA.DisRate,
                            TaxValue = Convert.ToDecimal(PA.VatValue),
                            CompanyName = D.Name,
                            Addresskh = R.Address,
                            TaxRate = Convert.ToDecimal(PA.VatRate),
                            TypeDis = PA.TypeDis,
                            CusName = BP.Name,
                            Tel1 = R.Tel1,
                            Tel2 = R.Tel2,
                            Phone = BP.Phone,
                            Address = BP.Address,
                            UserName = E.Name,
                            RefNo = PA.RefNo,
                            //c = PA.Number,
                            Logo = D.Logo,
                            LocalCurrency = C.Description,
                            //Deatai
                            Total = PAD.Total,
                            ItemNameKh = I.KhmerName,
                            ItemCode = I.Code,
                            Price = PAD.UnitPrice,
                            UomName = N.Name,
                            Qty = PAD.Qty,
                            Remarks = PA.Remarks
                        }).ToList();
            return list;
        }

    }
}
