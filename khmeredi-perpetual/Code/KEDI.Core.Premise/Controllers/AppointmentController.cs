using CKBS.AppContext;
using CKBS.Models.Services.Administrator.AlertManagement;
using CKBS.Models.Services.AlertManagement;
using CKBS.Models.Services.Appointment;
using CKBS.Models.Services.HumanResources;
using CKBS.Models.ServicesClass.KAMSService;
using KEDI.Core.Models.Validation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using KEDI.Core.Premise.Authorization;

namespace CKBS.Controllers
{
    [Privilege]
    public class AppointmentController : Controller
    {
        public readonly DataContext _context;

        public AppointmentController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult CheckAlertBeforeApp()
        {
            if (_context.AlertManagement.Count() <= 0)
            {
                return Ok(new { errormess = 1, href = 0 });
            }
            else
            {
                return Ok(new { errormess = 0, href = 1 });
            }
        }

        public IActionResult Appointment()
        {
            ViewBag.AppointmentList = "highlight";

            var _app = new Appointment();
            _app.AppointmentServices = new List<Models.Services.Appointment.AppointmentService>();

            var _data = new AppointmentServiceClass
            {
                Appointment = _app
            };

            return View(_data);
        }
        [HttpGet]
        public IActionResult GetCustomer()
        {
            var list = _context.BusinessPartners.Where(x => x.Delete == false && x.Type == "Customer").ToList();

            return Ok(list);
        }
        [HttpPost]
        public IActionResult GetVehicle(int CusID)
        {
            var vehicles = _context.AutoMobiles.Where(c => c.BusinessPartnerID == CusID);
            var _list = new List<VehicleServiceClass>();
            foreach (var item in vehicles)
            {
                var tempitem = new VehicleServiceClass
                {
                    AutoMID = item.AutoMID,
                    Plate = item.Plate,
                    Engine = item.Engine,
                    Frame = item.Frame,
                    VehiTypes = _context.AutoTypes.Where(c => c.TypeID == item.TypeID).FirstOrDefault().TypeName,
                    VehiBrands = _context.AutoBrands.Where(c => c.BrandID == item.BrandID).FirstOrDefault().BrandName,
                    VehiModels = _context.AutoModels.Where(c => c.ModelID == item.ModelID).FirstOrDefault().ModelName,
                    VehiColors = _context.AutoColors.Where(c => c.ColorID == item.ColorID).FirstOrDefault().ColorName,
                    Year = item.Year
                };
                _list.Add(tempitem);
            }
            return Ok(_list);
        }
        [HttpPost]
        public IActionResult ChkIfCusHasV(int CusID)
        {
            var __CusV = _context.AutoMobiles.Where(c => c.BusinessPartnerID == CusID);
            if (__CusV.Count() == 0)
            {
                return Ok(new { CusV = "No" });
            }
            else
            {
                return Ok(new { CusV = "Yes" });
            }
        }
        [HttpPost]
        public IActionResult GetResource(int Resource, int CusID, int AutoMID)
        {
            //From Quote
            if (Resource == 1)
            {
                if (AutoMID == 0)
                {
                    var _quoteData = _context.QuoteAutoMs.Where(c => c.Status == Status.Exist && c.CusID == CusID).Include(c => c.OrderQAutoM).ThenInclude(c => c.OrderDetailQAutoMs);
                    return Ok(_quoteData);
                }
                else
                {
                    var _quoteData = _context.QuoteAutoMs.Where(c => c.Status == Status.Exist && c.CusID == CusID && c.AutoMID == AutoMID).Include(c => c.OrderQAutoM).ThenInclude(c => c.OrderDetailQAutoMs);
                    return Ok(_quoteData);
                }
            }
            //From Invoice
            else if (Resource == 2)
            {
                if (AutoMID == 0)
                {
                    var _invoice = _context.ReceiptKvms.Include(c => c.KvmsInfo).Where(c => c.KvmsInfo.CusID == CusID);
                    return Ok(_invoice);
                }
                else
                {
                    var _invoice = _context.ReceiptKvms.Include(c => c.KvmsInfo).Where(c => c.KvmsInfo.CusID == CusID && c.KvmsInfo.AutoMID == AutoMID);
                    return Ok(_invoice);
                }
            }
            else
            //From ItemMasterData
            {
                var _itemMasterData = from itemM in _context.ItemMasterDatas
                                      join uom in _context.UnitofMeasures on itemM.InventoryUoMID equals uom.ID
                                      select new
                                      {
                                          ItemID = itemM.ID,
                                          ItemName = itemM.KhmerName,
                                          ItemUom = uom.Name
                                          //ItemCurrency = cur.Description,
                                          //ItemUnitPrice = i.UnitPrice,
                                          //ItemQty = 1

                                      };
                return Ok(_itemMasterData.ToList());
            }
        }
        [HttpPost]
        public IActionResult GetItemQuotes(int QuoteID)
        {
            var quote = _context.OrderDetailQAutoMs.Where(c => c.OrderID == QuoteID);
            return Ok(quote);
        }
        [HttpPost]
        public IActionResult GetItemInvoices(int InvoiceID)
        {
            var invoice = _context.ReceiptDetailKvms.Where(c => c.ReceiptKvmsID == InvoiceID).Include(c => c.UnitofMeansure);
            return Ok(invoice);
        }

        //Drop into App-List
        [HttpPost]
        public IActionResult DropQuote(int Id)
        {
            var keyId = (int)DateTime.Now.Ticks;
            if (keyId < 0)
            {
                var tempid = keyId.ToString().Replace("-", "");
                keyId = Convert.ToInt32(tempid);
            }
            var qlist = from q in _context.OrderDetailQAutoMs.Where(c => c.ID == Id)
                        select new
                        {
                            KeyID = keyId,
                            ID = 0,
                            ServiceName = q.KhmerName,
                            ServiceUom = q.Uom,
                            ServiceDate = "0001-01-01T00:00:00",
                            Status = StatusAppoint.open

                        };
            return Ok(qlist);
        }
        [HttpPost]
        public IActionResult DropInvoice(int Id)
        {
            var keyId = (int)DateTime.Now.Ticks;
            if (keyId < 0)
            {
                var tempid = keyId.ToString().Replace("-", "");
                keyId = Convert.ToInt32(tempid);
            }
            var invlist = from inv in _context.ReceiptDetailKvms
                          join uom in _context.UnitofMeasures on inv.UomID equals uom.ID
                          where inv.ID == Id
                          select new
                          {
                              KeyID = keyId,
                              ID = 0,
                              ServiceName = inv.KhmerName,
                              ServiceUom = uom.Name,
                              ServiceDate = "0001-01-01T00:00:00",
                              Status = StatusAppoint.open

                          };
            return Ok(invlist);
        }
        [HttpPost]
        public IActionResult DropItemMaster(int Id)
        {
            var keyId = (int)DateTime.Now.Ticks;
            if (keyId < 0)
            {
                var tempid = keyId.ToString().Replace("-", "");
                keyId = Convert.ToInt32(tempid);
            }
            var _itemMasterData = from itemM in _context.ItemMasterDatas
                                  join uom in _context.UnitofMeasures on itemM.InventoryUoMID equals uom.ID
                                  where itemM.ID == Id
                                  select new
                                  {
                                      KeyID = keyId,
                                      ID = 0,
                                      ServiceName = itemM.KhmerName,
                                      ServiceUom = uom.Name,
                                      ServiceDate = "0001-01-01T00:00:00",
                                      Status = StatusAppoint.open
                                  };
            return Ok(_itemMasterData.ToList());
        }
        [HttpPost]
        public IActionResult SaveAppointment(string appoint)
        {
            var _data = JsonConvert.DeserializeObject<Appointment>(appoint);
            _data.PostingDate = DateTime.Now;

            ModelMessage message = new ModelMessage(ModelState);

            if (_data.AppointmentServices.Any(c => c.ServiceDate.CompareTo(DateTime.Now) <= 0))
            {
                ModelState.AddModelError("ServiceDate ", "Invalid appointment date. All date must be greater than today! ");
            }

            if (ModelState.IsValid)
            {
                foreach (var item in _data.AppointmentServices)
                {
                    item.ServiceDate.ToShortTimeString();
                }

                _data.UserID = Convert.ToInt32(User.FindFirst("UserID").Value.ToString());
                _data.CompanyID = Convert.ToInt32(User.FindFirst("CompanyID").Value.ToString());

                _context.Appointment.Update(_data);
                _context.SaveChanges();

                ModelState.AddModelError("Success", "Appointment successfully saved!");
                message.Approve();
            }
            return Ok(message.Bind(ModelState));
        }

        //Appointment List
        [HttpGet]
        public IActionResult AppointmentList(string keyword = "", bool isJson = false)
        {
            ViewBag.style = "fa fa-cubes";
            ViewBag.Main = "Appointment";
            ViewBag.Page = "Appointment List";
            ViewBag.Subpage = "List";
            ViewBag.AppointmentList = "show";
            ViewBag.AppointmentList = "highlight";

            //Add Setting Alert Management
            DateTime currentDate = DateTime.Now;
            var alertMs = _context.AlertManagement.Where(am => am.StatusAlert == StatusAlert.Active).ToList();
            var _alertTimeStampMin = 0;
            alertMs.ForEach(am =>
            {
                if (am.Name == "Appointment")
                {
                    var appMs = _context.AppointmentService.Where(c => c.Status == StatusAppoint.open);
                    foreach (var item in appMs)
                    {
                        var settingApp = _context.SetttingAlert.FirstOrDefault(c => c.AlertManagementID == am.ID);
                        var bADate = settingApp.BeforeAppDate;
                        var typeBADate = settingApp.TypeBeforeAppDate;

                        switch (typeBADate)
                        {
                            case TypeFrequently.Minutes:
                                _alertTimeStampMin = bADate;
                                break;
                            case TypeFrequently.Hours:
                                _alertTimeStampMin = bADate * 60;
                                break;
                            case TypeFrequently.Days:
                                _alertTimeStampMin = bADate * 1440;
                                break;
                            case TypeFrequently.Weeks:
                                _alertTimeStampMin = bADate * 10080;
                                break;
                            case TypeFrequently.Months:
                                _alertTimeStampMin = bADate * 40320;
                                break;
                            case TypeFrequently.Years:
                                _alertTimeStampMin = bADate * 483840;
                                break;
                        }
                    }
                }
            });

            var allowUser = _context.SetttingAlertUser.Where(c => c.StatusAlertUser == StatusAlertUser.Active);
            bool __validUser = false;
            foreach (var item in allowUser)
            {
                __validUser = item.UserAccountID.ToString() == User.FindFirst("UserID").Value.ToString();
                if (__validUser)
                {
                    break;
                }
            }

            if (__validUser)
            {
                var _appData = from app in _context.Appointment.Where(c => c.CompanyID == Convert.ToInt32(User.FindFirst("CompanyID").Value.ToString()) && c.Status == StatusAppoint.open)
                               select new AppointmentViewModel
                               {
                                   ID = app.ID,
                                   CusName = _context.BusinessPartners.FirstOrDefault(c => c.ID == app.CustomerID).Name,
                                   PhoneCus = _context.BusinessPartners.FirstOrDefault(c => c.ID == app.CustomerID).Phone,
                                   Plate = _context.AutoMobiles.FirstOrDefault(c => c.AutoMID == app.VehicleID).Plate,
                                   Brand = _context.AutoBrands.FirstOrDefault(c => c.BrandID == _context.AutoMobiles.FirstOrDefault(c => c.AutoMID == app.VehicleID).BrandID).BrandName,
                                   Model = _context.AutoModels.FirstOrDefault(c => c.ModelID == _context.AutoMobiles.FirstOrDefault(c => c.AutoMID == app.VehicleID).ModelID).ModelName,
                                   Notification = _context.AppointmentService.Where(c => c.AppointmentID == app.ID && c.TimelyService == false && c.ServiceDate.AddMinutes(_alertTimeStampMin * (-1)).CompareTo(DateTime.Now) < 0).Count().ToString(),
                                   Status = app.Status == 0 ? "Open" : app.Status == (StatusAppoint)1 ? "Close" : "Cancel"
                               };
                if (isJson)
                {
                    if (!string.IsNullOrWhiteSpace(keyword))
                    {
                        keyword = RawWord(keyword);
                        StringComparison ignoreCase = StringComparison.InvariantCultureIgnoreCase;
                        _appData = _appData.Where(a => RawWord(a.CusName).Contains(keyword, ignoreCase)
                                    || RawWord(a.Plate).Contains(keyword, ignoreCase)
                                    || RawWord(a.Brand).Contains(keyword, ignoreCase)
                                    || RawWord(a.Model).Contains(keyword, ignoreCase)
                                    || RawWord(a.Status).Contains(keyword, ignoreCase));
                    }
                    return Ok(_appData);
                }

                return View(_appData.ToList());
            }
            else
            {
                var _appData = from app in _context.Appointment.Where(c => c.CompanyID == Convert.ToInt32(User.FindFirst("CompanyID").Value.ToString()) && c.Status == StatusAppoint.open)
                               select new AppointmentViewModel
                               {
                                   ID = app.ID,
                                   CusName = _context.BusinessPartners.FirstOrDefault(c => c.ID == app.CustomerID).Name,
                                   PhoneCus = _context.BusinessPartners.FirstOrDefault(c => c.ID == app.CustomerID).Phone,
                                   Plate = _context.AutoMobiles.FirstOrDefault(c => c.AutoMID == app.VehicleID).Plate,
                                   Brand = _context.AutoBrands.FirstOrDefault(c => c.BrandID == _context.AutoMobiles.FirstOrDefault(c => c.AutoMID == app.VehicleID).BrandID).BrandName,
                                   Model = _context.AutoModels.FirstOrDefault(c => c.ModelID == _context.AutoMobiles.FirstOrDefault(c => c.AutoMID == app.VehicleID).ModelID).ModelName,
                                   Notification = "0",
                                   Status = app.Status == 0 ? "Open" : app.Status == (StatusAppoint)1 ? "Close" : "Cancel"
                               };
                if (isJson)
                {
                    if (!string.IsNullOrWhiteSpace(keyword))
                    {
                        keyword = RawWord(keyword);
                        StringComparison ignoreCase = StringComparison.InvariantCultureIgnoreCase;
                        _appData = _appData.Where(a => RawWord(a.CusName).Contains(keyword, ignoreCase)
                                    || RawWord(a.Plate).Contains(keyword, ignoreCase)
                                    || RawWord(a.Brand).Contains(keyword, ignoreCase)
                                    || RawWord(a.Model).Contains(keyword, ignoreCase)
                                    || RawWord(a.Status).Contains(keyword, ignoreCase));
                    }
                    return Ok(_appData);
                }
                return View(_appData.ToList());
            }
        }
        private string RawWord(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                return string.Empty;
            }
            return Regex.Replace(keyword, "\\+s", string.Empty);
        }

        //Appointment Follow up
        [HttpGet]
        public IActionResult AppointmentFollowUp(int AppID)
        {
            var master = _context.Appointment.Include(c => c.AppointmentServices).FirstOrDefault(c => c.ID == AppID);

            //Add Setting Alert Management
            DateTime currentDate = DateTime.Now;
            var alertMs = _context.AlertManagement.Where(am => am.StatusAlert == StatusAlert.Active).ToList();
            var _alertTimeStampMin = 0;
            alertMs.ForEach(am =>
            {
                if (am.Name == "Appointment")
                {
                    var appMs = _context.AppointmentService.Where(c => c.Status == StatusAppoint.open);
                    foreach (var item in appMs)
                    {
                        var settingApp = _context.SetttingAlert.FirstOrDefault(c => c.AlertManagementID == am.ID);
                        var bADate = settingApp.BeforeAppDate;
                        var typeBADate = settingApp.TypeBeforeAppDate;

                        switch (typeBADate)
                        {
                            case TypeFrequently.Minutes:
                                _alertTimeStampMin = bADate;
                                break;
                            case TypeFrequently.Hours:
                                _alertTimeStampMin = bADate * 60;
                                break;
                            case TypeFrequently.Days:
                                _alertTimeStampMin = bADate * 1440;
                                break;
                            case TypeFrequently.Weeks:
                                _alertTimeStampMin = bADate * 10080;
                                break;
                            case TypeFrequently.Months:
                                _alertTimeStampMin = bADate * 40320;
                                break;
                            case TypeFrequently.Years:
                                _alertTimeStampMin = bADate * 483840;
                                break;
                        }
                    }
                }
            });

            List<Models.ServicesClass.KAMSService.AppointmentService> appServiceViewModels = new List<Models.ServicesClass.KAMSService.AppointmentService>();
            foreach (var item in master.AppointmentServices)
            {
                if (item.ServiceDate.AddMinutes(_alertTimeStampMin * (-1)).CompareTo(DateTime.Now) < 0)
                {
                    if (item.TimelyService == false)
                    {
                        var detail = new Models.ServicesClass.KAMSService.AppointmentService
                        {
                            ID = item.ID,
                            AppointmentID = item.AppointmentID,
                            ServiceName = item.ServiceName + "  <i class='fas fa-bell fa-sm text-danger'></i>",
                            ServiceDate = item.ServiceDate.ToShortDateString(),
                            ServiceUom = item.ServiceUom,
                            Status = item.Status == 0 ? "Open" : item.Status == (StatusAppoint)1 ? "Close" : "Cancel"
                        };
                        appServiceViewModels.Add(detail);
                    }
                    else
                    {
                        var detail = new Models.ServicesClass.KAMSService.AppointmentService
                        {
                            ID = item.ID,
                            AppointmentID = item.AppointmentID,
                            ServiceName = item.ServiceName + "  <i class='fas fa-bell-slash fa-sm text-danger'></i>",
                            ServiceDate = item.ServiceDate.ToShortDateString(),
                            ServiceUom = item.ServiceUom,
                            Status = item.Status == 0 ? "Open" : item.Status == (StatusAppoint)1 ? "Close" : "Cancel"
                        };
                        appServiceViewModels.Add(detail);
                    }
                }
                else
                {
                    var detail = new Models.ServicesClass.KAMSService.AppointmentService
                    {
                        ID = item.ID,
                        AppointmentID = item.AppointmentID,
                        ServiceName = item.ServiceName,
                        ServiceDate = item.ServiceDate.ToShortDateString(),
                        ServiceUom = item.ServiceUom,
                        Status = item.Status == 0 ? "Open" : item.Status == (StatusAppoint)1 ? "Close" : "Cancel"
                    };
                    appServiceViewModels.Add(detail);
                }
            }

            var _appointment = new AppointmentViewModel
            {
                ID = master.ID,
                CustomerID = master.CustomerID,
                CusName = _context.BusinessPartners.FirstOrDefault(c => c.ID == master.CustomerID).Name,
                PhoneCus = _context.BusinessPartners.FirstOrDefault(c => c.ID == master.CustomerID).Phone,
                VehicleID = master.VehicleID,
                Plate = master.VehicleID == 0 ? "No Vehicle" : _context.AutoMobiles.FirstOrDefault(c => c.AutoMID == master.VehicleID).Plate,
                Brand = master.VehicleID == 0 ? "No Vehicle" : _context.AutoBrands.FirstOrDefault(c => c.BrandID == _context.AutoMobiles.FirstOrDefault(c => c.AutoMID == master.VehicleID).BrandID).BrandName,
                Model = master.VehicleID == 0 ? "No Vehicle" : _context.AutoModels.FirstOrDefault(c => c.ModelID == _context.AutoMobiles.FirstOrDefault(c => c.AutoMID == master.VehicleID).ModelID).ModelName,
                Status = master.Status == 0 ? "Open" : master.Status == (StatusAppoint)1 ? "Close" : "Cancel",
                PostingDate = master.PostingDate,
                ClosingDate = master.ClosingDate,
                AppointmentServices = appServiceViewModels.ToList()
            };

            return View(_appointment);
        }

        [HttpPost]
        public IActionResult UpdateAppointment(string appoint)
        {
            var _data = JsonConvert.DeserializeObject<Appointment>(appoint);

            ModelMessage message = new ModelMessage(ModelState);

            if (_data.AppointmentServices.Any(c => c.ServiceDate.CompareTo(DateTime.Now) <= 0 && c.ID == 0))
            {
                ModelState.AddModelError("ServiceDate ", "Invalid appointment date. All date must be greater than today! ");
            }

            if (ModelState.IsValid)
            {
                var index = 0;
                foreach (var item in _data.AppointmentServices)
                {
                    string[] cutName = item.ServiceName.Split(" ");
                    if (cutName.Length > 1)
                    {
                        item.ServiceName = cutName[0];
                    }

                    if (item.Status == StatusAppoint.close || item.Status == StatusAppoint.cancel)
                    {
                        item.TimelyService = true;
                    }
                    else
                    {
                        index++;
                    }
                }

                if (index == 0)
                {
                    _data.Status = StatusAppoint.close;
                }
                else
                {
                    _data.Status = StatusAppoint.open;
                }

                _data.UserID = Convert.ToInt32(User.FindFirst("UserID").Value.ToString());
                _data.CompanyID = Convert.ToInt32(User.FindFirst("CompanyID").Value.ToString());

                _context.Appointment.Update(_data);
                _context.SaveChanges();

                ModelState.AddModelError("Success", "Appointment successfully updated!");
                message.Approve();
            }
            return Ok(message.Bind(ModelState));
        }

        [HttpPost]
        public IActionResult CancelAppointment(int AppID)
        {
            var app = _context.Appointment.FirstOrDefault(c => c.ID == AppID);
            app.Status = StatusAppoint.cancel;

            _context.Update(app);
            _context.SaveChanges();

            return Ok();
        }

        //Appointment List Report
        [HttpGet]
        public IActionResult AppointmentListReport(string keyword = "", bool isJson = false)
        {
            ViewBag.style = "fa fa-cubes";
            ViewBag.Main = "Appointment";
            ViewBag.Page = "Appointment List Report";
            ViewBag.Subpage = "List";
            ViewBag.AppointmentListReport = "show";
            ViewBag.AppointmentListReport = "highlight";

            var _appData = from app in _context.Appointment.Where(c => c.CompanyID == Convert.ToInt32(User.FindFirst("CompanyID").Value.ToString()) && c.Status != StatusAppoint.open)
                           select new AppointmentViewModel
                           {
                               ID = app.ID,
                               CusName = _context.BusinessPartners.FirstOrDefault(c => c.ID == app.CustomerID).Name,
                               PhoneCus = _context.BusinessPartners.FirstOrDefault(c => c.ID == app.CustomerID).Phone,
                               Plate = _context.AutoMobiles.FirstOrDefault(c => c.AutoMID == app.VehicleID).Plate,
                               Brand = _context.AutoBrands.FirstOrDefault(c => c.BrandID == _context.AutoMobiles.FirstOrDefault(c => c.AutoMID == app.VehicleID).BrandID).BrandName,
                               Model = _context.AutoModels.FirstOrDefault(c => c.ModelID == _context.AutoMobiles.FirstOrDefault(c => c.AutoMID == app.VehicleID).ModelID).ModelName,
                               //AppointmentDate = app.AppointmentDate.ToShortDateString(),
                               Status = app.Status == 0 ? "Open" : app.Status == (StatusAppoint)1 ? "Close" : "Cancel"
                           };
            if (isJson)
            {
                if (!string.IsNullOrWhiteSpace(keyword))
                {
                    keyword = RawWord(keyword);
                    StringComparison ignoreCase = StringComparison.InvariantCultureIgnoreCase;
                    _appData = _appData.Where(a => RawWord(a.CusName).Contains(keyword, ignoreCase)
                                || RawWord(a.Plate).Contains(keyword, ignoreCase)
                                || RawWord(a.Brand).Contains(keyword, ignoreCase)
                                || RawWord(a.Model).Contains(keyword, ignoreCase)
                                || RawWord(a.Status).Contains(keyword, ignoreCase));
                }
                return Ok(_appData);
            }

            return View(_appData.ToList());
        }
    }
}
