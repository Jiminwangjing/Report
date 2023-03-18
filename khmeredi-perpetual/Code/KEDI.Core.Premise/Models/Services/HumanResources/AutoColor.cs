using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.Services.HumanResources
{
    [Table("tbAutoColor", Schema = "dbo")]
    public class AutoColor
    {
        [Key]
        public int ColorID { get; set; }
        public string ColorName { get; set; }
        public bool Active { get; set; }
    }
}
