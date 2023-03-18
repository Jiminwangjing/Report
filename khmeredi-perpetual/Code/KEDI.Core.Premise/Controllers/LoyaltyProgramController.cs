using CKBS.AppContext;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using CKBS.Models.Services.Inventory;
using Newtonsoft.Json;
using KEDI.Core.Models.Validation;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using KEDI.Core.Premise.Models.Services.LoyaltyProgram.BuyXGetX;
using KEDI.Core.Premise.Authorization;
using KEDI.Core.Premise.Models.Services.LoyaltyProgram.PromoCode;
using System.Text;
using KEDI.Core.Premise.Repository;
using KEDI.Core.Security;
using CKBS.Models.Services.Production;
using CKBS.Models.Services.LoyaltyProgram.ComboSale;
using CKBS.Models.Services.LoyaltyProgram.BuyXAmountGetXDiscount;
using CKBS.Models.Services.Inventory.PriceList;
using KEDI.Core.Premise.Models.ServicesClass.BuyXGetX;
using CKBS.Models.Services.Administrator.Inventory;
using CKBS.Models.Services.Administrator.General;
using KEDI.Core.Premise.Models.ServicesClass.ClearPoint;
using CKBS.Models.Services.HumanResources;

namespace KEDI.Core.Premise.Controllers
{
    [Privilege]
    public class LoyaltyProgramController : Controller
    {
        private readonly DataContext _context;
        readonly PosRetailModule _posRetail;
        private readonly Random _random = new((int)DateTime.Now.Ticks);
        readonly Dictionary<int, string> DisType = new()
        {
                { (int)TypeDiscountBuyXAmountGetXDiscount.Rate, TypeDiscountBuyXAmountGetXDiscount.Rate.ToString()},
                { (int)TypeDiscountBuyXAmountGetXDiscount.Value, TypeDiscountBuyXAmountGetXDiscount.Value.ToString()},
            };
        public LoyaltyProgramController(DataContext context, PosRetailModule posRetail)
        {
            _context = context;
            _posRetail = posRetail;
        }

        [Privilege("JE005")]
        public IActionResult BuyXGetX()
        {
            ViewBag.BuyXGetX = "highlight";
            return View();
        }

        [Privilege("JE005")]
        public IActionResult BuyXGetXCreate()
        {
            ViewBag.BuyXGetX = "highlight";
            ViewBag.PriceLists = new SelectList(_context.PriceLists.Where(i => !i.Delete), "ID", "Name");
            return View(new BuyXGetXModel
            {
                BuyXGetX = new BuyXGetX(),
                BuyXGetXDetails = new List<BuyXGetXDetail>(),

            });
        }

        [Privilege("JE005")]
        public IActionResult BuyXGetXEdit(int Id)
        {
            ViewBag.BuyXGetX = "highlight";
            var prods = new List<BuyXGetXDetailModel>();
            var pds = _context.BuyXGetXDetails.Where(i => i.BuyXGetXID == Id && !i.Delete).ToList();
            foreach (var _pd in pds)
            {
                var BuyUoms = (from im in _context.ItemMasterDatas.Where(i => i.ID == _pd.BuyItemID)
                               join GDU in _context.GroupDUoMs on im.GroupUomID equals GDU.GroupUoMID
                               join UNM in _context.UnitofMeasures on GDU.AltUOM equals UNM.ID
                               select new ItemUoMView
                               {
                                   ID = UNM.ID,
                                   Name = UNM.Name,
                                   KhmerName = im.KhmerName,
                                   Code = im.Code,
                               }).ToList();

                var GetUoms = (from i in _context.ItemMasterDatas.Where(i => i.ID == _pd.GetItemID)
                               join GDU in _context.GroupDUoMs on i.GroupUomID equals GDU.GroupUoMID
                               join UNM in _context.UnitofMeasures on GDU.AltUOM equals UNM.ID
                               select new ItemUoMView
                               {
                                   ID = UNM.ID,
                                   Name = UNM.Name,
                                   KhmerName = i.KhmerName,
                                   Code = i.Code,
                               }).ToList();
                if (BuyUoms.Count == 0)
                {
                    BuyUoms.Add(new ItemUoMView
                    {
                        Name = "",
                        KhmerName = "",
                        Code = ""
                    });

                }
                if (GetUoms.Count == 0)
                {
                    GetUoms.Add(new ItemUoMView
                    {
                        Name = "",
                        KhmerName = "",
                        Code = ""
                    });
                }
                prods.Add(new BuyXGetXDetailModel
                {
                    LineID = _pd.LineID,
                    ID = _pd.ID,
                    BuyItemID = _pd.BuyItemID,
                    ProCode = _pd.Procode,
                    BuyItemName = BuyUoms.FirstOrDefault().KhmerName,
                    ItemCode = BuyUoms.FirstOrDefault().Code,
                    BuyQty = _pd.BuyQty,
                    ItemUomID = _pd.ItemUomID,
                    ItemUoMs = BuyUoms.Select(i => new SelectListItem
                    {
                        Value = i.ID.ToString(),
                        Text = i.Name,
                        Selected = i.ID == _pd.ItemUomID
                    }).ToList(),
                    GetItemID = _pd.GetItemID,
                    GetItemName = GetUoms.FirstOrDefault().KhmerName,
                    GetItemCode = GetUoms.FirstOrDefault().Code,
                    GetQty = _pd.GetQty,
                    GetUomID = _pd.GetUomID,
                    PromoUoMs = GetUoms.Select(i => new SelectListItem
                    {
                        Value = i.ID.ToString(),
                        Text = i.Name,
                        Selected = i.ID == _pd.GetUomID
                    }).ToList(),
                    BuyXGetXID = _pd.BuyXGetXID
                });
            }
            ViewBag.PriceLists = new SelectList(_context.PriceLists.Where(i => !i.Delete), "ID", "Name");
            var BuyXGetXModel = new BuyXGetXModel()
            {
                BuyXGetX = _context.BuyXGetXes.Find(Id) ?? new BuyXGetX(),
                BuyXGetXDetailModelView = prods,
            };
            return View(BuyXGetXModel);
        }

        [Privilege("JE005")]
        public IActionResult PromoCodeCreate()
        {
            ViewBag.PromoCode = "highlight";
            var setup = new PromoSetup
            {
                SelectPriceList = _posRetail.SelectPriceLists()
            };
            return View(setup);
        }

        [Privilege("JE005")]
        public IActionResult PromoCode()
        {
            ViewBag.PromoCode = "highlight";
            return View();
        }

        static string RawWord(string keyword)
        {
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                return Regex.Replace(keyword, "\\+s", string.Empty);
            }
            return string.Empty;
        }

        public IActionResult GetBuyXGetX(string keyword = "")
        {
            var promo = from bo in _context.BuyXGetXes
                        join pr in _context.PriceLists on bo.PriListID equals pr.ID
                        select new BuyXGetXView
                        {
                            ID = bo.ID,
                            Code = bo.Code,
                            Name = bo.Name,
                            PriceList = pr.Name,
                            DateF = bo.DateF.ToString("dd/MM/yyyy"),
                            DateT = bo.DateT.ToString("dd/MM/yyyy"),
                            StartTime = bo.DateF.ToString("hh:mm tt"),
                            StopTime = bo.DateT.ToString("hh:mm tt"),
                        };

            if (!string.IsNullOrWhiteSpace(keyword))

            {
                keyword = RawWord(keyword);
                StringComparison ignoreCase = StringComparison.InvariantCultureIgnoreCase;
                promo = promo.Where(c => RawWord(c.Name).Contains(keyword, ignoreCase));
            }
            return Ok(promo.ToList());
        }

        public IActionResult GetBuyXGetXDetails(int plId)
        {
            return Ok(CreateBuyXGetXDetails(plId));

        }

        private List<BuyXGetXDetailModel> CreateBuyXGetXDetails(int plId)
        {
            List<BuyXGetXDetailModel> details = new();
            var items = (from imd in _context.ItemMasterDatas.Where(_im => !_im.Delete && _im.PriceListID == plId)
                         join p in _context.PriceLists on imd.PriceListID equals p.ID
                         join uom in _context.GroupUOMs.Where(_u => !_u.Delete) on imd.GroupUomID equals uom.ID
                         select new BuyXGetXDetailModel
                         {
                             LineID = DateTime.Now.Ticks.ToString(),
                             BuyItemID = imd.ID,
                             ItemCode = imd.Code,
                             ItemName = imd.KhmerName,
                             UoM = uom.Name,
                             PromoUoMs = (from guom in _context.GroupDUoMs.Where(i => i.GroupUoMID == imd.GroupUomID)
                                          join _uom in _context.UnitofMeasures on guom.AltUOM equals _uom.ID
                                          select new SelectListItem
                                          {
                                              Value = _uom.ID.ToString(),
                                              Text = _uom.Name
                                          }).ToList(),
                         }).ToList();
            items.ForEach(im =>
            {
                details.Add(new BuyXGetXDetailModel
                {
                    LineID = DateTime.Now.Ticks.ToString(),
                    BuyItemID = im.BuyItemID,
                    ItemCode = im.ItemCode,
                    ItemName = im.ItemName,
                    UoM = im.UoM,
                    GetUomID = Convert.ToInt32(im.PromoUoMs.FirstOrDefault().Value),
                    PromoUoMs = im.PromoUoMs,

                });
            });
            return details;
        }

        private IActionResult GetBuyXGetXDetail(int itemId)
        {
            var item = _context.ItemMasterDatas.Find(itemId) ?? new ItemMasterData();
            BuyXGetXDetailModel detail = new()
            {
                BuyItemID = item.ID,
                ItemName = item.KhmerName,
            };
            return Ok(detail);
        }

        public IActionResult CreateBuyXGetXModel(int itemId)
        {
            var uoms = (from im in _context.ItemMasterDatas.Where(i => i.ID == itemId)
                        join gdu in _context.GroupDUoMs.Where(gdu => !gdu.Delete)
                        on im.GroupUomID equals gdu.GroupUoMID
                        join um in _context.UnitofMeasures.Where(um => !um.Delete)
                        on gdu.AltUOM equals um.ID
                        select new ItemUoMView
                        {
                            ID = um.ID,
                            Name = um.Name,

                        }).ToList();
            var items = (from i in _context.ItemMasterDatas
                         select new BuyXGetXDetailModel
                         {
                             LineID = DateTime.Now.Ticks.ToString(),
                             BuyItemID = i.ID,
                             ProCode = "",
                             ItemCode = i.Code,
                             BuyItemName = i.KhmerName,
                             BuyQty = 0,
                             ItemUoMs = uoms.Select(i => new SelectListItem
                             {
                                 Value = i.ID.ToString(),
                                 Text = i.Name
                             }).ToList(),
                             ItemUomID = uoms.FirstOrDefault().ID,
                             Item = "",
                             GetItemCode = "",
                             GetItemName = "",
                             GetQty = 0,

                         }).FirstOrDefault(i => i.BuyItemID == itemId);

            return Ok(items ?? new BuyXGetXDetailModel());
        }

        private void ValidateSummary(BuyXGetX master, List<BuyXGetXDetail> BuyXGetXDetails, int id)
        {
            if (master.Name == null || master.Name == "")
            {
                ModelState.AddModelError("Name", "Name is required!");
            }
            else if (master.Code == null || master.Code == "")
            {
                ModelState.AddModelError("Code", "Code is required!");
            }
            else
            {
                /// Checking Update
                if (id > 0)
                {
                    var promocode = _context.BuyXGetXes.AsNoTracking().FirstOrDefault(_id => _id.ID == id);

                    if (promocode.Code != master.Code)
                    {
                        var checkpromocode = _context.BuyXGetXes.FirstOrDefault(w => w.Code == master.Code);
                        if (checkpromocode != null)
                        {
                            ModelState.AddModelError("Code", "Code have already!");
                        }
                    }
                }
                /// Checking Create
                if (id == 0)
                {
                    var checkpromocode = _context.BuyXGetXes.FirstOrDefault(w => w.Code == master.Code);
                    if (checkpromocode != null)
                    {
                        ModelState.AddModelError("Code", "Code have already!");
                    }
                }
            }

            /// Checking Create
            if (id == 0)
            {
                var buy = BuyXGetXDetails.GroupBy(i => i.Procode).ToList();
                if (buy.Count != BuyXGetXDetails.Count)
                {
                    ModelState.AddModelError("Procode", "ProCode have already!");
                }
            }
            foreach (var pro in BuyXGetXDetails)
            {
                if (id == 0)
                {
                    var list = _context.BuyXGetXDetails.Any(i => i.Procode == pro.Procode);
                    if (list)
                    {
                        ModelState.AddModelError("Procode", "ProCode have already!");
                    }
                }
                //Check Edit
                if (id > 0)
                {
                    var promocode = _context.BuyXGetXDetails.AsNoTracking().FirstOrDefault(_id => _id.ID == pro.ID);

                    if (promocode != null)
                    {
                        if (promocode.Procode != pro.Procode)
                        {
                            var checkprocode = _context.BuyXGetXDetails.Any(w => w.Procode == pro.Procode);
                            if (checkprocode)
                            {
                                ModelState.AddModelError("Procode", "ProCode have already!");
                            }
                        }
                    }
                }

                if (string.IsNullOrWhiteSpace(pro.Procode))
                {
                    ModelState.AddModelError("Procode", "ProCode is required!");
                }

                else if (pro.BuyQty <= 0)
                {
                    ModelState.AddModelError("BuyQty", "BuyQty is required!");
                }
                else if (pro.GetItemID == 0)
                {
                    ModelState.AddModelError("GetItemID", "GetItem is required!");
                }
                else if (pro.GetQty <= 0)
                {
                    ModelState.AddModelError("GetQty", "GetQty is required!");
                }

            }
        }
        private void ValidateSummary(PBuyXAmountGetXDis buyxamountgetxdis, int index)
        {
            var bxagxd = _context.PBuyXAmountGetXDis.AsNoTracking().FirstOrDefault(i => i.ID == buyxamountgetxdis.ID);
            var bxagxds = _context.PBuyXAmountGetXDis.AsNoTracking().ToList();
            if (buyxamountgetxdis.Name == "")
            {
                ModelState.AddModelError("Name", "Please input name");
            }
            if (buyxamountgetxdis.Code == "")
            {
                ModelState.AddModelError("Code", "Please input Code");
            }
            if (buyxamountgetxdis.ID == 0)
            {
                if (bxagxds.Any(i => i.Code == buyxamountgetxdis.Code))
                {
                    ModelState.AddModelError("Code", $"At line \"{index}\" Code \"{buyxamountgetxdis.Code}\" is already existed");
                }
            }
            if (buyxamountgetxdis.ID > 0)
            {
                if (buyxamountgetxdis.Code != bxagxd.Code)
                {
                    if (bxagxds.Any(i => i.Code == buyxamountgetxdis.Code))
                    {
                        ModelState.AddModelError("Code", $"At line \"{index}\" Code \"{buyxamountgetxdis.Code}\" is already existed");
                    }
                }
            }
            if (buyxamountgetxdis.Amount <= 0)
            {
                ModelState.AddModelError("Amount", "Please input Amount");
            }

            if (buyxamountgetxdis.DisRateValue <= 0)
            {
                ModelState.AddModelError("DisRate", "Please Input DisRate");
            }
            if (buyxamountgetxdis.PriListID <= 0)
            {
                ModelState.AddModelError("PriceList", "Please Input DisRate");
            }
        }

        public IActionResult SubmitPromotion(string data)
        {
            BuyXGetXModel BuyXGetXModel = JsonConvert.DeserializeObject<BuyXGetXModel>(data, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });
            ModelMessage msg = new();
            ValidateSummary(BuyXGetXModel.BuyXGetX, BuyXGetXModel.BuyXGetXDetails, BuyXGetXModel.BuyXGetX.ID);
            using (var t = _context.Database.BeginTransaction())
            {
                if (ModelState.IsValid)
                {
                    BuyXGetXModel.BuyXGetX.BuyXGetXDetails = BuyXGetXModel.BuyXGetXDetails;
                    var buyx = _context.BuyXGetXes.AsNoTracking().ToList();
                    foreach (var value in buyx)
                    {
                        if (value.ID == BuyXGetXModel.BuyXGetX.ID)
                        {
                            value.Active = BuyXGetXModel.BuyXGetX.Active;
                        }
                        else
                        {
                            value.Active = false;
                            _context.Update(value);
                            _context.SaveChanges();
                        }
                    }
                    _context.Update(BuyXGetXModel.BuyXGetX);
                    _context.SaveChanges();
                    t.Commit();
                    ModelState.AddModelError("success", "Item save successfully.");
                    msg.Approve();
                }
            }
            return Ok(msg.Bind(ModelState));
        }

        [HttpPost]
        public async Task<int> DeleteItem(int? itemId)
        {
            var item = await _context.BuyXGetXDetails.FindAsync(itemId);
            if (item != null)
            {
                item.Delete = true;
                _context.BuyXGetXDetails.Update(item);
            }
            return await _context.SaveChangesAsync();
        }

        public IActionResult GetPriceList()
        {
            var pricelist = _context.PriceLists.Where(w => !w.Delete).ToList();
            return Ok(pricelist);
        }

        public IActionResult GetPromoCode(bool isUsed, string keyword = "")
        {
            var promo = from bo in _context.PromoCodeDiscounts.Where(s => s.Used == isUsed)
                        join pr in _context.PriceLists.Where(s => !s.Delete)
                        on bo.PriceListID equals pr.ID
                        select new
                        {
                            bo.ID,
                            bo.Code,
                            bo.Name,
                            DateF = bo.DateF.ToString("dd/MM/yyyy"),
                            bo.TimeF,
                            DateT = bo.DateT.ToString("dd/MM/yyyy"),
                            bo.TimeT,
                            bo.PromoValue,
                            PriceList = pr.Name,
                            bo.PromoCount,
                            bo.UseCountCode,
                            stringCount = bo.StringCount,
                            bo.Active,
                        };
            if (!string.IsNullOrWhiteSpace(keyword))

            {
                keyword = RawWord(keyword);
                StringComparison ignoreCase = StringComparison.InvariantCultureIgnoreCase;
                promo = promo.Where(c => RawWord(c.Name).Contains(keyword, ignoreCase));
            }
            return Ok(promo.ToList());
        }
        [HttpPost]
        public IActionResult SubmitPromoCode(PromoSetup promoSetup)
        {
            ModelMessage msg = new();
            ValidateSummary(promoSetup.PromoCodeDiscount, promoSetup.PromoCodeDiscount.ID);
            using (var t = _context.Database.BeginTransaction())
            {
                if (ModelState.IsValid)
                {
                    promoSetup.PromoCodeDiscount = promoSetup.PromoCodeDiscount;
                    var count = promoSetup.PromoCodeDiscount.PromoCount;
                    // edit update master
                    if (_context.PromoCodeDiscounts.Any(s => s.ID == promoSetup.PromoCodeDiscount.ID))
                    {
                        _context.PromoCodeDiscounts.Update(promoSetup.PromoCodeDiscount);
                        _context.SaveChanges();
                    }
                    // add master and Detail
                    else
                    {
                        _context.PromoCodeDiscounts.Update(promoSetup.PromoCodeDiscount);
                        _context.SaveChanges();
                        for (var i = 0; i < count; i++)
                        {
                            var size = promoSetup.PromoCodeDiscount.StringCount;
                            var builder = new StringBuilder(size);
                            const string lettersOffset = "abcdefghijklmnopqrstuvwyxzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
                            for (int j = 0; j < size; j++)
                            {
                                var @char = lettersOffset[_random.Next(lettersOffset.Length)];
                                builder.Append(@char);
                            }
                            PromoCodeDetail promoCodeDetail = new()
                            {
                                ID = promoSetup.PromoCodeDiscount.ID,
                                PromoCode = builder.ToString(),
                                MaxUse = promoSetup.PromoCodeDiscount.UseCountCode,
                            };
                            if (_context.PromoCodeDetails.Any(pc => pc.PromoCode == promoCodeDetail.PromoCode))
                            {
                                promoCodeDetail.PromoCode = LicenseFactory.GenerateKey(size);
                            }
                            _context.PromoCodeDetails.Add(promoCodeDetail);
                            _context.SaveChanges();
                        }
                    }
                    t.Commit();
                    ModelState.AddModelError("success", "Item save successfully.");
                    msg.Approve();
                }
            }
            return Ok(msg.Bind(ModelState));

        }
        private void ValidateSummary(PromoCodeDiscount master, int Id)
        {
            if (string.IsNullOrEmpty(master.Name))
            {
                ModelState.AddModelError("Name", "Name is required!");
            }
            else if (string.IsNullOrEmpty(master.Code))
            {
                ModelState.AddModelError("Code", "Code is required!");
            }
            else
            {
                /// Checking Update
                if (Id > 0)
                {
                    var promocode = _context.PromoCodeDiscounts.AsNoTracking().FirstOrDefault(_id => _id.ID == Id);

                    if (promocode.Code != master.Code)
                    {
                        var checkpromocode = _context.PromoCodeDiscounts.FirstOrDefault(w => w.Code == master.Code);
                        if (checkpromocode != null)
                        {
                            ModelState.AddModelError("Code", "Code have already!");
                        }
                    }
                }
                /// Checking Create
                if (Id == 0)
                {
                    var checkpromocode = _context.PromoCodeDiscounts.FirstOrDefault(w => w.Code == master.Code);
                    if (checkpromocode != null)
                    {
                        ModelState.AddModelError("Code", "Code have already!");
                    }
                }
            }
            if (master.PromoCount == 0)
            {
                ModelState.AddModelError("PromoCount", "PromoCount is required!");
            }
            else if (master.StringCount == 0)
            {
                ModelState.AddModelError("StringCount", "StringCount is required!");
            }
            else if (master.StringCount < 4)
            {
                ModelState.AddModelError("StringCount", "StringCount is big than 4!");
            }
            else if (master.PromoValue == 0)
            {
                ModelState.AddModelError("PromoValue", "PromoValue is required!");
            }
            else if (master.DateF == DateTime.MinValue)
            {
                ModelState.AddModelError("DateF", "DateFrom is required!");
            }
            else if (master.DateT == DateTime.MinValue)
            {
                ModelState.AddModelError("DateT", "DateTo is required!");
            }
            else if (master.PromoType.ToString() == "Percent" && master.PromoValue > 100)
            {
                ModelState.AddModelError("promoType", "Percent Can't be big than 100!");
            }
            else if (master.PriceListID <= 0)
            {
                ModelState.AddModelError("PriceListID", "PriceList is requried!");
            }
        }
        public IActionResult PromoCodeEdit(int ID)
        {
            ViewBag.PromoCode = "highlight";
            var PromoSetup = new PromoSetup()
            {
                PromoCodeDiscount = _context.PromoCodeDiscounts.Find(ID) ?? new PromoCodeDiscount(),
                SelectPriceList = _posRetail.SelectPriceLists()
            };
            return View(PromoSetup);
        }
        public IActionResult GetPromoCodeDetail(int ID, string keyword = "")
        {
            var promodetail = from pd in _context.PromoCodeDetails.Where(s => s.ID == ID)
                              select new PromoCodeDetail
                              {
                                  ID = pd.ID,
                                  PromoCodeID = pd.PromoCodeID,
                                  PromoCode = pd.PromoCode,
                                  UseCount = pd.UseCount,
                                  MaxUse = pd.MaxUse,
                              };
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                keyword = RawWord(keyword);
                StringComparison ignoreCase = StringComparison.InvariantCultureIgnoreCase;
                promodetail = promodetail.Where(c => RawWord(c.PromoCode).Contains(keyword, ignoreCase));
            }
            return Ok(promodetail.ToList());
        }
        //=========================Sart Sale Combo==================================
        public IActionResult SaleCombo()
        {
            ViewBag.SaleCombo = "highlight";
            ViewData["PriceListID"] = new SelectList(_context.PriceLists.Where(c => c.Delete == false), "ID", "Name");
            return View();

        }

        public async Task<IActionResult> SearchSaleCombo(string barCode)
        {
            var data = await (from cb in _context.SaleCombos.Where(i => i.Barcode == barCode)
                              join user in _context.UserAccounts on cb.CreatorID equals user.ID
                              join item in _context.ItemMasterDatas on cb.ItemID equals item.ID
                              join pl in _context.PriceLists on cb.PriListID equals pl.ID
                              join uom in _context.UnitofMeasures on cb.UomID equals uom.ID
                              select new SaleCombo
                              {
                                  Active = cb.Active,
                                  Barcode = cb.Barcode,
                                  PriListID = cb.PriListID,
                                  Code = cb.Code,
                                  ID = cb.ID,
                                  ItemID = cb.ItemID,
                                  PostingDate = cb.PostingDate,
                                  Type = cb.Type,
                                  UomID = cb.UomID,
                                  Creator = user.Username,
                                  CreatorID = cb.CreatorID,
                                  DateFormat = cb.PostingDate.ToShortDateString(),
                                  ItemName1 = item.KhmerName,
                                  ItemName2 = item.EnglishName ?? "",
                                  PriceListName = pl.Name,
                                  TypeDisplay = cb.Type == SaleType.SaleChild ? "Sale Child" : cb.Type == SaleType.SaleParent ? "Sale Parent" : "None",
                                  UoMName = uom.Name,
                                  ComboDetails = (from cbd in _context.SaleComboDetails.Where(i => i.SaleComboID == cb.ID)
                                                  join itemd in _context.ItemMasterDatas on cbd.ItemID equals itemd.ID
                                                  select new SaleComboDetail
                                                  {
                                                      LineID = $"{DateTime.Now.Ticks}{cbd.ID}",
                                                      Code = itemd.Code,
                                                      ID = cbd.ID,
                                                      SaleComboID = cb.ID,
                                                      Detele = cbd.Detele,
                                                      GUomID = cbd.GUomID,
                                                      ItemID = cbd.ItemID,
                                                      Qty = cbd.Qty,
                                                      UomID = cbd.UomID,
                                                      KhmerName = itemd.KhmerName,
                                                      UomSelect = (from guom in _context.GroupDUoMs.Where(i => itemd.GroupUomID == i.ID)
                                                                   join baseuom in _context.UnitofMeasures on guom.BaseUOM equals baseuom.ID
                                                                   join altuom in _context.UnitofMeasures on guom.AltUOM equals altuom.ID
                                                                   select new SelectListItem
                                                                   {
                                                                       Text = altuom.Name,
                                                                       Value = altuom.ID.ToString(),
                                                                       Selected = cbd.UomID == altuom.ID
                                                                   }).ToList()
                                                  }).ToList()
                              }).FirstOrDefaultAsync();
            return Ok(data);
        }

        public async Task<IActionResult> GetSaleCombos()
        {
            var data = await (from cb in _context.SaleCombos
                              join user in _context.UserAccounts on cb.CreatorID equals user.ID
                              join item in _context.ItemMasterDatas on cb.ItemID equals item.ID
                              join pl in _context.PriceLists on cb.PriListID equals pl.ID
                              join uom in _context.UnitofMeasures on cb.UomID equals uom.ID
                              select new SaleCombo
                              {
                                  Active = cb.Active,
                                  Barcode = cb.Barcode,
                                  PriListID = cb.PriListID,
                                  Code = cb.Code,
                                  ID = cb.ID,
                                  ItemID = cb.ItemID,
                                  PostingDate = cb.PostingDate,
                                  Type = cb.Type,
                                  UomID = cb.UomID,
                                  Creator = user.Username,
                                  CreatorID = cb.CreatorID,
                                  DateFormat = cb.PostingDate.ToShortDateString(),
                                  ItemName1 = item.KhmerName,
                                  ItemName2 = item.EnglishName ?? "",
                                  PriceListName = pl.Name,
                                  TypeDisplay = cb.Type == SaleType.SaleChild ? "Sale Child" : cb.Type == SaleType.SaleParent ? "Sale Parent" : "None",
                                  UoMName = uom.Name
                              }).ToListAsync();
            return Ok(data);
        }
        private int GetUserID()
        {
            _ = int.TryParse(User.FindFirst("UserID")?.Value, out int _id);
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
        //get item master
        [HttpGet]
        public IActionResult GetItemMasters(int ID)
        {
            var ListItem = new List<ItemDetails>();
            var list = from Item in _context.ItemMasterDatas.Where(x => x.Delete == false && x.Sale == true && x.PriceListID == ID)
                       join IUom in _context.UnitofMeasures on Item.InventoryUoMID equals IUom.ID
                       join Guom in _context.GroupUOMs on Item.GroupUomID equals Guom.ID
                       select new
                       {
                           Item.ID,
                           UomID = Item.InventoryUoMID,
                           GuomID = Guom.ID,
                           Item.Code,
                           Item.KhmerName,
                           Item.Barcode,
                           Uom = IUom.Name,
                           Guom = Guom.Name,
                           Item.EnglishName,
                           Item.GroupUomID,
                           Item.InventoryUoMID,
                       };
            foreach (var item in list)
            {
                var pld = _context.PriceListDetails.LastOrDefault(w => w.ItemID == item.ID && w.UomID == item.UomID && w.CurrencyID == GetCompany().SystemCurrencyID);
                var Gdoum = _context.GroupDUoMs.Where(w => w.GroupUoMID == item.GuomID);
                var Factor = Gdoum.FirstOrDefault(w => w.AltUOM == item.UomID).Factor;
                if (pld != null)
                {
                    var _item = new ItemDetails
                    {
                        LineID = DateTime.Now.Ticks.ToString(),
                        ID = 0,
                        ItemID = item.ID,
                        UomID = item.UomID,
                        GuomID = item.GuomID,
                        Code = item.Code,
                        KhmerName = item.KhmerName,
                        Barcode = item.Barcode,
                        Uom = item.Uom,
                        Guom = item.Guom,
                        EnglishName = item.EnglishName,
                        Factor = Factor,
                        UomSelect = (from guom in _context.GroupDUoMs.Where(i => item.GroupUomID == i.ID)
                                     join baseuom in _context.UnitofMeasures on guom.BaseUOM equals baseuom.ID
                                     join altuom in _context.UnitofMeasures on guom.AltUOM equals altuom.ID
                                     select new SelectListItem
                                     {
                                         Text = altuom.Name,
                                         Value = altuom.ID.ToString(),
                                         Selected = item.InventoryUoMID == altuom.ID
                                     }).ToList()
                    };
                    ListItem.Add(_item);
                }
                else
                {
                    var _item = new ItemDetails
                    {
                        LineID = DateTime.Now.Ticks.ToString(),
                        ID = 0,
                        ItemID = item.ID,
                        UomID = item.UomID,
                        GuomID = item.GuomID,
                        Code = item.Code,
                        KhmerName = item.KhmerName,
                        Barcode = item.Barcode,
                        Uom = item.Uom,
                        Guom = item.Guom,
                        EnglishName = item.EnglishName,
                        Factor = Factor,
                        UomSelect = (from guom in _context.GroupDUoMs.Where(i => item.GroupUomID == i.GroupUoMID)
                                     join altuom in _context.UnitofMeasures on guom.AltUOM equals altuom.ID
                                     select new SelectListItem
                                     {
                                         Text = altuom.Name,
                                         Value = altuom.ID.ToString(),
                                         Selected = item.InventoryUoMID == altuom.ID
                                     }).ToList()
                    };
                    ListItem.Add(_item);
                }
            }
            return Ok(ListItem);
        }
        //get item master detail
        [HttpGet]
        public IActionResult GetItemDetailspl(int ID)
        {
            var ListItem = new List<ItemDetails>();
            var list = from Item in _context.ItemMasterDatas.Where(x => x.Delete == false && x.Sale == true && x.PriceListID == ID)
                       join IUom in _context.UnitofMeasures on Item.InventoryUoMID equals IUom.ID
                       join Guom in _context.GroupUOMs on Item.GroupUomID equals Guom.ID
                       select new
                       {
                           Item.ID,
                           UomID = Item.InventoryUoMID,
                           GuomID = Guom.ID,
                           Item.Code,
                           Item.KhmerName,
                           Item.Barcode,
                           Uom = IUom.Name,
                           Guom = Guom.Name,
                           Item.EnglishName,
                           Item.GroupUomID,
                           Item.InventoryUoMID,
                       };
            foreach (var item in list)
            {
                var pld = _context.PriceListDetails.LastOrDefault(w => w.ItemID == item.ID && w.UomID == item.UomID && w.CurrencyID == GetCompany().SystemCurrencyID);
                var Gdoum = _context.GroupDUoMs.Where(w => w.GroupUoMID == item.GuomID);
                var Factor = Gdoum.FirstOrDefault(w => w.AltUOM == item.UomID).Factor;
                if (pld != null)
                {
                    var _item = new ItemDetails
                    {
                        LineID = DateTime.Now.Ticks.ToString(),
                        ID = 0,
                        ItemID = item.ID,
                        UomID = item.UomID,
                        GuomID = item.GuomID,
                        Code = item.Code,
                        KhmerName = item.KhmerName,
                        Barcode = item.Barcode,
                        Uom = item.Uom,
                        Guom = item.Guom,
                        EnglishName = item.EnglishName,
                        Factor = Factor,
                        UomSelect = (from guom in _context.GroupDUoMs.Where(i => item.GroupUomID == i.ID)
                                     join baseuom in _context.UnitofMeasures on guom.BaseUOM equals baseuom.ID
                                     join altuom in _context.UnitofMeasures on guom.AltUOM equals altuom.ID
                                     select new SelectListItem
                                     {
                                         Text = altuom.Name,
                                         Value = altuom.ID.ToString(),
                                         Selected = item.InventoryUoMID == altuom.ID
                                     }).ToList()
                    };
                    ListItem.Add(_item);
                }
                else
                {
                    var _item = new ItemDetails
                    {
                        LineID = DateTime.Now.Ticks.ToString(),
                        ID = 0,
                        ItemID = item.ID,
                        UomID = item.UomID,
                        GuomID = item.GuomID,
                        Code = item.Code,
                        KhmerName = item.KhmerName,
                        Barcode = item.Barcode,
                        Uom = item.Uom,
                        Guom = item.Guom,
                        EnglishName = item.EnglishName,
                        Factor = Factor,
                        UomSelect = (from guom in _context.GroupDUoMs.Where(i => item.GroupUomID == i.GroupUoMID)
                                     join altuom in _context.UnitofMeasures on guom.AltUOM equals altuom.ID
                                     select new SelectListItem
                                     {
                                         Text = altuom.Name,
                                         Value = altuom.ID.ToString(),
                                         Selected = item.InventoryUoMID == altuom.ID
                                     }).ToList()
                    };
                    ListItem.Add(_item);
                }
            }
            return Ok(ListItem);
        }
        //save item
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateSaleCombo(string data)
        {
            SaleCombo saleCombo = JsonConvert.DeserializeObject<SaleCombo>(data, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });
            ModelMessage msg = new();
            ValidateComboSale(saleCombo);
            saleCombo.CreatorID = GetUserID();
            if (ModelState.IsValid)
            {
                _context.SaleCombos.Update(saleCombo);
                _context.SaveChanges();
                ModelState.AddModelError("success", "Item save successfully.");
                msg.Approve();
            }
            ViewData["PriceListID"] = new SelectList(_context.PriceLists, "ID", "Name", saleCombo.PriListID);
            return Ok(new { Model = msg.Bind(ModelState) });
        }
        private void ValidateComboSale(SaleCombo saleCombo)
        {
            if (saleCombo.PriListID <= 0)
            {
                ModelState.AddModelError("PriListID", "Please select price list!");
            }
            if (saleCombo.ComboDetails.ToList().Count <= 0)
            {
                ModelState.AddModelError("ComboDetails", "Please choose combo details!");
            }
            if (saleCombo.Type == SaleType.None)
            {
                ModelState.AddModelError("Type", "Please select any type!");
            }
            if (string.IsNullOrEmpty(saleCombo.Barcode))
            {
                ModelState.AddModelError("Barcode", "Barcode is require!");
            }
            if (saleCombo.PostingDate < DateTime.Today && saleCombo.ID == 0)
            {
                ModelState.AddModelError("PostingDate", "Posting date is invalid!");
            }
            if (saleCombo.ItemID <= 0)
            {
                ModelState.AddModelError("Item", "Please choose any item!");
            }
            var scb = _context.SaleCombos.AsNoTracking().FirstOrDefault(i => i.ID == saleCombo.ID);
            var scbs = _context.SaleCombos.AsNoTracking().ToList();
            if (saleCombo.ID == 0)
            {
                if (scbs.Any(i => i.Barcode == saleCombo.Barcode))
                {
                    ModelState.AddModelError("Barcode", $"Barcode \"{saleCombo.Barcode}\" is already existed");
                }
            }
            if (saleCombo.ID > 0)
            {
                if (saleCombo.Barcode != scb.Barcode)
                {
                    if (scbs.Any(i => i.Barcode == saleCombo.Barcode))
                    {
                        ModelState.AddModelError("Barcode", $"Barcode \"{saleCombo.Barcode}\" is already existed");
                    }
                }
            }
            int index = 0;
            foreach (var i in saleCombo.ComboDetails.ToList())
            {
                index++;
                if (i.Qty <= 0)
                {
                    ModelState.AddModelError($"ItemQty{index}", $"At line \"{index}\" qty cannot be 0 or less than 0!");
                }
            }
        }
        // remove item detail
        [HttpPost]
        public IActionResult RemoveItemDetail(int detailID)
        {
            ModelMessage msg = new();
            var details = _context.SaleComboDetails.Where(d => d.ID == detailID);
            if (ModelState.IsValid)
            {
                _context.SaleComboDetails.Find(detailID).Detele = true;
                _context.SaveChanges();
                msg.Approve();
            }
            return Ok(msg.Bind(ModelState));
        }

        [HttpPost]
        public IActionResult DeleteComboSaleDetail(int id)
        {
            var data = _context.SaleComboDetails.Find(id);
            if (data == null)
            {
                return Ok(new { Error = true, Message = "Not found" });
            }
            try
            {
                _context.SaleComboDetails.Remove(data);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                return Ok(new { Error = true, ex.Message });
            }
            return Ok(new { Error = false });
        }
        //=====================End of Sale Combo=========================

        // Buy x amount get x discount //
        [Privilege("JE005")]
        [HttpGet]
        public IActionResult BuyXAmountGetXDisCount()
        {
            ViewBag.BuyXAmountGetXDis = "highlight";
            return View();
        }
        [HttpPost]
        public IActionResult UpdateItemdetail(string data)
        {
            List<PBuyXAmountGetXDis> buyXAmountGetXDis = JsonConvert.DeserializeObject<List<PBuyXAmountGetXDis>>(data.ToString(),
               new JsonSerializerSettings
               {
                   NullValueHandling = NullValueHandling.Ignore
               });
            var _buyXAmountGetXDis = buyXAmountGetXDis.Where(i => !string.IsNullOrEmpty(i.Code) && !string.IsNullOrEmpty(i.Name)).ToList();
            ModelMessage msg = new();
            var index = 0;
            foreach (var i in _buyXAmountGetXDis)
            {
                index++;
                ValidateSummary(i, index);
            }
            if (ModelState.IsValid)
            {
                _context.PBuyXAmountGetXDis.UpdateRange(_buyXAmountGetXDis);
                _context.SaveChanges();
                ModelState.AddModelError("success", "Item save successfully.");
                msg.Approve();
            }
            return Ok(msg.Bind(ModelState));
        }
        public IActionResult GetItemDis()
        {
            var itemdis = (from b in _context.PBuyXAmountGetXDis
                           join p in _context.PriceLists on b.PriListID equals p.ID
                           select new BuyXAmountGetXDiscountViewModel
                           {
                               LineID = $"{DateTime.Now.Ticks + b.ID}",
                               ID = b.ID,
                               DisType = b.DisType,
                               Code = b.Code,
                               Name = b.Name,
                               PriList = p.Name,
                               PriceListSelect = PriceLists().Select(i => new SelectListItem
                               {
                                   Text = i.Name,
                                   Value = i.ID.ToString(),
                                   Selected = i.ID == b.PriListID
                               }).ToList(),
                               DisTypeSelect = DisType.Select(i => new SelectListItem
                               {
                                   Text = i.Value,
                                   Value = i.Key.ToString(),
                                   Selected = i.Key == (int)b.DisType
                               }).ToList(),
                               DateF = b.DateF,
                               DateT = b.DateT,
                               Amount = b.Amount,
                               DisRateValue = b.DisRateValue,
                               Active = b.Active,
                               PriListID = b.PriListID,

                           }).ToList();
            return Ok(itemdis);
        }
        [HttpGet]
        public IActionResult GetTable()
        {
            List<BuyXAmountGetXDiscountViewModel> buyXAmountGetXDis = new();
            ViewData["PriceList"] = new SelectList(_context.PriceLists.Where(c => c.Delete == false), "ID", "Name");
            var buyxgetxdis = new BuyXAmountGetXDiscountViewModel
            {
                LineID = DateTime.Now.Ticks.ToString(),
                ID = 0,
                Name = "",
                Code = "",
                ItemName = "",
                DateT = default,
                DateF = default,
                DisRateValue = 0,
                Amount = 0,
                Active = false,
                PriceListSelect = PriceLists().Select(i => new SelectListItem
                {
                    Text = i.Name, //+ i.Name,
                    Value = i.ID.ToString(),
                }).ToList(),
                DisTypeSelect = DisType.Select(i => new SelectListItem
                {
                    Text = i.Value,
                    Value = i.Key.ToString(),
                    Selected = i.Key == (int)TypeDiscountBuyXAmountGetXDiscount.Rate
                }).ToList(),
                DisType = TypeDiscountBuyXAmountGetXDiscount.Rate,
            };
            return Ok(buyxgetxdis);
        }
        private List<PriceLists> PriceLists()
        {
            var data = _context.PriceLists.Where(c => c.Delete == false).ToList();
            // as add
            data.Insert(0, new PriceLists
            {
                Name = "-- Select --",
                ID = 0,
            });
            return data;
        }
        // End buy x amount get x discount //


        //Buy x qty get x discount
        public IActionResult BuyXQtyGetXDis()
        {

            ViewBag.BuyxQtyGetXdis = "highlight";
            return View();
        }
        [HttpGet]
        public IActionResult Buyitemgetdis()
        {
            var list = (from item in _context.ItemMasterDatas.Where(x => x.Delete == false && x.Sale == true)
                        join IUom in _context.UnitofMeasures on item.InventoryUoMID equals IUom.ID
                        join Guom in _context.GroupUOMs on item.GroupUomID equals Guom.ID
                        let gdoum = _context.GroupDUoMs.FirstOrDefault(w => w.GroupUoMID == Guom.ID && w.AltUOM == item.InventoryUoMID) ?? new GroupDUoM()
                        select new ItemDetails
                        {
                            LineID = $"{DateTime.Now.Ticks}{item.ID}",
                            ID = 0,
                            ItemID = item.ID,
                            UomID = item.InventoryUoMID,
                            GuomID = Guom.ID,
                            Code = item.Code,
                            KhmerName = item.KhmerName,
                            Barcode = item.Barcode,
                            Uom = IUom.Name,
                            Guom = Guom.Name,
                            EnglishName = item.EnglishName,
                            Factor = gdoum.Factor,
                            UomSelect = (from guom in _context.GroupDUoMs.Where(i => item.GroupUomID == i.GroupUoMID)
                                         join altuom in _context.UnitofMeasures on guom.AltUOM equals altuom.ID
                                         select new SelectListItem
                                         {
                                             Text = altuom.Name,
                                             Value = altuom.ID.ToString(),
                                             Selected = item.InventoryUoMID == altuom.ID
                                         }).ToList()
                        }).ToList();
            return Ok(list);
        }
        [HttpGet]
        public IActionResult GetEmptyTable()
        {
            var buyxqtygetxdis = new BuyXQtyGetXDisViewModel
            {
                LineID = DateTime.Now.Ticks.ToString(),
                ID = 0,
                Name = "",
                Code = "",
                DateT = default,
                DateF = default,
                BuyItemID = 0,
                Qty = 0,
                UomID = 0,
                BuyItem = "",
                UomSelect = default,
                DisItemID = 0,
                DisItem = "",
                Active = false,
            };
            return Ok(buyxqtygetxdis);
        }
        public IActionResult GetItemBuyXqtyGetXDis()
        {
            var itemdis = (from b in _context.BuyXQtyGetXDis
                           join item in _context.ItemMasterDatas on b.DisItemID equals item.ID
                           join d in _context.ItemMasterDatas on b.BuyItemID equals d.ID
                           select new BuyXQtyGetXDisViewModel
                           {
                               LineID = $"{DateTime.Now.Ticks + b.ID}",
                               ID = b.ID,
                               DisItemID = b.DisItemID,
                               BuyItemID = b.BuyItemID,
                               Code = b.Code,
                               Name = b.Name,
                               DateF = b.DateF,
                               DateT = b.DateT,
                               BuyItem = d.KhmerName,
                               DisItem = item.KhmerName,
                               UomID = b.UomID,
                               Qty = b.Qty,
                               DisRate = b.DisRate,
                               Active = b.Active,
                               UomSelect = (from guom in _context.GroupDUoMs.Where(i => d.GroupUomID == i.GroupUoMID)
                                            join altuom in _context.UnitofMeasures on guom.AltUOM equals altuom.ID
                                            select new SelectListItem
                                            {
                                                Text = altuom.Name,
                                                Value = altuom.ID.ToString(),
                                                Selected = d.InventoryUoMID == b.UomID
                                            }).ToList()
                           }).ToList();
            return Ok(itemdis);
        }
        private void ValidateItem(BuyXQtyGetXDis buyXQtyGetXDis, int index)
        {
            var bxagxd = _context.BuyXQtyGetXDis.AsNoTracking().FirstOrDefault(i => i.ID == buyXQtyGetXDis.ID);
            var bxagxds = _context.BuyXQtyGetXDis.AsNoTracking().ToList();
            if (buyXQtyGetXDis.Name == "")
            {
                ModelState.AddModelError("Name", "Please input Name.");
            }
            if (buyXQtyGetXDis.Code == "")
            {
                ModelState.AddModelError("Code", "Please Input Code");
            }
            if (buyXQtyGetXDis.ID == 0)
            {
                if (bxagxds.Any(i => i.Code == buyXQtyGetXDis.Code))
                {
                    ModelState.AddModelError("Code", $"At line \"{index}\" Code \"{buyXQtyGetXDis.Code}\" is already existed");
                }
            }
            if (buyXQtyGetXDis.ID > 0)
            {
                if (buyXQtyGetXDis.Code != bxagxd.Code)
                {
                    if (bxagxds.Any(i => i.Code == buyXQtyGetXDis.Code))
                    {
                        ModelState.AddModelError("Code", $"At line \"{index}\" Code \"{buyXQtyGetXDis.Code}\" is already existed");
                    }
                }
            }
            if (buyXQtyGetXDis.Qty <= 0)
            {
                ModelState.AddModelError("Qty", "Please Input Qty.");
            }
            if (buyXQtyGetXDis.DisRate <= 0)
            {
                ModelState.AddModelError("DisRate", "Please Input Rate.");

            }
        }
        [HttpPost]
        public IActionResult UpdateBuyXqtyGetDis(string data)
        {
            List<BuyXQtyGetXDis> buyxqtygetxdis = JsonConvert.DeserializeObject<List<BuyXQtyGetXDis>>(data,
               new JsonSerializerSettings
               {
                   NullValueHandling = NullValueHandling.Ignore
               });
            var _buyxqtygetxdis = buyxqtygetxdis.Where(i => !string.IsNullOrEmpty(i.Code) && !string.IsNullOrEmpty(i.Name)).ToList();
            ModelMessage msg = new();
            int index = 0;
            foreach (var i in _buyxqtygetxdis)
            {
                index++;
                ValidateItem(i, index);
            }
            if (ModelState.IsValid)
            {
                _context.BuyXQtyGetXDis.UpdateRange(_buyxqtygetxdis);
                _context.SaveChanges();
                ModelState.AddModelError("success", "Item save successfully.");
                msg.Approve();
            }
            return Ok(msg.Bind(ModelState));
        }


        public IActionResult ClearPoint()
        {
            ViewBag.ClearPoint = "highlight";
            return View();
        }
        public IActionResult GetTablePoints()
        {
            var clearpoint = new BusinessPartner
            {
                LineID = DateTime.Now.Ticks.ToString(),
                ID = 0,
                Code = "",
                Name = "",
                TotalPoint = 0,
                ClearPoints = 0,
                OutstandPoint = 0,
                AfterClear = 0
            };
            return Ok(clearpoint);
        }
        public IActionResult GetBussinessCustomer()
        {
            var datas = (from b in _context.BusinessPartners.Where(x => !x.Delete && x.Type == "Customer")
                         select new BusinessPartner
                         {
                             ID = b.ID,
                             Code = b.Code,
                             Name = b.Name,
                             Phone = b.Phone,
                             OutstandPoint = b.OutstandPoint,
                         }).ToList();
            return Ok(datas);
        }
        // [HttpPost]
        // public IActionResult ClearPoint(string _data)
        // {
        //     BusinessPartner data = JsonConvert.DeserializeObject<BusinessPartner>(_data, new JsonSerializerSettings
        //     {
        //         NullValueHandling = NullValueHandling.Ignore
        //     });
        //     ModelMessage msg = new();
        //     if (ModelState.IsValid)
        //     {
        //         _context.BusinessPartners.Update(data);
        //         _context.SaveChanges();
        //         ModelState.AddModelError("success", "Clear Point  succussfully!");
        //         msg.Approve();
        //     }
        //     return Ok(msg.Bind(ModelState));
        // }

        [HttpPost]
        public IActionResult ClearPoint(List<BusinessPartner> _data)
        {
            var datas = _context.BusinessPartners.Where(x => !x.Delete && x.Type == "Customer").ToList();
            foreach (var a in datas)
            {
                foreach (var b in _data)
                {
                    if (a.ID == b.ID)
                    {
                        a.OutstandPoint = b.OutstandPoint;
                        a.CumulativePoint = b.OutstandPoint;
                    }
                }
            }
            ModelMessage msg = new();
            if (ModelState.IsValid)
            {
                _context.BusinessPartners.UpdateRange(datas);
                _context.SaveChanges();
                ModelState.AddModelError("success", "Clear Point  succussfully!");
                msg.Approve();
            }
            return Ok(msg.Bind(ModelState));
        }

    }
}
