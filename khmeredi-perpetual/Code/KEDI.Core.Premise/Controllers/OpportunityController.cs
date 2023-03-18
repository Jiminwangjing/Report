using CKBS.AppContext;
using CKBS.Models.Services.Account;
using CKBS.Models.Services.Administrator.SystemInitialization;
using CKBS.Models.Services.HumanResources;
using CKBS.Models.Services.OpportunitReports;
using CKBS.Models.Services.Opportunity;
using CKBS.Models.Services.OpportunityReports;
using CKBS.Models.ServicesClass.InterestRange;
using CKBS.Models.ServicesClass.OpportunityView;
using KEDI.Core.Models.Validation;
using KEDI.Core.Premise.Models.GeneralSettingAdmin;
using KEDI.Core.Premise.Models.Services.Activity;
using KEDI.Core.Premise.Models.Services.HumanResources;
using KEDI.Core.Premise.Models.ServicesClass.ActivityViewModel;
using KEDI.Core.Premise.Models.ServicesClass.GeneralSettingAdminView;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CKBS.Controllers
{
    public class OpportunityController : Controller
    {
        private readonly DataContext _context;
        public OpportunityController(DataContext context)
        {
            _context = context;

        }
        public Dictionary<int, string> ThreaLevel => new()
        {
            { 1, "Low" },
            { 2, "Medium" },
            { 3, "Hight" }
        };
        public Dictionary<int, string> PreditedClosing => new()
        {
            { 1, "Days" },
            { 2, "Months" },
            { 3, "Years" }
        };
        private GeneralSettingAdminViewModel GetGeneralSettingAdmin()
        {
            Display display = _context.Displays.FirstOrDefault() ?? new Display();
            GeneralSettingAdminViewModel data = new()
            {
                Display = display
            };
            return data;
        }
        public IActionResult Opportunity()
        {
            ViewBag.Opportunity = "highlight";
            var preditedclosing = new preditedclosingViewModel
            {
                PreditedClosing = PreditedClosing.Select(p => new SelectListItem
                {
                    Value = p.Key.ToString(),
                    Text = p.Value
                }).ToList(),
                GeneralSetting = GetGeneralSettingAdmin().Display,
            };
            //ViewData["Emp"] = new SelectList(_context.SaleEmployees, "ID", "Name");
            ViewData["Emp"] = new SelectList(_context.SaleEmployees, "Name", "Name");
            ViewData["B"] = new SelectList(_context.Employees, "ID", "Name");
            ViewData["D"] = new SelectList(_context.DocumentTypes, "ID", "Name");
            ViewBag.Level = new SelectList(_context.InterestLevels, "Description", "Description");
            return View(preditedclosing);
        }
        //get Bussiness Partner
        [HttpGet]
        public IActionResult GetBP()
        {
            var bp = (from b in _context.BusinessPartners.Where(x => x.Type == "Customer" && x.Delete == false)
                      join p in _context.PriceLists on b.PriceListID equals p.ID
                      let g1 = _context.GroupCustomer1s.FirstOrDefault(i => i.ID == b.Group1ID) ?? new GroupCustomer1()
                      let em = _context.Employees.FirstOrDefault(x => x.ID == b.SaleEMID) ?? new Employee()
                      let con = _context.ContactPersons.FirstOrDefault(x => x.BusinessPartnerID == b.ID) ?? new ContactPerson()
                      let ac = _context.Activites.Where(x => x.BPID == b.ID).ToList()
                      let cus = _context.SetupCustomerSources.FirstOrDefault(x => x.ID == b.CustomerSourceID) ?? new SetupCustomerSource()
                      let terr = _context.Territories.FirstOrDefault(x => x.ID == b.TerritoryID) ?? new KEDI.Core.Premise.Models.Services.Territory.Territory()
                      select new BusinessPartner
                      {

                          ID = b.ID,
                          Code = b.Code,
                          Name = b.Name,
                          Type = b.Type,
                          Group1 = g1.Name ?? "",
                          PriceListName = p.Name,
                          SaleEMID = em.ID,
                          SaleEmpName = em.Name,
                          Territory = terr.Name,
                          Activities = ac.Count,
                          CustomerSourceName = cus.Name,
                          ContactPeople = (from con in _context.ContactPersons.Where(x => x.BusinessPartnerID == b.ID)
                                           select new ContactPerson
                                           {
                                               ID = con.ID,
                                               ContactID = con.ContactID,
                                               Tel1 = con.Tel1,
                                               SetAsDefualt = con.SetAsDefualt
                                           }).ToList() ?? new List<ContactPerson>()

                      }
                ).ToList();

            return Ok(bp.ToList());
        }
        [HttpGet]
        public IActionResult GetEmployee()
        {
            var em = _context.Employees.Where(i => i.Delete == false).ToList();
            return Ok(em);
        }

        [HttpPost]
        public IActionResult InsertInterestLevel(List<InterestLevel> interestLevel)
        {
            ModelMessage msg = new();
            List<InterestLevel> list = new();
            foreach (var i in interestLevel)
            {
                if (!string.IsNullOrWhiteSpace(i.Description))
                    list.Add(i);
            }
            if (list.Count == 0)
            {
                ModelState.AddModelError("", "please input information ...!");
            }
            else
            {
                foreach (var li in list)
                {
                    if (string.IsNullOrWhiteSpace(li.Description))
                        ModelState.AddModelError("Description", "please input Description ...!");
                }
            }
            if (ModelState.IsValid)
            {
                _context.InterestLevels.UpdateRange(list);
                _context.SaveChanges();
                ModelState.AddModelError("success", "Opportunity created succussfully!");
                msg.Approve();
            }
            var data = _context.InterestLevels.ToList();
            return Ok(new { Model = msg.Bind(ModelState), Data = data });
        }
        [HttpGet]
        public IActionResult GetEmptyTableSetupEmloyee()
        {
            var setupemp = EmptbleSetupemp();
            return Ok(setupemp);
        }
        [HttpGet]
        public IActionResult GetEmptyTableSaleEmpDesDefault()
        {
            List<SaleEmployee> setupemp = new();
            var data = _context.SaleEmployees.ToList();
            setupemp.AddRange(data);
            for (var i = 1; i <= 5; i++)
            {
                var saleemployee = new SaleEmployee
                {
                    LineID = DateTime.Now.Ticks.ToString(),
                    ID = 0,
                    Name = "",
                };
                setupemp.Add(saleemployee);
            }
            return Ok(setupemp);

        }
        static private SaleEmployeeViewModel EmptbleSetupemp()
        {
            SaleEmployeeViewModel saleemployee = new()
            {
                LineID = DateTime.Now.Ticks.ToString(),
                ID = 0,
                Name = "",
            };
            return saleemployee;
        }

        //[HttpGet]
        //public IActionResult getSaleemp()
        //{
        //    ViewData["Emp"] = new SelectList(_context.SaleEmployees, "ID", "Name");
        //    return Ok();
        //}
        [HttpPost]
        public IActionResult InsertSaleEmp(List<SaleEmployee> saleEmployees)
        {


            ModelMessage msg = new();
            List<SaleEmployee> list = new();
            foreach (var i in saleEmployees)
            {
                if (!string.IsNullOrWhiteSpace(i.Name))
                    list.Add(i);
            }
            if (list.Count == 0)
            {
                ModelState.AddModelError("", "please input SaleEmployee ...!");
            }
            else
            {
                foreach (var li in list)
                {
                    if (string.IsNullOrWhiteSpace(li.Name))
                        ModelState.AddModelError("Name", "please input Name ...!");
                }
            }
            if (ModelState.IsValid)
            {
                _context.SaleEmployees.UpdateRange(list);
                _context.SaveChanges();
                ModelState.AddModelError("success", "Opportunity created succussfully!");
                msg.Approve();
            }
            var data = _context.SaleEmployees.ToList();
            return Ok(new { Model = msg.Bind(ModelState), Data = data });
        }


        [HttpGet]
        public IActionResult GetEmptyTable()
        {
            var interestrange = EmpDataDesPotentail();
            return Ok(interestrange);

        }

        [HttpGet]
        public IActionResult GetEmptyTablePotentailDesDefault()
        {
            List<DescriptionPotentailViewModel> descriptionPotentailViewModels = new();
            for (var i = 1; i <= 5; i++)
            {
                descriptionPotentailViewModels.Add(EmpDataDesPotentail());
            }

            return Ok(descriptionPotentailViewModels);
        }
        private DescriptionPotentailViewModel EmpDataDesPotentail()
        {
            DescriptionPotentailViewModel interestdescription = new()
            {
                LineID = DateTime.Now.Ticks.ToString(),
                ID = 0,
                selectDescription = Descriptionselect().Select(i => new SelectListItem
                {
                    Text = i.DescriptionLevel, //+ i.Name,
                    Value = i.ID.ToString(),
                }).ToList(),
                interestID = 0
            };
            return interestdescription;
        }

        private List<SetupInterestange> Descriptionselect()
        {
            var data = _context.SetupInterestRange.ToList();
            // as add

            data.Insert(0, new SetupInterestange
            {
                DescriptionLevel = "-- Select --",
                ID = 0,
            });
            return data;
        }

        [HttpGet]
        public IActionResult GetEmptyTableSetup()
        {
            var setuprange = EmptbleSetup();
            return Ok(setuprange);
        }
        [HttpGet]
        public IActionResult GetEmptyTableRangeDesDefault()
        {
            List<SetupInterestange> setupview = new();
            var data = _context.SetupInterestRange.ToList();
            setupview.AddRange(data);
            for (var i = 1; i <= 5; i++)
            {
                SetupInterestange interestrange = new()
                {
                    LineID = DateTime.Now.Ticks.ToString(),
                    ID = 0,
                    DescriptionLevel = "",
                };
                setupview.Add(interestrange);
            }
            return Ok(setupview);

        }
        static private SetUpViewModel EmptbleSetup()
        {
            SetUpViewModel interestrange = new()
            {
                LineID = DateTime.Now.Ticks.ToString(),
                ID = 0,
                DescriptionLevel = "",
            };
            return interestrange;
        }


        [HttpPost]
        public IActionResult InsertInterestRange(List<SetupInterestange> interestRange)
        {


            ModelMessage msg = new();
            List<SetupInterestange> list = new();
            foreach (var i in interestRange)
            {
                if (!string.IsNullOrWhiteSpace(i.DescriptionLevel))
                    list.Add(i);
            }
            if (list.Count == 0)
            {
                ModelState.AddModelError("", "please input information ...!");
            }
            else
            {
                foreach (var li in list)
                {
                    if (string.IsNullOrWhiteSpace(li.DescriptionLevel))
                        ModelState.AddModelError("Description", "please input Description ...!");
                }
            }
            if (ModelState.IsValid)
            {
                _context.SetupInterestRange.UpdateRange(list);
                _context.SaveChanges();
                ModelState.AddModelError("success", "Opportunity created succussfully!");
                msg.Approve();
            }
            var data = _context.SetupInterestRange.ToList();
            return Ok(new { Model = msg.Bind(ModelState), Data = data });
        }
        [HttpGet]
        public IActionResult GetEmptyTableStage()
        {
            var datastage = EmpData();
            return Ok(datastage);
        }
        [HttpGet]
        public IActionResult GetEmptyTableStageDefault()
        {
            List<StageViewModel> stageViewModels = new();
            for (var i = 1; i <= 1; i++)
            {
                stageViewModels.Add(EmpData());
            }
            return Ok(stageViewModels);

        }
        private StageViewModel EmpData()
        {
            StageViewModel datastage = new()
            {
                LineID = DateTime.Now.Ticks.ToString(),
                ID = 0,
                StartDate = DateTime.Now,
                CloseDate = DateTime.Now,
                SaleEmpselect = SaleEmp().Select(i => new SelectListItem
                {
                    Text = i.Name, //+ i.Name,
                    Value = i.ID.ToString(),
                }).ToList(),
                Nameselect = Namestage().Select(i => new SelectListItem
                {
                    Text = i.Name, //+ i.Name,
                    Value = i.ID.ToString(),

                }).ToList(),
                SetupStage = "",
                Percent = 0,
                PotentailAmount = 0,
                WeightAmount = 0,
                ShowBpsDoc = false,
                Doctypeselect = Doctype().Select(i => new SelectListItem
                {
                    Text = i.Name, //+ i.Name,
                    Value = i.ID.ToString(),
                }).ToList(),
                Activety = "",
                ActivetyID = 0,
                Owner = "",
                OwnerID = 0,
                DocNo = 0,
                SaleEmpselectID = 0,
                StagesID = 0

            };
            return datastage;
        }


        [HttpGet]
        public IActionResult Getsupstage(int nameselect)
        {
            var getstage = _context.SetUpStages.Where(c => c.ID == nameselect).FirstOrDefault();
            return Ok(getstage);
        }
        private List<DocumentType> Doctype()
        {
            var data = _context.DocumentTypes.Where(x => x.Name == "Sale Order" || x.Name == "Sale A/R" || x.Name == "Sale Delivery" || x.Name == "Sale Credit Memo" || x.Name == "Sale Quotation").ToList();
            // as add
            data.Insert(0, new DocumentType
            {
                Name = "-- Select --",
                ID = 0,
            });
            return data;
        }
        private List<Employee> SaleEmp()
        {
            var data = _context.Employees.ToList();
            // as add
            data.Insert(0, new Employee
            {
                Name = "-- Select --",
                ID = 0,
            });
            return data;
        }
        private List<SetUpStage> Namestage()
        {
            var data = _context.SetUpStages.ToList();
            // as add
            data.Insert(0, new SetUpStage
            {
                Name = "-- Select --",
                ID = 0,
            });
            return data;
        }
        [HttpGet]
        public IActionResult GetSaleOrder()
        {
            var saleorder = (from bp in _context.BusinessPartners.Where(e => e.Delete == false && e.Type == "Customer")
                             join s in _context.SaleOrders on bp.ID equals s.CusID
                             select new
                             {
                                 s.SOID,
                                 BP = bp.ID,
                                 s.InvoiceNumber,
                                 POstingDate = s.PostingDate,
                                 CusName = bp.Name

                             }).ToList();
            return Ok(saleorder);

        }

        [HttpGet]
        public IActionResult GetEmptyTableSetupStage()
        {
            var setupstage = EmptbleSetupstage();
            return Ok(setupstage);
        }
        [HttpGet]
        public IActionResult GetEmptyTableSetupStageDesDefault()
        {
            List<SetUpStage> setupstage = new();
            var data = _context.SetUpStages.ToList();
            setupstage.AddRange(data);
            for (var i = 1; i <= 5; i++)
            {
                var stagesetup = new SetUpStage
                {
                    LineID = DateTime.Now.Ticks.ToString(),
                    ID = 0,
                    Name = "",
                    ClosingPercentTage = 0,
                    StageNo = 0,

                };
                setupstage.Add(stagesetup);
            }
            return Ok(setupstage);

        }
        static private SetUpStageViewModel EmptbleSetupstage()
        {
            SetUpStageViewModel setupstage = new()
            {
                LineID = DateTime.Now.Ticks.ToString(),
                ID = 0,
                Name = "",
                StageNo = 0,
                ClosingPercentTage = 0

            };
            return setupstage;
        }


        [HttpPost]
        public IActionResult InsertSetupStage(List<SetUpStage> setupstage)
        {
            ModelMessage msg = new();
            List<SetUpStage> list = new();

            foreach (var i in setupstage)
            {
                if (!string.IsNullOrWhiteSpace(i.Name) || i.StageNo != 0)
                    list.Add(i);
            }
            if (list.Count == 0)
            {
                ModelState.AddModelError("", "please input information ...!");
            }
            else
            {
                foreach (var li in list)
                {
                    if (string.IsNullOrWhiteSpace(li.Name))
                        ModelState.AddModelError("Name", "please input Name ...!");
                    if (li.StageNo == 0)
                        ModelState.AddModelError("StageNo", "please input StageNo ...!");
                    if (li.ClosingPercentTage == 0)
                        ModelState.AddModelError("ClosingPercentTage", "please input Closing percentTage ...!");
                }
            }


            if (ModelState.IsValid)
            {
                _context.SetUpStages.UpdateRange(list);
                _context.SaveChanges();
                ModelState.AddModelError("success", "Opportunity created succussfully!");
                msg.Approve();
            }
            var data = _context.SetUpStages.ToList();
            return Ok(new { Model = msg.Bind(ModelState), Data = data });

        }

        [HttpGet]
        public IActionResult GetEmptyTablePartner()
        {
            var partner = EmpDatapartner();
            return Ok(partner);
        }
        [HttpGet]
        public IActionResult GetEmptyTablePartnerDDefault()
        {
            List<PartnerViewModel> partnerViewModels = new();
            for (var i = 1; i <= 5; i++)
            {
                partnerViewModels.Add(EmpDatapartner());
            }
            return Ok(partnerViewModels);

        }
        private PartnerViewModel EmpDatapartner()
        {
            var partner = new PartnerViewModel
            {
                LineID = DateTime.Now.Ticks.ToString(),
                ID = 0,
                Nameselect = Nameselect().Select(i => new SelectListItem
                {
                    Text = i.Name, //+ i.Name,
                    Value = i.ID.ToString(),
                }).ToList(),
                SetupPartner = "",
                Relationshipselect = Relationshipselect().Select(i => new SelectListItem
                {
                    Text = i.RelationshipDscription, //+ i.Name,
                    Value = i.ID.ToString(),
                }).ToList(),
                SetupRelationship = "",
                RelatedBp = "",
                Remark = "",
                RelationshipID = 0,
                NamePartnerID = 0
            };
            return partner;
        }

        private List<SetupRelationship> Relationshipselect()
        {
            var data = _context.SetupRelationships.ToList();
            // as add
            data.Insert(0, new SetupRelationship
            {
                RelationshipDscription = "-- Select --",
                ID = 0,
            });
            return data;
        }
        private List<SetupPartner> Nameselect()
        {
            var data = _context.SetupPartneres.ToList();
            // as add
            data.Insert(0, new SetupPartner
            {
                Name = "-- Select --",
                ID = 0,
            });
            return data;
        }
        [HttpGet]
        public IActionResult GetEmptyTableSetupPartner()
        {
            var setupstage = EmptbleSetuppartner();
            return Ok(setupstage);
        }
        //========activity
        public Dictionary<int, string> AssignedTo => new()
        {
            /*{1, "User"},*/
            { 2, "Employee" }/*, {3, "Recipient List"}*/
        };
        public Dictionary<int, string> Recurrence => new()
        {
            { 1, "None" },
            { 2, "Daily" },
            { 3, "Weekly" },
            { 4, "Monthly" },
            { 5, "Annually" }
        };
        public Dictionary<int, string> Priorities => new()
        {
            { 1, "Low" },
            { 2, "Normal" },
            { 3, "Hight" }
        };
        //public IActionResult Activities()
        //{
        //    var assignedto = new ActivityView
        //    {
        //        AssignedTo = AssignedTo.Select(p => new SelectListItem
        //        {
        //            Value = p.Key.ToString(),
        //            Text = p.Value
        //        }).ToList(),

        //        Recurrence = Recurrence.Select(p => new SelectListItem
        //        {
        //            Value = p.Key.ToString(),
        //            Text = p.Value
        //        }).ToList(),
        //        Priorities = Priorities.Select(P => new SelectListItem
        //        {
        //            Value = P.Key.ToString(),
        //            Text = P.Value
        //        }).ToList(),
        //        GeneralSetting = GetGeneralSettingAdmin().Display,
        //    };
        //    ViewBag.Activities = "highlight";
        //    ViewBag.Activity = new SelectList(_context.SetupActivities, "Activities", "Activities");
        //    ViewBag.Types = new SelectList(_context.SetupTypes, "ID", "Name");
        //    ViewBag.Subject = new SelectList(_context.SetupSubjects, "Name", "Name");
        //    ViewBag.Status = new SelectList(_context.SetupStatuses, "Status", "Status");
        //    ViewBag.Location = new SelectList(_context.SetupLocations, "Name", "Name");
        //    ViewBag.Emp = new SelectList(_context.Employees, "ID", "Name");
        //    return View(assignedto);
        //}
        //==================
        [HttpGet]
        public IActionResult GetEmptyTableSetuppartnerDesDefault()
        {
            List<SetupPartner> setuppartner = new();
            var datapartner = (from par in _context.SetupPartneres

                               select new SetupPartner
                               {
                                   ID = par.ID,
                                   Name = par.Name,
                                   DFRelationshipselect = DFRelationship().Select(i => new SelectListItem
                                   {
                                       Text = i.RelationshipDscription, //+ i.Name,
                                       Value = i.ID.ToString(),
                                       Selected = par.DFRelationship == i.ID
                                   }).ToList(),
                                   RelatedBp = par.RelatedBp,
                                   DFRelationship = par.DFRelationship,
                                   Detail = par.Detail
                               }).ToList();
            setuppartner.AddRange(datapartner);

            for (var i = 1; i <= 5; i++)
            {
                var setppartner = new SetupPartner
                {
                    LineID = DateTime.Now.Ticks.ToString(),
                    ID = 0,
                    Name = "",

                    DFRelationshipselect = DFRelationship().Select(i => new SelectListItem
                    {
                        Text = i.RelationshipDscription, //+ i.Name,
                        Value = i.ID.ToString(),
                    }).ToList(),
                    RelatedBp = "",
                    Detail = ""
                };
                setuppartner.Add(setppartner);
            }


            return Ok(setuppartner);

        }
        public IActionResult Getopp()
        {
            //var oop = _context.OpportunityMasterDatas.ToList();
            var data = (from op in _context.OpportunityMasterDatas
                        let bp = _context.BusinessPartners.Where(x => x.ID == op.BPID).FirstOrDefault() ?? new BusinessPartner()
                        select new OpportunityView
                        {
                            ID = op.ID,
                            BPID = bp.ID,
                            OpportunityNo = op.OpportunityNo,
                            BPCode = bp.Code,
                            BPName = bp.Name
                        }
                      ).ToList();
            return Ok(data);
        }
        private List<SetupCompetitor> Selectname()
        {
            var data = _context.SetupCompetitors.ToList();
            // as add
            data.Insert(0, new SetupCompetitor
            {
                Name = "-- Select --",
                ID = 0,
            });
            return data;
        }

        private SetupPartnerViewModel EmptbleSetuppartner()
        {
            var setppartner = new SetupPartnerViewModel
            {
                LineID = DateTime.Now.Ticks.ToString(),
                ID = 0,
                Name = "",

                DFRelationshipselect = DFRelationship().Select(i => new SelectListItem
                {
                    Text = i.RelationshipDscription, //+ i.Name,
                    Value = i.ID.ToString(),
                }).ToList(),
                RelatedBp = "",
                Detail = ""
            };
            return setppartner;
        }

        [HttpPost]
        public IActionResult InsertSetuppartner(List<SetupPartner> setuppartner)
        {

            ModelMessage msg = new();
            List<SetupPartner> list = new();
            foreach (var i in setuppartner)
            {
                if (!string.IsNullOrWhiteSpace(i.Name) || i.DFRelationship != 0 || !string.IsNullOrWhiteSpace(i.Detail))
                    list.Add(i);
            }
            if (list.Count == 0)
            {
                ModelState.AddModelError("", "please input information ...!");
            }
            else
            {
                foreach (var li in list)
                {
                    if (string.IsNullOrWhiteSpace(li.Name))
                        ModelState.AddModelError("Name", "please input Name ...!");

                }
            }

            if (ModelState.IsValid)
            {
                _context.SetupPartneres.UpdateRange(list);
                _context.SaveChanges();
                ModelState.AddModelError("success", "Opportunity created succussfully!");
                msg.Approve();
            }
            var data = _context.SetupPartneres.ToList();

            return Ok(new { Model = msg.Bind(ModelState), Data = data });

        }
        [HttpPost]
        public IActionResult Insertsetuprelationship(List<SetupRelationship> setuprelationship)
        {
            ModelMessage msg = new();
            List<SetupRelationship> list = new();
            foreach (var i in setuprelationship)
            {
                if (!string.IsNullOrWhiteSpace(i.RelationshipDscription))
                    list.Add(i);
            }
            if (list.Count == 0)
            {
                ModelState.AddModelError("", "please input information ...!");
            }
            else
            {
                foreach (var li in list)
                {
                    if (string.IsNullOrWhiteSpace(li.RelationshipDscription))
                        ModelState.AddModelError("Description", "please input Description ...!");
                }
            }
            if (ModelState.IsValid)
            {
                _context.SetupRelationships.UpdateRange(setuprelationship);
                _context.SaveChanges();
                ModelState.AddModelError("success", "Opportunity created succussfully!");
                msg.Approve();
            }
            var data = _context.SetupRelationships.ToList();
            return Ok(new { Model = msg.Bind(ModelState), Data = data });

        }
        private List<SetupRelationship> DFRelationship()
        {
            var data = _context.SetupRelationships.ToList();
            // as add
            data.Insert(0, new SetupRelationship
            {
                RelationshipDscription = "-- Select --",
                ID = 0,
            });
            return data;
        }
        [HttpGet]
        public IActionResult GetEmptyTableSetupRelationship()
        {
            var setuprelationship = EmptbleSetuprelatonship();
            return Ok(setuprelationship);
        }
        [HttpGet]
        public IActionResult GetEmptyTableSetuprelationshipDesDefault()
        {
            List<SetupRelationship> setuppartner = new();
            var data = _context.SetupRelationships.ToList();
            setuppartner.AddRange(data);
            for (var i = 1; i <= 5; i++)
            {
                SetupRelationship setrelationship = new()
                {
                    LineID = DateTime.Now.Ticks.ToString(),
                    ID = 0,
                    RelationshipDscription = "",
                };
                setuppartner.Add(setrelationship);
            }
            return Ok(setuppartner);

        }
        static private SetupRelationshipViewModel EmptbleSetuprelatonship()
        {
            SetupRelationshipViewModel setrelationship = new()
            {
                LineID = DateTime.Now.Ticks.ToString(),
                ID = 0,
                RelationshipDscription = "",
            };
            return setrelationship;
        }
        [HttpGet]
        public IActionResult GetEmptyTableCompetitor()
        {
            var competior = EmpDatacompetitor();
            return Ok(competior);
        }
        [HttpGet]
        public IActionResult GetEmptyTableCompeitorDefault()
        {
            List<CompetitorViewModel> competitors = new();
            for (var i = 1; i <= 5; i++)
            {
                competitors.Add(EmpDatacompetitor());
            }
            return Ok(competitors);

        }
        private CompetitorViewModel EmpDatacompetitor()
        {
            CompetitorViewModel competitor = new()
            {
                LineID = DateTime.Now.Ticks.ToString(),
                ID = 0,
                Nameselect = Selectname().Select(i => new SelectListItem
                {
                    Text = i.Name, //+ i.Name,
                    Value = i.ID.ToString(),
                }).ToList(),
                SetupCompetitor = "",
                ThreaLevel = ThreaLevel.Select(p => new SelectListItem
                {

                    Value = p.Key.ToString(),
                    Text = p.Value
                }).ToList(),
                Remark = "",
                NameCompetitorID = 0,
                ThreaLevelID = 0
            };
            return competitor;
        }
        [HttpGet]
        public IActionResult GetEmptyTableSetupCompetitor()
        {
            var setupcompetitor = EmptbleSetupcompetitor();
            return Ok(setupcompetitor);
        }
        [HttpGet]
        public IActionResult GetEmptyTableSetupcompetitorDesDefault()
        {
            List<SetupCompetitor> setupcompetitor = new();
            var datapartner = (from com in _context.SetupCompetitors
                               select new SetupCompetitor
                               {
                                   ID = com.ID,
                                   Name = com.Name,
                                   ThreaLevel = ThreaLevel.Select(p => new SelectListItem
                                   {
                                       Value = p.Key.ToString(),
                                       Text = p.Value,
                                       Selected = com.ThreaLevelID == p.Key
                                   }).ToList(),
                                   Detail = com.Detail,
                                   ThreaLevelID = com.ThreaLevelID
                               }).ToList();
            setupcompetitor.AddRange(datapartner);
            for (var i = 1; i <= 5; i++)
            {
                var setupcom = new SetupCompetitor
                {
                    LineID = DateTime.Now.Ticks.ToString(),
                    ID = 0,
                    Name = "",
                    ThreaLevel = ThreaLevel.Select(p => new SelectListItem
                    {
                        Value = p.Key.ToString(),
                        Text = p.Value
                    }).ToList(),
                    Detail = ""

                };
                setupcompetitor.Add(setupcom);
            }
            return Ok(setupcompetitor);

        }
        private SetupCompetitorViewModel EmptbleSetupcompetitor()
        {
            SetupCompetitorViewModel setupcompetitor = new()
            {
                LineID = DateTime.Now.Ticks.ToString(),
                ID = 0,
                Name = "",
                ThreaLevel = ThreaLevel.Select(p => new SelectListItem
                {
                    Value = p.Key.ToString(),
                    Text = p.Value
                }).ToList(),
                Detail = ""

            };
            return setupcompetitor;
        }

        //get empty table of setup interstlevel
        [HttpGet]
        public IActionResult GetEmptyTableSetupLevel()
        {
            var setupinterestlevel = EmptbleSetupinterestlevel();
            return Ok(setupinterestlevel);
        }
        [HttpGet]
        public IActionResult GetEmptyTableSetupInterestlevelDefalut()
        {
            List<InterestLevel> setupinterestlevel = new();
            var data = _context.InterestLevels.ToList();
            setupinterestlevel.AddRange(data);
            for (var i = 1; i <= 5; i++)
            {
                InterestLevel interestlevel = new()
                {
                    LineID = DateTime.Now.Ticks.ToString(),
                    ID = 0,
                    Description = ""
                };
                setupinterestlevel.Add(interestlevel);
            }
            return Ok(setupinterestlevel);

        }
        static private SetUpLevelViewModel EmptbleSetupinterestlevel()
        {
            SetUpLevelViewModel interestlevel = new()
            {
                LineID = DateTime.Now.Ticks.ToString(),
                ID = 0,
                Description = ""
            };
            return interestlevel;
        }

        //end of get empty table of setup interstlevel
        [HttpPost]
        public IActionResult InsertsetupCompetitor(List<SetupCompetitor> setupcompetitor)
        {
            ModelMessage msg = new();
            List<SetupCompetitor> list = new();
            foreach (var i in setupcompetitor)
            {
                if (!string.IsNullOrWhiteSpace(i.Name) || i.ThreaLevelID != 0 || !string.IsNullOrWhiteSpace(i.Detail))
                    list.Add(i);
            }
            if (list.Count == 0)
            {
                ModelState.AddModelError("", "please input information ...!");
            }
            else
            {
                foreach (var li in list)
                {
                    if (string.IsNullOrWhiteSpace(li.Name))
                        ModelState.AddModelError("Name", "please input Name ...!");

                }
            }
            if (ModelState.IsValid)
            {
                _context.SetupCompetitors.UpdateRange(setupcompetitor);
                _context.SaveChanges();
                ModelState.AddModelError("success", "Opportunity created succussfully!");
                msg.Approve();
            }
            var data = _context.SetupCompetitors.ToList();
            return Ok(new { Model = msg.Bind(ModelState), Data = data });
        }
        [HttpGet]
        public IActionResult GetEmptyTableDescription()
        {
            var summary = EmpDatadessummary();
            return Ok(summary);
        }
        [HttpGet]
        public IActionResult GetEmptyTabledessummaryDefault()
        {
            List<DescriptionSummaryViewModel> descriptionSummaryViewModels = new();
            for (var i = 1; i <= 5; i++)
            {
                descriptionSummaryViewModels.Add(EmpDatadessummary());
            }
            return Ok(descriptionSummaryViewModels);

        }
        private DescriptionSummaryViewModel EmpDatadessummary()
        {
            DescriptionSummaryViewModel description = new()
            {
                LineID = DateTime.Now.Ticks.ToString(),
                ID = 0,
                Descriptionselect = Selectdescription().Select(i => new SelectListItem
                {
                    Text = i.Description, //+ i.Name,
                    Value = i.ID.ToString(),
                }).ToList(),
                ReasonsID = 0
            };
            return description;
        }

        private List<SetupReasons> Selectdescription()
        {
            var data = _context.SetupReasons.ToList();
            // as add
            data.Insert(0, new SetupReasons
            {
                Description = "-- Select --",
                ID = 0,
            });
            return data;
        }
        [HttpGet]
        public IActionResult GetEmptyTableSetupReasons()
        {
            var setupreasons = EmptbleSetupreasons();
            return Ok(setupreasons);
        }
        [HttpGet]
        public IActionResult GetEmptyTableSetupreasonsDesDefault()
        {
            List<SetupReasons> setupreasons = new();
            var data = _context.SetupReasons.ToList();
            setupreasons.AddRange(data);
            for (var i = 1; i <= 5; i++)
            {
                var setupreason = new SetupReasons
                {
                    LineID = DateTime.Now.Ticks.ToString(),
                    ID = 0,
                    Description = ""
                };
                setupreasons.Add(setupreason);
            }
            return Ok(setupreasons);

        }
        static private SetupReasonsViewModel EmptbleSetupreasons()
        {
            SetupReasonsViewModel setupreasons = new()
            {
                LineID = DateTime.Now.Ticks.ToString(),
                ID = 0,
                Description = ""
            };
            return setupreasons;
        }


        [HttpPost]
        public IActionResult InsertSetupReasons(List<SetupReasons> setupreasons)
        {

            ModelMessage msg = new();
            List<SetupReasons> list = new();
            foreach (var i in setupreasons)
            {
                if (!string.IsNullOrWhiteSpace(i.Description))
                    list.Add(i);
            }
            if (list.Count == 0)
            {
                ModelState.AddModelError("", "please input Description ...!");
            }
            else
            {
                foreach (var li in list)
                {
                    if (string.IsNullOrWhiteSpace(li.Description))
                        ModelState.AddModelError("Description", "please input Description ...!");
                }
            }
            if (ModelState.IsValid)
            {
                _context.SetupReasons.UpdateRange(list);
                _context.SaveChanges();
                ModelState.AddModelError("success", "Opportunity created succussfully!");
                msg.Approve();
            }
            var data = _context.SetupReasons.ToList();
            return Ok(new { Model = msg.Bind(ModelState), Data = data });
        }
        [HttpGet]
        public IActionResult GetDoctypeNo(int ID)
        {
            var saleorder = _context.SaleOrders.Where(w => w.DocTypeID == ID).ToList();
            var saleAR = _context.SaleARs.Where(w => w.DocTypeID == ID).ToList();
            var salejour = _context.JournalEntries.Where(w => w.DouTypeID == ID).ToList();
            var salepur = _context.PurchaseOrders.Where(w => w.DocumentTypeID == ID).ToList();
            var goodreceiptpo = _context.GoodsReciptPOs.Where(w => w.DocumentTypeID == ID).ToList();
            var salequote = _context.SaleQuotes.Where(w => w.DocTypeID == ID).ToList();
            var saledelivery = _context.SaleDeliveries.Where(w => w.DocTypeID == ID).ToList();
            var outgoingpayment = _context.OutgoingPayments.Where(w => w.DocumentID == ID).ToList();
            var incomingpayment = _context.IncomingPayments.Where(w => w.DocTypeID == ID).ToList();
            var goodisure = _context.GoodIssues.Where(w => w.DocTypeID == ID).ToList();
            var transfer = _context.Transfers.Where(w => w.DocTypeID == ID).ToList();
            var goodrescripts = _context.GoodsReceipts.Where(w => w.DocTypeID == ID).ToList();
            var purchaseap = _context.Purchase_APs.Where(w => w.DocumentTypeID == ID).ToList();


            if (saleorder.Count > 0)
            {
                var sorder = (from doc in _context.DocumentTypes
                              join sor in _context.SaleOrders on doc.ID equals sor.DocTypeID
                              join cus in _context.BusinessPartners.Where(e => e.Delete == false && e.Type == "Customer") on sor.CusID equals cus.ID
                              select new DocTypeData
                              {
                                  ModuleID = sor.SOID,
                                  DoctypeID = doc.ID,
                                  InvoiceNo = sor.InvoiceNumber,
                                  PostingDate = sor.PostingDate,
                                  DueDate = sor.DocumentDate,
                                  CustomerName = cus.Name,
                                  Remark = sor.Remarks,
                                  SeriesID = sor.SeriesID,
                                  SeriesDID = sor.SeriesDID
                              }).ToList();
                return Ok(sorder);

            }
            else if (saleAR.Count > 0)
            {
                var salear = (from doc in _context.DocumentTypes
                              join ar in _context.SaleARs on doc.ID equals ar.DocTypeID
                              join cus in _context.BusinessPartners.Where(e => e.Delete == false && e.Type == "Customer") on ar.CusID equals cus.ID
                              select new DocTypeData
                              {
                                  ModuleID = ar.SARID,
                                  DoctypeID = doc.DocuTypeID,
                                  InvoiceNo = ar.InvoiceNumber,
                                  PostingDate = ar.PostingDate,
                                  DueDate = ar.DueDate,
                                  CustomerName = cus.Name,
                                  Remark = ar.Remarks,
                                  SeriesDID = ar.SeriesDID,
                                  SeriesID = ar.SeriesID

                              }).ToList();
                return Ok(salear);

            }
            else if (salejour.Count > 0)
            {
                var salejoural = (from doc in _context.DocumentTypes
                                  join jo in _context.JournalEntries on doc.ID equals jo.DouTypeID
                                  select new DocTypeData
                                  {
                                      ModuleID = jo.ID,
                                      DoctypeID = doc.DocuTypeID,
                                      InvoiceNo = jo.TransNo,
                                      PostingDate = jo.PostingDate,
                                      DueDate = jo.DueDate,
                                      Remark = jo.Remarks,
                                      SeriesID = jo.SeriesID,
                                      SeriesDID = jo.SeriesDID
                                  }).ToList();
                return Ok(salejoural);
            }
            else if (salepur.Count > 0)
            {
                var salepurches = (from doc in _context.DocumentTypes
                                   join pur in _context.PurchaseOrders on doc.ID equals pur.DocumentTypeID
                                   join cus in _context.BusinessPartners on pur.VendorID equals cus.ID
                                   select new DocTypeData
                                   {
                                       ModuleID = pur.PurchaseOrderID,
                                       DoctypeID = doc.DocuTypeID,
                                       InvoiceNo = pur.Number,
                                       PostingDate = pur.PostingDate,
                                       DueDate = pur.DocumentDate,
                                       CustomerName = cus.Name,
                                       Remark = pur.Remark,
                                       SeriesDID = pur.SeriesDetailID,
                                       SeriesID = pur.SeriesID

                                   }).ToList();
                return Ok(salepurches);
            }
            else if (purchaseap.Count > 0)
            {
                var salepurchesap = (from doc in _context.DocumentTypes
                                     join ap in _context.Purchase_APs on doc.ID equals ap.DocumentTypeID
                                     join cus in _context.BusinessPartners on ap.VendorID equals cus.ID
                                     select new DocTypeData
                                     {
                                         ModuleID = ap.PurchaseAPID,
                                         DoctypeID = doc.DocuTypeID,
                                         InvoiceNo = ap.InvoiceNo,
                                         PostingDate = ap.PostingDate,
                                         DueDate = ap.DueDate,
                                         CustomerName = cus.Name,
                                         Remark = ap.Remark,
                                         SeriesDID = ap.SeriesDetailID,
                                         SeriesID = ap.SeriesID

                                     }).ToList();
                return Ok(salepurchesap);
            }
            else if (goodreceiptpo.Count > 0)
            {
                var receiptpo = (from doc in _context.DocumentTypes
                                 join re in _context.GoodsReciptPOs on doc.ID equals re.DocumentTypeID
                                 join cus in _context.BusinessPartners on re.VendorID equals cus.ID

                                 select new DocTypeData
                                 {
                                     ModuleID = re.ID,
                                     DoctypeID = doc.DocuTypeID,
                                     InvoiceNo = re.Number,
                                     PostingDate = re.PostingDate,
                                     DueDate = re.DueDate,
                                     CustomerName = cus.Name,
                                     Remark = re.Remark,
                                     SeriesDID = re.SeriesDetailID,
                                     SeriesID = re.SeriesID

                                 }).ToList();
                return Ok(receiptpo);
            }
            else if (goodrescripts.Count > 0)
            {
                var receipt = (from doc in _context.DocumentTypes
                               join go in _context.GoodsReceipts on doc.ID equals go.DocTypeID

                               select new DocTypeData
                               {
                                   ModuleID = go.GoodsReceiptID,
                                   DoctypeID = doc.DocuTypeID,
                                   InvoiceNo = go.Number_No,
                                   PostingDate = go.PostingDate,
                                   DueDate = go.DocumentDate,
                                   Remark = go.Remark,
                                   SeriesDID = go.SeriseDID,
                                   SeriesID = go.SeriseID

                               }).ToList();
                return Ok(receipt);
            }
            else if (salequote.Count > 0)
            {
                var quotation = (from doc in _context.DocumentTypes
                                 join qu in _context.SaleQuotes on doc.ID equals qu.DocTypeID
                                 join cus in _context.BusinessPartners on qu.CusID equals cus.ID

                                 select new DocTypeData
                                 {
                                     ModuleID = qu.SQID,
                                     DoctypeID = doc.DocuTypeID,
                                     InvoiceNo = qu.InvoiceNo,
                                     PostingDate = qu.PostingDate,
                                     DueDate = qu.DocumentDate,
                                     CustomerName = cus.Name,
                                     Remark = qu.Remarks,
                                     SeriesDID = qu.SeriesDID,
                                     SeriesID = qu.SeriesID

                                 }).ToList();
                return Ok(quotation);
            }
            else if (saledelivery.Count > 0)
            {
                var delivery = (from doc in _context.DocumentTypes
                                join de in _context.SaleDeliveries on doc.ID equals de.DocTypeID
                                join cus in _context.BusinessPartners on de.CusID equals cus.ID

                                select new DocTypeData
                                {
                                    ModuleID = de.SDID,
                                    DoctypeID = doc.DocuTypeID,
                                    InvoiceNo = de.InvoiceNo,
                                    PostingDate = de.PostingDate,
                                    DueDate = de.DueDate,
                                    CustomerName = cus.Name,
                                    Remark = de.Remarks,
                                    SeriesDID = de.SeriesDID,
                                    SeriesID = de.SeriesID

                                }).ToList();
                return Ok(delivery);
            }

            else if (outgoingpayment.Count > 0)
            {
                var payment = (from doc in _context.DocumentTypes
                               join outs in _context.OutgoingPayments on doc.ID equals outs.DocumentID
                               join cus in _context.BusinessPartners on outs.VendorID equals cus.ID

                               select new DocTypeData
                               {
                                   ModuleID = outs.OutgoingPaymentID,
                                   DoctypeID = doc.DocuTypeID,
                                   InvoiceNo = outs.NumberInvioce,
                                   PostingDate = outs.PostingDate,
                                   DueDate = outs.DocumentDate,
                                   CustomerName = cus.Name,
                                   Remark = outs.Remark,
                                   SeriesDID = outs.SeriesDetailID,
                                   SeriesID = outs.SeriesID

                               }).ToList();
                return Ok(payment);
            }
            else if (incomingpayment.Count > 0)
            {
                var inpayment = (from doc in _context.DocumentTypes
                                 join ints in _context.IncomingPayments on doc.ID equals ints.DocTypeID
                                 join cus in _context.BusinessPartners on ints.CustomerID equals cus.ID
                                 select new DocTypeData
                                 {
                                     ModuleID = ints.IncomingPaymentID,
                                     DoctypeID = doc.DocuTypeID,
                                     InvoiceNo = ints.InvoiceNumber,
                                     PostingDate = ints.PostingDate,
                                     DueDate = ints.DocumentDate,
                                     CustomerName = cus.Name,
                                     Remark = ints.Remark,
                                     SeriesDID = ints.SeriesDID,
                                     SeriesID = ints.SeriesID

                                 }).ToList();
                return Ok(inpayment);
            }
            else if (goodisure.Count > 0)
            {
                var goodisurres = (from doc in _context.DocumentTypes
                                   join isure in _context.GoodIssues on doc.ID equals isure.DocTypeID
                                   select new DocTypeData
                                   {
                                       ModuleID = isure.GoodIssuesID,
                                       DoctypeID = doc.DocuTypeID,
                                       InvoiceNo = isure.Number_No,
                                       PostingDate = isure.PostingDate,
                                       DueDate = isure.DocumentDate,
                                       Remark = isure.Remark,
                                       SeriesDID = isure.SeriseDID,
                                       SeriesID = isure.SeriseID

                                   }).ToList();
                return Ok(goodisurres);
            }
            else if (transfer.Count > 0)
            {
                var trans = (from doc in _context.DocumentTypes
                             join tr in _context.Transfers on doc.ID equals tr.DocTypeID
                             select new DocTypeData
                             {
                                 ModuleID = tr.TarmsferID,
                                 DoctypeID = doc.DocuTypeID,
                                 InvoiceNo = tr.Number,
                                 PostingDate = tr.PostingDate,
                                 DueDate = tr.DocumentDate,
                                 Remark = tr.Remark,
                                 SeriesDID = tr.SeriseDID,
                                 SeriesID = tr.SeriseID

                             }).ToList();
                return Ok(trans);
            }

            return Ok(new { Error = true, Message = "No Data!" });

        }
        private void ValidateSummary(OpportunityMasterData opportunityMaster, List<StageDetail> stageDetail, PotentialDetail potentialDetails)
        {
            List<StageDetail> stages = new();
            if (opportunityMaster.BPID == 0)
            {
                ModelState.AddModelError("BPID", "Please choose any Bussiness Partner Code");
            }
            if (opportunityMaster.SaleEmpID == 0)
            {
                ModelState.AddModelError("SaleEmpID", "Please select Sale Employee ");

            }
            if (opportunityMaster.OpportunityNo == 0)
            {
                ModelState.AddModelError("OpportunityNo", "Please input  OpportunityNo ");
            }
            if (potentialDetails.PotentailAmount == 0)
            {
                ModelState.AddModelError("PotentailAmount", "please input Potentail Amount In Potential");
            }

            if (potentialDetails.PredictedClosingInNum == 0)
            {
                ModelState.AddModelError("PredictedClosingInNumm", "please input PredictedClosingInNum in Potential");
            }

            //stage detail
            foreach (var i in stageDetail)
            {
                if (i.Percent != 0 || i.StagesID != 0 || i.DocNo != 0 || i.OwnerID != 0 || i.DocTypeID != 0 || i.SaleEmpselectID != 0)
                    stages.Add(i);
            }
            if (stages.Count == 0)
            {
                ModelState.AddModelError("stages", "please input Data In Stage Detail");
            }
            else
            {
                foreach (var li in stages)
                {

                    if (li.SaleEmpselectID == 0)
                        ModelState.AddModelError("SaleEmpselect", "please select Sale Employee in Stage");
                    if (li.StagesID == 0)
                        ModelState.AddModelError("StagesID", "please select Stage in Stage");


                }
            }


        }
        public IActionResult GetNum()
        {
            int num;
            var list = _context.OpportunityMasterDatas.ToList();
            num = list.Count;
            if (num == 0)
            {
                num = 1;
            }
            else
            {
                num++;
            }
            return Ok(num);

        }
        [HttpPost]
        public IActionResult SubmmitData(string _data)
        {
            OpportunityMasterData data = JsonConvert.DeserializeObject<OpportunityMasterData>(_data, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });
            data.SummaryDetails.DescriptionSummaryDetails = data.SummaryDetails.DescriptionSummaryDetails
           .Where(i => i.ReasonsID > 0).ToList();
            data.PotentialDetails.DescriptionPotentialDetail = data.PotentialDetails.DescriptionPotentialDetail
                .Where(i => i.interestID > 0).ToList();
            data.PartnerDetail = data.PartnerDetail.Where(i => i.NamePartnerID > 0 || i.RelationshipID > 0 || i.RelatedBp != "" || i.Remark != "").ToList();
            data.CompetitorDetail = data.CompetitorDetail.Where(i => i.NameCompetitorID > 0 || i.ThrealevelID > 0 || i.Remark != "").ToList();
            ValidateSummary(data, data.StageDetail, data.PotentialDetails);
            ModelMessage msg = new();
            if (data.ClosingDate < data.StartDate)
            {
                ModelState.AddModelError("ClosingDate", "invalid ClosingDate date.");
            }
            if (data.PotentialDetails.PredictedClosingDate < data.StartDate)
            {
                ModelState.AddModelError("PredictedClosingDate", "Predicted Closing Date must be later than or equal to start date");
            }
            foreach (var li in data.StageDetail)
            {
                if (li.CloseDate < li.StartDate)
                    ModelState.AddModelError("CloseDate", "invalid CloseDate in StageDetail");

            }
            if (ModelState.IsValid)
            {
                _context.OpportunityMasterDatas.Update(data);
                _context.SaveChanges();
                ModelState.AddModelError("success", "Opportunity created succussfully!");
                msg.Approve();
            }
            return Ok(msg.Bind(ModelState));
        }
        //Update data
        [HttpPost]
        public IActionResult UpdateData(string _data)
        {
            OpportunityMasterData data = JsonConvert.DeserializeObject<OpportunityMasterData>(_data, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });
            data.SummaryDetails.DescriptionSummaryDetails = data.SummaryDetails.DescriptionSummaryDetails
                .Where(i => i.ReasonsID > 0).ToList();
            data.PotentialDetails.DescriptionPotentialDetail = data.PotentialDetails.DescriptionPotentialDetail
             .Where(i => i.interestID > 0).ToList();
            data.PartnerDetail = data.PartnerDetail.Where(i => i.NamePartnerID > 0 || i.RelationshipID > 0 || i.RelatedBp != "" || i.Remark != "").ToList();
            data.CompetitorDetail = data.CompetitorDetail.Where(i => i.NameCompetitorID > 0 || i.ThrealevelID > 0 || i.Remark != "").ToList();
            ValidateSummary(data, data.StageDetail, data.PotentialDetails);
            ModelMessage msg = new();
            if (data.ClosingDate < data.StartDate)
            {
                ModelState.AddModelError("ClosingDate", "invalid ClosingDate date.");
            }
            foreach (var li in data.StageDetail)
            {
                if (li.CloseDate < li.StartDate)
                    ModelState.AddModelError("CloseDate", "invalid CloseDate in StageDetail");
            }
            if (ModelState.IsValid)
            {
                _context.OpportunityMasterDatas.Update(data);
                _context.SaveChanges();
                ModelState.AddModelError("success", "Opportunity Update succussfully!");
                msg.Approve();
            }
            return Ok(msg.Bind(ModelState));

        }
        //Find Opportunity
        [HttpGet]
        public IActionResult FindOpportunity(int number)
        {

            var lost = (from op in _context.OpportunityMasterDatas.Where(x => x.OpportunityNo == number)
                        join bp in _context.BusinessPartners on op.BPID equals bp.ID
                        let ac = _context.Activites.Where(x => x.BPID == bp.ID).FirstOrDefault() ?? new Activity()
                        join po in _context.PotentialDetails on op.ID equals po.OpportunityMasterDataID
                        let su = _context.SummaryDetail.FirstOrDefault(x => x.OpportunityMasterDataID == op.ID) ?? new SummaryDetail()
                        let doc = _context.DocumentTypes.FirstOrDefault(x => x.ID == su.DocTypeID) ?? new DocumentType()
                        let emp = _context.Employees.Where(x => x.ID == op.SaleEmpID).FirstOrDefault() ?? new Employee()
                        let terr = _context.Territories.FirstOrDefault(x => x.ID == bp.TerritoryID) ?? new KEDI.Core.Premise.Models.Services.Territory.Territory()
                        let cus = _context.SetupCustomerSources.FirstOrDefault(x => x.ID == bp.CustomerSourceID) ?? new SetupCustomerSource()

                        select new OpportunityView
                        {

                            BPID = bp.ID,
                            BPName = bp.Name,
                            BPCode = bp.Code,
                            PotentialDetailsID = po.ID,
                            SummaryDetailsID = su.ID,
                            //SaleEmpName = op.Employee,
                            EmpID = emp.ID,
                            SaleEmpName = emp.Name,
                            OwnerName = op.Owner,
                            ActID = ac.ID,
                            Territery = terr.Name,
                            CusSources = cus.Name,
                            selectsaleemp = Selectname().Select(i => new SelectListItem
                            {
                                Text = i.Name, //+ i.Name,
                                Value = i.ID.ToString(),
                                //Selected = op.EmpID == i.ID
                                Selected = op.SaleEmpID == i.ID
                            }).ToList(),
                            OpportunityNo = op.OpportunityNo,
                            CloingPercentage = op.CloingPercentage,
                            ClosingDate = op.ClosingDate,
                            CompetitorDetail = (from com in _context.CompetitorDetail.Where(i => i.OpportunityMasterDataID == op.ID)
                                                select new CompetitorViewModel
                                                {
                                                    ID = com.ID,
                                                    Nameselect = Selectname().Select(i => new SelectListItem
                                                    {
                                                        Text = i.Name, //+ i.Name,
                                                        Value = i.ID.ToString(),
                                                        Selected = com.NameCompetitorID == i.ID
                                                    }).ToList(),
                                                    Remark = com.Remark,
                                                    ThreaLevel = ThreaLevel.Select(p => new SelectListItem
                                                    {
                                                        Value = p.Key.ToString(),
                                                        Text = p.Value,
                                                        //Selected = (com.ThreaLevel == null ? 0 : (int)com.ThreaLevel) == p.Key
                                                        Selected = com.ThrealevelID == p.Key
                                                    }).ToList(),
                                                    NameCompetitorID = com.NameCompetitorID,
                                                    ThreaLevelID = com.ThrealevelID
                                                }).ToList() ?? new List<CompetitorViewModel>(),
                            ID = op.ID,
                            OpportunityName = op.OpportunityName,
                            StartDate = op.StartDate,
                            Status = op.Status,
                            PartnerDetail = (from pn in _context.PartnerDetails.Where(i => i.OpportunityMasterDataID == op.ID)
                                             select new PartnerViewModel
                                             {
                                                 ID = pn.ID,
                                                 Nameselect = Nameselect().Select(i => new SelectListItem
                                                 {
                                                     Text = i.Name, //+ i.Name,
                                                     Value = i.ID.ToString(),
                                                     Selected = pn.NamePartnerID == i.ID
                                                 }).ToList(),
                                                 RelatedBp = pn.RelatedBp,
                                                 Relationshipselect = Relationshipselect().Select(i => new SelectListItem
                                                 {
                                                     Text = i.RelationshipDscription, //+ i.Name,
                                                     Value = i.ID.ToString(),
                                                     Selected = pn.RelationshipID == i.ID
                                                 }).ToList(),
                                                 Remark = pn.Remark,
                                                 NamePartnerID = pn.NamePartnerID,
                                                 RelationshipID = pn.RelationshipID

                                             }).ToList() ?? new List<PartnerViewModel>(),
                            //PotentialDetails = new PotentialDetail(),
                            StageDetail = (from sg in _context.StageDetails.Where(i => i.OpportunityMasterDataID == op.ID)
                                               //join em in _context.Employees on sg.OwnerID equals em.ID
                                               //join saleemp in _context.Employees on sg.SaleEmpselectID equals saleemp.ID
                                               //join doc in _context.DocumentTypes on sg.DocTypeID equals doc.ID
                                           let em = _context.Employees.Where(i => i.ID == sg.OwnerID).FirstOrDefault() ?? new Models.Services.HumanResources.Employee()
                                           let doc = _context.DocumentTypes.Where(i => i.ID == sg.DocTypeID).FirstOrDefault() ?? new DocumentType()
                                           select new StageViewModel
                                           {
                                               ID = sg.ID,
                                               StagesID = sg.StagesID,
                                               Doctypeselect = Doctype().Select(i => new SelectListItem
                                               {
                                                   Text = i.Name, //+ i.Name,
                                                   Value = i.ID.ToString(),
                                                   Selected = sg.DocTypeID == i.ID
                                               }).ToList(),
                                               Percent = sg.Percent,
                                               PotentailAmount = sg.PotentailAmount,
                                               WeightAmount = sg.WeightAmount,
                                               SaleEmpselect = SaleEmp().Select(i => new SelectListItem
                                               {
                                                   Text = i.Name, //+ i.Name,
                                                   Value = i.ID.ToString(),
                                                   Selected = sg.SaleEmpselectID == i.ID
                                               }).ToList(),
                                               Nameselect = Namestage().Select(i => new SelectListItem
                                               {
                                                   Text = i.Name, //+ i.Name,
                                                   Value = i.ID.ToString(),
                                                   Selected = sg.StagesID == i.ID
                                               }).ToList(),
                                               CloseDate = sg.CloseDate,
                                               StartDate = sg.StartDate,
                                               ShowBpsDoc = sg.ShowBpsDoc,
                                               Owner = em.Name,
                                               //DocNo = sale.InvoiceNumber,
                                               SaleEmpselectID = sg.SaleEmpselectID,
                                               DocNo = sg.DocNo,
                                               DoctypeID = sg.DocTypeID,
                                               OwnerID = sg.OwnerID
                                           }).ToList() ?? new List<StageViewModel>(),

                            Descriptionsummary = (from su in _context.SummaryDetail.Where(i => i.OpportunityMasterDataID == op.ID)
                                                  join de in _context.DescriptionSummaryDetails on su.ID equals de.SummaryDetailID
                                                  join re in _context.SetupReasons on de.ReasonsID equals re.ID
                                                  select new DescriptionSummaryViewModel
                                                  {
                                                      ID = de.ID,
                                                      Descriptionselect = Selectdescription().Select(i => new SelectListItem
                                                      {
                                                          Text = i.Description, //+ i.Name,
                                                          Value = i.ID.ToString(),
                                                          Selected = de.ReasonsID == i.ID
                                                      }).ToList(),
                                                      ReasonsID = de.ReasonsID
                                                  }).ToList() ?? new List<DescriptionSummaryViewModel>(),

                            //potential detail
                            Descriptionpotential = (from po in _context.PotentialDetails.Where(i => i.OpportunityMasterDataID == op.ID)
                                                    join des in _context.DescriptionPotentials on po.ID equals des.PotentialDetailID
                                                    join inr in _context.SetupInterestRange on des.interestID equals inr.ID
                                                    select new DescriptionPotentailViewModel
                                                    {
                                                        ID = des.ID,
                                                        selectDescription = Descriptionselect().Select(i => new SelectListItem
                                                        {
                                                            Text = i.DescriptionLevel, //+ i.Name,
                                                            Value = i.ID.ToString(),
                                                            Selected = des.interestID == i.ID
                                                        }).ToList(),
                                                        interestID = des.interestID
                                                    }).ToList() ?? new List<DescriptionPotentailViewModel>(),
                            PredictedClosingInNum = po.PredictedClosingInNum,
                            PredictedClosingDate = po.PredictedClosingDate,
                            PotentailAmount = po.PotentailAmount,
                            GrossProfit = po.GrossProfit,
                            GrossProfitTotal = po.GrossProfitTotal,
                            PredictedClosingInTime = po.PredictedClosingInTime,
                            WeightAmount = po.WeightAmount,
                            POID = po.ID,
                            //LevelID = le.ID,
                            Level = po.Level,
                            //summary detail
                            SeriesDID = su.SeriesDID,
                            SeriesID = su.SeriesID,
                            DoctypeID = su.DocTypeID,
                            DocType = doc.Name,
                            IsWon = su.IsWon,
                            IsLost = su.IsLost,
                            IsOpen = su.IsOpen,
                            Contact = (from con in _context.ContactPersons.Where(x => x.BusinessPartnerID == bp.ID)
                                       select new ContactPersonViewModel
                                       {
                                           ID = con.ID,
                                           ContactID = con.ContactID,
                                           Tel1 = con.Tel1,
                                           SetAsDefualt = con.SetAsDefualt
                                       }).ToList() ?? new List<ContactPersonViewModel>(),

                        }).FirstOrDefault();
            if (lost != null)
            {
                List<PartnerViewModel> par = new();
                par.AddRange(lost.PartnerDetail);
                for (int i = 1; i <= 5; i++)
                {
                    par.Add(EmpDatapartner());
                }
                lost.PartnerDetail = par;
                List<CompetitorViewModel> com = new();
                com.AddRange(lost.CompetitorDetail);
                for (int i = 1; i <= 5; i++)
                {
                    com.Add(EmpDatacompetitor());
                }
                lost.CompetitorDetail = com;
                List<DescriptionSummaryViewModel> sum = new();
                sum.AddRange(lost.Descriptionsummary);
                for (int i = 1; i <= 5; i++)
                {
                    sum.Add(EmpDatadessummary());
                }
                lost.Descriptionsummary = sum;
                List<DescriptionPotentailViewModel> dpd = new();
                dpd.AddRange(lost.Descriptionpotential);
                for (int i = 1; i <= 5; i++)
                {
                    dpd.Add(EmpDataDesPotentail());
                }
                lost.Descriptionpotential = dpd;
            }
            return Ok(lost);
        }
        //find opportunity by code name of bp

        //Opoportunity Reports
        public IActionResult GetSaleEmployee()
        {
            var saleemp = (from em in _context.SaleEmployees
                           select new
                           {
                               em.ID,
                               em.Name,
                               Action = false,
                           }).ToList();
            return Ok(saleemp);
        }
        public IActionResult OpportunityReports()
        {
            ViewBag.OpportunityReports = "highlight";
            preditedclosingViewModel preditedclosing = new()
            {
                PreditedClosing = PreditedClosing.Select(p => new SelectListItem
                {
                    Value = p.Key.ToString(),
                    Text = p.Value
                }).ToList(),
                GeneralSetting = GetGeneralSettingAdmin().Display,
            };
            var bplist = (from bp in _context.BusinessPartners
                          select new
                          {
                              bp.ID,
                              Customer = bp.Type
                          }
                        ).GroupBy(i => i.Customer.ToList()).FirstOrDefault();
            ViewData["BPType"] = new SelectList(bplist, "Type", "Type");
            //var f = GetGeneralSettingAdmin().Display;
            return View(preditedclosing);
        }
        public IActionResult SelectBp(string data)
        {
            var bps = _context.BusinessPartners.Where(x => x.Type == data).FirstOrDefault();
            return Ok(bps);
        }
        //get stage
        public IActionResult GetStage()
        {
            var stage = (from stg in _context.SetUpStages
                         select new
                         {
                             stg.ID,
                             stg.Name,
                             Action = false,
                         }).ToList();

            return Ok(stage);
        }
        //get doctype
        public IActionResult Getdoctype()
        {
            var all = _context.DocumentTypes.Where(x => x.Name == "Sale Order" || x.Name == "Sale A/R" || x.Name == "Sale Delivery" || x.Name == "Sale Quotation" || x.Name == "Purchase Order" || x.Name == "Goods Receipt PO");
            return Ok(all);
            //return Ok(sale);

        }
        public IActionResult Getdoctypes(int ID)
        {
            if (ID == 1)
            {
                var all = _context.DocumentTypes.Where(x => x.Name == "Sale Order" || x.Name == "Sale A/R" || x.Name == "Sale Delivery" || x.Name == "Sale Quotation" || x.Name == "Purchase Order" || x.Name == "Goods Receipt PO");
                return Ok(all);
            }
            else if (ID == 2)
            {
                var sale = _context.DocumentTypes.Where(x => x.Name == "Sale Order" || x.Name == "Sale A/R" || x.Name == "Sale Delivery" || x.Name == "Sale Quotation").ToList();
                return Ok(sale);
            }
            else if (ID == 3)
            {
                var pur = _context.DocumentTypes.Where(x => x.Name == "Purchase Order" || x.Name == "Goods Receipt PO").ToList();
                return Ok(pur);
            }
            return Ok();
        }
        public IActionResult GetDataEmpSetup()
        {
            var emp = _context.SaleEmployees.ToList();
            return Ok(emp);
        }
        public IActionResult GetpreLevel()
        {
            var datalevel = _context.InterestLevels.ToList();
            return Ok(datalevel);
        }
        public IActionResult GetpreRange()
        {
            var datarange = _context.SetupInterestRange.ToList();
            return Ok(datarange);
        }
        public IActionResult GetPreDataStage()
        {
            var datastage = _context.SetUpStages.ToList();
            return Ok(datastage);
        }
        [HttpGet]
        public IActionResult GetDocumenttype(int doctype)
        {
            if (doctype == 6)
            {
                var saleQoute = (from bp in _context.BusinessPartners.Where(x => x.Type == "Customer")
                                 join sale in _context.SaleQuotes on bp.ID equals sale.CusID
                                 join doc in _context.DocumentTypes on sale.DocTypeID equals doc.ID
                                 select new
                                 {
                                     sale.SQID,
                                     sale.InvoiceNumber,
                                     Doctype = doc.Name,
                                     sale.PostingDate,
                                     sale.DocumentDate,
                                     CusName = bp.Name,
                                 }).ToList();
                return Ok(saleQoute);
            }
            else if (doctype == 7)
            {
                var saleOrder = (from bp in _context.BusinessPartners.Where(x => x.Type == "Customer")
                                 join sale in _context.SaleOrders on bp.ID equals sale.CusID
                                 join doc in _context.DocumentTypes on sale.DocTypeID equals doc.ID
                                 select new
                                 {
                                     sale.SOID,
                                     sale.InvoiceNumber,
                                     Doctype = doc.Name,
                                     sale.PostingDate,
                                     sale.DocumentDate,
                                     CusName = bp.Name,
                                 }).ToList();
                return Ok(saleOrder);
            }
            else if (doctype == 8)
            {
                var saleOrder = (from bp in _context.BusinessPartners.Where(x => x.Type == "Customer")
                                 join sale in _context.SaleDeliveries on bp.ID equals sale.CusID
                                 join doc in _context.DocumentTypes on sale.DocTypeID equals doc.ID
                                 select new
                                 {
                                     sale.SDID,
                                     sale.InvoiceNumber,
                                     Doctype = doc.Name,
                                     sale.PostingDate,
                                     sale.DocumentDate,
                                     CusName = bp.Name,
                                 }).ToList();
                return Ok(saleOrder);
            }
            else if (doctype == 9)
            {
                var saleOrder = (from bp in _context.BusinessPartners.Where(x => x.Type == "Customer")
                                 join sale in _context.SaleARs on bp.ID equals sale.CusID
                                 join doc in _context.DocumentTypes on sale.DocTypeID equals doc.ID
                                 select new
                                 {
                                     sale.SARID,
                                     sale.InvoiceNumber,
                                     Doctype = doc.Name,
                                     sale.PostingDate,
                                     sale.DocumentDate,
                                     CusName = bp.Name,
                                 }).ToList();
                return Ok(saleOrder);
            }
            else if (doctype == 10)
            {
                var saleOrder = (from bp in _context.BusinessPartners.Where(x => x.Type == "Customer")
                                 join sale in _context.SaleCreditMemos on bp.ID equals sale.CusID
                                 join doc in _context.DocumentTypes on sale.DocTypeID equals doc.ID
                                 select new
                                 {
                                     sale.SCMOID,
                                     sale.InvoiceNumber,
                                     Doctype = doc.Name,
                                     sale.PostingDate,
                                     sale.DocumentDate,
                                     CusName = bp.Name,
                                 }).ToList();
                return Ok(saleOrder);
            }
            return Ok();

        }
        public IActionResult GetprePartner()
        {
            var datapartner = (from par in _context.SetupPartneres

                               select new SetupPartnerViewModel
                               {
                                   ID = par.ID,
                                   Name = par.Name,
                                   DFRelationshipselect = DFRelationship().Select(i => new SelectListItem
                                   {
                                       Text = i.RelationshipDscription, //+ i.Name,
                                       Value = i.ID.ToString(),
                                       Selected = par.DFRelationship == i.ID
                                   }).ToList(),
                                   RelatedBp = par.RelatedBp,
                                   DFRelationship = par.DFRelationship,
                                   Detail = par.Detail
                               }).ToList();
            return Ok(datapartner);


        }
        public IActionResult GetpreRelationship()
        {
            var datarelationship = _context.SetupRelationships.ToList();
            return Ok(datarelationship);
        }
        public IActionResult GetpreCompetitor()
        {
            var datapartner = (from com in _context.SetupCompetitors
                               select new SetupCompetitorViewModel
                               {
                                   ID = com.ID,
                                   Name = com.Name,
                                   ThreaLevel = ThreaLevel.Select(p => new SelectListItem
                                   {
                                       Value = p.Key.ToString(),
                                       Text = p.Value,
                                       Selected = com.ThreaLevelID == p.Key
                                   }).ToList(),
                                   Detail = com.Detail,
                                   ThreaLevelID = com.ThreaLevelID
                               }).ToList();


            return Ok(datapartner);
        }
        public IActionResult GetpreReason()
        {
            var datareason = _context.SetupReasons.ToList();
            return Ok(datareason);
        }
        public IActionResult Getliststage()
        {
            var liststage = (from std in _context.StageDetails
                             join st in _context.SetUpStages on std.StagesID equals st.ID
                             group new { std, st } by new { std.ID } into datas
                             let data = datas.FirstOrDefault()
                             select new StageView
                             {
                                 ID = data.std.ID,
                                 StageID = data.st.ID,
                                 Name = data.st.Name,
                                 StageNo = data.st.StageNo,
                                 ExcetedAmount = data.std.PotentailAmount,
                                 WeightAmount = data.std.WeightAmount,
                                 Percent = data.std.Percent,
                             }).OrderBy(r => r.StageNo).ToList();
            for (int i = 0; i < liststage.Count; i++)
            {
                for (int j = i + 1; j < liststage.Count; j++)
                {
                    if (liststage[i].Name == liststage[j].Name)
                    {
                        liststage[i].ExcetedAmount += liststage[j].ExcetedAmount;
                    }
                }
            }
            for (int i = 0; i < liststage.Count; i++)
            {
                for (int j = i + 1; j < liststage.Count; j++)
                {
                    if (liststage[i].Name == liststage[j].Name)
                    {
                        liststage[i].WeightAmount += liststage[j].WeightAmount;
                    }
                }
            }

            var result = (from list in liststage
                          group new { list } by new { list.Name } into datas
                          let data = datas.FirstOrDefault()
                          select new StageView
                          {
                              ID = data.list.ID,
                              Name = data.list.Name,
                              StageNo = data.list.StageNo,
                              ExcetedAmount = data.list.ExcetedAmount,
                              WeightAmount = data.list.WeightAmount,
                              Percent = data.list.Percent,
                          }).OrderBy(r => r.StageNo).ToList();
            return Ok(result);
        }
        public IActionResult Getsaleemp()
        {
            var saleemp = _context.SaleEmployees.FirstOrDefault();
            return Ok(saleemp);
        }
        //get by search sale emp
        public IActionResult GetstagebyEmp(int ID)
        {
            var stagelist = (from saleemp in _context.SaleEmployees.Where(x => x.ID == ID)
                             join std in _context.StageDetails on saleemp.ID equals std.SaleEmpselectID
                             join st in _context.SetUpStages on std.StagesID equals st.ID
                             select new StageView
                             {
                                 ID = std.ID,
                                 StageID = st.ID,
                                 SaleempID = saleemp.ID,
                                 Name = st.Name,
                                 StageNo = st.StageNo,
                                 ExcetedAmount = std.PotentailAmount,
                                 WeightAmount = std.WeightAmount,
                                 Percent = st.ClosingPercentTage,
                             }).FirstOrDefault();

            return Ok(stagelist);
        }
        //search by date
        public IActionResult GetStageBydate(String DateFrom, String DateTo, String CDateFrom, String CDateTo, string PreDateFrom, string PreDateTo)
        {
            if (DateFrom != null && DateTo != null && CDateFrom == null && CDateTo == null && PreDateFrom == null && PreDateTo == null)
            {
                var liststage = (from std in _context.StageDetails.Where(w => w.StartDate >= Convert.ToDateTime(DateFrom) && w.StartDate <= Convert.ToDateTime(DateTo))
                                 join st in _context.SetUpStages on std.StagesID equals st.ID
                                 group new { std, st } by new { std.ID } into datas
                                 let data = datas.FirstOrDefault()
                                 select new StageView
                                 {
                                     ID = data.std.ID,
                                     StageID = data.st.ID,
                                     Name = data.st.Name,
                                     StageNo = data.st.StageNo,
                                     ExcetedAmount = data.std.PotentailAmount,
                                     WeightAmount = data.std.WeightAmount,
                                     Percent = data.st.ClosingPercentTage,
                                 }).OrderBy(r => r.StageNo).ToList();
                for (int i = 0; i < liststage.Count; i++)
                {
                    for (int j = i + 1; j < liststage.Count; j++)
                    {
                        if (liststage[i].Name == liststage[j].Name)
                        {
                            liststage[i].ExcetedAmount += liststage[j].ExcetedAmount;
                        }
                    }
                }
                for (int i = 0; i < liststage.Count; i++)
                {
                    for (int j = i + 1; j < liststage.Count; j++)
                    {
                        if (liststage[i].Name == liststage[j].Name)
                        {
                            liststage[i].WeightAmount += liststage[j].WeightAmount;
                        }
                    }
                }
                var result = (from list in liststage
                              group new { list } by new { list.Name } into datas
                              let data = datas.FirstOrDefault()
                              select new StageView
                              {
                                  ID = data.list.ID,
                                  Name = data.list.Name,
                                  StageNo = data.list.StageNo,
                                  ExcetedAmount = data.list.ExcetedAmount,
                                  WeightAmount = data.list.WeightAmount,
                                  Percent = data.list.Percent,
                              }).OrderBy(r => r.StageNo).ToList();
                return Ok(result);

            }
            else if (DateFrom == null && DateTo == null && PreDateFrom == null && PreDateTo == null && CDateFrom != null && CDateTo != null)
            {
                var liststage = (from std in _context.StageDetails.Where(w => w.CloseDate >= Convert.ToDateTime(CDateFrom) && w.CloseDate <= Convert.ToDateTime(CDateTo))
                                 join st in _context.SetUpStages on std.StagesID equals st.ID
                                 group new { std, st } by new { std.ID } into datas
                                 let data = datas.FirstOrDefault()
                                 select new StageView
                                 {
                                     ID = data.std.ID,
                                     StageID = data.st.ID,
                                     Name = data.st.Name,
                                     StageNo = data.st.StageNo,
                                     ExcetedAmount = data.std.PotentailAmount,
                                     WeightAmount = data.std.WeightAmount,
                                     Percent = data.st.ClosingPercentTage,
                                 }).OrderBy(r => r.StageNo).ToList();
                for (int i = 0; i < liststage.Count; i++)
                {
                    for (int j = i + 1; j < liststage.Count; j++)
                    {
                        if (liststage[i].Name == liststage[j].Name)
                        {
                            liststage[i].ExcetedAmount += liststage[j].ExcetedAmount;
                        }
                    }
                }
                for (int i = 0; i < liststage.Count; i++)
                {
                    for (int j = i + 1; j < liststage.Count; j++)
                    {
                        if (liststage[i].Name == liststage[j].Name)
                        {
                            liststage[i].WeightAmount += liststage[j].WeightAmount;
                        }
                    }
                }
                var result = (from list in liststage
                              group new { list } by new { list.Name } into datas
                              let data = datas.FirstOrDefault()
                              select new StageView
                              {
                                  ID = data.list.ID,
                                  Name = data.list.Name,
                                  StageNo = data.list.StageNo,
                                  ExcetedAmount = data.list.ExcetedAmount,
                                  WeightAmount = data.list.WeightAmount,
                                  Percent = data.list.Percent,
                              }).OrderBy(r => r.StageNo).ToList();
                return Ok(result);
            }
            else if (DateFrom == null && DateTo == null && CDateFrom == null && CDateTo == null && PreDateFrom != null && PreDateTo != null)
            {
                var liststage = (from po in _context.PotentialDetails.Where(x => x.PredictedClosingDate <= Convert.ToDateTime(PreDateFrom) && x.PredictedClosingDate <= Convert.ToDateTime(PreDateTo))
                                 join op in _context.OpportunityMasterDatas on po.OpportunityMasterDataID equals op.ID
                                 join std in _context.StageDetails on op.ID equals std.OpportunityMasterDataID
                                 join st in _context.SetUpStages on std.StagesID equals st.ID
                                 group new { std, st } by new { std.ID } into datas
                                 let data = datas.FirstOrDefault()
                                 select new StageView
                                 {
                                     ID = data.std.ID,
                                     StageID = data.st.ID,
                                     Name = data.st.Name,
                                     StageNo = data.st.StageNo,
                                     ExcetedAmount = data.std.PotentailAmount,
                                     WeightAmount = data.std.WeightAmount,
                                     Percent = data.st.ClosingPercentTage,
                                 }).OrderBy(r => r.StageNo).ToList();
                for (int i = 0; i < liststage.Count; i++)
                {
                    for (int j = i + 1; j < liststage.Count; j++)
                    {
                        if (liststage[i].Name == liststage[j].Name)
                        {
                            liststage[i].ExcetedAmount += liststage[j].ExcetedAmount;
                        }
                    }
                }
                for (int i = 0; i < liststage.Count; i++)
                {
                    for (int j = i + 1; j < liststage.Count; j++)
                    {
                        if (liststage[i].Name == liststage[j].Name)
                        {
                            liststage[i].WeightAmount += liststage[j].WeightAmount;
                        }
                    }
                }
                var result = (from list in liststage
                              group new { list } by new { list.Name } into datas
                              let data = datas.FirstOrDefault()
                              select new StageView
                              {
                                  ID = data.list.ID,
                                  Name = data.list.Name,
                                  StageNo = data.list.StageNo,
                                  ExcetedAmount = data.list.ExcetedAmount,
                                  WeightAmount = data.list.WeightAmount,
                                  Percent = data.list.Percent,
                              }).OrderBy(r => r.StageNo).ToList();
                return Ok(result);
            }
            else if (DateFrom != null && DateTo != null && CDateFrom != null && CDateTo != null && PreDateFrom == null && PreDateTo == null)
            {
                var liststage = (from std in _context.StageDetails.Where(w => w.StartDate >= Convert.ToDateTime(DateFrom) && w.StartDate <= Convert.ToDateTime(DateTo) && w.CloseDate >= Convert.ToDateTime(CDateFrom) && w.CloseDate <= Convert.ToDateTime(DateTo))
                                 join st in _context.SetUpStages on std.StagesID equals st.ID
                                 group new { std, st } by new { std.ID } into datas
                                 let data = datas.FirstOrDefault()
                                 select new StageView
                                 {
                                     ID = data.std.ID,
                                     StageID = data.st.ID,
                                     Name = data.st.Name,
                                     StageNo = data.st.StageNo,
                                     ExcetedAmount = data.std.PotentailAmount,
                                     WeightAmount = data.std.WeightAmount,
                                     Percent = data.st.ClosingPercentTage,
                                 }).OrderBy(r => r.StageNo).ToList();
                for (int i = 0; i < liststage.Count; i++)
                {
                    for (int j = i + 1; j < liststage.Count; j++)
                    {
                        if (liststage[i].Name == liststage[j].Name)
                        {
                            liststage[i].ExcetedAmount += liststage[j].ExcetedAmount;
                        }
                    }
                }
                for (int i = 0; i < liststage.Count; i++)
                {
                    for (int j = i + 1; j < liststage.Count; j++)
                    {
                        if (liststage[i].Name == liststage[j].Name)
                        {
                            liststage[i].WeightAmount += liststage[j].WeightAmount;
                        }
                    }
                }
                var result = (from list in liststage
                              group new { list } by new { list.Name } into datas
                              let data = datas.FirstOrDefault()
                              select new StageView
                              {
                                  ID = data.list.ID,
                                  Name = data.list.Name,
                                  StageNo = data.list.StageNo,
                                  ExcetedAmount = data.list.ExcetedAmount,
                                  WeightAmount = data.list.WeightAmount,
                                  Percent = data.list.Percent,
                              }).OrderBy(r => r.StageNo).ToList();
                return Ok(result);
            }
            else if (DateFrom != null && DateTo != null && CDateFrom != null && CDateTo != null && PreDateFrom != null && PreDateTo != null)
            {
                var liststage = (from std in _context.StageDetails.Where(w => w.StartDate >= Convert.ToDateTime(DateFrom) && w.StartDate <= Convert.ToDateTime(DateTo) && w.CloseDate >= Convert.ToDateTime(CDateFrom) && w.CloseDate <= Convert.ToDateTime(DateTo))
                                 join op in _context.OpportunityMasterDatas on std.OpportunityMasterDataID equals op.ID
                                 join po in _context.PotentialDetails.Where(w => w.PredictedClosingDate >= Convert.ToDateTime(PreDateFrom) && w.PredictedClosingDate <= Convert.ToDateTime(PreDateTo)) on op.ID equals po.OpportunityMasterDataID
                                 join st in _context.SetUpStages on std.StagesID equals st.ID
                                 group new { std, st } by new { std.ID } into datas
                                 let data = datas.FirstOrDefault()
                                 select new StageView
                                 {
                                     ID = data.std.ID,
                                     StageID = data.st.ID,
                                     Name = data.st.Name,
                                     StageNo = data.st.StageNo,
                                     ExcetedAmount = data.std.PotentailAmount,
                                     WeightAmount = data.std.WeightAmount,
                                     Percent = data.std.Percent,
                                 }).OrderBy(r => r.StageNo).ToList();
                for (int i = 0; i < liststage.Count; i++)
                {
                    for (int j = i + 1; j < liststage.Count; j++)
                    {
                        if (liststage[i].Name == liststage[j].Name)
                        {
                            liststage[i].ExcetedAmount += liststage[j].ExcetedAmount;
                        }
                    }
                }
                for (int i = 0; i < liststage.Count; i++)
                {
                    for (int j = i + 1; j < liststage.Count; j++)
                    {
                        if (liststage[i].Name == liststage[j].Name)
                        {
                            liststage[i].WeightAmount += liststage[j].WeightAmount;
                        }
                    }
                }
                var result = (from list in liststage
                              group new { list } by new { list.Name } into datas
                              let data = datas.FirstOrDefault()
                              select new StageView
                              {
                                  ID = data.list.ID,
                                  Name = data.list.Name,
                                  StageNo = data.list.StageNo,
                                  ExcetedAmount = data.list.ExcetedAmount,
                                  WeightAmount = data.list.WeightAmount,
                                  Percent = data.list.Percent,
                              }).OrderBy(r => r.StageNo).ToList();
                return Ok(result);
            }

            return Ok();

        }
        //serarch by percent rate
        public IActionResult GetStagebyPercent(float PerFrom, float PerTo)
        {
            if (PerFrom != 0 && PerTo != 0)
            {
                var liststage = (from std in _context.StageDetails.Where(x => x.Percent >= PerFrom && x.Percent <= PerTo)
                                 join st in _context.SetUpStages on std.StagesID equals st.ID
                                 group new { std, st } by new { std.ID } into datas
                                 let data = datas.FirstOrDefault()
                                 select new StageView
                                 {
                                     ID = data.std.ID,
                                     StageID = data.st.ID,
                                     Name = data.st.Name,
                                     StageNo = data.st.StageNo,
                                     ExcetedAmount = data.std.PotentailAmount,
                                     WeightAmount = data.std.WeightAmount,
                                     Percent = data.std.Percent,
                                 }).OrderBy(r => r.StageNo).ToList();
                for (int i = 0; i < liststage.Count; i++)
                {
                    for (int j = i + 1; j < liststage.Count; j++)
                    {
                        if (liststage[i].Name == liststage[j].Name)
                        {
                            liststage[i].ExcetedAmount += liststage[j].ExcetedAmount;
                        }
                    }
                }
                for (int i = 0; i < liststage.Count; i++)
                {
                    for (int j = i + 1; j < liststage.Count; j++)
                    {
                        if (liststage[i].Name == liststage[j].Name)
                        {
                            liststage[i].WeightAmount += liststage[j].WeightAmount;
                        }
                    }
                }
                var result = (from list in liststage
                              group new { list } by new { list.Name } into datas
                              let data = datas.FirstOrDefault()
                              select new StageView
                              {
                                  ID = data.list.ID,
                                  Name = data.list.Name,
                                  StageNo = data.list.StageNo,
                                  ExcetedAmount = data.list.ExcetedAmount,
                                  WeightAmount = data.list.WeightAmount,
                                  Percent = data.list.Percent,
                              }).OrderBy(r => r.StageNo).ToList();
                return Ok(result);
            }

            return Ok();
        }
        //search by  amount
        public IActionResult GetStageByamount(decimal Frompotential, decimal Topotential, decimal Fromweightamount, decimal Toweightamount, decimal Fromprofit, decimal Toprofit)
        {
            if (Frompotential != 0 && Topotential != 0 && Fromweightamount == 0 && Toweightamount == 0 && Fromprofit == 0 && Toprofit == 0)
            {
                var liststage = (from std in _context.StageDetails.Where(x => x.PotentailAmount >= Frompotential && x.PotentailAmount <= Topotential)
                                 join st in _context.SetUpStages on std.StagesID equals st.ID
                                 group new { std, st } by new { std.ID } into datas
                                 let data = datas.FirstOrDefault()
                                 select new StageView
                                 {
                                     ID = data.std.ID,
                                     StageID = data.st.ID,
                                     Name = data.st.Name,
                                     StageNo = data.st.StageNo,
                                     ExcetedAmount = data.std.PotentailAmount,
                                     WeightAmount = data.std.WeightAmount,
                                     Percent = data.std.Percent,
                                 }).OrderBy(r => r.StageNo).ToList(); ;
                for (int i = 0; i < liststage.Count; i++)
                {
                    for (int j = i + 1; j < liststage.Count; j++)
                    {
                        if (liststage[i].Name == liststage[j].Name)
                        {
                            liststage[i].ExcetedAmount += liststage[j].ExcetedAmount;
                        }
                    }
                }
                for (int i = 0; i < liststage.Count; i++)
                {
                    for (int j = i + 1; j < liststage.Count; j++)
                    {
                        if (liststage[i].Name == liststage[j].Name)
                        {
                            liststage[i].WeightAmount += liststage[j].WeightAmount;
                        }
                    }
                }
                var result = (from list in liststage
                              group new { list } by new { list.Name } into datas
                              let data = datas.FirstOrDefault()
                              select new StageView
                              {
                                  ID = data.list.ID,
                                  Name = data.list.Name,
                                  StageNo = data.list.StageNo,
                                  ExcetedAmount = data.list.ExcetedAmount,
                                  WeightAmount = data.list.WeightAmount,
                                  Percent = data.list.Percent,
                              }).OrderBy(r => r.StageNo).ToList();
                return Ok(result);

            }
            else if (Frompotential == 0 && Topotential == 0 && Fromprofit == 0 && Toprofit == 0 && Fromweightamount != 0 && Toweightamount != 0)
            {
                var liststage = (from std in _context.StageDetails.Where(x => x.WeightAmount >= Fromweightamount && x.WeightAmount <= Toweightamount)
                                 join st in _context.SetUpStages on std.StagesID equals st.ID
                                 group new { std, st } by new { std.ID } into datas
                                 let data = datas.FirstOrDefault()
                                 select new StageView
                                 {
                                     ID = data.std.ID,
                                     StageID = data.st.ID,
                                     Name = data.st.Name,
                                     StageNo = data.st.StageNo,
                                     ExcetedAmount = data.std.PotentailAmount,
                                     WeightAmount = data.std.WeightAmount,
                                     Percent = data.std.Percent,
                                 }).OrderBy(r => r.StageNo).ToList();
                for (int i = 0; i < liststage.Count; i++)
                {
                    for (int j = i + 1; j < liststage.Count; j++)
                    {
                        if (liststage[i].Name == liststage[j].Name)
                        {
                            liststage[i].ExcetedAmount += liststage[j].ExcetedAmount;
                        }
                    }
                }
                for (int i = 0; i < liststage.Count; i++)
                {
                    for (int j = i + 1; j < liststage.Count; j++)
                    {
                        if (liststage[i].Name == liststage[j].Name)
                        {
                            liststage[i].WeightAmount += liststage[j].WeightAmount;
                        }
                    }
                }
                var result = (from list in liststage
                              group new { list } by new { list.Name } into datas
                              let data = datas.FirstOrDefault()
                              select new StageView
                              {
                                  ID = data.list.ID,
                                  Name = data.list.Name,
                                  StageNo = data.list.StageNo,
                                  ExcetedAmount = data.list.ExcetedAmount,
                                  WeightAmount = data.list.WeightAmount,
                                  Percent = data.list.Percent,
                              }).OrderBy(r => r.StageNo).ToList();
                return Ok(result);

            }
            else if (Fromprofit != 0 && Toprofit != 0 && Frompotential == 0 && Topotential == 0 && Fromweightamount == 0 && Toweightamount == 0)
            {
                var liststage = (from gr in _context.PotentialDetails.Where(x => x.GrossProfit >= Fromprofit && x.GrossProfit <= Toprofit)
                                 join op in _context.OpportunityMasterDatas on gr.OpportunityMasterDataID equals op.ID
                                 join std in _context.StageDetails on op.ID equals std.OpportunityMasterDataID
                                 join st in _context.SetUpStages on std.StagesID equals st.ID
                                 group new { std, st } by new { std.ID } into datas
                                 let data = datas.FirstOrDefault()
                                 select new StageView
                                 {
                                     ID = data.std.ID,
                                     StageID = data.st.ID,
                                     Name = data.st.Name,
                                     StageNo = data.st.StageNo,
                                     ExcetedAmount = data.std.PotentailAmount,
                                     WeightAmount = data.std.WeightAmount,
                                     Percent = data.std.Percent,
                                 }).OrderBy(r => r.StageNo).ToList();
                for (int i = 0; i < liststage.Count; i++)
                {
                    for (int j = i + 1; j < liststage.Count; j++)
                    {
                        if (liststage[i].Name == liststage[j].Name)
                        {
                            liststage[i].ExcetedAmount += liststage[j].ExcetedAmount;
                        }
                    }
                }
                for (int i = 0; i < liststage.Count; i++)
                {
                    for (int j = i + 1; j < liststage.Count; j++)
                    {
                        if (liststage[i].Name == liststage[j].Name)
                        {
                            liststage[i].WeightAmount += liststage[j].WeightAmount;
                        }
                    }
                }
                var result = (from list in liststage
                              group new { list } by new { list.Name } into datas
                              let data = datas.FirstOrDefault()
                              select new StageView
                              {
                                  ID = data.list.ID,
                                  Name = data.list.Name,
                                  StageNo = data.list.StageNo,
                                  ExcetedAmount = data.list.ExcetedAmount,
                                  WeightAmount = data.list.WeightAmount,
                                  Percent = data.list.Percent,
                              }).OrderBy(r => r.StageNo).ToList();
                return Ok(result);

            }
            else if (Frompotential != 0 && Topotential != 0 && Fromweightamount != 0 && Toweightamount != 0 && Fromprofit != 0 && Toprofit != 0)
            {
                var liststage = (from std in _context.StageDetails.Where(x => x.PotentailAmount >= Frompotential && x.PotentailAmount <= Topotential && x.WeightAmount >= Fromweightamount && x.WeightAmount <= Toweightamount)
                                 join st in _context.SetUpStages on std.StagesID equals st.ID
                                 group new { std, st } by new { std.ID } into datas
                                 let data = datas.FirstOrDefault()
                                 select new StageView
                                 {
                                     ID = data.std.ID,
                                     StageID = data.st.ID,
                                     Name = data.st.Name,
                                     StageNo = data.st.StageNo,
                                     ExcetedAmount = data.std.PotentailAmount,
                                     WeightAmount = data.std.WeightAmount,
                                     Percent = data.std.Percent,
                                 }).OrderBy(r => r.StageNo).ToList();
                for (int i = 0; i < liststage.Count; i++)
                {
                    for (int j = i + 1; j < liststage.Count; j++)
                    {
                        if (liststage[i].Name == liststage[j].Name)
                        {
                            liststage[i].ExcetedAmount += liststage[j].ExcetedAmount;
                        }
                    }
                }
                for (int i = 0; i < liststage.Count; i++)
                {
                    for (int j = i + 1; j < liststage.Count; j++)
                    {
                        if (liststage[i].Name == liststage[j].Name)
                        {
                            liststage[i].WeightAmount += liststage[j].WeightAmount;
                        }
                    }
                }
                var result = (from list in liststage
                              group new { list } by new { list.Name } into datas
                              let data = datas.FirstOrDefault()
                              select new StageView
                              {
                                  ID = data.list.ID,
                                  Name = data.list.Name,
                                  StageNo = data.list.StageNo,
                                  ExcetedAmount = data.list.ExcetedAmount,
                                  WeightAmount = data.list.WeightAmount,
                                  Percent = data.list.Percent,
                              }).OrderBy(r => r.StageNo).ToList();
                return Ok(result);

            }

            return Ok();
        }
        public IActionResult GetStageReports()
        {


            var liststagereports = (from st in _context.SetUpStages
                                    join std in _context.StageDetails on st.ID equals std.StagesID
                                    select new StageView
                                    {
                                        ID = st.ID,
                                        Name = st.Name,
                                        StageNo = st.StageNo,
                                        Percent = std.Percent,
                                        WeightAmount = std.WeightAmount,
                                        ExcetedAmount = std.PotentailAmount
                                    }).OrderBy(r => r.StageNo).ToList();

            return Ok(liststagereports);
        }
        //search by sale emp check
        public IActionResult Getbpbycheck(string searchStageBysaleEmp)
        {
            //List < SaleEmployee> saleEmployees = new List<SaleEmployee>();
            //var datas = JsonConvert.DeserializeObject<List<SaleEmployee>>(searchStageBysaleEmp, new JsonSerializerSettings
            //{
            //    NullValueHandling = NullValueHandling.Ignore
            //});
            //var list = _context.SaleEmployees.Where(datas.Any(_x => _x.ID == x.ID)).ToList();

            List<StageView> stageViews = new();
            List<StageView> stageViews2 = new();
            foreach (var lis in searchStageBysaleEmp)
            {
                var list = _context.Employees.Where(s => s.ID == Convert.ToInt32(lis)).ToList();
                stageViews = GetStageList(list);
                stageViews2.AddRange(stageViews);
            }
            List<StageView> dataStages = new(
                 stageViews2.Count
              );
            dataStages.AddRange(stageViews2);
            return Ok(dataStages);
        }
        //search by bussines partner
        public IActionResult SearchByBp(int BPFrom, int BPTo, string Type)
        {
            if (BPFrom != 0 && BPTo != 0 && Type == "Customer")
            {
                var bp = (from op in _context.OpportunityMasterDatas.Where(x => BPFrom <= x.BPID && x.BPID <= BPTo)
                          join std in _context.StageDetails on op.ID equals std.OpportunityMasterDataID
                          join b in _context.BusinessPartners.Where(x => x.Type == Type) on op.BPID equals b.ID
                          join st in _context.SetUpStages on std.StagesID equals st.ID
                          group new { std, st } by new { std.ID } into datas
                          let data = datas.FirstOrDefault()
                          select new StageView
                          {
                              ID = data.std.ID,
                              StageID = data.st.ID,
                              Name = data.st.Name,
                              StageNo = data.st.StageNo,
                              ExcetedAmount = data.std.PotentailAmount,
                              WeightAmount = data.std.WeightAmount,
                              Percent = data.std.Percent,
                          }).OrderBy(r => r.StageNo).ToList();
                for (int i = 0; i < bp.Count; i++)
                {
                    for (int j = i + 1; j < bp.Count; j++)
                    {
                        if (bp[i].Name == bp[j].Name)
                        {
                            bp[i].ExcetedAmount += bp[j].ExcetedAmount;
                        }
                    }
                }
                for (int i = 0; i < bp.Count; i++)
                {
                    for (int j = i + 1; j < bp.Count; j++)
                    {
                        if (bp[i].Name == bp[j].Name)
                        {
                            bp[i].WeightAmount += bp[j].WeightAmount;
                        }
                    }
                }

                var result = (from list in bp
                              group new { list } by new { list.Name } into datas
                              let data = datas.FirstOrDefault()
                              select new StageView
                              {
                                  ID = data.list.ID,
                                  Name = data.list.Name,
                                  StageNo = data.list.StageNo,
                                  ExcetedAmount = data.list.ExcetedAmount,
                                  WeightAmount = data.list.WeightAmount,
                                  Percent = data.list.Percent,
                              }).OrderBy(r => r.StageNo).ToList();
                return Ok(result);
            }
            else if (BPFrom != 0 && BPTo != 0 && Type == "Vendor")
            {
                var bp = (from op in _context.OpportunityMasterDatas.Where(x => BPFrom <= x.BPID && x.BPID <= BPTo)
                          join b in _context.BusinessPartners.Where(x => x.Type == Type) on op.BPID equals b.ID
                          join std in _context.StageDetails on op.ID equals std.OpportunityMasterDataID
                          join st in _context.SetUpStages on std.StagesID equals st.ID
                          group new { std, st } by new { std.ID } into datas
                          let data = datas.FirstOrDefault()
                          select new StageView
                          {
                              ID = data.std.ID,
                              StageID = data.st.ID,
                              Name = data.st.Name,
                              StageNo = data.st.StageNo,
                              ExcetedAmount = data.std.PotentailAmount,
                              WeightAmount = data.std.WeightAmount,
                              Percent = data.std.Percent,
                          }).OrderBy(r => r.StageNo).ToList();
                for (int i = 0; i < bp.Count; i++)
                {
                    for (int j = i + 1; j < bp.Count; j++)
                    {
                        if (bp[i].Name == bp[j].Name)
                        {
                            bp[i].ExcetedAmount += bp[j].ExcetedAmount;
                        }
                    }
                }
                for (int i = 0; i < bp.Count; i++)
                {
                    for (int j = i + 1; j < bp.Count; j++)
                    {
                        if (bp[i].Name == bp[j].Name)
                        {
                            bp[i].WeightAmount += bp[j].WeightAmount;
                        }
                    }
                }

                var result = (from list in bp
                              group new { list } by new { list.Name } into datas
                              let data = datas.FirstOrDefault()
                              select new StageView
                              {
                                  ID = data.list.ID,
                                  Name = data.list.Name,
                                  StageNo = data.list.StageNo,
                                  ExcetedAmount = data.list.ExcetedAmount,
                                  WeightAmount = data.list.WeightAmount,
                                  Percent = data.list.Percent,
                              }).OrderBy(r => r.StageNo).ToList();
                return Ok(result);
            }
            else if (BPFrom != 0 && BPTo != 0 && Type == "Lead")
            {
                var bp = (from op in _context.OpportunityMasterDatas.Where(x => BPFrom <= x.BPID && x.BPID <= BPTo)
                          join b in _context.BusinessPartners.Where(x => x.Type == Type) on op.BPID equals b.ID
                          join std in _context.StageDetails on op.ID equals std.OpportunityMasterDataID
                          join st in _context.SetUpStages on std.StagesID equals st.ID
                          group new { std, st } by new { std.ID } into datas
                          let data = datas.FirstOrDefault()
                          select new StageView
                          {
                              ID = data.std.ID,
                              StageID = data.st.ID,
                              Name = data.st.Name,
                              StageNo = data.st.StageNo,
                              ExcetedAmount = data.std.PotentailAmount,
                              WeightAmount = data.std.WeightAmount,
                              Percent = data.std.Percent,
                          }).OrderBy(r => r.StageNo).ToList();
                for (int i = 0; i < bp.Count; i++)
                {
                    for (int j = i + 1; j < bp.Count; j++)
                    {
                        if (bp[i].Name == bp[j].Name)
                        {
                            bp[i].ExcetedAmount += bp[j].ExcetedAmount;
                        }
                    }
                }
                for (int i = 0; i < bp.Count; i++)
                {
                    for (int j = i + 1; j < bp.Count; j++)
                    {
                        if (bp[i].Name == bp[j].Name)
                        {
                            bp[i].WeightAmount += bp[j].WeightAmount;
                        }
                    }
                }

                var result = (from list in bp
                              group new { list } by new { list.Name } into datas
                              let data = datas.FirstOrDefault()
                              select new StageView
                              {
                                  ID = data.list.ID,
                                  Name = data.list.Name,
                                  StageNo = data.list.StageNo,
                                  ExcetedAmount = data.list.ExcetedAmount,
                                  WeightAmount = data.list.WeightAmount,
                                  Percent = data.list.Percent,
                              }).OrderBy(r => r.StageNo).ToList();
                return Ok(result);
            }
            else if (BPFrom != 0 && BPTo != 0 && Type == "Customer And Lead")
            {
                var bp = (from op in _context.OpportunityMasterDatas.Where(x => BPFrom <= x.BPID && x.BPID <= BPTo)
                          join b in _context.BusinessPartners.Where(x => x.Type == Type) on op.BPID equals b.ID
                          join std in _context.StageDetails on op.ID equals std.OpportunityMasterDataID
                          join st in _context.SetUpStages on std.StagesID equals st.ID
                          group new { std, st } by new { std.ID } into datas
                          let data = datas.FirstOrDefault()
                          select new StageView
                          {
                              ID = data.std.ID,
                              StageID = data.st.ID,
                              Name = data.st.Name,
                              StageNo = data.st.StageNo,
                              ExcetedAmount = data.std.PotentailAmount,
                              WeightAmount = data.std.WeightAmount,
                              Percent = data.std.Percent,
                          }).OrderBy(r => r.StageNo).ToList();
                for (int i = 0; i < bp.Count; i++)
                {
                    for (int j = i + 1; j < bp.Count; j++)
                    {
                        if (bp[i].Name == bp[j].Name)
                        {
                            bp[i].ExcetedAmount += bp[j].ExcetedAmount;
                        }
                    }
                }
                for (int i = 0; i < bp.Count; i++)
                {
                    for (int j = i + 1; j < bp.Count; j++)
                    {
                        if (bp[i].Name == bp[j].Name)
                        {
                            bp[i].WeightAmount += bp[j].WeightAmount;
                        }
                    }
                }

                var result = (from list in bp
                              group new { list } by new { list.Name } into datas
                              let data = datas.FirstOrDefault()
                              select new StageView
                              {
                                  ID = data.list.ID,
                                  Name = data.list.Name,
                                  StageNo = data.list.StageNo,
                                  ExcetedAmount = data.list.ExcetedAmount,
                                  WeightAmount = data.list.WeightAmount,
                                  Percent = data.list.Percent,
                              }).OrderBy(r => r.StageNo).ToList();
                return Ok(result);
            }
            else if (BPFrom != 0 && BPTo != 0 && Type == "All")
            {
                var bp = (from op in _context.OpportunityMasterDatas.Where(x => BPFrom <= x.BPID && x.BPID <= BPTo)
                          join b in _context.BusinessPartners.Where(x => x.Type == Type) on op.BPID equals b.ID
                          join std in _context.StageDetails on op.ID equals std.OpportunityMasterDataID
                          join st in _context.SetUpStages on std.StagesID equals st.ID
                          group new { std, st } by new { std.ID } into datas
                          let data = datas.FirstOrDefault()
                          select new StageView
                          {
                              ID = data.std.ID,
                              StageID = data.st.ID,
                              Name = data.st.Name,
                              StageNo = data.st.StageNo,
                              ExcetedAmount = data.std.PotentailAmount,
                              WeightAmount = data.std.WeightAmount,
                              Percent = data.std.Percent,
                          }).OrderBy(r => r.StageNo).ToList();
                for (int i = 0; i < bp.Count; i++)
                {
                    for (int j = i + 1; j < bp.Count; j++)
                    {
                        if (bp[i].Name == bp[j].Name)
                        {
                            bp[i].ExcetedAmount += bp[j].ExcetedAmount;
                        }
                    }
                }
                for (int i = 0; i < bp.Count; i++)
                {
                    for (int j = i + 1; j < bp.Count; j++)
                    {
                        if (bp[i].Name == bp[j].Name)
                        {
                            bp[i].WeightAmount += bp[j].WeightAmount;
                        }
                    }
                }

                var result = (from list in bp
                              group new { list } by new { list.Name } into datas
                              let data = datas.FirstOrDefault()
                              select new StageView
                              {
                                  ID = data.list.ID,
                                  Name = data.list.Name,
                                  StageNo = data.list.StageNo,
                                  ExcetedAmount = data.list.ExcetedAmount,
                                  WeightAmount = data.list.WeightAmount,
                                  Percent = data.list.Percent,
                              }).OrderBy(r => r.StageNo).ToList();
                return Ok(result);
            }

            return Ok();
        }
        //search by stage
        public IActionResult GetsearchByStage(string[] function_param)
        {
            List<StageView> stageViews = new();
            List<StageView> stageViews2 = new();
            foreach (var lis in function_param)
            {
                var list = _context.SetUpStages.Where(s => s.ID == Convert.ToInt32(lis)).ToList();
                stageViews = GetingStageView(list);
                stageViews2.AddRange(stageViews);
            }
            List<StageView> dataStages = new(
                 stageViews2.Count
              );
            dataStages.AddRange(stageViews2);
            return Ok(dataStages);

        }
        //Search By document
        public IActionResult SearchbyDoc(string searchStageParams)
        {
            var stageParams = JsonConvert.DeserializeObject<StageSearchParams>(searchStageParams, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });
            List<StageView> stageViewsPO = new();
            List<StageView> stageViewsGRPO = new();
            List<StageView> stageViewsAR = new();
            List<StageView> stageViewsSQ = new();
            List<StageView> stageViewsSO = new();
            List<StageView> stageViewsDN = new();
            if (stageParams.PurchaseOrder == true)
            {
                var dataType = _context.DocumentTypes.Where(i => i.Code == "PO").ToList();
                stageViewsPO = GetStageView(dataType);
            }
            if (stageParams.GoodsReceiptPO == true)
            {
                var dataType = _context.DocumentTypes.Where(i => i.Code == "PD").ToList();
                stageViewsGRPO = GetStageView(dataType);
            }
            if (stageParams.SaleAR == true)
            {
                var dataType = _context.DocumentTypes.Where(i => i.Code == "IN").ToList();
                stageViewsAR = GetStageView(dataType);
            }
            if (stageParams.SaleDelivery == true)
            {
                var dataType = _context.DocumentTypes.Where(i => i.Code == "DN").ToList();
                stageViewsDN = GetStageView(dataType);
            }
            if (stageParams.SaleQuotation == true)
            {
                var dataType = _context.DocumentTypes.Where(i => i.Code == "SQ").ToList();
                stageViewsSQ = GetStageView(dataType);
            }
            if (stageParams.SaleOrder == true)
            {
                var dataType = _context.DocumentTypes.Where(i => i.Code == "SO").ToList();
                stageViewsSO = GetStageView(dataType);
            }
            List<StageView> dataStages = new(
                stageViewsPO.Count + stageViewsGRPO.Count +
                stageViewsAR.Count + stageViewsDN.Count + stageViewsPO.Count + stageViewsSQ.Count
                );
            dataStages.AddRange(stageViewsPO);
            dataStages.AddRange(stageViewsGRPO);
            dataStages.AddRange(stageViewsAR);
            dataStages.AddRange(stageViewsDN);
            dataStages.AddRange(stageViewsPO);
            dataStages.AddRange(stageViewsSQ);
            return Ok(dataStages);
        }
        private List<StageView> GetStageView(List<DocumentType> _data)
        {
            var stage = (from doc in _data
                         join std in _context.StageDetails on doc.ID equals std.DocTypeID
                         join st in _context.SetUpStages on std.StagesID equals st.ID
                         group new { std, st } by new { std.ID } into datas
                         let data = datas.FirstOrDefault()
                         select new StageView
                         {
                             ID = data.std.ID,
                             StageID = data.st.ID,
                             Name = data.st.Name,
                             StageNo = data.st.StageNo,
                             ExcetedAmount = data.std.PotentailAmount,
                             WeightAmount = data.std.WeightAmount,
                             Percent = data.std.Percent,
                         }).OrderBy(r => r.StageNo).ToList();
            for (int i = 0; i < stage.Count; i++)
            {
                for (int j = i + 1; j < stage.Count; j++)
                {
                    if (stage[i].Name == stage[j].Name)
                    {
                        stage[i].ExcetedAmount += stage[j].ExcetedAmount;
                    }
                }
            }
            for (int i = 0; i < stage.Count; i++)
            {
                for (int j = i + 1; j < stage.Count; j++)
                {
                    if (stage[i].Name == stage[j].Name)
                    {
                        stage[i].WeightAmount += stage[j].WeightAmount;
                    }
                }
            }
            var result = (from list in stage
                          group new { list } by new { list.Name } into datas
                          let data = datas.FirstOrDefault()
                          select new StageView
                          {
                              ID = data.list.ID,
                              Name = data.list.Name,
                              StageNo = data.list.StageNo,
                              ExcetedAmount = data.list.ExcetedAmount,
                              WeightAmount = data.list.WeightAmount,
                              Percent = data.list.Percent,
                          }).OrderBy(r => r.StageNo).ToList();
            return result;

        }
        private List<StageView> GetStageList(List<Employee> _dataemp)
        {
            var bps = (from sl in _dataemp
                       join bp in _context.BusinessPartners on sl.ID equals bp.SaleEMID
                       join op in _context.OpportunityMasterDatas on bp.ID equals op.BPID
                       join std in _context.StageDetails on op.ID equals std.OpportunityMasterDataID
                       join st in _context.SetUpStages on std.StagesID equals st.ID
                       group new { std, st } by new { std.ID } into datas
                       let data = datas.FirstOrDefault()
                       select new StageView
                       {
                           ID = data.std.ID,
                           StageID = data.st.ID,
                           Name = data.st.Name,
                           StageNo = data.st.StageNo,
                           ExcetedAmount = data.std.PotentailAmount,
                           WeightAmount = data.std.WeightAmount,
                           Percent = data.std.Percent,
                       }).OrderBy(r => r.StageNo).ToList();
            for (int i = 0; i < bps.Count; i++)
            {
                for (int j = i + 1; j < bps.Count; j++)
                {
                    if (bps[i].Name == bps[j].Name)
                    {
                        bps[i].ExcetedAmount += bps[j].ExcetedAmount;
                    }
                }
            }
            for (int i = 0; i < bps.Count; i++)
            {
                for (int j = i + 1; j < bps.Count; j++)
                {
                    if (bps[i].Name == bps[j].Name)
                    {
                        bps[i].WeightAmount += bps[j].WeightAmount;
                    }
                }
            }
            var result = (from list in bps
                          group new { list } by new { list.Name } into datas
                          let data = datas.FirstOrDefault()
                          select new StageView
                          {
                              ID = data.list.ID,
                              Name = data.list.Name,
                              StageNo = data.list.StageNo,
                              ExcetedAmount = data.list.ExcetedAmount,
                              WeightAmount = data.list.WeightAmount,
                              Percent = data.list.Percent,
                          }).OrderBy(r => r.StageNo).ToList();
            return result;
        }
        private List<StageView> GetingStageView(List<SetUpStage> _datasetstage)
        {
            var stage = (from st in _datasetstage
                         join std in _context.StageDetails on st.ID equals std.StagesID
                         group new { std, st } by new { std.ID } into datas
                         let data = datas.FirstOrDefault()
                         select new StageView
                         {
                             ID = data.std.ID,
                             StageID = data.st.ID,
                             Name = data.st.Name,
                             StageNo = data.st.StageNo,
                             ExcetedAmount = data.std.PotentailAmount,
                             WeightAmount = data.std.WeightAmount,
                             Percent = data.std.Percent,
                         }).OrderBy(r => r.StageNo).ToList();
            for (int i = 0; i < stage.Count; i++)
            {
                for (int j = i + 1; j < stage.Count; j++)
                {
                    if (stage[i].Name == stage[j].Name)
                    {
                        stage[i].ExcetedAmount += stage[j].ExcetedAmount;
                    }
                }
            }
            for (int i = 0; i < stage.Count; i++)
            {
                for (int j = i + 1; j < stage.Count; j++)
                {
                    if (stage[i].Name == stage[j].Name)
                    {
                        stage[i].WeightAmount += stage[j].WeightAmount;
                    }
                }
            }
            var result = (from list in stage
                          group new { list } by new { list.Name } into datas
                          let data = datas.FirstOrDefault()
                          select new StageView
                          {
                              ID = data.list.ID,
                              Name = data.list.Name,
                              StageNo = data.list.StageNo,
                              ExcetedAmount = data.list.ExcetedAmount,
                              WeightAmount = data.list.WeightAmount,
                              Percent = data.list.Percent,
                          }).OrderBy(r => r.StageNo).ToList();
            return result;
            // return stage;
        }
        //public IActionResult Detail(int bpid)
        //{
        //    var data = (from ac in _context.Activites.Where(x => x.BPID == bpid)
        //                join g in _context.Generals on ac.ID equals g.ActivityID
        //                join bp in _context.BusinessPartners on ac.BPID equals bp.ID
        //                join con in _context.ContactPersons on bp.ID equals con.BusinessPartnerID
        //                let user = _context.Employees.Where(x => x.ID == ac.UserID).FirstOrDefault() ?? new Employee()
        //                let actype = _context.SetupTypes.FirstOrDefault(x => x.ID == ac.TypeID) ?? new SetupType()
        //                select new Activity
        //                {
        //                    ID = ac.ID,
        //                    GID = g.ID,
        //                    BPID = bp.ID,
        //                    Number = ac.Number,
        //                    //SetupActName = ac.SetupActName,
        //                    UserID = user.ID,
        //                    EmpName = user.Name,
        //                    TypeID = ac.TypeID,
        //                    TypeName = actype.Name,
        //                    //TelNo = .TelNo,
        //                    BpCode = bp.Code,
        //                    BpName = bp.Name,
        //                    BpType = bp.Type,
        //                    ContactID = con.ContactID,
        //                    TelNo = con.Tel1,
        //                    Contact = (from con in _context.ContactPersons.Where(x => x.BusinessPartnerID == bp.ID)
        //                               select new ContactPerson
        //                               {
        //                                   ID = con.ID,
        //                                   ContactID = con.ContactID,
        //                                   Tel1 = con.Tel1,
        //                                   SetAsDefualt = con.SetAsDefualt
        //                               }).ToList() ?? new List<ContactPerson>(),
        //                    SubName = ac.SubName,
        //                    Employee = ac.Employee,
        //                    Personal = ac.Personal,
        //                    //===general==
        //                    Remark = g.Remark,
        //                    StartTime = g.StartTime,
        //                    EndTime = g.EndTime,
        //                    Durration = g.Durration,
        //                    Status = g.Status,
        //                    Location = g.Location,
        //                    Priority = g.Priority,
        //                    Recurrences = g.Recurrence,
        //                    After = g.After,
        //                    By = g.By,
        //                    NoEndDate = g.NoEndDate,
        //                    NumAfter = g.NumAfter,
        //                    RepeatDate = g.RepeatDate,
        //                    Start = g.Start,
        //                    RepeatEveryRecurr = g.RepeatEveryRecurr,
        //                    RepeatEveryWeek = g.RepeatEveryWeek,
        //                    ByDate = g.ByDate,
        //                    Mon = g.Mon,
        //                    Tue = g.Tue,
        //                    Wed = g.Wed,
        //                    Thu = g.Thu,
        //                    Fri = g.Fri,
        //                    Sat = g.Sat,
        //                    Sun = g.Sun,
        //                    //==monthly====
        //                    Days = g.Days,
        //                    numDay = g.numDay,
        //                    repeatOn = g.repeatOn,
        //                    numOfRepeat = g.numOfRepeat,
        //                    DaysInMonthly = g.DaysInMonthly,
        //                    //======yearly=====
        //                    RepeatOncheckYearly = g.RepeatOncheckYearly,
        //                    MonthsInAnnualy = g.MonthsInAnnualy,
        //                    NumOfMonths = g.NumOfMonths,
        //                    checkNumAnnualy = g.checkNumAnnualy,
        //                    NumofAnnualy = g.NumofAnnualy,
        //                    DaysOfAnnualy = g.DaysOfAnnualy,
        //                    MonthsOfAnnulay = g.MonthsInAnnualy,
        //                    RepeatofNumAnnualy = g.RepeatofNumAnnualy,
        //                    RepeatNumOfmonths = g.RepeatNumOfmonths
        //                }
        //          ).FirstOrDefault();

        //    ViewBag.Activity = new SelectList(_context.SetupActivities, "Activities", "Activities");
        //    ViewBag.Types = new SelectList(_context.SetupTypes, "ID", "Name");
        //    ViewBag.Subject = new SelectList(_context.SetupSubjects, "ID", "Name");
        //    ViewBag.Status = new SelectList(_context.SetupStatuses, "ID", "Status");
        //    ViewBag.Location = new SelectList(_context.SetupLocations, "ID", "Name");
        //    ViewBag.Emp = new SelectList(_context.Employees, "ID", "Name");
        //    //ViewBag.Cont = new SelectList(_context.ContactPersons.Where(x => x.BusinessPartnerID == bpid), "ID", "ContactID");
        //    return View(data);
        //}
        [HttpPost]
        public IActionResult UpdateDataActivityOfOpp(string _data)
        {
            Activity data = JsonConvert.DeserializeObject<Activity>(_data, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });

            //ValidateSummary(data, data.SummaryDetails, data.StageDetail, data.PartnerDetail, data.CompetitorDetail, data.PotentialDetails, data.PotentialDetails.DescriptionPotentialDetail, data.SummaryDetails.DescriptionSummaryDetails);
            //ModelMessage msg = new ModelMessage();
            if (ModelState.IsValid)
            {
                _context.Activites.Update(data);
                _context.SaveChanges();
                //ModelState.AddModelError("success", "Activity updated succussfully!");
                //msg.Approve();
            }
            return Ok(new { url = "/Opportunity/Opportunity" });
        }
        //==================Won Opportunity Reports===============
        public IActionResult WonOpportunityReports()
        {
            ViewBag.WonOpportunityReports = "highlight";
            ViewData["A"] = new SelectList(_context.GroupCustomer1s.Where(x => x.Type == "Customer"), "ID", "Name");
            ViewData["B"] = new SelectList(_context.GroupCustomer1s.Where(x => x.Type == "Vendor"), "ID", "Name");
            return View();
        }
        public IActionResult GetDataWonReports()
        {
            var data = (from opp in _context.OpportunityMasterDatas
                        join po in _context.PotentialDetails on opp.ID equals po.OpportunityMasterDataID
                        join su in _context.SummaryDetail.Where(x => x.IsWon == true) on opp.ID equals su.OpportunityMasterDataID
                        select new OpportunityMasterData
                        {
                            ID = opp.ID,
                            StartDate = opp.StartDate,
                            ClosingDate = opp.ClosingDate,
                            StartOfDay = opp.StartDate.ToString("dd"),
                            CloseOfDay = opp.ClosingDate.ToString("dd"),
                            Day = (opp.ClosingDate - opp.StartDate).Days,//(opp.ClosingDate-opp.StartDate).Days,
                            OpportunityNo = opp.OpportunityNo,
                            PotentailAmount = po.PotentailAmount,

                        }
                      ).ToList();
            return Ok(data);
        }
        [HttpGet]
        public IActionResult DataWonReports()
        {
            var data = (from opp in _context.OpportunityMasterDatas
                        join po in _context.PotentialDetails on opp.ID equals po.OpportunityMasterDataID
                        select new OpportunityMasterData
                        {
                            ID = opp.ID,
                            StartDate = opp.StartDate,
                            ClosingDate = opp.ClosingDate,
                            StartOfDay = opp.StartDate.ToString("dd"),
                            CloseOfDay = opp.ClosingDate.ToString("dd"),
                            Day = (opp.ClosingDate - opp.StartDate).Days,
                            OpportunityNo = opp.OpportunityNo,
                            PotentailAmount = po.PotentailAmount,

                        }
                       ).ToList();
            return Ok(data);
        }
        //========get data sale employee====
        public IActionResult GetSaleEmpFilter()
        {
            //var data = _context.Employees.ToList();
            var data = (from emp in _context.Employees
                        select new Employee
                        {
                            ID = emp.ID,
                            Name = emp.Name,
                            Action = ""
                        }
                      ).ToList();
            return Ok(data);
        }
        public IActionResult GetBPFilter()
        {
            var data = _context.BusinessPartners.ToList();
            return Ok(data);
        }
        public IActionResult SearchByBpFilter(string strObject)
        {
            SearchWonOppParams datas = JsonConvert.DeserializeObject<SearchWonOppParams>(strObject, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });
            if (datas.BPFrom != 0 && datas.BPTo != 0 && string.IsNullOrEmpty(datas.Type) && datas.Cusgroup == 0 && datas.Vengroup == 0)
            {
                var data = (from opp in _context.OpportunityMasterDatas.Where(x => datas.BPFrom <= x.BPID && x.BPID <= datas.BPTo)
                            join po in _context.PotentialDetails on opp.ID equals po.OpportunityMasterDataID
                            select new OpportunityMasterData
                            {
                                ID = opp.ID,
                                StartDate = opp.StartDate,
                                ClosingDate = opp.ClosingDate,
                                StartOfDay = opp.StartDate.ToString("dd"),
                                CloseOfDay = opp.ClosingDate.ToString("dd"),
                                Day = (opp.ClosingDate - opp.StartDate).Days,
                                OpportunityNo = opp.OpportunityNo,
                                PotentailAmount = po.PotentailAmount,

                            }
                       ).ToList();
                return Ok(data);
            }
            else if (datas.BPFrom != 0 && datas.BPTo != 0 && datas.Type == "Customer" && datas.Cusgroup == 0 && datas.Vengroup == 0)
            {
                var data = (from opp in _context.OpportunityMasterDatas.Where(x => datas.BPFrom <= x.BPID && x.BPID <= datas.BPTo)
                            join bp in _context.BusinessPartners.Where(x => x.Type == "Customer") on opp.BPID equals bp.ID
                            join po in _context.PotentialDetails on opp.ID equals po.OpportunityMasterDataID
                            select new OpportunityMasterData
                            {
                                ID = opp.ID,
                                StartDate = opp.StartDate,
                                ClosingDate = opp.ClosingDate,
                                StartOfDay = opp.StartDate.ToString("dd"),
                                CloseOfDay = opp.ClosingDate.ToString("dd"),
                                Day = (opp.ClosingDate - opp.StartDate).Days,
                                OpportunityNo = opp.OpportunityNo,
                                PotentailAmount = po.PotentailAmount,

                            }
                       ).ToList();
                return Ok(data);
            }
            else if (datas.BPFrom != 0 && datas.BPTo != 0 && datas.Cusgroup == 0 && datas.Vengroup == 0 && datas.Type == "Vendor")
            {
                var data = (from opp in _context.OpportunityMasterDatas.Where(x => datas.BPFrom <= x.BPID && x.BPID <= datas.BPTo)
                            join bp in _context.BusinessPartners.Where(x => x.Type == "Vendor") on opp.BPID equals bp.ID
                            join po in _context.PotentialDetails on opp.ID equals po.OpportunityMasterDataID
                            select new OpportunityMasterData
                            {
                                ID = opp.ID,
                                StartDate = opp.StartDate,
                                ClosingDate = opp.ClosingDate,
                                StartOfDay = opp.StartDate.ToString("dd"),
                                CloseOfDay = opp.ClosingDate.ToString("dd"),
                                Day = (opp.ClosingDate - opp.StartDate).Days,
                                OpportunityNo = opp.OpportunityNo,
                                PotentailAmount = po.PotentailAmount,

                            }
                       ).ToList();
                return Ok(data);
            }
            else if (datas.BPFrom != 0 && datas.BPTo != 0 && datas.Type == "All" && datas.Cusgroup == 0 && datas.Vengroup == 0)
            {
                var data = (from opp in _context.OpportunityMasterDatas.Where(x => datas.BPFrom <= x.BPID && x.BPID <= datas.BPTo)
                            join po in _context.PotentialDetails on opp.ID equals po.OpportunityMasterDataID
                            select new OpportunityMasterData
                            {
                                ID = opp.ID,
                                StartDate = opp.StartDate,
                                ClosingDate = opp.ClosingDate,
                                StartOfDay = opp.StartDate.ToString("dd"),
                                CloseOfDay = opp.ClosingDate.ToString("dd"),
                                Day = (opp.ClosingDate - opp.StartDate).Days,
                                OpportunityNo = opp.OpportunityNo,
                                PotentailAmount = po.PotentailAmount,

                            }
                       ).ToList();
                return Ok(data);
            }
            else if (datas.BPFrom != 0 && datas.BPTo != 0 && string.IsNullOrEmpty(datas.Type) && datas.Cusgroup != 0 && datas.Vengroup == 0)
            {
                var data = (from opp in _context.OpportunityMasterDatas.Where(x => datas.BPFrom <= x.BPID && x.BPID <= datas.BPTo)
                            join bp in _context.BusinessPartners.Where(x => x.Group1ID == datas.Cusgroup) on opp.BPID equals bp.ID
                            join po in _context.PotentialDetails on opp.ID equals po.OpportunityMasterDataID
                            select new OpportunityMasterData
                            {
                                ID = opp.ID,
                                StartDate = opp.StartDate,
                                ClosingDate = opp.ClosingDate,
                                StartOfDay = opp.StartDate.ToString("dd"),
                                CloseOfDay = opp.ClosingDate.ToString("dd"),
                                Day = (opp.ClosingDate - opp.StartDate).Days,
                                OpportunityNo = opp.OpportunityNo,
                                PotentailAmount = po.PotentailAmount,

                            }
                       ).ToList();
                return Ok(data);
            }
            else if (datas.BPFrom != 0 && datas.BPTo != 0 && string.IsNullOrEmpty(datas.Type) && datas.Cusgroup == 0 && datas.Vengroup != 0)
            {
                var data = (from opp in _context.OpportunityMasterDatas.Where(x => datas.BPFrom <= x.BPID && x.BPID <= datas.BPTo)
                            join bp in _context.BusinessPartners.Where(x => x.Group2ID == datas.Vengroup) on opp.BPID equals bp.ID
                            join po in _context.PotentialDetails on opp.ID equals po.OpportunityMasterDataID
                            select new OpportunityMasterData
                            {
                                ID = opp.ID,
                                StartDate = opp.StartDate,
                                ClosingDate = opp.ClosingDate,
                                StartOfDay = opp.StartDate.ToString("dd"),
                                CloseOfDay = opp.ClosingDate.ToString("dd"),
                                Day = (opp.ClosingDate - opp.StartDate).Days,
                                OpportunityNo = opp.OpportunityNo,
                                PotentailAmount = po.PotentailAmount,

                            }
                       ).ToList();
                return Ok(data);
            }
            else if (datas.BPFrom != 0 && datas.BPTo != 0 && string.IsNullOrEmpty(datas.Type) && datas.Cusgroup != 0 && datas.Vengroup != 0)
            {
                var data = (from opp in _context.OpportunityMasterDatas.Where(x => datas.BPFrom <= x.BPID && x.BPID <= datas.BPTo)
                            join bp in _context.BusinessPartners.Where(x => x.Group2ID == datas.Vengroup && x.Group1ID == datas.Cusgroup) on opp.BPID equals bp.ID
                            join po in _context.PotentialDetails on opp.ID equals po.OpportunityMasterDataID
                            select new OpportunityMasterData
                            {
                                ID = opp.ID,
                                StartDate = opp.StartDate,
                                ClosingDate = opp.ClosingDate,
                                StartOfDay = opp.StartDate.ToString("dd"),
                                CloseOfDay = opp.ClosingDate.ToString("dd"),
                                Day = (opp.ClosingDate - opp.StartDate).Days,
                                OpportunityNo = opp.OpportunityNo,
                                PotentailAmount = po.PotentailAmount,

                            }
                       ).ToList();
                return Ok(data);

            }
            else if (datas.BPFrom != 0 && datas.BPTo != 0 && string.IsNullOrEmpty(datas.Type) && datas.Cusgroup == 0 && datas.Vengroup == 0)
            {
                var data = (from opp in _context.OpportunityMasterDatas.Where(x => datas.BPFrom <= x.BPID && x.BPID <= datas.BPTo)
                            join bp in _context.BusinessPartners.Where(x => x.Group2ID == datas.Vengroup && x.Group1ID == datas.Cusgroup) on opp.BPID equals bp.ID
                            join po in _context.PotentialDetails on opp.ID equals po.OpportunityMasterDataID
                            select new OpportunityMasterData
                            {
                                ID = opp.ID,
                                StartDate = opp.StartDate,
                                ClosingDate = opp.ClosingDate,
                                StartOfDay = opp.StartDate.ToString("dd"),
                                CloseOfDay = opp.ClosingDate.ToString("dd"),
                                Day = (opp.ClosingDate - opp.StartDate).Days,
                                OpportunityNo = opp.OpportunityNo,
                                PotentailAmount = po.PotentailAmount,

                            }
                       ).ToList();
                return Ok(data);

            }
            else if (!string.IsNullOrEmpty(datas.StartFrom) && !string.IsNullOrEmpty(datas.StartTo) && string.IsNullOrEmpty(datas.CloseTo))
            {
                var data = (from opp in _context.OpportunityMasterDatas.Where(x => x.StartDate >= Convert.ToDateTime(datas.StartFrom) && x.StartDate <= Convert.ToDateTime(datas.StartTo))
                            join bp in _context.BusinessPartners on opp.BPID equals bp.ID
                            join po in _context.PotentialDetails on opp.ID equals po.OpportunityMasterDataID
                            select new OpportunityMasterData
                            {
                                ID = opp.ID,
                                StartDate = opp.StartDate,
                                ClosingDate = opp.ClosingDate,
                                StartOfDay = opp.StartDate.ToString("dd"),
                                CloseOfDay = opp.ClosingDate.ToString("dd"),
                                Day = (opp.ClosingDate - opp.StartDate).Days,
                                OpportunityNo = opp.OpportunityNo,
                                PotentailAmount = po.PotentailAmount,

                            }
                      ).ToList();
                return Ok(data);
            }
            else if (string.IsNullOrEmpty(datas.StartFrom) && string.IsNullOrEmpty(datas.StartTo) && !string.IsNullOrEmpty(datas.CloseFrom) && !string.IsNullOrEmpty(datas.CloseTo))
            {
                var data = (from opp in _context.OpportunityMasterDatas.Where(x => x.ClosingDate >= Convert.ToDateTime(datas.CloseFrom) && x.ClosingDate <= Convert.ToDateTime(datas.CloseTo))
                            join bp in _context.BusinessPartners on opp.BPID equals bp.ID
                            join po in _context.PotentialDetails on opp.ID equals po.OpportunityMasterDataID
                            select new OpportunityMasterData
                            {
                                ID = opp.ID,
                                StartDate = opp.StartDate,
                                ClosingDate = opp.ClosingDate,
                                StartOfDay = opp.StartDate.ToString("dd"),
                                CloseOfDay = opp.ClosingDate.ToString("dd"),
                                Day = (opp.ClosingDate - opp.StartDate).Days,
                                OpportunityNo = opp.OpportunityNo,
                                PotentailAmount = po.PotentailAmount,

                            }
                      ).ToList();
                return Ok(data);
            }
            else if (!string.IsNullOrEmpty(datas.StartFrom) && !string.IsNullOrEmpty(datas.StartTo) && !string.IsNullOrEmpty(datas.CloseFrom) && !string.IsNullOrEmpty(datas.CloseTo))
            {
                var data = (from opp in _context.OpportunityMasterDatas.Where(x => x.StartDate >= Convert.ToDateTime(datas.StartFrom) && x.StartDate <= Convert.ToDateTime(datas.StartTo) && x.ClosingDate >= Convert.ToDateTime(datas.CloseFrom) && x.ClosingDate <= Convert.ToDateTime(datas.CloseTo))
                            join bp in _context.BusinessPartners.Where(x => x.Group2ID == datas.Vengroup && x.Group1ID == datas.Cusgroup) on opp.BPID equals bp.ID
                            join po in _context.PotentialDetails on opp.ID equals po.OpportunityMasterDataID
                            select new OpportunityMasterData
                            {
                                ID = opp.ID,
                                StartDate = opp.StartDate,
                                ClosingDate = opp.ClosingDate,
                                StartOfDay = opp.StartDate.ToString("dd"),
                                CloseOfDay = opp.ClosingDate.ToString("dd"),
                                Day = (opp.ClosingDate - opp.StartDate).Days,
                                OpportunityNo = opp.OpportunityNo,
                                PotentailAmount = po.PotentailAmount,

                            }
                      ).ToList();
                return Ok(data);
            }

            if (datas.SaleEmp.Count > 0)
            {
                List<Employee> employees = new();

                foreach (var items in datas.SaleEmp)
                {
                    var _emp = _context.Employees.Where(i => i.ID == items.ID).ToList();
                    if (_emp.Count > 0) employees.AddRange(_emp);
                }
                var data = (from emp in employees
                            let opp = _context.OpportunityMasterDatas.Where(x => x.SaleEmpID == emp.ID).FirstOrDefault() ?? new OpportunityMasterData()
                            join su in _context.SummaryDetail.Where(x => x.IsWon == true) on opp.ID equals su.OpportunityMasterDataID
                            join po in _context.PotentialDetails on opp.ID equals po.OpportunityMasterDataID
                            select new OpportunityMasterData
                            {
                                ID = opp.ID,
                                StartDate = opp.StartDate,
                                ClosingDate = opp.ClosingDate,
                                StartOfDay = opp.StartDate.ToString("dd"),
                                CloseOfDay = opp.ClosingDate.ToString("dd"),
                                Day = (opp.ClosingDate - opp.StartDate).Days,
                                OpportunityNo = opp.OpportunityNo,
                                PotentailAmount = po.PotentailAmount,

                            }
                       ).ToList();
                return Ok(data);
            }
            return Ok();

        }
        //==============geting data of activity=============
        public IActionResult GetdataActivity(int id)
        {
            var datas = (from ac in _context.Activites.Where(x => x.BPID == id)
                         join g in _context.Generals on ac.ID equals g.ActivityID
                         join bp in _context.BusinessPartners on ac.BPID equals bp.ID
                         let user = _context.Employees.Where(x => x.ID == ac.UserID).FirstOrDefault() ?? new Employee()
                         let actype = _context.SetupTypes.FirstOrDefault(x => x.ID == ac.TypeID) ?? new SetupType()
                         select new ActivityView
                         {
                             ID = ac.ID,
                             GID = g.ID,
                             BPID = bp.ID,
                             UserID = ac.UserID,
                             Number = ac.Number,
                             Activities = (int)ac.Activities,
                             TypeID = ac.TypeID,
                             BpCode = bp.Code,
                             BpName = bp.Name,
                             BpType = bp.Type,
                             TypeName = actype.Name,
                             SubNameID = ac.SubNameID,
                             EmpNameID = ac.EmpNameID,
                             EmpName = user.Name,
                             StartTime = g.StartTime,
                             EndTime = g.EndTime

                         }).ToList();
            return Ok(datas);
        }
        //============hrfe activity======
        public IActionResult Activity(int number, bool isop)
        {
            var assignedto = new ActivityView
            {
                AssignedTo = AssignedTo.Select(p => new SelectListItem
                {
                    Value = p.Key.ToString(),
                    Text = p.Value
                }).ToList(),

                Recurrence = Recurrence.Select(p => new SelectListItem
                {
                    Value = p.Key.ToString(),
                    Text = p.Value
                }).ToList(),
                Priorities = Priorities.Select(P => new SelectListItem
                {
                    Value = P.Key.ToString(),
                    Text = P.Value
                }).ToList(),
                GeneralSetting = GetGeneralSettingAdmin().Display,
                Number = number,
            };
            if (isop) ViewBag.Activities = "highlight";
            else ViewBag.Acitivity = "highlight";
            //ViewBag.Activity = new SelectList(_context.SetupActivities, "Activities", "Activities");
            ViewBag.Types = new SelectList(_context.SetupTypes, "ID", "Name");
            ViewBag.Subject = new SelectList(_context.SetupSubjects, "ID", "Name");
            ViewBag.Status = new SelectList(_context.SetupStatuses, "Status", "Status");
            ViewBag.Location = new SelectList(_context.SetupLocations, "Name", "Name");
            var userEmp = _context.UserAccounts.Include(i => i.Employee).Where(i => !i.Delete)
                .Select(i => new UserAccount { ID = i.ID, Username = i.Employee.Name }).ToList();
            ViewBag.Emp = new SelectList(userEmp, "ID", "Username");
            return View(assignedto);
        }
        //========Lost Oppoortunity Reports========
        public IActionResult GetDataLostReports()
        {
            var data = (from opp in _context.OpportunityMasterDatas
                        join bp in _context.BusinessPartners on opp.BPID equals bp.ID
                        join po in _context.PotentialDetails on opp.ID equals po.OpportunityMasterDataID
                        join su in _context.SummaryDetail.Where(x => x.IsLost == true) on opp.ID equals su.OpportunityMasterDataID
                        select new OpportunityMasterData
                        {
                            ID = opp.ID,
                            OpportunityNo = opp.OpportunityNo,
                            OpportunityName = opp.OpportunityName,
                            BPName = bp.Name,
                            BPCode = bp.Code,
                            CloseingPerecnt = opp.CloingPercentage,
                            PotentialAmount = po.PotentailAmount,
                            WeightAmount = po.WeightAmount
                        }
                     ).ToList();
            return Ok(data);
        }
        //========Get DataStage to Filter========
        [HttpGet]
        public IActionResult GetListSetupStage()
        {
            var data = (from st in _context.SetUpStages
                        select new StageDetail
                        {
                            ID = st.ID,
                            Name = st.Name,
                            Action = ""
                        }
                      ).ToList();
            return Ok(data);
        }
        public IActionResult LostOpportunityReports()
        {
            ViewBag.LostOpportunityReports = "highlight";
            ViewData["A"] = new SelectList(_context.GroupCustomer1s.Where(x => x.Type == "Customer"), "ID", "Name");
            ViewData["B"] = new SelectList(_context.GroupCustomer1s.Where(x => x.Type == "Vendor"), "ID", "Name");
            return View();
        }

        public IActionResult GetBPLostFitler()
        {
            var data = _context.BusinessPartners.ToList();
            return Ok(data);
        }
        public IActionResult GetComFitler()
        {
            var data = (from com in _context.SetupCompetitors
                        select new CompetitorDetail
                        {
                            ID = com.ID,
                            Name = com.Name,
                            Action = ""
                        }).ToList();
            return Ok(data);
        }
        public IActionResult SearchByBpFilterLostOpp(string strObject)
        {

            SearchByBpFilterLostOppParams datas = JsonConvert.DeserializeObject<SearchByBpFilterLostOppParams>(strObject, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });
            if (datas.BPFrom != 0 && datas.BPTo != 0 && datas.Type == null && datas.Cusgroup == 0 && datas.Vengroup == 0)
            {
                var data = (from opp in _context.OpportunityMasterDatas.Where(x => datas.BPFrom <= x.BPID && x.BPID <= datas.BPTo)
                            join bp in _context.BusinessPartners on opp.BPID equals bp.ID
                            join po in _context.PotentialDetails on opp.ID equals po.OpportunityMasterDataID
                            join su in _context.SummaryDetail.Where(x => x.IsLost == true) on opp.ID equals su.OpportunityMasterDataID
                            select new OpportunityMasterData
                            {
                                ID = opp.ID,
                                OpportunityNo = opp.OpportunityNo,
                                OpportunityName = opp.OpportunityName,
                                BPName = bp.Name,
                                BPCode = bp.Code,
                                CloseingPerecnt = opp.CloingPercentage,
                                PotentialAmount = po.PotentailAmount,
                                WeightAmount = po.WeightAmount
                            }
                      ).ToList();
                return Ok(data);
            }
            else if (datas.BPFrom != 0 && datas.BPTo != 0 && datas.Type == "Customer" && datas.Cusgroup == 0 && datas.Vengroup == 0)
            {
                var data = (from opp in _context.OpportunityMasterDatas.Where(x => datas.BPFrom <= x.BPID && x.BPID <= datas.BPTo)
                            join bp in _context.BusinessPartners.Where(x => x.Type == "Customer") on opp.BPID equals bp.ID
                            join po in _context.PotentialDetails on opp.ID equals po.OpportunityMasterDataID
                            join su in _context.SummaryDetail.Where(x => x.IsLost == true) on opp.ID equals su.OpportunityMasterDataID
                            select new OpportunityMasterData
                            {
                                ID = opp.ID,
                                OpportunityNo = opp.OpportunityNo,
                                OpportunityName = opp.OpportunityName,
                                BPName = bp.Name,
                                BPCode = bp.Code,
                                CloseingPerecnt = opp.CloingPercentage,
                                PotentialAmount = po.PotentailAmount,
                                WeightAmount = po.WeightAmount
                            }
                     ).ToList();
                return Ok(data);
            }
            else if (datas.BPFrom != 0 && datas.BPTo != 0 && datas.Type == "Vendor" && datas.Cusgroup == 0 && datas.Vengroup == 0)
            {
                var data = (from opp in _context.OpportunityMasterDatas.Where(x => datas.BPFrom <= x.BPID && x.BPID <= datas.BPTo)
                            join bp in _context.BusinessPartners.Where(x => x.Type == "Vendor") on opp.BPID equals bp.ID
                            join po in _context.PotentialDetails on opp.ID equals po.OpportunityMasterDataID
                            join su in _context.SummaryDetail.Where(x => x.IsLost == true) on opp.ID equals su.OpportunityMasterDataID
                            select new OpportunityMasterData
                            {
                                ID = opp.ID,
                                OpportunityNo = opp.OpportunityNo,
                                OpportunityName = opp.OpportunityName,
                                BPName = bp.Name,
                                BPCode = bp.Code,
                                CloseingPerecnt = opp.CloingPercentage,
                                PotentialAmount = po.PotentailAmount,
                                WeightAmount = po.WeightAmount
                            }
                     ).ToList();
                return Ok(data);
            }
            else if (datas.BPFrom != 0 && datas.BPTo != 0 && datas.Type == "All" && datas.Cusgroup == 0 && datas.Vengroup == 0)
            {
                var data = (from opp in _context.OpportunityMasterDatas.Where(x => datas.BPFrom <= x.BPID && x.BPID <= datas.BPTo)
                            join bp in _context.BusinessPartners on opp.BPID equals bp.ID
                            join po in _context.PotentialDetails on opp.ID equals po.OpportunityMasterDataID
                            join su in _context.SummaryDetail.Where(x => x.IsLost == true) on opp.ID equals su.OpportunityMasterDataID
                            select new OpportunityMasterData
                            {
                                ID = opp.ID,
                                OpportunityNo = opp.OpportunityNo,
                                OpportunityName = opp.OpportunityName,
                                BPName = bp.Name,
                                BPCode = bp.Code,
                                CloseingPerecnt = opp.CloingPercentage,
                                PotentialAmount = po.PotentailAmount,
                                WeightAmount = po.WeightAmount
                            }
                     ).ToList();
                return Ok(data);
            }
            else if (datas.BPFrom != 0 && datas.BPTo != 0 && string.IsNullOrEmpty(datas.Type) && datas.Cusgroup != 0 && datas.Vengroup == 0)
            {
                var data = (from opp in _context.OpportunityMasterDatas.Where(x => datas.BPFrom <= x.BPID && x.BPID <= datas.BPTo)
                            join bp in _context.BusinessPartners.Where(x => x.Group1ID == datas.Cusgroup) on opp.BPID equals bp.ID
                            join po in _context.PotentialDetails on opp.ID equals po.OpportunityMasterDataID
                            join su in _context.SummaryDetail.Where(x => x.IsLost == true) on opp.ID equals su.OpportunityMasterDataID
                            select new OpportunityMasterData
                            {
                                ID = opp.ID,
                                OpportunityNo = opp.OpportunityNo,
                                OpportunityName = opp.OpportunityName,
                                BPName = bp.Name,
                                BPCode = bp.Code,
                                CloseingPerecnt = opp.CloingPercentage,
                                PotentialAmount = po.PotentailAmount,
                                WeightAmount = po.WeightAmount
                            }
                     ).ToList();
                return Ok(data);
            }
            else if (datas.BPFrom != 0 && datas.BPTo != 0 && string.IsNullOrEmpty(datas.Type) && datas.Cusgroup == 0 && datas.Vengroup != 0)
            {
                var data = (from opp in _context.OpportunityMasterDatas.Where(x => datas.BPFrom <= x.BPID && x.BPID <= datas.BPTo)
                            join bp in _context.BusinessPartners.Where(x => x.Group2ID == datas.Vengroup) on opp.BPID equals bp.ID
                            join po in _context.PotentialDetails on opp.ID equals po.OpportunityMasterDataID
                            join su in _context.SummaryDetail.Where(x => x.IsLost == true) on opp.ID equals su.OpportunityMasterDataID
                            select new OpportunityMasterData
                            {
                                ID = opp.ID,
                                OpportunityNo = opp.OpportunityNo,
                                OpportunityName = opp.OpportunityName,
                                BPName = bp.Name,
                                BPCode = bp.Code,
                                CloseingPerecnt = opp.CloingPercentage,
                                PotentialAmount = po.PotentailAmount,
                                WeightAmount = po.WeightAmount
                            }
                     ).ToList();
                return Ok(data);
            }
            else if (datas.BPFrom != 0 && datas.BPTo != 0 && datas.Type == "Customer" && datas.Cusgroup != 0 && datas.Vengroup == 0)
            {
                var data = (from opp in _context.OpportunityMasterDatas.Where(x => datas.BPFrom <= x.BPID && x.BPID <= datas.BPTo)
                            join bp in _context.BusinessPartners.Where(x => x.Group1ID == datas.Cusgroup && x.Type == "Customer") on opp.BPID equals bp.ID
                            join po in _context.PotentialDetails on opp.ID equals po.OpportunityMasterDataID
                            join su in _context.SummaryDetail.Where(x => x.IsLost == true) on opp.ID equals su.OpportunityMasterDataID
                            select new OpportunityMasterData
                            {
                                ID = opp.ID,
                                OpportunityNo = opp.OpportunityNo,
                                OpportunityName = opp.OpportunityName,
                                BPName = bp.Name,
                                BPCode = bp.Code,
                                CloseingPerecnt = opp.CloingPercentage,
                                PotentialAmount = po.PotentailAmount,
                                WeightAmount = po.WeightAmount
                            }
                     ).ToList();
                return Ok(data);
            }
            else if (datas.BPFrom != 0 && datas.BPTo != 0 && datas.Type == "Vendor" && datas.Cusgroup == 0 && datas.Vengroup != 0)
            {
                var data = (from opp in _context.OpportunityMasterDatas.Where(x => datas.BPFrom <= x.BPID && x.BPID <= datas.BPTo)
                            join bp in _context.BusinessPartners.Where(x => x.Group2ID == datas.Vengroup && x.Type == "Vendor") on opp.BPID equals bp.ID
                            join po in _context.PotentialDetails on opp.ID equals po.OpportunityMasterDataID
                            join su in _context.SummaryDetail.Where(x => x.IsLost == true) on opp.ID equals su.OpportunityMasterDataID
                            select new OpportunityMasterData
                            {
                                ID = opp.ID,
                                OpportunityNo = opp.OpportunityNo,
                                OpportunityName = opp.OpportunityName,
                                BPName = bp.Name,
                                BPCode = bp.Code,
                                CloseingPerecnt = opp.CloingPercentage,
                                PotentialAmount = po.PotentailAmount,
                                WeightAmount = po.WeightAmount
                            }
                     ).ToList();
                return Ok(data);
            }
            else if (datas.BPFrom != 0 && datas.BPTo != 0 && datas.Type == "All" && datas.Cusgroup != 0 && datas.Vengroup == 0)
            {
                var data = (from opp in _context.OpportunityMasterDatas.Where(x => datas.BPFrom <= x.BPID && x.BPID <= datas.BPTo)
                            join bp in _context.BusinessPartners.Where(x => x.Group1ID == datas.Cusgroup) on opp.BPID equals bp.ID
                            join po in _context.PotentialDetails on opp.ID equals po.OpportunityMasterDataID
                            join su in _context.SummaryDetail.Where(x => x.IsLost == true) on opp.ID equals su.OpportunityMasterDataID
                            select new OpportunityMasterData
                            {
                                ID = opp.ID,
                                OpportunityNo = opp.OpportunityNo,
                                OpportunityName = opp.OpportunityName,
                                BPName = bp.Name,
                                BPCode = bp.Code,
                                CloseingPerecnt = opp.CloingPercentage,
                                PotentialAmount = po.PotentailAmount,
                                WeightAmount = po.WeightAmount
                            }
                     ).ToList();
                return Ok(data);
            }
            else if (datas.BPFrom != 0 && datas.BPTo != 0 && datas.Type == "All" && datas.Cusgroup == 0 && datas.Vengroup != 0)
            {
                var data = (from opp in _context.OpportunityMasterDatas.Where(x => datas.BPFrom <= x.BPID && x.BPID <= datas.BPTo)
                            join bp in _context.BusinessPartners.Where(x => x.Group2ID == datas.Vengroup) on opp.BPID equals bp.ID
                            join po in _context.PotentialDetails on opp.ID equals po.OpportunityMasterDataID
                            join su in _context.SummaryDetail.Where(x => x.IsLost == true) on opp.ID equals su.OpportunityMasterDataID
                            select new OpportunityMasterData
                            {
                                ID = opp.ID,
                                OpportunityNo = opp.OpportunityNo,
                                OpportunityName = opp.OpportunityName,
                                BPName = bp.Name,
                                BPCode = bp.Code,
                                CloseingPerecnt = opp.CloingPercentage,
                                PotentialAmount = po.PotentailAmount,
                                WeightAmount = po.WeightAmount
                            }
                     ).ToList();
                return Ok(data);
            }
            else if (datas.BPFrom == 0 && datas.BPTo == 0 && string.IsNullOrEmpty(datas.Type) && datas.Cusgroup != 0 && datas.Vengroup == 0)
            {
                var data = (from opp in _context.OpportunityMasterDatas
                            join bp in _context.BusinessPartners.Where(x => x.Group1ID == datas.Cusgroup) on opp.BPID equals bp.ID
                            join po in _context.PotentialDetails on opp.ID equals po.OpportunityMasterDataID
                            join su in _context.SummaryDetail.Where(x => x.IsLost == true) on opp.ID equals su.OpportunityMasterDataID
                            select new OpportunityMasterData
                            {
                                ID = opp.ID,
                                OpportunityNo = opp.OpportunityNo,
                                OpportunityName = opp.OpportunityName,
                                BPName = bp.Name,
                                BPCode = bp.Code,
                                CloseingPerecnt = opp.CloingPercentage,
                                PotentialAmount = po.PotentailAmount,
                                WeightAmount = po.WeightAmount
                            }
                     ).ToList();
                return Ok(data);
            }
            else if (datas.BPFrom == 0 && datas.BPTo == 0 && string.IsNullOrEmpty(datas.Type) && datas.Cusgroup == 0 && datas.Vengroup != 0)
            {
                var data = (from opp in _context.OpportunityMasterDatas
                            join bp in _context.BusinessPartners.Where(x => x.Group2ID == datas.Vengroup) on opp.BPID equals bp.ID
                            join po in _context.PotentialDetails on opp.ID equals po.OpportunityMasterDataID
                            join su in _context.SummaryDetail.Where(x => x.IsLost == true) on opp.ID equals su.OpportunityMasterDataID
                            select new OpportunityMasterData
                            {
                                ID = opp.ID,
                                OpportunityNo = opp.OpportunityNo,
                                OpportunityName = opp.OpportunityName,
                                BPName = bp.Name,
                                BPCode = bp.Code,
                                CloseingPerecnt = opp.CloingPercentage,
                                PotentialAmount = po.PotentailAmount,
                                WeightAmount = po.WeightAmount
                            }
                     ).ToList();
                return Ok(data);
            }
            else if (datas.BPFrom == 0 && datas.BPTo == 0 && string.IsNullOrEmpty(datas.Type) && datas.Cusgroup != 0 && datas.Vengroup != 0)
            {
                var data = (from opp in _context.OpportunityMasterDatas
                            join bp in _context.BusinessPartners.Where(x => x.Group2ID == datas.Vengroup && x.Group1ID == datas.Cusgroup) on opp.BPID equals bp.ID
                            join po in _context.PotentialDetails on opp.ID equals po.OpportunityMasterDataID
                            join su in _context.SummaryDetail.Where(x => x.IsLost == true) on opp.ID equals su.OpportunityMasterDataID
                            select new OpportunityMasterData
                            {
                                ID = opp.ID,
                                OpportunityNo = opp.OpportunityNo,
                                OpportunityName = opp.OpportunityName,
                                BPName = bp.Name,
                                BPCode = bp.Code,
                                CloseingPerecnt = opp.CloingPercentage,
                                PotentialAmount = po.PotentailAmount,
                                WeightAmount = po.WeightAmount
                            }
                     ).ToList();
                return Ok(data);
            }
            else if (datas.BPFrom == 0 && datas.BPTo == 0 && datas.Type == "All" && datas.Cusgroup != 0 && datas.Vengroup != 0)
            {
                var data = (from opp in _context.OpportunityMasterDatas
                            join bp in _context.BusinessPartners.Where(x => x.Group2ID == datas.Vengroup && x.Group1ID == datas.Cusgroup) on opp.BPID equals bp.ID
                            join po in _context.PotentialDetails on opp.ID equals po.OpportunityMasterDataID
                            join su in _context.SummaryDetail.Where(x => x.IsLost == true) on opp.ID equals su.OpportunityMasterDataID
                            select new OpportunityMasterData
                            {
                                ID = opp.ID,
                                OpportunityNo = opp.OpportunityNo,
                                OpportunityName = opp.OpportunityName,
                                BPName = bp.Name,
                                BPCode = bp.Code,
                                CloseingPerecnt = opp.CloingPercentage,
                                PotentialAmount = po.PotentailAmount,
                                WeightAmount = po.WeightAmount
                            }
                     ).ToList();
                return Ok(data);
            }
            else if (datas.BPFrom == 0 && datas.BPTo == 0 && datas.Type == "Customer" && datas.Cusgroup != 0 && datas.Vengroup == 0)
            {
                var data = (from opp in _context.OpportunityMasterDatas
                            join bp in _context.BusinessPartners.Where(x => x.Type == "Customer" && x.Group1ID == datas.Cusgroup) on opp.BPID equals bp.ID
                            join po in _context.PotentialDetails on opp.ID equals po.OpportunityMasterDataID
                            join su in _context.SummaryDetail.Where(x => x.IsLost == true) on opp.ID equals su.OpportunityMasterDataID
                            select new OpportunityMasterData
                            {
                                ID = opp.ID,
                                OpportunityNo = opp.OpportunityNo,
                                OpportunityName = opp.OpportunityName,
                                BPName = bp.Name,
                                BPCode = bp.Code,
                                CloseingPerecnt = opp.CloingPercentage,
                                PotentialAmount = po.PotentailAmount,
                                WeightAmount = po.WeightAmount
                            }
                     ).ToList();
                return Ok(data);
            }
            else if (datas.BPFrom == 0 && datas.BPTo == 0 && datas.Type == "Vendor" && datas.Cusgroup == 0 && datas.Vengroup != 0)
            {
                var data = (from opp in _context.OpportunityMasterDatas
                            join bp in _context.BusinessPartners.Where(x => x.Type == "Vendor" && x.Group2ID == datas.Vengroup) on opp.BPID equals bp.ID
                            join po in _context.PotentialDetails on opp.ID equals po.OpportunityMasterDataID
                            join su in _context.SummaryDetail.Where(x => x.IsLost == true) on opp.ID equals su.OpportunityMasterDataID
                            select new OpportunityMasterData
                            {
                                ID = opp.ID,
                                OpportunityNo = opp.OpportunityNo,
                                OpportunityName = opp.OpportunityName,
                                BPName = bp.Name,
                                BPCode = bp.Code,
                                CloseingPerecnt = opp.CloingPercentage,
                                PotentialAmount = po.PotentailAmount,
                                WeightAmount = po.WeightAmount
                            }
                     ).ToList();
                return Ok(data);
            }
            else if (!string.IsNullOrEmpty(datas.DateStartFrom) && !string.IsNullOrEmpty(datas.DateStartTo) && string.IsNullOrEmpty(datas.DateCloseFrom) && string.IsNullOrEmpty(datas.DateCloseTo) && string.IsNullOrEmpty(datas.DatePrFrom) && string.IsNullOrEmpty(datas.DatePrTo))
            {
                var data = (from opp in _context.OpportunityMasterDatas.Where(x => x.StartDate >= Convert.ToDateTime(datas.DateStartFrom) && x.StartDate <= Convert.ToDateTime(datas.DateStartTo))
                            join bp in _context.BusinessPartners on opp.BPID equals bp.ID
                            join po in _context.PotentialDetails on opp.ID equals po.OpportunityMasterDataID
                            join su in _context.SummaryDetail.Where(x => x.IsLost == true) on opp.ID equals su.OpportunityMasterDataID
                            select new OpportunityMasterData
                            {
                                ID = opp.ID,
                                OpportunityNo = opp.OpportunityNo,
                                OpportunityName = opp.OpportunityName,
                                BPName = bp.Name,
                                BPCode = bp.Code,
                                CloseingPerecnt = opp.CloingPercentage,
                                PotentialAmount = po.PotentailAmount,
                                WeightAmount = po.WeightAmount
                            }
                     ).ToList();
                return Ok(data);
            }
            else if (string.IsNullOrEmpty(datas.DateStartFrom) && datas.DateStartTo == null && datas.DateCloseFrom == null && datas.DateCloseTo == null && datas.DatePrFrom != null && datas.DatePrTo != null)
            {
                var data = (from opp in _context.OpportunityMasterDatas
                            join bp in _context.BusinessPartners on opp.BPID equals bp.ID
                            join po in _context.PotentialDetails.Where(x => x.PredictedClosingDate >= Convert.ToDateTime(datas.DatePrFrom) && x.PredictedClosingDate <= Convert.ToDateTime(datas.DatePrTo)) on opp.ID equals po.OpportunityMasterDataID
                            join su in _context.SummaryDetail.Where(x => x.IsLost == true) on opp.ID equals su.OpportunityMasterDataID
                            select new OpportunityMasterData
                            {
                                ID = opp.ID,
                                OpportunityNo = opp.OpportunityNo,
                                OpportunityName = opp.OpportunityName,
                                BPName = bp.Name,
                                BPCode = bp.Code,
                                CloseingPerecnt = opp.CloingPercentage,
                                PotentialAmount = po.PotentailAmount,
                                WeightAmount = po.WeightAmount
                            }
                     ).ToList();
                return Ok(data);
            }
            else if (!string.IsNullOrEmpty(datas.DateStartFrom) && datas.DateStartTo != null && datas.DateCloseFrom != null && datas.DateCloseTo != null && datas.DatePrFrom == null && datas.DatePrTo == null)
            {
                var data = (from opp in _context.OpportunityMasterDatas.Where(x => x.StartDate >= Convert.ToDateTime(datas.DateStartFrom) && x.StartDate <= Convert.ToDateTime(datas.DateStartTo) && x.ClosingDate >= Convert.ToDateTime(datas.DateCloseFrom) && x.ClosingDate <= Convert.ToDateTime(datas.DateCloseTo))
                            join bp in _context.BusinessPartners on opp.BPID equals bp.ID
                            join po in _context.PotentialDetails on opp.ID equals po.OpportunityMasterDataID
                            join su in _context.SummaryDetail.Where(x => x.IsLost == true) on opp.ID equals su.OpportunityMasterDataID
                            select new OpportunityMasterData
                            {
                                ID = opp.ID,
                                OpportunityNo = opp.OpportunityNo,
                                OpportunityName = opp.OpportunityName,
                                BPName = bp.Name,
                                BPCode = bp.Code,
                                CloseingPerecnt = opp.CloingPercentage,
                                PotentialAmount = po.PotentailAmount,
                                WeightAmount = po.WeightAmount
                            }
                     ).ToList();
                return Ok(data);
            }
            else if (string.IsNullOrEmpty(datas.DateStartFrom) && datas.DateStartTo == null && datas.DateCloseFrom != null && datas.DateCloseTo != null && datas.DatePrFrom != null && datas.DatePrTo != null)
            {
                var data = (from opp in _context.OpportunityMasterDatas.Where(x => x.ClosingDate >= Convert.ToDateTime(datas.DateCloseFrom) && x.ClosingDate <= Convert.ToDateTime(datas.DateCloseTo))
                            join bp in _context.BusinessPartners on opp.BPID equals bp.ID
                            join po in _context.PotentialDetails.Where(x => x.PredictedClosingDate >= Convert.ToDateTime(datas.DatePrFrom) && x.PredictedClosingDate <= Convert.ToDateTime(datas.DatePrTo)) on opp.ID equals po.OpportunityMasterDataID
                            join su in _context.SummaryDetail.Where(x => x.IsLost == true) on opp.ID equals su.OpportunityMasterDataID
                            select new OpportunityMasterData
                            {
                                ID = opp.ID,
                                OpportunityNo = opp.OpportunityNo,
                                OpportunityName = opp.OpportunityName,
                                BPName = bp.Name,
                                BPCode = bp.Code,
                                CloseingPerecnt = opp.CloingPercentage,
                                PotentialAmount = po.PotentailAmount,
                                WeightAmount = po.WeightAmount
                            }
                     ).ToList();
                return Ok(data);
            }
            else if (!string.IsNullOrEmpty(datas.DateStartFrom) && !string.IsNullOrEmpty(datas.DateStartTo) && string.IsNullOrEmpty(datas.DateCloseFrom) && string.IsNullOrEmpty(datas.DateCloseTo) && !string.IsNullOrEmpty(datas.DatePrFrom) && !string.IsNullOrEmpty(datas.DatePrTo))
            {
                var data = (from opp in _context.OpportunityMasterDatas.Where(x => x.StartDate >= Convert.ToDateTime(datas.DateStartFrom) && x.StartDate <= Convert.ToDateTime(datas.DateStartTo))
                            join bp in _context.BusinessPartners on opp.BPID equals bp.ID
                            join po in _context.PotentialDetails.Where(x => x.PredictedClosingDate >= Convert.ToDateTime(datas.DatePrFrom) && x.PredictedClosingDate <= Convert.ToDateTime(datas.DatePrTo)) on opp.ID equals po.OpportunityMasterDataID
                            join su in _context.SummaryDetail.Where(x => x.IsLost == true) on opp.ID equals su.OpportunityMasterDataID
                            select new OpportunityMasterData
                            {
                                ID = opp.ID,
                                OpportunityNo = opp.OpportunityNo,
                                OpportunityName = opp.OpportunityName,
                                BPName = bp.Name,
                                BPCode = bp.Code,
                                CloseingPerecnt = opp.CloingPercentage,
                                PotentialAmount = po.PotentailAmount,
                                WeightAmount = po.WeightAmount
                            }
                     ).ToList();
                return Ok(data);
            }
            else if (!string.IsNullOrEmpty(datas.DateStartFrom) && !string.IsNullOrEmpty(datas.DateStartTo) && !string.IsNullOrEmpty(datas.DateCloseFrom) && !string.IsNullOrEmpty(datas.DateCloseTo) && !string.IsNullOrEmpty(datas.DatePrFrom) && !string.IsNullOrEmpty(datas.DatePrTo))
            {
                var data = (from opp in _context.OpportunityMasterDatas.Where(x => x.StartDate >= Convert.ToDateTime(datas.DateStartFrom) && x.StartDate <= Convert.ToDateTime(datas.DateStartTo) && x.ClosingDate >= Convert.ToDateTime(datas.DateCloseFrom) && x.ClosingDate <= Convert.ToDateTime(datas.DateCloseTo))
                            join bp in _context.BusinessPartners on opp.BPID equals bp.ID
                            join po in _context.PotentialDetails.Where(x => x.PredictedClosingDate >= Convert.ToDateTime(datas.DatePrFrom) && x.PredictedClosingDate <= Convert.ToDateTime(datas.DatePrTo)) on opp.ID equals po.OpportunityMasterDataID
                            join su in _context.SummaryDetail.Where(x => x.IsLost == true) on opp.ID equals su.OpportunityMasterDataID
                            select new OpportunityMasterData
                            {
                                ID = opp.ID,
                                OpportunityNo = opp.OpportunityNo,
                                OpportunityName = opp.OpportunityName,
                                BPName = bp.Name,
                                BPCode = bp.Code,
                                CloseingPerecnt = opp.CloingPercentage,
                                PotentialAmount = po.PotentailAmount,
                                WeightAmount = po.WeightAmount
                            }
                     ).ToList();
                return Ok(data);
            }
            else if (datas.PoAmountFrom != 0 && datas.PoAmountTo != 0 && datas.WeAmountFrom == 0 && datas.WeAmountTo == 0 && datas.GrPrFrom == 0 && datas.GrPrTo == 0)
            {
                var data = (from opp in _context.OpportunityMasterDatas
                            join bp in _context.BusinessPartners on opp.BPID equals bp.ID
                            join po in _context.PotentialDetails.Where(x => x.PotentailAmount >= datas.PoAmountFrom && x.PotentailAmount <= datas.PoAmountTo) on opp.ID equals po.OpportunityMasterDataID
                            join su in _context.SummaryDetail.Where(x => x.IsLost == true) on opp.ID equals su.OpportunityMasterDataID
                            select new OpportunityMasterData
                            {
                                ID = opp.ID,
                                OpportunityNo = opp.OpportunityNo,
                                OpportunityName = opp.OpportunityName,
                                BPName = bp.Name,
                                BPCode = bp.Code,
                                CloseingPerecnt = opp.CloingPercentage,
                                PotentialAmount = po.PotentailAmount,
                                WeightAmount = po.WeightAmount
                            }
                     ).ToList();
                return Ok(data);
            }
            else if (datas.PoAmountFrom == 0 && datas.PoAmountTo == 0 && datas.WeAmountFrom != 0 && datas.WeAmountTo != 0 && datas.GrPrFrom == 0 && datas.GrPrTo == 0)
            {
                var data = (from opp in _context.OpportunityMasterDatas
                            join bp in _context.BusinessPartners on opp.BPID equals bp.ID
                            join po in _context.PotentialDetails.Where(x => x.WeightAmount >= datas.WeAmountFrom && x.WeightAmount <= datas.WeAmountTo) on opp.ID equals po.OpportunityMasterDataID
                            join su in _context.SummaryDetail.Where(x => x.IsLost == true) on opp.ID equals su.OpportunityMasterDataID
                            select new OpportunityMasterData
                            {
                                ID = opp.ID,
                                OpportunityNo = opp.OpportunityNo,
                                OpportunityName = opp.OpportunityName,
                                BPName = bp.Name,
                                BPCode = bp.Code,
                                CloseingPerecnt = opp.CloingPercentage,
                                PotentialAmount = po.PotentailAmount,
                                WeightAmount = po.WeightAmount
                            }
                     ).ToList();
                return Ok(data);
            }
            else if (datas.PoAmountFrom == 0 && datas.PoAmountTo == 0 && datas.WeAmountFrom == 0 && datas.WeAmountTo == 0 && datas.GrPrFrom != 0 && datas.GrPrTo != 0)
            {
                var data = (from opp in _context.OpportunityMasterDatas
                            join bp in _context.BusinessPartners on opp.BPID equals bp.ID
                            join po in _context.PotentialDetails.Where(x => x.GrossProfit >= datas.GrPrFrom && x.GrossProfit <= datas.GrPrTo) on opp.ID equals po.OpportunityMasterDataID
                            join su in _context.SummaryDetail.Where(x => x.IsLost == true) on opp.ID equals su.OpportunityMasterDataID
                            select new OpportunityMasterData
                            {
                                ID = opp.ID,
                                OpportunityNo = opp.OpportunityNo,
                                OpportunityName = opp.OpportunityName,
                                BPName = bp.Name,
                                BPCode = bp.Code,
                                CloseingPerecnt = opp.CloingPercentage,
                                PotentialAmount = po.PotentailAmount,
                                WeightAmount = po.WeightAmount
                            }
                     ).ToList();
                return Ok(data);
            }
            else if (datas.PoAmountFrom != 0 && datas.PoAmountTo != 0 && datas.WeAmountFrom != 0 && datas.WeAmountTo != 0 && datas.GrPrFrom == 0 && datas.GrPrTo == 0)
            {
                var data = (from opp in _context.OpportunityMasterDatas
                            join bp in _context.BusinessPartners on opp.BPID equals bp.ID
                            join po in _context.PotentialDetails.Where(x => x.PotentailAmount >= datas.PoAmountFrom && x.PotentailAmount <= datas.PoAmountTo && x.WeightAmount >= datas.WeAmountFrom && x.WeightAmount <= datas.WeAmountTo) on opp.ID equals po.OpportunityMasterDataID
                            join su in _context.SummaryDetail.Where(x => x.IsLost == true) on opp.ID equals su.OpportunityMasterDataID
                            select new OpportunityMasterData
                            {
                                ID = opp.ID,
                                OpportunityNo = opp.OpportunityNo,
                                OpportunityName = opp.OpportunityName,
                                BPName = bp.Name,
                                BPCode = bp.Code,
                                CloseingPerecnt = opp.CloingPercentage,
                                PotentialAmount = po.PotentailAmount,
                                WeightAmount = po.WeightAmount
                            }
                     ).ToList();
                return Ok(data);
            }
            else if (datas.PoAmountFrom != 0 && datas.PoAmountTo != 0 && datas.WeAmountFrom == 0 && datas.WeAmountTo == 0 && datas.GrPrFrom != 0 && datas.GrPrTo != 0)
            {
                var data = (from opp in _context.OpportunityMasterDatas
                            join bp in _context.BusinessPartners on opp.BPID equals bp.ID
                            join po in _context.PotentialDetails.Where(x => x.PotentailAmount >= datas.PoAmountFrom && x.PotentailAmount <= datas.PoAmountTo && x.GrossProfit >= datas.GrPrFrom && x.GrossProfit <= datas.GrPrTo) on opp.ID equals po.OpportunityMasterDataID
                            join su in _context.SummaryDetail.Where(x => x.IsLost == true) on opp.ID equals su.OpportunityMasterDataID
                            select new OpportunityMasterData
                            {
                                ID = opp.ID,
                                OpportunityNo = opp.OpportunityNo,
                                OpportunityName = opp.OpportunityName,
                                BPName = bp.Name,
                                BPCode = bp.Code,
                                CloseingPerecnt = opp.CloingPercentage,
                                PotentialAmount = po.PotentailAmount,
                                WeightAmount = po.WeightAmount
                            }
                     ).ToList();
                return Ok(data);
            }
            else if (datas.PoAmountFrom == 0 && datas.PoAmountTo == 0 && datas.WeAmountFrom != 0 && datas.WeAmountTo != 0 && datas.GrPrFrom != 0 && datas.GrPrTo != 0)
            {
                var data = (from opp in _context.OpportunityMasterDatas
                            join bp in _context.BusinessPartners on opp.BPID equals bp.ID
                            join po in _context.PotentialDetails.Where(x => x.WeightAmount >= datas.WeAmountFrom && x.WeightAmount <= datas.WeAmountTo && x.GrossProfit >= datas.GrPrFrom && x.GrossProfit <= datas.GrPrTo) on opp.ID equals po.OpportunityMasterDataID
                            join su in _context.SummaryDetail.Where(x => x.IsLost == true) on opp.ID equals su.OpportunityMasterDataID
                            select new OpportunityMasterData
                            {
                                ID = opp.ID,
                                OpportunityNo = opp.OpportunityNo,
                                OpportunityName = opp.OpportunityName,
                                BPName = bp.Name,
                                BPCode = bp.Code,
                                CloseingPerecnt = opp.CloingPercentage,
                                PotentialAmount = po.PotentailAmount,
                                WeightAmount = po.WeightAmount
                            }
                     ).ToList();
                return Ok(data);
            }
            else if (datas.PoAmountFrom != 0 && datas.PoAmountTo != 0 && datas.WeAmountFrom == 0 && datas.WeAmountTo == 0 && datas.GrPrFrom != 0 && datas.GrPrTo != 0)
            {
                var data = (from opp in _context.OpportunityMasterDatas
                            join bp in _context.BusinessPartners on opp.BPID equals bp.ID
                            join po in _context.PotentialDetails.Where(x => x.PotentailAmount >= datas.PoAmountFrom && x.PotentailAmount <= datas.PoAmountTo && x.GrossProfit >= datas.GrPrFrom && x.GrossProfit <= datas.GrPrTo) on opp.ID equals po.OpportunityMasterDataID
                            join su in _context.SummaryDetail.Where(x => x.IsLost == true) on opp.ID equals su.OpportunityMasterDataID
                            select new OpportunityMasterData
                            {
                                ID = opp.ID,
                                OpportunityNo = opp.OpportunityNo,
                                OpportunityName = opp.OpportunityName,
                                BPName = bp.Name,
                                BPCode = bp.Code,
                                CloseingPerecnt = opp.CloingPercentage,
                                PotentialAmount = po.PotentailAmount,
                                WeightAmount = po.WeightAmount
                            }
                     ).ToList();
                return Ok(data);
            }
            else if (datas.PerFrom != 0 && datas.PerTo != 0)
            {
                var data = (from opp in _context.OpportunityMasterDatas.Where(x => x.CloingPercentage >= datas.PerFrom && x.CloingPercentage <= datas.PerTo)
                            join bp in _context.BusinessPartners on opp.BPID equals bp.ID
                            join po in _context.PotentialDetails on opp.ID equals po.OpportunityMasterDataID
                            join su in _context.SummaryDetail.Where(x => x.IsLost == true) on opp.ID equals su.OpportunityMasterDataID
                            select new OpportunityMasterData
                            {
                                ID = opp.ID,
                                OpportunityNo = opp.OpportunityNo,
                                OpportunityName = opp.OpportunityName,
                                BPName = bp.Name,
                                BPCode = bp.Code,
                                CloseingPerecnt = opp.CloingPercentage,
                                PotentialAmount = po.PotentailAmount,
                                WeightAmount = po.WeightAmount
                            }
                     ).ToList();
                return Ok(data);
            }
            else if (datas.Competitor.Count > 0)
            {
                List<CompetitorDetail> competitorDetails = new();

                foreach (var items in datas.Competitor)
                {
                    var _competitor = _context.CompetitorDetail.Where(i => i.NameCompetitorID == items.OpportunityMasterDataID).ToList();
                    if (_competitor.Count > 0) competitorDetails.AddRange(_competitor);
                }
                //var datacom = JsonConvert.DeserializeObject<List<CompetitorDetail>>(datas.Competitor, new JsonSerializerSettings
                //{
                //    NullValueHandling = NullValueHandling.Ignore
                //});
                //var list = _context.CompetitorDetail.Where(x => datas.Competitor.Any(_x => _x.OpportunityMasterDataID == x.ID)).ToList();
                var data = (from com in competitorDetails
                            let opp = _context.OpportunityMasterDatas.Where(x => x.ID == com.OpportunityMasterDataID).FirstOrDefault() ?? new OpportunityMasterData()
                            join bp in _context.BusinessPartners on opp.BPID equals bp.ID
                            join po in _context.PotentialDetails on opp.ID equals po.OpportunityMasterDataID
                            join su in _context.SummaryDetail.Where(x => x.IsLost == true) on opp.ID equals su.OpportunityMasterDataID

                            select new OpportunityMasterData
                            {
                                ID = opp.ID,
                                OpportunityNo = opp.OpportunityNo,
                                OpportunityName = opp.OpportunityName,
                                BPName = bp.Name,
                                BPCode = bp.Code,
                                CloseingPerecnt = opp.CloingPercentage,
                                PotentialAmount = po.PotentailAmount,
                                WeightAmount = po.WeightAmount
                            }
                     ).ToList();
                return Ok(data);
            }
            else if (datas.Stages.Count > 0)
            {
                //var datastage = JsonConvert.DeserializeObject<List<StageDetail>>(datas.Stages, new JsonSerializerSettings
                //{
                //    NullValueHandling = NullValueHandling.Ignore
                //});
                //var list = _context.StageDetails.Where(x => datas.Stages.Any(_x => _x.ID == x.OpportunityMasterDataID)).ToList();
                List<StageDetail> stageDetails = new();

                foreach (var item in datas.Stages)
                {
                    var _stageDetails = _context.StageDetails.Where(i => i.StagesID == item.ID).ToList();
                    if (_stageDetails.Count > 0) stageDetails.AddRange(_stageDetails);
                }

                var data = (from st in stageDetails
                            join opp in _context.OpportunityMasterDatas on st.OpportunityMasterDataID equals opp.ID
                            join bp in _context.BusinessPartners on opp.BPID equals bp.ID
                            join po in _context.PotentialDetails on opp.ID equals po.OpportunityMasterDataID
                            join su in _context.SummaryDetail.Where(x => x.IsLost == true) on opp.ID equals su.OpportunityMasterDataID
                            select new OpportunityMasterData
                            {
                                ID = opp.ID,
                                OpportunityNo = opp.OpportunityNo,
                                OpportunityName = opp.OpportunityName,
                                BPName = bp.Name,
                                BPCode = bp.Code,
                                CloseingPerecnt = opp.CloingPercentage,
                                PotentialAmount = po.PotentailAmount,
                                WeightAmount = po.WeightAmount
                            }
                     ).ToList();
                return Ok(data);
            }
            return Ok();
        }
    }
    public class SearchWonOppParams
    {

        public int BPFrom { get; set; }
        public int BPTo { get; set; }
        public string Type { get; set; }
        public int Cusgroup { get; set; }
        public int Vengroup { get; set; }
        public string StartFrom { get; set; }
        public string StartTo { get; set; }
        public string CloseFrom { get; set; }
        public string CloseTo { get; set; }
        public List<SaleEmpParams> SaleEmp { get; set; }

    }
    public class SaleEmpParams
    {
        public int ID { get; set; }
    }
    //========lost oarams======
    public class SearchByBpFilterLostOppParams
    {
        public int BPFrom { get; set; }
        public int BPTo { get; set; }
        public string Type { get; set; }
        public int Cusgroup { get; set; }
        public int Vengroup { get; set; }
        public string DateStartFrom { get; set; }
        public string DateStartTo { get; set; }
        public string DateCloseFrom { get; set; }
        public string DateCloseTo { get; set; }
        public string DatePrFrom { get; set; }
        public string DatePrTo { get; set; }
        public decimal PoAmountFrom { get; set; }
        public decimal PoAmountTo { get; set; }
        public decimal WeAmountFrom { get; set; }
        public decimal WeAmountTo { get; set; }
        public decimal GrPrFrom { get; set; }
        public decimal GrPrTo { get; set; }
        public float PerFrom { get; set; }
        public float PerTo { get; set; }
        public List<CompetitorParam> Competitor { get; set; }
        public List<StageParam> Stages { get; set; }

    }

    public class StageParam
    {
        public int ID { get; set; }
    }
    public class CompetitorParam
    {
        public int OpportunityMasterDataID { get; set; }
    }
}
