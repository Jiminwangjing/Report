using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.Services.Opportunity
{
    [Table("SummaryDetail")]
    public class SummaryDetail
    {
        public int ID { get; set; }
        public int OpportunityMasterDataID { get; set; }
        public int DocTypeID { get; set; }
        public int SeriesID { get; set; }
        public int SeriesDID { get; set; }
        public bool IsOpen { get; set; }
        public bool IsLost { get; set; }
        public bool IsWon { get; set; }
        public List<DescriptionSummaryDetail> DescriptionSummaryDetails { get; set; }
    }
}
