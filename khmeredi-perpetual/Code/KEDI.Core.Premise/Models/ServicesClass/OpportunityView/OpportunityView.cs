using CKBS.Models.Services.HumanResources;
using CKBS.Models.Services.Opportunity;
using CKBS.Models.ServicesClass.InterestRange;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.ServicesClass.OpportunityView
{
    public class OpportunityView
    {
        public int ID { get; set; }
        public int DoctypeID { get; set; }
        public int PotentialDetailsID { get; set; }
        public int SummaryDetailsID { get; set; }
        public int BPID { get; set; }
        public int ActID { get; set; }
        public string Territery { get; set; }
        public string CusSources { get; set; }


        public string ContactPerson { get; set; }
        public int Tel { get; set; }
        public string SaleEmpName { get; set; }
        public int LevelID { get; set; }
        public string OwnerName { get; set; }
        public List<SelectListItem> selectsaleemp { get; set; }
        public List<SelectListItem> Descriptionselect { get; set; }

        public int EmpID { get; set; }
        public int OwnerID { get; set; }
        public string OpportunityName { get; set; }
        public int OpportunityNo { get; set; }
        public string BPCode { get; set; }
        public string BPName { get; set; }
        public string Status { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime ClosingDate { get; set; }
        public bool OppenActivity { get; set; }
        public float CloingPercentage { get; set; }
        public bool OpportunityTpe { get; set; }
        //public PotentialDetail PotentialDetails { get; set; }
        public List<StageViewModel> StageDetail { get; set; }
        public List<PartnerViewModel> PartnerDetail { get; set; }
        public List<CompetitorViewModel> CompetitorDetail { get; set; }
        //public SummaryDetail SummaryDetails { get; set; }
        public List<DescriptionSummaryViewModel> Descriptionsummary { get; set; }
        public List<DescriptionPotentailViewModel> Descriptionpotential { get; set; }

        public int POID { get; set; }
        public string PredictedClosingInTime { get; set; }
        public int PredictedClosingInNum { get; set; }
        public DateTime PredictedClosingDate { get; set; }
        public decimal PotentailAmount { get; set; }
        public decimal WeightAmount { get; set; }
        public decimal GrossProfit { get; set; }
        public decimal GrossProfitTotal { get; set; }
        public string Level { get; set; }
        public List<DescriptionPotentialDetail> DescriptionPotentialDetail { get; set; }

        public string DocType { get; set; }
        public int SeriesID { get; set; }
        public int SeriesDID { get; set; }
        public bool IsOpen { get; set; }
        public bool IsLost { get; set; }
        public bool IsWon { get; set; }
        public List<DescriptionSummaryDetail> DescriptionSummaryDetails { get; set; }
        public List<ContactPersonViewModel> Contact { get; set; }

    }
}
