using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace KEDI.Core.Premise.Models.Services.ServiceContractTemplate
{
    [Table("Remark")]
    public class Remark
    {
        [Key]
        public int ID { get; set; }
        //public int ContractTemplateID { get; set; }
        public string Remarks { get; set; }

    }
}
