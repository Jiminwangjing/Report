using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace KEDI.Core.Premise.Models.Services.HumanResources
{
    [Table("SetupContractName")]
    public class SetupContractName
    {
        [Key]
        public int ID { get; set; }
        [NotMapped]
        public string LineID { get; set; } = DateTime.Now.Ticks.ToString();
        public string ContractName { get; set; }
        [NotMapped]
        public List<SelectListItem> SelecContracttName { get; set; }
    }
}
