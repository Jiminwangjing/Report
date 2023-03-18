using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace KEDI.Core.Premise.Models.Services.CardMembers
{
    public class CardMemberDeposit
    {
        public int ID { get; set; }
        public int CusID { get; set; }
        public int CardMemberID { get; set; }
        public int UserID { get; set; }
        public int SeriesID { get; set; }
        public int SeriesDID { get; set; }
        public int DocTypeID { get; set; }
        [NotMapped]
        public string DocCode { get; set; }
        public string Number { get; set; }
        public DateTime PostingDate { get; set; }
        [NotMapped]
        public string PostingDateFormat { get; set; }
        [NotMapped]
        public string CustomerName { get; set; }
        [NotMapped]
        public string PriceListName { get; set; }
        [NotMapped]
        public string CardName { get; set; }
        public decimal TotalDeposit { get; set; }
        [NotMapped]
        public string TotalDepositF { get; set; }
    }
    public class CardMemberDepositTransaction
    {
        public int ID { get; set; }
        public int CardMemberDepositID { get; set; }
        [NotMapped]
        public string DocCode { get; set; }
        public int CusID { get; set; }
        public int CardMemberID { get; set; }
        public int UserID { get; set; }
        public int SeriesID { get; set; }
        public int SeriesDID { get; set; }
        public int DocTypeID { get; set; }
        public string Number { get; set; }
        [NotMapped]
        public string PostingDateFormat { get; set; }
        [NotMapped]
        public string CustomerName { get; set; }
        [NotMapped]
        public string PriceListName { get; set; }
        [NotMapped]
        public string CardName { get; set; }
        public DateTime PostingDate { get; set; }
        public decimal Amount { get; set; }
        public decimal CumulativeBalance { get; set; }

        [NotMapped]
        public string InAmount { get; set; }
        [NotMapped]
        public string OutAmount { get; set; }
        [NotMapped]
        public string CumulativeBalanceDisplay { get; set; }
    }
}
