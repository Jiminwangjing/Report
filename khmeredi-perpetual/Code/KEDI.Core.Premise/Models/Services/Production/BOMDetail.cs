using CKBS.Models.Services.Administrator.Inventory;
using CKBS.Models.Services.Banking;
using CKBS.Models.Services.Inventory;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace CKBS.Models.Services.Production
{
    [Table("tbBOMDetail", Schema = "dbo")]
    public class BOMDetail
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int BDID { get; set; }
        public int BID { get; set; }
        [Required(ErrorMessage = "Please select Item !")]
        public int ItemID { get; set; }
        public double Qty { get; set; }
        [Required(ErrorMessage = "Please select Uom !")]
        public int UomID { get; set; }
        public int GUomID { get; set; }
        public double Cost { get; set; }
        public double Amount { get; set; }
        public bool Detele { get; set; }
        public bool NegativeStock { get; set; } 
        //ForeiegnKey
        [ForeignKey("ItemID")]
        public ItemMasterData ItemMasterData { get; set; }
        [ForeignKey("UomID")]
        public UnitofMeasure UnitofMeasure { get; set; }
        [ForeignKey("GUomID")]
        public GroupUOM GroupUOM { get; set; }
    }
}
