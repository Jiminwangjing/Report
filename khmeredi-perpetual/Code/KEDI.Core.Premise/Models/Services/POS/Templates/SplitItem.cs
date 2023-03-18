using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.Services.POS.service
{
    public class SplitItem
    {
        public int OrderID { get; set; }
        public List<ItemSplit> ItemSplit { get; set; }
    }
}
