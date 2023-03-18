using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using CKBS.AppContext;
using CKBS.Models.Services.HumanResources;
using KEDI.Core.Models.Validation;

using KEDI.Core.Premise.Models.Partners;
using KEDI.Core.Premise.Models.Services.HumanResources;
using KEDI.Core.Premise.Models.Services.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace KEDI.Core.Premise.Controllers
{
    
    public class LoanPartnerController : Controller
    {
        private readonly DataContext _context;
        private readonly ILoanPartnerRepo _loanpartner;

        public LoanPartnerController(DataContext context,ILoanPartnerRepo loanpartner)
        {
            _context = context;
            _loanpartner=loanpartner;
        }
        public IActionResult Index()
        {
            ViewBag.style = "fa fa-shopping-cart";
            ViewBag.Main = "LoanPartner";
            ViewBag.Page = "LoanPartner";
            ViewBag.PurchaseMenu = "show";
            ViewBag.LoanPartner = "highlight";

            return View();
        }
       
        public IActionResult CreateLoanPartner()
        {
            ViewBag.style = "fa fa-shopping-cart";
            ViewBag.Main = "LoanPartner";
            ViewBag.Page = "LoanPartner";
            ViewBag.PurchaseMenu = "show";
            ViewBag.LoanPartner = "highlight";
            var loan = _context.LoanPartners.Where(i => i.Delete == false).ToList().Count;
            decimal code=23000 + loan;
            ViewBag.Code = $"LP{code}";
          
            return View();
        }
         [HttpGet]
        public IActionResult GetLoanGroup(Grouploan group1)
        {
            var list= _context.GroupLoanPartners.Where(s=> s.Grouploan==group1);
            return Ok(list);
        }
         [HttpGet]
        public IActionResult GetSaleEmployee()
        {
            var list = (from em in _context.Employees.Where(s => s.Delete == false)
                        let empdType = _context.EmployeeTypes.FirstOrDefault(i => em.EMTypeID == i.ID) ?? new EmployeeType()
                        select new Employee
                        {
                            ID = em.ID,
                            Code = em.Code,
                            Name = em.Name,
                            GenderDisplay = em.Gender.ToString(),
                            Position = em.Position,
                            Phone = em.Phone,
                            Email = em.Email,
                            Address = em.Address,
                            EMType = empdType.Type ?? "",
                        }).OrderBy(s => s.Name).ToList();

            return Ok(list);
        }
        [HttpGet]
        public async Task<IActionResult> GetLoanPartner()
        {
            var list = await  _context.LoanPartners.Where(s=> s.Delete==false).ToListAsync();
            return Ok(list);
        }
        [HttpGet]
        public async Task<IActionResult> GetDefultRowsConteractPerson()
        {
            var contactPeople =await _loanpartner.BindRowsDefaultAsynce();
            return Ok(contactPeople);

        }
         [HttpGet]
        public async Task<IActionResult> AddRowsConteractPerson()
        {
            var objcontract= await _loanpartner.AddRowsAsynce();
            return Ok(objcontract);
        }
        [HttpPost]
        public  IActionResult CreateGroup1(GroupLoanPartner data)
        {
             ModelMessage msg = new();
             if(string.IsNullOrWhiteSpace(data.Name))
                 ModelState.AddModelError("Name", " please input Name");
            if (ModelState.IsValid)
            {
                _context.GroupLoanPartners.Update(data);
                _context.SaveChanges();
                ModelState.AddModelError("success", "Group 1 created succussfully!");
                msg.Approve();
            }
             return Ok(msg.Bind(ModelState));
        }
        [HttpPost]
        public IActionResult SaveLoanPartner(LoanPartner obj)
        {
            ModelMessage msg = new();
            obj.LoanContactPeople=obj.LoanContactPeople.Where(s=> !string.IsNullOrWhiteSpace( s.FirstName)).ToList();
            if(string.IsNullOrWhiteSpace(obj.Code))
                 ModelState.AddModelError("Code", " please Code invalid ...!");
            if(string.IsNullOrWhiteSpace(obj.Name1))
                 ModelState.AddModelError("Name1", " please input Name1 ...!");
            if(obj.LoanContactPeople.Count>0)
            {
                foreach(var item in obj.LoanContactPeople)
               {
                 if(string.IsNullOrWhiteSpace(item.ContactID))
                     ModelState.AddModelError("ContactID", " please input ContactID in tab ContractPerson ...!");
                 if(string.IsNullOrWhiteSpace(item.FirstName))
                     ModelState.AddModelError("FirstName", " please input FirstName in tab ContractPerson ...!");
                if(string.IsNullOrWhiteSpace(item.MidelName))
                     ModelState.AddModelError("MidelName", " please input MidelName in tab ContractPerson ...!");
                if(string.IsNullOrWhiteSpace(item.LastName))
                     ModelState.AddModelError("LastName", " please input LastName in tab ContractPerson ...!");
                if(string.IsNullOrWhiteSpace(item.Position))
                     ModelState.AddModelError("Position", " please input Position in tab ContractPerson ...!");
                if(string.IsNullOrWhiteSpace(item.Address))
                     ModelState.AddModelError("Address", " please input Address in tab ContractPerson ...!");
                if(string.IsNullOrWhiteSpace(item.Tel1))
                     ModelState.AddModelError("Tel1", " please input TelePhone1 in tab ContractPerson ...!");
                if(item.Gender==0)
                     ModelState.AddModelError("Gender", " please select Gender in tab ContractPerson ...!");
               }
            }
            if (ModelState.IsValid)
            {
                _context.LoanPartners.Update(obj);
                _context.SaveChanges();
                ModelState.AddModelError("success", "LoanPartners created succussfully!");
                msg.Approve();
            }
           return Ok(msg.Bind(ModelState));
        }
         public List<SelectListItem> SelectGroupLoanPartner(int Id ,Grouploan grouploan)
        {
            return _context.GroupLoanPartners.Where(p => p.Grouploan== grouploan &&!p.Delete).Select(p => new SelectListItem
            {
                Value = p.ID.ToString(),
                Text = p.Name,
                Selected = p.ID == Id,
                //Disabled = disabled
            }).ToList();
        }

        public async Task<IActionResult> UpdateLoanPartner(int id)
        {
             ViewBag.style = "fa fa-shopping-cart";
            ViewBag.Main = "LoanPartner";
            ViewBag.Page = "LoanPartner";
            ViewBag.PurchaseMenu = "show";
            ViewBag.LoanPartner = "highlight";
            
            LoanPartner obj= await _loanpartner.FindLoanPartner(id);
            ViewData["G1"] = SelectGroupLoanPartner(obj.Group1ID,Grouploan.Group1);
            ViewData["G2"] = SelectGroupLoanPartner(obj.Group2ID,Grouploan.Group2);
            return View(obj);
        }
       
    }
}