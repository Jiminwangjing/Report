using CKBS.Models.Services.Administrator.Inventory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KEDI.Core.Premise.Models.Sale
{
    public class SaleOrderViewModel
    {
        public SaleOrder SaleOrder { get; set; }
        public Warehouse Warehouse { get; set; }
    }
}
