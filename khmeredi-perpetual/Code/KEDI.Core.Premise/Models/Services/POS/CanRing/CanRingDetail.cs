using System.ComponentModel.DataAnnotations.Schema;
using MainCanRing = KEDI.Core.Premise.Models.Services.Administrator.CanRingSetup.MainCanRing;

namespace KEDI.Core.Premise.Models.Services.POS.CanRing
{
    [Table("CanRingDetail")]
    public class CanRingDetail : MainCanRing
    {
        public int ID { get; set; }
        public int CanRingID { get; set; }
        public int CanRingMasterID { get; set; }
    }
}
