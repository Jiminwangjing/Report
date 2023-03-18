using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.Services.Opportunity
{

    [Table("SetUpStage")]
    public class SetUpStage
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public float ClosingPercentTage { get; set; }
        public int StageNo { get; set; }
        [NotMapped]
        public string LineID { get; set; } = DateTime.Now.Ticks.ToString();
        [NotMapped]
        public string Action { get; set; }
        [NotMapped]
        public int OppID { get; set; }
    }
}
