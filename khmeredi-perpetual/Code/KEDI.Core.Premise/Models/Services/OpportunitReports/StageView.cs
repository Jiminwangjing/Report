using KEDI.Core.Premise.Models.GeneralSettingAdmin;
using System;
using System.Collections.Generic;
namespace CKBS.Models.Services.OpportunityReports
{
    public class StageView
    {
        public int ID { get; set; }
        public int BpID { get; set; }
        public int StageID { get; set; }
        public int CountSummary { get; set; }
        public bool IsWon { get; set; }
        public int CountOpportunity { get; set; }
        public int SummaryID { get; set; }
        public int PredictedClosingInNum { get; set; }
        public string PredictedClosingInTime { get; set; }
        public string CustomerSourceName { get; set; }
        public int SaleempID { get; set; }
        public int CustomerSourceID { get; set; }
        public string Name { get; set; }
        public int StageNo { get; set; }
        public decimal ExcetedAmount { get; set; }
        public int CYDYS { get; set; }
        public decimal WeightAmount { get; set; }
        public float Percent { get; set; }
        public string SaleEmp { get; set; }
        public string BPName { get; set; }
        public string Employee { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime ClosingDate { get; set; }
        public string StageName { get; set; }
        public string SDate { get; set; }
        public string CDate { get; set; }
        public Display GeneralSetting { get; set; }
    }
}
