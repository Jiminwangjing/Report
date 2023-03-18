using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KEDI.Core.Premise.Models.Services.LoyaltyProgram.MemberPoints
{
    public class PointSetup
    {
        public List<PointCard> PointCards { set; get; }
        public PointRedemption Redemption { set; get; }
    }
}
