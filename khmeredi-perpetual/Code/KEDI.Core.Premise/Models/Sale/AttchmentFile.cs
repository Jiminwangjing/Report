using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace KEDI.Core.Premise.Models.Sale
{
    [Table("AttchmentFile")]
    public class AttchmentFile
    {
        [Key]
        public int ID { get; set; }
        public int ServiceContractID { get; set; }
        [NotMapped]
        public string LineID { get; set; } = DateTime.Now.Ticks.ToString();
        public string TargetPath { get; set; }
        public string FileName { get; set; }
        public string AttachmentDate { get; set; }
        public bool Delete { get; set; }
    }
    public class DraftAttchmentFile
    {
        [Key]
        public int ID { get; set; }
        public int DraftServiceContractID { get; set; }
        [NotMapped]
        public string LineID { get; set; } = DateTime.Now.Ticks.ToString();
        public string TargetPath { get; set; }
        public string FileName { get; set; }
        public string AttachmentDate { get; set; }
        public bool Delete { get; set; }
    }
}
