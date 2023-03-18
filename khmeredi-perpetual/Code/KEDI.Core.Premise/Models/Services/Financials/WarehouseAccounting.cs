using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.Services.Financials
{
    public class WarehouseAccounting : ItemAccounting
    {
        public int LineID { get; set; }
        public string CodeWarehouse { get; set; }
        public string Name { get; set; }
    }
}
