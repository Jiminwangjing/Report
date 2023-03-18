using CKBS.AppContext;
using CKBS.Models.Services.Account;
using CKBS.Models.Services.Administrator.General;
using CKBS.Models.Services.HumanResources;
using KEDI.Core.Premise.Repository;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KEDI.Core.Premise.Controllers
{
    public class SerialBatchController : Controller
    {
        private readonly DataContext _context;
        private readonly ISerialBatchReportRepository _serialBatchReport;

        public SerialBatchController(DataContext context, ISerialBatchReportRepository serialBatchReport)
        {
            _context = context;
            _serialBatchReport = serialBatchReport;
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
        public IActionResult Serial()
        {
            ViewBag.Serial = "highlight";
            return View();
        }
        public IActionResult Batch()
        {
            ViewBag.Batch = "highlight";
            return View();
        }
        public IActionResult GetWarehouse()
        {
            var list = _context.Warehouses.Where(x => x.Delete == false).ToList();
            return Ok(list);
        }
        public IActionResult GetBranch()
        {
            var list = _context.Branches.Where(x => x.Delete == false).ToList();
            return Ok(list);
        }

        public IActionResult GetEmployee(int BranchID)
        {
            var list = from user in _context.UserAccounts.Where(x => x.Delete == false)
                       join emp in _context.Employees
                       on user.EmployeeID equals emp.ID
                       where user.BranchID == BranchID
                       select new UserAccount
                       {
                           ID = user.ID,
                           Employee = new Employee
                           {
                               Name = emp.Name
                           }
                       };
            return Ok(list);
        }

        public async Task<IActionResult> GetSerial(string dateFrom, string dateTo, int branchID, int wahouseID, int userId, int status)
        {
            var data = await _serialBatchReport.GetSerialsAsync(dateFrom, dateTo, branchID, wahouseID, userId, status, GetCompany().SystemCurrencyID);
            return Ok(data);
        }
        public async Task<IActionResult> GetBatch(string dateFrom, string dateTo, int branchID, int wahouseID, int userId)
        {
            var data = await _serialBatchReport.GetBatchAsyns(dateFrom, dateTo, branchID, wahouseID, userId);
            return Ok(data);
        }
    }
}
