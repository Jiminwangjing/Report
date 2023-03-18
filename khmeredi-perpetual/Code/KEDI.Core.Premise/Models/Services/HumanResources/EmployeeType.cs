using System.ComponentModel.DataAnnotations.Schema;

namespace KEDI.Core.Premise.Models.Services.HumanResources
{
    public class EmployeeType
    {
        public int ID { get; set; }
        [NotMapped]
        public string LineID { get; set; }
        public string Type { get; set; }
        public bool Delete { get; set; }
        [NotMapped]
        public bool Status { get; set; }
    }
}
