using CKBS.Models.Services.Inventory.Category;
using CKBS.Models.Services.POS.service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.Services.POS.Template
{
    public class SaleItemsByGroup
    {
        public ItemGroup1 Group1 { get; set; }
        public ItemGroup2 Group2 { get; set; }
        public ItemGroup3 Group3 { get; set; }
        public ServiceItemSales Item { get; set; }
        public IEnumerable<ServiceItemSales> ItemsInGroup { get; set; }
    }
}
