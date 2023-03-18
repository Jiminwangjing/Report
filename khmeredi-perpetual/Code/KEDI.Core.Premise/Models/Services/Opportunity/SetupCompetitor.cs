using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.Services.Opportunity
{
    [Table("SetupCompetitor")]

    public class SetupCompetitor
    {
        public int ID { get; set; }

        public string Name { get; set; }
        [NotMapped]
        public List<SelectListItem> ThreaLevel { get; set; }

        public int ThreaLevelID { get; set; }
        public string Detail { get; set; }
        [NotMapped]
        public string LineID { get; set; } = DateTime.Now.Ticks.ToString();
    }
    public enum ThreaLevels
    {
        Low = 1,
        Medium = 2,
        Hight = 3,

    }
}
