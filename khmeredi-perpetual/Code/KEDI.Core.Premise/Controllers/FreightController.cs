using CKBS.AppContext;
using CKBS.Models.Services.Administrator.Setup;
using CKBS.Models.Services.Banking;
using CKBS.Models.ServicesClass.Friegh;
using KEDI.Core.Models.Validation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using KEDI.Core.Premise.Authorization;
using KEDI.Core.Premise.Models.Services.Banking;
using CKBS.Models.Services.ChartOfAccounts;
using KEDI.Core.Helpers.Enumerations;

namespace KEDI.Core.Premise.Controllers
{
    [Privilege]
    public class FreightController : Controller
    {
        private readonly DataContext _context;
        public Dictionary<int, string> FreightReceiptType => EnumHelper.ToDictionary(typeof(FreightReceiptType));
        public FreightController(DataContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            ViewBag.Freight = "highlight";
            return View();
        }
        
        public IActionResult GetGlAcc()
        {
            var glAcc = _context.GLAccounts.Where(i => i.IsActive).ToList();
            return Ok(glAcc.OrderBy(o=>o.Code));
        }

        //get data show on view
        public IActionResult Getfrieht()
        {
            var data = (from fr in _context.Freights
                        join glEx in _context.GLAccounts.DefaultIfEmpty() on fr.ExpenAcctID equals glEx.ID
                        into gex
                        from glE in gex.DefaultIfEmpty()
                        join glRev in _context.GLAccounts.DefaultIfEmpty() on fr.RevenAcctID equals glRev.ID
                        into gRev
                        from glR in gRev.DefaultIfEmpty()
                        select new FreightViewModel
                        {
                            ID = fr.ID,
                            RevenAcctID = fr.RevenAcctID,
                            AmountExpen = string.Format("{0:#,0.000}", fr.AmountExpen),
                            AmountReven = string.Format("{0:#,0.000}", fr.AmountReven),
                            Default = fr.Default,
                            ExpenAcctCode = glE.Code,
                            ExpenAcctID = fr.ExpenAcctID,
                            InTaxID = fr.InTaxID,
                            InTaxList = GetInTaxGroup().Select(i => new SelectListItem
                            {
                                Text = i.Code,
                                Value = i.ID.ToString(),
                                Selected = i.ID == fr.InTaxID
                            }).ToList(),
                            OutTaxID = fr.OutTaxID,
                            OutTaxList = GetOutTaxGroup().Select(i => new SelectListItem
                            {
                                Text = i.Code,
                                Value = i.ID.ToString(),
                                Selected = i.ID == fr.OutTaxID
                            }).ToList(),
                            FreightReceiptType = fr.FreightReceiptType,
                            FreightReceiptTypes = FreightReceiptType.Select(i => new SelectListItem
                            {
                                Text = i.Value,
                                Value = i.Key.ToString(),
                                Selected = (FreightReceiptType)i.Key == fr.FreightReceiptType
                            }).ToList(),
                            IsActive = fr.IsActive,
                            InTaxRate = fr.InTaxRate,
                            LineID = $"{DateTime.Now.Ticks + fr.ID}",
                            Name = fr.Name,
                            OutTaxRate = fr.OutTaxRate,
                            RevenAcctCode = glR.Code,
                        }).ToList();
            return Ok(data);
        }

        //get data active in TaxGroup show in select(OutTaxList)
        private List<TaxGroup> GetOutTaxGroup()
        {
            var data = _context.TaxGroups.Where(i => i.Type == TypeTax.OutPutTax && i.Active).ToList();
            // as add
            data.Insert(0, new TaxGroup
            {
                Code = "-- Select --",
                ID = 0,
            });
            return data;
        }

        //get data ative in TaxGroup show in Select(InTaxList)
        private List<TaxGroup> GetInTaxGroup()
        {
            var data = _context.TaxGroups.Where(i => i.Type == TypeTax.InputTax && i.Active).ToList();
            //as add
            data.Insert(0, new TaxGroup
            {
                Code = "-- Select --",
                ID = 0,
            });
            return data;
        }

        //when click create get data(culumn) null(0)
        public IActionResult GetEmptyData()
        {
            var data = new FreightViewModel
            {
                LineID = DateTime.Now.Ticks.ToString(),
                ID = 0,
                AmountExpen = "",       //0M(decimal)
                AmountReven = "",
                Default = false,
                ExpenAcctCode = "",
                ExpenAcctID = 0,
                InTaxID = 0,
                InTaxRate = 0M,
                Name = "",
                OutTaxID = 0,
                OutTaxRate = 0M,
                RevenAcctCode = "",
                RevenAcctID = 0,
                OutTaxList = GetOutTaxGroup().Select(i => new SelectListItem
                {
                    Text = i.Code, //+ i.Name,
                    Value = i.ID.ToString(),
                }).ToList(),
                InTaxList = GetInTaxGroup().Select(i => new SelectListItem
                {
                    Text = i.Code, //+ i.Name,
                    Value = i.ID.ToString(),
                }).ToList(),
               FreightReceiptTypes = FreightReceiptType.Select(i => new SelectListItem
                {
                        Text = i.Value,
                        Value = i.Key.ToString(),
             }).ToList()
            };
            return Ok(data);
        }

        //Save Frieht//
        public IActionResult UpdateFrieht(string data)
        {
            List<Freight> freights = JsonConvert.DeserializeObject<List<Freight>>(data,
                new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                });
            ModelMessage msg = new();
            foreach (var i in freights)
            {
                ValidateSummary(i);
            }
            if (freights.Count <= 0)
            {
                ModelState.AddModelError("NoData", "Please input some data!");
            }
            if (ModelState.IsValid)
            {
                _context.Freights.UpdateRange(freights);
                _context.SaveChanges();
                ModelState.AddModelError("success", "Frieht created succussfully!");
                msg.Approve();
            }
            return Ok(msg.Bind(ModelState));
        }

        //ValidateSummary or send messang when user forgot input data
        private void ValidateSummary(Freight frieht)
        {
            if (frieht.Name == "" || frieht.Name == null)
            {
                ModelState.AddModelError("Name", "Please Input name!");
            }
            if (frieht.RevenAcctID == 0)
            {
                ModelState.AddModelError("RevenAcctID", "Please Input Revenue Account!");
            }
            if (frieht.ExpenAcctID == 0)
            {
                ModelState.AddModelError("ExpenAcctID", "Please Input Revenue Account!");
            }
        }

        [HttpPost]
        public IActionResult SetGLDefualt(int id)
        {
            ModelMessage msg = new ();
            var frieght = _context.Freights.ToList();
            foreach (var item in frieght)
            {
                if (item.ID == id)
                {
                    item.Default = true;
                    ModelState.AddModelError("success", $"{item.Name} is set to default");
                    msg.Approve();
                }
                else
                {
                    item.Default = false;
                }
                _context.SaveChanges();

            }
            return Ok(msg.Bind(ModelState));
        }
    }
}
