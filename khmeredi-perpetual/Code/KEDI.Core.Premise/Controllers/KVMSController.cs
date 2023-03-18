using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using CKBS.AppContext;
using Rotativa.AspNetCore;
using CKBS.Models.Services.Responsitory;
using CKBS.Models.Services.HumanResources;
using KEDI.Core.Models.Validation;
using CKBS.Models.Services.POS.Template;
using CKBS.Models.ServicesClass.KAMSService;
using CKBS.Models.Services.POS.service;
using CKBS.Models.Services.Banking;
using CKBS.Models.Services.POS;
using CKBS.Models.Services.POS.KVMS;
using CKBS.Models.Services.Administrator.Tables;
using CKBS.Models.Services.Administrator.General;
using CKBS.Models.Services.Inventory.PriceList;
using CKBS.Models.Services.Administrator.SystemInitialization;
using KEDI.Core.Premise.Authorization;

namespace CKBS.Controllers
{
    [Privilege]
    public class KVMSController : Controller
    {
        private readonly DataContext _context;
        private readonly IKVMS _ikvms;
        private readonly IActionContextAccessor _accessor;

        public KVMSController(IKVMS pos, DataContext context, IActionContextAccessor accessor)
        {
            _ikvms = pos;
            _context = context;
            _accessor = accessor;
        }

        [Privilege("A044")]
        public IActionResult KVMS()
        {
            int branchid = Convert.ToInt32(@User.FindFirst("BranchID").Value);
            var ValidCustomers = _context.BusinessPartners.Where(w => !w.Delete);
            var check_setting = _context.GeneralSettings.Where(w => w.BranchID == branchid && ValidCustomers.Any(c => c.ID == w.CustomerID));

            ViewData["SelType"] = new SelectList(_context.AutoTypes.Where(c => c.Active), "TypeID", "TypeName");
            ViewData["SelBrand"] = new SelectList(_context.AutoBrands.Where(c => c.Active), "BrandID", "BrandName");
            ViewData["SelModel"] = new SelectList(_context.AutoModels.Where(c => c.Active), "ModelID", "ModelName");
            ViewData["SelColor"] = new SelectList(_context.AutoColors.Where(c => c.Active), "ColorID", "ColorName");

            ViewData["PlQcus"] = new SelectList(_context.PriceLists, "ID", "Name");

            BusinessPartner _partner = new BusinessPartner();
            _partner.AutoMobile = new List<AutoMobile>();
            ViewBag.Master_bus = _partner;

            if (check_setting.Count() > 0)
            {
                var group = _ikvms.GetGroup1s;
                var pos = new POSModel
                {
                    ItemGroup1s = _ikvms.GetGroup1s.ToList(),
                    Setting = GetSettingViewModel("/KVMS/KVMS")
                };
                ViewBag.DefaultCusName = _context.BusinessPartners.FirstOrDefault(c => c.ID == pos.Setting.Setting.CustomerID).Name;
                return View(pos);
            }
            else
            {
                ViewBag.KVMS = "highlight";
                return View("GeneralSetting", GetSettingViewModel("/KVMS/KVMS"));
            }
        }

        //VMC Edition
        public IActionResult GetAutoType()
        {
            return Ok(_context.AutoTypes);
        }

        [HttpPost]
        public IActionResult UpdateAutoType(IEnumerable<AutoType> data)
        {
            var items = data.GroupBy(s => s.TypeName.ToLower().Replace(" ", ""))
                            .Where(g => g.Count() > 1)
                            .Select(g => g.Key);

            ModelMessage errors = new ModelMessage(ModelState);
            if (items.Count() > 0)
            {
                foreach (var error in items)
                {
                    ModelState.AddModelError(error, string.Format("existed -> "));
                }
            }

            if (ModelState.IsValid)
            {
                _context.AutoTypes.UpdateRange(data);
                _context.SaveChanges();
            }

            return Ok(new
            {
                Data = _context.AutoTypes.Where(a => a.Active == true),
                Errors = errors.Bind(ModelState)
            });
        }

        public IActionResult GetAutoBrand()
        {
            return Ok(_context.AutoBrands);
        }

        public IActionResult UpdateAutoBrand(IEnumerable<AutoBrand> data)
        {
            var items = data.GroupBy(s => s.BrandName.ToLower().Replace(" ", ""))
                            .Where(g => g.Count() > 1)
                            .Select(g => g.Key);

            ModelMessage errors = new ModelMessage(ModelState);
            if (items.Count() > 0)
            {
                foreach (var error in items)
                {
                    ModelState.AddModelError(error, string.Format("existed -> "));
                }
            }

            if (ModelState.IsValid)
            {
                _context.AutoBrands.UpdateRange(data);
                _context.SaveChanges();
            }

            return Ok(new
            {
                Data = _context.AutoBrands.Where(a => a.Active == true),
                Errors = errors.Bind(ModelState)
            });
        }

        public IActionResult GetAutoModel()
        {
            return Ok(_context.AutoModels);
        }
        public IActionResult UpdateAutoModel(IEnumerable<AutoModel> data)
        {
            var items = data.GroupBy(s => s.ModelName.ToLower().Replace(" ", ""))
                            .Where(g => g.Count() > 1)
                            .Select(g => g.Key);

            ModelMessage errors = new ModelMessage(ModelState);
            if (items.Count() > 0)
            {
                foreach (var error in items)
                {
                    ModelState.AddModelError(error, string.Format("existed -> "));
                }
            }

            if (ModelState.IsValid)
            {
                _context.AutoModels.UpdateRange(data);
                _context.SaveChanges();
            }

            return Ok(new
            {
                Data = _context.AutoModels.Where(a => a.Active == true),
                Errors = errors.Bind(ModelState)
            });
        }

        public IActionResult GetAutoColor()
        {
            return Ok(_context.AutoColors);
        }
        public IActionResult UpdateAutoColor(IEnumerable<AutoColor> data)
        {
            var items = data.GroupBy(s => s.ColorName.ToLower().Replace(" ", ""))
                            .Where(g => g.Count() > 1)
                            .Select(g => g.Key);

            ModelMessage errors = new ModelMessage(ModelState);
            if (items.Count() > 0)
            {
                foreach (var error in items)
                {
                    ModelState.AddModelError(error, string.Format("existed -> "));
                }
            }

            if (ModelState.IsValid)
            {
                _context.AutoColors.UpdateRange(data);
                _context.SaveChanges();
            }

            return Ok(new
            {
                Data = _context.AutoColors.Where(a => a.Active == true),
                Errors = errors.Bind(ModelState)
            });
        }

        [HttpGet]
        public IActionResult GetListCusForm()
        {
            var _partner = _context.BusinessPartners.Include(c => c.AutoMobile).Where(c => !c.Delete);
            return Ok(_partner);
        }
        [HttpPost]
        public IActionResult CheckVCus(int id)
        {
            var _partner = _context.BusinessPartners.Include(c => c.AutoMobile).First(c => c.ID == id);
            if (_partner != null)
            {
                if (_partner.AutoMobile.Count() != 0)
                {
                    return Ok(new { status = "Y" });
                }
                return Ok(new { status = "N" });
            }
            return Ok();
        }

        [HttpPost]
        public IActionResult GetVCus(int id)
        {
            var chkV = from auto in _context.AutoMobiles.Where(c => c.BusinessPartnerID == id && !c.Deleted)
                       join type in _context.AutoTypes on auto.TypeID equals type.TypeID
                       join brand in _context.AutoBrands on auto.BrandID equals brand.BrandID
                       join model in _context.AutoModels on auto.ModelID equals model.ModelID
                       join color in _context.AutoColors on auto.ColorID equals color.ColorID
                       select new
                       {
                           auto.AutoMID,
                           auto.Plate,
                           auto.Frame,
                           auto.Engine,
                           type.TypeName,
                           brand.BrandName,
                           model.ModelName,
                           auto.Year,
                           color.ColorName
                       };
            return Ok(chkV);
        }

        [HttpPost]
        public IActionResult GoChooseCus(int id)
        {
            var _bus = _context.BusinessPartners.First(c => c.ID == id);
            return Ok(_bus);
        }

        [HttpPost]
        public IActionResult ChooseCusV(int id)
        {
            var dropCnV = _context.AutoMobiles.Where(c => c.AutoMID == id);
            var busid = dropCnV.FirstOrDefault().BusinessPartnerID;
            var _bus = _context.BusinessPartners.First(c => c.ID == busid);

            return Ok(_bus);
        }

        [HttpPost]
        public IActionResult GetGeneralCus(int genid)
        {
            var geninfo = _context.BusinessPartners.First(c => c.ID == genid);
            return Ok(geninfo);
        }

        [HttpPost]
        public IActionResult SaveKVMSQuote(string quote)
        {
            QuoteAutoM _qam = JsonConvert.DeserializeObject<QuoteAutoM>(quote);

            var businfo = _context.BusinessPartners.FirstOrDefault(c => c.ID == _qam.CusID);
            if (_qam.ID == 0)
            {
                //Save Quote
                var qNo = "";
                SeriesDetail seriesDetail = new SeriesDetail();
                var _docType = _context.DocumentTypes.FirstOrDefault(c => c.Code == "PQ");
                var _seriesQ = _context.Series.FirstOrDefault(c => c.DocuTypeID == _docType.ID && c.Default);


                //insert seriesDetail
                seriesDetail.SeriesID = _seriesQ.ID;
                seriesDetail.Number = _seriesQ.NextNo;
                _context.Update(seriesDetail);
                //update series
                string Sno = _seriesQ.NextNo;
                long No = long.Parse(Sno);
                _seriesQ.NextNo = Convert.ToString(No + 1);
                _context.Update(_seriesQ);
                _context.SaveChanges();
                //update quote
                _qam.SeriesID = _seriesQ.ID;
                _qam.SeriesDID​ = seriesDetail.ID;
                _qam.DocTypeID = _docType.ID;
                qNo = _seriesQ.PreFix + "-" + seriesDetail.Number;

                if (_qam.AutoMID == 0)
                {
                    _qam.Code = businfo.Code;
                    _qam.Name = businfo.Name;
                    _qam.PriceListID = businfo.PriceListID;
                    _qam.Type = businfo.Type;
                    _qam.Phone = businfo.Phone;
                    _qam.Email = businfo.Email;
                    _qam.Address = businfo.Address;
                    //Queue Invoice
                    _qam.QNo = qNo;
                    _qam.Status = Status.Exist;
                    _context.QuoteAutoMs.Update(_qam);
                    _context.SaveChanges();

                }
                else
                {
                    var autoinfo = _context.AutoMobiles.First(c => c.AutoMID == _qam.AutoMID);
                    // Customer
                    _qam.Code = businfo.Code;
                    _qam.Name = businfo.Name;
                    _qam.PriceListID = businfo.PriceListID;
                    _qam.Type = businfo.Type;
                    _qam.Phone = businfo.Phone;
                    _qam.Email = businfo.Email;
                    _qam.Address = businfo.Address;
                    //Queue Invoice
                    _qam.QNo = qNo;
                    _qam.Status = Status.Exist;
                    // AutoMobile
                    _qam.Plate = autoinfo.Plate;
                    _qam.Frame = autoinfo.Frame;
                    _qam.Engine = autoinfo.Engine;
                    _qam.TypeName = _context.AutoTypes.First(c => c.TypeID == autoinfo.TypeID).TypeName;
                    _qam.BrandName = _context.AutoBrands.First(c => c.BrandID == autoinfo.BrandID).BrandName;
                    _qam.ModelName = _context.AutoModels.First(c => c.ModelID == autoinfo.ModelID).ModelName;
                    _qam.Year = autoinfo.Year;
                    _qam.ColorName = _context.AutoColors.First(c => c.ColorID == autoinfo.ColorID).ColorName;
                    // OrderDetail
                    _context.QuoteAutoMs.Update(_qam);
                    _context.SaveChanges();
                }
            }
            else
            {

                var _DeleteOrderD = _context.OrderDetailQAutoMs.Where(c => c.OrderID == _qam.OrderQAutoM.OrderID);
                if (_qam.OrderQAutoM.OrderDetailQAutoMs.Count() < _DeleteOrderD.Count())
                {
                    _context.OrderDetailQAutoMs.RemoveRange(_DeleteOrderD);
                    _context.SaveChanges();

                    foreach (var _item in _qam.OrderQAutoM.OrderDetailQAutoMs)
                    {
                        _item.ID = 0;
                    }

                    _context.OrderDetailQAutoMs.AddRange(_qam.OrderQAutoM.OrderDetailQAutoMs);
                    _context.SaveChanges();
                }

                //Update Quote
                if (_qam.AutoMID == 0)
                {
                    // Customer
                    _qam.Code = businfo.Code;
                    _qam.Name = businfo.Name;
                    _qam.PriceListID = businfo.PriceListID;
                    _qam.Type = businfo.Type;
                    _qam.Phone = businfo.Phone;
                    _qam.Email = businfo.Email;
                    _qam.Address = businfo.Address;
                    //Queue Invoice
                    _qam.QNo = _qam.QNo;
                    _qam.Status = Status.Exist;
                    _context.QuoteAutoMs.Update(_qam);
                    _context.SaveChanges();
                }
                else
                {
                    var autoinfo = _context.AutoMobiles.First(c => c.AutoMID == _qam.AutoMID);
                    // Customer
                    _qam.Code = businfo.Code;
                    _qam.Name = businfo.Name;
                    _qam.PriceListID = businfo.PriceListID;
                    _qam.Type = businfo.Type;
                    _qam.Phone = businfo.Phone;
                    _qam.Email = businfo.Email;
                    _qam.Address = businfo.Address;
                    //Queue Invoice
                    _qam.QNo = _qam.QNo;
                    _qam.Status = Status.Exist;
                    // AutoMobile
                    _qam.Plate = autoinfo.Plate;
                    _qam.Frame = autoinfo.Frame;
                    _qam.Engine = autoinfo.Engine;
                    _qam.TypeName = _context.AutoTypes.First(c => c.TypeID == autoinfo.TypeID).TypeName;
                    _qam.BrandName = _context.AutoBrands.First(c => c.BrandID == autoinfo.BrandID).BrandName;
                    _qam.ModelName = _context.AutoModels.First(c => c.ModelID == autoinfo.ModelID).ModelName;
                    _qam.Year = autoinfo.Year;
                    _qam.ColorName = _context.AutoColors.First(c => c.ColorID == autoinfo.ColorID).ColorName;
                    // OrderDetail
                    _context.QuoteAutoMs.Update(_qam);
                    _context.SaveChanges();
                }
            }
            return Ok(_qam.ID);
        }

        public IActionResult PrintQuoteCus(int qid)
        {
            var data = _context.QuoteAutoMs.Include(c => c.OrderQAutoM).ThenInclude(c => c.OrderDetailQAutoMs).First(c => c.ID == qid);
            var count = data.OrderQAutoM.OrderDetailQAutoMs.Count();
            var index = 0;

            List<PrintDetailQuotes> orderauto = new List<PrintDetailQuotes>();
            foreach (var item in data.OrderQAutoM.OrderDetailQAutoMs.ToList())
            {
                index++;
                PrintDetailQuotes qd = new PrintDetailQuotes
                {
                    Index = index.ToString(),
                    Code = item.Code,
                    EnglishName = item.EnglishName,
                    KhmerName = item.KhmerName,
                    Qty = item.Qty.ToString(),
                    UoM = _context.GroupUOMs.First(c => c.ID == item.UomID).Name,
                    UnitPrice = string.Format("{0:#,0.000}", item.UnitPrice),
                    DisRate = item.DiscountRate + " %",
                    TypeDis = item.TypeDis,
                    Currency = item.Currency,
                    Total = string.Format("{0:#,0.000}", item.Total)
                };
                orderauto.Add(qd);
            }

            var printq = data;
            var combid = _context.UserAccounts.First(c => c.ID == printq.OrderQAutoM.UserOrderID).BranchID;
            var comb = _context.ReceiptInformation.First(c => c.BranchID == combid);
            PrintQuoteKams list = new PrintQuoteKams();
            if (data.AutoMID == 0)
            {
                list = new PrintQuoteKams()
                {
                    QNo = printq.QNo,
                    Code = printq.Code,
                    Name = printq.Name,
                    PriceListName = _context.PriceLists.First(c => c.ID == printq.PriceListID).Name,
                    Phone = printq.Phone,
                    Email = printq.Email,
                    Address = printq.Address,
                    PrintDetailQuotes = orderauto.ToList(),
                    Username = _context.UserAccounts.First(c => c.ID == printq.OrderQAutoM.UserOrderID).Username,
                    //Summary
                    Count = count.ToString(),
                    Subtotal = string.Format("{0:#,0.000}", printq.OrderQAutoM.Sub_Total),
                    DisRate = printq.OrderQAutoM.DiscountRate.ToString() + " %",
                    TaxValue = string.Format("{0:#,0.000}", printq.OrderQAutoM.TaxValue),
                    GrandTotal = string.Format("{0:#,0.000}", printq.OrderQAutoM.GrandTotal),
                    //Company Info
                    ComBName = comb.Title,
                    ComBAddress = comb.Address,
                    ComBPhone = comb.Tel1 + " / " + comb.Tel2
                };
            }
            else
            {
                list = new PrintQuoteKams()
                {
                    QNo = printq.QNo,
                    Code = printq.Code,
                    Name = printq.Name,
                    PriceListName = _context.PriceLists.First(c => c.ID == printq.PriceListID).Name,
                    Phone = printq.Phone,
                    Email = printq.Email,
                    Address = printq.Address,
                    Plate = printq.Plate,
                    Frame = printq.Frame,
                    Engine = printq.Engine,
                    TypeName = printq.TypeName,
                    BrandName = printq.BrandName,
                    ModelName = printq.ModelName,
                    Year = printq.Year,
                    ColorName = printq.ColorName,
                    PrintDetailQuotes = orderauto.ToList(),
                    Username = _context.UserAccounts.First(c => c.ID == printq.OrderQAutoM.UserOrderID).Username,
                    //Summary
                    Count = count.ToString(),
                    Subtotal = string.Format("{0:#,0.000}", printq.OrderQAutoM.Sub_Total),
                    DisRate = printq.OrderQAutoM.DiscountRate.ToString() + " %",
                    TaxValue = string.Format("{0:#,0.000}", printq.OrderQAutoM.TaxValue),
                    GrandTotal = string.Format("{0:#,0.000}", printq.OrderQAutoM.GrandTotal),
                    //Company Info
                    ComBName = comb.Title,
                    ComBAddress = comb.Address,
                    ComBPhone = comb.Tel1 + " / " + comb.Tel2
                };
            }

            if (list.PrintDetailQuotes.Count() > 0)
            {
                return new ViewAsPdf(list);
            }

            return Ok();
        }

        public IActionResult PrintInvoice(int Invid)
        {
            var datas = _context.ReceiptDetailKvms.Include(c => c.ReceiptKvms).ThenInclude(c => c.KvmsInfo);
            var data = datas.Where(c => c.ReceiptKvmsID == Invid);

            var kmvsinfo = data.FirstOrDefault().ReceiptKvms.KvmsInfo;
            var receipt = data.FirstOrDefault().ReceiptKvms;
            var receiptdetail = data;

            var count = receiptdetail.Count();
            var index = 0;

            List<PrintDetailQuotes> orderauto = new List<PrintDetailQuotes>();
            foreach (var item in receiptdetail.ToList())
            {
                index++;
                PrintDetailQuotes qd = new PrintDetailQuotes
                {
                    Index = index.ToString(),
                    Code = item.Code,
                    EnglishName = item.EnglishName,
                    KhmerName = item.KhmerName,
                    Qty = item.Qty.ToString(),
                    UoM = _context.GroupUOMs.FirstOrDefault(c => c.ID == item.UomID).Name,
                    UnitPrice = string.Format("{0:#,0.000}", item.UnitPrice),
                    DisRate = item.DiscountRate + " %",
                    TypeDis = item.TypeDis,
                    Currency = item.Currency,
                    Total = string.Format("{0:#,0.000}", item.Total)
                };
                orderauto.Add(qd);
            }

            var combid = _context.UserAccounts.First(c => c.ID == receipt.UserOrderID).BranchID;
            var comb = _context.ReceiptInformation.First(c => c.BranchID == combid);

            PrintInvKvms list = new PrintInvKvms();
            if (kmvsinfo.AutoMID == 0)
            {
                list = new PrintInvKvms()
                {
                    QNo = receipt.ReceiptNo,
                    Code = kmvsinfo.Code,
                    Name = kmvsinfo.Name,
                    PriceListName = _context.PriceLists.First(c => c.ID == kmvsinfo.PriceListID).Name,
                    Phone = kmvsinfo.Phone,
                    Email = kmvsinfo.Email,
                    Address = kmvsinfo.Address,
                    PrintDetailQuotes = orderauto.ToList(),
                    Username = _context.UserAccounts.First(c => c.ID == receipt.UserOrderID).Username,
                    //Summary
                    Count = count.ToString(),
                    Subtotal = string.Format("{0:#,0.000}", receipt.Sub_Total),
                    DisRate = receipt.DiscountRate.ToString() + " %",
                    TaxValue = string.Format("{0:#,0.000}", receipt.TaxValue),
                    GrandTotal = string.Format("{0:#,0.000}", receipt.GrandTotal),
                    AppliedAmount = string.Format("{0:#,0.000}", receipt.AppliedAmount),
                    BalanceDue = string.Format("{0:#,0.000}", receipt.BalanceDue),
                    //Company Info
                    ComBName = comb.Title,
                    ComBAddress = comb.Address,
                    ComBPhone = comb.Tel1 + " / " + comb.Tel2
                };
            }
            else
            {
                list = new PrintInvKvms()
                {
                    QNo = receipt.ReceiptNo,
                    Code = kmvsinfo.Code,
                    Name = kmvsinfo.Name,
                    PriceListName = _context.PriceLists.First(c => c.ID == kmvsinfo.PriceListID).Name,
                    Phone = kmvsinfo.Phone,
                    Email = kmvsinfo.Email,
                    Address = kmvsinfo.Address,
                    Plate = kmvsinfo.Plate,
                    Frame = kmvsinfo.Frame,
                    Engine = kmvsinfo.Engine,
                    TypeName = kmvsinfo.TypeName,
                    BrandName = kmvsinfo.BrandName,
                    ModelName = kmvsinfo.ModelName,
                    Year = kmvsinfo.Year,
                    ColorName = kmvsinfo.ColorName,
                    PrintDetailQuotes = orderauto.ToList(),
                    Username = _context.UserAccounts.First(c => c.ID == receipt.UserOrderID).Username,
                    //Summary
                    Count = count.ToString(),
                    Subtotal = string.Format("{0:#,0.000}", receipt.Sub_Total),
                    DisRate = receipt.DiscountRate.ToString() + " %",
                    TaxValue = string.Format("{0:#,0.000}", receipt.TaxValue),
                    GrandTotal = string.Format("{0:#,0.000}", receipt.GrandTotal),
                    AppliedAmount = string.Format("{0:#,0.000}", receipt.AppliedAmount),
                    BalanceDue = string.Format("{0:#,0.000}", receipt.BalanceDue),
                    //Company Info
                    ComBName = comb.Title,
                    ComBAddress = comb.Address,
                    ComBPhone = comb.Tel1 + " / " + comb.Tel2
                };
            }

            if (list.PrintDetailQuotes.Count() > 0)
            {
                return new ViewAsPdf(list);
            }

            return Ok();
        }

        public IActionResult PrintReceiptMemo(int RMID)
        {
            var master = _context.ReceiptMemo.FirstOrDefault(c => c.ID == RMID);
            var detail = _context.ReceiptDetailMemoKvms.Where(c => c.ReceiptMemoID == RMID);

            var count = detail.Count();
            var index = 0;

            List<PrintDetailQuotes> detailMemo = new List<PrintDetailQuotes>();
            foreach (var item in detail.ToList())
            {
                index++;
                PrintDetailQuotes qd = new PrintDetailQuotes
                {
                    Index = index.ToString(),
                    Code = item.Code,
                    EnglishName = item.EnglishName,
                    KhmerName = item.KhmerName,
                    Qty = item.Qty.ToString(),
                    UoM = _context.GroupUOMs.First(c => c.ID == item.UomID).Name,
                    UnitPrice = string.Format("{0:#,0.000}", item.UnitPrice),
                    DisRate = item.DisRate + " %",
                    TypeDis = item.TypeDis,
                    Currency = item.Currency,
                    Total = string.Format("{0:#,0.000}", item.Total)
                };
                detailMemo.Add(qd);
            }

            var combid = _context.UserAccounts.First(c => c.ID == master.UserOrderID).BranchID;
            var comb = _context.ReceiptInformation.First(c => c.BranchID == combid);

            var _kvmsinfoId = _context.ReceiptKvms.FirstOrDefault(c => c.ReceiptKvmsID == master.ReceiptKvmsID).KvmsInfoID;
            var _kvmsInfo = _context.KvmsInfo.FirstOrDefault(c => c.KvmsInfoID == _kvmsinfoId);

            PrintReceiptMemo list = new PrintReceiptMemo();
            if (_kvmsInfo.AutoMID == 0)
            {
                list = new PrintReceiptMemo()
                {
                    RMemoNo = master.ReceiptMemoNo,
                    BaseOn = master.ReceiptNo,
                    Code = _kvmsInfo.Code,
                    Name = _kvmsInfo.Name,
                    PriceListName = _context.PriceLists.First(c => c.ID == _kvmsInfo.PriceListID).Name,
                    Phone = _kvmsInfo.Phone,
                    Email = _kvmsInfo.Email,
                    Address = _kvmsInfo.Address,
                    PrintDetailQuotes = detailMemo.ToList(),
                    Username = _context.UserAccounts.First(c => c.ID == master.UserOrderID).Username,
                    //Summary
                    Count = count.ToString(),
                    Subtotal = string.Format("{0:#,0.000}", master.SubTotal),
                    DisRate = master.DisRate.ToString() + " %",
                    TaxValue = string.Format("{0:#,0.000}", master.TaxValue),
                    GrandTotal = string.Format("{0:#,0.000}", master.GrandTotal),
                    AppliedAmount = string.Format("{0:#,0.000}", master.AppliedAmount),
                    BalanceDue = string.Format("{0:#,0.000}", master.BalanceDue),
                    //Company Info
                    ComBName = comb.Title,
                    ComBAddress = comb.Address,
                    ComBPhone = comb.Tel1 + " / " + comb.Tel2
                };
            }
            else
            {
                list = new PrintReceiptMemo()
                {
                    RMemoNo = master.ReceiptMemoNo,
                    BaseOn = master.ReceiptNo,
                    Code = _kvmsInfo.Code,
                    Name = _kvmsInfo.Name,
                    PriceListName = _context.PriceLists.First(c => c.ID == _kvmsInfo.PriceListID).Name,
                    Phone = _kvmsInfo.Phone,
                    Email = _kvmsInfo.Email,
                    Address = _kvmsInfo.Address,
                    Plate = _kvmsInfo.Plate,
                    Frame = _kvmsInfo.Frame,
                    Engine = _kvmsInfo.Engine,
                    TypeName = _kvmsInfo.TypeName,
                    BrandName = _kvmsInfo.BrandName,
                    ModelName = _kvmsInfo.ModelName,
                    Year = _kvmsInfo.Year,
                    ColorName = _kvmsInfo.ColorName,
                    PrintDetailQuotes = detailMemo.ToList(),
                    Username = _context.UserAccounts.First(c => c.ID == master.UserOrderID).Username,
                    //Summary
                    Count = count.ToString(),
                    Subtotal = string.Format("{0:#,0.000}", master.SubTotal),
                    DisRate = master.DisRate.ToString() + " %",
                    TaxValue = string.Format("{0:#,0.000}", master.TaxValue),
                    GrandTotal = string.Format("{0:#,0.000}", master.GrandTotal),
                    AppliedAmount = string.Format("{0:#,0.000}", master.AppliedAmount),
                    BalanceDue = string.Format("{0:#,0.000}", master.BalanceDue),
                    //Company Info
                    ComBName = comb.Title,
                    ComBAddress = comb.Address,
                    ComBPhone = comb.Tel1 + " / " + comb.Tel2
                };
            }

            if (list.PrintDetailQuotes.Count() > 0)
            {
                return new ViewAsPdf(list);
            }

            return Ok();
        }

        [HttpGet]
        public IActionResult GetListQuote()
        {
            var qlist = _context.QuoteAutoMs.Where(c => c.Status == Status.Exist).Include(c => c.OrderQAutoM).ThenInclude(c => c.OrderDetailQAutoMs);
            return Ok(qlist);
        }

        [HttpGet]
        public IActionResult GetListInvoice()
        {
            var list = _context.ReceiptKvms.Include(c => c.KvmsInfo);
            return Ok(list);
        }

        [HttpPost]
        public IActionResult DropQuote(int id)
        {
            var data = _context.QuoteAutoMs.Include(c => c.OrderQAutoM).ThenInclude(c => c.OrderDetailQAutoMs).ThenInclude(c => c.UnitofMeansure).First(c => c.ID == id);
            return Ok(data);
        }

        [HttpPost]
        public IActionResult GetCusVInfo(int QID)
        {
            var cusVinfo = _context.QuoteAutoMs.FirstOrDefault(c => c.ID == QID);
            if (cusVinfo.AutoMID == 0)
            {
                return Ok(new { QId = QID, CusId = cusVinfo.CusID, QNo = cusVinfo.QNo, CusName = cusVinfo.Name });
            }
            else
            {
                return Ok(new { QId = QID, CusId = cusVinfo.CusID, AutoMId = cusVinfo.AutoMID, QNo = cusVinfo.QNo, CusName = cusVinfo.Name });
            }
        }

        public IActionResult FetchOrders(int tableId, int orderId = 0)
        {
            return Ok(GetOrderInfoQuote(tableId, orderId));
        }

        public IActionResult FindOrderofQuote(int tableId, int orderId)
        {
            return Ok(GetOrderInfoQuote(tableId, orderId));
        }

        OrderInfoOfQuote GetOrderInfoQuote(int tableId, int orderId = 0, bool setDefaultOrder = false)
        {
            List<ServiceItemSales> serviceItemSales = new List<ServiceItemSales>();
            DisplayCurrencyModel displayCurrency = new DisplayCurrencyModel();
            List<ServiceItemSales> itemGroup1s = new List<ServiceItemSales>();

            var table = _context.Tables.Find(tableId);
            if (table == null)
            {
                table = _context.Tables.FirstOrDefault();
            }

            var orders = GetOrdersofQoute(table.ID, orderId);
            var order = orders.FirstOrDefault(o => o.OrderID == orderId);
            if (order == null)
            {
                order = CreateOrderQuote(tableId);
            }

            if (orders.Count > 0 && setDefaultOrder)
            {
                order = orders.LastOrDefault();
            }

            itemGroup1s.AddRange(_ikvms.GetGroup1s.Select(g => new ServiceItemSales
            {
                ID = g.ItemG1ID,
                KhmerName = g.Name,
                Image = g.Images,
                Group1 = g.ItemG1ID,
                Group2 = 1,
                Group3 = 1
            }));

            displayCurrency = GetDisplayCurrency(order.PriceListID);
            serviceItemSales = _ikvms.GetItemMasterDatas(order.PriceListID).ToList();

            OrderInfoOfQuote orderInfoOfQoute = new OrderInfoOfQuote
            {
                BaseItemGroups = itemGroup1s,
                Setting = Setting,
                Orders = orders,
                Order = order,
                OrderTable = table,
                DisplayCurrency = displayCurrency,
                ServiceItems = serviceItemSales
            };
            return orderInfoOfQoute;
        }

        public List<OrderQAutoM> GetOrdersofQoute(int tableId, int orderId = 0)
        {
            var ordersqoute = _context.OrderQAutoMs.Where(o => o.TableID == tableId
                && o.UserOrderID == GetUserID() && !o.Cancel)
                .Include(o => o.OrderDetailQAutoMs).Include(o => o.Currency)
                .ToList();

            ordersqoute.ForEach(o =>
            {
                o.OrderDetailQAutoMs.ToList().ForEach(od =>
                {
                    od.ItemStatus = "old";
                    od.PrintQty = 0;
                });
                o.OrderDetailQAutoMs = (from od in o.OrderDetailQAutoMs
                                        join uom in _context.GroupDUoMs on od.UomID equals uom.AltUOM
                                        select od).ToList();
            });
            return ordersqoute;
        }

        public OrderQAutoM CreateOrderQuote(int tableId)
        {
            var setRate = _context.ExchangeRates.Where(c => c.CurrencyID == Setting.LocalCurrencyID).Select(c => c.SetRate).FirstOrDefault();
            var tax = _context.Tax.FirstOrDefault(t => !t.Delete) ?? new Tax();
            var priceList = GetPriceList(Setting.PriceListID);
            return new OrderQAutoM
            {
                OrderID = 0,
                OrderNo = "",
                TableID = tableId,
                ReceiptNo = "N/A",
                QueueNo = "1",
                DateIn = DateTime.Parse(DateTime.Today.ToString("yyyy-MM-dd")),
                DateOut = DateTime.Parse(DateTime.Today.ToString("yyyy-MM-dd")),
                TimeIn = DateTime.Now.ToShortTimeString(),
                TimeOut = DateTime.Now.ToShortTimeString(),
                WaiterID = 1,
                UserOrderID = GetUserID(),
                UserDiscountID = GetUserID(),
                CustomerID = Setting.CustomerID,
                CustomerCount = 1,
                PriceListID = Setting.PriceListID,
                LocalCurrencyID = Setting.LocalCurrencyID,
                SysCurrencyID = Setting.SysCurrencyID,
                LocalSetRate = setRate,
                ExchangeRate = Setting.RateIn,
                WarehouseID = Setting.WarehouseID,
                BranchID = Setting.BranchID,
                CompanyID = Setting.CompanyID,
                Sub_Total = 0,
                DiscountRate = 0,
                DiscountValue = 0,
                TypeDis = "Percent",
                TaxRate = tax.Rate,
                TaxValue = 0,
                GrandTotal = 0,
                GrandTotal_Sys = 0,
                Received = 0,
                Change = 0,
                DisplayRate = 0,
                PLCurrencyID = priceList.CurrencyID,
                Currency = priceList.Currency,
                PLRate = Setting.RateIn,
                GrandTotal_Display = 0,
                Change_Display = 0,
                PaymentMeansID = Setting.PaymentMeansID,
                CheckBill = 'N',
                OrderDetailQAutoMs = new List<OrderDetailQAutoMs>()
            };
        }

        [HttpPost]
        public IActionResult DeleteOrderDetailQuote(int id, int userId = 0)
        {
            if (userId == 0)
            {
                userId = GetUserID();
            }

            var _orderDetailQ = _context.OrderDetailQAutoMs.Find(id);
            if (_orderDetailQ != null)
            {
                _context.OrderDetailQAutoMs.Remove(_orderDetailQ);
                _context.SaveChanges();
            }

            return Ok();
        }

        [HttpPost]
        public IActionResult SendQuote(string data, string printType, int CID, int VID, string SQNo)
        {
            Order order = JsonConvert.DeserializeObject<Order>(data, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });

            OrderQAutoM orderQuote = JsonConvert.DeserializeObject<OrderQAutoM>(data, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });

            var qcount = _context.KvmsInfo.Count() + 1;
            var qNo = "NSQ-" + qcount.ToString().PadLeft(7, '0');

            //Pay without return from Quote
            var updateq = _context.QuoteAutoMs.FirstOrDefault(c => c.ID == orderQuote.OrderID);
            if (updateq == null)
            {
                var Cusdata = _context.BusinessPartners.Include(c => c.AutoMobile).FirstOrDefault(c => c.ID == CID);
                var mcid = (int)DateTime.Now.Ticks;
                var QID = 0;
                if (mcid < 0)
                {
                    var tempid = mcid.ToString().Replace("-", "");
                    QID = Convert.ToInt32(tempid);
                }
                else
                {
                    QID = mcid;
                }
                //Gernel Customer
                if (Cusdata == null)
                {
                    var gencusid = Setting.CustomerID;
                    var gendata = _context.BusinessPartners.FirstOrDefault(c => c.ID == gencusid);
                    KvmsInfo kvmsInfos = new KvmsInfo
                    {
                        //Customer Info from setting // Not finished yet
                        KvmsInfoID = QID,
                        QNo = qNo,
                        Code = gendata.Code,
                        Name = gendata.Name,
                        CusID = gendata.ID,
                        PriceListID = gendata.PriceListID,
                        Type = gendata.Type,
                        Phone = gendata.Phone,
                        Email = gendata.Email,
                        Address = gendata.Address,
                        Status = StatusKvmsInfo.Exist
                    };
                    _context.KvmsInfo.Add(kvmsInfos);
                    _context.SaveChanges();
                }
                else
                {
                    //Non Gernel Customer
                    //Without Vehicle
                    if (Cusdata.AutoMobile.Count() == 0)
                    {
                        KvmsInfo kvmsInfos = new KvmsInfo
                        {
                            //Customer Info
                            KvmsInfoID = QID,
                            QNo = qNo,
                            Code = Cusdata.Code,
                            Name = Cusdata.Name,
                            CusID = Cusdata.ID,
                            PriceListID = Cusdata.PriceListID,
                            Type = Cusdata.Type,
                            Phone = Cusdata.Phone,
                            Email = Cusdata.Email,
                            Address = Cusdata.Address,
                            Status = StatusKvmsInfo.Exist
                        };
                        _context.KvmsInfo.Add(kvmsInfos);
                        _context.SaveChanges();
                    }
                    else
                    {
                        //With Vehicle
                        var vehicleinfo = Cusdata.AutoMobile.Where(c => c.AutoMID == VID);
                        KvmsInfo kvmsInfos = new KvmsInfo
                        {
                            //Customer Info
                            KvmsInfoID = QID,
                            QNo = qNo,
                            Code = Cusdata.Code,
                            Name = Cusdata.Name,
                            CusID = Cusdata.ID,
                            PriceListID = Cusdata.PriceListID,
                            Type = Cusdata.Type,
                            Phone = Cusdata.Phone,
                            Email = Cusdata.Email,
                            Address = Cusdata.Address,
                            Status = StatusKvmsInfo.Exist,
                            //Vehicle Info
                            AutoMID = vehicleinfo.FirstOrDefault().AutoMID,
                            Plate = vehicleinfo.FirstOrDefault().Plate,
                            Engine = vehicleinfo.FirstOrDefault().Engine,
                            Frame = vehicleinfo.FirstOrDefault().Frame,
                            TypeName = _context.AutoTypes.FirstOrDefault(c => c.TypeID == vehicleinfo.FirstOrDefault().TypeID).TypeName,
                            BrandName = _context.AutoBrands.FirstOrDefault(c => c.BrandID == vehicleinfo.FirstOrDefault().BrandID).BrandName,
                            ModelName = _context.AutoModels.FirstOrDefault(c => c.ModelID == vehicleinfo.FirstOrDefault().ModelID).ModelName,
                            ColorName = _context.AutoColors.FirstOrDefault(c => c.ColorID == vehicleinfo.FirstOrDefault().ColorID).ColorName,
                            Year = vehicleinfo.FirstOrDefault().Year
                        };
                        _context.KvmsInfo.Add(kvmsInfos);
                        _context.SaveChanges();
                    }
                }

                return Ok(_ikvms.SendOrderQuote(order, printType, QID));
            }
            else
            {
                //Pay with return from Quote
                updateq.Status = Status.Delete;
                _context.QuoteAutoMs.Update(updateq);
                _context.SaveChanges();

                //Add datas from QuoteAutoMs to KvmsInfo for customer that has Quote
                var Cusdata = _context.QuoteAutoMs.FirstOrDefault(c => c.CusID == CID && c.QNo == SQNo);
                var QID = Cusdata.ID;
                var QuoteData = _context.QuoteAutoMs.FirstOrDefault(c => c.ID == QID);
                KvmsInfo kvmsInfo = new KvmsInfo
                {
                    //Customer Info
                    KvmsInfoID = QuoteData.ID,
                    Code = QuoteData.Code,
                    Name = QuoteData.Name,
                    CusID = QuoteData.CusID,
                    PriceListID = QuoteData.PriceListID,
                    Type = QuoteData.Type,
                    Phone = QuoteData.Phone,
                    Email = QuoteData.Email,
                    Address = QuoteData.Address,
                    Status = StatusKvmsInfo.Exist,
                    //Vehicle Info
                    AutoMID = QuoteData.AutoMID,
                    QNo = QuoteData.QNo,
                    Plate = QuoteData.Plate,
                    Engine = QuoteData.Engine,
                    Frame = QuoteData.Frame,
                    TypeName = QuoteData.TypeName,
                    BrandName = QuoteData.BrandName,
                    ModelName = QuoteData.ModelName,
                    ColorName = QuoteData.ColorName,
                    Year = QuoteData.Year
                };
                _context.KvmsInfo.Add(kvmsInfo);
                _context.SaveChanges();

                return Ok(_ikvms.SendOrderQuote(order, printType, QID));
            }

        }

        public IActionResult GetNewOrderDetail(int id, int orderId = 0)
        {
            OrderQAutoM order = _context.OrderQAutoMs.Find(orderId);
            var priceListId = Setting.PriceListID;
            if (order != null)
            {
                priceListId = order.PriceListID;
            }
            ServiceItemSales item = _ikvms.GetItemMasterDatas(priceListId).FirstOrDefault(i => i.ID == id);
            if (item == null) { return Ok(new OrderDetailQAutoMs()); }
            return Ok(new OrderDetailQAutoMs
            {
                Line_ID = item.ID,
                OrderID = orderId,
                ID = 0,
                ItemID = item.ItemID,
                Code = item.Code,
                KhmerName = item.KhmerName,
                EnglishName = item.EnglishName,
                Cost = item.Cost,
                DiscountRate = item.DiscountRate,
                DiscountValue = item.DiscountValue,
                Qty = item.Qty,
                PrintQty = item.PrintQty,
                UnitPrice = item.UnitPrice,
                Total = item.Qty * item.UnitPrice,
                UomID = item.UomID,
                Uom = item.UoM,
                ItemStatus = "new",
                ItemPrintTo = item.PrintTo,
                Currency = item.Currency,
                ItemType = item.ItemType,
                TypeDis = item.TypeDis,
                Description = item.Description,
            });
        }

        public IActionResult GetGroupItems(int group1, int group2, int group3, int orderId, int level = 0)
        {
            var priceListId = Setting.PriceListID;
            var order = _context.OrderQAutoMs.Find(orderId);
            if (order != null)
            {
                priceListId = order.PriceListID;
            }
            var itemInfos = new List<ServiceItemSales>();
            var items = _ikvms.GetItemMasterDatas(priceListId).ToList();
            switch (level)
            {
                case 1:
                    itemInfos.AddRange(_ikvms.FilterGroup2(group1).Select(g => new ServiceItemSales
                    {
                        ID = g.ItemG2ID,
                        KhmerName = g.Name,
                        Image = g.Images,
                        Group1 = (int)g.ItemG1ID,
                        Group2 = g.ItemG2ID,
                        Group3 = 1
                    }));
                    itemInfos.AddRange(items.Where(i => i.Group1 == group1 && i.Group2 == group2));
                    break;
                case 2:
                    itemInfos.AddRange(_ikvms.FilterGroup3(group1, group2).Select(g => new ServiceItemSales
                    {
                        ID = g.ID,
                        KhmerName = g.Name,
                        Image = g.Images,
                        Group1 = (int)g.ItemG1ID,
                        Group2 = (int)g.ItemG2ID,
                        Group3 = g.ID
                    }));
                    itemInfos.AddRange(items.Where(i => i.Group1 == group1 && i.Group2 == group2 && i.Group3 == group3));
                    break;
            }
            return Ok(itemInfos);
        }

        [HttpPost]
        public IActionResult DefaultCusN(int CusId)
        {
            var cusname = _context.BusinessPartners.FirstOrDefault(c => c.ID == CusId).Name;
            return Ok(new { CusName = cusname });
        }

        [HttpPost]
        public IActionResult CancelReFirst(int ReceiptID)
        {
            var _receipt = _context.ReceiptKvms.FirstOrDefault(c => c.ReceiptKvmsID == ReceiptID);
            _receipt.AppliedAmount = 0;
            _receipt.BalanceDue = _receipt.GrandTotal;
            _receipt.Received = 0;
            _receipt.Change = (_receipt.GrandTotal * -1);
            _receipt.Status = StatusReceipt.Aging;
            //Update OpenQty to Orignal Qty
            _context.Update(_receipt);
            _context.SaveChanges();
            return Ok();
        }

        [HttpPost]
        public IActionResult CheckIfCuzHasV(int CusID)
        {
            var __CusV = _context.AutoMobiles.Where(c => c.BusinessPartnerID == CusID);
            if (__CusV.Count() == 0)
            {
                return Ok(new { CusV = "No" });
            }
            else
            {
                return Ok(new { CusV = "Yes" });
            }
        }

        //End of VMC Edition

        //Section 1

        [HttpGet]
        public IActionResult datatable()
        {
            return View();
        }

        public IActionResult Table()
        {
            var branchid = Convert.ToInt32(@User.FindFirst("BranchID").Value);
            List<GroupTable> groupTables = _context.GroupTables.Where(w => !w.Delete && w.BranchID == branchid).ToList();
            List<Table> ls_table = new List<Table>();
            foreach (var item in groupTables.ToList())
            {
                var tables = _context.Tables.Where(w => w.Delete == false && w.GroupTableID == item.ID).ToList();
                ls_table.AddRange(tables);
            }
            ServiceTable service_Table = new ServiceTable
            {
                GroupTables = groupTables,
                Tables = ls_table
            };
            return Ok(service_Table);
        }

        public IActionResult InitialStatusTable()
        {
            _ikvms.InitialStatusTable();
            return Ok();
        }

        public IActionResult UpdateTimeOnTable()
        {
            _ikvms.UpdateTimeOnTable();
            return Ok();
        }

        [HttpGet]
        public IActionResult GetTableAvailable(int group_id, int tableid)
        {
            var branchid = Convert.ToInt32(@User.FindFirst("BranchID").Value);
            List<GroupTable> groupTables = _context.GroupTables.Where(w => w.Delete == false && w.BranchID == branchid).ToList();
            List<Table> ls_table = new List<Table>();
            foreach (var item in groupTables.ToList())
            {
                var tables = _context.Tables.Where(w => w.Delete == false && w.GroupTableID == item.ID && w.Status == 'A').ToList();
                ls_table.AddRange(tables);
            }

            if (group_id == 0)
            {
                //var Tables = _context.Tables.Where(w => w.ID != tableid && w.Status == 'A' && w.Delete==false).OrderBy(by=>by.ID);
                return Ok(ls_table);
            }
            else
            {
                var Tables = _context.Tables.Where(w => w.ID != tableid && w.Status == 'A' && w.GroupTableID == group_id && w.Delete == false).OrderBy(by => by.ID);
                return Ok(Tables);
            }
        }

        [HttpGet]
        public IActionResult GetReceiptCombine(int orderid)
        {
            var branchid = Convert.ToInt32(@User.FindFirst("BranchID").Value);
            var orders = _context.Order.Where(w => w.OrderID != orderid && w.BranchID == branchid).OrderBy(by => by.TableID).ToList();
            return Ok(orders);
        }

        public IActionResult CombineReceipt(CombineOrder combineReceipt)
        {
            _ikvms.CombineReceipt(combineReceipt);
            return Ok();
        }

        public IActionResult SplitItem(Order order)
        {
            // _pos.SplitItem(order);
            return Ok();
        }

        public IActionResult SecondScreen()
        {
            return View();
        }

        [HttpGet]
        public IActionResult GetTableByGroup(int group)
        {
            if (group == 0)
            {
                var table = _context.Tables.Where(w => w.Delete == false).OrderBy(by => by.ID);
                return Ok(table);
            }
            else
            {
                var table = _context.Tables.Where(w => w.GroupTableID == group && w.Delete == false).OrderBy(by => by.ID);
                return Ok(table);
            }

        }

        [HttpGet]
        public IActionResult GetGroupUomDefined()
        {
            var GroupUomDefined = _context.GroupDUoMs.Where(w => w.Delete == false).ToList();
            return Ok(GroupUomDefined);
        }

        //Section 2
        public IActionResult Counter()
        {
            ViewBag.style = "fa fa-cogs";
            ViewBag.Main = "KVMS";
            ViewBag.Page = "Choose";
            ViewBag.Subpage = "Counter";
            ViewBag.type = "List";
            ViewBag.button = "fa-edit";
            ViewBag.Menu = "show";
            var count = _context.Counters.ToList();
            return View(count);
        }

        public IActionResult GeneralSetting(bool json = false)
        {
            ViewBag.Setting = "highlight";
            if (json)
            {
                return Ok(Setting);
            }
            return View("GeneralSetting", GetSettingViewModel());
        }

        public IActionResult GetSetting(int userid = 0)
        {
            var settings = _context.GeneralSettings.Where(w => w.UserID == GetUserID()).ToList();
            return Ok(settings);
        }

        public IActionResult GetUoMs()
        {
            var Uom = _context.UnitofMeasures.Where(u => !u.Delete);
            return Ok(Uom);
        }

        [HttpPost]
        public IActionResult GetOrder(int tableid, int orderid, int userid)
        {
            var order = _ikvms.GetOrder(tableid, orderid, userid);
            return Ok(order);
        }

        public IActionResult ClearUserOrder(int tableid)
        {
            _ikvms.ClearUserOrder(tableid);
            return Ok();
        }

        public IActionResult GetItemMasterData(int PriceListID)
        {
            var item = _ikvms.GetItemMasterDatas(PriceListID).ToList();
            return Ok(item);
        }

        public IActionResult GetItemMasterByBarcode(int pricelist, string barcode)
        {
            if (string.IsNullOrWhiteSpace(barcode)) { return Ok(); }
            try
            {
                var item = _ikvms.GetItemMasterByBarcode(pricelist, barcode).ToList();
                return Ok(item);
            }
            catch (Exception)
            {
                return Ok();
            }
        }

        public IActionResult GetItemMasterDataByGroup(int PriceListID)
        {
            var item = _ikvms.FilterItemByGroup(PriceListID).ToList();
            return Ok(item);
        }

        public IActionResult GetReceiptReprint(int branchid, string date_from, string date_to)
        {
            var receipts = _ikvms.GetReceiptReprint(branchid, date_from, date_to);
            return Ok(receipts);
        }

        public IActionResult GetReceiptCancel(int branchid, string date_from, string date_to)
        {
            var receipts = _ikvms.GetReceiptCancel(branchid, date_from, date_to);
            return Ok(receipts);
        }

        public IActionResult GetReceiptReturn(int branchid, string date_from, string date_to)
        {
            var receipts = _ikvms.GetReceiptReturn(branchid, date_from, date_to);
            return Ok(receipts);
        }

        public IActionResult GetReceiptReturns(string dateFrom, string dateTo, string keyword = "")
        {
            var receipts = _ikvms.GetReceiptReturn(GetBranchID(), dateFrom, dateTo);
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                receipts = receipts.Where(r => r.ReceiptNo.ToLowerInvariant()
                .Contains(keyword.Replace("\\+s", string.Empty), StringComparison.CurrentCultureIgnoreCase));
            }
            return Ok(receipts);
        }

        public IActionResult GetReceiptReturnDetail(int receiptId)
        {
            return Ok(GetReturnItems(receiptId));
        }

        List<ReturnItem> GetReturnItems(int receiptId)
        {
            var details = _context.ReceiptDetailKvms.Where(w => w.ReceiptKvmsID == receiptId && w.OpenQty > 0)
               .Select(r => new ReturnItem
               {
                   ID = r.ID,
                   ReceiptID = r.ReceiptKvmsID,
                   ItemID = r.ItemID,
                   Code = r.Code,
                   KhName = r.KhmerName,
                   UoM = r.UnitofMeansure.Name,
                   OpenQty = r.OpenQty,
                   ReturnQty = 0,
                   UserID = GetUserID()
               });
            return details.ToList();
        }

        public IActionResult GetGroupItem(int group1_id, int group2_id, int? level, int itemid, int pricelistid)
        {
            switch (level)
            {
                case 0:
                    return Ok(_ikvms.GetGroup1s);
                case 1:
                    return Ok(_ikvms.FilterGroup2(group1_id));
                case 2:
                    return Ok(_ikvms.FilterGroup3(group1_id, group2_id));
                case 3:
                    return Ok(_ikvms.FilterItem(pricelistid, itemid));
            }
            return Ok();
        }

        public IActionResult PriceListCurrecny(int pricelistID)
        {
            var pricelist = _context.PriceLists.FirstOrDefault(w => w.CurrencyID == pricelistID);
            var localcurrency = _context.ExchangeRates.Include(c => c.Currency).FirstOrDefault(w => w.CurrencyID == pricelist.CurrencyID);
            return Ok(localcurrency);
        }

        public IActionResult GetExchangeRate()
        {
            var exchage_rate = _context.ExchangeRates.Include(cur => cur.Currency).Where(w => w.Currency.Delete == false).ToList();
            return Ok(exchage_rate);
        }

        public IActionResult GetVat()
        {
            var vat = _context.Tax.Where(w => w.Delete == false);
            return Ok(vat);
        }

        public IActionResult GetMemberCard()
        {
            var Date = Convert.ToDateTime(DateTime.Today);
            return Ok(_context.MemberCards.Include(c => c.CardType).Where(w => w.Delete == false && w.ExpireDate >= Date).ToList());
        }

        public IActionResult GetUserInfo(int userid)
        {
            var user = _context.UserAccounts.Include(emp => emp.Employee).Where(w => w.ID == userid).ToList();
            return Ok(user);
        }

        [HttpGet]
        public IActionResult CheckOpenShift(int userid = 0)
        {
            if (userid == 0)
            {
                userid = GetUserID();
            }

            var hasOpenShift = _context.OpenShift.Any(w => w.UserID == userid && w.Open);
            return Ok(hasOpenShift);
        }

        [HttpGet]
        public IActionResult GetTimeByTable(int TableID)
        {
            var time = _ikvms.GetTimeByTable(TableID);
            return Ok(time);
        }

        [HttpGet]
        public IActionResult GetLocalRate(int localID)
        {
            var LocalID = _context.ExchangeRates.FirstOrDefault(w => w.CurrencyID == localID);
            return Ok(LocalID);
        }

        // Method get macaddress
        public string GetMacAddress(string ipAddress)
        {
            string macAddress = string.Empty;
            System.Diagnostics.Process pProcess = new System.Diagnostics.Process();
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

        public IActionResult CreateCustomer(string business)
        {
            BusinessPartner _partner = JsonConvert.DeserializeObject<BusinessPartner>(business);
            if (ModelState.IsValid)
            {
                var chkid = _context.BusinessPartners.FirstOrDefault(c => c.Code == _partner.Code);
                if (chkid == null)
                {
                    _context.BusinessPartners.Add(_partner);
                    _context.SaveChanges();
                    return Ok(_partner);
                }
                else
                {
                    _partner.ID = 0;
                    return Ok(_partner);
                }
            }
            return Ok(_partner);
        }

        public IActionResult SendSplit(Order data, Order addnew)
        {
            _ikvms.SendSplit(data, addnew);
            return Ok(data);
        }

        public IActionResult SendReturnItem(List<ReturnItem> returnItems)
        {
            var chkApplied = _context.ReceiptKvms.FirstOrDefault(c => c.ReceiptKvmsID == returnItems.FirstOrDefault().ReceiptID);

            if (chkApplied.Return == true)
            {
                string isReturned = "false";
                var _allowRQ = returnItems.Where(c => c.ReturnQty > 0).ToList();
                if (_allowRQ.Count() > 0)
                {
                    isReturned = _ikvms.SendReturnItem(_allowRQ);
                }
                return Ok(isReturned);
            }
            else
            {
                if (chkApplied.AppliedAmount > 0)
                {
                    return Ok("CancelF");
                }
                else
                {
                    string isReturned = "false";
                    var _allowRQ = returnItems.Where(c => c.ReturnQty > 0).ToList();
                    if (_allowRQ.Count() > 0)
                    {
                        isReturned = _ikvms.SendReturnItem(_allowRQ);
                    }
                    return Ok(isReturned);
                }
            }
        }

        public IActionResult SendDataToSecondScreen(Order data)
        {
            //_pos.SendDataToSecondScreen(data);
            return Ok();
        }

        public IActionResult PrintReceiptBill(int orderid, string print_type)
        {
            if (orderid > 0)
            {
                _ikvms.PrintReceiptBill(orderid, print_type);
            }
            return Ok();
        }

        public IActionResult PrintReceiptReprint(int orderid, string print_type)
        {
            if (orderid > 0)
            {
                _ikvms.PrintReceiptReprint(orderid, print_type);
            }
            return Ok();
        }

        public IActionResult CancelReceipt(int orderid, string print_type)
        {
            if (orderid > 0)
            {
                _ikvms.CancelReceipt(orderid);
            }
            return Ok();
        }

        public IActionResult OpenShift(int userid, double cash)
        {
            return Ok(_ikvms.OpenShiftData(userid, cash));
        }

        public IActionResult CloseShift(int userid, double cashout)
        {
            var data = _ikvms.CloseShiftData(userid, cashout);
            return Ok(data);
        }

        //Setting
        public IActionResult GetCustomer()
        {
            var customer = _context.BusinessPartners.Where(w => w.Type == "Customer" && w.Delete == false).ToList();
            return Ok(customer);
        }
        public IActionResult GetPriceList()
        {
            var pricelist = _context.PriceLists.Where(w => w.Delete == false).ToList();
            return Ok(pricelist);
        }
        public IActionResult GetPaymentMeans()
        {
            var paymentmeans = _context.PaymentMeans.Where(w => w.Delete == false).ToList();
            return Ok(paymentmeans);
        }
        public IActionResult GetWarehouse(int branchid)
        {
            var warehouse = _context.Warehouses.Where(w => w.BranchID == branchid && w.Delete == false).ToList();
            return Ok(warehouse);
        }
        public IActionResult GetPrinterName()
        {
            var pritner = _context.PrinterNames.Where(w => w.Delete == false);
            return Ok(pritner);
        }
        [HttpPost]
        public IActionResult UpdateSetting(GeneralSetting setting, string redirectUrl = "")
        {
            var ip = _accessor.ActionContext.HttpContext.Connection.RemoteIpAddress.ToString();
            var macAddress = ip;

            if (setting.ID == 0)
            {
                var branchid = Convert.ToInt32(@User.FindFirst("BranchID").Value);
                var setting_update = _context.GeneralSettings.FirstOrDefault(w => w.BranchID == branchid);
                if (setting_update != null)
                {
                    var currency_local = _context.PriceLists.FirstOrDefault(w => w.ID == setting.PriceListID);
                    var company = _context.Company.FirstOrDefault();
                    var currency_sys = _context.PriceLists.FirstOrDefault(w => w.ID == company.PriceListID);
                    var exchage = _context.ExchangeRates.FirstOrDefault(w => w.CurrencyID == currency_local.CurrencyID);
                    setting_update.SysCurrencyID = company.SystemCurrencyID;
                    setting_update.LocalCurrencyID = company.LocalCurrencyID;
                    setting_update.BranchID = Convert.ToInt32(@User.FindFirst("BranchID").Value);
                    setting_update.CompanyID = Convert.ToInt32(@User.FindFirst("CompanyID").Value);
                    setting_update.RateIn = exchage.Rate;
                    setting_update.RateOut = exchage.RateOut;
                    setting_update.CustomerID = setting.CustomerID;
                    setting_update.DaulScreen = setting.DaulScreen;
                    setting_update.PaymentMeansID = setting.PaymentMeansID;
                    setting_update.PriceListID = setting.PriceListID;
                    setting_update.PrintCountReceipt = setting.PrintCountReceipt;
                    setting_update.PrintReceiptOrder = setting.PrintReceiptOrder;
                    setting_update.PrintReceiptTender = setting.PrintReceiptTender;
                    setting_update.QueueCount = setting.QueueCount;
                    setting_update.Receiptsize = setting.Receiptsize;
                    setting_update.ReceiptTemplate = setting.ReceiptTemplate;
                    setting_update.VatAble = setting.VatAble;
                    setting_update.VatNum = setting.VatNum;
                    setting_update.WarehouseID = setting.WarehouseID;
                    setting_update.Wifi = setting.Wifi;
                    setting_update.MacAddress = macAddress;
                    setting_update.AutoQueue = setting.AutoQueue;
                    setting_update.PrintLabel = setting.PrintLabel;
                    setting_update.CloseShift = setting.CloseShift;
                    setting_update.UserID = GetUserID();
                    _context.GeneralSettings.Update(setting_update);
                    _context.SaveChanges();
                }
                else
                {
                    var currency_local = _context.PriceLists.FirstOrDefault(w => w.ID == setting.PriceListID);
                    var company = _context.Company.FirstOrDefault();
                    var currency_sys = _context.PriceLists.FirstOrDefault(w => w.ID == company.PriceListID);
                    var exchage = _context.ExchangeRates.FirstOrDefault(w => w.CurrencyID == currency_local.CurrencyID);
                    setting.SysCurrencyID = company.SystemCurrencyID;
                    setting.LocalCurrencyID = company.LocalCurrencyID;
                    setting.BranchID = Convert.ToInt32(@User.FindFirst("BranchID").Value);
                    setting.CompanyID = Convert.ToInt32(@User.FindFirst("CompanyID").Value);
                    setting.RateIn = exchage.Rate;
                    setting.RateOut = exchage.RateOut;
                    setting.MacAddress = macAddress;
                    setting.PrintLabel = setting.PrintLabel;
                    setting.CloseShift = setting.CloseShift;
                    setting.UserID = GetUserID();
                    _context.GeneralSettings.Add(setting);
                    _context.SaveChanges();

                }
            }
            else
            {
                var branchid = Convert.ToInt32(@User.FindFirst("BranchID").Value);
                var setting_update = _context.GeneralSettings.FirstOrDefault(w => w.BranchID == branchid);
                var currency_local = _context.PriceLists.FirstOrDefault(w => w.ID == setting.PriceListID);
                var company = _context.Company.FirstOrDefault();
                var currency_sys = _context.PriceLists.FirstOrDefault(w => w.ID == company.PriceListID);
                var exchage = _context.ExchangeRates.FirstOrDefault(w => w.CurrencyID == currency_local.CurrencyID);
                setting_update.SysCurrencyID = company.SystemCurrencyID;
                setting_update.LocalCurrencyID = company.LocalCurrencyID;
                setting_update.BranchID = Convert.ToInt32(@User.FindFirst("BranchID").Value);
                setting_update.CompanyID = Convert.ToInt32(@User.FindFirst("CompanyID").Value);
                setting_update.RateIn = exchage.Rate;
                setting_update.RateOut = exchage.RateOut;
                setting_update.CustomerID = setting.CustomerID;
                setting_update.DaulScreen = setting.DaulScreen;
                setting_update.PaymentMeansID = setting.PaymentMeansID;
                setting_update.PriceListID = setting.PriceListID;
                setting_update.PrintCountReceipt = setting.PrintCountReceipt;
                setting_update.PrintReceiptOrder = setting.PrintReceiptOrder;
                setting_update.PrintReceiptTender = setting.PrintReceiptTender;
                setting_update.QueueCount = setting.QueueCount;
                setting_update.Receiptsize = setting.Receiptsize;
                setting_update.ReceiptTemplate = setting.ReceiptTemplate;
                setting_update.VatAble = setting.VatAble;
                setting_update.VatNum = setting.VatNum;
                setting_update.WarehouseID = setting.WarehouseID;
                setting_update.Wifi = setting.Wifi;
                setting_update.MacAddress = macAddress;
                setting_update.AutoQueue = setting.AutoQueue;
                setting_update.PrintLabel = setting.PrintLabel;
                setting_update.CloseShift = setting.CloseShift;
                setting_update.UserID = GetUserID();
                _context.GeneralSettings.Update(setting_update);
                _context.SaveChanges();
            }

            if (string.IsNullOrEmpty(redirectUrl))
            {
                return RedirectToAction("GeneralSetting");
            }

            return Redirect(redirectUrl);
        }

        public IActionResult SetPrinterName(PrinterName printer)
        {
            if (printer.Name != null)
            {
#pragma warning disable CA1416 // Validate platform compatibility
                var machine = System.Security.Principal.WindowsIdentity.GetCurrent().Name.ToString();
#pragma warning restore CA1416 // Validate platform compatibility
                var printerName = _context.PrinterNames.Where(w => w.Name == printer.Name && w.Delete == false && w.MachineName == machine);
                if (printerName.Count() > 0)
                {
                    printerName.First().Name = printer.Name;
                    _context.PrinterNames.Update(printerName.First());
                    _context.SaveChanges();

                }
                else
                {
                    PrinterName addNew = new PrinterName
                    {
                        MachineName = machine,
                        Name = printer.Name
                    };
                    _context.PrinterNames.Add(addNew);
                    _context.SaveChanges();
                }
                return Ok();
            }
            return Ok();
        }

        public IActionResult GetUserPriviliges(int userid)
        {
            var privilige = _context.UserPrivilleges.Where(w => w.UserID == userid && w.Function.Type == "POS" && w.Delete == false);
            return Ok(privilige);

        }

        public IActionResult VoidOrder(int orderId)
        {
            if (orderId > 0)
            {
                var order = _context.Order.FirstOrDefault(w => w.OrderID == orderId);
                if (order.CheckBill == 'Y')
                {
                    return Ok("N");
                }
                else
                {
                    _ikvms.VoidOrder(orderId);
                    return Ok("Y");
                }
            }
            return Ok('N');
        }

        public string Encrypt(string clearText)
        {
            if (clearText != null)
            {
                string EncryptionKey = "MAKV2SPBNI99212";
                byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
                using (Aes encryptor = Aes.Create())
                {
                    Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                    encryptor.Key = pdb.GetBytes(32);
                    encryptor.IV = pdb.GetBytes(16);
                    using (MemoryStream ms = new MemoryStream())
                    {
                        using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                        {
                            cs.Write(clearBytes, 0, clearBytes.Length);
                            cs.Close();
                        }
                        clearText = Convert.ToBase64String(ms.ToArray());
                    }
                }
                return clearText;
            }
            else
            {
                return clearText;
            }

        }

        public IActionResult GetItemComment()
        {
            var comment = _context.ItemComment;
            return Ok(comment);
        }

        public IActionResult CreateItemComment(string Description)
        {
            ItemComment comment = new ItemComment
            {
                Description = Description
            };
            _context.Add(comment);
            _context.SaveChanges();
            return Ok(comment);
        }

        public IActionResult DeleteItemComment(int ID)
        {
            if (ID != 0)
            {
                var comment = _context.ItemComment.Find(ID);
                if (comment != null)
                {
                    _context.Remove(comment);
                    _context.SaveChanges();
                }
                return Ok(comment);
            }
            return Ok();
        }

        public IActionResult CheckSetting(int branckid)
        {
            var check = _context.GeneralSettings.FirstOrDefault(w => w.BranchID == branckid);
            return View(check);
        }

        //New Update by Mak Sokmanh
        int GetBranchID()
        {
            int.TryParse(User.FindFirst("BranchID").Value, out int branchId);
            return branchId;
        }

        int GetUserID()
        {
            int.TryParse(User.FindFirst("ID").Value, out int userId);
            return userId;
        }

        int GetCompanyID()
        {
            int.TryParse(User.FindFirst("CompanyID").Value, out int companyId);
            return companyId;
        }

        private GeneralSetting Setting
        {
            get
            {
                return _context.GeneralSettings.FirstOrDefault(w => w.UserID == GetUserID()) ?? new GeneralSetting();
            }
        }

        private PriceLists GetPriceList(int id = 0)
        {
            int pricelistId = id;
            if (id == 0)
            {
                pricelistId = Setting.PriceListID;
            }
            return _context.PriceLists.Include(p => p.Currency)
                     .FirstOrDefault(p => p.ID == pricelistId)
                     ?? new PriceLists
                     {
                         Currency = new Currency()
                     };
        }

        IEnumerable<UserPrivillege> GetUserPrivileges(int userId = 0)
        {
            if (userId == 0)
            {
                userId = GetUserID();
            }
            return _context.UserPrivilleges.Where(w => w.UserID == userId && w.Function.Type == "POS" && !w.Delete).ToList();
        }

        public IActionResult CheckPrivilege(string code)
        {
            ModelMessage message = new ModelMessage();
            var privilege = GetUserPrivileges().FirstOrDefault(p => string.Compare(p.Code, code) == 0);
            if (privilege == null || !privilege.Used)
            {
                message.Add(code, "You have no privilege to access this feature.");
            }
            else
            {
                message.Add(code, "Access granted.");
                message.Approve();
            }
            return Ok(new { Privilege = privilege, Message = message });
        }

        public IActionResult GetUserAccessAdmin(string username, string password, string code)
        {
            ModelMessage message = new ModelMessage();
            UserPrivillege privilege = new UserPrivillege();
            var userAccount = _context.UserAccounts.FirstOrDefault(u => u.Username == username && u.Password == Encrypt(password) && !u.Delete);
            if (userAccount == null)
            {
                message.Add(code, "Wrong username or password.");
            }
            else
            {
                privilege = GetUserPrivileges(userAccount.ID).FirstOrDefault(p => p.Code == code && !p.Delete);
                if (privilege == null || !privilege.Used)
                {
                    message.Add(code, "You have no privilege to access this feature.");
                }
                else
                {
                    message.Add(code, "Access granted.");
                    message.Approve();
                }
            }

            return Ok(new { Privilege = privilege, Message = message });
        }

        public List<Order> GetOrders(int tableId, int orderId = 0)
        {
            var orders = _context.Order.Where(o => o.TableID == tableId
                && o.UserOrderID == GetUserID() && !o.Cancel)
                .Include(o => o.OrderDetail).Include(o => o.Currency)
                .ToList();

            orders.ForEach(o =>
            {
                o.OrderDetail.ToList().ForEach(od =>
                {
                    od.ItemStatus = "old";
                    od.PrintQty = 0;
                });
                o.OrderDetail = (from od in o.OrderDetail
                                 join uom in _context.GroupDUoMs on od.UomID equals uom.AltUOM
                                 select od).ToList();
            });
            return orders;
        }

        public SettingModel GetSettingViewModel(string redirectUrl = "")
        {
            var customers = _context.BusinessPartners.Where(c => !c.Delete).ToList();
            var priceLists = _context.PriceLists.Where(p => !p.Delete).ToList();
            var warehouses = _context.Warehouses.Where(w => w.BranchID == GetBranchID() && !w.Delete);
            var systemTypes = _context.SystemType.Where(s => s.Status).ToList();
            var paymentMeans = _context.PaymentMeans.Where(p => !p.Delete).ToList();
            return new SettingModel
            {
                Setting = Setting,
                Customers = new SelectList(customers, "ID", "Name", Setting.CustomerID),
                PriceLists = priceLists.Select(p => new SelectListItem
                {
                    Value = p.ID.ToString(),
                    Text = p.Name,
                    Selected = p.ID == Setting.PriceListID
                    //Disabled = p.ID != Setting.PriceListID && _context.Order.Where(o => !o.Delete).Count() > 0
                }).ToList(),
                Warehouses = new SelectList(warehouses, "ID", "Name", Setting.WarehouseID),
                PaymentMeans = new SelectList(paymentMeans, "ID", "Type", Setting.PaymentMeansID),
                RedirectUrl = redirectUrl
            };
        }

        public IActionResult GetGeneralSetting()
        {
            return Ok(Setting);
        }
        public IActionResult GetReceiptsToCombine(int orderId)
        {
            var orders = _context.Order.Where(w => w.OrderID != orderId && w.BranchID == GetBranchID()).OrderBy(by => by.TableID).ToList();
            if (orders.Count > 0)
            {
                var receipts = orders.Select(o => new
                {
                    o.OrderID,
                    o.TableID,
                    ReceiptNote = string.Format($"{_context.Tables.Find(o.TableID).Name} #{ o.OrderNo }")
                });
                return Ok(receipts);
            }
            return Ok();
        }
        DisplayCurrencyModel GetDisplayCurrency(int priceListId)
        {
            PriceLists priceList = GetPriceList(priceListId);
            var displayCurrency = (from c in _context.PriceLists
                                   join dc in _context.DisplayCurrencies on c.ID equals dc.AltCurrencyID
                                   where dc.PriceListID == priceList.ID
                                   select new DisplayCurrencyModel
                                   {
                                       ID = c.ID,
                                       BaseCurrencyID = priceList.CurrencyID,
                                       AltCurrencyID = dc.AltCurrencyID,
                                       BaseCurrency = priceList.Currency.Description,
                                       AltCurrency = c.Currency.Description,
                                       Rate = dc.DisplayRate
                                   }).FirstOrDefault() ?? new DisplayCurrencyModel();
            return displayCurrency;
        }
        public List<OrderDetail> GetOrderDetailsByOrderId(int tableId, int orderId = 0)
        {
            if (orderId == 0)
            {
                var __orders = _context.Order.Where(w => w.TableID == tableId && !w.Cancel);
                if (__orders.Count() > 0)
                {
                    orderId = __orders.Max(o => o.OrderID);
                }
            }

            var itemOrderDetails = from od in _context.OrderDetail.Where(od => od.OrderID == orderId)
                                   join uom in _context.UnitofMeasures on od.UomID equals uom.ID
                                   select od;
            return itemOrderDetails.ToList();
        }
        public IActionResult FindSaleItemByBarcode(string keyword)
        {
            var saleItem = _ikvms.GetItemMasterByBarcode(Setting.PriceListID, keyword).FirstOrDefault();
            return Ok(saleItem);
        }
        public IActionResult SearchSingleItems(string keyword, int orderId = 0)
        {
            int pricelistId = Setting.PriceListID;
            if (orderId > 0)
            {
                var order = _context.OrderQAutoMs.Find(orderId);
                if (order != null)
                {
                    pricelistId = order.PriceListID;
                }
            }
            var saleItems = _ikvms.GetItemMasterDatas(pricelistId);
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                StringComparison ignoreCase = StringComparison.InvariantCultureIgnoreCase;
                keyword = RawWord(keyword);
                saleItems = saleItems.Where(s => RawWord(s.KhmerName).Contains(keyword, ignoreCase)
                        || RawWord(s.EnglishName).Contains(keyword, ignoreCase)
                        || RawWord(s.Barcode).Contains(keyword, ignoreCase));
            }
            return Ok(saleItems);
        }

        string RawWord(string word)
        {
            return Regex.Replace(word, "\\s+", "", RegexOptions.IgnoreCase);
        }
        public IActionResult GetServiceTables()
        {
            return Ok(GetServiceTable());
        }
        public ServiceTable GetServiceTable()
        {
            int branchid = GetBranchID();
            List<GroupTable> groupTables = _context.GroupTables.Where(w => !w.Delete && w.BranchID == branchid).ToList();
            List<Table> ls_table = new List<Table>();
            foreach (var item in groupTables.ToList())
            {
                ls_table.AddRange(GetTablesByGroupId(item.ID));
            }
            ServiceTable serviceTable = new ServiceTable
            {
                GroupTables = groupTables,
                Tables = ls_table
            };
            return serviceTable;
        }
        public IActionResult SearchTables(string keyword = "", bool onlyFree = false)
        {
            var tables = _context.Tables.Where(t => !t.Delete && t.GroupTable.BranchID == GetBranchID());
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                keyword = Regex.Replace(keyword, "\\s+", string.Empty);
                StringComparison ignoreCase = StringComparison.InvariantCultureIgnoreCase;
                tables = tables.Where(t => Regex.Replace(t.Name, "\\s+", string.Empty).Contains(keyword, ignoreCase)
                        || string.Compare(t.Status.ToString(), keyword, ignoreCase) == 0);
            }

            if (onlyFree)
            {
                return Ok(tables.Where(t => t.Status == 'A'));
            }
            return Ok(tables);
        }
        public IActionResult GetTableById(int tableId)
        {
            var table = _context.Tables.FirstOrDefault(t => t.ID == tableId
                && !t.Delete && t.GroupTable.BranchID == GetBranchID());
            return Ok(table);
        }
        [HttpPost]
        public IActionResult MoveTable(int previousId, int currentId)
        {
            _ikvms.MoveTable(previousId, currentId);
            var currentTable = _context.Tables.Find(currentId);
            return Ok(currentTable);
        }
        public IActionResult FilterTablesByGroup(int groupId = 0)
        {
            return Ok(GetTablesByGroupId(groupId));
        }
        IEnumerable<GroupTable> GetGroupTables()
        {
            return _context.GroupTables.Where(g => !g.Delete && g.BranchID == GetBranchID()).ToList();
        }
        List<Table> GetTablesByGroupId(int groupId = 0)
        {
            var tables = _context.Tables.Where(t => !t.Delete).ToList();
            if (groupId > 0)
            {
                tables = tables.Where(t => t.GroupTableID == groupId).ToList();
            }
            return tables;
        }
        public IActionResult GetItemsByGroup(int level, int group1, int group2, int group3, int orderId = 0)
        {
            int pricelistId = Setting.PriceListID;
            if (orderId > 0)
            {
                var order = _context.Order.Find(orderId);
                if (order != null)
                {
                    pricelistId = order.PriceListID;
                }
            }

            var items = _ikvms.GetItemMasterDatas(pricelistId).ToList();
            switch (level)
            {
                case 1:
                    return Ok(items.Where(i => i.Group1 == group1));
                case 2:
                    return Ok(items.Where(i => i.Group1 == group1 && i.Group2 == group2));
                case 3:
                    return Ok(items.Where(i => i.Group1 == group1 && i.Group2 == group2 && i.Group3 == group3));
            }
            return Ok(items);
        }
        public IActionResult FindItemInGroup(int id)
        {
            var items = _ikvms.GetItemMasterDatas(Setting.PriceListID);
            var item = items.FirstOrDefault(i => i.ID == id);
            var saleItem = new SaleItemsByGroup
            {
                Group1 = _context.ItemGroup1.Find(item.Group1),
                Group2 = _context.ItemGroup2.Find(item.Group2),
                Group3 = _context.ItemGroup3.Find(item.Group3),
                Item = item,
                ItemsInGroup = items.Where(i => i.Group1 == item.Group1 && i.Group2 == item.Group2 && i.Group3 == item.Group3)
            };
            return Ok(saleItem);
        }
        public IActionResult GetDisplayRateTemplate(int priceListId = 0)
        {
            var displayCurrencies = _context.PriceLists.Where(p => !p.Delete).Select(p => new
            {
                p.ID,
                Name = p.Currency.Description + ((priceListId == p.ID) ? "<i class='fn-red'>*</i>" : ""),
                Currencies = _context.Currency.Select(c => new SelectListItem
                {
                    Value = c.ID.ToString(),
                    Text = c.Description,
                    Selected = _context.DisplayCurrencies.Any(dc => dc.ID == p.ID && dc.AltCurrencyID == c.ID)
                }),
                Rate = _context.DisplayCurrencies.Any(dc => dc.ID == p.ID) ?
                _context.DisplayCurrencies.FirstOrDefault(dc => dc.ID == p.ID).DisplayRate : 1.00M,
                DisplayCurrency = _context.DisplayCurrencies.FirstOrDefault(dc => dc.ID == p.ID) ??
                new DisplayCurrency
                {
                    PriceListID = p.ID,
                    AltCurrencyID = p.CurrencyID,
                    DisplayRate = 1
                }
            });

            return Ok(displayCurrencies);
        }
        [HttpPost]
        public IActionResult SaveDisplayCurrencies(IEnumerable<DisplayCurrency> displayCurrencies)
        {
            ModelMessage message = new ModelMessage();
            if (ModelState.IsValid)
            {
                _context.DisplayCurrencies.UpdateRange(displayCurrencies);
                _context.SaveChanges();
                message.Approve();
                message.Add("DisplayCurrency", "Data has been saved successfully.");
            }
            return Ok(message.Bind(ModelState));
        }
        public IActionResult GetShiftTemplate()
        {
            List<ShiftForm> shiftForms = new List<ShiftForm>();
            string sysCurrency = "";
            foreach (var xr in GetExchangeRates())
            {
                if (xr.CurrencyID == Setting.SysCurrencyID)
                {
                    sysCurrency = xr.Currency.Description;
                }
                shiftForms.Add(new ShiftForm
                {
                    ID = xr.CurrencyID,
                    Decription = "Cash",
                    InputCash = 0,
                    Currency = xr.Currency.Description,
                    RateIn = xr.Rate
                });
            }
            return Ok(new { ShiftForms = shiftForms, SystemCurrency = sysCurrency });
        }
        [HttpPost]
        public IActionResult ProcessOpenShift(double total)
        {
            ModelMessage message = new ModelMessage();
            if (ModelState.IsValid)
            {
                _ikvms.OpenShiftData(GetUserID(), total);
                message.Approve();
                return Ok(message.Bind(ModelState));
            }
            return Ok(message.Bind(ModelState));
        }
        [HttpPost]
        public IActionResult ProcessCloseShift(double total)
        {
            ModelMessage message = new ModelMessage();
            if (ModelState.IsValid)
            {
                _ikvms.CloseShiftData(GetUserID(), total);
                message.Approve();
                return Ok(message.Bind(ModelState));
            }
            return Ok(message.Bind(ModelState));
        }
        IEnumerable<ExchangeRate> GetExchangeRates()
        {
            return _context.ExchangeRates.Include(cur => cur.Currency).Where(w => !w.Currency.Delete).ToList();
        }
        IEnumerable<ReceiptSummary> MapReceiptSummaries(IEnumerable<Receipt> receipts)
        {
            receipts = receipts ?? new List<Receipt>();
            List<ReceiptSummary> reprintReceipts = new List<ReceiptSummary>();
            if (receipts.Count() > 0)
            {
                foreach (Receipt r in receipts)
                {
                    reprintReceipts.Add(new ReceiptSummary
                    {
                        ReceiptID = r.ReceiptID,
                        ReceiptNo = r.ReceiptNo,
                        Cashier = r.UserAccount.Employee.Name,
                        DateOut = r.DateOut.ToString("MM-dd-yyyy"),
                        TimeOut = DateTime.Parse(r.TimeOut).ToString("hh:mm tt"),
                        TableName = r.Table.Name,
                        GrandTotal = string.Format($"{r.Currency.Description} {r.GrandTotal:N3}"),
                    });
                }
            }
            return reprintReceipts;
        }
        public IActionResult GetReceiptReprints(string dateFrom, string dateTo, string keyword = "")
        {
            var receipts = MapReceiptSummaries(_ikvms.GetReceiptReprint(GetBranchID(), dateFrom, dateTo));
            StringComparison ignoreCase = StringComparison.InvariantCultureIgnoreCase;
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                keyword = RawWord(keyword);
                receipts = receipts.Where(r =>
                RawWord(r.ReceiptNo).Contains(keyword, ignoreCase)
                || RawWord(r.Cashier).Contains(keyword, ignoreCase)
                || RawWord(r.TableName).Contains(keyword, ignoreCase)
                || RawWord(r.GrandTotal).Contains(keyword, ignoreCase));
            }
            return Ok(receipts);
        }
    }
}
