using KEDI.Core.Premise.Models.Sync;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.Services.Administrator.Setup
{
    [Table("Freight")]
    public class Freight : ISyncEntity
    {
        [Key]
        public int ID { get; set; }
        public string Name { get; set; }
        public FreightReceiptType FreightReceiptType { get; set; }
        public int RevenAcctID { get; set; }
        public int ExpenAcctID { get; set; }
        public int OutTaxID { get; set; }
        public int InTaxID { get; set; }
        public decimal AmountReven { get; set; }
        public decimal AmountExpen { get; set; }
        public bool Default { get; set; }
        public decimal OutTaxRate { get; set; }
        public decimal InTaxRate { get; set; }
        public bool IsActive { get; set; }

        //ISyncEntity
        public Guid RowId { set; get; }
        public DateTimeOffset ChangeLog { set; get; }
        public int Spk { get; set; } // Server Primary Key
        public int Cpk { get; set; } // Client Primary Key
        public bool Synced { get; set; }
        public DateTime SyncDate { get; set; }
    }
}
