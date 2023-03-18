using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KEDI.Core.Premise.Models.Services.POS.LoyaltyProgram
{
    public class MemberCardModel
    {
        public int ID { set; get; }
        public string RefNo { set; get; }
        public string Name { set; get; }
        public string CardType { set; get; }
        public string Description { set; get; }
        public string Discount { set; get; }
        public string ExpireDate { set; get; }
        public string DiscountType { set; get; }
    }
}
