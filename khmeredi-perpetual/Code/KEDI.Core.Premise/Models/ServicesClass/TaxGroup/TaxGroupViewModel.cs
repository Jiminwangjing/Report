using KEDI.Core.Premise.Models.Services.Banking;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;

namespace KEDI.Core.Premise.Models.ServicesClass.TaxGroup
{
    public class TaxGroupViewModel
    {
        public string LineID { get; set; }
        public int ID { get; set; }
        public int GLID { get; set; }
        public int CompanyID { get; set; }
        public string Code { get; set; }
        public bool Active { get; set; }
        public string Name { get; set; }
        public List<SelectListItem> Types { get; set; }
        public int Type { get; set; }
        public DateTime Effectivefrom { get; set; }
        public decimal Rate { get; set; }
        public bool Delete { get; set; }
        public string GlAcc { get; set; }
        public List<TaxGroupDefinition> TaxGroupDefinitions { get; set; }

    }
}
