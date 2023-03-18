using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace KEDI.Core.Premise.Models.Services.ServiceContractTemplate
{
    [Table("AttachmentFileOfContractTemplate")]
    public class AttachmentFileOfContractTemplate
    {
        [Key]
        public int ID { get; set; }
        public int ContractTemplateID { get; set; }
        [NotMapped]
        public string LineID { get; set; } = DateTime.Now.Ticks.ToString();
        [NotMapped]
        public string LineMID { get; set; }
        public string TargetPath { get; set; }
        public string FileName { get; set; }
        public string AttachmentDate { get; set; }
        public bool Delete { get; set; }
    }
}
