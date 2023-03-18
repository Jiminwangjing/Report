 using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CKBS.AppContext;
using CKBS.Models.Services.Administrator.Inventory;
using CKBS.Models.Services.Pagination;
using CKBS.Models.Services.Responsitory;
using CKBS.Models.ServicesClass;
using System.Text.RegularExpressions;
using KEDI.Core.Premise.Authorization;

namespace CKBS.Controllers.Developer2
{
    [Privilege]
    public class GroupUOMController : Controller
    {
        private readonly DataContext _context;
        private readonly IGUOM _guom;

        public GroupUOMController(DataContext context,IGUOM gUOM)
        {
            _context = context;
            _guom = gUOM;
        }

        [Privilege("A007")]
        public IActionResult Index()
        {
            ViewBag.style = "fa fa-cogs";
            ViewBag.Main = "Administrator";
            ViewBag.Page = "Inventory";
            ViewBag.Subpage = "Unit of Measure Group";
            ViewBag.Administrator = "show";
            ViewBag.Inventory = "show";
            ViewBag.UnitofMeasureGroup = "highlight";
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

        public IActionResult GetGroupUOM(string keyword = "")
        {
            var list = from du in _context.GroupUOMs.Where(o => o.Delete == false)

                       select new
                       {
                           ID = du.ID,
                           Code = du.Code,
                           Name = du.Name
                       };
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                keyword = RawWord(keyword);
                StringComparison ignoreCase = StringComparison.InvariantCultureIgnoreCase;
                list = list.Where(c => RawWord(c.Code).Contains(keyword, ignoreCase));
            }
            return Ok(list.ToList());
        }
       
        [Privilege("A007")]
        public IActionResult Create()
        {
            ViewBag.style = "fa fa-cogs";
            ViewBag.Main = "Administrator";
            ViewBag.Page = "Inventory";
            ViewBag.Subpage = "Unit of Measure Group";
            ViewBag.type = "create";
            ViewBag.button = "fa-plus-circle";
            ViewBag.Inventory = "show";
            ViewBag.UnitofMeasureGroup = "highlight";
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Code,Name,Delete")] GroupUOM groupUOM)
        {

            ViewBag.style = "fa fa-cogs";
            ViewBag.Main = "Administrator";
            ViewBag.Page = "Inventory";
            ViewBag.Subpage = "Unit of Measure Group";
            ViewBag.type = "create";
            ViewBag.button = "fa-plus-circle";
            ViewBag.Inventory = "show";
            ViewBag.UnitofMeasureGroup = "highlight";

            if (ModelState.IsValid)
            {
                try
                {
                    await _guom.AddorEditGUOM(groupUOM);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception)
                {
                    ViewBag.Error = "This code already exist !";
                    return View(groupUOM);
                }
                
            }
           
            return View(groupUOM);
        }

        [Privilege("A007")]
        public IActionResult Edit(int id)
        {
            ViewBag.style = "fa fa-cogs";
            ViewBag.Main = "Administrator";
            ViewBag.Page = "Inventory";
            ViewBag.Subpage = "Unit of Measure Group";
            ViewBag.type = "Edit";
            ViewBag.button = "fa-edit";
            ViewBag.Inventory = "show";
            ViewBag.UnitofMeasureGroup = "highlight";
            var groupUOM = _guom.getid(id);
            if (groupUOM.Delete == true)
            {
                return RedirectToAction(nameof(Index));
            }
            if (groupUOM == null)
            {
                return NotFound();
            }
            return View(groupUOM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Code,Name,Delete")] GroupUOM groupUOM)
        {
            ViewBag.style = "fa fa-cogs";
            ViewBag.Main = "Administrator";
            ViewBag.Page = "Inventory";
            ViewBag.Subpage = "Unit of Measure Group";
            ViewBag.type = "Edit";
            ViewBag.button = "fa-edit";
            ViewBag.Inventory = "show";
            ViewBag.UnitofMeasureGroup = "highlight";
            if (groupUOM.Code == null)
            {
                ViewBag.required = "Please input code !";
                return View(groupUOM);
            }
            if (ModelState.IsValid)
            {
                try
                {
                    await _guom.AddorEditGUOM(groupUOM);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception)
                {
                    ViewBag.Error = "This code already exist !";
                    return View(groupUOM);
                }
            }
            return View(groupUOM);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteGroupUoM(int ID)
        {
            await _guom.DleteGUoM(ID);
            return Ok();
        }

        [HttpGet]
        public IActionResult GetDataDGroup(int ID)
        {
            var list = _guom.GetGroupDUoMs(ID).Where(g => g.GroupUoMID ==ID).ToList();
            return Ok(list);
        }

        [HttpPost]
        public async Task<IActionResult> InsertDGroupUOM(GroupDUoM groupDUoM,int ID)
        {
            await _guom.InsertDGroupUOM(groupDUoM);
            var list = _guom.GetGroupDUoMs(ID).Where(g => g.GroupUoMID == ID).ToList();
            return Ok(list);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteDefined(int ID,int IDGroup)
        {
              await _guom.DeleteDefinedGroup(ID);

              var list = _guom.GetGroupDUoMs(IDGroup).Where(g => g.GroupUoMID == IDGroup).ToList();
              return Ok(list);
        }
        
        [HttpGet]
        public IActionResult ListGroupUom(int id)
        {
            var list = _guom.GetGroupDUoMs(id).Where(x => x.GroupUoMID == id).ToList();
            return Ok(list);
        }
    }
}
