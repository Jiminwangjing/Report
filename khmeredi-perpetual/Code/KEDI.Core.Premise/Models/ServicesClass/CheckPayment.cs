using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.ServicesClass
{
    [Table("ServiceCheckPayment",Schema ="dbo")]
    public class CheckPayment
    {
        [Key]
        public int ID { get; set; }
        public string Invoice { get; set; }
        public string Check { get; set; }
    }
}
