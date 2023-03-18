
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.Services.POS.tmptable
{
    [Table("tmpOrderDetail", Schema = "dbo")]
    public class TmpOrderDetail
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public int ItemID { get; set; }
        public double Qty { get; set; }
        public int UomID { get; set; }
        public double Cost { get; set; }
        public double UnitPrice { get; set; }
        public int CurrencyID { get; set; }
        public string Process { get; set; }
        public int WarehouseID { get; set; }
        public int BranchID { get; set; }
        public int UserID { get; set; }
        public string InvoiceNo { get; set; }
        public double ExchageRate { get; set; }
        [Column(TypeName = "Date")]
        public DateTime ExpireDate { get; set; }


    }
}
