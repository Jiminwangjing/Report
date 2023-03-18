using CKBS.Models.Services.Administrator.Inventory;
using CKBS.Models.Services.Banking;
using CKBS.Models.Services.Inventory;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;


namespace CKBS.Models.Services.Promotions
{
    [Table("tbPointDetail",Schema ="dbo")]
    public class PointDetail
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public int PointID { get; set; }
        public int? ItemID { get; set; }
        public int Qty { get; set; }
        public double Price { get; set; }
        public int? CurrencyID { get; set; }
        public int? UomID { get; set; }
        public bool Delete { get; set; }

        [ForeignKey("PointID")]
        public Point Point { get; set; }
        [ForeignKey("ItemID")]
        public virtual ItemMasterData ItemMasterData { get; set; }
        [ForeignKey("CurrencyID")]
        public virtual Currency Currency { get; set; }
        [ForeignKey("UomID")]
        public virtual UnitofMeasure UnitofMeasure { get; set; }
    }
   
}
