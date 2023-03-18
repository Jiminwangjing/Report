using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.Services.Administrator.SystemInitialization
{
    [Table("tbDocuNumbering", Schema="dbo")]
    public class DocuNumbering
    {
        [Key]
        public int ID { get; set; }   
        public string Document { get; set; }
        public string DefaultSeries { get; set; }
        public string FirstNo { get; set; }
        public string NextNo { get; set; }
        public string LastNo { get; set; }
        public int DocuTypeID { get; set; }
    }
}
