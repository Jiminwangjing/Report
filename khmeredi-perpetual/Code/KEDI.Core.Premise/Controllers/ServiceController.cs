using CKBS.AppContext;
using CKBS.Models.Services.HumanResources;
using CKBS.Models.Services.Inventory;
using CKBS.Models.Services.Inventory.Category;
using KEDI.Core.Models.Validation;
using KEDI.Core.Premise.Authorization;
using KEDI.Core.Premise.Models.Services.Activity;
using KEDI.Core.Premise.Models.Services.Inventory;
using KEDI.Core.Premise.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KEDI.Core.Premise.Controllers
{
    [Privilege]
    public class ServiceController : Controller
    {
        private readonly IEquipmentRepository _equipment;

        private readonly DataContext _context;
        public ServiceController(IEquipmentRepository equipment, DataContext context)
        {
            _context = context;
            _equipment = equipment;
        }
        public IActionResult EquipmentCard()
        {
            ViewBag.EquipmentCard = "highlight";
            return View();
        }
        public IActionResult ServiceCall()
        {
            ViewBag.ServiceCall = "highlight";
            var seriesSC = _equipment.GetSeries();
            ViewBag.SeriesSC = seriesSC;
            return View(seriesSC);
        }

        public IActionResult SerialNumberReport()
        {
            ViewBag.SerialNumberReport = "highlight";

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetEquipmentMaster(string serialNumber, string type)
        {
            var data = await _equipment.GetEquipmentMasterAsync(serialNumber, type);
            return Ok(data);
        }
        [HttpGet]
        public async Task<IActionResult> GetItemmasterData()
        {
            var list = (from IM in _context.ItemMasterDatas.Where(s => s.Delete == false)
                        join wh in _context.Warehouses on IM.WarehouseID equals wh.ID
                        join whd in _context.WarehouseDetails on wh.ID equals whd.WarehouseID
                        select new
                        {
                            ID = IM.ID,
                            ItemCode = IM.Code,
                            ItemName = IM.EnglishName,
                            MfrSerialNo = whd.MfrSerialNumber,
                            SerialNo = whd.SerialNumber,
                            Unitprice = IM.UnitPrice,
                            Stock = IM.StockIn
                        }).ToList();
            return Ok(await Task.FromResult(list));
        }
        public List<ServiceItem> BindRowsServiceItem(string lineMTID = "")
        {
            var activity = (from setact in _context.SetupTypes

                            select new Activytyview
                            {
                                ID = setact.ID,
                                Name = setact.Name,
                            }).ToList();
            activity.Insert(0, new Activytyview
            {
                ID = 0,
                Name = "-- Select --",
            });
            List<ServiceItem> SI = new List<ServiceItem>();
            for (var j = 0; j < 10; j++)
            {
                string lineId = DateTime.Now.Ticks.ToString();
                ServiceItem siobj = new ServiceItem
                {

                    LineID = lineId,
                    LineMTID = lineMTID,
                    Activitys = activity.Select(i => new SelectListItem
                    {
                        Text = i.Name,
                        Value = i.ID.ToString(),
                    }).ToList(),

                    HandledByName = "",
                    Remark = "",
                    TechnicianName = "",
                    ActivityName = "",

                };
                SI.Add(siobj);
            }
            return SI;
        }
        [HttpGet]
        public IActionResult CreateDefualtRows()
        {
            List<ServiceData> SD = new List<ServiceData>();

            for (var i = 0; i < 10; i++)
            {
                string lineMTId = DateTime.Now.Ticks.ToString();

                ServiceData sdobj = new ServiceData
                {
                    LineMTID = lineMTId,
                    ItemCode = "",
                    ItemName = "",
                    MfrSerialNo = "",
                    SerialNo = "",
                    PlateNumber = "",

                    ServiceItems = BindRowsServiceItem(lineMTId),
                };
                SD.Add(sdobj);
            }
            return Ok(SD);
        }
        [HttpGet]
        public IActionResult GetActivity()
        {
            var list = (from act in _context.Activites.Where(s => s.Activities == Activities.Services)
                        join settype in _context.SetupTypes on act.TypeID equals settype.ID
                        join subj in _context.SetupSubjects on act.SubNameID equals subj.ID
                        let bp = _context.BusinessPartners.Where(s => s.ID == act.BPID).FirstOrDefault()
                        select new
                        {
                            ID = act.ID,
                            ActivityType = act.Activities.ToString(),
                            Activity = settype.Name,
                            Subject = subj.Name,
                            BPName = bp.Name,
                        }).ToList();
            return Ok(list);
        }
        [HttpPost]
        public async Task<IActionResult> UpdateServiceCall(string serviceCalls)
        {
            var serviceCall = JsonConvert.DeserializeObject<ServiceCall>(serviceCalls, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });
            ModelMessage msg = new();
            if (serviceCall.BPID <= 0)
            {
                ModelState.AddModelError("bp", "BP is require!");
            }
            if (string.IsNullOrWhiteSpace(serviceCall.Subject))
            {
                ModelState.AddModelError("Subject", "Item Subject is require!");
            }
            if (serviceCall.CreatedOnDate.Year == 1)
            {
                ModelState.AddModelError("CreatedOnDate", "CreatedOnDate is require!");
            }
            if (serviceCall.CallStatus == CallStatusType.Closed)
            {
                if (string.IsNullOrWhiteSpace(serviceCall.Resolution))
                {
                    ModelState.AddModelError("Resolution", "please input  Resolution ...!");
                }
                if (serviceCall.ClosedOnDate == null)
                    ModelState.AddModelError("ClosedOnDate", "Closed Date not null ...!");
                if (serviceCall.ClosedOnTime == null)
                    ModelState.AddModelError("ClosedOnTime", "Closed Time not null ...!");
                if ((DateTime)serviceCall.ClosedOnDate < serviceCall.CreatedOnDate)
                    ModelState.AddModelError("ClosedOnDate", "Closed Date must big than Create Date ...!");
            }
            if (serviceCall.CallStatus == CallStatusType.Pending)
            {
                if (serviceCall.ChannelID == 0)
                    ModelState.AddModelError("Channel", "please Select  Channel ...!");
            }

            if (serviceCall.TechnicianID == 0)
            {
                ModelState.AddModelError("TechnicianID", "Technician. is require!");
            }
            if (string.IsNullOrWhiteSpace(serviceCall.Number))
            {
                ModelState.AddModelError("Number", "Number is require!");
            }
            //if (string.IsNullOrWhiteSpace(serviceCall.SerialNumber))
            //{
            //    ModelState.AddModelError("SerialNumber", "SerialNumber is require!");
            //}
            //if (string.IsNullOrWhiteSpace(serviceCall.Name))
            //{
            //    ModelState.AddModelError("Name", "Name is require!");
            //}
            serviceCall.ServiceDatas = serviceCall.ServiceDatas.Where(i => i.ItemID > 0).ToList();
            foreach (var s in serviceCall.ServiceDatas)
            {
                if (string.IsNullOrWhiteSpace(s.ItemCode))
                    ModelState.AddModelError("ItemCode", "ItemCode in ServiceData is null ...!");
                if (s.Qty <= 0)
                    ModelState.AddModelError("Qty", "Qty in ServiceData is 0 ...!");
                s.ServiceItems = s.ServiceItems.Where(s => s.HandledByID > 0).ToList();
                foreach (var x in s.ServiceItems)
                {
                    if (x.ActivityID == 0)
                        ModelState.AddModelError("Task", "please select Task in ServiceItem...!");
                    if (x.HandledByID == 0)
                        ModelState.AddModelError("Handled By", "please input Handled By in ServiceItem ...!");
                    if (x.TechnicianID == 0)
                        ModelState.AddModelError("Technician", "please input Technician in ServiceItem ...!");
                    if (x.StartDate.Year == 1)
                        ModelState.AddModelError("StartDate", "please select StartDate in ServiceItem ...!");
                    if (x.EndDate.Year == 1)
                        ModelState.AddModelError("EndDate", "please select EndDate in ServiceItem ...!");
                    Activity objact = new Activity();
                    General gen = new General();
                    if (x.Acitivity == true)
                    {
                        objact.TypeID = x.ActivityID;
                        objact.EmpNameID = (AssignedTo)x.TechnicianID;
                        objact.AssignByID = x.HandledByID;
                        //objact.Activities =(Activities)x.LinkActivytyID;

                        gen.Remark = x.Remark;
                        gen.StartTime = x.StartDate + x.StartTime;
                        gen.EndTime = x.EndDate + x.EndTime;
                        if (ModelState.IsValid)
                        {
                            _context.Add(objact);
                            _context.SaveChanges();
                            gen.ActivityID = objact.ID;
                            _context.Add(gen);
                            _context.SaveChanges();
                        }
                    }
                }
            }

            if (ModelState.IsValid)
            {
                await _equipment.UpdateServiceCallAsync(serviceCall);
                msg.Approve();
                ModelState.AddModelError("success", "Service Call created successfully!");
            }

            return Ok(msg.Bind(ModelState));
        }
        [HttpGet]
        public IActionResult FindServiceCall(string number, int seriesid)
        {
            var obj = _equipment.FindServiceCall(number, seriesid);
            return Ok(obj);
        }
        [HttpGet]
        public async Task<IActionResult> GetCustomers()
        {
            var data = await _equipment.GetCustomersAsync();
            return Ok(data);
        }
        [HttpGet]
        public async Task<IActionResult> GetSerials(int bpid)
        {
            var data = await _equipment.GetSerialsAsync(bpid);
            return Ok(data);
        }
        // Mr Bunthorn
        [HttpGet]
        public async Task<IActionResult> GetCallID()
        {
            var number = await _equipment.GetServiceAsync();

            return Ok(number.Count + 1);
        }
        [HttpGet]
        public async Task<IActionResult> GetServiceCall()
        {
            var list = await _equipment.GetServiceCallAsync();

            return Ok(list);
        }
        public IActionResult ServiceCallReport()
        {
            ViewBag.ServiceCallReport = "highlight";
            return View();
        }

        [HttpPost]// save data to database
        public IActionResult CreateUpdate(Channel data)
        {

            ModelMessage msg = new();

            if (string.IsNullOrWhiteSpace(data.Name))
            {
                ModelState.AddModelError("Name", "please input Name ...!");
            }

            if (ModelState.IsValid)
            {
                _context.Update(data);
                _context.SaveChanges();
                ModelState.AddModelError("success", "Item save successfully.");
                msg.Approve();
            }
            var list = (from ch in _context.Channels.Where(s => s.Status == false)
                        select new Channel
                        {
                            ID = ch.ID,
                            Name = ch.Name,

                        }).OrderBy(s => s.Name).ToList();
            return Ok(new { Model = msg.Bind(ModelState), Data = list });
        }
        public IActionResult GetChannel()
        {
            var list = _context.Channels.OrderBy(s => s.Name).ToList();
            return Ok(list);
        }
        //  employee
        [HttpGet]
        public IActionResult GetSaleEmployee()
        {
            var list = (from em in _context.Employees.Where(s => s.Delete == false)
                        select new Employee
                        {
                            ID = em.ID,
                            Code = em.Code,
                            Name = em.Name,
                            GenderDisplay = em.Gender == 0 ? "Male" : "Female",
                            Position = em.Position,
                            Phone = em.Phone,
                            Email = em.Email,
                            Address = em.Address,
                            EMType = em.EMType,
                        }).OrderBy(s => s.Name).ToList();
            return Ok(list);
        }
        //Filter Data
        [HttpGet]
        public IActionResult GetServiceReport(string DateFrom, string DateTo, int priority, int callstatus)
        {
            List<ServiceCall> ServiceCalls = new();

            if (DateFrom != null && DateTo == null && priority == 0 && callstatus == 0)
            {
                ServiceCalls = _context.ServiceCalls.Where(w => w.CreatedOnDate >= Convert.ToDateTime(DateFrom)).ToList();
            }
            else if (DateFrom != null && DateTo != null && priority == 0 && callstatus == 0)
            {
                ServiceCalls = _context.ServiceCalls.Where(w => w.CreatedOnDate >= Convert.ToDateTime(DateFrom) && w.CreatedOnDate <= Convert.ToDateTime(DateTo)).ToList();
            }
            else if (DateFrom != null && DateTo != null && priority != 0 && callstatus == 0)
            {
                ServiceCalls = _context.ServiceCalls.Where(w => w.CreatedOnDate >= Convert.ToDateTime(DateFrom) && w.CreatedOnDate <= Convert.ToDateTime(DateTo) && (int)w.Priority == priority).ToList();
            }
            else if (DateFrom != null && DateTo != null && priority != 0 && callstatus != 0)
            {
                ServiceCalls = _context.ServiceCalls.Where(w => w.CreatedOnDate >= Convert.ToDateTime(DateFrom) && w.CreatedOnDate <= Convert.ToDateTime(DateTo) && (int)w.Priority == priority && (int)w.CallStatus == callstatus).ToList();
            }
            else if (DateFrom != null && DateTo != null && priority != 0 && callstatus != 0)
            {
                ServiceCalls = _context.ServiceCalls.Where(w => w.CreatedOnDate >= Convert.ToDateTime(DateFrom) && w.CreatedOnDate <= Convert.ToDateTime(DateTo) && (int)w.Priority == priority).ToList();
            }

            else if (DateFrom != null && DateTo == null && priority != 0 && callstatus == 0)
            {
                ServiceCalls = _context.ServiceCalls.Where(w => w.CreatedOnDate >= Convert.ToDateTime(DateFrom) && (int)w.Priority == priority).ToList();
            }
            else if (DateFrom != null && DateTo == null && priority == 0 && callstatus != 0)
            {
                ServiceCalls = _context.ServiceCalls.Where(w => w.CreatedOnDate >= Convert.ToDateTime(DateFrom) && (int)w.CallStatus == callstatus).ToList();
            }
            else if (DateFrom == null && DateTo == null && priority != 0 && callstatus == 0)
            {
                ServiceCalls = _context.ServiceCalls.Where(w => (int)w.Priority == priority).ToList();
            }
            else if (DateFrom == null && DateTo == null && priority != 0 && callstatus != 0)
            {
                ServiceCalls = _context.ServiceCalls.Where(w => (int)w.Priority == priority && (int)w.CallStatus == callstatus).ToList();
            }
            else if (DateFrom == null && DateTo == null && priority == 0 && callstatus != 0)
            {
                ServiceCalls = _context.ServiceCalls.Where(w => (int)w.CallStatus == callstatus).ToList();
            }
            else
            {
                return Ok(new List<ServiceCall>());
            }
            var list = (from s in ServiceCalls
                        join cus in _context.BusinessPartners on s.BPID equals cus.ID
                        let item = _context.ItemMasterDatas.Where(i => i.ID == s.ItemID).FirstOrDefault() ?? new ItemMasterData()
                        let g = _context.ItemGroup1.Where(i => i.ItemG1ID == s.ItemGroupID).FirstOrDefault() ?? new ItemGroup1()
                        select new ServiceCallview
                        {
                            ID = s.ID,
                            ItemID = s.ItemID,
                            BPID = s.BPID,
                            BPCode = cus.Code,
                            BName = cus.Name,

                            MfrSerialNo = s.MfrSerialNo,
                            CallID = s.CallID,

                            ClosedOnDate = s.ClosedOnDate?.ToString("dd/MM/yyyy"),
                            ClosedOnTime = s.ClosedOnTime,

                            CreatedOnDate = s.CreatedOnDate.ToString("dd/MM/yyyy"),
                            CreatedOnTime = s.CreatedOnTime,

                            GName = g.Name,
                            ItemGroupID = s.ItemGroupID,
                            CallStatus = s.CallStatus.ToString(),
                            ItemCode = item.Code,
                            ItemName = item.KhmerName,
                            Number = s.Number,
                            Priority = s.Priority.ToString(),
                            SerialNumber = s.Number,
                            Resolvedondate = s.ClosedOnDate?.ToString("dd/MM/yyyy"),
                            Resolvedontime = s.ClosedOnTime,
                            Subject = s.Subject,
                            Resolution = s.Resolution,
                        }).ToList();
            return Ok(list);
        }

        //Services Dashboard
        public IActionResult ServicesDashboard()
        {
            ViewBag.ServiceDashboard = "highlight";

            return View();
        }
        // start block Chart 
        public async Task<IActionResult> CountChartPie()
        {
            var list = _equipment.GetPie1Async();
            return Ok(await list);
        }
        public async Task<IActionResult> CountChartPieOrigin()
        {
            var list = _equipment.GetPieOriginAsync();
            return Ok(await list);
        }
        public async Task<IActionResult> CountChartAcount()
        {
            var list = _equipment.GetAcountAsync();
            return Ok(await list);
        }

        public async Task<IActionResult> ClosebyAcount()
        {
            var list = _equipment.CloseByAcountAsyc();
            return Ok(await list);
        }
        public async Task<IActionResult> AverageResolutiontAcount()
        {
            var list = await _equipment.GetResolutionAsync();
            return Ok(list);
        }
        public async Task<IActionResult> AVGCloseTime()
        {
            var list = _equipment.AvgCloseTimeAsync();
            return Ok(await list);
        }
        //  -------------------start block Dashboard Income Statement ----------------------------

        [HttpGet]
        public async Task<IActionResult> ChartOfAccount()
        {
            var list = await _equipment.ChartOfAccountAsync();

            return Ok(list);
        }
        [HttpGet]
        public async Task<IActionResult> BarchartRevenue_COGS()
        {
            var list = await _equipment.BarchartRevenue_COGSAsync();
            return Ok(list);
        }
        [HttpGet]
        public async Task<IActionResult> ChartbarEBIT()
        {
            var list = await _equipment.ChartbarEBITAsync();
            return Ok(list);
        }
        [HttpGet]
        public async Task<IActionResult> BarchartOpexs()
        {
            var list = await _equipment.BarchartOpexsAsync();
            return Ok(list);
        }

    }
}
