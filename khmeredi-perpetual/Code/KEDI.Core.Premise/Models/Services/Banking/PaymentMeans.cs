using CKBS.Models.Services.ChartOfAccounts;
using KEDI.Core.Premise.Models.Services.POS;
using KEDI.Core.Premise.Models.Sync;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.Services.Banking
{
    [Table("tbPaymentMeans", Schema = "dbo")]
    public class PaymentMeans : ISyncEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Required(ErrorMessage = "Please choose Account !")]
        public int AccountID { get; set; }
        [Required(ErrorMessage = "Please input name !")]
        public string Type { get; set; }
        [NotMapped]
        public List<SelectListItem> Currency { get; set; }
        public bool Status { get; set; }
        public bool Delete { get; set; }
        public bool Default { get; set; }
        public int CompanyID { get; set; }
        [NotMapped]
        public bool IsChecked { set; get; }
        [NotMapped]
        public bool IsReceivedChange { set; get; }

        [NotMapped]
        public decimal Amount { get; set; }
        [NotMapped]

        public string PMName { get; set; }
        [NotMapped]
        public int AltCurrencyID { get; set; }

        [NotMapped]
        public string GLAccName { get; set; }
        [NotMapped]
        public string GLAccCode { get; set; }
        [NotMapped]
        public List<PaymentMeans> Payment { get; set; }
        public PaymentMethod PaymentMethod { get; set; }

        //ISyncEntity
        public Guid RowId { set; get; }
        public DateTimeOffset ChangeLog { get; set; }
        public int Spk { get; set; } // Server Primary Key
        public int Cpk { get; set; } // Client Primary Key
        public bool Synced { get; set; }
        public DateTime SyncDate { get; set; }

    }
    public enum PaymentMethod
    {
        Cash = 0,
        BankTransfer = 1,
        CreditCard = 2
    }
}
