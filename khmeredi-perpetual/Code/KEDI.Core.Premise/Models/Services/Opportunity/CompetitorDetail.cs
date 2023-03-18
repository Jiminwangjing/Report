using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.Services.Opportunity
{
    [Table("CompetitorDetail")]

    public class CompetitorDetail
    {
        public int ID { get; set; }
        public int OpportunityMasterDataID { get; set; }
        public int NameCompetitorID { get; set; }
        //public ThreaLevel? ThreaLevel { get; set; }
        public int ThrealevelID { get; set; }   
        public string Remark { get; set; }
        [NotMapped]
        public string Name { get; set; }
        [NotMapped]
        public string Action { get; set; }
    }
    public enum ThreaLevel
    {
        Low = 1,
        Medium = 2,
        Hight = 3,

    }

}
