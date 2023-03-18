using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.Services.Banking
{
    [Table("tbTax",Schema ="dbo")]
    public class Tax
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [Required(ErrorMessage = "Please input name !")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Please input rate !")]
        public double Rate { get; set; } = 0;
        [Required]
        public string Type { get; set; }// Input,Output
        [DataType(DataType.Date, ErrorMessage = "Please input the effective date!")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Effective { get; set; }
        public bool Delete { get; set; }
    }
}
