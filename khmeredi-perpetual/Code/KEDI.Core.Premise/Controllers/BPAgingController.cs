using CKBS.AppContext;
using CKBS.Models.Services.Banking;
using CKBS.Models.Services.POS;
using CKBS.Models.Services.POS.KVMS;
using CKBS.Models.Services.Purchase;
using CKBS.Models.ServicesClass.Report;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using StatusReceipt = CKBS.Models.Services.POS.KVMS.StatusReceipt;
using KEDI.Core.Premise.Authorization;
using KEDI.Core.Premise.Models.Sale;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;
using KEDI.Core.Premise.Models.ServicesClass.ActivityViewModel;
using KEDI.Core.Premise.Models.Services.Activity;
using CKBS.Models.Services.HumanResources;
using KEDI.Core.Premise.Models.Services.HumanResources;
using Microsoft.EntityFrameworkCore;
using CKBS.Models.Services.Account;

namespace CKBS.Controllers
{
    [Privilege]
    public class BPAgingController : Controller
    {
        private readonly DataContext _context;
        public BPAgingController(DataContext context)
        {
            _context = context;
        }
        private int GetUserId()
        {
            _ = int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out int _userId);
            return _userId;
        }
        public IActionResult CustomerAging()
        {
            ViewBag.CustomerAging = "highlight";
            return View();
        }

        public IActionResult VendorAging()
        {
            ViewBag.VendorAging = "highlight";
            return View();
        }
        //======================My Activity Report====================
        public Dictionary<int, string> AssignedTo => new()
        {
            /*{1, "User"},*/ {2, "Employee"}/*, {3, "Recipient List"}*/
        };
        public Dictionary<int, string> Recurrence => new()
        {
            {1, "None"}, {2, "Daily"}, {3, "Weekly"},{4, "Monthly"},{5, "Annually"}
        };
        public Dictionary<int, string> Priorities => new()
        {
            {1, "Low"}, {2, "Normal"}, {3, "Hight"}
        };
        public IActionResult MyActivities()
        {
            ViewBag.MyActivities = "highlight";
            return View();
        }
        public IActionResult GetActivity()
        {
            var user = _context.UserAccounts.Include(i => i.Employee).Where(x => x.ID == GetUserId()).FirstOrDefault() ?? new UserAccount();
            var empName = user.Employee != null ? user.Employee.Name : "";
            var data = (from act in _context.Activites.Where(i => i.UserID == user.ID)
                        join g in _context.Generals on act.ID equals g.ActivityID
                        let bp = _context.BusinessPartners.Where(x => x.ID == act.BPID).FirstOrDefault() ?? new BusinessPartner()
                        let st = _context.SetupStatuses.FirstOrDefault(x => x.ID == g.StatusID) ?? new SetupStatus()
                        select new ActivityView
                        {
                            ID = act.ID,
                            GID = g.ID,
                            BPID = bp.ID,
                            Number = act.Number,
                            StartTime = g.StartTime,
                            StartTimes = g.StartTime.ToString("HH:mm"),
                            SetActName = act.Activities.ToString(),
                            EmpName = empName,
                            Recurrences = g.Recurrence,
                            BpName = bp.Name,
                            StatusID = g.StatusID,
                            Status = st.Status,
                            Remark = g.Remark,
                            AssignedBy = empName
                        }
                      ).ToList();
            return Ok(data);
        }

        //========Update Data Calandar========
        [HttpPost]
        public IActionResult UpdateDataMyActivity(string _data)
        {
            Activity data = JsonConvert.DeserializeObject<Activity>(_data, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });


            if (ModelState.IsValid)
            {
                _context.Activites.Update(data);
                _context.SaveChanges();

            }
            return Ok(new { url = "/BPAging/MyActivities" });
        }
        //===========find data activity======
        //=====================end My Activity Report================
        //==================Activity Overview Report=========
        public IActionResult ActivitiesOverview()
        {
            ViewBag.ActivitiesOverview = "highlight";
            return View();
        }
        //=========get bp===============
        [HttpGet]
        public IActionResult GetBP()
        {
            var bp = _context.BusinessPartners.Where(x => x.Type == "Customer").ToList();
            return Ok(bp);
        }
        //=======get data assignto==============
        //public IActionResult GetDataUser()
        //{
        //    var datauser = (from user in _context.UserAccounts
        //                    join emp in _context.Employees on user.EmployeeID equals emp.ID
        //                    join br in _context.Branches on user.BranchID equals br.ID
        //                    group new {emp,br, user } by new { emp.Name } into datas
        //                    let data = datas.FirstOrDefault()
        //                    let user = data.user
        //                    let emp=data.emp
        //                    let br=data.br
        //                    select new ActivityView
        //                    {
        //                        ID = user.ID,
        //                        UserName = emp.Name,
        //                        UserID=emp.ID,
        //                        Position=emp.Position,
        //                        Branch=br.Name,
        //                        Action=""
        //                    }
        //                    ).ToList();
        //    return Ok(datauser);
        //}
        public IActionResult GetDataUser()
        {
            var datauser = (from user in _context.UserAccounts
                            let emp = _context.Employees.Where(x => x.ID == user.EmployeeID).FirstOrDefault() ?? new Employee()
                            let br = _context.Branches.Where(x => x.ID == user.BranchID).FirstOrDefault() ?? new Models.Services.Administrator.General.Branch()
                            select new ActivityView
                            {
                                ID = emp.ID,
                                UserName = emp.Name,
                                UserID = emp.ID,
                                Position = emp.Position,
                                Branch = br.Name,
                                Action = ""
                            }
                            ).ToList();
            return Ok(datauser);
        }
        public IActionResult GetDataReportActivity()
        {
            var data = (from act in _context.Activites
                        join g in _context.Generals on act.ID equals g.ActivityID
                        let bp = _context.BusinessPartners.Where(x => x.ID == act.BPID).FirstOrDefault() ?? new BusinessPartner()
                        let user = _context.Employees.Where(x => x.ID == act.UserID).FirstOrDefault() ?? new Employee()
                        let st = _context.SetupStatuses.FirstOrDefault(x => x.ID == g.StatusID) ?? new SetupStatus()
                        select new ActivityView
                        {
                            ID = act.ID,
                            GID = g.ID,
                            BPID = bp.ID,
                            Number = act.Number,
                            StartTime = g.StartTime,
                            StartTimes = g.StartTime.ToString("HH:mm"),
                            SetActName = act.Activities.ToString(),
                            EmpName = user.Name,
                            Recurrences = g.Recurrence,
                            BpName = bp.Name,
                            StatusID = g.StatusID,
                            Status = st.Status,
                            Remark = g.Remark,
                            AssignedBy = user.Name
                        }
                        ).ToList();
            return Ok(data);
        }
        public IActionResult GetDataFilterReportActivity(string strObject)
        {
            SearchBPOppParams datas = JsonConvert.DeserializeObject<SearchBPOppParams>(strObject, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });
            if (datas.BpID1 != 0 && datas.BpID2 != 0 && datas.Employee.Count <= 0)
            {
                //var list = _context.Activites.Where(x => BpID1 <= x.BPID && x.BPID <= BpID2
                //   && datas.Any(_x => _x.UserID == x.UserID)).ToList();
                var data = (from act in _context.Activites.Where(x => datas.BpID1 <= x.BPID && x.BPID <= datas.BpID2)
                            join g in _context.Generals on act.ID equals g.ActivityID
                            let bp = _context.BusinessPartners.Where(x => x.ID == act.BPID).FirstOrDefault() ?? new Models.Services.HumanResources.BusinessPartner()
                            let user = _context.Employees.Where(x => x.ID == act.UserID).FirstOrDefault() ?? new Models.Services.HumanResources.Employee()
                            let st = _context.SetupStatuses.FirstOrDefault(x => x.ID == g.StatusID) ?? new SetupStatus()

                            select new ActivityView
                            {
                                ID = act.ID,
                                GID = g.ID,
                                BPID = bp.ID,
                                Number = act.Number,
                                StartTime = g.StartTime,
                                StartTimes = g.StartTime.ToString("HH:mm"),
                                //SetActName = act.SetupActName,
                                EmpName = user.Name,
                                Recurrences = g.Recurrence,
                                BpName = bp.Name,
                                StatusID = g.StatusID,
                                Status = st.Status,
                                Remark = g.Remark,
                                AssignedBy = user.Name
                            }
                         ).ToList();
                return Ok(data);
            }
            else if (datas.Employee.Count > 0 && datas.BpID1 == 0 && datas.BpID2 == 0)
            {

                //var datas = JsonConvert.DeserializeObject<List<Activity>>(Employee, new JsonSerializerSettings
                //{
                //    NullValueHandling = NullValueHandling.Ignore
                //});
                List<Employee> employee = new();
                foreach (var items in datas.Employee)
                {
                    var employees = _context.Employees.Where(i => i.ID == items.ID).ToList();
                    if (employees.Count > 0) employee.AddRange(employees);
                }
                //var list = _context.Activites.Where(x => datas.Any(_x => _x.UserID == x.UserID)).ToList();
                var data = (from emp in employee
                            let act = _context.Activites.Where(x => x.UserID == emp.ID).FirstOrDefault() ?? new Activity()
                            join g in _context.Generals on act.ID equals g.ActivityID
                            let bp = _context.BusinessPartners.Where(x => x.ID == act.BPID).FirstOrDefault() ?? new Models.Services.HumanResources.BusinessPartner()
                            let user = _context.Employees.Where(x => x.ID == act.UserID).FirstOrDefault() ?? new Models.Services.HumanResources.Employee()
                            let st = _context.SetupStatuses.FirstOrDefault(x => x.ID == g.StatusID) ?? new SetupStatus()

                            select new ActivityView
                            {
                                ID = act.ID,
                                GID = g.ID,
                                BPID = bp.ID,
                                Number = act.Number,
                                StartTime = g.StartTime,
                                StartTimes = g.StartTime.ToString("HH:mm"),
                                //SetActName = act.SetupActName,
                                EmpName = user.Name,
                                Recurrences = g.Recurrence,
                                BpName = bp.Name,
                                StatusID = g.StatusID,
                                Status = st.Status,
                                Remark = g.Remark,
                                AssignedBy = user.Name
                            }
                         ).ToList();
                return Ok(data);
            }
            else if (datas.BpID1 != 0 && datas.BpID2 != 0 && datas.Employee.Count > 0)
            {

                //var datas = JsonConvert.DeserializeObject<List<Activity>>(Employee, new JsonSerializerSettings
                //{
                //    NullValueHandling = NullValueHandling.Ignore
                //});
                List<Employee> employee = new();
                foreach (var items in datas.Employee)
                {
                    var employees = _context.Employees.Where(i => i.ID == items.ID).ToList();
                    if (employees.Count > 0) employee.AddRange(employees);
                }
                //var list = _context.Activites.Where(x => datas.Any(_x => _x.UserID == x.UserID)).ToList();
                var data = (from emp in employee
                            let act = _context.Activites.Where(x => x.UserID == emp.ID && datas.BpID1 <= x.BPID && x.BPID <= datas.BpID2).FirstOrDefault() ?? new Activity()
                            join g in _context.Generals on act.ID equals g.ActivityID
                            let bp = _context.BusinessPartners.Where(x => x.ID == act.BPID).FirstOrDefault() ?? new Models.Services.HumanResources.BusinessPartner()
                            let user = _context.Employees.Where(x => x.ID == act.UserID).FirstOrDefault() ?? new Models.Services.HumanResources.Employee()
                            let st = _context.SetupStatuses.FirstOrDefault(x => x.ID == g.StatusID) ?? new SetupStatus()
                            select new ActivityView
                            {
                                ID = act.ID,
                                GID = g.ID,
                                BPID = bp.ID,
                                Number = act.Number,
                                StartTime = g.StartTime,
                                StartTimes = g.StartTime.ToString("HH:mm"),
                                //SetActName = act.SetupActName,
                                EmpName = user.Name,
                                Recurrences = g.Recurrence,
                                BpName = bp.Name,
                                StatusID = g.StatusID,
                                Status = st.Status,
                                Remark = g.Remark,
                                AssignedBy = user.Name
                            }
                         ).ToList();
                return Ok(data);
            }
            return Ok();

        }
        //==================End Activity Overview Report=========
        public IActionResult GetCusAging(string DateFrom, string DateTo, bool DeplayRe)
        {
            List<SaleAR> saleARs = new();
            List<SaleAREdite> saleAREdites = new();
            List<SaleCreditMemo> saleCredits = new();
            List<IncomingPayment> incomings = new();
            List<Receipt> receipts = new();
            List<ReceiptMemo> receiptMemos = new();
            List<ARReserveInvoice> arrINV = new();
            List<ARReserveInvoiceEditable> arrEDT = new();
            if (DateFrom == null && DateTo == null && DeplayRe == false)
            {
                saleARs = _context.SaleARs.Where(w => w.Status == "open").ToList();
                saleAREdites = _context.SaleAREdites.Where(s => s.Status == "open").ToList();
                receipts = _context.Receipt.Where(w => w.GrandTotal != (double)w.AppliedAmount && w.Status == StatusReceipt.Aging).ToList();
                saleCredits = _context.SaleCreditMemos.Where(w => w.Status == "open").ToList();
                arrEDT = _context.ARReserveInvoiceEditables.Where(w => w.Status == "open").ToList();
                arrINV = _context.ARReserveInvoices.Where(w => w.Status == "open").ToList();

            }
            else if (DateFrom == null && DateTo == null && DeplayRe == true)
            {
                saleARs = _context.SaleARs.ToList();
                saleAREdites = _context.SaleAREdites.ToList();
                receipts = _context.Receipt.Where(w => w.Status == StatusReceipt.Aging).ToList();
                receiptMemos = _context.ReceiptMemo.Where(w => w.Status == StatusReceipt.Aging).ToList();
                incomings = _context.IncomingPayments.ToList();
                saleCredits = _context.SaleCreditMemos.ToList();
                arrEDT = _context.ARReserveInvoiceEditables.ToList();
                arrINV = _context.ARReserveInvoices.ToList();
            }
            else if (DateFrom != null && DateTo != null && DeplayRe == true)
            {
                saleARs = _context.SaleARs.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo)).ToList();
                saleAREdites = _context.SaleAREdites.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo)).ToList();
                receipts = _context.Receipt.Where(w => w.DateOut >= Convert.ToDateTime(DateFrom) && w.DateOut <= Convert.ToDateTime(DateTo) && w.Status == StatusReceipt.Aging).ToList();
                receiptMemos = _context.ReceiptMemo.Where(w => w.DateOut >= Convert.ToDateTime(DateFrom) && w.DateOut <= Convert.ToDateTime(DateTo) && w.Status == StatusReceipt.Aging).ToList();
                saleCredits = _context.SaleCreditMemos.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo)).ToList();
                incomings = _context.IncomingPayments.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo)).ToList();

                arrEDT = _context.ARReserveInvoiceEditables.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo)).ToList();
                arrINV = _context.ARReserveInvoices.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo)).ToList();
            }
            else if (DateFrom != null && DateTo != null && DeplayRe == false)
            {
                saleAREdites = _context.SaleAREdites.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.Status == "open").ToList();
                saleARs = _context.SaleARs.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.Status == "open").ToList();
                receipts = _context.Receipt.Where(w => w.DateOut >= Convert.ToDateTime(DateFrom) && w.DateOut <= Convert.ToDateTime(DateTo) && w.GrandTotal != (double)w.AppliedAmount && w.Status == StatusReceipt.Aging).ToList();
                saleCredits = _context.SaleCreditMemos.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.Status == "open").ToList(); ;
                arrEDT = _context.ARReserveInvoiceEditables.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.Status == "open").ToList();
                arrINV = _context.ARReserveInvoices.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.Status == "open").ToList();
            }
            //SaleRA
            List<BPAging> ARsreceipt = new();
            List<BPAging> AREditreceipt = new();
            List<BPAging> CreditMemo = new();
            List<BPAging> Receipt = new();
            List<BPAging> ARREDT = new();
            List<BPAging> ARRINV = new();
            if (DeplayRe == true)
            {
                Receipt = (from re in receipts
                           join cus in _context.BusinessPartners on re.CustomerID equals cus.ID
                           join curr in _context.Currency on re.PLCurrencyID equals curr.ID
                           join scurr in _context.Currency on re.SysCurrencyID equals scurr.ID
                           let douType = _context.DocumentTypes.FirstOrDefault(w => w.Code == "SP")
                           select new BPAging
                           {
                               SeriesDID = re.SeriesDID,
                               SaleARID = re.ReceiptID,
                               InvoiceNo = re.ReceiptNo,
                               DouType = douType.Code,
                               PostingDate = re.DateOut.ToString("dd-MM-yyyy"),
                               DueDate = re.DateOut.ToString("dd-MM-yyyy"),
                               RefNo = "",
                               CustomerCode = $"{cus.Code} - {cus.Name}",
                               CustomerName = cus.Name,
                               TotalAmount = re.GrandTotal - (double)re.AppliedAmount,
                               BalanceDue = curr.Description + "   " + string.Format("{0:#,0.000}", re.GrandTotal),
                               SBalanceDue = re.Change * (-1),
                               Currencysys = scurr.Description,
                           }).ToList();
                AREditreceipt = (from ar in saleAREdites
                                 join cus in _context.BusinessPartners on ar.CusID equals cus.ID
                                 join douType in _context.DocumentTypes on ar.DocTypeID equals douType.ID
                                 join curr in _context.Currency on ar.SaleCurrencyID equals curr.ID
                                 join com in _context.Company on ar.CompanyID equals com.ID
                                 join scurr in _context.Currency on com.SystemCurrencyID equals scurr.ID
                                 select new BPAging
                                 {
                                     SeriesDID = ar.SeriesDID,
                                     SaleARID = ar.SARID,
                                     InvoiceNo = ar.InvoiceNo,
                                     DouType = douType.Code,
                                     PostingDate = ar.PostingDate.ToString("dd-MM-yyyy"),
                                     DueDate = ar.DocumentDate.ToString("dd-MM-yyyy"),
                                     RefNo = "",
                                     CustomerCode = $"{cus.Code} - {cus.Name}",
                                     CustomerName = cus.Name,
                                     TotalAmount = ar.TotalAmount,
                                     BalanceDue = curr.Description + "   " + string.Format("{0:#,0.000}", ar.TotalAmount - ar.AppliedAmount),
                                     SBalanceDue = ar.TotalAmountSys,
                                     Currencysys = scurr.Description,
                                 }).ToList();
                ARsreceipt = (from ar in saleARs
                              join cus in _context.BusinessPartners on ar.CusID equals cus.ID
                              join douType in _context.DocumentTypes on ar.DocTypeID equals douType.ID
                              join curr in _context.Currency on ar.SaleCurrencyID equals curr.ID
                              join com in _context.Company on ar.CompanyID equals com.ID
                              join scurr in _context.Currency on com.SystemCurrencyID equals scurr.ID
                              select new BPAging
                              {
                                  SeriesDID = ar.SeriesDID,
                                  SaleARID = ar.SARID,
                                  InvoiceNo = ar.InvoiceNo,
                                  DouType = douType.Code,
                                  PostingDate = ar.PostingDate.ToString("dd-MM-yyyy"),
                                  DueDate = ar.DocumentDate.ToString("dd-MM-yyyy"),
                                  RefNo = "",
                                  CustomerCode = $"{cus.Code} - {cus.Name}",
                                  CustomerName = cus.Name,
                                  TotalAmount = ar.TotalAmount,
                                  BalanceDue = curr.Description + "   " + string.Format("{0:#,0.000}", ar.TotalAmount - ar.AppliedAmount),
                                  SBalanceDue = ar.TotalAmountSys,
                                  Currencysys = scurr.Description,
                              }).ToList();
                CreditMemo = (from ar in saleCredits

                              join cus in _context.BusinessPartners on ar.CusID equals cus.ID
                              join douType in _context.DocumentTypes on ar.DocTypeID equals douType.ID
                              join curr in _context.Currency on ar.SaleCurrencyID equals curr.ID
                              join com in _context.Company on ar.CompanyID equals com.ID
                              join scurr in _context.Currency on com.SystemCurrencyID equals scurr.ID
                              select new BPAging
                              {
                                  SeriesDID = ar.SeriesDID,
                                  SaleARID = ar.SCMOID,
                                  InvoiceNo = ar.InvoiceNo,
                                  DouType = douType.Code,
                                  PostingDate = ar.PostingDate.ToString("dd-MM-yyyy"),
                                  DueDate = ar.DocumentDate.ToString("dd-MM-yyyy"),
                                  RefNo = "",
                                  CustomerCode = $"{cus.Code} - {cus.Name}",
                                  CustomerName = cus.Name,
                                  TotalAmount = ar.TotalAmount * -1,
                                  BalanceDue = curr.Description + "   " + string.Format("{0:#,0.000}", (ar.TotalAmount * -1)),
                                  SBalanceDue = ar.TotalAmountSys * -1,
                                  Currencysys = scurr.Description,
                              }).ToList();
                ARRINV = (from ar in arrINV
                          join cus in _context.BusinessPartners on ar.CusID equals cus.ID
                          join douType in _context.DocumentTypes on ar.DocTypeID equals douType.ID
                          join curr in _context.Currency on ar.SaleCurrencyID equals curr.ID
                          join com in _context.Company on ar.CompanyID equals com.ID
                          join scurr in _context.Currency on com.SystemCurrencyID equals scurr.ID
                          select new BPAging
                          {
                              SeriesDID = ar.SeriesDID,
                              SaleARID = ar.ID,
                              InvoiceNo = ar.InvoiceNo,
                              DouType = douType.Code,
                              PostingDate = ar.PostingDate.ToString("dd-MM-yyyy"),
                              DueDate = ar.DocumentDate.ToString("dd-MM-yyyy"),
                              RefNo = "",
                              CustomerCode = $"{cus.Code} - {cus.Name}",
                              CustomerName = cus.Name,
                              TotalAmount = ar.TotalAmount,
                              BalanceDue = curr.Description + "   " + string.Format("{0:#,0.000}", ar.TotalAmount - ar.AppliedAmount),
                              SBalanceDue = ar.TotalAmountSys,
                              Currencysys = scurr.Description,
                          }).ToList();
                ARREDT = (from ar in arrEDT
                          join cus in _context.BusinessPartners on ar.CusID equals cus.ID
                          join douType in _context.DocumentTypes on ar.DocTypeID equals douType.ID
                          join curr in _context.Currency on ar.SaleCurrencyID equals curr.ID
                          join com in _context.Company on ar.CompanyID equals com.ID
                          join scurr in _context.Currency on com.SystemCurrencyID equals scurr.ID
                          select new BPAging
                          {
                              SeriesDID = ar.SeriesDID,
                              SaleARID = ar.ID,
                              InvoiceNo = ar.InvoiceNo,
                              DouType = douType.Code,
                              PostingDate = ar.PostingDate.ToString("dd-MM-yyyy"),
                              DueDate = ar.DocumentDate.ToString("dd-MM-yyyy"),
                              RefNo = "",
                              CustomerCode = $"{cus.Code} - {cus.Name}",
                              CustomerName = cus.Name,
                              TotalAmount = ar.TotalAmount,
                              BalanceDue = curr.Description + "   " + string.Format("{0:#,0.000}", ar.TotalAmount - ar.AppliedAmount),
                              SBalanceDue = ar.TotalAmountSys,
                              Currencysys = scurr.Description,
                          }).ToList();
            }
            else
            {
                Receipt = (from re in receipts
                           join cus in _context.BusinessPartners on re.CustomerID equals cus.ID
                           join curr in _context.Currency on re.PLCurrencyID equals curr.ID
                           join scurr in _context.Currency on re.SysCurrencyID equals scurr.ID
                           let douType = _context.DocumentTypes.FirstOrDefault(w => w.Code == "SP")
                           select new BPAging
                           {
                               SeriesDID = re.SeriesDID,
                               SaleARID = re.ReceiptID,
                               InvoiceNo = re.ReceiptNo,
                               DouType = douType.Code,
                               PostingDate = re.DateOut.ToString("dd-MM-yyyy"),
                               DueDate = re.DateOut.ToString("dd-MM-yyyy"),
                               RefNo = "",
                               CustomerCode = $"{cus.Code} - {cus.Name}",
                               CustomerName = cus.Name,
                               TotalAmount = re.GrandTotal - (double)re.AppliedAmount,
                               BalanceDue = curr.Description + "   " + string.Format("{0:#,0.000}", (re.GrandTotal - (double)re.AppliedAmount)),
                               SBalanceDue = re.GrandTotal_Sys - ((double)re.AppliedAmount * re.PLRate),
                               Currencysys = scurr.Description,
                           }).ToList();
                AREditreceipt = (from ar in saleAREdites
                                 join cus in _context.BusinessPartners on ar.CusID equals cus.ID
                                 join douType in _context.DocumentTypes on ar.DocTypeID equals douType.ID
                                 join curr in _context.Currency on ar.SaleCurrencyID equals curr.ID
                                 join com in _context.Company on ar.CompanyID equals com.ID
                                 join scurr in _context.Currency on com.SystemCurrencyID equals scurr.ID
                                 select new BPAging
                                 {
                                     SeriesDID = ar.SeriesDID,
                                     SaleARID = ar.SARID,
                                     InvoiceNo = ar.InvoiceNo,
                                     DouType = douType.Code,
                                     PostingDate = ar.PostingDate.ToString("dd-MM-yyyy"),
                                     DueDate = ar.DocumentDate.ToString("dd-MM-yyyy"),
                                     RefNo = "",
                                     CustomerCode = $"{cus.Code} - {cus.Name}",
                                     CustomerName = cus.Name,
                                     TotalAmount = ar.TotalAmount,
                                     BalanceDue = curr.Description + "   " + string.Format("{0:#,0.000}", ar.TotalAmount - ar.AppliedAmount),
                                     SBalanceDue = ar.TotalAmountSys - ar.AppliedAmount * ar.ExchangeRate,
                                     Currencysys = scurr.Description,
                                 }).ToList();
                ARsreceipt = (from ar in saleARs
                              join cus in _context.BusinessPartners on ar.CusID equals cus.ID
                              join douType in _context.DocumentTypes on ar.DocTypeID equals douType.ID
                              join curr in _context.Currency on ar.SaleCurrencyID equals curr.ID
                              join com in _context.Company on ar.CompanyID equals com.ID
                              join scurr in _context.Currency on com.SystemCurrencyID equals scurr.ID
                              select new BPAging
                              {
                                  SeriesDID = ar.SeriesDID,
                                  SaleARID = ar.SARID,
                                  InvoiceNo = ar.InvoiceNo,
                                  DouType = douType.Code,
                                  PostingDate = ar.PostingDate.ToString("dd-MM-yyyy"),
                                  DueDate = ar.DocumentDate.ToString("dd-MM-yyyy"),
                                  RefNo = "",
                                  CustomerCode = $"{cus.Code} - {cus.Name}",
                                  CustomerName = cus.Name,
                                  TotalAmount = ar.TotalAmount,
                                  BalanceDue = curr.Description + "   " + string.Format("{0:#,0.000}", ar.TotalAmount - ar.AppliedAmount),
                                  SBalanceDue = ar.TotalAmountSys - ar.AppliedAmount * ar.ExchangeRate,
                                  Currencysys = scurr.Description,
                              }).ToList();
                CreditMemo = (from ar in saleCredits
                              join ipc in _context.IncomingPaymentCustomers on ar.SeriesID equals ipc.SeriesID
                              join cus in _context.BusinessPartners on ar.CusID equals cus.ID
                              join douType in _context.DocumentTypes on ar.DocTypeID equals douType.ID
                              join curr in _context.Currency on ar.SaleCurrencyID equals curr.ID
                              join com in _context.Company on ar.CompanyID equals com.ID
                              join scurr in _context.Currency on com.SystemCurrencyID equals scurr.ID
                              select new BPAging
                              {
                                  SeriesDID = ar.SeriesDID,
                                  SaleARID = ar.SCMOID,
                                  InvoiceNo = ar.InvoiceNo,
                                  DouType = douType.Code,
                                  PostingDate = ar.PostingDate.ToString("dd-MM-yyyy"),
                                  DueDate = ar.DocumentDate.ToString("dd-MM-yyyy"),
                                  RefNo = "",
                                  CustomerCode = $"{cus.Code} - {cus.Name}",
                                  CustomerName = cus.Name,
                                  TotalAmount = ar.TotalAmount * -1,
                                  BalanceDue = curr.Description + "   " + string.Format("{0:#,0.000}", ar.TotalAmount * (-1)),
                                  SBalanceDue = (ar.TotalAmountSys) * (-1),
                                  Currencysys = scurr.Description,
                              }).ToList();


                ARRINV = (from ar in arrINV
                          join cus in _context.BusinessPartners on ar.CusID equals cus.ID
                          join douType in _context.DocumentTypes on ar.DocTypeID equals douType.ID
                          join curr in _context.Currency on ar.SaleCurrencyID equals curr.ID
                          join com in _context.Company on ar.CompanyID equals com.ID
                          join scurr in _context.Currency on com.SystemCurrencyID equals scurr.ID
                          select new BPAging
                          {
                              SeriesDID = ar.SeriesDID,
                              SaleARID = ar.ID,
                              InvoiceNo = ar.InvoiceNo,
                              DouType = douType.Code,
                              PostingDate = ar.PostingDate.ToString("dd-MM-yyyy"),
                              DueDate = ar.DocumentDate.ToString("dd-MM-yyyy"),
                              RefNo = "",
                              CustomerCode = $"{cus.Code} - {cus.Name}",
                              CustomerName = cus.Name,
                              TotalAmount = ar.TotalAmount,
                              BalanceDue = curr.Description + "   " + string.Format("{0:#,0.000}", ar.TotalAmount - ar.AppliedAmount),
                              SBalanceDue = ar.TotalAmountSys - ar.AppliedAmount * ar.ExchangeRate,
                              Currencysys = scurr.Description,
                          }).ToList();
                ARREDT = (from ar in arrEDT
                          join cus in _context.BusinessPartners on ar.CusID equals cus.ID
                          join douType in _context.DocumentTypes on ar.DocTypeID equals douType.ID
                          join curr in _context.Currency on ar.SaleCurrencyID equals curr.ID
                          join com in _context.Company on ar.CompanyID equals com.ID
                          join scurr in _context.Currency on com.SystemCurrencyID equals scurr.ID
                          select new BPAging
                          {
                              SeriesDID = ar.SeriesDID,
                              SaleARID = ar.ID,
                              InvoiceNo = ar.InvoiceNo,
                              DouType = douType.Code,
                              PostingDate = ar.PostingDate.ToString("dd-MM-yyyy"),
                              DueDate = ar.DocumentDate.ToString("dd-MM-yyyy"),
                              RefNo = "",
                              CustomerCode = $"{cus.Code} - {cus.Name}",
                              CustomerName = cus.Name,
                              TotalAmount = ar.TotalAmount,
                              BalanceDue = curr.Description + "   " + string.Format("{0:#,0.000}", ar.TotalAmount - ar.AppliedAmount),
                              SBalanceDue = ar.TotalAmountSys - ar.AppliedAmount * ar.ExchangeRate,
                              Currencysys = scurr.Description,
                          }).ToList();
            }
            var ReceiptMemo = (from re in receiptMemos
                               join cus in _context.BusinessPartners on re.CustomerID equals cus.ID
                               join curr in _context.Currency on re.PLCurrencyID equals curr.ID
                               join scurr in _context.Currency on re.SysCurrencyID equals scurr.ID
                               let douType = _context.DocumentTypes.FirstOrDefault(w => w.Code == "RP")
                               select new BPAging
                               {
                                   SeriesDID = re.SeriesDID,
                                   SaleARID = re.ID,
                                   InvoiceNo = re.ReceiptNo,
                                   DouType = douType.Code,
                                   PostingDate = re.DateOut.ToString("dd-MM-yyyy"),
                                   DueDate = re.DateOut.ToString("dd-MM-yyyy"),
                                   RefNo = "",
                                   CustomerCode = $"{cus.Code} - {cus.Name}",
                                   CustomerName = cus.Name,
                                   TotalAmount = re.GrandTotal * -1,
                                   BalanceDue = curr.Description + "   " + string.Format("{0:#,0.000}", (re.GrandTotal) * -1),
                                   SBalanceDue = re.GrandTotalSys * -1,
                                   Currencysys = scurr.Description,
                               }).ToList();
            var incomSaleAR = (from re in incomings
                               join cus in _context.BusinessPartners on re.CustomerID equals cus.ID
                               join douType in _context.DocumentTypes on re.DocTypeID equals douType.ID
                               join com in _context.Company on re.CompanyID equals com.ID
                               join scurr in _context.Currency on com.SystemCurrencyID equals scurr.ID
                               where re.TotalApplied > 0
                               select new BPAging
                               {
                                   SeriesDID = re.SeriesDID,
                                   SaleARID = re.IncomingPaymentID,
                                   InvoiceNo = re.InvoiceNumber,
                                   DouType = douType.Code,
                                   PostingDate = re.PostingDate.ToString("dd-MM-yyyy"),
                                   DueDate = re.DocumentDate.ToString("dd-MM-yyyy"),
                                   RefNo = "",
                                   CustomerCode = $"{cus.Code} - {cus.Name}",
                                   TotalAmount = (double)re.TotalApplied * -1,
                                   BalanceDue = scurr.Description + string.Format("{0:#,0.000}", re.TotalApplied * -1),
                                   SBalanceDue = (double)re.TotalApplied * -1,
                                   Currencysys = scurr.Description,
                               }).ToList();
            var incomSaleARs = (from re in incomings
                                join cus in _context.BusinessPartners on re.CustomerID equals cus.ID
                                join douType in _context.DocumentTypes on re.DocTypeID equals douType.ID
                                join com in _context.Company on re.CompanyID equals com.ID
                                join scurr in _context.Currency on com.SystemCurrencyID equals scurr.ID
                                where re.TotalApplied < 0
                                select new BPAging
                                {
                                    SeriesDID = re.SeriesDID,
                                    SaleARID = re.IncomingPaymentID,
                                    InvoiceNo = re.InvoiceNumber,
                                    DouType = douType.Code,
                                    PostingDate = re.PostingDate.ToString("dd-MM-yyyy"),
                                    DueDate = re.DocumentDate.ToString("dd-MM-yyyy"),
                                    RefNo = "",
                                    CustomerCode = $"{cus.Code} - {cus.Name}",
                                    CustomerName = cus.Name,
                                    TotalAmount = (double)re.TotalApplied * -1,
                                    BalanceDue = scurr.Description + string.Format("{0:#,0.000}", re.TotalApplied),
                                    SBalanceDue = (double)re.TotalApplied,
                                    Currencysys = scurr.Description,
                                }).ToList();

            var allsale = new List<BPAging>
            (incomSaleAR.Count + incomSaleARs.Count + ARsreceipt.Count + CreditMemo.Count + Receipt.Count + ReceiptMemo.Count + AREditreceipt.Count + ARRINV.Count + ARREDT.Count);
            allsale.AddRange(incomSaleAR);
            allsale.AddRange(incomSaleARs);
            allsale.AddRange(ARsreceipt);
            allsale.AddRange(CreditMemo);
            allsale.AddRange(Receipt);
            allsale.AddRange(ReceiptMemo);
            allsale.AddRange(AREditreceipt);
            allsale.AddRange(ARRINV);
            allsale.AddRange(ARREDT);
            var allSummarySales = (from all in allsale
                                   select new
                                   {
                                       all.SeriesDID,
                                       all.DouType,
                                       all.CustomerCode,
                                       all.CustomerName,
                                       all.PostingDate,
                                       all.DueDate,
                                       all.InvoiceNo,
                                       all.BalanceDue,
                                       SBalanceDue = Convert.ToDouble(all.SBalanceDue),
                                       TotalBalance = all.Currencysys + " " + allsale.Where(i => i.CustomerCode == all.CustomerCode).Sum(i => i.SBalanceDue).ToString("0,0.000"),
                                       GrandTotal = all.Currencysys + " " + allsale.Sum(s => s.SBalanceDue).ToString("0,0.000"),
                                   });
            return Ok(allSummarySales.OrderBy(o => o.SeriesDID));
        }

        public IActionResult GetVenAging(string DateFrom, string DateTo, bool DeplayRe)
        {
            List<PurchaseCreditMemo> purchaseCreditMemos = new();
            List<OutgoingPayment> outgoingPayments = new();
            List<Purchase_AP> purchase_APs = new();
            List<PurchaseAPReserve> purchase_APRS = new();
            if (DateFrom == null && DateTo == null && DeplayRe == false)
            {
                purchase_APs = _context.Purchase_APs.Where(w => w.Status == "open").ToList();
                purchaseCreditMemos = _context.PurchaseCreditMemos.Where(w => w.Status == "open").ToList();
                purchase_APRS = _context.PurchaseAPReserves.Where(w => w.Status == "open").ToList();
            }
            else if (DateFrom == null && DateTo == null && DeplayRe == true)
            {
                purchase_APs = _context.Purchase_APs.ToList();
                outgoingPayments = _context.OutgoingPayments.ToList();
                purchaseCreditMemos = _context.PurchaseCreditMemos.ToList();
                purchase_APRS = _context.PurchaseAPReserves.ToList();
            }
            else if (DateFrom != null && DateTo != null && DeplayRe == true)
            {
                purchase_APs = _context.Purchase_APs.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo)).ToList();
                outgoingPayments = _context.OutgoingPayments.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo)).ToList();
                purchaseCreditMemos = _context.PurchaseCreditMemos.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo)).ToList();
                purchase_APRS = _context.PurchaseAPReserves.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo)).ToList();
            }
            else if (DateFrom != null && DateTo != null && DeplayRe == false)
            {
                purchase_APs = _context.Purchase_APs.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.Status == "open").ToList();
                purchaseCreditMemos = _context.PurchaseCreditMemos.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.Status == "open").ToList();
                purchase_APRS = _context.PurchaseAPReserves.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.Status == "open").ToList();
            }
            List<BPAging> purchaseAP = new();
            List<BPAging> purchaseMemo = new();
            List<BPAging> purchaseAPRS = new();
            if (DeplayRe == true)
            {
                purchaseAP = (from ap in purchase_APs
                              join cus in _context.BusinessPartners on ap.VendorID equals cus.ID
                              join douType in _context.DocumentTypes on ap.DocumentTypeID equals douType.ID
                              join curr in _context.Currency on ap.PurCurrencyID equals curr.ID
                              join scurr in _context.Currency on ap.SysCurrencyID equals scurr.ID
                              select new BPAging
                              {
                                  SeriesDID = ap.SeriesDetailID,
                                  PurchaseAPID = ap.PurchaseAPID,
                                  InvoiceNo = ap.InvoiceNo,
                                  DouType = douType.Code,
                                  PostingDate = ap.PostingDate.ToString("dd-MM-yyyy"),
                                  DueDate = ap.DueDate.ToString("dd-MM-yyyy"),
                                  RefNo = ap.ReffNo,
                                  VendorCode = $"{cus.Code} - {cus.Name}",
                                  CustomerName = cus.Name,
                                  BalanceDue = curr.Description + "   " + string.Format("{0:#,0.000}", ap.SubTotal * (-1)),
                                  SBalanceDue = Convert.ToDouble(ap.SubTotalSys) * (-1),
                                  Currencysys = scurr.Description,
                              }).ToList();
                purchaseMemo = (from po in purchaseCreditMemos
                                join bs in _context.BusinessPartners on po.VendorID equals bs.ID
                                join DouType in _context.DocumentTypes on po.DocumentTypeID equals DouType.ID
                                join curr in _context.Currency on po.PurchaseMemoID equals curr.ID
                                join scurr in _context.Currency on po.SysCurrencyID equals scurr.ID
                                select new BPAging
                                {
                                    SeriesDID = po.SeriesDetailID,
                                    PurchaseMemoID = po.PurchaseMemoID,
                                    InvoiceNo = po.InvoiceNo,
                                    DouType = DouType.Code,
                                    PostingDate = po.PostingDate.ToString("dd-MM-yyyy"),
                                    DueDate = po.DueDate.ToString("dd-MM-yyyy"),
                                    RefNo = po.ReffNo,
                                    VendorCode = $"{bs.Code} - {bs.Name}",
                                    VendorName = bs.Name,
                                    BalanceDue = curr.Description + "   " + string.Format("{0:#,0.000}", po.SubTotal),
                                    SBalanceDue = Convert.ToDouble(po.SubTotalSys),
                                    Currencysys = scurr.Description
                                }).ToList();
                purchaseAPRS = (from ap in purchase_APRS
                                join cus in _context.BusinessPartners on ap.VendorID equals cus.ID
                                join douType in _context.DocumentTypes on ap.DocumentTypeID equals douType.ID
                                join curr in _context.Currency on ap.PurCurrencyID equals curr.ID
                                join scurr in _context.Currency on ap.SysCurrencyID equals scurr.ID
                                select new BPAging
                                {
                                    SeriesDID = ap.SeriesDetailID,
                                    PurchaseAPID = ap.ID,
                                    InvoiceNo = ap.InvoiceNo,
                                    DouType = douType.Code,
                                    PostingDate = ap.PostingDate.ToString("dd-MM-yyyy"),
                                    DueDate = ap.DueDate.ToString("dd-MM-yyyy"),
                                    RefNo = ap.ReffNo,
                                    VendorCode = $"{cus.Code} - {cus.Name}",
                                    CustomerName = cus.Name,
                                    BalanceDue = curr.Description + "   " + string.Format("{0:#,0.000}", ap.SubTotal * (-1)),
                                    SBalanceDue = Convert.ToDouble(ap.SubTotalSys) * (-1),
                                    Currencysys = scurr.Description,
                                }).ToList();
            }
            else
            {
                purchaseAP = (from ap in purchase_APs
                              join cus in _context.BusinessPartners on ap.VendorID equals cus.ID
                              join douType in _context.DocumentTypes on ap.DocumentTypeID equals douType.ID
                              join curr in _context.Currency on ap.PurCurrencyID equals curr.ID
                              join scurr in _context.Currency on ap.SysCurrencyID equals scurr.ID
                              select new BPAging
                              {
                                  SeriesDID = ap.SeriesDetailID,
                                  PurchaseAPID = ap.PurchaseAPID,
                                  InvoiceNo = ap.InvoiceNo,
                                  DouType = douType.Code,
                                  PostingDate = ap.PostingDate.ToString("dd-MM-yyyy"),
                                  DueDate = ap.DueDate.ToString("dd-MM-yyyy"),
                                  RefNo = ap.ReffNo,
                                  VendorCode = $"{cus.Code} - {cus.Name}",
                                  VendorName = cus.Name,
                                  BalanceDue = curr.Description + "   " + string.Format("{0:#,0.000}", ap.BalanceDue * (-1)),
                                  SBalanceDue = (ap.BalanceDue * ap.PurRate) * (-1),
                                  Currencysys = scurr.Description,
                              }).ToList();
                purchaseMemo = (from po in purchaseCreditMemos
                                join bs in _context.BusinessPartners on po.VendorID equals bs.ID
                                join DouType in _context.DocumentTypes on po.DocumentTypeID equals DouType.ID
                                join curr in _context.Currency on po.PurchaseMemoID equals curr.ID
                                join scurr in _context.Currency on po.SysCurrencyID equals scurr.ID
                                select new BPAging
                                {
                                    SeriesDID = po.SeriesDetailID,
                                    PurchaseMemoID = po.PurchaseMemoID,
                                    InvoiceNo = po.InvoiceNo,
                                    DouType = DouType.Code,
                                    PostingDate = po.PostingDate.ToString("dd-MM-yyyy"),
                                    DueDate = po.DueDate.ToString("dd-MM-yyyy"),
                                    RefNo = po.ReffNo,
                                    VendorCode = $"{bs.Code} - {bs.Name}",
                                    VendorName = bs.Name,
                                    BalanceDue = curr.Description + "   " + string.Format("{0:#,0.000}", (po.SubTotal - po.AppliedAmount) * -1),
                                    SBalanceDue = (po.SubTotalSys - (po.AppliedAmount * po.PurRate) * -1),
                                    Currencysys = scurr.Description,
                                }).ToList();

                purchaseAPRS = (from ap in purchase_APRS
                                join cus in _context.BusinessPartners on ap.VendorID equals cus.ID
                                join douType in _context.DocumentTypes on ap.DocumentTypeID equals douType.ID
                                join curr in _context.Currency on ap.PurCurrencyID equals curr.ID
                                join scurr in _context.Currency on ap.SysCurrencyID equals scurr.ID
                                select new BPAging
                                {
                                    SeriesDID = ap.SeriesDetailID,
                                    PurchaseAPID = ap.ID,
                                    InvoiceNo = ap.InvoiceNo,
                                    DouType = douType.Code,
                                    PostingDate = ap.PostingDate.ToString("dd-MM-yyyy"),
                                    DueDate = ap.DueDate.ToString("dd-MM-yyyy"),
                                    RefNo = ap.ReffNo,
                                    VendorCode = $"{cus.Code} - {cus.Name}",
                                    VendorName = cus.Name,
                                    BalanceDue = curr.Description + "   " + string.Format("{0:#,0.000}", ap.BalanceDue * (-1)),
                                    SBalanceDue = (ap.BalanceDue * ap.PurRate) * (-1),
                                    Currencysys = scurr.Description,
                                }).ToList();
            }
            var outgoing = (from outs in outgoingPayments
                            join bs in _context.BusinessPartners on outs.VendorID equals bs.ID
                            join outd in _context.OutgoingPaymentDetails on outs.OutgoingPaymentID equals outd.OutgoingPaymentID
                            join doutype in _context.DocumentTypes on outs.DocumentID equals doutype.ID
                            join com in _context.Company on outs.CompanyID equals com.ID
                            join scurr in _context.Currency on com.SystemCurrencyID equals scurr.ID
                            select new BPAging
                            {
                                SeriesDID = outs.SeriesDetailID,
                                OutgoingPaymentID = outs.OutgoingPaymentID,
                                DouType = doutype.Code,
                                InvoiceNo = outs.NumberInvioce,
                                VendorCode = $"{bs.Code} - {bs.Name}",
                                VendorName = bs.Name,
                                BalanceDue = outd.CurrencyName + "   " + string.Format("{0:#,0.000}", outd.Totalpayment),
                                SBalanceDue = Convert.ToDouble(outd.Totalpayment) * outd.ExchangeRate,
                                PostingDate = outs.PostingDate.ToString("dd-MM-yyyy"),
                                DueDate = outs.DocumentDate.ToString("dd-MM-yyyy"),
                                Currencysys = scurr.Description
                            }).ToList();

            var allSummarySalePur = new List<BPAging>
            (purchaseAP.Count + purchaseMemo.Count + outgoing.Count + purchaseAPRS.Count);
            allSummarySalePur.AddRange(purchaseAP);
            allSummarySalePur.AddRange(purchaseMemo);
            allSummarySalePur.AddRange(outgoing);
            allSummarySalePur.AddRange(purchaseAPRS);
            var allsale = (from all in allSummarySalePur
                           select new
                           {
                               all.SeriesDID,
                               all.DouType,
                               all.VendorCode,
                               all.PostingDate,
                               all.DueDate,
                               all.InvoiceNo,
                               all.BalanceDue,
                               SBalanceDue = Convert.ToDouble(all.SBalanceDue),
                               TotalBalance = all.Currencysys + " " + allSummarySalePur.Where(i => i.VendorCode == all.VendorCode).Sum(i => i.SBalanceDue).ToString("0,0.000"),
                               GrandTotal = allSummarySalePur.Sum(s => s.SBalanceDue)
                           });
            return Ok(allsale.OrderBy(o => o.SeriesDID));
        }
    }
    public class SearchBPOppParams
    {
        public int BpID1 { get; set; }
        public int BpID2 { get; set; }
        public List<EmployeeParam> Employee { get; set; }

    }
    public class EmployeeParam
    {
        public int ID { get; set; }
    }
}
