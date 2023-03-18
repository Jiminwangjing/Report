using CKBS.Models.Services.Administrator.General;
using CKBS.Models.Services.AlertManagement;
using CKBS.Models.Services.Responsitory;
using KEDI.Core.Models.Validation;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using KEDI.Core.Premise.Authorization;
using System.Security.Claims;

namespace CKBS.Controllers
{
    [Privilege]
    public class ControlAlertController : Controller
    {
        private readonly IControlAlertRepository _alert;

        public ControlAlertController(IControlAlertRepository controlAlert)
        {
            _alert = controlAlert;
        }

        private int GetUserID()
        {
            _ = int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier).Value, out int _id);
            return _id;
        }

        private Company GetCompany()
        {
            return _alert.GetCompany(GetUserID());
        }
        #region
        private async Task AlertMasterValidateAsync(AlertMaster alertMaster)
        {
            var alertMs = await _alert.GetAlertMastersAsync(GetCompany().ID);
            if (alertMaster.Code == "" || alertMaster.Code == null)
            {
                ModelState.AddModelError("Code", "Please input code!");
            }
            if (alertMaster.ID <= 0)
            {
                if (alertMs.Any(i => i.Code == alertMaster.Code))
                {
                    ModelState.AddModelError("Code", "Code is already existed!");
                }
            }
            if (alertMaster.ID > 0)
            {
                var alertM = await _alert.GetAlertMasterAsync(alertMaster.ID);
                if (alertM != null)
                {
                    if (alertMs.Any(i => i.Code == alertMaster.Code && alertM.Code != alertMaster.Code))
                    {
                        ModelState.AddModelError("Code", "Code is already existed!");

                    }
                }
            }
            if (alertMaster.TypeOfAlert == TypeOfAlert.Null)
            {
                ModelState.AddModelError("TypeOfAlert", "Please select type of alert!");
            }
            if (alertMaster.ID <= 0)
            {
                if (alertMaster.TypeOfAlert == TypeOfAlert.Stock)
                {
                    var stock = await _alert.CheckTypeAlertAsync(TypeOfAlert.Stock);
                    if (stock.ID > 0)
                    {
                        ModelState.AddModelError("TypeOfAlert", "Alert management type \"Stock\" has already existed!");
                    }
                }
                if (alertMaster.TypeOfAlert == TypeOfAlert.ExpireItem)
                {
                    var expireItem = await _alert.CheckTypeAlertAsync(TypeOfAlert.ExpireItem);
                    if (expireItem.ID > 0)
                    {
                        ModelState.AddModelError("TypeOfAlert", "Alert management type \"Expire Item\" has already existed!");
                    }
                }
                if (alertMaster.TypeOfAlert == TypeOfAlert.DueDate)
                {
                    var dueDate = await _alert.CheckTypeAlertAsync(TypeOfAlert.DueDate);
                    if (dueDate.ID > 0)
                    {
                        ModelState.AddModelError("TypeOfAlert", "Alert management type \"Due Date\" has already existed!");
                    }
                }
                if (alertMaster.TypeOfAlert == TypeOfAlert.Appointment)
                {
                    var dueDate = await _alert.CheckTypeAlertAsync(TypeOfAlert.Appointment);
                    if (dueDate.ID > 0)
                    {
                        ModelState.AddModelError("TypeOfAlert", "Alert management type \"Appointment\" has already existed!");
                    }
                }
                if (alertMaster.TypeOfAlert == TypeOfAlert.CashOut)
                {
                    var dueDate = await _alert.CheckTypeAlertAsync(TypeOfAlert.CashOut);
                    if (dueDate.ID > 0)
                    {
                        ModelState.AddModelError("TypeOfAlert", "Alert management type \"Cash Out\" has already existed!");
                    }
                }
                if (alertMaster.TypeOfAlert == TypeOfAlert.Payment)
                {
                    var dueDate = await _alert.CheckTypeAlertAsync(TypeOfAlert.Payment);
                    if (dueDate.ID > 0)
                    {
                        ModelState.AddModelError("TypeOfAlert", "Alert management type \"Payment\" has already existed!");
                    }
                }
            }
        }
        #endregion 
        #region
        private async Task AlertDetailValidateAsync(AlertDetail alertDetail)
        {
            var alertDs = await _alert.GetAlertDetailsAsync(GetCompany().ID);
            if (alertDetail.Code == "" || alertDetail.Code == null)
            {
                ModelState.AddModelError("Code", "Please input the Code!");
            }
            if (alertDetail.Name == "" || alertDetail.Name == null)
            {
                ModelState.AddModelError("Name", "Please input the Name!");
            }
            if (alertDetail.ID <= 0)
            {
                if (alertDs.Any(i => i.Code == alertDetail.Code))
                {
                    ModelState.AddModelError("Code", "Code is already existed!");
                }
            }
            if (alertDetail.ID > 0)
            {
                var alertD = await _alert.GetAlertDetailAsync(alertDetail.ID);
                if (alertD != null)
                {
                    if (alertDs.Any(i => i.Code == alertDetail.Code && alertD.Code != alertDetail.Code))
                    {
                        ModelState.AddModelError("Code", "Code is already existed!");

                    }
                }
            }
        }
        #endregion
        public IActionResult Index()
        {
            ViewBag.AlertIndex = "highlight";
            return View();
        }

        public async Task<IActionResult> Edit(int? id)
        {
            ViewBag.AlertIndex = "highlight";
            var data = await _alert.GetAlertMasterAsync(id);
            return View(data);
        }

        public IActionResult Create()
        {
            ViewBag.AlertIndex = "highlight";
            return View();
        }
        public async Task<IActionResult> AlertDetail(int id)
        {
            ViewBag.AlertIndex = "highlight";
            var data = await _alert.GetAlertDetailsAsync(id, GetCompany().ID);
            return View(data);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(AlertMaster alertMaster)
        {
            ViewBag.AlertIndex = "highlight";
            alertMaster.CompanyID = GetCompany().ID;
            await AlertMasterValidateAsync(alertMaster);
            if (ModelState.IsValid)
            {
                await _alert.AddAsync(alertMaster);
                return RedirectToAction(nameof(Index));
            }
            else
            {
                return View(alertMaster);
            }

        }
        [HttpPost]
        public async Task<IActionResult> Create(AlertMaster alertMaster)
        {
            ViewBag.AlertIndex = "highlight";
            alertMaster.CompanyID = GetCompany().ID;
            await AlertMasterValidateAsync(alertMaster);
            if (ModelState.IsValid)
            {
                await _alert.AddAsync(alertMaster);
                return RedirectToAction(nameof(Index));
            }
            else
            {
                return View(alertMaster);
            }
        }

        public async Task<IActionResult> CashOutAlert(int id)
        {
            ViewBag.AlertIndex = "highlight";
            var data = await _alert.GetCashOutAsync(id);
            return View(data);
        }
        public async Task<IActionResult> ExprationItemAlert(int id)
        {
            ViewBag.AlertIndex = "highlight";
            var data = await _alert.GetExpirationStockItemAsync(id);
            return View(data);
        }

        public async Task<IActionResult> GetAlertMasters()
        {
            var data = await _alert.GetAlertMastersAsync(GetCompany().ID);
            return Ok(data);
        }

        [HttpPost]
        public async Task<IActionResult> CheckActive(AlertMaster alertMaster)
        {
            await _alert.AddAsync(alertMaster);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> CheckActiveDetail(int id, bool active)
        {
            await _alert.CheckActiveDeltailAsync(id, active);
            return RedirectToAction(nameof(AlertDetail), new { id });
        }

        [HttpGet]
        public async Task<IActionResult> GetWHUser(int alertM)
        {
            var data = await _alert.GetWHUserAsync(GetCompany().ID, 0, alertM);
            return Ok(data);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateAlertDetails(AlertDetail alertDetail)
        {
            ModelMessage msg = new();
            alertDetail.CompanyID = GetCompany().ID;
            await AlertDetailValidateAsync(alertDetail);
            if (ModelState.IsValid)
            {
                if (alertDetail.ID > 0)
                {
                    ModelState.AddModelError("success", "Alert Detail updated successfully!");
                }
                else
                {
                    ModelState.AddModelError("success", "Alert Detail created successfully!");
                }
                await _alert.UpdateAlertDetailsAsync(alertDetail);
                msg.Approve();
            }
            return Ok(msg.Bind(ModelState));
        }

        [HttpGet]
        public async Task<IActionResult> GetNotification()
        {
            var data = await _alert.GetNotificationAsync(GetUserID());
            return Ok(data);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateStockAlert(int id)
        {
            var isFalse = await _alert.UpdateStockAlertAsync(id);
            return Ok(isFalse);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateDueDateAlert(int id)
        {
            var isFalse = await _alert.UpdateDueDateAlertAsync(id);
            return Ok(isFalse);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateCashOutAlert(int id)
        {
            var isFalse = await _alert.UpdateCashOutAlertAsync(id);
            return Ok(isFalse);
        }
        [HttpPost]
        public async Task<IActionResult> UpdateExpirationItemAlert(int id)
        {
            var isFalse = await _alert.UpdateExpirationItemAlertAsync(id);
            return Ok(isFalse);
        }

        [HttpGet]
        public async Task<IActionResult> GetDueDateAlert(string typeRead, string typeOrder, bool isClear = false)
        {
            var data = await _alert.GetDueDateAlertAsync(typeRead, typeOrder, GetUserID(), isClear);
            return Ok(data);
        }

        [HttpGet]
        public async Task<IActionResult> GetStockAlert(string typeRead, string typeOrder, bool isClear = false)
        {
            var data = await _alert.GetStockAlertAsync(typeRead, typeOrder, GetUserID(), isClear);
            return Ok(data);
        }

        [HttpGet]
        public async Task<IActionResult> GetExpirationStockItemAlert(string typeRead, string typeOrder, bool isClear = false)
        {
            var data = await _alert.GetExpirationStockItemAlertAsync(typeRead, typeOrder, GetUserID(), isClear);
            return Ok(data);
        }

        [HttpGet]
        public async Task<IActionResult> GetCashOutAlert(string typeRead, string typeOrder, bool isClear = false)
        {
            var data = await _alert.GetCashOutAlertAsync(typeRead, typeOrder, GetUserID(), isClear);
            return Ok(data);
        }
    }
}
