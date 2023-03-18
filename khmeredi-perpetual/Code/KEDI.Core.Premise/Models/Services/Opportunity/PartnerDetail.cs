using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.Services.Opportunity
{
    [Table("PartnerDetail")]
    public class PartnerDetail
    {
        public int ID { get; set; }
        public int OpportunityMasterDataID { get; set; }
        public int NamePartnerID { get; set; }

        public int RelationshipID { get; set; }
        public string RelatedBp { get; set; }
        public string Remark { get; set; }
    }
}
