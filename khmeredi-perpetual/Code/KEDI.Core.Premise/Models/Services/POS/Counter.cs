using CKBS.Models.Services.Administrator.General;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.Services.POS
{
    [Table("tbCounter", Schema = "dbo")]
    public class Counter
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [Required]
        public string Name { get; set; }
        public bool Delete { get; set; }
        [Required]
        public int PrinterID { get; set; }
        [ForeignKey("PrinterID")]
        public PrinterName PrinterName { get; set; }
    }
}
