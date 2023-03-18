using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.Services.ReportPurchase
{
    [Table("rp_SummaryPurchaseAP",Schema ="dbo")]
    public class SummaryPurchaseAP
    {
        [Key]
        public int PurchaseID { get; set; }
        public string Invoice { get; set; }
        public string PostingDate { get; set; }
        public string DocumentDate { get; set; }
        public string DueDate { get; set; }
        public double Sub_Total { get; set; }
        public double SubTotal_Sys { get; set; }
        public double DiscountRate { get; set; }
        public double TaxRate { get; set; }
        public double Balance_Due { get; set; }
        public double Balance_Deu_Sys { get; set; }
        public double Applied_Amount { get; set; }
        public string Warehouse { get; set; }
        public string Branch { get; set; }
        public string VendorName { get; set; }
        public string User { get; set; }
        public string LocalCurrency { get; set; }
        public string SysCurrency { get; set; }

    }
}
