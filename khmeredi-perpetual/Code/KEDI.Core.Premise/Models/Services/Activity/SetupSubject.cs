using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace KEDI.Core.Premise.Models.Services.Activity
{
    [Table("SetupSubject")]
    public class SetupSubject
    {
        [Key]
        [NotMapped]
        public string LineID { get; set; } = DateTime.Now.Ticks.ToString();
        public int ID { get; set; }
        public string Name { get; set; }
        public int TypeID { get; set; }
        [NotMapped]
        public List<SelectListItem> SetupType { get; set; }
        //[NotMapped]
        //public string SubName { get; set; }
        //[NotMapped]
        //public int SubID { get; set; }
    }
}
