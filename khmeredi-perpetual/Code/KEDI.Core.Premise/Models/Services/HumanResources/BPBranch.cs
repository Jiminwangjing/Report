using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace KEDI.Core.Premise.Models.Services.HumanResources
{
    [Table("BPBranch")]
    public class BPBranch
    {
        [Key]
        public int ID { get; set; }
        [NotMapped]
        public string LineID { get; set; } = DateTime.Now.Ticks.ToString();
        public int BusinessPartnerID { get; set; }
        public string Name { get; set; }
        public string Tel { get; set; }
        public string Address { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        public string BranchCotactPerson { get; set; }
        public string ContactTel { get; set; }
        public string ContactEmail { get; set; }
        public string GPSLink { get; set; }
        public bool SetDefualt { get; set; }
    }
}
