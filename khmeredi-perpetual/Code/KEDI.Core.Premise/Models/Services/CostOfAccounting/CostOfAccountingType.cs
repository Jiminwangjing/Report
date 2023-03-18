using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.Services.CostOfAccounting
{
    public class CostOfAccountingType
    {
        [Key]
        public int ID { get; set; }
        public string CACodeType { get; set; }
        public string CACodeName { get; set; }
    }
}
