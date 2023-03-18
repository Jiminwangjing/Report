using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.ReportClass
{
    [Table("ReportPurchaseOrder", Schema = "dbo")]
    public class ReportPurchaseOrder
    {
        [Key]
        public int ID { get; set; }
        public string InvoiceNo { get; set; }
        public string Prefix { get; set; }
        public string Number { get; set; } 
        public string BusinessName { get; set; }
        public string Warehouse { get; set; }
        public string UserName { get; set; }
        public string LocalCurrency { get; set; }
        public string SystemCurrency { get; set; }
        public double Balance_due { get; set; }
        public double Balance_due_sys { get; set; }
        public double ExchangeRate { get; set; }
        public string Status { get; set; }
        [NotMapped]
        public string Cencel { get; set; }
        public double Total { get; set; }
        [NotMapped]
        public List<SelectListItem> VatType { get; set; }
    }
}
