using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.InventoryAuditReport
{
    [Table("ServiceInventoryAudit",Schema ="dbo")]
    public class ServiceInventoryAudit
    {
        [Key]
        public int ID { get; set; }
        public int UomID { get; set; }
        public string Tarns_Type { get; set; }
        public string Process { get; set; }
        [DataType(DataType.Date)]
        [Column(TypeName = "Date")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yy}", ApplyFormatInEditMode = true)]
        public DateTime SystemDate { get; set; }
        public string TimeIn { get; set; }
        public double Qty { get; set; }
        public double CumulativeQty  { get; set; }
        public double CumulativeValue { get; set; } 
        public double Trans_value { get; set; }
        public double Cost { get; set; }
        public int ItemID { get; set; }
        public string EnglistName { get; set; }
        public string KhmerName { get; set; }
        public string Code { get; set; }
        public string Warehouse { get; set; }
        public string Branch { get; set; }
        public string Employee { get; set; }
        public string Currency { get; set; }
        public string Uom { get; set; }
        public int WarehouseID { get; set; }
        [DataType(DataType.Date)]
        [Column(TypeName = "Date")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yy}", ApplyFormatInEditMode = true)]
        public DateTime ExpireDate { get; set; } 

    }
}
