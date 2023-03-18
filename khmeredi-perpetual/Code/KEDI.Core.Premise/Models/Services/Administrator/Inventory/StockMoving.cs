using CKBS.Models.Services.Inventory;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.Services.Administrator.Inventory
{
    [Table("tbStockMoving", Schema = "dbo")]
    public class StockMoving
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public int WarehouseID { get; set; }
        public int UomID { get; set; }
        public int UserID { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString ="0:yyyy-MM-dd")]
        [Column(TypeName ="Date")]
        public DateTime SyetemDate { get; set; }
       
        public string TimeIn { get; set; }
        public double InStock { get; set; }
        public double Committed { get; set; }
        public double Ordered { get; set; }
        public double Available { get; set; }
        public int CurrencyID { get; set; }
        [DataType(DataType.Date)]
        [Column(TypeName = "Date")]
        [DisplayFormat(DataFormatString = "0:yyyy-MM-dd")]
        public DateTime ExpireDate { get; set; }
        public int ItemID { get; set; }
        public double Cost { get; set; }
        public int WarehoseDetailLineID { get; set; }
        //Foreign Key
        [ForeignKey("WarehouseID")]
        public Warehouse Warehouse { get; set; }

    }
}
