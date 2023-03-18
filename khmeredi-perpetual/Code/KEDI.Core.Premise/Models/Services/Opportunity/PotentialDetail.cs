using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.Services.Opportunity
{
    [Table("PotentialDetail")]

    public class PotentialDetail
    {
        public int ID { get; set; }
        public int OpportunityMasterDataID { get; set; }
        public string PredictedClosingInTime { get; set; }
        public int PredictedClosingInNum { get; set; }
        public DateTime PredictedClosingDate { get; set; }
        public decimal PotentailAmount { get; set; }
        public decimal WeightAmount { get; set; }
        public decimal GrossProfit { get; set; }
        public decimal GrossProfitTotal { get; set; }
        public string Level { get; set; }
        public List<DescriptionPotentialDetail> DescriptionPotentialDetail { get; set; }
    }
    public enum ClosingDate
    {
        Days = 1,
        Months = 2,
        Years = 3,

    }
}
