using CKBS.AppContext;
using CKBS.Models.Services.Administrator.General;
using KEDI.Core.Models.Validation;
using KEDI.Core.Premise.Authorization;
using KEDI.Core.Premise.Models.Services.Banking;
using KEDI.Core.Premise.Models.Services.Responsitory;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace KEDI.Core.Premise.Controllers
{
    [Privilege]
    public class TaxGroupController : Controller
    {
        private readonly ITaxGroup _tax;
        private readonly DataContext _context;

        public TaxGroupController(ITaxGroup tax, DataContext context)
        {
            _tax = tax;
            _context = context;
        }

        private int GetUserID()
        {
            _ = int.TryParse(User.FindFirst("UserID").Value, out int _id);
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
            ViewBag.TaxGroup = "highlight";

            return View();
        }
        public IActionResult Create()
        {
            ViewBag.TaxGroup = "highlight";
            return View();
        }
        public IActionResult GetNewOneToCreate()
        {
            var taxGroup = _tax.GetNewOneToCreate();
            return Ok(taxGroup);
        }

        public IActionResult GetActiveGlacc()
        {
            var glaccs = _tax.GetActiveGlacc(GetCompany().ID);
            return Ok(glaccs);
        }
        public IActionResult GetTaxGraoups()
        {
            var taxes = _tax.GetTaxGraoups(GetCompany().ID);
            return Ok(taxes);
        }
        public IActionResult CreateTaxGroup(TaxGroup tax)
        {
            ModelMessage msg = new();
            CheckError(tax);
            tax.CompanyID = GetCompany().ID;
            if (ModelState.IsValid)
            {
                _tax.CreateTaxGroup(tax);
                ModelState.AddModelError("success", "Tax group has been saved successfully!");
                msg.Approve();
            }
            return Ok(msg.Bind(ModelState));
        }
        public IActionResult UpdateTaxgroups(string taxes)
        {
            ModelMessage msg = new();
            List<TaxGroup> taxGroups = JsonConvert.DeserializeObject<List<TaxGroup>>(taxes,
                new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                });
            CheckError(taxGroups);
            if (ModelState.IsValid)
            {
                _tax.UpdateTaxgroups(taxGroups);
                ModelState.AddModelError("success", $"Tax Group{(taxGroups.Count > 1 ? "s have" : " has")} been updated successfully!");
                msg.Approve();
            }
            return Ok(msg.Bind(ModelState));
        }
        public IActionResult GetNewOneToCreateTd()
        {
            return Ok(_tax.GetNewOneToCreateTd());
        }
        public IActionResult GetTD(int tid)
        {
            return Ok(_tax.GetTD(tid));
        }
        private void CheckError(TaxGroup tax)
        {
            if(tax.Name == "" || tax.Name == null)
            {
                ModelState.AddModelError("Name", "Please input name tax group!");
            }
            if(tax.Code == "" || tax.Code == null)
            {
                ModelState.AddModelError("Code", "Please input code tax group!");
            }
            if(tax.Type == 0)
            {
                ModelState.AddModelError("Type", "Type can not be None!");
            }
            var allTaxGroups = _context.TaxGroups.ToList();
            if(allTaxGroups.Any(i=> i.Code == tax.Code))
            {
                ModelState.AddModelError("Code", "Code tax group is already existed!");
            }
            if (tax.TaxGroupDefinitions == null)
            {
                ModelState.AddModelError("Effectivefrom", "Effective From is invalid!");
            }
            if (tax.TaxGroupDefinitions != null)
            {
                if (tax.TaxGroupDefinitions.Count <= 0)
                {
                    ModelState.AddModelError("Effectivefrom", "Effective From is invalid!");
                }
            }
            
        }
        private void CheckError(List<TaxGroup> taxGroups)
        {
            var line = 0;
            foreach(var tax in taxGroups)
            {
                line++;
                if (tax.Name == "" || tax.Name == null)
                {
                    ModelState.AddModelError("Name", $"Please input name tax group at line {line}!");
                }
            }
        }
    }
}
