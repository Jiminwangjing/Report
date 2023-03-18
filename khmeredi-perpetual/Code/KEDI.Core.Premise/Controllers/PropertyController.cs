using CKBS.AppContext;
using CKBS.Models.Services.Administrator.General;
using CKBS.Models.Services.Inventory.Property;
using CKBS.Models.Services.Responsitory;
using KEDI.Core.Models.Validation;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KEDI.Core.Premise.Authorization;
using System.Security.Claims;

namespace CKBS.Controllers
{
    [Privilege]
    public class PropertyController : Controller
    {
        private readonly IPropertyRepository _prop;
        private readonly DataContext _context;
        public PropertyController(DataContext context, IPropertyRepository propertyRepository)
        {
            _prop = propertyRepository;
            _context = context;
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

        public IActionResult Index()
        {
            ViewBag.style = "fa fa-random";
            ViewBag.Property = "show";
            ViewBag.Index = "highlight";
            return View();
        }

        public IActionResult Create()
        {
            ViewBag.style = "fa fa-random";
            ViewBag.Property = "show";
            ViewBag.Index = "highlight";
            return View();
        }

        public IActionResult Update(int id)
        {
            ViewBag.style = "fa fa-random";
            ViewBag.Property = "show";
            ViewBag.Index = "highlight";
            var prop = _prop.GetProperty(id, GetCompany().ID);
            return View(prop);
        }

        [HttpPost]
        public IActionResult Create(Property property)
        {
            property.CompanyID = GetCompany().ID;
            if (ModelState.IsValid)
            {
                _prop.CreateOrUpdate(property);
                return RedirectToAction(nameof(Index));
            }
            return View(property);
        }
        [HttpPost]
        public IActionResult UpdateProp(Property data)
        {
            _prop.CreateOrUpdate(data);
            return Ok();
        }
        public IActionResult GetProperties()
        {
            var pro = _prop.GetProperties(GetCompany().ID);
            return Ok(pro);
        }

        public IActionResult GetActiveProperties()
        {
            var pro = _prop.GetActiveProperties(GetCompany().ID);
            return Ok(pro);
        }

        public IActionResult GetActivePropertiesOrdering()
        {
            var pro = _prop.GetActivePropertiesOrdering(GetCompany().ID);
            return Ok(pro);
        }

        public IActionResult CreateCPD(ChildPreoperty childPreoperty)
        {
            ModelMessage msg = new();
            if (childPreoperty.Name == "" || childPreoperty.Name == null)
            {
                ModelState.AddModelError("error", "Name for child property is require!");
            }
            if (ModelState.IsValid)
            {
                if (childPreoperty.ID == 0)
                {
                    ModelState.AddModelError("success", "Item saved successfully!");
                }
                _prop.CreateCPD(childPreoperty);
                msg.Approve();
            }
            return Ok(msg.Bind(ModelState));
        }

        public IActionResult GetChildrenOfProperty(int propID)
        {
            var pro = _prop.GetChildrenOfProperty(propID);
            return Ok(pro);
        }

        public IActionResult UpdateCPD(string cprop)
        {
            ModelMessage msg = new();
            if (cprop != null)
            {
                _prop.UpdateCPD(cprop);
                ModelState.AddModelError("success", "Items updated successfully!");
                msg.Approve();
            }
            else
            {
                ModelState.AddModelError("Error", "Something went wrong please and try again");
            }
            return Ok(msg.Bind(ModelState));
        }

        public IActionResult GetLastestCCP(int PropID)
        {
            var ccp = _prop.GetLastestCCP(PropID);
            return Ok(ccp);
        }

        public IActionResult DeletedCPD(int id)
        {
            ModelMessage msg = new();
            var propD = _context.PropertyDetails.ToList();
            if (propD.Any(i => i.Value == id))
            {
                ModelState.AddModelError("error", "Child Property can not delete. It already connected to Property!");
            }
            else
            {
                _prop.DeletedCPD(id);
                ModelState.AddModelError("success", "Item deleted successfully!");
                msg.Approve();
            }

            return Ok(msg.Bind(ModelState));
        }
    }
}
