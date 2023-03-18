using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.Services.CostOfAccounting
{
    public class Dimension
    {
        public int ID { get; set; }
        public string DimensionName { get; set; }
        public string Description { get; set; }
        public int Level { get; set; }
    }
}
