using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace KEDI.Core.Premise.Models.Services.POS
{
    [Table("SettingPayment")]
    public class SettingPayment
    {
        [Key]
        public int ID { get; set; }
        public int PaymentMeansID { get; set; }
        public int SettingID { get; set; }
        public bool Payment { get; set; }

    }
}
