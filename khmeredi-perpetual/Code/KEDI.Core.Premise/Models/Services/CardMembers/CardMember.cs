using CKBS.Models.Services.HumanResources;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KEDI.Core.Premise.Models.Services.CardMembers
{
    public enum LengthExpireCard
    {
        None = 0,
        [Display(Name = "3 Months")]
        ThreeMonths = 1,
        [Display(Name = "6 Months")]
        SixMonths = 2,
        [Display(Name = "1 Year")]
        OneYear = 3,
    }
    public class RenewCardParamsObject
    {
        public int CardID { get; set; }


        public int CusID { get; set; }
        public LengthExpireCard LengthExpireCard { get; set; }
        public string DateFrom { get; set; }
        public string DateTo { get; set; }
    }
    public class CardMember
    {
        public int ID { get; set; }
        public int TypeCardID { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        [Column(TypeName = "Date")]
        public DateTime ExpireDateFrom { get; set; }
        [Column(TypeName = "Date")]
        public DateTime ExpireDateTo { get; set; }
        public LengthExpireCard LengthExpireCard { get; set; }
        public bool Active { get; set; }
        [NotMapped]
        public BusinessPartner Customer { get; set; }
        [NotMapped]
        public decimal Discount { get; set; }
        [NotMapped]
        public TypeCardDiscountType TypeDiscount { get; set; }
    }
}
