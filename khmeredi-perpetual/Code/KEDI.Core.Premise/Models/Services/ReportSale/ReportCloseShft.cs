using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.Services.ReportSale
{
    [Table("rp_CloseShift",Schema ="dbo")]
    public class ReportCloseShft
    {
        [Key]
        public int ID { get; set; }
        public string Branch { get; set; }
        public string User { get; set; }
        public string DateOut { get; set; }
        public string DateIn { get; set; }
        public string TimeIn { get; set; }
        public string TimeOut { get; set; }
        public double CashInAmount { get; set; }
        public double CashOutAmount { get; set; }
        public double SaleAmount_Local { get; set; }
        public double SaleAmount_Sys { get; set; }
        public string SysCurrency { get; set; }
        public string LocalCurrency { get; set; }
        public int UserID { get; set; }
        public double Tran_From { get; set; }
        public double Tran_To { get; set; }

    }
}
