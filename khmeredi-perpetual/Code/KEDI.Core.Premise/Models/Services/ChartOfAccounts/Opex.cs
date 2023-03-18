using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace KEDI.Core.Premise.Models.Services.ChartOfAccounts
{
    [Table("tbOpexs")]
    public class Opex
    {
        [Key]
        public int ID { get; set; }
        public string Name { get; set; }
        [NotMapped]
        public decimal Amount { get; set; }
        [NotMapped]
        public string AmountDiplay { get; set; }
        public bool Delete { get; set; }
    }
    public class SubTypeAcount
    {
        [Key]
        public int ID { get; set; }
        [NotMapped]
        public string LineID { get; set; } = DateTime.Now.Ticks.ToString();
        public string Name { get; set; }
        public bool Default { get; set; }
        public bool Delete { get; set; }
    }
}
