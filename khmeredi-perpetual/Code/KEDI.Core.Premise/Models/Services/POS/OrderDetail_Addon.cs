using CKBS.Models.Services.Administrator.Inventory;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.Services.POS
{
    [Table("tbOrderDetailAddon", Schema = "dbo")]
    public class OrderDetail_Addon
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AddOnID { get; set; }
        public int OrderDetailID { get; set; }// Price list detail identity
        public int? OrderID { get; set; }
        public string LineID { get; set; }
        public string Line_ID { get; set; }// order detail line id + auto count item in add on
        public int ItemID { get; set; }
        public string Code { get; set; }
        public string KhmerName { get; set; }
        public string EnglishName { get; set; }
        public double Qty { get; set; }
        public double PrintQty { get; set; }
        public double UnitPrice { get; set; }
        public double Cost { get; set; }
        public double DiscountRate { get; set; }
        public double DiscountValue { get; set; }
        public string TypeDis { get; set; }
        public double Total { get; set; }
        public double Total_Sys { get; set; }
        public int UomID { get; set; }
        public string ItemStatus { get; set; }//new,old
        public string ItemPrintTo { get; set; }
        public string Currency { get; set; }
        public string Comment { get; set; }
        public string ItemType { get; set; }
        public string ParentLineID { set; get; }
        public string Description { get; set; }
        [ForeignKey("UomID")]
        public UnitofMeasure UnitofMeansure { get; set; }
    }
}
