using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.Services.HumanResources
{
    [Table("tbInstaillments", Schema = "dbo")]

    public class Instaillment
    {
        [Key]
        public int ID { get; set; }
        public int NoOfInstaillment { get; set; }
        public bool ApplyTax { get; set; }
        public bool UpdateTax { get; set; }
        public List<InstaillmentDetail> InstaillmentDetails { get; set; }
        public CreditMethod? CreditMethod { get; set; }
    }
    public enum CreditMethod
    {
        FirstInstaillment = 1,
        LastInstaillment = 2,
        Equally = 3,
    }
}
