using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.Services.Opportunity
{
    [Table("OpportunityMasterData")]

    public class OpportunityMasterData
    {
        public int ID { get; set; }
        //public string Employee { get; set; }
        public int BPID { get; set; }
        public int SaleEmpID { get; set; }
        //public string BPCode { get; set; }
        //public string BPName { get; set; }
        public string Owner { get; set; }
        public string Status { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime ClosingDate { get; set; }

        public float CloingPercentage { get; set; }

        public PotentialDetail PotentialDetails { get; set; }
        public List<StageDetail> StageDetail { get; set; }
        public List<PartnerDetail> PartnerDetail { get; set; }
        public List<CompetitorDetail> CompetitorDetail { get; set; }
        public SummaryDetail SummaryDetails { get; set; }
        [NotMapped]
        public int Day { get; set; }
        public int OpportunityNo { get; set; }
        public string OpportunityName { get; set; }

        [NotMapped]
        public decimal PotentailAmount { get; set; }
        [NotMapped]
        public string StartOfDay { get; set; }
        [NotMapped]
        public string CloseOfDay { get; set; }
        [NotMapped]
        public int NumofStartDate { get; set; }
        [NotMapped]
        public int NumofCloseDate { get; set; }
        [NotMapped]
        public string BPName { get; set; }
        [NotMapped]
        public string BPCode { get; set; }
        [NotMapped]
        public float CloseingPerecnt { get; set; }
        [NotMapped]
        public decimal PotentialAmount { get; set; }
        [NotMapped]
        public decimal WeightAmount { get; set; }

    }
}
