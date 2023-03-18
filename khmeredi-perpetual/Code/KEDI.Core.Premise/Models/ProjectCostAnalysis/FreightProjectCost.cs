using KEDI.Core.Premise.Models.Sale;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace KEDI.Core.Premise.Models.ProjectCostAnalysis
{
    [Table("tbFreightProjectCost")]
    public class FreightProjectCost
    {

            [Key]
            public int ID { set; get; }
            public int ProjCAID { set; get; }
            public SaleCopyType SaleType { get; set; }
            public decimal AmountReven { set; get; }
            public decimal OpenAmountReven { set; get; }
            public decimal TaxSumValue { set; get; }
            public IEnumerable<FreightProjCostDetail> FreightProjCostDetails { get; set; }
      
    }
}
