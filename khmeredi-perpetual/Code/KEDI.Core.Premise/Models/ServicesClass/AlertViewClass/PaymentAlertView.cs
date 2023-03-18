using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KEDI.Core.Premise.Models.ServicesClass.AlertViewClass
{
    public class PaymentAlertView
    {
        public int ID { get; set; }
        public int BrandID { get; set; }
        public int UserID { get; set; }
        public int EmpID { get; set; }
        public int CurrencyID { get; set; }
        public DateTime CreatedAt { get; set; }
        public string BrandName { get; set; }
        public string EmpName { get; set; }
        public string TimeIn { get; set; }
        public string TimeOut { get; set; }
        public DateTime DateIn { get; set; }
        public DateTime DateOut { get; set; }
        public string Currency { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Discount { get; set; }
        public string No { get; set; }
        public decimal GrandTotal { get; set; }
        public decimal Received { get; set; }
        public decimal Change { get; set; }
        public decimal Vatable { get; set; }
        public bool IsRead { get; set; }

    }
}
