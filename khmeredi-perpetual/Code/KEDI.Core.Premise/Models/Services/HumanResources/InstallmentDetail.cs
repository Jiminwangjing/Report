using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.Services.HumanResources
{
    [Table("tbInstallmentDetail", Schema = "dbo")]

    public class InstaillmentDetail
    {
        [Key]
        public int ID { get; set; }
        public int InstaillmentID { get; set; }
        public int Months { get; set; }
        public int Day { get; set; }
        public decimal Percent { get; set; }

    }
}
