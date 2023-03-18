using CKBS.Models.Services.POS.service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.Services.POS.Template
{
    public class ReceiptSummary
    {
        public int ReceiptID { set; get; }
        public string ReceiptNo { set; get; }
        public string Cashier { get; set; }
        public string CusName { get; set; }
        public string Phone { get; set; }
        public string DateOut { set; get; }
        public string PostingDate { get; set; }
        public string TimeOut { set; get; }
        public string TableName { set; get; }
        public string GrandTotal { set; get; }
        public string Currency { set; get; }

        public List<ReturnItem> ReturnItems { set; get; }
    }
}
