using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.Services.AlertManagement
{
    public class CashOutAlert
    {
        public int ID { get; set; }
        public int BrandID { get; set; }
        public int UserID { get; set; }
        public int EmpID { get; set; }
        public int CurrencyID { get; set; }
        public DateTime CreatedAt { get; set; }
        public string TimeIn { get; set; }
        public string TimeOut { get; set; }
        public DateTime DateIn { get; set; }
        public DateTime DateOut { get; set; }
        public decimal CashInAmountSys { get; set; }
        public decimal SaleAmountSys { get; set; }
        public decimal GrandTotal { get; set; }
        public bool IsRead { get; set; }
    }
}
