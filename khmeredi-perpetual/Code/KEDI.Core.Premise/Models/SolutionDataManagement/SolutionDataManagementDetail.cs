using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace KEDI.Core.Premise.Models.SolutionDataManagement
{
    [Table("tbSolutionDataManagementDetail")]
    public class SolutionDataManagementDetail
    {
        [Key]
        public int ID { get; set; }
        public int SolutionDataManagementID { get; set; }
        public int ItemID { get; set; }
        
        public string ItemCode { get; set; }
        public string ItemNameKH { get; set; }
        public string ItemNameEN { get; set; }
        public double Qty { get; set; }
      
        public int GUomID { get; set; }
        public int UomID { get; set; }
        public string UomName { get; set; }
       
        public double InStock { get; set; }
       
        public string Remarks { get; set; }
        public bool Delete { get; set; }

    }
}
