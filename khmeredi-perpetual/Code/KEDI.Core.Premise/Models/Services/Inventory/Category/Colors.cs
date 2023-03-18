using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.Services.Inventory.Category
{
    [Table("Colors",Schema ="dbo")]
    public class Colors
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ColorID { get; set; }
        public string Name { get; set; }
        public bool Delete { get; set; }
   
    }
}
