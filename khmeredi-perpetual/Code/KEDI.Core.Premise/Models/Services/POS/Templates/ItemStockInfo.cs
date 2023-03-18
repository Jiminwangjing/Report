using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.Services.POS.Template
{
    public class ItemStockInfo
    {
        public int Line_ID { get; set; }
        public int ItemID { get; set; }
        public string Code { get; set; }
        public string KhmerName { get; set; }
        public string UomName { get; set; }
        public decimal InStock { get; set; }
        public decimal OrderQty { get; set; }
        public decimal Committed { get; set; }
    }
}
