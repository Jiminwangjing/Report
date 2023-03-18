using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.Services.Opportunity
{
    [Table("DescriptionPotentialDetail")]
    public class DescriptionPotentialDetail
    {
        public int ID { get; set; }
        public int interestID { get; set; }
        public int PotentialDetailID { get; set; }
        
    }
}
