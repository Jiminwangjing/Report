using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.Services.Opportunity
{
    [Table("SetupPartner")]

    public class SetupPartner
    {
        [NotMapped]
        public string LineID { get; set; } = DateTime.Now.Ticks.ToString();
        public int ID { get; set; }
        public string Name { get; set; }
        [NotMapped]
        public List<SelectListItem> DFRelationshipselect { get; set; }
        public string RelatedBp { get; set; }
        [NotMapped]
        public string RelatedBpName { get; set; }
        public int DFRelationship { get; set; }
        public string Detail { get; set; }
    }
}
