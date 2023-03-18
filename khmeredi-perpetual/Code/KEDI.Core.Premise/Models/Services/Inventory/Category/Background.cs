using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.Services.Inventory.Category
{
    [Table("Background",Schema ="dbo")]
    public class Background
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int BackID { get; set; }
        public string Name { get; set; }
        public bool Delete { get; set; }

    }
}
