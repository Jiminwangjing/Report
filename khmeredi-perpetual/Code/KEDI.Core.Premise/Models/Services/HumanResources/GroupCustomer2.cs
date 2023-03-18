using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace KEDI.Core.Premise.Models.Services.HumanResources
{
    [Table("tbGroup2")]
    public class GroupCustomer2
    {
        [Key]
        public int ID { get; set; }
        public int Group1ID { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }

        public bool Delete { get; set; }
    }
}
