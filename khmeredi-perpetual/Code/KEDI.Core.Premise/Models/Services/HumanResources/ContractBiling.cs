using KEDI.Core.Premise.Models.Sale;
using KEDI.Core.Premise.Models.Services.ServiceContractTemplate;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace KEDI.Core.Premise.Models.Services.HumanResources
{
    [Table("ContractBillingss")]
    public class ContractBiling
    {
        [Key]
        public int ID { get; set; }
        [NotMapped]
        public string LineID { get; set; } = DateTime.Now.Ticks.ToString();
        public int BusinessPartnerID { get; set; }
        public int ContractID { get; set; }

        [NotMapped]
        public string InvoiceNo { get; set; }
        [NotMapped]
        public string SumGrandTotalSys { get; set; }
        [NotMapped]
        public string SumGrandTotal { get; set; }

        public int BPID { get; set; }
        public int SaleID { get; set; }
        public int SeriesDID { get; set; }
        [NotMapped]
        public int No { get; set; }
        [NotMapped]
        public string CusName { get; set; }
        [NotMapped]
        public string CusCode { get; set; }
        public string DocumentNo { get; set; }
        public string Amount { get; set; }
        [NotMapped]
        public string ReceiptNmber { get; set; }

        [NotMapped]
        public string SGrandTotalSys { get; set; }
        public string NumExpiresOfDay { get; set; }

        [NotMapped]
        public DateTime NumExpiresOfMonth { get; set; }
        [NotMapped]
        public DateTime NumExpiresOfYear { get; set; }

        public int Status { get; set; }
        [NotMapped]
        public List<SelectListItem> Statuss { get; set; }
        public int ConfrimRenew { get; set; }
        [NotMapped]
        public List<SelectListItem> ConfrimRenews { get; set; }
        public int Payment { get; set; }
        [NotMapped]
        public List<SelectListItem> Payments { get; set; }
        public DateTime NewContractStartDate { get; set; }
        public DateTime NewContractEndDate { get; set; }
        [NotMapped]
        public string ContractStartDate { get; set; }
        [NotMapped]
        public string ContractEndDate { get; set; }
        [NotMapped]
        public string ContractRenewalDate { get; set; }
        public DateTime NextOpenRenewalDate { get; set; }
        public DateTime Renewalstartdate { get; set; }
        public DateTime Renewalenddate { get; set; }
        public DateTime TerminateDate { get; set; }
        public string ContractType { get; set; }
        [NotMapped]
        public List<SelectListItem> ContractTypes { get; set; }
        public string ContractNameTemplate { get; set; }
        public string SubContractTypeTemplate { get; set; }
        public int Activities { get; set; }
        public decimal EstimateSupportCost { get; set; }
        public string Remark { get; set; }
        public int Attachement { get; set; }
        public List<AttchmentFile> AttchmentFiles { get; set; }
        public string Path { get; set; }
        public string FileName { get; set; }
        [NotMapped]
        public string LineMID { get; set; }
    }
    public enum Status
    {
        FullActive = 1,
        NotFullActive = 2
    }
    public enum ConfirmRenew
    {
        Yes = 1,
        No = 2
    }
    public enum Payment
    {
        Paid = 1,
        NotYetPaid = 2
    }
    public enum ContractType
    {
        New = 1,
        Renewal = 2
    }
}
