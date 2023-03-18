using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.ServicesClass
{
    public class ItemsReturnPC
    {
        public int Line_ID { get; set; }
        public int ItemID { get; set; }
        public string Code { get; set; }
        public string KhmerName { get; set; }
        public string InStock { get; set; }
        public string OrderQty { get; set; }
        public string Committed { get; set; }
    }
}
