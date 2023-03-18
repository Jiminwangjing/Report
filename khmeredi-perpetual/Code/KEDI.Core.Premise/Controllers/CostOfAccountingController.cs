using CKBS.AppContext;
using CKBS.Models.Services.Administrator.General;
using CKBS.Models.Services.CostOfAccounting;
using CKBS.Models.Services.Responsitory;
using KEDI.Core.Models.Validation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using KEDI.Core.Premise.Authorization;

namespace CKBS.Controllers
{
    [Privilege]
    public class CostOfAccountingController : Controller
    {
        private readonly DataContext _context;
        private readonly ICostOfAccounting _costAcc;

        public CostOfAccountingController(DataContext dataContext, ICostOfAccounting costOfAccounting)
        {
            _context = dataContext;
            _costAcc = costOfAccounting;
        }
        public IActionResult CostCenter() 
        {
            ViewBag.style = "fa fa-random";
            ViewBag.CostOfAccounting = "show";
            ViewBag.CostCenter = "highlight";
            var costOfAccounting = new CostOfCenter();
            return View(costOfAccounting);
        }
        public IActionResult Dimension()
        {
            ViewBag.style = "fa fa-random";
            ViewBag.CostOfAccounting = "show";
            ViewBag.Dimension = "highlight";
            return View();
        }

        public IActionResult CreateDimension() 
        {
            var dimemsion = new CostOfCenter();
            return View(dimemsion);
        }

        private int GetUserID()
        {
            int.TryParse(User.FindFirst("UserID").Value, out int _id);
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

        [HttpPost]
        public IActionResult CreateDimension(CostOfCenter data)
        {
            _context.CostOfCenter.Update(data);
            _context.SaveChanges();
            var dinmen = _context.CostOfCenter.Where(i=> i.IsDimension && i.ActiveDimension).ToList();
            return Ok();
        }

        public CostOfCenter GetCategories()
        {
            return _context.CostOfCenter.FirstOrDefault(m => m.IsDimension && m.ActiveDimension);
        }

        [HttpPost]
        public IActionResult CreateCategoryByDefault()
        {
            
            CreateCategory();
            return RedirectToAction(nameof(CreateDimension));
        }

        void CreateCategory()
        {
            _context.CostOfCenter.Update(new CostOfCenter
            {
                ActiveDimension = true,
                IsDimension = true,
                CostCenter = "",
                CostOfAccountingTypeID = 0,
                EffectiveFrom = DateTime.Now,
                EffectiveTo = DateTime.Now,
                MainParentID = 0,
                Name = "Company",
                OwnerEmpID = 0,
                ParentID = 0,
                ShortCode = "",
            });
            _context.SaveChanges();
        }

        public IActionResult GetEmps()
        {
            var emps = _costAcc.GetEmps(GetCompany().ID);
            return Ok(emps);
        }

        public IActionResult GetNone()
        {
            var emps = _costAcc.GetNone(GetCompany().ID);
            return Ok(emps);
        }

        public IActionResult GetGroup(int id)
        {
            var group = _costAcc.GetGroup(id);
            return Ok(group);
        }

        public IActionResult GetDimensions()
        {
            var dimensions = _costAcc.GetDimensions(GetCompany().ID);
            return Ok(dimensions);
        }

        public IActionResult CreateCostOfAccounting(CostOfCenter costOfCenter)
        {
            ModelMessage msg = new ModelMessage();
            if (_costAcc.GetCostOfCenter(GetCompany().ID).Any(i => i.CostCenter == costOfCenter.CostCenter) && costOfCenter.ID == 0)
            {
                ModelState.AddModelError("CostCenter", "Cost Center already existed");
            }
            costOfCenter.CompanyID = GetCompany().ID;
            if (ModelState.IsValid)
            {
                _costAcc.CreateOrUpdate(costOfCenter);
                if(costOfCenter.ID == 0)
                    ModelState.AddModelError("success", "Item saved successfully.");
                if(costOfCenter.ID > 0)
                    ModelState.AddModelError("success", "Item updated successfully.");
                msg.Approve();
            }
            return Ok(msg.Bind(ModelState));
        }

        public IActionResult CreateDetailbyCategory(int id)
        {
            var newCC = _costAcc.CreateDetailbyCategory(id);
            return Ok(newCC);
        }

        public IActionResult GetCostOfCenter()
        {
            var cc = _costAcc.GetCostOfCenter(GetCompany().ID);
            return Ok(cc);
        }

        public IActionResult CreateCCT(CostOfAccountingType costOfAccountingType)
        {
            ModelMessage msg = new ModelMessage();
            var ccts = _context.CostOfAccountingTypes.ToList();
            if (costOfAccountingType.CACodeName == "" || costOfAccountingType.CACodeName == null)
            {
                ModelState.AddModelError("CACodeName", "Name is require!");
            }
            if (costOfAccountingType.CACodeType == "" || costOfAccountingType.CACodeType == null)
            {
                ModelState.AddModelError("CACodeType", "Code is require!");
            }
            if (ccts.Any(i => i.CACodeType == costOfAccountingType.CACodeType) && costOfAccountingType.ID == 0)
            {
                ModelState.AddModelError("CACodeType", "Code is already existed!");
            }
            if (ModelState.IsValid)
            {
                
                if (costOfAccountingType.ID == 0)
                {
                    ModelState.AddModelError("success", "Item saved successfully!");
                }
                if (costOfAccountingType.ID > 0)
                {
                    ModelState.AddModelError("success", "Item updated successfully!");
                }
                _costAcc.CreateCCT(costOfAccountingType);
                msg.Approve();
            }
            return Ok(msg.Bind(ModelState));
        }

        public IActionResult GetLatestCCT()
        {
            return Ok(_costAcc.GetLatestCCT());
        }

        public IActionResult GetCCT()
        {
            return Ok(_costAcc.GetCCT());
        }

        public IActionResult GetDimensionsItSelf()
        {
            var dimensions = _costAcc.GetDimensionsItSelf(GetCompany().ID);
            return Ok(dimensions);
        }

        public IActionResult UpdateDimensions(string dimensions)
        {
            ModelMessage msg = new ModelMessage();
            if (dimensions != null)
            {
                _costAcc.UpdateDimensions(dimensions);
                ModelState.AddModelError("success", "Items saved successfully!");
                msg.Approve();
            }
            else
            {
                ModelState.AddModelError("error", "Something went wrong!");
            }
            return Ok(msg.Bind(ModelState));
        }
    }
}
