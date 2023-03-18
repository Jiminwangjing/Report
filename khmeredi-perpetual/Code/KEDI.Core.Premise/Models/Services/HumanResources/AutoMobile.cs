using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.Services.HumanResources
{
    [Table("tbAutoMobile", Schema = "dbo")]
    public class AutoMobile
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AutoMID { get; set; }
        public int BusinessPartnerID { get; set; }
        public string Plate { get; set; }
        public string Frame { get; set; }
        public string Engine { get; set; }
        public int TypeID { get; set; }
        public int BrandID { get; set; }
        public int ModelID { get; set; }
        public int ColorID { get; set; }
        public string Year { get; set; }
        public bool Deleted { get; set; }
        public string KeyID { get; set; }
    }
}
