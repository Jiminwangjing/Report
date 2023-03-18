using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CKBS.AppContext;
using CKBS.Models.Services.Responsitory;
using CKBS.Models.Services.HumanResources;
using Newtonsoft.Json;
using KEDI.Core.Models.Validation;
using System.Text.RegularExpressions;
using KEDI.Core.Premise.Authorization;
using KEDI.Core.Premise.Models.Services.HumanResources;
using KEDI.Core.Premise.Models.ServicesClass.GeneralSettingAdminView;
using KEDI.Core.Premise.Models.GeneralSettingAdmin;
using KEDI.Core.Premise.Models.ServicesClass.ActivityViewModel;
using KEDI.Core.Premise.Models.Services.Activity;
using CKBS.Models.Services.Administrator.General;
using KEDI.Core.Helpers.Enumerations;
using KEDI.Core.Premise.Models.ServicesClass;
using CKBS.Models.Services.Account;
using KEDI.Core.Premise.Repository;
using KEDI.Core.Premise.Models.Sale;
using CKBS.Models.Services.POS;
using KEDI.Core.Premise.Models.ServicesClass.Report;
using CKBS.Models.Services.Administrator.SystemInitialization;
using CKBS.Models.Services.Banking;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Microsoft.AspNetCore.Http;
using CKBS.Models.Services.ExcelFile;
using KEDI.Core.Premise.Models.Services.HumanResources.Templates;
using KEDI.Core.System.Models;
using KEDI.Core.Premise.Utilities;

namespace CKBS.Controllers
{
    [Privilege]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class BusinessPartnerController : Controller
    {
        private readonly IBusinessPartner _bPartner;
        private readonly DataContext _context;
        private readonly UtilityModule _fncModule;
        private readonly IWebHostEnvironment _env;
        private readonly IWorkbookAdapter _wbAdapter;
        public BusinessPartnerController(IBusinessPartner partner, DataContext context,
            UtilityModule format, IWorkbookAdapter wbAdapter, IWebHostEnvironment env)
        {
            _bPartner = partner;
            _context = context;
            _fncModule = format;
            _wbAdapter = wbAdapter;
            _env = env;
        }
        public Dictionary<int, string> AssignedTo => EnumHelper.ToDictionary(typeof(AssignedTo));
        public Dictionary<int, string> Recurrence => EnumHelper.ToDictionary(typeof(Recurrence));
        public Dictionary<int, string> Priorities => EnumHelper.ToDictionary(typeof(Priorities));
        //enum of payment term
        public Dictionary<int, string> PaymentTermsDisplay => new()
        {
            { 0, "" },
            { 1, "Starting Month" },
            { 2, "Haft of Month" },
            { 3, "End of Month" }
        };
        public Dictionary<int, string> DueDateDisplay => new()
        {
            { 0, "" },
            { 1, "Posting Date" },
            { 2, "System Date" },
            { 3, "Document Date" }
        };
        public Dictionary<int, string> IncomingPaymentdisplay => new()
        {
            { 0, "" },
            { 1, "No." },
            { 2, "Cash" },
            { 3, "Checks" },
            { 4, "Credit" },
            { 5, "Bink Transfer" }
        };
        public Dictionary<int, string> CreditMethodDisplay => new()
        {
            { 0, "" },
            { 1, "First Instaillment" },
            { 2, "Last Instaillment" },
            { 3, "Equally" }
        };
        public Dictionary<int, string> DueDate => new()
        {
            { 0, "0" },
            { 1, "SystemDate" },
            { 2, "PostingDate" },
            { 3, "DocumentDate" }
        };

        //end of enum payment term
        [Privilege("BP001")]
        public IActionResult Customer()
        {
            ViewBag.Customer = "highlight";
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> GetFromExcel(IFormFile formFile)
        {
            var bpDicationary = new Dictionary<string, List<BusinessPartner>>();
            if (formFile == null)
            {
                return Ok(bpDicationary);
            }
            bpDicationary = await _bPartner.GetFromExcelAsync(ModelState, formFile);
            return Ok(bpDicationary);
        }

        public IActionResult ImportMasterData()
        {
            ViewBag.ImportBp = "highlight";
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ImportMasterData([FromForm] ExcelFormFile excelForm)
        {
            ViewBag.ImportBp = "highlight";
            await _bPartner.SubmitFromExcelAsync(ModelState, excelForm.FormFile, excelForm.SheetIndex);
            var message = new ModelMessage(ModelState);
            return Ok(message);
        }

        public async Task<IActionResult> GetExcelTemplate()
        {
            string fullPath = Path.GetFullPath(string.Format("{0}{1}", _env.WebRootPath, "/FileTemplate/BusinessPartner.xlsx"));

            MemoryStream memory = new();
            using Stream fs = System.IO.File.OpenRead(fullPath);
            await fs.CopyToAsync(memory);
            memory.Position = 0;
            return File(memory, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", Path.GetFileName(fullPath));
        }

        public async Task<IActionResult> GetImportReference()
        {
            var glaccts = await _bPartner.GetGLAccountsAsync();
            var pricelists = await _bPartner.GetPriceListsAsync();
            return Ok(new ImportReference
            {
                GLAccounts = glaccts,
                PriceLists = pricelists
            });
        }

        //==========Calandar===========
        public IActionResult Calandar()
        {
            return View();
        }
        //============get next number==========
        public IActionResult GetNum()
        {
            int num;
            var list = _context.Activites.ToList();
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
        //=========get bp===============
        [HttpGet]
        [HttpGet]
        public IActionResult GetBP()
        {
            //var bp = _context.BusinessPartners.Where(x => x.Type == "Customer").ToList();
            var bp = (from b in _context.BusinessPartners.Where(x => x.Type == "Customer" && x.Delete == false)
                      join p in _context.PriceLists on b.PriceListID equals p.ID
                      let g1 = _context.GroupCustomer1s.FirstOrDefault(i => i.ID == b.Group1ID) ?? new GroupCustomer1()
                      select new BusinessPartner
                      {
                          ID = b.ID,
                          Code = b.Code,
                          Name = b.Name,
                          Type = b.Type,
                          Group1 = g1.Name ?? "",
                          PriceListName = p.Name,
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
            return Ok(bp);
        }
        //=======get data assignto==============
        public IActionResult GetData(int typeassigned)
        {
            if (typeassigned == 2)
            {
                var data = (from em in _context.Employees
                            select new Employee
                            {
                                ID = em.ID,
                                Name = em.Name
                            }
                           ).ToList();
                return Ok(data);
            }
            else if (typeassigned == 1)
            {
                var data = (from ur in _context.UserAccounts
                            select new
                            {
                                ur.ID,
                                ur.Username
                            }
                          ).ToList();
                return Ok(data);
            }
            return Ok();

        }
        //end index of customer
        //====================Get Territory=====================
        public IActionResult GetTerritoryEmployee()
        {
            var data = _context.Territories.ToList();
            return Ok(data);
        }

        //=====================end Get Territory==========
        //======================Activity========================
        public IActionResult GetListDataActivity(int id)
        {
            if (id != 0)
            {
                var data = (from ac in _context.Activites.Where(x => x.UserID == id)
                            join g in _context.Generals on ac.ID equals g.ActivityID
                            join us in _context.UserAccounts on ac.UserID equals us.ID
                            let emp = _context.Employees.FirstOrDefault(x => x.ID == us.EmployeeID) ?? new Employee()
                            let st = _context.SetupStatuses.FirstOrDefault(x => x.ID == g.StatusID) ?? new SetupStatus()
                            select new ActivityView
                            {
                                ActivityID = g.ActivityID,
                                ID = ac.ID,
                                Remark = g.Remark,
                                TypeID = ac.TypeID,
                                StartTime = g.StartTime,
                                EndTime = g.EndTime,
                                StartHour = g.StartTime.ToShortTimeString(),
                                EndHour = g.EndTime.ToShortTimeString(),
                                Durration = g.Durration,
                                StatusID = g.StatusID,
                                Color = st.Color,
                                Status = st.Status,
                                GetStartHour = g.StartTime.ToString("HH:mm"),
                                GetEndHour = g.EndTime.ToString("HH:mm"),
                                EmpID = ac.EmpID,
                                EmpName = emp.Name,
                                Number = ac.Number,
                                UserID = ac.UserID,
                            }
                       ).ToList();
                return Ok(data);
            }
            else
            {
                var data = (from ac in _context.Activites
                            join g in _context.Generals on ac.ID equals g.ActivityID
                            let emp = _context.Employees.FirstOrDefault(x => x.ID == ac.EmpID) ?? new Employee()
                            let st = _context.SetupStatuses.FirstOrDefault(x => x.ID == g.StatusID) ?? new SetupStatus()
                            select new ActivityView
                            {
                                ActivityID = g.ActivityID,
                                ID = ac.ID,
                                Remark = g.Remark,
                                TypeID = ac.TypeID,
                                StartTime = g.StartTime,
                                EndTime = g.EndTime,
                                StatusID = g.StatusID,
                                Color = st.Color,
                                EmpID = ac.EmpID,
                                EmpName = emp.Name,
                                Durration = g.Durration,
                                GetStartHour = g.StartTime.ToString("HH:mm"),
                                GetEndHour = g.EndTime.ToString("HH:mm"),
                                Number = ac.Number,
                                UserID = ac.UserID,
                            }
                      ).ToList();
                return Ok(data);
            }

        }
        public IActionResult GetListDataEmp()
        {
            var list = (from ac in _context.Activites
                        join g in _context.Generals on ac.ID equals g.ActivityID
                        join us in _context.UserAccounts on ac.UserID equals us.ID
                        let emp = _context.Employees.FirstOrDefault(x => x.ID == us.EmployeeID) ?? new Employee()
                        let st = _context.SetupStatuses.FirstOrDefault(x => x.ID == g.StatusID) ?? new SetupStatus()
                        group new { ac, st, emp } by new { ac.UserID } into g
                        let data = g.FirstOrDefault()
                        select new ActivityView
                        {
                            LineID = DateTime.Now.Ticks.ToString(),
                            ID = data.ac.ID,
                            UserID = data.ac.UserID,
                            EmpID = data.ac.EmpID,
                            EmpName = data.emp.Name,
                            Color = data.st.Color

                        }).ToList();
            return Ok(list);
        }
        private GeneralSettingAdminViewModel GetGeneralSettingAdmin()
        {
            Display display = _context.Displays.FirstOrDefault() ?? new Display();
            GeneralSettingAdminViewModel data = new()
            {
                Display = display
            };
            return data;
        }
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
            ViewBag.Status = new SelectList(_context.SetupStatuses, "ID", "Status");
            ViewBag.Location = new SelectList(_context.SetupLocations, "Name", "Name");
            var userEmp = _context.UserAccounts.Include(i => i.Employee).Where(i => !i.Delete)
                .Select(i => new UserAccount { ID = i.ID, Username = i.Employee.Name }).ToList();
            ViewBag.Emp = new SelectList(userEmp, "ID", "Username");
            //ViewBag.Emp = new SelectList(_context.Employees, "ID", "Name");
            return View(assignedto);
        }
        //=====get empty table add list activity=====

        [HttpGet]
        public IActionResult GetEmptyTableAddActivity()
        {
            List<SetupActivity> setupactivities = new();
            var data = _context.SetupActivities.ToList();
            setupactivities.AddRange(data);
            for (var i = 1; i <= 5; i++)
            {
                var setupactivity = new SetupActivity
                {
                    LineID = DateTime.Now.Ticks.ToString(),
                    ID = 0,
                    Activities = ""
                };
                setupactivities.Add(setupactivity);
            }
            return Ok(setupactivities);

        }

        //=========insert activity list===================
        [HttpPost]
        public IActionResult InsertActivity(List<SetupActivity> activities)
        {
            ModelMessage msg = new();
            if (ModelState.IsValid)
            {
                _context.SetupActivities.UpdateRange(activities);
                _context.SaveChanges();
                ModelState.AddModelError("success", "Activity created succussfully!");
                msg.Approve();
            }
            var data = _context.SetupActivities.ToList();
            return Ok(new { Model = msg.Bind(ModelState), Data = data });
        }
        //============get empty table list type===========

        [HttpGet]
        public IActionResult GetEmptyTableListType()
        {
            List<SetupType> types = new();
            var data = _context.SetupTypes.ToList();
            types.AddRange(data);
            for (var i = 1; i <= 5; i++)
            {
                var type = new SetupType
                {
                    LineID = DateTime.Now.Ticks.ToString(),
                    ID = 0,
                    Name = ""
                };
                types.Add(type);
            }
            return Ok(types);

        }
        //=================get empty table setup customer source====

        [HttpGet]
        public IActionResult GetEmptyTableListTerritory()
        {
            List<SetupCustomerSource> territory = new();
            var data = _context.SetupCustomerSources.ToList();
            territory.AddRange(data);
            for (var i = 1; i <= 10; i++)
            {
                var terr = new SetupCustomerSource
                {
                    LineID = DateTime.Now.Ticks.ToString(),
                    ID = 0,
                    Name = ""
                };
                territory.Add(terr);
            }
            return Ok(territory);

        }
        //=========insert type list===================
        [HttpPost]
        public IActionResult InsertType(List<SetupType> types)
        {
            ModelMessage msg = new();
            if (ModelState.IsValid)
            {
                _context.SetupTypes.UpdateRange(types);
                _context.SaveChanges();
                ModelState.AddModelError("success", "Type created succussfully!");
                msg.Approve();
            }
            var data = _context.SetupTypes.ToList();
            return Ok(new { Model = msg.Bind(ModelState), Data = data });
        }

        //===============insert territory===========
        [HttpPost]
        public IActionResult InsertTerrority(List<SetupCustomerSource> territory)
        {
            ModelMessage msg = new();
            if (ModelState.IsValid)
            {
                _context.SetupCustomerSources.UpdateRange(territory);
                _context.SaveChanges();
                ModelState.AddModelError("success", "Type created succussfully!");
                msg.Approve();
            }
            var data = _context.SetupCustomerSources.ToList();
            return Ok(new { Model = msg.Bind(ModelState), Data = data });
        }

        //============get empty table list subject===========

        private List<SetupType> SetupTypes()
        {
            var data = _context.SetupTypes.ToList();
            // as add
            data.Insert(0, new SetupType
            {
                Name = "-- Select --",
                ID = 0,
            });
            return data;
        }
        [HttpGet]
        public IActionResult GetEmptyTableListSubject()
        {
            List<SetupSubject> setupSubjects = new();
            var setupsubj = (from sub in _context.SetupSubjects
                             select new SetupSubject
                             {
                                 ID = sub.ID,
                                 Name = sub.Name,
                                 SetupType = SetupTypes().Select(i => new SelectListItem
                                 {
                                     Text = i.Name, //+ i.Name,
                                     Value = i.ID.ToString(),
                                     Selected = sub.TypeID == i.ID
                                 }).ToList(),
                                 TypeID = sub.TypeID
                             }).ToList();
            setupSubjects.AddRange(setupsubj);

            for (var i = 1; i <= 5; i++)
            {
                SetupSubject subject = new()
                {
                    LineID = DateTime.Now.Ticks.ToString(),
                    ID = 0,
                    Name = "",
                    SetupType = SetupTypes().Select(i => new SelectListItem
                    {
                        Text = i.Name, //+ i.Name,
                        Value = i.ID.ToString(),
                    }).ToList(),
                };
                setupSubjects.Add(subject);
            }
            return Ok(setupSubjects);
        }
        //=========insert type list===================

        [HttpPost]
        public IActionResult InsertSubject(List<SetupSubject> subject)
        {
            ModelMessage msg = new();
            if (ModelState.IsValid)
            {
                _context.SetupSubjects.UpdateRange(subject);
                _context.SaveChanges();
                ModelState.AddModelError("success", "Subject created succussfully!");
                msg.Approve();
            }
            var data = _context.SetupSubjects.ToList();
            return Ok(new { Model = msg.Bind(ModelState), Data = data });
        }
        //================get empty table status======
        [HttpGet]
        public IActionResult GetEmptyTableListStatus()
        {
            List<SetupStatus> setupStatuses = new();
            var data = _context.SetupStatuses.ToList();
            setupStatuses.AddRange(data);
            for (var i = 1; i <= 5; i++)
            {
                var setupstatus = new SetupStatus
                {
                    LineID = DateTime.Now.Ticks.ToString(),
                    ID = 0,
                    Status = "",
                    Color = ""
                };
                setupStatuses.Add(setupstatus);
            }
            return Ok(setupStatuses);

        }
        //=========insert status list===================
        [HttpPost]
        public IActionResult InsertStatus(List<SetupStatus> setupstatus)
        {
            ModelMessage msg = new();
            if (ModelState.IsValid)
            {
                _context.SetupStatuses.UpdateRange(setupstatus);
                _context.SaveChanges();
                ModelState.AddModelError("success", "Status created succussfully!");
                msg.Approve();
            }
            var data = _context.SetupStatuses.ToList();
            return Ok(new { Model = msg.Bind(ModelState), Data = data });
        }
        //================get empty table location======
        [HttpGet]
        public IActionResult GetEmptyTableListLocation()
        {
            List<SetupLocation> setupLocations = new();
            var data = _context.SetupLocations.ToList();
            setupLocations.AddRange(data);
            for (var i = 1; i <= 5; i++)
            {
                var setuplocation = new SetupLocation
                {
                    LineID = DateTime.Now.Ticks.ToString(),
                    ID = 0,
                    Name = ""
                };
                setupLocations.Add(setuplocation);
            }
            return Ok(setupLocations);
        }
        //=========insert location list===================
        [HttpPost]
        public IActionResult InsertLocation(List<SetupLocation> setuplocation)
        {
            ModelMessage msg = new();
            if (ModelState.IsValid)
            {
                _context.SetupLocations.UpdateRange(setuplocation);
                _context.SaveChanges();
                ModelState.AddModelError("success", "SetupLocation created succussfully!");
                msg.Approve();
            }
            var data = _context.SetupLocations.ToList();
            return Ok(new { Model = msg.Bind(ModelState), Data = data });
        }
        //==========submit date acitvity======
        public IActionResult GeteDaysOfMonths1(DateTime date1, DateTime date2)
        {
            var year1 = date1.Year;
            var month1 = date1.Month;
            var month2 = date2.Month;
            var months = month1 - month2;
            var dayOfMonthCount = DateTime.DaysInMonth(year1, month1);

            var days = 0;
            if (months > 0)
            {
                for (var x = 1; x <= months; x++)
                {
                    for (var i = 1; i <= dayOfMonthCount; i++)
                    {
                        days = i;
                    }
                    days += days;

                }
                return Ok(days);
            }
            else if (months == 0)
            {
                for (var i = 1; i <= dayOfMonthCount; i++)
                {
                    days = i;
                }
                return Ok(days);
            }

            return Ok();
        }
        public IActionResult GeteDaysOfMonths2(DateTime date2)
        {
            var year = date2.Year;
            var month = date2.Month;
            var dayOfMonthCount = DateTime.DaysInMonth(year, month);
            var days = 0;
            for (var i = 1; i <= dayOfMonthCount; i++)
            {
                days = i;
            }

            return Ok(days);
        }
        private void ValidateSummary(Activity activity, General general)
        {

            if (activity.Activities == 0)
            {
                ModelState.AddModelError("Activities", "Please select  Actitvity Type ");
            }
            if (activity.UserID == 0)
            {
                ModelState.AddModelError("UserID", "Please select  User ");

            }
            if (general.Remark == "")
            {
                ModelState.AddModelError("Remark", "Please input  Remark ");
            }
            //if (activity.BPID ==0 )
            //{
            //    ModelState.AddModelError("BPID", "Please Choosse Customer");
            //}
            //if (general.EndTime ==null)
            //{
            //    ModelState.AddModelError("SetupActName", "Please input EndTime");
            //}
        }

        [HttpPost]
        public IActionResult SubmmitData(string _data)
        {
            Activity data = JsonConvert.DeserializeObject<Activity>(_data, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });

            ValidateSummary(data, data.General);
            ModelMessage msg = new();
            if (ModelState.IsValid)
            {
                _context.Activites.Update(data);
                _context.SaveChanges();
                ModelState.AddModelError("success", "Activity created succussfully!");
                msg.Approve();
            }
            return Ok(msg.Bind(ModelState));
        }
        //========Update Data========
        [HttpPost]
        public IActionResult UpdateData(string _data)
        {
            Activity data = JsonConvert.DeserializeObject<Activity>(_data, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });
            ValidateSummary(data, data.General);
            ModelMessage msg = new();
            if (ModelState.IsValid)
            {
                _context.Activites.Update(data);
                _context.SaveChanges();
                ModelState.AddModelError("success", "Activity updated succussfully!");
                msg.Approve();
            }
            return Ok(msg.Bind(ModelState));
        }
        //========Update Data Calandar========
        [HttpPost]
        public IActionResult UpdateDataCalandar(string _data)
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
            return Ok(new { url = "/BusinessPartner/Calandar" });
        }


        //===========find data activity======
        public IActionResult FindActivity(int number)
        {
            var data = _bPartner.FindActivity(number);
            return Ok(data);
        }
        //codeid
        private static List<string> GetBcode(params string[] codes)
        {
            List<string> _code = new();
            if (codes.Length > 0)
            {
                foreach (var c in codes)
                {
                    if (c.Contains("C00"))
                    {
                        string[] _cds = c.Split("C00");
                        foreach (string cd in _cds)
                        {
                            _code.Add(cd);
                        }
                    }
                    else
                    {
                        _code.Add(c);
                    }
                }
                return _code;
            }
            return _code.ToList();
        }

        //index of vendor
        [Privilege("BP002")]
        public IActionResult Vendor()
        {
            ViewBag.Vendor = "highlight";
            return View();
        }

        //end index of vendor
        static string RawWord(string keyword)
        {
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                return Regex.Replace(keyword, "\\+s", string.Empty);
            }
            return string.Empty;
        }


        //get customer
        public IActionResult GetCustomer(string keyword = "")
        {
            int userid = int.Parse(User.FindFirst("UserID").Value);
            var businessPartners = (from b in _context.BusinessPartners.Where(e => e.Delete == false && e.Type == "Customer")
                                    join p in _context.PriceLists on b.PriceListID equals p.ID
                                    let g1 = _context.GroupCustomer1s.FirstOrDefault(i => i.ID == b.Group1ID) ?? new GroupCustomer1()
                                    select new
                                    {
                                        b.ID,
                                        b.Code,
                                        b.Name,
                                        b.Type,
                                        Group1 = g1.Name ?? "",
                                        b.Phone,
                                        PriceList = p.Name
                                    }).ToList();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                keyword = RawWord(keyword);
                StringComparison ignoreCase = StringComparison.InvariantCultureIgnoreCase;
                businessPartners = businessPartners.Where(c =>
                    RawWord(c.Code).Contains(keyword, ignoreCase)
                || RawWord(c.Name).Contains(keyword, ignoreCase)
                || RawWord(c.Phone).Contains(keyword, ignoreCase)
                || RawWord(c.PriceList).Contains(keyword, ignoreCase)
                || RawWord(c.Group1).Contains(keyword, ignoreCase)).ToList();
            }
            return Ok(businessPartners.ToList());
        }
        //end get customer

        //get vendor
        public IActionResult GetVendor(string keyword = "")
        {
            int userid = int.Parse(User.FindFirst("UserID").Value);
            var businessPartners = (from b in _context.BusinessPartners.Where(e => e.Delete == false && e.Type == "Vendor")
                                    join p in _context.PriceLists on b.PriceListID equals p.ID
                                    let g1 = _context.GroupCustomer1s.FirstOrDefault(i => i.ID == b.Group1ID) ?? new GroupCustomer1()
                                    select new
                                    {
                                        b.ID,
                                        b.Code,
                                        b.Name,
                                        b.Type,
                                        Group1 = g1.Name ?? "",
                                        b.Phone,
                                        PriceList = p.Name
                                    }).ToList();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                keyword = RawWord(keyword);
                StringComparison ignoreCase = StringComparison.InvariantCultureIgnoreCase;
                businessPartners = businessPartners.Where(c =>
                RawWord(c.Code).Contains(keyword, ignoreCase)
                || RawWord(c.Name).Contains(keyword, ignoreCase)
                || RawWord(c.Phone).Contains(keyword, ignoreCase)
                || RawWord(c.PriceList).Contains(keyword, ignoreCase)
                || RawWord(c.Group1).Contains(keyword, ignoreCase)).ToList();
            }
            return Ok(businessPartners.ToList());
        }

        [Privilege("BP001")]
        public IActionResult CreateCustomer()
        {
            ViewBag.Customer = "highlight";
            BusinessPartner _parnter = new();
            _parnter.AutoMobile = new List<AutoMobile>();
            BusinessPartnerViewModel am_sv = new()
            {
                BusinessPartner = _parnter,
                PaymentTerms = PaymentTermsDisplay.Select(p => new SelectListItem
                {
                    Value = p.Key.ToString(),
                    Text = p.Value
                }).ToList(),

                DueDate = DueDateDisplay.Select(d => new SelectListItem
                {
                    Value = d.Key.ToString(),
                    Text = d.Value
                }).ToList(),

                OpenIncomingPayment = IncomingPaymentdisplay.Select(d => new SelectListItem
                {
                    Value = d.Key.ToString(),
                    Text = d.Value
                }).ToList(),
                CreditMethod = CreditMethodDisplay.Select(i => new SelectListItem
                {
                    Value = i.Key.ToString(),
                    Text = i.Value
                }).ToList(),
            };

            var pricelists = _context.PriceLists.Where(p => !p.Delete).ToList();
            ViewData["Types"] = new SelectList(_context.AutoTypes.Where(c => c.Active), "TypeID", "TypeName");
            ViewData["Brands"] = new SelectList(_context.AutoBrands.Where(c => c.Active), "BrandID", "BrandName");
            ViewData["Models"] = new SelectList(_context.AutoModels.Where(c => c.Active), "ModelID", "ModelName");
            ViewData["Colors"] = new SelectList(_context.AutoColors.Where(c => c.Active), "ColorID", "ColorName");

            ViewData["B"] = _bPartner.SelectPricelists(0);
            ViewData["G"] = _bPartner.SelectGroup1(0, "Customer");
            ViewData["G2"] = _bPartner.SelectGroup2(0, "Customer");
            ViewData["P"] = new SelectList(_context.PaymentTerms, "ID", "Code");
            ViewData["D"] = new SelectList(_context.CashDiscounts, "ID", "CodeName");
            ViewData["T"] = new SelectList(_context.SetupCustomerSources, "ID", "Name");
            var cus = _context.BusinessPartners.Where(i => i.Type == "Customer").ToList().Count;
            ViewBag.Code = $"C{220000 + cus}";
            return View(am_sv);
        }
        //create vendor
        [Privilege("BP002")]
        public IActionResult CreateVendor()
        {
            ViewBag.Vendor = "highlight";
            BusinessPartner _parnter = new();
            _parnter.AutoMobile = new();
            BusinessPartnerViewModel am_sv = new()
            {
                BusinessPartner = _parnter,
                PaymentTerms = PaymentTermsDisplay.Select(p => new SelectListItem
                {
                    Value = p.Key.ToString(),
                    Text = p.Value
                }).ToList(),

                DueDate = DueDateDisplay.Select(d => new SelectListItem
                {
                    Value = d.Key.ToString(),
                    Text = d.Value
                }).ToList(),

                OpenIncomingPayment = IncomingPaymentdisplay.Select(d => new SelectListItem
                {
                    Value = d.Key.ToString(),
                    Text = d.Value
                }).ToList(),
                CreditMethod = CreditMethodDisplay.Select(i => new SelectListItem
                {
                    Value = i.Key.ToString(),
                    Text = i.Value
                }).ToList(),
            };

            ViewData["Types"] = new SelectList(_context.AutoTypes.Where(c => c.Active), "TypeID", "TypeName");
            ViewData["Brands"] = new SelectList(_context.AutoBrands.Where(c => c.Active), "BrandID", "BrandName");
            ViewData["Models"] = new SelectList(_context.AutoModels.Where(c => c.Active), "ModelID", "ModelName");
            ViewData["Colors"] = new SelectList(_context.AutoColors.Where(c => c.Active), "ColorID", "ColorName");

            ViewData["B"] = _bPartner.SelectPricelists(0);
            ViewData["G1"] = _bPartner.SelectGroup1(0, "Vendor");
            ViewData["G2"] = _bPartner.SelectGroup2(0, "Vendor");
            ViewData["P"] = new SelectList(_context.PaymentTerms, "ID", "Code");
            ViewData["D"] = new SelectList(_context.CashDiscounts, "ID", "CodeName");
            var cus = _context.BusinessPartners.Where(i => i.Type == "Vendor").ToList().Count;
            ViewBag.Code = $"V{220000 + cus}";
            return View(am_sv);
        }
        [HttpPost]
        public IActionResult CreateVendor(string data)
        {
            BusinessPartner business = JsonConvert.DeserializeObject<BusinessPartner>(data, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });
            BusinessPartnerViewModel bpViewModel = new()
            {
                BusinessPartner = business ?? new BusinessPartner(),
                PriceLists = new SelectList(_context.PriceLists, "ID", "Name"),
                PaymentTerms = PaymentTermsDisplay.Select(p => new SelectListItem
                {
                    Value = p.Key.ToString(),
                    Text = p.Value
                }).ToList(),

                DueDate = DueDateDisplay.Select(d => new SelectListItem
                {
                    Value = d.Key.ToString(),
                    Text = d.Value
                }).ToList(),

                OpenIncomingPayment = IncomingPaymentdisplay.Select(d => new SelectListItem
                {
                    Value = d.Key.ToString(),
                    Text = d.Value
                }).ToList(),
                CreditMethod = CreditMethodDisplay.Select(i => new SelectListItem
                {
                    Value = i.Key.ToString(),
                    Text = i.Value
                }).ToList()

            };

            var buscode = _context.BusinessPartners.FirstOrDefault(c => c.Code == business.Code);
            if (buscode != null)
            {
                ViewBag.ErrorCode = "This code already exist !";
                ViewData["B"] = _bPartner.SelectPricelists(business.PriceListID);
                ViewData["G1"] = _bPartner.SelectGroup1(business.Group2ID, "Vendor");
                ViewData["G2"] = _bPartner.SelectGroup2(business.Group2ID, "Vendor");
                ViewData["P"] = new SelectList(_context.PaymentTerms, "ID", "Code");
                ViewData["D"] = new SelectList(_context.CashDiscounts, "ID", "CodeName");
                return View(business);
            }


            if (ModelState.IsValid)
            {
                try
                {
                    if (business != null)
                    {
                        if (business.ID == 0)
                        {
                            if (_context.SystemLicenses.Any(w => w.Edition == SystemEdition.Basic))
                            {
                                ValidateSummaryBasic(business);
                            }
                            else
                            {
                                ValidateSummary(business);
                            }

                            ModelMessage msg = new();
                            if (ModelState.IsValid)
                            {
                                _context.BusinessPartners.Update(business);
                                _context.SaveChanges();
                                ModelState.AddModelError("success", "Vendor created succussfully!");
                                msg.Approve();
                                return Ok(msg.Bind(ModelState));
                            }
                            return Ok(msg.Bind(ModelState));
                        }
                    }

                }
                catch (Exception)
                {
                    ViewBag.Error = "This code already exist !";
                    ViewData["B"] = _bPartner.SelectPricelists(business.PriceListID);
                    ViewData["G1"] = _bPartner.SelectGroup1(business.Group2ID, "Vendor");
                    ViewData["G2"] = _bPartner.SelectGroup2(business.Group2ID, "Vendor");
                    ViewData["P"] = new SelectList(_context.PaymentTerms, "ID", "Code");
                    ViewData["D"] = new SelectList(_context.CashDiscounts, "ID", "CodeName");
                    return View(business);
                }
                return Ok(new { url = "/BusinessPartner/Vendor" });
            }
            else
            {
                if (business.PriceListID == 0)
                {
                    ViewBag.pricelist = "Please select price list !";
                    ViewData["B"] = _bPartner.SelectPricelists(business.PriceListID);
                    ViewData["G1"] = _bPartner.SelectGroup1(business.Group2ID, "Vendor");
                    ViewData["G2"] = _bPartner.SelectGroup2(business.Group2ID, "Vendor");
                    ViewData["P"] = new SelectList(_context.PaymentTerms, "ID", "Code");
                    ViewData["D"] = new SelectList(_context.CashDiscounts, "ID", "CodeName");
                }
                if (business.Type == "0")
                {
                    ViewBag.Errortype = "Please select type !";
                    ViewData["B"] = _bPartner.SelectPricelists(0);
                    ViewData["G1"] = _bPartner.SelectGroup1(business.Group2ID, "Vendor");
                    ViewData["G2"] = _bPartner.SelectGroup2(business.Group2ID, "Vendor");
                    ViewData["P"] = new SelectList(_context.PaymentTerms, "ID", "Code");
                    ViewData["D"] = new SelectList(_context.CashDiscounts, "ID", "CodeName");
                    return View(business);
                }
            }
            ViewData["B"] = _bPartner.SelectPricelists(business.PriceListID);
            ViewData["G1"] = _bPartner.SelectGroup1(business.Group2ID, "Vendor");
            ViewData["G2"] = _bPartner.SelectGroup2(business.Group2ID, "Vendor");
            ViewData["P"] = new SelectList(_context.PaymentTerms, "ID", "Code");
            ViewData["D"] = new SelectList(_context.CashDiscounts, "ID", "CodeName");
            return View(business);
        }

        //end create vendor
        private List<SetupContractName> Nameselect()
        {
            var data = _context.SetupContractNames.ToList();
            // as add
            data.Insert(0, new SetupContractName
            {
                ContractName = "-- Select --",
                ID = 0,
            });
            return data;
        }
        //=================get attchment of service contract ======
        public IActionResult GetAttchmentService(int id)
        {
            var data = (from ser in _context.ServiceContracts.Where(x => x.ID == id)
                        join bp in _context.BusinessPartners on ser.CusID equals bp.ID
                        select new ServiceContract
                        {
                            ID = ser.ID,
                            AttchmentFiles = (from att in _context.AttchmentFiles.Where(x => x.ServiceContractID == ser.ID)
                                              select new AttchmentFile
                                              {
                                                  ID = att.ID,
                                                  TargetPath = att.TargetPath,
                                                  FileName = att.FileName,
                                                  AttachmentDate = att.AttachmentDate
                                              }
                                            ).ToList() ?? new List<AttchmentFile>()
                        }
                      ).ToList();
            return Ok(data);


        }
        //update customer
        [HttpGet]
        [Privilege("BP001")]
        public IActionResult UpdateCustomer(int? id)
        {
            ViewBag.Customer = "highlight";
            var _partner = _bPartner.GetCustomerData(Convert.ToInt32(id));
            if (_partner.Delete == true)
            {
                return NotFound();
            }
            if (_partner != null)
            {
                List<ContactPerson> con = new();
                con.AddRange(_partner.ContactPeople);
                for (int i = 1; i <= 5; i++)
                {
                    con.Add(_bPartner.Contact(_partner.ID));
                }
                _partner.ContactPeople = con;

                List<BPBranch> br = new();
                br.AddRange(_partner.BPBranches);
                for (int i = 1; i <= 5; i++)
                {
                    br.Add(Tbbranch(_partner.ID));
                }
                _partner.BPBranches = br;
            }
            var _glAccID = _context.BusinessPartners.Find(id).GLAccID;
            var _glAcc = _context.GLAccounts.Find(_glAccID);
            List<AutoMobileViewModel> amViews = new();
            foreach (var item in _partner.AutoMobile)
            {
                amViews.Add(new AutoMobileViewModel
                {
                    AutoMID = item.AutoMID,
                    BusinessPartnerID = item.BusinessPartnerID,
                    Plate = item.Plate,
                    Frame = item.Frame,
                    Engine = item.Engine,
                    VehiTypes = _context.AutoTypes.Select(t => new SelectListItem
                    {
                        Value = t.TypeID.ToString(),
                        Text = t.TypeName,
                        Selected = item.TypeID == t.TypeID
                    }).ToList(),
                    VehiBrands = _context.AutoBrands.Select(t => new SelectListItem
                    {
                        Value = t.BrandID.ToString(),
                        Text = t.BrandName,
                        Selected = item.BrandID == t.BrandID
                    }).ToList(),
                    VehiModels = _context.AutoModels.Select(t => new SelectListItem
                    {
                        Value = t.ModelID.ToString(),
                        Text = t.ModelName,
                        Selected = item.ModelID == t.ModelID
                    }).ToList(),
                    Year = item.Year,
                    VehiColors = _context.AutoColors.Select(t => new SelectListItem
                    {
                        Value = t.ColorID.ToString(),
                        Text = t.ColorName,
                        Selected = item.ColorID == t.ColorID
                    }).ToList(),
                    Deleted = item.Deleted,
                    KeyID = item.KeyID,
                    BrandID = item.BrandID,
                    ColorID = item.ColorID,
                    ModelID = item.ModelID,
                    TypeID = item.TypeID
                });
            }
            BusinessPartnerViewModel bpViewModel = new()
            {
                BusinessPartner = _partner ?? new BusinessPartner(),
                AutoMobiles = amViews,
                PriceLists = new SelectList(_context.PriceLists, "ID", "Name"),
                GLAccID = _glAcc == null ? 0 : _glAcc.ID,
                GLAccName = _glAcc == null ? "" : _glAcc.Name,
                GLAccCode = _glAcc == null ? "" : _glAcc.Code,
                PaymentTerms = PaymentTermsDisplay.Select(p => new SelectListItem
                {
                    Value = p.Key.ToString(),
                    Text = p.Value
                }).ToList(),

                DueDate = DueDateDisplay.Select(d => new SelectListItem
                {
                    Value = d.Key.ToString(),
                    Text = d.Value
                }).ToList(),

                OpenIncomingPayment = IncomingPaymentdisplay.Select(d => new SelectListItem
                {
                    Value = d.Key.ToString(),
                    Text = d.Value
                }).ToList(),
                CreditMethod = CreditMethodDisplay.Select(i => new SelectListItem
                {
                    Value = i.Key.ToString(),
                    Text = i.Value
                }).ToList()
            };

            var service = _bPartner.GetServiceContract(GetCompany().ID, Convert.ToInt32(id));
            foreach (var i in service)
            {
                var conTractBilling = _partner.ContractBilings.FirstOrDefault(c => c.SeriesDID == i.SeriesDID);
                if (conTractBilling == null) _partner.ContractBilings.Add(i);
            }

            ViewData["Types"] = new SelectList(_context.AutoTypes.Where(c => c.Active), "TypeID", "TypeName");
            ViewData["Brands"] = new SelectList(_context.AutoBrands.Where(c => c.Active), "BrandID", "BrandName");
            ViewData["Models"] = new SelectList(_context.AutoModels.Where(c => c.Active), "ModelID", "ModelName");
            ViewData["Colors"] = new SelectList(_context.AutoColors.Where(c => c.Active), "ColorID", "ColorName");

            ViewData["B"] = _bPartner.SelectPricelists(_partner.PriceListID);
            ViewData["C"] = _bPartner.SelectListTerritory(_partner.CustomerSourceID);
            ViewData["G1"] = _bPartner.SelectGroup1(_partner.Group1ID, "Customer");
            ViewData["G2"] = _bPartner.SelectGroup2(_partner.Group2ID, "Customer");
            ViewData["P"] = new SelectList(_context.PaymentTerms, "ID", "Code");
            ViewData["D"] = new SelectList(_context.CashDiscounts, "ID", "CodeName");
            ViewData["Totalbalance"] = _context.IncomingPaymentCustomers.Where(s => s.CustomerID == Convert.ToInt32(id) && s.Status == "open").Sum(x => x.TotalPayment);
            return View(bpViewModel);
        }
        //===========Setup Contract Name =======
        private List<SetupContractName> Selectdescription()
        {
            var data = _context.SetupContractNames.ToList();
            // as add
            data.Insert(0, new SetupContractName
            {
                ContractName = "-- Select --",
                ID = 0,
            });
            return data;
        }

        [HttpGet]
        public IActionResult GetSetupContractNameDefalutTable()
        {
            List<SetupContractName> setupcontractname = new();
            var data = _context.SetupContractNames.ToList();
            setupcontractname.AddRange(data);
            for (var i = 1; i <= 5; i++)
            {
                var contractname = new SetupContractName
                {
                    LineID = DateTime.Now.Ticks.ToString(),
                    ID = 0,
                    ContractName = ""
                };
                setupcontractname.Add(contractname);
            }
            return Ok(setupcontractname);

        }
        [HttpPost]
        public IActionResult InsertSetupContractName(List<SetupContractName> setupcontractname)
        {

            ModelMessage msg = new();
            if (ModelState.IsValid)
            {
                _context.SetupContractNames.UpdateRange(setupcontractname);
                _context.SaveChanges();
                ModelState.AddModelError("success", "setupcontractname created succussfully!");
                msg.Approve();
            }
            var data = _context.SetupContractNames.ToList();
            return Ok(new { Model = msg.Bind(ModelState), Data = data });
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
        //end update customer

        //save change customer
        [HttpPost]
        [RequestFormLimits(ValueCountLimit = int.MaxValue, Order = 1)]
        [ValidateAntiForgeryToken(Order = 2)]
        public async Task<IActionResult> UpdateCustomer(BusinessPartnerViewModel form)
        {
            form.BusinessPartner.ContactPeople = form.BusinessPartner.ContactPeople.Where(i => !string.IsNullOrEmpty(i.ContactID)).ToList();
            form.BusinessPartner.BPBranches = form.BusinessPartner.BPBranches.Where(i => !string.IsNullOrEmpty(i.Name)).ToList();
            form.BusinessPartner.ContractBilings = form.BusinessPartner.ContractBilings?.Where(i => !string.IsNullOrEmpty(i.DocumentNo)).ToList();
            if (ModelState.IsValid)
            {
                await _bPartner.SaveCustomerAsync(form.BusinessPartner, ModelState);
                _context.SaveChanges();
                return RedirectToAction(nameof(Customer));
            }
            return View(form);
        }
        //end save change customer

        //update vendor
        [HttpGet]
        [Privilege("BP002")]
        public IActionResult UpdateVendor(int? id)
        {
            ViewBag.Vendor = "highlight";
            var _partner = _context.BusinessPartners.Include(c => c.AutoMobile).First(c => c.ID == id);
            if (_partner.Delete == true)
            {
                return NotFound();
            }
            var _glAccID = _context.BusinessPartners.Find(id).GLAccID;
            var _glAcc = _context.GLAccounts.Find(_glAccID);
            var _saleempid = _context.BusinessPartners.Find(id).SaleEMID;
            var _saleemp = _context.Employees.Find(_saleempid);
            List<AutoMobileViewModel> amViews = new();
            BusinessPartnerViewModel bpViewModel = new()
            {
                BusinessPartner = _partner ?? new BusinessPartner(),
                AutoMobiles = amViews,
                PriceLists = new SelectList(_context.PriceLists, "ID", "Name"),
                GLAccID = _glAcc == null ? 0 : _glAcc.ID,
                GLAccName = _glAcc == null ? "" : _glAcc.Name,
                GLAccCode = _glAcc == null ? "" : _glAcc.Code,
                SaleEmpID = _saleemp == null ? 0 : _saleemp.ID,
                SaleEmpName = _saleemp == null ? "" : _saleemp.Name
            };


            ViewData["Types"] = new SelectList(_context.AutoTypes.Where(c => c.Active), "TypeID", "TypeName");
            ViewData["Brands"] = new SelectList(_context.AutoBrands.Where(c => c.Active), "BrandID", "BrandName");
            ViewData["Models"] = new SelectList(_context.AutoModels.Where(c => c.Active), "ModelID", "ModelName");
            ViewData["Colors"] = new SelectList(_context.AutoColors.Where(c => c.Active), "ColorID", "ColorName");

            ViewData["B"] = _bPartner.SelectPricelists(_partner.PriceListID);
            ViewData["G"] = _bPartner.SelectGroup1(_partner.Group1ID, "Vendor");
            ViewData["G1"] = _bPartner.SelectGroup1(_partner.Group1ID, "Vendor");
            ViewData["G2"] = _bPartner.SelectGroup2(_partner.Group2ID, "Vendor");
            ViewData["P"] = new SelectList(_context.PaymentTerms, "ID", "Code");
            ViewData["D"] = new SelectList(_context.CashDiscounts, "ID", "CodeName");
            return View(bpViewModel);
        }
        //end update vendor

        //save change vendor
        [HttpPost]
        [RequestFormLimits(ValueCountLimit = int.MaxValue, Order = 1)]
        [ValidateAntiForgeryToken(Order = 2)]
        public IActionResult UpdateVendor(BusinessPartnerViewModel business)
        {
            ViewBag.Main = "Vendor";
            if (ModelState.IsValid)
            {
                business.BusinessPartner.Type = "Vendor";
                _context.BusinessPartners.Update(business.BusinessPartner);
                _context.SaveChanges();
            }
            else
            {
                var _glAccID = business.BusinessPartner.GLAccID;
                var _glAcc = _context.GLAccounts.Find(_glAccID);
                var _saleempid = business.BusinessPartner.SaleEMID;
                var _saleemp = _context.Employees.Find(_saleempid);
                BusinessPartnerViewModel bpViewModel = new()
                {
                    BusinessPartner = business.BusinessPartner ?? new BusinessPartner(),
                    PriceLists = new SelectList(_context.PriceLists, "ID", "Name"),
                    GLAccID = _glAcc == null ? 0 : _glAcc.ID,
                    GLAccName = _glAcc == null ? "" : _glAcc.Name,
                    GLAccCode = _glAcc == null ? "" : _glAcc.Code,
                    SaleEmpID = _saleemp == null ? 0 : _saleemp.ID,
                    SaleEmpName = _saleemp == null ? "" : _saleemp.Name
                };
                ViewData["B"] = _bPartner.SelectPricelists(business.BusinessPartner.PriceListID);
                ViewData["G"] = _bPartner.SelectGroup1(business.BusinessPartner.Group1ID, "Vendor");
                ViewData["G1"] = _bPartner.SelectGroup1(business.BusinessPartner.Group1ID, "Vendor");
                ViewData["G2"] = _bPartner.SelectGroup2(business.BusinessPartner.Group2ID, "Vendor");
                ViewData["P"] = new SelectList(_context.PaymentTerms, "ID", "Code");
                ViewData["D"] = new SelectList(_context.CashDiscounts, "ID", "CodeName");
                return View(bpViewModel);
            }
            return RedirectToAction(nameof(Vendor));
        }
        //end save change vendor
        [HttpPost]
        public IActionResult GetBusDetail(AutoMobile data)
        {
            AutoMobileViewModel _detail = new()
            {
                KeyID = data.KeyID,
                Plate = data.Plate,
                Frame = data.Frame,
                Engine = data.Engine,
                Year = data.Year,
                TypeID = data.TypeID,
                BrandID = data.BrandID,
                ModelID = data.ModelID,
                ColorID = data.ColorID,
                VehiTypes = _context.AutoTypes.Select(t => new SelectListItem
                {
                    Value = t.TypeID.ToString(),
                    Text = t.TypeName,
                    Selected = t.TypeID == data.TypeID
                }).ToList(),
                VehiBrands = _context.AutoBrands.Select(t => new SelectListItem
                {
                    Value = t.BrandID.ToString(),
                    Text = t.BrandName,
                    Selected = t.BrandID == data.BrandID
                }).ToList(),
                VehiModels = _context.AutoModels.Select(t => new SelectListItem
                {
                    Value = t.ModelID.ToString(),
                    Text = t.ModelName,
                    Selected = t.ModelID == data.ModelID
                }).ToList(),
                VehiColors = _context.AutoColors.Select(t => new SelectListItem
                {
                    Value = t.ColorID.ToString(),
                    Text = t.ColorName,
                    Selected = t.ColorID == data.ColorID
                }).ToList()
            };
            return Ok(_detail);
        }
        [HttpGet]
        public IActionResult GetVehicleComponents()
        {
            return GetBusDetail(new AutoMobile());
        }
        [HttpPost]
        public async Task<IActionResult> Delete(int? id)
        {
            await _bPartner.Delete(id);
            return Ok(new { url = "/BusinessPartner/Index" });
        }

        //Model
        public IActionResult GetAutoType()
        {
            return Ok(_context.AutoTypes);
        }
        [HttpPost]
        public IActionResult UpdateAutoType(IEnumerable<AutoType> data)
        {
            var items = data.GroupBy(s => s.TypeName.ToLower().Replace(" ", ""))
                            .Where(g => g.Count() > 1)
                            .Select(g => g.Key).ToList();

            ModelMessage errors = new(ModelState);
            if (items.Count > 0)
            {
                foreach (var error in items)
                {
                    ModelState.AddModelError(error, string.Format("existed -> "));
                }
            }

            if (ModelState.IsValid)
            {
                _context.AutoTypes.UpdateRange(data);
                _context.SaveChanges();
            }

            return Ok(new
            {
                Data = _context.AutoTypes.Where(a => a.Active == true),
                Errors = errors.Bind(ModelState)
            });
        }

        public IActionResult GetAutoBrand()
        {
            return Ok(_context.AutoBrands);
        }
        public IActionResult UpdateAutoBrand(IEnumerable<AutoBrand> data)
        {
            var items = data.GroupBy(s => s.BrandName.ToLower().Replace(" ", ""))
                            .Where(g => g.Count() > 1)
                            .Select(g => g.Key).ToList();

            ModelMessage errors = new(ModelState);
            if (items.Count > 0)
            {
                foreach (var error in items)
                {
                    ModelState.AddModelError(error, string.Format("existed -> "));
                }
            }

            if (ModelState.IsValid)
            {
                _context.AutoBrands.UpdateRange(data);
                _context.SaveChanges();
            }

            return Ok(new
            {
                Data = _context.AutoBrands.Where(a => a.Active == true),
                Errors = errors.Bind(ModelState)
            });
        }

        public IActionResult GetAutoModel()
        {
            return Ok(_context.AutoModels);
        }

        public IActionResult UpdateAutoModel(IEnumerable<AutoModel> data)
        {
            var items = data.GroupBy(s => s.ModelName.ToLower().Replace(" ", ""))
                            .Where(g => g.Count() > 1)
                            .Select(g => g.Key).ToList();

            ModelMessage errors = new(ModelState);
            if (items.Count > 0)
            {
                foreach (var error in items)
                {
                    ModelState.AddModelError(error, string.Format("existed -> "));
                }
            }

            if (ModelState.IsValid)
            {
                _context.AutoModels.UpdateRange(data);
                _context.SaveChanges();
            }

            return Ok(new
            {
                Data = _context.AutoModels.Where(a => a.Active == true),
                Errors = errors.Bind(ModelState)
            });
        }

        public IActionResult GetAutoColor()
        {
            return Ok(_context.AutoColors);
        }

        public IActionResult UpdateAutoColor(IEnumerable<AutoColor> data)
        {
            var items = data.GroupBy(s => s.ColorName.ToLower().Replace(" ", ""))
                            .Where(g => g.Count() > 1)
                            .Select(g => g.Key).ToList();

            ModelMessage errors = new(ModelState);
            if (items.Count > 0)
            {
                foreach (var error in items)
                {
                    ModelState.AddModelError(error, string.Format("existed -> "));
                }
            }

            if (ModelState.IsValid)
            {
                _context.AutoColors.UpdateRange(data);
                _context.SaveChanges();
            }

            return Ok(new
            {
                Data = _context.AutoColors.Where(a => a.Active == true),
                Errors = errors.Bind(ModelState)
            });
        }

        [HttpGet]
        [Route("/bussinesspartner/glcontrolaccount")]
        public IActionResult GetControlAccount()
        {
            var controlAcc = _context.GLAccounts.Where(i => i.IsControlAccount == true).ToList();
            return Ok(controlAcc);
        }
        //================ContactPerson================
        public IActionResult GetDefultContactPerson()
        {
            List<ContactPerson> contactPeople = _bPartner.GetDefultContactPerson();
            return Ok(contactPeople);

        }
        //============Branch============
        private List<Company> Company()
        {
            var data = _context.Company.ToList();
            // as add
            data.Insert(0, new Company
            {
                Name = "-- Select --",
                ID = 0,
            });
            return data;
        }
        public IActionResult GetDefultBranch()
        {
            List<BPBranch> bPBranches = new();
            for (var i = 1; i <= 5; i++)
            {
                var bpbranch = new BPBranch
                {
                    LineID = DateTime.Now.Ticks.ToString(),
                    ID = 0,
                    Name = "",
                    Tel = "",
                    Address = "",
                    Email = "",
                    BranchCotactPerson = "",
                    ContactTel = "",
                    ContactEmail = "",
                    GPSLink = "",
                    SetDefualt = false
                };
                bPBranches.Add(bpbranch);
            }
            return Ok(bPBranches);

        }
        [HttpGet]
        public IActionResult GetTable()
        {
            List<ContactPerson> contacts = new();
            for (var i = 1; i <= 5; i++)
            {
                contacts.Add(_bPartner.Contact());
            }
            return Ok(contacts);

        }


        private static BPBranch Tbbranch(int brid = 0)
        {
            BPBranch bpbranch = new()
            {
                LineID = DateTime.Now.Ticks.ToString(),
                ID = 0,
                BusinessPartnerID = brid,
                Name = "",
                Tel = "",
                Address = "",
                Email = "",
                BranchCotactPerson = "",
                ContactTel = "",
                ContactEmail = "",
                GPSLink = "",
                SetDefualt = false
            };
            return bpbranch;
        }
        #region  ValidateSummaryBasic
        private void ValidateSummaryBasic(BusinessPartner businessPartner)
        {
            if (businessPartner.Code == "")
            {
                ModelState.AddModelError("Code", "Please input Bussiness Partner Code");
            }
            if (businessPartner.Name == "")
            {
                ModelState.AddModelError("Name", "Please input Bussiness Partner Name");
            }
            if (businessPartner.Type == "")
            {
                ModelState.AddModelError("Type", "Please input Bussiness Partner Type");
            }

            if (businessPartner.PriceListID == 0)
            {
                ModelState.AddModelError("PriceListID", "Please select Bussiness Partner PriceListID");
            }

        }
        #endregion ValidateSummaryBasic

        private void ValidateSummary(BusinessPartner businessPartner)
        {
            //List<ContactPerson> contacts = new();
            if (businessPartner.Code == "")
            {
                ModelState.AddModelError("Code", "Please input Bussiness Partner Code");
            }
            //if (businessPartner.SaleEMID == 0)
            //{
            //    ModelState.AddModelError("SaleEMID", "Please choose sale employee");

            //}

            if (businessPartner.Name == "")
            {
                ModelState.AddModelError("Name", "Please input Bussiness Partner Name");
            }
            if (businessPartner.Type == "")
            {
                ModelState.AddModelError("Type", "Please input Bussiness Partner Type");
            }

            if (businessPartner.PriceListID == 0)
            {
                ModelState.AddModelError("PriceListID", "Please select Bussiness Partner PriceListID");
            }
            if (businessPartner.GLAccID == 0)
            {
                ModelState.AddModelError("GLAccID", "Please Choose any Bussiness Partner GLAcount");
            }

        }

        [HttpPost]
        public IActionResult SubmmitDataBP(string _data)
        {
            BusinessPartner data = JsonConvert.DeserializeObject<BusinessPartner>(_data, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });
            data.Type = "Customer";
            data.ContactPeople = data.ContactPeople.Where(i => i.ContactID != "").ToList();
            data.BPBranches = data.BPBranches.Where(i => i.Name != "").ToList();
            if (_context.SystemLicenses.Any(w => w.Edition == SystemEdition.Basic))
            {
                ValidateSummaryBasic(data);
            }
            else
            {
                ValidateSummary(data);
            }

            ModelMessage msg = new();
            if (ModelState.IsValid)
            {
                _context.BusinessPartners.Update(data);
                _context.SaveChanges();
                ModelState.AddModelError("success", "BussinesssPartners created succussfully!");
                msg.Approve();
                return Ok(msg.Bind(ModelState));
            }
            return Ok(msg.Bind(ModelState));
            //return Ok(new { url = "/BusinessPartner/Customer" });
        }
        [HttpPost]
        public async Task<IActionResult> CreateUpdateG1(GroupCustomer1 group1)
        {
            await _bPartner.AddOrEdit(group1);
            return Ok();
        }
        [HttpGet]
        public async Task<IActionResult> GetGroup1(string type)
        {
            var list = await _bPartner.GetGroup1(type);
            return Ok(list);
        }
        [HttpPost]
        public async Task<IActionResult> CreateUpdateG2(GroupCustomer2 group2)
        {
            await _bPartner.AddGroup2OrEdit(group2);
            return Ok();
        }
        [HttpGet]
        public IActionResult GetGroup2(int value, string type)
        {
            if (value == 0)
            {
                var list = _context.GroupCustomer2s.Where(s => s.Delete == false && s.Type == type).ToList();
                return Ok(list);
            }
            else
            {
                var list = _context.GroupCustomer2s.Where(s => s.Group1ID == value && s.Type == type).ToList();
                return Ok(list);
            }
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
        public IActionResult CreateDefualtType()
        {
            List<EmployeeType> emtype = new();

            var list = (from em in _context.EmployeeTypes.Where(s => s.Delete == false)
                        select new EmployeeType
                        {
                            ID = em.ID,
                            LineID = DateTime.Now.Ticks.ToString() + em.ID,
                            Type = em.Type,
                            Status = false,
                        }).OrderBy(s => s.Type).ToList();


            return Ok(list);
        }
        [HttpPost]
        public IActionResult SaveEMType(EmployeeType data)
        {

            ModelMessage msg = new();

            if (string.IsNullOrWhiteSpace(data.Type))
            {
                ModelState.AddModelError("", "please input Type ...!");
            }

            if (ModelState.IsValid)
            {
                _context.Update(data);
                _context.SaveChanges();
                ModelState.AddModelError("success", "Item save successfully.");
                msg.Approve();
            }
            var list = (from em in _context.EmployeeTypes.Where(s => s.Delete == false)
                        select new EmployeeType
                        {
                            ID = em.ID,
                            LineID = DateTime.Now.Ticks.ToString() + em.ID,
                            Type = em.Type,
                            Status = false,
                        }).OrderBy(s => s.Type).ToList();
            return Ok(new { Model = msg.Bind(ModelState), Data = list });

        }
        //=======================Paymetn Term=========================
        //CreateTerms
        [HttpPost]
        public IActionResult InsertPayment(PaymentTerms payment)
        {

            ModelMessage msg = new();
            var payments = _context.PaymentTerms.FirstOrDefault(c => c.Code == payment.Code);
            if (payments != null)
            {
                ModelState.AddModelError("Code", "This code already exist !");
            }
            if (ModelState.IsValid)
            {
                _context.PaymentTerms.Update(payment);
                _context.SaveChanges();
                var list = _context.PaymentTerms.ToList();
                return Ok(new { Error = false, Data = list });
            }
            return Ok(new { Error = true, Model = msg.Bind(ModelState) });

        }
        [HttpGet]
        public IActionResult GetPayment(int id)
        {
            var list = (from p in _context.PaymentTerms
                        join i in _context.Instaillments on p.InstaillmentID equals i.ID
                        join d in _context.PriceLists on p.PriceListID equals d.ID
                        join c in _context.CashDiscounts on p.CashDiscountID equals c.ID
                        select new
                        {
                            p.ID,
                            p.Code,
                            p.Days,
                            p.Months,
                            p.DueDate,
                            p.StartFrom,
                            p.TolerenceDay,
                            p.OpenIncomingPayment,
                            InterestOnRecive = p.InterestOnReceiVables,
                            p.TotalDiscount,
                            p.CommitLimit,
                            p.MaxCredit,
                            PriceList = d.ID,
                            CodeInstaill = i.ID,
                            InstailMent = i.NoOfInstaillment,
                            CashDiscount = c.ID
                        }).FirstOrDefault(e => e.ID == id);

            return Ok(list);
        }
        [HttpPost]
        public IActionResult InsertInstaillment(Instaillment instaillment)
        {
            _context.Instaillments.Update(instaillment);
            _context.SaveChanges();
            return Ok();
        }
        /// <summary>
        /// Create Default InstallmentDetail
        /// </summary>
        /// <param name="amount">Amount : Number of rows</param>
        /// <returns>List Objects Of Installment but empty</returns>
        [Route("/bp/createdefaultinstalldetail")]
        public IActionResult CreateDefaultInstallmentDetail(int amount)
        {
            List<InstallmentDetailViewModel> installmentDetails = new();
            for (var i = 0; i < amount; i++)
            {
                installmentDetails.Add(new InstallmentDetailViewModel
                {
                    UnitID = DateTime.Now.Ticks.ToString(),
                    Day = 0,
                    InstallmentID = 0,
                    Months = 0,
                    Percent = 100 / Convert.ToDecimal(amount),
                });
            }
            return Ok(installmentDetails);
        }

        public IActionResult InsertCashDiscount(CashDiscount cashDiscount)
        {
            _context.CashDiscounts.Update(cashDiscount);
            _context.SaveChanges();
            return Ok();
        }

        public IActionResult GetInstaillment(int id)
        {
            var getinstaillment = _context.Instaillments.Find(id) ?? new Instaillment();
            return Ok(getinstaillment);
        }
        public IActionResult GetInstaillmentLastest()
        {
            var getinstaillment = _context.Instaillments.OrderByDescending(id => id.ID).FirstOrDefault();
            return Ok(getinstaillment);
        }
        public IActionResult GetCashDiscountLastest()
        {
            var ins = _context.CashDiscounts.OrderByDescending(d => d.ID).FirstOrDefault();
            return Ok(ins);
        }
        public IActionResult GetPaymentCodeList()
        {
            var ins = _context.PaymentTerms.OrderByDescending(d => d.ID).FirstOrDefault();
            return Ok(ins);
        }
        public IActionResult GetPaymentEditCode()
        {
            var data = _context.PaymentTerms.ToList();
            return Ok(data);
        }
        [HttpGet]
        public IActionResult GetCashDiscount(int id)
        {
            var discount = _context.CashDiscounts.Find(id) ?? new CashDiscount();
            return Ok(discount);
        }
        [HttpGet]
        public IActionResult GetingDiscount(int id)
        {
            var getingdiscount = _context.CashDiscounts.Find(id);
            return Ok(getingdiscount);
        }

        public IActionResult FetchPaymentTerm(int paymentTermId = 0)
        {
            var list = (from p in _context.PaymentTerms.Where(i => i.ID == paymentTermId)
                        let i = _context.Instaillments.Where(x => x.ID == p.InstaillmentID).FirstOrDefault() ?? new Instaillment()
                        select new
                        {
                            p.ID,
                            p.Code,
                            p.Months,
                            p.Days,
                            p.StartFrom,
                            p.DueDate,
                            p.OpenIncomingPayment,
                            p.TolerenceDay,
                            p.TotalDiscount,
                            p.InterestOnReceiVables,
                            p.MaxCredit,
                            p.CommitLimit,
                            p.PriceListID,
                            p.InstaillmentID,
                            p.CashDiscountID,
                            InstaillmentName = i.NoOfInstaillment
                        }).FirstOrDefault();
            //var paymentTerm = _context.PaymentTerms.Find(paymentTermId);
            if (list == null)
            {
                return Ok(new PaymentTerms());
            }
            return Ok(list);
        }

        public IActionResult FetchInstaillment(int instaillmentId = 0)
        {
            var list = (from i in _context.Instaillments.Where(i => i.ID == instaillmentId)
                            //join id in _context.InstallmentDetails on i.ID equals id.InstaillmentID
                        let id = _context.InstaillmentDetails.Where(ind => ind.InstaillmentID == i.ID).ToList()
                        select new
                        {
                            i.ID,
                            i.NoOfInstaillment,
                            i.ApplyTax,
                            i.UpdateTax,
                            //Month = i.Month,
                            //Day = i.Day,
                            //Percent = i.Percent,
                            InstaillmentDetails = id,
                            i.CreditMethod
                        }).ToList();
            if (list == null)
            {
                return Ok(new Instaillment());
            }
            return Ok(list);
        }

        //[HttpPost]
        //public IActionResult InsertEditinstallment(Instaillment instaillments)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context.Update(instaillments);
        //        _context.SaveChanges();
        //        return Ok();
        //    }
        //    return Ok();
        //}
        [HttpPost]
        public IActionResult InsertEditinstallment(Instaillment obj)
        {
            if (ModelState.IsValid)
            {
                _context.Update(obj);
                _context.SaveChanges();
                return Ok();
            }
            return Ok();
        }

        public IActionResult FetchDiscount(int discountId = 0)
        {
            var discount = _context.CashDiscounts.Find(discountId);
            if (discount == null)
            {
                return Ok(new CashDiscount());
            }
            return Ok(discount);
        }
        [HttpPost]
        public IActionResult InsertEditPayment(PaymentTerms paymentTerms)
        {
            if (ModelState.IsValid)
            {
                _context.Update(paymentTerms);
                _context.SaveChanges();
                return Ok();
            }
            var list = _context.PaymentTerms.ToList();
            return Ok(list);
        }
        [HttpPost]
        public IActionResult InsertEditdiscount(CashDiscount discounts)
        {
            if (ModelState.IsValid)
            {
                _context.Update(discounts);
                _context.SaveChanges();
                return Ok();
            }
            return Ok();
        }

        [HttpPost]
        public IActionResult InserEditDataDis(CashDiscount editdiscounts)
        {
            if (ModelState.IsValid)
            {
                _context.Update(editdiscounts);
                _context.SaveChanges();
                return Ok();
            }
            return Ok();
        }
        public IActionResult GetDataDis(int getdata)
        {
            var data = _context.CashDiscounts.Where(x => x.ID == getdata).ToList();
            return Ok(data);
        }
        //end method of payment term

        //==========Sale History=============//
        public IActionResult SaleHistory()
        {
            ViewBag.SaleHistory = "highlight";
            return View();
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
        public IActionResult GetBranch()
        {
            var list = _context.Branches.Where(x => x.Delete == false).ToList();
            return Ok(list);
        }
        [HttpGet]
        public IActionResult GetHistoryPurchase(int id, string? datefrom = "", string? dateto = "")
        {
            var list = datefrom == "" | datefrom == null ? _context.Purchase_APs.Where(s => s.VendorID == id).Take(20).ToList()
                                                        : _context.Purchase_APs.Where(s => s.VendorID == id && s.PostingDate >= Convert.ToDateTime(datefrom) && s.PostingDate <= Convert.ToDateTime(dateto)).ToList();
            var purchaseAP = (from purap in list
                              join purardetail in _context.PurchaseDetails on purap.PurchaseAPID equals purardetail.PurchaseAPID
                              join doc in _context.DocumentTypes on purap.DocumentTypeID equals doc.ID
                              join item in _context.ItemMasterDatas on purardetail.ItemID equals item.ID
                              join uom in _context.UnitofMeasures on purardetail.UomID equals uom.ID
                              join curency in _context.Currency on purap.PurCurrencyID equals curency.ID
                              let plCur = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == curency.ID) ?? new Display()

                              group new { purap, purardetail, doc, item, uom, curency, plCur } by new { purardetail.PurchaseDetailAPID } into datas
                              let data = datas.FirstOrDefault()

                              let sysCur = _context.Currency.FirstOrDefault(i => i.ID == GetCompany().SystemCurrencyID) ?? new Currency()
                              let lccCur = _context.Currency.FirstOrDefault(i => i.ID == GetCompany().LocalCurrencyID) ?? new Currency()
                              select new
                              {
                                  LineID = DateTime.Now.Ticks.ToString() + data.purardetail.PurchaseDetailAPID,
                                  ReceiptNmber = data.purap.InvoiceNo == "" || data.purap.InvoiceNo == null ? data.purap.Number : data.purap.InvoiceNo,
                                  DouType = data.doc.Code,
                                  DateOut = data.purap.PostingDate.ToString("dd/MM/yyyy"),


                                  ItemName = data.item.KhmerName == "" || data.item.KhmerName == null ? data.item.EnglishName : data.item.KhmerName,
                                  Uom = data.uom.Name,
                                  Price = $"{data.curency.Description} {_fncModule.ToCurrency(data.purardetail.PurchasPrice, data.plCur.Amounts)}",
                                  Qty = data.purardetail.Qty,
                                  Total = $"{data.curency.Description} {_fncModule.ToCurrency(data.purardetail.Total, data.plCur.Amounts)}",

                                  DiscountItem = $"{sysCur.Description} {_fncModule.ToCurrency(datas.Sum(s => s.purardetail.DiscountValue) * data.purap.PurRate, data.plCur.Amounts)}",
                                  DiscountTotal = $"{sysCur.Description} {_fncModule.ToCurrency(datas.Sum(s => s.purap.DiscountValue) * data.purap.PurRate, data.plCur.Amounts)}",
                                  GrandTotalLCC = $"{lccCur.Description} {_fncModule.ToCurrency(datas.Sum(s => s.purap.SubTotalAfterDisSys) * (decimal)data.purap.LocalSetRate, data.plCur.Amounts)}",
                                  GrandTotalSys = $"{sysCur.Description} {_fncModule.ToCurrency(datas.Sum(s => s.purap.SubTotalAfterDisSys), data.plCur.Amounts)}",
                                  VatCal = $"{sysCur.Description} {_fncModule.ToCurrency(datas.Sum(s => s.purap.TaxValue), data.plCur.Amounts)}",

                              }).OrderByDescending(s => s.DateOut).ToList();

            return Ok(purchaseAP);
        }
        public IActionResult GetingSaleHistory(int id, string? datefrom = "", string? dateto = "")
        {
            List<Receipt> receiptsFilter = datefrom == "" ? _context.Receipt.Where(w => w.CompanyID == GetCompany().ID && w.CustomerID == id).OrderByDescending(s => s.ReceiptID).Take(10).ToList()
                                                       : _context.Receipt.Where(w => w.CompanyID == GetCompany().ID && w.CustomerID == id && w.DateOut >= Convert.ToDateTime(datefrom) && w.DateOut <= Convert.ToDateTime(dateto)).ToList();

            List<SaleAR> saleARs = datefrom == "" ? _context.SaleARs.Where(w => w.CompanyID == GetCompany().ID && w.CusID == id).OrderByDescending(s => s.SARID).Take(10).ToList()
                                                  : _context.SaleARs.Where(w => w.CompanyID == GetCompany().ID && w.CusID == id && w.PostingDate >= Convert.ToDateTime(datefrom) && w.PostingDate <= Convert.ToDateTime(dateto)).ToList();



            // _context.SaleARs.Where(w => w.CompanyID == GetCompany().ID && w.CusID == id).Take(10).ToList();
            var receipt = datefrom == "" ? _context.Receipt.Where(w => w.CompanyID == GetCompany().ID && w.CustomerID == id).OrderByDescending(s => s.ReceiptID).Take(10).ToList()
                                           : _context.Receipt.Where(w => w.CompanyID == GetCompany().ID && w.CustomerID == id && w.DateOut >= Convert.ToDateTime(datefrom) && w.DateOut <= Convert.ToDateTime(dateto)).ToList();
            var receipts = (from r in receipt
                            join rd in _context.ReceiptDetail on r.ReceiptID equals rd.ReceiptID
                            join uom in _context.UnitofMeasures on rd.UomID equals uom.ID
                            join curr_pl in _context.Currency on r.PLCurrencyID equals curr_pl.ID
                            let doc = _context.DocumentTypes.FirstOrDefault(w => w.Code == "SP") ?? new DocumentType()
                            let plCur = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == curr_pl.ID) ?? new Display()
                            //let receiptDetails = _context.ReceiptDetail.Where(i => i.ReceiptID == r.ReceiptID).ToList()
                            select new SaleHistoryByCustomers
                            {
                                LineID = DateTime.Now.Ticks.ToString() + rd.ID,
                                DateOut = r.DateOut.ToString("dd/MM/yyyy"),
                                DouType = doc.Code,
                                GrandTotal = $"{curr_pl.Description} {_fncModule.ToCurrency(r.GrandTotal, plCur.Amounts)}",
                                ReceiptNmber = r.ReceiptNo,
                                ItemName = rd.KhmerName == "" || rd.KhmerName == null ? rd.EnglishName : rd.KhmerName,
                                Price = $"{rd.Currency} {_fncModule.ToCurrency(rd.UnitPrice, plCur.Amounts)}",
                                Qty = rd.Qty,
                                Total = $"{rd.Currency} {_fncModule.ToCurrency(rd.Total, plCur.Amounts)}",
                                Uom = uom.Name,
                                SaleID = r.ReceiptID,
                                SDiscountItemCal = 0,//receiptDetails.Sum(i => i.DiscountValue) * r.PLRate,
                                SDiscountTotalCal = r.DiscountValue * r.PLRate,
                                SGrandTotalLCCCal = r.GrandTotal_Sys * r.LocalSetRate,
                                SGrandTotalSysCal = r.GrandTotal_Sys,
                                SVatCal = r.TaxValue,
                            }).ToList();
            var saleArs = (from s in saleARs
                           join srd in _context.SaleARDetails on s.SARID equals srd.SARID
                           join doc in _context.DocumentTypes on s.DocTypeID equals doc.ID
                           join curr_pl in _context.Currency on s.SaleCurrencyID equals curr_pl.ID
                           // let sd = _context.SaleARDetails.Where(i => i.SARID == s.SARID).ToList()
                           let plCur = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == curr_pl.ID) ?? new Display()
                           select new SaleHistoryByCustomers
                           {
                               LineID = DateTime.Now.Ticks.ToString() + srd.SARDID,
                               DateOut = s.PostingDate.ToString("dd/MM/yyyy"),
                               DouType = doc.Code,
                               GrandTotal = $"{curr_pl.Description} {_fncModule.ToCurrency(s.TotalAmount, plCur.Amounts)}",
                               ReceiptNmber = s.InvoiceNumber,
                               ItemName = srd.ItemNameKH == "" || srd.ItemNameKH == null ? srd.ItemNameEN : srd.ItemNameKH,
                               Price = $"{curr_pl.Description} {_fncModule.ToCurrency(srd.UnitPrice, plCur.Amounts)}",
                               Qty = srd.Qty,
                               Total = $"{curr_pl.Description} {_fncModule.ToCurrency(srd.Total, plCur.Amounts)}",
                               Uom = srd.UomName,
                               SaleID = s.SARID,
                               SDiscountItemCal = 0,//sd.Sum(i => i.DisValue) * s.ExchangeRate,
                               SDiscountTotalCal = s.DisValue,
                               SGrandTotalLCCCal = s.TotalAmountSys * s.LocalSetRate,
                               SGrandTotalSysCal = s.TotalAmountSys,
                               SVatCal = s.VatValue,
                           }).ToList();


            List<SaleHistoryByCustomers> allSummarySale = new(receipts.Count + saleArs.Count);
            var receiptpos = (from re in receipt
                              let red = _context.ReceiptDetail.Where(s => s.ReceiptID == re.ReceiptID)
                              select new
                              {
                                  SDiscountItemCal = red.Sum(s => s.DiscountValue) * re.LocalSetRate,
                              }).ToList();
            var receiptar = (from sar in saleARs
                             let red = _context.SaleARDetails.Where(s => s.SARID == sar.SARID).ToList()
                             select new
                             {
                                 SDiscountItemCal = red.Sum(s => s.DisValue) * sar.ExchangeRate,
                             }).ToList();
            double SDiscountItemCal = receiptpos.Sum(s => s.SDiscountItemCal) + receiptar.Sum(s => s.SDiscountItemCal);
            double SDiscountTotalCal = receipt.Sum(s => s.DiscountValue * s.PLRate) + saleARs.Sum(s => s.DisValue);
            double SGrandTotalLCCCal = receipt.Sum(s => s.GrandTotal_Sys * s.LocalSetRate) + saleARs.Sum(s => s.TotalAmountSys * s.LocalSetRate);
            double SGrandTotalSysCal = receipt.Sum(s => s.GrandTotal_Sys) + saleARs.Sum(s => s.TotalAmountSys);
            double SVatCal = receipt.Sum(s => s.TaxValue) + saleARs.Sum(s => s.VatValue);
            allSummarySale.AddRange(receipts);
            allSummarySale.AddRange(saleArs);
            var sysCurDis = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == GetCompany().SystemCurrencyID) ?? new Display();
            var lccCurDis = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == GetCompany().LocalCurrencyID) ?? new Display();
            var sysCur = _context.Currency.FirstOrDefault(i => i.ID == GetCompany().SystemCurrencyID) ?? new Currency();
            var lccCur = _context.Currency.FirstOrDefault(i => i.ID == GetCompany().LocalCurrencyID) ?? new Currency();
            var allSale = (from all in allSummarySale

                           select new SaleHistoryByCustomers
                           {
                               LineID = all.LineID,
                               DateOut = all.DateOut,
                               DouType = all.DouType,
                               GrandTotal = all.GrandTotal,
                               ReceiptNmber = all.ReceiptNmber,
                               ItemName = all.ItemName,
                               Price = all.Price,
                               Qty = all.Qty,
                               Total = all.Total,
                               Uom = all.Uom,
                               SaleID = all.SaleID,
                               SDiscountItem = sysCur.Description + " " + _fncModule.ToCurrency(SDiscountItemCal, sysCurDis.Amounts),
                               SDiscountTotal = sysCur.Description + " " + _fncModule.ToCurrency(SDiscountTotalCal, sysCurDis.Amounts),
                               SVat = sysCur.Description + " " + _fncModule.ToCurrency(SVatCal, sysCurDis.Amounts),
                               SGrandTotalSys = sysCur.Description + " " + _fncModule.ToCurrency(SGrandTotalSysCal, sysCurDis.Amounts),
                               SGrandTotalLCC = lccCur.Description + " " + _fncModule.ToCurrency(SGrandTotalLCCCal, lccCurDis.Amounts),
                           }).OrderByDescending(o => o.DateOut).ToList();

            return Ok(allSale);
        }

        //================contract billing======


        [HttpPost]
        public async Task<IActionResult> SaveAttachment()
        {
            ContractBiling obj = new();
            var files = HttpContext.Request.Form.Files;

            foreach (var f in files)
            {
                var savePath = Path.Combine(_env.WebRootPath + "\\BussinessPartner\\UploadFile\\" + f.FileName);

                obj.Path = savePath;
                obj.FileName = f.FileName;
                using Stream fs = System.IO.File.Create(savePath);
                await f.CopyToAsync(fs);
            }
            return Ok(obj);
        }
        public async Task<IActionResult> DowloadFile(int AttachID)
        {
            ContractBiling attachment = _context.ContractBilings.Find(AttachID) ?? new ContractBiling();
            if (attachment.ID > 0)
            {
                string fullPath = Path.GetFullPath(string.Format("{0}{1}", _env.WebRootPath + "\\BussinessPartner\\UploadFile\\", attachment.FileName));
                byte[] bytes = await System.IO.File.ReadAllBytesAsync(fullPath);
                //Send the File to Download.
                return File(bytes, "application/octet-stream", attachment.FileName);
            }
            return NotFound();
        }
        [HttpPost]
        public IActionResult RemoveFileFromFolderMastre(string file)
        {
            string fullPath = Path.GetFullPath(string.Format("{0}{1}", _env.WebRootPath + "\\BussinessPartner\\UploadFile\\", file));
            if (System.IO.File.Exists(fullPath))
            {
                System.IO.File.Delete(fullPath);
            }
            return Ok();
        }

        //    [HttpPost]
        //    public IActionResult DeleteFileFromDatabase(int id, int psid, string key)
        //    {
        //        ContractBiling attachment = _context.ContractBilings.Find(id) ?? new ContractBiling();
        //        string fullPath = Path.GetFullPath(string.Format("{0}{1}", _env.WebRootPath + "\\BussinessPartner\\UploadFile\\", attachment.FileName));
        //        if (System.IO.File.Exists(fullPath))
        //        {
        //            System.IO.File.Delete(fullPath);
        //            _context.ContractBilings.RemoveRange(attachment);
        //            _context.SaveChanges();
        //        }
        //        var list = _bPartner.GetAttachmentAsync(psid, key);
        //        return Ok(list);
        //    }

        public IActionResult GetEmployee()
        {
            var list = (from ac in _context.Activites
                        join us in _context.UserAccounts on ac.UserID equals us.ID
                        join em in _context.Employees on us.EmployeeID equals em.ID
                        group new { ac, us, em } by new { em.ID } into datas
                        let data = datas.FirstOrDefault()
                        select new Activity
                        {
                            ID = data.ac.ID,
                            EmpName = data.em.Name
                        }
                      ).ToList();
            return Ok(list);
        }

        public async Task<IActionResult> ExportCustomer()
        {
            var getCustomer = await (from b in _context.BusinessPartners.Where(d => !d.Delete && d.Type == "Customer")
                                     join gr1 in _context.GroupCustomer1s.Where(d => !d.Delete) on b.Group1ID equals gr1.ID into group1
                                     from _gr1 in group1.DefaultIfEmpty()
                                     let gr1 = _gr1 ?? new GroupCustomer1()
                                     join pl in _context.PriceLists on b.PriceListID equals pl.ID
                                     select new Customer
                                     {
                                         Code = b.Code,
                                         Name = b.Name,
                                         Type = b.Type,
                                         Group1 = gr1.Name,
                                         Phone = b.Phone,
                                         PriceList = pl.Name
                                     }).ToListAsync();
            _wbAdapter.AddSheet(getCustomer);
            Stream ms = new MemoryStream();
            _wbAdapter.Write(ms);
            ms.Position = 0;
            return File(ms, "application/octet-stream", "BusinessPartnerExport.xls");
        }

        public async Task<IActionResult> ExportVender()
        {
            var getVender = await (from b in _context.BusinessPartners.Where(d => !d.Delete && d.Type == "Vendor")
                                   join gr1 in _context.GroupCustomer1s.Where(d => !d.Delete) on b.Group1ID equals gr1.ID into group1
                                   from _gr1 in group1.DefaultIfEmpty()
                                   let gr1 = _gr1 ?? new GroupCustomer1()
                                   join pl in _context.PriceLists on b.PriceListID equals pl.ID
                                   select new Vender
                                   {
                                       Code = b.Code,
                                       Name = b.Name,
                                       Type = b.Type,
                                       Group1 = gr1.Name,
                                       Phone = b.Phone,
                                       PriceList = pl.Name
                                   }).ToListAsync();
            _wbAdapter.AddSheet(getVender);
            Stream ms = new MemoryStream();
            _wbAdapter.Write(ms);
            ms.Position = 0;
            return File(ms, "application/octet-stream", "BusinessPartnerExport.xls");
        }

    }
}