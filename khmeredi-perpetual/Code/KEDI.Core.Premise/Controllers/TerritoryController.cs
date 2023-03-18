using CKBS.AppContext;
using KEDI.Core.Models.Validation;
using KEDI.Core.Premise.Models.Services.Territory;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KEDI.Core.Premise.Controllers
{
    public class TerritoryController : Controller
    {
        private readonly DataContext _context;

        public TerritoryController(DataContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            ViewBag.Territory = "highlight";
            ViewData["ParentId"] = new SelectList(_context.Territories, "ID", "Name");
            return View();
        }
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("ID,Name,ParentId,ParentId,MainParentId,Level,LocationId")] Territory territory)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context.Add(territory);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));

        //    }
        //    return View(territory);
        //}

        [HttpPost]
        public IActionResult SubmmitData(string _data)
        {
            Territory data = JsonConvert.DeserializeObject<Territory>(_data, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });

            ModelMessage msg = new();
            if (ModelState.IsValid)
            {
                if(data.ID == 0)
                {
                    var lastCreatedata = _context.Territories.AsNoTracking().Where(i => i.ParentId == data.ParentId).OrderByDescending(i => i.ID).FirstOrDefault() ?? new Territory();
                    data.LoationId = lastCreatedata.ID;
                }
                _context.Territories.Update(data);
                _context.SaveChanges();
                ModelState.AddModelError("success", "Territory created succussfully!");
                msg.Approve();
            }
            return Ok(msg.Bind(ModelState));
        }
        public IActionResult GetDataParentTer(int id)
        {
            if (id != 0)
            {
                var data = _context.Territories.Where(x => x.ParentId == id).ToList();
                return Ok(data);
            }
            else if (id == 0)
            {
                var data = _context.Territories.Where(x => x.ParentId == 0).ToList();
                return Ok(data);
            }
            return Ok();
        }
        public IActionResult GetParent()
        {
            var data = _context.Territories.Where(x => x.ParentId == 0).ToList();
            return Ok(data);
        }
        public IActionResult GetdataBind()
        {
            var data = _context.Territories.ToList();
            return Ok(data);
        }
        public IActionResult GetListSub(int parentId, int id)
        {
            var data = _context.Territories.Where(x => x.ParentId == parentId && x.ID != id).ToList();
            data.Insert(0, new Territory { ID = 0, Name = "First" });
            data.Insert(data.Count, new Territory { ID = -1, Name = "Last" });
            return Ok(data);

        }
    }
}
