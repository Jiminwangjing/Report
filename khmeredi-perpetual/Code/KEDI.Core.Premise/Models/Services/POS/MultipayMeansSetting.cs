using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace KEDI.Core.Premise.Models.Services.POS
{
    [Table("MultipayMeansSetting")]
    public class MultipayMeansSetting
    {
        [Key]
        public int ID { get; set; }
        public int SettingID { get; set; }
        public int PaymentID { get; set; }
        public bool Check { get; set; }
        public bool Changed { get; set; }
        public int AltCurrencyID {get;set;}
        
    }
}
