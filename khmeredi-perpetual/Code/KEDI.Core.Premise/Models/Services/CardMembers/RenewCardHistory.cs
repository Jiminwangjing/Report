using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace KEDI.Core.Premise.Models.Services.CardMembers
{
    [Table("RenewCardHistory")]
    public class RenewCardHistory
    {
        public int ID { get; set; }
        public int CardID { get; set; }
        public int CusID { get; set; }
        [Column(TypeName = "Date")]
        public DateTime LastDateExpirationFrom { get; set; }
        [Column(TypeName = "Date")]
        public DateTime LastDateExpirationTo { get; set; }
        [Column(TypeName = "Date")]
        public DateTime RenewDateFrom { get; set; }
        [Column(TypeName = "Date")]
        public DateTime RenewDateTo{ get; set; }
        public LengthExpireCard LengthExpireCard { get; set; }
    }

    public class RenewCardHistoryModel
    {
        public int ID { get; set; }
        public string CardName { get; set; }
        public string CardNumber { get; set; }
        public string CardHolderName { get; set; }
        public string LastDateExpirationFrom { get; set; }
        public string LastDateExpirationTo { get; set; }
        public string RenewDateFrom { get; set; }
        public string RenewDateTo { get; set; }
        public string LengthExpireCard { get; set; }
    }
}
