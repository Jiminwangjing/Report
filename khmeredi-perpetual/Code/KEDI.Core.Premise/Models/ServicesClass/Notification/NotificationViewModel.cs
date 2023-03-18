using System.Collections.Generic;
using CKBS.Models.ServicesClass.AlertViewClass;
using KEDI.Core.Premise.Models.Services.AlertManagement;

namespace CKBS.Models.ServicesClass.Notification
{
    public class NotificationViewModel
    {
        public int CountStock { get; set; }
        public int CountDueDate { get; set; }
        public int CountCashOut{ get; set; }
        public int CountExpirationItem { get; set; }
        public int CountNoti { get; set; }
        public List<DueDateAlertViewModel> DueDate { get; set; }
        public List<StockAlertViewModel> Stock { get; set; }
        public List<CashOutAlertViewModel> CashOuts { get; set; }
        public List<ExpirationStockItem> ExpirationItems { get; set; } 
    }
}
