using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.Services.Opportunity
{
    [Table("SaleEmployee")]
    public class SaleEmployee
    {
        public int ID { get; set;}
        public string Name { get; set; }
        public bool Action { get; set; }
        [NotMapped]
        public string LineID { get; set; } = DateTime.Now.Ticks.ToString();
    }
}
