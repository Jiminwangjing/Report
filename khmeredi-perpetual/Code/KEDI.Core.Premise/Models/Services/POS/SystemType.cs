using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.Services.POS
{
    [Table("tbSystemType",Schema ="dbo")]
    public class SystemType
    {
        [Key]
        public int ID { get; set; }
        public string Type { get; set; }
        public bool Status { get; set; }
    }
}
