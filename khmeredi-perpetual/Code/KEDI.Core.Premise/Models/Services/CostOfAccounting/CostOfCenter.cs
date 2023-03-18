using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.Services.CostOfAccounting
{
    public class CostOfCenter
    {
        [Key]
        public int ID { get; set; }
        public int ParentID { get; set; }
        public int MainParentID { get; set; }
        public int OwnerEmpID { get; set; }
        public int? CostOfAccountingTypeID { get; set; }
        public int CompanyID { get; set; }
        public int Level { get; set; }
        public string OwnerName { get; set; }
        public string CostOfAccountingType { get; set; }
        [Required(ErrorMessage = ("Cost Center is require!"))]
        public string CostCenter { get; set; }
        [Required(ErrorMessage = ("Name is require!"))]
        public string Name { get; set; }
        public string ShortCode { get; set; }
        public DateTime? EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }
        public bool ActiveDimension { get; set; }
        public bool IsDimension { get; set; }
        public bool Active { get; set; } 
        public bool None { get; set; }
    }
}
