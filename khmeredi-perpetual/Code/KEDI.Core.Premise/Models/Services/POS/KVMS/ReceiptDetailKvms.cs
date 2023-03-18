using CKBS.Models.Services.Administrator.Inventory;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.Services.POS.KVMS
{
    [Table("tbReceiptDetailKvms", Schema = "dbo")]
    public class ReceiptDetailKvms
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public int ReceiptKvmsID { get; set; }
        public int OrderDetailID { get; set; }// Price list detail identity
        public int? OrderID { get; set; }
        public int Line_ID { get; set; }
        public int ItemID { get; set; }
        public string Code { get; set; }
        public string KhmerName { get; set; }
        public string EnglishName { get; set; }
        public double Qty { get; set; }
        public double PrintQty { get; set; }
        public double OpenQty { get; set; }
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
        public string Description { get; set; }
        public string ParentLevel { get; set; }
        [ForeignKey("UomID")]
        public UnitofMeasure UnitofMeansure { get; set; }

        [ForeignKey("ReceiptKvmsID")]
        public ReceiptKvms ReceiptKvms { get; set; }
    }
}
