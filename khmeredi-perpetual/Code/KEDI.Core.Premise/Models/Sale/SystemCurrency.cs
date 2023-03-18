using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KEDI.Core.Premise.Models.Sale
{
    public class SystemCurrency
    {
        public int ID { get; set; }
        public string Description { get; set; }
        public int DecimalPlaces { get; set; }
    }
}
