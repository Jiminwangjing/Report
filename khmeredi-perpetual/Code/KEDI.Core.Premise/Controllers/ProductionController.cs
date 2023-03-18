using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using CKBS.AppContext;
using CKBS.Models.Services.BOM;
using CKBS.Models.Services.Production;
using KEDI.Core.Models.Validation;
using KEDI.Core.Premise.Authorization;
using System.Security.Claims;

namespace CKBS.Controllers
{
    [Privilege]
    public class ProductionController : Controller
    {
        private readonly DataContext _Context;
        public ProductionController(DataContext Context)
        {
            _Context = Context;
        }

        [Privilege("A045")]
        public IActionResult BOMaterial()
        {
            ViewBag.Production = "show";
            ViewBag.BOMaterial = "highlight";
            return View();
        }

        private void ValidateSummary(dynamic master, IEnumerable<dynamic> details)
        {
            if (master.ItemID == 0)
            {
                ModelState.AddModelError("ItemID", "Item master need to be selected.");
            }
            foreach (var dt in details)
            {
                if (dt.Qty <= 0)
                {
                    ModelState.AddModelError("Details", "Required item detail quantity greater than 0.");
                }
                if (dt.ItemID == 0)
                {
                    ModelState.AddModelError("Details", "Required item detail quantity greater than 0.");
                }
            }  
            if(details.ToList().Count == 0)
            {
                ModelState.AddModelError("Details", "Item details need to be selected.");
            }
        }

        [HttpGet]
        public IActionResult GetItemMasters()
        {
            var list = from Item in _Context.ItemMasterDatas.Where(x => x.Delete == false && x.Sale == true)
                       join IUom in _Context.UnitofMeasures on Item.InventoryUoMID equals IUom.ID
                       select new
                       {
                           Item.ID,
                           UomID = Item.InventoryUoMID,
                           Item.Code,
                           Item.Barcode,
                           Uom = IUom.Name,
                           Item.KhmerName,
                           Item.EnglishName
                       };
            return Ok(list);
        }

        [HttpGet]
        public IActionResult GetItemDetails()
        {
            var com = _Context.Company.FirstOrDefault(w => !w.Delete);
            var ListItem = new List<ItemDetails>();
            var list = from Item in _Context.ItemMasterDatas.Where(x => x.Delete == false && x.Process == "Average")
                       join IUom in _Context.UnitofMeasures on Item.InventoryUoMID equals IUom.ID
                       join Guom in _Context.GroupUOMs on Item.GroupUomID equals Guom.ID
                       select new
                       {
                           Item.ID,
                           UomID = Item.InventoryUoMID,
                           GuomID = Guom.ID,
                           Item.Code,
                           Item.Barcode,
                           Uom = IUom.Name,
                           Guom = Guom.Name,
                           Item.KhmerName,
                           Item.EnglishName,
                           
                       };
            foreach (var item in list)
            {
                var pld = _Context.PriceListDetails.LastOrDefault(w => w.ItemID == item.ID && w.UomID == item.UomID && w.CurrencyID == com.SystemCurrencyID);
                var Gdoum = _Context.GroupDUoMs.Where(w => w.GroupUoMID == item.GuomID);
                var Factor = Gdoum.FirstOrDefault(w => w.AltUOM == item.UomID).Factor;
                if (pld != null)
                {
                    var _item = new ItemDetails
                    {
                        ID = item.ID,
                        UomID = item.UomID,
                        GuomID = item.GuomID,
                        Code = item.Code,
                        Barcode = item.Barcode,
                        Uom = item.Uom,
                        Guom = item.Guom,
                        Cost = pld.Cost,
                        KhmerName = item.KhmerName,
                        EnglishName = item.EnglishName,
                        Factor = Factor
                     };
                        ListItem.Add(_item);                    
                }
                else
                {
                    var _item = new ItemDetails
                    {
                        ID = item.ID,
                        UomID = item.UomID,
                        GuomID = item.GuomID,
                        Code = item.Code,
                        Barcode = item.Barcode,
                        Uom = item.Uom,
                        Guom = item.Guom,
                        Cost = 0,
                        KhmerName = item.KhmerName,
                        EnglishName = item.EnglishName,
                        Factor = Factor
                    };
                    ListItem.Add(_item);
                }
            }
            return Ok(ListItem);
        }

        [HttpGet]
        public IActionResult GetSysCurrency()
        {
            var cur = from com in _Context.Company.Where(x => x.Delete == false)                      
                      join c in _Context.Currency.Where(x => x.Delete == false) on com.SystemCurrencyID equals c.ID
                      select new
                      {
                          c.ID,
                          c.Description
                      };
            return Ok(cur);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateBOMaterial(string data)
        {            
            BOMaterial BOMaterials = JsonConvert.DeserializeObject<BOMaterial>(data, new JsonSerializerSettings{
                NullValueHandling = NullValueHandling.Ignore
            });

            BOMaterials.UserID = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            ModelMessage msg = new ();
            ValidateSummary(BOMaterials, BOMaterials.BOMDetails);
            using (var t = _Context.Database.BeginTransaction())
            {
                if (ModelState.IsValid)
                {
                    _Context.BOMaterial.Update(BOMaterials);
                    _Context.SaveChanges();
                    t.Commit();
                    ModelState.AddModelError("success", "Item save successfully.");
                    msg.Approve();
                }
            }                 
            return Ok(new { Model = msg.Bind(ModelState) });
        }

        public IActionResult GetItemMaterial( int ItemID)
        {
            List<ItemGetdetails> GetItem = new ();
            var item = _Context.BOMaterial.FirstOrDefault(b => b.ItemID == ItemID);
            if (item == null)
            {
                return Ok();
            }
            var details = (from bom in _Context.BOMaterial.Where(b => b.ItemID == ItemID)
                           join bomd in _Context.BOMDetail.Where(d => d.Detele == false) on bom.BID equals bomd.BID
                           join i in _Context.ItemMasterDatas on bomd.ItemID equals i.ID
                           join u in _Context.UnitofMeasures on bomd.UomID equals u.ID
                           join g in _Context.GroupUOMs on bomd.GUomID equals g.ID
                           select new
                           {
                               MBID = bom.BID, 
                               MItemID = bom.ItemID,
                               MUomID = bom.UomID,
                               bom.PostingDate,
                               bom.TotalCost,
                               bom.Active,
                               //
                               bomd.BID,
                               bomd.BDID,
                               bomd.Qty,
                               bomd.Cost,
                               bomd.Amount,
                               ItemID = i.ID,
                               bomd.UomID,
                               GuomID = bomd.GUomID,
                               i.Code,
                               i.Barcode,
                               Uom = u.Name,
                               Guom = g.Name,
                               i.KhmerName,
                               i.EnglishName,
                           }).ToList();
            foreach (var data in details)
            {
                var Gdoum = _Context.GroupDUoMs.Where(w => w.GroupUoMID == data.GuomID);
                var Factor = Gdoum.FirstOrDefault(w => w.AltUOM == data.UomID).Factor;
                var _item = new ItemGetdetails
                {
                    MBID = data.BID,
                    MItemID = data.ItemID,
                    MUomID = data.UomID,
                    PostingDate = data.PostingDate,
                    TotalCost = data.TotalCost,
                    Active = data.Active,
                    //
                    BID = data.BID,
                    BDID = data.BDID,
                    Qty = data.Qty,
                    Cost = data.Cost,
                    Amount = data.Amount,
                    ItemID = data.ItemID,
                    UomID = data.UomID,
                    GuomID = data.GuomID,
                    Code = data.Code,
                    Barcode = data.Barcode,
                    Uom = data.Uom,
                    Guom = data.Guom,
                    KhmerName = data.KhmerName,
                    EnglishName = data.EnglishName,
                    Factor = Factor
                };
                GetItem.Add(_item);
            }
            return Ok(GetItem);
        }

        [HttpGet]
        public IActionResult RemoveMaterialDetail(int detailID)
        {
            ModelMessage msg = new ();
            var details = _Context.BOMDetail.Where(d => d.BDID == detailID);
            if (ModelState.IsValid)
            {
                _Context.BOMDetail.Find(detailID).Detele = true;
                _Context.SaveChanges();
                msg.Approve();
            }
            return Ok(msg.Bind(ModelState));
        }

        //Bom Report 
        [Privilege("A046")]
        public IActionResult BOMReport()
        {
            ViewBag.BomReport = "highlight";
            var list = GetData();
            return View(list);
        }

        private object GetData()
        {
            var list = (from bom in _Context.BOMaterial
                       join bomd in _Context.BOMDetail.Where(w=>w.Detele == false) on bom.BID equals bomd.BID 
                       join id in _Context.ItemMasterDatas on bomd.ItemID equals id.ID
                       join i in _Context.ItemMasterDatas on bom.ItemID equals i.ID
                       join u in _Context.UnitofMeasures on bomd.UomID equals u.ID
                       join guom in _Context.GroupUOMs on bom.UomID equals guom.ID
                       join syscy in _Context.Currency on bom.SysCID equals syscy.ID
                       group new { bom, bomd, id, i, u, guom,syscy } by bom.BID into g
                       select new BomReport
                       {
                           //master
                           BID = g.Key,
                           ID = g.First().bom.BID,
                           KhmerName = g.First().i.KhmerName,
                           Uom = g.First().guom.Name,
                           PostingDate = g.First().bom.PostingDate.ToString("dd-MM-yyyy"),
                           TotalCost = g.First().bom.TotalCost,
                           Active = g.First().bom.Active,
                           SysCy = g.First().syscy.Description,
                           BomDetails = g
                            .Select(x => new BomDetail
                            {
                                //Detail
                                ID = x.bomd.BDID,
                                Qty = x.bomd.Qty,
                                UomD = x.u.Name,
                                Cost = x.bomd.Cost,
                                Amount = x.bomd.Amount,
                                ItemName = x.id.KhmerName,
                                Negativestock = x.bomd.NegativeStock

                            }).ToList(),
                       }).ToList();
            return list;
        }

        [HttpGet]
        public IActionResult CheckItem(int ID, bool Check)
        {
            var checkbox = _Context.BOMDetail.FirstOrDefault(x => x.BDID == ID);
            if (checkbox != null)
            {
                checkbox.NegativeStock = Check;
                _Context.Update(checkbox);
                _Context.SaveChanges();

                return Ok(new { status = "T" });

            }
            else
            {
                return Ok(new { status = "F" });
            }
        }

        [HttpGet]
        public IActionResult CheckItemMaster(int ID, bool Check)
        {
            var checkbox = _Context.BOMaterial.FirstOrDefault(x => x.BID == ID);
            if (checkbox != null)
            {
                checkbox.Active = Check;
                _Context.Update(checkbox);
                _Context.SaveChanges();
                return Ok(new { status = "T" });
            }
            else
            {
                return Ok(new { status = "F" });
            }
        }
    }
}
