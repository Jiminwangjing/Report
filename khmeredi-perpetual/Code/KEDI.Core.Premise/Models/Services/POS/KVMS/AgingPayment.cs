using CKBS.Models.Services.Account;
using CKBS.Models.Services.Administrator.General;
using CKBS.Models.Services.HumanResources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.Services.POS.KVMS
{
    [Table("tbAgingPayment", Schema = "dbo")]
    public class AgingPayment
    {
        [Key]
        public int AgingPaymentID { get; set; }
        public int UserID { get; set; }
        public int CustomerID { get; set; }
        public int BranchID { get; set; }
        public string Ref_No { get; set; }
        [Column(TypeName = "Date")]
        public DateTime PostingDate { get; set; }
        [Column(TypeName = "Date")]
        public DateTime DocumentDate { get; set; }
        public double TotalAmountDue { get; set; }
        public string Number { get; set; }
        public string Remark { get; set; }

        //VMC EDITION
        public int SeriesID { get; set; }
        public int SeriesDID { get; set; }
        public int DocTypeID { get; set; }
        //END VMC

        public List<AgingPaymentDetail> AgingPaymentDetails { get; set; }

        [ForeignKey("UserID")]
        public UserAccount UserAccount { get; set; }
        [ForeignKey("CustomerID ")]
        public BusinessPartner BusinessPartner { get; set; }
        [ForeignKey("BranchID")]
        public Branch Branch { get; set; }
        public int LocalCurID { get; set; }
        public double LocalSetRate { get; set; }
    }
}
