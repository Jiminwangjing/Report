using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KEDI.Core.Premise.Models.Services.GeneralSettingAdmin
{
    public enum CardMemberOptions { Manual, Scan }
    [Table("CardMemberTemplate")]
    public class CardMemberTemplate
    {
        [Key]
        public int ID { set; get; }
        public CardMemberOptions Option { set; get; }
    }
}
