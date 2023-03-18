using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.ServicesClass
{
    public class PricelistSetUpdatePrice
    {
        public List<PricelistSetPrice> PricelistSetPrice { get; set; }
    }
}
