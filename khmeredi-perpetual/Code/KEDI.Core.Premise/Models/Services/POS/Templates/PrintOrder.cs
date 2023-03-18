using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.Services.POS.service
{
    public class PrintOrder
    {
        public string LineID { get; set; }
        public string Table { get; set; }
        public string OrderNo { get; set; }
        public string Cashier { get; set; }
        public string Item { get; set; }
        public string ItemEng { get; set; }
        public string PrintQty { get; set; }
        public string Uom { get; set; }
        public string ItemPrintTo { get; set; }
        public string Comment { get; set; }
        public string ParentLineID { get; set; }
        public string ItemType { get; set; }
        public string Price { get; set; }
        public string LinePosition { get; set; }
        public string Qty { get; set; }
    }
}
