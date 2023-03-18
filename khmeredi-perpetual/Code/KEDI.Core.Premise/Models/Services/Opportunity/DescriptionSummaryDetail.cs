using System.ComponentModel.DataAnnotations.Schema;

namespace CKBS.Models.Services.Opportunity
{
    [Table("DescriptionSummaryDetail")]
    public class DescriptionSummaryDetail
    {
        public int ID { get; set; }
        public int ReasonsID { get; set; }
        public int SummaryDetailID { get; set; }
    }
}
