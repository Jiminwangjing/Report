using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.Services.Promotions
{
    [Table("tbPoint",Schema ="dbo")]
    public class Point
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public int Quantity { get; set; }
        public int Points { get; set; }
        public bool Delete { get; set; }
        public double Amount { get; set; }
    }
}
