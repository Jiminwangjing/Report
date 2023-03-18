using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.Services.ReportInventory
{
    [Table("rp_StockInWarehouse_Detail",Schema ="dbo")]
    public class StockInWarehouse_Detail
    {
        [Key]
        public int ItemID { get; set; }
        public string Cost { get; set; }
        public double InStock { get; set; }
        public double Committed { get; set; }
        public double Ordered { get; set; }
        public string Uom { get; set; }
        public int UomID { get; set; }
        public string ExpireDate { get; set; }
        public string Warehouse { get; set; }
        public int ID { get; set; }
    }
}
