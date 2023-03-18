using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.Services.POS.service
{
    public class ReturnItem
    {
        public int ID { get; set; }
        public int ItemID { get; set; }
        public int ReceiptID { get; set; }
        public string Code { get; set; }
        public string KhName { get; set; }
        public string UoM { get; set; }
        public int UomID { set; get; }
        public double OpenQty { get; set; }
        public double ReturnQty { get; set; }
        public int UserID { get; set; }
        [NotMapped]
        public double Price { get; set; }
        [NotMapped]
        public double Amount { get; set; }
        [NotMapped]
        public double GrandAmount { get; set; }
        [NotMapped]
        public bool Status { get; set; }
    }
}
