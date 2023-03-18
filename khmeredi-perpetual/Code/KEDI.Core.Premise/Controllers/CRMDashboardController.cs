using CKBS.AppContext;
using CKBS.Models.Services.HumanResources;
using CKBS.Models.Services.Opportunity;
using CKBS.Models.Services.OpportunityReports;
using KEDI.Core.Premise.Models.GeneralSettingAdmin;
using KEDI.Core.Premise.Models.Services.Activity;
using KEDI.Core.Premise.Models.ServicesClass.ActivityViewModel;
using KEDI.Core.Premise.Models.ServicesClass.GeneralSettingAdminView;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace KEDI.Core.Premise.Controllers
{
    public class CRMDashboardController : Controller
    {
        private readonly DataContext _context;
        public CRMDashboardController(DataContext context)
        {
            _context = context;
        }
        public IActionResult CRMDashboard()
        {
            ViewBag.CRMDashboard = "highlight";
            var views = new StageView
            {
                GeneralSetting = GetGeneralSettingAdmin().Display,
            };
            return View(views);
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
        public IActionResult GetActivityByType()
        {
            var datatype = (from ac in _context.Activites.Where(x => (int)x.Activities == 1)
                            join g in _context.Generals on ac.ID equals g.ActivityID
                            let actype = _context.SetupTypes.FirstOrDefault(x => x.ID == ac.TypeID) ?? new SetupType()
                            let bp = _context.BusinessPartners.Where(x => x.ID == ac.BPID).FirstOrDefault() ?? new BusinessPartner()
                            join us in _context.UserAccounts on ac.UserID equals us.ID
                            let emp = _context.Employees.Where(x => x.ID == us.EmployeeID).FirstOrDefault() ?? new Employee()
                            select new ActivityView
                            {
                                ID = ac.ID,
                                GID = g.ID,
                                TypeID = ac.TypeID,
                                TypeName = actype.Name ?? ""

                            }).ToList();
            var result = (from list in datatype
                          group list by new { list.TypeID } into d
                          let datas = d.FirstOrDefault()
                          let count = d.Count()
                          select new ActivityView
                          {
                              ID = datas.ID,
                              GID = datas.ID,
                              TypeName = datas.TypeName,
                              CountData = count,
                              Counting = datatype.Count,
                              PercentTage = count / (float)datatype.Count * 100,

                          }).ToList();
            return Ok(result);
        }
        public IActionResult GetOpportunityTopFive()
        {
            var liststage = (from op in _context.OpportunityMasterDatas.Where(x => x.Status == "Open")
                             join po in _context.PotentialDetails on op.ID equals po.OpportunityMasterDataID
                             let sl = _context.Employees.Where(x => x.ID == op.SaleEmpID).FirstOrDefault() ?? new CKBS.Models.Services.HumanResources.Employee()

                             select new StageView
                             {
                                 ID = op.ID,
                                 SaleempID = op.SaleEmpID,
                                 BpID = op.BPID,
                                 SaleEmp = sl.Name,
                                 ExcetedAmount = po.PotentailAmount,
                             }).ToList();
            for (int i = 0; i < liststage.Count; i++)
            {
                for (int j = i + 1; j < liststage.Count; j++)
                {
                    if (liststage[i].SaleempID == liststage[j].SaleempID)
                    {
                        liststage[i].ExcetedAmount += liststage[j].ExcetedAmount;
                    }
                }
            }

            var result = (from list in liststage
                          group new { list } by new { list.SaleempID } into datas
                          let data = datas.FirstOrDefault()
                          select new StageView
                          {
                              ID = data.list.ID,
                              SaleempID = data.list.SaleempID,
                              BpID = data.list.BpID,
                              SaleEmp = data.list.SaleEmp,
                              ExcetedAmount = data.list.ExcetedAmount,
                          }).OrderByDescending(r => r.ExcetedAmount).Take(5).Reverse().ToList();
            return Ok(result);
        }


        public IActionResult GetOpportunityByStage()
        {
            var liststage = (from op in _context.OpportunityMasterDatas
                             join std in _context.StageDetails on op.ID equals std.OpportunityMasterDataID
                             join st in _context.SetUpStages on std.StagesID equals st.ID
                             join sum in _context.SummaryDetail.Where(x => x.IsOpen) on op.ID equals sum.OpportunityMasterDataID
                             select new StageView
                             {
                                 ID = std.ID,
                                 StageID = std.StagesID,
                                 Name = st.Name,
                                 StageNo = st.StageNo,
                                 ExcetedAmount = std.PotentailAmount,
                             }).ToList();
            for (int i = 0; i < liststage.Count; i++)
            {
                for (int j = i + 1; j < liststage.Count; j++)
                {
                    if (liststage[i].StageID == liststage[j].StageID)
                    {
                        liststage[i].ExcetedAmount += liststage[j].ExcetedAmount;
                    }
                }
            }

            var result = (from list in liststage
                          group new { list } by new { list.StageID } into datas
                          let data = datas.FirstOrDefault()
                          select new StageView
                          {
                              ID = data.list.ID,
                              StageID = data.list.StageID,
                              Name = data.list.Name,
                              StageNo = data.list.StageNo,
                              ExcetedAmount = data.list.ExcetedAmount,
                          }).OrderBy(r => r.StageNo).ToList();
            return Ok(result);
        }
        public IActionResult GetDataUser()
        {
            List<ActivityView> activityViews = new();
            var users = (from ac in _context.Activites
                         join us in _context.UserAccounts on ac.UserID equals us.ID
                         join emp in _context.Employees on us.EmployeeID equals emp.ID
                         group new { ac, us,emp } by new { ac.UserID } into datas
                         let data = datas.Count()
                         orderby data descending
                         select new ActivityView
                         {
                             ID = datas.First().ac.ID,
                             EmpName = datas.First().emp.Name,
                             data = data
                         }
                      ).OrderByDescending(r => r.data).Take(10).ToList();
            return Ok(users);

        }
        public IActionResult GetSyscurr()
        {
            var data = (from com in _context.Company
                        join pl in _context.PriceLists.Where(x => !x.Delete) on com.PriceListID equals pl.ID
                        join cur in _context.Currency on pl.CurrencyID equals cur.ID
                        select new
                        {
                            ID = com.ID,
                            Description = cur.Description,

                        }
                      ).FirstOrDefault();
            return Ok(data);
        }
        public IActionResult GetTopDeals()
        {
            var data = (from op in _context.OpportunityMasterDatas
                        join bp in _context.BusinessPartners on op.BPID equals bp.ID
                        join std in _context.StageDetails on op.ID equals std.OpportunityMasterDataID
                        join st in _context.SetUpStages on std.StagesID equals st.ID
                        join po in _context.PotentialDetails on op.ID equals po.OpportunityMasterDataID
                        let emp = _context.Employees.Where(x => x.ID == op.SaleEmpID).FirstOrDefault() ?? new Employee()
                        select new StageView
                        {
                            ID = op.ID,
                            BpID = op.BPID,
                            BPName = bp.Name,
                            SaleempID = op.SaleEmpID,
                            Employee = emp.Name,
                            StageName = st.Name,
                            ExcetedAmount = po.PotentailAmount,
                            StartDate = op.StartDate,
                            ClosingDate = op.ClosingDate,
                            PredictedClosingInNum = po.PredictedClosingInNum,
                            PredictedClosingInTime = po.PredictedClosingInTime,
                            //SDate=op.StartDate.ToString("dd"),
                            //CDate=op.ClosingDate.ToString("dd")
                            CYDYS = (op.ClosingDate - op.StartDate).Days
                        }
                      ).ToList();
            for (int i = 0; i < data.Count; i++)
            {
                for (int j = i + 1; j < data.Count; j++)
                {
                    if (data[i].BpID == data[j].BpID)
                    {
                        data[i].ExcetedAmount += data[j].ExcetedAmount;
                    }
                }
            }
            var result = (from list in data
                          group new { list } by new { list.BpID } into d
                          let datas = d.FirstOrDefault()
                          select new StageView
                          {
                              ID = datas.list.ID,
                              SaleempID = datas.list.SaleempID,
                              BpID = datas.list.BpID,
                              BPName = datas.list.BPName,
                              Employee = datas.list.Employee,
                              StageName = datas.list.StageName,
                              ExcetedAmount = datas.list.ExcetedAmount,
                              StartDate = datas.list.StartDate,
                              ClosingDate = datas.list.ClosingDate,
                              SDate = datas.list.SDate,
                              CDate = datas.list.CDate,
                              PredictedClosingInNum = datas.list.PredictedClosingInNum,
                              PredictedClosingInTime = datas.list.PredictedClosingInTime,
                              CYDYS = datas.list.CYDYS
                          }).OrderByDescending(r => r.ExcetedAmount).Take(5).ToList();
            return Ok(result);
        }


        public IActionResult GetdataDealByCustomerSource()
        {
            var liststage = (from opp in _context.OpportunityMasterDatas.Where(x => x.Status == "Open")
                             join bp in _context.BusinessPartners on opp.BPID equals bp.ID
                             join po in _context.PotentialDetails on opp.ID equals po.OpportunityMasterDataID
                             join cussur in _context.SetupCustomerSources on bp.CustomerSourceID equals cussur.ID
                             //let cussur = _context.SetupCustomerSources.FirstOrDefault(x=>x.ID==bp.CustomerSourceID) ?? new Models.Services.HumanResources.SetupCustomerSource()
                             select new StageView
                             {
                                 ID = opp.ID,
                                 BpID = bp.ID,
                                 CustomerSourceID = cussur.ID,
                                 CustomerSourceName = cussur.Name,
                                 ExcetedAmount = po.PotentailAmount,
                             }).ToList();
            for (int i = 0; i < liststage.Count; i++)
            {
                for (int j = i + 1; j < liststage.Count; j++)
                {
                    if (liststage[i].CustomerSourceID == liststage[j].CustomerSourceID)
                    {
                        liststage[i].ExcetedAmount += liststage[j].ExcetedAmount;
                    }
                }
            }

            var result = (from list in liststage
                          group new { list } by new { list.CustomerSourceID } into datas
                          let data = datas.FirstOrDefault()
                          select new StageView
                          {
                              ID = data.list.ID,
                              BpID = data.list.BpID,
                              CustomerSourceID = data.list.CustomerSourceID,
                              CustomerSourceName = data.list.CustomerSourceName,
                              ExcetedAmount = data.list.ExcetedAmount,

                          }).ToList();
            return Ok(result);
        }

        public IActionResult GetdataWinDealByStage()
        {
            var liststage = (from opp in _context.OpportunityMasterDatas.Where(x => x.Status == "Open")
                             join st in _context.StageDetails on opp.ID equals st.OpportunityMasterDataID
                             join po in _context.PotentialDetails on opp.ID equals po.OpportunityMasterDataID
                             join std in _context.SetUpStages on st.StagesID equals std.ID
                             join sum in _context.SummaryDetail.Where(x => x.IsWon == true) on opp.ID equals sum.OpportunityMasterDataID
                             //let sum=_context.SummaryDetail.FirstOrDefault(x=>x.OpportunityMasterDataID==opp.ID && x.IsWon==true) ?? new CKBS.Models.Services.Opportunity.SummaryDetail()
                             select new StageView
                             {
                                 ID = opp.ID,
                                 SummaryID = sum.ID,
                                 StageID = st.StagesID,
                                 StageName = std.Name,
                                 ExcetedAmount = po.PotentailAmount,
                             }).ToList();
            for (int i = 0; i < liststage.Count; i++)
            {
                for (int j = i + 1; j < liststage.Count; j++)
                {
                    if (liststage[i].StageID == liststage[j].StageID)
                    {
                        liststage[i].ExcetedAmount += liststage[j].ExcetedAmount;
                    }
                }
            }
            var result = (from list in liststage
                          group new { list } by new { list.StageID } into datas
                          let data = datas.FirstOrDefault()
                          select new StageView
                          {
                              ID = data.list.ID,
                              StageID = data.list.StageID,
                              StageName = data.list.StageName,
                              ExcetedAmount = data.list.ExcetedAmount,

                          }).ToList();
            return Ok(result);
        }
        public IActionResult GetWinOpportunityRatio()
        {
            List<OpportunityMasterData> opportunityMasterDatas = _context.OpportunityMasterDatas.ToList();
            var users = (from opp in opportunityMasterDatas
                         let sum = _context.SummaryDetail.Where(x => x.IsWon == true).ToList().Count
                         select new StageView
                         {
                             ID = opp.ID,
                             CountSummary = sum
                         }
                     ).ToList();
            return Ok(users);
        }
        public IActionResult GetTotalOpporturnity()
        {
            List<OpportunityMasterData> opportunityMasterDatas = _context.OpportunityMasterDatas.ToList();
            var data = (from opp in opportunityMasterDatas
                        let sum = _context.SummaryDetail.Where(x => x.IsWon == true).ToList().Count
                        select new StageView
                        {
                            ID = opp.ID,
                            CountSummary = sum
                        }).ToList();
            return Ok(data);
        }
    }
}
