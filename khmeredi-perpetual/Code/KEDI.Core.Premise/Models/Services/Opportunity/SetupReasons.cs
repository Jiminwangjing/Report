using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.Services.Opportunity
{
    [Table("SetupReasons")]

    public class SetupReasons
    {
        public int ID { get; set; }
        public string Description { get; set; }
        [NotMapped]
        public string LineID { get; set; } = DateTime.Now.Ticks.ToString();
    }
}
