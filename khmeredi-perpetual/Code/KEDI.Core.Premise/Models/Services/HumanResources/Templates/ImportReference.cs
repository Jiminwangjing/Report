using CKBS.Models.Services.ChartOfAccounts;
using CKBS.Models.Services.Inventory.PriceList;
using System.Collections.Generic;

namespace KEDI.Core.Premise.Models.Services.HumanResources.Templates
{
    public class ImportReference
    {
        public List<GLAccount> GLAccounts { get; set; }
        public List<PriceLists> PriceLists { set; get; }
    }
}
