using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace KEDI.Core.Premise.Models.Services.Territory
{
    [Table("Territories")]
    public class Territory
    {
        [Key]
        public int ID { get; set; }
        public string Name { get; set; }
        public int LoationId { get; set; }
        public int ParentTerID { get; set; }
        public int ParentId {get;set;}
        public int Level {get;set;}
    }
}
