using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CKBS.Models.Services.POS.service;
using KEDI.Core.Premise.Models.Services.POS.LoyaltyProgram;

namespace KEDI.Core.Premise.Models.Services.POS.Templates
{
    public class PromotionInfo
    {
        public LoyaltyProgModel LoyaltyProgram {set; get;}
        public IEnumerable<ServiceItemSales> SaleItems { set; get; }
    }
}