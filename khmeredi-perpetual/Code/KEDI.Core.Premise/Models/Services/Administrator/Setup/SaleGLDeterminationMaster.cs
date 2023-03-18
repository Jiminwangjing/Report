using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KEDI.Core.Premise.Models.Services.Administrator.SetUp
{
    [Table("SaleGLADeter")]
    public class SaleGLDeterminationMaster
    {
        [Key]
        public int ID { get; set; }
        public int CusID { get; set; }
        public List<SaleGLAccountDetermination> SaleGLAccountDeterminations { get; set; }
        public List<SaleGLAccountDeterminationResources> SaleGLAccountDeterminationResources { get; set; }
        public AccountMemberCard AccountMemberCard { get; set; }
    }
    public class AccountMemberCard
    {
        public int ID { get; set; }
        public int CashAccID { get; set; }
        public int UnearnedRevenueID { get; set; }
        [NotMapped]
        public string CashAccCode { get; set; }
        [NotMapped]
        public string UnearnedRevenueCode { get; set; }
        [NotMapped]
        public string CashAccName { get; set; }
        [NotMapped]
        public string UnearnedRevenueName { get; set; }
    }
}
