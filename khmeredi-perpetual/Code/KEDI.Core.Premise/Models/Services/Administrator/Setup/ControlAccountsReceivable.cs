using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KEDI.Core.Premise.Models.Services.Administrator.SetUp
{
    [Table("ControlAccountsReceivable")]
    public class ControlAccountsReceivable
    {
        [Key]
        public int ID { get; set; }
        public string TypeOfAccount { get; set; }
        public int CustID { get; set; }
        public int GLAID { get; set; }
    }
}
