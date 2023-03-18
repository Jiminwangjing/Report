using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace KEDI.Core.Premise.Models.Services.LoyaltyProgram.PromoCode
{
    public class PromoSetup
    {
        public PromoCodeDetail PromoCodeDetail { set; get; } = new();
        public PromoCodeDiscount PromoCodeDiscount { set; get; }
        public List<SelectListItem> SelectPriceList { get; set; }
    }
}
