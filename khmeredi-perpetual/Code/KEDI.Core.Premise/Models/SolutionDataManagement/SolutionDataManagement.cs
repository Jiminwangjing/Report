using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace KEDI.Core.Premise.Models.SolutionDataManagement
{
    public enum Status { Open = 1, Confirmed = 2, Closed = 3 }
    [Table("tbSolutionDataManagement")]
    public class SolutionDataManagement
    {
        [Key]
        public int ID { get; set; }
        public string Name { get; set; }
        public int CusID { get; set; }
        public int ConTactID { get; set; }
        [Required]
        public int BranchID { get; set; }
        [Required]
        public int WarehouseID { get; set; }
        public int UserID { get; set; }
        public int SaleCurrencyID { get; set; }
        public int CompanyID { get; set; }
        public int DocTypeID { get; set; }
        public int SeriesID { get; set; }
        public int SeriesDID { get; set; }
        public string InvoiceNumber { get; set; }
        public string RefNo { get; set; }
        public string InvoiceNo { get; set; }
       
        [Column(TypeName = "Date")]
        public DateTime PostingDate { get; set; }
        [Column(TypeName = "Date")]
        public DateTime ValidUntilDate { get; set; }
        public Status Status { get; set; }
        public string Remarks { get; set; }
        public int SaleEMID { get; set; }
        public int OwnerID { get; set; }
        public int PriceListID { get; set; }
        public IEnumerable<SolutionDataManagementDetail> SolutionDataManagementDetails { get; set; }
    }
}
