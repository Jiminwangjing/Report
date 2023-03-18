using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KEDI.Core.Premise.Models.ServicesClass.TaxGroup
{
    public class TaxGroupDefinitionViewModel
    {
        public string LineID { get; set; }
        public int ID { get; set; }
        public int TaxGroupID { get; set; }
        public DateTime EffectiveFrom { get; set; }
        public decimal Rate { get; set; }
    }
}
