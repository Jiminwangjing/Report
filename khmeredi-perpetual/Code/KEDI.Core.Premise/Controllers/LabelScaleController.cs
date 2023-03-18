using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using CKBS.AppContext;
using CKBS.Models.Services.Inventory.PriceList;
using KEDI.Core.Premise.Authorization;
using System;
using KEDI.Core.Premise.Models.ServicesClass;
using KEDI.Core.Premise.Models.Services.Inventory;

namespace PLU.Controllers
{
    [Privilege]
    public class LabelScaleController : Controller
    {
        private readonly DataContext _context;
        public LabelScaleController(DataContext context)
        {
            _context = context;
        }

        [Privilege("SR001")]
        public IActionResult LabelScale()
        {
            ViewBag.LabelScale = "highlight";
            ViewBag.LabelScale = "show";
            ViewBag.LabelScale = "highlight";
            return View();
        }
        public IActionResult LabelScaleCAS()
        {
            ViewBag.LabelScaleCAS = "highlight";
            ViewBag.LabelScaleCAS = "show";
            ViewBag.LabelScaleCAS = "highlight";
            return View();
        }
        [HttpGet]
        public IActionResult LabelScaleCL5500()//cL5500
        {
            ViewBag.LabelScaleCL5500 = "highlight";
            return View();
        }
        public IActionResult GetLabelScale(int PLUID)
        {
            List<PriceListDetail> pldfilter = new ();
            if(PLUID != 0)
            {
                pldfilter = _context.PriceListDetails.Where(x => x.PriceListID == PLUID).ToList();
            }
            else
            {
                return Ok(pldfilter);
            }
            var list = from pd in pldfilter
                       join p in _context.PriceLists on pd.PriceListID equals p.ID
                       join cur in _context.Currency on p.CurrencyID equals cur.ID
                       join i in _context.ItemMasterDatas.Where(i=> i.Scale) on pd.ItemID equals i.ID
                       join guom in _context.GroupUOMs on i.GroupUomID equals guom.ID
                       where pd.UnitPrice > 0
                       select new
                       {
                           Hotkey = 0,
                           Name = i.KhmerName,
                           LFCode = i.Barcode,
                           Code = i.Barcode,
                           BarcodeType = 2,
                           pd.UnitPrice,
                           UnitWeight = 4, //kg=4 , g=1
                           UnitAmount = 0,
                           Department = 1,
                           PTWeight = 0,
                           ShelfTime = 0,
                           PackType = 0,
                           Tare = 0,
                           Error = 0,
                           Message1 = 0,
                           Message2 = 0,
                           Label = 0,
                           Discount = 0
                       };
            return Ok(list);
        }

        //GetPriceList
        public IActionResult GetPriceList()
        {
            var pricelist = _context.PriceLists.Where(x => x.Delete == false).ToList();
            return Ok(pricelist);
        }

        //LabelScaleCL5500
        public IActionResult GetLabelScaleCL5500(int PLUID)
        {
  
            var list1 = from pd in _context.PriceListDetails.Where(w => w.PriceListID == PLUID)
                        join i in _context.ItemMasterDatas.Where(w => w.Scale == true) on pd.ItemID equals i.ID

                        join prd in _context.PropertyDetails on i.ID equals prd.ItemID
                        join pr_COO in _context.Property.Where(w => w.Name == "Country Of Origin") on prd.ProID equals pr_COO.ID
                        join crp_COO in _context.ChildPreoperties on prd.Value equals crp_COO.ID

                        join prd2 in _context.PropertyDetails on i.ID equals prd2.ItemID
                        join pr_SBD in _context.Property.Where(w => w.Name == "Sell By Date") on prd2.ProID equals pr_SBD.ID
                        join crp_SBD in _context.ChildPreoperties on prd2.Value equals crp_SBD.ID
                        //group new { crp_SBD, pd, i, prd, pr_COO, crp_COO, prd2, pr_SBD } by new { crp_SBD.ProID } into datas
                        //let data = datas.FirstOrDefault()
                        select new 
                        {
                            DepartmentNo = 1,
                            PLUNo =i.Barcode,
                            PLUType = 1,
                            Itemcode = i.Barcode,
                            Name1 = i.KhmerName,
                            Name2 = crp_COO.Name,
                            GroupNo = 0,
                            OriginNo = 0,
                            DirectIngredient = "",
                            UnitPrice = (decimal)pd.UnitPrice,
                            Image = "",
                            TareValue=0,
                            NutrifactNo=0,
                           SellByDate = crp_SBD.Name,
                            UpdateDate = DateTime.Now.ToShortDateString(),
                        };      
           
      
            return Ok(list1);
        }
    }
}
