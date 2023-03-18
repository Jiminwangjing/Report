using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.Services.Administrator.AlertManagement
{
    public enum TypeOfAlertUsed { Inactive, Active}
    [Table("tbTypeOfAlertM", Schema ="dbo")]
    public class TypeOfAlertM
    {
        [Key]
        public int ID { get; set; }
        public string Name { get; set; }
        public TypeOfAlertUsed TypeOfAlertUsed { get; set; }
    }
}
