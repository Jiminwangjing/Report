using System;
using System.Collections.Generic;
using System.Web;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CKBS.AppContext;
using CKBS.Models.Services.Pagination;
using CKBS.Models.Services.Promotions;
using CKBS.Models.Services.Responsitory;
using CKBS.Models.ServicesClass;
using Microsoft.AspNetCore.Authorization;
using KEDI.Core.Premise.Authorization;

namespace CKBS.Controllers
{
    [Privilege]
    public class PointController : Controller
    {
        private readonly DataContext _context;
        private readonly IPromotion _promotion;
        private readonly IItemMasterData _itemMasterdata;
        private readonly IExchangeRate _exchangreate;
        public PointController(DataContext context,IPromotion promotion, IItemMasterData itemMasterData, IExchangeRate exchangreate)
        {
            _context = context;
            _promotion = promotion;
            _itemMasterdata = itemMasterData;
            _exchangreate = exchangreate;
        }
        
        [Privilege("A030")]
        public IActionResult Point()
        {
            ViewBag.style = "fa fa-percent";
            ViewBag.Main = "Promotion";
            ViewBag.Page = "Point";
            ViewBag.Subpage = "Create Point";
            ViewBag.Menu = "show";
            return View();
        }

        private bool PointExists(int id)
        {
            return _context.Points.Any(e => e.ID == id);
        }

        [HttpGet]
        public IActionResult GetPointItemMaster()
        {
            var list = _promotion.ServicePointDetails.ToList();
            return Ok(list);
        }

       [HttpPost]
       public  IActionResult  InsertPoint(PointService servicedata)
        {
            _promotion.AddorEditPoint(servicedata);
            var list = _promotion.GetPoints().Where(x => x.Delete == false).ToList();
            return Ok(list);
        }

        [HttpGet]
        public IActionResult GetPointDetail(int ID)
        {
            var list = _promotion.GetPointDetails().Where(x => x.PointID == ID).ToList();
            return Ok(list);
        }

        [HttpGet]
        public IActionResult GetPoint()
        {
            var list = _promotion.GetPoints().Where(x => x.Delete == false).ToList();
            return Ok(list);
        }

        [HttpPost]
        public  IActionResult DeletePointDetail(int ID)
        {
            _promotion.DeletePointDelail(ID);
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> DeletePoint(int ID)
        {
           await _promotion.DeletePoint(ID);
            var list = _promotion.GetPoints().Where(x => x.Delete == false).ToList();
            return Ok(list);
        }

        [HttpGet]
        public IActionResult GetBasecurrency()
        {
           var list =_exchangreate.GetBaseCurrencyName().ToList();
            return Ok(list);
        }

        [HttpPost]
        public IActionResult CheckSet_Point(int Check)
        {
            var list = _context.Points.Where(x => x.Quantity == Check && x.Delete == false);
            return Ok(list);
        }
    }
} 
