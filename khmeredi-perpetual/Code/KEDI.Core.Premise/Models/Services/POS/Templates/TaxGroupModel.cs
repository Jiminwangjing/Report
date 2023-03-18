using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KEDI.Core.Premise.Models.Services.POS.Templates
{
    public class TaxGroupModel
    {
        public int ID { set; get; }
        public string Name { set; get; }
        public decimal Rate { set; get; }
    }
}
