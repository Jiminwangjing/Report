using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.Services.HumanResources
{
    [Table("tbAutoBrand", Schema = "dbo")]
    public class AutoBrand
    {
        [Key]
        public int BrandID { get; set; }
        public string BrandName { get; set; }
        public bool Active { get; set; }
    }
}
