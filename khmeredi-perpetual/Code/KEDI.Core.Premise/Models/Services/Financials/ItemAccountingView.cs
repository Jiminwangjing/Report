using CKBS.Models.Services.Inventory;
using CKBS.Models.Services.Inventory.Category;
using CKBS.Models.Services.Inventory.Property;
using CKBS.Models.ServicesClass.Property;
using KEDI.Core.Premise.Models.Services.Inventory;
using KEDI.Core.Premise.Models.Services.ServiceContractTemplate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.Services.Financials
{
    public class ItemAccountingView
    {
        public List<ItemAccounting> ItemAccountings { get; set; }
        public ItemMasterData ItemMasterData { get; set; }
        public ItemAccounting ItemAccounting { get; set; }
        public ItemGroup1 ItemGroup1 { get; set; }
        public List<PropertyViewModel> PropertyDetails { get; set; }
        public ContractTemplate Contract { get; set; }
    }
}
