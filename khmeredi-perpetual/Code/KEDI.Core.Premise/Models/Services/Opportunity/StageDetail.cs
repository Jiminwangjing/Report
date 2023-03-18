using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.Services.Opportunity
{

    [Table("StageDetail")]
    public class StageDetail
    {
        public int ID { get; set; }
        public DateTime StartDate {get;set;}
        public DateTime CloseDate { get; set; }
        public int OpportunityMasterDataID { get; set; }
        public int SaleEmpselectID { get; set; }
        public int StagesID { get; set; }
        public float Percent { get; set; }
        public decimal PotentailAmount { get; set; }
        public decimal WeightAmount { get; set; }
        public bool ShowBpsDoc { get; set; }
        public int DocTypeID { get; set; }
        public int DocNo { get; set; }
      public int ActivityID { get; set; }
        public int OwnerID { get; set; }
        [NotMapped]
        public string Name { get; set; }
        [NotMapped]
        public string Action { get; set; }
       
    }
}
