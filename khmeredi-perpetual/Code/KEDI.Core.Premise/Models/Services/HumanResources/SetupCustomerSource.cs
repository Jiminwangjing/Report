using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace KEDI.Core.Premise.Models.Services.HumanResources
{
    [Table("CustomerSource")]
    public class SetupCustomerSource
    {
        [NotMapped]
        public string LineID { get; set; } = DateTime.Now.Ticks.ToString();
        public int ID { get; set; }
        public string Name { get; set; }
    }
}
