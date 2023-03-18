using CKBS.Models.Services.Banking;
using CKBS.Models.Services.Inventory.PriceList;
using KEDI.Core.Premise.Models.Sync;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.Services.Administrator.General
{
    [Table("tbCompany", Schema = "dbo")]
    public class Company : ISyncEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [StringLength(50), Required(ErrorMessage = "Please input name !")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Please Select PriceList !")]
        public int PriceListID { get; set; }
        [StringLength(50), Required(ErrorMessage = "Please input location !")]
        public string Location { get; set; }
        [StringLength(220)]
        [DataType(DataType.MultilineText)]
        public string Address { get; set; }
        [StringLength(25), Required]
        public string Process { get; set; } //FIFO,Average,Standard
        public bool Delete { get; set; }
        public int SystemCurrencyID { get; set; }
        public int LocalCurrencyID { get; set; }
        //Foreign Key
        public string Logo { get; set; }
        public string Logo2 { get; set; }
        [ForeignKey("PriceListID")]
        public PriceLists PriceList { get; set; }
        [ForeignKey("SystemCurrencyID")]
        public Currency SystemCurrency { get; set; }
        [ForeignKey("LocalCurrencyID")]
        public Currency LocalCurrency { get; set; }
        public string TenantID { set; get; }


        //ISyncEntity
        public Guid RowId { set; get; }
        public DateTimeOffset ChangeLog { set; get; }
        public int Spk { get; set; } // Server Primary Key
        public int Cpk { get; set; } // Client Primary Key
        public bool Synced { get; set; }
        public DateTime SyncDate { get; set; }
    }
}
