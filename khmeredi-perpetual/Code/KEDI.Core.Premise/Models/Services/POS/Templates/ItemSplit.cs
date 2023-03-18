using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.Services.POS.service
{
    public class ItemSplit
    {
        public int Line_ID { get; set; }
        public double Qty { get; set; }
        public double SplitQty { get; set; }
    }
}
