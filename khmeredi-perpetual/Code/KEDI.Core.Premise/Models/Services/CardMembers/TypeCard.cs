using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KEDI.Core.Premise.Models.Services.CardMembers
{
    public enum TypeCardDiscountType { Rate = 0, Value = 1 }
    public class TypeCard
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public TypeCardDiscountType TypeDiscount { get; set; }
        [NotMapped]
        public string TypeDiscountName { get; set; }
        public decimal Discount { get; set; }
        public bool IsDeleted { get; set; }
    }
}
