using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.Services.POS.service
{
    public class CombineOrder
    {
        public int TableID { get; set; }
        public int OrderID { get; set; }
        public List<Order> Orders { get; set; }
    }
}
