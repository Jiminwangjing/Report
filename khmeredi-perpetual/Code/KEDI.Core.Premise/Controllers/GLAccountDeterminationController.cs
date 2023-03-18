using CKBS.AppContext;
using KEDI.Core.Premise.Models.Services.Administrator.SetUp;
using CKBS.Models.Services.HumanResources;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using KEDI.Core.Premise.Authorization;
using KEDI.Core.Models.Validation;
using System;

namespace CKBS.Controllers
{
    [Privilege]
    public class GLAccountDeterminationController : Controller
    {
        private readonly DataContext _context;
        public GLAccountDeterminationController(DataContext context)
        {
            _context = context;
        }
        public IActionResult GLAccounting()
        {
            ViewBag.GLAccountDetermination = "highlight";
            return View();
        }
        public IActionResult GetSaleGLDetermination()
        {
            var __gldm = _context.SaleGLDeterminationMasters.FirstOrDefault() ?? new SaleGLDeterminationMaster();
            var bp = _context.BusinessPartners.FirstOrDefault(i => i.ID == __gldm.CusID) ?? new BusinessPartner();
            var memberCardAccount = (from mca in _context.AccountMemberCards
                                     join glaCash in _context.GLAccounts on mca.CashAccID equals glaCash.ID
                                     join glaUR in _context.GLAccounts on mca.UnearnedRevenueID equals glaUR.ID
                                     select new AccountMemberCard
                                     {
                                         CashAccCode = glaCash.Code,
                                         UnearnedRevenueID = mca.UnearnedRevenueID,
                                         ID = mca.ID,
                                         CashAccID = mca.CashAccID,
                                         CashAccName = glaCash.Name,
                                         UnearnedRevenueCode = glaUR.Code,
                                         UnearnedRevenueName = glaUR.Name
                                     }).FirstOrDefault();
            var _gldm = (from gld in _context.SaleGLAccountDeterminations.Where(i=> i.SaleGLDeterminationMasterID == __gldm.ID)
                         join glAcc in _context.GLAccounts on gld.GLID equals glAcc.ID into glAccs
                         from glAcc in glAccs.DefaultIfEmpty()
                         select new
                         {
                             gld.ID,
                             gld.TypeOfAccount,
                             GlAccCode = glAcc.Code ?? "",
                             GLAccName = glAcc.Name ?? "",
                             gld.GLID,
                             gld.Code,
                         }).ToList();
            var data = new
            {
                GLdm = __gldm,
                BP = bp,
                gld = _gldm,
                MemberCard = memberCardAccount
            };
            return Ok(data);
        }
        public IActionResult GetControlAccount()
        {
            var conR = (from car in _context.ControlAccountsReceivables
                        join glAccs in _context.GLAccounts on car.GLAID equals glAccs.ID into GlCon
                        from glAcc in GlCon.DefaultIfEmpty()
                        select new
                        {
                            car.ID,
                            car.TypeOfAccount,
                            GLCode = glAcc.Code ?? "",
                            GLName = glAcc.Name ?? "",
                            car.GLAID,
                        });
            return Ok(conR);
        }
        public IActionResult GetResources()
        {
            var RS = (from rs in _context.SaleGLAccountDeterminationResources
                      join glAcc in _context.GLAccounts on rs.GLAID equals glAcc.ID into GLRS
                      from glA in GLRS.DefaultIfEmpty()
                      select new
                      {
                          rs.ID,
                          rs.TypeOfAccount,
                          GLACode = glA.Code ?? "",
                          GLAName = glA.Name ?? "",
                          rs.GLAID,
                      });
            return Ok(RS);
        }
        public IActionResult GetGLARS()
        {
            var RS = _context.GLAccounts.Where(r => r.IsActive).ToList();
            return Ok(RS);
        }
        public IActionResult GetGlAcc(bool isAllAcc)
        {
            if (isAllAcc) {
                var data = _context.GLAccounts.Where(i => i.IsActive).OrderBy(i=> i.Code).ToList();
                return Ok(data);
            }
            else
            {
                var data = _context.GLAccounts.Where(i => i.IsControlAccount).ToList();
                return Ok(data);
            }
            
        }
        public IActionResult GetChooseCustomer()
        {
            var cus = _context.BusinessPartners.ToList();
            return Ok(cus);
        }
        public IActionResult GetChooseControl()
        {
            var con = _context.GLAccounts.Where(c => c.IsControlAccount).ToList();
            return Ok(con);
        }
        [HttpPost]
        public IActionResult Update(SaleGLDeterminationMaster saleGLDetermination)
        {
            ModelMessage msg = new(); 
            if (ModelState.IsValid)
            {
                try
                {
                    _context.SaleGLDeterminationMasters.Update(saleGLDetermination);
                    _context.SaveChanges();
                    msg.Approve();
                    ModelState.AddModelError("Success", "Saved Successfully!");
                }
                catch (Exception e)
                {
                    ModelState.AddModelError("Error", e.Message);
                }
            }
            return Ok(msg.Bind(ModelState));
        }
        [HttpPost]
        public IActionResult CASave(string controlAccountsReceivable)
        {
            //convert string to arry
            List<ControlAccountsReceivable> _ca = JsonConvert.DeserializeObject<List<ControlAccountsReceivable>>(controlAccountsReceivable,
                new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                });
            if (ModelState.IsValid)
            {
                _context.ControlAccountsReceivables.UpdateRange(_ca);
                _context.SaveChanges();
            }
            return Ok();
        }
       
    }
}
