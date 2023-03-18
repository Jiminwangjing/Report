using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CKBS.AppContext;
using CKBS.Models.Services.Administrator.Inventory;
using CKBS.Models.Services.Responsitory;
using System.Text.RegularExpressions;
using KEDI.Core.Premise.Authorization;

namespace CKBS.Controllers.Developer2
{
    [Privilege]
    public class UnitofMeasureController : Controller
    {
        private readonly DataContext _context;
        private readonly IUOM _uOM;

        public UnitofMeasureController(DataContext context,IUOM uOM)
        {
            _context = context;
            _uOM = uOM;
        }

        [HttpGet]
        [Privilege("A006")]
        public IActionResult Index()
        {
            ViewBag.UnitofMeasure = "highlight";
            return View();
        }

        string RawWord(string keyword)
        {
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                return Regex.Replace(keyword, "\\+s", string.Empty);
            }
            return string.Empty;
        }

        public IActionResult GetUnitofMeasure(string keyword = "")
        {
            int userid = int.Parse(User.FindFirst("UserID").Value);
            var unitofMeasure = _context.UnitofMeasures.Where(u => u.Delete == false);

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                keyword = RawWord(keyword);
                StringComparison ignoreCase = StringComparison.InvariantCultureIgnoreCase;
                unitofMeasure = unitofMeasure.Where(c => RawWord(c.Code).Contains(keyword, ignoreCase));
            }
            return Ok(unitofMeasure.ToList());
        }

        [Privilege("A006")]
        public IActionResult Create()
        {
            ViewBag.UnitofMeasure = "highlight";
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Privilege("A006")]
        public async Task<IActionResult> Create([Bind("ID,Code,Name,Delete")] UnitofMeasure unitofMeasure)
        {
            ViewBag.UnitofMeasure = "highlight";
            if (ModelState.IsValid)
            {        
                var m = _context.UnitofMeasures.FirstOrDefault(x => x.Code == unitofMeasure.Code);
                if (m == null)
                {                    
                    await _uOM.AddOrEidt(unitofMeasure);
                    return RedirectToAction(nameof(Index));
                }
                else
                {                 
                    ViewBag.Error = "This code already exist!";
                }
            }
            return View(unitofMeasure);
        }

        [Privilege("A006")]
        public IActionResult Edit(int id)
        {
            ViewBag.UnitofMeasure = "highlight";
            var unitofMeasure = _uOM.getid(id);
            if (unitofMeasure.Delete == true)
            {
                return RedirectToAction(nameof(Index));
            }
            if (unitofMeasure == null)
            {
                return NotFound();
            }
            return View(unitofMeasure);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Code,Name,Delete")] UnitofMeasure unitofMeasure)
        {
            ViewBag.UnitofMeasure = "highlight";
            if (unitofMeasure.Code == null)
            {
                ViewBag.Requied = "Pleaes input code !";
                return View(unitofMeasure);
            }
            if (ModelState.IsValid)
            {
                try
                {
                    await _uOM.AddOrEidt(unitofMeasure);
                }
                catch (Exception)
                {
                    ViewBag.code = "This is code already exist ";
                    return View(unitofMeasure);
                }
               
                return RedirectToAction(nameof(Index));
            }
            return View(unitofMeasure);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUOm(int id)
        {
            await _uOM.DeleteUOM(id);
            return Ok();
        }
  
        [HttpGet]
        public IActionResult GetUOM()
        {
            var list = _uOM.GetUnitofMeasures().Where(u=>u.Delete==false).ToList();
            return Ok(list);
        }
   }
}
