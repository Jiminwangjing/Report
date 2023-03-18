using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace KEDI.Core.Premise.Models.Services.Inventory
{
    [Table("tbServiceItem")]
    public class ServiceItem
    {
        [Key]
        public int ID { get; set; }
        [NotMapped]
        public string LineMTID { get; set; }
        [NotMapped]
        public string LineID { get; set; }
        public int ServiceDataID { get; set; }

        public int ActivityID { get; set; }
        [NotMapped]
        public List<SelectListItem> Activitys { get; set; }

        public int HandledByID  { get; set; }
        [NotMapped]
        public string HandledByName { get; set; }
        public int TechnicianID { get; set; }
        [NotMapped]
        public string TechnicianName { get; set; }
        public DateTime StartDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public DateTime EndDate { get; set; }
        public TimeSpan EndTime { get; set; }
        public double Completed { get; set; }
        public bool Finnish { get; set; }
        public bool Acitivity { get; set; }
        public int LinkActivytyID { get; set; }
        public string ActivityName { get; set; }
        public string Remark { get; set; }
    }
}
