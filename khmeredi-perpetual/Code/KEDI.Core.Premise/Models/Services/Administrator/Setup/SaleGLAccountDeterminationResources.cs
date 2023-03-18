using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KEDI.Core.Premise.Models.Services.Administrator.SetUp
{
    [Table("SaleGLADeterRes")]
    public class SaleGLAccountDeterminationResources
    {
        [Key]
        public int ID { get; set; }
        public string TypeOfAccount { get; set; }
        public int GLAID { get; set; }
        public int SaleGLDeterminationMasterID { get; set; }

    }
}
