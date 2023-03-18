using KEDI.Core.Premise.Models.Sync;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace KEDI.Core.Premise.Models.Services.Banking
{
    [Table("TaxGroup", Schema = "dbo")]
    public class TaxGroup : ISyncEntity
    {
        [Key]
        public int ID { get; set; }
        public int GLID { get; set; }
        public int CompanyID { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public TypeTax Type { get; set; }
        public bool Delete { get; set; }
        public bool Active { get; set; }
        public List<TaxGroupDefinition> TaxGroupDefinitions { get; set; }

        //ISyncEntity
        public Guid RowId { set; get; }
        public DateTimeOffset ChangeLog { set; get; }
        public int Spk { get; set; } // Server Primary Key
        public int Cpk { get; set; } // Client Primary Key
        public bool Synced { get; set; }
        public DateTime SyncDate { get; set; }
    }

    public enum TypeTax
    {
        [Display(Name = "-- Select --")]
        None = 0,
        [Display(Name = "Output Tax")]
        InputTax = 1,
        [Display(Name = "Input Tax")]
        OutPutTax = 2
    }
}
