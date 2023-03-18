using KEDI.Core.Premise.Models.ServicesClass.TaxGroup;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace KEDI.Core.Premise.Models.ProjectCostAnalysis
{
    [Table("tbFreightProjCostDetail")]
    public class FreightProjCostDetail
    {

            [Key]
            public int ID { get; set; }
            public int TaxGroupID { get; set; }
            public int FreightID { get; set; }
            [NotMapped]
            public string LineID { get; set; }
      
             public int FreightProjectCostID { get; set; }
            public string Name { get; set; }
            public string TaxGroup { get; set; }
            [NotMapped]
            public List<SelectListItem> TaxGroupSelect { get; set; }
            [NotMapped]
            public List<TaxGroupViewModel> TaxGroups { get; set; }
            public decimal TaxRate { get; set; }
            public decimal TotalTaxAmount { get; set; }
            public decimal Amount { get; set; }
            public decimal AmountWithTax { get; set; }



    }
}
