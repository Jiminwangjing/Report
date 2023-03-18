using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KEDI.Core.Premise.Models.Services.Administrator.SetUp
{
    [Table("SaleGLAccountDetermination")]
    public class SaleGLAccountDetermination
    {
        [Key]
        public int ID { get; set; }
        public string TypeOfAccount { get; set; }
        public int CusID { get; set; }
        public int GLID { get; set; }
        public string Code { get; set; } // Take Every First letter of the world of name GLAccount
        public int SaleGLDeterminationMasterID { get; set; }
    }
}
