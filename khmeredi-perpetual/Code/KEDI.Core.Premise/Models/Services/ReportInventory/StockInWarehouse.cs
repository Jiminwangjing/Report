using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.Services.ReportInventory
{
    [Table("rp_StokInWarehouse",Schema ="dbo")]
    public class StockInWarehouse
    {
        [Key]
        public int ID { get; set; }
        public string Code { get; set; }
        public string KhmerName { get; set; }
        public string EnglishName { get; set; }
        public double InStock { get; set; }
        public double Committed { get; set; }
        public double Ordered { get; set; }
        public string Uom { get; set; }
        public string Image { get; set; }
        public string Barcode { get; set; }
        public string ExpireDate { get; set; }
        public int ItemID { get; set; }
    }
}
