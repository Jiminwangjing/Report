using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.ServicesClass
{
    public class DiscountItem
    {
        public int PromotionID { get; set; }
        public List<DiscountItemDetail> DiscountItemDetails { get; set; }
    }
}
