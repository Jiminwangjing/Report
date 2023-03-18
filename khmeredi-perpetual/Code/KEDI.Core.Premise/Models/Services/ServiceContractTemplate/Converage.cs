using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace KEDI.Core.Premise.Models.Services.ServiceContractTemplate
{

    [Table("Converage")]
    public class Converage
    {
        [Key]
        public int ID { get; set; }


        public bool Part { get; set; }
        public bool Labor { get; set; }
        public bool Travel { get; set; }
        public bool Holiday { get; set; }


        public bool Expired { get; set; }
        public bool Monthday { get; set; }
        public bool Thuesday { get; set; }
        public bool Wednesday { get; set; }
        public bool Thursday { get; set; }
        public bool Friday { get; set; }
        public bool Saturday { get; set; }
        public bool Sunday { get; set; }

        public string StarttimeMon { get; set; }
        public string StarttimeThu { get; set; }
        public string StarttimeWed { get; set; }
        public string StarttimeThur { get; set; }
        public string StarttimeFri { get; set; }
        public string StarttimeSat { get; set; }
        public string StarttimeSun { get; set; }

        public string EndtimeMon { get; set; }
        public string EndtimeThu { get; set; }
        public string EndtimeWed { get; set; }
        public string EndtimeThur { get; set; }
        public string EndtimeFri { get; set; }
        public string EndtimeSat { get; set; }
        public string EndtimeSun { get; set; }
    }
}
