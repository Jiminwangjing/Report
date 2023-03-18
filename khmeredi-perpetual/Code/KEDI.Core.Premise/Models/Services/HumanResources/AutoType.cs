using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.Services.HumanResources
{
    [Table("tbAutoType", Schema = "dbo")]
    public class AutoType
    {
        [Key]
        public int TypeID { get; set; }
        public string TypeName { get; set; }
        public bool Active { get; set; }
    }
}
