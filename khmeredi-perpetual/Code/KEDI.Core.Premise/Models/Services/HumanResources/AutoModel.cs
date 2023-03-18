using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.Services.HumanResources
{
    [Table("tbAutoModel", Schema = "dbo")]
    public class AutoModel
    {
        [Key]
        public int ModelID { get; set; }
        public string ModelName { get; set; }
        public bool Active { get; set; }
    }
}
